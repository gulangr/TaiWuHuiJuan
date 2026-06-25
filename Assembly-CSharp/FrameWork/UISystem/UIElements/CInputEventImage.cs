using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace FrameWork.UISystem.UIElements
{
	// Token: 0x02001005 RID: 4101
	public class CInputEventImage : CImage, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
	{
		// Token: 0x17001517 RID: 5399
		// (get) Token: 0x0600BB24 RID: 47908 RVA: 0x00552B1B File Offset: 0x00550D1B
		// (set) Token: 0x0600BB25 RID: 47909 RVA: 0x00552B23 File Offset: 0x00550D23
		public bool IgnoreDrag { get; set; } = false;

		// Token: 0x0600BB26 RID: 47910 RVA: 0x00552B2C File Offset: 0x00550D2C
		public void OnBeginDrag(PointerEventData eventData)
		{
			bool flag = !this.IgnoreDrag;
			if (flag)
			{
				UnityEvent<PointerEventData> callbackOnBeginDrag = this.CallbackOnBeginDrag;
				if (callbackOnBeginDrag != null)
				{
					callbackOnBeginDrag.Invoke(eventData);
				}
			}
		}

		// Token: 0x0600BB27 RID: 47911 RVA: 0x00552B5C File Offset: 0x00550D5C
		public void OnDrag(PointerEventData eventData)
		{
			bool flag = !this.IgnoreDrag;
			if (flag)
			{
				UnityEvent<PointerEventData> callbackOnDrag = this.CallbackOnDrag;
				if (callbackOnDrag != null)
				{
					callbackOnDrag.Invoke(eventData);
				}
			}
		}

		// Token: 0x0600BB28 RID: 47912 RVA: 0x00552B8C File Offset: 0x00550D8C
		public void OnEndDrag(PointerEventData eventData)
		{
			bool flag = !this.IgnoreDrag;
			if (flag)
			{
				UnityEvent<PointerEventData> callbackOnEndDrag = this.CallbackOnEndDrag;
				if (callbackOnEndDrag != null)
				{
					callbackOnEndDrag.Invoke(eventData);
				}
			}
		}

		// Token: 0x0400906E RID: 36974
		public UnityEvent<PointerEventData> CallbackOnBeginDrag = new UnityEvent<PointerEventData>();

		// Token: 0x0400906F RID: 36975
		public UnityEvent<PointerEventData> CallbackOnDrag = new UnityEvent<PointerEventData>();

		// Token: 0x04009070 RID: 36976
		public UnityEvent<PointerEventData> CallbackOnEndDrag = new UnityEvent<PointerEventData>();
	}
}
