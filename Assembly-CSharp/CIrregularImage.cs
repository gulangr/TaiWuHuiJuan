using System;
using UnityEngine;

// Token: 0x020000CD RID: 205
public class CIrregularImage : CImage
{
	// Token: 0x0600070E RID: 1806 RVA: 0x0003169C File Offset: 0x0002F89C
	protected override void Awake()
	{
		base.Awake();
		Texture2D texture = base.sprite.texture;
		base.alphaHitTestMinimumThreshold = this.threshold;
	}

	// Token: 0x04000778 RID: 1912
	public float threshold = 0.5f;
}
