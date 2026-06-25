using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill.Simplified
{
	// Token: 0x02000E45 RID: 3653
	public class CombatSkillQiTypeMenu : DetailedFilterMenuLogic<CombatSkillDisplayDataForList>
	{
		// Token: 0x1700132B RID: 4907
		// (get) Token: 0x0600ABED RID: 44013 RVA: 0x004ECD44 File Offset: 0x004EAF44
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x1700132C RID: 4908
		// (get) Token: 0x0600ABEE RID: 44014 RVA: 0x004ECD47 File Offset: 0x004EAF47
		public override int Id
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x0600ABEF RID: 44015 RVA: 0x004ECD4C File Offset: 0x004EAF4C
		public override StringKey GetMenuBarLabel()
		{
			return StringKey.CreateKey("LK_CombatSkill_Filter_QiType");
		}

		// Token: 0x0600ABF0 RID: 44016 RVA: 0x004ECD68 File Offset: 0x004EAF68
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			this._equipTypeOptions.Clear();
			List<FilterDropdownItemConfig> dropdownConfigs = new List<FilterDropdownItemConfig>();
			for (sbyte equipType = 0; equipType < 5; equipType += 1)
			{
				this._equipTypeOptions.Add(equipType);
				dropdownConfigs.Add(new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(string.Format("LK_CombatSkill_EquipType_{0}", equipType))
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x0600ABF1 RID: 44017 RVA: 0x004ECDDC File Offset: 0x004EAFDC
		public override bool IsDataMatch(CombatSkillDisplayDataForList data, IReadOnlyCollection<int> selectedIndices)
		{
			CombatSkillItem combatSkillConfig = CombatSkill.Instance[data.TemplateId];
			sbyte dataEquipType = combatSkillConfig.EquipType;
			foreach (int selectionIndex in selectedIndices)
			{
				bool flag = selectionIndex < 0 || selectionIndex >= this._equipTypeOptions.Count;
				if (!flag)
				{
					bool flag2 = this._equipTypeOptions[selectionIndex] == dataEquipType;
					if (flag2)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0400852B RID: 34091
		private readonly List<sbyte> _equipTypeOptions = new List<sbyte>();
	}
}
