using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill.Simplified
{
	// Token: 0x02000E49 RID: 3657
	public class FiveElementsMenu : DetailedFilterMenuLogic<CombatSkillDisplayDataForList>
	{
		// Token: 0x17001334 RID: 4916
		// (get) Token: 0x0600AC06 RID: 44038 RVA: 0x004ED418 File Offset: 0x004EB618
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001335 RID: 4917
		// (get) Token: 0x0600AC07 RID: 44039 RVA: 0x004ED41B File Offset: 0x004EB61B
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600AC08 RID: 44040 RVA: 0x004ED420 File Offset: 0x004EB620
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_CombatSKill_3;
		}

		// Token: 0x0600AC09 RID: 44041 RVA: 0x004ED43C File Offset: 0x004EB63C
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			this._fiveElementsOptions.Clear();
			List<FilterDropdownItemConfig> dropdownConfigs = new List<FilterDropdownItemConfig>();
			for (sbyte fiveElement = 0; fiveElement < 6; fiveElement += 1)
			{
				this._fiveElementsOptions.Add(fiveElement);
				dropdownConfigs.Add(new FilterDropdownItemConfig
				{
					Text = LocalStringManager.Get(string.Format("LK_FiveElements_Type_{0}", fiveElement))
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x0600AC0A RID: 44042 RVA: 0x004ED4B4 File Offset: 0x004EB6B4
		public override bool IsDataMatch(CombatSkillDisplayDataForList data, IReadOnlyCollection<int> selectedIndices)
		{
			CombatSkillItem combatSkillConfig = CombatSkill.Instance[data.TemplateId];
			sbyte fiveElement = combatSkillConfig.FiveElements;
			foreach (int selectionIndex in selectedIndices)
			{
				sbyte selectionFiveElement = this._fiveElementsOptions[selectionIndex];
				bool flag = selectionFiveElement == fiveElement;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0400852F RID: 34095
		private readonly List<sbyte> _fiveElementsOptions = new List<sbyte>();
	}
}
