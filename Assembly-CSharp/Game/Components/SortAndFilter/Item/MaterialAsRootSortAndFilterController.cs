using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D9E RID: 3486
	public class MaterialAsRootSortAndFilterController : ItemSortAndFilterController
	{
		// Token: 0x0600A935 RID: 43317 RVA: 0x004E4D9D File Offset: 0x004E2F9D
		public MaterialAsRootSortAndFilterController(ISortAndFilterView sortAndFilter) : base(sortAndFilter, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Main_5)
		{
			this.SortController = new ItemSortController();
		}

		// Token: 0x0600A936 RID: 43318 RVA: 0x004E4DB8 File Offset: 0x004E2FB8
		protected override IEnumerable<FilterLineBase<ITradeableContent>> GenerateFilterLines()
		{
			yield return new MaterialRootFilterLine();
			yield break;
		}
	}
}
