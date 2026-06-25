using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020000A9 RID: 169
[RequireComponent(typeof(CanvasGroup))]
public class UIRectDragMove : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
{
	// Token: 0x17000090 RID: 144
	// (get) Token: 0x060005D1 RID: 1489 RVA: 0x000269A8 File Offset: 0x00024BA8
	public bool Dragging
	{
		get
		{
			return this._dragging;
		}
	}

	// Token: 0x060005D2 RID: 1490 RVA: 0x000269B0 File Offset: 0x00024BB0
	private void Start()
	{
		this._selfRectTransform = base.GetComponent<RectTransform>();
		this._parentRectTransform = this._selfRectTransform.parent.GetComponent<RectTransform>();
		this._canvasGroup = base.GetComponent<CanvasGroup>();
		bool flag = null == this._canvasGroup;
		if (flag)
		{
			this._canvasGroup = base.gameObject.AddComponent<CanvasGroup>();
		}
	}

	// Token: 0x060005D3 RID: 1491 RVA: 0x00026A10 File Offset: 0x00024C10
	private void OnApplicationFocus(bool focus)
	{
		bool flag = !focus;
		if (flag)
		{
			this._dragging = false;
		}
		bool flag2 = focus && null != this._canvasGroup;
		if (flag2)
		{
			this._canvasGroup.blocksRaycasts = true;
		}
	}

	// Token: 0x060005D4 RID: 1492 RVA: 0x00026A50 File Offset: 0x00024C50
	private ValueTuple<Vector2, Vector2> GetRectMinMax()
	{
		Vector3 localScale = this._selfRectTransform.localScale;
		Rect selfRect = this._selfRectTransform.rect;
		Vector2 min = selfRect.min;
		Vector2 max = selfRect.max;
		min.x += (float)this.AdjustPadding.left;
		min.y += (float)this.AdjustPadding.bottom;
		max.x -= (float)this.AdjustPadding.right;
		max.y -= (float)this.AdjustPadding.top;
		min.x *= localScale.x;
		min.y *= localScale.y;
		max.x *= localScale.x;
		max.y *= localScale.y;
		Vector2 anchoredPosition = this._selfRectTransform.anchoredPosition;
		min += anchoredPosition;
		max += anchoredPosition;
		return new ValueTuple<Vector2, Vector2>(min, max);
	}

	// Token: 0x060005D5 RID: 1493 RVA: 0x00026B54 File Offset: 0x00024D54
	private void Clamp(float xDir, float yDir, bool setPos = false, bool anim = false)
	{
		Vector2 clampOffset = Vector2.zero;
		Rect parentRect = this._parentRectTransform.rect;
		float edgeX = parentRect.width * 0.5f;
		float edgeY = parentRect.height * 0.5f;
		float lineLeft = (this.ClampType == UIRectDragMove.DragClampType.Center) ? 0f : (-edgeX);
		float lineRight = (this.ClampType == UIRectDragMove.DragClampType.Center) ? 0f : edgeX;
		float lineTop = (this.ClampType == UIRectDragMove.DragClampType.Center) ? 0f : edgeY;
		float lineBottom = (this.ClampType == UIRectDragMove.DragClampType.Center) ? 0f : (-edgeY);
		ValueTuple<Vector2, Vector2> rectMinMax = this.GetRectMinMax();
		Vector2 min = rectMinMax.Item1;
		Vector2 max = rectMinMax.Item2;
		float centerX = (max.x - min.x) * 0.5f + min.x;
		float centerY = (max.y - min.y) * 0.5f + min.y;
		bool needAdjustX = centerX < -edgeX || centerX > edgeX;
		bool needAdjustY = centerY < -edgeY || centerY > edgeY;
		float selfWidth = Mathf.Abs(max.x - min.x);
		float selfHeight = Mathf.Abs(max.y - min.y);
		bool flag = !this._dragged && !this._dragging;
		if (flag)
		{
			lineLeft = -edgeX;
			lineRight = edgeX;
			lineBottom = -edgeY;
			lineTop = edgeY;
			float leftOffset = Mathf.Abs(min.x - lineLeft);
			float rightOffset = Mathf.Abs(max.x - lineRight);
			float topOffset = Mathf.Abs(max.y - lineTop);
			float bottomOffset = Mathf.Abs(min.y - lineBottom);
			bool flag2 = selfWidth > parentRect.width;
			if (flag2)
			{
				bool flag3 = leftOffset < rightOffset;
				if (flag3)
				{
					bool flag4 = min.x > lineLeft;
					if (flag4)
					{
						clampOffset.x = lineLeft - min.x;
					}
				}
				else
				{
					bool flag5 = max.x < lineRight;
					if (flag5)
					{
						clampOffset.x = lineRight - max.x;
					}
				}
			}
			else
			{
				bool flag6 = leftOffset < rightOffset;
				if (flag6)
				{
					bool flag7 = Math.Abs(min.x - lineLeft) > 0.001f;
					if (flag7)
					{
						clampOffset.x = lineLeft - min.x;
					}
				}
				else
				{
					bool flag8 = Math.Abs(max.x - lineRight) > 0.001f;
					if (flag8)
					{
						clampOffset.x = lineRight - max.x;
					}
				}
			}
			bool flag9 = selfHeight > parentRect.height;
			if (flag9)
			{
				bool flag10 = bottomOffset < topOffset;
				if (flag10)
				{
					bool flag11 = min.y > lineBottom;
					if (flag11)
					{
						clampOffset.y = lineBottom - min.y;
					}
				}
				else
				{
					bool flag12 = max.y < lineTop;
					if (flag12)
					{
						clampOffset.y = lineTop - max.y;
					}
				}
			}
			else
			{
				bool flag13 = bottomOffset < topOffset;
				if (flag13)
				{
					bool flag14 = Math.Abs(min.y - lineBottom) > 0.001f;
					if (flag14)
					{
						clampOffset.y = lineBottom - min.y;
					}
				}
				else
				{
					bool flag15 = Math.Abs(max.y - lineTop) > 0.001f;
					if (flag15)
					{
						clampOffset.y = lineTop - max.y;
					}
				}
			}
		}
		else
		{
			bool flag16 = selfWidth > parentRect.width || this.ClampType == UIRectDragMove.DragClampType.Center;
			if (flag16)
			{
				bool flag17 = min.x > lineLeft && xDir > 0f;
				if (flag17)
				{
					clampOffset.x = lineLeft - min.x;
				}
				bool flag18 = max.x < lineRight && xDir < 0f;
				if (flag18)
				{
					clampOffset.x = lineRight - max.x;
				}
			}
			else
			{
				bool flag19 = min.x < lineLeft && xDir < 0f;
				if (flag19)
				{
					clampOffset.x = lineLeft - min.x;
				}
				bool flag20 = max.x > lineRight && xDir > 0f;
				if (flag20)
				{
					clampOffset.x = lineRight - max.x;
				}
			}
			bool flag21 = selfHeight > parentRect.height || this.ClampType == UIRectDragMove.DragClampType.Center;
			if (flag21)
			{
				bool flag22 = min.y > lineBottom && yDir > 0f;
				if (flag22)
				{
					clampOffset.y = lineBottom - min.y;
				}
				bool flag23 = max.y < lineTop && yDir < 0f;
				if (flag23)
				{
					clampOffset.y = lineTop - max.y;
				}
			}
			else
			{
				bool flag24 = min.y < lineBottom && yDir < 0f;
				if (flag24)
				{
					clampOffset.y = lineBottom - min.y;
				}
				bool flag25 = max.y > lineTop && yDir > 0f;
				if (flag25)
				{
					clampOffset.y = lineTop - max.y;
				}
			}
		}
		bool flag26 = this.ClampType == UIRectDragMove.DragClampType.Center;
		if (flag26)
		{
			bool flag27 = !needAdjustX;
			if (flag27)
			{
				clampOffset.x = 0f;
			}
			bool flag28 = !needAdjustY;
			if (flag28)
			{
				clampOffset.y = 0f;
			}
		}
		bool flag29 = clampOffset != Vector2.zero && setPos;
		if (flag29)
		{
			Vector2 targetPos = this._selfRectTransform.anchoredPosition + clampOffset;
			if (anim)
			{
				this._selfRectTransform.DOAnchorPos(targetPos, 0.3f, false).SetUpdate(true).OnComplete(new TweenCallback(this.CheckListenState));
			}
			else
			{
				this._selfRectTransform.anchoredPosition = targetPos;
			}
		}
	}

	// Token: 0x060005D6 RID: 1494 RVA: 0x000270FC File Offset: 0x000252FC
	public void CheckListenState()
	{
		this._canvasGroup.blocksRaycasts = true;
		bool flag = RectTransformUtility.RectangleContainsScreenPoint(this._parentRectTransform, Input.mousePosition, UIManager.Instance.UiCamera);
		if (flag)
		{
			this.OnDrag(null);
		}
		else
		{
			this.OnEndDrag(null);
		}
	}

	// Token: 0x060005D7 RID: 1495 RVA: 0x0002714C File Offset: 0x0002534C
	public void SetDirty()
	{
		this._dirtyFlag = true;
	}

	// Token: 0x060005D8 RID: 1496 RVA: 0x00027158 File Offset: 0x00025358
	public void OnDrag(PointerEventData eventData)
	{
		bool dragging = this._dragging;
		if (dragging)
		{
			this.Drag();
		}
	}

	// Token: 0x060005D9 RID: 1497 RVA: 0x0002717C File Offset: 0x0002537C
	protected virtual void Drag()
	{
		Vector2 mousePos = Input.mousePosition;
		bool flag = mousePos.x < 0f || mousePos.x > (float)Screen.width || mousePos.y < 0f || mousePos.y > (float)Screen.height;
		if (!flag)
		{
			Vector2 newPos = UIManager.Instance.MousePosToLocalPos(this._parentRectTransform);
			this._offset = newPos - this._lastPos;
			bool checkClampWhenDrag = this.CheckClampWhenDrag;
			if (checkClampWhenDrag)
			{
				this._offset = this.ComputeClampedOffset(this._offset);
			}
			this._selfRectTransform.anchoredPosition = this._selfRectTransform.anchoredPosition + this._offset;
			this._dragged = true;
			this.SetDirty();
			this._lastPos = newPos;
		}
	}

	// Token: 0x060005DA RID: 1498 RVA: 0x0002724C File Offset: 0x0002544C
	public void OnBeginDrag(PointerEventData eventData)
	{
		bool flag = eventData.button == PointerEventData.InputButton.Left && !Input.GetKey(KeyCode.Mouse1);
		if (flag)
		{
			this.BeginDrag();
		}
	}

	// Token: 0x060005DB RID: 1499 RVA: 0x0002727F File Offset: 0x0002547F
	protected virtual void BeginDrag()
	{
		this._lastPos = UIManager.Instance.MousePosToLocalPos(this._parentRectTransform);
		this._dragging = true;
		this._canvasGroup.blocksRaycasts = false;
		Action beginDragCallback = this.BeginDragCallback;
		if (beginDragCallback != null)
		{
			beginDragCallback();
		}
	}

	// Token: 0x060005DC RID: 1500 RVA: 0x000272BE File Offset: 0x000254BE
	public void OnEndDrag(PointerEventData eventData)
	{
		this.EndDrag();
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x000272C8 File Offset: 0x000254C8
	protected virtual void EndDrag()
	{
		bool flag = !this._dragging;
		if (!flag)
		{
			bool belastic = this.BElastic;
			if (belastic)
			{
				float xSpeed = Input.GetAxis("Mouse X") * this.SpeedFactor;
				float ySpeed = Input.GetAxis("Mouse Y") * this.SpeedFactor;
				DOVirtual.Float(1f, 0f, this.ElasticDuration, delegate(float val)
				{
					xSpeed *= val;
					ySpeed *= val;
					this._offset.x = xSpeed;
					this._offset.y = ySpeed;
					this._selfRectTransform.anchoredPosition += this._offset;
					this._dragged = true;
					this.SetDirty();
				}).SetEase(Ease.InSine).SetUpdate(true).SetAutoKill(true).SetAutoKill(true);
			}
			this._dragging = false;
			this._canvasGroup.blocksRaycasts = true;
			Action endDragCallback = this.EndDragCallback;
			if (endDragCallback != null)
			{
				endDragCallback();
			}
		}
	}

	// Token: 0x060005DE RID: 1502 RVA: 0x00027390 File Offset: 0x00025590
	private void LateUpdate()
	{
		bool dirtyFlag = this._dirtyFlag;
		if (dirtyFlag)
		{
			bool flag = !this.CheckClampWhenDrag;
			if (flag)
			{
				this.Clamp(this._offset.x, this._offset.y, true, false);
			}
			this._offset = Vector2.zero;
			this._dirtyFlag = false;
			this._dragged = false;
			Action afterClampCallback = this.AfterClampCallback;
			if (afterClampCallback != null)
			{
				afterClampCallback();
			}
		}
	}

	// Token: 0x060005DF RID: 1503 RVA: 0x00027404 File Offset: 0x00025604
	protected virtual void Update()
	{
		bool mouseButtonDown = Input.GetMouseButtonDown(0);
		if (mouseButtonDown)
		{
			this._canvasGroup.blocksRaycasts = true;
		}
		bool key = Input.GetKey(KeyCode.Mouse1);
		if (key)
		{
			this.OnEndDrag(null);
		}
	}

	// Token: 0x060005E0 RID: 1504 RVA: 0x00027443 File Offset: 0x00025643
	public void AdjustOffsetAfterScale()
	{
		UIRectDragMove.AdjustOffsetAfterScale(this._selfRectTransform, this._parentRectTransform);
	}

	// Token: 0x060005E1 RID: 1505 RVA: 0x00027458 File Offset: 0x00025658
	public static void AdjustOffsetAfterScale(RectTransform selfRectTransform, RectTransform parentRectTransform)
	{
		Vector3[] parentCorners = new Vector3[4];
		parentRectTransform.GetWorldCorners(parentCorners);
		Vector3[] childCorners = new Vector3[4];
		selfRectTransform.GetWorldCorners(childCorners);
		float minPx = float.MaxValue;
		float maxPx = float.MinValue;
		float minPy = float.MaxValue;
		float maxPy = float.MinValue;
		foreach (Vector3 c in parentCorners)
		{
			bool flag = c.x < minPx;
			if (flag)
			{
				minPx = c.x;
			}
			bool flag2 = c.x > maxPx;
			if (flag2)
			{
				maxPx = c.x;
			}
			bool flag3 = c.y < minPy;
			if (flag3)
			{
				minPy = c.y;
			}
			bool flag4 = c.y > maxPy;
			if (flag4)
			{
				maxPy = c.y;
			}
		}
		float minCx = float.MaxValue;
		float maxCx = float.MinValue;
		float minCy = float.MaxValue;
		float maxCy = float.MinValue;
		foreach (Vector3 c2 in childCorners)
		{
			bool flag5 = c2.x < minCx;
			if (flag5)
			{
				minCx = c2.x;
			}
			bool flag6 = c2.x > maxCx;
			if (flag6)
			{
				maxCx = c2.x;
			}
			bool flag7 = c2.y < minCy;
			if (flag7)
			{
				minCy = c2.y;
			}
			bool flag8 = c2.y > maxCy;
			if (flag8)
			{
				maxCy = c2.y;
			}
		}
		float lowerX = maxPx - maxCx;
		float upperX = minPx - minCx;
		bool canCoverX = lowerX <= upperX + 1E-06f;
		float deltaX = 0f;
		bool flag9 = canCoverX;
		if (flag9)
		{
			deltaX = Mathf.Clamp(0f, lowerX, upperX);
		}
		float lowerY = maxPy - maxCy;
		float upperY = minPy - minCy;
		bool canCoverY = lowerY <= upperY + 1E-06f;
		float deltaY = 0f;
		bool flag10 = canCoverY;
		if (flag10)
		{
			deltaY = Mathf.Clamp(0f, lowerY, upperY);
		}
		bool flag11 = canCoverX || canCoverY;
		if (flag11)
		{
			selfRectTransform.position += new Vector3(deltaX, deltaY, 0f);
		}
	}

	// Token: 0x060005E2 RID: 1506 RVA: 0x0002767C File Offset: 0x0002587C
	private Vector2 ComputeClampedOffset(Vector2 rawOffset)
	{
		ValueTuple<Vector2, Vector2> rectMinMax = this.GetRectMinMax();
		Vector2 min = rectMinMax.Item1;
		Vector2 max = rectMinMax.Item2;
		Rect parentRect = this._parentRectTransform.rect;
		float edgeX = parentRect.width * 0.5f;
		float edgeY = parentRect.height * 0.5f;
		float left = (this.ClampType == UIRectDragMove.DragClampType.Center) ? 0f : (-edgeX);
		float right = (this.ClampType == UIRectDragMove.DragClampType.Center) ? 0f : edgeX;
		float bottom = (this.ClampType == UIRectDragMove.DragClampType.Center) ? 0f : (-edgeY);
		float top = (this.ClampType == UIRectDragMove.DragClampType.Center) ? 0f : edgeY;
		Vector2 result = rawOffset;
		float rectRealWidth = max.x - min.x;
		float rectRealHeight = max.y - min.y;
		bool flag = parentRect.width >= rectRealWidth;
		if (flag)
		{
			bool flag2 = rawOffset.x > 0f && max.x + rawOffset.x > right;
			if (flag2)
			{
				result.x -= max.x + rawOffset.x - right;
			}
			bool flag3 = rawOffset.x < 0f && min.x + rawOffset.x < left;
			if (flag3)
			{
				result.x -= min.x + rawOffset.x - left;
			}
		}
		else
		{
			bool flag4 = rawOffset.x > 0f && min.x + rawOffset.x > left;
			if (flag4)
			{
				result.x = left - min.x;
			}
			bool flag5 = rawOffset.x < 0f && max.x + rawOffset.x < right;
			if (flag5)
			{
				result.x = right - max.x;
			}
		}
		bool flag6 = parentRect.height >= rectRealHeight;
		if (flag6)
		{
			bool flag7 = rawOffset.y > 0f && max.y + rawOffset.y > top;
			if (flag7)
			{
				result.y -= max.y + rawOffset.y - top;
			}
			bool flag8 = rawOffset.y < 0f && min.y + rawOffset.y < bottom;
			if (flag8)
			{
				result.y -= min.y + rawOffset.y - bottom;
			}
		}
		else
		{
			bool flag9 = rawOffset.y > 0f && min.y + rawOffset.y > bottom;
			if (flag9)
			{
				result.y = bottom - min.y;
			}
			bool flag10 = rawOffset.y < 0f && max.y + rawOffset.y < top;
			if (flag10)
			{
				result.y = top - max.y;
			}
		}
		return result;
	}

	// Token: 0x040004C9 RID: 1225
	public UIRectDragMove.DragClampType ClampType = UIRectDragMove.DragClampType.Center;

	// Token: 0x040004CA RID: 1226
	[Tooltip("是否缓动")]
	public bool BElastic;

	// Token: 0x040004CB RID: 1227
	[Tooltip("缓动速度参数")]
	public float SpeedFactor = 5f;

	// Token: 0x040004CC RID: 1228
	[Tooltip("缓动持续时间")]
	public float ElasticDuration = 0.3f;

	// Token: 0x040004CD RID: 1229
	[Tooltip("边界修正")]
	public RectOffset AdjustPadding;

	// Token: 0x040004CE RID: 1230
	[Tooltip("是否在拖动时就检查移动范围，而不是移动后再Clamp")]
	public bool CheckClampWhenDrag = false;

	// Token: 0x040004CF RID: 1231
	private RectTransform _selfRectTransform;

	// Token: 0x040004D0 RID: 1232
	private RectTransform _parentRectTransform;

	// Token: 0x040004D1 RID: 1233
	private CanvasGroup _canvasGroup;

	// Token: 0x040004D2 RID: 1234
	private Vector2 _lastPos;

	// Token: 0x040004D3 RID: 1235
	private Vector2 _offset;

	// Token: 0x040004D4 RID: 1236
	protected bool _dragging = false;

	// Token: 0x040004D5 RID: 1237
	private bool _dirtyFlag;

	// Token: 0x040004D6 RID: 1238
	private bool _dragged;

	// Token: 0x040004D7 RID: 1239
	public Action BeginDragCallback;

	// Token: 0x040004D8 RID: 1240
	public Action EndDragCallback;

	// Token: 0x040004D9 RID: 1241
	public Action AfterClampCallback;

	// Token: 0x0200110F RID: 4367
	public enum DragClampType
	{
		// Token: 0x04009581 RID: 38273
		Center,
		// Token: 0x04009582 RID: 38274
		Corner
	}
}
