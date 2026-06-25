using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork.UISystem.Components;
using GameData.Domains.Character;

namespace CommonSortAndFilterLegacy.CharacterTable
{
	// Token: 0x02000593 RID: 1427
	public class FeastMenu : StaticDetailedFilterMenuBase<CharacterTableSortAndFilterData>
	{
		// Token: 0x17000867 RID: 2151
		// (get) Token: 0x06004512 RID: 17682 RVA: 0x0020B767 File Offset: 0x00209967
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x06004513 RID: 17683 RVA: 0x0020B76C File Offset: 0x0020996C
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateKey(LanguageKey.LK_UI_Following_SubTitle_List)
			};
		}

		// Token: 0x06004514 RID: 17684 RVA: 0x0020B798 File Offset: 0x00209998
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return new List<DetailFilterMultiSelectDropdownItemConfig>
			{
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = StringKey.CreateKey(LanguageKey.LK_Villager)
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = StringKey.CreateKey(LanguageKey.LK_Guest)
				}
			};
		}

		// Token: 0x06004515 RID: 17685 RVA: 0x0020B804 File Offset: 0x00209A04
		public override bool IsDataMatch(CharacterTableSortAndFilterData data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any((int index) => this.IsOrgMatch(data, index));
		}

		// Token: 0x06004516 RID: 17686 RVA: 0x0020B83C File Offset: 0x00209A3C
		private bool IsOrgMatch(CharacterTableSortAndFilterData data, int index)
		{
			OrganizationInfo orgInfo = data.Data.GetOrg(92);
			return orgInfo.OrgTemplateId == 16 == (index == 0);
		}

		// Token: 0x17000868 RID: 2152
		// (get) Token: 0x06004517 RID: 17687 RVA: 0x0020B86C File Offset: 0x00209A6C
		public override int Id
		{
			get
			{
				return 11;
			}
		}
	}
}
