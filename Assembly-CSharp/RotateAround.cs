using System;
using UnityEngine;

// Token: 0x02000091 RID: 145
public class RotateAround : MonoBehaviour
{
	// Token: 0x06000529 RID: 1321 RVA: 0x0002360F File Offset: 0x0002180F
	[ExecuteAlways]
	private void Update()
	{
		base.transform.RotateAround(base.transform.parent.position, this.Axis, this.Speed);
	}

	// Token: 0x04000432 RID: 1074
	public float Speed;

	// Token: 0x04000433 RID: 1075
	public Vector3 Axis;
}
