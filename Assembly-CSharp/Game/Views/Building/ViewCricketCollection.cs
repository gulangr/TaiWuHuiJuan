using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coffee.UIExtensions;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.Components.EffectPlayer;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter.Item.Apply;
using Game.Views.Cricket;
using Game.Views.Cricket.Combat;
using Game.Views.Select;
using GameData.DLC.CricketPolymorph;
using GameData.Domains.Building;
using GameData.Domains.Building.Display;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Building
{
	// Token: 0x02000BE3 RID: 3043
	public class ViewCricketCollection : UIBase
	{
		// Token: 0x17001062 RID: 4194
		// (get) Token: 0x06009A08 RID: 39432 RVA: 0x00480BB4 File Offset: 0x0047EDB4
		// (set) Token: 0x06009A09 RID: 39433 RVA: 0x00480BBC File Offset: 0x0047EDBC
		private bool IsSelectingItem
		{
			get
			{
				return this._isSelectingItem;
			}
			set
			{
				bool flag = this._isSelectingItem == value;
				if (!flag)
				{
					this._isSelectingItem = value;
					Tweener parallaxTween = this._parallaxTween;
					if (parallaxTween != null)
					{
						parallaxTween.Kill(false);
					}
					Vector2 target = value ? this.openPos : (this._isDlcEnabled ? this.closedDlcPos : this.closedNoDlcPos);
					this.collectionMoveRoot.DOAnchorPos(target, 0.3f, false).SetAutoKill(true);
					float startX = this.parallax.MiddleLayer.anchoredPosition.x;
					float targetX = startX + (value ? this.quickModeBgOffsetX : (-this.quickModeBgOffsetX));
					this._parallaxTween = DOVirtual.Float(0f, 1f, 0.3f, delegate(float t)
					{
						this.parallax.SetMiddleX(Mathf.Lerp(startX, targetX, t));
					}).SetAutoKill(true);
					this.AnimateSelector(value);
					this.displayLeftInfo.DOFade(value ? 0f : 1f, 0.3f);
					this.displayLeftInfo.blocksRaycasts = !value;
					if (value)
					{
						UIManager.Instance.SetEscHandler(delegate
						{
							this.IsSelectingItem = false;
						});
					}
					else
					{
						this.SetEscHandlerForCurrentState();
						foreach (CricketCollectionJar slot in this._slots)
						{
							slot.SetDeselected();
						}
						this._currIndex = -1;
					}
				}
			}
		}

		// Token: 0x06009A0A RID: 39434 RVA: 0x00480D58 File Offset: 0x0047EF58
		public override void OnInit(ArgumentBox argsBox)
		{
		}

		// Token: 0x06009A0B RID: 39435 RVA: 0x00480D5C File Offset: 0x0047EF5C
		private void Awake()
		{
			this.InitSlots();
			this.InitQuickSelector();
			this.InitButtons();
			this._isDlcEnabled = CricketPolymorphHelper.IsCricketPolymorphEnabled;
			bool isDlcEnabled = this._isDlcEnabled;
			if (isDlcEnabled)
			{
				this.InitDlcMode();
			}
			else
			{
				this.InitNoDlcMode();
			}
		}

		// Token: 0x06009A0C RID: 39436 RVA: 0x00480DA4 File Offset: 0x0047EFA4
		private void OnEnable()
		{
			this.RequestData(null);
			GlobalDomainMethod.Call.InvokeGuidingTrigger(125);
			bool flag = !this._isDlcEnabled;
			if (flag)
			{
				this.SetSlotsRect(true);
			}
			else
			{
				this.SetSlotsRect(false);
			}
			this.SwitchImages(this._isDlcEnabled);
		}

		// Token: 0x06009A0D RID: 39437 RVA: 0x00480DF0 File Offset: 0x0047EFF0
		private void SwitchImages(bool isDlcEnabled)
		{
			foreach (CImage noDlcImges in this.imageArrayForNoDlc)
			{
				noDlcImges.enabled = !isDlcEnabled;
			}
		}

		// Token: 0x06009A0E RID: 39438 RVA: 0x00480E24 File Offset: 0x0047F024
		private void RequestData(Action onComplete = null)
		{
			BuildingDomainMethod.AsyncCall.GetCricketCollectionDisplayData(null, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._data);
				CricketCollectionDisplayData data = this._data;
				if (data.Items == null)
				{
					data.Items = new List<ItemDisplayData>();
				}
				data = this._data;
				if (data.AliveCrickets == null)
				{
					data.AliveCrickets = new Dictionary<int, bool>();
				}
				this.Refresh();
				this.RemoveOrphanedCrickets();
				Action onComplete2 = onComplete;
				if (onComplete2 != null)
				{
					onComplete2();
				}
			});
		}

		// Token: 0x06009A0F RID: 39439 RVA: 0x00480E5C File Offset: 0x0047F05C
		private void RemoveOrphanedCrickets()
		{
			for (int i = 0; i < this._data.CollectionCrickets.Length; i++)
			{
				bool hasJar = !this._data.CollectionJars[i].RealKey.Equals(ItemKey.Invalid);
				bool hasCricket = !this._data.CollectionCrickets[i].RealKey.Equals(ItemKey.Invalid);
				bool flag = hasCricket && !hasJar;
				if (flag)
				{
					BuildingDomainMethod.Call.CricketCollectionRemove(i, true);
					this.RequestData(null);
					break;
				}
			}
		}

		// Token: 0x06009A10 RID: 39440 RVA: 0x00480EF8 File Offset: 0x0047F0F8
		protected override void OnClick(Transform btn)
		{
			bool flag = "ButtonCloseView" == btn.name;
			if (flag)
			{
				bool isSelectingItem = this.IsSelectingItem;
				if (isSelectingItem)
				{
					this.OnClickQuick();
				}
				this.QuickHide();
			}
		}

		// Token: 0x06009A11 RID: 39441 RVA: 0x00480F38 File Offset: 0x0047F138
		public override void QuickHide()
		{
			bool isPlayingPerformance = this._isPlayingPerformance;
			if (!isPlayingPerformance)
			{
				bool isViewingMode = this._isViewingMode;
				if (isViewingMode)
				{
					this.ExitViewingMode();
				}
				else
				{
					bool isSelectingItem = this.IsSelectingItem;
					if (isSelectingItem)
					{
						this.OnClickQuick();
					}
					else
					{
						bool isDlcEnabled = this._isDlcEnabled;
						if (isDlcEnabled)
						{
							bool isLeftPanel = this._isLeftPanel;
							if (isLeftPanel)
							{
								this.SwitchToRightPanel();
								return;
							}
							bool isUpgradeMode = this._isUpgradeMode;
							if (isUpgradeMode)
							{
								this.SwitchToDisplayMode();
								return;
							}
						}
						this.ReleaseDlcEscHandler();
						base.QuickHide();
					}
				}
			}
		}

		// Token: 0x06009A12 RID: 39442 RVA: 0x00480FC0 File Offset: 0x0047F1C0
		private void OnDisable()
		{
			this.KillPerformanceTween();
			this._isCallingUpgrade = false;
			bool isPlayingPerformance = this._isPlayingPerformance;
			if (isPlayingPerformance)
			{
				this._isPlayingPerformance = false;
				this.rightUIRoot.SetActive(true);
				this.leftUIRoot.SetActive(false);
				this.displayLeftInfo.gameObject.SetActive(true);
				Vector2 returnPos = this._isLeftPanel ? this.parallax.MiddleAtLeftPanel : this.parallax.MiddleAtRightPanel;
				this.parallax.MiddleLayer.anchoredPosition = returnPos;
			}
			this.TeardownPolymorphPortrait();
			this.KillReturnSpiritTween();
			bool isPlayingReturnSpiritPerformance = this._isPlayingReturnSpiritPerformance;
			if (isPlayingReturnSpiritPerformance)
			{
				this._isPlayingReturnSpiritPerformance = false;
				this.SetEscHandlerForCurrentState();
			}
			bool isViewingMode = this._isViewingMode;
			if (isViewingMode)
			{
				this.backgroundScrollRect.enabled = false;
				this._isViewingMode = false;
			}
			UIElement cricketWishing = UIElement.CricketWishing;
			cricketWishing.OnHide = (Action)Delegate.Remove(cricketWishing.OnHide, new Action(this.OnWishingPanelHide));
			UIElement selectChar = UIElement.SelectChar;
			selectChar.OnHide = (Action)Delegate.Remove(selectChar.OnHide, new Action(this.OnSelectCharHide));
		}

		// Token: 0x06009A13 RID: 39443 RVA: 0x004810E4 File Offset: 0x0047F2E4
		private void InitSlots()
		{
			int index = 0;
			for (int i = 0; i < this.slots.childCount; i++)
			{
				Transform line = this.slots.GetChild(i);
				for (int j = 0; j < line.childCount; j++)
				{
					CricketCollectionJar obj = line.GetChild(j).GetComponent<CricketCollectionJar>();
					obj.Init(this, index++, new Action<int>(this.OnClickJar), new Action<int>(this.OnClickCricket), new Action<int, bool>(this.OnClickCancel));
					this._slots.Add(obj);
				}
			}
		}

		// Token: 0x06009A14 RID: 39444 RVA: 0x0048118C File Offset: 0x0047F38C
		private void InitQuickSelector()
		{
			this.itemSourceTypeToggle.Init(-1);
			this.itemSourceTypeToggle.OnActiveIndexChange += this.OnItemSourceToggleChange;
			this.itemSubTypeToggle.Init(-1);
			this.itemSubTypeToggle.OnActiveIndexChange += this.OnItemTypeChange;
			this.displayModeToggle.Init(0);
			this.displayModeToggle.OnActiveIndexChange += this.OnDisplayModeChange;
		}

		// Token: 0x06009A15 RID: 39445 RVA: 0x0048120C File Offset: 0x0047F40C
		private void InitButtons()
		{
			this.btnRemoveAllJar.ClearAndAddListener(new Action(this.OnClickRemoveAllJar));
			this.btnRemoveAllCricket.ClearAndAddListener(new Action(this.OnClickRemoveAllCricket));
			this.btnAutoJar.ClearAndAddListener(new Action(this.OnClickAutoJar));
			this.btnAutoCricket.ClearAndAddListener(new Action(this.OnClickAutoCricket));
		}

		// Token: 0x06009A16 RID: 39446 RVA: 0x0048127C File Offset: 0x0047F47C
		private void Refresh()
		{
			int count = 0;
			CricketPropertyDisplayMode displayMode = this.GetDisplayMode();
			for (int i = 0; i < this._slots.Count; i++)
			{
				this._slots[i].SetCanUse(this._data.IsInVillage);
				ItemDisplayData jarData = this._data.CollectionJars[i];
				bool flag = jarData.RealKey.Equals(ItemKey.Invalid);
				if (flag)
				{
					this._slots[i].SetEmptyJar();
				}
				else
				{
					this._slots[i].SetJar(jarData.RealKey.TemplateId);
				}
				ItemDisplayData cricketData = this._data.CollectionCrickets[i];
				bool flag2 = cricketData.RealKey.Equals(ItemKey.Invalid);
				if (flag2)
				{
					this._slots[i].SetEmptyCricket();
				}
				else
				{
					this._slots[i].SetCricket(cricketData, this._data.AliveCrickets.ContainsKey(cricketData.RealKey.Id), this._data.CollectionCricketRegen[i] > 0);
					this._slots[i].SetDisplayMode(displayMode);
					count++;
				}
			}
			this.authorityAmount.text = string.Format("+{0}", this._data.AuthorityGain);
			this.cricketAmount.text = LanguageKey.LK_CricketCollection_Title.TrFormat(count, this._slots.Count);
			this.RefreshScroll();
			this.RefreshButtons();
			bool isDlcEnabled = this._isDlcEnabled;
			if (isDlcEnabled)
			{
				this.RefreshDlcDisplay();
			}
			bool flag3 = this._performancePendingLevel >= 0;
			if (flag3)
			{
				CricketCollectionDisplayData data = this._data;
				int? num;
				if (data == null)
				{
					num = null;
				}
				else
				{
					CricketRoomData cricketRoomData = data.CricketRoomData;
					num = ((cricketRoomData != null) ? new int?(cricketRoomData.Level) : null);
				}
				int? num2 = num;
				int currentLevel = num2.GetValueOrDefault();
				bool flag4 = currentLevel > this._performancePendingLevel;
				if (flag4)
				{
					this.PlayUpgradePerformance(this._performancePendingLevel, currentLevel);
				}
				this._performancePendingLevel = -1;
			}
			bool isUpgradeMode = this._isUpgradeMode;
			if (isUpgradeMode)
			{
				this.RefreshUpgradeMode();
			}
		}

		// Token: 0x06009A17 RID: 39447 RVA: 0x004814C4 File Offset: 0x0047F6C4
		private void RefreshScroll()
		{
			int type = this.itemSubTypeToggle.GetActiveIndex();
			this.itemScroll.SetRowTemplate(this.rowTemplates[type]);
			ListStyleGeneralScroll listStyleGeneralScroll = this.itemScroll;
			IEnumerable<ColumnDefinition> currentColumnDefinitions = this.GetCurrentColumnDefinitions();
			bool enableRowInteraction = true;
			Action<int, RowItem> onClick = new Action<int, RowItem>(this.OnClickRow);
			listStyleGeneralScroll.Init<ITradeableContent>(currentColumnDefinitions, enableRowInteraction, new Action<int, GameObject>(this.OnRenderScrollRow), onClick);
			int sourceType = this._itemSourceTypeMap[this.itemSourceTypeToggle.GetActiveIndex()];
			this._filteredItems.Clear();
			foreach (ItemDisplayData item2 in this._data.Items)
			{
				bool flag = (int)item2.ItemSourceType == sourceType && (int)ItemTemplateHelper.GetItemSubType(item2.RealKey.ItemType, item2.RealKey.TemplateId) == this._itemTypeMap[type];
				if (flag)
				{
					this._filteredItems.Add(item2);
				}
			}
			bool flag2 = type == 0;
			if (flag2)
			{
				ItemDisplayData[] collectionCrickets = this._data.CollectionCrickets;
				for (int j = 0; j < collectionCrickets.Length; j++)
				{
					ItemDisplayData item = collectionCrickets[j];
					bool flag3 = item.RealKey.IsValid() && this._filteredItems.FindIndex((ItemDisplayData i) => i.RealKey.Equals(item.RealKey)) < 0;
					if (flag3)
					{
						this._filteredItems.Add(item);
					}
				}
			}
			this._filteredItems.Sort(delegate(ItemDisplayData a, ItemDisplayData b)
			{
				bool aInCollection = this.IsItemInCollection(a);
				bool bInCollection = this.IsItemInCollection(b);
				bool flag7 = aInCollection != bInCollection;
				int result;
				if (flag7)
				{
					result = (aInCollection ? -1 : 1);
				}
				else
				{
					result = 0;
				}
				return result;
			});
			int selectedIndex = -1;
			bool flag4 = this._currIndex >= 0 && type == 0;
			if (flag4)
			{
				ItemKey slotKey = this._data.CollectionCrickets[this._currIndex].RealKey;
				bool flag5 = slotKey.IsValid();
				if (flag5)
				{
					for (int k = 0; k < this._filteredItems.Count; k++)
					{
						bool flag6 = this._filteredItems[k].RealKey.Equals(slotKey);
						if (flag6)
						{
							selectedIndex = k;
							break;
						}
					}
				}
			}
			this.itemScroll.SetData<ItemDisplayData>(this._filteredItems, selectedIndex);
		}

		// Token: 0x06009A18 RID: 39448 RVA: 0x00481724 File Offset: 0x0047F924
		private void OnRenderScrollRow(int index, GameObject rowObject)
		{
			RowItem rowItem = rowObject.GetComponent<RowItem>();
			ItemDisplayData data = this._filteredItems.GetOrDefault(index);
			bool flag = rowItem == null || rowItem.TipDisplayer == null || data == null;
			if (!flag)
			{
				TooltipInvoker displayer = rowItem.TipDisplayer;
				displayer.enabled = true;
				displayer.Type = TooltipManager.ItemTypeToTipType[data.RealKey.ItemType];
				displayer.RuntimeParam = new ArgumentBox().SetObject("ItemData", data.Clone(-1));
				displayer.Refresh(true, -1);
				bool isJarMode = this.itemSubTypeToggle.GetActiveIndex() == 1;
				bool flag2 = isJarMode && this._currIndex >= 0;
				if (flag2)
				{
					ItemKey currentJarKey = this._data.CollectionJars[this._currIndex].RealKey;
					bool isCurrentJar = currentJarKey.IsValid() && currentJarKey.Equals(data.RealKey);
					rowItem.SetInteractable(!isCurrentJar, true);
					rowItem.SetDisabled(isCurrentJar);
				}
				else
				{
					rowItem.SetInteractable(true, true);
					rowItem.SetDisabled(false);
				}
			}
		}

		// Token: 0x06009A19 RID: 39449 RVA: 0x00481847 File Offset: 0x0047FA47
		private void RefreshButtons()
		{
			this.RefreshRemoveAllCricketButton();
			this.RefreshRemoveAllJarButton();
			this.RefreshAutoJarButton();
			this.RefreshAutoCricketButton();
		}

		// Token: 0x06009A1A RID: 39450 RVA: 0x00481868 File Offset: 0x0047FA68
		private void RefreshRemoveAllCricketButton()
		{
			this.btnRemoveAllCricket.interactable = this._data.BatchModeButtonStateData.HasCricketInCollection;
			TooltipInvoker tips = this.btnRemoveAllCricket.GetComponent<TooltipInvoker>();
			TooltipInvoker tooltipInvoker = tips;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			tips.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_CricketCollection_Batch_RemoveAllCricket));
			tips.RuntimeParam.Set("arg1", this.btnRemoveAllCricket.interactable ? LocalStringManager.Get(LanguageKey.LK_CricketCollection_Batch_RemoveAllCricket_Tips) : LocalStringManager.Get(LanguageKey.LK_CricketCollection_Batch_RemoveAllCricket_Tips2));
			tips.Refresh(false, -1);
		}

		// Token: 0x06009A1B RID: 39451 RVA: 0x00481910 File Offset: 0x0047FB10
		private void RefreshRemoveAllJarButton()
		{
			this.btnRemoveAllJar.interactable = this._data.BatchModeButtonStateData.HasJarInCollection;
			TooltipInvoker tips = this.btnRemoveAllJar.GetComponent<TooltipInvoker>();
			TooltipInvoker tooltipInvoker = tips;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			tips.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_CricketCollection_Batch_RemoveAllJar));
			tips.RuntimeParam.Set("arg1", this.btnRemoveAllJar.interactable ? LocalStringManager.Get(LanguageKey.LK_CricketCollection_Batch_RemoveAllJar_Tips) : LocalStringManager.Get(LanguageKey.LK_CricketCollection_Batch_RemoveAllJar_Tips2));
			tips.Refresh(false, -1);
		}

		// Token: 0x06009A1C RID: 39452 RVA: 0x004819B8 File Offset: 0x0047FBB8
		private void RefreshAutoJarButton()
		{
			this.btnAutoJar.interactable = (this._data.BatchModeButtonStateData.HasJarInSources && this._data.BatchModeButtonStateData.HasEmptyPositionInCollection);
			TooltipInvoker tips = this.btnAutoJar.GetComponent<TooltipInvoker>();
			TooltipInvoker tooltipInvoker = tips;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			tips.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_CricketCollection_Batch_AddAllJar));
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(LanguageKey.LK_CricketCollection_Batch_AddAllJar_Tips.Tr());
			bool flag = !this._data.BatchModeButtonStateData.HasJarInSources;
			if (flag)
			{
				sb.AppendLine(this._data.IsInVillage ? LocalStringManager.Get(LanguageKey.LK_CricketCollection_Batch_AddAllJar_Tips2) : LanguageKey.LK_Building_Cannot_Transfer_Warehouse.Tr());
			}
			else
			{
				bool flag2 = !this._data.BatchModeButtonStateData.HasEmptyPositionInCollection;
				if (flag2)
				{
					sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_CricketCollection_Batch_AddAllJar_Tips3));
				}
			}
			tips.RuntimeParam.Set("arg1", sb.ToString());
			tips.Refresh(false, -1);
		}

		// Token: 0x06009A1D RID: 39453 RVA: 0x00481ADC File Offset: 0x0047FCDC
		private void RefreshAutoCricketButton()
		{
			this.btnAutoCricket.interactable = (this._data.BatchModeButtonStateData.HasCricketInSources && this._data.BatchModeButtonStateData.HasEmptyJarInCollection);
			TooltipInvoker tipDisplayer = this.btnAutoCricket.GetComponent<TooltipInvoker>();
			TooltipInvoker tooltipInvoker = tipDisplayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			tipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_CricketCollection_Batch_AddAllCricket));
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(LanguageKey.LK_CricketCollection_Batch_AddAllCricket_Tips.Tr());
			bool flag = !this._data.BatchModeButtonStateData.HasCricketInSources;
			if (flag)
			{
				sb.AppendLine(this._data.IsInVillage ? LocalStringManager.Get(LanguageKey.LK_CricketCollection_Batch_AddAllCricket_Tips2) : LanguageKey.LK_Building_Cannot_Transfer_Warehouse.Tr());
			}
			else
			{
				bool flag2 = !this._data.BatchModeButtonStateData.HasEmptyJarInCollection;
				if (flag2)
				{
					sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_CricketCollection_Batch_AddAllCricket_Tips3));
				}
			}
			tipDisplayer.RuntimeParam.Set("arg1", sb.ToString());
			tipDisplayer.Refresh(false, -1);
		}

		// Token: 0x06009A1E RID: 39454 RVA: 0x00481C00 File Offset: 0x0047FE00
		private void RefreshDlcDisplay()
		{
			CricketRoomData roomData = this._data.CricketRoomData;
			bool flag = roomData == null;
			if (!flag)
			{
				this.roomLevelText.text = LanguageKey.LK_CricketCollection_RoomLevel.TrFormat(roomData.Level);
				int requireExp = GlobalConfig.Instance.CricketRoomRequireExpPerLevel;
				bool isMaxLevel = roomData.Level >= GlobalConfig.Instance.CricketRoomMaxLevel;
				int expInLevel = isMaxLevel ? requireExp : (roomData.Exp % requireExp);
				this.expProgressText.text = LanguageKey.LK_CricketCollection_ExpProgress.TrFormat(expInLevel, requireExp);
				this.expProgressImage.type = Image.Type.Filled;
				this.expProgressImage.fillAmount = (isMaxLevel ? 1f : ((requireExp > 0) ? ((float)expInLevel / (float)requireExp) : 0f));
				ViewCricketCollection.RefreshTip(this.tipExpProgress, LanguageKey.LK_CricketRoom_Tip_ExpProgress_Title.Tr(), LanguageKey.LK_CricketRoom_Tip_ExpProgress_Desc.TrFormat(expInLevel, requireExp));
				int luckCost = GlobalConfig.Instance.CricketWishingCostLuckPoint;
				string luckColor = (this._data.CricketLuckPoint >= luckCost) ? "brightblue" : "brightred";
				this.luckPointText.text = LanguageKey.LK_CricketCollection_LuckPoint.TrFormat(this._data.CricketLuckPoint.ToString().SetColor(luckColor), luckCost);
				this.RefreshBuffList(roomData);
				this.RefreshLockedButtons(roomData);
				this.RefreshLevelGos(roomData.Level);
			}
		}

		// Token: 0x06009A1F RID: 39455 RVA: 0x00481D78 File Offset: 0x0047FF78
		private void RefreshLockedButtons(CricketRoomData roomData)
		{
			bool returnUnlocked = roomData.EnabledPolymorphReturn;
			this.btnReturnSpiritJade.interactable = returnUnlocked;
			TooltipInvoker returnTip = this.btnReturnSpiritJade.GetComponent<TooltipInvoker>();
			bool flag = returnTip != null;
			if (flag)
			{
				returnTip.enabled = !returnUnlocked;
				bool flag2 = !returnUnlocked;
				if (flag2)
				{
					returnTip.Type = TipType.SingleDesc;
					returnTip.RuntimeParam = new ArgumentBox().Set("arg0", LocalStringManager.GetFormat("LK_CricketCollection_ReturnSpiritJadeLocked", GlobalConfig.Instance.CricketRoomPolymorphReturnRequireLevel));
				}
			}
			bool wishUnlocked = roomData.EnabledMakingWish;
			this.btnWishingCricket.interactable = wishUnlocked;
			TooltipInvoker wishTip = this.btnWishingCricket.GetComponent<TooltipInvoker>();
			bool flag3 = wishTip != null;
			if (flag3)
			{
				wishTip.enabled = !wishUnlocked;
				bool flag4 = !wishUnlocked;
				if (flag4)
				{
					wishTip.Type = TipType.SingleDesc;
					wishTip.RuntimeParam = new ArgumentBox().Set("arg0", LocalStringManager.GetFormat("LK_CricketCollection_WishingCricketLocked", GlobalConfig.Instance.CricketRoomMakingWishRequireLevel));
				}
			}
		}

		// Token: 0x06009A20 RID: 39456 RVA: 0x00481E80 File Offset: 0x00480080
		private void RefreshLevelGos(int currentLevel)
		{
			for (int i = 0; i < this.levelGos.Length; i++)
			{
				bool isUnlocked = i < currentLevel;
				bool flag = this.levelGos[i] != null;
				if (flag)
				{
					this.levelGos[i].gameObject.SetActive(isUnlocked);
					bool flag2 = isUnlocked;
					if (flag2)
					{
						CanvasGroup cg = this.levelGos[i].GetComponent<CanvasGroup>();
						bool flag3 = cg != null;
						if (flag3)
						{
							cg.alpha = 1f;
						}
					}
				}
				bool flag4 = this.extraLevelGos != null && i < this.extraLevelGos.Length && this.extraLevelGos[i] != null;
				if (flag4)
				{
					this.extraLevelGos[i].gameObject.SetActive(isUnlocked);
				}
			}
		}

		// Token: 0x06009A21 RID: 39457 RVA: 0x00481F4C File Offset: 0x0048014C
		private void RefreshBuffList(CricketRoomData roomData)
		{
			this.buffReduceAgeText.text = LanguageKey.LK_CricketRoom_Buff_ReduceAge.TrFormat(roomData.ReduceAgeEffect).ColorReplace();
			this.buffAddSpiritText.text = LanguageKey.LK_CricketRoom_Buff_AddSpirit.TrFormat(roomData.AddSpiritEffect).ColorReplace();
			this.buffPolymorphRateText.text = LanguageKey.LK_CricketRoom_Buff_PolymorphRate.TrFormat(roomData.PolymorphRateEffect).ColorReplace();
			this.buffRecoverDurabilityText.text = LanguageKey.LK_CricketRoom_Buff_RecoverDurability.TrFormat(roomData.RecoverDurabilityEffect).ColorReplace();
			this.buffAuthorityText.text = LanguageKey.LK_CricketCollection_BuffAuthority.TrFormat(this._data.AuthorityGain).ColorReplace();
			ViewCricketCollection.RefreshTip(this.tipAuthority, LanguageKey.LK_CricketRoom_Tip_Authority_Title.Tr(), LanguageKey.LK_CricketRoom_Tip_Authority_Desc.TrFormat(this._data.AuthorityGain));
			ViewCricketCollection.RefreshTip(this.tipAddSpirit, LanguageKey.LK_CricketRoom_Tip_AddSpirit_Title.Tr(), LanguageKey.LK_CricketRoom_Tip_AddSpirit_Desc.TrFormat(roomData.AddSpiritEffect));
			this.RefreshReduceAgeTip(roomData.ReduceAgeEffect);
			ViewCricketCollection.RefreshTip(this.tipPolymorphRate, LanguageKey.LK_CricketRoom_Tip_PolymorphRate_Title.Tr(), LanguageKey.LK_CricketRoom_Tip_PolymorphRate_Desc.TrFormat(roomData.PolymorphRateEffect));
			ViewCricketCollection.RefreshTip(this.tipRecoverDurability, LanguageKey.LK_CricketRoom_Tip_RecoverDurability_Title.Tr(), LanguageKey.LK_CricketRoom_Tip_RecoverDurability_Desc.TrFormat(roomData.RecoverDurabilityEffect));
		}

		// Token: 0x06009A22 RID: 39458 RVA: 0x004820DC File Offset: 0x004802DC
		private void RefreshReduceAgeTip(int reduceAgeEffect)
		{
			bool flag = reduceAgeEffect >= 100;
			string content;
			if (flag)
			{
				content = LanguageKey.LK_CricketRoom_Tip_ReduceAge_Max.Tr();
			}
			else
			{
				int agePercent = 100 - reduceAgeEffect;
				int months = Mathf.CeilToInt(1200f / (float)agePercent);
				content = LanguageKey.LK_CricketRoom_Tip_ReduceAge_Desc.TrFormat(reduceAgeEffect, months);
			}
			ViewCricketCollection.RefreshTip(this.tipReduceAge, LanguageKey.LK_CricketRoom_Tip_ReduceAge_Title.Tr(), content);
		}

		// Token: 0x06009A23 RID: 39459 RVA: 0x00482148 File Offset: 0x00480348
		private static void RefreshTip(TooltipInvoker tip, string title, string content)
		{
			bool flag = tip == null;
			if (!flag)
			{
				tip.Type = TipType.Simple;
				if (tip.RuntimeParam == null)
				{
					tip.RuntimeParam = new ArgumentBox();
				}
				tip.RuntimeParam.Set("arg0", title);
				tip.RuntimeParam.Set("arg1", content);
			}
		}

		// Token: 0x06009A24 RID: 39460 RVA: 0x004821A8 File Offset: 0x004803A8
		private void OnClickJar(int index)
		{
			bool flag = this._currIndex >= 0 && this._currIndex != index;
			if (flag)
			{
				this._slots[this._currIndex].SetDeselected();
			}
			this._currIndex = index;
			this.itemSubTypeToggle.Set(1, false);
			this.RefreshScroll();
			bool flag2 = !this.IsSelectingItem;
			if (flag2)
			{
				this.IsSelectingItem = true;
			}
		}

		// Token: 0x06009A25 RID: 39461 RVA: 0x0048221C File Offset: 0x0048041C
		private void OnClickCricket(int index)
		{
			bool flag = this._currIndex >= 0 && this._currIndex != index;
			if (flag)
			{
				this._slots[this._currIndex].SetDeselected();
			}
			this._currIndex = index;
			this.itemSubTypeToggle.Set(0, false);
			this.RefreshScroll();
			bool flag2 = !this.IsSelectingItem;
			if (flag2)
			{
				this.IsSelectingItem = true;
			}
		}

		// Token: 0x06009A26 RID: 39462 RVA: 0x00482290 File Offset: 0x00480490
		private void OnClickCancel(int index, bool isCricket)
		{
			if (isCricket)
			{
				bool flag = this._data.CollectionCrickets[index].RealKey.Equals(ItemKey.Invalid);
				if (flag)
				{
					return;
				}
			}
			else
			{
				bool flag2 = this._data.CollectionJars[index].RealKey.Equals(ItemKey.Invalid);
				if (flag2)
				{
					return;
				}
				bool flag3 = !this._data.CollectionCrickets[index].RealKey.Equals(ItemKey.Invalid);
				if (flag3)
				{
					BuildingDomainMethod.Call.CricketCollectionRemove(index, true);
				}
				AudioManager.Instance.PlaySound("ui_jar_take", false, false);
			}
			BuildingDomainMethod.Call.CricketCollectionRemove(index, isCricket);
			this.RequestData(null);
		}

		// Token: 0x06009A27 RID: 39463 RVA: 0x00482345 File Offset: 0x00480545
		private void OnClickRemoveAllJar()
		{
			BuildingDomainMethod.Call.CricketCollectionBatchRemoveJar(this._data.IsInVillage ? ItemSourceType.Inventory : ItemSourceType.Warehouse);
			this.RequestData(null);
		}

		// Token: 0x06009A28 RID: 39464 RVA: 0x00482367 File Offset: 0x00480567
		private void OnClickRemoveAllCricket()
		{
			BuildingDomainMethod.Call.CricketCollectionBatchRemoveCricket(this._data.IsInVillage ? ItemSourceType.Inventory : ItemSourceType.Warehouse);
			this.RequestData(null);
		}

		// Token: 0x06009A29 RID: 39465 RVA: 0x00482389 File Offset: 0x00480589
		private void OnClickAutoJar()
		{
			BuildingDomainMethod.Call.CricketCollectionBatchAddCricketJar();
			this.RequestData(null);
		}

		// Token: 0x06009A2A RID: 39466 RVA: 0x0048239A File Offset: 0x0048059A
		private void OnClickAutoCricket()
		{
			BuildingDomainMethod.Call.CricketCollectionBatchAddCricket();
			this.RequestData(null);
		}

		// Token: 0x06009A2B RID: 39467 RVA: 0x004823AC File Offset: 0x004805AC
		private void OnClickQuick()
		{
			this.IsSelectingItem = !this.IsSelectingItem;
			bool flag = !this.IsSelectingItem;
			if (flag)
			{
				bool changed = false;
				for (int i = 0; i < this._slots.Count; i++)
				{
					bool flag2 = this._data.CollectionJars[i].RealKey.Equals(ItemKey.Invalid) && !this._data.CollectionCrickets[i].RealKey.Equals(ItemKey.Invalid);
					if (flag2)
					{
						BuildingDomainMethod.Call.CricketCollectionRemove(i, true);
						changed = true;
					}
				}
				this.Deselect();
				bool flag3 = changed;
				if (flag3)
				{
					DialogCmd dialogCmd = new DialogCmd
					{
						Type = 2,
						Title = LocalStringManager.Get(LanguageKey.LK_CricketCollection_AutoRemove_Title),
						Content = LocalStringManager.Get(LanguageKey.LK_CricketCollection_AutoRemove_Content)
					};
					UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
					UIManager.Instance.MaskUI(UIElement.Dialog);
					this.RequestData(null);
				}
			}
		}

		// Token: 0x06009A2C RID: 39468 RVA: 0x004824C6 File Offset: 0x004806C6
		private void OnItemSourceToggleChange(int _, int __)
		{
			this.RefreshScroll();
		}

		// Token: 0x06009A2D RID: 39469 RVA: 0x004824D0 File Offset: 0x004806D0
		private void OnItemTypeChange(int _, int __)
		{
			bool flag = this._data == null;
			if (!flag)
			{
				this.RefreshScroll();
			}
		}

		// Token: 0x06009A2E RID: 39470 RVA: 0x004824F4 File Offset: 0x004806F4
		private void OnDisplayModeChange(int newIndex, int _)
		{
			for (int i = 0; i < this._slots.Count; i++)
			{
				this._slots[i].SetDisplayMode((CricketPropertyDisplayMode)newIndex);
			}
		}

		// Token: 0x06009A2F RID: 39471 RVA: 0x00482534 File Offset: 0x00480734
		private CricketPropertyDisplayMode GetDisplayMode()
		{
			bool flag = !this._isDlcEnabled;
			CricketPropertyDisplayMode result;
			if (flag)
			{
				result = CricketPropertyDisplayMode.Durability;
			}
			else
			{
				int idx = this.displayModeToggle.GetActiveIndex();
				result = (CricketPropertyDisplayMode)((idx >= 0) ? idx : 0);
			}
			return result;
		}

		// Token: 0x06009A30 RID: 39472 RVA: 0x0048256C File Offset: 0x0048076C
		private void OnClickRow(int index, RowItem item)
		{
			bool flag = this._currIndex < 0;
			if (!flag)
			{
				bool isCricket = this.itemSubTypeToggle.GetActiveIndex() == 0;
				bool flag2 = isCricket && this._data.CollectionJars[this._currIndex].RealKey.Equals(ItemKey.Invalid);
				if (!flag2)
				{
					ItemKey key = this._filteredItems[index].RealKey;
					bool flag3 = isCricket;
					if (flag3)
					{
						ItemKey currentKey = this._data.CollectionCrickets[this._currIndex].RealKey;
						bool flag4 = currentKey.IsValid() && currentKey.Equals(key);
						if (flag4)
						{
							BuildingDomainMethod.Call.CricketCollectionRemove(this._currIndex, true);
							this.RequestData(null);
							return;
						}
						int existingSlot = -1;
						for (int i = 0; i < this._data.CollectionCrickets.Length; i++)
						{
							bool flag5 = i != this._currIndex && this._data.CollectionCrickets[i].RealKey.Equals(key);
							if (flag5)
							{
								existingSlot = i;
								break;
							}
						}
						bool targetOccupied = !this._data.CollectionCrickets[this._currIndex].RealKey.Equals(ItemKey.Invalid);
						bool flag6 = existingSlot >= 0 && targetOccupied;
						if (flag6)
						{
							return;
						}
						bool flag7 = targetOccupied;
						if (flag7)
						{
							BuildingDomainMethod.Call.CricketCollectionRemove(this._currIndex, true);
						}
						bool flag8 = existingSlot >= 0;
						if (flag8)
						{
							BuildingDomainMethod.Call.CricketCollectionRemove(existingSlot, true);
						}
						BuildingDomainMethod.Call.CricketCollectionAdd(this._currIndex, true, key);
					}
					else
					{
						bool targetOccupied2 = !this._data.CollectionJars[this._currIndex].RealKey.Equals(ItemKey.Invalid);
						bool flag9 = targetOccupied2;
						if (flag9)
						{
							BuildingDomainMethod.Call.CricketCollectionRemove(this._currIndex, false);
						}
						AudioManager.Instance.PlaySound("ui_jar_put", false, false);
						BuildingDomainMethod.Call.CricketCollectionAdd(this._currIndex, false, key);
					}
					this.RequestData(null);
				}
			}
		}

		// Token: 0x06009A31 RID: 39473 RVA: 0x00482780 File Offset: 0x00480980
		private void Deselect()
		{
			bool flag = this._currIndex >= 0;
			if (flag)
			{
				this._slots[this._currIndex].SetDeselected();
			}
			this._currIndex = -1;
		}

		// Token: 0x06009A32 RID: 39474 RVA: 0x004827BC File Offset: 0x004809BC
		private void AnimateSelector(bool show)
		{
			bool flag = this.itemSelectorHolder == null;
			if (!flag)
			{
				Vector2 target = show ? this.selectorOpenPos : this.selectorClosedPos;
				this.itemSelectorHolder.DOAnchorPos(target, 0.3f, false).SetAutoKill(true);
				bool flag2 = !this._isDlcEnabled;
				if (flag2)
				{
					this.SetSlotsRect(!show);
				}
				else
				{
					this.SetSlotsRect(false);
				}
			}
		}

		// Token: 0x06009A33 RID: 39475 RVA: 0x0048282C File Offset: 0x00480A2C
		private void SetSlotsRect(bool isChange)
		{
			RectTransform rect = this.slots.GetComponent<RectTransform>();
			int x = isChange ? 280 : 0;
			rect.offsetMax = new Vector2((float)x, rect.offsetMax.y);
			rect.offsetMin = new Vector2((float)(-(float)x), rect.offsetMin.y);
		}

		// Token: 0x06009A34 RID: 39476 RVA: 0x00482888 File Offset: 0x00480A88
		private IEnumerable<ColumnDefinition> GetCurrentColumnDefinitions()
		{
			int activeIndex = this.itemSubTypeToggle.GetActiveIndex();
			if (!true)
			{
			}
			IEnumerable<ColumnDefinition> result;
			if (activeIndex != 0)
			{
				if (activeIndex != 1)
				{
					throw new ArgumentOutOfRangeException();
				}
				result = this.GetJarColumnDefinitions();
			}
			else
			{
				result = this.GetCricketColumnDefinitions();
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06009A35 RID: 39477 RVA: 0x004828D0 File Offset: 0x00480AD0
		private bool IsItemInCollection(ItemDisplayData item)
		{
			ItemKey key = item.RealKey;
			foreach (ItemDisplayData jar in this._data.CollectionJars)
			{
				bool flag = jar.RealKey.Equals(key);
				if (flag)
				{
					return true;
				}
			}
			foreach (ItemDisplayData cricket in this._data.CollectionCrickets)
			{
				bool flag2 = cricket.RealKey.Equals(key);
				if (flag2)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06009A36 RID: 39478 RVA: 0x0048296A File Offset: 0x00480B6A
		private IEnumerable<ColumnDefinition> GetCricketColumnDefinitions()
		{
			ColumnDefinition<ItemDisplayData, CricketCollectionFirstCellData> columnDefinition = new ColumnDefinition<ItemDisplayData, CricketCollectionFirstCellData>();
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_ItemName.Tr());
			columnDefinition.LayoutOption = new LayoutOption
			{
				PreferredWidth = 300f,
				FlexibleWidth = 1f
			};
			columnDefinition.CellDataGenerator = ((ItemDisplayData item) => new CricketCollectionFirstCellData
			{
				ItemData = item,
				IsInCollection = this.IsItemInCollection(item)
			});
			yield return columnDefinition;
			ColumnDefinition<ItemDisplayData, string> columnDefinition2 = new ColumnDefinition<ItemDisplayData, string>();
			columnDefinition2.LayoutOption = new LayoutOption
			{
				FlexibleWidth = 1f,
				PreferredWidth = 150f
			};
			columnDefinition2.TableHeadLabel = (() => LanguageKey.LK_Durability.Tr());
			columnDefinition2.CellDataGenerator = ((ItemDisplayData data) => string.Format("{0}/{1}", data.Durability, data.MaxDurability));
			columnDefinition2.SortId = 18;
			yield return columnDefinition2;
			ColumnDefinition<ItemDisplayData, string> columnDefinition3 = new ColumnDefinition<ItemDisplayData, string>();
			columnDefinition3.LayoutOption = new LayoutOption
			{
				FlexibleWidth = 1f,
				PreferredWidth = 150f
			};
			columnDefinition3.TableHeadLabel = (() => LanguageKey.LK_SelectItem_Column_CricketWins.Tr());
			columnDefinition3.CellDataGenerator = ((ItemDisplayData data) => string.Format("{0}", data.CricketData.WinsCount));
			columnDefinition3.SortId = 44;
			yield return columnDefinition3;
			yield break;
		}

		// Token: 0x06009A37 RID: 39479 RVA: 0x0048297C File Offset: 0x00480B7C
		private void InitNoDlcMode()
		{
			this.noDlcBackground.SetActive(true);
			this.dlcBackground.SetActive(false);
			this.backgroundScrollRect.enabled = false;
			this.backgroundScrollRect.gameObject.SetActive(false);
			this.leftUIRoot.SetActive(false);
			this.upgradeModeRoot.SetActive(false);
			this.displayModeToggle.gameObject.SetActive(false);
			this.displayLeftInfo.gameObject.SetActive(false);
			this.collectionMoveRoot.anchoredPosition = this.closedNoDlcPos;
			this._isLeftPanel = false;
			this._isUpgradeMode = false;
			CImage moveRootImage;
			bool flag = this.collectionMoveRoot.TryGetComponent<CImage>(out moveRootImage);
			if (flag)
			{
				moveRootImage.enabled = false;
			}
			this.amountRoot.anchoredPosition = this.amountRootNoDlcPos;
			bool flag2 = this.amountRootImage != null;
			if (flag2)
			{
				this.amountRootImage.enabled = false;
			}
			this.buttonsRoot.SetParent(this.collectionMoveRoot.parent, true);
			this.buttonsRoot.SetAsLastSibling();
			this.buttonsRoot.anchoredPosition = this.buttonsRootNoDlcPos;
			this.buttonsRoot.sizeDelta = this.buttonsRootNoDlcSize;
			bool flag3 = this.itemSelectorRect != null;
			if (flag3)
			{
				this.itemSelectorRect.sizeDelta = new Vector2(this.itemSelectorRect.sizeDelta.x, this.itemSelectorHeightNoDlc);
			}
		}

		// Token: 0x06009A38 RID: 39480 RVA: 0x00482AE8 File Offset: 0x00480CE8
		private void InitDlcMode()
		{
			this.noDlcBackground.SetActive(false);
			this.dlcBackground.SetActive(true);
			this.noDlcAuthorityRoot.SetActive(false);
			this.displayModeToggle.gameObject.SetActive(true);
			this.backgroundScrollRect.gameObject.SetActive(true);
			this.collectionRoot.SetActive(true);
			this.collectionCanvasGroup.alpha = 1f;
			this.collectionCanvasGroup.blocksRaycasts = true;
			this.upgradeModeRoot.SetActive(false);
			this.leftUIRoot.SetActive(false);
			this.leftUICanvasGroup.alpha = 0f;
			this._isLeftPanel = false;
			this._isUpgradeMode = false;
			this.btnUpgradeRoom.ClearAndAddListener(new Action(this.OnClickUpgradeRoom));
			this.btnWishingCricket.ClearAndAddListener(new Action(this.OnClickWishingCricket));
			this.btnReturnSpiritJade.ClearAndAddListener(new Action(this.OnClickReturnSpiritJade));
			this.btnAddCharacter.ClearAndAddListener(new Action(this.OnClickAddCharacter));
			this.btnConfirmReturn.ClearAndAddListener(new Action(this.OnClickConfirmReturn));
			this.btnBackFromUpgrade.ClearAndAddListener(new Action(this.SwitchToDisplayMode));
			this.btnBackFromReturnSpirit.ClearAndAddListener(new Action(this.SwitchToRightPanel));
			this.btnViewCharacter.ClearAndAddListener(new Action(this.OnClickViewCharacter));
			this.btnChangeCharacter.ClearAndAddListener(new Action(this.OnClickAddCharacter));
			this.btnViewingMode.ClearAndAddListener(new Action(this.ToggleViewingMode));
			this.backgroundScrollRect.enabled = false;
			this.parallax.SnapToRightPanel();
			this.displayLeftInfo.gameObject.SetActive(true);
			this.SetEscHandlerForCurrentState();
			this.amountRoot.anchoredPosition = this.amountRootDlcPos;
			bool flag = this.amountRootImage != null;
			if (flag)
			{
				this.amountRootImage.enabled = true;
			}
			this.buttonsRoot.anchoredPosition = this.buttonsRootDlcPos;
			this.buttonsRoot.sizeDelta = this.buttonsRootDlcSize;
			this.buttonsRoot.SetParent(this.collectionMoveRoot, true);
			this.InitUpgradeMode();
			this.HidePreview();
			this.selectedCharRoot.SetActive(false);
			this.btnChangeCharacter.gameObject.SetActive(false);
			CImage moveRootImage;
			bool flag2 = this.collectionMoveRoot.TryGetComponent<CImage>(out moveRootImage);
			if (flag2)
			{
				moveRootImage.enabled = true;
			}
			bool flag3 = this.itemSelectorRect != null;
			if (flag3)
			{
				this.itemSelectorRect.sizeDelta = new Vector2(this.itemSelectorRect.sizeDelta.x, this.itemSelectorHeightDlc);
			}
		}

		// Token: 0x06009A39 RID: 39481 RVA: 0x00482DAC File Offset: 0x00480FAC
		private void InitUpgradeMode()
		{
			this.materialScroll.Init("CricketCollection_UpgradeMaterial", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnRenderUpgradeMaterial), new Action<ITradeableContent, RowItemLine>(this.OnClickUpgradeMaterial), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount, null, null, null);
			this.investedMaterialScroll.OnItemRender += this.OnRenderInvestedMaterial;
			this.materialSourceToggle.Init(-1);
			this.materialSourceToggle.OnActiveIndexChange += this.OnMaterialSourceToggleChange;
		}

		// Token: 0x06009A3A RID: 39482 RVA: 0x00482E28 File Offset: 0x00481028
		private void AnimateCameraTo(bool goingLeft)
		{
			Tweener parallaxTween = this._parallaxTween;
			if (parallaxTween != null)
			{
				parallaxTween.Kill(false);
			}
			float targetRightAlpha = goingLeft ? 0f : 1f;
			float targetLeftAlpha = goingLeft ? 1f : 0f;
			bool goingLeft2 = goingLeft;
			if (goingLeft2)
			{
				this.leftUIRoot.SetActive(true);
			}
			else
			{
				this.rightUIRoot.SetActive(true);
			}
			this.rightUICanvasGroup.blocksRaycasts = (targetRightAlpha > 0.5f);
			this.rightUICanvasGroup.DOKill(false);
			this.rightUICanvasGroup.DOFade(targetRightAlpha, this.panelSwitchDuration);
			this.leftUICanvasGroup.blocksRaycasts = (targetLeftAlpha > 0.5f);
			this.leftUICanvasGroup.DOKill(false);
			this.leftUICanvasGroup.DOFade(targetLeftAlpha, this.panelSwitchDuration).OnUpdate(delegate
			{
				bool flag = !goingLeft && this.cricketHolder != null && !this.cricketHolder.gameObject.activeSelf && this.leftUICanvasGroup.alpha <= 0.5f;
				if (flag)
				{
					this.cricketHolder.gameObject.SetActive(true);
				}
			}).OnComplete(delegate
			{
				bool goingLeft3 = goingLeft;
				if (goingLeft3)
				{
					this.rightUIRoot.SetActive(false);
				}
				else
				{
					this.leftUIRoot.SetActive(false);
				}
			});
			Vector2 startPos = this.parallax.MiddleLayer.anchoredPosition;
			Vector2 targetPos = goingLeft ? this.parallax.MiddleAtLeftPanel : this.parallax.MiddleAtRightPanel;
			this._parallaxTween = DOTween.To(() => 0f, delegate(float t)
			{
				this.parallax.MiddleLayer.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
			}, 1f, this.panelSwitchDuration).SetAutoKill(true);
		}

		// Token: 0x06009A3B RID: 39483 RVA: 0x00482FBC File Offset: 0x004811BC
		private void ToggleViewingMode()
		{
			bool isViewingMode = this._isViewingMode;
			if (isViewingMode)
			{
				this.ExitViewingMode();
			}
			else
			{
				this.EnterViewingMode();
			}
		}

		// Token: 0x06009A3C RID: 39484 RVA: 0x00482FE4 File Offset: 0x004811E4
		private void EnterViewingMode()
		{
			bool isViewingMode = this._isViewingMode;
			if (!isViewingMode)
			{
				this._isViewingMode = true;
				bool isDlcEnabled = this._isDlcEnabled;
				if (isDlcEnabled)
				{
					this.dlcBackground.SetActive(false);
				}
				bool isSelectingItem = this.IsSelectingItem;
				if (isSelectingItem)
				{
					this._isSelectingItem = false;
					this.collectionMoveRoot.anchoredPosition = (this._isDlcEnabled ? this.closedDlcPos : this.closedNoDlcPos);
					this.AnimateSelector(false);
					this.Deselect();
				}
				Tweener parallaxTween = this._parallaxTween;
				if (parallaxTween != null)
				{
					parallaxTween.Kill(false);
				}
				this.backgroundScrollRect.enabled = true;
				this.rightUIRoot.SetActive(false);
				this.leftUIRoot.SetActive(false);
				this.displayLeftInfo.gameObject.SetActive(false);
				UIManager.Instance.SetEscHandler(new Action(this.ExitViewingMode));
			}
		}

		// Token: 0x06009A3D RID: 39485 RVA: 0x004830C8 File Offset: 0x004812C8
		private void ExitViewingMode()
		{
			bool flag = !this._isViewingMode;
			if (!flag)
			{
				this._isViewingMode = false;
				bool isDlcEnabled = this._isDlcEnabled;
				if (isDlcEnabled)
				{
					this.dlcBackground.SetActive(true);
				}
				this.backgroundScrollRect.enabled = false;
				Tweener parallaxTween = this._parallaxTween;
				if (parallaxTween != null)
				{
					parallaxTween.Kill(false);
				}
				float startX = this.parallax.MiddleLayer.anchoredPosition.x;
				Vector2 targetPos = this._isLeftPanel ? this.parallax.MiddleAtLeftPanel : this.parallax.MiddleAtRightPanel;
				this._parallaxTween = DOTween.To(() => 0f, delegate(float t)
				{
					this.parallax.MiddleLayer.anchoredPosition = Vector2.Lerp(new Vector2(startX, this.parallax.MiddleLayer.anchoredPosition.y), targetPos, t);
				}, 1f, 0.3f).SetAutoKill(true);
				this.rightUIRoot.SetActive(!this._isLeftPanel);
				this.leftUIRoot.SetActive(this._isLeftPanel);
				this.displayLeftInfo.gameObject.SetActive(true);
				this.SetEscHandlerForCurrentState();
			}
		}

		// Token: 0x06009A3E RID: 39486 RVA: 0x004831FC File Offset: 0x004813FC
		private void PlayUpgradePerformance(int oldLevel, int newLevel)
		{
			bool flag = this.levelGos == null || this.levelGos.Length == 0 || this._isPlayingPerformance;
			if (!flag)
			{
				int targetIdx = Mathf.Min(newLevel - 1, this.levelGos.Length - 1);
				Transform targetGoTransform = this.levelGos[targetIdx];
				bool flag2 = targetGoTransform == null;
				if (!flag2)
				{
					RectTransform targetGo = targetGoTransform.GetComponent<RectTransform>();
					bool flag3 = targetGo == null;
					if (!flag3)
					{
						this._isPlayingPerformance = true;
						this.KillPerformanceTween();
						bool wasLeftPanel = this._isLeftPanel;
						Vector2 startMiddlePos = this.parallax.MiddleLayer.anchoredPosition;
						Vector2 targetPos = this.parallax.CalculateCenteredPosition(targetGo, this.backgroundScrollRect.viewport);
						int firstNewLevel = oldLevel + 1;
						for (int i = firstNewLevel - 1; i < newLevel; i++)
						{
							bool flag4 = i < 0 || i >= this.levelGos.Length || this.levelGos[i] == null;
							if (!flag4)
							{
								this.levelGos[i].gameObject.SetActive(true);
								CanvasGroup cg = this.levelGos[i].GetComponent<CanvasGroup>();
								bool flag5 = cg == null;
								if (flag5)
								{
									cg = this.levelGos[i].gameObject.AddComponent<CanvasGroup>();
								}
								cg.alpha = 0f;
								bool flag6 = this.extraLevelGos != null && i < this.extraLevelGos.Length && this.extraLevelGos[i] != null;
								if (flag6)
								{
									this.extraLevelGos[i].gameObject.SetActive(true);
								}
							}
						}
						bool isSelectingItem = this.IsSelectingItem;
						if (isSelectingItem)
						{
							this._isSelectingItem = false;
							this.collectionMoveRoot.anchoredPosition = (this._isDlcEnabled ? this.closedDlcPos : this.closedNoDlcPos);
							this.AnimateSelector(false);
							this.Deselect();
						}
						Tweener parallaxTween = this._parallaxTween;
						if (parallaxTween != null)
						{
							parallaxTween.Kill(false);
						}
						this._performanceSequence = DOTween.Sequence();
						this.rightUIRoot.SetActive(false);
						this.leftUIRoot.SetActive(false);
						this.displayLeftInfo.gameObject.SetActive(false);
						this._performanceSequence.Append(DOVirtual.Float(0f, 1f, 0.43f, delegate(float t)
						{
							Vector2 pos = this.parallax.MiddleLayer.anchoredPosition;
							float newX = Mathf.Lerp(startMiddlePos.x, targetPos.x, t);
							pos.x = newX;
							this.parallax.MiddleLayer.anchoredPosition = pos;
						}));
						this._performanceSequence.AppendCallback(delegate
						{
							for (int j = firstNewLevel - 1; j < newLevel; j++)
							{
								bool flag7 = j < 0 || j >= this.levelGos.Length || this.levelGos[j] == null;
								if (!flag7)
								{
									CanvasGroup cg2 = this.levelGos[j].GetComponent<CanvasGroup>();
									bool flag8 = cg2 != null;
									if (flag8)
									{
										cg2.DOFade(1f, 1.71f);
									}
								}
							}
						});
						this._performanceSequence.AppendInterval(1.71f);
						float returnX = wasLeftPanel ? this.parallax.MiddleAtLeftPanel.x : this.parallax.MiddleAtRightPanel.x;
						this._performanceSequence.Append(DOVirtual.Float(0f, 1f, 0.43f, delegate(float t)
						{
							Vector2 pos = this.parallax.MiddleLayer.anchoredPosition;
							pos.x = Mathf.Lerp(targetPos.x, returnX, t);
							this.parallax.MiddleLayer.anchoredPosition = pos;
						}));
						this._performanceSequence.AppendCallback(delegate
						{
							this.rightUIRoot.SetActive(!wasLeftPanel);
							this.rightUICanvasGroup.alpha = (wasLeftPanel ? 0f : 1f);
							this.rightUICanvasGroup.blocksRaycasts = !wasLeftPanel;
							this.leftUIRoot.SetActive(wasLeftPanel);
							this.leftUICanvasGroup.alpha = (wasLeftPanel ? 1f : 0f);
							this.leftUICanvasGroup.blocksRaycasts = wasLeftPanel;
							this.displayLeftInfo.gameObject.SetActive(true);
							this.SetEscHandlerForCurrentState();
							this._isPlayingPerformance = false;
							this._performanceSequence = null;
							this.PlayPostUpgradeEffects();
						});
					}
				}
			}
		}

		// Token: 0x06009A3F RID: 39487 RVA: 0x0048352C File Offset: 0x0048172C
		private void KillPerformanceTween()
		{
			bool flag = this._performanceSequence != null;
			if (flag)
			{
				this._performanceSequence.Kill(false);
				this._performanceSequence = null;
			}
		}

		// Token: 0x06009A40 RID: 39488 RVA: 0x00483560 File Offset: 0x00481760
		private void PlayPostUpgradeEffects()
		{
			bool flag = this.fxUpgradeRoom != null;
			if (flag)
			{
				this._particleHelper.PlayOnceParticle(this.fxUpgradeRoom, 1f, null);
			}
			CricketCollectionDisplayData data = this._data;
			CricketRoomData roomData = (data != null) ? data.CricketRoomData : null;
			bool flag2 = roomData != null;
			if (flag2)
			{
				bool flag3 = !this._preUpgradeReturnUnlocked && roomData.EnabledPolymorphReturn;
				if (flag3)
				{
					ViewNewFunctionUnlock.Queue.Enqueue(26);
				}
				bool flag4 = !this._preUpgradeWishingUnlocked && roomData.EnabledMakingWish;
				if (flag4)
				{
					ViewNewFunctionUnlock.Queue.Enqueue(27);
				}
				bool flag5 = ViewNewFunctionUnlock.Queue.Count > 0;
				if (flag5)
				{
					UIManager.Instance.ShowUI(UIElement.NewFunctionUnlock, true);
				}
			}
		}

		// Token: 0x06009A41 RID: 39489 RVA: 0x00483620 File Offset: 0x00481820
		private void PlayReturnSpiritPerformance(ItemDisplayData cricketItem)
		{
			this.KillReturnSpiritTween();
			this._isPlayingReturnSpiritPerformance = true;
			bool isLeftPanel = this._isLeftPanel;
			if (isLeftPanel)
			{
				UIManager.Instance.SetEscHandler(null);
			}
			this._returnSpiritSequence = DOTween.Sequence();
			this._returnSpiritSequence.AppendCallback(delegate
			{
				AudioManager.Instance.PlaySound("CCricket_BackStone", false, false);
				this.SetupPolymorphPortrait();
				this._particleHelper.PlayOnceParticle(this.fxReturnSpiritSuccess, 2.5f, null);
			});
			this._returnSpiritSequence.AppendInterval(2.5f);
			this._returnSpiritSequence.OnComplete(delegate
			{
				this.TeardownPolymorphPortrait();
				this._returnSpiritSequence = null;
				this._isPlayingReturnSpiritPerformance = false;
				this.SetEscHandlerForCurrentState();
				this.ResetReturnSpiritSelection();
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.SetObject("ItemList", new List<ItemDisplayData>
				{
					cricketItem
				});
				UIElement.GetItem.SetOnInitArgs(argBox);
				UIElement getItem = UIElement.GetItem;
				getItem.OnHide = (Action)Delegate.Remove(getItem.OnHide, new Action(this.OnReturnSpiritObtainHide));
				UIElement getItem2 = UIElement.GetItem;
				getItem2.OnHide = (Action)Delegate.Combine(getItem2.OnHide, new Action(this.OnReturnSpiritObtainHide));
				UIManager.Instance.MaskUI(UIElement.GetItem);
			});
		}

		// Token: 0x06009A42 RID: 39490 RVA: 0x004836B2 File Offset: 0x004818B2
		private void OnReturnSpiritObtainHide()
		{
			UIElement getItem = UIElement.GetItem;
			getItem.OnHide = (Action)Delegate.Remove(getItem.OnHide, new Action(this.OnReturnSpiritObtainHide));
			this.RequestData(null);
			this.RefreshReturnSpiritAddButton();
		}

		// Token: 0x06009A43 RID: 39491 RVA: 0x004836EC File Offset: 0x004818EC
		private void KillReturnSpiritTween()
		{
			bool flag = this._returnSpiritSequence != null;
			if (flag)
			{
				this._returnSpiritSequence.Kill(false);
				this._returnSpiritSequence = null;
			}
			this.TeardownPolymorphPortrait();
		}

		// Token: 0x06009A44 RID: 39492 RVA: 0x00483724 File Offset: 0x00481924
		private void OnMaterialSourceToggleChange(int from, int to)
		{
			bool isCallingUpgrade = this._isCallingUpgrade;
			if (!isCallingUpgrade)
			{
				bool isUpgradeMode = this._isUpgradeMode;
				if (isUpgradeMode)
				{
					this.RefreshMaterialList();
				}
			}
		}

		// Token: 0x06009A45 RID: 39493 RVA: 0x0048374F File Offset: 0x0048194F
		private void RefreshUpgradeMode()
		{
			this.RefreshMaterialList();
			this.RefreshInvestedMaterialList();
		}

		// Token: 0x06009A46 RID: 39494 RVA: 0x00483760 File Offset: 0x00481960
		private void RefreshMaterialList()
		{
			int sourceType = this._itemSourceTypeMap[this.materialSourceToggle.GetActiveIndex()];
			this._materialItems.Clear();
			bool flag = this._data.MaterialItems != null;
			if (flag)
			{
				foreach (ItemDisplayData item in this._data.MaterialItems)
				{
					bool flag2 = (int)item.ItemSourceType == sourceType;
					if (flag2)
					{
						this._materialItems.Add(item);
					}
				}
			}
			this.materialScroll.SetItemList(this._materialItems);
		}

		// Token: 0x06009A47 RID: 39495 RVA: 0x0048381C File Offset: 0x00481A1C
		private void RefreshInvestedMaterialList()
		{
			this._investedMaterialItems.Clear();
			CricketRoomData roomData = this._data.CricketRoomData;
			bool flag = ((roomData != null) ? roomData.MaterialCounts : null) != null;
			if (flag)
			{
				foreach (KeyValuePair<short, int> kvp in roomData.MaterialCounts)
				{
					ItemDisplayData data = new ItemDisplayData(12, kvp.Key);
					data.Amount = kvp.Value;
					this._investedMaterialItems.Add(data);
				}
			}
			this.investedMaterialScroll.UpdateData(this._investedMaterialItems.Count);
		}

		// Token: 0x06009A48 RID: 39496 RVA: 0x004838E0 File Offset: 0x00481AE0
		private void OnRenderInvestedMaterial(int index, GameObject obj)
		{
			bool flag = index < 0 || index >= this._investedMaterialItems.Count;
			if (!flag)
			{
				CardItem cardItem = obj.GetComponent<CardItem>();
				bool flag2 = cardItem == null;
				if (!flag2)
				{
					ItemDisplayData data = this._investedMaterialItems[index];
					RowItemMain rowItemMain = obj.GetComponent<RowItemMain>();
					bool flag3 = rowItemMain == null;
					if (!flag3)
					{
						rowItemMain.SetData(data);
						cardItem.Set(rowItemMain, false);
						cardItem.SetClickEvent(delegate
						{
							this.OnClickInvestedMaterial(index, cardItem);
						});
					}
				}
			}
		}

		// Token: 0x06009A49 RID: 39497 RVA: 0x004839A0 File Offset: 0x00481BA0
		private void OnRenderUpgradeMaterial(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			bool flag = rowItemMain == null;
			if (!flag)
			{
				rowItemMain.SetData(itemData);
				rowItemLine.Set(rowItemMain, true);
			}
		}

		// Token: 0x06009A4A RID: 39498 RVA: 0x004839D4 File Offset: 0x00481BD4
		private void OnClickUpgradeMaterial(ITradeableContent content, RowItemLine itemView)
		{
			bool flag = this._isCallingUpgrade || content == null || ((itemView != null) ? itemView.RowItemMain : null) == null;
			if (!flag)
			{
				ItemSourceType sourceType = (ItemSourceType)this._itemSourceTypeMap[this.materialSourceToggle.GetActiveIndex()];
				short templateId = content.Key.TemplateId;
				Action <>9__5;
				AsyncMethodCallbackDelegate <>9__4;
				Action<List<ItemKeyAndCount>> putMaterial = delegate(List<ItemKeyAndCount> keys)
				{
					bool isCallingUpgrade = this._isCallingUpgrade;
					if (!isCallingUpgrade)
					{
						this._isCallingUpgrade = true;
						CricketCollectionDisplayData data = this._data;
						int? num;
						if (data == null)
						{
							num = null;
						}
						else
						{
							CricketRoomData cricketRoomData = data.CricketRoomData;
							num = ((cricketRoomData != null) ? new int?(cricketRoomData.Level) : null);
						}
						int? num2 = num;
						int currentLevel = num2.GetValueOrDefault();
						this._performancePendingLevel = currentLevel;
						ViewCricketCollection <>4__this = this;
						CricketCollectionDisplayData data2 = this._data;
						bool? flag3;
						if (data2 == null)
						{
							flag3 = null;
						}
						else
						{
							CricketRoomData cricketRoomData2 = data2.CricketRoomData;
							flag3 = ((cricketRoomData2 != null) ? new bool?(cricketRoomData2.EnabledPolymorphReturn) : null);
						}
						bool? flag4 = flag3;
						<>4__this._preUpgradeReturnUnlocked = flag4.GetValueOrDefault();
						ViewCricketCollection <>4__this2 = this;
						CricketCollectionDisplayData data3 = this._data;
						bool? flag5;
						if (data3 == null)
						{
							flag5 = null;
						}
						else
						{
							CricketRoomData cricketRoomData3 = data3.CricketRoomData;
							flag5 = ((cricketRoomData3 != null) ? new bool?(cricketRoomData3.EnabledMakingWish) : null);
						}
						flag4 = flag5;
						<>4__this2._preUpgradeWishingUnlocked = flag4.GetValueOrDefault();
						IAsyncMethodRequestHandler <>4__this3 = this;
						ItemSourceType sourceType = sourceType;
						AsyncMethodCallbackDelegate callback;
						if ((callback = <>9__4) == null)
						{
							callback = (<>9__4 = delegate(int _, RawDataPool _)
							{
								ViewCricketCollection <>4__this4 = this;
								Action onComplete;
								if ((onComplete = <>9__5) == null)
								{
									onComplete = (<>9__5 = delegate()
									{
										this._isCallingUpgrade = false;
									});
								}
								<>4__this4.RequestData(onComplete);
							});
						}
						TaiwuDomainMethod.AsyncCall.PutMaterialToCricketRoom(<>4__this3, keys, sourceType, callback);
					}
				};
				bool flag2 = content.Amount > 1;
				if (flag2)
				{
					UIManager.Instance.SetEscHandler(null);
					this.materialScroll.SetItemToSelectCountMode(itemView, delegate(int selectedCount)
					{
						this.ShowPreview(templateId, selectedCount, true);
						this.ApplyOptimisticRefresh();
						putMaterial(new List<ItemKeyAndCount>
						{
							new ItemKeyAndCount(content.Key, selectedCount)
						});
					}, delegate
					{
						this.HidePreview();
					}, 0, 0, 1, null, false, delegate(int count)
					{
						this.ShowPreview(templateId, count, true);
					}, false);
					UIElement setSelectCount = UIElement.SetSelectCount;
					setSelectCount.OnHide = (Action)Delegate.Combine(setSelectCount.OnHide, new Action(delegate()
					{
						this.SetEscHandlerForCurrentState();
					}));
				}
				else
				{
					this.ShowPreview(templateId, 1, true);
					this.ApplyOptimisticRefresh();
					putMaterial(new List<ItemKeyAndCount>
					{
						new ItemKeyAndCount(content.Key, 1)
					});
				}
			}
		}

		// Token: 0x06009A4B RID: 39499 RVA: 0x00483B28 File Offset: 0x00481D28
		private void OnClickInvestedMaterial(int index, RowItemLine itemView)
		{
			bool flag = this._isCallingUpgrade || index < 0 || index >= this._investedMaterialItems.Count;
			if (!flag)
			{
				ItemDisplayData data = this._investedMaterialItems[index];
				bool flag2;
				if (data != null)
				{
					RowItemLine itemView2 = itemView;
					flag2 = (((itemView2 != null) ? itemView2.RowItemMain : null) == null);
				}
				else
				{
					flag2 = true;
				}
				bool flag3 = flag2;
				if (!flag3)
				{
					short templateId = data.Key.TemplateId;
					ItemSourceType sourceType = (ItemSourceType)this._itemSourceTypeMap[this.materialSourceToggle.GetActiveIndex()];
					Action <>9__2;
					AsyncMethodCallbackDelegate <>9__1;
					Action<int> takeMaterial = delegate(int count)
					{
						bool isCallingUpgrade = this._isCallingUpgrade;
						if (!isCallingUpgrade)
						{
							this._isCallingUpgrade = true;
							this._performancePendingLevel = -1;
							List<ItemKeyAndCount> keys = new List<ItemKeyAndCount>
							{
								new ItemKeyAndCount(data.Key, count)
							};
							IAsyncMethodRequestHandler <>4__this = this;
							List<ItemKeyAndCount> keys2 = keys;
							ItemSourceType sourceType = sourceType;
							AsyncMethodCallbackDelegate callback;
							if ((callback = <>9__1) == null)
							{
								callback = (<>9__1 = delegate(int _, RawDataPool _)
								{
									ViewCricketCollection <>4__this2 = this;
									Action onComplete;
									if ((onComplete = <>9__2) == null)
									{
										onComplete = (<>9__2 = delegate()
										{
											this._isCallingUpgrade = false;
										});
									}
									<>4__this2.RequestData(onComplete);
								});
							}
							TaiwuDomainMethod.AsyncCall.TakeMaterialFromCricketRoom(<>4__this, keys2, sourceType, callback);
						}
					};
					bool flag4 = data.Amount > 1;
					if (flag4)
					{
						int maxCount = data.Amount;
						ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
						argBox.Set("MinCount", 1);
						argBox.Set("MaxCount", maxCount);
						argBox.Set("InitCount", maxCount);
						argBox.Set("LimitCount", 0);
						argBox.Set("LimitTip", string.Empty);
						argBox.SetObject("OnValueChanged", new Action<int>(delegate(int count)
						{
							this.ShowPreview(templateId, count, false);
						}));
						argBox.SetObject("OnConfirmSetCount", new Action<int>(delegate(int count)
						{
							this.ShowPreview(templateId, count, false);
							this.ApplyOptimisticRefresh();
							takeMaterial(count);
						}));
						argBox.SetObject("OnCancelSetCount", new Action(delegate
						{
							this.HidePreview();
						}));
						argBox.SetObject("ItemRectTrans", itemView.transform as RectTransform);
						argBox.Set("ZeroValid", false);
						Transform originalParent = itemView.transform.parent;
						int originalSibling = itemView.transform.GetSiblingIndex();
						UIManager.Instance.SetEscHandler(null);
						UIElement.SetSelectCount.SetOnInitArgs(argBox);
						UIManager.Instance.ShowUI(UIElement.SetSelectCount, true);
						UIElement setSelectCount = UIElement.SetSelectCount;
						setSelectCount.OnShowed = (Action)Delegate.Combine(setSelectCount.OnShowed, new Action(delegate()
						{
							itemView.SetSelected(true);
							itemView.transform.SetParent(UIElement.SetSelectCount.UiBase.transform);
							itemView.SetClickEvent(delegate
							{
								GEvent.OnEvent(UiEvents.OnConfirmSetSelectCount, null);
							});
						}));
						UIElement setSelectCount2 = UIElement.SetSelectCount;
						Action <>9__9;
						setSelectCount2.OnHide = (Action)Delegate.Combine(setSelectCount2.OnHide, new Action(delegate()
						{
							itemView.SetSelected(false);
							itemView.transform.SetParent(originalParent);
							itemView.transform.SetSiblingIndex(originalSibling);
							RowItem itemView3 = itemView;
							Action clickEvent;
							if ((clickEvent = <>9__9) == null)
							{
								clickEvent = (<>9__9 = delegate()
								{
									this.OnClickInvestedMaterial(index, itemView);
								});
							}
							itemView3.SetClickEvent(clickEvent);
							this.SetEscHandlerForCurrentState();
						}));
					}
					else
					{
						this.ShowPreview(templateId, 1, false);
						this.ApplyOptimisticRefresh();
						takeMaterial(1);
					}
				}
			}
		}

		// Token: 0x06009A4C RID: 39500 RVA: 0x00483E04 File Offset: 0x00482004
		private void ShowPreview(short templateId, int deltaCount, bool isAdding)
		{
			CricketCollectionDisplayData data = this._data;
			CricketRoomData roomData = (data != null) ? data.CricketRoomData : null;
			bool flag = roomData == null;
			if (!flag)
			{
				Dictionary<short, int> previewCounts = new Dictionary<short, int>();
				bool flag2 = roomData.MaterialCounts != null;
				if (flag2)
				{
					foreach (KeyValuePair<short, int> kvp in roomData.MaterialCounts)
					{
						previewCounts[kvp.Key] = kvp.Value;
					}
				}
				if (isAdding)
				{
					int existing = previewCounts.GetOrDefault(templateId);
					previewCounts[templateId] = existing + deltaCount;
				}
				else
				{
					int existing2;
					bool flag3 = previewCounts.TryGetValue(templateId, out existing2);
					if (flag3)
					{
						int newCount = existing2 - deltaCount;
						bool flag4 = newCount <= 0;
						if (flag4)
						{
							previewCounts.Remove(templateId);
						}
						else
						{
							previewCounts[templateId] = newCount;
						}
					}
				}
				this._previewExp = CricketRoomData.CalcExpFromMaterialCounts(previewCounts);
				int requireExpPerLevel = GlobalConfig.Instance.CricketRoomRequireExpPerLevel;
				this._previewLevel = GlobalConfig.Instance.CricketRoomBaseLevel + this._previewExp / requireExpPerLevel;
				int currentLevel = roomData.Level;
				bool flag5 = this._previewLevel != currentLevel;
				if (flag5)
				{
					this.levelPreviewRoot.SetActive(true);
					string color = (this._previewLevel > currentLevel) ? "brightblue" : "brightred";
					this.levelPreviewText.text = this._previewLevel.ToString().SetColor(color);
				}
				else
				{
					this.levelPreviewRoot.SetActive(false);
				}
				this.UpdateBuffPreview(this.buffReduceAgePreviewRoot, this.buffReduceAgePreviewText, roomData.ReduceAgeEffect, 10 * (this._previewLevel - 1), "%");
				this.UpdateBuffPreview(this.buffAddSpiritPreviewRoot, this.buffAddSpiritPreviewText, roomData.AddSpiritEffect, 2 * this._previewLevel, "");
				this.UpdateBuffPreview(this.buffPolymorphRatePreviewRoot, this.buffPolymorphRatePreviewText, roomData.PolymorphRateEffect, GlobalConfig.Instance.CricketPolymorphBaseRate + 2 * this._previewLevel, "%");
				this.UpdateBuffPreview(this.buffRecoverDurabilityPreviewRoot, this.buffRecoverDurabilityPreviewText, roomData.RecoverDurabilityEffect, 30 * this._previewLevel, "%");
			}
		}

		// Token: 0x06009A4D RID: 39501 RVA: 0x00484044 File Offset: 0x00482244
		private void UpdateBuffPreview(GameObject previewRoot, TextMeshProUGUI previewText, int currentValue, int previewValue, string suffix = "")
		{
			bool flag = previewRoot == null;
			if (!flag)
			{
				bool flag2 = previewValue == currentValue;
				if (flag2)
				{
					previewRoot.SetActive(false);
				}
				else
				{
					previewRoot.SetActive(true);
					string color = (previewValue > currentValue) ? "brightblue" : "brightred";
					previewText.text = string.Format("{0}{1}", previewValue, suffix).SetColor(color);
				}
			}
		}

		// Token: 0x06009A4E RID: 39502 RVA: 0x004840B0 File Offset: 0x004822B0
		private void HidePreview()
		{
			bool flag = this.levelPreviewRoot != null;
			if (flag)
			{
				this.levelPreviewRoot.SetActive(false);
			}
			bool flag2 = this.buffReduceAgePreviewRoot != null;
			if (flag2)
			{
				this.buffReduceAgePreviewRoot.SetActive(false);
			}
			bool flag3 = this.buffAddSpiritPreviewRoot != null;
			if (flag3)
			{
				this.buffAddSpiritPreviewRoot.SetActive(false);
			}
			bool flag4 = this.buffPolymorphRatePreviewRoot != null;
			if (flag4)
			{
				this.buffPolymorphRatePreviewRoot.SetActive(false);
			}
			bool flag5 = this.buffRecoverDurabilityPreviewRoot != null;
			if (flag5)
			{
				this.buffRecoverDurabilityPreviewRoot.SetActive(false);
			}
		}

		// Token: 0x06009A4F RID: 39503 RVA: 0x00484154 File Offset: 0x00482354
		private void ApplyOptimisticRefresh()
		{
			bool flag = this._previewLevel <= 0;
			if (!flag)
			{
				this.roomLevelText.text = LanguageKey.LK_CricketCollection_RoomLevel.TrFormat(this._previewLevel);
				int requireExp = GlobalConfig.Instance.CricketRoomRequireExpPerLevel;
				bool isMaxLevel = this._previewLevel >= GlobalConfig.Instance.CricketRoomMaxLevel;
				int expInLevel = isMaxLevel ? requireExp : (this._previewExp % requireExp);
				this.expProgressText.text = LanguageKey.LK_CricketCollection_ExpProgress.TrFormat(expInLevel, requireExp);
				this.expProgressImage.fillAmount = (isMaxLevel ? 1f : ((requireExp > 0) ? ((float)expInLevel / (float)requireExp) : 0f));
				this.buffReduceAgeText.text = LanguageKey.LK_CricketRoom_Buff_ReduceAge.TrFormat(10 * (this._previewLevel - 1)).ColorReplace();
				this.buffAddSpiritText.text = LanguageKey.LK_CricketRoom_Buff_AddSpirit.TrFormat(2 * this._previewLevel).ColorReplace();
				this.buffPolymorphRateText.text = LanguageKey.LK_CricketRoom_Buff_PolymorphRate.TrFormat(GlobalConfig.Instance.CricketPolymorphBaseRate + 2 * this._previewLevel).ColorReplace();
				this.buffRecoverDurabilityText.text = LanguageKey.LK_CricketRoom_Buff_RecoverDurability.TrFormat(30 * this._previewLevel).ColorReplace();
				this.HidePreview();
			}
		}

		// Token: 0x06009A50 RID: 39504 RVA: 0x004842C8 File Offset: 0x004824C8
		private void SwitchToRightPanel()
		{
			bool flag = !this._isLeftPanel;
			if (!flag)
			{
				this.ResetReturnSpiritSelection();
				this._isLeftPanel = false;
				this.AnimateCameraTo(false);
				this.SetEscHandlerForCurrentState();
			}
		}

		// Token: 0x06009A51 RID: 39505 RVA: 0x00484304 File Offset: 0x00482504
		private void SwitchToLeftPanel()
		{
			bool isLeftPanel = this._isLeftPanel;
			if (!isLeftPanel)
			{
				this._isLeftPanel = true;
				bool flag = this.cricketHolder != null;
				if (flag)
				{
					this.cricketHolder.gameObject.SetActive(false);
				}
				this.AnimateCameraTo(true);
				this.SetEscHandlerForCurrentState();
				this.RefreshReturnSpiritAddButton();
			}
		}

		// Token: 0x06009A52 RID: 39506 RVA: 0x00484360 File Offset: 0x00482560
		private void RefreshReturnSpiritAddButton()
		{
			bool isCallingReturn = this._isCallingReturn;
			if (!isCallingReturn)
			{
				this.btnConfirmReturn.interactable = false;
				this.SetConfirmReturnTip(false);
				List<int> teamCharIds = SingletonObject.getInstance<CharacterMonitorModel>().GetTaiwuTeamCharIds();
				bool flag = teamCharIds == null || teamCharIds.Count == 0;
				if (flag)
				{
					this._cachedPolymorphCharacters = null;
				}
				else
				{
					teamCharIds.RemoveAll((int id) => id == this._selectedReturnCharId);
					bool flag2 = teamCharIds.Count == 0;
					if (flag2)
					{
						this._cachedPolymorphCharacters = null;
					}
					else
					{
						CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForGeneralScrollListBatch(this, teamCharIds, delegate(int offset, RawDataPool pool)
						{
							List<CharacterDisplayDataForGeneralScrollList> displayDataList = null;
							Serializer.Deserialize(pool, offset, ref displayDataList);
							bool flag3 = displayDataList == null || displayDataList.Count == 0;
							if (flag3)
							{
								this._cachedPolymorphCharacters = null;
							}
							else
							{
								this._cachedPolymorphCharacters = displayDataList.Where(new Func<CharacterDisplayDataForGeneralScrollList, bool>(CricketPolymorphHelper.IsCricketPolymorphCharacter)).ToList<CharacterDisplayDataForGeneralScrollList>();
							}
						});
					}
				}
			}
		}

		// Token: 0x06009A53 RID: 39507 RVA: 0x004843F8 File Offset: 0x004825F8
		private void SetConfirmReturnTip(bool hasSelection)
		{
			TooltipInvoker tip = this.btnConfirmReturn.GetComponent<TooltipInvoker>();
			bool flag = tip == null;
			if (!flag)
			{
				tip.enabled = true;
				TooltipInvoker tooltipInvoker = tip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				if (hasSelection)
				{
					tip.Type = TipType.Simple;
					tip.RuntimeParam.Set("arg0", LanguageKey.LK_CricketCollection_ConfirmReturn_Title.Tr());
					tip.RuntimeParam.Set("arg1", LanguageKey.LK_CricketCollection_ConfirmReturn_Desc.Tr());
				}
				else
				{
					tip.Type = TipType.SingleDesc;
					tip.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_CricketCollection_ConfirmReturn_DisabledTip));
				}
				tip.Refresh(false, -1);
			}
		}

		// Token: 0x06009A54 RID: 39508 RVA: 0x004844B8 File Offset: 0x004826B8
		private void SwitchToDisplayMode()
		{
			bool flag = !this._isUpgradeMode;
			if (!flag)
			{
				this._isUpgradeMode = false;
				this.btnUpgradeRoom.interactable = true;
				this.collectionCanvasGroup.DOKill(false);
				this.upgradeModeCanvasGroup.DOKill(false);
				this.upgradeModeRoot.SetActive(false);
				this.collectionRoot.SetActive(true);
				this.collectionCanvasGroup.alpha = 1f;
				this.SetEscHandlerForCurrentState();
			}
		}

		// Token: 0x06009A55 RID: 39509 RVA: 0x00484538 File Offset: 0x00482738
		private void SwitchToUpgradeMode()
		{
			bool isUpgradeMode = this._isUpgradeMode;
			if (!isUpgradeMode)
			{
				this._isUpgradeMode = true;
				this.btnUpgradeRoom.interactable = false;
				this.collectionCanvasGroup.DOKill(false);
				this.upgradeModeCanvasGroup.DOKill(false);
				this.collectionRoot.SetActive(false);
				this.upgradeModeRoot.SetActive(true);
				this.upgradeModeCanvasGroup.alpha = 0f;
				this.upgradeModeCanvasGroup.DOFade(1f, 0.3f);
				this.RefreshUpgradeMode();
				this.SetEscHandlerForCurrentState();
			}
		}

		// Token: 0x06009A56 RID: 39510 RVA: 0x004845D0 File Offset: 0x004827D0
		private void SetEscHandlerForCurrentState()
		{
			bool isLeftPanel = this._isLeftPanel;
			if (isLeftPanel)
			{
				UIManager.Instance.SetEscHandler(new Action(this.SwitchToRightPanel));
			}
			else
			{
				bool isUpgradeMode = this._isUpgradeMode;
				if (isUpgradeMode)
				{
					UIManager.Instance.SetEscHandler(new Action(this.SwitchToDisplayMode));
				}
				else
				{
					UIManager.Instance.SetEscHandler(null);
				}
			}
		}

		// Token: 0x06009A57 RID: 39511 RVA: 0x00484630 File Offset: 0x00482830
		private void ReleaseDlcEscHandler()
		{
			bool flag = UIManager.Instance.CheckEscHandler(new Action(this.SwitchToRightPanel)) || UIManager.Instance.CheckEscHandler(new Action(this.SwitchToDisplayMode));
			if (flag)
			{
				UIManager.Instance.SetEscHandler(null);
			}
		}

		// Token: 0x06009A58 RID: 39512 RVA: 0x0048467F File Offset: 0x0048287F
		private void OnClickUpgradeRoom()
		{
			this.SwitchToUpgradeMode();
		}

		// Token: 0x06009A59 RID: 39513 RVA: 0x0048468C File Offset: 0x0048288C
		private void OnClickWishingCricket()
		{
			CricketCollectionDisplayData data = this._data;
			bool flag;
			if (data == null)
			{
				flag = true;
			}
			else
			{
				CricketRoomData cricketRoomData = data.CricketRoomData;
				bool? flag2 = (cricketRoomData != null) ? new bool?(cricketRoomData.EnabledMakingWish) : null;
				bool flag3 = true;
				flag = !(flag2.GetValueOrDefault() == flag3 & flag2 != null);
			}
			bool flag4 = flag;
			if (!flag4)
			{
				this._wishingPanelWasUpgradeMode = this._isUpgradeMode;
				bool isUpgradeMode = this._isUpgradeMode;
				if (isUpgradeMode)
				{
					UIManager.Instance.SetEscHandler(null);
				}
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Set("LuckPoint", this._data.CricketLuckPoint);
				argBox.SetObject("OnWishSuccess", new Action<Location>(this.OnWishingSuccess));
				UIElement.CricketWishing.SetOnInitArgs(argBox);
				UIManager.Instance.MaskUI(UIElement.CricketWishing);
				UIElement cricketWishing = UIElement.CricketWishing;
				cricketWishing.OnHide = (Action)Delegate.Remove(cricketWishing.OnHide, new Action(this.OnWishingPanelHide));
				UIElement cricketWishing2 = UIElement.CricketWishing;
				cricketWishing2.OnHide = (Action)Delegate.Combine(cricketWishing2.OnHide, new Action(this.OnWishingPanelHide));
			}
		}

		// Token: 0x06009A5A RID: 39514 RVA: 0x004847A8 File Offset: 0x004829A8
		private void OnWishingPanelHide()
		{
			UIElement cricketWishing = UIElement.CricketWishing;
			cricketWishing.OnHide = (Action)Delegate.Remove(cricketWishing.OnHide, new Action(this.OnWishingPanelHide));
			bool wishingPanelWasUpgradeMode = this._wishingPanelWasUpgradeMode;
			if (wishingPanelWasUpgradeMode)
			{
				this.SetEscHandlerForCurrentState();
			}
		}

		// Token: 0x06009A5B RID: 39515 RVA: 0x004847F0 File Offset: 0x004829F0
		private void OnWishingSuccess(Location location)
		{
			this.RequestData(null);
			bool flag = !location.IsValid();
			if (!flag)
			{
				bool flag2 = this.ShouldPlayWishingWorldMapPerformance(location);
				if (flag2)
				{
					this.StartWishingPerformanceFlow(location);
				}
				else
				{
					this.PlayWishingSuccessHint();
				}
			}
		}

		// Token: 0x06009A5C RID: 39516 RVA: 0x00484834 File Offset: 0x00482A34
		private bool ShouldPlayWishingWorldMapPerformance(Location location)
		{
			WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
			short villageAreaId = mapModel.GetTaiwuVillageAreaId();
			return mapModel.CurrentAreaId == location.AreaId && mapModel.GetStateId(mapModel.CurrentAreaId) == mapModel.GetStateId(villageAreaId);
		}

		// Token: 0x06009A5D RID: 39517 RVA: 0x0048487C File Offset: 0x00482A7C
		private void PlayWishingSuccessHint()
		{
			DialogCmd dialogCmd = new DialogCmd
			{
				Type = 2,
				Title = LocalStringManager.Get(LanguageKey.LK_CricketWish_Title),
				Content = LocalStringManager.Get(LanguageKey.LK_Cricket_Wish_Popup)
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x06009A5E RID: 39518 RVA: 0x004848E4 File Offset: 0x00482AE4
		private void StartWishingPerformanceFlow(Location location)
		{
			ViewCricketCollection.<>c__DisplayClass233_0 CS$<>8__locals1 = new ViewCricketCollection.<>c__DisplayClass233_0();
			CS$<>8__locals1.location = location;
			CS$<>8__locals1.<>4__this = this;
			UIElement.ScreenFade.SetOnInitArgs(EasyPool.Get<ArgumentBox>());
			UIElement.ScreenFade.Show();
			UIElement screenFade = UIElement.ScreenFade;
			screenFade.OnShowed = (Action)Delegate.Combine(screenFade.OnShowed, new Action(CS$<>8__locals1.<StartWishingPerformanceFlow>g__OnScreenFadeReady|0));
		}

		// Token: 0x06009A5F RID: 39519 RVA: 0x00484948 File Offset: 0x00482B48
		private void ReopenCricketViewsWithFade(ViewScreenFade fade)
		{
			ViewCricketCollection.<>c__DisplayClass234_0 CS$<>8__locals1 = new ViewCricketCollection.<>c__DisplayClass234_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.fade = fade;
			UIElement cricketCollection = UIElement.CricketCollection;
			cricketCollection.OnShowed = (Action)Delegate.Combine(cricketCollection.OnShowed, new Action(CS$<>8__locals1.<ReopenCricketViewsWithFade>g__OnCricketCollectionReopened|0));
			this._wishingPanelWasUpgradeMode = false;
			UIManager.Instance.StackToUI(UIElement.CricketCollection);
		}

		// Token: 0x06009A60 RID: 39520 RVA: 0x004849A8 File Offset: 0x00482BA8
		private void OnClickReturnSpiritJade()
		{
			this.SwitchToLeftPanel();
		}

		// Token: 0x06009A61 RID: 39521 RVA: 0x004849B4 File Offset: 0x00482BB4
		private void OnClickAddCharacter()
		{
			bool isCallingReturn = this._isCallingReturn;
			if (!isCallingReturn)
			{
				this._returnSpiritHadEscHandler = this._isLeftPanel;
				bool isLeftPanel = this._isLeftPanel;
				if (isLeftPanel)
				{
					UIManager.Instance.SetEscHandler(null);
				}
				UIElement selectChar = UIElement.SelectChar;
				selectChar.OnHide = (Action)Delegate.Remove(selectChar.OnHide, new Action(this.OnSelectCharHide));
				UIElement selectChar2 = UIElement.SelectChar;
				selectChar2.OnHide = (Action)Delegate.Combine(selectChar2.OnHide, new Action(this.OnSelectCharHide));
				List<ISelectCharacterData> dataList = new List<ISelectCharacterData>();
				bool flag = this._cachedPolymorphCharacters != null;
				if (flag)
				{
					foreach (CharacterDisplayDataForGeneralScrollList ch in this._cachedPolymorphCharacters)
					{
						dataList.Add(new BasicSelectCharacterDataAdapter(ch));
					}
				}
				CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.None);
				config.InteractionMode = ESelectCharacterInteractionMode.Instant;
				config.SelectionMode = ESelectCharacterSelectionMode.Single;
				config.TargetCount = 1;
				ViewSelectCharacter.Show(config, dataList, new SelectCharacterCallback(this.OnReturnCharSelected), null, false);
			}
		}

		// Token: 0x06009A62 RID: 39522 RVA: 0x00484ADC File Offset: 0x00482CDC
		private void OnSelectCharHide()
		{
			UIElement selectChar = UIElement.SelectChar;
			selectChar.OnHide = (Action)Delegate.Remove(selectChar.OnHide, new Action(this.OnSelectCharHide));
			bool returnSpiritHadEscHandler = this._returnSpiritHadEscHandler;
			if (returnSpiritHadEscHandler)
			{
				this.SetEscHandlerForCurrentState();
			}
		}

		// Token: 0x06009A63 RID: 39523 RVA: 0x00484B24 File Offset: 0x00482D24
		private void OnReturnCharSelected(List<int> selectedIds)
		{
			bool flag = selectedIds == null || selectedIds.Count == 0;
			if (!flag)
			{
				this._selectedReturnCharId = selectedIds[0];
				List<CharacterDisplayDataForGeneralScrollList> cachedPolymorphCharacters = this._cachedPolymorphCharacters;
				CharacterDisplayDataForGeneralScrollList charData = (cachedPolymorphCharacters != null) ? cachedPolymorphCharacters.Find((CharacterDisplayDataForGeneralScrollList x) => x.CharacterId == this._selectedReturnCharId) : null;
				bool flag2 = charData == null;
				if (!flag2)
				{
					this.btnConfirmReturn.interactable = true;
					this.SetConfirmReturnTip(true);
					this.charAvatar.Refresh(charData.AvatarRelatedData, charData.CharacterTemplateId);
					this.charNameText.text = NameCenter.GetMonasticTitleOrDisplayName(ref charData.NameData, false, false).SetColor("white");
					this.selectedCharRoot.SetActive(true);
					this.btnAddCharacter.gameObject.SetActive(false);
					this.btnChangeCharacter.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x06009A64 RID: 39524 RVA: 0x00484C04 File Offset: 0x00482E04
		private void ResetReturnSpiritSelection()
		{
			this._selectedReturnCharId = -1;
			this.btnConfirmReturn.interactable = false;
			this.SetConfirmReturnTip(false);
			this.selectedCharRoot.SetActive(false);
			this.btnAddCharacter.gameObject.SetActive(true);
			this.btnChangeCharacter.gameObject.SetActive(false);
		}

		// Token: 0x06009A65 RID: 39525 RVA: 0x00484C60 File Offset: 0x00482E60
		private void OnClickConfirmReturn()
		{
			bool flag = this._isCallingReturn || this._selectedReturnCharId < 0;
			if (!flag)
			{
				this._isCallingReturn = true;
				TaiwuDomainMethod.AsyncCall.CricketRoomPolymorphReturn(this, this._selectedReturnCharId, delegate(int offset, RawDataPool pool)
				{
					this._isCallingReturn = false;
					ItemDisplayData cricketItem = null;
					Serializer.Deserialize(pool, offset, ref cricketItem);
					bool flag2 = cricketItem != null && cricketItem.RealKey.IsValid();
					if (flag2)
					{
						this.PlayReturnSpiritPerformance(cricketItem);
					}
					else
					{
						Debug.LogWarning("[CricketCollection] CricketRoomPolymorphReturn failed: no cricket returned");
						this.ResetReturnSpiritSelection();
						this.RequestData(null);
					}
				});
			}
		}

		// Token: 0x06009A66 RID: 39526 RVA: 0x00484CA8 File Offset: 0x00482EA8
		private void OnClickViewCharacter()
		{
			bool flag = this._selectedReturnCharId < 0;
			if (!flag)
			{
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Set("CharacterId", this._selectedReturnCharId);
				UIElement.CharacterMenu.SetOnInitArgs(argBox);
				UIManager.Instance.MaskUI(UIElement.CharacterMenu);
			}
		}

		// Token: 0x06009A67 RID: 39527 RVA: 0x00484CFC File Offset: 0x00482EFC
		private void SetupPolymorphPortrait()
		{
			bool flag = this.fxPolymorphMask == null;
			if (!flag)
			{
				RectTransform maskTransform = this.fxPolymorphMask;
				this._maskOriginalParent = maskTransform.parent;
				this._maskOriginalSiblingIndex = maskTransform.GetSiblingIndex();
				this._maskOriginalLocalPosition = maskTransform.localPosition;
				this._maskOriginalLocalScale = maskTransform.localScale;
				this._polymorphRT = new RenderTexture(960, 1052, 24, RenderTextureFormat.ARGB32);
				this._polymorphRT.Create();
				this._polymorphShapeTex = new Texture2D(960, 1052, TextureFormat.ARGB32, false);
				int uiLayer = 5;
				this._polymorphCameraObj = new GameObject("PolymorphPortraitCamera");
				this._polymorphCameraObj.transform.position = new Vector3(0f, 0f, -10f);
				this._polymorphCamera = this._polymorphCameraObj.AddComponent<Camera>();
				this._polymorphCamera.orthographic = true;
				this._polymorphCamera.orthographicSize = 526f;
				this._polymorphCamera.clearFlags = CameraClearFlags.Color;
				this._polymorphCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
				this._polymorphCamera.nearClipPlane = 0.3f;
				this._polymorphCamera.farClipPlane = 500f;
				this._polymorphCamera.targetTexture = this._polymorphRT;
				this._polymorphCamera.cullingMask = 1 << uiLayer;
				this._polymorphCamera.depth = 10f;
				this._polymorphCameraObj.layer = uiLayer;
				GameObject canvasObj = new GameObject("PolymorphPortraitCanvas", new Type[]
				{
					typeof(Canvas)
				});
				canvasObj.layer = uiLayer;
				canvasObj.transform.SetParent(this._polymorphCameraObj.transform, false);
				this._polymorphCanvas = canvasObj.GetComponent<Canvas>();
				this._polymorphCanvas.renderMode = RenderMode.ScreenSpaceCamera;
				this._polymorphCanvas.worldCamera = this._polymorphCamera;
				this._polymorphCanvas.planeDistance = 250f;
				this._polymorphCanvas.sortingOrder = 100;
				CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
				scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
				scaler.referenceResolution = new Vector2(960f, 1052f);
				scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
				maskTransform.SetParent(this._polymorphCanvas.transform, false);
				RectTransform maskRt;
				bool flag2;
				if (maskTransform != null)
				{
					maskRt = maskTransform;
					flag2 = true;
				}
				else
				{
					flag2 = false;
				}
				bool flag3 = flag2;
				if (flag3)
				{
					maskRt.anchorMin = new Vector2(0.5f, 0.5f);
					maskRt.anchorMax = new Vector2(0.5f, 0.5f);
					maskRt.anchoredPosition = Vector2.zero;
					maskRt.localScale = Vector3.one;
				}
				this._isPolymorphActive = true;
				this.SyncPolymorphShapeTex();
				this.AssignPortraitToPolymorphParticle();
			}
		}

		// Token: 0x06009A68 RID: 39528 RVA: 0x00484FC8 File Offset: 0x004831C8
		private void AssignPortraitToPolymorphParticle()
		{
			bool flag = this._polymorphRT == null;
			if (!flag)
			{
				bool flag2 = this.fxPolymorphBlend != null;
				if (flag2)
				{
					this.fxPolymorphBlend.shape.texture = this._polymorphShapeTex;
				}
				bool flag3 = this.fxPolymorphAdd != null;
				if (flag3)
				{
					this.fxPolymorphAdd.shape.texture = this._polymorphShapeTex;
				}
				bool flag4 = this.fxPolymorphMain != null;
				if (flag4)
				{
					ParticleSystemRenderer renderer = this.fxPolymorphMain.GetComponent<ParticleSystemRenderer>();
					bool flag5 = renderer != null;
					if (flag5)
					{
						Material mat = renderer.material;
						mat.mainTexture = this._polymorphRT;
					}
				}
			}
		}

		// Token: 0x06009A69 RID: 39529 RVA: 0x0048508C File Offset: 0x0048328C
		private void SyncPolymorphShapeTex()
		{
			bool flag = this._polymorphRT == null || this._polymorphShapeTex == null;
			if (!flag)
			{
				RenderTexture prevActive = RenderTexture.active;
				RenderTexture.active = this._polymorphRT;
				this._polymorphShapeTex.ReadPixels(new Rect(0f, 0f, 960f, 1052f), 0, 0);
				this._polymorphShapeTex.Apply();
				RenderTexture.active = prevActive;
				bool flag2 = this.fxPolymorphBlend != null;
				if (flag2)
				{
					this.fxPolymorphBlend.shape.texture = this._polymorphShapeTex;
				}
				bool flag3 = this.fxPolymorphAdd != null;
				if (flag3)
				{
					this.fxPolymorphAdd.shape.texture = this._polymorphShapeTex;
				}
			}
		}

		// Token: 0x06009A6A RID: 39530 RVA: 0x00485168 File Offset: 0x00483368
		private void Update()
		{
			bool flag = this._isPolymorphActive && this._polymorphCamera != null && this._polymorphCamera.enabled;
			if (flag)
			{
				this.SyncPolymorphShapeTex();
			}
		}

		// Token: 0x06009A6B RID: 39531 RVA: 0x004851A8 File Offset: 0x004833A8
		private void TeardownPolymorphPortrait()
		{
			bool flag = this.fxPolymorphMask != null && this._maskOriginalParent != null;
			if (flag)
			{
				RectTransform maskTransform = this.fxPolymorphMask;
				maskTransform.SetParent(this._maskOriginalParent, false);
				maskTransform.SetSiblingIndex(this._maskOriginalSiblingIndex);
				maskTransform.localPosition = this._maskOriginalLocalPosition;
				maskTransform.localScale = this._maskOriginalLocalScale;
			}
			bool flag2 = this.fxReturnSpiritSuccess != null;
			if (flag2)
			{
				this.fxReturnSpiritSuccess.Stop();
				this.fxReturnSpiritSuccess.gameObject.SetActive(false);
			}
			this._isPolymorphActive = false;
			bool flag3 = this._polymorphCameraObj != null;
			if (flag3)
			{
				this._polymorphCamera.targetTexture = null;
				Object.Destroy(this._polymorphCameraObj);
				this._polymorphCameraObj = null;
				this._polymorphCamera = null;
				this._polymorphCanvas = null;
			}
			bool flag4 = this._polymorphRT != null;
			if (flag4)
			{
				this._polymorphRT.Release();
				Object.Destroy(this._polymorphRT);
				this._polymorphRT = null;
			}
			bool flag5 = this._polymorphShapeTex != null;
			if (flag5)
			{
				Object.Destroy(this._polymorphShapeTex);
				this._polymorphShapeTex = null;
			}
			this._maskOriginalParent = null;
			this._maskOriginalSiblingIndex = 0;
			this._maskOriginalLocalPosition = Vector3.zero;
			this._maskOriginalLocalScale = Vector3.one;
		}

		// Token: 0x06009A6C RID: 39532 RVA: 0x0048530A File Offset: 0x0048350A
		private IEnumerable<ColumnDefinition> GetJarColumnDefinitions()
		{
			ColumnDefinition<ITradeableContent, CricketCollectionFirstCellData> columnDefinition = new ColumnDefinition<ITradeableContent, CricketCollectionFirstCellData>();
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_ItemName.Tr());
			columnDefinition.LayoutOption = new LayoutOption
			{
				PreferredWidth = 300f,
				FlexibleWidth = 1f
			};
			columnDefinition.CellDataGenerator = ((ITradeableContent item) => new CricketCollectionFirstCellData
			{
				ItemData = item,
				IsInCollection = this.IsItemInCollection(item as ItemDisplayData)
			});
			yield return columnDefinition;
			ColumnDefinition<ITradeableContent, string> columnDefinition2 = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition2.TableHeadLabel = (() => LanguageKey.LK_Count.Tr());
			columnDefinition2.LayoutOption = new LayoutOption
			{
				PreferredWidth = 300f,
				FlexibleWidth = 1f
			};
			columnDefinition2.CellDataGenerator = ((ITradeableContent data) => data.Amount.ToString());
			yield return columnDefinition2;
			yield break;
		}

		// Token: 0x040076C5 RID: 30405
		[SerializeField]
		private RectTransform collectionMoveRoot;

		// Token: 0x040076C6 RID: 30406
		[SerializeField]
		private TextMeshProUGUI authorityAmount;

		// Token: 0x040076C7 RID: 30407
		[SerializeField]
		private TextMeshProUGUI cricketAmount;

		// Token: 0x040076C8 RID: 30408
		[SerializeField]
		private Transform slots;

		// Token: 0x040076C9 RID: 30409
		[SerializeField]
		private CToggleGroup itemSourceTypeToggle;

		// Token: 0x040076CA RID: 30410
		[SerializeField]
		private CToggleGroup itemSubTypeToggle;

		// Token: 0x040076CB RID: 30411
		[SerializeField]
		private ListStyleGeneralScroll itemScroll;

		// Token: 0x040076CC RID: 30412
		[SerializeField]
		private RowItem[] rowTemplates = new RowItem[2];

		// Token: 0x040076CD RID: 30413
		[SerializeField]
		private CButton btnRemoveAllJar;

		// Token: 0x040076CE RID: 30414
		[SerializeField]
		private CButton btnRemoveAllCricket;

		// Token: 0x040076CF RID: 30415
		[SerializeField]
		private CButton btnAutoJar;

		// Token: 0x040076D0 RID: 30416
		[SerializeField]
		private CButton btnAutoCricket;

		// Token: 0x040076D1 RID: 30417
		public Transform cricketHolder;

		// Token: 0x040076D2 RID: 30418
		[Header("DLC - Parallax")]
		[SerializeField]
		private CricketCollectionParallax parallax;

		// Token: 0x040076D3 RID: 30419
		[SerializeField]
		private float panelSwitchDuration = 0.5f;

		// Token: 0x040076D4 RID: 30420
		[Header("DLC - 观赏模式")]
		[SerializeField]
		private CButton btnViewingMode;

		// Token: 0x040076D5 RID: 30421
		[SerializeField]
		private ScrollRect backgroundScrollRect;

		// Token: 0x040076D6 RID: 30422
		[Header("DLC - UI Roots")]
		[SerializeField]
		private GameObject rightUIRoot;

		// Token: 0x040076D7 RID: 30423
		[SerializeField]
		private GameObject leftUIRoot;

		// Token: 0x040076D8 RID: 30424
		[SerializeField]
		private CanvasGroup rightUICanvasGroup;

		// Token: 0x040076D9 RID: 30425
		[SerializeField]
		private CanvasGroup leftUICanvasGroup;

		// Token: 0x040076DA RID: 30426
		[Header("DLC - Display Mode Left Elements")]
		[SerializeField]
		private TextMeshProUGUI roomLevelText;

		// Token: 0x040076DB RID: 30427
		[SerializeField]
		private CImage expProgressImage;

		// Token: 0x040076DC RID: 30428
		[SerializeField]
		private TextMeshProUGUI expProgressText;

		// Token: 0x040076DD RID: 30429
		[SerializeField]
		private TextMeshProUGUI buffReduceAgeText;

		// Token: 0x040076DE RID: 30430
		[SerializeField]
		private TextMeshProUGUI buffAddSpiritText;

		// Token: 0x040076DF RID: 30431
		[SerializeField]
		private TextMeshProUGUI buffPolymorphRateText;

		// Token: 0x040076E0 RID: 30432
		[SerializeField]
		private TextMeshProUGUI buffRecoverDurabilityText;

		// Token: 0x040076E1 RID: 30433
		[SerializeField]
		private TextMeshProUGUI buffAuthorityText;

		// Token: 0x040076E2 RID: 30434
		[SerializeField]
		private CButton btnUpgradeRoom;

		// Token: 0x040076E3 RID: 30435
		[SerializeField]
		private CButton btnWishingCricket;

		// Token: 0x040076E4 RID: 30436
		[SerializeField]
		private TextMeshProUGUI luckPointText;

		// Token: 0x040076E5 RID: 30437
		[SerializeField]
		private CButton btnReturnSpiritJade;

		// Token: 0x040076E6 RID: 30438
		[Header("DLC - Tips")]
		[SerializeField]
		private TooltipInvoker tipExpProgress;

		// Token: 0x040076E7 RID: 30439
		[SerializeField]
		private TooltipInvoker tipAuthority;

		// Token: 0x040076E8 RID: 30440
		[SerializeField]
		private TooltipInvoker tipAddSpirit;

		// Token: 0x040076E9 RID: 30441
		[SerializeField]
		private TooltipInvoker tipReduceAge;

		// Token: 0x040076EA RID: 30442
		[SerializeField]
		private TooltipInvoker tipPolymorphRate;

		// Token: 0x040076EB RID: 30443
		[SerializeField]
		private TooltipInvoker tipRecoverDurability;

		// Token: 0x040076EC RID: 30444
		[Header("DLC - Display Mode Side Info")]
		[SerializeField]
		private CanvasGroup displayLeftInfo;

		// Token: 0x040076ED RID: 30445
		[Header("DLC - Display Mode Content")]
		[SerializeField]
		private CToggleGroup displayModeToggle;

		// Token: 0x040076EE RID: 30446
		[SerializeField]
		private GameObject collectionRoot;

		// Token: 0x040076EF RID: 30447
		[SerializeField]
		private CanvasGroup collectionCanvasGroup;

		// Token: 0x040076F0 RID: 30448
		[SerializeField]
		private GameObject noDlcBackground;

		// Token: 0x040076F1 RID: 30449
		[SerializeField]
		private GameObject noDlcAuthorityRoot;

		// Token: 0x040076F2 RID: 30450
		[SerializeField]
		private GameObject dlcBackground;

		// Token: 0x040076F3 RID: 30451
		[Header("DLC - Upgrade Mode")]
		[SerializeField]
		private GameObject upgradeModeRoot;

		// Token: 0x040076F4 RID: 30452
		[SerializeField]
		private CanvasGroup upgradeModeCanvasGroup;

		// Token: 0x040076F5 RID: 30453
		[SerializeField]
		private CToggleGroup materialSourceToggle;

		// Token: 0x040076F6 RID: 30454
		[SerializeField]
		private ItemListScroll materialScroll;

		// Token: 0x040076F7 RID: 30455
		[SerializeField]
		private InfinityScroll investedMaterialScroll;

		// Token: 0x040076F8 RID: 30456
		[SerializeField]
		private CButton btnBackFromUpgrade;

		// Token: 0x040076F9 RID: 30457
		[Header("DLC - Upgrade Performance")]
		[SerializeField]
		private Transform[] levelGos;

		// Token: 0x040076FA RID: 30458
		[SerializeField]
		private Transform[] extraLevelGos;

		// Token: 0x040076FB RID: 30459
		[Header("DLC - Upgrade Preview")]
		[SerializeField]
		private GameObject levelPreviewRoot;

		// Token: 0x040076FC RID: 30460
		[SerializeField]
		private TextMeshProUGUI levelPreviewText;

		// Token: 0x040076FD RID: 30461
		[SerializeField]
		private GameObject buffReduceAgePreviewRoot;

		// Token: 0x040076FE RID: 30462
		[SerializeField]
		private TextMeshProUGUI buffReduceAgePreviewText;

		// Token: 0x040076FF RID: 30463
		[SerializeField]
		private GameObject buffAddSpiritPreviewRoot;

		// Token: 0x04007700 RID: 30464
		[SerializeField]
		private TextMeshProUGUI buffAddSpiritPreviewText;

		// Token: 0x04007701 RID: 30465
		[SerializeField]
		private GameObject buffPolymorphRatePreviewRoot;

		// Token: 0x04007702 RID: 30466
		[SerializeField]
		private TextMeshProUGUI buffPolymorphRatePreviewText;

		// Token: 0x04007703 RID: 30467
		[SerializeField]
		private GameObject buffRecoverDurabilityPreviewRoot;

		// Token: 0x04007704 RID: 30468
		[SerializeField]
		private TextMeshProUGUI buffRecoverDurabilityPreviewText;

		// Token: 0x04007705 RID: 30469
		[Header("DLC - 返灵玉")]
		[SerializeField]
		private CButton btnAddCharacter;

		// Token: 0x04007706 RID: 30470
		[SerializeField]
		private CButton btnConfirmReturn;

		// Token: 0x04007707 RID: 30471
		[SerializeField]
		private CButton btnBackFromReturnSpirit;

		// Token: 0x04007708 RID: 30472
		[SerializeField]
		private GameObject selectedCharRoot;

		// Token: 0x04007709 RID: 30473
		[SerializeField]
		private Game.Components.Avatar.Avatar charAvatar;

		// Token: 0x0400770A RID: 30474
		[SerializeField]
		private TextMeshProUGUI charNameText;

		// Token: 0x0400770B RID: 30475
		[SerializeField]
		private CButton btnViewCharacter;

		// Token: 0x0400770C RID: 30476
		[SerializeField]
		private CButton btnChangeCharacter;

		// Token: 0x0400770D RID: 30477
		[Header("DLC - Effects")]
		[SerializeField]
		private UIParticle fxUpgradeRoom;

		// Token: 0x0400770E RID: 30478
		[SerializeField]
		private UIParticle fxReturnSpiritSuccess;

		// Token: 0x0400770F RID: 30479
		[Header("DLC - 促织返灵特效贴图")]
		[SerializeField]
		private ParticleSystem fxPolymorphBlend;

		// Token: 0x04007710 RID: 30480
		[SerializeField]
		private ParticleSystem fxPolymorphAdd;

		// Token: 0x04007711 RID: 30481
		[SerializeField]
		private ParticleSystem fxPolymorphMain;

		// Token: 0x04007712 RID: 30482
		[SerializeField]
		private RectTransform fxPolymorphMask;

		// Token: 0x04007713 RID: 30483
		[Header("DLC - 返灵玉 Performance")]
		[SerializeField]
		private UIVignetteEffect vignetteOverlay;

		// Token: 0x04007714 RID: 30484
		[SerializeField]
		private Transform returnSpiritScaleRoot;

		// Token: 0x04007715 RID: 30485
		[SerializeField]
		private SkeletonGraphic returnSpiritSpine;

		// Token: 0x04007716 RID: 30486
		[SerializeField]
		private float returnSpiritSpineSpeedUp = 3f;

		// Token: 0x04007717 RID: 30487
		[SerializeField]
		private float returnSpiritZoomScale = 1.15f;

		// Token: 0x04007718 RID: 30488
		[SerializeField]
		private float returnSpiritZoomDuration = 0.5f;

		// Token: 0x04007719 RID: 30489
		[SerializeField]
		private float returnSpiritVignetteDarkness = 0.7f;

		// Token: 0x0400771A RID: 30490
		[Header("Quick Mode Positions")]
		[SerializeField]
		private Vector2 closedDlcPos;

		// Token: 0x0400771B RID: 30491
		[SerializeField]
		private Vector2 closedNoDlcPos;

		// Token: 0x0400771C RID: 30492
		[SerializeField]
		private Vector2 openPos;

		// Token: 0x0400771D RID: 30493
		[SerializeField]
		private float quickModeBgOffsetX = 100f;

		// Token: 0x0400771E RID: 30494
		[Header("Item Selector")]
		[SerializeField]
		private RectTransform itemSelectorHolder;

		// Token: 0x0400771F RID: 30495
		[SerializeField]
		private Vector2 selectorClosedPos;

		// Token: 0x04007720 RID: 30496
		[SerializeField]
		private Vector2 selectorOpenPos;

		// Token: 0x04007721 RID: 30497
		[SerializeField]
		private RectTransform itemSelectorRect;

		// Token: 0x04007722 RID: 30498
		[SerializeField]
		private float itemSelectorHeightDlc;

		// Token: 0x04007723 RID: 30499
		[SerializeField]
		private float itemSelectorHeightNoDlc;

		// Token: 0x04007724 RID: 30500
		[Header("DLC - Layout Positions")]
		[SerializeField]
		private RectTransform amountRoot;

		// Token: 0x04007725 RID: 30501
		[SerializeField]
		private CImage amountRootImage;

		// Token: 0x04007726 RID: 30502
		[SerializeField]
		private Vector2 amountRootDlcPos;

		// Token: 0x04007727 RID: 30503
		[SerializeField]
		private Vector2 amountRootNoDlcPos;

		// Token: 0x04007728 RID: 30504
		[SerializeField]
		private RectTransform buttonsRoot;

		// Token: 0x04007729 RID: 30505
		[SerializeField]
		private Vector2 buttonsRootDlcPos;

		// Token: 0x0400772A RID: 30506
		[SerializeField]
		private Vector2 buttonsRootNoDlcPos;

		// Token: 0x0400772B RID: 30507
		[SerializeField]
		private Vector2 buttonsRootDlcSize;

		// Token: 0x0400772C RID: 30508
		[SerializeField]
		private Vector2 buttonsRootNoDlcSize;

		// Token: 0x0400772D RID: 30509
		[SerializeField]
		private CImage[] imageArrayForNoDlc;

		// Token: 0x0400772E RID: 30510
		private const float Duration = 0.3f;

		// Token: 0x0400772F RID: 30511
		private readonly Dictionary<int, int> _itemSourceTypeMap = new Dictionary<int, int>
		{
			{
				0,
				1
			},
			{
				1,
				2
			}
		};

		// Token: 0x04007730 RID: 30512
		private readonly Dictionary<int, int> _itemTypeMap = new Dictionary<int, int>
		{
			{
				0,
				1100
			},
			{
				1,
				1201
			}
		};

		// Token: 0x04007731 RID: 30513
		private List<ItemDisplayData> _filteredItems = new List<ItemDisplayData>();

		// Token: 0x04007732 RID: 30514
		private List<CricketCollectionJar> _slots = new List<CricketCollectionJar>();

		// Token: 0x04007733 RID: 30515
		private CricketCollectionDisplayData _data;

		// Token: 0x04007734 RID: 30516
		private bool _isSelectingItem;

		// Token: 0x04007735 RID: 30517
		private int _currIndex = -1;

		// Token: 0x04007736 RID: 30518
		private bool _isDlcEnabled;

		// Token: 0x04007737 RID: 30519
		private bool _isUpgradeMode;

		// Token: 0x04007738 RID: 30520
		private bool _isLeftPanel;

		// Token: 0x04007739 RID: 30521
		private bool _wishingPanelWasUpgradeMode;

		// Token: 0x0400773A RID: 30522
		private bool _isCallingUpgrade;

		// Token: 0x0400773B RID: 30523
		private int _selectedReturnCharId = -1;

		// Token: 0x0400773C RID: 30524
		private bool _isCallingReturn;

		// Token: 0x0400773D RID: 30525
		private bool _returnSpiritHadEscHandler;

		// Token: 0x0400773E RID: 30526
		private List<CharacterDisplayDataForGeneralScrollList> _cachedPolymorphCharacters;

		// Token: 0x0400773F RID: 30527
		private int _previewLevel;

		// Token: 0x04007740 RID: 30528
		private int _previewExp;

		// Token: 0x04007741 RID: 30529
		private Tweener _parallaxTween;

		// Token: 0x04007742 RID: 30530
		private bool _isViewingMode;

		// Token: 0x04007743 RID: 30531
		private int _performancePendingLevel = -1;

		// Token: 0x04007744 RID: 30532
		private bool _isPlayingPerformance;

		// Token: 0x04007745 RID: 30533
		private Sequence _performanceSequence;

		// Token: 0x04007746 RID: 30534
		private readonly List<ItemDisplayData> _materialItems = new List<ItemDisplayData>();

		// Token: 0x04007747 RID: 30535
		private readonly List<ItemDisplayData> _investedMaterialItems = new List<ItemDisplayData>();

		// Token: 0x04007748 RID: 30536
		private readonly UIParticlePlayHelper _particleHelper = new UIParticlePlayHelper();

		// Token: 0x04007749 RID: 30537
		private bool _preUpgradeReturnUnlocked;

		// Token: 0x0400774A RID: 30538
		private bool _preUpgradeWishingUnlocked;

		// Token: 0x0400774B RID: 30539
		private Sequence _returnSpiritSequence;

		// Token: 0x0400774C RID: 30540
		private bool _isPlayingReturnSpiritPerformance;

		// Token: 0x0400774D RID: 30541
		private GameObject _polymorphCameraObj;

		// Token: 0x0400774E RID: 30542
		private Camera _polymorphCamera;

		// Token: 0x0400774F RID: 30543
		private Canvas _polymorphCanvas;

		// Token: 0x04007750 RID: 30544
		private RenderTexture _polymorphRT;

		// Token: 0x04007751 RID: 30545
		private Texture2D _polymorphShapeTex;

		// Token: 0x04007752 RID: 30546
		private Transform _maskOriginalParent;

		// Token: 0x04007753 RID: 30547
		private int _maskOriginalSiblingIndex;

		// Token: 0x04007754 RID: 30548
		private Vector3 _maskOriginalLocalPosition;

		// Token: 0x04007755 RID: 30549
		private Vector3 _maskOriginalLocalScale;

		// Token: 0x04007756 RID: 30550
		private bool _isPolymorphActive;
	}
}
