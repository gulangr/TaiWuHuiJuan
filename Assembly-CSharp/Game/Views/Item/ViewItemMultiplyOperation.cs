using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Item;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter.Item.Apply;
using Game.Views.Exchange;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Item
{
	// Token: 0x02000A15 RID: 2581
	public class ViewItemMultiplyOperation : UIBase
	{
		// Token: 0x17000DB9 RID: 3513
		// (get) Token: 0x06007E41 RID: 32321 RVA: 0x003AA516 File Offset: 0x003A8716
		private int TaiwuCharId
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x17000DBA RID: 3514
		// (get) Token: 0x06007E42 RID: 32322 RVA: 0x003AA522 File Offset: 0x003A8722
		private EItemSourceToggleKey CurTogKey
		{
			get
			{
				return (EItemSourceToggleKey)this.toggleGroupItemSource.GetActiveIndex();
			}
		}

		// Token: 0x06007E43 RID: 32323 RVA: 0x003AA52F File Offset: 0x003A872F
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get<EItemSourceToggleKey>("InitTogKey", out this._initTogKey);
			this.NeedWaitData = true;
			this.RequestData();
			base.gameObject.GetOrAddComponent<CanvasGroup>().alpha = 0f;
		}

		// Token: 0x06007E44 RID: 32324 RVA: 0x003AA568 File Offset: 0x003A8768
		private void RequestData()
		{
			TaiwuDomainMethod.AsyncCall.GetTaiwuItemMultiplyOperationDisplayData(this, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._displayData);
				this._itemDict.Clear();
				this._itemDict.Add(ItemSourceType.Inventory, this._displayData.InventoryItems ?? new List<ItemDisplayData>());
				this._itemDict.Add(ItemSourceType.Warehouse, this._displayData.WarehouseItems ?? new List<ItemDisplayData>());
				this._itemDict.Add(ItemSourceType.Treasury, this._displayData.TreasuryItems ?? new List<ItemDisplayData>());
				this._itemDict.Add(ItemSourceType.Stock, this._displayData.StockItems ?? new List<ItemDisplayData>());
				this.toggleGroupItemSource.Set(this._initTogKey.ToInt(), true);
				base.gameObject.GetOrAddComponent<CanvasGroup>().alpha = 1f;
			});
		}

		// Token: 0x06007E45 RID: 32325 RVA: 0x003AA580 File Offset: 0x003A8780
		private void Awake()
		{
			this.toggleGroupItemSource.Init(-1);
			this.toggleGroupItemSource.OnActiveIndexChange += this.ToggleGroupItemSourceOnActiveIndexChange;
			this.targetToggleGroup.Init(0);
			this.targetToggleGroup.OnActiveIndexChange += this.OnTargetCardModeChange;
			this.itemListScroll.CustomAmountDataGenerator = new Func<ITradeableContent, string>(this.AmountCellDataGenerator);
			this.itemListScroll.Init("ViewItemMultiplyOperationItemListScroll", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnRenderItem), null, ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability, null, null, null);
			this.selectedItemListScroll.Init("ViewItemMultiplyOperationSelectedItemListScroll", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnRenderItem), null, ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability, null, null, null);
			this.buttonClose.ClearAndAddListener(new Action(this.QuickHide));
		}

		// Token: 0x06007E46 RID: 32326 RVA: 0x003AA654 File Offset: 0x003A8854
		private void OnDestroy()
		{
			this.toggleGroupItemSource.OnActiveIndexChange -= this.ToggleGroupItemSourceOnActiveIndexChange;
			this.targetToggleGroup.OnActiveIndexChange -= this.OnTargetCardModeChange;
		}

		// Token: 0x06007E47 RID: 32327 RVA: 0x003AA688 File Offset: 0x003A8888
		private void OnEnable()
		{
			GEvent.Add(UiEvents.ItemMultiplyOperationTypeChange, new GEvent.Callback(this.ItemMultiplyOperationTypeChange));
			GEvent.Add(UiEvents.ItemMultiplyOperationTargetChange, new GEvent.Callback(this.ItemMultiplyOperationTargetChange));
			GEvent.Add(UiEvents.ItemMultiplyOperationConfirm, new GEvent.Callback(this.multiplyItemListScroll.OnItemMultiplyOperationConfirm));
			GEvent.Add(UiEvents.ItemMultiplyOperationCancelSelection, new GEvent.Callback(this.multiplyItemListScroll.OnItemMultiplyOperationCancelSelection));
			GEvent.Add(UiEvents.ItemMultiplyOperationFinish, new GEvent.Callback(this.ItemMultiplyOperationFinish));
		}

		// Token: 0x06007E48 RID: 32328 RVA: 0x003AA720 File Offset: 0x003A8920
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.ItemMultiplyOperationTypeChange, new GEvent.Callback(this.ItemMultiplyOperationTypeChange));
			GEvent.Remove(UiEvents.ItemMultiplyOperationTargetChange, new GEvent.Callback(this.ItemMultiplyOperationTargetChange));
			GEvent.Remove(UiEvents.ItemMultiplyOperationConfirm, new GEvent.Callback(this.multiplyItemListScroll.OnItemMultiplyOperationConfirm));
			GEvent.Remove(UiEvents.ItemMultiplyOperationCancelSelection, new GEvent.Callback(this.multiplyItemListScroll.OnItemMultiplyOperationCancelSelection));
			GEvent.Remove(UiEvents.ItemMultiplyOperationFinish, new GEvent.Callback(this.ItemMultiplyOperationFinish));
		}

		// Token: 0x06007E49 RID: 32329 RVA: 0x003AA7B8 File Offset: 0x003A89B8
		private void ToggleGroupItemSourceOnActiveIndexChange(int newIndex, int oldIndex)
		{
			this._initTogKey = (EItemSourceToggleKey)newIndex;
			MultiplyItemListScroll multiplyItemListScroll = this.multiplyItemListScroll;
			EItemSourceToggleKey initTogKey = this._initTogKey;
			if (!true)
			{
			}
			ItemSourceType itemSourceType;
			switch (initTogKey)
			{
			case EItemSourceToggleKey.Inventory:
				itemSourceType = ItemSourceType.Inventory;
				break;
			case EItemSourceToggleKey.Warehouse:
				itemSourceType = ItemSourceType.Warehouse;
				break;
			case EItemSourceToggleKey.Treasury:
				itemSourceType = ItemSourceType.Treasury;
				break;
			case EItemSourceToggleKey.Stock:
				itemSourceType = ItemSourceType.Stock;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (!true)
			{
			}
			multiplyItemListScroll.ItemSourceType = itemSourceType;
			this.Refresh();
		}

		// Token: 0x06007E4A RID: 32330 RVA: 0x003AA81F File Offset: 0x003A8A1F
		private void OnTargetCardModeChange(int previousIndex, int currentIndex)
		{
			this.itemListScroll.SwitchCardModeToggle(previousIndex, currentIndex);
			this.selectedItemListScroll.SwitchCardModeToggle(previousIndex, currentIndex);
		}

		// Token: 0x06007E4B RID: 32331 RVA: 0x003AA83E File Offset: 0x003A8A3E
		public override void QuickHide()
		{
			this.multiplyItemListScroll.TryExitMultiplyMode(delegate
			{
				base.QuickHide();
			});
		}

		// Token: 0x06007E4C RID: 32332 RVA: 0x003AA85C File Offset: 0x003A8A5C
		private void Refresh()
		{
			ItemSourceToggleHelper.RefreshInteractableForInteract(this.toggleGroupItemSource, this._displayData.CanTransferItemToWarehouse, true);
			this.InitMultiplyItemScrollView();
			this.multiplyItemListScroll.Set(this._itemDict);
			this.multiplyItemListScroll.EnterMultiplyMode();
			this.multiplyItemListScroll.RefreshItems();
			this.RefreshLoad();
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x06007E4D RID: 32333 RVA: 0x003AA8C8 File Offset: 0x003A8AC8
		private void InitMultiplyItemScrollView()
		{
			bool hasInit = this.multiplyItemListScroll.HasInit;
			if (!hasInit)
			{
				this.multiplyItemListScroll.Init(this.TaiwuCharId, this._displayData.EmptyToolKey, this._displayData.CharacterDisplayData, null, null, null, null, null);
				this.InitSwitchSelection();
				this.multiplyItemListScroll.HideMultiplySelectButton();
			}
		}

		// Token: 0x06007E4E RID: 32334 RVA: 0x003AA928 File Offset: 0x003A8B28
		private void InitSwitchSelection()
		{
			bool flag = !this.multiplyItemListScroll.SwitchSelection;
			if (!flag)
			{
				this.multiplyItemListScroll.SwitchSelection.onValueChanged.RemoveAllListeners();
				this.multiplyItemListScroll.SwitchSelection.onValueChanged.AddListener(delegate(bool isOn)
				{
					float height = isOn ? this.itemListScrollHeightSelected : this.itemListScrollHeightFull;
					this.itemListScroll.RectTransform.sizeDelta = this.itemListScroll.RectTransform.sizeDelta.SetY(height);
					this.selectedTitle.SetActive(isOn);
					this.selectedItemListScroll.gameObject.SetActive(isOn);
					this.multiplyItemListScroll.RefreshMultiplyCanOperateItems();
				});
				this.multiplyItemListScroll.SwitchSelection.isOn = false;
			}
		}

		// Token: 0x06007E4F RID: 32335 RVA: 0x003AA99C File Offset: 0x003A8B9C
		private void OnRenderItem(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, true);
			bool isLock = this.CheckItemIsLocked(itemData);
			rowItemLine.SetInteractable(!isLock, true);
			rowItemLine.SetDisabled(isLock);
			this.SetResourceItemTip(rowItemLine);
			this.multiplyItemListScroll.OnRenderItemMultiply(itemData, rowItemLine);
		}

		// Token: 0x06007E50 RID: 32336 RVA: 0x003AA9F4 File Offset: 0x003A8BF4
		private bool CheckItemIsLocked(ITradeableContent itemData)
		{
			bool isMultiItemSelect = this.multiplyItemListScroll.IsMultiItemSelect;
			if (isMultiItemSelect)
			{
				bool flag = this.multiplyItemListScroll.SelectedMultiplyItemDict.ContainsKey(itemData as ItemDisplayData);
				if (flag)
				{
					return false;
				}
			}
			else
			{
				sbyte itemType = itemData.Key.ItemType;
				bool flag2 = itemType == 8 || itemType == 7;
				if (flag2)
				{
					return false;
				}
				bool flag3 = ItemTemplateHelper.IsTianJieFuLu(itemData.Key.ItemType, itemData.Key.TemplateId);
				if (flag3)
				{
					return false;
				}
			}
			return false;
		}

		// Token: 0x06007E51 RID: 32337 RVA: 0x003AAA88 File Offset: 0x003A8C88
		private void SetResourceItemTip(RowItemLine rowItemLine)
		{
			bool flag = !rowItemLine.Data.IsResource;
			if (!flag)
			{
				string charName = NameCenter.GetMonasticTitleOrDisplayName(this._displayData.CharacterDisplayData, true);
				RowItemLine.SetResourceTip(rowItemLine.Data, rowItemLine.TipDisplayer, charName, true, false);
			}
		}

		// Token: 0x06007E52 RID: 32338 RVA: 0x003AAAD4 File Offset: 0x003A8CD4
		private void RefreshLoad()
		{
			bool flag = this.CurTogKey == EItemSourceToggleKey.Inventory;
			if (flag)
			{
				this.RefreshInventoryLoad();
			}
			else
			{
				this.RefreshWarehouseLoad();
			}
			this.RefreshOverloadMouseTips();
		}

		// Token: 0x06007E53 RID: 32339 RVA: 0x003AAB08 File Offset: 0x003A8D08
		private void RefreshInventoryLoad()
		{
			this.loadText.text = ViewExchangeBase.GetWeightString(this._displayData.CurInventoryLoad, this._displayData.MaxInventoryLoad, this._displayData.CurInventoryLoad, this._displayData.MaxInventoryLoad, LanguageKey.LK_Exchange_Weight_Value);
		}

		// Token: 0x06007E54 RID: 32340 RVA: 0x003AAB58 File Offset: 0x003A8D58
		private void RefreshWarehouseLoad()
		{
			this.loadText.text = ViewExchangeBase.GetWeightString(this._displayData.CurWarehouseLoad, this._displayData.MaxWarehouseLoad, this._displayData.CurWarehouseLoad, this._displayData.MaxWarehouseLoad, LanguageKey.LK_Exchange_Weight_Value);
		}

		// Token: 0x06007E55 RID: 32341 RVA: 0x003AABA8 File Offset: 0x003A8DA8
		private void RefreshOverloadMouseTips()
		{
			TooltipInvoker tipDisplayer = this.loadOverflowTips;
			tipDisplayer.Type = TipType.Simple;
			tipDisplayer.enabled = (this.CurTogKey == EItemSourceToggleKey.Inventory && this._displayData.CurInventoryLoad > this._displayData.MaxInventoryLoad);
			TooltipInvoker tooltipInvoker = tipDisplayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			WorldStateItem worldStateItem = WorldState.Instance[11];
			tipDisplayer.RuntimeParam.Set("arg0", worldStateItem.Name);
			string loadTipContent = LocalStringManager.GetFormat(LanguageKey.LK_Inventory_Overflow_Tips, this._displayData.MoveTimeCostPercent - 100).ColorReplace();
			tipDisplayer.RuntimeParam.Set("arg1", loadTipContent);
		}

		// Token: 0x06007E56 RID: 32342 RVA: 0x003AAC60 File Offset: 0x003A8E60
		private void ItemMultiplyOperationTypeChange(ArgumentBox argsBox)
		{
			this.multiplyItemListScroll.OnItemMultiplyOperationTypeChange(argsBox);
		}

		// Token: 0x06007E57 RID: 32343 RVA: 0x003AAC70 File Offset: 0x003A8E70
		private void ItemMultiplyOperationTargetChange(ArgumentBox argumentBox)
		{
			ItemDisplayData target;
			argumentBox.Get<ItemDisplayData>("FeedingTarget", out target);
			this.multiplyItemListScroll.SetFeedingTarget(target);
		}

		// Token: 0x06007E58 RID: 32344 RVA: 0x003AAC99 File Offset: 0x003A8E99
		private void ItemMultiplyOperationFinish(ArgumentBox argumentBox)
		{
			this.RequestData();
		}

		// Token: 0x06007E59 RID: 32345 RVA: 0x003AACA4 File Offset: 0x003A8EA4
		private string AmountCellDataGenerator(ITradeableContent content)
		{
			bool flag = !(content is ItemDisplayData);
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				ItemDisplayData selectedData = this.multiplyItemListScroll.SelectedMultiplyItemOrderedList.Find((ItemDisplayData d) => d.RealKey.Equals(content.RealKey));
				int selectedCount = 0;
				bool isSelected = selectedData != null && this.multiplyItemListScroll.SelectedMultiplyItemDict.TryGetValue(selectedData, out selectedCount) && selectedCount > 0;
				string maxAmountStr = CommonUtils.GetDisplayStringForNum(content.Amount, 100000);
				bool flag2 = !isSelected;
				if (flag2)
				{
					result = maxAmountStr;
				}
				else
				{
					string selectedAmountStr = CommonUtils.GetDisplayStringForNum(selectedCount, 100000);
					result = selectedAmountStr + "/" + maxAmountStr;
				}
			}
			return result;
		}

		// Token: 0x0400605F RID: 24671
		[SerializeField]
		private CToggleGroup toggleGroupItemSource;

		// Token: 0x04006060 RID: 24672
		[SerializeField]
		private ItemListScroll itemListScroll;

		// Token: 0x04006061 RID: 24673
		[SerializeField]
		private ItemListScroll selectedItemListScroll;

		// Token: 0x04006062 RID: 24674
		[SerializeField]
		private float itemListScrollHeightFull;

		// Token: 0x04006063 RID: 24675
		[SerializeField]
		private float itemListScrollHeightSelected;

		// Token: 0x04006064 RID: 24676
		[SerializeField]
		private MultiplyItemListScroll multiplyItemListScroll;

		// Token: 0x04006065 RID: 24677
		[SerializeField]
		private GameObject selectedTitle;

		// Token: 0x04006066 RID: 24678
		[SerializeField]
		private TextMeshProUGUI loadText;

		// Token: 0x04006067 RID: 24679
		[SerializeField]
		private TooltipInvoker loadOverflowTips;

		// Token: 0x04006068 RID: 24680
		[SerializeField]
		private CButton buttonClose;

		// Token: 0x04006069 RID: 24681
		[SerializeField]
		private CToggleGroup targetToggleGroup;

		// Token: 0x0400606A RID: 24682
		private TaiwuItemMultiplyOperationDisplayData _displayData;

		// Token: 0x0400606B RID: 24683
		private EItemSourceToggleKey _initTogKey;

		// Token: 0x0400606C RID: 24684
		private readonly Dictionary<ItemSourceType, List<ItemDisplayData>> _itemDict = new Dictionary<ItemSourceType, List<ItemDisplayData>>();
	}
}
