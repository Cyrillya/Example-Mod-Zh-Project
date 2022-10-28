using ExampleMod.Content.Tiles.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
// using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Accessories
{
	[AutoloadEquip(EquipType.Shield)] // 被装备时, 以"盾"的形式加载你的贴图.
	public class ExampleShield : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("这是一面模组盾.");

			//CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
			SacrificeTotal = 1; // 上一行与这一行的效果是一样的, 旅途研究所需数量, 只是下面这个更简洁, 还不需要 using Terraria.GameContent.Creative;
		}

		public override void SetDefaults() {
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(10);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true; // 是个饰品

			Item.defense = 1000;
			Item.lifeRegen = 10;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) {
			player.GetDamage(DamageClass.Generic) += 1f; // 所有伤害 +100%
			player.endurance = 1f - (0.1f * (1f - player.endurance));  // 伤害减免的百分比, 你可以画出它的函数图像来看看是什么效果
			player.GetModPlayer<ExampleDashPlayer>().DashAccessoryEquipped = true; // 详见本文件中的 ExampleDashPlayer
		}

		// 合成配方的创建详见 Content/ExampleRecipes.cs
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<ExampleWorkbench>()
				.Register();
		}
	}

	public class ExampleDashPlayer : ModPlayer // 这里是一个简单的 冲! 刺! 是的, 就是这里
	{
		// 这些常量表示计时器数组 (详见下) 中不同的方向
		// 这些数字对应的方向键 (Player.doubleTapCardinalTimer 中元素的序号) 是固定的
		public const int DashDown = 0; // 下
		public const int DashUp = 1; // 上
		public const int DashRight = 2; // 右
		public const int DashLeft = 3; // 左

		public const int DashCooldown = 50; // 冲刺冷却 (帧). 如果小于 DashDuration, 你甚至可以在第一次冲刺结束前就开始第二次冲刺
		public const int DashDuration = 35; // 冲刺的持续时间 (帧)

		// 冲刺的初速度, 10f 大约是 37.5格每秒 或 50 mph
		public const float DashVelocity = 10f;

		// 玩家双击的方向键的方向, 默认为-1表示没有双击
		public int DashDir = -1;

		// 与对应冲刺饰品相关的字段
		public bool DashAccessoryEquipped; // 用于判断是否装备了对应的冲刺饰品 (否则就不应触发这个冲刺)
		public int DashDelay = 0; // 冲刺冷却完成所需的剩余时间 (帧)
		public int DashTimer = 0; // 冲刺的剩余时间 (帧)

		// 请注意, 不是说你定义了上面这些量, 冲刺就自动好了, 请看后面的实现

		public override void ResetEffects() {
			// 重置代表装备了对应饰品的标记 (下面这个布尔值). 如果对应饰品被装备了, ExampleShield.UpdateAccessory 将会被其调用 (详见上) 并在 PreUpdateMovement 前设置此标记
			DashAccessoryEquipped = false;

			// ResetEffects 在 player.doubleTapCardinalTimer (双击间隔时间) 的值被设定后不久被调用
			// 当某个方向键被按下再松开时, 原版会启动一个15帧 (四分之一秒) 的计时器, 期间再次按下同一方向键可以激活冲刺
			// 如果计时器的值是15, 这意味着方向键刚刚被第一次按下, 否则就是"双击"了
			if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[DashDown] < 15) { // 如果刚刚按下某方向键, 现在松开了它, 且是第二次点击
				DashDir = DashDown; // 设定此次冲刺的方向
			}
			else if (Player.controlUp && Player.releaseUp && Player.doubleTapCardinalTimer[DashUp] < 15) {
				DashDir = DashUp;
			}
			else if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15) {
				DashDir = DashRight;
			}
			else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15) {
				DashDir = DashLeft;
			}
			else {
				DashDir = -1;
			}
		}

		// 这个函数是最适合施加冲刺的地方, 它在原版移动之后, 玩家的位置基于速度变化之前被调用
		// 如果这一帧双击了, 那这一帧就开冲
		public override void PreUpdateMovement() {
			// 如果玩家可以用该冲刺 (CanDash方法见下), 双击了某一方向键, 且该冲刺的冷却已结束
			if (CanUseDash() && DashDir != -1 && DashDelay == 0) {
				Vector2 newVelocity = Player.velocity;

				switch (DashDir) {
					// 仅在当前轴向速度小于冲刺初速度的情况下开冲 (不然反而减速了)
					case DashUp when Player.velocity.Y > -DashVelocity: // "为啥这里不要写?" 答: 看看switch的语法
					case DashDown when Player.velocity.Y < DashVelocity: {
							// 在此设置Y方向 (纵向) 的速度
							// 如果是向上冲, 那么就将初速度调高一点以对抗重力
							// 大约需要1.3倍速度来对抗重力
							float dashDirection = DashDir == DashDown ? 1 : -1.3f;
							newVelocity.Y = dashDirection * DashVelocity;
							break;
						}
					case DashLeft when Player.velocity.X > -DashVelocity:
					case DashRight when Player.velocity.X < DashVelocity: {
							// 在此设置X方向 (横向) 的速度
							float dashDirection = DashDir == DashRight ? 1 : -1;
							newVelocity.X = dashDirection * DashVelocity;
							break;
						}
					default:
						return; // 你跑得比冲刺还快, 不冲了
				}

				DashDelay = DashCooldown; // 开始冷却
				DashTimer = DashDuration; // 开始记录冲刺时间
				Player.velocity = newVelocity; // 开冲

				// 你还可以在这里为该冲刺加一些炫酷的效果
				// 例如: 忍者冲刺时的烟雾效果
			}

			if (DashDelay > 0)
				DashDelay--; // 冲刺冷却读条

			if (DashTimer > 0) { // 冲刺的剩余时间大于0, 意味着正在冲刺
				// 下面两行设置了残影效果, 你当然可以将它们换成任何你想要在冲刺时触发的效果
				// 例如: 生成Dust, 加buff或给无敌帧
				// 这里我们利用 "player.eocDash" 和 "player.armorEffectDrawShadowEOCShield" 给玩家上克盾冲刺的残影效果
				Player.eocDash = DashTimer;
				Player.armorEffectDrawShadowEOCShield = true;

				// 冲刺剩余时间读条
				DashTimer--;
			}
		}

		private bool CanUseDash() { // 判断能不能用该冲刺的方法
			return DashAccessoryEquipped
				&& Player.dashType == 0 // 玩家没有忍者或克盾冲刺 (优先让它们冲)
				&& !Player.setSolar // 玩家没有穿日耀套
				&& !Player.mount.Active; // 玩家没在坐骑上, 因为一边骑一边冲真的很怪欸
		}
	}
}
