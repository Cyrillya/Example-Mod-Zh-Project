using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.Localization;

namespace ExampleMod.Content.Items.Tools
{
	public class ExampleFishingRod : ModItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Example Fishing Rod");
			Tooltip.SetDefault("Fires multiple lines at once. Can fish in lava.\n" +
				"The fishing line never snaps.");
			DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "示例钓竿");
			Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "一次射出多条鱼线\n" +
				"可在熔岩中钓鱼\n" +
				"鱼线永远不断裂");

			// 可在熔岩中钓鱼
			ItemID.Sets.CanFishInLava[Item.type] = true;
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() {
			// 使用 CloneDefaults 方法从原版木钓竿中复制属性，大致为以下属性:
			// Item.width = 24;
			// Item.height = 28;
			// Item.useStyle = ItemUseStyleID.Swing;
			// Item.useAnimation = 8;
			// Item.useTime = 8;
			// Item.UseSound = SoundID.Item1;
			Item.CloneDefaults(ItemID.WoodFishingPole); // CloneDefaults 的使用将使属性设置变得简洁

			Item.fishingPole = 30; // 设置钓竿的渔力
			Item.shootSpeed = 12f; // 设置浮标发射的速度，木钓竿是 9f，金钓竿是 17f
			Item.shoot = ModContent.ProjectileType<Projectiles.ExampleBobber>(); // 钓竿射弹
		}

		// 如果持有该物品，则获得优质钓鱼线饰品的效果
		// 注意：仅在快捷栏持有时触发，如果把物品拉到物品栏外使用就不会触发
		public override void HoldItem(Player player) {
			player.accFishingLine = true;
		}

		// 覆盖 Shoot 方法以发射多个浮标
		// 注意：这将导致在有多个松露虫时召唤多个猪鲨
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			int bobberAmount = Main.rand.Next(3, 6); // 3到5个浮标
			float spreadAmount = 75f; // 浮标扩散的量值

			for (int index = 0; index < bobberAmount; ++index) {
				Vector2 bobberSpeed = velocity + new Vector2(Main.rand.NextFloat(-spreadAmount, spreadAmount) * 0.05f, Main.rand.NextFloat(-spreadAmount, spreadAmount) * 0.05f);

				// 生成浮标
				Projectile.NewProjectile(source, position, bobberSpeed, type, 0, 0f, player.whoAmI);
			}
			return false;
		}
		
		// 合成配方的创建详见 Content/ExampleRecipes.cs
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>(10)
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}