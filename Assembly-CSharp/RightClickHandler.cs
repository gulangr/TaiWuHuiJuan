using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x020000FE RID: 254
public class RightClickHandler : MonoBehaviour, IPointerUpHandler, IEventSystemHandler
{
	// Token: 0x0600088D RID: 2189 RVA: 0x0003AB90 File Offset: 0x00038D90
	public void OnPointerUp(PointerEventData eventData)
	{
		bool flag = eventData.button == PointerEventData.InputButton.Right;
		if (flag)
		{
			UnityEvent unityEvent = this.onRightClicked;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
		}
	}

	// Token: 0x04000BBC RID: 3004
	[SerializeField]
	private UnityEvent onRightClicked;
}
