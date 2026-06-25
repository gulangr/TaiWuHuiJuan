using System;
using System.Collections.Generic;
using UILogic.DisplayDataStructure;

namespace CommonSortAndFilterLegacy.InstantNotification
{
	// Token: 0x02000563 RID: 1379
	public class InstantNotificationSecondaryFilterLine : SecondaryFilterLineBase<NotificationItem>
	{
		// Token: 0x17000825 RID: 2085
		// (get) Token: 0x0600444E RID: 17486 RVA: 0x002096C1 File Offset: 0x002078C1
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000826 RID: 2086
		// (get) Token: 0x0600444F RID: 17487 RVA: 0x002096C4 File Offset: 0x002078C4
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000827 RID: 2087
		// (get) Token: 0x06004450 RID: 17488 RVA: 0x002096C7 File Offset: 0x002078C7
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06004451 RID: 17489 RVA: 0x002096CA File Offset: 0x002078CA
		protected override IEnumerable<DetailedFilterMenuBase<NotificationItem>> GenerateMenus()
		{
			yield return new InstantNotificationImportanceSecondaryMenu();
			yield return new InstantNotificationTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x06004452 RID: 17490 RVA: 0x002096DC File Offset: 0x002078DC
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
