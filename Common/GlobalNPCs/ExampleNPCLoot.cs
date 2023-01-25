using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using ExampleMod.Content.Items;
using ExampleMod.Common.ItemDropRules.DropConditions;
using System.Linq;

namespace ExampleMod.Common.GlobalNPCs
{
	// 此文件展示了大量 NPC 掉落系统的操作示例
	// 你可在此英文 Wiki 中了解更多: https://github.com/tModLoader/tModLoader/wiki/Basic-NPC-Drops-and-Loot-1.4
	// 中文教程可在裙中世界网站中找到: https://fs49.org/sample-page/
	// 尽管此文件被标记为 GlobalNPC, 但其中的所有内容都可以在 ModNPC 中使用! 你可在 Content/NPCs 文件夹中找到使用示例
	public class ExampleNPCLoot : GlobalNPC
	{
		// ModifyNPCLoot 使用一个名为 ItemDropDatabase 的独特系统，该系统针对许多不同的掉落用例具有许多不同的规则。
		// 在这里，我们将介绍它们的一些基本用法。
		// 原版中还有很多其他例子！ 在反编译的原版代码中，你可以在 GameContent/ItemDropRules/ItemDropDatabase
		// 找到所有的原版 NPC 物品掉落注册代码，可供参考。

		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
			if (!NPCID.Sets.CountsAsCritter[npc.type]) { // 如果 NPC 不是小动物
				// 让它掉落 ExampleItem
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ExampleItem>(), 1));

				// 在大多数情况下会使用 ItemDropRule.Common，用于以指定的分数几率掉落物品。
				// chanceDenominator 参数是用于掉落该物品的分数几率的分母部分。

				// 在旅行模式中以 2/7 的几率掉落 ExampleResearchPresent
				IItemDropRule presentDropRule = new LeadingConditionRule(new ExampleJourneyModeDropCondition());

				// ItemDropRule.Common(...) 方法不允许指定分子，因此可以改用 new CommonDrop(...)。
				// (如果使用 ItemDropRule.Common，则分子默认为 1)
				// 例如，如果分母(chanceDenominator)为 7，分子(chanceNumerator)为 2，则物品掉落的几率为 2/7，即约 28%。
				presentDropRule.OnSuccess(new CommonDrop(ModContent.ItemType<ExampleResearchPresent>(), chanceDenominator: 7, chanceNumerator: 2));
				npcLoot.Add(presentDropRule);
			}

			// 这里拿向导开刀，作为一个找到并移除原有规则的例子。
			if (npc.type == NPCID.Guide) {
				// RemoveWhere 将删除与提供的表达式匹配的任何规则。
				// 要创建自己的表达式来删除原版掉落规则，一般来说必须查看添加这些规则的源码。
				npcLoot.RemoveWhere(
					// 如果满足以下条件，则表达式返回 true：
					rule => rule is ItemDropWithConditionRule drop // 如果规则是 ItemDropWithConditionRule 实例
						&& drop.itemId == ItemID.GreenCap // 并且该实例会掉落一个绿帽
						&& drop.condition is Conditions.NamedNPC npcNameCondition // 且该规则的条件是 NPC 名称必须匹配某个字符串
						&& npcNameCondition.neededName == "Andrew" // 且该条件要求的字符串是“Andrew”
				);

				// 删除了原版规则后，这里我们让所有的向导都掉落绿帽。
				npcLoot.Add(ItemDropRule.Common(ItemID.GreenCap, 1));
			}

			// 修改原有的掉落规则
			if (npc.type == NPCID.BloodNautilus) {
				// 恐惧鹦鹉螺，在代码中被称为 BloodNautilus，掉落血红法杖。
				// 掉率在专家模式下为 100%，在普通模式下为 50%。 这里我们将更改它的掉率。
				// 对应的原版规则是：ItemDropRule.NormalvsExpert(4269, 2, 1)
				// NormalvsExpert 方法创建一个 DropBasedOnExpertMode 规则，该规则由 2 个 CommonDrop 规则组成。
				// 我们需要在匹配规则的表达式中使用这些信息来正确识别要编辑的配方。

				// 有两个选择。 一种选择是删除原始规则，然后重新添加类似的规则。 另一种选择是修改现有规则。
				// 最好选择修改现有规则而不是移除，以保持与其他模组的兼容性。

				// 调整现有规则：将普通模式掉落率从50%更改为33.3%
				foreach (var rule in npcLoot.Get()) {
					// 必须查看原版代码以了解如何匹配到对应规则
					if (rule is DropBasedOnExpertMode {ruleForNormalMode: CommonDrop {itemId: ItemID.SanguineStaff} normalDropRule})
						normalDropRule.chanceDenominator = 3;
				}

				// 你也可以选择删除规则，然后添加另一个规则
				// 这里我们将普通模式掉落率从 50% 更改为 16.6%
				/*
				npcLoot.RemoveWhere(
					rule => rule is DropBasedOnExpertMode drop && drop.ruleForNormalMode is CommonDrop normalDropRule && normalDropRule.itemId == ItemID.SanguineStaff
				);
				npcLoot.Add(ItemDropRule.NormalvsExpert(4269, 6, 1));
				*/
			}

			// 修改现有的 Boss 的掉落规则
			// 除了这段代码，我们还在 Common/GlobalItems/BossBagLoot.cs 中写了类似的代码来修改宝藏袋的战利品。
			// 如果你的修改应该影响宝藏袋，切记同时执行这两项操作。
			if (npc.type == NPCID.QueenBee) {
				foreach (var rule in npcLoot.Get()) {
					if (rule is DropBasedOnExpertMode {ruleForNormalMode: OneFromOptionsNotScaledWithLuckDropRule oneFromOptionsDrop} && oneFromOptionsDrop.dropIds.Contains(ItemID.BeeGun)) {
						var original = oneFromOptionsDrop.dropIds.ToList();
						original.Add(ModContent.ItemType<Content.Items.Accessories.WaspNest>());
						oneFromOptionsDrop.dropIds = original.ToArray();
					}
				}
			}

			if (npc.type is NPCID.Crimera or NPCID.Corruptor) {
				// 在这里，我们使用我们自己创建的特殊规则：仅在白天掉落
				ExampleDropCondition exampleDropCondition = new ExampleDropCondition();
				IItemDropRule conditionalRule = new LeadingConditionRule(exampleDropCondition);

				int itemType = ItemID.Vertebrae;
				if (npc.type == NPCID.Crimera) {
					itemType = ItemID.RottenChunk;
				}
				// 33% 几率掉落其他相应物品
				IItemDropRule rule = ItemDropRule.Common(itemType, chanceDenominator: 3);

				// 将我们的物品掉落规则链接到条件规则，也就是添加一个前提条件
				conditionalRule.OnSuccess(rule);
				// 添加规则
				npcLoot.Add(conditionalRule);
				// 这将导致掉落物显示在怪物图鉴中，但只有在条件为真时才会真正掉落。
			}
		}

		// ModifyGlobalLoot 允许您修改每个 NPC 应该能够掉落的战利品，最好给全局掉落一个条件。
		// 原版将其用于生物群落钥匙、光明/暗影之魂以及节日掉落物。
		// ModifyGlobalLoot 中的任何掉落规则都应该只运行一次。 其他一切都应该放在 ModifyNPCLoot 中。
		public override void ModifyGlobalLoot(GlobalLoot globalLoot) {
			// 如果 ExampleSoulCondition 为真，则有 20% 的几率掉落 ExampleSoul
			// 请参阅 Common/ItemDropRules/DropConditions/ExampleSoulCondition.cs 以了解相应条件判断
			globalLoot.Add(ItemDropRule.ByCondition(new ExampleSoulCondition(), ModContent.ItemType<ExampleSoul>(), 5, 1, 1));
		}
	}
}
