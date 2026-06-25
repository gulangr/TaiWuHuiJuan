using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.CharacterLocationDisplayData
{
	// Token: 0x02000E65 RID: 3685
	public class CharacterLocationDisplayDataCityMenu : DetailedFilterMenuLogic<CharacterLocationDisplayData>
	{
		// Token: 0x17001347 RID: 4935
		// (get) Token: 0x0600AC4D RID: 44109 RVA: 0x004EDFB5 File Offset: 0x004EC1B5
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17001348 RID: 4936
		// (get) Token: 0x0600AC4E RID: 44110 RVA: 0x004EDFB8 File Offset: 0x004EC1B8
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x0600AC4F RID: 44111 RVA: 0x004EDFBB File Offset: 0x004EC1BB
		public override StringKey GetMenuBarLabel()
		{
			return StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_MapBlock_Type_0);
		}

		// Token: 0x17001349 RID: 4937
		// (get) Token: 0x0600AC50 RID: 44112 RVA: 0x004EDFC7 File Offset: 0x004EC1C7
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 0));
			}
		}

		// Token: 0x0600AC51 RID: 44113 RVA: 0x004EDFD8 File Offset: 0x004EC1D8
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Jingcheng.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Chengdu.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Guizhou.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Xiangyang.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Taiyuan.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Guangzhou.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Qingzhou.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Jiangling.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Fuzhou.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Liaoyang.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Qinzhou.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Dali.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Shouchun.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Hangzhou.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Yangzhou.Name))
			};
		}

		// Token: 0x0600AC52 RID: 44114 RVA: 0x004EE180 File Offset: 0x004EC380
		public override bool IsDataMatch(CharacterLocationDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any(delegate(int index)
			{
				EMapBlockSubType subType = data.BlockData.GetConfig().SubType;
				switch (index)
				{
				case 0:
					if (subType != EMapBlockSubType.Jingcheng)
					{
						goto IL_C1;
					}
					break;
				case 1:
					if (subType != EMapBlockSubType.Chengdu)
					{
						goto IL_C1;
					}
					break;
				case 2:
					if (subType != EMapBlockSubType.Guizhou)
					{
						goto IL_C1;
					}
					break;
				case 3:
					if (subType != EMapBlockSubType.Xiangyang)
					{
						goto IL_C1;
					}
					break;
				case 4:
					if (subType != EMapBlockSubType.Taiyuan)
					{
						goto IL_C1;
					}
					break;
				case 5:
					if (subType != EMapBlockSubType.Guangzhou)
					{
						goto IL_C1;
					}
					break;
				case 6:
					if (subType != EMapBlockSubType.Qingzhou)
					{
						goto IL_C1;
					}
					break;
				case 7:
					if (subType != EMapBlockSubType.Jiangling)
					{
						goto IL_C1;
					}
					break;
				case 8:
					if (subType != EMapBlockSubType.Fuzhou)
					{
						goto IL_C1;
					}
					break;
				case 9:
					if (subType != EMapBlockSubType.Liaoyang)
					{
						goto IL_C1;
					}
					break;
				case 10:
					if (subType != EMapBlockSubType.Qinzhou)
					{
						goto IL_C1;
					}
					break;
				case 11:
					if (subType != EMapBlockSubType.Dali)
					{
						goto IL_C1;
					}
					break;
				case 12:
					if (subType != EMapBlockSubType.Shouchun)
					{
						goto IL_C1;
					}
					break;
				case 13:
					if (subType != EMapBlockSubType.Hangzhou)
					{
						goto IL_C1;
					}
					break;
				case 14:
					if (subType != EMapBlockSubType.Yangzhou)
					{
						goto IL_C1;
					}
					break;
				default:
					goto IL_C1;
				}
				return true;
				IL_C1:
				return false;
			});
		}
	}
}
