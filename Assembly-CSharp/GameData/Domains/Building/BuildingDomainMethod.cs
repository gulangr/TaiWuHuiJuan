using System;
using System.Collections.Generic;
using GameData.Domains.Character;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Utilities;

namespace GameData.Domains.Building
{
	// Token: 0x02000FCF RID: 4047
	public static class BuildingDomainMethod
	{
		// Token: 0x02002619 RID: 9753
		public static class Call
		{
			// Token: 0x060118A5 RID: 71845 RVA: 0x006827C3 File Offset: 0x006809C3
			public static void SetShopManager(BuildingBlockKey blockKey, sbyte index, int charId)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, sbyte, int>(-1, 9, 0, blockKey, index, charId);
			}

			// Token: 0x060118A6 RID: 71846 RVA: 0x006827D3 File Offset: 0x006809D3
			public static void SetCollectBuildingResourceType(BuildingBlockKey blockKey, sbyte resourceType)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, sbyte>(-1, 9, 1, blockKey, resourceType);
			}

			// Token: 0x060118A7 RID: 71847 RVA: 0x006827E2 File Offset: 0x006809E2
			public static void ClearBuildingBlockEarningsData(BuildingBlockKey key, bool isPawnShop)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, bool>(-1, 9, 2, key, isPawnShop);
			}

			// Token: 0x060118A8 RID: 71848 RVA: 0x006827F1 File Offset: 0x006809F1
			public static void GetBuildingEarningData(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 3, blockKey);
			}

			// Token: 0x060118A9 RID: 71849 RVA: 0x006827FF File Offset: 0x006809FF
			public static void GetBuildingOperatesData(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 4, blockKey);
			}

			// Token: 0x060118AA RID: 71850 RVA: 0x0068280D File Offset: 0x00680A0D
			public static void GetBuildingBuildPeopleAttainments(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 5, blockKey);
			}

			// Token: 0x060118AB RID: 71851 RVA: 0x0068281B File Offset: 0x00680A1B
			public static void AcceptBuildingBlockCollectEarning(int listenerId, BuildingBlockKey key, int earningDataIndex, bool isPutInInventory)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, int, bool>(listenerId, 9, 6, key, earningDataIndex, isPutInInventory);
			}

			// Token: 0x060118AC RID: 71852 RVA: 0x0068282B File Offset: 0x00680A2B
			public static void AcceptBuildingBlockCollectEarning(int listenerId, BuildingBlockKey key, int earningDataIndex, bool isPutInInventory, bool isSetData)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, int, bool, bool>(listenerId, 9, 6, key, earningDataIndex, isPutInInventory, isSetData);
			}

			// Token: 0x060118AD RID: 71853 RVA: 0x0068283D File Offset: 0x00680A3D
			public static void AcceptBuildingBlockCollectEarning(int listenerId, BuildingBlockKey key, int earningDataIndex, bool isPutInInventory, bool isSetData, bool isCostMoney)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, int, bool, bool, bool>(listenerId, 9, 6, key, earningDataIndex, isPutInInventory, isSetData, isCostMoney);
			}

			// Token: 0x060118AE RID: 71854 RVA: 0x00682851 File Offset: 0x00680A51
			public static void AcceptBuildingBlockCollectEarningQuick(BuildingBlockKey key, bool isPutInInventory)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, bool>(-1, 9, 7, key, isPutInInventory);
			}

			// Token: 0x060118AF RID: 71855 RVA: 0x00682860 File Offset: 0x00680A60
			public static void AcceptBuildingBlockRecruitPeople(int listenerId, BuildingBlockKey key, int earningDataIndex)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, int>(listenerId, 9, 8, key, earningDataIndex);
			}

			// Token: 0x060118B0 RID: 71856 RVA: 0x0068286F File Offset: 0x00680A6F
			public static void AcceptBuildingBlockRecruitPeople(int listenerId, BuildingBlockKey key, int earningDataIndex, bool isSetData)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, int, bool>(listenerId, 9, 8, key, earningDataIndex, isSetData);
			}

			// Token: 0x060118B1 RID: 71857 RVA: 0x0068287F File Offset: 0x00680A7F
			public static void AcceptBuildingBlockRecruitPeopleQuick(int listenerId, BuildingBlockKey key)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 9, key);
			}

			// Token: 0x060118B2 RID: 71858 RVA: 0x0068288E File Offset: 0x00680A8E
			public static void ShopBuildingSoldItemReceive(BuildingBlockKey key, int earningDataIndex)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, int>(-1, 9, 10, key, earningDataIndex);
			}

			// Token: 0x060118B3 RID: 71859 RVA: 0x0068289E File Offset: 0x00680A9E
			public static void ShopBuildingSoldItemReceive(BuildingBlockKey key, int earningDataIndex, bool isSetData)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, int, bool>(-1, 9, 10, key, earningDataIndex, isSetData);
			}

			// Token: 0x060118B4 RID: 71860 RVA: 0x006828AF File Offset: 0x00680AAF
			public static void ShopBuildingSoldItemReceiveQuick(BuildingBlockKey key)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(-1, 9, 11, key);
			}

			// Token: 0x060118B5 RID: 71861 RVA: 0x006828BE File Offset: 0x00680ABE
			public static void QuickCollectShopItem(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 12);
			}

			// Token: 0x060118B6 RID: 71862 RVA: 0x006828CC File Offset: 0x00680ACC
			public static void QuickCollectShopItemCount(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 13);
			}

			// Token: 0x060118B7 RID: 71863 RVA: 0x006828DA File Offset: 0x00680ADA
			public static void QuickCollectShopSoldItem()
			{
				GameDataBridge.AddMethodCall(-1, 9, 14);
			}

			// Token: 0x060118B8 RID: 71864 RVA: 0x006828E8 File Offset: 0x00680AE8
			public static void QuickCollectShopSoldItemCount(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 15);
			}

			// Token: 0x060118B9 RID: 71865 RVA: 0x006828F6 File Offset: 0x00680AF6
			public static void QuickRecruitPeople(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 16);
			}

			// Token: 0x060118BA RID: 71866 RVA: 0x00682904 File Offset: 0x00680B04
			public static void QuickRecruitPeopleCount(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 17);
			}

			// Token: 0x060118BB RID: 71867 RVA: 0x00682912 File Offset: 0x00680B12
			public static void QuickCollectBuildingEarn()
			{
				GameDataBridge.AddMethodCall(-1, 9, 18);
			}

			// Token: 0x060118BC RID: 71868 RVA: 0x00682920 File Offset: 0x00680B20
			public static void QuickCollectBuildingEarnCount(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 19);
			}

			// Token: 0x060118BD RID: 71869 RVA: 0x0068292E File Offset: 0x00680B2E
			public static void AddFixBook(BuildingBlockKey key, ItemKey itemKey, ItemSourceType itemSourceType)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, ItemKey, ItemSourceType>(-1, 9, 20, key, itemKey, itemSourceType);
			}

			// Token: 0x060118BE RID: 71870 RVA: 0x0068293F File Offset: 0x00680B3F
			public static void ChangeFixBook(int listenerId, BuildingBlockKey key, ItemKey itemKey, ItemSourceType itemSourceType)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, ItemKey, ItemSourceType>(listenerId, 9, 21, key, itemKey, itemSourceType);
			}

			// Token: 0x060118BF RID: 71871 RVA: 0x00682950 File Offset: 0x00680B50
			public static void ReceiveFixBook(BuildingBlockKey key, bool isPutInInventory)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, bool>(-1, 9, 22, key, isPutInInventory);
			}

			// Token: 0x060118C0 RID: 71872 RVA: 0x00682960 File Offset: 0x00680B60
			public static void GetFixBookProgress(int listenerId, BuildingBlockKey key)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 23, key);
			}

			// Token: 0x060118C1 RID: 71873 RVA: 0x0068296F File Offset: 0x00680B6F
			public static void SetTeaHorseCaravanState(sbyte state)
			{
				GameDataBridge.AddMethodCall<sbyte>(-1, 9, 24, state);
			}

			// Token: 0x060118C2 RID: 71874 RVA: 0x0068297E File Offset: 0x00680B7E
			public static void ExchangeItemToReplenishment(List<ItemKey> carryItems, List<ItemKey> gainItems)
			{
				GameDataBridge.AddMethodCall<List<ItemKey>, List<ItemKey>>(-1, 9, 25, carryItems, gainItems);
			}

			// Token: 0x060118C3 RID: 71875 RVA: 0x0068298E File Offset: 0x00680B8E
			public static void StartSearchReplenishment()
			{
				GameDataBridge.AddMethodCall(-1, 9, 26);
			}

			// Token: 0x060118C4 RID: 71876 RVA: 0x0068299C File Offset: 0x00680B9C
			public static void QuickGetExchangeItem()
			{
				GameDataBridge.AddMethodCall(-1, 9, 27);
			}

			// Token: 0x060118C5 RID: 71877 RVA: 0x006829AA File Offset: 0x00680BAA
			public static void QuickGetExchangeItem(ItemSourceType source)
			{
				GameDataBridge.AddMethodCall<ItemSourceType>(-1, 9, 27, source);
			}

			// Token: 0x060118C6 RID: 71878 RVA: 0x006829B9 File Offset: 0x00680BB9
			public static void GetShrineDisplayData(int listenerId, short areaId, short blockId, short buildingBlockIndex)
			{
				GameDataBridge.AddMethodCall<short, short, short>(listenerId, 9, 28, areaId, blockId, buildingBlockIndex);
			}

			// Token: 0x060118C7 RID: 71879 RVA: 0x006829CA File Offset: 0x00680BCA
			public static void TeachSkill(int characterId, SkillQualificationBonus bonus)
			{
				GameDataBridge.AddMethodCall<int, SkillQualificationBonus>(-1, 9, 29, characterId, bonus);
			}

			// Token: 0x060118C8 RID: 71880 RVA: 0x006829DA File Offset: 0x00680BDA
			public static void CricketCollectionAdd(int index, bool isCricket, ItemKey itemKey)
			{
				GameDataBridge.AddMethodCall<int, bool, ItemKey>(-1, 9, 30, index, isCricket, itemKey);
			}

			// Token: 0x060118C9 RID: 71881 RVA: 0x006829EB File Offset: 0x00680BEB
			public static void CricketCollectionRemove(int index, bool isCricket)
			{
				GameDataBridge.AddMethodCall<int, bool>(-1, 9, 31, index, isCricket);
			}

			// Token: 0x060118CA RID: 71882 RVA: 0x006829FB File Offset: 0x00680BFB
			public static void GetCollectionCrickets(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 32);
			}

			// Token: 0x060118CB RID: 71883 RVA: 0x00682A09 File Offset: 0x00680C09
			public static void GetCollectionJars(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 33);
			}

			// Token: 0x060118CC RID: 71884 RVA: 0x00682A17 File Offset: 0x00680C17
			public static void GetCollectionCricketRegen(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 34);
			}

			// Token: 0x060118CD RID: 71885 RVA: 0x00682A25 File Offset: 0x00680C25
			public static void GetAuthorityGain(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 35);
			}

			// Token: 0x060118CE RID: 71886 RVA: 0x00682A33 File Offset: 0x00680C33
			public static void GmCmd_BuildImmediately(int listenerId, short buildingTemplateId, BuildingBlockKey blockKey, sbyte level)
			{
				GameDataBridge.AddMethodCall<short, BuildingBlockKey, sbyte>(listenerId, 9, 36, buildingTemplateId, blockKey, level);
			}

			// Token: 0x060118CF RID: 71887 RVA: 0x00682A44 File Offset: 0x00680C44
			public static void GmCmd_RemoveBuildingImmediately(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 37, blockKey);
			}

			// Token: 0x060118D0 RID: 71888 RVA: 0x00682A53 File Offset: 0x00680C53
			public static void StartMakeItem(int listenerId, StartMakeArguments startMakeArguments)
			{
				GameDataBridge.AddMethodCall<StartMakeArguments>(listenerId, 9, 38, startMakeArguments);
			}

			// Token: 0x060118D1 RID: 71889 RVA: 0x00682A62 File Offset: 0x00680C62
			public static void CheckMakeCondition(int listenerId, MakeConditionArguments makeConditionArguments)
			{
				GameDataBridge.AddMethodCall<MakeConditionArguments>(listenerId, 9, 39, makeConditionArguments);
			}

			// Token: 0x060118D2 RID: 71890 RVA: 0x00682A71 File Offset: 0x00680C71
			public static void GetMakeItems(int listenerId, BuildingBlockKey buildingBlockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 40, buildingBlockKey);
			}

			// Token: 0x060118D3 RID: 71891 RVA: 0x00682A80 File Offset: 0x00680C80
			public static void GetMakingItemData(int listenerId, BuildingBlockKey buildingBlockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 41, buildingBlockKey);
			}

			// Token: 0x060118D4 RID: 71892 RVA: 0x00682A8F File Offset: 0x00680C8F
			public static void CheckRepairConditionIsMeet(int listenerId, int charId, ItemKey toolKey, ItemKey itemKey, BuildingBlockKey buildingBlockKey)
			{
				GameDataBridge.AddMethodCall<int, ItemKey, ItemKey, BuildingBlockKey>(listenerId, 9, 42, charId, toolKey, itemKey, buildingBlockKey);
			}

			// Token: 0x060118D5 RID: 71893 RVA: 0x00682AA2 File Offset: 0x00680CA2
			public static void AddItemPoison(int listenerId, int charId, ItemDisplayData tool, ItemDisplayData target, ItemDisplayData[] poisons, List<ItemDisplayData> condensePoisonItemList)
			{
				GameDataBridge.AddMethodCall<int, ItemDisplayData, ItemDisplayData, ItemDisplayData[], List<ItemDisplayData>>(listenerId, 9, 43, charId, tool, target, poisons, condensePoisonItemList);
			}

			// Token: 0x060118D6 RID: 71894 RVA: 0x00682AB8 File Offset: 0x00680CB8
			public static void CheckAddPoisonCondition(int listenerId, int charId, ItemKey toolKey, ItemKey targetKey, ItemKey[] poisonKeys, BuildingBlockKey buildingBlockKey, FullPoisonEffects tempPoisonEffects)
			{
				GameDataBridge.AddMethodCall<int, ItemKey, ItemKey, ItemKey[], BuildingBlockKey, FullPoisonEffects>(listenerId, 9, 44, charId, toolKey, targetKey, poisonKeys, buildingBlockKey, tempPoisonEffects);
			}

			// Token: 0x060118D7 RID: 71895 RVA: 0x00682ADA File Offset: 0x00680CDA
			public static void RemoveItemPoison(int listenerId, int charId, ItemDisplayData tool, ItemDisplayData target, ItemDisplayData[] medicines, bool isExtract)
			{
				GameDataBridge.AddMethodCall<int, ItemDisplayData, ItemDisplayData, ItemDisplayData[], bool>(listenerId, 9, 45, charId, tool, target, medicines, isExtract);
			}

			// Token: 0x060118D8 RID: 71896 RVA: 0x00682AF0 File Offset: 0x00680CF0
			public static void CheckRemovePoisonCondition(int listenerId, int charId, ItemKey toolKey, ItemKey targetKey, ItemKey[] medicineKeys, BuildingBlockKey buildingBlockKey, bool isExtract)
			{
				GameDataBridge.AddMethodCall<int, ItemKey, ItemKey, ItemKey[], BuildingBlockKey, bool>(listenerId, 9, 46, charId, toolKey, targetKey, medicineKeys, buildingBlockKey, isExtract);
			}

			// Token: 0x060118D9 RID: 71897 RVA: 0x00682B12 File Offset: 0x00680D12
			public static void Build(int listenerId, BuildingBlockKey blockKey, short buildingTemplateId, int[] workers)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, short, int[]>(listenerId, 9, 47, blockKey, buildingTemplateId, workers);
			}

			// Token: 0x060118DA RID: 71898 RVA: 0x00682B23 File Offset: 0x00680D23
			public static void Remove(int listenerId, BuildingBlockKey blockKey, int[] workers)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, int[]>(listenerId, 9, 48, blockKey, workers);
			}

			// Token: 0x060118DB RID: 71899 RVA: 0x00682B33 File Offset: 0x00680D33
			public static void SetStopOperation(int listenerId, BuildingBlockKey blockKey, bool stop)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, bool>(listenerId, 9, 49, blockKey, stop);
			}

			// Token: 0x060118DC RID: 71900 RVA: 0x00682B43 File Offset: 0x00680D43
			public static void SetOperator(BuildingBlockKey blockKey, sbyte index, int charId)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, sbyte, int>(-1, 9, 50, blockKey, index, charId);
			}

			// Token: 0x060118DD RID: 71901 RVA: 0x00682B54 File Offset: 0x00680D54
			public static void Repair(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 51, blockKey);
			}

			// Token: 0x060118DE RID: 71902 RVA: 0x00682B63 File Offset: 0x00680D63
			public static void ConfirmPlanBuilding(List<IntPair> operateRecord, Location location, List<int> sameSet)
			{
				GameDataBridge.AddMethodCall<List<IntPair>, Location, List<int>>(-1, 9, 52, operateRecord, location, sameSet);
			}

			// Token: 0x060118DF RID: 71903 RVA: 0x00682B74 File Offset: 0x00680D74
			public static void AddToResidence(int charId, BuildingBlockKey buildingBlockKey)
			{
				GameDataBridge.AddMethodCall<int, BuildingBlockKey>(-1, 9, 53, charId, buildingBlockKey);
			}

			// Token: 0x060118E0 RID: 71904 RVA: 0x00682B84 File Offset: 0x00680D84
			public static void RemoveFromResidence(int listenerId, int charId, BuildingBlockKey buildingBlockKey)
			{
				GameDataBridge.AddMethodCall<int, BuildingBlockKey>(listenerId, 9, 54, charId, buildingBlockKey);
			}

			// Token: 0x060118E1 RID: 71905 RVA: 0x00682B94 File Offset: 0x00680D94
			public static void ReplaceCharacterInResidence(int listenerId, int charId, BuildingBlockKey buildingBlockKey, sbyte index)
			{
				GameDataBridge.AddMethodCall<int, BuildingBlockKey, sbyte>(listenerId, 9, 55, charId, buildingBlockKey, index);
			}

			// Token: 0x060118E2 RID: 71906 RVA: 0x00682BA5 File Offset: 0x00680DA5
			public static void ReplaceCharacterInComfortableHouse(int listenerId, int charIdB, BuildingBlockKey buildingBlockKey, sbyte index)
			{
				GameDataBridge.AddMethodCall<int, BuildingBlockKey, sbyte>(listenerId, 9, 56, charIdB, buildingBlockKey, index);
			}

			// Token: 0x060118E3 RID: 71907 RVA: 0x00682BB6 File Offset: 0x00680DB6
			public static void AddToComfortableHouse(int listenerId, int charId, BuildingBlockKey buildingBlockKey)
			{
				GameDataBridge.AddMethodCall<int, BuildingBlockKey>(listenerId, 9, 57, charId, buildingBlockKey);
			}

			// Token: 0x060118E4 RID: 71908 RVA: 0x00682BC6 File Offset: 0x00680DC6
			public static void RemoveFromComfortableHouse(int listenerId, int charId, BuildingBlockKey buildingBlockKey)
			{
				GameDataBridge.AddMethodCall<int, BuildingBlockKey>(listenerId, 9, 58, charId, buildingBlockKey);
			}

			// Token: 0x060118E5 RID: 71909 RVA: 0x00682BD6 File Offset: 0x00680DD6
			public static void QuickFillResidence(int listenerId, BuildingBlockKey buildingBlockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 59, buildingBlockKey);
			}

			// Token: 0x060118E6 RID: 71910 RVA: 0x00682BE5 File Offset: 0x00680DE5
			public static void GetCharsInResidence(int listenerId, BuildingBlockKey key)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 60, key);
			}

			// Token: 0x060118E7 RID: 71911 RVA: 0x00682BF4 File Offset: 0x00680DF4
			public static void GetAllResidents(int listenerId, BuildingBlockKey blockKey, bool homelessFirst)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, bool>(listenerId, 9, 61, blockKey, homelessFirst);
			}

			// Token: 0x060118E8 RID: 71912 RVA: 0x00682C04 File Offset: 0x00680E04
			public static void GetCharsInComfortableHouse(int listenerId, BuildingBlockKey key)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 62, key);
			}

			// Token: 0x060118E9 RID: 71913 RVA: 0x00682C13 File Offset: 0x00680E13
			public static void GetHomeless(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 63);
			}

			// Token: 0x060118EA RID: 71914 RVA: 0x00682C21 File Offset: 0x00680E21
			public static void GetSamsaraPlatformCharList(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 64);
			}

			// Token: 0x060118EB RID: 71915 RVA: 0x00682C2F File Offset: 0x00680E2F
			public static void SetSamsaraPlatformChar(sbyte destinyType, int charId)
			{
				GameDataBridge.AddMethodCall<sbyte, int>(-1, 9, 65, destinyType, charId);
			}

			// Token: 0x060118EC RID: 71916 RVA: 0x00682C3F File Offset: 0x00680E3F
			public static void SamsaraPlatformReborn(int listenerId, sbyte destinyType)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 9, 66, destinyType);
			}

			// Token: 0x060118ED RID: 71917 RVA: 0x00682C4E File Offset: 0x00680E4E
			public static void GetBuildingAreaData(int listenerId, Location location)
			{
				GameDataBridge.AddMethodCall<Location>(listenerId, 9, 67, location);
			}

			// Token: 0x060118EE RID: 71918 RVA: 0x00682C5D File Offset: 0x00680E5D
			public static void GetBuildingBlockList(int listenerId, Location location)
			{
				GameDataBridge.AddMethodCall<Location>(listenerId, 9, 68, location);
			}

			// Token: 0x060118EF RID: 71919 RVA: 0x00682C6C File Offset: 0x00680E6C
			public static void GetBuildingBlockData(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 69, blockKey);
			}

			// Token: 0x060118F0 RID: 71920 RVA: 0x00682C7B File Offset: 0x00680E7B
			public static void SetBuildingCustomName(BuildingBlockKey blockKey, string name)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, string>(-1, 9, 70, blockKey, name);
			}

			// Token: 0x060118F1 RID: 71921 RVA: 0x00682C8B File Offset: 0x00680E8B
			public static void GetEmptyBlockCount(int listenerId, short areaId, short blockId)
			{
				GameDataBridge.AddMethodCall<short, short>(listenerId, 9, 71, areaId, blockId);
			}

			// Token: 0x060118F2 RID: 71922 RVA: 0x00682C9B File Offset: 0x00680E9B
			public static void AddChicken(int listenerId, int settlementId, short templateId)
			{
				GameDataBridge.AddMethodCall<int, short>(listenerId, 9, 72, settlementId, templateId);
			}

			// Token: 0x060118F3 RID: 71923 RVA: 0x00682CAB File Offset: 0x00680EAB
			public static void RemoveChicken(int id)
			{
				GameDataBridge.AddMethodCall<int>(-1, 9, 73, id);
			}

			// Token: 0x060118F4 RID: 71924 RVA: 0x00682CBA File Offset: 0x00680EBA
			public static void RemoveAllChicken()
			{
				GameDataBridge.AddMethodCall(-1, 9, 74);
			}

			// Token: 0x060118F5 RID: 71925 RVA: 0x00682CC8 File Offset: 0x00680EC8
			public static void MoveChicken(int id, int targetSettlementId)
			{
				GameDataBridge.AddMethodCall<int, int>(-1, 9, 75, id, targetSettlementId);
			}

			// Token: 0x060118F6 RID: 71926 RVA: 0x00682CD8 File Offset: 0x00680ED8
			public static void TransferChicken(int id, int targetSettlementId)
			{
				GameDataBridge.AddMethodCall<int, int>(-1, 9, 76, id, targetSettlementId);
			}

			// Token: 0x060118F7 RID: 71927 RVA: 0x00682CE8 File Offset: 0x00680EE8
			public static void GetSettlementChickenList(int listenerId, int sourceSettlementId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 9, 77, sourceSettlementId);
			}

			// Token: 0x060118F8 RID: 71928 RVA: 0x00682CF7 File Offset: 0x00680EF7
			public static void GetSettlementChickenList(int listenerId, int sourceSettlementId, bool ignoreFulong)
			{
				GameDataBridge.AddMethodCall<int, bool>(listenerId, 9, 77, sourceSettlementId, ignoreFulong);
			}

			// Token: 0x060118F9 RID: 71929 RVA: 0x00682D07 File Offset: 0x00680F07
			public static void GetChickenData(int listenerId, int id)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 9, 78, id);
			}

			// Token: 0x060118FA RID: 71930 RVA: 0x00682D16 File Offset: 0x00680F16
			public static void InitMapBlockChicken()
			{
				GameDataBridge.AddMethodCall(-1, 9, 79);
			}

			// Token: 0x060118FB RID: 71931 RVA: 0x00682D24 File Offset: 0x00680F24
			public static void IsHaveChickenKing(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 80);
			}

			// Token: 0x060118FC RID: 71932 RVA: 0x00682D32 File Offset: 0x00680F32
			public static void RemoveAllFormResidence(BuildingBlockKey buildingBlockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(-1, 9, 81, buildingBlockKey);
			}

			// Token: 0x060118FD RID: 71933 RVA: 0x00682D41 File Offset: 0x00680F41
			public static void GetBuildingAttainment(int listenerId, BuildingBlockData blockData, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockData, BuildingBlockKey>(listenerId, 9, 82, blockData, blockKey);
			}

			// Token: 0x060118FE RID: 71934 RVA: 0x00682D51 File Offset: 0x00680F51
			public static void GetBuildingAttainment(int listenerId, BuildingBlockData blockData, BuildingBlockKey blockKey, bool isAverage)
			{
				GameDataBridge.AddMethodCall<BuildingBlockData, BuildingBlockKey, bool>(listenerId, 9, 82, blockData, blockKey, isAverage);
			}

			// Token: 0x060118FF RID: 71935 RVA: 0x00682D62 File Offset: 0x00680F62
			public static void CalcResourceOutputCount(int listenerId, BuildingBlockKey key, sbyte resourceType)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, sbyte>(listenerId, 9, 83, key, resourceType);
			}

			// Token: 0x06011900 RID: 71936 RVA: 0x00682D72 File Offset: 0x00680F72
			public static void DealInfectedPeople(List<int> charList, byte dealType)
			{
				GameDataBridge.AddMethodCall<List<int>, byte>(-1, 9, 84, charList, dealType);
			}

			// Token: 0x06011901 RID: 71937 RVA: 0x00682D82 File Offset: 0x00680F82
			public static void QuickCollectSingleShopItem(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 85, blockKey);
			}

			// Token: 0x06011902 RID: 71938 RVA: 0x00682D91 File Offset: 0x00680F91
			public static void QuickCollectSingleShopSoldItem(BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(-1, 9, 86, blockKey);
			}

			// Token: 0x06011903 RID: 71939 RVA: 0x00682DA0 File Offset: 0x00680FA0
			public static void QuickRecruitSingleBuildingPeople(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 87, blockKey);
			}

			// Token: 0x06011904 RID: 71940 RVA: 0x00682DAF File Offset: 0x00680FAF
			public static void QuickFillComfortableHouse(int listenerId, BuildingBlockKey buildingBlockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 88, buildingBlockKey);
			}

			// Token: 0x06011905 RID: 71941 RVA: 0x00682DBE File Offset: 0x00680FBE
			public static void RemoveAllFromComfortableHouse(BuildingBlockKey buildingBlockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(-1, 9, 89, buildingBlockKey);
			}

			// Token: 0x06011906 RID: 71942 RVA: 0x00682DCD File Offset: 0x00680FCD
			public static void SortedComfortableHousePeople(int listenerId, List<int> charIdList)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 9, 90, charIdList);
			}

			// Token: 0x06011907 RID: 71943 RVA: 0x00682DDC File Offset: 0x00680FDC
			public static void GetMakeResult(int listenerId, short materialTemplateId, ItemKey toolKey, BuildingBlockKey buildingBlockKey, sbyte lifeSkillType, List<short> makeItemSubtypeIdList, short makeItemSubTypeId, bool isPerfect, bool isManual)
			{
				GameDataBridge.AddMethodCall<short, ItemKey, BuildingBlockKey, sbyte, List<short>, short, bool, bool>(listenerId, 9, 91, materialTemplateId, toolKey, buildingBlockKey, lifeSkillType, makeItemSubtypeIdList, makeItemSubTypeId, isPerfect, isManual);
			}

			// Token: 0x06011908 RID: 71944 RVA: 0x00682E02 File Offset: 0x00681002
			public static void GetSutraReadingRoomBuffValue(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 92);
			}

			// Token: 0x06011909 RID: 71945 RVA: 0x00682E10 File Offset: 0x00681010
			public static void SetBuildingAutoWork(short blockIndex, bool isAutoWork)
			{
				GameDataBridge.AddMethodCall<short, bool>(-1, 9, 93, blockIndex, isAutoWork);
			}

			// Token: 0x0601190A RID: 71946 RVA: 0x00682E20 File Offset: 0x00681020
			public static void GetBuildingIsAutoWork(int listenerId, short blockIndex)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 9, 94, blockIndex);
			}

			// Token: 0x0601190B RID: 71947 RVA: 0x00682E2F File Offset: 0x0068102F
			public static void ShopBuildingMultiChangeSoldItem(BuildingBlockKey key, List<ItemKey> itemList, List<int> operateTypeList)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, List<ItemKey>, List<int>>(-1, 9, 95, key, itemList, operateTypeList);
			}

			// Token: 0x0601190C RID: 71948 RVA: 0x00682E40 File Offset: 0x00681040
			public static void RepairItemList(int listenerId, int charId, List<MultiplyOperation> operationList)
			{
				GameDataBridge.AddMethodCall<int, List<MultiplyOperation>>(listenerId, 9, 96, charId, operationList);
			}

			// Token: 0x0601190D RID: 71949 RVA: 0x00682E50 File Offset: 0x00681050
			public static void SetBuildingAutoSold(short blockIndex, bool isAutoSold)
			{
				GameDataBridge.AddMethodCall<short, bool>(-1, 9, 97, blockIndex, isAutoSold);
			}

			// Token: 0x0601190E RID: 71950 RVA: 0x00682E60 File Offset: 0x00681060
			public static void GetBuildingIsAutoSold(int listenerId, short blockIndex)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 9, 98, blockIndex);
			}

			// Token: 0x0601190F RID: 71951 RVA: 0x00682E6F File Offset: 0x0068106F
			public static void GetXiangshuIdInKungfuRoom(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 99);
			}

			// Token: 0x06011910 RID: 71952 RVA: 0x00682E7D File Offset: 0x0068107D
			public static void RepairItemOptional(int listenerId, int charId, ItemKey toolKey, ItemKey itemKey, sbyte toolSourceType)
			{
				GameDataBridge.AddMethodCall<int, ItemKey, ItemKey, sbyte>(listenerId, 9, 100, charId, toolKey, itemKey, toolSourceType);
			}

			// Token: 0x06011911 RID: 71953 RVA: 0x00682E90 File Offset: 0x00681090
			public static void SetNickNameByChickenId(int id, string nickname)
			{
				GameDataBridge.AddMethodCall<int, string>(-1, 9, 101, id, nickname);
			}

			// Token: 0x06011912 RID: 71954 RVA: 0x00682EA0 File Offset: 0x006810A0
			public static void GetSettlementChickenDataList(int listenerId, Location location)
			{
				GameDataBridge.AddMethodCall<Location>(listenerId, 9, 102, location);
			}

			// Token: 0x06011913 RID: 71955 RVA: 0x00682EAF File Offset: 0x006810AF
			public static void GetSettlementChickenDataList(int listenerId, Location location, bool ignoreFulong)
			{
				GameDataBridge.AddMethodCall<Location, bool>(listenerId, 9, 102, location, ignoreFulong);
			}

			// Token: 0x06011914 RID: 71956 RVA: 0x00682EBF File Offset: 0x006810BF
			public static void SetTeaHorseCaravanWeather(short weatherId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 9, 103, weatherId);
			}

			// Token: 0x06011915 RID: 71957 RVA: 0x00682ECE File Offset: 0x006810CE
			public static void GetComfortableIsAutoCheckIn(int listenerId, short blockIndex)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 9, 104, blockIndex);
			}

			// Token: 0x06011916 RID: 71958 RVA: 0x00682EDD File Offset: 0x006810DD
			public static void GetResidenceIsAutoCheckIn(int listenerId, short blockIndex)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 9, 105, blockIndex);
			}

			// Token: 0x06011917 RID: 71959 RVA: 0x00682EEC File Offset: 0x006810EC
			public static void SetComfortableAutoCheckIn(short blockIndex, bool isAutoCheckIn)
			{
				GameDataBridge.AddMethodCall<short, bool>(-1, 9, 106, blockIndex, isAutoCheckIn);
			}

			// Token: 0x06011918 RID: 71960 RVA: 0x00682EFC File Offset: 0x006810FC
			public static void SetResidenceAutoCheckIn(short blockIndex, bool isAutoCheckIn)
			{
				GameDataBridge.AddMethodCall<short, bool>(-1, 9, 107, blockIndex, isAutoCheckIn);
			}

			// Token: 0x06011919 RID: 71961 RVA: 0x00682F0C File Offset: 0x0068110C
			public static void GmCmd_AddLegacyBuilding(short buildingTemplateId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 9, 108, buildingTemplateId);
			}

			// Token: 0x0601191A RID: 71962 RVA: 0x00682F1B File Offset: 0x0068111B
			public static void SetUnlockedWorkingVillagers(int charId, bool add)
			{
				GameDataBridge.AddMethodCall<int, bool>(-1, 9, 109, charId, add);
			}

			// Token: 0x0601191B RID: 71963 RVA: 0x00682F2B File Offset: 0x0068112B
			public static void WeaveClothingItem(int listenerId, ItemDisplayData tool, ItemDisplayData target, short weaveClothingTemplateId)
			{
				GameDataBridge.AddMethodCall<ItemDisplayData, ItemDisplayData, short>(listenerId, 9, 110, tool, target, weaveClothingTemplateId);
			}

			// Token: 0x0601191C RID: 71964 RVA: 0x00682F3C File Offset: 0x0068113C
			public static void GmCmd_GetChickenData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 111);
			}

			// Token: 0x0601191D RID: 71965 RVA: 0x00682F4A File Offset: 0x0068114A
			public static void GetPossessionPreview(int listenerId, List<int> soulCharIds, int bodyCharId, List<short> featureIds, int previewId)
			{
				GameDataBridge.AddMethodCall<List<int>, int, List<short>, int>(listenerId, 9, 112, soulCharIds, bodyCharId, featureIds, previewId);
			}

			// Token: 0x0601191E RID: 71966 RVA: 0x00682F5D File Offset: 0x0068115D
			public static void TrySwapSoulCeremony(int listenerId, List<int> soulCharIds, int bodyCharId, List<short> featureIds)
			{
				GameDataBridge.AddMethodCall<List<int>, int, List<short>>(listenerId, 9, 113, soulCharIds, bodyCharId, featureIds);
			}

			// Token: 0x0601191F RID: 71967 RVA: 0x00682F6E File Offset: 0x0068116E
			public static void GetBackTeaHorseCarryItem(ItemKey itemKey, sbyte itemSource)
			{
				GameDataBridge.AddMethodCall<ItemKey, sbyte>(-1, 9, 114, itemKey, itemSource);
			}

			// Token: 0x06011920 RID: 71968 RVA: 0x00682F7E File Offset: 0x0068117E
			public static void AddItemToTeaHorseCarryItem(ItemKey itemKey, sbyte itemSource)
			{
				GameDataBridge.AddMethodCall<ItemKey, sbyte>(-1, 9, 115, itemKey, itemSource);
			}

			// Token: 0x06011921 RID: 71969 RVA: 0x00682F8E File Offset: 0x0068118E
			public static void SetTemporaryPossessionCharacterAvatar(AvatarData avatar)
			{
				GameDataBridge.AddMethodCall<AvatarData>(-1, 9, 116, avatar);
			}

			// Token: 0x06011922 RID: 71970 RVA: 0x00682F9D File Offset: 0x0068119D
			public static void GetSwapSoulCeremonyBodyCharIdList(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 117);
			}

			// Token: 0x06011923 RID: 71971 RVA: 0x00682FAB File Offset: 0x006811AB
			public static void GetBuildingShopManagerAutoArrangeSorted(int listenerId, BuildingBlockKey blockKey, int[] managerCharacterIds)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, int[]>(listenerId, 9, 118, blockKey, managerCharacterIds);
			}

			// Token: 0x06011924 RID: 71972 RVA: 0x00682FBB File Offset: 0x006811BB
			public static void SectMainStoryJingangClickMonkSoulBtn()
			{
				GameDataBridge.AddMethodCall(-1, 9, 119);
			}

			// Token: 0x06011925 RID: 71973 RVA: 0x00682FC9 File Offset: 0x006811C9
			public static void RejectBuildingBlockRecruitPeople(BuildingBlockKey key, int earningDataIndex)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, int>(-1, 9, 120, key, earningDataIndex);
			}

			// Token: 0x06011926 RID: 71974 RVA: 0x00682FD9 File Offset: 0x006811D9
			public static void RejectBuildingBlockRecruitPeople(BuildingBlockKey key, int earningDataIndex, bool isSetData)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, int, bool>(-1, 9, 120, key, earningDataIndex, isSetData);
			}

			// Token: 0x06011927 RID: 71975 RVA: 0x00682FEA File Offset: 0x006811EA
			public static void RejectBuildingBlockRecruitPeopleQuick(BuildingBlockKey key)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(-1, 9, 121, key);
			}

			// Token: 0x06011928 RID: 71976 RVA: 0x00682FF9 File Offset: 0x006811F9
			public static void GetShopManagementYieldTipsData(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 122, blockKey);
			}

			// Token: 0x06011929 RID: 71977 RVA: 0x00683008 File Offset: 0x00681208
			public static void CalculateBuildingManageHarvestSuccessRate(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 123, blockKey);
			}

			// Token: 0x0601192A RID: 71978 RVA: 0x00683017 File Offset: 0x00681217
			public static void GetOrCreateShopEventCollection(int listenerId, BuildingBlockKey buildingBlockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 124, buildingBlockKey);
			}

			// Token: 0x0601192B RID: 71979 RVA: 0x00683026 File Offset: 0x00681226
			public static void GetSamsaraPlatformRecord(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 125);
			}

			// Token: 0x0601192C RID: 71980 RVA: 0x00683034 File Offset: 0x00681234
			public static void GetSwapSoulCeremonySoulCharIdList(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 126);
			}

			// Token: 0x0601192D RID: 71981 RVA: 0x00683042 File Offset: 0x00681242
			public static void CricketCollectionBatchAddCricketJar()
			{
				GameDataBridge.AddMethodCall(-1, 9, 127);
			}

			// Token: 0x0601192E RID: 71982 RVA: 0x00683050 File Offset: 0x00681250
			public static void CricketCollectionBatchAddCricket()
			{
				GameDataBridge.AddMethodCall(-1, 9, 128);
			}

			// Token: 0x0601192F RID: 71983 RVA: 0x00683061 File Offset: 0x00681261
			public static void CricketCollectionBatchRemoveJar(ItemSourceType sourceType)
			{
				GameDataBridge.AddMethodCall<ItemSourceType>(-1, 9, 129, sourceType);
			}

			// Token: 0x06011930 RID: 71984 RVA: 0x00683073 File Offset: 0x00681273
			public static void CricketCollectionBatchRemoveCricket(ItemSourceType sourceType)
			{
				GameDataBridge.AddMethodCall<ItemSourceType>(-1, 9, 130, sourceType);
			}

			// Token: 0x06011931 RID: 71985 RVA: 0x00683085 File Offset: 0x00681285
			public static void GetCricketOrJarFromSourceStorage(int listenerId, short itemSubType, ItemSourceType sourceType)
			{
				GameDataBridge.AddMethodCall<short, ItemSourceType>(listenerId, 9, 131, itemSubType, sourceType);
			}

			// Token: 0x06011932 RID: 71986 RVA: 0x00683098 File Offset: 0x00681298
			public static void SmartOperateCricketOrJarCollection(int collectionIndex, short itemSubType, ItemSourceType sourceType, ItemKey itemKey)
			{
				GameDataBridge.AddMethodCall<int, short, ItemSourceType, ItemKey>(-1, 9, 132, collectionIndex, itemSubType, sourceType, itemKey);
			}

			// Token: 0x06011933 RID: 71987 RVA: 0x006830AD File Offset: 0x006812AD
			public static void GetBatchButtonEnableState(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 133);
			}

			// Token: 0x06011934 RID: 71988 RVA: 0x006830BE File Offset: 0x006812BE
			public static void CalculateBuildingManageHarvestSuccessRates(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 134, blockKey);
			}

			// Token: 0x06011935 RID: 71989 RVA: 0x006830D0 File Offset: 0x006812D0
			public static void UnsetFulongChicken(int listenerId, short orgMemberTemplateId, int chickenId)
			{
				GameDataBridge.AddMethodCall<short, int>(listenerId, 9, 135, orgMemberTemplateId, chickenId);
			}

			// Token: 0x06011936 RID: 71990 RVA: 0x006830E3 File Offset: 0x006812E3
			public static void SetFulongChicken(int listenerId, short orgMemberTemplateId, int chickenId)
			{
				GameDataBridge.AddMethodCall<short, int>(listenerId, 9, 136, orgMemberTemplateId, chickenId);
			}

			// Token: 0x06011937 RID: 71991 RVA: 0x006830F6 File Offset: 0x006812F6
			public static void GetChickenDataList(int listenerId, List<int> idList)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 9, 137, idList);
			}

			// Token: 0x06011938 RID: 71992 RVA: 0x00683108 File Offset: 0x00681308
			public static void GetChickenNicknameList(int listenerId, List<int> chickenIdList)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 9, 138, chickenIdList);
			}

			// Token: 0x06011939 RID: 71993 RVA: 0x0068311A File Offset: 0x0068131A
			public static void GetSettlementChickenIdList(int listenerId, Location location)
			{
				GameDataBridge.AddMethodCall<Location>(listenerId, 9, 139, location);
			}

			// Token: 0x0601193A RID: 71994 RVA: 0x0068312C File Offset: 0x0068132C
			public static void GetChickensNicknameByLocation(int listenerId, Location location)
			{
				GameDataBridge.AddMethodCall<Location>(listenerId, 9, 140, location);
			}

			// Token: 0x0601193B RID: 71995 RVA: 0x0068313E File Offset: 0x0068133E
			public static void AllChickenInTaiwuVillage(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 141);
			}

			// Token: 0x0601193C RID: 71996 RVA: 0x0068314F File Offset: 0x0068134F
			public static void GetVillagerRoleExtraEffectUnlockState(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 142);
			}

			// Token: 0x0601193D RID: 71997 RVA: 0x00683160 File Offset: 0x00681360
			public static void ClickChickenMap(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 143);
			}

			// Token: 0x0601193E RID: 71998 RVA: 0x00683171 File Offset: 0x00681371
			public static void ClickChickenMap(int listenerId, bool ignoreTask)
			{
				GameDataBridge.AddMethodCall<bool>(listenerId, 9, 143, ignoreTask);
			}

			// Token: 0x0601193F RID: 71999 RVA: 0x00683183 File Offset: 0x00681383
			public static void ClickChickenSign(int chickenId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 9, 144, chickenId);
			}

			// Token: 0x06011940 RID: 72000 RVA: 0x00683195 File Offset: 0x00681395
			public static void SetBuildingResourceOutputSetting(int blockIndex, BuildingResourceOutputSetting setting)
			{
				GameDataBridge.AddMethodCall<int, BuildingResourceOutputSetting>(-1, 9, 145, blockIndex, setting);
			}

			// Token: 0x06011941 RID: 72001 RVA: 0x006831A8 File Offset: 0x006813A8
			public static void GetBuildingResourceOutputSetting(int listenerId, int blockIndex)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 9, 146, blockIndex);
			}

			// Token: 0x06011942 RID: 72002 RVA: 0x006831BA File Offset: 0x006813BA
			public static void GetBuildingExceptionData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 147);
			}

			// Token: 0x06011943 RID: 72003 RVA: 0x006831CB File Offset: 0x006813CB
			public static void AllDependBuildingAvailable(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 148, blockKey);
			}

			// Token: 0x06011944 RID: 72004 RVA: 0x006831DD File Offset: 0x006813DD
			public static void PracticingCombatSkillInPracticeRoom(int listenerId, BuildingBlockKey blockKey, short skillTemplateId, int count, int cost)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, short, int, int>(listenerId, 9, 149, blockKey, skillTemplateId, count, cost);
			}

			// Token: 0x06011945 RID: 72005 RVA: 0x006831F3 File Offset: 0x006813F3
			public static void HasShopManagerLeader(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 150, blockKey);
			}

			// Token: 0x06011946 RID: 72006 RVA: 0x00683205 File Offset: 0x00681405
			public static void QuickArrangeShopManager(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 151, blockKey);
			}

			// Token: 0x06011947 RID: 72007 RVA: 0x00683217 File Offset: 0x00681417
			public static void QuickArrangeShopManager(int listenerId, BuildingBlockKey blockKey, bool onlyCheck)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, bool>(listenerId, 9, 151, blockKey, onlyCheck);
			}

			// Token: 0x06011948 RID: 72008 RVA: 0x0068322A File Offset: 0x0068142A
			public static void QuickArrangeBuildOperator(int listenerId, short buildingTemplateId, BuildingBlockKey blockKey, sbyte operationType)
			{
				GameDataBridge.AddMethodCall<short, BuildingBlockKey, sbyte>(listenerId, 9, 152, buildingTemplateId, blockKey, operationType);
			}

			// Token: 0x06011949 RID: 72009 RVA: 0x0068323E File Offset: 0x0068143E
			public static void QuickArrangeBuildOperator(int listenerId, short buildingTemplateId, BuildingBlockKey blockKey, sbyte operationType, List<int> exceptCharList)
			{
				GameDataBridge.AddMethodCall<short, BuildingBlockKey, sbyte, List<int>>(listenerId, 9, 152, buildingTemplateId, blockKey, operationType, exceptCharList);
			}

			// Token: 0x0601194A RID: 72010 RVA: 0x00683254 File Offset: 0x00681454
			public static void ShopBuildingCanTeach(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 153, blockKey);
			}

			// Token: 0x0601194B RID: 72011 RVA: 0x00683266 File Offset: 0x00681466
			public static void GetOperationLeftTime(int listenerId, short buildingTemplateId, BuildingBlockKey blockKey, sbyte operationType, List<int> operatorList)
			{
				GameDataBridge.AddMethodCall<short, BuildingBlockKey, sbyte, List<int>>(listenerId, 9, 154, buildingTemplateId, blockKey, operationType, operatorList);
			}

			// Token: 0x0601194C RID: 72012 RVA: 0x0068327C File Offset: 0x0068147C
			public static void GetBuildingOperationLeftTime(int listenerId, BuildingBlockKey blockKey, sbyte operationType)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, sbyte>(listenerId, 9, 155, blockKey, operationType);
			}

			// Token: 0x0601194D RID: 72013 RVA: 0x0068328F File Offset: 0x0068148F
			public static void GetShopBuildingTeachBookData(int listenerId, BuildingBlockKey blockKey, int memberId)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, int>(listenerId, 9, 156, blockKey, memberId);
			}

			// Token: 0x0601194E RID: 72014 RVA: 0x006832A2 File Offset: 0x006814A2
			public static void CalcExtraTaiwuGroupMaxCountByStrategyRoom(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 157);
			}

			// Token: 0x0601194F RID: 72015 RVA: 0x006832B3 File Offset: 0x006814B3
			public static void GetTaiwuCanFixBookItemDataList(int listenerId, ItemSourceType itemSourceType)
			{
				GameDataBridge.AddMethodCall<ItemSourceType>(listenerId, 9, 158, itemSourceType);
			}

			// Token: 0x06011950 RID: 72016 RVA: 0x006832C5 File Offset: 0x006814C5
			public static void GetResidenceInfo(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 159);
			}

			// Token: 0x06011951 RID: 72017 RVA: 0x006832D6 File Offset: 0x006814D6
			public static void GetTaiwuVillageResourceBlockEffect(int listenerId, EBuildingScaleEffect effectType)
			{
				GameDataBridge.AddMethodCall<EBuildingScaleEffect>(listenerId, 9, 160, effectType);
			}

			// Token: 0x06011952 RID: 72018 RVA: 0x006832E8 File Offset: 0x006814E8
			public static void GetTaiwuLocationResourceBlockEffect(int listenerId, EBuildingScaleEffect effectType)
			{
				GameDataBridge.AddMethodCall<EBuildingScaleEffect>(listenerId, 9, 161, effectType);
			}

			// Token: 0x06011953 RID: 72019 RVA: 0x006832FA File Offset: 0x006814FA
			public static void GetTaiwuVillageResourceBlockEffectInfo(int listenerId, short templateId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 9, 162, templateId);
			}

			// Token: 0x06011954 RID: 72020 RVA: 0x0068330C File Offset: 0x0068150C
			public static void CanQuickArrangeShopManager(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 163, blockKey);
			}

			// Token: 0x06011955 RID: 72021 RVA: 0x0068331E File Offset: 0x0068151E
			public static void GetBuildingFormulaContextBridge(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 164, blockKey);
			}

			// Token: 0x06011956 RID: 72022 RVA: 0x00683330 File Offset: 0x00681530
			public static void GetBuildingEffectForMake(int listenerId, BuildingBlockKey buildingBlockKey, sbyte skillType)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, sbyte>(listenerId, 9, 165, buildingBlockKey, skillType);
			}

			// Token: 0x06011957 RID: 72023 RVA: 0x00683343 File Offset: 0x00681543
			public static void GmCmd_BuildingCollectPerform(int listenerId, int totalAttainment, short buildingTemplateId, int repeat)
			{
				GameDataBridge.AddMethodCall<int, short, int>(listenerId, 9, 166, totalAttainment, buildingTemplateId, repeat);
			}

			// Token: 0x06011958 RID: 72024 RVA: 0x00683357 File Offset: 0x00681557
			public static void GmCmd_BeatMinionPerform(int listenerId, sbyte grade, int repeat)
			{
				GameDataBridge.AddMethodCall<sbyte, int>(listenerId, 9, 167, grade, repeat);
			}

			// Token: 0x06011959 RID: 72025 RVA: 0x0068336A File Offset: 0x0068156A
			public static void GetStoreLocation(int listenerId, int type)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 9, 168, type);
			}

			// Token: 0x0601195A RID: 72026 RVA: 0x0068337C File Offset: 0x0068157C
			public static void SetStoreLocation(int type, int value)
			{
				GameDataBridge.AddMethodCall<int, int>(-1, 9, 169, type, value);
			}

			// Token: 0x0601195B RID: 72027 RVA: 0x0068338F File Offset: 0x0068158F
			public static void GetFeastTargetCharList(int listenerId, BuildingBlockKey buildingBlockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 170, buildingBlockKey);
			}

			// Token: 0x0601195C RID: 72028 RVA: 0x006833A1 File Offset: 0x006815A1
			public static void TryShowNotifications()
			{
				GameDataBridge.AddMethodCall(-1, 9, 171);
			}

			// Token: 0x0601195D RID: 72029 RVA: 0x006833B2 File Offset: 0x006815B2
			public static void QuickRemoveShopSoldItem(BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(-1, 9, 172, blockKey);
			}

			// Token: 0x0601195E RID: 72030 RVA: 0x006833C4 File Offset: 0x006815C4
			public static void QuickAddShopSoldItem(BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(-1, 9, 173, blockKey);
			}

			// Token: 0x0601195F RID: 72031 RVA: 0x006833D6 File Offset: 0x006815D6
			public static void CalcTaiwuVillagerInfoDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 174);
			}

			// Token: 0x06011960 RID: 72032 RVA: 0x006833E7 File Offset: 0x006815E7
			public static void CalcTaiwuVillagerEfficiencyInBuilding(int listenerId, BuildingBlockKey blockKey, int charId)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, int>(listenerId, 9, 175, blockKey, charId);
			}

			// Token: 0x06011961 RID: 72033 RVA: 0x006833FA File Offset: 0x006815FA
			public static void QuickGetSpecificExchangeItem(ItemKey key)
			{
				GameDataBridge.AddMethodCall<ItemKey>(-1, 9, 176, key);
			}

			// Token: 0x06011962 RID: 72034 RVA: 0x0068340C File Offset: 0x0068160C
			public static void QuickGetSpecificExchangeItem(ItemKey key, ItemSourceType source)
			{
				GameDataBridge.AddMethodCall<ItemKey, ItemSourceType>(-1, 9, 176, key, source);
			}

			// Token: 0x06011963 RID: 72035 RVA: 0x0068341F File Offset: 0x0068161F
			public static void AddLocationMark(int listenerId, Location location)
			{
				GameDataBridge.AddMethodCall<Location>(listenerId, 9, 177, location);
			}

			// Token: 0x06011964 RID: 72036 RVA: 0x00683431 File Offset: 0x00681631
			public static void RemoveLocationMark(int listenerId, Location location)
			{
				GameDataBridge.AddMethodCall<Location>(listenerId, 9, 178, location);
			}

			// Token: 0x06011965 RID: 72037 RVA: 0x00683443 File Offset: 0x00681643
			public static void RequestUnlockedWorkingVillagers(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 179);
			}

			// Token: 0x06011966 RID: 72038 RVA: 0x00683454 File Offset: 0x00681654
			public static void SetBuildingArrangementSetting(BuildingBlockKey key, BuildingOptionAutoGiveMemberPreset setting)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, BuildingOptionAutoGiveMemberPreset>(-1, 9, 180, key, setting);
			}

			// Token: 0x06011967 RID: 72039 RVA: 0x00683467 File Offset: 0x00681667
			public static void UpgradeResourceBuilding(int listenerId, BuildingBlockKey key, int level)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, int>(listenerId, 9, 181, key, level);
			}

			// Token: 0x06011968 RID: 72040 RVA: 0x0068347A File Offset: 0x0068167A
			public static void UpgradeSlotBuilding(int listenerId, BuildingBlockKey key, int levelSlotIndex)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, int>(listenerId, 9, 182, key, levelSlotIndex);
			}

			// Token: 0x06011969 RID: 72041 RVA: 0x0068348D File Offset: 0x0068168D
			public static void UnlockBuildingLevelSlot(int listenerId, BuildingBlockKey key, int index)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, int>(listenerId, 9, 183, key, index);
			}

			// Token: 0x0601196A RID: 72042 RVA: 0x006834A0 File Offset: 0x006816A0
			public static void SetBuildingSoldItemSetting(BuildingBlockKey key, BuildingOptionAutoAddSoldItemPreset setting)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, BuildingOptionAutoAddSoldItemPreset>(-1, 9, 184, key, setting);
			}

			// Token: 0x0601196B RID: 72043 RVA: 0x006834B3 File Offset: 0x006816B3
			public static void GetPuppetPageDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 185);
			}

			// Token: 0x0601196C RID: 72044 RVA: 0x006834C4 File Offset: 0x006816C4
			public static void QuickRepairAllBuilding()
			{
				GameDataBridge.AddMethodCall(-1, 9, 186);
			}

			// Token: 0x0601196D RID: 72045 RVA: 0x006834D5 File Offset: 0x006816D5
			public static void CalcQuickRepairAllBuildingCostMoney(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 187);
			}

			// Token: 0x0601196E RID: 72046 RVA: 0x006834E6 File Offset: 0x006816E6
			public static void GetBuildingFunctionData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 188);
			}

			// Token: 0x0601196F RID: 72047 RVA: 0x006834F7 File Offset: 0x006816F7
			public static void GetTaiwuVillageBuildingAreaData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 189);
			}

			// Token: 0x06011970 RID: 72048 RVA: 0x00683508 File Offset: 0x00681708
			public static void GetAllPawnShopItem(int listenerId, bool getItem)
			{
				GameDataBridge.AddMethodCall<bool>(listenerId, 9, 190, getItem);
			}

			// Token: 0x06011971 RID: 72049 RVA: 0x0068351A File Offset: 0x0068171A
			public static void GetTaiwuVillageBlockEffectInfo(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 191, blockKey);
			}

			// Token: 0x06011972 RID: 72050 RVA: 0x0068352C File Offset: 0x0068172C
			public static void GetTaiwuVillageShopData(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 192, blockKey);
			}

			// Token: 0x06011973 RID: 72051 RVA: 0x0068353E File Offset: 0x0068173E
			public static void GetTaiwuVillageShopData(int listenerId, BuildingBlockKey blockKey, EBuildingScaleEffect effectType)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, EBuildingScaleEffect>(listenerId, 9, 192, blockKey, effectType);
			}

			// Token: 0x06011974 RID: 72052 RVA: 0x00683551 File Offset: 0x00681751
			public static void GetBuildingEarningDisplayData(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 193, blockKey);
			}

			// Token: 0x06011975 RID: 72053 RVA: 0x00683563 File Offset: 0x00681763
			public static void GetBuildingManageDisplayData(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 194, blockKey);
			}

			// Token: 0x06011976 RID: 72054 RVA: 0x00683575 File Offset: 0x00681775
			public static void GetUnlockedFeastTypeList(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 195);
			}

			// Token: 0x06011977 RID: 72055 RVA: 0x00683586 File Offset: 0x00681786
			public static void GetReversedSamsaraRecord(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 196);
			}

			// Token: 0x06011978 RID: 72056 RVA: 0x00683597 File Offset: 0x00681797
			public static void GetLockedComfortableHouseCharacters(int listenerId, BuildingBlockKey buildingBlockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 197, buildingBlockKey);
			}

			// Token: 0x06011979 RID: 72057 RVA: 0x006835A9 File Offset: 0x006817A9
			public static void GetLockedResidenceCharacters(int listenerId, BuildingBlockKey buildingBlockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 198, buildingBlockKey);
			}

			// Token: 0x0601197A RID: 72058 RVA: 0x006835BB File Offset: 0x006817BB
			public static void UnlockComfortableHouseCharacter(int listenerId, BuildingBlockKey buildingBlockKey, int charId)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, int>(listenerId, 9, 199, buildingBlockKey, charId);
			}

			// Token: 0x0601197B RID: 72059 RVA: 0x006835CE File Offset: 0x006817CE
			public static void LockComfortableHouseCharacter(int listenerId, BuildingBlockKey buildingBlockKey, int charId)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, int>(listenerId, 9, 200, buildingBlockKey, charId);
			}

			// Token: 0x0601197C RID: 72060 RVA: 0x006835E1 File Offset: 0x006817E1
			public static void UnlockResidenceCharacter(int listenerId, BuildingBlockKey buildingBlockKey, int charId)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, int>(listenerId, 9, 201, buildingBlockKey, charId);
			}

			// Token: 0x0601197D RID: 72061 RVA: 0x006835F4 File Offset: 0x006817F4
			public static void SetComfortableAutoCheckInType(BuildingBlockKey buildingBlockKey, bool checkInType)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, bool>(-1, 9, 202, buildingBlockKey, checkInType);
			}

			// Token: 0x0601197E RID: 72062 RVA: 0x00683607 File Offset: 0x00681807
			public static void LockResidenceCharacter(int listenerId, BuildingBlockKey buildingBlockKey, int charId)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, int>(listenerId, 9, 203, buildingBlockKey, charId);
			}

			// Token: 0x0601197F RID: 72063 RVA: 0x0068361A File Offset: 0x0068181A
			public static void SetNextTeaHorseCaravanEvent(short eventTemplateId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 9, 204, eventTemplateId);
			}

			// Token: 0x06011980 RID: 72064 RVA: 0x0068362C File Offset: 0x0068182C
			public static void GetLockedInComfortableHouseIds(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 205);
			}

			// Token: 0x06011981 RID: 72065 RVA: 0x0068363D File Offset: 0x0068183D
			public static void GetLockedInResidenceIds(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 206);
			}

			// Token: 0x06011982 RID: 72066 RVA: 0x0068364E File Offset: 0x0068184E
			public static void GetReversedBlockShopEvent(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 207, blockKey);
			}

			// Token: 0x06011983 RID: 72067 RVA: 0x00683660 File Offset: 0x00681860
			public static void PluckAllChickenFeathers(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 208);
			}

			// Token: 0x06011984 RID: 72068 RVA: 0x00683671 File Offset: 0x00681871
			public static void IsAllChickensCanPluck(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 209);
			}

			// Token: 0x06011985 RID: 72069 RVA: 0x00683682 File Offset: 0x00681882
			public static void GetCharacterChickenFeatures(int listenerId, int characterId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 9, 210, characterId);
			}

			// Token: 0x06011986 RID: 72070 RVA: 0x00683694 File Offset: 0x00681894
			public static void GetChickensByPersonalityType(int listenerId, sbyte personalityType)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 9, 211, personalityType);
			}

			// Token: 0x06011987 RID: 72071 RVA: 0x006836A6 File Offset: 0x006818A6
			public static void GetCurrentFeatherValue(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 212);
			}

			// Token: 0x06011988 RID: 72072 RVA: 0x006836B7 File Offset: 0x006818B7
			public static void CanCultivateFeather(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 213);
			}

			// Token: 0x06011989 RID: 72073 RVA: 0x006836C8 File Offset: 0x006818C8
			public static void GetChickenPluckFeatherDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 214);
			}

			// Token: 0x0601198A RID: 72074 RVA: 0x006836D9 File Offset: 0x006818D9
			public static void IsFeatherSystemUnlocked(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 215);
			}

			// Token: 0x0601198B RID: 72075 RVA: 0x006836EA File Offset: 0x006818EA
			public static void UnlockFeatherSystem()
			{
				GameDataBridge.AddMethodCall(-1, 9, 216);
			}

			// Token: 0x0601198C RID: 72076 RVA: 0x006836FB File Offset: 0x006818FB
			public static void PluckChickenFeather(int listenerId, int chickenId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 9, 217, chickenId);
			}

			// Token: 0x0601198D RID: 72077 RVA: 0x0068370D File Offset: 0x0068190D
			public static void CanUseChickenFeather(int listenerId, int characterId, sbyte personalityType)
			{
				GameDataBridge.AddMethodCall<int, sbyte>(listenerId, 9, 218, characterId, personalityType);
			}

			// Token: 0x0601198E RID: 72078 RVA: 0x00683720 File Offset: 0x00681920
			public static void UseChickenFeather(int listenerId, int characterId, ItemKey itemKey, sbyte personalityType)
			{
				GameDataBridge.AddMethodCall<int, ItemKey, sbyte>(listenerId, 9, 219, characterId, itemKey, personalityType);
			}

			// Token: 0x0601198F RID: 72079 RVA: 0x00683734 File Offset: 0x00681934
			public static void CanPluckFeatherInVillage(int listenerId, int chickenId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 9, 220, chickenId);
			}

			// Token: 0x06011990 RID: 72080 RVA: 0x00683746 File Offset: 0x00681946
			public static void CultivateFeather(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 221);
			}

			// Token: 0x06011991 RID: 72081 RVA: 0x00683757 File Offset: 0x00681957
			public static void GetCanPluckFeatherChickenIds(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 222);
			}

			// Token: 0x06011992 RID: 72082 RVA: 0x00683768 File Offset: 0x00681968
			public static void GetSamsaraPlatformBonusAttributes(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 223);
			}

			// Token: 0x06011993 RID: 72083 RVA: 0x00683779 File Offset: 0x00681979
			public static void GetSamsaraPlatformBonusAttributes(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 9, 223, charId);
			}

			// Token: 0x06011994 RID: 72084 RVA: 0x0068378B File Offset: 0x0068198B
			public static void GetSamsaraPlatformCharDisplayData(int listenerId, sbyte slot)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 9, 224, slot);
			}

			// Token: 0x06011995 RID: 72085 RVA: 0x0068379D File Offset: 0x0068199D
			public static void QuickAssignChicken(short orgMemberTemplateId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 9, 225, orgMemberTemplateId);
			}

			// Token: 0x06011996 RID: 72086 RVA: 0x006837AF File Offset: 0x006819AF
			public static void GetCricketCollectionDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 226);
			}

			// Token: 0x06011997 RID: 72087 RVA: 0x006837C0 File Offset: 0x006819C0
			public static void GetBuildingMakeDisplayData(int listenerId, BuildingBlockKey blockKey, sbyte lifeSkillType)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, sbyte>(listenerId, 9, 227, blockKey, lifeSkillType);
			}

			// Token: 0x06011998 RID: 72088 RVA: 0x006837D3 File Offset: 0x006819D3
			public static void CheckRefineCondition(int listenerId, int charId, ItemKey[] toolKeys, ItemKey equipItemKey, ItemDisplayData[] materialItemData, BuildingBlockKey buildingBlockKey)
			{
				GameDataBridge.AddMethodCall<int, ItemKey[], ItemKey, ItemDisplayData[], BuildingBlockKey>(listenerId, 9, 228, charId, toolKeys, equipItemKey, materialItemData, buildingBlockKey);
			}

			// Token: 0x06011999 RID: 72089 RVA: 0x006837EB File Offset: 0x006819EB
			public static void RefineItem(int listenerId, int charId, ItemDisplayData[] tools, ItemDisplayData target, ItemDisplayData[] materialItemArray, List<ItemSourceChange> changeList)
			{
				GameDataBridge.AddMethodCall<int, ItemDisplayData[], ItemDisplayData, ItemDisplayData[], List<ItemSourceChange>>(listenerId, 9, 229, charId, tools, target, materialItemArray, changeList);
			}

			// Token: 0x0601199A RID: 72090 RVA: 0x00683803 File Offset: 0x00681A03
			public static void GetCraftManDisplayDataForCharacter(int listenerId, int artisanId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 9, 230, artisanId);
			}

			// Token: 0x0601199B RID: 72091 RVA: 0x00683815 File Offset: 0x00681A15
			public static void GetCraftManDisplayDataForBuilding(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 9, 231, blockKey);
			}

			// Token: 0x0601199C RID: 72092 RVA: 0x00683827 File Offset: 0x00681A27
			public static void GetTeaHorseCaravanEvent(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 232);
			}

			// Token: 0x0601199D RID: 72093 RVA: 0x00683838 File Offset: 0x00681A38
			public static void TriggerCultivateFeatherEvent()
			{
				GameDataBridge.AddMethodCall(-1, 9, 233);
			}

			// Token: 0x0601199E RID: 72094 RVA: 0x00683849 File Offset: 0x00681A49
			public static void GetTeaHorseCaravanData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 234);
			}

			// Token: 0x0601199F RID: 72095 RVA: 0x0068385A File Offset: 0x00681A5A
			public static void QuickDiscardExchangeItem()
			{
				GameDataBridge.AddMethodCall(-1, 9, 235);
			}

			// Token: 0x060119A0 RID: 72096 RVA: 0x0068386B File Offset: 0x00681A6B
			public static void GetTaiwuVillageBuildingDataForVillagerRole(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 236);
			}

			// Token: 0x060119A1 RID: 72097 RVA: 0x0068387C File Offset: 0x00681A7C
			public static void IsAnyChickensCanPluck(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 237);
			}

			// Token: 0x060119A2 RID: 72098 RVA: 0x0068388D File Offset: 0x00681A8D
			public static void FeedChicken(int listenerId, int id, ItemKey itemKey)
			{
				GameDataBridge.AddMethodCall<int, ItemKey>(listenerId, 9, 238, id, itemKey);
			}

			// Token: 0x060119A3 RID: 72099 RVA: 0x006838A0 File Offset: 0x00681AA0
			public static void GetBuildingBlockEffect(int listenerId, short settlementId, EBuildingScaleEffect effectType)
			{
				GameDataBridge.AddMethodCall<short, EBuildingScaleEffect>(listenerId, 9, 239, settlementId, effectType);
			}

			// Token: 0x060119A4 RID: 72100 RVA: 0x006838B3 File Offset: 0x00681AB3
			public static void GetBuildingBlockEffect(int listenerId, short settlementId, EBuildingScaleEffect effectType, int subType)
			{
				GameDataBridge.AddMethodCall<short, EBuildingScaleEffect, int>(listenerId, 9, 239, settlementId, effectType, subType);
			}

			// Token: 0x060119A5 RID: 72101 RVA: 0x006838C7 File Offset: 0x00681AC7
			public static void ClearNewlyCreatedBuildingIndex()
			{
				GameDataBridge.AddMethodCall(-1, 9, 240);
			}

			// Token: 0x060119A6 RID: 72102 RVA: 0x006838D8 File Offset: 0x00681AD8
			public static void GetNewlyCreatedBuildingIndex(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 9, 241);
			}
		}

		// Token: 0x0200261A RID: 9754
		public static class AsyncCall
		{
			// Token: 0x060119A7 RID: 72103 RVA: 0x006838E9 File Offset: 0x00681AE9
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.SetShopManager instead.", true)]
			public static void SetShopManager(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, sbyte index, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119A8 RID: 72104 RVA: 0x006838F1 File Offset: 0x00681AF1
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.SetCollectBuildingResourceType instead.", true)]
			public static void SetCollectBuildingResourceType(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, sbyte resourceType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119A9 RID: 72105 RVA: 0x006838F9 File Offset: 0x00681AF9
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.ClearBuildingBlockEarningsData instead.", true)]
			public static void ClearBuildingBlockEarningsData(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, bool isPawnShop, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119AA RID: 72106 RVA: 0x00683904 File Offset: 0x00681B04
			public static void GetBuildingEarningData(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 3, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119AB RID: 72107 RVA: 0x00683930 File Offset: 0x00681B30
			public static void GetBuildingOperatesData(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 4, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119AC RID: 72108 RVA: 0x0068395C File Offset: 0x00681B5C
			public static void GetBuildingBuildPeopleAttainments(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 5, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119AD RID: 72109 RVA: 0x00683988 File Offset: 0x00681B88
			public static void AcceptBuildingBlockCollectEarning(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, int earningDataIndex, bool isPutInInventory, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, int, bool>(9, 6, key, earningDataIndex, isPutInInventory, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119AE RID: 72110 RVA: 0x006839B8 File Offset: 0x00681BB8
			public static void AcceptBuildingBlockCollectEarning(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, int earningDataIndex, bool isPutInInventory, bool isSetData, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, int, bool, bool>(9, 6, key, earningDataIndex, isPutInInventory, isSetData, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119AF RID: 72111 RVA: 0x006839E8 File Offset: 0x00681BE8
			public static void AcceptBuildingBlockCollectEarning(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, int earningDataIndex, bool isPutInInventory, bool isSetData, bool isCostMoney, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, int, bool, bool, bool>(9, 6, key, earningDataIndex, isPutInInventory, isSetData, isCostMoney, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119B0 RID: 72112 RVA: 0x00683A1A File Offset: 0x00681C1A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.AcceptBuildingBlockCollectEarningQuick instead.", true)]
			public static void AcceptBuildingBlockCollectEarningQuick(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, bool isPutInInventory, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119B1 RID: 72113 RVA: 0x00683A24 File Offset: 0x00681C24
			public static void AcceptBuildingBlockRecruitPeople(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, int earningDataIndex, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, int>(9, 8, key, earningDataIndex, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119B2 RID: 72114 RVA: 0x00683A50 File Offset: 0x00681C50
			public static void AcceptBuildingBlockRecruitPeople(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, int earningDataIndex, bool isSetData, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, int, bool>(9, 8, key, earningDataIndex, isSetData, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119B3 RID: 72115 RVA: 0x00683A80 File Offset: 0x00681C80
			public static void AcceptBuildingBlockRecruitPeopleQuick(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 9, key, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119B4 RID: 72116 RVA: 0x00683AAC File Offset: 0x00681CAC
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.ShopBuildingSoldItemReceive instead.", true)]
			public static void ShopBuildingSoldItemReceive(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, int earningDataIndex, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119B5 RID: 72117 RVA: 0x00683AB4 File Offset: 0x00681CB4
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.ShopBuildingSoldItemReceive instead.", true)]
			public static void ShopBuildingSoldItemReceive(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, int earningDataIndex, bool isSetData, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119B6 RID: 72118 RVA: 0x00683ABC File Offset: 0x00681CBC
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.ShopBuildingSoldItemReceiveQuick instead.", true)]
			public static void ShopBuildingSoldItemReceiveQuick(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119B7 RID: 72119 RVA: 0x00683AC4 File Offset: 0x00681CC4
			public static void QuickCollectShopItem(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 12, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119B8 RID: 72120 RVA: 0x00683AF0 File Offset: 0x00681CF0
			public static void QuickCollectShopItemCount(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 13, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119B9 RID: 72121 RVA: 0x00683B1B File Offset: 0x00681D1B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.QuickCollectShopSoldItem instead.", true)]
			public static void QuickCollectShopSoldItem(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119BA RID: 72122 RVA: 0x00683B24 File Offset: 0x00681D24
			public static void QuickCollectShopSoldItemCount(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 15, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119BB RID: 72123 RVA: 0x00683B50 File Offset: 0x00681D50
			public static void QuickRecruitPeople(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 16, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119BC RID: 72124 RVA: 0x00683B7C File Offset: 0x00681D7C
			public static void QuickRecruitPeopleCount(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 17, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119BD RID: 72125 RVA: 0x00683BA7 File Offset: 0x00681DA7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.QuickCollectBuildingEarn instead.", true)]
			public static void QuickCollectBuildingEarn(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119BE RID: 72126 RVA: 0x00683BB0 File Offset: 0x00681DB0
			public static void QuickCollectBuildingEarnCount(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 19, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119BF RID: 72127 RVA: 0x00683BDB File Offset: 0x00681DDB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.AddFixBook instead.", true)]
			public static void AddFixBook(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, ItemKey itemKey, ItemSourceType itemSourceType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119C0 RID: 72128 RVA: 0x00683BE4 File Offset: 0x00681DE4
			public static void ChangeFixBook(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, ItemKey itemKey, ItemSourceType itemSourceType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, ItemKey, ItemSourceType>(9, 21, key, itemKey, itemSourceType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119C1 RID: 72129 RVA: 0x00683C13 File Offset: 0x00681E13
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.ReceiveFixBook instead.", true)]
			public static void ReceiveFixBook(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, bool isPutInInventory, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119C2 RID: 72130 RVA: 0x00683C1C File Offset: 0x00681E1C
			public static void GetFixBookProgress(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 23, key, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119C3 RID: 72131 RVA: 0x00683C48 File Offset: 0x00681E48
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.SetTeaHorseCaravanState instead.", true)]
			public static void SetTeaHorseCaravanState(IAsyncMethodRequestHandler requestHandler, sbyte state, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119C4 RID: 72132 RVA: 0x00683C50 File Offset: 0x00681E50
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.ExchangeItemToReplenishment instead.", true)]
			public static void ExchangeItemToReplenishment(IAsyncMethodRequestHandler requestHandler, List<ItemKey> carryItems, List<ItemKey> gainItems, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119C5 RID: 72133 RVA: 0x00683C58 File Offset: 0x00681E58
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.StartSearchReplenishment instead.", true)]
			public static void StartSearchReplenishment(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119C6 RID: 72134 RVA: 0x00683C60 File Offset: 0x00681E60
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.QuickGetExchangeItem instead.", true)]
			public static void QuickGetExchangeItem(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119C7 RID: 72135 RVA: 0x00683C68 File Offset: 0x00681E68
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.QuickGetExchangeItem instead.", true)]
			public static void QuickGetExchangeItem(IAsyncMethodRequestHandler requestHandler, ItemSourceType source, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119C8 RID: 72136 RVA: 0x00683C70 File Offset: 0x00681E70
			public static void GetShrineDisplayData(IAsyncMethodRequestHandler requestHandler, short areaId, short blockId, short buildingBlockIndex, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, short, short>(9, 28, areaId, blockId, buildingBlockIndex, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119C9 RID: 72137 RVA: 0x00683C9F File Offset: 0x00681E9F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.TeachSkill instead.", true)]
			public static void TeachSkill(IAsyncMethodRequestHandler requestHandler, int characterId, SkillQualificationBonus bonus, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119CA RID: 72138 RVA: 0x00683CA7 File Offset: 0x00681EA7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.CricketCollectionAdd instead.", true)]
			public static void CricketCollectionAdd(IAsyncMethodRequestHandler requestHandler, int index, bool isCricket, ItemKey itemKey, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119CB RID: 72139 RVA: 0x00683CAF File Offset: 0x00681EAF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.CricketCollectionRemove instead.", true)]
			public static void CricketCollectionRemove(IAsyncMethodRequestHandler requestHandler, int index, bool isCricket, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119CC RID: 72140 RVA: 0x00683CB8 File Offset: 0x00681EB8
			public static void GetCollectionCrickets(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 32, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119CD RID: 72141 RVA: 0x00683CE4 File Offset: 0x00681EE4
			public static void GetCollectionJars(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 33, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119CE RID: 72142 RVA: 0x00683D10 File Offset: 0x00681F10
			public static void GetCollectionCricketRegen(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 34, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119CF RID: 72143 RVA: 0x00683D3C File Offset: 0x00681F3C
			public static void GetAuthorityGain(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 35, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119D0 RID: 72144 RVA: 0x00683D68 File Offset: 0x00681F68
			public static void GmCmd_BuildImmediately(IAsyncMethodRequestHandler requestHandler, short buildingTemplateId, BuildingBlockKey blockKey, sbyte level, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, BuildingBlockKey, sbyte>(9, 36, buildingTemplateId, blockKey, level, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119D1 RID: 72145 RVA: 0x00683D98 File Offset: 0x00681F98
			public static void GmCmd_RemoveBuildingImmediately(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 37, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119D2 RID: 72146 RVA: 0x00683DC4 File Offset: 0x00681FC4
			public static void StartMakeItem(IAsyncMethodRequestHandler requestHandler, StartMakeArguments startMakeArguments, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<StartMakeArguments>(9, 38, startMakeArguments, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119D3 RID: 72147 RVA: 0x00683DF0 File Offset: 0x00681FF0
			public static void CheckMakeCondition(IAsyncMethodRequestHandler requestHandler, MakeConditionArguments makeConditionArguments, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<MakeConditionArguments>(9, 39, makeConditionArguments, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119D4 RID: 72148 RVA: 0x00683E1C File Offset: 0x0068201C
			public static void GetMakeItems(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey buildingBlockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 40, buildingBlockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119D5 RID: 72149 RVA: 0x00683E48 File Offset: 0x00682048
			public static void GetMakingItemData(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey buildingBlockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 41, buildingBlockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119D6 RID: 72150 RVA: 0x00683E74 File Offset: 0x00682074
			public static void CheckRepairConditionIsMeet(IAsyncMethodRequestHandler requestHandler, int charId, ItemKey toolKey, ItemKey itemKey, BuildingBlockKey buildingBlockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ItemKey, ItemKey, BuildingBlockKey>(9, 42, charId, toolKey, itemKey, buildingBlockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119D7 RID: 72151 RVA: 0x00683EA8 File Offset: 0x006820A8
			public static void AddItemPoison(IAsyncMethodRequestHandler requestHandler, int charId, ItemDisplayData tool, ItemDisplayData target, ItemDisplayData[] poisons, List<ItemDisplayData> condensePoisonItemList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ItemDisplayData, ItemDisplayData, ItemDisplayData[], List<ItemDisplayData>>(9, 43, charId, tool, target, poisons, condensePoisonItemList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119D8 RID: 72152 RVA: 0x00683EDC File Offset: 0x006820DC
			public static void CheckAddPoisonCondition(IAsyncMethodRequestHandler requestHandler, int charId, ItemKey toolKey, ItemKey targetKey, ItemKey[] poisonKeys, BuildingBlockKey buildingBlockKey, FullPoisonEffects tempPoisonEffects, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ItemKey, ItemKey, ItemKey[], BuildingBlockKey, FullPoisonEffects>(9, 44, charId, toolKey, targetKey, poisonKeys, buildingBlockKey, tempPoisonEffects, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119D9 RID: 72153 RVA: 0x00683F14 File Offset: 0x00682114
			public static void RemoveItemPoison(IAsyncMethodRequestHandler requestHandler, int charId, ItemDisplayData tool, ItemDisplayData target, ItemDisplayData[] medicines, bool isExtract, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ItemDisplayData, ItemDisplayData, ItemDisplayData[], bool>(9, 45, charId, tool, target, medicines, isExtract, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119DA RID: 72154 RVA: 0x00683F48 File Offset: 0x00682148
			public static void CheckRemovePoisonCondition(IAsyncMethodRequestHandler requestHandler, int charId, ItemKey toolKey, ItemKey targetKey, ItemKey[] medicineKeys, BuildingBlockKey buildingBlockKey, bool isExtract, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ItemKey, ItemKey, ItemKey[], BuildingBlockKey, bool>(9, 46, charId, toolKey, targetKey, medicineKeys, buildingBlockKey, isExtract, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119DB RID: 72155 RVA: 0x00683F80 File Offset: 0x00682180
			public static void Build(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, short buildingTemplateId, int[] workers, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, short, int[]>(9, 47, blockKey, buildingTemplateId, workers, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119DC RID: 72156 RVA: 0x00683FB0 File Offset: 0x006821B0
			public static void Remove(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, int[] workers, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, int[]>(9, 48, blockKey, workers, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119DD RID: 72157 RVA: 0x00683FE0 File Offset: 0x006821E0
			public static void SetStopOperation(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, bool stop, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, bool>(9, 49, blockKey, stop, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119DE RID: 72158 RVA: 0x0068400D File Offset: 0x0068220D
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.SetOperator instead.", true)]
			public static void SetOperator(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, sbyte index, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119DF RID: 72159 RVA: 0x00684018 File Offset: 0x00682218
			public static void Repair(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 51, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119E0 RID: 72160 RVA: 0x00684044 File Offset: 0x00682244
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.ConfirmPlanBuilding instead.", true)]
			public static void ConfirmPlanBuilding(IAsyncMethodRequestHandler requestHandler, List<IntPair> operateRecord, Location location, List<int> sameSet, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119E1 RID: 72161 RVA: 0x0068404C File Offset: 0x0068224C
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.AddToResidence instead.", true)]
			public static void AddToResidence(IAsyncMethodRequestHandler requestHandler, int charId, BuildingBlockKey buildingBlockKey, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119E2 RID: 72162 RVA: 0x00684054 File Offset: 0x00682254
			public static void RemoveFromResidence(IAsyncMethodRequestHandler requestHandler, int charId, BuildingBlockKey buildingBlockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, BuildingBlockKey>(9, 54, charId, buildingBlockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119E3 RID: 72163 RVA: 0x00684084 File Offset: 0x00682284
			public static void ReplaceCharacterInResidence(IAsyncMethodRequestHandler requestHandler, int charId, BuildingBlockKey buildingBlockKey, sbyte index, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, BuildingBlockKey, sbyte>(9, 55, charId, buildingBlockKey, index, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119E4 RID: 72164 RVA: 0x006840B4 File Offset: 0x006822B4
			public static void ReplaceCharacterInComfortableHouse(IAsyncMethodRequestHandler requestHandler, int charIdB, BuildingBlockKey buildingBlockKey, sbyte index, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, BuildingBlockKey, sbyte>(9, 56, charIdB, buildingBlockKey, index, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119E5 RID: 72165 RVA: 0x006840E4 File Offset: 0x006822E4
			public static void AddToComfortableHouse(IAsyncMethodRequestHandler requestHandler, int charId, BuildingBlockKey buildingBlockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, BuildingBlockKey>(9, 57, charId, buildingBlockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119E6 RID: 72166 RVA: 0x00684114 File Offset: 0x00682314
			public static void RemoveFromComfortableHouse(IAsyncMethodRequestHandler requestHandler, int charId, BuildingBlockKey buildingBlockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, BuildingBlockKey>(9, 58, charId, buildingBlockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119E7 RID: 72167 RVA: 0x00684144 File Offset: 0x00682344
			public static void QuickFillResidence(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey buildingBlockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 59, buildingBlockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119E8 RID: 72168 RVA: 0x00684170 File Offset: 0x00682370
			public static void GetCharsInResidence(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 60, key, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119E9 RID: 72169 RVA: 0x0068419C File Offset: 0x0068239C
			public static void GetAllResidents(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, bool homelessFirst, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, bool>(9, 61, blockKey, homelessFirst, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119EA RID: 72170 RVA: 0x006841CC File Offset: 0x006823CC
			public static void GetCharsInComfortableHouse(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 62, key, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119EB RID: 72171 RVA: 0x006841F8 File Offset: 0x006823F8
			public static void GetHomeless(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 63, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119EC RID: 72172 RVA: 0x00684224 File Offset: 0x00682424
			public static void GetSamsaraPlatformCharList(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 64, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119ED RID: 72173 RVA: 0x0068424F File Offset: 0x0068244F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.SetSamsaraPlatformChar instead.", true)]
			public static void SetSamsaraPlatformChar(IAsyncMethodRequestHandler requestHandler, sbyte destinyType, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119EE RID: 72174 RVA: 0x00684258 File Offset: 0x00682458
			public static void SamsaraPlatformReborn(IAsyncMethodRequestHandler requestHandler, sbyte destinyType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(9, 66, destinyType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119EF RID: 72175 RVA: 0x00684284 File Offset: 0x00682484
			public static void GetBuildingAreaData(IAsyncMethodRequestHandler requestHandler, Location location, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Location>(9, 67, location, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119F0 RID: 72176 RVA: 0x006842B0 File Offset: 0x006824B0
			public static void GetBuildingBlockList(IAsyncMethodRequestHandler requestHandler, Location location, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Location>(9, 68, location, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119F1 RID: 72177 RVA: 0x006842DC File Offset: 0x006824DC
			public static void GetBuildingBlockData(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 69, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119F2 RID: 72178 RVA: 0x00684308 File Offset: 0x00682508
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.SetBuildingCustomName instead.", true)]
			public static void SetBuildingCustomName(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, string name, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119F3 RID: 72179 RVA: 0x00684310 File Offset: 0x00682510
			public static void GetEmptyBlockCount(IAsyncMethodRequestHandler requestHandler, short areaId, short blockId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, short>(9, 71, areaId, blockId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119F4 RID: 72180 RVA: 0x00684340 File Offset: 0x00682540
			public static void AddChicken(IAsyncMethodRequestHandler requestHandler, int settlementId, short templateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short>(9, 72, settlementId, templateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119F5 RID: 72181 RVA: 0x0068436D File Offset: 0x0068256D
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.RemoveChicken instead.", true)]
			public static void RemoveChicken(IAsyncMethodRequestHandler requestHandler, int id, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119F6 RID: 72182 RVA: 0x00684375 File Offset: 0x00682575
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.RemoveAllChicken instead.", true)]
			public static void RemoveAllChicken(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119F7 RID: 72183 RVA: 0x0068437D File Offset: 0x0068257D
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.MoveChicken instead.", true)]
			public static void MoveChicken(IAsyncMethodRequestHandler requestHandler, int id, int targetSettlementId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119F8 RID: 72184 RVA: 0x00684385 File Offset: 0x00682585
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.TransferChicken instead.", true)]
			public static void TransferChicken(IAsyncMethodRequestHandler requestHandler, int id, int targetSettlementId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119F9 RID: 72185 RVA: 0x00684390 File Offset: 0x00682590
			public static void GetSettlementChickenList(IAsyncMethodRequestHandler requestHandler, int sourceSettlementId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(9, 77, sourceSettlementId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119FA RID: 72186 RVA: 0x006843BC File Offset: 0x006825BC
			public static void GetSettlementChickenList(IAsyncMethodRequestHandler requestHandler, int sourceSettlementId, bool ignoreFulong, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, bool>(9, 77, sourceSettlementId, ignoreFulong, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119FB RID: 72187 RVA: 0x006843EC File Offset: 0x006825EC
			public static void GetChickenData(IAsyncMethodRequestHandler requestHandler, int id, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(9, 78, id, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119FC RID: 72188 RVA: 0x00684418 File Offset: 0x00682618
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.InitMapBlockChicken instead.", true)]
			public static void InitMapBlockChicken(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119FD RID: 72189 RVA: 0x00684420 File Offset: 0x00682620
			public static void IsHaveChickenKing(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 80, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060119FE RID: 72190 RVA: 0x0068444B File Offset: 0x0068264B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.RemoveAllFormResidence instead.", true)]
			public static void RemoveAllFormResidence(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey buildingBlockKey, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060119FF RID: 72191 RVA: 0x00684454 File Offset: 0x00682654
			public static void GetBuildingAttainment(IAsyncMethodRequestHandler requestHandler, BuildingBlockData blockData, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockData, BuildingBlockKey>(9, 82, blockData, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A00 RID: 72192 RVA: 0x00684484 File Offset: 0x00682684
			public static void GetBuildingAttainment(IAsyncMethodRequestHandler requestHandler, BuildingBlockData blockData, BuildingBlockKey blockKey, bool isAverage, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockData, BuildingBlockKey, bool>(9, 82, blockData, blockKey, isAverage, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A01 RID: 72193 RVA: 0x006844B4 File Offset: 0x006826B4
			public static void CalcResourceOutputCount(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, sbyte resourceType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, sbyte>(9, 83, key, resourceType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A02 RID: 72194 RVA: 0x006844E1 File Offset: 0x006826E1
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.DealInfectedPeople instead.", true)]
			public static void DealInfectedPeople(IAsyncMethodRequestHandler requestHandler, List<int> charList, byte dealType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A03 RID: 72195 RVA: 0x006844EC File Offset: 0x006826EC
			public static void QuickCollectSingleShopItem(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 85, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A04 RID: 72196 RVA: 0x00684518 File Offset: 0x00682718
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.QuickCollectSingleShopSoldItem instead.", true)]
			public static void QuickCollectSingleShopSoldItem(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A05 RID: 72197 RVA: 0x00684520 File Offset: 0x00682720
			public static void QuickRecruitSingleBuildingPeople(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 87, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A06 RID: 72198 RVA: 0x0068454C File Offset: 0x0068274C
			public static void QuickFillComfortableHouse(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey buildingBlockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 88, buildingBlockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A07 RID: 72199 RVA: 0x00684578 File Offset: 0x00682778
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.RemoveAllFromComfortableHouse instead.", true)]
			public static void RemoveAllFromComfortableHouse(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey buildingBlockKey, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A08 RID: 72200 RVA: 0x00684580 File Offset: 0x00682780
			public static void SortedComfortableHousePeople(IAsyncMethodRequestHandler requestHandler, List<int> charIdList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(9, 90, charIdList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A09 RID: 72201 RVA: 0x006845AC File Offset: 0x006827AC
			public static void GetMakeResult(IAsyncMethodRequestHandler requestHandler, short materialTemplateId, ItemKey toolKey, BuildingBlockKey buildingBlockKey, sbyte lifeSkillType, List<short> makeItemSubtypeIdList, short makeItemSubTypeId, bool isPerfect, bool isManual, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, ItemKey, BuildingBlockKey, sbyte, List<short>, short, bool, bool>(9, 91, materialTemplateId, toolKey, buildingBlockKey, lifeSkillType, makeItemSubtypeIdList, makeItemSubTypeId, isPerfect, isManual, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A0A RID: 72202 RVA: 0x006845E8 File Offset: 0x006827E8
			public static void GetSutraReadingRoomBuffValue(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 92, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A0B RID: 72203 RVA: 0x00684613 File Offset: 0x00682813
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.SetBuildingAutoWork instead.", true)]
			public static void SetBuildingAutoWork(IAsyncMethodRequestHandler requestHandler, short blockIndex, bool isAutoWork, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A0C RID: 72204 RVA: 0x0068461C File Offset: 0x0068281C
			public static void GetBuildingIsAutoWork(IAsyncMethodRequestHandler requestHandler, short blockIndex, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(9, 94, blockIndex, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A0D RID: 72205 RVA: 0x00684648 File Offset: 0x00682848
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.ShopBuildingMultiChangeSoldItem instead.", true)]
			public static void ShopBuildingMultiChangeSoldItem(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, List<ItemKey> itemList, List<int> operateTypeList, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A0E RID: 72206 RVA: 0x00684650 File Offset: 0x00682850
			public static void RepairItemList(IAsyncMethodRequestHandler requestHandler, int charId, List<MultiplyOperation> operationList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, List<MultiplyOperation>>(9, 96, charId, operationList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A0F RID: 72207 RVA: 0x0068467D File Offset: 0x0068287D
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.SetBuildingAutoSold instead.", true)]
			public static void SetBuildingAutoSold(IAsyncMethodRequestHandler requestHandler, short blockIndex, bool isAutoSold, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A10 RID: 72208 RVA: 0x00684688 File Offset: 0x00682888
			public static void GetBuildingIsAutoSold(IAsyncMethodRequestHandler requestHandler, short blockIndex, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(9, 98, blockIndex, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A11 RID: 72209 RVA: 0x006846B4 File Offset: 0x006828B4
			public static void GetXiangshuIdInKungfuRoom(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 99, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A12 RID: 72210 RVA: 0x006846E0 File Offset: 0x006828E0
			public static void RepairItemOptional(IAsyncMethodRequestHandler requestHandler, int charId, ItemKey toolKey, ItemKey itemKey, sbyte toolSourceType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ItemKey, ItemKey, sbyte>(9, 100, charId, toolKey, itemKey, toolSourceType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A13 RID: 72211 RVA: 0x00684711 File Offset: 0x00682911
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.SetNickNameByChickenId instead.", true)]
			public static void SetNickNameByChickenId(IAsyncMethodRequestHandler requestHandler, int id, string nickname, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A14 RID: 72212 RVA: 0x0068471C File Offset: 0x0068291C
			public static void GetSettlementChickenDataList(IAsyncMethodRequestHandler requestHandler, Location location, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Location>(9, 102, location, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A15 RID: 72213 RVA: 0x00684748 File Offset: 0x00682948
			public static void GetSettlementChickenDataList(IAsyncMethodRequestHandler requestHandler, Location location, bool ignoreFulong, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Location, bool>(9, 102, location, ignoreFulong, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A16 RID: 72214 RVA: 0x00684775 File Offset: 0x00682975
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.SetTeaHorseCaravanWeather instead.", true)]
			public static void SetTeaHorseCaravanWeather(IAsyncMethodRequestHandler requestHandler, short weatherId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A17 RID: 72215 RVA: 0x00684780 File Offset: 0x00682980
			public static void GetComfortableIsAutoCheckIn(IAsyncMethodRequestHandler requestHandler, short blockIndex, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(9, 104, blockIndex, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A18 RID: 72216 RVA: 0x006847AC File Offset: 0x006829AC
			public static void GetResidenceIsAutoCheckIn(IAsyncMethodRequestHandler requestHandler, short blockIndex, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(9, 105, blockIndex, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A19 RID: 72217 RVA: 0x006847D8 File Offset: 0x006829D8
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.SetComfortableAutoCheckIn instead.", true)]
			public static void SetComfortableAutoCheckIn(IAsyncMethodRequestHandler requestHandler, short blockIndex, bool isAutoCheckIn, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A1A RID: 72218 RVA: 0x006847E0 File Offset: 0x006829E0
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.SetResidenceAutoCheckIn instead.", true)]
			public static void SetResidenceAutoCheckIn(IAsyncMethodRequestHandler requestHandler, short blockIndex, bool isAutoCheckIn, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A1B RID: 72219 RVA: 0x006847E8 File Offset: 0x006829E8
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.GmCmd_AddLegacyBuilding instead.", true)]
			public static void GmCmd_AddLegacyBuilding(IAsyncMethodRequestHandler requestHandler, short buildingTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A1C RID: 72220 RVA: 0x006847F0 File Offset: 0x006829F0
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.SetUnlockedWorkingVillagers instead.", true)]
			public static void SetUnlockedWorkingVillagers(IAsyncMethodRequestHandler requestHandler, int charId, bool add, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A1D RID: 72221 RVA: 0x006847F8 File Offset: 0x006829F8
			public static void WeaveClothingItem(IAsyncMethodRequestHandler requestHandler, ItemDisplayData tool, ItemDisplayData target, short weaveClothingTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemDisplayData, ItemDisplayData, short>(9, 110, tool, target, weaveClothingTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A1E RID: 72222 RVA: 0x00684828 File Offset: 0x00682A28
			public static void GmCmd_GetChickenData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 111, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A1F RID: 72223 RVA: 0x00684854 File Offset: 0x00682A54
			public static void GetPossessionPreview(IAsyncMethodRequestHandler requestHandler, List<int> soulCharIds, int bodyCharId, List<short> featureIds, int previewId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>, int, List<short>, int>(9, 112, soulCharIds, bodyCharId, featureIds, previewId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A20 RID: 72224 RVA: 0x00684888 File Offset: 0x00682A88
			public static void TrySwapSoulCeremony(IAsyncMethodRequestHandler requestHandler, List<int> soulCharIds, int bodyCharId, List<short> featureIds, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>, int, List<short>>(9, 113, soulCharIds, bodyCharId, featureIds, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A21 RID: 72225 RVA: 0x006848B7 File Offset: 0x00682AB7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.GetBackTeaHorseCarryItem instead.", true)]
			public static void GetBackTeaHorseCarryItem(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, sbyte itemSource, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A22 RID: 72226 RVA: 0x006848BF File Offset: 0x00682ABF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.AddItemToTeaHorseCarryItem instead.", true)]
			public static void AddItemToTeaHorseCarryItem(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, sbyte itemSource, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A23 RID: 72227 RVA: 0x006848C7 File Offset: 0x00682AC7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.SetTemporaryPossessionCharacterAvatar instead.", true)]
			public static void SetTemporaryPossessionCharacterAvatar(IAsyncMethodRequestHandler requestHandler, AvatarData avatar, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A24 RID: 72228 RVA: 0x006848D0 File Offset: 0x00682AD0
			public static void GetSwapSoulCeremonyBodyCharIdList(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 117, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A25 RID: 72229 RVA: 0x006848FC File Offset: 0x00682AFC
			public static void GetBuildingShopManagerAutoArrangeSorted(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, int[] managerCharacterIds, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, int[]>(9, 118, blockKey, managerCharacterIds, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A26 RID: 72230 RVA: 0x00684929 File Offset: 0x00682B29
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.SectMainStoryJingangClickMonkSoulBtn instead.", true)]
			public static void SectMainStoryJingangClickMonkSoulBtn(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A27 RID: 72231 RVA: 0x00684931 File Offset: 0x00682B31
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.RejectBuildingBlockRecruitPeople instead.", true)]
			public static void RejectBuildingBlockRecruitPeople(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, int earningDataIndex, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A28 RID: 72232 RVA: 0x00684939 File Offset: 0x00682B39
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.RejectBuildingBlockRecruitPeople instead.", true)]
			public static void RejectBuildingBlockRecruitPeople(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, int earningDataIndex, bool isSetData, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A29 RID: 72233 RVA: 0x00684941 File Offset: 0x00682B41
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.RejectBuildingBlockRecruitPeopleQuick instead.", true)]
			public static void RejectBuildingBlockRecruitPeopleQuick(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A2A RID: 72234 RVA: 0x0068494C File Offset: 0x00682B4C
			public static void GetShopManagementYieldTipsData(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 122, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A2B RID: 72235 RVA: 0x00684978 File Offset: 0x00682B78
			public static void CalculateBuildingManageHarvestSuccessRate(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 123, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A2C RID: 72236 RVA: 0x006849A4 File Offset: 0x00682BA4
			public static void GetOrCreateShopEventCollection(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey buildingBlockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 124, buildingBlockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A2D RID: 72237 RVA: 0x006849D0 File Offset: 0x00682BD0
			public static void GetSamsaraPlatformRecord(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 125, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A2E RID: 72238 RVA: 0x006849FC File Offset: 0x00682BFC
			public static void GetSwapSoulCeremonySoulCharIdList(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 126, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A2F RID: 72239 RVA: 0x00684A27 File Offset: 0x00682C27
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.CricketCollectionBatchAddCricketJar instead.", true)]
			public static void CricketCollectionBatchAddCricketJar(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A30 RID: 72240 RVA: 0x00684A2F File Offset: 0x00682C2F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.CricketCollectionBatchAddCricket instead.", true)]
			public static void CricketCollectionBatchAddCricket(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A31 RID: 72241 RVA: 0x00684A37 File Offset: 0x00682C37
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.CricketCollectionBatchRemoveJar instead.", true)]
			public static void CricketCollectionBatchRemoveJar(IAsyncMethodRequestHandler requestHandler, ItemSourceType sourceType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A32 RID: 72242 RVA: 0x00684A3F File Offset: 0x00682C3F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.CricketCollectionBatchRemoveCricket instead.", true)]
			public static void CricketCollectionBatchRemoveCricket(IAsyncMethodRequestHandler requestHandler, ItemSourceType sourceType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A33 RID: 72243 RVA: 0x00684A48 File Offset: 0x00682C48
			public static void GetCricketOrJarFromSourceStorage(IAsyncMethodRequestHandler requestHandler, short itemSubType, ItemSourceType sourceType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, ItemSourceType>(9, 131, itemSubType, sourceType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A34 RID: 72244 RVA: 0x00684A78 File Offset: 0x00682C78
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.SmartOperateCricketOrJarCollection instead.", true)]
			public static void SmartOperateCricketOrJarCollection(IAsyncMethodRequestHandler requestHandler, int collectionIndex, short itemSubType, ItemSourceType sourceType, ItemKey itemKey, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A35 RID: 72245 RVA: 0x00684A80 File Offset: 0x00682C80
			public static void GetBatchButtonEnableState(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 133, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A36 RID: 72246 RVA: 0x00684AB0 File Offset: 0x00682CB0
			public static void CalculateBuildingManageHarvestSuccessRates(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 134, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A37 RID: 72247 RVA: 0x00684AE0 File Offset: 0x00682CE0
			public static void UnsetFulongChicken(IAsyncMethodRequestHandler requestHandler, short orgMemberTemplateId, int chickenId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, int>(9, 135, orgMemberTemplateId, chickenId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A38 RID: 72248 RVA: 0x00684B10 File Offset: 0x00682D10
			public static void SetFulongChicken(IAsyncMethodRequestHandler requestHandler, short orgMemberTemplateId, int chickenId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, int>(9, 136, orgMemberTemplateId, chickenId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A39 RID: 72249 RVA: 0x00684B40 File Offset: 0x00682D40
			public static void GetChickenDataList(IAsyncMethodRequestHandler requestHandler, List<int> idList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(9, 137, idList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A3A RID: 72250 RVA: 0x00684B70 File Offset: 0x00682D70
			public static void GetChickenNicknameList(IAsyncMethodRequestHandler requestHandler, List<int> chickenIdList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(9, 138, chickenIdList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A3B RID: 72251 RVA: 0x00684BA0 File Offset: 0x00682DA0
			public static void GetSettlementChickenIdList(IAsyncMethodRequestHandler requestHandler, Location location, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Location>(9, 139, location, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A3C RID: 72252 RVA: 0x00684BD0 File Offset: 0x00682DD0
			public static void GetChickensNicknameByLocation(IAsyncMethodRequestHandler requestHandler, Location location, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Location>(9, 140, location, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A3D RID: 72253 RVA: 0x00684C00 File Offset: 0x00682E00
			public static void AllChickenInTaiwuVillage(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 141, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A3E RID: 72254 RVA: 0x00684C30 File Offset: 0x00682E30
			public static void GetVillagerRoleExtraEffectUnlockState(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 142, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A3F RID: 72255 RVA: 0x00684C60 File Offset: 0x00682E60
			public static void ClickChickenMap(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 143, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A40 RID: 72256 RVA: 0x00684C90 File Offset: 0x00682E90
			public static void ClickChickenMap(IAsyncMethodRequestHandler requestHandler, bool ignoreTask, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool>(9, 143, ignoreTask, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A41 RID: 72257 RVA: 0x00684CBF File Offset: 0x00682EBF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.ClickChickenSign instead.", true)]
			public static void ClickChickenSign(IAsyncMethodRequestHandler requestHandler, int chickenId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A42 RID: 72258 RVA: 0x00684CC7 File Offset: 0x00682EC7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.SetBuildingResourceOutputSetting instead.", true)]
			public static void SetBuildingResourceOutputSetting(IAsyncMethodRequestHandler requestHandler, int blockIndex, BuildingResourceOutputSetting setting, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A43 RID: 72259 RVA: 0x00684CD0 File Offset: 0x00682ED0
			public static void GetBuildingResourceOutputSetting(IAsyncMethodRequestHandler requestHandler, int blockIndex, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(9, 146, blockIndex, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A44 RID: 72260 RVA: 0x00684D00 File Offset: 0x00682F00
			public static void GetBuildingExceptionData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 147, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A45 RID: 72261 RVA: 0x00684D30 File Offset: 0x00682F30
			public static void AllDependBuildingAvailable(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 148, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A46 RID: 72262 RVA: 0x00684D60 File Offset: 0x00682F60
			public static void PracticingCombatSkillInPracticeRoom(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, short skillTemplateId, int count, int cost, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, short, int, int>(9, 149, blockKey, skillTemplateId, count, cost, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A47 RID: 72263 RVA: 0x00684D94 File Offset: 0x00682F94
			public static void HasShopManagerLeader(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 150, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A48 RID: 72264 RVA: 0x00684DC4 File Offset: 0x00682FC4
			public static void QuickArrangeShopManager(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 151, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A49 RID: 72265 RVA: 0x00684DF4 File Offset: 0x00682FF4
			public static void QuickArrangeShopManager(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, bool onlyCheck, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, bool>(9, 151, blockKey, onlyCheck, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A4A RID: 72266 RVA: 0x00684E24 File Offset: 0x00683024
			public static void QuickArrangeBuildOperator(IAsyncMethodRequestHandler requestHandler, short buildingTemplateId, BuildingBlockKey blockKey, sbyte operationType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, BuildingBlockKey, sbyte>(9, 152, buildingTemplateId, blockKey, operationType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A4B RID: 72267 RVA: 0x00684E58 File Offset: 0x00683058
			public static void QuickArrangeBuildOperator(IAsyncMethodRequestHandler requestHandler, short buildingTemplateId, BuildingBlockKey blockKey, sbyte operationType, List<int> exceptCharList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, BuildingBlockKey, sbyte, List<int>>(9, 152, buildingTemplateId, blockKey, operationType, exceptCharList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A4C RID: 72268 RVA: 0x00684E8C File Offset: 0x0068308C
			public static void ShopBuildingCanTeach(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 153, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A4D RID: 72269 RVA: 0x00684EBC File Offset: 0x006830BC
			public static void GetOperationLeftTime(IAsyncMethodRequestHandler requestHandler, short buildingTemplateId, BuildingBlockKey blockKey, sbyte operationType, List<int> operatorList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, BuildingBlockKey, sbyte, List<int>>(9, 154, buildingTemplateId, blockKey, operationType, operatorList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A4E RID: 72270 RVA: 0x00684EF0 File Offset: 0x006830F0
			public static void GetBuildingOperationLeftTime(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, sbyte operationType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, sbyte>(9, 155, blockKey, operationType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A4F RID: 72271 RVA: 0x00684F20 File Offset: 0x00683120
			public static void GetShopBuildingTeachBookData(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, int memberId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, int>(9, 156, blockKey, memberId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A50 RID: 72272 RVA: 0x00684F50 File Offset: 0x00683150
			public static void CalcExtraTaiwuGroupMaxCountByStrategyRoom(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 157, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A51 RID: 72273 RVA: 0x00684F80 File Offset: 0x00683180
			public static void GetTaiwuCanFixBookItemDataList(IAsyncMethodRequestHandler requestHandler, ItemSourceType itemSourceType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemSourceType>(9, 158, itemSourceType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A52 RID: 72274 RVA: 0x00684FB0 File Offset: 0x006831B0
			public static void GetResidenceInfo(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 159, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A53 RID: 72275 RVA: 0x00684FE0 File Offset: 0x006831E0
			public static void GetTaiwuVillageResourceBlockEffect(IAsyncMethodRequestHandler requestHandler, EBuildingScaleEffect effectType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<EBuildingScaleEffect>(9, 160, effectType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A54 RID: 72276 RVA: 0x00685010 File Offset: 0x00683210
			public static void GetTaiwuLocationResourceBlockEffect(IAsyncMethodRequestHandler requestHandler, EBuildingScaleEffect effectType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<EBuildingScaleEffect>(9, 161, effectType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A55 RID: 72277 RVA: 0x00685040 File Offset: 0x00683240
			public static void GetTaiwuVillageResourceBlockEffectInfo(IAsyncMethodRequestHandler requestHandler, short templateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(9, 162, templateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A56 RID: 72278 RVA: 0x00685070 File Offset: 0x00683270
			public static void CanQuickArrangeShopManager(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 163, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A57 RID: 72279 RVA: 0x006850A0 File Offset: 0x006832A0
			public static void GetBuildingFormulaContextBridge(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 164, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A58 RID: 72280 RVA: 0x006850D0 File Offset: 0x006832D0
			public static void GetBuildingEffectForMake(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey buildingBlockKey, sbyte skillType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, sbyte>(9, 165, buildingBlockKey, skillType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A59 RID: 72281 RVA: 0x00685100 File Offset: 0x00683300
			public static void GmCmd_BuildingCollectPerform(IAsyncMethodRequestHandler requestHandler, int totalAttainment, short buildingTemplateId, int repeat, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short, int>(9, 166, totalAttainment, buildingTemplateId, repeat, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A5A RID: 72282 RVA: 0x00685134 File Offset: 0x00683334
			public static void GmCmd_BeatMinionPerform(IAsyncMethodRequestHandler requestHandler, sbyte grade, int repeat, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte, int>(9, 167, grade, repeat, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A5B RID: 72283 RVA: 0x00685164 File Offset: 0x00683364
			public static void GetStoreLocation(IAsyncMethodRequestHandler requestHandler, int type, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(9, 168, type, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A5C RID: 72284 RVA: 0x00685193 File Offset: 0x00683393
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.SetStoreLocation instead.", true)]
			public static void SetStoreLocation(IAsyncMethodRequestHandler requestHandler, int type, int value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A5D RID: 72285 RVA: 0x0068519C File Offset: 0x0068339C
			public static void GetFeastTargetCharList(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey buildingBlockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 170, buildingBlockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A5E RID: 72286 RVA: 0x006851CB File Offset: 0x006833CB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.TryShowNotifications instead.", true)]
			public static void TryShowNotifications(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A5F RID: 72287 RVA: 0x006851D3 File Offset: 0x006833D3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.QuickRemoveShopSoldItem instead.", true)]
			public static void QuickRemoveShopSoldItem(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A60 RID: 72288 RVA: 0x006851DB File Offset: 0x006833DB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.QuickAddShopSoldItem instead.", true)]
			public static void QuickAddShopSoldItem(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A61 RID: 72289 RVA: 0x006851E4 File Offset: 0x006833E4
			public static void CalcTaiwuVillagerInfoDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 174, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A62 RID: 72290 RVA: 0x00685214 File Offset: 0x00683414
			public static void CalcTaiwuVillagerEfficiencyInBuilding(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, int>(9, 175, blockKey, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A63 RID: 72291 RVA: 0x00685244 File Offset: 0x00683444
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.QuickGetSpecificExchangeItem instead.", true)]
			public static void QuickGetSpecificExchangeItem(IAsyncMethodRequestHandler requestHandler, ItemKey key, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A64 RID: 72292 RVA: 0x0068524C File Offset: 0x0068344C
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.QuickGetSpecificExchangeItem instead.", true)]
			public static void QuickGetSpecificExchangeItem(IAsyncMethodRequestHandler requestHandler, ItemKey key, ItemSourceType source, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A65 RID: 72293 RVA: 0x00685254 File Offset: 0x00683454
			public static void AddLocationMark(IAsyncMethodRequestHandler requestHandler, Location location, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Location>(9, 177, location, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A66 RID: 72294 RVA: 0x00685284 File Offset: 0x00683484
			public static void RemoveLocationMark(IAsyncMethodRequestHandler requestHandler, Location location, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Location>(9, 178, location, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A67 RID: 72295 RVA: 0x006852B4 File Offset: 0x006834B4
			public static void RequestUnlockedWorkingVillagers(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 179, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A68 RID: 72296 RVA: 0x006852E2 File Offset: 0x006834E2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.SetBuildingArrangementSetting instead.", true)]
			public static void SetBuildingArrangementSetting(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, BuildingOptionAutoGiveMemberPreset setting, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A69 RID: 72297 RVA: 0x006852EC File Offset: 0x006834EC
			public static void UpgradeResourceBuilding(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, int level, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, int>(9, 181, key, level, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A6A RID: 72298 RVA: 0x0068531C File Offset: 0x0068351C
			public static void UpgradeSlotBuilding(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, int levelSlotIndex, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, int>(9, 182, key, levelSlotIndex, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A6B RID: 72299 RVA: 0x0068534C File Offset: 0x0068354C
			public static void UnlockBuildingLevelSlot(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, int index, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, int>(9, 183, key, index, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A6C RID: 72300 RVA: 0x0068537C File Offset: 0x0068357C
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.SetBuildingSoldItemSetting instead.", true)]
			public static void SetBuildingSoldItemSetting(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey key, BuildingOptionAutoAddSoldItemPreset setting, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A6D RID: 72301 RVA: 0x00685384 File Offset: 0x00683584
			public static void GetPuppetPageDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 185, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A6E RID: 72302 RVA: 0x006853B2 File Offset: 0x006835B2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.QuickRepairAllBuilding instead.", true)]
			public static void QuickRepairAllBuilding(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A6F RID: 72303 RVA: 0x006853BC File Offset: 0x006835BC
			public static void CalcQuickRepairAllBuildingCostMoney(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 187, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A70 RID: 72304 RVA: 0x006853EC File Offset: 0x006835EC
			public static void GetBuildingFunctionData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 188, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A71 RID: 72305 RVA: 0x0068541C File Offset: 0x0068361C
			public static void GetTaiwuVillageBuildingAreaData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 189, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A72 RID: 72306 RVA: 0x0068544C File Offset: 0x0068364C
			public static void GetAllPawnShopItem(IAsyncMethodRequestHandler requestHandler, bool getItem, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool>(9, 190, getItem, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A73 RID: 72307 RVA: 0x0068547C File Offset: 0x0068367C
			public static void GetTaiwuVillageBlockEffectInfo(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 191, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A74 RID: 72308 RVA: 0x006854AC File Offset: 0x006836AC
			public static void GetTaiwuVillageShopData(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 192, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A75 RID: 72309 RVA: 0x006854DC File Offset: 0x006836DC
			public static void GetTaiwuVillageShopData(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, EBuildingScaleEffect effectType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, EBuildingScaleEffect>(9, 192, blockKey, effectType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A76 RID: 72310 RVA: 0x0068550C File Offset: 0x0068370C
			public static void GetBuildingEarningDisplayData(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 193, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A77 RID: 72311 RVA: 0x0068553C File Offset: 0x0068373C
			public static void GetBuildingManageDisplayData(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 194, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A78 RID: 72312 RVA: 0x0068556C File Offset: 0x0068376C
			public static void GetUnlockedFeastTypeList(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 195, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A79 RID: 72313 RVA: 0x0068559C File Offset: 0x0068379C
			public static void GetReversedSamsaraRecord(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 196, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A7A RID: 72314 RVA: 0x006855CC File Offset: 0x006837CC
			public static void GetLockedComfortableHouseCharacters(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey buildingBlockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 197, buildingBlockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A7B RID: 72315 RVA: 0x006855FC File Offset: 0x006837FC
			public static void GetLockedResidenceCharacters(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey buildingBlockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 198, buildingBlockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A7C RID: 72316 RVA: 0x0068562C File Offset: 0x0068382C
			public static void UnlockComfortableHouseCharacter(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey buildingBlockKey, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, int>(9, 199, buildingBlockKey, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A7D RID: 72317 RVA: 0x0068565C File Offset: 0x0068385C
			public static void LockComfortableHouseCharacter(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey buildingBlockKey, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, int>(9, 200, buildingBlockKey, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A7E RID: 72318 RVA: 0x0068568C File Offset: 0x0068388C
			public static void UnlockResidenceCharacter(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey buildingBlockKey, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, int>(9, 201, buildingBlockKey, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A7F RID: 72319 RVA: 0x006856BC File Offset: 0x006838BC
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.SetComfortableAutoCheckInType instead.", true)]
			public static void SetComfortableAutoCheckInType(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey buildingBlockKey, bool checkInType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A80 RID: 72320 RVA: 0x006856C4 File Offset: 0x006838C4
			public static void LockResidenceCharacter(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey buildingBlockKey, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, int>(9, 203, buildingBlockKey, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A81 RID: 72321 RVA: 0x006856F4 File Offset: 0x006838F4
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.SetNextTeaHorseCaravanEvent instead.", true)]
			public static void SetNextTeaHorseCaravanEvent(IAsyncMethodRequestHandler requestHandler, short eventTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A82 RID: 72322 RVA: 0x006856FC File Offset: 0x006838FC
			public static void GetLockedInComfortableHouseIds(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 205, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A83 RID: 72323 RVA: 0x0068572C File Offset: 0x0068392C
			public static void GetLockedInResidenceIds(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 206, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A84 RID: 72324 RVA: 0x0068575C File Offset: 0x0068395C
			public static void GetReversedBlockShopEvent(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 207, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A85 RID: 72325 RVA: 0x0068578C File Offset: 0x0068398C
			public static void PluckAllChickenFeathers(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 208, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A86 RID: 72326 RVA: 0x006857BC File Offset: 0x006839BC
			public static void IsAllChickensCanPluck(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 209, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A87 RID: 72327 RVA: 0x006857EC File Offset: 0x006839EC
			public static void GetCharacterChickenFeatures(IAsyncMethodRequestHandler requestHandler, int characterId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(9, 210, characterId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A88 RID: 72328 RVA: 0x0068581C File Offset: 0x00683A1C
			public static void GetChickensByPersonalityType(IAsyncMethodRequestHandler requestHandler, sbyte personalityType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(9, 211, personalityType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A89 RID: 72329 RVA: 0x0068584C File Offset: 0x00683A4C
			public static void GetCurrentFeatherValue(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 212, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A8A RID: 72330 RVA: 0x0068587C File Offset: 0x00683A7C
			public static void CanCultivateFeather(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 213, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A8B RID: 72331 RVA: 0x006858AC File Offset: 0x00683AAC
			public static void GetChickenPluckFeatherDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 214, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A8C RID: 72332 RVA: 0x006858DC File Offset: 0x00683ADC
			public static void IsFeatherSystemUnlocked(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 215, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A8D RID: 72333 RVA: 0x0068590A File Offset: 0x00683B0A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.UnlockFeatherSystem instead.", true)]
			public static void UnlockFeatherSystem(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A8E RID: 72334 RVA: 0x00685914 File Offset: 0x00683B14
			public static void PluckChickenFeather(IAsyncMethodRequestHandler requestHandler, int chickenId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(9, 217, chickenId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A8F RID: 72335 RVA: 0x00685944 File Offset: 0x00683B44
			public static void CanUseChickenFeather(IAsyncMethodRequestHandler requestHandler, int characterId, sbyte personalityType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, sbyte>(9, 218, characterId, personalityType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A90 RID: 72336 RVA: 0x00685974 File Offset: 0x00683B74
			public static void UseChickenFeather(IAsyncMethodRequestHandler requestHandler, int characterId, ItemKey itemKey, sbyte personalityType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ItemKey, sbyte>(9, 219, characterId, itemKey, personalityType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A91 RID: 72337 RVA: 0x006859A8 File Offset: 0x00683BA8
			public static void CanPluckFeatherInVillage(IAsyncMethodRequestHandler requestHandler, int chickenId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(9, 220, chickenId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A92 RID: 72338 RVA: 0x006859D8 File Offset: 0x00683BD8
			public static void CultivateFeather(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 221, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A93 RID: 72339 RVA: 0x00685A08 File Offset: 0x00683C08
			public static void GetCanPluckFeatherChickenIds(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 222, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A94 RID: 72340 RVA: 0x00685A38 File Offset: 0x00683C38
			public static void GetSamsaraPlatformBonusAttributes(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 223, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A95 RID: 72341 RVA: 0x00685A68 File Offset: 0x00683C68
			public static void GetSamsaraPlatformBonusAttributes(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(9, 223, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A96 RID: 72342 RVA: 0x00685A98 File Offset: 0x00683C98
			public static void GetSamsaraPlatformCharDisplayData(IAsyncMethodRequestHandler requestHandler, sbyte slot, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(9, 224, slot, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A97 RID: 72343 RVA: 0x00685AC7 File Offset: 0x00683CC7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.QuickAssignChicken instead.", true)]
			public static void QuickAssignChicken(IAsyncMethodRequestHandler requestHandler, short orgMemberTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011A98 RID: 72344 RVA: 0x00685AD0 File Offset: 0x00683CD0
			public static void GetCricketCollectionDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 226, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A99 RID: 72345 RVA: 0x00685B00 File Offset: 0x00683D00
			public static void GetBuildingMakeDisplayData(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, sbyte lifeSkillType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, sbyte>(9, 227, blockKey, lifeSkillType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A9A RID: 72346 RVA: 0x00685B30 File Offset: 0x00683D30
			public static void CheckRefineCondition(IAsyncMethodRequestHandler requestHandler, int charId, ItemKey[] toolKeys, ItemKey equipItemKey, ItemDisplayData[] materialItemData, BuildingBlockKey buildingBlockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ItemKey[], ItemKey, ItemDisplayData[], BuildingBlockKey>(9, 228, charId, toolKeys, equipItemKey, materialItemData, buildingBlockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A9B RID: 72347 RVA: 0x00685B68 File Offset: 0x00683D68
			public static void RefineItem(IAsyncMethodRequestHandler requestHandler, int charId, ItemDisplayData[] tools, ItemDisplayData target, ItemDisplayData[] materialItemArray, List<ItemSourceChange> changeList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ItemDisplayData[], ItemDisplayData, ItemDisplayData[], List<ItemSourceChange>>(9, 229, charId, tools, target, materialItemArray, changeList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A9C RID: 72348 RVA: 0x00685BA0 File Offset: 0x00683DA0
			public static void GetCraftManDisplayDataForCharacter(IAsyncMethodRequestHandler requestHandler, int artisanId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(9, 230, artisanId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A9D RID: 72349 RVA: 0x00685BD0 File Offset: 0x00683DD0
			public static void GetCraftManDisplayDataForBuilding(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(9, 231, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A9E RID: 72350 RVA: 0x00685C00 File Offset: 0x00683E00
			public static void GetTeaHorseCaravanEvent(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 232, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011A9F RID: 72351 RVA: 0x00685C2E File Offset: 0x00683E2E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.TriggerCultivateFeatherEvent instead.", true)]
			public static void TriggerCultivateFeatherEvent(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011AA0 RID: 72352 RVA: 0x00685C38 File Offset: 0x00683E38
			public static void GetTeaHorseCaravanData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 234, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011AA1 RID: 72353 RVA: 0x00685C66 File Offset: 0x00683E66
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.QuickDiscardExchangeItem instead.", true)]
			public static void QuickDiscardExchangeItem(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011AA2 RID: 72354 RVA: 0x00685C70 File Offset: 0x00683E70
			public static void GetTaiwuVillageBuildingDataForVillagerRole(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 236, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011AA3 RID: 72355 RVA: 0x00685CA0 File Offset: 0x00683EA0
			public static void IsAnyChickensCanPluck(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 237, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011AA4 RID: 72356 RVA: 0x00685CD0 File Offset: 0x00683ED0
			public static void FeedChicken(IAsyncMethodRequestHandler requestHandler, int id, ItemKey itemKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ItemKey>(9, 238, id, itemKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011AA5 RID: 72357 RVA: 0x00685D00 File Offset: 0x00683F00
			public static void GetBuildingBlockEffect(IAsyncMethodRequestHandler requestHandler, short settlementId, EBuildingScaleEffect effectType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, EBuildingScaleEffect>(9, 239, settlementId, effectType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011AA6 RID: 72358 RVA: 0x00685D30 File Offset: 0x00683F30
			public static void GetBuildingBlockEffect(IAsyncMethodRequestHandler requestHandler, short settlementId, EBuildingScaleEffect effectType, int subType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, EBuildingScaleEffect, int>(9, 239, settlementId, effectType, subType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011AA7 RID: 72359 RVA: 0x00685D62 File Offset: 0x00683F62
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use BuildingDomainMethod.Call.ClearNewlyCreatedBuildingIndex instead.", true)]
			public static void ClearNewlyCreatedBuildingIndex(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011AA8 RID: 72360 RVA: 0x00685D6C File Offset: 0x00683F6C
			public static void GetNewlyCreatedBuildingIndex(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(9, 241, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}
		}
	}
}
