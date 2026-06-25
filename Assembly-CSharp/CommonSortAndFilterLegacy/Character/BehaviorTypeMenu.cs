using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy.Character
{
	// Token: 0x02000595 RID: 1429
	public class BehaviorTypeMenu<T> : StaticDetailedFilterMenuBase<T> where T : ICharacterSortAndFilterData
	{
		// Token: 0x1700086C RID: 2156
		// (get) Token: 0x06004520 RID: 17696 RVA: 0x0020BA22 File Offset: 0x00209C22
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x06004521 RID: 17697 RVA: 0x0020BA28 File Offset: 0x00209C28
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Character_1)
			};
		}

		// Token: 0x06004522 RID: 17698 RVA: 0x0020BA54 File Offset: 0x00209C54
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return (from behaviorTypeItem in BehaviorType.Instance
			select new DetailFilterMultiSelectDropdownItemConfig
			{
				IconPath = behaviorTypeItem.Icon,
				Text = StringKey.CreateDirect(behaviorTypeItem.Name)
			}).ToList<DetailFilterMultiSelectDropdownItemConfig>();
		}

		// Token: 0x06004523 RID: 17699 RVA: 0x0020BA94 File Offset: 0x00209C94
		public override bool IsDataMatch(T data, IReadOnlyCollection<int> selectedIndices)
		{
			sbyte dataBehaviorType = data.BehaviorType;
			return selectedIndices.Any((int index) => BehaviorType.Instance[index].TemplateId == (short)dataBehaviorType);
		}

		// Token: 0x1700086D RID: 2157
		// (get) Token: 0x06004524 RID: 17700 RVA: 0x0020BAD1 File Offset: 0x00209CD1
		public override int Id
		{
			get
			{
				return 1;
			}
		}
	}
}
