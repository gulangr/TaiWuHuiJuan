using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using CharacterDataMonitor;
using CommonSortAndFilterLegacy;
using CommonSortAndFilterLegacy.Item;
using Config;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.Components;
using Game.Components.Avatar;
using Game.Views;
using Game.Views.CharacterMenu;
using Game.Views.Select;
using GameData.Common;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Profession;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.TaiwuEvent.Enum;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using GameDataExtensions;
using Spine;
using Spine.Unity;
using TMPro;
using UICommon.Character;
using UICommon.Character.Elements;
using UnityEngine;

// Token: 0x020001D2 RID: 466
public class UI_CharacterMenuItems : UI_CharacterMenuSubPageBase
{
	// Token: 0x17000301 RID: 769
	// (get) Token: 0x06001D95 RID: 7573 RVA: 0x000D1D7D File Offset: 0x000CFF7D
	public override LanguageKey TitleKey
	{
		get
		{
			return LanguageKey.LK_CharacterMenu_Title_Items;
		}
	}

	// Token: 0x06001D96 RID: 7574 RVA: 0x000D1D84 File Offset: 0x000CFF84
	public override bool CheckState(ECharacterSubToggleBase curSubTogglePage, ECharacterSubPage curSubPage)
	{
		return curSubPage == ECharacterSubPage.Prison || curSubTogglePage == ECharacterSubToggleBase.ItemBase;
	}

	// Token: 0x17000302 RID: 770
	// (get) Token: 0x06001D97 RID: 7575 RVA: 0x000D1DA1 File Offset: 0x000CFFA1
	public override bool ShowBaseAttribute
	{
		get
		{
			return this.CurTabIndex == 0;
		}
	}

	// Token: 0x17000303 RID: 771
	// (get) Token: 0x06001D98 RID: 7576 RVA: 0x000D1DAC File Offset: 0x000CFFAC
	private bool SelectingTransferItemChar
	{
		get
		{
			return this._currItemOperation == ItemOperationType.EItemOperationType.Transfer;
		}
	}

	// Token: 0x17000304 RID: 772
	// (get) Token: 0x06001D99 RID: 7577 RVA: 0x000D1DB7 File Offset: 0x000CFFB7
	private bool CurCharacterIsTaiwu
	{
		get
		{
			return base.CharacterMenu.CurrentCharacterIsTaiwu;
		}
	}

	// Token: 0x17000305 RID: 773
	// (get) Token: 0x06001D9A RID: 7578 RVA: 0x000D1DC4 File Offset: 0x000CFFC4
	private bool ChickenMapInteractable
	{
		get
		{
			return this._lastUseChickenMapAreaId != (int)SingletonObject.getInstance<WorldMapModel>().CurrentAreaId || this._chickenMapInteractable;
		}
	}

	// Token: 0x06001D9B RID: 7579 RVA: 0x000D1DE4 File Offset: 0x000CFFE4
	public override void OnInit(ArgumentBox argsBox)
	{
		this._currentCharacterDisplayData = null;
		this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		this._maxWorthCanBeLentToTaiwu = -1L;
		this._currItemOperation = ItemOperationType.EItemOperationType.Invalid;
		this._canOperateItems.Clear();
		this._inventoryItems.Clear();
		this.NeedDataListenerId = true;
		this.InitChoosyPage();
		int tabIndex;
		bool flag = argsBox.Get("TabIndex", out tabIndex);
		if (flag)
		{
			this.CurTabIndex = tabIndex;
		}
		Enum itemOperationType;
		bool flag2 = argsBox.Get("ItemOperationType", out itemOperationType);
		if (flag2)
		{
			this._targetItemOperationType = (ItemOperationType.EItemOperationType)itemOperationType;
		}
		else
		{
			this._targetItemOperationType = ItemOperationType.EItemOperationType.Invalid;
		}
		this._itemPage = base.CGet<Refers>("ItemPage");
		this._itemScroll = this._itemPage.CGet<GroupedItemScrollView2>("GroupedItemScrollView");
		this._itemScroll.Init(ESortAndFilterControllerType.Item, null);
		this._itemScroll.SetItemList(this._inventoryItems, new GroupedItemScrollView2.ItemGroupGetter(this.GroupItems), new Action<ItemDisplayData, CommonTableRowForItem>(this.OnRenderItem), true, "charMenu_item");
		this._selectedItemScroll = this._itemPage.CGet<ItemScrollViewForCommonTableRow>("SelectedItemScrollView");
		this._selectedItemScroll.Init(ESortAndFilterControllerType.Item, null);
		this._selectedItemScroll.SetItemList(ref this._inventoryItems, true, "charMenu_item_selected", new Action<ItemDisplayData, CommonTableRowForItem>(this.OnRenderItem), null, true);
		this._itemPageParent = this._itemPage.transform.parent;
		this._kidnapPage = base.CGet<Refers>("KidnapPage");
		this._kidnapScroll = this._kidnapPage.CGet<InfinityScrollLegacy>("ScrollView");
		this._kidnapScroll.OnItemRender = new Action<int, Refers>(this.OnRenderKidnapChar);
		this._kidnapScroll.OnItemHide = new Action<Refers>(this.OnKidnapCharHide);
		CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(this, this._taiwuCharId, delegate(int offset, RawDataPool dataPool)
		{
			Serializer.Deserialize(dataPool, offset, ref this._taiwuCharacterDisplayData);
		});
		this._attributeMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<AttributeMonitor>(this._taiwuCharId, false);
		this._attributeMonitor.AddMainAttributeListener(delegate(sbyte s)
		{
		});
		this._equipCombatSkillMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<EquipCombatSkillMonitor>(this._taiwuCharId, false);
		this.InitMultiplyItemScrollView();
	}

	// Token: 0x06001D9C RID: 7580 RVA: 0x000D2010 File Offset: 0x000D0210
	private void OnEnable()
	{
		GEvent.Add(UiEvents.ExitMultiplyOperation, new GEvent.Callback(this._multiplyItemScrollView.OnExitMultiplyOperation));
		GEvent.Add(UiEvents.ItemMultiplyOperationTypeChange, new GEvent.Callback(this.ItemMultiplyOperationTypeChange));
		GEvent.Add(UiEvents.ItemMultiplyOperationTargetChange, new GEvent.Callback(this.ItemMultiplyOperationTargetChange));
		GEvent.Add(UiEvents.ItemMultiplyOperationConfirm, new GEvent.Callback(this._multiplyItemScrollView.OnItemMultiplyOperationConfirm));
		GEvent.Add(UiEvents.ItemMultiplyOperationCancelSelection, new GEvent.Callback(this._multiplyItemScrollView.OnItemMultiplyOperationCancelSelection));
		GEvent.Add(UiEvents.OnRefreshCharacterMenuItem, new GEvent.Callback(this.CallRefreshItems));
		ExtraDomainMethod.AsyncCall.GetAllHeavenlyTrees(this, delegate(int offset, RawDataPool dataPool)
		{
			this._heavenlyTreeList.Clear();
			Serializer.Deserialize(dataPool, offset, ref this._heavenlyTreeList);
		});
	}

	// Token: 0x06001D9D RID: 7581 RVA: 0x000D20DC File Offset: 0x000D02DC
	private new void OnDisable()
	{
		GEvent.Remove(UiEvents.ExitMultiplyOperation, new GEvent.Callback(this._multiplyItemScrollView.OnExitMultiplyOperation));
		GEvent.Remove(UiEvents.ItemMultiplyOperationTypeChange, new GEvent.Callback(this.ItemMultiplyOperationTypeChange));
		GEvent.Remove(UiEvents.ItemMultiplyOperationTargetChange, new GEvent.Callback(this.ItemMultiplyOperationTargetChange));
		GEvent.Remove(UiEvents.ItemMultiplyOperationConfirm, new GEvent.Callback(this._multiplyItemScrollView.OnItemMultiplyOperationConfirm));
		GEvent.Remove(UiEvents.ItemMultiplyOperationCancelSelection, new GEvent.Callback(this._multiplyItemScrollView.OnItemMultiplyOperationCancelSelection));
		GEvent.Remove(UiEvents.OnRefreshCharacterMenuItem, new GEvent.Callback(this.CallRefreshItems));
		ResourceMonitor resourceMonitor = this._resourceMonitor;
		if (resourceMonitor != null)
		{
			resourceMonitor.RemoveResourceListener(new Action<sbyte>(this.OnResourceChange));
		}
		this._itemScroll.SaveSortFilterSetting(true);
		this.ClearKidnapCharElementDict();
		this._kidnapScroll.UpdateData(0);
	}

	// Token: 0x06001D9E RID: 7582 RVA: 0x000D21D4 File Offset: 0x000D03D4
	private void InitMultiplyItemScrollView()
	{
		this._multiplyItemScrollView = base.CGet<MultiplyItemScrollView2>("MultiplyItemScrollView");
		Dictionary<ItemSourceType, List<ItemDisplayData>> itemDict = new Dictionary<ItemSourceType, List<ItemDisplayData>>
		{
			{
				ItemSourceType.Inventory,
				this._inventoryItems
			}
		};
		this._multiplyItemScrollView.Init(base.CharacterMenu.CurCharacterId, itemDict, new Action(this.<InitMultiplyItemScrollView>g__OnEnter|61_0), new Action(this.<InitMultiplyItemScrollView>g__OnExit|61_1), null, this.IsTaiwuTeamButNotBeast);
		this.InitSwitchSelection();
	}

	// Token: 0x06001D9F RID: 7583 RVA: 0x000D2248 File Offset: 0x000D0448
	private void InitSwitchSelection()
	{
		bool flag = !this._multiplyItemScrollView.SwitchSelection;
		if (!flag)
		{
			this._multiplyItemScrollView.SwitchSelection.onValueChanged.RemoveAllListeners();
			this._multiplyItemScrollView.SwitchSelection.onValueChanged.AddListener(delegate(bool isOn)
			{
				int height = isOn ? 548 : 920;
				this._itemScroll.RectTransform.sizeDelta = this._itemScroll.RectTransform.sizeDelta.SetY((float)height);
				this._itemPage.CGet<GameObject>("SelectedTitle").SetActive(isOn);
				this._selectedItemScroll.gameObject.SetActive(isOn);
				this._multiplyItemScrollView.RefreshMultiplyCanOperateItems();
			});
			this._multiplyItemScrollView.SwitchSelection.isOn = false;
		}
	}

	// Token: 0x06001DA0 RID: 7584 RVA: 0x000D22BC File Offset: 0x000D04BC
	public override bool CanSubpageShow(ECharacterSubPage subPageIndex)
	{
		bool flag = subPageIndex == ECharacterSubPage.Prison;
		bool result;
		if (flag)
		{
			result = SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(19);
		}
		else
		{
			result = base.CanSubpageShow(subPageIndex);
		}
		return result;
	}

	// Token: 0x06001DA1 RID: 7585 RVA: 0x000D22EC File Offset: 0x000D04EC
	public override void OnSwitchToSubpage(int subPageIndex)
	{
		this.CurTabIndex = subPageIndex;
		this._itemPage.gameObject.SetActive(this.CurTabIndex == 0);
		this._kidnapPage.gameObject.SetActive(this.CurTabIndex == 1);
		base.CharacterMenu.SetBaseAttributeState((this.CurTabIndex == 0) ? 1 : -1);
		bool flag = this.CurTabIndex == 1;
		if (flag)
		{
			this._kidnapScroll.UpdateData(this._kidnapChars.Count);
		}
	}

	// Token: 0x06001DA2 RID: 7586 RVA: 0x000D2374 File Offset: 0x000D0574
	public override void OnSubpageVisible()
	{
		GEvent.Add(UiEvents.OnSelectTransferItemChar, new GEvent.Callback(this.OnSelectTransferItemChar));
		GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChange));
		base.CharacterMenu.SetBaseAttributeState((this.CurTabIndex == 0) ? 1 : -1);
		CharacterDomainMethod.Call.GetAllInventoryItems(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
		CharacterDomainMethod.Call.GetAllEquipmentItems(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
		CharacterDomainMethod.Call.GetKidnapMaxSlotCount(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
		CharacterDomainMethod.Call.GetSomeoneKidnapCharacters(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
	}

	// Token: 0x06001DA3 RID: 7587 RVA: 0x000D243C File Offset: 0x000D063C
	public override void OnSubpageInVisible()
	{
		GEvent.Remove(UiEvents.OnSelectTransferItemChar, new GEvent.Callback(this.OnSelectTransferItemChar));
		GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChange));
	}

	// Token: 0x06001DA4 RID: 7588 RVA: 0x000D2470 File Offset: 0x000D0670
	private void OnTopUiChange(ArgumentBox argumentBox)
	{
		bool flag = this._selectedToTransferItemData != null && this._currItemOperation == ItemOperationType.EItemOperationType.Transfer && !this._multiplyItemScrollView.IsMultiItemSelect;
		if (flag)
		{
		}
	}

	// Token: 0x06001DA5 RID: 7589 RVA: 0x000D24A8 File Offset: 0x000D06A8
	public override void OnCurrentCharacterChange(int prevCharacterId)
	{
		this._multiplyItemScrollView.CurCharId = base.CharacterMenu.CurCharacterId;
		bool flag = !this._multiplyItemScrollView.IsMultiItemSelect;
		if (flag)
		{
			this._multiplyItemScrollView.ShowMultiplySelectButton(null, base.CharacterMenu.CanOperate);
		}
		ResourceMonitor resourceMonitor = this._resourceMonitor;
		if (resourceMonitor != null)
		{
			resourceMonitor.RemoveResourceListener(new Action<sbyte>(this.OnResourceChange));
		}
		this.ClearKidnapCharElementDict();
		base.ClearMonitorFields();
		bool flag2 = base.CharacterMenu.CurCharacterId < 0;
		if (!flag2)
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(this, base.CharacterMenu.CurCharacterId, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._currentCharacterDisplayData);
			});
			this._resourceMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<ResourceMonitor>(base.CharacterMenu.CurCharacterId, false);
			this._resourceMonitor.AddResourceListener(new Action<sbyte>(this.OnResourceChange));
			this._lifeSkillMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<LifeSkillMonitor>(base.CharacterMenu.CurCharacterId, false);
			this._ageMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<AgeHealthMonitor>(base.CharacterMenu.CurCharacterId, false);
			this._basicInfoMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<BasicInfoMonitor>(base.CharacterMenu.CurCharacterId, false);
			base.AppendMonitorFieldId(new UIBase.MonitorDataField(4, 0, (ulong)((long)base.CharacterMenu.CurCharacterId), new uint[]
			{
				104U,
				103U,
				66U
			}));
			bool isTaiwuTeamButNotBeast = this.IsTaiwuTeamButNotBeast;
			if (isTaiwuTeamButNotBeast)
			{
				bool flag3 = !this.CurCharacterIsTaiwu;
				if (flag3)
				{
					this._warehouseItems.Clear();
					this._treasuryItems.Clear();
					CharacterDomainMethod.Call.GetMaxWorthCanBeLentToTaiwu(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
				}
				else
				{
					TaiwuDomainMethod.Call.GetAllWarehouseItems(this.Element.GameDataListenerId);
					TaiwuDomainMethod.Call.GetAllTreasuryItems(this.Element.GameDataListenerId);
				}
			}
			CharacterDomainMethod.Call.GetAllInventoryItems(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
			CharacterDomainMethod.Call.GetAllEquipmentItems(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
			CharacterDomainMethod.Call.GetKidnapMaxSlotCount(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
			CharacterDomainMethod.Call.GetSomeoneKidnapCharacters(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
			CharacterDomainMethod.Call.GetCharacterLoveAndHateItemInfo(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
			this.UpdateHobby();
			this._itemScroll.SetCharId(base.CharacterMenu.CurCharacterId);
			bool init = this._resourceMonitor.Init;
			if (init)
			{
				this._resourceMonitor.OnDataInit();
			}
		}
	}

	// Token: 0x06001DA6 RID: 7590 RVA: 0x000D2748 File Offset: 0x000D0948
	protected override void OnNotifyGameDataFiltered(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type2 = notification.Type;
			byte b = type2;
			if (b != 0)
			{
				if (b == 1)
				{
					bool flag = notification.DomainId == 4;
					if (flag)
					{
						bool flag2 = notification.MethodId == 27;
						if (flag2)
						{
							this._inventoryItems.Clear();
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._inventoryItems);
							for (int type = 0; type < 8; type++)
							{
								int amount = this._resourceMonitor.Resources[type];
								bool flag3 = amount <= 0;
								if (!flag3)
								{
									short templateId = Convert.ToInt16(type);
									ItemKey itemKey = new ItemKey(12, 0, templateId, 0);
									int value = amount * (int)GlobalConfig.ResourcesWorth[type];
									ItemDisplayData itemData = new ItemDisplayData
									{
										Key = itemKey,
										Amount = amount,
										Value = (long)value
									};
									this._inventoryItems.Add(itemData);
								}
							}
							this.ChickenMapHandle();
						}
						else
						{
							bool flag4 = notification.MethodId == 29;
							if (flag4)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._equipItems);
								sbyte slotIndex = 0;
								while ((int)slotIndex < this._equipItems.Count)
								{
									ItemDisplayData itemData2 = this._equipItems[(int)slotIndex];
									bool flag5 = itemData2.Key.IsValid();
									if (flag5)
									{
										itemData2.UsingType = ItemDisplayData.ItemUsingType.Equiped;
										this._inventoryItems.Add(itemData2);
									}
									slotIndex += 1;
								}
								this._itemScroll.SetItemList(ref this._inventoryItems);
								this._multiplyItemScrollView.RefreshItems();
							}
							else
							{
								bool flag6 = notification.MethodId == 38;
								if (flag6)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._maxKidnapSlotCount);
								}
								else
								{
									bool flag7 = notification.MethodId == 53;
									if (flag7)
									{
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._kidnapInfo);
										List<int> charIdList = new List<int>();
										List<ItemKey> ropeKeyList = new List<ItemKey>();
										bool flag8 = this._kidnapInfo != null;
										if (flag8)
										{
											for (int i = 0; i < this._kidnapInfo.GetCount(); i++)
											{
												KidnappedCharacter kidnappedChar = this._kidnapInfo.Get(i);
												charIdList.Add(kidnappedChar.CharId);
												ropeKeyList.Add(kidnappedChar.RopeItemKey);
											}
										}
										bool flag9 = charIdList.Count > 0;
										if (flag9)
										{
											CharacterDomainMethod.Call.GetCharacterDisplayDataList(this.Element.GameDataListenerId, charIdList);
											this.RefreshRope(ropeKeyList);
										}
										else
										{
											this._kidnapChars.Clear();
											this._kidnapRopes.Clear();
											this._kidnapScroll.UpdateData(0);
											this.RefreshKidnapCount();
											this.Element.ShowAfterRefresh();
										}
									}
									else
									{
										bool flag10 = notification.MethodId == 48;
										if (flag10)
										{
											Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._kidnapChars);
											this.RefreshKidnap();
										}
										else
										{
											bool flag11 = notification.MethodId == 45;
											if (flag11)
											{
												bool maxWorthInited = this._maxWorthCanBeLentToTaiwu >= 0L;
												Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._maxWorthCanBeLentToTaiwu);
												this._multiplyItemScrollView.MaxWorthCanBeLentToTaiwu = this._maxWorthCanBeLentToTaiwu;
												bool flag12 = maxWorthInited;
												if (flag12)
												{
													this._itemScroll.SetItemList(ref this._inventoryItems);
												}
											}
											else
											{
												bool flag13 = notification.MethodId == 138;
												if (flag13)
												{
													if (this._curCharLoveAndHateItemInfo == null)
													{
														this._curCharLoveAndHateItemInfo = new CharacterLoveAndHateItemInfo();
													}
													Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._curCharLoveAndHateItemInfo);
													bool flag14 = !this.TryShowHobbyUnlockEffect();
													if (flag14)
													{
														this.UpdateHobby();
													}
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
						bool flag15 = notification.DomainId == 6;
						if (flag15)
						{
							bool flag16 = notification.MethodId == 14 || notification.MethodId == 15;
							if (flag16)
							{
								ItemOperationType.EItemOperationType operateType = (notification.MethodId == 14) ? ItemOperationType.EItemOperationType.Repair : ItemOperationType.EItemOperationType.Disassemble;
								bool flag17 = this._currItemOperation != operateType;
								if (flag17)
								{
									break;
								}
								List<ItemKey> itemKeys = null;
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref itemKeys);
								bool needSaveFilter = this._canOperateItems.Count == 0;
								this._canOperateItems.Clear();
								if (itemKeys != null)
								{
									itemKeys.ForEach(delegate(ItemKey key)
									{
										ItemDisplayData result = this._inventoryItems.Find((ItemDisplayData data) => data.ContainsItemKey(key));
										bool flag26 = result != null;
										if (flag26)
										{
											this._canOperateItems.Add(result);
										}
									});
								}
								this._itemScroll.SetItemList(ref this._canOperateItems);
								bool flag18 = needSaveFilter;
								if (flag18)
								{
									this.RefreshSort();
								}
							}
						}
						else
						{
							bool flag19 = notification.DomainId == 5;
							if (flag19)
							{
								bool flag20 = notification.MethodId == 15;
								if (flag20)
								{
									this._warehouseItems.Clear();
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._warehouseItems);
								}
								else
								{
									bool flag21 = notification.MethodId == 64;
									if (flag21)
									{
										this._treasuryItems.Clear();
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._treasuryItems);
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
				bool flag22 = uid.DomainId == 4 && uid.DataId == 0;
				if (flag22)
				{
					bool flag23 = uid.SubId1 == 104U;
					if (flag23)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._curLoad);
						this.OnInventoryLoadChange();
					}
					else
					{
						bool flag24 = uid.SubId1 == 103U;
						if (flag24)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._maxLoad);
							this.OnInventoryLoadChange();
						}
						else
						{
							bool flag25 = uid.SubId1 == 66U;
							if (flag25)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._exp);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06001DA7 RID: 7591 RVA: 0x000D2DA0 File Offset: 0x000D0FA0
	protected override void OnClick(Transform btn)
	{
		string name = btn.name;
		string a = name;
		if (!(a == "ExchangeResource"))
		{
			if (a == "DebtEntrance")
			{
				this.OnClickDebtEntrance();
			}
		}
		else
		{
			this.ExchangeResource(base.CharacterMenu.CurCharacterId, true);
		}
	}

	// Token: 0x06001DA8 RID: 7592 RVA: 0x000D2DF1 File Offset: 0x000D0FF1
	private void OnClickDebtEntrance()
	{
	}

	// Token: 0x06001DA9 RID: 7593 RVA: 0x000D2DF4 File Offset: 0x000D0FF4
	private List<GroupedItemScrollView2.ItemGroup> GroupItems(List<ItemDisplayData> items)
	{
		bool isSorting = this._itemScroll.SortAndFilterController.SortAndFilterState.SortData.ItemStates.Count > 0;
		UI_CharacterMenuItems.<>c__DisplayClass72_0 CS$<>8__locals1;
		CS$<>8__locals1.groups = new List<GroupedItemScrollView2.ItemGroup>();
		int stateIndex = this._itemScroll.SortAndFilterController.SortAndFilterState.LineStates.FindLastIndex(delegate(LineState s)
		{
			bool result;
			if (s.IsActive)
			{
				ESortAndFilterOneLineType type = s.Type;
				result = (type == ESortAndFilterOneLineType.ToggleGroup || type == ESortAndFilterOneLineType.SingleSelectFilter);
			}
			else
			{
				result = false;
			}
			return result;
		});
		bool filterLevelOneAndTwo = stateIndex >= 0;
		bool noGroup = isSorting || !filterLevelOneAndTwo;
		bool flag = noGroup;
		List<GroupedItemScrollView2.ItemGroup> groups;
		if (flag)
		{
			CS$<>8__locals1.groups.Add(new GroupedItemScrollView2.ItemGroup
			{
				GroupId = -1,
				ItemList = items
			});
			groups = CS$<>8__locals1.groups;
		}
		else
		{
			bool flag2 = filterLevelOneAndTwo;
			if (flag2)
			{
				List<FilterLineBase<ItemDisplayData>> lineList = this._itemScroll.SortAndFilterController.FilterLines;
				FilterLineBase<ItemDisplayData> line = lineList[stateIndex];
				bool isLevelOne = line.Id == 0;
				HashSet<ItemDisplayData> sourceSet = new HashSet<ItemDisplayData>(items);
				bool flag3 = isLevelOne;
				if (flag3)
				{
					List<ItemDisplayData> resourceItemsToRemove = (from d in sourceSet
					where d.IsResource
					select d).ToList<ItemDisplayData>();
					UI_CharacterMenuItems.<GroupItems>g__AddGroup|72_1(0, LanguageKey.LK_CommonSortAndFilter_Filter_Misc_0.Tr(), resourceItemsToRemove, sourceSet, ref CS$<>8__locals1);
					List<FilterToggleConfig> toggleConfigs = line.GenerateConfig().ToggleGroupLineConfig.Config.FilterToggleConfigs;
					for (int index = 0; index < toggleConfigs.Count; index++)
					{
						FilterToggleConfig config = toggleConfigs[index];
						LineState childLineState = new LineState
						{
							IsActive = true,
							Type = ESortAndFilterOneLineType.ToggleGroup,
							ToggleGroupState = new ToggleKey
							{
								Index = index,
								IsAll = false
							}
						};
						List<ItemDisplayData> itemsToRemove = (from d in sourceSet
						where line.IsDataMatch(d, childLineState)
						select d).ToList<ItemDisplayData>();
						UI_CharacterMenuItems.<GroupItems>g__AddGroup|72_1(index, config.TipContent.GetString(), itemsToRemove, sourceSet, ref CS$<>8__locals1);
					}
				}
				else
				{
					SecondaryFilterLineBase<ItemDisplayData> secondaryFilterLine = line as SecondaryFilterLineBase<ItemDisplayData>;
					DetailedFilterMenuBase<ItemDisplayData> menuLine = (secondaryFilterLine != null) ? secondaryFilterLine.GetMenus().First<DetailedFilterMenuBase<ItemDisplayData>>() : null;
					List<DetailFilterMultiSelectDropdownItemConfig> menuConfigs = menuLine.GetMenuConfigs();
					List<int> selections = new List<int>();
					Func<ItemDisplayData, bool> <>9__4;
					for (int index2 = 0; index2 < menuConfigs.Count; index2++)
					{
						DetailFilterMultiSelectDropdownItemConfig config2 = menuConfigs[index2];
						selections.Clear();
						selections.Add(index2);
						IEnumerable<ItemDisplayData> source = sourceSet;
						Func<ItemDisplayData, bool> predicate;
						if ((predicate = <>9__4) == null)
						{
							predicate = (<>9__4 = ((ItemDisplayData d) => menuLine.IsDataMatch(d, selections)));
						}
						List<ItemDisplayData> itemsToRemove2 = source.Where(predicate).ToList<ItemDisplayData>();
						UI_CharacterMenuItems.<GroupItems>g__AddGroup|72_1(index2, config2.Text.GetString(), itemsToRemove2, sourceSet, ref CS$<>8__locals1);
					}
				}
			}
			groups = CS$<>8__locals1.groups;
		}
		return groups;
	}

	// Token: 0x06001DAA RID: 7594 RVA: 0x000D3120 File Offset: 0x000D1320
	private void InitResourceSprite()
	{
		this.HideResourceSprite();
		Refers template = this._choosyPage.CGet<Refers>("Template");
		for (int i = 0; i < template.transform.childCount; i++)
		{
			Transform child = template.transform.GetChild(i);
			child.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001DAB RID: 7595 RVA: 0x000D317C File Offset: 0x000D137C
	private void HideResourceSprite()
	{
		GameObject pool = this._choosyPage.CGet<GameObject>("Pool");
		for (int i = 0; i < pool.transform.childCount; i++)
		{
			Transform child = pool.transform.GetChild(i);
			child.DOKill(false);
			bool activeSelf = child.gameObject.activeSelf;
			if (activeSelf)
			{
				child.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06001DAC RID: 7596 RVA: 0x000D31EC File Offset: 0x000D13EC
	private void DestroyResourceSprite()
	{
		GameObject pool = this._choosyPage.CGet<GameObject>("Pool");
		for (int i = 0; i < pool.transform.childCount; i++)
		{
			Transform child = pool.transform.GetChild(i);
			Object.Destroy(child.gameObject);
		}
		this._makeResourceSpriteDict.Clear();
	}

	// Token: 0x06001DAD RID: 7597 RVA: 0x000D324C File Offset: 0x000D144C
	private void ShowResourceSprite(string resourceName, sbyte type, int curCount, bool isMore, bool needFast)
	{
		GameObject pool = this._choosyPage.CGet<GameObject>("Pool");
		Refers template = this._choosyPage.CGet<Refers>("Template");
		GameObject birthPoints = this._choosyPage.CGet<GameObject>("BirthPoints");
		Vector3 min = birthPoints.transform.GetChild(0).position;
		Vector3 max = birthPoints.transform.GetChild(1).position;
		float x = Random.Range(min.x, max.x);
		float y = Random.Range(min.y, max.y);
		Vector3 pos = min.SetX(x).SetY(y);
		List<GameObject> list;
		bool flag = !this._makeResourceSpriteDict.TryGetValue(resourceName, out list);
		if (flag)
		{
			list = new List<GameObject>();
			this._makeResourceSpriteDict[resourceName] = list;
		}
		if (isMore)
		{
			GameObject templateGo = template.CGet<GameObject>(resourceName);
			bool flag2 = curCount > list.Count && curCount <= 100;
			if (flag2)
			{
				GameObject go = Object.Instantiate<GameObject>(templateGo, pos, Quaternion.identity, pool.transform);
				go.SetActive(true);
				string spriteName = CommonUtils.GetResourceSpriteName(type, true);
				AtlasInfo.Instance.GetSprite(spriteName, delegate(Sprite sprite)
				{
					go.GetComponent<SpriteRenderer>().sprite = sprite;
				});
				list.Add(go);
			}
			else
			{
				bool flag3 = !needFast || curCount <= 100;
				if (flag3)
				{
					int index = Math.Max(0, curCount % 100 - 1);
					GameObject go3 = list.GetOrDefault(index);
					bool flag4 = go3;
					if (flag4)
					{
						go3.transform.SetPositionAndRotation(pos, Quaternion.identity);
						SpriteRenderer spriteRenderer = go3.GetComponent<SpriteRenderer>();
						spriteRenderer.color.SetAlpha(1f);
						go3.SetActive(true);
					}
				}
			}
		}
		else
		{
			bool flag5 = curCount < 100;
			if (flag5)
			{
				GameObject go2 = list.GetOrDefault(curCount);
				if (go2 != null)
				{
					go2.SetActive(false);
				}
			}
		}
	}

	// Token: 0x06001DAE RID: 7598 RVA: 0x000D3454 File Offset: 0x000D1654
	private void OnResourceChange(sbyte resourceType)
	{
		bool flag;
		if (this.CurCharacterIsTaiwu)
		{
			ItemOperationType.EItemOperationType currItemOperation = this._currItemOperation;
			flag = (currItemOperation != ItemOperationType.EItemOperationType.Repair && currItemOperation != ItemOperationType.EItemOperationType.Disassemble);
		}
		else
		{
			flag = false;
		}
		bool flag2 = flag;
		if (flag2)
		{
			this.CallRefreshItems(null);
		}
		bool isShowing = UIElement.ExchangeResource.IsShowing;
		if (isShowing)
		{
			this.CallRefreshItems(null);
		}
	}

	// Token: 0x06001DAF RID: 7599 RVA: 0x000D34A4 File Offset: 0x000D16A4
	public override bool CanItemDropToCharacter(int characterId, object data)
	{
		return characterId != base.CharacterMenu.CurCharacterId;
	}

	// Token: 0x06001DB0 RID: 7600 RVA: 0x000D34C8 File Offset: 0x000D16C8
	private void OnInventoryLoadChange()
	{
		string currLoad = ((float)this._curLoad / 100f).ToString("f1");
		string maxLoad = ((float)this._maxLoad / 100f).ToString("f1");
		this._itemPage.CGet<TextMeshProUGUI>("LoadText").text = currLoad + "/" + maxLoad;
		this._itemPage.CGet<TooltipInvoker>("LoadOverflowTips").enabled = (this.IsTaiwuTeamButNotBeast && this._curLoad > this._maxLoad);
		this.RefreshOverloadMouseTips();
	}

	// Token: 0x06001DB1 RID: 7601 RVA: 0x000D3564 File Offset: 0x000D1764
	private bool CheckItemCanTransfer(ItemDisplayData itemData)
	{
		bool isChildCloth = itemData.UsingType == ItemDisplayData.ItemUsingType.Equiped && this._ageMonitor.DisplayAge < 16 && ItemTemplateHelper.GetEquipmentType(itemData.Key.ItemType, itemData.Key.TemplateId) == 2;
		bool isTransferable = ItemTemplateHelper.IsTransferable(itemData.Key.ItemType, itemData.Key.TemplateId) && !isChildCloth;
		bool miscResourceCanExchange = ItemTemplateHelper.MiscResourceCanExchange(itemData.Key.ItemType, itemData.Key.TemplateId);
		bool canTransfer = isTransferable || miscResourceCanExchange;
		return canTransfer && !ItemDisplayDataHelper.IsItemLockedByTask(itemData);
	}

	// Token: 0x06001DB2 RID: 7602 RVA: 0x000D3608 File Offset: 0x000D1808
	private bool CheckItemIsLocked(ItemDisplayData itemData)
	{
		bool flag = !base.CharacterMenu.CanOperate;
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			bool isBaby = this._currentCharacterDisplayData != null && AgeGroup.GetAgeGroup(this._currentCharacterDisplayData.PhysiologicalAge) == 0;
			bool flag2 = isBaby;
			if (flag2)
			{
				result = true;
			}
			else
			{
				bool isMultiItemSelect = this._multiplyItemScrollView.IsMultiItemSelect;
				if (isMultiItemSelect)
				{
					bool flag3 = this._multiplyItemScrollView.SelectedMultiplyItemDict.ContainsKey(itemData);
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

	// Token: 0x06001DB3 RID: 7603 RVA: 0x000D36F8 File Offset: 0x000D18F8
	private bool CheckSpecialItemCanInteract(ItemDisplayData itemData)
	{
		bool isHeavenlyTreeSeeds = ItemTemplateHelper.CheckIsHeavenlyTreeSeeds(itemData.Key.ItemType, itemData.Key.TemplateId);
		bool isSectMainStoryItemXuannvNotes = ItemTemplateHelper.CheckIsSectMainStoryItemXuannvNotes(itemData.Key.ItemType, itemData.Key.TemplateId);
		bool isSectMainStoryItemWuxianWugFairy = ItemTemplateHelper.CheckIsSectMainStoryItemWuxianWugFairy(itemData.Key.ItemType, itemData.Key.TemplateId);
		bool isSectMainStoryFulongChickenMap = ItemTemplateHelper.CheckIsSectMainStoryFulongChickenMap(itemData.Key.ItemType, itemData.Key.TemplateId);
		bool isSectMainStoryItemYuanshanRosary = ItemTemplateHelper.CheckIsSectMainStoryItemYuanshanRosary(itemData.Key.ItemType, itemData.Key.TemplateId);
		bool isSectMainStoryItemJieqingStars = ItemTemplateHelper.CheckIsSectMainStoryItemJieQingStars(itemData.Key.ItemType, itemData.Key.TemplateId);
		bool isThanksLetter = ItemTemplateHelper.IsThanksLetter(itemData.Key.ItemType, itemData.Key.TemplateId);
		return isHeavenlyTreeSeeds || isSectMainStoryItemXuannvNotes || isSectMainStoryItemWuxianWugFairy || isSectMainStoryFulongChickenMap || isSectMainStoryItemYuanshanRosary || isThanksLetter || isSectMainStoryItemJieqingStars;
	}

	// Token: 0x06001DB4 RID: 7604 RVA: 0x000D37E4 File Offset: 0x000D19E4
	private void OnRenderItem(ItemDisplayData itemData, CommonTableRowForItem itemView)
	{
		bool isLock = this.CheckItemIsLocked(itemData);
		itemView.IsLocked = isLock;
		this.SetResourceItemTip(itemView);
		bool isMultiItemSelect = this._multiplyItemScrollView.IsMultiItemSelect;
		if (isMultiItemSelect)
		{
			this._multiplyItemScrollView.OnRenderItemMultiply(itemData, itemView);
		}
		else
		{
			this.OnRenderItemSingle(itemData, itemView);
		}
	}

	// Token: 0x06001DB5 RID: 7605 RVA: 0x000D3834 File Offset: 0x000D1A34
	public static ResourceInts GetDisassembleResources(ItemDisplayData itemDisplayData, int amount)
	{
		ItemKey itemKey = itemDisplayData.Key;
		bool flag = ItemType.IsEquipmentItemType(itemKey.ItemType);
		ResourceInts resourceInts;
		if (flag)
		{
			resourceInts = ItemTemplateHelper.GetDisassembleResources(itemDisplayData.MaterialResources, itemKey.ItemType, itemKey.TemplateId, amount);
		}
		else
		{
			bool flag2 = itemKey.ItemType == 12;
			if (flag2)
			{
				MiscItem miscConfig = Misc.Instance[itemKey.TemplateId];
				MakeItemSubTypeItem makeConfig = MakeItemSubType.Instance[miscConfig.MakeItemSubType];
				bool flag3 = makeConfig == null;
				MaterialResources presetResources;
				if (flag3)
				{
					presetResources = default(MaterialResources);
				}
				else
				{
					presetResources = makeConfig.MaxMaterialResources;
				}
				resourceInts = ItemTemplateHelper.GetDisassembleResources(presetResources, itemKey.ItemType, itemKey.TemplateId, amount);
			}
			else
			{
				resourceInts = ItemTemplateHelper.GetDisassembleResources(default(MaterialResources), itemKey.ItemType, itemKey.TemplateId, amount);
			}
		}
		return resourceInts;
	}

	// Token: 0x06001DB6 RID: 7606 RVA: 0x000D390C File Offset: 0x000D1B0C
	private void OnRenderItemSingle(ItemDisplayData itemData, CommonTableRowForItem itemView)
	{
		bool isLocked = itemView.IsLocked;
		if (isLocked)
		{
			itemView.ShowInteractionStateLocked();
		}
		else
		{
			itemView.HideInteractionState();
		}
		itemView.SetClickEvent(delegate
		{
			this.OnClickItem(itemData, itemView);
		});
		bool currentCharacterIsTaiwu = base.CharacterMenu.CurrentCharacterIsTaiwu;
		if (currentCharacterIsTaiwu)
		{
			itemView.SetFavoriteStatus(CommonTableRowForItem.FavoriteStatus.None);
		}
		else
		{
			bool flag = this._curCharLoveAndHateItemInfo != null;
			if (flag)
			{
				itemView.SetFavoriteStatus(this._curCharLoveAndHateItemInfo.LovingItemSubType, this._curCharLoveAndHateItemInfo.HatingItemSubType);
			}
		}
	}

	// Token: 0x06001DB7 RID: 7607 RVA: 0x000D39C8 File Offset: 0x000D1BC8
	private void SetResourceItemTip(CommonTableRowForItem itemView)
	{
		bool flag = this._currentCharacterDisplayData == null || !itemView.Data.IsResource;
		if (!flag)
		{
			string charName = NameCenter.GetMonasticTitleOrDisplayName(this._currentCharacterDisplayData, SingletonObject.getInstance<BasicGameData>().TaiwuCharId == this._currentCharacterDisplayData.CharacterId);
			itemView.SetResourceTip(charName, this.CurCharacterIsTaiwu);
		}
	}

	// Token: 0x06001DB8 RID: 7608 RVA: 0x000D3A28 File Offset: 0x000D1C28
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

	// Token: 0x06001DB9 RID: 7609 RVA: 0x000D3A9C File Offset: 0x000D1C9C
	public static bool CheckCurrentToolAttainment(ItemOperationType.EItemOperationType operationType, ItemDisplayData tool, ItemDisplayData itemData, LifeSkillShorts lifeSkillAttainments, out sbyte lifeSkillType, out short requiredAttainment)
	{
		lifeSkillType = ItemTemplateHelper.GetCraftRequiredLifeSkillType(itemData.Key.ItemType, itemData.Key.TemplateId);
		List<sbyte> toolRequiredLifeSkillTypes = CraftTool.Instance[tool.Key.TemplateId].RequiredLifeSkillTypes;
		if (!true)
		{
		}
		short num;
		if (operationType != ItemOperationType.EItemOperationType.Repair)
		{
			if (operationType != ItemOperationType.EItemOperationType.Disassemble)
			{
				throw new ArgumentOutOfRangeException();
			}
			num = ItemTemplateHelper.GetDisassembleRequiredAttainment(itemData.Key.ItemType, itemData.Key.TemplateId);
		}
		else
		{
			num = ItemTemplateHelper.GetRepairRequiredAttainment(itemData.Key.ItemType, itemData.Key.TemplateId, itemData.Durability);
		}
		if (!true)
		{
		}
		requiredAttainment = num;
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

	// Token: 0x06001DBA RID: 7610 RVA: 0x000D3B88 File Offset: 0x000D1D88
	private void OnDragStart(object dragObj)
	{
		ItemDisplayData itemData = dragObj as ItemDisplayData;
		bool flag = this.CurCharacterIsTaiwu && CommonUtils.CanItemEat(itemData.Key.ItemType, itemData.Key.TemplateId);
		if (flag)
		{
			base.CharacterMenu.SetBaseAttributeState(1);
		}
		bool flag2 = itemData.Key.ItemType == 11;
		if (flag2)
		{
			UIElement.DragShow.OnShowed = delegate()
			{
				CricketView cricket = UIElement.DragShow.UiBaseAs<UI_DragShow>().DragItem.GetComponentInChildren<CricketView>();
				cricket.Inited = false;
				cricket.SetCricketData(itemData.CricketColorId, itemData.CricketPartId, false, null, false);
			};
		}
	}

	// Token: 0x06001DBB RID: 7611 RVA: 0x000D3C1C File Offset: 0x000D1E1C
	private void RefreshCharListAndMaxWorth(int charId)
	{
		base.CharacterMenu.RerenderCharacterScroll();
		bool flag = charId >= 0;
		if (flag)
		{
			SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<BasicInfoMonitor>(base.CharacterMenu.IsTaiwu(charId) ? base.CharacterMenu.CurCharacterId : charId, false).Refresh();
			bool currentCharacterIsTaiwuTeammate = base.CharacterMenu.CurrentCharacterIsTaiwuTeammate;
			if (currentCharacterIsTaiwuTeammate)
			{
				CharacterDomainMethod.Call.GetMaxWorthCanBeLentToTaiwu(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
			}
		}
	}

	// Token: 0x06001DBC RID: 7612 RVA: 0x000D3C9C File Offset: 0x000D1E9C
	public override void OnEnterFocusMode(Transform maskTrans)
	{
		bool flag = this._currItemOperation == ItemOperationType.EItemOperationType.Disassemble || this._currItemOperation == ItemOperationType.EItemOperationType.Repair;
		if (flag)
		{
			this._itemPage.transform.SetParent(maskTrans, true);
		}
	}

	// Token: 0x06001DBD RID: 7613 RVA: 0x000D3CD8 File Offset: 0x000D1ED8
	public override void OnExitFocusMode()
	{
		this._selectedToTransferItemData = null;
		bool flag = this._currItemOperation == ItemOperationType.EItemOperationType.Disassemble || this._currItemOperation == ItemOperationType.EItemOperationType.Repair;
		if (flag)
		{
			this._currItemOperation = ItemOperationType.EItemOperationType.Invalid;
			this.RefreshSort();
			this._itemScroll.SetItemList(ref this._inventoryItems);
		}
		else
		{
			this._itemScroll.ReRender();
		}
		this._currItemOperation = ItemOperationType.EItemOperationType.Invalid;
		this._canOperateItems.Clear();
		this.AddDebtMouseTipInfo();
		this.CancelHighLightItemView();
		this._multiplyItemScrollView.ShowMultiplySelectButton(null, true);
	}

	// Token: 0x06001DBE RID: 7614 RVA: 0x000D3D67 File Offset: 0x000D1F67
	private void TransferMultiplyItems(int characterId)
	{
	}

	// Token: 0x06001DBF RID: 7615 RVA: 0x000D3D6C File Offset: 0x000D1F6C
	private void CallRefreshItems(ArgumentBox args = null)
	{
		CharacterDomainMethod.Call.GetAllInventoryItems(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
		CharacterDomainMethod.Call.GetAllEquipmentItems(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
		TaiwuDomainMethod.Call.GetAllWarehouseItems(this.Element.GameDataListenerId);
		TaiwuDomainMethod.Call.GetAllTreasuryItems(this.Element.GameDataListenerId);
	}

	// Token: 0x06001DC0 RID: 7616 RVA: 0x000D3DD4 File Offset: 0x000D1FD4
	private void EnterItemOperation()
	{
		this._multiplyItemScrollView.HideMultiplySelectButton();
		base.CharacterMenu.EnterFocusMode(null, true, false);
		base.CharacterMenu.OnTryClosePage = delegate()
		{
			this.ExitItemOperation(false);
		};
	}

	// Token: 0x06001DC1 RID: 7617 RVA: 0x000D3E0C File Offset: 0x000D200C
	private void ExitItemOperation(bool needRefresh = false)
	{
		this._multiplyItemScrollView.ShowMultiplySelectButton(null, true);
		base.CharacterMenu.ExitFocusMode(true);
		base.CharacterMenu.OnTryClosePage = null;
		if (needRefresh)
		{
			this.CallRefreshItems(null);
		}
	}

	// Token: 0x06001DC2 RID: 7618 RVA: 0x000D3E4E File Offset: 0x000D204E
	private void SetCursorForOperation()
	{
		ConchShipCursor.Instance.SetCursorImage("sp_cursor_clickable_chuizi", -1f, -1f);
		ConchShipCursor.Instance.CanChange = false;
	}

	// Token: 0x06001DC3 RID: 7619 RVA: 0x000D3E76 File Offset: 0x000D2076
	private void ResetCursor()
	{
		ConchShipCursor.Instance.CanChange = true;
		ConchShipCursor.Instance.SetDefaultCursor();
	}

	// Token: 0x06001DC4 RID: 7620 RVA: 0x000D3E90 File Offset: 0x000D2090
	private void HighLightItemView(CommonTableRowForItem itemView)
	{
		bool flag = null == itemView;
		if (!flag)
		{
			this._focusingTuple.Item1 = itemView;
			this._focusingTuple.Item2 = itemView.transform.parent;
			this._focusingTuple.Item3 = itemView.transform.GetSiblingIndex();
			RectTransform focusMask = this._itemScroll.CGet<RectTransform>("FocusItemMask");
			focusMask.gameObject.SetActive(true);
			itemView.transform.SetParent(focusMask, true);
			itemView.transform.localScale = Vector3.one;
		}
	}

	// Token: 0x06001DC5 RID: 7621 RVA: 0x000D3F20 File Offset: 0x000D2120
	private void CancelHighLightItemView()
	{
		this.SetItemScrollViewCanScroll(true);
		bool flag = null != this._focusingTuple.Item1;
		if (flag)
		{
			this._focusingTuple.Item1.transform.SetParent(this._focusingTuple.Item2, true);
			this._focusingTuple.Item1.transform.SetSiblingIndex(this._focusingTuple.Item3);
			this._focusingTuple.Item1 = null;
			this._itemScroll.CGet<RectTransform>("FocusItemMask").gameObject.SetActive(false);
		}
		this._itemPage.transform.SetParent(this._itemPageParent, true);
		base.CharacterMenu.Injury.HideNotice(true, true);
	}

	// Token: 0x06001DC6 RID: 7622 RVA: 0x000D3FE3 File Offset: 0x000D21E3
	private void SetItemScrollViewCanScroll(bool canScroll)
	{
		this._itemScroll.ItemScrollView.LoopScroll.enabled = canScroll;
	}

	// Token: 0x06001DC7 RID: 7623 RVA: 0x000D4000 File Offset: 0x000D2200
	private void OnClickItem(ItemDisplayData itemData, CommonTableRowForItem itemView)
	{
		UI_CharacterMenuItems.<>c__DisplayClass102_0 CS$<>8__locals1 = new UI_CharacterMenuItems.<>c__DisplayClass102_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.itemData = itemData;
		bool flag = !base.CharacterMenu.CanOperate;
		if (!flag)
		{
			this._itemScroll.HandleClickItem(CS$<>8__locals1.itemData, itemView, new Action<CommonTableRowForItem>(CS$<>8__locals1.<OnClickItem>g__Action|0));
		}
	}

	// Token: 0x06001DC8 RID: 7624 RVA: 0x000D4058 File Offset: 0x000D2258
	private void ShowItemOperateMenu(ItemDisplayData itemData, CommonTableRowForItem itemView)
	{
		UI_CharacterMenuItems.<>c__DisplayClass103_0 CS$<>8__locals1 = new UI_CharacterMenuItems.<>c__DisplayClass103_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.itemView = itemView;
		CS$<>8__locals1.itemData = itemData;
		bool selectingTransferItemChar = this.SelectingTransferItemChar;
		if (selectingTransferItemChar)
		{
			base.CharacterMenu.ExitFocusMode(true);
		}
		bool canTransfer = this.CheckItemCanTransfer(CS$<>8__locals1.itemData);
		CS$<>8__locals1.btnList = new List<ViewPopupMenu.BtnData>();
		Queue<InventoryItemOperationType> optionQueue = new Queue<InventoryItemOperationType>();
		bool isMiscResource = ItemTemplateHelper.IsMiscResource(CS$<>8__locals1.itemData.Key.ItemType, CS$<>8__locals1.itemData.Key.TemplateId);
		bool canExchange = ItemTemplateHelper.MiscResourceCanExchange(CS$<>8__locals1.itemData.Key.ItemType, CS$<>8__locals1.itemData.Key.TemplateId);
		bool canOperate = canTransfer && CS$<>8__locals1.itemData.Amount > 0 && (!isMiscResource || canExchange);
		bool curCharacterIsTaiwu = this.CurCharacterIsTaiwu;
		if (curCharacterIsTaiwu)
		{
			this.ShowItemOperateMenuSect(CS$<>8__locals1.btnList, CS$<>8__locals1.itemData);
			this.ShowItemOperateMenuCommonEvent(CS$<>8__locals1.btnList, CS$<>8__locals1.itemData);
			this.ShowItemOperateMenuFeed(CS$<>8__locals1.btnList, CS$<>8__locals1.itemData, CS$<>8__locals1.itemView);
			this.ShowItemOperateMenuIdentify(CS$<>8__locals1.btnList, CS$<>8__locals1.itemData, CS$<>8__locals1.itemView);
			this.ShowAddAppointment(CS$<>8__locals1.btnList, CS$<>8__locals1.itemData, CS$<>8__locals1.itemView);
			this.ShowItemOperateMenuTool(CS$<>8__locals1.btnList, CS$<>8__locals1.itemData, CS$<>8__locals1.itemView);
			bool flag = canOperate;
			if (flag)
			{
				this.ShowItemOperateMenuGive(CS$<>8__locals1.btnList, CS$<>8__locals1.itemData);
				this.ShowItemOperateMenuChoosy(CS$<>8__locals1.btnList, CS$<>8__locals1.itemData);
				this.ShowItemOperateMenuDiscard(CS$<>8__locals1.btnList, CS$<>8__locals1.itemData);
				bool flag2 = !isMiscResource;
				if (flag2)
				{
					this.ShowProfessionMenuSelf(CS$<>8__locals1.btnList, CS$<>8__locals1.itemData, new Action(CS$<>8__locals1.<ShowItemOperateMenu>g__Action|0), optionQueue);
				}
			}
		}
		else
		{
			bool flag3 = canOperate && !base.CharacterMenu.IsTaiwuSpecialTeammate(base.CharacterMenu.CurCharacterId);
			if (flag3)
			{
				this.ShowItemOperateMenuEnemy(CS$<>8__locals1.btnList, CS$<>8__locals1.itemData);
				bool flag4 = !isMiscResource;
				if (flag4)
				{
					this.ShowProfessionMenuOther(CS$<>8__locals1.btnList, CS$<>8__locals1.itemData, new Action(CS$<>8__locals1.<ShowItemOperateMenu>g__Action|0), optionQueue);
				}
			}
		}
		bool isTaiwuTeamButNotBeast = this.IsTaiwuTeamButNotBeast;
		if (isTaiwuTeamButNotBeast)
		{
			this.ShowItemOperateMenuEat(CS$<>8__locals1.btnList, CS$<>8__locals1.itemData);
			bool flag5 = !this.CurCharacterIsTaiwu;
			if (flag5)
			{
				bool flag6 = canOperate;
				if (flag6)
				{
					this.ShowItemOperateMenuTake(CS$<>8__locals1.btnList, CS$<>8__locals1.itemData);
					bool flag7 = !base.CharacterMenu.IsTaiwuGearMate(base.CharacterMenu.CurCharacterId);
					if (flag7)
					{
						this.ShowItemOperateMenuExchange(CS$<>8__locals1.btnList, CS$<>8__locals1.itemData);
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

	// Token: 0x17000306 RID: 774
	// (get) Token: 0x06001DC9 RID: 7625 RVA: 0x000D432C File Offset: 0x000D252C
	private bool IsTaiwuTeamButNotBeast
	{
		get
		{
			return base.CharacterMenu.IsTaiwuTeam && !base.CharacterMenu.IsTaiwuBeastTeammate(base.CharacterMenu.CurCharacterId);
		}
	}

	// Token: 0x06001DCA RID: 7626 RVA: 0x000D4358 File Offset: 0x000D2558
	private void ShowItemOperateMenuCommonEvent(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData)
	{
		ItemKey itemKey = itemData.Key;
		bool flag = itemKey.ItemType != 12 || !Misc.Instance[itemKey.TemplateId].CanTriggerCommonEvent;
		if (!flag)
		{
			string btnName = LocalStringManager.Get(LanguageKey.LK_Option_On);
			ViewPopupMenu.BtnData btnOn = new ViewPopupMenu.BtnData(btnName, true, EItemMenuDisplayOrder.Use, delegate()
			{
				this.OnClickUseCommonEvent(itemData);
			}, null, null, false);
			btnList.Add(btnOn);
		}
	}

	// Token: 0x06001DCB RID: 7627 RVA: 0x000D43E0 File Offset: 0x000D25E0
	private void ShowItemOperateMenuSect(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData)
	{
		WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
		bool isHeavenlyTreeSeeds = ItemTemplateHelper.CheckIsHeavenlyTreeSeeds(itemData.Key.ItemType, itemData.Key.TemplateId);
		bool flag = isHeavenlyTreeSeeds;
		if (flag)
		{
			bool currentBlockIsFitHeavenlyTree = mapModel.IsCurrentBlockMeetTypeForHeavenlyTree();
			bool isBlockAwayFromHeavenlyTree = mapModel.IsCurrentBlockMeetDistanceForHeavenTree(6);
			bool interactable = currentBlockIsFitHeavenlyTree && isBlockAwayFromHeavenlyTree;
			string btnName = LocalStringManager.Get(LanguageKey.LK_Plant);
			ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(btnName, interactable, EItemMenuDisplayOrder.Sect, delegate()
			{
				this.CharacterMenu.ExitFocusMode(true);
				UIManager.Instance.HideUI(UIElement.CharacterMenu);
				TaiwuEventDomainMethod.Call.CloseUI("WudangHeavenlyTreeSeed", false, (int)itemData.Key.TemplateId);
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
				ViewPopupMenu.BtnData btnOn = new ViewPopupMenu.BtnData(btnName2, !isEnabled, EItemMenuDisplayOrder.Sect, new Action(this.OnClickSectMainStoryItemXuannvNotes), null, null, false);
				btnList.Add(btnOn);
				btnName2 = LocalStringManager.Get(LanguageKey.LK_Option_Off);
				ViewPopupMenu.BtnData btnOff = new ViewPopupMenu.BtnData(btnName2, isEnabled, EItemMenuDisplayOrder.Sect, new Action(this.OnClickSectMainStoryItemXuannvNotes), null, null, false);
				btnList.Add(btnOff);
			}
			else
			{
				bool canOpenMakeWugKing = ItemTemplateHelper.CheckIsSectMainStoryItemWuxianWugFairy(itemData.Key.ItemType, itemData.Key.TemplateId);
				bool flag5 = canOpenMakeWugKing;
				if (flag5)
				{
					string btnName3 = LocalStringManager.Get(LanguageKey.LK_Option_On);
					ViewPopupMenu.BtnData btnOn2 = new ViewPopupMenu.BtnData(btnName3, true, EItemMenuDisplayOrder.Sect, new Action(this.OnClickSectMainStoryItemWuxianWugFairy), null, null, false);
					btnList.Add(btnOn2);
				}
				else
				{
					bool findChicken = ItemTemplateHelper.CheckIsSectMainStoryFulongChickenMap(itemData.Key.ItemType, itemData.Key.TemplateId);
					bool flag6 = findChicken;
					if (flag6)
					{
						string btnName4 = LocalStringManager.Get(LanguageKey.LK_Option_FindChicken);
						ViewPopupMenu.BtnData btnOn3 = new ViewPopupMenu.BtnData(btnName4, this.ChickenMapInteractable && !this._allChickenInTaiwuVillage, EItemMenuDisplayOrder.Sect, new Action(this.OnClickSectMainStoryItemFulongChickenMap), null, null, false);
						bool allChickenInTaiwuVillage = this._allChickenInTaiwuVillage;
						if (allChickenInTaiwuVillage)
						{
							btnOn3.SetTip("", LocalStringManager.Get(LanguageKey.LK_Option_AllChickenGetTips));
						}
						else
						{
							bool flag7 = !this.ChickenMapInteractable;
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
						bool canOpenThreeVitals = ItemTemplateHelper.CheckIsSectMainStoryItemYuanshanRosary(itemData.Key.ItemType, itemData.Key.TemplateId);
						bool flag8 = canOpenThreeVitals;
						if (flag8)
						{
							string btnName5 = LocalStringManager.Get(LanguageKey.LK_Option_On);
							ViewPopupMenu.BtnData btnOn4 = new ViewPopupMenu.BtnData(btnName5, true, EItemMenuDisplayOrder.Sect, new Action(this.OnClickSectMainStoryItemYuanshanRosary), null, null, false);
							btnList.Add(btnOn4);
						}
						else
						{
							bool isThanksLetter = ItemTemplateHelper.IsThanksLetter(itemData.Key.ItemType, itemData.Key.TemplateId);
							bool flag9 = isThanksLetter;
							if (flag9)
							{
								string btnName6 = LocalStringManager.Get(LanguageKey.LK_Use_TianSuiBaoLuItem);
								ViewPopupMenu.BtnData btnOn5 = new ViewPopupMenu.BtnData(btnName6, true, EItemMenuDisplayOrder.Other, delegate()
								{
									this.OnClickThanksLetter(itemData);
								}, null, null, false);
								btnList.Add(btnOn5);
							}
							else
							{
								bool canOpenQiwenxingdou = ItemTemplateHelper.CheckIsSectMainStoryItemJieQingStars(itemData.Key.ItemType, itemData.Key.TemplateId);
								bool flag10 = canOpenQiwenxingdou;
								if (flag10)
								{
									string btnName7 = LocalStringManager.Get(LanguageKey.LK_Option_On);
									ViewPopupMenu.BtnData btnOn6 = new ViewPopupMenu.BtnData(btnName7, true, EItemMenuDisplayOrder.Sect, new Action(this.OnClickSectMainStoryItemJieQingStars), null, null, false);
									btnList.Add(btnOn6);
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06001DCC RID: 7628 RVA: 0x000D481C File Offset: 0x000D2A1C
	private void ShowItemOperateMenuFeed(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData, CommonTableRowForItem itemView)
	{
		bool flag = ItemTemplateHelper.IsFeedingAble(itemData.Key.ItemType, itemData.Key.TemplateId);
		if (flag)
		{
			bool interactable = this._inventoryItems.Any(new Func<ItemDisplayData, bool>(UI_CharacterMenuItems.CheckNeedFeedCarrier));
			string btnName = LocalStringManager.Get(LanguageKey.LK_Feeding_Item);
			ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(btnName, interactable, EItemMenuDisplayOrder.Feed, delegate()
			{
				this.OnClickItemEnterMultiplyMode(itemView, ItemOperationType.EItemOperationType.Feeding, null);
			}, null, null, false);
			btnList.Add(btnData);
			bool flag2 = !interactable;
			if (flag2)
			{
				LanguageKey key = LanguageKey.LK_Feeding_Item_Tip_CarrierNotMeet;
				btnData.SetTip("", LocalStringManager.Get(key));
			}
		}
		else
		{
			bool flag3 = ItemTemplateHelper.CanFeedCarrier(itemData.Key.ItemType, itemData.Key.TemplateId);
			if (flag3)
			{
				bool needFeed = UI_CharacterMenuItems.CheckNeedFeedCarrier(itemData);
				bool hasFood = this._inventoryItems.Exists((ItemDisplayData d) => ItemTemplateHelper.IsFeedingAble(d.Key.ItemType, d.Key.TemplateId));
				bool interactable2 = needFeed && hasFood;
				string btnName2 = LocalStringManager.Get(LanguageKey.LK_Feeding_Item);
				ViewPopupMenu.BtnData btnData2 = new ViewPopupMenu.BtnData(btnName2, interactable2, EItemMenuDisplayOrder.Feed, delegate()
				{
					this.OnClickItemEnterMultiplyMode(null, ItemOperationType.EItemOperationType.Feeding, itemData);
				}, null, null, false);
				btnList.Add(btnData2);
				bool flag4 = !interactable2;
				if (flag4)
				{
					LanguageKey key2 = needFeed ? LanguageKey.LK_Feeding_Item_Tip_MaterialNotMeet : LanguageKey.LK_Feeding_Item_Tip_NoNeed;
					btnData2.SetTip("", LocalStringManager.Get(key2));
				}
			}
		}
	}

	// Token: 0x06001DCD RID: 7629 RVA: 0x000D49B4 File Offset: 0x000D2BB4
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

	// Token: 0x06001DCE RID: 7630 RVA: 0x000D4A40 File Offset: 0x000D2C40
	private void ShowItemOperateMenuIdentify(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData, CommonTableRowForItem itemView)
	{
		bool flag = !ItemTemplateHelper.IsPoisonable(itemData.Key.ItemType, itemData.Key.TemplateId);
		if (!flag)
		{
			ItemDisplayData data = this._inventoryItems.Find(new Predicate<ItemDisplayData>(UI_CharacterMenuItems.<ShowItemOperateMenuIdentify>g__Match|111_1));
			int needleAmount = (data != null) ? data.Amount : 0;
			bool isInDoor = SingletonObject.getInstance<WorldMapModel>().GetTaiwuCharOnSettlement() > 0;
			bool flag2 = isInDoor;
			if (flag2)
			{
				data = this._warehouseItems.Find(new Predicate<ItemDisplayData>(UI_CharacterMenuItems.<ShowItemOperateMenuIdentify>g__Match|111_1));
				needleAmount += ((data != null) ? data.Amount : 0);
				data = this._treasuryItems.Find(new Predicate<ItemDisplayData>(UI_CharacterMenuItems.<ShowItemOperateMenuIdentify>g__Match|111_1));
				needleAmount += ((data != null) ? data.Amount : 0);
			}
			string btnName = LocalStringManager.GetFormat(LanguageKey.LK_Item_Identify_Poison, needleAmount);
			bool flag3 = itemData.PoisonEffects != null && itemData.PoisonEffects.IsIdentified;
			bool isInteractable;
			string content;
			if (flag3)
			{
				isInteractable = false;
				content = LocalStringManager.Get(LanguageKey.LK_Poison_Identified);
			}
			else
			{
				int leftDays = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
				bool timeIsMeet = leftDays >= 1;
				bool itemIsMeet = needleAmount >= 1;
				isInteractable = (timeIsMeet && itemIsMeet);
				string timeStr = leftDays.ToString().SetColor(timeIsMeet ? "brightblue" : "brightred");
				string itemStr = needleAmount.ToString().SetColor(itemIsMeet ? "brightblue" : "brightred");
				content = LocalStringManager.GetFormat(LanguageKey.LK_Poison_Identify_Tip, itemStr, timeStr);
			}
			string title = LocalStringManager.Get(LanguageKey.LK_Poison_Identify);
			ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(btnName, isInteractable, EItemMenuDisplayOrder.Identify, delegate()
			{
				this.OnClickIdentifyItem(itemView);
			}, null, null, false);
			btnData.SetTip(title, content);
			btnList.Add(btnData);
		}
	}

	// Token: 0x06001DCF RID: 7631 RVA: 0x000D4C08 File Offset: 0x000D2E08
	private void ShowAddAppointment(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData, CommonTableRowForItem itemView)
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
			ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(btnName, isInteractable, EItemMenuDisplayOrder.Other, delegate()
			{
				this.OnClickHomingPegon(itemView);
			}, null, null, false);
			btnData.SetTip(title, content);
			btnList.Add(btnData);
		}
	}

	// Token: 0x06001DD0 RID: 7632 RVA: 0x000D4CF8 File Offset: 0x000D2EF8
	private void ShowItemOperateMenuTool(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData, CommonTableRowForItem itemView)
	{
		bool flag = itemData.Key.ItemType == 6 && itemData.Durability > 0;
		if (flag)
		{
			CraftToolItem toolConfig = CraftTool.Instance[itemData.Key.TemplateId];
			bool flag2 = toolConfig.RequiredLifeSkillTypes.Exists((sbyte type) => UI_CharacterMenuItems.CanRepairLifeSkillType.Contains(type));
			if (flag2)
			{
				string btnName = LocalStringManager.Get(LanguageKey.LK_Repair_Item);
				bool interactable = this.CheckHasRepairableItemForTool(itemData);
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
			bool flag4 = toolConfig.RequiredLifeSkillTypes.Exists((sbyte type) => UI_CharacterMenuItems.CanDisassembleLifeSkillType.Contains(type));
			if (flag4)
			{
				string btnName2 = LocalStringManager.Get(LanguageKey.LK_Disassemble_Item);
				bool interactable2 = this.CheckHasDisassemblableItemForTool(itemData);
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
			bool isRepairable = ItemTemplateHelper.IsRepairable(itemData.Key.ItemType, itemData.Key.TemplateId);
			bool flag6 = isRepairable;
			if (flag6)
			{
				string btnName3 = LocalStringManager.Get(LanguageKey.LK_Repair_Item);
				bool emptyToolMeet;
				bool noNeed;
				bool noResource;
				bool interactable3 = this.CheckItemRepairCondition(itemData, out emptyToolMeet, out noNeed, out noResource);
				ViewPopupMenu.BtnData btnData3 = new ViewPopupMenu.BtnData(btnName3, interactable3, EItemMenuDisplayOrder.Tool, delegate()
				{
					this.OnClickItemEnterMultiplyMode(itemView, ItemOperationType.EItemOperationType.Repair, null);
				}, null, null, false);
				btnList.Add(btnData3);
				bool flag7 = !interactable3;
				if (flag7)
				{
					LanguageKey contentKey = LanguageKey.LK_Repair_Item_Tip_NoTool;
					bool flag8 = noNeed;
					if (flag8)
					{
						contentKey = LanguageKey.LK_Repair_Item_Tip_NoNeed;
					}
					bool flag9 = noResource;
					if (flag9)
					{
						contentKey = LanguageKey.LK_Repair_Item_Tip_NoResource;
					}
					string content3 = LocalStringManager.Get(contentKey);
					btnData3.SetTip("", content3);
				}
			}
			bool dissemblable = ItemTemplateHelper.GetCanDisassemble(itemData.Key.ItemType, itemData.Key.TemplateId);
			bool flag10 = dissemblable;
			if (flag10)
			{
				string btnName4 = LocalStringManager.Get(LanguageKey.LK_Disassemble_Item);
				bool emptyToolMeet2;
				bool interactable4 = this.CheckItemDisassembleCondition(itemData, out emptyToolMeet2);
				ViewPopupMenu.BtnData btnData4 = new ViewPopupMenu.BtnData(btnName4, interactable4, EItemMenuDisplayOrder.Tool, delegate()
				{
					this.OnClickItemEnterMultiplyMode(itemView, ItemOperationType.EItemOperationType.Disassemble, null);
				}, null, null, false);
				btnList.Add(btnData4);
				bool flag11 = !interactable4;
				if (flag11)
				{
					btnData4.SetTip("", LocalStringManager.Get(LanguageKey.LK_Disassemble_Item_Tip_NoTool));
				}
			}
		}
	}

	// Token: 0x06001DD1 RID: 7633 RVA: 0x000D4FBC File Offset: 0x000D31BC
	private void ShowItemOperateMenuEnemy(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData)
	{
		StringBuilder sb = EasyPool.Get<StringBuilder>();
		int costTime = GlobalConfig.Instance.HostileOperationTakeItemCostTime;
		int leftTime = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
		bool timeIsMeet = leftTime >= costTime;
		sbyte behaviorType = this._taiwuCharacterDisplayData.BehaviorType;
		bool ignoreBehavior = EventModel.IgnoreEventBehavior || !SingletonObject.getInstance<BasicGameData>().RestrictOptionsBehaviorType;
		short concentration = this._attributeMonitor.CurMainAttribute[2];
		short strength = this._attributeMonitor.CurMainAttribute[0];
		short dexterity = this._attributeMonitor.CurMainAttribute[1];
		bool scamAttributeIsMeet = concentration >= 20;
		bool canScam = timeIsMeet && scamAttributeIsMeet;
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
		bool canSteal = timeIsMeet && stealAttributeIsMeet && stealBehaviorIsMeet;
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
		bool canRob = timeIsMeet && robAttributeIsMeet && robBehaviorIsMeet;
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

	// Token: 0x06001DD2 RID: 7634 RVA: 0x000D52F0 File Offset: 0x000D34F0
	private void ShowItemOperateMenuEat(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData)
	{
		EatingItemMonitor eatingItems = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<EatingItemMonitor>(base.CharacterMenu.CurCharacterId, false);
		ValueTuple<int, string> valueTuple = CommonUtils.CalculateCountAndTip(base.CharacterMenu.CurCharacterId, itemData.Key, itemData.Amount);
		int eatCount = valueTuple.Item1;
		string limitTip = valueTuple.Item2;
		bool canEatMore = eatCount > 0;
		bool hasEatWugKing = (from x in eatingItems.EatingItemList
		select x.Item1).Any(new Func<ItemKey, bool>(EatingItems.IsWugKing));
		bool isWugKing = EatingItems.IsWugKing(itemData.Key);
		bool canReplaceWugKing = isWugKing && hasEatWugKing;
		bool flag = itemData.Key.ItemType == 8;
		if (flag)
		{
			bool interactable = true;
			bool flag2 = interactable;
			if (flag2)
			{
				bool hasAttributeToTopical = base.CharacterMenu.HasAttributeToTopical(itemData.Key);
				interactable = CommonUtils.GetMedicineItemMenuInteractable(itemData.Key, canEatMore, canReplaceWugKing, hasAttributeToTopical, ref limitTip);
			}
			else
			{
				limitTip += LanguageKey.LK_Ignore.Tr().SetColor("brightred");
			}
			string btnName = CommonUtils.GetCanEatItemButtonName(itemData.RealKey);
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
			bool flag3 = !interactable;
			if (flag3)
			{
				btnData.SetTip(string.Empty, limitTip);
			}
		}
		else
		{
			bool flag4 = CommonUtils.CanItemEat(itemData.Key.ItemType, itemData.Key.TemplateId, base.CharacterMenu.CurCharacterId);
			if (flag4)
			{
				bool flag5 = ItemTemplateHelper.IsTianJieFuLu(itemData.Key.ItemType, itemData.Key.TemplateId);
				if (flag5)
				{
					bool isEnough = itemData.Amount >= ItemTemplateHelper.GetTianJieFuLuCountUnit();
					ViewPopupMenu.BtnData innerButton = new ViewPopupMenu.BtnData(LocalStringManager.Get(LanguageKey.LK_Eat_Item), canEatMore && isEnough, EItemMenuDisplayOrder.Eat, delegate()
					{
						this.OnClickEatItem(itemData);
					}, delegate()
					{
						this.CharacterMenu.SetEatItemInfectNotice(true, itemData, 1);
					}, delegate()
					{
						this.CharacterMenu.SetEatItemInfectNotice(false, null, 1);
					}, false);
					bool flag6 = !isEnough;
					if (flag6)
					{
						innerButton.SetTip(string.Empty, LanguageKey.LK_Mousetip_TianjieFulu_NotEnough.Tr().SetColor("brightred"));
					}
					else
					{
						bool flag7 = !canEatMore;
						if (flag7)
						{
							innerButton.SetTip("", LocalStringManager.Get(LanguageKey.LK_Use_Medicine_Tip_NoSlot));
						}
					}
					btnList.Add(innerButton);
				}
				else
				{
					bool changeNeili = CommonUtils.CanItemEatForChangeNeili(itemData.Key.ItemType, itemData.Key.TemplateId);
					bool neiliIsNotMax = this._equipCombatSkillMonitor.CurrNeili < this._equipCombatSkillMonitor.MaxNeili;
					bool interactable2 = changeNeili ? (neiliIsNotMax && canEatMore) : canEatMore;
					string btnName2 = LocalStringManager.Get(LanguageKey.LK_Eat_Item);
					ViewPopupMenu.BtnData btnData2 = new ViewPopupMenu.BtnData(btnName2, interactable2, EItemMenuDisplayOrder.Eat, delegate()
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
					bool flag8 = changeNeili && !neiliIsNotMax;
					if (flag8)
					{
						btnData2.SetTip("", LocalStringManager.Get(LanguageKey.LK_ItemTips_Use_NeiliIsMax));
					}
					else
					{
						bool flag9 = !canEatMore;
						if (flag9)
						{
							btnData2.SetTip("", LocalStringManager.Get(LanguageKey.LK_Use_Medicine_Tip_NoSlot));
						}
					}
				}
			}
		}
	}

	// Token: 0x06001DD3 RID: 7635 RVA: 0x000D56A0 File Offset: 0x000D38A0
	private void ShowItemOperateMenuTake(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData)
	{
		bool isTaiwuGearMate = base.CharacterMenu.IsTaiwuGearMate(base.CharacterMenu.CurCharacterId);
		int limitSelectCount;
		bool interactable = UI_CharacterMenuItems.CanTakeItem(itemData, this._maxWorthCanBeLentToTaiwu, isTaiwuGearMate, out limitSelectCount);
		string btnName = LocalStringManager.Get(LanguageKey.LK_TakeFrom_Item);
		ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(btnName, interactable, EItemMenuDisplayOrder.Take, delegate()
		{
			this.OnClickTakeItemFrom(itemData, limitSelectCount);
		}, null, null, false);
		btnList.Add(btnData);
	}

	// Token: 0x06001DD4 RID: 7636 RVA: 0x000D5720 File Offset: 0x000D3920
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

	// Token: 0x06001DD5 RID: 7637 RVA: 0x000D57E8 File Offset: 0x000D39E8
	private void ShowItemOperateMenuExchange(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData)
	{
		bool flag = ItemTemplateHelper.MiscResourceCanExchange(itemData.Key.ItemType, itemData.Key.TemplateId);
		if (flag)
		{
			string btnName = LocalStringManager.Get(LanguageKey.LK_Bottom_Exchange);
			ViewPopupMenu.BtnData btnOn = new ViewPopupMenu.BtnData(btnName, true, EItemMenuDisplayOrder.Resource, new Action(this.OnClickResourceExchange), null, null, false);
			btnList.Add(btnOn);
		}
	}

	// Token: 0x06001DD6 RID: 7638 RVA: 0x000D5844 File Offset: 0x000D3A44
	private void ShowItemOperateMenuChoosy(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData)
	{
		bool flag = ItemTemplateHelper.MiscResourceCanChoosy(itemData.Key.ItemType, itemData.Key.TemplateId);
		if (flag)
		{
			sbyte type = ItemTemplateHelper.GetMiscResourceType(itemData.Key.ItemType, itemData.Key.TemplateId);
			string btnName = LocalStringManager.Get(LanguageKey.LK_Resource_Choosy_ItemMenu);
			ViewPopupMenu.BtnData btnOn = new ViewPopupMenu.BtnData(btnName, true, EItemMenuDisplayOrder.Resource, delegate()
			{
				this.OnClickResourceChoosy();
			}, null, null, false);
			btnList.Add(btnOn);
		}
	}

	// Token: 0x06001DD7 RID: 7639 RVA: 0x000D58BC File Offset: 0x000D3ABC
	private void ShowItemOperateMenuGive(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData)
	{
		bool hasTeammate = SingletonObject.getInstance<CharacterMonitorModel>().GetTaiwuTeamCharIds().Count > 1 || SingletonObject.getInstance<CharacterMonitorModel>().GetTaiwuSpecialGroup().Count > 0;
		string btnName = LocalStringManager.Get(LanguageKey.LK_Transfer_Item);
		ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(btnName, hasTeammate, EItemMenuDisplayOrder.Give, delegate()
		{
			this.OnClickTransferItem(itemData);
		}, null, null, false);
		btnList.Add(btnData);
		bool flag = !hasTeammate;
		if (flag)
		{
			string content = LocalStringManager.Get(LanguageKey.LK_Transfer_Item_Tip_NoTargetCharacter);
			btnData.SetTip(string.Empty, content);
		}
	}

	// Token: 0x06001DD8 RID: 7640 RVA: 0x000D5958 File Offset: 0x000D3B58
	private void ShowItemOperateMenuDiscard(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData)
	{
		bool inTutorial = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
		string btnName = LocalStringManager.Get(LanguageKey.LK_Discard_Item);
		ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(btnName, !inTutorial, EItemMenuDisplayOrder.Discard, delegate()
		{
			this.OnClickDiscard(itemData);
		}, null, null, false);
		btnList.Add(btnData);
	}

	// Token: 0x06001DD9 RID: 7641 RVA: 0x000D59B4 File Offset: 0x000D3BB4
	private void ShowProfessionMenuOther(List<ViewPopupMenu.BtnData> btnList, ItemDisplayData itemData, Action onShow, Queue<InventoryItemOperationType> queue)
	{
		this.AddProfessionButtonData(60, InventoryItemOperationType.ProfessionCapitalistSkill0, itemData, btnList, queue, onShow);
	}

	// Token: 0x06001DDA RID: 7642 RVA: 0x000D59C8 File Offset: 0x000D3BC8
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

	// Token: 0x06001DDB RID: 7643 RVA: 0x000D5A60 File Offset: 0x000D3C60
	private void AddProfessionButtonData(short professionSkillTemplateId, InventoryItemOperationType operationType, ItemDisplayData itemData, List<ViewPopupMenu.BtnData> btnList, Queue<InventoryItemOperationType> optionQueue, Action onShow)
	{
		UI_CharacterMenuItems.<>c__DisplayClass124_0 CS$<>8__locals1 = new UI_CharacterMenuItems.<>c__DisplayClass124_0();
		CS$<>8__locals1.professionSkillTemplateId = professionSkillTemplateId;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.itemData = itemData;
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
				bool flag4 = resourceInfo.ResourceCount > this._resourceMonitor.Resources[(int)resourceInfo.ResourceType];
				if (flag4)
				{
					CS$<>8__locals1.stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_Resource_NotMeet).SetColor(CS$<>8__locals1.color));
					CS$<>8__locals1.interactable = false;
					break;
				}
			}
			bool flag5 = skillConfig.ExpCost > this._exp;
			if (flag5)
			{
				CS$<>8__locals1.stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Exp_Not_Enough).SetColor(CS$<>8__locals1.color));
				CS$<>8__locals1.interactable = false;
			}
			bool isFavorMeet = this.CurCharacterIsTaiwu || this._basicInfoMonitor.FavorabilityToTaiwu >= skillConfig.RequiredFavorability;
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
				bool isSeniorityMeet = grade >= (int)this._basicInfoMonitor.NameRelatedData.OrgGrade;
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
					bool flag10 = !canIdentifyCricket;
					if (flag10)
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
					bool flag10 = !canUpgradeCricket;
					if (flag10)
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
			case InventoryItemOperationType.ProfessionHunterSkill3:
				CS$<>8__locals1.<AddProfessionButtonData>g__EnQueue|1();
				ExtraDomainMethod.AsyncCall.CanConvertToAnimalCharacter(this, CS$<>8__locals1.itemData.RealKey, delegate(int valueOffset, RawDataPool dataPool)
				{
					bool canConvertToAnimalCharacter = false;
					Serializer.Deserialize(dataPool, valueOffset, ref canConvertToAnimalCharacter);
					CS$<>8__locals1.interactable = (CS$<>8__locals1.interactable && canConvertToAnimalCharacter);
					bool flag10 = !canConvertToAnimalCharacter;
					if (flag10)
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

	// Token: 0x06001DDC RID: 7644 RVA: 0x000D5EA8 File Offset: 0x000D40A8
	private bool CheckHasRepairableItemForTool(ItemDisplayData toolData)
	{
		return this._inventoryItems.Exists(delegate(ItemDisplayData itemData)
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
					LifeSkillShorts lifeSkillAttainments = new LifeSkillShorts(this._lifeSkillMonitor.Attainments);
					sbyte b;
					short num;
					result = UI_CharacterMenuItems.CheckCurrentToolAttainment(ItemOperationType.EItemOperationType.Repair, toolData, itemData, lifeSkillAttainments, out b, out num);
				}
			}
			return result;
		});
	}

	// Token: 0x06001DDD RID: 7645 RVA: 0x000D5EE8 File Offset: 0x000D40E8
	private bool CheckHasDisassemblableItemForTool(ItemDisplayData toolData)
	{
		return this._inventoryItems.Exists(delegate(ItemDisplayData itemData)
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
				LifeSkillShorts lifeSkillAttainments = new LifeSkillShorts(this._lifeSkillMonitor.Attainments);
				sbyte b;
				short num;
				result = UI_CharacterMenuItems.CheckCurrentToolAttainment(ItemOperationType.EItemOperationType.Disassemble, toolData, itemData, lifeSkillAttainments, out b, out num);
			}
			return result;
		});
	}

	// Token: 0x06001DDE RID: 7646 RVA: 0x000D5F28 File Offset: 0x000D4128
	private bool CheckItemRepairCondition(ItemDisplayData itemData, out bool emptyToolMeet, out bool noNeed, out bool noResource)
	{
		emptyToolMeet = false;
		noNeed = false;
		noResource = false;
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
			ResourceInts curResource = new ResourceInts(this._resourceMonitor.Resources);
			bool flag2 = !curResource.CheckIsMeet(ref needResource);
			if (flag2)
			{
				noResource = true;
				result = false;
			}
			else
			{
				bool toolMeet = this.CheckInventoryTool(ItemOperationType.EItemOperationType.Repair, itemData, out emptyToolMeet);
				result = toolMeet;
			}
		}
		return result;
	}

	// Token: 0x06001DDF RID: 7647 RVA: 0x000D5FB8 File Offset: 0x000D41B8
	private bool CheckItemDisassembleCondition(ItemDisplayData itemData, out bool emptyToolMeet)
	{
		return this.CheckInventoryTool(ItemOperationType.EItemOperationType.Disassemble, itemData, out emptyToolMeet);
	}

	// Token: 0x06001DE0 RID: 7648 RVA: 0x000D5FD8 File Offset: 0x000D41D8
	private bool CheckInventoryTool(ItemOperationType.EItemOperationType operationType, ItemDisplayData itemData, out bool emptyToolMeet)
	{
		sbyte skillType = ItemTemplateHelper.GetCraftRequiredLifeSkillType(itemData.Key.ItemType, itemData.Key.TemplateId);
		short skillAttainment = this._lifeSkillMonitor.Attainments[(int)skillType];
		if (!true)
		{
		}
		short requiredAttainment2;
		if (operationType != ItemOperationType.EItemOperationType.Repair)
		{
			if (operationType != ItemOperationType.EItemOperationType.Disassemble)
			{
				throw new ArgumentOutOfRangeException();
			}
			requiredAttainment2 = ItemTemplateHelper.GetDisassembleRequiredAttainment(itemData.Key.ItemType, itemData.Key.TemplateId);
		}
		else
		{
			requiredAttainment2 = ItemTemplateHelper.GetRepairRequiredAttainment(itemData.Key.ItemType, itemData.Key.TemplateId, itemData.Durability);
		}
		if (!true)
		{
		}
		short requiredAttainment = requiredAttainment2;
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
			bool toolMeet = this._inventoryItems.Exists(delegate(ItemDisplayData item)
			{
				bool flag2 = item.Key.ItemType != 6;
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
						if (flag4)
						{
							result2 = false;
						}
						else
						{
							short finalAttainment2 = UI_Make.GetFinalAttainment(item.Key.TemplateId, skillAttainment, skillType);
							result2 = (finalAttainment2 >= requiredAttainment);
						}
					}
				}
				return result2;
			});
			result = toolMeet;
		}
		return result;
	}

	// Token: 0x06001DE1 RID: 7649 RVA: 0x000D60D4 File Offset: 0x000D42D4
	private void OnClickItemEnterMultiplyMode(CommonTableRowForItem itemView, ItemOperationType.EItemOperationType operationType, ItemDisplayData feedingTarget = null)
	{
		UIManager.Instance.HideUI(UIElement.PopupMenu);
		this.ExitItemOperation(false);
		ItemKey itemKey = (itemView == null) ? ItemKey.Invalid : itemView.Data.RealKey;
		if (operationType != ItemOperationType.EItemOperationType.Repair)
		{
			if (operationType != ItemOperationType.EItemOperationType.Disassemble)
			{
				if (operationType != ItemOperationType.EItemOperationType.Feeding)
				{
					throw new ArgumentOutOfRangeException("operationType", operationType, null);
				}
				this._multiplyItemScrollView.EnterFeedingMode(feedingTarget);
			}
			else
			{
				this._multiplyItemScrollView.EnterDisassembleMode();
			}
		}
		else
		{
			this._multiplyItemScrollView.EnterRepairMode();
		}
		bool flag = itemKey.IsValid();
		if (flag)
		{
			int index = this._itemScroll.OutputItemList.FindIndex((ItemDisplayData d) => d.RealKey.Equals(itemKey));
			this._itemScroll.ItemScrollView.LoopScroll.RefillCells(index, false);
			this._multiplyItemScrollView.ShowItemMultiplyOperationPanel();
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
			{
				CommonTableRowForItem clickItemView = this._itemScroll.FindItemViewByItem(itemKey);
				clickItemView.Click();
			});
		}
	}

	// Token: 0x06001DE2 RID: 7650 RVA: 0x000D61E0 File Offset: 0x000D43E0
	private void OnClickDiscard(ItemDisplayData itemData)
	{
		bool flag = itemData.Amount > 1;
		if (flag)
		{
			this._currItemOperation = ItemOperationType.EItemOperationType.Discard;
			int index = this._itemScroll.OutputItemList.IndexOf(itemData);
			this._itemScroll.SetItemToSelectCountMode(index, this._focusingTuple.Item1, delegate(int count)
			{
				bool flag2 = count > 0;
				if (flag2)
				{
					this.ShowDiscardItemConfirmDialog(itemData, count);
				}
			}, new Action(this.OnExitFocusMode), 1, 0, 1, null, false, null, null, -1);
		}
		else
		{
			this.ShowDiscardItemConfirmDialog(itemData, 1);
		}
	}

	// Token: 0x06001DE3 RID: 7651 RVA: 0x000D627F File Offset: 0x000D447F
	private void ShowDiscardItemConfirmDialog(ItemDisplayData itemData, int count)
	{
	}

	// Token: 0x06001DE4 RID: 7652 RVA: 0x000D6282 File Offset: 0x000D4482
	private void ClearItemUsingState(ItemDisplayData itemData)
	{
		ItemDisplayData.ClearItemUsingState(itemData, this._inventoryItems);
	}

	// Token: 0x06001DE5 RID: 7653 RVA: 0x000D6294 File Offset: 0x000D4494
	private short FindToolAttainment(ItemDisplayData itemData, sbyte skillType)
	{
		short toolAttainment = 0;
		this._multiplyItemScrollView.RefreshMultiplyAvailableTool();
		List<ItemDisplayData> availableToolList = this._multiplyItemScrollView.GetAvailableToolList(itemData);
		bool flag = availableToolList != null && availableToolList.Count > 0;
		if (flag)
		{
			this._currCraftTool = availableToolList.First<ItemDisplayData>();
			toolAttainment = UI_Make.GetToolAttainment(this._currCraftTool.Key.TemplateId, skillType);
		}
		return toolAttainment;
	}

	// Token: 0x06001DE6 RID: 7654 RVA: 0x000D62FC File Offset: 0x000D44FC
	private void OnClickIdentifyItem(CommonTableRowForItem itemView)
	{
	}

	// Token: 0x06001DE7 RID: 7655 RVA: 0x000D6300 File Offset: 0x000D4500
	private void ShowIdentifiedPoisonTip(bool show, Action action)
	{
		this._curIdentifiedResultAction = action;
		GameObject tip = this._itemPage.CGet<GameObject>("IdentifiedPoisonTip");
		tip.SetActive(show);
		bool flag = !show;
		if (!flag)
		{
			TextMeshProUGUI text = tip.GetComponentInChildren<TextMeshProUGUI>();
			text.SetText(LocalStringManager.Get(LanguageKey.LK_Poison_Identifying), true);
			SkeletonGraphic spine = tip.GetComponentInChildren<SkeletonGraphic>();
			string animName = this._curIdentifiedResultHasPoison ? "bad" : "good";
			spine.timeScale = 2f;
			spine.AnimationState.SetAnimation(0, animName, false);
			spine.AnimationState.Complete -= this.AnimationStateOnEnd;
			spine.AnimationState.Complete += this.AnimationStateOnEnd;
		}
	}

	// Token: 0x06001DE8 RID: 7656 RVA: 0x000D63BC File Offset: 0x000D45BC
	private void AnimationStateOnEnd(TrackEntry trackEntry)
	{
		GameObject tip = this._itemPage.CGet<GameObject>("IdentifiedPoisonTip");
		TextMeshProUGUI text = tip.GetComponentInChildren<TextMeshProUGUI>();
		LanguageKey key = (!this._curIdentifySuccess) ? LanguageKey.LK_Poison_Identify_LifeSkill_NotMeet : (this._curIdentifiedResultHasPoison ? LanguageKey.LK_Poison_Identify_HasPoison : LanguageKey.LK_Poison_Identify_NoPoison);
		text.SetText(LocalStringManager.Get(key).ColorReplace(), true);
		Action curIdentifiedResultAction = this._curIdentifiedResultAction;
		if (curIdentifiedResultAction != null)
		{
			curIdentifiedResultAction();
		}
	}

	// Token: 0x06001DE9 RID: 7657 RVA: 0x000D642C File Offset: 0x000D462C
	private void OnClickEatItem(ItemDisplayData itemData)
	{
		UI_CharacterMenuItems.<>c__DisplayClass142_0 CS$<>8__locals1 = new UI_CharacterMenuItems.<>c__DisplayClass142_0();
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
			this._multiplyItemScrollView.HideMultiplySelectButton();
			bool flag2 = CS$<>8__locals1.itemData.Key.ItemType == 12 && CS$<>8__locals1.itemData.Key.TemplateId != 265;
			int limitCount;
			string limitTip;
			if (flag2)
			{
				MiscItem miscItem = Misc.Instance[CS$<>8__locals1.itemData.Key.TemplateId];
				limitCount = Mathf.CeilToInt((float)(this._equipCombatSkillMonitor.MaxNeili - this._equipCombatSkillMonitor.CurrNeili) / (float)miscItem.Neili);
				limitTip = LocalStringManager.Get(LanguageKey.LK_ItemTips_Use_NeiliIsMax);
			}
			else
			{
				limitCount = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<EatingItemMonitor>(base.CharacterMenu.CurCharacterId, false).GetAvailableEatingSlotsCount() * ItemTemplateHelper.GetItemCountUnit(CS$<>8__locals1.itemData.Key.ItemType, CS$<>8__locals1.itemData.Key.TemplateId);
				limitTip = LocalStringManager.Get(LanguageKey.LK_SelectCount_Limit_Eat);
			}
			bool flag3 = CS$<>8__locals1.itemData.Amount > 1 && ItemTemplateHelper.CanUseMultiple(CS$<>8__locals1.itemData.Key);
			if (flag3)
			{
				int index = this._itemScroll.OutputItemList.IndexOf(CS$<>8__locals1.itemData);
				this._itemScroll.SetItemToSelectCountMode(index, this._focusingTuple.Item1, delegate(int count)
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
				}, new Action(CS$<>8__locals1.<OnClickEatItem>g__Cancel|1), 1, limitCount, 1, limitTip, false, delegate(int count)
				{
					CS$<>8__locals1.<>4__this.CharacterMenu.SetEatItemInfectNotice(true, CS$<>8__locals1.itemData, count);
				}, null, -1);
			}
			else
			{
				CS$<>8__locals1.<OnClickEatItem>g__Confirm|0(1);
			}
		}
	}

	// Token: 0x06001DEA RID: 7658 RVA: 0x000D6639 File Offset: 0x000D4839
	private void OnClickUseMedicineOuter(ItemDisplayData itemData)
	{
	}

	// Token: 0x06001DEB RID: 7659 RVA: 0x000D663C File Offset: 0x000D483C
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

	// Token: 0x06001DEC RID: 7660 RVA: 0x000D6708 File Offset: 0x000D4908
	private void EnterTransferMode(ItemDisplayData itemData)
	{
		this._selectedToTransferItemData = itemData;
		this._multiplyItemScrollView.HideMultiplySelectButton();
		this.AddDebtMouseTipInfo();
		this._currItemOperation = ItemOperationType.EItemOperationType.Transfer;
		CommonTableRowForItem itemView = this._itemScroll.FindItemViewByItem(itemData.RealKey);
		bool flag = itemData.Amount > 1;
		if (flag)
		{
			int index = this._itemScroll.OutputItemList.IndexOf(itemData);
			this._itemScroll.SetItemToSelectCountMode(index, this._focusingTuple.Item1, delegate(int count)
			{
				bool flag2 = count <= 0;
				if (flag2)
				{
					this.OnExitFocusMode();
				}
				else
				{
					itemView.SetSelectState(true);
					ItemDisplayData newItemDisplayData = itemData.Clone(count);
					GEvent.OnEvent(UiEvents.EnterTransferItem, EasyPool.Get<ArgumentBox>().SetObject("ItemDisplayData", newItemDisplayData));
					this.SetItemScrollViewCanScroll(false);
				}
			}, new Action(this.OnExitFocusMode), 1, 0, 1, null, true, null, null, -1);
		}
		else
		{
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
			{
				this.SetItemScrollViewCanScroll(false);
			});
			GEvent.OnEvent(UiEvents.EnterTransferItem, EasyPool.Get<ArgumentBox>().SetObject("ItemDisplayData", itemData));
		}
	}

	// Token: 0x06001DED RID: 7661 RVA: 0x000D680E File Offset: 0x000D4A0E
	private void AddDebtMouseTipInfo()
	{
		base.CharacterMenu.RerenderCharacterScroll();
	}

	// Token: 0x06001DEE RID: 7662 RVA: 0x000D6820 File Offset: 0x000D4A20
	private void OnSelectTransferItemChar(ArgumentBox argsBox)
	{
		int characterId;
		argsBox.Get("CharacterId", out characterId);
		bool isMultiItemSelect = this._multiplyItemScrollView.IsMultiItemSelect;
		if (isMultiItemSelect)
		{
			this._multiplyItemScrollView.CurCharId = characterId;
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

	// Token: 0x06001DEF RID: 7663 RVA: 0x000D6880 File Offset: 0x000D4A80
	private void OnClickTakeItemFrom(ItemDisplayData itemData, int limitSelectCount)
	{
		bool flag = itemData.Amount > 1;
		if (flag)
		{
			this._currItemOperation = ItemOperationType.EItemOperationType.Take;
			int index = this._itemScroll.OutputItemList.IndexOf(itemData);
			this._itemScroll.SetItemToSelectCountMode(index, this._focusingTuple.Item1, delegate(int count)
			{
				this.OnExitFocusMode();
				bool flag2 = count > 0;
				if (flag2)
				{
					ItemDisplayData newItemDisplayData = itemData.Clone(count);
					this.TransferItem(newItemDisplayData, this._taiwuCharId);
				}
			}, new Action(this.OnExitFocusMode), 1, limitSelectCount, 1, null, false, null, null, -1);
		}
		else
		{
			this.TransferItem(itemData, this._taiwuCharId);
			this.OnExitFocusMode();
		}
	}

	// Token: 0x06001DF0 RID: 7664 RVA: 0x000D692B File Offset: 0x000D4B2B
	private void TransferItem(ItemDisplayData itemData, int characterId)
	{
	}

	// Token: 0x06001DF1 RID: 7665 RVA: 0x000D692E File Offset: 0x000D4B2E
	private void RefreshSort()
	{
	}

	// Token: 0x06001DF2 RID: 7666 RVA: 0x000D6931 File Offset: 0x000D4B31
	private void OnClickUseCommonEvent(ItemDisplayData itemData)
	{
		base.CharacterMenu.ExitFocusMode(true);
		base.CharacterMenu.QuickHide();
		TaiwuEventDomainMethod.Call.OperateInventoryItem(this._taiwuCharId, 9, itemData);
	}

	// Token: 0x06001DF3 RID: 7667 RVA: 0x000D695C File Offset: 0x000D4B5C
	private void OnClickHomingPegon(CommonTableRowForItem itemView)
	{
		base.CharacterMenu.ExitFocusMode(true);
		GEvent.OnEvent(UiEvents.InviteSelectBlockStart, null);
		base.CharacterMenu.QuickHide();
	}

	// Token: 0x06001DF4 RID: 7668 RVA: 0x000D698C File Offset: 0x000D4B8C
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

	// Token: 0x06001DF5 RID: 7669 RVA: 0x000D69DD File Offset: 0x000D4BDD
	private void OnClickSectMainStoryItemWuxianWugFairy()
	{
		this.OnExitFocusMode();
		UIManager.Instance.HideUI(UIElement.CharacterMenu);
		TaiwuEventDomainMethod.Call.CloseUI("WuxianWugFairy", false, -1);
	}

	// Token: 0x06001DF6 RID: 7670 RVA: 0x000D6A04 File Offset: 0x000D4C04
	private void OnClickSectMainStoryItemYuanshanRosary()
	{
		this.OnExitFocusMode();
		UIManager.Instance.HideUI(UIElement.CharacterMenu);
		UIManager.Instance.ShowUI(UIElement.ThreeVitals, true);
	}

	// Token: 0x06001DF7 RID: 7671 RVA: 0x000D6A2F File Offset: 0x000D4C2F
	private void OnClickSectMainStoryItemJieQingStars()
	{
		this.OnExitFocusMode();
		UIManager.Instance.HideUI(UIElement.CharacterMenu);
		UIManager.Instance.ShowUI(UIElement.JieQingInteract, true);
	}

	// Token: 0x06001DF8 RID: 7672 RVA: 0x000D6A5C File Offset: 0x000D4C5C
	private void OnClickThanksLetter(ItemDisplayData itemData)
	{
		bool flag = itemData.Amount > 1;
		if (flag)
		{
			int index = this._itemScroll.OutputItemList.IndexOf(itemData);
			this._itemScroll.SetItemToSelectCountMode(index, this._focusingTuple.Item1, delegate(int count)
			{
				bool flag2 = count > 0;
				if (flag2)
				{
					this.UseThanksLetter(itemData, count);
				}
			}, new Action(this.OnExitFocusMode), 1, 0, 1, null, false, null, null, -1);
		}
		else
		{
			this.UseThanksLetter(itemData, 1);
		}
	}

	// Token: 0x06001DF9 RID: 7673 RVA: 0x000D6AF4 File Offset: 0x000D4CF4
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

	// Token: 0x06001DFA RID: 7674 RVA: 0x000D6BA0 File Offset: 0x000D4DA0
	private void OnClickSectMainStoryItemFulongChickenMap()
	{
		this.OnExitFocusMode();
		UIManager.Instance.HideUI(UIElement.CharacterMenu);
		UIManager.Instance.ShowUI(UIElement.ChickenMap, true);
		this._lastUseChickenMapAreaId = (int)SingletonObject.getInstance<WorldMapModel>().CurrentAreaId;
		BuildingDomainMethod.AsyncCall.ClickChickenMap(this, delegate(int offset, RawDataPool dataPool)
		{
			Serializer.Deserialize(dataPool, offset, ref this._chickenMapInteractable);
		});
	}

	// Token: 0x06001DFB RID: 7675 RVA: 0x000D6BFC File Offset: 0x000D4DFC
	private void ChickenMapHandle()
	{
		foreach (ItemDisplayData itemDisplayData in this._inventoryItems)
		{
			bool flag = itemDisplayData.Key.ItemType == 12 && itemDisplayData.Key.TemplateId == 370;
			if (flag)
			{
				BuildingDomainMethod.AsyncCall.AllChickenInTaiwuVillage(this, delegate(int offset, RawDataPool dataPool)
				{
					Serializer.Deserialize(dataPool, offset, ref this._allChickenInTaiwuVillage);
				});
			}
		}
	}

	// Token: 0x06001DFC RID: 7676 RVA: 0x000D6C8C File Offset: 0x000D4E8C
	private void RefreshOverloadMouseTips()
	{
	}

	// Token: 0x06001DFD RID: 7677 RVA: 0x000D6C8F File Offset: 0x000D4E8F
	private void OnClickResourceChoosy()
	{
		this.OnExitFocusMode();
		this.ShowChoosyPage();
	}

	// Token: 0x06001DFE RID: 7678 RVA: 0x000D6CA0 File Offset: 0x000D4EA0
	private void OnClickResourceExchange()
	{
		int charId = base.CharacterMenu.CurCharacterId;
		this.OnExitFocusMode();
		this.ExchangeResource(charId, true);
	}

	// Token: 0x06001DFF RID: 7679 RVA: 0x000D6CCC File Offset: 0x000D4ECC
	private void OnClickResourceScam(ItemDisplayData itemData)
	{
		sbyte operationType = InventoryItemOperationType.Scam.ToSbyte();
		this.OperateResourceItem(operationType, itemData);
	}

	// Token: 0x06001E00 RID: 7680 RVA: 0x000D6CF0 File Offset: 0x000D4EF0
	private void OnClickResourceSteal(ItemDisplayData itemData)
	{
		sbyte operationType = InventoryItemOperationType.Steal.ToSbyte();
		this.OperateResourceItem(operationType, itemData);
	}

	// Token: 0x06001E01 RID: 7681 RVA: 0x000D6D14 File Offset: 0x000D4F14
	private void OnClickResourceRob(ItemDisplayData itemData)
	{
		sbyte operationType = InventoryItemOperationType.Rob.ToSbyte();
		this.OperateResourceItem(operationType, itemData);
	}

	// Token: 0x06001E02 RID: 7682 RVA: 0x000D6D38 File Offset: 0x000D4F38
	private void OperateResourceItem(sbyte operationType, ItemDisplayData itemData)
	{
		int limitSelectCount = itemData.Amount / GlobalConfig.Instance.HostileOperationTakeItemMaxResourceFactor;
		limitSelectCount = Math.Max(1, limitSelectCount);
		this._currItemOperation = ItemOperationType.EItemOperationType.Take;
		int index = this._itemScroll.OutputItemList.IndexOf(itemData);
		this._itemScroll.SetItemToSelectCountMode(index, this._focusingTuple.Item1, delegate(int count)
		{
			int charId = this.CharacterMenu.CurCharacterId;
			this.OnExitFocusMode();
			UIManager.Instance.HideUI(UIElement.CharacterMenu);
			ItemDisplayData newItemData = itemData.Clone(count);
			TaiwuEventDomainMethod.Call.OperateInventoryItem(charId, operationType, newItemData);
		}, new Action(this.OnExitFocusMode), 1, limitSelectCount, 1, null, false, null, null, -1);
	}

	// Token: 0x06001E03 RID: 7683 RVA: 0x000D6DD8 File Offset: 0x000D4FD8
	private void ItemMultiplyOperationTypeChange(ArgumentBox argsBox)
	{
		Enum type;
		bool flag = argsBox.Get("ItemOperationType", out type);
		if (flag)
		{
			ItemOperationType.EItemOperationType itemOperationType = (ItemOperationType.EItemOperationType)type;
			ItemOperationType.EItemOperationType currItemOperation = this._multiplyItemScrollView.CurrItemOperation;
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
		}
		this._multiplyItemScrollView.OnItemMultiplyOperationTypeChange(argsBox);
	}

	// Token: 0x06001E04 RID: 7684 RVA: 0x000D6E58 File Offset: 0x000D5058
	private void ItemMultiplyOperationTargetChange(ArgumentBox argumentBox)
	{
		ItemDisplayData target;
		argumentBox.Get<ItemDisplayData>("FeedingTarget", out target);
		this._multiplyItemScrollView.SetFeedingTarget(target);
	}

	// Token: 0x06001E05 RID: 7685 RVA: 0x000D6E81 File Offset: 0x000D5081
	private void ExchangeResource(int charId, bool worthLimited)
	{
		UIElement.ExchangeResource.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("NpcCharId", charId).Set("WorthLimited", worthLimited));
		UIManager.Instance.ShowUI(UIElement.ExchangeResource, true);
	}

	// Token: 0x06001E06 RID: 7686 RVA: 0x000D6EBC File Offset: 0x000D50BC
	private void UpdateDebt()
	{
		bool showDebtEntrance = !base.CharacterMenu.CurrentCharacterIsTaiwu && this._curCharLoveAndHateItemInfo != null && this._curCharLoveAndHateItemInfo.CharacterId == base.CharacterMenu.CurCharacterId && this._curCharLoveAndHateItemInfo.CreatingType == 1;
		GameObject debtEntrance = this._itemPage.CGet<GameObject>("DebtEntrance");
		debtEntrance.gameObject.SetActive(showDebtEntrance);
	}

	// Token: 0x06001E07 RID: 7687 RVA: 0x000D6F28 File Offset: 0x000D5128
	private void UpdateHobby()
	{
		this.UpdateDebt();
		Refers hobby = this._itemPage.CGet<Refers>("Hobby");
		bool currentCharacterIsTaiwu = base.CharacterMenu.CurrentCharacterIsTaiwu;
		if (currentCharacterIsTaiwu)
		{
			hobby.gameObject.SetActive(false);
		}
		else
		{
			bool flag = this._curCharLoveAndHateItemInfo == null || this._curCharLoveAndHateItemInfo.CharacterId != base.CharacterMenu.CurCharacterId;
			if (!flag)
			{
				hobby.gameObject.SetActive(this._curCharLoveAndHateItemInfo.CreatingType == 1);
				CommonConfigurableParameterGrid commonConfigurableParameterGrid = hobby.CGet<CommonConfigurableParameterGrid>("CommonConfigurableParameterGrid");
				commonConfigurableParameterGrid.Init();
				Refers loveItem = commonConfigurableParameterGrid.GetCellItem(0);
				Refers hateItem = commonConfigurableParameterGrid.GetCellItem(1);
				string questionMark = LocalStringManager.Get(LanguageKey.LK_QuestioMark);
				string lovingName = questionMark + questionMark + questionMark;
				bool lovingItemRevealed = this._curCharLoveAndHateItemInfo.LovingItemRevealed;
				if (lovingItemRevealed)
				{
					lovingName = ((this._curCharLoveAndHateItemInfo.LovingItemSubType >= 0) ? LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", this._curCharLoveAndHateItemInfo.LovingItemSubType)) : LocalStringManager.Get(LanguageKey.LK_None));
				}
				loveItem.CGet<TextMeshProUGUI>("Value").text = lovingName;
				loveItem.CGet<CImage>("Icon").SetSprite("ui_sp_items_icon_loveandhate_0", false, null);
				loveItem.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.Get(LanguageKey.LK_Loving).SetColor("favorite");
				string hateName = questionMark + questionMark + questionMark;
				bool hatingItemRevealed = this._curCharLoveAndHateItemInfo.HatingItemRevealed;
				if (hatingItemRevealed)
				{
					hateName = ((this._curCharLoveAndHateItemInfo.HatingItemSubType >= 0) ? LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", this._curCharLoveAndHateItemInfo.HatingItemSubType)) : LocalStringManager.Get(LanguageKey.LK_None));
				}
				hateItem.CGet<TextMeshProUGUI>("Value").text = hateName;
				hateItem.CGet<CImage>("Icon").SetSprite("ui_sp_items_icon_loveandhate_1", false, null);
				hateItem.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.Get(LanguageKey.LK_Hate).SetColor("hate");
				int time = this._curCharLoveAndHateItemInfo.HobbyExpirationDate - SingletonObject.getInstance<BasicGameData>().CurrDate;
				string timeStr = (time <= 0) ? "-" : time.ToString();
				hobby.CGet<TextMeshProUGUI>("Time").text = timeStr;
				TooltipInvoker tip = hobby.CGet<TooltipInvoker>("Tip");
				tip.Type = TipType.Simple;
				string title = LocalStringManager.Get(LanguageKey.LK_Hobby_Tip_Tittle);
				StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
				strBuilder.Clear();
				bool flag2 = !this._curCharLoveAndHateItemInfo.LovingItemRevealed || (this._curCharLoveAndHateItemInfo.LovingItemRevealed && this._curCharLoveAndHateItemInfo.LovingItemSubType >= 0);
				if (flag2)
				{
					string lovingTip = LocalStringManager.GetFormat(LanguageKey.LK_Hobby_Tip_Loving, lovingName.SetColor("favorite"));
					strBuilder.Append(lovingTip);
					strBuilder.Append("\n");
				}
				bool flag3 = !this._curCharLoveAndHateItemInfo.HatingItemRevealed || (this._curCharLoveAndHateItemInfo.HatingItemRevealed && this._curCharLoveAndHateItemInfo.HatingItemSubType >= 0);
				if (flag3)
				{
					string hateTip = LocalStringManager.GetFormat(LanguageKey.LK_Hobby_Tip_Hate, hateName.SetColor("hate"));
					strBuilder.Append(hateTip);
					strBuilder.Append("\n");
				}
				string timeTip = LocalStringManager.GetFormat(LanguageKey.LK_Hobby_Tip_Time, timeStr);
				strBuilder.Append(timeTip);
				string content = strBuilder.ToString();
				EasyPool.Free<StringBuilder>(strBuilder);
				tip.PresetParam = new string[]
				{
					title,
					content
				};
			}
		}
	}

	// Token: 0x06001E08 RID: 7688 RVA: 0x000D72C4 File Offset: 0x000D54C4
	private bool TryShowHobbyUnlockEffect()
	{
		bool flag = ViewCharacterMenu.CurSubSubPageIndex != ECharacterSubPage.Prison && ViewCharacterMenu.CurSubToggleIndex != ECharacterSubToggleBase.ItemBase;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool currentCharacterIsTaiwu = base.CharacterMenu.CurrentCharacterIsTaiwu;
			if (currentCharacterIsTaiwu)
			{
				result = false;
			}
			else
			{
				base.StopCoroutine("ShowHobbyUnlockEffect");
				this._itemPage.CGet<Refers>("Hobby").gameObject.SetActive(false);
				bool flag2 = this._curCharLoveAndHateItemInfo.CharacterId != base.CharacterMenu.CurCharacterId || this._curCharLoveAndHateItemInfo.CreatingType != 1;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = !this._curCharLoveAndHateItemInfo.NeedShowFirstRevealLovingEffect && !this._curCharLoveAndHateItemInfo.NeedShowFirstRevealHatingEffect;
					if (flag3)
					{
						result = false;
					}
					else
					{
						base.StartCoroutine(this.ShowHobbyUnlockEffect());
						result = true;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06001E09 RID: 7689 RVA: 0x000D739F File Offset: 0x000D559F
	private IEnumerator ShowHobbyUnlockEffect()
	{
		Refers hobby = this._itemPage.CGet<Refers>("Hobby");
		hobby.gameObject.SetActive(true);
		bool needShowFirstRevealLovingEffect = this._curCharLoveAndHateItemInfo.NeedShowFirstRevealLovingEffect;
		if (needShowFirstRevealLovingEffect)
		{
			ExtraDomainMethod.Call.SetCharacterRevealedHobbies(this._curCharLoveAndHateItemInfo.CharacterId, true);
		}
		this._curCharLoveAndHateItemInfo.NeedShowFirstRevealLovingEffect = false;
		bool needShowFirstRevealHatingEffect = this._curCharLoveAndHateItemInfo.NeedShowFirstRevealHatingEffect;
		if (needShowFirstRevealHatingEffect)
		{
			ExtraDomainMethod.Call.SetCharacterRevealedHobbies(this._curCharLoveAndHateItemInfo.CharacterId, false);
		}
		this._curCharLoveAndHateItemInfo.NeedShowFirstRevealHatingEffect = false;
		this.UpdateHobby();
		yield return null;
		yield break;
	}

	// Token: 0x06001E0A RID: 7690 RVA: 0x000D73B0 File Offset: 0x000D55B0
	public override void OnLanguageChange(LocalStringManager.LanguageType languageType)
	{
		base.OnLanguageChange(languageType);
		bool activeSelf = this._itemPage.gameObject.activeSelf;
		if (activeSelf)
		{
			this._itemScroll.RefreshTableHeadTitle();
			this._itemScroll.Refresh();
		}
		bool activeSelf2 = this._kidnapPage.gameObject.activeSelf;
		if (activeSelf2)
		{
			this._kidnapScroll.ReRender();
		}
	}

	// Token: 0x17000307 RID: 775
	// (get) Token: 0x06001E0B RID: 7691 RVA: 0x000D7416 File Offset: 0x000D5616
	private UI_CollectResource uiCollectResource
	{
		get
		{
			return this._choosyPage.CGet<UI_CollectResource>("UI_CollectResource");
		}
	}

	// Token: 0x17000308 RID: 776
	// (get) Token: 0x06001E0C RID: 7692 RVA: 0x000D7428 File Offset: 0x000D5628
	private GameObject AnimMask
	{
		get
		{
			return this._choosyPage.CGet<GameObject>("AnimMask");
		}
	}

	// Token: 0x17000309 RID: 777
	// (get) Token: 0x06001E0D RID: 7693 RVA: 0x000D743A File Offset: 0x000D563A
	private bool DisableOperation
	{
		get
		{
			return this.AnimMask.activeSelf;
		}
	}

	// Token: 0x06001E0E RID: 7694 RVA: 0x000D7448 File Offset: 0x000D5648
	private void InitChoosyPage()
	{
		this._choosyPage = base.CGet<Refers>("ChoosyPage");
		this.HideChoosyPage();
		RectTransform resourceLayout = this._choosyPage.CGet<RectTransform>("ResourceLayout");
		for (sbyte i = 0; i < 6; i += 1)
		{
			sbyte resourceType = i;
			Refers refers = resourceLayout.GetChild((int)resourceType).GetComponent<Refers>();
			refers.CGet<CommonItemBack>("CommonItemBack").SetInteractable(false);
			CButtonObsolete buttonMore = refers.CGet<CButtonObsolete>("ButtonMore");
			buttonMore.ClearAndAddListener(delegate
			{
				this.ChangeMakeResourceCount(resourceType, true, false, false, -1);
				this.RefreshChoosyPage();
			});
			CButtonObsolete buttonLess = refers.CGet<CButtonObsolete>("ButtonLess");
			buttonLess.ClearAndAddListener(delegate
			{
				this.ChangeMakeResourceCount(resourceType, false, false, false, -1);
				this.RefreshChoosyPage();
			});
			TMP_InputField inputField = refers.CGet<TMP_InputField>("InputField");
			CSliderLegacy slider = refers.CGet<CSliderLegacy>("Slider");
			inputField.onEndEdit.RemoveAllListeners();
			inputField.onEndEdit.AddListener(delegate(string value)
			{
				long tempNumber;
				long.TryParse(value, out tempNumber);
				int number = (int)Math.Clamp(tempNumber, 0L, (long)this._resourceMonitor.Resources[(int)resourceType]);
				int times = number / this._choosyCostUnit;
				int target = times * this._choosyCostUnit;
				this.ChangeMakeResourceCount(resourceType, false, false, false, target);
				this.RefreshChoosyInteractable();
				slider.SetValueWithoutNotify((float)times);
				inputField.SetTextWithoutNotify(target.ToString());
			});
			slider.onValueChanged.RemoveAllListeners();
			slider.onValueChanged.AddListener(delegate(float value)
			{
				int target = Convert.ToInt32(value * (float)this._choosyCostUnit);
				this.ChangeMakeResourceCount(resourceType, false, false, false, target);
				this.RefreshChoosyInteractable();
				inputField.SetTextWithoutNotify(target.ToString());
			});
		}
		CButtonObsolete buttonChoosy = this._choosyPage.CGet<CButtonObsolete>("ButtonChoosy");
		buttonChoosy.interactable = false;
		buttonChoosy.ClearAndAddListener(new Action(this.OnClickButtonChoosy));
		this._choosyPage.CGet<CButtonObsolete>("ButtonBack").ClearAndAddListener(new Action(this.HideChoosyPage));
		this._choosyPage.CGet<CButtonObsolete>("ButtonCancel").ClearAndAddListener(new Action(this.HideChoosyPage));
	}

	// Token: 0x06001E0F RID: 7695 RVA: 0x000D7608 File Offset: 0x000D5808
	private void RefreshChoosyInteractable()
	{
		CButtonObsolete buttonChoosy = this._choosyPage.CGet<CButtonObsolete>("ButtonChoosy");
		TooltipInvoker tip = buttonChoosy.GetComponent<TooltipInvoker>();
		bool isMeet = this._curChoosyResourceValues.Any((int v) => v >= this._choosyCostUnit);
		bool isZero = this._curChoosyResourceValues.All((int v) => v == 0);
		string tipText = LocalStringManager.Get(isMeet ? LanguageKey.LK_Resource_Choosy_Tip : (isZero ? LanguageKey.LK_Resource_Choosy_Zero_Tip : LanguageKey.LK_Resource_Choosy_NotMeet_Tip));
		TooltipInvoker tooltipInvoker = tip;
		string[] array = new string[2];
		array[0] = tipText;
		tooltipInvoker.PresetParam = array;
		buttonChoosy.interactable = isMeet;
		RectTransform resourceLayout = this._choosyPage.CGet<RectTransform>("ResourceLayout");
		for (sbyte resourceType = 0; resourceType < 6; resourceType += 1)
		{
			Refers refers = resourceLayout.GetChild((int)resourceType).GetComponent<Refers>();
			int minCount = 0;
			int curCount = this._curChoosyResourceValues[(int)resourceType];
			int maxCount = this._resourceMonitor.Resources[(int)resourceType] / this._choosyCostUnit * this._choosyCostUnit;
			CButtonObsolete btnMore = refers.CGet<CButtonObsolete>("ButtonMore");
			CButtonObsolete btnLess = refers.CGet<CButtonObsolete>("ButtonLess");
			btnMore.interactable = (curCount < maxCount);
			btnLess.interactable = (curCount > minCount);
		}
	}

	// Token: 0x06001E10 RID: 7696 RVA: 0x000D774C File Offset: 0x000D594C
	private void RefreshChoosyPage()
	{
		RectTransform resourceLayout = this._choosyPage.CGet<RectTransform>("ResourceLayout");
		for (sbyte resourceType = 0; resourceType < 6; resourceType += 1)
		{
			Refers refers = resourceLayout.GetChild((int)resourceType).GetComponent<Refers>();
			int amount = this._resourceMonitor.Resources[(int)resourceType];
			short templateId = Convert.ToInt16((int)resourceType);
			ItemDisplayData itemData = new ItemDisplayData(12, templateId);
			ResourceTypeItem config = Config.ResourceType.Instance[resourceType];
			refers.CGet<CommonItemBack>("CommonItemBack").SetData(itemData, amount);
			refers.CGet<CImage>("Icon").SetSprite(config.Icon, false, null);
			refers.CGet<TextMeshProUGUI>("Name").text = config.Name;
			refers.CGet<TextMeshProUGUI>("Amount").text = CommonUtils.GetDisplayStringForNum(amount, 100000);
			int target = this._curChoosyResourceValues[(int)resourceType];
			TMP_InputField inputField = refers.CGet<TMP_InputField>("InputField");
			inputField.SetTextWithoutNotify(target.ToString());
			inputField.interactable = (amount > this._choosyCostUnit);
			CSliderLegacy slider = refers.CGet<CSliderLegacy>("Slider");
			slider.minValue = 0f;
			slider.maxValue = (float)(amount / this._choosyCostUnit);
			slider.SetValueWithoutNotify((float)(target / this._choosyCostUnit));
			slider.interactable = (amount > this._choosyCostUnit);
		}
		this.RefreshChoosyInteractable();
	}

	// Token: 0x06001E11 RID: 7697 RVA: 0x000D78B0 File Offset: 0x000D5AB0
	private void ShowChoosyPage()
	{
		this.uiCollectResource.InitForChoosy();
		this.RefreshChoosyPage();
		this.AnimMask.SetActive(false);
		this._choosyPage.gameObject.SetActive(true);
		ViewCharacterMenu characterMenu = base.CharacterMenu;
		if (characterMenu.OnTryClosePage == null)
		{
			characterMenu.OnTryClosePage = delegate()
			{
				this.HideChoosyPage();
				base.CharacterMenu.OnTryClosePage = null;
			};
		}
	}

	// Token: 0x06001E12 RID: 7698 RVA: 0x000D7914 File Offset: 0x000D5B14
	private void HideChoosyPage()
	{
		bool activeSelf = this.AnimMask.gameObject.activeSelf;
		if (!activeSelf)
		{
			for (int index = 0; index < this._curChoosyResourceValues.Length; index++)
			{
				this._curChoosyResourceValues[index] = 0;
			}
			this.uiCollectResource.Clear();
			this._choosyPage.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001E13 RID: 7699 RVA: 0x000D7978 File Offset: 0x000D5B78
	private void ChangeMakeResourceCount(sbyte resourceType, bool isMore, bool isToEdge = false, bool isToLast = false, int targetResource = -1)
	{
		UI_CharacterMenuItems.<>c__DisplayClass192_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.isMore = isMore;
		CS$<>8__locals1.resourceType = resourceType;
		bool disableOperation = this.DisableOperation;
		if (!disableOperation)
		{
			CS$<>8__locals1.minCount = 0;
			CS$<>8__locals1.curCount = this._curChoosyResourceValues[(int)CS$<>8__locals1.resourceType];
			CS$<>8__locals1.maxCount = this._resourceMonitor.Resources[(int)CS$<>8__locals1.resourceType] / this._choosyCostUnit * this._choosyCostUnit;
			bool flag = targetResource >= 0;
			if (flag)
			{
				targetResource = Mathf.Clamp(targetResource, CS$<>8__locals1.minCount, CS$<>8__locals1.maxCount);
				bool flag2 = CS$<>8__locals1.curCount == targetResource;
				if (flag2)
				{
					return;
				}
				CS$<>8__locals1.isMore = (CS$<>8__locals1.curCount < targetResource);
				CS$<>8__locals1.minCount = (CS$<>8__locals1.maxCount = targetResource);
				isToEdge = true;
			}
			int moreDiff = Mathf.Max(CS$<>8__locals1.maxCount - CS$<>8__locals1.curCount, 0) / this._choosyCostUnit;
			int lessDiff = Mathf.Max(CS$<>8__locals1.curCount - CS$<>8__locals1.minCount, 0) / this._choosyCostUnit;
			int offset = isToEdge ? (CS$<>8__locals1.isMore ? moreDiff : lessDiff) : 1;
			bool needFast = offset > 100;
			bool flag3 = targetResource >= 0;
			if (flag3)
			{
				this._curChoosyResourceValues[(int)CS$<>8__locals1.resourceType] = targetResource;
				this.uiCollectResource.PlayResourceDropAnim(CS$<>8__locals1.resourceType, this._curChoosyResourceValues[(int)CS$<>8__locals1.resourceType]);
			}
			else
			{
				bool flag4 = !isToLast;
				if (flag4)
				{
					int change = offset * this._choosyCostUnit;
					this._curChoosyResourceValues[(int)CS$<>8__locals1.resourceType] = (CS$<>8__locals1.isMore ? (CS$<>8__locals1.curCount + change) : (CS$<>8__locals1.curCount - change));
				}
				bool flag5 = needFast || isToLast || offset > 1;
				if (flag5)
				{
					int spriteCount = CS$<>8__locals1.curCount / this._choosyCostUnit;
					int from = isToLast ? 1 : 0;
					int to = isToLast ? (spriteCount + 1) : offset;
					for (int i = from; i < to; i++)
					{
						this.<ChangeMakeResourceCount>g__Tick|192_0(ref CS$<>8__locals1);
					}
				}
				else
				{
					this.<ChangeMakeResourceCount>g__Tick|192_0(ref CS$<>8__locals1);
				}
				this.uiCollectResource.PlayResourceDropAnim(CS$<>8__locals1.resourceType, this._curChoosyResourceValues[(int)CS$<>8__locals1.resourceType]);
			}
		}
	}

	// Token: 0x06001E14 RID: 7700 RVA: 0x000D7BB4 File Offset: 0x000D5DB4
	private void OnClickButtonChoosy()
	{
		this.AnimMask.SetActive(true);
		List<ItemDisplayData> allResults = new List<ItemDisplayData>();
		Action <>9__2;
		TweenCallback <>9__1;
		for (sbyte i = 0; i < 6; i += 1)
		{
			sbyte resourceType = i;
			int amount = this._curChoosyResourceValues[(int)resourceType];
			int count = amount / this._choosyCostUnit;
			this._curChoosyResourceValues[(int)resourceType] = 0;
			float duration;
			this.uiCollectResource.PlayResourceCollectAnim(resourceType, out duration);
			TaiwuDomainMethod.AsyncCall.ChoosyGetMaterial(this, resourceType, count, delegate(int offset, RawDataPool dataPool)
			{
				List<ItemDisplayData> results = null;
				Serializer.Deserialize(dataPool, offset, ref results);
				bool flag = results != null;
				if (flag)
				{
					allResults.AddRange(results);
				}
				bool flag2 = resourceType == 5;
				if (flag2)
				{
					float duration = duration;
					TweenCallback callback;
					if ((callback = <>9__1) == null)
					{
						callback = (<>9__1 = delegate()
						{
							ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("ItemList", allResults);
							UIElement.GetItem.SetOnInitArgs(args);
							UIElement getItem = UIElement.GetItem;
							Delegate onShowed = getItem.OnShowed;
							Action b;
							if ((b = <>9__2) == null)
							{
								b = (<>9__2 = delegate()
								{
									this.AnimMask.SetActive(false);
									this.uiCollectResource.Clear();
									this.RefreshChoosyPage();
								});
							}
							getItem.OnShowed = (Action)Delegate.Combine(onShowed, b);
							UIManager.Instance.MaskUI(UIElement.GetItem);
						});
					}
					DOVirtual.DelayedCall(duration, callback, true);
				}
			});
		}
	}

	// Token: 0x06001E15 RID: 7701 RVA: 0x000D7C6C File Offset: 0x000D5E6C
	private void OnOpenChoosyPage(ArgumentBox args)
	{
		this.ShowChoosyPage();
		List<int> resources;
		bool flag = args.Get<List<int>>("Resources", out resources);
		if (flag)
		{
			for (sbyte i = 0; i < 6; i += 1)
			{
				this.ChangeMakeResourceCount(i, true, false, false, resources[(int)i]);
			}
		}
	}

	// Token: 0x06001E16 RID: 7702 RVA: 0x000D7CBC File Offset: 0x000D5EBC
	private void ClearKidnapCharElementDict()
	{
		foreach (Refers slotRefers in this._kidnapAvatarDict.Keys)
		{
			this._kidnapAvatarDict[slotRefers].CharacterId = -1;
			this._kidnapHealthDict[slotRefers].CharacterId = -1;
		}
		this._kidnapAvatarDict.Clear();
		this._kidnapHealthDict.Clear();
		this._kidnapChars.Clear();
	}

	// Token: 0x06001E17 RID: 7703 RVA: 0x000D7D5C File Offset: 0x000D5F5C
	private void OnRenderKidnapChar(int index, Refers slotRefers)
	{
		bool flag = index < 0 || this._kidnapChars == null || index > this._kidnapChars.Count;
		if (!flag)
		{
			CharacterDisplayData charData = this._kidnapChars[index];
			int charId = charData.CharacterId;
			ItemDisplayData ropeData = this._kidnapRopes[index];
			KidnappedCharacter kidnappedChar = this._kidnapInfo.Get(index);
			sbyte resistance = kidnappedChar.ExtraResistance;
			bool flag2 = !this._kidnapAvatarDict.ContainsKey(slotRefers);
			if (flag2)
			{
				this._kidnapAvatarDict.Add(slotRefers, new CharacterAvatar(slotRefers.CGet<Game.Components.Avatar.Avatar>("Avatar"), true));
				this._kidnapHealthDict.Add(slotRefers, new CharacterHealth(slotRefers.CGet<CharacterHealthBar>("CharacterHealthBar")));
			}
			this._kidnapAvatarDict[slotRefers].CharacterId = charId;
			this._kidnapHealthDict[slotRefers].CharacterId = charId;
			string nameText = NameCenter.GetMonasticTitleOrDisplayName(charData, false);
			CommonCharacterNameFrame nameFrame = slotRefers.CGet<CommonCharacterNameFrame>("Name");
			nameFrame.SetName(nameText);
			nameFrame.OnLanguageChange(this.CurLanguageType);
			CommonConfigurableParameterGrid parameterGrid = slotRefers.CGet<CommonConfigurableParameterGrid>("ParameterGrid");
			parameterGrid.Init();
			Refers ageRefers = parameterGrid.GetCellItem(1);
			ageRefers.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.Get(LanguageKey.LK_Char_Age);
			ageRefers.CGet<TextMeshProUGUI>("Value").text = LocalStringManager.GetFormat(LanguageKey.LK_Age, charData.PhysiologicalAge);
			Refers happyRefers = parameterGrid.GetCellItem(2);
			happyRefers.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Happiness);
			happyRefers.CGet<TextMeshProUGUI>("Value").text = CommonUtils.GetHappinessString(HappinessType.GetHappinessType(charData.Happiness));
			Refers favorRefers = parameterGrid.GetCellItem(3);
			favorRefers.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.Get(LanguageKey.LK_Favorability);
			favorRefers.CGet<TextMeshProUGUI>("Value").text = CommonUtils.GetFavorString(charData.FavorabilityToTaiwu);
			int prisonTime = SingletonObject.getInstance<BasicGameData>().CurrDate - kidnappedChar.KidnapBeginDate;
			Refers timeRefers = parameterGrid.GetCellItem(parameterGrid.CellCount - 3);
			timeRefers.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.Get(LanguageKey.LK_PrisonTime);
			timeRefers.CGet<TextMeshProUGUI>("Value").text = prisonTime.ToString();
			Refers resistanceRefers = parameterGrid.GetCellItem(parameterGrid.CellCount - 1);
			resistanceRefers.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.Get(LanguageKey.LK_Kidnap_Resistance_Value);
			resistanceRefers.CGet<TextMeshProUGUI>("Value").text = resistance.ToString().SetColor((resistance > 50) ? "brightred" : "brightblue");
			slotRefers.CGet<CButtonObsolete>("ButtonView").ClearAndAddListener(delegate
			{
				this.ViewPrisoner(charId);
			});
			slotRefers.CGet<CButtonObsolete>("ButtonView").gameObject.SetActive(base.CharacterMenu.CanOperate && !base.CharacterMenu.OpenFromCombatPrepare);
			bool isOnNormalInteractEvent = SingletonObject.getInstance<EventModel>().IsOnNormalInteractEvent;
			bool canInteract = base.CharacterMenu.CanOperate && !base.CharacterMenu.OpenFromCombatPrepare && this.IsTaiwuTeamButNotBeast && !isOnNormalInteractEvent;
			CButtonObsolete interactButton = slotRefers.CGet<CButtonObsolete>("ButtonInteract");
			interactButton.gameObject.SetActive(true);
			interactButton.interactable = canInteract;
			interactButton.ClearAndAddListener(delegate
			{
				this.InteractPrisoner(charId);
			});
			bool canSelectRope = this.IsTaiwuTeamButNotBeast && base.CharacterMenu.CanOperate && base.CharacterMenu.CurrentCharacterIsTaiwu;
			CButtonObsolete buttonRope = slotRefers.CGet<CButtonObsolete>("ButtonRope");
			buttonRope.ClearAndAddListener(delegate
			{
				this.SelectRopeForPrisoner(index);
			});
			buttonRope.interactable = canSelectRope;
			buttonRope.gameObject.SetActive(true);
			CommonItemBack ropeItem = slotRefers.CGet<CommonItemBack>("RopeItem");
			ropeItem.SetData(ropeData, -1);
			ropeItem.SetDisable(!base.CharacterMenu.CanOperate);
			ropeItem.SetInteractable(false);
		}
	}

	// Token: 0x06001E18 RID: 7704 RVA: 0x000D81A8 File Offset: 0x000D63A8
	private void OnKidnapCharHide(Refers slotRefers)
	{
		bool flag = this._kidnapAvatarDict.ContainsKey(slotRefers);
		if (flag)
		{
			this._kidnapAvatarDict[slotRefers].CharacterId = -1;
			this._kidnapAvatarDict.Remove(slotRefers);
		}
		bool flag2 = this._kidnapHealthDict.ContainsKey(slotRefers);
		if (flag2)
		{
			this._kidnapHealthDict[slotRefers].CharacterId = -1;
			this._kidnapHealthDict.Remove(slotRefers);
		}
	}

	// Token: 0x06001E19 RID: 7705 RVA: 0x000D821C File Offset: 0x000D641C
	private void ViewPrisoner(int charId)
	{
		bool activeSelf = base.CharacterMenu.StackView.gameObject.activeSelf;
		if (!activeSelf)
		{
			base.CharacterMenu.StackToNewCharacter(charId);
		}
	}

	// Token: 0x06001E1A RID: 7706 RVA: 0x000D8252 File Offset: 0x000D6452
	private void InteractPrisoner(int charId)
	{
		TaiwuEventDomainMethod.Call.OnInteractKidnappedCharacter(charId);
	}

	// Token: 0x06001E1B RID: 7707 RVA: 0x000D825C File Offset: 0x000D645C
	private void SelectRopeForPrisoner(int index)
	{
		CharacterDisplayData charData = this._kidnapChars[index];
		SelectItemConfig config = SelectItemConfig.CreateSingleSelectConfig(new SelectItemRules
		{
			ItemSubType = 1206,
			OnlyFromInventory = true
		}, delegate(List<SelectedItemData> selectedItems)
		{
			bool flag = selectedItems.Count > 0;
			if (flag)
			{
				ItemKey ropeItemKey = selectedItems[0].ItemData.Key;
				CharacterDomainMethod.Call.ChangeKidnappedCharacterRope(this.CharacterMenu.CurCharacterId, charData.CharacterId, ropeItemKey);
				List<ItemKey> list = new List<ItemKey>(this._kidnapRopes.ConvertAll<ItemKey>((ItemDisplayData e) => e.Key));
				list[index] = ropeItemKey;
				this.RefreshRope(list);
				this.RefreshKidnap();
			}
		}, "", new ESelectItemColumnType?(ESelectItemColumnType.IconAndName | ESelectItemColumnType.Amount | ESelectItemColumnType.Type | ESelectItemColumnType.Value | ESelectItemColumnType.Weight | ESelectItemColumnType.EscapeRate));
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.SetObject("SelectItemConfig", config);
		UIElement.SelectItem.SetOnInitArgs(argBox);
		UIManager.Instance.MaskUI(UIElement.SelectItem);
	}

	// Token: 0x06001E1C RID: 7708 RVA: 0x000D82FC File Offset: 0x000D64FC
	private void RefreshRope(List<ItemKey> ropeItems)
	{
		this._kidnapRopes.Clear();
		foreach (ItemKey itemKey in ropeItems)
		{
			ItemDisplayData itemData = new ItemDisplayData(itemKey.ItemType, itemKey.TemplateId);
			this._kidnapRopes.Add(itemData);
		}
	}

	// Token: 0x06001E1D RID: 7709 RVA: 0x000D8374 File Offset: 0x000D6574
	private void RefreshKidnap()
	{
		this._kidnapScroll.UpdateData(this._kidnapChars.Count);
		this.RefreshKidnapCount();
		this.Element.ShowAfterRefresh();
		bool flag = this._targetItemOperationType == ItemOperationType.EItemOperationType.PutPoisonMaterial;
		if (flag)
		{
			this._targetItemOperationType = ItemOperationType.EItemOperationType.Invalid;
			this._multiplyItemScrollView.EnterPutPoisonMaterialMode();
		}
	}

	// Token: 0x06001E1E RID: 7710 RVA: 0x000D83D0 File Offset: 0x000D65D0
	public void OnPointerEnterPrisoner(CImage image)
	{
		bool flag = !base.CharacterMenu.CanOperate || base.CharacterMenu.OpenFromCombatPrepare;
		if (!flag)
		{
			image.gameObject.SetActive(true);
			image.color = image.color.SetAlpha(0.5f);
		}
	}

	// Token: 0x06001E1F RID: 7711 RVA: 0x000D8423 File Offset: 0x000D6623
	public void OnPointerExitPrisoner(CImage image)
	{
		image.gameObject.SetActive(false);
		image.color = image.color.SetAlpha(0.5f);
	}

	// Token: 0x06001E20 RID: 7712 RVA: 0x000D844C File Offset: 0x000D664C
	private void RefreshKidnapCount()
	{
		bool outOfRange = this._kidnapChars.Count > this._maxKidnapSlotCount;
		string curColor = outOfRange ? "brightred" : "pinkyellow";
		string curCount = this._kidnapChars.Count.ToString().SetColor(curColor);
		this._kidnapPage.CGet<TextMeshProUGUI>("KidnapCountContent").text = LocalStringManager.GetFormat(LanguageKey.LK_Kidnap_Count_Content, curCount, this._maxKidnapSlotCount);
		TooltipInvoker tip = this._kidnapPage.CGet<TooltipInvoker>("KidnapCountTip");
		tip.enabled = outOfRange;
	}

	// Token: 0x06001E25 RID: 7717 RVA: 0x000D86A8 File Offset: 0x000D68A8
	[CompilerGenerated]
	private void <InitMultiplyItemScrollView>g__OnEnter|61_0()
	{
		base.CharacterMenu.IsMultiplySelect = true;
		this._itemPage.CGet<Refers>("Hobby").gameObject.SetActive(false);
		base.CharacterMenu.OnTryClosePage = delegate()
		{
			this._multiplyItemScrollView.TryExitMultiplyMode(delegate
			{
				base.CharacterMenu.OnTryClosePage = null;
			});
		};
		int num = this.CurCharacterIsTaiwu ? 8859 : 8853;
		base.CharacterMenu.EnterFocusMode(null, false, false);
	}

	// Token: 0x06001E28 RID: 7720 RVA: 0x000D8744 File Offset: 0x000D6944
	[CompilerGenerated]
	private void <InitMultiplyItemScrollView>g__OnExit|61_1()
	{
		base.CharacterMenu.IsMultiplySelect = false;
		base.CharacterMenu.ExitFocusMode(true);
		base.CharacterMenu.OnTryClosePage = null;
		this._itemPage.CGet<Refers>("Hobby").gameObject.SetActive(!this.CurCharacterIsTaiwu);
	}

	// Token: 0x06001E2C RID: 7724 RVA: 0x000D8870 File Offset: 0x000D6A70
	[CompilerGenerated]
	internal static void <GroupItems>g__AddGroup|72_1(int index, string groupName, List<ItemDisplayData> itemsToRemove, HashSet<ItemDisplayData> sourceSet, ref UI_CharacterMenuItems.<>c__DisplayClass72_0 A_4)
	{
		bool flag = itemsToRemove.Count <= 0;
		if (!flag)
		{
			GroupedItemScrollView2.ItemGroup group = new GroupedItemScrollView2.ItemGroup(index, groupName);
			foreach (ItemDisplayData item in itemsToRemove)
			{
				group.ItemList.Add(item);
				sourceSet.Remove(item);
			}
			A_4.groups.Add(group);
		}
	}

	// Token: 0x06001E2E RID: 7726 RVA: 0x000D8906 File Offset: 0x000D6B06
	[CompilerGenerated]
	internal static bool <ShowItemOperateMenuIdentify>g__Match|111_1(ItemDisplayData i)
	{
		return i.Key.ItemType == 12 && i.Key.TemplateId == 264;
	}

	// Token: 0x06001E34 RID: 7732 RVA: 0x000D897C File Offset: 0x000D6B7C
	[CompilerGenerated]
	private void <ChangeMakeResourceCount>g__Tick|192_0(ref UI_CharacterMenuItems.<>c__DisplayClass192_0 A_1)
	{
		bool disableOperation = this.DisableOperation;
		if (!disableOperation)
		{
			A_1.curCount = (A_1.isMore ? (A_1.curCount + this._choosyCostUnit) : (A_1.curCount - this._choosyCostUnit));
			A_1.curCount = Mathf.Clamp(A_1.curCount, A_1.minCount, A_1.maxCount);
			this._curChoosyResourceValues[(int)A_1.resourceType] = A_1.curCount;
		}
	}

	// Token: 0x040016B5 RID: 5813
	private static readonly List<sbyte> CanRepairLifeSkillType = new List<sbyte>
	{
		6,
		7,
		10,
		11,
		8,
		9
	};

	// Token: 0x040016B6 RID: 5814
	private static readonly List<sbyte> CanDisassembleLifeSkillType = new List<sbyte>
	{
		6,
		7,
		10,
		11,
		14,
		8
	};

	// Token: 0x040016B7 RID: 5815
	private int _taiwuCharId;

	// Token: 0x040016B8 RID: 5816
	private ResourceMonitor _resourceMonitor;

	// Token: 0x040016B9 RID: 5817
	private LifeSkillMonitor _lifeSkillMonitor;

	// Token: 0x040016BA RID: 5818
	private AgeHealthMonitor _ageMonitor;

	// Token: 0x040016BB RID: 5819
	private AttributeMonitor _attributeMonitor;

	// Token: 0x040016BC RID: 5820
	private EquipCombatSkillMonitor _equipCombatSkillMonitor;

	// Token: 0x040016BD RID: 5821
	private BasicInfoMonitor _basicInfoMonitor;

	// Token: 0x040016BE RID: 5822
	private List<ItemDisplayData> _inventoryItems = new List<ItemDisplayData>();

	// Token: 0x040016BF RID: 5823
	private List<ItemDisplayData> _warehouseItems = new List<ItemDisplayData>();

	// Token: 0x040016C0 RID: 5824
	private List<ItemDisplayData> _treasuryItems = new List<ItemDisplayData>();

	// Token: 0x040016C1 RID: 5825
	private List<SectStoryHeavenlyTreeExtendable> _heavenlyTreeList = new List<SectStoryHeavenlyTreeExtendable>();

	// Token: 0x040016C2 RID: 5826
	private int _maxLoad;

	// Token: 0x040016C3 RID: 5827
	private int _curLoad;

	// Token: 0x040016C4 RID: 5828
	private long _maxWorthCanBeLentToTaiwu;

	// Token: 0x040016C5 RID: 5829
	private ItemDisplayData _currCraftTool;

	// Token: 0x040016C6 RID: 5830
	private ItemOperationType.EItemOperationType _currItemOperation;

	// Token: 0x040016C7 RID: 5831
	private List<ItemDisplayData> _canOperateItems = new List<ItemDisplayData>();

	// Token: 0x040016C8 RID: 5832
	private Refers _itemPage;

	// Token: 0x040016C9 RID: 5833
	private Refers _kidnapPage;

	// Token: 0x040016CA RID: 5834
	private Refers _choosyPage;

	// Token: 0x040016CB RID: 5835
	private GroupedItemScrollView2 _itemScroll;

	// Token: 0x040016CC RID: 5836
	private ItemScrollViewForCommonTableRow _selectedItemScroll;

	// Token: 0x040016CD RID: 5837
	private Transform _itemPageParent;

	// Token: 0x040016CE RID: 5838
	[TupleElementNames(new string[]
	{
		"focusingItemView",
		"parent",
		"sibling"
	})]
	private ValueTuple<CommonTableRowForItem, Transform, int> _focusingTuple;

	// Token: 0x040016CF RID: 5839
	private KidnappedCharacterList _kidnapInfo;

	// Token: 0x040016D0 RID: 5840
	private List<CharacterDisplayData> _kidnapChars = new List<CharacterDisplayData>();

	// Token: 0x040016D1 RID: 5841
	private List<ItemDisplayData> _kidnapRopes = new List<ItemDisplayData>();

	// Token: 0x040016D2 RID: 5842
	private readonly Dictionary<Refers, CharacterAvatar> _kidnapAvatarDict = new Dictionary<Refers, CharacterAvatar>();

	// Token: 0x040016D3 RID: 5843
	private readonly Dictionary<Refers, CharacterHealth> _kidnapHealthDict = new Dictionary<Refers, CharacterHealth>();

	// Token: 0x040016D4 RID: 5844
	private int _maxKidnapSlotCount;

	// Token: 0x040016D5 RID: 5845
	private InfinityScrollLegacy _kidnapScroll;

	// Token: 0x040016D6 RID: 5846
	private CharacterLoveAndHateItemInfo _curCharLoveAndHateItemInfo;

	// Token: 0x040016D7 RID: 5847
	private CharacterDisplayData _currentCharacterDisplayData;

	// Token: 0x040016D8 RID: 5848
	private CharacterDisplayData _taiwuCharacterDisplayData;

	// Token: 0x040016D9 RID: 5849
	public static readonly List<ItemSortAndFilter.ItemFilterType> DisassembleFilterTypes = new List<ItemSortAndFilter.ItemFilterType>
	{
		ItemSortAndFilter.ItemFilterType.Invalid,
		ItemSortAndFilter.ItemFilterType.Equip,
		ItemSortAndFilter.ItemFilterType.Material,
		ItemSortAndFilter.ItemFilterType.Other
	};

	// Token: 0x040016DA RID: 5850
	public static readonly List<EMainFilterKeys> DisassembleFilterTypes2 = new List<EMainFilterKeys>
	{
		EMainFilterKeys.Equip,
		EMainFilterKeys.Material,
		EMainFilterKeys.Misc
	};

	// Token: 0x040016DB RID: 5851
	private readonly Dictionary<string, List<GameObject>> _makeResourceSpriteDict = new Dictionary<string, List<GameObject>>();

	// Token: 0x040016DC RID: 5852
	private MultiplyItemScrollView2 _multiplyItemScrollView;

	// Token: 0x040016DD RID: 5853
	private List<ItemDisplayData> _equipItems = new List<ItemDisplayData>();

	// Token: 0x040016DE RID: 5854
	private ItemOperationType.EItemOperationType _targetItemOperationType;

	// Token: 0x040016DF RID: 5855
	private ItemDisplayData _selectedToTransferItemData;

	// Token: 0x040016E0 RID: 5856
	private int _exp;

	// Token: 0x040016E1 RID: 5857
	private bool _chickenMapInteractable;

	// Token: 0x040016E2 RID: 5858
	private int _lastUseChickenMapAreaId = -1;

	// Token: 0x040016E3 RID: 5859
	private bool _allChickenInTaiwuVillage = false;

	// Token: 0x040016E4 RID: 5860
	private const int HeavenlyTreeNeedDistance = 6;

	// Token: 0x040016E5 RID: 5861
	private bool _curIdentifySuccess;

	// Token: 0x040016E6 RID: 5862
	private bool _curIdentifiedResultHasPoison;

	// Token: 0x040016E7 RID: 5863
	private Action _curIdentifiedResultAction;

	// Token: 0x040016E8 RID: 5864
	private readonly List<ItemKey> _identifyKeyList = new List<ItemKey>();

	// Token: 0x040016E9 RID: 5865
	private string _curEffectName;

	// Token: 0x040016EA RID: 5866
	private readonly int _choosyCostUnit = GlobalConfig.Instance.ChoosyResourceBaseCost;

	// Token: 0x040016EB RID: 5867
	private readonly int[] _curChoosyResourceValues = new int[6];

	// Token: 0x040016EC RID: 5868
	private const int MinResourceValue = 0;

	// Token: 0x040016ED RID: 5869
	private const int MaxResourceSpriteCount = 100;
}
