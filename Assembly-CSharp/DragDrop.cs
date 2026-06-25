using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200006B RID: 107
public class DragDrop : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
	// Token: 0x17000066 RID: 102
	// (get) Token: 0x060003AD RID: 941 RVA: 0x00016CBC File Offset: 0x00014EBC
	// (set) Token: 0x060003AE RID: 942 RVA: 0x00016CCE File Offset: 0x00014ECE
	public object Identify
	{
		get
		{
			return this._identity ?? base.gameObject;
		}
		set
		{
			this._identity = value;
		}
	}

	// Token: 0x060003AF RID: 943 RVA: 0x00016CD8 File Offset: 0x00014ED8
	public void OnBeginDrag(PointerEventData eventData)
	{
	}

	// Token: 0x060003B0 RID: 944 RVA: 0x00016CDB File Offset: 0x00014EDB
	public void OnDrag(PointerEventData eventData)
	{
	}

	// Token: 0x060003B1 RID: 945 RVA: 0x00016CDE File Offset: 0x00014EDE
	public void OnPointerEnter(PointerEventData eventData)
	{
	}

	// Token: 0x060003B2 RID: 946 RVA: 0x00016CE1 File Offset: 0x00014EE1
	public void OnPointerExit(PointerEventData eventData)
	{
	}

	// Token: 0x060003B3 RID: 947 RVA: 0x00016CE4 File Offset: 0x00014EE4
	private void OnEnable()
	{
		base.enabled = false;
	}

	// Token: 0x0400023F RID: 575
	public bool DragOn;

	// Token: 0x04000240 RID: 576
	public bool DropOn;

	// Token: 0x04000241 RID: 577
	public GameObject CopyNode;

	// Token: 0x04000242 RID: 578
	public DragDropGroup Controller;

	// Token: 0x04000243 RID: 579
	private object _identity;

	// Token: 0x04000244 RID: 580
	public Predicate<object> CanDrop;

	// Token: 0x04000245 RID: 581
	public Func<bool> CanDrag;

	// Token: 0x04000246 RID: 582
	public Action OnEnterNotice;

	// Token: 0x04000247 RID: 583
	public Action OnExitNotice;
}
