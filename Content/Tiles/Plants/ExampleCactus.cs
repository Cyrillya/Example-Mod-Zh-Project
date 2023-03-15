using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace ExampleMod.Content.Tiles
{
	// ModCactus这个类允许你自定义自己的仙人掌类植物
	public class ExampleCactus : ModCactus
	{
		public override void SetStaticDefaults() {
			// 使示例仙人掌在示例矿石上生长
			GrowsOnTileId = new int[1] { ModContent.TileType<ExampleOre>() };
		}

		public override Asset<Texture2D> GetTexture() {
			// 获得示例仙人掌的纹理
			return ModContent.Request<Texture2D>("ExampleMod/Content/Tiles/Plants/ExampleCactus");
		}

		// 如果你的仙人掌会结果实，你可以用上面GetTexture里相同的方法获得果实纹理
		// 这个例子里我们不让他长果实，直接返回null
		public override Asset<Texture2D> GetFruitTexture() {
			return null;
		}
	}
}