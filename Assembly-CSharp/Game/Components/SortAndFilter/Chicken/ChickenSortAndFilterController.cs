using System;
using System.Collections.Generic;
using Game.Views.Building.BuildingManage;

namespace Game.Components.SortAndFilter.Chicken
{
	// Token: 0x02000E4E RID: 3662
	public class ChickenSortAndFilterController : SortAndFilterController<ChickenData>
	{
		// Token: 0x0600AC23 RID: 44067 RVA: 0x004ED941 File Offset: 0x004EBB41
		public ChickenSortAndFilterController(ISortAndFilterView sortAndFilter) : base(sortAndFilter, LanguageKey.EventEditor_Error_DuplicateGroupKey)
		{
			this.SortController = new ChickenSortController();
		}

		// Token: 0x0600AC24 RID: 44068 RVA: 0x004ED958 File Offset: 0x004EBB58
		protected override IEnumerable<FilterLineBase<ChickenData>> GenerateFilterLines()
		{
			yield return new ChickenFilterLine();
			yield break;
		}
	}
}
