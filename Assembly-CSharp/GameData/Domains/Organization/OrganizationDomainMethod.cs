using System;
using System.Collections.Generic;
using GameData.Domains.Global.Inscription;
using GameData.GameDataBridge;

namespace GameData.Domains.Organization
{
	// Token: 0x02000FC1 RID: 4033
	public static class OrganizationDomainMethod
	{
		// Token: 0x020025FD RID: 9725
		public static class Call
		{
			// Token: 0x06011100 RID: 69888 RVA: 0x00677B60 File Offset: 0x00675D60
			public static void GetDisplayData(int listenerId, short settlementId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 3, 0, settlementId);
			}

			// Token: 0x06011101 RID: 69889 RVA: 0x00677B6D File Offset: 0x00675D6D
			public static void GetSettlementNameRelatedData(int listenerId, List<short> settlementIds)
			{
				GameDataBridge.AddMethodCall<List<short>>(listenerId, 3, 1, settlementIds);
			}

			// Token: 0x06011102 RID: 69890 RVA: 0x00677B7A File Offset: 0x00675D7A
			public static void GetSettlementMembers(int listenerId, short settlementId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 3, 2, settlementId);
			}

			// Token: 0x06011103 RID: 69891 RVA: 0x00677B87 File Offset: 0x00675D87
			public static void GetOrganizationCombatSkillsDisplayData(int listenerId, sbyte organizationTemplateId)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 3, 3, organizationTemplateId);
			}

			// Token: 0x06011104 RID: 69892 RVA: 0x00677B94 File Offset: 0x00675D94
			public static void GetSectPreparationForMartialArtTournament(int listenerId, sbyte orgTemplateId)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 3, 4, orgTemplateId);
			}

			// Token: 0x06011105 RID: 69893 RVA: 0x00677BA1 File Offset: 0x00675DA1
			public static void GetMartialArtTournamentCurrentHostSettlementId(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 3, 5);
			}

			// Token: 0x06011106 RID: 69894 RVA: 0x00677BAD File Offset: 0x00675DAD
			public static void GmCmd_SetAllSettlementInformationVisited()
			{
				GameDataBridge.AddMethodCall(-1, 3, 6);
			}

			// Token: 0x06011107 RID: 69895 RVA: 0x00677BB9 File Offset: 0x00675DB9
			public static void GmCmd_GetAllFactionMembers(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 3, 7);
			}

			// Token: 0x06011108 RID: 69896 RVA: 0x00677BC5 File Offset: 0x00675DC5
			public static void GetSettlementIdByAreaIdAndBlockId(int listenerId, short areaId, short blockId)
			{
				GameDataBridge.AddMethodCall<short, short>(listenerId, 3, 8, areaId, blockId);
			}

			// Token: 0x06011109 RID: 69897 RVA: 0x00677BD3 File Offset: 0x00675DD3
			public static void GetCultureByAreaIdAndBlockId(int listenerId, short areaId, short blockId)
			{
				GameDataBridge.AddMethodCall<short, short>(listenerId, 3, 9, areaId, blockId);
			}

			// Token: 0x0601110A RID: 69898 RVA: 0x00677BE2 File Offset: 0x00675DE2
			public static void CalcApprovingRateEffectAuthorityGain(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 3, 10);
			}

			// Token: 0x0601110B RID: 69899 RVA: 0x00677BEF File Offset: 0x00675DEF
			public static void GetSettlementTreasuryDisplayData(int listenerId, short settlementId, sbyte layerIndex)
			{
				GameDataBridge.AddMethodCall<short, sbyte>(listenerId, 3, 11, settlementId, layerIndex);
			}

			// Token: 0x0601110C RID: 69900 RVA: 0x00677BFE File Offset: 0x00675DFE
			public static void GetSettlementTreasuryRecordCollection(int listenerId, short settlementId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 3, 12, settlementId);
			}

			// Token: 0x0601110D RID: 69901 RVA: 0x00677C0C File Offset: 0x00675E0C
			public static void SetInscribedCharactersForCreation(List<InscribedCharacterKey> inscribedCharList)
			{
				GameDataBridge.AddMethodCall<List<InscribedCharacterKey>>(-1, 3, 13, inscribedCharList);
			}

			// Token: 0x0601110E RID: 69902 RVA: 0x00677C1A File Offset: 0x00675E1A
			public static void SetInscribedCharactersForCreation(List<InscribedCharacterKey> inscribedCharList, List<short> ages)
			{
				GameDataBridge.AddMethodCall<List<InscribedCharacterKey>, List<short>>(-1, 3, 13, inscribedCharList, ages);
			}

			// Token: 0x0601110F RID: 69903 RVA: 0x00677C29 File Offset: 0x00675E29
			public static void GmCmd_UpdateSettlementTreasury(short settlementId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 3, 14, settlementId);
			}

			// Token: 0x06011110 RID: 69904 RVA: 0x00677C37 File Offset: 0x00675E37
			public static void GmCmd_ClearSettlementTreasuryAlertTime(short settlementId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 3, 15, settlementId);
			}

			// Token: 0x06011111 RID: 69905 RVA: 0x00677C45 File Offset: 0x00675E45
			public static void GmCmd_ClearSettlementTreasuryItemAndResource(short settlementId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 3, 16, settlementId);
			}

			// Token: 0x06011112 RID: 69906 RVA: 0x00677C53 File Offset: 0x00675E53
			public static void GmCmd_ForceUpdateTreasuryGuards(short settlementId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 3, 17, settlementId);
			}

			// Token: 0x06011113 RID: 69907 RVA: 0x00677C61 File Offset: 0x00675E61
			public static void AddSectBounty(sbyte orgTemplateId, int charId, sbyte punishmentSeverity, short punishmentType, int duration)
			{
				GameDataBridge.AddMethodCall<sbyte, int, sbyte, short, int>(-1, 3, 18, orgTemplateId, charId, punishmentSeverity, punishmentType, duration);
			}

			// Token: 0x06011114 RID: 69908 RVA: 0x00677C74 File Offset: 0x00675E74
			public static void AddSectPrisoner(sbyte orgTemplateId, int charId, sbyte punishmentSeverity, short punishmentType, int duration)
			{
				GameDataBridge.AddMethodCall<sbyte, int, sbyte, short, int>(-1, 3, 19, orgTemplateId, charId, punishmentSeverity, punishmentType, duration);
			}

			// Token: 0x06011115 RID: 69909 RVA: 0x00677C87 File Offset: 0x00675E87
			public static void GetSettlementPrisonDisplayData(int listenerId, short settlementId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 3, 20, settlementId);
			}

			// Token: 0x06011116 RID: 69910 RVA: 0x00677C95 File Offset: 0x00675E95
			public static void GetSettlementBountyDisplayData(int listenerId, short settlementId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 3, 21, settlementId);
			}

			// Token: 0x06011117 RID: 69911 RVA: 0x00677CA3 File Offset: 0x00675EA3
			public static void GetSettlementPrisonRecordCollection(int listenerId, short settlementId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 3, 22, settlementId);
			}

			// Token: 0x06011118 RID: 69912 RVA: 0x00677CB1 File Offset: 0x00675EB1
			public static void GmCmd_ForceUpdateInfluencePower(short settlementId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 3, 23, settlementId);
			}

			// Token: 0x06011119 RID: 69913 RVA: 0x00677CBF File Offset: 0x00675EBF
			public static void GetBountyCharacterDisplayDataFromCharacterList(int listenerId, List<int> characterIds)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 3, 24, characterIds);
			}

			// Token: 0x0601111A RID: 69914 RVA: 0x00677CCD File Offset: 0x00675ECD
			public static void ForceUpdateTaiwuVillager()
			{
				GameDataBridge.AddMethodCall(-1, 3, 25);
			}

			// Token: 0x0601111B RID: 69915 RVA: 0x00677CDA File Offset: 0x00675EDA
			public static void IsTaiwuSectFugitive(int listenerId, sbyte orgTemplateId)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 3, 26, orgTemplateId);
			}

			// Token: 0x0601111C RID: 69916 RVA: 0x00677CE8 File Offset: 0x00675EE8
			public static void GetOrganizationTemplateIdOfTaiwuLocation(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 3, 27);
			}

			// Token: 0x0601111D RID: 69917 RVA: 0x00677CF5 File Offset: 0x00675EF5
			public static void GetLastSettlementTreasuryOperationData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 3, 28);
			}

			// Token: 0x0601111E RID: 69918 RVA: 0x00677D02 File Offset: 0x00675F02
			public static void GmCmd_GetSettlementPrisoner(int listenerId, int prisonType)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 3, 29, prisonType);
			}

			// Token: 0x0601111F RID: 69919 RVA: 0x00677D10 File Offset: 0x00675F10
			public static void CheckSettlementGuardFavorabilityType(int listenerId, short settlementId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 3, 30, settlementId);
			}

			// Token: 0x06011120 RID: 69920 RVA: 0x00677D1E File Offset: 0x00675F1E
			public static void GmCmd_SetAllSettlementMemberApprovedTaiwu(sbyte orgTemplateId, bool approvedTaiwu)
			{
				GameDataBridge.AddMethodCall<sbyte, bool>(-1, 3, 31, orgTemplateId, approvedTaiwu);
			}

			// Token: 0x06011121 RID: 69921 RVA: 0x00677D2D File Offset: 0x00675F2D
			public static void GetSectFunctionStatus(int listenerId, sbyte orgTemplateId, SectFunctionStatuses.SectFunctionStatusType statusType)
			{
				GameDataBridge.AddMethodCall<sbyte, SectFunctionStatuses.SectFunctionStatusType>(listenerId, 3, 32, orgTemplateId, statusType);
			}

			// Token: 0x06011122 RID: 69922 RVA: 0x00677D3C File Offset: 0x00675F3C
			public static void GmCmd_SetSectFunctionStatus(sbyte orgTemplateId, SectFunctionStatuses.SectFunctionStatusType statusType, bool value)
			{
				GameDataBridge.AddMethodCall<sbyte, SectFunctionStatuses.SectFunctionStatusType, bool>(-1, 3, 33, orgTemplateId, statusType, value);
			}

			// Token: 0x06011123 RID: 69923 RVA: 0x00677D4C File Offset: 0x00675F4C
			public static void UpdateCityPunishmentSeverityCustomizeData(sbyte stateTemplateId, bool isSect, short punishmentTypeTemplateId, sbyte customizedPunishmentSeverityTemplateId)
			{
				GameDataBridge.AddMethodCall<sbyte, bool, short, sbyte>(-1, 3, 34, stateTemplateId, isSect, punishmentTypeTemplateId, customizedPunishmentSeverityTemplateId);
			}

			// Token: 0x06011124 RID: 69924 RVA: 0x00677D5D File Offset: 0x00675F5D
			public static void GetCustomizePunishmentSeverityCost(int listenerId, sbyte stateTemplateId, bool isSect)
			{
				GameDataBridge.AddMethodCall<sbyte, bool>(listenerId, 3, 35, stateTemplateId, isSect);
			}

			// Token: 0x06011125 RID: 69925 RVA: 0x00677D6C File Offset: 0x00675F6C
			public static void WillCustomizePunishmentBreakWithoutVillagerHead(int listenerId, int villagerHeadCharId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 3, 36, villagerHeadCharId);
			}

			// Token: 0x06011126 RID: 69926 RVA: 0x00677D7A File Offset: 0x00675F7A
			public static void GetSettlementPopulationDisplayData(int listenerId, short settlementId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 3, 37, settlementId);
			}

			// Token: 0x06011127 RID: 69927 RVA: 0x00677D88 File Offset: 0x00675F88
			public static void GetReversedSettlementPrisonRecordCollection(int listenerId, short settlementId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 3, 38, settlementId);
			}

			// Token: 0x06011128 RID: 69928 RVA: 0x00677D96 File Offset: 0x00675F96
			public static void GetReversedSettlementTreasuryRecordCollection(int listenerId, short settlementId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 3, 39, settlementId);
			}

			// Token: 0x06011129 RID: 69929 RVA: 0x00677DA4 File Offset: 0x00675FA4
			public static void GetSettlementApproveTaiwuMembers(int listenerId, short settlementId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 3, 40, settlementId);
			}
		}

		// Token: 0x020025FE RID: 9726
		public static class AsyncCall
		{
			// Token: 0x0601112A RID: 69930 RVA: 0x00677DB4 File Offset: 0x00675FB4
			public static void GetDisplayData(IAsyncMethodRequestHandler requestHandler, short settlementId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(3, 0, settlementId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601112B RID: 69931 RVA: 0x00677DE0 File Offset: 0x00675FE0
			public static void GetSettlementNameRelatedData(IAsyncMethodRequestHandler requestHandler, List<short> settlementIds, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<short>>(3, 1, settlementIds, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601112C RID: 69932 RVA: 0x00677E0C File Offset: 0x0067600C
			public static void GetSettlementMembers(IAsyncMethodRequestHandler requestHandler, short settlementId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(3, 2, settlementId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601112D RID: 69933 RVA: 0x00677E38 File Offset: 0x00676038
			public static void GetOrganizationCombatSkillsDisplayData(IAsyncMethodRequestHandler requestHandler, sbyte organizationTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(3, 3, organizationTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601112E RID: 69934 RVA: 0x00677E64 File Offset: 0x00676064
			public static void GetSectPreparationForMartialArtTournament(IAsyncMethodRequestHandler requestHandler, sbyte orgTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(3, 4, orgTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601112F RID: 69935 RVA: 0x00677E90 File Offset: 0x00676090
			public static void GetMartialArtTournamentCurrentHostSettlementId(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(3, 5, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011130 RID: 69936 RVA: 0x00677EB9 File Offset: 0x006760B9
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use OrganizationDomainMethod.Call.GmCmd_SetAllSettlementInformationVisited instead.", true)]
			public static void GmCmd_SetAllSettlementInformationVisited(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011131 RID: 69937 RVA: 0x00677EC4 File Offset: 0x006760C4
			public static void GmCmd_GetAllFactionMembers(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(3, 7, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011132 RID: 69938 RVA: 0x00677EF0 File Offset: 0x006760F0
			public static void GetSettlementIdByAreaIdAndBlockId(IAsyncMethodRequestHandler requestHandler, short areaId, short blockId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, short>(3, 8, areaId, blockId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011133 RID: 69939 RVA: 0x00677F1C File Offset: 0x0067611C
			public static void GetCultureByAreaIdAndBlockId(IAsyncMethodRequestHandler requestHandler, short areaId, short blockId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, short>(3, 9, areaId, blockId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011134 RID: 69940 RVA: 0x00677F48 File Offset: 0x00676148
			public static void CalcApprovingRateEffectAuthorityGain(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(3, 10, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011135 RID: 69941 RVA: 0x00677F74 File Offset: 0x00676174
			public static void GetSettlementTreasuryDisplayData(IAsyncMethodRequestHandler requestHandler, short settlementId, sbyte layerIndex, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, sbyte>(3, 11, settlementId, layerIndex, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011136 RID: 69942 RVA: 0x00677FA0 File Offset: 0x006761A0
			public static void GetSettlementTreasuryRecordCollection(IAsyncMethodRequestHandler requestHandler, short settlementId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(3, 12, settlementId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011137 RID: 69943 RVA: 0x00677FCB File Offset: 0x006761CB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use OrganizationDomainMethod.Call.SetInscribedCharactersForCreation instead.", true)]
			public static void SetInscribedCharactersForCreation(IAsyncMethodRequestHandler requestHandler, List<InscribedCharacterKey> inscribedCharList, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011138 RID: 69944 RVA: 0x00677FD3 File Offset: 0x006761D3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use OrganizationDomainMethod.Call.SetInscribedCharactersForCreation instead.", true)]
			public static void SetInscribedCharactersForCreation(IAsyncMethodRequestHandler requestHandler, List<InscribedCharacterKey> inscribedCharList, List<short> ages, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011139 RID: 69945 RVA: 0x00677FDB File Offset: 0x006761DB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use OrganizationDomainMethod.Call.GmCmd_UpdateSettlementTreasury instead.", true)]
			public static void GmCmd_UpdateSettlementTreasury(IAsyncMethodRequestHandler requestHandler, short settlementId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601113A RID: 69946 RVA: 0x00677FE3 File Offset: 0x006761E3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use OrganizationDomainMethod.Call.GmCmd_ClearSettlementTreasuryAlertTime instead.", true)]
			public static void GmCmd_ClearSettlementTreasuryAlertTime(IAsyncMethodRequestHandler requestHandler, short settlementId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601113B RID: 69947 RVA: 0x00677FEB File Offset: 0x006761EB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use OrganizationDomainMethod.Call.GmCmd_ClearSettlementTreasuryItemAndResource instead.", true)]
			public static void GmCmd_ClearSettlementTreasuryItemAndResource(IAsyncMethodRequestHandler requestHandler, short settlementId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601113C RID: 69948 RVA: 0x00677FF3 File Offset: 0x006761F3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use OrganizationDomainMethod.Call.GmCmd_ForceUpdateTreasuryGuards instead.", true)]
			public static void GmCmd_ForceUpdateTreasuryGuards(IAsyncMethodRequestHandler requestHandler, short settlementId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601113D RID: 69949 RVA: 0x00677FFB File Offset: 0x006761FB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use OrganizationDomainMethod.Call.AddSectBounty instead.", true)]
			public static void AddSectBounty(IAsyncMethodRequestHandler requestHandler, sbyte orgTemplateId, int charId, sbyte punishmentSeverity, short punishmentType, int duration, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601113E RID: 69950 RVA: 0x00678003 File Offset: 0x00676203
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use OrganizationDomainMethod.Call.AddSectPrisoner instead.", true)]
			public static void AddSectPrisoner(IAsyncMethodRequestHandler requestHandler, sbyte orgTemplateId, int charId, sbyte punishmentSeverity, short punishmentType, int duration, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601113F RID: 69951 RVA: 0x0067800C File Offset: 0x0067620C
			public static void GetSettlementPrisonDisplayData(IAsyncMethodRequestHandler requestHandler, short settlementId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(3, 20, settlementId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011140 RID: 69952 RVA: 0x00678038 File Offset: 0x00676238
			public static void GetSettlementBountyDisplayData(IAsyncMethodRequestHandler requestHandler, short settlementId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(3, 21, settlementId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011141 RID: 69953 RVA: 0x00678064 File Offset: 0x00676264
			public static void GetSettlementPrisonRecordCollection(IAsyncMethodRequestHandler requestHandler, short settlementId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(3, 22, settlementId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011142 RID: 69954 RVA: 0x0067808F File Offset: 0x0067628F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use OrganizationDomainMethod.Call.GmCmd_ForceUpdateInfluencePower instead.", true)]
			public static void GmCmd_ForceUpdateInfluencePower(IAsyncMethodRequestHandler requestHandler, short settlementId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011143 RID: 69955 RVA: 0x00678098 File Offset: 0x00676298
			public static void GetBountyCharacterDisplayDataFromCharacterList(IAsyncMethodRequestHandler requestHandler, List<int> characterIds, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(3, 24, characterIds, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011144 RID: 69956 RVA: 0x006780C3 File Offset: 0x006762C3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use OrganizationDomainMethod.Call.ForceUpdateTaiwuVillager instead.", true)]
			public static void ForceUpdateTaiwuVillager(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011145 RID: 69957 RVA: 0x006780CC File Offset: 0x006762CC
			public static void IsTaiwuSectFugitive(IAsyncMethodRequestHandler requestHandler, sbyte orgTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(3, 26, orgTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011146 RID: 69958 RVA: 0x006780F8 File Offset: 0x006762F8
			public static void GetOrganizationTemplateIdOfTaiwuLocation(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(3, 27, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011147 RID: 69959 RVA: 0x00678124 File Offset: 0x00676324
			public static void GetLastSettlementTreasuryOperationData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(3, 28, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011148 RID: 69960 RVA: 0x00678150 File Offset: 0x00676350
			public static void GmCmd_GetSettlementPrisoner(IAsyncMethodRequestHandler requestHandler, int prisonType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(3, 29, prisonType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011149 RID: 69961 RVA: 0x0067817C File Offset: 0x0067637C
			public static void CheckSettlementGuardFavorabilityType(IAsyncMethodRequestHandler requestHandler, short settlementId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(3, 30, settlementId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601114A RID: 69962 RVA: 0x006781A7 File Offset: 0x006763A7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use OrganizationDomainMethod.Call.GmCmd_SetAllSettlementMemberApprovedTaiwu instead.", true)]
			public static void GmCmd_SetAllSettlementMemberApprovedTaiwu(IAsyncMethodRequestHandler requestHandler, sbyte orgTemplateId, bool approvedTaiwu, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601114B RID: 69963 RVA: 0x006781B0 File Offset: 0x006763B0
			public static void GetSectFunctionStatus(IAsyncMethodRequestHandler requestHandler, sbyte orgTemplateId, SectFunctionStatuses.SectFunctionStatusType statusType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte, SectFunctionStatuses.SectFunctionStatusType>(3, 32, orgTemplateId, statusType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601114C RID: 69964 RVA: 0x006781DC File Offset: 0x006763DC
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use OrganizationDomainMethod.Call.GmCmd_SetSectFunctionStatus instead.", true)]
			public static void GmCmd_SetSectFunctionStatus(IAsyncMethodRequestHandler requestHandler, sbyte orgTemplateId, SectFunctionStatuses.SectFunctionStatusType statusType, bool value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601114D RID: 69965 RVA: 0x006781E4 File Offset: 0x006763E4
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use OrganizationDomainMethod.Call.UpdateCityPunishmentSeverityCustomizeData instead.", true)]
			public static void UpdateCityPunishmentSeverityCustomizeData(IAsyncMethodRequestHandler requestHandler, sbyte stateTemplateId, bool isSect, short punishmentTypeTemplateId, sbyte customizedPunishmentSeverityTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601114E RID: 69966 RVA: 0x006781EC File Offset: 0x006763EC
			public static void GetCustomizePunishmentSeverityCost(IAsyncMethodRequestHandler requestHandler, sbyte stateTemplateId, bool isSect, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte, bool>(3, 35, stateTemplateId, isSect, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601114F RID: 69967 RVA: 0x00678218 File Offset: 0x00676418
			public static void WillCustomizePunishmentBreakWithoutVillagerHead(IAsyncMethodRequestHandler requestHandler, int villagerHeadCharId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(3, 36, villagerHeadCharId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011150 RID: 69968 RVA: 0x00678244 File Offset: 0x00676444
			public static void GetSettlementPopulationDisplayData(IAsyncMethodRequestHandler requestHandler, short settlementId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(3, 37, settlementId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011151 RID: 69969 RVA: 0x00678270 File Offset: 0x00676470
			public static void GetReversedSettlementPrisonRecordCollection(IAsyncMethodRequestHandler requestHandler, short settlementId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(3, 38, settlementId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011152 RID: 69970 RVA: 0x0067829C File Offset: 0x0067649C
			public static void GetReversedSettlementTreasuryRecordCollection(IAsyncMethodRequestHandler requestHandler, short settlementId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(3, 39, settlementId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011153 RID: 69971 RVA: 0x006782C8 File Offset: 0x006764C8
			public static void GetSettlementApproveTaiwuMembers(IAsyncMethodRequestHandler requestHandler, short settlementId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(3, 40, settlementId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}
		}
	}
}
