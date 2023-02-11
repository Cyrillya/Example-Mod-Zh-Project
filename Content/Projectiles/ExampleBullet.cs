using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles
{
	public class ExampleBullet : ModProjectile
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Example Bullet"); // 该射弹的英文名
			DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "示例子弹");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // 射弹的途径位置记录数量（用于实现残影效果）
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // 记录格式，0代表仅记录坐标位置
		}

		public override void SetDefaults() {
			Projectile.width = 8; // 该射弹碰撞箱的宽度
			Projectile.height = 8; // 该射弹碰撞箱的高度
			Projectile.aiStyle = 1; // 射弹的ai类型，该数值需要参考泰拉瑞亚代码，1为常规子弹
			Projectile.friendly = true; //true会对敌怪造成伤害，false不会
			Projectile.hostile = false; // true会对玩家造成伤害，false不会
			Projectile.DamageType = DamageClass.Ranged; // 射弹的伤害类型，这里为远程伤害
			Projectile.penetrate = 5; // 射弹的穿透数量 (OnTileCollide 为true时命中物块反弹也会减少一次穿透数量)
			Projectile.timeLeft = 600; // 射弹存在的剩余时间，写在这里为射弹的最大存在时间 (60 为 1 秒, 这里是 10 秒)
			Projectile.alpha = 255; // 射弹的透明度，255为完全透明，因为我们上面用来 aiStyle 为1，其中的一项特性就是自动淡入子弹，
									// 既子弹会由透明转为不透明（255→0），如果你没有使用 aiStyle = 1，那这里应该为 0 来使射弹完全不透明
									// 你也可以在 ai 中不断改变该值来改变射弹的透明度，但除非你确实想让射弹完全不可见，最好别让该值长时间停留在255
			Projectile.light = 0.5f; // 射弹发光亮度
			Projectile.ignoreWater = true; // true射弹入水不会改变速度，false会
			Projectile.tileCollide = true; // true能在命中物块后进行反弹，false不能
			Projectile.extraUpdates = 1; // 额外更新数量，如果你希望射弹在1帧中多次更新状态，这里要大于0

			AIType = ProjectileID.Bullet; // 使用传统子弹的ai
		}

		public override bool OnTileCollide(Vector2 oldVelocity) {
			// 碰到物块则减少穿透数量
			// 所以这个射弹最多反弹5次
			Projectile.penetrate--;
			if (Projectile.penetrate <= 0) {
				Projectile.Kill();
			}
			else {
				Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
				SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

				// 当射弹命中一个物块的侧边时，反转x方向上的速度
				if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon) {
					Projectile.velocity.X = -oldVelocity.X;
				}

				// 当射弹命中一个物块的上下面时，反转y方向上的速度
				if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) {
					Projectile.velocity.Y = -oldVelocity.Y;
				}
			}

			return false;
		}

		public override bool PreDraw(ref Color lightColor) {
			Main.instance.LoadProjectile(Projectile.type);
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

			// 使用不被光照影响的方式绘制射弹
			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++) {
				Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}

			return true;
		}

		public override void Kill(int timeLeft) {
			// 这块的代码和上面 OnTileCollide 的代码都会让射弹在命中物块时出现一些物块颜色的粒子效果
			// SoundID.Item10 是反弹的声音
			Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
			SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
		}
	}
}
