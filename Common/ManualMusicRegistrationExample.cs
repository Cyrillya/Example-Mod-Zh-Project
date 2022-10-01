using Terraria.ModLoader;

namespace ExampleMod.Common
{
	// 一个用于手动加载音乐的ILoadable类例子
	// 几乎不需要手动加载，因为在默认情况下，TML会自动加载'Music'文件夹（包括子目录）中的每一个.wav、.ogg和.mp3声音文件
	public sealed class ManualMusicRegistrationExample : ILoadable
	{
		public void Load(Mod mod) {
			// 手动注册音乐时，你需要提供一个你的模组的Mod实例
			// 既然你提供了一个Mod类实例，路径的开头就不需要是你的模组的类名了
			// 可用的音乐格式为: .mp3, .ogg, .wav
			// 注意，在路径中不应该包括音乐文件的扩展名

			// MusicLoader.AddMusic(Mod, "Assets/Music/MysteriousMystery");

			// 在“Content/Items/Placeable/ExampleMusicBox.cs”中可以找到一个注册音乐盒的例子
		}

		public void Unload() { }
	}
}
