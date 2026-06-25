using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020000A6 RID: 166
public class UIDragRotate : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
{
	// Token: 0x060005BF RID: 1471 RVA: 0x00026505 File Offset: 0x00024705
	public void OnBeginDrag(PointerEventData eventData)
	{
		this._draging = true;
		Action onDragBegin = this.OnDragBegin;
		if (onDragBegin != null)
		{
			onDragBegin();
		}
	}

	// Token: 0x060005C0 RID: 1472 RVA: 0x00026524 File Offset: 0x00024724
	public void OnDrag(PointerEventData eventData)
	{
		bool flag = !this._draging;
		if (!flag)
		{
			Vector2 curScreenPosition = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, base.transform.position);
			Vector2 directionTo = curScreenPosition - eventData.position;
			Vector2 directionFrom = directionTo - eventData.delta;
			base.transform.rotation *= Quaternion.FromToRotation(directionTo, directionFrom);
			Action<float> onDraging = this.OnDraging;
			if (onDraging != null)
			{
				onDraging(base.transform.localEulerAngles.z);
			}
		}
	}

	// Token: 0x060005C1 RID: 1473 RVA: 0x000265BC File Offset: 0x000247BC
	public void OnEndDrag(PointerEventData eventData)
	{
		this._draging = false;
		Action<float> onDragEnd = this.OnDragEnd;
		if (onDragEnd != null)
		{
			onDragEnd(base.transform.localEulerAngles.z);
		}
	}

	// Token: 0x060005C2 RID: 1474 RVA: 0x000265E8 File Offset: 0x000247E8
	private void OnApplicationFocus(bool focus)
	{
		bool flag = !focus;
		if (flag)
		{
			this._draging = false;
		}
	}

	// Token: 0x040004B1 RID: 1201
	public Action OnDragBegin;

	// Token: 0x040004B2 RID: 1202
	public Action<float> OnDragEnd;

	// Token: 0x040004B3 RID: 1203
	public Action<float> OnDraging;

	// Token: 0x040004B4 RID: 1204
	private bool _draging = false;
}
