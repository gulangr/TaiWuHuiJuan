using System;
using UnityEngine;

namespace FrameWork.Tools
{
	// Token: 0x02001031 RID: 4145
	public static class BezierMath
	{
		// Token: 0x0600BD75 RID: 48501 RVA: 0x00560F1C File Offset: 0x0055F11C
		public static Vector2 Bezier2(Vector2 p0, Vector2 p1, Vector2 p2, float t)
		{
			return (1f - t) * ((1f - t) * p0 + t * p1) + t * ((1f - t) * p1 + t * p2);
		}

		// Token: 0x0600BD76 RID: 48502 RVA: 0x00560F78 File Offset: 0x0055F178
		public static Vector3 Bezier2(Vector3 p0, Vector3 p1, Vector3 p2, float t)
		{
			return (1f - t) * ((1f - t) * p0 + t * p1) + t * ((1f - t) * p1 + t * p2);
		}

		// Token: 0x0600BD77 RID: 48503 RVA: 0x00560FD4 File Offset: 0x0055F1D4
		public static Vector2 Bezier3(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
		{
			return (1f - t) * ((1f - t) * ((1f - t) * p0 + t * p1) + t * ((1f - t) * p1 + t * p2)) + t * ((1f - t) * ((1f - t) * p1 + t * p2) + t * ((1f - t) * p2 + t * p3));
		}

		// Token: 0x0600BD78 RID: 48504 RVA: 0x005610A0 File Offset: 0x0055F2A0
		public static Vector3 Bezier3(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			return (1f - t) * ((1f - t) * ((1f - t) * p0 + t * p1) + t * ((1f - t) * p1 + t * p2)) + t * ((1f - t) * ((1f - t) * p1 + t * p2) + t * ((1f - t) * p2 + t * p3));
		}
	}
}
