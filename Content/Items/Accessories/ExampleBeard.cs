using Terraria;
using Terraria.ID;
// using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Accessories
{
	// 此物品是一个使用了灰度图的胡须, 从人物的头发取色
	// 需要 ArmorIDs.Beard.Sets.UseHairColor 和  Item.color 来起作用
	// 对于固定颜色的胡须, 写一个 Item.color, 剩下的就不要了
	[AutoloadEquip(EquipType.Beard)]
	public class ExampleBeard : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("这是个与你发色相同的模组胡须.");

			// CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
			SacrificeTotal = 1; // 上一行与这一行的效果是一样的, 旅途研究所需数量, 只是下面这个更简洁, 还不需要 using Terraria.GameContent.Creative;

			ArmorIDs.Beard.Sets.UseHairColor[Item.beardSlot] = true; // 使用人物发色
		}

		public override void SetDefaults() {
			Item.width = 18;
			Item.height = 14;
			Item.maxStack = 1;
			Item.color = Main.LocalPlayer.hairColor; // 本地人物的发色
			Item.value = Item.sellPrice(0, 1);
			Item.accessory = true;
			Item.vanity = true;
		}
	}
}
