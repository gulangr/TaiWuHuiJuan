using System;

namespace FrameWork.CommandSystem
{
	// Token: 0x0200106E RID: 4206
	public class CommandMaskUI : CommandShowUI
	{
		// Token: 0x0600BF1E RID: 48926 RVA: 0x005671C8 File Offset: 0x005653C8
		protected override void DoShow()
		{
			UIManager.Instance.MaskUI(this.Element);
		}
	}
}
