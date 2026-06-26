using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaiwuModdingLib.Core.Plugin;

namespace BetterSearchBackend
{
	// Token: 0x02000006 RID: 6
	[PluginConfig("BetterSearchBackend", "kesar", "1.0.0")]
	public class BetterSearchBackend : TaiwuRemakePlugin
	{
		// Token: 0x06000009 RID: 9 RVA: 0x000021C0 File Offset: 0x000003C0
		public override void Initialize()
		{
			this._harmony = Harmony.CreateAndPatchAll(typeof(BetterSearchBackend), null);
			BetterSearchBackend.AllPatchList.ForEach(delegate(BaseBackendPatch patch)
			{
				this._harmony.PatchAll(patch.GetType());
			});
			BetterSearchBackend.AllPatchList.ForEach(delegate(BaseBackendPatch patch)
			{
				patch.Initialize(this._harmony, base.ModIdStr);
			});
		}

		// Token: 0x0600000A RID: 10 RVA: 0x0000220F File Offset: 0x0000040F
		public override void OnModSettingUpdate()
		{
			BetterSearchBackend.AllPatchList.ForEach(delegate(BaseBackendPatch patch)
			{
				patch.OnModSettingUpdate(base.ModIdStr);
			});
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002228 File Offset: 0x00000428
		public override void Dispose()
		{
			BetterSearchBackend.AllPatchList.ForEach(delegate(BaseBackendPatch patch)
			{
				patch.Dispose();
			});
			Harmony harmony = this._harmony;
			if (harmony != null)
			{
				harmony.UnpatchSelf();
			}
			this._harmony = null;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002276 File Offset: 0x00000476
		public override void OnEnterNewWorld()
		{
			BetterSearchBackend.AllPatchList.ForEach(delegate(BaseBackendPatch patch)
			{
				patch.OnEnterNewWorld();
			});
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000022A1 File Offset: 0x000004A1
		public override void OnLoadedArchiveData()
		{
			BetterSearchBackend.AllPatchList.ForEach(delegate(BaseBackendPatch patch)
			{
				patch.OnLoadedArchiveData();
			});
		}

		// Token: 0x0400000D RID: 13
		[Nullable(1)]
		private static readonly List<BaseBackendPatch> AllPatchList = new List<BaseBackendPatch>
		{
			new CharacterSearchPatch(),
			new VillagerDataEarlyGuardPatch(),
			new ExpelVillagerGuardPatch()
		};

		// Token: 0x0400000E RID: 14
		[Nullable(2)]
		private Harmony _harmony;
	}
}
