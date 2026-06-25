using System;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020000D1 RID: 209
[Obsolete]
[RequireComponent(typeof(PointerTrigger))]
public class CScrollRectLegacy : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IScrollHandler
{
	// Token: 0x170000A9 RID: 169
	// (get) Token: 0x06000736 RID: 1846 RVA: 0x00032011 File Offset: 0x00030211
	private bool Horizontal
	{
		get
		{
			return this.Direction == CScrollRectLegacy.ScrollDirection.Horizontal;
		}
	}

	// Token: 0x170000AA RID: 170
	// (get) Token: 0x06000737 RID: 1847 RVA: 0x0003201C File Offset: 0x0003021C
	private bool Vertical
	{
		get
		{
			return this.Direction == CScrollRectLegacy.ScrollDirection.Vertical;
		}
	}

	// Token: 0x1400000C RID: 12
	// (add) Token: 0x06000738 RID: 1848 RVA: 0x00032028 File Offset: 0x00030228
	// (remove) Token: 0x06000739 RID: 1849 RVA: 0x00032060 File Offset: 0x00030260
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnScrollEvent;

	// Token: 0x1400000D RID: 13
	// (add) Token: 0x0600073A RID: 1850 RVA: 0x00032098 File Offset: 0x00030298
	// (remove) Token: 0x0600073B RID: 1851 RVA: 0x000320D0 File Offset: 0x000302D0
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnListenerDimensionsChangeEvent;

	// Token: 0x170000AB RID: 171
	// (get) Token: 0x0600073C RID: 1852 RVA: 0x00032105 File Offset: 0x00030305
	// (set) Token: 0x0600073D RID: 1853 RVA: 0x0003210D File Offset: 0x0003030D
	public CScrollRectLegacy.State CurState { get; private set; }

	// Token: 0x170000AC RID: 172
	// (get) Token: 0x0600073E RID: 1854 RVA: 0x00032116 File Offset: 0x00030316
	// (set) Token: 0x0600073F RID: 1855 RVA: 0x0003211E File Offset: 0x0003031E
	public bool CanScroll { get; set; }

	// Token: 0x170000AD RID: 173
	// (get) Token: 0x06000740 RID: 1856 RVA: 0x00032127 File Offset: 0x00030327
	private Camera EventCamera
	{
		get
		{
			return UIManager.Instance.UiCamera;
		}
	}

	// Token: 0x170000AE RID: 174
	// (get) Token: 0x06000741 RID: 1857 RVA: 0x00032134 File Offset: 0x00030334
	private Vector2 ClampMin
	{
		get
		{
			return this.Viewport.rect.min;
		}
	}

	// Token: 0x170000AF RID: 175
	// (get) Token: 0x06000742 RID: 1858 RVA: 0x00032154 File Offset: 0x00030354
	private Vector2 ClampMax
	{
		get
		{
			return this.Viewport.rect.max;
		}
	}

	// Token: 0x170000B0 RID: 176
	// (get) Token: 0x06000743 RID: 1859 RVA: 0x00032174 File Offset: 0x00030374
	private Vector2 ContentLocalPos
	{
		get
		{
			return (this.Content.parent != this.Viewport) ? this.Viewport.InverseTransformPoint(this.Content.position) : this.Content.localPosition;
		}
	}

	// Token: 0x170000B1 RID: 177
	// (get) Token: 0x06000744 RID: 1860 RVA: 0x000321C4 File Offset: 0x000303C4
	private Vector2 ContentMin
	{
		get
		{
			return this.ContentLocalPos + this.Content.rect.min;
		}
	}

	// Token: 0x170000B2 RID: 178
	// (get) Token: 0x06000745 RID: 1861 RVA: 0x000321F0 File Offset: 0x000303F0
	private Vector2 ContentMax
	{
		get
		{
			return this.ContentLocalPos + this.Content.rect.max;
		}
	}

	// Token: 0x170000B3 RID: 179
	// (get) Token: 0x06000746 RID: 1862 RVA: 0x0003221C File Offset: 0x0003041C
	private float ContentSize
	{
		get
		{
			return this.Horizontal ? this.Content.rect.width : this.Content.rect.height;
		}
	}

	// Token: 0x170000B4 RID: 180
	// (get) Token: 0x06000747 RID: 1863 RVA: 0x0003225C File Offset: 0x0003045C
	private float ViewportSize
	{
		get
		{
			return this.Horizontal ? this.Viewport.rect.width : this.Viewport.rect.height;
		}
	}

	// Token: 0x170000B5 RID: 181
	// (get) Token: 0x06000748 RID: 1864 RVA: 0x0003229C File Offset: 0x0003049C
	private PointerTrigger PointerTrigger
	{
		get
		{
			bool flag = null == this._pointerTrigger;
			if (flag)
			{
				this._pointerTrigger = base.GetComponent<PointerTrigger>();
			}
			return this._pointerTrigger;
		}
	}

	// Token: 0x06000749 RID: 1865 RVA: 0x000322D0 File Offset: 0x000304D0
	private void Awake()
	{
		this.rectTransform = base.GetComponent<RectTransform>();
		bool flag = null != this.ScrollBar;
		if (flag)
		{
			this.ScrollBar.value = 0f;
			this.SetScrollBarActive(false);
		}
		this.CanScroll = true;
	}

	// Token: 0x0600074A RID: 1866 RVA: 0x00032320 File Offset: 0x00030520
	private void SetScrollBarActive(bool isActive)
	{
		Scrollbar scrollBar = this.ScrollBar;
		CScrollbarStateHelper helper = (scrollBar != null) ? scrollBar.GetComponent<CScrollbarStateHelper>() : null;
		bool flag = helper != null && helper.Status != 0;
		if (flag)
		{
			helper.Status = (isActive ? 2 : 1);
		}
		else
		{
			Scrollbar scrollBar2 = this.ScrollBar;
			if (scrollBar2 != null)
			{
				scrollBar2.gameObject.SetActive(isActive);
			}
		}
	}

	// Token: 0x0600074B RID: 1867 RVA: 0x0003237C File Offset: 0x0003057C
	private void OnEnable()
	{
		bool flag = !this._initFinished;
		if (flag)
		{
			this.Init();
		}
		this._canControl = RectTransformUtility.RectangleContainsScreenPoint(this.Viewport, Input.mousePosition);
		this.UpdateScrollBarValue();
	}

	// Token: 0x0600074C RID: 1868 RVA: 0x000323C0 File Offset: 0x000305C0
	private void Update()
	{
		bool flag = this.CanScroll && (this.CurState == CScrollRectLegacy.State.None || this.CurState == CScrollRectLegacy.State.DampedMoving);
		if (flag)
		{
			float scrollValue = Input.GetAxis("Mouse ScrollWheel");
			bool flag2 = Mathf.Abs(scrollValue) > 0.05f;
			if (flag2)
			{
				bool flag3 = !this.IsInScrollArea() || !this.IsFocusScroll();
				if (flag3)
				{
					return;
				}
				bool flag4 = this.Vertical && this.Content.rect.height > this.Viewport.rect.height;
				if (flag4)
				{
					bool edgePingFlag = false;
					bool flag5 = scrollValue < 0f && this.ScrollBar.value >= 1f;
					if (flag5)
					{
						edgePingFlag = true;
						bool flag6 = this.ScrollBar.direction == Scrollbar.Direction.TopToBottom;
						if (flag6)
						{
							this.DoEdgePing(RectTransform.Edge.Bottom);
						}
						else
						{
							bool flag7 = this.ScrollBar.direction == Scrollbar.Direction.BottomToTop;
							if (flag7)
							{
								this.DoEdgePing(RectTransform.Edge.Top);
							}
						}
					}
					else
					{
						bool flag8 = scrollValue > 0f && this.ScrollBar.value <= 0f;
						if (flag8)
						{
							edgePingFlag = true;
							bool flag9 = this.ScrollBar.direction == Scrollbar.Direction.TopToBottom;
							if (flag9)
							{
								this.DoEdgePing(RectTransform.Edge.Top);
							}
							else
							{
								bool flag10 = this.ScrollBar.direction == Scrollbar.Direction.BottomToTop;
								if (flag10)
								{
									this.DoEdgePing(RectTransform.Edge.Bottom);
								}
							}
						}
					}
					bool flag11 = !edgePingFlag;
					if (flag11)
					{
						bool flag12 = this.ScrollBar.direction == Scrollbar.Direction.TopToBottom;
						if (flag12)
						{
							this._speed.y = -this.ScrollSpeed * Mathf.Sign(scrollValue);
						}
						else
						{
							bool flag13 = this.ScrollBar.direction == Scrollbar.Direction.BottomToTop;
							if (flag13)
							{
								this._speed.y = this.ScrollSpeed * Mathf.Sign(scrollValue);
							}
						}
						this.CurState = CScrollRectLegacy.State.DampedMoving;
					}
				}
				bool flag14 = this.Horizontal && this.Content.rect.width > this.Viewport.rect.width;
				if (flag14)
				{
					bool edgePingFlag2 = false;
					bool flag15 = scrollValue > 0f && this.ScrollBar.value >= 1f;
					if (flag15)
					{
						edgePingFlag2 = true;
						bool flag16 = this.ScrollBar.direction == Scrollbar.Direction.LeftToRight;
						if (flag16)
						{
							this.DoEdgePing(RectTransform.Edge.Left);
						}
						else
						{
							bool flag17 = this.ScrollBar.direction == Scrollbar.Direction.RightToLeft;
							if (flag17)
							{
								this.DoEdgePing(RectTransform.Edge.Right);
							}
						}
					}
					else
					{
						bool flag18 = scrollValue < 0f && this.ScrollBar.value <= 0f;
						if (flag18)
						{
							edgePingFlag2 = true;
							bool flag19 = this.ScrollBar.direction == Scrollbar.Direction.LeftToRight;
							if (flag19)
							{
								this.DoEdgePing(RectTransform.Edge.Right);
							}
							else
							{
								bool flag20 = this.ScrollBar.direction == Scrollbar.Direction.RightToLeft;
								if (flag20)
								{
									this.DoEdgePing(RectTransform.Edge.Left);
								}
							}
						}
					}
					bool flag21 = !edgePingFlag2;
					if (flag21)
					{
						bool flag22 = this.ScrollBar.direction == Scrollbar.Direction.LeftToRight;
						if (flag22)
						{
							this._speed.x = -this.ScrollSpeed * Mathf.Sign(scrollValue);
						}
						else
						{
							bool flag23 = this.ScrollBar.direction == Scrollbar.Direction.RightToLeft;
							if (flag23)
							{
								this._speed.x = this.ScrollSpeed * Mathf.Sign(scrollValue);
							}
						}
						this.CurState = CScrollRectLegacy.State.DampedMoving;
					}
				}
			}
		}
		bool flag24;
		if (this.ScrollBar != null)
		{
			bool activeSelf = this.ScrollBar.gameObject.activeSelf;
			bool? previousSetScrollBarActive = this._previousSetScrollBarActive;
			if (!(activeSelf == previousSetScrollBarActive.GetValueOrDefault() & previousSetScrollBarActive != null))
			{
				flag24 = this.AutoControlViewportSize;
				goto IL_3A7;
			}
		}
		flag24 = false;
		IL_3A7:
		bool flag25 = flag24;
		if (flag25)
		{
			bool flag26 = this.Viewport != this.rectTransform;
			if (flag26)
			{
				this.Viewport.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.rectTransform.rect.width - this.viewportGapNormal - (this.ScrollBar.gameObject.activeSelf ? this.viewportGapScroll : this.viewportGapNormal));
			}
		}
		Scrollbar scrollBar = this.ScrollBar;
		this._previousSetScrollBarActive = ((scrollBar != null) ? new bool?(scrollBar.gameObject.activeSelf) : null);
	}

	// Token: 0x0600074D RID: 1869 RVA: 0x0003280C File Offset: 0x00030A0C
	private void LateUpdate()
	{
		this.UpdatePosition();
		bool flag = Math.Abs(this._contentNoteSize - this.ContentSize) > 2f;
		if (flag)
		{
			this.AdjustToFitNewContentSize();
		}
	}

	// Token: 0x0600074E RID: 1870 RVA: 0x00032848 File Offset: 0x00030A48
	private void OnDisable()
	{
		this.CurState = CScrollRectLegacy.State.None;
		bool flag = null != this.ScrollBar;
		if (flag)
		{
			this.ScrollBar.value = Mathf.Clamp01(this.ScrollBar.value);
		}
	}

	// Token: 0x0600074F RID: 1871 RVA: 0x0003288C File Offset: 0x00030A8C
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
			this._canControl = RectTransformUtility.RectangleContainsScreenPoint(this.Viewport, Input.mousePosition, UIManager.Instance.UiCamera);
			bool flag3 = !this._canControl || this.CurState == CScrollRectLegacy.State.AutoAdjust || this.CurState == CScrollRectLegacy.State.Disable;
			if (!flag3)
			{
				this._horizontalAdjustCode = 0;
				this._verticalAdjustCode = 0;
				this._speed = Vector2.zero;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(this.Viewport, eventData.position, this.EventCamera, out this._dragStartPosition);
				this._prevDragPosition = this._dragStartPosition;
				this._dragStartTime = Time.time;
				this.CurState = CScrollRectLegacy.State.Dragging;
			}
		}
	}

	// Token: 0x06000750 RID: 1872 RVA: 0x0003296C File Offset: 0x00030B6C
	public void OnDrag(PointerEventData eventData)
	{
		bool flag = !this._initFinished;
		if (flag)
		{
			this.Init();
		}
		bool flag2 = this.CurState == CScrollRectLegacy.State.Dragging;
		if (flag2)
		{
			Vector2 position = eventData.position;
			position.x = Mathf.Clamp(position.x, 0f, (float)Screen.width);
			position.y = Mathf.Clamp(position.y, 0f, (float)Screen.height);
			Vector2 curPos;
			bool flag3 = RectTransformUtility.ScreenPointToLocalPointInRectangle(this.Viewport, position, this.EventCamera, out curPos);
			if (flag3)
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
				Action onScrollEvent = this.OnScrollEvent;
				if (onScrollEvent != null)
				{
					onScrollEvent();
				}
			}
		}
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x00032A7C File Offset: 0x00030C7C
	public void OnEndDrag(PointerEventData eventData)
	{
		bool flag = !this._initFinished;
		if (flag)
		{
			this.Init();
		}
		bool flag2 = this.CurState != CScrollRectLegacy.State.Dragging;
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
				this.CurState = CScrollRectLegacy.State.AutoAdjust;
			}
			else
			{
				Vector2 curPos;
				bool flag6 = RectTransformUtility.ScreenPointToLocalPointInRectangle(this.Viewport, position, this.EventCamera, out curPos);
				if (flag6)
				{
					bool flag7 = (this.Horizontal && this.Content.rect.width < this.Viewport.rect.width) || (this.Vertical && this.Content.rect.height < this.Viewport.rect.height);
					if (flag7)
					{
						this.CurState = CScrollRectLegacy.State.Disable;
						this._speed = Vector2.zero;
						DOVirtual.Float(this.ScrollBar.value, 0f, 0.3f, delegate(float stepValue)
						{
							this.ScrollBar.value = stepValue;
						}).SetAutoKill(true).OnComplete(delegate
						{
							this.CurState = CScrollRectLegacy.State.None;
						});
					}
					else
					{
						Vector2 totalOffset = curPos - this._dragStartPosition;
						float dragDur = Time.time - this._dragStartTime;
						bool flag8 = dragDur > 0f;
						if (flag8)
						{
							this._speed.x = Mathf.Min(this.ScrollSpeed, Mathf.Abs(totalOffset.x / this.ScrollSpeed / dragDur)) * Mathf.Sign(totalOffset.x);
							this._speed.y = Mathf.Min(this.ScrollSpeed, Mathf.Abs(totalOffset.y / this.ScrollSpeed / dragDur)) * Mathf.Sign(totalOffset.y);
							bool flag9 = !this.Horizontal;
							if (flag9)
							{
								this._speed.x = 0f;
							}
							bool flag10 = !this.Vertical;
							if (flag10)
							{
								this._speed.y = 0f;
							}
						}
						this.CurState = CScrollRectLegacy.State.DampedMoving;
					}
				}
			}
		}
	}

	// Token: 0x06000752 RID: 1874 RVA: 0x00032D7E File Offset: 0x00030F7E
	private void OnPointerEnter()
	{
		this._canControl = true;
	}

	// Token: 0x06000753 RID: 1875 RVA: 0x00032D88 File Offset: 0x00030F88
	private void OnPointerExit()
	{
		bool flag = !RectTransformUtility.RectangleContainsScreenPoint(this.Viewport, Input.mousePosition);
		if (flag)
		{
			this._canControl = false;
		}
	}

	// Token: 0x06000754 RID: 1876 RVA: 0x00032DBC File Offset: 0x00030FBC
	private bool IsInScrollArea()
	{
		bool flag = null != this.ScrollBar && RectTransformUtility.RectangleContainsScreenPoint(this.ScrollBar.transform as RectTransform, Input.mousePosition, UIManager.Instance.UiCamera);
		return !flag && RectTransformUtility.RectangleContainsScreenPoint(this.Viewport, Input.mousePosition, UIManager.Instance.UiCamera);
	}

	// Token: 0x06000755 RID: 1877 RVA: 0x00032E30 File Offset: 0x00031030
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
				CScrollRectLegacy component = _raycastResultList[0].gameObject.GetComponentInParent<CScrollRectLegacy>();
				result = (component != null && component.Equals(this));
			}
			else
			{
				result = false;
			}
		}
		return result;
	}

	// Token: 0x06000756 RID: 1878 RVA: 0x00032E9C File Offset: 0x0003109C
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
			float sizeValue = 1f;
			bool flag3 = this.Content.rect.width > 0f;
			if (flag3)
			{
				sizeValue = Mathf.Clamp01(this.Viewport.rect.width / this.Content.rect.width);
			}
			CScrollbarLegacy cScrollBar = this.ScrollBar as CScrollbarLegacy;
			bool flag4 = cScrollBar != null;
			if (flag4)
			{
				cScrollBar.HandleSize = sizeValue;
			}
			else
			{
				this.ScrollBar.size = sizeValue;
			}
		}
		bool vertical = this.Vertical;
		if (vertical)
		{
			bool flag5 = this.ScrollBar.direction == Scrollbar.Direction.TopToBottom;
			if (flag5)
			{
				scrollValue = (this.ContentMax.y - this.ClampMax.y) / (this.Content.rect.height - this.Viewport.rect.height);
			}
			else
			{
				bool flag6 = this.ScrollBar.direction == Scrollbar.Direction.BottomToTop;
				if (flag6)
				{
					scrollValue = (this.ClampMin.y - this.ContentMin.y) / (this.Content.rect.height - this.Viewport.rect.height);
				}
			}
			float sizeValue2 = 1f;
			bool flag7 = this.Content.rect.height > 0f;
			if (flag7)
			{
				sizeValue2 = Mathf.Clamp01(this.Viewport.rect.height / this.Content.rect.height);
			}
			CScrollbarLegacy cScrollBar2 = this.ScrollBar as CScrollbarLegacy;
			bool flag8 = cScrollBar2 != null;
			if (flag8)
			{
				cScrollBar2.HandleSize = sizeValue2;
			}
			else
			{
				this.ScrollBar.size = sizeValue2;
			}
		}
		bool flag9 = float.IsNaN(scrollValue);
		if (flag9)
		{
			scrollValue = float.MinValue;
		}
		return scrollValue;
	}

	// Token: 0x06000757 RID: 1879 RVA: 0x0003315C File Offset: 0x0003135C
	public void UpdateScrollBarValue()
	{
		bool flag = null == this.ScrollBar;
		if (!flag)
		{
			float scrollValue = Mathf.Clamp01(this.UpdateScrollBarSizeAndGetRealBarValue());
			this.ScrollBar.value = scrollValue;
			bool flag2 = !this.NeedHideScrollBar;
			if (flag2)
			{
				this.SetScrollBarActive(this.ScrollBar.size < 1f);
			}
			else
			{
				this.SetScrollBarActive(false);
			}
			bool flag3;
			if (!this.ScrollBar.gameObject.activeSelf)
			{
				CScrollbarStateHelper component = this.ScrollBar.GetComponent<CScrollbarStateHelper>();
				flag3 = (component == null || component.Status == 2);
			}
			else
			{
				flag3 = false;
			}
			bool flag4 = flag3;
			if (flag4)
			{
				this.OnScrollBarValueChanged(scrollValue);
			}
		}
	}

	// Token: 0x06000758 RID: 1880 RVA: 0x00033210 File Offset: 0x00031410
	private void OnScrollBarValueChanged(float value)
	{
		bool flag = null == this.ScrollBar;
		if (!flag)
		{
			bool horizontal = this.Horizontal;
			if (horizontal)
			{
				float sizeValue = 1f;
				bool flag2 = this.Content.rect.width > 0f;
				if (flag2)
				{
					sizeValue = Mathf.Clamp01(this.Viewport.rect.width / this.Content.rect.width);
				}
				CScrollbarLegacy cScrollBar = this.ScrollBar as CScrollbarLegacy;
				bool flag3 = cScrollBar != null;
				if (flag3)
				{
					cScrollBar.HandleSize = sizeValue;
				}
				else
				{
					this.ScrollBar.size = sizeValue;
				}
			}
			bool vertical = this.Vertical;
			if (vertical)
			{
				float sizeValue2 = 1f;
				bool flag4 = this.Content.rect.height > 0f;
				if (flag4)
				{
					sizeValue2 = Mathf.Clamp01(this.Viewport.rect.height / this.Content.rect.height);
				}
				CScrollbarLegacy cScrollBar2 = this.ScrollBar as CScrollbarLegacy;
				bool flag5 = cScrollBar2 != null;
				if (flag5)
				{
					cScrollBar2.HandleSize = sizeValue2;
				}
				else
				{
					this.ScrollBar.size = sizeValue2;
				}
			}
			bool flag6 = !this.NeedHideScrollBar;
			if (flag6)
			{
				this.SetScrollBarActive(this.ScrollBar.size < 1f);
			}
			else
			{
				this.SetScrollBarActive(false);
			}
			bool flag7 = this.CurState == CScrollRectLegacy.State.None || this.CurState == CScrollRectLegacy.State.Disable;
			if (flag7)
			{
				this.SetToPositionByValue(value);
			}
		}
	}

	// Token: 0x06000759 RID: 1881 RVA: 0x000333B8 File Offset: 0x000315B8
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

	// Token: 0x0600075A RID: 1882 RVA: 0x00033550 File Offset: 0x00031750
	private void OnScrollBarValueClickSet(float value)
	{
		bool flag = this.CurState == CScrollRectLegacy.State.DampedMoving || this.CurState == CScrollRectLegacy.State.AutoAdjust;
		if (flag)
		{
			this.CurState = CScrollRectLegacy.State.None;
		}
		this.SetToPositionByValue(value);
		Action onScrollEvent = this.OnScrollEvent;
		if (onScrollEvent != null)
		{
			onScrollEvent();
		}
	}

	// Token: 0x0600075B RID: 1883 RVA: 0x0003359C File Offset: 0x0003179C
	private void OnListenerDimensionsChange()
	{
		bool flag = this.CurState != CScrollRectLegacy.State.Disable;
		if (flag)
		{
			this.CurState = CScrollRectLegacy.State.None;
		}
		this.UpdateScrollBarValue();
		this.UpdateScrollBarSizeAndGetRealBarValue();
		Action onListenerDimensionsChangeEvent = this.OnListenerDimensionsChangeEvent;
		if (onListenerDimensionsChangeEvent != null)
		{
			onListenerDimensionsChangeEvent();
		}
	}

	// Token: 0x0600075C RID: 1884 RVA: 0x000335E4 File Offset: 0x000317E4
	private void AdjustToFitNewContentSize()
	{
		float scrollValue = this.UpdateScrollBarSizeAndGetRealBarValue();
		bool flag = this.ViewportSize > this.ContentSize;
		if (flag)
		{
			scrollValue = 0f;
		}
		bool flag2 = Math.Abs(scrollValue - this.ScrollBar.value) > 0.01f;
		if (flag2)
		{
			this.SetToPositionByValue(Mathf.Clamp01(scrollValue));
			this.UpdateScrollBarValue();
		}
		this._contentNoteSize = this.ContentSize;
	}

	// Token: 0x0600075D RID: 1885 RVA: 0x00033650 File Offset: 0x00031850
	private void Init()
	{
		bool flag = null == this.Viewport;
		if (flag)
		{
			throw new Exception("null Viewport of CScrollRect " + base.name);
		}
		bool flag2 = null == this.Content;
		if (flag2)
		{
			throw new Exception("null Content of CScrollRect " + base.name);
		}
		bool flag3 = null == this.ScrollBar;
		if (flag3)
		{
			throw new Exception("null ScrollBar of CScrollRect " + base.name);
		}
		DimensionsChangeListener dimensionsChangeListener = this.Content.GetComponent<DimensionsChangeListener>();
		bool flag4 = null == dimensionsChangeListener;
		if (flag4)
		{
			dimensionsChangeListener = this.Content.gameObject.AddComponent<DimensionsChangeListener>();
			dimensionsChangeListener.OnDimensionsChange = new UnityEvent();
		}
		dimensionsChangeListener.EventType = EEvents.Invalid;
		dimensionsChangeListener.OnDimensionsChange.AddListener(new UnityAction(this.OnListenerDimensionsChange));
		dimensionsChangeListener = this.Viewport.GetComponent<DimensionsChangeListener>();
		bool flag5 = null == dimensionsChangeListener;
		if (flag5)
		{
			dimensionsChangeListener = this.Viewport.gameObject.AddComponent<DimensionsChangeListener>();
			dimensionsChangeListener.OnDimensionsChange = new UnityEvent();
		}
		dimensionsChangeListener.EventType = EEvents.Invalid;
		dimensionsChangeListener.OnDimensionsChange.AddListener(new UnityAction(this.OnListenerDimensionsChange));
		bool autoBindTriggerElement = this.AutoBindTriggerElement;
		if (autoBindTriggerElement)
		{
			UIBase uIBase = base.GetComponentInParent<UIBase>();
			bool flag6 = null != uIBase;
			if (flag6)
			{
				this.PointerTrigger.SetBindElement(uIBase.Element);
			}
		}
		this.PointerTrigger.EnterEvent.AddListener(new UnityAction(this.OnPointerEnter));
		this.PointerTrigger.ExitEvent.AddListener(new UnityAction(this.OnPointerExit));
		this.ScrollBar.onValueChanged.AddListener(new UnityAction<float>(this.OnScrollBarValueChanged));
		float size = Mathf.Clamp01(this.Viewport.rect.width / this.Content.rect.width);
		bool vertical = this.Vertical;
		if (vertical)
		{
			size = Mathf.Clamp01(this.Viewport.rect.height / this.Content.rect.height);
		}
		CScrollbarLegacy cScrollbar = this.ScrollBar as CScrollbarLegacy;
		bool flag7 = cScrollbar != null;
		if (flag7)
		{
			cScrollbar.HandleSize = size;
			cScrollbar.OnClickSetValueEvent += this.OnScrollBarValueClickSet;
		}
		else
		{
			this.ScrollBar.size = size;
		}
		bool flag8 = !this.NeedHideScrollBar;
		if (flag8)
		{
			this.SetScrollBarActive(this.ScrollBar.size < 1f);
		}
		else
		{
			this.SetScrollBarActive(false);
		}
		this._initFinished = true;
	}

	// Token: 0x0600075E RID: 1886 RVA: 0x000338F4 File Offset: 0x00031AF4
	private void UpdatePosition()
	{
		bool flag = this.CurState != CScrollRectLegacy.State.AutoAdjust && this.CurState != CScrollRectLegacy.State.DampedMoving;
		if (!flag)
		{
			Vector2 stepValue = this._speed;
			this.Content.anchoredPosition += stepValue;
			Vector2 fixVector = Vector2.zero;
			bool flag2 = this.CurState == CScrollRectLegacy.State.AutoAdjust;
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
				bool flag7 = this.CurState == CScrollRectLegacy.State.DampedMoving;
				if (flag7)
				{
					bool horizontal2 = this.Horizontal;
					if (horizontal2)
					{
						bool flag8 = this._speed.x > 0f;
						if (flag8)
						{
							this._speed.x = Mathf.Max(0f, this._speed.x - this.ScrollSpeed * this.DampedCoefficient);
							bool flag9 = this.ContentMin.x > this.ClampMin.x;
							if (flag9)
							{
								fixVector.x = this.ClampMin.x - this.ContentMin.x;
								this._speed.x = 0f;
							}
						}
						else
						{
							this._speed.x = Mathf.Min(0f, this._speed.x + this.ScrollSpeed * this.DampedCoefficient);
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
							this._speed.y = Mathf.Max(0f, this._speed.y - this.ScrollSpeed * this.DampedCoefficient);
							bool flag12 = this.ContentMin.y > this.ClampMin.y;
							if (flag12)
							{
								fixVector.y = this.ClampMin.y - this.ContentMin.y;
								this._speed.y = 0f;
							}
						}
						else
						{
							this._speed.y = Mathf.Min(0f, this._speed.y + this.ScrollSpeed * this.DampedCoefficient);
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
			bool flag14 = Mathf.Abs(this._speed.x) <= 0f && Mathf.Abs(this._speed.y) <= 0f;
			if (flag14)
			{
				this.CurState = CScrollRectLegacy.State.None;
				this._horizontalAdjustCode = 0;
				this._verticalAdjustCode = 0;
			}
		}
	}

	// Token: 0x0600075F RID: 1887 RVA: 0x00033E68 File Offset: 0x00032068
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

	// Token: 0x06000760 RID: 1888 RVA: 0x0003412C File Offset: 0x0003232C
	private void DoEdgePing(RectTransform.Edge edge)
	{
		CScrollRectLegacy.<>c__DisplayClass89_0 CS$<>8__locals1 = new CScrollRectLegacy.<>c__DisplayClass89_0();
		CS$<>8__locals1.<>4__this = this;
		bool flag = this.CurState == CScrollRectLegacy.State.EdgePing;
		if (!flag)
		{
			this.CurState = CScrollRectLegacy.State.EdgePing;
			float pingSize = this.ViewportSize * 0.1f;
			CS$<>8__locals1.startPos = 0f;
			CS$<>8__locals1.endPos = 0f;
			bool flag2 = edge == RectTransform.Edge.Top;
			if (flag2)
			{
				CS$<>8__locals1.startPos = this.Content.anchoredPosition.y;
				CS$<>8__locals1.endPos = CS$<>8__locals1.startPos - pingSize;
			}
			else
			{
				bool flag3 = edge == RectTransform.Edge.Bottom;
				if (flag3)
				{
					CS$<>8__locals1.startPos = this.Content.anchoredPosition.y;
					CS$<>8__locals1.endPos = CS$<>8__locals1.startPos + pingSize;
				}
				else
				{
					bool flag4 = edge == RectTransform.Edge.Left;
					if (flag4)
					{
						CS$<>8__locals1.startPos = this.Content.anchoredPosition.x;
						CS$<>8__locals1.endPos = CS$<>8__locals1.startPos - pingSize;
					}
					else
					{
						bool flag5 = edge == RectTransform.Edge.Right;
						if (flag5)
						{
							CS$<>8__locals1.startPos = this.Content.anchoredPosition.x;
							CS$<>8__locals1.endPos = CS$<>8__locals1.startPos + pingSize;
						}
					}
				}
			}
			Sequence sequence = DOTween.Sequence();
			sequence.Append(DOVirtual.Float(0f, 1f, 0.15f, new TweenCallback<float>(CS$<>8__locals1.<DoEdgePing>g__SetStepPos|0)).SetAutoKill(true));
			sequence.Append(DOVirtual.Float(1f, 0f, 0.15f, new TweenCallback<float>(CS$<>8__locals1.<DoEdgePing>g__SetStepPos|0)).SetAutoKill(true));
			sequence.AppendCallback(delegate
			{
				CS$<>8__locals1.<>4__this.CurState = CScrollRectLegacy.State.None;
			});
			sequence.Play<Sequence>().SetUpdate(true).SetAutoKill(true);
		}
	}

	// Token: 0x06000761 RID: 1889 RVA: 0x000342D4 File Offset: 0x000324D4
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

	// Token: 0x06000762 RID: 1890 RVA: 0x000343FC File Offset: 0x000325FC
	public void ScrollTo(Vector2 targetAnchorPosition, float duration = 0.3f)
	{
		Vector2 startPos = this.Content.anchoredPosition;
		DOVirtual.Float(0f, 1f, duration, delegate(float stepValue)
		{
			this.CurState = CScrollRectLegacy.State.AutoAdjust;
			bool flag = null != this.Content;
			if (flag)
			{
				this.Content.anchoredPosition = Vector2.Lerp(startPos, targetAnchorPosition, stepValue);
			}
		}).SetAutoKill(true).OnComplete(delegate
		{
			this.CurState = CScrollRectLegacy.State.None;
			this.UpdateScrollBarValue();
			Action onScrollEnd = this.OnScrollEnd;
			if (onScrollEnd != null)
			{
				onScrollEnd();
			}
		}).SetUpdate(true);
	}

	// Token: 0x06000763 RID: 1891 RVA: 0x00034469 File Offset: 0x00032669
	public void SetScrollEnable(bool canScroll)
	{
		this.ScrollBar.enabled = canScroll;
		this.CanScroll = canScroll;
	}

	// Token: 0x06000764 RID: 1892 RVA: 0x00034484 File Offset: 0x00032684
	public bool ScrollTo(RectTransform refersRectTrans, float duration = 0.3f, Vector2 offset = default(Vector2))
	{
		bool flag = this.Direction == CScrollRectLegacy.ScrollDirection.Vertical;
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
				Vector2 scrollToPos = this.Content.anchoredPosition - Vector2.up * yOffset + offset;
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

	// Token: 0x06000765 RID: 1893 RVA: 0x000345F4 File Offset: 0x000327F4
	public void OnPointerClick(PointerEventData eventData)
	{
		bool flag = this.CurState == CScrollRectLegacy.State.Dragging;
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

	// Token: 0x06000766 RID: 1894 RVA: 0x00034635 File Offset: 0x00032835
	public void SetClick(Action onClick)
	{
		this._onClick = onClick;
	}

	// Token: 0x06000767 RID: 1895 RVA: 0x00034640 File Offset: 0x00032840
	void IScrollHandler.OnScroll(PointerEventData eventData)
	{
		bool flag = this.blockScrollEvent;
		if (flag)
		{
			eventData.Use();
		}
	}

	// Token: 0x0400078A RID: 1930
	[Tooltip("滚动裁切视窗区域")]
	public RectTransform Viewport;

	// Token: 0x0400078B RID: 1931
	[Tooltip("滚动内容RectTransform")]
	public RectTransform Content;

	// Token: 0x0400078C RID: 1932
	public float viewportGapNormal = 6f;

	// Token: 0x0400078D RID: 1933
	public float viewportGapScroll = 34f;

	// Token: 0x0400078E RID: 1934
	[Tooltip("自动控制视窗大小")]
	public bool AutoControlViewportSize = false;

	// Token: 0x0400078F RID: 1935
	public CScrollRectLegacy.ScrollDirection Direction;

	// Token: 0x04000790 RID: 1936
	public Scrollbar ScrollBar;

	// Token: 0x04000791 RID: 1937
	[HideInInspector]
	public bool AutoHideScrollBar;

	// Token: 0x04000792 RID: 1938
	[HideInInspector]
	public float ScrollSpeed;

	// Token: 0x04000793 RID: 1939
	[HideInInspector]
	public float DampedCoefficient;

	// Token: 0x04000794 RID: 1940
	[Tooltip("是否自动绑定所属UI的元素")]
	public bool AutoBindTriggerElement;

	// Token: 0x04000797 RID: 1943
	private bool _initFinished;

	// Token: 0x04000798 RID: 1944
	private RectTransform rectTransform;

	// Token: 0x04000799 RID: 1945
	[Header("是否阻断滚动事件传递，默认不阻断")]
	[SerializeField]
	private bool blockScrollEvent;

	// Token: 0x0400079C RID: 1948
	[HideInInspector]
	public bool NeedHideScrollBar;

	// Token: 0x0400079D RID: 1949
	private bool _canControl;

	// Token: 0x0400079E RID: 1950
	private Vector2 _prevDragPosition;

	// Token: 0x0400079F RID: 1951
	private Vector2 _dragStartPosition;

	// Token: 0x040007A0 RID: 1952
	private float _dragStartTime;

	// Token: 0x040007A1 RID: 1953
	private PointerTrigger _pointerTrigger;

	// Token: 0x040007A2 RID: 1954
	[Tooltip("当超出了范围以后自己调整靠近边界值的移动速度")]
	public float AdjustSpeed = 100f;

	// Token: 0x040007A3 RID: 1955
	private sbyte _horizontalAdjustCode;

	// Token: 0x040007A4 RID: 1956
	private sbyte _verticalAdjustCode;

	// Token: 0x040007A5 RID: 1957
	private float _contentNoteSize;

	// Token: 0x040007A6 RID: 1958
	private Vector2 _speed;

	// Token: 0x040007A7 RID: 1959
	public Action OnScrollEnd;

	// Token: 0x040007A8 RID: 1960
	private Action _onClick;

	// Token: 0x040007A9 RID: 1961
	private bool? _previousSetScrollBarActive;

	// Token: 0x02001130 RID: 4400
	public enum ScrollDirection
	{
		// Token: 0x040095F9 RID: 38393
		Horizontal,
		// Token: 0x040095FA RID: 38394
		Vertical
	}

	// Token: 0x02001131 RID: 4401
	public enum State
	{
		// Token: 0x040095FC RID: 38396
		None,
		// Token: 0x040095FD RID: 38397
		DampedMoving,
		// Token: 0x040095FE RID: 38398
		Dragging,
		// Token: 0x040095FF RID: 38399
		AutoAdjust,
		// Token: 0x04009600 RID: 38400
		EdgePing,
		// Token: 0x04009601 RID: 38401
		Disable
	}
}
