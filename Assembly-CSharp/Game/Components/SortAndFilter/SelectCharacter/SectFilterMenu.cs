using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.SelectCharacter
{
	// Token: 0x02000CFA RID: 3322
	public class SectFilterMenu<TData> : SelectCharacterFilterMenuBase<TData> where TData : ISelectCharacterData
	{
		// Token: 0x1700117F RID: 4479
		// (get) Token: 0x0600A6B7 RID: 42679 RVA: 0x004D83A1 File Offset: 0x004D65A1
		public override int Id
		{
			get
			{
				return 7;
			}
		}

		// Token: 0x0600A6B8 RID: 42680 RVA: 0x004D83A4 File Offset: 0x004D65A4
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Character_6;
		}

		// Token: 0x0600A6B9 RID: 42681 RVA: 0x004D83B0 File Offset: 0x004D65B0
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

		// Token: 0x0600A6BA RID: 42682 RVA: 0x004D8464 File Offset: 0x004D6664
		public override bool IsDataMatch(TData data, IReadOnlyCollection<int> selectedIndices)
		{
			CharacterDisplayDataForGeneralScrollList generalData = base.GetGeneralData(data);
			bool flag = generalData == null;
			return !flag && selectedIndices.Any((int index) => this._organizationOptions[index] == generalData.OrgInfo.OrgTemplateId);
		}

		// Token: 0x17001180 RID: 4480
		// (get) Token: 0x0600A6BB RID: 42683 RVA: 0x004D84B3 File Offset: 0x004D66B3
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(6, 1));
			}
		}

		// Token: 0x0400832D RID: 33581
		private readonly List<sbyte> _organizationOptions = new List<sbyte>();
	}
}
