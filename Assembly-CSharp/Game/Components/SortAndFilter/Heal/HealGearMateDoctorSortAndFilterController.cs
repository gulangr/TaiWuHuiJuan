using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.Heal
{
	// Token: 0x02000E15 RID: 3605
	public class HealGearMateDoctorSortAndFilterController : SortAndFilterController<HealDoctorSortData>
	{
		// Token: 0x0600AB35 RID: 43829 RVA: 0x004EAA78 File Offset: 0x004E8C78
		public HealGearMateDoctorSortAndFilterController(ISortAndFilterView sortAndFilter) : base(sortAndFilter, LanguageKey.EventEditor_Error_DuplicateGroupKey)
		{
			this.SortController = new HealGearMateDoctorSortController();
		}

		// Token: 0x0600AB36 RID: 43830 RVA: 0x004EAA8F File Offset: 0x004E8C8F
		protected override IEnumerable<FilterLineBase<HealDoctorSortData>> GenerateFilterLines()
		{
			yield break;
		}
	}
}
