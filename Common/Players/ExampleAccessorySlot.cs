using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.Players
{
	public class ExampleModAccessorySlot1 : ModAccessorySlot
	{
		// 如果这个类是空的，那就会是一个默认为原版参数的装备栏位
	}

	public class ExampleCustomLocationAndTextureSlot : ModAccessorySlot
	{
		// 我们把这个栏位的位置设置为地图中心，然后我们自定义一个UI
		public override Vector2? CustomLocation => new Vector2(Main.screenWidth / 2, 3 * Main.screenHeight / 4);

		// 如果有染料的话绘制一个染料栏
		public override bool DrawVanitySlot => !DyeItem.IsAir;

		//     我们使用 'custom' （自定义）纹理
		// Background Textures（背景材质）一般来说直接用内置颜色就行了
		public override string VanityBackgroundTexture => "Terraria/Images/Inventory_Back14"; // 黄色
		public override string FunctionalBackgroundTexture => "Terraria/Images/Inventory_Back7"; // 淡蓝色

		// 图标材质。绝大多数材质图片是32x32像素大的。小猪存钱罐是16x24，但是因为材质绘制是居中绘制的，所以游戏中的表现也是正常的
		public override string VanityTexture => "Terraria/Images/Item_" + ItemID.PiggyBank;

		// 绝大多数时候我们让这个槽位隐藏起来，这样大多数时候我们看到的还是原版的UI
		public override bool IsHidden() {
			return IsEmpty; // 只有当槽位中有物品时才显示，物品可以通过右键单击直接塞进去
		}
	}

	public class ExampleModWingSlot : ModAccessorySlot
	{
		public override bool CanAcceptItem(Item checkItem, AccessorySlotType context) {
			if (checkItem.wingSlot > 0) // 如果选择的物品是翅膀就可以塞进这个物品栏
				return true;

			return false; // 其他东西都不行
		}

		// 让我们的槽位具有更高的优先级（右键时翅膀会优先进到这个槽位）
		// 如果不想其他什么槽位放入翅膀，使用 ItemLoader.CanEquipAccessory 来进行修改
		public override bool ModifyDefaultSwapSlot(Item item, int accSlotToSwapTo) {
			if (item.wingSlot > 0) // 如果是翅膀的话进入我们的槽位
				return true;

			return false;
		}

		public override bool IsEnabled() {
			if (Player.armor[0].headSlot >= 0) // 这里我们额外检测玩家头上的头盔
				return true; // 装备了头盔才能使用这个自定义的槽位

			return false; // 否则就用不了
		}

		// 原版槽位中含有物品时我们可以直接取回，这里我们重写一下
		public override bool IsVisibleWhenNotEnabled() {
			return false; // 这里设置为false就可以在槽位为启用时（如上面写的头上没戴头盔）让槽位隐藏起来
		}

		// 放置翅膀时绘制翅膀的贴图纹理。一般32x32大小，居中绘制
		public override string FunctionalTexture => "Terraria/Images/Item_" + ItemID.CreativeWings;

		// 设置当鼠标悬浮在槽位上时的显示内容
		public override void OnMouseHover(AccessorySlotType context) {
			// 当槽位是空的时候，修改鼠标悬浮显示的文字
			switch (context) {
				case AccessorySlotType.FunctionalSlot:
				case AccessorySlotType.VanitySlot:
					Main.hoverItemName = "Wings"; // 告诉玩家这个槽位是放翅膀的
					break;
				case AccessorySlotType.DyeSlot:
					Main.hoverItemName = "Wings Dye"; // 这个是给翅膀染色的
					break;
			}
		}
	}
}
