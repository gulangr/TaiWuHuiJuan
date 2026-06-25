using System;
using System.Threading;
using UnityEngine;

// Token: 0x02000041 RID: 65
internal class WaitForThreadedTask : CustomYieldInstruction
{
	// Token: 0x0600022B RID: 555 RVA: 0x0000D0A0 File Offset: 0x0000B2A0
	public WaitForThreadedTask(Action task)
	{
		WaitForThreadedTask <>4__this = this;
		this._isRunning = true;
		new Thread(delegate()
		{
			task();
			<>4__this._isRunning = false;
		}).Start();
	}

	// Token: 0x1700004B RID: 75
	// (get) Token: 0x0600022C RID: 556 RVA: 0x0000D0E8 File Offset: 0x0000B2E8
	public override bool keepWaiting
	{
		get
		{
			return this._isRunning;
		}
	}

	// Token: 0x04000110 RID: 272
	private bool _isRunning;
}
