using System;
using System.Collections;
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
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.Information;
using GameData.Domains.Item.Display;
using GameData.Domains.Merchant;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Domains.Taiwu.ExchangeSystem;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Exchange
{
	// Token: 0x02000A2E RID: 2606
	public class ViewShop : ViewExchangeBase, IShopRefresh
	{
		// Token: 0x17000DD1 RID: 3537
		// (get) Token: 0x06007F95 RID: 32661 RVA: 0x003B634B File Offset: 0x003B454B
		LanguageKey IShopRefresh.NameId
		{
			get
			{
				return LanguageKey.LK_Shop_Refresh_Goods;
			}
		}

		// Token: 0x17000DD2 RID: 3538
		// (get) Token: 0x06007F96 RID: 32662 RVA: 0x003B6352 File Offset: 0x003B4552
		LanguageKey IShopRefresh.DescId
		{
			get
			{
				return LanguageKey.LK_Shop_Refresh_Goods_Desc;
			}
		}

		// Token: 0x17000DD3 RID: 3539
		// (get) Token: 0x06007F97 RID: 32663 RVA: 0x003B6359 File Offset: 0x003B4559
		LanguageKey IShopRefresh.BreakId
		{
			get
			{
				return LanguageKey.LK_Shop_Refresh_Goods_Desc_AdditionalBreak;
			}
		}

		// Token: 0x17000DD4 RID: 3540
		// (get) Token: 0x06007F98 RID: 32664 RVA: 0x003B6360 File Offset: 0x003B4560
		LanguageKey IShopRefresh.NoTimeId
		{
			get
			{
				return LanguageKey.LK_Shop_Refresh_Goods_Desc_NoTime;
			}
		}

		// Token: 0x17000DD5 RID: 3541
		// (get) Token: 0x06007F99 RID: 32665 RVA: 0x003B6367 File Offset: 0x003B4567
		LanguageKey IShopRefresh.NeedClearId
		{
			get
			{
				return LanguageKey.LK_Shop_Refresh_Goods_Desc_NeedClear;
			}
		}

		// Token: 0x06007F9A RID: 32666 RVA: 0x003B6370 File Offset: 0x003B4570
		void IShopRefresh.InitShopRefresh(Action refreshCount, Action<bool> refreshActiveStates, Action<bool> refreshTips)
		{
			this._protected = false;
			this._refreshTips = refreshTips;
			if (refreshTips != null)
			{
				Exchange exchange = this.Exchange;
				refreshTips(exchange != null && exchange.TargetContentList.Count > 0);
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

		// Token: 0x06007F9B RID: 32667 RVA: 0x003B63E8 File Offset: 0x003B45E8
		void IShopRefresh.RefreshCurrentGoods()
		{
			bool @protected = this._protected;
			if (!@protected)
			{
				this._protected = true;
				int charId = this._displayData.Exchange.CharId;
				OpenShopEventArguments openShopEventArguments = this._displayData.Exchange.TradeArguments.OpenShopEventArguments;
				bool isChar = openShopEventArguments != null && !openShopEventArguments.IsCaravan && !openShopEventArguments.IsFromBuilding && !openShopEventArguments.IsSpecialBuilding && !openShopEventArguments.IsSettlementTreasury;
				ExchangeContainer exchangeContainer = this.exchangeContainer;
				int? num;
				if (exchangeContainer == null)
				{
					num = null;
				}
				else
				{
					CToggleGroup targetPage = exchangeContainer.targetPage;
					num = ((targetPage != null) ? new int?(targetPage.GetActiveIndex()) : null);
				}
				MerchantDomainMethod.AsyncCall.RefreshMerchantGoods(this, charId, isChar, (sbyte)(num ?? -1), this._displayData.Exchange.TradeArguments.OpenShopEventArguments.IsFromBuilding, this._displayData.Exchange.TradeArguments.OpenShopEventArguments.IsHeadBuildingMerchant, this._displayData.Exchange.TradeArguments.OpenShopEventArguments.BuildingMerchantType, delegate(int offset, RawDataPool dataPool)
				{
					bool curr = false;
					Serializer.Deserialize(dataPool, offset, ref curr);
					bool flag = curr;
					if (flag)
					{
						this.RequestData();
						ShopExchange exchange = this._displayData.Exchange;
						sbyte? b;
						if (exchange == null)
						{
							b = null;
						}
						else
						{
							MerchantTradeArguments tradeArguments = exchange.TradeArguments;
							if (tradeArguments == null)
							{
								b = null;
							}
							else
							{
								MerchantData merchantData = tradeArguments.MerchantData;
								b = ((merchantData != null) ? new sbyte?(merchantData.MerchantType) : null);
							}
						}
						sbyte? b2 = b;
						string desc;
						bool flag2;
						if (b2 != null)
						{
							sbyte templateId = b2.GetValueOrDefault();
							if (templateId != -1)
							{
								MerchantTypeItem merchantTypeItem = Config.MerchantType.Instance[(int)templateId];
								desc = ((merchantTypeItem != null) ? merchantTypeItem.RefreshDesc : null);
								flag2 = (desc != null);
								goto IL_A2;
							}
						}
						flag2 = false;
						IL_A2:
						bool flag3 = flag2;
						if (flag3)
						{
							this.ShowBubble(desc, BubbleLevel.Middle, 5f, 1f);
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
		}

		// Token: 0x17000DD6 RID: 3542
		// (get) Token: 0x06007F9C RID: 32668 RVA: 0x003B6504 File Offset: 0x003B4704
		bool IShopRefresh.CanRefreshCurrentGoods
		{
			get
			{
				Exchange exchange = this.Exchange;
				bool? flag;
				if (exchange == null)
				{
					flag = null;
				}
				else
				{
					List<ITradeableContent> targetContentList = exchange.TargetContentList;
					flag = ((targetContentList != null) ? new int?(targetContentList.Count) : null);
				}
				if (!(flag ?? true))
				{
					int activeIndex = this.exchangeContainer.targetPage.GetActiveIndex();
					if (activeIndex >= 0 && activeIndex <= 6)
					{
						return SingletonObject.getInstance<TimeManager>().IsActionPointEnough(GlobalConfig.Instance.RefreshItemApCost);
					}
				}
				return false;
			}
		}

		// Token: 0x17000DD7 RID: 3543
		// (get) Token: 0x06007F9D RID: 32669 RVA: 0x003B658A File Offset: 0x003B478A
		private bool _isNormalCharacter
		{
			get
			{
				return this._openShopEventArguments.MerchantSourceTypeEnum == OpenShopEventArguments.EMerchantSourceType.NormalCharacter;
			}
		}

		// Token: 0x17000DD8 RID: 3544
		// (get) Token: 0x06007F9E RID: 32670 RVA: 0x003B659C File Offset: 0x003B479C
		bool IShopRefresh.CanShow
		{
			get
			{
				ExchangeContainer exchangeContainer = this.exchangeContainer;
				int? num;
				if (exchangeContainer == null)
				{
					num = null;
				}
				else
				{
					CToggleGroup targetPage = exchangeContainer.targetPage;
					num = ((targetPage != null) ? new int?(targetPage.GetActiveIndex()) : null);
				}
				bool result;
				if ((sbyte)(num ?? 7) < 7)
				{
					ShopDisplayData displayData = this._displayData;
					OpenShopEventArguments openShopEventArguments;
					if (displayData == null)
					{
						openShopEventArguments = null;
					}
					else
					{
						ShopExchange exchange = displayData.Exchange;
						if (exchange == null)
						{
							openShopEventArguments = null;
						}
						else
						{
							MerchantTradeArguments tradeArguments = exchange.TradeArguments;
							openShopEventArguments = ((tradeArguments != null) ? tradeArguments.OpenShopEventArguments : null);
						}
					}
					OpenShopEventArguments.EMerchantSourceType merchantSourceTypeEnum = (openShopEventArguments ?? this._openShopEventArguments).MerchantSourceTypeEnum;
					result = (merchantSourceTypeEnum == OpenShopEventArguments.EMerchantSourceType.NormalCharacter || merchantSourceTypeEnum == OpenShopEventArguments.EMerchantSourceType.SpecifiedOnBuildingMerchantType || merchantSourceTypeEnum == OpenShopEventArguments.EMerchantSourceType.MerchantHeadBuilding || merchantSourceTypeEnum == OpenShopEventArguments.EMerchantSourceType.MerchantBranchBuilding);
				}
				else
				{
					result = false;
				}
				return result;
			}
		}

		// Token: 0x17000DD9 RID: 3545
		// (get) Token: 0x06007F9F RID: 32671 RVA: 0x003B6649 File Offset: 0x003B4849
		bool IShopRefresh.Protected
		{
			get
			{
				return this._protected;
			}
		}

		// Token: 0x06007FA0 RID: 32672 RVA: 0x003B6651 File Offset: 0x003B4851
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
				refreshTips(this.Exchange.TargetContentList.Count > 0);
			}
			this._protected = false;
			Action<bool> refreshActiveStates = this._refreshActiveStates;
			if (refreshActiveStates != null)
			{
				refreshActiveStates(((IShopRefresh)this).CanShow);
			}
			yield break;
		}

		// Token: 0x06007FA1 RID: 32673 RVA: 0x003B6660 File Offset: 0x003B4860
		public void RefreshRefreshButton(bool shouldAlsoRefreshPreview)
		{
			Action<bool> refreshTips = this._refreshTips;
			if (refreshTips != null)
			{
				refreshTips(this.Exchange.TargetContentList.Count > 0);
			}
		}

		// Token: 0x06007FA2 RID: 32674 RVA: 0x003B6688 File Offset: 0x003B4888
		[Button("Transfer")]
		private void Transfer(MapBlockCharStat character)
		{
			FieldInfo field = character.GetType().GetField("merchantSprites", (BindingFlags)(-1));
			this.merchantSprites = (Sprite[])((field != null) ? field.GetValue(character) : null);
		}

		// Token: 0x17000DDA RID: 3546
		// (get) Token: 0x06007FA3 RID: 32675 RVA: 0x003B66B4 File Offset: 0x003B48B4
		protected override string SortSaveKeyPrefix
		{
			get
			{
				return "ViewShop";
			}
		}

		// Token: 0x17000DDB RID: 3547
		// (get) Token: 0x06007FA4 RID: 32676 RVA: 0x003B66BB File Offset: 0x003B48BB
		public override Exchange Exchange
		{
			get
			{
				ShopDisplayData displayData = this._displayData;
				return (displayData != null) ? displayData.Exchange : null;
			}
		}

		// Token: 0x17000DDC RID: 3548
		// (get) Token: 0x06007FA5 RID: 32677 RVA: 0x003B66CF File Offset: 0x003B48CF
		public override SecretInformationDisplayPackage SecretInformationDisplayPackage
		{
			get
			{
				return new SecretInformationDisplayPackage();
			}
		}

		// Token: 0x17000DDD RID: 3549
		// (get) Token: 0x06007FA6 RID: 32678 RVA: 0x003B66D6 File Offset: 0x003B48D6
		public override CharacterDisplayData TargetDisplayData
		{
			get
			{
				ShopDisplayData displayData = this._displayData;
				return (displayData != null) ? displayData.TargetCharacterDisplayData : null;
			}
		}

		// Token: 0x17000DDE RID: 3550
		// (get) Token: 0x06007FA7 RID: 32679 RVA: 0x003B66EA File Offset: 0x003B48EA
		public override CharacterDisplayData TaiwuDisplayData
		{
			get
			{
				ShopDisplayData displayData = this._displayData;
				return (displayData != null) ? displayData.TaiwuDisplayData : null;
			}
		}

		// Token: 0x06007FA8 RID: 32680 RVA: 0x003B6700 File Offset: 0x003B4900
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

		// Token: 0x06007FA9 RID: 32681 RVA: 0x003B6793 File Offset: 0x003B4993
		public override IReadOnlyList<ITradeableContent> GetTargetTradeableList(int index)
		{
			return (index < 0 || index > 7) ? this._displayData[7] : this._displayData[index];
		}

		// Token: 0x17000DDF RID: 3551
		// (get) Token: 0x06007FAA RID: 32682 RVA: 0x003B67B7 File Offset: 0x003B49B7
		protected override ESortAndFilterControllerType ControllerType
		{
			get
			{
				return ESortAndFilterControllerType.Shop;
			}
		}

		// Token: 0x17000DE0 RID: 3552
		// (get) Token: 0x06007FAB RID: 32683 RVA: 0x003B67BA File Offset: 0x003B49BA
		protected override ItemListScroll.EColumnType ColumnType
		{
			get
			{
				return ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Durability | ItemListScroll.EColumnType.ShopPrice;
			}
		}

		// Token: 0x06007FAC RID: 32684 RVA: 0x003B67C2 File Offset: 0x003B49C2
		private void RefreshDebt(ShopExchange shopExchange)
		{
			this.RequestData();
		}

		// Token: 0x06007FAD RID: 32685 RVA: 0x003B67CC File Offset: 0x003B49CC
		protected override void Awake()
		{
			this.ConfirmOnHideKey = LanguageKey.LK_Exchange_Shop_Title;
			base.Awake();
			this.exchangeContainer.exchangeBack.SetMerchant(1);
			this.exchangeContainer.exchangeFront.SetMerchant(1);
			this.refreshGoods.onClick.ResetListener(new Action(this.RefreshCurrentGoods));
			this.merchantInfo.onClick.ResetListener(new Action(this.MerchantInfo));
			this.giftBtn.onClick.ResetListener(delegate()
			{
				UIElement.NewShopGift.SetOnInitArgs(this._ab.SetObject("callback", new Action<ShopExchange>(this.RefreshDebt)));
				UIManager.Instance.MaskUI(UIElement.NewShopGift);
			});
			this.exchangeContainer.targetPage.OnActiveIndexChange += delegate(int newTog, int _)
			{
				this.refreshGoods.gameObject.SetActive(((IShopRefresh)this).CanShow);
				this.refreshGoods.interactable = ((IShopRefresh)this).CanRefreshCurrentGoods;
				this.CheckHasExtraGoods(newTog);
			};
			this.exchangeContainer.currPage.OnActiveIndexChange += delegate(int newTog, int oldTog)
			{
				this.SetTitleTaiwu(newTog);
			};
		}

		// Token: 0x06007FAE RID: 32686 RVA: 0x003B68A4 File Offset: 0x003B4AA4
		private void CheckHasExtraGoods(int newTog)
		{
			bool flag;
			if (!this._guidingHasExtraGoods && newTog >= 0 && newTog < 7)
			{
				IReadOnlyList<ITradeableContent> targetTradeableList = this.GetTargetTradeableList(newTog);
				if (targetTradeableList == null)
				{
					flag = false;
				}
				else
				{
					flag = targetTradeableList.Any(delegate(ITradeableContent x)
					{
						sbyte? b = (x != null) ? new sbyte?(x.ExtraGoodsType) : null;
						if (b != null)
						{
							sbyte valueOrDefault = b.GetValueOrDefault();
							if (valueOrDefault != 0)
							{
								return valueOrDefault != 1;
							}
						}
						return false;
					});
				}
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				this._guidingHasExtraGoods = true;
				GlobalDomainMethod.Call.InvokeGuidingTrigger(312);
			}
		}

		// Token: 0x06007FAF RID: 32687 RVA: 0x003B690F File Offset: 0x003B4B0F
		private void SetTitleTaiwu(int newTog)
		{
			this.SetTitle(this.exchangeContainer.titleTaiwu, newTog);
		}

		// Token: 0x06007FB0 RID: 32688 RVA: 0x003B6924 File Offset: 0x003B4B24
		protected override void AwakeAvatarBtn()
		{
			this.exchangeContainer.avatarBtn.interactable = true;
			this.exchangeContainer.avatarBtn.onClick.ResetListener(new Action(this.AvatarClicked));
			this.exchangeContainer.btnOpenCharMenu.onClick.ResetListener(new Action(this.AvatarClicked));
		}

		// Token: 0x06007FB1 RID: 32689 RVA: 0x003B6988 File Offset: 0x003B4B88
		private void Update()
		{
			this.UpdateDialog();
		}

		// Token: 0x06007FB2 RID: 32690 RVA: 0x003B6994 File Offset: 0x003B4B94
		public override void Submit()
		{
			UIElement.ShopConfirm.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("tradeMoney", (int)Math.Clamp(-this._displayData.Exchange.TotalValue, -2147483648L, 2147483647L)).Set("merchantMoney", (int)this._displayData.Exchange.TargetValueBase).Set("selfMoney", (int)this._displayData.Exchange.TaiwuValueBase).Set("merchantType", this._displayData.Exchange.MerchantData.MerchantType).Set("isAreaDebtShop", this._displayData.Exchange.IsAreaDebtShop).SetObject("onConfirm", new Action(this.SubmitImpl)));
			UIManager.Instance.MaskUI(UIElement.ShopConfirm);
		}

		// Token: 0x06007FB3 RID: 32691 RVA: 0x003B6A74 File Offset: 0x003B4C74
		public void SubmitImpl()
		{
			bool flag = -this._displayData.Exchange.TotalValue != 0L;
			if (flag)
			{
				this.RefreshSpeakOnDeal(-this._displayData.Exchange.TotalValue < 0L);
			}
			AudioManager.Instance.PlaySound("ui_caravan_coin", false, false);
			TaiwuDomainMethod.Call.ConfirmShopExchange(this._displayData.Exchange);
			this.Reset();
		}

		// Token: 0x06007FB4 RID: 32692 RVA: 0x003B6AE4 File Offset: 0x003B4CE4
		public void MerchantInfo()
		{
			UIElement.MerchantInfo.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("MerchantType", this._displayData.Exchange.MerchantData.MerchantType).Set("MerchantFavorability", this._displayData.Exchange.MerchantFavor).Set("MerchantTemplateId", this._displayData.Exchange.MerchantData.MerchantTemplateId).Set("Toggle", ViewMerchantInfo.GetToggle(this._displayData.Exchange.TradeArguments.OpenShopEventArguments)));
			UIManager.Instance.MaskUI(UIElement.MerchantInfo);
		}

		// Token: 0x06007FB5 RID: 32693 RVA: 0x003B6B90 File Offset: 0x003B4D90
		public override void OnInit(ArgumentBox argsBox)
		{
			this._ab = (ArgumentBox)argsBox.Clone();
			this._protected = (this._init = false);
			this._displayData = null;
			if (argsBox != null)
			{
				argsBox.Get<OpenShopEventArguments>("OpenShopEventArguments", out this._openShopEventArguments);
			}
			if (argsBox != null)
			{
				argsBox.Set("CharacterId", this._openShopEventArguments.Id);
			}
			base.OnInit(argsBox);
			this.exchangeContainer.confirmButtonText.text = LanguageKey.LK_Shop_DoDeal.Tr();
			this.exchangeContainer.AddSwitchToggleListener();
			this.refreshGoods.gameObject.SetActive(false);
			bool flag = !this._firstEnter;
			if (flag)
			{
				this._firstEnter = true;
				GlobalDomainMethod.Call.InvokeGuidingTrigger(67);
			}
			this.exchangeContainer.currPage.Get(3).gameObject.SetActive(false);
		}

		// Token: 0x06007FB6 RID: 32694 RVA: 0x003B6C75 File Offset: 0x003B4E75
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

		// Token: 0x06007FB7 RID: 32695 RVA: 0x003B6CB4 File Offset: 0x003B4EB4
		protected override void RequestData()
		{
			TaiwuDomainMethod.AsyncCall.GetShopDisplayData(this, this._openShopEventArguments, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._displayData);
				CharacterDisplayData data = this._displayData.TargetCharacterDisplayData;
				bool flag = data != null;
				if (flag)
				{
					this.exchangeContainer.btnOpenCharMenu.gameObject.SetActive(true);
				}
				else
				{
					this.exchangeContainer.btnOpenCharMenu.gameObject.SetActive(false);
				}
				ShopDisplayData displayData = this._displayData;
				this.ExchangeValueType = ((displayData != null && displayData.Exchange.IsAreaDebtShop) ? 10 : 6);
				bool flag2 = !this._level7ShopOccurs && this._displayData.Exchange.MaxDebtLevel >= GuidingChapterTrigger.DefValue.FirstEnterLevel7Shop.Int1;
				if (flag2)
				{
					this._level7ShopOccurs = true;
					GlobalDomainMethod.Call.InvokeGuidingTrigger(111);
				}
				RowItemMain.MinDebtLevel = this._displayData.Exchange.MinDebtLevel;
				RowItemMain.MaxDebtLevel = this._displayData.Exchange.MaxDebtLevel;
				bool flag3 = !this._overDebtShop && RowItemMain.MinDebtLevel < RowItemMain.MaxDebtLevel;
				if (flag3)
				{
					this._overDebtShop = true;
					GlobalDomainMethod.Call.InvokeGuidingTrigger(128);
				}
				MerchantItem merchantCfg = this._displayData.Exchange.MerchantData.MerchantConfig;
				bool flag4 = !this._init;
				if (flag4)
				{
					bool flag5 = merchantCfg.TemplateId == 53;
					if (flag5)
					{
						this.exchangeContainer.targetItemList.SortAndFilterController.SetToggleVisible(0, 6);
						this.exchangeContainer.targetItemList.SortAndFilterController.SetToggleIsOn(0, 6);
					}
					else
					{
						bool flag6 = merchantCfg.TemplateId == 52;
						if (flag6)
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
					this.exchangeContainer.targetPage.Get(7).interactable &= (merchantCfg.TemplateId != 53);
				}
				MerchantTypeItem merchantTypeItem = Config.MerchantType.Instance[(merchantCfg != null) ? merchantCfg.MerchantType : -1];
				string typeName = ((merchantTypeItem != null) ? merchantTypeItem.Name : null) ?? "";
				bool flag7 = this.merchantSprites.CheckIndex((int)this._displayData.Exchange.MerchantData.MerchantTemplateId);
				if (flag7)
				{
					Sprite icon = this.merchantSprites[(int)this._displayData.Exchange.MerchantData.MerchantTemplateId];
					this.merchantType.Set(icon, string.Empty, typeName, null, false);
				}
				else
				{
					this.merchantContainer.SetActive(false);
				}
				sbyte? b = (merchantCfg != null) ? new sbyte?(merchantCfg.MerchantType) : null;
				bool flag8;
				if (b != null)
				{
					sbyte valueOrDefault = b.GetValueOrDefault();
					if (valueOrDefault >= 0)
					{
						flag8 = (valueOrDefault <= 6);
						goto IL_32B;
					}
				}
				flag8 = false;
				IL_32B:
				bool flag9 = flag8;
				if (flag9)
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
						goto IL_3F4;
					}
				}
				active = false;
				IL_3F4:
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
					this.refreshGoods.gameObject.SetActive(((IShopRefresh)this).CanShow);
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
					bool flag10;
					if (valueTuple.Item1 != null)
					{
						k = valueTuple.Item2;
						flag10 = true;
					}
					else
					{
						flag10 = false;
					}
					bool flag11 = flag10;
					if (flag11)
					{
						this.exchangeContainer.targetPage.Set(k, false);
					}
					this.Element.ShowAfterRefresh();
				}
			});
		}

		// Token: 0x06007FB8 RID: 32696 RVA: 0x003B6CD0 File Offset: 0x003B4ED0
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

		// Token: 0x06007FB9 RID: 32697 RVA: 0x003B6DBC File Offset: 0x003B4FBC
		protected override void RefreshTargetItems(int index)
		{
			base.RefreshTargetItems(index);
			this.bar.Set(this._displayData.Exchange.MerchantFavor, this._displayData.Exchange.FavorabilityWithDelta);
			this.debtMask.gameObject.SetActive(this._displayData.Exchange.TradeArguments.OverFavorData.MerchantOverFavorLevelDataArray.CheckIndex(index) && this._displayData.Exchange.TradeArguments.OverFavorData.MerchantOverFavorLevelDataArray[index].BuyCount <= 0);
		}

		// Token: 0x06007FBA RID: 32698 RVA: 0x003B6E5C File Offset: 0x003B505C
		protected override void OnClickTargetItem(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			Action <>9__1;
			Action<int> <>9__2;
			this.exchangeContainer.targetItemList.HandleClickItem(itemData, rowItemLine, delegate(RowItemLine x)
			{
				bool flag = itemData.Amount == 1;
				if (flag)
				{
					TipType type = rowItemLine.TipDisplayer.Type;
					this.Exchange.SelectTargetItem(itemData, 1);
					this.Refresh();
					YieldHelper instance = SingletonObject.getInstance<YieldHelper>();
					uint frame = 1U;
					Action job;
					if ((job = <>9__1) == null)
					{
						job = (<>9__1 = delegate()
						{
							rowItemLine.TipDisplayer.Refresh(true, -1);
						});
					}
					instance.DelayFrameDo(frame, job);
				}
				else
				{
					int index = (int)(itemData.ItemSourceType - 10);
					int limitCount = (int)((!this._displayData.Exchange.TradeArguments.OverFavorData.MerchantOverFavorLevelDataArray.CheckIndex(index) || this._displayData.Exchange.TradeArguments.OverFavorData.MerchantOverFavorLevelDataArray[index].BuyCount == short.MaxValue) ? 0 : this._displayData.Exchange.TradeArguments.OverFavorData.MerchantOverFavorLevelDataArray[index].BuyCount);
					ItemListScroll targetItemList = this.exchangeContainer.targetItemList;
					Action<int> onConfirmSetCount;
					if ((onConfirmSetCount = <>9__2) == null)
					{
						onConfirmSetCount = (<>9__2 = delegate(int count)
						{
							this.Exchange.SelectTargetItem(itemData, count);
							this.Refresh();
						});
					}
					targetItemList.SetItemToSelectCountMode(x, onConfirmSetCount, delegate
					{
					}, 0, limitCount, 1, null, false, null, false);
				}
			});
		}

		// Token: 0x06007FBB RID: 32699 RVA: 0x003B6EB0 File Offset: 0x003B50B0
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
			this.refreshGoods.interactable = ((IShopRefresh)this).CanRefreshCurrentGoods;
			this.giftInvoker.Type = TipType.Simple;
			bool flag3 = this.giftBtn.interactable = (this.Exchange.ExchangeItemList.Count == 0);
			if (flag3)
			{
				TooltipInvoker tooltipInvoker = this.giftInvoker;
				ArgumentBox argumentBox;
				if ((argumentBox = tooltipInvoker.RuntimeParam) == null)
				{
					argumentBox = (tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>());
				}
				argumentBox.Set("arg0", LanguageKey.LK_Exchange_Gift.Tr()).Set("arg1", LanguageKey.LK_Exchange_Gift_Desc.Tr());
			}
			else
			{
				TooltipInvoker tooltipInvoker = this.giftInvoker;
				ArgumentBox argumentBox2;
				if ((argumentBox2 = tooltipInvoker.RuntimeParam) == null)
				{
					argumentBox2 = (tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>());
				}
				argumentBox2.Set("arg0", LanguageKey.LK_Exchange_Gift.Tr()).Set("arg1", LanguageKey.LK_Exchange_Gift_Disable_Desc.Tr());
			}
			ShopRefreshButton shopRefreshButton = this.shopRefreshButton;
			Exchange exchange = this.Exchange;
			int? num;
			if (exchange == null)
			{
				num = null;
			}
			else
			{
				List<ITradeableContent> targetContentList = exchange.TargetContentList;
				num = ((targetContentList != null) ? new int?(targetContentList.Count) : null);
			}
			int? num2 = num;
			shopRefreshButton.RefreshTips(num2.GetValueOrDefault() > 0);
		}

		// Token: 0x06007FBC RID: 32700 RVA: 0x003B709C File Offset: 0x003B529C
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

		// Token: 0x06007FBD RID: 32701 RVA: 0x003B7130 File Offset: 0x003B5330
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

		// Token: 0x06007FBE RID: 32702 RVA: 0x003B72F8 File Offset: 0x003B54F8
		private void UpdateDialog()
		{
			bool flag = this._openShopEventArguments.IsSettlementTreasury || this._displayData == null;
			if (!flag)
			{
				this._timer += Time.deltaTime;
				bool flag2 = this._onlyIntroduceDialog || this._openShopEventArguments.MerchantSourceTypeEnum == OpenShopEventArguments.EMerchantSourceType.SpecifiedOnBuildingMerchantType;
				if (flag2)
				{
					bool flag3 = this._timer >= 30f;
					if (flag3)
					{
						this.ShowBubble(this.GetIntroduceDialog(), BubbleLevel.Low, 5f, 1f);
						this._timer = 0f;
					}
				}
				else
				{
					int index = 0;
					bool flag4 = this._timer >= 10f * (float)index++ && !this._firstIntroduceDialog;
					if (flag4)
					{
						this._firstIntroduceDialog = true;
						this.ShowBubble(this.GetIntroduceDialog(), BubbleLevel.Low, 5f, 1f);
					}
					MerchantExtraGoodsData extraGoodsData = this._displayData.Exchange.ExtraGoodsData;
					bool flag5;
					if (extraGoodsData != null && extraGoodsData.SeasonTemplateId >= 0)
					{
						List<MerchantExtraGoodsItem> seasonExtraGoods = extraGoodsData.SeasonExtraGoods;
						if (seasonExtraGoods != null)
						{
							flag5 = (seasonExtraGoods.Count > 0);
							goto IL_116;
						}
					}
					flag5 = false;
					IL_116:
					bool flag6 = flag5;
					if (flag6)
					{
						bool flag7 = this._timer >= 10f * (float)index++ && !this._seasonDialog;
						if (flag7)
						{
							this._seasonDialog = true;
							this.ShowBubble(this.GetSeasonDialog(), BubbleLevel.Low, 5f, 1f);
						}
					}
					bool flag8 = this._timer >= 10f * (float)index++ && !this._favorDialog;
					if (flag8)
					{
						this._favorDialog = true;
						this.ShowBubble(this.GetFavorDialog(GameData.Domains.Merchant.SharedMethods.GetFavorLevel(this._displayData.Exchange.MerchantFavor)), BubbleLevel.Low, 5f, 1f);
					}
					bool flag9 = this._timer >= 10f * (float)index++ && !this._remainDialog;
					if (flag9)
					{
						this._remainDialog = true;
						this.ShowBubble((Random.Range(0, 2) == 0) ? LanguageKey.LK_Shop_Speak_Remain1.Tr() : LanguageKey.LK_Shop_Speak_Remain2.Tr(), BubbleLevel.Low, 5f, 1f);
					}
					bool flag10 = this._timer >= 10f * (float)index++ && !this._secondIntroduceDialog;
					if (flag10)
					{
						this._secondIntroduceDialog = true;
						this.ShowBubble(this.GetIntroduceDialog(), BubbleLevel.Low, 5f, 1f);
						this._onlyIntroduceDialog = true;
						this._timer = 0f;
					}
				}
			}
		}

		// Token: 0x06007FBF RID: 32703 RVA: 0x003B7580 File Offset: 0x003B5780
		private string GetIntroduceDialog()
		{
			return this._openShopEventArguments.IsSpecialBuilding ? LanguageKey.LK_SectZhujian_SpecialMerchant_IntroduceDialog.Tr() : Config.MerchantType.Instance[this._displayData.Exchange.MerchantData.MerchantType].IntroduceDialog;
		}

		// Token: 0x06007FC0 RID: 32704 RVA: 0x003B75D4 File Offset: 0x003B57D4
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

		// Token: 0x06007FC1 RID: 32705 RVA: 0x003B774C File Offset: 0x003B594C
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

		// Token: 0x06007FC2 RID: 32706 RVA: 0x003B7810 File Offset: 0x003B5A10
		private void RefreshSpeakOnDeal(bool isBuy)
		{
			bool flag = !this._displayData.Exchange.IsAreaDebtShop;
			if (flag)
			{
				this.ShowBubble((isBuy ? LanguageKey.LK_Shop_Speak_Buy : LanguageKey.LK_Shop_Speak_Sell).Tr(), BubbleLevel.High, 5f, 1f);
			}
		}

		// Token: 0x06007FC3 RID: 32707 RVA: 0x003B785C File Offset: 0x003B5A5C
		private void RefreshSpeakOnItemOverProgress(bool isBuy)
		{
			bool flag = !this._displayData.Exchange.IsAreaDebtShop;
			if (flag)
			{
				this.ShowBubble((isBuy ? LanguageKey.LK_Shop_Speak_Buy_OverProgress : LanguageKey.LK_Shop_Speak_Sell_OverProgress).Tr(), BubbleLevel.High, 5f, 1f);
			}
		}

		// Token: 0x06007FC4 RID: 32708 RVA: 0x003B78A7 File Offset: 0x003B5AA7
		public override void PrepareCustomRowTemplateContainersForCharacter(RowItem rowTemplate)
		{
			this.PrepareCustomRowTemplateContainers(rowTemplate, false);
		}

		// Token: 0x06007FC5 RID: 32709 RVA: 0x003B78B4 File Offset: 0x003B5AB4
		public override void PrepareCustomRowTemplateContainers(RowItem rowTemplate, bool isItemMode)
		{
			Transform containerRoot = rowTemplate.ContainerRoot;
			Object.Instantiate<RowCellContainer>(this.exchangeContainer.itemIconAndNameCellContainer, containerRoot).gameObject.SetActive(true);
			Object.Instantiate<RowCellContainer>(this.exchangeContainer.singleTextCellContainer, containerRoot).gameObject.SetActive(true);
			Object.Instantiate<RowCellContainer>(this.exchangeContainer.singleTextCellContainer, containerRoot).gameObject.SetActive(true);
			Object.Instantiate<RowCellContainer>(this.exchangeContainer.singleTextCellContainer, containerRoot).gameObject.SetActive(true);
			Object.Instantiate<RowCellContainer>(this.exchangeContainer.singleTextCellContainer, containerRoot).gameObject.SetActive(true);
		}

		// Token: 0x06007FC6 RID: 32710 RVA: 0x003B795A File Offset: 0x003B5B5A
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

		// Token: 0x17000DE1 RID: 3553
		// (get) Token: 0x06007FC7 RID: 32711 RVA: 0x003B7978 File Offset: 0x003B5B78
		protected override ItemDisplayData.ItemUsingOperationType ItemUsingOperationType
		{
			get
			{
				return ItemDisplayData.ItemUsingOperationType.Sell;
			}
		}

		// Token: 0x17000DE2 RID: 3554
		// (get) Token: 0x06007FC8 RID: 32712 RVA: 0x003B797B File Offset: 0x003B5B7B
		protected override string ConfirmOnHideDesc
		{
			get
			{
				return LanguageKey.LK_Exchange_Confirm_Shop_Cancel.Tr();
			}
		}

		// Token: 0x06007FC9 RID: 32713 RVA: 0x003B7987 File Offset: 0x003B5B87
		public override void QuickHideImpl()
		{
			TaiwuEventDomainMethod.Call.TriggerListener("MerchantShopClose", true);
			TaiwuEventDomainMethod.Call.TriggerListener("ShopActionComplete", true);
			base.QuickHideImpl();
		}

		// Token: 0x06007FCA RID: 32714 RVA: 0x003B79AC File Offset: 0x003B5BAC
		private void AvatarClicked()
		{
			CharacterDisplayData data = this._displayData.TargetCharacterDisplayData;
			bool flag = data != null && data.CreatingType == 1;
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

		// Token: 0x040061E2 RID: 25058
		private bool _protected;

		// Token: 0x040061E3 RID: 25059
		private Action<bool> _refreshActiveStates;

		// Token: 0x040061E4 RID: 25060
		private Action _refreshCount;

		// Token: 0x040061E5 RID: 25061
		private Action<bool> _refreshTips;

		// Token: 0x040061E6 RID: 25062
		private const int BuyBackTogIndex = 7;

		// Token: 0x040061E7 RID: 25063
		[SerializeField]
		private ShopRefreshButton shopRefreshButton;

		// Token: 0x040061E8 RID: 25064
		[SerializeField]
		private ShopProgressBar bar;

		// Token: 0x040061E9 RID: 25065
		[SerializeField]
		private CButton refreshGoods;

		// Token: 0x040061EA RID: 25066
		[SerializeField]
		private CButton merchantInfo;

		// Token: 0x040061EB RID: 25067
		[SerializeField]
		private CButton giftBtn;

		// Token: 0x040061EC RID: 25068
		[SerializeField]
		private ShopToggle[] toggles;

		// Token: 0x040061ED RID: 25069
		[SerializeField]
		private Sprite[] merchantSprites;

		// Token: 0x040061EE RID: 25070
		[SerializeField]
		private GameObject merchantContainer;

		// Token: 0x040061EF RID: 25071
		[SerializeField]
		private Identity identity;

		// Token: 0x040061F0 RID: 25072
		[SerializeField]
		private PropertyItem merchantType;

		// Token: 0x040061F1 RID: 25073
		[SerializeField]
		private CImage[] moneyIcons;

		// Token: 0x040061F2 RID: 25074
		[SerializeField]
		private Sprite debt;

		// Token: 0x040061F3 RID: 25075
		[SerializeField]
		private Sprite money;

		// Token: 0x040061F4 RID: 25076
		[SerializeField]
		private TooltipInvoker giftInvoker;

		// Token: 0x040061F5 RID: 25077
		private ShopDisplayData _displayData;

		// Token: 0x040061F6 RID: 25078
		private OpenShopEventArguments _openShopEventArguments;

		// Token: 0x040061F7 RID: 25079
		private const int MaxShopLevel = 7;

		// Token: 0x040061F8 RID: 25080
		private ArgumentBox _ab;

		// Token: 0x040061F9 RID: 25081
		private bool _guidingHasExtraGoods;

		// Token: 0x040061FA RID: 25082
		private bool _isFavorExchange;

		// Token: 0x040061FB RID: 25083
		private bool _init;

		// Token: 0x040061FC RID: 25084
		private bool _firstEnter;

		// Token: 0x040061FD RID: 25085
		private bool _level7ShopOccurs;

		// Token: 0x040061FE RID: 25086
		private bool _overDebtShop;

		// Token: 0x040061FF RID: 25087
		[SerializeField]
		private PropertyItem favorProperty;

		// Token: 0x04006200 RID: 25088
		[SerializeField]
		private GameObject debtMask;

		// Token: 0x04006201 RID: 25089
		private const float BubbleDuration = 8f;

		// Token: 0x04006202 RID: 25090
		private const float SeasonBubbleDuration = 12f;

		// Token: 0x04006203 RID: 25091
		private const float BubbleInterval = 10f;

		// Token: 0x04006204 RID: 25092
		private const float IntroduceBubbleInterval = 30f;

		// Token: 0x04006205 RID: 25093
		private float _timer;

		// Token: 0x04006206 RID: 25094
		private bool _onlyIntroduceDialog;

		// Token: 0x04006207 RID: 25095
		private bool _firstIntroduceDialog;

		// Token: 0x04006208 RID: 25096
		private bool _secondIntroduceDialog;

		// Token: 0x04006209 RID: 25097
		private bool _seasonDialog;

		// Token: 0x0400620A RID: 25098
		private bool _favoriteDialog;

		// Token: 0x0400620B RID: 25099
		private bool _favorDialog;

		// Token: 0x0400620C RID: 25100
		private bool _remainDialog;
	}
}
