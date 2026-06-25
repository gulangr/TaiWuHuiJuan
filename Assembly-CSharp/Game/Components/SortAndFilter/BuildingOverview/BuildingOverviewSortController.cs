using System;

namespace Game.Components.SortAndFilter.BuildingOverview
{
	// Token: 0x02000E78 RID: 3704
	public class BuildingOverviewSortController : SortController<BuildingOverviewSortData>
	{
		// Token: 0x0600ACB7 RID: 44215 RVA: 0x004EF3CC File Offset: 0x004ED5CC
		public override Comparison<BuildingOverviewSortData> GenerateComparer(SortStateData sortData)
		{
			return (BuildingOverviewSortData x, BuildingOverviewSortData y) => this.CompareData(x, y, sortData);
		}

		// Token: 0x0600ACB8 RID: 44216 RVA: 0x004EF400 File Offset: 0x004ED600
		private int CompareData(BuildingOverviewSortData x, BuildingOverviewSortData y, SortStateData sortData)
		{
			bool flag = x == null && y == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = x == null;
				if (flag2)
				{
					result = 1;
				}
				else
				{
					bool flag3 = y == null;
					if (flag3)
					{
						result = -1;
					}
					else
					{
						bool flag4 = ((sortData != null) ? sortData.ItemStates : null) != null;
						if (flag4)
						{
							foreach (SortItemState itemState in sortData.ItemStates)
							{
								short sortId = itemState.SortId;
								ESortDirection order = itemState.SortDirection;
								int comparisonResult = this.CompareBySortId(x, y, sortId);
								bool flag5 = comparisonResult != 0;
								if (flag5)
								{
									return (order == ESortDirection.Ascending) ? comparisonResult : (-comparisonResult);
								}
							}
						}
						result = x.Building.TemplateId.CompareTo(y.Building.TemplateId);
					}
				}
			}
			return result;
		}

		// Token: 0x0600ACB9 RID: 44217 RVA: 0x004EF4F8 File Offset: 0x004ED6F8
		private int CompareBySortId(BuildingOverviewSortData x, BuildingOverviewSortData y, short sortId)
		{
			return x.Building.TemplateId.CompareTo(y.Building.TemplateId);
		}

		// Token: 0x0600ACBA RID: 44218 RVA: 0x004EF528 File Offset: 0x004ED728
		public override SortUiConfig GenerateConfig()
		{
			return new SortUiConfig
			{
				DefaultSortState = default(SortUiState)
			};
		}
	}
}
