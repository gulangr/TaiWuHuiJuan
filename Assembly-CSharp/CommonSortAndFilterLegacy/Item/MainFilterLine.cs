using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;
using UICommon.Character.Elements;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000514 RID: 1300
	public class MainFilterLine : FilterToggleGroupLine<ItemDisplayData>
	{
		// Token: 0x170007BF RID: 1983
		// (get) Token: 0x0600430C RID: 17164 RVA: 0x00205D10 File Offset: 0x00203F10
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x170007C0 RID: 1984
		// (get) Token: 0x0600430D RID: 17165 RVA: 0x00205D13 File Offset: 0x00203F13
		protected override bool IndividualLine
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600430E RID: 17166 RVA: 0x00205D18 File Offset: 0x00203F18
		protected override List<FilterToggleConfig> GetFilterToggleConfigs()
		{
			return new List<FilterToggleConfig>
			{
				new FilterToggleConfig(ToggleTransitionIconSpriteNames.CreateWithSameIcon("filter_icon_food"), LanguageKey.LK_CommonSortAndFilter_Filter_Item_Main_0),
				new FilterToggleConfig(ToggleTransitionIconSpriteNames.CreateWithSameIcon("filter_icon_medicine"), LanguageKey.LK_CommonSortAndFilter_Filter_Item_Main_1),
				new FilterToggleConfig(ToggleTransitionIconSpriteNames.CreateWithSameIcon("filter_icon_weapon"), LanguageKey.LK_CommonSortAndFilter_Filter_Item_Main_2),
				new FilterToggleConfig(ToggleTransitionIconSpriteNames.CreateWithSameIcon("filter_icon_book"), LanguageKey.LK_CommonSortAndFilter_Filter_Item_Main_3),
				new FilterToggleConfig(ToggleTransitionIconSpriteNames.CreateWithSameIcon("filter_icon_tool"), LanguageKey.LK_CommonSortAndFilter_Filter_Item_Main_4),
				new FilterToggleConfig(ToggleTransitionIconSpriteNames.CreateWithSameIcon("filter_icon_material"), LanguageKey.LK_CommonSortAndFilter_Filter_Item_Main_5),
				new FilterToggleConfig(ToggleTransitionIconSpriteNames.CreateWithSameIcon("filter_icon_misc"), LanguageKey.LK_CommonSortAndFilter_Filter_Item_Main_6)
			};
		}

		// Token: 0x0600430F RID: 17167 RVA: 0x00205E10 File Offset: 0x00204010
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}

		// Token: 0x170007C1 RID: 1985
		// (get) Token: 0x06004310 RID: 17168 RVA: 0x00205E23 File Offset: 0x00204023
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06004311 RID: 17169 RVA: 0x00205E28 File Offset: 0x00204028
		public override bool IsDataMatch(ItemDisplayData data, LineState lineState)
		{
			ToggleKey toggleState = lineState.ToggleGroupState;
			bool isAll = toggleState.IsAll;
			bool result;
			if (isAll)
			{
				result = true;
			}
			else
			{
				sbyte itemType = data.Key.ItemType;
				int index = toggleState.Index;
				if (!true)
				{
				}
				bool flag;
				switch (index)
				{
				case 0:
					flag = ItemFilterCommon.IsFood(itemType);
					break;
				case 1:
					flag = ItemFilterCommon.IsMedicine(itemType);
					break;
				case 2:
					flag = ItemFilterCommon.IsEquip(itemType);
					break;
				case 3:
					flag = ItemFilterCommon.IsSkillBook(itemType);
					break;
				case 4:
					flag = ItemFilterCommon.IsCraftTool(itemType);
					break;
				case 5:
					flag = ItemFilterCommon.IsMaterial(itemType);
					break;
				case 6:
					flag = ItemFilterCommon.IsMiscOrCricket(itemType);
					break;
				default:
					flag = true;
					break;
				}
				if (!true)
				{
				}
				result = flag;
			}
			return result;
		}
	}
}
