using System;
using System.Collections.Generic;
using FrameWork;
using UILogic.DisplayDataStructure;
using UnityEngine;

namespace Game.Views.MonthNotify
{
	// Token: 0x020008C3 RID: 2243
	public class MonthNotifySubPageReview : MonoBehaviour
	{
		// Token: 0x17000C9E RID: 3230
		// (get) Token: 0x06006AEA RID: 27370 RVA: 0x003169F1 File Offset: 0x00314BF1
		private static MonthlyNotificationSortingGroups MonthlyNotificationSortingGroups
		{
			get
			{
				return SingletonObject.getInstance<MonthlyNotificationSortingGroups>();
			}
		}

		// Token: 0x06006AEB RID: 27371 RVA: 0x003169F8 File Offset: 0x00314BF8
		public void Init(Action<NotificationItem, ArgumentBox> action)
		{
			for (int i = 0; i < this.sections.childCount; i++)
			{
				this.sections.GetChild(i).GetComponent<MonthNotifyReviewScroll>().Init(action, MonthNotifySubPageReview.MonthlyNotificationSortingGroups.ReviewTitles[this._sectionTypes[i]]);
			}
		}

		// Token: 0x06006AEC RID: 27372 RVA: 0x00316A54 File Offset: 0x00314C54
		public void Set(Dictionary<EMonthlyNotificationSectionType, List<NotificationItem>> data)
		{
			for (int i = 0; i < this.sections.childCount; i++)
			{
				this.sections.GetChild(i).GetComponent<MonthNotifyReviewScroll>().Set(data[this._sectionTypes[i]]);
			}
		}

		// Token: 0x04004D89 RID: 19849
		[SerializeField]
		protected Transform sections;

		// Token: 0x04004D8A RID: 19850
		private readonly Dictionary<int, EMonthlyNotificationSectionType> _sectionTypes = new Dictionary<int, EMonthlyNotificationSectionType>
		{
			{
				0,
				EMonthlyNotificationSectionType.Biography
			},
			{
				1,
				EMonthlyNotificationSectionType.Gossip
			},
			{
				2,
				EMonthlyNotificationSectionType.Worldwide
			}
		};
	}
}
