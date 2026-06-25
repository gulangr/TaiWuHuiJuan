using System;
using UnityEngine;

// Token: 0x020000AE RID: 174
public class UIRectSizeControllerMinWidth : UIRectSizeController
{
	// Token: 0x06000600 RID: 1536 RVA: 0x00028730 File Offset: 0x00026930
	protected override Vector2 SizeCheck(Vector2 newSize)
	{
		bool flag = newSize.x < this.minWidth;
		if (flag)
		{
			newSize.x = this.minWidth;
		}
		return newSize;
	}

	// Token: 0x040004EC RID: 1260
	[SerializeField]
	private float minWidth;
}
