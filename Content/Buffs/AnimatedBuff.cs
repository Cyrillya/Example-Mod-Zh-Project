using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Buffs
{
	// 这个buff有额外的动画帧图, 同时也专门展示了PreDraw
	// (我们将自动加载的材质作为其中一帧, 以免有模组需要获取这个buff的材质而错过了)
	public class AnimatedBuff : ModBuff
	{
		// 设置一些简化操作的常量
		public const int FrameCount = 4; // 我们帧图的帧数
		public const int AnimationSpeed = 60; // 每一段帧图持续多久, 单位为帧
		public const string AnimationSheetPath = "ExampleMod/Content/Buffs/AnimatedBuff_Animation"; // 这是此buff的帧图路径

		private Asset<Texture2D> animatedTexture;

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Animated Buff");
			Description.SetDefault("Animates and increases all damage by 10%.");
			// 中文本地化见 zh-Hans.hjson

			if (Main.netMode != NetmodeID.Server) {
				// 不! 要! 在服务器上加载材质!
				animatedTexture = ModContent.Request<Texture2D>(AnimationSheetPath);
			}
		}

		public override void Unload() {
			animatedTexture = null;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams) {
			// 你可以调用此钩子来让一些特别的事情在buff图标绘制时发生 (比如画在别的地方, 使用不同的材质等等)

			// 我们绘制自己的帧图 (AnimatedBuff_Animation.png) 而不是自动加载的图标 (AnimatedBuff.png)

			// 用自己的帧图
			Texture2D ourTexture = animatedTexture.Value;
			// 选择要绘制的那一帧, 这里取决于先前设置好的常量和游戏帧数
			Rectangle ourSourceRectangle = ourTexture.Frame(verticalFrames: FrameCount, frameY: (int)Main.GameUpdateCount / AnimationSpeed % FrameCount);

			// 你还能再这个钩子里做别的事情
			/*
			// 这里让图标染上橙绿色
			drawParams.drawColor = Color.LimeGreen * Main.buffAlpha[buffIndex];
			*/

			// 注意 drawParams.mouseRectangle 仍在: 它默认是buff图标的大小
			// 它处理鼠标悬停与点击buff图标. 既然我们的每一帧也是32x32 (与自动加载的图标大小相同),
			// 我们也不修改绘制的位置 drawParams.position, 就不对此进行修改了. 如果你修改了位置或使用了非标准大小, 记得改这个

			// We have two options here:
			// 我们有两个选择:
			// 一是比较推荐的, 需要的码更少
			// 第二种更自由, 但是你得自己写了

			// For demonstration, both options' codes are written down, but the latter is commented out using /* and */.
			// 出于演示目的, 这里写了两种方法, 后者被 /* 和 */ 注释掉了

			// 选择1 - 让游戏帮我们画好, 因此我们需要确定好 drawParams 的变量:
			drawParams.Texture = ourTexture;
			drawParams.SourceRectangle = ourSourceRectangle;
			// 返回 true 让游戏把图标画出来
			return true;

			/*
			// 选择2 - 手动绘制图标:
			spriteBatch.Draw(ourTexture, drawParams.position, ourSourceRectangle, drawParams.drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			// 返回 false 以阻止游戏自身的绘制, 因为我们已经手动画过了
			return false;
			*/
		}

		public override void Update(Player player, ref int buffIndex) {
			// 所有伤害+10%
			player.GetDamage<GenericDamageClass>() += 0.1f;
		}
	}
}
