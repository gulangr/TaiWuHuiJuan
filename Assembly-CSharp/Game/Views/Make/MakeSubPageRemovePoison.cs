using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using Game.Components.Item;
using Game.Components.ListStyleGeneralScroll.CellContent;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter.Item;
using Game.Components.SortAndFilter.Item.Apply;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Game.Views.Make
{
	// Token: 0x02000957 RID: 2391
	public class MakeSubPageRemovePoison : MakeSubPage
	{
		// Token: 0x17000CF1 RID: 3313
		// (get) Token: 0x060071AF RID: 29103 RVA: 0x0034D0F3 File Offset: 0x0034B2F3
		private int TaiwuCharId
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x17000CF2 RID: 3314
		// (get) Token: 0x060071B0 RID: 29104 RVA: 0x0034D0FF File Offset: 0x0034B2FF
		private ViewMake.ItemSourceTogKey CurMedicineTogKey
		{
			get
			{
				return (ViewMake.ItemSourceTogKey)this.medicineSourceToggleGroup.GetActiveIndex();
			}
		}

		// Token: 0x17000CF3 RID: 3315
		// (get) Token: 0x060071B1 RID: 29105 RVA: 0x0034D10C File Offset: 0x0034B30C
		private ItemSourceType CurMedicineItemSource
		{
			get
			{
				return ViewMake.GetItemSourceType(this.CurMedicineTogKey);
			}
		}

		// Token: 0x17000CF4 RID: 3316
		// (get) Token: 0x060071B2 RID: 29106 RVA: 0x0034D119 File Offset: 0x0034B319
		private ViewMake.ItemSourceTogKey CurTargetTogKey
		{
			get
			{
				return (ViewMake.ItemSourceTogKey)this.targetToggleGroup.GetActiveIndex();
			}
		}

		// Token: 0x17000CF5 RID: 3317
		// (get) Token: 0x060071B3 RID: 29107 RVA: 0x0034D126 File Offset: 0x0034B326
		private ItemSourceType CurTargetItemSource
		{
			get
			{
				return ViewMake.GetItemSourceType(this.CurTargetTogKey);
			}
		}

		// Token: 0x060071B4 RID: 29108 RVA: 0x0034D134 File Offset: 0x0034B334
		private void RefreshAllItemList()
		{
			this._allToolList.Clear();
			this._allMedicineList.Clear();
			this._allItemList.Clear();
			bool canTransferItemToWarehouse = this.DisplayData.CanTransferItemToWarehouse;
			if (canTransferItemToWarehouse)
			{
				this.<RefreshAllItemList>g__Add|47_0(this.DisplayData.InventoryItemList);
				this.<RefreshAllItemList>g__Add|47_0(this.DisplayData.EquippedItemList);
			}
			this.<RefreshAllItemList>g__Add|47_0(this.DisplayData.WarehouseItemList);
			this.<RefreshAllItemList>g__Add|47_0(this.DisplayData.TreasuryItemList);
		}

		// Token: 0x060071B5 RID: 29109 RVA: 0x0034D1C4 File Offset: 0x0034B3C4
		private void RefreshTargetList()
		{
			this._curSourceTargetList.Clear();
			foreach (ItemDisplayData target in this._allItemList)
			{
				bool flag = !this.CheckItemSourceMeet(target);
				if (!flag)
				{
					bool flag2 = !ItemTemplateHelper.IsPoisonable(target.Key.ItemType, target.Key.TemplateId);
					if (!flag2)
					{
						this._curSourceTargetList.Add(target);
					}
				}
			}
			this.targetListScroll.SetItemList(this._curSourceTargetList);
		}

		// Token: 0x060071B6 RID: 29110 RVA: 0x0034D274 File Offset: 0x0034B474
		private bool CheckItemSourceMeet(ItemDisplayData itemDisplayData)
		{
			return itemDisplayData.ItemSourceTypeEnum == this.CurTargetItemSource || (this.CurTargetItemSource == ItemSourceType.Inventory && itemDisplayData.ItemSourceTypeEnum == ItemSourceType.Equipment);
		}

		// Token: 0x060071B7 RID: 29111 RVA: 0x0034D2AC File Offset: 0x0034B4AC
		private void RefreshMedicineList()
		{
			this._curSourceMedicineList.Clear();
			bool shouldShowEmpty = !this.targetSlot.IsValid || !this.targetSlot.ItemData.HasAnyPoison || !this.targetSlot.ItemData.PoisonIsIdentified;
			bool flag = shouldShowEmpty;
			if (flag)
			{
				this.medicineListScroll.SetItemList(this._curSourceMedicineList);
			}
			else
			{
				foreach (ItemDisplayData target in this._allMedicineList)
				{
					bool flag2 = target.ItemSourceTypeEnum != this.CurMedicineItemSource;
					if (!flag2)
					{
						this._curSourceMedicineList.Add(target);
					}
				}
				List<ItemDisplayData> copyMedicineList = new List<ItemDisplayData>();
				for (int i = 0; i < this._curSourceMedicineList.Count; i++)
				{
					ItemDisplayData itemData = this._curSourceMedicineList[i];
					copyMedicineList.Add(itemData.Clone(-1));
				}
				for (int j = 0; j < this.medicineSlotList.Length; j++)
				{
					MakeTargetSlot slot = this.medicineSlotList[j];
					bool flag3 = !slot.IsValid;
					if (!flag3)
					{
						for (int k = copyMedicineList.Count - 1; k >= 0; k--)
						{
							ItemDisplayData itemData2 = copyMedicineList[k];
							bool flag4 = itemData2.Key.Equals(slot.ItemData.Key);
							if (flag4)
							{
								itemData2.Amount--;
								bool flag5 = itemData2.Amount == 0;
								if (flag5)
								{
									copyMedicineList.RemoveAt(k);
								}
							}
						}
					}
				}
				this.medicineListScroll.SetItemList(copyMedicineList);
			}
		}

		// Token: 0x060071B8 RID: 29112 RVA: 0x0034D494 File Offset: 0x0034B694
		private void RefreshAllData()
		{
			this.RefreshAllItemList();
			this.RefreshTargetList();
			this.RefreshMedicineList();
		}

		// Token: 0x060071B9 RID: 29113 RVA: 0x0034D4AC File Offset: 0x0034B6AC
		public override void Init(ViewMake parentView)
		{
			base.Init(parentView);
			this.ClearMedicineList();
			this.ClearTargetList();
			this.ResetPanelState();
			this.SetTargetCache(null);
			this._isShowEffectHandler = false;
			this.ParentView.ShowToolPanel(false);
			this.medicinePanel.gameObject.SetActive(false);
			this.goMaterialHolder.SetActive(false);
			this.toolSlot.gameObject.SetActive(false);
			this.targetSlot.Init(EMakeTargetSlotInteract.Always, EMakeTargetSlotType.RemovePoisonTarget, new Action<int, ItemDisplayData>(this.OnTargetSlotCancel), new Action(this.OnTargetSlotSelect), null, null, -1, null, false, null);
			this.targetSlot.SetEffectHandlerState(true);
			this.toolSlot.Init(EMakeTargetSlotInteract.Custom, EMakeTargetSlotType.Tool, new Action<int, ItemDisplayData>(this.OnCancelTool), delegate
			{
			}, new Func<bool>(this.GetToolInteractable), new Action<bool>(this.OnAutoSelectToolToggleChange), -1, null, false, null);
			this.toolSlot.IsToggleOn = SingletonObject.getInstance<GlobalSettings>().AutoSelectToolOnMake;
			foreach (MakeTargetSlot slot in this.medicineSlotList)
			{
				slot.Init(EMakeTargetSlotInteract.Custom, EMakeTargetSlotType.MakeMaterial, new Action<int, ItemDisplayData>(this.OnMedicineSlotCancel), delegate
				{
				}, new Func<bool>(this.GetMedicineInteractable), null, -1, null, true, null);
			}
			this.targetListScroll.SetCustomBuildGroup(new Action(this.TargetCustomBuildGroup), true);
			this.targetListScroll.Init("MakeSubPageRemovePoisonTarget", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnTargetListItemRender), new Action<ITradeableContent, RowItemLine>(this.OnTargetListItemClick), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.PoisonInfo, null, null, null);
			this.targetListScroll.SortAndFilterController.SetToggleVisible(EFilterLine.MainFilter.ToInt(), new List<int>
			{
				EMainFilterKeys.Food.ToInt(),
				EMainFilterKeys.Medicine.ToInt(),
				EMainFilterKeys.Equip.ToInt(),
				EMainFilterKeys.Book.ToInt()
			}, false);
			this.medicineListScroll.SetCustomBuildGroup(new Action(this.MedicineCustomBuildGroup), true);
			this.medicineListScroll.Init("MakeSubPageRemovePoisonMedicine", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnMedicineListItemRender), new Action<ITradeableContent, RowItemLine>(this.OnMedicineListItemClick), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount, null, null, null);
			this.medicineListScroll.SortAndFilterController.SetToggleVisible(EFilterLine.MainFilter.ToInt(), EMainFilterKeys.Medicine.ToInt());
			this.medicineListScroll.SortAndFilterController.SetToggleIsOnWithoutNotify(EFilterLine.MainFilter.ToInt(), EMainFilterKeys.Medicine.ToInt());
		}

		// Token: 0x060071BA RID: 29114 RVA: 0x0034D778 File Offset: 0x0034B978
		public override void Refresh(BuildingMakeDisplayData displayData)
		{
			base.Refresh(displayData);
			this.toolSlot.IsToggleOn = SingletonObject.getInstance<GlobalSettings>().AutoSelectToolOnMake;
			this.ParentView.RefreshSourceToggleInteractable(this.medicineSourceToggleGroup);
			this.ParentView.RefreshSourceToggleInteractable(this.targetToggleGroup);
			this.ClearToolList();
			this.RefreshAllData();
			this.CalcToolMaxFinalAttainment();
			this.ReloadSlot();
			this.RefreshInfo();
			UIElement.FullScreenMask.Hide(false);
		}

		// Token: 0x060071BB RID: 29115 RVA: 0x0034D7F7 File Offset: 0x0034B9F7
		private void OnAutoSelectToolToggleChange(bool isOn)
		{
			SingletonObject.getInstance<GlobalSettings>().AutoSelectToolOnMake = isOn;
			SingletonObject.getInstance<GlobalSettings>().SaveSettings();
		}

		// Token: 0x060071BC RID: 29116 RVA: 0x0034D814 File Offset: 0x0034BA14
		private void ReloadSlot()
		{
			bool flag = !this.targetSlot.IsValid;
			if (!flag)
			{
				bool flag2 = this.targetCache == null;
				if (flag2)
				{
					this.targetSlot.Cancel();
				}
				else
				{
					ItemDisplayData targetData = null;
					foreach (ItemDisplayData target in this._allItemList)
					{
						bool flag3 = target.RealKey == this.targetCache.RealKey;
						if (flag3)
						{
							targetData = target;
						}
					}
					bool flag4 = targetData == null;
					if (flag4)
					{
						this.targetSlot.Cancel();
					}
					else
					{
						this.targetSlot.Select(targetData, false);
						this.RefreshMedicineList();
						this.SetTargetCache(null);
						for (int i = 0; i < this.medicineSlotList.Length; i++)
						{
							MakeTargetSlot slot = this.medicineSlotList[i];
							slot.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.None);
							slot.Select(null, true);
							slot.HidePoison();
							bool flag5 = !targetData.PoisonIsIdentified;
							if (flag5)
							{
								slot.SwitchToPoison(true);
								slot.UnverifiedPoison();
							}
							else
							{
								bool flag6 = i >= targetData.PoisonEffects.PoisonSlotList.Count;
								if (flag6)
								{
									slot.HidePoison();
								}
								else
								{
									PoisonSlot poisonSlot = targetData.PoisonEffects.PoisonSlotList[i];
									bool isValid = poisonSlot.IsValid;
									if (isValid)
									{
										slot.ShowPoison(poisonSlot.GetPoisonType());
									}
									else
									{
										slot.HidePoison();
									}
								}
							}
						}
						bool flag7 = this.toolSlot == null || this.toolSlot.ItemData == null;
						if (flag7)
						{
							this.toolSlot.Cancel();
						}
						else
						{
							ItemDisplayData toolData = (this.toolSlot.ItemData == this.ParentView.EmptyTool) ? this.ParentView.EmptyTool : this.ParentView.AllToolList.Find((ItemDisplayData d) => d.RealKey == this.toolSlot.ItemData.RealKey && d.ItemSourceTypeEnum == this.toolSlot.ItemData.ItemSourceTypeEnum);
							bool flag8 = toolData == null;
							if (flag8)
							{
								this.toolSlot.Cancel();
							}
							else
							{
								this.SelectTool(toolData);
							}
						}
					}
				}
			}
		}

		// Token: 0x060071BD RID: 29117 RVA: 0x0034DA68 File Offset: 0x0034BC68
		private void ResetPanelState()
		{
			this.targetSlot.Select(null, false);
			this.toolSlot.Select(null, false);
			foreach (MakeTargetSlot slot in this.medicineSlotList)
			{
				slot.Select(null, false);
			}
		}

		// Token: 0x060071BE RID: 29118 RVA: 0x0034DAB8 File Offset: 0x0034BCB8
		private void Awake()
		{
			this.medicineSourceToggleGroup.OnActiveIndexChange += this.MedicineSourceToggleGroupOnOnActiveIndexChange;
			this.medicineSourceToggleGroup.Init(-1);
			this.targetToggleGroup.OnActiveIndexChange += this.TargetToggleGroupOnOnActiveIndexChange;
			this.targetToggleGroup.Init(-1);
			this.identityBtn.gameObject.SetActive(true);
			this.identityBtn.onClick.ResetListener(new Action(this.ConfirmIdentifyBtn));
			this.confirmBtn.onClick.ResetListener(new Action(this.ConfirmRemovePoison));
			this.extractToggle.onValueChanged.ResetListener(delegate(bool value)
			{
				this.RefreshInfo();
			});
			this.buttonCancelSelectTarget.ClearAndAddListener(delegate
			{
				this.targetPanel.gameObject.SetActive(false);
				this.ParentView.ExitFocusMode();
			});
			this.extractPanel.SetActive(false);
			PointerTrigger pointerTrigger = this.extractToggle.GetComponent<PointerTrigger>();
			pointerTrigger.EnterEvent.RemoveAllListeners();
			pointerTrigger.EnterEvent.AddListener(delegate()
			{
				this.extractPanel.SetActive(true);
			});
			pointerTrigger.ExitEvent.RemoveAllListeners();
			pointerTrigger.ExitEvent.AddListener(delegate()
			{
				this.extractPanel.SetActive(false);
			});
		}

		// Token: 0x060071BF RID: 29119 RVA: 0x0034DBF2 File Offset: 0x0034BDF2
		private void OnDestroy()
		{
			this.medicineSourceToggleGroup.OnActiveIndexChange -= this.MedicineSourceToggleGroupOnOnActiveIndexChange;
			this.targetToggleGroup.OnActiveIndexChange -= this.TargetToggleGroupOnOnActiveIndexChange;
		}

		// Token: 0x060071C0 RID: 29120 RVA: 0x0034DC28 File Offset: 0x0034BE28
		private void OnTargetListItemClick(ITradeableContent content, RowItemLine rowItemLine)
		{
			ItemDisplayData itemData = (ItemDisplayData)content;
			foreach (MakeTargetSlot slot in this.medicineSlotList)
			{
				slot.Refresh(true);
			}
			this.targetSlot.SetEffectHandlerState(false);
			this.targetPanel.gameObject.SetActive(false);
			this.medicinePanel.gameObject.SetActive(true);
			this.ParentView.ShowToolPanel(true);
			this.ParentView.ExitFocusMode();
			this.goMaterialHolder.SetActive(true);
			this.toolSlot.gameObject.SetActive(true);
			this.targetSlot.Select(itemData, false);
			this.toolSlot.Refresh(false);
			this.RefreshMedicineList();
			this.RefreshInfo();
			this.toolSlot.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.None);
			for (int i = 0; i < this.medicineSlotList.Length; i++)
			{
				MakeTargetSlot slot2 = this.medicineSlotList[i];
				slot2.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.None);
				bool flag = !itemData.HasAnyPoison;
				if (flag)
				{
					slot2.HidePoison();
				}
				else
				{
					bool flag2 = !itemData.PoisonIsIdentified;
					if (flag2)
					{
						slot2.SwitchToPoison(true);
						slot2.UnverifiedPoison();
					}
					else
					{
						bool flag3 = i >= itemData.PoisonEffects.PoisonSlotList.Count;
						if (flag3)
						{
							slot2.HidePoison();
						}
						else
						{
							PoisonSlot poisonSlot = itemData.PoisonEffects.PoisonSlotList[i];
							bool isValid = poisonSlot.IsValid;
							if (isValid)
							{
								slot2.ShowPoison(poisonSlot.GetPoisonType());
							}
							else
							{
								slot2.HidePoison();
							}
						}
					}
				}
			}
		}

		// Token: 0x060071C1 RID: 29121 RVA: 0x0034DDE2 File Offset: 0x0034BFE2
		private void RefreshInfo()
		{
			this.RefreshIdentifyInfo();
			this.ShowIdentifiedPoisonTip(false, null);
			this.RefreshPoisonInfo();
			this.RefreshSelectTargetTips();
			this.RefreshMixPoisonName();
			this.RefreshButton();
			this.RefreshExtractPoisonPreview();
			this.CheckCondition();
		}

		// Token: 0x060071C2 RID: 29122 RVA: 0x0034DE20 File Offset: 0x0034C020
		private void RefreshExtractPoisonPreview()
		{
			bool flag = !this.targetSlot.IsValid;
			if (!flag)
			{
				bool flag2 = this.targetSlot.ItemData.PoisonEffects == null;
				if (!flag2)
				{
					List<PoisonSlot> poisonSlotList = this.targetSlot.ItemData.PoisonEffects.PoisonSlotList;
					Dictionary<short, int> medicineCountDic = new Dictionary<short, int>();
					var source = from poisonSlot in poisonSlotList
					select poisonSlot.GetAllMedicineTemplateId(true) into templateId
					from id in templateId
					select new
					{
						templateId,
						id
					};
					Func<<>f__AnonymousType8<List<short>, short>, bool> <>9__3;
					var predicate;
					if ((predicate = <>9__3) == null)
					{
						predicate = (<>9__3 = (<>h__TransparentIdentifier0 => !medicineCountDic.TryAdd(<>h__TransparentIdentifier0.id, 1)));
					}
					foreach (short id2 in from <>h__TransparentIdentifier0 in source.Where(predicate)
					select <>h__TransparentIdentifier0.id)
					{
						Dictionary<short, int> medicineCountDic2 = medicineCountDic;
						short key = id2;
						int num = medicineCountDic2[key];
						medicineCountDic2[key] = num + 1;
					}
					CommonUtils.PrepareEnoughChildren(this.extractContent, this.extractPoisonItemTemplate.gameObject, medicineCountDic.Count, null);
					List<short> medicineTypeList = medicineCountDic.Keys.ToList<short>();
					for (int i = 0; i < medicineTypeList.Count; i++)
					{
						ItemBack itemBack = this.extractContent.GetChild(i).GetComponent<ItemBack>();
						short medicineId = medicineTypeList[i];
						MedicineItem medicineItem = Medicine.Instance[medicineId];
						itemBack.SetBack(medicineItem.Grade);
						itemBack.SetIcon(medicineItem.Icon);
						itemBack.SetCount(medicineCountDic[medicineId], false);
						itemBack.transform.GetChild(4).GetComponent<TextMeshProUGUI>().SetText(medicineItem.Name, true);
					}
				}
			}
		}

		// Token: 0x060071C3 RID: 29123 RVA: 0x0034E07C File Offset: 0x0034C27C
		private void RefreshButton()
		{
			this.identityBtn.gameObject.SetActive(this.targetSlot.IsValid);
			this.confirmBtn.gameObject.SetActive(this.targetSlot.IsValid);
			this.extractToggle.gameObject.SetActive(this.targetSlot.IsValid);
		}

		// Token: 0x060071C4 RID: 29124 RVA: 0x0034E0E0 File Offset: 0x0034C2E0
		private void OnTargetListItemRender(ITradeableContent content, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(content);
			rowItemLine.Set(rowItemMain, true);
			rowItemLine.SetInteractable(true, true);
			rowItemLine.SetDisabled(false);
			rowItemLine.SetSelected(content == this.targetSlot.ItemData);
		}

		// Token: 0x060071C5 RID: 29125 RVA: 0x0034E12C File Offset: 0x0034C32C
		private void TargetCustomBuildGroup()
		{
			List<ITradeableContent> havePoisonIdentifiedTargetList = new List<ITradeableContent>();
			List<ITradeableContent> poisonNotIdentifiedTargetList = new List<ITradeableContent>();
			List<ITradeableContent> notHavePoisonTargetList = new List<ITradeableContent>();
			foreach (ITradeableContent target in this.targetListScroll.FilteredData)
			{
				bool flag = !target.PoisonIsIdentified && target.HasAnyPoison;
				if (flag)
				{
					poisonNotIdentifiedTargetList.Add(target);
				}
				else
				{
					bool flag2 = target.PoisonIsIdentified && target.HasAnyPoison;
					if (flag2)
					{
						havePoisonIdentifiedTargetList.Add(target);
					}
					else
					{
						notHavePoisonTargetList.Add(target);
					}
				}
			}
			string title = LocalStringManager.Get(LanguageKey.LK_Remove_Poison_Target_Group_Can_Remove);
			this.targetListScroll.AddGroup(0, title, havePoisonIdentifiedTargetList, null, true);
			string title2 = LocalStringManager.Get(LanguageKey.LK_Remove_Poison_Target_Group_Not_Identify);
			this.targetListScroll.AddGroup(1, title2, poisonNotIdentifiedTargetList, null, true);
			string title3 = LocalStringManager.Get(LanguageKey.LK_Remove_Poison_Target_Group_Not_Poison);
			this.targetListScroll.AddGroup(2, title3, notHavePoisonTargetList, null, true);
		}

		// Token: 0x060071C6 RID: 29126 RVA: 0x0034E248 File Offset: 0x0034C448
		private void TargetToggleGroupOnOnActiveIndexChange(int newIndex, int oldIndex)
		{
			this.RefreshTargetList();
		}

		// Token: 0x060071C7 RID: 29127 RVA: 0x0034E252 File Offset: 0x0034C452
		private void ClearTargetList()
		{
			this._curSourceTargetList.Clear();
			this.targetListScroll.SetEmptyContent(string.Empty);
			this.targetListScroll.SetItemList(this._curSourceTargetList);
		}

		// Token: 0x060071C8 RID: 29128 RVA: 0x0034E284 File Offset: 0x0034C484
		private void OnTargetSlotSelect()
		{
			this.targetPanel.gameObject.SetActive(true);
			this.ParentView.EnterFocusMode(this.targetPanel.transform, null);
			this.targetListScroll.SetItemList(this._curSourceTargetList);
		}

		// Token: 0x060071C9 RID: 29129 RVA: 0x0034E2C4 File Offset: 0x0034C4C4
		private void OnTargetSlotCancel(int index, ItemDisplayData itemDisplayData)
		{
			foreach (MakeTargetSlot slot in this.medicineSlotList)
			{
				slot.Cancel();
			}
			this.toolSlot.Cancel();
			this.ClearMedicineList();
			this.medicinePanel.gameObject.SetActive(false);
			this.ParentView.ShowToolPanel(false);
			this.goMaterialHolder.SetActive(false);
			this.toolSlot.gameObject.SetActive(false);
			this.targetSlot.SetEffectHandlerState(true);
			this.RefreshInfo();
		}

		// Token: 0x060071CA RID: 29130 RVA: 0x0034E35A File Offset: 0x0034C55A
		private void RefreshSelectTargetTips()
		{
			this.selectTargetTips.gameObject.SetActive(!this.targetSlot.IsValid);
		}

		// Token: 0x060071CB RID: 29131 RVA: 0x0034E37C File Offset: 0x0034C57C
		private void OnMedicineListItemClick(ITradeableContent content, RowItemLine rowItemLine)
		{
			ItemDisplayData medicine = (ItemDisplayData)content;
			MedicineItem config = Medicine.Instance[medicine.RealKey.TemplateId];
			int slotIndex = this.GetMatchPoisonTypeSlotIndex(config.PoisonType);
			bool flag = slotIndex >= 0;
			if (flag)
			{
				this.medicineSlotList[slotIndex].Select(medicine, false);
				this.ParentView.RefreshToolList(this.GetRemovePoisonRequiredAttainment(), new List<sbyte>
				{
					this.ParentView.CurLifeSkillType
				}, new List<List<sbyte>>
				{
					new List<sbyte>
					{
						this.GetRemovePoisonMaxGrade()
					}
				}, this.toolSlot.ItemData, new Action<ItemDisplayData>(this.SelectTool), this.toolSlot.IsToggleOn, 1);
			}
			this.RefreshInfo();
			this.RefreshMedicineList();
			bool flag2 = !this.toolSlot.IsToggleOn;
			if (flag2)
			{
				this.toolSlot.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.Enable);
			}
			bool flag3 = !this._isShowEffectHandler;
			if (flag3)
			{
				this._isShowEffectHandler = (this.targetSlot.ItemData.HasAnyPoison && this.targetSlot.ItemData.PoisonIsIdentified);
				this.toolSlot.SetEffectHandlerState(this._isShowEffectHandler);
				this.leftLineGold.gameObject.SetActive(this._isShowEffectHandler);
			}
		}

		// Token: 0x060071CC RID: 29132 RVA: 0x0034E4D4 File Offset: 0x0034C6D4
		private int GetMatchPoisonTypeSlotIndex(sbyte poisonType)
		{
			bool flag = !this.targetSlot.IsValid || !this.targetSlot.ItemData.HasAnyPoison || !this.targetSlot.ItemData.PoisonIsIdentified;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				for (int i = 0; i < this.targetSlot.ItemData.PoisonEffects.PoisonSlotList.Count; i++)
				{
					PoisonSlot slot = this.targetSlot.ItemData.PoisonEffects.PoisonSlotList[i];
					bool flag2 = slot.GetPoisonType() == poisonType;
					if (flag2)
					{
						return i;
					}
				}
				result = -1;
			}
			return result;
		}

		// Token: 0x060071CD RID: 29133 RVA: 0x0034E580 File Offset: 0x0034C780
		private void OnMedicineListItemRender(ITradeableContent content, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(content);
			rowItemLine.Set(rowItemMain, true);
			bool canSelect = this.MedicineCanSelect((ItemDisplayData)content);
			rowItemLine.SetInteractable(canSelect, true);
			rowItemLine.SetDisabled(!canSelect);
		}

		// Token: 0x060071CE RID: 29134 RVA: 0x0034E5C8 File Offset: 0x0034C7C8
		private void MedicineCustomBuildGroup()
		{
			List<ITradeableContent> attainmentNotMeetMedicineList = new List<ITradeableContent>();
			List<ITradeableContent> gradeNotMeetMedicineList = new List<ITradeableContent>();
			List<ITradeableContent> itemTypeNotMeetMedicineList = new List<ITradeableContent>();
			List<ITradeableContent> meetMedicineList = new List<ITradeableContent>();
			bool flag = this._curSourceMedicineList.Count <= 0;
			if (!flag)
			{
				foreach (ITradeableContent medicine in this.medicineListScroll.FilteredData)
				{
					MedicineItem itemConfig = Medicine.Instance[medicine.Key.TemplateId];
					sbyte poisonType = itemConfig.PoisonType;
					short poisonLevel = itemConfig.EffectThresholdValue;
					sbyte poisonGrade = itemConfig.Grade;
					bool flag2 = !this.targetSlot.ItemData.PoisonEffects.PoisonSlotList.Exists((PoisonSlot s) => s.IsValid && s.MedicineConfig.PoisonType == poisonType);
					if (flag2)
					{
						itemTypeNotMeetMedicineList.Add(medicine);
					}
					else
					{
						bool flag3 = this.targetSlot.ItemData.PoisonEffects.PoisonSlotList.Exists((PoisonSlot s) => s.IsValid && s.MedicineConfig.PoisonType == poisonType && poisonGrade < s.MedicineConfig.Grade);
						if (flag3)
						{
							gradeNotMeetMedicineList.Add(medicine);
						}
						else
						{
							bool flag4 = this.GetMinNeedAttainment(poisonType) > this._toolMaxFinalAttainment;
							if (flag4)
							{
								attainmentNotMeetMedicineList.Add(medicine);
							}
							else
							{
								meetMedicineList.Add(medicine);
							}
						}
					}
				}
				string title = LocalStringManager.Get(LanguageKey.LK_Remove_Poison_Medicine_Group_Can_Remove);
				this.medicineListScroll.AddGroup(0, title, meetMedicineList, null, true);
				string title2 = LocalStringManager.Get(LanguageKey.LK_Remove_Poison_Medicine_Group_Type_Not_Match);
				this.medicineListScroll.AddGroup(1, title2, itemTypeNotMeetMedicineList, null, true);
				string title3 = LocalStringManager.Get(LanguageKey.LK_Remove_Poison_Medicine_Group_Grade_Not_Match);
				this.medicineListScroll.AddGroup(2, title3, gradeNotMeetMedicineList, null, true);
				string title4 = LocalStringManager.Get(LanguageKey.LK_Remove_Poison_Medicine_Group_Attainment_Not_Match);
				this.medicineListScroll.AddGroup(3, title4, attainmentNotMeetMedicineList, null, true);
			}
		}

		// Token: 0x060071CF RID: 29135 RVA: 0x0034E7D4 File Offset: 0x0034C9D4
		private bool MedicineCanSelect(ItemDisplayData medicine)
		{
			MedicineItem itemConfig = Medicine.Instance[medicine.Key.TemplateId];
			sbyte poisonType = itemConfig.PoisonType;
			short poisonLevel = itemConfig.EffectThresholdValue;
			sbyte poisonGrade = itemConfig.Grade;
			bool result = true;
			bool flag = !this.targetSlot.ItemData.PoisonEffects.PoisonSlotList.Exists((PoisonSlot s) => s.IsValid && s.MedicineConfig.PoisonType == poisonType);
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this.targetSlot.ItemData.PoisonEffects.PoisonSlotList.Exists((PoisonSlot s) => s.IsValid && s.MedicineConfig.PoisonType == poisonType && poisonGrade < s.MedicineConfig.Grade);
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = this.GetMinNeedAttainment(poisonType) > this._toolMaxFinalAttainment;
					if (flag3)
					{
						result = false;
					}
				}
			}
			return result;
		}

		// Token: 0x060071D0 RID: 29136 RVA: 0x0034E8AB File Offset: 0x0034CAAB
		private void ClearMedicineList()
		{
			this._curSourceMedicineList.Clear();
			this.medicineListScroll.SetEmptyContent(string.Empty);
			this.medicineListScroll.SetItemList(this._curSourceMedicineList);
			this.ClearToolList();
		}

		// Token: 0x060071D1 RID: 29137 RVA: 0x0034E8E4 File Offset: 0x0034CAE4
		private void OnMedicineSlotCancel(int index, ItemDisplayData itemDisplayData)
		{
			bool isAllEmpty = this.MedicineSlotAllEmpty();
			bool flag = isAllEmpty;
			if (flag)
			{
				this.toolSlot.Cancel();
				this.ClearToolList();
				this._isShowEffectHandler = false;
				this.toolSlot.SetEffectHandlerState(false);
				this.leftLineGold.gameObject.SetActive(false);
				this.rightLineGold.gameObject.SetActive(false);
			}
			this.RefreshMedicineList();
			this.ParentView.RefreshToolList(this.GetRemovePoisonRequiredAttainment(), new List<sbyte>
			{
				this.ParentView.CurLifeSkillType
			}, new List<List<sbyte>>
			{
				new List<sbyte>
				{
					this.GetRemovePoisonMaxGrade()
				}
			}, this.toolSlot.ItemData, new Action<ItemDisplayData>(this.SelectTool), this.toolSlot.IsToggleOn, 1);
			bool flag2 = isAllEmpty;
			if (flag2)
			{
				this.toolSlot.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.Disable);
			}
			bool flag3 = !this.toolSlot.IsValid && !isAllEmpty;
			if (flag3)
			{
				this.toolSlot.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.Enable);
			}
			for (int i = 0; i < this.medicineSlotList.Length; i++)
			{
				MakeTargetSlot slot = this.medicineSlotList[i];
				bool isValid = slot.IsValid;
				if (!isValid)
				{
					bool isValid2 = this.targetSlot.IsValid;
					if (isValid2)
					{
						slot.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.Enable);
					}
					else
					{
						slot.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.Disable);
					}
				}
			}
		}

		// Token: 0x060071D2 RID: 29138 RVA: 0x0034EA52 File Offset: 0x0034CC52
		private void ClearToolList()
		{
			this.ParentView.ClearToolList(LocalStringManager.Get(LanguageKey.LK_Make_Remove_Poison_Not_Selected).ColorReplace());
		}

		// Token: 0x060071D3 RID: 29139 RVA: 0x0034EA70 File Offset: 0x0034CC70
		private bool GetMedicineInteractable()
		{
			return this.targetSlot.IsValid;
		}

		// Token: 0x060071D4 RID: 29140 RVA: 0x0034EA7D File Offset: 0x0034CC7D
		private void MedicineSourceToggleGroupOnOnActiveIndexChange(int newIndex, int oldIndex)
		{
			this.RefreshMedicineList();
		}

		// Token: 0x060071D5 RID: 29141 RVA: 0x0034EA87 File Offset: 0x0034CC87
		private void RefreshPoisonInfo()
		{
			this.poisonInfos.Refresh(this.targetSlot.ItemData);
		}

		// Token: 0x060071D6 RID: 29142 RVA: 0x0034EAA4 File Offset: 0x0034CCA4
		private ItemKey[] GetMedicineItemKeyList()
		{
			return (from slot in this.medicineSlotList
			where slot.IsValid
			select slot.ItemData.RealKey).ToArray<ItemKey>();
		}

		// Token: 0x060071D7 RID: 29143 RVA: 0x0034EB0C File Offset: 0x0034CD0C
		private ItemDisplayData[] GetMedicineItemDisplayDataArray()
		{
			return (from slot in this.medicineSlotList
			where slot.IsValid
			select slot.ItemData).ToArray<ItemDisplayData>();
		}

		// Token: 0x060071D8 RID: 29144 RVA: 0x0034EB74 File Offset: 0x0034CD74
		private int GetMinNeedAttainment(sbyte poisonType)
		{
			int minValue = int.MaxValue;
			bool isValid = this.targetSlot.IsValid;
			if (isValid)
			{
				foreach (PoisonSlot slot in this.targetSlot.ItemData.PoisonEffects.PoisonSlotList)
				{
					bool flag = slot.GetPoisonType() != poisonType;
					if (!flag)
					{
						short requiredAttainment = this.ParentView.GetPoisonRequiredAttainment(slot.MedicineTemplateId, this.extractToggle.isOn);
						bool flag2 = (int)requiredAttainment < minValue;
						if (flag2)
						{
							minValue = (int)requiredAttainment;
						}
					}
				}
			}
			return minValue;
		}

		// Token: 0x060071D9 RID: 29145 RVA: 0x0034EC38 File Offset: 0x0034CE38
		private bool MedicineSlotAllEmpty()
		{
			bool result = true;
			for (int i = 0; i < this.medicineSlotList.Length; i++)
			{
				MakeTargetSlot slot = this.medicineSlotList[i];
				bool isValid = slot.IsValid;
				if (isValid)
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x060071DA RID: 29146 RVA: 0x0034EC80 File Offset: 0x0034CE80
		private sbyte GetRemovePoisonMaxGrade()
		{
			sbyte grade = 0;
			bool flag = !this.targetSlot.IsValid || !this.targetSlot.ItemData.HasAnyPoison || !this.targetSlot.ItemData.PoisonIsIdentified;
			sbyte result;
			if (flag)
			{
				result = grade;
			}
			else
			{
				List<short> poisonTemplateIds = this.targetSlot.ItemData.PoisonEffects.GetAllMedicineTemplateIds(false);
				for (int i = 0; i < this.medicineSlotList.Length; i++)
				{
					MakeTargetSlot slot = this.medicineSlotList[i];
					bool flag2 = !slot.IsValid;
					if (!flag2)
					{
						MedicineItem config = Medicine.Instance[slot.ItemData.Key.TemplateId];
						short targetPoisonId = poisonTemplateIds.Find((short id) => id > -1 && Medicine.Instance[id].PoisonType == config.PoisonType);
						sbyte curGrade = ItemTemplateHelper.GetGrade(8, targetPoisonId);
						grade = Math.Max(grade, curGrade);
					}
				}
				result = grade;
			}
			return result;
		}

		// Token: 0x060071DB RID: 29147 RVA: 0x0034ED7C File Offset: 0x0034CF7C
		private void CalcToolMaxFinalAttainment()
		{
			sbyte skillType = this.ParentView.CurLifeSkillType;
			short lifeSkillAttainment = this.DisplayData.LifeSkillAttainments.Get((int)skillType);
			this._toolMaxFinalAttainment = (int)ViewMake.GetFinalAttainment(54, lifeSkillAttainment, skillType);
			foreach (ItemDisplayData tool in this._allToolList)
			{
				short finalAttainment = ViewMake.GetFinalAttainment(tool.RealKey.TemplateId, lifeSkillAttainment, skillType);
				this._toolMaxFinalAttainment = Math.Max(this._toolMaxFinalAttainment, (int)finalAttainment);
			}
		}

		// Token: 0x060071DC RID: 29148 RVA: 0x0034EE24 File Offset: 0x0034D024
		private short GetLifeSkillTotalAttainment(sbyte type)
		{
			short attainment = this.DisplayData.LifeSkillAttainments.Get((int)type);
			bool flag = !this.toolSlot.IsValid;
			short result;
			if (flag)
			{
				result = attainment;
			}
			else
			{
				bool flag2 = this.IsEmptyTool();
				if (flag2)
				{
					short finalAttainment = ViewMake.GetFinalAttainment(this.toolSlot.ItemData.Key.TemplateId, attainment, this.ParentView.CurLifeSkillType);
					result = finalAttainment;
				}
				else
				{
					CraftToolItem toolConfig = CraftTool.Instance[this.toolSlot.ItemData.Key.TemplateId];
					bool flag3 = toolConfig != null && toolConfig.RequiredLifeSkillTypes.Contains(type);
					if (flag3)
					{
						short finalAttainment2 = ViewMake.GetFinalAttainment(this.toolSlot.ItemData.Key.TemplateId, attainment, this.ParentView.CurLifeSkillType);
						result = finalAttainment2;
					}
					else
					{
						this.toolSlot.SetEffectHandlerState(false);
						result = attainment;
					}
				}
			}
			return result;
		}

		// Token: 0x060071DD RID: 29149 RVA: 0x0034EF10 File Offset: 0x0034D110
		private bool IsEmptyTool()
		{
			return this.toolSlot.IsValid && this.toolSlot.ItemData.Key == this.DisplayData.EmptyToolKey;
		}

		// Token: 0x060071DE RID: 29150 RVA: 0x0034EF52 File Offset: 0x0034D152
		private void SelectTool(ItemDisplayData itemData)
		{
			this.toolSlot.Select(itemData, false);
			this.CheckCondition();
		}

		// Token: 0x060071DF RID: 29151 RVA: 0x0034EF6A File Offset: 0x0034D16A
		private void OnCancelTool(int index, ItemDisplayData itemDisplayData)
		{
			this.ParentView.RerenderToolList(this.targetSlot.ItemData);
			this.CheckCondition();
		}

		// Token: 0x060071E0 RID: 29152 RVA: 0x0034EF8B File Offset: 0x0034D18B
		private bool GetToolInteractable()
		{
			return this.targetSlot.IsValid;
		}

		// Token: 0x060071E1 RID: 29153 RVA: 0x0034EF98 File Offset: 0x0034D198
		private void ShowUnidentifiedPoisonTip(bool show, LanguageKey key = LanguageKey.LK_Remove_Poison_Has_Unidentified)
		{
			this.identityTips.transform.parent.gameObject.SetActive(show);
			bool flag = !show;
			if (!flag)
			{
				this.identityTips.SetText(LocalStringManager.Get(key), true);
			}
		}

		// Token: 0x060071E2 RID: 29154 RVA: 0x0034EFE0 File Offset: 0x0034D1E0
		private void ShowIdentifiedPoisonTip(bool show, Action action)
		{
			this._curIdentifiedResultAction = action;
			GameObject tip = this.identifiedPoisonTip;
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

		// Token: 0x060071E3 RID: 29155 RVA: 0x0034F090 File Offset: 0x0034D290
		private void AnimationStateOnEnd(TrackEntry trackEntry)
		{
			GameObject tip = this.identifiedPoisonTip;
			TextMeshProUGUI text = tip.GetComponentInChildren<TextMeshProUGUI>();
			LanguageKey key = (!this._curIdentifySuccess) ? LanguageKey.LK_Poison_Identify_LifeSkill_NotMeet : (this._curIdentifiedResultHasPoison ? LanguageKey.LK_Poison_Identify_HasPoison : LanguageKey.LK_Poison_Identify_NoPoison);
			text.SetText(LocalStringManager.Get(key).ColorReplace(), true);
			Action curIdentifiedResultAction = this._curIdentifiedResultAction;
			if (curIdentifiedResultAction != null)
			{
				curIdentifiedResultAction();
			}
		}

		// Token: 0x060071E4 RID: 29156 RVA: 0x0034F0F8 File Offset: 0x0034D2F8
		private void RefreshIdentifyInfo()
		{
			int leftDays = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
			int needleAmount = this.GetNeedleCount();
			string title = LocalStringManager.Get(LanguageKey.LK_Poison_Identify);
			bool interactable = false;
			bool flag = !this.targetSlot.IsValid;
			string content;
			if (flag)
			{
				content = LocalStringManager.Get(LanguageKey.LK_Poison_Identify_No_Target);
			}
			else
			{
				bool poisonIsIdentified = this.targetSlot.ItemData.PoisonIsIdentified;
				if (poisonIsIdentified)
				{
					content = LocalStringManager.Get(LanguageKey.LK_Poison_Identified);
				}
				else
				{
					interactable = (this.GetNeedleCount() > 0 && leftDays > 1);
					bool timeIsMeet = leftDays >= 1;
					bool itemIsMeet = needleAmount >= 1;
					string timeStr = leftDays.ToString().SetColor(timeIsMeet ? "brightblue" : "brightred");
					string itemStr = needleAmount.ToString().SetColor(itemIsMeet ? "brightblue" : "brightred");
					content = LocalStringManager.GetFormat(LanguageKey.LK_Poison_Identify_Tip, itemStr, timeStr);
				}
			}
			bool showTip = this.targetSlot.IsValid && !this.targetSlot.ItemData.PoisonIsIdentified && this.targetSlot.ItemData.HasAnyPoison;
			this.ShowUnidentifiedPoisonTip(showTip, LanguageKey.LK_Remove_Poison_Has_Unidentified);
			this.identityBtnTip.Type = TipType.Simple;
			bool flag2 = this.identityBtnTip.PresetParam == null || this.identityBtnTip.PresetParam.Length < 2;
			if (flag2)
			{
				this.identityBtnTip.PresetParam = new string[2];
			}
			this.identityBtnTip.PresetParam[0] = title;
			this.identityBtnTip.PresetParam[1] = content;
			this.identityBtn.interactable = interactable;
		}

		// Token: 0x060071E5 RID: 29157 RVA: 0x0034F28D File Offset: 0x0034D48D
		private void ConfirmIdentifyBtn()
		{
			UIElement.FullScreenMask.Show();
			ItemDomainMethod.AsyncCall.IdentifyPoisons(null, this.TaiwuCharId, this.targetSlot.ItemData, delegate(int offset, RawDataPool dataPool)
			{
				List<ItemDisplayData> resultList = null;
				Serializer.Deserialize(dataPool, offset, ref resultList);
				this._curIdentifySuccess = (resultList != null && resultList.Count > 0);
				bool curIdentifySuccess = this._curIdentifySuccess;
				if (curIdentifySuccess)
				{
					this.SetTargetCache(resultList[0]);
				}
				else
				{
					this.SetTargetCache(this.targetSlot.ItemData);
				}
				bool curIdentifiedResultHasPoison;
				if (this._curIdentifySuccess)
				{
					curIdentifiedResultHasPoison = resultList.Any((ItemDisplayData d) => d.HasAnyPoison);
				}
				else
				{
					curIdentifiedResultHasPoison = false;
				}
				this._curIdentifiedResultHasPoison = curIdentifiedResultHasPoison;
				Action <>9__3;
				this.ShowIdentifiedPoisonTip(true, delegate
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
							this._curIdentifySuccess = false;
							this._curIdentifiedResultHasPoison = false;
							this.DisplayData.Clear();
							this.ParentView.RequestData();
						});
					}
					instance.DelaySecondsDo(sec, job);
				});
			});
		}

		// Token: 0x060071E6 RID: 29158 RVA: 0x0034F2C0 File Offset: 0x0034D4C0
		private int GetNeedleCount()
		{
			List<ItemDisplayData> items = this._allItemList.FindAll(new Predicate<ItemDisplayData>(MakeSubPageRemovePoison.<GetNeedleCount>g__Match|101_0));
			int totalCount = 0;
			foreach (ItemDisplayData item in items)
			{
				totalCount += item.Amount;
			}
			return totalCount;
		}

		// Token: 0x060071E7 RID: 29159 RVA: 0x0034F338 File Offset: 0x0034D538
		private void RefreshMixPoisonName()
		{
			bool flag;
			if (this.targetSlot.IsValid)
			{
				FullPoisonEffects poisonEffects = this.targetSlot.ItemData.PoisonEffects;
				flag = (poisonEffects != null && poisonEffects.IsThreeMixed);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				this.ShowPreviewPoison(this.targetSlot.ItemData.PoisonEffects);
			}
			else
			{
				this.mixedPoisonTip.gameObject.SetActive(false);
			}
		}

		// Token: 0x060071E8 RID: 29160 RVA: 0x0034F3A4 File Offset: 0x0034D5A4
		private void ShowPreviewPoison(FullPoisonEffects poisonEffects)
		{
			this.mixedPoisonTip.gameObject.SetActive(true);
			short medicineTemplateId = poisonEffects.GetMixedMedicineTemplateId();
			string medicineName = Medicine.Instance[medicineTemplateId].Name;
			this.mixedPoisonName.text = medicineName;
			this.mixedPoisonTip.Type = TipType.MixPoison;
			this.mixedPoisonTip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("MedicineId", medicineTemplateId);
		}

		// Token: 0x060071E9 RID: 29161 RVA: 0x0034F414 File Offset: 0x0034D614
		private bool CheckSkillAttainment()
		{
			int requiredAttainment = this.GetRemovePoisonRequiredAttainment();
			short totalAttainment = this.GetLifeSkillTotalAttainment(this.ParentView.CurLifeSkillType);
			sbyte skillType = this.ParentView.CurLifeSkillType;
			LifeSkillTypeItem lifeSkillConfig = Config.LifeSkillType.Instance[skillType];
			string finalAttainmentColor = ((int)totalAttainment >= requiredAttainment) ? "brightblue" : "brightred";
			string attainmentStr = string.Format("{0}/{1}", totalAttainment.ToString().SetColor(finalAttainmentColor), requiredAttainment);
			this.propertyNeedAttainment.Set(lifeSkillConfig.Icon, lifeSkillConfig.Name, attainmentStr, null, false);
			this.propertyNeedAttainment.gameObject.SetActive(this.toolSlot.IsValid && requiredAttainment > 0);
			this.RefreshAttainmentTip(this.needAttainmentTip);
			return (int)totalAttainment >= requiredAttainment;
		}

		// Token: 0x060071EA RID: 29162 RVA: 0x0034F4F4 File Offset: 0x0034D6F4
		private int GetRemovePoisonRequiredAttainment()
		{
			int maxRequiredAttainment = -1;
			bool flag = !this.targetSlot.IsValid || !this.targetSlot.ItemData.HasAnyPoison || !this.targetSlot.ItemData.PoisonIsIdentified;
			int result;
			if (flag)
			{
				result = maxRequiredAttainment;
			}
			else
			{
				for (int i = 0; i < this.targetSlot.ItemData.PoisonEffects.PoisonSlotList.Count; i++)
				{
					PoisonSlot slot = this.targetSlot.ItemData.PoisonEffects.PoisonSlotList[i];
					short requiredAttainment = this.ParentView.GetPoisonRequiredAttainment(slot.MedicineTemplateId, this.extractToggle.isOn);
					bool flag2 = (int)requiredAttainment > maxRequiredAttainment;
					if (flag2)
					{
						maxRequiredAttainment = (int)requiredAttainment;
					}
				}
				result = maxRequiredAttainment;
			}
			return result;
		}

		// Token: 0x060071EB RID: 29163 RVA: 0x0034F5C0 File Offset: 0x0034D7C0
		private void RefreshAttainmentTip(TooltipInvoker tipDisplayer)
		{
			bool flag = !this.toolSlot.IsValid;
			if (flag)
			{
				tipDisplayer.enabled = false;
			}
			else
			{
				bool isEmptyTool = this.IsEmptyTool();
				CraftToolItem toolConfig = CraftTool.Instance[this.toolSlot.ItemData.Key.TemplateId];
				tipDisplayer.enabled = true;
				List<HealAttainmentTipsHelper.AttainmentItem> attainmentItems = EasyPool.Get<List<HealAttainmentTipsHelper.AttainmentItem>>();
				attainmentItems.Clear();
				for (int index = 0; index < GameData.Domains.Character.LifeSkillType.CraftingTypes.Length; index++)
				{
					sbyte tipSkillType = GameData.Domains.Character.LifeSkillType.CraftingTypes[index];
					bool flag2 = isEmptyTool && tipSkillType != this.ParentView.CurLifeSkillType;
					if (!flag2)
					{
						bool flag3 = !toolConfig.RequiredLifeSkillTypes.Contains(tipSkillType);
						if (!flag3)
						{
							short toolAttainment = UI_Make.GetToolAttainment(this.toolSlot.ItemData.Key.TemplateId, this.DisplayData.LifeSkillAttainments.Get((int)tipSkillType), tipSkillType);
							int delta = (int)(toolConfig.RequiredLifeSkillTypes.Contains(tipSkillType) ? toolAttainment : 0);
							attainmentItems.Add(new HealAttainmentTipsHelper.AttainmentItem
							{
								SkillType = tipSkillType,
								DeltaAttainment = delta,
								Attainment = (int)this.DisplayData.LifeSkillAttainments.Get((int)tipSkillType)
							});
						}
					}
				}
				bool flag4 = attainmentItems.Count > 0;
				if (flag4)
				{
					HealAttainmentTipsHelper.RefreshTips(tipDisplayer, this.toolSlot.ItemData.Key, isEmptyTool, attainmentItems, false);
				}
				EasyPool.Free<List<HealAttainmentTipsHelper.AttainmentItem>>(attainmentItems);
			}
		}

		// Token: 0x060071EC RID: 29164 RVA: 0x0034F748 File Offset: 0x0034D948
		private void SetTargetCache(ItemDisplayData cache)
		{
			this.targetCache = cache;
		}

		// Token: 0x060071ED RID: 29165 RVA: 0x0034F754 File Offset: 0x0034D954
		private void CheckCondition()
		{
			bool attainmentMeet = this.CheckSkillAttainment();
			bool toolMeet = this.CheckTool();
			this.RefreshConfirmBtn(this.targetSlot.IsValid && this.toolSlot.IsValid && attainmentMeet && toolMeet && !this.MedicineSlotAllEmpty());
			this.RefreshConfirmMouseTip(attainmentMeet);
			this.rightLineGold.gameObject.SetActive(this.toolSlot.IsValid);
		}

		// Token: 0x060071EE RID: 29166 RVA: 0x0034F7C8 File Offset: 0x0034D9C8
		private bool CheckTool()
		{
			bool isToolMeet = false;
			bool hasTool = this.toolSlot.IsValid;
			bool flag = hasTool && this.targetSlot.IsValid;
			if (flag)
			{
				int totalCost = this.GetMaxToolDurabilityCost();
				short curDurability = this.toolSlot.ItemData.Durability;
				short maxDurability = this.toolSlot.ItemData.MaxDurability;
				isToolMeet = ((int)curDurability >= totalCost);
				string curDurabilityColor = isToolMeet ? "brightblue" : "brightred";
				string costDurabilityStr = string.Format("-{0}", totalCost);
				string durabilityStr = string.Format("{0}{1}/{2}", curDurability.ToString().SetColor(curDurabilityColor), costDurabilityStr, maxDurability);
				this.propertyToolDurability.SetValue(durabilityStr);
				sbyte skillType = this.ParentView.CurLifeSkillType;
				LifeSkillTypeItem lifeSkillConfig = Config.LifeSkillType.Instance[skillType];
				short lifeSkillAttainment = this.DisplayData.LifeSkillAttainments.Get((int)skillType);
				short toolAttainment = UI_Make.GetToolAttainment(this.toolSlot.ItemData.RealKey.TemplateId, lifeSkillAttainment, skillType);
				string toolAttainmentText = (toolAttainment >= 0) ? string.Format("+{0}", toolAttainment) : toolAttainment.ToString().SetColor("brightred");
				this.propertyToolAttainment.Set(lifeSkillConfig.Icon, lifeSkillConfig.Name, toolAttainmentText, null, false);
			}
			this.propertyToolDurability.gameObject.SetActive(hasTool);
			this.propertyToolAttainment.gameObject.SetActive(hasTool);
			return isToolMeet;
		}

		// Token: 0x060071EF RID: 29167 RVA: 0x0034F954 File Offset: 0x0034DB54
		private int GetMaxToolDurabilityCost()
		{
			bool flag = !this.toolSlot.IsValid || !this.targetSlot.IsValid || !this.targetSlot.ItemData.HasAnyPoison || !this.targetSlot.ItemData.PoisonIsIdentified;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				sbyte grade = this.GetRemovePoisonMaxGrade();
				short cost = ViewMake.GetToolDurabilityCost(this.toolSlot.ItemData, grade);
				result = (int)cost;
			}
			return result;
		}

		// Token: 0x060071F0 RID: 29168 RVA: 0x0034F9CA File Offset: 0x0034DBCA
		private void RefreshConfirmBtn(bool interactable)
		{
			this.confirmBtn.interactable = interactable;
		}

		// Token: 0x060071F1 RID: 29169 RVA: 0x0034F9DC File Offset: 0x0034DBDC
		private void ConfirmRemovePoison()
		{
			UIElement.FullScreenMask.Show();
			this.RefreshConfirmBtn(false);
			ItemKey[] medicineItemKeyList = this.GetMedicineItemKeyList();
			BuildingDomainMethod.AsyncCall.CheckRemovePoisonCondition(null, this.TaiwuCharId, this.toolSlot.ItemData.Key, this.targetSlot.ItemData.Key, medicineItemKeyList, this.ParentView.BuildingBlockKey, this.extractToggle.isOn, delegate(int offset, RawDataPool dataPool)
			{
				bool isMeet = false;
				Serializer.Deserialize(dataPool, offset, ref isMeet);
				bool flag = !isMeet;
				if (flag)
				{
					UIElement.FullScreenMask.Hide(false);
				}
				else
				{
					BuildingDomainMethod.AsyncCall.RemoveItemPoison(null, this.TaiwuCharId, this.toolSlot.ItemData, this.targetSlot.ItemData, this.GetMedicineItemDisplayDataArray(), this.extractToggle.isOn, delegate(int offset2, RawDataPool dataPool2)
					{
						MakeSubPageRemovePoison.<>c__DisplayClass113_0 CS$<>8__locals1;
						CS$<>8__locals1.result = new ValueTuple<bool, List<ItemDisplayData>>(false, new List<ItemDisplayData>());
						Serializer.Deserialize(dataPool2, offset2, ref CS$<>8__locals1.result);
						this.<ConfirmRemovePoison>g__Action|113_2(ref CS$<>8__locals1);
					});
				}
			});
		}

		// Token: 0x060071F2 RID: 29170 RVA: 0x0034FA54 File Offset: 0x0034DC54
		private void RefreshConfirmMouseTip(bool attainmentMeet)
		{
			this.sb.Clear();
			bool flag = !this.targetSlot.IsValid;
			if (flag)
			{
				this.StringBuilderAppend(this.sb, LocalStringManager.Get(LanguageKey.LK_Remove_Poison_Item_Not_Selected));
			}
			bool flag2 = this.MedicineSlotAllEmpty();
			if (flag2)
			{
				this.StringBuilderAppend(this.sb, LocalStringManager.Get(LanguageKey.LK_Remove_Poison_Material_Not_Selected));
			}
			bool flag3 = !this.toolSlot.IsValid;
			if (flag3)
			{
				this.StringBuilderAppend(this.sb, LocalStringManager.Get(LanguageKey.LK_Making_Tool_Not_Selected));
			}
			bool flag4 = !attainmentMeet;
			if (flag4)
			{
				this.StringBuilderAppend(this.sb, LocalStringManager.Get(LanguageKey.LK_Making_Attainment_Not_Enough));
			}
			this.confirmTip.Type = TipType.SingleDesc;
			TooltipInvoker tooltipInvoker = this.confirmTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			this.confirmTip.RuntimeParam.Set("arg0", this.sb.ToString());
			this.confirmTip.enabled = !this.confirmBtn.interactable;
		}

		// Token: 0x060071F3 RID: 29171 RVA: 0x0034FB68 File Offset: 0x0034DD68
		private void StringBuilderAppend(StringBuilder sb, string text)
		{
			bool flag = sb.Length > 0;
			if (flag)
			{
				sb.Append("\n");
			}
			sb.Append(text);
		}

		// Token: 0x060071F5 RID: 29173 RVA: 0x0034FC00 File Offset: 0x0034DE00
		[CompilerGenerated]
		private void <RefreshAllItemList>g__Add|47_0(List<ItemDisplayData> list)
		{
			bool flag = list == null;
			if (!flag)
			{
				foreach (ItemDisplayData itemData in list)
				{
					sbyte itemType = itemData.RealKey.ItemType;
					sbyte b = itemType;
					if (b != 6)
					{
						if (b == 8)
						{
							MedicineItem config = Medicine.Instance[itemData.RealKey.TemplateId];
							bool flag2 = config.EffectType == EMedicineEffectType.DetoxPoison;
							if (flag2)
							{
								this._allMedicineList.Add(itemData);
							}
						}
					}
					else
					{
						CraftToolItem config2 = CraftTool.Instance[itemData.RealKey.TemplateId];
						bool flag3 = config2.RequiredLifeSkillTypes.Contains(this.ParentView.CurLifeSkillType);
						if (flag3)
						{
							this._allToolList.Add(itemData);
						}
					}
					this._allItemList.Add(itemData);
				}
			}
		}

		// Token: 0x060071FC RID: 29180 RVA: 0x0034FE58 File Offset: 0x0034E058
		[CompilerGenerated]
		internal static bool <GetNeedleCount>g__Match|101_0(ItemDisplayData i)
		{
			ItemKey key = i.Key;
			return key.ItemType == 12 && key.TemplateId == 264;
		}

		// Token: 0x060071FF RID: 29183 RVA: 0x0034FF38 File Offset: 0x0034E138
		[CompilerGenerated]
		private void <ConfirmRemovePoison>g__Action|113_2(ref MakeSubPageRemovePoison.<>c__DisplayClass113_0 A_1)
		{
			bool item = A_1.result.Item1;
			if (item)
			{
				this.SetTargetCache(A_1.result.Item2[0]);
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.SetObject("ItemList", A_1.result.Item2);
				argBox.Set("ObtainType", 12);
				UIElement.GetItem.SetOnInitArgs(argBox);
				UIManager.Instance.MaskUI(UIElement.GetItem);
				this.toolSlot.Cancel();
				this.ClearToolList();
				this.DisplayData.Clear();
				this.ParentView.RequestData();
				bool isShowEffectHandler = this._isShowEffectHandler;
				if (isShowEffectHandler)
				{
					this._isShowEffectHandler = false;
					this.toolSlot.SetEffectHandlerState(false);
					this.leftLineGold.gameObject.SetActive(false);
					this.rightLineGold.gameObject.SetActive(false);
				}
			}
			else
			{
				this.ShowUnidentifiedPoisonTip(true, LanguageKey.LK_Remove_Poison_Material_Unidentified);
			}
		}

		// Token: 0x0400545A RID: 21594
		[SerializeField]
		private MakeTargetSlot[] medicineSlotList = new MakeTargetSlot[FullPoisonEffects.MaxSlotCount];

		// Token: 0x0400545B RID: 21595
		[SerializeField]
		private MakeTargetSlot targetSlot;

		// Token: 0x0400545C RID: 21596
		[SerializeField]
		private MakeTargetSlot toolSlot;

		// Token: 0x0400545D RID: 21597
		[SerializeField]
		private GameObject goMaterialHolder;

		// Token: 0x0400545E RID: 21598
		[SerializeField]
		private CToggleGroup medicineSourceToggleGroup;

		// Token: 0x0400545F RID: 21599
		[SerializeField]
		private ItemListScroll medicineListScroll;

		// Token: 0x04005460 RID: 21600
		[SerializeField]
		private GameObject medicinePanel;

		// Token: 0x04005461 RID: 21601
		[SerializeField]
		private GameObject leftLineGold;

		// Token: 0x04005462 RID: 21602
		[SerializeField]
		private GameObject rightLineGold;

		// Token: 0x04005463 RID: 21603
		[SerializeField]
		private CToggleGroup targetToggleGroup;

		// Token: 0x04005464 RID: 21604
		[SerializeField]
		private ItemListScroll targetListScroll;

		// Token: 0x04005465 RID: 21605
		[SerializeField]
		private GameObject targetPanel;

		// Token: 0x04005466 RID: 21606
		[SerializeField]
		private CButton buttonCancelSelectTarget;

		// Token: 0x04005467 RID: 21607
		[SerializeField]
		private PropertyItem propertyToolDurability;

		// Token: 0x04005468 RID: 21608
		[SerializeField]
		private PropertyItem propertyToolAttainment;

		// Token: 0x04005469 RID: 21609
		[SerializeField]
		private PropertyItem propertyNeedAttainment;

		// Token: 0x0400546A RID: 21610
		[SerializeField]
		private PoisonInfos poisonInfos;

		// Token: 0x0400546B RID: 21611
		[SerializeField]
		private CButton confirmBtn;

		// Token: 0x0400546C RID: 21612
		[SerializeField]
		private CButton identityBtn;

		// Token: 0x0400546D RID: 21613
		[SerializeField]
		private CToggle extractToggle;

		// Token: 0x0400546E RID: 21614
		[SerializeField]
		private GameObject extractPanel;

		// Token: 0x0400546F RID: 21615
		[SerializeField]
		private ItemBack extractPoisonItemTemplate;

		// Token: 0x04005470 RID: 21616
		[SerializeField]
		private RectTransform extractContent;

		// Token: 0x04005471 RID: 21617
		[SerializeField]
		private TextMeshProUGUI identityTips;

		// Token: 0x04005472 RID: 21618
		[SerializeField]
		private TextMeshProUGUI mixedPoisonName;

		// Token: 0x04005473 RID: 21619
		[SerializeField]
		private GameObject selectTargetTips;

		// Token: 0x04005474 RID: 21620
		[SerializeField]
		private GameObject identifiedPoisonTip;

		// Token: 0x04005475 RID: 21621
		[SerializeField]
		private TooltipInvoker mixedPoisonTip;

		// Token: 0x04005476 RID: 21622
		[SerializeField]
		private TooltipInvoker needAttainmentTip;

		// Token: 0x04005477 RID: 21623
		[SerializeField]
		private TooltipInvoker identityBtnTip;

		// Token: 0x04005478 RID: 21624
		[SerializeField]
		private TooltipInvoker confirmTip;

		// Token: 0x04005479 RID: 21625
		private readonly List<ItemDisplayData> _allItemList = new List<ItemDisplayData>();

		// Token: 0x0400547A RID: 21626
		private readonly List<ItemDisplayData> _allToolList = new List<ItemDisplayData>();

		// Token: 0x0400547B RID: 21627
		private readonly List<ItemDisplayData> _allMedicineList = new List<ItemDisplayData>();

		// Token: 0x0400547C RID: 21628
		private readonly List<ItemDisplayData> _curSourceMedicineList = new List<ItemDisplayData>();

		// Token: 0x0400547D RID: 21629
		private readonly List<ItemDisplayData> _curSourceTargetList = new List<ItemDisplayData>();

		// Token: 0x0400547E RID: 21630
		private bool _isShowEffectHandler;

		// Token: 0x0400547F RID: 21631
		private int _toolMaxFinalAttainment;

		// Token: 0x04005480 RID: 21632
		private bool _curIdentifySuccess;

		// Token: 0x04005481 RID: 21633
		private bool _curIdentifiedResultHasPoison;

		// Token: 0x04005482 RID: 21634
		private Action _curIdentifiedResultAction;

		// Token: 0x04005483 RID: 21635
		private ItemDisplayData targetCache;

		// Token: 0x04005484 RID: 21636
		private StringBuilder sb = new StringBuilder();
	}
}
