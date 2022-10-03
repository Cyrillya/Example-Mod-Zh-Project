using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ExampleMod.Content.BossBars
{
	// 一个用了自定义材质的boss血条. 只修改了视觉效果, 若要有实际效果的boss血条, 参见 MinionBossBossBar
	// 在要使用自定义boss血条的NPC的 SetDefaults 中写:
	// NPC.BossBar = ModContent.GetInstance<你boss血条的类名>();

	// 注意, 如果一个NPC有它的boss头图标, 就会自动显示原版的boss血条. ModdBossBar不一定要给boss用

	// 你可以使NPC永远也不显示boss血条, 就像地牢守卫和拜月教徒分身那样:
	//  NPC.BossBar = Main.BigBossProgressBar.NeverValid;
	public class ExampleBossBar : ModBossBar
	{
		public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame) {
			return TextureAssets.NpcHead[36]; // 柯基犬(?)头图标
		}

		public override bool PreDraw(SpriteBatch spriteBatch, NPC npc, ref BossBarDrawParams drawParams) {
			// 使血条随NPC血量降低而抖动
			float shakeIntensity = Utils.Clamp(1f - drawParams.LifePercentToShow - 0.2f, 0f, 1f);
			drawParams.BarCenter.Y -= 20f;
			drawParams.BarCenter += new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)) * shakeIntensity * 15f;

			drawParams.IconColor = Main.DiscoColor;

			return true;
		}
	}
}
