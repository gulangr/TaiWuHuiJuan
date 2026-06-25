using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;
using UICommon.Character.Elements;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004F5 RID: 1269
	public class FoodFilterLine : FilterToggleGroupLine<ItemDisplayData>
	{
		// Token: 0x170007A8 RID: 1960
		// (get) Token: 0x060042C1 RID: 17089 RVA: 0x00204D39 File Offset: 0x00202F39
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x170007A9 RID: 1961
		// (get) Token: 0x060042C2 RID: 17090 RVA: 0x00204D3C File Offset: 0x00202F3C
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x060042C3 RID: 17091 RVA: 0x00204D40 File Offset: 0x00202F40
		protected override List<FilterToggleConfig> GetFilterToggleConfigs()
		{
			ToggleTransitionIconSpriteNames defaultIconNames = ToggleTransitionIconSpriteNames.Default();
			return new List<FilterToggleConfig>
			{
				new FilterToggleConfig(defaultIconNames, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Food_0),
				new FilterToggleConfig(defaultIconNames, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Food_1),
				new FilterToggleConfig(defaultIconNames, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Food_2),
				new FilterToggleConfig(defaultIconNames, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Food_3)
			};
		}

		// Token: 0x060042C4 RID: 17092 RVA: 0x00204DBC File Offset: 0x00202FBC
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(0))
			};
		}

		// Token: 0x060042C5 RID: 17093 RVA: 0x00204DE8 File Offset: 0x00202FE8
		public override bool IsDataMatch(ItemDisplayData data, LineState foodLineState)
		{
			ToggleKey foodToggleState = foodLineState.ToggleGroupState;
			bool isAll = foodToggleState.IsAll;
			bool result;
			if (isAll)
			{
				result = true;
			}
			else
			{
				bool flag = data.Key.ItemType != 7 && data.Key.ItemType != 9;
				if (flag)
				{
					result = false;
				}
				else
				{
					switch (foodToggleState.Index)
					{
					case 0:
					{
						bool flag2 = !ItemFilterCommon.IsItemMatchItemSubType(data, 700);
						if (flag2)
						{
							return false;
						}
						break;
					}
					case 1:
					{
						bool flag3 = !ItemFilterCommon.IsItemMatchItemSubType(data, 701);
						if (flag3)
						{
							return false;
						}
						break;
					}
					case 2:
					{
						bool flag4 = !ItemFilterCommon.IsItemMatchItemSubType(data, 900);
						if (flag4)
						{
							return false;
						}
						break;
					}
					case 3:
					{
						bool flag5 = !ItemFilterCommon.IsItemMatchItemSubType(data, 901);
						if (flag5)
						{
							return false;
						}
						break;
					}
					}
					result = true;
				}
			}
			return result;
		}
	}
}
