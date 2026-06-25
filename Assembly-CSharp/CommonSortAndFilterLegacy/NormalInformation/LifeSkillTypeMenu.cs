using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Information;

namespace CommonSortAndFilterLegacy.NormalInformation
{
	// Token: 0x0200047C RID: 1148
	public class LifeSkillTypeMenu : StaticDetailedFilterMenuBase<NormalInformationDisplayData>
	{
		// Token: 0x170006E2 RID: 1762
		// (get) Token: 0x060040F7 RID: 16631 RVA: 0x00200A0B File Offset: 0x001FEC0B
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x060040F8 RID: 16632 RVA: 0x00200A10 File Offset: 0x001FEC10
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateDirect(InformationType.Instance[2].Name)
			};
		}

		// Token: 0x060040F9 RID: 16633 RVA: 0x00200A48 File Offset: 0x001FEC48
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return (from lifeSkillTypeConfig in LifeSkillType.Instance
			select new DetailFilterMultiSelectDropdownItemConfig
			{
				IconPath = lifeSkillTypeConfig.Icon,
				Text = lifeSkillTypeConfig.Name
			}).ToList<DetailFilterMultiSelectDropdownItemConfig>();
		}

		// Token: 0x060040FA RID: 16634 RVA: 0x00200A88 File Offset: 0x001FEC88
		public override bool IsDataMatch(NormalInformationDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			InformationInfoItem info = Utils.GetInformationInfo(data);
			return (from index in selectedIndices
			select LifeSkillType.Instance[index].TemplateId).Contains(info.LifeSkillType);
		}

		// Token: 0x170006E3 RID: 1763
		// (get) Token: 0x060040FB RID: 16635 RVA: 0x00200AD1 File Offset: 0x001FECD1
		public override int Id
		{
			get
			{
				return 0;
			}
		}
	}
}
