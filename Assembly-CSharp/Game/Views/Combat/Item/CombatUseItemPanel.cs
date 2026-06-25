using System;
using System.Collections.Generic;
using CharacterDataMonitor;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Components.ListStyleGeneralScroll.CellContent;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Item;
using Game.Components.SortAndFilter.Item.Apply;
using Game.Views.CharacterMenu;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Combat.Item
{
	// Token: 0x02000B3D RID: 2877
	public class CombatUseItemPanel : MonoBehaviour
	{
		// Token: 0x17000FA1 RID: 4001
		// (get) Token: 0x06008F01 RID: 36609 RVA: 0x0042A1DC File Offset: 0x004283DC
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x17000FA2 RID: 4002
		// (get) Token: 0x06008F02 RID: 36610 RVA: 0x0042A1E3 File Offset: 0x004283E3
		private int TotalWisdomCount
		{
			get
			{
				return (int)(this._wisdomCount + this._specialWisdomCount);
			}
		}

		// Token: 0x17000FA3 RID: 4003
		// (get) Token: 0x06008F03 RID: 36611 RVA: 0x0042A1F4 File Offset: 0x004283F4
		private bool IsMedicineFilter
		{
			get
			{
				List<LineState> lineStates = this.itemListScroll.SortAndFilterController.SortAndFilterState.LineStates;
				int num;
				if (lineStates == null)
				{
					num = -1;
				}
				else
				{
					num = lineStates.FindIndex((LineState s) => s.IsActive && s.Type == ESortAndFilterOneLineType.SingleSelectFilter);
				}
				int stateIndex = num;
				bool flag = stateIndex >= 0;
				bool result;
				if (flag)
				{
					List<FilterLineBase<ITradeableContent>> lineList = this.itemListScroll.SortAndFilterController.FilterLines;
					FilterLineBase<ITradeableContent> line = lineList[stateIndex];
					bool isMedicineFilter = line.Id == 2;
					result = isMedicineFilter;
				}
				else
				{
					result = false;
				}
				return result;
			}
		}

		// Token: 0x06008F04 RID: 36612 RVA: 0x0042A280 File Offset: 0x00428480
		public void Show(ArgumentBox argsBox, Action onHide)
		{
			this._onHide = onHide;
			sbyte wisdomType;
			argsBox.Get("WisdomType", out wisdomType);
			argsBox.Get("WisdomCount", out this._wisdomCount);
			argsBox.Get("SpecialWisdomCount", out this._specialWisdomCount);
			argsBox.Get("CanEatMore", out this._canEatMore);
			argsBox.Get("CanUseSwordFragment", out this._canUseSwordFragment);
			argsBox.Get<sbyte[]>("WeaponTricks", out this._weaponTricks);
			argsBox.Get<Action<ItemKey, sbyte, ItemKey, List<sbyte>>>("CallBack", out this._onSelected);
			argsBox.Get("CharId", out this._currentCharId);
			argsBox.Get<Action<OuterAndInnerShorts>>("UpdateAttackRangePreview", out this._onUpdateAttackRangePreview);
			this._usingMedicineItemType = UsingMedicineItemType.Invalid;
			this._wisdomIcon = CommonUtils.GetWisdomIcon(wisdomType);
			this.imageWisdomIcon.SetSprite(this._wisdomIcon, false, null);
			this._wisdomCost = 0;
			this.RefreshWisdomCount();
			this._selectedInjuryPartList.Clear();
			this._selectedRepairItem = ItemKey.Invalid;
			this._selectedItem = ItemKey.Invalid;
			this._useType = -1;
			this._resourceMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<ResourceMonitor>(this._currentCharId, false);
			this._lifeSkillMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<LifeSkillMonitor>(this._currentCharId, false);
			this._equipCombatSkillMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<EquipCombatSkillMonitor>(this._currentCharId, false);
			this.injury.Set(this._currentCharId);
			this.rootInjury.SetActive(false);
			this.buttonClose.ClearAndAddListener(new Action(this.OnClickButtonClose));
			this.equipmentListScroll.gameObject.SetActive(false);
			this.ApplyTargetCanvasesSortOrder(true);
			this.ApplyTargetSkeletonAnimationsOrder(true);
			base.gameObject.SetActive(true);
			this.canvasGroup.alpha = 0f;
			int lineId = EFilterLine.MainFilter.ToInt();
			this.itemListScroll.CustomWisdomDataGenerator = new Func<ITradeableContent, IconAndTextCellData>(this.CustomWisdomDataGenerator);
			this.itemListScroll.OnSortAndFilterChangedCallback = new Action(this.OnSortAndFilterChangedCallback);
			this.itemListScroll.Init("CombatUseItemPanelItemListScroll", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnItemRender), new Action<ITradeableContent, RowItemLine>(this.OnItemClick), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Wisdom, null, null, null);
			this.itemListScroll.SortAndFilterController.SetToggleVisible(lineId, CombatUseItemPanel.DefaultFilterTypes, false);
			this.OnSortAndFilterChangedCallback();
			this.equipmentListScroll.Init("CombatUseItemPanelEquipmentListScroll", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnEquipmentRender), new Action<ITradeableContent, RowItemLine>(this.OnEquipmentClick), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Durability, null, null, null);
			this.equipmentListScroll.SortAndFilterController.SetToggleVisible(lineId, EFilterLine.EquipFilter.ToInt() - 1);
			CombatDomainMethod.AsyncCall.RequestValidItemsInCombat(null, this._currentCharId, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._itemList);
				ItemDisplayData itemDisplayData = this._itemList.Find(delegate(ItemDisplayData d)
				{
					ItemKey key = d.Key;
					return key.ItemType == 6 && key.TemplateId == 54;
				});
				this._emptyToolKey = ((itemDisplayData != null) ? itemDisplayData.Key : ItemKey.Invalid);
				bool flag = !SingletonObject.getInstance<ProfessionModel>().IsSkillEquipped(54);
				if (flag)
				{
					this._itemList.RemoveAll((ItemDisplayData data) => data.Key.ItemType == 5);
				}
			});
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(5U, delegate
			{
				this.itemListScroll.SetItemList(this._itemList);
				this.canvasGroup.alpha = 1f;
			});
			GlobalDomainMethod.Call.InvokeGuidingTrigger(148);
		}

		// Token: 0x06008F05 RID: 36613 RVA: 0x0042A56A File Offset: 0x0042876A
		private void OnEnable()
		{
			GEvent.Add(UiEvents.UsingMedicineItemSwitch, new GEvent.Callback(this.OnUsingMedicineItemSwitch));
		}

		// Token: 0x06008F06 RID: 36614 RVA: 0x0042A586 File Offset: 0x00428786
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.UsingMedicineItemSwitch, new GEvent.Callback(this.OnUsingMedicineItemSwitch));
		}

		// Token: 0x06008F07 RID: 36615 RVA: 0x0042A5A4 File Offset: 0x004287A4
		private IconAndTextCellData CustomWisdomDataGenerator(ITradeableContent content)
		{
			return new IconAndTextCellData(this._wisdomIcon, string.Format("X{0}", content.RealKey.GetConsumedFeatureMedals()), true, false, false, false);
		}

		// Token: 0x06008F08 RID: 36616 RVA: 0x0042A5DF File Offset: 0x004287DF
		private void OnSortAndFilterChangedCallback()
		{
			this.rootInjury.SetActive(this.IsMedicineFilter);
			this.injury.UnselectPartByUsingMedicineItemType();
		}

		// Token: 0x06008F09 RID: 36617 RVA: 0x0042A600 File Offset: 0x00428800
		private void HideInjury()
		{
			bool flag = !this.IsMedicineFilter;
			if (flag)
			{
				this.rootInjury.SetActive(false);
			}
		}

		// Token: 0x06008F0A RID: 36618 RVA: 0x0042A628 File Offset: 0x00428828
		public bool Hide(bool playSound = true)
		{
			bool flag = !base.gameObject.activeSelf;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this._usingMedicineItemType > UsingMedicineItemType.Invalid;
				if (flag2)
				{
					this._usingMedicineItemType = UsingMedicineItemType.Invalid;
					this.OnUsingMedicineItemSwitch();
					result = true;
				}
				else
				{
					bool activeSelf = this.equipmentListScroll.gameObject.activeSelf;
					if (activeSelf)
					{
						this._selectedItem = ItemKey.Invalid;
						this.equipmentListScroll.gameObject.SetActive(false);
						this.itemListScroll.gameObject.SetActive(true);
						if (playSound)
						{
							AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
						}
						result = true;
					}
					else
					{
						bool isSelectInjuryPart = this.injury.IsSelectInjuryPart;
						if (isSelectInjuryPart)
						{
							this.injury.ExitSelectInjuryPart();
							this.HideInjury();
							if (playSound)
							{
								AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
							}
							result = true;
						}
						else
						{
							if (playSound)
							{
								AudioManager.Instance.PlaySound("ui_default_small_back", false, false);
							}
							this.ApplyTargetCanvasesSortOrder(false);
							this.ApplyTargetSkeletonAnimationsOrder(false);
							base.gameObject.SetActive(false);
							Action onHide = this._onHide;
							if (onHide != null)
							{
								onHide();
							}
							result = false;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06008F0B RID: 36619 RVA: 0x0042A770 File Offset: 0x00428970
		private void Update()
		{
			bool flag = CommonCommandKit.RightMouse.Check(UIElement.Combat, false, false, false, true, false) || CommonCommandKit.Esc.Check(UIElement.Combat, false, false, false, true, false);
			if (flag)
			{
				bool flag2 = this.Hide(true);
				if (flag2)
				{
				}
			}
		}

		// Token: 0x06008F0C RID: 36620 RVA: 0x0042A7C0 File Offset: 0x004289C0
		private void RefreshWisdomCount()
		{
			string totalCountStr = (this._specialWisdomCount == 0) ? this._wisdomCount.ToString() : string.Format("{0}+{1}", (int)(this._wisdomCount - this._specialWisdomCount), this._specialWisdomCount.ToString().SetColor("brightblue"));
			this.textWisdomCount.text = ((this._wisdomCost == 0) ? totalCountStr : (totalCountStr + (-this._wisdomCost).ToString().SetColor("brightred")).ColorReplace());
		}

		// Token: 0x06008F0D RID: 36621 RVA: 0x0042A850 File Offset: 0x00428A50
		private void ApplyTargetCanvasesSortOrder(bool isApply)
		{
			bool flag = this.targetCanvases == null || this.targetCanvases.Length == 0;
			if (!flag)
			{
				if (isApply)
				{
					this._canvasSortOrderBackup.Clear();
					foreach (Canvas canvas in this.targetCanvases)
					{
						bool flag2 = canvas != null;
						if (flag2)
						{
							this._canvasSortOrderBackup[canvas] = canvas.sortingOrder;
							canvas.sortingOrder = 251;
						}
					}
				}
				else
				{
					foreach (KeyValuePair<Canvas, int> kvp in this._canvasSortOrderBackup)
					{
						bool flag3 = kvp.Key != null;
						if (flag3)
						{
							kvp.Key.sortingOrder = kvp.Value;
						}
					}
					this._canvasSortOrderBackup.Clear();
				}
			}
		}

		// Token: 0x06008F0E RID: 36622 RVA: 0x0042A95C File Offset: 0x00428B5C
		private void ApplyTargetSkeletonAnimationsOrder(bool isApply)
		{
			bool flag = this.targetSkeletonAnimations == null || this.targetSkeletonAnimations.Length == 0;
			if (!flag)
			{
				if (isApply)
				{
					this._skeletonAnimationOrderBackup.Clear();
					foreach (SkeletonAnimation skeletonAnimation in this.targetSkeletonAnimations)
					{
						bool flag2 = skeletonAnimation != null;
						if (flag2)
						{
							MeshRenderer meshRenderer = skeletonAnimation.GetComponent<MeshRenderer>();
							bool flag3 = meshRenderer != null;
							if (flag3)
							{
								this._skeletonAnimationOrderBackup[skeletonAnimation] = meshRenderer.sortingOrder;
								meshRenderer.sortingOrder = 251;
							}
						}
					}
				}
				else
				{
					foreach (KeyValuePair<SkeletonAnimation, int> kvp in this._skeletonAnimationOrderBackup)
					{
						bool flag4 = kvp.Key != null;
						if (flag4)
						{
							MeshRenderer meshRenderer2 = kvp.Key.GetComponent<MeshRenderer>();
							bool flag5 = meshRenderer2 != null;
							if (flag5)
							{
								meshRenderer2.sortingOrder = kvp.Value;
							}
						}
					}
					this._skeletonAnimationOrderBackup.Clear();
				}
			}
		}

		// Token: 0x06008F0F RID: 36623 RVA: 0x0042AA9C File Offset: 0x00428C9C
		private void OnItemClick(ITradeableContent content, RowItemLine rowItemLine)
		{
			CombatUseItemPanel.<>c__DisplayClass52_0 CS$<>8__locals1 = new CombatUseItemPanel.<>c__DisplayClass52_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.itemData = (ItemDisplayData)content;
			CS$<>8__locals1.itemKey = CS$<>8__locals1.itemData.RealKey;
			CS$<>8__locals1.isRecoverInjuryMainMedicineItem = CommonUtils.IsRecoverInjuryMainMedicineItem(CS$<>8__locals1.itemKey);
			bool isRecoverInjuryMainMedicineItem = CS$<>8__locals1.isRecoverInjuryMainMedicineItem;
			if (isRecoverInjuryMainMedicineItem)
			{
				bool isSelectInjuryPart = this.injury.IsSelectInjuryPart;
				if (isSelectInjuryPart)
				{
					this.injury.ExitSelectInjuryPart();
					this.HideInjury();
					return;
				}
			}
			ValueTuple<int, string> valueTuple = Injury.CheckEatSlot(this.injury.Data, CS$<>8__locals1.itemKey, CS$<>8__locals1.itemData.Amount);
			int count = valueTuple.Item1;
			string reason = valueTuple.Item2;
			bool canEatMore = count > 0;
			CS$<>8__locals1.isWugKing = EatingItems.IsWugKing(CS$<>8__locals1.itemKey);
			CS$<>8__locals1.sheetInfos = new List<ViewPopupMenu.BtnData>();
			CS$<>8__locals1.isMedicine = (CS$<>8__locals1.itemKey.ItemType == 8);
			CS$<>8__locals1.isEat = CommonUtils.CanItemEat(CS$<>8__locals1.itemKey.ItemType, CS$<>8__locals1.itemKey.TemplateId, this._currentCharId);
			bool isTool = CS$<>8__locals1.itemKey.ItemType == 6;
			bool isMedicine = CS$<>8__locals1.isMedicine;
			if (isMedicine)
			{
				MedicineItem medicineConfig = Medicine.Instance[CS$<>8__locals1.itemKey.TemplateId];
				string tipContent;
				bool innerInteractable = !CombatUseItemPanel.CheckEatItemIsLocked(CS$<>8__locals1.itemData, this.injury.Data, out tipContent);
				string btnName = CommonUtils.GetCanEatItemButtonName(CS$<>8__locals1.itemData.RealKey);
				ViewPopupMenu.BtnData innerButton = new ViewPopupMenu.BtnData(btnName, innerInteractable, EItemMenuDisplayOrder.Eat, new Action(CS$<>8__locals1.<OnItemClick>g__OnButtonClickEat|1), new UnityAction(CS$<>8__locals1.<OnItemClick>g__OnButtonEnterEat|2), new UnityAction(CS$<>8__locals1.<OnItemClick>g__OnButtonExit|5), false);
				innerButton.SetTip(string.Empty, tipContent);
				CS$<>8__locals1.sheetInfos.Add(innerButton);
				bool flag = medicineConfig.MaxUseDistance >= 0;
				if (flag)
				{
					CS$<>8__locals1.<OnItemClick>g__AddTrowButton|6((short)medicineConfig.MaxUseDistance);
				}
			}
			else
			{
				bool isEat = CS$<>8__locals1.isEat;
				if (isEat)
				{
					bool flag2 = ItemTemplateHelper.IsTianJieFuLu(CS$<>8__locals1.itemKey.ItemType, CS$<>8__locals1.itemKey.TemplateId);
					if (flag2)
					{
						bool innerInteractable2 = canEatMore;
						string tipContent2 = string.Empty;
						bool flag3 = !canEatMore;
						if (flag3)
						{
							tipContent2 = LocalStringManager.Get(LanguageKey.LK_Use_Medicine_Tip_NoSlot);
						}
						ViewPopupMenu.BtnData innerButton2 = new ViewPopupMenu.BtnData(LocalStringManager.Get(LanguageKey.LK_Eat_Item), innerInteractable2, EItemMenuDisplayOrder.Eat, new Action(CS$<>8__locals1.<OnItemClick>g__OnButtonClickEat|1), new UnityAction(CS$<>8__locals1.<OnItemClick>g__OnButtonEnterEat|2), new UnityAction(CS$<>8__locals1.<OnItemClick>g__OnButtonExit|5), false);
						innerButton2.SetTip(string.Empty, tipContent2);
						CS$<>8__locals1.sheetInfos.Add(innerButton2);
					}
					else
					{
						bool isMisc = CS$<>8__locals1.itemKey.ItemType == 12;
						bool neiliIsNotMax = this._equipCombatSkillMonitor.CurrNeili < this._equipCombatSkillMonitor.MaxNeili;
						bool innerInteractable3 = isMisc ? neiliIsNotMax : canEatMore;
						string tipContent3 = string.Empty;
						bool flag4 = !canEatMore;
						if (flag4)
						{
							tipContent3 = LocalStringManager.Get(LanguageKey.LK_Use_Medicine_Tip_NoSlot);
						}
						bool flag5 = isMisc && !neiliIsNotMax;
						if (flag5)
						{
							tipContent3 = LocalStringManager.Get(LanguageKey.LK_ItemTips_Use_NeiliIsMax);
						}
						ViewPopupMenu.BtnData innerButton3 = new ViewPopupMenu.BtnData(LocalStringManager.Get(LanguageKey.LK_Eat_Item), innerInteractable3, EItemMenuDisplayOrder.Eat, new Action(CS$<>8__locals1.<OnItemClick>g__OnButtonClickEat|1), new UnityAction(CS$<>8__locals1.<OnItemClick>g__OnButtonEnterEat|2), new UnityAction(CS$<>8__locals1.<OnItemClick>g__OnButtonExit|5), false);
						innerButton3.SetTip(string.Empty, tipContent3);
						CS$<>8__locals1.sheetInfos.Add(innerButton3);
					}
				}
				else
				{
					bool flag6 = CS$<>8__locals1.itemKey.ItemType == 12;
					if (flag6)
					{
						MiscItem miscConfig = Misc.Instance.GetItem(CS$<>8__locals1.itemKey.TemplateId);
						bool flag7 = miscConfig.MaxUseDistance > 0;
						if (flag7)
						{
							CS$<>8__locals1.<OnItemClick>g__AddTrowButton|6((short)miscConfig.MaxUseDistance);
						}
						else
						{
							bool flag8 = GameData.Domains.Combat.SharedConstValue.SwordFragment2BossId.ContainsKey(CS$<>8__locals1.itemKey.TemplateId);
							if (flag8)
							{
								ViewPopupMenu.BtnData innerButton4 = new ViewPopupMenu.BtnData(LocalStringManager.Get(LanguageKey.LK_Use_SwordFragment), true, EItemMenuDisplayOrder.Other, delegate()
								{
									CS$<>8__locals1.<>4__this.SetSelectedItem(CS$<>8__locals1.itemData, -1);
								}, null, null, false);
								CS$<>8__locals1.sheetInfos.Add(innerButton4);
							}
							else
							{
								bool flag9 = CommonUtils.IsTianSuiBaoLuItem(CS$<>8__locals1.itemKey.ItemType, CS$<>8__locals1.itemKey.TemplateId);
								if (flag9)
								{
									ViewPopupMenu.BtnData useButton = new ViewPopupMenu.BtnData(LocalStringManager.Get(LanguageKey.LK_Use_TianSuiBaoLuItem), true, EItemMenuDisplayOrder.Other, new Action(CS$<>8__locals1.<OnItemClick>g__OnButtonClickEat|1), null, null, false);
									CS$<>8__locals1.sheetInfos.Add(useButton);
								}
							}
						}
					}
					else
					{
						bool flag10 = isTool;
						if (flag10)
						{
							ViewPopupMenu.BtnData button = new ViewPopupMenu.BtnData(LocalStringManager.Get(LanguageKey.LK_Repair_Item), true, EItemMenuDisplayOrder.Tool, new Action(CS$<>8__locals1.<OnItemClick>g__OnButtonClickTool|0), null, null, false);
							CS$<>8__locals1.sheetInfos.Add(button);
						}
					}
				}
			}
			bool flag11 = CS$<>8__locals1.sheetInfos.Count > 0;
			if (flag11)
			{
				this.itemListScroll.SetItemToPopupMenuMode(rowItemLine, CS$<>8__locals1.sheetInfos, new Action(this.HideInjury), false);
			}
		}

		// Token: 0x06008F10 RID: 36624 RVA: 0x0042AF6E File Offset: 0x0042916E
		private void EnterAttackRangePreview(short distance)
		{
			this._onUpdateAttackRangePreview(new OuterAndInnerShorts(0, distance));
		}

		// Token: 0x06008F11 RID: 36625 RVA: 0x0042AF83 File Offset: 0x00429183
		private void CancelAttackRangePreview()
		{
			this._onUpdateAttackRangePreview(new OuterAndInnerShorts(-1, -1));
		}

		// Token: 0x06008F12 RID: 36626 RVA: 0x0042AF98 File Offset: 0x00429198
		private void OnItemRender(ITradeableContent content, RowItemLine rowItemLine)
		{
			ItemDisplayData itemData = (ItemDisplayData)content;
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, true);
			int costWisdom = itemData.RealKey.GetConsumedFeatureMedals();
			CombatSubProcessorCharacter processor;
			bool flag = this.Model.ProcessorCharacters.TryGetValue(this._currentCharId, out processor) && processor.UseItemCostNoWisdom;
			if (flag)
			{
				costWisdom = 0;
			}
			bool costEnough = costWisdom <= this.TotalWisdomCount;
			string itemTip = string.Empty;
			bool interactable = costEnough && !itemData.IsLocked && CombatUseItemPanel.GetInteractable(itemData, out itemTip, this._canEatMore, this._canUseSwordFragment, this._weaponTricks, this.injury.Data);
			rowItemLine.SetInteractable(interactable, true);
			rowItemLine.SetDisabled(!interactable);
			rowItemLine.SetSelected(itemData.ContainsItemKey(this._selectedItem));
			bool flag2 = interactable;
			if (flag2)
			{
				rowItemMain.HideInteractionState();
			}
			else
			{
				bool flag3 = !itemTip.IsNullOrEmpty();
				if (flag3)
				{
					rowItemMain.SetInteractionStateLockText(itemTip);
				}
			}
			sbyte distance = ItemTemplateHelper.GetMaxUseDistance(itemData.RealKey.ItemType, itemData.RealKey.TemplateId);
			rowItemLine.OnPointerEnterEvent = null;
			rowItemLine.OnPointerExitEvent = null;
			bool flag4 = interactable && distance > 0;
			if (flag4)
			{
				rowItemLine.OnPointerEnterEvent = delegate()
				{
					this.EnterAttackRangePreview((short)distance);
				};
				rowItemLine.OnPointerExitEvent = new Action(this.CancelAttackRangePreview);
			}
		}

		// Token: 0x06008F13 RID: 36627 RVA: 0x0042B114 File Offset: 0x00429314
		public static bool GetInteractable(ItemDisplayData itemData, out string itemTip, bool canEatMore, bool canUseSwordFragment, sbyte[] weaponTricks, CharacterInjuryDisplayData characterInjuryDisplayData)
		{
			itemTip = string.Empty;
			sbyte itemType = itemData.RealKey.ItemType;
			bool flag = itemType == 7 || itemType == 9;
			bool result;
			if (flag)
			{
				result = canEatMore;
			}
			else
			{
				bool flag2 = ItemTemplateHelper.IsTianJieFuLu(itemData.RealKey.ItemType, itemData.RealKey.TemplateId);
				if (flag2)
				{
					result = (canEatMore && itemData.Amount >= ItemTemplateHelper.GetTianJieFuLuCountUnit());
				}
				else
				{
					bool flag3 = itemData.RealKey.ItemType == 12 && GameData.Domains.Combat.SharedConstValue.SwordFragment2BossId.ContainsKey(itemData.RealKey.TemplateId);
					if (flag3)
					{
						short skillId = (short)itemData.SpecialArg;
						bool flag4 = skillId >= 0;
						if (flag4)
						{
							List<NeedTrick> skillTricks = CombatSkill.Instance[skillId].TrickCost;
							result = (canUseSwordFragment && !skillTricks.Exists((NeedTrick costTrick) => !weaponTricks.Exist(costTrick.TrickType)));
						}
						else
						{
							result = false;
						}
					}
					else
					{
						bool flag5 = itemData.RealKey.ItemType == 8;
						if (flag5)
						{
							bool flag6 = Medicine.Instance[itemData.RealKey.TemplateId].MaxUseDistance >= 0;
							result = (flag6 || !CombatUseItemPanel.CheckEatItemIsLocked(itemData, characterInjuryDisplayData, out itemTip));
						}
						else
						{
							result = true;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06008F14 RID: 36628 RVA: 0x0042B270 File Offset: 0x00429470
		public static bool CheckEatItemIsLocked(ItemDisplayData itemData, CharacterInjuryDisplayData characterInjuryDisplayData, out string tipContent)
		{
			tipContent = string.Empty;
			bool isLocked = itemData.IsLocked;
			return isLocked || Injury.CheckEatItemIsLocked(characterInjuryDisplayData, itemData, (int)UsingMedicineItemType.Invalid, out tipContent);
		}

		// Token: 0x06008F15 RID: 36629 RVA: 0x0042B2A4 File Offset: 0x004294A4
		private void SetSelectedItem(ItemDisplayData itemData, sbyte useType = -1)
		{
			bool flag = this._selectedItem.IsValid();
			if (flag)
			{
				this.CancelSelection();
			}
			else
			{
				this.injury.HideNotice(false, true);
			}
			this._wisdomCost = itemData.Key.GetConsumedFeatureMedals();
			this.RefreshWisdomCount();
			this._selectedItem = itemData.RealKey;
			this._useType = useType;
			bool flag2 = itemData.Key.ItemType != 6;
			if (flag2)
			{
				this.OnConfirmSelect();
			}
		}

		// Token: 0x06008F16 RID: 36630 RVA: 0x0042B320 File Offset: 0x00429520
		private void OnConfirmSelect()
		{
			this._onSelected(this._selectedItem, this._useType, this._selectedRepairItem, this._selectedInjuryPartList);
			bool flag;
			do
			{
				flag = !this.Hide(false);
			}
			while (!flag);
		}

		// Token: 0x06008F17 RID: 36631 RVA: 0x0042B36A File Offset: 0x0042956A
		private void CancelSelection()
		{
			this._selectedItem = ItemKey.Invalid;
		}

		// Token: 0x06008F18 RID: 36632 RVA: 0x0042B378 File Offset: 0x00429578
		private void OnEquipmentRender(ITradeableContent content, RowItemLine rowItemLine)
		{
			ItemDisplayData itemData = (ItemDisplayData)content;
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, true);
			ItemDisplayData tool = this._itemList.Find((ItemDisplayData data) => data.ContainsItemKey(this._selectedItem));
			ResourceInts needResource = ItemTemplateHelper.GetRepairNeedResources(itemData.MaterialResources, itemData.Key, itemData.Durability);
			ResourceInts curResource = new ResourceInts(this._resourceMonitor.Resources);
			bool resourceIsMeet = curResource.CheckIsMeet(ref needResource);
			LifeSkillShorts lifeSkillAttainments = new LifeSkillShorts(this._lifeSkillMonitor.Attainments);
			sbyte lifeSkillType;
			short requiredAttainment;
			bool lifeSkillIsMeet = ViewCharacterMenuItems.CheckCurrentToolAttainment(ItemOperationType.EItemOperationType.Repair, tool, itemData, lifeSkillAttainments, out lifeSkillType, out requiredAttainment);
			short durabilityCost;
			bool durabilityIsMeet = ViewCharacterMenuItems.CheckCurrentToolDurability(tool, itemData, out durabilityCost);
			bool allIsMeet = lifeSkillIsMeet && durabilityIsMeet && resourceIsMeet;
			bool flag = allIsMeet;
			if (flag)
			{
				rowItemLine.OnPointerEnterEvent = new Action(CombatUseItemPanel.SetCursorForOperation);
				rowItemLine.OnPointerExitEvent = new Action(CombatUseItemPanel.ResetCursor);
			}
			else
			{
				rowItemLine.OnPointerEnterEvent = null;
				rowItemLine.OnPointerExitEvent = null;
			}
			rowItemLine.SetInteractable(allIsMeet, true);
			rowItemLine.SetDisabled(!allIsMeet);
			rowItemMain.ShowInteractionStateAttainment(lifeSkillType, requiredAttainment, lifeSkillIsMeet);
			bool flag2 = !durabilityIsMeet;
			if (flag2)
			{
				string durabilityTip = LanguageKey.LK_Tool_Durability_Not_Enough.Tr().SetColor("brightred");
				rowItemMain.SetItemNotCanSelectReason(durabilityTip);
			}
			else
			{
				bool flag3 = !resourceIsMeet;
				if (flag3)
				{
					string resourceTip = LanguageKey.LK_Building_LockOfResource.Tr().SetColor("brightred");
					rowItemMain.SetItemNotCanSelectReason(resourceTip);
				}
				else
				{
					rowItemMain.ShowInteractionStateAttainment(lifeSkillType, requiredAttainment, lifeSkillIsMeet);
				}
			}
			TooltipInvoker tipDisplayer = rowItemLine.TipDisplayer;
			tipDisplayer.Type = TipType.RepairItem;
			tipDisplayer.NeedRefresh = true;
			tipDisplayer.RuntimeParam = new ArgumentBox().SetObject("Resource", new ResourceInts(this._resourceMonitor.Resources)).SetObject("NeedResource", needResource).SetObject("Item", itemData).SetObject("Tool", tool).Set("DurabilityCost", durabilityCost);
		}

		// Token: 0x06008F19 RID: 36633 RVA: 0x0042B570 File Offset: 0x00429770
		private void OnEquipmentClick(ITradeableContent content, RowItemLine rowItemLine)
		{
			ItemDisplayData itemData = (ItemDisplayData)content;
			ItemKey itemKey = itemData.RealKey;
			this._selectedRepairItem = itemKey;
			BuildingDomainMethod.AsyncCall.CheckRepairConditionIsMeet(null, this._currentCharId, this._selectedItem, this._selectedRepairItem, BuildingBlockKey.Invalid, delegate(int offset, RawDataPool pool)
			{
				bool canRepair = false;
				Serializer.Deserialize(pool, offset, ref canRepair);
				bool flag = !canRepair;
				if (!flag)
				{
					DialogCmd dialogCmd = new DialogCmd();
					dialogCmd.Type = 1;
					dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_Item_Repair_Tip_Title);
					dialogCmd.Content = LocalStringManager.Get(LanguageKey.LK_Item_Repair_In_Combat_Tips);
					dialogCmd.Yes = new Action(this.OnConfirmSelect);
					UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
					UIManager.Instance.MaskUI(UIElement.Dialog);
				}
			});
		}

		// Token: 0x06008F1A RID: 36634 RVA: 0x0042B5BD File Offset: 0x004297BD
		public static void SetCursorForOperation()
		{
			ConchShipCursor.Instance.SetCursorImage("sp_cursor_clickable_chuizi", -1f, -1f);
			ConchShipCursor.Instance.CanChange = false;
		}

		// Token: 0x06008F1B RID: 36635 RVA: 0x0042B5E5 File Offset: 0x004297E5
		public static void ResetCursor()
		{
			ConchShipCursor.Instance.CanChange = true;
			ConchShipCursor.Instance.SetDefaultCursor();
		}

		// Token: 0x06008F1C RID: 36636 RVA: 0x0042B5FE File Offset: 0x004297FE
		private void OnClickButtonClose()
		{
			this.Hide(true);
		}

		// Token: 0x06008F1D RID: 36637 RVA: 0x0042B608 File Offset: 0x00429808
		private void OnUsingMedicineItemSwitch(ArgumentBox argbox)
		{
			argbox.Get("UsingMedicineItemType", out this._usingMedicineItemType);
			this.OnUsingMedicineItemSwitch();
		}

		// Token: 0x06008F1E RID: 36638 RVA: 0x0042B624 File Offset: 0x00429824
		private void OnUsingMedicineItemSwitch()
		{
			this.injury.SelectPartByUsingMedicineItemType(this._usingMedicineItemType);
			int toggleIndex = -1;
			bool flag = UsingMedicineItemType.IsHurt(this._usingMedicineItemType);
			if (flag)
			{
				sbyte injuryType = (sbyte)this._usingMedicineItemType;
				ValueTuple<sbyte, sbyte> valueTuple = this.injury.Data.Injuries.Get(injuryType);
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
					toggleIndex = EMedicineSubFilterKeys.Detox.ToInt();
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
			this.itemListScroll.OnSortAndFilterChangedCallback = null;
			this.itemListScroll.SortAndFilterController.SetToggleIsOn(EFilterLine.MedicineFilter.ToInt(), -1);
			this.itemListScroll.SortAndFilterController.SetDropdownOption(EFilterLine.MedicineFilter.ToInt(), 0, toggleIndex);
			this.itemListScroll.OnSortAndFilterChangedCallback = new Action(this.OnSortAndFilterChangedCallback);
			foreach (ItemDisplayData itemDisplayData in this._itemList)
			{
				string text;
				itemDisplayData.Interactable = !this.CheckItemIsLocked(itemDisplayData, out text);
			}
			this.itemListScroll.SetItemList(this._itemList);
		}

		// Token: 0x06008F1F RID: 36639 RVA: 0x0042B7E0 File Offset: 0x004299E0
		private bool CheckItemIsLocked(ITradeableContent itemData, out string tipContent)
		{
			tipContent = string.Empty;
			bool isLocked = itemData.IsLocked;
			return isLocked || Injury.CheckEatItemIsLocked(this.injury.Data, itemData, (int)this._usingMedicineItemType, out tipContent);
		}

		// Token: 0x04006D3B RID: 27963
		[SerializeField]
		private CanvasGroup canvasGroup;

		// Token: 0x04006D3C RID: 27964
		[SerializeField]
		private ItemListScroll itemListScroll;

		// Token: 0x04006D3D RID: 27965
		[SerializeField]
		private ItemListScroll equipmentListScroll;

		// Token: 0x04006D3E RID: 27966
		[SerializeField]
		private CImage imageWisdomIcon;

		// Token: 0x04006D3F RID: 27967
		[SerializeField]
		private TextMeshProUGUI textWisdomCount;

		// Token: 0x04006D40 RID: 27968
		[SerializeField]
		private CButton buttonClose;

		// Token: 0x04006D41 RID: 27969
		[SerializeField]
		private Injury injury;

		// Token: 0x04006D42 RID: 27970
		[SerializeField]
		private GameObject rootInjury;

		// Token: 0x04006D43 RID: 27971
		public static readonly List<int> DefaultFilterTypes = new List<int>
		{
			EMainFilterKeys.Food.ToInt(),
			EMainFilterKeys.Medicine.ToInt(),
			EMainFilterKeys.Material.ToInt(),
			EMainFilterKeys.CraftTool.ToInt(),
			EMainFilterKeys.Misc.ToInt()
		};

		// Token: 0x04006D44 RID: 27972
		private int _currentCharId;

		// Token: 0x04006D45 RID: 27973
		private string _wisdomIcon;

		// Token: 0x04006D46 RID: 27974
		private short _wisdomCount;

		// Token: 0x04006D47 RID: 27975
		private short _specialWisdomCount;

		// Token: 0x04006D48 RID: 27976
		private int _wisdomCost;

		// Token: 0x04006D49 RID: 27977
		private List<ItemDisplayData> _itemList = new List<ItemDisplayData>();

		// Token: 0x04006D4A RID: 27978
		private Action<ItemKey, sbyte, ItemKey, List<sbyte>> _onSelected;

		// Token: 0x04006D4B RID: 27979
		private ItemKey _selectedItem;

		// Token: 0x04006D4C RID: 27980
		private ItemKey _selectedRepairItem;

		// Token: 0x04006D4D RID: 27981
		private sbyte _useType;

		// Token: 0x04006D4E RID: 27982
		private bool _canEatMore;

		// Token: 0x04006D4F RID: 27983
		private bool _canUseSwordFragment;

		// Token: 0x04006D50 RID: 27984
		private sbyte[] _weaponTricks;

		// Token: 0x04006D51 RID: 27985
		private List<ItemDisplayData> _repairItemDatas = new List<ItemDisplayData>();

		// Token: 0x04006D52 RID: 27986
		private readonly List<sbyte> _selectedInjuryPartList = new List<sbyte>();

		// Token: 0x04006D53 RID: 27987
		private ItemKey _emptyToolKey;

		// Token: 0x04006D54 RID: 27988
		private ResourceMonitor _resourceMonitor;

		// Token: 0x04006D55 RID: 27989
		private LifeSkillMonitor _lifeSkillMonitor;

		// Token: 0x04006D56 RID: 27990
		private EquipCombatSkillMonitor _equipCombatSkillMonitor;

		// Token: 0x04006D57 RID: 27991
		private Action _onHide;

		// Token: 0x04006D58 RID: 27992
		private Action<OuterAndInnerShorts> _onUpdateAttackRangePreview;

		// Token: 0x04006D59 RID: 27993
		private short _usingMedicineItemType;

		// Token: 0x04006D5A RID: 27994
		[SerializeField]
		private Canvas[] targetCanvases;

		// Token: 0x04006D5B RID: 27995
		private readonly Dictionary<Canvas, int> _canvasSortOrderBackup = new Dictionary<Canvas, int>();

		// Token: 0x04006D5C RID: 27996
		[SerializeField]
		private SkeletonAnimation[] targetSkeletonAnimations;

		// Token: 0x04006D5D RID: 27997
		private readonly Dictionary<SkeletonAnimation, int> _skeletonAnimationOrderBackup = new Dictionary<SkeletonAnimation, int>();
	}
}
