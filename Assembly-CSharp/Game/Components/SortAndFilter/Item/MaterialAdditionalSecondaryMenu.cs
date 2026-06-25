using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DE6 RID: 3558
	public class MaterialAdditionalSecondaryMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x170012B7 RID: 4791
		// (get) Token: 0x0600AA6C RID: 43628 RVA: 0x004E823F File Offset: 0x004E643F
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170012B8 RID: 4792
		// (get) Token: 0x0600AA6D RID: 43629 RVA: 0x004E8242 File Offset: 0x004E6442
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600AA6E RID: 43630 RVA: 0x004E8248 File Offset: 0x004E6448
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600AA6F RID: 43631 RVA: 0x004E8264 File Offset: 0x004E6464
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Material_Additional_1),
				new FilterDropdownItemConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Material_Additional_2)
			};
		}

		// Token: 0x0600AA70 RID: 43632 RVA: 0x004E82A8 File Offset: 0x004E64A8
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			bool flag = data.Key.ItemType != 5;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				MaterialItem materialConfig = Material.Instance[data.Key.TemplateId];
				foreach (int selectedIndex in selectedIndices)
				{
					EMaterialAdditionalSubFilterKeys ematerialAdditionalSubFilterKeys = (EMaterialAdditionalSubFilterKeys)selectedIndex;
					EMaterialAdditionalSubFilterKeys ematerialAdditionalSubFilterKeys2 = ematerialAdditionalSubFilterKeys;
					if (ematerialAdditionalSubFilterKeys2 != EMaterialAdditionalSubFilterKeys.MakerInput)
					{
						if (ematerialAdditionalSubFilterKeys2 == EMaterialAdditionalSubFilterKeys.Refine)
						{
							bool flag2 = materialConfig.RefiningEffect != -1;
							if (flag2)
							{
								return true;
							}
						}
					}
					else
					{
						List<short> craftableItemTypes = materialConfig.CraftableItemTypes;
						bool flag3 = craftableItemTypes != null && craftableItemTypes.Count > 0;
						if (flag3)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}
	}
}
