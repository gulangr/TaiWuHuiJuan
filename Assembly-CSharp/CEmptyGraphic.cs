using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000C9 RID: 201
[RequireComponent(typeof(CanvasRenderer))]
public class CEmptyGraphic : Graphic
{
	// Token: 0x060006F1 RID: 1777 RVA: 0x0003086E File Offset: 0x0002EA6E
	protected override void Awake()
	{
		this.raycastTarget = true;
	}

	// Token: 0x060006F2 RID: 1778 RVA: 0x00030879 File Offset: 0x0002EA79
	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
	}
}
