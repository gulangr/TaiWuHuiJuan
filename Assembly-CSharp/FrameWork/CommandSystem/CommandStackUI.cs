using System;

namespace FrameWork.CommandSystem
{
	// Token: 0x0200106C RID: 4204
	public class CommandStackUI : CommandShowUI
	{
		// Token: 0x0600BF1A RID: 48922 RVA: 0x00567175 File Offset: 0x00565375
		protected override void DoShow()
		{
			UIManager.Instance.StackToUI(this.Element);
		}
	}
}
