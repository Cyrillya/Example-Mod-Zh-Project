using ExampleMod.Content.Biomes;
using ExampleMod.Content.Items;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.Utilities;

namespace ExampleMod.Content.NPCs
{
	// ���NPC�������ʱ������ͨ��ʬһ�������ǻ���ȡ��������ϵģ�ExampleItem���������Լ����ϣ������ȡ�������㹻�࣬�����ʬ�������Ʒһ�𱣴���������
	public class ExampleZombieThief : ModNPC
	{
		public int StolenItems = 0;

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Example Zombie Thief");
			DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "������ʬ");

			Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Zombie];

			NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) {
				// Ӱ�콩ʬ������ͼ���е����
				Velocity = 1f // ������ͼ����NPC��+1ͼ����ٶ�ǰ�������ң�
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
		}

		public override void SetDefaults() {
			NPC.width = 18;
			NPC.height = 40;
			NPC.damage = 14;
			NPC.defense = 6;
			NPC.lifeMax = 200;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
			NPC.value = 60f;
			NPC.knockBackResist = 0.5f;
			NPC.aiStyle = 3; // 3ΪսʿAI���˴�Ӧѡ����������ƥ�������AI

			AIType = NPCID.Zombie; // ʹ�ý�ʬ��AI���루��ζ�����NPC���ڰ��쿨������̫��
			AnimationType = NPCID.Zombie; // ���ý�ʬ����ͼ֡���봦��ʽ��Ӧ����SetStaticDefaults�е�Main.npcFrameCount[NPC.type]����ͬ������
			Banner = Item.NPCtoBanner(NPCID.Zombie); // ���������ܵ���ʬ���ĵ��˺�Ӱ��
			BannerItem = Item.BannerToItem(Banner); // ��ɱ��������Ϊ��ý�ʬ�������Ӽ���
			SpawnModBiomes = new int[] { ModContent.GetInstance<ExampleSurfaceBiome>().Type }; // ������ͼ�������������չʾ����ΪExampleSurfaceBiome��ʾ���ر�Ⱥϵ��
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			// ����ʹ��AddRange�����Ƕ�ε���Add����Ӷ����Ŀ
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// ��������ͼ������ʾ����������
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

				// ��������ͼ���е�����
				new FlavorTextBestiaryInfoElement("This type of zombie really like Example Items. They steal them as soon as they find some."),
			});
		}

		public override void AI() {
			if (Main.netMode == NetmodeID.MultiplayerClient) {
				return;
			}

			Rectangle hitbox = NPC.Hitbox;
			foreach (Item item in Main.item) {
				// ���������ϵ����ExampleItemʱ�������
				if (item.active && !item.beingGrabbed && item.type == ModContent.ItemType<ExampleItem>() &&
				    hitbox.Intersects(item.Hitbox)) {
					item.active = false;
					StolenItems += item.stack;

					NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item.whoAmI);
				}
			}
		}

		public override void SendExtraAI(BinaryWriter writer) {
			writer.Write(StolenItems);
		}

		public override void ReceiveExtraAI(BinaryReader reader) {
			StolenItems = reader.ReadInt32();
		}

		public override void OnKill() {
			if (Main.netMode == NetmodeID.MultiplayerClient) {
				return;
			}

			// NPC����ʱ����ȫ�������ExampleItem
			while (StolenItems > 0) {
				// ����Ʒ����ǰ����ѭ�����Է�ֹ�������鷶Χ
				int droppedAmount = Math.Min(ModContent.GetInstance<ExampleItem>().Item.maxStack, StolenItems);
				StolenItems -= droppedAmount;
				Item.NewItem(NPC.GetSource_Death(), NPC.Center, ModContent.ItemType<ExampleItem>(), droppedAmount, true);
			}
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) {
			if (spawnInfo.Player.InModBiome(ModContent.GetInstance<ExampleSurfaceBiome>()) // ֻ��ExampleSurfaceBiome������
			    && !NPC.AnyNPCs(Type)) {
				// ֻ����û�б��������ʬ����ʱ����
				return SpawnCondition.OverworldNightMonster.Chance * 0.1f; // ��ͨ��ʬ1/10�����ɼ���
			}

			return 0f;
		}

		public override bool NeedSaving() {
			return StolenItems >= 10; // ��NPC���г���10����Ʒʱ���ᱻ���棬���������Ϊ�˷�ֹռ��̫���ڴ�
		}

		public override void SaveData(TagCompound tag) {
			if (StolenItems > 0) {
				// ������mod֮�����������NPC������һ������10��������Ʒ
				tag["StolenItems"] = StolenItems;
			}
		}

		public override void LoadData(TagCompound tag) {
			StolenItems = tag.GetInt("StolenItems");
		}
	}
}