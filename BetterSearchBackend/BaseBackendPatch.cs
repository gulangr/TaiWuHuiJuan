using System;
using System.Runtime.CompilerServices;
using HarmonyLib;

namespace BetterSearchBackend
{
	// Token: 0x02000005 RID: 5
	[NullableContext(1)]
	[Nullable(0)]
	internal abstract class BaseBackendPatch
	{
		// Token: 0x06000003 RID: 3
		public abstract void OnModSettingUpdate(string modIdStr);

		// Token: 0x06000004 RID: 4 RVA: 0x000021B0 File Offset: 0x000003B0
		public virtual void Initialize(Harmony harmony, string modIdStr)
		{
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000021B2 File Offset: 0x000003B2
		public virtual void Dispose()
		{
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000021B4 File Offset: 0x000003B4
		public virtual void OnEnterNewWorld()
		{
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000021B6 File Offset: 0x000003B6
		public virtual void OnLoadedArchiveData()
		{
		}
	}
}
