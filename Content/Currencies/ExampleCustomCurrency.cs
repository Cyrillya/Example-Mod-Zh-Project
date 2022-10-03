using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.Localization;

namespace ExampleMod.Content.Currencies
{
	public class ExampleCustomCurrency : CustomCurrencySingleCoin
	{
		// 参见Mod主类里的引用
		public ExampleCustomCurrency(int coinItemID, long currencyCap, string CurrencyTextKey) : base(coinItemID, currencyCap) {
			this.CurrencyTextKey = CurrencyTextKey;
			CurrencyTextColor = Color.BlueViolet;
		}
	}
}