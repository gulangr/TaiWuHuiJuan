using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.Heal
{
	// Token: 0x02000E13 RID: 3603
	public class HealGearMateDoctorSortController : SortController<HealDoctorSortData>
	{
		// Token: 0x0600AB2F RID: 43823 RVA: 0x004EA884 File Offset: 0x004E8A84
		public override Comparison<HealDoctorSortData> GenerateComparer(SortStateData sortData)
		{
			return (HealDoctorSortData x, HealDoctorSortData y) => HealGearMateDoctorSortController.CompareData(x, y, sortData);
		}

		// Token: 0x0600AB30 RID: 43824 RVA: 0x004EA8B0 File Offset: 0x004E8AB0
		private static int CompareData(HealDoctorSortData x, HealDoctorSortData y, SortStateData sortData)
		{
			foreach (SortItemState itemState in sortData.ItemStates)
			{
				short sortId = itemState.SortId;
				ESortDirection order = itemState.SortDirection;
				int result;
				switch (sortId)
				{
				case 72:
					result = x.ForgingAttainment.CompareTo(y.ForgingAttainment);
					goto IL_AB;
				case 73:
					result = x.WoodworkingAttainment.CompareTo(y.WoodworkingAttainment);
					goto IL_AB;
				case 76:
					result = x.WeavingAttainment.CompareTo(y.WeavingAttainment);
					goto IL_AB;
				case 77:
					result = x.JadeAttainment.CompareTo(y.JadeAttainment);
					goto IL_AB;
				}
				continue;
				IL_AB:
				bool flag = result != 0;
				if (flag)
				{
					return (order == ESortDirection.Ascending) ? result : (-result);
				}
			}
			return 0;
		}

		// Token: 0x0600AB31 RID: 43825 RVA: 0x004EA9B8 File Offset: 0x004E8BB8
		public override SortUiConfig GenerateConfig()
		{
			List<short> sortIds = new List<short>
			{
				72,
				73,
				77,
				76
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
