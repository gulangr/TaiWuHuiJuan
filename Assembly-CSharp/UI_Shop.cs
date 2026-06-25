using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using CharacterDataMonitor;
using CommonSortAndFilterLegacy;
using Config;
using FrameWork;
using Game.Components.Avatar;
using GameData.Common;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.Relation;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Merchant;
using GameData.Domains.Organization;
using GameData.Domains.Organization.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Profession;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using GameDataExtensions;
using TMPro;
using UICommon.Character;
using UnityEngine;

// Token: 0x02000302 RID: 770
public class UI_Shop : UIBase, IShopRefresh
{
	// Token: 0x170004F1 RID: 1265
	// (get) Token: 0x06002D10 RID: 11536 RVA: 0x00161E07 File Offset: 0x00160007
	LanguageKey IShopRefresh.NameId
	{
		get
		{
			return LanguageKey.LK_Shop_Refresh_Goods;
		}
	}

	// Token: 0x170004F2 RID: 1266
	// (get) Token: 0x06002D11 RID: 11537 RVA: 0x00161E0E File Offset: 0x0016000E
	LanguageKey IShopRefresh.DescId
	{
		get
		{
			return LanguageKey.LK_Shop_Refresh_Goods_Desc;
		}
	}

	// Token: 0x170004F3 RID: 1267
	// (get) Token: 0x06002D12 RID: 11538 RVA: 0x00161E15 File Offset: 0x00160015
	LanguageKey IShopRefresh.BreakId
	{
		get
		{
			return LanguageKey.LK_Shop_Refresh_Goods_Desc_AdditionalBreak;
		}
	}

	// Token: 0x170004F4 RID: 1268
	// (get) Token: 0x06002D13 RID: 11539 RVA: 0x00161E1C File Offset: 0x0016001C
	LanguageKey IShopRefresh.NoTimeId
	{
		get
		{
			return LanguageKey.LK_Shop_Refresh_Goods_Desc_NoTime;
		}
	}

	// Token: 0x170004F5 RID: 1269
	// (get) Token: 0x06002D14 RID: 11540 RVA: 0x00161E23 File Offset: 0x00160023
	LanguageKey IShopRefresh.NeedClearId
	{
		get
		{
			return LanguageKey.LK_Shop_Refresh_Goods_Desc_NeedClear;
		}
	}

	// Token: 0x06002D15 RID: 11541 RVA: 0x00161E2C File Offset: 0x0016002C
	void IShopRefresh.InitShopRefresh(Action refreshCount, Action<bool> refreshActiveStates, Action<bool> refreshTips)
	{
		this._protected = false;
		this._refreshTips = refreshTips;
		if (refreshTips != null)
		{
			refreshTips(this._tradeItemsMerchant.Count > 0);
		}
		this._refreshCount = refreshCount;
		if (refreshCount != null)
		{
			refreshCount();
		}
		this._refreshActiveStates = refreshActiveStates;
		if (refreshActiveStates != null)
		{
			refreshActiveStates(((IShopRefresh)this).CanShow);
		}
	}

	// Token: 0x06002D16 RID: 11542 RVA: 0x00161E98 File Offset: 0x00160098
	void IShopRefresh.RefreshCurrentGoods()
	{
		this._protected = true;
		MerchantDomainMethod.AsyncCall.RefreshMerchantGoods(this, this._merchantId, this._isNormalCharacter || this._isCharacterOnSpecificatedMerchant, (sbyte)this.CurShopItemListIndex, this._openShopEventArguments.IsFromBuilding, this._openShopEventArguments.IsHeadBuildingMerchant, this._openShopEventArguments.BuildingMerchantType, delegate(int offset, RawDataPool dataPool)
		{
			bool curr = false;
			Serializer.Deserialize(dataPool, offset, ref curr);
			bool flag = curr;
			if (flag)
			{
				this.GetMerchantData();
				MerchantTypeItem merchantTypeItem = Config.MerchantType.Instance[this._merchantData.MerchantType];
				string desc = (merchantTypeItem != null) ? merchantTypeItem.RefreshDesc : null;
				bool flag2 = desc != null;
				if (flag2)
				{
					this.BubbleStartCoroutine(desc, BubbleLevel.Middle, false);
				}
				base.StartCoroutine(this.RefreshImpl());
			}
			else
			{
				this._protected = false;
				Action<bool> refreshActiveStates = this._refreshActiveStates;
				if (refreshActiveStates != null)
				{
					refreshActiveStates(((IShopRefresh)this).CanShow);
				}
				Debug.LogWarning("Refresh Goods Failed due to lack of action points.");
			}
		});
	}

	// Token: 0x170004F6 RID: 1270
	// (get) Token: 0x06002D17 RID: 11543 RVA: 0x00161EFF File Offset: 0x001600FF
	bool IShopRefresh.CanRefreshCurrentGoods
	{
		get
		{
			return this._tradeItemsMerchant.Count == 0 && SingletonObject.getInstance<TimeManager>().IsActionPointEnough(GlobalConfig.Instance.RefreshItemApCost);
		}
	}

	// Token: 0x170004F7 RID: 1271
	// (get) Token: 0x06002D18 RID: 11544 RVA: 0x00161F25 File Offset: 0x00160125
	bool IShopRefresh.CanShow
	{
		get
		{
			return this.CurShopItemListIndex < 7 && !this._isClickedBuyBack && (this._isNormalCharacter || this._isCharacterOnSpecificatedMerchant || this._openShopEventArguments.IsFromBuilding);
		}
	}

	// Token: 0x170004F8 RID: 1272
	// (get) Token: 0x06002D19 RID: 11545 RVA: 0x00161F59 File Offset: 0x00160159
	bool IShopRefresh.Protected
	{
		get
		{
			return this._protected;
		}
	}

	// Token: 0x06002D1A RID: 11546 RVA: 0x00161F61 File Offset: 0x00160161
	private IEnumerator RefreshImpl()
	{
		yield return new WaitForSeconds(0.5f);
		Action refreshCount = this._refreshCount;
		if (refreshCount != null)
		{
			refreshCount();
		}
		Action<bool> refreshTips = this._refreshTips;
		if (refreshTips != null)
		{
			refreshTips(this._tradeItemsMerchant.Count > 0);
		}
		this._protected = false;
		Action<bool> refreshActiveStates = this._refreshActiveStates;
		if (refreshActiveStates != null)
		{
			refreshActiveStates(((IShopRefresh)this).CanShow);
		}
		yield break;
	}

	// Token: 0x06002D1B RID: 11547 RVA: 0x00161F70 File Offset: 0x00160170
	public void RefreshRefreshButton(bool shouldAlsoRefreshPreview)
	{
		Action<bool> refreshTips = this._refreshTips;
		if (refreshTips != null)
		{
			refreshTips(this._tradeItemsMerchant.Count > 0);
		}
	}

	// Token: 0x170004F9 RID: 1273
	// (get) Token: 0x06002D1C RID: 11548 RVA: 0x00161F93 File Offset: 0x00160193
	public bool IsExistTradeItems
	{
		get
		{
			return this._tradeItemsMerchant.Count > 0 || this._tradeItemsSelf.Count > 0;
		}
	}

	// Token: 0x170004FA RID: 1274
	// (get) Token: 0x06002D1D RID: 11549 RVA: 0x00161FB4 File Offset: 0x001601B4
	private bool CurShopTogIsBugBack
	{
		get
		{
			return this.CurShopItemListIndex == 7;
		}
	}

	// Token: 0x170004FB RID: 1275
	// (get) Token: 0x06002D1E RID: 11550 RVA: 0x00161FC0 File Offset: 0x001601C0
	private int CurShopItemListIndex
	{
		get
		{
			int result;
			if (!this._isClickedBuyBack)
			{
				if (!this._openShopEventArguments.IsSettlementTreasury)
				{
					CToggleObsolete active = this._shopItemTogGroup.GetActive();
					result = ((active != null) ? active.Key : UI_Shop.TradeItemSource.ShopList0.ToInt());
				}
				else
				{
					result = UI_Shop.TradeItemSource.SettlementTreasury.ToInt();
				}
			}
			else
			{
				result = 7;
			}
			return result;
		}
	}

	// Token: 0x170004FC RID: 1276
	// (get) Token: 0x06002D1F RID: 11551 RVA: 0x00162014 File Offset: 0x00160214
	private List<ItemDisplayData> CurShopItemList
	{
		get
		{
			return this._openShopEventArguments.IsSettlementTreasury ? this._settlementTreasuryItems : this._shopItemList[this.CurShopItemListIndex];
		}
	}

	// Token: 0x06002D20 RID: 11552 RVA: 0x00162038 File Offset: 0x00160238
	private bool CheckIsExtra(int id, out MerchantExtraGoodsData.ExtraGoodsType extraGoodsType)
	{
		extraGoodsType = MerchantExtraGoodsData.ExtraGoodsType.None;
		bool flag = this._openShopEventArguments.IsSettlementTreasury || this.CurShopTogIsBugBack;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			MerchantExtraGoodsData extraGoods = this._extraGoods;
			bool check = extraGoods != null && extraGoods.Check(id, out extraGoodsType);
			result = check;
		}
		return result;
	}

	// Token: 0x170004FD RID: 1277
	// (get) Token: 0x06002D21 RID: 11553 RVA: 0x00162081 File Offset: 0x00160281
	private int CurSelfItemListIndex
	{
		get
		{
			CToggleObsolete active = this._selfItemTogGroup.GetActive();
			return (active != null) ? active.Key : UI_Shop.TogType.Inventory.ToInt();
		}
	}

	// Token: 0x170004FE RID: 1278
	// (get) Token: 0x06002D22 RID: 11554 RVA: 0x001620A4 File Offset: 0x001602A4
	private UI_Shop.TradeItemSource CurSelfTradeItemSourceType
	{
		get
		{
			UI_Shop.TradeItemSource result;
			switch (this.CurSelfItemListIndex)
			{
			case 0:
				result = UI_Shop.TradeItemSource.Inventory;
				break;
			case 1:
				result = UI_Shop.TradeItemSource.Warehouse;
				break;
			case 2:
				result = UI_Shop.TradeItemSource.Treasury;
				break;
			default:
				result = UI_Shop.TradeItemSource.Inventory;
				break;
			}
			return result;
		}
	}

	// Token: 0x170004FF RID: 1279
	// (get) Token: 0x06002D23 RID: 11555 RVA: 0x001620E4 File Offset: 0x001602E4
	private List<ItemDisplayData> CurSelfItemList
	{
		get
		{
			List<ItemDisplayData> result;
			switch (this.CurSelfItemListIndex)
			{
			case 0:
				result = this._inventoryItems;
				break;
			case 1:
				result = this._warehouseItems;
				break;
			case 2:
				result = this._treasuryItems;
				break;
			default:
				result = null;
				break;
			}
			return result;
		}
	}

	// Token: 0x17000500 RID: 1280
	// (get) Token: 0x06002D24 RID: 11556 RVA: 0x00162131 File Offset: 0x00160331
	public int SettlementTreasuryLayerIndex
	{
		get
		{
			CToggleObsolete active = this._settlementTreasuryTogGroup.GetActive();
			return (active != null) ? active.Key : SettlementTreasuryLayers.Shallow.ToInt();
		}
	}

	// Token: 0x06002D25 RID: 11557 RVA: 0x00162154 File Offset: 0x00160354
	public override void OnInit(ArgumentBox argsBox)
	{
		this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		this._inventoryItems.Clear();
		this._warehouseItems.Clear();
		this._treasuryItems.Clear();
		this._settlementTreasuryItems.Clear();
		this._tradeItemsMerchant.Clear();
		this._tradeItemsSelf.Clear();
		this._tradeItemSourceDict.Clear();
		this._tradeMoneySources.Clear();
		this._buyMoney = 0;
		this._confirmedTreasuryOperation = false;
		this._extraGoods = null;
		this._merchantData = null;
		this._merchantBuyBackData = null;
		this._merchantCharData = null;
		this._merchantTypeConfig = null;
		this._originMerchantOverFavorData = null;
		this._tempMerchantOverFavorData = null;
		this._isGetLastSettlementTreasuryOperationData = true;
		if (argsBox != null)
		{
			argsBox.Get<OpenShopEventArguments>("OpenShopEventArguments", out this._openShopEventArguments);
		}
		this._isCharacterOnSpecificatedMerchant = (this._openShopEventArguments.MerchantSourceTypeEnum == OpenShopEventArguments.EMerchantSourceType.SpecifiedOnBuildingMerchantType);
		this._sellingDisabled = this._isCharacterOnSpecificatedMerchant;
		this._merchantId = this._openShopEventArguments.Id;
		this._isNormalCharacter = (this._openShopEventArguments.MerchantSourceTypeEnum == OpenShopEventArguments.EMerchantSourceType.NormalCharacter);
		string audioIn = this._openShopEventArguments.IsSettlementTreasury ? "ui_building_treasury" : "ui_caravan_open";
		AudioManager.Instance.PlaySound(audioIn, false, false);
		this._clerkAvatar = base.CGet<Game.Components.Avatar.Avatar>("ClerkAvatar");
		this._clerkAvatar.ResetToBlank(false);
		bool hasMerchantInfo = !this._isCharacterOnSpecificatedMerchant && !this._openShopEventArguments.IsSettlementTreasury;
		base.CGet<Refers>("ShopLevel").gameObject.SetActive(hasMerchantInfo);
		base.CGet<CButtonObsolete>("ButtonInfo").gameObject.SetActive(hasMerchantInfo);
		Transform frontTrans = base.CGet<GameObject>("Front").transform;
		for (int i = 0; i < frontTrans.childCount; i++)
		{
			frontTrans.GetChild(i).gameObject.SetActive(false);
		}
		Transform merchantNPCTrans = base.CGet<GameObject>("MerchantNPC").transform;
		for (int j = 0; j < merchantNPCTrans.childCount; j++)
		{
			merchantNPCTrans.GetChild(j).gameObject.SetActive(false);
		}
		for (int k = 0; k < this._shopItemList.Length; k++)
		{
			bool flag = this._shopItemList[k] == null;
			if (flag)
			{
				this._shopItemList[k] = new List<ItemDisplayData>();
			}
			else
			{
				this._shopItemList[k].Clear();
			}
		}
		this._shopItemScroll = base.CGet<ItemScrollViewForCommonTableRow>("ShopItemScroll");
		this._shopItemScroll.Init(ESortAndFilterControllerType.Shop, null);
		List<ItemDisplayData> shopItemList = this._openShopEventArguments.IsSettlementTreasury ? this._settlementTreasuryItems : this._shopItemList[0];
		this._shopItemScroll.SetItemList(ref shopItemList, true, "shop_merchant", new Action<ItemDisplayData, CommonTableRowForItem>(this.OnRenderShopItem), null, true);
		this._selfItemScroll = base.CGet<ItemScrollViewForCommonTableRow>("SelfItemScroll");
		this._selfItemScroll.Init(ESortAndFilterControllerType.Shop, null);
		this._selfItemScroll.SetItemList(ref this._inventoryItems, true, "shop_self", new Action<ItemDisplayData, CommonTableRowForItem>(this.OnRenderSelfItem), null, true);
		this._tradeMerchantScrollView = base.CGet<ItemScrollViewForCommonTableRow>("TradeMerchantScrollView");
		this._tradeMerchantScrollView.Init(ESortAndFilterControllerType.Item, null);
		this._tradeMerchantScrollView.SetItemList(ref this._tradeItemsMerchant, true, "shop_trade_merchant", this.OnRenderTradeItemGenerator(this._tradeItemSourceDict), null, true);
		this._tradeSelfScrollView = base.CGet<ItemScrollViewForCommonTableRow>("TradeSelfScrollView");
		this._tradeSelfScrollView.Init(ESortAndFilterControllerType.Item, null);
		this._tradeSelfScrollView.SetItemList(ref this._tradeItemsSelf, true, "shop_trade_self", this.OnRenderTradeItemGenerator(this._tradeItemSourceDict), null, true);
		this._shopItemTogGroup = base.CGet<CToggleGroupObsolete>("GoodsListTogGroup");
		this._shopItemTogGroup.InitPreOnToggle(-1);
		this._shopItemTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnShopItemTogChange);
		this._selfItemTogGroup = base.CGet<CToggleGroupObsolete>("SelfItemTogGroup");
		this._selfItemTogGroup.InitPreOnToggle(-1);
		this._selfItemTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnSelfItemTogChange);
		this._settlementTreasuryTogGroup = base.CGet<Refers>("TreasuryInfoRoot").CGet<CToggleGroupObsolete>("SelfItemTogGroup");
		this._settlementTreasuryTogGroup.InitPreOnToggle(-1);
		this._settlementTreasuryTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnsettlementTreasuryTogChange);
		base.CGet<CButtonObsolete>("ButtonReplenish").gameObject.SetActive(this._openShopEventArguments.IsSettlementTreasury);
		base.CGet<CButtonObsolete>("ButtonBuyAll").gameObject.SetActive(!this._openShopEventArguments.IsSettlementTreasury);
		base.CGet<CButtonObsolete>("ButtonSellAll").gameObject.SetActive(!this._openShopEventArguments.IsSettlementTreasury);
		bool isSettlementTreasury = this._openShopEventArguments.IsSettlementTreasury;
		if (isSettlementTreasury)
		{
			this._settlementTreasuryTogGroup.SetWithoutNotify((int)this._openShopEventArguments.CurrPage, true);
		}
		this._shopItemMask = base.CGet<GameObject>("ShopMask");
		this._shopItemMask.SetActive(false);
		base.CGet<GameObject>("CharacterInfoRoot").gameObject.SetActive(true);
		base.CGet<ShopProgressBar>("ProgressBar").gameObject.SetActive(!this._openShopEventArguments.IsSettlementTreasury);
		this._shopItemTogGroup.gameObject.SetActive(!this._openShopEventArguments.IsSettlementTreasury);
		this._selfItemTogGroup.Set(0, true, false);
		CButtonObsolete confirm = base.CGet<CButtonObsolete>("ButtonConfirm");
		confirm.transform.Find("Shop").gameObject.SetActive(!this._openShopEventArguments.IsSettlementTreasury);
		confirm.transform.Find("Treasury").gameObject.SetActive(this._openShopEventArguments.IsSettlementTreasury);
		this.SetConfirmButton(false, "");
		base.CGet<CButtonObsolete>("ButtonReset").interactable = false;
		base.CGet<GameObject>("ShopInfoRoot").SetActive(!this._openShopEventArguments.IsSettlementTreasury);
		base.CGet<Refers>("TreasuryInfoRoot").gameObject.SetActive(this._openShopEventArguments.IsSettlementTreasury);
		base.CGet<GameObject>("ShopTradeRoot").SetActive(!this._openShopEventArguments.IsSettlementTreasury);
		base.CGet<GameObject>("TreasuryTradeRoot").SetActive(this._openShopEventArguments.IsSettlementTreasury);
		Refers loadRectTrans = base.CGet<Refers>("Load");
		GameObject loadParent = base.CGet<GameObject>(this._openShopEventArguments.IsSettlementTreasury ? "TreasurySelfLayout" : "SelfMoneyLayout");
		loadRectTrans.transform.SetParent(loadParent.transform);
		base.CGet<GameObject>("StealTip").SetActive(false);
		base.CGet<GameObject>("StoreTip").SetActive(false);
		base.CGet<GameObject>("ExchangeAndStealTip").gameObject.SetActive(false);
		base.CGet<Refers>("ExchangeTip").gameObject.SetActive(false);
		this._giveToTreasuryScrollView = base.CGet<ItemScrollView>("GiveToTreasuryScrollView");
		this._giveToTreasuryScrollView.Init();
		this._giveToTreasuryScrollView.SortAndFilter.SortEnabled = false;
		this._giveToTreasuryScrollView.SetItemList(ref this._tradeItemsSelf, true, "shop_trade", false, this.OnRenderTradeItemGeneratorForSettlement(this._tradeItemSourceDict));
		this._giveToTreasuryScrollView.InfinityScroll.Scroll.SetScrollEnable(false);
		this._getFromTreasuryScrollView = base.CGet<ItemScrollView>("GetFromTreasuryScrollView");
		this._getFromTreasuryScrollView.Init();
		this._getFromTreasuryScrollView.SortAndFilter.SortEnabled = false;
		this._getFromTreasuryScrollView.SetItemList(ref this._tradeItemsMerchant, true, "shop_trade", false, this.OnRenderTradeItemGeneratorForSettlement(this._tradeItemSourceDict));
		this._getFromTreasuryScrollView.InfinityScroll.Scroll.SetScrollEnable(false);
		this._isClickedBuyBack = false;
		this.ClearSlotItemChange();
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
		UIElement element2 = this.Element;
		element2.OnShowed = (Action)Delegate.Combine(element2.OnShowed, new Action(delegate()
		{
			bool flag2 = !this._openShopEventArguments.IsSettlementTreasury;
			if (flag2)
			{
				this.InitGoodsTog(-1);
			}
		}));
		UIElement element3 = this.Element;
		element3.OnHide = (Action)Delegate.Combine(element3.OnHide, new Action(delegate()
		{
			TaiwuEventDomainMethod.Call.TriggerListener("MerchantShopClose", true);
		}));
		this._leftCloudBubble = base.CGet<Bubble>("LeftCloudBubble");
		this._leftCloudBubble.Hide();
		this.ResetDialog();
	}

	// Token: 0x06002D26 RID: 11558 RVA: 0x001629E0 File Offset: 0x00160BE0
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)this._taiwuCharId, new uint[]
		{
			34U,
			104U,
			103U,
			59U,
			76U
		}));
		bool isNormalCharacter = this._isNormalCharacter;
		if (isNormalCharacter)
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)this._merchantId, new uint[]
			{
				35U,
				36U
			}));
		}
		else
		{
			this._lovingItemSubType = -1;
			this._hatingItemSubType = -1;
		}
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 8, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 7, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(1, 17, ulong.MaxValue, null));
	}

	// Token: 0x06002D27 RID: 11559 RVA: 0x00162AA4 File Offset: 0x00160CA4
	private void OnEnable()
	{
		GEvent.Add(EEvents.OnAreaSpiritualDebtChange, new GEvent.Callback(this.RefreshMoneyPanel));
	}

	// Token: 0x06002D28 RID: 11560 RVA: 0x00162AC0 File Offset: 0x00160CC0
	private void OnDisable()
	{
		GEvent.Remove(EEvents.OnAreaSpiritualDebtChange, new GEvent.Callback(this.RefreshMoneyPanel));
		bool flag = this._favorabilityHandler != null;
		if (flag)
		{
			BasicInfoMonitor monitor = this._favorabilityHandler.GetMonitor<BasicInfoMonitor>();
			monitor.RemoveFavorabilityListener(new Action(this.RefreshMerchantCharFavorProgress));
			this._favorabilityHandler.CharacterId = -1;
			this._favorabilityHandler = null;
		}
		this._shopItemScroll.SaveSortFilterSetting(false);
		this._selfItemScroll.SaveSortFilterSetting(true);
		MerchantDomainMethod.Call.PullTradeCaravanLocations();
	}

	// Token: 0x06002D29 RID: 11561 RVA: 0x00162B4C File Offset: 0x00160D4C
	private void GetMerchantData()
	{
		bool isFromBuilding = this._openShopEventArguments.IsFromBuilding;
		if (isFromBuilding)
		{
			MerchantDomainMethod.Call.GetBuildingMerchantData(this.Element.GameDataListenerId, this._openShopEventArguments.BuildingMerchantType, this._openShopEventArguments.IsHeadBuildingMerchant);
			int merchantId = GameData.Domains.Merchant.SharedMethods.GetBuildingMerchantCaravanId(this._openShopEventArguments.BuildingMerchantType, this._openShopEventArguments.IsHeadBuildingMerchant);
			ExtraDomainMethod.Call.GetMerchantExtraGoods(this.Element.GameDataListenerId, merchantId);
		}
		else
		{
			bool flag = this._openShopEventArguments.MerchantSourceTypeEnum == OpenShopEventArguments.EMerchantSourceType.SpecialBuilding;
			if (flag)
			{
				MerchantDomainMethod.Call.GetSectStorySpecialMerchantData(this.Element.GameDataListenerId);
			}
			else
			{
				bool isCharacterOnSpecificatedMerchant = this._isCharacterOnSpecificatedMerchant;
				if (isCharacterOnSpecificatedMerchant)
				{
					MerchantDomainMethod.Call.GetMerchantData(this.Element.GameDataListenerId, this._openShopEventArguments.Id);
				}
				else
				{
					bool flag2 = !this._isNormalCharacter;
					if (flag2)
					{
						MerchantDomainMethod.Call.GetCaravanMerchantData(this.Element.GameDataListenerId, this._merchantId);
					}
					else
					{
						MerchantDomainMethod.Call.GetMerchantData(this.Element.GameDataListenerId, this._merchantId);
					}
				}
				ExtraDomainMethod.Call.GetMerchantExtraGoods(this.Element.GameDataListenerId, this._merchantId);
			}
		}
		MerchantDomainMethod.Call.GetMerchantBuyBackData(this.Element.GameDataListenerId, this._openShopEventArguments);
	}

	// Token: 0x06002D2A RID: 11562 RVA: 0x00162C88 File Offset: 0x00160E88
	private void OnListenerIdReady()
	{
		bool isSettlementTreasury = this._openShopEventArguments.IsSettlementTreasury;
		if (isSettlementTreasury)
		{
			CharacterDomainMethod.Call.GetCharacterDisplayData(this.Element.GameDataListenerId, this._taiwuCharId);
			OrganizationDomainMethod.Call.GetSettlementTreasuryDisplayData(this.Element.GameDataListenerId, this._openShopEventArguments.SettlementId, (sbyte)this.SettlementTreasuryLayerIndex);
			OrganizationDomainMethod.Call.CheckSettlementGuardFavorabilityType(this.Element.GameDataListenerId, this._openShopEventArguments.SettlementId);
			Array.Clear(this._settlementTreasuryAccessable, 0, this._settlementTreasuryAccessable.Length);
		}
		else
		{
			this.GetMerchantData();
			bool isNormalCharacter = this._isNormalCharacter;
			if (isNormalCharacter)
			{
				CharacterDomainMethod.Call.GetCharacterDisplayData(this.Element.GameDataListenerId, this._merchantId);
			}
			else
			{
				bool isCharacterOnSpecificatedMerchant = this._isCharacterOnSpecificatedMerchant;
				if (isCharacterOnSpecificatedMerchant)
				{
					CharacterDomainMethod.Call.GetCharacterDisplayData(this.Element.GameDataListenerId, this._openShopEventArguments.Id);
				}
				else
				{
					this._merchantCharData = null;
				}
			}
		}
		this.SendRefreshSelfItem();
	}

	// Token: 0x06002D2B RID: 11563 RVA: 0x00162D78 File Offset: 0x00160F78
	private void SendRefreshSelfItem()
	{
		CharacterDomainMethod.Call.GetAllInventoryItems(this.Element.GameDataListenerId, this._taiwuCharId);
		CharacterDomainMethod.Call.GetAllEquipmentItems(this.Element.GameDataListenerId, this._taiwuCharId);
		TaiwuDomainMethod.Call.CanTransferItemToWarehouse(this.Element.GameDataListenerId);
	}

	// Token: 0x17000501 RID: 1281
	// (get) Token: 0x06002D2C RID: 11564 RVA: 0x00162DC5 File Offset: 0x00160FC5
	public bool IsBreaking
	{
		get
		{
			return !this._settlementTreasuryTogGroup.Get(0).interactable;
		}
	}

	// Token: 0x06002D2D RID: 11565 RVA: 0x00162DDC File Offset: 0x00160FDC
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b != 0)
			{
				if (b == 1)
				{
					bool flag = notification.DomainId == 14;
					if (flag)
					{
						bool flag2 = notification.MethodId == 0 || notification.MethodId == 4 || notification.MethodId == 15;
						if (flag2)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._merchantData);
							this._merchantTypeConfig = Config.MerchantType.Instance[this._merchantData.MerchantType];
							MerchantDomainMethod.Call.GetMerchantOverFavorData(this.Element.GameDataListenerId, this._merchantData.MerchantType);
							MerchantDomainMethod.Call.GetCurFavorability(this.Element.GameDataListenerId, this._merchantData.MerchantType);
						}
						else
						{
							bool flag3 = notification.MethodId == 27;
							if (flag3)
							{
								SectStorySpecialMerchant sectStorySpecialMerchant = null;
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref sectStorySpecialMerchant);
								this._merchantData = sectStorySpecialMerchant.MerchantData;
								this._extraGoods = sectStorySpecialMerchant.MerchantExtraGoodsData;
								this._merchantTypeConfig = Config.MerchantType.Instance[this._merchantData.MerchantType];
								MerchantDomainMethod.Call.GetMerchantOverFavorData(this.Element.GameDataListenerId, this._merchantData.MerchantType);
								MerchantDomainMethod.Call.GetCurFavorability(this.Element.GameDataListenerId, this._merchantData.MerchantType);
							}
							else
							{
								bool flag4 = notification.MethodId == 14;
								if (flag4)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._originMerchantOverFavorData);
								}
								else
								{
									bool flag5 = notification.MethodId == 8;
									if (flag5)
									{
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref UI_Shop._merchantFavorability);
										this.RefreshMerchantData();
										base.CGet<ShopProgressBar>("ProgressBar").InitWithIdAndFavor((int)this._merchantData.MerchantTemplateId, UI_Shop._merchantFavorability);
										this.RefreshTreasuryBalanceButton();
										this.Element.ShowAfterRefresh();
									}
									else
									{
										bool flag6 = notification.MethodId == 31;
										if (flag6)
										{
											int favor = 0;
											Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref favor);
											base.CGet<ShopProgressBar>("ProgressBar").PreviewValue = favor;
										}
										else
										{
											bool flag7 = notification.MethodId == 29;
											if (flag7)
											{
												Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._merchantBuyBackData);
												if (this._merchantBuyBackData == null)
												{
													this._merchantBuyBackData = new MerchantBuyBackData();
												}
											}
										}
									}
								}
							}
						}
					}
					else
					{
						bool flag8 = notification.DomainId == 4;
						if (flag8)
						{
							bool flag9 = notification.MethodId == 131;
							if (flag9)
							{
								CharacterDisplayData characterDisplayData = null;
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref characterDisplayData);
								bool flag10 = characterDisplayData.CharacterId == this._taiwuCharId;
								if (flag10)
								{
									this._taiwuCharacterDisplayData = characterDisplayData;
								}
								else
								{
									this._merchantCharData = characterDisplayData;
									this.RefreshMerchantCharData();
								}
							}
							else
							{
								bool flag11 = notification.MethodId == 27;
								if (flag11)
								{
									this._inventoryItems.Clear();
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._inventoryItems);
								}
								else
								{
									bool flag12 = notification.MethodId == 29;
									if (flag12)
									{
										List<ItemDisplayData> equipItems = new List<ItemDisplayData>();
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref equipItems);
										sbyte slotIndex = 0;
										while ((int)slotIndex < equipItems.Count)
										{
											ItemDisplayData itemData = equipItems[(int)slotIndex];
											bool flag13 = itemData.Key.IsValid();
											if (flag13)
											{
												this._inventoryItems.Add(itemData);
											}
											slotIndex += 1;
										}
										foreach (ItemDisplayData data in this._inventoryItems)
										{
											int basePrice = this.GetItemBasePrice(data, false);
											int priceChangePercent = this.GetPriceChangePercentValue(data, false);
											int priceChange = basePrice * priceChangePercent;
											data.ItemPriceState = ((priceChange == 0) ? EItemPriceState.None : ((priceChange > 0) ? EItemPriceState.Up : EItemPriceState.Down));
										}
										this.RefreshSelfItems();
									}
								}
							}
						}
						else
						{
							bool flag14 = notification.DomainId == 5;
							if (flag14)
							{
								bool flag15 = notification.MethodId == 42;
								if (flag15)
								{
									bool canOpenWarehouse = false;
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref canOpenWarehouse);
									CToggleObsolete warehouseTog = this._selfItemTogGroup.Get(UI_Shop.TogType.Warehouse.ToInt());
									warehouseTog.interactable = canOpenWarehouse;
									warehouseTog.GetComponent<TooltipInvoker>().enabled = !warehouseTog.interactable;
									CToggleObsolete treasuryTog = this._selfItemTogGroup.Get(UI_Shop.TogType.Treasury.ToInt());
									treasuryTog.interactable = canOpenWarehouse;
									treasuryTog.GetComponent<TooltipInvoker>().enabled = !treasuryTog.interactable;
									bool flag16 = canOpenWarehouse;
									if (flag16)
									{
										TaiwuDomainMethod.Call.GetAllWarehouseItems(this.Element.GameDataListenerId);
										TaiwuDomainMethod.Call.GetTreasuryNeededItemList(this.Element.GameDataListenerId);
										TaiwuDomainMethod.Call.GetAllResources(this.Element.GameDataListenerId, ItemSourceType.Treasury);
										TaiwuDomainMethod.Call.GetAllTreasuryItems(this.Element.GameDataListenerId);
									}
									else
									{
										this.Element.ShowAfterRefresh();
										this.RefreshTreasuryBalanceButton();
										this.RefreshSelfItems();
									}
								}
								else
								{
									bool flag17 = notification.MethodId == 15;
									if (flag17)
									{
										this._warehouseItems.Clear();
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._warehouseItems);
									}
									else
									{
										bool flag18 = notification.MethodId == 139;
										if (flag18)
										{
											Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._villagerNeededItemSet);
										}
										else
										{
											bool flag19 = notification.MethodId == 126;
											if (flag19)
											{
												ValueTuple<ItemSourceType, ResourceInts> tuple = default(ValueTuple<ItemSourceType, ResourceInts>);
												Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref tuple);
												bool flag20 = tuple.Item1 == ItemSourceType.Treasury;
												if (flag20)
												{
													this._treasuryResources = tuple.Item2;
												}
											}
											else
											{
												bool flag21 = notification.MethodId == 64;
												if (flag21)
												{
													this._treasuryItems.Clear();
													Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._treasuryItems);
													bool isSettlementTreasury = this._openShopEventArguments.IsSettlementTreasury;
													if (isSettlementTreasury)
													{
														this.AddTempResourcesItem(this._treasuryResources, this._treasuryItems);
													}
													this.RefreshSelfItems();
												}
											}
										}
									}
								}
							}
							else
							{
								bool flag22 = notification.DomainId == 3;
								if (flag22)
								{
									bool flag23 = notification.MethodId == 11;
									if (flag23)
									{
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._settlementTreasuryDisplayData);
										this._settlementTreasury = this._settlementTreasuryDisplayData.SettlementTreasury;
									}
									else
									{
										bool flag24 = notification.MethodId == 30;
										if (flag24)
										{
											bool[] settlementTreasuryGuaerdFavorStatus = new bool[Enum.GetValues(typeof(SettlementTreasuryLayers)).Length];
											Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref settlementTreasuryGuaerdFavorStatus);
											for (int i = 0; i < settlementTreasuryGuaerdFavorStatus.Length; i++)
											{
												settlementTreasuryGuaerdFavorStatus[i] = true;
											}
											OrganizationItem orgConfig = Organization.Instance[this._settlementTreasuryDisplayData.OrgTemplateId];
											bool isSect = orgConfig.IsSect;
											sbyte templateId = orgConfig.TemplateId;
											bool isSect2 = isSect;
											if (isSect2)
											{
												OrganizationDomainMethod.AsyncCall.GetOrganizationCombatSkillsDisplayData(this, templateId, delegate(int offset, RawDataPool dataPool)
												{
													OrganizationCombatSkillsDisplayData data2 = new OrganizationCombatSkillsDisplayData();
													Serializer.Deserialize(dataPool, offset, ref data2);
													int value2 = (int)Math.Round((double)((float)data2.ApprovingRate / 10f), 1);
													this.<OnNotifyGameData>g__SetMouseTipSettlementTreasuryOrPrisonLayer|139_0(isSect, value2, settlementTreasuryGuaerdFavorStatus);
													this.RefreshSettlementTreasury();
												});
											}
											else
											{
												int value = SingletonObject.getInstance<WorldMapModel>().GetAreaSpiritualDebt(SingletonObject.getInstance<WorldMapModel>().CurrentAreaId);
												this.<OnNotifyGameData>g__SetMouseTipSettlementTreasuryOrPrisonLayer|139_0(isSect, value, settlementTreasuryGuaerdFavorStatus);
												this.RefreshSettlementTreasury();
											}
											this.ClearSlotItemChange();
										}
									}
								}
								else
								{
									bool flag25 = notification.DomainId == 19;
									if (flag25)
									{
										bool flag26 = notification.MethodId == 149;
										if (flag26)
										{
											Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._extraGoods);
										}
									}
								}
							}
						}
					}
				}
			}
			else
			{
				DataUid uid = notification.Uid;
				bool flag27 = uid.DomainId == 4;
				if (flag27)
				{
					bool flag28 = uid.SubId1 == 34U;
					if (flag28)
					{
						this._selfResources.Initialize();
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._selfResources);
						this._selfMoney = this._selfResources.Get(6);
						this.RefreshMoneyPanel();
						bool isSettlementTreasury2 = this._openShopEventArguments.IsSettlementTreasury;
						if (isSettlementTreasury2)
						{
							this.AddTempResourcesItem(this._selfResources, this._inventoryItems);
						}
						this.RefreshSelfItems();
					}
					else
					{
						bool flag29 = uid.SubId1 == 104U;
						if (flag29)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._curInventoryLoad);
							this.RefreshLoad();
						}
						else
						{
							bool flag30 = uid.SubId1 == 103U;
							if (flag30)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._maxInventoryLoad);
								this.RefreshLoad();
							}
							else
							{
								bool flag31 = uid.SubId1 == 59U;
								if (flag31)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._learnedSkillList);
								}
								else
								{
									bool flag32 = uid.SubId1 == 35U;
									if (flag32)
									{
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._lovingItemSubType);
									}
									else
									{
										bool flag33 = uid.SubId1 == 36U;
										if (flag33)
										{
											Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._hatingItemSubType);
										}
									}
								}
							}
						}
					}
				}
				else
				{
					bool flag34 = uid.DomainId == 5;
					if (flag34)
					{
						bool flag35 = uid.DataId == 8;
						if (flag35)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._warehouseCurLoad);
							this.RefreshLoad();
						}
						else
						{
							bool flag36 = uid.DataId == 7;
							if (flag36)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._warehouseMaxLoad);
								this.RefreshLoad();
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06002D2E RID: 11566 RVA: 0x00163874 File Offset: 0x00161A74
	private void RefreshMerchantData()
	{
		this.RefreshSpeakOnInit();
		MerchantItem merchantConfig = Merchant.Instance[this._merchantData.MerchantTemplateId];
		base.CGet<TextMeshProUGUI>("ShopTitle").text = merchantConfig.UiName;
		base.CGet<Refers>("ShopLevel").CGet<TextMeshProUGUI>("Value").text = merchantConfig.UiName.SetColor("caravan");
		this.RefreshMerchantCharData();
		Transform frontTrans = base.CGet<GameObject>("Front").transform;
		Transform merchantNPCTrans = base.CGet<GameObject>("MerchantNPC").transform;
		for (int i = 0; i < frontTrans.childCount; i++)
		{
			frontTrans.GetChild(i).gameObject.SetActive((int)this._merchantData.MerchantType == i);
		}
		int targetIndex = (int)(this._openShopEventArguments.IsHeadBuildingMerchant ? this._merchantData.MerchantType : (this._merchantData.MerchantType + 7));
		for (int j = 0; j < merchantNPCTrans.childCount; j++)
		{
			merchantNPCTrans.GetChild(j).gameObject.SetActive(!this._isNormalCharacter && !this._isCharacterOnSpecificatedMerchant && targetIndex == j);
		}
		for (int k = 0; k <= 6; k++)
		{
			this.InitGoodsItemList(this._merchantData.GetGoodsList(k), k);
		}
		this.InitGoodsItemList(this._merchantBuyBackData.BuyInGoodsList, 7);
		this.UpdateTradeAreaDisplay();
	}

	// Token: 0x06002D2F RID: 11567 RVA: 0x00163A08 File Offset: 0x00161C08
	private void RefreshMerchantCharData()
	{
		CommonCharacterNameFrame nameFrame = base.CGet<CommonCharacterNameFrame>("MerchantName");
		bool hasCharData = this._merchantCharData != null;
		bool flag = hasCharData;
		if (flag)
		{
			this._clerkAvatar.Refresh(this._merchantCharData, true);
			string charName = NameCenter.GetMonasticTitleOrDisplayName(this._merchantCharData, false);
			nameFrame.SetName(charName);
		}
		nameFrame.gameObject.SetActive(hasCharData);
		Refers favorInfo = base.CGet<Refers>("MerchantFavor");
		CharacterDisplayData merchantCharData = this._merchantCharData;
		bool showFavor = merchantCharData != null && merchantCharData.CreatingType == 1;
		favorInfo.gameObject.SetActive(showFavor);
		base.CGet<CommonProgressFavaor>("CommonProgressFavaor").gameObject.SetActive(showFavor);
		bool flag2 = !showFavor;
		if (!flag2)
		{
			TextMeshProUGUI favorText = favorInfo.CGet<TextMeshProUGUI>("Value");
			if (this._favorabilityHandler == null)
			{
				this._favorabilityHandler = new CharacterFavorability(null, favorText, null, null, null);
			}
			this._favorabilityHandler.CharacterId = (this._isCharacterOnSpecificatedMerchant ? this._openShopEventArguments.Id : this._merchantCharData.CharacterId);
			BasicInfoMonitor monitor = this._favorabilityHandler.GetMonitor<BasicInfoMonitor>();
			monitor.RemoveFavorabilityListener(new Action(this.RefreshMerchantCharFavorProgress));
			monitor.AddFavorabilityListener(new Action(this.RefreshMerchantCharFavorProgress));
			bool init = monitor.Init;
			if (init)
			{
				monitor.Refresh();
			}
		}
	}

	// Token: 0x06002D30 RID: 11568 RVA: 0x00163B64 File Offset: 0x00161D64
	private void RefreshMerchantCharFavorProgress()
	{
		BasicInfoMonitor monitor = this._favorabilityHandler.GetMonitor<BasicInfoMonitor>();
		short favor = (monitor != null) ? monitor.FavorabilityToTaiwu : 0;
		base.CGet<CommonProgressFavaor>("CommonProgressFavaor").Setup(favor);
		base.CGet<CommonProgressFavaor>("CommonProgressFavaor").SetProgress(favor);
	}

	// Token: 0x06002D31 RID: 11569 RVA: 0x00163BB0 File Offset: 0x00161DB0
	private void InitGoodsItemList(Inventory goodList, int goodsIndex)
	{
		bool flag = goodList == null;
		if (!flag)
		{
			Inventory tempInventory = new Inventory();
			bool flag2 = !SingletonObject.getInstance<ProfessionModel>().IsProfessionalSkillUnlockedAndEquipped(63) && this._extraGoods != null;
			if (flag2)
			{
				foreach (KeyValuePair<ItemKey, int> keyValuePair in goodList.Items)
				{
					ItemKey itemKey3;
					int num;
					keyValuePair.Deconstruct(out itemKey3, out num);
					ItemKey itemKey = itemKey3;
					int amount = num;
					MerchantExtraGoodsData.ExtraGoodsType extraGoodsType;
					bool isProfession = this._extraGoods.Check(itemKey.Id, out extraGoodsType) && extraGoodsType == MerchantExtraGoodsData.ExtraGoodsType.Capitalist;
					bool flag3 = !isProfession;
					if (flag3)
					{
						tempInventory.OfflineAdd(itemKey, amount);
					}
				}
			}
			else
			{
				foreach (KeyValuePair<ItemKey, int> keyValuePair in goodList.Items)
				{
					ItemKey itemKey3;
					int num;
					keyValuePair.Deconstruct(out itemKey3, out num);
					ItemKey itemKey2 = itemKey3;
					int amount2 = num;
					tempInventory.OfflineAdd(itemKey2, amount2);
				}
			}
			ItemDomainMethod.AsyncCall.GetItemDisplayDataListOptionalFromInventory(this, tempInventory, this._taiwuCharId, -1, true, delegate(int offset, RawDataPool dataPool)
			{
				List<ItemDisplayData> itemList = this._shopItemList[goodsIndex];
				Serializer.Deserialize(dataPool, offset, ref itemList);
				foreach (ItemDisplayData data in itemList)
				{
					bool flag4 = goodsIndex == 7;
					if (flag4)
					{
						data.ItemPriceState = EItemPriceState.None;
					}
					else
					{
						int basePrice = this.GetItemBasePrice(data, true);
						int priceChangePercent = this.GetPriceChangePercentValue(data, true);
						int priceChange = basePrice * priceChangePercent;
						data.ItemPriceState = ((priceChange == 0) ? EItemPriceState.None : ((priceChange > 0) ? EItemPriceState.Up : EItemPriceState.Down));
					}
				}
				bool flag5 = goodsIndex == 6;
				if (flag5)
				{
					CToggleObsolete active = this._shopItemTogGroup.GetActive();
					int targetTogKey = (active != null) ? active.Key : -1;
					this.InitGoodsTog(targetTogKey);
				}
				else
				{
					bool flag6 = goodsIndex == 7;
					if (flag6)
					{
						this.InitBuyBack();
					}
				}
			});
		}
	}

	// Token: 0x06002D32 RID: 11570 RVA: 0x00163D1C File Offset: 0x00161F1C
	private void InitGoodsTog(int targetTogIndex = -1)
	{
		int worldProgressLimitedLevel;
		int worldProgressLimitedFavor;
		UI_Shop.IsReachProgressLimit(UI_Shop._merchantFavorability, out worldProgressLimitedLevel, out worldProgressLimitedFavor);
		int merchantFavorability = Mathf.Min(worldProgressLimitedFavor, UI_Shop._merchantFavorability);
		int autoCheckKey = 0;
		int length = this._shopItemTogGroup.Count();
		for (int i = 0; i < length; i++)
		{
			bool isPageDisabled = this.IsPageDisabled(i);
			bool isOverProgress = i > GameData.Domains.Merchant.SharedMethods.GetFavorLevel(merchantFavorability);
			CToggleObsolete tog = this._shopItemTogGroup.Get(i);
			Refers refers = tog.GetComponent<Refers>();
			TooltipInvoker tip = refers.CGet<TooltipInvoker>("Tip");
			tip.Type = TipType.SingleDesc;
			tip.enabled = isOverProgress;
			bool flag = this.IsPageShow(i);
			if (flag)
			{
				tog.gameObject.SetActive(true);
				tog.interactable = true;
				bool flag2 = i > autoCheckKey && !isOverProgress;
				if (flag2)
				{
					autoCheckKey = i;
				}
				StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
				bool flag3 = isOverProgress;
				if (flag3)
				{
					stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Shop_Tog_Tip));
					bool flag4 = isPageDisabled;
					if (flag4)
					{
						stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Shop_Tog_Tip_DebtLimit));
					}
				}
				tip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("arg0", stringBuilder.ToString().ColorReplace()).Set("Width", 600f);
				EasyPool.Free<StringBuilder>(stringBuilder);
			}
			else
			{
				tog.gameObject.SetActive(false);
			}
			bool flag5 = autoCheckKey < (int)this._merchantData.GroupConfig.Level;
			if (flag5)
			{
				autoCheckKey = (int)this._merchantData.GroupConfig.Level;
			}
			refers.CGet<GameObject>("Normal").SetActive(true);
			refers.CGet<GameObject>("Label").SetActive(true);
			refers.CGet<GameObject>("Disabled").SetActive(false);
			refers.CGet<GameObject>("BuyCountBg").SetActive(true);
			short originBuyCount = this._originMerchantOverFavorData.MerchantOverFavorLevelDataArray[i].BuyCount;
			short curBuyCount = this._tempMerchantOverFavorData.MerchantOverFavorLevelDataArray[i].BuyCount;
			short maxBuyCount = this.GetMaxBuyCount(i);
			bool flag6 = this._tradeItemsSelf.Any(new Func<ItemDisplayData, bool>(UI_Shop.IsSealOfMerchant));
			if (flag6)
			{
				curBuyCount = maxBuyCount;
			}
			string color = (curBuyCount < originBuyCount) ? "brightred" : ((curBuyCount > originBuyCount) ? "brightblue" : "pinkyellow");
			refers.CGet<TextMeshProUGUI>("BuyCount").text = (isOverProgress ? string.Format("{0}/{1}", curBuyCount.ToString().SetColor(color), maxBuyCount) : LocalStringManager.Get(LanguageKey.LK_Infinity));
		}
		bool flag7 = targetTogIndex > -1;
		if (flag7)
		{
			autoCheckKey = targetTogIndex;
		}
		this._shopItemTogGroup.Set(autoCheckKey, true, true);
		this.InitBuyBack();
	}

	// Token: 0x06002D33 RID: 11571 RVA: 0x00163FDC File Offset: 0x001621DC
	private void InitBuyBack()
	{
		CToggleGroupObsolete toggleGroup = base.CGet<CToggleGroupObsolete>("BuyBackToggleGroup");
		toggleGroup.gameObject.SetActive(!this._openShopEventArguments.IsSettlementTreasury);
		bool isSettlementTreasury = this._openShopEventArguments.IsSettlementTreasury;
		if (!isSettlementTreasury)
		{
			toggleGroup.InitPreOnToggle(-1);
			CToggleGroupObsolete ctoggleGroupObsolete = toggleGroup;
			if (ctoggleGroupObsolete.OnActiveToggleChange == null)
			{
				ctoggleGroupObsolete.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnActiveToggleChange);
			}
			toggleGroup.SetWithoutNotify(this._isClickedBuyBack ? UI_Shop.BuyModeTogKey.BuyBack.ToInt() : UI_Shop.BuyModeTogKey.Buy.ToInt(), true);
			CToggleObsolete btnBuyBack = toggleGroup.Get(UI_Shop.BuyModeTogKey.BuyBack.ToInt());
			bool canBuyBack = this._shopItemList[7].Count > 0;
			btnBuyBack.interactable = canBuyBack;
		}
	}

	// Token: 0x06002D34 RID: 11572 RVA: 0x0016409C File Offset: 0x0016229C
	private void OnActiveToggleChange(CToggleObsolete newTog, CToggleObsolete oldTog)
	{
		bool isClickedBuyBack = this._isClickedBuyBack;
		if (isClickedBuyBack)
		{
			this._isClickedBuyBack = false;
			Action<bool> refreshActiveStates = this._refreshActiveStates;
			if (refreshActiveStates != null)
			{
				refreshActiveStates(true);
			}
			bool flag = this._lastGoodsTogKey > -1;
			if (flag)
			{
				this._shopItemTogGroup.Set(this._lastGoodsTogKey, true, true);
			}
		}
		else
		{
			Action<bool> refreshActiveStates2 = this._refreshActiveStates;
			if (refreshActiveStates2 != null)
			{
				refreshActiveStates2(false);
			}
			this._shopItemScroll.SetItemList(ref this._shopItemList[7], false, null, null, null, true);
			this.UpdateAllShopItemInteractable();
			this._isClickedBuyBack = true;
			this._lastGoodsTogKey = this._shopItemTogGroup.GetActive().Key;
			this._shopItemTogGroup.Set(this._lastGoodsTogKey, false, false);
		}
	}

	// Token: 0x06002D35 RID: 11573 RVA: 0x00164160 File Offset: 0x00162360
	protected override void OnClick(Transform btn)
	{
		bool flag = !this.Element.IsShowing;
		if (!flag)
		{
			string name = btn.name;
			string text = name;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num <= 1438754770U)
			{
				if (num <= 74328841U)
				{
					if (num != 38372828U)
					{
						if (num == 74328841U)
						{
							if (text == "ButtonInfo")
							{
								this.ShowMerchantInfo();
							}
						}
					}
					else if (text == "ButtonReset")
					{
						this.OnClickReset();
					}
				}
				else if (num != 224241645U)
				{
					if (num != 890545507U)
					{
						if (num == 1438754770U)
						{
							if (text == "ButtonBuyAll")
							{
								this.OnClickButtonBuyAll();
							}
						}
					}
					else if (text == "ButtonConfirm")
					{
						this.OnClickConfirm();
					}
				}
				else if (text == "ButtonReplenish")
				{
					this.OnClickButtonReplenish();
				}
			}
			else if (num <= 2762327207U)
			{
				if (num != 2219685708U)
				{
					if (num == 2762327207U)
					{
						if (text == "ButtonBalance")
						{
							this.OnClickButtonBalance();
						}
					}
				}
				else if (text == "Avatar_BigSize")
				{
					bool flag2 = this._isNormalCharacter || (this._openShopEventArguments.IsSettlementTreasury && this._merchantCharData != null) || this._isCharacterOnSpecificatedMerchant;
					if (flag2)
					{
						ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
						argBox.Set("CharacterId", this._isCharacterOnSpecificatedMerchant ? this._openShopEventArguments.Id : this._merchantCharData.CharacterId);
						UIElement.CharacterMenu.SetOnInitArgs(argBox);
						UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
					}
					bool flag3 = this._openShopEventArguments.IsCaravan && this._merchantId >= 0;
					if (flag3)
					{
						this.BubbleStartCoroutine(this._merchantTypeConfig.IntroduceDialog, BubbleLevel.Middle, false);
					}
					bool isFromBuilding = this._openShopEventArguments.IsFromBuilding;
					if (isFromBuilding)
					{
						int index = Random.Range(1, 4);
						string dialog = LocalStringManager.Get(string.Format("LK_Shop_Speak_ClickCharacter{0}", index));
						this.BubbleStartCoroutine(dialog, BubbleLevel.Middle, false);
					}
				}
			}
			else if (num != 3005249876U)
			{
				if (num != 3482682496U)
				{
					if (num == 3660725027U)
					{
						if (text == "BtnTreasuryRecord")
						{
							this.OnClickBtnTreasuryRecord();
						}
					}
				}
				else if (text == "ButtonSellAll")
				{
					this.OnClickButtonSellAll();
				}
			}
			else if (text == "CommonButtonClose")
			{
				this.QuickHide();
			}
		}
	}

	// Token: 0x06002D36 RID: 11574 RVA: 0x0016444C File Offset: 0x0016264C
	public override void QuickHide()
	{
		bool flag = this._openShopEventArguments.IsSettlementTreasury && !this._confirmedTreasuryOperation;
		if (flag)
		{
			bool isBreaking = this.IsBreaking;
			if (isBreaking)
			{
				UIElement dialog = UIElement.Dialog;
				ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
				string key = "Cmd";
				DialogCmd dialogCmd = new DialogCmd();
				dialogCmd.Type = 1;
				dialogCmd.Title = LanguageKey.LK_Building_LeaveTreasury.Tr();
				dialogCmd.Content = LanguageKey.LK_Building_LeaveTreasury_Desc.Tr();
				dialogCmd.Yes = new Action(this.<QuickHide>g__TreasuryHideActions|149_0);
				dialogCmd.No = delegate()
				{
				};
				dialog.SetOnInitArgs(argumentBox.SetObject(key, dialogCmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
				return;
			}
		}
		this.<QuickHide>g__HideActions|149_1();
	}

	// Token: 0x06002D37 RID: 11575 RVA: 0x00164520 File Offset: 0x00162720
	public bool IsPageDisabled(int index)
	{
		bool flag = index == 7 || index == UI_Shop.TradeItemSource.SettlementTreasury.ToInt();
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			short curBuyCount = this._tempMerchantOverFavorData.MerchantOverFavorLevelDataArray[index].BuyCount;
			bool isPageOverDebt = curBuyCount <= 0;
			bool favorIsMeet = !isPageOverDebt || this._openShopEventArguments.IgnoreFavorability;
			result = !favorIsMeet;
		}
		return result;
	}

	// Token: 0x06002D38 RID: 11576 RVA: 0x00164584 File Offset: 0x00162784
	public static bool IsReachProgressLimit(int merchantFavorability, out int worldProgressLimitedLevel, out int worldProgressLimitedFavor)
	{
		return CommonUtils.IsMerchantFavorabilityReachProgressLimit(merchantFavorability, out worldProgressLimitedLevel, out worldProgressLimitedFavor);
	}

	// Token: 0x06002D39 RID: 11577 RVA: 0x00164590 File Offset: 0x00162790
	private bool IsPageShow(int index)
	{
		bool flag = index == 7;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = this._merchantData == null;
			if (flag2)
			{
				result = false;
			}
			else
			{
				MerchantItem merchantConfig = Merchant.Instance[this._merchantData.MerchantTemplateId];
				MerchantItem merchantGroupConfig = Merchant.Instance[merchantConfig.GroupId];
				sbyte goodsLevelMax = merchantConfig.Level;
				sbyte goodLevelMin = merchantGroupConfig.Level;
				bool levelIsMeet = index >= (int)goodLevelMin && index <= (int)goodsLevelMax;
				result = levelIsMeet;
			}
		}
		return result;
	}

	// Token: 0x06002D3A RID: 11578 RVA: 0x00164610 File Offset: 0x00162810
	private void OnShopItemTogChange(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		bool flag = togNew;
		if (flag)
		{
			this._shopItemScroll.SetItemList(ref this._shopItemList[togNew.Key], false, null, null, null, true);
			this.UpdateAllShopItemInteractable();
			bool isClickedBuyBack = this._isClickedBuyBack;
			if (isClickedBuyBack)
			{
				this._lastGoodsTogKey = -1;
				this._isClickedBuyBack = false;
				this.InitBuyBack();
			}
			else
			{
				Action<bool> refreshActiveStates = this._refreshActiveStates;
				if (refreshActiveStates != null)
				{
					refreshActiveStates(true);
				}
			}
			this._lastGoodsTogKey = togNew.Key;
			bool showMask = this.IsPageDisabled(togNew.Key) && !this._isClickedBuyBack;
			this._shopItemMask.SetActive(showMask);
		}
		else
		{
			bool flag2 = togOld;
			if (flag2)
			{
				this._shopItemMask.SetActive(false);
			}
		}
	}

	// Token: 0x06002D3B RID: 11579 RVA: 0x001646DD File Offset: 0x001628DD
	private void OnSelfItemTogChange(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		this.RefreshSelfItems();
		this.RefreshTreasuryBalanceButton();
	}

	// Token: 0x06002D3C RID: 11580 RVA: 0x001646F0 File Offset: 0x001628F0
	private void RefreshSelfItems()
	{
		UI_Shop.TogType togType = (UI_Shop.TogType)this.CurSelfItemListIndex;
		LanguageKey stringKey = LanguageKey.Invalid;
		switch (togType)
		{
		case UI_Shop.TogType.Inventory:
			stringKey = LanguageKey.LK_Inventory;
			break;
		case UI_Shop.TogType.Warehouse:
			stringKey = LanguageKey.LK_Warehouse;
			break;
		case UI_Shop.TogType.Treasury:
			stringKey = LanguageKey.LK_Treasury;
			break;
		}
		base.CGet<TextMeshProUGUI>("SelfItemTitle").text = LocalStringManager.Get(stringKey);
		List<ItemDisplayData> itemList = this.CurSelfItemList;
		this._selfItemScroll.SetItemList(ref itemList, false, null, null, null, true);
		this.UpdateAllSelfItemInteractable();
		this.RefreshLoad();
		GameObject sellingMask = base.CGet<GameObject>("SellingMask");
		sellingMask.SetActive(this._sellingDisabled);
	}

	// Token: 0x06002D3D RID: 11581 RVA: 0x00164794 File Offset: 0x00162994
	private void OnsettlementTreasuryTogChange(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		this._isGetLastSettlementTreasuryOperationData = false;
		OrganizationItem orgConfig = Organization.Instance[this._settlementTreasuryDisplayData.OrgTemplateId];
		sbyte templateId = orgConfig.TemplateId;
		TaiwuEventDomainMethod.Call.SettlementTreasuryBuildingClicked((short)templateId, 0, (sbyte)this.SettlementTreasuryLayerIndex);
		this._settlementTreasuryTogGroup.SetWithoutNotify(togOld.Key, true);
	}

	// Token: 0x06002D3E RID: 11582 RVA: 0x001647E8 File Offset: 0x001629E8
	public void UpdateSettlementTreasuryTog(int index, byte enterSatte)
	{
		this._settlementTreasuryTogGroup.SetWithoutNotify(index, true);
	}

	// Token: 0x06002D3F RID: 11583 RVA: 0x001647FC File Offset: 0x001629FC
	private void RefreshExtraGoodsMark(ItemDisplayData itemData, Refers view, bool show)
	{
		MerchantExtraGoodsData.ExtraGoodsType extraGoodsType;
		bool isExtra = this.CheckIsExtra(itemData.Key.Id, out extraGoodsType);
		sbyte seasonTemplateId = isExtra ? this._extraGoods.SeasonTemplateId : -1;
		ItemView itemView = view as ItemView;
		bool flag = itemView != null;
		if (flag)
		{
			itemView.RefreshExtraGoodsMark(show && isExtra, extraGoodsType, seasonTemplateId);
		}
		else
		{
			CommonTableRowForItem tableRowForItem = view as CommonTableRowForItem;
			bool flag2 = tableRowForItem != null;
			if (flag2)
			{
				tableRowForItem.RefreshExtraGoodsMark(show && isExtra, extraGoodsType, seasonTemplateId);
			}
		}
	}

	// Token: 0x06002D40 RID: 11584 RVA: 0x00164870 File Offset: 0x00162A70
	private void ShowItemPrice(ItemDisplayData itemData, Refers itemView, bool isBuy, long price = -1L, int priceChangePercentValue = -1, bool showTotalPrice = false)
	{
		UI_Shop.<>c__DisplayClass159_0 CS$<>8__locals1;
		CS$<>8__locals1.itemView = itemView;
		bool flag = priceChangePercentValue == -1;
		if (flag)
		{
			priceChangePercentValue = this.GetPriceChangePercentValue(itemData, isBuy);
		}
		bool flag2 = price == -1L;
		if (flag2)
		{
			price = (long)this.GetItemPrice(itemData, isBuy, priceChangePercentValue);
		}
		CS$<>8__locals1.itemView.UserFloat = (float)price;
		bool isResource = ItemTemplateHelper.IsMiscResource(itemData.Key.ItemType, itemData.Key.TemplateId);
		long totalPrice = (showTotalPrice && !isResource) ? (price * (long)itemData.Amount) : price;
		string priceContent = this.IsAreaDebtShop ? WorldMapModel.GetFormatSpiritualDebt((int)totalPrice, 0) : CommonUtils.GetDisplayStringForNum(totalPrice);
		bool allowTrade = ItemTemplateHelper.AllowTrade(itemData.Key.ItemType, itemData.Key.TemplateId);
		int basePrice = this.GetItemBasePrice(itemData, isBuy);
		int changedPrice = basePrice * priceChangePercentValue / 100;
		bool showPriceDelta = changedPrice != 0;
		bool isAdd = priceChangePercentValue > 0;
		bool isBadForTaiwu = (isAdd && isBuy) || (!isAdd && !isBuy);
		string color = isBadForTaiwu ? "brightred" : "brightblue";
		string priceChangeValueText = isAdd ? string.Format("+{0}", changedPrice) : string.Format("{0}", changedPrice);
		string priceChangePercentValueText = isAdd ? string.Format("+{0}%", priceChangePercentValue) : string.Format("{0}%", priceChangePercentValue);
		string tipContent = LanguageKey.LK_Shop_PriceStateTip.TrFormat(priceChangeValueText.SetColor(color), priceChangePercentValueText.SetColor(color));
		CommonTableRowForItem tableRowForItem = CS$<>8__locals1.itemView as CommonTableRowForItem;
		bool flag3 = tableRowForItem != null;
		if (flag3)
		{
			tableRowForItem.SetValue(priceContent);
			EItemPriceState priceState = allowTrade ? ((changedPrice > 0) ? EItemPriceState.Up : ((changedPrice < 0) ? EItemPriceState.Down : EItemPriceState.None)) : EItemPriceState.None;
			tableRowForItem.SetPriceState(priceState, tipContent);
		}
		else
		{
			TextMeshProUGUI priceLabel = CS$<>8__locals1.itemView.CGet<TextMeshProUGUI>("Price");
			priceLabel.text = priceContent;
			CS$<>8__locals1.priceRoot = priceLabel.rectTransform.parent;
			Transform priceDelta = CS$<>8__locals1.priceRoot.Find("PriceDelta");
			bool flag4 = allowTrade;
			if (flag4)
			{
				UI_Shop.<ShowItemPrice>g__ShowPrice|159_0(true, ref CS$<>8__locals1);
				bool flag5 = priceDelta;
				if (flag5)
				{
					priceDelta.gameObject.SetActive(showPriceDelta);
					bool flag6 = showPriceDelta;
					if (flag6)
					{
						TooltipInvoker tip = priceDelta.GetComponent<TooltipInvoker>();
						tip.Type = TipType.SingleDesc;
						string[] presetParam = tip.PresetParam;
						bool flag7 = presetParam == null || presetParam.Length != 1;
						if (flag7)
						{
							tip.PresetParam = new string[1];
						}
						tip.PresetParam[0] = tipContent;
						CImage priceDeltaBg = priceDelta.GetComponent<CImage>();
						bool flag8 = priceDeltaBg;
						if (flag8)
						{
							priceDeltaBg.SetSprite(isBadForTaiwu ? "shop_itemdiscount_1" : "shop_itemdiscount_0", false, null);
						}
						Transform transform = priceDelta.Find("Value");
						TextMeshProUGUI text = (transform != null) ? transform.GetComponent<TextMeshProUGUI>() : null;
						bool flag9 = text;
						if (flag9)
						{
							string content = isAdd ? string.Format("+{0}%", priceChangePercentValue) : string.Format("{0}%", priceChangePercentValue);
							text.text = content.SetColor(color);
						}
						Transform transform2 = priceDelta.Find("Icon");
						CImage icon = (transform2 != null) ? transform2.GetComponent<CImage>() : null;
						bool flag10 = icon;
						if (flag10)
						{
							icon.SetSprite(isAdd ? "shop_wupinxinxi_0" : "shop_wupinxinxi_1", false, null);
						}
					}
				}
			}
			else
			{
				UI_Shop.<ShowItemPrice>g__ShowPrice|159_0(false, ref CS$<>8__locals1);
			}
		}
	}

	// Token: 0x06002D41 RID: 11585 RVA: 0x00164BE4 File Offset: 0x00162DE4
	private bool UpdateShopItemInteractable(ItemDisplayData itemData, out LanguageKey tip)
	{
		tip = LanguageKey.Invalid;
		bool interactable = this._openShopEventArguments.IsSettlementTreasury ? (this._tradeItemsMerchant.Count < GlobalConfig.Instance.SettlementTreasuryGetItemMaxCount) : (!this.IsPageDisabled(this.CurShopItemListIndex));
		bool flag = !interactable;
		if (flag)
		{
			tip = LanguageKey.LK_ItemSelectCountIsMax;
		}
		bool flag2 = interactable && this._openShopEventArguments.IsSettlementTreasury && itemData.IsResource;
		if (flag2)
		{
			sbyte resourceType = ItemTemplateHelper.GetMiscResourceType(itemData.Key.ItemType, itemData.Key.TemplateId);
			int minCount = this.CalcResourceAmount(resourceType, GlobalConfig.Instance.SettlementTreasuryGetResourceMinValue);
			bool flag3 = itemData.Amount < minCount;
			if (flag3)
			{
				interactable = false;
				tip = LanguageKey.LK_ItemAmountIsLessThanMin;
			}
		}
		itemData.Interactable = interactable;
		return interactable;
	}

	// Token: 0x06002D42 RID: 11586 RVA: 0x00164CB4 File Offset: 0x00162EB4
	private void UpdateAllShopItemInteractable()
	{
		foreach (ItemDisplayData itemData in this._shopItemScroll.OutputItemList)
		{
			LanguageKey languageKey;
			this.UpdateShopItemInteractable(itemData, out languageKey);
		}
	}

	// Token: 0x06002D43 RID: 11587 RVA: 0x00164D14 File Offset: 0x00162F14
	private void OnRenderShopItem(ItemDisplayData itemData, CommonTableRowForItem itemView)
	{
		bool curShopTogIsBugBack = this.CurShopTogIsBugBack;
		if (curShopTogIsBugBack)
		{
			long price = this._merchantBuyBackData.BuyInPrice[itemData.RealKey];
			int priceChange = 0;
			this.ShowItemPrice(itemData, itemView, true, price, priceChange, false);
		}
		else
		{
			this.ShowItemPrice(itemData, itemView, true, -1L, -1, false);
		}
		itemView.UserString = "shop";
		itemView.SetLocked(false);
		LanguageKey tip;
		bool interactable = this.UpdateShopItemInteractable(itemData, out tip);
		bool flag = interactable;
		if (flag)
		{
			itemView.HideInteractionState();
		}
		else
		{
			itemView.SetItemNotCanSelectReason(LocalStringManager.Get(tip).SetColor("brightred"));
		}
		itemView.SetInteractable(interactable);
		itemView.SetClickEvent(delegate
		{
			this._shopItemScroll.HandleClickItem(itemData, itemView, new Action<CommonTableRowForItem>(base.<OnRenderShopItem>g__Action|1));
		});
		this.RefreshTreasuryLoveItem(itemData, itemView, true);
		this.RefreshPriceIcon(itemView);
		this.SetResourceItemTip(itemView, itemView.Data.IsResource, false);
		this.RefreshExtraGoodsMark(itemData, itemView, true);
		UI_Shop.TradeItemSource itemSource = (UI_Shop.TradeItemSource)this.CurShopItemListIndex;
		this.CalcDebt(itemData, itemSource, false, true);
		itemView.SetShopLevelMark(itemData.ItemDebtState, itemData.ItemShopLevel);
	}

	// Token: 0x06002D44 RID: 11588 RVA: 0x00164EAC File Offset: 0x001630AC
	private void PutShopItemToTrade(CommonTableRowForItem itemView)
	{
		bool isLocked = itemView.IsLocked;
		if (!isLocked)
		{
			int price = (int)itemView.UserFloat;
			bool flag = itemView.Data.Amount > 1;
			if (flag)
			{
				ItemScrollViewForCommonTableRow scrollView = itemView.GetComponentInParent<ItemScrollViewForCommonTableRow>(true);
				bool flag2 = scrollView == null;
				if (!flag2)
				{
					int index = scrollView.OutputItemList.IndexOf(itemView.Data);
					bool isResource = ItemTemplateHelper.IsMiscResource(itemView.Data.Key.ItemType, itemView.Data.Key.TemplateId);
					int maxCount = itemView.Data.Amount;
					int minCount = 1;
					bool isSettlementTreasury = this._openShopEventArguments.IsSettlementTreasury;
					if (isSettlementTreasury)
					{
						bool flag3 = isResource;
						if (flag3)
						{
							sbyte resourceType = ItemTemplateHelper.GetMiscResourceType(itemView.Data.Key.ItemType, itemView.Data.Key.TemplateId);
							maxCount = Debts.WorthToResourceAmount((short)resourceType, (long)this._settlementTreasuryResourceMaxValue, false);
							minCount = this.CalcResourceAmount(resourceType, GlobalConfig.Instance.SettlementTreasuryGetResourceMinValue);
						}
						else
						{
							maxCount = GlobalConfig.Instance.SettlementTreasuryGetItemMaxCount - this._tradeItemsMerchant.Count;
						}
					}
					scrollView.SetItemToSelectCountMode(index, itemView, delegate(int count)
					{
						this.ExitHighLight(scrollView);
						this.PutShopItemToTrade(itemView.Data, count, price, true, true);
					}, delegate
					{
						this.ExitHighLight(scrollView);
					}, 0, maxCount, minCount, LocalStringManager.Get(LanguageKey.LK_SelectCount_Limit_Shop), false, null, null, -1);
					this.HighLightItemView(scrollView, itemView, true);
				}
			}
			else
			{
				this.PutShopItemToTrade(itemView.Data, 1, price, true, true);
			}
		}
	}

	// Token: 0x06002D45 RID: 11589 RVA: 0x001650D8 File Offset: 0x001632D8
	private void PutShopItemToTrade(ItemDisplayData itemData, int count, int price, bool playSound = true, bool refresh = true)
	{
		if (playSound)
		{
			this.PlayMoveItemSound();
		}
		List<ItemDisplayData> shopItemList = this.CurShopItemList;
		UI_Shop.TradeItemSource sourceType = UI_Shop.TradeItemSource.ShopList0 + this.CurShopItemListIndex;
		bool isResource = ItemTemplateHelper.IsMiscResource(itemData.Key.ItemType, itemData.Key.TemplateId);
		bool flag = isResource;
		if (flag)
		{
			bool isSettlementTreasury = this._openShopEventArguments.IsSettlementTreasury;
			if (isSettlementTreasury)
			{
				ItemDisplayData tradeItem = itemData.Clone(count);
				tradeItem.ItemSourceType = sourceType.ToSbyte();
				this._tradeItemsMerchant.Add(tradeItem);
				this._tradeItemSourceDict.Add(tradeItem, sourceType);
				itemData.Amount -= count;
				bool flag2 = itemData.Amount <= 0;
				if (flag2)
				{
					shopItemList.Remove(itemData);
				}
			}
		}
		int money = price * count;
		this._tradeMoneySources[itemData.Key] = this._tradeMoneySources.GetOrDefault(itemData.Key, 0) - money;
		bool isBuyShopItem = sourceType < UI_Shop.TradeItemSource.ShopBuyBackList;
		bool flag3 = isBuyShopItem;
		if (flag3)
		{
			this._buyMoney += money;
		}
		if (refresh)
		{
			this.RefreshAfterPutShopItemToTrade();
		}
	}

	// Token: 0x06002D46 RID: 11590 RVA: 0x001651F8 File Offset: 0x001633F8
	private void RefreshAfterPutShopItemToTrade()
	{
		this.SortTradeItems(this._tradeItemsMerchant);
		this.UpdateTradeAreaDisplay();
		this.RefreshTreasuryTrade();
		List<ItemDisplayData> shopItemList = this.CurShopItemList;
		this._shopItemScroll.SetItemList(ref shopItemList, false, null, null, null, true);
		this.UpdateAllShopItemInteractable();
		bool isSettlementTreasury = this._openShopEventArguments.IsSettlementTreasury;
		if (isSettlementTreasury)
		{
			this._getFromTreasuryScrollView.SetItemList(ref this._tradeItemsMerchant, false, null, false, null);
			this._giveToTreasuryScrollView.ReRender();
		}
		else
		{
			this._tradeMerchantScrollView.SetItemList(ref this._tradeItemsMerchant, false, null, null, null, true);
		}
		List<ItemDisplayData> selfItemList = this.CurSelfItemList;
		this._selfItemScroll.SetItemList(ref selfItemList, false, null, null, null, true);
		this.UpdateAllSelfItemInteractable();
		base.CGet<CButtonObsolete>("ButtonReset").interactable = true;
	}

	// Token: 0x06002D47 RID: 11591 RVA: 0x001652C4 File Offset: 0x001634C4
	private Action<ItemDisplayData, ItemView> OnRenderTradeItemGeneratorForSettlement(IDictionary<ItemDisplayData, UI_Shop.TradeItemSource> itemDict)
	{
		UI_Shop.<>c__DisplayClass166_0 CS$<>8__locals1 = new UI_Shop.<>c__DisplayClass166_0();
		CS$<>8__locals1.itemDict = itemDict;
		CS$<>8__locals1.<>4__this = this;
		return delegate(ItemDisplayData itemData, ItemView itemView)
		{
			UI_Shop.TradeItemSource sourceType = CS$<>8__locals1.itemDict[itemData];
			bool isBuy = sourceType < UI_Shop.TradeItemSource.Inventory || sourceType == UI_Shop.TradeItemSource.SettlementTreasury;
			int priceChange = (sourceType != UI_Shop.TradeItemSource.ShopBuyBackList && !CS$<>8__locals1.<>4__this.IsAreaDebtShop) ? CS$<>8__locals1.<>4__this.GetPriceChangePercentValue(itemData, isBuy) : 0;
			long price = (sourceType != UI_Shop.TradeItemSource.ShopBuyBackList) ? ((long)CS$<>8__locals1.<>4__this.GetItemPrice(itemData, isBuy, priceChange)) : CS$<>8__locals1.<>4__this._merchantBuyBackData.BuyInPrice[itemData.Key];
			itemView.UserString = "trade";
			CS$<>8__locals1.<>4__this.ShowItemPrice(itemData, itemView, isBuy, price, priceChange, true);
			itemView.SetClickEvent(delegate
			{
				ItemScrollView scroll = isBuy ? CS$<>8__locals1.<>4__this._getFromTreasuryScrollView : CS$<>8__locals1.<>4__this._giveToTreasuryScrollView;
				scroll.HandleClickItem(itemData, itemView, new Action<ItemView>(CS$<>8__locals1.<OnRenderTradeItemGeneratorForSettlement>g__Action|2));
			});
			CS$<>8__locals1.<>4__this.RefreshTreasuryLoveItem(itemData, itemView, true);
			CS$<>8__locals1.<>4__this.RefreshPriceIcon(itemView);
			CS$<>8__locals1.<>4__this.RefreshTreasuryTradeItem(itemData, itemView);
			CS$<>8__locals1.<>4__this.SetResourceItemTip(itemView, itemView.Data.IsResource, !isBuy);
			itemView.SetShopLevelMark(itemData.ItemDebtState, itemData.ItemShopLevel);
			CS$<>8__locals1.<>4__this.RefreshExtraGoodsMark(itemData, itemView, false);
		};
	}

	// Token: 0x06002D48 RID: 11592 RVA: 0x001652F8 File Offset: 0x001634F8
	private Action<ItemDisplayData, CommonTableRowForItem> OnRenderTradeItemGenerator(IDictionary<ItemDisplayData, UI_Shop.TradeItemSource> itemDict)
	{
		UI_Shop.<>c__DisplayClass167_0 CS$<>8__locals1 = new UI_Shop.<>c__DisplayClass167_0();
		CS$<>8__locals1.itemDict = itemDict;
		CS$<>8__locals1.<>4__this = this;
		return delegate(ItemDisplayData itemData, CommonTableRowForItem itemView)
		{
			UI_Shop.TradeItemSource sourceType = CS$<>8__locals1.itemDict[itemData];
			bool isBuy = sourceType < UI_Shop.TradeItemSource.Inventory || sourceType == UI_Shop.TradeItemSource.SettlementTreasury;
			int priceChange = (sourceType != UI_Shop.TradeItemSource.ShopBuyBackList && !CS$<>8__locals1.<>4__this.IsAreaDebtShop) ? CS$<>8__locals1.<>4__this.GetPriceChangePercentValue(itemData, isBuy) : 0;
			long price = (sourceType != UI_Shop.TradeItemSource.ShopBuyBackList) ? ((long)CS$<>8__locals1.<>4__this.GetItemPrice(itemData, isBuy, priceChange)) : CS$<>8__locals1.<>4__this._merchantBuyBackData.BuyInPrice[itemData.Key];
			itemView.UserString = "trade";
			CS$<>8__locals1.<>4__this.ShowItemPrice(itemData, itemView, isBuy, price, priceChange, true);
			itemView.SetClickEvent(delegate
			{
				ItemScrollViewForCommonTableRow scroll = isBuy ? CS$<>8__locals1.<>4__this._tradeMerchantScrollView : CS$<>8__locals1.<>4__this._tradeSelfScrollView;
				scroll.HandleClickItem(itemData, itemView, new Action<CommonTableRowForItem>(CS$<>8__locals1.<OnRenderTradeItemGenerator>g__Action|2));
			});
			CS$<>8__locals1.<>4__this.RefreshTreasuryLoveItem(itemData, itemView, true);
			CS$<>8__locals1.<>4__this.RefreshPriceIcon(itemView);
			CS$<>8__locals1.<>4__this.RefreshTreasuryTradeItem(itemData, itemView);
			CS$<>8__locals1.<>4__this.SetResourceItemTip(itemView, itemView.Data.IsResource, !isBuy);
			itemView.SetShopLevelMark(itemData.ItemDebtState, itemData.ItemShopLevel);
			CS$<>8__locals1.<>4__this.RefreshExtraGoodsMark(itemData, itemView, false);
		};
	}

	// Token: 0x06002D49 RID: 11593 RVA: 0x0016532C File Offset: 0x0016352C
	private void RemoveTradeItem(ItemView itemView)
	{
		bool flag = itemView.Data.Amount > 1;
		if (flag)
		{
			ItemScrollView scrollView = itemView.GetComponentInParent<ItemScrollView>(true);
			bool flag2 = scrollView == null;
			if (!flag2)
			{
				int index = scrollView.SortAndFilter.OutputItemList.IndexOf(itemView.Data);
				bool flag3 = this._openShopEventArguments.IsSettlementTreasury && itemView.Data.IsResource;
				if (flag3)
				{
					this.RemoveTradeItem(itemView, itemView.Data, itemView.Data.Amount);
				}
				else
				{
					scrollView.SetItemToSelectCountMode(index, itemView, delegate(int count)
					{
						this.ExitHighLight(scrollView);
						this.RemoveTradeItem(itemView, itemView.Data, count);
					}, delegate
					{
						this.ExitHighLight(scrollView);
					}, 0, 0, 1, null, false, null, null, -1);
					this.HighLightItemView(scrollView, itemView, !this._openShopEventArguments.IsSettlementTreasury);
				}
			}
		}
		else
		{
			this.RemoveTradeItem(itemView, itemView.Data, 1);
		}
	}

	// Token: 0x06002D4A RID: 11594 RVA: 0x001654AC File Offset: 0x001636AC
	private void RemoveTradeItem(CommonTableRowForItem itemView)
	{
		bool flag = itemView.Data.Amount > 1;
		if (flag)
		{
			ItemScrollViewForCommonTableRow scrollView = itemView.GetComponentInParent<ItemScrollViewForCommonTableRow>(true);
			bool flag2 = scrollView == null;
			if (!flag2)
			{
				int index = scrollView.OutputItemList.IndexOf(itemView.Data);
				bool flag3 = this._openShopEventArguments.IsSettlementTreasury && itemView.Data.IsResource;
				if (flag3)
				{
					this.RemoveTradeItem(itemView, itemView.Data, itemView.Data.Amount);
				}
				else
				{
					scrollView.SetItemToSelectCountMode(index, itemView, delegate(int count)
					{
						this.ExitHighLight(scrollView);
						this.RemoveTradeItem(itemView, itemView.Data, count);
					}, delegate
					{
						this.ExitHighLight(scrollView);
					}, 0, 0, 1, null, false, null, null, -1);
					this.HighLightItemView(scrollView, itemView, !this._openShopEventArguments.IsSettlementTreasury);
				}
			}
		}
		else
		{
			this.RemoveTradeItem(itemView, itemView.Data, 1);
		}
	}

	// Token: 0x06002D4B RID: 11595 RVA: 0x00165628 File Offset: 0x00163828
	private void RemoveTradeItem(Refers itemView, ItemDisplayData itemData, int count)
	{
		this.PlayMoveItemSound();
		UI_Shop.TradeItemSource sourceType = this._tradeItemSourceDict[itemData];
		bool fromShop = sourceType < UI_Shop.TradeItemSource.Inventory || sourceType == UI_Shop.TradeItemSource.SettlementTreasury;
		List<ItemDisplayData> tradeItemList = this.GetTradeItemList(sourceType);
		ItemDisplayData itemInList = tradeItemList.Find((ItemDisplayData data) => data.CanMerge(itemData));
		bool isResource = ItemTemplateHelper.IsMiscResource(itemData.Key.ItemType, itemData.Key.TemplateId);
		bool flag = isResource;
		if (flag)
		{
			bool flag2 = itemInList == null;
			if (flag2)
			{
				itemInList = itemData.Clone(count);
				bool flag3 = !fromShop;
				if (flag3)
				{
					itemInList.UsingType = itemData.UsingType;
				}
				tradeItemList.Add(itemInList);
			}
			else
			{
				itemInList.Amount += count;
			}
			itemData.Amount -= count;
		}
		itemInList.TreasuryOperation = ETreasuryOperation.Invalid;
		bool flag4 = itemData.Amount <= 0;
		if (flag4)
		{
			this._currentOperationData = itemData;
			bool flag5 = fromShop;
			if (flag5)
			{
				this._tradeItemsMerchant.Remove(itemData);
			}
			else
			{
				this._tradeItemsSelf.Remove(itemData);
			}
			this._tradeItemSourceDict.Remove(itemData);
		}
		int money = (int)itemView.UserFloat * (fromShop ? count : (-count));
		int tradeMoney;
		bool flag6 = !this._tradeMoneySources.TryGetValue(itemData.Key, out tradeMoney);
		if (flag6)
		{
			this._tradeMoneySources.Add(itemData.Key, tradeMoney = 0);
		}
		tradeMoney += money;
		this._tradeMoneySources[itemData.Key] = tradeMoney;
		this.SortTradeItems(this._tradeItemsMerchant);
		this.SortTradeItems(this._tradeItemsSelf);
		bool flag7 = fromShop;
		if (flag7)
		{
			bool flag8 = sourceType < UI_Shop.TradeItemSource.ShopBuyBackList;
			if (flag8)
			{
				this._buyMoney -= money;
			}
			List<ItemDisplayData> shopList = this.CurShopItemList;
			this._shopItemScroll.SetItemList(ref shopList, false, null, null, null, true);
			this.UpdateAllShopItemInteractable();
			bool isSettlementTreasury = this._openShopEventArguments.IsSettlementTreasury;
			if (isSettlementTreasury)
			{
				this._getFromTreasuryScrollView.SetItemList(ref this._tradeItemsMerchant, false, null, false, null);
				this._giveToTreasuryScrollView.ReRender();
			}
			else
			{
				this._tradeMerchantScrollView.SetItemList(ref this._tradeItemsMerchant, false, null, null, null, true);
			}
			List<ItemDisplayData> selfList = this.CurSelfItemList;
			this._selfItemScroll.SetItemList(ref selfList, false, null, null, null, true);
			this.UpdateAllSelfItemInteractable();
		}
		else
		{
			List<ItemDisplayData> selfList2 = this.CurSelfItemList;
			this._selfItemScroll.SetItemList(ref selfList2, false, null, null, null, true);
			this.UpdateAllSelfItemInteractable();
			bool isSettlementTreasury2 = this._openShopEventArguments.IsSettlementTreasury;
			if (isSettlementTreasury2)
			{
				this._giveToTreasuryScrollView.SetItemList(ref this._tradeItemsSelf, false, null, false, null);
				this._getFromTreasuryScrollView.ReRender();
			}
			else
			{
				this._tradeSelfScrollView.SetItemList(ref this._tradeItemsSelf, false, null, null, null, true);
			}
			List<ItemDisplayData> shopList2 = this.CurShopItemList;
			this._shopItemScroll.SetItemList(ref shopList2, false, null, null, null, true);
			this.UpdateAllShopItemInteractable();
		}
		this.UpdateTradeAreaDisplay();
		this.RefreshTreasuryTrade();
		base.CGet<CButtonObsolete>("ButtonReset").interactable = (this._tradeItemsMerchant.Count + this._tradeItemsSelf.Count > 0);
	}

	// Token: 0x06002D4C RID: 11596 RVA: 0x001659A4 File Offset: 0x00163BA4
	private bool UpdateSelfItemInteractable(ItemDisplayData itemData, out LanguageKey tip)
	{
		tip = LanguageKey.Invalid;
		bool locked = this._sellingDisabled || !ItemTemplateHelper.AllowTrade(itemData.Key.ItemType, itemData.Key.TemplateId);
		bool interactable = this._openShopEventArguments.IsSettlementTreasury ? (!locked && this._tradeItemsSelf.Count < GlobalConfig.Instance.SettlementTreasuryGiveItemMaxCount) : (!locked);
		bool flag = !interactable;
		if (flag)
		{
			tip = LanguageKey.LK_ItemSelectCountIsMax;
		}
		bool flag2 = interactable && this._openShopEventArguments.IsSettlementTreasury && itemData.IsResource;
		if (flag2)
		{
			sbyte resourceType = ItemTemplateHelper.GetMiscResourceType(itemData.Key.ItemType, itemData.Key.TemplateId);
			int minCount = this.CalcResourceAmount(resourceType, GlobalConfig.Instance.SettlementTreasuryGetResourceMinValue);
			bool flag3 = itemData.Amount < minCount;
			if (flag3)
			{
				interactable = false;
				tip = LanguageKey.LK_ItemAmountIsLessThanMin;
			}
		}
		itemData.Interactable = interactable;
		return interactable;
	}

	// Token: 0x06002D4D RID: 11597 RVA: 0x00165A98 File Offset: 0x00163C98
	private void UpdateAllSelfItemInteractable()
	{
		foreach (ItemDisplayData itemData in this._selfItemScroll.OutputItemList)
		{
			LanguageKey languageKey;
			this.UpdateSelfItemInteractable(itemData, out languageKey);
		}
	}

	// Token: 0x06002D4E RID: 11598 RVA: 0x00165AF8 File Offset: 0x00163CF8
	private void OnRenderSelfItem(ItemDisplayData itemData, CommonTableRowForItem itemView)
	{
		this.ShowItemPrice(itemData, itemView, false, -1L, -1, false);
		itemView.UserString = "self";
		bool locked = this._sellingDisabled || !ItemTemplateHelper.AllowTrade(itemData.Key.ItemType, itemData.Key.TemplateId);
		itemView.SetLocked(locked);
		bool interactable = !locked;
		bool flag = interactable;
		if (flag)
		{
			LanguageKey tip;
			interactable = this.UpdateSelfItemInteractable(itemData, out tip);
			itemView.SetInteractable(interactable);
			bool flag2 = interactable;
			if (flag2)
			{
				itemView.HideInteractionState();
			}
			else
			{
				itemView.SetItemNotCanSelectReason(LocalStringManager.Get(tip).SetColor("brightred"));
			}
		}
		itemView.SetClickEvent(delegate
		{
			bool isShowing = this.Element.IsShowing;
			if (isShowing)
			{
				this.OnClickSelfItem(itemView);
			}
		});
		this.RefreshTreasuryLoveItem(itemData, itemView, true);
		this.RefreshPriceIcon(itemView);
		this.SetResourceItemTip(itemView, itemView.Data.IsResource, true);
		this.RefreshExtraGoodsMark(itemData, itemView, false);
		this.SetVillagerNeedMark(itemView, itemData.RealKey, itemData.ItemSourceTypeEnum);
		bool flag3 = interactable;
		if (flag3)
		{
			int replayLevel = this.GetReplayLevel(itemData.Key);
			bool flag4 = replayLevel >= 0;
			if (flag4)
			{
				replayLevel = (int)ItemTemplateHelper.GetMerchantLevel(itemData.Key.ItemType, itemData.Key.TemplateId);
				itemView.SetShopLevelMark(EItemDebtState.Remove, replayLevel);
				return;
			}
		}
		itemView.SetShopLevelMark(EItemDebtState.None, 0);
	}

	// Token: 0x06002D4F RID: 11599 RVA: 0x00165CA8 File Offset: 0x00163EA8
	private void OnClickSelfItem(CommonTableRowForItem itemView)
	{
		bool isLocked = itemView.IsLocked;
		if (!isLocked)
		{
			ItemDisplayData itemData = itemView.Data;
			bool flag = itemData.UsingType != ItemDisplayData.ItemUsingType.Invalid;
			if (flag)
			{
				ItemDisplayData.ItemUsingOperationType itemUsingOperationType = this._openShopEventArguments.IsSettlementTreasury ? ItemDisplayData.ItemUsingOperationType.Give : ItemDisplayData.ItemUsingOperationType.Sell;
				DialogCmd cmd = new DialogCmd
				{
					Title = LocalStringManager.Get(LanguageKey.LK_Common_Attention),
					Content = itemData.GetUsingOperationConfirmTip(itemUsingOperationType),
					Type = 1,
					Yes = delegate()
					{
						this.PutSelfItemToTrade(itemView);
					}
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			else
			{
				this.PutSelfItemToTrade(itemView);
			}
		}
	}

	// Token: 0x06002D50 RID: 11600 RVA: 0x00165D8C File Offset: 0x00163F8C
	public void ExecuteRefreshActions(bool updateDebt = true, bool refreshLoad = true, bool refreshConfirmButton = true, bool refreshPreview = true, bool refreshRefreshButton = true)
	{
		if (updateDebt)
		{
			this.UpdateDebt();
		}
		if (refreshLoad)
		{
			this.RefreshLoad();
		}
		if (refreshConfirmButton)
		{
			this.RefreshConfirmButton();
		}
		bool flag = refreshPreview && (this._isNormalCharacter || this._openShopEventArguments.IsFromBuilding || this._openShopEventArguments.IsCaravan);
		if (flag)
		{
			this.RefreshPreview();
		}
		if (refreshRefreshButton)
		{
			List<ItemDisplayData> tradeItemsMerchant = this._tradeItemsMerchant;
			this.RefreshRefreshButton(((tradeItemsMerchant != null) ? tradeItemsMerchant.Count : 0) > 0);
		}
	}

	// Token: 0x06002D51 RID: 11601 RVA: 0x00165E24 File Offset: 0x00164024
	private void UpdateTradeAreaDisplay()
	{
		bool isSettlementTreasury = this._openShopEventArguments.IsSettlementTreasury;
		if (isSettlementTreasury)
		{
			for (int i = 0; i < Enum.GetValues(typeof(SettlementTreasuryLayers)).Length; i++)
			{
				CToggleObsolete toggle = this._settlementTreasuryTogGroup.Get(i);
				toggle.interactable = (toggle == this._settlementTreasuryTogGroup.GetActive() || !this.IsExistTradeItems);
			}
			this.ExecuteRefreshActions(false, true, true, true, true);
		}
		else
		{
			Refers selfMoneyRefers = base.CGet<Refers>("SelfMoney");
			Refers shopMoneyRefers = base.CGet<Refers>("ShopMoney");
			shopMoneyRefers.gameObject.SetActive(!this.IsAreaDebtShop);
			bool isAreaDebtShop = this.IsAreaDebtShop;
			string title;
			string iconName;
			string selfCurMoneyText;
			if (isAreaDebtShop)
			{
				title = LanguageKey.LK_Building_SpiritualDebt_Title.Tr();
				iconName = "sp_icon_enyixiaohao";
				selfCurMoneyText = WorldMapModel.GetFormatSpiritualDebt(SingletonObject.getInstance<WorldMapModel>().GetCurrAreaSpiritualDebt(), 0);
			}
			else
			{
				title = LanguageKey.LK_Resource_Name_Money.Tr();
				iconName = "sp_icon_resource_money";
				selfCurMoneyText = CommonUtils.GetDisplayStringForNum(this._selfMoney, 100000);
				shopMoneyRefers.CGet<CImage>("Icon").SetSprite(iconName, true, null);
			}
			selfMoneyRefers.CGet<CImage>("Icon").SetSprite(iconName, true, null);
			selfMoneyRefers.CGet<TextMeshProUGUI>("Title").text = title;
			string selfChangeMoneyText = string.Empty;
			string shopChangeMoneyText = string.Empty;
			int tradeMoney = this._tradeMoneySources.Values.Sum();
			int tradeMoneyAbs = Mathf.Abs(tradeMoney);
			bool flag = tradeMoneyAbs != 0;
			if (flag)
			{
				string merchantColor = CommonUtils.GetMoneyValueColor(tradeMoney < 0);
				string selfColor = CommonUtils.GetMoneyValueColor(tradeMoney > 0);
				string spName = (tradeMoney < 0) ? "sp_fuhao_1" : "sp_fuhao_0";
				bool isNormalCharacter = this._isNormalCharacter;
				if (isNormalCharacter)
				{
					int sellMoney = tradeMoney + this._buyMoney;
					string merchantText = string.Empty;
					bool flag2 = this._buyMoney > 0 && sellMoney > 0;
					if (flag2)
					{
						merchantText = string.Format("+ {0} (<SpName={1}>80%) - {2}", this._buyMoney, spName, sellMoney).SetColor(merchantColor);
					}
					else
					{
						bool flag3 = this._buyMoney > 0;
						if (flag3)
						{
							merchantText = string.Format("+ {0} (<SpName={1}>80%)", this._buyMoney, spName).SetColor(merchantColor);
						}
						else
						{
							bool flag4 = sellMoney > 0;
							if (flag4)
							{
								merchantText = string.Format("- {0}", sellMoney).SetColor(merchantColor);
							}
						}
					}
					shopChangeMoneyText = merchantText;
				}
				else
				{
					shopChangeMoneyText = string.Format("{0} {1}", (tradeMoney < 0) ? '+' : '-', tradeMoneyAbs).SetColor(merchantColor);
				}
				selfChangeMoneyText = string.Format("{0} {1}", (tradeMoney > 0) ? '+' : '-', this.IsAreaDebtShop ? WorldMapModel.GetFormatSpiritualDebt(tradeMoneyAbs, 0) : tradeMoneyAbs).SetColor(selfColor);
			}
			bool shopMoneyIsNotEnough = this._merchantData.Money < tradeMoneyAbs && tradeMoney > 0;
			string shopColor = shopMoneyIsNotEnough ? CommonUtils.GetMoneyValueColor(false) : "pinkyellow";
			string shopCurMoneyText = CommonUtils.GetDisplayStringForNum(this._merchantData.Money, 100000).SetColor(shopColor);
			selfMoneyRefers.CGet<TextMeshProUGUI>("Value").text = selfCurMoneyText + " " + selfChangeMoneyText;
			shopMoneyRefers.CGet<TextMeshProUGUI>("Value").text = shopCurMoneyText + " " + shopChangeMoneyText;
			shopMoneyRefers.CGet<TextMeshProUGUI>("Value").GetComponent<TMPTextSpriteHelper>().Parse();
			bool flag5 = shopMoneyIsNotEnough;
			if (flag5)
			{
				string speakStr = LocalStringManager.Get(LanguageKey.LK_Shop_Speak_Merchant_No_Money);
				this.BubbleStartCoroutine(speakStr, BubbleLevel.High, false);
			}
			this.ExecuteRefreshActions(true, true, true, true, true);
		}
	}

	// Token: 0x06002D52 RID: 11602 RVA: 0x001661CC File Offset: 0x001643CC
	private void PutSelfItemToTrade(CommonTableRowForItem itemView)
	{
		int price = (int)itemView.UserFloat;
		bool flag = itemView.Data.Amount > 1;
		if (flag)
		{
			ItemScrollViewForCommonTableRow scrollView = itemView.GetComponentInParent<ItemScrollViewForCommonTableRow>(true);
			bool flag2 = scrollView == null;
			if (!flag2)
			{
				int index = scrollView.OutputItemList.IndexOf(itemView.Data);
				bool isResource = ItemTemplateHelper.IsMiscResource(itemView.Data.Key.ItemType, itemView.Data.Key.TemplateId);
				int maxCount = itemView.Data.Amount;
				int minCount = 1;
				int initSelectCount = 0;
				bool isSettlementTreasury = this._openShopEventArguments.IsSettlementTreasury;
				if (isSettlementTreasury)
				{
					sbyte resourceType = isResource ? ItemTemplateHelper.GetMiscResourceType(itemView.Data.Key.ItemType, itemView.Data.Key.TemplateId) : -1;
					bool flag3 = isResource;
					if (flag3)
					{
						maxCount = Debts.WorthToResourceAmount((short)resourceType, (long)this._settlementTreasuryResourceMaxValue, false);
						minCount = this.CalcResourceAmount(resourceType, GlobalConfig.Instance.SettlementTreasuryGetResourceMinValue);
					}
					else
					{
						maxCount = GlobalConfig.Instance.SettlementTreasuryGiveItemMaxCount - this._tradeItemsSelf.Count;
					}
					bool flag4 = this._tradeItemsMerchant.Count > this._tradeItemsSelf.Count;
					if (flag4)
					{
						ItemDisplayData targetMerchantItem = this._tradeItemsMerchant[this._tradeItemsSelf.Count];
						int targetPrice = this.GetItemPrice(targetMerchantItem, true, 0);
						bool flag5 = isResource;
						int result;
						if (flag5)
						{
							result = this.CalcResourceAmount(resourceType, targetPrice);
						}
						else
						{
							int curPriceUnit = this.GetItemPrice(itemView.Data, false, 0);
							result = targetPrice / curPriceUnit;
							int mod = targetPrice % curPriceUnit;
							bool flag6 = mod > 0;
							if (flag6)
							{
								result++;
							}
						}
						initSelectCount = Mathf.Clamp(result, minCount, maxCount);
					}
				}
				scrollView.SetItemToSelectCountMode(index, itemView, delegate(int count)
				{
					this.ExitHighLight(scrollView);
					this.PutSelfItemToTrade(itemView.Data, count, price, true, true, ItemSourceType.Invalid);
				}, delegate
				{
					this.ExitHighLight(scrollView);
				}, initSelectCount, maxCount, minCount, LocalStringManager.Get(LanguageKey.LK_SelectCount_Limit_Shop), false, null, null, -1);
				this.HighLightItemView(scrollView, itemView, true);
			}
		}
		else
		{
			this.PutSelfItemToTrade(itemView.Data, 1, price, true, true, ItemSourceType.Invalid);
		}
	}

	// Token: 0x06002D53 RID: 11603 RVA: 0x00166494 File Offset: 0x00164694
	private void PutSelfItemToTrade(ItemDisplayData itemData, int count, int price, bool playSound = true, bool refresh = true, ItemSourceType itemSourceType = ItemSourceType.Invalid)
	{
		if (playSound)
		{
			this.PlayMoveItemSound();
		}
		List<ItemDisplayData> selfItemList = this.CurSelfItemList;
		bool isResource = ItemTemplateHelper.IsMiscResource(itemData.Key.ItemType, itemData.Key.TemplateId);
		bool flag = isResource;
		if (flag)
		{
			bool isSettlementTreasury = this._openShopEventArguments.IsSettlementTreasury;
			if (isSettlementTreasury)
			{
				ItemDisplayData tradeItem = itemData.Clone(count);
				tradeItem.ItemSourceType = this.CurSelfTradeItemSourceType.ToSbyte();
				this._tradeItemsSelf.Add(tradeItem);
				this._tradeItemSourceDict.Add(tradeItem, this.CurSelfTradeItemSourceType);
				itemData.Amount -= count;
				bool flag2 = itemData.Amount <= 0;
				if (flag2)
				{
					selfItemList.Remove(itemData);
				}
			}
		}
		int money = price * count;
		int tradeMoney;
		bool flag3 = !this._tradeMoneySources.TryGetValue(itemData.Key, out tradeMoney);
		if (flag3)
		{
			this._tradeMoneySources.Add(itemData.Key, tradeMoney = 0);
		}
		tradeMoney += money;
		this._tradeMoneySources[itemData.Key] = tradeMoney;
		if (refresh)
		{
			this.RefreshAfterPutSelfItemToTrade();
		}
	}

	// Token: 0x06002D54 RID: 11604 RVA: 0x001665BC File Offset: 0x001647BC
	private void RefreshAfterPutSelfItemToTrade()
	{
		this.SortTradeItems(this._tradeItemsSelf);
		this.UpdateTradeAreaDisplay();
		this.RefreshTreasuryTrade();
		List<ItemDisplayData> selfItemList = this.CurSelfItemList;
		this._selfItemScroll.SetItemList(ref selfItemList, false, null, null, null, true);
		this.UpdateAllSelfItemInteractable();
		bool isSettlementTreasury = this._openShopEventArguments.IsSettlementTreasury;
		if (isSettlementTreasury)
		{
			this._giveToTreasuryScrollView.SetItemList(ref this._tradeItemsSelf, false, null, false, null);
			this._getFromTreasuryScrollView.ReRender();
		}
		else
		{
			this._tradeSelfScrollView.SetItemList(ref this._tradeItemsSelf, false, null, null, null, true);
		}
		List<ItemDisplayData> shopItemList = this.CurShopItemList;
		this._shopItemScroll.SetItemList(ref shopItemList, false, null, null, null, true);
		this.UpdateAllShopItemInteractable();
		base.CGet<CButtonObsolete>("ButtonReset").interactable = true;
	}

	// Token: 0x06002D55 RID: 11605 RVA: 0x00166687 File Offset: 0x00164887
	private void PlayMoveItemSound()
	{
		AudioManager.Instance.PlaySound("ui_caravan_move", false, false);
	}

	// Token: 0x06002D56 RID: 11606 RVA: 0x0016669C File Offset: 0x0016489C
	private void RefreshLoad()
	{
		bool flag = this.CurSelfItemListIndex == UI_Shop.TogType.Inventory.ToInt() || this._openShopEventArguments.IsSettlementTreasury;
		if (flag)
		{
			this.RefreshInventoryLoad();
		}
		else
		{
			this.RefreshWarehouseLoad();
		}
	}

	// Token: 0x06002D57 RID: 11607 RVA: 0x001666E0 File Offset: 0x001648E0
	private void RefreshInventoryLoad()
	{
		string currLoad = ((float)this._curInventoryLoad / 100f).ToString("f1").SetColor(CommonUtils.GetLoadWeightValueColor(this._curInventoryLoad, this._maxInventoryLoad));
		Refers loadRoot = base.CGet<Refers>("Load");
		int deltaLoad = 0;
		foreach (ItemDisplayData tradeItem in this._tradeItemsMerchant)
		{
			bool flag = !this._openShopEventArguments.IsSettlementTreasury || (this._tradeItemSourceDict[tradeItem] == UI_Shop.TradeItemSource.SettlementTreasury && this.CurSelfTradeItemSourceType == UI_Shop.TradeItemSource.Inventory);
			if (flag)
			{
				deltaLoad += tradeItem.Weight * tradeItem.Amount;
			}
		}
		foreach (ItemDisplayData tradeItem2 in this._tradeItemsSelf)
		{
			bool flag2 = this._tradeItemSourceDict[tradeItem2] == UI_Shop.TradeItemSource.Inventory;
			if (flag2)
			{
				deltaLoad -= tradeItem2.Weight * tradeItem2.Amount;
			}
		}
		bool flag3 = deltaLoad == 0;
		string loadChangeText;
		if (flag3)
		{
			loadChangeText = string.Format("/ {0:f1}", (float)this._maxInventoryLoad / 100f);
		}
		else
		{
			int deltaLoadAbs = Mathf.Abs(deltaLoad);
			loadChangeText = string.Format("{0} {1:f1} / {2:f1}", (deltaLoad > 0) ? '+' : '-', (float)deltaLoadAbs / 100f, (float)this._maxInventoryLoad / 100f);
		}
		loadRoot.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.Get(LanguageKey.LK_Inventory_Load_Title);
		loadRoot.CGet<TextMeshProUGUI>("Value").text = currLoad + " " + loadChangeText;
	}

	// Token: 0x06002D58 RID: 11608 RVA: 0x001668CC File Offset: 0x00164ACC
	private void RefreshWarehouseLoad()
	{
		string currLoad = ((float)this._warehouseCurLoad / 100f).ToString("f1").SetColor(CommonUtils.GetLoadWeightValueColor(this._warehouseCurLoad, this._warehouseMaxLoad));
		Refers loadRoot = base.CGet<Refers>("Load");
		int deltaLoad = 0;
		foreach (ItemDisplayData tradeItem in this._tradeItemsMerchant)
		{
			deltaLoad += tradeItem.Weight * tradeItem.Amount;
		}
		foreach (ItemDisplayData tradeItem2 in this._tradeItemsSelf)
		{
			UI_Shop.TradeItemSource tradeItemSource = this._tradeItemSourceDict[tradeItem2];
			bool flag = tradeItemSource == UI_Shop.TradeItemSource.Warehouse || tradeItemSource == UI_Shop.TradeItemSource.Treasury;
			if (flag)
			{
				deltaLoad -= tradeItem2.Weight * tradeItem2.Amount;
			}
		}
		bool flag2 = deltaLoad == 0;
		string loadChangeText;
		if (flag2)
		{
			loadChangeText = string.Format("/ {0:f1}", (float)this._warehouseMaxLoad / 100f);
		}
		else
		{
			int deltaLoadAbs = Mathf.Abs(deltaLoad);
			loadChangeText = string.Format("{0} {1:f1} / {2:f1}", (deltaLoad > 0) ? '+' : '-', (float)deltaLoadAbs / 100f, (float)this._warehouseMaxLoad / 100f);
		}
		loadRoot.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.Get(LanguageKey.LK_Warehouse_Load_Title);
		loadRoot.CGet<TextMeshProUGUI>("Value").text = currLoad + loadChangeText;
	}

	// Token: 0x06002D59 RID: 11609 RVA: 0x00166A94 File Offset: 0x00164C94
	private int GetItemPrice(ItemDisplayData itemData, bool isBuy, int percentValue = -2147483648)
	{
		int basePrice = this.GetItemBasePrice(itemData, isBuy);
		bool flag = percentValue == int.MinValue;
		if (flag)
		{
			percentValue = this.GetPriceChangePercentValue(itemData, isBuy);
		}
		return basePrice + basePrice * percentValue / 100;
	}

	// Token: 0x06002D5A RID: 11610 RVA: 0x00166AD0 File Offset: 0x00164CD0
	private int GetItemBasePrice(ItemDisplayData itemData, bool isBuy)
	{
		bool isAreaDebtShop = this.IsAreaDebtShop;
		int result;
		if (isAreaDebtShop)
		{
			int price = (ItemTemplateHelper.GetGrade(itemData.Key.ItemType, itemData.Key.TemplateId) == 6) ? 1000 : 400;
			result = (this._openShopEventArguments.IgnoreFavorability ? (price / 2) : price);
		}
		else
		{
			bool isSettlementTreasury = this._openShopEventArguments.IsSettlementTreasury;
			if (isSettlementTreasury)
			{
				result = this.GetTreasuryItemPrice(itemData);
			}
			else
			{
				int baseEffect = isBuy ? 200 : 20;
				int worldRate = this.GetWorldDetailPriceRate(isBuy);
				int percentValue = baseEffect * worldRate / 100;
				percentValue = Math.Max(0, percentValue);
				result = (int)itemData.Value * percentValue / 100;
			}
		}
		return result;
	}

	// Token: 0x06002D5B RID: 11611 RVA: 0x00166B84 File Offset: 0x00164D84
	private int GetTreasuryItemPrice(ItemDisplayData itemData)
	{
		bool flag = itemData == null;
		int result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			bool isResource = ItemTemplateHelper.IsMiscResource(itemData.Key.ItemType, itemData.Key.TemplateId);
			bool flag2 = isResource;
			if (flag2)
			{
				sbyte resourceType = ItemTemplateHelper.GetMiscResourceType(itemData.Key.ItemType, itemData.Key.TemplateId);
				int resourceValue = this.CalcResourceValue(resourceType, itemData.Amount);
				result = resourceValue;
			}
			else
			{
				short itemSubType = ItemTemplateHelper.GetItemSubType(itemData.Key.ItemType, itemData.Key.TemplateId);
				int adjustedWorth = this._settlementTreasury.CalcAdjustedWorth(itemSubType, (int)itemData.Value) * GlobalConfig.Instance.ItemContributionPercent / 100;
				result = adjustedWorth;
			}
		}
		return result;
	}

	// Token: 0x06002D5C RID: 11612 RVA: 0x00166C40 File Offset: 0x00164E40
	private int GetPriceChangePercentValue(ItemDisplayData itemData, bool isBuy)
	{
		return this.GetPricePercentValue(itemData, isBuy) - 100;
	}

	// Token: 0x06002D5D RID: 11613 RVA: 0x00166C60 File Offset: 0x00164E60
	private int GetPricePercentValue(ItemDisplayData itemData, bool isBuy)
	{
		bool flag = this._openShopEventArguments.IsSettlementTreasury || this.IsAreaDebtShop;
		int result2;
		if (flag)
		{
			result2 = 100;
		}
		else
		{
			int createEffect = 0;
			int extraEffect = 0;
			if (isBuy)
			{
				MerchantExtraGoodsData.ExtraGoodsType extraGoodsType;
				bool isExtra = this.CheckIsExtra(itemData.Key.Id, out extraGoodsType);
				bool flag2 = isExtra && extraGoodsType == MerchantExtraGoodsData.ExtraGoodsType.Normal;
				if (flag2)
				{
					extraEffect = 50;
				}
				else
				{
					MerchantData merchantData = this._merchantData;
					createEffect = ((merchantData != null) ? merchantData.GetPriceChangePercent(itemData.Key) : 0);
				}
			}
			int favorabilityEffect = this.GetCharFavorabilityEffect(isBuy);
			int result = (100 + createEffect + favorabilityEffect + extraEffect) * this.GetProfessionPriceRate(isBuy) / 100;
			result2 = result;
		}
		return result2;
	}

	// Token: 0x06002D5E RID: 11614 RVA: 0x00166D08 File Offset: 0x00164F08
	private int GetProfessionPriceRate(bool isBuy)
	{
		bool flag = this._openShopEventArguments.MerchantSourceTypeEnum == OpenShopEventArguments.EMerchantSourceType.ProfessionSkillCaravan;
		int result;
		if (flag)
		{
			ProfessionData professionData = SingletonObject.getInstance<ProfessionModel>().GetProfessionData(15);
			ValueTuple<int, int> valueTuple = professionData.SeniorityToCaravanPrice();
			int sell = valueTuple.Item1;
			int buy = valueTuple.Item2;
			int delta = isBuy ? buy : sell;
			result = delta + 100;
		}
		else
		{
			result = 100;
		}
		return result;
	}

	// Token: 0x06002D5F RID: 11615 RVA: 0x00166D64 File Offset: 0x00164F64
	private int GetWorldDetailPriceRate(bool isBuy)
	{
		return (int)(isBuy ? 100 : GameData.Domains.World.SharedMethods.GetGainResourcePercent(11));
	}

	// Token: 0x06002D60 RID: 11616 RVA: 0x00166D84 File Offset: 0x00164F84
	private int GetCharFavorabilityEffect(bool isBuy)
	{
		bool flag = !this._isNormalCharacter || this._merchantCharData == null;
		int result;
		if (flag)
		{
			result = 0;
		}
		else
		{
			sbyte favorabilityType = FavorabilityType.GetFavorabilityType(this._merchantCharData.FavorabilityToTaiwu);
			sbyte favorabilityIndex = FavorabilityType.ToIndex(favorabilityType);
			int favorabilityEffect = isBuy ? GlobalConfig.Instance.MerchantCharFavorabilityBuyEffect[(int)favorabilityIndex] : GlobalConfig.Instance.MerchantCharFavorabilitySellEffect[(int)favorabilityIndex];
			result = favorabilityEffect;
		}
		return result;
	}

	// Token: 0x06002D61 RID: 11617 RVA: 0x00166DEC File Offset: 0x00164FEC
	public void OnClickConfirm()
	{
		bool isSettlementTreasury = this._openShopEventArguments.IsSettlementTreasury;
		if (isSettlementTreasury)
		{
			this.ConfirmSettlementTreasuryOperation();
		}
		else
		{
			ArgumentBox args = EasyPool.Get<ArgumentBox>();
			args.Set("tradeMoney", this._tradeMoneySources.Values.Sum());
			args.Set("merchantMoney", this._merchantData.Money);
			args.Set("selfMoney", this.IsAreaDebtShop ? SingletonObject.getInstance<WorldMapModel>().GetCurrAreaSpiritualDebt() : this._selfMoney);
			args.Set("merchantType", this._merchantData.MerchantType);
			args.Set("isAreaDebtShop", this.IsAreaDebtShop);
			Action onConfirm = new Action(this.Deal);
			args.SetObject("onConfirm", onConfirm);
			UIElement.ShopConfirm.SetOnInitArgs(args);
			UIManager.Instance.ShowUI(UIElement.ShopConfirm, true);
		}
	}

	// Token: 0x06002D62 RID: 11618 RVA: 0x00166ED8 File Offset: 0x001650D8
	private void ConfirmSettlementTreasuryOperation()
	{
		this._confirmedTreasuryOperation = true;
		foreach (ItemDisplayData itemData in this._tradeItemsSelf.Concat(this._tradeItemsMerchant))
		{
			UI_Shop.TradeItemSource sourceType = this._tradeItemSourceDict[itemData];
			bool fromShop = sourceType < UI_Shop.TradeItemSource.Inventory || sourceType == UI_Shop.TradeItemSource.SettlementTreasury;
			bool isResource = itemData.IsResource;
			if (isResource)
			{
				bool flag = fromShop;
				if (flag)
				{
					ItemSourceChange change = this._itemChangeDict[this.GetItemSourceType(UI_Shop.TradeItemSource.Inventory)];
					change.AddItem(itemData.Key, itemData.Amount, 0);
				}
				else
				{
					ItemSourceChange change2 = this._itemChangeDict[this.GetItemSourceType(sourceType)];
					change2.RemoveItem(itemData.Key, itemData.Amount, 0);
				}
			}
		}
		this.ClearSlotItemChange();
		this.QuickHide();
	}

	// Token: 0x06002D63 RID: 11619 RVA: 0x00166FD4 File Offset: 0x001651D4
	private void RefreshMoneyPanel(ArgumentBox _)
	{
		this.RefreshMoneyPanel();
	}

	// Token: 0x06002D64 RID: 11620 RVA: 0x00166FDD File Offset: 0x001651DD
	private void RefreshMoneyPanel()
	{
		this.UpdateTradeAreaDisplay();
	}

	// Token: 0x06002D65 RID: 11621 RVA: 0x00166FE8 File Offset: 0x001651E8
	private void Deal()
	{
		AudioManager.Instance.PlaySound("ui_caravan_coin", false, false);
		this.SetConfirmButton(false, "");
		base.CGet<CButtonObsolete>("ButtonReset").interactable = false;
		List<ItemDisplayData> selfItemList = this.CurSelfItemList;
		using (IEnumerator<ItemDisplayData> enumerator = this._tradeItemsSelf.Concat(this._tradeItemsMerchant).GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				ItemDisplayData itemData = enumerator.Current;
				UI_Shop.TradeItemSource sourceType = this._tradeItemSourceDict[itemData];
				List<ItemDisplayData> itemList = (sourceType < UI_Shop.TradeItemSource.Inventory) ? selfItemList : this._shopItemList[UI_Shop.TradeItemSource.ShopBuyBackList.ToInt()];
				ItemDisplayData itemInList = itemList.Find((ItemDisplayData data) => data.CanMerge(itemData));
			}
		}
		this._tradeItemsMerchant.Clear();
		this._tradeItemsSelf.Clear();
		this._tradeItemSourceDict.Clear();
		List<ItemDisplayData> shopList = this.CurShopItemList;
		this._shopItemScroll.SetItemList(ref shopList, false, null, null, null, true);
		this.UpdateAllShopItemInteractable();
		this._tradeMerchantScrollView.SetItemList(ref this._tradeItemsMerchant, false, null, null, null, true);
		this._tradeSelfScrollView.SetItemList(ref this._tradeItemsSelf, false, null, null, null, true);
		int tradeMoney = this._tradeMoneySources.Values.Sum();
		tradeMoney = Mathf.Min(tradeMoney, this._merchantData.Money);
		this._selfMoney += tradeMoney;
		bool anySeal = this._merchantBuyBackData.BuyInGoodsList.Items.Any(new Func<KeyValuePair<ItemKey, int>, bool>(UI_Shop.IsSealOfMerchant));
		bool flag = anySeal;
		if (flag)
		{
			List<ItemKey> cache = EasyPool.Get<List<ItemKey>>();
			this._merchantBuyBackData.BuyInGoodsList.Items.RemoveAllKeys(new Func<ItemKey, bool>(UI_Shop.IsSealOfMerchant), cache);
			this._merchantBuyBackData.BuyInPrice.RemoveAllKeys(new Func<ItemKey, bool>(UI_Shop.IsSealOfMerchant), cache);
			EasyPool.Free<List<ItemKey>>(cache);
			MerchantOverFavorLevelData[] favor = this._tempMerchantOverFavorData.MerchantOverFavorLevelDataArray;
			for (int i = 0; i < favor.Length; i++)
			{
				favor[i].BuyCount = this.GetMaxBuyCount(i);
			}
		}
		MerchantTradeArguments merchantTradeArguments2 = new MerchantTradeArguments();
		merchantTradeArguments2.TradeMoneySources = this._tradeMoneySources.ToDictionary((KeyValuePair<ItemKey, int> x) => x.Key, (KeyValuePair<ItemKey, int> x) => (long)x.Value);
		merchantTradeArguments2.BuyMoney = (long)this._buyMoney;
		merchantTradeArguments2.ItemChangeList = this._itemChangeDict.Values.ToList<ItemSourceChange>();
		merchantTradeArguments2.MerchantBuyBackData = this._merchantBuyBackData;
		merchantTradeArguments2.MerchantData = this._merchantData;
		merchantTradeArguments2.OpenShopEventArguments = this._openShopEventArguments;
		merchantTradeArguments2.OverFavorData = this._tempMerchantOverFavorData;
		MerchantTradeArguments merchantTradeArguments = merchantTradeArguments2;
		MerchantDomainMethod.Call.SettleTrade(merchantTradeArguments);
		this._originMerchantOverFavorData = this._tempMerchantOverFavorData;
		bool flag2 = tradeMoney > 0;
		if (flag2)
		{
			this.RefreshSpeakOnDeal(false);
		}
		else
		{
			bool flag3 = tradeMoney < 0;
			if (flag3)
			{
				this.RefreshSpeakOnDeal(true);
			}
		}
		this._tradeMoneySources.Clear();
		this._buyMoney = 0;
		this.UpdateTradeAreaDisplay();
		this.ClearSlotItemChange();
		this.GetMerchantData();
		this.SendRefreshSelfItem();
	}

	// Token: 0x06002D66 RID: 11622 RVA: 0x00167344 File Offset: 0x00165544
	private void OnClickReset()
	{
		bool isSettlementTreasury = this._openShopEventArguments.IsSettlementTreasury;
		if (isSettlementTreasury)
		{
			string title = LocalStringManager.Get(LanguageKey.LK_Building_Treasury_ClearTip_Title);
			string content = LocalStringManager.Get(LanguageKey.LK_Building_Treasury_ClearTip_Content);
			CommonUtils.ShowConfirmDialog(title, content, new Action(this.Reset), null, EDialogType.None);
		}
		else
		{
			this.Reset();
		}
	}

	// Token: 0x06002D67 RID: 11623 RVA: 0x0016739C File Offset: 0x0016559C
	private void Reset()
	{
		this.PlayMoveItemSound();
		using (IEnumerator<ItemDisplayData> enumerator = this._tradeItemsMerchant.Concat(this._tradeItemsSelf).GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				ItemDisplayData itemData = enumerator.Current;
				UI_Shop.TradeItemSource sourceType = this._tradeItemSourceDict[itemData];
				bool fromShop = sourceType < UI_Shop.TradeItemSource.Inventory;
				List<ItemDisplayData> itemList = this.GetTradeItemList(sourceType);
				ItemDisplayData itemInList = itemList.Find((ItemDisplayData data) => data.CanMerge(itemData));
				bool isResource = itemData.IsResource;
				if (isResource)
				{
					bool flag = itemInList == null;
					if (flag)
					{
						itemInList = itemData.Clone(itemData.Amount);
						itemInList.ItemSourceType = sourceType.ToSbyte();
						itemList.Add(itemInList);
					}
					else
					{
						itemInList.Amount += itemData.Amount;
					}
				}
			}
		}
		this._tradeItemsMerchant.Clear();
		this._tradeItemsSelf.Clear();
		this._tradeItemSourceDict.Clear();
		List<ItemDisplayData> shopList = this.CurShopItemList;
		this._shopItemScroll.SetItemList(ref shopList, false, null, null, null, true);
		this.UpdateAllShopItemInteractable();
		List<ItemDisplayData> selfList = this.CurSelfItemList;
		this._selfItemScroll.SetItemList(ref selfList, false, null, null, null, true);
		this.UpdateAllSelfItemInteractable();
		bool isSettlementTreasury = this._openShopEventArguments.IsSettlementTreasury;
		if (isSettlementTreasury)
		{
			this._getFromTreasuryScrollView.SetItemList(ref this._tradeItemsMerchant, false, null, false, null);
			this._giveToTreasuryScrollView.SetItemList(ref this._tradeItemsSelf, false, null, false, null);
		}
		else
		{
			this._tradeMerchantScrollView.SetItemList(ref this._tradeItemsMerchant, false, null, null, null, true);
			this._tradeSelfScrollView.SetItemList(ref this._tradeItemsSelf, false, null, null, null, true);
		}
		this._tradeMoneySources.Clear();
		this._buyMoney = 0;
		this.SetConfirmButton(false, "");
		base.CGet<CButtonObsolete>("ButtonReset").interactable = false;
		this.UpdateTradeAreaDisplay();
		this.RefreshTreasuryTrade();
		this.ClearSlotItemChange();
	}

	// Token: 0x06002D68 RID: 11624 RVA: 0x001675D8 File Offset: 0x001657D8
	private void HighLightItemView(Refers scrollView, Refers view, bool showMask)
	{
		bool flag = null == view;
		if (!flag)
		{
			scrollView.GetComponent<CScrollRectLegacy>().SetScrollEnable(false);
			ItemView itemView = this._focusingTuple.Item1 as ItemView;
			bool flag2 = itemView != null;
			if (flag2)
			{
				itemView.SetHighLight(true);
			}
			else
			{
				CommonTableRowForItem tableRowForItem = this._focusingTuple.Item1 as CommonTableRowForItem;
				bool flag3 = tableRowForItem != null;
				if (flag3)
				{
					tableRowForItem.SetSelectState(true);
				}
			}
			this._focusingTuple.Item1 = view;
			this._focusingTuple.Item2 = view.transform.parent;
			this._focusingTuple.Item3 = view.transform.GetSiblingIndex();
			RectTransform focusMask = scrollView.CGet<RectTransform>("FocusItemMask");
			focusMask.anchoredPosition = Vector2.zero;
			focusMask.sizeDelta = Vector2.zero;
			view.transform.SetParent(focusMask, true);
			focusMask.gameObject.SetActive(showMask);
		}
	}

	// Token: 0x06002D69 RID: 11625 RVA: 0x001676C4 File Offset: 0x001658C4
	public void ExitHighLight(Refers scrollView)
	{
		scrollView.GetComponent<CScrollRectLegacy>().SetScrollEnable(true);
		bool flag = null != this._focusingTuple.Item1;
		if (flag)
		{
			ItemView itemView = this._focusingTuple.Item1 as ItemView;
			bool flag2 = itemView != null;
			if (flag2)
			{
				itemView.SetHighLight(false);
			}
			else
			{
				CommonTableRowForItem tableRowForItem = this._focusingTuple.Item1 as CommonTableRowForItem;
				bool flag3 = tableRowForItem != null;
				if (flag3)
				{
					tableRowForItem.SetSelectState(false);
				}
			}
			this._focusingTuple.Item1.transform.SetParent(this._focusingTuple.Item2, true);
			this._focusingTuple.Item1.transform.SetSiblingIndex(this._focusingTuple.Item3);
			this._focusingTuple.Item1 = null;
			scrollView.CGet<RectTransform>("FocusItemMask").gameObject.SetActive(false);
		}
		ConchShipCursor.Instance.SetDefaultCursor();
	}

	// Token: 0x06002D6A RID: 11626 RVA: 0x001677B0 File Offset: 0x001659B0
	private List<ItemDisplayData> GetTradeItemList(UI_Shop.TradeItemSource tradeItemSource)
	{
		List<ItemDisplayData> result;
		switch (tradeItemSource)
		{
		case UI_Shop.TradeItemSource.Inventory:
			result = this._inventoryItems;
			break;
		case UI_Shop.TradeItemSource.Warehouse:
			result = this._warehouseItems;
			break;
		case UI_Shop.TradeItemSource.Treasury:
			result = this._treasuryItems;
			break;
		case UI_Shop.TradeItemSource.SettlementTreasury:
			result = this._settlementTreasuryItems;
			break;
		default:
			result = this._shopItemList[tradeItemSource - UI_Shop.TradeItemSource.ShopList0];
			break;
		}
		return result;
	}

	// Token: 0x06002D6B RID: 11627 RVA: 0x00167810 File Offset: 0x00165A10
	private void ClearSlotItemChange()
	{
		foreach (ItemSourceChange itemSourceChange in this._itemChangeDict.Values)
		{
			itemSourceChange.Items.Clear();
		}
	}

	// Token: 0x06002D6C RID: 11628 RVA: 0x00167874 File Offset: 0x00165A74
	private ItemSourceType GetItemSourceType(UI_Shop.TradeItemSource tradeItemSource)
	{
		ItemSourceType result;
		switch (tradeItemSource)
		{
		case UI_Shop.TradeItemSource.Inventory:
			result = ItemSourceType.Inventory;
			break;
		case UI_Shop.TradeItemSource.Warehouse:
			result = ItemSourceType.Warehouse;
			break;
		case UI_Shop.TradeItemSource.Treasury:
			result = ItemSourceType.Treasury;
			break;
		case UI_Shop.TradeItemSource.SettlementTreasury:
			result = ItemSourceType.SettlementTreasury;
			break;
		default:
			result = ItemSourceType.Inventory;
			break;
		}
		return result;
	}

	// Token: 0x06002D6D RID: 11629 RVA: 0x001678B8 File Offset: 0x00165AB8
	private void SetConfirmButton(bool interactable, string tipContent = "")
	{
		CButtonObsolete confirmButton = base.CGet<CButtonObsolete>("ButtonConfirm");
		confirmButton.interactable = interactable;
		TooltipInvoker tip = confirmButton.GetComponent<TooltipInvoker>();
		tip.enabled = !tipContent.IsNullOrEmpty();
		bool enabled = tip.enabled;
		if (enabled)
		{
			string[] presetParam = tip.PresetParam;
			bool flag = presetParam == null || presetParam.Length != 1;
			if (flag)
			{
				tip.PresetParam = new string[1];
			}
			tip.PresetParam[0] = tipContent;
		}
	}

	// Token: 0x06002D6E RID: 11630 RVA: 0x00167930 File Offset: 0x00165B30
	private void RefreshConfirmButton()
	{
		bool flag = this._tradeItemsMerchant.Count > 0 || this._tradeItemsSelf.Count > 0;
		if (flag)
		{
			bool isSettlementTreasury = this._openShopEventArguments.IsSettlementTreasury;
			if (isSettlementTreasury)
			{
				this.SetConfirmButton(true, "");
			}
			else
			{
				int tradeMoney = this._tradeMoneySources.Values.Sum();
				bool debtNotMeet = this._tempMerchantOverFavorData.MerchantOverFavorLevelDataArray.ToList<MerchantOverFavorLevelData>().Exists((MerchantOverFavorLevelData d) => d.BuyCount < 0);
				bool flag2 = debtNotMeet;
				if (flag2)
				{
					string content = LocalStringManager.Get(LanguageKey.LK_Shop_DoDeal_OverDebtLimitCount).ColorReplace();
					this.SetConfirmButton(false, content);
				}
				else
				{
					bool flag3 = this._merchantData.Money >= tradeMoney && (this.IsAreaDebtShop ? SingletonObject.getInstance<WorldMapModel>().GetCurrAreaSpiritualDebt() : this._selfMoney) < -tradeMoney;
					if (flag3)
					{
						string moneyName = this.IsAreaDebtShop ? LocalStringManager.Get(LanguageKey.LK_Area_Debt_Tip_Title) : LocalStringManager.Get(LanguageKey.LK_Resource_Name_Money);
						string content2 = LocalStringManager.GetFormat(LanguageKey.LK_Shop_DoDeal_Money_Not_Enough, moneyName).SetColor("brightred");
						this.SetConfirmButton(false, content2);
					}
					else
					{
						this.SetConfirmButton(true, "");
					}
				}
			}
		}
		else
		{
			this.SetConfirmButton(false, "");
		}
	}

	// Token: 0x06002D6F RID: 11631 RVA: 0x00167A92 File Offset: 0x00165C92
	public void RefreshPreview()
	{
		MerchantDomainMethod.Call.GetFavorabilityWithDelta(this.Element.GameDataListenerId, this._merchantData.MerchantType, GameData.Domains.Merchant.SharedMethods.RealFavorabilityGain(this._buyMoney));
	}

	// Token: 0x06002D70 RID: 11632 RVA: 0x00167ABC File Offset: 0x00165CBC
	private void OnClickButtonBuyAll()
	{
		bool flag = this.IsPageDisabled(this.CurShopItemListIndex);
		if (!flag)
		{
			List<ItemDisplayData> list = (from d in this._shopItemScroll.OutputItemList
			where d.Interactable
			select d).ToList<ItemDisplayData>();
			bool flag2 = list.Count == 0;
			if (!flag2)
			{
				int buyCount = (int)this.GetMaxBuyCount(list.First<ItemDisplayData>().ItemShopLevel);
				bool flag3 = buyCount == 0;
				if (!flag3)
				{
					foreach (ItemDisplayData data in list)
					{
						int price = this.GetItemPrice(data, true, int.MinValue);
						int amount = Math.Min(data.Amount, buyCount);
						this.PutShopItemToTrade(data, amount, price, false, false);
						buyCount -= amount;
						bool flag4 = buyCount == 0;
						if (flag4)
						{
							break;
						}
					}
					this.PlayMoveItemSound();
					this.RefreshAfterPutShopItemToTrade();
				}
			}
		}
	}

	// Token: 0x06002D71 RID: 11633 RVA: 0x00167BD4 File Offset: 0x00165DD4
	private void OnClickButtonSellAll()
	{
		List<ItemDisplayData> list = (from d in this._selfItemScroll.OutputItemList
		where d.Interactable
		select d).ToList<ItemDisplayData>();
		bool flag = list.Count == 0;
		if (!flag)
		{
			foreach (ItemDisplayData data in list)
			{
				int price = this.GetItemPrice(data, true, int.MinValue);
				this.PutSelfItemToTrade(data, data.Amount, price, false, false, ItemSourceType.Invalid);
			}
			this.PlayMoveItemSound();
			this.RefreshAfterPutSelfItemToTrade();
		}
	}

	// Token: 0x06002D72 RID: 11634 RVA: 0x00167C94 File Offset: 0x00165E94
	private void UpdateDebt()
	{
		bool flag = this._originMerchantOverFavorData == null;
		if (!flag)
		{
			this._tempMerchantOverFavorData = (this._originMerchantOverFavorData.Clone() as MerchantOverFavorData);
			this.CalcDebt(this._tradeItemsMerchant);
			this.CalcDebt(this._tradeItemsSelf);
			this._tradeMerchantScrollView.ReRender();
			this._tradeSelfScrollView.ReRender();
			this._selfItemScroll.ReRender();
			this.InitGoodsTog(this.CurShopItemListIndex);
		}
	}

	// Token: 0x06002D73 RID: 11635 RVA: 0x00167D14 File Offset: 0x00165F14
	private void CalcDebt(List<ItemDisplayData> items)
	{
		foreach (ItemDisplayData data in items)
		{
			UI_Shop.TradeItemSource itemSource = this._tradeItemSourceDict[data];
			bool showSpeak = this._currentOperationData == data;
			this.CalcDebt(data, itemSource, showSpeak, false);
		}
	}

	// Token: 0x06002D74 RID: 11636 RVA: 0x00167D84 File Offset: 0x00165F84
	private void CalcDebt(ItemDisplayData data, UI_Shop.TradeItemSource itemSource, bool showSpeak, bool isSimulate)
	{
	}

	// Token: 0x06002D75 RID: 11637 RVA: 0x00167D88 File Offset: 0x00165F88
	private EItemDebtState ChangeDebt(ItemKey itemKey, UI_Shop.TradeItemSource sourceType, bool showSpeak, bool isSimulate, out int level)
	{
		level = 0;
		long worth = Debts.ItemToWorth(itemKey.ItemType, itemKey.TemplateId);
		bool flag = worth <= 0L;
		EItemDebtState result;
		if (flag)
		{
			result = EItemDebtState.None;
		}
		else
		{
			bool flag2 = sourceType == UI_Shop.TradeItemSource.ShopBuyBackList;
			if (flag2)
			{
				for (int i = this._tempMerchantOverFavorData.MerchantOverFavorLevelDataArray.Length - 1; i >= 0; i--)
				{
					MerchantOverFavorLevelData levelData = this._tempMerchantOverFavorData.MerchantOverFavorLevelDataArray[i];
					bool flag3 = levelData.Inventory.Items.ContainsKey(itemKey);
					if (flag3)
					{
						level = i;
						bool flag4 = !isSimulate;
						if (flag4)
						{
							levelData.Inventory.OfflineRemove(itemKey, 1);
							MerchantOverFavorLevelData merchantOverFavorLevelData = levelData;
							merchantOverFavorLevelData.BuyCount -= 1;
						}
						if (showSpeak)
						{
							this.RefreshSpeakOnItemOverProgress(true);
						}
						return EItemDebtState.Add;
					}
				}
				result = EItemDebtState.None;
			}
			else
			{
				int worldProgressLimitedLevel;
				int worldProgressLimitedFavor;
				UI_Shop.IsReachProgressLimit(UI_Shop._merchantFavorability, out worldProgressLimitedLevel, out worldProgressLimitedFavor);
				int merchantFavorability = Mathf.Min(worldProgressLimitedFavor, UI_Shop._merchantFavorability);
				bool isBuy = sourceType <= UI_Shop.TradeItemSource.ShopBuyBackList;
				bool flag5 = isBuy;
				if (flag5)
				{
					int sourceIndex = sourceType.ToInt();
					short maxBuyCount = this.GetMaxBuyCount(sourceIndex);
					bool flag6 = sourceIndex > GameData.Domains.Merchant.SharedMethods.GetFavorLevel(merchantFavorability) && maxBuyCount < short.MaxValue;
					if (flag6)
					{
						level = sourceIndex;
						bool flag7 = !isSimulate;
						if (flag7)
						{
							MerchantOverFavorLevelData merchantOverFavorLevelData2 = this._tempMerchantOverFavorData.MerchantOverFavorLevelDataArray[sourceIndex];
							merchantOverFavorLevelData2.BuyCount -= 1;
						}
						if (showSpeak)
						{
							this.RefreshSpeakOnItemOverProgress(true);
						}
						return EItemDebtState.Add;
					}
				}
				else
				{
					int repayLevel = this.GetReplayLevel(itemKey);
					bool flag8 = repayLevel >= 0;
					if (flag8)
					{
						bool flag9 = !isSimulate;
						if (flag9)
						{
							MerchantOverFavorLevelData merchantOverFavorLevelData3 = this._tempMerchantOverFavorData.MerchantOverFavorLevelDataArray[repayLevel];
							merchantOverFavorLevelData3.BuyCount += 1;
							this._tempMerchantOverFavorData.MerchantOverFavorLevelDataArray[repayLevel].Inventory.OfflineAdd(itemKey, 1);
						}
						if (showSpeak)
						{
							this.RefreshSpeakOnItemOverProgress(false);
						}
						level = repayLevel;
						return EItemDebtState.Remove;
					}
				}
				result = EItemDebtState.None;
			}
		}
		return result;
	}

	// Token: 0x06002D76 RID: 11638 RVA: 0x00167F98 File Offset: 0x00166198
	private int GetReplayLevel(ItemKey itemKey)
	{
		int repayLevel = -1;
		bool flag = this._tempMerchantOverFavorData == null;
		int result;
		if (flag)
		{
			result = repayLevel;
		}
		else
		{
			long worth = Debts.ItemToWorth(itemKey.ItemType, itemKey.TemplateId);
			bool flag2 = worth <= 0L;
			if (flag2)
			{
				result = repayLevel;
			}
			else
			{
				sbyte itemMerchantLevel = ItemTemplateHelper.GetMerchantLevel(itemKey.ItemType, itemKey.TemplateId);
				for (int i = this._tempMerchantOverFavorData.MerchantOverFavorLevelDataArray.Length - 1; i >= 0; i--)
				{
					short buyCount = this._tempMerchantOverFavorData.MerchantOverFavorLevelDataArray[i].BuyCount;
					short maxBuyCount = this.GetMaxBuyCount(i);
					bool flag3 = this.IsPageShow(i) && buyCount < maxBuyCount && maxBuyCount < short.MaxValue && (int)itemMerchantLevel >= i;
					if (flag3)
					{
						repayLevel = i;
						break;
					}
				}
				result = repayLevel;
			}
		}
		return result;
	}

	// Token: 0x06002D77 RID: 11639 RVA: 0x00168078 File Offset: 0x00166278
	private short GetMaxBuyCount(int merchantLevel)
	{
		short maxBuyCount = GlobalConfig.Instance.MerchantOverFavorBuyCount[merchantLevel];
		bool flag = maxBuyCount < 0;
		if (flag)
		{
			maxBuyCount = short.MaxValue;
		}
		return maxBuyCount;
	}

	// Token: 0x06002D78 RID: 11640 RVA: 0x001680A6 File Offset: 0x001662A6
	private void RefreshSpeakOnInit()
	{
		this.BubbleStartCoroutine(this._merchantTypeConfig.Prologue, BubbleLevel.Low, false);
	}

	// Token: 0x06002D79 RID: 11641 RVA: 0x001680C0 File Offset: 0x001662C0
	private void RefreshSpeakOnDeal(bool isBuy)
	{
		string speakStr = LocalStringManager.Get(isBuy ? LanguageKey.LK_Shop_Speak_Buy : LanguageKey.LK_Shop_Speak_Sell);
		this.BubbleStartCoroutine(speakStr, BubbleLevel.High, false);
		this.ResetDialog();
	}

	// Token: 0x06002D7A RID: 11642 RVA: 0x001680F4 File Offset: 0x001662F4
	private void ResetDialog()
	{
		this._timer = 0f;
		this._onlyIntroduceDialog = false;
		this._firstIntroduceDialog = false;
		this._secondIntroduceDialog = false;
		this._remainDialog = false;
		this._favorDialog = false;
		this._seasonDialog = false;
		this._currBubbleLevel = BubbleLevel.Low;
	}

	// Token: 0x06002D7B RID: 11643 RVA: 0x00168134 File Offset: 0x00166334
	private void RefreshSpeakOnItemOverProgress(bool isBuy)
	{
		string speakStr = LocalStringManager.Get(isBuy ? LanguageKey.LK_Shop_Speak_Buy_OverProgress : LanguageKey.LK_Shop_Speak_Sell_OverProgress);
		this.BubbleStartCoroutine(speakStr, BubbleLevel.High, false);
	}

	// Token: 0x06002D7C RID: 11644 RVA: 0x00168164 File Offset: 0x00166364
	private void BubbleStartCoroutine(string content, BubbleLevel bubbleLevel, bool isSeason = false)
	{
		bool flag = bubbleLevel.ToInt() < this._currBubbleLevel.ToInt();
		if (!flag)
		{
			this._currBubbleLevel = bubbleLevel;
			this._leftCloudBubble.SetText(content, true);
			bool flag2 = this.coroutine != null;
			if (flag2)
			{
				base.StopCoroutine(this.coroutine);
			}
			this.coroutine = base.StartCoroutine(this.Wait(this._leftCloudBubble, isSeason));
		}
	}

	// Token: 0x06002D7D RID: 11645 RVA: 0x001681DF File Offset: 0x001663DF
	private IEnumerator Wait(Bubble bubble, bool isSeason = false)
	{
		float time = isSeason ? 12f : 8f;
		yield return new WaitForSeconds(time);
		bubble.Hide();
		this._currBubbleLevel = BubbleLevel.Low;
		yield break;
	}

	// Token: 0x06002D7E RID: 11646 RVA: 0x001681FC File Offset: 0x001663FC
	private void Update()
	{
		bool flag = !this.Element.Ready;
		if (!flag)
		{
			this.UpdateDialog();
			bool flag2 = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) && base.CGet<CButtonObsolete>("ButtonConfirm").interactable;
			if (flag2)
			{
				this.OnClickConfirm();
			}
		}
	}

	// Token: 0x06002D7F RID: 11647 RVA: 0x0016825C File Offset: 0x0016645C
	private void UpdateDialog()
	{
		bool isSettlementTreasury = this._openShopEventArguments.IsSettlementTreasury;
		if (!isSettlementTreasury)
		{
			bool flag = !this._leftCloudBubble.BubbleHide;
			if (!flag)
			{
				this._timer += Time.deltaTime;
				bool flag2 = this._onlyIntroduceDialog || this._isCharacterOnSpecificatedMerchant;
				if (flag2)
				{
					bool flag3 = this._timer >= 30f;
					if (flag3)
					{
						this.BubbleStartCoroutine(this.GetIntroduceDialog(), BubbleLevel.Low, false);
						this._timer = 0f;
					}
				}
				else
				{
					int index = 1;
					bool flag4 = this._timer >= 10f * (float)index++ && !this._firstIntroduceDialog;
					if (flag4)
					{
						this._firstIntroduceDialog = true;
						this.BubbleStartCoroutine(this.GetIntroduceDialog(), BubbleLevel.Low, false);
					}
					bool flag5 = this._extraGoods != null && this._extraGoods.SeasonTemplateId >= 0 && this._extraGoods.SeasonExtraGoods.Count > 0;
					if (flag5)
					{
						bool flag6 = this._timer >= 10f * (float)index++ && !this._seasonDialog;
						if (flag6)
						{
							this._seasonDialog = true;
							this.BubbleStartCoroutine(this.GetSeasonDialog(), BubbleLevel.Low, true);
						}
					}
					bool flag7 = this._timer >= 10f * (float)index++ && !this._favorDialog;
					if (flag7)
					{
						this._favorDialog = true;
						this.BubbleStartCoroutine(this.GetFavorDialog(GameData.Domains.Merchant.SharedMethods.GetFavorLevel(UI_Shop._merchantFavorability)), BubbleLevel.Low, false);
					}
					bool flag8 = this._timer >= 10f * (float)index++ && !this._remainDialog;
					if (flag8)
					{
						this._remainDialog = true;
						this.BubbleStartCoroutine((Random.Range(0, 2) == 0) ? LocalStringManager.Get(LanguageKey.LK_Shop_Speak_Remain1) : LocalStringManager.Get(LanguageKey.LK_Shop_Speak_Remain2), BubbleLevel.Low, false);
					}
					bool flag9 = this._timer >= 10f * (float)index++ && !this._secondIntroduceDialog;
					if (flag9)
					{
						this._secondIntroduceDialog = true;
						this.BubbleStartCoroutine(this.GetIntroduceDialog(), BubbleLevel.Low, false);
						this._onlyIntroduceDialog = true;
						this._timer = 0f;
					}
				}
			}
		}
	}

	// Token: 0x06002D80 RID: 11648 RVA: 0x001684A4 File Offset: 0x001666A4
	private string GetIntroduceDialog()
	{
		return this._openShopEventArguments.IsSpecialBuilding ? LocalStringManager.Get(LanguageKey.LK_SectZhujian_SpecialMerchant_IntroduceDialog) : this._merchantTypeConfig.IntroduceDialog;
	}

	// Token: 0x06002D81 RID: 11649 RVA: 0x001684DC File Offset: 0x001666DC
	private string GetFavorDialog(int level)
	{
		bool isSpecialBuilding = this._openShopEventArguments.IsSpecialBuilding;
		if (isSpecialBuilding)
		{
			bool flag = level <= 2;
			if (flag)
			{
				return LocalStringManager.Get(LanguageKey.LK_SectZhujian_SpecialMerchant_FavorDialog1);
			}
			bool flag2 = level <= 3;
			if (flag2)
			{
				return LocalStringManager.Get(LanguageKey.LK_SectZhujian_SpecialMerchant_FavorDialog2);
			}
			bool flag3 = level <= 4;
			if (flag3)
			{
				return LocalStringManager.Get(LanguageKey.LK_SectZhujian_SpecialMerchant_FavorDialog3);
			}
		}
		bool flag4 = level <= 1;
		string result;
		if (flag4)
		{
			result = this._merchantTypeConfig.FavorDialog1;
		}
		else
		{
			bool flag5 = level <= 3;
			if (flag5)
			{
				result = this._merchantTypeConfig.FavorDialog2;
			}
			else
			{
				result = this._merchantTypeConfig.FavorDialog3;
			}
		}
		return result;
	}

	// Token: 0x06002D82 RID: 11650 RVA: 0x0016858C File Offset: 0x0016678C
	private string GetSeasonDialog()
	{
		bool flag = this._openShopEventArguments.MerchantSourceTypeEnum == OpenShopEventArguments.EMerchantSourceType.SingleAdventureCaravan;
		string result;
		if (flag)
		{
			string seasonName = LocalStringManager.Get(string.Format("LK_Season_{0}", this._extraGoods.SeasonTemplateId));
			seasonName += LocalStringManager.Get(LanguageKey.LK_Quarter);
			result = this._merchantTypeConfig.SpringMarketsAdventureSeasonDialog.GetFormat(seasonName);
		}
		else
		{
			sbyte seasonTemplateId = this._extraGoods.SeasonTemplateId;
			if (!true)
			{
			}
			string text;
			switch (seasonTemplateId)
			{
			case 0:
				text = this._merchantTypeConfig.SpringSeasonDialog;
				break;
			case 1:
				text = this._merchantTypeConfig.SummerSeasonDialog;
				break;
			case 2:
				text = this._merchantTypeConfig.AutumnSeasonDialog;
				break;
			case 3:
				text = this._merchantTypeConfig.WinterSeasonDialog;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (!true)
			{
			}
			string seasonDialog = text;
			result = seasonDialog;
		}
		return result;
	}

	// Token: 0x06002D83 RID: 11651 RVA: 0x0016866C File Offset: 0x0016686C
	private void RefreshSettlementTreasury()
	{
		this.RefreshTreasuryBalanceButton();
		this.RefreshTreasuryTrade();
		this.UpdateTradeAreaDisplay();
		OrganizationItem orgConfig = Organization.Instance[this._settlementTreasuryDisplayData.OrgTemplateId];
		string settlementName = this.GetSettlementName();
		base.CGet<TextMeshProUGUI>("ShopTitle").text = settlementName + LocalStringManager.Get(LanguageKey.LK_Building_Treasury);
		Refers settlementTreasuryRoot = base.CGet<Refers>("TreasuryInfoRoot");
		string typeIcon = orgConfig.IsSect ? "shop_treasury_type_0" : "shop_treasury_type_1";
		settlementTreasuryRoot.CGet<CImage>("SettlementTypeIcon").SetSprite(typeIcon, false, null);
		string levelIcon = this._settlementTreasuryDisplayData.GetDisplayLevelIcon();
		settlementTreasuryRoot.CGet<CImage>("SettlementLevelIcon").SetSprite(levelIcon, false, null);
		settlementTreasuryRoot.CGet<TextMeshProUGUI>("SettlementLevel").text = this._settlementTreasuryDisplayData.GetDisplayLevelStr();
		TooltipInvoker tip = settlementTreasuryRoot.CGet<TooltipInvoker>("TipArea");
		tip.enabled = Organization.Instance[this._settlementTreasuryDisplayData.OrgTemplateId].IsSect;
		tip.Type = TipType.SettlementTreasury;
		tip.RuntimeParam = EasyPool.Get<ArgumentBox>().SetObject("SettlementTreasuryDisplayData", this._settlementTreasuryDisplayData);
		Transform frontTrans = base.CGet<GameObject>("Front").transform;
		int lastIndex = frontTrans.childCount - 1;
		for (int i = 0; i < frontTrans.childCount; i++)
		{
			frontTrans.GetChild(i).gameObject.SetActive(lastIndex == i);
		}
		int settlementTreasuryLayerIndex = this.SettlementTreasuryLayerIndex;
		if (!true)
		{
		}
		CharacterDisplayData[] array;
		switch (settlementTreasuryLayerIndex)
		{
		case 0:
			array = this._settlementTreasuryDisplayData.GuardianCharacterDisplayDataLow;
			break;
		case 1:
			array = this._settlementTreasuryDisplayData.GuardianCharacterDisplayDataMid;
			break;
		case 2:
			array = this._settlementTreasuryDisplayData.GuardianCharacterDisplayDataHigh;
			break;
		default:
			array = null;
			break;
		}
		if (!true)
		{
		}
		CharacterDisplayData[] guardianCharacter = array;
		bool showCharacter = guardianCharacter != null && guardianCharacter.Length != 0 && this._settlementTreasuryTogGroup.GetAll()[0].interactable;
		base.CGet<GameObject>("CharacterInfoRoot").gameObject.SetActive(showCharacter);
		bool flag = showCharacter;
		if (flag)
		{
			this._merchantCharData = guardianCharacter.FirstOrDefault<CharacterDisplayData>();
			this.RefreshMerchantCharData();
		}
		ItemDomainMethod.AsyncCall.GetItemDisplayDataListOptionalFromInventory(this, this._settlementTreasury.Inventory, -1, ItemSourceType.SettlementTreasury.ToSbyte(), false, delegate(int offset, RawDataPool dataPool)
		{
			this._settlementTreasuryItems.Clear();
			Serializer.Deserialize(dataPool, offset, ref this._settlementTreasuryItems);
			this.AddTempResourcesItem(this._settlementTreasury.Resources, this._settlementTreasuryItems);
			this._shopItemScroll.SetItemList(ref this._settlementTreasuryItems, false, null, null, null, true);
			this.UpdateAllShopItemInteractable();
			bool isGetLastSettlementTreasuryOperationData = this._isGetLastSettlementTreasuryOperationData;
			if (isGetLastSettlementTreasuryOperationData)
			{
				OrganizationDomainMethod.AsyncCall.GetLastSettlementTreasuryOperationData(this, delegate(int offset, RawDataPool dataPool)
				{
					List<ItemSourceChange> changes = null;
					Serializer.Deserialize(dataPool, offset, ref changes);
					this.InitSettlementTreasuryOperation(changes);
					this.Element.ShowAfterRefresh();
				});
			}
		});
		this._settlementTreasuryResourceMaxValue = Accessory.Instance.First((AccessoryItem a) => a.Grade == 8).BaseValue;
	}

	// Token: 0x06002D84 RID: 11652 RVA: 0x00168904 File Offset: 0x00166B04
	private void InitSettlementTreasuryOperation(List<ItemSourceChange> changes)
	{
		bool flag = !this._openShopEventArguments.IsSettlementTreasury || changes == null;
		if (!flag)
		{
			foreach (ItemSourceChange change in changes)
			{
				foreach (ItemKeyAndCount itemKeyAndCount in change.Items)
				{
					ItemKey itemKey2;
					int num;
					itemKeyAndCount.Deconstruct(out itemKey2, out num);
					ItemKey itemKey = itemKey2;
					int amount = num;
					bool isBuy = amount >= 0;
					List<ItemDisplayData> list;
					if (isBuy)
					{
						list = this._settlementTreasuryItems;
					}
					else
					{
						ItemSourceType itemSourceTypeEnum = change.ItemSourceTypeEnum;
						if (!true)
						{
						}
						List<ItemDisplayData> list2;
						switch (itemSourceTypeEnum)
						{
						case ItemSourceType.Equipment:
							list2 = this._inventoryItems;
							break;
						case ItemSourceType.Inventory:
							list2 = this._inventoryItems;
							break;
						case ItemSourceType.Warehouse:
							list2 = this._warehouseItems;
							break;
						case ItemSourceType.Treasury:
							list2 = this._treasuryItems;
							break;
						default:
							if (!true)
							{
							}
							<PrivateImplementationDetails>.ThrowSwitchExpressionException(itemSourceTypeEnum);
							break;
						}
						if (!true)
						{
						}
						list = list2;
					}
					List<ItemDisplayData> itemList = list;
					ItemDisplayData itemData = itemList.Find((ItemDisplayData d) => d.ContainsItemKey(itemKey));
					bool flag2 = itemData == null;
					if (!flag2)
					{
						bool flag3 = isBuy;
						if (flag3)
						{
							this.PutShopItemToTrade(itemData, amount, this.GetItemPrice(itemData, true, int.MinValue), false, false);
						}
						else
						{
							this.PutSelfItemToTrade(itemData, -amount, this.GetItemPrice(itemData, false, int.MinValue), false, false, change.ItemSourceTypeEnum);
						}
					}
				}
			}
			this.RefreshAfterPutSelfItemToTrade();
			this.RefreshAfterPutShopItemToTrade();
		}
	}

	// Token: 0x06002D85 RID: 11653 RVA: 0x00168AEC File Offset: 0x00166CEC
	private void RefreshTreasuryLoveItem(ItemDisplayData itemData, Refers view, bool show)
	{
		bool flag = !this._openShopEventArguments.IsSettlementTreasury;
		if (!flag)
		{
			short itemSubType = ItemTemplateHelper.GetItemSubType(itemData.Key.ItemType, itemData.Key.TemplateId);
			bool isLoving = show && this._settlementTreasury.LovingItemSubTypes.Contains(itemSubType);
			bool isHating = show && this._settlementTreasury.HatingItemSubTypes.Contains(itemSubType);
			ItemView itemView = view as ItemView;
			bool flag2 = itemView != null;
			if (flag2)
			{
				itemView.SetHateAndLoveIconVisibility(isHating, isLoving);
			}
			else
			{
				CommonTableRowForItem tableRowForItem = view as CommonTableRowForItem;
				bool flag3 = tableRowForItem != null;
				if (flag3)
				{
					CommonTableRowForItem.FavoriteStatus favoriteStatus = isLoving ? CommonTableRowForItem.FavoriteStatus.Love : (isHating ? CommonTableRowForItem.FavoriteStatus.Hate : CommonTableRowForItem.FavoriteStatus.None);
					tableRowForItem.SetFavoriteStatus(favoriteStatus);
				}
			}
		}
	}

	// Token: 0x06002D86 RID: 11654 RVA: 0x00168BAC File Offset: 0x00166DAC
	private int CalcResourceValue(sbyte resourceType, int amount)
	{
		int orgTemplateId = this._settlementTreasuryDisplayData.OrgTemplateId;
		OrganizationItem orgConfig = Organization.Instance[orgTemplateId];
		short orgMemberId = orgConfig.Members[8];
		OrganizationMemberItem memberConfig = OrganizationMember.Instance[orgMemberId];
		long value = memberConfig.AdjustResourceValue(resourceType, (long)amount);
		int result = Convert.ToInt32(value * (long)GlobalConfig.Instance.ResourceContributionPercent / 100L);
		return Math.Min(int.MaxValue, result);
	}

	// Token: 0x06002D87 RID: 11655 RVA: 0x00168C20 File Offset: 0x00166E20
	private int CalcResourceAmount(sbyte resourceType, int value)
	{
		int tempValue = Convert.ToInt32((long)value * 100L / (long)GlobalConfig.Instance.ResourceContributionPercent);
		int orgTemplateId = this._settlementTreasuryDisplayData.OrgTemplateId;
		OrganizationItem orgConfig = Organization.Instance[orgTemplateId];
		short orgMemberId = orgConfig.Members[8];
		OrganizationMemberItem memberConfig = OrganizationMember.Instance[orgMemberId];
		int amount = (int)memberConfig.AdjustResourceAmount(resourceType, (long)tempValue, false);
		int previewValue = this.CalcResourceValue(resourceType, amount);
		bool flag = previewValue >= value;
		int result;
		if (flag)
		{
			result = amount;
		}
		else
		{
			while (previewValue < value)
			{
				previewValue = this.CalcResourceValue(resourceType, ++amount);
			}
			result = amount;
		}
		return result;
	}

	// Token: 0x06002D88 RID: 11656 RVA: 0x00168CC8 File Offset: 0x00166EC8
	private void RefreshTreasuryTrade()
	{
		bool flag = !this._openShopEventArguments.IsSettlementTreasury;
		if (!flag)
		{
			this._getTreasuryInventory.Items.Clear();
			this._stealTreasuryInventory.Items.Clear();
			this._exchangeTreasuryInventory.Items.Clear();
			this._storeTreasuryInventory.Items.Clear();
			this._treasuryTradeItemsMerchant.Clear();
			this._treasuryTradeItemsSelf.Clear();
			this._treasuryTradeItemsMerchant.AddRange(this._tradeItemsMerchant);
			this._treasuryTradeItemsSelf.AddRange(this._tradeItemsSelf);
			int itemWorth = 0;
			RectTransform actionLayout = base.CGet<RectTransform>("ActionLayout");
			for (int i = 0; i < GlobalConfig.Instance.SettlementTreasuryGetItemMaxCount; i++)
			{
				CImage actionImage = actionLayout.GetChild(i).GetComponent<CImage>();
				ItemDisplayData buyItemData = this._treasuryTradeItemsMerchant.CheckIndex(i) ? this._treasuryTradeItemsMerchant[i] : null;
				ItemDisplayData sellItemData = this._treasuryTradeItemsSelf.CheckIndex(i) ? this._treasuryTradeItemsSelf[i] : null;
				int buyWorth = this.GetTreasuryItemPrice(buyItemData);
				int sellWorth = this.GetTreasuryItemPrice(sellItemData);
				bool flag2 = buyWorth == -1 || sellWorth == -1;
				if (flag2)
				{
					bool flag3 = buyWorth == -1 && sellWorth == -1;
					if (flag3)
					{
						actionImage.SetSprite("", false, null);
					}
					else
					{
						bool flag4 = sellWorth >= 0;
						if (flag4)
						{
							this._storeTreasuryInventory.OfflineAdd(sellItemData.Key, sellItemData.Amount);
							sellItemData.TreasuryOperation = ETreasuryOperation.Store;
							actionImage.SetSprite("shop_treasury_base_3", false, null);
						}
						else
						{
							bool flag5 = buyWorth >= 0;
							if (flag5)
							{
								this._stealTreasuryInventory.OfflineAdd(buyItemData.Key, buyItemData.Amount);
								buyItemData.TreasuryOperation = ETreasuryOperation.Steal;
								actionImage.SetSprite("shop_treasury_base_2", false, null);
							}
						}
					}
				}
				else
				{
					bool flag6 = sellWorth >= buyWorth;
					if (flag6)
					{
						itemWorth += buyWorth;
						this._exchangeTreasuryInventory.OfflineAdd(sellItemData.Key, sellItemData.Amount);
						this._getTreasuryInventory.OfflineAdd(buyItemData.Key, buyItemData.Amount);
						buyItemData.TreasuryOperation = ETreasuryOperation.Exchange;
						sellItemData.TreasuryOperation = ETreasuryOperation.Invalid;
						actionImage.SetSprite("shop_treasury_base_3", false, null);
					}
					else
					{
						this._stealTreasuryInventory.OfflineAdd(buyItemData.Key, buyItemData.Amount);
						buyItemData.TreasuryOperation = ETreasuryOperation.Steal;
						sellItemData.TreasuryOperation = ETreasuryOperation.Invalid;
						actionImage.SetSprite("shop_treasury_base_2", false, null);
					}
				}
			}
			int authority = Debts.WorthToResourceAmount(7, (long)itemWorth, false);
			this._settlementTreasuryNeedAuthority = authority;
			bool hasSteal = this._stealTreasuryInventory.InventoryItemTotalCount > 0;
			bool hasStore = this._storeTreasuryInventory.InventoryItemTotalCount > 0;
			bool hasExchange = this._exchangeTreasuryInventory.InventoryItemTotalCount > 0;
			bool showSteal = !hasExchange && hasSteal;
			bool showStore = !hasExchange && !hasSteal && hasStore;
			bool showExchange = hasExchange && !hasSteal;
			bool showExchangeAndSteal = hasExchange && hasSteal;
			base.CGet<GameObject>("StealTip").SetActive(showSteal);
			base.CGet<GameObject>("StoreTip").SetActive(showStore);
			base.CGet<GameObject>("ExchangeAndStealTip").SetActive(showExchangeAndSteal);
			Refers exchangeTip = base.CGet<Refers>("ExchangeTip");
			exchangeTip.gameObject.SetActive(showExchange);
			bool flag7 = showExchange;
			if (flag7)
			{
				TextMeshProUGUI text = exchangeTip.CGet<TextMeshProUGUI>("Text");
				ResourceTypeItem config = Config.ResourceType.Instance[7];
				string authorityColor = (this._selfResources.Get(7) >= authority) ? "brightblue" : "brightred";
				string content = config.Name + CommonUtils.GetDisplayStringForNum(authority, 100000).SetColor(authorityColor);
				text.text = LocalStringManager.GetFormat(LanguageKey.LK_Building_Treasury_ExchangeTip, content).ColorReplace();
				text.GetComponent<TMPTextSpriteHelper>().Parse();
			}
			string symbol = LocalStringManager.Get(LanguageKey.LK_Colon_Symbol);
			int curSelfCount = this._tradeItemsMerchant.Count;
			int maxSelfCount = GlobalConfig.Instance.SettlementTreasuryGetItemMaxCount;
			string selfCountColor = (curSelfCount == maxSelfCount) ? "brightred" : "brightblue";
			string selfCountStr = curSelfCount.ToString().SetColor(selfCountColor);
			string getName = LocalStringManager.Get(LanguageKey.LK_Building_Treasury_Get);
			base.CGet<TextMeshProUGUI>("GetCount").text = string.Format("{0}{1}{2}/{3}", new object[]
			{
				getName,
				symbol,
				selfCountStr,
				maxSelfCount
			});
			int curShopCount = this._tradeItemsSelf.Count;
			int maxShopCount = GlobalConfig.Instance.SettlementTreasuryGiveItemMaxCount;
			string shopCountColor = (curShopCount == maxShopCount) ? "brightred" : "brightblue";
			string shopCountStr = curShopCount.ToString().SetColor(shopCountColor);
			string giveName = LocalStringManager.Get(LanguageKey.LK_Building_Treasury_Give);
			base.CGet<TextMeshProUGUI>("GiveCount").text = string.Format("{0}{1}{2}/{3}", new object[]
			{
				giveName,
				symbol,
				shopCountStr,
				maxShopCount
			});
			this.RefreshTreasuryBalanceButton();
		}
	}

	// Token: 0x06002D89 RID: 11657 RVA: 0x001691DC File Offset: 0x001673DC
	private void RefreshTreasuryTradeItem(ItemDisplayData itemData, Refers view)
	{
		bool flag = !this._openShopEventArguments.IsSettlementTreasury;
		if (!flag)
		{
			ItemView itemView = view as ItemView;
			bool flag2 = itemView != null;
			if (flag2)
			{
				itemView.SetTreasuryTradeMark(itemData.TreasuryOperation);
			}
			else
			{
				CommonTableRowForItem tableRowForItem = view as CommonTableRowForItem;
				bool flag3 = tableRowForItem != null;
				if (flag3)
				{
					tableRowForItem.SetTreasuryTradeMark(itemData.TreasuryOperation);
				}
			}
		}
	}

	// Token: 0x06002D8A RID: 11658 RVA: 0x0016923C File Offset: 0x0016743C
	private void SortTradeItems(List<ItemDisplayData> list)
	{
		bool flag = !this._openShopEventArguments.IsSettlementTreasury;
		if (!flag)
		{
			list.Sort(delegate(ItemDisplayData a, ItemDisplayData b)
			{
				int aValue = this.GetTreasuryItemPrice(a);
				int bValue = this.GetTreasuryItemPrice(b);
				bool flag2 = aValue == bValue;
				int result;
				if (flag2)
				{
					result = 0;
				}
				else
				{
					result = ((aValue > bValue) ? -1 : 1);
				}
				return result;
			});
		}
	}

	// Token: 0x06002D8B RID: 11659 RVA: 0x00169274 File Offset: 0x00167474
	private void OnClickBtnTreasuryRecord()
	{
		ArgumentBox args = EasyPool.Get<ArgumentBox>().Set("SettlementId", this._openShopEventArguments.SettlementId);
		UIElement.SettlementTreasuryRecords.SetOnInitArgs(args);
		UIManager.Instance.ShowUI(UIElement.SettlementTreasuryRecords, true);
	}

	// Token: 0x06002D8C RID: 11660 RVA: 0x001692BC File Offset: 0x001674BC
	private string GetSettlementName()
	{
		WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
		MapBlockData blockData = worldMapModel.CurrentBlockData;
		MapBlockItem blockConfig = MapBlock.Instance[blockData.TemplateId];
		string result;
		switch (blockConfig.Type)
		{
		case EMapBlockType.City:
			result = blockConfig.Name;
			break;
		case EMapBlockType.Sect:
		{
			OrganizationItem orgConfig = Organization.Instance[this._settlementTreasuryDisplayData.OrgTemplateId];
			result = orgConfig.Name;
			break;
		}
		case EMapBlockType.Town:
			result = worldMapModel.GetCurBlockName();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		return result;
	}

	// Token: 0x06002D8D RID: 11661 RVA: 0x00169348 File Offset: 0x00167548
	private void RefreshTreasuryBalanceButton()
	{
		CButtonObsolete buttonBalance = base.CGet<CButtonObsolete>("ButtonBalance");
		buttonBalance.gameObject.SetActive(this._openShopEventArguments.IsSettlementTreasury);
		bool flag = !this._openShopEventArguments.IsSettlementTreasury;
		if (!flag)
		{
			bool hasTarget = this._tradeItemsMerchant.Count > this._tradeItemsSelf.Count;
			bool hasResource = this.CurSelfItemList.Any((ItemDisplayData d) => d.IsResource && d.Amount > 0);
			buttonBalance.interactable = (hasTarget && hasResource);
			TooltipInvoker tip = buttonBalance.GetComponent<TooltipInvoker>();
			tip.enabled = !buttonBalance.interactable;
			bool flag2 = !tip.enabled;
			if (!flag2)
			{
				StringBuilder sb = EasyPool.Get<StringBuilder>();
				sb.Clear();
				bool flag3 = !hasTarget;
				if (flag3)
				{
					sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_Building_Treasury_Balance_Tip_NotTarget).SetColor("brightred"));
				}
				bool flag4 = !hasResource;
				if (flag4)
				{
					sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_Building_Treasury_Balance_Tip_NoResource).SetColor("brightred"));
				}
				string[] presetParam = tip.PresetParam;
				bool flag5 = presetParam == null || presetParam.Length != 1;
				if (flag5)
				{
					tip.PresetParam = new string[1];
				}
				tip.PresetParam[0] = sb.ToString();
				sb.Clear();
				EasyPool.Free<StringBuilder>(sb);
			}
		}
	}

	// Token: 0x06002D8E RID: 11662 RVA: 0x001694B0 File Offset: 0x001676B0
	private void OnClickButtonBalance()
	{
		bool hasMove = false;
		List<ItemDisplayData> resourceItemList = (from d in this.CurSelfItemList
		where d.IsResource
		select d).ToList<ItemDisplayData>();
		for (int i = this._tradeItemsSelf.Count; i < this._tradeItemsMerchant.Count; i++)
		{
			ItemDisplayData itemData = this._tradeItemsMerchant[i];
			int targetPrice = this.GetItemPrice(itemData, true, int.MinValue);
			ItemDisplayData resourceItem = (from d in resourceItemList
			where d.Amount > 0
			orderby d.Amount descending
			select d).FirstOrDefault<ItemDisplayData>();
			bool flag = resourceItem == null;
			if (!flag)
			{
				sbyte resourceType = resourceItem.ResourceType;
				int targetAmount = this.CalcResourceAmount(resourceType, targetPrice);
				int maxCount = Debts.WorthToResourceAmount((short)resourceType, (long)this._settlementTreasuryResourceMaxValue, false);
				int minCount = this.CalcResourceAmount(resourceType, GlobalConfig.Instance.SettlementTreasuryGetResourceMinValue);
				targetAmount = Math.Clamp(targetAmount, minCount, maxCount);
				bool flag2 = resourceItem.Amount < targetAmount;
				if (!flag2)
				{
					int price = this.GetItemPrice(resourceItem, false, int.MinValue);
					this.PutSelfItemToTrade(resourceItem, targetAmount, price, false, false, ItemSourceType.Invalid);
					hasMove = true;
				}
			}
		}
		bool flag3 = hasMove;
		if (flag3)
		{
			this.PlayMoveItemSound();
			this.RefreshAfterPutSelfItemToTrade();
		}
	}

	// Token: 0x06002D8F RID: 11663 RVA: 0x0016962C File Offset: 0x0016782C
	private void OnClickButtonReplenish()
	{
		ArgumentBox args = EasyPool.Get<ArgumentBox>().Set("InfluenceRefreshTime", (int)this._settlementTreasuryDisplayData.InfluenceRefreshTime).Set<Inventory>("SupplyItems", this._settlementTreasuryDisplayData.SupplyItems).SetObject("SupplyCounts", this._settlementTreasuryDisplayData.SupplyCounts);
		UIElement.SettlementTreasuryReplenish.SetOnInitArgs(args);
		UIManager.Instance.MaskUI(UIElement.SettlementTreasuryReplenish);
	}

	// Token: 0x06002D90 RID: 11664 RVA: 0x0016969B File Offset: 0x0016789B
	private void RefreshPriceIcon(Refers itemView)
	{
	}

	// Token: 0x06002D91 RID: 11665 RVA: 0x001696A0 File Offset: 0x001678A0
	private void SetResourceItemTip(Refers view, bool isIsResource, bool isTaiwu)
	{
		bool flag = this._taiwuCharacterDisplayData == null || !isIsResource;
		if (!flag)
		{
			string charName = isTaiwu ? NameCenter.GetMonasticTitleOrDisplayName(this._taiwuCharacterDisplayData, true) : this.GetSettlementName();
			ItemView itemView = view as ItemView;
			bool flag2 = itemView != null;
			if (flag2)
			{
				itemView.SetResourceTip(charName, isTaiwu);
			}
			else
			{
				CommonTableRowForItem tableRowForItem = view as CommonTableRowForItem;
				bool flag3 = tableRowForItem != null;
				if (flag3)
				{
					tableRowForItem.SetResourceTip(charName, isTaiwu);
				}
			}
		}
	}

	// Token: 0x06002D92 RID: 11666 RVA: 0x00169714 File Offset: 0x00167914
	private void ShowMerchantInfo()
	{
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("MerchantType", this._merchantData.MerchantType).Set("MerchantFavorability", UI_Shop._merchantFavorability).Set("MerchantTemplateId", this._merchantData.MerchantTemplateId);
		UIElement.MerchantInfo.SetOnInitArgs(argumentBox);
		UIManager.Instance.ShowUI(UIElement.MerchantInfo, true);
	}

	// Token: 0x06002D93 RID: 11667 RVA: 0x00169780 File Offset: 0x00167980
	private void SetVillagerNeedMark(Refers view, ItemKey itemKey, ItemSourceType sourceType)
	{
		bool sourceTypeIsMeet = sourceType == ItemSourceType.Treasury;
		ItemKey tempKey = ItemKey.Invalid;
		bool flag = sourceTypeIsMeet;
		if (flag)
		{
			tempKey.ItemType = itemKey.ItemType;
			tempKey.TemplateId = itemKey.TemplateId;
		}
		bool isNeeded = this._villagerNeededItemSet.Contains(tempKey);
		ItemView itemView = view as ItemView;
		bool flag2 = itemView != null;
		if (flag2)
		{
			itemView.SetVillagerNeedMark(isNeeded, true);
		}
		else
		{
			CommonTableRowForItem tableRowForItem = view as CommonTableRowForItem;
			bool flag3 = tableRowForItem != null;
			if (flag3)
			{
				tableRowForItem.SetVillagerNeedMark(isNeeded);
			}
		}
	}

	// Token: 0x06002D94 RID: 11668 RVA: 0x00169804 File Offset: 0x00167A04
	private void AddTempResourcesItem(ResourceInts resources, List<ItemDisplayData> items)
	{
		for (int type = 0; type < 7; type++)
		{
			int amount = resources.Get(type);
			bool flag = amount <= 0;
			if (!flag)
			{
				short templateId = Convert.ToInt16(type);
				ItemKey itemKey = new ItemKey(12, 0, templateId, 0);
				ItemDisplayData itemData = new ItemDisplayData
				{
					Key = itemKey,
					Amount = amount
				};
				items.Add(itemData);
			}
		}
	}

	// Token: 0x17000502 RID: 1282
	// (get) Token: 0x06002D95 RID: 11669 RVA: 0x00169870 File Offset: 0x00167A70
	internal bool IsAreaDebtShop
	{
		get
		{
			bool flag = !this._isCharacterOnSpecificatedMerchant;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				MerchantTypeItem merchantTypeConfig = this._merchantTypeConfig;
				sbyte? b = (merchantTypeConfig != null) ? new sbyte?(merchantTypeConfig.TemplateId) : null;
				if (!true)
				{
				}
				bool flag2;
				if (b != null)
				{
					sbyte valueOrDefault = b.GetValueOrDefault();
					if (valueOrDefault == 8)
					{
						flag2 = true;
						goto IL_58;
					}
				}
				flag2 = false;
				IL_58:
				if (!true)
				{
				}
				result = flag2;
			}
			return result;
		}
	}

	// Token: 0x06002D96 RID: 11670 RVA: 0x001698DE File Offset: 0x00167ADE
	private static bool IsSealOfMerchant(KeyValuePair<ItemKey, int> data)
	{
		return UI_Shop.IsSealOfMerchant(data.Key);
	}

	// Token: 0x06002D97 RID: 11671 RVA: 0x001698EC File Offset: 0x00167AEC
	private static bool IsSealOfMerchant(ItemDisplayData data)
	{
		return UI_Shop.IsSealOfMerchant(data.Key);
	}

	// Token: 0x06002D98 RID: 11672 RVA: 0x001698FC File Offset: 0x00167AFC
	private static bool IsSealOfMerchant(ItemKey itemKey)
	{
		return itemKey.TemplateEquals(12, 380);
	}

	// Token: 0x06002D9C RID: 11676 RVA: 0x00169B20 File Offset: 0x00167D20
	[CompilerGenerated]
	private void <OnNotifyGameData>g__SetMouseTipSettlementTreasuryOrPrisonLayer|139_0(bool isSect, int value, bool[] settlementTreasuryGuaerdFavorStatus)
	{
		for (int i = 0; i < settlementTreasuryGuaerdFavorStatus.Length; i++)
		{
			if (!true)
			{
			}
			CharacterDisplayData[] array;
			switch (i)
			{
			case 0:
				array = this._settlementTreasuryDisplayData.GuardianCharacterDisplayDataLow;
				break;
			case 1:
				array = this._settlementTreasuryDisplayData.GuardianCharacterDisplayDataMid;
				break;
			case 2:
				array = this._settlementTreasuryDisplayData.GuardianCharacterDisplayDataHigh;
				break;
			default:
				array = null;
				break;
			}
			if (!true)
			{
			}
			CharacterDisplayData[] guardianCharacter = array;
			CToggleObsolete toggle = this._settlementTreasuryTogGroup.Get(i);
			this._settlementTreasuryAccessable[i] = (this._settlementTreasuryAccessable[i] ? true : ((isSect && !settlementTreasuryGuaerdFavorStatus[i]) ? false : (isSect ? (value >= ((i == 1) ? GlobalConfig.Instance.TreasuryRquireApprovingMid : GlobalConfig.Instance.TreasuryRquireApprovingHigh)) : (value >= ((i == 1) ? GlobalConfig.Instance.TreasuryRquireSpiritualDebtMid : GlobalConfig.Instance.TreasuryRquireSpiritualDebtHigh)))));
			bool isDebtOrSupportEnough = isSect ? (value >= ((i == 1) ? GlobalConfig.Instance.TreasuryRquireApprovingMid : GlobalConfig.Instance.TreasuryRquireApprovingHigh)) : (value >= ((i == 1) ? GlobalConfig.Instance.TreasuryRquireSpiritualDebtMid : GlobalConfig.Instance.TreasuryRquireSpiritualDebtHigh));
			toggle.interactable = this._settlementTreasuryAccessable[i];
			TooltipInvoker mouseTip = toggle.GetComponent<TooltipInvoker>();
			TooltipInvoker tooltipInvoker = mouseTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			mouseTip.RuntimeParam.Set("layerIndex", i);
			mouseTip.RuntimeParam.Set("isFavor", settlementTreasuryGuaerdFavorStatus[i]);
			mouseTip.RuntimeParam.Set("isSect", isSect);
			mouseTip.RuntimeParam.Set("isDebtOrSupportEnough", isDebtOrSupportEnough);
			mouseTip.RuntimeParam.Set<CharacterDisplayData>("guardianCharacterDisplayData", guardianCharacter.FirstOrDefault<CharacterDisplayData>());
		}
		bool flag = this.SettlementTreasuryLayerIndex == 0;
		if (flag)
		{
			this._settlementTreasuryTogGroup.SetInteractable(true, null);
		}
		else
		{
			bool isDebtOrSupportEnough2 = isSect ? (value >= ((this.SettlementTreasuryLayerIndex == 1) ? GlobalConfig.Instance.TreasuryRquireApprovingMid : GlobalConfig.Instance.TreasuryRquireApprovingHigh)) : (value >= ((this.SettlementTreasuryLayerIndex == 1) ? GlobalConfig.Instance.TreasuryRquireSpiritualDebtMid : GlobalConfig.Instance.TreasuryRquireSpiritualDebtHigh));
			bool isCanUse = isSect ? (settlementTreasuryGuaerdFavorStatus[this.SettlementTreasuryLayerIndex] && isDebtOrSupportEnough2) : isDebtOrSupportEnough2;
			this._settlementTreasuryTogGroup.SetInteractable(isCanUse, null);
		}
	}

	// Token: 0x06002D9D RID: 11677 RVA: 0x00169D84 File Offset: 0x00167F84
	[CompilerGenerated]
	private void <QuickHide>g__TreasuryHideActions|149_0()
	{
		this.<QuickHide>g__HideActions|149_1();
	}

	// Token: 0x06002D9E RID: 11678 RVA: 0x00169D8E File Offset: 0x00167F8E
	[CompilerGenerated]
	private void <QuickHide>g__HideActions|149_1()
	{
		TaiwuEventDomainMethod.Call.TriggerListener("ShopActionComplete", true);
		AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x06002D9F RID: 11679 RVA: 0x00169DC0 File Offset: 0x00167FC0
	[CompilerGenerated]
	internal static void <ShowItemPrice>g__ShowPrice|159_0(bool show, ref UI_Shop.<>c__DisplayClass159_0 A_1)
	{
		for (int i = 0; i < A_1.priceRoot.childCount; i++)
		{
			A_1.priceRoot.GetChild(i).gameObject.SetActive(show);
		}
		TextMeshProUGUI view;
		bool flag = A_1.itemView.CTryGet<TextMeshProUGUI>("Value", out view);
		if (flag)
		{
			view.rectTransform.parent.gameObject.SetActive(show);
		}
	}

	// Token: 0x040020AA RID: 8362
	private bool _protected;

	// Token: 0x040020AB RID: 8363
	private Action<bool> _refreshActiveStates;

	// Token: 0x040020AC RID: 8364
	private Action _refreshCount;

	// Token: 0x040020AD RID: 8365
	private Action<bool> _refreshTips;

	// Token: 0x040020AE RID: 8366
	private bool _isNormalCharacter;

	// Token: 0x040020AF RID: 8367
	private bool _sellingDisabled;

	// Token: 0x040020B0 RID: 8368
	private bool _isCharacterOnSpecificatedMerchant;

	// Token: 0x040020B1 RID: 8369
	private int _selfMoney;

	// Token: 0x040020B2 RID: 8370
	private ResourceInts _selfResources;

	// Token: 0x040020B3 RID: 8371
	private ResourceInts _treasuryResources;

	// Token: 0x040020B4 RID: 8372
	private List<short> _learnedSkillList = new List<short>();

	// Token: 0x040020B5 RID: 8373
	private int _maxInventoryLoad;

	// Token: 0x040020B6 RID: 8374
	private int _curInventoryLoad;

	// Token: 0x040020B7 RID: 8375
	private int _warehouseMaxLoad;

	// Token: 0x040020B8 RID: 8376
	private int _warehouseCurLoad;

	// Token: 0x040020B9 RID: 8377
	private int _taiwuCharId;

	// Token: 0x040020BA RID: 8378
	private int _merchantId;

	// Token: 0x040020BB RID: 8379
	private MerchantData _merchantData;

	// Token: 0x040020BC RID: 8380
	private MerchantBuyBackData _merchantBuyBackData;

	// Token: 0x040020BD RID: 8381
	private CharacterDisplayData _merchantCharData;

	// Token: 0x040020BE RID: 8382
	private static int _merchantFavorability;

	// Token: 0x040020BF RID: 8383
	private short _lovingItemSubType;

	// Token: 0x040020C0 RID: 8384
	private short _hatingItemSubType;

	// Token: 0x040020C1 RID: 8385
	private MerchantOverFavorData _originMerchantOverFavorData;

	// Token: 0x040020C2 RID: 8386
	private MerchantOverFavorData _tempMerchantOverFavorData;

	// Token: 0x040020C3 RID: 8387
	private readonly List<ItemDisplayData>[] _shopItemList = new List<ItemDisplayData>[8];

	// Token: 0x040020C4 RID: 8388
	private List<ItemDisplayData> _inventoryItems = new List<ItemDisplayData>();

	// Token: 0x040020C5 RID: 8389
	private List<ItemDisplayData> _warehouseItems = new List<ItemDisplayData>();

	// Token: 0x040020C6 RID: 8390
	private List<ItemDisplayData> _treasuryItems = new List<ItemDisplayData>();

	// Token: 0x040020C7 RID: 8391
	private List<ItemDisplayData> _settlementTreasuryItems = new List<ItemDisplayData>();

	// Token: 0x040020C8 RID: 8392
	private List<ItemKey> _villagerNeededItemSet = new List<ItemKey>();

	// Token: 0x040020C9 RID: 8393
	private List<ItemDisplayData> _tradeItemsMerchant = new List<ItemDisplayData>();

	// Token: 0x040020CA RID: 8394
	private List<ItemDisplayData> _tradeItemsSelf = new List<ItemDisplayData>();

	// Token: 0x040020CB RID: 8395
	private readonly Dictionary<ItemDisplayData, UI_Shop.TradeItemSource> _tradeItemSourceDict = new Dictionary<ItemDisplayData, UI_Shop.TradeItemSource>();

	// Token: 0x040020CC RID: 8396
	private readonly Dictionary<ItemKey, int> _tradeMoneySources = new Dictionary<ItemKey, int>();

	// Token: 0x040020CD RID: 8397
	private int _buyMoney;

	// Token: 0x040020CE RID: 8398
	private Game.Components.Avatar.Avatar _clerkAvatar;

	// Token: 0x040020CF RID: 8399
	private CharacterFavorability _favorabilityHandler;

	// Token: 0x040020D0 RID: 8400
	private ItemScrollViewForCommonTableRow _shopItemScroll;

	// Token: 0x040020D1 RID: 8401
	private ItemScrollViewForCommonTableRow _selfItemScroll;

	// Token: 0x040020D2 RID: 8402
	private CToggleGroupObsolete _shopItemTogGroup;

	// Token: 0x040020D3 RID: 8403
	private CToggleGroupObsolete _selfItemTogGroup;

	// Token: 0x040020D4 RID: 8404
	private CToggleGroupObsolete _settlementTreasuryTogGroup;

	// Token: 0x040020D5 RID: 8405
	private GameObject _shopItemMask;

	// Token: 0x040020D6 RID: 8406
	private Bubble _leftCloudBubble;

	// Token: 0x040020D7 RID: 8407
	private const float BubbleDuration = 8f;

	// Token: 0x040020D8 RID: 8408
	private const float SeasonBubbleDuration = 12f;

	// Token: 0x040020D9 RID: 8409
	private const float BubbleInterval = 10f;

	// Token: 0x040020DA RID: 8410
	private const float IntroduceBubbleInterval = 30f;

	// Token: 0x040020DB RID: 8411
	private float _timer = 0f;

	// Token: 0x040020DC RID: 8412
	private MerchantTypeItem _merchantTypeConfig;

	// Token: 0x040020DD RID: 8413
	private bool _onlyIntroduceDialog;

	// Token: 0x040020DE RID: 8414
	private bool _firstIntroduceDialog;

	// Token: 0x040020DF RID: 8415
	private bool _secondIntroduceDialog;

	// Token: 0x040020E0 RID: 8416
	private bool _favorDialog;

	// Token: 0x040020E1 RID: 8417
	private bool _seasonDialog;

	// Token: 0x040020E2 RID: 8418
	private bool _remainDialog;

	// Token: 0x040020E3 RID: 8419
	private BubbleLevel _currBubbleLevel;

	// Token: 0x040020E4 RID: 8420
	private ItemScrollViewForCommonTableRow _tradeMerchantScrollView;

	// Token: 0x040020E5 RID: 8421
	private ItemScrollViewForCommonTableRow _tradeSelfScrollView;

	// Token: 0x040020E6 RID: 8422
	private int _lastGoodsTogKey;

	// Token: 0x040020E7 RID: 8423
	private bool _isClickedBuyBack;

	// Token: 0x040020E8 RID: 8424
	public const int MaxMerchantFavorability = 100;

	// Token: 0x040020E9 RID: 8425
	public const int MaxMerchantLevel = 6;

	// Token: 0x040020EA RID: 8426
	private const int BuyBackTogIndex = 7;

	// Token: 0x040020EB RID: 8427
	private const int MinMerchantLevel = 0;

	// Token: 0x040020EC RID: 8428
	private const int BuyPriceBaseEffect = 200;

	// Token: 0x040020ED RID: 8429
	private const int SellPriceBaseEffect = 20;

	// Token: 0x040020EE RID: 8430
	[TupleElementNames(new string[]
	{
		"focusingItemView",
		"parent",
		"sibling"
	})]
	private ValueTuple<Refers, Transform, int> _focusingTuple;

	// Token: 0x040020EF RID: 8431
	private MerchantExtraGoodsData _extraGoods;

	// Token: 0x040020F0 RID: 8432
	private const int ExtraGoodsPriceEffect = 50;

	// Token: 0x040020F1 RID: 8433
	private OpenShopEventArguments _openShopEventArguments;

	// Token: 0x040020F2 RID: 8434
	private readonly Dictionary<ItemSourceType, ItemSourceChange> _itemChangeDict = new Dictionary<ItemSourceType, ItemSourceChange>
	{
		{
			ItemSourceType.Inventory,
			new ItemSourceChange(ItemSourceType.Inventory)
		},
		{
			ItemSourceType.Warehouse,
			new ItemSourceChange(ItemSourceType.Warehouse)
		},
		{
			ItemSourceType.Treasury,
			new ItemSourceChange(ItemSourceType.Treasury)
		},
		{
			ItemSourceType.Equipment,
			new ItemSourceChange(ItemSourceType.Equipment)
		}
	};

	// Token: 0x040020F3 RID: 8435
	private Coroutine coroutine;

	// Token: 0x040020F4 RID: 8436
	private SettlementTreasuryDisplayData _settlementTreasuryDisplayData;

	// Token: 0x040020F5 RID: 8437
	private SettlementTreasury _settlementTreasury;

	// Token: 0x040020F6 RID: 8438
	private readonly Inventory _getTreasuryInventory = new Inventory();

	// Token: 0x040020F7 RID: 8439
	private readonly Inventory _stealTreasuryInventory = new Inventory();

	// Token: 0x040020F8 RID: 8440
	private readonly Inventory _exchangeTreasuryInventory = new Inventory();

	// Token: 0x040020F9 RID: 8441
	private readonly Inventory _storeTreasuryInventory = new Inventory();

	// Token: 0x040020FA RID: 8442
	private int _settlementTreasuryNeedAuthority;

	// Token: 0x040020FB RID: 8443
	private readonly List<ItemDisplayData> _treasuryTradeItemsMerchant = new List<ItemDisplayData>();

	// Token: 0x040020FC RID: 8444
	private readonly List<ItemDisplayData> _treasuryTradeItemsSelf = new List<ItemDisplayData>();

	// Token: 0x040020FD RID: 8445
	private CharacterDisplayData _taiwuCharacterDisplayData;

	// Token: 0x040020FE RID: 8446
	private int _settlementTreasuryResourceMaxValue;

	// Token: 0x040020FF RID: 8447
	private bool _confirmedTreasuryOperation;

	// Token: 0x04002100 RID: 8448
	private bool _isGetLastSettlementTreasuryOperationData;

	// Token: 0x04002101 RID: 8449
	private readonly bool[] _settlementTreasuryAccessable = new bool[Enum.GetValues(typeof(SettlementTreasuryLayers)).Length];

	// Token: 0x04002102 RID: 8450
	private ItemScrollView _giveToTreasuryScrollView;

	// Token: 0x04002103 RID: 8451
	private ItemScrollView _getFromTreasuryScrollView;

	// Token: 0x04002104 RID: 8452
	private ItemDisplayData _currentOperationData;

	// Token: 0x02001675 RID: 5749
	public enum TradeItemSource
	{
		// Token: 0x0400A7FB RID: 43003
		Invalid = -1,
		// Token: 0x0400A7FC RID: 43004
		ShopList0,
		// Token: 0x0400A7FD RID: 43005
		ShopList1,
		// Token: 0x0400A7FE RID: 43006
		ShopList2,
		// Token: 0x0400A7FF RID: 43007
		ShopList3,
		// Token: 0x0400A800 RID: 43008
		ShopList4,
		// Token: 0x0400A801 RID: 43009
		ShopList5,
		// Token: 0x0400A802 RID: 43010
		ShopList6,
		// Token: 0x0400A803 RID: 43011
		ShopBuyBackList,
		// Token: 0x0400A804 RID: 43012
		Inventory,
		// Token: 0x0400A805 RID: 43013
		Warehouse,
		// Token: 0x0400A806 RID: 43014
		Treasury,
		// Token: 0x0400A807 RID: 43015
		SettlementTreasury
	}

	// Token: 0x02001676 RID: 5750
	private enum TogType
	{
		// Token: 0x0400A809 RID: 43017
		Inventory,
		// Token: 0x0400A80A RID: 43018
		Warehouse,
		// Token: 0x0400A80B RID: 43019
		Treasury
	}

	// Token: 0x02001677 RID: 5751
	private enum BuyModeTogKey
	{
		// Token: 0x0400A80D RID: 43021
		Buy,
		// Token: 0x0400A80E RID: 43022
		BuyBack
	}
}
