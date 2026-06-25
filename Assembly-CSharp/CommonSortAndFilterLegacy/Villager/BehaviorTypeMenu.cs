using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy.Villager
{
	// Token: 0x02000461 RID: 1121
	public class BehaviorTypeMenu<T> : StaticDetailedFilterMenuBase<T> where T : IVillagerSortAndFilterData
	{
		// Token: 0x170006A4 RID: 1700
		// (get) Token: 0x06004074 RID: 16500 RVA: 0x001FF83A File Offset: 0x001FDA3A
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x06004075 RID: 16501 RVA: 0x001FF840 File Offset: 0x001FDA40
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Villager_1)
			};
		}

		// Token: 0x06004076 RID: 16502 RVA: 0x001FF86C File Offset: 0x001FDA6C
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return (from behaviorTypeItem in BehaviorType.Instance
			select new DetailFilterMultiSelectDropdownItemConfig
			{
				IconPath = behaviorTypeItem.Icon,
				Text = StringKey.CreateDirect(behaviorTypeItem.Name)
			}).ToList<DetailFilterMultiSelectDropdownItemConfig>();
		}

		// Token: 0x06004077 RID: 16503 RVA: 0x001FF8AC File Offset: 0x001FDAAC
		public override bool IsDataMatch(T data, IReadOnlyCollection<int> selectedIndices)
		{
			sbyte dataBehaviorType = data.BehaviorType;
			return selectedIndices.Any((int index) => BehaviorType.Instance[index].TemplateId == (short)dataBehaviorType);
		}

		// Token: 0x170006A5 RID: 1701
		// (get) Token: 0x06004078 RID: 16504 RVA: 0x001FF8E9 File Offset: 0x001FDAE9
		public override int Id
		{
			get
			{
				return 1;
			}
		}
	}
}
