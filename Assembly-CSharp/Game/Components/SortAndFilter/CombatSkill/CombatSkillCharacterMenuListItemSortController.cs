using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill
{
	// Token: 0x02000E37 RID: 3639
	public class CombatSkillCharacterMenuListItemSortController : SortController<CombatSkillDisplayDataCharacterMenuListItem>
	{
		// Token: 0x0600ABB8 RID: 43960 RVA: 0x004EC4F8 File Offset: 0x004EA6F8
		public override Comparison<CombatSkillDisplayDataCharacterMenuListItem> GenerateComparer(SortStateData sortData)
		{
			return (CombatSkillDisplayDataCharacterMenuListItem x, CombatSkillDisplayDataCharacterMenuListItem y) => CombatSkillCharacterMenuListItemSortController.CompareData(x, y, sortData);
		}

		// Token: 0x0600ABB9 RID: 43961 RVA: 0x004EC524 File Offset: 0x004EA724
		private static int CompareData(CombatSkillDisplayDataCharacterMenuListItem x, CombatSkillDisplayDataCharacterMenuListItem y, SortStateData sortData)
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
					comparisonResult = CombatSkillCharacterMenuListItemSortController.GetBonusCount(x).CompareTo(CombatSkillCharacterMenuListItemSortController.GetBonusCount(y));
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

		// Token: 0x0600ABBA RID: 43962 RVA: 0x004EC654 File Offset: 0x004EA854
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

		// Token: 0x0600ABBB RID: 43963 RVA: 0x004EC6F4 File Offset: 0x004EA8F4
		private static int GetBonusCount(CombatSkillDisplayDataCharacterMenuListItem data)
		{
			List<sbyte> breakBonusGrades = data.BreakBonusGrades;
			return (breakBonusGrades != null) ? breakBonusGrades.Count : 0;
		}
	}
}
