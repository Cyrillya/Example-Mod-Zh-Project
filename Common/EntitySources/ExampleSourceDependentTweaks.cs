using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Common.EntitySources
{
	// 下面的两个类展示了通过模式匹配对 IEntitySource 实例进行操作，以使事情只在特定情况下发生
	// 附(1): 微软文档模式匹配概述: https://learn.microsoft.com/zh-cn/dotnet/csharp/fundamentals/functional/pattern-matching
	// 附(2): 裙中世界 IEntitySource 概述 https://fs49.org/2022/05/03/1-4-%e7%94%9f%e6%88%90%e6%ba%90%e4%bf%a1%e6%81%afientitysource%e7%9a%84%e7%94%a8%e6%b3%95/

	public sealed class ExampleSourceDependentProjectileTweaks : GlobalProjectile
	{
		public override void OnSpawn(Projectile projectile, IEntitySource source) {
			// 如果射弹是由火把神射出的，并且瞄准了一个玩家，则另外射出2个较慢的分散开来的射弹
			if (source is EntitySource_TorchGod { TargetedEntity: Player } && projectile.type == ProjectileID.TorchGod) {
				var newSource = projectile.GetSource_FromThis(); // 为新生成的射弹使用另一个单独的生成源，以免发生栈溢出 (死循环调用方法)

				// for 循环这么写，则只会有 2 次运行，i 值分别为 -1 和 1
				for (int i = -1; i <= 1; i += 2) {
					var velocity = projectile.velocity.RotatedBy(MathHelper.ToRadians(15f * i)) * 0.5f;

					Projectile.NewProjectile(newSource, projectile.position, velocity, projectile.type, projectile.damage, projectile.knockBack, projectile.owner);
				}
			}

			// 对每个来源信息为 FallenStar (落星) 的射弹进行操作
			// 原版里在天上掉下落星时会用到这个来源信息
			if (source is EntitySource_Misc { Context: "FallingStar" }) {
				float closestPlayerSqrDistance = -1f;
				Player closestPlayer = null;

				for (int i = 0; i < Main.maxPlayers; i++) {
					var player = Main.player[i];

					if (player?.active != true || player.DeadOrGhost) {
						continue;
					}

					// 这里是要选出离落星最近的玩家，直接用距离平方比较即可
					// 即 distance=a²+b², 这样做的好处是运行起来更快，少了开根号这一步骤
					float sqrDistance = player.Center.LengthSquared();

					if (closestPlayer == null || sqrDistance < closestPlayerSqrDistance) {
						closestPlayer = player;
						closestPlayerSqrDistance = sqrDistance;
					}
				}

				// 如果找到了最近的玩家
				if (closestPlayer != null) {
					// 将落星对准最近的玩家
					var directionTowardsPlayer = (closestPlayer.Center - projectile.Center).SafeNormalize(default);

					if (directionTowardsPlayer != default) {
						projectile.velocity = directionTowardsPlayer * (projectile.velocity.Length() + 10f); // “小”加一下速度

						// 由于OnSpawn是在射弹联机同步之前调用的，所以在这种情况下，我们不需要专门为多人游戏同步速度，会自动同步
					}
				}
			}
		}
	}
	
	public sealed class ExampleSourceDependentItemTweaks : GlobalItem
	{
		public override void OnSpawn(Item item, IEntitySource source) {
			// 摇树会100%生成史莱姆
			if (source is EntitySource_ShakeTree) {
				var newSource = item.GetSource_FromThis(); // 为新生成的NPC使用另一个单独的生成源，以免发生栈溢出 (死循环调用方法)

				NPC.NewNPC(newSource, (int)item.position.X, (int)item.position.Y, NPCID.BlueSlime);
			}
		}
	}
}
