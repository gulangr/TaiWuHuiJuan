using System;

namespace FrameWork.CommandSystem
{
	// Token: 0x02001058 RID: 4184
	public interface ICollectable<in T1, in T2> : ICollectable
	{
		// Token: 0x0600BEA5 RID: 48805
		void Reset(T1 arg1, T2 arg2);
	}
}
