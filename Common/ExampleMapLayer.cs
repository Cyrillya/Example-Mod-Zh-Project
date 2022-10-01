using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.UI;

namespace ExampleMod.Common
{
	// ModMapLayers 是用来在地图上绘制图标或其他东西的。即类似原版中显示在小地图上的晶塔、出生点和床图标。这个例子在地牢上添加了一个图标
	public class ExampleMapLayer : ModMapLayer
	{
		// 绘制发生在 Draw 方法中。学习正确使用其的一个良好资源是原版源码
		public override void Draw(ref MapOverlayDrawContext context, ref string text) {
			// 分别声明当光标悬停在图标上和没悬停在图标上时的图标缩放大小
			const float scaleIfNotSelected = 1f;
			const float scaleIfSelected = scaleIfNotSelected * 2f;

			// 直接使用骷髅王的小地图图标贴图作为绘制贴图。注意，并不是所有的贴图都是默认加载的
			// 对于一些贴图（如物品、NPC、射弹等）你可能需要提前使用类似 `Main.instance.LoadItem(ItemID.BoneKey);` 的代码以确保贴图已加载
			var dungeonTexture = TextureAssets.NpcHeadBoss[19].Value;

			// 这里使用的 MapOverlayDrawContext.Draw 方法可以快捷处理许多绘制图标的小细节，如果可以的话应该优先选择这种绘制方式。
			// 它将处理贴图缩放、对齐、剪裁、取景和考虑地图UI缩放。要手动处理这些的话工作量会很大
			// 这里的 position 参数是以 Vector2 类型表示的物块坐标。不要将物块坐标×16来转换成世界坐标。
			// 而对于世界坐标应对其÷16
			// MapOverlayDrawContext.Draw 的返回值是一个指示光标是否悬停于图标上的bool类型值
			if (context.Draw(dungeonTexture, new Vector2(Main.dungeonX, Main.dungeonY), Color.White, new SpriteFrame(1, 1, 0, 0), scaleIfNotSelected, scaleIfSelected, Alignment.Center).IsMouseOver) {
				// 当鼠标光标悬停于图标上时，将在光标右下角显示自动翻译的“地牢”字样
				text = Language.GetTextValue("Bestiary_Biomes.TheDungeon");
			}
		}
	}
}