using System;
using UnityEngine;

// Token: 0x02000073 RID: 115
[ExecuteInEditMode]
public class KeepLookAt : MonoBehaviour
{
	// Token: 0x0600042B RID: 1067 RVA: 0x00019E20 File Offset: 0x00018020
	private void LateUpdate()
	{
		bool flag = null != this.Target;
		if (flag)
		{
			this._vecDir = this.Target.position;
		}
		bool flag2 = this.Type == KeepLookAt.KeepType.Forward;
		if (flag2)
		{
			base.transform.forward = this._vecDir;
		}
		else
		{
			bool flag3 = this.Type == KeepLookAt.KeepType.Up;
			if (flag3)
			{
				base.transform.up = this._vecDir;
			}
			else
			{
				bool flag4 = this.Type == KeepLookAt.KeepType.Right;
				if (flag4)
				{
					base.transform.right = this._vecDir;
				}
			}
		}
	}

	// Token: 0x04000294 RID: 660
	[Tooltip("锁定方向")]
	public KeepLookAt.KeepType Type;

	// Token: 0x04000295 RID: 661
	[Tooltip("锁定朝向的目标")]
	public Transform Target;

	// Token: 0x04000296 RID: 662
	private Vector3 _vecDir;

	// Token: 0x020010E6 RID: 4326
	public enum KeepType
	{
		// Token: 0x040094B4 RID: 38068
		Forward,
		// Token: 0x040094B5 RID: 38069
		Up,
		// Token: 0x040094B6 RID: 38070
		Right
	}
}
