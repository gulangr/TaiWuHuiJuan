using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill.Simplified
{
	// Token: 0x02000E47 RID: 3655
	public class CombatSkillTypeMenu : DetailedFilterMenuLogic<CombatSkillDisplayDataForList>
	{
		// Token: 0x17001330 RID: 4912
		// (get) Token: 0x0600ABFA RID: 44026 RVA: 0x004ECFE4 File Offset: 0x004EB1E4
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001331 RID: 4913
		// (get) Token: 0x0600ABFB RID: 44027 RVA: 0x004ECFE7 File Offset: 0x004EB1E7
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600ABFC RID: 44028 RVA: 0x004ECFEC File Offset: 0x004EB1EC
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_CombatSKill_0;
		}

		// Token: 0x0600ABFD RID: 44029 RVA: 0x004ED008 File Offset: 0x004EB208
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			this._combatSkillTypeOptions.Clear();
			List<FilterDropdownItemConfig> dropdownConfigs = new List<FilterDropdownItemConfig>();
			foreach (CombatSkillTypeItem combatSkillType in ((IEnumerable<CombatSkillTypeItem>)Config.CombatSkillType.Instance))
			{
				bool flag = combatSkillType.TemplateId == 2;
				if (flag)
				{
					dropdownConfigs.Add(new FilterDropdownItemConfig
					{
						Text = LanguageKey.LK_CombatSkill_EquipType_3
					});
					List<CombatSkillTypeMenu.CombatSkillTypeOption> combatSkillTypeOptions = this._combatSkillTypeOptions;
					CombatSkillTypeMenu.CombatSkillTypeOption item = new CombatSkillTypeMenu.CombatSkillTypeOption
					{
						DisplayType = combatSkillType.TemplateId,
						MatchEquipType = new sbyte?(3)
					};
					combatSkillTypeOptions.Add(item);
					dropdownConfigs.Add(new FilterDropdownItemConfig
					{
						Text = LanguageKey.LK_CombatSkill_EquipType_4
					});
					List<CombatSkillTypeMenu.CombatSkillTypeOption> combatSkillTypeOptions2 = this._combatSkillTypeOptions;
					item = new CombatSkillTypeMenu.CombatSkillTypeOption
					{
						DisplayType = combatSkillType.TemplateId,
						MatchEquipType = new sbyte?(4)
					};
					combatSkillTypeOptions2.Add(item);
				}
				else
				{
					dropdownConfigs.Add(new FilterDropdownItemConfig
					{
						Text = combatSkillType.Name
					});
					List<CombatSkillTypeMenu.CombatSkillTypeOption> combatSkillTypeOptions3 = this._combatSkillTypeOptions;
					CombatSkillTypeMenu.CombatSkillTypeOption item = new CombatSkillTypeMenu.CombatSkillTypeOption
					{
						DisplayType = combatSkillType.TemplateId,
						MatchEquipType = null
					};
					combatSkillTypeOptions3.Add(item);
				}
			}
			return dropdownConfigs;
		}

		// Token: 0x0600ABFE RID: 44030 RVA: 0x004ED18C File Offset: 0x004EB38C
		public override bool IsDataMatch(CombatSkillDisplayDataForList data, IReadOnlyCollection<int> selectedIndices)
		{
			CombatSkillItem combatSkillConfig = CombatSkill.Instance[data.TemplateId];
			sbyte dataCombatSkillType = combatSkillConfig.Type;
			sbyte dataEquipType = combatSkillConfig.EquipType;
			foreach (int selectionIndex in selectedIndices)
			{
				CombatSkillTypeMenu.CombatSkillTypeOption selectionOption = this._combatSkillTypeOptions[selectionIndex];
				bool flag = selectionOption.MatchEquipType == null;
				if (flag)
				{
					bool flag2 = selectionOption.DisplayType == dataCombatSkillType;
					if (flag2)
					{
						return true;
					}
				}
				else
				{
					bool flag3;
					if (dataCombatSkillType == 2)
					{
						sbyte? matchEquipType = selectionOption.MatchEquipType;
						int? num = (matchEquipType != null) ? new int?((int)matchEquipType.GetValueOrDefault()) : null;
						int num2 = (int)dataEquipType;
						flag3 = (num.GetValueOrDefault() == num2 & num != null);
					}
					else
					{
						flag3 = false;
					}
					bool flag4 = flag3;
					if (flag4)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0400852D RID: 34093
		private readonly List<CombatSkillTypeMenu.CombatSkillTypeOption> _combatSkillTypeOptions = new List<CombatSkillTypeMenu.CombatSkillTypeOption>();

		// Token: 0x020024DA RID: 9434
		private struct CombatSkillTypeOption
		{
			// Token: 0x0400E5E2 RID: 58850
			public sbyte DisplayType;

			// Token: 0x0400E5E3 RID: 58851
			public sbyte? MatchEquipType;
		}
	}
}
