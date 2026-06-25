using System;

namespace FrameWork.CommandSystem
{
	// Token: 0x02001059 RID: 4185
	public interface ICollectable<in T1, in T2, in T3> : ICollectable
	{
		// Token: 0x0600BEA6 RID: 48806
		void Reset(T1 arg1, T2 arg2, T3 arg3);
	}
}
