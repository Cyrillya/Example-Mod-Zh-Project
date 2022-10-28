using ExampleMod.Content.DamageClasses;
using Terraria;
// using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Accessories
{
	public class ExampleStatBonusAccessory : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("提升25%伤害, 直接乘算\n"
							 + "伤害提升至112%, 最终乘算\n"
							 + "提升4基础伤害, 直接加算\n"
							 + "提升5总伤害, 最终加算\n"
							 + "公式: 伤害 = (初始伤害 + 4) * (1 + 25%) * 112% + 5\n"
							 + "提升10%近战暴击率\n"
							 + "提升100%示例(伤害类型的)击退\n"
							 + "提升5魔法盔甲穿透\n"
							 + "提升15%远程攻速");

			// CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
			SacrificeTotal = 1; // 上一行与这一行的效果是一样的, 旅途研究所需数量, 只是下面这个更简洁, 还不需要 using Terraria.GameContent.Creative;
		}

		public override void SetDefaults() {
			Item.width = 40;
			Item.height = 40;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) {
			// GetDamage 返回特定伤害类型的 StatModifier 的引用
			// 因为它返回的是 StatModifier 的引用, 所以你可以自由地用数学运算符修改它 (+, -, *, / 等).
			// StatModifier 是一个分别保存了直接乘算 (additive) 和最终乘算 (multiplicative) 的乘数的结构体, 还包括对于基础伤害 (base) 和总伤害 (flat) 的加成
			// 当 StatModifier 被施加于一个值时, 其直接乘算的乘数在最终乘算的乘数前生效
			// 基础伤害直接加算于武器原版的伤害, 受其它伤害加成影响; 总伤害加成在其它加成后生效
			// 在此示例中, 我们:
			// - 提升25%伤害, 直接乘算. 典型的 "提升X%伤害" 用的就是这个
			// - 伤害提升至112%, 最终乘算. 原版中几乎没有用这个效果(蘑菇头, 箭术药水等), 一般用的是上面的 additive 乘数. 此种加成是极难平衡的
			// - 提升4基础伤害
			// - 提升5总伤害
			// 公式: 伤害 = (初始伤害 + 4 + 其它base加成) * (1 + 25% + 其它additive加成) * 112% * 其它multiplicative加成 + 5 + 其它flat加成
			// 因为我们用的是 DamageClass.Generic, 所有非"无"类型伤害受到了加成
			player.GetDamage(DamageClass.Generic) += 0.25f;
			player.GetDamage(DamageClass.Generic) *= 1.12f;
			player.GetDamage(DamageClass.Generic).Base += 4f;
			player.GetDamage(DamageClass.Generic).Flat += 5f;

			// GetCrit 返回特定伤害类型的暴击率的引用
			// 此示例中, 我们提升10%暴击率, 但只有近战伤害受到加成
			// 注意: 暴击率作为 float 计算完成后会被转换为 int
			player.GetCritChance(DamageClass.Melee) += 10f;

			// GetAttackSpeed 返回特定伤害类型的攻速的引用
			// 在此示例中, 我们使所有远程武器提升15%攻速
			// 注意: 若结果是0或负数, 将会抛出异常
			player.GetAttackSpeed(DamageClass.Ranged) += 0.15f;

			// GetArmorPenetration 是盔甲穿透, 相信你能举一反三
			// 注意: 盔甲穿透作为 float 计算完成后会被转换为 int
			player.GetArmorPenetration(DamageClass.Magic) += 5f;

			// GetKnockback 击退
			// 注意此例中应用模组伤害类型的方法
			player.GetKnockback<ExampleDamageClass>() += 1f;
		}
	}
}