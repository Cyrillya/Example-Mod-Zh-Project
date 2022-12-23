using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;

namespace ExampleMod.Content.Items.Tools
{
	// 这是一个捕虫网示例，旨在演示与捕捉 NPC (例如带物品的小动物) 相关的各种钩子(也就是方法)的使用
	public class ExampleBugNet : ModItem
	{
		public override string Texture => $"Terraria/Images/Item_{ItemID.BugNet}";

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("ExampleBugNet");
			DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "示例虫网");

			// 需要将这个物品归到 CatchingTool 集合中以让其可以捕捉 NPC
			// 另一个叫做 LavaproofCatchingTool 的集合让你的捕虫网能够捕捉地狱的熔岩昆虫
			ItemID.Sets.CatchingTool[Item.type] = true;
			// ItemID.Sets.LavaproofCatchingTool[Item.type] = true;

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
		
		// 这里是经过一些修改后应用于基本捕虫网的物品属性
		// 关于每个物品属性的解释可参考 Content/Items/Weapons/ExampleSword.cs
		public override void SetDefaults() {
			// 一般属性
			Item.width = 24;
			Item.height = 28;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(0, 0, 40);

			// 物品使用属性
			Item.useAnimation = 25;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.UseSound = SoundID.Item1;
		}

		public override bool? CanCatchNPC(NPC target, Player player) {
			// 此钩子用于确定您的捕捉工具是否可以捕捉给定的 NPC
			// 默认返回 null，由原版代码决定是否应该捕捉 NPC
			// 返回 true 表示可以捕捉 NPC，返回 false 则表示不可以
			// 如果你不确定要返回什么，请返回 null
			// 在这里，我们的捕虫网有 20% 的几率成功捕捉熔岩小动物 (获得温暖药水Buff后，几率上调为 50%)
			if (ItemID.Sets.IsLavaBait[target.catchItem]) {
				if (Main.rand.NextBool(player.resistCold ? 2 : 5)) {
					return true;
				}
			}

			// 对于未明确返回 true 的所有情况，我们将返回 null 让原版捕捉规则和效果代码运行
			return null;
		}
		
		// 合成配方的创建详见 Content/ExampleRecipes.cs
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}

	// 这个类演示了如何通过 OnSpawn 来修改因捕捉 NPC 或其他实体而生成的物品
	public class ExampleCatchItemModification : GlobalItem
	{
		public override void OnSpawn(Item item, IEntitySource source) {
			if (source is not EntitySource_CatchEntity catchEntity) {
				return;
			}

			if (catchEntity.Entity is Player player) {
				// 被 ExampleBugNet 捕捉的 NPC 有 5% 的概率双倍
				if (player.HeldItem.type == ModContent.ItemType<ExampleBugNet>() && Main.rand.NextBool(20)) {
					item.stack *= 2;
				}
			}
		}
	}
}
