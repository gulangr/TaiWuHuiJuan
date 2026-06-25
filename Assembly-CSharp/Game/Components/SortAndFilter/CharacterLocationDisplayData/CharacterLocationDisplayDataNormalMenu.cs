using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.CharacterLocationDisplayData
{
	// Token: 0x02000E69 RID: 3689
	public class CharacterLocationDisplayDataNormalMenu : DetailedFilterMenuLogic<CharacterLocationDisplayData>
	{
		// Token: 0x17001353 RID: 4947
		// (get) Token: 0x0600AC69 RID: 44137 RVA: 0x004EE599 File Offset: 0x004EC799
		public override int Id
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x17001354 RID: 4948
		// (get) Token: 0x0600AC6A RID: 44138 RVA: 0x004EE59C File Offset: 0x004EC79C
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x0600AC6B RID: 44139 RVA: 0x004EE59F File Offset: 0x004EC79F
		public override StringKey GetMenuBarLabel()
		{
			return StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_MapBlock_Type_5);
		}

		// Token: 0x17001355 RID: 4949
		// (get) Token: 0x0600AC6C RID: 44140 RVA: 0x004EE5AB File Offset: 0x004EC7AB
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 5));
			}
		}

		// Token: 0x0600AC6D RID: 44141 RVA: 0x004EE5BC File Offset: 0x004EC7BC
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Mountain1.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.BigMountain1.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Canyon1.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.BigCanyon1.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Hill1.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.BigHill1.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Field1.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.BigField1.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Woodland1.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.BigWoodland1.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.RiverBeach1.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.HeGu1.Name))
			};
		}

		// Token: 0x0600AC6E RID: 44142 RVA: 0x004EE714 File Offset: 0x004EC914
		public override bool IsDataMatch(CharacterLocationDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any(delegate(int index)
			{
				EMapBlockSubType subType = data.BlockData.GetConfig().SubType;
				switch (index)
				{
				case 0:
					if (subType != EMapBlockSubType.Mountain)
					{
						goto IL_A8;
					}
					break;
				case 1:
					if (subType != EMapBlockSubType.BigMountain)
					{
						goto IL_A8;
					}
					break;
				case 2:
					if (subType != EMapBlockSubType.Canyon)
					{
						goto IL_A8;
					}
					break;
				case 3:
					if (subType != EMapBlockSubType.BigCanyon)
					{
						goto IL_A8;
					}
					break;
				case 4:
					if (subType != EMapBlockSubType.Hill)
					{
						goto IL_A8;
					}
					break;
				case 5:
					if (subType != EMapBlockSubType.BigHill)
					{
						goto IL_A8;
					}
					break;
				case 6:
					if (subType != EMapBlockSubType.Field)
					{
						goto IL_A8;
					}
					break;
				case 7:
					if (subType != EMapBlockSubType.BigField)
					{
						goto IL_A8;
					}
					break;
				case 8:
					if (subType != EMapBlockSubType.Woodland)
					{
						goto IL_A8;
					}
					break;
				case 9:
					if (subType != EMapBlockSubType.BigWoodland)
					{
						goto IL_A8;
					}
					break;
				case 10:
					if (subType != EMapBlockSubType.RiverBeach)
					{
						goto IL_A8;
					}
					break;
				case 11:
					if (subType != EMapBlockSubType.BigRiverBeach)
					{
						goto IL_A8;
					}
					break;
				default:
					goto IL_A8;
				}
				return true;
				IL_A8:
				return false;
			});
		}
	}
}
