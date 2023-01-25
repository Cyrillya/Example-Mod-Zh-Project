using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.GlobalItems
{
	public class BossBagLoot : GlobalItem
	{
		public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
			// 除了这段代码，我们还在 Common/GlobalNPCs/ExampleNPCLoot.cs 中写了类似的代码来修改非专家掉落的 Boss 战利品。
			// 如果你的修改也应该影响非专家掉落，切记同时执行这两项操作。
			if(item.type == ItemID.QueenBeeBossBag) {
				foreach (var rule in itemLoot.Get()) {
					if (rule is OneFromOptionsNotScaledWithLuckDropRule oneFromOptionsDrop && oneFromOptionsDrop.dropIds.Contains(ItemID.BeeGun)) {
						var original = oneFromOptionsDrop.dropIds.ToList();
						original.Add(ModContent.ItemType<Content.Items.Accessories.WaspNest>());
						oneFromOptionsDrop.dropIds = original.ToArray();
					}
				}
			}
		}
	}
}
