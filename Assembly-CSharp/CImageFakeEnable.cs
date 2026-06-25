using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200004A RID: 74
[RequireComponent(typeof(CImage))]
public class CImageFakeEnable : BaseMeshEffect
{
	// Token: 0x0600027C RID: 636 RVA: 0x0000F10F File Offset: 0x0000D30F
	protected override void Awake()
	{
		base.Awake();
		this._cImage = base.GetComponent<CImage>();
	}

	// Token: 0x0600027D RID: 637 RVA: 0x0000F128 File Offset: 0x0000D328
	public override void ModifyMesh(VertexHelper vh)
	{
		bool flag = this._cImage == null;
		if (flag)
		{
			this._cImage = base.GetComponent<CImage>();
		}
		bool flag2 = !this._cImage.enabled;
		if (flag2)
		{
			vh.Clear();
		}
	}

	// Token: 0x0400013D RID: 317
	private CImage _cImage;
}
