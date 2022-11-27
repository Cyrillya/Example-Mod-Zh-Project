using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	// 这是一把用于展示tML各种重写函数的示例枪械，这里面的东西也可用于使用弹药的枪械
	public class ExampleSpecificAmmoGun : ModItem
	{
		public bool consumptionDamageBoost = false;

		public override string Texture => "ExampleMod/Content/Items/Weapons/ExampleGun";

		public override void SetStaticDefaults() {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() {
			// 普遍属性
			Item.width = 62; // 物品的碰撞箱宽度(像素)
			Item.height = 32; // 物品的碰撞箱高度(像素)
			Item.scale = 0.75f;
			Item.rare = ItemRarityID.Green; // 物品稀有度

			// 使用属性
			Item.useTime = 5; // 物品实际使用一次所需时间 (帧) (60帧=1秒)
			Item.useAnimation = 15; // 物品动画播放一次所需时间 (帧) (60帧=1秒)
			Item.reuseDelay = 5; // 物品动画播放完成后所等待的时间 (帧) (60帧=1秒)
			Item.useStyle = ItemUseStyleID.Shoot; // 物品的使用类型 (如挥舞，刺出等)
			Item.autoReuse = true; // 这个物品默认能不能自动挥舞
			Item.UseSound = SoundID.Item11;

			// 武器属性
			Item.DamageType = DamageClass.Ranged; // 伤害类型设置为远程
			Item.damage = 20; // 物品基础伤害，注意: 射出的射弹伤害=武器伤害+弹药伤害
			Item.knockBack = 5f; // 物品基础击退，注意: 射出的射弹击退=武器击退+弹药击退
			Item.noMelee = true; // 让这个物品的使用动画不会造成伤害 (指枪托近战怼人)

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

		// 通过这个重写函数修改武器持握在玩家手上时的位置 (让他握着枪柄而不是反重力悬空)
		public override Vector2? HoldoutOffset() {
			return new Vector2(2f, -2f);
		}

		public override void UpdateInventory(Player player) {
			// 若物品处于玩家背包，就会调用 UpdateInventory
			// 在这里我们要让这个 bool 变量设置为 false
			// 具体这个变量有什么用，请继续往下看
			consumptionDamageBoost = false;
		}

		public override bool? CanChooseAmmo(Item ammo, Player player) {
			// CanChooseAmmo 可以让弹药被选择或者拒绝，而不受到弹药类型的限制
			// (在弹药里有一个与它相同功能的重写函数 CanBeChosenAsAmmo)
			// 在默认情况下，它会返回 null，只根据你在上面写的 useAmmo 来选择弹药
			// 返回 true 将会强制使用该弹药，返回 false 将会强制无法使用该弹药
			// 在本示例中，我们将会强制阻止诅咒弹的使用，但不影响其它子弹类型的弹药
			if (ammo.type == ItemID.CursedBullet) {
				return false;
			}

			// 哦对了，我这边建议最好按照上面的说明，让它始终默认返回 null
			// 你当然可以试着让它返回 true 或 false，不过这可能会产生一些难以预料的后果
			return null;
		}

		public override bool CanConsumeAmmo(Item ammo, Player player) {
			// CanConsumeAmmo 控制弹药是否被消耗
			// (对于弹药类物品，有一个与它相同功能的重写函数 CanBeConsumedAsAmmo)
			// 默认情况下会返回 true，当它返回 false 就会阻止弹药的消耗，不论为什么
			// 注意，这并不代表返回 true 就会强制消耗弹药，像这样的功能需要 IL 或 Detour

			return player.ItemUsesThisAnimation switch {
				// 每轮发射的第一枪有20%的几率不消耗弹药
				0 => Main.rand.NextFloat() >= 0.20f,
				// 而第二枪则有63%的几率不消耗弹药
				1 => Main.rand.NextFloat() >= 0.63f,
				// 第三枪则为36%的几率
				2 => Main.rand.NextFloat() >= 0.36f,
				_ => true
			};

			// 以上的代码使用 switch 以直观反映 ItemUsesThisAnimation 对应值所指向的结果
			// 等同于以下代码 (此为官方 Example Mod 原代码)
			/*
			if (player.ItemUsesThisAnimation == 0)
				return Main.rand.NextFloat() >= 0.20f;
			else if (player.ItemUsesThisAnimation == 1)
				return Main.rand.NextFloat() >= 0.63f;
			else if (player.ItemUsesThisAnimation == 2)
				return Main.rand.NextFloat() >= 0.36f;
			 */
		}

		public override void OnConsumeAmmo(Item ammo, Player player) {
			// OnConsumeAmmo 将会在弹药被消耗时调用到
			// (对于弹药类物品，有一个与它相同功能的重写函数 OnConsumedAsAmmo)
			// 在这里，我们把一个负责控制伤害加成的bool变量设为 true
			// 这会让这把枪在消耗弹药后的下一次射击获得伤害加成
			consumptionDamageBoost = true;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			// 如果该变量为 true 则将本次伤害提高
			if (consumptionDamageBoost) {
				damage = (int)(damage * 1.2f);
			}
		}
	}
}
