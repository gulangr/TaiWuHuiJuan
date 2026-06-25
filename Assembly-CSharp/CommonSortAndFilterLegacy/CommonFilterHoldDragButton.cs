using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000423 RID: 1059
	public class CommonFilterHoldDragButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		// Token: 0x06003EE3 RID: 16099 RVA: 0x001F7745 File Offset: 0x001F5945
		public void OnPointerDown(PointerEventData eventData)
		{
			CommonFilterHoldDragButton.HoldDragEvent onPointerDownEvent = this.OnPointerDownEvent;
			if (onPointerDownEvent != null)
			{
				onPointerDownEvent(base.gameObject, eventData);
			}
			this.ForwardEvent<IPointerDownHandler>(eventData, ExecuteEvents.pointerDownHandler);
		}

		// Token: 0x06003EE4 RID: 16100 RVA: 0x001F776E File Offset: 0x001F596E
		public void OnPointerUp(PointerEventData eventData)
		{
			CommonFilterHoldDragButton.HoldDragEvent onPointerUpEvent = this.OnPointerUpEvent;
			if (onPointerUpEvent != null)
			{
				onPointerUpEvent(base.gameObject, eventData);
			}
			this.ForwardEvent<IPointerUpHandler>(eventData, ExecuteEvents.pointerUpHandler);
		}

		// Token: 0x06003EE5 RID: 16101 RVA: 0x001F7797 File Offset: 0x001F5997
		public void OnBeginDrag(PointerEventData eventData)
		{
			CommonFilterHoldDragButton.HoldDragEvent onBeginDragEvent = this.OnBeginDragEvent;
			if (onBeginDragEvent != null)
			{
				onBeginDragEvent(base.gameObject, eventData);
			}
			this.ForwardEvent<IBeginDragHandler>(eventData, ExecuteEvents.beginDragHandler);
		}

		// Token: 0x06003EE6 RID: 16102 RVA: 0x001F77C0 File Offset: 0x001F59C0
		public void OnDrag(PointerEventData eventData)
		{
			CommonFilterHoldDragButton.HoldDragEvent onDragEvent = this.OnDragEvent;
			if (onDragEvent != null)
			{
				onDragEvent(base.gameObject, eventData);
			}
			this.ForwardEvent<IDragHandler>(eventData, ExecuteEvents.dragHandler);
		}

		// Token: 0x06003EE7 RID: 16103 RVA: 0x001F77E9 File Offset: 0x001F59E9
		public void OnEndDrag(PointerEventData eventData)
		{
			CommonFilterHoldDragButton.HoldDragEvent onEndDragEvent = this.OnEndDragEvent;
			if (onEndDragEvent != null)
			{
				onEndDragEvent(base.gameObject, eventData);
			}
			this.ForwardEvent<IEndDragHandler>(eventData, ExecuteEvents.endDragHandler);
		}

		// Token: 0x06003EE8 RID: 16104 RVA: 0x001F7814 File Offset: 0x001F5A14
		private void ForwardEvent<T>(PointerEventData eventData, ExecuteEvents.EventFunction<T> eventFunction) where T : IEventSystemHandler
		{
			bool flag = this.ForwardTarget != null;
			if (flag)
			{
				ExecuteEvents.Execute<T>(this.ForwardTarget, eventData, eventFunction);
			}
		}

		// Token: 0x04002D28 RID: 11560
		public CommonFilterHoldDragButton.HoldDragEvent OnPointerDownEvent;

		// Token: 0x04002D29 RID: 11561
		public CommonFilterHoldDragButton.HoldDragEvent OnPointerUpEvent;

		// Token: 0x04002D2A RID: 11562
		public CommonFilterHoldDragButton.HoldDragEvent OnBeginDragEvent;

		// Token: 0x04002D2B RID: 11563
		public CommonFilterHoldDragButton.HoldDragEvent OnDragEvent;

		// Token: 0x04002D2C RID: 11564
		public CommonFilterHoldDragButton.HoldDragEvent OnEndDragEvent;

		// Token: 0x04002D2D RID: 11565
		public GameObject ForwardTarget;

		// Token: 0x020018C8 RID: 6344
		// (Invoke) Token: 0x0600D7D9 RID: 55257
		public delegate void HoldDragEvent(GameObject target, PointerEventData eventData);
	}
}
