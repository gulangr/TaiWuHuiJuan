using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DC1 RID: 3521
	public abstract class MaterialCommonMakeTypeMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x1700127F RID: 4735
		// (get) Token: 0x0600A9A9 RID: 43433 RVA: 0x004E5ED1 File Offset: 0x004E40D1
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x0600A9AA RID: 43434 RVA: 0x004E5ED4 File Offset: 0x004E40D4
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_MaterialMakeTyep_0),
				new FilterDropdownItemConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_MaterialMakeTyep_1)
			};
		}

		// Token: 0x0600A9AB RID: 43435 RVA: 0x004E5F18 File Offset: 0x004E4118
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
