using System;
using System.Collections.Generic;
using HarmonyLib;
using TaiwuModdingLib.Core.Plugin;

namespace BetterSearchFrontend
{
	// Token: 0x02000008 RID: 8
	[PluginConfig("BetterSearchFrontend", "kesar", "1.0.0")]
	public class BetterSearchFrontend : TaiwuRemakePlugin
	{
		// Token: 0x0600000B RID: 11 RVA: 0x000021D8 File Offset: 0x000003D8
		public override void Initialize()
		{
			this._harmony = Harmony.CreateAndPatchAll(typeof(BetterSearchFrontend), null);
			this._allPatchList.ForEach(delegate(BaseFrontPatch patch)
			{
				this._harmony.PatchAll(patch.GetType());
			});
			this._allPatchList.ForEach(delegate(BaseFrontPatch patch)
			{
				patch.Initialize(this._harmony, base.ModIdStr);
			});
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002229 File Offset: 0x00000429
		public override void OnModSettingUpdate()
		{
			this._allPatchList.ForEach(delegate(BaseFrontPatch patch)
			{
				patch.OnModSettingUpdate(base.ModIdStr);
			});
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002244 File Offset: 0x00000444
		public override void Dispose()
		{
			this._allPatchList.ForEach(delegate(BaseFrontPatch patch)
			{
				patch.Dispose();
			});
			if (this._harmony != null)
			{
				this._harmony.UnpatchSelf();
			}
			this._harmony = null;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002295 File Offset: 0x00000495
		public override void OnEnterNewWorld()
		{
			this._allPatchList.ForEach(delegate(BaseFrontPatch patch)
			{
				patch.OnEnterNewWorld();
			});
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000022C1 File Offset: 0x000004C1
		public override void OnLoadedArchiveData()
		{
			this._allPatchList.ForEach(delegate(BaseFrontPatch patch)
			{
				patch.OnLoadedArchiveData();
			});
		}

		// Token: 0x0400000E RID: 14
		private readonly List<BaseFrontPatch> _allPatchList = new List<BaseFrontPatch>
		{
			new TaiwuVillagersSearchScopePatch(),
			new ViewMapBlockCharListItemSearchPatch(),
			new VillagerPanelEarlyOpenPatch()
		};

		// Token: 0x0400000F RID: 15
		private Harmony _harmony;
	}
}
