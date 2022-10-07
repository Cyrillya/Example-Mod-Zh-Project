using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content.DamageClasses
{
	public class ExampleDamageClass : DamageClass
	{
		// 此示例伤害类型被设计用于展示 DamageClass 的现有功能并教你如何自己写一个
		// 至于如何对特定的伤害类型应用加成, 参见 ExampleMod/Content/Items/Accessories/ExampleStatBonusAccessory
		public override StatInheritanceData GetModifierInheritance(DamageClass damageClass) {
			// 此方法让你的伤害类型能够享受其它伤害类型的加成, 也包括通用伤害类型
			// 简要总结一下伤害类型使用的两种非标准名称:
			// Default (默认), 是的你已经猜到了, 就是默认的伤害类型. 它不会受到任何特定伤害类型或通用伤害类型加成的影响
			// 有相当一部分原版物品和射弹用的是这个伤害类型, 如投掷水瓶和骨头手套的十字骨
			// Geberic (通用), 与之相反, 受所有伤害类型的加成影响. 这是除了 Default 以外所有伤害类型的基础
			if (damageClass == DamageClass.Generic)
				return StatInheritanceData.Full;

			return new StatInheritanceData(
				damageInheritance: 0f, // 伤害继承
				critChanceInheritance: 0f, // 暴击率继承
				attackSpeedInheritance: 0f, // 攻速继承
				armorPenInheritance: 0f, // 盔甲穿透继承
				knockbackInheritance: 0f // 击退继承
			);
			// 现在, 你可能会问: "上面那些代码是什么意思?". 让我们来看看:
			// StatInheritanceData 是一个你需要返回的结构体
			// 一般的, 上面两个 StatInheritanceData 的后者应当被写为 "StatInheritanceData.None" 而不是将变量一个一个写出来...
			// ...但为了讲得清楚一点, 这里我们把每一个变量都写出来并赋值; 它们的作用应当是一目了然的
			// 每个返回值是加成所继承的百分比, 0f是0%, 1f即为100%, 以此类推
			// 这个百分比决定了你的伤害类型受指定的加成类型多少影响
			// 如果你创建了一个 StatInheritanceData 而不给变量赋值, 所有的变量将会被设为1f
			// 举个例子, 假设我们为 DamageClass.Ranged 返回一个不同的 StatInheritanceData...
			/*
			if (damageClass == DamageClass.Ranged)
				return new StatInheritanceData(
					damageInheritance: 1f,
					critChanceInheritance: -1f,
					attackSpeedInheritance: 0.4f,
					armorPenInheritance: 2.5f,
					knockbackInheritance: 0f
				);
			*/
			// 则远程伤害加成会对此伤害类型产生以下效果:
			// 伤害, 受100%加成
			// 攻速, 受40%加成
			// 暴击率, 受-100%加成 (也就是说远程暴击的加成反而会等量地减少此伤害的暴击率)
			// 盔甲穿透, 受250%加成

			// 警 告: 这些数值没有内置的上下限. 你所设置的数值 (比如非常大或非常小的数值) 有可能导致意外后果
			// 由于你病态的好奇心, 而对你, 你的角色或你的世界造成的, 任何临时或永久性的伤害, 我们不负责任

			// 要使用非原版的伤害类型, 请用 "ModContent.GetInstance<伤害类型>()" 来替代 "DamageClass.伤害类型"
		}

		public override bool GetEffectInheritance(DamageClass damageClass) {
			// 此方法允许你使你的伤害类型触发本该由其它伤害类型触发的效果 (如岩浆石只对近战伤害生效)
			// 不像上面的属性继承, 你不需要在此方法里写通用加成
			// 举个例子, 下面我们使此伤害类型能够触发近战和魔法伤害的效果
			if (damageClass == DamageClass.Melee)
				return true;
			if (damageClass == DamageClass.Magic)
				return true;

			return false;
		}

		public override void SetDefaultStats(Player player) {
			// 此方法让你设置此伤害类型的默认属性加成 (像原版的伤害默认有+4%暴击率)
			// 此处我们使其默认拥有+4%暴击率和+10盔甲穿透
			player.GetCritChance<ExampleDamageClass>() += 4;
			player.GetArmorPenetration<ExampleDamageClass>() += 10;
			// 你也可以在这里写伤害 (GetDamage), 击退 (GetKnockback), 和攻速 (GetAttackSpeed)
		}

		// 此属性决定此伤害类型是否使用标准的暴击计算公式
		// 请注意将其设为 false 会阻止描述中 "暴击率" 一行的显示
		// 并且即使你在 ShowStatTooltipLine 返回 true 也不行, 所以要小心!
		public override bool UseStandardCritCalcs => true;

		public override bool ShowStatTooltipLine(Player player, string lineName) {
			// 此方法允许你隐藏物品描述中特定伤害类型的数据显示
			// 四个可用的名称是 "Damage", "CritChance", "Speed", 和 "Knockback"
			// 这四行描述默认返回 true, 因此会显示出来 (废话), 但如果我们...
			if (lineName == "Speed")
				return false;

			return true;
			// 注 意: 这个钩子将来会被移除, 并以一个更好的替代
		}
	}
}