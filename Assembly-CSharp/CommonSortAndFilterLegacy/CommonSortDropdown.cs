using System;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using FrameWork.UISystem.Components;
using UICommon;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x0200042F RID: 1071
	public class CommonSortDropdown : CommonFakeHidePanel, ICommonSortUi
	{
		// Token: 0x17000675 RID: 1653
		// (get) Token: 0x06003F71 RID: 16241 RVA: 0x001F9C2C File Offset: 0x001F7E2C
		private HashSet<short> _displayingSortIds
		{
			get
			{
				return this._sortState.DisplayingSortIds;
			}
		}

		// Token: 0x17000676 RID: 1654
		// (get) Token: 0x06003F72 RID: 16242 RVA: 0x001F9C39 File Offset: 0x001F7E39
		public HashSet<short> DisplayingSortIds
		{
			get
			{
				return this._displayingSortIds;
			}
		}

		// Token: 0x17000677 RID: 1655
		// (get) Token: 0x06003F73 RID: 16243 RVA: 0x001F9C41 File Offset: 0x001F7E41
		private float MenuBarExpectedWidth
		{
			get
			{
				return (this._maxButtonWidth != null) ? Mathf.Max(this.baseWidth, this._maxButtonWidth.Value) : this.baseWidth;
			}
		}

		// Token: 0x17000678 RID: 1656
		// (get) Token: 0x06003F74 RID: 16244 RVA: 0x001F9C6E File Offset: 0x001F7E6E
		private CommonSortMenuBar MenuBar
		{
			get
			{
				return this._menuBarInstance.GetComponent<CommonSortMenuBar>();
			}
		}

		// Token: 0x06003F75 RID: 16245 RVA: 0x001F9C7B File Offset: 0x001F7E7B
		protected override void Awake()
		{
			base.Awake();
			this._autoHeightHelper = base.GetComponent<ScrollRectAutoHeightHelper>();
			this.RefreshAutoHeightHelperEnabled();
		}

		// Token: 0x06003F76 RID: 16246 RVA: 0x001F9C98 File Offset: 0x001F7E98
		private void RefreshAutoHeightHelperEnabled()
		{
			if (this._autoHeightHelper == null)
			{
				this._autoHeightHelper = base.GetComponent<ScrollRectAutoHeightHelper>();
			}
			this._autoHeightHelper.enabled = !this.DisableAutoHeight;
		}

		// Token: 0x06003F77 RID: 16247 RVA: 0x001F9CC4 File Offset: 0x001F7EC4
		private void OnEnable()
		{
			this.CalcMaxHeight();
		}

		// Token: 0x17000679 RID: 1657
		// (get) Token: 0x06003F78 RID: 16248 RVA: 0x001F9CCE File Offset: 0x001F7ECE
		protected override bool IsMenuShow
		{
			get
			{
				return this.showHideMenuTarget.gameObject.activeSelf;
			}
		}

		// Token: 0x06003F79 RID: 16249 RVA: 0x001F9CE0 File Offset: 0x001F7EE0
		public void Setup(CommonSortUiConfig uiConfig, Action onSortChanged, Action onMenuShow, Action onMenuHide)
		{
			this._onSortChanged = onSortChanged;
			this._onMenuShow = onMenuShow;
			this._onMenuHide = onMenuHide;
			this._config = uiConfig;
			this._sortState.ItemStates.Clear();
			this._sortState.ClearPendingRemoval();
			SortUiState defaultSortState = uiConfig.DefaultSortState;
			bool flag = ((defaultSortState != null) ? defaultSortState.ItemStates : null) != null;
			if (flag)
			{
				foreach (SortItemState state in uiConfig.DefaultSortState.ItemStates)
				{
					this._sortState.ItemStates.Add(new SortItemState
					{
						SortId = state.SortId,
						SortDirection = state.SortDirection
					});
				}
			}
			int menuConfigsCount = this.GetDropdownCount();
			CommonUtils.PrepareEnoughChildren(this.itemRoot, this.itemTemplate, menuConfigsCount, null);
			for (int i = 0; i < menuConfigsCount; i++)
			{
				Transform item = this.itemRoot.GetChild(i);
				CommonSortDropdownItem dropdownItem = item.GetComponent<CommonSortDropdownItem>();
				this.RefreshItem(i, dropdownItem);
			}
			this.SetupMenuBar();
			this.HideMenu();
		}

		// Token: 0x06003F7A RID: 16250 RVA: 0x001F9E30 File Offset: 0x001F8030
		public void RefreshDisplayOptions<TData>(List<FilterLineBase<TData>> filterLines, List<LineState> lineStates)
		{
			this._displayingSortIds.Clear();
			this.CollectDisplayingSortIds<TData>(filterLines, lineStates);
			int menuConfigsCount = this.GetDropdownCount();
			for (int i = 0; i < menuConfigsCount; i++)
			{
				Transform item = this.itemRoot.GetChild(i);
				CommonSortDropdownItem dropdownItem = item.GetComponent<CommonSortDropdownItem>();
				this.RefreshItem(i, dropdownItem);
			}
			bool flag = this._autoHeightHelper != null;
			if (flag)
			{
				this.CalcMaxHeight();
			}
			this.RefreshMenuBar();
		}

		// Token: 0x06003F7B RID: 16251 RVA: 0x001F9EAC File Offset: 0x001F80AC
		private void CollectDisplayingSortIds<TData>(List<FilterLineBase<TData>> filterLines, List<LineState> lineStates)
		{
			bool flag = this._config.FilterTypeDic != null;
			if (flag)
			{
				for (int i = 0; i < filterLines.Count; i++)
				{
					bool flag2 = lineStates[i].Type > ESortAndFilterOneLineType.ToggleGroup;
					if (!flag2)
					{
						List<short> sortIdList;
						bool flag3 = this._config.FilterTypeDic.TryGetValue(new ValueTuple<int, int>(filterLines[i].Id, lineStates[i].ToggleGroupState.Index), out sortIdList);
						if (flag3)
						{
							foreach (short id in sortIdList)
							{
								this._displayingSortIds.Add(id);
							}
						}
						List<short> sortIdListParent;
						bool flag4 = lineStates[i].ToggleGroupState.Index != -1 && this._config.FilterTypeDic.TryGetValue(new ValueTuple<int, int>(filterLines[i].Id, -1), out sortIdListParent);
						if (flag4)
						{
							foreach (short id2 in sortIdListParent)
							{
								this._displayingSortIds.Add(id2);
							}
						}
					}
				}
			}
			bool flag5 = this._displayingSortIds.Count == 0;
			if (flag5)
			{
				foreach (short item in this._config.DefaultSortIds)
				{
					this._displayingSortIds.Add(item);
				}
			}
		}

		// Token: 0x06003F7C RID: 16252 RVA: 0x001FA08C File Offset: 0x001F828C
		public void OnSwitchCustomOrderSettingMode(bool isOn)
		{
			this._isSettingMode = isOn;
		}

		// Token: 0x06003F7D RID: 16253 RVA: 0x001FA098 File Offset: 0x001F8298
		private void SetupMenuBar()
		{
			this._menuBarInstance = Object.Instantiate<GameObject>(this.menuBarTemplate, this.menuBarSlot);
			this._menuBarInstance.gameObject.SetActive(true);
			CommonSortMenuBar menuBar = this.MenuBar;
			menuBar.RemoveEnterEvent(new UnityAction(this.OnPointerEnter));
			menuBar.RemoveExitEvent(new UnityAction(this.OnPointerExit));
			menuBar.AddEnterEvent(new UnityAction(this.OnPointerEnter));
			menuBar.AddExitEvent(new UnityAction(this.OnPointerExit));
			this.RefreshMenuBar();
			this.RefreshMenuBarButton();
			this.clearAllButton.ClearAndAddListener(new Action(this.OnClickClearAllButton));
		}

		// Token: 0x06003F7E RID: 16254 RVA: 0x001FA148 File Offset: 0x001F8348
		private void OnPointerExit()
		{
			this._pointerTriggered = false;
		}

		// Token: 0x06003F7F RID: 16255 RVA: 0x001FA152 File Offset: 0x001F8352
		private void OnPointerEnter()
		{
			this._pointerTriggered = true;
		}

		// Token: 0x06003F80 RID: 16256 RVA: 0x001FA15C File Offset: 0x001F835C
		private void OnClickClearAllButton()
		{
			bool flag = !this.Interactable;
			if (!flag)
			{
				this._sortState.ItemStates.Clear();
				this._sortState.ClearPendingRemoval();
				this.RefreshAllItemsDisplay();
				this.RefreshMenuBar();
				Action onSortChanged = this._onSortChanged;
				if (onSortChanged != null)
				{
					onSortChanged();
				}
			}
		}

		// Token: 0x06003F81 RID: 16257 RVA: 0x001FA1B8 File Offset: 0x001F83B8
		private void RefreshItem(int index, CommonSortDropdownItem dropdownItem)
		{
			short sortId = this._config.SortIds[index];
			bool flag = this._displayingSortIds.Count != 0 && !this._displayingSortIds.Contains(sortId);
			if (flag)
			{
				dropdownItem.gameObject.SetActive(false);
			}
			else
			{
				dropdownItem.gameObject.SetActive(true);
				this.RefreshItemDisplay(index, dropdownItem);
				this.RefreshItemButton(index, dropdownItem);
				this.RefreshItemCheckBox(index, dropdownItem);
				this._menuItemsDirty = true;
			}
		}

		// Token: 0x06003F82 RID: 16258 RVA: 0x001FA23C File Offset: 0x001F843C
		private void RefreshItemCheckBox(int index, CommonSortDropdownItem dropdownItem)
		{
			CommonDirectionButton directionButton = dropdownItem.DirectionButton;
			short sortId;
			directionButton.ClearAndAddButtonListener(delegate
			{
				bool flag = !this.Interactable;
				if (!flag)
				{
					short sortId = this._config.SortIds[index];
					bool flag2 = dropdownItem.CurrentSortState == CommonSortDropdownItem.ESortState.PendingRemoval;
					if (flag2)
					{
						dropdownItem.SetNormal();
						this._sortState.RestoreStateFromPendingRemoval();
					}
					else
					{
						int myIdOrder = this._sortState.ItemStates.FindIndex((SortItemState s) => s.SortId == sortId);
						bool flag3 = myIdOrder == -1;
						if (flag3)
						{
							this._sortState.ItemStates.Add(new SortItemState
							{
								SortId = sortId,
								SortDirection = ESortDirection.Descending
							});
						}
						else
						{
							this.ChangeSortStateForCheckBox(myIdOrder, dropdownItem);
						}
					}
					this.RefreshAllItemsDisplay();
					this.RefreshMenuBar();
					Action onSortChanged = this._onSortChanged;
					if (onSortChanged != null)
					{
						onSortChanged();
					}
				}
			});
			sortId = this._config.SortIds[index];
			Predicate<SortItemState> <>9__3;
			dropdownItem.SetCheckBoxMouseExitEvent(delegate
			{
				bool flag = this._sortState.IsSortIdPendingForRemove(sortId);
				if (flag)
				{
					this._sortState.ClearPendingRemoval();
					List<SortItemState> itemStates = this._sortState.ItemStates;
					Predicate<SortItemState> match;
					if ((match = <>9__3) == null)
					{
						match = (<>9__3 = ((SortItemState s) => s.SortId == sortId));
					}
					itemStates.RemoveAll(match);
					this.RefreshAllItemsDisplay();
					this.RefreshMenuBar();
					Action onSortChanged = this._onSortChanged;
					if (onSortChanged != null)
					{
						onSortChanged();
					}
				}
			});
		}

		// Token: 0x06003F83 RID: 16259 RVA: 0x001FA2B8 File Offset: 0x001F84B8
		private void ChangeSortStateForCheckBox(int myIdOrder, CommonSortDropdownItem dropdownItem)
		{
			SortItemState state = this._sortState.ItemStates[myIdOrder];
			bool flag = state.SortDirection == ESortDirection.Descending;
			if (flag)
			{
				state.SortDirection = ESortDirection.Ascending;
				this._sortState.ItemStates[myIdOrder] = state;
			}
			else
			{
				bool flag2 = state.SortDirection == ESortDirection.Ascending;
				if (flag2)
				{
					short sortId = state.SortId;
					this._sortState.SetPendingRemoval(sortId, myIdOrder);
					dropdownItem.SetPendingRemoval();
				}
			}
		}

		// Token: 0x06003F84 RID: 16260 RVA: 0x001FA330 File Offset: 0x001F8530
		private void ChangeSortStateForButton(int myIdOrder)
		{
			SortItemState state = this._sortState.ItemStates[myIdOrder];
			bool flag = state.SortDirection == ESortDirection.Descending;
			if (flag)
			{
				state.SortDirection = ESortDirection.Ascending;
				this._sortState.ItemStates[myIdOrder] = state;
			}
			else
			{
				bool flag2 = state.SortDirection == ESortDirection.Ascending;
				if (flag2)
				{
					this._sortState.ItemStates.RemoveAt(myIdOrder);
				}
			}
		}

		// Token: 0x06003F85 RID: 16261 RVA: 0x001FA3A0 File Offset: 0x001F85A0
		private void RefreshItemButton(int index, CommonSortDropdownItem dropdownItem)
		{
			CButtonObsolete button = dropdownItem.Button;
			button.ClearAndAddListener(delegate
			{
				bool flag = !this.Interactable;
				if (!flag)
				{
					short sortId = this._config.SortIds[index];
					this._sortState.ClearPendingRemoval();
					CommonSortDropdownItem[] allItems = this.itemRoot.GetComponentsInChildren<CommonSortDropdownItem>();
					foreach (CommonSortDropdownItem item in allItems)
					{
						item.SetNormal();
					}
					int myIdOrder = this._sortState.ItemStates.FindIndex((SortItemState s) => s.SortId == sortId);
					bool flag2 = myIdOrder >= 0;
					if (flag2)
					{
						this.ChangeSortStateForButton(myIdOrder);
					}
					else
					{
						this._sortState.ItemStates.Add(new SortItemState
						{
							SortId = sortId,
							SortDirection = ESortDirection.Descending
						});
					}
					this.RefreshAllItemsDisplay();
					this.RefreshMenuBar();
					Action onSortChanged = this._onSortChanged;
					if (onSortChanged != null)
					{
						onSortChanged();
					}
				}
			});
		}

		// Token: 0x06003F86 RID: 16262 RVA: 0x001FA3DC File Offset: 0x001F85DC
		private void RefreshItemDisplay(int index, CommonSortDropdownItem dropdownItem)
		{
			short sortId = this._config.SortIds[index];
			int sortNameIndex = this._config.SortNameIndexList[index];
			SortItemItem sortConfig = SortItem.Instance[sortId];
			dropdownItem.UpdateLabels(sortConfig.Names[sortNameIndex]);
			int myIdOrder = this._sortState.ItemStates.FindIndex((SortItemState s) => s.SortId == sortId);
			bool isSorting = myIdOrder != -1;
			bool flag = this._sortState.IsSortIdPendingForRemove(sortId) || !isSorting;
			CommonDirectionButton.EDirection direction;
			if (flag)
			{
				direction = CommonDirectionButton.EDirection.None;
			}
			else
			{
				ESortDirection sortDirection = this._sortState.ItemStates[myIdOrder].SortDirection;
				if (!true)
				{
				}
				CommonDirectionButton.EDirection edirection;
				if (sortDirection != ESortDirection.Ascending)
				{
					if (sortDirection != ESortDirection.Descending)
					{
						throw new ArgumentOutOfRangeException();
					}
					edirection = CommonDirectionButton.EDirection.Down;
				}
				else
				{
					edirection = CommonDirectionButton.EDirection.Up;
				}
				if (!true)
				{
				}
				direction = edirection;
			}
			dropdownItem.DirectionButton.SetDirection(direction);
			dropdownItem.UpdateNumber(isSorting, myIdOrder + 1);
		}

		// Token: 0x06003F87 RID: 16263 RVA: 0x001FA4E8 File Offset: 0x001F86E8
		private void RefreshMenuBar()
		{
			CommonSortMenuBar menuBar = this.MenuBar;
			bool hasSortState = this._sortState.ValidStates.Count > 0;
			bool flag = hasSortState;
			string labelText;
			if (flag)
			{
				SortItemState sortState = this._sortState.ValidStates[0];
				int configIndex = this._config.SortIds.IndexOf(sortState.SortId);
				bool flag2 = configIndex >= 0 && configIndex < this._config.SortNameIndexList.Count;
				if (flag2)
				{
					SortItemItem sortConfig = SortItem.Instance[sortState.SortId];
					labelText = sortConfig.Names[this._config.SortNameIndexList[configIndex]];
				}
				else
				{
					labelText = LocalStringManager.Get(LanguageKey.LK_CommonSortAndFilter_Sort);
				}
			}
			else
			{
				labelText = LocalStringManager.Get(LanguageKey.LK_CommonSortAndFilter_Sort);
			}
			bool isMultiSortState = this._sortState.ValidStates.Count > 1;
			bool flag3 = isMultiSortState;
			if (flag3)
			{
				labelText += string.Format("({0})", this._sortState.ValidStates.Count);
			}
			menuBar.UpdateLabel(labelText);
			bool flag4 = hasSortState;
			if (flag4)
			{
				ESortDirection firstDirection = this._sortState.ValidStates[0].SortDirection;
				menuBar.UpdateSortDirectionIcon(true, firstDirection == ESortDirection.Descending);
			}
			else
			{
				menuBar.UpdateSortDirectionIcon(false, false);
			}
			this.SetClearAllButtonActive(this._sortState.ItemStates.Count > 0);
		}

		// Token: 0x06003F88 RID: 16264 RVA: 0x001FA657 File Offset: 0x001F8857
		private void SetClearAllButtonActive(bool isActive)
		{
			this.clearAllButton.gameObject.SetActive(isActive);
		}

		// Token: 0x06003F89 RID: 16265 RVA: 0x001FA66C File Offset: 0x001F886C
		private void RefreshMenuBarButton()
		{
			CButtonObsolete button = this._menuBarInstance.GetComponent<CButtonObsolete>();
			button.ClearAndAddListener(new Action(this.OnMenuBarClicked));
		}

		// Token: 0x06003F8A RID: 16266 RVA: 0x001FA69C File Offset: 0x001F889C
		private void OnMenuBarClicked()
		{
			bool isMenuShow = this.IsMenuShow;
			if (isMenuShow)
			{
				this.HideMenu();
			}
			else
			{
				this.ShowMenu();
			}
		}

		// Token: 0x06003F8B RID: 16267 RVA: 0x001FA6C8 File Offset: 0x001F88C8
		protected override void ShowMenu()
		{
			this.showHideMenuTarget.gameObject.SetActive(true);
			this.RefreshMenuBarIconDirection();
			this.CalcMaxHeight();
			this.MenuBar.SetSelected(true);
			Action onMenuShow = this._onMenuShow;
			if (onMenuShow != null)
			{
				onMenuShow();
			}
		}

		// Token: 0x06003F8C RID: 16268 RVA: 0x001FA718 File Offset: 0x001F8918
		public override void HideMenu()
		{
			bool hasPendingRemoval = this._sortState.HasPendingRemoval;
			if (hasPendingRemoval)
			{
				this._sortState.ClearPendingRemoval();
				this.RefreshAllItemsDisplay();
				this.RefreshMenuBar();
				Action onSortChanged = this._onSortChanged;
				if (onSortChanged != null)
				{
					onSortChanged();
				}
			}
			this.showHideMenuTarget.gameObject.SetActive(false);
			this.RefreshMenuBarIconDirection();
			this.MenuBar.SetSelected(false);
			Action onMenuHide = this._onMenuHide;
			if (onMenuHide != null)
			{
				onMenuHide();
			}
		}

		// Token: 0x06003F8D RID: 16269 RVA: 0x001FA79C File Offset: 0x001F899C
		private void CalcMaxHeight()
		{
			VerticalLayoutGroup layout = this.itemRoot.GetComponent<VerticalLayoutGroup>();
			LayoutRebuilder.ForceRebuildLayoutImmediate(this.itemRoot);
			RectOffset padding = layout.padding;
			float spacing = layout.spacing;
			float itemHeight = this.itemTemplate.GetComponent<RectTransform>().rect.height;
			int itemCount = this.GetDropdownDisplayCount();
			this._autoHeightHelper.maxHeight = (float)(padding.top + padding.bottom) + itemHeight * (float)itemCount + spacing * (float)(itemCount - 1);
		}

		// Token: 0x06003F8E RID: 16270 RVA: 0x001FA81C File Offset: 0x001F8A1C
		private void RefreshMenuBarIconDirection()
		{
			CommonSortMenuBar menuBar = this.MenuBar;
			menuBar.UpdateStatusIcon(this.IsMenuShow);
		}

		// Token: 0x06003F8F RID: 16271 RVA: 0x001FA840 File Offset: 0x001F8A40
		private int GetDropdownCount()
		{
			return this._config.SortIds.Count;
		}

		// Token: 0x06003F90 RID: 16272 RVA: 0x001FA864 File Offset: 0x001F8A64
		private int GetDropdownDisplayCount()
		{
			return this._displayingSortIds.Count;
		}

		// Token: 0x06003F91 RID: 16273 RVA: 0x001FA884 File Offset: 0x001F8A84
		public SortStateData GetSortData()
		{
			return new SortStateData
			{
				ItemStates = this._sortState.AllStates
			};
		}

		// Token: 0x06003F92 RID: 16274 RVA: 0x001FA8AC File Offset: 0x001F8AAC
		public void SetSortData(SortStateData data)
		{
			this._sortState.ItemStates.Clear();
			this._sortState.ClearPendingRemoval();
			bool flag = ((data != null) ? data.ItemStates : null) != null;
			if (flag)
			{
				foreach (SortItemState state in data.ItemStates)
				{
					this._sortState.ItemStates.Add(new SortItemState
					{
						SortId = state.SortId,
						SortDirection = state.SortDirection
					});
				}
			}
			this.RefreshAllItemsDisplay();
			this.RefreshMenuBar();
			Action onSortChanged = this._onSortChanged;
			if (onSortChanged != null)
			{
				onSortChanged();
			}
		}

		// Token: 0x06003F93 RID: 16275 RVA: 0x001FA988 File Offset: 0x001F8B88
		private void RefreshAllItemsDisplay()
		{
			for (int i = 0; i < this.GetDropdownCount(); i++)
			{
				Transform item = this.itemRoot.GetChild(i);
				CommonSortDropdownItem dropdownItem = item.GetComponent<CommonSortDropdownItem>();
				bool activeSelf = dropdownItem.gameObject.activeSelf;
				if (activeSelf)
				{
					short sortId = this._config.SortIds[i];
					bool flag = this._sortState.IsSortIdPendingForRemove(sortId);
					if (flag)
					{
						dropdownItem.SetPendingRemoval();
					}
					else
					{
						dropdownItem.SetNormal();
					}
					this.RefreshItemDisplay(i, dropdownItem);
				}
			}
		}

		// Token: 0x06003F94 RID: 16276 RVA: 0x001FAA18 File Offset: 0x001F8C18
		protected override void Update()
		{
			base.Update();
			bool menuItemsDirty = this._menuItemsDirty;
			if (menuItemsDirty)
			{
				this.CalculateMenuItemButtonMaxWidth();
				this._menuItemsDirty = false;
			}
			this.UpdateMenuBarWidth();
		}

		// Token: 0x06003F95 RID: 16277 RVA: 0x001FAA50 File Offset: 0x001F8C50
		private void CalculateMenuItemButtonMaxWidth()
		{
			float currentMaxWidth = 0f;
			int menuConfigsCount = this.GetDropdownCount();
			for (int i = 0; i < menuConfigsCount; i++)
			{
				Transform item = this.itemRoot.GetChild(i);
				CommonSortDropdownItem dropdownItem = item.GetComponent<CommonSortDropdownItem>();
				currentMaxWidth = Mathf.Max(currentMaxWidth, dropdownItem.GetButtonWidth());
			}
			this._maxButtonWidth = new float?(currentMaxWidth);
		}

		// Token: 0x06003F96 RID: 16278 RVA: 0x001FAAB0 File Offset: 0x001F8CB0
		private void UpdateMenuBarWidth()
		{
			RectTransform myRect = base.GetComponent<RectTransform>();
			bool isActive = this.clearAllButton.gameObject.activeSelf;
			float clearAllButtonWidth = this.clearAllButton.GetComponent<RectTransform>().rect.width;
			myRect.SetSize(new Vector2(this.MenuBarExpectedWidth + (isActive ? clearAllButtonWidth : 0f), myRect.rect.height));
			this._menuBarInstance.GetComponent<RectTransform>().SetSize(new Vector2(this.MenuBarExpectedWidth, myRect.rect.height));
		}

		// Token: 0x06003F97 RID: 16279 RVA: 0x001FAB45 File Offset: 0x001F8D45
		protected override void CheckShowMenu()
		{
		}

		// Token: 0x06003F98 RID: 16280 RVA: 0x001FAB48 File Offset: 0x001F8D48
		protected override void BeforeFadeFakeMask(bool goFakeHide)
		{
			base.BeforeFadeFakeMask(goFakeHide);
			CanvasGroup viewPortCanvasGroup = this.viewport.GetComponent<CanvasGroup>();
			viewPortCanvasGroup.alpha = (float)(goFakeHide ? 1 : 0);
			foreach (CanvasGroup extraCanvasGroup in this.extraFadeTargets)
			{
				extraCanvasGroup.alpha = (float)(goFakeHide ? 0 : 1);
			}
		}

		// Token: 0x06003F99 RID: 16281 RVA: 0x001FABA4 File Offset: 0x001F8DA4
		protected override void JoinFadeFakeMask(Sequence fakeMaskTween, bool goFakeHide)
		{
			base.JoinFadeFakeMask(fakeMaskTween, goFakeHide);
			this._fakeMaskTween.Join(this.viewport.GetComponent<CanvasGroup>().DOFade((float)(goFakeHide ? 0 : 1), 0.2f));
			foreach (CanvasGroup extraCanvasGroup in this.extraFadeTargets)
			{
				this._fakeMaskTween.Join(extraCanvasGroup.DOFade((float)(goFakeHide ? 0 : 1), 0.2f));
			}
		}

		// Token: 0x06003F9A RID: 16282 RVA: 0x001FAC20 File Offset: 0x001F8E20
		protected override void FadeFakeMaskOver(bool goFakeHide)
		{
			base.FadeFakeMaskOver(goFakeHide);
			this.viewport.GetComponent<CanvasGroup>().alpha = (float)(goFakeHide ? 0 : 1);
			foreach (CanvasGroup extraCanvasGroup in this.extraFadeTargets)
			{
				extraCanvasGroup.alpha = (float)(goFakeHide ? 0 : 1);
			}
		}

		// Token: 0x1700067A RID: 1658
		// (get) Token: 0x06003F9B RID: 16283 RVA: 0x001FAC7A File Offset: 0x001F8E7A
		private bool Interactable
		{
			get
			{
				return !this._isSettingMode;
			}
		}

		// Token: 0x1700067B RID: 1659
		// (get) Token: 0x06003F9C RID: 16284 RVA: 0x001FAC88 File Offset: 0x001F8E88
		// (set) Token: 0x06003F9D RID: 16285 RVA: 0x001FACA0 File Offset: 0x001F8EA0
		public bool DisableAutoHeight
		{
			get
			{
				return this._disableAutoHeight;
			}
			set
			{
				this._disableAutoHeight = value;
				this.RefreshAutoHeightHelperEnabled();
			}
		}

		// Token: 0x04002D63 RID: 11619
		private CommonSortUiConfig _config;

		// Token: 0x04002D64 RID: 11620
		private GameObject _menuBarInstance;

		// Token: 0x04002D65 RID: 11621
		private ScrollRectAutoHeightHelper _autoHeightHelper;

		// Token: 0x04002D66 RID: 11622
		private bool _isSettingMode;

		// Token: 0x04002D67 RID: 11623
		private bool _menuItemsDirty;

		// Token: 0x04002D68 RID: 11624
		private bool _pointerTriggered;

		// Token: 0x04002D69 RID: 11625
		private float? _maxButtonWidth;

		// Token: 0x04002D6A RID: 11626
		private readonly SortUiState _sortState = new SortUiState
		{
			ItemStates = new List<SortItemState>()
		};

		// Token: 0x04002D6B RID: 11627
		private Action _onSortChanged;

		// Token: 0x04002D6C RID: 11628
		private Action _onMenuShow;

		// Token: 0x04002D6D RID: 11629
		private Action _onMenuHide;

		// Token: 0x04002D6E RID: 11630
		[SerializeField]
		private RectTransform showHideMenuTarget;

		// Token: 0x04002D6F RID: 11631
		[SerializeField]
		private RectTransform scrollRect;

		// Token: 0x04002D70 RID: 11632
		[SerializeField]
		private RectTransform viewport;

		// Token: 0x04002D71 RID: 11633
		[SerializeField]
		private RectTransform itemRoot;

		// Token: 0x04002D72 RID: 11634
		[SerializeField]
		protected GameObject menuBarTemplate;

		// Token: 0x04002D73 RID: 11635
		[SerializeField]
		protected GameObject itemTemplate;

		// Token: 0x04002D74 RID: 11636
		[SerializeField]
		protected RectTransform menuBarSlot;

		// Token: 0x04002D75 RID: 11637
		[SerializeField]
		private CButtonObsolete clearAllButton;

		// Token: 0x04002D76 RID: 11638
		[SerializeField]
		private CanvasGroup[] extraFadeTargets;

		// Token: 0x04002D77 RID: 11639
		[Header("基础宽度。即不显示ClearAllButton时的宽度")]
		[SerializeField]
		private float baseWidth = 180f;

		// Token: 0x04002D78 RID: 11640
		private bool _disableAutoHeight;
	}
}
