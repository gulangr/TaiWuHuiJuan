using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D8F RID: 3471
	public class FoodSortAndFilterController : ItemSortAndFilterController
	{
		// Token: 0x0600A901 RID: 43265 RVA: 0x004E25A9 File Offset: 0x004E07A9
		public FoodSortAndFilterController(ISortAndFilterView sortAndFilter) : base(sortAndFilter, LanguageKey.EventEditor_Error_DuplicateGroupKey)
		{
			this.SortController = new ItemSortController();
		}

		// Token: 0x0600A902 RID: 43266 RVA: 0x004E25C0 File Offset: 0x004E07C0
		protected override IEnumerable<FilterLineBase<ITradeableContent>> GenerateFilterLines()
		{
			yield return new FoodFilterLine();
			yield return new FoodVegetarianTypeDetailedFilterLine();
			yield return new FoodMeatTypeDetailedFilterLine();
			yield return new FoodTeaTypeDetailedFilterLine();
			yield return new FoodAlcoholTypeDetailedFilterLine();
			yield break;
		}
	}
}
