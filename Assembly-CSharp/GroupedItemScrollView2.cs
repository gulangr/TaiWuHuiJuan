using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CommonSortAndFilterLegacy;
using CommonSortAndFilterLegacy.Item;
using FrameWork;
using Game.Views;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200022C RID: 556
public class GroupedItemScrollView2 : BaseItemScrollView2
{
	// Token: 0x17000375 RID: 885
	// (get) Token: 0x0600235D RID: 9053 RVA: 0x00104109 File Offset: 0x00102309
	public override ItemSortAndFilterController SortAndFilterController
	{
		get
		{
			return this._itemSortAndFilterController;
		}
	}

	// Token: 0x17000376 RID: 886
	// (get) Token: 0x0600235E RID: 9054 RVA: 0x00104111 File Offset: 0x00102311
	public override CommonTableHead TableHead
	{
		get
		{
			return this._commonTableHead;
		}
	}

	// Token: 0x17000377 RID: 887
	// (get) Token: 0x0600235F RID: 9055 RVA: 0x00104119 File Offset: 0x00102319
	public override RectTransform ViewportRectTransform
	{
		get
		{
			return this.ItemScrollView.GetViewport();
		}
	}

	// Token: 0x17000378 RID: 888
	// (get) Token: 0x06002360 RID: 9056 RVA: 0x00104126 File Offset: 0x00102326
	public override List<ItemDisplayData> OutputItemList
	{
		get
		{
			return this.SortAndFilterController.OutputDataList;
		}
	}

	// Token: 0x17000379 RID: 889
	// (get) Token: 0x06002361 RID: 9057 RVA: 0x00104133 File Offset: 0x00102333
	// (set) Token: 0x06002362 RID: 9058 RVA: 0x0010413B File Offset: 0x0010233B
	public GroupedInfinityScroll ItemScrollView { get; private set; }

	// Token: 0x06002363 RID: 9059 RVA: 0x00104144 File Offset: 0x00102344
	private void Awake()
	{
		this.Init(ESortAndFilterControllerType.Item, null);
	}

	// Token: 0x06002364 RID: 9060 RVA: 0x00104150 File Offset: 0x00102350
	public override void Init(ESortAndFilterControllerType sortAndFilterControllerType = ESortAndFilterControllerType.Item, short[] columnSortIds = null)
	{
		bool inited = this._inited;
		if (!inited)
		{
			this._commonTableHead = base.CGet<CommonTableHead>("CommonTableHeadForItem");
			base.RefreshTableHeadTitle();
			CommonSortAndFilter commonSortAndFilter = base.CGet<CommonSortAndFilter>("CommonSortAndFilter");
			this._itemSortAndFilterController = sortAndFilterControllerType.GetSortAndFilterController(commonSortAndFilter);
			if (columnSortIds == null)
			{
				columnSortIds = new short[]
				{
					0,
					-1,
					56,
					6,
					5,
					17
				};
			}
			this._itemSortAndFilterController.RegisterTableHead(this._commonTableHead, columnSortIds);
			this.ItemScrollView = base.GetComponent<GroupedInfinityScroll>();
			this.ItemScrollView.Init();
			this.ItemScrollView.OnGroupTitleRender = new Action<int, Refers>(this.OnRenderTitle);
			this.ItemScrollView.OnItemRender = new Action<int, int, Refers>(this.OnRenderItem);
			this._inited = true;
		}
	}

	// Token: 0x06002365 RID: 9061 RVA: 0x00104215 File Offset: 0x00102415
	public override void ReRender()
	{
		this.ItemScrollView.ReRender();
	}

	// Token: 0x06002366 RID: 9062 RVA: 0x00104224 File Offset: 0x00102424
	public override void SetItemList(ref List<ItemDisplayData> itemList)
	{
		this.SetItemList(itemList, null, null, false, null);
	}

	// Token: 0x06002367 RID: 9063 RVA: 0x00104234 File Offset: 0x00102434
	public void SetClickEvent(CommonTableRowForItem commonTableRowForItem, UnityAction onClick)
	{
		CButtonObsolete btn = commonTableRowForItem.GetComponent<CButtonObsolete>();
		btn.onClick.RemoveAllListeners();
		bool flag = onClick != null;
		if (flag)
		{
			btn.onClick.AddListener(onClick);
		}
	}

	// Token: 0x06002368 RID: 9064 RVA: 0x0010426C File Offset: 0x0010246C
	public override void SetItemToSelectCountMode(int index, CommonTableRowForItem itemView, Action<int> onConfirmSetCount, Action onCancelSetCount, int initSelectCount = 0, int limitCount = 0, int minCount = 1, string limitTip = null, bool keepSelectedOnHide = false, Action<int> onCountChange = null, Transform itemViewOriginParent = null, int itemViewOriginSibling = -1)
	{
		GroupedItemScrollView2.<>c__DisplayClass29_0 CS$<>8__locals1 = new GroupedItemScrollView2.<>c__DisplayClass29_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.onConfirmSetCount = onConfirmSetCount;
		CS$<>8__locals1.onCancelSetCount = onCancelSetCount;
		CS$<>8__locals1.itemViewOriginParent = itemViewOriginParent;
		CS$<>8__locals1.itemViewOriginSibling = itemViewOriginSibling;
		CS$<>8__locals1.itemView = itemView;
		bool flag = CS$<>8__locals1.itemViewOriginParent == null;
		if (flag)
		{
			CS$<>8__locals1.itemViewOriginParent = CS$<>8__locals1.itemView.transform.parent;
		}
		bool flag2 = CS$<>8__locals1.itemViewOriginSibling == -1;
		if (flag2)
		{
			CS$<>8__locals1.itemViewOriginSibling = CS$<>8__locals1.itemView.transform.GetSiblingIndex();
		}
		ItemDisplayData itemData = this.OutputItemList[index];
		CS$<>8__locals1.itemRectTrans = CS$<>8__locals1.itemView.GetComponent<RectTransform>();
		CS$<>8__locals1.spaceHolder = new GameObject();
		CS$<>8__locals1.spaceHolder.AddComponent<RectTransform>();
		CS$<>8__locals1.spaceHolder.name = "_spaceHolder";
		CS$<>8__locals1.spaceHolderRectTransform = CS$<>8__locals1.spaceHolder.GetComponent<RectTransform>();
		int maxCount = (limitCount > 0) ? Mathf.Min(limitCount, itemData.Amount) : itemData.Amount;
		bool flag3 = initSelectCount == 0;
		if (flag3)
		{
			initSelectCount = maxCount;
		}
		minCount = Mathf.Clamp(minCount, 1, maxCount);
		initSelectCount = Mathf.Clamp(initSelectCount, minCount, maxCount);
		bool isMiscResource = ItemTemplateHelper.IsMiscResource(itemData.RealKey.ItemType, itemData.RealKey.TemplateId);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("MinCount", minCount);
		argBox.Set("MaxCount", maxCount);
		argBox.Set("InitCount", initSelectCount);
		argBox.Set("LimitCount", limitCount);
		bool flag4 = limitCount >= itemData.Amount;
		if (flag4)
		{
			limitTip = string.Empty;
		}
		argBox.Set("LimitTip", limitTip);
		int changeValue = isMiscResource ? 10 : 1;
		argBox.Set("ChangeValue", changeValue);
		Vector2 followOffset = Vector2.zero;
		argBox.SetObject("FollowOffset", followOffset);
		argBox.SetObject("OnValueChanged", onCountChange);
		argBox.SetObject("OnConfirmSetCount", new Action<int>(CS$<>8__locals1.<SetItemToSelectCountMode>g__DelayedCallConfirm|0));
		argBox.SetObject("OnCancelSetCount", new Action(CS$<>8__locals1.<SetItemToSelectCountMode>g__DelayedCallCancel|1));
		argBox.SetObject("ItemRectTrans", CS$<>8__locals1.itemRectTrans);
		argBox.Set("ZeroValid", false);
		UIElement.SetSelectCount.SetOnInitArgs(argBox);
		CS$<>8__locals1.layout = CS$<>8__locals1.itemViewOriginParent.GetComponent<HorizontalLayoutGroup>();
		bool flag5 = CS$<>8__locals1.layout;
		if (flag5)
		{
			CS$<>8__locals1.layout.enabled = false;
		}
		UIManager.Instance.ShowUI(UIElement.SetSelectCount, true);
		UIElement setSelectCount = UIElement.SetSelectCount;
		setSelectCount.OnShowed = (Action)Delegate.Combine(setSelectCount.OnShowed, new Action(delegate()
		{
			bool flag6 = CS$<>8__locals1.itemViewOriginParent != null;
			if (flag6)
			{
				CS$<>8__locals1.spaceHolder.transform.SetParent(CS$<>8__locals1.itemViewOriginParent);
			}
			CS$<>8__locals1.spaceHolder.transform.SetSiblingIndex(CS$<>8__locals1.itemViewOriginSibling);
			CS$<>8__locals1.spaceHolderRectTransform.sizeDelta = CS$<>8__locals1.itemRectTrans.sizeDelta;
			CS$<>8__locals1.spaceHolderRectTransform.localScale = CS$<>8__locals1.itemRectTrans.localScale;
			bool flag7 = CS$<>8__locals1.layout;
			if (flag7)
			{
				CS$<>8__locals1.layout.enabled = true;
			}
			CS$<>8__locals1.<>4__this.SetClickEvent(CS$<>8__locals1.itemView, delegate
			{
				GEvent.OnEvent(UiEvents.OnConfirmSetSelectCount, null);
			});
		}));
		UIElement setSelectCount2 = UIElement.SetSelectCount;
		setSelectCount2.OnHide = (Action)Delegate.Combine(setSelectCount2.OnHide, new Action(delegate()
		{
			bool flag6 = CS$<>8__locals1.itemViewOriginParent != null;
			if (flag6)
			{
				CS$<>8__locals1.itemRectTrans.SetParent(CS$<>8__locals1.itemViewOriginParent);
				CS$<>8__locals1.itemRectTrans.SetSiblingIndex(CS$<>8__locals1.itemViewOriginSibling);
			}
			Object.Destroy(CS$<>8__locals1.spaceHolder);
			CS$<>8__locals1.itemView.GetComponentInParent<GroupedItemScrollView2>().ReRenderChild(CS$<>8__locals1.itemView.Data);
		}));
	}

	// Token: 0x06002369 RID: 9065 RVA: 0x0010454C File Offset: 0x0010274C
	public void SetItemToPopupMenuMode(ItemKey key, List<ViewPopupMenu.BtnData> btnList, Action onCancel)
	{
		CommonTableRowForItem itemView = this.FindItemViewByItem(key);
		RectTransform itemRectTrans = itemView.GetComponent<RectTransform>();
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		float screenPosX = Input.mousePosition.x;
		Vector3 screenPosY = UIManager.Instance.UiCamera.WorldToScreenPoint(itemRectTrans.position);
		Vector3 screenPos = screenPosY.SetX(screenPosX);
		argBox.SetObject("BtnInfo", btnList);
		argBox.SetObject("ScreenPos", screenPos);
		argBox.SetObject("ItemSize", itemRectTrans.sizeDelta.SetX(0f));
		argBox.SetObject("OnCancel", onCancel);
		UIElement popupMenu = UIElement.PopupMenu;
		popupMenu.OnShowed = (Action)Delegate.Combine(popupMenu.OnShowed, new Action(delegate()
		{
			this.ItemScrollView.LoopScroll.enabled = false;
			this.ItemScrollView.LoopScroll.verticalScrollbar.enabled = false;
		}));
		UIElement popupMenu2 = UIElement.PopupMenu;
		popupMenu2.OnHide = (Action)Delegate.Combine(popupMenu2.OnHide, new Action(delegate()
		{
			this.ItemScrollView.LoopScroll.enabled = true;
			this.ItemScrollView.LoopScroll.verticalScrollbar.enabled = true;
		}));
		UIElement.PopupMenu.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.PopupMenu, true);
	}

	// Token: 0x0600236A RID: 9066 RVA: 0x00104654 File Offset: 0x00102854
	public override CommonTableRowForItem FindItemViewByItem(ItemKey key)
	{
		return this._renderingItemViews.GetValueOrDefault(key);
	}

	// Token: 0x0600236B RID: 9067 RVA: 0x00104672 File Offset: 0x00102872
	public void ReRenderChild(ItemDisplayData itemViewData)
	{
		this.ItemScrollView.ReRender();
	}

	// Token: 0x0600236C RID: 9068 RVA: 0x00104684 File Offset: 0x00102884
	public void SetItemList(List<ItemDisplayData> itemList, GroupedItemScrollView2.ItemGroupGetter groupGetter = null, Action<ItemDisplayData, CommonTableRowForItem> onRenderItem = null, bool isReset = false, string listTag = null)
	{
		if (isReset)
		{
			this._onRenderItem = onRenderItem;
			bool flag = groupGetter != null;
			if (flag)
			{
				this._groupGetter = groupGetter;
			}
			bool flag2 = this._groupGetter == null;
			if (flag2)
			{
				throw new Exception("GroupGetter is null");
			}
			this._itemSortAndFilterController.Init(new Action(this.OnDataListChanged), listTag);
			this.ResetScroll();
		}
		this._itemSortAndFilterController.SetDataList(itemList, true);
	}

	// Token: 0x0600236D RID: 9069 RVA: 0x001046F7 File Offset: 0x001028F7
	public void ResetScroll()
	{
		this.ItemScrollView.UpdateData(this._scrollDataList, true);
	}

	// Token: 0x0600236E RID: 9070 RVA: 0x0010470D File Offset: 0x0010290D
	private void OnDataListChanged()
	{
		this.UpdateUI();
	}

	// Token: 0x0600236F RID: 9071 RVA: 0x00104718 File Offset: 0x00102918
	public void Refresh()
	{
		SortStateData currentSortData = this._itemSortAndFilterController.SortAndFilterState.SortData;
		this._itemSortAndFilterController.SetSortState(currentSortData);
		this.UpdateUI();
	}

	// Token: 0x06002370 RID: 9072 RVA: 0x0010474C File Offset: 0x0010294C
	private void UpdateUI()
	{
		this._itemGroups.Clear();
		bool flag = this._groupGetter != null;
		if (flag)
		{
			this._itemGroups.AddRange(this._groupGetter(this._itemSortAndFilterController.OutputDataList));
		}
		this.UpdateScrollDataList(this._itemGroups);
		this.ItemScrollView.UpdateData(this._scrollDataList, !this._inited);
		Action itemListChangedAction = this.ItemListChangedAction;
		if (itemListChangedAction != null)
		{
			itemListChangedAction();
		}
	}

	// Token: 0x06002371 RID: 9073 RVA: 0x001047D4 File Offset: 0x001029D4
	private void UpdateScrollDataList(List<GroupedItemScrollView2.ItemGroup> itemGroups)
	{
		this._scrollDataList.Clear();
		foreach (GroupedItemScrollView2.ItemGroup itemGroup in itemGroups)
		{
			GroupedInfinityScroll.GroupItem groupItem = new GroupedInfinityScroll.GroupItem(itemGroup.GroupId, itemGroup.ItemList.Count);
			this._scrollDataList.Add(groupItem);
		}
	}

	// Token: 0x06002372 RID: 9074 RVA: 0x00104854 File Offset: 0x00102A54
	public void SetCharId(int charId)
	{
		this._charId = charId;
	}

	// Token: 0x06002373 RID: 9075 RVA: 0x00104860 File Offset: 0x00102A60
	private void OnRenderTitle(int groupIndex, Refers refers)
	{
		TextMeshProUGUI titleName = refers.CGet<TextMeshProUGUI>("TitleName");
		titleName.text = this._itemGroups[groupIndex].GroupTitleText;
		bool disableStyle = this.GrayEmptyGroupTitle && this._itemGroups[groupIndex].ItemList.Count == 0;
		HSVStyleRoot disable = refers.GetComponent<HSVStyleRoot>();
		bool flag = disableStyle;
		if (flag)
		{
			disable.SetDefaultGrayAndBlack();
		}
		else
		{
			disable.SetDefault();
		}
	}

	// Token: 0x06002374 RID: 9076 RVA: 0x001048D4 File Offset: 0x00102AD4
	private void OnRenderItem(int groupIndex, int index, Refers refers)
	{
		CommonTableRowForItem itemView = (CommonTableRowForItem)refers;
		ItemDisplayData displayData = this._itemGroups[groupIndex].ItemList[index];
		this._renderingItemViews[displayData.RealKey] = itemView;
		itemView.SetData(displayData);
		itemView.GetComponent<TooltipInvoker>().NeedRefresh = true;
		itemView.GetComponent<TooltipInvoker>().Refresh(false, -1);
		CImage itemGraphic = itemView.GetComponent<CImage>();
		itemGraphic.enabled = false;
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
		{
			bool flag3 = null != itemGraphic;
			if (flag3)
			{
				itemGraphic.enabled = true;
			}
		});
		bool flag = this.checkQuestStatus && !this.IsItemLockedByTask(displayData);
		if (flag)
		{
			itemView.SetLocked(false);
		}
		Action<ItemDisplayData, CommonTableRowForItem> onRenderItem = this._onRenderItem;
		if (onRenderItem != null)
		{
			onRenderItem(displayData, itemView);
		}
		bool flag2 = this.IsItemLockedByTask(displayData);
		if (flag2)
		{
			itemView.SetLocked(true);
		}
	}

	// Token: 0x06002375 RID: 9077 RVA: 0x001049B8 File Offset: 0x00102BB8
	public override void HandleClickItem(ItemDisplayData itemData, CommonTableRowForItem itemView, Action<CommonTableRowForItem> onClick)
	{
		GroupedItemScrollView2.<>c__DisplayClass42_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.itemData = itemData;
		CS$<>8__locals1.onClick = onClick;
		Rect itemRect = CommonUtils.RectTransToScreenPos(itemView.RectTransform, UIManager.Instance.UiCamera);
		Rect scrollRect = CommonUtils.RectTransToScreenPos(this.ViewportRectTransform, UIManager.Instance.UiCamera);
		bool flag = !scrollRect.ContainsWithBorder(itemRect.min);
		if (flag)
		{
			this.ItemScrollView.LoopScroll.MoveToShowEdgeLine(itemView.transform.parent.parent.GetComponent<RectTransform>());
		}
		else
		{
			bool flag2 = !scrollRect.ContainsWithBorder(itemRect.max);
			if (flag2)
			{
				this.ItemScrollView.LoopScroll.MoveToShowEdgeLine(itemView.transform.parent.parent.GetComponent<RectTransform>());
			}
		}
		this.<HandleClickItem>g__Action|42_0(ref CS$<>8__locals1);
	}

	// Token: 0x06002379 RID: 9081 RVA: 0x00104B1C File Offset: 0x00102D1C
	[CompilerGenerated]
	private void <HandleClickItem>g__Action|42_0(ref GroupedItemScrollView2.<>c__DisplayClass42_0 A_1)
	{
		CommonTableRowForItem targetItemView = this.FindItemViewByItem(A_1.itemData.RealKey);
		A_1.onClick(targetItemView);
	}

	// Token: 0x04001B10 RID: 6928
	private ItemSortAndFilterController _itemSortAndFilterController;

	// Token: 0x04001B11 RID: 6929
	private CommonTableHead _commonTableHead;

	// Token: 0x04001B13 RID: 6931
	[Tooltip("是否需要把空白分组压暗")]
	public bool GrayEmptyGroupTitle;

	// Token: 0x04001B14 RID: 6932
	private bool _inited;

	// Token: 0x04001B15 RID: 6933
	private readonly List<GroupedItemScrollView2.ItemGroup> _itemGroups = new List<GroupedItemScrollView2.ItemGroup>();

	// Token: 0x04001B16 RID: 6934
	private readonly List<GroupedInfinityScroll.GroupItem> _scrollDataList = new List<GroupedInfinityScroll.GroupItem>();

	// Token: 0x04001B17 RID: 6935
	private GroupedItemScrollView2.ItemGroupGetter _groupGetter;

	// Token: 0x04001B18 RID: 6936
	private Action<ItemDisplayData, CommonTableRowForItem> _onRenderItem;

	// Token: 0x04001B19 RID: 6937
	private readonly Dictionary<ItemKey, CommonTableRowForItem> _renderingItemViews = new Dictionary<ItemKey, CommonTableRowForItem>();

	// Token: 0x04001B1A RID: 6938
	[HideInInspector]
	public Action ItemListChangedAction;

	// Token: 0x04001B1B RID: 6939
	private int _charId = -1;

	// Token: 0x02001507 RID: 5383
	// (Invoke) Token: 0x0600CDA2 RID: 52642
	public delegate List<GroupedItemScrollView2.ItemGroup> ItemGroupGetter(List<ItemDisplayData> itemList);

	// Token: 0x02001508 RID: 5384
	public struct ItemGroup
	{
		// Token: 0x0600CDA5 RID: 52645 RVA: 0x0059C3CF File Offset: 0x0059A5CF
		public ItemGroup(int groupId, string groupTitleText)
		{
			this.GroupId = groupId;
			this.GroupTitleText = groupTitleText;
			this.ItemList = new List<ItemDisplayData>();
		}

		// Token: 0x0400A34E RID: 41806
		public int GroupId;

		// Token: 0x0400A34F RID: 41807
		public string GroupTitleText;

		// Token: 0x0400A350 RID: 41808
		public List<ItemDisplayData> ItemList;
	}
}
