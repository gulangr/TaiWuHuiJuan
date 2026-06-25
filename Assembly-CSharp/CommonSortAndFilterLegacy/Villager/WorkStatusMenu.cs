using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy.Villager
{
	// Token: 0x02000470 RID: 1136
	public class WorkStatusMenu<T> : StaticDetailedFilterMenuBase<T> where T : IVillagerSortAndFilterData
	{
		// Token: 0x170006D1 RID: 1745
		// (get) Token: 0x060040C7 RID: 16583 RVA: 0x00200286 File Offset: 0x001FE486
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x060040C8 RID: 16584 RVA: 0x0020028C File Offset: 0x001FE48C
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Villager_5)
			};
		}

		// Token: 0x060040C9 RID: 16585 RVA: 0x002002B8 File Offset: 0x001FE4B8
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> configs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (int i = 0; i < 5; i++)
			{
				configs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Villager_Third_WorkStatus_{0}", i))
				});
			}
			return configs;
		}

		// Token: 0x060040CA RID: 16586 RVA: 0x0020031C File Offset: 0x001FE51C
		public override bool IsDataMatch(T data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any((int index) => WorkStatusMenu<T>.IsOptionMatch((EWorkStatusMenuOption)index, data));
		}

		// Token: 0x060040CB RID: 16587 RVA: 0x00200350 File Offset: 0x001FE550
		private static bool IsOptionMatch(EWorkStatusMenuOption option, IVillagerSortAndFilterData data)
		{
			return data.IsWorkStatusMatched(option);
		}

		// Token: 0x170006D2 RID: 1746
		// (get) Token: 0x060040CC RID: 16588 RVA: 0x00200369 File Offset: 0x001FE569
		public override int Id
		{
			get
			{
				return 5;
			}
		}
	}
}
