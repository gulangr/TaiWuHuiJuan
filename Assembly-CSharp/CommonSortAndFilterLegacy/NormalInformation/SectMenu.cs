using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Information;

namespace CommonSortAndFilterLegacy.NormalInformation
{
	// Token: 0x02000484 RID: 1156
	public class SectMenu : StaticDetailedFilterMenuBase<NormalInformationDisplayData>
	{
		// Token: 0x170006ED RID: 1773
		// (get) Token: 0x06004112 RID: 16658 RVA: 0x00200C77 File Offset: 0x001FEE77
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x06004113 RID: 16659 RVA: 0x00200C7C File Offset: 0x001FEE7C
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateDirect(InformationType.Instance[1].Name)
			};
		}

		// Token: 0x06004114 RID: 16660 RVA: 0x00200CB4 File Offset: 0x001FEEB4
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> configs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			this._organizationList.Clear();
			foreach (OrganizationItem organization in ((IEnumerable<OrganizationItem>)Organization.Instance))
			{
				bool flag = !organization.IsSect;
				if (!flag)
				{
					this._organizationList.Add(organization.TemplateId);
					configs.Add(new DetailFilterMultiSelectDropdownItemConfig
					{
						IconPath = null,
						Text = StringKey.CreateDirect(organization.Name)
					});
				}
			}
			return configs;
		}

		// Token: 0x06004115 RID: 16661 RVA: 0x00200D64 File Offset: 0x001FEF64
		public override bool IsDataMatch(NormalInformationDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			InformationInfoItem info = Utils.GetInformationInfo(data);
			return selectedIndices.Any((int index) => info.Oraganization == this._organizationList[index]);
		}

		// Token: 0x170006EE RID: 1774
		// (get) Token: 0x06004116 RID: 16662 RVA: 0x00200DA1 File Offset: 0x001FEFA1
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x04002E4C RID: 11852
		private readonly List<sbyte> _organizationList = new List<sbyte>();
	}
}
