using System;

namespace Game.Components.SortAndFilter.Item.Apply
{
	// Token: 0x02000DEB RID: 3563
	public class CraftsmanProductSortAndFilterController : ItemSortAndFilterController
	{
		// Token: 0x0600AA89 RID: 43657 RVA: 0x004E880A File Offset: 0x004E6A0A
		public CraftsmanProductSortAndFilterController(ISortAndFilterView sortAndFilter) : base(sortAndFilter, LanguageKey.EventEditor_Error_DuplicateGroupKey)
		{
			this.SortController = new CraftsmanProductSortController();
		}
	}
}
