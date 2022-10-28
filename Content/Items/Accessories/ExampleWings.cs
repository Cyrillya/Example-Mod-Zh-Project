using ExampleMod.Common.Configs;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
// using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Accessories
{
	[AutoloadEquip(EquipType.Wings)] // 记得给翅膀加这一行
	public class ExampleWings : ModItem
	{
		//相关的模组配置见 ExampleModConfig.cs
		public override bool IsLoadingEnabled(Mod mod) {
			return ModContent.GetInstance<ExampleModConfig>().ExampleWingsToggle;
		}

		public override void SetStaticDefaults() {
			Tooltip.SetDefault("这是一双模组翅膀");

			// CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
			SacrificeTotal = 1; // 上一行与这一行的效果是一样的, 旅途研究所需数量, 只是下面这个更简洁, 还不需要 using Terraria.GameContent.Creative;

			// 翅膀的各项参数, 下列参数与日耀翅膀相同
			// 飞行时间: 180 帧 = 3 秒 (实际上并不是, 但你这么算没问题的)
			// 飞行速度: 9
			// 加速度乘数: 2.5
			ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(180, 9f, 2.5f);
		}

		public override void SetDefaults() {
			Item.width = 22;
			Item.height = 20;
			Item.value = 10000;
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;
		}

		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
			ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend) {
			ascentWhenFalling = 0.85f; // 滑翔速度
			ascentWhenRising = 0.15f; // 爬升速度
			maxCanAscendMultiplier = 1f; // ??? (待补充)
			maxAscentMultiplier = 3f;
			constantAscend = 0.135f;
		}

		// 合成配方的创建详见 Content/ExampleRecipes.cs
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.SortBefore(Main.recipe.First(recipe => recipe.createItem.wingSlot != -1)) // 将此配方置于所有翅膀之前以使配方目录中的翅膀都被放在一起
				.Register();
		}
	}
}
