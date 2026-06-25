using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;
using UICommon.Character.Elements;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000490 RID: 1168
	public class BookFilterLine : FilterToggleGroupLine<ItemDisplayData>
	{
		// Token: 0x170006FE RID: 1790
		// (get) Token: 0x0600413E RID: 16702 RVA: 0x0020110A File Offset: 0x001FF30A
		public override int Id
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x170006FF RID: 1791
		// (get) Token: 0x0600413F RID: 16703 RVA: 0x0020110D File Offset: 0x001FF30D
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x06004140 RID: 16704 RVA: 0x00201110 File Offset: 0x001FF310
		public override bool IsDataMatch(ItemDisplayData data, LineState lineState)
		{
			ToggleKey toggleGroupState = lineState.ToggleGroupState;
			bool isAll = toggleGroupState.IsAll;
			bool result;
			if (isAll)
			{
				result = true;
			}
			else
			{
				bool flag = !ItemFilterCommon.IsSkillBook(data.Key.ItemType);
				if (flag)
				{
					result = false;
				}
				else
				{
					int index = toggleGroupState.Index;
					int num = index;
					if (num != 0)
					{
						if (num == 1)
						{
							bool flag2 = !ItemFilterCommon.IsItemMatchItemSubType(data, 1000);
							if (flag2)
							{
								return false;
							}
						}
					}
					else
					{
						bool flag3 = !ItemFilterCommon.IsItemMatchItemSubType(data, 1001);
						if (flag3)
						{
							return false;
						}
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06004141 RID: 16705 RVA: 0x002011A0 File Offset: 0x001FF3A0
		protected override List<FilterToggleConfig> GetFilterToggleConfigs()
		{
			ToggleTransitionIconSpriteNames defaultIconNames = ToggleTransitionIconSpriteNames.Default();
			return new List<FilterToggleConfig>
			{
				new FilterToggleConfig(defaultIconNames, StringKey.CreateKey(string.Format("LK_ItemSubType_{0}", 1001))),
				new FilterToggleConfig(defaultIconNames, StringKey.CreateKey(string.Format("LK_ItemSubType_{0}", 1000)))
			};
		}

		// Token: 0x06004142 RID: 16706 RVA: 0x0020120C File Offset: 0x001FF40C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(3))
			};
		}
	}
}
