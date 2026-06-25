using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D8E RID: 3470
	public class FoodTypeSecondaryMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x17001251 RID: 4689
		// (get) Token: 0x0600A8FB RID: 43259 RVA: 0x004E243B File Offset: 0x004E063B
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001252 RID: 4690
		// (get) Token: 0x0600A8FC RID: 43260 RVA: 0x004E243E File Offset: 0x004E063E
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A8FD RID: 43261 RVA: 0x004E2444 File Offset: 0x004E0644
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600A8FE RID: 43262 RVA: 0x004E2460 File Offset: 0x004E0660
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Sub_Food_0")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Sub_Food_1")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Sub_Food_2")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Sub_Food_3"))
			};
		}

		// Token: 0x0600A8FF RID: 43263 RVA: 0x004E24D0 File Offset: 0x004E06D0
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			foreach (int selectedSubType in selectedIndices)
			{
				switch ((sbyte)selectedSubType)
				{
				case 0:
				{
					bool flag = ItemFilterCommon.IsItemMatchItemSubType(data, 700);
					if (flag)
					{
						return true;
					}
					break;
				}
				case 1:
				{
					bool flag2 = ItemFilterCommon.IsItemMatchItemSubType(data, 701);
					if (flag2)
					{
						return true;
					}
					break;
				}
				case 2:
				{
					bool flag3 = ItemFilterCommon.IsItemMatchItemSubType(data, 900);
					if (flag3)
					{
						return true;
					}
					break;
				}
				case 3:
				{
					bool flag4 = ItemFilterCommon.IsItemMatchItemSubType(data, 901);
					if (flag4)
					{
						return true;
					}
					break;
				}
				}
			}
			return false;
		}
	}
}
