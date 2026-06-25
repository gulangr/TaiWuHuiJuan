using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DEA RID: 3562
	public class SortOnlyItemController : ItemSortAndFilterController
	{
		// Token: 0x0600AA87 RID: 43655 RVA: 0x004E87F0 File Offset: 0x004E69F0
		public SortOnlyItemController(ISortAndFilterView sortAndFilter, LanguageKey panelTitleKey = LanguageKey.EventEditor_Error_DuplicateGroupKey) : base(sortAndFilter, panelTitleKey)
		{
			this.SortController = new ItemSortController();
		}

		// Token: 0x0600AA88 RID: 43656 RVA: 0x004E8807 File Offset: 0x004E6A07
		protected override IEnumerable<FilterLineBase<ITradeableContent>> GenerateFilterLines()
		{
			return null;
		}
	}
}
