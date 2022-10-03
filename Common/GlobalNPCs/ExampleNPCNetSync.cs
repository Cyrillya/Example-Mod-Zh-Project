using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ExampleMod.Common.GlobalNPCs
{
	// 这是一个专门展示Send/ReceiveExtraAI()方法的类
	public class ExampleNPCNetSync : GlobalNPC
	{
		public override bool InstancePerEntity => true;
		private bool differentBehavior;

		// 使得这个GlobalNPC仅对于Sharkron2创造实例，也就是说更改只会对于Sharkron2这个NPC有效
		// 使用这种写法替代方法中的“if (npc.type==xx)”判断，在一定程度上可以提升性能
		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
			return entity.type == NPCID.Sharkron2;
		}
		
		// 尽管这在客户端和服务器上都运行，但只有生成此NPC的端得到它的来源信息(即IEntitySource)
		// 因此，下面演示的“if”判断检查内的条件在多人模式客户端只会是false，即代码永远不会运行
		// 由于代码只会在服务器运行，那么就要进行下文的联机同步了
		public override void OnSpawn(NPC npc, IEntitySource source) {

			// When spawned by a Cthulunado during a Blood Moon
			if (source is EntitySource_Parent parent
				&& parent.Entity is Projectile projectile
				&& projectile.type == ProjectileID.Cthulunado
				&& Main.bloodMoon) {
				differentBehavior = true;
			}
		}

		// 因为这个GlobalNPC只适用于Sharkron2(使用了AppliesToEntity判断)，所以这个传输数据不会附在所有的NPC传输数据包中
		public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {
			bitWriter.WriteBit(differentBehavior);
		}

		// 请确保你发了多少数据就接收多少
		public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {
			differentBehavior = bitReader.ReadBit();
		}

		public override void AI(NPC npc) {
			if (differentBehavior) {
				npc.scale *= 1.0025f;
				if (npc.scale > 3f) {
					npc.scale = 3f;
				}
			}
		}
	}
}