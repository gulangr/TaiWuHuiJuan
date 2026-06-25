using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill
{
	// Token: 0x02000E23 RID: 3619
	public class AttackCombatSkillTypeMenu : DetailedFilterMenuLogic<IFilterableCombatSkill>
	{
		// Token: 0x170012F9 RID: 4857
		// (get) Token: 0x0600AB5F RID: 43871 RVA: 0x004EB800 File Offset: 0x004E9A00
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170012FA RID: 4858
		// (get) Token: 0x0600AB60 RID: 43872 RVA: 0x004EB803 File Offset: 0x004E9A03
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x170012FB RID: 4859
		// (get) Token: 0x0600AB61 RID: 43873 RVA: 0x004EB806 File Offset: 0x004E9A06
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 1));
			}
		}

		// Token: 0x0600AB62 RID: 43874 RVA: 0x004EB814 File Offset: 0x004E9A14
		public override StringKey GetMenuBarLabel()
		{
			return StringKey.CreateKey("LK_CombatSkill_Filter_AttackCombatSkillType");
		}

		// Token: 0x0600AB63 RID: 43875 RVA: 0x004EB830 File Offset: 0x004E9A30
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

		// Token: 0x0600AB64 RID: 43876 RVA: 0x004EB8A4 File Offset: 0x004E9AA4
		public override bool IsDataMatch(IFilterableCombatSkill data, IReadOnlyCollection<int> selectedIndices)
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

		// Token: 0x0400850D RID: 34061
		private readonly List<sbyte> _combatSkillTypeOptions = new List<sbyte>();
	}
}
