using ExampleMod.Content.Tiles.Furniture;
using Terraria.ID;
// using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Ammo
{
	public class ExampleBullet : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("这是一发模组子弹.");

			// CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
			SacrificeTotal = 99; // 上一行与这一行的效果是一样的, 旅途研究所需数量, 只是下面这个更简洁, 还不需要 using Terraria.GameContent.Creative;
		}

		public override void SetDefaults() {
			Item.damage = 12; // 打出去的子弹的伤害实际上是枪的伤害加弹药的伤害
			Item.DamageType = DamageClass.Ranged; // 记得写伤害类型
			Item.width = 8;
			Item.height = 8;
			Item.maxStack = 999;
			Item.consumable = true; // 将该物品标记为消耗品, 如果允许的话, 使其在作为弹药或随便什么被使用的时候消耗
			Item.knockBack = 1.5f;
			Item.value = 10;
			Item.rare = ItemRarityID.Green;
			Item.shoot = ModContent.ProjectileType<Projectiles.ExampleBullet>(); // 以此物品为弹药时所发射的射弹
			Item.shootSpeed = 16f; // 射弹初速
			Item.ammo = AmmoID.Bullet; // 该弹药所属的类型, 此处是子弹
		}

		// 合成配方的创建详见 Content/ExampleRecipes.cs
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<ExampleWorkbench>()
				.Register();
		}
	}
}
