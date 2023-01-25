using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	public class ExampleShotgun : ModItem
	{
		public override void SetStaticDefaults() {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() {
			// 普遍属性
			Item.width = 44; // 物品的碰撞箱宽度(像素)
			Item.height = 18; // 物品的碰撞箱高度(像素)
			Item.rare = ItemRarityID.Green; // 物品稀有度

			// 使用属性
			Item.useTime = 55; // 物品实际使用一次所需时间 (帧) (60帧=1秒)
			Item.useAnimation = 55; // 物品动画播放一次所需时间 (帧) (60帧=1秒)
			Item.useStyle = ItemUseStyleID.Shoot; // 物品的使用类型
			Item.autoReuse = true; // 这个物品默认能不能自动挥舞
			Item.UseSound = SoundID.Item36; // 物品被使用时播放的声音

			// 武器属性
			Item.DamageType = DamageClass.Ranged; // 伤害类型设置为远程
			Item.damage = 10; // 物品基础伤害，注意: 射出的射弹伤害=武器伤害+弹药伤害
			Item.knockBack = 6f; // 物品基础击退，注意: 射出的射弹击退=武器击退+弹药击退
			Item.noMelee = true; // 让这个物品的使用动画不会造成伤害 (∵嘴炮是近战伤害且喷子=嘴炮，∴喷子是近战伤害)

			// 枪属性
			Item.shoot = ProjectileID.PurificationPowder; // 出于某种原因，原版枪的 Item.shoot 都是这么设置的，实际射弹基于弹药以及 Shoot() 相关代码
			Item.shootSpeed = 10f; // 射弹的速度 (像素/帧) (比如这里是每帧16像素，也就是960像素每秒，即60物块每秒)
			Item.useAmmo = AmmoID.Bullet; // 物品使用的弹药类型ID，用 AmmoID.XX 来选一个原版弹药类型，这里是所有子弹的意思
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			const int NumProjectiles = 8; // 这把枪在每次射击时所发射出的子弹数量

			for (int i = 0; i < NumProjectiles; i++) {
				// 让发射的子弹拥有随机的旋转角度。这里是上下各15°，总计30°
				Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));

				// 随机降低子弹的速度以达到更好的视觉效果
				newVelocity *= 1f - Main.rand.NextFloat(0.3f);

				// 创建射弹
				Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
			}

			return false; // 返回flase以防止tModLoader发射默认的射弹
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
			return new Vector2(-2f, -2f);
		}
	}
}
