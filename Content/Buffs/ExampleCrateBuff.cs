using ExampleMod.Common.Players;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Content.Buffs
{
	public class ExampleCrateBuff : ModBuff
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Example Crate");
			Description.SetDefault("Greater chance of fishing up a crate");
			// 中文本地化见 zh-Hans.hjson
		}

		public override void Update(Player player, ref int buffIndex) {
			// 用 ModPlayer 来看此buff是否激活
			player.GetModPlayer<ExampleFishingPlayer>().hasExampleCrateBuff = true;
		}
	}
}
