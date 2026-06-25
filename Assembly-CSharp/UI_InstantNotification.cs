using System;
using System.Collections.Generic;
using CommonSortAndFilterLegacy;
using CommonSortAndFilterLegacy.InstantNotification;
using Config;
using FrameWork;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.TaiwuEvent.EventLog;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UILogic.DisplayDataStructure;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000387 RID: 903
public class UI_InstantNotification : UIBase
{
	// Token: 0x060035A9 RID: 13737 RVA: 0x001AFB14 File Offset: 0x001ADD14
	public override void OnInit(ArgumentBox argsBox)
	{
		CToggleGroupObsolete toggleGroup = this._toggleGroup;
		if (toggleGroup != null)
		{
			toggleGroup.Set(0, true, false);
		}
		UIElement element = this.Element;
		element.OnActive = (Action)Delegate.Combine(element.OnActive, new Action(this.RefreshPage));
	}

	// Token: 0x060035AA RID: 13738 RVA: 0x001AFB54 File Offset: 0x001ADD54
	private void Awake()
	{
		this._toggleGroup = base.CGet<CToggleGroupObsolete>("ToggleGroup");
		this._toggleGroup.InitPreOnToggle(-1);
		this._toggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnToggleChanged);
		CommonSortAndFilter commonSortAndFilter = base.CGet<CommonSortAndFilter>("CommonSortAndFilter");
		this._instantNotificationSortAndFilterController = new InstantNotificationSortAndFilterController(commonSortAndFilter);
		this._instantNotificationSortAndFilterController.Init(new Action(this.OnInstantNotificationFilterChanged), "UI_InstantNotification");
		this._groupedScrollView = base.CGet<GroupedInfinityScroll>("GroupedScrollView");
		this._groupedScrollView.Init();
		this._groupedScrollView.OnItemRender = new Action<int, int, Refers>(this.OnItemRender);
		this._groupedScrollView.OnGroupTitleRender = new Action<int, Refers>(this.OnGroupTitleRender);
		this._groupedScrollView.UpdateData(this._scrollDataList, true);
	}

	// Token: 0x060035AB RID: 13739 RVA: 0x001AFC2A File Offset: 0x001ADE2A
	public override void QuickHide()
	{
		AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
		base.QuickHide();
	}

	// Token: 0x060035AC RID: 13740 RVA: 0x001AFC46 File Offset: 0x001ADE46
	private void OnEnable()
	{
		TaiwuEventDomainMethod.AsyncCall.GetEventLogData(this, delegate(int offset, RawDataPool dataPool)
		{
			EventLogData eventLogData = null;
			Serializer.Deserialize(dataPool, offset, ref eventLogData);
			base.CGet<EventLogHelper>("EventLog").Init(eventLogData);
		});
	}

	// Token: 0x060035AD RID: 13741 RVA: 0x001AFC5C File Offset: 0x001ADE5C
	private void OnDisable()
	{
		this._dataList = null;
	}

	// Token: 0x060035AE RID: 13742 RVA: 0x001AFC68 File Offset: 0x001ADE68
	protected override void OnClick(Transform btn)
	{
		string name = btn.name;
		string a = name;
		if (a == "CommonButtonClose")
		{
			this.QuickHide();
		}
	}

	// Token: 0x060035AF RID: 13743 RVA: 0x001AFC98 File Offset: 0x001ADE98
	private void OnToggleChanged(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		bool flag = togNew == null;
		if (!flag)
		{
			GameObject instantNotification = base.CGet<GameObject>("InstantNotification");
			EventLogHelper eventLog = base.CGet<EventLogHelper>("EventLog");
			bool flag2 = togNew.Key == 0;
			if (flag2)
			{
				instantNotification.SetActive(true);
				eventLog.transform.localPosition = eventLog.transform.localPosition.SetX(3000f);
			}
			else
			{
				instantNotification.SetActive(false);
				eventLog.transform.localPosition = eventLog.transform.localPosition.SetX(0f);
			}
		}
	}

	// Token: 0x060035B0 RID: 13744 RVA: 0x001AFD30 File Offset: 0x001ADF30
	private void OnGroupTitleRender(int groupIndex, Refers refers)
	{
		UI_InstantNotification.RecordGroup group = this._scrollGroupList[groupIndex];
		refers.CGet<TextMeshProUGUI>("TitleName").text = group.GroupTitleText;
	}

	// Token: 0x060035B1 RID: 13745 RVA: 0x001AFD64 File Offset: 0x001ADF64
	private void OnItemRender(int groupIndex, int dataIndex, Refers refers)
	{
		UI_InstantNotification.RecordGroup group = this._scrollGroupList[groupIndex];
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

	// Token: 0x060035B2 RID: 13746 RVA: 0x001AFE77 File Offset: 0x001AE077
	private string GetReadIcon(bool isRead)
	{
		return isRead ? "ui_taiwuevent_recall_icon_open_0" : "ui_taiwuevent_recall_icon_open_1";
	}

	// Token: 0x060035B3 RID: 13747 RVA: 0x001AFE88 File Offset: 0x001AE088
	private void OnInstantNotificationFilterChanged()
	{
		this.UpdateDisplayDataList();
	}

	// Token: 0x060035B4 RID: 13748 RVA: 0x001AFE94 File Offset: 0x001AE094
	private void UpdateScrollGroupList(List<NotificationItem> notificationItems)
	{
		this._scrollGroupList.Clear();
		int groupId = 0;
		using (List<NotificationItem>.Enumerator enumerator = notificationItems.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				NotificationItem notificationItem = enumerator.Current;
				int groupIndex = this._scrollGroupList.FindIndex((UI_InstantNotification.RecordGroup g) => g.Date == notificationItem.Date);
				bool flag = groupIndex < 0;
				UI_InstantNotification.RecordGroup group;
				if (flag)
				{
					group = new UI_InstantNotification.RecordGroup(groupId++, notificationItem.Date, new List<NotificationItem>());
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

	// Token: 0x060035B5 RID: 13749 RVA: 0x001AFF74 File Offset: 0x001AE174
	private void UpdateScrollDataList(List<UI_InstantNotification.RecordGroup> itemGroups)
	{
		this._scrollDataList.Clear();
		foreach (UI_InstantNotification.RecordGroup itemGroup in itemGroups)
		{
			GroupedInfinityScroll.GroupItem groupItem = new GroupedInfinityScroll.GroupItem(itemGroup.GroupId, itemGroup.NotificationItems.Count);
			this._scrollDataList.Add(groupItem);
		}
	}

	// Token: 0x060035B6 RID: 13750 RVA: 0x001AFFF4 File Offset: 0x001AE1F4
	private void UpdateDisplayDataList()
	{
		this.UpdateScrollGroupList(this._instantNotificationSortAndFilterController.OutputDataList);
		this.UpdateScrollDataList(this._scrollGroupList);
		this._groupedScrollView.UpdateData(this._scrollDataList, false);
		int targetIndex = this._instantNotificationSortAndFilterController.OutputDataList.FindIndex((NotificationItem n) => n.EnterClick);
		bool flag = targetIndex >= 0;
		if (flag)
		{
			this._groupedScrollView.LoopScroll.RefillCells(targetIndex, false);
		}
	}

	// Token: 0x060035B7 RID: 13751 RVA: 0x001B0082 File Offset: 0x001AE282
	private void RefreshPage()
	{
		this._dataList = SingletonObject.getInstance<DisplayTriggerModel>().RenderedNotificationList;
		this._instantNotificationSortAndFilterController.SetDataList(this._dataList, true);
		this.Element.ShowAfterRefresh();
	}

	// Token: 0x040026F5 RID: 9973
	private List<NotificationItem> _dataList;

	// Token: 0x040026F6 RID: 9974
	private readonly List<GroupedInfinityScroll.GroupItem> _scrollDataList = new List<GroupedInfinityScroll.GroupItem>();

	// Token: 0x040026F7 RID: 9975
	private readonly List<UI_InstantNotification.RecordGroup> _scrollGroupList = new List<UI_InstantNotification.RecordGroup>();

	// Token: 0x040026F8 RID: 9976
	private CToggleGroupObsolete _toggleGroup;

	// Token: 0x040026F9 RID: 9977
	private InstantNotificationSortAndFilterController _instantNotificationSortAndFilterController;

	// Token: 0x040026FA RID: 9978
	private GroupedInfinityScroll _groupedScrollView;

	// Token: 0x0200179F RID: 6047
	private struct RecordGroup
	{
		// Token: 0x0600D498 RID: 54424 RVA: 0x005B6D90 File Offset: 0x005B4F90
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

		// Token: 0x0400AC25 RID: 44069
		public readonly int GroupId;

		// Token: 0x0400AC26 RID: 44070
		public readonly int Date;

		// Token: 0x0400AC27 RID: 44071
		public readonly string GroupTitleText;

		// Token: 0x0400AC28 RID: 44072
		public readonly List<NotificationItem> NotificationItems;
	}
}
