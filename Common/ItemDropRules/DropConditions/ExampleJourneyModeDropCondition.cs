using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace ExampleMod.Common.ItemDropRules.DropConditions
{
	// 仅在旅行模式掉落的条件
	public class ExampleJourneyModeDropCondition : IItemDropRuleCondition
	{
		public bool CanDrop(DropAttemptInfo info) {
			if (info.IsInSimulation) {
				return false;
			}
			return Main.GameModeInfo.IsJourneyMode;
		}

		public bool CanShowItemDropInUI() {
			return true;
		}

		public string GetConditionDescription() {
			return "仅在旅行模式下掉落";
		}
	}
}
