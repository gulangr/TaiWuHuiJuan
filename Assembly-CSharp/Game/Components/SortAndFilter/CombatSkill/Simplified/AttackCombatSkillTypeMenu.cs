using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill.Simplified
{
	// Token: 0x02000E46 RID: 3654
	public class AttackCombatSkillTypeMenu : DetailedFilterMenuLogic<CombatSkillDisplayDataForList>
	{
		// Token: 0x1700132D RID: 4909
		// (get) Token: 0x0600ABF3 RID: 44019 RVA: 0x004ECE90 File Offset: 0x004EB090
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x1700132E RID: 4910
		// (get) Token: 0x0600ABF4 RID: 44020 RVA: 0x004ECE93 File Offset: 0x004EB093
		public override int Id
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x1700132F RID: 4911
		// (get) Token: 0x0600ABF5 RID: 44021 RVA: 0x004ECE96 File Offset: 0x004EB096
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(4, 1));
			}
		}

		// Token: 0x0600ABF6 RID: 44022 RVA: 0x004ECEA4 File Offset: 0x004EB0A4
		public override StringKey GetMenuBarLabel()
		{
			return StringKey.CreateKey("LK_CombatSkill_Filter_AttackCombatSkillType");
		}

		// Token: 0x0600ABF7 RID: 44023 RVA: 0x004ECEC0 File Offset: 0x004EB0C0
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			this._combatSkillTypeOptions.Clear();
			List<FilterDropdownItemConfig> dropdownConfigs = new List<FilterDropdownItemConfig>();
			for (sbyte combatSkillType = 3; combatSkillType < 14; combatSkillType += 1)
			{
				this._combatSkillTypeOptions.Add(combatSkillType);
				dropdownConfigs.Add(new FilterDropdownItemConfig
				{
					Text = Config.CombatSkillType.Instance[combatSkillType].Name
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x0600ABF8 RID: 44024 RVA: 0x004ECF34 File Offset: 0x004EB134
		public override bool IsDataMatch(CombatSkillDisplayDataForList data, IReadOnlyCollection<int> selectedIndices)
		{
			sbyte dataCombatSkillType = CombatSkill.Instance[data.TemplateId].Type;
			foreach (int selectionIndex in selectedIndices)
			{
				bool flag = selectionIndex < 0 || selectionIndex >= this._combatSkillTypeOptions.Count;
				if (!flag)
				{
					bool flag2 = this._combatSkillTypeOptions[selectionIndex] == dataCombatSkillType;
					if (flag2)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0400852C RID: 34092
		private readonly List<sbyte> _combatSkillTypeOptions = new List<sbyte>();
	}
}
