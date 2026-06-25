using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Config;
using EasyButtons;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Components.Common;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter.Item.Apply;
using Game.Views.MapBlockCharList;
using GameData.Domains.Character.Display;
using GameData.Domains.Information;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Merchant;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Domains.Taiwu.ExchangeSystem;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Exchange
{
	// Token: 0x02000A34 RID: 2612
	public class ViewShopGift : ViewExchangeBase
	{
		// Token: 0x060080A7 RID: 32935 RVA: 0x003BD956 File Offset: 0x003BBB56
		[Button("Transfer")]
		private void Transfer(MapBlockCharStat character)
		{
			FieldInfo field = character.GetType().GetField("merchantSprites", (BindingFlags)(-1));
			this.merchantSprites = (Sprite[])((field != null) ? field.GetValue(character) : null);
		}

		// Token: 0x17000E0F RID: 3599
		// (get) Token: 0x060080A8 RID: 32936 RVA: 0x003BD982 File Offset: 0x003BBB82
		protected override string SortSaveKeyPrefix
		{
			get
			{
				return "ViewShop";
			}
		}

		// Token: 0x17000E10 RID: 3600
		// (get) Token: 0x060080A9 RID: 32937 RVA: 0x003BD989 File Offset: 0x003BBB89
		public override Exchange Exchange
		{
			get
			{
				ShopDisplayData displayData = this._displayData;
				return (displayData != null) ? displayData.Exchange : null;
			}
		}

		// Token: 0x17000E11 RID: 3601
		// (get) Token: 0x060080AA RID: 32938 RVA: 0x003BD99D File Offset: 0x003BBB9D
		public override SecretInformationDisplayPackage SecretInformationDisplayPackage
		{
			get
			{
				return new SecretInformationDisplayPackage();
			}
		}

		// Token: 0x17000E12 RID: 3602
		// (get) Token: 0x060080AB RID: 32939 RVA: 0x003BD9A4 File Offset: 0x003BBBA4
		public override CharacterDisplayData TargetDisplayData
		{
			get
			{
				ShopDisplayData displayData = this._displayData;
				return (displayData != null) ? displayData.TargetCharacterDisplayData : null;
			}
		}

		// Token: 0x17000E13 RID: 3603
		// (get) Token: 0x060080AC RID: 32940 RVA: 0x003BD9B8 File Offset: 0x003BBBB8
		public override CharacterDisplayData TaiwuDisplayData
		{
			get
			{
				ShopDisplayData displayData = this._displayData;
				return (displayData != null) ? displayData.TaiwuDisplayData : null;
			}
		}

		// Token: 0x060080AD RID: 32941 RVA: 0x003BD9CC File Offset: 0x003BBBCC
		public override IReadOnlyList<ITradeableContent> GetSelfTradeableList(int index)
		{
			if (!true)
			{
			}
			IEnumerable<ITradeableContent> enumerable;
			switch (index)
			{
			case 0:
				enumerable = this._displayData.TaiwuInventoryItemDisplayDataList;
				break;
			case 1:
				enumerable = this._displayData.TaiwuWarehouseItemDisplayDataList;
				break;
			case 2:
				enumerable = this._displayData.TaiwuTreasuryItemDisplayDataList;
				break;
			case 3:
				enumerable = this._displayData.TaiwuStockItemDisplayDataList;
				break;
			default:
				enumerable = Enumerable.Empty<ITradeableContent>();
				break;
			}
			if (!true)
			{
			}
			return ((enumerable != null) ? (from x in enumerable
			where this.CheckItemCanTransfer(x, this._displayData.TaiwuDisplayData)
			select x).ToArray<ITradeableContent>() : null) ?? Array.Empty<ITradeableContent>();
		}

		// Token: 0x060080AE RID: 32942 RVA: 0x003BDA60 File Offset: 0x003BBC60
		protected override bool CheckItemCanTransfer(ITradeableContent itemData, CharacterDisplayData cd)
		{
			ItemKey key = itemData.Key;
			return (key.ItemType == 12 && key.TemplateId == 380) || base.CheckItemCanTransfer(itemData, cd);
		}

		// Token: 0x060080AF RID: 32943 RVA: 0x003BDA96 File Offset: 0x003BBC96
		public override IReadOnlyList<ITradeableContent> GetTargetTradeableList(int index)
		{
			return (index < 0 || index > 7) ? this._displayData[7] : this._displayData[index];
		}

		// Token: 0x17000E14 RID: 3604
		// (get) Token: 0x060080B0 RID: 32944 RVA: 0x003BDABA File Offset: 0x003BBCBA
		protected override ESortAndFilterControllerType ControllerType
		{
			get
			{
				return ESortAndFilterControllerType.Shop;
			}
		}

		// Token: 0x17000E15 RID: 3605
		// (get) Token: 0x060080B1 RID: 32945 RVA: 0x003BDABD File Offset: 0x003BBCBD
		protected override ItemListScroll.EColumnType ColumnType
		{
			get
			{
				return ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Durability | ItemListScroll.EColumnType.ShopPrice;
			}
		}

		// Token: 0x060080B2 RID: 32946 RVA: 0x003BDAC8 File Offset: 0x003BBCC8
		protected override void Awake()
		{
			this.ConfirmOnHideKey = LanguageKey.LK_Exchange_Shop_Title;
			base.Awake();
			this.exchangeContainer.exchangeBack.SetMerchant(1);
			this.exchangeContainer.exchangeFront.SetMerchant(1);
			this.merchantInfo.onClick.ResetListener(new Action(this.MerchantInfo));
			this.exchangeContainer.targetPage.OnActiveIndexChange += delegate(int newTog, int _)
			{
				this.CheckHasExtraGoods(newTog);
			};
			this.exchangeContainer.currPage.OnActiveIndexChange += delegate(int newTog, int oldTog)
			{
				this.SetTitleTaiwu(newTog);
			};
		}

		// Token: 0x060080B3 RID: 32947 RVA: 0x003BDB63 File Offset: 0x003BBD63
		private void CheckHasExtraGoods(int newTog)
		{
		}

		// Token: 0x060080B4 RID: 32948 RVA: 0x003BDB68 File Offset: 0x003BBD68
		private void SetTitleTaiwu(int newTog)
		{
			switch (newTog)
			{
			case 0:
				this.exchangeContainer.titleTaiwu.text = LanguageKey.LK_SelectItem_Group_Inventory.Tr();
				break;
			case 1:
				this.exchangeContainer.titleTaiwu.text = LanguageKey.LK_SelectItem_Group_Warehouse.Tr();
				break;
			case 2:
				this.exchangeContainer.titleTaiwu.text = LanguageKey.LK_SelectItem_Group_Treasury.Tr();
				break;
			case 3:
				this.exchangeContainer.titleTaiwu.text = LanguageKey.LK_Exchange_Stock.Tr();
				break;
			}
		}

		// Token: 0x060080B5 RID: 32949 RVA: 0x003BDC06 File Offset: 0x003BBE06
		protected override void AwakeAvatarBtn()
		{
			this.exchangeContainer.avatarBtn.interactable = true;
			this.exchangeContainer.avatarBtn.onClick.ResetListener(new Action(this.AvatarClicked));
		}

		// Token: 0x060080B6 RID: 32950 RVA: 0x003BDC3D File Offset: 0x003BBE3D
		private void Update()
		{
			this.UpdateDialog();
		}

		// Token: 0x060080B7 RID: 32951 RVA: 0x003BDC47 File Offset: 0x003BBE47
		public override void Submit()
		{
			this.SubmitImpl();
		}

		// Token: 0x060080B8 RID: 32952 RVA: 0x003BDC51 File Offset: 0x003BBE51
		public void SubmitImpl()
		{
			TaiwuDomainMethod.Call.ConfirmShopExchange(this._displayData.Exchange);
			Action<ShopExchange> callback = this._callback;
			if (callback != null)
			{
				callback(this._displayData.Exchange);
			}
			this.QuickHideImpl();
		}

		// Token: 0x060080B9 RID: 32953 RVA: 0x003BDC8C File Offset: 0x003BBE8C
		public void MerchantInfo()
		{
			UIElement.MerchantInfo.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("MerchantType", this._displayData.Exchange.MerchantData.MerchantType).Set("MerchantFavorability", this._displayData.Exchange.MerchantFavor).Set("MerchantTemplateId", this._displayData.Exchange.MerchantData.MerchantTemplateId).Set("Toggle", ViewMerchantInfo.GetToggle(this._displayData.Exchange.TradeArguments.OpenShopEventArguments)));
			UIManager.Instance.MaskUI(UIElement.MerchantInfo);
		}

		// Token: 0x060080BA RID: 32954 RVA: 0x003BDD38 File Offset: 0x003BBF38
		public override void OnInit(ArgumentBox argsBox)
		{
			this._ab = argsBox;
			this._init = false;
			this._displayData = null;
			if (argsBox != null)
			{
				argsBox.Get<OpenShopEventArguments>("OpenShopEventArguments", out this._openShopEventArguments);
			}
			if (argsBox != null)
			{
				argsBox.Get<Action<ShopExchange>>("callback", out this._callback);
			}
			base.OnInit(argsBox);
			this.exchangeContainer.confirmButtonText.text = LanguageKey.LK_Exchange_Confirm_As_Gift.Tr();
			this.exchangeContainer.AddSwitchToggleListener();
			this.refreshGoods.gameObject.SetActive(false);
			this.exchangeContainer.currPage.Get(3).gameObject.SetActive(false);
		}

		// Token: 0x060080BB RID: 32955 RVA: 0x003BDDE7 File Offset: 0x003BBFE7
		protected override void ResetDialog()
		{
			this._timer = 0f;
			this._onlyIntroduceDialog = false;
			this._firstIntroduceDialog = false;
			this._secondIntroduceDialog = false;
			this._remainDialog = false;
			this._favorDialog = false;
			this._seasonDialog = false;
			base.ResetDialog();
		}

		// Token: 0x060080BC RID: 32956 RVA: 0x003BDE26 File Offset: 0x003BC026
		protected override void RequestData()
		{
			TaiwuDomainMethod.AsyncCall.GetShopDisplayData(this, this._openShopEventArguments, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._displayData);
				this._displayData.Exchange.IsGift = true;
				ShopDisplayData displayData = this._displayData;
				this.ExchangeValueType = ((displayData != null && displayData.Exchange.IsAreaDebtShop) ? 10 : 6);
				RowItemMain.MinDebtLevel = this._displayData.Exchange.MinDebtLevel;
				RowItemMain.MaxDebtLevel = this._displayData.Exchange.MaxDebtLevel;
				MerchantItem merchantCfg = this._displayData.Exchange.MerchantData.MerchantConfig;
				bool flag = !this._init;
				if (flag)
				{
					bool flag2 = merchantCfg.TemplateId == 53;
					if (flag2)
					{
						this.exchangeContainer.targetItemList.SortAndFilterController.SetToggleVisible(0, 6);
						this.exchangeContainer.targetItemList.SortAndFilterController.SetToggleIsOn(0, 6);
					}
					else
					{
						bool flag3 = merchantCfg.TemplateId == 52;
						if (flag3)
						{
							this.exchangeContainer.targetItemList.SortAndFilterController.SetToggleVisible(0, 0);
							this.exchangeContainer.targetItemList.SortAndFilterController.SetToggleIsOn(0, 0);
						}
						else
						{
							this.exchangeContainer.targetItemList.SortAndFilterController.SetToggleVisible(0, -1);
							this.exchangeContainer.targetItemList.SortAndFilterController.SetToggleIsOn(0, -1);
						}
					}
					CButton putSelfAll = this.exchangeContainer.putSelfAll;
					if (putSelfAll != null)
					{
						putSelfAll.gameObject.SetActive(merchantCfg.TemplateId != 53);
					}
				}
				MerchantTypeItem merchantTypeItem = Config.MerchantType.Instance[(merchantCfg != null) ? merchantCfg.MerchantType : -1];
				string typeName = ((merchantTypeItem != null) ? merchantTypeItem.Name : null) ?? "";
				bool flag4 = this.merchantSprites.CheckIndex((int)this._displayData.Exchange.MerchantData.MerchantTemplateId);
				if (flag4)
				{
					Sprite icon = this.merchantSprites[(int)this._displayData.Exchange.MerchantData.MerchantTemplateId];
					this.merchantType.Set(icon, string.Empty, typeName, null, false);
				}
				else
				{
					this.merchantContainer.SetActive(false);
				}
				sbyte? b = (merchantCfg != null) ? new sbyte?(merchantCfg.MerchantType) : null;
				bool flag5;
				if (b != null)
				{
					sbyte valueOrDefault = b.GetValueOrDefault();
					if (valueOrDefault >= 0)
					{
						flag5 = (valueOrDefault <= 6);
						goto IL_259;
					}
				}
				flag5 = false;
				IL_259:
				bool flag6 = flag5;
				if (flag6)
				{
					this.exchangeContainer.exchangeBack.SetMerchant((int)merchantCfg.MerchantType);
					this.exchangeContainer.exchangeFront.SetMerchant((int)merchantCfg.MerchantType);
				}
				else
				{
					this.exchangeContainer.exchangeBack.SetTreasury(0);
					this.exchangeContainer.exchangeFront.SetTreasury(0);
				}
				foreach (CImage icon2 in this.moneyIcons)
				{
					icon2.sprite = ((merchantCfg != null && merchantCfg.MerchantType == 8) ? this.debt : this.money);
				}
				GameObject gameObject = this.merchantInfo.gameObject;
				bool active;
				if (merchantCfg != null)
				{
					sbyte valueOrDefault = merchantCfg.MerchantType;
					if (valueOrDefault >= 0)
					{
						active = (valueOrDefault <= 6);
						goto IL_322;
					}
				}
				active = false;
				IL_322:
				gameObject.SetActive(active);
				this.SetTargetCharacterDisplayData(this._displayData.TargetCharacterDisplayData, LanguageKey.LK_Exchange_SubTitle_Shop);
				sbyte b2 = (this._displayData.Exchange.MerchantData.MerchantTemplateId < 7) ? (this._displayData.Exchange.MerchantData.MerchantTemplateId + 1) : (this._displayData.Exchange.MerchantData.MerchantTemplateId % 7 + 1);
				this.exchangeContainer.titleTarget.text = this._displayData.Exchange.MerchantData.MerchantConfig.UiName;
				bool showOrg = this._displayData.TargetCharacterDisplayData != null && this._displayData.TargetCharacterDisplayData.OrgInfo.OrgTemplateId >= 0;
				this.identity.gameObject.SetActive(showOrg);
				this.identity.Set(this._displayData.TargetCharacterDisplayData, false, true, true);
				this.exchangeContainer.titleTaiwu.text = SingletonObject.getInstance<BasicGameData>().TaiwuMonasticTitleOrDisplayName;
				this.SetTitleTaiwu(this.exchangeContainer.currPage.GetActiveIndex());
				this.SetWarehouseCanInteract(this._displayData.CanTransferItemToWarehouse);
				this.SetExchangeData();
				this.Refresh();
				this.CheckHasExtraGoods(this.exchangeContainer.targetPage.GetActiveIndex());
				bool init = this._init;
				if (!init)
				{
					this._init = true;
					ValueTuple<CToggle, int> valueTuple = this.exchangeContainer.targetPage.GetAll().Select((CToggle n, int i) => new ValueTuple<CToggle, int>(n, i)).Where(delegate([TupleElementNames(new string[]
					{
						"n",
						"i"
					})] ValueTuple<CToggle, int> n)
					{
						ShopToggle component = n.Item1.GetComponent<ShopToggle>();
						return component != null && component.IsUnlock;
					}).FirstOrDefault<ValueTuple<CToggle, int>>();
					int k;
					bool flag7;
					if (valueTuple.Item1 != null)
					{
						k = valueTuple.Item2;
						flag7 = true;
					}
					else
					{
						flag7 = false;
					}
					bool flag8 = flag7;
					if (flag8)
					{
						this.exchangeContainer.targetPage.Set(k, false);
					}
					this.Element.ShowAfterRefresh();
				}
			});
		}

		// Token: 0x060080BD RID: 32957 RVA: 0x003BDE44 File Offset: 0x003BC044
		protected override void SetTargetCharacterDisplayData(CharacterDisplayData displayData, LanguageKey targetTitle)
		{
			bool flag;
			if (displayData != null)
			{
				short templateId = displayData.TemplateId;
				flag = (templateId >= 959 && templateId <= 965);
			}
			else
			{
				flag = true;
			}
			bool isCaravanAvatar = flag;
			bool flag2 = isCaravanAvatar;
			if (flag2)
			{
				this.exchangeContainer.avatar.RefreshAsSpine(this._displayData.Exchange.TradeArguments.OpenShopEventArguments.IsHeadBuildingMerchant ? Config.MerchantType.Instance[this._displayData.Exchange.MerchantData.MerchantType].BuildingSpineName : Config.MerchantType.Instance[this._displayData.Exchange.MerchantData.MerchantType].CaravanSpineName, null);
				this.exchangeContainer.nameTarget.text = this._displayData.Exchange.MerchantData.MerchantConfig.UiName;
			}
			else
			{
				base.SetTargetCharacterDisplayData(displayData, targetTitle);
			}
		}

		// Token: 0x060080BE RID: 32958 RVA: 0x003BDF2E File Offset: 0x003BC12E
		protected override void RefreshTargetItems(int index)
		{
			base.RefreshTargetItems(index);
			this.bar.Set(this._displayData.Exchange.MerchantFavor, this._displayData.Exchange.FavorabilityWithDelta);
		}

		// Token: 0x060080BF RID: 32959 RVA: 0x003BDF68 File Offset: 0x003BC168
		protected override void Refresh()
		{
			int debtLevel = this._displayData.Exchange.RefreshDebt();
			bool flag = debtLevel != RowItemMain.MinDebtShowLevel;
			if (flag)
			{
				RowItemMain.MinDebtShowLevel = debtLevel;
				this.exchangeContainer.selfExchangeList.ReRender();
				this.exchangeContainer.selfItemList.ReRender();
			}
			base.Refresh();
			bool flag2 = !this._displayData.Exchange.DebtEnough;
			if (flag2)
			{
				this.exchangeContainer.confirm.interactable = false;
				this.exchangeContainer.confirmDisplayer.enabled = true;
				this.exchangeContainer.confirmDisplayer.PresetParam[0] = LanguageKey.LK_Exchange_Debt_Not_Enough.Tr();
			}
		}

		// Token: 0x060080C0 RID: 32960 RVA: 0x003BE020 File Offset: 0x003BC220
		protected override void SetItemLockedStatus(ITradeableContent itemData, CharacterDisplayData cd, RowItemLine rowItemLine, RowItemMain main)
		{
			ShopDisplayData displayData = this._displayData;
			bool flag;
			if (displayData != null && displayData.Exchange.IsAreaDebtShop)
			{
				int? num = (cd != null) ? new int?(cd.CharacterId) : null;
				int characterId = this._displayData.TaiwuDisplayData.CharacterId;
				flag = (num.GetValueOrDefault() == characterId & num != null);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				rowItemLine.SetInteractable(false, true);
				rowItemLine.SetDisabled(true);
				main.HideInteractionState();
			}
			else
			{
				base.SetItemLockedStatus(itemData, cd, rowItemLine, main);
			}
		}

		// Token: 0x060080C1 RID: 32961 RVA: 0x003BE0B4 File Offset: 0x003BC2B4
		protected override void RefreshValues()
		{
			foreach (ShopToggle toggle in this.toggles)
			{
				toggle.Refresh(this._displayData.Exchange);
			}
			this.exchangeContainer.targetValue1.text = (this._displayData.Exchange.IsAreaDebtShop ? "-" : ViewExchangeBase.GetPreviewString((int)this.Exchange.TargetValueBase, (int)(this.Exchange.TargetValueBase + this.Exchange.TotalValueWithAdvantage), 1.0));
			this.exchangeContainer.selfValue1.text = ViewExchangeBase.GetPreviewString((int)this.Exchange.TaiwuValueBase, (int)(this.Exchange.TaiwuValueBase - this.Exchange.TotalValueWithAdvantage), 1.0);
			TMP_Text selfValue = this.exchangeContainer.selfValue3;
			int activeIndex = this.exchangeContainer.currPage.GetActiveIndex();
			selfValue.text = ((activeIndex == 0 || activeIndex == 5) ? ViewExchangeBase.GetWeightString(this.Exchange.TaiwuInventoryCurLoad, this.Exchange.TaiwuInventoryMaxLoad, this.Exchange.TaiwuInventoryCurLoadPreview, this.Exchange.TaiwuInventoryMaxLoadPreview, LanguageKey.LK_Exchange_Weight_Value) : ViewExchangeBase.GetWeightString(this.Exchange.TaiwuWarehouseCurLoad, this.Exchange.TaiwuWarehouseMaxLoad, this.Exchange.TaiwuWarehouseCurLoadPreview, this.Exchange.TaiwuWarehouseMaxLoadPreview, LanguageKey.LK_Exchange_Weight_Value));
			this.exchangeContainer.secretText.text = LanguageKey.LK_Exchange_Use_Secret.Tr();
			bool flag = this.Exchange.TargetValueBase < -this.Exchange.TotalValueWithAdvantage;
			if (flag)
			{
				this.ShowBubble(LanguageKey.LK_Shop_Speak_Merchant_No_Money.Tr(), BubbleLevel.High, 5f, 1f);
			}
		}

		// Token: 0x060080C2 RID: 32962 RVA: 0x003BE27C File Offset: 0x003BC47C
		private void UpdateDialog()
		{
		}

		// Token: 0x060080C3 RID: 32963 RVA: 0x003BE28C File Offset: 0x003BC48C
		private string GetIntroduceDialog()
		{
			return this._openShopEventArguments.IsSpecialBuilding ? LanguageKey.LK_SectZhujian_SpecialMerchant_IntroduceDialog.Tr() : Config.MerchantType.Instance[this._displayData.Exchange.MerchantData.MerchantType].IntroduceDialog;
		}

		// Token: 0x060080C4 RID: 32964 RVA: 0x003BE2E0 File Offset: 0x003BC4E0
		private string GetSeasonDialog()
		{
			bool flag = this._openShopEventArguments.MerchantSourceTypeEnum == OpenShopEventArguments.EMerchantSourceType.SingleAdventureCaravan;
			string result;
			if (flag)
			{
				string seasonName = LocalStringManager.Get(string.Format("LK_Season_{0}", this._displayData.Exchange.ExtraGoodsData.SeasonTemplateId));
				seasonName += LocalStringManager.Get(LanguageKey.LK_Quarter);
				result = Config.MerchantType.Instance[this._displayData.Exchange.MerchantData.MerchantType].SpringMarketsAdventureSeasonDialog.GetFormat(seasonName);
			}
			else
			{
				sbyte seasonTemplateId = this._displayData.Exchange.ExtraGoodsData.SeasonTemplateId;
				if (!true)
				{
				}
				string text;
				switch (seasonTemplateId)
				{
				case 0:
					text = Config.MerchantType.Instance[this._displayData.Exchange.MerchantData.MerchantType].SpringSeasonDialog;
					break;
				case 1:
					text = Config.MerchantType.Instance[this._displayData.Exchange.MerchantData.MerchantType].SummerSeasonDialog;
					break;
				case 2:
					text = Config.MerchantType.Instance[this._displayData.Exchange.MerchantData.MerchantType].AutumnSeasonDialog;
					break;
				case 3:
					text = Config.MerchantType.Instance[this._displayData.Exchange.MerchantData.MerchantType].WinterSeasonDialog;
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

		// Token: 0x060080C5 RID: 32965 RVA: 0x003BE458 File Offset: 0x003BC658
		private string GetFavorDialog(int level)
		{
			bool isSpecialBuilding = this._openShopEventArguments.IsSpecialBuilding;
			if (isSpecialBuilding)
			{
				bool flag = level <= 2;
				if (flag)
				{
					return LanguageKey.LK_SectZhujian_SpecialMerchant_FavorDialog1.Tr();
				}
				bool flag2 = level <= 3;
				if (flag2)
				{
					return LanguageKey.LK_SectZhujian_SpecialMerchant_FavorDialog2.Tr();
				}
				bool flag3 = level <= 4;
				if (flag3)
				{
					return LanguageKey.LK_SectZhujian_SpecialMerchant_FavorDialog3.Tr();
				}
			}
			MerchantTypeItem merchantTypeCfg = Config.MerchantType.Instance[this._displayData.Exchange.MerchantData.MerchantType];
			bool flag4 = level <= 1;
			string result;
			if (flag4)
			{
				result = merchantTypeCfg.FavorDialog1;
			}
			else
			{
				bool flag5 = level <= 3;
				if (flag5)
				{
					result = merchantTypeCfg.FavorDialog2;
				}
				else
				{
					result = merchantTypeCfg.FavorDialog3;
				}
			}
			return result;
		}

		// Token: 0x060080C6 RID: 32966 RVA: 0x003BE51C File Offset: 0x003BC71C
		private void RefreshSpeakOnDeal(bool isBuy)
		{
			bool flag = !this._displayData.Exchange.IsAreaDebtShop;
			if (flag)
			{
				this.ShowBubble((isBuy ? LanguageKey.LK_Shop_Speak_Buy : LanguageKey.LK_Shop_Speak_Sell).Tr(), BubbleLevel.High, 5f, 1f);
			}
		}

		// Token: 0x060080C7 RID: 32967 RVA: 0x003BE568 File Offset: 0x003BC768
		private void RefreshSpeakOnItemOverProgress(bool isBuy)
		{
			bool flag = !this._displayData.Exchange.IsAreaDebtShop;
			if (flag)
			{
				this.ShowBubble((isBuy ? LanguageKey.LK_Shop_Speak_Buy_OverProgress : LanguageKey.LK_Shop_Speak_Sell_OverProgress).Tr(), BubbleLevel.High, 5f, 1f);
			}
		}

		// Token: 0x060080C8 RID: 32968 RVA: 0x003BE5B3 File Offset: 0x003BC7B3
		public override void PrepareCustomRowTemplateContainersForCharacter(RowItem rowTemplate)
		{
			this.PrepareCustomRowTemplateContainers(rowTemplate, false);
		}

		// Token: 0x060080C9 RID: 32969 RVA: 0x003BE5C0 File Offset: 0x003BC7C0
		public override void PrepareCustomRowTemplateContainers(RowItem rowTemplate, bool isItemMode)
		{
			Transform containerRoot = rowTemplate.ContainerRoot;
			Object.Instantiate<RowCellContainer>(this.exchangeContainer.itemIconAndNameCellContainer, containerRoot).gameObject.SetActive(true);
			Object.Instantiate<RowCellContainer>(this.exchangeContainer.singleTextCellContainer, containerRoot).gameObject.SetActive(true);
			Object.Instantiate<RowCellContainer>(this.exchangeContainer.singleTextCellContainer, containerRoot).gameObject.SetActive(true);
			Object.Instantiate<RowCellContainer>(this.exchangeContainer.singleTextCellContainer, containerRoot).gameObject.SetActive(true);
			Object.Instantiate<RowCellContainer>(this.exchangeContainer.singleTextCellContainer, containerRoot).gameObject.SetActive(true);
		}

		// Token: 0x060080CA RID: 32970 RVA: 0x003BE666 File Offset: 0x003BC866
		public override IEnumerable<ColumnDefinition> GenerateColumnDefinitions(ItemListScroll scroll, bool isItemMode)
		{
			LayoutOption option = default(LayoutOption);
			yield return scroll.ColumnIconAndName(option, false);
			yield return scroll.ColumnSubType(option, false);
			yield return scroll.ColumnAmount(option, false);
			yield return scroll.ColumnValue(option, false, LanguageKey.LK_ItemValue);
			yield return scroll.ColumnDurability(option, false);
			yield break;
		}

		// Token: 0x17000E16 RID: 3606
		// (get) Token: 0x060080CB RID: 32971 RVA: 0x003BE684 File Offset: 0x003BC884
		protected override ItemDisplayData.ItemUsingOperationType ItemUsingOperationType
		{
			get
			{
				return ItemDisplayData.ItemUsingOperationType.Sell;
			}
		}

		// Token: 0x17000E17 RID: 3607
		// (get) Token: 0x060080CC RID: 32972 RVA: 0x003BE687 File Offset: 0x003BC887
		protected override string ConfirmOnHideDesc
		{
			get
			{
				return LanguageKey.LK_Exchange_Confirm_Shop_Cancel.Tr();
			}
		}

		// Token: 0x060080CD RID: 32973 RVA: 0x003BE693 File Offset: 0x003BC893
		public override void QuickHideImpl()
		{
			base.QuickHideImpl();
		}

		// Token: 0x060080CE RID: 32974 RVA: 0x003BE6A0 File Offset: 0x003BC8A0
		private void AvatarClicked()
		{
			CharacterDisplayData data = this._displayData.TargetCharacterDisplayData;
			bool flag = data != null;
			if (flag)
			{
				base.ShowCharacterMenu(data);
			}
			else
			{
				bool flag2 = this._displayData.Exchange.TradeArguments.OpenShopEventArguments.IsCaravan && this._displayData.Exchange.TradeArguments.OpenShopEventArguments.Id >= 0;
				if (flag2)
				{
					this.ShowBubble(Config.MerchantType.Instance[this._displayData.Exchange.TradeArguments.MerchantData.MerchantType].IntroduceDialog, BubbleLevel.Middle, 5f, 1f);
				}
				else
				{
					bool isFromBuilding = this._displayData.Exchange.TradeArguments.OpenShopEventArguments.IsFromBuilding;
					if (isFromBuilding)
					{
						this.ShowBubble(LocalStringManager.Get(string.Format("LK_Shop_Speak_ClickCharacter{0}", Random.Range(1, 4))), BubbleLevel.Middle, 5f, 1f);
					}
				}
			}
		}

		// Token: 0x04006245 RID: 25157
		[SerializeField]
		private ShopRefreshButton shopRefreshButton;

		// Token: 0x04006246 RID: 25158
		[SerializeField]
		private ShopProgressBar bar;

		// Token: 0x04006247 RID: 25159
		[SerializeField]
		private CButton refreshGoods;

		// Token: 0x04006248 RID: 25160
		[SerializeField]
		private CButton merchantInfo;

		// Token: 0x04006249 RID: 25161
		[SerializeField]
		private ShopToggle[] toggles;

		// Token: 0x0400624A RID: 25162
		[SerializeField]
		private Sprite[] merchantSprites;

		// Token: 0x0400624B RID: 25163
		[SerializeField]
		private GameObject merchantContainer;

		// Token: 0x0400624C RID: 25164
		[SerializeField]
		private Identity identity;

		// Token: 0x0400624D RID: 25165
		[SerializeField]
		private PropertyItem merchantType;

		// Token: 0x0400624E RID: 25166
		[SerializeField]
		private CImage[] moneyIcons;

		// Token: 0x0400624F RID: 25167
		[SerializeField]
		private Sprite debt;

		// Token: 0x04006250 RID: 25168
		[SerializeField]
		private Sprite money;

		// Token: 0x04006251 RID: 25169
		private ShopDisplayData _displayData;

		// Token: 0x04006252 RID: 25170
		private OpenShopEventArguments _openShopEventArguments;

		// Token: 0x04006253 RID: 25171
		private const int MaxShopLevel = 7;

		// Token: 0x04006254 RID: 25172
		private bool _guidingHasExtraGoods;

		// Token: 0x04006255 RID: 25173
		private bool _isFavorExchange;

		// Token: 0x04006256 RID: 25174
		private bool _init;

		// Token: 0x04006257 RID: 25175
		private bool _firstEnter;

		// Token: 0x04006258 RID: 25176
		private ArgumentBox _ab;

		// Token: 0x04006259 RID: 25177
		private Action<ShopExchange> _callback;

		// Token: 0x0400625A RID: 25178
		private bool _level7ShopOccurs;

		// Token: 0x0400625B RID: 25179
		private bool _overDebtShop;

		// Token: 0x0400625C RID: 25180
		[SerializeField]
		private PropertyItem favorProperty;

		// Token: 0x0400625D RID: 25181
		private const float BubbleDuration = 8f;

		// Token: 0x0400625E RID: 25182
		private const float SeasonBubbleDuration = 12f;

		// Token: 0x0400625F RID: 25183
		private const float BubbleInterval = 10f;

		// Token: 0x04006260 RID: 25184
		private const float IntroduceBubbleInterval = 30f;

		// Token: 0x04006261 RID: 25185
		private float _timer;

		// Token: 0x04006262 RID: 25186
		private bool _onlyIntroduceDialog;

		// Token: 0x04006263 RID: 25187
		private bool _firstIntroduceDialog;

		// Token: 0x04006264 RID: 25188
		private bool _secondIntroduceDialog;

		// Token: 0x04006265 RID: 25189
		private bool _seasonDialog;

		// Token: 0x04006266 RID: 25190
		private bool _favoriteDialog;

		// Token: 0x04006267 RID: 25191
		private bool _favorDialog;

		// Token: 0x04006268 RID: 25192
		private bool _remainDialog;
	}
}
