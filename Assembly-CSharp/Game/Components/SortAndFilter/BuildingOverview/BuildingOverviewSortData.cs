using System;
using Config;
using Game.Views.Buildings.Migrate;

namespace Game.Components.SortAndFilter.BuildingOverview
{
	// Token: 0x02000E76 RID: 3702
	public class BuildingOverviewSortData
	{
		// Token: 0x1700136D RID: 4973
		// (get) Token: 0x0600ACB0 RID: 44208 RVA: 0x004EF374 File Offset: 0x004ED574
		// (set) Token: 0x0600ACB1 RID: 44209 RVA: 0x004EF37C File Offset: 0x004ED57C
		public BuildingBlockItem Building { get; set; }

		// Token: 0x1700136E RID: 4974
		// (get) Token: 0x0600ACB2 RID: 44210 RVA: 0x004EF385 File Offset: 0x004ED585
		// (set) Token: 0x0600ACB3 RID: 44211 RVA: 0x004EF38D File Offset: 0x004ED58D
		public BuildingOverviewBuildingPrefab.EBuildingStatus Status { get; set; }
	}
}
