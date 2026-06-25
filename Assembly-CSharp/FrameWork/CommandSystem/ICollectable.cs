using System;

namespace FrameWork.CommandSystem
{
	// Token: 0x02001056 RID: 4182
	public interface ICollectable
	{
		// Token: 0x17001577 RID: 5495
		// (get) Token: 0x0600BEA0 RID: 48800
		// (set) Token: 0x0600BEA1 RID: 48801
		bool Pooled { get; set; }

		// Token: 0x0600BEA2 RID: 48802
		void Reset();

		// Token: 0x0600BEA3 RID: 48803
		void Collect();
	}
}
