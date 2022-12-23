using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Tools
{
	// 一些钩爪的概念可以看Wiki: https://terraria.wiki.gg/zh/wiki/%E9%92%A9%E7%88%AA
	internal class ExampleHookItem : ModItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Example Hook");
			DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "示例钩爪");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() {
			// 从原版紫晶钩爪复制基本属性
			Item.CloneDefaults(ItemID.AmethystHook);
			Item.shootSpeed = 18f; // 钩爪的射出速度
			Item.shoot = ModContent.ProjectileType<ExampleHookProjectile>(); // 使用时射出自定义的钩爪射弹
		}
		
		// 合成配方的创建详见 Content/ExampleRecipes.cs
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}

	internal class ExampleHookProjectile : ModProjectile
	{
		private static Asset<Texture2D> chainTexture;

		public override void Load() { // 在加载或重新加载时调用
			// 这是钩子链的贴图路径，如果要在你的Mod使用，请确保修改了路径
			chainTexture = ModContent.Request<Texture2D>("ExampleMod/Content/Items/Tools/ExampleHookChain");
		}

		public override void Unload() { // 在卸载时调用
			// 目前，像这样卸载静态字段非常重要，以避免卸载后部分Mod内容仍保留在内存中，占用内存
			chainTexture = null;
		}

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("${ProjectileName.GemHookAmethyst}");
		}

		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.GemHookAmethyst); // 复制紫晶钩的射弹属性
		}

		// 将此钩子用于在飞行途中可以有多个钩子的钩子，如: 双钩、蛛丝吊索、鱼钩、静止钩、月钩
		public override bool? CanUseGrapple(Player player) {
			int hooksOut = 0;
			for (int l = 0; l < Main.maxProjectiles; l++) {
				if (Main.projectile[l].active && Main.projectile[l].owner == Main.myPlayer && Main.projectile[l].type == Projectile.type) {
					hooksOut++;
				}
			}

			return hooksOut <= 2;
		}

		// 对于像 抓钩、糖棒钩、蝙蝠钩、宝石钩 这些仅能同时存在一个飞出射弹的钩爪，在这里返回 true
		// public override bool? SingleGrappleHook(Player player)
		// {
		//	return true;
		// }

		// 在射出时使目前存活时间最长的钩爪消失
		// public override void UseGrapple(Player player, ref int type)
		// {
		//	int hooksOut = 0;
		//	int oldestHookIndex = -1;
		//	int oldestHookTimeLeft = 100000;
		//	for (int i = 0; i < 1000; i++)
		//	{
		//		if (Main.projectile[i].active && Main.projectile[i].owner == projectile.whoAmI && Main.projectile[i].type == projectile.type)
		//		{
		//			hooksOut++;
		//			if (Main.projectile[i].timeLeft < oldestHookTimeLeft)
		//			{
		//				oldestHookIndex = i;
		//				oldestHookTimeLeft = Main.projectile[i].timeLeft;
		//			}
		//		}
		//	}
		//	if (hooksOut > 1)
		//	{
		//		Main.projectile[oldestHookIndex].Kill();
		//	}
		// }

		// 控制钩爪的最大射程，单位为像素。紫晶钩是300, 静止钩是600
		public override float GrappleRange() {
			return 500f;
		}

		public override void NumGrappleHooks(Player player, ref int numHooks) {
			numHooks = 2; // 可同时射出的钩爪数量
		}

		// 默认为11，月钩为24
		public override void GrappleRetreatSpeed(Player player, ref float speed) {
			speed = 18f; // 达到最大射击距离后，钩爪返回的速度有多快
		}

		public override void GrapplePullSpeed(Player player, ref float speed) {
			speed = 10; // 被拉到钩爪勾到的物块上的速度
		}

		// 调整玩家将被拉向的位置，这里使它们悬挂在距离被勾到的方块 50 像素的地方
		public override void GrappleTargetPoint(Player player, ref float grappleX, ref float grappleY) {
			Vector2 dirToPlayer = Projectile.DirectionTo(player.Center);
			float hangDist = 50f;
			grappleX += dirToPlayer.X * hangDist;
			grappleY += dirToPlayer.Y * hangDist;
		}

		// 绘制钩爪链
		public override bool PreDrawExtras() {
			Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter;
			Vector2 center = Projectile.Center;
			Vector2 directionToPlayer = playerCenter - Projectile.Center;
			float chainRotation = directionToPlayer.ToRotation() - MathHelper.PiOver2;
			float distanceToPlayer = directionToPlayer.Length();

			while (distanceToPlayer > 20f && !float.IsNaN(distanceToPlayer)) {
				directionToPlayer /= distanceToPlayer; // get unit vector
				directionToPlayer *= chainTexture.Height(); // multiply by chain link length

				center += directionToPlayer; // update draw position
				directionToPlayer = playerCenter - center; // update distance
				distanceToPlayer = directionToPlayer.Length();

				Color drawColor = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16));

				// 绘制链子
				Main.EntitySpriteDraw(chainTexture.Value, center - Main.screenPosition,
					chainTexture.Value.Bounds, drawColor, chainRotation,
					chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			}
			// 避免绘制原版默认的链子
			return false;
		}
	}
}
