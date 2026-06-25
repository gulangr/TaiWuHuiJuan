using System;
using DG.Tweening;
using UnityEngine;

// Token: 0x0200005E RID: 94
public class EffectBezierFlyCommandUGUI
{
	// Token: 0x0600031B RID: 795 RVA: 0x0001312C File Offset: 0x0001132C
	public static Vector2 GetBezierControlPosBaseOnFrom(Vector2 from, Vector2 to, float xFactor, float yFactor)
	{
		Vector2 result = from;
		float distance = Mathf.Abs(from.x - to.x);
		bool flag = from.x < to.x;
		if (flag)
		{
			result.x = from.x + distance * xFactor;
		}
		else
		{
			result.x = from.x - distance * xFactor;
		}
		distance = Mathf.Abs(from.y - to.y);
		bool flag2 = from.y < to.y;
		if (flag2)
		{
			result.y = from.y + distance * yFactor;
		}
		else
		{
			result.y = from.y - distance * yFactor;
		}
		return result;
	}

	// Token: 0x0600031C RID: 796 RVA: 0x000131D8 File Offset: 0x000113D8
	public static Vector2 GetBezierControlPosBaseOnTo(Vector2 from, Vector2 to, float xFactor, float yFactor)
	{
		Vector2 result = from;
		float distance = Mathf.Abs(from.x - to.x);
		bool flag = to.x < from.x;
		if (flag)
		{
			result.x = to.x + distance * xFactor;
		}
		else
		{
			result.x = to.x - distance * xFactor;
		}
		distance = Mathf.Abs(from.y - to.y);
		bool flag2 = to.y < from.y;
		if (flag2)
		{
			result.y = to.y + distance * yFactor;
		}
		else
		{
			result.y = to.y - distance * yFactor;
		}
		return result;
	}

	// Token: 0x040001C6 RID: 454
	public Vector3 FromPosition;

	// Token: 0x040001C7 RID: 455
	public Vector3 ToPosition;

	// Token: 0x040001C8 RID: 456
	public Vector3 ControlPosition;

	// Token: 0x040001C9 RID: 457
	public float Duration;

	// Token: 0x040001CA RID: 458
	public Ease EaseType = Ease.Linear;

	// Token: 0x040001CB RID: 459
	public GameObject EffectObject;

	// Token: 0x040001CC RID: 460
	public Action OnShowComplete;
}
