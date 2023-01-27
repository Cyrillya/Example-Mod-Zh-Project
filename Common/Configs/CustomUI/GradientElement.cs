using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using Terraria.GameContent;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using ExampleMod.Common.Configs.CustomDataTypes;

// 请注意：以下内容为自定义设置界面UI元素
// 因为TML官方目前仍在对自定义设置界面UI的相关内容进行开发，如果你使用了自定义设置界面UI元素，你的mod很可能在最近的几次更新后无法兼容新代码
// 如果你决定要使用自定义设置界面UI元素的相关内容，你最好积极更新mod以防TML更新时导致的代码失效

// 这个文件基于 Gradient 数据类型自定义 ConfigElement
// 实现在 ModConfig 类中自定义UI的功能
namespace ExampleMod.Common.Configs.CustomUI
{
	// 这个自定义UI方法使用了内置的UI元素，并允许你自定义绘制他们
	class GradientElement : ConfigElement
	{
		public override void OnBind() {
			base.OnBind();

			object subitem = MemberInfo.GetValue(Item);

			if (subitem == null) {
				subitem = Activator.CreateInstance(MemberInfo.Type);
				JsonConvert.PopulateObject("{}", subitem, ConfigManager.serializerSettings);
				MemberInfo.SetValue(Item, subitem);
			}

			// item 是UI对象的实例，memberinfo 是 item 中关于此字段的信息

			int height = 30;
			int order = 0;

			foreach (PropertyFieldWrapper variable in ConfigManager.GetFieldsAndProperties(subitem)) {
				var wrapped = ConfigManager.WrapIt(this, ref height, variable, subitem, order++);

				if (List != null) {
					wrapped.Item1.Left.Pixels -= 20;
					wrapped.Item1.Width.Pixels += 20;
				}
			}
		}

		public override void Draw(SpriteBatch spriteBatch) {
			base.Draw(spriteBatch);
			var hitbox = GetInnerDimensions().ToRectangle();
			var g = MemberInfo.GetValue(Item) as Gradient;
			if (g != null) {
				int left = (hitbox.Left + hitbox.Right) / 2;
				int right = hitbox.Right;
				int steps = right - left;
				for (int i = 0; i < steps; i += 1) {
					float percent = (float)i / steps;
					spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left + i, hitbox.Y, 1, 30), Color.Lerp(g.start, g.end, percent));
				}

				//Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.X + hitbox.Width / 2, hitbox.Y, hitbox.Width / 4, 30), g.start);
				//Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.X + 3 * hitbox.Width / 4, hitbox.Y, hitbox.Width / 4, 30), g.end);
			}
		}
	}
}
