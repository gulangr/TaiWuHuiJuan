using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter.Item;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000C92 RID: 3218
	public class EmptyItemController : ItemSortAndFilterController
	{
		// Token: 0x0600A3EF RID: 41967 RVA: 0x004CA203 File Offset: 0x004C8403
		public EmptyItemController(ISortAndFilterView sortAndFilter, LanguageKey panelTitleKey = LanguageKey.EventEditor_Error_DuplicateGroupKey) : base(sortAndFilter, panelTitleKey)
		{
			this.SortController = new EmptySortController();
		}

		// Token: 0x0600A3F0 RID: 41968 RVA: 0x004CA21A File Offset: 0x004C841A
		protected override IEnumerable<FilterLineBase<ITradeableContent>> GenerateFilterLines()
		{
			return null;
		}
	}
}
