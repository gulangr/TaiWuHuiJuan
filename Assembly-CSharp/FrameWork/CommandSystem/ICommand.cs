using System;

namespace FrameWork.CommandSystem
{
	// Token: 0x02001055 RID: 4181
	public interface ICommand
	{
		// Token: 0x0600BE9E RID: 48798
		bool Execute();

		// Token: 0x17001576 RID: 5494
		// (get) Token: 0x0600BE9F RID: 48799
		bool Done { get; }
	}
}
