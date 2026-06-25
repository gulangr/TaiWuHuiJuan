using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.Secret
{
	// Token: 0x02000CE1 RID: 3297
	public class SecretSortController : SortController<SecretSortAndFilterData>
	{
		// Token: 0x0600A64C RID: 42572 RVA: 0x004D68C4 File Offset: 0x004D4AC4
		public override Comparison<SecretSortAndFilterData> GenerateComparer(SortStateData sortData)
		{
			return (SecretSortAndFilterData x, SecretSortAndFilterData y) => SecretSortController.CompareData(x, y, sortData);
		}

		// Token: 0x0600A64D RID: 42573 RVA: 0x004D68F0 File Offset: 0x004D4AF0
		private static int CompareData(SecretSortAndFilterData x, SecretSortAndFilterData y, SortStateData sortData)
		{
			int comparisonResult = 0;
			foreach (SortItemState itemState in sortData.ItemStates)
			{
				short sortId = itemState.SortId;
				ESortDirection order = itemState.SortDirection;
				switch (sortId)
				{
				case 159:
					comparisonResult = x.Data.OccurenceDate.CompareTo(y.Data.OccurenceDate);
					break;
				case 160:
					comparisonResult = x.LevelScore.CompareTo(y.LevelScore);
					break;
				case 161:
					comparisonResult = x.LifeTime.CompareTo(y.LifeTime);
					break;
				case 162:
					comparisonResult = x.Data.HolderCount.CompareTo(y.Data.HolderCount);
					break;
				case 163:
					comparisonResult = x.CanUseCount.CompareTo(y.CanUseCount);
					break;
				case 164:
					comparisonResult = x.Data.DisseminationRate.CompareTo(y.Data.DisseminationRate);
					break;
				default:
					continue;
				}
				bool flag = comparisonResult != 0;
				if (flag)
				{
					return (order == ESortDirection.Ascending) ? comparisonResult : (-comparisonResult);
				}
			}
			return comparisonResult;
		}

		// Token: 0x0600A64E RID: 42574 RVA: 0x004D6A58 File Offset: 0x004D4C58
		public override SortUiConfig GenerateConfig()
		{
			List<short> sortIds = new List<short>
			{
				159,
				160,
				161,
				162,
				163,
				164
			};
			List<int> sortNameIndexList = new List<int>
			{
				0,
				0,
				0,
				0,
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
