using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameData.Domains;
using GameData.Domains.Taiwu;
using GameData.Utilities;
using HarmonyLib;

namespace BetterSearchBackend
{
	// Token: 0x02000008 RID: 8
	internal sealed class ExpelVillagerGuardPatch : BaseBackendPatch
	{
		// Token: 0x06000027 RID: 39 RVA: 0x00002F48 File Offset: 0x00001148
		[NullableContext(1)]
		public override void OnModSettingUpdate(string modIdStr)
		{
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002F4A File Offset: 0x0000114A
		[HarmonyPrefix]
		[HarmonyPatch(typeof(TaiwuDomain), "ExpelVillager")]
		public static bool ExpelVillager_Prefix(int charId)
		{
			if (!ExpelVillagerGuardPatch.IsConfirmedNonMember(charId))
			{
				return true;
			}
			AdaptableLog.Info("BetterSearch blocked an expel of non-village-member character " + charId.ToString() + "; only Taiwu Village members can be expelled.");
			return false;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002F74 File Offset: 0x00001174
		private static bool IsConfirmedNonMember(int charId)
		{
			bool result;
			try
			{
				if (DomainManager.Taiwu.GetTaiwuVillageSettlementId() < 0)
				{
					result = false;
				}
				else
				{
					List<int> list = new List<int>();
					DomainManager.Taiwu.TaiwuVillage.GetMembers().GetAllMembers(list);
					result = !list.Contains(charId);
				}
			}
			catch (Exception ex)
			{
				AdaptableLog.Warning("BetterSearch village-membership check failed, allowing expel: " + ex.Message, false);
				result = false;
			}
			return result;
		}
	}
}
