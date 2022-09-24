using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ID;
using ExampleMod.Backgrounds;

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

		public override string DisplayName => "Example ModMenu";

		public override void OnSelected() {
			SoundEngine.PlaySound(SoundID.Thunder); // 选择了这个ModMenu之后播放一个打雷音效
		}

		public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor) {
			drawColor = Main.DiscoColor; // 修改Logo的颜色
			return true;
		}
	}
}
