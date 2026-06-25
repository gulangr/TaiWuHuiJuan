using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.CharacterLocationDisplayData
{
	// Token: 0x02000E6B RID: 3691
	public class CharacterLocationDisplayDataBadMenu : DetailedFilterMenuLogic<CharacterLocationDisplayData>
	{
		// Token: 0x17001359 RID: 4953
		// (get) Token: 0x0600AC77 RID: 44151 RVA: 0x004EE855 File Offset: 0x004ECA55
		public override int Id
		{
			get
			{
				return 7;
			}
		}

		// Token: 0x1700135A RID: 4954
		// (get) Token: 0x0600AC78 RID: 44152 RVA: 0x004EE858 File Offset: 0x004ECA58
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x0600AC79 RID: 44153 RVA: 0x004EE85B File Offset: 0x004ECA5B
		public override StringKey GetMenuBarLabel()
		{
			return StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_MapBlock_Type_7);
		}

		// Token: 0x1700135B RID: 4955
		// (get) Token: 0x0600AC7A RID: 44154 RVA: 0x004EE867 File Offset: 0x004ECA67
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 7));
			}
		}

		// Token: 0x0600AC7B RID: 44155 RVA: 0x004EE878 File Offset: 0x004ECA78
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_MapBlock_Destroyed)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Wild1.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Ruin1.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Abyss.Name)),
				new FilterDropdownItemConfig(StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_MapBlock_SwordTomb)),
				new FilterDropdownItemConfig(StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_MapBlock_Loong))
			};
		}

		// Token: 0x0600AC7C RID: 44156 RVA: 0x004EE920 File Offset: 0x004ECB20
		public override bool IsDataMatch(CharacterLocationDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any(delegate(int index)
			{
				bool flag = index == 0 && data.BlockData.Destroyed;
				bool flag2 = flag;
				if (!flag2)
				{
					EMapBlockSubType subType = data.BlockData.GetConfig().SubType;
					switch (index)
					{
					case 1:
						if (subType != EMapBlockSubType.Wild)
						{
							goto IL_7A;
						}
						break;
					case 2:
						if (subType != EMapBlockSubType.Ruin)
						{
							goto IL_7A;
						}
						break;
					case 3:
						if (subType != EMapBlockSubType.DarkPool)
						{
							goto IL_7A;
						}
						break;
					case 4:
						if (subType != EMapBlockSubType.SwordTomb)
						{
							goto IL_7A;
						}
						break;
					case 5:
						if (subType != EMapBlockSubType.DLCLoong)
						{
							goto IL_7A;
						}
						break;
					default:
						goto IL_7A;
					}
					bool flag3 = true;
					goto IL_7D;
					IL_7A:
					flag3 = false;
					IL_7D:
					flag2 = flag3;
				}
				return flag2;
			});
		}
	}
}
