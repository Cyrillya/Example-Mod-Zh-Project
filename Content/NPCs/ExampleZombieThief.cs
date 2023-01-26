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
	// 这个NPC绝大多数时候与普通僵尸一样，但是会窃取（捡起地上的）ExampleItem并保存在自己身上，如果窃取的数量足够多，这个僵尸将会和物品一起保存在世界里
	public class ExampleZombieThief : ModNPC
	{
		public int StolenItems = 0;

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Example Zombie Thief");
			DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "窃贼僵尸");

			Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Zombie];

			NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) {
				// 影响僵尸在生物图鉴中的外观
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
			SpawnModBiomes = new int[] { ModContent.GetInstance<ExampleSurfaceBiome>().Type }; // 在生物图鉴中设置生物的展示背景为ExampleSurfaceBiome（示例地表群系）
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			// 我们使用AddRange而不是多次调用Add来添加多个条目
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// 设置生物图鉴中显示的生成条件
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

				// 设置生物图鉴中的描述
				new FlavorTextBestiaryInfoElement("This type of zombie really like Example Items. They steal them as soon as they find some."),
			});
		}

		public override void AI() {
			if (Main.netMode == NetmodeID.MultiplayerClient) {
				return;
			}

			Rectangle hitbox = NPC.Hitbox;
			foreach (Item item in Main.item) {
				// 当碰到地上掉落的ExampleItem时将其捡起
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

			// NPC死亡时掉落全部捡起的ExampleItem
			while (StolenItems > 0) {
				// 在物品掉落前不断循环，以防止超出数组范围
				int droppedAmount = Math.Min(ModContent.GetInstance<ExampleItem>().Item.maxStack, StolenItems);
				StolenItems -= droppedAmount;
				Item.NewItem(NPC.GetSource_Death(), NPC.Center, ModContent.ItemType<ExampleItem>(), droppedAmount, true);
			}
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) {
			if (spawnInfo.Player.InModBiome(ModContent.GetInstance<ExampleSurfaceBiome>()) // 只在ExampleSurfaceBiome中生成
			    && !NPC.AnyNPCs(Type)) {
				// 只会在没有别的窃贼僵尸存在时生成
				return SpawnCondition.OverworldNightMonster.Chance * 0.1f; // 普通僵尸1/10的生成几率
			}

			return 0f;
		}

		public override bool NeedSaving() {
			return StolenItems >= 10; // 当NPC持有超过10个物品时将会被保存，这个条件是为了防止占用太多内存
		}

		public override void SaveData(TagCompound tag) {
			if (StolenItems > 0) {
				// 如果别的mod之类的想存下这个NPC，他不一定持有10个以上物品
				tag["StolenItems"] = StolenItems;
			}
		}

		public override void LoadData(TagCompound tag) {
			StolenItems = tag.GetInt("StolenItems");
		}
	}
}