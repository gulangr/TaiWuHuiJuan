using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D76 RID: 3446
	public class CarrierSubTypeMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x17001225 RID: 4645
		// (get) Token: 0x0600A87D RID: 43133 RVA: 0x004E0EB3 File Offset: 0x004DF0B3
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001226 RID: 4646
		// (get) Token: 0x0600A87E RID: 43134 RVA: 0x004E0EB6 File Offset: 0x004DF0B6
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A87F RID: 43135 RVA: 0x004E0EBC File Offset: 0x004DF0BC
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600A880 RID: 43136 RVA: 0x004E0ED8 File Offset: 0x004DF0D8
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_Mousetip_Carrier_0"))
			};
		}

		// Token: 0x0600A881 RID: 43137 RVA: 0x004E0F08 File Offset: 0x004DF108
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			CarrierItem carrierConfig = Carrier.Instance[data.Key.TemplateId];
			bool flag = carrierConfig == null;
			return !flag && selectedIndices.Any((int index) => carrierConfig.ItemSubType == CarrierSubTypeMenu.SubTypeList[index]);
		}

		// Token: 0x040083EA RID: 33770
		private static readonly List<short> SubTypeList = new List<short>
		{
			400
		};
	}
}
