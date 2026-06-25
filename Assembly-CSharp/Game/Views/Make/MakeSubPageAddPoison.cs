using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using Game.Components.ListStyleGeneralScroll;
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
	// Token: 0x02000953 RID: 2387
	public class MakeSubPageAddPoison : MakeSubPage
	{
		// Token: 0x17000CE1 RID: 3297
		// (get) Token: 0x060070BA RID: 28858 RVA: 0x00342AFF File Offset: 0x00340CFF
		private int TaiwuCharId
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x17000CE2 RID: 3298
		// (get) Token: 0x060070BB RID: 28859 RVA: 0x00342B0B File Offset: 0x00340D0B
		private ViewMake.ItemSourceTogKey CurMedicineTogKey
		{
			get
			{
				return (ViewMake.ItemSourceTogKey)this.medicineSourceToggleGroup.GetActiveIndex();
			}
		}

		// Token: 0x17000CE3 RID: 3299
		// (get) Token: 0x060070BC RID: 28860 RVA: 0x00342B18 File Offset: 0x00340D18
		private ItemSourceType CurMedicineItemSource
		{
			get
			{
				return ViewMake.GetItemSourceType(this.CurMedicineTogKey);
			}
		}

		// Token: 0x17000CE4 RID: 3300
		// (get) Token: 0x060070BD RID: 28861 RVA: 0x00342B25 File Offset: 0x00340D25
		private ViewMake.ItemSourceTogKey CurTargetTogKey
		{
			get
			{
				return (ViewMake.ItemSourceTogKey)this.targetToggleGroup.GetActiveIndex();
			}
		}

		// Token: 0x17000CE5 RID: 3301
		// (get) Token: 0x060070BE RID: 28862 RVA: 0x00342B32 File Offset: 0x00340D32
		private ItemSourceType CurTargetItemSource
		{
			get
			{
				return ViewMake.GetItemSourceType(this.CurTargetTogKey);
			}
		}

		// Token: 0x060070BF RID: 28863 RVA: 0x00342B40 File Offset: 0x00340D40
		private void RefreshAllItemList()
		{
			this._allToolList.Clear();
			this._allMedicineList.Clear();
			this._allItemList.Clear();
			bool canTransferItemToWarehouse = this.DisplayData.CanTransferItemToWarehouse;
			if (canTransferItemToWarehouse)
			{
				this.<RefreshAllItemList>g__Add|62_0(this.DisplayData.InventoryItemList);
				this.<RefreshAllItemList>g__Add|62_0(this.DisplayData.EquippedItemList);
			}
			this.<RefreshAllItemList>g__Add|62_0(this.DisplayData.WarehouseItemList);
			this.<RefreshAllItemList>g__Add|62_0(this.DisplayData.TreasuryItemList);
		}

		// Token: 0x060070C0 RID: 28864 RVA: 0x00342BD0 File Offset: 0x00340DD0
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

		// Token: 0x060070C1 RID: 28865 RVA: 0x00342C80 File Offset: 0x00340E80
		private bool CheckItemSourceMeet(ItemDisplayData itemDisplayData)
		{
			return itemDisplayData.ItemSourceTypeEnum == this.CurTargetItemSource || (this.CurTargetItemSource == ItemSourceType.Inventory && itemDisplayData.ItemSourceTypeEnum == ItemSourceType.Equipment);
		}

		// Token: 0x060070C2 RID: 28866 RVA: 0x00342CB8 File Offset: 0x00340EB8
		private void RefreshMedicineList()
		{
			this._curSourceMedicineList.Clear();
			bool shouldShowEmpty = !this.targetSlot.IsValid || (this.targetSlot.ItemData.HasAnyPoison && !this.targetSlot.ItemData.PoisonIsIdentified);
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
					bool flag3 = !slot.IsValid || !slot.ItemData.RealKey.IsValid();
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

		// Token: 0x060070C3 RID: 28867 RVA: 0x00342EBC File Offset: 0x003410BC
		private void RefreshAllData()
		{
			this.RefreshAllItemList();
			this.RefreshTargetList();
			this.RefreshMedicineList();
		}

		// Token: 0x060070C4 RID: 28868 RVA: 0x00342ED4 File Offset: 0x003410D4
		public override void Init(ViewMake parentView)
		{
			base.Init(parentView);
			this._isShowEffectHandler = false;
			this.condenseToggle.isOn = false;
			this.imgMedicineSlotBack.gameObject.SetActive(false);
			this.ClearTargetList();
			this.ResetPanelState();
			this.RefreshPoisonInfo();
			this.SetTargetCache(null);
			this.ParentView.ShowToolPanel(false);
			this.medicinePanel.gameObject.SetActive(false);
			this.goMaterialHolder.SetActive(false);
			this.toolSlot.gameObject.SetActive(false);
			this.targetSlot.Init(EMakeTargetSlotInteract.Always, EMakeTargetSlotType.AddPoisonTarget, new Action<int, ItemDisplayData>(this.OnTargetSlotCancel), new Action(this.OnTargetSlotSelect), null, null, -1, null, false, null);
			this.targetSlot.SetEffectHandlerState(true);
			this.toolSlot.Init(EMakeTargetSlotInteract.Custom, EMakeTargetSlotType.Tool, new Action<int, ItemDisplayData>(this.OnCancelTool), delegate
			{
				parentView.SetRightMask(false);
			}, new Func<bool>(this.GetToolInteractable), null, -1, null, false, null);
			int index = 0;
			MakeTargetSlot[] array = this.medicineSlotList;
			for (int i = 0; i < array.Length; i++)
			{
				MakeTargetSlot slot = array[i];
				slot.Init(EMakeTargetSlotInteract.Custom, EMakeTargetSlotType.MakeMaterial, new Action<int, ItemDisplayData>(this.OnMedicineSlotCancel), null, () => slot.IsValid && slot.ItemData.RealKey.IsValid(), null, index, null, true, new Action<bool, int>(this.SetHoverShow));
				index++;
			}
			foreach (GameObject hover in this.hoverObjects)
			{
				hover.SetActive(false);
			}
			this.targetListScroll.SetCustomBuildGroup(new Action(this.TargetCustomBuildGroup), true);
			this.targetListScroll.Init("MakeSubPageAddPoisonTarget", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnTargetListItemRender), new Action<ITradeableContent, RowItemLine>(this.OnTargetListItemClick), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.PoisonInfo, null, null, null);
			this.targetListScroll.SortAndFilterController.SetToggleVisible(EFilterLine.MainFilter.ToInt(), new List<int>
			{
				EMainFilterKeys.Food.ToInt(),
				EMainFilterKeys.Medicine.ToInt(),
				EMainFilterKeys.Equip.ToInt(),
				EMainFilterKeys.Book.ToInt()
			}, false);
			this.medicineListScroll.SetCustomBuildGroup(new Action(this.MedicineCustomBuildGroup), true);
			this.medicineListScroll.Init("MakeSubPageAddPoisonMedicine", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnMedicineListItemRender), new Action<ITradeableContent, RowItemLine>(this.OnMedicineListItemClick), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability, null, this.GenerateColumnDefinitions(), new Action<RowItem>(this.GenerateRowTemplateContainers));
			this.medicineListScroll.SortAndFilterController.SetToggleVisible(EFilterLine.MainFilter.ToInt(), EMainFilterKeys.Medicine.ToInt());
			this.medicineListScroll.SortAndFilterController.SetToggleIsOnWithoutNotify(EFilterLine.MainFilter.ToInt(), EMainFilterKeys.Medicine.ToInt());
		}

		// Token: 0x060070C5 RID: 28869 RVA: 0x003431E2 File Offset: 0x003413E2
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitions()
		{
			ColumnDefinition<ITradeableContent, ITradeableContent> columnDefinition = new ColumnDefinition<ITradeableContent, ITradeableContent>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 200f,
				FlexibleWidth = 1f,
				PreferredWidth = 350f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Item.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => data);
			columnDefinition.SortId = 0;
			yield return columnDefinition;
			ColumnDefinition<ITradeableContent, ItemDisplayData> columnDefinition2 = new ColumnDefinition<ITradeableContent, ItemDisplayData>();
			columnDefinition2.LayoutOption = new LayoutOption
			{
				MinWidth = 150f,
				FlexibleWidth = 1f,
				PreferredWidth = 200f,
				Priority = 1
			};
			columnDefinition2.TableHeadLabel = (() => LanguageKey.LK_ItemTips_Self_Poison.Tr());
			columnDefinition2.CellDataGenerator = ((ITradeableContent data) => (ItemDisplayData)data);
			columnDefinition2.SortId = 217;
			yield return columnDefinition2;
			ColumnDefinition<ITradeableContent, string> columnDefinition3 = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition3.LayoutOption = new LayoutOption
			{
				MinWidth = 50f,
				FlexibleWidth = 1f,
				PreferredWidth = 150f,
				Priority = 1
			};
			columnDefinition3.TableHeadLabel = (() => LanguageKey.LK_Count.Tr());
			columnDefinition3.CellDataGenerator = ((ITradeableContent data) => CommonUtils.GetDisplayStringForNum(data.Amount, 100000));
			columnDefinition3.SortId = 17;
			yield return columnDefinition3;
			yield break;
		}

		// Token: 0x060070C6 RID: 28870 RVA: 0x003431F2 File Offset: 0x003413F2
		private void GenerateRowTemplateContainers(RowItem rowItem)
		{
			MakeSubPageAddPoison.<GenerateRowTemplateContainers>g__CreateCellContainers|69_0(rowItem.ContainerRoot, this.itemIconAndNameCellContainer);
			MakeSubPageAddPoison.<GenerateRowTemplateContainers>g__CreateCellContainers|69_0(rowItem.ContainerRoot, this.refineEffectCellContainer);
			MakeSubPageAddPoison.<GenerateRowTemplateContainers>g__CreateCellContainers|69_0(rowItem.ContainerRoot, this.singleTextCellContainer);
		}

		// Token: 0x060070C7 RID: 28871 RVA: 0x00343230 File Offset: 0x00341430
		public override void Refresh(BuildingMakeDisplayData displayData)
		{
			base.Refresh(displayData);
			this.ParentView.RefreshSourceToggleInteractable(this.medicineSourceToggleGroup);
			this.ParentView.RefreshSourceToggleInteractable(this.targetToggleGroup);
			this.ClearToolList();
			this.RefreshAllData();
			this.ReloadSlot();
			this.RefreshInfo();
			this.CalcToolMaxFinalAttainment();
			UIElement.FullScreenMask.Hide(false);
		}

		// Token: 0x060070C8 RID: 28872 RVA: 0x0034329C File Offset: 0x0034149C
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
						this.SetTargetCache(null);
						ItemDisplayData itemData = this.targetSlot.ItemData;
						this._originPoisonEffects = (((itemData != null) ? itemData.PoisonEffects : null) ?? new FullPoisonEffects());
						this._tempPoisonEffects.Assign(this._originPoisonEffects);
						this.RefreshInfo();
						this.RefreshMedicineList();
						bool flag5 = this.toolSlot == null || this.toolSlot.ItemData == null;
						if (flag5)
						{
							this.toolSlot.Cancel();
						}
						else
						{
							ItemDisplayData toolData = (this.toolSlot.ItemData == this.ParentView.EmptyTool) ? this.ParentView.EmptyTool : this.ParentView.AllToolList.Find((ItemDisplayData d) => d.RealKey == this.toolSlot.ItemData.RealKey && d.ItemSourceTypeEnum == this.toolSlot.ItemData.ItemSourceTypeEnum);
							bool flag6 = toolData == null;
							if (flag6)
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

		// Token: 0x060070C9 RID: 28873 RVA: 0x00343458 File Offset: 0x00341658
		private void ResetPanelState()
		{
			this.targetSlot.Select(null, false);
			this.toolSlot.Select(null, false);
			foreach (MakeTargetSlot slot in this.medicineSlotList)
			{
				slot.Select(null, false);
			}
		}

		// Token: 0x060070CA RID: 28874 RVA: 0x003434A8 File Offset: 0x003416A8
		private void Awake()
		{
			this.medicineSourceToggleGroup.OnActiveIndexChange += this.MedicineSourceToggleGroupOnOnActiveIndexChange;
			this.medicineSourceToggleGroup.Init(-1);
			this.targetToggleGroup.OnActiveIndexChange += this.TargetToggleGroupOnOnActiveIndexChange;
			this.targetToggleGroup.Init(-1);
			this.identityBtn.gameObject.SetActive(true);
			this.identityBtn.onClick.ResetListener(new Action(this.ConfirmIdentifyBtn));
			this.confirmBtn.onClick.ResetListener(new Action(this.ConfirmAddPoison));
			this.backButtn.onClick.ResetListener(new Action(this.OnClickBackButtn));
			for (int i = 0; i < this.medicineSlotList.Length; i++)
			{
				bool flag = i % PoisonSlot.MaxMedicineCount != 0;
				if (flag)
				{
					MakeTargetSlot slot = this.medicineSlotList[i];
					slot.gameObject.SetActive(false);
					slot.Select(null, false);
				}
			}
			this.condenseToggle.onValueChanged.ResetListener(delegate(bool isOn)
			{
				for (int j = 0; j < this.medicineSlotList.Length; j++)
				{
					MakeTargetSlot slot2 = this.medicineSlotList[j];
					bool flag2 = j % PoisonSlot.MaxMedicineCount != 0;
					if (flag2)
					{
						slot2.gameObject.SetActive(isOn);
						bool flag3 = !isOn;
						if (flag3)
						{
							slot2.Select(null, false);
						}
					}
				}
				this.RefreshMedicineList();
				this.RefreshInfo();
				this.imgMedicineSlotBack.gameObject.SetActive(isOn);
				this.goLineMaterialToTargetNormal.gameObject.SetActive(!isOn);
			});
			this.condenseToggle.isOn = false;
			this.imgMedicineSlotBack.gameObject.SetActive(false);
			this.goLineMaterialToTargetNormal.gameObject.SetActive(true);
		}

		// Token: 0x060070CB RID: 28875 RVA: 0x003435FF File Offset: 0x003417FF
		private void OnDestroy()
		{
			this.medicineSourceToggleGroup.OnActiveIndexChange -= this.MedicineSourceToggleGroupOnOnActiveIndexChange;
			this.targetToggleGroup.OnActiveIndexChange -= this.TargetToggleGroupOnOnActiveIndexChange;
		}

		// Token: 0x060070CC RID: 28876 RVA: 0x00343634 File Offset: 0x00341834
		private void OnTargetListItemClick(ITradeableContent content, RowItemLine rowItemLine)
		{
			ItemDisplayData itemData = (ItemDisplayData)content;
			this.targetSlot.SetEffectHandlerState(false);
			this.targetPanel.gameObject.SetActive(false);
			this.medicinePanel.gameObject.SetActive(true);
			this.ParentView.ShowToolPanel(true);
			this.ParentView.SetRightMask(true);
			this.ParentView.ExitFocusMode();
			this.targetSlot.Select(itemData, false);
			this.toolSlot.Refresh(false);
			this.toolSlot.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.Disable);
			this.goMaterialHolder.SetActive(true);
			this.toolSlot.gameObject.SetActive(true);
			this.goTxtSelectPoisonTips.SetActive(true);
			this.goLineTargetToToolGrey.SetActive(false);
			this.goLineTargetToToolGold.SetActive(false);
			ItemDisplayData itemData2 = this.targetSlot.ItemData;
			this._originPoisonEffects = (((itemData2 != null) ? itemData2.PoisonEffects : null) ?? new FullPoisonEffects());
			this._tempPoisonEffects.Assign(this._originPoisonEffects);
			this.RefreshInfo();
			this.RefreshMedicineList();
		}

		// Token: 0x060070CD RID: 28877 RVA: 0x00343754 File Offset: 0x00341954
		private void RefreshInfo()
		{
			this.RefreshMedicineSlot();
			this.ShowIdentifiedPoisonTip(false, null);
			this.RefreshPoisonInfo();
			this.RefreshIdentifyInfo();
			this.RefreshButton();
			this.RefreshSelectTargetTips();
			this.RefreshMixPoisonName();
			this.CheckCondition();
		}

		// Token: 0x060070CE RID: 28878 RVA: 0x00343794 File Offset: 0x00341994
		private void OnTargetListItemRender(ITradeableContent content, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(content);
			rowItemLine.Set(rowItemMain, true);
			bool canAdd = this.TargetCanAddPoison(content);
			rowItemLine.SetInteractable(canAdd, true);
			rowItemLine.SetDisabled(!canAdd);
			rowItemLine.SetSelected(content == this.targetSlot.ItemData);
		}

		// Token: 0x060070CF RID: 28879 RVA: 0x003437EC File Offset: 0x003419EC
		private void TargetCustomBuildGroup()
		{
			List<ITradeableContent> canAddPoisonTargetList = new List<ITradeableContent>();
			List<ITradeableContent> canNotAddPoisonTargetList = new List<ITradeableContent>();
			foreach (ITradeableContent target in this.targetListScroll.FilteredData)
			{
				bool flag = this.TargetCanAddPoison(target);
				if (flag)
				{
					canAddPoisonTargetList.Add(target);
				}
				else
				{
					canNotAddPoisonTargetList.Add(target);
				}
			}
			string title = LocalStringManager.Get(LanguageKey.LK_Add_Poison_Medicine_Group_Can_Remove);
			this.targetListScroll.AddGroup(0, title, canAddPoisonTargetList, null, true);
			string title2 = LocalStringManager.Get(LanguageKey.LK_Add_Poison_Medicine_Group_Can_Not_Remove);
			this.targetListScroll.AddGroup(1, title2, canNotAddPoisonTargetList, null, true);
		}

		// Token: 0x060070D0 RID: 28880 RVA: 0x003438A8 File Offset: 0x00341AA8
		private bool TargetCanAddPoison(ITradeableContent target)
		{
			bool flag = !target.PoisonIsIdentified;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = !target.HasAnyPoison || target.PoisonEffects.PoisonSlotList.Count < PoisonSlot.MaxMedicineCount;
				if (flag2)
				{
					result = true;
				}
				else
				{
					foreach (PoisonSlot slot in target.PoisonEffects.PoisonSlotList)
					{
						bool flag3 = !slot.MedicineCountIsMax;
						if (flag3)
						{
							return true;
						}
						MedicineItem config = Medicine.Instance[slot.MedicineTemplateId];
						bool flag4 = config.Grade < 8;
						if (flag4)
						{
							return true;
						}
					}
					result = false;
				}
			}
			return result;
		}

		// Token: 0x060070D1 RID: 28881 RVA: 0x00343980 File Offset: 0x00341B80
		private void TargetToggleGroupOnOnActiveIndexChange(int newIndex, int oldIndex)
		{
			this.RefreshTargetList();
		}

		// Token: 0x060070D2 RID: 28882 RVA: 0x0034398A File Offset: 0x00341B8A
		private void ClearTargetList()
		{
			this._curSourceTargetList.Clear();
			this.targetListScroll.SetEmptyContent(string.Empty);
			this.targetListScroll.SetItemList(this._curSourceTargetList);
		}

		// Token: 0x060070D3 RID: 28883 RVA: 0x003439BC File Offset: 0x00341BBC
		private void OnTargetSlotSelect()
		{
			this.targetPanel.gameObject.SetActive(true);
			this.ParentView.EnterFocusMode(this.targetPanel.transform, null);
			this.targetListScroll.SetItemList(this._curSourceTargetList);
			this.medicinePanel.gameObject.SetActive(false);
		}

		// Token: 0x060070D4 RID: 28884 RVA: 0x00343A18 File Offset: 0x00341C18
		private void OnTargetSlotCancel(int index, ItemDisplayData itemDisplayData)
		{
			this.targetSlot.SetEffectHandlerState(true);
			this.goMaterialHolder.SetActive(false);
			this.toolSlot.gameObject.SetActive(false);
			this.goLineTargetToToolGrey.SetActive(false);
			this.goLineTargetToToolGold.SetActive(false);
			this.toolSlot.Cancel();
			this.ClearToolList();
			this.medicinePanel.gameObject.SetActive(false);
			this.ParentView.ShowToolPanel(false);
			this.RefreshInfo();
		}

		// Token: 0x060070D5 RID: 28885 RVA: 0x00343AA5 File Offset: 0x00341CA5
		private void RefreshSelectTargetTips()
		{
			this.selectTargetTips.gameObject.SetActive(!this.targetSlot.IsValid);
		}

		// Token: 0x060070D6 RID: 28886 RVA: 0x00343AC8 File Offset: 0x00341CC8
		private void RefreshButton()
		{
			this.condenseToggle.gameObject.SetActive(this.targetSlot.IsValid);
			this.identityBtn.gameObject.SetActive(this.targetSlot.IsValid);
			this.confirmBtn.gameObject.SetActive(this.targetSlot.IsValid);
		}

		// Token: 0x060070D7 RID: 28887 RVA: 0x00343B2C File Offset: 0x00341D2C
		private void OnMedicineListItemClick(ITradeableContent content, RowItemLine rowItemLine)
		{
			this._tempPoisonEffects.AddPoison(content.RealKey.TemplateId);
			this.ParentView.RefreshToolList(this.GetPoisonRequiredAttainment(), new List<sbyte>
			{
				this.ParentView.CurLifeSkillType
			}, new List<List<sbyte>>
			{
				new List<sbyte>
				{
					this.GetMaterialMaxGrade()
				}
			}, this.toolSlot.ItemData, new Action<ItemDisplayData>(this.SelectTool), true, 1);
			this.RefreshInfo();
			this.RefreshMedicineList();
			this.goTxtSelectPoisonTips.SetActive(false);
			this.ParentView.SetRightMask(false);
			bool flag = !this._isShowEffectHandler;
			if (flag)
			{
				this.toolSlot.SetEffectHandlerState(true);
				this.leftLineGold.gameObject.SetActive(true);
				this._isShowEffectHandler = true;
			}
			bool flag2 = !this.toolSlot.IsToggleOn;
			if (flag2)
			{
				this.toolSlot.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.Enable);
				this.goLineTargetToToolGrey.SetActive(true);
				this.goLineTargetToToolGold.SetActive(false);
			}
			else
			{
				this.goLineTargetToToolGrey.SetActive(false);
				this.goLineTargetToToolGold.SetActive(true);
			}
		}

		// Token: 0x060070D8 RID: 28888 RVA: 0x00343C68 File Offset: 0x00341E68
		private void OnMedicineListItemRender(ITradeableContent content, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(content);
			rowItemLine.Set(rowItemMain, true);
			bool canSelect = this.meetMedicineList.Contains(content);
			rowItemLine.SetInteractable(canSelect, true);
			rowItemLine.SetDisabled(!canSelect);
			rowItemLine.SetSelected(false);
		}

		// Token: 0x060070D9 RID: 28889 RVA: 0x00343CB8 File Offset: 0x00341EB8
		private void MedicineCustomBuildGroup()
		{
			this.attainmentNotMeetMedicineList.Clear();
			this.gradeNotMeetMedicineList.Clear();
			this.itemTypeNotMeetMedicineList.Clear();
			this.poisonLevelNotMeetMedicineList.Clear();
			this.sameMedicineList.Clear();
			this.meetMedicineList.Clear();
			bool flag = this._curSourceMedicineList.Count <= 0;
			if (!flag)
			{
				foreach (ITradeableContent medicine in this.medicineListScroll.FilteredData)
				{
					MedicineItem itemConfig = Medicine.Instance[medicine.Key.TemplateId];
					sbyte poisonType = itemConfig.PoisonType;
					short poisonLevel = itemConfig.EffectThresholdValue;
					sbyte poisonGrade = itemConfig.Grade;
					bool flag2 = !this._tempPoisonEffects.IsValid;
					if (flag2)
					{
						this.meetMedicineList.Add(medicine);
					}
					else
					{
						bool flag3 = this._tempPoisonEffects.CurrentValidSlotCount == FullPoisonEffects.MaxSlotCount && this._tempPoisonEffects.PoisonSlotList.All((PoisonSlot s) => s.IsValid && s.MedicineConfig.PoisonType != poisonType);
						if (flag3)
						{
							this.itemTypeNotMeetMedicineList.Add(medicine);
						}
						else
						{
							bool flag4 = !this.condenseToggle.isOn;
							if (flag4)
							{
								bool flag5 = this._tempPoisonEffects.PoisonSlotList.Exists((PoisonSlot s) => s.IsValid && s.MedicineConfig.PoisonType == poisonType);
								if (flag5)
								{
									this.sameMedicineList.Add(medicine);
									continue;
								}
								bool flag6 = this._tempPoisonEffects.PoisonSlotList.Any((PoisonSlot s) => s.IsValid && s.MedicineConfig.PoisonType == poisonType && poisonGrade < s.MedicineConfig.Grade);
								if (flag6)
								{
									this.gradeNotMeetMedicineList.Add(medicine);
									continue;
								}
							}
							else
							{
								bool flag7 = this._tempPoisonEffects.PoisonSlotList.Exists((PoisonSlot s) => s.IsValid && s.MedicineConfig.PoisonType == poisonType && poisonLevel != s.MedicineConfig.EffectThresholdValue);
								if (flag7)
								{
									this.poisonLevelNotMeetMedicineList.Add(medicine);
									continue;
								}
								bool flag8 = this._tempPoisonEffects.PoisonSlotList.Exists((PoisonSlot s) => s.IsValid && s.MedicineConfig.PoisonType == poisonType && s.MedicineCountIsMax);
								if (flag8)
								{
									this.sameMedicineList.Add(medicine);
									continue;
								}
							}
							this.meetMedicineList.Add(medicine);
						}
					}
				}
				int groupIndex = 0;
				string title = "可淬毒";
				this.medicineListScroll.AddGroup(groupIndex++, title, this.meetMedicineList, null, true);
				string title2 = LanguageKey.LK_Add_Poison_Same.Tr().RemoveColor();
				this.medicineListScroll.AddGroup(groupIndex++, title2, this.sameMedicineList, null, true);
				string title3 = LanguageKey.LK_Add_Poison_Value_Not_Enough.Tr().RemoveColor();
				this.medicineListScroll.AddGroup(groupIndex++, title3, this.poisonLevelNotMeetMedicineList, null, true);
				string title4 = LanguageKey.LK_Remove_Poison_Medicine_Group_Grade_Not_Match.Tr().RemoveColor();
				this.medicineListScroll.AddGroup(groupIndex++, title4, this.gradeNotMeetMedicineList, null, true);
				string title5 = LanguageKey.LK_Remove_Poison_Medicine_Group_Attainment_Not_Match.Tr().RemoveColor();
				this.medicineListScroll.AddGroup(groupIndex++, title5, this.attainmentNotMeetMedicineList, null, true);
				string title6 = LanguageKey.LK_Remove_Poison_Medicine_Group_Type_Not_Match.Tr().RemoveColor();
				this.medicineListScroll.AddGroup(groupIndex++, title6, this.itemTypeNotMeetMedicineList, null, true);
			}
		}

		// Token: 0x060070DA RID: 28890 RVA: 0x00344038 File Offset: 0x00342238
		private void RefreshMedicineSlot()
		{
			int index = 0;
			while (index < this.medicineSlotList.Length)
			{
				MakeTargetSlot slot = this.medicineSlotList[index];
				int poisonSlotIndex = index / FullPoisonEffects.MaxSlotCount;
				List<PoisonSlot> poisonSlotList = this._tempPoisonEffects.PoisonSlotList;
				bool flag = poisonSlotList != null && poisonSlotList.CheckIndex(poisonSlotIndex);
				if (flag)
				{
					int medicineIndex = index % FullPoisonEffects.MaxSlotCount;
					PoisonSlot poisonSlot = this._tempPoisonEffects.PoisonSlotList[poisonSlotIndex];
					List<short> allMedicineTemplateIdList = poisonSlot.GetAllMedicineTemplateId(true);
					short medicineTemplateId = allMedicineTemplateIdList.GetOrDefault(medicineIndex, -1);
					bool flag2 = medicineTemplateId < 0;
					if (flag2)
					{
						slot.Select(null, false);
					}
					else
					{
						FullPoisonEffects originPoisonEffects = this._originPoisonEffects;
						PoisonSlot poisonSlot2;
						if (originPoisonEffects == null)
						{
							poisonSlot2 = null;
						}
						else
						{
							List<PoisonSlot> poisonSlotList2 = originPoisonEffects.PoisonSlotList;
							poisonSlot2 = ((poisonSlotList2 != null) ? poisonSlotList2.GetOrDefault(poisonSlotIndex) : null);
						}
						PoisonSlot originPoisonSlot = poisonSlot2;
						List<short> originAllMedicineTemplateIdList = (originPoisonSlot != null) ? originPoisonSlot.GetAllMedicineTemplateId(true) : null;
						short originMedicineTemplateId = (originAllMedicineTemplateIdList != null) ? originAllMedicineTemplateIdList.GetOrDefault(medicineIndex, -1) : -1;
						ItemDisplayData itemData = (medicineTemplateId == originMedicineTemplateId) ? new ItemDisplayData(8, medicineTemplateId) : this._curSourceMedicineList.Find((ItemDisplayData m) => m.RealKey.TemplateId == medicineTemplateId);
						slot.Select(itemData, false);
					}
				}
				else
				{
					slot.Select(null, false);
				}
				IL_12F:
				index++;
				continue;
				goto IL_12F;
			}
		}

		// Token: 0x060070DB RID: 28891 RVA: 0x0034418C File Offset: 0x0034238C
		private void OnMedicineSlotCancel(int index, ItemDisplayData itemDisplayData)
		{
			this._tempPoisonEffects.RemovePoison(itemDisplayData.RealKey.TemplateId);
			bool isAllEmpty = this.MedicineSlotAllEmpty();
			bool flag = isAllEmpty;
			if (flag)
			{
				this.toolSlot.Cancel();
				this.ClearToolList();
				this.goTxtSelectPoisonTips.SetActive(true);
				this.goLineTargetToToolGrey.SetActive(false);
				this.goLineTargetToToolGold.SetActive(false);
				bool isShowEffectHandler = this._isShowEffectHandler;
				if (isShowEffectHandler)
				{
					this._isShowEffectHandler = false;
					this.toolSlot.SetEffectHandlerState(false);
					this.leftLineGold.gameObject.SetActive(false);
					this.rightLineGold.gameObject.SetActive(false);
				}
			}
			this.ParentView.RefreshToolList(this.GetPoisonRequiredAttainment(), new List<sbyte>
			{
				this.ParentView.CurLifeSkillType
			}, new List<List<sbyte>>
			{
				new List<sbyte>
				{
					this.GetMaterialMaxGrade()
				}
			}, this.toolSlot.ItemData, new Action<ItemDisplayData>(this.SelectTool), true, 1);
			this.RefreshInfo();
			this.RefreshMedicineList();
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
		}

		// Token: 0x060070DC RID: 28892 RVA: 0x003442E7 File Offset: 0x003424E7
		private void ClearToolList()
		{
			this.ParentView.ClearToolList(LocalStringManager.Get(LanguageKey.LK_Make_Poison_Not_Selected).ColorReplace());
		}

		// Token: 0x060070DD RID: 28893 RVA: 0x00344305 File Offset: 0x00342505
		private void MedicineSourceToggleGroupOnOnActiveIndexChange(int newIndex, int oldIndex)
		{
			this.RefreshMedicineList();
		}

		// Token: 0x060070DE RID: 28894 RVA: 0x00344310 File Offset: 0x00342510
		private ItemKey[] GetMedicineItemArray()
		{
			List<ItemKey> itemKeys = new List<ItemKey>();
			for (int i = 0; i < this.medicineSlotList.Length; i++)
			{
				bool flag = i % PoisonSlot.MaxMedicineCount == 0 || this.condenseToggle.isOn;
				if (flag)
				{
					MakeTargetSlot slot = this.medicineSlotList[i];
					bool isValid = slot.IsValid;
					if (isValid)
					{
						itemKeys.Add(slot.ItemData.RealKey);
					}
				}
			}
			return itemKeys.ToArray();
		}

		// Token: 0x060070DF RID: 28895 RVA: 0x00344390 File Offset: 0x00342590
		private ItemDisplayData[] GetMedicineItemDisplayDataArray()
		{
			List<ItemDisplayData> itemKeys = new List<ItemDisplayData>();
			for (int i = 0; i < this.medicineSlotList.Length; i++)
			{
				bool flag = i % PoisonSlot.MaxMedicineCount == 0;
				if (flag)
				{
					MakeTargetSlot slot = this.medicineSlotList[i];
					bool isValid = slot.IsValid;
					if (isValid)
					{
						itemKeys.Add(slot.ItemData);
					}
				}
			}
			return itemKeys.ToArray();
		}

		// Token: 0x060070E0 RID: 28896 RVA: 0x00344400 File Offset: 0x00342600
		private List<ItemDisplayData> GetCondensePoisonItemList()
		{
			List<ItemDisplayData> itemKeys = new List<ItemDisplayData>();
			for (int i = 0; i < this.medicineSlotList.Length; i++)
			{
				bool flag = i % PoisonSlot.MaxMedicineCount != 0;
				if (flag)
				{
					MakeTargetSlot slot = this.medicineSlotList[i];
					bool flag2 = slot.IsValid && slot.ItemData.RealKey.IsValid();
					if (flag2)
					{
						itemKeys.Add(slot.ItemData);
					}
				}
			}
			return itemKeys;
		}

		// Token: 0x060070E1 RID: 28897 RVA: 0x00344484 File Offset: 0x00342684
		private int GetMinNeedAttainment(sbyte poisonType)
		{
			bool flag = !this.targetSlot.IsValid || !this._tempPoisonEffects.IsValid;
			int result;
			if (flag)
			{
				result = int.MinValue;
			}
			else
			{
				int minValue = int.MaxValue;
				foreach (PoisonSlot slot in this._tempPoisonEffects.PoisonSlotList)
				{
					bool flag2 = slot.GetPoisonType() != poisonType;
					if (!flag2)
					{
						short requiredAttainment = this.ParentView.GetPoisonRequiredAttainment(slot.MedicineTemplateId, this.condenseToggle.isOn);
						bool flag3 = (int)requiredAttainment < minValue;
						if (flag3)
						{
							minValue = (int)requiredAttainment;
						}
					}
				}
				result = minValue;
			}
			return result;
		}

		// Token: 0x060070E2 RID: 28898 RVA: 0x00344558 File Offset: 0x00342758
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

		// Token: 0x060070E3 RID: 28899 RVA: 0x003445A0 File Offset: 0x003427A0
		private sbyte GetMaterialMaxGrade()
		{
			sbyte grade = 0;
			for (int i = 0; i < this.medicineSlotList.Length; i++)
			{
				MakeTargetSlot slot = this.medicineSlotList[i];
				bool flag = !slot.IsValid;
				if (!flag)
				{
					grade = Math.Max(slot.ItemData.Grade, grade);
				}
			}
			return grade;
		}

		// Token: 0x060070E4 RID: 28900 RVA: 0x003445FC File Offset: 0x003427FC
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

		// Token: 0x060070E5 RID: 28901 RVA: 0x003446A4 File Offset: 0x003428A4
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
						bool isShowEffectHandler = this._isShowEffectHandler;
						if (isShowEffectHandler)
						{
							this._isShowEffectHandler = false;
							this.toolSlot.SetEffectHandlerState(false);
							this.rightLineGold.gameObject.SetActive(true);
						}
						result = attainment;
					}
				}
			}
			return result;
		}

		// Token: 0x060070E6 RID: 28902 RVA: 0x003447BC File Offset: 0x003429BC
		private bool IsEmptyTool()
		{
			return this.toolSlot.IsValid && this.toolSlot.ItemData.Key == this.DisplayData.EmptyToolKey;
		}

		// Token: 0x060070E7 RID: 28903 RVA: 0x003447FE File Offset: 0x003429FE
		private void SelectTool(ItemDisplayData itemData)
		{
			this.toolSlot.Select(itemData, false);
			this.CheckCondition();
			this.goLineTargetToToolGrey.SetActive(false);
			this.goLineTargetToToolGold.SetActive(true);
		}

		// Token: 0x060070E8 RID: 28904 RVA: 0x00344830 File Offset: 0x00342A30
		private void OnCancelTool(int index, ItemDisplayData itemDisplayData)
		{
			this.ParentView.RerenderToolList(this.targetSlot.ItemData);
			this.CheckCondition();
			this.goLineTargetToToolGrey.SetActive(true);
			this.goLineTargetToToolGold.SetActive(false);
		}

		// Token: 0x060070E9 RID: 28905 RVA: 0x0034486B File Offset: 0x00342A6B
		private bool GetToolInteractable()
		{
			return this.targetSlot.IsValid;
		}

		// Token: 0x060070EA RID: 28906 RVA: 0x00344878 File Offset: 0x00342A78
		private void ShowUnidentifiedPoisonTip(bool show, LanguageKey key = LanguageKey.LK_Remove_Poison_Has_Unidentified)
		{
			this.identityTips.transform.parent.gameObject.SetActive(show);
			bool flag = !show;
			if (!flag)
			{
				this.identityTips.SetText(LocalStringManager.Get(key), true);
			}
		}

		// Token: 0x060070EB RID: 28907 RVA: 0x003448C0 File Offset: 0x00342AC0
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

		// Token: 0x060070EC RID: 28908 RVA: 0x00344970 File Offset: 0x00342B70
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

		// Token: 0x060070ED RID: 28909 RVA: 0x003449D8 File Offset: 0x00342BD8
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

		// Token: 0x060070EE RID: 28910 RVA: 0x00344B6D File Offset: 0x00342D6D
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

		// Token: 0x060070EF RID: 28911 RVA: 0x00344BA0 File Offset: 0x00342DA0
		private int GetNeedleCount()
		{
			List<ItemDisplayData> items = this._allItemList.FindAll(new Predicate<ItemDisplayData>(MakeSubPageAddPoison.<GetNeedleCount>g__Match|114_0));
			int totalCount = 0;
			foreach (ItemDisplayData item in items)
			{
				totalCount += item.Amount;
			}
			return totalCount;
		}

		// Token: 0x060070F0 RID: 28912 RVA: 0x00344C18 File Offset: 0x00342E18
		private void RefreshMixPoisonName()
		{
			bool flag = this.targetSlot.IsValid && this._tempPoisonEffects.IsThreeMixed;
			if (flag)
			{
				this.ShowPreviewPoison(this._tempPoisonEffects);
			}
			else
			{
				this.mixedPoisonTip.gameObject.SetActive(false);
			}
		}

		// Token: 0x060070F1 RID: 28913 RVA: 0x00344C68 File Offset: 0x00342E68
		private void ShowPreviewPoison(FullPoisonEffects poisonEffects)
		{
			this.mixedPoisonTip.gameObject.SetActive(true);
			short medicineTemplateId = poisonEffects.GetMixedMedicineTemplateId();
			string medicineName = Medicine.Instance[medicineTemplateId].Name;
			this.mixedPoisonName.text = medicineName;
			this.mixedPoisonTip.Type = TipType.MixPoison;
			this.mixedPoisonTip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("MedicineId", medicineTemplateId);
		}

		// Token: 0x060070F2 RID: 28914 RVA: 0x00344CD8 File Offset: 0x00342ED8
		private bool CheckSkillAttainment()
		{
			int requiredAttainment = this.GetPoisonRequiredAttainment();
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

		// Token: 0x060070F3 RID: 28915 RVA: 0x00344DB8 File Offset: 0x00342FB8
		private int GetPoisonRequiredAttainment()
		{
			int maxRequiredAttainment = -1;
			bool flag = !this.targetSlot.IsValid || !this._tempPoisonEffects.IsValid;
			int result;
			if (flag)
			{
				result = maxRequiredAttainment;
			}
			else
			{
				for (int i = 0; i < this._tempPoisonEffects.PoisonSlotList.Count; i++)
				{
					PoisonSlot slot = this._tempPoisonEffects.PoisonSlotList[i];
					bool flag2 = slot.MedicineTemplateId < 0;
					if (!flag2)
					{
						short requiredAttainment = this.ParentView.GetPoisonRequiredAttainment(slot.MedicineTemplateId, this.condenseToggle.isOn);
						List<short> condensedMedicineTemplateIdList = slot.CondensedMedicineTemplateIdList;
						bool flag3 = condensedMedicineTemplateIdList != null && condensedMedicineTemplateIdList.Count > 0;
						if (flag3)
						{
							requiredAttainment = slot.CondensedMedicineTemplateIdList.Max((short m) => this.ParentView.GetPoisonRequiredAttainment(m, this.condenseToggle.isOn));
						}
						bool flag4 = (int)requiredAttainment > maxRequiredAttainment;
						if (flag4)
						{
							maxRequiredAttainment = (int)requiredAttainment;
						}
					}
				}
				result = maxRequiredAttainment;
			}
			return result;
		}

		// Token: 0x060070F4 RID: 28916 RVA: 0x00344EAC File Offset: 0x003430AC
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

		// Token: 0x060070F5 RID: 28917 RVA: 0x00345034 File Offset: 0x00343234
		private void SetTargetCache(ItemDisplayData cache)
		{
			this.targetCache = cache;
		}

		// Token: 0x060070F6 RID: 28918 RVA: 0x00345040 File Offset: 0x00343240
		private void CheckCondition()
		{
			bool attainmentMeet = this.CheckSkillAttainment();
			bool toolMeet = this.CheckTool();
			bool condenseMeet = true;
			bool isOn = this.condenseToggle.isOn;
			if (isOn)
			{
				bool flag;
				if (this._tempPoisonEffects.IsValid)
				{
					flag = this._tempPoisonEffects.PoisonSlotList.All((PoisonSlot s) => !s.IsCondensed || s.MedicineCountIsMax);
				}
				else
				{
					flag = false;
				}
				condenseMeet = flag;
			}
			List<PoisonSlot> poisonSlotList = this._tempPoisonEffects.PoisonSlotList;
			bool flag2;
			if (poisonSlotList == null)
			{
				flag2 = false;
			}
			else
			{
				FullPoisonEffects originPoisonEffects = this._originPoisonEffects;
				flag2 = poisonSlotList.ContentIsDifferent((originPoisonEffects != null) ? originPoisonEffects.PoisonSlotList : null);
			}
			bool isChanged = flag2;
			bool interactable = this.targetSlot.IsValid && this.toolSlot.IsValid && attainmentMeet && toolMeet && condenseMeet && isChanged;
			this.RefreshConfirmBtn(interactable);
			StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
			bool flag3 = !this.targetSlot.IsValid;
			if (flag3)
			{
				stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Add_Poison_Item_Not_Selected));
			}
			bool flag4 = this.MedicineSlotAllEmpty();
			if (flag4)
			{
				stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Add_Poison_Material_Not_Selected));
			}
			bool flag5 = !this.toolSlot.IsValid;
			if (flag5)
			{
				stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Making_Tool_Not_Selected));
			}
			bool flag6 = !toolMeet;
			if (flag6)
			{
				stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Making_Tool_Durability_Not_Enough));
			}
			bool flag7 = !attainmentMeet;
			if (flag7)
			{
				stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Making_Attainment_Not_Enough));
			}
			bool flag8 = !condenseMeet;
			if (flag8)
			{
				stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Add_Poison_Condense_MedicineCount_NotEnough));
			}
			this.confirmTip.Type = TipType.SingleDesc;
			TooltipInvoker tooltipInvoker = this.confirmTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			this.confirmTip.RuntimeParam.Set("arg0", stringBuilder.ToString());
			this.confirmTip.enabled = !this.confirmBtn.interactable;
			EasyPool.Free<StringBuilder>(stringBuilder);
		}

		// Token: 0x060070F7 RID: 28919 RVA: 0x00345238 File Offset: 0x00343438
		private bool CheckTool()
		{
			bool isToolMeet = false;
			bool hasTool = this.toolSlot.IsValid;
			bool flag = hasTool && this.targetSlot.IsValid;
			if (flag)
			{
				int totalCost = this.GetMaxToolDurabilityCost();
				short curDurability = this.toolSlot.ItemData.Durability;
				isToolMeet = ((int)curDurability >= totalCost);
				sbyte skillType = this.ParentView.CurLifeSkillType;
				LifeSkillTypeItem lifeSkillConfig = Config.LifeSkillType.Instance[skillType];
				short lifeSkillAttainment = this.DisplayData.LifeSkillAttainments.Get((int)skillType);
				short toolAttainment = UI_Make.GetToolAttainment(this.toolSlot.ItemData.RealKey.TemplateId, lifeSkillAttainment, skillType);
				string toolAttainmentText = (toolAttainment >= 0) ? string.Format("+{0}", toolAttainment) : toolAttainment.ToString().SetColor("brightred");
				this.propertyToolAttainment.Set(lifeSkillConfig.Icon, lifeSkillConfig.Name, toolAttainmentText, null, false);
			}
			return isToolMeet;
		}

		// Token: 0x060070F8 RID: 28920 RVA: 0x00345338 File Offset: 0x00343538
		private int GetMaxToolDurabilityCost()
		{
			bool flag = !this.toolSlot.IsValid || !this.targetSlot.IsValid;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				sbyte grade = this.GetMaterialMaxGrade();
				short cost = ViewMake.GetToolDurabilityCost(this.toolSlot.ItemData, grade);
				result = (int)cost;
			}
			return result;
		}

		// Token: 0x060070F9 RID: 28921 RVA: 0x0034538A File Offset: 0x0034358A
		private void RefreshConfirmBtn(bool interactable)
		{
			this.confirmBtn.interactable = interactable;
		}

		// Token: 0x060070FA RID: 28922 RVA: 0x0034539C File Offset: 0x0034359C
		private void ConfirmAddPoison()
		{
			UIElement.FullScreenMask.Show();
			this.RefreshConfirmBtn(false);
			ItemKey[] slotItemKeys = this.GetMedicineItemArray();
			BuildingDomainMethod.AsyncCall.CheckAddPoisonCondition(null, this.TaiwuCharId, this.toolSlot.ItemData.Key, this.targetSlot.ItemData.Key, slotItemKeys, this.ParentView.BuildingBlockKey, this._tempPoisonEffects, delegate(int offset, RawDataPool dataPool)
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
					ItemDisplayData[] slotItemArray = this.GetMedicineItemDisplayDataArray();
					List<ItemDisplayData> condensePoisonItemList = this.GetCondensePoisonItemList();
					BuildingDomainMethod.AsyncCall.AddItemPoison(null, this.TaiwuCharId, this.toolSlot.ItemData, this.targetSlot.ItemData, slotItemArray, condensePoisonItemList, delegate(int offset2, RawDataPool dataPool2)
					{
						MakeSubPageAddPoison.<>c__DisplayClass126_0 CS$<>8__locals1;
						CS$<>8__locals1.result = new ValueTuple<bool, ItemDisplayData>(false, new ItemDisplayData());
						Serializer.Deserialize(dataPool2, offset2, ref CS$<>8__locals1.result);
						this.<ConfirmAddPoison>g__Action|126_2(ref CS$<>8__locals1);
					});
				}
			});
		}

		// Token: 0x060070FB RID: 28923 RVA: 0x0034540F File Offset: 0x0034360F
		private void OnClickBackButtn()
		{
			this.ParentView.ExitFocusMode();
		}

		// Token: 0x060070FC RID: 28924 RVA: 0x0034541E File Offset: 0x0034361E
		private void RefreshPoisonInfo()
		{
			this.RefreshPoisonIcon();
			this.poisonInfos.Refresh(this._tempPoisonEffects, this.condenseToggle.isOn);
		}

		// Token: 0x060070FD RID: 28925 RVA: 0x00345448 File Offset: 0x00343648
		private void RefreshPoisonIcon()
		{
			for (int i = 0; i < this.medicineSlotList.Length; i++)
			{
				MakeTargetSlot medicineSlot = this.medicineSlotList[i];
				medicineSlot.HidePoison();
			}
			bool flag = this.targetSlot.IsValid && this.targetSlot.ItemData.HasAnyPoison && this.targetSlot.ItemData.PoisonIsIdentified;
			if (flag)
			{
				for (int j = 0; j < this.targetSlot.ItemData.PoisonEffects.PoisonSlotList.Count; j++)
				{
					PoisonSlot poisonSlot = this.targetSlot.ItemData.PoisonEffects.PoisonSlotList[j];
					MakeTargetSlot medicineSlot2 = this.medicineSlotList[j * PoisonSlot.MaxMedicineCount];
					bool flag2 = medicineSlot2.IsValid && !medicineSlot2.ItemData.RealKey.IsValid();
					if (flag2)
					{
						medicineSlot2.ShowPoison(poisonSlot.MedicineConfig.PoisonType);
					}
					bool flag3 = this.condenseToggle.isOn && poisonSlot.CondensedMedicineTemplateIdList != null;
					if (flag3)
					{
						for (int k = 0; k < poisonSlot.CondensedMedicineTemplateIdList.Count; k++)
						{
							medicineSlot2 = this.medicineSlotList[j * PoisonSlot.MaxMedicineCount + k + 1];
							short templateId = poisonSlot.CondensedMedicineTemplateIdList[k];
							MedicineItem config = Medicine.Instance[templateId];
							bool flag4 = medicineSlot2.IsValid && !medicineSlot2.ItemData.RealKey.IsValid();
							if (flag4)
							{
								medicineSlot2.ShowPoison(config.PoisonType);
							}
						}
					}
				}
				this.poisonInfos.SetStyleEffect(this.targetSlot.ItemData.PoisonEffects.PoisonSlotList.Count, false);
			}
			else
			{
				this.poisonInfos.SetStyleEffect(0, true);
			}
		}

		// Token: 0x060070FE RID: 28926 RVA: 0x00345650 File Offset: 0x00343850
		private void SetHoverShow(bool isShow, int slotIndex)
		{
			bool flag = this.hoverObjects == null && !this.condenseToggle.isOn;
			if (!flag)
			{
				this.hoverObjects[slotIndex / 3].SetActive(this.condenseToggle.isOn && isShow);
			}
		}

		// Token: 0x06007100 RID: 28928 RVA: 0x0034574C File Offset: 0x0034394C
		[CompilerGenerated]
		private void <RefreshAllItemList>g__Add|62_0(List<ItemDisplayData> list)
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
							bool flag2 = config.EffectType == EMedicineEffectType.ApplyPoison;
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

		// Token: 0x06007101 RID: 28929 RVA: 0x00345850 File Offset: 0x00343A50
		[CompilerGenerated]
		internal static void <GenerateRowTemplateContainers>g__CreateCellContainers|69_0(Transform containerRoot, RowCellContainer prefab)
		{
			RowCellContainer container = Object.Instantiate<RowCellContainer>(prefab, containerRoot);
			container.gameObject.SetActive(true);
		}

		// Token: 0x06007105 RID: 28933 RVA: 0x00345A18 File Offset: 0x00343C18
		[CompilerGenerated]
		internal static bool <GetNeedleCount>g__Match|114_0(ItemDisplayData i)
		{
			ItemKey key = i.Key;
			return key.ItemType == 12 && key.TemplateId == 264;
		}

		// Token: 0x06007109 RID: 28937 RVA: 0x00345B0C File Offset: 0x00343D0C
		[CompilerGenerated]
		private void <ConfirmAddPoison>g__Action|126_2(ref MakeSubPageAddPoison.<>c__DisplayClass126_0 A_1)
		{
			bool item = A_1.result.Item1;
			if (item)
			{
				this.SetTargetCache(A_1.result.Item2);
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.SetObject("ItemList", new List<ItemDisplayData>
				{
					A_1.result.Item2
				});
				argBox.Set("ObtainType", 11);
				UIElement.GetItem.SetOnInitArgs(argBox);
				UIManager.Instance.MaskUI(UIElement.GetItem);
				this.toolSlot.Cancel();
				this.toolSlot.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.Disable);
				this.ClearToolList();
				bool isShowEffectHandler = this._isShowEffectHandler;
				if (isShowEffectHandler)
				{
					this.toolSlot.SetEffectHandlerState(false);
					this.leftLineGold.gameObject.SetActive(false);
					this.rightLineGold.gameObject.SetActive(false);
					this._isShowEffectHandler = false;
				}
				this.DisplayData.Clear();
				this.ParentView.RequestData();
			}
			else
			{
				this.ShowUnidentifiedPoisonTip(true, LanguageKey.LK_Remove_Poison_Has_Unidentified);
			}
		}

		// Token: 0x040053B9 RID: 21433
		[SerializeField]
		private MakeTargetSlot[] medicineSlotList = new MakeTargetSlot[FullPoisonEffects.MaxSlotCount * PoisonSlot.MaxMedicineCount];

		// Token: 0x040053BA RID: 21434
		[SerializeField]
		private MakeTargetSlot targetSlot;

		// Token: 0x040053BB RID: 21435
		[SerializeField]
		private MakeTargetSlot toolSlot;

		// Token: 0x040053BC RID: 21436
		[SerializeField]
		private CImage imgTargetSlotBack;

		// Token: 0x040053BD RID: 21437
		[SerializeField]
		private CImage imgMedicineSlotBack;

		// Token: 0x040053BE RID: 21438
		[SerializeField]
		private GameObject goMaterialHolder;

		// Token: 0x040053BF RID: 21439
		[SerializeField]
		private GameObject goTxtSelectPoisonTips;

		// Token: 0x040053C0 RID: 21440
		[SerializeField]
		private GameObject goLineTargetToToolGrey;

		// Token: 0x040053C1 RID: 21441
		[SerializeField]
		private GameObject goLineTargetToToolGold;

		// Token: 0x040053C2 RID: 21442
		[SerializeField]
		private GameObject goLineMaterialToTargetNormal;

		// Token: 0x040053C3 RID: 21443
		[SerializeField]
		private CToggleGroup medicineSourceToggleGroup;

		// Token: 0x040053C4 RID: 21444
		[SerializeField]
		private ItemListScroll medicineListScroll;

		// Token: 0x040053C5 RID: 21445
		[SerializeField]
		private GameObject medicinePanel;

		// Token: 0x040053C6 RID: 21446
		[SerializeField]
		private CButton backButtn;

		// Token: 0x040053C7 RID: 21447
		[SerializeField]
		private GameObject[] hoverObjects;

		// Token: 0x040053C8 RID: 21448
		[SerializeField]
		private GameObject leftLineGold;

		// Token: 0x040053C9 RID: 21449
		[SerializeField]
		private GameObject rightLineGold;

		// Token: 0x040053CA RID: 21450
		[SerializeField]
		private CToggleGroup targetToggleGroup;

		// Token: 0x040053CB RID: 21451
		[SerializeField]
		private ItemListScroll targetListScroll;

		// Token: 0x040053CC RID: 21452
		[SerializeField]
		private GameObject targetPanel;

		// Token: 0x040053CD RID: 21453
		[SerializeField]
		private PropertyItem propertyToolDurability;

		// Token: 0x040053CE RID: 21454
		[SerializeField]
		private PropertyItem propertyToolAttainment;

		// Token: 0x040053CF RID: 21455
		[SerializeField]
		private PropertyItem propertyNeedAttainment;

		// Token: 0x040053D0 RID: 21456
		[SerializeField]
		private PoisonInfos poisonInfos;

		// Token: 0x040053D1 RID: 21457
		[SerializeField]
		private CButton confirmBtn;

		// Token: 0x040053D2 RID: 21458
		[SerializeField]
		private CButton identityBtn;

		// Token: 0x040053D3 RID: 21459
		[SerializeField]
		private CToggle condenseToggle;

		// Token: 0x040053D4 RID: 21460
		[SerializeField]
		private TextMeshProUGUI identityTips;

		// Token: 0x040053D5 RID: 21461
		[SerializeField]
		private TextMeshProUGUI mixedPoisonName;

		// Token: 0x040053D6 RID: 21462
		[SerializeField]
		private GameObject selectTargetTips;

		// Token: 0x040053D7 RID: 21463
		[SerializeField]
		private GameObject identifiedPoisonTip;

		// Token: 0x040053D8 RID: 21464
		[SerializeField]
		private TooltipInvoker mixedPoisonTip;

		// Token: 0x040053D9 RID: 21465
		[SerializeField]
		private TooltipInvoker needAttainmentTip;

		// Token: 0x040053DA RID: 21466
		[SerializeField]
		private TooltipInvoker identityBtnTip;

		// Token: 0x040053DB RID: 21467
		[SerializeField]
		private TooltipInvoker confirmTip;

		// Token: 0x040053DC RID: 21468
		[SerializeField]
		private RowCellContainer itemIconAndNameCellContainer;

		// Token: 0x040053DD RID: 21469
		[SerializeField]
		private RowCellContainer refineEffectCellContainer;

		// Token: 0x040053DE RID: 21470
		[SerializeField]
		private RowCellContainer singleTextCellContainer;

		// Token: 0x040053DF RID: 21471
		private readonly List<ItemDisplayData> _allItemList = new List<ItemDisplayData>();

		// Token: 0x040053E0 RID: 21472
		private readonly List<ItemDisplayData> _allToolList = new List<ItemDisplayData>();

		// Token: 0x040053E1 RID: 21473
		private readonly List<ItemDisplayData> _allMedicineList = new List<ItemDisplayData>();

		// Token: 0x040053E2 RID: 21474
		private readonly List<ItemDisplayData> _curSourceMedicineList = new List<ItemDisplayData>();

		// Token: 0x040053E3 RID: 21475
		private readonly List<ItemDisplayData> _curSourceTargetList = new List<ItemDisplayData>();

		// Token: 0x040053E4 RID: 21476
		private bool _isShowEffectHandler;

		// Token: 0x040053E5 RID: 21477
		private readonly FullPoisonEffects _tempPoisonEffects = new FullPoisonEffects();

		// Token: 0x040053E6 RID: 21478
		private FullPoisonEffects _originPoisonEffects;

		// Token: 0x040053E7 RID: 21479
		private List<ITradeableContent> attainmentNotMeetMedicineList = new List<ITradeableContent>();

		// Token: 0x040053E8 RID: 21480
		private List<ITradeableContent> gradeNotMeetMedicineList = new List<ITradeableContent>();

		// Token: 0x040053E9 RID: 21481
		private List<ITradeableContent> itemTypeNotMeetMedicineList = new List<ITradeableContent>();

		// Token: 0x040053EA RID: 21482
		private List<ITradeableContent> poisonLevelNotMeetMedicineList = new List<ITradeableContent>();

		// Token: 0x040053EB RID: 21483
		private List<ITradeableContent> sameMedicineList = new List<ITradeableContent>();

		// Token: 0x040053EC RID: 21484
		private List<ITradeableContent> meetMedicineList = new List<ITradeableContent>();

		// Token: 0x040053ED RID: 21485
		private int _toolMaxFinalAttainment;

		// Token: 0x040053EE RID: 21486
		private bool _curIdentifySuccess;

		// Token: 0x040053EF RID: 21487
		private bool _curIdentifiedResultHasPoison;

		// Token: 0x040053F0 RID: 21488
		private Action _curIdentifiedResultAction;

		// Token: 0x040053F1 RID: 21489
		private ItemDisplayData targetCache;
	}
}
