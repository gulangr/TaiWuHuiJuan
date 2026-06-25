using System;
using UnityEngine;

// Token: 0x020000AF RID: 175
public class UIRotateAround : MonoBehaviour
{
	// Token: 0x06000602 RID: 1538 RVA: 0x00028770 File Offset: 0x00026970
	private void Start()
	{
		bool flag = null == this.Target;
		if (flag)
		{
			this.Target = base.transform.parent;
		}
	}

	// Token: 0x06000603 RID: 1539 RVA: 0x0002879F File Offset: 0x0002699F
	private void Update()
	{
		base.transform.RotateAround(this.Target.position, Vector3.forward, Time.deltaTime * this.Speed);
	}

	// Token: 0x040004ED RID: 1261
	public Transform Target;

	// Token: 0x040004EE RID: 1262
	public float Speed;
}
