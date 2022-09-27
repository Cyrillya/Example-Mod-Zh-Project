using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace ExampleMod.Content
{
	// 这是一个添加新的显示信息的例子 (像原版的怀表、雷达等)
	// 查看下面的ExampleInfoDisplayPlayer类来了解如何使用它
	class ExampleInfoDisplay : InfoDisplay
	{
		public override void SetStaticDefaults() {
			// 这是当鼠标悬停在信息显示图标上时显示的名称
			InfoName.SetDefault("Minion Count");
			InfoName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "仆从计数");
		}

		// 这是让它显示的条件
		public override bool Active() {
			return Main.LocalPlayer.GetModPlayer<ExampleInfoDisplayPlayer>().showMinionCount;
		}

		// 使用这个重写函数来修改显示的值
		public override string DisplayValue() {
			// 统计玩家有多少个仆从
			int minionCount = Main.projectile.Count(x => x.active && x.minion && x.owner == Main.LocalPlayer.whoAmI);
			// 这是显示在信息图标右侧的内容
			return minionCount > 0 ? $"{minionCount} minions." : "No minions";
		}
	}

	public class ExampleInfoDisplayPlayer : ModPlayer
	{
		// 一个用于指示是否应该开启信息显示的bool变量
		public bool showMinionCount;

		public override void ResetEffects() {
			showMinionCount = false;
		}

		public override void UpdateEquips() {
			// 这个信息仅在拥有雷达时才会显示
			if (Player.accThirdEye)
				showMinionCount = true;
		}
	}
}
