using System;
using System.Collections.Generic;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;

// Token: 0x020003A5 RID: 933
public class UI_SelectItem : UIBase
{
	// Token: 0x06003812 RID: 14354 RVA: 0x001C3B18 File Offset: 0x001C1D18
	public override void OnInit(ArgumentBox argsBox)
	{
		bool flag = !argsBox.Get<ItemKey>("initItemKey", out this._initItem);
		if (flag)
		{
			this._initItem = ItemKey.Invalid;
		}
		this._currSelectedItem = ItemKey.Invalid;
		bool flag2 = null == this._popupWindow;
		if (flag2)
		{
			this._popupWindow = base.CGet<PopupWindow>("PopupWindowBase");
		}
		string title;
		bool flag3 = !argsBox.Get("title", out title);
		if (flag3)
		{
			title = LocalStringManager.Get(LanguageKey.LK_UI_SelectItem);
		}
		this._popupWindow.SetTitle(title);
		ItemSortAndFilter.ItemFilterType filterType;
		this._filterType = (argsBox.Get<ItemSortAndFilter.ItemFilterType>("filterType", out filterType) ? filterType : ItemSortAndFilter.ItemFilterType.Invalid);
		int charId;
		bool flag4 = !argsBox.Get("CharId", out charId);
		if (flag4)
		{
			charId = -1;
		}
		argsBox.Get("showNone", out this._showNone);
		argsBox.Get<Action<ItemKey>>("callback", out this._onSelected);
		this._popupWindow.CancelButton.interactable = true;
		this._popupWindow.OnConfirmClick = new Action(this.OnConfirmSelect);
		this._popupWindow.ConfirmButton.interactable = this._showNone;
		this._popupWindow.OnCancelClick = new Action(this.OnCancel);
		List<ItemDisplayData> itemList;
		bool flag5 = argsBox.Get<List<ItemDisplayData>>("DisplayData", out itemList);
		if (flag5)
		{
			this._itemList.Clear();
			bool showNone = this._showNone;
			if (showNone)
			{
				this._itemList.Add(new ItemDisplayData());
			}
			this._itemList.AddRange(itemList);
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
			{
				this._itemScroll.SetCharId(charId);
				this._itemScroll.SortAndFilter.StaticAheadItemKeysList.Clear();
				bool flag9 = !this._initItem.Equals(ItemKey.Invalid);
				if (flag9)
				{
					this._itemScroll.SortAndFilter.StaticAheadItemKeysList.Add(this._initItem);
				}
				this._itemScroll.SetItemList(ref this._itemList, false, null, false, null);
				this._itemScroll.SortAndFilter.LockFilterType(this._filterType, ItemSortAndFilter.LockFilterTypeToggleActionMode.Default);
				bool flag10 = this._initItem != ItemKey.Invalid && !this._showNone;
				if (flag10)
				{
					this._currSelectedItem = this._itemList[0].Key;
					this._initItem = ItemKey.Invalid;
					this._popupWindow.ConfirmButton.interactable = true;
					ConchShipCursor.Instance.SetSelectCount(1, this._selectCountLimit);
				}
			}));
		}
		else
		{
			argsBox.Get("itemSubType", out this._itemSubType);
			argsBox.Get("fromWhereSelect", out this._fromWhereSelect);
			bool flag6 = this._fromWhereSelect == 0;
			if (flag6)
			{
				UIElement element2 = this.Element;
				AsyncMethodCallbackDelegate <>9__2;
				element2.OnListenerIdReady = (Action)Delegate.Combine(element2.OnListenerIdReady, new Action(delegate()
				{
					IAsyncMethodRequestHandler <>4__this = this;
					int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
					short itemSubType = this._itemSubType;
					AsyncMethodCallbackDelegate callback;
					if ((callback = <>9__2) == null)
					{
						callback = (<>9__2 = delegate(int offset, RawDataPool dataPool)
						{
							Serializer.Deserialize(dataPool, offset, ref this._itemList);
							this._itemScroll.SetCharId(charId);
							bool showNone2 = this._showNone;
							if (showNone2)
							{
								this._itemList.Insert(0, new ItemDisplayData());
							}
							this._itemScroll.SortAndFilter.StaticAheadItemKeysList.Clear();
							bool flag9 = !this._initItem.Equals(ItemKey.Invalid);
							if (flag9)
							{
								this._itemScroll.SortAndFilter.StaticAheadItemKeysList.Add(this._initItem);
							}
							this._itemScroll.SetItemList(ref this._itemList, false, null, false, null);
							this._itemScroll.SortAndFilter.LockFilterType(this._filterType, ItemSortAndFilter.LockFilterTypeToggleActionMode.Default);
						});
					}
					CharacterDomainMethod.AsyncCall.GetInventoryItems(<>4__this, taiwuCharId, itemSubType, callback);
				}));
			}
			else
			{
				bool flag7 = this._fromWhereSelect == 1;
				if (flag7)
				{
					UIElement element3 = this.Element;
					AsyncMethodCallbackDelegate <>9__4;
					element3.OnListenerIdReady = (Action)Delegate.Combine(element3.OnListenerIdReady, new Action(delegate()
					{
						IAsyncMethodRequestHandler <>4__this = this;
						short itemSubType = this._itemSubType;
						AsyncMethodCallbackDelegate callback;
						if ((callback = <>9__4) == null)
						{
							callback = (<>9__4 = delegate(int offset, RawDataPool dataPool)
							{
								Serializer.Deserialize(dataPool, offset, ref this._itemList);
								this._itemScroll.SetCharId(charId);
								bool showNone2 = this._showNone;
								if (showNone2)
								{
									this._itemList.Insert(0, new ItemDisplayData());
								}
								this._itemScroll.SortAndFilter.StaticAheadItemKeysList.Clear();
								bool flag9 = !this._initItem.Equals(ItemKey.Invalid);
								if (flag9)
								{
									this._itemScroll.SortAndFilter.StaticAheadItemKeysList.Add(this._initItem);
								}
								this._itemScroll.SetItemList(ref this._itemList, false, null, false, null);
								this._itemScroll.SortAndFilter.LockFilterType(this._filterType, ItemSortAndFilter.LockFilterTypeToggleActionMode.Default);
							});
						}
						TaiwuDomainMethod.AsyncCall.GetCanOperateItemDisplayDataInVillage(<>4__this, itemSubType, callback);
					}));
				}
			}
		}
		bool flag8 = argsBox.Get("ShowSelectCountLimit", out this._showSelectCountLimit);
		if (flag8)
		{
			argsBox.Get("SelectCountLimit", out this._selectCountLimit);
			UIElement element4 = this.Element;
			element4.OnShowed = (Action)Delegate.Combine(element4.OnShowed, new Action(delegate()
			{
				ConchShipCursor.Instance.SetSelectCountActive(true);
			}));
			ConchShipCursor.Instance.SetSelectCount(0, this._selectCountLimit);
		}
	}

	// Token: 0x06003813 RID: 14355 RVA: 0x001C3DF0 File Offset: 0x001C1FF0
	private void Update()
	{
		bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			Action onConfirmClick = this._popupWindow.OnConfirmClick;
			if (onConfirmClick != null)
			{
				onConfirmClick();
			}
		}
	}

	// Token: 0x06003814 RID: 14356 RVA: 0x001C3E30 File Offset: 0x001C2030
	private void Awake()
	{
		this._itemScroll = base.CGet<ItemScrollView>("ItemScrollView");
		this._itemScroll.Init();
		this._itemScroll.SetItemList(ref this._itemList, true, null, this._itemScroll.SortAndFilter.IsDetailView, new Action<ItemDisplayData, ItemView>(this.OnRenderItem));
	}

	// Token: 0x06003815 RID: 14357 RVA: 0x001C3E8B File Offset: 0x001C208B
	private void OnDisable()
	{
		this._itemScroll.SaveSortFilterSetting(true);
	}

	// Token: 0x06003816 RID: 14358 RVA: 0x001C3E9C File Offset: 0x001C209C
	private void OnConfirmSelect()
	{
		bool showSelectCountLimit = this._showSelectCountLimit;
		if (showSelectCountLimit)
		{
			ConchShipCursor.Instance.SetSelectCountActive(false);
		}
		this._onSelected(this._currSelectedItem);
		UIManager.Instance.HideUI(this.Element);
		this._currSelectedItem = ItemKey.Invalid;
	}

	// Token: 0x06003817 RID: 14359 RVA: 0x001C3EF0 File Offset: 0x001C20F0
	private void OnCancel()
	{
		bool showSelectCountLimit = this._showSelectCountLimit;
		if (showSelectCountLimit)
		{
			ConchShipCursor.Instance.SetSelectCountActive(false);
		}
		UIManager.Instance.HideUI(this.Element);
		this._currSelectedItem = ItemKey.Invalid;
		this._itemList.Clear();
		this._itemScroll.SetItemList(ref this._itemList, false, null, false, null);
	}

	// Token: 0x06003818 RID: 14360 RVA: 0x001C3F54 File Offset: 0x001C2154
	private void OnRenderItem(ItemDisplayData itemData, ItemView itemView)
	{
		itemView.SetClickEvent(delegate
		{
			this.OnClickItem(itemView);
		});
		itemView.SetHighLight(itemData.ContainsItemKey(this._currSelectedItem));
		itemView.SetPrevSelected(itemData.ContainsItemKey(this._initItem));
	}

	// Token: 0x06003819 RID: 14361 RVA: 0x001C3FC0 File Offset: 0x001C21C0
	private void OnClickItem(ItemView itemView)
	{
		bool showSelectCountLimit = this._showSelectCountLimit;
		if (showSelectCountLimit)
		{
			ConchShipCursor.Instance.SetSelectCount(1, this._selectCountLimit);
		}
		ItemView itemView2 = this._itemScroll.FindActiveItem(this._currSelectedItem, false);
		if (itemView2 != null)
		{
			itemView2.SetHighLight(false);
		}
		itemView.SetHighLight(true);
		this._currSelectedItem = itemView.Data.Key;
		this._popupWindow.ConfirmButton.interactable = true;
	}

	// Token: 0x0600381A RID: 14362 RVA: 0x001C4034 File Offset: 0x001C2234
	public override void QuickHide()
	{
		bool showSelectCountLimit = this._showSelectCountLimit;
		if (showSelectCountLimit)
		{
			ConchShipCursor.Instance.SetSelectCountActive(false);
		}
		AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
		base.QuickHide();
	}

	// Token: 0x04002896 RID: 10390
	private Action<ItemKey> _onSelected;

	// Token: 0x04002897 RID: 10391
	private Action _onRemove;

	// Token: 0x04002898 RID: 10392
	private short _itemSubType;

	// Token: 0x04002899 RID: 10393
	private sbyte _fromWhereSelect;

	// Token: 0x0400289A RID: 10394
	private List<ItemDisplayData> _itemList = new List<ItemDisplayData>();

	// Token: 0x0400289B RID: 10395
	private ItemKey _initItem;

	// Token: 0x0400289C RID: 10396
	private ItemKey _currSelectedItem;

	// Token: 0x0400289D RID: 10397
	private bool _showNone;

	// Token: 0x0400289E RID: 10398
	private bool _showSelectCountLimit;

	// Token: 0x0400289F RID: 10399
	private int _selectCountLimit;

	// Token: 0x040028A0 RID: 10400
	private ItemScrollView _itemScroll;

	// Token: 0x040028A1 RID: 10401
	private ItemSortAndFilter.ItemFilterType _filterType;

	// Token: 0x040028A2 RID: 10402
	private PopupWindow _popupWindow;
}
