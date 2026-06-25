using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
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
using GameDataExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Make
{
	// Token: 0x02000956 RID: 2390
	public class MakeSubPageRefine : MakeSubPage
	{
		// Token: 0x17000CEB RID: 3307
		// (get) Token: 0x0600716B RID: 29035 RVA: 0x0034AD60 File Offset: 0x00348F60
		private int TaiwuCharId
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x17000CEC RID: 3308
		// (get) Token: 0x0600716C RID: 29036 RVA: 0x0034AD6C File Offset: 0x00348F6C
		private ViewMake.ItemSourceTogKey CurMaterialTogKey
		{
			get
			{
				return (ViewMake.ItemSourceTogKey)this.materialSourceToggleGroup.GetActiveIndex();
			}
		}

		// Token: 0x17000CED RID: 3309
		// (get) Token: 0x0600716D RID: 29037 RVA: 0x0034AD79 File Offset: 0x00348F79
		private ItemSourceType CurMaterialItemSource
		{
			get
			{
				return ViewMake.GetItemSourceType(this.CurMaterialTogKey);
			}
		}

		// Token: 0x17000CEE RID: 3310
		// (get) Token: 0x0600716E RID: 29038 RVA: 0x0034AD86 File Offset: 0x00348F86
		private ViewMake.ItemSourceTogKey CurTargetTogKey
		{
			get
			{
				return (ViewMake.ItemSourceTogKey)this.targetToggleGroup.GetActiveIndex();
			}
		}

		// Token: 0x17000CEF RID: 3311
		// (get) Token: 0x0600716F RID: 29039 RVA: 0x0034AD93 File Offset: 0x00348F93
		private ItemSourceType CurTargetItemSource
		{
			get
			{
				return ViewMake.GetItemSourceType(this.CurTargetTogKey);
			}
		}

		// Token: 0x06007170 RID: 29040 RVA: 0x0034ADA0 File Offset: 0x00348FA0
		private void RefreshAllItemList()
		{
			this._allToolList.Clear();
			this._allMaterialList.Clear();
			this._allItemList.Clear();
			bool canTransferItemToWarehouse = this.DisplayData.CanTransferItemToWarehouse;
			if (canTransferItemToWarehouse)
			{
				this.<RefreshAllItemList>g__Add|40_0(this.DisplayData.InventoryItemList);
				this.<RefreshAllItemList>g__Add|40_0(this.DisplayData.EquippedItemList);
			}
			this.<RefreshAllItemList>g__Add|40_0(this.DisplayData.WarehouseItemList);
			this.<RefreshAllItemList>g__Add|40_0(this.DisplayData.TreasuryItemList);
		}

		// Token: 0x06007171 RID: 29041 RVA: 0x0034AE30 File Offset: 0x00349030
		private void RefreshTargetList()
		{
			this._curSourceTargetList.Clear();
			foreach (ItemDisplayData target in this._allItemList)
			{
				bool flag = !this.CheckItemSourceMeet(target);
				if (!flag)
				{
					bool flag2 = !ItemTemplateHelper.IsRefinable(target.Key.ItemType, target.Key.TemplateId) || ItemTemplateHelper.GetCraftRequiredLifeSkillType(target.Key.ItemType, target.Key.TemplateId) != this.ParentView.CurLifeSkillType;
					if (!flag2)
					{
						this._curSourceTargetList.Add(target);
					}
				}
			}
			this.targetListScroll.SetItemList(this._curSourceTargetList);
		}

		// Token: 0x06007172 RID: 29042 RVA: 0x0034AF10 File Offset: 0x00349110
		private bool CheckItemSourceMeet(ItemDisplayData itemDisplayData)
		{
			return itemDisplayData.ItemSourceTypeEnum == this.CurTargetItemSource || (this.CurTargetItemSource == ItemSourceType.Inventory && itemDisplayData.ItemSourceTypeEnum == ItemSourceType.Equipment);
		}

		// Token: 0x06007173 RID: 29043 RVA: 0x0034AF48 File Offset: 0x00349148
		private void RefreshMaterialList()
		{
			this._curSourceMaterialList.Clear();
			bool flag = !this.targetSlot.IsValid;
			if (flag)
			{
				this.materialListScroll.SetItemList(this._curSourceMaterialList);
			}
			else
			{
				foreach (ItemDisplayData target in this._allMaterialList)
				{
					bool flag2 = target.ItemSourceTypeEnum != this.CurMaterialItemSource;
					if (!flag2)
					{
						this._curSourceMaterialList.Add(target);
					}
				}
				this.InitCurSourceMaterialCacheListFromDict();
				this.materialListScroll.SetItemList(this._curSourceMaterialCacheList);
			}
		}

		// Token: 0x06007174 RID: 29044 RVA: 0x0034B00C File Offset: 0x0034920C
		private void RefreshAllData()
		{
			this.RefreshAllItemList();
			this.RefreshTargetList();
			this.RefreshMaterialList();
		}

		// Token: 0x06007175 RID: 29045 RVA: 0x0034B024 File Offset: 0x00349224
		public override void Init(ViewMake parentView)
		{
			base.Init(parentView);
			this.ClearSlotItemChange();
			this.ClearMaterialList();
			this.ClearTargetList();
			this.ResetPanelState();
			this.SetTargetCache(null);
			this.details.SetActive(false);
			this.targetSlot.Init(EMakeTargetSlotInteract.Always, EMakeTargetSlotType.CustomTarget, new Action<int, ItemDisplayData>(this.OnTargetSlotCancel), new Action(this.OnTargetSlotSelect), null, null, -1, null, false, null);
			this.targetSlot.SetEffectHandlerState(true);
			for (int i = 0; i < this.toolSlotList.Length; i++)
			{
				MakeTargetSlot slot = this.toolSlotList[i];
				slot.Init(EMakeTargetSlotInteract.Custom, EMakeTargetSlotType.RefineTool, new Action<int, ItemDisplayData>(this.OnCancelTool), null, new Func<bool>(this.GetToolInteractable), null, i, null, false, null);
			}
			for (int j = 0; j < this.materialSlotList.Length; j++)
			{
				MakeTargetSlot slot2 = this.materialSlotList[j];
				slot2.Init(EMakeTargetSlotInteract.Custom, EMakeTargetSlotType.MakeMaterial, new Action<int, ItemDisplayData>(this.OnMaterialSlotCancel), null, new Func<bool>(this.GetMaterialInteractable), null, j, null, false, null);
			}
			this.targetListScroll.Init("MakeSubPageRefineTarget", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnTargetListItemRender), new Action<ITradeableContent, RowItemLine>(this.OnTargetListItemClick), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.RefineAttribute, null, null, null);
			this.targetListScroll.SortAndFilterController.SetToggleVisible(EFilterLine.MainFilter.ToInt(), new List<int>
			{
				EMainFilterKeys.Equip.ToInt()
			}, false);
			this.targetListScroll.SortAndFilterController.SetToggleIsOnWithoutNotify(EFilterLine.MainFilter.ToInt(), EMainFilterKeys.Equip.ToInt());
			this.materialListScroll.Init("MakeSubPageRefineMaterial", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnMaterialListItemRender), new Action<ITradeableContent, RowItemLine>(this.OnMaterialListItemClick), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability, null, this.GenerateColumnDefinitions(), new Action<RowItem>(this.GenerateRowTemplateContainers));
			this.materialListScroll.SortAndFilterController.SetToggleVisible(EFilterLine.MainFilter.ToInt(), new List<int>
			{
				EMainFilterKeys.Material.ToInt()
			}, false);
			this.materialListScroll.SortAndFilterController.SetToggleIsOnWithoutNotify(EFilterLine.MainFilter.ToInt(), EMainFilterKeys.Material.ToInt());
		}

		// Token: 0x06007176 RID: 29046 RVA: 0x0034B265 File Offset: 0x00349465
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitions()
		{
			ColumnDefinition<ITradeableContent, ITradeableContent> columnDefinition = new ColumnDefinition<ITradeableContent, ITradeableContent>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 200f,
				FlexibleWidth = 1f,
				PreferredWidth = 300f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Item.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => data);
			columnDefinition.SortId = 0;
			yield return columnDefinition;
			ColumnDefinition<ITradeableContent, ItemDisplayData> columnDefinition2 = new ColumnDefinition<ITradeableContent, ItemDisplayData>();
			columnDefinition2.LayoutOption = new LayoutOption
			{
				MinWidth = 200f,
				FlexibleWidth = 1f,
				PreferredWidth = 300f,
				Priority = 1
			};
			columnDefinition2.TableHeadLabel = (() => SortItem.Instance[167].Names[0]);
			columnDefinition2.CellDataGenerator = ((ITradeableContent data) => (ItemDisplayData)data);
			columnDefinition2.SortId = 167;
			yield return columnDefinition2;
			ColumnDefinition<ITradeableContent, string> columnDefinition3 = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition3.LayoutOption = new LayoutOption
			{
				MinWidth = 50f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition3.TableHeadLabel = (() => LanguageKey.LK_Count.Tr());
			columnDefinition3.CellDataGenerator = ((ITradeableContent data) => CommonUtils.GetDisplayStringForNum(data.Amount, 100000));
			columnDefinition3.SortId = 17;
			yield return columnDefinition3;
			yield break;
		}

		// Token: 0x06007177 RID: 29047 RVA: 0x0034B275 File Offset: 0x00349475
		private void GenerateRowTemplateContainers(RowItem rowItem)
		{
			MakeSubPageRefine.<GenerateRowTemplateContainers>g__CreateCellContainers|47_0(rowItem.ContainerRoot, this.itemIconAndNameCellContainer);
			MakeSubPageRefine.<GenerateRowTemplateContainers>g__CreateCellContainers|47_0(rowItem.ContainerRoot, this.refineEffectCellContainer);
			MakeSubPageRefine.<GenerateRowTemplateContainers>g__CreateCellContainers|47_0(rowItem.ContainerRoot, this.singleTextCellContainer);
		}

		// Token: 0x06007178 RID: 29048 RVA: 0x0034B2B0 File Offset: 0x003494B0
		public override void Refresh(BuildingMakeDisplayData displayData)
		{
			base.Refresh(displayData);
			this.ParentView.RefreshSourceToggleInteractable(this.materialSourceToggleGroup);
			this.ParentView.RefreshSourceToggleInteractable(this.targetToggleGroup);
			this.ParentView.ClearToolList("");
			this.ClearSlotItemChange();
			this.RefreshAllData();
			this.ReloadSlot();
			this.RefreshInfo();
			this._lastIsValid = this.targetSlot.IsValid;
			this.details.SetActive(this._lastIsValid);
			this.ParentView.ShowToolPanel(this._lastIsValid);
		}

		// Token: 0x06007179 RID: 29049 RVA: 0x0034B34C File Offset: 0x0034954C
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
						this.RefreshMaterialList();
						this.SetTargetCache(null);
						this.SetMaterialSlotFromTarget();
						MakeTargetSlot[] array = this.toolSlotList;
						for (int i = 0; i < array.Length; i++)
						{
							MakeTargetSlot slot = array[i];
							bool flag5 = !slot.IsValid;
							if (!flag5)
							{
								ItemDisplayData toolData = (slot.ItemData == this.ParentView.EmptyTool) ? this.ParentView.EmptyTool : this.ParentView.AllToolList.Find((ItemDisplayData d) => d.RealKey == slot.ItemData.RealKey && d.ItemSourceTypeEnum == slot.ItemData.ItemSourceTypeEnum);
								bool flag6 = toolData == null;
								if (flag6)
								{
									slot.Cancel();
									break;
								}
								this.SelectTool(toolData);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600717A RID: 29050 RVA: 0x0034B4F0 File Offset: 0x003496F0
		private void ResetPanelState()
		{
			this.targetSlot.Select(null, false);
			foreach (MakeTargetSlot slot in this.materialSlotList)
			{
				slot.Select(null, false);
			}
			foreach (MakeTargetSlot slot2 in this.toolSlotList)
			{
				slot2.Select(null, false);
			}
		}

		// Token: 0x0600717B RID: 29051 RVA: 0x0034B560 File Offset: 0x00349760
		private void Awake()
		{
			this.materialSourceToggleGroup.OnActiveIndexChange += this.MaterialSourceToggleGroupOnOnActiveIndexChange;
			this.materialSourceToggleGroup.Init(-1);
			this.targetToggleGroup.OnActiveIndexChange += this.TargetToggleGroupOnOnActiveIndexChange;
			this.targetToggleGroup.Init(-1);
			this.confirmBtn.onClick.ResetListener(new Action(this.ConfirmRefine));
			this.btnBackToMake.onClick.ResetListener(delegate()
			{
				this.ParentView.ExitFocusMode();
			});
		}

		// Token: 0x0600717C RID: 29052 RVA: 0x0034B5F4 File Offset: 0x003497F4
		private void Update()
		{
			bool isValid = this.targetSlot.IsValid;
			bool flag = this._lastIsValid != this.targetSlot.IsValid;
			if (flag)
			{
				this._lastIsValid = isValid;
				this.details.SetActive(isValid);
				this.ParentView.ShowToolPanel(isValid);
			}
		}

		// Token: 0x0600717D RID: 29053 RVA: 0x0034B64B File Offset: 0x0034984B
		private void OnDestroy()
		{
			this.materialSourceToggleGroup.OnActiveIndexChange -= this.MaterialSourceToggleGroupOnOnActiveIndexChange;
			this.targetToggleGroup.OnActiveIndexChange -= this.TargetToggleGroupOnOnActiveIndexChange;
		}

		// Token: 0x0600717E RID: 29054 RVA: 0x0034B680 File Offset: 0x00349880
		private void OnTargetListItemClick(ITradeableContent content, RowItemLine rowItemLine)
		{
			ItemDisplayData itemData = (ItemDisplayData)content;
			RefineEffect.SetTarget(itemData.RealKey.ItemType);
			foreach (MakeTargetSlot slot in this.materialSlotList)
			{
				slot.Refresh(false);
			}
			foreach (MakeTargetSlot slot2 in this.toolSlotList)
			{
				slot2.Refresh(false);
			}
			this.targetSlot.SetEffectHandlerState(false);
			this.targetPanel.gameObject.SetActive(false);
			this.ParentView.ExitFocusMode();
			this.targetSlot.Select(itemData, false);
			this.SetMaterialSlotFromTarget();
			this.ClearSlotItemChange();
			this.RefreshMaterialList();
			this.RefreshInfo();
		}

		// Token: 0x0600717F RID: 29055 RVA: 0x0034B74D File Offset: 0x0034994D
		private void RefreshInfo()
		{
			this.RefreshSelectTargetTips();
			this.RefreshTargetName();
			this.RefreshRefineInfo();
			this.RefreshCheckCondition();
			this.RefreshToolByMaterial();
			this.RefreshToolAttainmentAndDurability();
			this.CheckCondition();
		}

		// Token: 0x06007180 RID: 29056 RVA: 0x0034B784 File Offset: 0x00349984
		private void OnTargetListItemRender(ITradeableContent content, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(content);
			rowItemLine.Set(rowItemMain, true);
			rowItemLine.SetInteractable(true, true);
			rowItemLine.SetDisabled(false);
		}

		// Token: 0x06007181 RID: 29057 RVA: 0x0034B7BB File Offset: 0x003499BB
		private void TargetToggleGroupOnOnActiveIndexChange(int newIndex, int oldIndex)
		{
			this.RefreshTargetList();
		}

		// Token: 0x06007182 RID: 29058 RVA: 0x0034B7C5 File Offset: 0x003499C5
		private void ClearTargetList()
		{
			this._curSourceTargetList.Clear();
			this.targetListScroll.SetEmptyContent(string.Empty);
			this.targetListScroll.SetItemList(this._curSourceTargetList);
		}

		// Token: 0x06007183 RID: 29059 RVA: 0x0034B7F7 File Offset: 0x003499F7
		private void OnTargetSlotSelect()
		{
			this.targetPanel.gameObject.SetActive(true);
			this.ParentView.EnterFocusMode(this.targetPanel.transform, null);
		}

		// Token: 0x06007184 RID: 29060 RVA: 0x0034B824 File Offset: 0x00349A24
		private void OnTargetSlotCancel(int slotIndex, ItemDisplayData itemDisplayData)
		{
			foreach (MakeTargetSlot slot in this.materialSlotList)
			{
				slot.Cancel();
			}
			foreach (MakeTargetSlot slot2 in this.toolSlotList)
			{
				slot2.Cancel();
			}
			this.targetSlot.SetEffectHandlerState(true);
			this.ParentView.ClearToolList("");
			this.ClearMaterialList();
			this.ClearSlotItemChange();
			this.RefreshInfo();
		}

		// Token: 0x06007185 RID: 29061 RVA: 0x0034B8B2 File Offset: 0x00349AB2
		private void RefreshSelectTargetTips()
		{
			this.selectTargetTips.gameObject.SetActive(!this.targetSlot.IsValid);
		}

		// Token: 0x06007186 RID: 29062 RVA: 0x0034B8D4 File Offset: 0x00349AD4
		private void RefreshTargetName()
		{
			this.targetName.SetText(this.targetSlot.IsValid ? this.targetSlot.ItemData.GetName(false) : LocalStringManager.Get(LanguageKey.LK_Refine_Target), true);
		}

		// Token: 0x06007187 RID: 29063 RVA: 0x0034B910 File Offset: 0x00349B10
		private void OnMaterialListItemClick(ITradeableContent content, RowItemLine rowItemLine)
		{
			ItemDisplayData material = (ItemDisplayData)content;
			int slotIndex = this.GetMaterialSlotIndexEmpty();
			bool flag = slotIndex >= 0;
			if (flag)
			{
				this.materialSlotList[slotIndex].Select(material, false);
				this.ChangeMaterialCacheList(material, false, true);
				this.materialListScroll.SetItemList(this._curSourceMaterialCacheList);
				this.RefreshInfo();
				this.ParentView.RefreshToolList(this.targetSlot.ItemData.MakeNeedAttainment, GameData.Domains.Character.LifeSkillType.RefineTypes.ToList<sbyte>(), this.GetMaterialSlotGradeList(), null, new Action<ItemDisplayData>(this.SelectTool), false, 1);
			}
		}

		// Token: 0x06007188 RID: 29064 RVA: 0x0034B9A8 File Offset: 0x00349BA8
		private int GetMaterialSlotIndexEmpty()
		{
			for (int i = 0; i < this.materialSlotList.Length; i++)
			{
				MakeTargetSlot slot = this.materialSlotList[i];
				bool flag = !slot.IsValid;
				if (flag)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06007189 RID: 29065 RVA: 0x0034B9F0 File Offset: 0x00349BF0
		private void OnMaterialListItemRender(ITradeableContent content, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(content);
			rowItemLine.Set(rowItemMain, true);
			bool disable = this.GetMaterialSlotIndexEmpty() < 0;
			rowItemLine.SetInteractable(!disable, true);
			rowItemLine.SetDisabled(disable);
			rowItemMain.SetExtendStatus(!content.Key.IsValid(), "");
		}

		// Token: 0x0600718A RID: 29066 RVA: 0x0034BA51 File Offset: 0x00349C51
		private void ClearMaterialList()
		{
			this._curSourceMaterialList.Clear();
			this.materialListScroll.SetEmptyContent(string.Empty);
			this.materialListScroll.SetItemList(this._curSourceMaterialList);
		}

		// Token: 0x0600718B RID: 29067 RVA: 0x0034BA84 File Offset: 0x00349C84
		private void OnMaterialSlotCancel(int index, ItemDisplayData itemDisplayData)
		{
			this.ChangeMaterialCacheList(itemDisplayData, true, true);
			this.materialListScroll.SetItemList(this._curSourceMaterialCacheList);
			this.RefreshInfo();
			bool isValid = this.targetSlot.IsValid;
			if (isValid)
			{
				this.ParentView.RefreshToolList(this.targetSlot.ItemData.MakeNeedAttainment, GameData.Domains.Character.LifeSkillType.RefineTypes.ToList<sbyte>(), this.GetMaterialSlotGradeList(), null, new Action<ItemDisplayData>(this.SelectTool), false, 1);
			}
		}

		// Token: 0x0600718C RID: 29068 RVA: 0x0034BAFF File Offset: 0x00349CFF
		private bool GetMaterialInteractable()
		{
			return this.targetSlot.IsValid;
		}

		// Token: 0x0600718D RID: 29069 RVA: 0x0034BB0C File Offset: 0x00349D0C
		private void MaterialSourceToggleGroupOnOnActiveIndexChange(int newIndex, int oldIndex)
		{
			this.RefreshMaterialList();
		}

		// Token: 0x0600718E RID: 29070 RVA: 0x0034BB18 File Offset: 0x00349D18
		private ItemKey[] GetMaterialItemArray()
		{
			ItemKey[] result = new ItemKey[5];
			for (int i = 0; i < 5; i++)
			{
				MakeTargetSlot slot = this.materialSlotList[i];
				bool isValid = slot.IsValid;
				if (isValid)
				{
					result[i] = slot.ItemData.RealKey;
				}
				else
				{
					result[i] = ItemKey.Invalid;
				}
			}
			return result;
		}

		// Token: 0x0600718F RID: 29071 RVA: 0x0034BB7C File Offset: 0x00349D7C
		private ItemDisplayData[] GetMaterialItemDisplayDataArray()
		{
			ItemDisplayData[] result = new ItemDisplayData[5];
			for (int i = 0; i < 5; i++)
			{
				MakeTargetSlot slot = this.materialSlotList[i];
				bool isValid = slot.IsValid;
				if (isValid)
				{
					result[i] = slot.ItemData;
				}
				else
				{
					result[i] = new ItemDisplayData
					{
						Key = ItemKey.Invalid
					};
				}
			}
			return result;
		}

		// Token: 0x06007190 RID: 29072 RVA: 0x0034BBE0 File Offset: 0x00349DE0
		private short GetLifeSkillTotalAttainment(sbyte type, ItemDisplayData tool)
		{
			short attainment = this.DisplayData.LifeSkillAttainments.Get((int)type);
			bool flag = !tool.RealKey.IsValid();
			short result;
			if (flag)
			{
				result = attainment;
			}
			else
			{
				bool flag2 = this.IsEmptyTool(tool.RealKey);
				if (flag2)
				{
					short finalAttainment = ViewMake.GetFinalAttainment(tool.RealKey.TemplateId, attainment, this.ParentView.CurLifeSkillType);
					result = finalAttainment;
				}
				else
				{
					CraftToolItem toolConfig = CraftTool.Instance[tool.RealKey.TemplateId];
					bool flag3 = toolConfig != null && toolConfig.RequiredLifeSkillTypes.Contains(type);
					if (flag3)
					{
						short finalAttainment2 = ViewMake.GetFinalAttainment(tool.RealKey.TemplateId, attainment, this.ParentView.CurLifeSkillType);
						result = finalAttainment2;
					}
					else
					{
						result = attainment;
					}
				}
			}
			return result;
		}

		// Token: 0x06007191 RID: 29073 RVA: 0x0034BCB0 File Offset: 0x00349EB0
		private bool IsEmptyTool(ItemKey itemKey)
		{
			return itemKey.ItemType == 6 && itemKey.TemplateId == 54;
		}

		// Token: 0x06007192 RID: 29074 RVA: 0x0034BCD8 File Offset: 0x00349ED8
		private void SelectTool(ItemDisplayData itemData)
		{
			bool flag = itemData == null;
			if (!flag)
			{
				CraftToolItem config = CraftTool.Instance[itemData.RealKey.TemplateId];
				bool flag2 = config.TemplateId == 54;
				if (!flag2)
				{
					bool flag3 = config.RequiredLifeSkillTypes.Intersect(GameData.Domains.Character.LifeSkillType.RefineTypes).Any<sbyte>();
					if (flag3)
					{
						sbyte requireLifeSkillType = config.RequiredLifeSkillTypes[0];
						int index = GameData.Domains.Character.LifeSkillType.RefineTypes.IndexOf(requireLifeSkillType);
						bool flag4 = index >= 0 && this.toolSlotList[index].IsValid;
						if (flag4)
						{
							this.toolSlotList[index].Select(itemData, false);
						}
					}
					this.RefreshInfo();
				}
			}
		}

		// Token: 0x06007193 RID: 29075 RVA: 0x0034BD85 File Offset: 0x00349F85
		private void OnCancelTool(int index, ItemDisplayData itemDisplayData)
		{
			this.ParentView.RerenderToolList(this.targetSlot.ItemData);
			this.toolSlotList[index].Select(this.ParentView.EmptyTool, false);
			this.RefreshInfo();
		}

		// Token: 0x06007194 RID: 29076 RVA: 0x0034BDC0 File Offset: 0x00349FC0
		private bool GetToolInteractable()
		{
			return this.targetSlot.IsValid;
		}

		// Token: 0x06007195 RID: 29077 RVA: 0x0034BDD0 File Offset: 0x00349FD0
		private ItemKey[] GetToolItemArray()
		{
			ItemKey[] result = new ItemKey[GameData.Domains.Character.LifeSkillType.RefineTypes.Length];
			for (int i = 0; i < GameData.Domains.Character.LifeSkillType.RefineTypes.Length; i++)
			{
				MakeTargetSlot slot = this.toolSlotList[i];
				bool isValid = slot.IsValid;
				if (isValid)
				{
					result[i] = slot.ItemData.RealKey;
				}
				else
				{
					result[i] = this.ParentView.EmptyTool.RealKey;
				}
			}
			return result;
		}

		// Token: 0x06007196 RID: 29078 RVA: 0x0034BE4C File Offset: 0x0034A04C
		private ItemDisplayData[] GetToolItemDisplayDataArray()
		{
			ItemDisplayData[] result = new ItemDisplayData[GameData.Domains.Character.LifeSkillType.RefineTypes.Length];
			for (int i = 0; i < GameData.Domains.Character.LifeSkillType.RefineTypes.Length; i++)
			{
				MakeTargetSlot slot = this.toolSlotList[i];
				bool isValid = slot.IsValid;
				if (isValid)
				{
					result[i] = slot.ItemData;
				}
				else
				{
					result[i] = this.ParentView.EmptyTool;
				}
			}
			return result;
		}

		// Token: 0x06007197 RID: 29079 RVA: 0x0034BEB8 File Offset: 0x0034A0B8
		private void RefreshToolByMaterial()
		{
			bool flag = !this.targetSlot.IsValid;
			if (!flag)
			{
				for (int i = 0; i < this.toolSlotList.Length; i++)
				{
					MakeTargetSlot slot = this.toolSlotList[i];
					bool flag2 = this._requiredLifeSkillTypeWithGrade.Keys.Contains(GameData.Domains.Character.LifeSkillType.RefineTypes[i]);
					if (flag2)
					{
						bool flag3 = !slot.IsValid;
						if (flag3)
						{
							slot.Select(this.ParentView.EmptyTool, false);
						}
					}
					else
					{
						slot.Select(null, false);
					}
				}
			}
		}

		// Token: 0x06007198 RID: 29080 RVA: 0x0034BF4C File Offset: 0x0034A14C
		private List<List<sbyte>> GetMaterialSlotGradeList()
		{
			List<sbyte>[] grades = new List<sbyte>[GameData.Domains.Character.LifeSkillType.RefineTypes.Length];
			for (int i = 0; i < grades.Length; i++)
			{
				grades[i] = new List<sbyte>();
			}
			foreach (KeyValuePair<sbyte, List<sbyte>> keyValuePair in this._requiredLifeSkillTypeWithGrade)
			{
				sbyte b;
				List<sbyte> list;
				keyValuePair.Deconstruct(out b, out list);
				sbyte lifeSkillType = b;
				List<sbyte> gradeList = list;
				int index = GameData.Domains.Character.LifeSkillType.RefineTypes.IndexOf(lifeSkillType);
				bool flag = index >= 0;
				if (flag)
				{
					grades[index] = gradeList;
				}
			}
			return grades.ToList<List<sbyte>>();
		}

		// Token: 0x06007199 RID: 29081 RVA: 0x0034C00C File Offset: 0x0034A20C
		private void RefreshToolAttainmentAndDurability()
		{
			for (int i = 0; i < this.toolSlotList.Length; i++)
			{
				MakeTargetSlot toolSlot = this.toolSlotList[i];
				RefineToolInfo info = this.toolInfo[i];
				info.gameObject.SetActive(toolSlot.IsValid);
				bool isValid = toolSlot.IsValid;
				if (isValid)
				{
					sbyte skillType = GameData.Domains.Character.LifeSkillType.RefineTypes[i];
					LifeSkillTypeItem lifeSkillConfig = Config.LifeSkillType.Instance[skillType];
					short lifeSkillAttainment = this.DisplayData.LifeSkillAttainments.Get((int)skillType);
					short toolAttainment = UI_Make.GetToolAttainment(toolSlot.ItemData.RealKey.TemplateId, lifeSkillAttainment, skillType);
					string toolAttainmentText = (toolAttainment >= 0) ? string.Format("+{0}", toolAttainment) : toolAttainment.ToString().SetColor("brightred");
					info.SetAttainment(lifeSkillConfig.Name, toolAttainmentText);
					short curDurability = toolSlot.ItemData.Durability;
					short maxDurability = toolSlot.ItemData.MaxDurability;
					string curDurabilityColor = true ? "brightblue" : "brightred";
					string costDurabilityStr = string.Format("-{0}", 0);
					string durabilityStr = string.Format("{0}{1}/{2}", curDurability.ToString().SetColor(curDurabilityColor), costDurabilityStr, maxDurability);
					info.SetDurability(durabilityStr);
				}
			}
		}

		// Token: 0x0600719A RID: 29082 RVA: 0x0034C15C File Offset: 0x0034A35C
		private void SetTargetCache(ItemDisplayData cache)
		{
			this.targetCache = cache;
		}

		// Token: 0x0600719B RID: 29083 RVA: 0x0034C168 File Offset: 0x0034A368
		private void CheckCondition()
		{
			ValueTuple<bool, bool, bool, bool> valueTuple = this.RefreshCheckCondition();
			bool resourceMeet = valueTuple.Item1;
			bool durabilityMeet = valueTuple.Item2;
			bool attainmentMeet = valueTuple.Item3;
			bool haveNotChange = valueTuple.Item4;
			LayoutRebuilder.ForceRebuildLayoutImmediate(this.infoHolder);
			this.RefreshConfirmBtn(this.targetSlot.IsValid && resourceMeet && durabilityMeet && attainmentMeet && !haveNotChange);
			this.RefreshConfirmMouseTip(resourceMeet, durabilityMeet, attainmentMeet, haveNotChange);
		}

		// Token: 0x0600719C RID: 29084 RVA: 0x0034C1CF File Offset: 0x0034A3CF
		private void RefreshConfirmBtn(bool interactable)
		{
			this.confirmBtn.interactable = interactable;
		}

		// Token: 0x0600719D RID: 29085 RVA: 0x0034C1E0 File Offset: 0x0034A3E0
		private void RefreshConfirmMouseTip(bool resourceMeet, bool durabilityMeet, bool attainmentMeet, bool haveNotChange)
		{
			this._sb.Clear();
			this.StringBuilderAppend(this._sb, LocalStringManager.Get(LanguageKey.LK_Make_Refine_Confirm_Tip));
			bool flag = !this.targetSlot.IsValid;
			if (flag)
			{
				this.StringBuilderAppend(this._sb, LocalStringManager.Get(LanguageKey.LK_Strengthen_Item_Not_Selected));
			}
			else
			{
				if (haveNotChange)
				{
					this.StringBuilderAppend(this._sb, LocalStringManager.Get(LanguageKey.LK_Strengthen_Material_Not_Selected));
				}
				bool flag2 = !attainmentMeet;
				if (flag2)
				{
					this.StringBuilderAppend(this._sb, LocalStringManager.Get(LanguageKey.LK_Making_Attainment_Not_Enough));
				}
				bool flag3 = !resourceMeet;
				if (flag3)
				{
					this.StringBuilderAppend(this._sb, LocalStringManager.Get(LanguageKey.LK_Building_Repair_Resource_NotEnough));
				}
				bool flag4 = !durabilityMeet;
				if (flag4)
				{
					this.StringBuilderAppend(this._sb, LocalStringManager.Get(LanguageKey.LK_Making_Tool_Durability_Not_Enough));
				}
			}
			this.confirmTip.Type = TipType.SingleDesc;
			TooltipInvoker tooltipInvoker = this.confirmTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			this.confirmTip.RuntimeParam.Set("arg0", this._sb.ToString());
			this.confirmTip.enabled = !this.confirmBtn.interactable;
		}

		// Token: 0x0600719E RID: 29086 RVA: 0x0034C324 File Offset: 0x0034A524
		private void StringBuilderAppend(StringBuilder sb, string text)
		{
			bool flag = sb.Length > 0;
			if (flag)
			{
				sb.Append("\n");
			}
			sb.Append(text);
		}

		// Token: 0x0600719F RID: 29087 RVA: 0x0034C354 File Offset: 0x0034A554
		private void ConfirmRefine()
		{
			this.RefreshConfirmBtn(false);
			ItemDisplayData[] slotItemArray = this.GetMaterialItemDisplayDataArray();
			ItemDisplayData[] toolItemDisplayDataArray = this.GetToolItemDisplayDataArray();
			BuildingDomainMethod.AsyncCall.RefineItem(null, this.TaiwuCharId, toolItemDisplayDataArray, this.targetSlot.ItemData, slotItemArray, this._slotItemChangeDict.Values.ToList<ItemSourceChange>(), delegate(int offset2, RawDataPool dataPool2)
			{
				ItemDisplayData result = new ItemDisplayData();
				this.SetTargetCache(result);
				Serializer.Deserialize(dataPool2, offset2, ref result);
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.SetObject("ItemList", new List<ItemDisplayData>
				{
					result
				});
				argBox.Set("ObtainType", 9);
				UIElement.GetItem.SetOnInitArgs(argBox);
				UIManager.Instance.MaskUI(UIElement.GetItem);
				this.DisplayData.Clear();
				this.ParentView.RequestData();
			});
		}

		// Token: 0x060071A0 RID: 29088 RVA: 0x0034C3B0 File Offset: 0x0034A5B0
		private void ClearSlotItemChange()
		{
			foreach (ItemSourceChange itemSourceChange in this._slotItemChangeDict.Values)
			{
				itemSourceChange.Items.Clear();
			}
		}

		// Token: 0x060071A1 RID: 29089 RVA: 0x0034C414 File Offset: 0x0034A614
		private void SetMaterialSlotFromTarget()
		{
			bool flag = !this.targetSlot.IsValid;
			if (!flag)
			{
				for (int i = 0; i < 5; i++)
				{
					short templateId = this.targetSlot.ItemData.RefiningEffects.GetMaterialTemplateIdAt(i);
					bool flag2 = templateId >= 0;
					if (flag2)
					{
						ItemDisplayData material = new ItemDisplayData
						{
							Key = new ItemKey
							{
								ItemType = 5,
								TemplateId = templateId,
								Id = -1
							},
							Amount = 1
						};
						this.materialSlotList[i].SetRefine(material);
						this.materialSlotList[i].Select(material, false);
					}
					else
					{
						this.materialSlotList[i].SetRefine(null);
						this.materialSlotList[i].Select(null, false);
					}
				}
			}
		}

		// Token: 0x060071A2 RID: 29090 RVA: 0x0034C4F4 File Offset: 0x0034A6F4
		private void RefreshRefineInfo()
		{
			this.refineEffectInfo.gameObject.SetActive(this.targetSlot.IsValid);
			bool isValid = this.targetSlot.IsValid;
			if (isValid)
			{
				ItemKey[] items = this.GetMaterialItemArray();
				this.refineEffectInfo.Refresh(items.ToList<ItemKey>(), this.targetSlot.ItemData.RealKey.ItemType);
			}
		}

		// Token: 0x060071A3 RID: 29091 RVA: 0x0034C560 File Offset: 0x0034A760
		private void ChangeMaterialCacheList(ItemDisplayData itemDisplayData, bool add, bool record = true)
		{
			ItemDisplayData itemDisplay = this._curSourceMaterialCacheList.Find((ItemDisplayData item) => item.RealKey.Equals(itemDisplayData.RealKey));
			if (add)
			{
				bool flag = itemDisplay == null;
				if (flag)
				{
					this._curSourceMaterialCacheList.Add(itemDisplayData);
				}
				else
				{
					itemDisplay.ChangeAmount(itemDisplayData.RealKey, true, 1);
				}
			}
			else
			{
				bool flag2 = itemDisplay == null;
				if (flag2)
				{
					Debug.LogWarning("ChangeMaterialCacheList Exception");
				}
				else
				{
					bool flag3 = itemDisplay.Amount == 1;
					if (flag3)
					{
						this._curSourceMaterialCacheList.Remove(itemDisplay);
					}
					else
					{
						itemDisplay.ChangeAmount(itemDisplayData.RealKey, false, 1);
					}
				}
			}
			if (record)
			{
				if (add)
				{
					this._slotItemChangeDict[this.CurMaterialItemSource].AddItem(itemDisplayData.RealKey, 1, 0);
				}
				else
				{
					this._slotItemChangeDict[this.CurMaterialItemSource].RemoveItem(itemDisplayData.RealKey, 1, 0);
				}
			}
		}

		// Token: 0x060071A4 RID: 29092 RVA: 0x0034C67C File Offset: 0x0034A87C
		private void InitCurSourceMaterialCacheListFromDict()
		{
			this._curSourceMaterialCacheList.Clear();
			foreach (ItemDisplayData itemDisplay in this._curSourceMaterialList)
			{
				this._curSourceMaterialCacheList.Add(itemDisplay.Clone(-1));
			}
			ItemSourceChange changeList = this._slotItemChangeDict[this.CurMaterialItemSource];
			bool flag = changeList.Items != null;
			if (flag)
			{
				foreach (ItemKeyAndCount itemKeyAndCount in changeList.Items)
				{
					ItemKey itemKey2;
					int num;
					itemKeyAndCount.Deconstruct(out itemKey2, out num);
					ItemKey itemKey = itemKey2;
					int count = num;
					bool flag2 = count > 0;
					if (flag2)
					{
						ItemDisplayData itemDisplay2 = new ItemDisplayData
						{
							Key = itemKey,
							Amount = count
						};
						this.ChangeMaterialCacheList(itemDisplay2, true, false);
					}
					else
					{
						bool flag3 = count < 0;
						if (flag3)
						{
							ItemDisplayData itemDisplay3 = new ItemDisplayData
							{
								Key = itemKey,
								Amount = 1
							};
							for (int i = 0; i < Mathf.Abs(count); i++)
							{
								this.ChangeMaterialCacheList(itemDisplay3, false, false);
							}
						}
					}
				}
			}
		}

		// Token: 0x17000CF0 RID: 3312
		// (get) Token: 0x060071A5 RID: 29093 RVA: 0x0034C7EC File Offset: 0x0034A9EC
		private BuildingModel BuildingModel
		{
			get
			{
				return SingletonObject.getInstance<BuildingModel>();
			}
		}

		// Token: 0x060071A6 RID: 29094 RVA: 0x0034C7F4 File Offset: 0x0034A9F4
		[return: TupleElementNames(new string[]
		{
			"resourceMeet",
			"durabilityMeet",
			"attainmentMeet",
			"haveNotChange"
		})]
		private ValueTuple<bool, bool, bool, bool> RefreshCheckCondition()
		{
			bool flag = !this.targetSlot.IsValid;
			ValueTuple<bool, bool, bool, bool> result;
			if (flag)
			{
				this.resourceCostHolder.gameObject.SetActive(false);
				this.needAttainmentHolder.gameObject.SetActive(false);
				result = new ValueTuple<bool, bool, bool, bool>(false, false, false, false);
			}
			else
			{
				short[] originSlotTemplateId = this.targetSlot.ItemData.RefiningEffects.GetAllMaterialTemplateIds();
				ResourceInts needResources = default(ResourceInts);
				LifeSkillShorts needLifeSkill = default(LifeSkillShorts);
				bool resourceMeet = true;
				bool durabilityMeet = true;
				bool attainmentMeet = true;
				bool haveNotChange = true;
				int reducePercent = this.DisplayData.BuildingAttainmentEffect;
				this._requiredLifeSkillTypeWithGrade.Clear();
				Dictionary<ItemDisplayData, int> durabilityCostDict = new Dictionary<ItemDisplayData, int>();
				for (int k = 0; k < 5; k++)
				{
					short oldTemplateId = originSlotTemplateId[k];
					short curTemplateId = this.materialSlotList[k].IsValid ? this.materialSlotList[k].ItemData.RealKey.TemplateId : -1;
					bool isSame = oldTemplateId == curTemplateId;
					bool flag2 = !isSame;
					if (flag2)
					{
						haveNotChange = false;
					}
					bool flag3 = curTemplateId > -1 || oldTemplateId > -1;
					if (flag3)
					{
						short materialTemplateId = this.GetGreaterGradeMaterial(curTemplateId, oldTemplateId);
						MaterialItem materialConfig = Config.Material.Instance[materialTemplateId];
						bool flag4 = oldTemplateId != curTemplateId && curTemplateId > 0;
						if (flag4)
						{
							needResources.Change((int)materialConfig.ResourceType, (int)materialConfig.RequiredResourceAmount);
						}
						bool flag5 = !isSame;
						if (flag5)
						{
							short lifeSkillAttainment = needLifeSkill.Get((int)materialConfig.RequiredLifeSkillType);
							int attainmentEffect = (materialConfig.ResourceType == this.ParentView.CurLifeSkillType) ? reducePercent : 0;
							short attainmentRequirement = GameData.Domains.Building.SharedMethods.GetRequiredLifeSkillAttainmentByBuildingEffect((int)materialConfig.RequiredAttainment, attainmentEffect);
							needLifeSkill.Set((int)materialConfig.RequiredLifeSkillType, Math.Max(lifeSkillAttainment, attainmentRequirement));
							ItemDisplayData[] toolArray = this.GetToolItemDisplayDataArray();
							ItemDisplayData tool = this.GetRefineRequiredTool(toolArray, materialConfig.RequiredLifeSkillType);
							short cost = ViewMake.GetToolDurabilityCost(tool, materialConfig.Grade);
							bool flag6 = !durabilityCostDict.TryAdd(tool, (int)cost);
							if (flag6)
							{
								Dictionary<ItemDisplayData, int> dictionary = durabilityCostDict;
								ItemDisplayData itemDisplayData = tool;
								dictionary[itemDisplayData] += (int)cost;
							}
							List<sbyte> value;
							bool flag7 = this._requiredLifeSkillTypeWithGrade.TryGetValue(materialConfig.RequiredLifeSkillType, out value);
							if (flag7)
							{
								value.Add(materialConfig.Grade);
							}
							else
							{
								this._requiredLifeSkillTypeWithGrade.Add(materialConfig.RequiredLifeSkillType, new List<sbyte>
								{
									materialConfig.Grade
								});
							}
						}
					}
				}
				foreach (KeyValuePair<ItemDisplayData, int> keyValuePair in durabilityCostDict)
				{
					ItemDisplayData itemDisplayData;
					int num;
					keyValuePair.Deconstruct(out itemDisplayData, out num);
					ItemDisplayData tool2 = itemDisplayData;
					int totalCost = num;
					for (int j = 0; j < this.toolSlotList.Length; j++)
					{
						MakeTargetSlot toolSlot = this.toolSlotList[j];
						RefineToolInfo info = this.toolInfo[j];
						bool flag8 = toolSlot.IsValid && !toolSlot.ItemData.Equals(this.ParentView.EmptyTool) && toolSlot.ItemData.Equals(tool2);
						if (flag8)
						{
							short curDurability = toolSlot.ItemData.Durability;
							short maxDurability = toolSlot.ItemData.MaxDurability;
							string curDurabilityColor = ((int)curDurability >= totalCost) ? "brightblue" : "brightred";
							string costDurabilityStr = string.Format("-{0}", totalCost);
							string durabilityStr = string.Format("{0}{1}/{2}", curDurability.ToString().SetColor(curDurabilityColor), costDurabilityStr, maxDurability);
							info.SetDurability(durabilityStr);
							CraftToolItem config = CraftTool.Instance[toolSlot.ItemData.RealKey.TemplateId];
							sbyte requiredLifeSkillType = config.RequiredLifeSkillTypes.FirstOrDefault<sbyte>();
							List<sbyte> gradeList = this._requiredLifeSkillTypeWithGrade[requiredLifeSkillType];
							short costExpectLast = ViewMake.GetRefineToolDurabilityCostExpectLast(toolSlot.ItemData, gradeList);
							bool flag9 = curDurability <= costExpectLast;
							if (flag9)
							{
								durabilityMeet = false;
							}
						}
					}
				}
				bool flag10 = haveNotChange;
				if (flag10)
				{
					this.resourceCostHolder.gameObject.SetActive(false);
					this.needAttainmentHolder.gameObject.SetActive(false);
				}
				else
				{
					int itemCount = 0;
					this.resourceCostHolder.Rebuild<RectTransform>(8, delegate(RectTransform item, int i)
					{
						bool curResourceMeet = true;
						int resource = this.BuildingModel.GetResourceCount((sbyte)i);
						bool flag11 = resource < needResources.Get(i);
						if (flag11)
						{
							resourceMeet = false;
							curResourceMeet = false;
						}
						bool need = needResources.Get(i) > 0;
						item.gameObject.SetActive(need);
						bool flag12 = need;
						if (flag12)
						{
							int itemCount = itemCount;
							itemCount++;
							string color = curResourceMeet ? "brightblue" : "brightred";
							CImage icon = item.GetChild(0).GetComponent<CImage>();
							icon.SetSprite("ui9_btn_resource_bar_{0}_0".GetFormat(i), false, null);
							TextMeshProUGUI resourceName = item.GetChild(1).GetComponent<TextMeshProUGUI>();
							resourceName.SetText(Config.ResourceType.Instance[i].Name, true);
							TextMeshProUGUI cost2 = item.GetChild(2).GetComponent<TextMeshProUGUI>();
							cost2.text = resource.ToString().SetColor(color) + "/" + needResources.Get(i).ToString();
						}
					});
					this.resourceCostHolder.gameObject.SetActive(itemCount > 0);
					attainmentMeet = this.CheckAndShowLifeSkill(needLifeSkill);
				}
				result = new ValueTuple<bool, bool, bool, bool>(resourceMeet, durabilityMeet, attainmentMeet, haveNotChange);
			}
			return result;
		}

		// Token: 0x060071A7 RID: 29095 RVA: 0x0034CCDC File Offset: 0x0034AEDC
		private bool CheckAndShowLifeSkill(LifeSkillShorts needLifeSkill)
		{
			List<short> lifeSkillTypeList = new List<short>();
			bool meet = true;
			for (short i = 0; i < 16; i += 1)
			{
				short value = needLifeSkill.Get((int)i);
				bool flag = value > 0;
				if (flag)
				{
					lifeSkillTypeList.Add(i);
				}
			}
			ItemDisplayData[] toolList = this.GetToolItemDisplayDataArray();
			this.needAttainmentHolder.gameObject.SetActive(true);
			this.needAttainmentHolder.Rebuild<PropertyItem>(lifeSkillTypeList.Count, delegate(PropertyItem item, int index)
			{
				sbyte lifeSkillType = (sbyte)lifeSkillTypeList[index];
				int lifeSkillTypeIndex = GameData.Domains.Character.LifeSkillType.RefineTypes.IndexOf(lifeSkillType);
				short curAttainment = this.GetLifeSkillTotalAttainment(lifeSkillType, toolList[lifeSkillTypeIndex]);
				short needAttainment = needLifeSkill.Get((int)lifeSkillType);
				LifeSkillTypeItem lifeSkillConfig = Config.LifeSkillType.Instance[lifeSkillType];
				bool isSkillMeet = curAttainment >= needAttainment;
				bool flag2 = !isSkillMeet;
				if (flag2)
				{
					meet = false;
				}
				string finalAttainmentColor = isSkillMeet ? "brightblue" : "brightred";
				string attainmentStr = string.Format("{0}/{1}", curAttainment.ToString().SetColor(finalAttainmentColor), needAttainment);
				item.Set(lifeSkillConfig.Icon, lifeSkillConfig.Name, attainmentStr, null, false);
			});
			return meet;
		}

		// Token: 0x060071A8 RID: 29096 RVA: 0x0034CD98 File Offset: 0x0034AF98
		private ItemDisplayData GetRefineRequiredTool(ItemDisplayData[] toolKey, sbyte requiredLifeSkillType)
		{
			int index = GameData.Domains.Character.LifeSkillType.RefineTypes.IndexOf(requiredLifeSkillType);
			bool flag = index >= 0;
			ItemDisplayData result;
			if (flag)
			{
				result = toolKey[index];
			}
			else
			{
				result = this.ParentView.EmptyTool;
			}
			return result;
		}

		// Token: 0x060071A9 RID: 29097 RVA: 0x0034CDD4 File Offset: 0x0034AFD4
		private short GetGreaterGradeMaterial(short templateId1, short templateId2)
		{
			Tester.Assert(templateId1 >= 0 || templateId2 >= 0, "");
			short templateId3 = 0;
			sbyte grade = 0;
			bool flag = templateId1 >= 0;
			if (flag)
			{
				MaterialItem config = Config.Material.Instance[templateId1];
				grade = config.Grade;
				templateId3 = templateId1;
			}
			bool flag2 = templateId2 >= 0;
			if (flag2)
			{
				MaterialItem config2 = Config.Material.Instance[templateId2];
				bool flag3 = config2.Grade >= grade;
				if (flag3)
				{
					templateId3 = templateId2;
				}
			}
			return templateId3;
		}

		// Token: 0x060071AB RID: 29099 RVA: 0x0034CF30 File Offset: 0x0034B130
		[CompilerGenerated]
		private void <RefreshAllItemList>g__Add|40_0(List<ItemDisplayData> list)
		{
			bool flag = list == null;
			if (!flag)
			{
				foreach (ItemDisplayData itemData in list)
				{
					sbyte itemType = itemData.RealKey.ItemType;
					sbyte b = itemType;
					if (b != 5)
					{
						if (b == 6)
						{
							CraftToolItem config = CraftTool.Instance[itemData.RealKey.TemplateId];
							bool flag2 = config.RequiredLifeSkillTypes.Contains(this.ParentView.CurLifeSkillType);
							if (flag2)
							{
								this._allToolList.Add(itemData);
							}
						}
					}
					else
					{
						MaterialItem config2 = Config.Material.Instance[itemData.RealKey.TemplateId];
						bool flag3 = config2.RefiningEffect >= 0;
						if (flag3)
						{
							this._allMaterialList.Add(itemData);
						}
					}
					this._allItemList.Add(itemData);
				}
			}
		}

		// Token: 0x060071AC RID: 29100 RVA: 0x0034D038 File Offset: 0x0034B238
		[CompilerGenerated]
		internal static void <GenerateRowTemplateContainers>g__CreateCellContainers|47_0(Transform containerRoot, RowCellContainer prefab)
		{
			RowCellContainer container = Object.Instantiate<RowCellContainer>(prefab, containerRoot);
			container.gameObject.SetActive(true);
		}

		// Token: 0x04005437 RID: 21559
		[SerializeField]
		private MakeTargetSlot[] materialSlotList = new MakeTargetSlot[5];

		// Token: 0x04005438 RID: 21560
		[SerializeField]
		private MakeTargetSlot targetSlot;

		// Token: 0x04005439 RID: 21561
		[SerializeField]
		private MakeTargetSlot[] toolSlotList = new MakeTargetSlot[GameData.Domains.Character.LifeSkillType.RefineTypes.Length];

		// Token: 0x0400543A RID: 21562
		[SerializeField]
		private CToggleGroup materialSourceToggleGroup;

		// Token: 0x0400543B RID: 21563
		[SerializeField]
		private ItemListScroll materialListScroll;

		// Token: 0x0400543C RID: 21564
		[SerializeField]
		private GameObject materialPanel;

		// Token: 0x0400543D RID: 21565
		[SerializeField]
		private RefineEffectInfo refineEffectInfo;

		// Token: 0x0400543E RID: 21566
		[SerializeField]
		private CToggleGroup targetToggleGroup;

		// Token: 0x0400543F RID: 21567
		[SerializeField]
		private ItemListScroll targetListScroll;

		// Token: 0x04005440 RID: 21568
		[SerializeField]
		private GameObject targetPanel;

		// Token: 0x04005441 RID: 21569
		[SerializeField]
		private RefineToolInfo[] toolInfo = new RefineToolInfo[GameData.Domains.Character.LifeSkillType.RefineTypes.Length];

		// Token: 0x04005442 RID: 21570
		[SerializeField]
		private CButton confirmBtn;

		// Token: 0x04005443 RID: 21571
		[SerializeField]
		private TooltipInvoker confirmTip;

		// Token: 0x04005444 RID: 21572
		[SerializeField]
		private TextMeshProUGUI targetName;

		// Token: 0x04005445 RID: 21573
		[SerializeField]
		private GameObject selectTargetTips;

		// Token: 0x04005446 RID: 21574
		[SerializeField]
		private TemplatedContainerAssemblyNew resourceCostHolder;

		// Token: 0x04005447 RID: 21575
		[SerializeField]
		private TemplatedContainerAssemblyNew needAttainmentHolder;

		// Token: 0x04005448 RID: 21576
		[SerializeField]
		private RectTransform infoHolder;

		// Token: 0x04005449 RID: 21577
		[SerializeField]
		private GameObject details;

		// Token: 0x0400544A RID: 21578
		[SerializeField]
		private CButton btnBackToMake;

		// Token: 0x0400544B RID: 21579
		[SerializeField]
		private RowCellContainer itemIconAndNameCellContainer;

		// Token: 0x0400544C RID: 21580
		[SerializeField]
		private RowCellContainer refineEffectCellContainer;

		// Token: 0x0400544D RID: 21581
		[SerializeField]
		private RowCellContainer singleTextCellContainer;

		// Token: 0x0400544E RID: 21582
		private readonly List<ItemDisplayData> _allItemList = new List<ItemDisplayData>();

		// Token: 0x0400544F RID: 21583
		private readonly List<ItemDisplayData> _allToolList = new List<ItemDisplayData>();

		// Token: 0x04005450 RID: 21584
		private readonly List<ItemDisplayData> _allMaterialList = new List<ItemDisplayData>();

		// Token: 0x04005451 RID: 21585
		private readonly List<ItemDisplayData> _curSourceMaterialList = new List<ItemDisplayData>();

		// Token: 0x04005452 RID: 21586
		private readonly List<ItemDisplayData> _curSourceMaterialCacheList = new List<ItemDisplayData>();

		// Token: 0x04005453 RID: 21587
		private readonly List<ItemDisplayData> _curSourceTargetList = new List<ItemDisplayData>();

		// Token: 0x04005454 RID: 21588
		private bool _lastIsValid;

		// Token: 0x04005455 RID: 21589
		private readonly Dictionary<sbyte, List<sbyte>> _requiredLifeSkillTypeWithGrade = new Dictionary<sbyte, List<sbyte>>();

		// Token: 0x04005456 RID: 21590
		private ItemDisplayData targetCache;

		// Token: 0x04005457 RID: 21591
		private readonly StringBuilder _sb = new StringBuilder();

		// Token: 0x04005458 RID: 21592
		private readonly Dictionary<ItemSourceType, ItemSourceChange> _slotItemChangeDict = new Dictionary<ItemSourceType, ItemSourceChange>
		{
			{
				ItemSourceType.Inventory,
				new ItemSourceChange(ItemSourceType.Inventory)
			},
			{
				ItemSourceType.Warehouse,
				new ItemSourceChange(ItemSourceType.Warehouse)
			},
			{
				ItemSourceType.Treasury,
				new ItemSourceChange(ItemSourceType.Treasury)
			}
		};

		// Token: 0x04005459 RID: 21593
		private const short EmptyId = -1;
	}
}
