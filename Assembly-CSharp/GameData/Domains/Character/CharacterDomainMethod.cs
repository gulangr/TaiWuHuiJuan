using System;
using System.Collections.Generic;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.SortFilter;
using GameData.Domains.Item;
using GameData.Domains.Map;
using GameData.GameDataBridge;
using GameData.Utilities;

namespace GameData.Domains.Character
{
	// Token: 0x02000FCD RID: 4045
	public static class CharacterDomainMethod
	{
		// Token: 0x02002615 RID: 9749
		public static class Call
		{
			// Token: 0x060116AE RID: 71342 RVA: 0x0067F7BC File Offset: 0x0067D9BC
			public static void CreateProtagonist(int listenerId, ProtagonistCreationInfo info)
			{
				GameDataBridge.AddMethodCall<ProtagonistCreationInfo>(listenerId, 4, 0, info);
			}

			// Token: 0x060116AF RID: 71343 RVA: 0x0067F7C9 File Offset: 0x0067D9C9
			public static void GetRelatedCharactersForRelations(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 1, charId);
			}

			// Token: 0x060116B0 RID: 71344 RVA: 0x0067F7D6 File Offset: 0x0067D9D6
			public static void TryCreateRelation(int charId, int relatedCharId)
			{
				GameDataBridge.AddMethodCall<int, int>(-1, 4, 2, charId, relatedCharId);
			}

			// Token: 0x060116B1 RID: 71345 RVA: 0x0067F7E4 File Offset: 0x0067D9E4
			public static void GetGenealogy(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 3, charId);
			}

			// Token: 0x060116B2 RID: 71346 RVA: 0x0067F7F1 File Offset: 0x0067D9F1
			public static void GenerateRandomHanName(int listenerId, int customSurnameId, short surnameId, sbyte gender)
			{
				GameDataBridge.AddMethodCall<int, short, sbyte>(listenerId, 4, 4, customSurnameId, surnameId, gender);
			}

			// Token: 0x060116B3 RID: 71347 RVA: 0x0067F800 File Offset: 0x0067DA00
			public static void GenerateRandomZangName(int listenerId, sbyte gender)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 4, 5, gender);
			}

			// Token: 0x060116B4 RID: 71348 RVA: 0x0067F80D File Offset: 0x0067DA0D
			public static void GenerateRandomChildName(int listenerId, sbyte gender, FullName parentName)
			{
				GameDataBridge.AddMethodCall<sbyte, FullName>(listenerId, 4, 6, gender, parentName);
			}

			// Token: 0x060116B5 RID: 71349 RVA: 0x0067F81B File Offset: 0x0067DA1B
			public static void GetNameRelatedDataList(int listenerId, List<int> charIds)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 4, 7, charIds);
			}

			// Token: 0x060116B6 RID: 71350 RVA: 0x0067F828 File Offset: 0x0067DA28
			public static void GetNameRelatedData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 8, charId);
			}

			// Token: 0x060116B7 RID: 71351 RVA: 0x0067F835 File Offset: 0x0067DA35
			public static void GetNameAndLifeRelatedDataList(int listenerId, List<int> charIds)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 4, 9, charIds);
			}

			// Token: 0x060116B8 RID: 71352 RVA: 0x0067F843 File Offset: 0x0067DA43
			public static void GetNameAndLifeRelatedData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 10, charId);
			}

			// Token: 0x060116B9 RID: 71353 RVA: 0x0067F851 File Offset: 0x0067DA51
			public static void GetFavorability(int listenerId, int charId, int relatedCharId)
			{
				GameDataBridge.AddMethodCall<int, int>(listenerId, 4, 11, charId, relatedCharId);
			}

			// Token: 0x060116BA RID: 71354 RVA: 0x0067F860 File Offset: 0x0067DA60
			public static void GmCmd_GetAllGroupMembers(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 4, 12);
			}

			// Token: 0x060116BB RID: 71355 RVA: 0x0067F86D File Offset: 0x0067DA6D
			public static void GmCmd_GenerateRandomRefinedItemToCharacter(int charId, sbyte itemType, int times)
			{
				GameDataBridge.AddMethodCall<int, sbyte, int>(-1, 4, 13, charId, itemType, times);
			}

			// Token: 0x060116BC RID: 71356 RVA: 0x0067F87D File Offset: 0x0067DA7D
			public static void GmCmd_ChangeInjury(int charId, bool isInnerInjury, sbyte bodyPartType, sbyte delta)
			{
				GameDataBridge.AddMethodCall<int, bool, sbyte, sbyte>(-1, 4, 14, charId, isInnerInjury, bodyPartType, delta);
			}

			// Token: 0x060116BD RID: 71357 RVA: 0x0067F88E File Offset: 0x0067DA8E
			public static void GmCmd_ChangePoisonByType(int charId, sbyte poisonType, int changeValue)
			{
				GameDataBridge.AddMethodCall<int, sbyte, int>(-1, 4, 15, charId, poisonType, changeValue);
			}

			// Token: 0x060116BE RID: 71358 RVA: 0x0067F89E File Offset: 0x0067DA9E
			public static void GmCmd_ForgetCombatSkill(int charId, short skillTemplateId)
			{
				GameDataBridge.AddMethodCall<int, short>(-1, 4, 16, charId, skillTemplateId);
			}

			// Token: 0x060116BF RID: 71359 RVA: 0x0067F8AD File Offset: 0x0067DAAD
			public static void GmCmd_RevokeCombatSkill(int charId, List<short> skillTemplateIdList)
			{
				GameDataBridge.AddMethodCall<int, List<short>>(-1, 4, 17, charId, skillTemplateIdList);
			}

			// Token: 0x060116C0 RID: 71360 RVA: 0x0067F8BC File Offset: 0x0067DABC
			public static void GmCmd_SetLearnedLifeSkills(int charId, List<LifeSkillItem> learnedLifeSkills)
			{
				GameDataBridge.AddMethodCall<int, List<LifeSkillItem>>(-1, 4, 18, charId, learnedLifeSkills);
			}

			// Token: 0x060116C1 RID: 71361 RVA: 0x0067F8CB File Offset: 0x0067DACB
			public static void GmCmd_GetCricket(short colorId, short partId)
			{
				GameDataBridge.AddMethodCall<short, short>(-1, 4, 19, colorId, partId);
			}

			// Token: 0x060116C2 RID: 71362 RVA: 0x0067F8DA File Offset: 0x0067DADA
			public static void GmCmd_GetCricket(short colorId, short partId, int grade)
			{
				GameDataBridge.AddMethodCall<short, short, int>(-1, 4, 19, colorId, partId, grade);
			}

			// Token: 0x060116C3 RID: 71363 RVA: 0x0067F8EA File Offset: 0x0067DAEA
			public static void GmCmd_GetCricket(short colorId, short partId, int grade, short winsCount)
			{
				GameDataBridge.AddMethodCall<short, short, int, short>(-1, 4, 19, colorId, partId, grade, winsCount);
			}

			// Token: 0x060116C4 RID: 71364 RVA: 0x0067F8FB File Offset: 0x0067DAFB
			public static void GmCmd_GetCricket(short colorId, short partId, int grade, short winsCount, short lossesCount)
			{
				GameDataBridge.AddMethodCall<short, short, int, short, short>(-1, 4, 19, colorId, partId, grade, winsCount, lossesCount);
			}

			// Token: 0x060116C5 RID: 71365 RVA: 0x0067F90E File Offset: 0x0067DB0E
			public static void GmCmd_AddRelation(int charId, int relatedCharId, ushort addingType)
			{
				GameDataBridge.AddMethodCall<int, int, ushort>(-1, 4, 20, charId, relatedCharId, addingType);
			}

			// Token: 0x060116C6 RID: 71366 RVA: 0x0067F91E File Offset: 0x0067DB1E
			public static void GetGroupSet(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 21, charId);
			}

			// Token: 0x060116C7 RID: 71367 RVA: 0x0067F92C File Offset: 0x0067DB2C
			public static void TransferResourcesWithDebt(int srcCharId, int destCharId, ResourceInts resources, bool checkFavorability)
			{
				GameDataBridge.AddMethodCall<int, int, ResourceInts, bool>(-1, 4, 22, srcCharId, destCharId, resources, checkFavorability);
			}

			// Token: 0x060116C8 RID: 71368 RVA: 0x0067F93D File Offset: 0x0067DB3D
			public static void TransferInventoryItemWithDebt(int srcCharId, int destCharId, ItemKey itemKey, int amount, bool checkFavorability)
			{
				GameDataBridge.AddMethodCall<int, int, ItemKey, int, bool>(-1, 4, 23, srcCharId, destCharId, itemKey, amount, checkFavorability);
			}

			// Token: 0x060116C9 RID: 71369 RVA: 0x0067F950 File Offset: 0x0067DB50
			public static void ChangeEquipment(int charId, sbyte srcSlot, sbyte destSlot, ItemKey srcItemKey)
			{
				GameDataBridge.AddMethodCall<int, sbyte, sbyte, ItemKey>(-1, 4, 24, charId, srcSlot, destSlot, srcItemKey);
			}

			// Token: 0x060116CA RID: 71370 RVA: 0x0067F961 File Offset: 0x0067DB61
			public static void CreateInventoryItem(int charId, sbyte itemType, short templateId, int amount)
			{
				GameDataBridge.AddMethodCall<int, sbyte, short, int>(-1, 4, 25, charId, itemType, templateId, amount);
			}

			// Token: 0x060116CB RID: 71371 RVA: 0x0067F972 File Offset: 0x0067DB72
			public static void GetInventoryItems(int listenerId, int charId, short itemSubType)
			{
				GameDataBridge.AddMethodCall<int, short>(listenerId, 4, 26, charId, itemSubType);
			}

			// Token: 0x060116CC RID: 71372 RVA: 0x0067F981 File Offset: 0x0067DB81
			public static void GetAllInventoryItems(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 27, charId);
			}

			// Token: 0x060116CD RID: 71373 RVA: 0x0067F98F File Offset: 0x0067DB8F
			public static void GetInventoryItemAmount(int listenerId, int charId, sbyte itemType, short templateId)
			{
				GameDataBridge.AddMethodCall<int, sbyte, short>(listenerId, 4, 28, charId, itemType, templateId);
			}

			// Token: 0x060116CE RID: 71374 RVA: 0x0067F99F File Offset: 0x0067DB9F
			public static void GetAllEquipmentItems(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 29, charId);
			}

			// Token: 0x060116CF RID: 71375 RVA: 0x0067F9AD File Offset: 0x0067DBAD
			public static void GetInventoryItemDisplayData(int listenerId, int charId, ItemKey itemKey)
			{
				GameDataBridge.AddMethodCall<int, ItemKey>(listenerId, 4, 30, charId, itemKey);
			}

			// Token: 0x060116D0 RID: 71376 RVA: 0x0067F9BC File Offset: 0x0067DBBC
			public static void InventoryContainsItem(int listenerId, int charId, ItemKey itemKey)
			{
				GameDataBridge.AddMethodCall<int, ItemKey>(listenerId, 4, 31, charId, itemKey);
			}

			// Token: 0x060116D1 RID: 71377 RVA: 0x0067F9CB File Offset: 0x0067DBCB
			public static void AddEatingItem(int charId, ItemKey itemKey)
			{
				GameDataBridge.AddMethodCall<int, ItemKey>(-1, 4, 32, charId, itemKey);
			}

			// Token: 0x060116D2 RID: 71378 RVA: 0x0067F9DA File Offset: 0x0067DBDA
			public static void AddEatingItem(int charId, ItemKey itemKey, List<sbyte> targetBodyParts)
			{
				GameDataBridge.AddMethodCall<int, ItemKey, List<sbyte>>(-1, 4, 32, charId, itemKey, targetBodyParts);
			}

			// Token: 0x060116D3 RID: 71379 RVA: 0x0067F9EA File Offset: 0x0067DBEA
			public static void GetCurrMaxEatingSlotsCount(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 33, charId);
			}

			// Token: 0x060116D4 RID: 71380 RVA: 0x0067F9F8 File Offset: 0x0067DBF8
			public static void MerchantHasNewGoods(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 34, charId);
			}

			// Token: 0x060116D5 RID: 71381 RVA: 0x0067FA06 File Offset: 0x0067DC06
			public static void AddKidnappedCharacter(int charId, int kidnappedCharId, ItemKey ropeItemKey)
			{
				GameDataBridge.AddMethodCall<int, int, ItemKey>(-1, 4, 35, charId, kidnappedCharId, ropeItemKey);
			}

			// Token: 0x060116D6 RID: 71382 RVA: 0x0067FA16 File Offset: 0x0067DC16
			public static void TransferKidnappedCharacters(int targetKidnapperId, int sourceKidnapperId, KidnappedCharacterList kidnappedCharsToTransfer)
			{
				GameDataBridge.AddMethodCall<int, int, KidnappedCharacterList>(-1, 4, 36, targetKidnapperId, sourceKidnapperId, kidnappedCharsToTransfer);
			}

			// Token: 0x060116D7 RID: 71383 RVA: 0x0067FA26 File Offset: 0x0067DC26
			public static void ChangeKidnappedCharacterRope(int kidnapperId, int kidnappedCharId, ItemKey newRopeKey)
			{
				GameDataBridge.AddMethodCall<int, int, ItemKey>(-1, 4, 37, kidnapperId, kidnappedCharId, newRopeKey);
			}

			// Token: 0x060116D8 RID: 71384 RVA: 0x0067FA36 File Offset: 0x0067DC36
			public static void GetKidnapMaxSlotCount(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 38, charId);
			}

			// Token: 0x060116D9 RID: 71385 RVA: 0x0067FA44 File Offset: 0x0067DC44
			public static void TransferKidnappedCharacter(int targetKidnapperId, int sourceKidnapperId, KidnappedCharacter kidnappedCharData)
			{
				GameDataBridge.AddMethodCall<int, int, KidnappedCharacter>(-1, 4, 39, targetKidnapperId, sourceKidnapperId, kidnappedCharData);
			}

			// Token: 0x060116DA RID: 71386 RVA: 0x0067FA54 File Offset: 0x0067DC54
			public static void RemoveKidnappedCharacter(int kidnappedCharId, int kidnapperId, bool isEscaped)
			{
				GameDataBridge.AddMethodCall<int, int, bool>(-1, 4, 40, kidnappedCharId, kidnapperId, isEscaped);
			}

			// Token: 0x060116DB RID: 71387 RVA: 0x0067FA64 File Offset: 0x0067DC64
			public static void GetDisplayingAge(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 41, charId);
			}

			// Token: 0x060116DC RID: 71388 RVA: 0x0067FA72 File Offset: 0x0067DC72
			public static void GetMainAttributesRecoveries(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 42, charId);
			}

			// Token: 0x060116DD RID: 71389 RVA: 0x0067FA80 File Offset: 0x0067DC80
			public static void GetInscriptionStatus(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 43, charId);
			}

			// Token: 0x060116DE RID: 71390 RVA: 0x0067FA8E File Offset: 0x0067DC8E
			public static void GetCharacterBirthDate(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 44, charId);
			}

			// Token: 0x060116DF RID: 71391 RVA: 0x0067FA9C File Offset: 0x0067DC9C
			public static void GetMaxWorthCanBeLentToTaiwu(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 45, charId);
			}

			// Token: 0x060116E0 RID: 71392 RVA: 0x0067FAAA File Offset: 0x0067DCAA
			public static void CheckFavorabilityBeforeTransferring(int listenerId, int charId, ResourceInts resources)
			{
				GameDataBridge.AddMethodCall<int, ResourceInts>(listenerId, 4, 46, charId, resources);
			}

			// Token: 0x060116E1 RID: 71393 RVA: 0x0067FAB9 File Offset: 0x0067DCB9
			public static void GetClothingDisplayId(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 47, charId);
			}

			// Token: 0x060116E2 RID: 71394 RVA: 0x0067FAC7 File Offset: 0x0067DCC7
			public static void GetCharacterDisplayDataList(int listenerId, List<int> charIdList)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 4, 48, charIdList);
			}

			// Token: 0x060116E3 RID: 71395 RVA: 0x0067FAD5 File Offset: 0x0067DCD5
			public static void GetCharacterLifeSkillAttainmentList(int listenerId, List<int> charIdList, sbyte lifeSkillType)
			{
				GameDataBridge.AddMethodCall<List<int>, sbyte>(listenerId, 4, 49, charIdList, lifeSkillType);
			}

			// Token: 0x060116E4 RID: 71396 RVA: 0x0067FAE4 File Offset: 0x0067DCE4
			public static void GetTaiwuRelatedGraveDisplayDataList(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 4, 50);
			}

			// Token: 0x060116E5 RID: 71397 RVA: 0x0067FAF1 File Offset: 0x0067DCF1
			public static void GetGraveDisplayDataList(int listenerId, List<int> charIdList)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 4, 51, charIdList);
			}

			// Token: 0x060116E6 RID: 71398 RVA: 0x0067FAFF File Offset: 0x0067DCFF
			public static void GetCharacterDisplayDataListForRelations(int listenerId, List<int> charIds)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 4, 52, charIds);
			}

			// Token: 0x060116E7 RID: 71399 RVA: 0x0067FB0D File Offset: 0x0067DD0D
			public static void GetSomeoneKidnapCharacters(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 53, charId);
			}

			// Token: 0x060116E8 RID: 71400 RVA: 0x0067FB1B File Offset: 0x0067DD1B
			public static void GetCharacterAttributeDisplayData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 54, charId);
			}

			// Token: 0x060116E9 RID: 71401 RVA: 0x0067FB29 File Offset: 0x0067DD29
			public static void GetCharacterSamsaraData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 55, charId);
			}

			// Token: 0x060116EA RID: 71402 RVA: 0x0067FB37 File Offset: 0x0067DD37
			public static void GetEquipmentCompareData(int listenerId, int charId, sbyte srcSlot, sbyte destSlot, ItemKey srcItemKey)
			{
				GameDataBridge.AddMethodCall<int, sbyte, sbyte, ItemKey>(listenerId, 4, 56, charId, srcSlot, destSlot, srcItemKey);
			}

			// Token: 0x060116EB RID: 71403 RVA: 0x0067FB49 File Offset: 0x0067DD49
			public static void GetGroupCharDisplayDataList(int listenerId, List<int> charIdList)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 4, 57, charIdList);
			}

			// Token: 0x060116EC RID: 71404 RVA: 0x0067FB57 File Offset: 0x0067DD57
			public static void GetDefeatMarkCountList(int listenerId, List<int> charIdList)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 4, 58, charIdList);
			}

			// Token: 0x060116ED RID: 71405 RVA: 0x0067FB65 File Offset: 0x0067DD65
			public static void GetFeatureMedalValue(int listenerId, int charId, sbyte medalType)
			{
				GameDataBridge.AddMethodCall<int, sbyte>(listenerId, 4, 59, charId, medalType);
			}

			// Token: 0x060116EE RID: 71406 RVA: 0x0067FB74 File Offset: 0x0067DD74
			public static void GetFeatureMedalValueList(int listenerId, List<int> charIdList, sbyte medalType)
			{
				GameDataBridge.AddMethodCall<List<int>, sbyte>(listenerId, 4, 60, charIdList, medalType);
			}

			// Token: 0x060116EF RID: 71407 RVA: 0x0067FB83 File Offset: 0x0067DD83
			public static void GetFixedCharacterIdByTemplateId(int listenerId, short templateId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 4, 61, templateId);
			}

			// Token: 0x060116F0 RID: 71408 RVA: 0x0067FB94 File Offset: 0x0067DD94
			public static void GmCmd_SimulateNpcCombat(int charIdA, int charIdB, sbyte combatType, int killBaseChance, int kidnapBaseChance, int releaseBaseChance)
			{
				GameDataBridge.AddMethodCall<int, int, sbyte, int, int, int>(-1, 4, 62, charIdA, charIdB, combatType, killBaseChance, kidnapBaseChance, releaseBaseChance);
			}

			// Token: 0x060116F1 RID: 71409 RVA: 0x0067FBB4 File Offset: 0x0067DDB4
			public static void GmCmd_GetAllCharacterName(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 4, 63);
			}

			// Token: 0x060116F2 RID: 71410 RVA: 0x0067FBC1 File Offset: 0x0067DDC1
			public static void GmCmd_ChangeFavorability(int selfCharId, int relatedCharId, short delta)
			{
				GameDataBridge.AddMethodCall<int, int, short>(-1, 4, 64, selfCharId, relatedCharId, delta);
			}

			// Token: 0x060116F3 RID: 71411 RVA: 0x0067FBD1 File Offset: 0x0067DDD1
			public static void GmCmd_ClearFameActionRecords(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 4, 65, charId);
			}

			// Token: 0x060116F4 RID: 71412 RVA: 0x0067FBDF File Offset: 0x0067DDDF
			public static void GmCmd_RecordFameAction(int charId, short fameActionId)
			{
				GameDataBridge.AddMethodCall<int, short>(-1, 4, 66, charId, fameActionId);
			}

			// Token: 0x060116F5 RID: 71413 RVA: 0x0067FBEE File Offset: 0x0067DDEE
			public static void GmCmd_RecordFameAction(int charId, short fameActionId, int targetCharId)
			{
				GameDataBridge.AddMethodCall<int, short, int>(-1, 4, 66, charId, fameActionId, targetCharId);
			}

			// Token: 0x060116F6 RID: 71414 RVA: 0x0067FBFE File Offset: 0x0067DDFE
			public static void GmCmd_RecordFameAction(int charId, short fameActionId, int targetCharId, short fameMultiplier)
			{
				GameDataBridge.AddMethodCall<int, short, int, short>(-1, 4, 66, charId, fameActionId, targetCharId, fameMultiplier);
			}

			// Token: 0x060116F7 RID: 71415 RVA: 0x0067FC0F File Offset: 0x0067DE0F
			public static void GmCmd_CreateRandomIntelligentCharacters(int charCount, sbyte orgTemplateId, bool createHere)
			{
				GameDataBridge.AddMethodCall<int, sbyte, bool>(-1, 4, 67, charCount, orgTemplateId, createHere);
			}

			// Token: 0x060116F8 RID: 71416 RVA: 0x0067FC1F File Offset: 0x0067DE1F
			public static void GmCmd_ForceChangeOrganization(int charId, sbyte orgTemplateId)
			{
				GameDataBridge.AddMethodCall<int, sbyte>(-1, 4, 68, charId, orgTemplateId);
			}

			// Token: 0x060116F9 RID: 71417 RVA: 0x0067FC2E File Offset: 0x0067DE2E
			public static void GmCmd_ForceChangeGrade(int listenerId, int charId, sbyte grade, bool principal)
			{
				GameDataBridge.AddMethodCall<int, sbyte, bool>(listenerId, 4, 69, charId, grade, principal);
			}

			// Token: 0x060116FA RID: 71418 RVA: 0x0067FC3E File Offset: 0x0067DE3E
			public static void GmCmd_Die(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 4, 70, charId);
			}

			// Token: 0x060116FB RID: 71419 RVA: 0x0067FC4C File Offset: 0x0067DE4C
			public static void GmCmd_GetAliveCharByPreexistenceChar(int listenerId, int preexistenceCharId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 71, preexistenceCharId);
			}

			// Token: 0x060116FC RID: 71420 RVA: 0x0067FC5A File Offset: 0x0067DE5A
			public static void GmCmd_LogCharacterSamsaraInfo()
			{
				GameDataBridge.AddMethodCall(-1, 4, 72);
			}

			// Token: 0x060116FD RID: 71421 RVA: 0x0067FC67 File Offset: 0x0067DE67
			public static void GmCmd_EditExtraNeiliAllocation(int characterId, NeiliAllocation allocation)
			{
				GameDataBridge.AddMethodCall<int, NeiliAllocation>(-1, 4, 73, characterId, allocation);
			}

			// Token: 0x060116FE RID: 71422 RVA: 0x0067FC76 File Offset: 0x0067DE76
			public static void GmCmd_MakeCharacterKidnapped(int characterId, int targetCharacterId)
			{
				GameDataBridge.AddMethodCall<int, int>(-1, 4, 74, characterId, targetCharacterId);
			}

			// Token: 0x060116FF RID: 71423 RVA: 0x0067FC85 File Offset: 0x0067DE85
			public static void GmCmd_MoveIntelligentCharacter(int listenerId, int charId, Location destLocation)
			{
				GameDataBridge.AddMethodCall<int, Location>(listenerId, 4, 75, charId, destLocation);
			}

			// Token: 0x06011700 RID: 71424 RVA: 0x0067FC94 File Offset: 0x0067DE94
			public static void GmCmd_RandomizeRelationShipsInSettlement(sbyte orgTemplateId)
			{
				GameDataBridge.AddMethodCall<sbyte>(-1, 4, 76, orgTemplateId);
			}

			// Token: 0x06011701 RID: 71425 RVA: 0x0067FCA2 File Offset: 0x0067DEA2
			public static void GmCmd_MakeCharacterHaveSex(int selfCharId, int targetCharId, bool isRaped, int pregnantRemainTime)
			{
				GameDataBridge.AddMethodCall<int, int, bool, int>(-1, 4, 77, selfCharId, targetCharId, isRaped, pregnantRemainTime);
			}

			// Token: 0x06011702 RID: 71426 RVA: 0x0067FCB3 File Offset: 0x0067DEB3
			public static void GmCmd_GetCharacterPregnancyLockEndDates(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 78, charId);
			}

			// Token: 0x06011703 RID: 71427 RVA: 0x0067FCC1 File Offset: 0x0067DEC1
			public static void GmCmd_GetCharacterActualBloodParents(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 79, charId);
			}

			// Token: 0x06011704 RID: 71428 RVA: 0x0067FCCF File Offset: 0x0067DECF
			public static void CharacterShaveAvatar(int listenerId, int cutterCharId, int shaveCharId, AvatarData shaveResult)
			{
				GameDataBridge.AddMethodCall<int, int, AvatarData>(listenerId, 4, 80, cutterCharId, shaveCharId, shaveResult);
			}

			// Token: 0x06011705 RID: 71429 RVA: 0x0067FCDF File Offset: 0x0067DEDF
			public static void AllocateNeili(int charId, byte neiliAllocationType)
			{
				GameDataBridge.AddMethodCall<int, byte>(-1, 4, 81, charId, neiliAllocationType);
			}

			// Token: 0x06011706 RID: 71430 RVA: 0x0067FCEE File Offset: 0x0067DEEE
			public static void DeallocateNeili(int charId, byte neiliAllocationType)
			{
				GameDataBridge.AddMethodCall<int, byte>(-1, 4, 82, charId, neiliAllocationType);
			}

			// Token: 0x06011707 RID: 71431 RVA: 0x0067FCFD File Offset: 0x0067DEFD
			public static void GetChangeOfQiDisorder(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 83, charId);
			}

			// Token: 0x06011708 RID: 71432 RVA: 0x0067FD0B File Offset: 0x0067DF0B
			public static void GetUsableCombatResources(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 84, charId);
			}

			// Token: 0x06011709 RID: 71433 RVA: 0x0067FD19 File Offset: 0x0067DF19
			public static void GetCombatSkillSlotCounts(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 85, charId);
			}

			// Token: 0x0601170A RID: 71434 RVA: 0x0067FD27 File Offset: 0x0067DF27
			public static void SetCombatSkillSlot(int charId, sbyte equipType, int index, short skillTemplateId)
			{
				GameDataBridge.AddMethodCall<int, sbyte, int, short>(-1, 4, 86, charId, equipType, index, skillTemplateId);
			}

			// Token: 0x0601170B RID: 71435 RVA: 0x0067FD38 File Offset: 0x0067DF38
			public static void GetCombatSkillAttainment(int listenerId, int charId, sbyte type)
			{
				GameDataBridge.AddMethodCall<int, sbyte>(listenerId, 4, 87, charId, type);
			}

			// Token: 0x0601170C RID: 71436 RVA: 0x0067FD47 File Offset: 0x0067DF47
			public static void GetAllCombatSkillAttainment(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 88, charId);
			}

			// Token: 0x0601170D RID: 71437 RVA: 0x0067FD55 File Offset: 0x0067DF55
			public static void GetLifeSkillAttainment(int listenerId, int charId, sbyte type)
			{
				GameDataBridge.AddMethodCall<int, sbyte>(listenerId, 4, 89, charId, type);
			}

			// Token: 0x0601170E RID: 71438 RVA: 0x0067FD64 File Offset: 0x0067DF64
			public static void GetAllLifeSkillAttainment(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 90, charId);
			}

			// Token: 0x0601170F RID: 71439 RVA: 0x0067FD72 File Offset: 0x0067DF72
			public static void LearnCombatSkill(int charId, short skillTemplateId)
			{
				GameDataBridge.AddMethodCall<int, short>(-1, 4, 91, charId, skillTemplateId);
			}

			// Token: 0x06011710 RID: 71440 RVA: 0x0067FD81 File Offset: 0x0067DF81
			public static void LearnCombatSkill(int charId, short skillTemplateId, ushort readingState)
			{
				GameDataBridge.AddMethodCall<int, short, ushort>(-1, 4, 91, charId, skillTemplateId, readingState);
			}

			// Token: 0x06011711 RID: 71441 RVA: 0x0067FD91 File Offset: 0x0067DF91
			public static void LearnLifeSkill(int charId, short skillTemplateId)
			{
				GameDataBridge.AddMethodCall<int, short>(-1, 4, 92, charId, skillTemplateId);
			}

			// Token: 0x06011712 RID: 71442 RVA: 0x0067FDA0 File Offset: 0x0067DFA0
			public static void LearnLifeSkill(int charId, short skillTemplateId, byte readingState)
			{
				GameDataBridge.AddMethodCall<int, short, byte>(-1, 4, 92, charId, skillTemplateId, readingState);
			}

			// Token: 0x06011713 RID: 71443 RVA: 0x0067FDB0 File Offset: 0x0067DFB0
			public static void TryGetDeadCharacter(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 93, charId);
			}

			// Token: 0x06011714 RID: 71444 RVA: 0x0067FDBE File Offset: 0x0067DFBE
			public static void GetTitles(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 94, charId);
			}

			// Token: 0x06011715 RID: 71445 RVA: 0x0067FDCC File Offset: 0x0067DFCC
			public static void GetHighestGradeCombatSkillById(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 95, charId);
			}

			// Token: 0x06011716 RID: 71446 RVA: 0x0067FDDA File Offset: 0x0067DFDA
			public static void GetLeftMaxHealth(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 96, charId);
			}

			// Token: 0x06011717 RID: 71447 RVA: 0x0067FDE8 File Offset: 0x0067DFE8
			public static void GetHealthRecovery(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 97, charId);
			}

			// Token: 0x06011718 RID: 71448 RVA: 0x0067FDF6 File Offset: 0x0067DFF6
			public static void GetAvatarRelatedDataList(int listenerId, List<int> charIdList)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 4, 98, charIdList);
			}

			// Token: 0x06011719 RID: 71449 RVA: 0x0067FE04 File Offset: 0x0067E004
			public static void GetAvatarData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 99, charId);
			}

			// Token: 0x0601171A RID: 71450 RVA: 0x0067FE12 File Offset: 0x0067E012
			public static void TransferInventoryItemListWithDebt(int srcCharId, int destCharId, List<ItemKey> keyList, bool checkFavorability)
			{
				GameDataBridge.AddMethodCall<int, int, List<ItemKey>, bool>(-1, 4, 100, srcCharId, destCharId, keyList, checkFavorability);
			}

			// Token: 0x0601171B RID: 71451 RVA: 0x0067FE23 File Offset: 0x0067E023
			public static void GetFameType(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 101, charId);
			}

			// Token: 0x0601171C RID: 71452 RVA: 0x0067FE31 File Offset: 0x0067E031
			public static void GetAvatarRelatedData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 102, charId);
			}

			// Token: 0x0601171D RID: 71453 RVA: 0x0067FE3F File Offset: 0x0067E03F
			public static void GetCharacterListWisdomCount(int listenerId, List<int> charIdList)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 4, 103, charIdList);
			}

			// Token: 0x0601171E RID: 71454 RVA: 0x0067FE4D File Offset: 0x0067E04D
			public static void SortCharacterListByMaxCombatSkill(int listenerId, List<int> managerList)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 4, 104, managerList);
			}

			// Token: 0x0601171F RID: 71455 RVA: 0x0067FE5B File Offset: 0x0067E05B
			public static void GetCharacterMaxCombatSkillAttainment(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 105, charId);
			}

			// Token: 0x06011720 RID: 71456 RVA: 0x0067FE69 File Offset: 0x0067E069
			public static void GetItemPowerInfo(int listenerId, int charId, ItemKey itemKey)
			{
				GameDataBridge.AddMethodCall<int, ItemKey>(listenerId, 4, 106, charId, itemKey);
			}

			// Token: 0x06011721 RID: 71457 RVA: 0x0067FE78 File Offset: 0x0067E078
			public static void CalcMaxFavorabilityToTaiwuById(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 107, charId);
			}

			// Token: 0x06011722 RID: 71458 RVA: 0x0067FE86 File Offset: 0x0067E086
			public static void CheckDebtChange(int listenerId, int charId, ResourceInts resources)
			{
				GameDataBridge.AddMethodCall<int, ResourceInts>(listenerId, 4, 108, charId, resources);
			}

			// Token: 0x06011723 RID: 71459 RVA: 0x0067FE95 File Offset: 0x0067E095
			public static void GetCharacterWisdomCountById(int listenerId, int characterId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 109, characterId);
			}

			// Token: 0x06011724 RID: 71460 RVA: 0x0067FEA3 File Offset: 0x0067E0A3
			public static void TransferInventoryItemFromAToB(int charA, int charB, ItemKey itemKey, int amount)
			{
				GameDataBridge.AddMethodCall<int, int, ItemKey, int>(-1, 4, 110, charA, charB, itemKey, amount);
			}

			// Token: 0x06011725 RID: 71461 RVA: 0x0067FEB4 File Offset: 0x0067E0B4
			public static void GmCmd_SetCurReadingEvent(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 4, 111, charId);
			}

			// Token: 0x06011726 RID: 71462 RVA: 0x0067FEC2 File Offset: 0x0067E0C2
			public static void GmCmd_AddFeature(int charId, short templateId)
			{
				GameDataBridge.AddMethodCall<int, short>(-1, 4, 112, charId, templateId);
			}

			// Token: 0x06011727 RID: 71463 RVA: 0x0067FED1 File Offset: 0x0067E0D1
			public static void GmCmd_SetFeatures(int charId, List<short> features)
			{
				GameDataBridge.AddMethodCall<int, List<short>>(-1, 4, 113, charId, features);
			}

			// Token: 0x06011728 RID: 71464 RVA: 0x0067FEE0 File Offset: 0x0067E0E0
			public static void GmCmd_RemoveFeature(int charId, short templateId)
			{
				GameDataBridge.AddMethodCall<int, short>(-1, 4, 114, charId, templateId);
			}

			// Token: 0x06011729 RID: 71465 RVA: 0x0067FEEF File Offset: 0x0067E0EF
			public static void GmCmd_RemoveRelation(int charId, int relatedCharId, ushort removeType)
			{
				GameDataBridge.AddMethodCall<int, int, ushort>(-1, 4, 115, charId, relatedCharId, removeType);
			}

			// Token: 0x0601172A RID: 71466 RVA: 0x0067FEFF File Offset: 0x0067E0FF
			public static void IsReclusive(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 116, charId);
			}

			// Token: 0x0601172B RID: 71467 RVA: 0x0067FF0D File Offset: 0x0067E10D
			public static void GetInventoryEquipment(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 117, charId);
			}

			// Token: 0x0601172C RID: 71468 RVA: 0x0067FF1B File Offset: 0x0067E11B
			public static void GetFilteredCharacterCounts(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 4, 118);
			}

			// Token: 0x0601172D RID: 71469 RVA: 0x0067FF28 File Offset: 0x0067E128
			public static void ClearCharacterSortFilter()
			{
				GameDataBridge.AddMethodCall(-1, 4, 119);
			}

			// Token: 0x0601172E RID: 71470 RVA: 0x0067FF35 File Offset: 0x0067E135
			public static void UpdateSortFilterSettings(int listenerId, CharacterSortFilterSettings sortFilterSettings)
			{
				GameDataBridge.AddMethodCall<CharacterSortFilterSettings>(listenerId, 4, 120, sortFilterSettings);
			}

			// Token: 0x0601172F RID: 71471 RVA: 0x0067FF43 File Offset: 0x0067E143
			public static void InitializeCharacterSortFilter(int listenerId, CharacterSortFilterSettings sortFilterSettings)
			{
				GameDataBridge.AddMethodCall<CharacterSortFilterSettings>(listenerId, 4, 121, sortFilterSettings);
			}

			// Token: 0x06011730 RID: 71472 RVA: 0x0067FF51 File Offset: 0x0067E151
			public static void FindNameInCurrentSortFilter(int listenerId, string name)
			{
				GameDataBridge.AddMethodCall<string>(listenerId, 4, 122, name);
			}

			// Token: 0x06011731 RID: 71473 RVA: 0x0067FF5F File Offset: 0x0067E15F
			public static void GetCharacterDisplayDataListForUltimateSelect(int listenerId, List<int> charIdList)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 4, 123, charIdList);
			}

			// Token: 0x06011732 RID: 71474 RVA: 0x0067FF6D File Offset: 0x0067E16D
			public static void GetMaxSortingTypeCharIds(int listenerId, List<int> sortingTypes, sbyte filterSubId)
			{
				GameDataBridge.AddMethodCall<List<int>, sbyte>(listenerId, 4, 124, sortingTypes, filterSubId);
			}

			// Token: 0x06011733 RID: 71475 RVA: 0x0067FF7C File Offset: 0x0067E17C
			public static void GmCmd_SetCurrNeili(int charId, int value)
			{
				GameDataBridge.AddMethodCall<int, int>(-1, 4, 125, charId, value);
			}

			// Token: 0x06011734 RID: 71476 RVA: 0x0067FF8C File Offset: 0x0067E18C
			public static void GmCmd_AddPoisonedInventoryItem(int charId, sbyte baseItemType, short baseItemId, short poisonId1, short poisonId2, short poisonId3)
			{
				GameDataBridge.AddMethodCall<int, sbyte, short, short, short, short>(-1, 4, 126, charId, baseItemType, baseItemId, poisonId1, poisonId2, poisonId3);
			}

			// Token: 0x06011735 RID: 71477 RVA: 0x0067FFAC File Offset: 0x0067E1AC
			public static void GmCmd_AddPoisonedEatingItem(int charId, sbyte baseItemType, short baseItemId, short poisonId1, short poisonId2, short poisonId3)
			{
				GameDataBridge.AddMethodCall<int, sbyte, short, short, short, short>(-1, 4, 127, charId, baseItemType, baseItemId, poisonId1, poisonId2, poisonId3);
			}

			// Token: 0x06011736 RID: 71478 RVA: 0x0067FFCC File Offset: 0x0067E1CC
			public static void GmCmd_SetCharBaseNeiliProportionOfFiveElements(int charId, NeiliProportionOfFiveElements fiveElements)
			{
				GameDataBridge.AddMethodCall<int, NeiliProportionOfFiveElements>(-1, 4, 128, charId, fiveElements);
			}

			// Token: 0x06011737 RID: 71479 RVA: 0x0067FFDE File Offset: 0x0067E1DE
			public static void GetRelationBetweenCharacters(int listenerId, int charId, int relatedCharId)
			{
				GameDataBridge.AddMethodCall<int, int>(listenerId, 4, 129, charId, relatedCharId);
			}

			// Token: 0x06011738 RID: 71480 RVA: 0x0067FFF0 File Offset: 0x0067E1F0
			public static void GetInventoryItemsByItemType(int listenerId, int charId, sbyte itemType)
			{
				GameDataBridge.AddMethodCall<int, sbyte>(listenerId, 4, 130, charId, itemType);
			}

			// Token: 0x06011739 RID: 71481 RVA: 0x00680002 File Offset: 0x0067E202
			public static void GetCharacterDisplayData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 131, charId);
			}

			// Token: 0x0601173A RID: 71482 RVA: 0x00680013 File Offset: 0x0067E213
			public static void UnequipAllCombatSkills(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 4, 132, charId);
			}

			// Token: 0x0601173B RID: 71483 RVA: 0x00680024 File Offset: 0x0067E224
			public static void AutoEquipCombatSkills(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 4, 133, charId);
			}

			// Token: 0x0601173C RID: 71484 RVA: 0x00680035 File Offset: 0x0067E235
			public static void AutoEquipCombatSkills(int charId, short combatConfigTemplateId)
			{
				GameDataBridge.AddMethodCall<int, short>(-1, 4, 133, charId, combatConfigTemplateId);
			}

			// Token: 0x0601173D RID: 71485 RVA: 0x00680047 File Offset: 0x0067E247
			public static void GmCmd_AddCharacterExtraTitle(int charId, short titleTemplateId)
			{
				GameDataBridge.AddMethodCall<int, short>(-1, 4, 134, charId, titleTemplateId);
			}

			// Token: 0x0601173E RID: 71486 RVA: 0x00680059 File Offset: 0x0067E259
			public static void GmCmd_AddCharacterExtraTitle(int charId, short titleTemplateId, int duration)
			{
				GameDataBridge.AddMethodCall<int, short, int>(-1, 4, 134, charId, titleTemplateId, duration);
			}

			// Token: 0x0601173F RID: 71487 RVA: 0x0068006C File Offset: 0x0067E26C
			public static void TryAddAndApplyOneWayRelation(int charId, int relatedCharId, ushort relationType)
			{
				GameDataBridge.AddMethodCall<int, int, ushort>(-1, 4, 135, charId, relatedCharId, relationType);
			}

			// Token: 0x06011740 RID: 71488 RVA: 0x0068007F File Offset: 0x0067E27F
			public static void MoveFuyuHiltLocation(Location targetLocation)
			{
				GameDataBridge.AddMethodCall<Location>(-1, 4, 136, targetLocation);
			}

			// Token: 0x06011741 RID: 71489 RVA: 0x00680090 File Offset: 0x0067E290
			public static void MoveFuyuHiltLocation(Location targetLocation, bool hide)
			{
				GameDataBridge.AddMethodCall<Location, bool>(-1, 4, 136, targetLocation, hide);
			}

			// Token: 0x06011742 RID: 71490 RVA: 0x006800A2 File Offset: 0x0067E2A2
			public static void TryRemoveOneWayRelation(int charId, int relatedCharId, ushort relationType)
			{
				GameDataBridge.AddMethodCall<int, int, ushort>(-1, 4, 137, charId, relatedCharId, relationType);
			}

			// Token: 0x06011743 RID: 71491 RVA: 0x006800B5 File Offset: 0x0067E2B5
			public static void GetCharacterLoveAndHateItemInfo(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 138, charId);
			}

			// Token: 0x06011744 RID: 71492 RVA: 0x006800C6 File Offset: 0x0067E2C6
			public static void IsTemporaryIntelligentCharacter(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 139, charId);
			}

			// Token: 0x06011745 RID: 71493 RVA: 0x006800D7 File Offset: 0x0067E2D7
			public static void GetMixedPoisonTypeRelatedMarkCountArray(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 140, charId);
			}

			// Token: 0x06011746 RID: 71494 RVA: 0x006800E8 File Offset: 0x0067E2E8
			public static void SimulateEatingEffect(int listenerId, int charId, ItemKey itemKey, int amount)
			{
				GameDataBridge.AddMethodCall<int, ItemKey, int>(listenerId, 4, 141, charId, itemKey, amount);
			}

			// Token: 0x06011747 RID: 71495 RVA: 0x006800FB File Offset: 0x0067E2FB
			public static void GetCharacterDisplayDataForTooltip(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 142, charId);
			}

			// Token: 0x06011748 RID: 71496 RVA: 0x0068010C File Offset: 0x0067E30C
			public static void GmCmd_SetCurLoopingEvent(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 4, 143, charId);
			}

			// Token: 0x06011749 RID: 71497 RVA: 0x0068011D File Offset: 0x0067E31D
			public static void GetHealthType(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 144, charId);
			}

			// Token: 0x0601174A RID: 71498 RVA: 0x0068012E File Offset: 0x0067E32E
			public static void GetCharacterLocationDisplayData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 145, charId);
			}

			// Token: 0x0601174B RID: 71499 RVA: 0x0068013F File Offset: 0x0067E33F
			public static void GetCharacterDisplayDataForMapBlock(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 146, charId);
			}

			// Token: 0x0601174C RID: 71500 RVA: 0x00680150 File Offset: 0x0067E350
			public static void GetCharacterWisdomList(int listenerId, List<int> charIdList)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 4, 147, charIdList);
			}

			// Token: 0x0601174D RID: 71501 RVA: 0x00680161 File Offset: 0x0067E361
			public static void GetOrCreateSwordTombCharacterIdForNormalInformation(int listenerId, sbyte xiangshuAvatarId)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 4, 148, xiangshuAvatarId);
			}

			// Token: 0x0601174E RID: 71502 RVA: 0x00680172 File Offset: 0x0067E372
			public static void GetAllInventoryItemsExcludeValueZero(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 149, charId);
			}

			// Token: 0x0601174F RID: 71503 RVA: 0x00680183 File Offset: 0x0067E383
			public static void GetAddConsummateLevelRequiredMonth(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 150, charId);
			}

			// Token: 0x06011750 RID: 71504 RVA: 0x00680194 File Offset: 0x0067E394
			public static void GmCmd_ResetHairGrowth(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 4, 151, charId);
			}

			// Token: 0x06011751 RID: 71505 RVA: 0x006801A5 File Offset: 0x0067E3A5
			public static void GetAllItemsByAreaId(int listenerId, short areaId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 4, 152, areaId);
			}

			// Token: 0x06011752 RID: 71506 RVA: 0x006801B6 File Offset: 0x0067E3B6
			public static void SetCombatSkillPlanLock(int charId, bool isLocked)
			{
				GameDataBridge.AddMethodCall<int, bool>(-1, 4, 153, charId, isLocked);
			}

			// Token: 0x06011753 RID: 71507 RVA: 0x006801C8 File Offset: 0x0067E3C8
			public static void AppendCombatSkillPlan(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 4, 154, charId);
			}

			// Token: 0x06011754 RID: 71508 RVA: 0x006801D9 File Offset: 0x0067E3D9
			public static void DuplicateCurrentCombatSkillPlan(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 4, 155, charId);
			}

			// Token: 0x06011755 RID: 71509 RVA: 0x006801EA File Offset: 0x0067E3EA
			public static void UpdateCombatSkillPlan(int charId, int planId)
			{
				GameDataBridge.AddMethodCall<int, int>(-1, 4, 156, charId, planId);
			}

			// Token: 0x06011756 RID: 71510 RVA: 0x006801FC File Offset: 0x0067E3FC
			public static void GetCurrentPlanIdAndPlanCount(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 157, charId);
			}

			// Token: 0x06011757 RID: 71511 RVA: 0x0068020D File Offset: 0x0067E40D
			public static void DeleteCombatSkillPlan(int charId, int planId)
			{
				GameDataBridge.AddMethodCall<int, int>(-1, 4, 158, charId, planId);
			}

			// Token: 0x06011758 RID: 71512 RVA: 0x0068021F File Offset: 0x0067E41F
			public static void IsInteractedWithTaiwu(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 159, charId);
			}

			// Token: 0x06011759 RID: 71513 RVA: 0x00680230 File Offset: 0x0067E430
			public static void IsNeiliAllocationLocked(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 160, charId);
			}

			// Token: 0x0601175A RID: 71514 RVA: 0x00680241 File Offset: 0x0067E441
			public static void AllocateGenericGrid(int charId, sbyte equipType)
			{
				GameDataBridge.AddMethodCall<int, sbyte>(-1, 4, 161, charId, equipType);
			}

			// Token: 0x0601175B RID: 71515 RVA: 0x00680253 File Offset: 0x0067E453
			public static void DeallocateGenericGrid(int charId, sbyte equipType)
			{
				GameDataBridge.AddMethodCall<int, sbyte>(-1, 4, 162, charId, equipType);
			}

			// Token: 0x0601175C RID: 71516 RVA: 0x00680265 File Offset: 0x0067E465
			public static void IsCombatSkillEquipmentLocked(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 163, charId);
			}

			// Token: 0x0601175D RID: 71517 RVA: 0x00680276 File Offset: 0x0067E476
			public static void AutoAllocateNeili(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 4, 164, charId);
			}

			// Token: 0x0601175E RID: 71518 RVA: 0x00680287 File Offset: 0x0067E487
			public static void AutoSetCombatSkillAttainmentPanels(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 4, 165, charId);
			}

			// Token: 0x0601175F RID: 71519 RVA: 0x00680298 File Offset: 0x0067E498
			public static void AutoEquipItems(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 4, 166, charId);
			}

			// Token: 0x06011760 RID: 71520 RVA: 0x006802A9 File Offset: 0x0067E4A9
			public static void SetCombatSkillAttainmentLock(int charId, bool isLocked)
			{
				GameDataBridge.AddMethodCall<int, bool>(-1, 4, 167, charId, isLocked);
			}

			// Token: 0x06011761 RID: 71521 RVA: 0x006802BB File Offset: 0x0067E4BB
			public static void IsCombatSkillAttainmentLocked(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 168, charId);
			}

			// Token: 0x06011762 RID: 71522 RVA: 0x006802CC File Offset: 0x0067E4CC
			public static void GetGenericGridAllocation(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 169, charId);
			}

			// Token: 0x06011763 RID: 71523 RVA: 0x006802DD File Offset: 0x0067E4DD
			public static void SetNeiliAllocationLock(int charId, bool isLocked)
			{
				GameDataBridge.AddMethodCall<int, bool>(-1, 4, 170, charId, isLocked);
			}

			// Token: 0x06011764 RID: 71524 RVA: 0x006802EF File Offset: 0x0067E4EF
			public static void RemoveEquippedCombatSkill(int charId, short skillTemplateId)
			{
				GameDataBridge.AddMethodCall<int, short>(-1, 4, 171, charId, skillTemplateId);
			}

			// Token: 0x06011765 RID: 71525 RVA: 0x00680301 File Offset: 0x0067E501
			public static void AddEquippedCombatSkill(int charId, short skillTemplateId)
			{
				GameDataBridge.AddMethodCall<int, short>(-1, 4, 172, charId, skillTemplateId);
			}

			// Token: 0x06011766 RID: 71526 RVA: 0x00680313 File Offset: 0x0067E513
			public static void GetCombatSkillExtraSlotCounts(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 173, charId);
			}

			// Token: 0x06011767 RID: 71527 RVA: 0x00680324 File Offset: 0x0067E524
			public static void GetCharacterAllBodyPartExists(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 174, charId);
			}

			// Token: 0x06011768 RID: 71528 RVA: 0x00680335 File Offset: 0x0067E535
			public static void GmCmd_ChangeCharDisorderOfQi(int charId, int delta)
			{
				GameDataBridge.AddMethodCall<int, int>(-1, 4, 175, charId, delta);
			}

			// Token: 0x06011769 RID: 71529 RVA: 0x00680347 File Offset: 0x0067E547
			public static void GenerateRandomName(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 4, 176);
			}

			// Token: 0x0601176A RID: 71530 RVA: 0x00680357 File Offset: 0x0067E557
			public static void GetCharacterDisplayDataListForRelationsWithRelationType(int listenerId, int currCharId, List<int> charIds)
			{
				GameDataBridge.AddMethodCall<int, List<int>>(listenerId, 4, 177, currCharId, charIds);
			}

			// Token: 0x0601176B RID: 71531 RVA: 0x00680369 File Offset: 0x0067E569
			public static void GetPhysiologicalAge(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 178, charId);
			}

			// Token: 0x0601176C RID: 71532 RVA: 0x0068037A File Offset: 0x0067E57A
			public static void GetPhysiologicalAgeAffector(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 179, charId);
			}

			// Token: 0x0601176D RID: 71533 RVA: 0x0068038B File Offset: 0x0067E58B
			public static void GetKidnappedCharacterDisplayData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 180, charId);
			}

			// Token: 0x0601176E RID: 71534 RVA: 0x0068039C File Offset: 0x0067E59C
			public static void IsCarrierDurabilityRunningOut(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 181, charId);
			}

			// Token: 0x0601176F RID: 71535 RVA: 0x006803AD File Offset: 0x0067E5AD
			public static void GmCmd_ForceChangeOrganizationByName(int listenerId, int charId, string settlementName)
			{
				GameDataBridge.AddMethodCall<int, string>(listenerId, 4, 182, charId, settlementName);
			}

			// Token: 0x06011770 RID: 71536 RVA: 0x006803BF File Offset: 0x0067E5BF
			public static void GetAvailableFeature(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 183, charId);
			}

			// Token: 0x06011771 RID: 71537 RVA: 0x006803D0 File Offset: 0x0067E5D0
			public static void GetEquipmentCompare(int listenerId, int charId, ItemKey[] baselineEquipment, ItemKey[] targetEquipment, sbyte slotIndex)
			{
				GameDataBridge.AddMethodCall<int, ItemKey[], ItemKey[], sbyte>(listenerId, 4, 184, charId, baselineEquipment, targetEquipment, slotIndex);
			}

			// Token: 0x06011772 RID: 71538 RVA: 0x006803E5 File Offset: 0x0067E5E5
			public static void GetCharacterTableDisplayData(int listenerId, int charId, List<short> types)
			{
				GameDataBridge.AddMethodCall<int, List<short>>(listenerId, 4, 185, charId, types);
			}

			// Token: 0x06011773 RID: 71539 RVA: 0x006803F7 File Offset: 0x0067E5F7
			public static void GetCharacterTableDisplayDataList(int listenerId, List<int> charIds, List<short> types)
			{
				GameDataBridge.AddMethodCall<List<int>, List<short>>(listenerId, 4, 186, charIds, types);
			}

			// Token: 0x06011774 RID: 71540 RVA: 0x00680409 File Offset: 0x0067E609
			public static void GetCharacterTableDisplayDataListWithNeedItem(int listenerId, List<short> types, ItemKey itemKey)
			{
				GameDataBridge.AddMethodCall<List<short>, ItemKey>(listenerId, 4, 187, types, itemKey);
			}

			// Token: 0x06011775 RID: 71541 RVA: 0x0068041B File Offset: 0x0067E61B
			public static void TryGetGraveDisplayDataList(int listenerId, List<int> charIdList)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 4, 188, charIdList);
			}

			// Token: 0x06011776 RID: 71542 RVA: 0x0068042C File Offset: 0x0067E62C
			public static void GetDeadCharacterDisplayDataForTooltip(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 189, charId);
			}

			// Token: 0x06011777 RID: 71543 RVA: 0x0068043D File Offset: 0x0067E63D
			public static void GmCmd_DriveWugKing(int charId, sbyte wugType, bool isPositive)
			{
				GameDataBridge.AddMethodCall<int, sbyte, bool>(-1, 4, 190, charId, wugType, isPositive);
			}

			// Token: 0x06011778 RID: 71544 RVA: 0x00680450 File Offset: 0x0067E650
			public static void GetCharacterCurrentProfession(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 191, charId);
			}

			// Token: 0x06011779 RID: 71545 RVA: 0x00680461 File Offset: 0x0067E661
			public static void GmCmd_SetCharacterCurrProfessionSeniority(int charId, int professionId, int value)
			{
				GameDataBridge.AddMethodCall<int, int, int>(-1, 4, 192, charId, professionId, value);
			}

			// Token: 0x0601177A RID: 71546 RVA: 0x00680474 File Offset: 0x0067E674
			public static void GetCharacterTemporaryFeaturesExpireDate(int listenerId, IntPair key)
			{
				GameDataBridge.AddMethodCall<IntPair>(listenerId, 4, 193, key);
			}

			// Token: 0x0601177B RID: 71547 RVA: 0x00680485 File Offset: 0x0067E685
			public static void GetExtraNeiliAllocationProgress(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 194, charId);
			}

			// Token: 0x0601177C RID: 71548 RVA: 0x00680496 File Offset: 0x0067E696
			public static void GetCharacterMenuInfoDisplayData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 195, charId);
			}

			// Token: 0x0601177D RID: 71549 RVA: 0x006804A7 File Offset: 0x0067E6A7
			public static void GetAttributeWithDelta(int listenerId, int charId, ItemKey equipKey)
			{
				GameDataBridge.AddMethodCall<int, ItemKey>(listenerId, 4, 196, charId, equipKey);
			}

			// Token: 0x0601177E RID: 71550 RVA: 0x006804B9 File Offset: 0x0067E6B9
			public static void GetAttributeWithDelta(int listenerId, int charId, ItemKey equipKey, int slot)
			{
				GameDataBridge.AddMethodCall<int, ItemKey, int>(listenerId, 4, 196, charId, equipKey, slot);
			}

			// Token: 0x0601177F RID: 71551 RVA: 0x006804CC File Offset: 0x0067E6CC
			public static void GetAttributeDelta(int listenerId, int charId, ItemKey equipKey, int slot)
			{
				GameDataBridge.AddMethodCall<int, ItemKey, int>(listenerId, 4, 197, charId, equipKey, slot);
			}

			// Token: 0x06011780 RID: 71552 RVA: 0x006804DF File Offset: 0x0067E6DF
			public static void GetCharacterMenuLifeSkillDisplayData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 198, charId);
			}

			// Token: 0x06011781 RID: 71553 RVA: 0x006804F0 File Offset: 0x0067E6F0
			public static void GetEquipLoad(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 199, charId);
			}

			// Token: 0x06011782 RID: 71554 RVA: 0x00680501 File Offset: 0x0067E701
			public static void GetCharacterDisplayDataForNeiliPage(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 200, charId);
			}

			// Token: 0x06011783 RID: 71555 RVA: 0x00680512 File Offset: 0x0067E712
			public static void GetCharacterInjuryDisplayData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 201, charId);
			}

			// Token: 0x06011784 RID: 71556 RVA: 0x00680523 File Offset: 0x0067E723
			public static void GetCharacterMenuAttainmentDisplayData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 202, charId);
			}

			// Token: 0x06011785 RID: 71557 RVA: 0x00680534 File Offset: 0x0067E734
			public static void GetKidnapMenuDisplayData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 203, charId);
			}

			// Token: 0x06011786 RID: 71558 RVA: 0x00680545 File Offset: 0x0067E745
			public static void GetPersonalities(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 204, charId);
			}

			// Token: 0x06011787 RID: 71559 RVA: 0x00680556 File Offset: 0x0067E756
			public static void GetCharacterDisplayDataForPractice(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 205, charId);
			}

			// Token: 0x06011788 RID: 71560 RVA: 0x00680567 File Offset: 0x0067E767
			public static void GetCharacterUsingMedicineDisplayData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 206, charId);
			}

			// Token: 0x06011789 RID: 71561 RVA: 0x00680578 File Offset: 0x0067E778
			public static void GetCharacterItemsDisplayData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 207, charId);
			}

			// Token: 0x0601178A RID: 71562 RVA: 0x00680589 File Offset: 0x0067E789
			public static void GetViewCharacterMenuDisplayData(int listenerId, int charId, List<int> charIdList)
			{
				GameDataBridge.AddMethodCall<int, List<int>>(listenerId, 4, 208, charId, charIdList);
			}

			// Token: 0x0601178B RID: 71563 RVA: 0x0068059B File Offset: 0x0067E79B
			public static void GetCharacterDisplayDataForGeneralScrollListBatch(int listenerId, List<int> charIdList)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 4, 209, charIdList);
			}

			// Token: 0x0601178C RID: 71564 RVA: 0x006805AC File Offset: 0x0067E7AC
			public static void GetYuanshanSelectDataList(int listenerId, List<int> charIds)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 4, 210, charIds);
			}

			// Token: 0x0601178D RID: 71565 RVA: 0x006805BD File Offset: 0x0067E7BD
			public static void GetVillagerCharDisplayDataList(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 4, 211);
			}

			// Token: 0x0601178E RID: 71566 RVA: 0x006805CD File Offset: 0x0067E7CD
			public static void GetCharacterDisplayDataForBaihuaLifeLink(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 4, 212);
			}

			// Token: 0x0601178F RID: 71567 RVA: 0x006805DD File Offset: 0x0067E7DD
			public static void GetAlertnessValue(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 213, charId);
			}

			// Token: 0x06011790 RID: 71568 RVA: 0x006805EE File Offset: 0x0067E7EE
			public static void SetAlertnessValue(int charId, int value)
			{
				GameDataBridge.AddMethodCall<int, int>(-1, 4, 214, charId, value);
			}

			// Token: 0x06011791 RID: 71569 RVA: 0x00680600 File Offset: 0x0067E800
			public static void GetAlertnessData(int listenerId, int charId, bool init)
			{
				GameDataBridge.AddMethodCall<int, bool>(listenerId, 4, 215, charId, init);
			}

			// Token: 0x06011792 RID: 71570 RVA: 0x00680612 File Offset: 0x0067E812
			public static void GetTransferItemPreviewDisplayData(int listenerId, int characterId, Inventory items, bool isGive)
			{
				GameDataBridge.AddMethodCall<int, Inventory, bool>(listenerId, 4, 216, characterId, items, isGive);
			}

			// Token: 0x06011793 RID: 71571 RVA: 0x00680625 File Offset: 0x0067E825
			public static void GetAllRanshanReadBooksData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 217, charId);
			}

			// Token: 0x06011794 RID: 71572 RVA: 0x00680636 File Offset: 0x0067E836
			public static void GetCharacterOverviewEatingDisplayData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 218, charId);
			}

			// Token: 0x06011795 RID: 71573 RVA: 0x00680647 File Offset: 0x0067E847
			public static void GetEquipmentKeys(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 219, charId);
			}

			// Token: 0x06011796 RID: 71574 RVA: 0x00680658 File Offset: 0x0067E858
			public static void GetCharDisplayDataListAsVillager(int listenerId, List<int> charIds)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 4, 220, charIds);
			}

			// Token: 0x06011797 RID: 71575 RVA: 0x00680669 File Offset: 0x0067E869
			public static void GetPreviewLeftMaxHealth(int listenerId, int charId, short baseMaxHealthOffset)
			{
				GameDataBridge.AddMethodCall<int, short>(listenerId, 4, 221, charId, baseMaxHealthOffset);
			}

			// Token: 0x06011798 RID: 71576 RVA: 0x0068067B File Offset: 0x0067E87B
			public static void GetGraveDisplayDataListForSelection(int listenerId, List<int> charIdList)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 4, 222, charIdList);
			}

			// Token: 0x06011799 RID: 71577 RVA: 0x0068068C File Offset: 0x0067E88C
			public static void GetCharacterDisplayDataForBeggarUltimate(int listenerId, List<int> charIdList)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 4, 223, charIdList);
			}

			// Token: 0x0601179A RID: 71578 RVA: 0x0068069D File Offset: 0x0067E89D
			public static void GetAvatarRelatedDataListIncludeDead(int listenerId, List<int> charIdList)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 4, 224, charIdList);
			}

			// Token: 0x0601179B RID: 71579 RVA: 0x006806AE File Offset: 0x0067E8AE
			public static void GetFixedCharacterName(int listenerId, short templateId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 4, 225, templateId);
			}

			// Token: 0x0601179C RID: 71580 RVA: 0x006806BF File Offset: 0x0067E8BF
			public static void GetCharacterDisplayDataForGuard(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 226, charId);
			}

			// Token: 0x0601179D RID: 71581 RVA: 0x006806D0 File Offset: 0x0067E8D0
			public static void SimulateProfessionDoctorSkill0(int listenerId, int patientId, ItemKey itemKey, int amount, int doctorId)
			{
				GameDataBridge.AddMethodCall<int, ItemKey, int, int>(listenerId, 4, 227, patientId, itemKey, amount, doctorId);
			}

			// Token: 0x0601179E RID: 71582 RVA: 0x006806E5 File Offset: 0x0067E8E5
			public static void GetCharacterDivinePower(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 228, charId);
			}

			// Token: 0x0601179F RID: 71583 RVA: 0x006806F6 File Offset: 0x0067E8F6
			public static void GetCharacterGhostTechnique(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 229, charId);
			}

			// Token: 0x060117A0 RID: 71584 RVA: 0x00680707 File Offset: 0x0067E907
			public static void PreviewAllocateNeili(int listenerId, int charId, byte neiliAllocationType, bool isAdd)
			{
				GameDataBridge.AddMethodCall<int, byte, bool>(listenerId, 4, 230, charId, neiliAllocationType, isAdd);
			}

			// Token: 0x060117A1 RID: 71585 RVA: 0x0068071A File Offset: 0x0067E91A
			public static void GetCharacterDisplayDataForTasterUltimate(int listenerId, List<ItemKey> bookItemKeyList)
			{
				GameDataBridge.AddMethodCall<List<ItemKey>>(listenerId, 4, 231, bookItemKeyList);
			}

			// Token: 0x060117A2 RID: 71586 RVA: 0x0068072B File Offset: 0x0067E92B
			public static void GmCmd_ChangeXiangshuInfection(int charId, int delta)
			{
				GameDataBridge.AddMethodCall<int, int>(-1, 4, 232, charId, delta);
			}

			// Token: 0x060117A3 RID: 71587 RVA: 0x0068073D File Offset: 0x0067E93D
			public static void TransferInventoryItemInventoryWithDebt(int srcCharId, int destCharId, Inventory inventory, bool checkFavorability)
			{
				GameDataBridge.AddMethodCall<int, int, Inventory, bool>(-1, 4, 233, srcCharId, destCharId, inventory, checkFavorability);
			}

			// Token: 0x060117A4 RID: 71588 RVA: 0x00680751 File Offset: 0x0067E951
			public static void GetActionPlanningDsiplayData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 234, charId);
			}

			// Token: 0x060117A5 RID: 71589 RVA: 0x00680762 File Offset: 0x0067E962
			public static void GetCharacterProfessionList(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 235, charId);
			}

			// Token: 0x060117A6 RID: 71590 RVA: 0x00680773 File Offset: 0x0067E973
			public static void GetCarrierMaxProperty(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 4, 236, charId);
			}

			// Token: 0x060117A7 RID: 71591 RVA: 0x00680784 File Offset: 0x0067E984
			public static void GmCmd_ClearCharacterActionPlanningData(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 4, 237, charId);
			}

			// Token: 0x060117A8 RID: 71592 RVA: 0x00680795 File Offset: 0x0067E995
			public static void GmCmd_ClearAllActionPlanningData()
			{
				GameDataBridge.AddMethodCall(-1, 4, 238);
			}
		}

		// Token: 0x02002616 RID: 9750
		public static class AsyncCall
		{
			// Token: 0x060117A9 RID: 71593 RVA: 0x006807A8 File Offset: 0x0067E9A8
			public static void CreateProtagonist(IAsyncMethodRequestHandler requestHandler, ProtagonistCreationInfo info, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ProtagonistCreationInfo>(4, 0, info, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117AA RID: 71594 RVA: 0x006807D4 File Offset: 0x0067E9D4
			public static void GetRelatedCharactersForRelations(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 1, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117AB RID: 71595 RVA: 0x006807FE File Offset: 0x0067E9FE
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.TryCreateRelation instead.", true)]
			public static void TryCreateRelation(IAsyncMethodRequestHandler requestHandler, int charId, int relatedCharId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117AC RID: 71596 RVA: 0x00680808 File Offset: 0x0067EA08
			public static void GetGenealogy(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 3, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117AD RID: 71597 RVA: 0x00680834 File Offset: 0x0067EA34
			public static void GenerateRandomHanName(IAsyncMethodRequestHandler requestHandler, int customSurnameId, short surnameId, sbyte gender, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short, sbyte>(4, 4, customSurnameId, surnameId, gender, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117AE RID: 71598 RVA: 0x00680864 File Offset: 0x0067EA64
			public static void GenerateRandomZangName(IAsyncMethodRequestHandler requestHandler, sbyte gender, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(4, 5, gender, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117AF RID: 71599 RVA: 0x00680890 File Offset: 0x0067EA90
			public static void GenerateRandomChildName(IAsyncMethodRequestHandler requestHandler, sbyte gender, FullName parentName, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte, FullName>(4, 6, gender, parentName, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117B0 RID: 71600 RVA: 0x006808BC File Offset: 0x0067EABC
			public static void GetNameRelatedDataList(IAsyncMethodRequestHandler requestHandler, List<int> charIds, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(4, 7, charIds, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117B1 RID: 71601 RVA: 0x006808E8 File Offset: 0x0067EAE8
			public static void GetNameRelatedData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 8, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117B2 RID: 71602 RVA: 0x00680914 File Offset: 0x0067EB14
			public static void GetNameAndLifeRelatedDataList(IAsyncMethodRequestHandler requestHandler, List<int> charIds, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(4, 9, charIds, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117B3 RID: 71603 RVA: 0x00680940 File Offset: 0x0067EB40
			public static void GetNameAndLifeRelatedData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 10, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117B4 RID: 71604 RVA: 0x0068096C File Offset: 0x0067EB6C
			public static void GetFavorability(IAsyncMethodRequestHandler requestHandler, int charId, int relatedCharId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int>(4, 11, charId, relatedCharId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117B5 RID: 71605 RVA: 0x00680998 File Offset: 0x0067EB98
			public static void GmCmd_GetAllGroupMembers(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(4, 12, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117B6 RID: 71606 RVA: 0x006809C2 File Offset: 0x0067EBC2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_GenerateRandomRefinedItemToCharacter instead.", true)]
			public static void GmCmd_GenerateRandomRefinedItemToCharacter(IAsyncMethodRequestHandler requestHandler, int charId, sbyte itemType, int times, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117B7 RID: 71607 RVA: 0x006809CA File Offset: 0x0067EBCA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_ChangeInjury instead.", true)]
			public static void GmCmd_ChangeInjury(IAsyncMethodRequestHandler requestHandler, int charId, bool isInnerInjury, sbyte bodyPartType, sbyte delta, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117B8 RID: 71608 RVA: 0x006809D2 File Offset: 0x0067EBD2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_ChangePoisonByType instead.", true)]
			public static void GmCmd_ChangePoisonByType(IAsyncMethodRequestHandler requestHandler, int charId, sbyte poisonType, int changeValue, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117B9 RID: 71609 RVA: 0x006809DA File Offset: 0x0067EBDA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_ForgetCombatSkill instead.", true)]
			public static void GmCmd_ForgetCombatSkill(IAsyncMethodRequestHandler requestHandler, int charId, short skillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117BA RID: 71610 RVA: 0x006809E2 File Offset: 0x0067EBE2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_RevokeCombatSkill instead.", true)]
			public static void GmCmd_RevokeCombatSkill(IAsyncMethodRequestHandler requestHandler, int charId, List<short> skillTemplateIdList, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117BB RID: 71611 RVA: 0x006809EA File Offset: 0x0067EBEA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_SetLearnedLifeSkills instead.", true)]
			public static void GmCmd_SetLearnedLifeSkills(IAsyncMethodRequestHandler requestHandler, int charId, List<LifeSkillItem> learnedLifeSkills, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117BC RID: 71612 RVA: 0x006809F2 File Offset: 0x0067EBF2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_GetCricket instead.", true)]
			public static void GmCmd_GetCricket(IAsyncMethodRequestHandler requestHandler, short colorId, short partId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117BD RID: 71613 RVA: 0x006809FA File Offset: 0x0067EBFA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_GetCricket instead.", true)]
			public static void GmCmd_GetCricket(IAsyncMethodRequestHandler requestHandler, short colorId, short partId, int grade, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117BE RID: 71614 RVA: 0x00680A02 File Offset: 0x0067EC02
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_GetCricket instead.", true)]
			public static void GmCmd_GetCricket(IAsyncMethodRequestHandler requestHandler, short colorId, short partId, int grade, short winsCount, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117BF RID: 71615 RVA: 0x00680A0A File Offset: 0x0067EC0A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_GetCricket instead.", true)]
			public static void GmCmd_GetCricket(IAsyncMethodRequestHandler requestHandler, short colorId, short partId, int grade, short winsCount, short lossesCount, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117C0 RID: 71616 RVA: 0x00680A12 File Offset: 0x0067EC12
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_AddRelation instead.", true)]
			public static void GmCmd_AddRelation(IAsyncMethodRequestHandler requestHandler, int charId, int relatedCharId, ushort addingType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117C1 RID: 71617 RVA: 0x00680A1C File Offset: 0x0067EC1C
			public static void GetGroupSet(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 21, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117C2 RID: 71618 RVA: 0x00680A47 File Offset: 0x0067EC47
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.TransferResourcesWithDebt instead.", true)]
			public static void TransferResourcesWithDebt(IAsyncMethodRequestHandler requestHandler, int srcCharId, int destCharId, ResourceInts resources, bool checkFavorability, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117C3 RID: 71619 RVA: 0x00680A4F File Offset: 0x0067EC4F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.TransferInventoryItemWithDebt instead.", true)]
			public static void TransferInventoryItemWithDebt(IAsyncMethodRequestHandler requestHandler, int srcCharId, int destCharId, ItemKey itemKey, int amount, bool checkFavorability, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117C4 RID: 71620 RVA: 0x00680A57 File Offset: 0x0067EC57
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.ChangeEquipment instead.", true)]
			public static void ChangeEquipment(IAsyncMethodRequestHandler requestHandler, int charId, sbyte srcSlot, sbyte destSlot, ItemKey srcItemKey, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117C5 RID: 71621 RVA: 0x00680A5F File Offset: 0x0067EC5F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.CreateInventoryItem instead.", true)]
			public static void CreateInventoryItem(IAsyncMethodRequestHandler requestHandler, int charId, sbyte itemType, short templateId, int amount, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117C6 RID: 71622 RVA: 0x00680A68 File Offset: 0x0067EC68
			public static void GetInventoryItems(IAsyncMethodRequestHandler requestHandler, int charId, short itemSubType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short>(4, 26, charId, itemSubType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117C7 RID: 71623 RVA: 0x00680A94 File Offset: 0x0067EC94
			public static void GetAllInventoryItems(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 27, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117C8 RID: 71624 RVA: 0x00680AC0 File Offset: 0x0067ECC0
			public static void GetInventoryItemAmount(IAsyncMethodRequestHandler requestHandler, int charId, sbyte itemType, short templateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, sbyte, short>(4, 28, charId, itemType, templateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117C9 RID: 71625 RVA: 0x00680AF0 File Offset: 0x0067ECF0
			public static void GetAllEquipmentItems(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 29, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117CA RID: 71626 RVA: 0x00680B1C File Offset: 0x0067ED1C
			public static void GetInventoryItemDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, ItemKey itemKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ItemKey>(4, 30, charId, itemKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117CB RID: 71627 RVA: 0x00680B48 File Offset: 0x0067ED48
			public static void InventoryContainsItem(IAsyncMethodRequestHandler requestHandler, int charId, ItemKey itemKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ItemKey>(4, 31, charId, itemKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117CC RID: 71628 RVA: 0x00680B74 File Offset: 0x0067ED74
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.AddEatingItem instead.", true)]
			public static void AddEatingItem(IAsyncMethodRequestHandler requestHandler, int charId, ItemKey itemKey, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117CD RID: 71629 RVA: 0x00680B7C File Offset: 0x0067ED7C
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.AddEatingItem instead.", true)]
			public static void AddEatingItem(IAsyncMethodRequestHandler requestHandler, int charId, ItemKey itemKey, List<sbyte> targetBodyParts, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117CE RID: 71630 RVA: 0x00680B84 File Offset: 0x0067ED84
			public static void GetCurrMaxEatingSlotsCount(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 33, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117CF RID: 71631 RVA: 0x00680BB0 File Offset: 0x0067EDB0
			public static void MerchantHasNewGoods(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 34, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117D0 RID: 71632 RVA: 0x00680BDB File Offset: 0x0067EDDB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.AddKidnappedCharacter instead.", true)]
			public static void AddKidnappedCharacter(IAsyncMethodRequestHandler requestHandler, int charId, int kidnappedCharId, ItemKey ropeItemKey, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117D1 RID: 71633 RVA: 0x00680BE3 File Offset: 0x0067EDE3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.TransferKidnappedCharacters instead.", true)]
			public static void TransferKidnappedCharacters(IAsyncMethodRequestHandler requestHandler, int targetKidnapperId, int sourceKidnapperId, KidnappedCharacterList kidnappedCharsToTransfer, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117D2 RID: 71634 RVA: 0x00680BEB File Offset: 0x0067EDEB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.ChangeKidnappedCharacterRope instead.", true)]
			public static void ChangeKidnappedCharacterRope(IAsyncMethodRequestHandler requestHandler, int kidnapperId, int kidnappedCharId, ItemKey newRopeKey, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117D3 RID: 71635 RVA: 0x00680BF4 File Offset: 0x0067EDF4
			public static void GetKidnapMaxSlotCount(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 38, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117D4 RID: 71636 RVA: 0x00680C1F File Offset: 0x0067EE1F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.TransferKidnappedCharacter instead.", true)]
			public static void TransferKidnappedCharacter(IAsyncMethodRequestHandler requestHandler, int targetKidnapperId, int sourceKidnapperId, KidnappedCharacter kidnappedCharData, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117D5 RID: 71637 RVA: 0x00680C27 File Offset: 0x0067EE27
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.RemoveKidnappedCharacter instead.", true)]
			public static void RemoveKidnappedCharacter(IAsyncMethodRequestHandler requestHandler, int kidnappedCharId, int kidnapperId, bool isEscaped, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117D6 RID: 71638 RVA: 0x00680C30 File Offset: 0x0067EE30
			public static void GetDisplayingAge(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 41, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117D7 RID: 71639 RVA: 0x00680C5C File Offset: 0x0067EE5C
			public static void GetMainAttributesRecoveries(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 42, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117D8 RID: 71640 RVA: 0x00680C88 File Offset: 0x0067EE88
			public static void GetInscriptionStatus(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 43, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117D9 RID: 71641 RVA: 0x00680CB4 File Offset: 0x0067EEB4
			public static void GetCharacterBirthDate(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 44, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117DA RID: 71642 RVA: 0x00680CE0 File Offset: 0x0067EEE0
			public static void GetMaxWorthCanBeLentToTaiwu(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 45, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117DB RID: 71643 RVA: 0x00680D0C File Offset: 0x0067EF0C
			public static void CheckFavorabilityBeforeTransferring(IAsyncMethodRequestHandler requestHandler, int charId, ResourceInts resources, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ResourceInts>(4, 46, charId, resources, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117DC RID: 71644 RVA: 0x00680D38 File Offset: 0x0067EF38
			public static void GetClothingDisplayId(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 47, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117DD RID: 71645 RVA: 0x00680D64 File Offset: 0x0067EF64
			public static void GetCharacterDisplayDataList(IAsyncMethodRequestHandler requestHandler, List<int> charIdList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(4, 48, charIdList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117DE RID: 71646 RVA: 0x00680D90 File Offset: 0x0067EF90
			public static void GetCharacterLifeSkillAttainmentList(IAsyncMethodRequestHandler requestHandler, List<int> charIdList, sbyte lifeSkillType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>, sbyte>(4, 49, charIdList, lifeSkillType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117DF RID: 71647 RVA: 0x00680DBC File Offset: 0x0067EFBC
			public static void GetTaiwuRelatedGraveDisplayDataList(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(4, 50, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117E0 RID: 71648 RVA: 0x00680DE8 File Offset: 0x0067EFE8
			public static void GetGraveDisplayDataList(IAsyncMethodRequestHandler requestHandler, List<int> charIdList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(4, 51, charIdList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117E1 RID: 71649 RVA: 0x00680E14 File Offset: 0x0067F014
			public static void GetCharacterDisplayDataListForRelations(IAsyncMethodRequestHandler requestHandler, List<int> charIds, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(4, 52, charIds, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117E2 RID: 71650 RVA: 0x00680E40 File Offset: 0x0067F040
			public static void GetSomeoneKidnapCharacters(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 53, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117E3 RID: 71651 RVA: 0x00680E6C File Offset: 0x0067F06C
			public static void GetCharacterAttributeDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 54, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117E4 RID: 71652 RVA: 0x00680E98 File Offset: 0x0067F098
			public static void GetCharacterSamsaraData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 55, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117E5 RID: 71653 RVA: 0x00680EC4 File Offset: 0x0067F0C4
			public static void GetEquipmentCompareData(IAsyncMethodRequestHandler requestHandler, int charId, sbyte srcSlot, sbyte destSlot, ItemKey srcItemKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, sbyte, sbyte, ItemKey>(4, 56, charId, srcSlot, destSlot, srcItemKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117E6 RID: 71654 RVA: 0x00680EF4 File Offset: 0x0067F0F4
			public static void GetGroupCharDisplayDataList(IAsyncMethodRequestHandler requestHandler, List<int> charIdList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(4, 57, charIdList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117E7 RID: 71655 RVA: 0x00680F20 File Offset: 0x0067F120
			public static void GetDefeatMarkCountList(IAsyncMethodRequestHandler requestHandler, List<int> charIdList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(4, 58, charIdList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117E8 RID: 71656 RVA: 0x00680F4C File Offset: 0x0067F14C
			public static void GetFeatureMedalValue(IAsyncMethodRequestHandler requestHandler, int charId, sbyte medalType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, sbyte>(4, 59, charId, medalType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117E9 RID: 71657 RVA: 0x00680F78 File Offset: 0x0067F178
			public static void GetFeatureMedalValueList(IAsyncMethodRequestHandler requestHandler, List<int> charIdList, sbyte medalType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>, sbyte>(4, 60, charIdList, medalType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117EA RID: 71658 RVA: 0x00680FA4 File Offset: 0x0067F1A4
			public static void GetFixedCharacterIdByTemplateId(IAsyncMethodRequestHandler requestHandler, short templateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(4, 61, templateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117EB RID: 71659 RVA: 0x00680FCF File Offset: 0x0067F1CF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_SimulateNpcCombat instead.", true)]
			public static void GmCmd_SimulateNpcCombat(IAsyncMethodRequestHandler requestHandler, int charIdA, int charIdB, sbyte combatType, int killBaseChance, int kidnapBaseChance, int releaseBaseChance, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117EC RID: 71660 RVA: 0x00680FD8 File Offset: 0x0067F1D8
			public static void GmCmd_GetAllCharacterName(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(4, 63, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117ED RID: 71661 RVA: 0x00681002 File Offset: 0x0067F202
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_ChangeFavorability instead.", true)]
			public static void GmCmd_ChangeFavorability(IAsyncMethodRequestHandler requestHandler, int selfCharId, int relatedCharId, short delta, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117EE RID: 71662 RVA: 0x0068100A File Offset: 0x0067F20A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_ClearFameActionRecords instead.", true)]
			public static void GmCmd_ClearFameActionRecords(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117EF RID: 71663 RVA: 0x00681012 File Offset: 0x0067F212
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_RecordFameAction instead.", true)]
			public static void GmCmd_RecordFameAction(IAsyncMethodRequestHandler requestHandler, int charId, short fameActionId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117F0 RID: 71664 RVA: 0x0068101A File Offset: 0x0067F21A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_RecordFameAction instead.", true)]
			public static void GmCmd_RecordFameAction(IAsyncMethodRequestHandler requestHandler, int charId, short fameActionId, int targetCharId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117F1 RID: 71665 RVA: 0x00681022 File Offset: 0x0067F222
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_RecordFameAction instead.", true)]
			public static void GmCmd_RecordFameAction(IAsyncMethodRequestHandler requestHandler, int charId, short fameActionId, int targetCharId, short fameMultiplier, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117F2 RID: 71666 RVA: 0x0068102A File Offset: 0x0067F22A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_CreateRandomIntelligentCharacters instead.", true)]
			public static void GmCmd_CreateRandomIntelligentCharacters(IAsyncMethodRequestHandler requestHandler, int charCount, sbyte orgTemplateId, bool createHere, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117F3 RID: 71667 RVA: 0x00681032 File Offset: 0x0067F232
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_ForceChangeOrganization instead.", true)]
			public static void GmCmd_ForceChangeOrganization(IAsyncMethodRequestHandler requestHandler, int charId, sbyte orgTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117F4 RID: 71668 RVA: 0x0068103C File Offset: 0x0067F23C
			public static void GmCmd_ForceChangeGrade(IAsyncMethodRequestHandler requestHandler, int charId, sbyte grade, bool principal, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, sbyte, bool>(4, 69, charId, grade, principal, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117F5 RID: 71669 RVA: 0x0068106A File Offset: 0x0067F26A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_Die instead.", true)]
			public static void GmCmd_Die(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117F6 RID: 71670 RVA: 0x00681074 File Offset: 0x0067F274
			public static void GmCmd_GetAliveCharByPreexistenceChar(IAsyncMethodRequestHandler requestHandler, int preexistenceCharId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 71, preexistenceCharId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117F7 RID: 71671 RVA: 0x0068109F File Offset: 0x0067F29F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_LogCharacterSamsaraInfo instead.", true)]
			public static void GmCmd_LogCharacterSamsaraInfo(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117F8 RID: 71672 RVA: 0x006810A7 File Offset: 0x0067F2A7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_EditExtraNeiliAllocation instead.", true)]
			public static void GmCmd_EditExtraNeiliAllocation(IAsyncMethodRequestHandler requestHandler, int characterId, NeiliAllocation allocation, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117F9 RID: 71673 RVA: 0x006810AF File Offset: 0x0067F2AF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_MakeCharacterKidnapped instead.", true)]
			public static void GmCmd_MakeCharacterKidnapped(IAsyncMethodRequestHandler requestHandler, int characterId, int targetCharacterId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117FA RID: 71674 RVA: 0x006810B8 File Offset: 0x0067F2B8
			public static void GmCmd_MoveIntelligentCharacter(IAsyncMethodRequestHandler requestHandler, int charId, Location destLocation, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, Location>(4, 75, charId, destLocation, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117FB RID: 71675 RVA: 0x006810E4 File Offset: 0x0067F2E4
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_RandomizeRelationShipsInSettlement instead.", true)]
			public static void GmCmd_RandomizeRelationShipsInSettlement(IAsyncMethodRequestHandler requestHandler, sbyte orgTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117FC RID: 71676 RVA: 0x006810EC File Offset: 0x0067F2EC
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_MakeCharacterHaveSex instead.", true)]
			public static void GmCmd_MakeCharacterHaveSex(IAsyncMethodRequestHandler requestHandler, int selfCharId, int targetCharId, bool isRaped, int pregnantRemainTime, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060117FD RID: 71677 RVA: 0x006810F4 File Offset: 0x0067F2F4
			public static void GmCmd_GetCharacterPregnancyLockEndDates(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 78, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117FE RID: 71678 RVA: 0x00681120 File Offset: 0x0067F320
			public static void GmCmd_GetCharacterActualBloodParents(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 79, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060117FF RID: 71679 RVA: 0x0068114C File Offset: 0x0067F34C
			public static void CharacterShaveAvatar(IAsyncMethodRequestHandler requestHandler, int cutterCharId, int shaveCharId, AvatarData shaveResult, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int, AvatarData>(4, 80, cutterCharId, shaveCharId, shaveResult, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011800 RID: 71680 RVA: 0x0068117A File Offset: 0x0067F37A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.AllocateNeili instead.", true)]
			public static void AllocateNeili(IAsyncMethodRequestHandler requestHandler, int charId, byte neiliAllocationType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011801 RID: 71681 RVA: 0x00681182 File Offset: 0x0067F382
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.DeallocateNeili instead.", true)]
			public static void DeallocateNeili(IAsyncMethodRequestHandler requestHandler, int charId, byte neiliAllocationType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011802 RID: 71682 RVA: 0x0068118C File Offset: 0x0067F38C
			public static void GetChangeOfQiDisorder(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 83, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011803 RID: 71683 RVA: 0x006811B8 File Offset: 0x0067F3B8
			public static void GetUsableCombatResources(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 84, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011804 RID: 71684 RVA: 0x006811E4 File Offset: 0x0067F3E4
			public static void GetCombatSkillSlotCounts(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 85, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011805 RID: 71685 RVA: 0x0068120F File Offset: 0x0067F40F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.SetCombatSkillSlot instead.", true)]
			public static void SetCombatSkillSlot(IAsyncMethodRequestHandler requestHandler, int charId, sbyte equipType, int index, short skillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011806 RID: 71686 RVA: 0x00681218 File Offset: 0x0067F418
			public static void GetCombatSkillAttainment(IAsyncMethodRequestHandler requestHandler, int charId, sbyte type, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, sbyte>(4, 87, charId, type, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011807 RID: 71687 RVA: 0x00681244 File Offset: 0x0067F444
			public static void GetAllCombatSkillAttainment(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 88, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011808 RID: 71688 RVA: 0x00681270 File Offset: 0x0067F470
			public static void GetLifeSkillAttainment(IAsyncMethodRequestHandler requestHandler, int charId, sbyte type, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, sbyte>(4, 89, charId, type, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011809 RID: 71689 RVA: 0x0068129C File Offset: 0x0067F49C
			public static void GetAllLifeSkillAttainment(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 90, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601180A RID: 71690 RVA: 0x006812C7 File Offset: 0x0067F4C7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.LearnCombatSkill instead.", true)]
			public static void LearnCombatSkill(IAsyncMethodRequestHandler requestHandler, int charId, short skillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601180B RID: 71691 RVA: 0x006812CF File Offset: 0x0067F4CF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.LearnCombatSkill instead.", true)]
			public static void LearnCombatSkill(IAsyncMethodRequestHandler requestHandler, int charId, short skillTemplateId, ushort readingState, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601180C RID: 71692 RVA: 0x006812D7 File Offset: 0x0067F4D7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.LearnLifeSkill instead.", true)]
			public static void LearnLifeSkill(IAsyncMethodRequestHandler requestHandler, int charId, short skillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601180D RID: 71693 RVA: 0x006812DF File Offset: 0x0067F4DF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.LearnLifeSkill instead.", true)]
			public static void LearnLifeSkill(IAsyncMethodRequestHandler requestHandler, int charId, short skillTemplateId, byte readingState, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601180E RID: 71694 RVA: 0x006812E8 File Offset: 0x0067F4E8
			public static void TryGetDeadCharacter(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 93, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601180F RID: 71695 RVA: 0x00681314 File Offset: 0x0067F514
			public static void GetTitles(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 94, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011810 RID: 71696 RVA: 0x00681340 File Offset: 0x0067F540
			public static void GetHighestGradeCombatSkillById(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 95, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011811 RID: 71697 RVA: 0x0068136C File Offset: 0x0067F56C
			public static void GetLeftMaxHealth(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 96, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011812 RID: 71698 RVA: 0x00681398 File Offset: 0x0067F598
			public static void GetHealthRecovery(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 97, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011813 RID: 71699 RVA: 0x006813C4 File Offset: 0x0067F5C4
			public static void GetAvatarRelatedDataList(IAsyncMethodRequestHandler requestHandler, List<int> charIdList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(4, 98, charIdList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011814 RID: 71700 RVA: 0x006813F0 File Offset: 0x0067F5F0
			public static void GetAvatarData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 99, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011815 RID: 71701 RVA: 0x0068141B File Offset: 0x0067F61B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.TransferInventoryItemListWithDebt instead.", true)]
			public static void TransferInventoryItemListWithDebt(IAsyncMethodRequestHandler requestHandler, int srcCharId, int destCharId, List<ItemKey> keyList, bool checkFavorability, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011816 RID: 71702 RVA: 0x00681424 File Offset: 0x0067F624
			public static void GetFameType(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 101, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011817 RID: 71703 RVA: 0x00681450 File Offset: 0x0067F650
			public static void GetAvatarRelatedData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 102, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011818 RID: 71704 RVA: 0x0068147C File Offset: 0x0067F67C
			public static void GetCharacterListWisdomCount(IAsyncMethodRequestHandler requestHandler, List<int> charIdList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(4, 103, charIdList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011819 RID: 71705 RVA: 0x006814A8 File Offset: 0x0067F6A8
			public static void SortCharacterListByMaxCombatSkill(IAsyncMethodRequestHandler requestHandler, List<int> managerList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(4, 104, managerList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601181A RID: 71706 RVA: 0x006814D4 File Offset: 0x0067F6D4
			public static void GetCharacterMaxCombatSkillAttainment(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 105, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601181B RID: 71707 RVA: 0x00681500 File Offset: 0x0067F700
			public static void GetItemPowerInfo(IAsyncMethodRequestHandler requestHandler, int charId, ItemKey itemKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ItemKey>(4, 106, charId, itemKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601181C RID: 71708 RVA: 0x0068152C File Offset: 0x0067F72C
			public static void CalcMaxFavorabilityToTaiwuById(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 107, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601181D RID: 71709 RVA: 0x00681558 File Offset: 0x0067F758
			public static void CheckDebtChange(IAsyncMethodRequestHandler requestHandler, int charId, ResourceInts resources, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ResourceInts>(4, 108, charId, resources, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601181E RID: 71710 RVA: 0x00681584 File Offset: 0x0067F784
			public static void GetCharacterWisdomCountById(IAsyncMethodRequestHandler requestHandler, int characterId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 109, characterId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601181F RID: 71711 RVA: 0x006815AF File Offset: 0x0067F7AF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.TransferInventoryItemFromAToB instead.", true)]
			public static void TransferInventoryItemFromAToB(IAsyncMethodRequestHandler requestHandler, int charA, int charB, ItemKey itemKey, int amount, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011820 RID: 71712 RVA: 0x006815B7 File Offset: 0x0067F7B7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_SetCurReadingEvent instead.", true)]
			public static void GmCmd_SetCurReadingEvent(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011821 RID: 71713 RVA: 0x006815BF File Offset: 0x0067F7BF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_AddFeature instead.", true)]
			public static void GmCmd_AddFeature(IAsyncMethodRequestHandler requestHandler, int charId, short templateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011822 RID: 71714 RVA: 0x006815C7 File Offset: 0x0067F7C7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_SetFeatures instead.", true)]
			public static void GmCmd_SetFeatures(IAsyncMethodRequestHandler requestHandler, int charId, List<short> features, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011823 RID: 71715 RVA: 0x006815CF File Offset: 0x0067F7CF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_RemoveFeature instead.", true)]
			public static void GmCmd_RemoveFeature(IAsyncMethodRequestHandler requestHandler, int charId, short templateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011824 RID: 71716 RVA: 0x006815D7 File Offset: 0x0067F7D7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_RemoveRelation instead.", true)]
			public static void GmCmd_RemoveRelation(IAsyncMethodRequestHandler requestHandler, int charId, int relatedCharId, ushort removeType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011825 RID: 71717 RVA: 0x006815E0 File Offset: 0x0067F7E0
			public static void IsReclusive(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 116, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011826 RID: 71718 RVA: 0x0068160C File Offset: 0x0067F80C
			public static void GetInventoryEquipment(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 117, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011827 RID: 71719 RVA: 0x00681638 File Offset: 0x0067F838
			public static void GetFilteredCharacterCounts(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(4, 118, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011828 RID: 71720 RVA: 0x00681662 File Offset: 0x0067F862
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.ClearCharacterSortFilter instead.", true)]
			public static void ClearCharacterSortFilter(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011829 RID: 71721 RVA: 0x0068166C File Offset: 0x0067F86C
			public static void UpdateSortFilterSettings(IAsyncMethodRequestHandler requestHandler, CharacterSortFilterSettings sortFilterSettings, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<CharacterSortFilterSettings>(4, 120, sortFilterSettings, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601182A RID: 71722 RVA: 0x00681698 File Offset: 0x0067F898
			public static void InitializeCharacterSortFilter(IAsyncMethodRequestHandler requestHandler, CharacterSortFilterSettings sortFilterSettings, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<CharacterSortFilterSettings>(4, 121, sortFilterSettings, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601182B RID: 71723 RVA: 0x006816C4 File Offset: 0x0067F8C4
			public static void FindNameInCurrentSortFilter(IAsyncMethodRequestHandler requestHandler, string name, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<string>(4, 122, name, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601182C RID: 71724 RVA: 0x006816F0 File Offset: 0x0067F8F0
			public static void GetCharacterDisplayDataListForUltimateSelect(IAsyncMethodRequestHandler requestHandler, List<int> charIdList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(4, 123, charIdList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601182D RID: 71725 RVA: 0x0068171C File Offset: 0x0067F91C
			public static void GetMaxSortingTypeCharIds(IAsyncMethodRequestHandler requestHandler, List<int> sortingTypes, sbyte filterSubId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>, sbyte>(4, 124, sortingTypes, filterSubId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601182E RID: 71726 RVA: 0x00681748 File Offset: 0x0067F948
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_SetCurrNeili instead.", true)]
			public static void GmCmd_SetCurrNeili(IAsyncMethodRequestHandler requestHandler, int charId, int value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601182F RID: 71727 RVA: 0x00681750 File Offset: 0x0067F950
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_AddPoisonedInventoryItem instead.", true)]
			public static void GmCmd_AddPoisonedInventoryItem(IAsyncMethodRequestHandler requestHandler, int charId, sbyte baseItemType, short baseItemId, short poisonId1, short poisonId2, short poisonId3, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011830 RID: 71728 RVA: 0x00681758 File Offset: 0x0067F958
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_AddPoisonedEatingItem instead.", true)]
			public static void GmCmd_AddPoisonedEatingItem(IAsyncMethodRequestHandler requestHandler, int charId, sbyte baseItemType, short baseItemId, short poisonId1, short poisonId2, short poisonId3, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011831 RID: 71729 RVA: 0x00681760 File Offset: 0x0067F960
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_SetCharBaseNeiliProportionOfFiveElements instead.", true)]
			public static void GmCmd_SetCharBaseNeiliProportionOfFiveElements(IAsyncMethodRequestHandler requestHandler, int charId, NeiliProportionOfFiveElements fiveElements, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011832 RID: 71730 RVA: 0x00681768 File Offset: 0x0067F968
			public static void GetRelationBetweenCharacters(IAsyncMethodRequestHandler requestHandler, int charId, int relatedCharId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int>(4, 129, charId, relatedCharId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011833 RID: 71731 RVA: 0x00681798 File Offset: 0x0067F998
			public static void GetInventoryItemsByItemType(IAsyncMethodRequestHandler requestHandler, int charId, sbyte itemType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, sbyte>(4, 130, charId, itemType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011834 RID: 71732 RVA: 0x006817C8 File Offset: 0x0067F9C8
			public static void GetCharacterDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 131, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011835 RID: 71733 RVA: 0x006817F6 File Offset: 0x0067F9F6
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.UnequipAllCombatSkills instead.", true)]
			public static void UnequipAllCombatSkills(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011836 RID: 71734 RVA: 0x006817FE File Offset: 0x0067F9FE
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.AutoEquipCombatSkills instead.", true)]
			public static void AutoEquipCombatSkills(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011837 RID: 71735 RVA: 0x00681806 File Offset: 0x0067FA06
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.AutoEquipCombatSkills instead.", true)]
			public static void AutoEquipCombatSkills(IAsyncMethodRequestHandler requestHandler, int charId, short combatConfigTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011838 RID: 71736 RVA: 0x0068180E File Offset: 0x0067FA0E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_AddCharacterExtraTitle instead.", true)]
			public static void GmCmd_AddCharacterExtraTitle(IAsyncMethodRequestHandler requestHandler, int charId, short titleTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011839 RID: 71737 RVA: 0x00681816 File Offset: 0x0067FA16
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_AddCharacterExtraTitle instead.", true)]
			public static void GmCmd_AddCharacterExtraTitle(IAsyncMethodRequestHandler requestHandler, int charId, short titleTemplateId, int duration, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601183A RID: 71738 RVA: 0x0068181E File Offset: 0x0067FA1E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.TryAddAndApplyOneWayRelation instead.", true)]
			public static void TryAddAndApplyOneWayRelation(IAsyncMethodRequestHandler requestHandler, int charId, int relatedCharId, ushort relationType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601183B RID: 71739 RVA: 0x00681826 File Offset: 0x0067FA26
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.MoveFuyuHiltLocation instead.", true)]
			public static void MoveFuyuHiltLocation(IAsyncMethodRequestHandler requestHandler, Location targetLocation, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601183C RID: 71740 RVA: 0x0068182E File Offset: 0x0067FA2E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.MoveFuyuHiltLocation instead.", true)]
			public static void MoveFuyuHiltLocation(IAsyncMethodRequestHandler requestHandler, Location targetLocation, bool hide, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601183D RID: 71741 RVA: 0x00681836 File Offset: 0x0067FA36
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.TryRemoveOneWayRelation instead.", true)]
			public static void TryRemoveOneWayRelation(IAsyncMethodRequestHandler requestHandler, int charId, int relatedCharId, ushort relationType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601183E RID: 71742 RVA: 0x00681840 File Offset: 0x0067FA40
			public static void GetCharacterLoveAndHateItemInfo(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 138, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601183F RID: 71743 RVA: 0x00681870 File Offset: 0x0067FA70
			public static void IsTemporaryIntelligentCharacter(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 139, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011840 RID: 71744 RVA: 0x006818A0 File Offset: 0x0067FAA0
			public static void GetMixedPoisonTypeRelatedMarkCountArray(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 140, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011841 RID: 71745 RVA: 0x006818D0 File Offset: 0x0067FAD0
			public static void SimulateEatingEffect(IAsyncMethodRequestHandler requestHandler, int charId, ItemKey itemKey, int amount, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ItemKey, int>(4, 141, charId, itemKey, amount, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011842 RID: 71746 RVA: 0x00681904 File Offset: 0x0067FB04
			public static void GetCharacterDisplayDataForTooltip(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 142, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011843 RID: 71747 RVA: 0x00681932 File Offset: 0x0067FB32
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_SetCurLoopingEvent instead.", true)]
			public static void GmCmd_SetCurLoopingEvent(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011844 RID: 71748 RVA: 0x0068193C File Offset: 0x0067FB3C
			public static void GetHealthType(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 144, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011845 RID: 71749 RVA: 0x0068196C File Offset: 0x0067FB6C
			public static void GetCharacterLocationDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 145, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011846 RID: 71750 RVA: 0x0068199C File Offset: 0x0067FB9C
			public static void GetCharacterDisplayDataForMapBlock(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 146, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011847 RID: 71751 RVA: 0x006819CC File Offset: 0x0067FBCC
			public static void GetCharacterWisdomList(IAsyncMethodRequestHandler requestHandler, List<int> charIdList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(4, 147, charIdList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011848 RID: 71752 RVA: 0x006819FC File Offset: 0x0067FBFC
			public static void GetOrCreateSwordTombCharacterIdForNormalInformation(IAsyncMethodRequestHandler requestHandler, sbyte xiangshuAvatarId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(4, 148, xiangshuAvatarId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011849 RID: 71753 RVA: 0x00681A2C File Offset: 0x0067FC2C
			public static void GetAllInventoryItemsExcludeValueZero(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 149, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601184A RID: 71754 RVA: 0x00681A5C File Offset: 0x0067FC5C
			public static void GetAddConsummateLevelRequiredMonth(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 150, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601184B RID: 71755 RVA: 0x00681A8A File Offset: 0x0067FC8A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_ResetHairGrowth instead.", true)]
			public static void GmCmd_ResetHairGrowth(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601184C RID: 71756 RVA: 0x00681A94 File Offset: 0x0067FC94
			public static void GetAllItemsByAreaId(IAsyncMethodRequestHandler requestHandler, short areaId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(4, 152, areaId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601184D RID: 71757 RVA: 0x00681AC2 File Offset: 0x0067FCC2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.SetCombatSkillPlanLock instead.", true)]
			public static void SetCombatSkillPlanLock(IAsyncMethodRequestHandler requestHandler, int charId, bool isLocked, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601184E RID: 71758 RVA: 0x00681ACA File Offset: 0x0067FCCA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.AppendCombatSkillPlan instead.", true)]
			public static void AppendCombatSkillPlan(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601184F RID: 71759 RVA: 0x00681AD2 File Offset: 0x0067FCD2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.DuplicateCurrentCombatSkillPlan instead.", true)]
			public static void DuplicateCurrentCombatSkillPlan(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011850 RID: 71760 RVA: 0x00681ADA File Offset: 0x0067FCDA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.UpdateCombatSkillPlan instead.", true)]
			public static void UpdateCombatSkillPlan(IAsyncMethodRequestHandler requestHandler, int charId, int planId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011851 RID: 71761 RVA: 0x00681AE4 File Offset: 0x0067FCE4
			public static void GetCurrentPlanIdAndPlanCount(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 157, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011852 RID: 71762 RVA: 0x00681B12 File Offset: 0x0067FD12
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.DeleteCombatSkillPlan instead.", true)]
			public static void DeleteCombatSkillPlan(IAsyncMethodRequestHandler requestHandler, int charId, int planId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011853 RID: 71763 RVA: 0x00681B1C File Offset: 0x0067FD1C
			public static void IsInteractedWithTaiwu(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 159, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011854 RID: 71764 RVA: 0x00681B4C File Offset: 0x0067FD4C
			public static void IsNeiliAllocationLocked(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 160, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011855 RID: 71765 RVA: 0x00681B7A File Offset: 0x0067FD7A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.AllocateGenericGrid instead.", true)]
			public static void AllocateGenericGrid(IAsyncMethodRequestHandler requestHandler, int charId, sbyte equipType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011856 RID: 71766 RVA: 0x00681B82 File Offset: 0x0067FD82
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.DeallocateGenericGrid instead.", true)]
			public static void DeallocateGenericGrid(IAsyncMethodRequestHandler requestHandler, int charId, sbyte equipType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011857 RID: 71767 RVA: 0x00681B8C File Offset: 0x0067FD8C
			public static void IsCombatSkillEquipmentLocked(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 163, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011858 RID: 71768 RVA: 0x00681BBA File Offset: 0x0067FDBA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.AutoAllocateNeili instead.", true)]
			public static void AutoAllocateNeili(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011859 RID: 71769 RVA: 0x00681BC2 File Offset: 0x0067FDC2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.AutoSetCombatSkillAttainmentPanels instead.", true)]
			public static void AutoSetCombatSkillAttainmentPanels(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601185A RID: 71770 RVA: 0x00681BCA File Offset: 0x0067FDCA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.AutoEquipItems instead.", true)]
			public static void AutoEquipItems(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601185B RID: 71771 RVA: 0x00681BD2 File Offset: 0x0067FDD2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.SetCombatSkillAttainmentLock instead.", true)]
			public static void SetCombatSkillAttainmentLock(IAsyncMethodRequestHandler requestHandler, int charId, bool isLocked, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601185C RID: 71772 RVA: 0x00681BDC File Offset: 0x0067FDDC
			public static void IsCombatSkillAttainmentLocked(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 168, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601185D RID: 71773 RVA: 0x00681C0C File Offset: 0x0067FE0C
			public static void GetGenericGridAllocation(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 169, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601185E RID: 71774 RVA: 0x00681C3A File Offset: 0x0067FE3A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.SetNeiliAllocationLock instead.", true)]
			public static void SetNeiliAllocationLock(IAsyncMethodRequestHandler requestHandler, int charId, bool isLocked, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601185F RID: 71775 RVA: 0x00681C42 File Offset: 0x0067FE42
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.RemoveEquippedCombatSkill instead.", true)]
			public static void RemoveEquippedCombatSkill(IAsyncMethodRequestHandler requestHandler, int charId, short skillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011860 RID: 71776 RVA: 0x00681C4A File Offset: 0x0067FE4A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.AddEquippedCombatSkill instead.", true)]
			public static void AddEquippedCombatSkill(IAsyncMethodRequestHandler requestHandler, int charId, short skillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011861 RID: 71777 RVA: 0x00681C54 File Offset: 0x0067FE54
			public static void GetCombatSkillExtraSlotCounts(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 173, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011862 RID: 71778 RVA: 0x00681C84 File Offset: 0x0067FE84
			public static void GetCharacterAllBodyPartExists(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 174, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011863 RID: 71779 RVA: 0x00681CB2 File Offset: 0x0067FEB2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_ChangeCharDisorderOfQi instead.", true)]
			public static void GmCmd_ChangeCharDisorderOfQi(IAsyncMethodRequestHandler requestHandler, int charId, int delta, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011864 RID: 71780 RVA: 0x00681CBC File Offset: 0x0067FEBC
			public static void GenerateRandomName(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(4, 176, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011865 RID: 71781 RVA: 0x00681CEC File Offset: 0x0067FEEC
			public static void GetCharacterDisplayDataListForRelationsWithRelationType(IAsyncMethodRequestHandler requestHandler, int currCharId, List<int> charIds, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, List<int>>(4, 177, currCharId, charIds, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011866 RID: 71782 RVA: 0x00681D1C File Offset: 0x0067FF1C
			public static void GetPhysiologicalAge(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 178, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011867 RID: 71783 RVA: 0x00681D4C File Offset: 0x0067FF4C
			public static void GetPhysiologicalAgeAffector(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 179, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011868 RID: 71784 RVA: 0x00681D7C File Offset: 0x0067FF7C
			public static void GetKidnappedCharacterDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 180, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011869 RID: 71785 RVA: 0x00681DAC File Offset: 0x0067FFAC
			public static void IsCarrierDurabilityRunningOut(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 181, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601186A RID: 71786 RVA: 0x00681DDC File Offset: 0x0067FFDC
			public static void GmCmd_ForceChangeOrganizationByName(IAsyncMethodRequestHandler requestHandler, int charId, string settlementName, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, string>(4, 182, charId, settlementName, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601186B RID: 71787 RVA: 0x00681E0C File Offset: 0x0068000C
			public static void GetAvailableFeature(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 183, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601186C RID: 71788 RVA: 0x00681E3C File Offset: 0x0068003C
			public static void GetEquipmentCompare(IAsyncMethodRequestHandler requestHandler, int charId, ItemKey[] baselineEquipment, ItemKey[] targetEquipment, sbyte slotIndex, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ItemKey[], ItemKey[], sbyte>(4, 184, charId, baselineEquipment, targetEquipment, slotIndex, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601186D RID: 71789 RVA: 0x00681E70 File Offset: 0x00680070
			public static void GetCharacterTableDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, List<short> types, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, List<short>>(4, 185, charId, types, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601186E RID: 71790 RVA: 0x00681EA0 File Offset: 0x006800A0
			public static void GetCharacterTableDisplayDataList(IAsyncMethodRequestHandler requestHandler, List<int> charIds, List<short> types, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>, List<short>>(4, 186, charIds, types, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601186F RID: 71791 RVA: 0x00681ED0 File Offset: 0x006800D0
			public static void GetCharacterTableDisplayDataListWithNeedItem(IAsyncMethodRequestHandler requestHandler, List<short> types, ItemKey itemKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<short>, ItemKey>(4, 187, types, itemKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011870 RID: 71792 RVA: 0x00681F00 File Offset: 0x00680100
			public static void TryGetGraveDisplayDataList(IAsyncMethodRequestHandler requestHandler, List<int> charIdList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(4, 188, charIdList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011871 RID: 71793 RVA: 0x00681F30 File Offset: 0x00680130
			public static void GetDeadCharacterDisplayDataForTooltip(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 189, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011872 RID: 71794 RVA: 0x00681F5E File Offset: 0x0068015E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_DriveWugKing instead.", true)]
			public static void GmCmd_DriveWugKing(IAsyncMethodRequestHandler requestHandler, int charId, sbyte wugType, bool isPositive, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011873 RID: 71795 RVA: 0x00681F68 File Offset: 0x00680168
			public static void GetCharacterCurrentProfession(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 191, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011874 RID: 71796 RVA: 0x00681F96 File Offset: 0x00680196
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_SetCharacterCurrProfessionSeniority instead.", true)]
			public static void GmCmd_SetCharacterCurrProfessionSeniority(IAsyncMethodRequestHandler requestHandler, int charId, int professionId, int value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011875 RID: 71797 RVA: 0x00681FA0 File Offset: 0x006801A0
			public static void GetCharacterTemporaryFeaturesExpireDate(IAsyncMethodRequestHandler requestHandler, IntPair key, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<IntPair>(4, 193, key, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011876 RID: 71798 RVA: 0x00681FD0 File Offset: 0x006801D0
			public static void GetExtraNeiliAllocationProgress(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 194, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011877 RID: 71799 RVA: 0x00682000 File Offset: 0x00680200
			public static void GetCharacterMenuInfoDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 195, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011878 RID: 71800 RVA: 0x00682030 File Offset: 0x00680230
			public static void GetAttributeWithDelta(IAsyncMethodRequestHandler requestHandler, int charId, ItemKey equipKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ItemKey>(4, 196, charId, equipKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011879 RID: 71801 RVA: 0x00682060 File Offset: 0x00680260
			public static void GetAttributeWithDelta(IAsyncMethodRequestHandler requestHandler, int charId, ItemKey equipKey, int slot, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ItemKey, int>(4, 196, charId, equipKey, slot, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601187A RID: 71802 RVA: 0x00682094 File Offset: 0x00680294
			public static void GetAttributeDelta(IAsyncMethodRequestHandler requestHandler, int charId, ItemKey equipKey, int slot, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ItemKey, int>(4, 197, charId, equipKey, slot, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601187B RID: 71803 RVA: 0x006820C8 File Offset: 0x006802C8
			public static void GetCharacterMenuLifeSkillDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 198, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601187C RID: 71804 RVA: 0x006820F8 File Offset: 0x006802F8
			public static void GetEquipLoad(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 199, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601187D RID: 71805 RVA: 0x00682128 File Offset: 0x00680328
			public static void GetCharacterDisplayDataForNeiliPage(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 200, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601187E RID: 71806 RVA: 0x00682158 File Offset: 0x00680358
			public static void GetCharacterInjuryDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 201, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601187F RID: 71807 RVA: 0x00682188 File Offset: 0x00680388
			public static void GetCharacterMenuAttainmentDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 202, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011880 RID: 71808 RVA: 0x006821B8 File Offset: 0x006803B8
			public static void GetKidnapMenuDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 203, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011881 RID: 71809 RVA: 0x006821E8 File Offset: 0x006803E8
			public static void GetPersonalities(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 204, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011882 RID: 71810 RVA: 0x00682218 File Offset: 0x00680418
			public static void GetCharacterDisplayDataForPractice(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 205, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011883 RID: 71811 RVA: 0x00682248 File Offset: 0x00680448
			public static void GetCharacterUsingMedicineDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 206, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011884 RID: 71812 RVA: 0x00682278 File Offset: 0x00680478
			public static void GetCharacterItemsDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 207, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011885 RID: 71813 RVA: 0x006822A8 File Offset: 0x006804A8
			public static void GetViewCharacterMenuDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, List<int> charIdList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, List<int>>(4, 208, charId, charIdList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011886 RID: 71814 RVA: 0x006822D8 File Offset: 0x006804D8
			public static void GetCharacterDisplayDataForGeneralScrollListBatch(IAsyncMethodRequestHandler requestHandler, List<int> charIdList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(4, 209, charIdList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011887 RID: 71815 RVA: 0x00682308 File Offset: 0x00680508
			public static void GetYuanshanSelectDataList(IAsyncMethodRequestHandler requestHandler, List<int> charIds, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(4, 210, charIds, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011888 RID: 71816 RVA: 0x00682338 File Offset: 0x00680538
			public static void GetVillagerCharDisplayDataList(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(4, 211, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011889 RID: 71817 RVA: 0x00682368 File Offset: 0x00680568
			public static void GetCharacterDisplayDataForBaihuaLifeLink(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(4, 212, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601188A RID: 71818 RVA: 0x00682398 File Offset: 0x00680598
			public static void GetAlertnessValue(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 213, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601188B RID: 71819 RVA: 0x006823C6 File Offset: 0x006805C6
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.SetAlertnessValue instead.", true)]
			public static void SetAlertnessValue(IAsyncMethodRequestHandler requestHandler, int charId, int value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601188C RID: 71820 RVA: 0x006823D0 File Offset: 0x006805D0
			public static void GetAlertnessData(IAsyncMethodRequestHandler requestHandler, int charId, bool init, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, bool>(4, 215, charId, init, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601188D RID: 71821 RVA: 0x00682400 File Offset: 0x00680600
			public static void GetTransferItemPreviewDisplayData(IAsyncMethodRequestHandler requestHandler, int characterId, Inventory items, bool isGive, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, Inventory, bool>(4, 216, characterId, items, isGive, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601188E RID: 71822 RVA: 0x00682434 File Offset: 0x00680634
			public static void GetAllRanshanReadBooksData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 217, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601188F RID: 71823 RVA: 0x00682464 File Offset: 0x00680664
			public static void GetCharacterOverviewEatingDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 218, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011890 RID: 71824 RVA: 0x00682494 File Offset: 0x00680694
			public static void GetEquipmentKeys(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 219, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011891 RID: 71825 RVA: 0x006824C4 File Offset: 0x006806C4
			public static void GetCharDisplayDataListAsVillager(IAsyncMethodRequestHandler requestHandler, List<int> charIds, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(4, 220, charIds, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011892 RID: 71826 RVA: 0x006824F4 File Offset: 0x006806F4
			public static void GetPreviewLeftMaxHealth(IAsyncMethodRequestHandler requestHandler, int charId, short baseMaxHealthOffset, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short>(4, 221, charId, baseMaxHealthOffset, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011893 RID: 71827 RVA: 0x00682524 File Offset: 0x00680724
			public static void GetGraveDisplayDataListForSelection(IAsyncMethodRequestHandler requestHandler, List<int> charIdList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(4, 222, charIdList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011894 RID: 71828 RVA: 0x00682554 File Offset: 0x00680754
			public static void GetCharacterDisplayDataForBeggarUltimate(IAsyncMethodRequestHandler requestHandler, List<int> charIdList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(4, 223, charIdList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011895 RID: 71829 RVA: 0x00682584 File Offset: 0x00680784
			public static void GetAvatarRelatedDataListIncludeDead(IAsyncMethodRequestHandler requestHandler, List<int> charIdList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(4, 224, charIdList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011896 RID: 71830 RVA: 0x006825B4 File Offset: 0x006807B4
			public static void GetFixedCharacterName(IAsyncMethodRequestHandler requestHandler, short templateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(4, 225, templateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011897 RID: 71831 RVA: 0x006825E4 File Offset: 0x006807E4
			public static void GetCharacterDisplayDataForGuard(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 226, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011898 RID: 71832 RVA: 0x00682614 File Offset: 0x00680814
			public static void SimulateProfessionDoctorSkill0(IAsyncMethodRequestHandler requestHandler, int patientId, ItemKey itemKey, int amount, int doctorId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ItemKey, int, int>(4, 227, patientId, itemKey, amount, doctorId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011899 RID: 71833 RVA: 0x00682648 File Offset: 0x00680848
			public static void GetCharacterDivinePower(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 228, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601189A RID: 71834 RVA: 0x00682678 File Offset: 0x00680878
			public static void GetCharacterGhostTechnique(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 229, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601189B RID: 71835 RVA: 0x006826A8 File Offset: 0x006808A8
			public static void PreviewAllocateNeili(IAsyncMethodRequestHandler requestHandler, int charId, byte neiliAllocationType, bool isAdd, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, byte, bool>(4, 230, charId, neiliAllocationType, isAdd, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601189C RID: 71836 RVA: 0x006826DC File Offset: 0x006808DC
			public static void GetCharacterDisplayDataForTasterUltimate(IAsyncMethodRequestHandler requestHandler, List<ItemKey> bookItemKeyList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<ItemKey>>(4, 231, bookItemKeyList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601189D RID: 71837 RVA: 0x0068270A File Offset: 0x0068090A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_ChangeXiangshuInfection instead.", true)]
			public static void GmCmd_ChangeXiangshuInfection(IAsyncMethodRequestHandler requestHandler, int charId, int delta, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601189E RID: 71838 RVA: 0x00682712 File Offset: 0x00680912
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.TransferInventoryItemInventoryWithDebt instead.", true)]
			public static void TransferInventoryItemInventoryWithDebt(IAsyncMethodRequestHandler requestHandler, int srcCharId, int destCharId, Inventory inventory, bool checkFavorability, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601189F RID: 71839 RVA: 0x0068271C File Offset: 0x0068091C
			public static void GetActionPlanningDsiplayData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 234, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060118A0 RID: 71840 RVA: 0x0068274C File Offset: 0x0068094C
			public static void GetCharacterProfessionList(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 235, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060118A1 RID: 71841 RVA: 0x0068277C File Offset: 0x0068097C
			public static void GetCarrierMaxProperty(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(4, 236, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060118A2 RID: 71842 RVA: 0x006827AA File Offset: 0x006809AA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_ClearCharacterActionPlanningData instead.", true)]
			public static void GmCmd_ClearCharacterActionPlanningData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060118A3 RID: 71843 RVA: 0x006827B2 File Offset: 0x006809B2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CharacterDomainMethod.Call.GmCmd_ClearAllActionPlanningData instead.", true)]
			public static void GmCmd_ClearAllActionPlanningData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}
		}
	}
}
