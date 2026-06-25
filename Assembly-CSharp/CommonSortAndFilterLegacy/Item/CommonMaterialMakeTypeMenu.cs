using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000518 RID: 1304
	public abstract class CommonMaterialMakeTypeMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x170007C4 RID: 1988
		// (get) Token: 0x0600431F RID: 17183 RVA: 0x00206110 File Offset: 0x00204310
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x06004320 RID: 17184 RVA: 0x00206114 File Offset: 0x00204314
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x06004321 RID: 17185 RVA: 0x00206138 File Offset: 0x00204338
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return new List<DetailFilterMultiSelectDropdownItemConfig>
			{
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_MaterialMakeTyep_0
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_MaterialMakeTyep_1
				}
			};
		}

		// Token: 0x06004322 RID: 17186 RVA: 0x002061A4 File Offset: 0x002043A4
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			MaterialItem materialConfig = Material.Instance[data.Key.TemplateId];
			foreach (int index in selectedIndices)
			{
				EMaterialMakeType bookStatus = (EMaterialMakeType)index;
				EMaterialMakeType ematerialMakeType = bookStatus;
				EMaterialMakeType ematerialMakeType2 = ematerialMakeType;
				if (ematerialMakeType2 != EMaterialMakeType.Make)
				{
					if (ematerialMakeType2 == EMaterialMakeType.Refine)
					{
						bool flag = CommonMaterialMakeTypeMenu.IsRefine(materialConfig);
						if (flag)
						{
							return true;
						}
					}
				}
				else
				{
					bool flag2 = CommonMaterialMakeTypeMenu.IsMake(materialConfig);
					if (flag2)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06004323 RID: 17187 RVA: 0x00206244 File Offset: 0x00204444
		private static bool IsMake(MaterialItem materialConfig)
		{
			return materialConfig.CraftableItemTypes != null && materialConfig.CraftableItemTypes.Count > 0;
		}

		// Token: 0x06004324 RID: 17188 RVA: 0x00206270 File Offset: 0x00204470
		private static bool IsRefine(MaterialItem materialConfig)
		{
			return materialConfig.RefiningEffect != -1;
		}
	}
}
