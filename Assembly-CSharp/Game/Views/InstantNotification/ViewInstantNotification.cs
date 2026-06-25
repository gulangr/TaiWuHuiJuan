using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.InstantNotification;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.TaiwuEvent.EventLog;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UILogic.DisplayDataStructure;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.InstantNotification
{
	// Token: 0x02000A17 RID: 2583
	public class ViewInstantNotification : UIBase
	{
		// Token: 0x06007E9B RID: 32411 RVA: 0x003AEA2F File Offset: 0x003ACC2F
		public override void OnInit(ArgumentBox argsBox)
		{
			CToggleGroup ctoggleGroup = this.toggleGroup;
			if (ctoggleGroup != null)
			{
				ctoggleGroup.Set(0, false);
			}
			UIElement element = this.Element;
			element.OnActive = (Action)Delegate.Combine(element.OnActive, new Action(this.RefreshPage));
		}

		// Token: 0x06007E9C RID: 32412 RVA: 0x003AEA70 File Offset: 0x003ACC70
		private void Awake()
		{
			this.toggleGroup.Init(0);
			this.groupedScrollView.Init();
			this.groupedScrollView.DoForceRebuildLayoutOnRender = true;
			this.groupedScrollView.OnItemRender = new Action<int, int, Refers>(this.OnItemRender);
			this.groupedScrollView.OnGroupTitleRender = new Action<int, Refers>(this.OnGroupTitleRender);
			this.groupedScrollView.UpdateData(this._scrollDataList, true);
			this.noContent.SetActive(this._scrollDataList.Count == 0);
			if (this._dataList == null)
			{
				this._dataList = new List<NotificationItem>();
			}
			this._instantNotificationSortAndFilterController = new InstantNotificationSortAndFilterController(this.commonSortAndFilter);
			this._instantNotificationSortAndFilterController.Init(new Action(this.UpdateDisplayDataList), "UI_InstantNotification");
		}

		// Token: 0x06007E9D RID: 32413 RVA: 0x003AEB3F File Offset: 0x003ACD3F
		public override void QuickHide()
		{
			AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
			base.QuickHide();
			this.eventLog.ClearData();
		}

		// Token: 0x06007E9E RID: 32414 RVA: 0x003AEB67 File Offset: 0x003ACD67
		private void OnEnable()
		{
			TaiwuEventDomainMethod.AsyncCall.GetEventLogData(this, delegate(int offset, RawDataPool dataPool)
			{
				EventLogData eventLogData = null;
				Serializer.Deserialize(dataPool, offset, ref eventLogData);
				EventLogData eventLogData2 = eventLogData;
				if (eventLogData2.ResultList == null)
				{
					eventLogData2.ResultList = new List<EventLogResultData>();
				}
				this.eventLog.Init(this, eventLogData);
			});
		}

		// Token: 0x06007E9F RID: 32415 RVA: 0x003AEB7D File Offset: 0x003ACD7D
		private void OnDisable()
		{
			this._dataList = null;
		}

		// Token: 0x06007EA0 RID: 32416 RVA: 0x003AEB88 File Offset: 0x003ACD88
		protected override void OnClick(Transform btn)
		{
			string name = btn.name;
			string a = name;
			if (a == "CommonButtonClose")
			{
				this.QuickHide();
			}
		}

		// Token: 0x06007EA1 RID: 32417 RVA: 0x003AEBB8 File Offset: 0x003ACDB8
		private void OnGroupTitleRender(int groupIndex, Refers refers)
		{
			ViewInstantNotification.RecordGroup group = this._scrollGroupList[groupIndex];
			refers.CGet<TextMeshProUGUI>("TitleName").text = group.GroupTitleText;
		}

		// Token: 0x06007EA2 RID: 32418 RVA: 0x003AEBEC File Offset: 0x003ACDEC
		private void OnItemRender(int groupIndex, int dataIndex, Refers refers)
		{
			ViewInstantNotification.RecordGroup group = this._scrollGroupList[groupIndex];
			NotificationItem notificationItem = group.NotificationItems[dataIndex];
			CImage icon = refers.CGet<CImage>("Icon");
			icon.SetSprite(this.GetReadIcon(notificationItem.ReadState), false, null);
			TMPTextSpriteHelper spriteHelper = refers.CGet<TMPTextSpriteHelper>("SpriteHelper");
			refers.CGet<TextMeshProUGUI>("Content").text = notificationItem.ToString();
			spriteHelper.Parse();
			PointerTrigger pointerTrigger = refers.CGet<PointerTrigger>("PointerTrigger");
			bool flag = !notificationItem.ReadState;
			if (flag)
			{
				PointerTrigger pointerTrigger2 = pointerTrigger;
				if (pointerTrigger2.EnterEvent == null)
				{
					pointerTrigger2.EnterEvent = new UnityEvent();
				}
				pointerTrigger.EnterEvent.AddListener(delegate()
				{
					notificationItem.ReadState = true;
					icon.SetSprite(this.GetReadIcon(notificationItem.ReadState), false, null);
				});
			}
			else
			{
				pointerTrigger.EnterEvent.RemoveAllListeners();
			}
			notificationItem.ReadState = true;
			notificationItem.EnterClick = false;
		}

		// Token: 0x06007EA3 RID: 32419 RVA: 0x003AECFF File Offset: 0x003ACEFF
		private string GetReadIcon(bool isRead)
		{
			return isRead ? "ui_taiwuevent_recall_icon_open_0" : "ui_taiwuevent_recall_icon_open_1";
		}

		// Token: 0x06007EA4 RID: 32420 RVA: 0x003AED10 File Offset: 0x003ACF10
		private void UpdateScrollGroupList(List<NotificationItem> notificationItems)
		{
			this._scrollGroupList.Clear();
			int groupId = 0;
			using (List<NotificationItem>.Enumerator enumerator = notificationItems.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					NotificationItem notificationItem = enumerator.Current;
					int groupIndex = this._scrollGroupList.FindIndex((ViewInstantNotification.RecordGroup g) => g.Date == notificationItem.Date);
					bool flag = groupIndex < 0;
					ViewInstantNotification.RecordGroup group;
					if (flag)
					{
						group = new ViewInstantNotification.RecordGroup(groupId++, notificationItem.Date, new List<NotificationItem>());
						this._scrollGroupList.Add(group);
					}
					else
					{
						group = this._scrollGroupList[groupIndex];
					}
					group.NotificationItems.Add(notificationItem);
				}
			}
		}

		// Token: 0x06007EA5 RID: 32421 RVA: 0x003AEDF0 File Offset: 0x003ACFF0
		private void UpdateScrollDataList(List<ViewInstantNotification.RecordGroup> itemGroups)
		{
			this._scrollDataList.Clear();
			foreach (ViewInstantNotification.RecordGroup itemGroup in itemGroups)
			{
				GroupedInfinityScroll.GroupItem groupItem = new GroupedInfinityScroll.GroupItem(itemGroup.GroupId, itemGroup.NotificationItems.Count);
				this._scrollDataList.Add(groupItem);
			}
		}

		// Token: 0x06007EA6 RID: 32422 RVA: 0x003AEE70 File Offset: 0x003AD070
		private void UpdateDisplayDataList()
		{
			List<NotificationItem> data = this._dataList.Where(this._instantNotificationSortAndFilterController.GenerateFilter()).ToList<NotificationItem>();
			this._instantNotificationSortAndFilterController.SetFilteredCount(data.Count);
			this.UpdateScrollGroupList(data);
			this.UpdateScrollDataList(this._scrollGroupList);
			this.groupedScrollView.UpdateData(this._scrollDataList, false);
			this.noContent.SetActive(this._scrollDataList.Count == 0);
			int targetIndex = this._scrollGroupList.FindIndex((ViewInstantNotification.RecordGroup n) => n.NotificationItems.Any((NotificationItem x) => x.EnterClick));
			bool flag = targetIndex >= 0;
			if (flag)
			{
				this.groupedScrollView.LoopScroll.RefillCells(targetIndex, false);
			}
			else
			{
				this.groupedScrollView.RefillCellsFromEnd();
			}
		}

		// Token: 0x06007EA7 RID: 32423 RVA: 0x003AEF45 File Offset: 0x003AD145
		private void RefreshPage()
		{
			this._dataList = (SingletonObject.getInstance<DisplayTriggerModel>().RenderedNotificationList ?? new List<NotificationItem>());
			this.UpdateDisplayDataList();
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x040060AC RID: 24748
		private List<NotificationItem> _dataList;

		// Token: 0x040060AD RID: 24749
		private readonly List<GroupedInfinityScroll.GroupItem> _scrollDataList = new List<GroupedInfinityScroll.GroupItem>();

		// Token: 0x040060AE RID: 24750
		private readonly List<ViewInstantNotification.RecordGroup> _scrollGroupList = new List<ViewInstantNotification.RecordGroup>();

		// Token: 0x040060AF RID: 24751
		private InstantNotificationSortAndFilterController _instantNotificationSortAndFilterController;

		// Token: 0x040060B0 RID: 24752
		[SerializeField]
		private CToggleGroup toggleGroup;

		// Token: 0x040060B1 RID: 24753
		[SerializeField]
		private GroupedInfinityScroll groupedScrollView;

		// Token: 0x040060B2 RID: 24754
		[SerializeField]
		private GameObject instantNotification;

		// Token: 0x040060B3 RID: 24755
		[SerializeField]
		private EventLogHelper eventLog;

		// Token: 0x040060B4 RID: 24756
		[SerializeField]
		private SortAndFilter commonSortAndFilter;

		// Token: 0x040060B5 RID: 24757
		[SerializeField]
		private GameObject noContent;

		// Token: 0x02001F9C RID: 8092
		private struct RecordGroup
		{
			// Token: 0x0600F477 RID: 62583 RVA: 0x00620108 File Offset: 0x0061E308
			public RecordGroup(int groupId, int date, List<NotificationItem> notificationItems)
			{
				this.GroupId = groupId;
				this.Date = date;
				this.NotificationItems = notificationItems;
				int year = date / 12;
				int month = date % 12;
				this.GroupTitleText = LocalStringManager.GetFormat(LanguageKey.LK_Game_Time, new object[]
				{
					year + 1,
					month + 1,
					LocalStringManager.Get(string.Format("LK_Season_{0}", TimeKit.GetSeason(date))),
					Month.Instance[month].Name
				});
			}

			// Token: 0x0400CE0D RID: 52749
			public readonly int GroupId;

			// Token: 0x0400CE0E RID: 52750
			public readonly int Date;

			// Token: 0x0400CE0F RID: 52751
			public readonly string GroupTitleText;

			// Token: 0x0400CE10 RID: 52752
			public readonly List<NotificationItem> NotificationItems;
		}
	}
}
