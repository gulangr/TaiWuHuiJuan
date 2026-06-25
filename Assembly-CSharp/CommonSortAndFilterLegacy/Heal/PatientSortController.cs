using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter.Heal;

namespace CommonSortAndFilterLegacy.Heal
{
	// Token: 0x0200056B RID: 1387
	public class PatientSortController : CommonSortController<HealPatientSortData>
	{
		// Token: 0x0600446F RID: 17519 RVA: 0x00209BC8 File Offset: 0x00207DC8
		public override void Sort(List<HealPatientSortData> dataList, SortStateData sortData, Action actionAfterSort)
		{
			dataList.Sort(delegate(HealPatientSortData x, HealPatientSortData y)
			{
				foreach (SortItemState itemState in sortData.ItemStates)
				{
					short sortId = itemState.SortId;
					ESortDirection order = itemState.SortDirection;
					short num = sortId;
					short num2 = num;
					int result;
					if (num2 != 10)
					{
						switch (num2)
						{
						case 53:
							result = x.TotalInjuries.CompareTo(y.TotalInjuries);
							break;
						case 54:
							result = x.TotalPoisons.CompareTo(y.TotalPoisons);
							break;
						case 55:
							result = x.QiDisorder.CompareTo(y.QiDisorder);
							break;
						default:
							continue;
						}
					}
					else
					{
						result = x.HealthPercent.CompareTo(y.HealthPercent);
					}
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

		// Token: 0x06004470 RID: 17520 RVA: 0x00209C04 File Offset: 0x00207E04
		public override CommonSortUiConfig GenerateConfig()
		{
			List<short> sortIds = new List<short>
			{
				53,
				54,
				55,
				10
			};
			return new CommonSortUiConfig
			{
				SortIds = sortIds,
				SortNameIndexList = new List<int>
				{
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
