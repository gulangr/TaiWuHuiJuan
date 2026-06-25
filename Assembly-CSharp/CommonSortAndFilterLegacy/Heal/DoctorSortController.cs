using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter.Heal;

namespace CommonSortAndFilterLegacy.Heal
{
	// Token: 0x02000569 RID: 1385
	public class DoctorSortController : CommonSortController<HealDoctorSortData>
	{
		// Token: 0x06004469 RID: 17513 RVA: 0x00209AE8 File Offset: 0x00207CE8
		public override void Sort(List<HealDoctorSortData> dataList, SortStateData sortData, Action actionAfterSort)
		{
			dataList.Sort(delegate(HealDoctorSortData x, HealDoctorSortData y)
			{
				foreach (SortItemState itemState in sortData.ItemStates)
				{
					short sortId = itemState.SortId;
					ESortDirection order = itemState.SortDirection;
					short num = sortId;
					short num2 = num;
					int result;
					if (num2 != 51)
					{
						if (num2 != 52)
						{
							continue;
						}
						result = x.ToxicologyAttainment.CompareTo(y.ToxicologyAttainment);
					}
					else
					{
						result = x.MedicineAttainment.CompareTo(y.MedicineAttainment);
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

		// Token: 0x0600446A RID: 17514 RVA: 0x00209B24 File Offset: 0x00207D24
		public override CommonSortUiConfig GenerateConfig()
		{
			List<short> sortIds = new List<short>
			{
				51,
				52
			};
			return new CommonSortUiConfig
			{
				SortIds = sortIds,
				SortNameIndexList = new List<int>
				{
					0,
					0
				},
				DefaultSortState = null,
				DefaultSortIds = sortIds
			};
		}
	}
}
