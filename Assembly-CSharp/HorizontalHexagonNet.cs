using System;
using System.Collections.Generic;
using FrameWork;
using UnityEngine;

// Token: 0x02000065 RID: 101
[RequireComponent(typeof(LineRenderer2D))]
public class HorizontalHexagonNet : MonoBehaviour
{
	// Token: 0x1700005B RID: 91
	// (get) Token: 0x0600034B RID: 843 RVA: 0x00013FAB File Offset: 0x000121AB
	// (set) Token: 0x0600034C RID: 844 RVA: 0x00013FB3 File Offset: 0x000121B3
	public int Row { get; private set; }

	// Token: 0x1700005C RID: 92
	// (get) Token: 0x0600034D RID: 845 RVA: 0x00013FBC File Offset: 0x000121BC
	// (set) Token: 0x0600034E RID: 846 RVA: 0x00013FC4 File Offset: 0x000121C4
	public int Column { get; private set; }

	// Token: 0x1700005D RID: 93
	// (get) Token: 0x0600034F RID: 847 RVA: 0x00013FCD File Offset: 0x000121CD
	private int FinalLeftEdge
	{
		get
		{
			return -this.PadSize.x;
		}
	}

	// Token: 0x1700005E RID: 94
	// (get) Token: 0x06000350 RID: 848 RVA: 0x00013FDB File Offset: 0x000121DB
	private int FinalRightEdge
	{
		get
		{
			return this.Column + this.PadSize.x;
		}
	}

	// Token: 0x1700005F RID: 95
	// (get) Token: 0x06000351 RID: 849 RVA: 0x00013FEF File Offset: 0x000121EF
	private int FinalTopEdge
	{
		get
		{
			return -this.PadSize.y;
		}
	}

	// Token: 0x17000060 RID: 96
	// (get) Token: 0x06000352 RID: 850 RVA: 0x00013FFD File Offset: 0x000121FD
	private int FinalBottomEdge
	{
		get
		{
			return this.Row + this.PadSize.y;
		}
	}

	// Token: 0x06000353 RID: 851 RVA: 0x00014014 File Offset: 0x00012214
	private void Init()
	{
		this._lineRendererPreset = base.GetComponent<LineRenderer2D>();
		bool flag = null != this.NodeObject;
		if (flag)
		{
			RectTransform nodeRectTrans = this.NodeObject.GetComponent<RectTransform>();
			nodeRectTrans.anchorMin = (nodeRectTrans.anchorMax = Vector2.up);
			this.NodeObject.SetActive(false);
		}
	}

	// Token: 0x06000354 RID: 852 RVA: 0x00014070 File Offset: 0x00012270
	public void UpdateSize(int row, int column)
	{
		this.Row = row;
		this.Column = column;
		this.Clear();
		this._widthStep = (float)this.HexagonSize.x * 0.5f;
		this._heightStep = (float)this.HexagonSize.y * 0.5f;
		Vector2 size;
		size.x = this._widthStep * (float)(this.Column - 1) + (float)(2 * this.Padding.x);
		size.y = this._heightStep * (float)(this.Row - 1) + (float)(2 * this.Padding.y);
		base.GetComponent<RectTransform>().SetSize(size);
		bool flag = null != this.NodeObject;
		if (flag)
		{
			GameObject nodeRoot = new GameObject("NodeRoot", new Type[]
			{
				typeof(RectTransform)
			});
			nodeRoot.transform.SetParent(base.transform, false);
			nodeRoot.transform.SetAsLastSibling();
			this._nodeParent = nodeRoot.GetComponent<RectTransform>();
			this._nodeParent.SetSize(Vector2.one);
			this._nodeParent.anchorMin = (this._nodeParent.anchorMax = (this._nodeParent.pivot = Vector2.up));
			this._nodeParent.anchoredPosition = Vector2.zero;
		}
		for (int i = this.FinalTopEdge; i < this.FinalBottomEdge; i++)
		{
			List<Vector2> points = EasyPool.Get<List<Vector2>>();
			int jMax = (i % 2 == 0) ? (this.FinalRightEdge - 1) : this.FinalRightEdge;
			bool flag2 = i == this.FinalTopEdge;
			if (flag2)
			{
				points.Add(this.LocationToPoint(this.FinalTopEdge, this.FinalLeftEdge).SetY(0f));
				points.Add(this.LocationToPoint(this.FinalTopEdge, this.FinalRightEdge - 2).SetY(0f));
				bool flag3 = null != this.NodeObject;
				if (flag3)
				{
					for (int j = this.FinalLeftEdge; j < jMax; j++)
					{
						GameObject objNode = Object.Instantiate<GameObject>(this.NodeObject, this._nodeParent);
						objNode.name = string.Format("node_{0}_{1}", i, j);
						objNode.GetComponent<RectTransform>().anchoredPosition = this.LocationToPoint(i, j);
						objNode.SetActive(true);
					}
				}
			}
			else
			{
				bool flag4 = i % 2 == 0;
				if (flag4)
				{
					points.Add(this.LocationToPoint(i - 1, this.FinalLeftEdge).SetY(0f));
				}
				for (int k = this.FinalLeftEdge; k < jMax; k++)
				{
					points.Add(this.LocationToPoint(i, k).SetY(-this._heightStep));
					bool flag5 = k + 1 < jMax;
					if (flag5)
					{
						points.Add(this.LocationToPoint(i - 1, (i % 2 == 0) ? (k + 1) : k).SetY(0f));
					}
					bool flag6 = null == this.NodeObject;
					if (!flag6)
					{
						GameObject objNode2 = Object.Instantiate<GameObject>(this.NodeObject, this._nodeParent);
						objNode2.name = string.Format("node_{0}_{1}", i, k);
						objNode2.GetComponent<RectTransform>().anchoredPosition = this.LocationToPoint(i, k);
						objNode2.SetActive(true);
					}
				}
				bool flag7 = i % 2 == 0;
				if (flag7)
				{
					points.Add(this.LocationToPoint(i - 1, this.FinalRightEdge - 1).SetY(0f));
					points.Add(this.LocationToPoint(i - 1, this.FinalRightEdge - 2).SetY(0f));
					points.Add(this.LocationToPoint(i, this.FinalRightEdge - 2).SetY(-this._heightStep));
				}
				points.Add(this.LocationToPoint(i, this.FinalLeftEdge).SetY(-this._heightStep));
			}
			this.CreateNetPartRenderer(points, i);
			EasyPool.Free<List<Vector2>>(points);
		}
	}

	// Token: 0x06000355 RID: 853 RVA: 0x000144C4 File Offset: 0x000126C4
	private void Clear()
	{
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			Object.Destroy(base.transform.GetChild(i).gameObject);
		}
	}

	// Token: 0x06000356 RID: 854 RVA: 0x0001450C File Offset: 0x0001270C
	public Vector2 LocationToPoint(int row, int column)
	{
		bool flag = null == this._lineRendererPreset;
		if (flag)
		{
			this.Init();
		}
		Vector2 point;
		point.x = (float)column * this._widthStep + ((row % 2 == 0) ? (this._widthStep * 0.5f) : 0f) + (float)this.Padding.x;
		point.y = (float)(-(float)this.Padding.y) - (float)row * this._heightStep;
		return point;
	}

	// Token: 0x06000357 RID: 855 RVA: 0x0001458C File Offset: 0x0001278C
	private void CreateNetPartRenderer(List<Vector2> points, int row)
	{
		string rowName = string.Format("row_{0}", row);
		GameObject partObject = new GameObject(rowName, new Type[]
		{
			typeof(CImage),
			typeof(LineRenderer2D)
		});
		partObject.transform.SetParent(base.transform, false);
		partObject.transform.SetAsFirstSibling();
		RectTransform partRectTrans = partObject.GetComponent<RectTransform>();
		partRectTrans.pivot = (partRectTrans.anchorMin = (partRectTrans.anchorMax = Vector2.up));
		partRectTrans.SetSize(new Vector2(this._widthStep * (float)(this.Column - 1) + (float)(2 * this.Padding.x), (row == this.FinalTopEdge) ? this._lineRendererPreset.Width : this._heightStep));
		Vector2 pos = Vector2.down * (float)(row - 1) * this._heightStep;
		pos = pos.SetY(((row == this.FinalTopEdge) ? (pos.y - this._heightStep) : pos.y) - (float)this.Padding.y);
		partRectTrans.anchoredPosition = pos;
		LineRenderer2D renderer2D = partObject.GetComponent<LineRenderer2D>();
		renderer2D.Width = this._lineRendererPreset.Width;
		renderer2D.SubLineAmount = this._lineRendererPreset.SubLineAmount;
		renderer2D.CornerRadius = this._lineRendererPreset.CornerRadius;
		renderer2D.CornerQuality = this._lineRendererPreset.CornerQuality;
		renderer2D.StartColor = this._lineRendererPreset.StartColor;
		renderer2D.EndColor = this._lineRendererPreset.EndColor;
		renderer2D.Dashed = this._lineRendererPreset.Dashed;
		renderer2D.DashedSolidLength = this._lineRendererPreset.DashedSolidLength;
		renderer2D.DashedSpaceLength = this._lineRendererPreset.DashedSpaceLength;
		renderer2D.Shadowed = this._lineRendererPreset.Shadowed;
		renderer2D.ShadowWidth = this._lineRendererPreset.ShadowWidth;
		renderer2D.ShadowTint = this._lineRendererPreset.ShadowTint;
		renderer2D.Vertices = points.ToArray();
		partObject.GetComponent<CImage>().SetAllDirty();
	}

	// Token: 0x040001F8 RID: 504
	public Vector2Int HexagonSize;

	// Token: 0x040001F9 RID: 505
	public Vector2Int Padding;

	// Token: 0x040001FC RID: 508
	[Tooltip("上下左右网格点阵的额外铺设尺寸")]
	public Vector2Int PadSize;

	// Token: 0x040001FD RID: 509
	public GameObject NodeObject;

	// Token: 0x040001FE RID: 510
	private LineRenderer2D _lineRendererPreset;

	// Token: 0x040001FF RID: 511
	private RectTransform _nodeParent;

	// Token: 0x04000200 RID: 512
	private Vector2 _pivotOffset;

	// Token: 0x04000201 RID: 513
	private float _widthStep;

	// Token: 0x04000202 RID: 514
	private float _heightStep;
}
