using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000507 RID: 1287
	public class FoodTypeSecondaryMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x170007BC RID: 1980
		// (get) Token: 0x060042F9 RID: 17145 RVA: 0x00205A3B File Offset: 0x00203C3B
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x170007BD RID: 1981
		// (get) Token: 0x060042FA RID: 17146 RVA: 0x00205A3E File Offset: 0x00203C3E
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060042FB RID: 17147 RVA: 0x00205A44 File Offset: 0x00203C44
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x060042FC RID: 17148 RVA: 0x00205A68 File Offset: 0x00203C68
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return new List<DetailFilterMultiSelectDropdownItemConfig>
			{
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Sub_Food_0")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Sub_Food_1")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Sub_Food_2")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Sub_Food_3")
				}
			};
		}

		// Token: 0x060042FD RID: 17149 RVA: 0x00205B38 File Offset: 0x00203D38
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			FoodItem food = Food.Instance[data.Key.TemplateId];
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
