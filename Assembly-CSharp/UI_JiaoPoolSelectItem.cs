using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Item;
using Game.Components.SortAndFilter.Item.Apply;
using GameData.DLC.FiveLoong;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

// Token: 0x020001B8 RID: 440
public class UI_JiaoPoolSelectItem : UIBase
{
	// Token: 0x06001A5C RID: 6748 RVA: 0x000AE0A0 File Offset: 0x000AC2A0
	public override void OnInit(ArgumentBox argsBox)
	{
		this._allJiaoData.Clear();
		List<Jiao> allJiaoData;
		bool flag = argsBox.Get<List<Jiao>>("AllJiaoData", out allJiaoData);
		if (flag)
		{
			this._allJiaoData.AddRange(allJiaoData);
		}
		this._currentAllItemDisplayData.Clear();
		this._breedingJiaoList.Clear();
		argsBox.Get("IsCurrentLocatedInIndustryBlock", out this._isCurrentLocatedInIndustryBlock);
		int type;
		bool flag2 = argsBox.Get("Type", out type);
		if (flag2)
		{
			this._popUpType = (JiaoPoolPopUpType)type;
		}
		List<ItemDisplayData> displayDatas;
		bool flag3 = argsBox.Get<List<ItemDisplayData>>("AllItemDisplayData", out displayDatas);
		if (flag3)
		{
			this._currentAllItemDisplayData.AddRange(displayDatas);
		}
		argsBox.Get<Jiao>("CurrentJiao", out this._currentPoolJiao);
		this._currentPoolJiaoKey = ((this._currentPoolJiao == null) ? ItemKey.Invalid : this._currentPoolJiao.Key);
		argsBox.Get("PoolId", out this.poolId);
		string title;
		argsBox.Get("Title", out title);
		List<JiaoPool> poolList;
		bool flag4 = this._popUpType == JiaoPoolPopUpType.Breeding && argsBox.Get<List<JiaoPool>>("AllJiaoPool", out poolList);
		if (flag4)
		{
			foreach (JiaoPool pool in poolList)
			{
				bool flag5 = pool.Jiaos.Count > 1;
				if (flag5)
				{
					this._breedingJiaoList.AddRange(pool.Jiaos);
				}
			}
		}
		bool flag6 = argsBox.Get<Action<int, ItemKey>>("OnConfirm", out this._onConfirm);
		if (flag6)
		{
			this.sureBtn = base.CGet<CButton>("BtnSure");
			this.sureBtn.ClearAndAddListener(delegate
			{
				Action<int, ItemKey> onConfirm = this._onConfirm;
				if (onConfirm != null)
				{
					onConfirm(this.poolId, this._currentItemKey);
				}
				this.QuickHide();
			});
		}
		base.CGet<CToggleGroupObsolete>("InvestJiaoPool").gameObject.SetActive(false);
		base.CGet<CToggleGroupObsolete>("Breeding").gameObject.SetActive(false);
		base.CGet<ItemScrollView>("ItemScrollView").gameObject.SetActive(false);
		CButton cbutton = base.CGet<CButton>("BtnClose");
		if (cbutton != null)
		{
			cbutton.ClearAndAddListener(new Action(this.QuickHide));
		}
		this._itemPopUp = base.CGet<RectTransform>("ItemPopUp").GetComponent<RectTransform>();
		base.CGet<TextMeshProUGUI>("Title").text = title;
		this.BuildJiaoMap();
		this.PrepareDisplayItems();
		this.InitItemListScroll();
		this.sureBtn.interactable = false;
		this._initSelectItemKey = ItemKey.Invalid;
		this._currentItemKey = ItemKey.Invalid;
		UIElement element = this.Element;
		element.OnShowed = (Action)Delegate.Combine(element.OnShowed, new Action(delegate()
		{
			this._itemPopUp.DOLocalMove(new Vector3(0f, 0f, 0f), this.animaTime, false);
		}));
	}

	// Token: 0x06001A5D RID: 6749 RVA: 0x000AE34C File Offset: 0x000AC54C
	public override void QuickHide()
	{
		TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore = this._itemPopUp.DOLocalMove(new Vector3(0f, 0f, 0f), this.animaTime, false);
		tweenerCore.onComplete = (TweenCallback)Delegate.Combine(tweenerCore.onComplete, new TweenCallback(delegate()
		{
			base.QuickHide();
		}));
	}

	// Token: 0x06001A5E RID: 6750 RVA: 0x000AE3A4 File Offset: 0x000AC5A4
	private void BuildJiaoMap()
	{
		this._jiaoByKey.Clear();
		foreach (Jiao jiao in this._allJiaoData)
		{
			this._jiaoByKey[jiao.Key] = jiao;
		}
	}

	// Token: 0x06001A5F RID: 6751 RVA: 0x000AE414 File Offset: 0x000AC614
	private void PrepareDisplayItems()
	{
		this._displayItems.Clear();
		HashSet<ItemKey> eggKeys = null;
		bool flag = this._popUpType == JiaoPoolPopUpType.Breeding;
		if (flag)
		{
			eggKeys = (from item in this._allJiaoData
			where item.GrowthStage == 0
			select item.Key).ToHashSet<ItemKey>();
		}
		foreach (ItemDisplayData data in this._currentAllItemDisplayData)
		{
			bool flag2 = data.Key == this._currentPoolJiaoKey;
			if (!flag2)
			{
				bool flag3 = eggKeys != null && eggKeys.Contains(data.Key);
				if (!flag3)
				{
					this._displayItems.Add(data);
				}
			}
		}
	}

	// Token: 0x06001A60 RID: 6752 RVA: 0x000AE518 File Offset: 0x000AC718
	private void InitItemListScroll()
	{
		this._itemListScroll = base.CGet<ItemListScroll>("ItemListScroll");
		this._itemListScroll.Init("UI_JiaoPoolSelectItem", ESortAndFilterControllerType.Empty, true, new Action<ITradeableContent, RowItemLine>(this.OnItemRender), new Action<ITradeableContent, RowItemLine>(this.OnItemClick), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability, null, null, null);
		SortAndFilter sortAndFilterView = this._itemListScroll.GetComponentInChildren<SortAndFilter>(true);
		sortAndFilterView.SetEntryButtonForceHidden(true);
		this.ReplaceItemListSortFilter(new JiaoPoolSelectItemSortAndFilterController(sortAndFilterView, this._popUpType == JiaoPoolPopUpType.Invest, this._jiaoByKey));
		bool flag = this._popUpType == JiaoPoolPopUpType.Breeding;
		if (flag)
		{
			this._itemListScroll.SortAndFilterController.SetToggleIsOnWithoutNotify(47, 0);
		}
		this._itemListScroll.OnSortAndFilterChangedCallback = new Action(this.OnSortAndFilterChanged);
		this._itemListScroll.SetItemList(this._displayItems);
	}

	// Token: 0x06001A61 RID: 6753 RVA: 0x000AE5E5 File Offset: 0x000AC7E5
	private void OnSortAndFilterChanged()
	{
		this._currentItemKey = ItemKey.Invalid;
		this.sureBtn.interactable = false;
	}

	// Token: 0x06001A62 RID: 6754 RVA: 0x000AE600 File Offset: 0x000AC800
	private void OnItemRender(ITradeableContent data, RowItemLine rowItemLine)
	{
		RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
		rowItemMain.SetData(data);
		rowItemLine.Set(rowItemMain, true);
		string reason;
		bool canSelect = this.TryGetItemNotSelectableReason(data as ItemDisplayData, out reason);
		rowItemLine.SetInteractable(canSelect, true);
		rowItemLine.SetDisabled(!canSelect);
		bool flag = !canSelect;
		if (flag)
		{
			rowItemMain.SetItemNotCanSelectReason(reason);
		}
		else
		{
			rowItemMain.HideInteractionState();
		}
		rowItemLine.SetSelected(data.ContainsItemKey(this._currentItemKey));
	}

	// Token: 0x06001A63 RID: 6755 RVA: 0x000AE678 File Offset: 0x000AC878
	private void OnItemClick(ITradeableContent data, RowItemLine rowItemLine)
	{
		bool flag = !rowItemLine.Interactable;
		if (!flag)
		{
			RowItemLine rowItemLine2 = this._itemListScroll.FindActiveItem(this._currentItemKey, false);
			if (rowItemLine2 != null)
			{
				rowItemLine2.SetSelected(false);
			}
			rowItemLine.SetSelected(true);
			this._currentItemKey = data.Key;
			this.sureBtn.interactable = true;
		}
	}

	// Token: 0x06001A64 RID: 6756 RVA: 0x000AE6D8 File Offset: 0x000AC8D8
	private bool TryGetItemNotSelectableReason(ItemDisplayData data, out string reason)
	{
		reason = string.Empty;
		bool flag = data == null;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = !this._isCurrentLocatedInIndustryBlock && this._popUpType == JiaoPoolPopUpType.Breeding;
			if (flag2)
			{
				reason = "<color=#brightred>未达仓库</color>".ColorReplace();
				result = false;
			}
			else
			{
				Jiao jiao;
				bool flag3 = !this._jiaoByKey.TryGetValue(data.Key, out jiao) || this._popUpType != JiaoPoolPopUpType.Breeding;
				if (flag3)
				{
					result = true;
				}
				else
				{
					bool flag4 = jiao.Gender == this._currentPoolJiao.Gender;
					if (flag4)
					{
						reason = ("<color=#brightred>" + LocalStringManager.Get(LanguageKey.LK_JiaoPool_GenderMismatch) + "</color>").ColorReplace();
						result = false;
					}
					else
					{
						bool flag5 = jiao.GrowthStage == 1 || jiao.GrowthStage == 0;
						if (flag5)
						{
							reason = ("<color=#brightred>" + LocalStringManager.Get(LanguageKey.LK_JiaoPool_Minor) + "</color>").ColorReplace();
							result = false;
						}
						else
						{
							bool flag6 = !jiao.CanBreed;
							if (flag6)
							{
								reason = ("<color=#brightred>" + LocalStringManager.Get(LanguageKey.LK_JiaoPool_CannotBreed) + "</color>").ColorReplace();
								result = false;
							}
							else
							{
								bool flag7 = this._breedingJiaoList.Contains(jiao.Id);
								if (flag7)
								{
									reason = ("<color=#brightred>" + LocalStringManager.Get(LanguageKey.LK_JiaoPool_CannotBreed) + "</color>").ColorReplace();
									result = false;
								}
								else
								{
									result = true;
								}
							}
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06001A65 RID: 6757 RVA: 0x000AE850 File Offset: 0x000ACA50
	private void ReplaceItemListSortFilter(ItemSortAndFilterController newController)
	{
		Type listType = typeof(ItemListScroll);
		FieldInfo field = listType.GetField("_sortAndFilterController", BindingFlags.Instance | BindingFlags.NonPublic);
		SortAndFilterController<ITradeableContent> oldController = ((field != null) ? field.GetValue(this._itemListScroll) : null) as SortAndFilterController<ITradeableContent>;
		if (oldController != null)
		{
			oldController.UninitForReplace();
		}
		newController.Init(new Action(this.OnItemListSortAndFilterChanged), "UI_JiaoPoolSelectItem");
		FieldInfo field2 = listType.GetField("_sortAndFilterController", BindingFlags.Instance | BindingFlags.NonPublic);
		if (field2 != null)
		{
			field2.SetValue(this._itemListScroll, newController);
		}
		FieldInfo field3 = listType.GetField("scroll", BindingFlags.Instance | BindingFlags.NonPublic);
		ListStyleGeneralScroll rowScroll = ((field3 != null) ? field3.GetValue(this._itemListScroll) : null) as ListStyleGeneralScroll;
		bool flag = rowScroll != null;
		if (flag)
		{
			rowScroll.SetSortController(newController);
		}
		FieldInfo field4 = listType.GetField("cardScroll", BindingFlags.Instance | BindingFlags.NonPublic);
		CardStyleGeneralScroll cardScroll = ((field4 != null) ? field4.GetValue(this._itemListScroll) : null) as CardStyleGeneralScroll;
		bool flag2 = cardScroll != null;
		if (flag2)
		{
			cardScroll.SetSortController(newController);
		}
	}

	// Token: 0x06001A66 RID: 6758 RVA: 0x000AE940 File Offset: 0x000ACB40
	private void OnItemListSortAndFilterChanged()
	{
		this._itemListScroll.SetItemList(this._displayItems, -1);
		this.OnSortAndFilterChanged();
	}

	// Token: 0x040014B8 RID: 5304
	private const string SortSaveKey = "UI_JiaoPoolSelectItem";

	// Token: 0x040014B9 RID: 5305
	private const BindingFlags ItemListNonPublic = BindingFlags.Instance | BindingFlags.NonPublic;

	// Token: 0x040014BA RID: 5306
	[SerializeField]
	private float animaTime;

	// Token: 0x040014BB RID: 5307
	private RectTransform _itemPopUp;

	// Token: 0x040014BC RID: 5308
	private JiaoPoolPopUpType _popUpType;

	// Token: 0x040014BD RID: 5309
	private ItemKey _currentItemKey;

	// Token: 0x040014BE RID: 5310
	private ItemKey _initSelectItemKey;

	// Token: 0x040014BF RID: 5311
	private Action<int, ItemKey> _onConfirm;

	// Token: 0x040014C0 RID: 5312
	private readonly List<Jiao> _allJiaoData = new List<Jiao>();

	// Token: 0x040014C1 RID: 5313
	private readonly List<ItemDisplayData> _currentAllItemDisplayData = new List<ItemDisplayData>();

	// Token: 0x040014C2 RID: 5314
	private readonly List<ItemDisplayData> _displayItems = new List<ItemDisplayData>();

	// Token: 0x040014C3 RID: 5315
	private readonly List<int> _breedingJiaoList = new List<int>();

	// Token: 0x040014C4 RID: 5316
	private readonly Dictionary<ItemKey, Jiao> _jiaoByKey = new Dictionary<ItemKey, Jiao>();

	// Token: 0x040014C5 RID: 5317
	private int poolId;

	// Token: 0x040014C6 RID: 5318
	private Jiao _currentPoolJiao;

	// Token: 0x040014C7 RID: 5319
	private ItemKey _currentPoolJiaoKey;

	// Token: 0x040014C8 RID: 5320
	private CButton sureBtn;

	// Token: 0x040014C9 RID: 5321
	private ItemListScroll _itemListScroll;

	// Token: 0x040014CA RID: 5322
	private bool _isCurrentLocatedInIndustryBlock;
}
