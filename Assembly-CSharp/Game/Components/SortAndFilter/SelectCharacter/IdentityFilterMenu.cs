using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.SelectCharacter
{
	// Token: 0x02000CEB RID: 3307
	public class IdentityFilterMenu<TData> : SelectCharacterFilterMenuBase<TData> where TData : ISelectCharacterData
	{
		// Token: 0x17001171 RID: 4465
		// (get) Token: 0x0600A674 RID: 42612 RVA: 0x004D73F0 File Offset: 0x004D55F0
		public override int Id
		{
			get
			{
				return 11;
			}
		}

		// Token: 0x0600A675 RID: 42613 RVA: 0x004D73F4 File Offset: 0x004D55F4
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_VillagerRole_Title;
		}

		// Token: 0x0600A676 RID: 42614 RVA: 0x004D7400 File Offset: 0x004D5600
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> list = new List<FilterDropdownItemConfig>();
			short[] members = Organization.DefValue.Taiwu.Members;
			for (int i = 0; i < 8; i++)
			{
				list.Add(new FilterDropdownItemConfig
				{
					Text = StringKey.CreateDirect(OrganizationMember.Instance[members[i]].GradeName)
				});
			}
			return list;
		}

		// Token: 0x0600A677 RID: 42615 RVA: 0x004D7468 File Offset: 0x004D5668
		public override bool IsDataMatch(TData data, IReadOnlyCollection<int> selectedIndices)
		{
			CharacterDisplayDataForGeneralScrollList generalData = base.GetGeneralData(data);
			bool flag = generalData == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = generalData.OrgInfo.OrgTemplateId != 16;
				result = (!flag2 && selectedIndices.Contains((int)generalData.OrgInfo.Grade));
			}
			return result;
		}
	}
}
