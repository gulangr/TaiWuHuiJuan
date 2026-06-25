using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using Game.Views.CharacterMenu;
using GameData.Domains.Building;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Taiwu;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UICommon.Character.Elements;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001B2 RID: 434
public class UI_CraftsmanPanel : UIBase
{
	// Token: 0x170002B4 RID: 692
	// (get) Token: 0x060019CE RID: 6606 RVA: 0x000A9188 File Offset: 0x000A7388
	private sbyte LiftSkillType
	{
		get
		{
			return UI_CraftsmanPanel.GetOperationNeedSkillType(this._currentCraftType);
		}
	}

	// Token: 0x170002B5 RID: 693
	// (get) Token: 0x060019CF RID: 6607 RVA: 0x000A9195 File Offset: 0x000A7395
	private int ResourceCount
	{
		get
		{
			return SingletonObject.getInstance<BuildingModel>().GetResourceCount(this._productResourceType);
		}
	}

	// Token: 0x170002B6 RID: 694
	// (get) Token: 0x060019D0 RID: 6608 RVA: 0x000A91A8 File Offset: 0x000A73A8
	private short CurItemSubType
	{
		get
		{
			ECraftType currentCraftType = this._currentCraftType;
			if (!true)
			{
			}
			short result;
			if (currentCraftType != ECraftType.Tea)
			{
				if (currentCraftType != ECraftType.Wine)
				{
					ArtisanOrder artisanOrder = this._artisanOrder;
					result = ((artisanOrder != null) ? artisanOrder.ItemSubType : -1);
				}
				else
				{
					result = 901;
				}
			}
			else
			{
				result = 900;
			}
			if (!true)
			{
			}
			return result;
		}
	}

	// Token: 0x060019D1 RID: 6609 RVA: 0x000A91F8 File Offset: 0x000A73F8
	public override void OnInit(ArgumentBox argsBox)
	{
		bool flag = argsBox != null;
		if (flag)
		{
			bool flag2 = !argsBox.Get("charId", out this._artisanId);
			if (flag2)
			{
				this._artisanId = -1;
			}
			else
			{
				argsBox.Get("craftsmanPanelType", out this._craftsmanType);
			}
		}
		this._panelMode = EPanelMode.Villager;
		this.SetCraftTypesData();
		this.BindUIElements();
		this.InitCraftTip();
		this.SetupVillagerCraftTagPanel();
		this.RefreshData();
	}

	// Token: 0x060019D2 RID: 6610 RVA: 0x000A926D File Offset: 0x000A746D
	private void Awake()
	{
		SingletonObject.getInstance<LifeSkillCombatModel>().EndEvent += this.OnLifeSkillCombatEnd;
	}

	// Token: 0x060019D3 RID: 6611 RVA: 0x000A9287 File Offset: 0x000A7487
	private void OnEnable()
	{
		GEvent.Add(UiEvents.OnConfirmVillagerCraftInputMaterial, new GEvent.Callback(this.OnConfirmVillagerCraftInputMaterial));
	}

	// Token: 0x060019D4 RID: 6612 RVA: 0x000A92A6 File Offset: 0x000A74A6
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.OnConfirmVillagerCraftInputMaterial, new GEvent.Callback(this.OnConfirmVillagerCraftInputMaterial));
	}

	// Token: 0x060019D5 RID: 6613 RVA: 0x000A92C5 File Offset: 0x000A74C5
	private void OnDestroy()
	{
		SingletonObject.getInstance<LifeSkillCombatModel>().EndEvent -= this.OnLifeSkillCombatEnd;
	}

	// Token: 0x060019D6 RID: 6614 RVA: 0x000A92DF File Offset: 0x000A74DF
	private void RefreshData()
	{
		this._productionPool = null;
		this._artisanOrder = null;
		this.NeedDataListenerId = true;
		ExtraDomainMethod.AsyncCall.GetNpcArtisanOrder(this, this._artisanId, delegate(int offset, RawDataPool pool)
		{
			Serializer.Deserialize(pool, offset, ref this._artisanOrder);
			bool flag = this._artisanOrder == null;
			if (flag)
			{
				this.RefreshCraftTags();
				this.<RefreshData>g__GetPreviewProductionPool|28_2();
			}
			else
			{
				this.RefreshCraftTags();
				this.<RefreshData>g__GetProductionPool|28_1();
				this._craftsManExtraOperatePanel.storeToDropdown.ForceRefreshStorageType(this._artisanOrder.StorageType);
			}
		});
	}

	// Token: 0x060019D7 RID: 6615 RVA: 0x000A9314 File Offset: 0x000A7514
	private void ProcessData()
	{
		bool flag = this._panelMode.HasFlag(EPanelMode.Villager);
		if (flag)
		{
			this._panelMode = EPanelMode.Villager;
			ArtisanOrder artisanOrder = this._artisanOrder;
			int subscriberId = (artisanOrder != null) ? artisanOrder.SubscriberId : -1;
			bool flag2 = subscriberId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			if (flag2)
			{
				this._panelMode |= EPanelMode.TaiwuOrdered;
			}
			else
			{
				bool flag3 = subscriberId > 0;
				if (flag3)
				{
					this._panelMode |= EPanelMode.Intercept;
				}
				else
				{
					this._panelMode |= EPanelMode.NoOrder;
				}
			}
		}
		this.SetupCraftPanel();
		this.SetupOrderPanel();
		this._craftsManExtraOperatePanel.SetProductType(this.CurItemSubType);
	}

	// Token: 0x060019D8 RID: 6616 RVA: 0x000A93C8 File Offset: 0x000A75C8
	private void BindUIElements()
	{
		Refers craftsRootPanel = base.CGet<Refers>("CraftsRootPanel");
		this._craftsManExtraOperatePanel = craftsRootPanel.CGet<CraftsManExtraOperatePanel>("ExtraOperateRefer");
		this._craftsManExtraOperatePanel.Init(this, new Action(this.OnClickBtnProductType), new Action(this.OnClickBtnAddAttainments), new Action<int>(this.OnChangeStock), (int)(-32 + this.LiftSkillType), new Action(this.AddResource), new Action<Transform>(this.OnEnterFocusMode));
		this._villagerCraftPreviewPanel = craftsRootPanel.CGet<VillagerCraftPreviewPanel>("VillagerCraftPreviewPanel");
		this._orderPanelRefer = base.CGet<Refers>("OrderPanelRefer");
		this._villagerCraftPageRefers = base.CGet<Refers>("VillagerCraftPageRefers");
		this._villagerCraftsmanRefer = base.CGet<ResidentView>("VillagerCraftsman");
		this._btnOrder = this._orderPanelRefer.CGet<CButtonObsolete>("BtnOrder");
		this._btnOrder.ClearAndAddListener(new Action(this.OnBtnOrder));
		this._btnNegotiate = this._orderPanelRefer.CGet<CButtonObsolete>("BtnNegotiate");
		this._btnNegotiate.ClearAndAddListener(new Action(this.OnBtnNegotiate));
		this._btnCancel = base.CGet<CButtonObsolete>("ButtonCancel");
		this._btnCancel.ClearAndAddListener(new Action(this.QuickHide));
		CToggleGroupObsolete toggleGroup = this._villagerCraftPageRefers.CGet<CToggleGroupObsolete>("ContentToggleGroup");
		toggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnCraftTagChange);
	}

	// Token: 0x060019D9 RID: 6617 RVA: 0x000A9534 File Offset: 0x000A7734
	private void OnEnterFocusMode(Transform transform)
	{
		Action onClose = new Action(this.OnAddCraftResourceClose);
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("CallTriggerListner", true).SetObject("FocusTransform", transform).SetObject("ArtisanOrder", this._artisanOrder).SetObject("OnClose", onClose).Set("ResourceType", this._productResourceType).Set("BuildingCraftPanel", false);
		UIElement.AddCraftResource.SetOnInitArgs(argumentBox);
		UIManager.Instance.ShowUI(UIElement.AddCraftResource, true);
	}

	// Token: 0x060019DA RID: 6618 RVA: 0x000A95BE File Offset: 0x000A77BE
	private void OnAddCraftResourceClose()
	{
		this._craftsManExtraOperatePanel.CloseFocusModeUI();
		this.RefreshData();
	}

	// Token: 0x060019DB RID: 6619 RVA: 0x000A95D4 File Offset: 0x000A77D4
	private void OnCraftTagChange(CToggleObsolete newTog, CToggleObsolete pre)
	{
		bool flag = pre == newTog;
		if (!flag)
		{
			this._currentCraftType = (ECraftType)newTog.Key;
			this._craftsManExtraOperatePanel.storeToDropdown.MakeItemMethod = (int)(-32 + this.LiftSkillType);
			this.RefreshData();
		}
	}

	// Token: 0x060019DC RID: 6620 RVA: 0x000A9620 File Offset: 0x000A7820
	private void OnBtnNegotiate()
	{
		string title = LocalStringManager.Get(LanguageKey.LK_Craftsman_NegotiatePrice);
		string content = LocalStringManager.Get(LanguageKey.LK_Craftsman_NegotiatePrice_Confirm_Tip);
		CommonUtils.ShowConfirmDialog(title, content, new Action(this.Negotiate), null, EDialogType.None);
	}

	// Token: 0x060019DD RID: 6621 RVA: 0x000A965A File Offset: 0x000A785A
	private void OnBtnOrder()
	{
		this.AddOrder();
	}

	// Token: 0x060019DE RID: 6622 RVA: 0x000A9664 File Offset: 0x000A7864
	private void SetCraftTypesData()
	{
		this._craftTypes.Clear();
		bool flag = this._craftsmanType == 63;
		if (flag)
		{
			this._craftTypes.Add(ECraftType.Forging);
			this._craftTypes.Add(ECraftType.Woodworking);
			this._craftTypes.Add(ECraftType.Weaving);
			this._craftTypes.Add(ECraftType.Jade);
		}
		else
		{
			bool flag2 = this._craftsmanType == 64;
			if (flag2)
			{
				this._craftTypes.Add(ECraftType.Medicine);
				this._craftTypes.Add(ECraftType.Toxicology);
			}
			else
			{
				bool flag3 = this._craftsmanType == 65;
				if (flag3)
				{
					this._craftTypes.Add(ECraftType.Cooking);
				}
				else
				{
					bool flag4 = this._craftsmanType == 66;
					if (!flag4)
					{
						throw new NotImplementedException(string.Format("CraftsmanType {0} is not implemented.", this._craftsmanType));
					}
					this._craftTypes.Add(ECraftType.Tea);
					this._craftTypes.Add(ECraftType.Wine);
				}
			}
		}
		this._currentCraftType = this._craftTypes[0];
		this._productResourceType = UI_CraftsmanPanel.GetResourceTypeByCraftType(this._currentCraftType);
	}

	// Token: 0x060019DF RID: 6623 RVA: 0x000A9780 File Offset: 0x000A7980
	private void OnLifeSkillCombatEnd(bool isTaiwuWin)
	{
		bool flag = !this._startLifeSkillCombat;
		if (!flag)
		{
			this._startLifeSkillCombat = false;
			this._artisanOrder.IsDebateWon = isTaiwuWin;
			ExtraDomainMethod.Call.ArtisanOrderDebate(this._artisanOrder.ArtisanId, isTaiwuWin);
			this.RefreshData();
		}
	}

	// Token: 0x060019E0 RID: 6624 RVA: 0x000A97C9 File Offset: 0x000A79C9
	private void OnConfirmVillagerCraftInputMaterial(ArgumentBox argumentBox)
	{
	}

	// Token: 0x060019E1 RID: 6625 RVA: 0x000A97CC File Offset: 0x000A79CC
	private void OnChangeStock(int stock)
	{
		if (!true)
		{
		}
		ItemSourceType itemSourceType2;
		switch (stock)
		{
		case 0:
			itemSourceType2 = ItemSourceType.Inventory;
			break;
		case 1:
			itemSourceType2 = ItemSourceType.Warehouse;
			break;
		case 2:
			itemSourceType2 = ItemSourceType.Treasury;
			break;
		case 3:
			itemSourceType2 = ItemSourceType.Stock;
			break;
		default:
			throw new NotImplementedException(string.Format("not supported: {0}", stock));
		}
		if (!true)
		{
		}
		ItemSourceType itemSourceType = itemSourceType2;
		ExtraDomainMethod.Call.SetArtisanOrderStorageType(this._artisanOrder, itemSourceType);
		this.RefreshData();
	}

	// Token: 0x060019E2 RID: 6626 RVA: 0x000A9838 File Offset: 0x000A7A38
	private void AddResource()
	{
		List<ItemKey> inventoryItems;
		this._craftsManExtraOperatePanel.PutResourceDic.TryGetValue(1, out inventoryItems);
		List<ItemKey> warehouseItems;
		this._craftsManExtraOperatePanel.PutResourceDic.TryGetValue(2, out warehouseItems);
		List<ItemKey> resourcesItems;
		this._craftsManExtraOperatePanel.PutResourceDic.TryGetValue(9, out resourcesItems);
		SingletonObject.getInstance<BuildingModel>().RefreshResources();
		this.RefreshData();
	}

	// Token: 0x060019E3 RID: 6627 RVA: 0x000A9895 File Offset: 0x000A7A95
	private void Negotiate()
	{
		this._startLifeSkillCombat = true;
		SingletonObject.getInstance<LifeSkillCombatModel>().StartLifeSkillCombat(this._artisanOrder.ArtisanId, this.LiftSkillType, false);
	}

	// Token: 0x060019E4 RID: 6628 RVA: 0x000A98BC File Offset: 0x000A7ABC
	private void AddOrder()
	{
		int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		bool flag = this._panelMode.HasFlag(EPanelMode.Intercept);
		if (flag)
		{
			ExtraDomainMethod.Call.InterceptArtisanOrder(this._artisanOrder.ArtisanId, taiwuCharId, this._artisanOrder.IsDebateWon);
			this.RefreshData();
		}
		else
		{
			bool flag2 = this._panelMode.HasFlag(EPanelMode.NoOrder);
			if (flag2)
			{
				ExtraDomainMethod.Call.CreateArtisanOrder(this._artisanId, taiwuCharId, this.LiftSkillType);
				this.RefreshData();
			}
		}
	}

	// Token: 0x060019E5 RID: 6629 RVA: 0x000A9950 File Offset: 0x000A7B50
	private void OnClickBtnProductType()
	{
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>().SetObject("ArtisanOrder", this._artisanOrder).SetObject("OnConfirm", new Action(this.<OnClickBtnProductType>g__OnConfirm|43_0));
		UIElement.SelectProductType.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.SelectProductType, true);
	}

	// Token: 0x060019E6 RID: 6630 RVA: 0x000A99AC File Offset: 0x000A7BAC
	private void OnClickBtnAddAttainments()
	{
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>().SetObject("ArtisanOrder", this._artisanOrder).SetObject("ProductionPool", this._productionPool).SetObject("CurrentCraftType", this._currentCraftType).Set("ItemSubType", this.CurItemSubType);
		UIElement.VillagerCraftInputMaterial.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.VillagerCraftInputMaterial, true);
	}

	// Token: 0x060019E7 RID: 6631 RVA: 0x000A9A24 File Offset: 0x000A7C24
	private void SetupOrderPanel()
	{
		bool flag = !this._orderPanelRefer.gameObject.activeSelf;
		if (!flag)
		{
			ArtisanOrder artisanOrder = this._artisanOrder;
			bool isDebateWon = artisanOrder != null && artisanOrder.IsDebateWon;
			ArtisanOrder artisanOrder2 = this._artisanOrder;
			int debateCount = (artisanOrder2 != null) ? artisanOrder2.DebateCount : 0;
			this._orderPanelRefer.CGet<LifeSkillBattleNeed>("LifeSkillBattleNeed").SetInfo(this.LiftSkillType);
			CostResource costResource = this._orderPanelRefer.CGet<CostResource>("CostResource");
			bool isIntercept = this._panelMode.HasFlag(EPanelMode.Intercept);
			string btnContent = isIntercept ? LocalStringManager.Get(LanguageKey.LK_Craftsman_InterceptOrder) : LocalStringManager.Get(LanguageKey.LK_Craftsman_OrderProduct);
			this._btnOrder.GetComponent<Refers>().CGet<TextMeshProUGUI>("NormalLabel").text = btnContent;
			this._btnOrder.GetComponent<Refers>().CGet<TextMeshProUGUI>("DisableLabel").text = btnContent;
			ArtisanOrder artisanOrder3 = this._artisanOrder;
			sbyte orderLifeSkillType = (artisanOrder3 != null) ? artisanOrder3.LifeSkillType : -1;
			string lifeSkillTypeName = (orderLifeSkillType >= 0) ? LifeSkillType.Instance[orderLifeSkillType].Name : string.Empty;
			bool isSameResourceType = this.LiftSkillType == orderLifeSkillType || orderLifeSkillType < 0;
			GameObject orderTipBg = this._orderPanelRefer.CGet<GameObject>("OrderTipBg");
			TextMeshProUGUI orderTip = this._orderPanelRefer.CGet<TextMeshProUGUI>("OrderTip");
			bool flag2 = this._panelMode.HasFlag(EPanelMode.TaiwuOrdered);
			if (flag2)
			{
				this._btnOrder.interactable = false;
				costResource.gameObject.SetActive(false);
				orderTipBg.SetActive(!isSameResourceType);
				orderTip.text = LocalStringManager.GetFormat(LanguageKey.LK_Craftsman_Ordered_Tip, lifeSkillTypeName).ColorReplace();
			}
			else
			{
				int num;
				int price = (this._productionPool == null) ? 0 : (isIntercept ? this._productionPool.GetInterceptOrderPrice(isDebateWon, out num) : this._productionPool.GetCreateOrderPrice(out num));
				int money = SingletonObject.getInstance<BuildingModel>().GetResourceCount(6);
				costResource.SetInfo(6, price, money);
				costResource.gameObject.SetActive(true);
				this._btnOrder.interactable = (isSameResourceType && price > 0 && money >= price);
				string content = isSameResourceType ? (isIntercept ? LocalStringManager.Get(LanguageKey.LK_Craftsman_InterceptOrder_Tip) : LocalStringManager.Get(LanguageKey.LK_Craftsman_OrderProduct_Tip)) : LocalStringManager.GetFormat(LanguageKey.LK_Craftsman_Ordered_Tip, lifeSkillTypeName);
				orderTip.text = content.ColorReplace();
				orderTipBg.SetActive(true);
			}
			this._btnNegotiate.gameObject.SetActive(this._panelMode.HasFlag(EPanelMode.Intercept) && debateCount == 0 && isSameResourceType);
			SelectableCharacter characterAvatar = this._orderPanelRefer.CGet<SelectableCharacter>("SelectableCharacter");
			bool flag3 = this._artisanOrder != null && this._artisanOrder.SubscriberId > 0;
			if (flag3)
			{
				characterAvatar.CharacterId = this._artisanOrder.SubscriberId;
				characterAvatar.gameObject.SetActive(true);
				CButtonObsolete btn = characterAvatar.GetComponent<Refers>().CGet<CButtonObsolete>("BtnOrderer");
				btn.onClick.RemoveAllListeners();
				btn.onClick.AddListener(new UnityAction(this.OpenOrderCharacterInfo));
			}
			else
			{
				characterAvatar.gameObject.SetActive(false);
			}
			this._villagerCraftsmanRefer.RenderCraftsman(this._artisanId, this.LiftSkillType);
			this._craftsManExtraOperatePanel.storeToDropdown.MakeItemMethod = (int)(-32 + this.LiftSkillType);
			this._orderPanelRefer.CGet<TextMeshProUGUI>("teacherTitle").text = UI_CraftsmanPanel.GetLeaderNameByCraftType(this._currentCraftType);
		}
	}

	// Token: 0x060019E8 RID: 6632 RVA: 0x000A9DC0 File Offset: 0x000A7FC0
	private void OpenOrderCharacterInfo()
	{
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("CharacterId", this._artisanOrder.SubscriberId);
		argBox.SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.CharacterBase, ECharacterSubPage.Character));
		UIElement.CharacterMenu.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
	}

	// Token: 0x060019E9 RID: 6633 RVA: 0x000A9E20 File Offset: 0x000A8020
	private void SetupVillagerCraftTagPanel()
	{
		bool flag = (this._panelMode & EPanelMode.Building) == EPanelMode.Building;
		if (flag)
		{
			this._villagerCraftPageRefers.gameObject.SetActive(false);
		}
		else
		{
			this._villagerCraftPageRefers.gameObject.SetActive(true);
			CToggleGroupObsolete toggleGroup = this._villagerCraftPageRefers.CGet<CToggleGroupObsolete>("ContentToggleGroup");
			Transform parent = toggleGroup.transform;
			toggleGroup.Clear();
			CommonUtils.PrepareEnoughChildren(parent, this._villagerCraftPageRefers.CGet<Refers>("CraftTagTemplate").gameObject, this._craftTypes.Count, null);
			for (int i = 0; i < this._craftTypes.Count; i++)
			{
				Refers temp = parent.GetChild(i).GetComponent<Refers>();
				temp.gameObject.SetActive(true);
				temp.UserInt = (int)this._craftTypes[i];
				temp.CGet<TextMeshProUGUI>("CraftTypeName").text = CommonUtils.GetCraftTypesNameByEnum(this._craftTypes[i]);
				CImage imgComp = temp.CGet<CImage>("CraftTagIcon");
				CImage imgCompDisable = temp.CGet<CImage>("CraftTagIconDisable");
				imgComp.SetSprite(CommonUtils.GetCraftTypeIconByEnum(this._craftTypes[i]), false, null);
				imgCompDisable.SetSprite(CommonUtils.GetCraftTypeIconDisableByEnum(this._craftTypes[i]), false, null);
				CToggleObsolete tog = temp.GetComponent<CToggleObsolete>();
				tog.Key = (int)this._craftTypes[i];
				toggleGroup.Add(tog);
			}
			toggleGroup.SetWithoutNotify((int)this._craftTypes[0], true);
			this._villagerCraftPageRefers.CGet<TextMeshProUGUI>("TitleText").text = UI_CraftsmanPanel.GetCraftsmanTitleByCraftType(this._craftsmanType);
		}
	}

	// Token: 0x060019EA RID: 6634 RVA: 0x000A9FE4 File Offset: 0x000A81E4
	private void RefreshCraftTags()
	{
		CToggleGroupObsolete toggleGroup = this._villagerCraftPageRefers.CGet<CToggleGroupObsolete>("ContentToggleGroup");
		Transform parent = toggleGroup.transform;
		ArtisanOrder artisanOrder = this._artisanOrder;
		sbyte orderLifeSkillType = (artisanOrder != null) ? artisanOrder.LifeSkillType : -1;
		bool flag = this.LiftSkillType == orderLifeSkillType || orderLifeSkillType < 0;
		bool flag2 = this._artisanOrder != null && this._artisanOrder.SubscriberId > 0 && this._artisanOrder.LifeSkillType >= 0;
		if (flag2)
		{
			for (int i = 0; i < this._craftTypes.Count; i++)
			{
				Refers refers = parent.GetChild(i).GetComponent<Refers>();
				bool flag3 = UI_CraftsmanPanel.GetOperationNeedSkillType(this._craftTypes[i]) == this._artisanOrder.LifeSkillType;
				if (flag3)
				{
					this._currentCraftType = this._craftTypes[i];
					refers.CGet<CToggleObsolete>("CraftTagToggle").interactable = true;
					refers.CGet<CImage>("CraftTagIcon").gameObject.SetActive(true);
					refers.CGet<CImage>("CraftTagIconDisable").gameObject.SetActive(false);
					refers.CGet<DisableStyleRoot>("Normal").SetStyleEffect(false, false);
				}
				else
				{
					refers.CGet<CToggleObsolete>("CraftTagToggle").interactable = false;
					refers.CGet<CImage>("CraftTagIcon").gameObject.SetActive(false);
					refers.CGet<CImage>("CraftTagIconDisable").gameObject.SetActive(true);
					refers.CGet<DisableStyleRoot>("Normal").SetStyleEffect(true, false);
				}
			}
		}
		else
		{
			for (int j = 0; j < this._craftTypes.Count; j++)
			{
				Refers refers2 = parent.GetChild(j).GetComponent<Refers>();
				refers2.CGet<CToggleObsolete>("CraftTagToggle").interactable = true;
				refers2.CGet<CImage>("CraftTagIcon").gameObject.SetActive(true);
				refers2.CGet<CImage>("CraftTagIconDisable").gameObject.SetActive(false);
				refers2.CGet<DisableStyleRoot>("Normal").SetStyleEffect(false, false);
			}
		}
		toggleGroup.SetWithoutNotify((int)this._currentCraftType, true);
	}

	// Token: 0x060019EB RID: 6635 RVA: 0x000AA228 File Offset: 0x000A8428
	private void InitCraftTip()
	{
		bool flag = this._panelMode.HasFlag(EPanelMode.NoOrder);
		if (flag)
		{
			this._villagerCraftPreviewPanel.ShowTip(LanguageKey.LK_VillagerCraftPreviewPanel_Tip_NoOrder);
		}
		else
		{
			this._villagerCraftPreviewPanel.ShowTip(LanguageKey.LK_VillagerCraftPreviewPanel_Tip_Normal);
		}
	}

	// Token: 0x060019EC RID: 6636 RVA: 0x000AA278 File Offset: 0x000A8478
	private void SetupCraftPanel()
	{
		this._villagerCraftPreviewPanel.SetProductionPool(this._productionPool, null, this.CurItemSubType);
		this._craftsManExtraOperatePanel.Refresh(this._artisanOrder, (short)this.LiftSkillType, this.ResourceCount, this._panelMode, this._currentCraftType, false, -1);
	}

	// Token: 0x060019ED RID: 6637 RVA: 0x000AA2CC File Offset: 0x000A84CC
	public static sbyte GetOperationNeedSkillType(ECraftType craftType)
	{
		sbyte result;
		switch (craftType)
		{
		case ECraftType.Tea:
			result = 5;
			break;
		case ECraftType.Wine:
			result = 5;
			break;
		case ECraftType.Forging:
			result = 6;
			break;
		case ECraftType.Woodworking:
			result = 7;
			break;
		case ECraftType.Weaving:
			result = 10;
			break;
		case ECraftType.Medicine:
			result = 8;
			break;
		case ECraftType.Toxicology:
			result = 9;
			break;
		case ECraftType.Cooking:
			result = 14;
			break;
		case ECraftType.Jade:
			result = 11;
			break;
		default:
			result = -1;
			break;
		}
		return result;
	}

	// Token: 0x060019EE RID: 6638 RVA: 0x000AA33C File Offset: 0x000A853C
	public override void QuickHide()
	{
		bool focusMode = this._craftsManExtraOperatePanel.CraftsManAddResourcePanel.FocusMode;
		if (focusMode)
		{
			this._craftsManExtraOperatePanel.CraftsManAddResourcePanel.SetAddResourceFocusMode(false);
		}
		else
		{
			base.QuickHide();
			TaiwuEventDomainMethod.Call.TriggerListener("FinishCraftsmanPanelUI", true);
		}
	}

	// Token: 0x060019EF RID: 6639 RVA: 0x000AA388 File Offset: 0x000A8588
	public static sbyte GetResourceTypeByCraftType(ECraftType craftType)
	{
		if (!true)
		{
		}
		sbyte result;
		switch (craftType)
		{
		case ECraftType.None:
			result = -1;
			break;
		case ECraftType.Tea:
			result = 6;
			break;
		case ECraftType.Wine:
			result = 6;
			break;
		case ECraftType.Forging:
			result = 2;
			break;
		case ECraftType.Woodworking:
			result = 1;
			break;
		case ECraftType.Weaving:
			result = 4;
			break;
		case ECraftType.Medicine:
			result = 5;
			break;
		case ECraftType.Toxicology:
			result = 5;
			break;
		case ECraftType.Cooking:
			result = 0;
			break;
		case ECraftType.Jade:
			result = 3;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x060019F0 RID: 6640 RVA: 0x000AA404 File Offset: 0x000A8604
	public static string GetLeaderNameByCraftType(ECraftType craftType)
	{
		if (!true)
		{
		}
		short num;
		switch (craftType)
		{
		case ECraftType.None:
			num = -1;
			break;
		case ECraftType.Tea:
			num = 127;
			break;
		case ECraftType.Wine:
			num = 128;
			break;
		case ECraftType.Forging:
			num = 129;
			break;
		case ECraftType.Woodworking:
			num = 139;
			break;
		case ECraftType.Weaving:
			num = 169;
			break;
		case ECraftType.Medicine:
			num = 149;
			break;
		case ECraftType.Toxicology:
			num = 159;
			break;
		case ECraftType.Cooking:
			num = 203;
			break;
		case ECraftType.Jade:
			num = 179;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (!true)
		{
		}
		short buildingBlockTemplateId = num;
		BuildingBlockItem config = BuildingBlock.Instance.GetItem(buildingBlockTemplateId);
		return (config != null) ? config.LeaderName : null;
	}

	// Token: 0x060019F1 RID: 6641 RVA: 0x000AA4B8 File Offset: 0x000A86B8
	private static string GetCraftsmanTitleByCraftType(sbyte craftsmanType)
	{
		if (!true)
		{
		}
		LanguageKey languageKey;
		switch (craftsmanType)
		{
		case 63:
			languageKey = LanguageKey.LK_Craftsman_CraftsmanTitle;
			break;
		case 64:
			languageKey = LanguageKey.LK_Craftsman_DoctorTitle;
			break;
		case 65:
			languageKey = LanguageKey.LK_Craftsman_PeasantTitle;
			break;
		case 66:
			languageKey = LanguageKey.LK_Craftsman_LiteratiTitle;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (!true)
		{
		}
		LanguageKey lStringKey = languageKey;
		return LocalStringManager.Get(lStringKey);
	}

	// Token: 0x060019F4 RID: 6644 RVA: 0x000AA598 File Offset: 0x000A8798
	[CompilerGenerated]
	private void <RefreshData>g__GetProductionPool|28_1()
	{
		bool flag = this._artisanOrder == null;
		if (flag)
		{
			this._productionPool = null;
			this.ProcessData();
			this.Element.ShowAfterRefresh();
		}
		else
		{
			ExtraDomainMethod.AsyncCall.GetArtisanOrderProductionPool(this, this._artisanOrder, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._productionPool);
				this.ProcessData();
				this.Element.ShowAfterRefresh();
			});
		}
	}

	// Token: 0x060019F6 RID: 6646 RVA: 0x000AA610 File Offset: 0x000A8810
	[CompilerGenerated]
	private void <RefreshData>g__GetPreviewProductionPool|28_2()
	{
		ExtraDomainMethod.AsyncCall.GetProductionPoolPreview(this, this._artisanId, this.LiftSkillType, delegate(int offset, RawDataPool pool)
		{
			Serializer.Deserialize(pool, offset, ref this._productionPool);
			this.ProcessData();
			this.Element.ShowAfterRefresh();
		});
	}

	// Token: 0x060019F8 RID: 6648 RVA: 0x000AA656 File Offset: 0x000A8856
	[CompilerGenerated]
	private void <OnClickBtnProductType>g__OnConfirm|43_0()
	{
		ExtraDomainMethod.Call.SetArtisanOrderProductionType(this._artisanOrder, this._artisanOrder.ItemSubType);
		this.RefreshData();
	}

	// Token: 0x04001462 RID: 5218
	private CraftsManExtraOperatePanel _craftsManExtraOperatePanel;

	// Token: 0x04001463 RID: 5219
	private Refers _orderPanelRefer;

	// Token: 0x04001464 RID: 5220
	private Refers _villagerCraftPageRefers;

	// Token: 0x04001465 RID: 5221
	private ResidentView _villagerCraftsmanRefer;

	// Token: 0x04001466 RID: 5222
	private VillagerCraftPreviewPanel _villagerCraftPreviewPanel;

	// Token: 0x04001467 RID: 5223
	private CButtonObsolete _btnOrder;

	// Token: 0x04001468 RID: 5224
	private CButtonObsolete _btnNegotiate;

	// Token: 0x04001469 RID: 5225
	private CButtonObsolete _btnCancel;

	// Token: 0x0400146A RID: 5226
	private EPanelMode _panelMode;

	// Token: 0x0400146B RID: 5227
	private sbyte _productResourceType;

	// Token: 0x0400146C RID: 5228
	private ProductionPool _productionPool;

	// Token: 0x0400146D RID: 5229
	private ArtisanOrder _artisanOrder;

	// Token: 0x0400146E RID: 5230
	private int _artisanId;

	// Token: 0x0400146F RID: 5231
	private sbyte _craftsmanType;

	// Token: 0x04001470 RID: 5232
	private ECraftType _currentCraftType;

	// Token: 0x04001471 RID: 5233
	private readonly List<ECraftType> _craftTypes = new List<ECraftType>();

	// Token: 0x04001472 RID: 5234
	private bool _startLifeSkillCombat;
}
