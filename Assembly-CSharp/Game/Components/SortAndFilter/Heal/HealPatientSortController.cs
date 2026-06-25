using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.Heal
{
	// Token: 0x02000E19 RID: 3609
	public class HealPatientSortController : SortController<HealPatientSortData>
	{
		// Token: 0x0600AB3B RID: 43835 RVA: 0x004EAAD8 File Offset: 0x004E8CD8
		private static int CompareUnknownHealthOnTop(HealPatientSortData x, HealPatientSortData y)
		{
			bool flag = x.IsHealthUnknownOrNone == y.IsHealthUnknownOrNone;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				result = (x.IsHealthUnknownOrNone ? -1 : 1);
			}
			return result;
		}

		// Token: 0x0600AB3C RID: 43836 RVA: 0x004EAB0C File Offset: 0x004E8D0C
		public override Comparison<HealPatientSortData> GenerateComparer(SortStateData sortData)
		{
			return (HealPatientSortData x, HealPatientSortData y) => HealPatientSortController.CompareData(x, y, sortData);
		}

		// Token: 0x0600AB3D RID: 43837 RVA: 0x004EAB38 File Offset: 0x004E8D38
		private static int CompareData(HealPatientSortData x, HealPatientSortData y, SortStateData sortData)
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
					result = HealPatientSortController.CompareUnknownHealthOnTop(x, y);
					bool flag = result != 0;
					if (flag)
					{
						return result;
					}
					result = x.HealthPercent.CompareTo(y.HealthPercent);
				}
				bool flag2 = result != 0;
				if (flag2)
				{
					return (order == ESortDirection.Ascending) ? result : (-result);
				}
			}
			return 0;
		}

		// Token: 0x0600AB3E RID: 43838 RVA: 0x004EAC58 File Offset: 0x004E8E58
		public override SortUiConfig GenerateConfig()
		{
			List<short> sortIds = new List<short>
			{
				53,
				54,
				55,
				10
			};
			List<int> sortNameIndexList = new List<int>
			{
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
