using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.CharacterDisplayDataForMapBlock
{
	// Token: 0x02000E71 RID: 3697
	public class CharacterSectMenu : DetailedFilterMenuLogic<CharacterDisplayData>
	{
		// Token: 0x0600AC90 RID: 44176 RVA: 0x004EEAF4 File Offset: 0x004ECCF4
		public CharacterSectMenu(CharacterDisplayDataSortAndFilterController controller)
		{
			this._controller = controller;
		}

		// Token: 0x17001361 RID: 4961
		// (get) Token: 0x0600AC91 RID: 44177 RVA: 0x004EEB04 File Offset: 0x004ECD04
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(1, 0));
			}
		}

		// Token: 0x17001362 RID: 4962
		// (get) Token: 0x0600AC92 RID: 44178 RVA: 0x004EEB12 File Offset: 0x004ECD12
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001363 RID: 4963
		// (get) Token: 0x0600AC93 RID: 44179 RVA: 0x004EEB15 File Offset: 0x004ECD15
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600AC94 RID: 44180 RVA: 0x004EEB18 File Offset: 0x004ECD18
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_SettlementInfo_Remake_Sect;
		}

		// Token: 0x0600AC95 RID: 44181 RVA: 0x004EEB24 File Offset: 0x004ECD24
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Shaolin.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Emei.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Baihua.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Wudang.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Yuanshan.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Shixiang.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Ranshan.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Xuannv.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Zhujian.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Kongsang.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Jingang.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Wuxian.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Jieqing.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Fulong.Name
				},
				new FilterDropdownItemConfig
				{
					Text = Organization.DefValue.Xuehou.Name
				}
			};
		}

		// Token: 0x0600AC96 RID: 44182 RVA: 0x004EED70 File Offset: 0x004ECF70
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
					result = (((data2 != null) ? new sbyte?(data2.OrgInfo.OrgTemplateId) : null) == 1);
					break;
				}
				case 1:
				{
					CharacterDisplayData data3 = data;
					result = (((data3 != null) ? new sbyte?(data3.OrgInfo.OrgTemplateId) : null) == 2);
					break;
				}
				case 2:
				{
					CharacterDisplayData data4 = data;
					result = (((data4 != null) ? new sbyte?(data4.OrgInfo.OrgTemplateId) : null) == 3);
					break;
				}
				case 3:
				{
					CharacterDisplayData data5 = data;
					result = (((data5 != null) ? new sbyte?(data5.OrgInfo.OrgTemplateId) : null) == 4);
					break;
				}
				case 4:
				{
					CharacterDisplayData data6 = data;
					result = (((data6 != null) ? new sbyte?(data6.OrgInfo.OrgTemplateId) : null) == 5);
					break;
				}
				case 5:
				{
					CharacterDisplayData data7 = data;
					result = (((data7 != null) ? new sbyte?(data7.OrgInfo.OrgTemplateId) : null) == 6);
					break;
				}
				case 6:
				{
					CharacterDisplayData data8 = data;
					result = (((data8 != null) ? new sbyte?(data8.OrgInfo.OrgTemplateId) : null) == 7);
					break;
				}
				case 7:
				{
					CharacterDisplayData data9 = data;
					result = (((data9 != null) ? new sbyte?(data9.OrgInfo.OrgTemplateId) : null) == 8);
					break;
				}
				case 8:
				{
					CharacterDisplayData data10 = data;
					result = (((data10 != null) ? new sbyte?(data10.OrgInfo.OrgTemplateId) : null) == 9);
					break;
				}
				case 9:
				{
					CharacterDisplayData data11 = data;
					result = (((data11 != null) ? new sbyte?(data11.OrgInfo.OrgTemplateId) : null) == 10);
					break;
				}
				case 10:
				{
					CharacterDisplayData data12 = data;
					result = (((data12 != null) ? new sbyte?(data12.OrgInfo.OrgTemplateId) : null) == 11);
					break;
				}
				case 11:
				{
					CharacterDisplayData data13 = data;
					result = (((data13 != null) ? new sbyte?(data13.OrgInfo.OrgTemplateId) : null) == 12);
					break;
				}
				case 12:
				{
					CharacterDisplayData data14 = data;
					result = (((data14 != null) ? new sbyte?(data14.OrgInfo.OrgTemplateId) : null) == 13);
					break;
				}
				case 13:
				{
					CharacterDisplayData data15 = data;
					result = (((data15 != null) ? new sbyte?(data15.OrgInfo.OrgTemplateId) : null) == 14);
					break;
				}
				case 14:
				{
					CharacterDisplayData data16 = data;
					result = (((data16 != null) ? new sbyte?(data16.OrgInfo.OrgTemplateId) : null) == 15);
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

		// Token: 0x040085B4 RID: 34228
		private readonly CharacterDisplayDataSortAndFilterController _controller;
	}
}
