using System;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CCE RID: 3278
	public class SortDropdown : MonoBehaviour, ISortUiWithDisplayingSortIds, ISortUi
	{
		// Token: 0x17001144 RID: 4420
		// (get) Token: 0x0600A59C RID: 42396 RVA: 0x004D2D0C File Offset: 0x004D0F0C
		public HashSet<short> DisplayingSortIds
		{
			get
			{
				return this._sortState.DisplayingSortIds;
			}
		}

		// Token: 0x17001145 RID: 4421
		// (get) Token: 0x0600A59D RID: 42397 RVA: 0x004D2D19 File Offset: 0x004D0F19
		private SortMenuBar MenuBar
		{
			get
			{
				return this._menuBarInstance.GetComponent<SortMenuBar>();
			}
		}

		// Token: 0x17001146 RID: 4422
		// (get) Token: 0x0600A59E RID: 42398 RVA: 0x004D2D26 File Offset: 0x004D0F26
		private bool FakeMaskTweenActive
		{
			get
			{
				return this._fakeMaskTween != null && this._fakeMaskTween.IsActive() && !this._fakeMaskTween.IsComplete();
			}
		}

		// Token: 0x17001147 RID: 4423
		// (get) Token: 0x0600A59F RID: 42399 RVA: 0x004D2D4E File Offset: 0x004D0F4E
		private bool Interactable
		{
			get
			{
				return !this._isSettingMode;
			}
		}

		// Token: 0x17001148 RID: 4424
		// (get) Token: 0x0600A5A0 RID: 42400 RVA: 0x004D2D59 File Offset: 0x004D0F59
		private bool IsMenuShow
		{
			get
			{
				return this.showHideMenuTarget.gameObject.activeSelf;
			}
		}

		// Token: 0x0600A5A1 RID: 42401 RVA: 0x004D2D6C File Offset: 0x004D0F6C
		private void Awake()
		{
			this._fakeHideMarkCanvasGroup = this.fakeHideMark.gameObject.GetComponent<CanvasGroup>();
			this._fakeHideMarkCanvasGroup.alpha = 0f;
			if (this.ParentElement == null)
			{
				UIBase componentInParent = base.GetComponentInParent<UIBase>();
				this.ParentElement = ((componentInParent != null) ? componentInParent.Element : null);
			}
		}

		// Token: 0x0600A5A2 RID: 42402 RVA: 0x004D2DC4 File Offset: 0x004D0FC4
		private void Update()
		{
			bool isMenuShow = this.IsMenuShow;
			if (isMenuShow)
			{
				bool flag = Time.frameCount == this._skipHideCheckFrame;
				if (flag)
				{
					this.CheckFakeHideMenu();
				}
				else
				{
					bool flag2 = !this.CheckFakeHideMenu();
					if (flag2)
					{
						this.CheckHideMenu();
					}
				}
			}
			this.UpdateMenuBarWidth();
		}

		// Token: 0x0600A5A3 RID: 42403 RVA: 0x004D2E18 File Offset: 0x004D1018
		public void Setup(SortUiConfig uiConfig, Action onSortChanged, Action onMenuShow, Action onMenuHide)
		{
			this._onSortChanged = onSortChanged;
			this._onMenuShow = onMenuShow;
			this._onMenuHide = onMenuHide;
			this._config = uiConfig;
			this._sortState.ItemStates.Clear();
			this._sortState.ClearPendingRemoval();
			bool flag = uiConfig.DefaultSortState.ItemStates != null;
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
				SortDropdownItem dropdownItem = item.GetComponent<SortDropdownItem>();
				this.RefreshItem(i, dropdownItem);
			}
			this.SetupMenuBar();
			this.HideMenu();
		}

		// Token: 0x0600A5A4 RID: 42404 RVA: 0x004D2F60 File Offset: 0x004D1160
		public SortStateData GetSortData()
		{
			return new SortStateData
			{
				ItemStates = this._sortState.AllStates
			};
		}

		// Token: 0x0600A5A5 RID: 42405 RVA: 0x004D2F88 File Offset: 0x004D1188
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

		// Token: 0x0600A5A6 RID: 42406 RVA: 0x004D3064 File Offset: 0x004D1264
		public void RefreshDisplayOptions<TData>(List<FilterLineBase<TData>> filterLines, List<LineState> lineStates)
		{
			this.DisplayingSortIds.Clear();
			this.CollectDisplayingSortIds<TData>(filterLines, lineStates);
			int menuConfigsCount = this.GetDropdownCount();
			for (int i = 0; i < menuConfigsCount; i++)
			{
				Transform item = this.itemRoot.GetChild(i);
				SortDropdownItem dropdownItem = item.GetComponent<SortDropdownItem>();
				this.RefreshItem(i, dropdownItem);
			}
			this.RefreshMenuBar();
		}

		// Token: 0x0600A5A7 RID: 42407 RVA: 0x004D30C6 File Offset: 0x004D12C6
		public void OnSwitchCustomOrderSettingMode(bool isOn)
		{
			this._isSettingMode = isOn;
		}

		// Token: 0x0600A5A8 RID: 42408 RVA: 0x004D30D0 File Offset: 0x004D12D0
		public void HideMenu()
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

		// Token: 0x0600A5A9 RID: 42409 RVA: 0x004D3154 File Offset: 0x004D1354
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
								this.DisplayingSortIds.Add(id);
							}
						}
						List<short> sortIdListParent;
						bool flag4 = lineStates[i].ToggleGroupState.Index != -1 && this._config.FilterTypeDic.TryGetValue(new ValueTuple<int, int>(filterLines[i].Id, -1), out sortIdListParent);
						if (flag4)
						{
							foreach (short id2 in sortIdListParent)
							{
								this.DisplayingSortIds.Add(id2);
							}
						}
					}
				}
			}
			bool flag5 = this.DisplayingSortIds.Count == 0;
			if (flag5)
			{
				foreach (short item in this._config.DefaultSortIds)
				{
					this.DisplayingSortIds.Add(item);
				}
			}
		}

		// Token: 0x0600A5AA RID: 42410 RVA: 0x004D3334 File Offset: 0x004D1534
		private void SetupMenuBar()
		{
			bool flag = this._menuBarInstance != null;
			if (flag)
			{
				Object.Destroy(this._menuBarInstance);
			}
			this._menuBarInstance = Object.Instantiate<GameObject>(this.menuBarTemplate, this.menuBarSlot);
			this._menuBarInstance.gameObject.SetActive(true);
			SortMenuBar menuBar = this.MenuBar;
			menuBar.RemoveEnterEvent(new UnityAction(this.OnPointerEnter));
			menuBar.RemoveExitEvent(new UnityAction(this.OnPointerExit));
			menuBar.AddEnterEvent(new UnityAction(this.OnPointerEnter));
			menuBar.AddExitEvent(new UnityAction(this.OnPointerExit));
			this.RefreshMenuBar();
			this.clearAllButton.ClearAndAddListener(new Action(this.OnClickClearAllButton));
			menuBar.SetOnChangeSortOrder(delegate(bool isEsc)
			{
				bool flag2 = !this.Interactable;
				if (!flag2)
				{
					for (int i = 0; i < this._sortState.ItemStates.Count; i++)
					{
						SortItemState state = this._sortState.ItemStates[i];
						state.SortDirection = (isEsc ? ESortDirection.Ascending : ESortDirection.Descending);
						this._sortState.ItemStates[i] = state;
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

		// Token: 0x0600A5AB RID: 42411 RVA: 0x004D340C File Offset: 0x004D160C
		private void OnPointerExit()
		{
		}

		// Token: 0x0600A5AC RID: 42412 RVA: 0x004D3410 File Offset: 0x004D1610
		private void OnPointerEnter()
		{
			bool flag = !this.Interactable;
			if (!flag)
			{
				bool flag2 = this.ShouldBlockShowMenuDueToOtherMenuFakeHide();
				if (!flag2)
				{
					bool flag3 = this.IsMenuShow && this._isInFakeHideState;
					if (flag3)
					{
						this._skipHideCheckFrame = Time.frameCount;
					}
					this.ShowMenu();
				}
			}
		}

		// Token: 0x0600A5AD RID: 42413 RVA: 0x004D3460 File Offset: 0x004D1660
		private bool ShouldBlockShowMenuDueToOtherMenuFakeHide()
		{
			SortDropdown[] allSortDropdowns = Object.FindObjectsOfType<SortDropdown>();
			foreach (SortDropdown dropdown in allSortDropdowns)
			{
				bool flag = dropdown == this;
				if (!flag)
				{
					bool flag2 = dropdown.IsMenuShow && dropdown._isInFakeHideState;
					if (flag2)
					{
						return true;
					}
				}
			}
			FilterDropdown[] allFilterDropdowns = Object.FindObjectsOfType<FilterDropdown>();
			foreach (FilterDropdown dropdown2 in allFilterDropdowns)
			{
				bool flag3 = dropdown2.IsMenuShow && dropdown2.IsInFakeHideState;
				if (flag3)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600A5AE RID: 42414 RVA: 0x004D3508 File Offset: 0x004D1708
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

		// Token: 0x0600A5AF RID: 42415 RVA: 0x004D3564 File Offset: 0x004D1764
		private void RefreshItem(int index, SortDropdownItem dropdownItem)
		{
			short sortId = this._config.SortIds[index];
			bool flag = this.DisplayingSortIds.Count != 0 && !this.DisplayingSortIds.Contains(sortId);
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
			}
		}

		// Token: 0x0600A5B0 RID: 42416 RVA: 0x004D35E0 File Offset: 0x004D17E0
		private void RefreshItemCheckBox(int index, SortDropdownItem dropdownItem)
		{
			short sortId = this._config.SortIds[index];
			Predicate<SortItemState> <>9__1;
			dropdownItem.SetCheckBoxMouseExitEvent(delegate
			{
				bool flag = this._sortState.IsSortIdPendingForRemove(sortId);
				if (flag)
				{
					this._sortState.ClearPendingRemoval();
					List<SortItemState> itemStates = this._sortState.ItemStates;
					Predicate<SortItemState> match;
					if ((match = <>9__1) == null)
					{
						match = (<>9__1 = ((SortItemState s) => s.SortId == sortId));
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

		// Token: 0x0600A5B1 RID: 42417 RVA: 0x004D3628 File Offset: 0x004D1828
		private void ChangeSortStateForCheckBox(int myIdOrder, SortDropdownItem dropdownItem)
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

		// Token: 0x0600A5B2 RID: 42418 RVA: 0x004D36A0 File Offset: 0x004D18A0
		private void ChangeSortStateForButton(int myIdOrder)
		{
			SortItemState state = this._sortState.ItemStates[myIdOrder];
			bool flag = state.SortDirection > ESortDirection.None;
			if (flag)
			{
				this._sortState.ItemStates.RemoveAt(myIdOrder);
			}
		}

		// Token: 0x0600A5B3 RID: 42419 RVA: 0x004D36E4 File Offset: 0x004D18E4
		private void RefreshItemButton(int index, SortDropdownItem dropdownItem)
		{
			CButton button = dropdownItem.Button;
			button.ClearAndAddListener(delegate
			{
				bool flag = !this.Interactable;
				if (!flag)
				{
					short sortId = this._config.SortIds[index];
					this._sortState.ClearPendingRemoval();
					SortDropdownItem[] allItems = this.itemRoot.GetComponentsInChildren<SortDropdownItem>();
					foreach (SortDropdownItem item in allItems)
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
							SortDirection = (this.MenuBar.IsEsc ? ESortDirection.Ascending : ESortDirection.Descending)
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

		// Token: 0x0600A5B4 RID: 42420 RVA: 0x004D3720 File Offset: 0x004D1920
		private void RefreshItemDisplay(int index, SortDropdownItem dropdownItem)
		{
			short sortId = this._config.SortIds[index];
			int sortNameIndex = this._config.SortNameIndexList[index];
			SortItemItem sortConfig = SortItem.Instance[sortId];
			dropdownItem.UpdateLabels(sortConfig.Names[sortNameIndex]);
			int myIdOrder = this._sortState.ItemStates.FindIndex((SortItemState s) => s.SortId == sortId);
			bool isSorting = myIdOrder != -1;
			bool flag = this._sortState.IsSortIdPendingForRemove(sortId) || !isSorting;
			if (!flag)
			{
				ESortDirection direction = this._sortState.ItemStates[myIdOrder].SortDirection;
			}
			dropdownItem.UpdateNumber(isSorting, myIdOrder + 1);
		}

		// Token: 0x0600A5B5 RID: 42421 RVA: 0x004D37F8 File Offset: 0x004D19F8
		private void UpdateMenuBarWidth()
		{
			bool flag = this._menuBarInstance == null;
			if (!flag)
			{
				RectTransform myRect = base.GetComponent<RectTransform>();
				bool isActive = this.clearAllButton.gameObject.activeSelf;
				float clearAllButtonWidth = this.clearAllButton.GetComponent<RectTransform>().rect.width;
				myRect.SetSize(new Vector2(this.FixedWidth + (isActive ? clearAllButtonWidth : 0f), myRect.rect.height));
				this._menuBarInstance.GetComponent<RectTransform>().SetSize(new Vector2(this.FixedWidth, myRect.rect.height));
			}
		}

		// Token: 0x0600A5B6 RID: 42422 RVA: 0x004D38A4 File Offset: 0x004D1AA4
		private void RefreshMenuBar()
		{
			SortMenuBar menuBar = this.MenuBar;
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

		// Token: 0x0600A5B7 RID: 42423 RVA: 0x004D3A13 File Offset: 0x004D1C13
		private void SetClearAllButtonActive(bool isActive)
		{
			this.clearAllButton.gameObject.SetActive(isActive);
		}

		// Token: 0x0600A5B8 RID: 42424 RVA: 0x004D3A28 File Offset: 0x004D1C28
		private void ShowMenu()
		{
			this.showHideMenuTarget.gameObject.SetActive(true);
			this.RefreshMenuBarIconDirection();
			this.MenuBar.SetSelected(true);
			Action onMenuShow = this._onMenuShow;
			if (onMenuShow != null)
			{
				onMenuShow();
			}
		}

		// Token: 0x0600A5B9 RID: 42425 RVA: 0x004D3A64 File Offset: 0x004D1C64
		private void RefreshMenuBarIconDirection()
		{
			SortMenuBar menuBar = this.MenuBar;
			menuBar.SetSelected(this.IsMenuShow);
		}

		// Token: 0x0600A5BA RID: 42426 RVA: 0x004D3A88 File Offset: 0x004D1C88
		private int GetDropdownCount()
		{
			return this._config.SortIds.Count;
		}

		// Token: 0x0600A5BB RID: 42427 RVA: 0x004D3AAC File Offset: 0x004D1CAC
		private void RefreshAllItemsDisplay()
		{
			for (int i = 0; i < this.GetDropdownCount(); i++)
			{
				Transform item = this.itemRoot.GetChild(i);
				SortDropdownItem dropdownItem = item.GetComponent<SortDropdownItem>();
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

		// Token: 0x0600A5BC RID: 42428 RVA: 0x004D3B3C File Offset: 0x004D1D3C
		private bool NeedFakeHideMenu()
		{
			bool shortCutClicked = this.ParentElement != null && CommonCommandKit.Alt.Check(this.ParentElement, true, false, false, true, false);
			bool flag = shortCutClicked;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				foreach (RectTransform rect in this.checkFakeHideRects)
				{
					bool flag2 = !rect.gameObject.activeSelf;
					if (!flag2)
					{
						bool flag3 = SortDropdown.IsMouseInRect(rect);
						if (flag3)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x0600A5BD RID: 42429 RVA: 0x004D3BC4 File Offset: 0x004D1DC4
		private bool CheckFakeHideMenu()
		{
			bool needFakeHideMenu = this.NeedFakeHideMenu();
			bool shortCutClicked = this.ParentElement != null && CommonCommandKit.Alt.Check(this.ParentElement, true, false, false, true, false);
			bool flag = needFakeHideMenu || shortCutClicked;
			if (flag)
			{
				bool flag2 = this._fakeMaskState != SortDropdown.EFakeMaskState.PendingHighlight && this._fakeMaskState != SortDropdown.EFakeMaskState.HighlightMask;
				if (flag2)
				{
					this.UpdateFakeHideGlobalState(true);
					this.DoFadeFakeMask(true);
				}
			}
			else
			{
				bool flag3 = this._fakeMaskState != SortDropdown.EFakeMaskState.PendingNormal && this._fakeMaskState != SortDropdown.EFakeMaskState.NormalMask;
				if (flag3)
				{
					this.UpdateFakeHideGlobalState(false);
					this.DoFadeFakeMask(false);
				}
			}
			return needFakeHideMenu;
		}

		// Token: 0x0600A5BE RID: 42430 RVA: 0x004D3C6C File Offset: 0x004D1E6C
		private void CheckHideMenu()
		{
			foreach (RectTransform rect in this.checkExitRects)
			{
				bool flag = !rect.gameObject.activeSelf;
				if (!flag)
				{
					bool flag2 = SortDropdown.IsMouseInRect(rect);
					if (flag2)
					{
						return;
					}
				}
			}
			this.HideMenu();
		}

		// Token: 0x0600A5BF RID: 42431 RVA: 0x004D3CC0 File Offset: 0x004D1EC0
		private static bool IsMouseInRect(RectTransform rect)
		{
			Vector2 localPos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Input.mousePosition, UIManager.Instance.UiCamera, out localPos);
			return rect.rect.Contains(localPos);
		}

		// Token: 0x0600A5C0 RID: 42432 RVA: 0x004D3D00 File Offset: 0x004D1F00
		private void DoFadeFakeMask(bool goFakeHide)
		{
			bool fakeMaskTweenActive = this.FakeMaskTweenActive;
			if (fakeMaskTweenActive)
			{
				this._fakeMaskTween.Kill(true);
			}
			this._fakeMaskTween = DOTween.Sequence();
			this._fakeMaskState = (goFakeHide ? SortDropdown.EFakeMaskState.PendingHighlight : SortDropdown.EFakeMaskState.PendingNormal);
			this.BeforeFadeFakeMask(goFakeHide);
			this._fakeHideMarkCanvasGroup.alpha = (goFakeHide ? 0f : 0.4f);
			this._fakeMaskTween.Join(this._fakeHideMarkCanvasGroup.DOFade(goFakeHide ? 0.4f : 0f, 0.2f));
			this.JoinFadeFakeMask(this._fakeMaskTween, goFakeHide);
			this._fakeMaskTween.OnComplete(delegate
			{
				this.FadeFakeMaskOver(goFakeHide);
				this._fakeHideMarkCanvasGroup.alpha = (goFakeHide ? 0.4f : 0f);
				this._fakeMaskState = (goFakeHide ? SortDropdown.EFakeMaskState.HighlightMask : SortDropdown.EFakeMaskState.NormalMask);
			});
		}

		// Token: 0x0600A5C1 RID: 42433 RVA: 0x004D3DE0 File Offset: 0x004D1FE0
		private void BeforeFadeFakeMask(bool goFakeHide)
		{
			CanvasGroup viewPortCanvasGroup = this.viewport.GetComponent<CanvasGroup>();
			viewPortCanvasGroup.alpha = (float)(goFakeHide ? 1 : 0);
			foreach (CanvasGroup extraCanvasGroup in this.extraFadeTargets)
			{
				extraCanvasGroup.alpha = (float)(goFakeHide ? 0 : 1);
			}
		}

		// Token: 0x0600A5C2 RID: 42434 RVA: 0x004D3E34 File Offset: 0x004D2034
		private void JoinFadeFakeMask(Sequence fakeMaskTween, bool goFakeHide)
		{
			fakeMaskTween.Join(this.viewport.GetComponent<CanvasGroup>().DOFade((float)(goFakeHide ? 0 : 1), 0.2f));
			foreach (CanvasGroup extraCanvasGroup in this.extraFadeTargets)
			{
				fakeMaskTween.Join(extraCanvasGroup.DOFade((float)(goFakeHide ? 0 : 1), 0.2f));
			}
		}

		// Token: 0x0600A5C3 RID: 42435 RVA: 0x004D3E9C File Offset: 0x004D209C
		private void FadeFakeMaskOver(bool goFakeHide)
		{
			this.viewport.GetComponent<CanvasGroup>().alpha = (float)(goFakeHide ? 0 : 1);
			foreach (CanvasGroup extraCanvasGroup in this.extraFadeTargets)
			{
				extraCanvasGroup.alpha = (float)(goFakeHide ? 0 : 1);
			}
		}

		// Token: 0x0600A5C4 RID: 42436 RVA: 0x004D3EF0 File Offset: 0x004D20F0
		private void UpdateFakeHideGlobalState(bool isFakeHide)
		{
			if (isFakeHide)
			{
				bool flag = !this._isInFakeHideState;
				if (flag)
				{
					this._isInFakeHideState = true;
				}
			}
			else
			{
				bool isInFakeHideState = this._isInFakeHideState;
				if (isInFakeHideState)
				{
					this._isInFakeHideState = false;
				}
			}
		}

		// Token: 0x040082C5 RID: 33477
		private SortUiConfig _config;

		// Token: 0x040082C6 RID: 33478
		private readonly SortDropdownState _sortState = new SortDropdownState
		{
			ItemStates = new List<SortItemState>()
		};

		// Token: 0x040082C7 RID: 33479
		private GameObject _menuBarInstance;

		// Token: 0x040082C8 RID: 33480
		private bool _isSettingMode;

		// Token: 0x040082C9 RID: 33481
		public float FixedWidth = 300f;

		// Token: 0x040082CA RID: 33482
		private CanvasGroup _fakeHideMarkCanvasGroup;

		// Token: 0x040082CB RID: 33483
		private bool _isInFakeHideState;

		// Token: 0x040082CC RID: 33484
		private int _skipHideCheckFrame = -1;

		// Token: 0x040082CD RID: 33485
		private Sequence _fakeMaskTween;

		// Token: 0x040082CE RID: 33486
		private SortDropdown.EFakeMaskState _fakeMaskState = SortDropdown.EFakeMaskState.None;

		// Token: 0x040082CF RID: 33487
		private const float FakeMaskHighlightAlpha = 0.4f;

		// Token: 0x040082D0 RID: 33488
		private const float FakeMaskNormalAlpha = 0f;

		// Token: 0x040082D1 RID: 33489
		private const float FakeMaskTweenDuration = 0.2f;

		// Token: 0x040082D2 RID: 33490
		public UIElement ParentElement;

		// Token: 0x040082D3 RID: 33491
		private Action _onSortChanged;

		// Token: 0x040082D4 RID: 33492
		private Action _onMenuShow;

		// Token: 0x040082D5 RID: 33493
		private Action _onMenuHide;

		// Token: 0x040082D6 RID: 33494
		[Header("菜单显示隐藏")]
		[SerializeField]
		private RectTransform showHideMenuTarget;

		// Token: 0x040082D7 RID: 33495
		[SerializeField]
		private RectTransform scrollRect;

		// Token: 0x040082D8 RID: 33496
		[SerializeField]
		private RectTransform viewport;

		// Token: 0x040082D9 RID: 33497
		[SerializeField]
		private RectTransform itemRoot;

		// Token: 0x040082DA RID: 33498
		[Header("模板")]
		[SerializeField]
		private GameObject menuBarTemplate;

		// Token: 0x040082DB RID: 33499
		[SerializeField]
		private GameObject itemTemplate;

		// Token: 0x040082DC RID: 33500
		[Header("插槽")]
		[SerializeField]
		private RectTransform menuBarSlot;

		// Token: 0x040082DD RID: 33501
		[Header("按钮")]
		[SerializeField]
		private CButton clearAllButton;

		// Token: 0x040082DE RID: 33502
		[Header("FakeHide")]
		[SerializeField]
		private RectTransform[] checkFakeHideRects;

		// Token: 0x040082DF RID: 33503
		[SerializeField]
		private RectTransform[] checkExitRects;

		// Token: 0x040082E0 RID: 33504
		[SerializeField]
		private RectTransform fakeHideMark;

		// Token: 0x040082E1 RID: 33505
		[SerializeField]
		private CanvasGroup[] extraFadeTargets;

		// Token: 0x02002414 RID: 9236
		private enum EFakeMaskState
		{
			// Token: 0x0400E152 RID: 57682
			None,
			// Token: 0x0400E153 RID: 57683
			PendingNormal,
			// Token: 0x0400E154 RID: 57684
			NormalMask,
			// Token: 0x0400E155 RID: 57685
			PendingHighlight,
			// Token: 0x0400E156 RID: 57686
			HighlightMask
		}
	}
}
