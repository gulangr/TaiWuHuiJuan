using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using Game.Components.ListStyleGeneralScroll.Item;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using GameData.Domains.Information;
using GameData.Domains.Item.Display;
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
	// Token: 0x02000A30 RID: 2608
	public class ViewExchange : ViewExchangeBase
	{
		// Token: 0x17000DE4 RID: 3556
		// (get) Token: 0x06007FD6 RID: 32726 RVA: 0x003B8592 File Offset: 0x003B6792
		protected override string SortSaveKeyPrefix
		{
			get
			{
				return "ViewExchange";
			}
		}

		// Token: 0x17000DE5 RID: 3557
		// (get) Token: 0x06007FD7 RID: 32727 RVA: 0x003B8599 File Offset: 0x003B6799
		protected override LanguageKey ExchangeNotFair
		{
			get
			{
				return LanguageKey.LK_Exchange_Not_Fair_Exchange;
			}
		}

		// Token: 0x17000DE6 RID: 3558
		// (get) Token: 0x06007FD8 RID: 32728 RVA: 0x003B85A0 File Offset: 0x003B67A0
		public override Exchange Exchange
		{
			get
			{
				return this._displayData.Exchange;
			}
		}

		// Token: 0x17000DE7 RID: 3559
		// (get) Token: 0x06007FD9 RID: 32729 RVA: 0x003B85AD File Offset: 0x003B67AD
		public override SecretInformationDisplayPackage SecretInformationDisplayPackage
		{
			get
			{
				return this._displayData.SecretInformationDisplayPackage;
			}
		}

		// Token: 0x17000DE8 RID: 3560
		// (get) Token: 0x06007FDA RID: 32730 RVA: 0x003B85BA File Offset: 0x003B67BA
		public override CharacterDisplayData TargetDisplayData
		{
			get
			{
				return this._displayData.TargetCharacterDisplayData;
			}
		}

		// Token: 0x17000DE9 RID: 3561
		// (get) Token: 0x06007FDB RID: 32731 RVA: 0x003B85C7 File Offset: 0x003B67C7
		public override CharacterDisplayData TaiwuDisplayData
		{
			get
			{
				return this._displayData.TaiwuDisplayData;
			}
		}

		// Token: 0x17000DEA RID: 3562
		// (get) Token: 0x06007FDC RID: 32732 RVA: 0x003B85D4 File Offset: 0x003B67D4
		protected override ItemListScroll.EColumnType ColumnType
		{
			get
			{
				return ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value;
			}
		}

		// Token: 0x06007FDD RID: 32733 RVA: 0x003B85DC File Offset: 0x003B67DC
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
			case 4:
				readOnlyList = this._displayData.TaiwuTroughItemDisplayDataList;
				break;
			default:
				readOnlyList = this._displayData.TaiwuKidnapMenuDisplayData.KidnapCharDisplayDataList;
				break;
			}
			if (!true)
			{
			}
			return ((readOnlyList != null) ? (from x in readOnlyList
			where this.CheckItemCanTransfer(x, this._displayData.TaiwuDisplayData)
			select x).ToArray<ITradeableContent>() : null) ?? Array.Empty<ITradeableContent>();
		}

		// Token: 0x06007FDE RID: 32734 RVA: 0x003B868C File Offset: 0x003B688C
		public override IReadOnlyList<ITradeableContent> GetTargetTradeableList(int index)
		{
			return this._displayData.TargetItemDisplayDataList;
		}

		// Token: 0x06007FDF RID: 32735 RVA: 0x003B8699 File Offset: 0x003B6899
		protected override void Awake()
		{
			this.ConfirmOnHideKey = LanguageKey.LK_Exchange_Title;
			base.Awake();
			this.exchangeContainer.exchangeBack.SetNpc(0);
			this.exchangeContainer.exchangeFront.SetNpc(0);
		}

		// Token: 0x06007FE0 RID: 32736 RVA: 0x003B86D4 File Offset: 0x003B68D4
		protected override void AwakeAvatarBtn()
		{
			this.exchangeContainer.avatarBtn.interactable = true;
			this.exchangeContainer.avatarBtn.onClick.ResetListener(delegate()
			{
				base.ShowCharacterMenu(this._displayData.TargetCharacterDisplayData);
			});
			this.exchangeContainer.btnOpenCharMenu.onClick.ResetListener(delegate()
			{
				base.ShowCharacterMenu(this._displayData.TargetCharacterDisplayData);
			});
		}

		// Token: 0x06007FE1 RID: 32737 RVA: 0x003B8738 File Offset: 0x003B6938
		public override void Submit()
		{
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
			{
				Title = LanguageKey.LK_Exchange_Confirm_Title.Tr(),
				Content = LanguageKey.LK_Exchange_Confirm_Desc.Tr(),
				Yes = new Action(this.SubmitImpl)
			}));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x06007FE2 RID: 32738 RVA: 0x003B87A7 File Offset: 0x003B69A7
		public void SubmitImpl()
		{
			TaiwuDomainMethod.Call.ConfirmExchange(this.Exchange);
			GEvent.OnEvent(UiEvents.OnRefreshCharacterMenuItem, null);
			this.Reset();
		}

		// Token: 0x06007FE3 RID: 32739 RVA: 0x003B87D0 File Offset: 0x003B69D0
		public override void OnInit(ArgumentBox argsBox)
		{
			this.ExchangeValueType = 9;
			base.OnInit(argsBox);
			argsBox.Get("ShouldTriggerEvent", out this._shouldTrigger);
			this.exchangeContainer.secretText.text = LanguageKey.LK_Exchange_Use_Secret.Tr();
			this.exchangeContainer.confirmButtonText.text = LanguageKey.LK_Exchange_Confirm.Tr();
			this.exchangeContainer.AddSwitchToggleListener();
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
			bool flag = !this._firstEnter;
			if (flag)
			{
				this._firstEnter = true;
				GlobalDomainMethod.Call.InvokeGuidingTrigger(80);
			}
		}

		// Token: 0x06007FE4 RID: 32740 RVA: 0x003B88EA File Offset: 0x003B6AEA
		protected override void RequestData()
		{
			TaiwuDomainMethod.AsyncCall.GetExchangeDisplayData(this, this.TargetId, EExchangeType.Person, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._displayData);
				this.SetTargetCharacterDisplayData(this._displayData.TargetCharacterDisplayData, LanguageKey.LK_Exchange_SubTitle_Exchange);
				this.exchangeContainer.titleTaiwu.text = SingletonObject.getInstance<BasicGameData>().TaiwuMonasticTitleOrDisplayName;
				this.SetWarehouseCanInteract(this._displayData.CanTransferItemToWarehouse);
				this.SetExchangeData();
				ExchangeContainer exchangeContainer = this.exchangeContainer;
				KidnapMenuDisplayData taiwuKidnapMenuDisplayData = this._displayData.TaiwuKidnapMenuDisplayData;
				int? num;
				if (taiwuKidnapMenuDisplayData == null)
				{
					num = null;
				}
				else
				{
					List<KidnapCharDisplayData> kidnapCharDisplayDataList = taiwuKidnapMenuDisplayData.KidnapCharDisplayDataList;
					num = ((kidnapCharDisplayDataList != null) ? new int?(kidnapCharDisplayDataList.Count) : null);
				}
				int? num2 = num;
				exchangeContainer.SetCurrPageInteractable(num2.GetValueOrDefault() > 0, 5);
				this.Refresh();
				this.Element.ShowAfterRefresh();
			});
		}

		// Token: 0x06007FE5 RID: 32741 RVA: 0x003B8908 File Offset: 0x003B6B08
		protected override void RefreshValues()
		{
			this.exchangeContainer.confirmButtonText.text = LanguageKey.LK_Exchange_Confirm.Tr();
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
			this.exchangeContainer.secretText.text = (this.SecretInformationDisplayPackage.SecretInformationDisplayDataList.Exists((SecretInformationDisplayData item) => (int)item.SecretInformationId == this.Exchange.AdvantageSummary.SecretId) ? LanguageKey.LK_Exchange_Use_Secret_Effect.TrFormat((this.Exchange.AdvantageSummary.SecretValue == 0) ? "grey" : "brightblue", this.Exchange.AdvantageSummary.SecretValue).ColorReplace() : LanguageKey.LK_Exchange_Use_Secret.Tr());
		}

		// Token: 0x06007FE6 RID: 32742 RVA: 0x003B8C9C File Offset: 0x003B6E9C
		protected override RowItemMain OnItemRender(ITradeableContent itemData, RowItemLine rowItemLine, CharacterDisplayData displayData)
		{
			RowItemMain rowItemMain = base.OnItemRender(itemData, rowItemLine, displayData);
			rowItemMain.SetExchangeStatus(itemData, this.Exchange.AdvantageSummary, displayData.CharacterId != this._displayData.TaiwuDisplayData.CharacterId);
			return rowItemMain;
		}

		// Token: 0x06007FE7 RID: 32743 RVA: 0x003B8CE7 File Offset: 0x003B6EE7
		protected override void RefreshSelfItems(int index)
		{
			base.RefreshSelfItems(index);
			this.goSelfEmptyExchange.SetActive(this.exchangeContainer.selfExchangeList.Exchange.TaiwuContentList.Count <= 0);
		}

		// Token: 0x06007FE8 RID: 32744 RVA: 0x003B8D1E File Offset: 0x003B6F1E
		protected override void RefreshTargetItems(int index)
		{
			base.RefreshTargetItems(index);
			this.goTargetEmptyExchange.SetActive(this.exchangeContainer.targetExchangeList.Exchange.TargetContentList.Count <= 0);
		}

		// Token: 0x17000DEB RID: 3563
		// (get) Token: 0x06007FE9 RID: 32745 RVA: 0x003B8D55 File Offset: 0x003B6F55
		protected override string ConfirmOnHideDesc
		{
			get
			{
				return LanguageKey.LK_Exchange_CancelDesc.Tr();
			}
		}

		// Token: 0x06007FEA RID: 32746 RVA: 0x003B8D64 File Offset: 0x003B6F64
		public override void QuickHideImpl()
		{
			bool shouldTrigger = this._shouldTrigger;
			if (shouldTrigger)
			{
				TaiwuEventDomainMethod.Call.TriggerListener("ExchangeComplete", true);
			}
			base.QuickHideImpl();
		}

		// Token: 0x06007FEB RID: 32747 RVA: 0x003B8D90 File Offset: 0x003B6F90
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
					scroll.SetItemToSelectCountMode(x, delegate(int count)
					{
						action(x, count);
						this.Refresh();
					}, delegate
					{
					}, initSelectCount, 0, 1, null, false, null, false);
				}
			});
		}

		// Token: 0x0400621C RID: 25116
		[SerializeField]
		protected TooltipInvoker selfAdvantage;

		// Token: 0x0400621D RID: 25117
		[SerializeField]
		protected TooltipInvoker targetAdvantage;

		// Token: 0x0400621E RID: 25118
		[SerializeField]
		private GameObject goSelfEmptyExchange;

		// Token: 0x0400621F RID: 25119
		[SerializeField]
		private GameObject goTargetEmptyExchange;

		// Token: 0x04006220 RID: 25120
		private ExchangeDisplayData _displayData;

		// Token: 0x04006221 RID: 25121
		private bool _shouldTrigger;

		// Token: 0x04006222 RID: 25122
		private bool _firstEnter;
	}
}
