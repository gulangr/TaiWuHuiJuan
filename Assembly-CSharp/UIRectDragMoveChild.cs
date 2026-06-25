using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020000AA RID: 170
[RequireComponent(typeof(CanvasGroup))]
public class UIRectDragMoveChild : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
{
	// Token: 0x17000091 RID: 145
	// (get) Token: 0x060005E4 RID: 1508 RVA: 0x0002798D File Offset: 0x00025B8D
	protected bool _dragging
	{
		get
		{
			return this.uIRectDragBase;
		}
	}

	// Token: 0x060005E5 RID: 1509 RVA: 0x0002799C File Offset: 0x00025B9C
	private void Start()
	{
		this._canvasGroup = base.GetComponent<CanvasGroup>();
		bool flag = null == this._canvasGroup;
		if (flag)
		{
			this._canvasGroup = base.gameObject.AddComponent<CanvasGroup>();
		}
	}

	// Token: 0x060005E6 RID: 1510 RVA: 0x000279D7 File Offset: 0x00025BD7
	public void SetDirty()
	{
		this._dirtyFlag = true;
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x000279E1 File Offset: 0x00025BE1
	public void OnDrag(PointerEventData eventData)
	{
		this.uIRectDragBase.OnDrag(eventData);
	}

	// Token: 0x060005E8 RID: 1512 RVA: 0x000279F1 File Offset: 0x00025BF1
	public void OnBeginDrag(PointerEventData eventData)
	{
		this.uIRectDragBase.OnBeginDrag(eventData);
	}

	// Token: 0x060005E9 RID: 1513 RVA: 0x00027A01 File Offset: 0x00025C01
	public void OnEndDrag(PointerEventData eventData)
	{
		this.uIRectDragBase.OnEndDrag(eventData);
	}

	// Token: 0x040004DA RID: 1242
	public UIRectDragMove uIRectDragBase;

	// Token: 0x040004DB RID: 1243
	private RectTransform _selfRectTransform;

	// Token: 0x040004DC RID: 1244
	private RectTransform _parentRectTransform;

	// Token: 0x040004DD RID: 1245
	private CanvasGroup _canvasGroup;

	// Token: 0x040004DE RID: 1246
	private bool _dirtyFlag;

	// Token: 0x040004DF RID: 1247
	public Action BeginDragCallback;

	// Token: 0x040004E0 RID: 1248
	public Action EndDragCallback;
}
