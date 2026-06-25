using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using Game.Views.Building.BuildingManage;

namespace Game.Components.SortAndFilter.Chicken
{
	// Token: 0x02000E4F RID: 3663
	public class ChickenSortController : SortController<ChickenData>
	{
		// Token: 0x0600AC25 RID: 44069 RVA: 0x004ED968 File Offset: 0x004EBB68
		public override Comparison<ChickenData> GenerateComparer(SortStateData sortData)
		{
			return (ChickenData x, ChickenData y) => ChickenSortController.CompareData(x, y, sortData);
		}

		// Token: 0x0600AC26 RID: 44070 RVA: 0x004ED994 File Offset: 0x004EBB94
		private static int CompareData(ChickenData x, ChickenData y, SortStateData sortData)
		{
			Config.ChickenItem xTemplate = Chicken.Instance[x.TemplateId];
			Config.ChickenItem yTemplate = Chicken.Instance[y.TemplateId];
			foreach (SortItemState itemState in sortData.ItemStates)
			{
				short sortId = itemState.SortId;
				ESortDirection order = itemState.SortDirection;
				short num = sortId;
				short num2 = num;
				int comparisonResult;
				if (num2 != 0)
				{
					if (num2 != 12)
					{
						if (num2 != 150)
						{
							continue;
						}
						comparisonResult = xTemplate.PersonalityValue.CompareTo(yTemplate.PersonalityValue);
					}
					else
					{
						comparisonResult = x.Happiness.CompareTo(y.Happiness);
					}
				}
				else
				{
					string xName = x.Name;
					string yName = y.Name;
					comparisonResult = Utils_Sorting.CompareByCurrentLangEncoding(xName, yName);
				}
				bool flag = comparisonResult != 0;
				if (flag)
				{
					return (order == ESortDirection.Ascending) ? comparisonResult : (-comparisonResult);
				}
			}
			return 0;
		}

		// Token: 0x0600AC27 RID: 44071 RVA: 0x004EDAB0 File Offset: 0x004EBCB0
		public override SortUiConfig GenerateConfig()
		{
			return new SortUiConfig
			{
				SortIds = new List<short>(),
				SortNameIndexList = new List<int>(),
				DefaultSortState = default(SortUiState),
				DefaultSortIds = new List<short>()
			};
		}
	}
}
