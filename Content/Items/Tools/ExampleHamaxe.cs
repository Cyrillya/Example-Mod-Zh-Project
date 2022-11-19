using ExampleMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Tools
{
	public class ExampleHamaxe : ModItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Example Hamaxe");
			DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "示例锤斧");
			Tooltip.SetDefault("This is a modded hamaxe.");
			Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "这是一个模组锤斧");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() {
			Item.damage = 25;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true; // 自动挥舞，详见: https://terraria.wiki.gg/zh/wiki/%E8%87%AA%E5%8A%A8%E6%8C%A5%E8%88%9E

			Item.axe = 30; // 斧力，注意游戏中显示的斧力是这个值乘以5
			Item.hammer = 100; // 武器有多少锤力
		}

		public override void MeleeEffects(Player player, Rectangle hitbox) {
			if (Main.rand.NextBool(10)) { // 挥舞动画进行时每帧都有 10% 概率生成粒子效果
				// 在武器碰撞箱处创建一个粒子
				Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<Sparkle>());
			}
		}
		
		// 合成配方的创建详见 Content/ExampleRecipes.cs
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}
