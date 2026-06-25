using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000435 RID: 1077
	public class DetailedFilterMenuBarDragHandler
	{
		// Token: 0x17000684 RID: 1668
		// (get) Token: 0x06003FD5 RID: 16341 RVA: 0x001FBE30 File Offset: 0x001FA030
		public bool IsSettingMode
		{
			get
			{
				return this._isSettingMode;
			}
		}

		// Token: 0x17000685 RID: 1669
		// (get) Token: 0x06003FD6 RID: 16342 RVA: 0x001FBE38 File Offset: 0x001FA038
		public bool IsInDragMode
		{
			get
			{
				return this._currentState == DetailedFilterMenuBarDragHandler.MenuDragState.Dragging;
			}
		}

		// Token: 0x06003FD7 RID: 16343 RVA: 0x001FBE44 File Offset: 0x001FA044
		public DetailedFilterMenuBarDragHandler(ScrollRect scrollRect, HorizontalLayoutGroup layoutGroup, RectTransform layoutMask, List<DetailFilterMultiSelectDropdown> dropdowns, Action onDragCompleted, Action<int> onBarSwapToggleShowRequest)
		{
			this._scrollRect = scrollRect;
			this._layoutGroup = layoutGroup;
			this._layoutMask = layoutMask;
			this._layoutArea = layoutGroup.GetComponent<RectTransform>();
			this._dropdowns = dropdowns;
			this._onDragCompleted = onDragCompleted;
			this._onBarSwapToggleShowRequest = onBarSwapToggleShowRequest;
		}

		// Token: 0x06003FD8 RID: 16344 RVA: 0x001FBEF8 File Offset: 0x001FA0F8
		public void TickHoldDrag()
		{
			bool flag = this._currentState == DetailedFilterMenuBarDragHandler.MenuDragState.Idle;
			if (!flag)
			{
				float currentTime = Time.realtimeSinceStartup;
				float elapsedTime = currentTime - this._pointerDownTime;
				bool flag2 = this._isSettingMode && this._currentState == DetailedFilterMenuBarDragHandler.MenuDragState.Waiting;
				if (flag2)
				{
					this._currentState = DetailedFilterMenuBarDragHandler.MenuDragState.Dragging;
					this.OnStartDrag();
				}
				else
				{
					bool flag3 = this._currentState == DetailedFilterMenuBarDragHandler.MenuDragState.Waiting;
					if (flag3)
					{
						bool flag4 = elapsedTime < 0.15f;
						if (flag4)
						{
							return;
						}
						this._currentState = DetailedFilterMenuBarDragHandler.MenuDragState.Holding;
					}
					Vector3 mousePosition = Input.mousePosition;
					bool flag5 = this._currentState == DetailedFilterMenuBarDragHandler.MenuDragState.Holding && Vector3.Distance(mousePosition, this._pointerDownMousePosition) > 10f;
					if (flag5)
					{
						this.OnQuitHoldBeforeDrag();
					}
					else
					{
						float actualHoldTime = elapsedTime - 0.15f;
						bool flag6 = this._currentState == DetailedFilterMenuBarDragHandler.MenuDragState.Holding;
						if (flag6)
						{
							bool isBeforeHoldDrag = !this._isSettingMode && actualHoldTime < 0.4f;
							bool flag7 = isBeforeHoldDrag;
							if (flag7)
							{
								float percent = actualHoldTime / 0.4f;
								ConchShipCursor.Instance.SetWheelProgress(percent);
							}
							else
							{
								this._currentState = DetailedFilterMenuBarDragHandler.MenuDragState.Dragging;
								this.OnStartDrag();
							}
						}
						else
						{
							bool flag8 = this._currentState == DetailedFilterMenuBarDragHandler.MenuDragState.Dragging;
							if (flag8)
							{
								this.TickScroll();
								this.TickDrag();
							}
						}
					}
				}
			}
		}

		// Token: 0x06003FD9 RID: 16345 RVA: 0x001FC038 File Offset: 0x001FA238
		private void TickScroll()
		{
			bool flag = this._currentState != DetailedFilterMenuBarDragHandler.MenuDragState.Dragging || !this._dragTarget;
			if (!flag)
			{
				bool flag2 = this._layoutMask == null;
				if (!flag2)
				{
					RectTransform rectTransform = this._layoutGroup.GetComponent<RectTransform>();
					bool flag3 = rectTransform.rect.width <= this._layoutMask.rect.width;
					if (!flag3)
					{
						Vector2 localMousePos;
						RectTransformUtility.ScreenPointToLocalPointInRectangle(this._layoutMask, Input.mousePosition, UIManager.Instance.UiCamera, out localMousePos);
						float maskWidth = this._layoutMask.rect.width;
						float mouseXPercent = (localMousePos.x + maskWidth * 0.5f) / maskWidth;
						float scrollSpeed = 0f;
						float num = mouseXPercent;
						float num2 = num;
						if (num2 >= 0.1f)
						{
							if (num2 > 0.9f)
							{
								float t = (mouseXPercent - 0.9f) / 0.1f;
								scrollSpeed = -Mathf.Lerp(0f, 1000f, t);
							}
						}
						else
						{
							float t2 = 1f - mouseXPercent / 0.1f;
							scrollSpeed = Mathf.Lerp(0f, 1000f, t2);
						}
						bool flag4 = Mathf.Abs(scrollSpeed) < 0.01f;
						if (!flag4)
						{
							float newX = rectTransform.anchoredPosition.x + Time.deltaTime * scrollSpeed;
							newX = Mathf.Clamp(newX, -rectTransform.rect.width + this._layoutMask.rect.width, 0f);
							rectTransform.anchoredPosition = new Vector2(newX, 0f);
						}
					}
				}
			}
		}

		// Token: 0x06003FDA RID: 16346 RVA: 0x001FC1E8 File Offset: 0x001FA3E8
		private void TickDrag()
		{
			bool flag = this._currentState != DetailedFilterMenuBarDragHandler.MenuDragState.Dragging || this._dragTarget == null;
			if (!flag)
			{
				RectTransform layoutRect = this._layoutGroup.GetComponent<RectTransform>();
				Vector2 localMousePos;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(layoutRect, Input.mousePosition, UIManager.Instance.UiCamera, out localMousePos);
				int dragTargetOriginIndex = this._filteredDragTargetOriginIndex;
				bool flag2 = dragTargetOriginIndex < 0 || dragTargetOriginIndex >= this._activeMenuControllers.Count;
				if (!flag2)
				{
					float dragTargetCurrentCenterX = this._activeMenuControllers[dragTargetOriginIndex].GetCurrentCenterX();
					bool isMouseAtLeft = localMousePos.x < dragTargetCurrentCenterX;
					int dragTargetTargetIndex = dragTargetOriginIndex;
					bool flag3 = isMouseAtLeft;
					if (flag3)
					{
						for (int i = 0; i < this._activeMenuCenterPositions.Count; i++)
						{
							bool flag4 = localMousePos.x < this._activeMenuCenterPositions[i];
							if (flag4)
							{
								dragTargetTargetIndex = i;
								break;
							}
						}
					}
					else
					{
						dragTargetTargetIndex = this._activeMenuCenterPositions.Count;
						for (int j = 0; j < this._activeMenuCenterPositions.Count; j++)
						{
							bool flag5 = localMousePos.x > this._activeMenuCenterPositions[j];
							if (!flag5)
							{
								break;
							}
							dragTargetTargetIndex = j + 1;
						}
					}
					dragTargetTargetIndex = Mathf.Clamp(dragTargetTargetIndex, 0, this._activeMenuControllers.Count);
					this._dragTargetTargetIndex = dragTargetTargetIndex;
					for (int k = 0; k < this._activeMenuControllers.Count; k++)
					{
						DetailedFilterMenuBarDragHandler.MenuXController ctrl = this._activeMenuControllers[k];
						bool flag6 = k == dragTargetOriginIndex;
						int destSlotIndex;
						if (flag6)
						{
							bool flag7 = dragTargetOriginIndex < dragTargetTargetIndex;
							if (flag7)
							{
								destSlotIndex = dragTargetTargetIndex - 1;
							}
							else
							{
								destSlotIndex = dragTargetTargetIndex;
							}
						}
						else
						{
							bool flag8 = dragTargetOriginIndex < dragTargetTargetIndex;
							if (flag8)
							{
								bool flag9 = k > dragTargetOriginIndex && k < dragTargetTargetIndex;
								if (flag9)
								{
									destSlotIndex = k - 1;
								}
								else
								{
									destSlotIndex = k;
								}
							}
							else
							{
								bool flag10 = k >= dragTargetTargetIndex && k < dragTargetOriginIndex;
								if (flag10)
								{
									destSlotIndex = k + 1;
								}
								else
								{
									destSlotIndex = k;
								}
							}
						}
						ctrl.SetTargetX(this._activeMenuOriginAnchoredPositions[destSlotIndex]);
					}
					foreach (DetailedFilterMenuBarDragHandler.MenuXController ctrl2 in this._activeMenuControllers)
					{
						ctrl2.Tick();
					}
				}
			}
		}

		// Token: 0x06003FDB RID: 16347 RVA: 0x001FC464 File Offset: 0x001FA664
		private void OnStartDrag()
		{
			this._layoutGroup.GetComponent<ContentSizeFitter>().enabled = false;
			this._scrollRect.enabled = false;
			this._temporarilyHidden.Clear();
			foreach (DetailFilterMultiSelectDropdown dropdown in this._dropdowns)
			{
				MultiSelectDropdownConfig<DetailFilterMultiSelectDropdownMenuBarConfig, DetailFilterMultiSelectDropdownItemConfig> config = dropdown.Config;
				bool flag = config.Dependency != null && dropdown.gameObject.activeInHierarchy;
				if (flag)
				{
					this._temporarilyHidden.Add(dropdown);
					dropdown.gameObject.SetActive(false);
				}
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(this._layoutGroup.GetComponent<RectTransform>());
			this._layoutGroup.enabled = false;
			this._groupsInDrag.Clear();
			List<DetailFilterMultiSelectDropdown> currentGroup = null;
			foreach (DetailFilterMultiSelectDropdown dropdown2 in this._dropdowns)
			{
				MultiSelectDropdownConfig<DetailFilterMultiSelectDropdownMenuBarConfig, DetailFilterMultiSelectDropdownItemConfig> config = dropdown2.Config;
				bool flag2 = config.Dependency == null;
				if (flag2)
				{
					currentGroup = new List<DetailFilterMultiSelectDropdown>
					{
						dropdown2
					};
					this._groupsInDrag.Add(currentGroup);
				}
				else if (currentGroup != null)
				{
					currentGroup.Add(dropdown2);
				}
			}
			this._activeMenuControllers.Clear();
			this._activeMenuCenterPositions.Clear();
			this._activeMenuOriginAnchoredPositions.Clear();
			this._filteredDragTargetOriginIndex = -1;
			int currentIndependentIndex = 0;
			for (int i = 0; i < this._groupsInDrag.Count; i++)
			{
				List<DetailFilterMultiSelectDropdown> group = this._groupsInDrag[i];
				bool flag3 = group.Count == 0;
				if (!flag3)
				{
					DetailFilterMultiSelectDropdown groupHead = group[0];
					MultiSelectDropdownConfig<DetailFilterMultiSelectDropdownMenuBarConfig, DetailFilterMultiSelectDropdownItemConfig> config = groupHead.Config;
					bool flag4 = config.Dependency == null;
					if (flag4)
					{
						bool flag5 = !groupHead.gameObject.activeInHierarchy;
						if (!flag5)
						{
							bool flag6 = groupHead == this._dropdowns[this._dragTargetOriginIndex];
							if (flag6)
							{
								this._filteredDragTargetOriginIndex = currentIndependentIndex;
							}
							RectTransform menuRect = groupHead.GetComponent<RectTransform>();
							this._activeMenuControllers.Add(new DetailedFilterMenuBarDragHandler.MenuXController(menuRect));
							this._activeMenuOriginAnchoredPositions.Add(menuRect.anchoredPosition.x);
							this._activeMenuCenterPositions.Add(DetailedFilterMenuBarDragHandler.CalcRectCenterPosition(menuRect));
							currentIndependentIndex++;
						}
					}
				}
			}
			foreach (DetailFilterMultiSelectDropdown dropdown3 in this._dropdowns)
			{
				dropdown3.SetPointerTriggerInteractable(false);
			}
			bool flag7 = this._dragTarget != null;
			if (flag7)
			{
				this._dragTargetOriginalSiblingIndex = this._dragTarget.transform.GetSiblingIndex();
				this._dragTarget.transform.SetAsLastSibling();
			}
			ConchShipCursor.Instance.SetWheelProgress(0f);
			ConchShipCursor.Instance.SetCursorImageWithKey("common_detailed_filter_drag", "filter_cursor_move", -1f, -1f);
		}

		// Token: 0x06003FDC RID: 16348 RVA: 0x001FC7C4 File Offset: 0x001FA9C4
		private static float CalcRectCenterPosition(RectTransform menuRect)
		{
			Vector2 pivot = menuRect.pivot;
			float width = menuRect.rect.width;
			return menuRect.localPosition.x + (0.5f - pivot.x) * width;
		}

		// Token: 0x06003FDD RID: 16349 RVA: 0x001FC808 File Offset: 0x001FAA08
		public void OnMenuBarPointerDown(DetailFilterMultiSelectDropdown target)
		{
			bool flag = target.Config.Dependency != null;
			if (!flag)
			{
				int menuIndex = this._dropdowns.IndexOf(target);
				this._pointerDownTime = Time.realtimeSinceStartup;
				this._pointerDownMousePosition = Input.mousePosition;
				this._currentState = DetailedFilterMenuBarDragHandler.MenuDragState.Waiting;
				this._hasMoved = false;
				this._dragTarget = target.gameObject;
				this._dragTargetOriginIndex = menuIndex;
				RectTransform rect = target.GetComponent<RectTransform>();
				DetailedFilterMenuBarDragHandler.ClampTargetParentToLayoutX(rect, this._layoutMask, rect.parent.GetComponent<RectTransform>());
			}
		}

		// Token: 0x06003FDE RID: 16350 RVA: 0x001FC894 File Offset: 0x001FAA94
		public static void ClampTargetParentToLayoutX(RectTransform target, RectTransform layoutMask, RectTransform moveRect)
		{
			bool flag = layoutMask == null || target == null || moveRect == null;
			if (!flag)
			{
				target.GetWorldCorners(DetailedFilterMenuBarDragHandler._worldCornerCache);
				float targetLeft = DetailedFilterMenuBarDragHandler._worldCornerCache[0].x;
				float targetRight = DetailedFilterMenuBarDragHandler._worldCornerCache[3].x;
				layoutMask.GetWorldCorners(DetailedFilterMenuBarDragHandler._worldCornerCache);
				float maskLeft = DetailedFilterMenuBarDragHandler._worldCornerCache[0].x;
				float maskRight = DetailedFilterMenuBarDragHandler._worldCornerCache[3].x;
				float offsetX = 0f;
				bool flag2 = targetRight > maskRight;
				if (flag2)
				{
					offsetX = maskRight - targetRight;
				}
				else
				{
					bool flag3 = targetLeft < maskLeft;
					if (flag3)
					{
						offsetX = maskLeft - targetLeft;
					}
				}
				bool flag4 = offsetX != 0f;
				if (flag4)
				{
					Vector3 worldOffset = new Vector3(offsetX, 0f, 0f);
					Vector3 localOffset = target.InverseTransformVector(worldOffset);
					moveRect.localPosition += localOffset;
				}
			}
		}

		// Token: 0x06003FDF RID: 16351 RVA: 0x001FC998 File Offset: 0x001FAB98
		public void OnMenuBarPointerUp(PointerEventData eventData)
		{
			bool flag = this._currentState == DetailedFilterMenuBarDragHandler.MenuDragState.Idle;
			if (!flag)
			{
				bool flag2 = eventData != null && eventData.button == PointerEventData.InputButton.Right;
				if (flag2)
				{
					this.RestoreDragTargetSibling();
					this.OnQuitDrag();
					this.ResetMenuBarSwapState();
				}
				else
				{
					bool flag3 = this._currentState == DetailedFilterMenuBarDragHandler.MenuDragState.Waiting;
					if (flag3)
					{
						this.OnQuitHoldBeforeDrag();
					}
					else
					{
						bool flag4 = this._currentState == DetailedFilterMenuBarDragHandler.MenuDragState.Holding;
						if (flag4)
						{
							this.OnQuitHoldBeforeDrag();
						}
						else
						{
							bool flag5 = this._currentState == DetailedFilterMenuBarDragHandler.MenuDragState.Dragging && !this._hasMoved;
							if (flag5)
							{
								this.RestoreDragTargetSibling();
								this.OnQuitDrag();
								bool isSettingMode = this._isSettingMode;
								if (isSettingMode)
								{
									Action<int> onBarSwapToggleShowRequest = this._onBarSwapToggleShowRequest;
									if (onBarSwapToggleShowRequest != null)
									{
										onBarSwapToggleShowRequest(this._dragTargetOriginIndex);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06003FE0 RID: 16352 RVA: 0x001FCA62 File Offset: 0x001FAC62
		public void OnMenuBarBeginDrag()
		{
		}

		// Token: 0x06003FE1 RID: 16353 RVA: 0x001FCA65 File Offset: 0x001FAC65
		public void OnMenuBarDrag()
		{
			this._hasMoved = true;
		}

		// Token: 0x06003FE2 RID: 16354 RVA: 0x001FCA70 File Offset: 0x001FAC70
		public void OnMenuBarEndDrag()
		{
			bool flag = this._currentState != DetailedFilterMenuBarDragHandler.MenuDragState.Dragging;
			if (!flag)
			{
				bool flag2 = this._dragTarget == null;
				if (!flag2)
				{
					int targetGroupIndex = Mathf.Clamp(this._dragTargetTargetIndex, 0, this._activeMenuControllers.Count);
					int originGroupIndex = this._filteredDragTargetOriginIndex;
					bool flag3 = targetGroupIndex == originGroupIndex;
					if (flag3)
					{
						this.RestoreDragTargetSibling();
						this.OnQuitDrag();
					}
					else
					{
						List<DetailFilterMultiSelectDropdown> draggedGroup = this._groupsInDrag.Find((List<DetailFilterMultiSelectDropdown> g) => g.Count > 0 && g[0] == this._dropdowns[this._dragTargetOriginIndex]);
						int draggedGroupFullIndex = this._groupsInDrag.IndexOf(draggedGroup);
						this._groupsInDrag.RemoveAt(draggedGroupFullIndex);
						int insertionIndex = 0;
						int independentGroupCounter = 0;
						for (int i = 0; i < this._groupsInDrag.Count; i++)
						{
							bool flag4 = this._groupsInDrag[i].Count > 0 && this._groupsInDrag[i][0].Config.Dependency == null;
							if (flag4)
							{
								bool flag5 = independentGroupCounter == targetGroupIndex;
								if (flag5)
								{
									insertionIndex = i;
									break;
								}
								independentGroupCounter++;
							}
							bool flag6 = i == this._groupsInDrag.Count - 1;
							if (flag6)
							{
								insertionIndex = this._groupsInDrag.Count;
							}
						}
						this._groupsInDrag.Insert(insertionIndex, draggedGroup);
						List<DetailFilterMultiSelectDropdown> newDropdownsOrder = new List<DetailFilterMultiSelectDropdown>();
						foreach (List<DetailFilterMultiSelectDropdown> group in this._groupsInDrag)
						{
							newDropdownsOrder.AddRange(group);
						}
						this._dropdowns.Clear();
						this._dropdowns.AddRange(newDropdownsOrder);
						Action onDragCompleted = this._onDragCompleted;
						if (onDragCompleted != null)
						{
							onDragCompleted();
						}
						this.OnQuitDrag();
					}
				}
			}
		}

		// Token: 0x06003FE3 RID: 16355 RVA: 0x001FCC68 File Offset: 0x001FAE68
		private void RestoreDragTargetSibling()
		{
			bool flag = this._dragTarget != null;
			if (flag)
			{
				this._dragTarget.transform.SetSiblingIndex(this._dragTargetOriginalSiblingIndex);
			}
		}

		// Token: 0x06003FE4 RID: 16356 RVA: 0x001FCCA0 File Offset: 0x001FAEA0
		private void OnQuitDrag()
		{
			ConchShipCursor.Instance.SetWheelProgress(0f);
			ConchShipCursor.Instance.SetDefaultCursorAndReleaseKey("common_detailed_filter_drag");
			this._currentState = DetailedFilterMenuBarDragHandler.MenuDragState.Idle;
			this._pointerDownTime = 0f;
			this._hasMoved = false;
			foreach (DetailFilterMultiSelectDropdown dropdown in this._dropdowns)
			{
				dropdown.SetPointerTriggerInteractable(true);
			}
			foreach (DetailFilterMultiSelectDropdown dropdown2 in this._temporarilyHidden)
			{
				dropdown2.gameObject.SetActive(true);
			}
			this._temporarilyHidden.Clear();
			this._layoutGroup.enabled = true;
			this._layoutGroup.GetComponent<ContentSizeFitter>().enabled = true;
			bool flag = this._scrollRect;
			if (flag)
			{
				this._scrollRect.enabled = true;
			}
		}

		// Token: 0x06003FE5 RID: 16357 RVA: 0x001FCDC8 File Offset: 0x001FAFC8
		private void OnQuitHoldBeforeDrag()
		{
			ConchShipCursor.Instance.SetWheelProgress(0f);
			this._currentState = DetailedFilterMenuBarDragHandler.MenuDragState.Idle;
			this._pointerDownTime = 0f;
		}

		// Token: 0x06003FE6 RID: 16358 RVA: 0x001FCDF0 File Offset: 0x001FAFF0
		public void HandleMenuBarToggleClick(int menuIndex)
		{
			bool flag = !this._isSettingMode;
			if (!flag)
			{
				bool isWaitingForSecondMenuSelection = this._isWaitingForSecondMenuSelection;
				if (isWaitingForSecondMenuSelection)
				{
					bool flag2 = this._firstSelectedMenuIndex != menuIndex;
					if (flag2)
					{
						List<DetailFilterMultiSelectDropdown> dropdowns = this._dropdowns;
						int firstSelectedMenuIndex = this._firstSelectedMenuIndex;
						List<DetailFilterMultiSelectDropdown> dropdowns2 = this._dropdowns;
						DetailFilterMultiSelectDropdown value = this._dropdowns[menuIndex];
						DetailFilterMultiSelectDropdown value2 = this._dropdowns[this._firstSelectedMenuIndex];
						dropdowns[firstSelectedMenuIndex] = value;
						dropdowns2[menuIndex] = value2;
						Action onDragCompleted = this._onDragCompleted;
						if (onDragCompleted != null)
						{
							onDragCompleted();
						}
					}
					this.ResetMenuBarSwapState();
				}
				else
				{
					this._firstSelectedMenuIndex = menuIndex;
					this._isWaitingForSecondMenuSelection = true;
				}
			}
		}

		// Token: 0x06003FE7 RID: 16359 RVA: 0x001FCEB0 File Offset: 0x001FB0B0
		public bool IsWaitingForSecondMenuSelection()
		{
			return this._isWaitingForSecondMenuSelection;
		}

		// Token: 0x06003FE8 RID: 16360 RVA: 0x001FCEC8 File Offset: 0x001FB0C8
		private void ResetMenuBarSwapState()
		{
			this._firstSelectedMenuIndex = -1;
			this._isWaitingForSecondMenuSelection = false;
		}

		// Token: 0x06003FE9 RID: 16361 RVA: 0x001FCED9 File Offset: 0x001FB0D9
		public void ForceResetState()
		{
			this.RestoreDragTargetSibling();
			this.ResetMenuBarSwapState();
			this.OnQuitDrag();
		}

		// Token: 0x06003FEA RID: 16362 RVA: 0x001FCEF1 File Offset: 0x001FB0F1
		public void SetSettingMode(bool isSettingMode)
		{
			this._isSettingMode = isSettingMode;
			this.ResetMenuBarSwapState();
		}

		// Token: 0x04002D9B RID: 11675
		private const float AllFunctionDelay = 0.15f;

		// Token: 0x04002D9C RID: 11676
		private const float HoldDragThreshold = 0.4f;

		// Token: 0x04002D9D RID: 11677
		private DetailedFilterMenuBarDragHandler.MenuDragState _currentState = DetailedFilterMenuBarDragHandler.MenuDragState.Idle;

		// Token: 0x04002D9E RID: 11678
		private float _pointerDownTime;

		// Token: 0x04002D9F RID: 11679
		private Vector3 _pointerDownMousePosition;

		// Token: 0x04002DA0 RID: 11680
		private bool _hasMoved;

		// Token: 0x04002DA1 RID: 11681
		private GameObject _dragTarget;

		// Token: 0x04002DA2 RID: 11682
		private int _dragTargetOriginIndex;

		// Token: 0x04002DA3 RID: 11683
		private int _dragTargetTargetIndex;

		// Token: 0x04002DA4 RID: 11684
		private int _dragTargetOriginalSiblingIndex;

		// Token: 0x04002DA5 RID: 11685
		private readonly HorizontalLayoutGroup _layoutGroup;

		// Token: 0x04002DA6 RID: 11686
		private readonly ScrollRect _scrollRect;

		// Token: 0x04002DA7 RID: 11687
		private readonly RectTransform _layoutMask;

		// Token: 0x04002DA8 RID: 11688
		private readonly RectTransform _layoutArea;

		// Token: 0x04002DA9 RID: 11689
		private readonly List<DetailFilterMultiSelectDropdown> _dropdowns;

		// Token: 0x04002DAA RID: 11690
		private readonly Action _onDragCompleted;

		// Token: 0x04002DAB RID: 11691
		private readonly Action<int> _onBarSwapToggleShowRequest;

		// Token: 0x04002DAC RID: 11692
		private readonly List<DetailedFilterMenuBarDragHandler.MenuXController> _menuControllers = new List<DetailedFilterMenuBarDragHandler.MenuXController>();

		// Token: 0x04002DAD RID: 11693
		private readonly List<float> _menuCenterPositions = new List<float>();

		// Token: 0x04002DAE RID: 11694
		private readonly List<float> _menuOriginAnchoredPositions = new List<float>();

		// Token: 0x04002DAF RID: 11695
		private bool _isSettingMode;

		// Token: 0x04002DB0 RID: 11696
		private readonly List<List<DetailFilterMultiSelectDropdown>> _groupsInDrag = new List<List<DetailFilterMultiSelectDropdown>>();

		// Token: 0x04002DB1 RID: 11697
		private readonly List<DetailedFilterMenuBarDragHandler.MenuXController> _activeMenuControllers = new List<DetailedFilterMenuBarDragHandler.MenuXController>();

		// Token: 0x04002DB2 RID: 11698
		private readonly List<float> _activeMenuCenterPositions = new List<float>();

		// Token: 0x04002DB3 RID: 11699
		private readonly List<float> _activeMenuOriginAnchoredPositions = new List<float>();

		// Token: 0x04002DB4 RID: 11700
		private int _filteredDragTargetOriginIndex;

		// Token: 0x04002DB5 RID: 11701
		private readonly List<DetailFilterMultiSelectDropdown> _temporarilyHidden = new List<DetailFilterMultiSelectDropdown>();

		// Token: 0x04002DB6 RID: 11702
		private const float ScrollInteractAreaPercent = 0.1f;

		// Token: 0x04002DB7 RID: 11703
		private const float MaxScrollSpeed = 1000f;

		// Token: 0x04002DB8 RID: 11704
		private static Vector3[] _worldCornerCache = new Vector3[4];

		// Token: 0x04002DB9 RID: 11705
		private int _firstSelectedMenuIndex = -1;

		// Token: 0x04002DBA RID: 11706
		private bool _isWaitingForSecondMenuSelection;

		// Token: 0x020018DC RID: 6364
		private enum MenuDragState
		{
			// Token: 0x0400B035 RID: 45109
			Idle,
			// Token: 0x0400B036 RID: 45110
			Waiting,
			// Token: 0x0400B037 RID: 45111
			Holding,
			// Token: 0x0400B038 RID: 45112
			Dragging
		}

		// Token: 0x020018DD RID: 6365
		private class MenuXController
		{
			// Token: 0x0600D80B RID: 55307 RVA: 0x005C2AEC File Offset: 0x005C0CEC
			public MenuXController(RectTransform menuRect)
			{
				this._menuRect = menuRect;
				this._targetX = menuRect.anchoredPosition.x;
				this._width = menuRect.rect.width;
				this._pivotX = menuRect.pivot.x;
			}

			// Token: 0x0600D80C RID: 55308 RVA: 0x005C2B40 File Offset: 0x005C0D40
			public float GetCurrentX()
			{
				return this._menuRect.anchoredPosition.x;
			}

			// Token: 0x0600D80D RID: 55309 RVA: 0x005C2B64 File Offset: 0x005C0D64
			public float GetCurrentCenterX()
			{
				return this._menuRect.anchoredPosition.x + (0.5f - this._pivotX) * this._width;
			}

			// Token: 0x0600D80E RID: 55310 RVA: 0x005C2B9C File Offset: 0x005C0D9C
			public float GetWidth()
			{
				return this._width;
			}

			// Token: 0x0600D80F RID: 55311 RVA: 0x005C2BB4 File Offset: 0x005C0DB4
			public float GetTargetX()
			{
				return this._targetX;
			}

			// Token: 0x0600D810 RID: 55312 RVA: 0x005C2BCC File Offset: 0x005C0DCC
			public void SetTargetX(float targetX)
			{
				bool flag = !this._menuRect;
				if (!flag)
				{
					this._targetX = targetX;
				}
			}

			// Token: 0x0600D811 RID: 55313 RVA: 0x005C2BF5 File Offset: 0x005C0DF5
			public void Tick()
			{
				this.SmoothlyMoveToTarget();
			}

			// Token: 0x0600D812 RID: 55314 RVA: 0x005C2C00 File Offset: 0x005C0E00
			private void SmoothlyMoveToTarget()
			{
				bool flag = !this._menuRect;
				if (!flag)
				{
					float currentX = this._menuRect.anchoredPosition.x;
					float newX = Mathf.Lerp(currentX, this._targetX, Time.deltaTime * 10f);
					this._menuRect.anchoredPosition = new Vector2(newX, this._menuRect.anchoredPosition.y);
				}
			}

			// Token: 0x0400B039 RID: 45113
			private readonly RectTransform _menuRect;

			// Token: 0x0400B03A RID: 45114
			private readonly float _width;

			// Token: 0x0400B03B RID: 45115
			private readonly float _pivotX;

			// Token: 0x0400B03C RID: 45116
			private float _targetX;
		}
	}
}
