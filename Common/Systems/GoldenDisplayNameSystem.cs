using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour.HookGen;
using ReLogic.Content;
using System;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ExampleMod.Common.Systems;

/// <summary>
/// 这个类演示了如何使用 <seealso cref="HookEndpointManager"/> 对tML的类挂RuntimeDetour,
/// 以配合Shader使模组列表内Mod名称金黄变色，并且带有滚动颜色特效<br/>
/// Shader的fx文件在: Assets/Effects/Golden.fx<br/>
/// Golden调色板文件在: Assets/Textures/Shader/Golden<br/>
/// 这个类不是原ExampleMod具有的，而是汉化组成员Cyril塞的私货（
/// 裙中世界教程: https://fs49.org/2022/11/20/%e9%80%9a%e8%bf%87-hookendpointmanager-%e5%8a%a8%e6%80%81%e6%8c%82%e9%92%a9%e5%ad%90/
/// </summary>
public class GoldenDisplayNameSystem : ModSystem
{
	// 由于反复反射开销较大且可读性低，而实际使用中又需要多多用到反射，所以这里存储两个变量
	private static Type _uiModItemType;
	private static MethodInfo _drawMethod;

	// 用于滚动颜色效果
	private static RenderTarget2D _renderTarget;

	public override void Load() {
		// 服务器上是没UI的，自然没必要挂
		if (Main.dedServ) {
			return;
		}

		// 在主线程上运行，否则会报错
		Main.QueueMainThreadAction(() => {
			// 文字内容与长宽
			string text = Mod.DisplayName + " v" + Mod.Version;
			var size = ChatManager.GetStringSize(FontAssets.MouseText.Value, text, Vector2.One).ToPoint();

			// 实例化 RenderTarget，这里 width 和 height 使用 size 的值就行了
			_renderTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, size.X, size.Y);

			Main.spriteBatch.Begin();

			// 设置 RenderTarget 为我们的
			Main.graphics.GraphicsDevice.SetRenderTarget(_renderTarget);
			Main.graphics.GraphicsDevice.Clear(Color.Transparent);
			
			// 绘制字，注意别写成带描边的了，不然整个字就糊了
			ChatManager.DrawColorCodedString(Main.spriteBatch, FontAssets.MouseText.Value, text, Vector2.Zero,
				Color.White, 0f, Vector2.Zero, Vector2.One);

			// 还原
			Main.spriteBatch.End();
			Main.graphics.GraphicsDevice.SetRenderTarget(null);
		});
		
		// 由于原版中 UIModItem 类是内部(internal)的，无法直接使用 typeof(UIModItem) 获取其 Type 实例
		// 所以我们要通过从程序集中寻找的方法来获取
		// typeof(Main).Assembly 是原版的程序集，调用其 GetTypes() 方法并找到第一个名为 UIModItem 的 Type
		// 即可获取到我们想要修改的类的 Type 实例
		_uiModItemType = typeof(Main).Assembly.GetTypes().First(t => t.Name == "UIModItem");

		// 接下来反射获取我们要修改的方法
		_drawMethod = _uiModItemType.GetMethod("Draw", BindingFlags.Instance | BindingFlags.Public);

		// 若方法获取成功(一般来说，只要写得没问题都是成功的)，则调用 HookEndpointManager.Add 添加 RuntimeDetour
		if (_drawMethod is not null) {
			// Add 相当于添加 On 命名空间的委托
			HookEndpointManager.Add(_drawMethod, DrawHook);
		}
	}

	public override void Unload() {
		// 上面已经排除了服务器情况，这里服务器肯定是没挂上的
		if (Main.dedServ) {
			return;
		}

		// 防御性编程 - 确保是添加了委托才 Remove
		if (_drawMethod is not null) {
			// 使用 HookEndpointManager.Remove 卸载
			HookEndpointManager.Remove(_drawMethod, DrawHook);
		}

		if (_renderTarget is not null) {
            _renderTarget = null;
		}
	}
	
	// 这个委托表示原方法，包含一个类实例，以及相应方法的传入参数
	// UIModItem 是内部的，无法直接获取，这里可直接用 object 替代
	public delegate void DrawDelegate(object uiModItem, SpriteBatch sb);
	
	// 等同于一个 On 命名空间委托，在里面按照 On 的逻辑写就行了
	private void DrawHook(DrawDelegate orig, object uiModItem, SpriteBatch sb) {
		// 一定要记得调用原方法，不然你UI就没了
		orig.Invoke(uiModItem, sb);

		// RenderTarget为null，自然不需要进行了，因为不可能绘制出来
		if (_renderTarget is null) {
			return;
		}

		// 反射获取 _modName，后面修改以及绘制需要用到
		// 找不到 _modName 就报错
		if (_uiModItemType.GetField("_modName", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(uiModItem) is not UIText modName) {
			throw new Exception("出错啦!");
		}

		// 确保是修改自己Mod的名字 (不过你想改别的我也不拦你)
		if (!modName.Text.Contains(Mod.DisplayName)) {
			return;
		}

		// 加载所需的资源
		var texture = ModContent.Request<Texture2D>("ExampleMod/Assets/Textures/Shader/Golden");
		var shader = ModContent.Request<Effect>("ExampleMod/Assets/Effects/Golden", AssetRequestMode.ImmediateLoad).Value;

		// 传入 Shader 所需的参数
		shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.25f);
		Main.instance.GraphicsDevice.Textures[1] = texture.Value; // 传入调色板

		// 为什么y要-2呢？因为原版就这么写的。金色字实现的原理实际上是覆盖原版，所以要保证重合
		var position = modName.GetDimensions().Position() - new Vector2(0f, 2f);
		

		// 重新开启 SpriteBatch 以应用 Shader
		sb.End();
		// 这个Begin传参可以确保无关参数不被修改，以避免奇怪的错误
		// (而且很方便，不用去找原版都用了哪些参数)
		sb.Begin(SpriteSortMode.Immediate, sb.GraphicsDevice.BlendState, sb.GraphicsDevice.SamplerStates[0],
			sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, shader, Main.UIScaleMatrix);

		// 将 _renderTarget 进行绘制
		sb.Draw(_renderTarget, position, Color.White);
		// 如果不使用 _renderTarget，就用一般的绘制字符串方法吧
		// ChatManager.DrawColorCodedString(sb, FontAssets.MouseText.Value, modName.Text, position, Color.White, 0f, Vector2.Zero, Vector2.One);
		
		// 重新开启 SpriteBatch 以去除 Shader
		sb.End();
		sb.Begin(SpriteSortMode.Deferred, sb.GraphicsDevice.BlendState, sb.GraphicsDevice.SamplerStates[0],
			sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);
	}
}