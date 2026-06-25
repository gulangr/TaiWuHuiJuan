using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Character;

namespace CommonSortAndFilterLegacy.CharacterTable
{
	// Token: 0x02000591 RID: 1425
	public class SectMenu : StaticDetailedFilterMenuBase<CharacterTableSortAndFilterData>
	{
		// Token: 0x17000861 RID: 2145
		// (get) Token: 0x06004502 RID: 17666 RVA: 0x0020B536 File Offset: 0x00209736
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x06004503 RID: 17667 RVA: 0x0020B53C File Offset: 0x0020973C
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateKey(LanguageKey.LK_CharacterTableFilter_Settlement_1)
			};
		}

		// Token: 0x06004504 RID: 17668 RVA: 0x0020B568 File Offset: 0x00209768
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> configs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (sbyte i = 1; i <= 15; i += 1)
			{
				configs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = StringKey.CreateDirect(Organization.Instance[i].Name)
				});
			}
			return configs;
		}

		// Token: 0x06004505 RID: 17669 RVA: 0x0020B5D0 File Offset: 0x002097D0
		public override bool IsDataMatch(CharacterTableSortAndFilterData data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any((int index) => this.IsOrgMatch(data, index));
		}

		// Token: 0x06004506 RID: 17670 RVA: 0x0020B608 File Offset: 0x00209808
		private bool IsOrgMatch(CharacterTableSortAndFilterData data, int index)
		{
			OrganizationInfo orgInfo = data.Data.GetOrg(92);
			return (int)orgInfo.OrgTemplateId == index - 1;
		}

		// Token: 0x17000862 RID: 2146
		// (get) Token: 0x06004507 RID: 17671 RVA: 0x0020B633 File Offset: 0x00209833
		public override int Id
		{
			get
			{
				return 9;
			}
		}

		// Token: 0x17000863 RID: 2147
		// (get) Token: 0x06004508 RID: 17672 RVA: 0x0020B637 File Offset: 0x00209837
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(8, 1));
			}
		}
	}
}
