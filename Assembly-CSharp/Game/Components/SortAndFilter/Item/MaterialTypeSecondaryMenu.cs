using System;
using System.Collections.Generic;
using System.Linq;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DE5 RID: 3557
	public class MaterialTypeSecondaryMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x170012B5 RID: 4789
		// (get) Token: 0x0600AA66 RID: 43622 RVA: 0x004E80DF File Offset: 0x004E62DF
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170012B6 RID: 4790
		// (get) Token: 0x0600AA67 RID: 43623 RVA: 0x004E80E2 File Offset: 0x004E62E2
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AA68 RID: 43624 RVA: 0x004E80E8 File Offset: 0x004E62E8
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600AA69 RID: 43625 RVA: 0x004E8104 File Offset: 0x004E6304
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return (from itemSubType in this._itemSubTypes
			select new FilterDropdownItemConfig(StringKey.CreateKey(string.Format("LK_ItemSubType_{0}", itemSubType)))).ToList<FilterDropdownItemConfig>();
		}

		// Token: 0x0600AA6A RID: 43626 RVA: 0x004E8148 File Offset: 0x004E6348
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			bool flag = data.Key.ItemType != 5;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				foreach (int index in selectedIndices)
				{
					short selectedItemSubType = this._itemSubTypes[index];
					bool flag2 = ItemFilterCommon.IsItemMatchItemSubType(data, selectedItemSubType);
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x040084C5 RID: 33989
		private readonly List<short> _itemSubTypes = new List<short>
		{
			504,
			501,
			503,
			502,
			505,
			506,
			500
		};
	}
}
