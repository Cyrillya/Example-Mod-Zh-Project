using ExampleMod.Common.Players;
using ExampleMod.Content.Items.Placeable;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Buffs
{
	public class Blocky : ModBuff
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Blocky");
			Description.SetDefault("Jumping power is increased");
			// 中文本地化见 zh-Hans.hjson
			Main.debuff[Type] = true;
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
			BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			ExampleCostumePlayer p = player.GetModPlayer<ExampleCostumePlayer>();

			// 由于 UpdateBuffs 在 UpdateEquips 前但是在 ResetEffects 后执行, 这里用 blockyAccessoryPrevious  而不是 blockyAccessory 
			if (player.townNPCs >= 1 && p.BlockyAccessoryPrevious) {
				p.BlockyPower = true;

				if (Main.myPlayer == player.whoAmI && Main.time % 1000 == 0) {
					player.QuickSpawnItem(player.GetSource_Buff(buffIndex), ModContent.ItemType<ExampleBlock>());
				}

				player.jumpSpeedBoost += 4.8f;
				player.extraFall += 45;

				// 其他一些效果:
				//player.lifeRegen++;
				//player.GetCritChance(DamageClass.Melee) += 2;
				//player.GetDamage(DamageClass.Melee) += 0.051f;
				//player.GetAttackSpeed(DamageClass.Melee) += 0.051f;
				//player.statDefense += 3;
				//player.moveSpeed += 0.05f;
			}
			else {
				player.DelBuff(buffIndex);
				buffIndex--;
				// 其实下面这个写法用来清除buff更好:
				//player.buffTime[buffIndex] = 0; // 下一帧buff自然消失
			}
		}
	}
}
