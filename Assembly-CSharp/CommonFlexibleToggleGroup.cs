using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000343 RID: 835
[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class CommonFlexibleToggleGroup : UIBehaviour
{
	// Token: 0x060030E7 RID: 12519 RVA: 0x0017FF3A File Offset: 0x0017E13A
	protected override void Awake()
	{
		base.Awake();
		this._selfTrans = base.GetComponent<RectTransform>();
	}

	// Token: 0x060030E8 RID: 12520 RVA: 0x0017FF50 File Offset: 0x0017E150
	protected override void OnEnable()
	{
		this.OnRectTransformDimensionsChange();
	}

	// Token: 0x060030E9 RID: 12521 RVA: 0x0017FF5C File Offset: 0x0017E15C
	protected override void OnRectTransformDimensionsChange()
	{
		bool flag = this._selfTrans != null && this.decorate != null;
		if (flag)
		{
			this.decorate.SetWidth((float)((int)((this._selfTrans.rect.width - 20f) / 195f) * 195));
		}
	}

	// Token: 0x040023C9 RID: 9161
	private const int DecoWidth = 195;

	// Token: 0x040023CA RID: 9162
	private const int DecoOffset = 20;

	// Token: 0x040023CB RID: 9163
	public RectTransform decorate;

	// Token: 0x040023CC RID: 9164
	private RectTransform _selfTrans;
}
