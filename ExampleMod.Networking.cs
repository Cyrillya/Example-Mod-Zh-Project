using ExampleMod.Content.Items.Consumables;
using ExampleMod.Content.NPCs;
using System.IO;
using Terraria;

namespace ExampleMod
{
	// 这是一个partial(分部)类，这意味着它的一些部分被拆分到其他文件中。 其他部分参见 ExampleMod.*.cs。
	// 附: partial类微软文档: https://learn.microsoft.com/zh-cn/dotnet/csharp/programming-guide/classes-and-structs/partial-classes-and-methods
	partial class ExampleMod
	{
		/// <summary>
		/// 这是用来指定包类型的enum, 相当于给每个包分配一个指示身份的ID<br/>
		/// <b>重要</b>: 建议搭配Visual Studio功能「查找所有引用」来看，因为这玩意不搭配发包看是看不懂的<br/>
		/// 右键点击这个enum里面的某个类型，即可使用「查找所有引用」功能
		/// </summary>
		internal enum MessageType : byte
		{
			ExamplePlayerSyncPlayer,
			ExampleTeleportToStatue
		}

		// 使用 HandlePacket 重写函数来控制由 ExampleMod 传入的网络数据包 (ModPacket)
		// tML有把网络包进行重写改为「OOP架构」(即面向对象程序设计)的计划(不过只是计划罢了)
		// 重写之后网络数据包会更容易调控，并且大大提升可读性
		// tML的计划，有兴趣可以看看: https://github.com/tModLoader/tModLoader/issues/2436
		// Cyril做的OOP收发包库，现在还没做ExampleMod(或者说示例就是更好的体验Mod): https://github.com/Cyrillya/NetSimplified-tModLoader
		public override void HandlePacket(BinaryReader reader, int whoAmI) {
			MessageType msgType = (MessageType)reader.ReadByte();

			switch (msgType) {
				// 同步 ExamplePlayer.exampleLifeFruits
				case MessageType.ExamplePlayerSyncPlayer:
					byte playernumber = reader.ReadByte();
					ExampleLifeFruitPlayer examplePlayer = Main.player[playernumber].GetModPlayer<ExampleLifeFruitPlayer>();
					examplePlayer.exampleLifeFruits = reader.ReadInt32();
					// SyncPlayer将被自动调用，因此不需要将这些数据转发给其他客户端
					// (这个包是在SyncPlayer进行发包的，详见ExampleLifeFruit.cs)
					break;
				case MessageType.ExampleTeleportToStatue:
					// 根据读入的whoAmI标识，给对应NPC执行方法
					if (Main.npc[reader.ReadByte()].ModNPC is ExamplePerson person && person.NPC.active) {
						person.StatueTeleport();
					}

					break;
				default:
					// 这里的报错警告什么的都应该保持英文（你想用Language.GetTextValue也不是不行，但是得有英文）
					Logger.WarnFormat("ExampleMod: Unknown Message type: {0}", msgType);
					break;
			}
		}
	}
}