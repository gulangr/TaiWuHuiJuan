using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy.Character
{
	// Token: 0x020005A2 RID: 1442
	public class OrganizationSectMenu<T> : StaticDetailedFilterMenuBase<T> where T : ICharacterSortAndFilterData
	{
		// Token: 0x17000898 RID: 2200
		// (get) Token: 0x0600456C RID: 17772 RVA: 0x0020C250 File Offset: 0x0020A450
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x17000899 RID: 2201
		// (get) Token: 0x0600456D RID: 17773 RVA: 0x0020C253 File Offset: 0x0020A453
		public override int Id
		{
			get
			{
				return 6;
			}
		}

		// Token: 0x0600456E RID: 17774 RVA: 0x0020C258 File Offset: 0x0020A458
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Character_6);
		}

		// Token: 0x0600456F RID: 17775 RVA: 0x0020C27C File Offset: 0x0020A47C
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

		// Token: 0x06004570 RID: 17776 RVA: 0x0020C338 File Offset: 0x0020A538
		public override bool IsDataMatch(T data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any((int index) => this._organizationOptions[index] == data.Organization.OrgTemplateId);
		}

		// Token: 0x1700089A RID: 2202
		// (get) Token: 0x06004571 RID: 17777 RVA: 0x0020C370 File Offset: 0x0020A570
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(5, 1));
			}
		}

		// Token: 0x0400306C RID: 12396
		private readonly List<sbyte> _organizationOptions = new List<sbyte>();
	}
}
