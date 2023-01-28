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
	// 派对僵尸是一个普通的NPC示例。了解更多普通NPC的行为，点击（英文） https://github.com/tModLoader/tModLoader/wiki/Advanced-Vanilla-Code-Adaption#example-npc-npc-clone-with-modified-projectile-hoplite
		public class PartyZombie : ModNPC
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Party Zombie");
			DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "派对僵尸");

			Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Zombie];

			NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { // 影响这个NPC在生物图鉴中的形态
				Velocity = 1f // 在生物图鉴中NPC以+1图格的速度前进（向右）
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
			NPC.aiStyle = 3; // 3为战士AI，此处应选择与需求相匹配的内置AI

			AIType = NPCID.Zombie; // 使用僵尸的AI代码（意味着这个NPC将在白天卡其脱离太）
			AnimationType = NPCID.Zombie; // 调用僵尸的贴图帧数与处理方式，应当与SetStaticDefaults中的Main.npcFrameCount[NPC.type]保持同种生物
			Banner = Item.NPCtoBanner(NPCID.Zombie); // 这个生物会受到僵尸旗帜的伤害影响
			BannerItem = Item.BannerToItem(Banner); // 击杀这个生物会为获得僵尸旗帜增加计数
			SpawnModBiomes = new int[1] { ModContent.GetInstance<ExampleSurfaceBiome>().Type }; // 在生物图鉴中设置生物的展示背景为ExampleSurfaceBiome（示例地表群系）
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) {
			// 因为派对僵尸是一种普通僵尸的变体，我们想使用普通僵尸的掉落物
			// 为了实现这种效果，我们可以直接复制僵尸的掉落物，或者直接手搓和僵尸一样的掉落物
			// 直接复制的话当泰拉瑞亚更新导致僵尸掉落物变化时，你这也会跟着变
			// 直接手搓会有更大的自由度，你可以自定义很多东西，但最好先参考wiki、图鉴、源码再写

			// 下面的代码展示了如何直接复制某个NPC的掉落，为了保证MOD间的兼容性，最好选择最底层的NPC（如复制僵尸时不选择僵尸的变体而选择普通僵尸）
			var zombieDropRules = Main.ItemDropsDB.GetRulesForNPCID(NPCID.Zombie, false); // 这个false很重要
			foreach (var zombieDropRule in zombieDropRules) {
				// 通过foreach语句将所有僵尸的掉落添加到我们的NPC里面
				npcLoot.Add(zombieDropRule);
			}

			// 下面是重新手搓掉落物的写法，因为我们已经启用了上面的复制掉落物的写法，这里被注释掉了↓
			// npcLoot.Add(ItemDropRule.Common(ItemID.Shackle, 50)); // Drop shackles with a 1 out of 50 chance.
			// npcLoot.Add(ItemDropRule.Common(ItemID.ZombieArm, 250)); // Drop zombie arm with a 1 out of 250 chance.

			// 最后我们可以添加一些额外掉落物，下面这行就是添加的代码，很多僵尸的变体都有额外掉落物:（中文wiki） https://terraria.wiki.gg/zh/wiki/%E5%83%B5%E5%B0%B8
			npcLoot.Add(ItemDropRule.Common(ItemID.Confetti, 100)); // 1% 概率掉落彩纸
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) {
			return SpawnCondition.OverworldNightMonster.Chance * 0.2f; // 生成概率为普通僵尸的1/5
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			// 我们使用AddRange而不是多次使用Add来添加多个条目
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// 设置生物图鉴中显示的生成条件
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

				// 设置生物图鉴中的描述
				new FlavorTextBestiaryInfoElement("This type of zombie for some reason really likes to spread confetti around. Otherwise, it behaves just like a normal zombie."),

				// 默认最后一次调用的IBestiaryBackgroundImagePathAndColorProvider为生物图鉴显示的背景
				// 如果没有使用这个方法，会自动使用ExampleSurfaceBiome的ModBiomeBestiaryInfoElement作为背景
				// 所以我们用下面这个方法来确定生物图鉴中的背景
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<ExampleSurfaceBiome>().ModBiomeBestiaryInfoElement),
			});
		}

		public override void HitEffect(int hitDirection, double damage) {
			// 在这个NPC被击中时产生彩纸粒子效果

			for (int i = 0; i < 10; i++) {
				int dustType = Main.rand.Next(139, 143);
				var dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustType);

				dust.velocity.X += Main.rand.NextFloat(-0.05f, 0.05f);
				dust.velocity.Y += Main.rand.NextFloat(-0.05f, 0.05f);

				dust.scale *= 1f + Main.rand.NextFloat(-0.03f, 0.03f);
			}
		}

		public override void OnHitPlayer(Player target, int damage, bool crit) {
			// 这个方法会在NPC通过碰撞箱攻击玩家时调用，如果想要射弹命中的效果话一般写在射弹的文件里面
			// 一般用于添加受击Buff

			int buffType = ModContent.BuffType<AnimatedBuff>();
			// 或者你也可以这样直接用原版的Buff: int buffType = BuffID.Slow;

			int timeToAdd = 5 * 60; //持续5秒，1秒60帧
			target.AddBuff(buffType, timeToAdd);
		}
	}
}
