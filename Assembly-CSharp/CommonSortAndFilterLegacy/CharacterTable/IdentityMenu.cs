using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Character;

namespace CommonSortAndFilterLegacy.CharacterTable
{
	// Token: 0x0200058F RID: 1423
	public class IdentityMenu : StaticDetailedFilterMenuBase<CharacterTableSortAndFilterData>
	{
		// Token: 0x1700085D RID: 2141
		// (get) Token: 0x060044F5 RID: 17653 RVA: 0x0020B2B9 File Offset: 0x002094B9
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x060044F6 RID: 17654 RVA: 0x0020B2BC File Offset: 0x002094BC
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateKey(LanguageKey.LK_VillagerRole_Title)
			};
		}

		// Token: 0x060044F7 RID: 17655 RVA: 0x0020B2E8 File Offset: 0x002094E8
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> configs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			short[] members = Organization.DefValue.Taiwu.Members;
			for (int i = 0; i < 8; i++)
			{
				configs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = StringKey.CreateDirect(OrganizationMember.Instance[members[i]].GradeName)
				});
			}
			return configs;
		}

		// Token: 0x060044F8 RID: 17656 RVA: 0x0020B358 File Offset: 0x00209558
		public override bool IsDataMatch(CharacterTableSortAndFilterData data, IReadOnlyCollection<int> selectedIndices)
		{
			OrganizationInfo orgInfo = data.Data.GetOrg(92);
			return orgInfo.OrgTemplateId == 16 && selectedIndices.Contains((int)orgInfo.Grade);
		}

		// Token: 0x1700085E RID: 2142
		// (get) Token: 0x060044F9 RID: 17657 RVA: 0x0020B391 File Offset: 0x00209591
		public override int Id
		{
			get
			{
				return 7;
			}
		}
	}
}
