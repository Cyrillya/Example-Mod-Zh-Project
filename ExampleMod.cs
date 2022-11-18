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
			// Unload() 方法可用于卸载(Unload)/处置(Dispose)/清除(Clear)特殊对象、取消事件委托或撤消你的模组的某些操作
			// 当你的Mod的某些对象有可能保留在原版程序集中时，请务必编写卸载代码
			// 发生这种情况的最常见原因来自使用事件委托，而不是 On.* 和 IL.* 代码注入命名空间 (On与IL命名空间委托在tML会被自动卸载，一般无需过多处理)
			// 如果你进行了委托，请确保其被卸载
			// 附: 委托相关微软文档: https://learn.microsoft.com/zh-cn/dotnet/csharp/programming-guide/delegates/

			// 注意：在编写卸载代码时一定要使用“防御性编程”。也就是说，应该始终假设正在卸载的内容可能还没有被初始化 (可能没赋值就在运行Unload代码了)
			// 注意：很少需要清空静态字段的值，因为 TML 旨在于 Mod 重新加载时完全处理 Mod 程序集。
		}
	}
}
