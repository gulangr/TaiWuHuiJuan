using System;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill.Simplified
{
	// Token: 0x02000E4A RID: 3658
	public class CombatSkillStatusMenu : DetailedFilterMenuLogic<CombatSkillDisplayDataForList>
	{
		// Token: 0x17001336 RID: 4918
		// (get) Token: 0x0600AC0C RID: 44044 RVA: 0x004ED54C File Offset: 0x004EB74C
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001337 RID: 4919
		// (get) Token: 0x0600AC0D RID: 44045 RVA: 0x004ED54F File Offset: 0x004EB74F
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x0600AC0E RID: 44046 RVA: 0x004ED554 File Offset: 0x004EB754
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_CombatSKill_2;
		}

		// Token: 0x0600AC0F RID: 44047 RVA: 0x004ED570 File Offset: 0x004EB770
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig
				{
					Text = LanguageKey.LK_CommonSortAndFilter_DetailFilterOption_2_0
				},
				new FilterDropdownItemConfig
				{
					Text = LanguageKey.LK_CommonSortAndFilter_DetailFilterOption_2_2
				},
				new FilterDropdownItemConfig
				{
					Text = LanguageKey.LK_CommonSortAndFilter_DetailFilterOption_2_1
				},
				new FilterDropdownItemConfig
				{
					Text = LanguageKey.LK_CommonSortAndFilter_DetailFilterOption_2_3
				},
				new FilterDropdownItemConfig
				{
					Text = LanguageKey.LK_CommonSortAndFilter_DetailFilterOption_2_4
				}
			};
		}

		// Token: 0x0600AC10 RID: 44048 RVA: 0x004ED630 File Offset: 0x004EB830
		public override bool IsDataMatch(CombatSkillDisplayDataForList data, IReadOnlyCollection<int> selectedIndices)
		{
			bool flag = selectedIndices == null || selectedIndices.Count == 0;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				foreach (int selectionIndex in selectedIndices)
				{
					if (!true)
					{
					}
					bool flag2;
					switch (selectionIndex)
					{
					case 0:
						flag2 = CombatSkillStatusMenu.IsCombatSkillBroken(data);
						break;
					case 1:
						flag2 = CombatSkillStatusMenu.IsCombatSkillNotBroken(data);
						break;
					case 2:
						flag2 = CombatSkillStatusMenu.IsCombatSkillEquipped(data);
						break;
					case 3:
						flag2 = CombatSkillStatusMenu.IsCombatSkillNotEquipped(data);
						break;
					default:
						flag2 = CombatSkillStatusMenu.IsCombatSkillHasEmeiSkillBreakBonus(data);
						break;
					}
					if (!true)
					{
					}
					bool res = flag2;
					bool flag3 = res;
					if (flag3)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x0600AC11 RID: 44049 RVA: 0x004ED6F8 File Offset: 0x004EB8F8
		private static bool IsCombatSkillBroken(CombatSkillDisplayDataForList data)
		{
			return CombatSkillStateHelper.IsBrokenOut(data.ActivationState);
		}

		// Token: 0x0600AC12 RID: 44050 RVA: 0x004ED718 File Offset: 0x004EB918
		private static bool IsCombatSkillNotBroken(CombatSkillDisplayDataForList data)
		{
			return !CombatSkillStateHelper.IsBrokenOut(data.ActivationState);
		}

		// Token: 0x0600AC13 RID: 44051 RVA: 0x004ED738 File Offset: 0x004EB938
		private static bool IsCombatSkillEquipped(CombatSkillDisplayDataForList data)
		{
			return data.IsInAnyEquipPlans;
		}

		// Token: 0x0600AC14 RID: 44052 RVA: 0x004ED750 File Offset: 0x004EB950
		private static bool IsCombatSkillNotEquipped(CombatSkillDisplayDataForList data)
		{
			return !data.IsInAnyEquipPlans;
		}

		// Token: 0x0600AC15 RID: 44053 RVA: 0x004ED76C File Offset: 0x004EB96C
		private static bool IsCombatSkillHasEmeiSkillBreakBonus(CombatSkillDisplayDataForList data)
		{
			return data.EmeiBonus1 >= 0 || data.EmeiBonus2 >= 0;
		}
	}
}
