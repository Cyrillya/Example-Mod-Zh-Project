using ExampleMod.Content.Tiles;
using System.IO;
using Terraria;
using Terraria.GameContent.Tile_Entities;
using Terraria.ID;
using Terraria.ModLoader.Default;

namespace ExampleMod.Content.TileEntities
{
	/// <summary>
	/// 此 TileEntity 与 <seealso cref="ExamplePylonTileAdvanced"/> 一同使用,
	/// 以提供比原版普通晶塔 TileEntity（又称 <seealso cref="TETeleportationPylon"/>）更具灵活性的自定义晶塔<br/>
	/// <seealso cref="TEModdedPylon"/> 类内置于 tML 本体中
	/// <para>
	/// 这里是一个仅在<b>完全随机的时间间隔</b>内处于活动状态的晶塔
	/// </para>
	/// </summary>
	public class AdvancedPylonTileEntity : TEModdedPylon
	{
		/// <summary>
		/// 这是这个 TileEntity 的关键，晶塔功能仅在它为 <see langword="true"/> 时才可用
		/// </summary>
		public bool isActive;

		public override void OnNetPlace() {
			// 这个方法只会在服务器端调用
			// 需要在此处通过SendData方法发包，以同步TileEntity
			NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
		}

		public override void NetSend(BinaryWriter writer) {
			// 我们要确保数据在客户端和服务器之间正确同步
			// 每当发送 TileEntitySharing 包时都会调用 NetSend，因此游戏会自动为我们处理这件事，前提是我们会在需要时发送数据
			// 这里与下面的 NetReceive 相联系，以将 isActive 变量同步
			writer.Write(isActive);
		}

		public override void NetReceive(BinaryReader reader) {
			// 这里写自定义收包，请确保你发送了什么就要接收什么
			// 与上面的 NetSend 相联系，以将 isActive 变量同步
			isActive = reader.ReadBoolean();
		}

		public override void Update() {
			// 更新仅在服务器端或单人游戏中调用，因此我们的随机性将在该参考系中
			// 晶塔每帧都有 1/180 的几率切换其开关状态 (开启到关闭，反之亦然)
			if (!Main.rand.NextBool(180)) {
				return;
			}

			// 在这里切换 isActive 的值，如果在服务器上别忘了同步数据
			isActive = !isActive;
			if (Main.netMode == NetmodeID.Server) {
				NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
			}
		}
	}
}
