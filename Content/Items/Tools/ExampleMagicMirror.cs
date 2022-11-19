using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Tools
{
	// Magic Mirror 是为数不多的在动画起始之外的地方执行其操作的原版物品之一，这就是我们在 UseStyle 中使用代码而不是 UseItem 的原因。
	// 对于具有类似行为的模组物品，它可能是一个有用的指南。
	internal class ExampleMagicMirror : ExampleItem
	{
		private static readonly Color[] itemNameCycleColors = {
			new Color(254, 105, 47),
			new Color(190, 30, 209),
			new Color(34, 221, 151),
			new Color(0, 106, 185),
		};

		public override string Texture => $"Terraria/Images/Item_{ItemID.IceMirror}"; // 复制原版冰雪镜的贴图，你也可以用你自己的

		public override void SetStaticDefaults() {
			// 该物品在旅行模式的复制菜单用于研究的数量，这里是根据原版魔镜所需的数量设置的
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
			DisplayName.SetDefault("Example Magic Mirror");
			DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "示例魔镜");
		}

		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.IceMirror); // 复制原版冰雪镜的属性
			Item.color = Color.Violet; // 设置物品颜色
		}

		// UseStyle 在物品使用的每一帧都会调用
		public override void UseStyle(Player player, Rectangle heldItemFrame) {
			// 每帧生成一些粒子
			if (Main.rand.NextBool()) {
				Dust.NewDust(player.position, player.width, player.height, DustID.MagicMirror, 0f, 0f, 150, Color.White, 1.1f); // Makes dust from the player's position and copies the hitbox of which the dust may spawn. Change these arguments if needed.
			}

			// 正确设置 itemTime
			if (player.itemTime == 0) {
				player.ApplyItemTime(Item);
			}
			else if (player.itemTime == player.itemTimeMax / 2) {
				// 此代码在 Item 的 useTime 中途运行一次。 你会注意到在传送后你仍然会在一小段时间持有魔镜
				
				// 传送前，生成 70 个粒子用于传送特效
				for (int d = 0; d < 70; d++) {
					Dust.NewDust(player.position, player.width, player.height, DustID.MagicMirror, player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 150, default, 1.5f);
				}

				// 此代码会使所有钩爪消失
				player.grappling[0] = -1;
				player.grapCount = 0;

				for (int p = 0; p < 1000; p++) {
					if (Main.projectile[p].active && Main.projectile[p].owner == player.whoAmI && Main.projectile[p].aiStyle == 7) {
						Main.projectile[p].Kill();
					}
				}

				// 将玩家移回床上/重生点的方法
				player.Spawn(PlayerSpawnContext.RecallFromItem);
				
				// 传送后，生成 70 个粒子用于传送特效
				for (int d = 0; d < 70; d++) {
					Dust.NewDust(player.position, player.width, player.height, DustID.MagicMirror, 0f, 0f, 150, default, 1.5f);
				}
			}
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips) {
			// 此代码显示使用 Color.Lerp、Main.GameUpdateCount 和取模运算符 (%) 在 4 种自定义颜色之间进行巧妙的循环。
			int numColors = itemNameCycleColors.Length;
			
			foreach (TooltipLine line2 in tooltips) {
				if (line2.Mod == "Terraria" && line2.Name == "ItemName") {
					float fade = (Main.GameUpdateCount % 60) / 60f;
					int index = (int)((Main.GameUpdateCount / 60) % numColors);
					int nextIndex = (index + 1) % numColors;

					line2.OverrideColor = Color.Lerp(itemNameCycleColors[index], itemNameCycleColors[nextIndex], fade);
				}
			}
		}
		
		// 合成配方的创建详见 Content/ExampleRecipes.cs
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}
