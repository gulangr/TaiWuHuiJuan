using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.BuildingOverview
{
	// Token: 0x02000E7A RID: 3706
	public class BuildingOverviewMainFilterLine : DetailedFilterLineLogic<BuildingOverviewSortData>
	{
		// Token: 0x1700136F RID: 4975
		// (get) Token: 0x0600ACBC RID: 44220 RVA: 0x004EF559 File Offset: 0x004ED759
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600ACBD RID: 44221 RVA: 0x004EF55C File Offset: 0x004ED75C
		protected override IEnumerable<DetailedFilterMenuLogic<BuildingOverviewSortData>> GenerateMenus()
		{
			yield return new BuildingOverviewTypeMenu();
			yield return new BuildingOverviewStatusMenu();
			yield break;
		}

		// Token: 0x17001370 RID: 4976
		// (get) Token: 0x0600ACBE RID: 44222 RVA: 0x004EF56C File Offset: 0x004ED76C
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17001371 RID: 4977
		// (get) Token: 0x0600ACBF RID: 44223 RVA: 0x004EF56F File Offset: 0x004ED76F
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600ACC0 RID: 44224 RVA: 0x004EF574 File Offset: 0x004ED774
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
