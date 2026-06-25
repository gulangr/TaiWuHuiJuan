using System;
using System.Collections.Generic;
using UILogic.DisplayDataStructure;

namespace Game.Components.SortAndFilter.InstantNotification
{
	// Token: 0x02000DF8 RID: 3576
	public class InstantNotificationSecondaryFilterLine : DetailedFilterLineLogic<NotificationItem>
	{
		// Token: 0x170012C8 RID: 4808
		// (get) Token: 0x0600AAB7 RID: 43703 RVA: 0x004E93A3 File Offset: 0x004E75A3
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x170012C9 RID: 4809
		// (get) Token: 0x0600AAB8 RID: 43704 RVA: 0x004E93A6 File Offset: 0x004E75A6
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x170012CA RID: 4810
		// (get) Token: 0x0600AAB9 RID: 43705 RVA: 0x004E93A9 File Offset: 0x004E75A9
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600AABA RID: 43706 RVA: 0x004E93AC File Offset: 0x004E75AC
		protected override IEnumerable<DetailedFilterMenuLogic<NotificationItem>> GenerateMenus()
		{
			yield return new InstantNotificationImportanceSecondaryMenu();
			yield return new InstantNotificationTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x0600AABB RID: 43707 RVA: 0x004E93BC File Offset: 0x004E75BC
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
