using System;

namespace EventEditor
{
	// Token: 0x02000639 RID: 1593
	public class OperateCommand
	{
		// Token: 0x06004B50 RID: 19280 RVA: 0x00236A9E File Offset: 0x00234C9E
		public OperateCommand(string commandKey)
		{
			this.CommandKey = commandKey;
		}

		// Token: 0x04003452 RID: 13394
		public readonly string CommandKey;

		// Token: 0x04003453 RID: 13395
		public Action Do;

		// Token: 0x04003454 RID: 13396
		public Action Undo;
	}
}
