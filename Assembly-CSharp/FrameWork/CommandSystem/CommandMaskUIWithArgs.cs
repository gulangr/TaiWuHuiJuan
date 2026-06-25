using System;

namespace FrameWork.CommandSystem
{
	// Token: 0x0200106F RID: 4207
	public class CommandMaskUIWithArgs : CommandShowUIWithArgs
	{
		// Token: 0x0600BF20 RID: 48928 RVA: 0x005671E5 File Offset: 0x005653E5
		protected override void DoShow()
		{
			this.Element.SetOnInitArgs(this.ArgBox);
			this.ArgBox = null;
			UIManager.Instance.MaskUI(this.Element);
		}
	}
}
