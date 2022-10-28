using ExampleMod.Content.Items;
using ExampleMod.Content.Tiles.Furniture;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles.Minions
{
    // 这个文件里包含着该召唤物所需的全部代码
    // - ModItem - 用来召唤你召唤物的武器
    // - ModBuff - 右键点击buff图标来取消你的召唤物
    // - ModProjectile - 召唤物本身

    // 一般来说，是不建议把这些类放在一个文件里的。不过这是演示嘛，所以为了方便你预览召唤物的代码，就都放在一个文件里了
    // 如果你想更好理解这些东西的运作，以及如何写出一个ai，请阅读这个指南: https://github.com/tModLoader/tModLoader/wiki/Basic-Minion-Guide
    // 这不是给老手看的深入教学指南

    public class ExampleSimpleMinionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Example Minion");
            Description.SetDefault("The example minion will fight for you");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "示例召唤物");
            Description.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "示例召唤物将会为你而战");

            Main.buffNoSave[Type] = true; // 当你离开世界后buff将会消失
            Main.buffNoTimeDisplay[Type] = true; // 该buff的剩余时间不会进行显示
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // 如果召唤物存在就重置该buff的剩余时间，否则移除该buff
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ExampleSimpleMinion>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }

    public class ExampleSimpleMinionItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Example Minion Item");
            Tooltip.SetDefault("Summons an example minion to fight for you");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "示例召唤物武器");
            Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "召唤示例召唤物为你而战");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; // 设置该物品在旅途模式研究所需要的数量
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // 可以让该武器在整个屏幕的任意位置释放召唤物
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true; // 可以让手柄的锁定功能无视墙壁的存在
        }

        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.knockBack = 3f;
            Item.mana = 10; // 消耗的魔力
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing; // 武器的使用方式（这里是阔剑所使用的挥舞动作）
            Item.value = Item.sellPrice(gold: 30);
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item44; // 使用该武器时所播放的音效

            // 下面这些是召唤武器所需要的一些东西
            Item.noMelee = true; // 使用该物品不会造成伤害
            Item.DamageType = DamageClass.Summon; // 设置伤害类型为召唤伤害。如果你不设置它的话，那么这把武器无法吃到任何伤害加成。请确保你设置了伤害类型
            Item.buffType = ModContent.BuffType<ExampleSimpleMinionBuff>();
            // 不设置buff持续时间，不然工具提示里会有诸如1分钟持续时间这种东西
            Item.shoot = ModContent.ProjectileType<ExampleSimpleMinion>(); // 设置成你的召唤物射弹，使用时就会召唤这个射弹
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // 在这个重写函数里改变 position 既可修改射弹的召唤地点。一般来说召唤物会在光标处生成
            position = Main.MouseWorld;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // 刚才不是没给玩家上buff嘛，所以需要在这里手动给玩家上buff
            player.AddBuff(Item.buffType, 2);

            // 召唤物射弹需要手动召唤，召唤时再把这个射弹的 originalDamage 设置成和武器一样的伤害
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;

            // 因为你已经手动召唤了射弹，自然就没必要让它再召唤一个，所以 return false
            return false;
        }

        // 看不懂下面的合成表？没关系，看看 Content/ExampleRecipes.cs 这个文件，这里头有教你如何写合成表
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ExampleItem>())
                .AddTile(ModContent.TileType<ExampleWorkbench>())
                .Register();
        }
    }

    // 这个召唤物ai展示了一些可能有些复杂的ai，但是我相信你看完这个文件后应该就能读懂这ai的实现了
    // 这个召唤物的进攻ai很简单: 如果有敌怪在召唤物周围43格图块（半径）内，它就会飞向敌怪进行接触伤害（如有多个敌怪则为离召唤物最近的那个）
    // 如果玩家锁定了某个敌怪，那么召唤物会直接朝被锁定的敌怪飞过去进行接触伤害
    // 如果没有敌怪符合可被攻击的条件，那么召唤物就会以一个很小的速度在玩家周围漂浮
    public class ExampleSimpleMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Example Minion");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "示例召唤物");
            // 设置该召唤物的帧图数量
            Main.projFrames[Projectile.type] = 4;
            // 如果你想拥有锁定敌怪功能那就开启这个
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            Main.projPet[Projectile.type] = true; // 让tml知道这东西是个宠物或召唤物

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // 这个东西很必要，可以在你召唤新召唤物时正确的顶替掉旧召唤物
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // 让拜月邪教教徒对这种召唤物有减伤（一般而言拜月邪教教徒会对带追踪的射弹有减伤）
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 28;
            Projectile.tileCollide = false; // 让召唤物可以穿过物块

            // 下面这些是一个召唤物射弹所需要的一些东西
            Projectile.friendly = true; // 在接触时可对敌怪造成伤害（稍后会详细解释）
            Projectile.minion = true; // 将它声明为召唤物射弹（效果挺多的，不细讲了）
            Projectile.DamageType = DamageClass.Summon; // 声明该射弹的伤害类型
            Projectile.minionSlots = 1f; // 该召唤物会占据玩家多少召唤栏位（稍后会详细解释）
            Projectile.penetrate = -1; // 将它调为-1以达成无限穿透（除非你想要你的召唤物非正常死亡）
        }

        // 这个重写函数决定你的召唤物是否可以破坏诸如草或罐子之类的东西
        public override bool? CanCutTiles()
        {
            return false;
        }

        // 决定你召唤物是否造成接触伤害的重写函数，对于召唤物来说很重要
        public override bool MinionContactDamage()
        {
            return true;
        }

        // 这个召唤物的ai被分成了多个方法以防止太长导致看着头疼。所以只需要在ai重写函数里给各个方法传递参数即可
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (!CheckActive(owner))
            {
                return;
            }

            GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
            SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            Movement(foundTarget, distanceFromTarget, targetCenter, distanceToIdlePosition, vectorToIdlePosition);
            Visuals();
        }

        // 这里是召唤物的存活检测。如果玩家活着且玩家拥有召唤物buff，那么召唤物也会存活，反之则死
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<ExampleSimpleMinionBuff>());

                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<ExampleSimpleMinionBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition)
        {
            Vector2 idlePosition = owner.Center;
            idlePosition.Y -= 48f; // 向上移动48个像素（距离玩家中心3格图块）

            // If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
            // The index is projectile.minionPos
            float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -owner.direction;
            idlePosition.X += minionPositionOffsetX; // Go behind the player

            // All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

            // Teleport to player if distance is too big
            vectorToIdlePosition = idlePosition - Projectile.Center;
            distanceToIdlePosition = vectorToIdlePosition.Length();

            if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
            {
                // Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
                // and then set netUpdate to true
                Projectile.position = idlePosition;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }

            // If your minion is flying, you want to do this independently of any conditions
            float overlapVelocity = 0.04f;

            // Fix overlap with other minions
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];

                if (i != Projectile.whoAmI && other.active && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    if (Projectile.position.X < other.position.X)
                    {
                        Projectile.velocity.X -= overlapVelocity;
                    }
                    else
                    {
                        Projectile.velocity.X += overlapVelocity;
                    }

                    if (Projectile.position.Y < other.position.Y)
                    {
                        Projectile.velocity.Y -= overlapVelocity;
                    }
                    else
                    {
                        Projectile.velocity.Y += overlapVelocity;
                    }
                }
            }
        }

        private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter)
        {
            // Starting search distance
            distanceFromTarget = 700f;
            targetCenter = Projectile.position;
            foundTarget = false;

            // This code is required if your minion weapon has the targeting feature
            if (owner.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                float between = Vector2.Distance(npc.Center, Projectile.Center);

                // Reasonable distance away so it doesn't target across multiple screens
                if (between < 2000f)
                {
                    distanceFromTarget = between;
                    targetCenter = npc.Center;
                    foundTarget = true;
                }
            }

            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];

                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center);
                        bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;

                        if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }

            // friendly needs to be set to true so the minion can deal contact damage
            // friendly needs to be set to false so it doesn't damage things like target dummies while idling
            // Both things depend on if it has a target or not, so it's just one assignment here
            // You don't need this assignment if your minion is shooting things instead of dealing contact damage
            Projectile.friendly = foundTarget;
        }

        private void Movement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition)
        {
            // Default movement parameters (here for attacking)
            float speed = 8f;
            float inertia = 20f;

            if (foundTarget)
            {
                // Minion has a target: attack (here, fly towards the enemy)
                if (distanceFromTarget > 40f)
                {
                    // The immediate range around the target (so it doesn't latch onto it when close)
                    Vector2 direction = targetCenter - Projectile.Center;
                    direction.Normalize();
                    direction *= speed;

                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                }
            }
            else
            {
                // Minion doesn't have a target: return to player and idle
                if (distanceToIdlePosition > 600f)
                {
                    // Speed up the minion if it's away from the player
                    speed = 12f;
                    inertia = 60f;
                }
                else
                {
                    // Slow down the minion if closer to the player
                    speed = 4f;
                    inertia = 80f;
                }

                if (distanceToIdlePosition > 20f)
                {
                    // The immediate range around the player (when it passively floats about)

                    // This is a simple movement formula using the two parameters and its desired direction to create a "homing" movement
                    vectorToIdlePosition.Normalize();
                    vectorToIdlePosition *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
                }
                else if (Projectile.velocity == Vector2.Zero)
                {
                    // If there is a case where it's not moving at all, give it a little "poke"
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
            }
        }

        private void Visuals()
        {
            // So it will lean slightly towards the direction it's moving
            Projectile.rotation = Projectile.velocity.X * 0.05f;

            // This is a simple "loop through all frames from top to bottom" animation
            int frameSpeed = 5;

            Projectile.frameCounter++;

            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }
    }
}
