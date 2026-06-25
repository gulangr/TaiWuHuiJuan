using System;

namespace FrameWork.CommandSystem
{
	// Token: 0x02001054 RID: 4180
	public abstract class BaseCommand : ICommand, ICollectable
	{
		// Token: 0x17001574 RID: 5492
		// (get) Token: 0x0600BE97 RID: 48791 RVA: 0x005660BC File Offset: 0x005642BC
		// (set) Token: 0x0600BE98 RID: 48792 RVA: 0x005660C4 File Offset: 0x005642C4
		bool ICollectable.Pooled { get; set; }

		// Token: 0x0600BE99 RID: 48793
		public abstract bool Execute();

		// Token: 0x17001575 RID: 5493
		// (get) Token: 0x0600BE9A RID: 48794
		public abstract bool Done { get; }

		// Token: 0x0600BE9B RID: 48795
		public abstract void Reset();

		// Token: 0x0600BE9C RID: 48796
		public abstract void Collect();
	}
}
