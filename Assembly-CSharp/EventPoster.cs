using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000060 RID: 96
public class EventPoster : MonoBehaviour, IScrollHandler, IEventSystemHandler
{
	// Token: 0x06000328 RID: 808 RVA: 0x00013654 File Offset: 0x00011854
	public static void PostEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function, Transform current) where T : IEventSystemHandler
	{
		GameObject nextGo = ExecuteEvents.GetEventHandler<T>(current.parent.gameObject);
		ExecuteEvents.Execute<T>(nextGo, data, function);
	}

	// Token: 0x06000329 RID: 809 RVA: 0x0001367C File Offset: 0x0001187C
	public void OnScroll(PointerEventData eventData)
	{
		bool postScrollEvent = this.PostScrollEvent;
		if (postScrollEvent)
		{
			EventPoster.PostEvent<IScrollHandler>(eventData, ExecuteEvents.scrollHandler, base.transform);
		}
	}

	// Token: 0x040001D0 RID: 464
	public bool PostScrollEvent;
}
