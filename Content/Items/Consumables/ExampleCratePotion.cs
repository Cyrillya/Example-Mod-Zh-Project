using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace ExampleMod.Content.Items.Consumables
{
	public class ExampleCratePotion : ModItem
	{
		public override void SetStaticDefaults() {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 20;

			// 该物品的使用类型为DrinkLiquid时，会出现下面这些颜色的粒子
			ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
				new Color(240, 240, 240),
				new Color(200, 200, 200),
				new Color(140, 140, 140)
			};
		}

		public override void SetDefaults() {
			Item.width = 20;
			Item.height = 26;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useAnimation = 15;
			Item.useTime = 15;
			Item.useTurn = true;
			Item.UseSound = SoundID.Item3;
			Item.maxStack = 30;
			Item.consumable = true;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(silver: 8);
			Item.buffType = ModContent.BuffType<Buffs.ExampleCrateBuff>(); // 指定使用该时会获得的buff
			Item.buffTime = 3 * 60 * 60; // 在这里可以设置buff的持续时间，让我们设置成3分钟。注意：单位为帧（60帧为1秒，所以要写成60 * 60 * 3）
		}

		// 这里写的是合成配方，合成配方在 Content/ExampleRecipes.cs 有更详尽的介绍
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.CratePotion, 4)
				.AddTile(TileID.CrystalBall)
				.Register();
		}
	}
}
