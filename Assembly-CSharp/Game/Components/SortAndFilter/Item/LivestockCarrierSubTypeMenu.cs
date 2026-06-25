using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D78 RID: 3448
	public class LivestockCarrierSubTypeMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x17001229 RID: 4649
		// (get) Token: 0x0600A889 RID: 43145 RVA: 0x004E0FCB File Offset: 0x004DF1CB
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x1700122A RID: 4650
		// (get) Token: 0x0600A88A RID: 43146 RVA: 0x004E0FCE File Offset: 0x004DF1CE
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A88B RID: 43147 RVA: 0x004E0FD4 File Offset: 0x004DF1D4
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600A88C RID: 43148 RVA: 0x004E0FF0 File Offset: 0x004DF1F0
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_Mousetip_Carrier_1"))
			};
		}

		// Token: 0x0600A88D RID: 43149 RVA: 0x004E1020 File Offset: 0x004DF220
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			CarrierItem carrierConfig = Carrier.Instance[data.Key.TemplateId];
			bool flag = carrierConfig == null;
			return !flag && selectedIndices.Any((int index) => carrierConfig.ItemSubType == LivestockCarrierSubTypeMenu.SubTypeList[index]);
		}

		// Token: 0x040083EB RID: 33771
		private static readonly List<short> SubTypeList = new List<short>
		{
			401
		};
	}
}
