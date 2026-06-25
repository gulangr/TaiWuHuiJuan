using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill
{
	// Token: 0x02000E24 RID: 3620
	public class SectMenu : DetailedFilterMenuLogic<IFilterableCombatSkill>
	{
		// Token: 0x170012FC RID: 4860
		// (get) Token: 0x0600AB66 RID: 43878 RVA: 0x004EB954 File Offset: 0x004E9B54
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170012FD RID: 4861
		// (get) Token: 0x0600AB67 RID: 43879 RVA: 0x004EB957 File Offset: 0x004E9B57
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600AB68 RID: 43880 RVA: 0x004EB95C File Offset: 0x004E9B5C
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_CombatSKill_1;
		}

		// Token: 0x0600AB69 RID: 43881 RVA: 0x004EB978 File Offset: 0x004E9B78
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

		// Token: 0x0600AB6A RID: 43882 RVA: 0x004EBA2C File Offset: 0x004E9C2C
		public override bool IsDataMatch(IFilterableCombatSkill data, IReadOnlyCollection<int> selectedIndices)
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

		// Token: 0x0400850E RID: 34062
		private readonly List<sbyte> _organizationOptions = new List<sbyte>();
	}
}
