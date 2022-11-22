using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria.Localization;

namespace ExampleMod.Content.Items.Weapons
{
	public class ExampleMagicWeapon : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("This is an example magic weapon");
			Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "这是一把示例魔法武器");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() {
			Item.damage = 25;
			Item.DamageType = DamageClass.Magic; // 设置伤害类型为魔法伤害。如果你不设置它的话，那么这把武器无法吃到任何伤害加成。请确保你设置了伤害类型
			Item.width = 34;
			Item.height = 40;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot; // 将使用动作改为 Shoot
			Item.noMelee = true; // 确保你的武器本身不会攻击到敌人
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item71;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.BlackBolt; // 让这把武器发射玛瑙爆破枪所发射的射弹
			Item.shootSpeed = 7; //射弹的速度 (像素/帧) (比如这里是每帧7像素，也就是420像素每秒，即26.25物块每秒)
			Item.crit = 32; // 武器的暴击率 (不包括默认4%暴击率) (游戏内的无加成情况下暴击率应为36%)
			Item.mana = 11; // 使用该武器所需魔力
		}

		// 这里写的是合成配方，合成配方在 Content/ExampleRecipes.cs 有更详尽的介绍
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}
