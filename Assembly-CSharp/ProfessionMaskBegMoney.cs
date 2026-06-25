using System;
using UnityEngine;

// Token: 0x02000402 RID: 1026
public class ProfessionMaskBegMoney : MonoBehaviour
{
	// Token: 0x1700063A RID: 1594
	// (get) Token: 0x06003D38 RID: 15672 RVA: 0x001ECF65 File Offset: 0x001EB165
	public CanvasGroup ValueRoot
	{
		get
		{
			return this.valueRoot;
		}
	}

	// Token: 0x04002C0C RID: 11276
	[SerializeField]
	private CanvasGroup valueRoot;
}
