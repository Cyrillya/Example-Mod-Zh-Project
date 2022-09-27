using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace ExampleMod.Content.Rarities
{
	public class ExampleModRarity : ModRarity
	{
		// 该稀有度的颜色
		public override Color RarityColor => new Color(200, 215, 230);

		// 修饰词条可以影响稀有度
		public override int GetPrefixedRarity(int offset, float valueMult) {
			if (offset > 0) { // 如果修正值是1或2（正面的修饰词条）
				return ModContent.RarityType<ExampleHigherTierModRarity>(); // 使物品的稀有度在有正面修饰词条时的变成更高的一个
			}

			return Type; // 没有更低级的稀有度去降了，所以就返回这个稀有度的Type
		}
	}
}
