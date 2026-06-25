using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill
{
	// Token: 0x02000E34 RID: 3636
	public class CombatSkillCharacterMenuItemTypeMenu : DetailedFilterMenuLogic<CombatSkillDisplayDataCharacterMenuListItem>
	{
		// Token: 0x17001315 RID: 4885
		// (get) Token: 0x0600ABA6 RID: 43942 RVA: 0x004EBF90 File Offset: 0x004EA190
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001316 RID: 4886
		// (get) Token: 0x0600ABA7 RID: 43943 RVA: 0x004EBF93 File Offset: 0x004EA193
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600ABA8 RID: 43944 RVA: 0x004EBF98 File Offset: 0x004EA198
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_CombatSKill_0;
		}

		// Token: 0x0600ABA9 RID: 43945 RVA: 0x004EBFB4 File Offset: 0x004EA1B4
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
					List<CombatSkillCharacterMenuItemTypeMenu.CombatSkillTypeOption> combatSkillTypeOptions = this._combatSkillTypeOptions;
					CombatSkillCharacterMenuItemTypeMenu.CombatSkillTypeOption item = new CombatSkillCharacterMenuItemTypeMenu.CombatSkillTypeOption
					{
						DisplayType = combatSkillType.TemplateId,
						MatchEquipType = new sbyte?(3)
					};
					combatSkillTypeOptions.Add(item);
					dropdownConfigs.Add(new FilterDropdownItemConfig
					{
						Text = LanguageKey.LK_CombatSkill_EquipType_4
					});
					List<CombatSkillCharacterMenuItemTypeMenu.CombatSkillTypeOption> combatSkillTypeOptions2 = this._combatSkillTypeOptions;
					item = new CombatSkillCharacterMenuItemTypeMenu.CombatSkillTypeOption
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
					List<CombatSkillCharacterMenuItemTypeMenu.CombatSkillTypeOption> combatSkillTypeOptions3 = this._combatSkillTypeOptions;
					CombatSkillCharacterMenuItemTypeMenu.CombatSkillTypeOption item = new CombatSkillCharacterMenuItemTypeMenu.CombatSkillTypeOption
					{
						DisplayType = combatSkillType.TemplateId,
						MatchEquipType = null
					};
					combatSkillTypeOptions3.Add(item);
				}
			}
			return dropdownConfigs;
		}

		// Token: 0x0600ABAA RID: 43946 RVA: 0x004EC138 File Offset: 0x004EA338
		public override bool IsDataMatch(CombatSkillDisplayDataCharacterMenuListItem data, IReadOnlyCollection<int> selectedIndices)
		{
			CombatSkillItem combatSkillConfig = CombatSkill.Instance[data.TemplateId];
			sbyte dataCombatSkillType = combatSkillConfig.Type;
			sbyte dataEquipType = combatSkillConfig.EquipType;
			foreach (int selectionIndex in selectedIndices)
			{
				CombatSkillCharacterMenuItemTypeMenu.CombatSkillTypeOption selectionOption = this._combatSkillTypeOptions[selectionIndex];
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

		// Token: 0x0400851F RID: 34079
		private readonly List<CombatSkillCharacterMenuItemTypeMenu.CombatSkillTypeOption> _combatSkillTypeOptions = new List<CombatSkillCharacterMenuItemTypeMenu.CombatSkillTypeOption>();

		// Token: 0x020024D3 RID: 9427
		private struct CombatSkillTypeOption
		{
			// Token: 0x0400E5CE RID: 58830
			public sbyte DisplayType;

			// Token: 0x0400E5CF RID: 58831
			public sbyte? MatchEquipType;
		}
	}
}
