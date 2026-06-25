using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.CharacterLocationDisplayData
{
	// Token: 0x02000E6A RID: 3690
	public class CharacterLocationDisplayDataWildMenu : DetailedFilterMenuLogic<CharacterLocationDisplayData>
	{
		// Token: 0x17001356 RID: 4950
		// (get) Token: 0x0600AC70 RID: 44144 RVA: 0x004EE749 File Offset: 0x004EC949
		public override int Id
		{
			get
			{
				return 6;
			}
		}

		// Token: 0x17001357 RID: 4951
		// (get) Token: 0x0600AC71 RID: 44145 RVA: 0x004EE74C File Offset: 0x004EC94C
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x0600AC72 RID: 44146 RVA: 0x004EE74F File Offset: 0x004EC94F
		public override StringKey GetMenuBarLabel()
		{
			return StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_MapBlock_Type_6);
		}

		// Token: 0x17001358 RID: 4952
		// (get) Token: 0x0600AC73 RID: 44147 RVA: 0x004EE75B File Offset: 0x004EC95B
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 6));
			}
		}

		// Token: 0x0600AC74 RID: 44148 RVA: 0x004EE76C File Offset: 0x004EC96C
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Lake1.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Jungle1.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Cave1.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Swamp1.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.TaoYuan1.Name)),
				new FilterDropdownItemConfig(StringKey.CreateDirect(MapBlock.DefValue.Valley1.Name))
			};
		}

		// Token: 0x0600AC75 RID: 44149 RVA: 0x004EE820 File Offset: 0x004ECA20
		public override bool IsDataMatch(CharacterLocationDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any(delegate(int index)
			{
				EMapBlockSubType subType = data.BlockData.GetConfig().SubType;
				switch (index)
				{
				case 0:
					if (subType != EMapBlockSubType.Lake)
					{
						goto IL_66;
					}
					break;
				case 1:
					if (subType != EMapBlockSubType.Jungle)
					{
						goto IL_66;
					}
					break;
				case 2:
					if (subType != EMapBlockSubType.Cave)
					{
						goto IL_66;
					}
					break;
				case 3:
					if (subType != EMapBlockSubType.Swamp)
					{
						goto IL_66;
					}
					break;
				case 4:
					if (subType != EMapBlockSubType.TaoYuan)
					{
						goto IL_66;
					}
					break;
				case 5:
					if (subType != EMapBlockSubType.Valley)
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
