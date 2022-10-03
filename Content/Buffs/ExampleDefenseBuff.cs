using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Content.Buffs
{
	public class ExampleDefenseBuff : ModBuff
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Defensive Buff");
			Description.SetDefault("Grants +4 defense.");
			// 中文本地化见 zh-Hans.hjson
		}

		public override void Update(Player player, ref int buffIndex) {
			player.statDefense += 4; // 此buff激活时使玩家的防御+4
		}
	}
}
