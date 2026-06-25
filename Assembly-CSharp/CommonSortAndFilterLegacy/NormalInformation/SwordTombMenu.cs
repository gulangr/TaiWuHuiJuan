using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Information;

namespace CommonSortAndFilterLegacy.NormalInformation
{
	// Token: 0x02000487 RID: 1159
	public class SwordTombMenu : StaticDetailedFilterMenuBase<NormalInformationDisplayData>
	{
		// Token: 0x170006F2 RID: 1778
		// (get) Token: 0x0600411E RID: 16670 RVA: 0x00200E07 File Offset: 0x001FF007
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x0600411F RID: 16671 RVA: 0x00200E0C File Offset: 0x001FF00C
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateDirect(InformationType.Instance[5].Name)
			};
		}

		// Token: 0x06004120 RID: 16672 RVA: 0x00200E44 File Offset: 0x001FF044
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return new List<DetailFilterMultiSelectDropdownItemConfig>();
		}

		// Token: 0x06004121 RID: 16673 RVA: 0x00200E60 File Offset: 0x001FF060
		public override bool IsDataMatch(NormalInformationDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			InformationInfoItem info = Utils.GetInformationInfo(data);
			return false;
		}

		// Token: 0x170006F3 RID: 1779
		// (get) Token: 0x06004122 RID: 16674 RVA: 0x00200E7A File Offset: 0x001FF07A
		public override int Id
		{
			get
			{
				return 0;
			}
		}
	}
}
