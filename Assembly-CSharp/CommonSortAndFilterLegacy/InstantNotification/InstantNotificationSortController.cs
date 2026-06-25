using System;
using System.Collections.Generic;
using UILogic.DisplayDataStructure;

namespace CommonSortAndFilterLegacy.InstantNotification
{
	// Token: 0x02000567 RID: 1383
	public class InstantNotificationSortController : CommonSortController<NotificationItem>
	{
		// Token: 0x06004463 RID: 17507 RVA: 0x00209A52 File Offset: 0x00207C52
		public override void Sort(List<NotificationItem> dataList, SortStateData sortData, Action actionAfterSort)
		{
			if (actionAfterSort != null)
			{
				actionAfterSort();
			}
		}

		// Token: 0x06004464 RID: 17508 RVA: 0x00209A64 File Offset: 0x00207C64
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
