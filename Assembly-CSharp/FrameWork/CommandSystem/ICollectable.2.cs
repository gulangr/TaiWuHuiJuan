using System;

namespace FrameWork.CommandSystem
{
	// Token: 0x02001057 RID: 4183
	public interface ICollectable<in T> : ICollectable
	{
		// Token: 0x0600BEA4 RID: 48804
		void Reset(T arg);
	}
}
