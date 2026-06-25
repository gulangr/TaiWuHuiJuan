using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill
{
	// Token: 0x02000E22 RID: 3618
	public class CombatSkillSkillTypeMenu : DetailedFilterMenuLogic<IFilterableCombatSkill>
	{
		// Token: 0x170012F7 RID: 4855
		// (get) Token: 0x0600AB59 RID: 43865 RVA: 0x004EB53C File Offset: 0x004E973C
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170012F8 RID: 4856
		// (get) Token: 0x0600AB5A RID: 43866 RVA: 0x004EB53F File Offset: 0x004E973F
		public override int Id
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x0600AB5B RID: 43867 RVA: 0x004EB544 File Offset: 0x004E9744
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_CombatSKill_0;
		}

		// Token: 0x0600AB5C RID: 43868 RVA: 0x004EB560 File Offset: 0x004E9760
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
					List<CombatSkillSkillTypeMenu.CombatSkillTypeOption> combatSkillTypeOptions = this._combatSkillTypeOptions;
					CombatSkillSkillTypeMenu.CombatSkillTypeOption item = new CombatSkillSkillTypeMenu.CombatSkillTypeOption
					{
						DisplayType = combatSkillType.TemplateId,
						MatchEquipType = new sbyte?(3)
					};
					combatSkillTypeOptions.Add(item);
					dropdownConfigs.Add(new FilterDropdownItemConfig
					{
						Text = LanguageKey.LK_CombatSkill_EquipType_4
					});
					List<CombatSkillSkillTypeMenu.CombatSkillTypeOption> combatSkillTypeOptions2 = this._combatSkillTypeOptions;
					item = new CombatSkillSkillTypeMenu.CombatSkillTypeOption
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
					List<CombatSkillSkillTypeMenu.CombatSkillTypeOption> combatSkillTypeOptions3 = this._combatSkillTypeOptions;
					CombatSkillSkillTypeMenu.CombatSkillTypeOption item = new CombatSkillSkillTypeMenu.CombatSkillTypeOption
					{
						DisplayType = combatSkillType.TemplateId,
						MatchEquipType = null
					};
					combatSkillTypeOptions3.Add(item);
				}
			}
			return dropdownConfigs;
		}

		// Token: 0x0600AB5D RID: 43869 RVA: 0x004EB6E4 File Offset: 0x004E98E4
		public override bool IsDataMatch(IFilterableCombatSkill data, IReadOnlyCollection<int> selectedIndices)
		{
			CombatSkillItem combatSkillConfig = CombatSkill.Instance[data.TemplateId];
			sbyte dataCombatSkillType = combatSkillConfig.Type;
			sbyte dataEquipType = combatSkillConfig.EquipType;
			foreach (int selectionIndex in selectedIndices)
			{
				CombatSkillSkillTypeMenu.CombatSkillTypeOption selectionOption = this._combatSkillTypeOptions[selectionIndex];
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

		// Token: 0x0400850C RID: 34060
		private readonly List<CombatSkillSkillTypeMenu.CombatSkillTypeOption> _combatSkillTypeOptions = new List<CombatSkillSkillTypeMenu.CombatSkillTypeOption>();

		// Token: 0x020024C9 RID: 9417
		private struct CombatSkillTypeOption
		{
			// Token: 0x0400E5A8 RID: 58792
			public sbyte DisplayType;

			// Token: 0x0400E5A9 RID: 58793
			public sbyte? MatchEquipType;
		}
	}
}
