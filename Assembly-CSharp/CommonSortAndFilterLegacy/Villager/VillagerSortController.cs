using System;
using System.Collections.Generic;
using FrameWork;

namespace CommonSortAndFilterLegacy.Villager
{
	// Token: 0x0200046D RID: 1133
	public class VillagerSortController<T> : CommonSortController<T> where T : IVillagerSortAndFilterData
	{
		// Token: 0x060040AF RID: 16559 RVA: 0x001FFEB8 File Offset: 0x001FE0B8
		public override void Sort(List<T> dataList, SortStateData sortData, Action actionAfterSort)
		{
			dataList.Sort(delegate(T x, T y)
			{
				foreach (SortItemState itemState in sortData.ItemStates)
				{
					short sortId = itemState.SortId;
					ESortDirection order = itemState.SortDirection;
					int result;
					switch (sortId)
					{
					case 0:
					{
						string xName = x.Name;
						string yName = y.Name;
						result = Utils_Sorting.CompareByCurrentLangEncoding(xName, yName);
						goto IL_1C4;
					}
					case 1:
						result = x.Grade.CompareTo(y.Grade);
						goto IL_1C4;
					case 8:
						result = x.Age.CompareTo(y.Age);
						goto IL_1C4;
					case 9:
						result = x.Charm.CompareTo(y.Charm);
						goto IL_1C4;
					case 10:
						result = x.Health.CompareTo(y.Health);
						goto IL_1C4;
					case 11:
						result = x.FavorabilityToTaiwu.CompareTo(y.FavorabilityToTaiwu);
						goto IL_1C4;
					case 12:
						result = x.Happiness.CompareTo(y.Happiness);
						goto IL_1C4;
					case 13:
						result = x.LeftPotentialCount.CompareTo(y.LeftPotentialCount);
						goto IL_1C4;
					}
					continue;
					IL_1C4:
					bool flag = result != 0;
					if (flag)
					{
						return (order == ESortDirection.Ascending) ? result : (-result);
					}
				}
				return 0;
			});
			if (actionAfterSort != null)
			{
				actionAfterSort();
			}
		}

		// Token: 0x060040B0 RID: 16560 RVA: 0x001FFEF4 File Offset: 0x001FE0F4
		public override CommonSortUiConfig GenerateConfig()
		{
			List<short> sortIds = new List<short>
			{
				0,
				1,
				8,
				9,
				10,
				11,
				12,
				13
			};
			return new CommonSortUiConfig
			{
				SortIds = sortIds,
				SortNameIndexList = new List<int>
				{
					0,
					1,
					0,
					0,
					0,
					0,
					0,
					0
				},
				DefaultSortState = null,
				DefaultSortIds = sortIds
			};
		}
	}
}
