using System;
using System.Collections.Generic;
using GameData.Domains.Taiwu.Display;

namespace CommonSortAndFilterLegacy.BonusSelect
{
	// Token: 0x020005B5 RID: 1461
	public class BonusSelectSortController : CommonSortController<SkillBreakBonusSelectableItem>
	{
		// Token: 0x060045CC RID: 17868 RVA: 0x0020D3A6 File Offset: 0x0020B5A6
		public override void Sort(List<SkillBreakBonusSelectableItem> dataList, SortStateData sortData, Action actionAfterSort)
		{
			if (actionAfterSort != null)
			{
				actionAfterSort();
			}
		}

		// Token: 0x060045CD RID: 17869 RVA: 0x0020D3B8 File Offset: 0x0020B5B8
		public override CommonSortUiConfig GenerateConfig()
		{
			return new CommonSortUiConfig
			{
				SortIds = new List<short>(),
				SortNameIndexList = new List<int>(),
				DefaultSortIds = new List<short>(),
				DefaultSortState = null,
				FilterTypeDic = new Dictionary<ValueTuple<int, int>, List<short>>()
			};
		}
	}
}
