using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.CombatSkill;

namespace CommonSortAndFilterLegacy.CombatSkill
{
	// Token: 0x02000575 RID: 1397
	public class SectMenu : StaticDetailedFilterMenuBase<CombatSkillDisplayData>
	{
		// Token: 0x1700083A RID: 2106
		// (get) Token: 0x0600448C RID: 17548 RVA: 0x0020A05C File Offset: 0x0020825C
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x1700083B RID: 2107
		// (get) Token: 0x0600448D RID: 17549 RVA: 0x0020A05F File Offset: 0x0020825F
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600448E RID: 17550 RVA: 0x0020A064 File Offset: 0x00208264
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_CombatSKill_1);
		}

		// Token: 0x0600448F RID: 17551 RVA: 0x0020A088 File Offset: 0x00208288
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			this._organizationOptions.Clear();
			List<DetailFilterMultiSelectDropdownItemConfig> dropdownConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			foreach (OrganizationItem organization in ((IEnumerable<OrganizationItem>)Organization.Instance))
			{
				bool flag = organization.TemplateId != 0 && !organization.IsSect;
				if (!flag)
				{
					dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
					{
						IconPath = null,
						Text = organization.Name
					});
					this._organizationOptions.Add(organization.TemplateId);
				}
			}
			return dropdownConfigs;
		}

		// Token: 0x06004490 RID: 17552 RVA: 0x0020A144 File Offset: 0x00208344
		public override bool IsDataMatch(CombatSkillDisplayData data, IReadOnlyCollection<int> selectedIndices)
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

		// Token: 0x04003019 RID: 12313
		private readonly List<sbyte> _organizationOptions = new List<sbyte>();
	}
}
