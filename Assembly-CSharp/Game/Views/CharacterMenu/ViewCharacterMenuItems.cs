using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Components.Item;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter.Item;
using Game.Components.SortAndFilter.Item.Apply;
using Game.Views.Cricket.Combat;
using Game.Views.Exchange;
using Game.Views.SectInteract.Fulong;
using Game.Views.Select;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Story;
using GameData.Domains.Story.MainStory;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Profession;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.TaiwuEvent.Enum;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using GameDataExtensions;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000BA7 RID: 2983
	public class ViewCharacterMenuItems : UI_CharacterMenuSubPageBase
	{
		// Token: 0x1700100B RID: 4107
		// (get) Token: 0x060094E3 RID: 38115 RVA: 0x00455AEA File Offset: 0x00453CEA
		public override LanguageKey TitleKey
		{
			get
			{
				return LanguageKey.LK_CharacterMenu_Title_Items;
			}
		}

		// Token: 0x060094E4 RID: 38116 RVA: 0x00455AF4 File Offset: 0x00453CF4
		public override bool CheckState(ECharacterSubToggleBase curSubTogglePage, ECharacterSubPage curSubPage)
		{
			return curSubTogglePage == ECharacterSubToggleBase.ItemBase;
		}

		// Token: 0x1700100C RID: 4108
		// (get) Token: 0x060094E5 RID: 38117 RVA: 0x00455B0A File Offset: 0x00453D0A
		public override bool ShowBaseAttribute
		{
			get
			{
				return this.CurTabIndex == 0 && !this._IsForceHideAttribute;
			}
		}

		// Token: 0x1700100D RID: 4109
		// (get) Token: 0x060094E6 RID: 38118 RVA: 0x00455B20 File Offset: 0x00453D20
		private bool SelectingTransferItemChar
		{
			get
			{
				return ViewCharacterMenuItems._currItemOperation == ItemOperationType.EItemOperationType.Transfer;
			}
		}

		// Token: 0x1700100E RID: 4110
		// (get) Token: 0x060094E7 RID: 38119 RVA: 0x00455B2A File Offset: 0x00453D2A
		public static ItemOperationType.EItemOperationType CurrItemOperation
		{
			get
			{
				return ViewCharacterMenuItems._currItemOperation;
			}
		}

		// Token: 0x1700100F RID: 4111
		// (get) Token: 0x060094E8 RID: 38120 RVA: 0x00455B31 File Offset: 0x00453D31
		private bool CurCharacterIsTaiwu
		{
			get
			{
				return base.CharacterMenu.CurrentCharacterIsTaiwu;
			}
		}

		// Token: 0x17001010 RID: 4112
		// (get) Token: 0x060094E9 RID: 38121 RVA: 0x00455B3E File Offset: 0x00453D3E
		private ref List<ItemDisplayData> InventoryItems
		{
			get
			{
				return ref this._characterItemsDisplayData.InventoryItems;
			}
		}

		// Token: 0x17001011 RID: 4113
		// (get) Token: 0x060094EA RID: 38122 RVA: 0x00455B4B File Offset: 0x00453D4B
		private int MaxLoad
		{
			get
			{
				return this._characterItemsDisplayData.MaxLoad;
			}
		}

		// Token: 0x17001012 RID: 4114
		// (get) Token: 0x060094EB RID: 38123 RVA: 0x00455B58 File Offset: 0x00453D58
		private int CurLoad
		{
			get
			{
				return this._characterItemsDisplayData.CurLoad;
			}
		}

		// Token: 0x17001013 RID: 4115
		// (get) Token: 0x060094EC RID: 38124 RVA: 0x00455B65 File Offset: 0x00453D65
		private int Exp
		{
			get
			{
				return this._characterItemsDisplayData.Exp;
			}
		}

		// Token: 0x17001014 RID: 4116
		// (get) Token: 0x060094ED RID: 38125 RVA: 0x00455B72 File Offset: 0x00453D72
		private long MaxWorthCanBeLentToTaiwu
		{
			get
			{
				return this._characterItemsDisplayData.MaxWorthCanBeLentToTaiwu;
			}
		}

		// Token: 0x17001015 RID: 4117
		// (get) Token: 0x060094EE RID: 38126 RVA: 0x00455B7F File Offset: 0x00453D7F
		private CharacterLoveAndHateItemInfo LoveAndHateItemInfo
		{
			get
			{
				return this._characterItemsDisplayData.CharacterLoveAndHateItemInfo;
			}
		}

		// Token: 0x17001016 RID: 4118
		// (get) Token: 0x060094EF RID: 38127 RVA: 0x00455B8C File Offset: 0x00453D8C
		private CharacterDisplayData CurrentCharacterDisplayData
		{
			get
			{
				return this._characterItemsDisplayData.CharacterDisplayData;
			}
		}

		// Token: 0x17001017 RID: 4119
		// (get) Token: 0x060094F0 RID: 38128 RVA: 0x00455B99 File Offset: 0x00453D99
		private CharacterDisplayData TaiwuCharacterDisplayData
		{
			get
			{
				return this._characterItemsDisplayData.TaiwuCharacterDisplayData;
			}
		}

		// Token: 0x060094F1 RID: 38129 RVA: 0x00455BA6 File Offset: 0x00453DA6
		public void AddPreOperation(ItemOperationType.EItemOperationType operationType, ItemKey itemKey)
		{
			this._preOperation = new ValueTuple<ItemOperationType.EItemOperationType, ItemKey>(operationType, itemKey);
		}

		// Token: 0x060094F2 RID: 38130 RVA: 0x00455BB8 File Offset: 0x00453DB8
		private void TryExecuteOperation()
		{
			bool flag = this._preOperation.Item1 == ItemOperationType.EItemOperationType.Invalid;
			if (!flag)
			{
				ItemDisplayData item = this._characterItemsDisplayData.InventoryItems.Find((ItemDisplayData d) => d.ContainsItemKey(this._preOperation.Item2));
				bool flag2 = item != null;
				if (flag2)
				{
					this.OnClickItemEnterMultiplyMode(null, this._preOperation.Item1, item);
				}
				this._preOperation = new ValueTuple<ItemOperationType.EItemOperationType, ItemKey>(ItemOperationType.EItemOperationType.Invalid, ItemKey.Invalid);
			}
		}

		// Token: 0x060094F3 RID: 38131 RVA: 0x00455C24 File Offset: 0x00453E24
		public override void OnInit(ArgumentBox argsBox)
		{
			this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			ViewCharacterMenuItems._currItemOperation = ItemOperationType.EItemOperationType.Invalid;
			this.NeedDataListenerId = true;
			int tabIndex;
			bool flag = argsBox.Get("TabIndex", out tabIndex);
			if (flag)
			{
				this.CurTabIndex = tabIndex;
			}
			this.RefreshMultiplyButtons();
		}

		// Token: 0x060094F4 RID: 38132 RVA: 0x00455C70 File Offset: 0x00453E70
		private void Awake()
		{
			this.itemListScroll.CustomAmountDataGenerator = new Func<ITradeableContent, string>(this.AmountCellDataGenerator);
			this.itemListScroll.OnSortAndFilterChangedCallback = new Action(this.OnSortAndFilterChangedCallback);
			this.itemListScroll.Init("ViewCharacterMenuItems", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnRenderItem), new Action<ITradeableContent, RowItemLine>(this.OnClickItem), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability, null, null, null);
			this.selectedItemListScroll.Init("ViewCharacterMenuItemsSelected", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnRenderItem), new Action<ITradeableContent, RowItemLine>(this.OnClickItem), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability, null, null, null);
			this.maskToFocusGroupedItemList.GetComponent<CButton>().ClearAndAddListener(delegate
			{
				bool selectingTransferItemChar = this.SelectingTransferItemChar;
				if (selectingTransferItemChar)
				{
					Action onTryClosePage = base.CharacterMenu.OnTryClosePage;
					if (onTryClosePage != null)
					{
						onTryClosePage();
					}
				}
			});
			this.buttonAutoOperationSetting.ClearAndAddListener(new Action(this.OnClickButtonAutoOperationSetting));
			this.targetToggleGroup.Init(0);
		}

		// Token: 0x060094F5 RID: 38133 RVA: 0x00455D50 File Offset: 0x00453F50
		private void OnEnable()
		{
			GEvent.Add(UiEvents.ExitMultiplyOperation, new GEvent.Callback(this.multiplyItemListScroll.OnExitMultiplyOperation));
			GEvent.Add(UiEvents.ItemMultiplyOperationTypeChange, new GEvent.Callback(this.ItemMultiplyOperationTypeChange));
			GEvent.Add(UiEvents.ItemMultiplyOperationTargetChange, new GEvent.Callback(this.ItemMultiplyOperationTargetChange));
			GEvent.Add(UiEvents.ItemMultiplyOperationConfirm, new GEvent.Callback(this.multiplyItemListScroll.OnItemMultiplyOperationConfirm));
			GEvent.Add(UiEvents.ItemMultiplyOperationCancelSelection, new GEvent.Callback(this.multiplyItemListScroll.OnItemMultiplyOperationCancelSelection));
			GEvent.Add(UiEvents.OnRefreshCharacterMenuItem, new GEvent.Callback(this.CallRefreshItems));
			GEvent.Add(UiEvents.ExchangeResource, new GEvent.Callback(this.OnExchangeResource));
			GEvent.Add(UiEvents.ItemMultiplyOperationFinish, new GEvent.Callback(this.ItemMultiplyOperationFinish));
			GEvent.Add(UiEvents.RefreshCharacterMenuStack, new GEvent.Callback(this.RefreshCharacterMenuStack));
			this.targetToggleGroup.OnActiveIndexChange += this.OnTargetCardModeChange;
		}

		// Token: 0x060094F6 RID: 38134 RVA: 0x00455E70 File Offset: 0x00454070
		private new void OnDisable()
		{
			GEvent.Remove(UiEvents.ExitMultiplyOperation, new GEvent.Callback(this.multiplyItemListScroll.OnExitMultiplyOperation));
			GEvent.Remove(UiEvents.ItemMultiplyOperationTypeChange, new GEvent.Callback(this.ItemMultiplyOperationTypeChange));
			GEvent.Remove(UiEvents.ItemMultiplyOperationTargetChange, new GEvent.Callback(this.ItemMultiplyOperationTargetChange));
			GEvent.Remove(UiEvents.ItemMultiplyOperationConfirm, new GEvent.Callback(this.multiplyItemListScroll.OnItemMultiplyOperationConfirm));
			GEvent.Remove(UiEvents.ItemMultiplyOperationCancelSelection, new GEvent.Callback(this.multiplyItemListScroll.OnItemMultiplyOperationCancelSelection));
			GEvent.Remove(UiEvents.OnRefreshCharacterMenuItem, new GEvent.Callback(this.CallRefreshItems));
			GEvent.Remove(UiEvents.ExchangeResource, new GEvent.Callback(this.OnExchangeResource));
			GEvent.Remove(UiEvents.ItemMultiplyOperationFinish, new GEvent.Callback(this.ItemMultiplyOperationFinish));
			GEvent.Remove(UiEvents.RefreshCharacterMenuStack, new GEvent.Callback(this.RefreshCharacterMenuStack));
			this.targetToggleGroup.OnActiveIndexChange -= this.OnTargetCardModeChange;
		}

		// Token: 0x060094F7 RID: 38135 RVA: 0x00455F8F File Offset: 0x0045418F
		private void RefreshCharacterMenuStack(ArgumentBox _)
		{
			this.RefreshMultiplyButtons();
		}

		// Token: 0x060094F8 RID: 38136 RVA: 0x00455F98 File Offset: 0x00454198
		private void Update()
		{
			bool flag = ViewCharacterMenuItems._currItemOperation == ItemOperationType.EItemOperationType.Invalid && !this.multiplyItemListScroll.IsMultiItemSelect && CommonCommandKit.SecondaryInteraction.Check(this.Element, false, false, false, true, false) && UIManager.Instance.IsFocusElement(UIElement.CharacterMenu);
			if (flag)
			{
				RowItemLine[] lines = this.itemListScroll.InfiniteScroll.Scroll.Content.GetComponentsInChildren<RowItemLine>();
				bool flag2 = lines != null;
				if (flag2)
				{
					foreach (RowItemLine rowItemLine in lines)
					{
						RectTransform buttonRectTrans = rowItemLine.RectTransform;
						Vector2 mapFilterLocalPos;
						bool isHoverButton = RectTransformUtility.ScreenPointToLocalPointInRectangle(buttonRectTrans, Input.mousePosition, UIManager.Instance.UiCamera, out mapFilterLocalPos) && buttonRectTrans.rect.Contains(mapFilterLocalPos);
						bool flag3 = isHoverButton;
						if (flag3)
						{
							ItemDisplayData itemData = rowItemLine.Data as ItemDisplayData;
							bool flag4 = this.CanClickLock(itemData);
							if (flag4)
							{
								this.OnClickLock(itemData);
								string audioName = itemData.IsLocked ? "ui_default_reduce" : "ui_default_add";
								AudioManager.Instance.PlaySound(audioName, false, false);
								return;
							}
						}
					}
				}
				AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
			}
		}

		// Token: 0x060094F9 RID: 38137 RVA: 0x004560E0 File Offset: 0x004542E0
		private void InitMultiplyItemScrollView()
		{
			bool hasInit = this.multiplyItemListScroll.HasInit;
			if (!hasInit)
			{
				this.multiplyItemListScroll.Init(base.CharacterMenu.CurCharacterId, this._characterItemsDisplayData.EmptyToolKey, this._characterItemsDisplayData.TaiwuCharacterDisplayData, new Action(this.<InitMultiplyItemScrollView>g__OnEnterMultiplyOperation|69_0), new Action(this.<InitMultiplyItemScrollView>g__OnExitMultiplyOperation|69_1), new Action(this.<InitMultiplyItemScrollView>g__OnEnterMultiplyLock|69_2), new Action(this.<InitMultiplyItemScrollView>g__OnExitMultiplyLock|69_3), null);
				this.InitSwitchSelection();
				this.RefreshMultiplyButtons();
			}
		}

		// Token: 0x060094FA RID: 38138 RVA: 0x00456170 File Offset: 0x00454370
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

		// Token: 0x060094FB RID: 38139 RVA: 0x004561E4 File Offset: 0x004543E4
		private void RefreshMultiplyButtons()
		{
			CharacterMenuFunctionControlItem config;
			bool canOperate = base.CharacterMenu.CanOperate && (!base.CharacterMenu.TryGetFunctionControlConfig(out config) || !base.CharacterMenu.IsFunctionBanned(config.Batch));
			bool showStack = base.CharacterMenu.StackView.gameObject.activeSelf;
			bool canTransfer = canOperate && !showStack && this.IsTaiwuTeamButNotBeast;
			this.multiplyItemListScroll.ShowMultiplySelectButton(null, canTransfer);
			this.multiplyItemListScroll.ShowToggleMultiplyLock(null, canTransfer);
		}

		// Token: 0x060094FC RID: 38140 RVA: 0x0045626B File Offset: 0x0045446B
		public override void OnSwitchToSubpage(int subPageIndex)
		{
			this.CurTabIndex = subPageIndex;
			this.itemPage.gameObject.SetActive(this.CurTabIndex == 0);
			base.CharacterMenu.SetBaseAttributeState((this.CurTabIndex == 0) ? 1 : -1);
		}

		// Token: 0x060094FD RID: 38141 RVA: 0x004562A8 File Offset: 0x004544A8
		public override void OnSubpageVisible()
		{
			base.OnSubpageVisible();
			GEvent.Add(UiEvents.OnSelectTransferItemChar, new GEvent.Callback(this.OnSelectTransferItemChar));
			base.CharacterMenu.SetBaseAttributeState((this.CurTabIndex == 0) ? 1 : -1);
			this.CallRefreshItems(null);
		}

		// Token: 0x060094FE RID: 38142 RVA: 0x004562F7 File Offset: 0x004544F7
		public override void OnSubpageInVisible()
		{
			base.OnSubpageInVisible();
			GEvent.Remove(UiEvents.OnSelectTransferItemChar, new GEvent.Callback(this.OnSelectTransferItemChar));
		}

		// Token: 0x060094FF RID: 38143 RVA: 0x0045631A File Offset: 0x0045451A
		private void OnTargetCardModeChange(int previousIndex, int currentIndex)
		{
			this.itemListScroll.SwitchCardModeToggle(previousIndex, currentIndex);
			this.selectedItemListScroll.SwitchCardModeToggle(previousIndex, currentIndex);
		}

		// Token: 0x06009500 RID: 38144 RVA: 0x0045633C File Offset: 0x0045453C
		public override void OnCurrentCharacterChange(int prevCharacterId)
		{
			this.multiplyItemListScroll.CurCharId = base.CharacterMenu.CurCharacterId;
			bool flag = !this.multiplyItemListScroll.IsMultiItemSelect;
			if (flag)
			{
				this.RefreshMultiplyButtons();
			}
			bool flag2 = base.CharacterMenu.CurCharacterId < 0 || prevCharacterId == base.CharacterMenu.CurCharacterId;
			if (!flag2)
			{
				this.buttonAutoOperationSetting.gameObject.SetActive(false);
				base.ClearMonitorFields();
				bool flag3 = base.CharacterMenu.CurCharacterId < 0;
				if (!flag3)
				{
					this.localLoadingAnim.SetLoadingEvent(null, delegate
					{
						this.localLoadingAnim.SetLoadingContentList(null);
					});
					this.localLoadingAnim.SetLoadingContentList(new List<GameObject>
					{
						this.itemListScroll.IsCardMode ? this.itemListScrollCardStyle : this.itemListScrollListStyle
					});
					this.localLoadingAnim.SetLoadingState(true);
					CharacterDomainMethod.Call.GetCharacterItemsDisplayData(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
				}
			}
		}

		// Token: 0x06009501 RID: 38145 RVA: 0x00456444 File Offset: 0x00454644
		protected override void OnNotifyGameDataFiltered(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 1)
				{
					bool flag = notification.DomainId == 4;
					if (flag)
					{
						bool flag2 = notification.MethodId == 207;
						if (flag2)
						{
							this._characterItemsDisplayData = null;
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._characterItemsDisplayData);
							bool flag3 = this._characterItemsDisplayData == null;
							if (flag3)
							{
								break;
							}
							this.ChickenMapHandle();
							this.Refresh();
							this.localLoadingAnim.SetLoadingState(false);
							this.Element.ShowAfterRefresh();
							this.TryExecuteOperation();
						}
					}
				}
			}
		}

		// Token: 0x06009502 RID: 38146 RVA: 0x00456538 File Offset: 0x00454738
		private unsafe void Refresh()
		{
			this.InitMultiplyItemScrollView();
			Dictionary<ItemSourceType, List<ItemDisplayData>> itemDict = new Dictionary<ItemSourceType, List<ItemDisplayData>>
			{
				{
					ItemSourceType.Inventory,
					*this.InventoryItems
				}
			};
			this.multiplyItemListScroll.Set(itemDict);
			this.multiplyItemListScroll.MaxWorthCanBeLentToTaiwu = this.MaxWorthCanBeLentToTaiwu;
			this.multiplyItemListScroll.RefreshItems();
			this.RefreshLoad();
			this.SetItemScrollViewCanScroll(true);
			GlobalDomainMethod.Call.InvokeGuidingTrigger(138);
			List<ItemDisplayData> list = *this.InventoryItems;
			bool flag;
			if (list == null)
			{
				flag = false;
			}
			else
			{
				flag = list.Any((ItemDisplayData d) => d.RealKey.ItemType == 7);
			}
			bool flag2 = flag;
			if (flag2)
			{
				GlobalDomainMethod.Call.InvokeGuidingTrigger(92);
			}
			List<ItemDisplayData> list2 = *this.InventoryItems;
			bool flag3;
			if (list2 == null)
			{
				flag3 = false;
			}
			else
			{
				flag3 = list2.Any((ItemDisplayData d) => d.RealKey.GetConfig().ItemSubType == 800);
			}
			bool flag4 = flag3;
			if (flag4)
			{
				GlobalDomainMethod.Call.InvokeGuidingTrigger(94);
			}
			List<ItemDisplayData> list3 = *this.InventoryItems;
			bool flag5;
			if (list3 == null)
			{
				flag5 = false;
			}
			else
			{
				flag5 = list3.Any((ItemDisplayData d) => d.RealKey.GetConfig().ItemSubType == 801);
			}
			bool flag6 = flag5;
			if (flag6)
			{
				GlobalDomainMethod.Call.InvokeGuidingTrigger(93);
			}
		}

		// Token: 0x06009503 RID: 38147 RVA: 0x00456660 File Offset: 0x00454860
		protected override void OnClick(Transform btn)
		{
			string name = btn.name;
			string a = name;
			if (a == "ExchangeResource")
			{
				this.ExchangeResource(base.CharacterMenu.CurCharacterId);
			}
		}

		// Token: 0x06009504 RID: 38148 RVA: 0x0045669C File Offset: 0x0045489C
		private void RefreshLoad()
		{
			this.loadText.text = ViewExchangeBase.GetWeightString(this.CurLoad, this.MaxLoad, this.CurLoad, this.MaxLoad, LanguageKey.LK_Exchange_Weight_Value);
			TooltipInvoker tipDisplayer = this.loadOverflowTips;
			tipDisplayer.enabled = (this.IsTaiwuTeamButNotBeast && this.CurLoad > this.MaxLoad);
			bool enabled = tipDisplayer.enabled;
			if (enabled)
			{
				tipDisplayer.Type = TipType.Simple;
				TooltipInvoker tooltipInvoker = tipDisplayer;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				WorldStateItem worldStateItem = WorldState.Instance[11];
				tipDisplayer.RuntimeParam.Set("arg0", worldStateItem.Name);
				string loadTipContent = LocalStringManager.GetFormat(LanguageKey.LK_Inventory_Overflow_Tips, this._characterItemsDisplayData.MoveTimeCostPercent - 100).ColorReplace();
				tipDisplayer.RuntimeParam.Set("arg1", loadTipContent);
			}
		}

		// Token: 0x06009505 RID: 38149 RVA: 0x00456788 File Offset: 0x00454988
		private bool CheckItemCanTransfer(ITradeableContent itemData)
		{
			bool isChildCloth = itemData.UsingType == ItemDisplayData.ItemUsingType.Equiped && this.CurrentCharacterDisplayData.PhysiologicalAge < 16 && ItemTemplateHelper.GetEquipmentType(itemData.Key.ItemType, itemData.Key.TemplateId) == 2;
			bool isTransferable = ItemTemplateHelper.IsTransferable(itemData.Key.ItemType, itemData.Key.TemplateId) && !isChildCloth;
			bool miscResourceCanExchange = ItemTemplateHelper.MiscResourceCanExchange(itemData.Key.ItemType, itemData.Key.TemplateId);
			bool canTransfer = isTransferable || miscResourceCanExchange;
			return canTransfer && !ItemDisplayDataHelper.IsItemLockedByTask(itemData);
		}

		// Token: 0x06009506 RID: 38150 RVA: 0x0045682C File Offset: 0x00454A2C
		private bool CheckItemIsLocked(ITradeableContent itemData)
		{
			bool flag = !base.CharacterMenu.CanOperate;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool isBaby = this.CurrentCharacterDisplayData != null && AgeGroup.GetAgeGroup(this.CurrentCharacterDisplayData.PhysiologicalAge) == 0;
				bool flag2 = isBaby;
				if (flag2)
				{
					result = true;
				}
				else
				{
					bool isMultiItemSelect = this.multiplyItemListScroll.IsMultiItemSelect;
					if (isMultiItemSelect)
					{
						bool flag3 = this.multiplyItemListScroll.SelectedMultiplyItemDict.ContainsKey(itemData as ItemDisplayData);
						if (flag3)
						{
							return false;
						}
					}
					else
					{
						sbyte itemType = itemData.Key.ItemType;
						bool flag4 = itemType == 8 || itemType == 7;
						if (flag4)
						{
							return false;
						}
						bool flag5 = ItemTemplateHelper.IsTianJieFuLu(itemData.Key.ItemType, itemData.Key.TemplateId);
						if (flag5)
						{
							return false;
						}
					}
					result = (!this.CheckItemCanTransfer(itemData) && !this.CheckSpecialItemCanInteract(itemData));
				}
			}
			return result;
		}

		// Token: 0x06009507 RID: 38151 RVA: 0x00456920 File Offset: 0x00454B20
		private bool CheckSpecialItemCanInteract(ITradeableContent itemData)
		{
			bool isHeavenlyTreeSeeds = ItemTemplateHelper.CheckIsHeavenlyTreeSeeds(itemData.Key.ItemType, itemData.Key.TemplateId);
			bool isSectMainStoryItemXuannvNotes = ItemTemplateHelper.CheckIsSectMainStoryItemXuannvNotes(itemData.Key.ItemType, itemData.Key.TemplateId);
			bool isSectMainStoryItemWuxianWugFairy = ItemTemplateHelper.CheckIsSectMainStoryItemWuxianWugFairy(itemData.Key.ItemType, itemData.Key.TemplateId);
			bool isSectMainStoryFulongChickenMap = ItemTemplateHelper.CheckIsSectMainStoryFulongChickenMap(itemData.Key.ItemType, itemData.Key.TemplateId);
			bool isSectMainStoryItemYuanshanRosary = ItemTemplateHelper.CheckIsSectMainStoryItemYuanshanRosary(itemData.Key.ItemType, itemData.Key.TemplateId);
			bool isSectMainStoryItemJieqingStars = ItemTemplateHelper.CheckIsSectMainStoryItemJieQingStars(itemData.Key.ItemType, itemData.Key.TemplateId);
			bool isDamageHugeSword = ItemTemplateHelper.CheckIsDamageHugeSword(itemData.Key.ItemType, itemData.Key.TemplateId);
			bool isThanksLetter = ItemTemplateHelper.IsThanksLetter(itemData.Key.ItemType, itemData.Key.TemplateId);
			bool canTriggerCommonEvent = ItemTemplateHelper.CanTriggerCommonEvent(itemData.Key.ItemType, itemData.Key.TemplateId);
			bool canClickSwordFragment = this.CanClickSwordFragment(itemData);
			return isHeavenlyTreeSeeds || isSectMainStoryItemXuannvNotes || isSectMainStoryItemWuxianWugFairy || isSectMainStoryFulongChickenMap || isSectMainStoryItemYuanshanRosary || isThanksLetter || isSectMainStoryItemJieqingStars || isDamageHugeSword || canTriggerCommonEvent || canClickSwordFragment;
		}

		// Token: 0x06009508 RID: 38152 RVA: 0x00456A58 File Offset: 0x00454C58
		private void OnSortAndFilterChangedCallback()
		{
			ItemSortAndFilterController itemSortAndFilterController = this.itemListScroll.SortAndFilterController as ItemSortAndFilterController;
			bool flag = itemSortAndFilterController == null;
			if (!flag)
			{
				ESelectItemFilterType currentFilterType = itemSortAndFilterController.GetCurrentFilterType();
				if (!true)
				{
				}
				ItemListScroll.EColumnType ecolumnType;
				switch (currentFilterType)
				{
				case ESelectItemFilterType.Food:
					ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value);
					break;
				case ESelectItemFilterType.Medicine:
					ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value);
					break;
				case ESelectItemFilterType.Equipment:
					ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability | ItemListScroll.EColumnType.Power);
					break;
				case ESelectItemFilterType.EquipmentWeapon:
					ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability | ItemListScroll.EColumnType.Power);
					break;
				case ESelectItemFilterType.EquipmentArmor:
					ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability | ItemListScroll.EColumnType.Power);
					break;
				case ESelectItemFilterType.EquipmentAccessory:
					ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability | ItemListScroll.EColumnType.Power);
					break;
				case ESelectItemFilterType.EquipmentClothing:
					ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability | ItemListScroll.EColumnType.Power);
					break;
				case ESelectItemFilterType.EquipmentCarrier:
					ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability | ItemListScroll.EColumnType.Power);
					break;
				case ESelectItemFilterType.Book:
					ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability | ItemListScroll.EColumnType.Book);
					break;
				case ESelectItemFilterType.Tool:
					ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability | ItemListScroll.EColumnType.ToolAttainment);
					break;
				case ESelectItemFilterType.Material:
					ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value);
					break;
				case ESelectItemFilterType.Misc:
					ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value);
					break;
				case ESelectItemFilterType.MiscCricket:
					ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.CricketAge | ItemListScroll.EColumnType.CricketDurability | ItemListScroll.EColumnType.CricketWin | ItemListScroll.EColumnType.CricketLose);
					break;
				default:
					ecolumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability);
					break;
				}
				if (!true)
				{
				}
				ItemListScroll.EColumnType columnType = ecolumnType;
				bool flag2 = this.itemListScroll.ColumnTypeFlags != columnType;
				if (flag2)
				{
					this.itemListScroll.SetColumnTypeFlags(columnType);
				}
			}
		}

		// Token: 0x06009509 RID: 38153 RVA: 0x00456B78 File Offset: 0x00454D78
		private void OnRenderItem(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, true);
			bool isLock = this.CheckItemIsLocked(itemData);
			rowItemLine.SetInteractable(!isLock, true);
			rowItemLine.SetDisabled(isLock);
			this.SetResourceItemTip(rowItemLine);
			bool isMultiItemSelect = this.multiplyItemListScroll.IsMultiItemSelect;
			if (isMultiItemSelect)
			{
				this.multiplyItemListScroll.OnRenderItemMultiply(itemData, rowItemLine);
			}
			else
			{
				this.OnRenderItemSingle(itemData, rowItemLine);
			}
		}

		// Token: 0x0600950A RID: 38154 RVA: 0x00456BEC File Offset: 0x00454DEC
		private void OnRenderItemSingle(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			bool isSelected = false;
			bool flag = ViewCharacterMenuItems._currItemOperation == ItemOperationType.EItemOperationType.Transfer && !this.multiplyItemListScroll.IsMultiItemSelect;
			if (flag)
			{
				isSelected = itemData.RealKey.Equals(this._selectedToTransferItemData.RealKey);
			}
			rowItemLine.SetSelected(isSelected);
			bool flag2 = !rowItemLine.Interactable;
			if (flag2)
			{
				rowItemLine.RowItemMain.ShowInteractionStateLocked();
			}
			else
			{
				rowItemLine.RowItemMain.HideInteractionState();
			}
			bool currentCharacterIsTaiwu = base.CharacterMenu.CurrentCharacterIsTaiwu;
			if (currentCharacterIsTaiwu)
			{
				rowItemLine.RowItemMain.SetFavoriteStatus(RowItemMain.FavoriteStatus.None);
			}
			else
			{
				bool flag3 = this.LoveAndHateItemInfo != null;
				if (flag3)
				{
					rowItemLine.RowItemMain.SetFavoriteStatus(this.LoveAndHateItemInfo.LovingItemSubType, this.LoveAndHateItemInfo.HatingItemSubType);
				}
			}
		}

		// Token: 0x0600950B RID: 38155 RVA: 0x00456CB8 File Offset: 0x00454EB8
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
				bool isMultiplySelected = selectedData != null && this.multiplyItemListScroll.SelectedMultiplyItemDict.TryGetValue(selectedData, out selectedCount) && selectedCount > 0;
				bool isSingleSelected = this._selectedToTransferItemData != null;
				bool flag2 = isSingleSelected;
				if (flag2)
				{
					selectedCount = this._selectedToTransferItemData.Amount;
				}
				bool isSelected = isMultiplySelected || isSingleSelected;
				string maxAmountStr = CommonUtils.GetDisplayStringForNum(content.Amount, 100000);
				bool flag3 = !isSelected;
				if (flag3)
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

		// Token: 0x0600950C RID: 38156 RVA: 0x00456DA8 File Offset: 0x00454FA8
		private void SetResourceItemTip(RowItemLine rowItemLine)
		{
			bool flag = this.CurrentCharacterDisplayData == null || !rowItemLine.Data.IsResource;
			if (!flag)
			{
				bool isTaiwu = SingletonObject.getInstance<BasicGameData>().TaiwuCharId == this.CurrentCharacterDisplayData.CharacterId;
				string charName = NameCenter.GetMonasticTitleOrDisplayName(this.CurrentCharacterDisplayData, isTaiwu);
				RowItemLine.SetResourceTip(rowItemLine.Data, rowItemLine.TipDisplayer, charName, isTaiwu, false);
			}
		}

		// Token: 0x0600950D RID: 38157 RVA: 0x00456E10 File Offset: 0x00455010
		public static bool CheckCurrentToolAttainment(ItemOperationType.EItemOperationType operationType, ItemDisplayData tool, ItemDisplayData itemData, LifeSkillShorts lifeSkillAttainments, out sbyte lifeSkillType, out short requiredAttainment)
		{
			lifeSkillType = ItemTemplateHelper.GetCraftRequiredLifeSkillType(itemData.Key.ItemType, itemData.Key.TemplateId);
			List<sbyte> toolRequiredLifeSkillTypes = CraftTool.Instance[tool.Key.TemplateId].RequiredLifeSkillTypes;
			requiredAttainment = ViewCharacterMenuItems.GetOperationRequiredAttainment(operationType, itemData);
			bool flag = !toolRequiredLifeSkillTypes.Contains(lifeSkillType);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				short skillAttainment = lifeSkillAttainments.Get((int)lifeSkillType);
				short finalAttainment = UI_Make.GetFinalAttainment(tool.Key.TemplateId, skillAttainment, lifeSkillType);
				result = (finalAttainment >= requiredAttainment);
			}
			return result;
		}

		// Token: 0x0600950E RID: 38158 RVA: 0x00456EA8 File Offset: 0x004550A8
		public static short GetOperationRequiredAttainment(ItemOperationType.EItemOperationType operationType, ItemDisplayData itemData)
		{
			if (!true)
			{
			}
			short result;
			if (operationType != ItemOperationType.EItemOperationType.Repair)
			{
				if (operationType != ItemOperationType.EItemOperationType.Disassemble)
				{
					throw new ArgumentOutOfRangeException();
				}
				result = ItemTemplateHelper.GetDisassembleRequiredAttainment(itemData.Key.ItemType, itemData.Key.TemplateId);
			}
			else
			{
				result = ItemTemplateHelper.GetRepairRequiredAttainment(itemData.Key.ItemType, itemData.Key.TemplateId, itemData.Durability);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600950F RID: 38159 RVA: 0x00456F18 File Offset: 0x00455118
		public static bool CheckCurrentToolDurability(ItemDisplayData tool, ItemDisplayData itemData, out short durabilityCost)
		{
			durabilityCost = 0;
			bool flag = tool == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				sbyte grade = ItemTemplateHelper.GetGrade(itemData.Key.ItemType, itemData.Key.TemplateId);
				CraftToolItem toolConfig = CraftTool.Instance[tool.Key.TemplateId];
				durabilityCost = toolConfig.DurabilityCost[(int)grade];
				bool durabilityIsMeet = durabilityCost == 0 || tool.Durability > 0;
				result = durabilityIsMeet;
			}
			return result;
		}

		// Token: 0x06009510 RID: 38160 RVA: 0x00456F8C File Offset: 0x0045518C
		public override void OnExitFocusMode()
		{
			this.ExitTransferFocusItem();
			this._selectedToTransferItemData = null;
			this.itemListScroll.ReRender();
			ViewCharacterMenuItems._currItemOperation = ItemOperationType.EItemOperationType.Invalid;
			this.AddDebtMouseTipInfo();
			base.CharacterMenu.Injury.HideNotice(true, true);
			this.RefreshMultiplyButtons();
			this.SetItemScrollViewCanScroll(true);
		}

		// Token: 0x06009511 RID: 38161 RVA: 0x00456FE4 File Offset: 0x004551E4
		private void CallRefreshItems(ArgumentBox args = null)
		{
			int charId = (ViewCharacterMenuItems._currItemOperation == ItemOperationType.EItemOperationType.Invalid) ? base.CharacterMenu.CurCharacterId : SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			bool flag = charId < 0;
			if (!flag)
			{
				CharacterDomainMethod.Call.GetCharacterItemsDisplayData(this.Element.GameDataListenerId, charId);
			}
		}

		// Token: 0x06009512 RID: 38162 RVA: 0x00457030 File Offset: 0x00455230
		private void ExitItemOperation(bool needRefresh = false)
		{
			this.RefreshMultiplyButtons();
			base.CharacterMenu.ExitFocusMode(true);
			base.CharacterMenu.OnTryClosePage = null;
			if (needRefresh)
			{
				this.CallRefreshItems(null);
			}
		}

		// Token: 0x06009513 RID: 38163 RVA: 0x0045706B File Offset: 0x0045526B
		private void SetItemScrollViewCanScroll(bool canScroll)
		{
			this.itemListScroll.SetScrollEnable(canScroll);
		}

		// Token: 0x06009514 RID: 38164 RVA: 0x0045707C File Offset: 0x0045527C
		private void OnClickItem(ITradeableContent content, RowItemLine rowItemLine)
		{
			ViewCharacterMenuItems.<>c__DisplayClass96_0 CS$<>8__locals1 = new ViewCharacterMenuItems.<>c__DisplayClass96_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.content = content;
			bool flag = !base.CharacterMenu.CanOperate;
			if (!flag)
			{
				this.itemListScroll.HandleClickItem(CS$<>8__locals1.content, rowItemLine, new Action<RowItemLine>(CS$<>8__locals1.<OnClickItem>g__Action|0));
			}
		}

		// Token: 0x06009515 RID: 38165 RVA: 0x004570D4 File Offset: 0x004552D4
		private void ShowItemOperateMenu(ItemDisplayData itemData, RowItemLine rowItemLine)
		{
			ViewCharacterMenuItems.<>c__DisplayClass97_0 CS$<>8__locals1 = new ViewCharacterMenuItems.<>c__DisplayClass97_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.rowItemLine = rowItemLine;
			bool selectingTransferItemChar = this.SelectingTransferItemChar;
			if (selectingTransferItemChar)
			{
				base.CharacterMenu.ExitFocusMode(true);
			}
			bool canTransfer = this.CheckItemCanTransfer(itemData);
			CS$<>8__locals1.btnList = new List<ViewPopupMenu.BtnData>();
			Queue<InventoryItemOperationType> optionQueue = new Queue<InventoryItemOperationType>();
			bool isMiscResource = ItemTemplateHelper.IsMiscResource(itemData.Key.ItemType, itemData.Key.TemplateId);
			bool canExchange = ItemTemplateHelper.MiscResourceCanExchange(itemData.Key.ItemType, itemData.Key.TemplateId);
			bool canOperate = canTransfer && itemData.Amount > 0 && (!isMiscResource || canExchange);
			bool curCharacterIsTaiwu = this.CurCharacterIsTaiwu;
			if (curCharacterIsTaiwu)
			{
				this.ShowItemOperateMenuSect(CS$<>8__locals1.btnList, itemData);
				this.ShowItemOperateMenuCommonEvent(CS$<>8__locals1.btnList, itemData);
				this.ShowItemOperateMenuFeed(CS$<>8__locals1.btnList, itemData, CS$<>8__locals1.rowItemLine);
				this.ShowItemOperateMenuRename(CS$<>8__locals1.btnList, itemData, CS$<>8__locals1.rowItemLine);
				this.ShowItemOperateMenuChangeInnerRatio(CS$<>8__locals1.btnList, itemData, CS$<>8__locals1.rowItemLine);
				this.ShowItemOperateMenuIdentify(CS$<>8__locals1.btnList, itemData, CS$<>8__locals1.rowItemLine);
				this.ShowAddAppointment(CS$<>8__locals1.btnList, itemData, CS$<>8__locals1.rowItemLine);
				this.ShowItemOperateMenuTool(CS$<>8__locals1.btnList, itemData, CS$<>8__locals1.rowItemLine);
				this.ShowItemOperateMenuDivineFlame(CS$<>8__locals1.btnList, itemData);
				bool flag = canOperate;
				if (flag)
				{
					this.ShowItemOperateMenuGive(CS$<>8__locals1.btnList, itemData);
					this.ShowItemOperateMenuChoosy(CS$<>8__locals1.btnList, itemData);
					this.ShowItemOperateMenuDiscard(CS$<>8__locals1.btnList, itemData);
					bool flag2 = !isMiscResource;
					if (flag2)
					{
						this.ShowProfessionMenuSelf(CS$<>8__locals1.btnList, itemData, new Action(CS$<>8__locals1.<ShowItemOperateMenu>g__Action|0), optionQueue);
					}
					this.ShowItemOperateMenuLock(CS$<>8__locals1.btnList, itemData);
				}
			}
			else
			{
				bool flag3 = canOperate && !base.CharacterMenu.IsTaiwuSpecialTeammate(base.CharacterMenu.CurCharacterId);
				if (flag3)
				{
					this.ShowItemOperateMenuEnemy(CS$<>8__locals1.btnList, itemData);
					bool flag4 = !isMiscResource;
					if (flag4)
					{
						this.ShowProfessionMenuOther(CS$<>8__locals1.btnList, itemData, new Action(CS$<>8__locals1.<ShowItemOperateMenu>g__Action|0), optionQueue);
					}
				}
			}
			bool isTaiwuTeamButNotBeast = this.IsTaiwuTeamButNotBeast;
			if (isTaiwuTeamButNotBeast)
			{
				this.ShowItemOperateMenuEat(CS$<>8__locals1.btnList, itemData);
				this.ShowItemOperateMenuFeather(CS$<>8__locals1.btnList, itemData, new Action(CS$<>8__locals1.<ShowItemOperateMenu>g__Action|0), optionQueue);
				bool flag5 = !this.CurCharacterIsTaiwu;
				if (flag5)
				{
					bool flag6 = canOperate;
					if (flag6)
					{
						this.ShowItemOperateMenuTake(CS$<>8__locals1.btnList, itemData);
						bool flag7 = !base.CharacterMenu.IsTaiwuGearMate(base.CharacterMenu.CurCharacterId);
						if (flag7)
						{
							this.ShowItemOperateMenuExchange(CS$<>8__locals1.btnList, itemData);
						}
					}
				}
			}
			bool flag8 = optionQueue.Count == 0;
			if (flag8)
			{
				CS$<>8__locals1.<ShowItemOperateMenu>g__Action|0();
			}
		}

		// Token: 0x17001018 RID: 4120
		// (get) Token: 0x06009516 RID: 38166 RVA: 0x0045739A File Offset: 0x0045559A
		private bool IsTaiwuTeamButNotBeast
		{
			get
			{
				return base.CharacterMenu.IsTaiwuTeam && !base.CharacterMenu.IsTaiwuBeastTeammate(base.CharacterMenu.CurCharacterId);
			}
		}

		// Token: 0x06009517 RID: 38167 RVA: 0x004573C8 File Offset: 0x004555C8
		private void ShowItemOperateMenuSect(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData)
		{
			CharacterMenuFunctionControlItem config;
			bool banned = base.CharacterMenu.TryGetFunctionControlConfig(out config) && base.CharacterMenu.IsFunctionBanned(config.SectStory);
			WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
			bool isHeavenlyTreeSeeds = ItemTemplateHelper.CheckIsHeavenlyTreeSeeds(itemData.Key.ItemType, itemData.Key.TemplateId);
			bool flag = isHeavenlyTreeSeeds;
			if (flag)
			{
				bool currentBlockIsFitHeavenlyTree = mapModel.IsCurrentBlockMeetTypeForHeavenlyTree();
				bool isBlockAwayFromHeavenlyTree = mapModel.IsCurrentBlockMeetDistanceForHeavenTree(6);
				bool interactable = currentBlockIsFitHeavenlyTree && isBlockAwayFromHeavenlyTree && !banned;
				string btnName = LocalStringManager.Get(LanguageKey.LK_Plant);
				ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(btnName, interactable, EItemMenuDisplayOrder.Sect, delegate()
				{
					this.CharacterMenu.ExitFocusMode(true);
					UIManager.Instance.HideUI(UIElement.CharacterMenu);
					TaiwuEventDomainMethod.Call.CloseUI("WudangHeavenlyTreeSeed", false, (int)itemData.Key.TemplateId);
					GlobalDomainMethod.Call.InvokeGuidingTrigger(248);
				}, null, null, false);
				btnList.Add(btnData);
				bool flag2 = !isBlockAwayFromHeavenlyTree;
				if (flag2)
				{
					btnData.SetTip(string.Empty, LocalStringManager.Get(LanguageKey.LK_Planting_ShortDistance));
				}
				else
				{
					bool flag3 = !currentBlockIsFitHeavenlyTree;
					if (flag3)
					{
						LanguageKey tipKey = mapModel.CurrentBlockIsLoongMapBlock() ? LanguageKey.LK_NotCanPlanting_LoongMapBlock : LanguageKey.LK_Planting_UncivilizedArea;
						btnData.SetTip(string.Empty, LocalStringManager.Get(tipKey));
					}
					else
					{
						btnData.SetTip(string.Empty, string.Empty);
					}
				}
			}
			else
			{
				bool canTranscribe = SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(28) && ItemTemplateHelper.CheckIsSectMainStoryItemXuannvNotes(itemData.Key.ItemType, itemData.Key.TemplateId);
				bool flag4 = canTranscribe;
				if (flag4)
				{
					bool isEnabled = SingletonObject.getInstance<MusicPlayerModel>().IsEnabled;
					string btnName2 = LocalStringManager.Get(LanguageKey.LK_Option_On);
					ViewPopupMenu.BtnData btnOn = new ViewPopupMenu.BtnData(btnName2, !isEnabled && !banned, EItemMenuDisplayOrder.Sect, new Action(this.OnClickSectMainStoryItemXuannvNotes), null, null, false);
					btnList.Add(btnOn);
					btnName2 = LocalStringManager.Get(LanguageKey.LK_Option_Off);
					ViewPopupMenu.BtnData btnOff = new ViewPopupMenu.BtnData(btnName2, isEnabled && !banned, EItemMenuDisplayOrder.Sect, new Action(this.OnClickSectMainStoryItemXuannvNotes), null, null, false);
					btnList.Add(btnOff);
				}
				else
				{
					bool canOpenMakeWugKing = ItemTemplateHelper.CheckIsSectMainStoryItemWuxianWugFairy(itemData.Key.ItemType, itemData.Key.TemplateId);
					bool flag5 = canOpenMakeWugKing;
					if (flag5)
					{
						string btnName3 = LocalStringManager.Get(LanguageKey.LK_Option_On);
						ViewPopupMenu.BtnData btnOn2 = new ViewPopupMenu.BtnData(btnName3, !banned, EItemMenuDisplayOrder.Sect, new Action(this.OnClickSectMainStoryItemWuxianWugFairy), null, null, false);
						btnList.Add(btnOn2);
					}
					else
					{
						bool findChicken = ItemTemplateHelper.CheckIsSectMainStoryFulongChickenMap(itemData.Key.ItemType, itemData.Key.TemplateId);
						bool flag6 = findChicken;
						if (flag6)
						{
							string btnName4 = LocalStringManager.Get(LanguageKey.LK_Option_FindChicken);
							ViewPopupMenu.BtnData btnOn3 = new ViewPopupMenu.BtnData(btnName4, ViewChickenMap.IsChickenMapInteractable && !ViewChickenMap.AllChickenInTaiwuVillage && !banned, EItemMenuDisplayOrder.Sect, new Action(this.OnClickSectMainStoryItemFulongChickenMap), null, null, false);
							bool allChickenInTaiwuVillage = ViewChickenMap.AllChickenInTaiwuVillage;
							if (allChickenInTaiwuVillage)
							{
								btnOn3.SetTip("", LocalStringManager.Get(LanguageKey.LK_Option_AllChickenGetTips));
							}
							else
							{
								bool flag7 = !ViewChickenMap.IsChickenMapInteractable;
								if (flag7)
								{
									btnOn3.SetTip("", LocalStringManager.Get(LanguageKey.LK_Option_FindChickenTips1));
								}
								else
								{
									btnOn3.SetTip("", "");
								}
							}
							btnList.Add(btnOn3);
						}
						else
						{
							bool damageHugeSword = ItemTemplateHelper.CheckIsDamageHugeSword(itemData.Key.ItemType, itemData.Key.TemplateId);
							bool flag8 = damageHugeSword;
							if (flag8)
							{
								string btnName5 = LocalStringManager.Get(LanguageKey.LK_Option_DamageHugeSword);
								WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
								bool canNotUse = false;
								bool traveling = WorldMapModel.Traveling;
								if (traveling)
								{
									canNotUse = true;
								}
								else
								{
									AdventureRemakeModel adventureRemakeModel = SingletonObject.getInstance<AdventureRemakeModel>();
									bool haveAny = adventureRemakeModel.LocationHaveAnyAdventureOrMajorEvent(worldMapModel.PlayerAtBlock.GetLocation());
									bool flag9 = haveAny;
									if (flag9)
									{
										canNotUse = true;
									}
								}
								ViewPopupMenu.BtnData btnOn4 = new ViewPopupMenu.BtnData(btnName5, !canNotUse && !banned, EItemMenuDisplayOrder.Sect, new Action(this.OnClickDamageHugeSword), null, null, false);
								bool flag10 = canNotUse;
								if (flag10)
								{
									btnOn4.SetTip("", LocalStringManager.Get(LanguageKey.LK_Option_DamageHugeSword_Disable_Content));
								}
								btnList.Add(btnOn4);
							}
							else
							{
								bool canOpenThreeVitals = ItemTemplateHelper.CheckIsSectMainStoryItemYuanshanRosary(itemData.Key.ItemType, itemData.Key.TemplateId);
								bool flag11 = canOpenThreeVitals;
								if (flag11)
								{
									string btnName6 = LocalStringManager.Get(LanguageKey.LK_Option_On);
									ViewPopupMenu.BtnData btnOn5 = new ViewPopupMenu.BtnData(btnName6, !banned, EItemMenuDisplayOrder.Sect, new Action(this.OnClickSectMainStoryItemYuanshanRosary), null, null, false);
									btnList.Add(btnOn5);
								}
								else
								{
									bool isThanksLetter = ItemTemplateHelper.IsThanksLetter(itemData.Key.ItemType, itemData.Key.TemplateId);
									bool flag12 = isThanksLetter;
									if (flag12)
									{
										string btnName7 = LocalStringManager.Get(LanguageKey.LK_Use_TianSuiBaoLuItem);
										ViewPopupMenu.BtnData btnOn6 = new ViewPopupMenu.BtnData(btnName7, true, EItemMenuDisplayOrder.Other, delegate()
										{
											this.OnClickThanksLetter(itemData);
										}, null, null, false);
										btnList.Add(btnOn6);
									}
									else
									{
										bool canOpenQiwenxingdou = ItemTemplateHelper.CheckIsSectMainStoryItemJieQingStars(itemData.Key.ItemType, itemData.Key.TemplateId);
										bool flag13 = canOpenQiwenxingdou;
										if (flag13)
										{
											string btnName8 = LocalStringManager.Get(LanguageKey.LK_Option_On);
											ViewPopupMenu.BtnData btnOn7 = new ViewPopupMenu.BtnData(btnName8, !banned, EItemMenuDisplayOrder.Sect, new Action(this.OnClickSectMainStoryItemJieQingStars), null, null, false);
											btnList.Add(btnOn7);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06009518 RID: 38168 RVA: 0x00457920 File Offset: 0x00455B20
		private void ShowItemOperateMenuCommonEvent(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData)
		{
			ItemKey itemKey = itemData.Key;
			bool flag = !ItemTemplateHelper.CanTriggerCommonEvent(itemKey.ItemType, itemKey.TemplateId);
			if (!flag)
			{
				CharacterMenuFunctionControlItem config;
				bool banned = base.CharacterMenu.TryGetFunctionControlConfig(out config) && base.CharacterMenu.IsFunctionBanned(config.EventTrigger);
				string btnName = LocalStringManager.Get(LanguageKey.LK_Use_Item_Common);
				ViewPopupMenu.BtnData btnOn = new ViewPopupMenu.BtnData(btnName, !banned, EItemMenuDisplayOrder.Use, delegate()
				{
					this.OnClickUseCommonEvent(itemData);
				}, null, null, false);
				btnList.Add(btnOn);
			}
		}

		// Token: 0x06009519 RID: 38169 RVA: 0x004579C0 File Offset: 0x00455BC0
		private unsafe void ShowItemOperateMenuFeed(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData, RowItemLine rowItemLine)
		{
			CharacterMenuFunctionControlItem config;
			bool banned = base.CharacterMenu.TryGetFunctionControlConfig(out config) && base.CharacterMenu.IsFunctionBanned(config.Feed);
			bool flag = ItemTemplateHelper.IsFeedingAble(itemData.Key.ItemType, itemData.Key.TemplateId);
			if (flag)
			{
				bool interactable = (*this.InventoryItems).Any(new Func<ItemDisplayData, bool>(ViewCharacterMenuItems.CheckNeedFeedCarrier)) && !itemData.IsLocked && !banned;
				string btnName = LocalStringManager.Get(LanguageKey.LK_Feeding_Item);
				ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(btnName, interactable, EItemMenuDisplayOrder.Feed, delegate()
				{
					this.OnClickItemEnterMultiplyMode(rowItemLine, ItemOperationType.EItemOperationType.Feeding, null);
				}, null, null, false);
				btnList.Add(btnData);
				bool flag2 = !interactable;
				if (flag2)
				{
					LanguageKey contentKey = itemData.IsLocked ? LanguageKey.LK_Item_State_Locked_Tip : LanguageKey.LK_Feeding_Item_Tip_CarrierNotMeet;
					btnData.SetTip("", LocalStringManager.Get(contentKey).ColorReplace());
				}
			}
			else
			{
				bool flag3 = ItemTemplateHelper.CanFeedCarrier(itemData.Key.ItemType, itemData.Key.TemplateId);
				if (flag3)
				{
					bool needFeed = ViewCharacterMenuItems.CheckNeedFeedCarrier(itemData);
					bool hasFood = this.InventoryItems->Exists((ItemDisplayData d) => ItemTemplateHelper.IsFeedingAble(d.Key.ItemType, d.Key.TemplateId));
					bool interactable2 = needFeed && hasFood && !banned;
					string btnName2 = LocalStringManager.Get(LanguageKey.LK_Feeding_Item);
					ViewPopupMenu.BtnData btnData2 = new ViewPopupMenu.BtnData(btnName2, interactable2, EItemMenuDisplayOrder.Feed, delegate()
					{
						this.OnClickItemEnterMultiplyMode(null, ItemOperationType.EItemOperationType.Feeding, itemData);
					}, null, null, false);
					btnList.Add(btnData2);
					bool flag4 = !interactable2;
					if (flag4)
					{
						LanguageKey key = needFeed ? LanguageKey.LK_Feeding_Item_Tip_MaterialNotMeet : LanguageKey.LK_Feeding_Item_Tip_NoNeed;
						btnData2.SetTip("", LocalStringManager.Get(key));
					}
				}
				else
				{
					bool flag5 = itemData.Key.ItemType == 11 && itemData.CricketData != null && itemData.Durability > 0 && CricketPolymorphHelper.IsCricketPolymorphEnabled;
					if (flag5)
					{
						int spiritMax = GlobalConfig.Instance.CricketSpiritMax;
						bool spiritFull = itemData.CricketData.Spirit >= spiritMax;
						bool hasBloodDew = this.InventoryItems->Exists(new Predicate<ItemDisplayData>(ViewCharacterMenuItems.IsBloodDew));
						bool interactable3 = !spiritFull && hasBloodDew && !itemData.IsLocked && !banned;
						string btnName3 = LocalStringManager.Get(LanguageKey.LK_Feeding_Item);
						ViewPopupMenu.BtnData btnData3 = new ViewPopupMenu.BtnData(btnName3, interactable3, EItemMenuDisplayOrder.Feed, delegate()
						{
							this.OnClickItemEnterMultiplyMode(null, ItemOperationType.EItemOperationType.Feeding, itemData);
						}, null, null, false);
						btnList.Add(btnData3);
						bool flag6 = !interactable3;
						if (flag6)
						{
							LanguageKey key2 = itemData.IsLocked ? LanguageKey.LK_Item_State_Locked_Tip : (spiritFull ? LanguageKey.LK_Cricket_Feeding_Tip_NoNeed : LanguageKey.LK_Cricket_Feeding_Tip_NoBloodDew);
							btnData3.SetTip("", LocalStringManager.Get(key2).ColorReplace());
						}
					}
				}
			}
		}

		// Token: 0x0600951A RID: 38170 RVA: 0x00457CE0 File Offset: 0x00455EE0
		private void ShowItemOperateMenuRename(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData, RowItemLine rowItemLine)
		{
			bool flag = itemData.Key.ItemType == 11 && itemData.CricketData != null;
			if (flag)
			{
				RenameCfg renameConfig = new RenameCfg
				{
					Title = LanguageKey.LK_UI_Following_Rename_Title.Tr(),
					Description = LanguageKey.LK_Cricket_Rename_Desc.TrFormat(itemData.CalcCricketName()),
					EmptyDesc = LanguageKey.LK_Cricket_Rename_Empty.Tr(),
					Default = string.Empty,
					Submit = delegate(string nameStr)
					{
						ItemDomainMethod.Call.SetCricketName(itemData.RealKey, nameStr);
						this.CallRefreshItems(null);
					},
					CharCount = 6
				};
				string btnName = LocalStringManager.Get(LanguageKey.LK_Cricket_Rename_Btn);
				ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(btnName, true, EItemMenuDisplayOrder.Rename, delegate()
				{
					renameConfig.Show();
				}, null, null, false);
				btnList.Add(btnData);
			}
		}

		// Token: 0x0600951B RID: 38171 RVA: 0x00457DD0 File Offset: 0x00455FD0
		private void ShowItemOperateMenuChangeInnerRatio(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData, RowItemLine rowItemLine)
		{
			bool flag = itemData.Key.ItemType == 0;
			if (flag)
			{
				string btnName = LocalStringManager.Get(LanguageKey.LK_ItemUsingOperationType_ChangeInnerRatio);
				ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(btnName, true, EItemMenuDisplayOrder.ChangeInnerRatio, delegate()
				{
					this.itemListScroll.SetItemToSetWeaponInnerRatioMode(rowItemLine);
				}, null, null, false);
				btnList.Add(btnData);
			}
		}

		// Token: 0x0600951C RID: 38172 RVA: 0x00457E34 File Offset: 0x00456034
		public static bool CheckNeedFeedCarrier(ItemDisplayData itemData)
		{
			bool flag = !ItemTemplateHelper.CanFeedCarrier(itemData.Key.ItemType, itemData.Key.TemplateId);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool isDurabilityMax = itemData.Durability >= itemData.MaxDurability;
				CarrierItem carrierConfig = Carrier.Instance[itemData.Key.TemplateId];
				bool isTameMax = carrierConfig.CombatState < 0 || itemData.CarrierTamePoint >= 100;
				bool need = !isDurabilityMax || !isTameMax;
				result = need;
			}
			return result;
		}

		// Token: 0x0600951D RID: 38173 RVA: 0x00457EC0 File Offset: 0x004560C0
		public static bool IsBloodDew(ItemDisplayData itemData)
		{
			bool result;
			if (itemData.Key.ItemType == 12)
			{
				short templateId = itemData.Key.TemplateId;
				result = (templateId >= 9 && templateId <= 17);
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600951E RID: 38174 RVA: 0x00457F00 File Offset: 0x00456100
		private void ShowItemOperateMenuIdentify(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData, RowItemLine rowItemLine)
		{
			bool flag = !ItemTemplateHelper.IsPoisonable(itemData.Key.ItemType, itemData.Key.TemplateId);
			if (!flag)
			{
				int needleAmount = this._characterItemsDisplayData.NeedleAmount;
				string btnName = LocalStringManager.GetFormat(LanguageKey.LK_Item_Identify_Poison, needleAmount);
				bool flag2 = itemData.PoisonEffects != null && itemData.PoisonEffects.IsIdentified;
				bool isInteractable;
				string content;
				if (flag2)
				{
					isInteractable = false;
					content = LocalStringManager.Get(LanguageKey.LK_Poison_Identified);
				}
				else
				{
					int leftDays = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
					bool timeIsMeet = leftDays >= 1;
					bool itemIsMeet = needleAmount >= 1;
					CharacterMenuFunctionControlItem config;
					isInteractable = (timeIsMeet && itemIsMeet && (!base.CharacterMenu.TryGetFunctionControlConfig(out config) || !base.CharacterMenu.IsFunctionBanned(config.Check)));
					string timeStr = leftDays.ToString().SetColor(timeIsMeet ? "brightblue" : "brightred");
					string itemStr = needleAmount.ToString().SetColor(itemIsMeet ? "brightblue" : "brightred");
					content = LocalStringManager.GetFormat(LanguageKey.LK_Poison_Identify_Tip, itemStr, timeStr);
				}
				string title = LocalStringManager.Get(LanguageKey.LK_Poison_Identify);
				ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(btnName, isInteractable, EItemMenuDisplayOrder.Identify, delegate()
				{
					this.OnClickIdentifyItem(rowItemLine);
				}, null, null, true);
				btnData.SetTip(title, content);
				btnList.Add(btnData);
			}
		}

		// Token: 0x0600951F RID: 38175 RVA: 0x00458074 File Offset: 0x00456274
		private void ShowAddAppointment(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData, RowItemLine rowItemLine)
		{
			bool flag = itemData.Key.ItemType != 12 || itemData.Key.TemplateId != 266;
			if (!flag)
			{
				string btnName = LocalStringManager.Get(LanguageKey.LK_Item_Invite_Title);
				int leftDays = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
				bool timeIsMeet = leftDays >= 5;
				bool isInteractable = timeIsMeet;
				string timeStr = leftDays.ToString().SetColor(timeIsMeet ? "brightblue" : "brightred");
				string content = LocalStringManager.GetFormat(LanguageKey.LK_Item_Invite_Content, timeStr, GlobalConfig.Instance.AppointmentCostDays);
				string title = LocalStringManager.Get(LanguageKey.LK_Item_Invite_Title);
				ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(btnName, isInteractable, EItemMenuDisplayOrder.Give, delegate()
				{
					this.OnClickHomingPegon(rowItemLine);
				}, null, null, false);
				btnData.SetTip(title, content);
				btnList.Add(btnData);
			}
		}

		// Token: 0x06009520 RID: 38176 RVA: 0x00458164 File Offset: 0x00456364
		private void ShowItemOperateMenuTool(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData, RowItemLine rowItemLine)
		{
			bool flag = itemData.Key.ItemType == 6 && itemData.Durability > 0;
			if (flag)
			{
				CraftToolItem toolConfig = CraftTool.Instance[itemData.Key.TemplateId];
				bool flag2 = toolConfig.RequiredLifeSkillTypes.Exists((sbyte type) => ViewCharacterMenuItems.CanRepairLifeSkillType.Contains(type));
				if (flag2)
				{
					string btnName = LocalStringManager.Get(LanguageKey.LK_Repair_Item);
					CharacterMenuFunctionControlItem config;
					bool interactable = this.CheckHasRepairableItemForTool(itemData) && (!base.CharacterMenu.TryGetFunctionControlConfig(out config) || !base.CharacterMenu.IsFunctionBanned(config.Repair));
					ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(btnName, interactable, EItemMenuDisplayOrder.Tool, delegate()
					{
						this.OnClickItemEnterMultiplyMode(null, ItemOperationType.EItemOperationType.Repair, null);
					}, null, null, false);
					btnList.Add(btnData);
					bool flag3 = !interactable;
					if (flag3)
					{
						string content = LocalStringManager.Get(LanguageKey.LK_Repair_Item_Tip_NoTarget);
						btnData.SetTip("", content);
					}
				}
				bool flag4 = toolConfig.RequiredLifeSkillTypes.Exists((sbyte type) => ViewCharacterMenuItems.CanDisassembleLifeSkillType.Contains(type));
				if (flag4)
				{
					string btnName2 = LocalStringManager.Get(LanguageKey.LK_Disassemble_Item);
					CharacterMenuFunctionControlItem config2;
					bool interactable2 = this.CheckHasDisassemblableItemForTool(itemData) && (!base.CharacterMenu.TryGetFunctionControlConfig(out config2) || !base.CharacterMenu.IsFunctionBanned(config2.Disassemble));
					ViewPopupMenu.BtnData btnData2 = new ViewPopupMenu.BtnData(btnName2, interactable2, EItemMenuDisplayOrder.Tool, delegate()
					{
						this.OnClickItemEnterMultiplyMode(null, ItemOperationType.EItemOperationType.Disassemble, null);
					}, null, null, false);
					btnList.Add(btnData2);
					bool flag5 = !interactable2;
					if (flag5)
					{
						string content2 = LocalStringManager.Get(LanguageKey.LK_Disassemble_Item_Tip_NoTarget);
						btnData2.SetTip("", content2);
					}
				}
			}
			else
			{
				sbyte skillType = ItemTemplateHelper.GetCraftRequiredLifeSkillType(itemData.Key.ItemType, itemData.Key.TemplateId);
				LifeSkillTypeItem skillConfig = Config.LifeSkillType.Instance[skillType];
				bool isRepairable = ItemTemplateHelper.IsRepairable(itemData.Key.ItemType, itemData.Key.TemplateId);
				bool flag6 = isRepairable;
				if (flag6)
				{
					string btnName3 = LocalStringManager.Get(LanguageKey.LK_Repair_Item);
					bool flag7;
					bool noNeed;
					bool noResource;
					bool hasAnySkillMatchedTool;
					CharacterMenuFunctionControlItem config3;
					bool interactable3 = this.CheckItemRepairCondition(itemData, out flag7, out noNeed, out noResource, out hasAnySkillMatchedTool) && (!base.CharacterMenu.TryGetFunctionControlConfig(out config3) || !base.CharacterMenu.IsFunctionBanned(config3.Repair));
					ViewPopupMenu.BtnData btnData3 = new ViewPopupMenu.BtnData(btnName3, interactable3, EItemMenuDisplayOrder.Tool, delegate()
					{
						this.OnClickItemEnterMultiplyMode(rowItemLine, ItemOperationType.EItemOperationType.Repair, null);
					}, null, null, false);
					btnList.Add(btnData3);
					bool flag8 = !interactable3;
					if (flag8)
					{
						bool flag9 = noNeed;
						string content3;
						if (flag9)
						{
							content3 = LanguageKey.LK_Repair_Item_Tip_NoNeed.Tr();
						}
						else
						{
							bool isLocked = itemData.IsLocked;
							if (isLocked)
							{
								content3 = LanguageKey.LK_Item_State_Locked_Tip.Tr();
							}
							else
							{
								bool flag10 = hasAnySkillMatchedTool;
								if (flag10)
								{
									content3 = LanguageKey.LK_Item_Operation_LifeSkillAttainment_NotMeet.TrFormat(skillConfig.Name).SetColor("brightred");
								}
								else
								{
									bool flag11 = noResource;
									if (flag11)
									{
										content3 = LanguageKey.LK_Repair_Item_Tip_NoResource.Tr();
									}
									else
									{
										content3 = LanguageKey.LK_Repair_Item_Tip_NoTool.Tr();
									}
								}
							}
						}
						btnData3.SetTip("", content3.ColorReplace());
					}
				}
				bool dissemblable = ItemTemplateHelper.GetCanDisassemble(itemData.Key.ItemType, itemData.Key.TemplateId);
				bool flag12 = dissemblable;
				if (flag12)
				{
					string btnName4 = LocalStringManager.Get(LanguageKey.LK_Disassemble_Item);
					bool emptyToolMeet;
					bool hasAnySkillMatchedTool2;
					CharacterMenuFunctionControlItem config4;
					bool interactable4 = this.CheckItemDisassembleCondition(itemData, out emptyToolMeet, out hasAnySkillMatchedTool2) && (!base.CharacterMenu.TryGetFunctionControlConfig(out config4) || !base.CharacterMenu.IsFunctionBanned(config4.Disassemble));
					ViewPopupMenu.BtnData btnData4 = new ViewPopupMenu.BtnData(btnName4, interactable4, EItemMenuDisplayOrder.Tool, delegate()
					{
						this.OnClickItemEnterMultiplyMode(rowItemLine, ItemOperationType.EItemOperationType.Disassemble, null);
					}, null, null, false);
					btnList.Add(btnData4);
					bool flag13 = !interactable4;
					if (flag13)
					{
						bool isLocked2 = itemData.IsLocked;
						string content4;
						if (isLocked2)
						{
							content4 = LanguageKey.LK_Item_State_Locked_Tip.Tr();
						}
						else
						{
							bool flag14 = hasAnySkillMatchedTool2;
							if (flag14)
							{
								content4 = LanguageKey.LK_Item_Operation_LifeSkillAttainment_NotMeet.TrFormat(skillConfig.Name).SetColor("brightred");
							}
							else
							{
								content4 = LanguageKey.LK_Disassemble_Item_Tip_NoTool.Tr();
							}
						}
						btnData4.SetTip("", content4.ColorReplace());
					}
				}
			}
		}

		// Token: 0x06009521 RID: 38177 RVA: 0x004585AC File Offset: 0x004567AC
		private unsafe void ShowItemOperateMenuEnemy(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData)
		{
			StringBuilder sb = EasyPool.Get<StringBuilder>();
			int costTime = GlobalConfig.Instance.HostileOperationTakeItemCostTime;
			int leftTime = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
			bool timeIsMeet = leftTime >= costTime;
			sbyte behaviorType = this.TaiwuCharacterDisplayData.BehaviorType;
			bool ignoreBehavior = EventModel.IgnoreEventBehavior || !SingletonObject.getInstance<BasicGameData>().RestrictOptionsBehaviorType;
			short concentration = *this._characterItemsDisplayData.CurMainAttributes[2];
			short strength = *this._characterItemsDisplayData.CurMainAttributes[0];
			short dexterity = *this._characterItemsDisplayData.CurMainAttributes[1];
			bool scamAttributeIsMeet = concentration >= 20;
			CharacterMenuFunctionControlItem config;
			bool canScam = timeIsMeet && scamAttributeIsMeet && (!base.CharacterMenu.TryGetFunctionControlConfig(out config) || !base.CharacterMenu.IsFunctionBanned(config.Scam));
			string btnName = LocalStringManager.Get(LanguageKey.LK_ItemMenu_Scam);
			ViewPopupMenu.BtnData scamBtnData = new ViewPopupMenu.BtnData(btnName, canScam, EItemMenuDisplayOrder.Hostile, delegate()
			{
				this.OnClickResourceScam(itemData);
			}, null, null, false);
			btnList.Add(scamBtnData);
			sb.Clear();
			bool flag = !timeIsMeet;
			if (flag)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_Making_Time_Not_Enough).ColorReplace());
			}
			bool flag2 = !scamAttributeIsMeet;
			if (flag2)
			{
				string name = CharacterPropertyDisplay.Instance[ECharacterPropertyDisplayType.Concentration.ToInt()].Name;
				sb.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_ItemMenu_NotEnough, name).ColorReplace());
			}
			scamBtnData.SetTip(null, sb.ToString());
			bool stealAttributeIsMeet = dexterity >= 20;
			bool stealBehaviorIsMeet = ignoreBehavior || UI_EventWindow.CheckBehaviorCondition(behaviorType, 3);
			bool canSteal = timeIsMeet && stealAttributeIsMeet && stealBehaviorIsMeet && (!base.CharacterMenu.TryGetFunctionControlConfig(out config) || !base.CharacterMenu.IsFunctionBanned(config.Steal));
			btnName = LocalStringManager.Get(LanguageKey.LK_ItemMenu_Steal);
			ViewPopupMenu.BtnData stealBtnData = new ViewPopupMenu.BtnData(btnName, canSteal, EItemMenuDisplayOrder.Hostile, delegate()
			{
				this.OnClickResourceSteal(itemData);
			}, null, null, false);
			btnList.Add(stealBtnData);
			sb.Clear();
			bool flag3 = !timeIsMeet;
			if (flag3)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_Making_Time_Not_Enough).ColorReplace());
			}
			bool flag4 = !stealAttributeIsMeet;
			if (flag4)
			{
				string name2 = CharacterPropertyDisplay.Instance[ECharacterPropertyDisplayType.Dexterity.ToInt()].Name;
				sb.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_ItemMenu_NotEnough, name2).ColorReplace());
			}
			bool flag5 = !stealBehaviorIsMeet;
			if (flag5)
			{
				string str = UI_EventWindow.GetCharacterBehaviorConditionStr(3, false);
				sb.AppendLine(str);
			}
			stealBtnData.SetTip(null, sb.ToString());
			bool robAttributeIsMeet = strength >= 20;
			bool robBehaviorIsMeet = ignoreBehavior || UI_EventWindow.CheckBehaviorCondition(behaviorType, 4);
			bool canRob = timeIsMeet && robAttributeIsMeet && robBehaviorIsMeet && (!base.CharacterMenu.TryGetFunctionControlConfig(out config) || !base.CharacterMenu.IsFunctionBanned(config.Rob));
			btnName = LocalStringManager.Get(LanguageKey.LK_ItemMenu_Rob);
			ViewPopupMenu.BtnData robBtnData = new ViewPopupMenu.BtnData(btnName, canRob, EItemMenuDisplayOrder.Hostile, delegate()
			{
				this.OnClickResourceRob(itemData);
			}, null, null, false);
			btnList.Add(robBtnData);
			sb.Clear();
			bool flag6 = !timeIsMeet;
			if (flag6)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_Making_Time_Not_Enough).ColorReplace());
			}
			bool flag7 = !robAttributeIsMeet;
			if (flag7)
			{
				string name3 = CharacterPropertyDisplay.Instance[ECharacterPropertyDisplayType.Strength.ToInt()].Name;
				sb.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_ItemMenu_NotEnough, name3).ColorReplace());
			}
			bool flag8 = !robBehaviorIsMeet;
			if (flag8)
			{
				string str2 = UI_EventWindow.GetCharacterBehaviorConditionStr(4, false);
				sb.AppendLine(str2);
			}
			robBtnData.SetTip(null, sb.ToString());
		}

		// Token: 0x06009522 RID: 38178 RVA: 0x00458970 File Offset: 0x00456B70
		private bool CheckEatItemIsLocked(ItemDisplayData itemData, out string tipContent)
		{
			tipContent = string.Empty;
			bool flag = base.CharacterMenu.Element.IsShowing && !base.CharacterMenu.CanOperate;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool isLocked = itemData.IsLocked;
				result = (isLocked || Injury.CheckEatItemIsLocked(base.CharacterMenu.Injury.Data, itemData, (int)UsingMedicineItemType.Invalid, out tipContent));
			}
			return result;
		}

		// Token: 0x06009523 RID: 38179 RVA: 0x004589E0 File Offset: 0x00456BE0
		private void ShowItemOperateMenuEat(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData)
		{
			string limitTip;
			CharacterMenuFunctionControlItem config;
			bool interactable = !this.CheckEatItemIsLocked(itemData, out limitTip) && (!base.CharacterMenu.TryGetFunctionControlConfig(out config) || !base.CharacterMenu.IsFunctionBanned(config.Eat));
			string btnName = CommonUtils.GetCanEatItemButtonName(itemData.RealKey);
			bool flag = itemData.Key.ItemType == 8;
			if (flag)
			{
				ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(btnName, interactable, EItemMenuDisplayOrder.Eat, delegate()
				{
					this.OnClickEatItem(itemData);
				}, delegate()
				{
					this.CharacterMenu.SetEatItemInfectNotice(true, itemData, 1);
				}, delegate()
				{
					this.CharacterMenu.SetEatItemInfectNotice(false, null, 1);
				}, false);
				btnList.Add(btnData);
				bool flag2 = !interactable;
				if (flag2)
				{
					limitTip = (itemData.IsLocked ? LanguageKey.LK_Item_State_Locked_Tip.Tr() : this.GetLongEatLimitTip(itemData.Key, limitTip));
					btnData.SetTip(string.Empty, limitTip);
				}
			}
			else
			{
				bool flag3 = CommonUtils.CanItemEat(itemData.Key.ItemType, itemData.Key.TemplateId, base.CharacterMenu.CurCharacterId);
				if (flag3)
				{
					ViewPopupMenu.BtnData btnData2 = new ViewPopupMenu.BtnData(btnName, interactable, EItemMenuDisplayOrder.Eat, delegate()
					{
						this.OnClickEatItem(itemData);
					}, delegate()
					{
						this.CharacterMenu.SetEatItemInfectNotice(true, itemData, 1);
					}, delegate()
					{
						this.CharacterMenu.SetEatItemInfectNotice(false, null, 1);
					}, false);
					btnList.Add(btnData2);
					bool flag4 = !interactable;
					if (flag4)
					{
						limitTip = (itemData.IsLocked ? LanguageKey.LK_Item_State_Locked_Tip.Tr() : this.GetLongEatLimitTip(itemData.Key, limitTip));
						btnData2.SetTip(string.Empty, limitTip);
					}
				}
			}
		}

		// Token: 0x06009524 RID: 38180 RVA: 0x00458BAC File Offset: 0x00456DAC
		private void ShowItemOperateMenuFeather(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData, Action onShow, Queue<InventoryItemOperationType> optionQueue)
		{
			bool flag = itemData.Key.ItemType != 5;
			if (!flag)
			{
				bool flag2 = itemData.Key.TemplateId < 343 || itemData.Key.TemplateId > 349;
				if (!flag2)
				{
					optionQueue.Enqueue(InventoryItemOperationType.Feather);
					Action <>9__2;
					BuildingDomainMethod.AsyncCall.CanUseChickenFeather(this, this._characterItemsDisplayData.CharacterDisplayData.CharacterId, (sbyte)(itemData.Key.TemplateId - 343), delegate(int valueOffset, RawDataPool dataPool)
					{
						bool interactable = false;
						Serializer.Deserialize(dataPool, valueOffset, ref interactable);
						string btnName = LanguageKey.LK_Use.Tr();
						string name = btnName;
						bool interactable2 = interactable;
						EItemMenuDisplayOrder displayOrder = EItemMenuDisplayOrder.Eat;
						Action onClick;
						if ((onClick = <>9__2) == null)
						{
							onClick = (<>9__2 = delegate()
							{
								BuildingDomainMethod.Call.UseChickenFeather(this.Element.GameDataListenerId, this._characterItemsDisplayData.CharacterDisplayData.CharacterId, itemData.Key, (sbyte)(itemData.Key.TemplateId - 343));
								this.CallRefreshItems(null);
							});
						}
						ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(name, interactable2, displayOrder, onClick, null, null, false);
						btnList.Add(btnData);
						bool flag3 = !interactable;
						if (flag3)
						{
							btnData.SetTip(string.Empty, LanguageKey.LK_ChickenCantUseFeather.Tr());
						}
						base.<ShowItemOperateMenuFeather>g__Dequeue|1();
					});
				}
			}
		}

		// Token: 0x06009525 RID: 38181 RVA: 0x00458C80 File Offset: 0x00456E80
		private string GetLongEatLimitTip(ItemKey itemKey, string limitTip)
		{
			bool flag = ItemTemplateHelper.IsTianJieFuLu(itemKey.ItemType, itemKey.TemplateId) && limitTip.Equals(LanguageKey.LK_UsingMedicine_Tip_CountLess.Tr());
			if (flag)
			{
				limitTip = LanguageKey.LK_Mousetip_TianjieFulu_NotEnough.Tr().SetColor("brightred");
			}
			else
			{
				bool flag2 = !limitTip.Equals(LanguageKey.LK_Use_Medicine_Tip_NotAllow.Tr());
				if (flag2)
				{
					limitTip += LanguageKey.LK_Ignore.Tr().SetColor("brightred");
				}
			}
			return limitTip;
		}

		// Token: 0x06009526 RID: 38182 RVA: 0x00458D08 File Offset: 0x00456F08
		private void ShowItemOperateMenuTake(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData)
		{
			bool isTaiwuGearMate = base.CharacterMenu.IsTaiwuGearMate(base.CharacterMenu.CurCharacterId);
			int limitSelectCount;
			CharacterMenuFunctionControlItem config;
			bool interactable = ViewCharacterMenuItems.CanTakeItem(itemData, this.MaxWorthCanBeLentToTaiwu, isTaiwuGearMate, out limitSelectCount) && (!base.CharacterMenu.TryGetFunctionControlConfig(out config) || !base.CharacterMenu.IsFunctionBanned(config.Take));
			string btnName = LocalStringManager.Get(LanguageKey.LK_TakeFrom_Item);
			ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(btnName, interactable, EItemMenuDisplayOrder.Take, delegate()
			{
				this.OnClickTakeItemFrom(itemData, limitSelectCount);
			}, delegate()
			{
				bool flag = itemData.Amount == 1;
				if (flag)
				{
					GEvent.OnEvent(UiEvents.OnPointEnterItemMenuTake, null);
				}
			}, delegate()
			{
				bool flag = itemData.Amount == 1;
				if (flag)
				{
					GEvent.OnEvent(UiEvents.OnPointExitItemMenuTake, null);
				}
			}, false);
			btnList.Add(btnData);
		}

		// Token: 0x06009527 RID: 38183 RVA: 0x00458DCC File Offset: 0x00456FCC
		public static bool CanTakeItem(ItemDisplayData itemData, long maxWorth, bool isTaiwuGearMate, out int limitSelectCount)
		{
			limitSelectCount = itemData.Amount;
			bool flag = limitSelectCount == 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool interactable;
				if (isTaiwuGearMate)
				{
					interactable = true;
				}
				else
				{
					bool isMiscResource = ItemTemplateHelper.IsMiscResource(itemData.Key.ItemType, itemData.Key.TemplateId);
					bool flag2 = isMiscResource;
					long itemValue;
					if (flag2)
					{
						sbyte resourceType = ItemTemplateHelper.GetMiscResourceType(itemData.Key.ItemType, itemData.Key.TemplateId);
						itemValue = Debts.ResourceAmountToWorth((short)resourceType, 1);
						limitSelectCount = Debts.WorthToResourceAmount((short)resourceType, maxWorth, false);
					}
					else
					{
						itemValue = itemData.Value;
						bool flag3 = itemValue > 0L;
						if (flag3)
						{
							limitSelectCount = (int)Math.Min(maxWorth / itemValue, 2147483647L);
						}
					}
					interactable = (itemValue <= maxWorth);
				}
				result = interactable;
			}
			return result;
		}

		// Token: 0x06009528 RID: 38184 RVA: 0x00458E94 File Offset: 0x00457094
		private void ShowItemOperateMenuExchange(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData)
		{
			CharacterMenuFunctionControlItem config;
			bool interactable = !base.CharacterMenu.TryGetFunctionControlConfig(out config) || !base.CharacterMenu.IsFunctionBanned(config.Exchange);
			string btnName = LocalStringManager.Get(LanguageKey.LK_Bottom_Exchange);
			ViewPopupMenu.BtnData btnOn = new ViewPopupMenu.BtnData(btnName, interactable, EItemMenuDisplayOrder.Resource, new Action(this.OnClickResourceExchange), null, null, false);
			btnList.Add(btnOn);
		}

		// Token: 0x06009529 RID: 38185 RVA: 0x00458EF4 File Offset: 0x004570F4
		private void ShowItemOperateMenuChoosy(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData)
		{
			bool flag = ItemTemplateHelper.MiscResourceCanChoosy(itemData.Key.ItemType, itemData.Key.TemplateId);
			if (flag)
			{
				sbyte type = ItemTemplateHelper.GetMiscResourceType(itemData.Key.ItemType, itemData.Key.TemplateId);
				CharacterMenuFunctionControlItem config;
				bool interactable = !base.CharacterMenu.TryGetFunctionControlConfig(out config) || !base.CharacterMenu.IsFunctionBanned(config.Filter);
				string btnName = LocalStringManager.Get(LanguageKey.LK_Resource_Choosy_ItemMenu);
				ViewPopupMenu.BtnData btnOn = new ViewPopupMenu.BtnData(btnName, interactable, EItemMenuDisplayOrder.Resource, delegate()
				{
					this.OnClickResourceChoosy();
				}, null, null, false);
				btnList.Add(btnOn);
			}
		}

		// Token: 0x0600952A RID: 38186 RVA: 0x00458F98 File Offset: 0x00457198
		private void ShowItemOperateMenuGive(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData)
		{
			bool hasTeammate = SingletonObject.getInstance<CharacterMonitorModel>().GetTaiwuTeamCharIds().Count > 1 || SingletonObject.getInstance<CharacterMonitorModel>().GetTaiwuSpecialGroup().Count > 0;
			string btnName = LocalStringManager.Get(LanguageKey.LK_Transfer_Item);
			CharacterMenuFunctionControlItem config;
			bool interactable = !itemData.IsLocked && hasTeammate && (!base.CharacterMenu.TryGetFunctionControlConfig(out config) || !base.CharacterMenu.IsFunctionBanned(config.Gift));
			ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(btnName, interactable, EItemMenuDisplayOrder.Give, delegate()
			{
				this.OnClickTransferItem(itemData);
			}, null, null, false);
			btnList.Add(btnData);
			bool flag = !interactable && (itemData.IsLocked || !hasTeammate);
			if (flag)
			{
				LanguageKey contentKey = itemData.IsLocked ? LanguageKey.LK_Item_State_Locked_Tip : LanguageKey.LK_Transfer_Item_Tip_NoTargetCharacter;
				btnData.SetTip(string.Empty, LocalStringManager.Get(contentKey).ColorReplace());
			}
		}

		// Token: 0x0600952B RID: 38187 RVA: 0x004590A4 File Offset: 0x004572A4
		private void ShowItemOperateMenuDiscard(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData)
		{
			bool inTutorial = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			string btnName = LocalStringManager.Get(LanguageKey.LK_Discard_Item);
			CharacterMenuFunctionControlItem config;
			bool interactable = !inTutorial && !itemData.IsLocked && (!base.CharacterMenu.TryGetFunctionControlConfig(out config) || !base.CharacterMenu.IsFunctionBanned(config.Drop));
			ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(btnName, interactable, EItemMenuDisplayOrder.Discard, delegate()
			{
				this.OnClickDiscard(itemData);
			}, null, null, false);
			bool isLocked = itemData.IsLocked;
			if (isLocked)
			{
				btnData.SetTip(string.Empty, LanguageKey.LK_Item_State_Locked_Tip.Tr().ColorReplace());
			}
			btnList.Add(btnData);
		}

		// Token: 0x0600952C RID: 38188 RVA: 0x00459168 File Offset: 0x00457368
		private void ShowItemOperateMenuLock(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData)
		{
			bool flag = !this.CanShowLock(itemData);
			if (!flag)
			{
				bool interactable = this.CanClickLock(itemData);
				string btnName = itemData.IsLocked ? LanguageKey.LK_Unlock_Item.Tr() : LanguageKey.LK_Lock_Item.Tr();
				ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(btnName, interactable, EItemMenuDisplayOrder.Lock, delegate()
				{
					this.OnClickLock(itemData);
				}, null, null, false);
				string tipContent = itemData.IsLocked ? LanguageKey.LK_Unlock_Item_Tip.TrFormat(CommonCommandKit.SecondaryInteraction.ToString()) : LanguageKey.LK_Lock_Item_Tip.TrFormat(CommonCommandKit.SecondaryInteraction.ToString());
				btnData.SetTip(string.Empty, tipContent);
				btnList.Add(btnData);
			}
		}

		// Token: 0x0600952D RID: 38189 RVA: 0x00459240 File Offset: 0x00457440
		private void ShowItemOperateMenuDivineFlame(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData)
		{
			bool flag = itemData.RealKey.ItemType != 12;
			if (!flag)
			{
				short templateId = itemData.RealKey.TemplateId;
				bool flag2 = templateId < 229 || templateId > 238;
				if (!flag2)
				{
					sbyte xiangshuAvatarId = Convert.ToSByte((int)(itemData.RealKey.TemplateId - 229));
					int date = SingletonObject.getInstance<BasicGameData>().CurrDate;
					bool isCooldownEnd = this._characterItemsDisplayData.DivineFlameData.IsCooldownEnd(date);
					bool isGood = SingletonObject.getInstance<BasicGameData>().IsXiangshuAvatarTaskStatusGood((int)xiangshuAvatarId);
					bool isTargetMeet = this._characterItemsDisplayData.DivineFlameTargetState[(int)xiangshuAvatarId];
					bool interactable = isCooldownEnd && isTargetMeet;
					string btnName = isGood ? LanguageKey.LK_Story_Sword_Use_Good.Tr() : LanguageKey.LK_Story_Sword_Use_Bad.Tr();
					ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(btnName, interactable, EItemMenuDisplayOrder.Other, delegate()
					{
						this.OnClickDivineFlame(xiangshuAvatarId);
					}, null, null, false);
					StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
					bool flag3 = !isCooldownEnd;
					if (flag3)
					{
						stringBuilder.AppendLine(LanguageKey.LK_Story_Sword_Cooldowning.Tr().SetColor("brightred"));
					}
					bool flag4 = !isTargetMeet;
					if (flag4)
					{
						DivineFlameData.TargetType targetType = DivineFlameData.GetTargetType(xiangshuAvatarId, isGood);
						if (!true)
						{
						}
						LanguageKey languageKey;
						switch (targetType)
						{
						case DivineFlameData.TargetType.SelectCharacter:
							languageKey = LanguageKey.LK_Story_Sword_SelectTarget_NoChar;
							break;
						case DivineFlameData.TargetType.SelectBlock:
							languageKey = LanguageKey.LK_Story_Sword_SelectTarget_NoBlock;
							break;
						case DivineFlameData.TargetType.CheckCharacter:
							languageKey = LanguageKey.LK_Story_Sword_SelectTarget_NoChar;
							break;
						case DivineFlameData.TargetType.CheckBlock:
							languageKey = LanguageKey.LK_Story_Sword_SelectTarget_NoBlock;
							break;
						default:
							languageKey = LanguageKey.Invalid;
							break;
						}
						if (!true)
						{
						}
						LanguageKey key = languageKey;
						stringBuilder.AppendLine(key.Tr().SetColor("brightred"));
					}
					string tipContent = stringBuilder.ToString();
					btnData.SetTip(string.Empty, tipContent);
					btnList.Add(btnData);
				}
			}
		}

		// Token: 0x0600952E RID: 38190 RVA: 0x00459418 File Offset: 0x00457618
		private void ShowProfessionMenuOther(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData, Action onShow, Queue<InventoryItemOperationType> queue)
		{
			this.AddProfessionButtonData(60, InventoryItemOperationType.ProfessionCapitalistSkill0, itemData, btnList, queue, onShow);
		}

		// Token: 0x0600952F RID: 38191 RVA: 0x0045942C File Offset: 0x0045762C
		private void ShowProfessionMenuSelf(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData, Action onShow, Queue<InventoryItemOperationType> optionQueue)
		{
			short subType = ItemTemplateHelper.GetItemSubType(itemData.Key.ItemType, itemData.Key.TemplateId);
			bool flag = itemData.Key.ItemType == 11;
			if (flag)
			{
				this.AddProfessionButtonData(68, InventoryItemOperationType.ProfessionDukeSkill0_0, itemData, btnList, optionQueue, onShow);
				this.AddProfessionButtonData(68, InventoryItemOperationType.ProfessionDukeSkill0_1, itemData, btnList, optionQueue, onShow);
			}
			else
			{
				bool flag2 = itemData.CanSetEquipmentEffect();
				if (flag2)
				{
					this.AddProfessionButtonData(10, InventoryItemOperationType.ProfessionCraftSkill2, itemData, btnList, optionQueue, onShow);
				}
				else
				{
					bool flag3 = subType == 402;
					if (flag3)
					{
						this.AddProfessionButtonData(7, InventoryItemOperationType.ProfessionHunterSkill3, itemData, btnList, optionQueue, onShow);
					}
				}
			}
		}

		// Token: 0x06009530 RID: 38192 RVA: 0x004594C4 File Offset: 0x004576C4
		private unsafe void AddProfessionButtonData(short professionSkillTemplateId, InventoryItemOperationType operationType, ItemDisplayData itemData, List<ViewPopupMenu.BtnData> btnList, Queue<InventoryItemOperationType> optionQueue, Action onShow)
		{
			ViewCharacterMenuItems.<>c__DisplayClass126_0 CS$<>8__locals1 = new ViewCharacterMenuItems.<>c__DisplayClass126_0();
			CS$<>8__locals1.itemData = itemData;
			CS$<>8__locals1.professionSkillTemplateId = professionSkillTemplateId;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.operationType = operationType;
			CS$<>8__locals1.btnList = btnList;
			CS$<>8__locals1.optionQueue = optionQueue;
			CS$<>8__locals1.onShow = onShow;
			ProfessionModel professionModel = SingletonObject.getInstance<ProfessionModel>();
			bool flag = !professionModel.IsProfessionalSkillUnlockedAndEquipped((int)CS$<>8__locals1.professionSkillTemplateId);
			if (!flag)
			{
				int remainTime = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
				int currDate = SingletonObject.getInstance<BasicGameData>().CurrDate;
				ProfessionSkillItem skillConfig = ProfessionSkill.Instance[(int)CS$<>8__locals1.professionSkillTemplateId];
				ProfessionData professionData = professionModel.GetProfessionData(skillConfig.Profession);
				int skillIndex = professionData.GetSkillIndex((int)CS$<>8__locals1.professionSkillTemplateId);
				CS$<>8__locals1.interactable = true;
				CS$<>8__locals1.color = "brightred";
				CS$<>8__locals1.stringBuilder = EasyPool.Get<StringBuilder>();
				bool isSkillCooldown = professionData.IsSkillCooldown(currDate, skillIndex);
				bool flag2 = isSkillCooldown;
				if (flag2)
				{
					CS$<>8__locals1.stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_NotCoolDown).SetColor(CS$<>8__locals1.color));
					CS$<>8__locals1.interactable = false;
				}
				short timeCost = skillConfig.TimeCost;
				bool isTimeMeet = timeCost <= 0 || remainTime >= (int)timeCost;
				bool flag3 = !isTimeMeet;
				if (flag3)
				{
					CS$<>8__locals1.stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_Time_NotMeet).SetColor(CS$<>8__locals1.color));
					CS$<>8__locals1.interactable = false;
				}
				foreach (ResourceInfo resourceInfo in skillConfig.ResourcesCost)
				{
					bool flag4 = resourceInfo.ResourceCount > *this._characterItemsDisplayData.Resources[(int)resourceInfo.ResourceType];
					if (flag4)
					{
						CS$<>8__locals1.stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_Resource_NotMeet).SetColor(CS$<>8__locals1.color));
						CS$<>8__locals1.interactable = false;
						break;
					}
				}
				bool flag5 = skillConfig.ExpCost > this.Exp;
				if (flag5)
				{
					CS$<>8__locals1.stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Exp_Not_Enough).SetColor(CS$<>8__locals1.color));
					CS$<>8__locals1.interactable = false;
				}
				bool isFavorMeet = this.CurCharacterIsTaiwu || this._characterItemsDisplayData.CharacterDisplayData.FavorabilityToTaiwu >= skillConfig.RequiredFavorability;
				bool flag6 = !isFavorMeet;
				if (flag6)
				{
					CS$<>8__locals1.stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Item_Operation_Favorability_NotMeet).SetColor(CS$<>8__locals1.color));
					CS$<>8__locals1.interactable = false;
				}
				CS$<>8__locals1.btnName = ProfessionSkill.Instance[(int)CS$<>8__locals1.professionSkillTemplateId].Name;
				switch (CS$<>8__locals1.operationType)
				{
				case InventoryItemOperationType.ProfessionCapitalistSkill0:
				{
					int grade = professionData.GetSeniorityOrgGrade();
					bool isSeniorityMeet = grade >= (int)this._characterItemsDisplayData.CharacterDisplayData.OrgInfo.Grade;
					bool flag7 = !isSeniorityMeet;
					if (flag7)
					{
						CS$<>8__locals1.stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Item_Operation_ProfessionSeniority_NotMeet).SetColor(CS$<>8__locals1.color));
						CS$<>8__locals1.interactable = false;
					}
					CS$<>8__locals1.<AddProfessionButtonData>g__AddButton|0(CS$<>8__locals1.btnName, CS$<>8__locals1.interactable, null);
					break;
				}
				case InventoryItemOperationType.ProfessionDukeSkill0_0:
				{
					CS$<>8__locals1.<AddProfessionButtonData>g__EnQueue|1();
					bool flag8 = CS$<>8__locals1.itemData.Durability <= 0;
					if (flag8)
					{
						CS$<>8__locals1.stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Item_Operation_Cricket_Dead).SetColor(CS$<>8__locals1.color));
					}
					ExtraDomainMethod.AsyncCall.CanIdentifyCricket(this, CS$<>8__locals1.itemData.RealKey.Id, delegate(int valueOffset, RawDataPool dataPool)
					{
						bool canIdentifyCricket = false;
						Serializer.Deserialize(dataPool, valueOffset, ref canIdentifyCricket);
						CS$<>8__locals1.interactable = (CS$<>8__locals1.interactable && canIdentifyCricket);
						bool flag11 = !canIdentifyCricket;
						if (flag11)
						{
							CS$<>8__locals1.stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Item_Operation_IdentifyCricket_NotMeet).SetColor(CS$<>8__locals1.color));
						}
						string btnName = LocalStringManager.Get(LanguageKey.LK_Item_Operation_IdentifyCricket);
						base.<AddProfessionButtonData>g__AddButton|0(btnName, CS$<>8__locals1.interactable, null);
						base.<AddProfessionButtonData>g__Dequeue|2();
					});
					break;
				}
				case InventoryItemOperationType.ProfessionDukeSkill0_1:
				{
					CS$<>8__locals1.<AddProfessionButtonData>g__EnQueue|1();
					bool flag9 = CS$<>8__locals1.itemData.Durability <= 0;
					if (flag9)
					{
						CS$<>8__locals1.stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Item_Operation_Cricket_Dead).SetColor(CS$<>8__locals1.color));
					}
					ExtraDomainMethod.AsyncCall.CanUpgradeCricket(this, CS$<>8__locals1.itemData.RealKey.Id, delegate(int valueOffset, RawDataPool dataPool)
					{
						bool canUpgradeCricket = false;
						Serializer.Deserialize(dataPool, valueOffset, ref canUpgradeCricket);
						CS$<>8__locals1.interactable = (CS$<>8__locals1.interactable && canUpgradeCricket);
						bool flag11 = !canUpgradeCricket;
						if (flag11)
						{
							CS$<>8__locals1.stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Item_Operation_UpgradeCricket_NotMeet).SetColor(CS$<>8__locals1.color));
						}
						string btnName = LocalStringManager.Get(LanguageKey.LK_Item_Operation_UpgradeCricket);
						base.<AddProfessionButtonData>g__AddButton|0(btnName, CS$<>8__locals1.interactable, null);
						base.<AddProfessionButtonData>g__Dequeue|2();
					});
					break;
				}
				case InventoryItemOperationType.ProfessionCraftSkill2:
				{
					bool flag10 = CS$<>8__locals1.itemData.RealKey.ItemType == 0;
					if (flag10)
					{
						CS$<>8__locals1.<AddProfessionButtonData>g__AddButton|0(CS$<>8__locals1.btnName, CS$<>8__locals1.interactable, delegate
						{
							ArgumentBox args = EasyPool.Get<ArgumentBox>().Set<ItemKey>("SelectedItemKey", CS$<>8__locals1.itemData.RealKey);
							UIElement.ChangeWeaponTrick.SetOnInitArgs(args);
							UIManager.Instance.ShowUI(UIElement.ChangeWeaponTrick, true);
						});
					}
					break;
				}
				case InventoryItemOperationType.ProfessionHunterSkill3:
					CS$<>8__locals1.<AddProfessionButtonData>g__EnQueue|1();
					ExtraDomainMethod.AsyncCall.CanConvertToAnimalCharacter(this, CS$<>8__locals1.itemData.RealKey, delegate(int valueOffset, RawDataPool dataPool)
					{
						bool canConvertToAnimalCharacter = false;
						Serializer.Deserialize(dataPool, valueOffset, ref canConvertToAnimalCharacter);
						CS$<>8__locals1.interactable = (CS$<>8__locals1.interactable && canConvertToAnimalCharacter);
						bool flag11 = !canConvertToAnimalCharacter;
						if (flag11)
						{
							CS$<>8__locals1.stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Item_Operation_ConvertToAnimalCharacter_NotMeet).SetColor(CS$<>8__locals1.color));
						}
						CS$<>8__locals1.btnName = ProfessionSkill.Instance[(int)CS$<>8__locals1.professionSkillTemplateId].Name;
						base.<AddProfessionButtonData>g__AddButton|0(CS$<>8__locals1.btnName, CS$<>8__locals1.interactable, null);
						base.<AddProfessionButtonData>g__Dequeue|2();
					});
					break;
				default:
					CS$<>8__locals1.<AddProfessionButtonData>g__AddButton|0(CS$<>8__locals1.btnName, CS$<>8__locals1.interactable, null);
					break;
				}
			}
		}

		// Token: 0x06009531 RID: 38193 RVA: 0x00459954 File Offset: 0x00457B54
		private unsafe bool CheckHasRepairableItemForTool(ItemDisplayData toolData)
		{
			return this.InventoryItems->Exists(delegate(ItemDisplayData itemData)
			{
				bool isRepairable = ItemTemplateHelper.IsRepairable(itemData.Key.ItemType, itemData.Key.TemplateId);
				bool flag = !isRepairable;
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					bool flag2 = itemData.Durability >= itemData.MaxDurability;
					if (flag2)
					{
						result = false;
					}
					else
					{
						LifeSkillShorts lifeSkillAttainments = this._characterItemsDisplayData.LifeSkillAttainments;
						sbyte b;
						short num;
						result = ViewCharacterMenuItems.CheckCurrentToolAttainment(ItemOperationType.EItemOperationType.Repair, toolData, itemData, lifeSkillAttainments, out b, out num);
					}
				}
				return result;
			});
		}

		// Token: 0x06009532 RID: 38194 RVA: 0x00459994 File Offset: 0x00457B94
		private unsafe bool CheckHasDisassemblableItemForTool(ItemDisplayData toolData)
		{
			return this.InventoryItems->Exists(delegate(ItemDisplayData itemData)
			{
				bool dissemblable = ItemTemplateHelper.GetCanDisassemble(itemData.Key.ItemType, itemData.Key.TemplateId);
				bool flag = !dissemblable;
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					LifeSkillShorts lifeSkillAttainments = this._characterItemsDisplayData.LifeSkillAttainments;
					sbyte b;
					short num;
					result = ViewCharacterMenuItems.CheckCurrentToolAttainment(ItemOperationType.EItemOperationType.Disassemble, toolData, itemData, lifeSkillAttainments, out b, out num);
				}
				return result;
			});
		}

		// Token: 0x06009533 RID: 38195 RVA: 0x004599D4 File Offset: 0x00457BD4
		private bool CheckItemRepairCondition(ItemDisplayData itemData, out bool emptyToolMeet, out bool noNeed, out bool noResource, out bool hasAnySkillMatchedTool)
		{
			emptyToolMeet = false;
			noNeed = false;
			noResource = false;
			hasAnySkillMatchedTool = false;
			bool flag = itemData.Durability >= itemData.MaxDurability;
			bool result;
			if (flag)
			{
				noNeed = true;
				result = false;
			}
			else
			{
				ResourceInts needResource = ItemTemplateHelper.GetRepairNeedResources(itemData.MaterialResources, itemData.Key, itemData.Durability);
				ResourceInts curResource = this._characterItemsDisplayData.Resources;
				bool flag2 = !curResource.CheckIsMeet(ref needResource);
				if (flag2)
				{
					noResource = true;
					result = false;
				}
				else
				{
					bool toolMeet = this.CheckInventoryTool(ItemOperationType.EItemOperationType.Repair, itemData, out emptyToolMeet, out hasAnySkillMatchedTool);
					result = toolMeet;
				}
			}
			return result;
		}

		// Token: 0x06009534 RID: 38196 RVA: 0x00459A64 File Offset: 0x00457C64
		private bool CheckItemDisassembleCondition(ItemDisplayData itemData, out bool emptyToolMeet, out bool hasAnySkillMatchedTool)
		{
			emptyToolMeet = false;
			hasAnySkillMatchedTool = false;
			bool isLocked = itemData.IsLocked;
			bool result;
			if (isLocked)
			{
				result = false;
			}
			else
			{
				bool toolMeet = this.CheckInventoryTool(ItemOperationType.EItemOperationType.Disassemble, itemData, out emptyToolMeet, out hasAnySkillMatchedTool);
				result = toolMeet;
			}
			return result;
		}

		// Token: 0x06009535 RID: 38197 RVA: 0x00459A98 File Offset: 0x00457C98
		private unsafe bool CheckInventoryTool(ItemOperationType.EItemOperationType operationType, ItemDisplayData itemData, out bool emptyToolMeet, out bool hasAnySkillMatchedTool)
		{
			sbyte skillType = ItemTemplateHelper.GetCraftRequiredLifeSkillType(itemData.Key.ItemType, itemData.Key.TemplateId);
			short skillAttainment = *this._characterItemsDisplayData.LifeSkillAttainments[(int)skillType];
			short requiredAttainment = ViewCharacterMenuItems.GetOperationRequiredAttainment(operationType, itemData);
			hasAnySkillMatchedTool = false;
			short finalAttainment = UI_Make.GetFinalAttainment(-1, skillAttainment, skillType);
			emptyToolMeet = (finalAttainment >= requiredAttainment);
			bool flag = emptyToolMeet;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				List<ItemDisplayData> skillMatchedToolList = (*this.InventoryItems).Where(delegate(ItemDisplayData item)
				{
					bool flag2 = item.Key.ItemType != 6 || item.IsLocked;
					bool result2;
					if (flag2)
					{
						result2 = false;
					}
					else
					{
						CraftToolItem toolConfig = CraftTool.Instance[item.Key.TemplateId];
						bool flag3 = !toolConfig.RequiredLifeSkillTypes.Contains(skillType);
						if (flag3)
						{
							result2 = false;
						}
						else
						{
							bool flag4 = item.Durability < 0;
							result2 = !flag4;
						}
					}
					return result2;
				}).ToList<ItemDisplayData>();
				hasAnySkillMatchedTool = (skillMatchedToolList.Count > 0);
				bool toolMeet = skillMatchedToolList.Any(delegate(ItemDisplayData item)
				{
					short finalAttainment2 = UI_Make.GetFinalAttainment(item.Key.TemplateId, skillAttainment, skillType);
					return finalAttainment2 >= requiredAttainment;
				});
				result = toolMeet;
			}
			return result;
		}

		// Token: 0x06009536 RID: 38198 RVA: 0x00459B70 File Offset: 0x00457D70
		private void OnClickItemEnterMultiplyMode(RowItemLine rowItemLine, ItemOperationType.EItemOperationType operationType, ItemDisplayData feedingTarget = null)
		{
			UIManager.Instance.HideUI(UIElement.PopupMenu);
			this.ExitItemOperation(false);
			if (operationType != ItemOperationType.EItemOperationType.Repair)
			{
				if (operationType != ItemOperationType.EItemOperationType.Disassemble)
				{
					if (operationType != ItemOperationType.EItemOperationType.Feeding)
					{
						throw new ArgumentOutOfRangeException("operationType", operationType, null);
					}
					this.multiplyItemListScroll.EnterFeedingMode(feedingTarget);
				}
				else
				{
					this.multiplyItemListScroll.EnterDisassembleMode();
				}
			}
			else
			{
				this.multiplyItemListScroll.EnterRepairMode();
			}
			bool flag = ((rowItemLine == null) ? ItemKey.Invalid : rowItemLine.Data.RealKey).IsValid();
			if (flag)
			{
				this._clickedItemData = rowItemLine.Data;
				int index = this.itemListScroll.FindItemIndex(this._clickedItemData);
				this.itemListScroll.InfiniteScroll.Refresh(index);
				this.itemListScroll.InfiniteScroll.OnRenderEnd += this.ClickOnRenderEnd;
			}
		}

		// Token: 0x06009537 RID: 38199 RVA: 0x00459C5C File Offset: 0x00457E5C
		private void ClickOnRenderEnd()
		{
			bool flag = this._clickedItemData == null;
			if (!flag)
			{
				RowItemLine activeItem = this.itemListScroll.FindActiveItem(this._clickedItemData, false);
				bool flag2 = activeItem == null;
				if (!flag2)
				{
					this._clickedItemData = null;
					this.itemListScroll.InfiniteScroll.OnRenderEnd -= this.ClickOnRenderEnd;
					SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
					{
						this.itemListScroll.Click(activeItem.Data);
					});
				}
			}
		}

		// Token: 0x06009538 RID: 38200 RVA: 0x00459CEC File Offset: 0x00457EEC
		private void OnClickDiscard(ItemDisplayData itemData)
		{
			bool flag = itemData.Amount > 1;
			if (flag)
			{
				ViewCharacterMenuItems._currItemOperation = ItemOperationType.EItemOperationType.Discard;
				RowItemLine rowItemLine = this.itemListScroll.FindActiveItem(itemData, false);
				this.itemListScroll.SetItemToSelectCountMode(rowItemLine, delegate(int count)
				{
					bool flag2 = count > 0;
					if (flag2)
					{
						this.ShowDiscardItemConfirmDialog(itemData, count);
					}
				}, new Action(this.OnExitFocusMode), 1, 0, 1, null, false, null, false);
			}
			else
			{
				this.ShowDiscardItemConfirmDialog(itemData, 1);
			}
		}

		// Token: 0x06009539 RID: 38201 RVA: 0x00459D7A File Offset: 0x00457F7A
		private bool CanShowLock(ItemDisplayData itemData)
		{
			return !itemData.IsResource && itemData.IsTransferable;
		}

		// Token: 0x0600953A RID: 38202 RVA: 0x00459D90 File Offset: 0x00457F90
		private bool CanClickLock(ItemDisplayData itemData)
		{
			bool flag = !this.CanShowLock(itemData);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool inTutorial = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
				bool flag2 = inTutorial;
				result = !flag2;
			}
			return result;
		}

		// Token: 0x0600953B RID: 38203 RVA: 0x00459DCC File Offset: 0x00457FCC
		private void OnClickLock(ItemDisplayData itemData)
		{
			Inventory list = itemData.GetAllInventoryFromPool();
			TaiwuDomainMethod.Call.SetItemListLocked(list, !itemData.IsLocked);
			ItemDisplayData.ReturnInventoryToPool(list);
			this.CallRefreshItems(null);
		}

		// Token: 0x0600953C RID: 38204 RVA: 0x00459E00 File Offset: 0x00458000
		private void ShowDiscardItemConfirmDialog(ItemDisplayData itemData, int count)
		{
			ViewCharacterMenuItems.<>c__DisplayClass138_0 CS$<>8__locals1 = new ViewCharacterMenuItems.<>c__DisplayClass138_0();
			CS$<>8__locals1.itemData = itemData;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.count = count;
			this.OnExitFocusMode();
			bool operationNeedConfirm = CS$<>8__locals1.itemData.GetOperationNeedConfirm(ItemOperationType.EItemOperationType.Discard);
			bool usingTypeNeedConfirm = CS$<>8__locals1.itemData.GetUsingTypeNeedConfirm();
			bool flag = operationNeedConfirm && usingTypeNeedConfirm;
			if (flag)
			{
				DialogCmd cmd = new DialogCmd
				{
					Title = LanguageKey.LK_Common_Attention.Tr(),
					Content = CS$<>8__locals1.itemData.GetUsingOperationConfirmTip(ItemDisplayData.ItemUsingOperationType.Default),
					Type = 1,
					Yes = new Action(CS$<>8__locals1.<ShowDiscardItemConfirmDialog>g__Action|0)
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			else
			{
				CS$<>8__locals1.<ShowDiscardItemConfirmDialog>g__Action|0();
			}
		}

		// Token: 0x0600953D RID: 38205 RVA: 0x00459ECC File Offset: 0x004580CC
		private void OnClickIdentifyItem(RowItemLine rowItemLine)
		{
			UIElement.FullScreenMask.Show();
			bool flag = this.multiplyItemListScroll != null;
			if (flag)
			{
				this.multiplyItemListScroll.HighLightItemView(rowItemLine);
			}
			ITradeableContent itemDisplayData = rowItemLine.Data;
			ItemDomainMethod.AsyncCall.IdentifyPoisons(this, base.CharacterMenu.CurCharacterId, itemDisplayData as ItemDisplayData, delegate(int offset, RawDataPool dataPool)
			{
				List<ItemDisplayData> resultList = null;
				Serializer.Deserialize(dataPool, offset, ref resultList);
				this._curIdentifySuccess = (resultList != null && resultList.Count > 0);
				ViewCharacterMenuItems <>4__this = this;
				bool curIdentifiedResultHasPoison;
				if (this._curIdentifySuccess)
				{
					curIdentifiedResultHasPoison = resultList.Any((ItemDisplayData d) => d.HasAnyPoison);
				}
				else
				{
					curIdentifiedResultHasPoison = false;
				}
				<>4__this._curIdentifiedResultHasPoison = curIdentifiedResultHasPoison;
				Action <>9__3;
				this.ShowIdentifiedPoisonTip(true, rowItemLine, delegate
				{
					YieldHelper instance = SingletonObject.getInstance<YieldHelper>();
					float sec = 1f;
					Action job;
					if ((job = <>9__3) == null)
					{
						job = (<>9__3 = delegate()
						{
							bool curIdentifiedResultHasPoison2 = this._curIdentifiedResultHasPoison;
							if (curIdentifiedResultHasPoison2)
							{
								ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
								argBox.SetObject("ItemList", resultList);
								argBox.Set("ObtainType", 10);
								UIElement.GetItem.SetOnInitArgs(argBox);
								UIManager.Instance.MaskUI(UIElement.GetItem);
							}
							this.ShowIdentifiedPoisonTip(false, null, null);
							this.CallRefreshItems(null);
							this.OnExitFocusMode();
							UIElement.FullScreenMask.Hide(false);
						});
					}
					instance.DelaySecondsDo(sec, job);
				});
			});
		}

		// Token: 0x0600953E RID: 38206 RVA: 0x00459F4C File Offset: 0x0045814C
		private void ShowIdentifiedPoisonTip(bool show, RowItemLine rowItemLine, Action action)
		{
			this._curIdentifiedResultAction = action;
			ItemIdentifyPoison tip = this.identifiedPoisonTip;
			bool flag = tip == null;
			if (!flag)
			{
				bool flag2 = !show;
				if (flag2)
				{
					tip.Hide();
					bool flag3 = this.multiplyItemListScroll != null;
					if (flag3)
					{
						this.multiplyItemListScroll.CancelHighLightItemView();
					}
				}
				else
				{
					tip.Play(this._curIdentifySuccess, this._curIdentifiedResultHasPoison, new Action(this.AnimationStateOnEnd));
					this.AdjustIdentifiedPoisonTipPosition(rowItemLine.RectTransform);
				}
			}
		}

		// Token: 0x0600953F RID: 38207 RVA: 0x00459FCF File Offset: 0x004581CF
		private void AnimationStateOnEnd()
		{
			Action curIdentifiedResultAction = this._curIdentifiedResultAction;
			if (curIdentifiedResultAction != null)
			{
				curIdentifiedResultAction();
			}
		}

		// Token: 0x06009540 RID: 38208 RVA: 0x00459FE4 File Offset: 0x004581E4
		private void AdjustIdentifiedPoisonTipPosition(RectTransform rowRectTransform)
		{
			RectTransform rangeRect = this.multiplyItemListScroll.CurMultiplyScrollView.ViewportRectTransform;
			bool flag = rangeRect == null;
			if (!flag)
			{
				this.identifiedPoisonTip.AdjustPosition(rangeRect, rowRectTransform);
			}
		}

		// Token: 0x06009541 RID: 38209 RVA: 0x0045A020 File Offset: 0x00458220
		private void OnClickEatItem(ItemDisplayData itemData)
		{
			ViewCharacterMenuItems.<>c__DisplayClass143_0 CS$<>8__locals1 = new ViewCharacterMenuItems.<>c__DisplayClass143_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.itemData = itemData;
			ItemKey itemKey = CS$<>8__locals1.itemData.RealKey;
			bool flag = CommonUtils.IsRecoverInjuryMainMedicineItem(CS$<>8__locals1.itemData.RealKey);
			if (flag)
			{
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>().Set<ItemKey>("SelectedItemKey", itemKey).Set("CurrentCharacterId", base.CharacterMenu.CurCharacterId);
				UIElement.UsingMedicineItem.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.UsingMedicineItem, true);
				CS$<>8__locals1.<OnClickEatItem>g__Cancel|1();
			}
			else
			{
				this.multiplyItemListScroll.HideMultiplySelectButton();
				this.multiplyItemListScroll.HideToggleMultiplyLock();
				ValueTuple<int, string> valueTuple = Injury.CheckEatSlot(base.CharacterMenu.Injury.Data, CS$<>8__locals1.itemData.Key, CS$<>8__locals1.itemData.Amount);
				int limitCount = valueTuple.Item1;
				string limitTip = valueTuple.Item2;
				limitTip = this.GetLongEatLimitTip(CS$<>8__locals1.itemData.Key, limitTip);
				bool flag2 = CS$<>8__locals1.itemData.Amount > 1 && ItemTemplateHelper.CanUseMultiple(CS$<>8__locals1.itemData.Key);
				if (flag2)
				{
					int minCount = 1;
					bool flag3 = ItemTemplateHelper.IsTianJieFuLu(CS$<>8__locals1.itemData.Key.ItemType, CS$<>8__locals1.itemData.Key.TemplateId);
					if (flag3)
					{
						minCount = ItemTemplateHelper.GetTianJieFuLuCountUnit();
					}
					RowItemLine rowItemLine = this.itemListScroll.FindActiveItem(CS$<>8__locals1.itemData, false);
					this.itemListScroll.SetItemToSelectCountMode(rowItemLine, delegate(int count)
					{
						bool flag4 = count <= 0;
						if (flag4)
						{
							base.<OnClickEatItem>g__Cancel|1();
						}
						else
						{
							base.<OnClickEatItem>g__Confirm|0(count);
						}
					}, new Action(CS$<>8__locals1.<OnClickEatItem>g__Cancel|1), minCount, limitCount, minCount, limitTip, false, delegate(int count)
					{
						CS$<>8__locals1.<>4__this.CharacterMenu.SetEatItemInfectNotice(true, CS$<>8__locals1.itemData, count);
					}, false);
				}
				else
				{
					CS$<>8__locals1.<OnClickEatItem>g__Confirm|0(1);
				}
			}
		}

		// Token: 0x06009542 RID: 38210 RVA: 0x0045A1D8 File Offset: 0x004583D8
		private void OnClickTransferItem(ItemDisplayData itemData)
		{
			bool flag = itemData.UsingType != ItemDisplayData.ItemUsingType.Invalid;
			if (flag)
			{
				DialogCmd cmd = new DialogCmd
				{
					Title = LocalStringManager.Get(LanguageKey.LK_Common_Attention),
					Content = itemData.GetUsingOperationConfirmTip(ItemDisplayData.ItemUsingOperationType.Present),
					Type = 1,
					Yes = delegate()
					{
						this.EnterTransferMode(itemData);
					},
					No = delegate()
					{
						this.CharacterMenu.ExitFocusMode(true);
					}
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			else
			{
				this.EnterTransferMode(itemData);
			}
		}

		// Token: 0x06009543 RID: 38211 RVA: 0x0045A2A4 File Offset: 0x004584A4
		private void EnterTransferMode(ItemDisplayData itemData)
		{
			this.multiplyItemListScroll.HideMultiplySelectButton();
			this.multiplyItemListScroll.HideToggleMultiplyLock();
			this.AddDebtMouseTipInfo();
			ViewCharacterMenuItems._currItemOperation = ItemOperationType.EItemOperationType.Transfer;
			RowItemLine rowItemLine = this.itemListScroll.FindActiveItem(itemData, false);
			bool flag = itemData.Amount > 1;
			if (flag)
			{
				this.itemListScroll.SetItemToSelectCountMode(rowItemLine, delegate(int count)
				{
					bool flag2 = count <= 0;
					if (flag2)
					{
						this.OnExitFocusMode();
					}
					else
					{
						this._selectedToTransferItemData = itemData.Clone(count);
						((ICellContent<ITradeableContent>)rowItemLine.RowItemMain).SetSelected(true);
						ArgumentBox argBox2 = EasyPool.Get<ArgumentBox>();
						argBox2.SetObject("ItemDisplayData", this._selectedToTransferItemData);
						argBox2.SetObject("MaskToFocusGroupedItemList", this.maskToFocusGroupedItemList.transform);
						GEvent.OnEvent(UiEvents.EnterTransferItem, argBox2);
						this.SetItemScrollViewCanScroll(false);
						this.EnterTransferFocusItem(rowItemLine);
					}
				}, new Action(this.OnExitFocusMode), 1, 0, 1, null, true, null, false);
			}
			else
			{
				this._selectedToTransferItemData = itemData.Clone(-1);
				this.SetItemScrollViewCanScroll(false);
				this.itemListScroll.ReRender();
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.SetObject("ItemDisplayData", this._selectedToTransferItemData);
				argBox.SetObject("MaskToFocusGroupedItemList", this.maskToFocusGroupedItemList.transform);
				GEvent.OnEvent(UiEvents.EnterTransferItem, argBox);
				this.EnterTransferFocusItem(rowItemLine);
			}
		}

		// Token: 0x06009544 RID: 38212 RVA: 0x0045A3C0 File Offset: 0x004585C0
		private void EnterTransferFocusItem(RowItemLine rawItemLine)
		{
			bool flag = rawItemLine == null;
			if (!flag)
			{
				this._transferFocusItem = rawItemLine.transform;
				this._transferFocusItemParent = this._transferFocusItem.parent;
				this._transferFocusItemSiblingIndex = this._transferFocusItem.GetSiblingIndex();
				this._transferFocusItem.SetParent(this.maskToFocusGroupedItemList.transform, true);
				this.ShowTransferDetail(rawItemLine);
				UIManager.Instance.MaskComponent((RectTransform)this.maskToFocusGroupedItemList.transform);
			}
		}

		// Token: 0x06009545 RID: 38213 RVA: 0x0045A444 File Offset: 0x00458644
		private void ExitTransferFocusItem()
		{
			bool flag = this._transferFocusItem == null;
			if (!flag)
			{
				this._transferFocusItem.SetParent(this._transferFocusItemParent, true);
				this._transferFocusItem.SetSiblingIndex(this._transferFocusItemSiblingIndex);
				this.HideTransferDetail();
				UIManager.Instance.UnMaskComponent((RectTransform)this.maskToFocusGroupedItemList.transform);
				this._transferFocusItem = null;
				this._transferFocusItemParent = null;
				this._transferFocusItemSiblingIndex = -1;
			}
		}

		// Token: 0x06009546 RID: 38214 RVA: 0x0045A4C0 File Offset: 0x004586C0
		private void ShowTransferDetail(RowItemLine rawItemLine)
		{
			RowItemMain rowItemMain = this.transferDetailCardItem.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(rawItemLine.Data);
			this.transferDetailCardItem.Set(rowItemMain, true);
			this.transferDetailCardItem.gameObject.SetActive(false);
			this.transferDetail.transform.SetParent(rawItemLine.transform, true);
			this.transferDetail.GetComponent<CanvasGroup>().alpha = 0f;
			this.transferDetail.SetActive(true);
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
			{
				RectTransform detailRect = this.transferDetail.GetComponent<RectTransform>();
				detailRect.anchoredPosition = new Vector2(0f, -rawItemLine.RectTransform.rect.height);
				Rect itemRect = CommonUtils.RectTransToScreenPos(detailRect, UIManager.Instance.UiCamera);
				Rect scrollRect = CommonUtils.RectTransToScreenPos(this.RectTransform, UIManager.Instance.UiCamera);
				bool isOverlap = !scrollRect.ContainsWithBorder(itemRect.min);
				bool flag = isOverlap;
				if (flag)
				{
					detailRect.anchoredPosition = new Vector2(detailRect.anchoredPosition.x, detailRect.rect.height);
				}
				this.transferDetail.GetComponent<CanvasGroup>().alpha = 1f;
			});
		}

		// Token: 0x06009547 RID: 38215 RVA: 0x0045A578 File Offset: 0x00458778
		private void HideTransferDetail()
		{
			this.transferDetail.transform.SetParent(this.maskToFocusGroupedItemList.transform.parent, true);
			this.transferDetail.SetActive(false);
		}

		// Token: 0x06009548 RID: 38216 RVA: 0x0045A5AA File Offset: 0x004587AA
		private void AddDebtMouseTipInfo()
		{
			base.CharacterMenu.RerenderCharacterScroll();
		}

		// Token: 0x06009549 RID: 38217 RVA: 0x0045A5BC File Offset: 0x004587BC
		private void OnSelectTransferItemChar(ArgumentBox argsBox)
		{
			this.SetItemScrollViewCanScroll(true);
			int characterId;
			argsBox.Get("CharacterId", out characterId);
			bool isMultiItemSelect = this.multiplyItemListScroll.IsMultiItemSelect;
			if (isMultiItemSelect)
			{
				this.multiplyItemListScroll.CurCharId = characterId;
			}
			else
			{
				bool flag = !this.SelectingTransferItemChar;
				if (!flag)
				{
					ItemDisplayData itemData;
					argsBox.Get<ItemDisplayData>("ItemDisplayData", out itemData);
					this.TransferItem(itemData, characterId);
				}
			}
		}

		// Token: 0x0600954A RID: 38218 RVA: 0x0045A624 File Offset: 0x00458824
		private void OnClickTakeItemFrom(ItemDisplayData itemData, int limitSelectCount)
		{
			bool flag = itemData.Amount > 1;
			if (flag)
			{
				ViewCharacterMenuItems._currItemOperation = ItemOperationType.EItemOperationType.Take;
				RowItemLine rowItemLine = this.itemListScroll.FindActiveItem(itemData, false);
				this.itemListScroll.SetItemToSelectCountMode(rowItemLine, delegate(int count)
				{
					this.OnExitFocusMode();
					bool flag2 = count > 0;
					if (flag2)
					{
						ItemDisplayData newItemDisplayData = itemData.Clone(count);
						this.TransferItem(newItemDisplayData, this._taiwuCharId);
					}
				}, new Action(this.OnExitFocusMode), 1, limitSelectCount, 1, null, false, null, true);
			}
			else
			{
				this.TransferItem(itemData, this._taiwuCharId);
				this.OnExitFocusMode();
			}
		}

		// Token: 0x0600954B RID: 38219 RVA: 0x0045A6C0 File Offset: 0x004588C0
		private unsafe void TransferItem(ItemDisplayData itemData, int characterId)
		{
			ItemDisplayData realData = this.InventoryItems->Find((ItemDisplayData d) => d.ContainsItemKey(itemData.Key));
			bool flag = realData == null;
			if (!flag)
			{
				int changeAmount = itemData.Amount;
				bool isMiscResource = ItemTemplateHelper.IsMiscResource(itemData.Key.ItemType, itemData.Key.TemplateId);
				bool needRefreshCharacterDisplay = !isMiscResource && itemData.UsingType == ItemDisplayData.ItemUsingType.Equiped && ItemTemplateHelper.GetEquipmentType(itemData.Key.ItemType, itemData.Key.TemplateId) == 2;
				bool flag2 = isMiscResource;
				if (flag2)
				{
					sbyte resourceType = ItemTemplateHelper.GetMiscResourceType(itemData.Key.ItemType, itemData.Key.TemplateId);
					ResourceInts resourceInts = default(ResourceInts);
					resourceInts.Add(resourceType, changeAmount);
					CharacterDomainMethod.Call.TransferResourcesWithDebt(base.CharacterMenu.CurCharacterId, characterId, resourceInts, true);
					base.CharacterMenu.RerenderCharacterScroll();
					this.CallRefreshItems(null);
				}
				else
				{
					Inventory inventory = realData.GetOperationInventoryFromPool(changeAmount, false);
					CharacterDomainMethod.Call.TransferInventoryItemInventoryWithDebt(base.CharacterMenu.CurCharacterId, characterId, inventory, true);
					ItemDisplayData.ReturnInventoryToPool(inventory);
					bool flag3 = needRefreshCharacterDisplay;
					if (flag3)
					{
						CharacterDomainMethod.Call.GetCharacterDisplayData(base.CharacterMenu.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
						bool flag4 = characterId != base.CharacterMenu.CurCharacterId;
						if (flag4)
						{
							CharacterDomainMethod.Call.GetCharacterDisplayData(base.CharacterMenu.Element.GameDataListenerId, characterId);
						}
					}
					base.CharacterMenu.RerenderCharacterScroll();
					this.CallRefreshItems(null);
				}
			}
		}

		// Token: 0x0600954C RID: 38220 RVA: 0x0045A87A File Offset: 0x00458A7A
		private void OnClickUseCommonEvent(ItemDisplayData itemData)
		{
			base.CharacterMenu.ExitFocusMode(true);
			base.CharacterMenu.QuickHide();
			TaiwuEventDomainMethod.Call.OperateInventoryItem(this._taiwuCharId, 9, itemData);
		}

		// Token: 0x0600954D RID: 38221 RVA: 0x0045A8A5 File Offset: 0x00458AA5
		private void OnClickHomingPegon(RowItemLine rowItemLine)
		{
			base.CharacterMenu.ExitFocusMode(true);
			GEvent.OnEvent(UiEvents.InviteSelectBlockStart, null);
			base.CharacterMenu.QuickHide();
		}

		// Token: 0x0600954E RID: 38222 RVA: 0x0045A8D4 File Offset: 0x00458AD4
		private void OnClickSectMainStoryItemXuannvNotes()
		{
			base.CharacterMenu.ExitFocusMode(true);
			bool isEnabled = SingletonObject.getInstance<MusicPlayerModel>().IsEnabled;
			bool flag = isEnabled;
			if (flag)
			{
				SingletonObject.getInstance<MusicPlayerModel>().DisableMusicPlayer();
			}
			else
			{
				SingletonObject.getInstance<MusicPlayerModel>().EnableMusicPlayer();
				base.CharacterMenu.QuickHide();
			}
		}

		// Token: 0x0600954F RID: 38223 RVA: 0x0045A925 File Offset: 0x00458B25
		private void OnClickSectMainStoryItemWuxianWugFairy()
		{
			this.OnExitFocusMode();
			UIManager.Instance.HideUI(UIElement.CharacterMenu);
			TaiwuEventDomainMethod.Call.CloseUI("WuxianWugFairy", false, -1);
		}

		// Token: 0x06009550 RID: 38224 RVA: 0x0045A94C File Offset: 0x00458B4C
		private void OnClickSectMainStoryItemYuanshanRosary()
		{
			this.OnExitFocusMode();
			UIManager.Instance.HideUI(UIElement.CharacterMenu);
			UIManager.Instance.ShowUI(UIElement.ThreeVitals, true);
		}

		// Token: 0x06009551 RID: 38225 RVA: 0x0045A977 File Offset: 0x00458B77
		private void OnClickDamageHugeSword()
		{
			this.OnExitFocusMode();
			UIManager.Instance.HideUI(UIElement.CharacterMenu);
			WorldDomainMethod.Call.OnClickDamageHugeSword();
		}

		// Token: 0x06009552 RID: 38226 RVA: 0x0045A997 File Offset: 0x00458B97
		private void OnClickSectMainStoryItemJieQingStars()
		{
			this.OnExitFocusMode();
			UIManager.Instance.HideUI(UIElement.CharacterMenu);
			UIManager.Instance.ShowUI(UIElement.JieQingInteract, true);
		}

		// Token: 0x06009553 RID: 38227 RVA: 0x0045A9C4 File Offset: 0x00458BC4
		private void OnClickThanksLetter(ItemDisplayData itemData)
		{
			bool flag = itemData.Amount > 1;
			if (flag)
			{
				RowItemLine rowItemLine = this.itemListScroll.FindActiveItem(itemData, false);
				this.itemListScroll.SetItemToSelectCountMode(rowItemLine, delegate(int count)
				{
					bool flag2 = count > 0;
					if (flag2)
					{
						this.UseThanksLetter(itemData, count);
					}
				}, new Action(this.OnExitFocusMode), 1, 0, 1, null, false, null, false);
			}
			else
			{
				this.UseThanksLetter(itemData, 1);
			}
		}

		// Token: 0x06009554 RID: 38228 RVA: 0x0045AA4C File Offset: 0x00458C4C
		private void UseThanksLetter(ItemDisplayData itemData, int amount)
		{
			this.OnExitFocusMode();
			ExtraDomainMethod.Call.UseFeastThanksLetter(itemData.RealKey, amount);
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Clear();
			argBox.SetObject("ItemList", new List<ItemDisplayData>
			{
				new ItemDisplayData
				{
					Key = new ItemKey(12, 0, 8, 0),
					Amount = amount * Misc.Instance[itemData.RealKey.TemplateId].GainExp,
					Value = 0L
				}
			});
			UIElement.GetItem.SetOnInitArgs(argBox);
			UIManager.Instance.MaskUI(UIElement.GetItem);
			this.CallRefreshItems(null);
		}

		// Token: 0x06009555 RID: 38229 RVA: 0x0045AAF7 File Offset: 0x00458CF7
		private void OnClickSectMainStoryItemFulongChickenMap()
		{
			this.OnExitFocusMode();
			UIManager.Instance.HideUI(UIElement.CharacterMenu);
			ViewChickenMap.Open(this);
		}

		// Token: 0x06009556 RID: 38230 RVA: 0x0045AB18 File Offset: 0x00458D18
		private unsafe void ChickenMapHandle()
		{
			bool flag = *this.InventoryItems == null;
			if (!flag)
			{
				foreach (ItemDisplayData itemDisplayData in *this.InventoryItems)
				{
					bool flag2 = itemDisplayData.Key.ItemType == 12 && itemDisplayData.Key.TemplateId == 370;
					if (flag2)
					{
						BuildingDomainMethod.AsyncCall.AllChickenInTaiwuVillage(this, delegate(int offset, RawDataPool dataPool)
						{
							Serializer.Deserialize(dataPool, offset, ref ViewChickenMap.AllChickenInTaiwuVillage);
						});
					}
				}
			}
		}

		// Token: 0x06009557 RID: 38231 RVA: 0x0045ABD0 File Offset: 0x00458DD0
		private void OnClickResourceChoosy()
		{
			this.OnExitFocusMode();
			UIManager.Instance.MaskUI(UIElement.ChoosyResource);
		}

		// Token: 0x06009558 RID: 38232 RVA: 0x0045ABEC File Offset: 0x00458DEC
		private void OnClickResourceExchange()
		{
			int charId = base.CharacterMenu.CurCharacterId;
			this.OnExitFocusMode();
			this.ExchangeResource(charId);
		}

		// Token: 0x06009559 RID: 38233 RVA: 0x0045AC18 File Offset: 0x00458E18
		private void OnClickResourceScam(ItemDisplayData itemData)
		{
			sbyte operationType = InventoryItemOperationType.Scam.ToSbyte();
			this.OperateResourceItem(operationType, itemData);
		}

		// Token: 0x0600955A RID: 38234 RVA: 0x0045AC3C File Offset: 0x00458E3C
		private void OnClickResourceSteal(ItemDisplayData itemData)
		{
			sbyte operationType = InventoryItemOperationType.Steal.ToSbyte();
			this.OperateResourceItem(operationType, itemData);
		}

		// Token: 0x0600955B RID: 38235 RVA: 0x0045AC60 File Offset: 0x00458E60
		private void OnClickResourceRob(ItemDisplayData itemData)
		{
			sbyte operationType = InventoryItemOperationType.Rob.ToSbyte();
			this.OperateResourceItem(operationType, itemData);
		}

		// Token: 0x0600955C RID: 38236 RVA: 0x0045AC84 File Offset: 0x00458E84
		private void OperateResourceItem(sbyte operationType, ItemDisplayData itemData)
		{
			int limitSelectCount = itemData.Amount / GlobalConfig.Instance.HostileOperationTakeItemMaxResourceFactor;
			limitSelectCount = Math.Max(1, limitSelectCount);
			ViewCharacterMenuItems._currItemOperation = ItemOperationType.EItemOperationType.Take;
			RowItemLine rowItemLine = this.itemListScroll.FindActiveItem(itemData, false);
			this.itemListScroll.SetItemToSelectCountMode(rowItemLine, delegate(int count)
			{
				int charId = this.CharacterMenu.CurCharacterId;
				this.OnExitFocusMode();
				UIManager.Instance.HideUI(UIElement.CharacterMenu);
				ItemDisplayData newItemData = itemData.Clone(count);
				TaiwuEventDomainMethod.Call.OperateInventoryItem(charId, operationType, newItemData);
			}, new Action(this.OnExitFocusMode), 1, limitSelectCount, 1, null, false, null, false);
		}

		// Token: 0x0600955D RID: 38237 RVA: 0x0045AD14 File Offset: 0x00458F14
		private void ItemMultiplyOperationTypeChange(ArgumentBox argsBox)
		{
			Enum type;
			bool flag = argsBox.Get("ItemOperationType", out type);
			if (flag)
			{
				ItemOperationType.EItemOperationType itemOperationType = (ItemOperationType.EItemOperationType)type;
				ItemOperationType.EItemOperationType currItemOperation = this.multiplyItemListScroll.CurrItemOperation;
				bool flag2 = currItemOperation != ItemOperationType.EItemOperationType.Transfer && itemOperationType == ItemOperationType.EItemOperationType.Transfer;
				if (flag2)
				{
					base.CharacterMenu.FocusCharList();
				}
				bool flag3 = currItemOperation == ItemOperationType.EItemOperationType.Transfer && itemOperationType != ItemOperationType.EItemOperationType.Transfer;
				if (flag3)
				{
					base.CharacterMenu.StopFocusCharList();
				}
				ViewCharacterMenuItems._currItemOperation = itemOperationType;
			}
			this.multiplyItemListScroll.OnItemMultiplyOperationTypeChange(argsBox);
		}

		// Token: 0x0600955E RID: 38238 RVA: 0x0045AD98 File Offset: 0x00458F98
		private void ItemMultiplyOperationTargetChange(ArgumentBox argumentBox)
		{
			ItemDisplayData target;
			argumentBox.Get<ItemDisplayData>("FeedingTarget", out target);
			this.multiplyItemListScroll.SetFeedingTarget(target);
		}

		// Token: 0x0600955F RID: 38239 RVA: 0x0045ADC1 File Offset: 0x00458FC1
		private void ItemMultiplyOperationFinish(ArgumentBox argumentBox)
		{
			this.CallRefreshItems(null);
		}

		// Token: 0x06009560 RID: 38240 RVA: 0x0045ADCC File Offset: 0x00458FCC
		private bool CanClickSwordFragment(ITradeableContent itemData)
		{
			bool flag;
			if (itemData.RealKey.ItemType == 12)
			{
				short templateId = itemData.RealKey.TemplateId;
				flag = (templateId >= 229 && templateId <= 237);
			}
			else
			{
				flag = false;
			}
			bool isSword = flag;
			bool isUnlocked = this._characterItemsDisplayData.DivineFlameData.IsUnlocked;
			return isSword && isUnlocked;
		}

		// Token: 0x06009561 RID: 38241 RVA: 0x0045AE28 File Offset: 0x00459028
		private void OnClickDivineFlame(sbyte xiangshuAvatarId)
		{
			ViewCharacterMenuItems.<>c__DisplayClass175_0 CS$<>8__locals1 = new ViewCharacterMenuItems.<>c__DisplayClass175_0();
			CS$<>8__locals1.xiangshuAvatarId = xiangshuAvatarId;
			CS$<>8__locals1.<>4__this = this;
			bool flag = this._characterItemsDisplayData.DivineFlameTargetCharacter != null && CS$<>8__locals1.xiangshuAvatarId == 5;
			if (flag)
			{
				string title = LanguageKey.LK_Story_Sword_SelectTarget_ChangeChar_Title.Tr();
				string charName = NameCenter.GetMonasticTitleOrDisplayName(this._characterItemsDisplayData.DivineFlameTargetCharacter, false);
				string content = LanguageKey.LK_Story_Sword_SelectTarget_ChangeChar_Content.TrFormat(charName);
				CommonUtils.ShowConfirmDialog(title, content, new Action(CS$<>8__locals1.<OnClickDivineFlame>g__Action|0), null, EDialogType.None);
			}
			else
			{
				CS$<>8__locals1.<OnClickDivineFlame>g__Action|0();
			}
		}

		// Token: 0x06009562 RID: 38242 RVA: 0x0045AEB4 File Offset: 0x004590B4
		private void UseDivineFlameByNone(sbyte xiangshuAvatarId)
		{
			StoryDomainMethod.Call.UseDivineFlame(xiangshuAvatarId, -1, Location.Invalid);
			base.CharacterMenu.QuickHide();
		}

		// Token: 0x06009563 RID: 38243 RVA: 0x0045AED0 File Offset: 0x004590D0
		private void UseDivineFlameBySelectCharacter(sbyte xiangshuAvatarId)
		{
			AsyncMethodCallbackDelegate <>9__2;
			StoryDomainMethod.AsyncCall.GetDivineFlameSelectTargetCharIdList(null, xiangshuAvatarId, delegate(int offset, RawDataPool pool)
			{
				List<int> charIdList = null;
				Serializer.Deserialize(pool, offset, ref charIdList);
				bool flag = charIdList == null || charIdList.Count <= 0;
				if (!flag)
				{
					IAsyncMethodRequestHandler requestHandler = null;
					List<int> charIdList2 = charIdList;
					AsyncMethodCallbackDelegate callback;
					if ((callback = <>9__2) == null)
					{
						callback = (<>9__2 = delegate(int offset, RawDataPool pool)
						{
							List<CharacterDisplayDataForGeneralScrollList> charDataList = null;
							Serializer.Deserialize(pool, offset, ref charDataList);
							bool flag2 = charDataList == null || charDataList.Count <= 0;
							if (!flag2)
							{
								CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.None);
								bool isGood = SingletonObject.getInstance<BasicGameData>().IsXiangshuAvatarTaskStatusGood((int)xiangshuAvatarId);
								config.CustomTextGenerator = ((IReadOnlyList<int> _) => ViewCharacterMenuItems.GetDivineFlameSelectTargetTip(xiangshuAvatarId, isGood, false));
								config.InteractionMode = ESelectCharacterInteractionMode.Slot;
								config.SelectionMode = ESelectCharacterSelectionMode.Single;
								config.TargetCount = 1;
								ViewSelectCharacter.Show(config, charDataList, new SelectCharacterCallback(base.<UseDivineFlameBySelectCharacter>g__SelectCharacterCallback|1), null, false);
							}
						});
					}
					CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForGeneralScrollListBatch(requestHandler, charIdList2, callback);
				}
			});
		}

		// Token: 0x06009564 RID: 38244 RVA: 0x0045AF10 File Offset: 0x00459110
		private void UseDivineFlameBySelectBlock(sbyte xiangshuAvatarId)
		{
			StoryDomainMethod.AsyncCall.GetDivineFlameSelectTargetLocationList(this, xiangshuAvatarId, Location.Invalid, delegate(int offset, RawDataPool pool)
			{
				List<Location> list = null;
				Serializer.Deserialize(pool, offset, ref list);
				this.CharacterMenu.ExitFocusMode(true);
				ArgumentBox args = EasyPool.Get<ArgumentBox>().Set("xiangshuAvatarId", xiangshuAvatarId).SetObject("locationList", list);
				GEvent.OnEvent(UiEvents.DivineFlameSelectBlockStart, args);
				this.CharacterMenu.QuickHide();
			});
		}

		// Token: 0x06009565 RID: 38245 RVA: 0x0045AF50 File Offset: 0x00459150
		private void UseDivineFlameByCheckCharacter(sbyte xiangshuAvatarId)
		{
			bool isGood = SingletonObject.getInstance<BasicGameData>().IsXiangshuAvatarTaskStatusGood((int)xiangshuAvatarId);
			string title = ViewCharacterMenuItems.GetDivineFlameTitle(xiangshuAvatarId, isGood);
			string content = ViewCharacterMenuItems.GetDivineFlameSelectTargetTip(xiangshuAvatarId, isGood, true);
			CommonUtils.ShowConfirmDialog(title, content, delegate
			{
				StoryDomainMethod.Call.UseDivineFlame(xiangshuAvatarId, -1, Location.Invalid);
				this.CharacterMenu.QuickHide();
			}, null, EDialogType.None);
		}

		// Token: 0x06009566 RID: 38246 RVA: 0x0045AFB4 File Offset: 0x004591B4
		private void UseDivineFlameByCheckBlock(sbyte xiangshuAvatarId)
		{
			bool isGood = SingletonObject.getInstance<BasicGameData>().IsXiangshuAvatarTaskStatusGood((int)xiangshuAvatarId);
			string title = ViewCharacterMenuItems.GetDivineFlameTitle(xiangshuAvatarId, isGood);
			string content = ViewCharacterMenuItems.GetDivineFlameSelectTargetTip(xiangshuAvatarId, isGood, true);
			CommonUtils.ShowConfirmDialog(title, content, delegate
			{
				StoryDomainMethod.Call.UseDivineFlame(xiangshuAvatarId, -1, Location.Invalid);
				this.CharacterMenu.QuickHide();
			}, null, EDialogType.None);
		}

		// Token: 0x06009567 RID: 38247 RVA: 0x0045B018 File Offset: 0x00459218
		public static string GetDivineFlameSelectTargetTip(sbyte xiangshuAvatarId, bool isGood, bool isConfirm = false)
		{
			if (!true)
			{
			}
			LanguageKey languageKey;
			switch (xiangshuAvatarId)
			{
			case 0:
				languageKey = (isGood ? LanguageKey.LK_Story_Sword_SelectTarget_Monv_Good : LanguageKey.LK_Story_Sword_SelectTarget_Monv_Bad);
				break;
			case 1:
				languageKey = (isGood ? LanguageKey.LK_Story_Sword_SelectTarget_DayueYaochang_Good : LanguageKey.LK_Story_Sword_SelectTarget_DayueYaochang_Bad);
				break;
			case 2:
				languageKey = (isGood ? LanguageKey.LK_Story_Sword_SelectTarget_Jiuhan_Good : LanguageKey.LK_Story_Sword_SelectTarget_Jiuhan_Bad);
				break;
			case 3:
				languageKey = (isGood ? LanguageKey.LK_Story_Sword_SelectTarget_JinHuanger_Good : LanguageKey.LK_Story_Sword_SelectTarget_JinHuanger_Bad);
				break;
			case 4:
				languageKey = (isGood ? LanguageKey.LK_Story_Sword_SelectTarget_YiYihou_Good : LanguageKey.LK_Story_Sword_SelectTarget_YiYihou_Bad);
				break;
			case 5:
				languageKey = (isGood ? LanguageKey.LK_Story_Sword_SelectTarget_WeiQi_Good : LanguageKey.LK_Story_Sword_SelectTarget_WeiQi_Bad);
				break;
			case 6:
				languageKey = (isGood ? LanguageKey.LK_Story_Sword_SelectTarget_Yixiang_Good : LanguageKey.LK_Story_Sword_SelectTarget_Yixiang_Bad);
				break;
			case 7:
				languageKey = (isGood ? LanguageKey.LK_Story_Sword_SelectTarget_Xuefeng_Good : LanguageKey.LK_Story_Sword_SelectTarget_Xuefeng_Bad);
				break;
			case 8:
				languageKey = (isGood ? LanguageKey.LK_Story_Sword_SelectTarget_ShuFang_Good : LanguageKey.LK_Story_Sword_SelectTarget_ShuFang_Bad);
				break;
			default:
				languageKey = LanguageKey.Invalid;
				break;
			}
			if (!true)
			{
			}
			LanguageKey key = languageKey;
			bool flag = !isConfirm;
			string result;
			if (flag)
			{
				result = key.Tr();
			}
			else
			{
				StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
				stringBuilder.AppendLine(key.Tr());
				stringBuilder.AppendLine(LanguageKey.LK_Story_Sword_Use_Confirm_Tip.Tr());
				string content = stringBuilder.ToString();
				EasyPool.Free<StringBuilder>(stringBuilder);
				result = content;
			}
			return result;
		}

		// Token: 0x06009568 RID: 38248 RVA: 0x0045B158 File Offset: 0x00459358
		public static string GetDivineFlameTitle(sbyte xiangshuAvatarId, bool isGood)
		{
			short charTemplateId = XiangshuAvatarIds.XiangshuBossBeginIds[(int)xiangshuAvatarId];
			CharacterItem charConfig = Character.Instance[charTemplateId];
			string charName = NameCenter.FormatName(charConfig.Surname, charConfig.GivenName);
			LanguageKey goodDesc = isGood ? LanguageKey.LK_Story_Sword_Use_Good : LanguageKey.LK_Story_Sword_Use_Bad;
			return charName + goodDesc.Tr();
		}

		// Token: 0x06009569 RID: 38249 RVA: 0x0045B1AE File Offset: 0x004593AE
		private void ExchangeResource(int charId)
		{
			UIElement.Exchange.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", charId));
			UIManager.Instance.ShowUI(UIElement.Exchange, true);
		}

		// Token: 0x0600956A RID: 38250 RVA: 0x0045B1DD File Offset: 0x004593DD
		private void OnExchangeResource(ArgumentBox args)
		{
			this.CallRefreshItems(null);
		}

		// Token: 0x0600956B RID: 38251 RVA: 0x0045B1E8 File Offset: 0x004593E8
		private void OnClickButtonAutoOperationSetting()
		{
			UIManager.Instance.MaskUI(UIElement.ItemAutoOperation);
		}

		// Token: 0x0600956C RID: 38252 RVA: 0x0045B1FB File Offset: 0x004593FB
		public override void OnLanguageChange(LocalStringManager.LanguageType languageType)
		{
			base.OnLanguageChange(languageType);
		}

		// Token: 0x06009571 RID: 38257 RVA: 0x0045B2F4 File Offset: 0x004594F4
		[CompilerGenerated]
		private void <InitMultiplyItemScrollView>g__OnEnterMultiplyOperation|69_0()
		{
			base.CharacterMenu.IsMultiplySelect = true;
			this._IsForceHideAttribute = true;
			base.CharacterMenu.OnTryClosePage = delegate()
			{
				this.multiplyItemListScroll.TryExitMultiplyMode(delegate
				{
					base.CharacterMenu.OnTryClosePage = null;
				});
			};
			base.CharacterMenu.EnterFocusMode(null, false, true);
			base.CharacterMenu.SetBaseAttributeState(-1);
		}

		// Token: 0x06009574 RID: 38260 RVA: 0x0045B370 File Offset: 0x00459570
		[CompilerGenerated]
		private void <InitMultiplyItemScrollView>g__OnExitMultiplyOperation|69_1()
		{
			base.CharacterMenu.IsMultiplySelect = false;
			this._IsForceHideAttribute = false;
			base.CharacterMenu.ExitFocusMode(true);
			base.CharacterMenu.SetBaseAttributeState(this.ShowBaseAttribute ? -2 : -1);
			base.CharacterMenu.OnTryClosePage = null;
		}

		// Token: 0x06009575 RID: 38261 RVA: 0x0045B3C4 File Offset: 0x004595C4
		[CompilerGenerated]
		private void <InitMultiplyItemScrollView>g__OnEnterMultiplyLock|69_2()
		{
			base.CharacterMenu.OnTryClosePage = delegate()
			{
				this.multiplyItemListScroll.TryExitMultiplyMode(delegate
				{
					base.CharacterMenu.OnTryClosePage = null;
				});
			};
			base.CharacterMenu.EnterFocusMode(null, false, false);
		}

		// Token: 0x06009578 RID: 38264 RVA: 0x0045B415 File Offset: 0x00459615
		[CompilerGenerated]
		private void <InitMultiplyItemScrollView>g__OnExitMultiplyLock|69_3()
		{
			base.CharacterMenu.OnTryClosePage = null;
			base.CharacterMenu.ExitFocusMode(true);
		}

		// Token: 0x04007286 RID: 29318
		[SerializeField]
		private GameObject itemPage;

		// Token: 0x04007287 RID: 29319
		[SerializeField]
		private Refers choosyPage;

		// Token: 0x04007288 RID: 29320
		[SerializeField]
		private ItemListScroll itemListScroll;

		// Token: 0x04007289 RID: 29321
		[SerializeField]
		private GameObject itemListScrollListStyle;

		// Token: 0x0400728A RID: 29322
		[SerializeField]
		private GameObject itemListScrollCardStyle;

		// Token: 0x0400728B RID: 29323
		[SerializeField]
		private GameObject maskToFocusGroupedItemList;

		// Token: 0x0400728C RID: 29324
		[SerializeField]
		private ItemListScroll selectedItemListScroll;

		// Token: 0x0400728D RID: 29325
		[SerializeField]
		private float itemListScrollHeightFull;

		// Token: 0x0400728E RID: 29326
		[SerializeField]
		private float itemListScrollHeightSelected;

		// Token: 0x0400728F RID: 29327
		[SerializeField]
		private MultiplyItemListScroll multiplyItemListScroll;

		// Token: 0x04007290 RID: 29328
		[SerializeField]
		private ItemIdentifyPoison identifiedPoisonTip;

		// Token: 0x04007291 RID: 29329
		[SerializeField]
		private GameObject selectedTitle;

		// Token: 0x04007292 RID: 29330
		[SerializeField]
		private TextMeshProUGUI loadText;

		// Token: 0x04007293 RID: 29331
		[SerializeField]
		private TooltipInvoker loadOverflowTips;

		// Token: 0x04007294 RID: 29332
		[SerializeField]
		private CButton buttonAutoOperationSetting;

		// Token: 0x04007295 RID: 29333
		[SerializeField]
		private GameObject transferDetail;

		// Token: 0x04007296 RID: 29334
		[SerializeField]
		private CardItem transferDetailCardItem;

		// Token: 0x04007297 RID: 29335
		[SerializeField]
		private CToggleGroup targetToggleGroup;

		// Token: 0x04007298 RID: 29336
		[SerializeField]
		private CharacterMenuLocalLoadingAnim localLoadingAnim;

		// Token: 0x04007299 RID: 29337
		private static readonly List<sbyte> CanRepairLifeSkillType = new List<sbyte>
		{
			6,
			7,
			10,
			11,
			8,
			9
		};

		// Token: 0x0400729A RID: 29338
		private static readonly List<sbyte> CanDisassembleLifeSkillType = new List<sbyte>
		{
			6,
			7,
			10,
			11,
			14,
			8
		};

		// Token: 0x0400729B RID: 29339
		private static ItemOperationType.EItemOperationType _currItemOperation;

		// Token: 0x0400729C RID: 29340
		private int _taiwuCharId;

		// Token: 0x0400729D RID: 29341
		private ItemDisplayData _selectedToTransferItemData;

		// Token: 0x0400729E RID: 29342
		private ITradeableContent _clickedItemData;

		// Token: 0x0400729F RID: 29343
		private bool _IsForceHideAttribute = false;

		// Token: 0x040072A0 RID: 29344
		private int _transferFocusItemSiblingIndex;

		// Token: 0x040072A1 RID: 29345
		private Transform _transferFocusItem;

		// Token: 0x040072A2 RID: 29346
		private Transform _transferFocusItemParent;

		// Token: 0x040072A3 RID: 29347
		private CharacterItemsDisplayData _characterItemsDisplayData;

		// Token: 0x040072A4 RID: 29348
		private bool _curIdentifySuccess;

		// Token: 0x040072A5 RID: 29349
		private bool _curIdentifiedResultHasPoison;

		// Token: 0x040072A6 RID: 29350
		private Action _curIdentifiedResultAction;

		// Token: 0x040072A7 RID: 29351
		[TupleElementNames(new string[]
		{
			"operationType",
			"itemKey"
		})]
		private ValueTuple<ItemOperationType.EItemOperationType, ItemKey> _preOperation = new ValueTuple<ItemOperationType.EItemOperationType, ItemKey>(ItemOperationType.EItemOperationType.Invalid, ItemKey.Invalid);

		// Token: 0x040072A8 RID: 29352
		private const int HeavenlyTreeNeedDistance = 6;
	}
}
