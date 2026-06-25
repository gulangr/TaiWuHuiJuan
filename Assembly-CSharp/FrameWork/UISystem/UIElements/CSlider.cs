using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FrameWork.UISystem.UIElements
{
	// Token: 0x02001008 RID: 4104
	[RequireComponent(typeof(UIInteractionBehaviour))]
	public class CSlider : Slider
	{
		// Token: 0x0600BB8E RID: 48014 RVA: 0x00555AA0 File Offset: 0x00553CA0
		public override void OnPointerDown(PointerEventData eventData)
		{
			base.OnPointerDown(eventData);
			base.GetComponent<UIInteractionBehaviour>().Play(base.interactable);
		}

		// Token: 0x0600BB8F RID: 48015 RVA: 0x00555ABD File Offset: 0x00553CBD
		public override void OnPointerUp(PointerEventData eventData)
		{
			base.OnPointerUp(eventData);
		}
	}
}
