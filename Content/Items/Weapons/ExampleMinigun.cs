using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	public class ExampleMinigun : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("This is a modded minigun.");
			Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "这是一把加特林");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() {
			// 普遍属性
			Item.width = 54; // 物品的碰撞箱宽度(像素)
			Item.height = 22; // 物品的碰撞箱高度(像素)
			Item.rare = ItemRarityID.Green; // 物品稀有度

			// 使用属性
			Item.useTime = 5; // 物品实际使用一次所需时间 (帧) (60帧=1秒)
			Item.useAnimation = 5; // 物品动画播放一次所需时间 (帧) (60帧=1秒)
			Item.useStyle = ItemUseStyleID.Shoot; // 物品的使用类型
			Item.autoReuse = true; // 这个物品默认能不能自动挥舞
			Item.UseSound = SoundID.Item11; // 物品被使用时播放的声音

			// 武器属性
			Item.DamageType = DamageClass.Ranged; // 伤害类型设置为远程
			Item.damage = 11; // 物品基础伤害，注意: 射出的射弹伤害=武器伤害+弹药伤害
			Item.knockBack = 1f; // 物品基础击退，注意: 射出的射弹击退=武器击退+弹药击退
			Item.noMelee = true; // 让这个物品的使用动画不会造成伤害 (指用一把巨重的加特林活生生砸死对面)

			// 枪属性
			Item.shoot = ProjectileID.PurificationPowder; // 出于某种原因，原版枪的 Item.shoot 都是这么设置的，实际射弹基于弹药以及 Shoot() 相关代码
			Item.shootSpeed = 16f; // 射弹的速度 (像素/帧) (比如这里是每帧16像素，也就是960像素每秒，即60物块每秒)
			Item.useAmmo = AmmoID.Bullet; // 物品使用的弹药类型ID，用 AmmoID.XX 来选一个原版弹药类型，这里是所有子弹的意思
		}

		// 这里写的是合成配方，合成配方在 Content/ExampleRecipes.cs 有更详尽的介绍
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}

		// 让这把枪有38%的几率不消耗弹药
		public override bool CanConsumeAmmo(Item ammo, Player player) {
			return Main.rand.NextFloat() >= 0.38f;
		}

		// 这个重写函数可以让枪在没有弹药的时候射击
		// 本示例中写的是：当玩家背包内拥有十个 ExampleItem 即可在没有弹药的情况下射击
		// 然后，枪就会像使用默认弹药一样射击，本示例中默认弹药为火枪子弹
		public override bool NeedsAmmo(Player player) {
			return player.CountItem(ModContent.ItemType<ExampleItem>(), 10) < 10;
		}

		// 在这里改变枪的射击方向来让它变得不太精准 (就像原版那样)
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			velocity = velocity.RotatedByRandom(MathHelper.ToRadians(10));
		}

		// 通过这个重写函数修改武器持握在玩家手上时的位置 (让他握着枪柄而不是反重力悬空)
		public override Vector2? HoldoutOffset() {
			return new Vector2(-6f, -2f);
		}
	}
}
