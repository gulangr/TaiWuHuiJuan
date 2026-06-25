using System;
using UnityEngine;

// Token: 0x0200004D RID: 77
public class CircularLayoutElement : MonoBehaviour
{
	// Token: 0x0600028B RID: 651 RVA: 0x0000F870 File Offset: 0x0000DA70
	public float CalcSize()
	{
		RectTransform rectTransform = base.GetComponent<RectTransform>();
		return (this.sizeMode == CircularLayoutElement.SizeMode.Height) ? rectTransform.rect.height : rectTransform.rect.width;
	}

	// Token: 0x0400014C RID: 332
	public CircularLayoutElement.SizeMode sizeMode = CircularLayoutElement.SizeMode.Width;

	// Token: 0x0400014D RID: 333
	public bool ignoreLayout = false;

	// Token: 0x0400014E RID: 334
	public bool lockRotation = false;

	// Token: 0x0400014F RID: 335
	public Vector3 lockedRotation = Vector3.zero;

	// Token: 0x020010C8 RID: 4296
	public enum SizeMode
	{
		// Token: 0x0400945F RID: 37983
		Width,
		// Token: 0x04009460 RID: 37984
		Height
	}
}
