using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Localization;

namespace ExampleMod.Content.Items.Consumables
{
	// 制作一个类似生命果 (让生命超过500) 的物品需要很多代码，因为有很多要考虑的东西
	// （不过如果只是接近500的话会比较简单）：
	// 你没法让player.statLifeMax （玩家最大生命值） 超过500 (超过的部分不会被保存), 所以你得在mod中保存额外的生命值
	// 你需要在mod中保存与加载
	internal class ExampleLifeFruit : ModItem
	{
		public const int MaxExampleLifeFruits = 10;
		public const int LifePerFruit = 10;

		public override string Texture => "Terraria/Images/Item_" + ItemID.LifeFruit;

		public override void SetStaticDefaults() {
			Tooltip.SetDefault($"Permanently increases maximum life by {LifePerFruit}\nUp to {MaxExampleLifeFruits} can be used");
			Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), $"永久增加{LifePerFruit}点生命上限\n最多使用{MaxExampleLifeFruits}次");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
		}

		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.LifeFruit);
			Item.color = Color.Purple;
		}

		public override bool CanUseItem(Player player) {
			// mod不应当修改 statLifeMax 属性，这会破坏模组兼容性
			// 如果有mod把 statLifeMax 修改到了500以上，其余模组的类似以下检测 statLifeMax 的代码都会失效
			// 如果有mod这么做了，他们应当重写代码
			// 这个检测防止玩家在达到最大生命值前使用该物品
			return player.statLifeMax == 500 && player.GetModPlayer<ExampleLifeFruitPlayer>().exampleLifeFruits < MaxExampleLifeFruits;
		}

		public override bool? UseItem(Player player) {
			// 别这样写: player.statLifeMax += 2;
			// 尽量使用被赋值的变量而不是直接的数字，增强代码可读性也便于修改
			player.statLifeMax2 += LifePerFruit;
			player.statLife += LifePerFruit;
			if (Main.myPlayer == player.whoAmI) {
				// 生成恢复血量的绿色数字并通知其他客户端（既自动完成了多人游戏时的多端同步）
				player.HealEffect(LifePerFruit);
			}

			// 以下内容非常重要，涉及血量突破原版上限的部分
			player.GetModPlayer<ExampleLifeFruitPlayer>().exampleLifeFruits++;

			// 这会调用两个成就：使用增加生命上限的物品 与 血量/魔力达到最大值
			// 但是因为我们的物品只有在完成成就后才能使用，所以不会生效，注释掉了
			// AchievementsHelper.HandleSpecialEvent(player, 2);
			// 你可以使用类似上面这行的写法来使玩家获得原版成就，关于模组自定义的成就 TML 正在开发，或许在未来版本可以使用 ModAchievement
			return true;
		}

		// 参考 Content/ExampleRecipes.cs 中的合成表添加方法
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}

	public class ExampleLifeFruitPlayer : ModPlayer
	{
		public int exampleLifeFruits;

		public override void ResetEffects() {
			// 在这里增加生命上限才能在玩家的选择菜单中正确显示生命值
			// 生命恢复效果也才会正常恢复额外的生命值
			Player.statLifeMax2 += exampleLifeFruits * ExampleLifeFruit.LifePerFruit;
		}

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) {
			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)ExampleMod.MessageType.ExamplePlayerSyncPlayer);
			packet.Write((byte)Player.whoAmI);
			packet.Write(exampleLifeFruits);
			packet.Send(toWho, fromWho);
		}

		// 注意：SaveData默认为空（如果不写的话就不会存，做再多修改退出世界的时候都会丢失）
		// 阅读 https://github.com/tModLoader/tModLoader/wiki/Saving-and-loading-using-TagCompound 来获取更多关于存储和读取数据的信息
		// 如果你想让玩家/NPC/物品拥有原版之外的属性，那就需要使用这里的方法将额外的数据进行存储，不然就会在数据保存时（退出存档、关闭存档）直接丢失
		public override void SaveData(TagCompound tag) {
			tag["exampleLifeFruits"] = exampleLifeFruits;
		}

		public override void LoadData(TagCompound tag) {
			exampleLifeFruits = (int) tag["exampleLifeFruits"];
		}
	}
}
