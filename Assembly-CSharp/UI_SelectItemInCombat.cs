using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CharacterDataMonitor;
using Config;
using FrameWork;
using Game.Views;
using Game.Views.CharacterMenu;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020003A6 RID: 934
public class UI_SelectItemInCombat : UIBase
{
	// Token: 0x170005C0 RID: 1472
	// (get) Token: 0x0600381C RID: 14364 RVA: 0x001C4085 File Offset: 0x001C2285
	private CombatModel Model
	{
		get
		{
			return SingletonObject.getInstance<CombatModel>();
		}
	}

	// Token: 0x170005C1 RID: 1473
	// (get) Token: 0x0600381D RID: 14365 RVA: 0x001C408C File Offset: 0x001C228C
	private int TotalWisdomCount
	{
		get
		{
			return (int)(this._wisdomCount + this._specialWisdomCount);
		}
	}

	// Token: 0x170005C2 RID: 1474
	// (get) Token: 0x0600381E RID: 14366 RVA: 0x001C409B File Offset: 0x001C229B
	private bool SelectingRepairItem
	{
		get
		{
			return this._itemScroll.SortAndFilter.IsFilterTypeVisible(ItemSortAndFilter.ItemFilterType.Equip);
		}
	}

	// Token: 0x0600381F RID: 14367 RVA: 0x001C40B0 File Offset: 0x001C22B0
	public override void OnInit(ArgumentBox argsBox)
	{
		sbyte wisdomType;
		argsBox.Get("WisdomType", out wisdomType);
		argsBox.Get("WisdomCount", out this._wisdomCount);
		argsBox.Get("SpecialWisdomCount", out this._specialWisdomCount);
		argsBox.Get("CanEatMore", out this._canEatMore);
		argsBox.Get("CanUseSwordFragment", out this._canUseSwordFragment);
		argsBox.Get<MainAttributes>("MainAttributes", out this._mainAttributes);
		argsBox.Get<sbyte[]>("WeaponTricks", out this._weaponTricks);
		List<ItemDisplayData> itemList;
		argsBox.Get<List<ItemDisplayData>>("ItemList", out itemList);
		argsBox.Get<Action<ItemKey, sbyte, ItemKey, List<sbyte>>>("CallBack", out this._onSelected);
		argsBox.Get("CharId", out this._currentCharId);
		ItemDisplayData itemDisplayData = itemList.Find(delegate(ItemDisplayData d)
		{
			ItemKey key = d.Key;
			return key.ItemType == 6 && key.TemplateId == 54;
		});
		this._emptyToolKey = ((itemDisplayData != null) ? itemDisplayData.Key : ItemKey.Invalid);
		this._selectedInjuryPartList.Clear();
		this._selectedRepairItem = ItemKey.Invalid;
		this._wisdomIcon = ((wisdomType == 0) ? "sp_icon_renwutexing_11" : ((wisdomType == 1) ? "sp_icon_renwutexing_5" : "sp_icon_renwutexing_8"));
		this._selectedItem = ItemKey.Invalid;
		this._useType = -1;
		this._itemList.Clear();
		bool flag = this._popupWindow == null;
		if (flag)
		{
			this._popupWindow = base.CGet<PopupWindow>("PopupWindowBase");
			this._popupWindow.OnConfirmClick = new Action(this.OnConfirmSelect);
			this._popupWindow.OnCancelClick = new Action(this.QuickHide);
			this._itemScroll = base.CGet<ItemScrollView>("ItemScrollView");
			this._itemScroll.Init();
			this._itemScroll.SetItemList(ref this._itemList, true, null, this._itemScroll.SortAndFilter.IsDetailView, new Action<ItemDisplayData, ItemView>(this.OnRenderItem));
			this._itemScroll.SortAndFilter.ShowFilterType(UI_SelectItemInCombat.DefaultFilterTypes);
			this._charAttrDataView = base.CGet<CharacterAttributeDataView>("CharacterAttributeView");
		}
		this._popupWindow.ConfirmButton.interactable = false;
		this._popupWindow.CancelButton.interactable = true;
		this._itemList.AddRange(itemList);
		bool flag2 = !SingletonObject.getInstance<ProfessionModel>().IsSkillEquipped(54);
		if (flag2)
		{
			this._itemList.RemoveAll((ItemDisplayData data) => data.Key.ItemType == 5);
		}
		this._resourceMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<ResourceMonitor>(this._currentCharId, false);
		this._lifeSkillMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<LifeSkillMonitor>(this._currentCharId, false);
		this._equipCombatSkillMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<EquipCombatSkillMonitor>(this._currentCharId, false);
		base.CGet<CImage>("WisdomIcon").SetSprite(this._wisdomIcon, false, null);
		this._wisdomCost = 0;
		this.RefreshWisdomCount();
		this.NeedDataListenerId = true;
		UIElement element = this.Element;
		element.OnActive = (Action)Delegate.Combine(element.OnActive, new Action(delegate()
		{
			this._charAttrDataView.SetCurrentCharacterId(this._currentCharId);
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
			{
				this._itemScroll.SortAndFilter.StaticAheadItemKeysList.Clear();
				this._itemScroll.SortAndFilter.StaticAheadItemKeysList.Add(this._emptyToolKey);
				this._itemScroll.SetItemList(ref this._itemList, false, null, false, null);
				this.Element.ShowAfterRefresh();
			});
		}));
	}

	// Token: 0x06003820 RID: 14368 RVA: 0x001C43CC File Offset: 0x001C25CC
	private void RefreshWisdomCount()
	{
		string totalCountStr = (this._specialWisdomCount == 0) ? this._wisdomCount.ToString() : string.Format("{0}+{1}", (int)(this._wisdomCount - this._specialWisdomCount), this._specialWisdomCount.ToString().SetColor("brightblue"));
		base.CGet<TextMeshProUGUI>("WisdomCount").text = ((this._wisdomCost == 0) ? totalCountStr : (totalCountStr + (-this._wisdomCost).ToString().SetColor("brightred")).ColorReplace());
	}

	// Token: 0x06003821 RID: 14369 RVA: 0x001C4464 File Offset: 0x001C2664
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 1)
			{
				bool flag = notification.DomainId == 6;
				if (flag)
				{
					bool flag2 = notification.MethodId == 14;
					if (flag2)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._repairItemKeys);
						ItemDomainMethod.Call.GetItemDisplayDataListOptional(this.Element.GameDataListenerId, this._repairItemKeys, this._currentCharId, ItemSourceType.Inventory.ToSbyte());
					}
					else
					{
						bool flag3 = notification.MethodId == 27;
						if (flag3)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._repairItemDatas);
							this._itemScroll.SetItemList(ref this._repairItemDatas, false, null, false, null);
							this._itemScroll.SortAndFilter.ShowFilterType(ItemSortAndFilter.ItemFilterType.Equip);
							this._itemScroll.SortAndFilter.LockFilterType(ItemSortAndFilter.ItemFilterType.Equip, ItemSortAndFilter.LockFilterTypeToggleActionMode.Default);
							RectTransform focusMaskTransform = base.CGet<RectTransform>("FullScreenMask");
							Transform[] focusTransforms = new Transform[]
							{
								base.CGet<RectTransform>("ItemTitleBack"),
								base.CGet<RectTransform>("ItemListBack"),
								this._itemScroll.transform,
								base.CGet<RectTransform>("WisdomBack")
							};
							focusMaskTransform.gameObject.SetActive(true);
							int originChildCountInFocusMask = focusMaskTransform.childCount;
							for (int i = 0; i < focusTransforms.Length; i++)
							{
								Transform focusTransform = focusTransforms[i];
								this._focusTransforms.Add(new ValueTuple<Transform, Transform, int>(focusTransform, focusTransform.parent, focusTransform.GetSiblingIndex()));
								focusTransform.SetParent(focusMaskTransform, true);
								focusTransform.SetSiblingIndex(i + originChildCountInFocusMask);
							}
						}
					}
				}
				else
				{
					bool flag4 = notification.DomainId == 9;
					if (flag4)
					{
						bool flag5 = notification.MethodId == 42;
						if (flag5)
						{
							bool canRepair = false;
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref canRepair);
							bool flag6 = canRepair;
							if (flag6)
							{
								DialogCmd dialogCmd = new DialogCmd();
								dialogCmd.Type = 1;
								dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_Item_Repair_Tip_Title);
								dialogCmd.Content = LocalStringManager.Get(LanguageKey.LK_Item_Repair_In_Combat_Tips);
								dialogCmd.Yes = delegate()
								{
									this._onSelected(this._selectedItem, -1, this._itemToRepair, null);
									UIManager.Instance.HideUI(this.Element);
								};
								UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
								UIManager.Instance.MaskUI(UIElement.Dialog);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06003822 RID: 14370 RVA: 0x001C4734 File Offset: 0x001C2934
	private void OnEnable()
	{
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
		{
			this._charAttrDataView.SetTabState(1);
		});
	}

	// Token: 0x06003823 RID: 14371 RVA: 0x001C474F File Offset: 0x001C294F
	private void OnDisable()
	{
		this._itemList.Clear();
		this.ExitFocus();
		this._wisdomCost = 0;
		this.RefreshWisdomCount();
		this._charAttrDataView.HideInfectNotice(true, true);
		this._charAttrDataView.HideEatDropNotice(true);
	}

	// Token: 0x06003824 RID: 14372 RVA: 0x001C4790 File Offset: 0x001C2990
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = btnName == "FocusMask";
		if (flag)
		{
			this.ExitFocus();
		}
		else
		{
			bool flag2 = btnName == "FullScreenMask";
			if (flag2)
			{
				this._selectedItem = ItemKey.Invalid;
				this.ExitFocus();
			}
		}
	}

	// Token: 0x06003825 RID: 14373 RVA: 0x001C47E4 File Offset: 0x001C29E4
	public override void QuickHide()
	{
		AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
		bool selectingRepairItem = this.SelectingRepairItem;
		if (selectingRepairItem)
		{
			this._selectedItem = ItemKey.Invalid;
			this.ExitFocus();
		}
		else
		{
			bool isSelectInjuryPart = this._charAttrDataView.IsSelectInjuryPart;
			if (isSelectInjuryPart)
			{
				this._charAttrDataView.ExitSelectInjuryPart();
			}
			else
			{
				base.QuickHide();
			}
		}
	}

	// Token: 0x06003826 RID: 14374 RVA: 0x001C4848 File Offset: 0x001C2A48
	private void OnRenderItem(ItemDisplayData itemData, ItemView itemView)
	{
		UI_SelectItemInCombat.<>c__DisplayClass41_0 CS$<>8__locals1 = new UI_SelectItemInCombat.<>c__DisplayClass41_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.itemView = itemView;
		CS$<>8__locals1.itemData = itemData;
		ItemKey key = CS$<>8__locals1.itemData.Key;
		int costWisdom = key.GetConsumedFeatureMedals();
		CombatSubProcessorCharacter processor;
		bool flag = this.Model.ProcessorCharacters.TryGetValue(this._currentCharId, out processor) && processor.UseItemCostNoWisdom;
		if (flag)
		{
			costWisdom = 0;
		}
		bool costEnough = costWisdom <= this.TotalWisdomCount;
		TextMeshProUGUI costTextEnough = CS$<>8__locals1.itemView.CGet<TextMeshProUGUI>("CostWisdomEnough");
		TextMeshProUGUI costTextNotEnough = CS$<>8__locals1.itemView.CGet<TextMeshProUGUI>("CostWisdomNotEnough");
		bool canUse = costEnough;
		string itemTip = string.Empty;
		bool flag2 = canUse;
		if (flag2)
		{
			bool flag3 = key.ItemType == 7 || key.ItemType == 9;
			if (flag3)
			{
				canUse = this._canEatMore;
			}
			else
			{
				bool flag4 = ItemTemplateHelper.IsTianJieFuLu(CS$<>8__locals1.itemData.Key.ItemType, CS$<>8__locals1.itemData.Key.TemplateId);
				if (flag4)
				{
					canUse = (this._canEatMore && CS$<>8__locals1.itemData.Amount >= ItemTemplateHelper.GetTianJieFuLuCountUnit());
				}
				else
				{
					bool flag5 = key.ItemType == 12 && GameData.Domains.Combat.SharedConstValue.SwordFragment2BossId.ContainsKey(key.TemplateId);
					if (flag5)
					{
						short skillId = (short)CS$<>8__locals1.itemData.SpecialArg;
						bool flag6 = skillId >= 0;
						if (flag6)
						{
							List<NeedTrick> skillTricks = CombatSkill.Instance[skillId].TrickCost;
							canUse = (this._canUseSwordFragment && !skillTricks.Exists((NeedTrick costTrick) => !CS$<>8__locals1.<>4__this._weaponTricks.Exist(costTrick.TrickType)));
						}
						else
						{
							canUse = false;
						}
					}
					else
					{
						bool flag7 = key.ItemType == 8;
						if (flag7)
						{
							canUse = !UI_UsingMedicineItem.CheckEatItemIsLocked(CS$<>8__locals1.itemData, this._currentCharId, (int)UsingMedicineItemType.Invalid, this._charAttrDataView, out itemTip);
						}
					}
				}
			}
		}
		costTextEnough.gameObject.SetActive(costEnough);
		costTextNotEnough.gameObject.SetActive(!costEnough);
		(costEnough ? costTextEnough : costTextNotEnough).text = string.Format("x{0}", costWisdom);
		CS$<>8__locals1.itemView.CGet<CImage>("WisdomIcon").SetSprite(this._wisdomIcon, false, null);
		CS$<>8__locals1.itemView.SetClickEvent(delegate
		{
			CS$<>8__locals1.<>4__this._itemScroll.HandleClickItem(CS$<>8__locals1.itemView.Data, CS$<>8__locals1.itemView, new Action<ItemView>(CS$<>8__locals1.<>4__this.OnClickItem));
		});
		CS$<>8__locals1.itemView.SetHighLight(CS$<>8__locals1.itemData.ContainsItemKey(this._selectedItem));
		CS$<>8__locals1.itemView.SetLocked(!canUse);
		bool flag8 = canUse;
		if (flag8)
		{
			CS$<>8__locals1.itemView.HideInteractionState();
		}
		else
		{
			bool flag9 = !itemTip.IsNullOrEmpty();
			if (flag9)
			{
				CS$<>8__locals1.itemView.SetInteractionStateLockText(itemTip);
			}
		}
		CS$<>8__locals1.itemView.SetEnterEvent(null);
		CS$<>8__locals1.itemView.SetExitEvent(new UnityAction(this.ResetCursor));
		bool selectingRepairItem = this.SelectingRepairItem;
		if (selectingRepairItem)
		{
			ItemDisplayData tool = this._itemList.Find((ItemDisplayData data) => data.ContainsItemKey(CS$<>8__locals1.<>4__this._selectedItem));
			short durabilityCost;
			ResourceInts needResource;
			CS$<>8__locals1.<OnRenderItem>g__RefreshRepairItemState|1(tool, out durabilityCost, out needResource);
			TooltipInvoker tipDisplayer = CS$<>8__locals1.itemView.GetComponent<TooltipInvoker>();
			tipDisplayer.Type = TipType.RepairItem;
			tipDisplayer.NeedRefresh = true;
			tipDisplayer.RuntimeParam = new ArgumentBox().SetObject("Resource", new ResourceInts(this._resourceMonitor.Resources)).SetObject("NeedResource", needResource).SetObject("Item", CS$<>8__locals1.itemData).SetObject("Tool", tool).Set("DurabilityCost", durabilityCost);
		}
		else
		{
			CS$<>8__locals1.itemView.HideInteractionState();
		}
	}

	// Token: 0x06003827 RID: 14375 RVA: 0x001C4BF8 File Offset: 0x001C2DF8
	private void OnClickItem(ItemView itemView)
	{
		UI_SelectItemInCombat.<>c__DisplayClass42_0 CS$<>8__locals1 = new UI_SelectItemInCombat.<>c__DisplayClass42_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.itemView = itemView;
		CS$<>8__locals1.itemKey = CS$<>8__locals1.itemView.Data.Key;
		bool flag = CommonUtils.IsRecoverInjuryMainMedicineItem(CS$<>8__locals1.itemKey);
		if (flag)
		{
			bool isSelectInjuryPart = this._charAttrDataView.IsSelectInjuryPart;
			if (isSelectInjuryPart)
			{
				this._charAttrDataView.ExitSelectInjuryPart();
				return;
			}
		}
		bool flag2 = this._selectedItem.Equals(CS$<>8__locals1.itemKey);
		if (flag2)
		{
			this.CancelSelection(true, true);
		}
		else
		{
			bool selectingRepairItem = this.SelectingRepairItem;
			if (selectingRepairItem)
			{
				this._itemToRepair = CS$<>8__locals1.itemKey;
				BuildingDomainMethod.Call.CheckRepairConditionIsMeet(this.Element.GameDataListenerId, this._currentCharId, this._selectedItem, this._itemToRepair, BuildingBlockKey.Invalid);
			}
			else
			{
				this.ExitFocus();
				EatingItemMonitor eatingItems = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<EatingItemMonitor>(this._currentCharId, false);
				ValueTuple<int, string> valueTuple = CommonUtils.CalculateCountAndTip(this._currentCharId, CS$<>8__locals1.itemView.Data.RealKey, CS$<>8__locals1.itemView.Data.Amount);
				int count = valueTuple.Item1;
				string reason = valueTuple.Item2;
				bool canEatMore = count > 0;
				bool hasEatWugKing = (from x in eatingItems.EatingItemList
				select x.Item1).Any(new Func<ItemKey, bool>(EatingItems.IsWugKing));
				CS$<>8__locals1.isWugKing = EatingItems.IsWugKing(CS$<>8__locals1.itemKey);
				bool canReplaceWugKing = CS$<>8__locals1.isWugKing && hasEatWugKing;
				CS$<>8__locals1.sheetInfos = new List<ViewPopupMenu.BtnData>();
				bool flag3 = CS$<>8__locals1.itemKey.ItemType == 8;
				if (flag3)
				{
					MedicineItem medicineConfig = Medicine.Instance[CS$<>8__locals1.itemKey.TemplateId];
					bool isOuterMedicine = medicineConfig.EffectType == EMedicineEffectType.RecoverOuterInjury;
					bool isInnerMedicine = medicineConfig.EffectType == EMedicineEffectType.RecoverInnerInjury;
					bool healOuterLocked = SingletonObject.getInstance<DisplayTriggerModel>().HealingOuterRestriction;
					bool healInnerLocked = SingletonObject.getInstance<DisplayTriggerModel>().HealingInnerRestriction;
					bool medicineLocked = (isOuterMedicine && healOuterLocked) || (isInnerMedicine && healInnerLocked);
					bool innerInteractable = (canEatMore && !medicineLocked) || canReplaceWugKing;
					string tipContent = string.Empty;
					bool flag4 = !innerInteractable && canEatMore;
					if (flag4)
					{
						tipContent = LocalStringManager.Get(LanguageKey.LK_Use_Medicine_Tip_NotAllow);
					}
					string btnName = CommonUtils.GetCanEatItemButtonName(CS$<>8__locals1.itemKey);
					ViewPopupMenu.BtnData innerButton = new ViewPopupMenu.BtnData(btnName, innerInteractable, EItemMenuDisplayOrder.Eat, new Action(CS$<>8__locals1.<OnClickItem>g__OnButtonClick|1), new UnityAction(CS$<>8__locals1.<OnClickItem>g__OnButtonEnter|2), new UnityAction(CS$<>8__locals1.<OnClickItem>g__OnButtonExit|3), false);
					innerButton.SetTip(string.Empty, tipContent);
					CS$<>8__locals1.sheetInfos.Add(innerButton);
					bool flag5 = medicineConfig.MaxUseDistance >= 0;
					if (flag5)
					{
						CS$<>8__locals1.<OnClickItem>g__AddTrow|4();
					}
				}
				else
				{
					bool flag6 = CommonUtils.CanItemEat(CS$<>8__locals1.itemKey.ItemType, CS$<>8__locals1.itemKey.TemplateId, this._currentCharId);
					if (flag6)
					{
						bool flag7 = ItemTemplateHelper.IsTianJieFuLu(CS$<>8__locals1.itemKey.ItemType, CS$<>8__locals1.itemKey.TemplateId);
						if (flag7)
						{
							bool innerInteractable2 = canEatMore;
							string tipContent2 = string.Empty;
							bool flag8 = !canEatMore;
							if (flag8)
							{
								tipContent2 = LocalStringManager.Get(LanguageKey.LK_Use_Medicine_Tip_NoSlot);
							}
							ViewPopupMenu.BtnData innerButton2 = new ViewPopupMenu.BtnData(LocalStringManager.Get(LanguageKey.LK_Eat_Item), innerInteractable2, EItemMenuDisplayOrder.Eat, new Action(CS$<>8__locals1.<OnClickItem>g__OnButtonClick|1), new UnityAction(CS$<>8__locals1.<OnClickItem>g__OnButtonEnter|2), new UnityAction(CS$<>8__locals1.<OnClickItem>g__OnButtonExit|3), false);
							innerButton2.SetTip(string.Empty, tipContent2);
							CS$<>8__locals1.sheetInfos.Add(innerButton2);
						}
						else
						{
							bool neiliIsNotMax = this._equipCombatSkillMonitor.CurrNeili < this._equipCombatSkillMonitor.MaxNeili;
							bool innerInteractable3 = (CS$<>8__locals1.itemKey.ItemType == 12) ? neiliIsNotMax : canEatMore;
							string tipContent3 = string.Empty;
							bool flag9 = !canEatMore;
							if (flag9)
							{
								tipContent3 = LocalStringManager.Get(LanguageKey.LK_Use_Medicine_Tip_NoSlot);
							}
							bool flag10 = !neiliIsNotMax;
							if (flag10)
							{
								tipContent3 = LocalStringManager.Get(LanguageKey.LK_ItemTips_Use_NeiliIsMax);
							}
							ViewPopupMenu.BtnData innerButton3 = new ViewPopupMenu.BtnData(LocalStringManager.Get(LanguageKey.LK_Eat_Item), innerInteractable3, EItemMenuDisplayOrder.Eat, new Action(CS$<>8__locals1.<OnClickItem>g__OnButtonClick|1), new UnityAction(CS$<>8__locals1.<OnClickItem>g__OnButtonEnter|2), new UnityAction(CS$<>8__locals1.<OnClickItem>g__OnButtonExit|3), false);
							innerButton3.SetTip(string.Empty, tipContent3);
							CS$<>8__locals1.sheetInfos.Add(innerButton3);
						}
					}
					else
					{
						bool flag11 = CS$<>8__locals1.itemKey.ItemType == 12;
						if (flag11)
						{
							MiscItem miscConfig = Misc.Instance.GetItem(CS$<>8__locals1.itemKey.TemplateId);
							bool flag12 = miscConfig.MaxUseDistance > 0;
							if (flag12)
							{
								CS$<>8__locals1.<OnClickItem>g__AddTrow|4();
							}
							else
							{
								bool flag13 = GameData.Domains.Combat.SharedConstValue.SwordFragment2BossId.ContainsKey(CS$<>8__locals1.itemKey.TemplateId);
								if (flag13)
								{
									ViewPopupMenu.BtnData innerButton4 = new ViewPopupMenu.BtnData(LocalStringManager.Get(LanguageKey.LK_Use_SwordFragment), true, EItemMenuDisplayOrder.Other, new Action(CS$<>8__locals1.<OnClickItem>g__OnButtonClick|1), null, null, false);
									CS$<>8__locals1.sheetInfos.Add(innerButton4);
								}
								else
								{
									bool flag14 = CommonUtils.IsTianSuiBaoLuItem(CS$<>8__locals1.itemKey.ItemType, CS$<>8__locals1.itemKey.TemplateId);
									if (flag14)
									{
										ViewPopupMenu.BtnData useButton = new ViewPopupMenu.BtnData(LocalStringManager.Get(LanguageKey.LK_Use_TianSuiBaoLuItem), true, EItemMenuDisplayOrder.Other, new Action(CS$<>8__locals1.<OnClickItem>g__OnButtonClick|1), null, null, false);
										CS$<>8__locals1.sheetInfos.Add(useButton);
									}
								}
							}
						}
						else
						{
							ViewPopupMenu.BtnData button = new ViewPopupMenu.BtnData(LocalStringManager.Get(LanguageKey.LK_Repair_Item), true, EItemMenuDisplayOrder.Tool, delegate()
							{
								ItemDomainMethod.Call.GetRepairableItems(CS$<>8__locals1.<>4__this.Element.GameDataListenerId, CS$<>8__locals1.<>4__this._currentCharId, CS$<>8__locals1.itemKey);
								CS$<>8__locals1.<>4__this.SetSelectedItem(CS$<>8__locals1.itemView.Data, -1);
								CS$<>8__locals1.<>4__this._popupWindow.ConfirmButton.interactable = false;
								CS$<>8__locals1.<>4__this.ExitFocus();
							}, null, null, false);
							CS$<>8__locals1.sheetInfos.Add(button);
						}
					}
				}
				bool flag15 = CS$<>8__locals1.sheetInfos.Count > 0;
				if (flag15)
				{
					RectTransform itemMask = base.CGet<RectTransform>("ItemMask");
					itemMask.gameObject.SetActive(true);
					this._focusTransforms.Add(new ValueTuple<Transform, Transform, int>(CS$<>8__locals1.itemView.transform, CS$<>8__locals1.itemView.transform.parent, CS$<>8__locals1.itemView.transform.GetSiblingIndex()));
					CS$<>8__locals1.itemView.transform.SetParent(itemMask, true);
					this._itemScroll.SetItemToPopupMenuMode(CS$<>8__locals1.itemKey, CS$<>8__locals1.sheetInfos, delegate
					{
						CS$<>8__locals1.itemView.SetHighLight(false);
						CS$<>8__locals1.<>4__this.ExitFocus();
					});
				}
			}
		}
	}

	// Token: 0x06003828 RID: 14376 RVA: 0x001C5210 File Offset: 0x001C3410
	private void SetSelectedItem(ItemDisplayData itemData, sbyte useType = -1)
	{
		UI_SelectItemInCombat.<>c__DisplayClass43_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.itemData = itemData;
		CS$<>8__locals1.useType = useType;
		bool canEat = CommonUtils.CanItemEat(CS$<>8__locals1.itemData.Key.ItemType, CS$<>8__locals1.itemData.Key.TemplateId, this._currentCharId) && CS$<>8__locals1.useType < 1;
		bool flag = this._selectedItem.IsValid();
		if (flag)
		{
			this.CancelSelection(false, !canEat);
		}
		else
		{
			this._charAttrDataView.HideEatDropNotice(false);
			this._charAttrDataView.HideInfectNotice(false, !canEat);
		}
		bool flag2 = CS$<>8__locals1.itemData.ContainsItemKey(this._previewedItem);
		if (flag2)
		{
			this.<SetSelectedItem>g__Select|43_0(ref CS$<>8__locals1);
		}
		else
		{
			bool flag3 = canEat;
			if (flag3)
			{
				bool flag4 = this._useType == 0;
				if (flag4)
				{
					this._charAttrDataView.ShowEatDropNotice(CS$<>8__locals1.itemData, 1);
					this._charAttrDataView.ShowInfectNotice(CS$<>8__locals1.itemData, 1);
				}
			}
			this.<SetSelectedItem>g__Select|43_0(ref CS$<>8__locals1);
			this._wisdomCost = CS$<>8__locals1.itemData.Key.GetConsumedFeatureMedals();
			this.RefreshWisdomCount();
		}
	}

	// Token: 0x06003829 RID: 14377 RVA: 0x001C5338 File Offset: 0x001C3538
	private void CancelSelection(bool backToPrevState = true, bool refreshData = true)
	{
		this._popupWindow.ConfirmButton.interactable = false;
		bool flag = this._selectedItem.IsValid();
		if (flag)
		{
			ItemView itemView = this._itemScroll.FindActiveItem(this._selectedItem, false);
			if (itemView != null)
			{
				itemView.SetHighLight(false);
			}
		}
		this._selectedItem = ItemKey.Invalid;
		this._charAttrDataView.HideInfectNotice(backToPrevState, refreshData);
		this._charAttrDataView.HideEatDropNotice(backToPrevState);
		this._wisdomCost = 0;
		this.RefreshWisdomCount();
	}

	// Token: 0x0600382A RID: 14378 RVA: 0x001C53BC File Offset: 0x001C35BC
	private void ExitFocus()
	{
		base.CGet<RectTransform>("ItemMask").gameObject.SetActive(false);
		base.CGet<RectTransform>("FocusMask").gameObject.SetActive(false);
		base.CGet<RectTransform>("FullScreenMask").gameObject.SetActive(false);
		for (int i = 0; i < this._focusTransforms.Count; i++)
		{
			ValueTuple<Transform, Transform, int> transformInfo = this._focusTransforms[i];
			transformInfo.Item1.SetParent(transformInfo.Item2, true);
			transformInfo.Item1.SetSiblingIndex(transformInfo.Item3);
		}
		this._focusTransforms.Clear();
		bool selectingRepairItem = this.SelectingRepairItem;
		if (selectingRepairItem)
		{
			this._itemScroll.SortAndFilter.StaticAheadItemKeysList.Clear();
			this._itemScroll.SetItemList(ref this._itemList, false, null, false, null);
			this._itemScroll.SortAndFilter.ShowFilterType(UI_SelectItemInCombat.DefaultFilterTypes);
			this._itemScroll.SortAndFilter.LockFilterType(ItemSortAndFilter.ItemFilterType.Invalid, ItemSortAndFilter.LockFilterTypeToggleActionMode.Default);
			ConchShipCursor.Instance.CanChange = true;
			ConchShipCursor.Instance.SetDefaultCursor();
		}
	}

	// Token: 0x0600382B RID: 14379 RVA: 0x001C54E0 File Offset: 0x001C36E0
	private void OnConfirmSelect()
	{
		this._onSelected(this._selectedItem, this._useType, this._selectedRepairItem, this._selectedInjuryPartList);
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x0600382C RID: 14380 RVA: 0x001C5518 File Offset: 0x001C3718
	private void SetCursorForOperation()
	{
		ConchShipCursor.Instance.SetCursorImage("sp_cursor_clickable_chuizi", -1f, -1f);
		ConchShipCursor.Instance.CanChange = false;
	}

	// Token: 0x0600382D RID: 14381 RVA: 0x001C5540 File Offset: 0x001C3740
	private void ResetCursor()
	{
		ConchShipCursor.Instance.CanChange = true;
		ConchShipCursor.Instance.SetDefaultCursor();
	}

	// Token: 0x0600382E RID: 14382 RVA: 0x001C555C File Offset: 0x001C375C
	private void Update()
	{
		bool flag = this._popupWindow.ConfirmButton.interactable && CombatCommandKit.Pause.Check(this.Element, false, false, false, false, false);
		if (flag)
		{
			this.OnConfirmSelect();
		}
	}

	// Token: 0x06003835 RID: 14389 RVA: 0x001C56E4 File Offset: 0x001C38E4
	[CompilerGenerated]
	private void <SetSelectedItem>g__Select|43_0(ref UI_SelectItemInCombat.<>c__DisplayClass43_0 A_1)
	{
		ItemView itemView = this._itemScroll.FindActiveItem(A_1.itemData, false);
		bool flag = itemView;
		if (flag)
		{
			itemView.SetHighLight(true);
		}
		this._selectedItem = itemView.Data.Key;
		this._useType = A_1.useType;
		this._popupWindow.ConfirmButton.interactable = true;
		bool flag2 = A_1.itemData.Key.ItemType != 6;
		if (flag2)
		{
			this.OnConfirmSelect();
		}
	}

	// Token: 0x040028A3 RID: 10403
	private static readonly List<ItemSortAndFilter.ItemFilterType> DefaultFilterTypes = new List<ItemSortAndFilter.ItemFilterType>
	{
		ItemSortAndFilter.ItemFilterType.Food,
		ItemSortAndFilter.ItemFilterType.Medicine,
		ItemSortAndFilter.ItemFilterType.Material,
		ItemSortAndFilter.ItemFilterType.Make,
		ItemSortAndFilter.ItemFilterType.Other
	};

	// Token: 0x040028A4 RID: 10404
	private int _currentCharId;

	// Token: 0x040028A5 RID: 10405
	private string _wisdomIcon;

	// Token: 0x040028A6 RID: 10406
	private short _wisdomCount;

	// Token: 0x040028A7 RID: 10407
	private short _specialWisdomCount;

	// Token: 0x040028A8 RID: 10408
	private int _wisdomCost;

	// Token: 0x040028A9 RID: 10409
	private List<ItemDisplayData> _itemList = new List<ItemDisplayData>();

	// Token: 0x040028AA RID: 10410
	private Action<ItemKey, sbyte, ItemKey, List<sbyte>> _onSelected;

	// Token: 0x040028AB RID: 10411
	private ItemKey _selectedItem;

	// Token: 0x040028AC RID: 10412
	private ItemKey _previewedItem;

	// Token: 0x040028AD RID: 10413
	private ItemKey _selectedRepairItem;

	// Token: 0x040028AE RID: 10414
	private sbyte _useType;

	// Token: 0x040028AF RID: 10415
	private bool _canEatMore;

	// Token: 0x040028B0 RID: 10416
	private bool _canUseSwordFragment;

	// Token: 0x040028B1 RID: 10417
	private MainAttributes _mainAttributes;

	// Token: 0x040028B2 RID: 10418
	private sbyte[] _weaponTricks;

	// Token: 0x040028B3 RID: 10419
	private List<ItemKey> _repairItemKeys = new List<ItemKey>();

	// Token: 0x040028B4 RID: 10420
	private List<ItemDisplayData> _repairItemDatas = new List<ItemDisplayData>();

	// Token: 0x040028B5 RID: 10421
	private ItemKey _itemToRepair;

	// Token: 0x040028B6 RID: 10422
	private readonly List<sbyte> _selectedInjuryPartList = new List<sbyte>();

	// Token: 0x040028B7 RID: 10423
	private ItemKey _emptyToolKey;

	// Token: 0x040028B8 RID: 10424
	private ResourceMonitor _resourceMonitor;

	// Token: 0x040028B9 RID: 10425
	private LifeSkillMonitor _lifeSkillMonitor;

	// Token: 0x040028BA RID: 10426
	private EquipCombatSkillMonitor _equipCombatSkillMonitor;

	// Token: 0x040028BB RID: 10427
	private PopupWindow _popupWindow;

	// Token: 0x040028BC RID: 10428
	private ItemScrollView _itemScroll;

	// Token: 0x040028BD RID: 10429
	private CharacterAttributeDataView _charAttrDataView;

	// Token: 0x040028BE RID: 10430
	[TupleElementNames(new string[]
	{
		"focusTrans",
		"parent",
		"siblingIndex"
	})]
	private readonly List<ValueTuple<Transform, Transform, int>> _focusTransforms = new List<ValueTuple<Transform, Transform, int>>();
}
