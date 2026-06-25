using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.Heal
{
	// Token: 0x02000E14 RID: 3604
	public class HealDoctorSortAndFilterController : SortAndFilterController<HealDoctorSortData>
	{
		// Token: 0x0600AB33 RID: 43827 RVA: 0x004EAA51 File Offset: 0x004E8C51
		public HealDoctorSortAndFilterController(ISortAndFilterView sortAndFilter) : base(sortAndFilter, LanguageKey.EventEditor_Error_DuplicateGroupKey)
		{
			this.SortController = new HealDoctorSortController();
		}

		// Token: 0x0600AB34 RID: 43828 RVA: 0x004EAA68 File Offset: 0x004E8C68
		protected override IEnumerable<FilterLineBase<HealDoctorSortData>> GenerateFilterLines()
		{
			yield break;
		}
	}
}
