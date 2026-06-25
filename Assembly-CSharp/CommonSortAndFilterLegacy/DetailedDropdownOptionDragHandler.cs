using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000432 RID: 1074
	public class DetailedDropdownOptionDragHandler
	{
		// Token: 0x1700067F RID: 1663
		// (get) Token: 0x06003FAE RID: 16302 RVA: 0x001FAE01 File Offset: 0x001F9001
		public bool IsInDragMode
		{
			get
			{
				return this._currentState == DetailedDropdownOptionDragHandler.DragState.Dragging;
			}
		}

		// Token: 0x17000680 RID: 1664
		// (get) Token: 0x06003FAF RID: 16303 RVA: 0x001FAE0C File Offset: 0x001F900C
		public bool IsSettingMode
		{
			get
			{
				return this._isSettingMode;
			}
		}

		// Token: 0x06003FB0 RID: 16304 RVA: 0x001FAE14 File Offset: 0x001F9014
		public DetailedDropdownOptionDragHandler(VerticalLayoutGroup layoutGroup, RectTransform layoutMask, List<DetailFilterMultiSelectDropdown.OptionItem> options, int disabledItemsCount, Action onDragCompleted, Action<int> onEnterHold)
		{
			this._layoutGroup = layoutGroup;
			this._layoutMask = layoutMask;
			this._options = options;
			this._onDragCompleted = onDragCompleted;
			this._disabledItemsCount = disabledItemsCount;
			this._onEnterHold = onEnterHold;
		}

		// Token: 0x06003FB1 RID: 16305 RVA: 0x001FAE7C File Offset: 0x001F907C
		public void TickHoldDrag()
		{
			bool flag = this._currentState == DetailedDropdownOptionDragHandler.DragState.Idle;
			if (!flag)
			{
				float currentTime = Time.realtimeSinceStartup;
				float elapsedTime = currentTime - this._pointerDownTime;
				bool flag2 = this._isSettingMode && this._currentState == DetailedDropdownOptionDragHandler.DragState.Waiting;
				if (flag2)
				{
					this._currentState = DetailedDropdownOptionDragHandler.DragState.Dragging;
					this.OnStartDrag();
				}
				else
				{
					bool flag3 = this._currentState == DetailedDropdownOptionDragHandler.DragState.Waiting;
					if (flag3)
					{
						bool flag4 = elapsedTime < 0.15f;
						if (flag4)
						{
							return;
						}
						this._currentState = DetailedDropdownOptionDragHandler.DragState.Holding;
						Action<int> onEnterHold = this._onEnterHold;
						if (onEnterHold != null)
						{
							onEnterHold(this._dragTargetId);
						}
					}
					Vector3 mousePosition = Input.mousePosition;
					bool flag5 = this._currentState == DetailedDropdownOptionDragHandler.DragState.Holding && Vector3.Distance(mousePosition, this._pointerDownMousePosition) > 10f;
					if (flag5)
					{
						this.OnQuitHoldBeforeDrag();
					}
					else
					{
						float actualHoldTime = elapsedTime - 0.15f;
						bool flag6 = this._currentState == DetailedDropdownOptionDragHandler.DragState.Holding;
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
								this._currentState = DetailedDropdownOptionDragHandler.DragState.Dragging;
								this.OnStartDrag();
							}
						}
						else
						{
							bool flag8 = this._currentState == DetailedDropdownOptionDragHandler.DragState.Dragging;
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

		// Token: 0x06003FB2 RID: 16306 RVA: 0x001FAFD4 File Offset: 0x001F91D4
		private void TickScroll()
		{
			bool flag = this._currentState != DetailedDropdownOptionDragHandler.DragState.Dragging || !this._dragTarget;
			if (!flag)
			{
				RectTransform rectTransform = this._layoutGroup.GetComponent<RectTransform>();
				bool flag2 = rectTransform.rect.height <= this._layoutMask.rect.height;
				if (!flag2)
				{
					Vector2 localMousePos;
					RectTransformUtility.ScreenPointToLocalPointInRectangle(this._layoutMask, Input.mousePosition, UIManager.Instance.UiCamera, out localMousePos);
					float maskHeight = this._layoutMask.rect.height;
					float mouseYPercent = (localMousePos.y + maskHeight * 0.5f) / maskHeight;
					float scrollSpeed = 0f;
					float num = mouseYPercent;
					float num2 = num;
					if (num2 >= 0.1f)
					{
						if (num2 > 0.9f)
						{
							float t = (mouseYPercent - 0.9f) / 0.1f;
							scrollSpeed = -Mathf.Lerp(0f, 1000f, t);
						}
					}
					else
					{
						float t2 = 1f - mouseYPercent / 0.1f;
						scrollSpeed = Mathf.Lerp(0f, 1000f, t2);
					}
					bool flag3 = Mathf.Abs(scrollSpeed) < 0.01f;
					if (!flag3)
					{
						float newY = rectTransform.anchoredPosition.y + Time.deltaTime * scrollSpeed;
						newY = Mathf.Clamp(newY, -rectTransform.rect.height + this._layoutMask.rect.height, 0f);
						rectTransform.anchoredPosition = new Vector2(0f, newY);
					}
				}
			}
		}

		// Token: 0x06003FB3 RID: 16307 RVA: 0x001FB170 File Offset: 0x001F9370
		private void TickDrag()
		{
			bool flag = this._currentState != DetailedDropdownOptionDragHandler.DragState.Dragging || this._dragTarget == null;
			if (!flag)
			{
				RectTransform layoutRect = this._layoutGroup.GetComponent<RectTransform>();
				Vector2 localMousePos;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(layoutRect, Input.mousePosition, UIManager.Instance.UiCamera, out localMousePos);
				int dragTargetOriginIndex = this._dragTargetOriginIndex;
				float targetCurrentY = this._optionControllers[dragTargetOriginIndex].GetCurrentY();
				bool isMouseAtTop = localMousePos.y > targetCurrentY;
				int dragTargetTargetIndex = this._dragTargetOriginIndex;
				for (int i = 0; i < this._optionCenterPositions.Count; i++)
				{
					bool flag2 = isMouseAtTop;
					if (flag2)
					{
						bool flag3 = localMousePos.y > this._optionCenterPositions[i];
						if (flag3)
						{
							dragTargetTargetIndex = i;
							break;
						}
					}
					else
					{
						bool flag4 = localMousePos.y < this._optionCenterPositions[i];
						if (flag4)
						{
							dragTargetTargetIndex = i;
						}
					}
				}
				dragTargetTargetIndex = Mathf.Clamp(dragTargetTargetIndex, this._disabledItemsCount, this._optionCenterPositions.Count - 1);
				this._dragTargetTargetIndex = dragTargetTargetIndex;
				for (int j = 0; j < this._optionControllers.Count; j++)
				{
					DetailedDropdownOptionDragHandler.OptionYController ctrl = this._optionControllers[j];
					bool flag5 = j == dragTargetOriginIndex;
					if (flag5)
					{
						ctrl.SetTargetY(this._optionCenterPositions[dragTargetTargetIndex]);
					}
					else
					{
						bool flag6 = j < dragTargetOriginIndex;
						int myTargetIndex;
						if (flag6)
						{
							bool flag7 = j < dragTargetTargetIndex;
							if (flag7)
							{
								myTargetIndex = j;
							}
							else
							{
								myTargetIndex = j + 1;
							}
						}
						else
						{
							bool flag8 = j <= dragTargetTargetIndex;
							if (flag8)
							{
								myTargetIndex = j - 1;
							}
							else
							{
								myTargetIndex = j;
							}
						}
						ctrl.SetTargetY(this._optionCenterPositions[myTargetIndex]);
					}
				}
				foreach (DetailedDropdownOptionDragHandler.OptionYController ctrl2 in this._optionControllers)
				{
					ctrl2.Tick();
				}
			}
		}

		// Token: 0x06003FB4 RID: 16308 RVA: 0x001FB394 File Offset: 0x001F9594
		private void OnStartDrag()
		{
			this._layoutGroup.GetComponent<ContentSizeFitter>().enabled = false;
			this._layoutGroup.enabled = false;
			this._optionControllers.Clear();
			this._optionCenterPositions.Clear();
			foreach (DetailFilterMultiSelectDropdown.OptionItem option in this._options)
			{
				RectTransform menuRect = option.Refers.GetComponent<RectTransform>();
				DetailedDropdownOptionDragHandler.OptionYController menuController = new DetailedDropdownOptionDragHandler.OptionYController(menuRect);
				this._optionControllers.Add(menuController);
				float calcRectCenterPosition = DetailedDropdownOptionDragHandler.CalcRectCenterPosition(menuRect);
				this._optionCenterPositions.Add(calcRectCenterPosition);
			}
			bool flag = this._dragTarget != null;
			if (flag)
			{
				this._dragTargetOriginalSiblingIndex = this._dragTarget.transform.GetSiblingIndex();
				this._dragTarget.transform.SetAsLastSibling();
			}
			ConchShipCursor.Instance.SetWheelProgress(0f);
			ConchShipCursor.Instance.SetCursorImageWithKey("common_detailed_filter_drag", "filter_cursor_move", -1f, -1f);
		}

		// Token: 0x06003FB5 RID: 16309 RVA: 0x001FB4BC File Offset: 0x001F96BC
		private static float CalcRectCenterPosition(RectTransform menuRect)
		{
			Vector2 pivot = menuRect.pivot;
			float height = menuRect.rect.height;
			return menuRect.localPosition.y + (pivot.y - 0.5f) * height;
		}

		// Token: 0x06003FB6 RID: 16310 RVA: 0x001FB500 File Offset: 0x001F9700
		public void OnOptionPointerDown(DetailFilterMultiSelectDropdown.OptionItem target)
		{
			bool isSpecial = target.IsSpecial;
			if (!isSpecial)
			{
				int optionIndex = this._options.IndexOf(target);
				this._pointerDownTime = Time.realtimeSinceStartup;
				this._pointerDownMousePosition = Input.mousePosition;
				this._currentState = DetailedDropdownOptionDragHandler.DragState.Waiting;
				this._hasMoved = false;
				this._dragTarget = target.Refers.gameObject;
				this._dragTargetId = target.Id;
				this._dragTargetOriginIndex = optionIndex;
			}
		}

		// Token: 0x06003FB7 RID: 16311 RVA: 0x001FB570 File Offset: 0x001F9770
		public void OnOptionPointerUp(PointerEventData eventData)
		{
			bool flag = this._currentState == DetailedDropdownOptionDragHandler.DragState.Idle;
			if (!flag)
			{
				bool flag2 = eventData != null && eventData.button == PointerEventData.InputButton.Right;
				if (flag2)
				{
					this.RestoreDragTargetSibling();
					this.OnQuitDrag();
					this.ResetSwapState();
				}
				else
				{
					bool flag3 = this._currentState == DetailedDropdownOptionDragHandler.DragState.Waiting;
					if (flag3)
					{
						this.OnQuitHoldBeforeDrag();
					}
					else
					{
						bool flag4 = this._currentState == DetailedDropdownOptionDragHandler.DragState.Holding;
						if (flag4)
						{
							this.OnQuitHoldBeforeDrag();
						}
						else
						{
							bool flag5 = this._currentState == DetailedDropdownOptionDragHandler.DragState.Dragging && !this._hasMoved;
							if (flag5)
							{
								this.RestoreDragTargetSibling();
								this.OnQuitDrag();
							}
						}
					}
				}
			}
		}

		// Token: 0x06003FB8 RID: 16312 RVA: 0x001FB616 File Offset: 0x001F9816
		public void OnOptionBeginDrag()
		{
		}

		// Token: 0x06003FB9 RID: 16313 RVA: 0x001FB619 File Offset: 0x001F9819
		public void OnOptionDrag()
		{
			this._hasMoved = true;
		}

		// Token: 0x06003FBA RID: 16314 RVA: 0x001FB624 File Offset: 0x001F9824
		public void OnOptionEndDrag()
		{
			bool flag = this._currentState != DetailedDropdownOptionDragHandler.DragState.Dragging;
			if (!flag)
			{
				bool flag2 = this._dragTarget == null;
				if (!flag2)
				{
					this._wasInDragMode = true;
					bool flag3 = this._dragTargetTargetIndex == this._dragTargetOriginIndex;
					if (flag3)
					{
						this.RestoreDragTargetSibling();
						this.OnQuitDrag();
					}
					else
					{
						this.DragToTargetIndex(this._dragTargetOriginIndex, this._dragTargetTargetIndex);
						this.OnQuitDrag();
					}
				}
			}
		}

		// Token: 0x06003FBB RID: 16315 RVA: 0x001FB69C File Offset: 0x001F989C
		public void HandleToggleClick(int itemIndex)
		{
			bool flag = !this._isSettingMode || itemIndex < this._disabledItemsCount;
			if (!flag)
			{
				bool isWaitingForSecondItemSelection = this._isWaitingForSecondItemSelection;
				if (isWaitingForSecondItemSelection)
				{
					bool flag2 = this._firstSelectedItemIndex != itemIndex;
					if (flag2)
					{
						this.SwapWithTargetIndex(this._firstSelectedItemIndex, itemIndex);
					}
					this.ResetSwapState();
				}
				else
				{
					this._firstSelectedItemIndex = itemIndex;
					this._isWaitingForSecondItemSelection = true;
				}
			}
		}

		// Token: 0x06003FBC RID: 16316 RVA: 0x001FB70C File Offset: 0x001F990C
		public void HandleToggleClickOff(int itemIndex)
		{
			bool flag = !this._isSettingMode || itemIndex < this._disabledItemsCount;
			if (!flag)
			{
				this.ResetSwapState();
			}
		}

		// Token: 0x06003FBD RID: 16317 RVA: 0x001FB73C File Offset: 0x001F993C
		private void DragToTargetIndex(int sourceIndex, int targetIndex)
		{
			bool flag = sourceIndex < this._disabledItemsCount || targetIndex < this._disabledItemsCount;
			if (!flag)
			{
				DetailFilterMultiSelectDropdown.OptionItem dragTargetDropdown = this._options[sourceIndex];
				this._options.RemoveAt(sourceIndex);
				this._options.Insert(targetIndex, dragTargetDropdown);
				Action onDragCompleted = this._onDragCompleted;
				if (onDragCompleted != null)
				{
					onDragCompleted();
				}
			}
		}

		// Token: 0x06003FBE RID: 16318 RVA: 0x001FB7A0 File Offset: 0x001F99A0
		private void SwapWithTargetIndex(int sourceIndex, int targetIndex)
		{
			bool flag = sourceIndex < this._disabledItemsCount || targetIndex < this._disabledItemsCount;
			if (!flag)
			{
				List<DetailFilterMultiSelectDropdown.OptionItem> options = this._options;
				List<DetailFilterMultiSelectDropdown.OptionItem> options2 = this._options;
				DetailFilterMultiSelectDropdown.OptionItem value = this._options[targetIndex];
				DetailFilterMultiSelectDropdown.OptionItem value2 = this._options[sourceIndex];
				options[sourceIndex] = value;
				options2[targetIndex] = value2;
				Action onDragCompleted = this._onDragCompleted;
				if (onDragCompleted != null)
				{
					onDragCompleted();
				}
			}
		}

		// Token: 0x06003FBF RID: 16319 RVA: 0x001FB824 File Offset: 0x001F9A24
		private void RestoreDragTargetSibling()
		{
			bool flag = this._dragTarget != null;
			if (flag)
			{
				this._dragTarget.transform.SetSiblingIndex(this._dragTargetOriginalSiblingIndex);
			}
		}

		// Token: 0x06003FC0 RID: 16320 RVA: 0x001FB85C File Offset: 0x001F9A5C
		private void OnQuitDrag()
		{
			ConchShipCursor.Instance.SetWheelProgress(0f);
			ConchShipCursor.Instance.SetDefaultCursorAndReleaseKey("common_detailed_filter_drag");
			this._currentState = DetailedDropdownOptionDragHandler.DragState.Idle;
			this._pointerDownTime = 0f;
			this._hasMoved = false;
			this._layoutGroup.enabled = true;
			this._layoutGroup.GetComponent<ContentSizeFitter>().enabled = true;
		}

		// Token: 0x06003FC1 RID: 16321 RVA: 0x001FB8C2 File Offset: 0x001F9AC2
		private void OnQuitHoldBeforeDrag()
		{
			ConchShipCursor.Instance.SetWheelProgress(0f);
			this._currentState = DetailedDropdownOptionDragHandler.DragState.Idle;
			this._pointerDownTime = 0f;
		}

		// Token: 0x06003FC2 RID: 16322 RVA: 0x001FB8E7 File Offset: 0x001F9AE7
		public void OnSwitchCustomOrderSettingMode(bool isSettingMode)
		{
			this._isSettingMode = isSettingMode;
			this.ResetSwapState();
		}

		// Token: 0x06003FC3 RID: 16323 RVA: 0x001FB8F8 File Offset: 0x001F9AF8
		public void ResetSwapState()
		{
			this._firstSelectedItemIndex = -1;
			this._isWaitingForSecondItemSelection = false;
		}

		// Token: 0x06003FC4 RID: 16324 RVA: 0x001FB909 File Offset: 0x001F9B09
		public void ForceResetState()
		{
			this.RestoreDragTargetSibling();
			this.ResetSwapState();
			this.OnQuitDrag();
		}

		// Token: 0x06003FC5 RID: 16325 RVA: 0x001FB924 File Offset: 0x001F9B24
		public int GetFirstSelectedItemId()
		{
			return this._firstSelectedItemIndex;
		}

		// Token: 0x06003FC6 RID: 16326 RVA: 0x001FB93C File Offset: 0x001F9B3C
		public bool IsWaitingForSecondItemSelection()
		{
			return this._isWaitingForSecondItemSelection;
		}

		// Token: 0x06003FC7 RID: 16327 RVA: 0x001FB954 File Offset: 0x001F9B54
		public bool CheckAndClearWasInDragMode()
		{
			bool result = this._wasInDragMode;
			this._wasInDragMode = false;
			return result;
		}

		// Token: 0x04002D80 RID: 11648
		private const float AllFuctionDelay = 0.15f;

		// Token: 0x04002D81 RID: 11649
		private const float HoldDragThreshold = 0.4f;

		// Token: 0x04002D82 RID: 11650
		private Action<int> _onEnterHold;

		// Token: 0x04002D83 RID: 11651
		private DetailedDropdownOptionDragHandler.DragState _currentState = DetailedDropdownOptionDragHandler.DragState.Idle;

		// Token: 0x04002D84 RID: 11652
		private float _pointerDownTime;

		// Token: 0x04002D85 RID: 11653
		private Vector3 _pointerDownMousePosition;

		// Token: 0x04002D86 RID: 11654
		private bool _hasMoved;

		// Token: 0x04002D87 RID: 11655
		private GameObject _dragTarget;

		// Token: 0x04002D88 RID: 11656
		private int _dragTargetOriginIndex;

		// Token: 0x04002D89 RID: 11657
		private int _dragTargetId;

		// Token: 0x04002D8A RID: 11658
		private int _dragTargetTargetIndex;

		// Token: 0x04002D8B RID: 11659
		private int _dragTargetOriginalSiblingIndex;

		// Token: 0x04002D8C RID: 11660
		private readonly VerticalLayoutGroup _layoutGroup;

		// Token: 0x04002D8D RID: 11661
		private readonly RectTransform _layoutMask;

		// Token: 0x04002D8E RID: 11662
		private readonly List<DetailFilterMultiSelectDropdown.OptionItem> _options;

		// Token: 0x04002D8F RID: 11663
		private readonly Action _onDragCompleted;

		// Token: 0x04002D90 RID: 11664
		private readonly int _disabledItemsCount;

		// Token: 0x04002D91 RID: 11665
		private readonly List<DetailedDropdownOptionDragHandler.OptionYController> _optionControllers = new List<DetailedDropdownOptionDragHandler.OptionYController>();

		// Token: 0x04002D92 RID: 11666
		private readonly List<float> _optionCenterPositions = new List<float>();

		// Token: 0x04002D93 RID: 11667
		private bool _isSettingMode;

		// Token: 0x04002D94 RID: 11668
		private int _firstSelectedItemIndex = -1;

		// Token: 0x04002D95 RID: 11669
		private bool _isWaitingForSecondItemSelection;

		// Token: 0x04002D96 RID: 11670
		private bool _wasInDragMode;

		// Token: 0x04002D97 RID: 11671
		private const float ScrollInteractAreaPercent = 0.1f;

		// Token: 0x04002D98 RID: 11672
		private const float MaxScrollSpeed = 1000f;

		// Token: 0x020018D8 RID: 6360
		private enum DragState
		{
			// Token: 0x0400B02A RID: 45098
			Idle,
			// Token: 0x0400B02B RID: 45099
			Waiting,
			// Token: 0x0400B02C RID: 45100
			Holding,
			// Token: 0x0400B02D RID: 45101
			Dragging
		}

		// Token: 0x020018D9 RID: 6361
		private class OptionYController
		{
			// Token: 0x0600D803 RID: 55299 RVA: 0x005C299C File Offset: 0x005C0B9C
			public OptionYController(RectTransform optionRect)
			{
				this._optionRect = optionRect;
				this._targetY = optionRect.anchoredPosition.y;
			}

			// Token: 0x0600D804 RID: 55300 RVA: 0x005C29C0 File Offset: 0x005C0BC0
			public float GetCurrentY()
			{
				return this._optionRect.anchoredPosition.y;
			}

			// Token: 0x0600D805 RID: 55301 RVA: 0x005C29E4 File Offset: 0x005C0BE4
			public void SetTargetY(float targetY)
			{
				bool flag = !this._optionRect;
				if (!flag)
				{
					this._targetY = targetY;
				}
			}

			// Token: 0x0600D806 RID: 55302 RVA: 0x005C2A0D File Offset: 0x005C0C0D
			public void Tick()
			{
				this.SmoothlyMoveToTarget();
			}

			// Token: 0x0600D807 RID: 55303 RVA: 0x005C2A18 File Offset: 0x005C0C18
			private void SmoothlyMoveToTarget()
			{
				bool flag = !this._optionRect;
				if (!flag)
				{
					float currentY = this._optionRect.anchoredPosition.y;
					float newY = Mathf.Lerp(currentY, this._targetY, Time.deltaTime * 10f);
					this._optionRect.anchoredPosition = new Vector2(this._optionRect.anchoredPosition.x, newY);
				}
			}

			// Token: 0x0400B02E RID: 45102
			private readonly RectTransform _optionRect;

			// Token: 0x0400B02F RID: 45103
			private float _targetY;
		}
	}
}
