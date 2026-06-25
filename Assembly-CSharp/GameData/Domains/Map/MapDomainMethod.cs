using System;
using System.Collections.Generic;
using GameData.GameDataBridge;

namespace GameData.Domains.Map
{
	// Token: 0x02000FC4 RID: 4036
	public static class MapDomainMethod
	{
		// Token: 0x02002603 RID: 9731
		public static class Call
		{
			// Token: 0x060111C0 RID: 70080 RVA: 0x00678D0D File Offset: 0x00676F0D
			public static void GmCmd_SetLockTime(bool isLock)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 2, 0, isLock);
			}

			// Token: 0x060111C1 RID: 70081 RVA: 0x00678D1A File Offset: 0x00676F1A
			public static void GmCmd_SetTeleportMove(bool teleportOn)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 2, 1, teleportOn);
			}

			// Token: 0x060111C2 RID: 70082 RVA: 0x00678D27 File Offset: 0x00676F27
			public static void GmCmd_ShowAllMapBlock()
			{
				GameDataBridge.AddMethodCall(-1, 2, 2);
			}

			// Token: 0x060111C3 RID: 70083 RVA: 0x00678D33 File Offset: 0x00676F33
			public static void GmCmd_UnlockAllStation()
			{
				GameDataBridge.AddMethodCall(-1, 2, 3);
			}

			// Token: 0x060111C4 RID: 70084 RVA: 0x00678D3F File Offset: 0x00676F3F
			public static void GmCmd_ChangeSpiritualDebt(short areaId, int spiritualDebt)
			{
				GameDataBridge.AddMethodCall<short, int>(-1, 2, 4, areaId, spiritualDebt);
			}

			// Token: 0x060111C5 RID: 70085 RVA: 0x00678D4D File Offset: 0x00676F4D
			public static void GmCmd_SetMapBlockData(MapBlockData mapBlockData)
			{
				GameDataBridge.AddMethodCall<MapBlockData>(-1, 2, 5, mapBlockData);
			}

			// Token: 0x060111C6 RID: 70086 RVA: 0x00678D5A File Offset: 0x00676F5A
			public static void GmCmd_CreateFixedCharacterAtCurrentBlock(short templateId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 2, 6, templateId);
			}

			// Token: 0x060111C7 RID: 70087 RVA: 0x00678D67 File Offset: 0x00676F67
			public static void Move(short destBlockId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 2, 7, destBlockId);
			}

			// Token: 0x060111C8 RID: 70088 RVA: 0x00678D74 File Offset: 0x00676F74
			public static void MoveFinish(Location previous, Location current)
			{
				GameDataBridge.AddMethodCall<Location, Location>(-1, 2, 8, previous, current);
			}

			// Token: 0x060111C9 RID: 70089 RVA: 0x00678D82 File Offset: 0x00676F82
			public static void IsContinuousMovingBreak(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 2, 9);
			}

			// Token: 0x060111CA RID: 70090 RVA: 0x00678D8F File Offset: 0x00676F8F
			public static void UnlockStation(short areaId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 2, 10, areaId);
			}

			// Token: 0x060111CB RID: 70091 RVA: 0x00678D9D File Offset: 0x00676F9D
			public static void UnlockStation(short areaId, bool costAuthority)
			{
				GameDataBridge.AddMethodCall<short, bool>(-1, 2, 10, areaId, costAuthority);
			}

			// Token: 0x060111CC RID: 70092 RVA: 0x00678DAC File Offset: 0x00676FAC
			public static void GetTravelCost(int listenerId, short fromAreaId, short fromBlockId, short toAreaId)
			{
				GameDataBridge.AddMethodCall<short, short, short>(listenerId, 2, 11, fromAreaId, fromBlockId, toAreaId);
			}

			// Token: 0x060111CD RID: 70093 RVA: 0x00678DBC File Offset: 0x00676FBC
			public static void StartTravel(short toAreaId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 2, 12, toAreaId);
			}

			// Token: 0x060111CE RID: 70094 RVA: 0x00678DCA File Offset: 0x00676FCA
			public static void ContinueTravel(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 2, 13);
			}

			// Token: 0x060111CF RID: 70095 RVA: 0x00678DD7 File Offset: 0x00676FD7
			public static void StopTravel()
			{
				GameDataBridge.AddMethodCall(-1, 2, 14);
			}

			// Token: 0x060111D0 RID: 70096 RVA: 0x00678DE4 File Offset: 0x00676FE4
			public static void RecordTravelCostedDays(int costedDays)
			{
				GameDataBridge.AddMethodCall<int>(-1, 2, 15, costedDays);
			}

			// Token: 0x060111D1 RID: 70097 RVA: 0x00678DF2 File Offset: 0x00676FF2
			public static void GetAllAreaCompletelyInfectedCharCount(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 2, 16);
			}

			// Token: 0x060111D2 RID: 70098 RVA: 0x00678DFF File Offset: 0x00676FFF
			public static void GetTravelRoutesInState(int listenerId, sbyte stateId)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 2, 17, stateId);
			}

			// Token: 0x060111D3 RID: 70099 RVA: 0x00678E0D File Offset: 0x0067700D
			public static void TryTriggerCricketCatch(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 2, 18);
			}

			// Token: 0x060111D4 RID: 70100 RVA: 0x00678E1A File Offset: 0x0067701A
			public static void GetBlockData(int listenerId, short areaId, short blockId)
			{
				GameDataBridge.AddMethodCall<short, short>(listenerId, 2, 19, areaId, blockId);
			}

			// Token: 0x060111D5 RID: 70101 RVA: 0x00678E29 File Offset: 0x00677029
			public static void CollectResource(int listenerId, int charId, sbyte resourceType)
			{
				GameDataBridge.AddMethodCall<int, sbyte>(listenerId, 2, 20, charId, resourceType);
			}

			// Token: 0x060111D6 RID: 70102 RVA: 0x00678E38 File Offset: 0x00677038
			public static void CollectResource(int listenerId, int charId, sbyte resourceType, bool costTime)
			{
				GameDataBridge.AddMethodCall<int, sbyte, bool>(listenerId, 2, 20, charId, resourceType, costTime);
			}

			// Token: 0x060111D7 RID: 70103 RVA: 0x00678E48 File Offset: 0x00677048
			public static void CollectResource(int listenerId, int charId, sbyte resourceType, bool costTime, bool costResource)
			{
				GameDataBridge.AddMethodCall<int, sbyte, bool, bool>(listenerId, 2, 20, charId, resourceType, costTime, costResource);
			}

			// Token: 0x060111D8 RID: 70104 RVA: 0x00678E5A File Offset: 0x0067705A
			public static void GetMapBlockDataList(int listenerId, List<Location> locationList)
			{
				GameDataBridge.AddMethodCall<List<Location>>(listenerId, 2, 21, locationList);
			}

			// Token: 0x060111D9 RID: 70105 RVA: 0x00678E68 File Offset: 0x00677068
			public static void GetBelongBlockTemplateIdList(int listenerId, List<Location> locationList)
			{
				GameDataBridge.AddMethodCall<List<Location>>(listenerId, 2, 22, locationList);
			}

			// Token: 0x060111DA RID: 70106 RVA: 0x00678E76 File Offset: 0x00677076
			public static void GetLocationNameRelatedData(int listenerId, Location location)
			{
				GameDataBridge.AddMethodCall<Location>(listenerId, 2, 23, location);
			}

			// Token: 0x060111DB RID: 70107 RVA: 0x00678E84 File Offset: 0x00677084
			public static void GetLocationNameRelatedDataList(int listenerId, List<Location> locations)
			{
				GameDataBridge.AddMethodCall<List<Location>>(listenerId, 2, 24, locations);
			}

			// Token: 0x060111DC RID: 70108 RVA: 0x00678E92 File Offset: 0x00677092
			public static void ChangeBlockTemplate(int listenerId, Location location, short blockTemplateId, bool isTurnVisible)
			{
				GameDataBridge.AddMethodCall<Location, short, bool>(listenerId, 2, 25, location, blockTemplateId, isTurnVisible);
			}

			// Token: 0x060111DD RID: 70109 RVA: 0x00678EA2 File Offset: 0x006770A2
			public static void IsContainsPurpleBamboo(int listenerId, short areaId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 2, 26, areaId);
			}

			// Token: 0x060111DE RID: 70110 RVA: 0x00678EB0 File Offset: 0x006770B0
			public static void GetAllStateCompletelyInfectedCharCount(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 2, 27);
			}

			// Token: 0x060111DF RID: 70111 RVA: 0x00678EBD File Offset: 0x006770BD
			public static void GetBlockFullName(int listenerId, Location location)
			{
				GameDataBridge.AddMethodCall<Location>(listenerId, 2, 28, location);
			}

			// Token: 0x060111E0 RID: 70112 RVA: 0x00678ECB File Offset: 0x006770CB
			public static void GetMapBlockDataListOptional(int listenerId, List<Location> locationList)
			{
				GameDataBridge.AddMethodCall<List<Location>>(listenerId, 2, 29, locationList);
			}

			// Token: 0x060111E1 RID: 70113 RVA: 0x00678ED9 File Offset: 0x006770D9
			public static void GetMapBlockDataListOptional(int listenerId, List<Location> locationList, bool includeRoot)
			{
				GameDataBridge.AddMethodCall<List<Location>, bool>(listenerId, 2, 29, locationList, includeRoot);
			}

			// Token: 0x060111E2 RID: 70114 RVA: 0x00678EE8 File Offset: 0x006770E8
			public static void GetMapBlockDataListOptional(int listenerId, List<Location> locationList, bool includeRoot, bool includeBelong)
			{
				GameDataBridge.AddMethodCall<List<Location>, bool, bool>(listenerId, 2, 29, locationList, includeRoot, includeBelong);
			}

			// Token: 0x060111E3 RID: 70115 RVA: 0x00678EF8 File Offset: 0x006770F8
			public static void IsLocationInBuildingEffectRange(int listenerId, Location location, Location settlementLocation)
			{
				GameDataBridge.AddMethodCall<Location, Location>(listenerId, 2, 30, location, settlementLocation);
			}

			// Token: 0x060111E4 RID: 70116 RVA: 0x00678F07 File Offset: 0x00677107
			public static void ContinueTravelWithDetectTravelingEvent(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 2, 31);
			}

			// Token: 0x060111E5 RID: 70117 RVA: 0x00678F14 File Offset: 0x00677114
			public static void CollectAllResourcesFree(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 2, 32);
			}

			// Token: 0x060111E6 RID: 70118 RVA: 0x00678F21 File Offset: 0x00677121
			public static void QuickTravel(short destAreaId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 2, 33, destAreaId);
			}

			// Token: 0x060111E7 RID: 70119 RVA: 0x00678F2F File Offset: 0x0067712F
			public static void QueryFixedCharacterLocation(int listenerId, short templateId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 2, 34, templateId);
			}

			// Token: 0x060111E8 RID: 70120 RVA: 0x00678F3D File Offset: 0x0067713D
			public static void GetAllAreaDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 2, 35);
			}

			// Token: 0x060111E9 RID: 70121 RVA: 0x00678F4A File Offset: 0x0067714A
			public static void QueryTemplateBlockLocation(int listenerId, int templateId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 2, 36, templateId);
			}

			// Token: 0x060111EA RID: 70122 RVA: 0x00678F58 File Offset: 0x00677158
			public static void GetBlockDisplayDataInArea(int listenerId, short areaId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 2, 37, areaId);
			}

			// Token: 0x060111EB RID: 70123 RVA: 0x00678F66 File Offset: 0x00677166
			public static void UnlockTravelPath(int listenerId, short toAreaId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 2, 38, toAreaId);
			}

			// Token: 0x060111EC RID: 70124 RVA: 0x00678F74 File Offset: 0x00677174
			public static void GmCmd_HideAllMapBlock()
			{
				GameDataBridge.AddMethodCall(-1, 2, 39);
			}

			// Token: 0x060111ED RID: 70125 RVA: 0x00678F81 File Offset: 0x00677181
			public static void GetPathInAreaWithoutCost(int listenerId, Location start, Location end)
			{
				GameDataBridge.AddMethodCall<Location, Location>(listenerId, 2, 40, start, end);
			}

			// Token: 0x060111EE RID: 70126 RVA: 0x00678F90 File Offset: 0x00677190
			public static void GetTravelPreview(int listenerId, short toAreaId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 2, 41, toAreaId);
			}

			// Token: 0x060111EF RID: 70127 RVA: 0x00678F9E File Offset: 0x0067719E
			public static void RetrieveDreamBackLocation(Location location)
			{
				GameDataBridge.AddMethodCall<Location>(-1, 2, 42, location);
			}

			// Token: 0x060111F0 RID: 70128 RVA: 0x00678FAC File Offset: 0x006771AC
			public static void GetAreaByAreaId(int listenerId, short areaId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 2, 43, areaId);
			}

			// Token: 0x060111F1 RID: 70129 RVA: 0x00678FBA File Offset: 0x006771BA
			public static void GmCmd_AddAnimal(short templateId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 2, 44, templateId);
			}

			// Token: 0x060111F2 RID: 70130 RVA: 0x00678FC8 File Offset: 0x006771C8
			public static void GetTeammateBubbleCollection(int listenerId, bool isTraveling)
			{
				GameDataBridge.AddMethodCall<bool>(listenerId, 2, 45, isTraveling);
			}

			// Token: 0x060111F3 RID: 70131 RVA: 0x00678FD6 File Offset: 0x006771D6
			public static void GmCmd_AddRandomEnemyOnMap(short templateId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 2, 46, templateId);
			}

			// Token: 0x060111F4 RID: 70132 RVA: 0x00678FE4 File Offset: 0x006771E4
			public static void GMCmd_ThrowBackend()
			{
				GameDataBridge.AddMethodCall(-1, 2, 47);
			}

			// Token: 0x060111F5 RID: 70133 RVA: 0x00678FF1 File Offset: 0x006771F1
			public static void SimulateHealCost(int listenerId, int typeInt, int doctorId, int patientId)
			{
				GameDataBridge.AddMethodCall<int, int, int>(listenerId, 2, 48, typeInt, doctorId, patientId);
			}

			// Token: 0x060111F6 RID: 70134 RVA: 0x00679001 File Offset: 0x00677201
			public static void SimulateHealCost(int listenerId, int typeInt, int doctorId, int patientId, bool needPay)
			{
				GameDataBridge.AddMethodCall<int, int, int, bool>(listenerId, 2, 48, typeInt, doctorId, patientId, needPay);
			}

			// Token: 0x060111F7 RID: 70135 RVA: 0x00679013 File Offset: 0x00677213
			public static void SimulateHealCost(int listenerId, int typeInt, int doctorId, int patientId, bool needPay, bool isExpensiveHeal)
			{
				GameDataBridge.AddMethodCall<int, int, int, bool, bool>(listenerId, 2, 48, typeInt, doctorId, patientId, needPay, isExpensiveHeal);
			}

			// Token: 0x060111F8 RID: 70136 RVA: 0x00679027 File Offset: 0x00677227
			public static void HealOnMap(int listenerId, int typeInt, int doctorId, int patientId)
			{
				GameDataBridge.AddMethodCall<int, int, int>(listenerId, 2, 49, typeInt, doctorId, patientId);
			}

			// Token: 0x060111F9 RID: 70137 RVA: 0x00679037 File Offset: 0x00677237
			public static void HealOnMap(int listenerId, int typeInt, int doctorId, int patientId, bool needPay)
			{
				GameDataBridge.AddMethodCall<int, int, int, bool>(listenerId, 2, 49, typeInt, doctorId, patientId, needPay);
			}

			// Token: 0x060111FA RID: 70138 RVA: 0x00679049 File Offset: 0x00677249
			public static void HealOnMap(int listenerId, int typeInt, int doctorId, int patientId, bool needPay, int payerId)
			{
				GameDataBridge.AddMethodCall<int, int, int, bool, int>(listenerId, 2, 49, typeInt, doctorId, patientId, needPay, payerId);
			}

			// Token: 0x060111FB RID: 70139 RVA: 0x00679060 File Offset: 0x00677260
			public static void HealOnMap(int listenerId, int typeInt, int doctorId, int patientId, bool needPay, int payerId, bool isExpensiveHeal)
			{
				GameDataBridge.AddMethodCall<int, int, int, bool, int, bool>(listenerId, 2, 49, typeInt, doctorId, patientId, needPay, payerId, isExpensiveHeal);
			}

			// Token: 0x060111FC RID: 70140 RVA: 0x00679081 File Offset: 0x00677281
			public static void TeleportByTraveler(short destBlockId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 2, 50, destBlockId);
			}

			// Token: 0x060111FD RID: 70141 RVA: 0x0067908F File Offset: 0x0067728F
			public static void BuildTravelerPalace(int listenerId, Location location)
			{
				GameDataBridge.AddMethodCall<Location>(listenerId, 2, 51, location);
			}

			// Token: 0x060111FE RID: 70142 RVA: 0x0067909D File Offset: 0x0067729D
			public static void TeleportOnTravelerPalace(int listenerId, int index)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 2, 52, index);
			}

			// Token: 0x060111FF RID: 70143 RVA: 0x006790AB File Offset: 0x006772AB
			public static void ChangeTravelerPalaceName(int listenerId, int index, string newName)
			{
				GameDataBridge.AddMethodCall<int, string>(listenerId, 2, 53, index, newName);
			}

			// Token: 0x06011200 RID: 70144 RVA: 0x006790BA File Offset: 0x006772BA
			public static void DestroyTravelerPalace(int listenerId, int index)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 2, 54, index);
			}

			// Token: 0x06011201 RID: 70145 RVA: 0x006790C8 File Offset: 0x006772C8
			public static void GmCmd_ChangeAllSpiritualDebt(int spiritualDebt)
			{
				GameDataBridge.AddMethodCall<int>(-1, 2, 55, spiritualDebt);
			}

			// Token: 0x06011202 RID: 70146 RVA: 0x006790D6 File Offset: 0x006772D6
			public static void TaiwuBeKidnapped(Location targetLocation, int hunterCharId)
			{
				GameDataBridge.AddMethodCall<Location, int>(-1, 2, 56, targetLocation, hunterCharId);
			}

			// Token: 0x06011203 RID: 70147 RVA: 0x006790E5 File Offset: 0x006772E5
			public static void DirectTravelToTaiwuVillage()
			{
				GameDataBridge.AddMethodCall(-1, 2, 57);
			}

			// Token: 0x06011204 RID: 70148 RVA: 0x006790F2 File Offset: 0x006772F2
			public static void QueryTemplateBlockLocationInArea(int listenerId, int templateId, short areaId)
			{
				GameDataBridge.AddMethodCall<int, short>(listenerId, 2, 58, templateId, areaId);
			}

			// Token: 0x06011205 RID: 70149 RVA: 0x00679101 File Offset: 0x00677301
			public static void QueryFixedCharacterLocationInArea(int listenerId, short templateId, short areaId)
			{
				GameDataBridge.AddMethodCall<short, short>(listenerId, 2, 59, templateId, areaId);
			}

			// Token: 0x06011206 RID: 70150 RVA: 0x00679110 File Offset: 0x00677310
			public static void GmCmd_TurnMapBlockIntoAshes()
			{
				GameDataBridge.AddMethodCall(-1, 2, 60);
			}

			// Token: 0x06011207 RID: 70151 RVA: 0x0067911D File Offset: 0x0067731D
			public static void GmCmd_TriggerTravelingEvent(int listenerId, short templateId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 2, 61, templateId);
			}

			// Token: 0x06011208 RID: 70152 RVA: 0x0067912B File Offset: 0x0067732B
			public static void GmCmd_GetTreasuryValueByTaiwuLocation(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 2, 62);
			}

			// Token: 0x06011209 RID: 70153 RVA: 0x00679138 File Offset: 0x00677338
			public static void RequestMapBlockCharacterList(int listenerId, MapBlockData data)
			{
				GameDataBridge.AddMethodCall<MapBlockData>(listenerId, 2, 63, data);
			}

			// Token: 0x0601120A RID: 70154 RVA: 0x00679146 File Offset: 0x00677346
			public static void GetMapBlockCharacterCountData(int listenerId, Location location)
			{
				GameDataBridge.AddMethodCall<Location>(listenerId, 2, 64, location);
			}

			// Token: 0x0601120B RID: 70155 RVA: 0x00679154 File Offset: 0x00677354
			public static void GetMapBlockCharacterCountData(int listenerId, Location location, List<short> orgTemplateIds)
			{
				GameDataBridge.AddMethodCall<Location, List<short>>(listenerId, 2, 64, location, orgTemplateIds);
			}

			// Token: 0x0601120C RID: 70156 RVA: 0x00679163 File Offset: 0x00677363
			public static void GetMapBlockFindDataPreset(int listenerId, int index)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 2, 65, index);
			}

			// Token: 0x0601120D RID: 70157 RVA: 0x00679171 File Offset: 0x00677371
			public static void SetMapBlockFindDataPreset(int listenerId, int index, MapBlockFindData findData)
			{
				GameDataBridge.AddMethodCall<int, MapBlockFindData>(listenerId, 2, 66, index, findData);
			}

			// Token: 0x0601120E RID: 70158 RVA: 0x00679180 File Offset: 0x00677380
			public static void GetReversedTeammateBubble(int listenerId, bool isTraveling)
			{
				GameDataBridge.AddMethodCall<bool>(listenerId, 2, 67, isTraveling);
			}

			// Token: 0x0601120F RID: 70159 RVA: 0x0067918E File Offset: 0x0067738E
			public static void RemoveSwordTombFromLocation(Location location)
			{
				GameDataBridge.AddMethodCall<Location>(-1, 2, 68, location);
			}

			// Token: 0x06011210 RID: 70160 RVA: 0x0067919C File Offset: 0x0067739C
			public static void SetBlockAndViewRangeVisibleByBlockTemplateId(Location location, short templateId)
			{
				GameDataBridge.AddMethodCall<Location, short>(-1, 2, 69, location, templateId);
			}

			// Token: 0x06011211 RID: 70161 RVA: 0x006791AB File Offset: 0x006773AB
			public static void GetPathInAreaWithAvoidSettings(int listenerId, Location start, Location end)
			{
				GameDataBridge.AddMethodCall<Location, Location>(listenerId, 2, 70, start, end);
			}
		}

		// Token: 0x02002604 RID: 9732
		public static class AsyncCall
		{
			// Token: 0x06011212 RID: 70162 RVA: 0x006791BA File Offset: 0x006773BA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.GmCmd_SetLockTime instead.", true)]
			public static void GmCmd_SetLockTime(IAsyncMethodRequestHandler requestHandler, bool isLock, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011213 RID: 70163 RVA: 0x006791C2 File Offset: 0x006773C2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.GmCmd_SetTeleportMove instead.", true)]
			public static void GmCmd_SetTeleportMove(IAsyncMethodRequestHandler requestHandler, bool teleportOn, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011214 RID: 70164 RVA: 0x006791CA File Offset: 0x006773CA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.GmCmd_ShowAllMapBlock instead.", true)]
			public static void GmCmd_ShowAllMapBlock(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011215 RID: 70165 RVA: 0x006791D2 File Offset: 0x006773D2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.GmCmd_UnlockAllStation instead.", true)]
			public static void GmCmd_UnlockAllStation(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011216 RID: 70166 RVA: 0x006791DA File Offset: 0x006773DA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.GmCmd_ChangeSpiritualDebt instead.", true)]
			public static void GmCmd_ChangeSpiritualDebt(IAsyncMethodRequestHandler requestHandler, short areaId, int spiritualDebt, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011217 RID: 70167 RVA: 0x006791E2 File Offset: 0x006773E2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.GmCmd_SetMapBlockData instead.", true)]
			public static void GmCmd_SetMapBlockData(IAsyncMethodRequestHandler requestHandler, MapBlockData mapBlockData, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011218 RID: 70168 RVA: 0x006791EA File Offset: 0x006773EA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.GmCmd_CreateFixedCharacterAtCurrentBlock instead.", true)]
			public static void GmCmd_CreateFixedCharacterAtCurrentBlock(IAsyncMethodRequestHandler requestHandler, short templateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011219 RID: 70169 RVA: 0x006791F2 File Offset: 0x006773F2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.Move instead.", true)]
			public static void Move(IAsyncMethodRequestHandler requestHandler, short destBlockId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601121A RID: 70170 RVA: 0x006791FA File Offset: 0x006773FA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.MoveFinish instead.", true)]
			public static void MoveFinish(IAsyncMethodRequestHandler requestHandler, Location previous, Location current, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601121B RID: 70171 RVA: 0x00679204 File Offset: 0x00677404
			public static void IsContinuousMovingBreak(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(2, 9, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601121C RID: 70172 RVA: 0x0067922E File Offset: 0x0067742E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.UnlockStation instead.", true)]
			public static void UnlockStation(IAsyncMethodRequestHandler requestHandler, short areaId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601121D RID: 70173 RVA: 0x00679236 File Offset: 0x00677436
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.UnlockStation instead.", true)]
			public static void UnlockStation(IAsyncMethodRequestHandler requestHandler, short areaId, bool costAuthority, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601121E RID: 70174 RVA: 0x00679240 File Offset: 0x00677440
			public static void GetTravelCost(IAsyncMethodRequestHandler requestHandler, short fromAreaId, short fromBlockId, short toAreaId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, short, short>(2, 11, fromAreaId, fromBlockId, toAreaId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601121F RID: 70175 RVA: 0x0067926E File Offset: 0x0067746E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.StartTravel instead.", true)]
			public static void StartTravel(IAsyncMethodRequestHandler requestHandler, short toAreaId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011220 RID: 70176 RVA: 0x00679278 File Offset: 0x00677478
			public static void ContinueTravel(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(2, 13, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011221 RID: 70177 RVA: 0x006792A2 File Offset: 0x006774A2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.StopTravel instead.", true)]
			public static void StopTravel(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011222 RID: 70178 RVA: 0x006792AA File Offset: 0x006774AA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.RecordTravelCostedDays instead.", true)]
			public static void RecordTravelCostedDays(IAsyncMethodRequestHandler requestHandler, int costedDays, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011223 RID: 70179 RVA: 0x006792B4 File Offset: 0x006774B4
			public static void GetAllAreaCompletelyInfectedCharCount(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(2, 16, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011224 RID: 70180 RVA: 0x006792E0 File Offset: 0x006774E0
			public static void GetTravelRoutesInState(IAsyncMethodRequestHandler requestHandler, sbyte stateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(2, 17, stateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011225 RID: 70181 RVA: 0x0067930C File Offset: 0x0067750C
			public static void TryTriggerCricketCatch(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(2, 18, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011226 RID: 70182 RVA: 0x00679338 File Offset: 0x00677538
			public static void GetBlockData(IAsyncMethodRequestHandler requestHandler, short areaId, short blockId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, short>(2, 19, areaId, blockId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011227 RID: 70183 RVA: 0x00679364 File Offset: 0x00677564
			public static void CollectResource(IAsyncMethodRequestHandler requestHandler, int charId, sbyte resourceType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, sbyte>(2, 20, charId, resourceType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011228 RID: 70184 RVA: 0x00679390 File Offset: 0x00677590
			public static void CollectResource(IAsyncMethodRequestHandler requestHandler, int charId, sbyte resourceType, bool costTime, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, sbyte, bool>(2, 20, charId, resourceType, costTime, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011229 RID: 70185 RVA: 0x006793C0 File Offset: 0x006775C0
			public static void CollectResource(IAsyncMethodRequestHandler requestHandler, int charId, sbyte resourceType, bool costTime, bool costResource, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, sbyte, bool, bool>(2, 20, charId, resourceType, costTime, costResource, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601122A RID: 70186 RVA: 0x006793F0 File Offset: 0x006775F0
			public static void GetMapBlockDataList(IAsyncMethodRequestHandler requestHandler, List<Location> locationList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<Location>>(2, 21, locationList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601122B RID: 70187 RVA: 0x0067941C File Offset: 0x0067761C
			public static void GetBelongBlockTemplateIdList(IAsyncMethodRequestHandler requestHandler, List<Location> locationList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<Location>>(2, 22, locationList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601122C RID: 70188 RVA: 0x00679448 File Offset: 0x00677648
			public static void GetLocationNameRelatedData(IAsyncMethodRequestHandler requestHandler, Location location, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Location>(2, 23, location, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601122D RID: 70189 RVA: 0x00679474 File Offset: 0x00677674
			public static void GetLocationNameRelatedDataList(IAsyncMethodRequestHandler requestHandler, List<Location> locations, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<Location>>(2, 24, locations, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601122E RID: 70190 RVA: 0x006794A0 File Offset: 0x006776A0
			public static void ChangeBlockTemplate(IAsyncMethodRequestHandler requestHandler, Location location, short blockTemplateId, bool isTurnVisible, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Location, short, bool>(2, 25, location, blockTemplateId, isTurnVisible, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601122F RID: 70191 RVA: 0x006794D0 File Offset: 0x006776D0
			public static void IsContainsPurpleBamboo(IAsyncMethodRequestHandler requestHandler, short areaId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(2, 26, areaId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011230 RID: 70192 RVA: 0x006794FC File Offset: 0x006776FC
			public static void GetAllStateCompletelyInfectedCharCount(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(2, 27, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011231 RID: 70193 RVA: 0x00679528 File Offset: 0x00677728
			public static void GetBlockFullName(IAsyncMethodRequestHandler requestHandler, Location location, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Location>(2, 28, location, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011232 RID: 70194 RVA: 0x00679554 File Offset: 0x00677754
			public static void GetMapBlockDataListOptional(IAsyncMethodRequestHandler requestHandler, List<Location> locationList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<Location>>(2, 29, locationList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011233 RID: 70195 RVA: 0x00679580 File Offset: 0x00677780
			public static void GetMapBlockDataListOptional(IAsyncMethodRequestHandler requestHandler, List<Location> locationList, bool includeRoot, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<Location>, bool>(2, 29, locationList, includeRoot, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011234 RID: 70196 RVA: 0x006795AC File Offset: 0x006777AC
			public static void GetMapBlockDataListOptional(IAsyncMethodRequestHandler requestHandler, List<Location> locationList, bool includeRoot, bool includeBelong, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<Location>, bool, bool>(2, 29, locationList, includeRoot, includeBelong, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011235 RID: 70197 RVA: 0x006795DC File Offset: 0x006777DC
			public static void IsLocationInBuildingEffectRange(IAsyncMethodRequestHandler requestHandler, Location location, Location settlementLocation, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Location, Location>(2, 30, location, settlementLocation, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011236 RID: 70198 RVA: 0x00679608 File Offset: 0x00677808
			public static void ContinueTravelWithDetectTravelingEvent(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(2, 31, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011237 RID: 70199 RVA: 0x00679634 File Offset: 0x00677834
			public static void CollectAllResourcesFree(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(2, 32, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011238 RID: 70200 RVA: 0x0067965E File Offset: 0x0067785E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.QuickTravel instead.", true)]
			public static void QuickTravel(IAsyncMethodRequestHandler requestHandler, short destAreaId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011239 RID: 70201 RVA: 0x00679668 File Offset: 0x00677868
			public static void QueryFixedCharacterLocation(IAsyncMethodRequestHandler requestHandler, short templateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(2, 34, templateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601123A RID: 70202 RVA: 0x00679694 File Offset: 0x00677894
			public static void GetAllAreaDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(2, 35, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601123B RID: 70203 RVA: 0x006796C0 File Offset: 0x006778C0
			public static void QueryTemplateBlockLocation(IAsyncMethodRequestHandler requestHandler, int templateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(2, 36, templateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601123C RID: 70204 RVA: 0x006796EC File Offset: 0x006778EC
			public static void GetBlockDisplayDataInArea(IAsyncMethodRequestHandler requestHandler, short areaId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(2, 37, areaId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601123D RID: 70205 RVA: 0x00679718 File Offset: 0x00677918
			public static void UnlockTravelPath(IAsyncMethodRequestHandler requestHandler, short toAreaId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(2, 38, toAreaId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601123E RID: 70206 RVA: 0x00679743 File Offset: 0x00677943
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.GmCmd_HideAllMapBlock instead.", true)]
			public static void GmCmd_HideAllMapBlock(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601123F RID: 70207 RVA: 0x0067974C File Offset: 0x0067794C
			public static void GetPathInAreaWithoutCost(IAsyncMethodRequestHandler requestHandler, Location start, Location end, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Location, Location>(2, 40, start, end, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011240 RID: 70208 RVA: 0x00679778 File Offset: 0x00677978
			public static void GetTravelPreview(IAsyncMethodRequestHandler requestHandler, short toAreaId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(2, 41, toAreaId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011241 RID: 70209 RVA: 0x006797A3 File Offset: 0x006779A3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.RetrieveDreamBackLocation instead.", true)]
			public static void RetrieveDreamBackLocation(IAsyncMethodRequestHandler requestHandler, Location location, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011242 RID: 70210 RVA: 0x006797AC File Offset: 0x006779AC
			public static void GetAreaByAreaId(IAsyncMethodRequestHandler requestHandler, short areaId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(2, 43, areaId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011243 RID: 70211 RVA: 0x006797D7 File Offset: 0x006779D7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.GmCmd_AddAnimal instead.", true)]
			public static void GmCmd_AddAnimal(IAsyncMethodRequestHandler requestHandler, short templateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011244 RID: 70212 RVA: 0x006797E0 File Offset: 0x006779E0
			public static void GetTeammateBubbleCollection(IAsyncMethodRequestHandler requestHandler, bool isTraveling, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool>(2, 45, isTraveling, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011245 RID: 70213 RVA: 0x0067980B File Offset: 0x00677A0B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.GmCmd_AddRandomEnemyOnMap instead.", true)]
			public static void GmCmd_AddRandomEnemyOnMap(IAsyncMethodRequestHandler requestHandler, short templateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011246 RID: 70214 RVA: 0x00679813 File Offset: 0x00677A13
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.GMCmd_ThrowBackend instead.", true)]
			public static void GMCmd_ThrowBackend(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011247 RID: 70215 RVA: 0x0067981C File Offset: 0x00677A1C
			public static void SimulateHealCost(IAsyncMethodRequestHandler requestHandler, int typeInt, int doctorId, int patientId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int, int>(2, 48, typeInt, doctorId, patientId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011248 RID: 70216 RVA: 0x0067984C File Offset: 0x00677A4C
			public static void SimulateHealCost(IAsyncMethodRequestHandler requestHandler, int typeInt, int doctorId, int patientId, bool needPay, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int, int, bool>(2, 48, typeInt, doctorId, patientId, needPay, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011249 RID: 70217 RVA: 0x0067987C File Offset: 0x00677A7C
			public static void SimulateHealCost(IAsyncMethodRequestHandler requestHandler, int typeInt, int doctorId, int patientId, bool needPay, bool isExpensiveHeal, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int, int, bool, bool>(2, 48, typeInt, doctorId, patientId, needPay, isExpensiveHeal, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601124A RID: 70218 RVA: 0x006798B0 File Offset: 0x00677AB0
			public static void HealOnMap(IAsyncMethodRequestHandler requestHandler, int typeInt, int doctorId, int patientId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int, int>(2, 49, typeInt, doctorId, patientId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601124B RID: 70219 RVA: 0x006798E0 File Offset: 0x00677AE0
			public static void HealOnMap(IAsyncMethodRequestHandler requestHandler, int typeInt, int doctorId, int patientId, bool needPay, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int, int, bool>(2, 49, typeInt, doctorId, patientId, needPay, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601124C RID: 70220 RVA: 0x00679910 File Offset: 0x00677B10
			public static void HealOnMap(IAsyncMethodRequestHandler requestHandler, int typeInt, int doctorId, int patientId, bool needPay, int payerId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int, int, bool, int>(2, 49, typeInt, doctorId, patientId, needPay, payerId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601124D RID: 70221 RVA: 0x00679944 File Offset: 0x00677B44
			public static void HealOnMap(IAsyncMethodRequestHandler requestHandler, int typeInt, int doctorId, int patientId, bool needPay, int payerId, bool isExpensiveHeal, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int, int, bool, int, bool>(2, 49, typeInt, doctorId, patientId, needPay, payerId, isExpensiveHeal, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601124E RID: 70222 RVA: 0x00679978 File Offset: 0x00677B78
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.TeleportByTraveler instead.", true)]
			public static void TeleportByTraveler(IAsyncMethodRequestHandler requestHandler, short destBlockId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601124F RID: 70223 RVA: 0x00679980 File Offset: 0x00677B80
			public static void BuildTravelerPalace(IAsyncMethodRequestHandler requestHandler, Location location, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Location>(2, 51, location, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011250 RID: 70224 RVA: 0x006799AC File Offset: 0x00677BAC
			public static void TeleportOnTravelerPalace(IAsyncMethodRequestHandler requestHandler, int index, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(2, 52, index, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011251 RID: 70225 RVA: 0x006799D8 File Offset: 0x00677BD8
			public static void ChangeTravelerPalaceName(IAsyncMethodRequestHandler requestHandler, int index, string newName, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, string>(2, 53, index, newName, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011252 RID: 70226 RVA: 0x00679A04 File Offset: 0x00677C04
			public static void DestroyTravelerPalace(IAsyncMethodRequestHandler requestHandler, int index, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(2, 54, index, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011253 RID: 70227 RVA: 0x00679A2F File Offset: 0x00677C2F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.GmCmd_ChangeAllSpiritualDebt instead.", true)]
			public static void GmCmd_ChangeAllSpiritualDebt(IAsyncMethodRequestHandler requestHandler, int spiritualDebt, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011254 RID: 70228 RVA: 0x00679A37 File Offset: 0x00677C37
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.TaiwuBeKidnapped instead.", true)]
			public static void TaiwuBeKidnapped(IAsyncMethodRequestHandler requestHandler, Location targetLocation, int hunterCharId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011255 RID: 70229 RVA: 0x00679A3F File Offset: 0x00677C3F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.DirectTravelToTaiwuVillage instead.", true)]
			public static void DirectTravelToTaiwuVillage(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011256 RID: 70230 RVA: 0x00679A48 File Offset: 0x00677C48
			public static void QueryTemplateBlockLocationInArea(IAsyncMethodRequestHandler requestHandler, int templateId, short areaId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short>(2, 58, templateId, areaId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011257 RID: 70231 RVA: 0x00679A74 File Offset: 0x00677C74
			public static void QueryFixedCharacterLocationInArea(IAsyncMethodRequestHandler requestHandler, short templateId, short areaId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, short>(2, 59, templateId, areaId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011258 RID: 70232 RVA: 0x00679AA0 File Offset: 0x00677CA0
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.GmCmd_TurnMapBlockIntoAshes instead.", true)]
			public static void GmCmd_TurnMapBlockIntoAshes(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011259 RID: 70233 RVA: 0x00679AA8 File Offset: 0x00677CA8
			public static void GmCmd_TriggerTravelingEvent(IAsyncMethodRequestHandler requestHandler, short templateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(2, 61, templateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601125A RID: 70234 RVA: 0x00679AD4 File Offset: 0x00677CD4
			public static void GmCmd_GetTreasuryValueByTaiwuLocation(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(2, 62, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601125B RID: 70235 RVA: 0x00679B00 File Offset: 0x00677D00
			public static void RequestMapBlockCharacterList(IAsyncMethodRequestHandler requestHandler, MapBlockData data, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<MapBlockData>(2, 63, data, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601125C RID: 70236 RVA: 0x00679B2C File Offset: 0x00677D2C
			public static void GetMapBlockCharacterCountData(IAsyncMethodRequestHandler requestHandler, Location location, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Location>(2, 64, location, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601125D RID: 70237 RVA: 0x00679B58 File Offset: 0x00677D58
			public static void GetMapBlockCharacterCountData(IAsyncMethodRequestHandler requestHandler, Location location, List<short> orgTemplateIds, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Location, List<short>>(2, 64, location, orgTemplateIds, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601125E RID: 70238 RVA: 0x00679B84 File Offset: 0x00677D84
			public static void GetMapBlockFindDataPreset(IAsyncMethodRequestHandler requestHandler, int index, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(2, 65, index, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601125F RID: 70239 RVA: 0x00679BB0 File Offset: 0x00677DB0
			public static void SetMapBlockFindDataPreset(IAsyncMethodRequestHandler requestHandler, int index, MapBlockFindData findData, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, MapBlockFindData>(2, 66, index, findData, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011260 RID: 70240 RVA: 0x00679BDC File Offset: 0x00677DDC
			public static void GetReversedTeammateBubble(IAsyncMethodRequestHandler requestHandler, bool isTraveling, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool>(2, 67, isTraveling, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011261 RID: 70241 RVA: 0x00679C07 File Offset: 0x00677E07
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.RemoveSwordTombFromLocation instead.", true)]
			public static void RemoveSwordTombFromLocation(IAsyncMethodRequestHandler requestHandler, Location location, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011262 RID: 70242 RVA: 0x00679C0F File Offset: 0x00677E0F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MapDomainMethod.Call.SetBlockAndViewRangeVisibleByBlockTemplateId instead.", true)]
			public static void SetBlockAndViewRangeVisibleByBlockTemplateId(IAsyncMethodRequestHandler requestHandler, Location location, short templateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011263 RID: 70243 RVA: 0x00679C18 File Offset: 0x00677E18
			public static void GetPathInAreaWithAvoidSettings(IAsyncMethodRequestHandler requestHandler, Location start, Location end, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Location, Location>(2, 70, start, end, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}
		}
	}
}
