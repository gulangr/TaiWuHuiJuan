using System;
using UnityEngine;

namespace Game.Components.Common
{
	// Token: 0x02000F85 RID: 3973
	public class MouseWheelScaleCustom : MonoBehaviour
	{
		// Token: 0x1700149A RID: 5274
		// (get) Token: 0x0600B69C RID: 46748 RVA: 0x00533FCB File Offset: 0x005321CB
		public float CurProgress
		{
			get
			{
				return this._curProgress;
			}
		}

		// Token: 0x0600B69D RID: 46749 RVA: 0x00533FD4 File Offset: 0x005321D4
		protected void Update()
		{
			this._listenScale = (UIManager.Instance.CheckPopupElementIsInTop(this.listeningUI) && RectTransformUtility.RectangleContainsScreenPoint(this.customDetector, Input.mousePosition, UIManager.Instance.UiCamera));
			bool listenScale = this._listenScale;
			if (listenScale)
			{
				float scrollValue = Input.GetAxis("Mouse ScrollWheel");
				this._scaled = (Math.Abs(scrollValue) > 0f);
				bool scaled = this._scaled;
				if (scaled)
				{
					this.ScaleProcess(scrollValue);
				}
			}
		}

		// Token: 0x0600B69E RID: 46750 RVA: 0x0053405C File Offset: 0x0053225C
		private void Awake()
		{
			bool flag = this._rectTrans == null;
			if (flag)
			{
				this._rectTrans = base.GetComponent<RectTransform>();
			}
			this._rectParent = base.transform.parent.GetComponent<RectTransform>();
		}

		// Token: 0x0600B69F RID: 46751 RVA: 0x005340A0 File Offset: 0x005322A0
		private void Start()
		{
			this._rectDragMove = base.GetComponent<UIRectDragMove>();
			bool flag = null != this._rectDragMove;
			if (flag)
			{
				this._padding = this._rectDragMove.AdjustPadding;
			}
			else
			{
				this._padding = new RectOffset
				{
					left = 0,
					right = 0,
					top = 0,
					bottom = 0
				};
			}
			this.Reset();
		}

		// Token: 0x0600B6A0 RID: 46752 RVA: 0x00534114 File Offset: 0x00532314
		private void OnEnable()
		{
			bool flag = this.NeedCheck();
			if (flag)
			{
				this.CheckListenState();
			}
		}

		// Token: 0x0600B6A1 RID: 46753 RVA: 0x00534134 File Offset: 0x00532334
		private bool NeedCheck()
		{
			Transform parentTrans = base.transform;
			UIBase uiBase;
			for (;;)
			{
				uiBase = parentTrans.GetComponent<UIBase>();
				bool flag = uiBase != null;
				if (flag)
				{
					break;
				}
				parentTrans = parentTrans.parent;
			}
			return UIManager.Instance.CheckPopupElementIsInTop(uiBase.Element);
		}

		// Token: 0x0600B6A2 RID: 46754 RVA: 0x00534180 File Offset: 0x00532380
		public void Reset()
		{
			Vector3 nowScale = base.transform.localScale;
			float nowX = Mathf.Abs(nowScale.x) - Mathf.Min(Mathf.Abs(this.Min.x), Mathf.Abs(this.Max.x));
			float nowY = Mathf.Abs(nowScale.y) - Mathf.Min(Mathf.Abs(this.Min.y), Mathf.Abs(this.Max.y));
			this._curProgress = (nowX / Mathf.Abs(this.Max.x - this.Min.x) + nowY / Mathf.Abs(this.Max.y - this.Min.y)) / 2f;
		}

		// Token: 0x0600B6A3 RID: 46755 RVA: 0x00534248 File Offset: 0x00532448
		private void UpdatePivot(bool scaleDown)
		{
			bool dynamicPivot = this.DynamicPivot;
			if (dynamicPivot)
			{
				Vector2 localPosSelf = UIManager.Instance.MousePosToLocalPos(this._rectTrans);
				Vector2 curPivot = this._rectTrans.pivot;
				Rect rect = this._rectTrans.rect;
				localPosSelf.x += curPivot.x * rect.width;
				localPosSelf.y += curPivot.y * rect.height;
				curPivot.x = Mathf.Clamp01(localPosSelf.x / rect.width);
				curPivot.y = Mathf.Clamp01(localPosSelf.y / rect.height);
				if (scaleDown)
				{
					ValueTuple<Vector2, Vector2> rectMinMax = this.GetRectMinMax(this._rectTrans.localScale);
					Vector2 min = rectMinMax.Item1;
					Vector2 max = rectMinMax.Item2;
					bool flag = min.x > 0f;
					if (flag)
					{
						curPivot.x = 0f;
					}
					bool flag2 = max.x < 0f;
					if (flag2)
					{
						curPivot.x = 1f;
					}
					bool flag3 = min.y > 0f;
					if (flag3)
					{
						curPivot.y = 0f;
					}
					bool flag4 = max.y < 0f;
					if (flag4)
					{
						curPivot.y = 1f;
					}
				}
				this._rectTrans.SetPivot(curPivot);
			}
		}

		// Token: 0x0600B6A4 RID: 46756 RVA: 0x005343B4 File Offset: 0x005325B4
		private ValueTuple<Vector2, Vector2> GetRectMinMax(Vector3 scale)
		{
			Rect selfRect = this._rectTrans.rect;
			Vector2 min = selfRect.min;
			Vector2 max = selfRect.max;
			min.x += (float)this._padding.left;
			min.y += (float)this._padding.bottom;
			max.x -= (float)this._padding.right;
			max.y -= (float)this._padding.top;
			min.x *= scale.x;
			min.y *= scale.y;
			max.x *= scale.x;
			max.y *= scale.y;
			Vector2 anchoredPosition = this._rectTrans.anchoredPosition;
			min += anchoredPosition;
			max += anchoredPosition;
			return new ValueTuple<Vector2, Vector2>(min, max);
		}

		// Token: 0x0600B6A5 RID: 46757 RVA: 0x005344A8 File Offset: 0x005326A8
		public bool CheckListenState()
		{
			bool result = RectTransformUtility.RectangleContainsScreenPoint(this._rectParent, Input.mousePosition, UIManager.Instance.UiCamera);
			bool flag = result;
			if (flag)
			{
				this.OnPointerEnter();
			}
			else
			{
				this.OnPointerExit();
			}
			return result;
		}

		// Token: 0x0600B6A6 RID: 46758 RVA: 0x005344F0 File Offset: 0x005326F0
		public virtual void OnPointerEnter()
		{
			bool flag = this.Min != this.Max && Math.Abs(this.ScaleSpeed) > 0f;
			if (flag)
			{
				this._listenScale = true;
			}
		}

		// Token: 0x0600B6A7 RID: 46759 RVA: 0x00534531 File Offset: 0x00532731
		public virtual void OnPointerExit()
		{
			this._listenScale = false;
			Action onScaleFinish = this.OnScaleFinish;
			if (onScaleFinish != null)
			{
				onScaleFinish();
			}
		}

		// Token: 0x0600B6A8 RID: 46760 RVA: 0x00534550 File Offset: 0x00532750
		public void ScaleProcess(float scrollValue)
		{
			this.UpdatePivot(scrollValue < 0f);
			this._curProgress += scrollValue * this.ScaleSpeed * Time.deltaTime;
			this._curProgress = Mathf.Clamp01(this._curProgress);
			this._rectTrans.localScale = Vector3.Lerp(this.Min, this.Max, this._curProgress);
			Action<Vector3> onScale = this.OnScale;
			if (onScale != null)
			{
				onScale(this._rectTrans.localScale);
			}
			bool flag = null != this._rectDragMove;
			if (flag)
			{
				this._rectDragMove.SetDirty();
			}
		}

		// Token: 0x0600B6A9 RID: 46761 RVA: 0x005345F8 File Offset: 0x005327F8
		public void SetScaleProcess(float scrollValue)
		{
			this._curProgress = scrollValue;
			this._curProgress = Mathf.Clamp01(this._curProgress);
			this._rectTrans.localScale = Vector3.Lerp(this.Min, this.Max, this._curProgress);
			Action<Vector3> onScale = this.OnScale;
			if (onScale != null)
			{
				onScale(this._rectTrans.localScale);
			}
			bool flag = null != this._rectDragMove;
			if (flag)
			{
				this._rectDragMove.SetDirty();
			}
		}

		// Token: 0x04008DD9 RID: 36313
		[Tooltip("自定义检测范围")]
		public RectTransform customDetector;

		// Token: 0x04008DDA RID: 36314
		[Tooltip("缩放最小值")]
		public Vector3 Min;

		// Token: 0x04008DDB RID: 36315
		[Tooltip("缩放最大值")]
		public Vector3 Max;

		// Token: 0x04008DDC RID: 36316
		[Tooltip("缩放速度")]
		public float ScaleSpeed;

		// Token: 0x04008DDD RID: 36317
		[Tooltip("缩放时是否动态更新锚点")]
		public bool DynamicPivot;

		// Token: 0x04008DDE RID: 36318
		public Action<Vector3> OnScale;

		// Token: 0x04008DDF RID: 36319
		public Action OnScaleFinish;

		// Token: 0x04008DE0 RID: 36320
		private float _curProgress = 0f;

		// Token: 0x04008DE1 RID: 36321
		protected bool _listenScale = false;

		// Token: 0x04008DE2 RID: 36322
		private bool _scaled = false;

		// Token: 0x04008DE3 RID: 36323
		[SerializeField]
		private RectTransform _rectTrans;

		// Token: 0x04008DE4 RID: 36324
		private RectTransform _rectParent;

		// Token: 0x04008DE5 RID: 36325
		private RectOffset _padding;

		// Token: 0x04008DE6 RID: 36326
		private UIRectDragMove _rectDragMove;

		// Token: 0x04008DE7 RID: 36327
		public UIElement listeningUI;
	}
}
