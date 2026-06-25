using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Character;

namespace CommonSortAndFilterLegacy.CharacterTable
{
	// Token: 0x02000592 RID: 1426
	public class CityMenu : StaticDetailedFilterMenuBase<CharacterTableSortAndFilterData>
	{
		// Token: 0x17000864 RID: 2148
		// (get) Token: 0x0600450A RID: 17674 RVA: 0x0020B64E File Offset: 0x0020984E
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x0600450B RID: 17675 RVA: 0x0020B654 File Offset: 0x00209854
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateKey(LanguageKey.LK_CharacterTableFilter_Settlement_2)
			};
		}

		// Token: 0x0600450C RID: 17676 RVA: 0x0020B680 File Offset: 0x00209880
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> configs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (sbyte i = 21; i <= 35; i += 1)
			{
				configs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = StringKey.CreateDirect(Organization.Instance[i].Name)
				});
			}
			return configs;
		}

		// Token: 0x0600450D RID: 17677 RVA: 0x0020B6E8 File Offset: 0x002098E8
		public override bool IsDataMatch(CharacterTableSortAndFilterData data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any((int index) => this.IsOrgMatch(data, index));
		}

		// Token: 0x0600450E RID: 17678 RVA: 0x0020B720 File Offset: 0x00209920
		private bool IsOrgMatch(CharacterTableSortAndFilterData data, int index)
		{
			OrganizationInfo orgInfo = data.Data.GetOrg(92);
			return (int)orgInfo.OrgTemplateId == index - 21;
		}

		// Token: 0x17000865 RID: 2149
		// (get) Token: 0x0600450F RID: 17679 RVA: 0x0020B74C File Offset: 0x0020994C
		public override int Id
		{
			get
			{
				return 10;
			}
		}

		// Token: 0x17000866 RID: 2150
		// (get) Token: 0x06004510 RID: 17680 RVA: 0x0020B750 File Offset: 0x00209950
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(8, 2));
			}
		}
	}
}
