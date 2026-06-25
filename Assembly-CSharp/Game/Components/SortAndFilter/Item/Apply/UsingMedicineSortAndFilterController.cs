using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item.Apply
{
	// Token: 0x02000DF7 RID: 3575
	public class UsingMedicineSortAndFilterController : ItemSortAndFilterController
	{
		// Token: 0x0600AAB5 RID: 43701 RVA: 0x004E937C File Offset: 0x004E757C
		public UsingMedicineSortAndFilterController(ISortAndFilterView sortAndFilter) : base(sortAndFilter, LanguageKey.EventEditor_Error_DuplicateGroupKey)
		{
			this.SortController = new ItemSortController();
		}

		// Token: 0x0600AAB6 RID: 43702 RVA: 0x004E9393 File Offset: 0x004E7593
		protected override IEnumerable<FilterLineBase<ITradeableContent>> GenerateFilterLines()
		{
			yield return new UsingMedicineFilterLine();
			yield return new MedicinePoisonTypeDetailedFilterLine();
			yield return new MedicineBuffTypeDetailedFilterLine();
			yield break;
		}
	}
}
