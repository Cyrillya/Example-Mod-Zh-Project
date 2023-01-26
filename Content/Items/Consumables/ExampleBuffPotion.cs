using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace ExampleMod.Content.Items.Consumables
{
	public class ExampleBuffPotion : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("Gives a light defense buff.");
			Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "������ͷ���Ч��");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 20;

			// ��ʹ����ItemUseStyleID.DrinkLiquid����Ʒʱ�����������ɫ������Ч��
			ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
				new Color(240, 240, 240),
				new Color(200, 200, 200),
				new Color(140, 140, 140)
			};
		}

		public override void SetDefaults() {
			Item.width = 20;
			Item.height = 26;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useAnimation = 15;
			Item.useTime = 15;
			Item.useTurn = true;
			Item.UseSound = SoundID.Item3;
			Item.maxStack = 30;
			Item.consumable = true;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(gold: 1);
			Item.buffType = ModContent.BuffType<Buffs.ExampleDefenseBuff>(); // ָ��ʹ��ʱ��õ�buff
			Item.buffTime = 5400; // buff�ĳ���ʱ�䣬5400/60=90,�������buff�����90��
		}
	}
}
