using Terraria;
// using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Ammo
{
	// 该物品向你展示如何自定义一个弹药类型
	// 使用于 ExampleCustomAmmoGun
	public class ExampleCustomAmmo : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("穿墙追踪敌人");

			// CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
			SacrificeTotal = 99; // 上一行与这一行的效果是一样的, 旅途研究所需数量, 只是下面这个更简洁, 还不需要 using Terraria.GameContent.Creative;
		}

		public override void SetDefaults() {
			Item.width = 14; // 碰撞箱的宽
			Item.height = 14; // 碰撞箱的高

			Item.damage = 8; // // 打出去的射弹的伤害实际上是发射器的伤害加弹药的伤害
			Item.DamageType = DamageClass.Ranged; // 记得写伤害类型, 此处是远程

			Item.maxStack = 999; // 最大堆叠数
			Item.consumable = true; // // 将该物品标记为消耗品, 如果允许的话, 使其在作为弹药或随便什么被使用的时候消耗, 如果允许的话
			Item.knockBack = 2f; // 击退, 射弹的击退是弹药和发射器的和
			Item.value = Item.sellPrice(0, 0, 1, 0);
			Item.rare = ItemRarityID.Yellow; // 稀有度颜色
			Item.shoot = ModContent.ProjectileType<Projectiles.ExampleHomingProjectile>(); // 以此物品为弹药时所发射的射弹

			Item.ammo = Item.type; // 重要! 将此种弹药的第一个物品的type设为该种弹药的类型(AmmoID)
		}

		// 合成配方的创建详见 Content/ExampleRecipes.cs
		public override void AddRecipes() {
			CreateRecipe(999) // 产出999个
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}
