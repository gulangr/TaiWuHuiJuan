using System;
using UnityEngine;

// Token: 0x0200035E RID: 862
public static class SortFilter
{
	// Token: 0x06003230 RID: 12848 RVA: 0x0018C110 File Offset: 0x0018A310
	public static Quaternion GetArrowRotation(bool isDescSort)
	{
		return Quaternion.Euler(0f, 0f, (float)(isDescSort ? 0 : 180));
	}

	// Token: 0x06003231 RID: 12849 RVA: 0x0018C140 File Offset: 0x0018A340
	public static Vector2 GetArrowAnchoredPos(bool isDescSort)
	{
		SortFilter._arrowLocalPos.y = (isDescSort ? -30f : 30f);
		return SortFilter._arrowLocalPos;
	}

	// Token: 0x040024B9 RID: 9401
	private const float ArrowOffsetY = 30f;

	// Token: 0x040024BA RID: 9402
	private static Vector2 _arrowLocalPos = Vector2.zero;
}
