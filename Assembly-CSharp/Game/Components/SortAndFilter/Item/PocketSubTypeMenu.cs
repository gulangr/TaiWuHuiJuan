using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D7C RID: 3452
	public class PocketSubTypeMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x17001231 RID: 4657
		// (get) Token: 0x0600A8A1 RID: 43169 RVA: 0x004E1260 File Offset: 0x004DF460
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001232 RID: 4658
		// (get) Token: 0x0600A8A2 RID: 43170 RVA: 0x004E1263 File Offset: 0x004DF463
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A8A3 RID: 43171 RVA: 0x004E1268 File Offset: 0x004DF468
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600A8A4 RID: 43172 RVA: 0x004E1284 File Offset: 0x004DF484
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			foreach (short subType in PocketSubTypeMenu.SubTypeList)
			{
				configs.Add(new FilterDropdownItemConfig(StringKey.CreateKey(string.Format("LK_ItemSubType_{0}", subType))));
			}
			return configs;
		}

		// Token: 0x0600A8A5 RID: 43173 RVA: 0x004E1300 File Offset: 0x004DF500
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			AccessoryItem accessoryConfig = Accessory.Instance[data.Key.TemplateId];
			bool flag = accessoryConfig == null;
			return !flag && selectedIndices.Any((int index) => accessoryConfig.ItemSubType == PocketSubTypeMenu.SubTypeList[index]);
		}

		// Token: 0x040083ED RID: 33773
		private static readonly List<short> SubTypeList = new List<short>
		{
			201
		};
	}
}
