using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill.Simplified
{
	// Token: 0x02000E43 RID: 3651
	public class SimplifiedCombatSkillSortController : SortController<CombatSkillDisplayDataForList>
	{
		// Token: 0x0600ABE1 RID: 44001 RVA: 0x004ECAA4 File Offset: 0x004EACA4
		public override Comparison<CombatSkillDisplayDataForList> GenerateComparer(SortStateData sortData)
		{
			return (CombatSkillDisplayDataForList x, CombatSkillDisplayDataForList y) => SimplifiedCombatSkillSortController.CompareData(x, y, sortData);
		}

		// Token: 0x0600ABE2 RID: 44002 RVA: 0x004ECAD0 File Offset: 0x004EACD0
		private static int CompareData(CombatSkillDisplayDataForList x, CombatSkillDisplayDataForList y, SortStateData sortData)
		{
			CombatSkillItem xTemplate = CombatSkill.Instance[x.TemplateId];
			CombatSkillItem yTemplate = CombatSkill.Instance[y.TemplateId];
			foreach (SortItemState itemState in sortData.ItemStates)
			{
				short sortId = itemState.SortId;
				ESortDirection order = itemState.SortDirection;
				short num = sortId;
				short num2 = num;
				int comparisonResult;
				switch (num2)
				{
				case 1:
					comparisonResult = xTemplate.Grade.CompareTo(yTemplate.Grade);
					break;
				case 2:
					comparisonResult = x.Power.CompareTo(y.Power);
					break;
				case 3:
					comparisonResult = SimplifiedCombatSkillSortController.GetBonusCount(x).CompareTo(SimplifiedCombatSkillSortController.GetBonusCount(y));
					break;
				case 4:
					comparisonResult = SimplifiedCombatSkillSortController.GetReadCount(x).CompareTo(SimplifiedCombatSkillSortController.GetReadCount(y));
					break;
				default:
					if (num2 != 216)
					{
						continue;
					}
					comparisonResult = x.CombatSkillProficiency.CompareTo(y.CombatSkillProficiency);
					break;
				}
				bool flag = comparisonResult != 0;
				if (flag)
				{
					return (order == ESortDirection.Ascending) ? comparisonResult : (-comparisonResult);
				}
			}
			return 0;
		}

		// Token: 0x0600ABE3 RID: 44003 RVA: 0x004ECC20 File Offset: 0x004EAE20
		public override SortUiConfig GenerateConfig()
		{
			List<short> sortIds = new List<short>
			{
				1,
				2,
				3,
				4,
				216
			};
			List<int> sortNameIndexList = new List<int>
			{
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

		// Token: 0x0600ABE4 RID: 44004 RVA: 0x004ECCC0 File Offset: 0x004EAEC0
		private static int GetBonusCount(CombatSkillDisplayDataForList data)
		{
			List<sbyte> breakBonusGrades = data.BreakBonusGrades;
			return (breakBonusGrades != null) ? breakBonusGrades.Count : 0;
		}

		// Token: 0x0600ABE5 RID: 44005 RVA: 0x004ECCE4 File Offset: 0x004EAEE4
		private static int GetReadCount(CombatSkillDisplayDataForList data)
		{
			return CombatSkillStateHelper.GetReadPagesCount(data.ReadingState);
		}
	}
}
