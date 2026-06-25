using System;
using System.Collections.Generic;
using UILogic.DisplayDataStructure;

namespace Game.Components.SortAndFilter.InstantNotification
{
	// Token: 0x02000DFB RID: 3579
	public class InstantNotificationSortAndFilterController : SortAndFilterController<NotificationItem>
	{
		// Token: 0x0600AAC9 RID: 43721 RVA: 0x004E9641 File Offset: 0x004E7841
		public InstantNotificationSortAndFilterController(ISortAndFilterView sortAndFilter) : base(sortAndFilter, LanguageKey.LK_HotKeyGroup_MainInterfaceFunction_TaiwuJournal)
		{
			this.SortController = new InstantNotificationSortController();
		}

		// Token: 0x0600AACA RID: 43722 RVA: 0x004E965C File Offset: 0x004E785C
		protected override IEnumerable<FilterLineBase<NotificationItem>> GenerateFilterLines()
		{
			yield return new InstantNotificationSecondaryFilterLine();
			yield break;
		}
	}
}
