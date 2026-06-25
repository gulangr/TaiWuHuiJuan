using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy.CharacterTable
{
	// Token: 0x02000589 RID: 1417
	public class BehaviorTypeMenu : StaticDetailedFilterMenuBase<CharacterTableSortAndFilterData>
	{
		// Token: 0x1700084E RID: 2126
		// (get) Token: 0x060044CA RID: 17610 RVA: 0x0020A9AF File Offset: 0x00208BAF
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x060044CB RID: 17611 RVA: 0x0020A9B4 File Offset: 0x00208BB4
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Villager_1)
			};
		}

		// Token: 0x060044CC RID: 17612 RVA: 0x0020A9E0 File Offset: 0x00208BE0
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return (from behaviorTypeItem in BehaviorType.Instance
			select new DetailFilterMultiSelectDropdownItemConfig
			{
				IconPath = behaviorTypeItem.Icon,
				Text = StringKey.CreateDirect(behaviorTypeItem.Name)
			}).ToList<DetailFilterMultiSelectDropdownItemConfig>();
		}

		// Token: 0x060044CD RID: 17613 RVA: 0x0020AA20 File Offset: 0x00208C20
		public override bool IsDataMatch(CharacterTableSortAndFilterData data, IReadOnlyCollection<int> selectedIndices)
		{
			int dataBehaviorType = data.Data.GetInt(6);
			return selectedIndices.Any((int index) => (int)BehaviorType.Instance[index].TemplateId == dataBehaviorType);
		}

		// Token: 0x1700084F RID: 2127
		// (get) Token: 0x060044CE RID: 17614 RVA: 0x0020AA5C File Offset: 0x00208C5C
		public override int Id
		{
			get
			{
				return 1;
			}
		}
	}
}
