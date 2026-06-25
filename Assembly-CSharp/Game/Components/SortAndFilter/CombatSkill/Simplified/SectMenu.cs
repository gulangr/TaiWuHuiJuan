using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill.Simplified
{
	// Token: 0x02000E48 RID: 3656
	public class SectMenu : DetailedFilterMenuLogic<CombatSkillDisplayDataForList>
	{
		// Token: 0x17001332 RID: 4914
		// (get) Token: 0x0600AC00 RID: 44032 RVA: 0x004ED2A8 File Offset: 0x004EB4A8
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001333 RID: 4915
		// (get) Token: 0x0600AC01 RID: 44033 RVA: 0x004ED2AB File Offset: 0x004EB4AB
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600AC02 RID: 44034 RVA: 0x004ED2B0 File Offset: 0x004EB4B0
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_CombatSKill_1;
		}

		// Token: 0x0600AC03 RID: 44035 RVA: 0x004ED2CC File Offset: 0x004EB4CC
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			this._organizationOptions.Clear();
			List<FilterDropdownItemConfig> dropdownConfigs = new List<FilterDropdownItemConfig>();
			foreach (OrganizationItem organization in ((IEnumerable<OrganizationItem>)Organization.Instance))
			{
				bool flag = organization.TemplateId != 0 && !organization.IsSect;
				if (!flag)
				{
					dropdownConfigs.Add(new FilterDropdownItemConfig
					{
						Text = organization.Name
					});
					this._organizationOptions.Add(organization.TemplateId);
				}
			}
			return dropdownConfigs;
		}

		// Token: 0x0600AC04 RID: 44036 RVA: 0x004ED380 File Offset: 0x004EB580
		public override bool IsDataMatch(CombatSkillDisplayDataForList data, IReadOnlyCollection<int> selectedIndices)
		{
			CombatSkillItem combatSkillConfig = CombatSkill.Instance[data.TemplateId];
			sbyte sectId = combatSkillConfig.SectId;
			foreach (int selectionIndex in selectedIndices)
			{
				sbyte selectionOrganization = this._organizationOptions[selectionIndex];
				bool flag = selectionOrganization == sectId;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0400852E RID: 34094
		private readonly List<sbyte> _organizationOptions = new List<sbyte>();
	}
}
