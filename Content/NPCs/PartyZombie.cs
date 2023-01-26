using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader.Utilities;
using Terraria.DataStructures;
using ExampleMod.Content.Biomes;
using ExampleMod.Content.Buffs;
using Terraria.Localization;

namespace ExampleMod.Content.NPCs
{
	// �ɶԽ�ʬ��һ����ͨ��NPCʾ�����˽������ͨNPC����Ϊ�������Ӣ�ģ� https://github.com/tModLoader/tModLoader/wiki/Advanced-Vanilla-Code-Adaption#example-npc-npc-clone-with-modified-projectile-hoplite
		public class PartyZombie : ModNPC
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Party Zombie");
			DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "�ɶԽ�ʬ");

			Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Zombie];

			NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { // Ӱ�����NPC������ͼ���е���̬
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
			SpawnModBiomes = new int[1] { ModContent.GetInstance<ExampleSurfaceBiome>().Type }; // ������ͼ�������������չʾ����ΪExampleSurfaceBiome��ʾ���ر�Ⱥϵ��
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) {
			// ��Ϊ�ɶԽ�ʬ��һ����ͨ��ʬ�ı��壬������ʹ����ͨ��ʬ�ĵ�����
			// Ϊ��ʵ������Ч�������ǿ���ֱ�Ӹ��ƽ�ʬ�ĵ��������ֱ���ִ�ͽ�ʬһ���ĵ�����
			// ֱ�Ӹ��ƵĻ���̩�����Ǹ��µ��½�ʬ������仯ʱ������Ҳ����ű�
			// ֱ���ִ���и�������ɶȣ�������Զ���ܶණ����������Ȳο�wiki��ͼ����Դ����д

			// ����Ĵ���չʾ�����ֱ�Ӹ���ĳ��NPC�ĵ��䣬Ϊ�˱�֤MOD��ļ����ԣ����ѡ����ײ��NPC���縴�ƽ�ʬʱ��ѡ��ʬ�ı����ѡ����ͨ��ʬ��
			var zombieDropRules = Main.ItemDropsDB.GetRulesForNPCID(NPCID.Zombie, false); // ���false����Ҫ
			foreach (var zombieDropRule in zombieDropRules) {
				// ͨ��foreach��佫���н�ʬ�ĵ�����ӵ����ǵ�NPC����
				npcLoot.Add(zombieDropRule);
			}

			// �����������ִ�������д������Ϊ�����Ѿ�����������ĸ��Ƶ������д�������ﱻע�͵��ˡ�
			// npcLoot.Add(ItemDropRule.Common(ItemID.Shackle, 50)); // Drop shackles with a 1 out of 50 chance.
			// npcLoot.Add(ItemDropRule.Common(ItemID.ZombieArm, 250)); // Drop zombie arm with a 1 out of 250 chance.

			// ������ǿ������һЩ���������������о�����ӵĴ��룬�ܶཀྵʬ�ı��嶼�ж��������:������wiki�� https://terraria.wiki.gg/zh/wiki/%E5%83%B5%E5%B0%B8
			npcLoot.Add(ItemDropRule.Common(ItemID.Confetti, 100)); // 1% ���ʵ����ֽ
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) {
			return SpawnCondition.OverworldNightMonster.Chance * 0.2f; // ���ɸ���Ϊ��ͨ��ʬ��1/5
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			// ����ʹ��AddRange�����Ƕ��ʹ��Add����Ӷ����Ŀ
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// ��������ͼ������ʾ����������
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

				// ��������ͼ���е�����
				new FlavorTextBestiaryInfoElement("This type of zombie for some reason really likes to spread confetti around. Otherwise, it behaves just like a normal zombie."),

				// Ĭ�����һ�ε��õ�IBestiaryBackgroundImagePathAndColorProviderΪ����ͼ����ʾ�ı���
				// ���û��ʹ��������������Զ�ʹ��ExampleSurfaceBiome��ModBiomeBestiaryInfoElement��Ϊ����
				// �����������������������ȷ������ͼ���еı���
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<ExampleSurfaceBiome>().ModBiomeBestiaryInfoElement),
			});
		}

		public override void HitEffect(int hitDirection, double damage) {
			// �����NPC������ʱ������ֽ����Ч��

			for (int i = 0; i < 10; i++) {
				int dustType = Main.rand.Next(139, 143);
				var dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustType);

				dust.velocity.X += Main.rand.NextFloat(-0.05f, 0.05f);
				dust.velocity.Y += Main.rand.NextFloat(-0.05f, 0.05f);

				dust.scale *= 1f + Main.rand.NextFloat(-0.03f, 0.03f);
			}
		}

		public override void OnHitPlayer(Player target, int damage, bool crit) {
			// �����������NPCͨ����ײ�乥�����ʱ���ã������Ҫ�䵯���е�Ч����һ��д���䵯���ļ�����
			// һ����������ܻ�Buff

			int buffType = ModContent.BuffType<AnimatedBuff>();
			// ������Ҳ��������ֱ����ԭ���Buff: int buffType = BuffID.Slow;

			int timeToAdd = 5 * 60; //����5�룬1��60֡
			target.AddBuff(buffType, timeToAdd);
		}
	}
}
