using System;
using System.Diagnostics;
using FrameWork;
using UnityEngine;

// Token: 0x0200006C RID: 108
public class DragDropGroup : MonoBehaviour
{
	// Token: 0x17000067 RID: 103
	// (get) Token: 0x060003B5 RID: 949 RVA: 0x00016CF8 File Offset: 0x00014EF8
	// (set) Token: 0x060003B6 RID: 950 RVA: 0x00016D00 File Offset: 0x00014F00
	public object DraggingIdentify { get; private set; }

	// Token: 0x14000004 RID: 4
	// (add) Token: 0x060003B7 RID: 951 RVA: 0x00016D0C File Offset: 0x00014F0C
	// (remove) Token: 0x060003B8 RID: 952 RVA: 0x00016D44 File Offset: 0x00014F44
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private event DragDropGroup.OnDropHandler OnDropEvent;

	// Token: 0x14000005 RID: 5
	// (add) Token: 0x060003B9 RID: 953 RVA: 0x00016D7C File Offset: 0x00014F7C
	// (remove) Token: 0x060003BA RID: 954 RVA: 0x00016DB4 File Offset: 0x00014FB4
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private event DragDropGroup.OnDragStartHandler OnDragStartEvent;

	// Token: 0x060003BB RID: 955 RVA: 0x00016DE9 File Offset: 0x00014FE9
	public void AddOnDragStartHandler(DragDropGroup.OnDragStartHandler handler)
	{
		this.OnDragStartEvent -= handler;
		this.OnDragStartEvent += handler;
	}

	// Token: 0x060003BC RID: 956 RVA: 0x00016DFC File Offset: 0x00014FFC
	public void RemoveOnDragStartHandler(DragDropGroup.OnDragStartHandler handler)
	{
		this.OnDragStartEvent -= handler;
	}

	// Token: 0x060003BD RID: 957 RVA: 0x00016E07 File Offset: 0x00015007
	public void AddOnDropHandler(DragDropGroup.OnDropHandler handler)
	{
		this.OnDropEvent -= handler;
		this.OnDropEvent += handler;
	}

	// Token: 0x060003BE RID: 958 RVA: 0x00016E1A File Offset: 0x0001501A
	public void RemoveOnDropHandler(DragDropGroup.OnDropHandler handler)
	{
		this.OnDropEvent -= handler;
	}

	// Token: 0x060003BF RID: 959 RVA: 0x00016E28 File Offset: 0x00015028
	public void StartDrag(DragDrop dragDrop)
	{
		this.DraggingIdentify = dragDrop.Identify;
		DragDropGroup.OnDragStartHandler onDragStartEvent = this.OnDragStartEvent;
		if (onDragStartEvent != null)
		{
			onDragStartEvent(this.DraggingIdentify);
		}
		bool flag = null != dragDrop.CopyNode;
		if (flag)
		{
			GameObject copy = Object.Instantiate<GameObject>(dragDrop.CopyNode);
			copy.name = dragDrop.CopyNode.name;
			UIElement.DragShow.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("DragItem", copy));
			UIElement.DragShow.Show();
		}
	}

	// Token: 0x060003C0 RID: 960 RVA: 0x00016EB4 File Offset: 0x000150B4
	public void EndDrag()
	{
		bool flag = this.DraggingIdentify != null;
		if (flag)
		{
			object draggingObj = this.DraggingIdentify;
			this.DraggingIdentify = null;
			DragDropGroup.OnDropHandler onDropEvent = this.OnDropEvent;
			if (onDropEvent != null)
			{
				onDropEvent(draggingObj, null);
			}
			UIElement.DragShow.Hide(false);
		}
	}

	// Token: 0x04000248 RID: 584
	[HideInInspector]
	public DragDrop Current;

	// Token: 0x04000249 RID: 585
	public Color[] Colors;

	// Token: 0x020010DB RID: 4315
	// (Invoke) Token: 0x0600C0D0 RID: 49360
	public delegate void OnDropHandler(object dragObj, object dropObj);

	// Token: 0x020010DC RID: 4316
	// (Invoke) Token: 0x0600C0D4 RID: 49364
	public delegate void OnDragStartHandler(object dragObj);
}
