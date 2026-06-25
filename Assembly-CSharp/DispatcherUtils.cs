using System;
using FrameWork;

// Token: 0x02000123 RID: 291
public static class DispatcherUtils
{
	// Token: 0x06000B9B RID: 2971 RVA: 0x0004B2C4 File Offset: 0x000494C4
	public static DispatcherInstance RegisterDispatcher()
	{
		return EasyPool.Get<DispatcherInstance>();
	}

	// Token: 0x06000B9C RID: 2972 RVA: 0x0004B2DC File Offset: 0x000494DC
	public static void UnregisterDispatcher(DispatcherInstance instance)
	{
		bool flag = instance == null;
		if (!flag)
		{
			instance.ClearAsyncMethodCalls();
			EasyPool.Free<DispatcherInstance>(instance);
		}
	}
}
