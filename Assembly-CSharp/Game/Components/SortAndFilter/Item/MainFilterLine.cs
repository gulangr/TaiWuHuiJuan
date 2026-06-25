using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D9D RID: 3485
	public class MainFilterLine : FilterToggleGroupLineLogic<ITradeableContent>
	{
		// Token: 0x17001255 RID: 4693
		// (get) Token: 0x0600A92E RID: 43310 RVA: 0x004E4BE9 File Offset: 0x004E2DE9
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17001256 RID: 4694
		// (get) Token: 0x0600A92F RID: 43311 RVA: 0x004E4BEC File Offset: 0x004E2DEC
		protected override bool IndividualLine
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600A930 RID: 43312 RVA: 0x004E4BF0 File Offset: 0x004E2DF0
		protected override List<FilterToggleConfig> GetFilterToggleConfigs()
		{
			return new List<FilterToggleConfig>
			{
				new FilterToggleConfig("ui9_btn_filter_food", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Main_0),
				new FilterToggleConfig("ui9_btn_filter_medicine", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Main_1),
				new FilterToggleConfig("ui9_btn_filter_equip", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Main_2),
				new FilterToggleConfig("ui9_btn_filter_book", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Main_3),
				new FilterToggleConfig("ui9_btn_filter_crafttool", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Main_4),
				new FilterToggleConfig("ui9_btn_filter_material", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Main_5),
				new FilterToggleConfig("ui9_btn_filter_misc", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Main_6)
			};
		}

		// Token: 0x0600A931 RID: 43313 RVA: 0x004E4CC4 File Offset: 0x004E2EC4
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}

		// Token: 0x17001257 RID: 4695
		// (get) Token: 0x0600A932 RID: 43314 RVA: 0x004E4CD7 File Offset: 0x004E2ED7
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A933 RID: 43315 RVA: 0x004E4CDC File Offset: 0x004E2EDC
		public override bool IsDataMatch(ITradeableContent data, LineState lineState)
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
