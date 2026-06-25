using System;
using UnityEngine;

// Token: 0x0200008B RID: 139
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(RectTransform))]
public class PolygonRaycastTarget : MonoBehaviour, ICanvasRaycastFilter
{
	// Token: 0x06000503 RID: 1283 RVA: 0x00022915 File Offset: 0x00020B15
	private void Awake()
	{
		this._rectTrans = base.GetComponent<RectTransform>();
		this._collider = base.GetComponent<PolygonCollider2D>();
	}

	// Token: 0x06000504 RID: 1284 RVA: 0x00022930 File Offset: 0x00020B30
	public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
	{
		bool flag = null == this._collider;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			Vector2 localPos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this._rectTrans, sp, eventCamera, out localPos);
			result = this.ContainsPoint(this._collider.points, localPos);
		}
		return result;
	}

	// Token: 0x06000505 RID: 1285 RVA: 0x00022978 File Offset: 0x00020B78
	private bool ContainsPoint(Vector2[] polyPoints, Vector2 p)
	{
		int cn = 0;
		for (int i = 0; i < polyPoints.Length - 1; i++)
		{
			bool flag = (polyPoints[i].y <= p.y && polyPoints[i + 1].y > p.y) || (polyPoints[i].y > p.y && polyPoints[i + 1].y <= p.y);
			if (flag)
			{
				float vt = (p.y - polyPoints[i].y) / (polyPoints[i + 1].y - polyPoints[i].y);
				bool flag2 = p.x < polyPoints[i].x + vt * (polyPoints[i + 1].x - polyPoints[i].x);
				if (flag2)
				{
					cn++;
				}
			}
		}
		bool flag3 = cn == 0;
		return !flag3 && cn % 2 != 0;
	}

	// Token: 0x0400040D RID: 1037
	private PolygonCollider2D _collider;

	// Token: 0x0400040E RID: 1038
	private RectTransform _rectTrans;
}
