using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy.Villager
{
	// Token: 0x0200046B RID: 1131
	public class RoleArrangementMenu<T> : StaticDetailedFilterMenuBase<T> where T : IVillagerSortAndFilterData
	{
		// Token: 0x170006BD RID: 1725
		// (get) Token: 0x060040A4 RID: 16548 RVA: 0x001FFD80 File Offset: 0x001FDF80
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x060040A5 RID: 16549 RVA: 0x001FFD84 File Offset: 0x001FDF84
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Villager_6)
			};
		}

		// Token: 0x060040A6 RID: 16550 RVA: 0x001FFDB0 File Offset: 0x001FDFB0
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> configs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (int i = 0; i < Enum.GetValues(typeof(EArrangementMenuOption)).Length; i++)
			{
				configs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Villager_Third_RoleArrangement_{0}", i))
				});
			}
			return configs;
		}

		// Token: 0x060040A7 RID: 16551 RVA: 0x001FFE24 File Offset: 0x001FE024
		public override bool IsDataMatch(T data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any((int index) => RoleArrangementMenu<T>.IsArrangementTypeMatch(data, (EArrangementMenuOption)index));
		}

		// Token: 0x060040A8 RID: 16552 RVA: 0x001FFE58 File Offset: 0x001FE058
		private static bool IsArrangementTypeMatch(IVillagerSortAndFilterData data, EArrangementMenuOption arrangementMenuOption)
		{
			return data.IsArrangementMatched(arrangementMenuOption);
		}

		// Token: 0x170006BE RID: 1726
		// (get) Token: 0x060040A9 RID: 16553 RVA: 0x001FFE71 File Offset: 0x001FE071
		public override int Id
		{
			get
			{
				return 6;
			}
		}

		// Token: 0x170006BF RID: 1727
		// (get) Token: 0x060040AA RID: 16554 RVA: 0x001FFE74 File Offset: 0x001FE074
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(5, 2));
			}
		}
	}
}
