using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000A4 RID: 164
public class UIChildScroll : MonoBehaviour
{
	// Token: 0x1700008E RID: 142
	// (get) Token: 0x060005B1 RID: 1457 RVA: 0x00025CCC File Offset: 0x00023ECC
	private float HorizontalGap
	{
		get
		{
			return (null == this.HorizontalLayoutController) ? 0f : this.HorizontalLayoutController.spacing;
		}
	}

	// Token: 0x1700008F RID: 143
	// (get) Token: 0x060005B2 RID: 1458 RVA: 0x00025CEE File Offset: 0x00023EEE
	private float VerticalGap
	{
		get
		{
			return (null == this.VerticalLayoutController) ? 0f : this.VerticalLayoutController.spacing;
		}
	}

	// Token: 0x060005B3 RID: 1459 RVA: 0x00025D10 File Offset: 0x00023F10
	private void Start()
	{
		bool flag = null == this.HorizontalLayoutController;
		if (flag)
		{
			this.HorizontalLayoutController = base.GetComponent<HorizontalLayoutGroup>();
		}
		bool flag2 = null == this.VerticalLayoutController;
		if (flag2)
		{
			this.VerticalLayoutController = base.GetComponent<VerticalLayoutGroup>();
		}
		bool flag3 = null != this.HorizontalLayoutController;
		if (flag3)
		{
			this.HorizontalLayoutController.enabled = false;
		}
		bool flag4 = null != this.VerticalLayoutController;
		if (flag4)
		{
			this.VerticalLayoutController.enabled = false;
		}
		bool flag5 = Math.Abs(this.ScrollXSpeed) > 0f && Math.Abs(this.ScrollYSpeed) > 0f;
		if (flag5)
		{
			GLog.Error("can not scroll along both x direction and y direction!");
			base.enabled = false;
		}
		else
		{
			this._rectParent = (base.transform as RectTransform);
			bool flag6 = this._rectParent != null;
			if (flag6)
			{
				this._rectParent.pivot = 0.5f * Vector3.one;
			}
			this.Init();
		}
	}

	// Token: 0x060005B4 RID: 1460 RVA: 0x00025E1D File Offset: 0x0002401D
	public void Init()
	{
		this._scrollElems = new List<RectTransform>(base.transform.GetComponentsInTopChildren(false));
		this._startStatus.Clear();
		this._scrollElems.ForEach(delegate(RectTransform e)
		{
			this._startStatus.Add(e, e.localPosition);
		});
	}

	// Token: 0x060005B5 RID: 1461 RVA: 0x00025E5C File Offset: 0x0002405C
	public void ResetToInitState()
	{
		foreach (KeyValuePair<RectTransform, Vector3> pair in this._startStatus)
		{
			pair.Key.localPosition = pair.Value;
		}
	}

	// Token: 0x060005B6 RID: 1462 RVA: 0x00025EC4 File Offset: 0x000240C4
	private void LateUpdate()
	{
		this.DoScroll();
		this.DoCheck();
	}

	// Token: 0x060005B7 RID: 1463 RVA: 0x00025ED8 File Offset: 0x000240D8
	private void DoScroll()
	{
		this._cumX += this.ScrollXSpeed * Time.deltaTime;
		this._cumY += this.ScrollYSpeed * Time.deltaTime;
		this._scrollElems.ForEach(delegate(RectTransform e)
		{
			Vector2 pos = e.localPosition;
			pos.x += (this.roundToNearestIntCoord ? ((float)Mathf.RoundToInt(this._cumX)) : this._cumX);
			pos.y += (this.roundToNearestIntCoord ? ((float)Mathf.RoundToInt(this._cumY)) : this._cumY);
			e.localPosition = pos;
			bool flag2 = Math.Abs(this.ScrollXSpeed) > 0f;
			if (flag2)
			{
				bool flag3 = null == this._min || this._min.localPosition.x > pos.x;
				if (flag3)
				{
					this._min = e;
				}
				bool flag4 = null == this._max || this._max.localPosition.x < pos.x;
				if (flag4)
				{
					this._max = e;
				}
			}
			bool flag5 = Math.Abs(this.ScrollYSpeed) > 0f;
			if (flag5)
			{
				bool flag6 = null == this._min || this._min.localPosition.y > pos.y;
				if (flag6)
				{
					this._min = e;
				}
				bool flag7 = null == this._max || this._max.localPosition.y < pos.y;
				if (flag7)
				{
					this._max = e;
				}
			}
		});
		bool flag = this.roundToNearestIntCoord;
		if (flag)
		{
			this._cumX -= (float)Mathf.RoundToInt(this._cumX);
			this._cumY -= (float)Mathf.RoundToInt(this._cumY);
		}
		else
		{
			this._cumX = (this._cumY = 0f);
		}
	}

	// Token: 0x060005B8 RID: 1464 RVA: 0x00025F84 File Offset: 0x00024184
	private void DoCheck()
	{
		bool flag = null == this._min || null == this._max;
		if (!flag)
		{
			Rect minRect = this._min.rect;
			Rect maxRect = this._max.rect;
			this._parentRect = this._rectParent.rect;
			float offsetX = maxRect.width * 0.5f + minRect.width * 0.5f + this.HorizontalGap;
			float offsetY = maxRect.height * 0.5f + minRect.height * 0.5f + this.VerticalGap;
			float halfBufferW = this._parentRect.width * 0.5f + this.BufferSize;
			float halfBufferH = this._parentRect.height * 0.5f + this.BufferSize;
			bool flag2 = this.ScrollXSpeed < 0f;
			if (flag2)
			{
				bool flag3 = this._min.localPosition.x + minRect.width * 0.5f < -halfBufferW;
				if (flag3)
				{
					this._min.localPosition = this._max.localPosition + Vector3.right * offsetX;
				}
			}
			else
			{
				bool flag4 = this.ScrollXSpeed > 0f;
				if (flag4)
				{
					bool flag5 = this._max.localPosition.x - maxRect.width * 0.5f > halfBufferW;
					if (flag5)
					{
						this._max.localPosition = this._min.localPosition - Vector3.right * offsetX;
					}
				}
				else
				{
					bool flag6 = this.ScrollYSpeed < 0f;
					if (flag6)
					{
						bool flag7 = this._min.localPosition.y + minRect.height * 0.5f < -halfBufferH;
						if (flag7)
						{
							this._min.localPosition = this._max.localPosition + Vector3.up * offsetY;
						}
					}
					else
					{
						bool flag8 = this.ScrollYSpeed > 0f;
						if (flag8)
						{
							bool flag9 = this._max.localPosition.y - maxRect.height * 0.5f > halfBufferH;
							if (flag9)
							{
								this._max.localPosition = this._min.localPosition - Vector3.up * offsetY;
							}
						}
					}
				}
			}
			this._min = null;
			this._max = null;
		}
	}

	// Token: 0x0400049E RID: 1182
	public float ScrollXSpeed;

	// Token: 0x0400049F RID: 1183
	public float ScrollYSpeed;

	// Token: 0x040004A0 RID: 1184
	public float BufferSize;

	// Token: 0x040004A1 RID: 1185
	private List<RectTransform> _scrollElems;

	// Token: 0x040004A2 RID: 1186
	private RectTransform _rectParent;

	// Token: 0x040004A3 RID: 1187
	private Rect _parentRect;

	// Token: 0x040004A4 RID: 1188
	private RectTransform _min;

	// Token: 0x040004A5 RID: 1189
	private RectTransform _max;

	// Token: 0x040004A6 RID: 1190
	private Dictionary<RectTransform, Vector3> _startStatus = new Dictionary<RectTransform, Vector3>();

	// Token: 0x040004A7 RID: 1191
	public HorizontalLayoutGroup HorizontalLayoutController;

	// Token: 0x040004A8 RID: 1192
	public VerticalLayoutGroup VerticalLayoutController;

	// Token: 0x040004A9 RID: 1193
	public Action<Transform> OnReset;

	// Token: 0x040004AA RID: 1194
	[SerializeField]
	private bool roundToNearestIntCoord = false;

	// Token: 0x040004AB RID: 1195
	private float _cumX;

	// Token: 0x040004AC RID: 1196
	private float _cumY;
}
