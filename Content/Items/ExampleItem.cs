using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ExampleMod.Content.Items.Placeable;
using ExampleMod.Content.Items.Placeable.Furniture;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;

namespace ExampleMod.Content.Items
{
	public class ExampleItem : ModItem
	{
		public override void SetStaticDefaults() {
			// 这是这个物品的工具提示，也就是显示在物品基础属性下方的那一串文字
			Tooltip.SetDefault("This is a modded Item.");
			// 下面这行给工具提示添加中文翻译，则如果游戏语言为中文就会显示“这是一个模组物品”
			Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "这是一个模组物品");

			// 在旅行模式研究这个物品需要多少个
			// 附: Wiki相关链接
			// 全物品需求量明细: https://terraria.wiki.gg/zh/wiki/%E6%97%85%E8%A1%8C%E6%A8%A1%E5%BC%8F/%E7%A0%94%E7%A9%B6%E5%88%97%E8%A1%A8
			// 旅行模式研究介绍: https://terraria.wiki.gg/zh/wiki/%E6%97%85%E8%A1%8C%E6%A8%A1%E5%BC%8F#%E7%A0%94%E7%A9%B6
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
			// 如果你觉得这个太长了，可以使用下面这种方式:
			// SacrificeTotal = 100;
		}

		public override void SetDefaults() {
			Item.width = 20; // 物品贴图的宽度(像素)
			Item.height = 20; // 物品贴图的高度(像素)

			Item.maxStack = 999; // 物品的最大可堆叠量
			
			// 物品的价格，这里使用 buyPrice 也就是买入价，silver: 1 也就是一银的买入价
			// 而物品出售价=买入价/5，1金=100银，所以这个物品的出售价就是20银
			Item.value = Item.buyPrice(silver: 1);
			// 用 sellPrice 可以直接设置物品的出售价，也就是说上面这行等价于下面这行:
			// Item.value = Item.sellPrice(copper: 20);
		}
		
		// 这里写的是合成配方，合成配方在 Content/ExampleRecipes.cs 有更详尽的介绍
		public override void AddRecipes() {
			CreateRecipe(999)
				.AddIngredient(ItemID.DirtBlock, 10)
				.AddTile(TileID.WorkBenches)
				.Register();
		}

		// 使得在旅行模式研究这个物品会同时解锁示例火把、方块、墙和工作台
		public override void OnResearched(bool fullyResearched) {
			if (fullyResearched) {
				CreativeUI.ResearchItem(ModContent.ItemType<ExampleTorch>());
				CreativeUI.ResearchItem(ModContent.ItemType<ExampleBlock>());
				CreativeUI.ResearchItem(ModContent.ItemType<ExampleWall>());
				CreativeUI.ResearchItem(ModContent.ItemType<ExampleWorkbench>());
			}
		}
	}
}
