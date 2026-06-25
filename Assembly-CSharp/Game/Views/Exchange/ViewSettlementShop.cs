using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Views.CharacterMenu;
using Game.Views.Select;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using GameData.Domains.Information;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Merchant;
using GameData.Domains.Organization;
using GameData.Domains.Organization.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Domains.Taiwu.ExchangeSystem;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Exchange
{
	// Token: 0x02000A33 RID: 2611
	public class ViewSettlementShop : ViewExchangeBase
	{
		// Token: 0x17000E02 RID: 3586
		// (get) Token: 0x06008076 RID: 32886 RVA: 0x003BC4A0 File Offset: 0x003BA6A0
		protected override string SortSaveKeyPrefix
		{
			get
			{
				return "ViewSettlementShop";
			}
		}

		// Token: 0x06008077 RID: 32887 RVA: 0x003BC4A8 File Offset: 0x003BA6A8
		public static void ShowOrUpdate(OpenShopEventArguments openShopEventArguments)
		{
			bool isShowing = UIElement.SettlementShop.IsShowing;
			if (isShowing)
			{
				ViewSettlementShop self = UIElement.SettlementShop.UiBaseAs<ViewSettlementShop>();
				self.EventProcessingTargetPageClicked = true;
				bool flag = openShopEventArguments.CurrPage < 0;
				if (flag)
				{
					UIManager.Instance.HideUI(UIElement.SettlementShop);
				}
				self.UpdateTog(openShopEventArguments);
			}
			else
			{
				UIElement.SettlementShop.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("OpenShopEventArguments", openShopEventArguments));
				UIManager.Instance.ShowUI(UIElement.SettlementShop, true);
			}
		}

		// Token: 0x17000E03 RID: 3587
		// (get) Token: 0x06008078 RID: 32888 RVA: 0x003BC52A File Offset: 0x003BA72A
		public override Exchange Exchange
		{
			get
			{
				return this._displayData.Exchange;
			}
		}

		// Token: 0x17000E04 RID: 3588
		// (get) Token: 0x06008079 RID: 32889 RVA: 0x003BC537 File Offset: 0x003BA737
		public override SecretInformationDisplayPackage SecretInformationDisplayPackage
		{
			get
			{
				return this._displayData.SecretInformationDisplayPackage;
			}
		}

		// Token: 0x17000E05 RID: 3589
		// (get) Token: 0x0600807A RID: 32890 RVA: 0x003BC544 File Offset: 0x003BA744
		public override CharacterDisplayData TargetDisplayData
		{
			get
			{
				return this._displayData.TargetCharacterDisplayData;
			}
		}

		// Token: 0x17000E06 RID: 3590
		// (get) Token: 0x0600807B RID: 32891 RVA: 0x003BC551 File Offset: 0x003BA751
		public override CharacterDisplayData TaiwuDisplayData
		{
			get
			{
				return this._displayData.TaiwuDisplayData;
			}
		}

		// Token: 0x17000E07 RID: 3591
		// (get) Token: 0x0600807C RID: 32892 RVA: 0x003BC55E File Offset: 0x003BA75E
		protected override ItemListScroll.EColumnType ColumnType
		{
			get
			{
				return ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value;
			}
		}

		// Token: 0x0600807D RID: 32893 RVA: 0x003BC564 File Offset: 0x003BA764
		public override IReadOnlyList<ITradeableContent> GetSelfTradeableList(int index)
		{
			if (!true)
			{
			}
			IReadOnlyList<ITradeableContent> readOnlyList;
			switch (index)
			{
			case 0:
				readOnlyList = this._displayData.TaiwuInventoryItemDisplayDataList;
				break;
			case 1:
				readOnlyList = this._displayData.TaiwuWarehouseItemDisplayDataList;
				break;
			case 2:
				readOnlyList = this._displayData.TaiwuTreasuryItemDisplayDataList;
				break;
			case 3:
				readOnlyList = this._displayData.TaiwuStockItemDisplayDataList;
				break;
			default:
			{
				KidnapMenuDisplayData taiwuKidnapMenuDisplayData = this._displayData.TaiwuKidnapMenuDisplayData;
				readOnlyList = ((taiwuKidnapMenuDisplayData != null) ? taiwuKidnapMenuDisplayData.KidnapCharDisplayDataList : null);
				break;
			}
			}
			if (!true)
			{
			}
			return ((readOnlyList != null) ? (from x in readOnlyList
			where this.CheckItemCanTransfer(x, this._displayData.TaiwuDisplayData)
			select x).ToArray<ITradeableContent>() : null) ?? Array.Empty<ITradeableContent>();
		}

		// Token: 0x0600807E RID: 32894 RVA: 0x003BC60C File Offset: 0x003BA80C
		public override IReadOnlyList<ITradeableContent> GetTargetTradeableList(int index)
		{
			sbyte settlementTreasuryLayerIndex = ViewSettlementShop.SettlementTreasuryLayerIndex;
			if (!true)
			{
			}
			List<ItemDisplayData> list;
			if (settlementTreasuryLayerIndex != 0)
			{
				if (settlementTreasuryLayerIndex != 1)
				{
					list = this._displayData.TreasuryData.SettlementTreasuryDisplayDataListHigh;
				}
				else
				{
					list = this._displayData.TreasuryData.SettlementTreasuryDisplayDataListMid;
				}
			}
			else
			{
				list = this._displayData.TreasuryData.SettlementTreasuryDisplayDataListLow;
			}
			if (!true)
			{
			}
			IReadOnlyList<ITradeableContent> readOnlyList = list;
			return readOnlyList ?? Array.Empty<ITradeableContent>();
		}

		// Token: 0x0600807F RID: 32895 RVA: 0x003BC677 File Offset: 0x003BA877
		protected override void Awake()
		{
			this.ConfirmOnHideKey = LanguageKey.LK_Exchange_Title;
			base.Awake();
			this.exchangeContainer.exchangeBack.SetNpc(0);
			this.exchangeContainer.exchangeFront.SetNpc(0);
			this.AwakeShop();
		}

		// Token: 0x06008080 RID: 32896 RVA: 0x003BC6B7 File Offset: 0x003BA8B7
		private void OnEnable()
		{
			this._submitting = false;
		}

		// Token: 0x06008081 RID: 32897 RVA: 0x003BC6C4 File Offset: 0x003BA8C4
		private void SetTargetPage(sbyte newTog)
		{
			this._shopEventArguments.CurrPage = newTog;
			if (!true)
			{
			}
			CharacterDisplayData displayData;
			if (newTog != 0)
			{
				if (newTog != 1)
				{
					displayData = this._displayData.TreasuryData.SettlementGuardDisplayDataListHigh;
				}
				else
				{
					displayData = this._displayData.TreasuryData.SettlementGuardDisplayDataListMid;
				}
			}
			else
			{
				displayData = this._displayData.TreasuryData.SettlementGuardDisplayDataListLow;
			}
			if (!true)
			{
			}
			this.SetTargetCharacterDisplayData(displayData, LanguageKey.Invalid);
			this.Exchange.SetSettlementTreasury(this._displayData.TreasuryData.Treasuries[(int)newTog]);
			this.RefreshTargetItems();
		}

		// Token: 0x06008082 RID: 32898 RVA: 0x003BC760 File Offset: 0x003BA960
		public override void Submit()
		{
			bool flag = this.IsBreaking && this.Exchange.TargetContentList.Count == 0;
			if (flag)
			{
				UIElement dialog = UIElement.Dialog;
				ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
				string key = "Cmd";
				DialogCmd dialogCmd = new DialogCmd();
				dialogCmd.Type = 1;
				dialogCmd.Title = LanguageKey.LK_Building_LeaveTreasury.Tr();
				dialogCmd.Content = LanguageKey.LK_Building_LeaveTreasury_Desc.Tr();
				dialogCmd.Yes = new Action(this.QuickHideImpl);
				dialogCmd.No = delegate()
				{
				};
				dialog.SetOnInitArgs(argumentBox.SetObject(key, dialogCmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			else
			{
				this._submitting = true;
				TaiwuDomainMethod.Call.ConfirmExchange(this.Exchange);
				long taiwuTotalValueWithAdvantage = this.Exchange.TaiwuTotalValueWithAdvantage;
				long targetTotalValueWithAdvantage = this.Exchange.TargetTotalValueWithAdvantage;
				bool isExchange = taiwuTotalValueWithAdvantage >= targetTotalValueWithAdvantage && targetTotalValueWithAdvantage > 0L;
				bool flag2 = isExchange && !this.IsBreaking;
				if (flag2)
				{
					this.Reset();
				}
				else
				{
					this.QuickHideImpl();
				}
				this._submitting = false;
			}
		}

		// Token: 0x06008083 RID: 32899 RVA: 0x003BC88C File Offset: 0x003BAA8C
		public override void OnInit(ArgumentBox argsBox)
		{
			this.ExchangeValueType = 9;
			argsBox.Get<OpenShopEventArguments>("OpenShopEventArguments", out this._shopEventArguments);
			this.Init(this._shopEventArguments);
			base.OnInit(argsBox);
			this.exchangeContainer.confirmButtonText.text = LanguageKey.LK_Exchange_Confirm.Tr();
			TooltipInvoker tooltipInvoker = this.selfAdvantage;
			ArgumentBox argumentBox;
			if ((argumentBox = tooltipInvoker.RuntimeParam) == null)
			{
				argumentBox = (tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>());
			}
			argumentBox.Set("arg0", this.selfAdvantage.PresetParam[0] = LanguageKey.LK_Exchange_Advantage.Tr());
			tooltipInvoker = this.targetAdvantage;
			ArgumentBox argumentBox2;
			if ((argumentBox2 = tooltipInvoker.RuntimeParam) == null)
			{
				argumentBox2 = (tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>());
			}
			argumentBox2.Set("arg0", this.targetAdvantage.PresetParam[0] = LanguageKey.LK_Exchange_Advantage.Tr());
			this.exchangeContainer.secretText.text = LanguageKey.LK_Exchange_Approve.Tr();
			this.exchangeContainer.AddSwitchToggleListener();
			bool flag = !this._firstEnter;
			if (flag)
			{
				this._firstEnter = true;
				GlobalDomainMethod.Call.InvokeGuidingTrigger(73);
			}
		}

		// Token: 0x06008084 RID: 32900 RVA: 0x003BC9B4 File Offset: 0x003BABB4
		private void Init(OpenShopEventArguments openShopEventArguments)
		{
			this.EventProcessingTargetPageClicked = false;
			this._shopEventArguments = openShopEventArguments;
			sbyte currPage = openShopEventArguments.CurrPage;
			bool flag = currPage >= 0 && currPage <= 2;
			if (flag)
			{
				this.targetPage.SetWithoutNotify((int)(ViewSettlementShop.SettlementTreasuryLayerIndex = openShopEventArguments.CurrPage));
			}
		}

		// Token: 0x06008085 RID: 32901 RVA: 0x003BCA01 File Offset: 0x003BAC01
		private void UpdateTog(OpenShopEventArguments openShopEventArguments)
		{
			this.Init(openShopEventArguments);
			this.RefreshAvatar();
			this.Refresh();
		}

		// Token: 0x06008086 RID: 32902 RVA: 0x003BCA1C File Offset: 0x003BAC1C
		private void RefreshAvatar()
		{
			ExchangeDisplayData displayData = this._displayData;
			sbyte currPage = this._shopEventArguments.CurrPage;
			if (!true)
			{
			}
			CharacterDisplayData targetCharacterDisplayData;
			if (currPage != 0)
			{
				if (currPage != 1)
				{
					targetCharacterDisplayData = this._displayData.TreasuryData.SettlementGuardDisplayDataListHigh;
				}
				else
				{
					targetCharacterDisplayData = this._displayData.TreasuryData.SettlementGuardDisplayDataListMid;
				}
			}
			else
			{
				targetCharacterDisplayData = this._displayData.TreasuryData.SettlementGuardDisplayDataListLow;
			}
			if (!true)
			{
			}
			displayData.TargetCharacterDisplayData = targetCharacterDisplayData;
			this.SetTargetCharacterDisplayData(this._displayData.TargetCharacterDisplayData, LanguageKey.Invalid);
			this.exchangeContainer.avatarBtn.interactable = (this._displayData.TargetCharacterDisplayData.CreatingType == 1);
		}

		// Token: 0x06008087 RID: 32903 RVA: 0x003BCAC8 File Offset: 0x003BACC8
		private string GetSettlementName(WorldMapModel worldMapModel, MapBlockItem blockConfig)
		{
			EMapBlockType type = blockConfig.Type;
			if (!true)
			{
			}
			string result;
			switch (type)
			{
			case EMapBlockType.City:
				result = blockConfig.Name;
				break;
			case EMapBlockType.Sect:
				result = Config.Organization.Instance[this.Exchange.AdvantageSummary.NpcOrganization.OrgTemplateId].Name;
				break;
			case EMapBlockType.Town:
				result = worldMapModel.GetCurBlockName();
				break;
			default:
				result = "";
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06008088 RID: 32904 RVA: 0x003BCB3F File Offset: 0x003BAD3F
		protected override void RequestData()
		{
			TaiwuDomainMethod.AsyncCall.GetExchangeDisplayData(this, (int)this._shopEventArguments.SettlementId, EExchangeType.Settlement, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._displayData);
				this._displayData.Exchange.AdvantageSummary.NpcOrganization.Grade = (sbyte)GlobalConfig.Instance.ExchangeTreasuryLevelGrade[(int)ViewSettlementShop.SettlementTreasuryLayerIndex];
				this.RefreshData(this._displayData);
			});
		}

		// Token: 0x06008089 RID: 32905 RVA: 0x003BCB64 File Offset: 0x003BAD64
		public void RefreshData(ExchangeDisplayData displayData)
		{
			this._displayData = displayData;
			this.Exchange.SetSettlementTreasury(this._displayData.TreasuryData.Treasuries[(int)this._shopEventArguments.CurrPage]);
			this.RefreshAvatar();
			WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
			MapBlockData blockData = worldMapModel.CurrentBlockData;
			MapBlockItem blockConfig = MapBlock.Instance[blockData.TemplateId];
			bool flag = this.exchangeContainer.secret;
			if (flag)
			{
				this.exchangeContainer.secret.gameObject.SetActive(blockConfig.Type == EMapBlockType.Sect);
			}
			this.exchangeContainer.titleTarget.text = LanguageKey.LK_Exchange_SubTitle_Settlement.TrFormat(this.GetSettlementName(worldMapModel, blockConfig));
			this.exchangeContainer.titleTaiwu.text = SingletonObject.getInstance<BasicGameData>().TaiwuMonasticTitleOrDisplayName;
			this.SetWarehouseCanInteract(this._displayData.CanTransferItemToWarehouse);
			this.SetExchangeData();
			this.RefreshSelfColumnDefinitions(this.exchangeContainer.currPage.GetActiveIndex());
			this.Refresh();
			this.InitShopRelatedData();
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x0600808A RID: 32906 RVA: 0x003BCC88 File Offset: 0x003BAE88
		protected override void RefreshValues()
		{
			this.RefreshShop();
			this.exchangeContainer.targetValue1.text = ViewExchangeBase.GetWeightString(this.Exchange.TargetCurLoad, this.Exchange.TargetMaxLoad, this.Exchange.TargetCurLoadPreview, this.Exchange.TargetMaxLoadPreview, LanguageKey.LK_Exchange_Weight_Value);
			int cmp = this.Exchange.AdvantageSummary.TargetAdvantage - this.Exchange.AdvantageSummary.TaiwuAdvantage;
			TMP_Text targetValue = this.exchangeContainer.targetValue2;
			if (!true)
			{
			}
			string arg;
			if (cmp <= 0)
			{
				if (cmp != 0)
				{
					arg = "brightred";
				}
				else
				{
					arg = "grey";
				}
			}
			else
			{
				arg = "brightblue";
			}
			if (!true)
			{
			}
			targetValue.text = LanguageKey.LK_Exchange_Advantage_Value.TrFormat(arg, this.Exchange.AdvantageSummary.TargetAdvantage).ColorReplace();
			this.exchangeContainer.targetValue3.text = LanguageKey.LK_Exchange_Value_Value.TrFormat(this.Exchange.TargetTotalValueWithAdvantage);
			TMP_Text selfValue = this.exchangeContainer.selfValue1;
			int activeIndex = this.exchangeContainer.currPage.GetActiveIndex();
			selfValue.text = ((activeIndex == 0 || activeIndex == 5) ? ViewExchangeBase.GetWeightString(this.Exchange.TaiwuInventoryCurLoad, this.Exchange.TaiwuInventoryMaxLoad, this.Exchange.TaiwuInventoryCurLoadPreview, this.Exchange.TaiwuInventoryMaxLoadPreview, LanguageKey.LK_Exchange_Weight_Value) : ViewExchangeBase.GetWeightString(this.Exchange.TaiwuWarehouseCurLoad, this.Exchange.TaiwuWarehouseMaxLoad, this.Exchange.TaiwuWarehouseCurLoadPreview, this.Exchange.TaiwuWarehouseMaxLoadPreview, LanguageKey.LK_Exchange_Weight_Value));
			TMP_Text selfValue2 = this.exchangeContainer.selfValue2;
			int num = -cmp;
			if (!true)
			{
			}
			if (num <= 0)
			{
				if (num != 0)
				{
					arg = "brightred";
				}
				else
				{
					arg = "grey";
				}
			}
			else
			{
				arg = "brightblue";
			}
			if (!true)
			{
			}
			selfValue2.text = LanguageKey.LK_Exchange_Advantage_Value.TrFormat(arg, this.Exchange.AdvantageSummary.TaiwuAdvantage).ColorReplace();
			this.exchangeContainer.selfValue3.text = LanguageKey.LK_Exchange_Value_Value.TrFormat(this.Exchange.TaiwuTotalValueWithAdvantage);
			TooltipInvoker tooltipInvoker = this.selfAdvantage;
			ArgumentBox argumentBox;
			if ((argumentBox = tooltipInvoker.RuntimeParam) == null)
			{
				argumentBox = (tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>());
			}
			argumentBox.Set("arg1", this.selfAdvantage.PresetParam[1] = this.Exchange.AdvantageSummary.TaiwuAdvantageDesc(this.Exchange.TaiwuTotalValue, this.Exchange.TaiwuTotalValueWithAdvantage));
			tooltipInvoker = this.targetAdvantage;
			ArgumentBox argumentBox2;
			if ((argumentBox2 = tooltipInvoker.RuntimeParam) == null)
			{
				argumentBox2 = (tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>());
			}
			argumentBox2.Set("arg1", this.targetAdvantage.PresetParam[1] = this.Exchange.AdvantageSummary.TargetAdvantageDesc(this.Exchange.TargetTotalValue, this.Exchange.TargetTotalValueWithAdvantage));
		}

		// Token: 0x17000E08 RID: 3592
		// (get) Token: 0x0600808B RID: 32907 RVA: 0x003BCF89 File Offset: 0x003BB189
		protected override string ConfirmOnHideDesc
		{
			get
			{
				return LanguageKey.LK_Exchange_CancelDesc.Tr();
			}
		}

		// Token: 0x0600808C RID: 32908 RVA: 0x003BCF98 File Offset: 0x003BB198
		public override void QuickHideImpl()
		{
			bool flag = !this._submitting;
			if (flag)
			{
				this.Exchange.Clear();
				TaiwuDomainMethod.Call.ConfirmExchange(this.Exchange);
			}
			TaiwuEventDomainMethod.Call.TriggerListener("ShopActionComplete", true);
			base.QuickHideImpl();
		}

		// Token: 0x17000E09 RID: 3593
		// (get) Token: 0x0600808D RID: 32909 RVA: 0x003BCFE0 File Offset: 0x003BB1E0
		public override bool CanConfirmExchange
		{
			get
			{
				return this.Exchange.CanConfirmExchange || (this.Exchange.TaiwuContentList.Count == 0 && this.Exchange.TargetContentList.Count != 0);
			}
		}

		// Token: 0x0600808E RID: 32910 RVA: 0x003BD01C File Offset: 0x003BB21C
		public void AwakeShop()
		{
			this.targetPage.Init(-1);
			this.targetPage.OnActiveIndexChange += delegate(int newTog, int _)
			{
				bool eventProcessingTargetPageClicked = this.EventProcessingTargetPageClicked;
				if (!eventProcessingTargetPageClicked)
				{
					this.targetPage.SetWithoutNotify((int)ViewSettlementShop.SettlementTreasuryLayerIndex);
					TaiwuEventDomainMethod.Call.SettlementTreasuryBuildingClicked(this.Exchange.AdvantageSummary.NpcOrganization.SettlementId, 0, (sbyte)newTog);
				}
			};
			this.record.onClick.ResetListener(new Action(this.OnClickBtnTreasuryRecord));
			this.supply.onClick.ResetListener(new Action(this.OnClickButtonReplenish));
		}

		// Token: 0x0600808F RID: 32911 RVA: 0x003BD08C File Offset: 0x003BB28C
		protected override void AwakeAvatarBtn()
		{
			this.exchangeContainer.avatarBtn.onClick.ResetListener(delegate()
			{
				base.ShowCharacterMenu(this._displayData.TargetCharacterDisplayData);
			});
			this.exchangeContainer.btnOpenCharMenu.onClick.ResetListener(delegate()
			{
				base.ShowCharacterMenu(this._displayData.TargetCharacterDisplayData);
			});
		}

		// Token: 0x06008090 RID: 32912 RVA: 0x003BD0E0 File Offset: 0x003BB2E0
		private void OnClickBtnTreasuryRecord()
		{
			ArgumentBox args = EasyPool.Get<ArgumentBox>().Set("SettlementId", this.Exchange.AdvantageSummary.NpcOrganization.SettlementId);
			UIElement.SettlementTreasuryRecords.SetOnInitArgs(args);
			UIManager.Instance.MaskUI(UIElement.SettlementTreasuryRecords);
		}

		// Token: 0x06008091 RID: 32913 RVA: 0x003BD130 File Offset: 0x003BB330
		private void OnClickButtonReplenish()
		{
			OrganizationDomainMethod.AsyncCall.GetSettlementTreasuryDisplayData(this, this.Exchange.AdvantageSummary.NpcOrganization.SettlementId, this.Exchange.LayerIndex, delegate(int offset, RawDataPool pool)
			{
				SettlementTreasuryDisplayData settlementTreasuryDisplayData = default(SettlementTreasuryDisplayData);
				Serializer.Deserialize(pool, offset, ref settlementTreasuryDisplayData);
				UIElement.SettlementTreasuryReplenish.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("InfluenceRefreshTime", (int)settlementTreasuryDisplayData.InfluenceRefreshTime).Set<Inventory>("SupplyItems", settlementTreasuryDisplayData.SupplyItems).SetObject("SupplyCounts", settlementTreasuryDisplayData.SupplyCounts));
				UIManager.Instance.MaskUI(UIElement.SettlementTreasuryReplenish);
			});
		}

		// Token: 0x06008092 RID: 32914 RVA: 0x003BD184 File Offset: 0x003BB384
		private void InitShopRelatedData()
		{
			this.treasuryLevel.text = this.DisplayLevelStr;
			this.treasuryLevelIcon.SetSprite(this.DisplayLevelIcon, false, null);
			this.fameProperty.Set(this._displayData.TaiwuDisplayData, true);
		}

		// Token: 0x17000E0A RID: 3594
		// (get) Token: 0x06008093 RID: 32915 RVA: 0x003BD1D0 File Offset: 0x003BB3D0
		public string DisplayLevelStr
		{
			get
			{
				string str = this.DisplaySupplyLevel.ToString();
				int supplyLevelSpriteIndex = this.SupplyLevelSpriteIndex;
				if (!true)
				{
				}
				string color;
				if (supplyLevelSpriteIndex != 0)
				{
					if (supplyLevelSpriteIndex != 1)
					{
						color = "brightblue";
					}
					else
					{
						color = "pinkyellow";
					}
				}
				else
				{
					color = "brightred";
				}
				if (!true)
				{
				}
				return str.SetColor(color);
			}
		}

		// Token: 0x17000E0B RID: 3595
		// (get) Token: 0x06008094 RID: 32916 RVA: 0x003BD225 File Offset: 0x003BB425
		public string DisplayLevelIcon
		{
			get
			{
				return string.Format("shop_treasury_scale_{0}", this.SupplyLevelSpriteIndex);
			}
		}

		// Token: 0x17000E0C RID: 3596
		// (get) Token: 0x06008095 RID: 32917 RVA: 0x003BD23C File Offset: 0x003BB43C
		private int DisplaySupplyLevel
		{
			get
			{
				return this._displayData.TreasuryData.SupplyLevel + 1;
			}
		}

		// Token: 0x17000E0D RID: 3597
		// (get) Token: 0x06008096 RID: 32918 RVA: 0x003BD250 File Offset: 0x003BB450
		private int SupplyLevelSpriteIndex
		{
			get
			{
				return Math.Clamp(this._displayData.TreasuryData.SupplyLevel, 0, 2);
			}
		}

		// Token: 0x17000E0E RID: 3598
		// (get) Token: 0x06008097 RID: 32919 RVA: 0x003BD269 File Offset: 0x003BB469
		public bool IsBreaking
		{
			get
			{
				return !this._canEnterLayer[(int)ViewSettlementShop.SettlementTreasuryLayerIndex];
			}
		}

		// Token: 0x06008098 RID: 32920 RVA: 0x003BD27C File Offset: 0x003BB47C
		private void RefreshShop()
		{
			bool switchPageEnabled = this.Exchange.ExchangeItemList.Count == 0;
			bool isSect = Config.Organization.Instance[this._displayData.TreasuryData.SettlementGuardDisplayDataListLow.OrgInfo.OrgTemplateId].IsSect;
			this.mainIcon.sprite = (isSect ? this.sect : this.city);
			this.SetMouseTipSettlementTreasuryOrPrisonLayer(isSect);
			this.targetPage.SetInteractable(switchPageEnabled && !this.IsBreaking);
			this.exchangeContainer.secretText.text = ((this.Exchange.AdvantageSummary.ApprovingCharId != -1) ? NameCenter.GetMonasticTitleOrDisplayName(ref this._nameRelatedData, false, false) : LanguageKey.LK_Exchange_Approve.Tr());
			TMP_Text confirmButtonText = this.exchangeContainer.confirmButtonText;
			bool flag = this.Exchange.TargetContentList.Count == 0;
			bool flag2 = this.Exchange.TaiwuContentList.Count == 0;
			if (!true)
			{
			}
			string text;
			if (flag)
			{
				if (!flag2)
				{
					text = LanguageKey.LK_Exchange_Confirm_As_Gift.Tr();
					goto IL_12D;
				}
			}
			else if (flag2)
			{
				text = LanguageKey.LK_Exchange_Steal.Tr();
				goto IL_12D;
			}
			text = LanguageKey.LK_Exchange_Confirm.Tr();
			IL_12D:
			if (!true)
			{
			}
			confirmButtonText.text = text;
		}

		// Token: 0x06008099 RID: 32921 RVA: 0x003BD3C4 File Offset: 0x003BB5C4
		public override void AddSecretOrPopSelections()
		{
			bool flag = this.Exchange.CharId == -1;
			if (flag)
			{
				this.<AddSecretOrPopSelections>g__ShowSelectSecret|69_1();
			}
			else
			{
				List<ViewPopupMenu.BtnData> btnList = new List<ViewPopupMenu.BtnData>
				{
					new ViewPopupMenu.BtnData(LanguageKey.LK_Exchange_Secret_Remove.Tr(), true, EItemMenuDisplayOrder.Discard, delegate()
					{
						this.<AddSecretOrPopSelections>g__SetSecret|69_2(null);
						ViewSettlementShop.<AddSecretOrPopSelections>g__OnCancel|69_0();
					}, null, null, false),
					new ViewPopupMenu.BtnData(LanguageKey.LK_Exchange_Secret_Relpace.Tr(), true, EItemMenuDisplayOrder.Info, new Action(this.<AddSecretOrPopSelections>g__ShowSelectSecret|69_1), null, null, false)
				};
				RectTransform itemRectTrans = this.exchangeContainer.secret.transform as RectTransform;
				Vector3 itemScreenPos = UIManager.Instance.UiCamera.WorldToScreenPoint(itemRectTrans.position);
				Vector3 mouseScreenPos = Input.mousePosition;
				itemScreenPos.x = mouseScreenPos.x;
				UIElement.PopupMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("BtnInfo", btnList).SetObject("ScreenPos", itemScreenPos).SetObject("ItemSize", itemRectTrans.rect.size).SetObject("OnCancel", new Action(ViewSettlementShop.<AddSecretOrPopSelections>g__OnCancel|69_0)));
				UIManager.Instance.ShowUI(UIElement.PopupMenu, true);
			}
		}

		// Token: 0x0600809A RID: 32922 RVA: 0x003BD4FC File Offset: 0x003BB6FC
		private void SetMouseTipSettlementTreasuryOrPrisonLayer(bool isSect)
		{
			int value = this._displayData.TreasuryData.DebtOrSupport;
			for (int i = 0; i < this.displayers.Length; i++)
			{
				if (!true)
				{
				}
				CharacterDisplayData characterDisplayData;
				if (i != 0)
				{
					if (i != 1)
					{
						characterDisplayData = this._displayData.TreasuryData.SettlementGuardDisplayDataListHigh;
					}
					else
					{
						characterDisplayData = this._displayData.TreasuryData.SettlementGuardDisplayDataListMid;
					}
				}
				else
				{
					characterDisplayData = this._displayData.TreasuryData.SettlementGuardDisplayDataListLow;
				}
				if (!true)
				{
				}
				CharacterDisplayData guardianCharacter = characterDisplayData;
				this.displayers[i].Type = TipType.SettlementTreasuryOrPrisonLayer;
				TooltipInvoker tooltipInvoker = this.displayers[i];
				ArgumentBox argumentBox;
				if ((argumentBox = tooltipInvoker.RuntimeParam) == null)
				{
					argumentBox = (tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>());
				}
				ArgumentBox argumentBox2 = argumentBox.Set("layerIndex", i).Set("isSect", isSect);
				bool[] canEnterLayer = this._canEnterLayer;
				int num = i;
				int num2 = value;
				bool flag = i < 2;
				if (!true)
				{
				}
				int num3;
				if (isSect)
				{
					if (!flag)
					{
						num3 = GlobalConfig.Instance.TreasuryRquireApprovingHigh * 10;
					}
					else
					{
						num3 = GlobalConfig.Instance.TreasuryRquireApprovingMid * 10;
					}
				}
				else if (!flag)
				{
					num3 = GlobalConfig.Instance.TreasuryRquireSpiritualDebtHigh;
				}
				else
				{
					num3 = GlobalConfig.Instance.TreasuryRquireSpiritualDebtMid;
				}
				if (!true)
				{
				}
				argumentBox2.Set("isDebtOrSupportEnough", canEnterLayer[num] = (num2 >= num3)).Set<CharacterDisplayData>("guardianCharacterDisplayData", guardianCharacter);
			}
			this._canEnterLayer[0] = true;
			bool flag2 = !this.IsBreaking && this.Exchange.ExchangeItemList.Count > 0;
			if (flag2)
			{
				foreach (TooltipInvoker displayer in this.displayers)
				{
					displayer.Type = TipType.SingleDesc;
				}
			}
		}

		// Token: 0x0600809B RID: 32923 RVA: 0x003BD6C8 File Offset: 0x003BB8C8
		protected override void OnClickItem(ItemListScroll scroll, ITradeableContent itemData, RowItemLine rowItemLine, Action<RowItemLine, int> action)
		{
			Action <>9__1;
			scroll.HandleClickItem(itemData, rowItemLine, delegate(RowItemLine x)
			{
				bool flag = itemData.Amount == 1;
				if (flag)
				{
					TipType type = rowItemLine.TipDisplayer.Type;
					action(x, 1);
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
					bool isSelected = scroll == this.exchangeContainer.selfExchangeList;
					int initSelectCount = this.GetInitSelectCount(itemData, isSelected);
					int minCount = (scroll == this.exchangeContainer.targetItemList && itemData.IsResource) ? GlobalConfig.Instance.SettlementTreasuryGetResourceMinValue : 1;
					scroll.SetItemToSelectCountMode(x, delegate(int count)
					{
						action(x, count);
						this.Refresh();
					}, delegate
					{
					}, initSelectCount, 0, minCount, null, false, null, false);
				}
			});
		}

		// Token: 0x060080A3 RID: 32931 RVA: 0x003BD826 File Offset: 0x003BBA26
		[CompilerGenerated]
		internal static void <AddSecretOrPopSelections>g__OnCancel|69_0()
		{
		}

		// Token: 0x060080A4 RID: 32932 RVA: 0x003BD829 File Offset: 0x003BBA29
		[CompilerGenerated]
		private void <AddSecretOrPopSelections>g__ShowSelectSecret|69_1()
		{
			OrganizationDomainMethod.AsyncCall.GetSettlementApproveTaiwuMembers(this, this.Exchange.AdvantageSummary.NpcOrganization.SettlementId, delegate(int offset, RawDataPool pool)
			{
				OrganizationMemberDisplayDataForGeneralScrollList[] result = Array.Empty<OrganizationMemberDisplayDataForGeneralScrollList>();
				Serializer.Deserialize(pool, offset, ref result);
				CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.ApproveRate);
				config.InteractionMode = ESelectCharacterInteractionMode.Slot;
				config.SelectionMode = ESelectCharacterSelectionMode.Single;
				config.TargetCount = 1;
				config.FilterMenuIds = new List<ESelectCharacterFilterMenuId>
				{
					ESelectCharacterFilterMenuId.Gender,
					ESelectCharacterFilterMenuId.BehaviorType,
					ESelectCharacterFilterMenuId.Relation,
					ESelectCharacterFilterMenuId.AdoreRelation,
					ESelectCharacterFilterMenuId.EnemyRelation
				};
				ViewSelectCharacter.Show(config, result, delegate(List<int> selected)
				{
					OrganizationMemberDisplayDataForGeneralScrollList approveChar = (selected != null && selected.Count > 0) ? result.FirstOrDefault((OrganizationMemberDisplayDataForGeneralScrollList x) => x.CharacterDisplayDataForGeneralScrollList.CharacterId == selected[0]) : null;
					this.Exchange.SetApproveChar((approveChar != null) ? approveChar.CharacterDisplayDataForGeneralScrollList.CharacterId : -1, (approveChar != null) ? approveChar.ApprovingRate : 0);
					this._nameRelatedData = ((approveChar != null) ? approveChar.CharacterDisplayDataForGeneralScrollList.NameData : default(NameRelatedData));
					this.exchangeContainer.secretText.text = ((this.Exchange.AdvantageSummary.ApprovingCharId != -1) ? NameCenter.GetMonasticTitleOrDisplayName(ref this._nameRelatedData, false, false) : LanguageKey.LK_Exchange_Approve.Tr());
					this.Refresh();
				}, null, false);
			});
		}

		// Token: 0x060080A6 RID: 32934 RVA: 0x003BD8F8 File Offset: 0x003BBAF8
		[CompilerGenerated]
		private void <AddSecretOrPopSelections>g__SetSecret|69_2(SecretInformationDisplayData selectedSecret)
		{
			this.Exchange.SetSecret(selectedSecret);
			Image secretImg = this.exchangeContainer.secretImg;
			Sprite sprite;
			if (selectedSecret != null)
			{
				SecretInformationId secretInformationId = selectedSecret.SecretInformationId;
				if (secretInformationId.Valid)
				{
					sprite = this.exchangeContainer.secretIsSome;
					goto IL_44;
				}
			}
			sprite = this.exchangeContainer.secretIsNone;
			IL_44:
			secretImg.sprite = sprite;
			this.Refresh();
		}

		// Token: 0x04006230 RID: 25136
		public static sbyte SettlementTreasuryLayerIndex;

		// Token: 0x04006231 RID: 25137
		public static bool IsExistTradeItems;

		// Token: 0x04006232 RID: 25138
		[SerializeField]
		protected TooltipInvoker selfAdvantage;

		// Token: 0x04006233 RID: 25139
		[SerializeField]
		protected TooltipInvoker targetAdvantage;

		// Token: 0x04006234 RID: 25140
		private ExchangeDisplayData _displayData;

		// Token: 0x04006235 RID: 25141
		private OpenShopEventArguments _shopEventArguments;

		// Token: 0x04006236 RID: 25142
		[NonSerialized]
		public bool EventProcessingTargetPageClicked;

		// Token: 0x04006237 RID: 25143
		private bool _submitting;

		// Token: 0x04006238 RID: 25144
		private bool _firstEnter;

		// Token: 0x04006239 RID: 25145
		[SerializeField]
		private TMP_Text treasuryLevel;

		// Token: 0x0400623A RID: 25146
		[SerializeField]
		private CImage treasuryLevelIcon;

		// Token: 0x0400623B RID: 25147
		[SerializeField]
		private CImage mainIcon;

		// Token: 0x0400623C RID: 25148
		[SerializeField]
		private CButton record;

		// Token: 0x0400623D RID: 25149
		[SerializeField]
		private CButton supply;

		// Token: 0x0400623E RID: 25150
		[SerializeField]
		private CToggleGroup targetPage;

		// Token: 0x0400623F RID: 25151
		[SerializeField]
		private Fame fameProperty;

		// Token: 0x04006240 RID: 25152
		[SerializeField]
		private Sprite city;

		// Token: 0x04006241 RID: 25153
		[SerializeField]
		private Sprite sect;

		// Token: 0x04006242 RID: 25154
		private bool[] _canEnterLayer = new bool[3];

		// Token: 0x04006243 RID: 25155
		[SerializeField]
		private TooltipInvoker[] displayers;

		// Token: 0x04006244 RID: 25156
		private NameRelatedData _nameRelatedData;
	}
}
