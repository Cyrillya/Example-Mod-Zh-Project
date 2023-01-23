using ExampleMod.Content.Biomes;
using ExampleMod.Content.Dusts;
using ExampleMod.Content.Items;
using ExampleMod.Content.Items.Accessories;
using ExampleMod.Content.Items.Armor;
using ExampleMod.Content.Tiles;
using ExampleMod.Content.Tiles.Furniture;
using ExampleMod.Content.Walls;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using Terraria.DataStructures;
using System.Collections.Generic;
using ReLogic.Content;
using Terraria.ModLoader.IO;

namespace ExampleMod.Content.NPCs
{
	// [AutoloadHead] 这个对于城镇NPC来说非常重要，如果你想让你的城镇NPC有图标的话，你就加上
	[AutoloadHead]
	public class ExamplePerson : ModNPC
	{
		public int NumberOfTimesTalkedTo = 0;

		public override void SetStaticDefaults() {
			// DisplayName从本地化文件自动匹配，但下面的注释行是正常的方法
			// DisplayName.SetDefault("Example Person");
			Main.npcFrameCount[Type] = 25; // NPC所拥有的帧数

			NPCID.Sets.ExtraFramesCount[Type] = 9; // 通常是城镇NPC，但这是NPC如何做额外的事情，如坐在椅子上和其他NPC交谈
			NPCID.Sets.AttackFrameCount[Type] = 4;
			NPCID.Sets.DangerDetectRange[Type] = 700; // NPC试图攻击敌人的npc中心的范围（像素）
			NPCID.Sets.AttackType[Type] = 0;
			NPCID.Sets.AttackTime[Type] = 90; // NPC攻击动画开始后结束所需的时间
			NPCID.Sets.AttackAverageChance[Type] = 30;
			NPCID.Sets.HatOffsetY[Type] = 4; // 当派对开始时，被帽子替换的Y偏移（也就是帽子的所在位置）

			// 这个是绘制NPC在图鉴里的形象
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0) {
				Velocity = 1f, // 让NPC可以在图鉴中行走（就像是其他的城镇NPC一样)
				Direction = 1 // -1是左，1是右。npc在默认情况下是面向左侧绘制的，但ExamplePerson将面向右侧绘制
				// Rotation = MathHelper.ToRadians(180) // 你也可以改变NPC的旋转。旋转以弧度为单位
				// 如果您想查看在绘制NPC时手动修改这些内容的示例，请参阅PreDraw
			};

			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

			// 使用NPCHappiness设置Person的生物群落和邻居偏好。你可以在本地化中添加幸福文本和备注(参见exampleemod / localization /en-US.lang中的例子)。
			// NOTE: The following code uses chaining - a style that works due to the fact that the SetXAffection methods return the same NPCHappiness instance they're called on.
			NPC.Happiness
				.SetBiomeAffection<ForestBiome>(AffectionLevel.Like) // Example Person 更喜欢森林
				.SetBiomeAffection<SnowBiome>(AffectionLevel.Dislike) // Example Person 不喜欢雪原
				.SetBiomeAffection<ExampleSurfaceBiome>(AffectionLevel.Love) // Example Person 喜欢 Example Surface Biome（生物群系）
				.SetNPCAffection(NPCID.Dryad, AffectionLevel.Love) // 喜欢住在树妖附近
				.SetNPCAffection(NPCID.Guide, AffectionLevel.Like) // 喜欢住在向导附近
				.SetNPCAffection(NPCID.Merchant, AffectionLevel.Dislike) // 不喜欢住在商人附近
				.SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Hate) // 不喜欢住在爆破专家附近
			; // < Mind the semicolon!
		}

		public override void SetDefaults() {
			NPC.townNPC = true; // 设定NPC为城镇NPC
			NPC.friendly = true; // NPC将不会攻击玩家
			NPC.width = 18;
			NPC.height = 40;
			NPC.aiStyle = 7;
			NPC.damage = 10;
			NPC.defense = 15;
			NPC.lifeMax = 250;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0.5f;

			AnimationType = NPCID.Guide;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			// 我们可以使用AddRange而不是多次调用Add，以便一次添加多个项目
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// 设定图鉴中NPC的首选生物群系
				// 对于城镇NPC，您通常会将其设置为它最喜欢的NPC生物群系。
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				// 设定城镇NPC在图鉴中的文本（也就是描述）
				new FlavorTextBestiaryInfoElement("Hailing from a mysterious greyscale cube world, the Example Person is here to help you understand everything about tModLoader."),

				// 如果你想，你可以添加更多
				// 您还可以使用本地化（请参阅 Localization/en-US.lang）
				new FlavorTextBestiaryInfoElement("Mods.ExampleMod.Bestiary.ExamplePerson")
			});
		}

		// The PreDraw hook is useful for drawing things before our sprite is drawn or running code before the sprite is drawn
		// Returning false will allow you to manually draw your NPC
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
			// This code slowly rotates the NPC in the bestiary
			// (simply checking NPC.IsABestiaryIconDummy and incrementing NPC.Rotation won't work here as it gets overridden by drawModifiers.Rotation each tick)
			if (NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(Type, out NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers)) {
				drawModifiers.Rotation += 0.001f;

				// Replace the existing NPCBestiaryDrawModifiers with our new one with an adjusted rotation
				NPCID.Sets.NPCBestiaryDrawOffset.Remove(Type);
				NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
			}

			return true;
		}

		public override void HitEffect(int hitDirection, double damage) {
			int num = NPC.life > 0 ? 1 : 5;

			for (int k = 0; k < num; k++) {
				Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<Sparkle>());
			}
		}

		public override bool CanTownNPCSpawn(int numTownNPCs, int money) { // Requirements for the town NPC to spawn.
			for (int k = 0; k < 255; k++) {
				Player player = Main.player[k];
				if (!player.active) {
					continue;
				}

				// Player has to have either an ExampleItem or an ExampleBlock in order for the NPC to spawn
				if (player.inventory.Any(item => item.type == ModContent.ItemType<ExampleItem>() || item.type == ModContent.ItemType<Items.Placeable.ExampleBlock>())) {
					return true;
				}
			}

			return false;
		}

		// Example Person needs a house built out of ExampleMod tiles. You can delete this whole method in your townNPC for the regular house conditions.
		public override bool CheckConditions(int left, int right, int top, int bottom) {
			int score = 0;
			for (int x = left; x <= right; x++) {
				for (int y = top; y <= bottom; y++) {
					int type = Main.tile[x, y].TileType;
					if (type == ModContent.TileType<ExampleBlock>() || type == ModContent.TileType<ExampleChair>() || type == ModContent.TileType<ExampleWorkbench>() || type == ModContent.TileType<ExampleBed>() || type == ModContent.TileType<ExampleDoorOpen>() || type == ModContent.TileType<ExampleDoorClosed>()) {
						score++;
					}

					if (Main.tile[x, y].WallType == ModContent.WallType<ExampleWall>()) {
						score++;
					}
				}
			}

			return score >= ((right - left) * (bottom - top)) / 2;
		}

		public override ITownNPCProfile TownNPCProfile() {
			return new ExamplePersonProfile();
		}

		public override List<string> SetNPCNameList() {
			return new List<string>() {
				"Someone",
				"Somebody",
				"Blocky",
				"Colorless"
			};
		}

		public override void FindFrame(int frameHeight) {
			/*npc.frame.Width = 40;
			if (((int)Main.time / 10) % 2 == 0)
			{
				npc.frame.X = 40;
			}
			else
			{
				npc.frame.X = 0;
			}*/
		}

		public override string GetChat() {
			WeightedRandom<string> chat = new WeightedRandom<string>();

			int partyGirl = NPC.FindFirstNPC(NPCID.PartyGirl);
			if (partyGirl >= 0 && Main.rand.NextBool(4)) {
				chat.Add(Language.GetTextValue("Mods.ExampleMod.Dialogue.ExamplePerson.PartyGirlDialogue", Main.npc[partyGirl].GivenName));
			}
			// These are things that the NPC has a chance of telling you when you talk to it.
			chat.Add(Language.GetTextValue("Mods.ExampleMod.Dialogue.ExamplePerson.StandardDialogue1"));
			chat.Add(Language.GetTextValue("Mods.ExampleMod.Dialogue.ExamplePerson.StandardDialogue2"));
			chat.Add(Language.GetTextValue("Mods.ExampleMod.Dialogue.ExamplePerson.StandardDialogue3"));
			chat.Add(Language.GetTextValue("Mods.ExampleMod.Dialogue.ExamplePerson.CommonDialogue"), 5.0);
			chat.Add(Language.GetTextValue("Mods.ExampleMod.Dialogue.ExamplePerson.RareDialogue"), 0.1);

			NumberOfTimesTalkedTo++;
			if (NumberOfTimesTalkedTo >= 10) {
				//This counter is linked to a single instance of the NPC, so if ExamplePerson is killed, the counter will reset.
				chat.Add(Language.GetTextValue("Mods.ExampleMod.Dialogue.ExamplePerson.TalkALot"));
			}

			return chat; // chat is implicitly cast to a string.
		}

		public override void SetChatButtons(ref string button, ref string button2) { // What the chat buttons are when you open up the chat UI
			button = Language.GetTextValue("LegacyInterface.28");
			button2 = "Awesomeify";
			if (Main.LocalPlayer.HasItem(ItemID.HiveBackpack)) {
				button = "Upgrade " + Lang.GetItemNameValue(ItemID.HiveBackpack);
			}
		}

		public override void OnChatButtonClicked(bool firstButton, ref bool shop) {
			if (firstButton) {
				// We want 3 different functionalities for chat buttons, so we use HasItem to change button 1 between a shop and upgrade action.

				if (Main.LocalPlayer.HasItem(ItemID.HiveBackpack)) {
					SoundEngine.PlaySound(SoundID.Item37); // Reforge/Anvil sound

					Main.npcChatText = $"I upgraded your {Lang.GetItemNameValue(ItemID.HiveBackpack)} to a {Lang.GetItemNameValue(ModContent.ItemType<WaspNest>())}";

					int hiveBackpackItemIndex = Main.LocalPlayer.FindItem(ItemID.HiveBackpack);
					var entitySource = NPC.GetSource_GiftOrReward();

					Main.LocalPlayer.inventory[hiveBackpackItemIndex].TurnToAir();
					Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<WaspNest>());

					return;
				}

				shop = true;
			}
		}

		// Not completely finished, but below is what the NPC will sell

		// public override void SetupShop(Chest shop, ref int nextSlot) {
		// 	shop.item[nextSlot++].SetDefaults(ItemType<ExampleItem>());
		// 	// shop.item[nextSlot].SetDefaults(ItemType<EquipMaterial>());
		// 	// nextSlot++;
		// 	// shop.item[nextSlot].SetDefaults(ItemType<BossItem>());
		// 	// nextSlot++;
		// 	shop.item[nextSlot++].SetDefaults(ItemType<Items.Placeable.Furniture.ExampleWorkbench>());
		// 	shop.item[nextSlot++].SetDefaults(ItemType<Items.Placeable.Furniture.ExampleChair>());
		// 	shop.item[nextSlot++].SetDefaults(ItemType<Items.Placeable.Furniture.ExampleDoor>());
		// 	shop.item[nextSlot++].SetDefaults(ItemType<Items.Placeable.Furniture.ExampleBed>());
		// 	shop.item[nextSlot++].SetDefaults(ItemType<Items.Placeable.Furniture.ExampleChest>());
		// 	shop.item[nextSlot++].SetDefaults(ItemType<ExamplePickaxe>());
		// 	shop.item[nextSlot++].SetDefaults(ItemType<ExampleHamaxe>());
		//
		// 	if (Main.LocalPlayer.HasBuff(BuffID.Lifeforce)) {
		// 		shop.item[nextSlot++].SetDefaults(ItemType<ExampleHealingPotion>());
		// 	}
		//
		// 	// if (Main.LocalPlayer.GetModPlayer<ExamplePlayer>().ZoneExample && !GetInstance<ExampleConfigServer>().DisableExampleWings) {
		// 	// 	shop.item[nextSlot].SetDefaults(ItemType<ExampleWings>());
		// 	// 	nextSlot++;
		// 	// }
		//
		// 	if (Main.moonPhase < 2) {
		// 		shop.item[nextSlot++].SetDefaults(ItemType<ExampleSword>());
		// 	}
		// 	else if (Main.moonPhase < 4) {
		// 		// shop.item[nextSlot++].SetDefaults(ItemType<ExampleGun>());
		// 		shop.item[nextSlot].SetDefaults(ItemType<ExampleBullet>());
		// 	}
		// 	else if (Main.moonPhase < 6) {
		// 		// shop.item[nextSlot++].SetDefaults(ItemType<ExampleStaff>());
		// 	}
		//
		// 	// todo: Here is an example of how your npc can sell items from other mods.
		// 	// var modSummonersAssociation = ModLoader.TryGetMod("SummonersAssociation");
		// 	// if (ModLoader.TryGetMod("SummonersAssociation", out Mod modSummonersAssociation)) {
		// 	// 	shop.item[nextSlot].SetDefaults(modSummonersAssociation.ItemType("BloodTalisman"));
		// 	// 	nextSlot++;
		// 	// }
		//
		// 	// if (!Main.LocalPlayer.GetModPlayer<ExamplePlayer>().examplePersonGiftReceived && GetInstance<ExampleConfigServer>().ExamplePersonFreeGiftList != null) {
		// 	// 	foreach (var item in GetInstance<ExampleConfigServer>().ExamplePersonFreeGiftList) {
		// 	// 		if (Item.IsUnloaded) continue;
		// 	// 		shop.item[nextSlot].SetDefaults(Item.Type);
		// 	// 		shop.item[nextSlot].shopCustomPrice = 0;
		// 	// 		shop.item[nextSlot].GetGlobalItem<ExampleInstancedGlobalItem>().examplePersonFreeGift = true;
		// 	// 		nextSlot++;
		// 	// 		//TODO: Have tModLoader handle index issues.
		// 	// 	}
		// 	// }
		// }

		public override void ModifyNPCLoot(NPCLoot npcLoot) {
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ExampleCostume>()));
		}

		// Make this Town NPC teleport to the King and/or Queen statue when triggered.
		public override bool CanGoToStatue(bool toKingStatue) => true;

		// Make something happen when the npc teleports to a statue. Since this method only runs server side, any visual effects like dusts or gores have to be synced across all clients manually.
		public override void OnGoToStatue(bool toKingStatue) {
			if (Main.netMode == NetmodeID.Server) {
				ModPacket packet = Mod.GetPacket();
				packet.Write((byte)ExampleMod.MessageType.ExampleTeleportToStatue);
				packet.Write((byte)NPC.whoAmI);
				packet.Send();
			}
			else {
				StatueTeleport();
			}
		}

		// Create a square of pixels around the NPC on teleport.
		public void StatueTeleport() {
			for (int i = 0; i < 30; i++) {
				Vector2 position = Main.rand.NextVector2Square(-20, 21);
				if (Math.Abs(position.X) > Math.Abs(position.Y)) {
					position.X = Math.Sign(position.X) * 20;
				}
				else {
					position.Y = Math.Sign(position.Y) * 20;
				}

				Dust.NewDustPerfect(NPC.Center + position, ModContent.DustType<Sparkle>(), Vector2.Zero).noGravity = true;
			}
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback) {
			damage = 20;
			knockback = 4f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown) {
			cooldown = 30;
			randExtraCooldown = 30;
		}

		// todo: implement
		// public override void TownNPCAttackProj(ref int projType, ref int attackDelay) {
		// 	projType = ProjectileType<SparklingBall>();
		// 	attackDelay = 1;
		// }

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset) {
			multiplier = 12f;
			randomOffset = 2f;
		}

		public override void LoadData(TagCompound tag) {
			NumberOfTimesTalkedTo = tag.GetInt("numberOfTimesTalkedTo");
		}

		public override void SaveData(TagCompound tag) {
			tag["numberOfTimesTalkedTo"] = NumberOfTimesTalkedTo;
		}
	}

	public class ExamplePersonProfile : ITownNPCProfile
	{
		public int RollVariation() => 0;
		public string GetNameForVariant(NPC npc) => npc.getNewNPCName();

		public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc) {
			if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
				return ModContent.Request<Texture2D>("ExampleMod/Content/NPCs/ExamplePerson");

			if (npc.altTexture == 1)
				return ModContent.Request<Texture2D>("ExampleMod/Content/NPCs/ExamplePerson_Party");

			return ModContent.Request<Texture2D>("ExampleMod/Content/NPCs/ExamplePerson");
		}

		public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("ExampleMod/Content/NPCs/ExamplePerson_Head");
	}
}
