using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Item;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Item.Apply;
using Game.Views.Item;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Story;
using GameData.Domains.Taiwu;
using GameData.Domains.World.Display;
using GameData.GameDataBridge;
using GameData.Serializer;
using GearMate;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract.Zhujian
{
	// Token: 0x020009D1 RID: 2513
	public class ZhujianGearMateSubPageFeature : ZhujianGearMateSubPage
	{
		// Token: 0x17000D82 RID: 3458
		// (get) Token: 0x06007A45 RID: 31301 RVA: 0x0038C934 File Offset: 0x0038AB34
		private ItemSourceType CurItemSourceType
		{
			get
			{
				ZhujianGearMateSubPageFeature.ItemSourceTogKey activeIndex = (ZhujianGearMateSubPageFeature.ItemSourceTogKey)this.toggleGroupItemSource.GetActiveIndex();
				if (!true)
				{
				}
				ItemSourceType result;
				switch (activeIndex)
				{
				case ZhujianGearMateSubPageFeature.ItemSourceTogKey.Inventory:
					result = ItemSourceType.Inventory;
					break;
				case ZhujianGearMateSubPageFeature.ItemSourceTogKey.Warehouse:
					result = ItemSourceType.Warehouse;
					break;
				case ZhujianGearMateSubPageFeature.ItemSourceTogKey.Treasury:
					result = ItemSourceType.Treasury;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				if (!true)
				{
				}
				return result;
			}
		}

		// Token: 0x17000D83 RID: 3459
		// (get) Token: 0x06007A46 RID: 31302 RVA: 0x0038C97C File Offset: 0x0038AB7C
		private ItemListScroll ItemList
		{
			get
			{
				return (this.managedMultiplyItemListScroll != null) ? this.managedMultiplyItemListScroll.CurMultiplyScrollView : null;
			}
		}

		// Token: 0x06007A47 RID: 31303 RVA: 0x0038C99A File Offset: 0x0038AB9A
		public override void Init(ViewZhujianGearMate parent)
		{
			base.Init(parent);
			this.masterLightEffect.SetActive(false);
		}

		// Token: 0x06007A48 RID: 31304 RVA: 0x0038C9B4 File Offset: 0x0038ABB4
		public override void SetGearMateId(int gearMateId)
		{
			bool flag = this.GearMateId == gearMateId;
			if (!flag)
			{
				base.SetGearMateId(gearMateId);
				bool isVisible = this.IsVisible;
				if (isVisible)
				{
					this.RequestGearMateData();
				}
			}
		}

		// Token: 0x06007A49 RID: 31305 RVA: 0x0038C9EC File Offset: 0x0038ABEC
		private void Awake()
		{
			this.buttonConfirm.ClearAndAddListener(new Action(this.OnConfirm));
			this.EnsureMultiplyItemListScrollReference();
			bool flag = this.managedMultiplyItemListScroll != null;
			if (flag)
			{
				this.managedMultiplyItemListScroll.SetupItemListScroll(new ManagedMultiplyItemListScrollSetup
				{
					MainSortSaveKey = "ZhujianGearMateSubPageFeature",
					SelectedSortSaveKey = "ZhujianGearMateSubPageFeature_Selected",
					SortType = ESortAndFilterControllerType.GearMateFeatureEquipment,
					SelectedSortType = new ESortAndFilterControllerType?(ESortAndFilterControllerType.SortOnly),
					EnableRowInteraction = true,
					OnRender = new Action<ITradeableContent, RowItemLine>(this.OnItemRender),
					OnClick = new Action<ITradeableContent, RowItemLine>(this.OnItemClick),
					ColumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Durability),
					AmountGenerator = new Func<ITradeableContent, string>(this.AmountCellDataGenerator)
				});
			}
			this.InitMultiplyItemListScroll();
			this.toggleGroupItemSource.Init(-1);
			this.toggleGroupItemSource.OnActiveIndexChange += this.ToggleGroupItemSourceOnActiveIndexChange;
		}

		// Token: 0x06007A4A RID: 31306 RVA: 0x0038CADC File Offset: 0x0038ACDC
		private void EnsureMultiplyItemListScrollReference()
		{
			bool flag = this.managedMultiplyItemListScroll != null;
			if (!flag)
			{
				this.managedMultiplyItemListScroll = base.GetComponentInChildren<ManagedMultiplyItemListScroll>(true);
			}
		}

		// Token: 0x06007A4B RID: 31307 RVA: 0x0038CB0C File Offset: 0x0038AD0C
		private void InitMultiplyItemListScroll()
		{
			bool flag = this.managedMultiplyItemListScroll == null;
			if (!flag)
			{
				bool flag2 = !this.managedMultiplyItemListScroll.HasInit;
				if (flag2)
				{
					this.managedMultiplyItemListScroll.Init(null);
				}
				this.managedMultiplyItemListScroll.CanSelectItemPredicate = new Func<ItemDisplayData, bool>(ZhujianGearMateSubPageFeature.CanSelectFeatureItem);
				this.managedMultiplyItemListScroll.GetSelectLimitOverride = ((ItemDisplayData _, int defaultLimit) => defaultLimit);
				this.managedMultiplyItemListScroll.SelectAllHandler = new Action(this.OnSelectAllItems);
				this.managedMultiplyItemListScroll.ClearSelectionHandler = delegate()
				{
					this.OnClearAllSelection(false);
				};
				this.managedMultiplyItemListScroll.CanOperateSelection = new Func<bool>(this.CanOperateFeatureSelection);
				this.managedMultiplyItemListScroll.GetSelectedCountForLabel = (() => this._selectedItems.Count);
				this.managedMultiplyItemListScroll.CanEnableSelectAllButton = new Func<bool>(this.HasSelectableFeatureItems);
				bool flag3 = !this.managedMultiplyItemListScroll.IsMultiItemSelect;
				if (flag3)
				{
					this.managedMultiplyItemListScroll.EnterMultiplyMode(false);
				}
				this._multiplyReady = true;
			}
		}

		// Token: 0x06007A4C RID: 31308 RVA: 0x0038CC2A File Offset: 0x0038AE2A
		private bool CanOperateFeatureSelection()
		{
			return this._displayData != null && this._currentFeatureLevelCount < 84;
		}

		// Token: 0x06007A4D RID: 31309 RVA: 0x0038CC41 File Offset: 0x0038AE41
		private static bool CanSelectFeatureItem(ItemDisplayData item)
		{
			return item != null && !item.IsLocked;
		}

		// Token: 0x06007A4E RID: 31310 RVA: 0x0038CC54 File Offset: 0x0038AE54
		private bool HasSelectableFeatureItems()
		{
			bool flag = !this.CanOperateFeatureSelection();
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.FillFilteredSourceItems(this._filteredSourceBuffer);
				for (int i = 0; i < this._filteredSourceBuffer.Count; i++)
				{
					bool flag2 = ZhujianGearMateSubPageFeature.CanSelectFeatureItem(this._filteredSourceBuffer[i]);
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06007A4F RID: 31311 RVA: 0x0038CCBC File Offset: 0x0038AEBC
		private void FillFilteredSourceItems(List<ItemDisplayData> buffer)
		{
			buffer.Clear();
			ManagedMultiplyItemListScroll managedMultiplyItemListScroll = this.managedMultiplyItemListScroll;
			if (managedMultiplyItemListScroll != null)
			{
				managedMultiplyItemListScroll.CollectFilteredSourceItems(buffer);
			}
		}

		// Token: 0x06007A50 RID: 31312 RVA: 0x0038CCDC File Offset: 0x0038AEDC
		private void OnSelectAllItems()
		{
			base.StopDropAnimation();
			this.OnClearAllSelection(true);
			this.FillFilteredSourceItems(this._selectAllWorkList);
			for (int i = 0; i < this._selectAllWorkList.Count; i++)
			{
				ItemDisplayData itemData = this._selectAllWorkList[i];
				bool flag = !ZhujianGearMateSubPageFeature.CanSelectFeatureItem(itemData);
				if (!flag)
				{
					bool usingTypeNeedConfirm = itemData.GetUsingTypeNeedConfirm();
					bool flag2 = usingTypeNeedConfirm;
					if (!flag2)
					{
						bool flag3 = !this._selectedItems.Contains(itemData);
						if (flag3)
						{
							this._selectedItems.Add(itemData);
						}
						this.OnItemChanged(itemData.RealKey, itemData.Amount, true, true, true);
					}
				}
			}
			base.PlayDropAnimation();
			this.SyncMultiplySelection();
		}

		// Token: 0x06007A51 RID: 31313 RVA: 0x0038CD98 File Offset: 0x0038AF98
		private void OnClearAllSelection(bool skipSync = false)
		{
			foreach (ItemDisplayData itemData in this._selectedItems.ToArray())
			{
				this.OnItemChanged(itemData.RealKey, -itemData.Amount, false, false, false);
			}
			this._selectedItems.Clear();
			bool flag = !skipSync;
			if (flag)
			{
				this.SyncMultiplySelection();
			}
		}

		// Token: 0x06007A52 RID: 31314 RVA: 0x0038CDF7 File Offset: 0x0038AFF7
		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.toggleGroupItemSource.OnActiveIndexChange -= this.ToggleGroupItemSourceOnActiveIndexChange;
		}

		// Token: 0x06007A53 RID: 31315 RVA: 0x0038CE1C File Offset: 0x0038B01C
		private void OnItemClick(ITradeableContent content, RowItemLine rowItemLine)
		{
			ZhujianGearMateSubPageFeature.<>c__DisplayClass57_0 CS$<>8__locals1 = new ZhujianGearMateSubPageFeature.<>c__DisplayClass57_0();
			CS$<>8__locals1.content = content;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.itemData = (CS$<>8__locals1.content as ItemDisplayData);
			ItemDisplayData existing = this._selectedItems.Find((ItemDisplayData d) => d.RealKey.Equals(CS$<>8__locals1.content.RealKey));
			bool flag = existing != null;
			if (flag)
			{
				this._selectedItems.Remove(existing);
				this.OnItemChanged(existing.RealKey, -existing.Amount, false, false, true);
				this.SyncMultiplySelection();
			}
			else
			{
				bool usingTypeNeedConfirm = CS$<>8__locals1.itemData.GetUsingTypeNeedConfirm();
				bool flag2 = usingTypeNeedConfirm;
				if (flag2)
				{
					string title = LanguageKey.LK_Common_Attention.Tr();
					string desc = CS$<>8__locals1.itemData.GetUsingOperationConfirmTip(ItemDisplayData.ItemUsingOperationType.Default);
					CommonUtils.ShowConfirmDialog(title, desc, new Action(CS$<>8__locals1.<OnItemClick>g__OnSelect|1), new Action(this.SyncMultiplySelection), EDialogType.None);
				}
				else
				{
					CS$<>8__locals1.<OnItemClick>g__OnSelect|1();
				}
			}
		}

		// Token: 0x06007A54 RID: 31316 RVA: 0x0038CF00 File Offset: 0x0038B100
		private bool IsSelected(ITradeableContent itemData)
		{
			ItemDisplayData displayData = itemData as ItemDisplayData;
			bool flag = displayData == null;
			return !flag && this._selectedItems.Exists((ItemDisplayData d) => d.RealKey.Equals(displayData.RealKey));
		}

		// Token: 0x06007A55 RID: 31317 RVA: 0x0038CF50 File Offset: 0x0038B150
		private void OnItemRender(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, true);
			bool flag = this._multiplyReady && this.managedMultiplyItemListScroll.IsMultiItemSelect;
			if (flag)
			{
				this.managedMultiplyItemListScroll.OnRenderItemMultiply(itemData, rowItemLine);
			}
			else
			{
				bool interactable = !itemData.IsLocked;
				rowItemLine.SetInteractable(interactable, true);
				rowItemLine.SetSelected(this.IsSelected(itemData));
			}
		}

		// Token: 0x06007A56 RID: 31318 RVA: 0x0038CFC4 File Offset: 0x0038B1C4
		private void SyncMultiplySelection()
		{
			bool flag = !this._multiplyReady || this.managedMultiplyItemListScroll == null;
			if (!flag)
			{
				Dictionary<ItemDisplayData, int> dict = this.managedMultiplyItemListScroll.SelectedMultiplyItemDict;
				List<ItemDisplayData> list = this.managedMultiplyItemListScroll.SelectedMultiplyItemOrderedList;
				dict.Clear();
				list.Clear();
				foreach (ItemDisplayData itemData in this._selectedItems)
				{
					dict[itemData] = itemData.Amount;
					list.Add(itemData);
				}
				this.managedMultiplyItemListScroll.SyncSelectedListPanelVisibility();
				bool isBatchSelectionOperation = this.managedMultiplyItemListScroll.IsBatchSelectionOperation;
				if (!isBatchSelectionOperation)
				{
					this.managedMultiplyItemListScroll.RefreshSelectionVisual(new bool?(this.managedMultiplyItemListScroll.IsSelectedListExpanded));
				}
			}
		}

		// Token: 0x06007A57 RID: 31319 RVA: 0x0038D0B0 File Offset: 0x0038B2B0
		private void ToggleGroupItemSourceOnActiveIndexChange(int newIndex, int oldIndex)
		{
			this.RefreshItems();
		}

		// Token: 0x06007A58 RID: 31320 RVA: 0x0038D0BA File Offset: 0x0038B2BA
		protected override void OnShowDataRequest()
		{
			this.toggleGroupItemSource.Set(0, false);
			this.RequestGearMateData();
		}

		// Token: 0x06007A59 RID: 31321 RVA: 0x0038D0D2 File Offset: 0x0038B2D2
		public override void OnListenerIdReady()
		{
			base.OnListenerIdReady();
			this.RequestGearMateData();
		}

		// Token: 0x06007A5A RID: 31322 RVA: 0x0038D0E4 File Offset: 0x0038B2E4
		public override bool CanQuickHide()
		{
			return !this._isPlayingAnim;
		}

		// Token: 0x06007A5B RID: 31323 RVA: 0x0038D100 File Offset: 0x0038B300
		private void RequestGearMateData()
		{
			bool flag = this.GearMateId < 0 || this.ListenerId == 0;
			if (!flag)
			{
				StoryDomainMethod.Call.GetSectZhujianGearMateFeatureDisplayData(this.ListenerId, this.GearMateId);
				UIManager.Instance.HideUI(UIElement.FullScreenMask);
			}
		}

		// Token: 0x06007A5C RID: 31324 RVA: 0x0038D14C File Offset: 0x0038B34C
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				bool flag = notification.Type == 1;
				if (flag)
				{
					bool flag2 = notification.DomainId == 20 && notification.MethodId == 20;
					if (flag2)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._displayData);
						this.Refresh();
						base.SetContentReady();
					}
				}
			}
		}

		// Token: 0x06007A5D RID: 31325 RVA: 0x0038D1F4 File Offset: 0x0038B3F4
		private void Refresh()
		{
			this.textNotice.text = LanguageKey.LK_GearMate_Feature_MaxTip.TrFormat(this.ParentView.GetGearMateName());
			this.toggleGroupItemSource.SetInteractable(this._displayData.CanUseWarehouse, ZhujianGearMateSubPageFeature.ItemSourceTogKey.Warehouse.ToInt());
			this.toggleGroupItemSource.SetInteractable(this._displayData.CanUseWarehouse, ZhujianGearMateSubPageFeature.ItemSourceTogKey.Treasury.ToInt());
			this._selectedItems.Clear();
			this.ResetProcessValue();
			this.RefreshItems();
			this.SyncMultiplySelection();
		}

		// Token: 0x06007A5E RID: 31326 RVA: 0x0038D288 File Offset: 0x0038B488
		private void RefreshItems()
		{
			this._availableItemList.Clear();
			ItemSourceType curItemSourceType = this.CurItemSourceType;
			bool flag = this._displayData.CanUpgradeFeatureItemList != null;
			if (flag)
			{
				foreach (ItemDisplayData itemData in this._displayData.CanUpgradeFeatureItemList)
				{
					bool flag2 = itemData.ItemSourceTypeEnum != curItemSourceType;
					if (!flag2)
					{
						this._availableItemList.Add(itemData);
					}
				}
			}
			bool flag3 = this.managedMultiplyItemListScroll != null;
			if (flag3)
			{
				this.managedMultiplyItemListScroll.SetItems(this._availableItemList);
			}
			else
			{
				ItemListScroll itemList = this.ItemList;
				if (itemList != null)
				{
					itemList.SetItemList(this._availableItemList);
				}
			}
			ItemListScroll itemList2 = this.ItemList;
			if (itemList2 != null)
			{
				SortAndFilterController<ITradeableContent> sortAndFilterController = itemList2.SortAndFilterController;
				if (sortAndFilterController != null)
				{
					sortAndFilterController.NotifyDataChanged(this._availableItemList);
				}
			}
			ItemSourceToggleHelper.RefreshInteractableForInteract(this.toggleGroupItemSource, this._displayData.CanUseWarehouse, false);
			this.SyncMultiplySelection();
		}

		// Token: 0x06007A5F RID: 31327 RVA: 0x0038D3A8 File Offset: 0x0038B5A8
		private void SetFeatureProgressBar()
		{
			this.imageCurProgress.fillAmount = (float)this._currentProcessPercent / 100f;
			this.imagePreviewProgress.fillAmount = (float)this._previewProcessPercent / 100f;
		}

		// Token: 0x06007A60 RID: 31328 RVA: 0x0038D3E0 File Offset: 0x0038B5E0
		public void OnItemChanged(ItemKey itemKey, int amount, bool queueAnim = false, bool isAllSelected = false, bool playItemAnim = true)
		{
			sbyte grade = ItemTemplateHelper.GetGrade(itemKey.ItemType, itemKey.TemplateId);
			this._previewProcessValue += ZhujianGearMateSubPage.CalcGradeProcessValue(grade) * amount;
			int increaseCount;
			int processPercent;
			this.GetIncreaseCountAndProcessValue(out increaseCount, out processPercent, true);
			this._previewProcessPercent = processPercent;
			this._previewFeatureLevelCount = increaseCount + this._currentFeatureLevelCount;
			this._previewFeatureLevelIncreaseCount = increaseCount;
			this.SetFeatureProgressBar();
			int changeCount = this._previewFeatureLevelIncreaseCount - this._currentFeatureLevelIncreaseCount;
			int changePercent = processPercent + changeCount * 100 - this._currentProcessPercent;
			string curStr = string.Format("{0}%", this._currentProcessPercent);
			string changeStr = (changePercent > 0) ? string.Format("+{0}%", changePercent).SetColor("lightblue") : string.Empty;
			this.processPercentText.text = curStr + changeStr;
			this.previewLevelTextValue.text = this._previewFeatureLevelCount.ToString();
			bool hasChange = this._previewProcessValue != this._currentProcessValue;
			this.SetButtonState(hasChange);
			this.ShowPreviewLevel(hasChange);
			bool flag = amount > 0 && playItemAnim;
			if (flag)
			{
				GameObject itemPrefab = Object.Instantiate<GameObject>(this.itemPrefab, this.itemPrefab.transform.parent);
				ItemDrop itemDrop = itemPrefab.GetComponent<ItemDrop>();
				itemDrop.eff_gearmate_zhujian_tubiaozha = this.effGearmateZhujianTubiaozha;
				itemDrop.OnTrigger += delegate()
				{
					this.effGearmateZhujianHuoxing.GetComponent<ParticleSystem>().Play();
					bool flag3 = !queueAnim;
					if (flag3)
					{
						this.SetMachineWaterHeight(1.5f);
					}
				};
				itemPrefab.transform.position = Vector3.Lerp(this.itemPrefabLeftPoint.transform.position, this.itemPrefabRightPoint.transform.position, Random.Range(0f, 1f));
				itemPrefab.GetComponent<CImage>().SetSprite(ItemTemplateHelper.GetIcon(itemKey.ItemType, itemKey.TemplateId), false, null);
				bool queueAnim2 = queueAnim;
				if (queueAnim2)
				{
					base.ItemDrop(itemPrefab);
				}
				else
				{
					itemPrefab.SetActive(true);
				}
			}
			else
			{
				this.SetMachineWaterHeight(0.5f);
			}
			bool flag2 = !isAllSelected;
			if (flag2)
			{
				this.SyncMultiplySelection();
			}
		}

		// Token: 0x06007A61 RID: 31329 RVA: 0x0038D608 File Offset: 0x0038B808
		public void ResetProcessValue()
		{
			this._currentProcessValue = this._displayData.GearMate.FeatureProgress;
			this._currentFeatureLevelCount = 0;
			bool flag = this._displayData.FeatureIds != null;
			if (flag)
			{
				foreach (short featureId in this._displayData.FeatureIds)
				{
					CharacterFeatureItem config = CharacterFeature.Instance[featureId];
					ECharacterFeatureType type = config.Type;
					bool flag2 = type == ECharacterFeatureType.Good || type == ECharacterFeatureType.Bad;
					if (flag2)
					{
						this._currentFeatureLevelCount += (int)config.Level;
					}
				}
			}
			int increaseCount;
			int processPercent;
			this.GetIncreaseCountAndProcessValue(out increaseCount, out processPercent, false);
			this._currentFeatureLevelIncreaseCount = increaseCount;
			this._currentProcessPercent = processPercent;
			this._previewProcessValue = this._currentProcessValue;
			this._previewFeatureLevelCount = this._currentFeatureLevelCount;
			this._previewProcessPercent = this._currentProcessPercent;
			this._previewFeatureLevelIncreaseCount = this._currentFeatureLevelIncreaseCount;
			this.SetFeatureProgressBar();
			this.processPercentText.text = this._currentProcessPercent.ToString() + "%";
			this.curLevelTextValue.text = this._currentFeatureLevelIncreaseCount.ToString();
			this.previewLevelTextValue.text = this._currentFeatureLevelIncreaseCount.ToString();
			this.SetButtonState(false);
			this.ShowPreviewLevel(false);
			this.SetMachineWaterHeight(0.5f);
			this.ShowTip();
		}

		// Token: 0x06007A62 RID: 31330 RVA: 0x0038D798 File Offset: 0x0038B998
		private void GetIncreaseCountAndProcessValue(out int increaseCount, out int processPercent, bool isPreview = false)
		{
			increaseCount = 0;
			int processValue = isPreview ? this._previewProcessValue : this._currentProcessValue;
			while (processValue >= this.GetUpgradeRequirement(increaseCount))
			{
				processValue -= this.GetUpgradeRequirement(increaseCount);
				increaseCount++;
			}
			processPercent = processValue * 100 / this.GetUpgradeRequirement(increaseCount);
		}

		// Token: 0x06007A63 RID: 31331 RVA: 0x0038D7F4 File Offset: 0x0038B9F4
		protected override void SetMachineWaterHeight(float duration = 0.5f)
		{
			ZhujianGearMateSubPageFeature.<>c__DisplayClass73_0 CS$<>8__locals1 = new ZhujianGearMateSubPageFeature.<>c__DisplayClass73_0();
			CS$<>8__locals1.<>4__this = this;
			int value = this._previewProcessValue - this._currentProcessValue;
			CS$<>8__locals1.height = (float)value / (float)(value + ZhujianGearMateSubPage.CalcGradeProcessValue(8));
			CS$<>8__locals1.minHeight = 0.05f;
			bool flag = CS$<>8__locals1.height > 0f && CS$<>8__locals1.height < CS$<>8__locals1.minHeight;
			if (flag)
			{
				CS$<>8__locals1.height = CS$<>8__locals1.minHeight;
			}
			bool showSpark = CS$<>8__locals1.height > 0f;
			bool flag2 = showSpark;
			if (flag2)
			{
				this.masterLightEffect.SetActive(true);
			}
			bool flag3 = this.HeightCoroutine != null;
			if (flag3)
			{
				base.StopCoroutine(this.HeightCoroutine);
			}
			this.HeightCoroutine = base.StartCoroutine(base.ScaleCoroutine(this.handle.transform, duration, new Vector3(Math.Min(CS$<>8__locals1.height, 1f), 1f, 1f), new Action(CS$<>8__locals1.<SetMachineWaterHeight>g__Action|0)));
		}

		// Token: 0x06007A64 RID: 31332 RVA: 0x0038D8F4 File Offset: 0x0038BAF4
		private int GetUpgradeRequirement(int increaseCount = 0)
		{
			return (this._currentFeatureLevelCount + 1 + increaseCount) * 10;
		}

		// Token: 0x06007A65 RID: 31333 RVA: 0x0038D914 File Offset: 0x0038BB14
		private void OnConfirm()
		{
			UIManager.Instance.ShowUI(UIElement.FullScreenMask, true);
			this.SetButtonState(false);
			this.ShowPreviewLevel(false);
			foreach (ItemDisplayData itemData in this._selectedItems)
			{
				ExtraDomainMethod.Call.UpgradeGearMate(this._displayData.GearMateDisplayData.CharacterId, 7, itemData.RealKey, itemData.Amount, itemData.ItemSourceTypeEnum);
			}
			this._selectedItems.Clear();
			this.SyncMultiplySelection();
			this.PlayUpgradeAnim(new Action(this.RequestGearMateData));
		}

		// Token: 0x06007A66 RID: 31334 RVA: 0x0038D9D8 File Offset: 0x0038BBD8
		public void PlayUpgradeAnim(Action onComplete)
		{
			this._isPlayingAnim = true;
			this.ParentView.SetChangeButtonInteractable(false);
			this.gearMateMachine0.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "move", false);
			AudioManager.Instance.PlaySound("SFX_GearMate_machine_loop", false, false);
			bool flag = this.HeightCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this.HeightCoroutine);
			}
			this.HeightCoroutine = base.StartCoroutine(base.ScaleCoroutine(this.handle.transform, 1.13f, new Vector3(0f, 1f, 1f), delegate
			{
				this.masterLightEffect.SetActive(false);
			}));
			float x = this.handle.transform.localScale.x;
			if (!true)
			{
			}
			int num;
			if (x < 0.67f)
			{
				if (x < 0.33f)
				{
					num = 0;
				}
				else
				{
					num = 1;
				}
			}
			else
			{
				num = 2;
			}
			if (!true)
			{
			}
			int jinshuiIndex = num;
			ParticleSystem jinshuiParticle = this.jinshuiGroup.transform.GetChild(jinshuiIndex).GetComponent<ParticleSystem>();
			jinshuiParticle.Play();
			int randomValue = Random.Range(0, 3);
			this.ParentView.ShowBubble(LocalStringManager.Get(LanguageKey.LK_GearMateFeature_SpeakWord0 + randomValue), 1.5f);
			this.ParentView.DoGearMateAnimation("break_1");
			this.imageCurProgress.fillAmount = 0f;
			int count = this._previewFeatureLevelIncreaseCount - this._currentFeatureLevelIncreaseCount;
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.34f, delegate
			{
				this._curPlayEffectCount = 0;
				float duration = 1.16f;
				this.StartCoroutine(this.AnimateAttributeUpgrade(this.imageCurProgress, duration, 0, this._currentProcessValue, count, (float)this._currentProcessPercent / 100f, (float)this._previewProcessPercent / 100f, new Action<int, int>(base.<PlayUpgradeAnim>g__PlaySingleUpdateAnim|2), new Action(base.<PlayUpgradeAnim>g__OnComplete|3)));
			});
		}

		// Token: 0x06007A67 RID: 31335 RVA: 0x0038DB76 File Offset: 0x0038BD76
		private void SetButtonState(bool state)
		{
			this.buttonConfirm.interactable = (state && this._currentFeatureLevelCount < 84);
		}

		// Token: 0x06007A68 RID: 31336 RVA: 0x0038DB95 File Offset: 0x0038BD95
		private void ShowPreviewLevel(bool show)
		{
			this.previewLevelTextValue.gameObject.SetActive(show);
			this.previewLevelArrow.SetActive(show);
		}

		// Token: 0x06007A69 RID: 31337 RVA: 0x0038DBB8 File Offset: 0x0038BDB8
		private void ShowTip()
		{
			TooltipInvoker tipDisplayer = this.tipButtonConfirm;
			tipDisplayer.Type = TipType.MouseTipGearMateUpgradeFeature;
			tipDisplayer.enabled = true;
			TooltipInvoker tooltipInvoker = tipDisplayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			string gearMateName = NameCenter.GetMonasticTitleOrDisplayName(this._displayData.GearMateDisplayData, false);
			for (int i = 0; i < 3; i++)
			{
				tipDisplayer.RuntimeParam.Set(string.Format("Desc{0}", i), LocalStringManager.GetFormat(LanguageKey.UI_MouseTipGearMateUpgradeFeature_Desc0 + i, gearMateName).ColorReplace());
			}
			tipDisplayer.RuntimeParam.Set("Value", LocalStringManager.GetFormat(LanguageKey.UI_MouseTipGearMateUpgradeFeature_Notice, this._currentFeatureLevelIncreaseCount).ColorReplace());
			this.featureNumber.enabled = true;
			tooltipInvoker = this.featureNumber;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			this.featureNumber.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.UI_MouseTipGearMateUpgradeFeature_LevelTip_Title).ColorReplace());
			this.featureNumber.RuntimeParam.Set("arg1", LocalStringManager.GetFormat(LanguageKey.UI_MouseTipGearMateUpgradeFeature_LevelTip_Desc, gearMateName, this._currentFeatureLevelIncreaseCount).ColorReplace());
			this.notice.SetActive(this._currentFeatureLevelCount >= 84);
		}

		// Token: 0x06007A6A RID: 31338 RVA: 0x0038DD14 File Offset: 0x0038BF14
		private string AmountCellDataGenerator(ITradeableContent content)
		{
			ItemDisplayData itemData = content as ItemDisplayData;
			bool flag = itemData == null;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				string maxAmountStr = CommonUtils.GetDisplayStringForNum(itemData.Amount, 100000);
				bool flag2 = !this.IsSelected(itemData);
				if (flag2)
				{
					result = maxAmountStr;
				}
				else
				{
					result = string.Format("{0}/{1}", 1, maxAmountStr);
				}
			}
			return result;
		}

		// Token: 0x04005CA3 RID: 23715
		[SerializeField]
		private TextMeshProUGUI curLevelTextValue;

		// Token: 0x04005CA4 RID: 23716
		[SerializeField]
		private TextMeshProUGUI previewLevelTextValue;

		// Token: 0x04005CA5 RID: 23717
		[SerializeField]
		private CImage imageCurProgress;

		// Token: 0x04005CA6 RID: 23718
		[SerializeField]
		private CImage imagePreviewProgress;

		// Token: 0x04005CA7 RID: 23719
		[SerializeField]
		private GameObject previewLevelArrow;

		// Token: 0x04005CA8 RID: 23720
		[SerializeField]
		private TextMeshProUGUI processPercentText;

		// Token: 0x04005CA9 RID: 23721
		[SerializeField]
		private GameObject gearMateMachine0;

		// Token: 0x04005CAA RID: 23722
		[SerializeField]
		private GameObject handle;

		// Token: 0x04005CAB RID: 23723
		[SerializeField]
		private GameObject effGearmateZhujianHuoxing;

		// Token: 0x04005CAC RID: 23724
		[SerializeField]
		private CButton buttonConfirm;

		// Token: 0x04005CAD RID: 23725
		[SerializeField]
		private GameObject itemPrefab;

		// Token: 0x04005CAE RID: 23726
		[SerializeField]
		private GameObject effGearmateZhujianTubiaozha;

		// Token: 0x04005CAF RID: 23727
		[SerializeField]
		private GameObject itemPrefabLeftPoint;

		// Token: 0x04005CB0 RID: 23728
		[SerializeField]
		private GameObject itemPrefabRightPoint;

		// Token: 0x04005CB1 RID: 23729
		[SerializeField]
		private GameObject jinshuiGroup;

		// Token: 0x04005CB2 RID: 23730
		[SerializeField]
		private TooltipInvoker featureNumber;

		// Token: 0x04005CB3 RID: 23731
		[SerializeField]
		private GameObject notice;

		// Token: 0x04005CB4 RID: 23732
		[SerializeField]
		private TextMeshProUGUI textNotice;

		// Token: 0x04005CB5 RID: 23733
		[SerializeField]
		private ManagedMultiplyItemListScroll managedMultiplyItemListScroll;

		// Token: 0x04005CB6 RID: 23734
		[SerializeField]
		private CToggleGroup toggleGroupItemSource;

		// Token: 0x04005CB7 RID: 23735
		[SerializeField]
		private TooltipInvoker tipButtonConfirm;

		// Token: 0x04005CB8 RID: 23736
		[SerializeField]
		private GameObject masterLightEffect;

		// Token: 0x04005CB9 RID: 23737
		private SectZhujianGearMateFeatureDisplayData _displayData;

		// Token: 0x04005CBA RID: 23738
		private int _currentProcessValue;

		// Token: 0x04005CBB RID: 23739
		private int _currentProcessPercent;

		// Token: 0x04005CBC RID: 23740
		private int _currentFeatureLevelCount;

		// Token: 0x04005CBD RID: 23741
		private int _currentFeatureLevelIncreaseCount;

		// Token: 0x04005CBE RID: 23742
		private int _previewProcessValue;

		// Token: 0x04005CBF RID: 23743
		private int _previewProcessPercent;

		// Token: 0x04005CC0 RID: 23744
		private int _previewFeatureLevelCount;

		// Token: 0x04005CC1 RID: 23745
		private int _previewFeatureLevelIncreaseCount;

		// Token: 0x04005CC2 RID: 23746
		private const int MaxFeatureLevelCount = 84;

		// Token: 0x04005CC3 RID: 23747
		private readonly List<ItemDisplayData> _selectedItems = new List<ItemDisplayData>();

		// Token: 0x04005CC4 RID: 23748
		private readonly List<ItemDisplayData> _availableItemList = new List<ItemDisplayData>();

		// Token: 0x04005CC5 RID: 23749
		private readonly List<ItemDisplayData> _filteredSourceBuffer = new List<ItemDisplayData>();

		// Token: 0x04005CC6 RID: 23750
		private readonly List<ItemDisplayData> _selectAllWorkList = new List<ItemDisplayData>();

		// Token: 0x04005CC7 RID: 23751
		private const int MaxPlayEffectCount = 20;

		// Token: 0x04005CC8 RID: 23752
		private int _curPlayEffectCount;

		// Token: 0x04005CC9 RID: 23753
		private bool _isPlayingAnim;

		// Token: 0x04005CCA RID: 23754
		private bool _multiplyReady;

		// Token: 0x02001F32 RID: 7986
		private enum ItemSourceTogKey
		{
			// Token: 0x0400CCD4 RID: 52436
			Inventory,
			// Token: 0x0400CCD5 RID: 52437
			Warehouse,
			// Token: 0x0400CCD6 RID: 52438
			Treasury
		}
	}
}
