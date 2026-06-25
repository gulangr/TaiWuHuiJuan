using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D7A RID: 3450
	public class BeastCarrierSubTypeMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x1700122D RID: 4653
		// (get) Token: 0x0600A895 RID: 43157 RVA: 0x004E10E4 File Offset: 0x004DF2E4
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x1700122E RID: 4654
		// (get) Token: 0x0600A896 RID: 43158 RVA: 0x004E10E7 File Offset: 0x004DF2E7
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A897 RID: 43159 RVA: 0x004E10EC File Offset: 0x004DF2EC
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600A898 RID: 43160 RVA: 0x004E1108 File Offset: 0x004DF308
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			foreach (short subType in BeastCarrierSubTypeMenu.SubTypeList)
			{
				configs.Add(new FilterDropdownItemConfig(StringKey.CreateKey(string.Format("LK_ItemSubType_{0}", subType))));
			}
			return configs;
		}

		// Token: 0x0600A899 RID: 43161 RVA: 0x004E1184 File Offset: 0x004DF384
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			CarrierItem carrierConfig = Carrier.Instance[data.Key.TemplateId];
			bool flag = carrierConfig == null;
			return !flag && selectedIndices.Any((int index) => carrierConfig.ItemSubType == BeastCarrierSubTypeMenu.SubTypeList[index]);
		}

		// Token: 0x040083EC RID: 33772
		private static readonly List<short> SubTypeList = new List<short>
		{
			402,
			403,
			404
		};
	}
}
