using System;
using HarmonyLib;

namespace BetterSearchFrontend
{
	// Token: 0x02000007 RID: 7
	internal abstract class BaseFrontPatch
	{
		// Token: 0x06000005 RID: 5
		public abstract void OnModSettingUpdate(string modIdStr);

		// Token: 0x06000006 RID: 6 RVA: 0x000021C8 File Offset: 0x000003C8
		public virtual void Initialize(Harmony harmony, string modIdStr)
		{
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000021CA File Offset: 0x000003CA
		public virtual void Dispose()
		{
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000021CC File Offset: 0x000003CC
		public virtual void OnEnterNewWorld()
		{
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000021CE File Offset: 0x000003CE
		public virtual void OnLoadedArchiveData()
		{
		}
	}
}
