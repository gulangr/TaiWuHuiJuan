using System;
using GameData.Domains.Organization.Display;

namespace GameDataExtensions
{
	// Token: 0x020006D9 RID: 1753
	public static class SettlementTreasuryDisplayDataHelper
	{
		// Token: 0x06005372 RID: 21362 RVA: 0x0026B8C0 File Offset: 0x00269AC0
		public static string GetDisplayLevelStr(this SettlementTreasuryDisplayData displayData)
		{
			int supplyLevelSpriteIndex = displayData.GetSupplyLevelSpriteIndex();
			if (!true)
			{
			}
			string text;
			if (supplyLevelSpriteIndex != 0)
			{
				if (supplyLevelSpriteIndex != 1)
				{
					text = "brightblue";
				}
				else
				{
					text = "pinkyellow";
				}
			}
			else
			{
				text = "brightred";
			}
			if (!true)
			{
			}
			string levelColor = text;
			return displayData.GetDisplaySupplyLevel().ToString().SetColor(levelColor);
		}

		// Token: 0x06005373 RID: 21363 RVA: 0x0026B91C File Offset: 0x00269B1C
		public static string GetDisplayLevelIcon(this SettlementTreasuryDisplayData displayData)
		{
			return string.Format("shop_treasury_scale_{0}", displayData.GetSupplyLevelSpriteIndex());
		}

		// Token: 0x06005374 RID: 21364 RVA: 0x0026B933 File Offset: 0x00269B33
		private static int GetDisplaySupplyLevel(this SettlementTreasuryDisplayData data)
		{
			return data.SupplyLevel + 1;
		}

		// Token: 0x06005375 RID: 21365 RVA: 0x0026B93D File Offset: 0x00269B3D
		private static int GetSupplyLevelSpriteIndex(this SettlementTreasuryDisplayData data)
		{
			return Math.Clamp(data.SupplyLevel, 0, 2);
		}
	}
}
