using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.CharacterLocationDisplayData
{
	// Token: 0x02000E66 RID: 3686
	public class CharacterLocationDisplayDataSectMenu : DetailedFilterMenuLogic<CharacterLocationDisplayData>
	{
		// Token: 0x1700134A RID: 4938
		// (get) Token: 0x0600AC54 RID: 44116 RVA: 0x004EE1B5 File Offset: 0x004EC3B5
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x1700134B RID: 4939
		// (get) Token: 0x0600AC55 RID: 44117 RVA: 0x004EE1B8 File Offset: 0x004EC3B8
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x0600AC56 RID: 44118 RVA: 0x004EE1BB File Offset: 0x004EC3BB
		public override StringKey GetMenuBarLabel()
		{
			return StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_MapBlock_Type_1);
		}

		// Token: 0x1700134C RID: 4940
		// (get) Token: 0x0600AC57 RID: 44119 RVA: 0x004EE1C7 File Offset: 0x004EC3C7
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 1));
			}
		}

		// Token: 0x0600AC58 RID: 44120 RVA: 0x004EE1D8 File Offset: 0x004EC3D8
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Shaolin.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Emei.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Baihua.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Wudang.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Yuanshan.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Shixiang.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Ranshan.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Xuannv.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Zhujian.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Kongsang.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Jingang.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Wuxian.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Jieqing.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Fulong.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Xuehou.Name))
			};
		}

		// Token: 0x0600AC59 RID: 44121 RVA: 0x004EE380 File Offset: 0x004EC580
		public override bool IsDataMatch(CharacterLocationDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any(delegate(int index)
			{
				EMapBlockSubType subType = data.BlockData.GetConfig().SubType;
				switch (index)
				{
				case 0:
					if (subType != EMapBlockSubType.ShaolinPai)
					{
						goto IL_C9;
					}
					break;
				case 1:
					if (subType != EMapBlockSubType.EmeiPai)
					{
						goto IL_C9;
					}
					break;
				case 2:
					if (subType != EMapBlockSubType.BaihuaGu)
					{
						goto IL_C9;
					}
					break;
				case 3:
					if (subType != EMapBlockSubType.WudangPai)
					{
						goto IL_C9;
					}
					break;
				case 4:
					if (subType != EMapBlockSubType.YuanshanPai)
					{
						goto IL_C9;
					}
					break;
				case 5:
					if (subType != EMapBlockSubType.ShixiangMen)
					{
						goto IL_C9;
					}
					break;
				case 6:
					if (subType != EMapBlockSubType.RanshanPai)
					{
						goto IL_C9;
					}
					break;
				case 7:
					if (subType != EMapBlockSubType.XuannvPai)
					{
						goto IL_C9;
					}
					break;
				case 8:
					if (subType != EMapBlockSubType.ZhujianShanzhuang)
					{
						goto IL_C9;
					}
					break;
				case 9:
					if (subType != EMapBlockSubType.KongsangPai)
					{
						goto IL_C9;
					}
					break;
				case 10:
					if (subType != EMapBlockSubType.JingangZong)
					{
						goto IL_C9;
					}
					break;
				case 11:
					if (subType != EMapBlockSubType.WuxianJiao)
					{
						goto IL_C9;
					}
					break;
				case 12:
					if (subType != EMapBlockSubType.JieqingMen)
					{
						goto IL_C9;
					}
					break;
				case 13:
					if (subType != EMapBlockSubType.FulongTan)
					{
						goto IL_C9;
					}
					break;
				case 14:
					if (subType != EMapBlockSubType.XuehouJiao)
					{
						goto IL_C9;
					}
					break;
				default:
					goto IL_C9;
				}
				return true;
				IL_C9:
				return false;
			});
		}
	}
}
