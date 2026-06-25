using System;
using System.Collections.Generic;
using System.Linq;
using CharacterDataMonitor;
using CommonSortAndFilterLegacy.Item;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.Character;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter.Item.Apply;
using Game.Views.CharacterMenu;
using Game.Views.Combat.Migrate;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UICommon.Character;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Combat.Item
{
	// Token: 0x02000B3B RID: 2875
	public class CombatQuickUseItemPanel : MonoBehaviour
	{
		// Token: 0x17000F9C RID: 3996
		// (get) Token: 0x06008EDB RID: 36571 RVA: 0x00428569 File Offset: 0x00426769
		public RectTransform Root
		{
			get
			{
				return this.root;
			}
		}

		// Token: 0x17000F9D RID: 3997
		// (get) Token: 0x06008EDC RID: 36572 RVA: 0x00428571 File Offset: 0x00426771
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x17000F9E RID: 3998
		// (get) Token: 0x06008EDD RID: 36573 RVA: 0x00428578 File Offset: 0x00426778
		private int TotalWisdomCount
		{
			get
			{
				return (int)(this._wisdomCount + this._specialWisdomCount);
			}
		}

		// Token: 0x06008EDE RID: 36574 RVA: 0x00428588 File Offset: 0x00426788
		public void Setup(int charId)
		{
			bool flag = this._currentCharId == charId;
			if (!flag)
			{
				this._currentCharId = charId;
				this._equipCombatSkillMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<EquipCombatSkillMonitor>(charId, false);
			}
		}

		// Token: 0x06008EDF RID: 36575 RVA: 0x004285BE File Offset: 0x004267BE
		private void OnCloseEncyclopedia()
		{
			UIManager.Instance.SetEscHandler(this._cachedEscHandler);
		}

		// Token: 0x06008EE0 RID: 36576 RVA: 0x004285D2 File Offset: 0x004267D2
		private void OnOpenEncyclopedia()
		{
			UIManager.Instance.SetEscHandler(null);
		}

		// Token: 0x06008EE1 RID: 36577 RVA: 0x004285E4 File Offset: 0x004267E4
		public void Show(ArgumentBox argsBox, Action onHide)
		{
			this.Model.RaiseEvent(ECombatEvents.OnCirclePanelShow);
			this._cachedEscHandler = delegate()
			{
				this.Hide(true, true);
			};
			this._pendingSetEscHandler = true;
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
			this._wisdomIcon = CommonUtils.GetWisdomIcon(wisdomType);
			this.imageWisdomIcon.SetSprite(this._wisdomIcon, false, null);
			this._wisdomCost = 0;
			this.RefreshWisdomCount();
			this._selectedInjuryPartList.Clear();
			this._selectedRepairItem = ItemKey.Invalid;
			this._selectedItem = ItemKey.Invalid;
			this._useType = -1;
			this.Setup(this._currentCharId);
			this._resourceMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<ResourceMonitor>(this._currentCharId, false);
			this._lifeSkillMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<LifeSkillMonitor>(this._currentCharId, false);
			this.buttonSetting.ClearAndAddListener(new Action(this.OnClickButtonSetting));
			int lineId = EFilterLine.MainFilter.ToInt();
			this.equipmentListScroll.Init("CombatUseItemPanelEquipmentListScroll", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnEquipmentRender), new Action<ITradeableContent, RowItemLine>(this.OnEquipmentClick), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Durability, null, null, null);
			this.equipmentListScroll.SortAndFilterController.SetToggleVisible(lineId, EFilterLine.EquipFilter.ToInt() - 1);
			this.buttonClose.ClearAndAddListener(new Action(this.OnClickButtonClose));
			this.buttonHide.ClearAndAddListener(new Action(this.OnClickButtonClose));
			this.injury.Set(this._currentCharId);
			this.rootInjury.SetActive(false);
			this.rootEquipment.SetActive(false);
			this.ApplyTargetCanvasesSortOrder(true);
			base.gameObject.SetActive(true);
			this.canvasGroup.alpha = 0f;
			this._openFrameCount = 1;
			this._characterAvatarSelf = new CharacterAvatar(this.avatarSelf, true);
			this._characterAvatarEnemy = new CharacterAvatar(this.avatarEnemy, true);
			this._characterAvatarSelf.CharacterId = this.Model.SelfCharId;
			this._characterAvatarEnemy.CharacterId = this.Model.EnemyCharId;
			CharacterDisplayData selfDisplayData = this.Model.DisplayDataCache[this.Model.SelfCharId];
			this.selfName.text = CombatUtils.GetNameString(selfDisplayData, true);
			CharacterDisplayData enemyDisplayData = this.Model.DisplayDataCache[this.Model.EnemyCharId];
			this.enemyName.text = CombatUtils.GetNameString(enemyDisplayData, false);
			UIElement encyclopedia = UIElement.Encyclopedia;
			encyclopedia.OnActive = (Action)Delegate.Combine(encyclopedia.OnActive, new Action(this.OnOpenEncyclopedia));
			UIElement encyclopedia2 = UIElement.Encyclopedia;
			encyclopedia2.OnDeActive = (Action)Delegate.Combine(encyclopedia2.OnDeActive, new Action(this.OnCloseEncyclopedia));
			CombatDomainMethod.AsyncCall.RequestValidItemsInCombat(null, this._currentCharId, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._itemList);
				bool flag = !SingletonObject.getInstance<ProfessionModel>().IsSkillEquipped(54);
				if (flag)
				{
					this._itemList.RemoveAll((ItemDisplayData data) => data.Key.ItemType == 5);
				}
			});
			CombatDomainMethod.AsyncCall.GetCombatQuickUseItemSlotData(null, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._slotDataList);
				bool flag = !base.gameObject.activeSelf;
				if (!flag)
				{
					int totalValidCount = this._slotDataList.Count(delegate(CombatQuickUseItemSlotData d)
					{
						Inventory inventory2 = d.Inventory;
						return inventory2 != null && inventory2.InventoryItemTotalCount > 0;
					});
					for (int index = 0; index < this.itemSlotArray.Length; index++)
					{
						CombatQuickUseItemPanel.<>c__DisplayClass55_0 CS$<>8__locals1 = new CombatQuickUseItemPanel.<>c__DisplayClass55_0();
						CombatQuickUseItemSlot item = this.itemSlotArray[index];
						CombatQuickUseItemSlotData slotData = this._slotDataList[index];
						CombatQuickUseItemPanel.<>c__DisplayClass55_0 CS$<>8__locals2 = CS$<>8__locals1;
						Inventory inventory = slotData.Inventory;
						CS$<>8__locals2.firstKey = ((inventory != null && inventory.InventoryItemTotalCount > 0) ? slotData.Inventory.Items.Keys.First<ItemKey>() : ItemKey.Invalid);
						ItemDisplayData itemData = (CS$<>8__locals1.firstKey == ItemKey.Invalid) ? null : this._itemList.Find((ItemDisplayData d) => d.ContainsItemKey(CS$<>8__locals1.firstKey));
						int costWisdom = CS$<>8__locals1.firstKey.GetConsumedFeatureMedals();
						CombatSubProcessorCharacter processor;
						bool flag2 = this.Model.ProcessorCharacters.TryGetValue(this._currentCharId, out processor) && processor.UseItemCostNoWisdom;
						if (flag2)
						{
							costWisdom = 0;
						}
						bool costEnough = costWisdom <= this.TotalWisdomCount;
						string costColor = costEnough ? "brightblue" : "brightred";
						string itemTip = string.Empty;
						bool interactable = itemData != null && itemData.Amount > 0 && costEnough && CombatUseItemPanel.GetInteractable(itemData, out itemTip, this._canEatMore, this._canUseSwordFragment, this._weaponTricks, this.injury.Data);
						bool isShow = CS$<>8__locals1.firstKey.HasTemplate || index == totalValidCount;
						item.Refresh(index, isShow, interactable, CS$<>8__locals1.firstKey, itemData, this._wisdomIcon, costColor, new Action<int, ItemDisplayData>(this.OnClickButtonUse), new Action(this.OnClickButtonSetting), new Action<short>(this.OnEnterSlot), new Action(this.OnExitSlot));
					}
					this.canvasGroup.alpha = 1f;
				}
			});
		}

		// Token: 0x06008EE2 RID: 36578 RVA: 0x00428978 File Offset: 0x00426B78
		public CombatQuickUseItemSlot GetQuickUseItemSlot(ItemKey itemKey)
		{
			return this.itemSlotArray.FirstOrDefault((CombatQuickUseItemSlot slot) => slot.ItemKey.Id == itemKey.Id);
		}

		// Token: 0x06008EE3 RID: 36579 RVA: 0x004289A9 File Offset: 0x00426BA9
		private void OnEnterSlot(short distance)
		{
			this._onUpdateAttackRangePreview(new OuterAndInnerShorts(0, distance));
		}

		// Token: 0x06008EE4 RID: 36580 RVA: 0x004289BE File Offset: 0x00426BBE
		private void OnExitSlot()
		{
			this._onUpdateAttackRangePreview(new OuterAndInnerShorts(-1, -1));
		}

		// Token: 0x06008EE5 RID: 36581 RVA: 0x004289D3 File Offset: 0x00426BD3
		public bool CanCheckMouseExit()
		{
			return base.gameObject.activeSelf && !this.rootEquipment.activeSelf && !this.injury.IsSelectInjuryPart;
		}

		// Token: 0x06008EE6 RID: 36582 RVA: 0x00428A00 File Offset: 0x00426C00
		public bool Hide(bool playSound = true, bool executeHideEvent = true)
		{
			bool flag = this._openFrameCount <= 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				UIManager.Instance.HideUI(UIElement.PopupMenu);
				UIElement encyclopedia = UIElement.Encyclopedia;
				encyclopedia.OnActive = (Action)Delegate.Remove(encyclopedia.OnActive, new Action(this.OnOpenEncyclopedia));
				UIElement encyclopedia2 = UIElement.Encyclopedia;
				encyclopedia2.OnDeActive = (Action)Delegate.Remove(encyclopedia2.OnDeActive, new Action(this.OnCloseEncyclopedia));
				bool activeSelf = this.rootEquipment.activeSelf;
				if (activeSelf)
				{
					this._selectedItem = ItemKey.Invalid;
					this.rootEquipment.SetActive(false);
					this._subPanelClosedFrame = Time.frameCount;
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
						this.rootInjury.SetActive(false);
						this._subPanelClosedFrame = Time.frameCount;
						if (playSound)
						{
							AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
						}
						result = true;
					}
					else
					{
						bool flag2 = this._subPanelClosedFrame == Time.frameCount;
						if (flag2)
						{
							result = true;
						}
						else
						{
							if (playSound)
							{
								AudioManager.Instance.PlaySound("ui_default_small_back", false, false);
							}
							this.ApplyTargetCanvasesSortOrder(false);
							base.gameObject.SetActive(false);
							if (executeHideEvent)
							{
								this.ExecuteHideEvent();
							}
							result = false;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06008EE7 RID: 36583 RVA: 0x00428B80 File Offset: 0x00426D80
		private void ExecuteHideEvent()
		{
			Action onHide = this._onHide;
			if (onHide != null)
			{
				onHide();
			}
			this.Model.RaiseEvent(ECombatEvents.OnCirclePanelHide);
			this._openFrameCount = 0;
			this._pendingSetEscHandler = false;
			bool flag = UIManager.Instance.CheckEscHandler(this._cachedEscHandler);
			if (flag)
			{
				UIManager.Instance.SetEscHandler(null);
			}
			this._onHide = null;
		}

		// Token: 0x06008EE8 RID: 36584 RVA: 0x00428BE4 File Offset: 0x00426DE4
		private void OnDisable()
		{
			bool flag = this._cachedEscHandler != null && UIManager.Instance.CheckEscHandler(this._cachedEscHandler);
			if (flag)
			{
				UIManager.Instance.SetEscHandler(null);
			}
		}

		// Token: 0x17000F9F RID: 3999
		// (get) Token: 0x06008EE9 RID: 36585 RVA: 0x00428C1D File Offset: 0x00426E1D
		public bool IsOpened
		{
			get
			{
				return this._openFrameCount > 0;
			}
		}

		// Token: 0x06008EEA RID: 36586 RVA: 0x00428C28 File Offset: 0x00426E28
		private void Update()
		{
			bool flag = this._pendingSetEscHandler && this._openFrameCount > 1;
			if (flag)
			{
				this._pendingSetEscHandler = false;
				UIManager.Instance.SetEscHandler(this._cachedEscHandler);
			}
			bool flag2 = CommonCommandKit.RightMouse.Check(UIElement.Combat, false, false, false, true, false) || CommonCommandKit.Esc.Check(UIElement.Combat, false, false, false, true, false) || CombatCommandKit.OpenQuickWheel.Check(UIElement.Combat, false, false, false, true, false);
			if (flag2)
			{
				bool flag3 = this.Hide(true, true);
				if (flag3)
				{
					return;
				}
			}
			bool flag4 = this._openFrameCount > 0;
			if (flag4)
			{
				this._openFrameCount++;
			}
		}

		// Token: 0x06008EEB RID: 36587 RVA: 0x00428CDC File Offset: 0x00426EDC
		private void ApplyTargetCanvasesSortOrder(bool isApply)
		{
			bool flag = this.targetCanvases == null || this.targetCanvases.Length == 0;
			if (!flag)
			{
				if (isApply)
				{
					this._canvasSortOrderBackup.Clear();
					Canvas myCanvas = base.GetComponent<Canvas>();
					int targetOrder = (myCanvas != null) ? (myCanvas.sortingOrder - 1) : 0;
					foreach (Canvas canvas in this.targetCanvases)
					{
						bool flag2 = canvas != null;
						if (flag2)
						{
							this._canvasSortOrderBackup[canvas] = canvas.sortingOrder;
							canvas.sortingOrder = targetOrder;
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

		// Token: 0x06008EEC RID: 36588 RVA: 0x00428E08 File Offset: 0x00427008
		private void RefreshWisdomCount()
		{
			string totalCountStr = (this._specialWisdomCount == 0) ? this._wisdomCount.ToString() : string.Format("{0}+{1}", (int)(this._wisdomCount - this._specialWisdomCount), this._specialWisdomCount.ToString().SetColor("brightblue"));
			this.textWisdomCount.text = ((this._wisdomCost == 0) ? totalCountStr : (totalCountStr + (-this._wisdomCost).ToString().SetColor("brightred")).ColorReplace());
		}

		// Token: 0x06008EED RID: 36589 RVA: 0x00428E98 File Offset: 0x00427098
		private void OnClickButtonClose()
		{
			this.Hide(true, true);
		}

		// Token: 0x06008EEE RID: 36590 RVA: 0x00428EA4 File Offset: 0x004270A4
		private void OnClickButtonSetting()
		{
			ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("ItemList", this._itemList).SetObject("SlotList", this._slotDataList).Set("WisdomIcon", this._wisdomIcon).Set("CanEatMore", this._canEatMore).Set("CanUseSwordFragment", this._canUseSwordFragment).SetObject("WeaponTricks", this._weaponTricks).Set("CharId", this._currentCharId).Set("WisdomCount", this._wisdomCount).Set("SpecialWisdomCount", this._specialWisdomCount).SetObject("InjuryData", this.injury.Data);
			UIElement.SetCombatQuickUseItemSlot.SetOnInitArgs(args);
			UIElement setCombatQuickUseItemSlot = UIElement.SetCombatQuickUseItemSlot;
			setCombatQuickUseItemSlot.OnActive = (Action)Delegate.Combine(setCombatQuickUseItemSlot.OnActive, new Action(delegate()
			{
				this.Hide(false, false);
			}));
			UIElement setCombatQuickUseItemSlot2 = UIElement.SetCombatQuickUseItemSlot;
			setCombatQuickUseItemSlot2.OnDeActive = (Action)Delegate.Combine(setCombatQuickUseItemSlot2.OnDeActive, new Action(this.ExecuteHideEvent));
			UIManager.Instance.ShowUI(UIElement.SetCombatQuickUseItemSlot, true);
		}

		// Token: 0x06008EEF RID: 36591 RVA: 0x00428FC8 File Offset: 0x004271C8
		public void SelectItem(int index)
		{
			CombatQuickUseItemPanel.<>c__DisplayClass74_0 CS$<>8__locals1 = new CombatQuickUseItemPanel.<>c__DisplayClass74_0();
			bool flag = this._itemList == null || index < 0 || index >= this._itemList.Count;
			if (!flag)
			{
				CombatQuickUseItemSlotData slotData = this._slotDataList[index];
				CombatQuickUseItemPanel.<>c__DisplayClass74_0 CS$<>8__locals2 = CS$<>8__locals1;
				Inventory inventory = slotData.Inventory;
				CS$<>8__locals2.firstKey = ((inventory != null && inventory.InventoryItemTotalCount > 0) ? slotData.Inventory.Items.Keys.First<ItemKey>() : ItemKey.Invalid);
				ItemDisplayData itemData = (CS$<>8__locals1.firstKey == ItemKey.Invalid) ? null : this._itemList.Find((ItemDisplayData d) => d.ContainsItemKey(CS$<>8__locals1.firstKey));
				List<CombatItemActionInfo> itemActions = this.GetItemActions(index, itemData);
				bool flag2 = itemActions.Count == 0;
				if (!flag2)
				{
					itemActions[0].OnClick();
				}
			}
		}

		// Token: 0x06008EF0 RID: 36592 RVA: 0x004290A8 File Offset: 0x004272A8
		public List<CombatItemActionInfo> GetItemActions(int index, ItemDisplayData itemData)
		{
			List<CombatItemActionInfo> actions = new List<CombatItemActionInfo>();
			bool flag = itemData == null;
			List<CombatItemActionInfo> result;
			if (flag)
			{
				result = actions;
			}
			else
			{
				ItemKey itemKey = itemData.RealKey;
				bool isRecoverInjuryMainMedicineItem = CommonUtils.IsRecoverInjuryMainMedicineItem(itemKey);
				bool flag2 = isRecoverInjuryMainMedicineItem;
				if (flag2)
				{
					bool isSelectInjuryPart = this.injury.IsSelectInjuryPart;
					if (isSelectInjuryPart)
					{
						this.injury.ExitSelectInjuryPart();
						this.rootInjury.SetActive(false);
						return actions;
					}
				}
				ValueTuple<int, string> valueTuple = Injury.CheckEatSlot(this.injury.Data, itemKey, itemData.Amount);
				int count = valueTuple.Item1;
				string reason = valueTuple.Item2;
				bool canEatMore = count > 0;
				bool isWugKing = EatingItems.IsWugKing(itemKey);
				bool isMedicine = itemKey.ItemType == 8;
				bool isEat = CommonUtils.CanItemEat(itemKey.ItemType, itemKey.TemplateId, this._currentCharId);
				bool isTool = itemKey.ItemType == 6;
				bool flag3 = isMedicine;
				if (flag3)
				{
					MedicineItem medicineConfig = Medicine.Instance[itemKey.TemplateId];
					string tipContent;
					bool innerInteractable = !CombatUseItemPanel.CheckEatItemIsLocked(itemData, this.injury.Data, out tipContent);
					string btnName = CommonUtils.GetCanEatItemButtonName(itemKey);
					bool flag4 = isWugKing;
					Action onClick;
					if (flag4)
					{
						Action <>9__6;
						onClick = delegate()
						{
							int currentCharId = this._currentCharId;
							ITradeableContent itemData2 = itemData;
							int count2 = 1;
							Action onRequest;
							if ((onRequest = <>9__6) == null)
							{
								onRequest = (<>9__6 = delegate()
								{
									this.SetSelectedItem(itemData, 0);
								});
							}
							EatingUtils.TryRequestAddEatingItems(currentCharId, itemData2, count2, onRequest, true, null);
						};
					}
					else
					{
						bool flag5 = isRecoverInjuryMainMedicineItem;
						if (flag5)
						{
							Action<List<sbyte>> <>9__7;
							Action <>9__8;
							onClick = delegate()
							{
								this.rootInjury.SetActive(true);
								Injury injury = this.injury;
								ITradeableContent itemData2 = itemData;
								Action<List<sbyte>> onConfirm;
								if ((onConfirm = <>9__7) == null)
								{
									onConfirm = (<>9__7 = delegate(List<sbyte> partList)
									{
										this.rootInjury.SetActive(false);
										this._selectedInjuryPartList.Clear();
										this._selectedInjuryPartList.AddRange(partList);
										this.SetSelectedItem(itemData, 0);
									});
								}
								Action onCancel;
								if ((onCancel = <>9__8) == null)
								{
									onCancel = (<>9__8 = delegate()
									{
										this.rootInjury.SetActive(false);
									});
								}
								injury.EnterSelectInjuryPart(itemData2, onConfirm, onCancel);
							};
						}
						else
						{
							onClick = delegate()
							{
								this.SetSelectedItem(itemData, 0);
							};
						}
					}
					UnityAction onEnter = delegate()
					{
						this._wisdomCost = itemData.Key.GetConsumedFeatureMedals();
						this.RefreshWisdomCount();
					};
					UnityAction onExit = delegate()
					{
						this._wisdomCost = 0;
						this.RefreshWisdomCount();
						this.OnExitSlot();
					};
					actions.Add(new CombatItemActionInfo(btnName, innerInteractable, EItemMenuDisplayOrder.Eat, onClick, onEnter, onExit)
					{
						TipContent = tipContent
					});
					bool flag6 = medicineConfig.MaxUseDistance >= 0;
					if (flag6)
					{
						sbyte distance = medicineConfig.MaxUseDistance;
						actions.Add(new CombatItemActionInfo(LocalStringManager.Get(LanguageKey.LK_Throw_Poison), true, EItemMenuDisplayOrder.Other, delegate()
						{
							this.SetSelectedItem(itemData, 1);
						}, delegate()
						{
							this._wisdomCost = itemData.Key.GetConsumedFeatureMedals();
							this.RefreshWisdomCount();
							this.OnEnterSlot((short)distance);
						}, delegate()
						{
							this._wisdomCost = 0;
							this.RefreshWisdomCount();
							this.OnExitSlot();
						})
						{
							ThrowDistance = new short?((short)distance)
						});
					}
				}
				else
				{
					bool flag7 = isEat;
					if (flag7)
					{
						bool flag8 = ItemTemplateHelper.IsTianJieFuLu(itemKey.ItemType, itemKey.TemplateId);
						if (flag8)
						{
							bool innerInteractable2 = canEatMore;
							string tipContent2 = string.Empty;
							bool flag9 = !canEatMore;
							if (flag9)
							{
								tipContent2 = LocalStringManager.Get(LanguageKey.LK_Use_Medicine_Tip_NoSlot);
							}
							actions.Add(new CombatItemActionInfo(LocalStringManager.Get(LanguageKey.LK_Eat_Item), innerInteractable2, EItemMenuDisplayOrder.Eat, delegate()
							{
								this.SetSelectedItem(itemData, 0);
							}, delegate()
							{
								this._wisdomCost = itemData.Key.GetConsumedFeatureMedals();
								this.RefreshWisdomCount();
							}, delegate()
							{
								this._wisdomCost = 0;
								this.RefreshWisdomCount();
								this.OnExitSlot();
							})
							{
								TipContent = tipContent2
							});
						}
						else
						{
							bool isMisc = itemKey.ItemType == 12;
							bool neiliIsNotMax = this._equipCombatSkillMonitor.CurrNeili < this._equipCombatSkillMonitor.MaxNeili;
							bool innerInteractable3 = isMisc ? neiliIsNotMax : canEatMore;
							string tipContent3 = string.Empty;
							bool flag10 = !canEatMore;
							if (flag10)
							{
								tipContent3 = LocalStringManager.Get(LanguageKey.LK_Use_Medicine_Tip_NoSlot);
							}
							bool flag11 = isMisc && !neiliIsNotMax;
							if (flag11)
							{
								tipContent3 = LocalStringManager.Get(LanguageKey.LK_ItemTips_Use_NeiliIsMax);
							}
							actions.Add(new CombatItemActionInfo(LocalStringManager.Get(LanguageKey.LK_Eat_Item), innerInteractable3, EItemMenuDisplayOrder.Eat, delegate()
							{
								this.SetSelectedItem(itemData, 0);
							}, delegate()
							{
								this._wisdomCost = itemData.Key.GetConsumedFeatureMedals();
								this.RefreshWisdomCount();
							}, delegate()
							{
								this._wisdomCost = 0;
								this.RefreshWisdomCount();
								this.OnExitSlot();
							})
							{
								TipContent = tipContent3
							});
						}
					}
					else
					{
						bool flag12 = itemKey.ItemType == 12;
						if (flag12)
						{
							MiscItem miscConfig = Misc.Instance.GetItem(itemKey.TemplateId);
							bool flag13 = miscConfig.MaxUseDistance > 0;
							if (flag13)
							{
								sbyte distance = miscConfig.MaxUseDistance;
								actions.Add(new CombatItemActionInfo(LocalStringManager.Get(LanguageKey.LK_Throw_Poison), true, EItemMenuDisplayOrder.Other, delegate()
								{
									this.SetSelectedItem(itemData, 1);
								}, delegate()
								{
									this._wisdomCost = itemData.Key.GetConsumedFeatureMedals();
									this.RefreshWisdomCount();
									this.OnEnterSlot((short)distance);
								}, delegate()
								{
									this._wisdomCost = 0;
									this.RefreshWisdomCount();
									this.OnExitSlot();
								})
								{
									ThrowDistance = new short?((short)distance)
								});
							}
							else
							{
								bool flag14 = GameData.Domains.Combat.SharedConstValue.SwordFragment2BossId.ContainsKey(itemKey.TemplateId);
								if (flag14)
								{
									actions.Add(new CombatItemActionInfo(LocalStringManager.Get(LanguageKey.LK_Use_SwordFragment), true, EItemMenuDisplayOrder.Other, delegate()
									{
										this.SetSelectedItem(itemData, 0);
									}, delegate()
									{
										this._wisdomCost = itemData.Key.GetConsumedFeatureMedals();
										this.RefreshWisdomCount();
									}, delegate()
									{
										this._wisdomCost = 0;
										this.RefreshWisdomCount();
										this.OnExitSlot();
									}));
								}
								else
								{
									bool flag15 = CommonUtils.IsTianSuiBaoLuItem(itemKey.ItemType, itemKey.TemplateId);
									if (flag15)
									{
										actions.Add(new CombatItemActionInfo(LocalStringManager.Get(LanguageKey.LK_Use_TianSuiBaoLuItem), true, EItemMenuDisplayOrder.Other, delegate()
										{
											this.SetSelectedItem(itemData, 0);
										}, delegate()
										{
											this._wisdomCost = itemData.Key.GetConsumedFeatureMedals();
											this.RefreshWisdomCount();
										}, delegate()
										{
											this._wisdomCost = 0;
											this.RefreshWisdomCount();
											this.OnExitSlot();
										}));
									}
								}
							}
						}
						else
						{
							bool flag16 = isTool;
							if (flag16)
							{
								AsyncMethodCallbackDelegate <>9__28;
								AsyncMethodCallbackDelegate <>9__27;
								actions.Add(new CombatItemActionInfo(LocalStringManager.Get(LanguageKey.LK_Repair_Item), true, EItemMenuDisplayOrder.Tool, delegate()
								{
									this.SetSelectedItem(itemData, -1);
									IAsyncMethodRequestHandler requestHandler = null;
									int currentCharId = this._currentCharId;
									ItemKey itemKey = itemKey;
									AsyncMethodCallbackDelegate callback;
									if ((callback = <>9__27) == null)
									{
										callback = (<>9__27 = delegate(int offset, RawDataPool pool)
										{
											List<ItemKey> repairItemKeys = null;
											Serializer.Deserialize(pool, offset, ref repairItemKeys);
											IAsyncMethodRequestHandler requestHandler2 = null;
											List<ItemKey> itemKeyList = repairItemKeys;
											int currentCharId2 = this._currentCharId;
											sbyte itemSourceType = ItemSourceType.Inventory.ToSbyte();
											AsyncMethodCallbackDelegate callback2;
											if ((callback2 = <>9__28) == null)
											{
												callback2 = (<>9__28 = delegate(int offset2, RawDataPool pool2)
												{
													Serializer.Deserialize(pool2, offset2, ref this._repairItemDatas);
													this.equipmentListScroll.SetItemList(this._repairItemDatas);
													this.rootEquipment.SetActive(true);
												});
											}
											ItemDomainMethod.AsyncCall.GetItemDisplayDataListOptional(requestHandler2, itemKeyList, currentCharId2, itemSourceType, callback2);
										});
									}
									ItemDomainMethod.AsyncCall.GetRepairableItems(requestHandler, currentCharId, itemKey, callback);
								}, null, null));
							}
						}
					}
				}
				result = actions;
			}
			return result;
		}

		// Token: 0x06008EF1 RID: 36593 RVA: 0x00429650 File Offset: 0x00427850
		private void OnClickButtonUse(int index, ItemDisplayData itemData)
		{
			List<CombatItemActionInfo> actions = this.GetItemActions(index, itemData);
			bool flag = actions.Count == 0;
			if (!flag)
			{
				List<ViewPopupMenu.BtnData> sheetInfos = new List<ViewPopupMenu.BtnData>();
				foreach (CombatItemActionInfo actionInfo in actions)
				{
					ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(actionInfo.Name, actionInfo.Interactable, actionInfo.DisplayOrder, actionInfo.OnClick, actionInfo.OnEnter, actionInfo.OnExit, false);
					btnData.SetTip(string.Empty, actionInfo.TipContent);
					sheetInfos.Add(btnData);
				}
				bool flag2 = sheetInfos.Count > 0;
				if (flag2)
				{
					RectTransform itemRectTrans = this.itemSlotArray[index].transform as RectTransform;
					ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
					Vector3 itemScreenPos = UIManager.Instance.UiCamera.WorldToScreenPoint(itemRectTrans.position);
					Vector3 mouseScreenPos = Input.mousePosition;
					itemScreenPos.x = mouseScreenPos.x;
					argBox.SetObject("BtnInfo", sheetInfos);
					argBox.SetObject("ScreenPos", itemScreenPos);
					argBox.SetObject("ItemSize", itemRectTrans.rect.size);
					argBox.SetObject("TargetItem", itemData.Clone(-1));
					UIElement.PopupMenu.SetOnInitArgs(argBox);
					UIManager.Instance.ShowUI(UIElement.PopupMenu, true);
				}
			}
		}

		// Token: 0x06008EF2 RID: 36594 RVA: 0x004297E0 File Offset: 0x004279E0
		private void SetSelectedItem(ItemDisplayData itemData, sbyte useType = -1)
		{
			this.injury.HideNotice(false, true);
			this._wisdomCost = itemData.Key.GetConsumedFeatureMedals();
			this.RefreshWisdomCount();
			this._selectedItem = itemData.RealKey;
			this._useType = useType;
			bool flag = itemData.Key.ItemType != 6;
			if (flag)
			{
				this.OnConfirmSelect();
			}
		}

		// Token: 0x06008EF3 RID: 36595 RVA: 0x00429844 File Offset: 0x00427A44
		private void OnConfirmSelect()
		{
			this._onSelected(this._selectedItem, this._useType, this._selectedRepairItem, this._selectedInjuryPartList);
			bool activeSelf = this.rootEquipment.activeSelf;
			if (activeSelf)
			{
				this._selectedItem = ItemKey.Invalid;
				this.rootEquipment.SetActive(false);
			}
			bool isSelectInjuryPart = this.injury.IsSelectInjuryPart;
			if (isSelectInjuryPart)
			{
				this.injury.ExitSelectInjuryPart();
				this.rootInjury.SetActive(false);
			}
			this.Hide(false, true);
		}

		// Token: 0x06008EF4 RID: 36596 RVA: 0x004298D4 File Offset: 0x00427AD4
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

		// Token: 0x06008EF5 RID: 36597 RVA: 0x00429ACC File Offset: 0x00427CCC
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

		// Token: 0x04006CFF RID: 27903
		[SerializeField]
		private CanvasGroup canvasGroup;

		// Token: 0x04006D00 RID: 27904
		[SerializeField]
		private RectTransform root;

		// Token: 0x04006D01 RID: 27905
		[SerializeField]
		private CButton buttonClose;

		// Token: 0x04006D02 RID: 27906
		[SerializeField]
		private CButton buttonHide;

		// Token: 0x04006D03 RID: 27907
		[SerializeField]
		private CombatQuickUseItemSlot[] itemSlotArray;

		// Token: 0x04006D04 RID: 27908
		[SerializeField]
		private CButton buttonSetting;

		// Token: 0x04006D05 RID: 27909
		[SerializeField]
		private CImage imageWisdomIcon;

		// Token: 0x04006D06 RID: 27910
		[SerializeField]
		private TextMeshProUGUI textWisdomCount;

		// Token: 0x04006D07 RID: 27911
		[SerializeField]
		private Game.Components.Avatar.Avatar avatarSelf;

		// Token: 0x04006D08 RID: 27912
		[SerializeField]
		private Game.Components.Avatar.Avatar avatarEnemy;

		// Token: 0x04006D09 RID: 27913
		[SerializeField]
		private TextMeshProUGUI selfName;

		// Token: 0x04006D0A RID: 27914
		[SerializeField]
		private TextMeshProUGUI enemyName;

		// Token: 0x04006D0B RID: 27915
		[SerializeField]
		private ItemListScroll equipmentListScroll;

		// Token: 0x04006D0C RID: 27916
		[SerializeField]
		private GameObject rootEquipment;

		// Token: 0x04006D0D RID: 27917
		[SerializeField]
		private Injury injury;

		// Token: 0x04006D0E RID: 27918
		[SerializeField]
		private GameObject rootInjury;

		// Token: 0x04006D0F RID: 27919
		public CombatCirclePanelReserve circlePanelReserve;

		// Token: 0x04006D10 RID: 27920
		public CombatOtherActionHolder otherActionHolder;

		// Token: 0x04006D11 RID: 27921
		public TextMeshProUGUI selfInjuryCount;

		// Token: 0x04006D12 RID: 27922
		public TextMeshProUGUI enemyInjuryCount;

		// Token: 0x04006D13 RID: 27923
		public GameObject useItemEffect;

		// Token: 0x04006D14 RID: 27924
		[SerializeField]
		private Canvas[] targetCanvases;

		// Token: 0x04006D15 RID: 27925
		private readonly Dictionary<Canvas, int> _canvasSortOrderBackup = new Dictionary<Canvas, int>();

		// Token: 0x04006D16 RID: 27926
		private int _currentCharId;

		// Token: 0x04006D17 RID: 27927
		private string _wisdomIcon;

		// Token: 0x04006D18 RID: 27928
		private short _wisdomCount;

		// Token: 0x04006D19 RID: 27929
		private short _specialWisdomCount;

		// Token: 0x04006D1A RID: 27930
		private int _wisdomCost;

		// Token: 0x04006D1B RID: 27931
		private List<ItemDisplayData> _itemList = new List<ItemDisplayData>();

		// Token: 0x04006D1C RID: 27932
		private Action<ItemKey, sbyte, ItemKey, List<sbyte>> _onSelected;

		// Token: 0x04006D1D RID: 27933
		private ItemKey _selectedItem;

		// Token: 0x04006D1E RID: 27934
		private ItemKey _selectedRepairItem;

		// Token: 0x04006D1F RID: 27935
		private sbyte _useType;

		// Token: 0x04006D20 RID: 27936
		private bool _canEatMore;

		// Token: 0x04006D21 RID: 27937
		private bool _canUseSwordFragment;

		// Token: 0x04006D22 RID: 27938
		private sbyte[] _weaponTricks;

		// Token: 0x04006D23 RID: 27939
		private List<ItemDisplayData> _repairItemDatas = new List<ItemDisplayData>();

		// Token: 0x04006D24 RID: 27940
		private readonly List<sbyte> _selectedInjuryPartList = new List<sbyte>();

		// Token: 0x04006D25 RID: 27941
		private ResourceMonitor _resourceMonitor;

		// Token: 0x04006D26 RID: 27942
		private LifeSkillMonitor _lifeSkillMonitor;

		// Token: 0x04006D27 RID: 27943
		private EquipCombatSkillMonitor _equipCombatSkillMonitor;

		// Token: 0x04006D28 RID: 27944
		private Action _onHide;

		// Token: 0x04006D29 RID: 27945
		private Action<OuterAndInnerShorts> _onUpdateAttackRangePreview;

		// Token: 0x04006D2A RID: 27946
		private List<CombatQuickUseItemSlotData> _slotDataList;

		// Token: 0x04006D2B RID: 27947
		private CharacterAvatar _characterAvatarSelf;

		// Token: 0x04006D2C RID: 27948
		private CharacterAvatar _characterAvatarEnemy;

		// Token: 0x04006D2D RID: 27949
		private int _openFrameCount;

		// Token: 0x04006D2E RID: 27950
		private int _subPanelClosedFrame = -1;

		// Token: 0x04006D2F RID: 27951
		private Action _cachedEscHandler;

		// Token: 0x04006D30 RID: 27952
		private bool _pendingSetEscHandler;
	}
}
