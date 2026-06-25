using System;

namespace FrameWork.CommandSystem
{
	// Token: 0x0200105E RID: 4190
	public class CommandWaitUntil : BaseCommand, ICollectable<CmdGetDoneDelegate>, ICollectable
	{
		// Token: 0x0600BED5 RID: 48853 RVA: 0x00566A3F File Offset: 0x00564C3F
		public override bool Execute()
		{
			return true;
		}

		// Token: 0x1700157B RID: 5499
		// (get) Token: 0x0600BED6 RID: 48854 RVA: 0x00566A42 File Offset: 0x00564C42
		public override bool Done
		{
			get
			{
				CmdGetDoneDelegate funcGetDone = this.FuncGetDone;
				return funcGetDone == null || funcGetDone();
			}
		}

		// Token: 0x0600BED7 RID: 48855 RVA: 0x00566A56 File Offset: 0x00564C56
		public void Reset(CmdGetDoneDelegate arg)
		{
			this.FuncGetDone = arg;
		}

		// Token: 0x0600BED8 RID: 48856 RVA: 0x00566A5F File Offset: 0x00564C5F
		public override void Reset()
		{
			this.Reset(null);
		}

		// Token: 0x0600BED9 RID: 48857 RVA: 0x00566A69 File Offset: 0x00564C69
		public override void Collect()
		{
			this.FuncGetDone = null;
		}

		// Token: 0x0400925B RID: 37467
		protected CmdGetDoneDelegate FuncGetDone;
	}
}
