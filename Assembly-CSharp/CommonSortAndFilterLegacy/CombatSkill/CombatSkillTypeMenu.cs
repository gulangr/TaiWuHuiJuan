using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.CombatSkill;

namespace CommonSortAndFilterLegacy.CombatSkill
{
	// Token: 0x02000574 RID: 1396
	public class CombatSkillTypeMenu : StaticDetailedFilterMenuBase<CombatSkillDisplayData>
	{
		// Token: 0x17000838 RID: 2104
		// (get) Token: 0x06004486 RID: 17542 RVA: 0x00209D68 File Offset: 0x00207F68
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x17000839 RID: 2105
		// (get) Token: 0x06004487 RID: 17543 RVA: 0x00209D6B File Offset: 0x00207F6B
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06004488 RID: 17544 RVA: 0x00209D70 File Offset: 0x00207F70
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_CombatSKill_0);
		}

		// Token: 0x06004489 RID: 17545 RVA: 0x00209D94 File Offset: 0x00207F94
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			this._combatSkillTypeOptions.Clear();
			List<DetailFilterMultiSelectDropdownItemConfig> dropdownConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			foreach (CombatSkillTypeItem combatSkillType in ((IEnumerable<CombatSkillTypeItem>)Config.CombatSkillType.Instance))
			{
				bool flag = combatSkillType.TemplateId == 2;
				if (flag)
				{
					dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
					{
						IconPath = combatSkillType.Icon,
						Text = LanguageKey.LK_CombatSkill_EquipType_3
					});
					List<CombatSkillTypeMenu.CombatSkillTypeOption> combatSkillTypeOptions = this._combatSkillTypeOptions;
					CombatSkillTypeMenu.CombatSkillTypeOption item = new CombatSkillTypeMenu.CombatSkillTypeOption
					{
						DisplayType = combatSkillType.TemplateId,
						MatchEquipType = new sbyte?(3)
					};
					combatSkillTypeOptions.Add(item);
					dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
					{
						IconPath = combatSkillType.Icon,
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
					dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
					{
						IconPath = combatSkillType.Icon,
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

		// Token: 0x0600448A RID: 17546 RVA: 0x00209F40 File Offset: 0x00208140
		public override bool IsDataMatch(CombatSkillDisplayData data, IReadOnlyCollection<int> selectedIndices)
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

		// Token: 0x04003018 RID: 12312
		private readonly List<CombatSkillTypeMenu.CombatSkillTypeOption> _combatSkillTypeOptions = new List<CombatSkillTypeMenu.CombatSkillTypeOption>();

		// Token: 0x02001957 RID: 6487
		private struct CombatSkillTypeOption
		{
			// Token: 0x0400B20B RID: 45579
			public sbyte DisplayType;

			// Token: 0x0400B20C RID: 45580
			public sbyte? MatchEquipType;
		}
	}
}
