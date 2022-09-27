using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Content.Rarities
{
	public class ExampleHigherTierModRarity : ModRarity
	{
		// 该稀有度的颜色
		public override Color RarityColor => new Color(Main.DiscoR / 2, (byte)(Main.DiscoG / 1.25f), (byte)(Main.DiscoB / 1.5f));

		// 修饰词条可以影响稀有度
		public override int GetPrefixedRarity(int offset, float valueMult) {
			if (offset < 0) { // 如果修正值是-1或-2（负面的修饰词条）
				return ModContent.RarityType<ExampleModRarity>(); // 使物品的稀有度在有负面修饰词条时的变成更低的一个
			}

			return Type; // 没有更高级的稀有度去升了，所以就返回这个稀有度的Type
		}
	}
}
