using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill
{
	// Token: 0x02000E21 RID: 3617
	public class CombatSkillTypeMenu : DetailedFilterMenuLogic<IFilterableCombatSkill>
	{
		// Token: 0x170012F5 RID: 4853
		// (get) Token: 0x0600AB53 RID: 43859 RVA: 0x004EB3F0 File Offset: 0x004E95F0
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170012F6 RID: 4854
		// (get) Token: 0x0600AB54 RID: 43860 RVA: 0x004EB3F3 File Offset: 0x004E95F3
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AB55 RID: 43861 RVA: 0x004EB3F8 File Offset: 0x004E95F8
		public override StringKey GetMenuBarLabel()
		{
			return StringKey.CreateKey("LK_CombatSkill_Filter_QiType");
		}

		// Token: 0x0600AB56 RID: 43862 RVA: 0x004EB414 File Offset: 0x004E9614
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

		// Token: 0x0600AB57 RID: 43863 RVA: 0x004EB488 File Offset: 0x004E9688
		public override bool IsDataMatch(IFilterableCombatSkill data, IReadOnlyCollection<int> selectedIndices)
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

		// Token: 0x0400850B RID: 34059
		private readonly List<sbyte> _equipTypeOptions = new List<sbyte>();
	}
}
