using System;
using UnityEngine;

// Token: 0x02000012 RID: 18
public class AudioCommand
{
	// Token: 0x0400001E RID: 30
	public SEType AudioType = SEType.Sound;

	// Token: 0x0400001F RID: 31
	public string AudioName;

	// Token: 0x04000020 RID: 32
	public AudioClip Clip;

	// Token: 0x04000021 RID: 33
	public float ProgressTime;

	// Token: 0x04000022 RID: 34
	public float FadeTimeOut;

	// Token: 0x04000023 RID: 35
	public float FadeTimeIn;

	// Token: 0x04000024 RID: 36
	public int Volume = 100;

	// Token: 0x04000025 RID: 37
	public float Pitch = 1f;

	// Token: 0x04000026 RID: 38
	public bool Loop;

	// Token: 0x04000027 RID: 39
	public bool CanSetPitchByGlobal;

	// Token: 0x04000028 RID: 40
	public Action<string> OnPlayFinish;

	// Token: 0x04000029 RID: 41
	public Action<AudioCommandOnPlayeUpdateParam> OnPlayUpdate;
}
