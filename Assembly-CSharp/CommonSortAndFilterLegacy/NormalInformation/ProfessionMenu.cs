using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Information;

namespace CommonSortAndFilterLegacy.NormalInformation
{
	// Token: 0x02000481 RID: 1153
	public class ProfessionMenu : StaticDetailedFilterMenuBase<NormalInformationDisplayData>
	{
		// Token: 0x170006E8 RID: 1768
		// (get) Token: 0x06004106 RID: 16646 RVA: 0x00200B57 File Offset: 0x001FED57
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x06004107 RID: 16647 RVA: 0x00200B5C File Offset: 0x001FED5C
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateDirect(InformationType.Instance[6].Name)
			};
		}

		// Token: 0x06004108 RID: 16648 RVA: 0x00200B94 File Offset: 0x001FED94
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return (from professionConfig in Profession.Instance
			select new DetailFilterMultiSelectDropdownItemConfig
			{
				IconPath = null,
				Text = professionConfig.Name
			}).ToList<DetailFilterMultiSelectDropdownItemConfig>();
		}

		// Token: 0x06004109 RID: 16649 RVA: 0x00200BD4 File Offset: 0x001FEDD4
		public override bool IsDataMatch(NormalInformationDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			InformationInfoItem info = Utils.GetInformationInfo(data);
			return (from index in selectedIndices
			select Profession.Instance[index].TemplateId).Contains((int)info.Profession);
		}

		// Token: 0x170006E9 RID: 1769
		// (get) Token: 0x0600410A RID: 16650 RVA: 0x00200C1D File Offset: 0x001FEE1D
		public override int Id
		{
			get
			{
				return 0;
			}
		}
	}
}
