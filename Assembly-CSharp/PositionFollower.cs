using System;
using UnityEngine;

// Token: 0x0200008C RID: 140
[ExecuteAlways]
public class PositionFollower : MonoBehaviour
{
	// Token: 0x06000507 RID: 1287 RVA: 0x00022A9A File Offset: 0x00020C9A
	private void LateUpdate()
	{
		this.Excute();
	}

	// Token: 0x06000508 RID: 1288 RVA: 0x00022AA4 File Offset: 0x00020CA4
	public void Excute()
	{
		bool flag = null == this.Target;
		if (!flag)
		{
			RectTransform rectTrans;
			bool flag2;
			if (this.ContainTargetRectWidth)
			{
				rectTrans = (this.Target as RectTransform);
				flag2 = (rectTrans != null);
			}
			else
			{
				flag2 = false;
			}
			bool flag3 = flag2;
			Vector3 targetPos;
			if (flag3)
			{
				float x = this.FollowLeft ? (-rectTrans.rect.width) : rectTrans.rect.width;
				targetPos = rectTrans.TransformPoint(new Vector3(x, 0f));
			}
			else
			{
				targetPos = this.Target.position;
			}
			Vector3 pos = base.transform.parent.InverseTransformPoint(targetPos);
			pos.x += this.Offset.x;
			pos.y += this.Offset.y;
			pos.z += this.Offset.z;
			base.transform.localPosition = pos;
		}
	}

	// Token: 0x0400040F RID: 1039
	[Tooltip("要跟随坐标的目标")]
	public Transform Target;

	// Token: 0x04000410 RID: 1040
	[Tooltip("与目标的坐标偏移值")]
	public Vector3 Offset;

	// Token: 0x04000411 RID: 1041
	public bool ContainTargetRectWidth;

	// Token: 0x04000412 RID: 1042
	public bool FollowLeft;
}
