using System;
using UnityEngine;

// Token: 0x02000404 RID: 1028
public class ProfessionMaskPropertyChange : MonoBehaviour
{
	// Token: 0x17000645 RID: 1605
	// (get) Token: 0x06003D4A RID: 15690 RVA: 0x001ED0B2 File Offset: 0x001EB2B2
	public CImage Icon
	{
		get
		{
			return this.icon;
		}
	}

	// Token: 0x17000646 RID: 1606
	// (get) Token: 0x06003D4B RID: 15691 RVA: 0x001ED0BA File Offset: 0x001EB2BA
	public CanvasGroup ValueRoot
	{
		get
		{
			return this.valueRoot;
		}
	}

	// Token: 0x04002C18 RID: 11288
	[SerializeField]
	private CImage icon;

	// Token: 0x04002C19 RID: 11289
	[SerializeField]
	private CanvasGroup valueRoot;
}
