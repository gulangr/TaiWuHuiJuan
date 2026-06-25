using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200008A RID: 138
[ExecuteAlways]
[RequireComponent(typeof(CanvasRenderer))]
public class PolygonRaycastGraphic : Graphic
{
	// Token: 0x060004FD RID: 1277 RVA: 0x000226A0 File Offset: 0x000208A0
	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
	}

	// Token: 0x060004FE RID: 1278 RVA: 0x000226AC File Offset: 0x000208AC
	public override bool Raycast(Vector2 sp, Camera eventCamera)
	{
		bool flag = this.polygonPoints == null || this.polygonPoints.Length < 3;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			Vector2 localPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(base.rectTransform, sp, eventCamera, out localPoint);
			result = this.IsPointInPolygon(localPoint);
		}
		return result;
	}

	// Token: 0x060004FF RID: 1279 RVA: 0x000226F4 File Offset: 0x000208F4
	private bool IsPointInPolygon(Vector2 point)
	{
		return PolygonRaycastGraphic.IsPointInPolygon(this.polygonPoints, point);
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x00022714 File Offset: 0x00020914
	public static bool IsPointInPolygon(Vector2[] polygon, Vector2 point)
	{
		bool odd = false;
		float px = point.x;
		float py = point.y;
		int i = polygon.Length - 1;
		int j = 0;
		while (j < polygon.Length)
		{
			float startX = polygon[i].x;
			float startY = polygon[i].y;
			float endX = polygon[j].x;
			float endY = polygon[j].y;
			bool flag = (Mathf.Approximately(px, startX) && Mathf.Approximately(py, startY)) || (Mathf.Approximately(px, endX) && Mathf.Approximately(py, endY));
			if (!flag)
			{
				bool flag2 = (py > startY && py <= endY) || (py <= startY && py > endY);
				if (flag2)
				{
					float x = startX + (endX - startX) * (py - startY) / (endY - startY);
					bool flag3 = Mathf.Approximately(x, px);
					if (flag3)
					{
						return true;
					}
					bool flag4 = x < px;
					if (flag4)
					{
						odd = !odd;
					}
				}
				i = j;
				j++;
				continue;
			}
			return true;
		}
		return odd;
	}

	// Token: 0x06000501 RID: 1281 RVA: 0x00022834 File Offset: 0x00020A34
	private void OnDrawGizmos()
	{
		bool flag = this.polygonPoints == null || this.polygonPoints.Length < 3;
		if (!flag)
		{
			Gizmos.color = Color.green;
			Vector3[] worldPoints = new Vector3[this.polygonPoints.Length];
			for (int i = 0; i < this.polygonPoints.Length; i++)
			{
				worldPoints[i] = base.rectTransform.TransformPoint(this.polygonPoints[i]);
			}
			for (int j = 0; j < this.polygonPoints.Length; j++)
			{
				Gizmos.DrawLine(worldPoints[j], worldPoints[(j + 1) % this.polygonPoints.Length]);
			}
		}
	}

	// Token: 0x04000408 RID: 1032
	public Vector2[] polygonPoints;

	// Token: 0x04000409 RID: 1033
	[Header("参考图片（二选一，优先使用 Sprite）")]
	public Sprite referenceSprite;

	// Token: 0x0400040A RID: 1034
	public Texture2D referenceTexture;

	// Token: 0x0400040B RID: 1035
	[Header("生成参数")]
	public float alphaThreshold = 0.1f;

	// Token: 0x0400040C RID: 1036
	public float simplificationTolerance = 2f;
}
