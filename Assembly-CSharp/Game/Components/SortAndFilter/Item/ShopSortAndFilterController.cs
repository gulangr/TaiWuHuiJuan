using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DE9 RID: 3561
	public class ShopSortAndFilterController : ItemSortAndFilterController
	{
		// Token: 0x0600AA84 RID: 43652 RVA: 0x004E87CC File Offset: 0x004E69CC
		public ShopSortAndFilterController(ISortAndFilterView sortAndFilter, LanguageKey language = LanguageKey.LK_Exchange_Filter_Title) : base(sortAndFilter, language)
		{
		}

		// Token: 0x0600AA85 RID: 43653 RVA: 0x004E87D8 File Offset: 0x004E69D8
		protected override IEnumerable<FilterLineBase<ITradeableContent>> GenerateFilterLines()
		{
			foreach (FilterLineBase<ITradeableContent> item in base.GenerateFilterLines())
			{
				yield return item;
				item = null;
			}
			IEnumerator<FilterLineBase<ITradeableContent>> enumerator = null;
			yield return new PriceDetailedFilterLine();
			yield break;
			yield break;
		}
	}
}
