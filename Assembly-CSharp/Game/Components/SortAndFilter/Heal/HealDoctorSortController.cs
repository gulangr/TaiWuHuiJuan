using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.Heal
{
	// Token: 0x02000E12 RID: 3602
	public class HealDoctorSortController : SortController<HealDoctorSortData>
	{
		// Token: 0x0600AB2B RID: 43819 RVA: 0x004EA71C File Offset: 0x004E891C
		public override Comparison<HealDoctorSortData> GenerateComparer(SortStateData sortData)
		{
			return (HealDoctorSortData x, HealDoctorSortData y) => HealDoctorSortController.CompareData(x, y, sortData);
		}

		// Token: 0x0600AB2C RID: 43820 RVA: 0x004EA748 File Offset: 0x004E8948
		private static int CompareData(HealDoctorSortData x, HealDoctorSortData y, SortStateData sortData)
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
		}

		// Token: 0x0600AB2D RID: 43821 RVA: 0x004EA80C File Offset: 0x004E8A0C
		public override SortUiConfig GenerateConfig()
		{
			List<short> sortIds = new List<short>
			{
				51,
				52
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
