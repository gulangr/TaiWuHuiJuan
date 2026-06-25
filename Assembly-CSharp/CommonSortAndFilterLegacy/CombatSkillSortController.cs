using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.CombatSkill;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x0200041D RID: 1053
	public class CombatSkillSortController : CommonSortController<CombatSkillDisplayData>
	{
		// Token: 0x06003EA0 RID: 16032 RVA: 0x001F5E54 File Offset: 0x001F4054
		public override void Sort(List<CombatSkillDisplayData> dataList, SortStateData sortData, Action actionAfterSort)
		{
			dataList.Sort((CombatSkillDisplayData x, CombatSkillDisplayData y) => CombatSkillSortController.CompareData(x, y, sortData));
			if (actionAfterSort != null)
			{
				actionAfterSort();
			}
		}

		// Token: 0x06003EA1 RID: 16033 RVA: 0x001F5E90 File Offset: 0x001F4090
		private static int CompareData(CombatSkillDisplayData x, CombatSkillDisplayData y, SortStateData sortData)
		{
			CombatSkillItem xTemplate = CombatSkill.Instance[x.TemplateId];
			CombatSkillItem yTemplate = CombatSkill.Instance[y.TemplateId];
			foreach (SortItemState itemState in sortData.ItemStates)
			{
				short sortId = itemState.SortId;
				ESortDirection order = itemState.SortDirection;
				int comparisonResult;
				switch (sortId)
				{
				case 1:
					comparisonResult = xTemplate.Grade.CompareTo(yTemplate.Grade);
					break;
				case 2:
					comparisonResult = x.Power.CompareTo(y.Power);
					break;
				case 3:
					comparisonResult = CombatSkillSortController.GetBonusCount(x).CompareTo(CombatSkillSortController.GetBonusCount(y));
					break;
				case 4:
					comparisonResult = CombatSkillSortController.GetReadCount(x).CompareTo(CombatSkillSortController.GetReadCount(y));
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
			return 0;
		}

		// Token: 0x06003EA2 RID: 16034 RVA: 0x001F5FBC File Offset: 0x001F41BC
		public override CommonSortUiConfig GenerateConfig()
		{
			List<short> sortIds = new List<short>
			{
				1,
				2,
				3,
				4
			};
			List<int> sortNameIndexList = new List<int>
			{
				0,
				0,
				0,
				0
			};
			return new CommonSortUiConfig
			{
				SortIds = sortIds,
				SortNameIndexList = sortNameIndexList,
				DefaultSortState = null,
				DefaultSortIds = sortIds
			};
		}

		// Token: 0x06003EA3 RID: 16035 RVA: 0x001F6044 File Offset: 0x001F4244
		private static int GetBonusCount(CombatSkillDisplayData data)
		{
			List<sbyte> breakBonusGrades = data.BreakBonusGrades;
			return (breakBonusGrades != null) ? breakBonusGrades.Count : 0;
		}

		// Token: 0x06003EA4 RID: 16036 RVA: 0x001F6068 File Offset: 0x001F4268
		private static int GetReadCount(CombatSkillDisplayData data)
		{
			return CombatSkillStateHelper.GetReadPagesCount(data.ReadingState);
		}
	}
}
