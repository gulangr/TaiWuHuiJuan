using System;
using System.Collections.Generic;
using UILogic.DisplayDataStructure;

namespace CommonSortAndFilterLegacy.InstantNotification
{
	// Token: 0x02000566 RID: 1382
	public class InstantNotificationSortAndFilterController : CommonSortAndFilterController<NotificationItem>
	{
		// Token: 0x1700082C RID: 2092
		// (get) Token: 0x06004460 RID: 17504 RVA: 0x00209A25 File Offset: 0x00207C25
		protected override string FilterCustomOrderKey
		{
			get
			{
				return "InstantNotificationFilterCustomOrder";
			}
		}

		// Token: 0x06004461 RID: 17505 RVA: 0x00209A2C File Offset: 0x00207C2C
		public InstantNotificationSortAndFilterController(CommonSortAndFilter sortAndFilter) : base(sortAndFilter)
		{
			this.SortController = new InstantNotificationSortController();
		}

		// Token: 0x06004462 RID: 17506 RVA: 0x00209A42 File Offset: 0x00207C42
		protected override IEnumerable<FilterLineBase<NotificationItem>> GenerateFilterLines()
		{
			yield return new InstantNotificationSecondaryFilterLine();
			yield break;
		}
	}
}
