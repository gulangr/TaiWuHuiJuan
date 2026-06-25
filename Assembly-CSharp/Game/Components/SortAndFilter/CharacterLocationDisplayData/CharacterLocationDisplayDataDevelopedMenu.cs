using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.CharacterLocationDisplayData
{
	// Token: 0x02000E68 RID: 3688
	public class CharacterLocationDisplayDataDevelopedMenu : DetailedFilterMenuLogic<CharacterLocationDisplayData>
	{
		// Token: 0x17001350 RID: 4944
		// (get) Token: 0x0600AC62 RID: 44130 RVA: 0x004EE48D File Offset: 0x004EC68D
		public override int Id
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x17001351 RID: 4945
		// (get) Token: 0x0600AC63 RID: 44131 RVA: 0x004EE490 File Offset: 0x004EC690
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x0600AC64 RID: 44132 RVA: 0x004EE493 File Offset: 0x004EC693
		public override StringKey GetMenuBarLabel()
		{
			return StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_MapBlock_Type_4);
		}

		// Token: 0x17001352 RID: 4946
		// (get) Token: 0x0600AC65 RID: 44133 RVA: 0x004EE49F File Offset: 0x004EC69F
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 4));
			}
		}

		// Token: 0x0600AC66 RID: 44134 RVA: 0x004EE4B0 File Offset: 0x004EC6B0
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Farmland1.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Gardens1.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.StoneForest1.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.MulberryField1.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.HerbalGarden1.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.JadeMountain1.Name))
			};
		}

		// Token: 0x0600AC67 RID: 44135 RVA: 0x004EE564 File Offset: 0x004EC764
		public override bool IsDataMatch(CharacterLocationDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any(delegate(int index)
			{
				EMapBlockSubType subType = data.BlockData.GetConfig().SubType;
				switch (index)
				{
				case 0:
					if (subType != EMapBlockSubType.Farmland)
					{
						goto IL_66;
					}
					break;
				case 1:
					if (subType != EMapBlockSubType.Gardens)
					{
						goto IL_66;
					}
					break;
				case 2:
					if (subType != EMapBlockSubType.StoneForest)
					{
						goto IL_66;
					}
					break;
				case 3:
					if (subType != EMapBlockSubType.MulberryField)
					{
						goto IL_66;
					}
					break;
				case 4:
					if (subType != EMapBlockSubType.HerbalGarden)
					{
						goto IL_66;
					}
					break;
				case 5:
					if (subType != EMapBlockSubType.JadeMountain)
					{
						goto IL_66;
					}
					break;
				default:
					goto IL_66;
				}
				return true;
				IL_66:
				return false;
			});
		}
	}
}
