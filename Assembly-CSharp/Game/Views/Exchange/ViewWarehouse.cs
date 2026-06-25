using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Views.Record;
using GameData.Domains.Building;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using GameData.Domains.Information;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Domains.Taiwu.ExchangeSystem;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Exchange
{
	// Token: 0x02000A36 RID: 2614
	public class ViewWarehouse : ViewExchangeBase
	{
		// Token: 0x17000E18 RID: 3608
		// (get) Token: 0x060080EF RID: 33007 RVA: 0x003C0361 File Offset: 0x003BE561
		protected override string SortSaveKeyPrefix
		{
			get
			{
				return "ViewWarehouse";
			}
		}

		// Token: 0x17000E19 RID: 3609
		// (get) Token: 0x060080F0 RID: 33008 RVA: 0x003C0368 File Offset: 0x003BE568
		private int TransferType
		{
			get
			{
				return (int)((ushort)this.exchangeContainer.currPage.GetActiveIndex()) << 16 | (int)((ushort)this.exchangeContainer.targetPage.GetActiveIndex());
			}
		}

		// Token: 0x17000E1A RID: 3610
		// (get) Token: 0x060080F1 RID: 33009 RVA: 0x003C0390 File Offset: 0x003BE590
		public override Exchange Exchange
		{
			get
			{
				return this._cache.Exchange;
			}
		}

		// Token: 0x17000E1B RID: 3611
		// (get) Token: 0x060080F2 RID: 33010 RVA: 0x003C039D File Offset: 0x003BE59D
		public override SecretInformationDisplayPackage SecretInformationDisplayPackage
		{
			get
			{
				return this._cache.SecretInformationDisplayPackage;
			}
		}

		// Token: 0x17000E1C RID: 3612
		// (get) Token: 0x060080F3 RID: 33011 RVA: 0x003C03AA File Offset: 0x003BE5AA
		public override CharacterDisplayData TargetDisplayData
		{
			get
			{
				return this._cache.TargetCharacterDisplayData;
			}
		}

		// Token: 0x17000E1D RID: 3613
		// (get) Token: 0x060080F4 RID: 33012 RVA: 0x003C03B7 File Offset: 0x003BE5B7
		public override CharacterDisplayData TaiwuDisplayData
		{
			get
			{
				return this._cache.TaiwuDisplayData;
			}
		}

		// Token: 0x17000E1E RID: 3614
		// (get) Token: 0x060080F5 RID: 33013 RVA: 0x003C03C4 File Offset: 0x003BE5C4
		protected override ItemListScroll.EColumnType ColumnType
		{
			get
			{
				return ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value;
			}
		}

		// Token: 0x060080F6 RID: 33014 RVA: 0x003C03C9 File Offset: 0x003BE5C9
		protected override void AwakeBack()
		{
			this.exchangeContainer.exchangeBack.SetTreasury(0);
			this.exchangeContainer.exchangeFront.SetTreasury(0);
		}

		// Token: 0x060080F7 RID: 33015 RVA: 0x003C03F0 File Offset: 0x003BE5F0
		protected override void AwakePager()
		{
			this.exchangeContainer.currPage.Init(-1);
			this.exchangeContainer.currPage.OnActiveIndexChange += delegate(int newTog, int _)
			{
				this.SetTitle(this.exchangeContainer.titleTaiwu, newTog);
				this.exchangeContainer.targetPage.SetInteractable(true);
				this.exchangeContainer.targetPage.SetInteractable(false, newTog);
				this.SetToggleTextColors(this.exchangeContainer.targetPage);
				bool flag = this.Exchange.ExchangeItemList.Count > 0;
				if (flag)
				{
					this.ExtraScrollToTopAction = 1;
					this.Submit();
					this.exchangeContainer.selfItemList.SetItemList(new List<ITradeableContent>());
				}
				else
				{
					this.Exchange.CharId = this.TransferType;
					this.RefreshSelfItems(newTog);
					this.RefreshButtons();
					this.RefreshValues();
					this.exchangeContainer.selfItemList.InfiniteScroll.ScrollTo(0, 0.3f);
				}
			};
			this.exchangeContainer.targetPage.Init(-1);
			this.exchangeContainer.targetPage.OnActiveIndexChange += delegate(int newTog, int _)
			{
				this.SetTitle(this.exchangeContainer.titleTarget, newTog);
				this.exchangeContainer.currPage.SetInteractable(true);
				this.exchangeContainer.currPage.SetInteractable(false, newTog);
				this.SetToggleTextColors(this.exchangeContainer.currPage);
				bool flag = this.Exchange.ExchangeItemList.Count > 0;
				if (flag)
				{
					this.ExtraScrollToTopAction = 2;
					this.Submit();
					this.exchangeContainer.targetItemList.SetItemList(new List<ITradeableContent>());
				}
				else
				{
					this.Exchange.CharId = this.TransferType;
					this.RefreshTargetItems(newTog);
					this.RefreshButtons();
					this.RefreshValues();
					this.exchangeContainer.targetItemList.InfiniteScroll.ScrollTo(0, 0.3f);
				}
			};
			this.SetTitle(this.exchangeContainer.titleTaiwu, this.exchangeContainer.currPage.GetActiveIndex());
			this.SetTitle(this.exchangeContainer.titleTarget, this.exchangeContainer.targetPage.GetActiveIndex());
		}

		// Token: 0x060080F8 RID: 33016 RVA: 0x003C04A0 File Offset: 0x003BE6A0
		protected override void AwakeButtons()
		{
			base.AwakeButtons();
			this.openItemRecord.onClick.ResetListener(delegate()
			{
				UIElement.TaiwuVillageStoragesRecord.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("StorageType", (TaiwuVillageStorageType)ViewTaiwuStorageRecord.GetStorageType(this.exchangeContainer.targetPage.GetActiveIndex() - 1)));
				UIManager.Instance.MaskUI(UIElement.TaiwuVillageStoragesRecord);
			});
			this.openItemTake.onClick.ResetListener(delegate()
			{
				UIElement.TaiwuVillagerNeedItem.SetOnInitArgs(EasyPool.Get<ArgumentBox>());
				UIManager.Instance.ShowUI(UIElement.TaiwuVillagerNeedItem, true);
			});
		}

		// Token: 0x060080F9 RID: 33017 RVA: 0x003C0504 File Offset: 0x003BE704
		protected override bool CheckItemCanTransfer(ITradeableContent itemData, CharacterDisplayData cd)
		{
			return (this.exchangeContainer.targetPage.GetActiveIndex() == 4) ? GameData.Domains.Building.SharedMethods.CheckItemCanFeedChicken(itemData.Key) : (this._cache.CanTransferItemToWarehouse && ((this.exchangeContainer.targetPage.GetActiveIndex() == 2 && this.exchangeContainer.currPage.GetActiveIndex() == 0) || !ItemTemplateHelper.IsMiscResource(itemData.Key.ItemType, itemData.Key.TemplateId)) && base.CheckItemCanTransfer(itemData, cd));
		}

		// Token: 0x060080FA RID: 33018 RVA: 0x003C058B File Offset: 0x003BE78B
		public override void Submit()
		{
			TaiwuDomainMethod.Call.ConfirmExchange(this.Exchange);
			GEvent.OnEvent(UiEvents.OnRefreshCharacterMenuItem, null);
			this.Reset();
		}

		// Token: 0x060080FB RID: 33019 RVA: 0x003C05B2 File Offset: 0x003BE7B2
		private void OnEnable()
		{
			GEvent.Add(UiEvents.ItemMultiplyOperationFinish, new GEvent.Callback(this.ItemMultiplyOperationFinish));
		}

		// Token: 0x060080FC RID: 33020 RVA: 0x003C05CE File Offset: 0x003BE7CE
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.ItemMultiplyOperationFinish, new GEvent.Callback(this.ItemMultiplyOperationFinish));
		}

		// Token: 0x060080FD RID: 33021 RVA: 0x003C05EC File Offset: 0x003BE7EC
		public override void OnInit(ArgumentBox argsBox)
		{
			this.ShowNonInteractableGoods = true;
			this.SetWarehouseCanInteract(true);
			ItemSourceType source;
			bool flag = argsBox == null || !argsBox.Get<ItemSourceType>("WarehouseItemSourceType", out source);
			if (flag)
			{
				source = ItemSourceType.Warehouse;
			}
			bool taiwuFunctionAvailable = source != ItemSourceType.Trough && SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10);
			this.openItemRecord.gameObject.SetActive(taiwuFunctionAvailable);
			this.openItemTake.gameObject.SetActive(taiwuFunctionAvailable);
			bool flag2 = source == ItemSourceType.Trough;
			if (flag2)
			{
				this.weightSummary.gameObject.SetActive(false);
				this.exchangeContainer.targetPage.Get(1).gameObject.SetActive(false);
				this.exchangeContainer.targetPage.Get(2).gameObject.SetActive(false);
				this.exchangeContainer.targetPage.Get(3).gameObject.SetActive(false);
				this.exchangeContainer.targetPage.Get(4).gameObject.SetActive(true);
			}
			else
			{
				this.weightSummary.gameObject.SetActive(true);
				this.exchangeContainer.targetPage.Get(1).gameObject.SetActive(true);
				this.exchangeContainer.targetPage.Get(2).gameObject.SetActive(taiwuFunctionAvailable);
				this.exchangeContainer.targetPage.Get(3).gameObject.SetActive(taiwuFunctionAvailable);
				this.exchangeContainer.targetPage.Get(4).gameObject.SetActive(false);
			}
			this.exchangeContainer.currPage.SetWithoutNotify(0);
			CToggleGroup targetPage = this.exchangeContainer.targetPage;
			if (!true)
			{
			}
			int withoutNotify;
			switch (source)
			{
			case ItemSourceType.Warehouse:
				withoutNotify = 1;
				goto IL_1D5;
			case ItemSourceType.Treasury:
				withoutNotify = 2;
				goto IL_1D5;
			case ItemSourceType.Stock:
				withoutNotify = 3;
				goto IL_1D5;
			}
			withoutNotify = 4;
			IL_1D5:
			if (!true)
			{
			}
			targetPage.SetWithoutNotify(withoutNotify);
			this.exchangeContainer.targetPage.SetInteractable(true);
			this.exchangeContainer.currPage.SetInteractable(true);
			this.exchangeContainer.currPage.SetInteractable(false, this.exchangeContainer.targetPage.GetActiveIndex());
			this.exchangeContainer.AddSwitchToggleListener();
			this.SetToggleTextColors(this.exchangeContainer.currPage);
			this.SetToggleTextColors(this.exchangeContainer.currPage);
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
			{
				TaiwuDomainMethod.AsyncCall.GetTreasuryNeededItemDataList(this, delegate(int offset, RawDataPool dataPool)
				{
					List<ItemNeedDisplayData> cachedData = new List<ItemNeedDisplayData>();
					Serializer.Deserialize(dataPool, offset, ref cachedData);
					this._cachedData.Clear();
					foreach (ItemNeedDisplayData item in cachedData)
					{
						List<ItemNeedCharacterDisplayData> list = this._cachedData.GetValueOrDefault(new ValueTuple<sbyte, short>(item.ItemDisplayData.Key.ItemType, item.ItemDisplayData.Key.TemplateId));
						bool flag4 = list == null;
						if (flag4)
						{
							list = new List<ItemNeedCharacterDisplayData>();
							this._cachedData[new ValueTuple<sbyte, short>(item.ItemDisplayData.Key.ItemType, item.ItemDisplayData.Key.TemplateId)] = list;
						}
						bool flag5 = item.CharacterDisplayDataList != null;
						if (flag5)
						{
							list.AddRange(item.CharacterDisplayDataList);
						}
					}
				});
			}));
			base.OnInit(argsBox);
			bool flag3 = !this._firstEnter;
			if (flag3)
			{
				this._firstEnter = true;
				GlobalDomainMethod.Call.InvokeGuidingTrigger(130);
			}
		}

		// Token: 0x060080FE RID: 33022 RVA: 0x003C08A4 File Offset: 0x003BEAA4
		protected override void RequestData()
		{
			int tr = this.TransferType;
			TaiwuDomainMethod.AsyncCall.GetExchangeDisplayData(this, this.TransferType, EExchangeType.Warehouse, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._cache);
				this.SetExchangeData();
				this.RefreshSelfColumnDefinitions(this.exchangeContainer.currPage.GetActiveIndex());
				this.Refresh();
				this.Element.ShowAfterRefresh();
			});
		}

		// Token: 0x060080FF RID: 33023 RVA: 0x003C08D3 File Offset: 0x003BEAD3
		private void ItemMultiplyOperationFinish(ArgumentBox _)
		{
			this.RequestData();
		}

		// Token: 0x06008100 RID: 33024 RVA: 0x003C08E0 File Offset: 0x003BEAE0
		protected override void RefreshValues()
		{
			this.exchangeContainer.selfItemList.InfiniteScroll.ReRender();
			this.exchangeContainer.targetItemList.InfiniteScroll.ReRender();
			this.exchangeContainer.confirmButtonText.text = ((this.exchangeContainer.confirm.interactable && this.Exchange.TargetContentList.Count == 0) ? LanguageKey.LK_Exchange_Confirm_As_Gift.Tr() : LanguageKey.LK_Exchange_Confirm.Tr());
			bool flag = this.exchangeContainer.targetPage.GetActiveIndex() == 4;
			if (flag)
			{
				this.exchangeContainer.targetValue1.text = this.<RefreshValues>g__GetBaseWeight|30_0(this.exchangeContainer.targetPage.GetActiveIndex(), this.Exchange.TargetCurWeightChange);
				this.exchangeContainer.selfValue1.text = this.<RefreshValues>g__GetBaseWeight|30_0(this.exchangeContainer.currPage.GetActiveIndex(), -this.Exchange.TargetCurWeightChange);
			}
			else
			{
				int activeIndex = this.exchangeContainer.targetPage.GetActiveIndex();
				if (!true)
				{
				}
				ValueTuple<int, int> valueTuple;
				switch (activeIndex)
				{
				case 1:
					valueTuple = new ValueTuple<int, int>(this._cache.WarehouseWeight, this.Exchange.TaiwuWarehouseMaxLoad - this._cache.TreasuryWeight - this._cache.StockWeight);
					break;
				case 2:
					valueTuple = new ValueTuple<int, int>(this._cache.TreasuryWeight, this.Exchange.TaiwuWarehouseMaxLoad - this._cache.WarehouseWeight - this._cache.StockWeight);
					break;
				case 3:
					valueTuple = new ValueTuple<int, int>(this._cache.StockWeight, this.Exchange.TaiwuWarehouseMaxLoad - this._cache.WarehouseWeight - this._cache.TreasuryWeight);
					break;
				default:
					valueTuple = new ValueTuple<int, int>(this.Exchange.TaiwuInventoryCurLoad, this.Exchange.TaiwuInventoryMaxLoad);
					break;
				}
				if (!true)
				{
				}
				ValueTuple<int, int> valueTuple2 = valueTuple;
				int targetBase = valueTuple2.Item1;
				int targetMaxBase = valueTuple2.Item2;
				int activeIndex2 = this.exchangeContainer.currPage.GetActiveIndex();
				if (!true)
				{
				}
				switch (activeIndex2)
				{
				case 1:
					valueTuple = new ValueTuple<int, int>(this._cache.WarehouseWeight, this.Exchange.TaiwuWarehouseMaxLoad - this._cache.TreasuryWeight - this._cache.StockWeight);
					break;
				case 2:
					valueTuple = new ValueTuple<int, int>(this._cache.TreasuryWeight, this.Exchange.TaiwuWarehouseMaxLoad - this._cache.WarehouseWeight - this._cache.StockWeight);
					break;
				case 3:
					valueTuple = new ValueTuple<int, int>(this._cache.StockWeight, this.Exchange.TaiwuWarehouseMaxLoad - this._cache.WarehouseWeight - this._cache.TreasuryWeight);
					break;
				default:
					valueTuple = new ValueTuple<int, int>(this.Exchange.TaiwuInventoryCurLoad, this.Exchange.TaiwuInventoryMaxLoad);
					break;
				}
				if (!true)
				{
				}
				ValueTuple<int, int> valueTuple3 = valueTuple;
				int selfBase = valueTuple3.Item1;
				int selfMaxBase = valueTuple3.Item2;
				targetBase += this.Exchange.TargetCurWeightChange;
				selfBase -= this.Exchange.TargetCurWeightChange;
				int activeIndex3 = this.exchangeContainer.currPage.GetActiveIndex();
				bool flag2;
				if (activeIndex3 == 1 || activeIndex3 == 2 || activeIndex3 == 3)
				{
					activeIndex3 = this.exchangeContainer.targetPage.GetActiveIndex();
					flag2 = (activeIndex3 == 1 || activeIndex3 == 2 || activeIndex3 == 3);
				}
				else
				{
					flag2 = false;
				}
				bool flag3 = flag2;
				if (flag3)
				{
					targetMaxBase += this.Exchange.TargetCurWeightChange;
					selfMaxBase -= this.Exchange.TargetCurWeightChange;
				}
				this.totalValue.text = ((this.exchangeContainer.currPage.GetActiveIndex() != 0) ? ViewExchangeBase.GetWeightString(this.Exchange.TaiwuWarehouseCurLoad, this.Exchange.TaiwuWarehouseMaxLoad, this.Exchange.TaiwuWarehouseCurLoad, this.Exchange.TaiwuWarehouseMaxLoad, LanguageKey.LK_Exchange_Taiwu_Treasury_Weight_Value) : ViewExchangeBase.GetWeightString(this.Exchange.TaiwuWarehouseCurLoad + this.Exchange.TargetCurWeightChange, this.Exchange.TaiwuWarehouseMaxLoad, this.Exchange.TaiwuWarehouseCurLoad + this.Exchange.TargetCurWeightChange, this.Exchange.TaiwuWarehouseMaxLoad, LanguageKey.LK_Exchange_Taiwu_Treasury_Weight_Value));
				this.exchangeContainer.targetValue1.text = LanguageKey.LK_Exchange_Weight_Warehouse_Value.TrFormat(string.Format("{0}", (float)targetBase / 100f).SetColor(CommonUtils.GetLoadWeightValueColor(this.Exchange.TaiwuWarehouseCurLoad + this.Exchange.TargetCurWeightChange, this.Exchange.TaiwuWarehouseMaxLoad)));
				this.exchangeContainer.selfValue1.text = ((this.exchangeContainer.currPage.GetActiveIndex() != 0) ? ViewExchangeBase.GetWarehouseWeightString(selfBase) : ViewExchangeBase.GetWeightString(selfBase, selfMaxBase, selfBase, selfMaxBase, LanguageKey.LK_Exchange_Weight_Value));
			}
		}

		// Token: 0x06008101 RID: 33025 RVA: 0x003C0DD8 File Offset: 0x003BEFD8
		public override IReadOnlyList<ITradeableContent> GetTargetTradeableList(int index)
		{
			if (!true)
			{
			}
			List<ItemDisplayData> list;
			switch (index)
			{
			case 0:
				list = this._cache.TaiwuInventoryItemDisplayDataList;
				break;
			case 1:
				list = this._cache.TaiwuWarehouseItemDisplayDataList;
				break;
			case 2:
				list = this._cache.TaiwuTreasuryItemDisplayDataList;
				break;
			case 3:
				list = this._cache.TaiwuStockItemDisplayDataList;
				break;
			case 4:
				list = this._cache.TaiwuTroughItemDisplayDataList;
				break;
			default:
				throw new ArgumentException();
			}
			if (!true)
			{
			}
			return (list ?? new List<ItemDisplayData>()).Concat(this.Exchange.TaiwuContentItemList ?? new List<ItemDisplayData>()).ToList<ItemDisplayData>();
		}

		// Token: 0x06008102 RID: 33026 RVA: 0x003C0E80 File Offset: 0x003BF080
		public override IReadOnlyList<ITradeableContent> GetSelfTradeableList(int index)
		{
			if (!true)
			{
			}
			List<ItemDisplayData> list;
			switch (index)
			{
			case 0:
				list = this._cache.TaiwuInventoryItemDisplayDataList;
				break;
			case 1:
				list = this._cache.TaiwuWarehouseItemDisplayDataList;
				break;
			case 2:
				list = this._cache.TaiwuTreasuryItemDisplayDataList;
				break;
			default:
				list = this._cache.TaiwuStockItemDisplayDataList;
				break;
			}
			if (!true)
			{
			}
			return (list ?? new List<ItemDisplayData>()).Concat(this.Exchange.TargetContentItemList ?? new List<ItemDisplayData>()).ToList<ItemDisplayData>();
		}

		// Token: 0x17000E1F RID: 3615
		// (get) Token: 0x06008103 RID: 33027 RVA: 0x003C0F09 File Offset: 0x003BF109
		protected override string ConfirmOnHideDesc
		{
			get
			{
				return LanguageKey.LK_Exchange_Confirm_Cancel.Tr();
			}
		}

		// Token: 0x17000E20 RID: 3616
		// (get) Token: 0x06008104 RID: 33028 RVA: 0x003C0F15 File Offset: 0x003BF115
		protected override bool ShouldConfirmHide
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06008105 RID: 33029 RVA: 0x003C0F18 File Offset: 0x003BF118
		public override void QuickHideImpl()
		{
			bool flag = this.Exchange.ExchangeItemList.Count > 0;
			if (flag)
			{
				TaiwuDomainMethod.Call.ConfirmExchange(this.Exchange);
			}
			base.QuickHideImpl();
		}

		// Token: 0x06008106 RID: 33030 RVA: 0x003C0F50 File Offset: 0x003BF150
		public override void PrepareCustomRowTemplateContainers(RowItem rowTemplate, bool isItemMode)
		{
			Transform containerRoot = rowTemplate.ContainerRoot;
			Object.Instantiate<RowCellContainer>(this.exchangeContainer.itemIconAndNameCellContainer, containerRoot).gameObject.SetActive(true);
			Object.Instantiate<RowCellContainer>(this.exchangeContainer.singleTextCellContainer, containerRoot).gameObject.SetActive(true);
			Object.Instantiate<RowCellContainer>(this.exchangeContainer.singleTextCellContainer, containerRoot).gameObject.SetActive(true);
			Object.Instantiate<RowCellContainer>(this.exchangeContainer.singleTextCellContainer, containerRoot).gameObject.SetActive(true);
			Object.Instantiate<RowCellContainer>(this.exchangeContainer.singleTextCellContainer, containerRoot).gameObject.SetActive(true);
		}

		// Token: 0x06008107 RID: 33031 RVA: 0x003C0FF6 File Offset: 0x003BF1F6
		public override IEnumerable<ColumnDefinition> GenerateColumnDefinitions(ItemListScroll scroll, bool isItemMode)
		{
			LayoutOption option = default(LayoutOption);
			yield return scroll.ColumnIconAndName(option, false);
			option = new LayoutOption(122f, 1f, 122f, 1);
			yield return scroll.ColumnAmount(option, true);
			yield return scroll.ColumnSubType(option, true);
			yield return scroll.ColumnDurability(option, true);
			yield return scroll.ColumnValue(option, true, LanguageKey.LK_ItemValue);
			yield break;
			yield break;
		}

		// Token: 0x06008108 RID: 33032 RVA: 0x003C1014 File Offset: 0x003BF214
		public List<ItemNeedCharacterDisplayData> GetNeedList(ITradeableContent data)
		{
			return this._cachedData.GetValueOrDefault(new ValueTuple<sbyte, short>(data.Key.ItemType, data.Key.TemplateId));
		}

		// Token: 0x06008109 RID: 33033 RVA: 0x003C103C File Offset: 0x003BF23C
		protected override RowItemMain OnItemRender(ITradeableContent itemData, RowItemLine rowItemLine, CharacterDisplayData displayData)
		{
			RowItemMain rowItemMain = base.OnItemRender(itemData, rowItemLine, displayData);
			RowItemMain rowItemMain2 = rowItemMain;
			int? num = (displayData != null) ? new int?(displayData.CharacterId) : null;
			int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			bool isShow;
			if (((num.GetValueOrDefault() == taiwuCharId & num != null) ? this.exchangeContainer.currPage : this.exchangeContainer.targetPage).GetActiveIndex() == 2)
			{
				List<ItemNeedCharacterDisplayData> needList = this.GetNeedList(itemData);
				isShow = (needList != null && needList.Count > 0);
			}
			else
			{
				isShow = false;
			}
			rowItemMain2.ShowVillagerNeedItem(isShow);
			bool flag = !itemData.IsResource;
			if (flag)
			{
				RowItemLine.SetMouseTipDisplayer(true, itemData, rowItemLine.TipDisplayer);
			}
			else
			{
				RowItemLine.SetResourceTip(itemData, rowItemLine.TipDisplayer, SingletonObject.getInstance<BasicGameData>().TaiwuMonasticTitleOrDisplayName, true, true);
			}
			return rowItemMain;
		}

		// Token: 0x0600810A RID: 33034 RVA: 0x003C1114 File Offset: 0x003BF314
		protected override void SetItemLockedStatus(ITradeableContent itemData, CharacterDisplayData cd, RowItemLine rowItemLine, RowItemMain main)
		{
			bool isLocked = itemData.IsLocked;
			if (isLocked)
			{
				bool interactable = this.exchangeContainer.targetPage.GetActiveIndex() <= 1 && this.exchangeContainer.currPage.GetActiveIndex() <= 1;
				rowItemLine.SetInteractable(interactable, true);
				rowItemLine.SetDisabled(!interactable);
			}
			else
			{
				base.SetItemLockedStatus(itemData, cd, rowItemLine, main);
			}
		}

		// Token: 0x0600810B RID: 33035 RVA: 0x003C117C File Offset: 0x003BF37C
		protected override bool CheckItemIsLocked(ITradeableContent itemData, CharacterDisplayData cd, out string reason)
		{
			bool canTransferItemToWarehouse = this._cache.CanTransferItemToWarehouse;
			bool result;
			if (canTransferItemToWarehouse)
			{
				result = base.CheckItemIsLocked(itemData, cd, out reason);
			}
			else
			{
				reason = LanguageKey.LK_Item_Operation_FarAway.Tr();
				result = true;
			}
			return result;
		}

		// Token: 0x0600810C RID: 33036 RVA: 0x003C11B8 File Offset: 0x003BF3B8
		private void SetToggleTextColors(CToggleGroup toggleGroup)
		{
			for (int i = 0; i < toggleGroup.Count(); i++)
			{
				CToggle toggle = toggleGroup.Get(i);
				TMP_Text child = toggle.GetComponentInChildren<TMP_Text>();
				child.color = Colors.Instance[toggle.interactable ? "pinkyellow" : "grey"];
			}
		}

		// Token: 0x06008114 RID: 33044 RVA: 0x003C15A4 File Offset: 0x003BF7A4
		[CompilerGenerated]
		private string <RefreshValues>g__GetBaseWeight|30_0(int toggle, int change)
		{
			if (!true)
			{
			}
			string weightString;
			if (toggle != 0)
			{
				if (toggle != 4)
				{
					weightString = ViewExchangeBase.GetWeightString(this.Exchange.TaiwuWarehouseCurLoad + change, this.Exchange.TaiwuWarehouseMaxLoad, this.Exchange.TaiwuWarehouseCurLoad + change, this.Exchange.TaiwuWarehouseMaxLoad, LanguageKey.LK_Exchange_Weight_Value);
				}
				else
				{
					weightString = ViewExchangeBase.GetWeightString(this.Exchange.TargetCurLoad + change, this.Exchange.TargetMaxLoad, this.Exchange.TargetCurLoad + change, this.Exchange.TargetMaxLoad, LanguageKey.LK_Exchange_Weight_Value);
				}
			}
			else
			{
				weightString = ViewExchangeBase.GetWeightString(this.Exchange.TaiwuInventoryCurLoad + change, this.Exchange.TaiwuInventoryMaxLoadPreview, this.Exchange.TaiwuInventoryCurLoad + change, this.Exchange.TaiwuInventoryMaxLoadPreview, LanguageKey.LK_Exchange_Weight_Value);
			}
			if (!true)
			{
			}
			return weightString;
		}

		// Token: 0x0400627B RID: 25211
		[SerializeField]
		private CButton openItemRecord;

		// Token: 0x0400627C RID: 25212
		[SerializeField]
		private CButton openItemTake;

		// Token: 0x0400627D RID: 25213
		[SerializeField]
		private TMP_Text totalValue;

		// Token: 0x0400627E RID: 25214
		[SerializeField]
		private GameObject weightSummary;

		// Token: 0x0400627F RID: 25215
		private ExchangeDisplayData _cache;

		// Token: 0x04006280 RID: 25216
		private bool _firstEnter;

		// Token: 0x04006281 RID: 25217
		private Dictionary<ValueTuple<sbyte, short>, List<ItemNeedCharacterDisplayData>> _cachedData = new Dictionary<ValueTuple<sbyte, short>, List<ItemNeedCharacterDisplayData>>();
	}
}
