using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ID;
using ExampleMod.Backgrounds;
using Terraria.Localization;

namespace ExampleMod.Content
{
	public class ExampleModMenu : ModMenu
	{
		// 创建一个指示贴图资源文件夹路径的常量，这样在要用到的时候就不需要重复写一长串了
		private const string menuAssetPath = "ExampleMod/Assets/Textures/Menu";

		public override Asset<Texture2D> Logo => base.Logo;

		public override Asset<Texture2D> SunTexture => ModContent.Request<Texture2D>($"{menuAssetPath}/ExampleSun");

		public override Asset<Texture2D> MoonTexture => ModContent.Request<Texture2D>($"{menuAssetPath}/ExampliumMoon");

		public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/MysteriousMystery");

		public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<ExampleSurfaceBackgroundStyle>();

		// 作为一个品德优良的Mod作者，应当尽量减少直接使用字符串作为显示文本，而是使用 Language.GetTextValue 获取翻译文本并搭配 hjson 文件使用
		// 其显示文本详见 Localization/ 文件夹下的 en-US.hjson 与 zh-Hans.hjson
		// 被注释掉的是原版 Example Mod 的源码
		// public override string DisplayName => "Example ModMenu";
		public override string DisplayName => Language.GetTextValue("Mods.ExampleMod.ModMenuName"); // 提倡以这种形式外显文本

		public override void OnSelected() {
			SoundEngine.PlaySound(SoundID.Thunder); // 选择了这个ModMenu之后播放一个打雷音效
		}

		public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor) {
			drawColor = Main.DiscoColor; // 修改Logo的颜色
			return true;
		}
	}
}
