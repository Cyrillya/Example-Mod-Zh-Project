using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace ExampleMod
{
	// 注意 partial 关键字，这是一个分部类型，也就是说其内容被拆分到了好几个其他的文件中
	// 你可以通过其他的 ExampleMod.*.cs 文件来了解其全部内容
	// 附: 微软关于分部类型的文档: https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/keywords/partial-type
	public partial class ExampleMod : Mod
	{
		public const string AssetPath = $"{nameof(ExampleMod)}/Assets/";

		// 这个变量用来存储一个自定义的货币类型（类比一下原版的护卫奖章）
		// 如果你在用 Visual Studio 的话，可以右键变量名选择“查找所有引用”来看看他都被用在了哪里
		public static int ExampleCustomCurrencyId;

		public override void Load() {
			// 注册一个新的货币类型
			ExampleCustomCurrencyId = CustomCurrencyManager.RegisterCurrency(new Content.Currencies.ExampleCustomCurrency(ModContent.ItemType<Content.Items.ExampleItem>(), 999L, "Mods.ExampleMod.Currencies.ExampleCustomCurrency"));
		}

		public override void Unload() {
			// The Unload() methods can be used for unloading/disposing/clearing special objects, unsubscribing from events, or for undoing some of your mod's actions.
			// Be sure to always write unloading code when there is a chance of some of your mod's objects being kept present inside the vanilla assembly.
			// The most common reason for that to happen comes from using events, NOT counting On.* and IL.* code-injection namespaces.
			// If you subscribe to an event - be sure to eventually unsubscribe from it.

			// NOTE: When writing unload code - be sure use 'defensive programming'. Or, in other words, you should always assume that everything in the mod you're unloading might've not even been initialized yet.
			// NOTE: There is rarely a need to null-out values of static fields, since TML aims to completely dispose mod assemblies in-between mod reloads.
		}
	}
}
