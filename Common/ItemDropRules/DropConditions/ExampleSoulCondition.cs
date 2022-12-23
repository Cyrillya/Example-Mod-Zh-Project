using ExampleMod.Content.Biomes;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.ItemDropRules;

namespace ExampleMod.Common.ItemDropRules.DropConditions
{
	public class ExampleSoulCondition : IItemDropRuleCondition
	{
		public bool CanDrop(DropAttemptInfo info) {
			if (!info.IsInSimulation) {
				// 如果是困难模式，不是小动物或无关紧要的敌人，并且玩家处于ExampleUndergroundBiome中，就可以掉落。（对于敌人的判断和原版是一致的）
				// 这是从原版的 Conditions.SoulOfWhateverConditionCanDrop(info) 方法修改而来的
				// 去掉了对洞穴层（即在下方使用/**/注释掉的条件代码）的限制，因为ExampleUndergroundBiome也延伸到了泥土层。

				NPC npc = info.npc;
				if (npc.boss || NPCID.Sets.CannotDropSouls[npc.type]) {
					return false;
				}

				if (!Main.hardMode || npc.lifeMax <= 1 || npc.friendly /*|| npc.position.Y <= Main.rockLayer * 16.0*/ || npc.value < 1f) {
					return false;
				}

				return info.player.InModBiome<ExampleUndergroundBiome>();
			}
			return false;
		}

		public bool CanShowItemDropInUI() {
			return true;
		}

		public string GetConditionDescription() {
			return "在困难模式中的Example Underground Biome掉落";
		}
	}
}
