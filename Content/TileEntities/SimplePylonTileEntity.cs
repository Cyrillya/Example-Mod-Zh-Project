using Terraria.ModLoader.Default;

namespace ExampleMod.Content.TileEntities
{
	/// <summary>
	/// 这是一个继承 <seealso cref="TEModdedPylon"/> 的空子类，它与原版晶塔 TileEntity 完全相同<br/>
	/// 这种继承可以让模组晶塔 TileEntity 正确地设置其“Mod”属性，以用于 I/O (输入/输出, 详情可百度) (这里指的是让tML那边可以正确操作这个晶塔)<br/>
	/// 这个类由 <see langword="sealed"/>(封闭) 修饰，因为它和父类完全相同，没必要继承<br/>
	/// Sealed参考: https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/keywords/sealed
	/// </summary>
	public sealed class SimplePylonTileEntity : TEModdedPylon { }
}
