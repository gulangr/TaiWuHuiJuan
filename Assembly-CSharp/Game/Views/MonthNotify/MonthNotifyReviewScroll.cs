using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using TMPro;
using UILogic.DisplayDataStructure;
using UnityEngine;

namespace Game.Views.MonthNotify
{
	// Token: 0x020008BF RID: 2239
	public class MonthNotifyReviewScroll : MonoBehaviour
	{
		// Token: 0x17000C9A RID: 3226
		// (get) Token: 0x06006AAF RID: 27311 RVA: 0x00314151 File Offset: 0x00312351
		private MonthlyNotificationSortingGroups MonthlyNotificationSortingGroups
		{
			get
			{
				return SingletonObject.getInstance<MonthlyNotificationSortingGroups>();
			}
		}

		// Token: 0x17000C9B RID: 3227
		// (get) Token: 0x06006AB0 RID: 27312 RVA: 0x00314158 File Offset: 0x00312358
		private bool IsShowHidden
		{
			get
			{
				return this.toggleShowHidden.isOn;
			}
		}

		// Token: 0x06006AB1 RID: 27313 RVA: 0x00314168 File Offset: 0x00312368
		public void Init(Action<NotificationItem, ArgumentBox> action, LanguageKey title)
		{
			this._tipAction = action;
			this._title = title;
			this.titleLabel.text = LanguageKey.LK_TaiwuSummary_Format.TrFormat(title.Tr(), 0);
			this.toggleShowHidden.onValueChanged.ResetListener(new Action<bool>(this.OnClickToggleShowHidden));
			this.scroll.OnItemRender += this.OnItemRender;
		}

		// Token: 0x06006AB2 RID: 27314 RVA: 0x003141DB File Offset: 0x003123DB
		public void Set(List<NotificationItem> data)
		{
			this._data = data;
			this.Refresh();
		}

		// Token: 0x06006AB3 RID: 27315 RVA: 0x003141EC File Offset: 0x003123EC
		public void Refresh()
		{
			this._sortAndFilteredData.Clear();
			bool flag = this._data != null;
			if (flag)
			{
				foreach (NotificationItem item in this._data)
				{
					bool flag2 = this.IsNotificationItemAbleToDisplay(ViewMonthNotify.GetConfig(item).SortingGroup);
					if (flag2)
					{
						this._sortAndFilteredData.Add(item);
					}
				}
			}
			this._sortAndFilteredData.Sort(new Comparison<NotificationItem>(this.CompareNotificationItem));
			this.titleLabel.text = LanguageKey.LK_TaiwuSummary_Format.TrFormat(this._title.Tr(), this._sortAndFilteredData.Count);
			this.scroll.SetDataCount(this._sortAndFilteredData.Count);
			this.noContent.SetActive(this._sortAndFilteredData.Count == 0);
		}

		// Token: 0x06006AB4 RID: 27316 RVA: 0x003142F0 File Offset: 0x003124F0
		private void OnItemRender(int index, GameObject obj)
		{
			obj.GetComponent<MonthNotifyReviewCardItem>().Set(this._tipAction, this._sortAndFilteredData[index]);
		}

		// Token: 0x06006AB5 RID: 27317 RVA: 0x00314311 File Offset: 0x00312511
		private void OnClickToggleShowHidden(bool value)
		{
			this.Refresh();
		}

		// Token: 0x06006AB6 RID: 27318 RVA: 0x0031431C File Offset: 0x0031251C
		private int CompareNotificationItem(NotificationItem a, NotificationItem b)
		{
			MonthlyNotificationItem configA = ViewMonthNotify.GetConfig(a);
			MonthlyNotificationItem configB = ViewMonthNotify.GetConfig(b);
			bool flag = configA.SortingGroup < 0 || configB.SortingGroup < 0;
			if (flag)
			{
				bool flag2 = configA.SortingGroup < 0 && configB.SortingGroup < 0;
				if (flag2)
				{
					return -configA.Priority.CompareTo(configB.Priority);
				}
				bool flag3 = configA.SortingGroup < 0;
				if (flag3)
				{
					return 1;
				}
				bool flag4 = configB.SortingGroup < 0;
				if (flag4)
				{
					return -1;
				}
			}
			return (configA.SortingGroup == configB.SortingGroup) ? (-configA.Priority.CompareTo(configB.Priority)) : this.MonthlyNotificationSortingGroups.MonthlyNotificationSortingGroupOrder[(int)configA.SortingGroup].CompareTo(this.MonthlyNotificationSortingGroups.MonthlyNotificationSortingGroupOrder[(int)configB.SortingGroup]);
		}

		// Token: 0x06006AB7 RID: 27319 RVA: 0x0031440A File Offset: 0x0031260A
		private bool IsNotificationItemAbleToDisplay(short sortingGroup)
		{
			return sortingGroup >= 0 && (!this.MonthlyNotificationSortingGroups.Data[(int)sortingGroup].IsHidden || this.IsShowHidden);
		}

		// Token: 0x04004D1C RID: 19740
		[SerializeField]
		protected TextMeshProUGUI titleLabel;

		// Token: 0x04004D1D RID: 19741
		[SerializeField]
		protected CToggle toggleShowHidden;

		// Token: 0x04004D1E RID: 19742
		[SerializeField]
		protected InfinityScroll scroll;

		// Token: 0x04004D1F RID: 19743
		[SerializeField]
		protected GameObject noContent;

		// Token: 0x04004D20 RID: 19744
		private List<NotificationItem> _data;

		// Token: 0x04004D21 RID: 19745
		private List<NotificationItem> _sortAndFilteredData = new List<NotificationItem>();

		// Token: 0x04004D22 RID: 19746
		private Action<NotificationItem, ArgumentBox> _tipAction;

		// Token: 0x04004D23 RID: 19747
		private LanguageKey _title;
	}
}
