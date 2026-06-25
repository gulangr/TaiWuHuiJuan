using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000215 RID: 533
public class LeftMouseSkipAnimNotice : Refers
{
	// Token: 0x17000362 RID: 866
	// (get) Token: 0x0600223D RID: 8765 RVA: 0x000FDFD9 File Offset: 0x000FC1D9
	// (set) Token: 0x0600223E RID: 8766 RVA: 0x000FDFE1 File Offset: 0x000FC1E1
	public bool IsSkipEnabled { get; private set; } = false;

	// Token: 0x17000363 RID: 867
	// (get) Token: 0x0600223F RID: 8767 RVA: 0x000FDFEA File Offset: 0x000FC1EA
	// (set) Token: 0x06002240 RID: 8768 RVA: 0x000FDFF2 File Offset: 0x000FC1F2
	public bool IsAnimLoopEnable { get; set; } = true;

	// Token: 0x06002241 RID: 8769 RVA: 0x000FDFFB File Offset: 0x000FC1FB
	private void OnEnable()
	{
		base.StartCoroutine(this.SetSkipTime());
	}

	// Token: 0x06002242 RID: 8770 RVA: 0x000FE00B File Offset: 0x000FC20B
	private IEnumerator SetSkipTime()
	{
		this.IsSkipEnabled = false;
		yield return this._disableSkipWait;
		this.IsSkipEnabled = true;
		yield break;
	}

	// Token: 0x04001A5F RID: 6751
	private const float DisableSkipTime = 0.3f;

	// Token: 0x04001A60 RID: 6752
	private readonly WaitForSeconds _disableSkipWait = new WaitForSeconds(0.3f);
}
