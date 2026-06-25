using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
	// Token: 0x020009D0 RID: 2512
	public class ZhujianGearMateSubPageConsummate : ZhujianGearMateSubPage
	{
		// Token: 0x17000D80 RID: 3456
		// (get) Token: 0x06007A12 RID: 31250 RVA: 0x0038B11C File Offset: 0x0038931C
		private ItemSourceType CurItemSourceType
		{
			get
			{
				ZhujianGearMateSubPageConsummate.ItemSourceTogKey activeIndex = (ZhujianGearMateSubPageConsummate.ItemSourceTogKey)this.toggleGroupItemSource.GetActiveIndex();
				if (!true)
				{
				}
				ItemSourceType result;
				switch (activeIndex)
				{
				case ZhujianGearMateSubPageConsummate.ItemSourceTogKey.Inventory:
					result = ItemSourceType.Inventory;
					break;
				case ZhujianGearMateSubPageConsummate.ItemSourceTogKey.Warehouse:
					result = ItemSourceType.Warehouse;
					break;
				case ZhujianGearMateSubPageConsummate.ItemSourceTogKey.Treasury:
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

		// Token: 0x17000D81 RID: 3457
		// (get) Token: 0x06007A13 RID: 31251 RVA: 0x0038B164 File Offset: 0x00389364
		private ItemListScroll ItemList
		{
			get
			{
				return (this.multiplyItemListScroll != null) ? this.multiplyItemListScroll.CurMultiplyScrollView : null;
			}
		}

		// Token: 0x06007A14 RID: 31252 RVA: 0x0038B182 File Offset: 0x00389382
		public override void Init(ViewZhujianGearMate parent)
		{
			base.Init(parent);
			this.masterLightEffect.SetActive(false);
		}

		// Token: 0x06007A15 RID: 31253 RVA: 0x0038B19C File Offset: 0x0038939C
		private void Awake()
		{
			this.buttonConfirm.ClearAndAddListener(new Action(this.OnConfirm));
			this.EnsureMultiplyItemListScrollReference();
			bool flag = this.multiplyItemListScroll != null;
			if (flag)
			{
				this.multiplyItemListScroll.SetupItemListScroll(new ManagedMultiplyItemListScrollSetup
				{
					MainSortSaveKey = "ZhujianGearMateSubPageConsummate",
					SelectedSortSaveKey = "ZhujianGearMateSubPageConsummate_Selected",
					SortType = ESortAndFilterControllerType.MaterialAsRoot,
					SelectedSortType = new ESortAndFilterControllerType?(ESortAndFilterControllerType.SortOnly),
					EnableRowInteraction = true,
					OnRender = new Action<ITradeableContent, RowItemLine>(this.OnItemRender),
					OnClick = new Action<ITradeableContent, RowItemLine>(this.OnItemClick),
					ColumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount),
					AmountGenerator = new Func<ITradeableContent, string>(this.AmountCellDataGenerator)
				});
			}
			this.InitMultiplyItemListScroll();
			this.toggleGroupItemSource.Init(-1);
			this.toggleGroupItemSource.OnActiveIndexChange += this.ToggleGroupItemSourceOnActiveIndexChange;
		}

		// Token: 0x06007A16 RID: 31254 RVA: 0x0038B28C File Offset: 0x0038948C
		private void EnsureMultiplyItemListScrollReference()
		{
			bool flag = this.multiplyItemListScroll != null;
			if (!flag)
			{
				this.multiplyItemListScroll = base.GetComponentInChildren<ManagedMultiplyItemListScroll>(true);
			}
		}

		// Token: 0x06007A17 RID: 31255 RVA: 0x0038B2BC File Offset: 0x003894BC
		private void InitMultiplyItemListScroll()
		{
			bool flag = this.multiplyItemListScroll == null;
			if (!flag)
			{
				bool flag2 = !this.multiplyItemListScroll.HasInit;
				if (flag2)
				{
					this.multiplyItemListScroll.Init(null);
				}
				this.multiplyItemListScroll.CanSelectItemPredicate = new Func<ItemDisplayData, bool>(this.CanSelectConsummateItem);
				this.multiplyItemListScroll.GetSelectLimitOverride = new Func<ItemDisplayData, int, int>(this.GetConsummateSelectLimit);
				this.multiplyItemListScroll.SelectAllHandler = new Action(this.OnSelectAllItems);
				this.multiplyItemListScroll.ClearSelectionHandler = delegate()
				{
					this.OnClearAllSelection(false);
				};
				this.multiplyItemListScroll.CanOperateSelection = new Func<bool>(this.CanOperateConsummateSelection);
				this.multiplyItemListScroll.GetSelectedCountForLabel = new Func<int>(this.GetSelectedMaterialCount);
				this.multiplyItemListScroll.CanEnableSelectAllButton = new Func<bool>(this.HasSelectableConsummateItems);
				bool flag3 = !this.multiplyItemListScroll.IsMultiItemSelect;
				if (flag3)
				{
					this.multiplyItemListScroll.EnterMultiplyMode(false);
				}
				this._multiplyReady = true;
			}
		}

		// Token: 0x06007A18 RID: 31256 RVA: 0x0038B3C7 File Offset: 0x003895C7
		private bool CanOperateConsummateSelection()
		{
			return this._displayData != null && this._displayData.ConsummateLevel < GlobalConfig.Instance.MaxConsummateLevel;
		}

		// Token: 0x06007A19 RID: 31257 RVA: 0x0038B3EC File Offset: 0x003895EC
		private bool CanSelectConsummateItem(ItemDisplayData item)
		{
			int num;
			return item != null && !item.IsLocked && this.CheckItemInteractable(item, out num);
		}

		// Token: 0x06007A1A RID: 31258 RVA: 0x0038B410 File Offset: 0x00389610
		private int GetConsummateSelectLimit(ItemDisplayData item, int defaultLimit)
		{
			int canSelectCount;
			this.CheckItemInteractable(item, out canSelectCount);
			return Mathf.Min(defaultLimit, canSelectCount);
		}

		// Token: 0x06007A1B RID: 31259 RVA: 0x0038B434 File Offset: 0x00389634
		private int GetSelectedMaterialCount()
		{
			int sum = 0;
			foreach (KeyValuePair<ItemDisplayData, int> keyValuePair in this._selectedMultiplyItemDict)
			{
				ItemDisplayData itemDisplayData;
				int num;
				keyValuePair.Deconstruct(out itemDisplayData, out num);
				int count = num;
				sum += count;
			}
			return sum;
		}

		// Token: 0x06007A1C RID: 31260 RVA: 0x0038B4A0 File Offset: 0x003896A0
		private bool HasSelectableConsummateItems()
		{
			this.FillFilteredSourceItems(this._filteredSourceBuffer);
			for (int i = 0; i < this._filteredSourceBuffer.Count; i++)
			{
				int num;
				bool flag = this.CheckItemInteractable(this._filteredSourceBuffer[i], out num);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06007A1D RID: 31261 RVA: 0x0038B4FA File Offset: 0x003896FA
		private void FillFilteredSourceItems(List<ItemDisplayData> buffer)
		{
			buffer.Clear();
			ManagedMultiplyItemListScroll managedMultiplyItemListScroll = this.multiplyItemListScroll;
			if (managedMultiplyItemListScroll != null)
			{
				managedMultiplyItemListScroll.CollectFilteredSourceItems(buffer);
			}
		}

		// Token: 0x06007A1E RID: 31262 RVA: 0x0038B518 File Offset: 0x00389718
		private void OnSelectAllItems()
		{
			base.StopDropAnimation();
			this.OnClearAllSelection(true);
			this.FillFilteredSourceItems(this._selectAllWorkList);
			for (int i = 0; i < this._selectAllWorkList.Count; i++)
			{
				ItemDisplayData itemData = this._selectAllWorkList[i];
				int canSelectCount;
				bool flag = !this.CheckItemInteractable(itemData, out canSelectCount);
				if (!flag)
				{
					this._selectedMultiplyItemDict[itemData] = canSelectCount;
					this.OnItemChanged(itemData.RealKey, canSelectCount, true, true, true);
				}
			}
			base.PlayDropAnimation();
			this.SyncMultiplySelection();
		}

		// Token: 0x06007A1F RID: 31263 RVA: 0x0038B5AC File Offset: 0x003897AC
		private void OnClearAllSelection(bool skipSync = false)
		{
			foreach (KeyValuePair<ItemDisplayData, int> keyValuePair in this._selectedMultiplyItemDict)
			{
				ItemDisplayData itemDisplayData;
				int num;
				keyValuePair.Deconstruct(out itemDisplayData, out num);
				ItemDisplayData itemData = itemDisplayData;
				int count = num;
				this.OnItemChanged(itemData.RealKey, -count, false, false, false);
			}
			this._selectedMultiplyItemDict.Clear();
			bool flag = !skipSync;
			if (flag)
			{
				this.SyncMultiplySelection();
			}
		}

		// Token: 0x06007A20 RID: 31264 RVA: 0x0038B63C File Offset: 0x0038983C
		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.toggleGroupItemSource.OnActiveIndexChange -= this.ToggleGroupItemSourceOnActiveIndexChange;
		}

		// Token: 0x06007A21 RID: 31265 RVA: 0x0038B660 File Offset: 0x00389860
		private void OnItemClick(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			ItemDisplayData displayData = (ItemDisplayData)itemData;
			bool isSelected = this.IsSelected(itemData);
			bool flag = isSelected;
			if (flag)
			{
				this.SelectItem(displayData, 0);
				this.SyncMultiplySelection();
			}
			else
			{
				bool flag2 = itemData.Amount > 1;
				if (flag2)
				{
					int canSelectCount;
					this.CheckItemInteractable(displayData, out canSelectCount);
					ItemListScroll itemList = this.ItemList;
					if (itemList != null)
					{
						itemList.SetItemToSelectCountMode(rowItemLine, delegate(int count)
						{
							this.SelectItem(displayData, count);
							this.SyncMultiplySelection();
						}, delegate
						{
							this.SyncMultiplySelection();
						}, 0, canSelectCount, 1, null, false, null, false);
					}
				}
				else
				{
					this.SelectItem(displayData, 1);
					this.SyncMultiplySelection();
				}
			}
		}

		// Token: 0x06007A22 RID: 31266 RVA: 0x0038B718 File Offset: 0x00389918
		private void SelectItem(ItemDisplayData itemData, int amount)
		{
			ItemDisplayData existingKey;
			int selectedCount;
			bool flag = !this.TryGetSelectedEntry(itemData, out existingKey, out selectedCount);
			if (flag)
			{
				bool flag2 = amount > 0;
				if (flag2)
				{
					this._selectedMultiplyItemDict[itemData] = amount;
					this.OnItemChanged(itemData.RealKey, amount, false, false, true);
				}
			}
			else
			{
				bool flag3 = amount <= 0;
				if (flag3)
				{
					this._selectedMultiplyItemDict.Remove(existingKey);
				}
				else
				{
					this._selectedMultiplyItemDict[existingKey] = amount;
				}
				this.OnItemChanged(existingKey.RealKey, amount - selectedCount, false, false, true);
			}
		}

		// Token: 0x06007A23 RID: 31267 RVA: 0x0038B7A4 File Offset: 0x003899A4
		private bool IsSelected(ITradeableContent itemData)
		{
			ItemDisplayData displayData = itemData as ItemDisplayData;
			ItemDisplayData itemDisplayData;
			int count;
			return displayData != null && this.TryGetSelectedEntry(displayData, out itemDisplayData, out count) && count > 0;
		}

		// Token: 0x06007A24 RID: 31268 RVA: 0x0038B7D0 File Offset: 0x003899D0
		private bool TryGetSelectedEntry(ItemDisplayData itemData, out ItemDisplayData existingKey, out int count)
		{
			existingKey = null;
			count = 0;
			bool flag = itemData == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				foreach (KeyValuePair<ItemDisplayData, int> keyValuePair in this._selectedMultiplyItemDict)
				{
					ItemDisplayData itemDisplayData;
					int num;
					keyValuePair.Deconstruct(out itemDisplayData, out num);
					ItemDisplayData data = itemDisplayData;
					int selectedCount = num;
					bool flag2 = !data.RealKey.Equals(itemData.RealKey);
					if (!flag2)
					{
						existingKey = data;
						count = selectedCount;
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06007A25 RID: 31269 RVA: 0x0038B878 File Offset: 0x00389A78
		private void OnItemRender(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, true);
			bool flag = this._multiplyReady && this.multiplyItemListScroll.IsMultiItemSelect;
			if (flag)
			{
				this.multiplyItemListScroll.OnRenderItemMultiply(itemData, rowItemLine);
			}
			else
			{
				bool isSelected = this.IsSelected(itemData);
				rowItemLine.SetSelected(isSelected);
				int num;
				bool interactable = isSelected || this.CheckItemInteractable((ItemDisplayData)itemData, out num);
				interactable = (interactable && !itemData.IsLocked);
				rowItemLine.SetInteractable(interactable, true);
				rowItemLine.SetDisabled(!interactable);
			}
		}

		// Token: 0x06007A26 RID: 31270 RVA: 0x0038B914 File Offset: 0x00389B14
		private void SyncMultiplySelection()
		{
			bool flag = !this._multiplyReady || this.multiplyItemListScroll == null;
			if (!flag)
			{
				Dictionary<ItemDisplayData, int> dict = this.multiplyItemListScroll.SelectedMultiplyItemDict;
				List<ItemDisplayData> list = this.multiplyItemListScroll.SelectedMultiplyItemOrderedList;
				dict.Clear();
				list.Clear();
				foreach (KeyValuePair<ItemDisplayData, int> keyValuePair in this._selectedMultiplyItemDict)
				{
					ItemDisplayData itemDisplayData;
					int num;
					keyValuePair.Deconstruct(out itemDisplayData, out num);
					ItemDisplayData itemData = itemDisplayData;
					int count = num;
					bool flag2 = count <= 0;
					if (!flag2)
					{
						dict[itemData] = count;
						list.Add(itemData);
					}
				}
				this.multiplyItemListScroll.SyncSelectedListPanelVisibility();
				bool isBatchSelectionOperation = this.multiplyItemListScroll.IsBatchSelectionOperation;
				if (!isBatchSelectionOperation)
				{
					this.multiplyItemListScroll.RefreshSelectionVisual(new bool?(this.multiplyItemListScroll.IsSelectedListExpanded));
				}
			}
		}

		// Token: 0x06007A27 RID: 31271 RVA: 0x0038BA20 File Offset: 0x00389C20
		private void ToggleGroupItemSourceOnActiveIndexChange(int newIndex, int oldIndex)
		{
			this.RefreshItems();
		}

		// Token: 0x06007A28 RID: 31272 RVA: 0x0038BA2A File Offset: 0x00389C2A
		protected override void OnShowDataRequest()
		{
			this.toggleGroupItemSource.Set(0, false);
			this.RequestGearMateData();
		}

		// Token: 0x06007A29 RID: 31273 RVA: 0x0038BA44 File Offset: 0x00389C44
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

		// Token: 0x06007A2A RID: 31274 RVA: 0x0038BA7C File Offset: 0x00389C7C
		public override void OnListenerIdReady()
		{
			base.OnListenerIdReady();
			this.RequestGearMateData();
		}

		// Token: 0x06007A2B RID: 31275 RVA: 0x0038BA90 File Offset: 0x00389C90
		public override bool CanQuickHide()
		{
			return !this._isPlayingAnim;
		}

		// Token: 0x06007A2C RID: 31276 RVA: 0x0038BAAC File Offset: 0x00389CAC
		private void RequestGearMateData()
		{
			bool flag = this.GearMateId < 0 || this.ListenerId == 0;
			if (!flag)
			{
				StoryDomainMethod.Call.GetSectZhujianGearMateConsummateDisplayData(this.ListenerId, this.GearMateId);
				UIManager.Instance.HideUI(UIElement.FullScreenMask);
			}
		}

		// Token: 0x06007A2D RID: 31277 RVA: 0x0038BAF8 File Offset: 0x00389CF8
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				bool flag = notification.Type == 1;
				if (flag)
				{
					bool flag2 = notification.DomainId == 20 && notification.MethodId == 21;
					if (flag2)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._displayData);
						this.Refresh();
						base.SetContentReady();
					}
				}
			}
		}

		// Token: 0x06007A2E RID: 31278 RVA: 0x0038BBA0 File Offset: 0x00389DA0
		private void Refresh()
		{
			this.textNotice.text = LanguageKey.LK_GearMate_Consummate_MaxTip.TrFormat(this.ParentView.GetGearMateName());
			ItemSourceToggleHelper.RefreshInteractableForInteract(this.toggleGroupItemSource, this._displayData.CanUseWarehouse, false);
			this._selectedMultiplyItemDict.Clear();
			this.ResetProcessValue();
			this.RefreshItems();
			this.SyncMultiplySelection();
		}

		// Token: 0x06007A2F RID: 31279 RVA: 0x0038BC08 File Offset: 0x00389E08
		private void RefreshItems()
		{
			this._availableItemList.Clear();
			ItemSourceType curItemSourceType = this.CurItemSourceType;
			bool flag = this._displayData.CanUpgradeConsummateItemList != null;
			if (flag)
			{
				foreach (ItemDisplayData itemData in this._displayData.CanUpgradeConsummateItemList)
				{
					bool flag2 = itemData.ItemSourceTypeEnum != curItemSourceType;
					if (!flag2)
					{
						this._availableItemList.Add(itemData);
					}
				}
			}
			bool flag3 = this.multiplyItemListScroll != null;
			if (flag3)
			{
				this.multiplyItemListScroll.SetItems(this._availableItemList);
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
			this.SyncMultiplySelection();
		}

		// Token: 0x06007A30 RID: 31280 RVA: 0x0038BD10 File Offset: 0x00389F10
		private void SetConsummateProgressBar()
		{
			this.imageCurProgress.fillAmount = (float)this._currentProgressPercent / 100f;
			this.imagePreviewProgress.fillAmount = (float)this._previewProgressPercent / 100f;
			this.imageCurTotalProgress.fillAmount = (float)this._currentConsummateLevel / 18f;
			this.imagePreviewTotalProgress.fillAmount = (float)this._previewConsummateLevel / 18f;
		}

		// Token: 0x06007A31 RID: 31281 RVA: 0x0038BD84 File Offset: 0x00389F84
		private void ResetProcessValue()
		{
			this._currentConsummateLevel = (int)this._displayData.ConsummateLevel;
			this._currentProcessValue = this._displayData.GearMate.ConsummateLevelProgress;
			int processPercent;
			this.GetIncreaseCountAndProcessValue(this._currentProcessValue, out processPercent);
			this._currentProgressPercent = processPercent;
			this._previewProcessValue = this._currentProcessValue;
			this._previewConsummateLevel = this._currentConsummateLevel;
			this._previewProgressPercent = this._currentProgressPercent;
			this.SetConsummateProgressBar();
			this.SetProgressPercentText();
			this.SetConsummateLevel();
			this.SetButtonState(false);
			this.ShowPreviewLevel(false);
			this.SetMachineWaterHeight(0.5f);
			this.ShowTip();
		}

		// Token: 0x06007A32 RID: 31282 RVA: 0x0038BE2C File Offset: 0x0038A02C
		private void SetProgressPercentText()
		{
			bool flag = this.IsMaxLevel();
			if (flag)
			{
				this.processPercentText.text = LocalStringManager.Get(LanguageKey.LK_GearMate_Consummate_Max).SetColor("PersonalityType_Calm");
			}
			else
			{
				int changeCount = this._previewConsummateLevel - this._currentConsummateLevel;
				int changePercent = this._previewProgressPercent + changeCount * 100 - this._currentProgressPercent;
				string curStr = string.Format("{0}%", this._currentProgressPercent);
				string changeStr = (changePercent > 0) ? string.Format("+{0}%", changePercent) : string.Empty;
				this.processPercentText.text = curStr.SetColor("PersonalityType_Calm") + changeStr.SetColor("FiveElementType_Xuanyin");
			}
		}

		// Token: 0x06007A33 RID: 31283 RVA: 0x0038BEE4 File Offset: 0x0038A0E4
		private void SetConsummateLevel()
		{
			int curLevel = Mathf.Clamp(Mathf.Max(this._currentConsummateLevel - 1, 0) / 2, 0, 8);
			this.imageCurLevel.SetSprite(CommonUtils.GetConsummateIcon((sbyte)curLevel), false, null);
			LanguageKey curKey = LanguageKey.LK_Consummate_Level_0 + curLevel;
			this.textCurLevel.text = string.Format("{0}[{1}]", curKey.Tr(), this._currentConsummateLevel).SetGradeColor(curLevel);
			int previewLevel = Mathf.Clamp(Mathf.Max(this._previewConsummateLevel - 1, 0) / 2, 0, 8);
			this.imagePreviewLevel.SetSprite(CommonUtils.GetConsummateIcon((sbyte)previewLevel), false, null);
			LanguageKey previewKey = LanguageKey.LK_Consummate_Level_0 + previewLevel;
			this.textPreviewLevel.text = string.Format("{0}[{1}]", previewKey.Tr(), this._previewConsummateLevel).SetGradeColor(previewLevel);
		}

		// Token: 0x06007A34 RID: 31284 RVA: 0x0038BFB6 File Offset: 0x0038A1B6
		private void ShowPreviewLevel(bool show)
		{
			this.previewLevelArrow.SetActive(show);
			this.textPreviewLevel.gameObject.SetActive(show);
			this.imagePreviewLevel.gameObject.SetActive(show);
		}

		// Token: 0x06007A35 RID: 31285 RVA: 0x0038BFEC File Offset: 0x0038A1EC
		public void OnItemChanged(ItemKey itemKey, int amount, bool queueAnim = false, bool isAllSelected = false, bool playItemAnim = true)
		{
			sbyte grade = Config.Material.Instance[itemKey.TemplateId].Grade;
			this._previewProcessValue += ZhujianGearMateSubPage.CalcGradeProcessValue(grade) * amount;
			int processPercent;
			int increaseCount = this.GetIncreaseCountAndProcessValue(this._previewProcessValue, out processPercent);
			increaseCount = Math.Min(increaseCount, 18 - this._currentConsummateLevel);
			this._previewProgressPercent = processPercent;
			this._previewConsummateLevel = increaseCount + this._currentConsummateLevel;
			this.SetConsummateProgressBar();
			this.SetProgressPercentText();
			this.SetConsummateLevel();
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
				itemPrefab.GetComponent<CImage>().SetSprite(Config.Material.Instance[itemKey.TemplateId].Icon, false, null);
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
			bool hasChange = this._previewProcessValue != this._currentProcessValue;
			this.SetButtonState(hasChange);
			this.ShowPreviewLevel(hasChange);
			bool flag2 = !isAllSelected;
			if (flag2)
			{
				this.SyncMultiplySelection();
			}
		}

		// Token: 0x06007A36 RID: 31286 RVA: 0x0038C1AC File Offset: 0x0038A3AC
		protected override void SetMachineWaterHeight(float duration = 0.5f)
		{
			ZhujianGearMateSubPageConsummate.<>c__DisplayClass77_0 CS$<>8__locals1 = new ZhujianGearMateSubPageConsummate.<>c__DisplayClass77_0();
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

		// Token: 0x06007A37 RID: 31287 RVA: 0x0038C2AC File Offset: 0x0038A4AC
		private int GetIncreaseCountAndProcessValue(int processValue, out int resultProcessPercent)
		{
			int increaseCount = 0;
			int resultProcessValue = processValue;
			while (resultProcessValue >= this.GetUpgradeRequirement(increaseCount))
			{
				resultProcessValue -= this.GetUpgradeRequirement(increaseCount);
				increaseCount++;
			}
			resultProcessPercent = resultProcessValue * 100 / this.GetUpgradeRequirement(increaseCount);
			return increaseCount;
		}

		// Token: 0x06007A38 RID: 31288 RVA: 0x0038C2F4 File Offset: 0x0038A4F4
		private int GetUpgradeRequirement(int increaseCount = 0)
		{
			int consummateLevel = this._currentConsummateLevel + increaseCount;
			if (!true)
			{
			}
			int result;
			switch (consummateLevel)
			{
			case 0:
				result = 30;
				break;
			case 1:
				result = 60;
				break;
			case 2:
				result = 120;
				break;
			default:
				result = 240 * (consummateLevel - 2);
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06007A39 RID: 31289 RVA: 0x0038C34C File Offset: 0x0038A54C
		private void OnConfirm()
		{
			UIManager.Instance.ShowUI(UIElement.FullScreenMask, true);
			this.SetButtonState(false);
			this.ShowPreviewLevel(false);
			foreach (KeyValuePair<ItemDisplayData, int> keyValuePair in this._selectedMultiplyItemDict)
			{
				ItemDisplayData itemDisplayData;
				int num;
				keyValuePair.Deconstruct(out itemDisplayData, out num);
				ItemDisplayData itemData = itemDisplayData;
				int count = num;
				ExtraDomainMethod.Call.UpgradeGearMate(this._displayData.GearMateDisplayData.CharacterId, 6, itemData.RealKey, count, itemData.ItemSourceTypeEnum);
			}
			this._selectedMultiplyItemDict.Clear();
			this.SyncMultiplySelection();
			this.PlayUpgradeAnim(new Action(this.RequestGearMateData));
		}

		// Token: 0x06007A3A RID: 31290 RVA: 0x0038C41C File Offset: 0x0038A61C
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
			int count = this._previewConsummateLevel - this._currentConsummateLevel;
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.34f, delegate
			{
				float duration = 1.16f;
				this.StartCoroutine(this.AnimateAttributeUpgrade(this.imageCurProgress, duration, 0, this._currentProcessValue, count, (float)this._currentProgressPercent / 100f, (float)this._previewProgressPercent / 100f, new Action<int, int>(ZhujianGearMateSubPageConsummate.<PlayUpgradeAnim>g__PlaySingleUpdateAnim|81_2), new Action(base.<PlayUpgradeAnim>g__OnComplete|3)));
			});
		}

		// Token: 0x06007A3B RID: 31291 RVA: 0x0038C5AC File Offset: 0x0038A7AC
		private void ShowTip()
		{
			TooltipInvoker tip = this.tipButtonConfirm;
			tip.Type = TipType.Simple;
			tip.enabled = true;
			TooltipInvoker tooltipInvoker = tip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			string gearMateName = NameCenter.GetMonasticTitleOrDisplayName(this._displayData.GearMateDisplayData, false);
			tip.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_GearMateConsummate_Title));
			tip.RuntimeParam.Set("arg1", LocalStringManager.GetFormat(LanguageKey.UI_MouseTipGearMateUpgradeConsummate_Desc, gearMateName).ColorReplace());
			this.notice.SetActive(this._currentConsummateLevel >= 18);
		}

		// Token: 0x06007A3C RID: 31292 RVA: 0x0038C64F File Offset: 0x0038A84F
		public void SetButtonState(bool state)
		{
			this.buttonConfirm.interactable = (state && this._currentConsummateLevel < 18);
		}

		// Token: 0x06007A3D RID: 31293 RVA: 0x0038C670 File Offset: 0x0038A870
		public bool CheckItemInteractable(ItemDisplayData itemDisplayData, out int canSelectCount)
		{
			canSelectCount = 0;
			int previewValue = this._previewConsummateLevel;
			bool flag = previewValue >= 18;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				sbyte grade = Config.Material.Instance[itemDisplayData.Key.TemplateId].Grade;
				int addProcessValue = ZhujianGearMateSubPage.CalcGradeProcessValue(grade);
				int previewProcessValue = this._previewProcessValue;
				for (int i = 0; i < itemDisplayData.Amount; i++)
				{
					int num;
					int increaseCount = this.GetIncreaseCountAndProcessValue(previewProcessValue, out num);
					int curValue = this._currentConsummateLevel + increaseCount;
					bool flag2 = curValue < 18;
					if (flag2)
					{
						previewProcessValue += addProcessValue;
						canSelectCount++;
					}
				}
				canSelectCount = Math.Min(itemDisplayData.Amount, canSelectCount);
				result = (canSelectCount > 0);
			}
			return result;
		}

		// Token: 0x06007A3E RID: 31294 RVA: 0x0038C730 File Offset: 0x0038A930
		public void RefreshItemTipNotInteractable(ItemView itemView)
		{
			TooltipInvoker tip = itemView.GetMouseTip();
			tip.enabled = true;
			tip.Type = TipType.SingleDesc;
			string charName = base.GetGearMateName();
			string typeName = LocalStringManager.Get(LanguageKey.LK_Consummate_Level);
			string content = LocalStringManager.GetFormat(LanguageKey.LK_GearMate_UpgradeMaxTip, charName, typeName).SetColor("brightred");
			string[] presetParam = tip.PresetParam;
			bool flag = presetParam == null || presetParam.Length <= 0;
			if (flag)
			{
				tip.PresetParam = new string[1];
			}
			tip.PresetParam[0] = content;
			tip.RuntimeParam = null;
		}

		// Token: 0x06007A3F RID: 31295 RVA: 0x0038C7BC File Offset: 0x0038A9BC
		public bool IsMaxLevel()
		{
			return this._previewConsummateLevel >= 18;
		}

		// Token: 0x06007A40 RID: 31296 RVA: 0x0038C7DC File Offset: 0x0038A9DC
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
				int selectedCount = this.GetSelectedCount(itemData);
				bool isSelected = selectedCount > 0;
				string maxAmountStr = CommonUtils.GetDisplayStringForNum(itemData.Amount, 100000);
				bool flag2 = !isSelected;
				if (flag2)
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

		// Token: 0x06007A41 RID: 31297 RVA: 0x0038C854 File Offset: 0x0038AA54
		private int GetSelectedCount(ItemDisplayData displayData)
		{
			foreach (KeyValuePair<ItemDisplayData, int> keyValuePair in this._selectedMultiplyItemDict)
			{
				ItemDisplayData itemDisplayData;
				int num;
				keyValuePair.Deconstruct(out itemDisplayData, out num);
				ItemDisplayData data = itemDisplayData;
				int count = num;
				bool flag = data.RealKey.Equals(displayData.RealKey);
				if (flag)
				{
					return count;
				}
			}
			return 0;
		}

		// Token: 0x06007A44 RID: 31300 RVA: 0x0038C91F File Offset: 0x0038AB1F
		[CompilerGenerated]
		internal static void <PlayUpgradeAnim>g__PlaySingleUpdateAnim|81_2(int index, int count)
		{
			AudioManager.Instance.PlaySound("SFX_GearMate_machine_up", false, false);
		}

		// Token: 0x04005C7D RID: 23677
		[SerializeField]
		private TextMeshProUGUI processPercentText;

		// Token: 0x04005C7E RID: 23678
		[SerializeField]
		private CImage imageCurProgress;

		// Token: 0x04005C7F RID: 23679
		[SerializeField]
		private CImage imagePreviewProgress;

		// Token: 0x04005C80 RID: 23680
		[SerializeField]
		private GameObject gearMateMachine0;

		// Token: 0x04005C81 RID: 23681
		[SerializeField]
		private GameObject handle;

		// Token: 0x04005C82 RID: 23682
		[SerializeField]
		private GameObject effGearmateZhujianHuoxing;

		// Token: 0x04005C83 RID: 23683
		[SerializeField]
		private CButton buttonConfirm;

		// Token: 0x04005C84 RID: 23684
		[SerializeField]
		private GameObject itemPrefab;

		// Token: 0x04005C85 RID: 23685
		[SerializeField]
		private GameObject effGearmateZhujianTubiaozha;

		// Token: 0x04005C86 RID: 23686
		[SerializeField]
		private GameObject itemPrefabLeftPoint;

		// Token: 0x04005C87 RID: 23687
		[SerializeField]
		private GameObject itemPrefabRightPoint;

		// Token: 0x04005C88 RID: 23688
		[SerializeField]
		private GameObject jinshuiGroup;

		// Token: 0x04005C89 RID: 23689
		[SerializeField]
		private GameObject notice;

		// Token: 0x04005C8A RID: 23690
		[SerializeField]
		private TextMeshProUGUI textNotice;

		// Token: 0x04005C8B RID: 23691
		[SerializeField]
		private TextMeshProUGUI textCurLevel;

		// Token: 0x04005C8C RID: 23692
		[SerializeField]
		private CImage imageCurLevel;

		// Token: 0x04005C8D RID: 23693
		[SerializeField]
		private GameObject previewLevelArrow;

		// Token: 0x04005C8E RID: 23694
		[SerializeField]
		private TextMeshProUGUI textPreviewLevel;

		// Token: 0x04005C8F RID: 23695
		[SerializeField]
		private CImage imagePreviewLevel;

		// Token: 0x04005C90 RID: 23696
		[SerializeField]
		private CImage imageCurTotalProgress;

		// Token: 0x04005C91 RID: 23697
		[SerializeField]
		private CImage imagePreviewTotalProgress;

		// Token: 0x04005C92 RID: 23698
		[SerializeField]
		private ManagedMultiplyItemListScroll multiplyItemListScroll;

		// Token: 0x04005C93 RID: 23699
		[SerializeField]
		private CToggleGroup toggleGroupItemSource;

		// Token: 0x04005C94 RID: 23700
		[SerializeField]
		private TooltipInvoker tipButtonConfirm;

		// Token: 0x04005C95 RID: 23701
		[SerializeField]
		private GameObject masterLightEffect;

		// Token: 0x04005C96 RID: 23702
		private SectZhujianGearMateConsummateDisplayData _displayData;

		// Token: 0x04005C97 RID: 23703
		private int _currentProcessValue;

		// Token: 0x04005C98 RID: 23704
		private int _currentProgressPercent;

		// Token: 0x04005C99 RID: 23705
		private int _currentConsummateLevel;

		// Token: 0x04005C9A RID: 23706
		private int _previewProcessValue;

		// Token: 0x04005C9B RID: 23707
		private int _previewProgressPercent;

		// Token: 0x04005C9C RID: 23708
		private int _previewConsummateLevel;

		// Token: 0x04005C9D RID: 23709
		private readonly List<ItemDisplayData> _availableItemList = new List<ItemDisplayData>();

		// Token: 0x04005C9E RID: 23710
		private readonly List<ItemDisplayData> _filteredSourceBuffer = new List<ItemDisplayData>();

		// Token: 0x04005C9F RID: 23711
		private readonly List<ItemDisplayData> _selectAllWorkList = new List<ItemDisplayData>();

		// Token: 0x04005CA0 RID: 23712
		private readonly Dictionary<ItemDisplayData, int> _selectedMultiplyItemDict = new Dictionary<ItemDisplayData, int>();

		// Token: 0x04005CA1 RID: 23713
		private bool _isPlayingAnim;

		// Token: 0x04005CA2 RID: 23714
		private bool _multiplyReady;

		// Token: 0x02001F2D RID: 7981
		private enum ItemSourceTogKey
		{
			// Token: 0x0400CCC6 RID: 52422
			Inventory,
			// Token: 0x0400CCC7 RID: 52423
			Warehouse,
			// Token: 0x0400CCC8 RID: 52424
			Treasury
		}
	}
}
