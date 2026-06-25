using System;
using System.Collections.Generic;
using Config;
using Game.Views.Looping;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill
{
	// Token: 0x02000E1E RID: 3614
	public class CombatSkillSortController : SortController<IFilterableCombatSkill>
	{
		// Token: 0x0600AB42 RID: 43842 RVA: 0x004EAD2B File Offset: 0x004E8F2B
		public CombatSkillSortController(EFilterType filterType)
		{
			this._filterType = filterType;
		}

		// Token: 0x0600AB43 RID: 43843 RVA: 0x004EAD3C File Offset: 0x004E8F3C
		public override Comparison<IFilterableCombatSkill> GenerateComparer(SortStateData sortData)
		{
			return (IFilterableCombatSkill x, IFilterableCombatSkill y) => CombatSkillSortController.CompareData(x, y, sortData);
		}

		// Token: 0x0600AB44 RID: 43844 RVA: 0x004EAD68 File Offset: 0x004E8F68
		private static int CompareData(IFilterableCombatSkill x, IFilterableCombatSkill y, SortStateData sortData)
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
					comparisonResult = CombatSkillSortController.GetBonusCount(x).CompareTo(CombatSkillSortController.GetBonusCount(y));
					break;
				case 4:
					comparisonResult = CombatSkillSortController.GetReadCount(x).CompareTo(CombatSkillSortController.GetReadCount(y));
					break;
				default:
					switch (num2)
					{
					case 144:
						comparisonResult = ((int)(x.MaxObtainableNeili - x.ObtainedNeili)).CompareTo((int)(y.MaxObtainableNeili - y.ObtainedNeili));
						break;
					case 145:
						comparisonResult = ((x.FiveElementDestTypeWhileLooping >= 0) ? ((y.FiveElementDestTypeWhileLooping >= 0) ? 0 : -1) : ((y.FiveElementDestTypeWhileLooping >= 0) ? 1 : 0));
						break;
					case 146:
						comparisonResult = CombatSkillSortController.GetReferenceSkillBonusPercentForSorting(x).CompareTo(CombatSkillSortController.GetReferenceSkillBonusPercentForSorting(y));
						break;
					case 147:
						comparisonResult = CombatSkillSortController.GetReferenceSkillBonusPercentForSorting(x).CompareTo(CombatSkillSortController.GetReferenceSkillBonusPercentForSorting(y));
						break;
					case 148:
					{
						CombatSkillItem combatSkillItem = CombatSkill.Instance[x.TemplateId];
						sbyte xEventRate = (combatSkillItem != null) ? combatSkillItem.QiArtStrategyGenerateProbability : 0;
						CombatSkillItem combatSkillItem2 = CombatSkill.Instance[y.TemplateId];
						sbyte yEventRate = (combatSkillItem2 != null) ? combatSkillItem2.QiArtStrategyGenerateProbability : 0;
						comparisonResult = xEventRate.CompareTo(yEventRate);
						break;
					}
					case 149:
						comparisonResult = CombatSkillSortController.CompareStrategy(x, y);
						break;
					default:
						if (num2 != 216)
						{
							continue;
						}
						comparisonResult = x.CombatSkillProficiency.CompareTo(y.CombatSkillProficiency);
						break;
					}
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

		// Token: 0x0600AB45 RID: 43845 RVA: 0x004EAFE0 File Offset: 0x004E91E0
		public override SortUiConfig GenerateConfig()
		{
			EFilterType filterType = this._filterType;
			if (!true)
			{
			}
			SortUiConfig result;
			if (filterType != EFilterType.Looping)
			{
				if (filterType != EFilterType.Reference)
				{
					result = this.GenerateCommonConfig();
				}
				else
				{
					result = this.GenerateReferenceConfig();
				}
			}
			else
			{
				result = this.GenerateLoopingConfig();
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600AB46 RID: 43846 RVA: 0x004EB02C File Offset: 0x004E922C
		private SortUiConfig GenerateCommonConfig()
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

		// Token: 0x0600AB47 RID: 43847 RVA: 0x004EB0CC File Offset: 0x004E92CC
		private SortUiConfig GenerateLoopingConfig()
		{
			List<short> sortIds = new List<short>
			{
				144,
				145
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

		// Token: 0x0600AB48 RID: 43848 RVA: 0x004EB140 File Offset: 0x004E9340
		private SortUiConfig GenerateReferenceConfig()
		{
			List<short> sortIds = new List<short>
			{
				146,
				147,
				148,
				149
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

		// Token: 0x0600AB49 RID: 43849 RVA: 0x004EB1DC File Offset: 0x004E93DC
		private static int GetBonusCount(IFilterableCombatSkill data)
		{
			List<sbyte> breakBonusGrades = data.BreakBonusGrades;
			return (breakBonusGrades != null) ? breakBonusGrades.Count : 0;
		}

		// Token: 0x0600AB4A RID: 43850 RVA: 0x004EB200 File Offset: 0x004E9400
		private static int GetReadCount(IFilterableCombatSkill data)
		{
			return CombatSkillStateHelper.GetReadPagesCount(data.ReadingState);
		}

		// Token: 0x0600AB4B RID: 43851 RVA: 0x004EB220 File Offset: 0x004E9420
		private static int GetReferenceSkillBonusPercentForSorting(IFilterableCombatSkill referenceData)
		{
			bool flag = UIElement.Looping == null || UIElement.Looping.UiBaseAs<ViewLooping>().GetCurrentLoopingNeigongId() < 0;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				short CurrentLoopingNeigongId = UIElement.Looping.UiBaseAs<ViewLooping>().GetCurrentLoopingNeigongId();
				CombatSkillItem loopingConfig = CombatSkill.Instance[CurrentLoopingNeigongId];
				CombatSkillItem referenceConfig = CombatSkill.Instance[referenceData.TemplateId];
				bool flag2 = loopingConfig == null || referenceConfig == null;
				if (flag2)
				{
					result = 0;
				}
				else
				{
					int bonus = 0;
					bool flag3 = loopingConfig.LoopBonusSkillList.Contains(referenceConfig.TemplateId);
					if (flag3)
					{
						bonus += 10;
					}
					bool flag4 = referenceConfig.SectId == loopingConfig.SectId;
					if (flag4)
					{
						bonus += 20;
					}
					result = bonus;
				}
			}
			return result;
		}

		// Token: 0x0600AB4C RID: 43852 RVA: 0x004EB2DC File Offset: 0x004E94DC
		private static int CompareStrategy(IFilterableCombatSkill x, IFilterableCombatSkill y)
		{
			CombatSkillItem xConfig = CombatSkill.Instance[x.TemplateId];
			CombatSkillItem yConfig = CombatSkill.Instance[y.TemplateId];
			bool flag = xConfig == null || yConfig == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool xHasStrategy = xConfig.PossibleQiArtStrategyList != null && xConfig.PossibleQiArtStrategyList.Count > 0;
				bool yHasStrategy = yConfig.PossibleQiArtStrategyList != null && yConfig.PossibleQiArtStrategyList.Count > 0;
				bool flag2 = xHasStrategy && !yHasStrategy;
				if (flag2)
				{
					result = -1;
				}
				else
				{
					bool flag3 = !xHasStrategy && yHasStrategy;
					if (flag3)
					{
						result = 1;
					}
					else
					{
						bool flag4 = xHasStrategy && yHasStrategy;
						if (flag4)
						{
							sbyte xFirstStrategyId = xConfig.PossibleQiArtStrategyList[0];
							sbyte yFirstStrategyId = yConfig.PossibleQiArtStrategyList[0];
							result = xFirstStrategyId.CompareTo(yFirstStrategyId);
						}
						else
						{
							result = 0;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x04008504 RID: 34052
		private EFilterType _filterType;
	}
}
