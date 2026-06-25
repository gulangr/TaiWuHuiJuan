using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill
{
	// Token: 0x02000E35 RID: 3637
	public class CharacterMenuItemSectMenu : DetailedFilterMenuLogic<CombatSkillDisplayDataCharacterMenuListItem>
	{
		// Token: 0x17001317 RID: 4887
		// (get) Token: 0x0600ABAC RID: 43948 RVA: 0x004EC254 File Offset: 0x004EA454
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001318 RID: 4888
		// (get) Token: 0x0600ABAD RID: 43949 RVA: 0x004EC257 File Offset: 0x004EA457
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600ABAE RID: 43950 RVA: 0x004EC25C File Offset: 0x004EA45C
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_CombatSKill_1;
		}

		// Token: 0x0600ABAF RID: 43951 RVA: 0x004EC278 File Offset: 0x004EA478
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

		// Token: 0x0600ABB0 RID: 43952 RVA: 0x004EC32C File Offset: 0x004EA52C
		public override bool IsDataMatch(CombatSkillDisplayDataCharacterMenuListItem data, IReadOnlyCollection<int> selectedIndices)
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

		// Token: 0x04008520 RID: 34080
		private readonly List<sbyte> _organizationOptions = new List<sbyte>();
	}
}
