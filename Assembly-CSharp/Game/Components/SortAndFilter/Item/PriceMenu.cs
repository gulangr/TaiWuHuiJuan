using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DD9 RID: 3545
	public class PriceMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x1700129F RID: 4767
		// (get) Token: 0x0600AA2A RID: 43562 RVA: 0x004E7664 File Offset: 0x004E5864
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170012A0 RID: 4768
		// (get) Token: 0x0600AA2B RID: 43563 RVA: 0x004E7667 File Offset: 0x004E5867
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AA2C RID: 43564 RVA: 0x004E766C File Offset: 0x004E586C
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_Shop_Filter_Price;
		}

		// Token: 0x0600AA2D RID: 43565 RVA: 0x004E7688 File Offset: 0x004E5888
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(LanguageKey.LK_Shop_Filter_Price_Up),
				new FilterDropdownItemConfig(LanguageKey.LK_Shop_Filter_Price_Origin),
				new FilterDropdownItemConfig(LanguageKey.LK_Shop_Filter_Price_Down)
			};
		}

		// Token: 0x0600AA2E RID: 43566 RVA: 0x004E76E4 File Offset: 0x004E58E4
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			using (IEnumerator<int> enumerator = selectedIndices.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current)
					{
					case 0:
					{
						ItemDisplayData itemDisplayData = data as ItemDisplayData;
						bool flag = ((itemDisplayData != null) ? itemDisplayData.PricePercent : 0) > 0;
						if (flag)
						{
							return true;
						}
						break;
					}
					case 1:
					{
						ItemDisplayData itemDisplayData2 = data as ItemDisplayData;
						bool flag2 = ((itemDisplayData2 != null) ? itemDisplayData2.PricePercent : 0) == 0;
						if (flag2)
						{
							return true;
						}
						break;
					}
					case 2:
					{
						ItemDisplayData itemDisplayData3 = data as ItemDisplayData;
						bool flag3 = ((itemDisplayData3 != null) ? itemDisplayData3.PricePercent : 0) < 0;
						if (flag3)
						{
							return true;
						}
						break;
					}
					}
				}
			}
			return false;
		}
	}
}
