using ExampleMod.Content.Items.Consumables;
using ExampleMod.Content.NPCs;
using System.IO;
using Terraria;

namespace ExampleMod
{
	// ����һ��partial(�ֲ�)�࣬����ζ������һЩ���ֱ���ֵ������ļ��С� �������ֲμ� ExampleMod.*.cs��
	// ��: partial��΢���ĵ�: https://learn.microsoft.com/zh-cn/dotnet/csharp/programming-guide/classes-and-structs/partial-classes-and-methods
	partial class ExampleMod
	{
		/// <summary>
		/// ��������ָ�������͵�enum, �൱�ڸ�ÿ��������һ��ָʾ��ݵ�ID<br/>
		/// <b>��Ҫ</b>: �������Visual Studio���ܡ������������á���������Ϊ�����ⲻ���䷢�����ǿ�������<br/>
		/// �Ҽ�������enum�����ĳ�����ͣ�����ʹ�á������������á�����
		/// </summary>
		internal enum MessageType : byte
		{
			ExamplePlayerSyncPlayer,
			ExampleTeleportToStatue
		}

		// ʹ�� HandlePacket ��д������������ ExampleMod ������������ݰ� (ModPacket)
		// tML�а������������д��Ϊ��OOP�ܹ���(���������������)�ļƻ�(����ֻ�Ǽƻ�����)
		// ��д֮���������ݰ�������׵��أ����Ҵ�������ɶ���
		// tML�ļƻ�������Ȥ���Կ���: https://github.com/tModLoader/tModLoader/issues/2436
		// Cyril����OOP�շ����⣬���ڻ�û��ExampleMod(����˵ʾ�����Ǹ��õ�����Mod): https://github.com/Cyrillya/NetSimplified-tModLoader
		public override void HandlePacket(BinaryReader reader, int whoAmI) {
			MessageType msgType = (MessageType)reader.ReadByte();

			switch (msgType) {
				// ͬ�� ExamplePlayer.exampleLifeFruits
				case MessageType.ExamplePlayerSyncPlayer:
					byte playernumber = reader.ReadByte();
					ExampleLifeFruitPlayer examplePlayer = Main.player[playernumber].GetModPlayer<ExampleLifeFruitPlayer>();
					examplePlayer.exampleLifeFruits = reader.ReadInt32();
					// SyncPlayer�����Զ����ã���˲���Ҫ����Щ����ת���������ͻ���
					// (���������SyncPlayer���з����ģ����ExampleLifeFruit.cs)
					break;
				case MessageType.ExampleTeleportToStatue:
					// ���ݶ����whoAmI��ʶ������ӦNPCִ�з���
					if (Main.npc[reader.ReadByte()].ModNPC is ExamplePerson person && person.NPC.active) {
						person.StatueTeleport();
					}

					break;
				default:
					// ����ı�����ʲô�Ķ�Ӧ�ñ���Ӣ�ģ�������Language.GetTextValueҲ���ǲ��У����ǵ���Ӣ�ģ�
					Logger.WarnFormat("ExampleMod: Unknown Message type: {0}", msgType);
					break;
			}
		}
	}
}