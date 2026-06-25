using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DCE RID: 3534
	public class MiscMiscTypeMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x1700128F RID: 4751
		// (get) Token: 0x0600A9DE RID: 43486 RVA: 0x004E6A7C File Offset: 0x004E4C7C
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001290 RID: 4752
		// (get) Token: 0x0600A9DF RID: 43487 RVA: 0x004E6A7F File Offset: 0x004E4C7F
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A9E0 RID: 43488 RVA: 0x004E6A84 File Offset: 0x004E4C84
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600A9E1 RID: 43489 RVA: 0x004E6AA0 File Offset: 0x004E4CA0
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return (from subType in this._itemSubType
			select new FilterDropdownItemConfig(StringKey.CreateKey(string.Format("LK_ItemSubType_{0}", subType)))).ToList<FilterDropdownItemConfig>();
		}

		// Token: 0x0600A9E2 RID: 43490 RVA: 0x004E6AE4 File Offset: 0x004E4CE4
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			MiscItem miscItem = Misc.Instance[data.Key.TemplateId];
			short subType = miscItem.ItemSubType;
			return selectedIndices.Any((int index) => this._itemSubType[index] == subType);
		}

		// Token: 0x040084BA RID: 33978
		private readonly List<short> _itemSubType = new List<short>
		{
			1204,
			1201
		};
	}
}
