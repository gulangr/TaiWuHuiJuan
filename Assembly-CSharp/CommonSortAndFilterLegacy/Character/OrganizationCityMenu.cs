using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy.Character
{
	// Token: 0x0200059F RID: 1439
	public class OrganizationCityMenu<T> : StaticDetailedFilterMenuBase<T> where T : ICharacterSortAndFilterData
	{
		// Token: 0x17000893 RID: 2195
		// (get) Token: 0x0600455D RID: 17757 RVA: 0x0020BFC4 File Offset: 0x0020A1C4
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x0600455E RID: 17758 RVA: 0x0020BFC8 File Offset: 0x0020A1C8
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Character_7);
		}

		// Token: 0x0600455F RID: 17759 RVA: 0x0020BFEC File Offset: 0x0020A1EC
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			this._organizationList.Clear();
			this._organizationList.AddRange(CommonUtils.IterCityOrganizationKeys());
			return (from organizationId in this._organizationList
			select Organization.Instance[organizationId] into organization
			select new DetailFilterMultiSelectDropdownItemConfig
			{
				IconPath = null,
				Text = StringKey.CreateDirect(organization.Name)
			}).ToList<DetailFilterMultiSelectDropdownItemConfig>();
		}

		// Token: 0x06004560 RID: 17760 RVA: 0x0020C070 File Offset: 0x0020A270
		public override bool IsDataMatch(T data, IReadOnlyCollection<int> selectedIndices)
		{
			sbyte organizationId = data.Organization.OrgTemplateId;
			return selectedIndices.Any((int index) => this._organizationList[index] == organizationId);
		}

		// Token: 0x17000894 RID: 2196
		// (get) Token: 0x06004561 RID: 17761 RVA: 0x0020C0B9 File Offset: 0x0020A2B9
		public override int Id
		{
			get
			{
				return 7;
			}
		}

		// Token: 0x17000895 RID: 2197
		// (get) Token: 0x06004562 RID: 17762 RVA: 0x0020C0BC File Offset: 0x0020A2BC
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(5, 2));
			}
		}

		// Token: 0x04003062 RID: 12386
		private readonly List<sbyte> _organizationList = new List<sbyte>();
	}
}
