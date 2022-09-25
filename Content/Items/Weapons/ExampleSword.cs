using ExampleMod.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Weapons
{
	public class ExampleSword : ModItem
	{
		public override void SetStaticDefaults() {
			// 这是这把武器的工具提示，也就是显示在物品基础属性下方的那一串文字
			Tooltip.SetDefault("This is a modded sword.");
			// 下面这行给工具提示添加中文翻译，则如果游戏语言为中文就会显示“这是一把剑”
			Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "这是一把剑");

			// 这是武器的名字
			DisplayName.SetDefault("Example Sword");
			DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "例子剑");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() {
			Item.width = 40; // 物品贴图的宽度(像素)
			Item.height = 40; // 物品贴图的高度(像素)

			// 物品的使用类型
			// 附: 使用类型ID中文Wiki: https://terraria.wiki.gg/zh/wiki/%E4%BD%BF%E7%94%A8%E7%B1%BB%E5%9E%8B_ID
			Item.useStyle = ItemUseStyleID.Swing;

			// 物品实际使用一次所需的时间，在泰拉里60帧=1秒
			// 这里20就是20帧时长，也就是说用一次物品需要 1/3秒
			Item.useTime = 20;
			Item.useAnimation = 20; // 物品使用动画播放一次所需的时间，这里建议和 useTime 设置成一样的值
			
			// 这个物品默认能不能自动挥舞
			// 附: 自动挥舞中文Wiki: https://terraria.wiki.gg/zh/wiki/%E8%87%AA%E5%8A%A8%E6%8C%A5%E8%88%9E
			Item.autoReuse = true;

			// 这个物品的伤害类型
			// Default - 默认伤害类型 (不受任何加成影响)
			// Generic - 全职业伤害类型 (受到来自所有职业的伤害加成，一般装备给全职业加伤害就是加在这个上面)
			// Melee - 近战
			// MeleeNoSpeed - 近战 (不受近战攻速加成，通常用于短剑、回旋镖、悠悠球之类的)
			// Ranged - 远程
			// Magic - 魔法
			// Summon - 召唤 (通常用于召唤杖)
			// SummonMeleeSpeed - 召唤 (攻速受近战攻速加成影响的武器，通常用于鞭子)
			// MagicSummonHybrid - 同时受到魔法和召唤伤害加成
			// Throwing - 投掷 (一个在1.4被移除的伤害类型，tML把他加回来了)
			Item.DamageType = DamageClass.Melee;
			Item.damage = 50; // 物品基础伤害
			Item.knockBack = 6; // 物品击退力，最大值为20，详见Wiki: https://terraria.wiki.gg/zh/wiki/%E5%87%BB%E9%80%80
			Item.crit = 6; // 物品本身所具有的基础暴击率，玩家的默认基础暴击率为 4%

			// 物品的价格，这里使用 buyPrice 也就是买入价，gold: 1 也就是一金的买入价
			// 而物品出售价=买入价/5，1金=100银，所以这个物品的出售价就是20银
			Item.value = Item.buyPrice(gold: 1);
			// 用 sellPrice 可以直接设置物品的出售价，也就是说上面这行等价于下面这行:
			// Item.value = Item.sellPrice(silver: 20);

			Item.rare = ModContent.RarityType<ExampleModRarity>(); // 给予这个物品自定义稀有度
			Item.UseSound = SoundID.Item1; // 物品使用时发出的声音
		}

		public override void MeleeEffects(Player player, Rectangle hitbox) {
			if (Main.rand.NextBool(3)) {
				// 在挥剑时生成粒子
				Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<Dusts.Sparkle>());
			}
		}

		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit) {
			// 打到敌怪时给对方附上1s的着火减益
			// 60就是持续时间为60帧的意思，60帧=1秒
			target.AddBuff(BuffID.OnFire, 60);
		}

		// 这里写的是合成配方，合成配方在 Content/ExampleRecipes.cs 有更详尽的介绍
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}
