using System;
using System.Collections.Generic;

namespace CommonSortAndFilterLegacy.CharacterTable
{
	// Token: 0x0200057E RID: 1406
	public class CharacterTableSortController : CommonSortController<CharacterTableSortAndFilterData>
	{
		// Token: 0x060044BA RID: 17594 RVA: 0x0020A6E5 File Offset: 0x002088E5
		public override void Sort(List<CharacterTableSortAndFilterData> dataList, SortStateData sortData, Action actionAfterSort)
		{
			if (actionAfterSort != null)
			{
				actionAfterSort();
			}
		}

		// Token: 0x060044BB RID: 17595 RVA: 0x0020A6F4 File Offset: 0x002088F4
		public override CommonSortUiConfig GenerateConfig()
		{
			return new CommonSortUiConfig
			{
				SortIds = new List<short>(),
				SortNameIndexList = new List<int>(),
				DefaultSortState = null,
				DefaultSortIds = new List<short>()
			};
		}
	}
}
