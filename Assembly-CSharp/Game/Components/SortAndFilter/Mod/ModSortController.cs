using System;
using System.Collections.Generic;
using FrameWork;

namespace Game.Components.SortAndFilter.Mod
{
	// Token: 0x02000D18 RID: 3352
	public class ModSortController : SortController<ModSortAndFilterData>
	{
		// Token: 0x0600A738 RID: 42808 RVA: 0x004DD3C8 File Offset: 0x004DB5C8
		public override Comparison<ModSortAndFilterData> GenerateComparer(SortStateData sortData)
		{
			return (ModSortAndFilterData x, ModSortAndFilterData y) => ModSortController.CompareData(x, y, sortData);
		}

		// Token: 0x0600A739 RID: 42809 RVA: 0x004DD3F4 File Offset: 0x004DB5F4
		private static int CompareData(ModSortAndFilterData x, ModSortAndFilterData y, SortStateData sortData)
		{
			bool flag = sortData.ItemStates.Count == 0;
			int result;
			if (flag)
			{
				result = Utils_Sorting.CompareByCurrentLangEncoding(x.Name, y.Name);
			}
			else
			{
				foreach (SortItemState itemState in sortData.ItemStates)
				{
					short sortId = itemState.SortId;
					ESortDirection order = itemState.SortDirection;
					int comparisonResult;
					switch (sortId)
					{
					case 170:
						comparisonResult = x.IsEnabled(x.ModId).CompareTo(y.IsEnabled(y.ModId));
						break;
					case 171:
						comparisonResult = x.GetOrder(x.ModId).CompareTo(y.GetOrder(y.ModId));
						break;
					case 172:
						comparisonResult = Utils_Sorting.CompareByCurrentLangEncoding(x.Name, y.Name);
						break;
					case 173:
						comparisonResult = x.Rate.CompareTo(y.Rate);
						break;
					case 174:
						comparisonResult = x.UploadTime.CompareTo(y.UploadTime);
						break;
					case 175:
						comparisonResult = x.UpdateTime.CompareTo(y.UpdateTime);
						break;
					case 176:
						comparisonResult = x.Size.CompareTo(y.Size);
						break;
					case 177:
						comparisonResult = x.IsExpired.CompareTo(y.IsExpired);
						break;
					default:
						continue;
					}
					bool flag2 = comparisonResult != 0;
					if (flag2)
					{
						return (order == ESortDirection.Ascending) ? comparisonResult : (-comparisonResult);
					}
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x0600A73A RID: 42810 RVA: 0x004DD5D0 File Offset: 0x004DB7D0
		public override SortUiConfig GenerateConfig()
		{
			List<short> sortIds = new List<short>
			{
				170,
				171,
				172,
				177,
				173,
				174,
				175,
				176
			};
			List<int> sortNameIndexList = new List<int>
			{
				0,
				0,
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
