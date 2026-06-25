using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.BuildingOverview
{
	// Token: 0x02000E77 RID: 3703
	public class BuildingOverviewSortAndFilterController : SortAndFilterController<BuildingOverviewSortData>
	{
		// Token: 0x0600ACB5 RID: 44213 RVA: 0x004EF39F File Offset: 0x004ED59F
		public BuildingOverviewSortAndFilterController(ISortAndFilterView sortAndFilter) : base(sortAndFilter, LanguageKey.LK_SettlementPrison)
		{
			this.SortController = new BuildingOverviewSortController();
		}

		// Token: 0x0600ACB6 RID: 44214 RVA: 0x004EF3BA File Offset: 0x004ED5BA
		protected override IEnumerable<FilterLineBase<BuildingOverviewSortData>> GenerateFilterLines()
		{
			yield return new BuildingOverviewMainFilterLine();
			yield break;
		}
	}
}
