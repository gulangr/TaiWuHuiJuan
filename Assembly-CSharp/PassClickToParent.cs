using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000084 RID: 132
public class PassClickToParent : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x060004E0 RID: 1248 RVA: 0x00021FA9 File Offset: 0x000201A9
	public void OnPointerClick(PointerEventData eventData)
	{
		ExecuteEvents.Execute<IPointerClickHandler>(this.parentTarget, eventData, ExecuteEvents.pointerClickHandler);
	}

	// Token: 0x040003E7 RID: 999
	public GameObject parentTarget;
}
