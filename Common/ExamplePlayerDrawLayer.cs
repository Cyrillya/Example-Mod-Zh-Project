using ExampleMod.Content.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace ExampleMod.Common
{
	public class ExamplePlayerDrawLayer : PlayerDrawLayer
	{
		private Asset<Texture2D> exampleItemTexture;

		// 在这个重写属性中返回true以让此绘制层出现在玩家的小地图头像中
		public override bool IsHeadLayer => true;

		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
			// 此绘制成只有在玩家手中拿着ExampleItem时才会可见。如果别的Mod强制使其可见的话也会显示，不过不用管别人。
			return drawInfo.drawPlayer.HeldItem?.type == ModContent.ItemType<ExampleItem>();

			// 如果你想要引用另一个绘制层的可见性
			// 你可以先用 ModContent.GetInstance<另一个可见层>() 方法获取对应的实例
			// 然后调用其 GetDefaultVisiblity 方法
		}

		// 该层将会是头部绘制层的子层，并在其绘制前绘制（即此绘制层会被头部层覆盖在下面）
		// 作为头部绘制层的子层:
		// 若头部层被隐藏了，这个绘制层也会一起隐藏
		// 若头部层移动了，这个绘制层也会跟着他一起移动
		public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Head);
		// 如果你想要一个不是任何层的子层的绘制层，可以用 “new Between(Layer1, Layer2)” 来指定其位置
		// 如果你想制作一个可以根据drawInfo在不同绘制层绘制的“移动”的图层，可以用 “Multiple”

		protected override void Draw(ref PlayerDrawSet drawInfo) {
			// 下面的代码在玩家的头后面绘制出了ExampleItem的贴图

			if (exampleItemTexture == null) {
				exampleItemTexture = ModContent.Request<Texture2D>("ExampleMod/Content/Items/ExampleItem");
			}

			var position = drawInfo.Center + new Vector2(0f, -20f) - Main.screenPosition;
			position = new Vector2((int)position.X, (int)position.Y); // 将位置取整以避免贴图抖动

			// 将绘制加入队列，请不要在玩家绘制层中使用 SpriteBatch 绘制
			drawInfo.DrawDataCache.Add(new DrawData(
				exampleItemTexture.Value, // 要绘制的贴图
				position, // 贴图显示的位置
				null, // 源矩形（用于将贴图中的某一部分单独出来绘制，null即绘制整个贴图）
				Color.White, // 颜色
				0f, // 贴图旋转（弧度制）
				exampleItemTexture.Size() * 0.5f, // 锚点。请使用贴图中心坐标
				1f, // 缩放尺寸
				SpriteEffects.None, // SpriteEffect
				0 // “图层”。实际上并没有用，在泰拉中的绘制一般都是填0
			));
		}
	}
}
