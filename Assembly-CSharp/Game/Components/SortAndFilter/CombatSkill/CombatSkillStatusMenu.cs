using System;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill
{
	// Token: 0x02000E3A RID: 3642
	public class CombatSkillStatusMenu : DetailedFilterMenuLogic<IFilterableCombatSkill>
	{
		// Token: 0x1700131E RID: 4894
		// (get) Token: 0x0600ABC3 RID: 43971 RVA: 0x004EC758 File Offset: 0x004EA958
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x1700131F RID: 4895
		// (get) Token: 0x0600ABC4 RID: 43972 RVA: 0x004EC75B File Offset: 0x004EA95B
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600ABC5 RID: 43973 RVA: 0x004EC760 File Offset: 0x004EA960
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_CombatSKill_2;
		}

		// Token: 0x0600ABC6 RID: 43974 RVA: 0x004EC77C File Offset: 0x004EA97C
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
					Text = LanguageKey.LK_CommonSortAndFilter_DetailFilterOption_2_5
				},
				new FilterDropdownItemConfig
				{
					Text = LanguageKey.LK_CommonSortAndFilter_DetailFilterOption_2_4
				}
			};
		}

		// Token: 0x0600ABC7 RID: 43975 RVA: 0x004EC85C File Offset: 0x004EAA5C
		public override bool IsDataMatch(IFilterableCombatSkill data, IReadOnlyCollection<int> selectedIndices)
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
					case 4:
						flag2 = CombatSkillStatusMenu.IsCombatSkillFavorite(data);
						break;
					case 5:
						flag2 = CombatSkillStatusMenu.IsCombatSkillHasEmeiSkillBreakBonus(data);
						break;
					default:
						flag2 = false;
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

		// Token: 0x0600ABC8 RID: 43976 RVA: 0x004EC940 File Offset: 0x004EAB40
		private static bool IsCombatSkillBroken(IFilterableCombatSkill data)
		{
			return CombatSkillStateHelper.IsBrokenOut(data.ActivationState);
		}

		// Token: 0x0600ABC9 RID: 43977 RVA: 0x004EC960 File Offset: 0x004EAB60
		private static bool IsCombatSkillNotBroken(IFilterableCombatSkill data)
		{
			return !CombatSkillStateHelper.IsBrokenOut(data.ActivationState);
		}

		// Token: 0x0600ABCA RID: 43978 RVA: 0x004EC980 File Offset: 0x004EAB80
		private static bool IsCombatSkillEquipped(IFilterableCombatSkill data)
		{
			return data.IsInAnyEquipPlans;
		}

		// Token: 0x0600ABCB RID: 43979 RVA: 0x004EC998 File Offset: 0x004EAB98
		private static bool IsCombatSkillNotEquipped(IFilterableCombatSkill data)
		{
			return !data.IsInAnyEquipPlans;
		}

		// Token: 0x0600ABCC RID: 43980 RVA: 0x004EC9B4 File Offset: 0x004EABB4
		private static bool IsCombatSkillFavorite(IFilterableCombatSkill data)
		{
			CombatSkillDisplayDataCharacterMenuListItem item = data as CombatSkillDisplayDataCharacterMenuListItem;
			return item != null && item.IsFavorite;
		}

		// Token: 0x0600ABCD RID: 43981 RVA: 0x004EC9DC File Offset: 0x004EABDC
		private static bool IsCombatSkillHasEmeiSkillBreakBonus(IFilterableCombatSkill data)
		{
			return data.HasSectEmeiSkillBreakBonus;
		}
	}
}
