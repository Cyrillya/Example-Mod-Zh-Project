using ExampleMod.Common.Systems;
using ExampleMod.Content;
using System;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod
{
	// 这是一个partial(分部)类，这意味着它的一些部分被拆分到其他文件中。 其他部分参见 ExampleMod.*.cs。
	// 附: partial类微软文档: https://learn.microsoft.com/zh-cn/dotnet/csharp/programming-guide/classes-and-structs/partial-classes-and-methods
	partial class ExampleMod
	{
		// 以下代码允许其他模组“调用” Example Mod 的数据
		// 这允许 Mod 开发人员通过「Mod.Call」方法访问 Example Mod 的数据而无需将其设置为引用 (即在build.txt标记modReference或weakReference)
		// 默认情况下没有Mod，如果你打算做跨Mod内容，或者你打算让别人来对你Mod中的一些内容进行适配，你可以给你的Mod添加ModCall
		public override object Call(params object[] args) {
			// 确保Call传入的参数值是合法的，不会导致不明所以的报错（而是自己报一个明确的报错）
			if (args is null) {
				throw new ArgumentNullException(nameof(args), "Arguments cannot be null!");
			}

			if (args.Length == 0) {
				throw new ArgumentException("Arguments cannot be empty!");
			}

			// 通过模式匹配确保参数是string类型
			// 由于我们只需要判断一个参数，所以只取args数组的第一个变量就行了(下标为0的)
			if (args[0] is string content) {
				// 根据传入string的内容不同，进行不同处理
				switch (content) {
					case "downedMinionBoss":
						// 如果内容是"downedMinionBoss"，即返回downedMinionBoss的值
						return DownedBossSystem.downedMinionBoss;
					case "showMinionCount":
						// 如果内容是"showMinionCount"，即返回本地玩家(Main.LocalPlayer)的showMinionCount的值
						// 本地玩家: 在单人模式就是玩家，在多人模式客户端是当前客户端的玩家角色，服务器上就不应该用它
						return Main.LocalPlayer.GetModPlayer<ExampleInfoDisplayPlayer>().showMinionCount;
					case "setMinionCount":
						// 确保传入的第二个变量是个bool类型，防止报错
						if (args[1] is not bool minionSet) {
							// 不是bool类型就不能进行下一步操作了
							// 抛出报错，告诉Mod开发者需要的参数类型，以及错误传入的参数类型
							throw new Exception($"Expected an argument of type bool when setting minion count, but got type {args[1].GetType().Name} instead.");
						}

						// 确保是bool之后，就可以将值设置为传入参数值了
						// 也可以可以返回一个特定的值，表示调用成功
						// return "Succeed!"; // 比如返回一个string类型的字符串
						Main.LocalPlayer.GetModPlayer<ExampleInfoDisplayPlayer>().showMinionCount = minionSet;

						// 简单地返回「true」也是说明调用成功的一个好方法
						return true;
				}
			}

			// 也可以根据参数的类型不同，导向不同的结果
			// 这里被注释的代码等同于「args[0] is 4」
			if (args[0] is 4/*args[0] is int contentInt && contentInt == 4*/) {
				return ModContent.GetInstance<ExampleBiomeTileCount>().exampleBlockCount;
			}

			// 如果没有一个参数符合需求，返回「false」
			// 这个值可用是你想要作为默认返回值的任意一个值
			return false;
		}
	}
}