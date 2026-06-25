using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Information;

namespace CommonSortAndFilterLegacy.NormalInformation
{
	// Token: 0x02000479 RID: 1145
	public class AreaMenu : StaticDetailedFilterMenuBase<NormalInformationDisplayData>
	{
		// Token: 0x170006DD RID: 1757
		// (get) Token: 0x060040EB RID: 16619 RVA: 0x002008A7 File Offset: 0x001FEAA7
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x060040EC RID: 16620 RVA: 0x002008AC File Offset: 0x001FEAAC
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateDirect(InformationType.Instance[0].Name)
			};
		}

		// Token: 0x060040ED RID: 16621 RVA: 0x002008E4 File Offset: 0x001FEAE4
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

		// Token: 0x060040EE RID: 16622 RVA: 0x00200968 File Offset: 0x001FEB68
		public override bool IsDataMatch(NormalInformationDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			InformationInfoItem info = Utils.GetInformationInfo(data);
			return selectedIndices.Any((int index) => info.Oraganization == this._organizationList[index]);
		}

		// Token: 0x170006DE RID: 1758
		// (get) Token: 0x060040EF RID: 16623 RVA: 0x002009A5 File Offset: 0x001FEBA5
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x04002E3D RID: 11837
		private readonly List<sbyte> _organizationList = new List<sbyte>();
	}
}
