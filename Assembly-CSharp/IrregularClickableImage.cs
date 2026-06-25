using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000072 RID: 114
public class IrregularClickableImage : Image
{
	// Token: 0x06000427 RID: 1063 RVA: 0x00019DB0 File Offset: 0x00017FB0
	protected override void Awake()
	{
		base.Awake();
		base.alphaHitTestMinimumThreshold = this.threshold;
	}

	// Token: 0x06000428 RID: 1064 RVA: 0x00019DC8 File Offset: 0x00017FC8
	public IrregularClickableImage SetColor(Color newColor)
	{
		this.color = newColor;
		return this;
	}

	// Token: 0x06000429 RID: 1065 RVA: 0x00019DE4 File Offset: 0x00017FE4
	public void SetAlpha(float alpha)
	{
		Color c = this.color;
		c.a = alpha;
		this.color = c;
	}

	// Token: 0x04000293 RID: 659
	public float threshold = 0.5f;
}
