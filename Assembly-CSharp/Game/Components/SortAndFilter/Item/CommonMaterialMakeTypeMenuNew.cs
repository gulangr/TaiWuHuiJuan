using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DA0 RID: 3488
	public abstract class CommonMaterialMakeTypeMenuNew : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x17001259 RID: 4697
		// (get) Token: 0x0600A93A RID: 43322 RVA: 0x004E4DE8 File Offset: 0x004E2FE8
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x0600A93B RID: 43323 RVA: 0x004E4DEC File Offset: 0x004E2FEC
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600A93C RID: 43324 RVA: 0x004E4E08 File Offset: 0x004E3008
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_MaterialMakeTyep_0),
				new FilterDropdownItemConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_MaterialMakeTyep_1)
			};
		}

		// Token: 0x0600A93D RID: 43325 RVA: 0x004E4E4C File Offset: 0x004E304C
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
				foreach (int index in selectedIndices)
				{
					EMaterialMakeType makeType = (EMaterialMakeType)index;
					EMaterialMakeType ematerialMakeType = makeType;
					EMaterialMakeType ematerialMakeType2 = ematerialMakeType;
					if (ematerialMakeType2 != EMaterialMakeType.Make)
					{
						if (ematerialMakeType2 == EMaterialMakeType.Refine)
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
