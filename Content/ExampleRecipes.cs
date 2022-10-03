using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ExampleMod.Content
{
	// 这个类包含一些合成配方的例子
	public class ExampleRecipes : ModSystem
	{
		// 一个用于存储自定义材料组的静态RecipeGroup实例，以便后续使用
		public static RecipeGroup ExampleRecipeGroup;

		public override void Unload() {
			// 静态的RecipeGroup实例要记得在Unload时卸载，避免爆内存
			ExampleRecipeGroup = null;
		}

		// 附: 中文Wiki材料组页面: https://terraria.wiki.gg/zh/wiki/%E5%8F%AF%E9%80%89%E6%8B%A9%E5%88%B6%E4%BD%9C%E6%9D%90%E6%96%99 
		public override void AddRecipeGroups() {
			// 创建一个材料组并存在ExampleRecipeGroup里
			// 材料组: 原版的“任何铁锭”就是一个材料组，而只要铁锭和铅锭总和 ≥3 就可以合成空桶
			// Language.GetTextValue("LegacyMisc.37") 是一个自动翻译文本，比如在游戏语言为英文时显示“Any”，而中文会显示“任何”
			// Lang.GetItemNameValue(ModContent.ItemType<Items.ExampleItem>()) 为获取物品对应的显示名称
			ExampleRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ModContent.ItemType<Items.ExampleItem>())}",
				ModContent.ItemType<Items.ExampleItem>(), ModContent.ItemType<Items.ExampleDataItem>());

			// 按照"Mod内部名:物品内部名"的格式作为注册名称可以避免和其他模组冲突
			RecipeGroup.RegisterGroup("ExampleMod:ExampleItem", ExampleRecipeGroup);

			// 下面这行代码是给原版已有的一个材料组添加物品
			// RecipeGroup.recipeGroups[RecipeGroupID.Snails].ValidItems.Add(ModContent.ItemType<Items.ExampleCritter>());

			// 原版有铁锭材料组，但是没有银锭材料组
			// 而tModLoader会自动将名称相同材料组合并
			// 因此，当你想要注册一个基于原版物品的材料组时，如果你认为其他Mod可能会有同样的材料组想法时，你可以直接用原版物品的
			// 内部名 即 nameof(ItemID.XX) 注册。那么多个Mod就可以将不同的物品添加到同一个组中，以实现功能的相互适配。
			// 这就是一个添加“银锭”材料组的例子
			// 注意不要像上面添加ExampleRecipeGroup一样把他存到一个全局变量里，因为它的实例可能不会被使用
			// 即: 与已有的同名材料组合并后，这里创建的实例就不会再被使用了
			// 那么怎么使用这个材料组呢? 有这两种方式:
			// 1. 在调用 Recipe.AddRecipeGroup 时用相同的 nameof(ItemID.XX) 获取，比如 AddRecipeGroup(nameof(ItemID.SilverBar))
			// 2. 将 RecipeGroup.RegisterGroup 返回的整数存起来 (这是材料组ID)，调用 Recipe.AddRecipeGroup 时用这个整数获取
			RecipeGroup SilverBarRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.SilverBar)}",
			ItemID.SilverBar, ItemID.TungstenBar, ModContent.ItemType<Items.Placeable.ExampleBar>());
			RecipeGroup.RegisterGroup(nameof(ItemID.SilverBar), SilverBarRecipeGroup);
		}

		public override void AddRecipes() {
			///////////////////////////////////////////////////
			// 这是一个用1个石块合成999个ExampleItem的例子 //
			///////////////////////////////////////////////////

			Recipe recipe = Recipe.Create(ModContent.ItemType<Items.ExampleItem>(), 999);
			// 下面这行代码表示这个配方需要1个石块
			recipe.AddIngredient(ItemID.StoneBlock);
			// 设置完属性之后，调用 Register() 来把配方添加进游戏里
			recipe.Register();

			//////////////////////////////////////////////////////////////////////////
			// 下面的例子包含了Recipe类所有方法的使用例，并且使用了高级的“链式”代码风格 //
			/////////////////////////////////////////////////////////////////////////
			
			// 由于Recipe类的所有方法都返回他自己的Recipe实例 (除了Register())，添加配方时可以链式添加，也就是说你可以对于
			// 方法的返回值调用其他的方法，而不需要存储在一个本地变量中
			// 使用链式代码时，需要注意只有最后一行要输入分号 (;)

			var resultItem = ModContent.GetInstance<Items.ExampleItem>();

			// 创建一个新的配方
			resultItem.CreateRecipe()
				// 添加一个原版物品合成材料
				// 可以在这里查找物品ID: https://github.com/tModLoader/tModLoader/wiki/Vanilla-Item-IDs
				// 注: 中文建议使用Wiki上的表: https://terraria.wiki.gg/zh/wiki/%E7%89%A9%E5%93%81_ID
				// 配合使用 Ctrl+F 可快速查找物品对应的内部名，并且，为了代码可读性，建议使用 ItemID.内部名 的ID表示法而不是数字ID
				// 如果你要添加不同的材料，可以反复调用 recipe.AddIngredient()
				.AddIngredient(ItemID.StoneBlock)
				// 可选的第二个传入参数 (也就是这里的10) 表明这个材料需要多少个。这里表示需要10个橡实
				// 如果不填第二个传入参数，那么材料将默认只需要一个
				.AddIngredient(ItemID.Acorn, 10)
				// 也可以将合成物品本身作为材料
				.AddIngredient(resultItem)
				// 将模组物品作为合成材料。不要类比上面的原版物品就直接用 ItemID.模组物品内部名 的方式，那是只能给原版物品用的
				.AddIngredient<Items.Weapons.ExampleSword>()
				// 添加模组物品的另一种方法，引号 ("") 内是模组物品的内部名。这种方法建议仅用作将其他模组的物品作为合成材料
				// 因为这种方法更慢，在加载模组时耗时更长
				.AddIngredient(Mod, "ExampleSword")

				// 材料组让这个合成材料允许用同类的物品替代。就像原版中只要 铅锭+铁锭≥3 就可以合成铁桶
				// 这里的 RecipeGroupID.Wood 就代表了原版所有的木头
				// 关于材料组的详解以及查询原版其他材料组，请看这里: https://github.com/tModLoader/tModLoader/wiki/Intermediate-Recipes#using-existing-recipegroups
				.AddRecipeGroup(RecipeGroupID.Wood)
				// 跟调用 AddIngredient 一样，有一个可选的物品堆叠参数
				.AddRecipeGroup(RecipeGroupID.IronBar, 2)
				// 这是一个模组添加的材料组，查看上面的 AddRecipeGroups() 来注册一个材料组
				.AddRecipeGroup(ExampleRecipeGroup, 2)
				// 添加材料组的另一种方法。建议仅用作将其他模组的材料组作为合成材料，因为这种方法更慢，在加载模组时耗时更长
				.AddRecipeGroup("Wood")
				.AddRecipeGroup("ExampleMod:ExampleItem", 2)

				// 添加制作站需求。需要给出制作站物块的物块ID，可以看这: https://github.com/tModLoader/tModLoader/wiki/Vanilla-Tile-IDs
				// 中文Wiki: https://terraria.wiki.gg/zh/wiki/%E5%9B%BE%E6%A0%BC_ID
				// 另: 为了代码可读性，建议使用 TileID.物块英文名 的形式而不是数字ID
				.AddTile(TileID.WorkBenches)
				// 添加模组制作站需求。若你想要添加不同的制作站，可以调用多次 recipe.AddTile()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				// 添加模组制作站的另一种方法。建议仅用作添加其他模组的制作站，因为这种方法更慢，在加载模组时耗时更长
				.AddTile(Mod, "ExampleWorkbench")

				// 添加已有的特定条件，以下三行使得合成只能在夜晚的沙漠环境中靠近水的地方完成
				.AddCondition(Recipe.Condition.InDesert)
				.AddCondition(Recipe.Condition.NearWater)
				.AddCondition(Recipe.Condition.TimeNight)
				// 添加一个自定义条件，下面是一个仅当玩家血量低于50%时才能合成的例子
				// 第一个参数是 NetworkText 实例，也就是自动翻译文本。这里用到的键(Key)值在源码中的“Localization/*.hjson”文件中被定义
				// 第二个参数是一个通过lambda表达式创建的委托，关于委托和lambda表达式可以自行上网搜索学习
				.AddCondition(NetworkText.FromKey("RecipeConditions.LowHealth"), r => Main.LocalPlayer.statLife < Main.LocalPlayer.statLifeMax / 2)

				// 最后调用 Register() 来注册配方，即真正地将配方加到游戏里。注意最后的分号
				.Register();

			////////////////////////////////////////////////////////////////////
			// 下面是一个复制配方的例子，以及对这个配方进行修改，使其与原配方不同 //
			///////////////////////////////////////////////////////////////////
			
			// 如果你要复制一份已经被添加的配方，可以使用 Mod.CloneRecipe 方法
			// 复制出来的配方的属性和被复制方的属性完全一致，但他所属的模组为调用复制方法的模组
			// 如果你只是想给你Mod的某个物品创建多个配方变种，使用一个帮助方法会是更好的选择
			// 另外，如果使用材料组或是自定义条件就能解决，请不要使用复制合成配方

			// 创建一个你想要复制的配方
			Recipe baseRecipe = Recipe.Create(ModContent.ItemType<Items.ExampleItem>(), 10);
			baseRecipe.AddIngredient(ItemID.Wood, 10)
				.AddIngredient(ItemID.CopperCoin)
				.AddCondition(Recipe.Condition.InBeach)
				.AddCondition(Recipe.Condition.TimeDay)
				.Register();
				
			// 从上面的 baseRecipe 配方复制过来
			Recipe clonedRecipe = baseRecipe.Clone()
				// 给这个配方添加的条件不会作用到被复制配方上去
				.AddIngredient(ItemID.SilverCoin)
				.AddTile(TileID.Anvils);
				
			// 可以删除某个特定的材料需求或条件
			clonedRecipe.RemoveIngredient(ItemID.CopperCoin);
			clonedRecipe.RemoveCondition(Recipe.Condition.InBeach);
			
			// 设置完属性之后，调用 Register() 来把配方添加进游戏里
			clonedRecipe.Register();
		}

		public override void PostAddRecipes() {
			for (int i = 0; i < Recipe.numRecipes; i++) {
				Recipe recipe = Main.recipe[i];
				
				// 使所有以木块为材料的配方的木块需求变成原来的两倍
				if (recipe.TryGetIngredient(ItemID.Wood, out Item ingredient)) {
					ingredient.stack *= 2;
				}
			}
		}
	}
}
