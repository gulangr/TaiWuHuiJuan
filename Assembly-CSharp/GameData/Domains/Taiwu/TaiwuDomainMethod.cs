using System;
using System.Collections.Generic;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Map;
using GameData.Domains.Merchant;
using GameData.Domains.Taiwu.Debate;
using GameData.Domains.Taiwu.ExchangeSystem;
using GameData.GameDataBridge;
using GameData.Utilities;

namespace GameData.Domains.Taiwu
{
	// Token: 0x02000FBE RID: 4030
	public static class TaiwuDomainMethod
	{
		// Token: 0x020025F7 RID: 9719
		public static class Call
		{
			// Token: 0x06010E1E RID: 69150 RVA: 0x00673494 File Offset: 0x00671694
			public static void GetAllVisitedSettlements(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 0);
			}

			// Token: 0x06010E1F RID: 69151 RVA: 0x006734A0 File Offset: 0x006716A0
			public static void SetVillagerCollectResourceWork(int listenerId, int charId, short areaId, short blockId, sbyte resourceType)
			{
				GameDataBridge.AddMethodCall<int, short, short, sbyte>(listenerId, 5, 1, charId, areaId, blockId, resourceType);
			}

			// Token: 0x06010E20 RID: 69152 RVA: 0x006734B1 File Offset: 0x006716B1
			public static void SetVillagerCollectTributeWork(int listenerId, int charId, short areaId, short blockId)
			{
				GameDataBridge.AddMethodCall<int, short, short>(listenerId, 5, 2, charId, areaId, blockId);
			}

			// Token: 0x06010E21 RID: 69153 RVA: 0x006734C0 File Offset: 0x006716C0
			public static void SetVillagerKeepGraveWork(int listenerId, int charId, short areaId, short blockId, int graveId)
			{
				GameDataBridge.AddMethodCall<int, short, short, int>(listenerId, 5, 3, charId, areaId, blockId, graveId);
			}

			// Token: 0x06010E22 RID: 69154 RVA: 0x006734D1 File Offset: 0x006716D1
			public static void SetVillagerIdleWork(int listenerId, int charId, short areaId, short blockId)
			{
				GameDataBridge.AddMethodCall<int, short, short>(listenerId, 5, 4, charId, areaId, blockId);
			}

			// Token: 0x06010E23 RID: 69155 RVA: 0x006734E0 File Offset: 0x006716E0
			public static void StopVillagerWork(int listenerId, short areaId, short blockId, sbyte workType)
			{
				GameDataBridge.AddMethodCall<short, short, sbyte>(listenerId, 5, 5, areaId, blockId, workType);
			}

			// Token: 0x06010E24 RID: 69156 RVA: 0x006734EF File Offset: 0x006716EF
			public static void StopVillagerCollectResourceWork(int listenerId, short areaId, short blockId)
			{
				GameDataBridge.AddMethodCall<short, short>(listenerId, 5, 6, areaId, blockId);
			}

			// Token: 0x06010E25 RID: 69157 RVA: 0x006734FD File Offset: 0x006716FD
			public static void GetCollectResourceWorkDataList(int listenerId, List<Location> locationList)
			{
				GameDataBridge.AddMethodCall<List<Location>>(listenerId, 5, 7, locationList);
			}

			// Token: 0x06010E26 RID: 69158 RVA: 0x0067350A File Offset: 0x0067170A
			public static void ExpelVillager(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 5, 8, charId);
			}

			// Token: 0x06010E27 RID: 69159 RVA: 0x00673517 File Offset: 0x00671717
			public static void GetVillagerStatusDisplayDataList(int listenerId, List<int> charIds)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 5, 9, charIds);
			}

			// Token: 0x06010E28 RID: 69160 RVA: 0x00673525 File Offset: 0x00671725
			public static void GetAllVillagersStatus(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 10);
			}

			// Token: 0x06010E29 RID: 69161 RVA: 0x00673532 File Offset: 0x00671732
			public static void GetAllVillagersAvailableForWork(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 11);
			}

			// Token: 0x06010E2A RID: 69162 RVA: 0x0067353F File Offset: 0x0067173F
			public static void GetAllVillagersAvailableForWork(int listenerId, bool actuallyNotOccupiedOnly)
			{
				GameDataBridge.AddMethodCall<bool>(listenerId, 5, 11, actuallyNotOccupiedOnly);
			}

			// Token: 0x06010E2B RID: 69163 RVA: 0x0067354D File Offset: 0x0067174D
			public static void CalcResourceChangeByVillageWork(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 12);
			}

			// Token: 0x06010E2C RID: 69164 RVA: 0x0067355A File Offset: 0x0067175A
			public static void CalcResourceChangeByBuildingEarn(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 13);
			}

			// Token: 0x06010E2D RID: 69165 RVA: 0x00673567 File Offset: 0x00671767
			public static void CalcResourceChangeByBuildingMaintain(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 14);
			}

			// Token: 0x06010E2E RID: 69166 RVA: 0x00673574 File Offset: 0x00671774
			public static void GetAllWarehouseItems(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 15);
			}

			// Token: 0x06010E2F RID: 69167 RVA: 0x00673581 File Offset: 0x00671781
			public static void GetWarehouseItemsBySubType(int listenerId, short itemSubType)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 5, 16, itemSubType);
			}

			// Token: 0x06010E30 RID: 69168 RVA: 0x0067358F File Offset: 0x0067178F
			public static void SwitchEquipmentPlan(int planId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 5, 17, planId);
			}

			// Token: 0x06010E31 RID: 69169 RVA: 0x0067359D File Offset: 0x0067179D
			public static void GmCmd_AddResource(sbyte type, int count)
			{
				GameDataBridge.AddMethodCall<sbyte, int>(-1, 5, 18, type, count);
			}

			// Token: 0x06010E32 RID: 69170 RVA: 0x006735AC File Offset: 0x006717AC
			public static void GmCmd_AddLegacyPoint(short template, int percent)
			{
				GameDataBridge.AddMethodCall<short, int>(-1, 5, 19, template, percent);
			}

			// Token: 0x06010E33 RID: 69171 RVA: 0x006735BB File Offset: 0x006717BB
			public static void GmCmd_AddExp(int count)
			{
				GameDataBridge.AddMethodCall<int>(-1, 5, 20, count);
			}

			// Token: 0x06010E34 RID: 69172 RVA: 0x006735C9 File Offset: 0x006717C9
			public static void GmCmd_SetTaiwuCombatSkillActiveState(short skillTemplateId, ushort selectedPages)
			{
				GameDataBridge.AddMethodCall<short, ushort>(-1, 5, 21, skillTemplateId, selectedPages);
			}

			// Token: 0x06010E35 RID: 69173 RVA: 0x006735D8 File Offset: 0x006717D8
			public static void GmCmd_SetTaiwuCombatSkillActiveState(short skillTemplateId, ushort selectedPages, bool bonusOn)
			{
				GameDataBridge.AddMethodCall<short, ushort, bool>(-1, 5, 21, skillTemplateId, selectedPages, bonusOn);
			}

			// Token: 0x06010E36 RID: 69174 RVA: 0x006735E8 File Offset: 0x006717E8
			public static void JoinGroup(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 5, 22, charId);
			}

			// Token: 0x06010E37 RID: 69175 RVA: 0x006735F6 File Offset: 0x006717F6
			public static void JoinGroup(int charId, bool showNotification)
			{
				GameDataBridge.AddMethodCall<int, bool>(-1, 5, 22, charId, showNotification);
			}

			// Token: 0x06010E38 RID: 69176 RVA: 0x00673605 File Offset: 0x00671805
			public static void LeaveGroup(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 5, 23, charId);
			}

			// Token: 0x06010E39 RID: 69177 RVA: 0x00673613 File Offset: 0x00671813
			public static void LeaveGroup(int charId, bool bringWards)
			{
				GameDataBridge.AddMethodCall<int, bool>(-1, 5, 23, charId, bringWards);
			}

			// Token: 0x06010E3A RID: 69178 RVA: 0x00673622 File Offset: 0x00671822
			public static void LeaveGroup(int charId, bool bringWards, bool showNotification)
			{
				GameDataBridge.AddMethodCall<int, bool, bool>(-1, 5, 23, charId, bringWards, showNotification);
			}

			// Token: 0x06010E3B RID: 69179 RVA: 0x00673632 File Offset: 0x00671832
			public static void LeaveGroup(int charId, bool bringWards, bool showNotification, bool moveToRandomAdjacentBlock)
			{
				GameDataBridge.AddMethodCall<int, bool, bool, bool>(-1, 5, 23, charId, bringWards, showNotification, moveToRandomAdjacentBlock);
			}

			// Token: 0x06010E3C RID: 69180 RVA: 0x00673643 File Offset: 0x00671843
			public static void CompletePassingLegacy()
			{
				GameDataBridge.AddMethodCall(-1, 5, 24);
			}

			// Token: 0x06010E3D RID: 69181 RVA: 0x00673650 File Offset: 0x00671850
			public static void SelectLegacy(short templateId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 5, 25, templateId);
			}

			// Token: 0x06010E3E RID: 69182 RVA: 0x0067365E File Offset: 0x0067185E
			public static void FindSuccessorCandidates(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 26);
			}

			// Token: 0x06010E3F RID: 69183 RVA: 0x0067366B File Offset: 0x0067186B
			public static void FindSuccessorCandidates(int listenerId, bool includeInvalid)
			{
				GameDataBridge.AddMethodCall<bool>(listenerId, 5, 26, includeInvalid);
			}

			// Token: 0x06010E40 RID: 69184 RVA: 0x00673679 File Offset: 0x00671879
			public static void ConfirmChosenSuccessor(int newTaiwuId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 5, 27, newTaiwuId);
			}

			// Token: 0x06010E41 RID: 69185 RVA: 0x00673687 File Offset: 0x00671887
			public static void SetReferenceBook(sbyte index, ItemKey bookItemKey)
			{
				GameDataBridge.AddMethodCall<sbyte, ItemKey>(-1, 5, 28, index, bookItemKey);
			}

			// Token: 0x06010E42 RID: 69186 RVA: 0x00673696 File Offset: 0x00671896
			public static void SetReadingBook(ItemKey bookItemKey)
			{
				GameDataBridge.AddMethodCall<ItemKey>(-1, 5, 29, bookItemKey);
			}

			// Token: 0x06010E43 RID: 69187 RVA: 0x006736A4 File Offset: 0x006718A4
			public static void GetCurReadingStrategies(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 30);
			}

			// Token: 0x06010E44 RID: 69188 RVA: 0x006736B1 File Offset: 0x006718B1
			public static void SetReadingStrategy(byte pageIndex, int strategyIndex, sbyte strategyId)
			{
				GameDataBridge.AddMethodCall<byte, int, sbyte>(-1, 5, 31, pageIndex, strategyIndex, strategyId);
			}

			// Token: 0x06010E45 RID: 69189 RVA: 0x006736C1 File Offset: 0x006718C1
			public static void ClearPageStrategy(byte pageIndex)
			{
				GameDataBridge.AddMethodCall<byte>(-1, 5, 32, pageIndex);
			}

			// Token: 0x06010E46 RID: 69190 RVA: 0x006736CF File Offset: 0x006718CF
			public static void GetRandomSelectableStrategies(int listenerId, byte pageIndex)
			{
				GameDataBridge.AddMethodCall<byte>(listenerId, 5, 33, pageIndex);
			}

			// Token: 0x06010E47 RID: 69191 RVA: 0x006736DD File Offset: 0x006718DD
			public static void CheckNotInInventoryBooks()
			{
				GameDataBridge.AddMethodCall(-1, 5, 34);
			}

			// Token: 0x06010E48 RID: 69192 RVA: 0x006736EA File Offset: 0x006718EA
			public static void GetTotalReadingProgress(int listenerId, int bookItemId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 5, 35, bookItemId);
			}

			// Token: 0x06010E49 RID: 69193 RVA: 0x006736F8 File Offset: 0x006718F8
			public static void GetCurrReadingEventBonusRate(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 36);
			}

			// Token: 0x06010E4A RID: 69194 RVA: 0x00673705 File Offset: 0x00671905
			public static void GetCurrReadingEfficiency(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 37);
			}

			// Token: 0x06010E4B RID: 69195 RVA: 0x00673712 File Offset: 0x00671912
			public static void WarehouseAdd(ItemKey itemKey, int amount)
			{
				GameDataBridge.AddMethodCall<ItemKey, int>(-1, 5, 38, itemKey, amount);
			}

			// Token: 0x06010E4C RID: 69196 RVA: 0x00673721 File Offset: 0x00671921
			public static void WarehouseRemove(ItemKey itemKey, int amount)
			{
				GameDataBridge.AddMethodCall<ItemKey, int>(-1, 5, 39, itemKey, amount);
			}

			// Token: 0x06010E4D RID: 69197 RVA: 0x00673730 File Offset: 0x00671930
			public static void WarehouseRemove(ItemKey itemKey, int amount, bool deleteItem)
			{
				GameDataBridge.AddMethodCall<ItemKey, int, bool>(-1, 5, 39, itemKey, amount, deleteItem);
			}

			// Token: 0x06010E4E RID: 69198 RVA: 0x00673740 File Offset: 0x00671940
			public static void PutItemIntoWarehouse(ItemKey itemKey, int amount)
			{
				GameDataBridge.AddMethodCall<ItemKey, int>(-1, 5, 40, itemKey, amount);
			}

			// Token: 0x06010E4F RID: 69199 RVA: 0x0067374F File Offset: 0x0067194F
			public static void TakeOutItemFromWarehouse(ItemKey itemKey, int amount)
			{
				GameDataBridge.AddMethodCall<ItemKey, int>(-1, 5, 41, itemKey, amount);
			}

			// Token: 0x06010E50 RID: 69200 RVA: 0x0067375E File Offset: 0x0067195E
			public static void CanTransferItemToWarehouse(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 42);
			}

			// Token: 0x06010E51 RID: 69201 RVA: 0x0067376B File Offset: 0x0067196B
			public static void CalcBuildingResourceOutput(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 5, 43, blockKey);
			}

			// Token: 0x06010E52 RID: 69202 RVA: 0x00673779 File Offset: 0x00671979
			public static void TransferAllItems(int listenerId, bool isToWarehouse, List<ItemKey> keyList)
			{
				GameDataBridge.AddMethodCall<bool, List<ItemKey>>(listenerId, 5, 44, isToWarehouse, keyList);
			}

			// Token: 0x06010E53 RID: 69203 RVA: 0x00673788 File Offset: 0x00671988
			public static void SelectCombatSkillAttainmentPanelPlan(sbyte combatSkillType, sbyte planId)
			{
				GameDataBridge.AddMethodCall<sbyte, sbyte>(-1, 5, 45, combatSkillType, planId);
			}

			// Token: 0x06010E54 RID: 69204 RVA: 0x00673797 File Offset: 0x00671997
			public static void GetGenericGridAllocation(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 46);
			}

			// Token: 0x06010E55 RID: 69205 RVA: 0x006737A4 File Offset: 0x006719A4
			public static void AllocateGenericGrid(sbyte equipType)
			{
				GameDataBridge.AddMethodCall<sbyte>(-1, 5, 47, equipType);
			}

			// Token: 0x06010E56 RID: 69206 RVA: 0x006737B2 File Offset: 0x006719B2
			public static void DeallocateGenericGrid(sbyte equipType)
			{
				GameDataBridge.AddMethodCall<sbyte>(-1, 5, 48, equipType);
			}

			// Token: 0x06010E57 RID: 69207 RVA: 0x006737C0 File Offset: 0x006719C0
			public static void UpdateCombatSkillPlan(int index)
			{
				GameDataBridge.AddMethodCall<int>(-1, 5, 49, index);
			}

			// Token: 0x06010E58 RID: 69208 RVA: 0x006737CE File Offset: 0x006719CE
			public static void GetBreakPlateData(int listenerId, short skillTemplateId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 5, 50, skillTemplateId);
			}

			// Token: 0x06010E59 RID: 69209 RVA: 0x006737DC File Offset: 0x006719DC
			public static void EnterSkillBreakPlate(int listenerId, short skillId, ushort selectedPages)
			{
				GameDataBridge.AddMethodCall<short, ushort>(listenerId, 5, 51, skillId, selectedPages);
			}

			// Token: 0x06010E5A RID: 69210 RVA: 0x006737EB File Offset: 0x006719EB
			public static void ClearBreakPlate(short skillId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 5, 52, skillId);
			}

			// Token: 0x06010E5B RID: 69211 RVA: 0x006737F9 File Offset: 0x006719F9
			public static void ClearBreakPlate(short skillId, bool fromGmCmd)
			{
				GameDataBridge.AddMethodCall<short, bool>(-1, 5, 52, skillId, fromGmCmd);
			}

			// Token: 0x06010E5C RID: 69212 RVA: 0x00673808 File Offset: 0x00671A08
			public static void SelectSkillBreakGrid(int listenerId, short skillId, SkillBreakPlateIndex index)
			{
				GameDataBridge.AddMethodCall<short, SkillBreakPlateIndex>(listenerId, 5, 53, skillId, index);
			}

			// Token: 0x06010E5D RID: 69213 RVA: 0x00673817 File Offset: 0x00671A17
			public static void EscapeToAdjacentBlock()
			{
				GameDataBridge.AddMethodCall(-1, 5, 54);
			}

			// Token: 0x06010E5E RID: 69214 RVA: 0x00673824 File Offset: 0x00671A24
			public static void GetCanOperateItemDisplayDataInVillage(int listenerId, short itemSubType)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 5, 55, itemSubType);
			}

			// Token: 0x06010E5F RID: 69215 RVA: 0x00673832 File Offset: 0x00671A32
			public static void PutItemListIntoWarehouse(List<ItemKey> keyList)
			{
				GameDataBridge.AddMethodCall<List<ItemKey>>(-1, 5, 56, keyList);
			}

			// Token: 0x06010E60 RID: 69216 RVA: 0x00673840 File Offset: 0x00671A40
			public static void WarehouseAddList(List<ItemKey> keyList)
			{
				GameDataBridge.AddMethodCall<List<ItemKey>>(-1, 5, 57, keyList);
			}

			// Token: 0x06010E61 RID: 69217 RVA: 0x0067384E File Offset: 0x00671A4E
			public static void TakeOutItemListFromWarehouse(List<ItemKey> keyList)
			{
				GameDataBridge.AddMethodCall<List<ItemKey>>(-1, 5, 58, keyList);
			}

			// Token: 0x06010E62 RID: 69218 RVA: 0x0067385C File Offset: 0x00671A5C
			public static void WarehouseRemoveList(List<ItemKey> keyList)
			{
				GameDataBridge.AddMethodCall<List<ItemKey>>(-1, 5, 59, keyList);
			}

			// Token: 0x06010E63 RID: 69219 RVA: 0x0067386A File Offset: 0x00671A6A
			public static void WarehouseRemoveList(List<ItemKey> keyList, bool deleteItem)
			{
				GameDataBridge.AddMethodCall<List<ItemKey>, bool>(-1, 5, 59, keyList, deleteItem);
			}

			// Token: 0x06010E64 RID: 69220 RVA: 0x00673879 File Offset: 0x00671A79
			public static void GetTaiwuAllItems(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 60);
			}

			// Token: 0x06010E65 RID: 69221 RVA: 0x00673886 File Offset: 0x00671A86
			public static void TransferItem(int listenerId, sbyte from, sbyte to, ItemKey itemKey, int amount)
			{
				GameDataBridge.AddMethodCall<sbyte, sbyte, ItemKey, int>(listenerId, 5, 61, from, to, itemKey, amount);
			}

			// Token: 0x06010E66 RID: 69222 RVA: 0x00673898 File Offset: 0x00671A98
			public static void TransferItem(int listenerId, sbyte from, sbyte to, ItemKey itemKey, int amount, bool offLine)
			{
				GameDataBridge.AddMethodCall<sbyte, sbyte, ItemKey, int, bool>(listenerId, 5, 61, from, to, itemKey, amount, offLine);
			}

			// Token: 0x06010E67 RID: 69223 RVA: 0x006738AC File Offset: 0x00671AAC
			public static void GetAllTroughItems(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 62);
			}

			// Token: 0x06010E68 RID: 69224 RVA: 0x006738B9 File Offset: 0x00671AB9
			public static void TransferItemList(int listenerId, sbyte from, sbyte to, List<ItemKey> keyList)
			{
				GameDataBridge.AddMethodCall<sbyte, sbyte, List<ItemKey>>(listenerId, 5, 63, from, to, keyList);
			}

			// Token: 0x06010E69 RID: 69225 RVA: 0x006738C9 File Offset: 0x00671AC9
			public static void GetAllTreasuryItems(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 64);
			}

			// Token: 0x06010E6A RID: 69226 RVA: 0x006738D6 File Offset: 0x00671AD6
			public static void GetTotalReadingProgressList(int listenerId, List<int> bookItemIdList)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 5, 65, bookItemIdList);
			}

			// Token: 0x06010E6B RID: 69227 RVA: 0x006738E4 File Offset: 0x00671AE4
			public static void CalcResourceChangeByAutoExpand(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 66);
			}

			// Token: 0x06010E6C RID: 69228 RVA: 0x006738F1 File Offset: 0x00671AF1
			public static void CalcAutoExpandNotSatisfyIndex(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 67);
			}

			// Token: 0x06010E6D RID: 69229 RVA: 0x006738FE File Offset: 0x00671AFE
			public static void GetRefBonusSpeed(int listenerId, ItemKey refBookKey)
			{
				GameDataBridge.AddMethodCall<ItemKey>(listenerId, 5, 68, refBookKey);
			}

			// Token: 0x06010E6E RID: 69230 RVA: 0x0067390C File Offset: 0x00671B0C
			public static void FindTaiwuBuilding(int listenerId, short buildingTemplateId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 5, 69, buildingTemplateId);
			}

			// Token: 0x06010E6F RID: 69231 RVA: 0x0067391A File Offset: 0x00671B1A
			public static void FindTaiwuBuilding(int listenerId, short buildingTemplateId, bool checkUsable)
			{
				GameDataBridge.AddMethodCall<short, bool>(listenerId, 5, 69, buildingTemplateId, checkUsable);
			}

			// Token: 0x06010E70 RID: 69232 RVA: 0x00673929 File Offset: 0x00671B29
			public static void ChoosyGetMaterial(int listenerId, sbyte resourceType, int count)
			{
				GameDataBridge.AddMethodCall<sbyte, int>(listenerId, 5, 70, resourceType, count);
			}

			// Token: 0x06010E71 RID: 69233 RVA: 0x00673938 File Offset: 0x00671B38
			public static void GetCannotOperateItemDisplayDataInInventory(int listenerId, short itemSubType)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 5, 71, itemSubType);
			}

			// Token: 0x06010E72 RID: 69234 RVA: 0x00673946 File Offset: 0x00671B46
			public static void GetInventoryOverloadedGroupCharNames(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 72);
			}

			// Token: 0x06010E73 RID: 69235 RVA: 0x00673953 File Offset: 0x00671B53
			public static void SetAutoAllocateNeiliToMax(bool value)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 5, 73, value);
			}

			// Token: 0x06010E74 RID: 69236 RVA: 0x00673961 File Offset: 0x00671B61
			public static void MasteredSkillWillChangePlan(int listenerId, short skillTemplateId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 5, 74, skillTemplateId);
			}

			// Token: 0x06010E75 RID: 69237 RVA: 0x0067396F File Offset: 0x00671B6F
			public static void GetVillagersForWork(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 75);
			}

			// Token: 0x06010E76 RID: 69238 RVA: 0x0067397C File Offset: 0x00671B7C
			public static void GetVillagersForWork(int listenerId, bool includeUnlockedWorkingVillagers)
			{
				GameDataBridge.AddMethodCall<bool>(listenerId, 5, 75, includeUnlockedWorkingVillagers);
			}

			// Token: 0x06010E77 RID: 69239 RVA: 0x0067398A File Offset: 0x00671B8A
			public static void GetVillagersForWork(int listenerId, bool includeUnlockedWorkingVillagers, bool farmerFirst)
			{
				GameDataBridge.AddMethodCall<bool, bool>(listenerId, 5, 75, includeUnlockedWorkingVillagers, farmerFirst);
			}

			// Token: 0x06010E78 RID: 69240 RVA: 0x00673999 File Offset: 0x00671B99
			public static void GetSeverelyInjuredGroupCharNames(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 76);
			}

			// Token: 0x06010E79 RID: 69241 RVA: 0x006739A6 File Offset: 0x00671BA6
			public static void GetSeverelyInjuredGroupCharNames(int listenerId, bool includeTaiwu)
			{
				GameDataBridge.AddMethodCall<bool>(listenerId, 5, 76, includeTaiwu);
			}

			// Token: 0x06010E7A RID: 69242 RVA: 0x006739B4 File Offset: 0x00671BB4
			public static void GetItemCount(int listenerId, sbyte itemType, short itemTemplateId)
			{
				GameDataBridge.AddMethodCall<sbyte, short>(listenerId, 5, 77, itemType, itemTemplateId);
			}

			// Token: 0x06010E7B RID: 69243 RVA: 0x006739C3 File Offset: 0x00671BC3
			public static void GetTaiwuVillagerMapBlockData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 78);
			}

			// Token: 0x06010E7C RID: 69244 RVA: 0x006739D0 File Offset: 0x00671BD0
			public static void StopVillagerWorkOptional(int listenerId, short areaId, short blockId, sbyte workType, bool removeUnlockedState)
			{
				GameDataBridge.AddMethodCall<short, short, sbyte, bool>(listenerId, 5, 79, areaId, blockId, workType, removeUnlockedState);
			}

			// Token: 0x06010E7D RID: 69245 RVA: 0x006739E2 File Offset: 0x00671BE2
			public static void GetLegacyMaxPointByType(int listenerId, short legacyType)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 5, 80, legacyType);
			}

			// Token: 0x06010E7E RID: 69246 RVA: 0x006739F0 File Offset: 0x00671BF0
			public static void GetCurrReadingBanByWug(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 81);
			}

			// Token: 0x06010E7F RID: 69247 RVA: 0x006739FD File Offset: 0x00671BFD
			public static void GmCmd_MarkAllCarrierFullTamePoint()
			{
				GameDataBridge.AddMethodCall(-1, 5, 82);
			}

			// Token: 0x06010E80 RID: 69248 RVA: 0x00673A0A File Offset: 0x00671C0A
			public static void GetSelectMapBlockHasMerchantId(int listenerId, Location location)
			{
				GameDataBridge.AddMethodCall<Location>(listenerId, 5, 83, location);
			}

			// Token: 0x06010E81 RID: 69249 RVA: 0x00673A18 File Offset: 0x00671C18
			public static void ActiveReadOnce()
			{
				GameDataBridge.AddMethodCall(-1, 5, 84);
			}

			// Token: 0x06010E82 RID: 69250 RVA: 0x00673A25 File Offset: 0x00671C25
			public static void ActiveNeigongLoopingOnce()
			{
				GameDataBridge.AddMethodCall(-1, 5, 85);
			}

			// Token: 0x06010E83 RID: 69251 RVA: 0x00673A32 File Offset: 0x00671C32
			public static void AppendCombatSkillPlan()
			{
				GameDataBridge.AddMethodCall(-1, 5, 86);
			}

			// Token: 0x06010E84 RID: 69252 RVA: 0x00673A3F File Offset: 0x00671C3F
			public static void CopyCombatSkillPlan()
			{
				GameDataBridge.AddMethodCall(-1, 5, 87);
			}

			// Token: 0x06010E85 RID: 69253 RVA: 0x00673A4C File Offset: 0x00671C4C
			public static void ClearCombatSkillPlan()
			{
				GameDataBridge.AddMethodCall(-1, 5, 88);
			}

			// Token: 0x06010E86 RID: 69254 RVA: 0x00673A59 File Offset: 0x00671C59
			public static void DeleteCombatSkillPlan()
			{
				GameDataBridge.AddMethodCall(-1, 5, 89);
			}

			// Token: 0x06010E87 RID: 69255 RVA: 0x00673A66 File Offset: 0x00671C66
			public static void GetLegacyMaxPointAndTimesListByType(int listenerId, short legacyType)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 5, 90, legacyType);
			}

			// Token: 0x06010E88 RID: 69256 RVA: 0x00673A74 File Offset: 0x00671C74
			public static void SetQiArtStrategy(int index, sbyte templateId)
			{
				GameDataBridge.AddMethodCall<int, sbyte>(-1, 5, 91, index, templateId);
			}

			// Token: 0x06010E89 RID: 69257 RVA: 0x00673A83 File Offset: 0x00671C83
			public static void GetCurrentBookAvailableReadingStrategies(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 92);
			}

			// Token: 0x06010E8A RID: 69258 RVA: 0x00673A90 File Offset: 0x00671C90
			public static void GetLoopingNeigongQiArtStrategies(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 93);
			}

			// Token: 0x06010E8B RID: 69259 RVA: 0x00673A9D File Offset: 0x00671C9D
			public static void SetReferenceCombatSkillAt(int index, short combatSkillTemplateId)
			{
				GameDataBridge.AddMethodCall<int, short>(-1, 5, 94, index, combatSkillTemplateId);
			}

			// Token: 0x06010E8C RID: 69260 RVA: 0x00673AAC File Offset: 0x00671CAC
			public static void GetLoopingNeigongQiArtStrategyDisplayDatas(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 95);
			}

			// Token: 0x06010E8D RID: 69261 RVA: 0x00673AB9 File Offset: 0x00671CB9
			public static void GetLoopingNeigongAvailableQiArtStrategies(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 96);
			}

			// Token: 0x06010E8E RID: 69262 RVA: 0x00673AC6 File Offset: 0x00671CC6
			public static void ClearCurrentLoopingNeigongEvent()
			{
				GameDataBridge.AddMethodCall(-1, 5, 97);
			}

			// Token: 0x06010E8F RID: 69263 RVA: 0x00673AD3 File Offset: 0x00671CD3
			public static void SetTaiwuLoopingNeigong(short combatSkillTemplateId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 5, 98, combatSkillTemplateId);
			}

			// Token: 0x06010E90 RID: 69264 RVA: 0x00673AE1 File Offset: 0x00671CE1
			public static void DeleteTaiwuFeature(short featureId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 5, 99, featureId);
			}

			// Token: 0x06010E91 RID: 69265 RVA: 0x00673AEF File Offset: 0x00671CEF
			public static void GmCmd_TaiwuActiveLoopingApply()
			{
				GameDataBridge.AddMethodCall(-1, 5, 100);
			}

			// Token: 0x06010E92 RID: 69266 RVA: 0x00673AFC File Offset: 0x00671CFC
			public static void GetIsFollowingNpcListMax(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 101);
			}

			// Token: 0x06010E93 RID: 69267 RVA: 0x00673B09 File Offset: 0x00671D09
			public static void GetFollowingNpcListMaxCount(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 102);
			}

			// Token: 0x06010E94 RID: 69268 RVA: 0x00673B16 File Offset: 0x00671D16
			public static void GmCmd_FollowRandomNpc(int count)
			{
				GameDataBridge.AddMethodCall<int>(-1, 5, 103, count);
			}

			// Token: 0x06010E95 RID: 69269 RVA: 0x00673B24 File Offset: 0x00671D24
			public static void TaiwuFollowNpc(int npcId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 5, 104, npcId);
			}

			// Token: 0x06010E96 RID: 69270 RVA: 0x00673B32 File Offset: 0x00671D32
			public static void TaiwuUnfollowNpc(int characterId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 5, 105, characterId);
			}

			// Token: 0x06010E97 RID: 69271 RVA: 0x00673B40 File Offset: 0x00671D40
			public static void SetFollowingNpcNickName(int listenerId, int characterId, string nickName)
			{
				GameDataBridge.AddMethodCall<int, string>(listenerId, 5, 106, characterId, nickName);
			}

			// Token: 0x06010E98 RID: 69272 RVA: 0x00673B4F File Offset: 0x00671D4F
			public static void GetFollowingNpcNickName(int listenerId, int characterId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 5, 107, characterId);
			}

			// Token: 0x06010E99 RID: 69273 RVA: 0x00673B5D File Offset: 0x00671D5D
			public static void GetFollowingNpcNickNameId(int listenerId, int characterId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 5, 108, characterId);
			}

			// Token: 0x06010E9A RID: 69274 RVA: 0x00673B6B File Offset: 0x00671D6B
			public static void GetVillagerRoleCharacterDisplayDataList(int listenerId, List<int> characterIdList)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 5, 109, characterIdList);
			}

			// Token: 0x06010E9B RID: 69275 RVA: 0x00673B79 File Offset: 0x00671D79
			public static void GetVillagerRoleCharacterDisplayData(int listenerId, int characterId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 5, 110, characterId);
			}

			// Token: 0x06010E9C RID: 69276 RVA: 0x00673B87 File Offset: 0x00671D87
			public static void BatchSetVillagerRole(int listenerId, List<int> characterIdList, short roleTemplateId)
			{
				GameDataBridge.AddMethodCall<List<int>, short>(listenerId, 5, 111, characterIdList, roleTemplateId);
			}

			// Token: 0x06010E9D RID: 69277 RVA: 0x00673B96 File Offset: 0x00671D96
			public static void DispatchVillagerArrangement(int listenerId, int characterId, short arrangementTemplateId, Location location)
			{
				GameDataBridge.AddMethodCall<int, short, Location>(listenerId, 5, 112, characterId, arrangementTemplateId, location);
			}

			// Token: 0x06010E9E RID: 69278 RVA: 0x00673BA6 File Offset: 0x00671DA6
			public static void RecallVillager(int listenerId, int characterId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 5, 113, characterId);
			}

			// Token: 0x06010E9F RID: 69279 RVA: 0x00673BB4 File Offset: 0x00671DB4
			public static void AssignTargetItem(int listenerId, int characterId, TemplateKey targetItem)
			{
				GameDataBridge.AddMethodCall<int, TemplateKey>(listenerId, 5, 114, characterId, targetItem);
			}

			// Token: 0x06010EA0 RID: 69280 RVA: 0x00673BC3 File Offset: 0x00671DC3
			public static void GetVillagerRoleDisplayData(int listenerId, short roleTemplateId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 5, 115, roleTemplateId);
			}

			// Token: 0x06010EA1 RID: 69281 RVA: 0x00673BD1 File Offset: 0x00671DD1
			public static void AssignArrangementIncreaseOrDecrease(int listenerId, int characterId, bool isIncrease)
			{
				GameDataBridge.AddMethodCall<int, bool>(listenerId, 5, 116, characterId, isIncrease);
			}

			// Token: 0x06010EA2 RID: 69282 RVA: 0x00673BE0 File Offset: 0x00671DE0
			public static void GetAllVillagerRoleDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 117);
			}

			// Token: 0x06010EA3 RID: 69283 RVA: 0x00673BED File Offset: 0x00671DED
			public static void GetAllItems(int listenerId, ItemSourceType itemSourceType)
			{
				GameDataBridge.AddMethodCall<ItemSourceType>(listenerId, 5, 118, itemSourceType);
			}

			// Token: 0x06010EA4 RID: 69284 RVA: 0x00673BFB File Offset: 0x00671DFB
			public static void GetAllItems(int listenerId, ItemSourceType itemSourceType, bool includeResources)
			{
				GameDataBridge.AddMethodCall<ItemSourceType, bool>(listenerId, 5, 118, itemSourceType, includeResources);
			}

			// Token: 0x06010EA5 RID: 69285 RVA: 0x00673C0A File Offset: 0x00671E0A
			public static void TransferResource(int listenerId, ItemSourceType srcType, ItemSourceType destType, sbyte resourceType, int amount)
			{
				GameDataBridge.AddMethodCall<ItemSourceType, ItemSourceType, sbyte, int>(listenerId, 5, 119, srcType, destType, resourceType, amount);
			}

			// Token: 0x06010EA6 RID: 69286 RVA: 0x00673C1C File Offset: 0x00671E1C
			public static void GetVillagerRoleTipsDisplayData(int listenerId, short roleTemplateId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 5, 120, roleTemplateId);
			}

			// Token: 0x06010EA7 RID: 69287 RVA: 0x00673C2A File Offset: 0x00671E2A
			public static void SetVillagerRole(int listenerId, int charId, short roleTemplateId)
			{
				GameDataBridge.AddMethodCall<int, short>(listenerId, 5, 121, charId, roleTemplateId);
			}

			// Token: 0x06010EA8 RID: 69288 RVA: 0x00673C39 File Offset: 0x00671E39
			public static void SetVillagerMigrateWork(int listenerId, int charId, short areaId, short blockId, sbyte resourceType)
			{
				GameDataBridge.AddMethodCall<int, short, short, sbyte>(listenerId, 5, 122, charId, areaId, blockId, resourceType);
			}

			// Token: 0x06010EA9 RID: 69289 RVA: 0x00673C4B File Offset: 0x00671E4B
			public static void GetVillagerRoleNpcNickName(int listenerId, short roleTemplateId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 5, 123, roleTemplateId);
			}

			// Token: 0x06010EAA RID: 69290 RVA: 0x00673C59 File Offset: 0x00671E59
			public static void SetVillagerRoleNickName(int listenerId, short roleTemplateId, string nickName)
			{
				GameDataBridge.AddMethodCall<short, string>(listenerId, 5, 124, roleTemplateId, nickName);
			}

			// Token: 0x06010EAB RID: 69291 RVA: 0x00673C68 File Offset: 0x00671E68
			public static void GetVillagersAvailableForVillagerRole(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 125);
			}

			// Token: 0x06010EAC RID: 69292 RVA: 0x00673C75 File Offset: 0x00671E75
			public static void GetVillagersAvailableForVillagerRole(int listenerId, bool removeTaiwuGroup)
			{
				GameDataBridge.AddMethodCall<bool>(listenerId, 5, 125, removeTaiwuGroup);
			}

			// Token: 0x06010EAD RID: 69293 RVA: 0x00673C83 File Offset: 0x00671E83
			public static void GetAllResources(int listenerId, ItemSourceType itemSourceType)
			{
				GameDataBridge.AddMethodCall<ItemSourceType>(listenerId, 5, 126, itemSourceType);
			}

			// Token: 0x06010EAE RID: 69294 RVA: 0x00673C91 File Offset: 0x00671E91
			public static void GetAllSwordTombDisplayDataForDispatch(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 127);
			}

			// Token: 0x06010EAF RID: 69295 RVA: 0x00673C9E File Offset: 0x00671E9E
			public static void GetVillagerRoleCharacterSlimDisplayData(int listenerId, int characterId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 5, 128, characterId);
			}

			// Token: 0x06010EB0 RID: 69296 RVA: 0x00673CAF File Offset: 0x00671EAF
			public static void GetAllWarehouseItemsExcludeValueZero(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 129);
			}

			// Token: 0x06010EB1 RID: 69297 RVA: 0x00673CBF File Offset: 0x00671EBF
			public static void GmCmd_FillLegacyPoint(short templateId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 5, 130, templateId);
			}

			// Token: 0x06010EB2 RID: 69298 RVA: 0x00673CD0 File Offset: 0x00671ED0
			public static void GetVillagerRoleExecuteFixedActionFailReasons(int listenerId, short villagerRoleTemplateId, sbyte fixedActionType)
			{
				GameDataBridge.AddMethodCall<short, sbyte>(listenerId, 5, 131, villagerRoleTemplateId, fixedActionType);
			}

			// Token: 0x06010EB3 RID: 69299 RVA: 0x00673CE2 File Offset: 0x00671EE2
			public static void SetMerchantType(int characterId, sbyte merchantType)
			{
				GameDataBridge.AddMethodCall<int, sbyte>(-1, 5, 132, characterId, merchantType);
			}

			// Token: 0x06010EB4 RID: 69300 RVA: 0x00673CF4 File Offset: 0x00671EF4
			public static void SetMerchantType(int characterId, sbyte merchantType, bool immediate)
			{
				GameDataBridge.AddMethodCall<int, sbyte, bool>(-1, 5, 132, characterId, merchantType, immediate);
			}

			// Token: 0x06010EB5 RID: 69301 RVA: 0x00673D07 File Offset: 0x00671F07
			public static void GetMerchantType(int listenerId, int characterId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 5, 133, characterId);
			}

			// Token: 0x06010EB6 RID: 69302 RVA: 0x00673D18 File Offset: 0x00671F18
			public static void GetProfessionTipDisplayData(int listenerId, int professionId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 5, 134, professionId);
			}

			// Token: 0x06010EB7 RID: 69303 RVA: 0x00673D29 File Offset: 0x00671F29
			public static void GetExpByRereading(int listenerId, bool isInBattle, int remainingSpeedPercent)
			{
				GameDataBridge.AddMethodCall<bool, int>(listenerId, 5, 135, isInBattle, remainingSpeedPercent);
			}

			// Token: 0x06010EB8 RID: 69304 RVA: 0x00673D3B File Offset: 0x00671F3B
			public static void EnterMerchant()
			{
				GameDataBridge.AddMethodCall(-1, 5, 136);
			}

			// Token: 0x06010EB9 RID: 69305 RVA: 0x00673D4B File Offset: 0x00671F4B
			public static void GetReadingResult(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 137);
			}

			// Token: 0x06010EBA RID: 69306 RVA: 0x00673D5B File Offset: 0x00671F5B
			public static void GetVillagerTreasuryNeed(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 5, 138, charId);
			}

			// Token: 0x06010EBB RID: 69307 RVA: 0x00673D6C File Offset: 0x00671F6C
			public static void GetTreasuryNeededItemList(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 139);
			}

			// Token: 0x06010EBC RID: 69308 RVA: 0x00673D7C File Offset: 0x00671F7C
			public static void GetDyingGroupCharNames(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 140);
			}

			// Token: 0x06010EBD RID: 69309 RVA: 0x00673D8C File Offset: 0x00671F8C
			public static void GetDyingGroupCharNames(int listenerId, bool includeTaiwu)
			{
				GameDataBridge.AddMethodCall<bool>(listenerId, 5, 140, includeTaiwu);
			}

			// Token: 0x06010EBE RID: 69310 RVA: 0x00673D9D File Offset: 0x00671F9D
			public static void GetBreakBaseCostExp(int listenerId, short skillId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 5, 141, skillId);
			}

			// Token: 0x06010EBF RID: 69311 RVA: 0x00673DAE File Offset: 0x00671FAE
			public static void SetBonusRelation(int listenerId, short skillId, SkillBreakPlateIndex index, int charId, ushort relationType)
			{
				GameDataBridge.AddMethodCall<short, SkillBreakPlateIndex, int, ushort>(listenerId, 5, 142, skillId, index, charId, relationType);
			}

			// Token: 0x06010EC0 RID: 69312 RVA: 0x00673DC3 File Offset: 0x00671FC3
			public static void SetBonusExp(int listenerId, short skillId, SkillBreakPlateIndex index, int expLevel)
			{
				GameDataBridge.AddMethodCall<short, SkillBreakPlateIndex, int>(listenerId, 5, 143, skillId, index, expLevel);
			}

			// Token: 0x06010EC1 RID: 69313 RVA: 0x00673DD6 File Offset: 0x00671FD6
			public static void SetBonusItem(int listenerId, short skillId, SkillBreakPlateIndex index, ItemKey itemKey, sbyte itemSourceType)
			{
				GameDataBridge.AddMethodCall<short, SkillBreakPlateIndex, ItemKey, sbyte>(listenerId, 5, 144, skillId, index, itemKey, itemSourceType);
			}

			// Token: 0x06010EC2 RID: 69314 RVA: 0x00673DEB File Offset: 0x00671FEB
			public static void SetActivePage(int listenerId, short skillId, byte pageId, sbyte direction)
			{
				GameDataBridge.AddMethodCall<short, byte, sbyte>(listenerId, 5, 145, skillId, pageId, direction);
			}

			// Token: 0x06010EC3 RID: 69315 RVA: 0x00673DFE File Offset: 0x00671FFE
			public static void GetAvailableRelationBonuses(int listenerId, short skillId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 5, 146, skillId);
			}

			// Token: 0x06010EC4 RID: 69316 RVA: 0x00673E0F File Offset: 0x0067200F
			public static void GetVillagerCollectStorageType(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 5, 147, charId);
			}

			// Token: 0x06010EC5 RID: 69317 RVA: 0x00673E20 File Offset: 0x00672020
			public static void SetVillagerCollectStorageType(int listenerId, int charId, sbyte villagerRoleStorageType)
			{
				GameDataBridge.AddMethodCall<int, sbyte>(listenerId, 5, 148, charId, villagerRoleStorageType);
			}

			// Token: 0x06010EC6 RID: 69318 RVA: 0x00673E32 File Offset: 0x00672032
			public static void ClearBonus(int listenerId, short skillId, SkillBreakPlateIndex index)
			{
				GameDataBridge.AddMethodCall<short, SkillBreakPlateIndex>(listenerId, 5, 149, skillId, index);
			}

			// Token: 0x06010EC7 RID: 69319 RVA: 0x00673E44 File Offset: 0x00672044
			public static void TaiwuAddFeature(short featureId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 5, 150, featureId);
			}

			// Token: 0x06010EC8 RID: 69320 RVA: 0x00673E55 File Offset: 0x00672055
			public static void GetRandomLegaciesInGroup(int listenerId, sbyte groupId, int count)
			{
				GameDataBridge.AddMethodCall<sbyte, int>(listenerId, 5, 151, groupId, count);
			}

			// Token: 0x06010EC9 RID: 69321 RVA: 0x00673E67 File Offset: 0x00672067
			public static void GetGroupBabyCount(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 152);
			}

			// Token: 0x06010ECA RID: 69322 RVA: 0x00673E77 File Offset: 0x00672077
			public static void GetStrategyRoomLevel(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 153);
			}

			// Token: 0x06010ECB RID: 69323 RVA: 0x00673E87 File Offset: 0x00672087
			public static void SetBonusFriend(int listenerId, short skillId, SkillBreakPlateIndex index, int charId)
			{
				GameDataBridge.AddMethodCall<short, SkillBreakPlateIndex, int>(listenerId, 5, 154, skillId, index, charId);
			}

			// Token: 0x06010ECC RID: 69324 RVA: 0x00673E9A File Offset: 0x0067209A
			public static void GmCmd_ShowUnlockedDebateStrategy(short start, short end)
			{
				GameDataBridge.AddMethodCall<short, short>(-1, 5, 155, start, end);
			}

			// Token: 0x06010ECD RID: 69325 RVA: 0x00673EAC File Offset: 0x006720AC
			public static void GmCmd_ChangeGamePoint(bool isTaiwu, int delta)
			{
				GameDataBridge.AddMethodCall<bool, int>(-1, 5, 156, isTaiwu, delta);
			}

			// Token: 0x06010ECE RID: 69326 RVA: 0x00673EBE File Offset: 0x006720BE
			public static void GmCmd_SetForceAiBribery(bool value)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 5, 157, value);
			}

			// Token: 0x06010ECF RID: 69327 RVA: 0x00673ECF File Offset: 0x006720CF
			public static void DebateGameOver(int listenerId, bool isTaiwuWin, bool isSurrender)
			{
				GameDataBridge.AddMethodCall<bool, bool>(listenerId, 5, 158, isTaiwuWin, isSurrender);
			}

			// Token: 0x06010ED0 RID: 69328 RVA: 0x00673EE1 File Offset: 0x006720E1
			public static void DebateGameSetTaiwuAi(int listenerId, bool isAi)
			{
				GameDataBridge.AddMethodCall<bool>(listenerId, 5, 159, isAi);
			}

			// Token: 0x06010ED1 RID: 69329 RVA: 0x00673EF2 File Offset: 0x006720F2
			public static void DebateGameMakeMove(int listenerId, IntPair coordinate, bool isTaiwu, sbyte grade)
			{
				GameDataBridge.AddMethodCall<IntPair, bool, sbyte>(listenerId, 5, 160, coordinate, isTaiwu, grade);
			}

			// Token: 0x06010ED2 RID: 69330 RVA: 0x00673F05 File Offset: 0x00672105
			public static void DebateGameMakeMove(int listenerId, IntPair coordinate, bool isTaiwu, sbyte grade, bool countAsMakeMove)
			{
				GameDataBridge.AddMethodCall<IntPair, bool, sbyte, bool>(listenerId, 5, 160, coordinate, isTaiwu, grade, countAsMakeMove);
			}

			// Token: 0x06010ED3 RID: 69331 RVA: 0x00673F1A File Offset: 0x0067211A
			public static void DebateGameNextState(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 161);
			}

			// Token: 0x06010ED4 RID: 69332 RVA: 0x00673F2A File Offset: 0x0067212A
			public static void DebateGamePickSpectators(int listenerId, sbyte lifeSkillType, int npcId, bool isTaiwu, List<int> spectatorIds)
			{
				GameDataBridge.AddMethodCall<sbyte, int, bool, List<int>>(listenerId, 5, 162, lifeSkillType, npcId, isTaiwu, spectatorIds);
			}

			// Token: 0x06010ED5 RID: 69333 RVA: 0x00673F3F File Offset: 0x0067213F
			public static void DebateGameInitialize(int listenerId, sbyte type, bool isTaiwuFirst, int npcId, List<int> spectatorIds)
			{
				GameDataBridge.AddMethodCall<sbyte, bool, int, List<int>>(listenerId, 5, 163, type, isTaiwuFirst, npcId, spectatorIds);
			}

			// Token: 0x06010ED6 RID: 69334 RVA: 0x00673F54 File Offset: 0x00672154
			public static void DebateGameCastStrategy(int listenerId, int index, bool isCastedByTaiwu, List<StrategyTarget> strategyTargets)
			{
				GameDataBridge.AddMethodCall<int, bool, List<StrategyTarget>>(listenerId, 5, 164, index, isCastedByTaiwu, strategyTargets);
			}

			// Token: 0x06010ED7 RID: 69335 RVA: 0x00673F67 File Offset: 0x00672167
			public static void GmCmd_GetDebateStrategyCard(int id)
			{
				GameDataBridge.AddMethodCall<int>(-1, 5, 165, id);
			}

			// Token: 0x06010ED8 RID: 69336 RVA: 0x00673F78 File Offset: 0x00672178
			public static void GmCmd_ChangeStrategyPoint(bool isTaiwu, int delta)
			{
				GameDataBridge.AddMethodCall<bool, int>(-1, 5, 166, isTaiwu, delta);
			}

			// Token: 0x06010ED9 RID: 69337 RVA: 0x00673F8A File Offset: 0x0067218A
			public static void GmCmd_ChangeBases(bool isTaiwu, int delta)
			{
				GameDataBridge.AddMethodCall<bool, int>(-1, 5, 167, isTaiwu, delta);
			}

			// Token: 0x06010EDA RID: 69338 RVA: 0x00673F9C File Offset: 0x0067219C
			public static void GetNewUnlockedDebateStrategyList(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 168);
			}

			// Token: 0x06010EDB RID: 69339 RVA: 0x00673FAC File Offset: 0x006721AC
			public static void GmCmd_ChangePressure(bool isTaiwu, int delta)
			{
				GameDataBridge.AddMethodCall<bool, int>(-1, 5, 169, isTaiwu, delta);
			}

			// Token: 0x06010EDC RID: 69340 RVA: 0x00673FBE File Offset: 0x006721BE
			public static void DebateGameSetTaiwuSelectedCardTypes(List<sbyte> types)
			{
				GameDataBridge.AddMethodCall<List<sbyte>>(-1, 5, 170, types);
			}

			// Token: 0x06010EDD RID: 69341 RVA: 0x00673FCF File Offset: 0x006721CF
			public static void DebateGameGetTaiwuSelectedCardTypes(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 171);
			}

			// Token: 0x06010EDE RID: 69342 RVA: 0x00673FDF File Offset: 0x006721DF
			public static void GmCmd_AddAiOwnedCard(int id)
			{
				GameDataBridge.AddMethodCall<int>(-1, 5, 172, id);
			}

			// Token: 0x06010EDF RID: 69343 RVA: 0x00673FF0 File Offset: 0x006721F0
			public static void GmCmd_EmptyAiOwnedCard()
			{
				GameDataBridge.AddMethodCall(-1, 5, 173);
			}

			// Token: 0x06010EE0 RID: 69344 RVA: 0x00674000 File Offset: 0x00672200
			public static void DebateGameTryForceWin(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 174);
			}

			// Token: 0x06010EE1 RID: 69345 RVA: 0x00674010 File Offset: 0x00672210
			public static void SetVillagerDevelopWork(int listenerId, int charId, short areaId, short blockId, sbyte resourceType, short index)
			{
				GameDataBridge.AddMethodCall<int, short, short, sbyte, short>(listenerId, 5, 175, charId, areaId, blockId, resourceType, index);
			}

			// Token: 0x06010EE2 RID: 69346 RVA: 0x00674027 File Offset: 0x00672227
			public static void GetVillagerRoleCharacterDisplayDataOnPanel(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 176);
			}

			// Token: 0x06010EE3 RID: 69347 RVA: 0x00674037 File Offset: 0x00672237
			public static void GetIsTaiwuFirstByLuck(int listenerId, int npcId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 5, 177, npcId);
			}

			// Token: 0x06010EE4 RID: 69348 RVA: 0x00674048 File Offset: 0x00672248
			public static void GmCmd_AddNodeEffect(short templateId, int spectatorId)
			{
				GameDataBridge.AddMethodCall<short, int>(-1, 5, 178, templateId, spectatorId);
			}

			// Token: 0x06010EE5 RID: 69349 RVA: 0x0067405A File Offset: 0x0067225A
			public static void GetVillagerFarmerMigrateResourceSuccessRateBonus(int listenerId, int characterId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 5, 179, characterId);
			}

			// Token: 0x06010EE6 RID: 69350 RVA: 0x0067406B File Offset: 0x0067226B
			public static void GetVillagerRoleHeadTotalAuthorityCost(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 180);
			}

			// Token: 0x06010EE7 RID: 69351 RVA: 0x0067407B File Offset: 0x0067227B
			public static void GetAllChildAvailableForWork(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 181);
			}

			// Token: 0x06010EE8 RID: 69352 RVA: 0x0067408B File Offset: 0x0067228B
			public static void GetAllChildAvailableForWork(int listenerId, bool actuallyNotOccupiedOnly)
			{
				GameDataBridge.AddMethodCall<bool>(listenerId, 5, 181, actuallyNotOccupiedOnly);
			}

			// Token: 0x06010EE9 RID: 69353 RVA: 0x0067409C File Offset: 0x0067229C
			public static void GetTaiwuVillageSpaceLimitInfo(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 182);
			}

			// Token: 0x06010EEA RID: 69354 RVA: 0x006740AC File Offset: 0x006722AC
			public static void GetGroupNeiliConflictingCharDataList(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 183);
			}

			// Token: 0x06010EEB RID: 69355 RVA: 0x006740BC File Offset: 0x006722BC
			public static void DebateGameResetCards(int listenerId, bool isTaiwu, bool isManul)
			{
				GameDataBridge.AddMethodCall<bool, bool>(listenerId, 5, 184, isTaiwu, isManul);
			}

			// Token: 0x06010EEC RID: 69356 RVA: 0x006740CE File Offset: 0x006722CE
			public static void DebateGameRemoveCards(int listenerId, bool isTaiwu, List<int> removingCards)
			{
				GameDataBridge.AddMethodCall<bool, List<int>>(listenerId, 5, 185, isTaiwu, removingCards);
			}

			// Token: 0x06010EED RID: 69357 RVA: 0x006740E0 File Offset: 0x006722E0
			public static void SetLastCricketPlan(int value)
			{
				GameDataBridge.AddMethodCall<int>(-1, 5, 186, value);
			}

			// Token: 0x06010EEE RID: 69358 RVA: 0x006740F1 File Offset: 0x006722F1
			public static void RequestValidCricketPlan(int listenerId, int index, CricketCombatConfig config)
			{
				GameDataBridge.AddMethodCall<int, CricketCombatConfig>(listenerId, 5, 187, index, config);
			}

			// Token: 0x06010EEF RID: 69359 RVA: 0x00674103 File Offset: 0x00672303
			public static void SetCricketPlan(int index, ItemKey cricket, int cricketIndex)
			{
				GameDataBridge.AddMethodCall<int, ItemKey, int>(-1, 5, 188, index, cricket, cricketIndex);
			}

			// Token: 0x06010EF0 RID: 69360 RVA: 0x00674116 File Offset: 0x00672316
			public static void ClearCricketPlan(int index)
			{
				GameDataBridge.AddMethodCall<int>(-1, 5, 189, index);
			}

			// Token: 0x06010EF1 RID: 69361 RVA: 0x00674127 File Offset: 0x00672327
			public static void GetLastCricketPlan(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 190);
			}

			// Token: 0x06010EF2 RID: 69362 RVA: 0x00674137 File Offset: 0x00672337
			public static void GetAiBriberyDataOnPrepareLifeSkillCombat(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 5, 191, charId);
			}

			// Token: 0x06010EF3 RID: 69363 RVA: 0x00674148 File Offset: 0x00672348
			public static void SwapSkillBreakGrid(int listenerId, short skillId, SkillBreakPlateIndex indexA, SkillBreakPlateIndex indexB)
			{
				GameDataBridge.AddMethodCall<short, SkillBreakPlateIndex, SkillBreakPlateIndex>(listenerId, 5, 192, skillId, indexA, indexB);
			}

			// Token: 0x06010EF4 RID: 69364 RVA: 0x0067415B File Offset: 0x0067235B
			public static void GetAllItemsForSelect(int listenerId, SelectItemRules rules)
			{
				GameDataBridge.AddMethodCall<SelectItemRules>(listenerId, 5, 193, rules);
			}

			// Token: 0x06010EF5 RID: 69365 RVA: 0x0067416C File Offset: 0x0067236C
			public static void GetAllCharacterPropertyBonusData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 194);
			}

			// Token: 0x06010EF6 RID: 69366 RVA: 0x0067417C File Offset: 0x0067237C
			public static void RequestCurrEquipmentPlanId(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 195);
			}

			// Token: 0x06010EF7 RID: 69367 RVA: 0x0067418C File Offset: 0x0067238C
			public static void RemoveManualChangeEquipGroupChar(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 5, 196, charId);
			}

			// Token: 0x06010EF8 RID: 69368 RVA: 0x0067419D File Offset: 0x0067239D
			public static void RemoveManualChangeEquipGroupChar(int charId, bool keepRecord)
			{
				GameDataBridge.AddMethodCall<int, bool>(-1, 5, 196, charId, keepRecord);
			}

			// Token: 0x06010EF9 RID: 69369 RVA: 0x006741AF File Offset: 0x006723AF
			public static void AddManualChangeEquipGroupChar(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 5, 197, charId);
			}

			// Token: 0x06010EFA RID: 69370 RVA: 0x006741C0 File Offset: 0x006723C0
			public static void RequestManualChangeEquipGroupCharIds(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 198);
			}

			// Token: 0x06010EFB RID: 69371 RVA: 0x006741D0 File Offset: 0x006723D0
			public static void RequestHideSkeletonEquipSlots(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 199);
			}

			// Token: 0x06010EFC RID: 69372 RVA: 0x006741E0 File Offset: 0x006723E0
			public static void GetSkillBreakBonusSelectDisplayData(int listenerId, short skillId, int selectedCharId)
			{
				GameDataBridge.AddMethodCall<short, int>(listenerId, 5, 200, skillId, selectedCharId);
			}

			// Token: 0x06010EFD RID: 69373 RVA: 0x006741F2 File Offset: 0x006723F2
			public static void GetEnterSkillBreakPlateInfo(int listenerId, short skillId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 5, 201, skillId);
			}

			// Token: 0x06010EFE RID: 69374 RVA: 0x00674203 File Offset: 0x00672403
			public static void RemoveFavoriteCombatSkill(short skillTemplateId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 5, 202, skillTemplateId);
			}

			// Token: 0x06010EFF RID: 69375 RVA: 0x00674214 File Offset: 0x00672414
			public static void AddFavoriteCombatSkill(short skillTemplateId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 5, 203, skillTemplateId);
			}

			// Token: 0x06010F00 RID: 69376 RVA: 0x00674225 File Offset: 0x00672425
			public static void RequestReadingAndLooping(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 204);
			}

			// Token: 0x06010F01 RID: 69377 RVA: 0x00674235 File Offset: 0x00672435
			public static void RequestTaiwuResourceDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 205);
			}

			// Token: 0x06010F02 RID: 69378 RVA: 0x00674245 File Offset: 0x00672445
			public static void SetExpandPracticePanel(sbyte[] value)
			{
				GameDataBridge.AddMethodCall<sbyte[]>(-1, 5, 206, value);
			}

			// Token: 0x06010F03 RID: 69379 RVA: 0x00674256 File Offset: 0x00672456
			public static void GetExpandPracticePanel(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 207);
			}

			// Token: 0x06010F04 RID: 69380 RVA: 0x00674266 File Offset: 0x00672466
			public static void GetVillagersAvailableForWorkDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 208);
			}

			// Token: 0x06010F05 RID: 69381 RVA: 0x00674276 File Offset: 0x00672476
			public static void GetVillagersAvailableForWorkDisplayData(int listenerId, bool actuallyNotOccupiedOnly)
			{
				GameDataBridge.AddMethodCall<bool>(listenerId, 5, 208, actuallyNotOccupiedOnly);
			}

			// Token: 0x06010F06 RID: 69382 RVA: 0x00674287 File Offset: 0x00672487
			public static void SetConsummateLevelOnNeiliPage()
			{
				GameDataBridge.AddMethodCall(-1, 5, 209);
			}

			// Token: 0x06010F07 RID: 69383 RVA: 0x00674297 File Offset: 0x00672497
			public static void GetVillagersAvailableForTreeClearEnemy(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 210);
			}

			// Token: 0x06010F08 RID: 69384 RVA: 0x006742A7 File Offset: 0x006724A7
			public static void SetCombatResultSelectAllItem(bool value)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 5, 211, value);
			}

			// Token: 0x06010F09 RID: 69385 RVA: 0x006742B8 File Offset: 0x006724B8
			public static void GetReadingResultPreview(int listenerId, ItemKey targetBook, int times)
			{
				GameDataBridge.AddMethodCall<ItemKey, int>(listenerId, 5, 212, targetBook, times);
			}

			// Token: 0x06010F0A RID: 69386 RVA: 0x006742CA File Offset: 0x006724CA
			public static void AddStockItem(ItemKey itemKey, int amount)
			{
				GameDataBridge.AddMethodCall<ItemKey, int>(-1, 5, 213, itemKey, amount);
			}

			// Token: 0x06010F0B RID: 69387 RVA: 0x006742DC File Offset: 0x006724DC
			public static void GetTaiwuVillageStoragesRecordCollection(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 214);
			}

			// Token: 0x06010F0C RID: 69388 RVA: 0x006742EC File Offset: 0x006724EC
			public static void GetExchangeDisplayData(int listenerId, int targetId, EExchangeType exchangeType)
			{
				GameDataBridge.AddMethodCall<int, EExchangeType>(listenerId, 5, 215, targetId, exchangeType);
			}

			// Token: 0x06010F0D RID: 69389 RVA: 0x006742FE File Offset: 0x006724FE
			public static void ConfirmExchange(Exchange exchange)
			{
				GameDataBridge.AddMethodCall<Exchange>(-1, 5, 216, exchange);
			}

			// Token: 0x06010F0E RID: 69390 RVA: 0x0067430F File Offset: 0x0067250F
			public static void GetTreasuryNeededItemDataList(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 217);
			}

			// Token: 0x06010F0F RID: 69391 RVA: 0x0067431F File Offset: 0x0067251F
			public static void GetShopDisplayData(int listenerId, OpenShopEventArguments arg)
			{
				GameDataBridge.AddMethodCall<OpenShopEventArguments>(listenerId, 5, 218, arg);
			}

			// Token: 0x06010F10 RID: 69392 RVA: 0x00674330 File Offset: 0x00672530
			public static void ConfirmShopExchange(ShopExchange exchange)
			{
				GameDataBridge.AddMethodCall<ShopExchange>(-1, 5, 219, exchange);
			}

			// Token: 0x06010F11 RID: 69393 RVA: 0x00674341 File Offset: 0x00672541
			public static void GetLoopingViewDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 220);
			}

			// Token: 0x06010F12 RID: 69394 RVA: 0x00674351 File Offset: 0x00672551
			public static void RequestLegacyDisplayData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 5, 221, charId);
			}

			// Token: 0x06010F13 RID: 69395 RVA: 0x00674362 File Offset: 0x00672562
			public static void SelectLegacies(List<short> templateIds, List<short> noCostIds)
			{
				GameDataBridge.AddMethodCall<List<short>, List<short>>(-1, 5, 222, templateIds, noCostIds);
			}

			// Token: 0x06010F14 RID: 69396 RVA: 0x00674374 File Offset: 0x00672574
			public static void RequestFollowingCharacterList(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 223);
			}

			// Token: 0x06010F15 RID: 69397 RVA: 0x00674384 File Offset: 0x00672584
			public static void SetLuohanBreak(short combatSkillId, sbyte luohanId, sbyte behaviorType)
			{
				GameDataBridge.AddMethodCall<short, sbyte, sbyte>(-1, 5, 224, combatSkillId, luohanId, behaviorType);
			}

			// Token: 0x06010F16 RID: 69398 RVA: 0x00674397 File Offset: 0x00672597
			public static void GetAllDishes(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 225);
			}

			// Token: 0x06010F17 RID: 69399 RVA: 0x006743A7 File Offset: 0x006725A7
			public static void GetLoopReadCountDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 226);
			}

			// Token: 0x06010F18 RID: 69400 RVA: 0x006743B7 File Offset: 0x006725B7
			public static void ExpelVillagers(List<int> charIds)
			{
				GameDataBridge.AddMethodCall<List<int>>(-1, 5, 227, charIds);
			}

			// Token: 0x06010F19 RID: 69401 RVA: 0x006743C8 File Offset: 0x006725C8
			public static void GetVillagerRoleCharacterDisplayDataRolePage(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 228);
			}

			// Token: 0x06010F1A RID: 69402 RVA: 0x006743D8 File Offset: 0x006725D8
			public static void GetChangeWeaponTrickDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 229);
			}

			// Token: 0x06010F1B RID: 69403 RVA: 0x006743E8 File Offset: 0x006725E8
			public static void SetItemLocked(ItemKey itemKey, bool isLocked)
			{
				GameDataBridge.AddMethodCall<ItemKey, bool>(-1, 5, 230, itemKey, isLocked);
			}

			// Token: 0x06010F1C RID: 69404 RVA: 0x006743FA File Offset: 0x006725FA
			public static void SetItemListLocked(Inventory inventory, bool isLocked)
			{
				GameDataBridge.AddMethodCall<Inventory, bool>(-1, 5, 231, inventory, isLocked);
			}

			// Token: 0x06010F1D RID: 69405 RVA: 0x0067440C File Offset: 0x0067260C
			public static void GetReversedTaiwuVillageStoragesRecordCollection(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 232);
			}

			// Token: 0x06010F1E RID: 69406 RVA: 0x0067441C File Offset: 0x0067261C
			public static void RequestLifeSkillStrategyPlans(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 233);
			}

			// Token: 0x06010F1F RID: 69407 RVA: 0x0067442C File Offset: 0x0067262C
			public static void SetLifeSkillStrategyPlansElement(int key, ShortList value)
			{
				GameDataBridge.AddMethodCall<int, ShortList>(-1, 5, 234, key, value);
			}

			// Token: 0x06010F20 RID: 69408 RVA: 0x0067443E File Offset: 0x0067263E
			public static void GetLifeSkillCombatBeginDisplayData(int listenerId, int enemyCharId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 5, 235, enemyCharId);
			}

			// Token: 0x06010F21 RID: 69409 RVA: 0x0067444F File Offset: 0x0067264F
			public static void DebateGameTryForceWinInCombatBegin(int listenerId, sbyte lifeSkillType)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 5, 236, lifeSkillType);
			}

			// Token: 0x06010F22 RID: 69410 RVA: 0x00674460 File Offset: 0x00672660
			public static void LearnProfessionSkill(int listenerId, int professionId, int skillIndex)
			{
				GameDataBridge.AddMethodCall<int, int>(listenerId, 5, 237, professionId, skillIndex);
			}

			// Token: 0x06010F23 RID: 69411 RVA: 0x00674472 File Offset: 0x00672672
			public static void GetCricketCombatTaiwuDisplayData(int listenerId, CricketCombatConfig config, List<ItemKey> enemyCrickets)
			{
				GameDataBridge.AddMethodCall<CricketCombatConfig, List<ItemKey>>(listenerId, 5, 238, config, enemyCrickets);
			}

			// Token: 0x06010F24 RID: 69412 RVA: 0x00674484 File Offset: 0x00672684
			public static void RequestTravelerSkillsDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 239);
			}

			// Token: 0x06010F25 RID: 69413 RVA: 0x00674494 File Offset: 0x00672694
			public static void SetCricketBettingAutoBet(bool value)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 5, 240, value);
			}

			// Token: 0x06010F26 RID: 69414 RVA: 0x006744A5 File Offset: 0x006726A5
			public static void GetTotalVillagerMaintenance(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 241);
			}

			// Token: 0x06010F27 RID: 69415 RVA: 0x006744B5 File Offset: 0x006726B5
			public static void GetUnlockScrollListForDisplay(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 242);
			}

			// Token: 0x06010F28 RID: 69416 RVA: 0x006744C5 File Offset: 0x006726C5
			public static void UpdateUnlockScrollList(List<IntPair> scrollList)
			{
				GameDataBridge.AddMethodCall<List<IntPair>>(-1, 5, 243, scrollList);
			}

			// Token: 0x06010F29 RID: 69417 RVA: 0x006744D6 File Offset: 0x006726D6
			public static void GetSkillBreakPlateSkillInfo(int listenerId, short skillId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 5, 244, skillId);
			}

			// Token: 0x06010F2A RID: 69418 RVA: 0x006744E7 File Offset: 0x006726E7
			public static void SetFarmerMigrateWork(int listenerId, int charId, short areaId, sbyte resourceType)
			{
				GameDataBridge.AddMethodCall<int, short, sbyte>(listenerId, 5, 245, charId, areaId, resourceType);
			}

			// Token: 0x06010F2B RID: 69419 RVA: 0x006744FA File Offset: 0x006726FA
			public static void SetFarmerCollectResourceWork(int listenerId, int charId, short areaId, sbyte resourceType)
			{
				GameDataBridge.AddMethodCall<int, short, sbyte>(listenerId, 5, 246, charId, areaId, resourceType);
			}

			// Token: 0x06010F2C RID: 69420 RVA: 0x0067450D File Offset: 0x0067270D
			public static void GetTaiwuVillagerRoleDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 247);
			}

			// Token: 0x06010F2D RID: 69421 RVA: 0x0067451D File Offset: 0x0067271D
			public static void GetTaiwuItemMultiplyOperationDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 248);
			}

			// Token: 0x06010F2E RID: 69422 RVA: 0x0067452D File Offset: 0x0067272D
			public static void GmCmd_GenerateCricketPolymorph(short templateId, sbyte gender)
			{
				GameDataBridge.AddMethodCall<short, sbyte>(-1, 5, 249, templateId, gender);
			}

			// Token: 0x06010F2F RID: 69423 RVA: 0x0067453F File Offset: 0x0067273F
			public static void PutMaterialToCricketRoom(int listenerId, List<ItemKeyAndCount> keys, ItemSourceType sourceType)
			{
				GameDataBridge.AddMethodCall<List<ItemKeyAndCount>, ItemSourceType>(listenerId, 5, 250, keys, sourceType);
			}

			// Token: 0x06010F30 RID: 69424 RVA: 0x00674551 File Offset: 0x00672751
			public static void TakeMaterialFromCricketRoom(int listenerId, List<ItemKeyAndCount> keys, ItemSourceType sourceType)
			{
				GameDataBridge.AddMethodCall<List<ItemKeyAndCount>, ItemSourceType>(listenerId, 5, 251, keys, sourceType);
			}

			// Token: 0x06010F31 RID: 69425 RVA: 0x00674563 File Offset: 0x00672763
			public static void ChangeLegacyPointWhilePassingLegacy(int delta)
			{
				GameDataBridge.AddMethodCall<int>(-1, 5, 252, delta);
			}

			// Token: 0x06010F32 RID: 69426 RVA: 0x00674574 File Offset: 0x00672774
			public static void GetVillagersForWorkDisplayData(int listenerId, List<int> charIds)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 5, 253, charIds);
			}

			// Token: 0x06010F33 RID: 69427 RVA: 0x00674585 File Offset: 0x00672785
			public static void RequestFollowingCharacter(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 5, 254, charId);
			}

			// Token: 0x06010F34 RID: 69428 RVA: 0x00674596 File Offset: 0x00672796
			public static void FeedingCricket(int listenerId, ItemKey targetKey, List<ItemKeyAndCount> keys, ItemSourceType sourceType)
			{
				GameDataBridge.AddMethodCall<ItemKey, List<ItemKeyAndCount>, ItemSourceType>(listenerId, 5, 255, targetKey, keys, sourceType);
			}

			// Token: 0x06010F35 RID: 69429 RVA: 0x006745A9 File Offset: 0x006727A9
			public static void CricketRoomPolymorphReturn(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 5, 256, charId);
			}

			// Token: 0x06010F36 RID: 69430 RVA: 0x006745BA File Offset: 0x006727BA
			public static void CricketRoomWishingCricket(int listenerId, short wishingCricketId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 5, 257, wishingCricketId);
			}

			// Token: 0x06010F37 RID: 69431 RVA: 0x006745CB File Offset: 0x006727CB
			public static void GmCmd_GenerateCricketWishing(short wishingCricketId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 5, 258, wishingCricketId);
			}

			// Token: 0x06010F38 RID: 69432 RVA: 0x006745DC File Offset: 0x006727DC
			public static void CricketWishingCricketReturnLuckPoint()
			{
				GameDataBridge.AddMethodCall(-1, 5, 259);
			}

			// Token: 0x06010F39 RID: 69433 RVA: 0x006745EC File Offset: 0x006727EC
			public static void GetTaiwuLifeSummaryDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 260);
			}

			// Token: 0x06010F3A RID: 69434 RVA: 0x006745FC File Offset: 0x006727FC
			public static void GetTotalTaiwuLifeSummaryInfo(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 261);
			}

			// Token: 0x06010F3B RID: 69435 RVA: 0x0067460C File Offset: 0x0067280C
			public static void SetMainOperationOrder(int[] vals)
			{
				GameDataBridge.AddMethodCall<int[]>(-1, 5, 262, vals);
			}

			// Token: 0x06010F3C RID: 69436 RVA: 0x0067461D File Offset: 0x0067281D
			public static void RequestMainOperationOrder(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 263);
			}

			// Token: 0x06010F3D RID: 69437 RVA: 0x0067462D File Offset: 0x0067282D
			public static void RemoveHideSkeletonEquipSlot(sbyte slotType)
			{
				GameDataBridge.AddMethodCall<sbyte>(-1, 5, 264, slotType);
			}

			// Token: 0x06010F3E RID: 69438 RVA: 0x0067463E File Offset: 0x0067283E
			public static void AddHideSkeletonEquipSlot(sbyte slotType)
			{
				GameDataBridge.AddMethodCall<sbyte>(-1, 5, 265, slotType);
			}

			// Token: 0x06010F3F RID: 69439 RVA: 0x0067464F File Offset: 0x0067284F
			public static void RequestTaiwuEquipWithoutHideForSkeleton(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 266);
			}

			// Token: 0x06010F40 RID: 69440 RVA: 0x0067465F File Offset: 0x0067285F
			public static void RequestTaiwuNeiliProportionDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 267);
			}

			// Token: 0x06010F41 RID: 69441 RVA: 0x0067466F File Offset: 0x0067286F
			public static void SetActiveShortCut(List<int> shortCuts)
			{
				GameDataBridge.AddMethodCall<List<int>>(-1, 5, 268, shortCuts);
			}

			// Token: 0x06010F42 RID: 69442 RVA: 0x00674680 File Offset: 0x00672880
			public static void RequestActiveShortCut(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 269);
			}

			// Token: 0x06010F43 RID: 69443 RVA: 0x00674690 File Offset: 0x00672890
			public static void RecordLifeSummary(int templateId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 5, 270, templateId);
			}

			// Token: 0x06010F44 RID: 69444 RVA: 0x006746A1 File Offset: 0x006728A1
			public static void RecordLifeSummary(int templateId, int delta)
			{
				GameDataBridge.AddMethodCall<int, int>(-1, 5, 270, templateId, delta);
			}

			// Token: 0x06010F45 RID: 69445 RVA: 0x006746B3 File Offset: 0x006728B3
			public static void TaiwuInventoryHasItem(int listenerId, sbyte itemType, short templateId)
			{
				GameDataBridge.AddMethodCall<sbyte, short>(listenerId, 5, 271, itemType, templateId);
			}

			// Token: 0x06010F46 RID: 69446 RVA: 0x006746C5 File Offset: 0x006728C5
			public static void GetWineTasterBonusPercentage(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 272);
			}

			// Token: 0x06010F47 RID: 69447 RVA: 0x006746D5 File Offset: 0x006728D5
			public static void HasSectItem(int listenerId, sbyte orgTemplateId)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 5, 273, orgTemplateId);
			}

			// Token: 0x06010F48 RID: 69448 RVA: 0x006746E6 File Offset: 0x006728E6
			public static void HasSectItem(int listenerId, sbyte orgTemplateId, bool extraConditionCheck)
			{
				GameDataBridge.AddMethodCall<sbyte, bool>(listenerId, 5, 273, orgTemplateId, extraConditionCheck);
			}

			// Token: 0x06010F49 RID: 69449 RVA: 0x006746F8 File Offset: 0x006728F8
			public static void ChangeCombatSkillBreakPlate(short skillId, sbyte index)
			{
				GameDataBridge.AddMethodCall<short, sbyte>(-1, 5, 274, skillId, index);
			}

			// Token: 0x06010F4A RID: 69450 RVA: 0x0067470A File Offset: 0x0067290A
			public static void GetCombatSkillBreakPreset(int listenerId, short skillTemplateId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 5, 275, skillTemplateId);
			}

			// Token: 0x06010F4B RID: 69451 RVA: 0x0067471B File Offset: 0x0067291B
			public static void AddCricketPlan()
			{
				GameDataBridge.AddMethodCall(-1, 5, 276);
			}

			// Token: 0x06010F4C RID: 69452 RVA: 0x0067472B File Offset: 0x0067292B
			public static void CloneCricketPlan()
			{
				GameDataBridge.AddMethodCall(-1, 5, 277);
			}

			// Token: 0x06010F4D RID: 69453 RVA: 0x0067473B File Offset: 0x0067293B
			public static void DeleteCricketPlan()
			{
				GameDataBridge.AddMethodCall(-1, 5, 278);
			}

			// Token: 0x06010F4E RID: 69454 RVA: 0x0067474B File Offset: 0x0067294B
			public static void GetCricketPlanCount(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 279);
			}

			// Token: 0x06010F4F RID: 69455 RVA: 0x0067475B File Offset: 0x0067295B
			public static void GetVillagerListClassArray(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 280);
			}

			// Token: 0x06010F50 RID: 69456 RVA: 0x0067476B File Offset: 0x0067296B
			public static void GetVillagerClassesDict(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 5, 281);
			}

			// Token: 0x06010F51 RID: 69457 RVA: 0x0067477B File Offset: 0x0067297B
			public static void GetTreasuryItemNeededCharDict(int listenerId, ItemKey itemKey)
			{
				GameDataBridge.AddMethodCall<ItemKey>(listenerId, 5, 282, itemKey);
			}

			// Token: 0x06010F52 RID: 69458 RVA: 0x0067478C File Offset: 0x0067298C
			public static void TransferItemInventory(int listenerId, sbyte from, sbyte to, Inventory inventory)
			{
				GameDataBridge.AddMethodCall<sbyte, sbyte, Inventory>(listenerId, 5, 283, from, to, inventory);
			}
		}

		// Token: 0x020025F8 RID: 9720
		public static class AsyncCall
		{
			// Token: 0x06010F53 RID: 69459 RVA: 0x006747A0 File Offset: 0x006729A0
			public static void GetAllVisitedSettlements(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 0, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F54 RID: 69460 RVA: 0x006747CC File Offset: 0x006729CC
			public static void SetVillagerCollectResourceWork(IAsyncMethodRequestHandler requestHandler, int charId, short areaId, short blockId, sbyte resourceType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short, short, sbyte>(5, 1, charId, areaId, blockId, resourceType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F55 RID: 69461 RVA: 0x006747FC File Offset: 0x006729FC
			public static void SetVillagerCollectTributeWork(IAsyncMethodRequestHandler requestHandler, int charId, short areaId, short blockId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short, short>(5, 2, charId, areaId, blockId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F56 RID: 69462 RVA: 0x0067482C File Offset: 0x00672A2C
			public static void SetVillagerKeepGraveWork(IAsyncMethodRequestHandler requestHandler, int charId, short areaId, short blockId, int graveId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short, short, int>(5, 3, charId, areaId, blockId, graveId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F57 RID: 69463 RVA: 0x0067485C File Offset: 0x00672A5C
			public static void SetVillagerIdleWork(IAsyncMethodRequestHandler requestHandler, int charId, short areaId, short blockId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short, short>(5, 4, charId, areaId, blockId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F58 RID: 69464 RVA: 0x0067488C File Offset: 0x00672A8C
			public static void StopVillagerWork(IAsyncMethodRequestHandler requestHandler, short areaId, short blockId, sbyte workType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, short, sbyte>(5, 5, areaId, blockId, workType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F59 RID: 69465 RVA: 0x006748BC File Offset: 0x00672ABC
			public static void StopVillagerCollectResourceWork(IAsyncMethodRequestHandler requestHandler, short areaId, short blockId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, short>(5, 6, areaId, blockId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F5A RID: 69466 RVA: 0x006748E8 File Offset: 0x00672AE8
			public static void GetCollectResourceWorkDataList(IAsyncMethodRequestHandler requestHandler, List<Location> locationList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<Location>>(5, 7, locationList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F5B RID: 69467 RVA: 0x00674912 File Offset: 0x00672B12
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.ExpelVillager instead.", true)]
			public static void ExpelVillager(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F5C RID: 69468 RVA: 0x0067491C File Offset: 0x00672B1C
			public static void GetVillagerStatusDisplayDataList(IAsyncMethodRequestHandler requestHandler, List<int> charIds, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(5, 9, charIds, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F5D RID: 69469 RVA: 0x00674948 File Offset: 0x00672B48
			public static void GetAllVillagersStatus(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 10, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F5E RID: 69470 RVA: 0x00674974 File Offset: 0x00672B74
			public static void GetAllVillagersAvailableForWork(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 11, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F5F RID: 69471 RVA: 0x006749A0 File Offset: 0x00672BA0
			public static void GetAllVillagersAvailableForWork(IAsyncMethodRequestHandler requestHandler, bool actuallyNotOccupiedOnly, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool>(5, 11, actuallyNotOccupiedOnly, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F60 RID: 69472 RVA: 0x006749CC File Offset: 0x00672BCC
			public static void CalcResourceChangeByVillageWork(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 12, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F61 RID: 69473 RVA: 0x006749F8 File Offset: 0x00672BF8
			public static void CalcResourceChangeByBuildingEarn(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 13, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F62 RID: 69474 RVA: 0x00674A24 File Offset: 0x00672C24
			public static void CalcResourceChangeByBuildingMaintain(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 14, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F63 RID: 69475 RVA: 0x00674A50 File Offset: 0x00672C50
			public static void GetAllWarehouseItems(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 15, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F64 RID: 69476 RVA: 0x00674A7C File Offset: 0x00672C7C
			public static void GetWarehouseItemsBySubType(IAsyncMethodRequestHandler requestHandler, short itemSubType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(5, 16, itemSubType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F65 RID: 69477 RVA: 0x00674AA7 File Offset: 0x00672CA7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.SwitchEquipmentPlan instead.", true)]
			public static void SwitchEquipmentPlan(IAsyncMethodRequestHandler requestHandler, int planId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F66 RID: 69478 RVA: 0x00674AAF File Offset: 0x00672CAF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.GmCmd_AddResource instead.", true)]
			public static void GmCmd_AddResource(IAsyncMethodRequestHandler requestHandler, sbyte type, int count, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F67 RID: 69479 RVA: 0x00674AB7 File Offset: 0x00672CB7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.GmCmd_AddLegacyPoint instead.", true)]
			public static void GmCmd_AddLegacyPoint(IAsyncMethodRequestHandler requestHandler, short template, int percent, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F68 RID: 69480 RVA: 0x00674ABF File Offset: 0x00672CBF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.GmCmd_AddExp instead.", true)]
			public static void GmCmd_AddExp(IAsyncMethodRequestHandler requestHandler, int count, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F69 RID: 69481 RVA: 0x00674AC7 File Offset: 0x00672CC7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.GmCmd_SetTaiwuCombatSkillActiveState instead.", true)]
			public static void GmCmd_SetTaiwuCombatSkillActiveState(IAsyncMethodRequestHandler requestHandler, short skillTemplateId, ushort selectedPages, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F6A RID: 69482 RVA: 0x00674ACF File Offset: 0x00672CCF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.GmCmd_SetTaiwuCombatSkillActiveState instead.", true)]
			public static void GmCmd_SetTaiwuCombatSkillActiveState(IAsyncMethodRequestHandler requestHandler, short skillTemplateId, ushort selectedPages, bool bonusOn, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F6B RID: 69483 RVA: 0x00674AD7 File Offset: 0x00672CD7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.JoinGroup instead.", true)]
			public static void JoinGroup(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F6C RID: 69484 RVA: 0x00674ADF File Offset: 0x00672CDF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.JoinGroup instead.", true)]
			public static void JoinGroup(IAsyncMethodRequestHandler requestHandler, int charId, bool showNotification, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F6D RID: 69485 RVA: 0x00674AE7 File Offset: 0x00672CE7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.LeaveGroup instead.", true)]
			public static void LeaveGroup(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F6E RID: 69486 RVA: 0x00674AEF File Offset: 0x00672CEF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.LeaveGroup instead.", true)]
			public static void LeaveGroup(IAsyncMethodRequestHandler requestHandler, int charId, bool bringWards, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F6F RID: 69487 RVA: 0x00674AF7 File Offset: 0x00672CF7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.LeaveGroup instead.", true)]
			public static void LeaveGroup(IAsyncMethodRequestHandler requestHandler, int charId, bool bringWards, bool showNotification, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F70 RID: 69488 RVA: 0x00674AFF File Offset: 0x00672CFF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.LeaveGroup instead.", true)]
			public static void LeaveGroup(IAsyncMethodRequestHandler requestHandler, int charId, bool bringWards, bool showNotification, bool moveToRandomAdjacentBlock, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F71 RID: 69489 RVA: 0x00674B07 File Offset: 0x00672D07
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.CompletePassingLegacy instead.", true)]
			public static void CompletePassingLegacy(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F72 RID: 69490 RVA: 0x00674B0F File Offset: 0x00672D0F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.SelectLegacy instead.", true)]
			public static void SelectLegacy(IAsyncMethodRequestHandler requestHandler, short templateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F73 RID: 69491 RVA: 0x00674B18 File Offset: 0x00672D18
			public static void FindSuccessorCandidates(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 26, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F74 RID: 69492 RVA: 0x00674B44 File Offset: 0x00672D44
			public static void FindSuccessorCandidates(IAsyncMethodRequestHandler requestHandler, bool includeInvalid, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool>(5, 26, includeInvalid, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F75 RID: 69493 RVA: 0x00674B6F File Offset: 0x00672D6F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.ConfirmChosenSuccessor instead.", true)]
			public static void ConfirmChosenSuccessor(IAsyncMethodRequestHandler requestHandler, int newTaiwuId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F76 RID: 69494 RVA: 0x00674B77 File Offset: 0x00672D77
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.SetReferenceBook instead.", true)]
			public static void SetReferenceBook(IAsyncMethodRequestHandler requestHandler, sbyte index, ItemKey bookItemKey, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F77 RID: 69495 RVA: 0x00674B7F File Offset: 0x00672D7F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.SetReadingBook instead.", true)]
			public static void SetReadingBook(IAsyncMethodRequestHandler requestHandler, ItemKey bookItemKey, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F78 RID: 69496 RVA: 0x00674B88 File Offset: 0x00672D88
			public static void GetCurReadingStrategies(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 30, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F79 RID: 69497 RVA: 0x00674BB2 File Offset: 0x00672DB2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.SetReadingStrategy instead.", true)]
			public static void SetReadingStrategy(IAsyncMethodRequestHandler requestHandler, byte pageIndex, int strategyIndex, sbyte strategyId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F7A RID: 69498 RVA: 0x00674BBA File Offset: 0x00672DBA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.ClearPageStrategy instead.", true)]
			public static void ClearPageStrategy(IAsyncMethodRequestHandler requestHandler, byte pageIndex, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F7B RID: 69499 RVA: 0x00674BC4 File Offset: 0x00672DC4
			public static void GetRandomSelectableStrategies(IAsyncMethodRequestHandler requestHandler, byte pageIndex, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<byte>(5, 33, pageIndex, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F7C RID: 69500 RVA: 0x00674BEF File Offset: 0x00672DEF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.CheckNotInInventoryBooks instead.", true)]
			public static void CheckNotInInventoryBooks(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F7D RID: 69501 RVA: 0x00674BF8 File Offset: 0x00672DF8
			public static void GetTotalReadingProgress(IAsyncMethodRequestHandler requestHandler, int bookItemId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(5, 35, bookItemId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F7E RID: 69502 RVA: 0x00674C24 File Offset: 0x00672E24
			public static void GetCurrReadingEventBonusRate(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 36, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F7F RID: 69503 RVA: 0x00674C50 File Offset: 0x00672E50
			public static void GetCurrReadingEfficiency(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 37, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F80 RID: 69504 RVA: 0x00674C7A File Offset: 0x00672E7A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.WarehouseAdd instead.", true)]
			public static void WarehouseAdd(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, int amount, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F81 RID: 69505 RVA: 0x00674C82 File Offset: 0x00672E82
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.WarehouseRemove instead.", true)]
			public static void WarehouseRemove(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, int amount, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F82 RID: 69506 RVA: 0x00674C8A File Offset: 0x00672E8A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.WarehouseRemove instead.", true)]
			public static void WarehouseRemove(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, int amount, bool deleteItem, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F83 RID: 69507 RVA: 0x00674C92 File Offset: 0x00672E92
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.PutItemIntoWarehouse instead.", true)]
			public static void PutItemIntoWarehouse(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, int amount, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F84 RID: 69508 RVA: 0x00674C9A File Offset: 0x00672E9A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.TakeOutItemFromWarehouse instead.", true)]
			public static void TakeOutItemFromWarehouse(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, int amount, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F85 RID: 69509 RVA: 0x00674CA4 File Offset: 0x00672EA4
			public static void CanTransferItemToWarehouse(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 42, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F86 RID: 69510 RVA: 0x00674CD0 File Offset: 0x00672ED0
			public static void CalcBuildingResourceOutput(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(5, 43, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F87 RID: 69511 RVA: 0x00674CFC File Offset: 0x00672EFC
			public static void TransferAllItems(IAsyncMethodRequestHandler requestHandler, bool isToWarehouse, List<ItemKey> keyList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool, List<ItemKey>>(5, 44, isToWarehouse, keyList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F88 RID: 69512 RVA: 0x00674D28 File Offset: 0x00672F28
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.SelectCombatSkillAttainmentPanelPlan instead.", true)]
			public static void SelectCombatSkillAttainmentPanelPlan(IAsyncMethodRequestHandler requestHandler, sbyte combatSkillType, sbyte planId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F89 RID: 69513 RVA: 0x00674D30 File Offset: 0x00672F30
			public static void GetGenericGridAllocation(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 46, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F8A RID: 69514 RVA: 0x00674D5A File Offset: 0x00672F5A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.AllocateGenericGrid instead.", true)]
			public static void AllocateGenericGrid(IAsyncMethodRequestHandler requestHandler, sbyte equipType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F8B RID: 69515 RVA: 0x00674D62 File Offset: 0x00672F62
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.DeallocateGenericGrid instead.", true)]
			public static void DeallocateGenericGrid(IAsyncMethodRequestHandler requestHandler, sbyte equipType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F8C RID: 69516 RVA: 0x00674D6A File Offset: 0x00672F6A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.UpdateCombatSkillPlan instead.", true)]
			public static void UpdateCombatSkillPlan(IAsyncMethodRequestHandler requestHandler, int index, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F8D RID: 69517 RVA: 0x00674D74 File Offset: 0x00672F74
			public static void GetBreakPlateData(IAsyncMethodRequestHandler requestHandler, short skillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(5, 50, skillTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F8E RID: 69518 RVA: 0x00674DA0 File Offset: 0x00672FA0
			public static void EnterSkillBreakPlate(IAsyncMethodRequestHandler requestHandler, short skillId, ushort selectedPages, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, ushort>(5, 51, skillId, selectedPages, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F8F RID: 69519 RVA: 0x00674DCC File Offset: 0x00672FCC
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.ClearBreakPlate instead.", true)]
			public static void ClearBreakPlate(IAsyncMethodRequestHandler requestHandler, short skillId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F90 RID: 69520 RVA: 0x00674DD4 File Offset: 0x00672FD4
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.ClearBreakPlate instead.", true)]
			public static void ClearBreakPlate(IAsyncMethodRequestHandler requestHandler, short skillId, bool fromGmCmd, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F91 RID: 69521 RVA: 0x00674DDC File Offset: 0x00672FDC
			public static void SelectSkillBreakGrid(IAsyncMethodRequestHandler requestHandler, short skillId, SkillBreakPlateIndex index, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, SkillBreakPlateIndex>(5, 53, skillId, index, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F92 RID: 69522 RVA: 0x00674E08 File Offset: 0x00673008
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.EscapeToAdjacentBlock instead.", true)]
			public static void EscapeToAdjacentBlock(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F93 RID: 69523 RVA: 0x00674E10 File Offset: 0x00673010
			public static void GetCanOperateItemDisplayDataInVillage(IAsyncMethodRequestHandler requestHandler, short itemSubType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(5, 55, itemSubType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F94 RID: 69524 RVA: 0x00674E3B File Offset: 0x0067303B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.PutItemListIntoWarehouse instead.", true)]
			public static void PutItemListIntoWarehouse(IAsyncMethodRequestHandler requestHandler, List<ItemKey> keyList, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F95 RID: 69525 RVA: 0x00674E43 File Offset: 0x00673043
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.WarehouseAddList instead.", true)]
			public static void WarehouseAddList(IAsyncMethodRequestHandler requestHandler, List<ItemKey> keyList, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F96 RID: 69526 RVA: 0x00674E4B File Offset: 0x0067304B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.TakeOutItemListFromWarehouse instead.", true)]
			public static void TakeOutItemListFromWarehouse(IAsyncMethodRequestHandler requestHandler, List<ItemKey> keyList, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F97 RID: 69527 RVA: 0x00674E53 File Offset: 0x00673053
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.WarehouseRemoveList instead.", true)]
			public static void WarehouseRemoveList(IAsyncMethodRequestHandler requestHandler, List<ItemKey> keyList, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F98 RID: 69528 RVA: 0x00674E5B File Offset: 0x0067305B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.WarehouseRemoveList instead.", true)]
			public static void WarehouseRemoveList(IAsyncMethodRequestHandler requestHandler, List<ItemKey> keyList, bool deleteItem, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010F99 RID: 69529 RVA: 0x00674E64 File Offset: 0x00673064
			public static void GetTaiwuAllItems(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 60, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F9A RID: 69530 RVA: 0x00674E90 File Offset: 0x00673090
			public static void TransferItem(IAsyncMethodRequestHandler requestHandler, sbyte from, sbyte to, ItemKey itemKey, int amount, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte, sbyte, ItemKey, int>(5, 61, from, to, itemKey, amount, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F9B RID: 69531 RVA: 0x00674EC0 File Offset: 0x006730C0
			public static void TransferItem(IAsyncMethodRequestHandler requestHandler, sbyte from, sbyte to, ItemKey itemKey, int amount, bool offLine, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte, sbyte, ItemKey, int, bool>(5, 61, from, to, itemKey, amount, offLine, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F9C RID: 69532 RVA: 0x00674EF4 File Offset: 0x006730F4
			public static void GetAllTroughItems(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 62, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F9D RID: 69533 RVA: 0x00674F20 File Offset: 0x00673120
			public static void TransferItemList(IAsyncMethodRequestHandler requestHandler, sbyte from, sbyte to, List<ItemKey> keyList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte, sbyte, List<ItemKey>>(5, 63, from, to, keyList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F9E RID: 69534 RVA: 0x00674F50 File Offset: 0x00673150
			public static void GetAllTreasuryItems(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 64, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010F9F RID: 69535 RVA: 0x00674F7C File Offset: 0x0067317C
			public static void GetTotalReadingProgressList(IAsyncMethodRequestHandler requestHandler, List<int> bookItemIdList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(5, 65, bookItemIdList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FA0 RID: 69536 RVA: 0x00674FA8 File Offset: 0x006731A8
			public static void CalcResourceChangeByAutoExpand(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 66, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FA1 RID: 69537 RVA: 0x00674FD4 File Offset: 0x006731D4
			public static void CalcAutoExpandNotSatisfyIndex(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 67, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FA2 RID: 69538 RVA: 0x00675000 File Offset: 0x00673200
			public static void GetRefBonusSpeed(IAsyncMethodRequestHandler requestHandler, ItemKey refBookKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey>(5, 68, refBookKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FA3 RID: 69539 RVA: 0x0067502C File Offset: 0x0067322C
			public static void FindTaiwuBuilding(IAsyncMethodRequestHandler requestHandler, short buildingTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(5, 69, buildingTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FA4 RID: 69540 RVA: 0x00675058 File Offset: 0x00673258
			public static void FindTaiwuBuilding(IAsyncMethodRequestHandler requestHandler, short buildingTemplateId, bool checkUsable, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, bool>(5, 69, buildingTemplateId, checkUsable, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FA5 RID: 69541 RVA: 0x00675084 File Offset: 0x00673284
			public static void ChoosyGetMaterial(IAsyncMethodRequestHandler requestHandler, sbyte resourceType, int count, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte, int>(5, 70, resourceType, count, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FA6 RID: 69542 RVA: 0x006750B0 File Offset: 0x006732B0
			public static void GetCannotOperateItemDisplayDataInInventory(IAsyncMethodRequestHandler requestHandler, short itemSubType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(5, 71, itemSubType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FA7 RID: 69543 RVA: 0x006750DC File Offset: 0x006732DC
			public static void GetInventoryOverloadedGroupCharNames(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 72, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FA8 RID: 69544 RVA: 0x00675106 File Offset: 0x00673306
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.SetAutoAllocateNeiliToMax instead.", true)]
			public static void SetAutoAllocateNeiliToMax(IAsyncMethodRequestHandler requestHandler, bool value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010FA9 RID: 69545 RVA: 0x00675110 File Offset: 0x00673310
			public static void MasteredSkillWillChangePlan(IAsyncMethodRequestHandler requestHandler, short skillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(5, 74, skillTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FAA RID: 69546 RVA: 0x0067513C File Offset: 0x0067333C
			public static void GetVillagersForWork(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 75, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FAB RID: 69547 RVA: 0x00675168 File Offset: 0x00673368
			public static void GetVillagersForWork(IAsyncMethodRequestHandler requestHandler, bool includeUnlockedWorkingVillagers, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool>(5, 75, includeUnlockedWorkingVillagers, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FAC RID: 69548 RVA: 0x00675194 File Offset: 0x00673394
			public static void GetVillagersForWork(IAsyncMethodRequestHandler requestHandler, bool includeUnlockedWorkingVillagers, bool farmerFirst, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool, bool>(5, 75, includeUnlockedWorkingVillagers, farmerFirst, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FAD RID: 69549 RVA: 0x006751C0 File Offset: 0x006733C0
			public static void GetSeverelyInjuredGroupCharNames(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 76, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FAE RID: 69550 RVA: 0x006751EC File Offset: 0x006733EC
			public static void GetSeverelyInjuredGroupCharNames(IAsyncMethodRequestHandler requestHandler, bool includeTaiwu, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool>(5, 76, includeTaiwu, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FAF RID: 69551 RVA: 0x00675218 File Offset: 0x00673418
			public static void GetItemCount(IAsyncMethodRequestHandler requestHandler, sbyte itemType, short itemTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte, short>(5, 77, itemType, itemTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FB0 RID: 69552 RVA: 0x00675244 File Offset: 0x00673444
			public static void GetTaiwuVillagerMapBlockData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 78, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FB1 RID: 69553 RVA: 0x00675270 File Offset: 0x00673470
			public static void StopVillagerWorkOptional(IAsyncMethodRequestHandler requestHandler, short areaId, short blockId, sbyte workType, bool removeUnlockedState, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, short, sbyte, bool>(5, 79, areaId, blockId, workType, removeUnlockedState, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FB2 RID: 69554 RVA: 0x006752A0 File Offset: 0x006734A0
			public static void GetLegacyMaxPointByType(IAsyncMethodRequestHandler requestHandler, short legacyType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(5, 80, legacyType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FB3 RID: 69555 RVA: 0x006752CC File Offset: 0x006734CC
			public static void GetCurrReadingBanByWug(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 81, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FB4 RID: 69556 RVA: 0x006752F6 File Offset: 0x006734F6
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.GmCmd_MarkAllCarrierFullTamePoint instead.", true)]
			public static void GmCmd_MarkAllCarrierFullTamePoint(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010FB5 RID: 69557 RVA: 0x00675300 File Offset: 0x00673500
			public static void GetSelectMapBlockHasMerchantId(IAsyncMethodRequestHandler requestHandler, Location location, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Location>(5, 83, location, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FB6 RID: 69558 RVA: 0x0067532B File Offset: 0x0067352B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.ActiveReadOnce instead.", true)]
			public static void ActiveReadOnce(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010FB7 RID: 69559 RVA: 0x00675333 File Offset: 0x00673533
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.ActiveNeigongLoopingOnce instead.", true)]
			public static void ActiveNeigongLoopingOnce(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010FB8 RID: 69560 RVA: 0x0067533B File Offset: 0x0067353B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.AppendCombatSkillPlan instead.", true)]
			public static void AppendCombatSkillPlan(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010FB9 RID: 69561 RVA: 0x00675343 File Offset: 0x00673543
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.CopyCombatSkillPlan instead.", true)]
			public static void CopyCombatSkillPlan(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010FBA RID: 69562 RVA: 0x0067534B File Offset: 0x0067354B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.ClearCombatSkillPlan instead.", true)]
			public static void ClearCombatSkillPlan(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010FBB RID: 69563 RVA: 0x00675353 File Offset: 0x00673553
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.DeleteCombatSkillPlan instead.", true)]
			public static void DeleteCombatSkillPlan(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010FBC RID: 69564 RVA: 0x0067535C File Offset: 0x0067355C
			public static void GetLegacyMaxPointAndTimesListByType(IAsyncMethodRequestHandler requestHandler, short legacyType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(5, 90, legacyType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FBD RID: 69565 RVA: 0x00675387 File Offset: 0x00673587
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.SetQiArtStrategy instead.", true)]
			public static void SetQiArtStrategy(IAsyncMethodRequestHandler requestHandler, int index, sbyte templateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010FBE RID: 69566 RVA: 0x00675390 File Offset: 0x00673590
			public static void GetCurrentBookAvailableReadingStrategies(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 92, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FBF RID: 69567 RVA: 0x006753BC File Offset: 0x006735BC
			public static void GetLoopingNeigongQiArtStrategies(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 93, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FC0 RID: 69568 RVA: 0x006753E6 File Offset: 0x006735E6
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.SetReferenceCombatSkillAt instead.", true)]
			public static void SetReferenceCombatSkillAt(IAsyncMethodRequestHandler requestHandler, int index, short combatSkillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010FC1 RID: 69569 RVA: 0x006753F0 File Offset: 0x006735F0
			public static void GetLoopingNeigongQiArtStrategyDisplayDatas(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 95, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FC2 RID: 69570 RVA: 0x0067541C File Offset: 0x0067361C
			public static void GetLoopingNeigongAvailableQiArtStrategies(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 96, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FC3 RID: 69571 RVA: 0x00675446 File Offset: 0x00673646
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.ClearCurrentLoopingNeigongEvent instead.", true)]
			public static void ClearCurrentLoopingNeigongEvent(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010FC4 RID: 69572 RVA: 0x0067544E File Offset: 0x0067364E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.SetTaiwuLoopingNeigong instead.", true)]
			public static void SetTaiwuLoopingNeigong(IAsyncMethodRequestHandler requestHandler, short combatSkillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010FC5 RID: 69573 RVA: 0x00675456 File Offset: 0x00673656
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.DeleteTaiwuFeature instead.", true)]
			public static void DeleteTaiwuFeature(IAsyncMethodRequestHandler requestHandler, short featureId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010FC6 RID: 69574 RVA: 0x0067545E File Offset: 0x0067365E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.GmCmd_TaiwuActiveLoopingApply instead.", true)]
			public static void GmCmd_TaiwuActiveLoopingApply(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010FC7 RID: 69575 RVA: 0x00675468 File Offset: 0x00673668
			public static void GetIsFollowingNpcListMax(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 101, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FC8 RID: 69576 RVA: 0x00675494 File Offset: 0x00673694
			public static void GetFollowingNpcListMaxCount(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 102, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FC9 RID: 69577 RVA: 0x006754BE File Offset: 0x006736BE
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.GmCmd_FollowRandomNpc instead.", true)]
			public static void GmCmd_FollowRandomNpc(IAsyncMethodRequestHandler requestHandler, int count, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010FCA RID: 69578 RVA: 0x006754C6 File Offset: 0x006736C6
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.TaiwuFollowNpc instead.", true)]
			public static void TaiwuFollowNpc(IAsyncMethodRequestHandler requestHandler, int npcId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010FCB RID: 69579 RVA: 0x006754CE File Offset: 0x006736CE
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.TaiwuUnfollowNpc instead.", true)]
			public static void TaiwuUnfollowNpc(IAsyncMethodRequestHandler requestHandler, int characterId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010FCC RID: 69580 RVA: 0x006754D8 File Offset: 0x006736D8
			public static void SetFollowingNpcNickName(IAsyncMethodRequestHandler requestHandler, int characterId, string nickName, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, string>(5, 106, characterId, nickName, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FCD RID: 69581 RVA: 0x00675504 File Offset: 0x00673704
			public static void GetFollowingNpcNickName(IAsyncMethodRequestHandler requestHandler, int characterId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(5, 107, characterId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FCE RID: 69582 RVA: 0x00675530 File Offset: 0x00673730
			public static void GetFollowingNpcNickNameId(IAsyncMethodRequestHandler requestHandler, int characterId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(5, 108, characterId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FCF RID: 69583 RVA: 0x0067555C File Offset: 0x0067375C
			public static void GetVillagerRoleCharacterDisplayDataList(IAsyncMethodRequestHandler requestHandler, List<int> characterIdList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(5, 109, characterIdList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FD0 RID: 69584 RVA: 0x00675588 File Offset: 0x00673788
			public static void GetVillagerRoleCharacterDisplayData(IAsyncMethodRequestHandler requestHandler, int characterId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(5, 110, characterId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FD1 RID: 69585 RVA: 0x006755B4 File Offset: 0x006737B4
			public static void BatchSetVillagerRole(IAsyncMethodRequestHandler requestHandler, List<int> characterIdList, short roleTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>, short>(5, 111, characterIdList, roleTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FD2 RID: 69586 RVA: 0x006755E0 File Offset: 0x006737E0
			public static void DispatchVillagerArrangement(IAsyncMethodRequestHandler requestHandler, int characterId, short arrangementTemplateId, Location location, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short, Location>(5, 112, characterId, arrangementTemplateId, location, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FD3 RID: 69587 RVA: 0x00675610 File Offset: 0x00673810
			public static void RecallVillager(IAsyncMethodRequestHandler requestHandler, int characterId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(5, 113, characterId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FD4 RID: 69588 RVA: 0x0067563C File Offset: 0x0067383C
			public static void AssignTargetItem(IAsyncMethodRequestHandler requestHandler, int characterId, TemplateKey targetItem, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, TemplateKey>(5, 114, characterId, targetItem, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FD5 RID: 69589 RVA: 0x00675668 File Offset: 0x00673868
			public static void GetVillagerRoleDisplayData(IAsyncMethodRequestHandler requestHandler, short roleTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(5, 115, roleTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FD6 RID: 69590 RVA: 0x00675694 File Offset: 0x00673894
			public static void AssignArrangementIncreaseOrDecrease(IAsyncMethodRequestHandler requestHandler, int characterId, bool isIncrease, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, bool>(5, 116, characterId, isIncrease, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FD7 RID: 69591 RVA: 0x006756C0 File Offset: 0x006738C0
			public static void GetAllVillagerRoleDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 117, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FD8 RID: 69592 RVA: 0x006756EC File Offset: 0x006738EC
			public static void GetAllItems(IAsyncMethodRequestHandler requestHandler, ItemSourceType itemSourceType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemSourceType>(5, 118, itemSourceType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FD9 RID: 69593 RVA: 0x00675718 File Offset: 0x00673918
			public static void GetAllItems(IAsyncMethodRequestHandler requestHandler, ItemSourceType itemSourceType, bool includeResources, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemSourceType, bool>(5, 118, itemSourceType, includeResources, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FDA RID: 69594 RVA: 0x00675744 File Offset: 0x00673944
			public static void TransferResource(IAsyncMethodRequestHandler requestHandler, ItemSourceType srcType, ItemSourceType destType, sbyte resourceType, int amount, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemSourceType, ItemSourceType, sbyte, int>(5, 119, srcType, destType, resourceType, amount, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FDB RID: 69595 RVA: 0x00675774 File Offset: 0x00673974
			public static void GetVillagerRoleTipsDisplayData(IAsyncMethodRequestHandler requestHandler, short roleTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(5, 120, roleTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FDC RID: 69596 RVA: 0x006757A0 File Offset: 0x006739A0
			public static void SetVillagerRole(IAsyncMethodRequestHandler requestHandler, int charId, short roleTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short>(5, 121, charId, roleTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FDD RID: 69597 RVA: 0x006757CC File Offset: 0x006739CC
			public static void SetVillagerMigrateWork(IAsyncMethodRequestHandler requestHandler, int charId, short areaId, short blockId, sbyte resourceType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short, short, sbyte>(5, 122, charId, areaId, blockId, resourceType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FDE RID: 69598 RVA: 0x006757FC File Offset: 0x006739FC
			public static void GetVillagerRoleNpcNickName(IAsyncMethodRequestHandler requestHandler, short roleTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(5, 123, roleTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FDF RID: 69599 RVA: 0x00675828 File Offset: 0x00673A28
			public static void SetVillagerRoleNickName(IAsyncMethodRequestHandler requestHandler, short roleTemplateId, string nickName, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, string>(5, 124, roleTemplateId, nickName, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FE0 RID: 69600 RVA: 0x00675854 File Offset: 0x00673A54
			public static void GetVillagersAvailableForVillagerRole(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 125, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FE1 RID: 69601 RVA: 0x00675880 File Offset: 0x00673A80
			public static void GetVillagersAvailableForVillagerRole(IAsyncMethodRequestHandler requestHandler, bool removeTaiwuGroup, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool>(5, 125, removeTaiwuGroup, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FE2 RID: 69602 RVA: 0x006758AC File Offset: 0x00673AAC
			public static void GetAllResources(IAsyncMethodRequestHandler requestHandler, ItemSourceType itemSourceType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemSourceType>(5, 126, itemSourceType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FE3 RID: 69603 RVA: 0x006758D8 File Offset: 0x00673AD8
			public static void GetAllSwordTombDisplayDataForDispatch(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 127, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FE4 RID: 69604 RVA: 0x00675904 File Offset: 0x00673B04
			public static void GetVillagerRoleCharacterSlimDisplayData(IAsyncMethodRequestHandler requestHandler, int characterId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(5, 128, characterId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FE5 RID: 69605 RVA: 0x00675934 File Offset: 0x00673B34
			public static void GetAllWarehouseItemsExcludeValueZero(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 129, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FE6 RID: 69606 RVA: 0x00675961 File Offset: 0x00673B61
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.GmCmd_FillLegacyPoint instead.", true)]
			public static void GmCmd_FillLegacyPoint(IAsyncMethodRequestHandler requestHandler, short templateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010FE7 RID: 69607 RVA: 0x0067596C File Offset: 0x00673B6C
			public static void GetVillagerRoleExecuteFixedActionFailReasons(IAsyncMethodRequestHandler requestHandler, short villagerRoleTemplateId, sbyte fixedActionType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, sbyte>(5, 131, villagerRoleTemplateId, fixedActionType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FE8 RID: 69608 RVA: 0x0067599B File Offset: 0x00673B9B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.SetMerchantType instead.", true)]
			public static void SetMerchantType(IAsyncMethodRequestHandler requestHandler, int characterId, sbyte merchantType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010FE9 RID: 69609 RVA: 0x006759A3 File Offset: 0x00673BA3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.SetMerchantType instead.", true)]
			public static void SetMerchantType(IAsyncMethodRequestHandler requestHandler, int characterId, sbyte merchantType, bool immediate, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010FEA RID: 69610 RVA: 0x006759AC File Offset: 0x00673BAC
			public static void GetMerchantType(IAsyncMethodRequestHandler requestHandler, int characterId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(5, 133, characterId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FEB RID: 69611 RVA: 0x006759DC File Offset: 0x00673BDC
			public static void GetProfessionTipDisplayData(IAsyncMethodRequestHandler requestHandler, int professionId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(5, 134, professionId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FEC RID: 69612 RVA: 0x00675A0C File Offset: 0x00673C0C
			public static void GetExpByRereading(IAsyncMethodRequestHandler requestHandler, bool isInBattle, int remainingSpeedPercent, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool, int>(5, 135, isInBattle, remainingSpeedPercent, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FED RID: 69613 RVA: 0x00675A3B File Offset: 0x00673C3B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.EnterMerchant instead.", true)]
			public static void EnterMerchant(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010FEE RID: 69614 RVA: 0x00675A44 File Offset: 0x00673C44
			public static void GetReadingResult(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 137, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FEF RID: 69615 RVA: 0x00675A74 File Offset: 0x00673C74
			public static void GetVillagerTreasuryNeed(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(5, 138, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FF0 RID: 69616 RVA: 0x00675AA4 File Offset: 0x00673CA4
			public static void GetTreasuryNeededItemList(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 139, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FF1 RID: 69617 RVA: 0x00675AD4 File Offset: 0x00673CD4
			public static void GetDyingGroupCharNames(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 140, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FF2 RID: 69618 RVA: 0x00675B04 File Offset: 0x00673D04
			public static void GetDyingGroupCharNames(IAsyncMethodRequestHandler requestHandler, bool includeTaiwu, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool>(5, 140, includeTaiwu, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FF3 RID: 69619 RVA: 0x00675B34 File Offset: 0x00673D34
			public static void GetBreakBaseCostExp(IAsyncMethodRequestHandler requestHandler, short skillId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(5, 141, skillId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FF4 RID: 69620 RVA: 0x00675B64 File Offset: 0x00673D64
			public static void SetBonusRelation(IAsyncMethodRequestHandler requestHandler, short skillId, SkillBreakPlateIndex index, int charId, ushort relationType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, SkillBreakPlateIndex, int, ushort>(5, 142, skillId, index, charId, relationType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FF5 RID: 69621 RVA: 0x00675B98 File Offset: 0x00673D98
			public static void SetBonusExp(IAsyncMethodRequestHandler requestHandler, short skillId, SkillBreakPlateIndex index, int expLevel, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, SkillBreakPlateIndex, int>(5, 143, skillId, index, expLevel, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FF6 RID: 69622 RVA: 0x00675BCC File Offset: 0x00673DCC
			public static void SetBonusItem(IAsyncMethodRequestHandler requestHandler, short skillId, SkillBreakPlateIndex index, ItemKey itemKey, sbyte itemSourceType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, SkillBreakPlateIndex, ItemKey, sbyte>(5, 144, skillId, index, itemKey, itemSourceType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FF7 RID: 69623 RVA: 0x00675C00 File Offset: 0x00673E00
			public static void SetActivePage(IAsyncMethodRequestHandler requestHandler, short skillId, byte pageId, sbyte direction, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, byte, sbyte>(5, 145, skillId, pageId, direction, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FF8 RID: 69624 RVA: 0x00675C34 File Offset: 0x00673E34
			public static void GetAvailableRelationBonuses(IAsyncMethodRequestHandler requestHandler, short skillId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(5, 146, skillId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FF9 RID: 69625 RVA: 0x00675C64 File Offset: 0x00673E64
			public static void GetVillagerCollectStorageType(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(5, 147, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FFA RID: 69626 RVA: 0x00675C94 File Offset: 0x00673E94
			public static void SetVillagerCollectStorageType(IAsyncMethodRequestHandler requestHandler, int charId, sbyte villagerRoleStorageType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, sbyte>(5, 148, charId, villagerRoleStorageType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FFB RID: 69627 RVA: 0x00675CC4 File Offset: 0x00673EC4
			public static void ClearBonus(IAsyncMethodRequestHandler requestHandler, short skillId, SkillBreakPlateIndex index, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, SkillBreakPlateIndex>(5, 149, skillId, index, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FFC RID: 69628 RVA: 0x00675CF3 File Offset: 0x00673EF3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.TaiwuAddFeature instead.", true)]
			public static void TaiwuAddFeature(IAsyncMethodRequestHandler requestHandler, short featureId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010FFD RID: 69629 RVA: 0x00675CFC File Offset: 0x00673EFC
			public static void GetRandomLegaciesInGroup(IAsyncMethodRequestHandler requestHandler, sbyte groupId, int count, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte, int>(5, 151, groupId, count, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FFE RID: 69630 RVA: 0x00675D2C File Offset: 0x00673F2C
			public static void GetGroupBabyCount(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 152, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010FFF RID: 69631 RVA: 0x00675D5C File Offset: 0x00673F5C
			public static void GetStrategyRoomLevel(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 153, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011000 RID: 69632 RVA: 0x00675D8C File Offset: 0x00673F8C
			public static void SetBonusFriend(IAsyncMethodRequestHandler requestHandler, short skillId, SkillBreakPlateIndex index, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, SkillBreakPlateIndex, int>(5, 154, skillId, index, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011001 RID: 69633 RVA: 0x00675DBD File Offset: 0x00673FBD
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.GmCmd_ShowUnlockedDebateStrategy instead.", true)]
			public static void GmCmd_ShowUnlockedDebateStrategy(IAsyncMethodRequestHandler requestHandler, short start, short end, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011002 RID: 69634 RVA: 0x00675DC5 File Offset: 0x00673FC5
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.GmCmd_ChangeGamePoint instead.", true)]
			public static void GmCmd_ChangeGamePoint(IAsyncMethodRequestHandler requestHandler, bool isTaiwu, int delta, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011003 RID: 69635 RVA: 0x00675DCD File Offset: 0x00673FCD
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.GmCmd_SetForceAiBribery instead.", true)]
			public static void GmCmd_SetForceAiBribery(IAsyncMethodRequestHandler requestHandler, bool value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011004 RID: 69636 RVA: 0x00675DD8 File Offset: 0x00673FD8
			public static void DebateGameOver(IAsyncMethodRequestHandler requestHandler, bool isTaiwuWin, bool isSurrender, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool, bool>(5, 158, isTaiwuWin, isSurrender, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011005 RID: 69637 RVA: 0x00675E08 File Offset: 0x00674008
			public static void DebateGameSetTaiwuAi(IAsyncMethodRequestHandler requestHandler, bool isAi, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool>(5, 159, isAi, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011006 RID: 69638 RVA: 0x00675E38 File Offset: 0x00674038
			public static void DebateGameMakeMove(IAsyncMethodRequestHandler requestHandler, IntPair coordinate, bool isTaiwu, sbyte grade, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<IntPair, bool, sbyte>(5, 160, coordinate, isTaiwu, grade, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011007 RID: 69639 RVA: 0x00675E6C File Offset: 0x0067406C
			public static void DebateGameMakeMove(IAsyncMethodRequestHandler requestHandler, IntPair coordinate, bool isTaiwu, sbyte grade, bool countAsMakeMove, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<IntPair, bool, sbyte, bool>(5, 160, coordinate, isTaiwu, grade, countAsMakeMove, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011008 RID: 69640 RVA: 0x00675EA0 File Offset: 0x006740A0
			public static void DebateGameNextState(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 161, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011009 RID: 69641 RVA: 0x00675ED0 File Offset: 0x006740D0
			public static void DebateGamePickSpectators(IAsyncMethodRequestHandler requestHandler, sbyte lifeSkillType, int npcId, bool isTaiwu, List<int> spectatorIds, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte, int, bool, List<int>>(5, 162, lifeSkillType, npcId, isTaiwu, spectatorIds, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601100A RID: 69642 RVA: 0x00675F04 File Offset: 0x00674104
			public static void DebateGameInitialize(IAsyncMethodRequestHandler requestHandler, sbyte type, bool isTaiwuFirst, int npcId, List<int> spectatorIds, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte, bool, int, List<int>>(5, 163, type, isTaiwuFirst, npcId, spectatorIds, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601100B RID: 69643 RVA: 0x00675F38 File Offset: 0x00674138
			public static void DebateGameCastStrategy(IAsyncMethodRequestHandler requestHandler, int index, bool isCastedByTaiwu, List<StrategyTarget> strategyTargets, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, bool, List<StrategyTarget>>(5, 164, index, isCastedByTaiwu, strategyTargets, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601100C RID: 69644 RVA: 0x00675F69 File Offset: 0x00674169
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.GmCmd_GetDebateStrategyCard instead.", true)]
			public static void GmCmd_GetDebateStrategyCard(IAsyncMethodRequestHandler requestHandler, int id, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601100D RID: 69645 RVA: 0x00675F71 File Offset: 0x00674171
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.GmCmd_ChangeStrategyPoint instead.", true)]
			public static void GmCmd_ChangeStrategyPoint(IAsyncMethodRequestHandler requestHandler, bool isTaiwu, int delta, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601100E RID: 69646 RVA: 0x00675F79 File Offset: 0x00674179
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.GmCmd_ChangeBases instead.", true)]
			public static void GmCmd_ChangeBases(IAsyncMethodRequestHandler requestHandler, bool isTaiwu, int delta, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601100F RID: 69647 RVA: 0x00675F84 File Offset: 0x00674184
			public static void GetNewUnlockedDebateStrategyList(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 168, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011010 RID: 69648 RVA: 0x00675FB1 File Offset: 0x006741B1
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.GmCmd_ChangePressure instead.", true)]
			public static void GmCmd_ChangePressure(IAsyncMethodRequestHandler requestHandler, bool isTaiwu, int delta, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011011 RID: 69649 RVA: 0x00675FB9 File Offset: 0x006741B9
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.DebateGameSetTaiwuSelectedCardTypes instead.", true)]
			public static void DebateGameSetTaiwuSelectedCardTypes(IAsyncMethodRequestHandler requestHandler, List<sbyte> types, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011012 RID: 69650 RVA: 0x00675FC4 File Offset: 0x006741C4
			public static void DebateGameGetTaiwuSelectedCardTypes(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 171, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011013 RID: 69651 RVA: 0x00675FF1 File Offset: 0x006741F1
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.GmCmd_AddAiOwnedCard instead.", true)]
			public static void GmCmd_AddAiOwnedCard(IAsyncMethodRequestHandler requestHandler, int id, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011014 RID: 69652 RVA: 0x00675FF9 File Offset: 0x006741F9
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.GmCmd_EmptyAiOwnedCard instead.", true)]
			public static void GmCmd_EmptyAiOwnedCard(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011015 RID: 69653 RVA: 0x00676004 File Offset: 0x00674204
			public static void DebateGameTryForceWin(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 174, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011016 RID: 69654 RVA: 0x00676034 File Offset: 0x00674234
			public static void SetVillagerDevelopWork(IAsyncMethodRequestHandler requestHandler, int charId, short areaId, short blockId, sbyte resourceType, short index, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short, short, sbyte, short>(5, 175, charId, areaId, blockId, resourceType, index, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011017 RID: 69655 RVA: 0x0067606C File Offset: 0x0067426C
			public static void GetVillagerRoleCharacterDisplayDataOnPanel(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 176, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011018 RID: 69656 RVA: 0x0067609C File Offset: 0x0067429C
			public static void GetIsTaiwuFirstByLuck(IAsyncMethodRequestHandler requestHandler, int npcId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(5, 177, npcId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011019 RID: 69657 RVA: 0x006760CA File Offset: 0x006742CA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.GmCmd_AddNodeEffect instead.", true)]
			public static void GmCmd_AddNodeEffect(IAsyncMethodRequestHandler requestHandler, short templateId, int spectatorId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601101A RID: 69658 RVA: 0x006760D4 File Offset: 0x006742D4
			public static void GetVillagerFarmerMigrateResourceSuccessRateBonus(IAsyncMethodRequestHandler requestHandler, int characterId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(5, 179, characterId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601101B RID: 69659 RVA: 0x00676104 File Offset: 0x00674304
			public static void GetVillagerRoleHeadTotalAuthorityCost(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 180, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601101C RID: 69660 RVA: 0x00676134 File Offset: 0x00674334
			public static void GetAllChildAvailableForWork(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 181, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601101D RID: 69661 RVA: 0x00676164 File Offset: 0x00674364
			public static void GetAllChildAvailableForWork(IAsyncMethodRequestHandler requestHandler, bool actuallyNotOccupiedOnly, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool>(5, 181, actuallyNotOccupiedOnly, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601101E RID: 69662 RVA: 0x00676194 File Offset: 0x00674394
			public static void GetTaiwuVillageSpaceLimitInfo(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 182, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601101F RID: 69663 RVA: 0x006761C4 File Offset: 0x006743C4
			public static void GetGroupNeiliConflictingCharDataList(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 183, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011020 RID: 69664 RVA: 0x006761F4 File Offset: 0x006743F4
			public static void DebateGameResetCards(IAsyncMethodRequestHandler requestHandler, bool isTaiwu, bool isManul, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool, bool>(5, 184, isTaiwu, isManul, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011021 RID: 69665 RVA: 0x00676224 File Offset: 0x00674424
			public static void DebateGameRemoveCards(IAsyncMethodRequestHandler requestHandler, bool isTaiwu, List<int> removingCards, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool, List<int>>(5, 185, isTaiwu, removingCards, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011022 RID: 69666 RVA: 0x00676253 File Offset: 0x00674453
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.SetLastCricketPlan instead.", true)]
			public static void SetLastCricketPlan(IAsyncMethodRequestHandler requestHandler, int value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011023 RID: 69667 RVA: 0x0067625C File Offset: 0x0067445C
			public static void RequestValidCricketPlan(IAsyncMethodRequestHandler requestHandler, int index, CricketCombatConfig config, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, CricketCombatConfig>(5, 187, index, config, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011024 RID: 69668 RVA: 0x0067628B File Offset: 0x0067448B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.SetCricketPlan instead.", true)]
			public static void SetCricketPlan(IAsyncMethodRequestHandler requestHandler, int index, ItemKey cricket, int cricketIndex, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011025 RID: 69669 RVA: 0x00676293 File Offset: 0x00674493
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.ClearCricketPlan instead.", true)]
			public static void ClearCricketPlan(IAsyncMethodRequestHandler requestHandler, int index, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011026 RID: 69670 RVA: 0x0067629C File Offset: 0x0067449C
			public static void GetLastCricketPlan(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 190, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011027 RID: 69671 RVA: 0x006762CC File Offset: 0x006744CC
			public static void GetAiBriberyDataOnPrepareLifeSkillCombat(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(5, 191, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011028 RID: 69672 RVA: 0x006762FC File Offset: 0x006744FC
			public static void SwapSkillBreakGrid(IAsyncMethodRequestHandler requestHandler, short skillId, SkillBreakPlateIndex indexA, SkillBreakPlateIndex indexB, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, SkillBreakPlateIndex, SkillBreakPlateIndex>(5, 192, skillId, indexA, indexB, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011029 RID: 69673 RVA: 0x00676330 File Offset: 0x00674530
			public static void GetAllItemsForSelect(IAsyncMethodRequestHandler requestHandler, SelectItemRules rules, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<SelectItemRules>(5, 193, rules, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601102A RID: 69674 RVA: 0x00676360 File Offset: 0x00674560
			public static void GetAllCharacterPropertyBonusData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 194, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601102B RID: 69675 RVA: 0x00676390 File Offset: 0x00674590
			public static void RequestCurrEquipmentPlanId(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 195, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601102C RID: 69676 RVA: 0x006763BD File Offset: 0x006745BD
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.RemoveManualChangeEquipGroupChar instead.", true)]
			public static void RemoveManualChangeEquipGroupChar(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601102D RID: 69677 RVA: 0x006763C5 File Offset: 0x006745C5
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.RemoveManualChangeEquipGroupChar instead.", true)]
			public static void RemoveManualChangeEquipGroupChar(IAsyncMethodRequestHandler requestHandler, int charId, bool keepRecord, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601102E RID: 69678 RVA: 0x006763CD File Offset: 0x006745CD
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.AddManualChangeEquipGroupChar instead.", true)]
			public static void AddManualChangeEquipGroupChar(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601102F RID: 69679 RVA: 0x006763D8 File Offset: 0x006745D8
			public static void RequestManualChangeEquipGroupCharIds(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 198, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011030 RID: 69680 RVA: 0x00676408 File Offset: 0x00674608
			public static void RequestHideSkeletonEquipSlots(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 199, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011031 RID: 69681 RVA: 0x00676438 File Offset: 0x00674638
			public static void GetSkillBreakBonusSelectDisplayData(IAsyncMethodRequestHandler requestHandler, short skillId, int selectedCharId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, int>(5, 200, skillId, selectedCharId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011032 RID: 69682 RVA: 0x00676468 File Offset: 0x00674668
			public static void GetEnterSkillBreakPlateInfo(IAsyncMethodRequestHandler requestHandler, short skillId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(5, 201, skillId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011033 RID: 69683 RVA: 0x00676496 File Offset: 0x00674696
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.RemoveFavoriteCombatSkill instead.", true)]
			public static void RemoveFavoriteCombatSkill(IAsyncMethodRequestHandler requestHandler, short skillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011034 RID: 69684 RVA: 0x0067649E File Offset: 0x0067469E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.AddFavoriteCombatSkill instead.", true)]
			public static void AddFavoriteCombatSkill(IAsyncMethodRequestHandler requestHandler, short skillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011035 RID: 69685 RVA: 0x006764A8 File Offset: 0x006746A8
			public static void RequestReadingAndLooping(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 204, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011036 RID: 69686 RVA: 0x006764D8 File Offset: 0x006746D8
			public static void RequestTaiwuResourceDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 205, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011037 RID: 69687 RVA: 0x00676505 File Offset: 0x00674705
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.SetExpandPracticePanel instead.", true)]
			public static void SetExpandPracticePanel(IAsyncMethodRequestHandler requestHandler, sbyte[] value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011038 RID: 69688 RVA: 0x00676510 File Offset: 0x00674710
			public static void GetExpandPracticePanel(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 207, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011039 RID: 69689 RVA: 0x00676540 File Offset: 0x00674740
			public static void GetVillagersAvailableForWorkDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 208, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601103A RID: 69690 RVA: 0x00676570 File Offset: 0x00674770
			public static void GetVillagersAvailableForWorkDisplayData(IAsyncMethodRequestHandler requestHandler, bool actuallyNotOccupiedOnly, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool>(5, 208, actuallyNotOccupiedOnly, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601103B RID: 69691 RVA: 0x0067659E File Offset: 0x0067479E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.SetConsummateLevelOnNeiliPage instead.", true)]
			public static void SetConsummateLevelOnNeiliPage(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601103C RID: 69692 RVA: 0x006765A8 File Offset: 0x006747A8
			public static void GetVillagersAvailableForTreeClearEnemy(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 210, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601103D RID: 69693 RVA: 0x006765D5 File Offset: 0x006747D5
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.SetCombatResultSelectAllItem instead.", true)]
			public static void SetCombatResultSelectAllItem(IAsyncMethodRequestHandler requestHandler, bool value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601103E RID: 69694 RVA: 0x006765E0 File Offset: 0x006747E0
			public static void GetReadingResultPreview(IAsyncMethodRequestHandler requestHandler, ItemKey targetBook, int times, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey, int>(5, 212, targetBook, times, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601103F RID: 69695 RVA: 0x0067660F File Offset: 0x0067480F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.AddStockItem instead.", true)]
			public static void AddStockItem(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, int amount, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011040 RID: 69696 RVA: 0x00676618 File Offset: 0x00674818
			public static void GetTaiwuVillageStoragesRecordCollection(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 214, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011041 RID: 69697 RVA: 0x00676648 File Offset: 0x00674848
			public static void GetExchangeDisplayData(IAsyncMethodRequestHandler requestHandler, int targetId, EExchangeType exchangeType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, EExchangeType>(5, 215, targetId, exchangeType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011042 RID: 69698 RVA: 0x00676677 File Offset: 0x00674877
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.ConfirmExchange instead.", true)]
			public static void ConfirmExchange(IAsyncMethodRequestHandler requestHandler, Exchange exchange, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011043 RID: 69699 RVA: 0x00676680 File Offset: 0x00674880
			public static void GetTreasuryNeededItemDataList(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 217, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011044 RID: 69700 RVA: 0x006766B0 File Offset: 0x006748B0
			public static void GetShopDisplayData(IAsyncMethodRequestHandler requestHandler, OpenShopEventArguments arg, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<OpenShopEventArguments>(5, 218, arg, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011045 RID: 69701 RVA: 0x006766DE File Offset: 0x006748DE
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.ConfirmShopExchange instead.", true)]
			public static void ConfirmShopExchange(IAsyncMethodRequestHandler requestHandler, ShopExchange exchange, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011046 RID: 69702 RVA: 0x006766E8 File Offset: 0x006748E8
			public static void GetLoopingViewDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 220, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011047 RID: 69703 RVA: 0x00676718 File Offset: 0x00674918
			public static void RequestLegacyDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(5, 221, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011048 RID: 69704 RVA: 0x00676746 File Offset: 0x00674946
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.SelectLegacies instead.", true)]
			public static void SelectLegacies(IAsyncMethodRequestHandler requestHandler, List<short> templateIds, List<short> noCostIds, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011049 RID: 69705 RVA: 0x00676750 File Offset: 0x00674950
			public static void RequestFollowingCharacterList(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 223, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601104A RID: 69706 RVA: 0x0067677D File Offset: 0x0067497D
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.SetLuohanBreak instead.", true)]
			public static void SetLuohanBreak(IAsyncMethodRequestHandler requestHandler, short combatSkillId, sbyte luohanId, sbyte behaviorType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601104B RID: 69707 RVA: 0x00676788 File Offset: 0x00674988
			public static void GetAllDishes(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 225, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601104C RID: 69708 RVA: 0x006767B8 File Offset: 0x006749B8
			public static void GetLoopReadCountDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 226, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601104D RID: 69709 RVA: 0x006767E5 File Offset: 0x006749E5
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.ExpelVillagers instead.", true)]
			public static void ExpelVillagers(IAsyncMethodRequestHandler requestHandler, List<int> charIds, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601104E RID: 69710 RVA: 0x006767F0 File Offset: 0x006749F0
			public static void GetVillagerRoleCharacterDisplayDataRolePage(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 228, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601104F RID: 69711 RVA: 0x00676820 File Offset: 0x00674A20
			public static void GetChangeWeaponTrickDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 229, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011050 RID: 69712 RVA: 0x0067684D File Offset: 0x00674A4D
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.SetItemLocked instead.", true)]
			public static void SetItemLocked(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, bool isLocked, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011051 RID: 69713 RVA: 0x00676855 File Offset: 0x00674A55
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.SetItemListLocked instead.", true)]
			public static void SetItemListLocked(IAsyncMethodRequestHandler requestHandler, Inventory inventory, bool isLocked, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011052 RID: 69714 RVA: 0x00676860 File Offset: 0x00674A60
			public static void GetReversedTaiwuVillageStoragesRecordCollection(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 232, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011053 RID: 69715 RVA: 0x00676890 File Offset: 0x00674A90
			public static void RequestLifeSkillStrategyPlans(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 233, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011054 RID: 69716 RVA: 0x006768BD File Offset: 0x00674ABD
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.SetLifeSkillStrategyPlansElement instead.", true)]
			public static void SetLifeSkillStrategyPlansElement(IAsyncMethodRequestHandler requestHandler, int key, ShortList value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011055 RID: 69717 RVA: 0x006768C8 File Offset: 0x00674AC8
			public static void GetLifeSkillCombatBeginDisplayData(IAsyncMethodRequestHandler requestHandler, int enemyCharId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(5, 235, enemyCharId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011056 RID: 69718 RVA: 0x006768F8 File Offset: 0x00674AF8
			public static void DebateGameTryForceWinInCombatBegin(IAsyncMethodRequestHandler requestHandler, sbyte lifeSkillType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(5, 236, lifeSkillType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011057 RID: 69719 RVA: 0x00676928 File Offset: 0x00674B28
			public static void LearnProfessionSkill(IAsyncMethodRequestHandler requestHandler, int professionId, int skillIndex, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int>(5, 237, professionId, skillIndex, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011058 RID: 69720 RVA: 0x00676958 File Offset: 0x00674B58
			public static void GetCricketCombatTaiwuDisplayData(IAsyncMethodRequestHandler requestHandler, CricketCombatConfig config, List<ItemKey> enemyCrickets, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<CricketCombatConfig, List<ItemKey>>(5, 238, config, enemyCrickets, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011059 RID: 69721 RVA: 0x00676988 File Offset: 0x00674B88
			public static void RequestTravelerSkillsDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 239, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601105A RID: 69722 RVA: 0x006769B5 File Offset: 0x00674BB5
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.SetCricketBettingAutoBet instead.", true)]
			public static void SetCricketBettingAutoBet(IAsyncMethodRequestHandler requestHandler, bool value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601105B RID: 69723 RVA: 0x006769C0 File Offset: 0x00674BC0
			public static void GetTotalVillagerMaintenance(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 241, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601105C RID: 69724 RVA: 0x006769F0 File Offset: 0x00674BF0
			public static void GetUnlockScrollListForDisplay(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 242, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601105D RID: 69725 RVA: 0x00676A1D File Offset: 0x00674C1D
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.UpdateUnlockScrollList instead.", true)]
			public static void UpdateUnlockScrollList(IAsyncMethodRequestHandler requestHandler, List<IntPair> scrollList, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601105E RID: 69726 RVA: 0x00676A28 File Offset: 0x00674C28
			public static void GetSkillBreakPlateSkillInfo(IAsyncMethodRequestHandler requestHandler, short skillId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(5, 244, skillId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601105F RID: 69727 RVA: 0x00676A58 File Offset: 0x00674C58
			public static void SetFarmerMigrateWork(IAsyncMethodRequestHandler requestHandler, int charId, short areaId, sbyte resourceType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short, sbyte>(5, 245, charId, areaId, resourceType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011060 RID: 69728 RVA: 0x00676A8C File Offset: 0x00674C8C
			public static void SetFarmerCollectResourceWork(IAsyncMethodRequestHandler requestHandler, int charId, short areaId, sbyte resourceType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short, sbyte>(5, 246, charId, areaId, resourceType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011061 RID: 69729 RVA: 0x00676AC0 File Offset: 0x00674CC0
			public static void GetTaiwuVillagerRoleDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 247, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011062 RID: 69730 RVA: 0x00676AF0 File Offset: 0x00674CF0
			public static void GetTaiwuItemMultiplyOperationDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 248, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011063 RID: 69731 RVA: 0x00676B1D File Offset: 0x00674D1D
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.GmCmd_GenerateCricketPolymorph instead.", true)]
			public static void GmCmd_GenerateCricketPolymorph(IAsyncMethodRequestHandler requestHandler, short templateId, sbyte gender, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011064 RID: 69732 RVA: 0x00676B28 File Offset: 0x00674D28
			public static void PutMaterialToCricketRoom(IAsyncMethodRequestHandler requestHandler, List<ItemKeyAndCount> keys, ItemSourceType sourceType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<ItemKeyAndCount>, ItemSourceType>(5, 250, keys, sourceType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011065 RID: 69733 RVA: 0x00676B58 File Offset: 0x00674D58
			public static void TakeMaterialFromCricketRoom(IAsyncMethodRequestHandler requestHandler, List<ItemKeyAndCount> keys, ItemSourceType sourceType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<ItemKeyAndCount>, ItemSourceType>(5, 251, keys, sourceType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011066 RID: 69734 RVA: 0x00676B87 File Offset: 0x00674D87
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.ChangeLegacyPointWhilePassingLegacy instead.", true)]
			public static void ChangeLegacyPointWhilePassingLegacy(IAsyncMethodRequestHandler requestHandler, int delta, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011067 RID: 69735 RVA: 0x00676B90 File Offset: 0x00674D90
			public static void GetVillagersForWorkDisplayData(IAsyncMethodRequestHandler requestHandler, List<int> charIds, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(5, 253, charIds, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011068 RID: 69736 RVA: 0x00676BC0 File Offset: 0x00674DC0
			public static void RequestFollowingCharacter(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(5, 254, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011069 RID: 69737 RVA: 0x00676BF0 File Offset: 0x00674DF0
			public static void FeedingCricket(IAsyncMethodRequestHandler requestHandler, ItemKey targetKey, List<ItemKeyAndCount> keys, ItemSourceType sourceType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey, List<ItemKeyAndCount>, ItemSourceType>(5, 255, targetKey, keys, sourceType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601106A RID: 69738 RVA: 0x00676C24 File Offset: 0x00674E24
			public static void CricketRoomPolymorphReturn(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(5, 256, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601106B RID: 69739 RVA: 0x00676C54 File Offset: 0x00674E54
			public static void CricketRoomWishingCricket(IAsyncMethodRequestHandler requestHandler, short wishingCricketId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(5, 257, wishingCricketId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601106C RID: 69740 RVA: 0x00676C82 File Offset: 0x00674E82
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.GmCmd_GenerateCricketWishing instead.", true)]
			public static void GmCmd_GenerateCricketWishing(IAsyncMethodRequestHandler requestHandler, short wishingCricketId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601106D RID: 69741 RVA: 0x00676C8A File Offset: 0x00674E8A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.CricketWishingCricketReturnLuckPoint instead.", true)]
			public static void CricketWishingCricketReturnLuckPoint(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601106E RID: 69742 RVA: 0x00676C94 File Offset: 0x00674E94
			public static void GetTaiwuLifeSummaryDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 260, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601106F RID: 69743 RVA: 0x00676CC4 File Offset: 0x00674EC4
			public static void GetTotalTaiwuLifeSummaryInfo(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 261, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011070 RID: 69744 RVA: 0x00676CF1 File Offset: 0x00674EF1
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.SetMainOperationOrder instead.", true)]
			public static void SetMainOperationOrder(IAsyncMethodRequestHandler requestHandler, int[] vals, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011071 RID: 69745 RVA: 0x00676CFC File Offset: 0x00674EFC
			public static void RequestMainOperationOrder(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 263, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011072 RID: 69746 RVA: 0x00676D29 File Offset: 0x00674F29
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.RemoveHideSkeletonEquipSlot instead.", true)]
			public static void RemoveHideSkeletonEquipSlot(IAsyncMethodRequestHandler requestHandler, sbyte slotType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011073 RID: 69747 RVA: 0x00676D31 File Offset: 0x00674F31
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.AddHideSkeletonEquipSlot instead.", true)]
			public static void AddHideSkeletonEquipSlot(IAsyncMethodRequestHandler requestHandler, sbyte slotType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011074 RID: 69748 RVA: 0x00676D3C File Offset: 0x00674F3C
			public static void RequestTaiwuEquipWithoutHideForSkeleton(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 266, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011075 RID: 69749 RVA: 0x00676D6C File Offset: 0x00674F6C
			public static void RequestTaiwuNeiliProportionDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 267, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011076 RID: 69750 RVA: 0x00676D99 File Offset: 0x00674F99
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.SetActiveShortCut instead.", true)]
			public static void SetActiveShortCut(IAsyncMethodRequestHandler requestHandler, List<int> shortCuts, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011077 RID: 69751 RVA: 0x00676DA4 File Offset: 0x00674FA4
			public static void RequestActiveShortCut(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 269, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011078 RID: 69752 RVA: 0x00676DD1 File Offset: 0x00674FD1
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.RecordLifeSummary instead.", true)]
			public static void RecordLifeSummary(IAsyncMethodRequestHandler requestHandler, int templateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011079 RID: 69753 RVA: 0x00676DD9 File Offset: 0x00674FD9
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.RecordLifeSummary instead.", true)]
			public static void RecordLifeSummary(IAsyncMethodRequestHandler requestHandler, int templateId, int delta, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601107A RID: 69754 RVA: 0x00676DE4 File Offset: 0x00674FE4
			public static void TaiwuInventoryHasItem(IAsyncMethodRequestHandler requestHandler, sbyte itemType, short templateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte, short>(5, 271, itemType, templateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601107B RID: 69755 RVA: 0x00676E14 File Offset: 0x00675014
			public static void GetWineTasterBonusPercentage(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 272, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601107C RID: 69756 RVA: 0x00676E44 File Offset: 0x00675044
			public static void HasSectItem(IAsyncMethodRequestHandler requestHandler, sbyte orgTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(5, 273, orgTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601107D RID: 69757 RVA: 0x00676E74 File Offset: 0x00675074
			public static void HasSectItem(IAsyncMethodRequestHandler requestHandler, sbyte orgTemplateId, bool extraConditionCheck, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte, bool>(5, 273, orgTemplateId, extraConditionCheck, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601107E RID: 69758 RVA: 0x00676EA3 File Offset: 0x006750A3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.ChangeCombatSkillBreakPlate instead.", true)]
			public static void ChangeCombatSkillBreakPlate(IAsyncMethodRequestHandler requestHandler, short skillId, sbyte index, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601107F RID: 69759 RVA: 0x00676EAC File Offset: 0x006750AC
			public static void GetCombatSkillBreakPreset(IAsyncMethodRequestHandler requestHandler, short skillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(5, 275, skillTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011080 RID: 69760 RVA: 0x00676EDA File Offset: 0x006750DA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.AddCricketPlan instead.", true)]
			public static void AddCricketPlan(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011081 RID: 69761 RVA: 0x00676EE2 File Offset: 0x006750E2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.CloneCricketPlan instead.", true)]
			public static void CloneCricketPlan(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011082 RID: 69762 RVA: 0x00676EEA File Offset: 0x006750EA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuDomainMethod.Call.DeleteCricketPlan instead.", true)]
			public static void DeleteCricketPlan(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011083 RID: 69763 RVA: 0x00676EF4 File Offset: 0x006750F4
			public static void GetCricketPlanCount(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 279, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011084 RID: 69764 RVA: 0x00676F24 File Offset: 0x00675124
			public static void GetVillagerListClassArray(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 280, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011085 RID: 69765 RVA: 0x00676F54 File Offset: 0x00675154
			public static void GetVillagerClassesDict(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(5, 281, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011086 RID: 69766 RVA: 0x00676F84 File Offset: 0x00675184
			public static void GetTreasuryItemNeededCharDict(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey>(5, 282, itemKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011087 RID: 69767 RVA: 0x00676FB4 File Offset: 0x006751B4
			public static void TransferItemInventory(IAsyncMethodRequestHandler requestHandler, sbyte from, sbyte to, Inventory inventory, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte, sbyte, Inventory>(5, 283, from, to, inventory, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}
		}
	}
}
