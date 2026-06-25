using System;
using UnityEngine;

// Token: 0x02000362 RID: 866
public class UIFixedScaleChildrenManual : Refers
{
	// Token: 0x06003241 RID: 12865 RVA: 0x0018CB29 File Offset: 0x0018AD29
	protected virtual void Awake()
	{
	}

	// Token: 0x06003242 RID: 12866 RVA: 0x0018CB2C File Offset: 0x0018AD2C
	public void AdjustScaleByParentScale(Vector3 scale)
	{
		Vector3 inverseScale = new Vector3(1f / scale.x, 1f / scale.y, 1f / scale.z);
		foreach (Transform trans in this._autoScaleTrans)
		{
			trans.localScale = inverseScale;
		}
	}

	// Token: 0x06003243 RID: 12867 RVA: 0x0018CB89 File Offset: 0x0018AD89
	public void SetParent(Transform target)
	{
		base.transform.SetParent(target);
	}

	// Token: 0x040024CD RID: 9421
	[SerializeField]
	protected Transform[] _autoScaleTrans;
}
