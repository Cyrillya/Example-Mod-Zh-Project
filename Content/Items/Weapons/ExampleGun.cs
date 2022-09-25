using ExampleMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	public class ExampleGun : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("This is a modded gun.");
			Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "这是一把枪");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() {
			// 普遍属性
			Item.width = 62; // 物品的碰撞箱宽度(像素)
			Item.height = 32; // 物品的碰撞箱高度(像素)
			Item.scale = 0.75f;
			Item.rare = ItemRarityID.Green; // 物品稀有度

			// 使用属性
			Item.useTime = 8; // 物品实际使用一次所需时间 (帧) (60帧=1秒)
			Item.useAnimation = 8; // 物品动画播放一次所需时间 (帧) (60帧=1秒)
			Item.useStyle = ItemUseStyleID.Shoot; // 物品的使用类型
			Item.autoReuse = true; // 这个物品默认能不能自动挥舞
			
			// 物品被使用时播放的声音
			Item.UseSound = new SoundStyle($"{nameof(ExampleMod)}/Assets/Sounds/Items/Guns/ExampleGun") {
				Volume = 0.9f,
				PitchVariance = 0.2f,
				MaxInstances = 3,
			};

			// 武器属性
			Item.DamageType = DamageClass.Ranged; // 伤害类型设置为远程
			Item.damage = 20; // 物品基础伤害，注意: 射出的射弹伤害=武器伤害+弹药伤害
			Item.knockBack = 5f; // 物品基础击退，注意: 射出的射弹击退=武器击退+弹药击退
			Item.noMelee = true; // 让这个物品的使用动画不会造成伤害 (指拿枪杆子打人)

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
		
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			// 由这把枪射出的射弹有 1/3 的概率变成 ExampleInstancedProjectile
			if (Main.rand.NextBool(3)) {
				type = ModContent.ProjectileType<ExampleInstancedProjectile>();
			}
		}

		/*
		 * 你可以取消某个注释掉的代码段来看看效果是什么
		 * 注意把上面已有的 ModifyShootStats 注释掉，VS快捷键为选择后按下 Ctrl+K 与 Ctrl+/
		 */

		// 和乌兹冲锋枪一样，将普通子弹替换成高速子弹
		/*public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			if (type == ProjectileID.Bullet) { // 注意是 ProjectileID 而不是别的什么 ItemID
				type = ProjectileID.BulletHighVelocity;
			}
		}*/
		
		// 和吸血鬼刀一样，圆弧式随机角度扩散
		/*public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			// 方法 Main.rand.Next(N) 返回一个范围在 [0, N-1] 范围内的随机整数，即 Main.rand.Next(3) 即为 0, 1, 2 中随机一个整数
			// 因此 numberProjectiles 为范围在 [3, 5] 内的随机整数
			float numberProjectiles = 3 + Main.rand.Next(3);
			float rotation = MathHelper.ToRadians(45); // MathHelper.ToRadians(N) 将角度制 N° 转换为对应的弧度制值

			position += Vector2.Normalize(velocity) * 45f;

			for (int i = 0; i < numberProjectiles; i++) {
				Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f;
				Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
			}

			return false; // 返回 false 以防止原版的 Projectile.NewProjectile 代码运行导致多生成一个射弹
		}*/

		// 让子弹准确地从枪口处出现，并且子弹不会因枪口太长而穿墙
		// 默认情况下使用物品的射弹都是从玩家中心出现的
		/*public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			Vector2 muzzleOffset = Vector2.Normalize(velocity) * 25f; // 25是枪杆的长度 (像素)

			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) {
				position += muzzleOffset;
			}
		}*/

		// 发条式突击步枪的效果: 一轮三连发，只消耗一发子弹
		// 两轮连发之间可以通过 reuseDelay 设置间隔 (即发射一轮后，需经过多少帧才能进行下一轮连发）
		// 在 https://terraria.wiki.gg/zh/wiki/%E5%8F%91%E6%9D%A1%E5%BC%8F%E7%AA%81%E5%87%BB%E6%AD%A5%E6%9E%AA 有部分介绍
		// 将下列更改写到 SetDefaults() 中:
		/*
			Item.useAnimation = 12;
			Item.useTime = 4; // useAnimation 的 1/3 (也就是一次动画中实际包含三次使用)
			Item.reuseDelay = 14;
			Item.consumeAmmoOnLastShotOnly = true;
		*/

		// 同时发射两发不同的射弹
		/*public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			// 生成第二发射弹，并将射弹ID设置为我们想要的某个射弹
			Projectile.NewProjectile(source, position, velocity, ProjectileID.GrenadeI, damage, knockback, player.whoAmI);

			// 返回 true 即执行原版发射行为，即生成一个基于使用的弹药的射弹
			return true;
		}*/
		
		// 从多个不同的射弹中随机选择一个
		/*public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			// 从以下射弹中随机选一个: 原射弹 (基于使用的弹药)、金子弹和模组自制的一个射弹
			type = Main.rand.Next(new int[] { type, ProjectileID.GoldenBullet, ModContent.ProjectileType<Projectiles.ExampleBullet>() });
		}*/
	}
}
