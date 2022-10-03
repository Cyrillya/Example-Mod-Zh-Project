using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Buffs
{
	public class ExampleWhipDebuff : ModBuff
	{
		public override void SetStaticDefaults() {
			// 使得此debuff能被施加在免疫所有debuff的NPC身上 (免疫鞭子标记是单独的一个)
			// 其它模组可能会检测这个
			BuffID.Sets.IsAnNPCWhipDebuff[Type] = true;
			// 此buff不会被施加在玩家身上, 所以没有名字或描述
			// 如果你要搞点骚操作, 记得写上
		}

		public override void Update(NPC npc, ref int buffIndex) {
			npc.GetGlobalNPC<ExampleWhipDebuffNPC>().markedByExampleWhip = true;
		}
	}

	public class ExampleWhipDebuffNPC : GlobalNPC
	{
		// 使得每个实体 (此处是NPC) 有自己的属性, 不然一个NPC被标记所有的NPC都会被标记
		public override bool InstancePerEntity => true;

		public bool markedByExampleWhip;

		public override void ResetEffects(NPC npc) {
			markedByExampleWhip = false;
		}

		// 原版的鞭子标记增伤是在伤害随机浮动之后, 但是tML还没有那个钩子, 所以先用下面这个伤害浮动之前的方法
		public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) {
			// 只有玩家的攻击从中获益, 所以要检测射弹是不是NPC或陷阱的
			if (markedByExampleWhip && !projectile.npcProj && !projectile.trap && (projectile.minion || ProjectileID.Sets.MinionShot[projectile.type])) {
				damage += 5;
			}
		}
	}
}
