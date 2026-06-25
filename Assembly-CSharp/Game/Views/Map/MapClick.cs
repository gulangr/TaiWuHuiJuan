using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Views.Map
{
	// Token: 0x02000940 RID: 2368
	public class MapClick : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
	{
		// Token: 0x06006E7A RID: 28282 RVA: 0x00331CD8 File Offset: 0x0032FED8
		public void OnPointerUp(PointerEventData eventData)
		{
			bool flag = eventData.button == PointerEventData.InputButton.Left;
			if (flag)
			{
				float tmLasts = Time.realtimeSinceStartup - this._tmDown;
				Vector3 vecDistance = Input.mousePosition - this._posDown;
				bool flag2 = tmLasts <= 0.2f && vecDistance.sqrMagnitude <= 2f;
				if (flag2)
				{
					Action onClickEvent = this.OnClickEvent;
					if (onClickEvent != null)
					{
						onClickEvent();
					}
				}
			}
		}

		// Token: 0x06006E7B RID: 28283 RVA: 0x00331D48 File Offset: 0x0032FF48
		public void OnPointerDown(PointerEventData eventData)
		{
			bool flag = eventData.button == PointerEventData.InputButton.Left;
			if (flag)
			{
				this._tmDown = Time.realtimeSinceStartup;
				this._posDown = Input.mousePosition;
			}
		}

		// Token: 0x06006E7C RID: 28284 RVA: 0x00331D7B File Offset: 0x0032FF7B
		public void OnPointerEnter(PointerEventData eventData)
		{
			Action onEnterEvent = this.OnEnterEvent;
			if (onEnterEvent != null)
			{
				onEnterEvent();
			}
		}

		// Token: 0x06006E7D RID: 28285 RVA: 0x00331D90 File Offset: 0x0032FF90
		public void OnPointerExit(PointerEventData eventData)
		{
			Action onExitEvent = this.OnExitEvent;
			if (onExitEvent != null)
			{
				onExitEvent();
			}
		}

		// Token: 0x04005232 RID: 21042
		public MapCamera MapCamera;

		// Token: 0x04005233 RID: 21043
		public Action OnClickEvent;

		// Token: 0x04005234 RID: 21044
		public Action OnEnterEvent;

		// Token: 0x04005235 RID: 21045
		public Action OnExitEvent;

		// Token: 0x04005236 RID: 21046
		private float _tmDown;

		// Token: 0x04005237 RID: 21047
		private Vector3 _posDown;
	}
}
