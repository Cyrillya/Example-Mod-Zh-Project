using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.UI.BigProgressBar;

namespace ExampleMod.Content.BossBars
{
	// 十分基本的, 在菜单的"界面"中可选的自定义boss血条样式
	// 如果你想要自定义自定义NPC的boss血条选择, 在 PreventUpdate 返回 false 并在 Update 钩子里写你自己的码
	public class ExampleBossBarStyle : ModBossBarStyle
	{
		public override bool PreventDraw => true; // 阻止默认的绘制

		public override void Draw(SpriteBatch spriteBatch, IBigProgressBar currentBar, BigProgressBarInfo info) {
			if (currentBar == null) {
				return;
				// 只在要画原版血条的时候画 (允许更新, 因为 PreventUpdate 没有被重写为返回 false)
			}

			if (currentBar is CommonBossBigProgressBar) {
				// 如果是个没有特殊效果的常规血条, 我们就画自己的东西. 可惜"要显示的生命"并不是我们能访问的变量
				// 不过这只是一个十分基本的实现, 只追踪一个NPC, 我们可以用"info"

				NPC npc = Main.npc[info.npcIndexToAimAt];
				float lifePercent = Utils.Clamp(npc.life / (float)npc.lifeMax, 0f, 1f);

				// 原版未使用的方法, 仅仅画几个简单的方框来表示boss血条(颜色与位置固定, 没有图标)
				BigProgressBarHelper.DrawBareBonesBar(spriteBatch, lifePercent);
			}
			else {
				// 如果选择的是别的特殊血条, 那就让它画吧, 我们又不能访问它的特殊属性

				currentBar.Draw(ref info, spriteBatch);
			}
		}
	}
}
