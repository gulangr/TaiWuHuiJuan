using System;
using System.Collections.Generic;
using CommonSortAndFilterLegacy;
using CommonSortAndFilterLegacy.Item;
using FrameWork;
using Game.Views;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000337 RID: 823
public class ItemScrollViewForCommonTableRow : BaseItemScrollView2
{
	// Token: 0x17000546 RID: 1350
	// (get) Token: 0x06002FE3 RID: 12259 RVA: 0x00176895 File Offset: 0x00174A95
	public override ItemSortAndFilterController SortAndFilterController
	{
		get
		{
			return this._itemSortAndFilterController;
		}
	}

	// Token: 0x17000547 RID: 1351
	// (get) Token: 0x06002FE4 RID: 12260 RVA: 0x0017689D File Offset: 0x00174A9D
	public override CommonTableHead TableHead
	{
		get
		{
			return this._commonTableHead;
		}
	}

	// Token: 0x17000548 RID: 1352
	// (get) Token: 0x06002FE5 RID: 12261 RVA: 0x001768A5 File Offset: 0x00174AA5
	public override RectTransform ViewportRectTransform
	{
		get
		{
			return this._itemScroll.Scroll.Viewport;
		}
	}

	// Token: 0x17000549 RID: 1353
	// (get) Token: 0x06002FE6 RID: 12262 RVA: 0x001768B7 File Offset: 0x00174AB7
	public override List<ItemDisplayData> OutputItemList
	{
		get
		{
			return this.SortAndFilterController.OutputDataList;
		}
	}

	// Token: 0x1700054A RID: 1354
	// (get) Token: 0x06002FE7 RID: 12263 RVA: 0x001768C4 File Offset: 0x00174AC4
	public InfinityScrollLegacy InfinityScroll
	{
		get
		{
			return this._itemScroll;
		}
	}

	// Token: 0x06002FE8 RID: 12264 RVA: 0x001768CC File Offset: 0x00174ACC
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
			this._itemScroll = base.GetComponent<InfinityScrollLegacy>();
			this._itemScroll.OnItemRender = new Action<int, Refers>(this.OnRenderItem);
			this._itemScroll.OnItemHide = new Action<Refers>(this.OnItemHide);
			this._inited = true;
		}
	}

	// Token: 0x06002FE9 RID: 12265 RVA: 0x00176984 File Offset: 0x00174B84
	public void SetItemList(ref List<ItemDisplayData> itemList, bool reset = false, string listTag = null, Action<ItemDisplayData, CommonTableRowForItem> onRenderItem = null, Func<ItemDisplayData, CommonTableRowForItem, int, bool> customRender = null, bool enableBasicSort = true)
	{
		if (reset)
		{
			this._customRender = customRender;
			this._onRenderItem = onRenderItem;
			this._listTag = listTag;
			this._itemSortAndFilterController.Init(new Action(this.OnDataListChanged), listTag);
		}
		this._itemSortAndFilterController.SetDataList(itemList, enableBasicSort);
	}

	// Token: 0x06002FEA RID: 12266 RVA: 0x001769D9 File Offset: 0x00174BD9
	public void SetCharId(int charId)
	{
		this._charId = charId;
	}

	// Token: 0x06002FEB RID: 12267 RVA: 0x001769E3 File Offset: 0x00174BE3
	public void ScrollToTop()
	{
		this._itemScroll.ScrollTo(0, 0.3f);
	}

	// Token: 0x06002FEC RID: 12268 RVA: 0x001769F8 File Offset: 0x00174BF8
	public override void ReRender()
	{
		this._itemScroll.ReRender();
	}

	// Token: 0x06002FED RID: 12269 RVA: 0x00176A07 File Offset: 0x00174C07
	public override void SetItemList(ref List<ItemDisplayData> itemList)
	{
		this.SetItemList(ref itemList, false, null, null, null, true);
	}

	// Token: 0x06002FEE RID: 12270 RVA: 0x00176A17 File Offset: 0x00174C17
	public void ReRenderChild(int index)
	{
		this._itemScroll.RefreshCell(index);
	}

	// Token: 0x06002FEF RID: 12271 RVA: 0x00176A28 File Offset: 0x00174C28
	public void ReRenderChild(ItemKey key)
	{
		int index = this.OutputItemList.FindIndex((ItemDisplayData data) => data.ContainsItemKey(key));
		CommonTableRowForItem itemView = this.FindActiveItem(key, false);
		bool flag = index >= 0 && null != itemView;
		if (flag)
		{
			this.OnRenderItem(index, itemView);
		}
	}

	// Token: 0x06002FF0 RID: 12272 RVA: 0x00176A88 File Offset: 0x00174C88
	public void ReRenderChild(ItemDisplayData itemDisplayData)
	{
		int index = this.OutputItemList.IndexOf(itemDisplayData);
		CommonTableRowForItem itemView = this.FindActiveItem(itemDisplayData, false);
		bool flag = index >= 0 && null != itemView;
		if (flag)
		{
			this.OnRenderItem(index, itemView);
		}
	}

	// Token: 0x06002FF1 RID: 12273 RVA: 0x00176AC9 File Offset: 0x00174CC9
	public int FindItemIndex(ItemDisplayData itemDisplayData)
	{
		return this.OutputItemList.IndexOf(itemDisplayData);
	}

	// Token: 0x06002FF2 RID: 12274 RVA: 0x00176AD8 File Offset: 0x00174CD8
	public CommonTableRowForItem FindActiveItem(ItemKey key, bool realShowing = false)
	{
		int index = this.OutputItemList.FindIndex((ItemDisplayData data) => data.ContainsItemKey(key));
		return this.GetShowingItem(index, realShowing);
	}

	// Token: 0x06002FF3 RID: 12275 RVA: 0x00176B1C File Offset: 0x00174D1C
	public CommonTableRowForItem FindActiveItem(ItemDisplayData itemDisplayData, bool realShowing = false)
	{
		int index = this.FindItemIndex(itemDisplayData);
		return this.GetShowingItem(index, realShowing);
	}

	// Token: 0x06002FF4 RID: 12276 RVA: 0x00176B40 File Offset: 0x00174D40
	private CommonTableRowForItem GetShowingItem(int index, bool realShowing = false)
	{
		CommonTableRowForItem itemView = (index >= 0) ? (this._itemScroll.GetActiveCell(index) as CommonTableRowForItem) : null;
		bool flag = itemView && realShowing;
		if (flag)
		{
			Rect itemRect = CommonUtils.RectTransToScreenPos(itemView.RectTransform, UIManager.Instance.UiCamera);
			Rect scrollRect = CommonUtils.RectTransToScreenPos(this._itemScroll.Scroll.Viewport, UIManager.Instance.UiCamera);
			bool isOverlap = itemRect.Overlaps(scrollRect);
			bool flag2 = !isOverlap;
			if (flag2)
			{
				return null;
			}
		}
		return itemView;
	}

	// Token: 0x06002FF5 RID: 12277 RVA: 0x00176BCC File Offset: 0x00174DCC
	public override void SetItemToSelectCountMode(int index, CommonTableRowForItem itemView, Action<int> onConfirmSetCount, Action onCancelSetCount, int initSelectCount = 0, int limitCount = 0, int minCount = 1, string limitTip = null, bool keepSelectedOnHide = false, Action<int> onCountChange = null, Transform itemViewOriginParent = null, int itemViewOriginSibling = -1)
	{
		CScrollRectLegacy scrollRect = this._itemScroll.GetComponent<CScrollRectLegacy>();
		ItemDisplayData itemData = this.OutputItemList[index];
		RectTransform itemRectTrans = itemView.GetComponent<RectTransform>();
		int maxCount = (limitCount > 0) ? Mathf.Min(limitCount, itemData.Amount) : itemData.Amount;
		bool flag = ItemTemplateHelper.IsTianJieFuLu(itemData.Key.ItemType, itemData.Key.TemplateId);
		if (flag)
		{
			initSelectCount = ItemTemplateHelper.GetTianJieFuLuCountUnit();
			minCount = ItemTemplateHelper.GetTianJieFuLuCountUnit();
		}
		else
		{
			bool flag2 = initSelectCount == 0;
			if (flag2)
			{
				initSelectCount = maxCount;
			}
		}
		minCount = Mathf.Clamp(minCount, 1, maxCount);
		initSelectCount = Mathf.Clamp(initSelectCount, minCount, maxCount);
		bool isMiscResource = ItemTemplateHelper.IsMiscResource(itemData.Key.ItemType, itemData.Key.TemplateId);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("MinCount", minCount);
		argBox.Set("MaxCount", maxCount);
		argBox.Set("InitCount", initSelectCount);
		argBox.Set("LimitCount", limitCount);
		bool flag3 = limitCount >= itemData.Amount;
		if (flag3)
		{
			limitTip = string.Empty;
		}
		argBox.Set("LimitTip", limitTip);
		int changeValue = ItemTemplateHelper.GetItemCountUnit(itemData.Key.ItemType, itemData.Key.TemplateId);
		argBox.Set("ChangeValue", changeValue);
		Vector2 followOffset = Vector2.zero;
		argBox.SetObject("FollowOffset", followOffset);
		argBox.SetObject("OnValueChanged", onCountChange);
		argBox.SetObject("OnConfirmSetCount", onConfirmSetCount);
		argBox.SetObject("OnCancelSetCount", onCancelSetCount);
		argBox.SetObject("ItemRectTrans", itemRectTrans);
		argBox.Set("ZeroValid", false);
		UIElement.SetSelectCount.SetOnInitArgs(argBox);
		Transform originParent = itemRectTrans.parent;
		UIManager.Instance.ShowUI(UIElement.SetSelectCount, true);
		UIElement setSelectCount = UIElement.SetSelectCount;
		setSelectCount.OnShowed = (Action)Delegate.Combine(setSelectCount.OnShowed, new Action(delegate()
		{
			scrollRect.SetScrollEnable(false);
			itemRectTrans.SetParent(UIElement.SetSelectCount.UiBase.transform);
			this.SetClickEvent(itemView, delegate
			{
				GEvent.OnEvent(UiEvents.OnConfirmSetSelectCount, null);
			});
		}));
		UIElement setSelectCount2 = UIElement.SetSelectCount;
		setSelectCount2.OnHide = (Action)Delegate.Combine(setSelectCount2.OnHide, new Action(delegate()
		{
			scrollRect.SetScrollEnable(true);
			CScrollRectLegacy scroll = originParent.GetComponentInParent<CScrollRectLegacy>(true);
			Refers refers = scroll.GetComponent<Refers>();
			bool keepSelectedOnHide2 = keepSelectedOnHide;
			if (keepSelectedOnHide2)
			{
				RectTransform focusItemMask = refers.CGet<RectTransform>("FocusItemMask");
				itemRectTrans.SetParent(focusItemMask);
			}
			else
			{
				itemRectTrans.SetParent(scroll.Content);
			}
			itemView.GetComponentInParent<ItemScrollViewForCommonTableRow>(true).ReRenderChild(this.OutputItemList[index]);
		}));
	}

	// Token: 0x06002FF6 RID: 12278 RVA: 0x00176E3C File Offset: 0x0017503C
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

	// Token: 0x06002FF7 RID: 12279 RVA: 0x00176E74 File Offset: 0x00175074
	public void SetItemToPopupMenuMode(ItemKey key, List<ViewPopupMenu.BtnData> btnList, Action onCancel)
	{
		int index = this.OutputItemList.FindIndex((ItemDisplayData data) => data.ContainsItemKey(key));
		CScrollRectLegacy scrollRect = this._itemScroll.GetComponent<CScrollRectLegacy>();
		CommonTableRowForItem itemView = this._itemScroll.GetActiveCell(index).GetComponent<CommonTableRowForItem>();
		RectTransform itemRectTrans = itemView.GetComponent<RectTransform>();
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.SetObject("BtnInfo", btnList);
		argBox.SetObject("ScreenPos", UIManager.Instance.UiCamera.WorldToScreenPoint(itemRectTrans.position));
		argBox.SetObject("ItemSize", itemRectTrans.sizeDelta);
		argBox.SetObject("OnCancel", onCancel);
		UIElement popupMenu = UIElement.PopupMenu;
		popupMenu.OnShowed = (Action)Delegate.Combine(popupMenu.OnShowed, new Action(delegate()
		{
			scrollRect.SetScrollEnable(false);
		}));
		UIElement popupMenu2 = UIElement.PopupMenu;
		popupMenu2.OnHide = (Action)Delegate.Combine(popupMenu2.OnHide, new Action(delegate()
		{
			scrollRect.SetScrollEnable(true);
		}));
		UIElement.PopupMenu.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.PopupMenu, true);
	}

	// Token: 0x06002FF8 RID: 12280 RVA: 0x00176F9D File Offset: 0x0017519D
	private void OnDataListChanged()
	{
		this.UpdateUI();
	}

	// Token: 0x06002FF9 RID: 12281 RVA: 0x00176FA8 File Offset: 0x001751A8
	public void Refresh()
	{
		SortStateData sortData = this._itemSortAndFilterController.SortAndFilterState.SortData;
		this._itemSortAndFilterController.SetSortState(sortData);
		this.UpdateUI();
	}

	// Token: 0x06002FFA RID: 12282 RVA: 0x00176FDB File Offset: 0x001751DB
	private void UpdateUI()
	{
		this._itemScroll.UpdateData(this.OutputItemList.Count);
	}

	// Token: 0x06002FFB RID: 12283 RVA: 0x00176FF8 File Offset: 0x001751F8
	private void OnRenderItem(int index, Refers itemRefers)
	{
		ItemDisplayData itemData = this.OutputItemList.CheckIndex(index) ? this.OutputItemList[index] : null;
		CommonTableRowForItem itemView = itemRefers as CommonTableRowForItem;
		bool flag = this._customRender != null && this._customRender(itemData, itemView, index);
		if (!flag)
		{
			bool flag2 = itemData == null || itemView == null;
			if (flag2)
			{
				string format = "Either index {0} out of range 0..{1}, or refers {2} is not non-null CommonTableRowForItem, skip rendering. _customRender is {3}, invoke result: {4}";
				object[] array = new object[5];
				array[0] = index;
				array[1] = this.OutputItemList.Count;
				array[2] = itemRefers;
				array[3] = ((this._customRender == null) ? "null" : this._customRender.ToString());
				int num = 4;
				Func<ItemDisplayData, CommonTableRowForItem, int, bool> customRender = this._customRender;
				array[num] = ((customRender != null) ? new bool?(customRender(itemData, itemView, index)) : null);
				Debug.LogWarning(string.Format(format, array));
			}
			else
			{
				CEmptyGraphic itemGraphic = itemView.GetComponent<CEmptyGraphic>();
				itemView.SetData(itemData);
				itemView.GetComponent<TooltipInvoker>().NeedRefresh = true;
				itemView.GetComponent<TooltipInvoker>().Refresh(true, -1);
				itemView.SetSelectState(false);
				bool flag3 = null != itemGraphic;
				if (flag3)
				{
					itemGraphic.enabled = false;
					SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
					{
						itemGraphic.enabled = true;
					});
				}
				bool flag4 = this.checkQuestStatus && !this.IsItemLockedByTask(itemData);
				if (flag4)
				{
					itemView.SetLocked(false);
				}
				Action<ItemDisplayData, CommonTableRowForItem> onRenderItem = this._onRenderItem;
				if (onRenderItem != null)
				{
					onRenderItem(itemData, itemView);
				}
				bool flag5 = this.IsItemLockedByTask(itemData);
				if (flag5)
				{
					itemView.SetLocked(true);
				}
			}
		}
	}

	// Token: 0x06002FFC RID: 12284 RVA: 0x001771A8 File Offset: 0x001753A8
	private void OnItemHide(Refers itemRefers)
	{
		CommonTableRowForItem itemView = itemRefers as CommonTableRowForItem;
	}

	// Token: 0x06002FFD RID: 12285 RVA: 0x001771C0 File Offset: 0x001753C0
	public override void HandleClickItem(ItemDisplayData itemData, CommonTableRowForItem itemView, Action<CommonTableRowForItem> onClick)
	{
		Action action = delegate()
		{
			CommonTableRowForItem targetItemView = this.FindActiveItem(itemData, false);
			onClick(targetItemView);
			this.InfinityScroll.Scroll.OnScrollEnd = null;
		};
		int duration = 0;
		Rect itemRect = CommonUtils.RectTransToScreenPos(itemView.RectTransform, UIManager.Instance.UiCamera);
		Rect scrollRect = CommonUtils.RectTransToScreenPos(this.ViewportRectTransform, UIManager.Instance.UiCamera);
		bool flag = !scrollRect.ContainsWithBorder(itemRect.min);
		if (flag)
		{
			this.InfinityScroll.Scroll.OnScrollEnd = action;
			Vector3 localPos = this.ViewportRectTransform.InverseTransformPoint(itemView.RectTransform.position);
			float yOffset = Mathf.Abs(localPos.y) + itemView.RectTransform.rect.height - this.ViewportRectTransform.rect.height;
			Vector2 targetPos = this.InfinityScroll.Scroll.Content.anchoredPosition + new Vector2(0f, yOffset);
			this.InfinityScroll.Scroll.ScrollTo(targetPos, (float)duration);
		}
		else
		{
			bool flag2 = !scrollRect.ContainsWithBorder(itemRect.max);
			if (flag2)
			{
				bool flag3 = this.InfinityScroll.Scroll.ScrollTo(itemView.RectTransform, (float)duration, default(Vector2));
				if (flag3)
				{
					this.InfinityScroll.Scroll.OnScrollEnd = action;
				}
				else
				{
					this.InfinityScroll.Scroll.OnScrollEnd = null;
					action();
				}
			}
			else
			{
				action();
			}
		}
	}

	// Token: 0x06002FFE RID: 12286 RVA: 0x0017735C File Offset: 0x0017555C
	public void SetToggleIsOn(int lineId, int toggleIndex)
	{
		this._itemSortAndFilterController.SetToggleIsOn(lineId, toggleIndex);
	}

	// Token: 0x06002FFF RID: 12287 RVA: 0x0017736D File Offset: 0x0017556D
	public override CommonTableRowForItem FindItemViewByItem(ItemKey key)
	{
		throw new NotImplementedException();
	}

	// Token: 0x040022CA RID: 8906
	private ItemSortAndFilterController _itemSortAndFilterController;

	// Token: 0x040022CB RID: 8907
	private CommonTableHead _commonTableHead;

	// Token: 0x040022CC RID: 8908
	private InfinityScrollLegacy _itemScroll;

	// Token: 0x040022CD RID: 8909
	private Action<ItemDisplayData, CommonTableRowForItem> _onRenderItem;

	// Token: 0x040022CE RID: 8910
	private Func<ItemDisplayData, CommonTableRowForItem, int, bool> _customRender = null;

	// Token: 0x040022CF RID: 8911
	private bool _inited = false;

	// Token: 0x040022D0 RID: 8912
	private int _charId = -1;
}
