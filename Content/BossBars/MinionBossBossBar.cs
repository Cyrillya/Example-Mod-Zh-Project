using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.GameContent.UI.BigProgressBar;
using ExampleMod.Content.NPCs.MinionBoss;

namespace ExampleMod.Content.BossBars
{
	// 有显示图标, 生命与护盾的基本逻辑的血条
	// 该例没有使用自定义材质, 用的是原版的
	public class MinionBossBossBar : ModBossBar
	{
		private int bossHeadIndex = -1;

		public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame) {
			// 显示先前指定好的图标
			if (bossHeadIndex != -1) {
				return TextureAssets.NpcHeadBoss[bossHeadIndex];
			}
			return null;
		}

		public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float lifePercent, ref float shieldPercent) {
			// 游戏在这里判断是否绘制boss血条. 当条件不允许时返回false
			// 如果不可能返回false或null, 那血条就会在不该绘制的时候绘制, 所以码要写得保守一点

			NPC npc = Main.npc[info.npcIndexToAimAt];
			if (!npc.active)
				return false;

			// 在这里指定 bossHeadIndex, 因为 GetIconTexture 要用
			bossHeadIndex = npc.GetBossHeadTextureIndex();

			lifePercent = Utils.Clamp(npc.life / (float)npc.lifeMax, 0f, 1f);

			if (npc.ModNPC is MinionBossBody body) {
				// 我们在主体NPC的 RemainingShields 里已经计算好了护盾值, 所以这里把值取出来就好
				shieldPercent = Utils.Clamp(body.RemainingShields, 0f, 1f);
			}

			return true;
		}
	}
}
