using System;

namespace FrameWork.CommandSystem
{
	// Token: 0x0200106D RID: 4205
	public class CommandStackUIWithArgs : CommandShowUIWithArgs
	{
		// Token: 0x0600BF1C RID: 48924 RVA: 0x00567192 File Offset: 0x00565392
		protected override void DoShow()
		{
			this.Element.SetOnInitArgs(this.ArgBox);
			this.ArgBox = null;
			UIManager.Instance.StackToUI(this.Element);
		}
	}
}
