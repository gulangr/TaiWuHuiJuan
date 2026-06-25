using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using CharacterDataMonitor;
using CommonSortAndFilterLegacy;
using CommonSortAndFilterLegacy.Item;
using Config;
using FrameWork;
using Game.Views;
using Game.Views.CharacterMenu;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020001DB RID: 475
public class UI_UsingMedicineItem : UIBase
{
	// Token: 0x17000323 RID: 803
	// (get) Token: 0x06001F0C RID: 7948 RVA: 0x000E1883 File Offset: 0x000DFA83
	public ViewCharacterMenu CharacterMenu
	{
		get
		{
			return UIElement.CharacterMenu.UiBaseAs<ViewCharacterMenu>();
		}
	}

	// Token: 0x06001F0D RID: 7949 RVA: 0x000E1890 File Offset: 0x000DFA90
	public override void OnInit(ArgumentBox argsBox)
	{
		base.GetComponent<CanvasGroup>().alpha = 0f;
		short type;
		bool flag = argsBox != null && argsBox.Get("UsingMedicineItemType", out type);
		if (flag)
		{
			this._usingMedicineItemType = type;
		}
		else
		{
			this._usingMedicineItemType = UsingMedicineItemType.Invalid;
		}
		ItemKey selectedItemKey;
		bool flag2 = argsBox != null && argsBox.Get<ItemKey>("SelectedItemKey", out selectedItemKey);
		if (flag2)
		{
			this._selectedItemKey = selectedItemKey;
		}
		else
		{
			this._selectedItemKey = ItemKey.Invalid;
		}
		this._charAttrDataView = base.CGet<CharacterAttributeDataView>("CharacterAttributeView");
		this._itemPage = base.CGet<Refers>("ItemPage");
		this._itemPageParent = this._itemPage.transform.parent;
		this._itemScroll = this._itemPage.CGet<ItemScrollViewForCommonTableRow>("ItemScrollView");
		this._itemScroll.Init(ESortAndFilterControllerType.UsingMedicine, null);
		this._itemScroll.SetItemList(ref this._medicineItemList, true, "UsingMedicineItem", new Action<ItemDisplayData, CommonTableRowForItem>(this.OnRenderItem), null, true);
		this._charAttrDataView.gameObject.SetActive(true);
		this._autoUseMedicineButton = this._itemPage.CGet<CButtonObsolete>("AutoUseMedicineButton");
		int[] lastPoisonValue;
		bool flag3 = argsBox != null && argsBox.Get<int[]>("LastPoisonValue", out lastPoisonValue);
		if (flag3)
		{
			this._charAttrDataView.LastPoisonValueArray = lastPoisonValue;
		}
		else
		{
			this._charAttrDataView.LastPoisonValueArray = null;
		}
		bool flag4 = argsBox == null || !argsBox.Get("CurrentCharacterId", out this._charId);
		if (flag4)
		{
			this._charId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		}
		this.NeedDataListenerId = true;
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
	}

	// Token: 0x06001F0E RID: 7950 RVA: 0x000E1A44 File Offset: 0x000DFC44
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
					bool flag = notification.DomainId == 4;
					if (flag)
					{
						bool flag2 = notification.MethodId == 27;
						if (flag2)
						{
							List<ItemDisplayData> data = null;
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref data);
							this._medicineItemList.Clear();
							bool flag3 = data != null;
							if (flag3)
							{
								foreach (ItemDisplayData item in data)
								{
									bool flag4 = item.Key.ItemType == 8 || ItemTemplateHelper.IsTianJieFuLu(item.Key.ItemType, item.Key.TemplateId) || CommonUtils.CanMaterialEat(item.Key.ItemType, item.Key.TemplateId, this._charId);
									if (flag4)
									{
										this._medicineItemList.Add(item);
									}
								}
							}
							bool flag5 = !this._characterDataViewInt;
							if (flag5)
							{
								this._charAttrDataView.SetTabState(1);
							}
							this._characterDataViewInt = true;
							this._charAttrDataView.CGet<GameObject>("ToggleSamsara").SetActive(false);
							this._charAttrDataView.SetCurrentCharacterId(this._charId);
							SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
							{
								this.RefreshItemListIsLocked();
								this._itemScroll.SetItemList(ref this._medicineItemList, false, null, null, null, true);
								this._itemScroll.InfinityScroll.InitPageCount();
								this.OnUsingMedicineItemSwitch();
								bool flag11 = !this._selectedItemKey.Equals(ItemKey.Invalid);
								if (flag11)
								{
									int targetDataIndex = this._itemScroll.OutputItemList.FindIndex((ItemDisplayData d) => d.ContainsItemKey(this._selectedItemKey));
									this._itemScroll.InfinityScroll.ScrollTo(targetDataIndex, 0f);
									this._itemScroll.InfinityScroll.OnRenderEnd = delegate()
									{
										CommonTableRowForItem targetView = this._itemScroll.FindActiveItem(this._selectedItemKey, false);
										this._selectedItemKey = ItemKey.Invalid;
										this.EnterSelectInjuryPart(targetView.Data, targetView, 1);
										this._itemScroll.InfinityScroll.OnRenderEnd = null;
										base.GetComponent<CanvasGroup>().alpha = 1f;
									};
								}
								else
								{
									base.GetComponent<CanvasGroup>().alpha = 1f;
								}
								this.Element.ShowAfterRefresh();
							});
						}
					}
				}
			}
			else
			{
				DataUid uid = notification.Uid;
				bool flag6 = uid.DomainId == 4 && uid.DataId == 0;
				if (flag6)
				{
					bool flag7 = uid.SubId1 == 104U;
					if (flag7)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._curLoad);
						this.OnInventoryLoadChange();
					}
					else
					{
						bool flag8 = uid.SubId1 == 103U;
						if (flag8)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._maxLoad);
							this.OnInventoryLoadChange();
						}
					}
				}
				else
				{
					bool flag9 = uid.DomainId == 5;
					if (flag9)
					{
						bool flag10 = uid.DataId == 22;
						if (flag10)
						{
							int moveTimeCostPercent = 0;
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref moveTimeCostPercent);
							this._itemPage.CGet<TooltipInvoker>("LoadOverflowTips").PresetParam[1] = LocalStringManager.GetFormat(LanguageKey.LK_Inventory_Overflow_Tips, moveTimeCostPercent - 100).ColorReplace();
						}
					}
				}
			}
		}
	}

	// Token: 0x06001F0F RID: 7951 RVA: 0x000E1D34 File Offset: 0x000DFF34
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)this._charId, new uint[]
		{
			104U,
			103U,
			35U,
			36U,
			50U,
			2U
		}));
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 22, ulong.MaxValue, null));
	}

	// Token: 0x06001F10 RID: 7952 RVA: 0x000E1D84 File Offset: 0x000DFF84
	protected override void OnClick(Transform btn)
	{
		base.OnClick(btn);
		bool flag = btn.name == "BackButton";
		if (flag)
		{
			this.QuickHide();
		}
		else
		{
			bool flag2 = btn.name == "AutoUseMedicineButton";
			if (flag2)
			{
				this.OnClickAutoUseMedicine();
			}
		}
	}

	// Token: 0x06001F11 RID: 7953 RVA: 0x000E1DD8 File Offset: 0x000DFFD8
	public override void QuickHide()
	{
		AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
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

	// Token: 0x06001F12 RID: 7954 RVA: 0x000E1E1D File Offset: 0x000E001D
	private void OnEnable()
	{
		GEvent.Add(UiEvents.UsingMedicineItemSwitch, new GEvent.Callback(this.OnUsingMedicineItemSwitch));
	}

	// Token: 0x06001F13 RID: 7955 RVA: 0x000E1E3C File Offset: 0x000E003C
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.UsingMedicineItemSwitch, new GEvent.Callback(this.OnUsingMedicineItemSwitch));
		bool flag = UIManager.Instance.IsElementActive(UIElement.SkillBreakPlate);
		if (flag)
		{
			this._charAttrDataView.CGet<GameObject>("ToggleSamsara").SetActive(false);
		}
		else
		{
			this._charAttrDataView.CGet<GameObject>("ToggleSamsara").SetActive(true);
		}
		this.OnExitFocusMode();
		this._characterDataViewInt = false;
		this._medicineItemList.Clear();
		this._itemScroll.SetItemList(ref this._medicineItemList, false, null, null, null, true);
		this.CharacterMenu.Injury.RefreshAllHealBtn(this._charId, false);
		this._charAttrDataView.HideInfectNotice(true, true);
	}

	// Token: 0x06001F14 RID: 7956 RVA: 0x000E1EFF File Offset: 0x000E00FF
	private void OnListenerIdReady()
	{
		CharacterDomainMethod.Call.GetAllInventoryItems(this.Element.GameDataListenerId, this._charId);
		this._itemScroll.SetCharId(this._charId);
	}

	// Token: 0x06001F15 RID: 7957 RVA: 0x000E1F2C File Offset: 0x000E012C
	private void OnInventoryLoadChange()
	{
		string currLoad = ((float)this._curLoad / 100f).ToString("f1");
		this._itemPage.CGet<TextMeshProUGUI>("LoadText").text = string.Format("{0}/{1:f1}", currLoad, (float)this._maxLoad / 100f);
		this._itemPage.CGet<TooltipInvoker>("LoadOverflowTips").enabled = (this.CharacterMenu.IsTaiwuTeam && this._curLoad > this._maxLoad);
	}

	// Token: 0x06001F16 RID: 7958 RVA: 0x000E1FBC File Offset: 0x000E01BC
	private void CallRefreshItems()
	{
		CharacterDomainMethod.Call.GetAllInventoryItems(this.Element.GameDataListenerId, this._charId);
		GEvent.OnEvent(UiEvents.OnRefreshCharacterMenuItem, null);
	}

	// Token: 0x06001F17 RID: 7959 RVA: 0x000E1FE7 File Offset: 0x000E01E7
	private void SetItemScrollViewCanScroll(bool canScroll)
	{
		this._itemScroll.GetComponent<CScrollRectLegacy>().SetScrollEnable(canScroll);
	}

	// Token: 0x06001F18 RID: 7960 RVA: 0x000E1FFC File Offset: 0x000E01FC
	private void SetEatItemInfectNotice(bool show, ItemDisplayData itemDisplayData = null, int amount = 1)
	{
		bool flag = show && itemDisplayData != null;
		if (flag)
		{
			this._charAttrDataView.ShowInfectNotice(itemDisplayData, amount);
		}
		else
		{
			this._charAttrDataView.HideInfectNotice(true, true);
		}
	}

	// Token: 0x06001F19 RID: 7961 RVA: 0x000E203C File Offset: 0x000E023C
	private void OnClickItem(ItemDisplayData itemData, CommonTableRowForItem itemView)
	{
		bool flag = !this.CharacterMenu.CanOperate;
		if (!flag)
		{
			bool flag2 = this.CharacterMenu.IsTaiwuTeam && !this._itemScroll.FindActiveItem(itemData.Key, false).IsLocked;
			if (flag2)
			{
				bool flag3 = CommonUtils.IsRecoverInjuryMainMedicineItem(itemData.RealKey);
				if (flag3)
				{
					this.OnClickEatItem(itemData, itemView);
				}
				else
				{
					this.ShowItemOperateMenu(itemData, itemView);
				}
			}
		}
	}

	// Token: 0x06001F1A RID: 7962 RVA: 0x000E20B4 File Offset: 0x000E02B4
	private void ShowItemOperateMenu(ItemDisplayData itemData, CommonTableRowForItem itemView)
	{
		List<ViewPopupMenu.BtnData> btnList = new List<ViewPopupMenu.BtnData>();
		bool isTaiwuTeam = this.CharacterMenu.IsTaiwuTeam;
		if (isTaiwuTeam)
		{
			ValueTuple<int, string> valueTuple = CommonUtils.CalculateCountAndTip(this._charId, itemData.Key, itemData.Amount);
			int count = valueTuple.Item1;
			string reason = valueTuple.Item2;
			bool canEatMore = count > 0;
			bool flag = itemData.Key.ItemType == 8;
			if (flag)
			{
				bool hasAttributeToTopical = this._charAttrDataView.HasAttributeToTopical(itemData.Key);
				bool interactable = CommonUtils.GetMedicineItemMenuInteractable(itemData.Key, canEatMore, false, hasAttributeToTopical, ref reason);
				string btnName = CommonUtils.GetCanEatItemButtonName(itemData.RealKey);
				ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(btnName, interactable, EItemMenuDisplayOrder.Eat, delegate()
				{
					this.OnClickEatItem(itemData, itemView);
				}, delegate()
				{
					this.SetEatItemInfectNotice(true, itemData, 1);
				}, delegate()
				{
					this.SetEatItemInfectNotice(false, null, 1);
				}, false);
				btnList.Add(btnData);
				bool flag2 = !interactable;
				if (flag2)
				{
					btnData.SetTip("", reason);
				}
			}
			else
			{
				bool flag3 = CommonUtils.CanItemEat(itemData.Key.ItemType, itemData.Key.TemplateId, this._charId);
				if (flag3)
				{
					bool flag4 = ItemTemplateHelper.IsTianJieFuLu(itemData.Key.ItemType, (short)itemData.Key.ItemType);
					if (flag4)
					{
						bool isEnough = itemData.Amount >= ItemTemplateHelper.GetTianJieFuLuCountUnit();
						ViewPopupMenu.BtnData innerButton = new ViewPopupMenu.BtnData(LocalStringManager.Get(LanguageKey.LK_Eat_Item), canEatMore && isEnough, EItemMenuDisplayOrder.Eat, delegate()
						{
							this.OnClickEatItem(itemData, itemView);
						}, delegate()
						{
							this.SetEatItemInfectNotice(true, itemData, 1);
						}, delegate()
						{
							this.SetEatItemInfectNotice(false, null, 1);
						}, false);
						bool flag5 = !isEnough;
						if (flag5)
						{
							innerButton.SetTip(string.Empty, LanguageKey.LK_Mousetip_TianjieFulu_NotEnough.Tr().SetColor("brightred"));
						}
						btnList.Add(innerButton);
					}
					else
					{
						btnList.Add(new ViewPopupMenu.BtnData(LocalStringManager.Get(LanguageKey.LK_Eat_Item), canEatMore, EItemMenuDisplayOrder.Eat, delegate()
						{
							this.OnClickEatItem(itemData, itemView);
						}, null, null, false));
					}
				}
			}
		}
		bool flag6 = btnList.Count > 0;
		if (flag6)
		{
			this._itemScroll.SetItemToPopupMenuMode(itemData.Key, btnList, delegate
			{
				this.OnExitFocusMode();
			});
			this.HighLightItemView(itemView);
		}
	}

	// Token: 0x06001F1B RID: 7963 RVA: 0x000E2340 File Offset: 0x000E0540
	private void OnRenderItem(ItemDisplayData itemData, CommonTableRowForItem itemView)
	{
		string tipContent;
		bool isLock = this.CheckItemIsLocked(itemData, out tipContent);
		itemView.IsLocked = isLock;
		bool flag = tipContent.IsNullOrEmpty();
		if (flag)
		{
			itemView.HideInteractionState();
		}
		else
		{
			itemView.SetItemNotCanSelectReason(tipContent.ColorReplace());
		}
		bool flag2 = !isLock;
		if (flag2)
		{
			itemView.SetClickEvent(delegate
			{
				this.OnClickItem(itemData, itemView);
			});
		}
	}

	// Token: 0x06001F1C RID: 7964 RVA: 0x000E23D4 File Offset: 0x000E05D4
	private bool CheckItemIsLocked(ItemDisplayData itemData, out string tipContent)
	{
		tipContent = string.Empty;
		bool flag = this.CharacterMenu.Element.IsShowing && !this.CharacterMenu.CanOperate;
		return flag || UI_UsingMedicineItem.CheckEatItemIsLocked(itemData, this._charId, (int)this._usingMedicineItemType, this._charAttrDataView, out tipContent);
	}

	// Token: 0x06001F1D RID: 7965 RVA: 0x000E2434 File Offset: 0x000E0634
	public static bool CheckEatItemIsLocked(ItemDisplayData itemData, int charId, int usingMedicineItemType, CharacterAttributeDataView charAttrDataView, out string tipContent)
	{
		tipContent = string.Empty;
		bool isLocked = UI_UsingMedicineItem.CheckMedicineItemIsLocked(itemData, usingMedicineItemType, charAttrDataView, out tipContent);
		bool flag = isLocked;
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			ValueTuple<int, string> valueTuple = CommonUtils.CalculateCountAndTip(charId, itemData.Key, itemData.Amount);
			int count = valueTuple.Item1;
			string reason = valueTuple.Item2;
			bool canEatMore = count > 0;
			bool flag2 = !canEatMore;
			if (flag2)
			{
				tipContent = reason;
				result = true;
			}
			else
			{
				bool flag3 = itemData.RealKey.ItemType != 8 && usingMedicineItemType > (int)UsingMedicineItemType.Invalid;
				if (flag3)
				{
					tipContent = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_Type_Different);
					result = true;
				}
				else
				{
					result = false;
				}
			}
		}
		return result;
	}

	// Token: 0x06001F1E RID: 7966 RVA: 0x000E24D4 File Offset: 0x000E06D4
	private static bool CheckMedicineItemIsLocked(ItemDisplayData itemData, int usingMedicineItemType, CharacterAttributeDataView charAttrDataView, out string tipContent)
	{
		tipContent = string.Empty;
		bool flag = itemData.Key.ItemType != 8;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			UI_UsingMedicineItem.<>c__DisplayClass38_0 CS$<>8__locals1;
			CS$<>8__locals1.medicineItem = Medicine.Instance[itemData.Key.TemplateId];
			CS$<>8__locals1.isCombat = UIManager.Instance.IsElementActive(UIElement.Combat);
			EMedicineEffectType effectType = CS$<>8__locals1.medicineItem.EffectType;
			bool flag2 = effectType == EMedicineEffectType.RecoverInnerInjury || effectType <= EMedicineEffectType.RecoverOuterInjury;
			if (flag2)
			{
				bool flag3 = usingMedicineItemType > (int)UsingMedicineItemType.Invalid && (usingMedicineItemType < (int)UsingMedicineItemType.BodyPartTypeChest || usingMedicineItemType > (int)UsingMedicineItemType.BodyPartTypeRightLeg);
				if (flag3)
				{
					tipContent = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_Type_Different);
					result = true;
				}
				else
				{
					result = !charAttrDataView.IsRecoverInnerOuterMedicineCanUse(itemData.Key.TemplateId, out tipContent);
				}
			}
			else
			{
				bool flag4 = CS$<>8__locals1.medicineItem.EffectType == EMedicineEffectType.DetoxPoison;
				if (flag4)
				{
					bool flag5 = usingMedicineItemType > (int)UsingMedicineItemType.Invalid && (usingMedicineItemType < (int)UsingMedicineItemType.PoisonTypeHot || usingMedicineItemType > (int)UsingMedicineItemType.PoisonTypeIllusory);
					if (flag5)
					{
						tipContent = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_Type_Different);
						result = true;
					}
					else
					{
						UI_UsingMedicineItem.<>c__DisplayClass38_1 CS$<>8__locals2;
						CS$<>8__locals2.monitor = charAttrDataView.GetInjuryPoisonMonitor();
						bool flag6 = CS$<>8__locals2.monitor == null;
						if (flag6)
						{
							result = false;
						}
						else
						{
							bool flag7 = usingMedicineItemType >= (int)UsingMedicineItemType.PoisonTypeHot && usingMedicineItemType <= (int)UsingMedicineItemType.PoisonTypeIllusory;
							if (flag7)
							{
								int poisonType = usingMedicineItemType - (int)UsingMedicineItemType.PoisonTypeHot;
								result = UI_UsingMedicineItem.<CheckMedicineItemIsLocked>g__CheckIsLockedForDetoxPoison|38_0((sbyte)poisonType, out tipContent, ref CS$<>8__locals1, ref CS$<>8__locals2);
							}
							else
							{
								result = UI_UsingMedicineItem.<CheckMedicineItemIsLocked>g__CheckIsLockedForDetoxPoison|38_0(CS$<>8__locals1.medicineItem.PoisonType, out tipContent, ref CS$<>8__locals1, ref CS$<>8__locals2);
							}
						}
					}
				}
				else
				{
					bool flag8 = CS$<>8__locals1.medicineItem.EffectType == EMedicineEffectType.ApplyPoison;
					if (flag8)
					{
						bool flag9 = usingMedicineItemType <= (int)UsingMedicineItemType.Invalid;
						if (flag9)
						{
							result = false;
						}
						else
						{
							bool flag10 = usingMedicineItemType < (int)UsingMedicineItemType.PoisonTypeHot || usingMedicineItemType > (int)UsingMedicineItemType.PoisonTypeIllusory;
							if (flag10)
							{
								tipContent = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_Type_Different);
								result = true;
							}
							else
							{
								int poisonType2 = usingMedicineItemType - (int)UsingMedicineItemType.PoisonTypeHot;
								bool flag11 = (int)CS$<>8__locals1.medicineItem.PoisonType != poisonType2;
								if (flag11)
								{
									tipContent = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_Type_Different);
									result = true;
								}
								else
								{
									result = false;
								}
							}
						}
					}
					else
					{
						bool flag12 = CS$<>8__locals1.medicineItem.EffectType == EMedicineEffectType.RecoverHealth;
						if (flag12)
						{
							bool flag13 = usingMedicineItemType > (int)UsingMedicineItemType.Invalid && usingMedicineItemType != (int)UsingMedicineItemType.Health;
							if (flag13)
							{
								tipContent = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_Type_Different);
								result = true;
							}
							else
							{
								bool flag14 = CS$<>8__locals1.medicineItem.Duration == 0;
								if (flag14)
								{
									AgeHealthMonitor health = charAttrDataView.GetAgeHealthMonitor();
									bool flag15 = (health == null || health.LeftMaxHealth <= health.Health) && !CS$<>8__locals1.isCombat;
									if (flag15)
									{
										tipContent = LanguageKey.LK_UsingMedicine_Tip_NoNeed.Tr();
										return true;
									}
								}
								result = false;
							}
						}
						else
						{
							bool flag16 = CS$<>8__locals1.medicineItem.EffectType == EMedicineEffectType.ChangeDisorderOfQi;
							if (flag16)
							{
								bool flag17 = usingMedicineItemType > (int)UsingMedicineItemType.Invalid && usingMedicineItemType != (int)UsingMedicineItemType.DisorderOfQi;
								if (flag17)
								{
									tipContent = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_Type_Different);
									result = true;
								}
								else
								{
									bool flag18 = CS$<>8__locals1.medicineItem.Duration == 0;
									if (flag18)
									{
										DisorderOfQiMonitor disorderOfQiMonitor = charAttrDataView.GetDisorderOfQiMonitor();
										bool flag19 = (disorderOfQiMonitor == null || disorderOfQiMonitor.DisorderOfQi <= 0) && !CS$<>8__locals1.isCombat;
										if (flag19)
										{
											tipContent = LanguageKey.LK_UsingMedicine_Tip_NoNeed.Tr();
											return true;
										}
									}
									result = false;
								}
							}
							else
							{
								bool flag20 = usingMedicineItemType > (int)UsingMedicineItemType.Invalid;
								if (flag20)
								{
									tipContent = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_Type_Different);
									result = true;
								}
								else
								{
									result = false;
								}
							}
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06001F1F RID: 7967 RVA: 0x000E2884 File Offset: 0x000E0A84
	private void RefreshItemListIsLocked()
	{
		foreach (ItemDisplayData itemData in this._medicineItemList)
		{
			string text;
			itemData.Interactable = !this.CheckItemIsLocked(itemData, out text);
		}
	}

	// Token: 0x06001F20 RID: 7968 RVA: 0x000E28E8 File Offset: 0x000E0AE8
	private void OnClickAutoUseMedicine()
	{
		StringBuilder names = EasyPool.Get<StringBuilder>();
		foreach (KeyValuePair<ItemKey, int> keyValuePair in this._autoUseMedicineInventory.Items)
		{
			ItemKey itemKey2;
			int num;
			keyValuePair.Deconstruct(out itemKey2, out num);
			ItemKey itemKey = itemKey2;
			int amount = num;
			names.Append(string.Format("{0}x{1} ", ItemTemplateHelper.GetName(itemKey.ItemType, itemKey.TemplateId), amount));
		}
		int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		string title = LocalStringManager.Get(LanguageKey.LK_AutoUseMedicine_Title);
		string content = LocalStringManager.GetFormat(LanguageKey.LK_AutoUseMedicine_Content, names.ToString());
		CommonUtils.ShowConfirmDialog(title, content, delegate
		{
			foreach (KeyValuePair<ItemKey, int> keyValuePair2 in this._autoUseMedicineInventory.Items)
			{
				ItemKey itemKey4;
				int num2;
				keyValuePair2.Deconstruct(out itemKey4, out num2);
				ItemKey itemKey3 = itemKey4;
				int amount2 = num2;
				ItemDisplayData itemData = new ItemDisplayData(itemKey3, amount2);
				EatingUtils.TryRequestAddEatingItems(taiwuCharId, itemData, amount2, null, false, null);
			}
			this.CallRefreshItems();
		}, null, EDialogType.None);
		EasyPool.Free<StringBuilder>(names);
	}

	// Token: 0x06001F21 RID: 7969 RVA: 0x000E29DC File Offset: 0x000E0BDC
	private void OnClickEatItem(ItemDisplayData itemData, CommonTableRowForItem itemView)
	{
		UI_UsingMedicineItem.<>c__DisplayClass41_0 CS$<>8__locals1 = new UI_UsingMedicineItem.<>c__DisplayClass41_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.itemData = itemData;
		bool flag = CommonUtils.IsRecoverInjuryMainMedicineItem(CS$<>8__locals1.itemData.RealKey);
		if (flag)
		{
			bool isSelectInjuryPart = this._charAttrDataView.IsSelectInjuryPart;
			if (isSelectInjuryPart)
			{
				this._charAttrDataView.ExitSelectInjuryPart();
			}
			else
			{
				this.EnterSelectInjuryPart(CS$<>8__locals1.itemData, itemView, 1);
			}
		}
		else
		{
			ValueTuple<int, string> valueTuple = CommonUtils.CalculateCountAndTip(this._charId, CS$<>8__locals1.itemData.Key, CS$<>8__locals1.itemData.Amount);
			int limitCount = valueTuple.Item1;
			string reason = valueTuple.Item2;
			bool flag2 = CS$<>8__locals1.itemData.Amount > 1 && ItemTemplateHelper.CanUseMultiple(CS$<>8__locals1.itemData.Key);
			if (flag2)
			{
				int index = this._itemScroll.OutputItemList.IndexOf(CS$<>8__locals1.itemData);
				this._itemScroll.SetItemToSelectCountMode(index, this._focusingTuple.Item1, delegate(int count)
				{
					bool flag3 = count <= 0;
					if (flag3)
					{
						base.<OnClickEatItem>g__Cancel|1();
					}
					else
					{
						base.<OnClickEatItem>g__Confirm|0(count);
					}
				}, new Action(CS$<>8__locals1.<OnClickEatItem>g__Cancel|1), 1, limitCount, 1, LocalStringManager.Get(LanguageKey.LK_SelectCount_Limit_Eat), false, delegate(int count)
				{
					CS$<>8__locals1.<>4__this.SetEatItemInfectNotice(true, CS$<>8__locals1.itemData, count);
				}, null, -1);
			}
			else
			{
				CS$<>8__locals1.<OnClickEatItem>g__Confirm|0(1);
			}
		}
	}

	// Token: 0x06001F22 RID: 7970 RVA: 0x000E2B14 File Offset: 0x000E0D14
	private void EnterSelectInjuryPart(ItemDisplayData itemData, CommonTableRowForItem itemView, int count)
	{
		PointerTrigger pointerTrigger = itemView.GetComponent<PointerTrigger>();
		pointerTrigger.IgnoreOnDisableTrigger = true;
		pointerTrigger.enabled = false;
		this.HighLightItemView(itemView);
		GameObject closeButton = base.CGet<GameObject>("CloseButton");
		closeButton.SetActive(false);
		Action <>9__3;
		this._charAttrDataView.EnterSelectInjuryPart(itemData, delegate(List<sbyte> targetBodyParts)
		{
			base.<EnterSelectInjuryPart>g__Cancel|2();
			AudioManager.Instance.PlaySound("ui_drug_drink", false, false);
			int charId = this._charId;
			ITradeableContent itemData2 = itemData;
			int count2 = count;
			Action onRequest;
			if ((onRequest = <>9__3) == null)
			{
				onRequest = (<>9__3 = delegate()
				{
					this._charAttrDataView.DelayRefreshOnEatItemSend();
					this._charAttrDataView.RefreshAllHealBtn(this._charId, false);
					LifeRecordsController instance = SingletonObject.getInstance<LifeRecordsController>();
					if (instance != null)
					{
						instance.RemoveLatestYearRecordCache();
					}
					this.CallRefreshItems();
				});
			}
			EatingUtils.TryRequestAddEatingItems(charId, itemData2, count2, onRequest, false, targetBodyParts);
		}, delegate
		{
			base.<EnterSelectInjuryPart>g__Cancel|2();
			this.OnUsingMedicineItemSwitch();
		});
	}

	// Token: 0x06001F23 RID: 7971 RVA: 0x000E2BB4 File Offset: 0x000E0DB4
	private void OnUsingMedicineItemSwitch(ArgumentBox argbox)
	{
		argbox.Get("UsingMedicineItemType", out this._usingMedicineItemType);
		this.OnUsingMedicineItemSwitch();
	}

	// Token: 0x06001F24 RID: 7972 RVA: 0x000E2BD0 File Offset: 0x000E0DD0
	private void OnUsingMedicineItemSwitch()
	{
		this._charAttrDataView.SelectPartByUsingMedicineItemType(this._usingMedicineItemType);
		int toggleIndex = -1;
		bool flag = UsingMedicineItemType.IsHurt(this._usingMedicineItemType);
		if (flag)
		{
			sbyte injuryType = (sbyte)this._usingMedicineItemType;
			ValueTuple<sbyte, sbyte> valueTuple = this._charAttrDataView.GetInjuryPoisonMonitor().Injuries.Get(injuryType);
			sbyte outer = valueTuple.Item1;
			sbyte inner = valueTuple.Item2;
			bool flag2 = outer > 0 || inner == 0;
			if (flag2)
			{
				toggleIndex = EMedicineSubFilterKeys.Outer.ToInt();
			}
			else
			{
				bool flag3 = inner > 0;
				if (flag3)
				{
					toggleIndex = EMedicineSubFilterKeys.Inner.ToInt();
				}
			}
		}
		else
		{
			bool flag4 = UsingMedicineItemType.IsPoison(this._usingMedicineItemType);
			if (flag4)
			{
				toggleIndex = EMedicineSubFilterKeys.Poison.ToInt();
			}
			else
			{
				bool flag5 = UsingMedicineItemType.IsHealth(this._usingMedicineItemType);
				if (flag5)
				{
					toggleIndex = EMedicineSubFilterKeys.Health.ToInt();
				}
				else
				{
					bool flag6 = UsingMedicineItemType.IsDisorderOfQi(this._usingMedicineItemType);
					if (flag6)
					{
						toggleIndex = EMedicineSubFilterKeys.Disorder.ToInt();
					}
				}
			}
		}
		this._itemScroll.SortAndFilterController.SetToggleIsOn(EFilterLine.MainFilter.ToInt(), EMainFilterKeys.Medicine.ToInt());
		this._itemScroll.SortAndFilterController.SetDropdownOption(EFilterLine.MedicineFilter.ToInt(), 0, toggleIndex);
		this.RefreshItemListIsLocked();
		this._itemScroll.SortAndFilterController.SetDataList(this._medicineItemList, true);
	}

	// Token: 0x06001F25 RID: 7973 RVA: 0x000E2D28 File Offset: 0x000E0F28
	private void OnExitFocusMode()
	{
		this._currItemOperation = ItemOperationType.EItemOperationType.Invalid;
		this._canOperateItems.Clear();
		this.SetItemScrollViewCanScroll(true);
		this.CancelHighLightItemView();
		this._itemPage.transform.SetParent(this._itemPageParent, true);
		this._itemScroll.SetItemList(ref this._medicineItemList, false, null, null, null, true);
		this._charAttrDataView.HideInfectNotice(true, true);
	}

	// Token: 0x06001F26 RID: 7974 RVA: 0x000E2D98 File Offset: 0x000E0F98
	private void HighLightItemView(CommonTableRowForItem itemView)
	{
		bool flag = null == itemView;
		if (!flag)
		{
			this._focusingTuple.Item1 = itemView;
			this._focusingTuple.Item2 = itemView.transform.parent;
			this._focusingTuple.Item3 = itemView.transform.GetSiblingIndex();
			RectTransform focusMask = this._itemScroll.CGet<RectTransform>("FocusItemMask");
			itemView.transform.SetParent(focusMask, true);
			itemView.transform.localScale = Vector3.one;
			itemView.SetSelectState(true);
			itemView.SetSelectState(true);
			TooltipInvoker itemTip = itemView.GetMouseTip();
			itemTip.enabled = false;
			itemTip.HideTips();
			focusMask.gameObject.SetActive(true);
		}
	}

	// Token: 0x06001F27 RID: 7975 RVA: 0x000E2E54 File Offset: 0x000E1054
	private void CancelHighLightItemView()
	{
		bool flag = null == this._focusingTuple.Item1;
		if (!flag)
		{
			this._focusingTuple.Item1.transform.SetParent(this._focusingTuple.Item2, true);
			this._focusingTuple.Item1.transform.SetSiblingIndex(this._focusingTuple.Item3);
			this._focusingTuple.Item1.SetSelectState(false);
			this._focusingTuple.Item1.SetSelectState(false);
			this._focusingTuple.Item1.GetMouseTip().enabled = true;
			this._focusingTuple.Item1 = null;
			this._itemScroll.CGet<RectTransform>("FocusItemMask").gameObject.SetActive(false);
		}
	}

	// Token: 0x06001F2C RID: 7980 RVA: 0x000E3080 File Offset: 0x000E1280
	[CompilerGenerated]
	internal static bool <CheckMedicineItemIsLocked>g__CheckIsLockedForDetoxPoison|38_0(sbyte poisonType, out string tipContent, ref UI_UsingMedicineItem.<>c__DisplayClass38_0 A_2, ref UI_UsingMedicineItem.<>c__DisplayClass38_1 A_3)
	{
		tipContent = string.Empty;
		int[] poisons = A_3.monitor.Poisons;
		int poisonNow = (poisons != null) ? poisons.GetOrDefault((int)poisonType) : 0;
		bool flag = A_2.medicineItem.Duration == 0;
		if (flag)
		{
			bool immuneFlag = A_3.monitor.IsImmune(poisonType);
			bool need = poisonNow > 0 | A_2.isCombat;
			bool flag2 = !need || immuneFlag;
			if (flag2)
			{
				tipContent = LanguageKey.LK_UsingMedicine_Tip_NoNeed.Tr();
				return true;
			}
		}
		sbyte poisonLevel = PoisonsAndLevels.CalcPoisonedLevel(poisonNow);
		bool flag3 = A_2.medicineItem.EffectThresholdValue < (short)poisonLevel && A_2.medicineItem.Duration == 0;
		bool result;
		if (flag3)
		{
			tipContent = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_Value_Not_Enough);
			result = true;
		}
		else
		{
			bool flag4 = A_2.medicineItem.PoisonType != poisonType;
			if (flag4)
			{
				tipContent = LanguageKey.LK_UsingMedicine_Tip_Type_Different.Tr();
				result = true;
			}
			else
			{
				result = false;
			}
		}
		return result;
	}

	// Token: 0x04001775 RID: 6005
	private ItemOperationType.EItemOperationType _currItemOperation;

	// Token: 0x04001776 RID: 6006
	private int _charId;

	// Token: 0x04001777 RID: 6007
	private int _maxLoad;

	// Token: 0x04001778 RID: 6008
	private int _curLoad;

	// Token: 0x04001779 RID: 6009
	private bool _characterDataViewInt;

	// Token: 0x0400177A RID: 6010
	private List<ItemDisplayData> _canOperateItems = new List<ItemDisplayData>();

	// Token: 0x0400177B RID: 6011
	private List<ItemDisplayData> _medicineItemList = new List<ItemDisplayData>();

	// Token: 0x0400177C RID: 6012
	[TupleElementNames(new string[]
	{
		"focusingItemView",
		"parent",
		"sibling"
	})]
	private ValueTuple<CommonTableRowForItem, Transform, int> _focusingTuple;

	// Token: 0x0400177D RID: 6013
	private CharacterAttributeDataView _charAttrDataView;

	// Token: 0x0400177E RID: 6014
	private Refers _itemPage;

	// Token: 0x0400177F RID: 6015
	private CButtonObsolete _autoUseMedicineButton;

	// Token: 0x04001780 RID: 6016
	private ItemScrollViewForCommonTableRow _itemScroll;

	// Token: 0x04001781 RID: 6017
	private Transform _itemPageParent;

	// Token: 0x04001782 RID: 6018
	private Dictionary<int, ItemSortAndFilter.MedicineFilterType> _medicineFilterTypeDict;

	// Token: 0x04001783 RID: 6019
	private short _usingMedicineItemType;

	// Token: 0x04001784 RID: 6020
	private ItemKey _selectedItemKey;

	// Token: 0x04001785 RID: 6021
	private bool _needAutoUseMedicine;

	// Token: 0x04001786 RID: 6022
	private bool _hasAutoUseMedicine;

	// Token: 0x04001787 RID: 6023
	private Inventory _autoUseMedicineInventory;
}
