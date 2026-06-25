using System;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill
{
	// Token: 0x02000E31 RID: 3633
	public class CombatSkillCharacterMenuItemStatusMenu : DetailedFilterMenuLogic<CombatSkillDisplayDataCharacterMenuListItem>
	{
		// Token: 0x17001310 RID: 4880
		// (get) Token: 0x0600AB98 RID: 43928 RVA: 0x004EBDA4 File Offset: 0x004E9FA4
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001311 RID: 4881
		// (get) Token: 0x0600AB99 RID: 43929 RVA: 0x004EBDA7 File Offset: 0x004E9FA7
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AB9A RID: 43930 RVA: 0x004EBDAC File Offset: 0x004E9FAC
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_CombatSKill_2;
		}

		// Token: 0x0600AB9B RID: 43931 RVA: 0x004EBDC8 File Offset: 0x004E9FC8
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> dropdownConfigs = new List<FilterDropdownItemConfig>
			{
				Capacity = this._combatSkillStatusConfigs.Count
			};
			foreach (StringKey config in this._combatSkillStatusConfigs)
			{
				dropdownConfigs.Add(new FilterDropdownItemConfig
				{
					Text = config
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x0600AB9C RID: 43932 RVA: 0x004EBE50 File Offset: 0x004EA050
		public override bool IsDataMatch(CombatSkillDisplayDataCharacterMenuListItem data, IReadOnlyCollection<int> selectedIndices)
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
					if (selectionIndex != 0)
					{
						flag2 = CombatSkillCharacterMenuItemStatusMenu.IsCombatSkillNotBroken(data);
					}
					else
					{
						flag2 = CombatSkillCharacterMenuItemStatusMenu.IsCombatSkillBroken(data);
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

		// Token: 0x0600AB9D RID: 43933 RVA: 0x004EBEE4 File Offset: 0x004EA0E4
		private static bool IsCombatSkillBroken(CombatSkillDisplayDataCharacterMenuListItem data)
		{
			return CombatSkillStateHelper.IsBrokenOut(data.ActivationState);
		}

		// Token: 0x0600AB9E RID: 43934 RVA: 0x004EBF04 File Offset: 0x004EA104
		private static bool IsCombatSkillNotBroken(CombatSkillDisplayDataCharacterMenuListItem data)
		{
			return !CombatSkillStateHelper.IsBrokenOut(data.ActivationState);
		}

		// Token: 0x0400851A RID: 34074
		private readonly List<StringKey> _combatSkillStatusConfigs = new List<StringKey>
		{
			LanguageKey.LK_CommonSortAndFilter_DetailFilterOption_2_0,
			LanguageKey.LK_CommonSortAndFilter_DetailFilterOption_2_2
		};
	}
}
