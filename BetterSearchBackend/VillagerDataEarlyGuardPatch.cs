using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameData.Domains;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu;
using GameData.Utilities;
using HarmonyLib;

namespace BetterSearchBackend
{
	// Token: 0x02000009 RID: 9
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class VillagerDataEarlyGuardPatch : BaseBackendPatch
	{
		// Token: 0x0600002B RID: 43 RVA: 0x00002FF0 File Offset: 0x000011F0
		public override void OnModSettingUpdate(string modIdStr)
		{
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002FF4 File Offset: 0x000011F4
		private static bool VillageMissing()
		{
			bool result;
			try
			{
				result = (DomainManager.Taiwu.GetTaiwuVillageSettlementId() < 0);
			}
			catch (Exception ex)
			{
				AdaptableLog.Warning("BetterSearch village-readiness check failed: " + ex.Message, false);
				result = false;
			}
			return result;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00003040 File Offset: 0x00001240
		[HarmonyPrefix]
		[HarmonyPatch(typeof(CharacterDomain), "GetVillagerCharDisplayDataList")]
		public static bool GetVillagerCharDisplayDataList_Prefix(ref List<VillagerCharDisplayData> __result)
		{
			if (!VillagerDataEarlyGuardPatch.VillageMissing())
			{
				return true;
			}
			__result = new List<VillagerCharDisplayData>();
			return false;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00003053 File Offset: 0x00001253
		[HarmonyPrefix]
		[HarmonyPatch(typeof(TaiwuDomain), "RequestTaiwuResourceDisplayData")]
		public static bool RequestTaiwuResourceDisplayData_Prefix(ref TaiwuResourceDisplayData __result)
		{
			if (!VillagerDataEarlyGuardPatch.VillageMissing())
			{
				return true;
			}
			__result = new TaiwuResourceDisplayData();
			return false;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00003066 File Offset: 0x00001266
		[HarmonyPrefix]
		[HarmonyPatch(typeof(TaiwuDomain), "GetTotalVillagerMaintenance")]
		public static bool GetTotalVillagerMaintenance_Prefix(ref ResourceInts __result)
		{
			if (!VillagerDataEarlyGuardPatch.VillageMissing())
			{
				return true;
			}
			__result = default(ResourceInts);
			return false;
		}
	}
}
