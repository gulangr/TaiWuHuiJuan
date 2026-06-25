using System;
using System.Collections.Generic;
using FrameWork;
using Game.Views;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using UnityEngine;

// Token: 0x0200022E RID: 558
public class ItemScrollView : BaseItemScrollView
{
	// Token: 0x1700037A RID: 890
	// (get) Token: 0x0600237F RID: 9087 RVA: 0x00104D42 File Offset: 0x00102F42
	// (set) Token: 0x06002380 RID: 9088 RVA: 0x00104D4A File Offset: 0x00102F4A
	public ItemSortAndFilter MySortAndFilter { get; private set; }

	// Token: 0x1700037B RID: 891
	// (get) Token: 0x06002381 RID: 9089 RVA: 0x00104D53 File Offset: 0x00102F53
	public override ItemSortAndFilter SortAndFilter
	{
		get
		{
			return this.MySortAndFilter;
		}
	}

	// Token: 0x1700037C RID: 892
	// (get) Token: 0x06002382 RID: 9090 RVA: 0x00104D5B File Offset: 0x00102F5B
	public override RectTransform ViewportRectTransform
	{
		get
		{
			return this._itemScroll.Scroll.Viewport;
		}
	}

	// Token: 0x1700037D RID: 893
	// (get) Token: 0x06002383 RID: 9091 RVA: 0x00104D6D File Offset: 0x00102F6D
	public InfinityScrollLegacy InfinityScroll
	{
		get
		{
			return this._itemScroll;
		}
	}

	// Token: 0x06002384 RID: 9092 RVA: 0x00104D75 File Offset: 0x00102F75
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x06002385 RID: 9093 RVA: 0x00104D80 File Offset: 0x00102F80
	public override void Init()
	{
		bool inited = this._inited;
		if (!inited)
		{
			this.MySortAndFilter = base.CGet<ItemSortAndFilter>("ItemSortAndFilter");
			this.MySortAndFilter.Init();
			this.MySortAndFilter.InitIsDetailView(this.IsDetailView);
			this._itemScroll = base.GetComponent<InfinityScrollLegacy>();
			this._itemScroll.OnItemRender = new Action<int, Refers>(this.OnRenderItem);
			this._itemScroll.OnItemHide = new Action<Refers>(this.OnItemHide);
			bool flag = this.SimpleViewGap == Vector2.zero && this.SimpleViewGap == this.DetailViewGap;
			if (flag)
			{
				this.SimpleViewGap = this._itemScroll.Gap;
				this.DetailViewGap = this._itemScroll.Gap;
			}
			this._simplePrefab = base.CGet<ItemView>("ItemView");
			this._detailPrefab = base.CGet<ItemView>("ItemDetailView");
			this._inited = true;
		}
	}

	// Token: 0x06002386 RID: 9094 RVA: 0x00104E80 File Offset: 0x00103080
	public void SetItemList(ref List<ItemDisplayData> itemList, bool reset = false, string listTag = null, bool detailView = false, Action<ItemDisplayData, ItemView> onRenderItem = null)
	{
		if (reset)
		{
			this._onRenderItem = onRenderItem;
			this._listTag = listTag;
		}
		this.MySortAndFilter.SetItemList(ref itemList, reset, listTag, detailView, new Action(this.OnItemListChanged), new Action(this.OnViewTypeChanged));
	}

	// Token: 0x06002387 RID: 9095 RVA: 0x00104ECD File Offset: 0x001030CD
	public void SetCharId(int charId)
	{
		this._charId = charId;
	}

	// Token: 0x06002388 RID: 9096 RVA: 0x00104ED7 File Offset: 0x001030D7
	public void ScrollToTop()
	{
		this._itemScroll.ScrollTo(0, 0.3f);
	}

	// Token: 0x06002389 RID: 9097 RVA: 0x00104EEC File Offset: 0x001030EC
	public override void ReRender()
	{
		this._itemScroll.ReRender();
	}

	// Token: 0x0600238A RID: 9098 RVA: 0x00104EFB File Offset: 0x001030FB
	public override void SetItemList(ref List<ItemDisplayData> itemList)
	{
		this.SetItemList(ref itemList, false, null, false, null);
	}

	// Token: 0x0600238B RID: 9099 RVA: 0x00104F0A File Offset: 0x0010310A
	public void ReRenderChild(int index)
	{
		this._itemScroll.RefreshCell(index);
	}

	// Token: 0x0600238C RID: 9100 RVA: 0x00104F1C File Offset: 0x0010311C
	public void ReRenderChild(ItemKey key)
	{
		int index = this.MySortAndFilter.OutputItemList.FindIndex((ItemDisplayData data) => data.ContainsItemKey(key));
		ItemView itemView = this.FindActiveItem(key, false);
		bool flag = index >= 0 && null != itemView;
		if (flag)
		{
			this.OnRenderItem(index, itemView);
		}
	}

	// Token: 0x0600238D RID: 9101 RVA: 0x00104F80 File Offset: 0x00103180
	public void ReRenderChild(ItemDisplayData itemDisplayData)
	{
		int index = this.MySortAndFilter.OutputItemList.IndexOf(itemDisplayData);
		ItemView itemView = this.FindActiveItem(itemDisplayData, false);
		bool flag = index >= 0 && null != itemView;
		if (flag)
		{
			this.OnRenderItem(index, itemView);
		}
	}

	// Token: 0x0600238E RID: 9102 RVA: 0x00104FC6 File Offset: 0x001031C6
	public int FindItemIndex(ItemDisplayData itemDisplayData)
	{
		return this.MySortAndFilter.OutputItemList.IndexOf(itemDisplayData);
	}

	// Token: 0x0600238F RID: 9103 RVA: 0x00104FDC File Offset: 0x001031DC
	public ItemView FindActiveItem(ItemKey key, bool realShowing = false)
	{
		int index = this.MySortAndFilter.OutputItemList.FindIndex((ItemDisplayData data) => data.ContainsItemKey(key));
		return this.GetShowingItem(index, realShowing);
	}

	// Token: 0x06002390 RID: 9104 RVA: 0x00105024 File Offset: 0x00103224
	public ItemView FindActiveItem(ItemDisplayData itemDisplayData, bool realShowing = false)
	{
		int index = this.FindItemIndex(itemDisplayData);
		return this.GetShowingItem(index, realShowing);
	}

	// Token: 0x06002391 RID: 9105 RVA: 0x00105048 File Offset: 0x00103248
	private ItemView GetShowingItem(int index, bool realShowing = false)
	{
		ItemView itemView = (index >= 0) ? (this._itemScroll.GetActiveCell(index) as ItemView) : null;
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

	// Token: 0x06002392 RID: 9106 RVA: 0x001050D4 File Offset: 0x001032D4
	public override void SetItemToSelectCountMode(int index, ItemView itemView, Action<int> onConfirmSetCount, Action onCancelSetCount, int initSelectCount = 0, int limitCount = 0, int minCount = 1, string limitTip = null, bool keepSelectedOnHide = false, Action<int> onCountChange = null, Transform itemViewOriginParent = null, int itemViewOriginSibling = -1)
	{
		CScrollRectLegacy scrollRect = this._itemScroll.GetComponent<CScrollRectLegacy>();
		ItemDisplayData itemData = this.MySortAndFilter.OutputItemList[index];
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
		itemView.SetHighLight(true);
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
			itemView.SetClickEvent(delegate
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
			itemView.GetComponentInParent<ItemScrollView>(true).ReRenderChild(itemView.Data);
		}));
	}

	// Token: 0x06002393 RID: 9107 RVA: 0x00105344 File Offset: 0x00103544
	public override ItemView FindItemViewByItem(ItemKey key)
	{
		return this.FindActiveItem(key, false);
	}

	// Token: 0x06002394 RID: 9108 RVA: 0x00105360 File Offset: 0x00103560
	public void SetItemToPopupMenuMode(ItemKey key, List<ViewPopupMenu.BtnData> btnList, Action onCancel)
	{
		int index = this.MySortAndFilter.OutputItemList.FindIndex((ItemDisplayData data) => data.ContainsItemKey(key));
		CScrollRectLegacy scrollRect = this._itemScroll.GetComponent<CScrollRectLegacy>();
		ItemView itemView = this._itemScroll.GetActiveCell(index).GetComponent<ItemView>();
		RectTransform itemRectTrans = itemView.GetComponent<RectTransform>();
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.SetObject("BtnInfo", btnList);
		argBox.SetObject("ScreenPos", UIManager.Instance.UiCamera.WorldToScreenPoint(itemRectTrans.position));
		argBox.SetObject("ItemSize", itemRectTrans.sizeDelta);
		argBox.SetObject("OnCancel", onCancel);
		UIElement popupMenu = UIElement.PopupMenu;
		popupMenu.OnShowed = (Action)Delegate.Combine(popupMenu.OnShowed, new Action(delegate()
		{
			itemView.SetHighLight(true);
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

	// Token: 0x06002395 RID: 9109 RVA: 0x00105492 File Offset: 0x00103692
	private void OnItemListChanged()
	{
		this._itemScroll.UpdateData(this.SortAndFilter.OutputItemList.Count);
		Action itemListChangedAction = this.ItemListChangedAction;
		if (itemListChangedAction != null)
		{
			itemListChangedAction();
		}
	}

	// Token: 0x06002396 RID: 9110 RVA: 0x001054C4 File Offset: 0x001036C4
	private void OnViewTypeChanged()
	{
		this._itemScroll.UpdateStyle(InfinityScrollLegacy.ScrollDirection.FromTop, this.MySortAndFilter.IsDetailView ? this.DetailViewLineCount : this.SimpleViewLineCount, this.MySortAndFilter.IsDetailView ? this.DetailViewGap : this.SimpleViewGap, this._itemScroll.Padding, this.MySortAndFilter.IsDetailView ? this._detailPrefab : this._simplePrefab);
	}

	// Token: 0x06002397 RID: 9111 RVA: 0x0010553C File Offset: 0x0010373C
	private void OnRenderItem(int index, Refers itemRefers)
	{
		ItemDisplayData itemData = this.MySortAndFilter.OutputItemList[index];
		ItemView itemView = itemRefers as ItemView;
		CEmptyGraphic itemGraphic = itemView.GetComponent<CEmptyGraphic>();
		itemView.SetData(itemData, this.MySortAndFilter.IsDetailView, -1, false, true, null, false, true);
		itemView.SetCharId(this._charId);
		itemView.GetComponent<TooltipInvoker>().NeedRefresh = true;
		itemView.GetComponent<TooltipInvoker>().Refresh(false, -1);
		itemView.CGet<CImage>("EnterMark").gameObject.SetActive(false);
		itemView.SetHighLight(false);
		itemView.SetSelectState(false);
		itemGraphic.enabled = false;
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
		{
			bool flag = null != itemGraphic;
			if (flag)
			{
				itemGraphic.enabled = true;
			}
		});
		Action<ItemDisplayData, ItemView> onRenderItem = this._onRenderItem;
		if (onRenderItem != null)
		{
			onRenderItem(itemData, itemView);
		}
	}

	// Token: 0x06002398 RID: 9112 RVA: 0x00105618 File Offset: 0x00103818
	private void OnItemHide(Refers itemRefers)
	{
		ItemView itemView = itemRefers as ItemView;
		itemView.OnItemHide();
	}

	// Token: 0x06002399 RID: 9113 RVA: 0x00105634 File Offset: 0x00103834
	public override void HandleClickItem(ItemDisplayData itemData, ItemView itemView, Action<ItemView> onClick)
	{
		Action action = delegate()
		{
			ItemView targetItemView = this.FindActiveItem(itemData, false);
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

	// Token: 0x04001B1D RID: 6941
	private InfinityScrollLegacy _itemScroll;

	// Token: 0x04001B1E RID: 6942
	private ItemView _simplePrefab;

	// Token: 0x04001B1F RID: 6943
	private ItemView _detailPrefab;

	// Token: 0x04001B20 RID: 6944
	private Action<ItemDisplayData, ItemView> _onRenderItem;

	// Token: 0x04001B21 RID: 6945
	private bool _inited = false;

	// Token: 0x04001B22 RID: 6946
	private int _charId = -1;

	// Token: 0x04001B23 RID: 6947
	[HideInInspector]
	public Action ItemListChangedAction;
}
