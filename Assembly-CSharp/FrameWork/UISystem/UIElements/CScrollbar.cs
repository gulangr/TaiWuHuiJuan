using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FrameWork.UISystem.UIElements
{
	// Token: 0x02001006 RID: 4102
	[DisallowMultipleComponent]
	public class CScrollbar : Scrollbar, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x17001518 RID: 5400
		// (get) Token: 0x0600BB2A RID: 47914 RVA: 0x00552BEB File Offset: 0x00550DEB
		// (set) Token: 0x0600BB2B RID: 47915 RVA: 0x00552BF3 File Offset: 0x00550DF3
		public float HandleSize
		{
			get
			{
				return base.size;
			}
			set
			{
				base.size = Mathf.Clamp(value, this.minHandleSize, 1f);
			}
		}

		// Token: 0x1400008F RID: 143
		// (add) Token: 0x0600BB2C RID: 47916 RVA: 0x00552C10 File Offset: 0x00550E10
		// (remove) Token: 0x0600BB2D RID: 47917 RVA: 0x00552C48 File Offset: 0x00550E48
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<float> OnClickSetValueEvent;

		// Token: 0x0600BB2E RID: 47918 RVA: 0x00552C7D File Offset: 0x00550E7D
		protected override void Awake()
		{
			base.Awake();
			this.CacheHandleGraphic();
			this.AttachHandleHoverListener();
			this.UpdateHandleVisualState(true);
		}

		// Token: 0x0600BB2F RID: 47919 RVA: 0x00552C9D File Offset: 0x00550E9D
		protected override void OnEnable()
		{
			base.OnEnable();
			this.UpdateHandleVisualState(true);
		}

		// Token: 0x0600BB30 RID: 47920 RVA: 0x00552CAF File Offset: 0x00550EAF
		protected override void OnDisable()
		{
			base.OnDisable();
			this._pointerOverBar = false;
			this._pointerOverHandle = false;
			this.UpdateHandleVisualState(true);
		}

		// Token: 0x0600BB31 RID: 47921 RVA: 0x00552CCF File Offset: 0x00550ECF
		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			this._pointerOverBar = true;
			this.UpdateHandleVisualState(false);
		}

		// Token: 0x0600BB32 RID: 47922 RVA: 0x00552CEC File Offset: 0x00550EEC
		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			bool isDraggingHandle = this._isDraggingHandle;
			if (!isDraggingHandle)
			{
				this._pointerOverBar = false;
				this._pointerOverHandle = false;
				this.UpdateHandleVisualState(false);
			}
		}

		// Token: 0x0600BB33 RID: 47923 RVA: 0x00552D24 File Offset: 0x00550F24
		public override void OnBeginDrag(PointerEventData eventData)
		{
			base.OnBeginDrag(eventData);
			bool flag = !this.IsActive() || !this.IsInteractable() || eventData.button > PointerEventData.InputButton.Left;
			if (!flag)
			{
				this._isDraggingHandle = true;
				this._pointerOverBar = true;
				this.UpdateHandleVisualState(false);
			}
		}

		// Token: 0x0600BB34 RID: 47924 RVA: 0x00552D74 File Offset: 0x00550F74
		public override void OnPointerUp(PointerEventData eventData)
		{
			base.OnPointerUp(eventData);
			this._isDraggingHandle = false;
			bool flag = !RectTransformUtility.RectangleContainsScreenPoint(base.transform as RectTransform, eventData.position, eventData.pressEventCamera);
			if (flag)
			{
				this._pointerOverBar = false;
				this._pointerOverHandle = false;
			}
			this.UpdateHandleVisualState(false);
		}

		// Token: 0x0600BB35 RID: 47925 RVA: 0x00552DCC File Offset: 0x00550FCC
		private new void Update()
		{
			bool flag = this._scrollActivityTimer <= 0f;
			if (!flag)
			{
				this._scrollActivityTimer = Mathf.Max(0f, this._scrollActivityTimer - Time.unscaledDeltaTime);
				bool flag2 = this._scrollActivityTimer <= 0f;
				if (flag2)
				{
					this._scrollActivityTimer = 0f;
					this.UpdateHandleVisualState(false);
				}
			}
		}

		// Token: 0x0600BB36 RID: 47926 RVA: 0x00552E38 File Offset: 0x00551038
		internal void NotifyHandleHover(bool isHovering)
		{
			this._pointerOverHandle = isHovering;
			if (isHovering)
			{
				this._pointerOverBar = true;
			}
			this.UpdateHandleVisualState(false);
		}

		// Token: 0x0600BB37 RID: 47927 RVA: 0x00552E64 File Offset: 0x00551064
		internal void NotifyScrollActivity()
		{
			bool wasInactive = this._scrollActivityTimer <= 0f;
			this._scrollActivityTimer = this.scrollActivityVisibleDuration;
			bool flag = wasInactive;
			if (flag)
			{
				this.UpdateHandleVisualState(false);
			}
		}

		// Token: 0x0600BB38 RID: 47928 RVA: 0x00552E9C File Offset: 0x0055109C
		internal void NotifyViewportHover(bool isHovering)
		{
			this._viewportHovered = isHovering;
			this.UpdateHandleVisualState(false);
		}

		// Token: 0x0600BB39 RID: 47929 RVA: 0x00552EB0 File Offset: 0x005510B0
		public override void OnPointerDown(PointerEventData eventData)
		{
			bool flag = eventData.button > PointerEventData.InputButton.Left;
			if (flag)
			{
				base.OnPointerDown(eventData);
			}
			else
			{
				RectTransform parentRect;
				bool flag2;
				if (!(base.handleRect == null))
				{
					parentRect = (base.handleRect.parent as RectTransform);
					flag2 = (parentRect == null);
				}
				else
				{
					flag2 = true;
				}
				bool flag3 = flag2;
				if (flag3)
				{
					base.OnPointerDown(eventData);
				}
				else
				{
					bool flag4 = !RectTransformUtility.RectangleContainsScreenPoint(base.handleRect, eventData.position, eventData.pressEventCamera) && RectTransformUtility.RectangleContainsScreenPoint(parentRect, eventData.position, eventData.pressEventCamera);
					if (flag4)
					{
						Vector2 localPos;
						bool flag5 = RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, eventData.pressEventCamera, out localPos);
						if (flag5)
						{
							Scrollbar.Direction direction = base.direction;
							if (!true)
							{
							}
							float num;
							switch (direction)
							{
							case Scrollbar.Direction.LeftToRight:
								num = Mathf.InverseLerp(parentRect.rect.xMin, parentRect.rect.xMax, localPos.x);
								break;
							case Scrollbar.Direction.RightToLeft:
								num = Mathf.InverseLerp(parentRect.rect.xMax, parentRect.rect.xMin, localPos.x);
								break;
							case Scrollbar.Direction.BottomToTop:
								num = Mathf.InverseLerp(parentRect.rect.yMin, parentRect.rect.yMax, localPos.y);
								break;
							case Scrollbar.Direction.TopToBottom:
								num = Mathf.InverseLerp(parentRect.rect.yMax, parentRect.rect.yMin, localPos.y);
								break;
							default:
								num = base.value;
								break;
							}
							if (!true)
							{
							}
							float clickValue = num;
							this.SetValueWithoutNotify(Mathf.Clamp01(clickValue));
							Action<float> onClickSetValueEvent = this.OnClickSetValueEvent;
							if (onClickSetValueEvent != null)
							{
								onClickSetValueEvent(base.value);
							}
						}
					}
					base.OnPointerDown(eventData);
				}
			}
		}

		// Token: 0x0600BB3A RID: 47930 RVA: 0x00553090 File Offset: 0x00551290
		private void CacheHandleGraphic()
		{
			bool flag = this.handleGraphic == null && base.handleRect != null;
			if (flag)
			{
				this.handleGraphic = base.handleRect.GetComponent<Graphic>();
			}
		}

		// Token: 0x0600BB3B RID: 47931 RVA: 0x005530D0 File Offset: 0x005512D0
		private void AttachHandleHoverListener()
		{
			bool flag = base.handleRect == null;
			if (!flag)
			{
				PointerTrigger pointerTrigger = base.handleRect.GetComponent<PointerTrigger>();
				pointerTrigger.EnterEvent.AddListener(delegate()
				{
					this.NotifyHandleHover(true);
				});
				pointerTrigger.ExitEvent.AddListener(delegate()
				{
					this.NotifyHandleHover(false);
				});
			}
		}

		// Token: 0x0600BB3C RID: 47932 RVA: 0x0055312C File Offset: 0x0055132C
		private void UpdateHandleVisualState(bool instant = false)
		{
			bool flag = this.handleGraphic == null;
			if (!flag)
			{
				bool flag2 = this._isDraggingHandle || this.DisableHover;
				float targetAlpha;
				if (flag2)
				{
					targetAlpha = 1f;
				}
				else
				{
					bool flag3 = this._pointerOverBar || this._viewportHovered || this._scrollActivityTimer > 0f;
					if (flag3)
					{
						targetAlpha = (this._pointerOverHandle ? 1f : this.hoverAlpha);
					}
					else
					{
						targetAlpha = 0f;
					}
				}
				this.SetHandleAlpha(targetAlpha, instant);
			}
		}

		// Token: 0x0600BB3D RID: 47933 RVA: 0x005531BC File Offset: 0x005513BC
		private void SetHandleAlpha(float alpha, bool instant)
		{
			this.handleGraphic.raycastTarget = (alpha > 0f);
			if (instant)
			{
				this.handleGraphic.canvasRenderer.SetAlpha(alpha);
			}
			else
			{
				this.handleGraphic.CrossFadeAlpha(alpha, this.fadeDuration, true);
			}
			bool flag = this.handleBackGraphic == null;
			if (!flag)
			{
				float handleBackAlpha = (alpha != 0f) ? 1f : 0f;
				if (instant)
				{
					this.handleBackGraphic.canvasRenderer.SetAlpha(handleBackAlpha);
				}
				else
				{
					this.handleBackGraphic.CrossFadeAlpha(handleBackAlpha, this.fadeDuration, true);
				}
			}
		}

		// Token: 0x04009071 RID: 36977
		[Range(0.01f, 1f)]
		[SerializeField]
		private float minHandleSize = 0.05f;

		// Token: 0x04009072 RID: 36978
		[SerializeField]
		private Graphic handleGraphic;

		// Token: 0x04009073 RID: 36979
		[Range(0f, 1f)]
		[SerializeField]
		private float hoverAlpha = 0.65f;

		// Token: 0x04009074 RID: 36980
		[Range(0f, 0.5f)]
		[SerializeField]
		private float fadeDuration = 0.1f;

		// Token: 0x04009075 RID: 36981
		[Tooltip("内容滚动时手柄保持可见的时间（秒）")]
		[Range(0.05f, 1f)]
		[SerializeField]
		private float scrollActivityVisibleDuration = 0.35f;

		// Token: 0x04009076 RID: 36982
		[SerializeField]
		private Graphic handleBackGraphic;

		// Token: 0x04009077 RID: 36983
		private bool _pointerOverBar;

		// Token: 0x04009078 RID: 36984
		private bool _pointerOverHandle;

		// Token: 0x04009079 RID: 36985
		private bool _isDraggingHandle;

		// Token: 0x0400907A RID: 36986
		private float _scrollActivityTimer;

		// Token: 0x0400907B RID: 36987
		private bool _viewportHovered;

		// Token: 0x0400907C RID: 36988
		[NonSerialized]
		public bool DisableHover;
	}
}
