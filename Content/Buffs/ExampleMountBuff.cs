using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Content.Buffs
{
	public class ExampleMountBuff : ModBuff
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("ExampleMount");
			Description.SetDefault("Leather seats, 4 cup holders");
			// 中文本地化见 zh-Hans.hjson
			Main.buffNoTimeDisplay[Type] = true; // 不显示此buff的持续时间
			Main.buffNoSave[Type] = true; // 此buff在你退出世界时不保留
		}

		public override void Update(Player player, ref int buffIndex) {
			player.mount.SetMount(ModContent.MountType<Mounts.ExampleMount>(), player);
			player.buffTime[buffIndex] = 10; // 重置持续时间, 这样在有对应坐骑时会一直有此buff
		}
	}
}
