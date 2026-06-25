using System;
using UnityEngine;

// Token: 0x02000013 RID: 19
public struct AudioCommandOnPlayeUpdateParam
{
	// Token: 0x0600004E RID: 78 RVA: 0x00002E24 File Offset: 0x00001024
	public AudioCommandOnPlayeUpdateParam(AudioSource player, float eclapsedTime)
	{
		this.player = player;
		this.eclapsedTime = eclapsedTime;
	}

	// Token: 0x0400002A RID: 42
	public AudioSource player;

	// Token: 0x0400002B RID: 43
	public float eclapsedTime;
}
