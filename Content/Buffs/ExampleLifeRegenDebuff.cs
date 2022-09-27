using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
//using Terraria.Localization;

namespace ExampleMod.Content.Buffs
{
	// 这个类作为持续掉血debuff的示例
	// 更多信息参见本文件末尾的 ExampleLifeRegenDebuffPlayer.UpdateBadLifeRegen
	public class ExampleLifeRegenDebuff : ModBuff
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Fire debuff"); // Buff显示的名称
			Description.SetDefault("Losing life"); // Buff描述
			// 下面的方法可以直接添加一种语言的翻译, 别忘了 using Terraria.Localization
			// 但是我建议你用.hjson文件进行本地化, 详见 Localization 文件夹中各.hjson文件的 BuffName 与 BuffDescription
			// DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "火焰debuff?");
			//Description.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "掉血?");
			Main.debuff[Type] = true;  // 这是个debuff吗?
			Main.pvpBuff[Type] = true; // 玩家可以对其他玩家施加被列为 pvpBuff 的 buff
			Main.buffNoSave[Type] = true; // 使这个buff在退出或重进世界时不持续
			BuffID.Sets.LongerExpertDebuff[Type] = true; // 如果这个buff是debuff, 那么将此字段设为 true 会使得专家模式下玩家被施加2倍时长的该debuff(大师模式则为2.5倍)
		}

		// 允许你让这个buff给特定的玩家特定的效果
		public override void Update(Player player, ref int buffIndex) {
			player.GetModPlayer<ExampleLifeRegenDebuffPlayer>().lifeRegenDebuff= true;
		}
	}

	public class ExampleLifeRegenDebuffPlayer : ModPlayer
	{
		// 在生命回复debuff应该被激活时进行标记检查
		public bool lifeRegenDebuff;

		public override void ResetEffects() {
			lifeRegenDebuff = false;
		}

		// 允许你基于玩家的状态给玩家负的生命回复(如, "着火了! "debuff令玩家随时间受到伤害)
		// 这主要由: 若 player.lifeRegen 为正则将其设为0, 再减去一个数字来完成的
		// 玩家会以每秒你所减去的数字的一半的速率掉血
		public override void UpdateBadLifeRegen() {
			if (lifeRegenDebuff) {
				// 这一行将正的生命回复变为0, 所有负生命回复debuff都应该这样做
				if (Player.lifeRegen > 0)
					Player.lifeRegen = 0;
				// Player.lifeRegenTime 用来加快玩家自然生命回复到达上限的速度
				// 所以我们将它设为0, 在这个debuff激活时, 它永远也不会达到上限(是0啊)
				Player.lifeRegenTime = 0;
				// lifeRegen 以0.5血/秒衡量. 由此, 这个效果导致玩家每秒掉8血
				Player.lifeRegen -= 16;
			}
		}
	}
}
