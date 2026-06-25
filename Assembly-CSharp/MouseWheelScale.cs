using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000081 RID: 129
[RequireComponent(typeof(PointerTrigger))]
[ExecuteAlways]
public class MouseWheelScale : MonoBehaviour
{
	// Token: 0x17000080 RID: 128
	// (get) Token: 0x060004C8 RID: 1224 RVA: 0x000215C2 File Offset: 0x0001F7C2
	public float CurProgress
	{
		get
		{
			return this._curProgress;
		}
	}

	// Token: 0x060004C9 RID: 1225 RVA: 0x000215CC File Offset: 0x0001F7CC
	private void Awake()
	{
		this.BindPointerTrigger();
		bool flag = null != this._pointerTrigger;
		if (flag)
		{
			bool flag2 = this._pointerTrigger.EnterEvent.GetPersistentEventCount() <= 0;
			if (flag2)
			{
				this._pointerTrigger.EnterEvent.AddListener(new UnityAction(this.OnPointerEnter));
			}
			bool flag3 = this._pointerTrigger.ExitEvent.GetPersistentEventCount() <= 0;
			if (flag3)
			{
				this._pointerTrigger.ExitEvent.AddListener(new UnityAction(this.OnPointerExit));
			}
		}
		bool flag4 = this._rectTrans == null;
		if (flag4)
		{
			this._rectTrans = base.GetComponent<RectTransform>();
		}
		this._rectParent = base.transform.parent.GetComponent<RectTransform>();
	}

	// Token: 0x060004CA RID: 1226 RVA: 0x00021695 File Offset: 0x0001F895
	protected virtual void BindPointerTrigger()
	{
		this._pointerTrigger = base.GetComponent<PointerTrigger>();
	}

	// Token: 0x060004CB RID: 1227 RVA: 0x000216A4 File Offset: 0x0001F8A4
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

	// Token: 0x060004CC RID: 1228 RVA: 0x00021718 File Offset: 0x0001F918
	private void OnEnable()
	{
		bool flag = this.NeedCheck();
		if (flag)
		{
			this.CheckListenState();
		}
	}

	// Token: 0x060004CD RID: 1229 RVA: 0x00021738 File Offset: 0x0001F938
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

	// Token: 0x060004CE RID: 1230 RVA: 0x00021784 File Offset: 0x0001F984
	public void Reset()
	{
		Vector3 nowScale = base.transform.localScale;
		float nowX = Mathf.Abs(nowScale.x) - Mathf.Min(Mathf.Abs(this.Min.x), Mathf.Abs(this.Max.x));
		float nowY = Mathf.Abs(nowScale.y) - Mathf.Min(Mathf.Abs(this.Min.y), Mathf.Abs(this.Max.y));
		this._curProgress = (nowX / Mathf.Abs(this.Max.x - this.Min.x) + nowY / Mathf.Abs(this.Max.y - this.Min.y)) / 2f;
	}

	// Token: 0x060004CF RID: 1231 RVA: 0x0002184C File Offset: 0x0001FA4C
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

	// Token: 0x060004D0 RID: 1232 RVA: 0x000219B8 File Offset: 0x0001FBB8
	public static void BeforeScale(RectTransform rectTrans, bool centerZoom = false)
	{
		bool flag = !rectTrans;
		if (!flag)
		{
			Vector2 pos;
			Vector2 localPosSelf = (centerZoom && RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTrans, new Vector2((float)Screen.width, (float)Screen.height) * 0.5f, UIManager.Instance.UiCamera, out pos)) ? pos : UIManager.Instance.MousePosToLocalPos(rectTrans);
			Vector2 curPivot = rectTrans.pivot;
			Rect rect = rectTrans.rect;
			localPosSelf += new Vector2(curPivot.x * rect.width, curPivot.y * rect.height);
			curPivot = new Vector2(Mathf.Clamp01(localPosSelf.x / rect.width), Mathf.Clamp01(localPosSelf.y / rect.height));
			rectTrans.SetPivot(curPivot);
		}
	}

	// Token: 0x060004D1 RID: 1233 RVA: 0x00021A88 File Offset: 0x0001FC88
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

	// Token: 0x060004D2 RID: 1234 RVA: 0x00021B7C File Offset: 0x0001FD7C
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

	// Token: 0x060004D3 RID: 1235 RVA: 0x00021BC4 File Offset: 0x0001FDC4
	public void OnPointerEnter()
	{
		bool flag = this.Min != this.Max && Math.Abs(this.ScaleSpeed) > 0f;
		if (flag)
		{
			this._listenScale = true;
		}
	}

	// Token: 0x060004D4 RID: 1236 RVA: 0x00021C05 File Offset: 0x0001FE05
	public void OnPointerExit()
	{
		this._listenScale = false;
		Action onScaleFinish = this.OnScaleFinish;
		if (onScaleFinish != null)
		{
			onScaleFinish();
		}
	}

	// Token: 0x060004D5 RID: 1237 RVA: 0x00021C24 File Offset: 0x0001FE24
	public void ScaleProcess(float scrollValue)
	{
		this.UpdatePivot(scrollValue < 0f);
		this._rectTrans.localScale = Vector3.Lerp(this.Min, this.Max, this._curProgress = Mathf.Clamp01(this._curProgress + scrollValue * this.ScaleSpeed * Time.deltaTime));
		bool flag = this.setPivotAfterScale;
		if (flag)
		{
			this._rectTrans.SetPivot(this.defaultPivot);
		}
		Action<Vector3> onScale = this.OnScale;
		if (onScale != null)
		{
			onScale(this._rectTrans.localScale);
		}
		bool flag2 = this._rectDragMove;
		if (flag2)
		{
			this._rectDragMove.SetDirty();
		}
	}

	// Token: 0x060004D6 RID: 1238 RVA: 0x00021CD8 File Offset: 0x0001FED8
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

	// Token: 0x060004D7 RID: 1239 RVA: 0x00021D5C File Offset: 0x0001FF5C
	private void Update()
	{
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

	// Token: 0x040003D0 RID: 976
	[Tooltip("缩放最小值")]
	public Vector3 Min;

	// Token: 0x040003D1 RID: 977
	[Tooltip("缩放最大值")]
	public Vector3 Max;

	// Token: 0x040003D2 RID: 978
	[Tooltip("缩放速度")]
	public float ScaleSpeed;

	// Token: 0x040003D3 RID: 979
	[Tooltip("缩放时是否动态更新锚点")]
	public bool DynamicPivot;

	// Token: 0x040003D4 RID: 980
	[Tooltip("在缩放后是否修正锚点到默认数值")]
	public bool setPivotAfterScale;

	// Token: 0x040003D5 RID: 981
	[Tooltip("要还原的锚点数值")]
	public Vector2 defaultPivot = new Vector2(0.5f, 0.5f);

	// Token: 0x040003D6 RID: 982
	public Action<Vector3> OnScale;

	// Token: 0x040003D7 RID: 983
	public Action OnScaleFinish;

	// Token: 0x040003D8 RID: 984
	private float _curProgress = 0f;

	// Token: 0x040003D9 RID: 985
	private bool _listenScale = false;

	// Token: 0x040003DA RID: 986
	private bool _scaled = false;

	// Token: 0x040003DB RID: 987
	[SerializeField]
	private RectTransform _rectTrans;

	// Token: 0x040003DC RID: 988
	private RectTransform _rectParent;

	// Token: 0x040003DD RID: 989
	[SerializeField]
	protected PointerTrigger _pointerTrigger;

	// Token: 0x040003DE RID: 990
	private RectOffset _padding;

	// Token: 0x040003DF RID: 991
	private UIRectDragMove _rectDragMove;
}
