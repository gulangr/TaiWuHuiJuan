using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020000D5 RID: 213
public class ReverseHorizontalScrollRect : ScrollRect
{
	// Token: 0x060007A5 RID: 1957 RVA: 0x00035A24 File Offset: 0x00033C24
	public override void OnScroll(PointerEventData data)
	{
		bool flag = base.horizontal && !base.vertical;
		if (flag)
		{
			data.scrollDelta = new Vector2(-data.scrollDelta.x, -data.scrollDelta.y);
			base.OnScroll(data);
		}
		else
		{
			base.OnScroll(data);
		}
	}
}
