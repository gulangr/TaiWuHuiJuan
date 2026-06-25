using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000258 RID: 600
public class TangramDraggable : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	// Token: 0x06002785 RID: 10117 RVA: 0x00123A3A File Offset: 0x00121C3A
	private void Awake()
	{
		this._rect = base.GetComponent<RectTransform>();
		this.OriginalPosition = this._rect.localPosition;
	}

	// Token: 0x06002786 RID: 10118 RVA: 0x00123A5A File Offset: 0x00121C5A
	public void OnBeginDrag(PointerEventData eventData)
	{
		Action onBeginDragAction = this.OnBeginDragAction;
		if (onBeginDragAction != null)
		{
			onBeginDragAction();
		}
	}

	// Token: 0x06002787 RID: 10119 RVA: 0x00123A6F File Offset: 0x00121C6F
	public void OnDrag(PointerEventData eventData)
	{
		Action onDragAction = this.OnDragAction;
		if (onDragAction != null)
		{
			onDragAction();
		}
	}

	// Token: 0x06002788 RID: 10120 RVA: 0x00123A84 File Offset: 0x00121C84
	public void OnEndDrag(PointerEventData eventData)
	{
		Action onEndDragAction = this.OnEndDragAction;
		if (onEndDragAction != null)
		{
			onEndDragAction();
		}
	}

	// Token: 0x06002789 RID: 10121 RVA: 0x00123A99 File Offset: 0x00121C99
	public void OnPointerClick(PointerEventData eventData)
	{
		Action onClickgAction = this.OnClickgAction;
		if (onClickgAction != null)
		{
			onClickgAction();
		}
	}

	// Token: 0x0600278A RID: 10122 RVA: 0x00123AAE File Offset: 0x00121CAE
	public void ResetPosition()
	{
		this._rect.localPosition = this.OriginalPosition;
	}

	// Token: 0x0600278B RID: 10123 RVA: 0x00123AC4 File Offset: 0x00121CC4
	public void SnapTo(Vector3 mousePosition)
	{
		Vector2 localMousePos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this._rect.parent as RectTransform, mousePosition, UIManager.Instance.UiCamera, out localMousePos);
		this._rect.localPosition = localMousePos;
	}

	// Token: 0x0600278C RID: 10124 RVA: 0x00123B0C File Offset: 0x00121D0C
	public void SnapToCursor(PointerEventData eventData)
	{
		Vector2 localMousePos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this._rect.parent as RectTransform, eventData.position, eventData.pressEventCamera, out localMousePos);
		this._rect.localPosition = localMousePos;
	}

	// Token: 0x04001CC7 RID: 7367
	public Action OnBeginDragAction;

	// Token: 0x04001CC8 RID: 7368
	public Action OnEndDragAction;

	// Token: 0x04001CC9 RID: 7369
	public Action OnDragAction;

	// Token: 0x04001CCA RID: 7370
	public Action OnClickgAction;

	// Token: 0x04001CCB RID: 7371
	private RectTransform _rect;

	// Token: 0x04001CCC RID: 7372
	public Vector3 OriginalPosition;
}
