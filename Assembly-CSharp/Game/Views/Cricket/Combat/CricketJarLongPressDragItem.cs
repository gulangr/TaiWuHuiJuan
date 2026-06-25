using System;
using System.Collections;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000ADC RID: 2780
	public class CricketJarLongPressDragItem : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		// Token: 0x060088CA RID: 35018 RVA: 0x003F5AD0 File Offset: 0x003F3CD0
		public void Initialize(CricketJarRoot owner, int jarIndex)
		{
			this._owner = owner;
			this._jarIndex = jarIndex;
			this._jar = base.GetComponent<CricketJar>();
			this._dragRectTransform = this._jar.CricketRectTransform;
			this._dragParentRectTransform = (this._dragRectTransform.parent as RectTransform);
			if (this._button == null)
			{
				this._button = base.GetComponent<CButton>();
			}
			this._canvasGroup = this._dragRectTransform.gameObject.GetOrAddComponent<CanvasGroup>();
		}

		// Token: 0x060088CB RID: 35019 RVA: 0x003F5B4C File Offset: 0x003F3D4C
		private void Update()
		{
			bool flag = !this._pointerDown;
			if (!flag)
			{
				bool flag2 = !this._dragging && !Input.GetMouseButton(0);
				if (flag2)
				{
					this.ResetPointerState();
				}
				else
				{
					bool dragging = this._dragging;
					if (dragging)
					{
						bool flag3 = !Input.GetMouseButton(0);
						if (flag3)
						{
							this.CompleteDrag();
						}
						else
						{
							this.UpdateDragPosition(Input.mousePosition);
						}
					}
					else
					{
						bool flag4 = !this._owner.CanStartJarDrag(this._jarIndex);
						if (!flag4)
						{
							bool flag5 = Vector2.Distance(this._pointerDownScreenPosition, Input.mousePosition) > 10f;
							if (flag5)
							{
								this.ResetPointerState();
							}
							else
							{
								bool flag6 = Time.unscaledTime - this._pointerDownTime < 0.4f;
								if (!flag6)
								{
									this.BeginDrag();
									this.UpdateDragPosition(Input.mousePosition);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060088CC RID: 35020 RVA: 0x003F5C44 File Offset: 0x003F3E44
		public void OnPointerDown(PointerEventData eventData)
		{
			bool flag = eventData.button > PointerEventData.InputButton.Left;
			if (!flag)
			{
				this._pointerDown = true;
				this._pointerDownTime = Time.unscaledTime;
				this._pointerDownScreenPosition = eventData.position;
				this._pressEventCamera = eventData.pressEventCamera;
			}
		}

		// Token: 0x060088CD RID: 35021 RVA: 0x003F5C8C File Offset: 0x003F3E8C
		public void OnPointerUp(PointerEventData eventData)
		{
			bool flag = eventData.button > PointerEventData.InputButton.Left;
			if (!flag)
			{
				bool dragging = this._dragging;
				if (dragging)
				{
					this.CompleteDrag();
				}
				else
				{
					this.ResetPointerState();
				}
			}
		}

		// Token: 0x060088CE RID: 35022 RVA: 0x003F5CC4 File Offset: 0x003F3EC4
		private void OnDisable()
		{
			this.RestoreDragState();
			this.ResetPointerState();
		}

		// Token: 0x060088CF RID: 35023 RVA: 0x003F5CD8 File Offset: 0x003F3ED8
		private void BeginDrag()
		{
			this._dragging = true;
			this._originParent = this._dragRectTransform.parent;
			this._originAnchoredPosition = this._dragRectTransform.anchoredPosition;
			this._originSiblingIndex = this._dragRectTransform.GetSiblingIndex();
			this._originLocalScale = this._dragRectTransform.localScale;
			this._dragRectTransform.SetParent(this._dragParentRectTransform, true);
			this._dragRectTransform.SetAsLastSibling();
			this._canvasGroup.blocksRaycasts = false;
			bool flag = this._dragCanvas == null;
			if (flag)
			{
				this._dragCanvas = this._dragRectTransform.gameObject.GetOrAddComponent<Canvas>();
				this._dragRectTransform.gameObject.GetOrAddComponent<ConchShipGraphicRaycaster>();
				this._dragCanvasWasEnabled = false;
			}
			else
			{
				this._dragCanvasWasEnabled = this._dragCanvas.enabled;
			}
			this._dragCanvasOverrideSorting = this._dragCanvas.overrideSorting;
			this._dragCanvasSortingLayerId = this._dragCanvas.sortingLayerID;
			this._dragCanvasSortingOrder = this._dragCanvas.sortingOrder;
			this._dragCanvas.enabled = true;
			this._dragCanvas.overrideSorting = true;
			this._dragCanvas.sortingLayerID = SortingLayer.NameToID("UI");
			this._dragCanvas.sortingOrder = 10000;
			Vector2 localPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this._dragParentRectTransform, Input.mousePosition, this._pressEventCamera, out localPoint);
			this._dragOffset = this._dragRectTransform.anchoredPosition - localPoint;
			bool flag2 = this._restoreInteractableCoroutine != null;
			if (flag2)
			{
				base.StopCoroutine(this._restoreInteractableCoroutine);
				this._restoreInteractableCoroutine = null;
			}
			this._button.interactable = false;
		}

		// Token: 0x060088D0 RID: 35024 RVA: 0x003F5E8C File Offset: 0x003F408C
		private void UpdateDragPosition(Vector2 screenPosition)
		{
			Vector2 localPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this._dragParentRectTransform, screenPosition, this._pressEventCamera, out localPoint);
			this._dragRectTransform.anchoredPosition = localPoint + this._dragOffset;
		}

		// Token: 0x060088D1 RID: 35025 RVA: 0x003F5EC8 File Offset: 0x003F40C8
		private void CompleteDrag()
		{
			int dropTargetIndex = this._owner.GetDropJarIndex(Input.mousePosition, this._pressEventCamera);
			this.RestoreDragState();
			this._owner.HandleJarDrop(this._jarIndex, dropTargetIndex);
			this.ResetPointerState();
			this._restoreInteractableCoroutine = base.StartCoroutine(this.CoRestoreInteractableNextFrame());
		}

		// Token: 0x060088D2 RID: 35026 RVA: 0x003F5F28 File Offset: 0x003F4128
		private void RestoreDragState()
		{
			bool flag = !this._dragging;
			if (!flag)
			{
				this._dragging = false;
				this._canvasGroup.blocksRaycasts = true;
				this._dragCanvas.overrideSorting = this._dragCanvasOverrideSorting;
				this._dragCanvas.sortingLayerID = this._dragCanvasSortingLayerId;
				this._dragCanvas.sortingOrder = this._dragCanvasSortingOrder;
				bool flag2 = !this._dragCanvasWasEnabled;
				if (flag2)
				{
					ConchShipGraphicRaycaster raycaster = this._dragRectTransform.gameObject.GetComponent<ConchShipGraphicRaycaster>();
					bool flag3 = raycaster != null;
					if (flag3)
					{
						Object.Destroy(raycaster);
					}
					Object.Destroy(this._dragCanvas);
					this._dragCanvas = null;
				}
				else
				{
					this._dragCanvas.enabled = this._dragCanvasWasEnabled;
				}
				this._dragRectTransform.SetParent(this._originParent, false);
				this._dragRectTransform.SetSiblingIndex(this._originSiblingIndex);
				this._dragRectTransform.localScale = this._originLocalScale;
				this._dragRectTransform.anchoredPosition = this._originAnchoredPosition;
			}
		}

		// Token: 0x060088D3 RID: 35027 RVA: 0x003F6036 File Offset: 0x003F4236
		private IEnumerator CoRestoreInteractableNextFrame()
		{
			yield return null;
			this._owner.RefreshJarInteractableState(this._jarIndex);
			this._restoreInteractableCoroutine = null;
			yield break;
		}

		// Token: 0x060088D4 RID: 35028 RVA: 0x003F6045 File Offset: 0x003F4245
		private void ResetPointerState()
		{
			this._pointerDown = false;
			this._pointerDownTime = 0f;
			this._pointerDownScreenPosition = Vector2.zero;
		}

		// Token: 0x040068B6 RID: 26806
		private const float HoldThreshold = 0.4f;

		// Token: 0x040068B7 RID: 26807
		private const float CancelHoldMoveThreshold = 10f;

		// Token: 0x040068B8 RID: 26808
		private const int DragSortingOrder = 10000;

		// Token: 0x040068B9 RID: 26809
		private CricketJarRoot _owner;

		// Token: 0x040068BA RID: 26810
		private CricketJar _jar;

		// Token: 0x040068BB RID: 26811
		private RectTransform _dragRectTransform;

		// Token: 0x040068BC RID: 26812
		private RectTransform _dragParentRectTransform;

		// Token: 0x040068BD RID: 26813
		private CButton _button;

		// Token: 0x040068BE RID: 26814
		private CanvasGroup _canvasGroup;

		// Token: 0x040068BF RID: 26815
		private Canvas _dragCanvas;

		// Token: 0x040068C0 RID: 26816
		private Camera _pressEventCamera;

		// Token: 0x040068C1 RID: 26817
		private Coroutine _restoreInteractableCoroutine;

		// Token: 0x040068C2 RID: 26818
		private int _jarIndex = -1;

		// Token: 0x040068C3 RID: 26819
		private bool _pointerDown;

		// Token: 0x040068C4 RID: 26820
		private bool _dragging;

		// Token: 0x040068C5 RID: 26821
		private float _pointerDownTime;

		// Token: 0x040068C6 RID: 26822
		private Vector2 _pointerDownScreenPosition;

		// Token: 0x040068C7 RID: 26823
		private Vector2 _dragOffset;

		// Token: 0x040068C8 RID: 26824
		private Vector2 _originAnchoredPosition;

		// Token: 0x040068C9 RID: 26825
		private int _originSiblingIndex;

		// Token: 0x040068CA RID: 26826
		private Vector3 _originLocalScale;

		// Token: 0x040068CB RID: 26827
		private Transform _originParent;

		// Token: 0x040068CC RID: 26828
		private bool _dragCanvasWasEnabled;

		// Token: 0x040068CD RID: 26829
		private bool _dragCanvasOverrideSorting;

		// Token: 0x040068CE RID: 26830
		private int _dragCanvasSortingLayerId;

		// Token: 0x040068CF RID: 26831
		private int _dragCanvasSortingOrder;
	}
}
