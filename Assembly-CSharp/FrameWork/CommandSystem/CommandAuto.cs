using System;

namespace FrameWork.CommandSystem
{
	// Token: 0x0200105D RID: 4189
	public class CommandAuto : BaseCommand, ICollectable<CmdExecuteDelegate, CmdGetDoneDelegate>, ICollectable
	{
		// Token: 0x0600BECF RID: 48847 RVA: 0x005669D0 File Offset: 0x00564BD0
		public override bool Execute()
		{
			CmdExecuteDelegate funcExecute = this.FuncExecute;
			return funcExecute != null && funcExecute();
		}

		// Token: 0x1700157A RID: 5498
		// (get) Token: 0x0600BED0 RID: 48848 RVA: 0x005669F4 File Offset: 0x00564BF4
		public override bool Done
		{
			get
			{
				CmdGetDoneDelegate funcGetDone = this.FuncGetDone;
				return funcGetDone == null || funcGetDone();
			}
		}

		// Token: 0x0600BED1 RID: 48849 RVA: 0x00566A08 File Offset: 0x00564C08
		public void Reset(CmdExecuteDelegate arg1, CmdGetDoneDelegate arg2)
		{
			this.FuncExecute = arg1;
			this.FuncGetDone = arg2;
		}

		// Token: 0x0600BED2 RID: 48850 RVA: 0x00566A19 File Offset: 0x00564C19
		public override void Reset()
		{
			this.Reset(null, null);
		}

		// Token: 0x0600BED3 RID: 48851 RVA: 0x00566A25 File Offset: 0x00564C25
		public override void Collect()
		{
			this.FuncExecute = null;
			this.FuncGetDone = null;
		}

		// Token: 0x04009259 RID: 37465
		protected CmdExecuteDelegate FuncExecute;

		// Token: 0x0400925A RID: 37466
		protected CmdGetDoneDelegate FuncGetDone;
	}
}
