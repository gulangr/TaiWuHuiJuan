using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.CharacterDisplayDataForMapBlock
{
	// Token: 0x02000E70 RID: 3696
	public class CharacterOrganizationMenu : DetailedFilterMenuLogic<CharacterDisplayData>
	{
		// Token: 0x0600AC8A RID: 44170 RVA: 0x004EEA0E File Offset: 0x004ECC0E
		public CharacterOrganizationMenu(CharacterDisplayDataSortAndFilterController controller)
		{
			this._controller = controller;
		}

		// Token: 0x1700135F RID: 4959
		// (get) Token: 0x0600AC8B RID: 44171 RVA: 0x004EEA1E File Offset: 0x004ECC1E
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001360 RID: 4960
		// (get) Token: 0x0600AC8C RID: 44172 RVA: 0x004EEA21 File Offset: 0x004ECC21
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600AC8D RID: 44173 RVA: 0x004EEA24 File Offset: 0x004ECC24
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_Main_SummaryInfo_Organization;
		}

		// Token: 0x0600AC8E RID: 44174 RVA: 0x004EEA30 File Offset: 0x004ECC30
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig
				{
					Text = LanguageKey.LK_SettlementInfo_Remake_Sect
				},
				new FilterDropdownItemConfig
				{
					Text = LanguageKey.LK_SettlementInfo_Remake_Town
				},
				new FilterDropdownItemConfig
				{
					Text = LanguageKey.LK_SettlementInfo_Remake_Home
				},
				new FilterDropdownItemConfig
				{
					Text = LanguageKey.LK_Other
				}
			};
		}

		// Token: 0x0600AC8F RID: 44175 RVA: 0x004EEAC8 File Offset: 0x004ECCC8
		public override bool IsDataMatch(CharacterDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any(delegate(int index)
			{
				if (!true)
				{
				}
				bool result;
				switch (index)
				{
				case 0:
				{
					CharacterDisplayData data2 = data;
					sbyte? b = (data2 != null) ? new sbyte?(data2.OrgInfo.OrgTemplateId) : null;
					bool flag;
					if (b != null && b.GetValueOrDefault() >= 0)
					{
						OrganizationItem organizationItem = Organization.Instance[data.OrgInfo.OrgTemplateId];
						flag = (organizationItem != null && organizationItem.IsSect);
					}
					else
					{
						flag = false;
					}
					result = flag;
					break;
				}
				case 1:
				{
					CharacterDisplayData data3 = data;
					sbyte? b = (data3 != null) ? new sbyte?(data3.OrgInfo.OrgTemplateId) : null;
					bool flag2;
					if (b != null)
					{
						sbyte valueOrDefault = b.GetValueOrDefault();
						if (valueOrDefault - 21 <= 17)
						{
							flag2 = true;
							goto IL_D1;
						}
					}
					flag2 = false;
					IL_D1:
					result = flag2;
					break;
				}
				case 2:
				{
					CharacterDisplayData data4 = data;
					result = (((data4 != null) ? new sbyte?(data4.OrgInfo.OrgTemplateId) : null) == 16);
					break;
				}
				case 3:
				{
					CharacterDisplayData data5 = data;
					sbyte? b = (data5 != null) ? new sbyte?(data5.OrgInfo.OrgTemplateId) : null;
					bool flag2;
					if (b != null)
					{
						sbyte valueOrDefault = b.GetValueOrDefault();
						if (valueOrDefault - 1 <= 15 || valueOrDefault - 21 <= 17)
						{
							flag2 = false;
							goto IL_16C;
						}
					}
					flag2 = true;
					IL_16C:
					result = flag2;
					break;
				}
				default:
					result = false;
					break;
				}
				if (!true)
				{
				}
				return result;
			});
		}

		// Token: 0x040085B3 RID: 34227
		private readonly CharacterDisplayDataSortAndFilterController _controller;
	}
}
