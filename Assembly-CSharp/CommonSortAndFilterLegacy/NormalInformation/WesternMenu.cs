using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Information;

namespace CommonSortAndFilterLegacy.NormalInformation
{
	// Token: 0x0200048C RID: 1164
	public class WesternMenu : StaticDetailedFilterMenuBase<NormalInformationDisplayData>
	{
		// Token: 0x170006F9 RID: 1785
		// (get) Token: 0x06004131 RID: 16689 RVA: 0x00200FCF File Offset: 0x001FF1CF
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x06004132 RID: 16690 RVA: 0x00200FD4 File Offset: 0x001FF1D4
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateDirect(InformationType.Instance[3].Name)
			};
		}

		// Token: 0x06004133 RID: 16691 RVA: 0x0020100C File Offset: 0x001FF20C
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return (from westernConfig in WesternRegion.Instance
			select new DetailFilterMultiSelectDropdownItemConfig
			{
				IconPath = null,
				Text = westernConfig.Name
			}).ToList<DetailFilterMultiSelectDropdownItemConfig>();
		}

		// Token: 0x06004134 RID: 16692 RVA: 0x0020104C File Offset: 0x001FF24C
		public override bool IsDataMatch(NormalInformationDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			InformationInfoItem info = Utils.GetInformationInfo(data);
			return (from index in selectedIndices
			select WesternRegion.Instance[index].TemplateId).Contains(info.WesternRegionId);
		}

		// Token: 0x170006FA RID: 1786
		// (get) Token: 0x06004135 RID: 16693 RVA: 0x00201095 File Offset: 0x001FF295
		public override int Id
		{
			get
			{
				return 0;
			}
		}
	}
}
