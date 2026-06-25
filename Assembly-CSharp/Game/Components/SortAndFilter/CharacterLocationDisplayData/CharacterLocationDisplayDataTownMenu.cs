using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.CharacterLocationDisplayData
{
	// Token: 0x02000E67 RID: 3687
	public class CharacterLocationDisplayDataTownMenu : DetailedFilterMenuLogic<CharacterLocationDisplayData>
	{
		// Token: 0x1700134D RID: 4941
		// (get) Token: 0x0600AC5B RID: 44123 RVA: 0x004EE3B5 File Offset: 0x004EC5B5
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x1700134E RID: 4942
		// (get) Token: 0x0600AC5C RID: 44124 RVA: 0x004EE3B8 File Offset: 0x004EC5B8
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x0600AC5D RID: 44125 RVA: 0x004EE3BB File Offset: 0x004EC5BB
		public override StringKey GetMenuBarLabel()
		{
			return StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_MapBlock_Type_2);
		}

		// Token: 0x1700134F RID: 4943
		// (get) Token: 0x0600AC5E RID: 44126 RVA: 0x004EE3C7 File Offset: 0x004EC5C7
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 2));
			}
		}

		// Token: 0x0600AC5F RID: 44127 RVA: 0x004EE3D8 File Offset: 0x004EC5D8
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Taiwucun.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Village.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Town.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Stockade.Name))
			};
		}

		// Token: 0x0600AC60 RID: 44128 RVA: 0x004EE458 File Offset: 0x004EC658
		public override bool IsDataMatch(CharacterLocationDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any(delegate(int index)
			{
				EMapBlockSubType subType = data.BlockData.GetConfig().SubType;
				switch (index)
				{
				case 0:
					if (subType != EMapBlockSubType.TaiwuCun)
					{
						goto IL_4E;
					}
					break;
				case 1:
					if (subType != EMapBlockSubType.Village)
					{
						goto IL_4E;
					}
					break;
				case 2:
					if (subType != EMapBlockSubType.Town)
					{
						goto IL_4E;
					}
					break;
				case 3:
					if (subType != EMapBlockSubType.WalledTown)
					{
						goto IL_4E;
					}
					break;
				default:
					goto IL_4E;
				}
				return true;
				IL_4E:
				return false;
			});
		}
	}
}
