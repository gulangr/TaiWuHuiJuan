using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.CharacterDisplayDataForMapBlock
{
	// Token: 0x02000E72 RID: 3698
	public class CharacterCityMenu : DetailedFilterMenuLogic<CharacterDisplayData>
	{
		// Token: 0x0600AC97 RID: 44183 RVA: 0x004EED9C File Offset: 0x004ECF9C
		public CharacterCityMenu(CharacterDisplayDataSortAndFilterController controller)
		{
			this._controller = controller;
		}

		// Token: 0x17001364 RID: 4964
		// (get) Token: 0x0600AC98 RID: 44184 RVA: 0x004EEDAC File Offset: 0x004ECFAC
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(1, 1));
			}
		}

		// Token: 0x17001365 RID: 4965
		// (get) Token: 0x0600AC99 RID: 44185 RVA: 0x004EEDBA File Offset: 0x004ECFBA
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001366 RID: 4966
		// (get) Token: 0x0600AC9A RID: 44186 RVA: 0x004EEDBD File Offset: 0x004ECFBD
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x0600AC9B RID: 44187 RVA: 0x004EEDC0 File Offset: 0x004ECFC0
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_SettlementInfo_Remake_Town;
		}

		// Token: 0x0600AC9C RID: 44188 RVA: 0x004EEDCC File Offset: 0x004ECFCC
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Jingcheng.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Chengdu.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Guizhou.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Xiangyang.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Taiyuan.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Guangzhou.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Qingzhou.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Jiangling.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Fuzhou.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Liaoyang.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Qinzhou.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Dali.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Shouchun.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Hangzhou.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Yangzhou.Name
				}
			};
		}

		// Token: 0x0600AC9D RID: 44189 RVA: 0x004EF018 File Offset: 0x004ED218
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
					result = (((data2 != null) ? new sbyte?(data2.OrgInfo.OrgTemplateId) : null) == 21);
					break;
				}
				case 1:
				{
					CharacterDisplayData data3 = data;
					result = (((data3 != null) ? new sbyte?(data3.OrgInfo.OrgTemplateId) : null) == 22);
					break;
				}
				case 2:
				{
					CharacterDisplayData data4 = data;
					result = (((data4 != null) ? new sbyte?(data4.OrgInfo.OrgTemplateId) : null) == 23);
					break;
				}
				case 3:
				{
					CharacterDisplayData data5 = data;
					result = (((data5 != null) ? new sbyte?(data5.OrgInfo.OrgTemplateId) : null) == 24);
					break;
				}
				case 4:
				{
					CharacterDisplayData data6 = data;
					result = (((data6 != null) ? new sbyte?(data6.OrgInfo.OrgTemplateId) : null) == 25);
					break;
				}
				case 5:
				{
					CharacterDisplayData data7 = data;
					result = (((data7 != null) ? new sbyte?(data7.OrgInfo.OrgTemplateId) : null) == 26);
					break;
				}
				case 6:
				{
					CharacterDisplayData data8 = data;
					result = (((data8 != null) ? new sbyte?(data8.OrgInfo.OrgTemplateId) : null) == 27);
					break;
				}
				case 7:
				{
					CharacterDisplayData data9 = data;
					result = (((data9 != null) ? new sbyte?(data9.OrgInfo.OrgTemplateId) : null) == 28);
					break;
				}
				case 8:
				{
					CharacterDisplayData data10 = data;
					result = (((data10 != null) ? new sbyte?(data10.OrgInfo.OrgTemplateId) : null) == 29);
					break;
				}
				case 9:
				{
					CharacterDisplayData data11 = data;
					result = (((data11 != null) ? new sbyte?(data11.OrgInfo.OrgTemplateId) : null) == 30);
					break;
				}
				case 10:
				{
					CharacterDisplayData data12 = data;
					result = (((data12 != null) ? new sbyte?(data12.OrgInfo.OrgTemplateId) : null) == 31);
					break;
				}
				case 11:
				{
					CharacterDisplayData data13 = data;
					result = (((data13 != null) ? new sbyte?(data13.OrgInfo.OrgTemplateId) : null) == 32);
					break;
				}
				case 12:
				{
					CharacterDisplayData data14 = data;
					result = (((data14 != null) ? new sbyte?(data14.OrgInfo.OrgTemplateId) : null) == 33);
					break;
				}
				case 13:
				{
					CharacterDisplayData data15 = data;
					result = (((data15 != null) ? new sbyte?(data15.OrgInfo.OrgTemplateId) : null) == 34);
					break;
				}
				case 14:
				{
					CharacterDisplayData data16 = data;
					result = (((data16 != null) ? new sbyte?(data16.OrgInfo.OrgTemplateId) : null) == 35);
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

		// Token: 0x040085B5 RID: 34229
		private readonly CharacterDisplayDataSortAndFilterController _controller;
	}
}
