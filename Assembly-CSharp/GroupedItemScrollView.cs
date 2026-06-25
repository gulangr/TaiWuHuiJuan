using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FrameWork;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200022B RID: 555
public class GroupedItemScrollView : BaseItemScrollView
{
	// Token: 0x17000371 RID: 881
	// (get) Token: 0x06002341 RID: 9025 RVA: 0x0010371B File Offset: 0x0010191B
	// (set) Token: 0x06002342 RID: 9026 RVA: 0x00103723 File Offset: 0x00101923
	public ItemSortAndFilter MySortAndFilter { get; private set; }

	// Token: 0x17000372 RID: 882
	// (get) Token: 0x06002343 RID: 9027 RVA: 0x0010372C File Offset: 0x0010192C
	// (set) Token: 0x06002344 RID: 9028 RVA: 0x00103734 File Offset: 0x00101934
	public GroupedInfinityScroll ItemScrollView { get; private set; }

	// Token: 0x06002345 RID: 9029 RVA: 0x0010373D File Offset: 0x0010193D
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x06002346 RID: 9030 RVA: 0x00103748 File Offset: 0x00101948
	public override void Init()
	{
		bool inited = this._inited;
		if (!inited)
		{
			this.InitRefers();
			this.MySortAndFilter = this._itemSortAndFilter;
			this.MySortAndFilter.Init();
			this.MySortAndFilter.InitIsDetailView(this.IsDetailView);
			this.ItemScrollView = base.GetComponent<GroupedInfinityScroll>();
			this.ItemScrollView.Init();
			this.ItemScrollView.OnGroupTitleRender = new Action<int, Refers>(this.OnRenderTitle);
			this.ItemScrollView.OnItemRender = new Action<int, int, Refers>(this.OnRenderItem);
			this.MySortAndFilter.SetItemList(ref this._itemList, true, null, this.IsDetailView, new Action(this.OnItemListChanged), new Action(this.OnViewTypeChanged));
			this._inited = true;
		}
	}

	// Token: 0x06002347 RID: 9031 RVA: 0x00103817 File Offset: 0x00101A17
	public override void ReRender()
	{
		this.ItemScrollView.ReRender();
	}

	// Token: 0x06002348 RID: 9032 RVA: 0x00103826 File Offset: 0x00101A26
	public override void SetItemList(ref List<ItemDisplayData> itemList)
	{
		this.SetItemList(itemList, null, this.IsDetailView, this._onRenderItem, false, this._listTag);
	}

	// Token: 0x17000373 RID: 883
	// (get) Token: 0x06002349 RID: 9033 RVA: 0x00103846 File Offset: 0x00101A46
	public override ItemSortAndFilter SortAndFilter
	{
		get
		{
			return this.MySortAndFilter;
		}
	}

	// Token: 0x17000374 RID: 884
	// (get) Token: 0x0600234A RID: 9034 RVA: 0x0010384E File Offset: 0x00101A4E
	public override RectTransform ViewportRectTransform
	{
		get
		{
			return this.ItemScrollView.GetViewport();
		}
	}

	// Token: 0x0600234B RID: 9035 RVA: 0x0010385C File Offset: 0x00101A5C
	public override void SetItemToSelectCountMode(int index, ItemView itemView, Action<int> onConfirmSetCount, Action onCancelSetCount, int initSelectCount = 0, int limitCount = 0, int minCount = 1, string limitTip = null, bool keepSelectedOnHide = false, Action<int> onCountChange = null, Transform itemViewOriginParent = null, int itemViewOriginSibling = -1)
	{
		GroupedItemScrollView.<>c__DisplayClass28_0 CS$<>8__locals1 = new GroupedItemScrollView.<>c__DisplayClass28_0();
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
		ItemDisplayData itemData = this.MySortAndFilter.OutputItemList[index];
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
		bool isMiscResource = ItemTemplateHelper.IsMiscResource(itemData.Key.ItemType, itemData.Key.TemplateId);
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
		CS$<>8__locals1.itemView.SetHighLight(true);
		argBox.SetObject("ItemRectTrans", CS$<>8__locals1.itemRectTrans);
		argBox.Set("ZeroValid", false);
		UIElement.SetSelectCount.SetOnInitArgs(argBox);
		CS$<>8__locals1.layout = CS$<>8__locals1.itemViewOriginParent.GetComponent<HorizontalLayoutGroup>();
		CS$<>8__locals1.layout.enabled = false;
		UIManager.Instance.ShowUI(UIElement.SetSelectCount, true);
		UIElement setSelectCount = UIElement.SetSelectCount;
		setSelectCount.OnShowed = (Action)Delegate.Combine(setSelectCount.OnShowed, new Action(delegate()
		{
			bool flag5 = CS$<>8__locals1.itemViewOriginParent != null;
			if (flag5)
			{
				CS$<>8__locals1.spaceHolder.transform.SetParent(CS$<>8__locals1.itemViewOriginParent);
			}
			CS$<>8__locals1.spaceHolder.transform.SetSiblingIndex(CS$<>8__locals1.itemViewOriginSibling);
			CS$<>8__locals1.spaceHolderRectTransform.sizeDelta = CS$<>8__locals1.itemRectTrans.sizeDelta;
			CS$<>8__locals1.spaceHolderRectTransform.localScale = CS$<>8__locals1.itemRectTrans.localScale;
			CS$<>8__locals1.layout.enabled = true;
			CS$<>8__locals1.itemView.SetClickEvent(delegate
			{
				GEvent.OnEvent(UiEvents.OnConfirmSetSelectCount, null);
			});
		}));
		UIElement setSelectCount2 = UIElement.SetSelectCount;
		setSelectCount2.OnHide = (Action)Delegate.Combine(setSelectCount2.OnHide, new Action(delegate()
		{
			bool flag5 = CS$<>8__locals1.itemViewOriginParent != null;
			if (flag5)
			{
				CS$<>8__locals1.itemRectTrans.SetParent(CS$<>8__locals1.itemViewOriginParent);
				CS$<>8__locals1.itemRectTrans.SetSiblingIndex(CS$<>8__locals1.itemViewOriginSibling);
			}
			Object.Destroy(CS$<>8__locals1.spaceHolder);
			CS$<>8__locals1.itemView.GetComponentInParent<GroupedItemScrollView>().ReRenderChild(CS$<>8__locals1.itemView.Data);
		}));
	}

	// Token: 0x0600234C RID: 9036 RVA: 0x00103B3C File Offset: 0x00101D3C
	public override ItemView FindItemViewByItem(ItemKey key)
	{
		return this._renderingItemViews.GetValueOrDefault(key);
	}

	// Token: 0x0600234D RID: 9037 RVA: 0x00103B5A File Offset: 0x00101D5A
	public void ReRenderChild(ItemDisplayData itemViewData)
	{
		this.ItemScrollView.ReRender();
	}

	// Token: 0x0600234E RID: 9038 RVA: 0x00103B6C File Offset: 0x00101D6C
	public void SetItemList(List<ItemDisplayData> itemList, GroupedItemScrollView.ItemGroupGetter groupGetter = null, bool isDetailView = false, Action<ItemDisplayData, ItemView> onRenderItem = null, bool isReset = false, string listTag = null)
	{
		this._onRenderItem = onRenderItem;
		this._itemList = itemList;
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
		this.IsDetailView = isDetailView;
		this.ChangeIsDetail();
		this.MySortAndFilter.SetItemList(ref itemList, isReset, listTag, isDetailView, new Action(this.OnItemListChanged), new Action(this.OnViewTypeChanged));
	}

	// Token: 0x0600234F RID: 9039 RVA: 0x00103BE9 File Offset: 0x00101DE9
	public void SetOnRenderTitle(Action<int, Refers> onRenderTitle)
	{
		this._onRenderTitle = onRenderTitle;
	}

	// Token: 0x06002350 RID: 9040 RVA: 0x00103BF2 File Offset: 0x00101DF2
	public void ResetScroll()
	{
		this.ItemScrollView.UpdateData(this._scrollDataList, true);
	}

	// Token: 0x06002351 RID: 9041 RVA: 0x00103C08 File Offset: 0x00101E08
	private void OnItemListChanged()
	{
		this._itemGroups.Clear();
		bool flag = this._groupGetter != null;
		if (flag)
		{
			this._itemGroups.AddRange(this._groupGetter(this.MySortAndFilter.OutputItemList));
		}
		this.UpdateScrollDataList(this._itemGroups);
		this.ItemScrollView.UpdateData(this._scrollDataList, !this._inited);
		Action itemListChangedAction = this.ItemListChangedAction;
		if (itemListChangedAction != null)
		{
			itemListChangedAction();
		}
	}

	// Token: 0x06002352 RID: 9042 RVA: 0x00103C8D File Offset: 0x00101E8D
	private void OnViewTypeChanged()
	{
		this.IsDetailView = this.MySortAndFilter.IsDetailView;
		this.ChangeIsDetail();
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
		{
			this.ItemScrollView.UpdateData(this._scrollDataList, false);
		});
	}

	// Token: 0x06002353 RID: 9043 RVA: 0x00103CC0 File Offset: 0x00101EC0
	private void ChangeIsDetail()
	{
		this.ItemScrollView.ContentItemCountPerLine = (this.IsDetailView ? this.DetailViewLineCount : this.SimpleViewLineCount);
		this.ItemScrollView.ContentItemTemplate = (this.IsDetailView ? this._itemDetailView : this._itemView);
		bool isVertical = this.ItemScrollView.IsVertical;
		if (isVertical)
		{
			this.ItemScrollView.InLineSpacing = (this.IsDetailView ? this.DetailViewGap.x : this.SimpleViewGap.x);
		}
		else
		{
			this.ItemScrollView.InLineSpacing = (this.IsDetailView ? this.DetailViewGap.y : this.SimpleViewGap.y);
		}
	}

	// Token: 0x06002354 RID: 9044 RVA: 0x00103D7C File Offset: 0x00101F7C
	private void UpdateScrollDataList(List<GroupedItemScrollView.ItemGroup> itemGroups)
	{
		this._scrollDataList.Clear();
		foreach (GroupedItemScrollView.ItemGroup itemGroup in itemGroups)
		{
			GroupedInfinityScroll.GroupItem groupItem = new GroupedInfinityScroll.GroupItem(itemGroup.GroupId, itemGroup.ItemList.Count);
			this._scrollDataList.Add(groupItem);
		}
	}

	// Token: 0x06002355 RID: 9045 RVA: 0x00103DFC File Offset: 0x00101FFC
	public void SetCharId(int charId)
	{
		this._charId = charId;
	}

	// Token: 0x06002356 RID: 9046 RVA: 0x00103E08 File Offset: 0x00102008
	private void OnRenderTitle(int groupIndex, Refers refers)
	{
		TextMeshProUGUI titleName = refers.CGet<TextMeshProUGUI>("TitleName");
		titleName.text = this._itemGroups[groupIndex].GroupTitleText;
		bool disableStyle = this.GrayEmptyGroupTitle && this._itemGroups[groupIndex].ItemList.Count == 0;
		DisableStyleRoot disable = refers.GetComponent<DisableStyleRoot>();
		disable.SetStyleEffect(disableStyle, false);
		Action<int, Refers> onRenderTitle = this._onRenderTitle;
		if (onRenderTitle != null)
		{
			onRenderTitle(groupIndex, refers);
		}
	}

	// Token: 0x06002357 RID: 9047 RVA: 0x00103E84 File Offset: 0x00102084
	private void OnRenderItem(int groupIndex, int index, Refers refers)
	{
		ItemView itemView = (ItemView)refers;
		ItemDisplayData displayData = this._itemGroups[groupIndex].ItemList[index];
		this._renderingItemViews[displayData.RealKey] = itemView;
		itemView.SetData(displayData, this.MySortAndFilter.IsDetailView, -1, false, true, null, false, true);
		itemView.SetCharId(this._charId);
		itemView.GetComponent<TooltipInvoker>().NeedRefresh = true;
		itemView.GetComponent<TooltipInvoker>().Refresh(false, -1);
		itemView.CGet<CImage>("EnterMark").gameObject.SetActive(false);
		itemView.SetHighLight(false);
		itemView.SetSelectState(false);
		CEmptyGraphic itemGraphic = itemView.GetComponent<CEmptyGraphic>();
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
			onRenderItem(displayData, itemView);
		}
	}

	// Token: 0x06002358 RID: 9048 RVA: 0x00103F78 File Offset: 0x00102178
	public override void HandleClickItem(ItemDisplayData itemData, ItemView itemView, Action<ItemView> onClick)
	{
		GroupedItemScrollView.<>c__DisplayClass41_0 CS$<>8__locals1;
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
		this.<HandleClickItem>g__Action|41_0(ref CS$<>8__locals1);
	}

	// Token: 0x06002359 RID: 9049 RVA: 0x00104051 File Offset: 0x00102251
	private void InitRefers()
	{
		this._itemSortAndFilter = base.CGet<ItemSortAndFilter>("ItemSortAndFilter");
		this._itemView = base.CGet<ItemView>("ItemView");
		this._itemDetailView = base.CGet<ItemView>("ItemDetailView");
	}

	// Token: 0x0600235C RID: 9052 RVA: 0x001040DC File Offset: 0x001022DC
	[CompilerGenerated]
	private void <HandleClickItem>g__Action|41_0(ref GroupedItemScrollView.<>c__DisplayClass41_0 A_1)
	{
		ItemView targetItemView = this.FindItemViewByItem(A_1.itemData.RealKey);
		A_1.onClick(targetItemView);
	}

	// Token: 0x04001B02 RID: 6914
	[Tooltip("是否需要把空白分组压暗")]
	public bool GrayEmptyGroupTitle;

	// Token: 0x04001B03 RID: 6915
	private bool _inited;

	// Token: 0x04001B04 RID: 6916
	private List<ItemDisplayData> _itemList = new List<ItemDisplayData>();

	// Token: 0x04001B05 RID: 6917
	private readonly List<GroupedItemScrollView.ItemGroup> _itemGroups = new List<GroupedItemScrollView.ItemGroup>();

	// Token: 0x04001B06 RID: 6918
	private readonly List<GroupedInfinityScroll.GroupItem> _scrollDataList = new List<GroupedInfinityScroll.GroupItem>();

	// Token: 0x04001B07 RID: 6919
	private GroupedItemScrollView.ItemGroupGetter _groupGetter;

	// Token: 0x04001B08 RID: 6920
	private Action<ItemDisplayData, ItemView> _onRenderItem;

	// Token: 0x04001B09 RID: 6921
	private Action<int, Refers> _onRenderTitle;

	// Token: 0x04001B0A RID: 6922
	private readonly Dictionary<ItemKey, ItemView> _renderingItemViews = new Dictionary<ItemKey, ItemView>();

	// Token: 0x04001B0B RID: 6923
	[HideInInspector]
	public Action ItemListChangedAction;

	// Token: 0x04001B0C RID: 6924
	private int _charId = -1;

	// Token: 0x04001B0D RID: 6925
	private ItemSortAndFilter _itemSortAndFilter;

	// Token: 0x04001B0E RID: 6926
	private ItemView _itemView;

	// Token: 0x04001B0F RID: 6927
	private ItemView _itemDetailView;

	// Token: 0x02001500 RID: 5376
	// (Invoke) Token: 0x0600CD91 RID: 52625
	public delegate List<GroupedItemScrollView.ItemGroup> ItemGroupGetter(List<ItemDisplayData> itemList);

	// Token: 0x02001501 RID: 5377
	public struct ItemGroup
	{
		// Token: 0x0600CD94 RID: 52628 RVA: 0x0059C1B4 File Offset: 0x0059A3B4
		public ItemGroup(int groupId, string groupTitleText)
		{
			this.GroupId = groupId;
			this.GroupTitleText = groupTitleText;
			this.ItemList = new List<ItemDisplayData>();
		}

		// Token: 0x0400A339 RID: 41785
		public int GroupId;

		// Token: 0x0400A33A RID: 41786
		public string GroupTitleText;

		// Token: 0x0400A33B RID: 41787
		public List<ItemDisplayData> ItemList;
	}
}
