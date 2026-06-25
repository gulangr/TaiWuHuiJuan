using System;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace FrameWork.UISystem.UIElements
{
	// Token: 0x02001007 RID: 4103
	public class CScrollRect : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IScrollHandler, IPointerEnterHandler, IPointerExitHandler
	{
		// Token: 0x17001519 RID: 5401
		// (get) Token: 0x0600BB41 RID: 47937 RVA: 0x005532A8 File Offset: 0x005514A8
		// (set) Token: 0x0600BB42 RID: 47938 RVA: 0x005532B0 File Offset: 0x005514B0
		public RectTransform Viewport
		{
			get
			{
				return this.viewport;
			}
			set
			{
				this.viewport = value;
			}
		}

		// Token: 0x1700151A RID: 5402
		// (get) Token: 0x0600BB43 RID: 47939 RVA: 0x005532B9 File Offset: 0x005514B9
		// (set) Token: 0x0600BB44 RID: 47940 RVA: 0x005532C1 File Offset: 0x005514C1
		public RectTransform Content
		{
			get
			{
				return this.content;
			}
			set
			{
				this.content = value;
			}
		}

		// Token: 0x1700151B RID: 5403
		// (get) Token: 0x0600BB45 RID: 47941 RVA: 0x005532CA File Offset: 0x005514CA
		// (set) Token: 0x0600BB46 RID: 47942 RVA: 0x005532D2 File Offset: 0x005514D2
		public CScrollRect.ScrollDirection Direction
		{
			get
			{
				return this.direction;
			}
			set
			{
				this.direction = value;
			}
		}

		// Token: 0x1700151C RID: 5404
		// (get) Token: 0x0600BB47 RID: 47943 RVA: 0x005532DB File Offset: 0x005514DB
		// (set) Token: 0x0600BB48 RID: 47944 RVA: 0x005532E3 File Offset: 0x005514E3
		public Scrollbar ScrollBar
		{
			get
			{
				return this.scrollBar;
			}
			set
			{
				this.scrollBar = value;
			}
		}

		// Token: 0x1700151D RID: 5405
		// (get) Token: 0x0600BB49 RID: 47945 RVA: 0x005532EC File Offset: 0x005514EC
		private bool Horizontal
		{
			get
			{
				return this.Direction == CScrollRect.ScrollDirection.Horizontal;
			}
		}

		// Token: 0x1700151E RID: 5406
		// (get) Token: 0x0600BB4A RID: 47946 RVA: 0x005532F7 File Offset: 0x005514F7
		private bool Vertical
		{
			get
			{
				return this.Direction == CScrollRect.ScrollDirection.Vertical;
			}
		}

		// Token: 0x1700151F RID: 5407
		// (get) Token: 0x0600BB4B RID: 47947 RVA: 0x00553302 File Offset: 0x00551502
		// (set) Token: 0x0600BB4C RID: 47948 RVA: 0x0055330A File Offset: 0x0055150A
		public float ScrollSpeed
		{
			get
			{
				return this.scrollSpeed;
			}
			set
			{
				this.scrollSpeed = value;
			}
		}

		// Token: 0x17001520 RID: 5408
		// (get) Token: 0x0600BB4D RID: 47949 RVA: 0x00553313 File Offset: 0x00551513
		// (set) Token: 0x0600BB4E RID: 47950 RVA: 0x0055331B File Offset: 0x0055151B
		public float DampedCoefficient
		{
			get
			{
				return this.dampedCoefficient;
			}
			set
			{
				this.dampedCoefficient = value;
			}
		}

		// Token: 0x14000090 RID: 144
		// (add) Token: 0x0600BB4F RID: 47951 RVA: 0x00553324 File Offset: 0x00551524
		// (remove) Token: 0x0600BB50 RID: 47952 RVA: 0x0055335C File Offset: 0x0055155C
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnScrollEvent;

		// Token: 0x14000091 RID: 145
		// (add) Token: 0x0600BB51 RID: 47953 RVA: 0x00553394 File Offset: 0x00551594
		// (remove) Token: 0x0600BB52 RID: 47954 RVA: 0x005533CC File Offset: 0x005515CC
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnListenerDimensionsChangeEvent;

		// Token: 0x17001521 RID: 5409
		// (get) Token: 0x0600BB53 RID: 47955 RVA: 0x00553401 File Offset: 0x00551601
		// (set) Token: 0x0600BB54 RID: 47956 RVA: 0x00553409 File Offset: 0x00551609
		public CScrollRect.State CurState { get; private set; }

		// Token: 0x17001522 RID: 5410
		// (get) Token: 0x0600BB55 RID: 47957 RVA: 0x00553412 File Offset: 0x00551612
		// (set) Token: 0x0600BB56 RID: 47958 RVA: 0x0055341A File Offset: 0x0055161A
		public bool CanScroll { get; set; } = true;

		// Token: 0x17001523 RID: 5411
		// (get) Token: 0x0600BB57 RID: 47959 RVA: 0x00553423 File Offset: 0x00551623
		// (set) Token: 0x0600BB58 RID: 47960 RVA: 0x0055342B File Offset: 0x0055162B
		public float AdjustSpeed
		{
			get
			{
				return this.adjustSpeed;
			}
			set
			{
				this.adjustSpeed = value;
			}
		}

		// Token: 0x17001524 RID: 5412
		// (get) Token: 0x0600BB59 RID: 47961 RVA: 0x00553434 File Offset: 0x00551634
		// (set) Token: 0x0600BB5A RID: 47962 RVA: 0x0055343C File Offset: 0x0055163C
		public bool IsPointerInside
		{
			get
			{
				return this._pointerInside;
			}
			private set
			{
				bool flag = this._pointerInside == value;
				if (!flag)
				{
					this._pointerInside = value;
					if (value)
					{
						CScrollRect._pointerInsideCount++;
					}
					else
					{
						CScrollRect._pointerInsideCount--;
					}
				}
			}
		}

		// Token: 0x17001525 RID: 5413
		// (get) Token: 0x0600BB5B RID: 47963 RVA: 0x0055347F File Offset: 0x0055167F
		public static bool IsPointerOverAnyCScrollRect
		{
			get
			{
				return CScrollRect._pointerInsideCount > 0;
			}
		}

		// Token: 0x17001526 RID: 5414
		// (get) Token: 0x0600BB5C RID: 47964 RVA: 0x00553489 File Offset: 0x00551689
		private Camera EventCamera
		{
			get
			{
				return UIManager.Instance.UiCamera;
			}
		}

		// Token: 0x17001527 RID: 5415
		// (get) Token: 0x0600BB5D RID: 47965 RVA: 0x00553498 File Offset: 0x00551698
		private Vector2 ClampMin
		{
			get
			{
				return this.Viewport.rect.min;
			}
		}

		// Token: 0x17001528 RID: 5416
		// (get) Token: 0x0600BB5E RID: 47966 RVA: 0x005534B8 File Offset: 0x005516B8
		private Vector2 ClampMax
		{
			get
			{
				return this.Viewport.rect.max;
			}
		}

		// Token: 0x17001529 RID: 5417
		// (get) Token: 0x0600BB5F RID: 47967 RVA: 0x005534D8 File Offset: 0x005516D8
		private Vector2 ContentLocalPos
		{
			get
			{
				return (this.Content.parent != this.Viewport) ? this.Viewport.InverseTransformPoint(this.Content.position) : this.Content.localPosition;
			}
		}

		// Token: 0x1700152A RID: 5418
		// (get) Token: 0x0600BB60 RID: 47968 RVA: 0x00553528 File Offset: 0x00551728
		private Vector2 ContentMin
		{
			get
			{
				return this.ContentLocalPos + this.Content.rect.min;
			}
		}

		// Token: 0x1700152B RID: 5419
		// (get) Token: 0x0600BB61 RID: 47969 RVA: 0x00553554 File Offset: 0x00551754
		private Vector2 ContentMax
		{
			get
			{
				return this.ContentLocalPos + this.Content.rect.max;
			}
		}

		// Token: 0x1700152C RID: 5420
		// (get) Token: 0x0600BB62 RID: 47970 RVA: 0x00553580 File Offset: 0x00551780
		private float ContentSize
		{
			get
			{
				return this.Horizontal ? this.Content.rect.width : this.Content.rect.height;
			}
		}

		// Token: 0x1700152D RID: 5421
		// (get) Token: 0x0600BB63 RID: 47971 RVA: 0x005535C0 File Offset: 0x005517C0
		private float ViewportSize
		{
			get
			{
				return this.Horizontal ? this.Viewport.rect.width : this.Viewport.rect.height;
			}
		}

		// Token: 0x0600BB64 RID: 47972 RVA: 0x00553600 File Offset: 0x00551800
		private void Awake()
		{
			bool flag = this.ScrollBar != null;
			if (flag)
			{
				this.ScrollBar.value = 0f;
				this.SetScrollBarActive(false);
			}
		}

		// Token: 0x0600BB65 RID: 47973 RVA: 0x0055363C File Offset: 0x0055183C
		private void OnEnable()
		{
			bool flag = !this._initFinished;
			if (flag)
			{
				this.Init();
			}
			this._canControl = (this.Viewport != null && RectTransformUtility.RectangleContainsScreenPoint(this.Viewport, Input.mousePosition, this.EventCamera));
			this.SnapContentToBounds();
			this.UpdateScrollBarValue();
			this.CacheSizes();
		}

		// Token: 0x0600BB66 RID: 47974 RVA: 0x005536A8 File Offset: 0x005518A8
		private void OnDisable()
		{
			this.CurState = CScrollRect.State.None;
			this._speed = Vector2.zero;
			this._horizontalAdjustCode = 0;
			this._verticalAdjustCode = 0;
			this.IsPointerInside = false;
			this.SnapContentToBounds();
			bool flag = this.ScrollBar != null;
			if (flag)
			{
				this.ScrollBar.value = Mathf.Clamp01(this.ScrollBar.value);
			}
			bool flag2 = this.content.anchoredPosition.y < 0f;
			if (flag2)
			{
				this.content.anchoredPosition = this.content.anchoredPosition.SetY(0f);
			}
		}

		// Token: 0x0600BB67 RID: 47975 RVA: 0x00553750 File Offset: 0x00551950
		private void Update()
		{
			bool flag = !this.CanScroll || this.ScrollBar == null;
			if (!flag)
			{
				this.HandleMouseScroll();
			}
		}

		// Token: 0x0600BB68 RID: 47976 RVA: 0x00553784 File Offset: 0x00551984
		private void LateUpdate()
		{
			this.UpdatePosition();
			bool flag = Mathf.Abs(this._contentNoteSize - this.ContentSize) > 2f;
			if (flag)
			{
				this.AdjustToFitNewContentSize();
			}
			this.CheckSizeChanges();
		}

		// Token: 0x0600BB69 RID: 47977 RVA: 0x005537C4 File Offset: 0x005519C4
		public void OnBeginDrag(PointerEventData eventData)
		{
			bool flag = eventData.button != PointerEventData.InputButton.Left || !this.CanScroll;
			if (!flag)
			{
				bool flag2 = !this._initFinished;
				if (flag2)
				{
					this.Init();
				}
				bool flag3 = !this.IsPointerActive() || this.CurState == CScrollRect.State.AutoAdjust || this.CurState == CScrollRect.State.Disable;
				if (!flag3)
				{
					this._horizontalAdjustCode = 0;
					this._verticalAdjustCode = 0;
					this._speed = Vector2.zero;
					bool flag4 = RectTransformUtility.ScreenPointToLocalPointInRectangle(this.Viewport, eventData.position, this.EventCamera, out this._dragStartPosition);
					if (flag4)
					{
						this._prevDragPosition = this._dragStartPosition;
						this._dragStartTime = Time.time;
						this.CurState = CScrollRect.State.Dragging;
					}
				}
			}
		}

		// Token: 0x0600BB6A RID: 47978 RVA: 0x00553884 File Offset: 0x00551A84
		public void OnDrag(PointerEventData eventData)
		{
			bool flag = !this._initFinished;
			if (flag)
			{
				this.Init();
			}
			bool flag2 = this.CurState != CScrollRect.State.Dragging;
			if (!flag2)
			{
				Vector2 position = eventData.position;
				position.x = Mathf.Clamp(position.x, 0f, (float)Screen.width);
				position.y = Mathf.Clamp(position.y, 0f, (float)Screen.height);
				Vector2 curPos;
				bool flag3 = !RectTransformUtility.ScreenPointToLocalPointInRectangle(this.Viewport, position, this.EventCamera, out curPos);
				if (!flag3)
				{
					Vector2 offset = curPos - this._prevDragPosition;
					bool flag4 = !this.Horizontal;
					if (flag4)
					{
						offset.x = 0f;
					}
					bool flag5 = !this.Vertical;
					if (flag5)
					{
						offset.y = 0f;
					}
					this.Content.anchoredPosition += offset;
					this._prevDragPosition = curPos;
					this.UpdateScrollBarValue();
					this.NotifyScrollBarActivity();
					Action onScrollEvent = this.OnScrollEvent;
					if (onScrollEvent != null)
					{
						onScrollEvent();
					}
				}
			}
		}

		// Token: 0x0600BB6B RID: 47979 RVA: 0x005539A4 File Offset: 0x00551BA4
		public void OnEndDrag(PointerEventData eventData)
		{
			bool flag = !this._initFinished;
			if (flag)
			{
				this.Init();
			}
			bool flag2 = this.CurState != CScrollRect.State.Dragging;
			if (!flag2)
			{
				this.UpdateAutoAdjustCode();
				Vector2 position = eventData.position;
				position.x = Mathf.Clamp(position.x, 0f, (float)Screen.width);
				position.y = Mathf.Clamp(position.y, 0f, (float)Screen.height);
				bool flag3 = this._horizontalAdjustCode != 0 || this._verticalAdjustCode != 0;
				if (flag3)
				{
					this._speed = Vector2.one * this.AdjustSpeed;
					this._speed.x = this._speed.x * Mathf.Sign((float)this._horizontalAdjustCode);
					this._speed.y = this._speed.y * Mathf.Sign((float)this._verticalAdjustCode);
					bool flag4 = !this.Horizontal;
					if (flag4)
					{
						this._speed.x = 0f;
					}
					bool flag5 = !this.Vertical;
					if (flag5)
					{
						this._speed.y = 0f;
					}
					this.CurState = CScrollRect.State.AutoAdjust;
				}
				else
				{
					Vector2 curPos;
					bool flag6 = !RectTransformUtility.ScreenPointToLocalPointInRectangle(this.Viewport, position, this.EventCamera, out curPos);
					if (!flag6)
					{
						bool flag7 = (this.Horizontal && this.Content.rect.width < this.Viewport.rect.width) || (this.Vertical && this.Content.rect.height < this.Viewport.rect.height);
						if (flag7)
						{
							this.CurState = CScrollRect.State.Disable;
							this._speed = Vector2.zero;
							DOVirtual.Float(this.ScrollBar.value, 0f, 0.3f, delegate(float stepValue)
							{
								this.ScrollBar.value = stepValue;
							}).SetUpdate(true).SetAutoKill(true).OnComplete(delegate
							{
								this.CurState = CScrollRect.State.None;
							});
						}
						else
						{
							Vector2 totalOffset = curPos - this._dragStartPosition;
							float dragDur = Mathf.Max(Time.time - this._dragStartTime, 0.0001f);
							this._speed.x = Mathf.Min(this.ScrollSpeed, Mathf.Abs(totalOffset.x / dragDur)) * Mathf.Sign(totalOffset.x);
							this._speed.y = Mathf.Min(this.ScrollSpeed, Mathf.Abs(totalOffset.y / dragDur)) * Mathf.Sign(totalOffset.y);
							bool flag8 = !this.Horizontal;
							if (flag8)
							{
								this._speed.x = 0f;
							}
							bool flag9 = !this.Vertical;
							if (flag9)
							{
								this._speed.y = 0f;
							}
							this.CurState = CScrollRect.State.DampedMoving;
						}
					}
				}
			}
		}

		// Token: 0x0600BB6C RID: 47980 RVA: 0x00553C94 File Offset: 0x00551E94
		public void OnPointerEnter(PointerEventData eventData)
		{
			this.IsPointerInside = true;
			CScrollbar customScrollbar = this.ScrollBar as CScrollbar;
			bool flag = customScrollbar != null;
			if (flag)
			{
				customScrollbar.NotifyViewportHover(true);
			}
		}

		// Token: 0x0600BB6D RID: 47981 RVA: 0x00553CC8 File Offset: 0x00551EC8
		public void OnPointerExit(PointerEventData eventData)
		{
			this.IsPointerInside = false;
			CScrollbar customScrollbar = this.ScrollBar as CScrollbar;
			bool flag = customScrollbar != null;
			if (flag)
			{
				customScrollbar.NotifyViewportHover(false);
			}
		}

		// Token: 0x0600BB6E RID: 47982 RVA: 0x00553CFC File Offset: 0x00551EFC
		public void OnPointerClick(PointerEventData eventData)
		{
			bool flag = this.CurState == CScrollRect.State.Dragging;
			if (!flag)
			{
				Action onClick = this._onClick;
				if (onClick != null)
				{
					onClick();
				}
				AudioManager.Instance.PlaySound("ui_default_click_left", false, false);
			}
		}

		// Token: 0x0600BB6F RID: 47983 RVA: 0x00553D3D File Offset: 0x00551F3D
		public void SetClick(Action onClick)
		{
			this._onClick = onClick;
		}

		// Token: 0x0600BB70 RID: 47984 RVA: 0x00553D48 File Offset: 0x00551F48
		void IScrollHandler.OnScroll(PointerEventData eventData)
		{
			bool flag = this.blockScrollEvent;
			if (flag)
			{
				eventData.Use();
			}
		}

		// Token: 0x0600BB71 RID: 47985 RVA: 0x00553D68 File Offset: 0x00551F68
		public bool IsInViewport(RectTransform target, float tolerance = 5f)
		{
			bool horizontalVisible = true;
			bool verticalVisible = true;
			Vector2 localPos = target.localPosition;
			bool flag = target.parent != this.Viewport;
			if (flag)
			{
				localPos = this.Viewport.InverseTransformPoint(target.position);
			}
			Vector2 targetMin = localPos + target.rect.min;
			Vector2 targetMax = localPos + target.rect.max;
			bool flag2 = targetMin.x - this.Viewport.rect.max.x > tolerance || this.Viewport.rect.min.x - targetMax.x > tolerance;
			if (flag2)
			{
				horizontalVisible = false;
			}
			bool flag3 = targetMin.y - this.Viewport.rect.max.y > tolerance || this.Viewport.rect.min.y - targetMax.y > tolerance;
			if (flag3)
			{
				verticalVisible = false;
			}
			return horizontalVisible && verticalVisible;
		}

		// Token: 0x0600BB72 RID: 47986 RVA: 0x00553E90 File Offset: 0x00552090
		public void SetScrollEnable(bool canScroll)
		{
			bool flag = this.ScrollBar != null;
			if (flag)
			{
				this.ScrollBar.enabled = canScroll;
			}
			this.CanScroll = canScroll;
		}

		// Token: 0x0600BB73 RID: 47987 RVA: 0x00553EC4 File Offset: 0x005520C4
		public void ScrollTo(Vector2 targetAnchorPosition, float duration = 0.3f)
		{
			Vector2 startPos = this.Content.anchoredPosition;
			DOVirtual.Float(0f, 1f, duration, delegate(float stepValue)
			{
				this.CurState = CScrollRect.State.AutoAdjust;
				bool flag = this.Content != null;
				if (flag)
				{
					this.Content.anchoredPosition = Vector2.Lerp(startPos, targetAnchorPosition, stepValue);
				}
			}).SetUpdate(true).SetAutoKill(true).OnComplete(delegate
			{
				this.CurState = CScrollRect.State.None;
				this.UpdateScrollBarValue();
				Action onScrollEnd = this.OnScrollEnd;
				if (onScrollEnd != null)
				{
					onScrollEnd();
				}
			});
		}

		// Token: 0x0600BB74 RID: 47988 RVA: 0x00553F34 File Offset: 0x00552134
		public bool ScrollTo(RectTransform refersRectTrans, float duration = 0.3f)
		{
			bool flag = this.Direction == CScrollRect.ScrollDirection.Vertical;
			bool result;
			if (flag)
			{
				float scrollMaxY = this.Content.rect.height - this.Viewport.rect.height;
				bool flag2 = scrollMaxY <= 0f;
				if (flag2)
				{
					result = false;
				}
				else
				{
					float yOffset = this.Viewport.InverseTransformPoint(refersRectTrans.position).y;
					Vector2 scrollToPos = this.Content.anchoredPosition - Vector2.up * yOffset;
					scrollToPos.x = 0f;
					scrollToPos.y = Mathf.Clamp(scrollToPos.y, 0f, scrollMaxY);
					this.ScrollTo(scrollToPos, duration);
					result = true;
				}
			}
			else
			{
				float scrollMaxX = this.Content.rect.width - this.Viewport.rect.width;
				bool flag3 = scrollMaxX <= 0f;
				if (flag3)
				{
					result = false;
				}
				else
				{
					float scrollToPosX = refersRectTrans.anchoredPosition.x + refersRectTrans.rect.width - this.Viewport.rect.width;
					scrollToPosX = Mathf.Clamp(scrollToPosX, 0f, scrollMaxX);
					this.ScrollTo(new Vector2(-scrollToPosX, 0f), duration);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600BB75 RID: 47989 RVA: 0x0055409C File Offset: 0x0055229C
		private void Init()
		{
			this.ScrollBar.onValueChanged.AddListener(new UnityAction<float>(this.OnScrollBarValueChanged));
			float size = this.Horizontal ? Mathf.Clamp01(this.Viewport.rect.width / Mathf.Max(this.Content.rect.width, 0.0001f)) : Mathf.Clamp01(this.Viewport.rect.height / Mathf.Max(this.Content.rect.height, 0.0001f));
			CScrollbar customScrollbar = this.ScrollBar as CScrollbar;
			bool flag = customScrollbar != null;
			if (flag)
			{
				customScrollbar.HandleSize = size;
				customScrollbar.OnClickSetValueEvent += this.OnScrollBarValueClickSet;
			}
			else
			{
				this.ScrollBar.size = size;
			}
			this.SetScrollBarActive(this.ScrollBar.size < 1f);
			this._contentNoteSize = this.ContentSize;
			this.CacheSizes();
			this._initFinished = true;
			this._forceDimensionRefresh = true;
		}

		// Token: 0x0600BB76 RID: 47990 RVA: 0x005541BC File Offset: 0x005523BC
		private void SetScrollBarActive(bool isActive)
		{
			bool flag = this.ScrollBar == null;
			if (!flag)
			{
				bool flag2 = this.showScrollBar;
				if (flag2)
				{
					this.ScrollBar.gameObject.SetActive(true);
				}
				else
				{
					bool flag3 = this.hideScrollBar;
					if (flag3)
					{
						this.ScrollBar.gameObject.SetActive(false);
					}
					else
					{
						this.ScrollBar.gameObject.SetActive(isActive);
					}
				}
				CScrollbar cScrollbar = this.ScrollBar as CScrollbar;
				bool flag4 = cScrollbar != null;
				if (flag4)
				{
					cScrollbar.DisableHover = (this.showScrollBar || this.hideScrollBar);
				}
			}
		}

		// Token: 0x0600BB77 RID: 47991 RVA: 0x00554258 File Offset: 0x00552458
		private void HandleMouseScroll()
		{
			bool flag = this.CurState != CScrollRect.State.None && this.CurState != CScrollRect.State.DampedMoving;
			if (!flag)
			{
				float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
				bool flag2 = Mathf.Abs(scrollDelta) < 0.05f;
				if (!flag2)
				{
					bool flag3 = !this.IsPointerActive() || !this.IsFocusScroll();
					if (!flag3)
					{
						bool consumed = false;
						bool flag4 = this.Vertical && this.Content.rect.height > this.Viewport.rect.height;
						if (flag4)
						{
							consumed = this.ProcessScrollAxis(scrollDelta, true);
						}
						bool flag5 = !consumed && this.Horizontal && this.Content.rect.width > this.Viewport.rect.width;
						if (flag5)
						{
							this.ProcessScrollAxis(scrollDelta * -1f, false);
						}
						this.NotifyScrollBarActivity();
					}
				}
			}
		}

		// Token: 0x0600BB78 RID: 47992 RVA: 0x00554360 File Offset: 0x00552560
		private bool ProcessScrollAxis(float scrollDelta, bool vertical)
		{
			bool edgePingFlag = false;
			float tolerance = 0.5f;
			bool result;
			if (vertical)
			{
				bool atBottom = this.ContentMin.y >= this.ClampMin.y - tolerance;
				bool atTop = this.ContentMax.y <= this.ClampMax.y + tolerance;
				bool flag = scrollDelta < 0f && atBottom;
				if (flag)
				{
					edgePingFlag = true;
					bool flag2 = this.ScrollBar.direction == Scrollbar.Direction.TopToBottom;
					if (flag2)
					{
						this.DoEdgePing(RectTransform.Edge.Bottom);
					}
					else
					{
						bool flag3 = this.ScrollBar.direction == Scrollbar.Direction.BottomToTop;
						if (flag3)
						{
							this.DoEdgePing(RectTransform.Edge.Top);
						}
					}
				}
				else
				{
					bool flag4 = scrollDelta > 0f && atTop;
					if (flag4)
					{
						edgePingFlag = true;
						bool flag5 = this.ScrollBar.direction == Scrollbar.Direction.TopToBottom;
						if (flag5)
						{
							this.DoEdgePing(RectTransform.Edge.Top);
						}
						else
						{
							bool flag6 = this.ScrollBar.direction == Scrollbar.Direction.BottomToTop;
							if (flag6)
							{
								this.DoEdgePing(RectTransform.Edge.Bottom);
							}
						}
					}
				}
				bool flag7 = !edgePingFlag;
				if (flag7)
				{
					bool flag8 = this.ScrollBar.direction == Scrollbar.Direction.TopToBottom;
					if (flag8)
					{
						this._speed.y = -this.ScrollSpeed * Mathf.Sign(scrollDelta);
					}
					else
					{
						bool flag9 = this.ScrollBar.direction == Scrollbar.Direction.BottomToTop;
						if (flag9)
						{
							this._speed.y = this.ScrollSpeed * Mathf.Sign(scrollDelta);
						}
					}
					this.CurState = CScrollRect.State.DampedMoving;
				}
				result = true;
			}
			else
			{
				bool horizontal = this.Horizontal;
				if (horizontal)
				{
					bool flag10 = this.ScrollBar.direction == Scrollbar.Direction.LeftToRight;
					if (flag10)
					{
						bool flag11 = scrollDelta > 0f && this.ContentMax.x <= this.ClampMax.x + tolerance;
						if (flag11)
						{
							edgePingFlag = true;
							this.DoEdgePing(RectTransform.Edge.Left);
						}
						else
						{
							bool flag12 = scrollDelta < 0f && this.ContentMin.x >= this.ClampMin.x - tolerance;
							if (flag12)
							{
								edgePingFlag = true;
								this.DoEdgePing(RectTransform.Edge.Right);
							}
						}
					}
					else
					{
						bool flag13 = this.ScrollBar.direction == Scrollbar.Direction.RightToLeft;
						if (flag13)
						{
							bool flag14 = scrollDelta > 0f && this.ContentMin.x >= this.ClampMin.x - tolerance;
							if (flag14)
							{
								edgePingFlag = true;
								this.DoEdgePing(RectTransform.Edge.Right);
							}
							else
							{
								bool flag15 = scrollDelta < 0f && this.ContentMax.x <= this.ClampMax.x + tolerance;
								if (flag15)
								{
									edgePingFlag = true;
									this.DoEdgePing(RectTransform.Edge.Left);
								}
							}
						}
					}
				}
				bool flag16 = !edgePingFlag;
				if (flag16)
				{
					bool flag17 = this.ScrollBar.direction == Scrollbar.Direction.LeftToRight;
					if (flag17)
					{
						this._speed.x = -this.ScrollSpeed * Mathf.Sign(scrollDelta);
					}
					else
					{
						bool flag18 = this.ScrollBar.direction == Scrollbar.Direction.RightToLeft;
						if (flag18)
						{
							this._speed.x = this.ScrollSpeed * Mathf.Sign(scrollDelta);
						}
					}
					this.CurState = CScrollRect.State.DampedMoving;
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600BB79 RID: 47993 RVA: 0x00554678 File Offset: 0x00552878
		private void UpdatePosition()
		{
			bool flag = this.CurState != CScrollRect.State.AutoAdjust && this.CurState != CScrollRect.State.DampedMoving;
			if (!flag)
			{
				Vector2 stepValue = this._speed * Time.unscaledDeltaTime;
				this.Content.anchoredPosition += stepValue;
				Vector2 fixVector = Vector2.zero;
				bool flag2 = this.CurState == CScrollRect.State.AutoAdjust;
				if (flag2)
				{
					bool horizontal = this.Horizontal;
					if (horizontal)
					{
						bool flag3 = (this._horizontalAdjustCode == 1 && this.ContentMin.x > this.ClampMin.x) || (this._horizontalAdjustCode == -1 && this.ContentMin.x < this.ClampMin.x);
						if (flag3)
						{
							fixVector.x = this.ClampMin.x - this.ContentMin.x;
							this._speed.x = 0f;
						}
						bool flag4 = (this._horizontalAdjustCode == 2 && this.ContentMax.x > this.ClampMax.x) || (this._horizontalAdjustCode == -2 && this.ContentMax.x < this.ClampMax.x);
						if (flag4)
						{
							fixVector.x = this.ClampMax.x - this.ContentMax.x;
							this._speed.x = 0f;
						}
					}
					bool vertical = this.Vertical;
					if (vertical)
					{
						bool flag5 = (this._verticalAdjustCode == 1 && this.ContentMax.y > this.ClampMax.y) || (this._verticalAdjustCode == -1 && this.ContentMax.y < this.ClampMax.y);
						if (flag5)
						{
							fixVector.y = this.ClampMax.y - this.ContentMax.y;
							this._speed.y = 0f;
						}
						bool flag6 = (this._verticalAdjustCode == 2 && this.ContentMin.y > this.ClampMin.y) || (this._verticalAdjustCode == -2 && this.ContentMin.y < this.ClampMin.y);
						if (flag6)
						{
							fixVector.y = this.ClampMin.y - this.ContentMin.y;
							this._speed.y = 0f;
						}
					}
				}
				else
				{
					bool flag7 = this.CurState == CScrollRect.State.DampedMoving;
					if (flag7)
					{
						bool horizontal2 = this.Horizontal;
						if (horizontal2)
						{
							bool flag8 = this._speed.x > 0f;
							if (flag8)
							{
								this._speed.x = Mathf.Max(0f, this._speed.x - this.ScrollSpeed * this.DampedCoefficient * Time.unscaledDeltaTime);
								bool flag9 = this.ContentMin.x > this.ClampMin.x;
								if (flag9)
								{
									fixVector.x = this.ClampMin.x - this.ContentMin.x;
									this._speed.x = 0f;
								}
							}
							else
							{
								this._speed.x = Mathf.Min(0f, this._speed.x + this.ScrollSpeed * this.DampedCoefficient * Time.unscaledDeltaTime);
								bool flag10 = this.ContentMax.x < this.ClampMax.x;
								if (flag10)
								{
									fixVector.x = this.ClampMax.x - this.ContentMax.x;
									this._speed.x = 0f;
								}
							}
						}
						bool vertical2 = this.Vertical;
						if (vertical2)
						{
							bool flag11 = this._speed.y > 0f;
							if (flag11)
							{
								this._speed.y = Mathf.Max(0f, this._speed.y - this.ScrollSpeed * this.DampedCoefficient * Time.unscaledDeltaTime);
								bool flag12 = this.ContentMin.y > this.ClampMin.y;
								if (flag12)
								{
									fixVector.y = this.ClampMin.y - this.ContentMin.y;
									this._speed.y = 0f;
								}
							}
							else
							{
								this._speed.y = Mathf.Min(0f, this._speed.y + this.ScrollSpeed * this.DampedCoefficient * Time.unscaledDeltaTime);
								bool flag13 = this.ContentMax.y < this.ClampMax.y;
								if (flag13)
								{
									fixVector.y = this.ClampMax.y - this.ContentMax.y;
									this._speed.y = 0f;
								}
							}
						}
					}
				}
				this.Content.anchoredPosition += fixVector;
				Action onScrollEvent = this.OnScrollEvent;
				if (onScrollEvent != null)
				{
					onScrollEvent();
				}
				this.UpdateScrollBarValue();
				bool flag14 = Mathf.Approximately(this._speed.x, 0f) && Mathf.Approximately(this._speed.y, 0f);
				if (flag14)
				{
					this.CurState = CScrollRect.State.None;
					this._horizontalAdjustCode = 0;
					this._verticalAdjustCode = 0;
				}
			}
		}

		// Token: 0x0600BB7A RID: 47994 RVA: 0x00554C10 File Offset: 0x00552E10
		private void UpdateAutoAdjustCode()
		{
			this._horizontalAdjustCode = 0;
			this._verticalAdjustCode = 0;
			bool horizontal = this.Horizontal;
			if (horizontal)
			{
				bool flag = this.Content.rect.width > this.Viewport.rect.width;
				if (flag)
				{
					bool flag2 = this.ContentMax.x < this.ClampMax.x;
					if (flag2)
					{
						this._horizontalAdjustCode = 2;
					}
					bool flag3 = this.ContentMin.x > this.ClampMin.x;
					if (flag3)
					{
						this._horizontalAdjustCode = -1;
					}
				}
				else
				{
					bool flag4 = this.ScrollBar.direction == Scrollbar.Direction.LeftToRight;
					if (flag4)
					{
						bool flag5 = this.ContentMin.x > this.ClampMin.x;
						if (flag5)
						{
							this._horizontalAdjustCode = -1;
						}
						bool flag6 = this.ContentMin.x < this.ClampMin.x;
						if (flag6)
						{
							this._horizontalAdjustCode = 1;
						}
					}
					else
					{
						bool flag7 = this.ScrollBar.direction == Scrollbar.Direction.RightToLeft;
						if (flag7)
						{
							bool flag8 = this.ContentMax.x < this.ClampMax.x;
							if (flag8)
							{
								this._horizontalAdjustCode = 2;
							}
							bool flag9 = this.ContentMax.x > this.ClampMax.x;
							if (flag9)
							{
								this._horizontalAdjustCode = -2;
							}
						}
					}
				}
			}
			bool vertical = this.Vertical;
			if (vertical)
			{
				bool flag10 = this.Content.rect.height > this.Viewport.rect.height;
				if (flag10)
				{
					bool flag11 = this.ContentMin.y > this.ClampMin.y;
					if (flag11)
					{
						this._verticalAdjustCode = -2;
					}
					bool flag12 = this.ContentMax.y < this.ClampMax.y;
					if (flag12)
					{
						this._verticalAdjustCode = 1;
					}
				}
				else
				{
					bool flag13 = this.ScrollBar.direction == Scrollbar.Direction.BottomToTop;
					if (flag13)
					{
						bool flag14 = this.ContentMin.y > this.ClampMin.y;
						if (flag14)
						{
							this._verticalAdjustCode = -2;
						}
						bool flag15 = this.ContentMin.y < this.ClampMin.y;
						if (flag15)
						{
							this._verticalAdjustCode = 2;
						}
					}
					else
					{
						bool flag16 = this.ScrollBar.direction == Scrollbar.Direction.TopToBottom;
						if (flag16)
						{
							bool flag17 = this.ContentMax.y < this.ClampMax.y;
							if (flag17)
							{
								this._verticalAdjustCode = 1;
							}
							bool flag18 = this.ContentMax.y > this.ClampMax.y;
							if (flag18)
							{
								this._verticalAdjustCode = -1;
							}
						}
					}
				}
			}
		}

		// Token: 0x0600BB7B RID: 47995 RVA: 0x00554ED4 File Offset: 0x005530D4
		private void DoEdgePing(RectTransform.Edge edge)
		{
			CScrollRect.<>c__DisplayClass111_0 CS$<>8__locals1 = new CScrollRect.<>c__DisplayClass111_0();
			CS$<>8__locals1.<>4__this = this;
			bool flag = this.CurState == CScrollRect.State.EdgePing;
			if (!flag)
			{
				this.CurState = CScrollRect.State.EdgePing;
				float pingSize = this.ViewportSize * 0.1f;
				CS$<>8__locals1.startPos = (this.Horizontal ? this.Content.anchoredPosition.x : this.Content.anchoredPosition.y);
				CS$<>8__locals1.endPos = CS$<>8__locals1.startPos;
				switch (edge)
				{
				case RectTransform.Edge.Left:
					CS$<>8__locals1.endPos = CS$<>8__locals1.startPos - pingSize;
					break;
				case RectTransform.Edge.Right:
					CS$<>8__locals1.endPos = CS$<>8__locals1.startPos + pingSize;
					break;
				case RectTransform.Edge.Top:
					CS$<>8__locals1.endPos = CS$<>8__locals1.startPos - pingSize;
					break;
				case RectTransform.Edge.Bottom:
					CS$<>8__locals1.endPos = CS$<>8__locals1.startPos + pingSize;
					break;
				}
				DOTween.Sequence().Append(DOVirtual.Float(0f, 1f, 0.15f, new TweenCallback<float>(CS$<>8__locals1.<DoEdgePing>g__SetStepPos|0)).SetUpdate(true).SetAutoKill(true)).Append(DOVirtual.Float(1f, 0f, 0.15f, new TweenCallback<float>(CS$<>8__locals1.<DoEdgePing>g__SetStepPos|0)).SetUpdate(true).SetAutoKill(true)).AppendCallback(delegate
				{
					CS$<>8__locals1.<>4__this.CurState = CScrollRect.State.None;
				}).SetUpdate(true).SetAutoKill(true);
			}
		}

		// Token: 0x0600BB7C RID: 47996 RVA: 0x00555038 File Offset: 0x00553238
		private bool IsInScrollArea()
		{
			bool flag = this.ScrollBar != null && RectTransformUtility.RectangleContainsScreenPoint(this.ScrollBar.transform as RectTransform, Input.mousePosition, this.EventCamera);
			return !flag && RectTransformUtility.RectangleContainsScreenPoint(this.Viewport, Input.mousePosition, this.EventCamera);
		}

		// Token: 0x0600BB7D RID: 47997 RVA: 0x005550A4 File Offset: 0x005532A4
		private bool IsFocusScroll()
		{
			RaycastAllManager raycastManager = SingletonObject.getInstance<RaycastAllManager>();
			bool flag = raycastManager == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				List<RaycastResult> _raycastResultList = raycastManager.GetCurrentFrameResults();
				bool flag2 = _raycastResultList.Count > 0;
				if (flag2)
				{
					CScrollRect component = _raycastResultList[0].gameObject.GetComponentInParent<CScrollRect>();
					bool flag3 = component == null;
					result = (!flag3 && component.Equals(this));
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x0600BB7E RID: 47998 RVA: 0x00555118 File Offset: 0x00553318
		private float UpdateScrollBarSizeAndGetRealBarValue()
		{
			float scrollValue = 0f;
			bool horizontal = this.Horizontal;
			if (horizontal)
			{
				bool flag = this.ScrollBar.direction == Scrollbar.Direction.LeftToRight;
				if (flag)
				{
					scrollValue = (this.ClampMin.x - this.ContentMin.x) / (this.Content.rect.width - this.Viewport.rect.width);
				}
				else
				{
					bool flag2 = this.ScrollBar.direction == Scrollbar.Direction.RightToLeft;
					if (flag2)
					{
						scrollValue = (this.ContentMax.x - this.ClampMax.x) / (this.Content.rect.width - this.Viewport.rect.width);
					}
				}
				float sizeValue = (this.Content.rect.width > 0f) ? Mathf.Clamp01(this.Viewport.rect.width / this.Content.rect.width) : 1f;
				this.ApplyHandleSize(sizeValue);
			}
			bool vertical = this.Vertical;
			if (vertical)
			{
				bool flag3 = this.ScrollBar.direction == Scrollbar.Direction.TopToBottom;
				if (flag3)
				{
					scrollValue = (this.ContentMax.y - this.ClampMax.y) / (this.Content.rect.height - this.Viewport.rect.height);
				}
				else
				{
					bool flag4 = this.ScrollBar.direction == Scrollbar.Direction.BottomToTop;
					if (flag4)
					{
						scrollValue = (this.ClampMin.y - this.ContentMin.y) / (this.Content.rect.height - this.Viewport.rect.height);
					}
				}
				float sizeValue2 = (this.Content.rect.height > 0f) ? Mathf.Clamp01(this.Viewport.rect.height / this.Content.rect.height) : 1f;
				this.ApplyHandleSize(sizeValue2);
			}
			bool flag5 = float.IsNaN(scrollValue);
			if (flag5)
			{
				scrollValue = float.MinValue;
			}
			return scrollValue;
		}

		// Token: 0x0600BB7F RID: 47999 RVA: 0x00555374 File Offset: 0x00553574
		private void ApplyHandleSize(float sizeValue)
		{
			CScrollbar custom = this.ScrollBar as CScrollbar;
			bool flag = custom != null;
			if (flag)
			{
				custom.HandleSize = sizeValue;
			}
			else
			{
				this.ScrollBar.size = sizeValue;
			}
		}

		// Token: 0x0600BB80 RID: 48000 RVA: 0x005553B0 File Offset: 0x005535B0
		private void NotifyScrollBarActivity()
		{
			CScrollbar custom = this.ScrollBar as CScrollbar;
			bool flag = custom != null;
			if (flag)
			{
				custom.NotifyScrollActivity();
			}
		}

		// Token: 0x0600BB81 RID: 48001 RVA: 0x005553DC File Offset: 0x005535DC
		public void UpdateScrollBarValue()
		{
			bool flag = this.ScrollBar == null;
			if (!flag)
			{
				float scrollValue = Mathf.Clamp01(this.UpdateScrollBarSizeAndGetRealBarValue());
				this.ScrollBar.value = scrollValue;
				this.SetScrollBarActive(this.ScrollBar.size < 1f);
			}
		}

		// Token: 0x0600BB82 RID: 48002 RVA: 0x00555430 File Offset: 0x00553630
		private void OnScrollBarValueChanged(float value)
		{
			bool flag = this.ScrollBar == null;
			if (!flag)
			{
				bool flag2 = value < 0f || value > 1f;
				if (!flag2)
				{
					this.ApplyHandleSize(this.ScrollBar.size);
					this.SetScrollBarActive(this.ScrollBar.size < 1f);
					bool flag3 = this.CurState == CScrollRect.State.None || this.CurState == CScrollRect.State.Disable;
					if (flag3)
					{
						this.SetToPositionByValue(value);
					}
				}
			}
		}

		// Token: 0x0600BB83 RID: 48003 RVA: 0x005554B4 File Offset: 0x005536B4
		private void SetToPositionByValue(float value)
		{
			bool horizontal = this.Horizontal;
			if (horizontal)
			{
				float size = this.Content.rect.width;
				float targetOffset = (size - this.Viewport.rect.width) * value;
				bool flag = size < this.Viewport.rect.width && value > 0f;
				if (flag)
				{
					targetOffset = 0f;
				}
				bool flag2 = this.ScrollBar.direction == Scrollbar.Direction.LeftToRight;
				if (flag2)
				{
					this.Content.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -targetOffset, size);
				}
				else
				{
					bool flag3 = this.ScrollBar.direction == Scrollbar.Direction.RightToLeft;
					if (flag3)
					{
						this.Content.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, -targetOffset, size);
					}
				}
				Action onScrollEvent = this.OnScrollEvent;
				if (onScrollEvent != null)
				{
					onScrollEvent();
				}
			}
			bool vertical = this.Vertical;
			if (vertical)
			{
				float size = this.Content.rect.height;
				float targetOffset = (size - this.Viewport.rect.height) * value;
				bool flag4 = size < this.Viewport.rect.height && value > 0f;
				if (flag4)
				{
					targetOffset = 0f;
				}
				bool flag5 = this.ScrollBar.direction == Scrollbar.Direction.TopToBottom;
				if (flag5)
				{
					this.Content.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -targetOffset, size);
				}
				else
				{
					bool flag6 = this.ScrollBar.direction == Scrollbar.Direction.BottomToTop;
					if (flag6)
					{
						this.Content.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, -targetOffset, size);
					}
				}
				Action onScrollEvent2 = this.OnScrollEvent;
				if (onScrollEvent2 != null)
				{
					onScrollEvent2();
				}
			}
		}

		// Token: 0x0600BB84 RID: 48004 RVA: 0x0055564C File Offset: 0x0055384C
		private void OnScrollBarValueClickSet(float value)
		{
			bool flag = this.CurState == CScrollRect.State.DampedMoving || this.CurState == CScrollRect.State.AutoAdjust;
			if (flag)
			{
				this.CurState = CScrollRect.State.None;
			}
			this.SetToPositionByValue(value);
			Action onScrollEvent = this.OnScrollEvent;
			if (onScrollEvent != null)
			{
				onScrollEvent();
			}
		}

		// Token: 0x0600BB85 RID: 48005 RVA: 0x00555698 File Offset: 0x00553898
		private void OnListenerDimensionsChange()
		{
			bool flag = this.CurState != CScrollRect.State.Disable;
			if (flag)
			{
				this.CurState = CScrollRect.State.None;
			}
			this.UpdateScrollBarValue();
			this.UpdateScrollBarSizeAndGetRealBarValue();
			Action onListenerDimensionsChangeEvent = this.OnListenerDimensionsChangeEvent;
			if (onListenerDimensionsChangeEvent != null)
			{
				onListenerDimensionsChangeEvent();
			}
		}

		// Token: 0x0600BB86 RID: 48006 RVA: 0x005556E0 File Offset: 0x005538E0
		private void AdjustToFitNewContentSize()
		{
			float scrollValue = this.UpdateScrollBarSizeAndGetRealBarValue();
			bool flag = this.ViewportSize > this.ContentSize;
			if (flag)
			{
				scrollValue = 0f;
			}
			bool flag2 = Mathf.Abs(scrollValue - this.ScrollBar.value) > 0.01f;
			if (flag2)
			{
				this.SetToPositionByValue(Mathf.Clamp01(scrollValue));
				this.UpdateScrollBarValue();
			}
			this._contentNoteSize = this.ContentSize;
		}

		// Token: 0x0600BB87 RID: 48007 RVA: 0x0055574C File Offset: 0x0055394C
		private void SnapContentToBounds()
		{
			bool flag = this.Viewport == null || this.Content == null || this.ScrollBar == null;
			if (!flag)
			{
				this.UpdateAutoAdjustCode();
				Vector2 fixVector = Vector2.zero;
				bool horizontal = this.Horizontal;
				if (horizontal)
				{
					bool flag2 = this._horizontalAdjustCode == 1 || this._horizontalAdjustCode == -1;
					if (flag2)
					{
						fixVector.x = this.ClampMin.x - this.ContentMin.x;
					}
					else
					{
						bool flag3 = this._horizontalAdjustCode == 2 || this._horizontalAdjustCode == -2;
						if (flag3)
						{
							fixVector.x = this.ClampMax.x - this.ContentMax.x;
						}
					}
				}
				bool vertical = this.Vertical;
				if (vertical)
				{
					bool flag4 = this._verticalAdjustCode == 1 || this._verticalAdjustCode == -1;
					if (flag4)
					{
						fixVector.y = this.ClampMax.y - this.ContentMax.y;
					}
					else
					{
						bool flag5 = this._verticalAdjustCode == 2 || this._verticalAdjustCode == -2;
						if (flag5)
						{
							fixVector.y = this.ClampMin.y - this.ContentMin.y;
						}
					}
				}
				bool flag6 = !Mathf.Approximately(fixVector.x, 0f) || !Mathf.Approximately(fixVector.y, 0f);
				if (flag6)
				{
					this.Content.anchoredPosition += fixVector;
				}
				this._horizontalAdjustCode = 0;
				this._verticalAdjustCode = 0;
			}
		}

		// Token: 0x0600BB88 RID: 48008 RVA: 0x005558F4 File Offset: 0x00553AF4
		private void CacheSizes()
		{
			bool flag = this.Viewport == null || this.Content == null;
			if (!flag)
			{
				this._lastViewportSize = this.Viewport.rect.size;
				this._lastContentSize = this.Content.rect.size;
			}
		}

		// Token: 0x0600BB89 RID: 48009 RVA: 0x00555958 File Offset: 0x00553B58
		private void CheckSizeChanges()
		{
			bool flag = this.Viewport == null || this.Content == null;
			if (!flag)
			{
				bool flag2 = this._forceDimensionRefresh || Vector2.Distance(this._lastViewportSize, this.Viewport.rect.size) > 0.01f || Vector2.Distance(this._lastContentSize, this.Content.rect.size) > 0.01f;
				if (flag2)
				{
					this._forceDimensionRefresh = false;
					this.CacheSizes();
					this.OnListenerDimensionsChange();
				}
			}
		}

		// Token: 0x0600BB8A RID: 48010 RVA: 0x005559FC File Offset: 0x00553BFC
		private bool IsPointerActive()
		{
			bool flag = !this.IsPointerInside;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !this.IsInScrollArea();
				result = !flag2;
			}
			return result;
		}

		// Token: 0x0400907E RID: 36990
		[Tooltip("滚动裁切视窗区域")]
		[FormerlySerializedAs("Viewport")]
		[SerializeField]
		private RectTransform viewport;

		// Token: 0x0400907F RID: 36991
		[Tooltip("滚动内容 RectTransform")]
		[FormerlySerializedAs("Content")]
		[SerializeField]
		private RectTransform content;

		// Token: 0x04009080 RID: 36992
		[FormerlySerializedAs("Direction")]
		[SerializeField]
		private CScrollRect.ScrollDirection direction;

		// Token: 0x04009081 RID: 36993
		[FormerlySerializedAs("ScrollBar")]
		[SerializeField]
		private Scrollbar scrollBar;

		// Token: 0x04009082 RID: 36994
		[FormerlySerializedAs("ScrollSpeed")]
		[SerializeField]
		private float scrollSpeed = 3000f;

		// Token: 0x04009083 RID: 36995
		[FormerlySerializedAs("DampedCoefficient")]
		[SerializeField]
		private float dampedCoefficient = 3f;

		// Token: 0x04009088 RID: 37000
		[Header("是否阻断滚动事件传递，默认不阻断")]
		[SerializeField]
		private bool blockScrollEvent;

		// Token: 0x04009089 RID: 37001
		[Header("是否永远隐藏滚动条")]
		[SerializeField]
		private bool hideScrollBar;

		// Token: 0x0400908A RID: 37002
		[Header("是否永远显示滚动条")]
		[SerializeField]
		private bool showScrollBar;

		// Token: 0x0400908B RID: 37003
		[Tooltip("当超出范围后自动靠近边界的移动速度")]
		[FormerlySerializedAs("AdjustSpeed")]
		[SerializeField]
		private float adjustSpeed = 4000f;

		// Token: 0x0400908C RID: 37004
		public Action OnScrollEnd;

		// Token: 0x0400908D RID: 37005
		private bool _initFinished;

		// Token: 0x0400908E RID: 37006
		private static int _pointerInsideCount;

		// Token: 0x0400908F RID: 37007
		private bool _pointerInside;

		// Token: 0x04009090 RID: 37008
		private bool _canControl;

		// Token: 0x04009091 RID: 37009
		private Vector2 _prevDragPosition;

		// Token: 0x04009092 RID: 37010
		private Vector2 _dragStartPosition;

		// Token: 0x04009093 RID: 37011
		private float _dragStartTime;

		// Token: 0x04009094 RID: 37012
		private Vector2 _speed;

		// Token: 0x04009095 RID: 37013
		private sbyte _horizontalAdjustCode;

		// Token: 0x04009096 RID: 37014
		private sbyte _verticalAdjustCode;

		// Token: 0x04009097 RID: 37015
		private float _contentNoteSize;

		// Token: 0x04009098 RID: 37016
		private Vector2 _lastViewportSize = Vector2.negativeInfinity;

		// Token: 0x04009099 RID: 37017
		private Vector2 _lastContentSize = Vector2.negativeInfinity;

		// Token: 0x0400909A RID: 37018
		private bool _forceDimensionRefresh;

		// Token: 0x0400909B RID: 37019
		private Action _onClick;

		// Token: 0x02002645 RID: 9797
		public enum ScrollDirection
		{
			// Token: 0x0400EA1A RID: 59930
			Horizontal,
			// Token: 0x0400EA1B RID: 59931
			Vertical
		}

		// Token: 0x02002646 RID: 9798
		public enum State
		{
			// Token: 0x0400EA1D RID: 59933
			None,
			// Token: 0x0400EA1E RID: 59934
			DampedMoving,
			// Token: 0x0400EA1F RID: 59935
			Dragging,
			// Token: 0x0400EA20 RID: 59936
			AutoAdjust,
			// Token: 0x0400EA21 RID: 59937
			EdgePing,
			// Token: 0x0400EA22 RID: 59938
			Disable
		}
	}
}
