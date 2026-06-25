using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020000AB RID: 171
[RequireComponent(typeof(CanvasGroup))]
public class UIRectDragMoveCustom : UIRectDragMove
{
	// Token: 0x060005EB RID: 1515 RVA: 0x00027A1C File Offset: 0x00025C1C
	public void OnBeginDragCustom(BaseEventData eventData)
	{
		PointerEventData pointerEventData = eventData as PointerEventData;
		bool flag = pointerEventData != null;
		if (flag)
		{
			base.OnBeginDrag(pointerEventData);
		}
	}

	// Token: 0x060005EC RID: 1516 RVA: 0x00027A44 File Offset: 0x00025C44
	public void OnEndDragCustom(BaseEventData eventData)
	{
		PointerEventData pointerEventData = eventData as PointerEventData;
		bool flag = pointerEventData != null;
		if (flag)
		{
			base.OnEndDrag(pointerEventData);
		}
	}

	// Token: 0x060005ED RID: 1517 RVA: 0x00027A6C File Offset: 0x00025C6C
	public void OnDragCustom(BaseEventData eventData)
	{
		PointerEventData pointerEventData = eventData as PointerEventData;
		bool flag = pointerEventData != null;
		if (flag)
		{
			base.OnDrag(pointerEventData);
		}
	}
}
