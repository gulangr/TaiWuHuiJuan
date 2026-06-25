using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.Information
{
	// Token: 0x02000E11 RID: 3601
	public class InformationSortController : SortController<InformationSortAndFilterData>
	{
		// Token: 0x0600AB27 RID: 43815 RVA: 0x004EA59C File Offset: 0x004E879C
		public override Comparison<InformationSortAndFilterData> GenerateComparer(SortStateData sortData)
		{
			return (InformationSortAndFilterData x, InformationSortAndFilterData y) => InformationSortController.CompareData(x, y, sortData);
		}

		// Token: 0x0600AB28 RID: 43816 RVA: 0x004EA5C8 File Offset: 0x004E87C8
		private static int CompareData(InformationSortAndFilterData x, InformationSortAndFilterData y, SortStateData sortData)
		{
			foreach (SortItemState itemState in sortData.ItemStates)
			{
				short sortId = itemState.SortId;
				ESortDirection order = itemState.SortDirection;
				short num = sortId;
				short num2 = num;
				int comparisonResult;
				if (num2 != 154)
				{
					if (num2 != 155)
					{
						continue;
					}
					comparisonResult = x.RemainCount.CompareTo(y.RemainCount);
				}
				else
				{
					comparisonResult = x.Level.CompareTo(y.Level);
				}
				bool flag = comparisonResult != 0;
				if (flag)
				{
					return (order == ESortDirection.Ascending) ? comparisonResult : (-comparisonResult);
				}
			}
			return 0;
		}

		// Token: 0x0600AB29 RID: 43817 RVA: 0x004EA69C File Offset: 0x004E889C
		public override SortUiConfig GenerateConfig()
		{
			List<short> sortIds = new List<short>
			{
				154,
				155
			};
			List<int> sortNameIndexList = new List<int>
			{
				0,
				0
			};
			return new SortUiConfig
			{
				SortIds = sortIds,
				SortNameIndexList = sortNameIndexList,
				DefaultSortState = default(SortUiState),
				DefaultSortIds = sortIds
			};
		}
	}
}
