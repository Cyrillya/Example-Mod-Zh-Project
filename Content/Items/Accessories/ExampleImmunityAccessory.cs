using ExampleMod.Common.Players;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Accessories
{
	public class ExampleImmunityAccessory : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("这是个将你的无敌帧延长一秒的模组项链.");
			SacrificeTotal = 1; // 旅途研究所需数量
		}

		public override void SetDefaults() {
			Item.width = 26;
			Item.height = 32;
			Item.maxStack = 1;
			Item.value = Item.sellPrice(0, 1);
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) {
			// 将bool值 HasExampleImmunityAcc 设为true
			// 然后在 ModPlayer.PostHurt 中写效果 (去看ExampleImmunityPlayer类中对这个值的操作)
			player.GetModPlayer<ExampleImmunityPlayer>().HasExampleImmunityAcc = true;
		}
	}
}
