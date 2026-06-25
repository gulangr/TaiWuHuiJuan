using System;
using UnityEngine;

// Token: 0x02000405 RID: 1029
public class ProfessionMaskRebirthCricket : MonoBehaviour
{
	// Token: 0x17000647 RID: 1607
	// (get) Token: 0x06003D4D RID: 15693 RVA: 0x001ED0CB File Offset: 0x001EB2CB
	public ParticleSystem Effect
	{
		get
		{
			return this.effect;
		}
	}

	// Token: 0x17000648 RID: 1608
	// (get) Token: 0x06003D4E RID: 15694 RVA: 0x001ED0D3 File Offset: 0x001EB2D3
	public CricketView CricketView
	{
		get
		{
			return this.cricketView;
		}
	}

	// Token: 0x04002C1A RID: 11290
	[SerializeField]
	private ParticleSystem effect;

	// Token: 0x04002C1B RID: 11291
	[SerializeField]
	private CricketView cricketView;
}
