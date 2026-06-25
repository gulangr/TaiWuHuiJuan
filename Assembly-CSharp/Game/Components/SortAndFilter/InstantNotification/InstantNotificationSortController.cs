using System;
using System.Collections.Generic;
using UILogic.DisplayDataStructure;

namespace Game.Components.SortAndFilter.InstantNotification
{
	// Token: 0x02000DFC RID: 3580
	public class InstantNotificationSortController : SortController<NotificationItem>
	{
		// Token: 0x0600AACB RID: 43723 RVA: 0x004E966C File Offset: 0x004E786C
		public override Comparison<NotificationItem> GenerateComparer(SortStateData sortData)
		{
			return (NotificationItem x, NotificationItem y) => 0;
		}

		// Token: 0x0600AACC RID: 43724 RVA: 0x004E9690 File Offset: 0x004E7890
		public override SortUiConfig GenerateConfig()
		{
			return new SortUiConfig
			{
				SortIds = new List<short>(),
				SortNameIndexList = new List<int>(),
				DefaultSortState = default(SortUiState),
				DefaultSortIds = new List<short>()
			};
		}
	}
}
