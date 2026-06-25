using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.Heal
{
	// Token: 0x02000E16 RID: 3606
	public class HealPatientSortAndFilterController : SortAndFilterController<HealPatientSortData>
	{
		// Token: 0x0600AB37 RID: 43831 RVA: 0x004EAA9F File Offset: 0x004E8C9F
		public HealPatientSortAndFilterController(ISortAndFilterView sortAndFilter) : base(sortAndFilter, LanguageKey.EventEditor_Error_DuplicateGroupKey)
		{
			this.SortController = new HealPatientSortController();
		}

		// Token: 0x0600AB38 RID: 43832 RVA: 0x004EAAB6 File Offset: 0x004E8CB6
		protected override IEnumerable<FilterLineBase<HealPatientSortData>> GenerateFilterLines()
		{
			yield break;
		}
	}
}
