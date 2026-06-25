using System;
using System.Collections.Generic;
using GameData.DLC.FiveLoong;
using GameData.Domains.Building;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Profession;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Utilities;

namespace GameData.Domains.Extra
{
	// Token: 0x02000FCA RID: 4042
	public static class ExtraDomainMethod
	{
		// Token: 0x0200260F RID: 9743
		public static class Call
		{
			// Token: 0x06011366 RID: 70502 RVA: 0x0067B29E File Offset: 0x0067949E
			public static void SetCombatSkillOrderPlan(ShortList plan)
			{
				GameDataBridge.AddMethodCall<ShortList>(-1, 19, 0, plan);
			}

			// Token: 0x06011367 RID: 70503 RVA: 0x0067B2AC File Offset: 0x006794AC
			public static void AddLocationMark(Location location)
			{
				GameDataBridge.AddMethodCall<Location>(-1, 19, 1, location);
			}

			// Token: 0x06011368 RID: 70504 RVA: 0x0067B2BA File Offset: 0x006794BA
			public static void RemoveLocationMark(Location location)
			{
				GameDataBridge.AddMethodCall<Location>(-1, 19, 2, location);
			}

			// Token: 0x06011369 RID: 70505 RVA: 0x0067B2C8 File Offset: 0x006794C8
			public static void AddReadingEventBookId(int listenerId, int id)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 3, id);
			}

			// Token: 0x0601136A RID: 70506 RVA: 0x0067B2D6 File Offset: 0x006794D6
			public static void RemoveReadingEventBookId(int id, short templateId)
			{
				GameDataBridge.AddMethodCall<int, short>(-1, 19, 4, id, templateId);
			}

			// Token: 0x0601136B RID: 70507 RVA: 0x0067B2E5 File Offset: 0x006794E5
			public static void GetAllLifeSkillCombatUsedCard(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 5);
			}

			// Token: 0x0601136C RID: 70508 RVA: 0x0067B2F2 File Offset: 0x006794F2
			public static void GetAllLifeSkillCombatCard(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 6);
			}

			// Token: 0x0601136D RID: 70509 RVA: 0x0067B2FF File Offset: 0x006794FF
			public static void SetLifeSkillCombatUsedCard(sbyte skillType, LifeSkillCombatCardCollection collection)
			{
				GameDataBridge.AddMethodCall<sbyte, LifeSkillCombatCardCollection>(-1, 19, 7, skillType, collection);
			}

			// Token: 0x0601136E RID: 70510 RVA: 0x0067B30E File Offset: 0x0067950E
			public static void GetCharacterLifeSkillCombatUsedCard(int listenerId, sbyte skillType, int charId)
			{
				GameDataBridge.AddMethodCall<sbyte, int>(listenerId, 19, 8, skillType, charId);
			}

			// Token: 0x0601136F RID: 70511 RVA: 0x0067B31D File Offset: 0x0067951D
			public static void GetLifeSkillCombatUsedCard(int listenerId, sbyte skillType)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 19, 9, skillType);
			}

			// Token: 0x06011370 RID: 70512 RVA: 0x0067B32C File Offset: 0x0067952C
			public static void GetAllLifeSkillCombatNewCard(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 10);
			}

			// Token: 0x06011371 RID: 70513 RVA: 0x0067B33A File Offset: 0x0067953A
			public static void SetLifeSkillCombatCardNotNew(sbyte skillType, sbyte cardId)
			{
				GameDataBridge.AddMethodCall<sbyte, sbyte>(-1, 19, 11, skillType, cardId);
			}

			// Token: 0x06011372 RID: 70514 RVA: 0x0067B34A File Offset: 0x0067954A
			public static void GetCharTeammateCommands(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 12, charId);
			}

			// Token: 0x06011373 RID: 70515 RVA: 0x0067B359 File Offset: 0x00679559
			public static void SetLegendaryBookWeaponSlot(sbyte skillType, ItemKey weaponKey)
			{
				GameDataBridge.AddMethodCall<sbyte, ItemKey>(-1, 19, 13, skillType, weaponKey);
			}

			// Token: 0x06011374 RID: 70516 RVA: 0x0067B369 File Offset: 0x00679569
			public static void SetLegendaryBookSkillSlot(sbyte skillType, int index, short skillTemplateId)
			{
				GameDataBridge.AddMethodCall<sbyte, int, short>(-1, 19, 14, skillType, index, skillTemplateId);
			}

			// Token: 0x06011375 RID: 70517 RVA: 0x0067B37A File Offset: 0x0067957A
			public static void UnlockLegendaryBookBreakPlate(sbyte skillType, bool isYin)
			{
				GameDataBridge.AddMethodCall<sbyte, bool>(-1, 19, 15, skillType, isYin);
			}

			// Token: 0x06011376 RID: 70518 RVA: 0x0067B38A File Offset: 0x0067958A
			public static void UnlockLegendaryBookBonus(sbyte skillType, bool isYin)
			{
				GameDataBridge.AddMethodCall<sbyte, bool>(-1, 19, 16, skillType, isYin);
			}

			// Token: 0x06011377 RID: 70519 RVA: 0x0067B39A File Offset: 0x0067959A
			public static void EnterUnlockBreakPlateCombat(sbyte skillType, bool isYin)
			{
				GameDataBridge.AddMethodCall<sbyte, bool>(-1, 19, 17, skillType, isYin);
			}

			// Token: 0x06011378 RID: 70520 RVA: 0x0067B3AA File Offset: 0x006795AA
			public static void ExecuteActiveProfessionSkill(int professionId, int skillIndex)
			{
				GameDataBridge.AddMethodCall<int, int>(-1, 19, 18, professionId, skillIndex);
			}

			// Token: 0x06011379 RID: 70521 RVA: 0x0067B3BA File Offset: 0x006795BA
			public static void IsProfessionalSkillUnlocked(int listenerId, int professionId, int skillIndex)
			{
				GameDataBridge.AddMethodCall<int, int>(listenerId, 19, 19, professionId, skillIndex);
			}

			// Token: 0x0601137A RID: 70522 RVA: 0x0067B3CA File Offset: 0x006795CA
			public static void CanExecuteProfessionSkill(int listenerId, int professionId, int skillIndex)
			{
				GameDataBridge.AddMethodCall<int, int>(listenerId, 19, 20, professionId, skillIndex);
			}

			// Token: 0x0601137B RID: 70523 RVA: 0x0067B3DA File Offset: 0x006795DA
			public static void SetProfessionTestSetting(bool noSkillCooldown, bool noSkillCost)
			{
				GameDataBridge.AddMethodCall<bool, bool>(-1, 19, 21, noSkillCooldown, noSkillCost);
			}

			// Token: 0x0601137C RID: 70524 RVA: 0x0067B3EA File Offset: 0x006795EA
			public static void GetCharacterCustomDisplayName(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 22, charId);
			}

			// Token: 0x0601137D RID: 70525 RVA: 0x0067B3F9 File Offset: 0x006795F9
			public static void GetTianJieFuLuCount(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 23);
			}

			// Token: 0x0601137E RID: 70526 RVA: 0x0067B407 File Offset: 0x00679607
			public static void GmCmd_GenerateTreasure()
			{
				GameDataBridge.AddMethodCall(-1, 19, 24);
			}

			// Token: 0x0601137F RID: 70527 RVA: 0x0067B415 File Offset: 0x00679615
			public static void FindTreasure(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 25, charId);
			}

			// Token: 0x06011380 RID: 70528 RVA: 0x0067B424 File Offset: 0x00679624
			public static void CheckSpecialCondition(int listenerId, int professionId, int skillIndex)
			{
				GameDataBridge.AddMethodCall<int, int>(listenerId, 19, 26, professionId, skillIndex);
			}

			// Token: 0x06011381 RID: 70529 RVA: 0x0067B434 File Offset: 0x00679634
			public static void ConfirmExecuteSkill(ProfessionSkillArg professionSkillArg, bool isFinished)
			{
				GameDataBridge.AddMethodCall<ProfessionSkillArg, bool>(-1, 19, 27, professionSkillArg, isFinished);
			}

			// Token: 0x06011382 RID: 70530 RVA: 0x0067B444 File Offset: 0x00679644
			public static void FindTreasureExpect(int listenerId, Location location)
			{
				GameDataBridge.AddMethodCall<Location>(listenerId, 19, 28, location);
			}

			// Token: 0x06011383 RID: 70531 RVA: 0x0067B453 File Offset: 0x00679653
			public static void UnlockAllProfessionSkills(bool maxSeniority)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 19, 29, maxSeniority);
			}

			// Token: 0x06011384 RID: 70532 RVA: 0x0067B462 File Offset: 0x00679662
			public static void SetProfessionSeniorityTarget(int seniority, int professionId)
			{
				GameDataBridge.AddMethodCall<int, int>(-1, 19, 30, seniority, professionId);
			}

			// Token: 0x06011385 RID: 70533 RVA: 0x0067B472 File Offset: 0x00679672
			public static void GmCmd_Profession_SetBuddhistMonkSavedSoulCount(int count)
			{
				GameDataBridge.AddMethodCall<int>(-1, 19, 31, count);
			}

			// Token: 0x06011386 RID: 70534 RVA: 0x0067B481 File Offset: 0x00679681
			public static void GmCmd_Profession_SetTempleVisited(sbyte stateTemplateId)
			{
				GameDataBridge.AddMethodCall<sbyte>(-1, 19, 32, stateTemplateId);
			}

			// Token: 0x06011387 RID: 70535 RVA: 0x0067B490 File Offset: 0x00679690
			public static void InitAiLifeSkillCombatUsedCard(int listenerId, sbyte skillType, int charId)
			{
				GameDataBridge.AddMethodCall<sbyte, int>(listenerId, 19, 33, skillType, charId);
			}

			// Token: 0x06011388 RID: 70536 RVA: 0x0067B4A0 File Offset: 0x006796A0
			public static void GmCmd_Profession_RecoverHunterCarrierAttackCount()
			{
				GameDataBridge.AddMethodCall(-1, 19, 34);
			}

			// Token: 0x06011389 RID: 70537 RVA: 0x0067B4AE File Offset: 0x006796AE
			public static void GetBlockMerchantTypes(int listenerId, Location location)
			{
				GameDataBridge.AddMethodCall<Location>(listenerId, 19, 35, location);
			}

			// Token: 0x0601138A RID: 70538 RVA: 0x0067B4BD File Offset: 0x006796BD
			public static void GetCharacterMasteredCombatSkills(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 36, charId);
			}

			// Token: 0x0601138B RID: 70539 RVA: 0x0067B4CC File Offset: 0x006796CC
			public static void AddCharacterMasteredCombatSkill(int listenerId, int charId, short skillTemplateId)
			{
				GameDataBridge.AddMethodCall<int, short>(listenerId, 19, 37, charId, skillTemplateId);
			}

			// Token: 0x0601138C RID: 70540 RVA: 0x0067B4DC File Offset: 0x006796DC
			public static void RemoveCharacterMasteredCombatSkill(int listenerId, int charId, short skillTemplateId)
			{
				GameDataBridge.AddMethodCall<int, short>(listenerId, 19, 38, charId, skillTemplateId);
			}

			// Token: 0x0601138D RID: 70541 RVA: 0x0067B4EC File Offset: 0x006796EC
			public static void InvokeFindExtraTreasureEvent(TreasureFindResult result)
			{
				GameDataBridge.AddMethodCall<TreasureFindResult>(-1, 19, 39, result);
			}

			// Token: 0x0601138E RID: 70542 RVA: 0x0067B4FB File Offset: 0x006796FB
			public static void SetAdvancedTeammateCommands(int listenerId, int charId, sbyte cmdType)
			{
				GameDataBridge.AddMethodCall<int, sbyte>(listenerId, 19, 40, charId, cmdType);
			}

			// Token: 0x0601138F RID: 70543 RVA: 0x0067B50B File Offset: 0x0067970B
			public static void CancelAdvancedTeammateCommands(int listenerId, int charId, sbyte cmdType)
			{
				GameDataBridge.AddMethodCall<int, sbyte>(listenerId, 19, 41, charId, cmdType);
			}

			// Token: 0x06011390 RID: 70544 RVA: 0x0067B51B File Offset: 0x0067971B
			public static void GetAllHeavenlyTrees(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 42);
			}

			// Token: 0x06011391 RID: 70545 RVA: 0x0067B529 File Offset: 0x00679729
			public static void GetHeavenlyTreeNearBlocks(int listenerId, int id, int maxSteps)
			{
				GameDataBridge.AddMethodCall<int, int>(listenerId, 19, 43, id, maxSteps);
			}

			// Token: 0x06011392 RID: 70546 RVA: 0x0067B539 File Offset: 0x00679739
			public static void GetInformationSettings(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 44);
			}

			// Token: 0x06011393 RID: 70547 RVA: 0x0067B547 File Offset: 0x00679747
			public static void GetPoisonImmunities(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 45, charId);
			}

			// Token: 0x06011394 RID: 70548 RVA: 0x0067B556 File Offset: 0x00679756
			public static void GetDreamBackTaiwuRelatedCharactersForRelations(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 46);
			}

			// Token: 0x06011395 RID: 70549 RVA: 0x0067B564 File Offset: 0x00679764
			public static void GetDreamBackTaiwuGenealogy(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 47);
			}

			// Token: 0x06011396 RID: 70550 RVA: 0x0067B572 File Offset: 0x00679772
			public static void GetCharacterDisplayDataListForDreamBackRelations(int listenerId, List<int> charIds)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 19, 48, charIds);
			}

			// Token: 0x06011397 RID: 70551 RVA: 0x0067B581 File Offset: 0x00679781
			public static void GetDreamBackLifeRecordByDate(int listenerId, int startDate, int monthCount)
			{
				GameDataBridge.AddMethodCall<int, int>(listenerId, 19, 49, startDate, monthCount);
			}

			// Token: 0x06011398 RID: 70552 RVA: 0x0067B591 File Offset: 0x00679791
			public static void GetNameAndLifeRelatedDataListForDreamBack(int listenerId, List<int> charIds)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 19, 50, charIds);
			}

			// Token: 0x06011399 RID: 70553 RVA: 0x0067B5A0 File Offset: 0x006797A0
			public static void IsCharacterHatingItemRevealed(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 51, charId);
			}

			// Token: 0x0601139A RID: 70554 RVA: 0x0067B5AF File Offset: 0x006797AF
			public static void IsCharacterLovingItemRevealed(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 52, charId);
			}

			// Token: 0x0601139B RID: 70555 RVA: 0x0067B5BE File Offset: 0x006797BE
			public static void IsCharacterHobbyRevealed(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 53, charId);
			}

			// Token: 0x0601139C RID: 70556 RVA: 0x0067B5CD File Offset: 0x006797CD
			public static void SetCharacterRevealedHobbies(int charId, bool isLovingItem)
			{
				GameDataBridge.AddMethodCall<int, bool>(-1, 19, 54, charId, isLovingItem);
			}

			// Token: 0x0601139D RID: 70557 RVA: 0x0067B5DD File Offset: 0x006797DD
			public static void GetConflictCombatSkill(int listenerId, short templateId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 19, 55, templateId);
			}

			// Token: 0x0601139E RID: 70558 RVA: 0x0067B5EC File Offset: 0x006797EC
			public static void GetAllDreamBackLifeRecords(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 56);
			}

			// Token: 0x0601139F RID: 70559 RVA: 0x0067B5FA File Offset: 0x006797FA
			public static void GetDreamBackTaiwuBirthAndEndDates(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 57);
			}

			// Token: 0x060113A0 RID: 70560 RVA: 0x0067B608 File Offset: 0x00679808
			public static void IsCurrentTaiwuOverwrittenByDreamBack(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 58);
			}

			// Token: 0x060113A1 RID: 70561 RVA: 0x0067B616 File Offset: 0x00679816
			public static void ApplyConflictCombatSkillResult(short templateId, bool overwrite)
			{
				GameDataBridge.AddMethodCall<short, bool>(-1, 19, 59, templateId, overwrite);
			}

			// Token: 0x060113A2 RID: 70562 RVA: 0x0067B626 File Offset: 0x00679826
			public static void HaveConflictCombatSkill(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 60);
			}

			// Token: 0x060113A3 RID: 70563 RVA: 0x0067B634 File Offset: 0x00679834
			public static void AddTaiwuOneWayRelationCoolDown(int charId, bool isAdoreRelation)
			{
				GameDataBridge.AddMethodCall<int, bool>(-1, 19, 61, charId, isAdoreRelation);
			}

			// Token: 0x060113A4 RID: 70564 RVA: 0x0067B644 File Offset: 0x00679844
			public static void IsTaiwuAbleToAddOneWayRelation(int listenerId, int charId, bool isAdoreRelation)
			{
				GameDataBridge.AddMethodCall<int, bool>(listenerId, 19, 62, charId, isAdoreRelation);
			}

			// Token: 0x060113A5 RID: 70565 RVA: 0x0067B654 File Offset: 0x00679854
			public static void GetTaiwuAddOneWayRelationCoolDown(int listenerId, int charId, bool isAdoreRelation)
			{
				GameDataBridge.AddMethodCall<int, bool>(listenerId, 19, 63, charId, isAdoreRelation);
			}

			// Token: 0x060113A6 RID: 70566 RVA: 0x0067B664 File Offset: 0x00679864
			public static void FeedCarrier(ItemKey carrier, ItemKey food, int count, ItemSourceType sourceType)
			{
				GameDataBridge.AddMethodCall<ItemKey, ItemKey, int, ItemSourceType>(-1, 19, 64, carrier, food, count, sourceType);
			}

			// Token: 0x060113A7 RID: 70567 RVA: 0x0067B676 File Offset: 0x00679876
			public static void GetCarrierTamePoint(int listenerId, int carrierId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 65, carrierId);
			}

			// Token: 0x060113A8 RID: 70568 RVA: 0x0067B685 File Offset: 0x00679885
			public static void GetDreamBackCharacterDisplayDataList(int listenerId, List<int> charIdList)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 19, 66, charIdList);
			}

			// Token: 0x060113A9 RID: 70569 RVA: 0x0067B694 File Offset: 0x00679894
			public static void GetCarrierMaxTamePoint(int listenerId, int carrierId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 67, carrierId);
			}

			// Token: 0x060113AA RID: 70570 RVA: 0x0067B6A3 File Offset: 0x006798A3
			public static void GetCurrMaxJiaoPoolCount(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 68);
			}

			// Token: 0x060113AB RID: 70571 RVA: 0x0067B6B1 File Offset: 0x006798B1
			public static void GmCmd_FindFiveLoongLocation(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 69);
			}

			// Token: 0x060113AC RID: 70572 RVA: 0x0067B6BF File Offset: 0x006798BF
			public static void GetJiaoPoolBlockStyle(int listenerId, int poolId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 70, poolId);
			}

			// Token: 0x060113AD RID: 70573 RVA: 0x0067B6CE File Offset: 0x006798CE
			public static void SetJiaoPoolBlockStyle(int poolId, short style)
			{
				GameDataBridge.AddMethodCall<int, short>(-1, 19, 71, poolId, style);
			}

			// Token: 0x060113AE RID: 70574 RVA: 0x0067B6DE File Offset: 0x006798DE
			public static void GetChildrenOfLoongById(int listenerId, int id)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 72, id);
			}

			// Token: 0x060113AF RID: 70575 RVA: 0x0067B6ED File Offset: 0x006798ED
			public static void GetJiaoPoolList(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 73);
			}

			// Token: 0x060113B0 RID: 70576 RVA: 0x0067B6FB File Offset: 0x006798FB
			public static void GetJiaoById(int listenerId, int id)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 74, id);
			}

			// Token: 0x060113B1 RID: 70577 RVA: 0x0067B70A File Offset: 0x0067990A
			public static void GetJiaoPoolAllJiaoData(int listenerId, int poolId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 75, poolId);
			}

			// Token: 0x060113B2 RID: 70578 RVA: 0x0067B719 File Offset: 0x00679919
			public static void PutJiaoInPool(int poolId, int id)
			{
				GameDataBridge.AddMethodCall<int, int>(-1, 19, 76, poolId, id);
			}

			// Token: 0x060113B3 RID: 70579 RVA: 0x0067B729 File Offset: 0x00679929
			public static void PutAnotherJiaoInPool(int poolId, int id)
			{
				GameDataBridge.AddMethodCall<int, int>(-1, 19, 77, poolId, id);
			}

			// Token: 0x060113B4 RID: 70580 RVA: 0x0067B739 File Offset: 0x00679939
			public static void PutJiaoOutOfPool(int listenerId, int poolId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 78, poolId);
			}

			// Token: 0x060113B5 RID: 70581 RVA: 0x0067B748 File Offset: 0x00679948
			public static void ChangeNurturance(int poolId, short templateId)
			{
				GameDataBridge.AddMethodCall<int, short>(-1, 19, 79, poolId, templateId);
			}

			// Token: 0x060113B6 RID: 70582 RVA: 0x0067B758 File Offset: 0x00679958
			public static void ChangeJiaoName(int poolId, string name)
			{
				GameDataBridge.AddMethodCall<int, string>(-1, 19, 80, poolId, name);
			}

			// Token: 0x060113B7 RID: 70583 RVA: 0x0067B768 File Offset: 0x00679968
			public static void DisableJiaoPool(int poolId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 19, 81, poolId);
			}

			// Token: 0x060113B8 RID: 70584 RVA: 0x0067B777 File Offset: 0x00679977
			public static void EnableJiaoPool(int poolId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 19, 82, poolId);
			}

			// Token: 0x060113B9 RID: 70585 RVA: 0x0067B786 File Offset: 0x00679986
			public static void GetJiaoLoongNameRelatedDataList(int listenerId, List<int> jiaoLoongIds)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 19, 83, jiaoLoongIds);
			}

			// Token: 0x060113BA RID: 70586 RVA: 0x0067B795 File Offset: 0x00679995
			public static void GetAllJiaoForPool(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 84);
			}

			// Token: 0x060113BB RID: 70587 RVA: 0x0067B7A3 File Offset: 0x006799A3
			public static void GetAllJiaoForEvolve(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 85);
			}

			// Token: 0x060113BC RID: 70588 RVA: 0x0067B7B1 File Offset: 0x006799B1
			public static void GetJiaoByItemKey(int listenerId, ItemKey key)
			{
				GameDataBridge.AddMethodCall<ItemKey>(listenerId, 19, 86, key);
			}

			// Token: 0x060113BD RID: 70589 RVA: 0x0067B7C0 File Offset: 0x006799C0
			public static void GetJiaosByItemKeys(int listenerId, List<ItemKey> keys)
			{
				GameDataBridge.AddMethodCall<List<ItemKey>>(listenerId, 19, 87, keys);
			}

			// Token: 0x060113BE RID: 70590 RVA: 0x0067B7CF File Offset: 0x006799CF
			public static void GetChildrenOfLoongByItemKey(int listenerId, ItemKey key)
			{
				GameDataBridge.AddMethodCall<ItemKey>(listenerId, 19, 88, key);
			}

			// Token: 0x060113BF RID: 70591 RVA: 0x0067B7DE File Offset: 0x006799DE
			public static void PutEggIntoPool(int poolId, ItemKey eggItemKey)
			{
				GameDataBridge.AddMethodCall<int, ItemKey>(-1, 19, 89, poolId, eggItemKey);
			}

			// Token: 0x060113C0 RID: 70592 RVA: 0x0067B7EE File Offset: 0x006799EE
			public static void JiaoPoolInteract(int poolId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 19, 90, poolId);
			}

			// Token: 0x060113C1 RID: 70593 RVA: 0x0067B800 File Offset: 0x00679A00
			public static void GmCmd_AddJiao(int listenerId, short templateId, short nurturanceTemplateId, sbyte growthStage, bool isMale, bool isMaxTamePoint, bool isMaxProperty)
			{
				GameDataBridge.AddMethodCall<short, short, sbyte, bool, bool, bool>(listenerId, 19, 91, templateId, nurturanceTemplateId, growthStage, isMale, isMaxTamePoint, isMaxProperty);
			}

			// Token: 0x060113C2 RID: 70594 RVA: 0x0067B822 File Offset: 0x00679A22
			public static void GmCmd_PutJiaoInFirstPool(int jiaoId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 19, 92, jiaoId);
			}

			// Token: 0x060113C3 RID: 70595 RVA: 0x0067B831 File Offset: 0x00679A31
			public static void GmCmd_AddChildOfLoong(int listenerId, short templateId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 19, 93, templateId);
			}

			// Token: 0x060113C4 RID: 70596 RVA: 0x0067B840 File Offset: 0x00679A40
			public static void JiaoEvolveToChildOfLoong(ItemKey key, short childOfLoongTemplateId)
			{
				GameDataBridge.AddMethodCall<ItemKey, short>(-1, 19, 94, key, childOfLoongTemplateId);
			}

			// Token: 0x060113C5 RID: 70597 RVA: 0x0067B850 File Offset: 0x00679A50
			public static void GetJiaoEvolutionChoice(int listenerId, ItemKey key)
			{
				GameDataBridge.AddMethodCall<ItemKey>(listenerId, 19, 95, key);
			}

			// Token: 0x060113C6 RID: 70598 RVA: 0x0067B85F File Offset: 0x00679A5F
			public static void ResetJiaoPoolStatus()
			{
				GameDataBridge.AddMethodCall(-1, 19, 96);
			}

			// Token: 0x060113C7 RID: 70599 RVA: 0x0067B86D File Offset: 0x00679A6D
			public static void GetAllAdultJiao(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 97);
			}

			// Token: 0x060113C8 RID: 70600 RVA: 0x0067B87B File Offset: 0x00679A7B
			public static void GetAllEvolvingJiao(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 98);
			}

			// Token: 0x060113C9 RID: 70601 RVA: 0x0067B889 File Offset: 0x00679A89
			public static void GetJiaoTemplateIdByCarrierTemplateId(int listenerId, short templateId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 19, 99, templateId);
			}

			// Token: 0x060113CA RID: 70602 RVA: 0x0067B898 File Offset: 0x00679A98
			public static void CalcResourceChangeByJiaoPool(int listenerId, sbyte resourceType)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 19, 100, resourceType);
			}

			// Token: 0x060113CB RID: 70603 RVA: 0x0067B8A7 File Offset: 0x00679AA7
			public static void IsOwnedChildrenOfLoong(int listenerId, short templateId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 19, 101, templateId);
			}

			// Token: 0x060113CC RID: 70604 RVA: 0x0067B8B6 File Offset: 0x00679AB6
			public static void GetNextRandomChildrenOfLoong(int listenerId, int jiaoId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 102, jiaoId);
			}

			// Token: 0x060113CD RID: 70605 RVA: 0x0067B8C5 File Offset: 0x00679AC5
			public static void GmCmd_AddFleeCarrier(bool isJiaoLoong)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 19, 103, isJiaoLoong);
			}

			// Token: 0x060113CE RID: 70606 RVA: 0x0067B8D4 File Offset: 0x00679AD4
			public static void GetIsJiaoPoolOpen(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 104);
			}

			// Token: 0x060113CF RID: 70607 RVA: 0x0067B8E2 File Offset: 0x00679AE2
			public static void FillJiaoRecordArgumentCollection(int listenerId, JiaoPoolRecordArgumentCollection collection)
			{
				GameDataBridge.AddMethodCall<JiaoPoolRecordArgumentCollection>(listenerId, 19, 105, collection);
			}

			// Token: 0x060113D0 RID: 70608 RVA: 0x0067B8F1 File Offset: 0x00679AF1
			public static void GetJiaoEvolutionPageStatus(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 106);
			}

			// Token: 0x060113D1 RID: 70609 RVA: 0x0067B8FF File Offset: 0x00679AFF
			public static void GetIsBabysittingMode(int listenerId, int poolId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 107, poolId);
			}

			// Token: 0x060113D2 RID: 70610 RVA: 0x0067B90E File Offset: 0x00679B0E
			public static void SetIsBabysittingMode(int poolId, bool isOn)
			{
				GameDataBridge.AddMethodCall<int, bool>(-1, 19, 108, poolId, isOn);
			}

			// Token: 0x060113D3 RID: 70611 RVA: 0x0067B91E File Offset: 0x00679B1E
			public static void GetFiveLoongDictCount(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 109);
			}

			// Token: 0x060113D4 RID: 70612 RVA: 0x0067B92C File Offset: 0x00679B2C
			public static void GetJiaoLoongNameRelatedData(int listenerId, ItemKey itemKey)
			{
				GameDataBridge.AddMethodCall<ItemKey>(listenerId, 19, 110, itemKey);
			}

			// Token: 0x060113D5 RID: 70613 RVA: 0x0067B93B File Offset: 0x00679B3B
			public static void IsJiaoAbleToPet(int listenerId, int jiaoId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 111, jiaoId);
			}

			// Token: 0x060113D6 RID: 70614 RVA: 0x0067B94A File Offset: 0x00679B4A
			public static void PetJiao(int jiaoId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 19, 112, jiaoId);
			}

			// Token: 0x060113D7 RID: 70615 RVA: 0x0067B959 File Offset: 0x00679B59
			public static void JiaoPoolPetJiao(int poolId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 19, 113, poolId);
			}

			// Token: 0x060113D8 RID: 70616 RVA: 0x0067B968 File Offset: 0x00679B68
			public static void GetTaiwuAddOneWayRelationResultCode(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 114, charId);
			}

			// Token: 0x060113D9 RID: 70617 RVA: 0x0067B977 File Offset: 0x00679B77
			public static void RequestRecruitCharacterData(int listenerId, BuildingBlockKey buildingBlockKey, int earningDataIndex)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, int>(listenerId, 19, 115, buildingBlockKey, earningDataIndex);
			}

			// Token: 0x060113DA RID: 70618 RVA: 0x0067B987 File Offset: 0x00679B87
			public static void RequestRecruitCharacterData(int listenerId, BuildingBlockKey buildingBlockKey, int earningDataIndex, bool autoRemove)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, int, bool>(listenerId, 19, 115, buildingBlockKey, earningDataIndex, autoRemove);
			}

			// Token: 0x060113DB RID: 70619 RVA: 0x0067B998 File Offset: 0x00679B98
			public static void GmCmd_AddThreeCorpses()
			{
				GameDataBridge.AddMethodCall(-1, 19, 116);
			}

			// Token: 0x060113DC RID: 70620 RVA: 0x0067B9A6 File Offset: 0x00679BA6
			public static void ApplyRanshanThreeCorpsesLegendaryBookKeepingResult(List<sbyte> huajuBooks, List<sbyte> xuanzhiBooks, List<sbyte> yingjiaoBooks)
			{
				GameDataBridge.AddMethodCall<List<sbyte>, List<sbyte>, List<sbyte>>(-1, 19, 117, huajuBooks, xuanzhiBooks, yingjiaoBooks);
			}

			// Token: 0x060113DD RID: 70621 RVA: 0x0067B9B7 File Offset: 0x00679BB7
			public static void GetItemListForRanshanTreeCorpsesLegendaryBookKeeping(int listenerId, int corpseId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 118, corpseId);
			}

			// Token: 0x060113DE RID: 70622 RVA: 0x0067B9C6 File Offset: 0x00679BC6
			public static void GmCmd_AddDisplayEventLegendaryBookKeeping()
			{
				GameDataBridge.AddMethodCall(-1, 19, 119);
			}

			// Token: 0x060113DF RID: 70623 RVA: 0x0067B9D4 File Offset: 0x00679BD4
			public static void SetRanshanThreeCorpsesCharacterTarget(short templateId, sbyte targetId, sbyte notch)
			{
				GameDataBridge.AddMethodCall<short, sbyte, sbyte>(-1, 19, 120, templateId, targetId, notch);
			}

			// Token: 0x060113E0 RID: 70624 RVA: 0x0067B9E5 File Offset: 0x00679BE5
			public static void GetBookStrategiesExpireTime(int listenerId, ItemKey book)
			{
				GameDataBridge.AddMethodCall<ItemKey>(listenerId, 19, 121, book);
			}

			// Token: 0x060113E1 RID: 70625 RVA: 0x0067B9F4 File Offset: 0x00679BF4
			public static void SetMonthlyNotificationSortingGroup(NotificationSortingGroup group)
			{
				GameDataBridge.AddMethodCall<NotificationSortingGroup>(-1, 19, 122, group);
			}

			// Token: 0x060113E2 RID: 70626 RVA: 0x0067BA03 File Offset: 0x00679C03
			public static void SetCharTeammateCommandsManual(int listenerId, int charId, List<sbyte> cmdTypes)
			{
				GameDataBridge.AddMethodCall<int, List<sbyte>>(listenerId, 19, 123, charId, cmdTypes);
			}

			// Token: 0x060113E3 RID: 70627 RVA: 0x0067BA13 File Offset: 0x00679C13
			public static void GetCharAdvancedTeammateCommands(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 124, charId);
			}

			// Token: 0x060113E4 RID: 70628 RVA: 0x0067BA22 File Offset: 0x00679C22
			public static void IsStoneRoomFull(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 125);
			}

			// Token: 0x060113E5 RID: 70629 RVA: 0x0067BA30 File Offset: 0x00679C30
			public static void ExtinguishFulongInFlameArea(int listenerId, short blockId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 19, 126, blockId);
			}

			// Token: 0x060113E6 RID: 70630 RVA: 0x0067BA3F File Offset: 0x00679C3F
			public static void ExtinguishFulongInFlameArea(int listenerId, short blockId, bool isEvent)
			{
				GameDataBridge.AddMethodCall<short, bool>(listenerId, 19, 126, blockId, isEvent);
			}

			// Token: 0x060113E7 RID: 70631 RVA: 0x0067BA4F File Offset: 0x00679C4F
			public static void TriggerFulongInFlameAreaMine(short blockId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 19, 127, blockId);
			}

			// Token: 0x060113E8 RID: 70632 RVA: 0x0067BA5E File Offset: 0x00679C5E
			public static void ApplyFulongInFlameAreaFullyExtinguished(int index)
			{
				GameDataBridge.AddMethodCall<int>(-1, 19, 128, index);
			}

			// Token: 0x060113E9 RID: 70633 RVA: 0x0067BA70 File Offset: 0x00679C70
			public static void ApplyFulongInFlameAreaFullyExtinguished(int index, bool triggerEvent)
			{
				GameDataBridge.AddMethodCall<int, bool>(-1, 19, 128, index, triggerEvent);
			}

			// Token: 0x060113EA RID: 70634 RVA: 0x0067BA83 File Offset: 0x00679C83
			public static void GmCmd_GenerateFulongFlameArea(int radius, int mineCount)
			{
				GameDataBridge.AddMethodCall<int, int>(-1, 19, 129, radius, mineCount);
			}

			// Token: 0x060113EB RID: 70635 RVA: 0x0067BA96 File Offset: 0x00679C96
			public static void HunterSkill_AnimalCharacterToItem(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 19, 130, charId);
			}

			// Token: 0x060113EC RID: 70636 RVA: 0x0067BAA8 File Offset: 0x00679CA8
			public static void ConfirmProfessionSkillsEquipment(TaiwuProfessionSkillSlots slots)
			{
				GameDataBridge.AddMethodCall<TaiwuProfessionSkillSlots>(-1, 19, 131, slots);
			}

			// Token: 0x060113ED RID: 70637 RVA: 0x0067BABA File Offset: 0x00679CBA
			public static void GmCmd_CastTasterUltimateOnCurrentBlock(int listenerId, bool isCombatSkill)
			{
				GameDataBridge.AddMethodCall<bool>(listenerId, 19, 132, isCombatSkill);
			}

			// Token: 0x060113EE RID: 70638 RVA: 0x0067BACC File Offset: 0x00679CCC
			public static void EatTianJieFuLu(int charId, ItemKey itemKey, int amount)
			{
				GameDataBridge.AddMethodCall<int, ItemKey, int>(-1, 19, 133, charId, itemKey, amount);
			}

			// Token: 0x060113EF RID: 70639 RVA: 0x0067BAE0 File Offset: 0x00679CE0
			public static void CheckAristocratUltimateSpecialCondition(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 134);
			}

			// Token: 0x060113F0 RID: 70640 RVA: 0x0067BAF1 File Offset: 0x00679CF1
			public static void CheckBeggarUltimateSpecialCondition(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 135);
			}

			// Token: 0x060113F1 RID: 70641 RVA: 0x0067BB02 File Offset: 0x00679D02
			public static void CheckTasterUltimateSpecialCondition(int listenerId, bool isCombatSkill)
			{
				GameDataBridge.AddMethodCall<bool>(listenerId, 19, 136, isCombatSkill);
			}

			// Token: 0x060113F2 RID: 70642 RVA: 0x0067BB14 File Offset: 0x00679D14
			public static void GM_GetFriendOrFamilySendGift(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 137, charId);
			}

			// Token: 0x060113F3 RID: 70643 RVA: 0x0067BB26 File Offset: 0x00679D26
			public static void GmCmd_CreateGearMate(int templateId, string name)
			{
				GameDataBridge.AddMethodCall<int, string>(-1, 19, 138, templateId, name);
			}

			// Token: 0x060113F4 RID: 70644 RVA: 0x0067BB39 File Offset: 0x00679D39
			public static void GetGearMateRepairEffect(int listenerId, int gearMateId, int artisanId, sbyte type, ItemKey toolKey)
			{
				GameDataBridge.AddMethodCall<int, int, sbyte, ItemKey>(listenerId, 19, 139, gearMateId, artisanId, type, toolKey);
			}

			// Token: 0x060113F5 RID: 70645 RVA: 0x0067BB4F File Offset: 0x00679D4F
			public static void RepairGearMate(int gearMateId, int artisanId, sbyte type, ItemKey toolKey)
			{
				GameDataBridge.AddMethodCall<int, int, sbyte, ItemKey>(-1, 19, 140, gearMateId, artisanId, type, toolKey);
			}

			// Token: 0x060113F6 RID: 70646 RVA: 0x0067BB64 File Offset: 0x00679D64
			public static void GetGearMateRepairRequirement(int listenerId, int gearMateId, int artisanId, sbyte type, ItemKey toolKey)
			{
				GameDataBridge.AddMethodCall<int, int, sbyte, ItemKey>(listenerId, 19, 141, gearMateId, artisanId, type, toolKey);
			}

			// Token: 0x060113F7 RID: 70647 RVA: 0x0067BB7A File Offset: 0x00679D7A
			public static void GetGearMateAvailableRepairCount(int listenerId, int artisanId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 142, artisanId);
			}

			// Token: 0x060113F8 RID: 70648 RVA: 0x0067BB8C File Offset: 0x00679D8C
			public static void GetGearMateRepairRequirementDisplayDatas(int listenerId, int gearMateId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 143, gearMateId);
			}

			// Token: 0x060113F9 RID: 70649 RVA: 0x0067BB9E File Offset: 0x00679D9E
			public static void UpgradeGearMate(int charId, sbyte type, ItemKey itemKey)
			{
				GameDataBridge.AddMethodCall<int, sbyte, ItemKey>(-1, 19, 144, charId, type, itemKey);
			}

			// Token: 0x060113FA RID: 70650 RVA: 0x0067BBB2 File Offset: 0x00679DB2
			public static void UpgradeGearMate(int charId, sbyte type, ItemKey itemKey, int count)
			{
				GameDataBridge.AddMethodCall<int, sbyte, ItemKey, int>(-1, 19, 144, charId, type, itemKey, count);
			}

			// Token: 0x060113FB RID: 70651 RVA: 0x0067BBC7 File Offset: 0x00679DC7
			public static void UpgradeGearMate(int charId, sbyte type, ItemKey itemKey, int count, ItemSourceType sourceType)
			{
				GameDataBridge.AddMethodCall<int, sbyte, ItemKey, int, ItemSourceType>(-1, 19, 144, charId, type, itemKey, count, sourceType);
			}

			// Token: 0x060113FC RID: 70652 RVA: 0x0067BBDE File Offset: 0x00679DDE
			public static void GetCharacterConsummateLevelProgress(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 145, charId);
			}

			// Token: 0x060113FD RID: 70653 RVA: 0x0067BBF0 File Offset: 0x00679DF0
			public static void GetMartialArtistCreateGoodRandomEnemyAndBadRandomEnemyCount(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 146);
			}

			// Token: 0x060113FE RID: 70654 RVA: 0x0067BC01 File Offset: 0x00679E01
			public static void GetGearMateById(int listenerId, int artisanId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 147, artisanId);
			}

			// Token: 0x060113FF RID: 70655 RVA: 0x0067BC13 File Offset: 0x00679E13
			public static void CheckSpecialCondition_SavageSkill_1(int listenerId, ProfessionData professionData)
			{
				GameDataBridge.AddMethodCall<ProfessionData>(listenerId, 19, 148, professionData);
			}

			// Token: 0x06011400 RID: 70656 RVA: 0x0067BC25 File Offset: 0x00679E25
			public static void GetMerchantExtraGoods(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 149, charId);
			}

			// Token: 0x06011401 RID: 70657 RVA: 0x0067BC37 File Offset: 0x00679E37
			public static void SetProfessionExtraSeniority(int professionId, int extraSeniority)
			{
				GameDataBridge.AddMethodCall<int, int>(-1, 19, 150, professionId, extraSeniority);
			}

			// Token: 0x06011402 RID: 70658 RVA: 0x0067BC4A File Offset: 0x00679E4A
			public static void CanShowProfessionSkillUnlocked(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 151);
			}

			// Token: 0x06011403 RID: 70659 RVA: 0x0067BC5B File Offset: 0x00679E5B
			public static void GetGearMateBreakoutCombatSkillBanReasonList(int listenerId, int gearMateId, List<short> checkCombatSkillList)
			{
				GameDataBridge.AddMethodCall<int, List<short>>(listenerId, 19, 152, gearMateId, checkCombatSkillList);
			}

			// Token: 0x06011404 RID: 70660 RVA: 0x0067BC6E File Offset: 0x00679E6E
			public static void SetDukeSkill3Crickets(List<short> effectBlocks)
			{
				GameDataBridge.AddMethodCall<List<short>>(-1, 19, 153, effectBlocks);
			}

			// Token: 0x06011405 RID: 70661 RVA: 0x0067BC80 File Offset: 0x00679E80
			public static void GetAllSkillBooksGearMateCanRead(int listenerId, bool isCombatSkill)
			{
				GameDataBridge.AddMethodCall<bool>(listenerId, 19, 154, isCombatSkill);
			}

			// Token: 0x06011406 RID: 70662 RVA: 0x0067BC92 File Offset: 0x00679E92
			public static void CanIdentifyCricket(int listenerId, int cricketId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 155, cricketId);
			}

			// Token: 0x06011407 RID: 70663 RVA: 0x0067BCA4 File Offset: 0x00679EA4
			public static void CanUpgradeCricket(int listenerId, int cricketId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 156, cricketId);
			}

			// Token: 0x06011408 RID: 70664 RVA: 0x0067BCB6 File Offset: 0x00679EB6
			public static void CanConvertToAnimalCharacter(int listenerId, ItemKey itemKey)
			{
				GameDataBridge.AddMethodCall<ItemKey>(listenerId, 19, 157, itemKey);
			}

			// Token: 0x06011409 RID: 70665 RVA: 0x0067BCC8 File Offset: 0x00679EC8
			public static void GetJiaoLoongDisplayDataByItemKey(int listenerId, ItemKey key)
			{
				GameDataBridge.AddMethodCall<ItemKey>(listenerId, 19, 158, key);
			}

			// Token: 0x0601140A RID: 70666 RVA: 0x0067BCDA File Offset: 0x00679EDA
			public static void GmCmd_SetCharacterProficiencies(int charId, short skillId, int value)
			{
				GameDataBridge.AddMethodCall<int, short, int>(-1, 19, 159, charId, skillId, value);
			}

			// Token: 0x0601140B RID: 70667 RVA: 0x0067BCEE File Offset: 0x00679EEE
			public static void GmCmd_CreateRandomEnemyAroundHeavenlyTree(int count, int range)
			{
				GameDataBridge.AddMethodCall<int, int>(-1, 19, 160, count, range);
			}

			// Token: 0x0601140C RID: 70668 RVA: 0x0067BD01 File Offset: 0x00679F01
			public static void GmCmd_ShowUnlockedProfessionSkill(int skillTemplateId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 19, 161, skillTemplateId);
			}

			// Token: 0x0601140D RID: 70669 RVA: 0x0067BD13 File Offset: 0x00679F13
			public static void SetVillagerRoleAutoActionState(short roleTemplateId, ulong actionStates)
			{
				GameDataBridge.AddMethodCall<short, ulong>(-1, 19, 162, roleTemplateId, actionStates);
			}

			// Token: 0x0601140E RID: 70670 RVA: 0x0067BD26 File Offset: 0x00679F26
			public static void ChangeBuildingArrangementSettingPresetData(int index, BuildingOptionAutoGiveMemberPreset preset)
			{
				GameDataBridge.AddMethodCall<int, BuildingOptionAutoGiveMemberPreset>(-1, 19, 163, index, preset);
			}

			// Token: 0x0601140F RID: 70671 RVA: 0x0067BD39 File Offset: 0x00679F39
			public static void AddMaterialToArtisanOrder(ArtisanOrder order, List<ItemDisplayData> itemList)
			{
				GameDataBridge.AddMethodCall<ArtisanOrder, List<ItemDisplayData>>(-1, 19, 164, order, itemList);
			}

			// Token: 0x06011410 RID: 70672 RVA: 0x0067BD4C File Offset: 0x00679F4C
			public static void GetArtisanOrderProductionPool(int listenerId, ArtisanOrder order)
			{
				GameDataBridge.AddMethodCall<ArtisanOrder>(listenerId, 19, 165, order);
			}

			// Token: 0x06011411 RID: 70673 RVA: 0x0067BD5E File Offset: 0x00679F5E
			public static void SetArtisanOrderProductionType(ArtisanOrder order, short itemSubType)
			{
				GameDataBridge.AddMethodCall<ArtisanOrder, short>(-1, 19, 166, order, itemSubType);
			}

			// Token: 0x06011412 RID: 70674 RVA: 0x0067BD71 File Offset: 0x00679F71
			public static void SetArtisanOrderStorageType(ArtisanOrder order, ItemSourceType type)
			{
				GameDataBridge.AddMethodCall<ArtisanOrder, ItemSourceType>(-1, 19, 167, order, type);
			}

			// Token: 0x06011413 RID: 70675 RVA: 0x0067BD84 File Offset: 0x00679F84
			public static void GetNpcArtisanOrder(int listenerId, int artisanId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 168, artisanId);
			}

			// Token: 0x06011414 RID: 70676 RVA: 0x0067BD96 File Offset: 0x00679F96
			public static void InterceptArtisanOrder(int artisanId, int subscriberId, bool isDebateWon)
			{
				GameDataBridge.AddMethodCall<int, int, bool>(-1, 19, 169, artisanId, subscriberId, isDebateWon);
			}

			// Token: 0x06011415 RID: 70677 RVA: 0x0067BDAA File Offset: 0x00679FAA
			public static void GetBuildingArtisanOrder(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 19, 170, blockKey);
			}

			// Token: 0x06011416 RID: 70678 RVA: 0x0067BDBC File Offset: 0x00679FBC
			public static void CreateArtisanOrder(int artisanId, int subscriberId, sbyte lifeSkillType)
			{
				GameDataBridge.AddMethodCall<int, int, sbyte>(-1, 19, 171, artisanId, subscriberId, lifeSkillType);
			}

			// Token: 0x06011417 RID: 70679 RVA: 0x0067BDD0 File Offset: 0x00679FD0
			public static void CreateArtisanOrder(int artisanId, int subscriberId, sbyte lifeSkillType, short makeItemSubType)
			{
				GameDataBridge.AddMethodCall<int, int, sbyte, short>(-1, 19, 171, artisanId, subscriberId, lifeSkillType, makeItemSubType);
			}

			// Token: 0x06011418 RID: 70680 RVA: 0x0067BDE5 File Offset: 0x00679FE5
			public static void GetProductionPoolPreview(int listenerId, int artisanId, sbyte lifeSkillType)
			{
				GameDataBridge.AddMethodCall<int, sbyte>(listenerId, 19, 172, artisanId, lifeSkillType);
			}

			// Token: 0x06011419 RID: 70681 RVA: 0x0067BDF8 File Offset: 0x00679FF8
			public static void GetProductionPoolPreview(int listenerId, int artisanId, sbyte lifeSkillType, short itemSubType)
			{
				GameDataBridge.AddMethodCall<int, sbyte, short>(listenerId, 19, 172, artisanId, lifeSkillType, itemSubType);
			}

			// Token: 0x0601141A RID: 70682 RVA: 0x0067BE0C File Offset: 0x0067A00C
			public static void ArtisanOrderDebate(int artisanId, bool isDebateWon)
			{
				GameDataBridge.AddMethodCall<int, bool>(-1, 19, 173, artisanId, isDebateWon);
			}

			// Token: 0x0601141B RID: 70683 RVA: 0x0067BE1F File Offset: 0x0067A01F
			public static void GetArtisanOrderMaterialPreview(int listenerId, ArtisanOrder order, List<ItemDisplayData> itemList)
			{
				GameDataBridge.AddMethodCall<ArtisanOrder, List<ItemDisplayData>>(listenerId, 19, 174, order, itemList);
			}

			// Token: 0x0601141C RID: 70684 RVA: 0x0067BE32 File Offset: 0x0067A032
			public static void GetArtisanOrderCanProduceItemSubType(int listenerId, ArtisanOrder order)
			{
				GameDataBridge.AddMethodCall<ArtisanOrder>(listenerId, 19, 175, order);
			}

			// Token: 0x0601141D RID: 70685 RVA: 0x0067BE44 File Offset: 0x0067A044
			public static void SetFarmerAutoCollectStorageType(sbyte type)
			{
				GameDataBridge.AddMethodCall<sbyte>(-1, 19, 176, type);
			}

			// Token: 0x0601141E RID: 70686 RVA: 0x0067BE56 File Offset: 0x0067A056
			public static void UpdateWoodenXiangshuAvatarSelectedFeatures(short featureId, bool isAdd)
			{
				GameDataBridge.AddMethodCall<short, bool>(-1, 19, 177, featureId, isAdd);
			}

			// Token: 0x0601141F RID: 70687 RVA: 0x0067BE69 File Offset: 0x0067A069
			public static void GmCmd_GetBuildingAreaEffectProgresses(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 178);
			}

			// Token: 0x06011420 RID: 70688 RVA: 0x0067BE7A File Offset: 0x0067A07A
			public static void GmCmd_SetBuildingAreaEffectProgresses(int animal, int cricket, int adventure)
			{
				GameDataBridge.AddMethodCall<int, int, int>(-1, 19, 179, animal, cricket, adventure);
			}

			// Token: 0x06011421 RID: 70689 RVA: 0x0067BE8E File Offset: 0x0067A08E
			public static void GmCmd_ReleaseAllKilledByLongYufuCharacters()
			{
				GameDataBridge.AddMethodCall(-1, 19, 180);
			}

			// Token: 0x06011422 RID: 70690 RVA: 0x0067BE9F File Offset: 0x0067A09F
			public static void GmCmd_RecordKilledByLongYufuCharacter(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 19, 181, charId);
			}

			// Token: 0x06011423 RID: 70691 RVA: 0x0067BEB1 File Offset: 0x0067A0B1
			public static void GmCmd_VitalInfectionInOut(int charId, int type, int value)
			{
				GameDataBridge.AddMethodCall<int, int, int>(-1, 19, 182, charId, type, value);
			}

			// Token: 0x06011424 RID: 70692 RVA: 0x0067BEC5 File Offset: 0x0067A0C5
			public static void CheckSpecialCondition_HunterSkill2(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 183);
			}

			// Token: 0x06011425 RID: 70693 RVA: 0x0067BED6 File Offset: 0x0067A0D6
			public static void GetThreeVitalsCharDataList(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 184);
			}

			// Token: 0x06011426 RID: 70694 RVA: 0x0067BEE7 File Offset: 0x0067A0E7
			public static void GmCmd_InitThreeVitals()
			{
				GameDataBridge.AddMethodCall(-1, 19, 185);
			}

			// Token: 0x06011427 RID: 70695 RVA: 0x0067BEF8 File Offset: 0x0067A0F8
			public static void GetThreeVitalsTargetCharDataList(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 186);
			}

			// Token: 0x06011428 RID: 70696 RVA: 0x0067BF09 File Offset: 0x0067A109
			public static void TransferInfectionBetweenVitalAndCharacter(int charId, SectStoryThreeVitalsCharacterType type, int value)
			{
				GameDataBridge.AddMethodCall<int, SectStoryThreeVitalsCharacterType, int>(-1, 19, 187, charId, type, value);
			}

			// Token: 0x06011429 RID: 70697 RVA: 0x0067BF1D File Offset: 0x0067A11D
			public static void SetVitalInPrison(SectStoryThreeVitalsCharacterType type, bool isInPrison)
			{
				GameDataBridge.AddMethodCall<SectStoryThreeVitalsCharacterType, bool>(-1, 19, 188, type, isInPrison);
			}

			// Token: 0x0601142A RID: 70698 RVA: 0x0067BF30 File Offset: 0x0067A130
			public static void GetBuildingArtisanOrderAfterUpdate(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 19, 189, blockKey);
			}

			// Token: 0x0601142B RID: 70699 RVA: 0x0067BF42 File Offset: 0x0067A142
			public static void GetCanSelectThreeVitalsDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 190);
			}

			// Token: 0x0601142C RID: 70700 RVA: 0x0067BF53 File Offset: 0x0067A153
			public static void AreVitalsDemon(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 191);
			}

			// Token: 0x0601142D RID: 70701 RVA: 0x0067BF64 File Offset: 0x0067A164
			public static void GetOppositeThreeVitalsCharDataList(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 192);
			}

			// Token: 0x0601142E RID: 70702 RVA: 0x0067BF75 File Offset: 0x0067A175
			public static void SetVitalHasPlayedComeAnim(SectStoryThreeVitalsCharacterType type, bool hasPlayedComeAnim)
			{
				GameDataBridge.AddMethodCall<SectStoryThreeVitalsCharacterType, bool>(-1, 19, 193, type, hasPlayedComeAnim);
			}

			// Token: 0x0601142F RID: 70703 RVA: 0x0067BF88 File Offset: 0x0067A188
			public static void GetResourceBlockProducingCoreCooldown(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 19, 194, blockKey);
			}

			// Token: 0x06011430 RID: 70704 RVA: 0x0067BF9A File Offset: 0x0067A19A
			public static void FeastAddDish(BuildingBlockKey blockKey, int index, ItemKey itemKey, ItemSourceType sourceType)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, int, ItemKey, ItemSourceType>(-1, 19, 195, blockKey, index, itemKey, sourceType);
			}

			// Token: 0x06011431 RID: 70705 RVA: 0x0067BFAF File Offset: 0x0067A1AF
			public static void FeastSetAutoRefill(BuildingBlockKey blockKey, bool value)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, bool>(-1, 19, 196, blockKey, value);
			}

			// Token: 0x06011432 RID: 70706 RVA: 0x0067BFC2 File Offset: 0x0067A1C2
			public static void GetFeast(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 19, 197, blockKey);
			}

			// Token: 0x06011433 RID: 70707 RVA: 0x0067BFD4 File Offset: 0x0067A1D4
			public static void FeastRemoveDish(BuildingBlockKey blockKey, int index)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, int>(-1, 19, 198, blockKey, index);
			}

			// Token: 0x06011434 RID: 70708 RVA: 0x0067BFE7 File Offset: 0x0067A1E7
			public static void FeastRemoveDish(BuildingBlockKey blockKey, int index, ItemSourceType sourceType)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, int, ItemSourceType>(-1, 19, 198, blockKey, index, sourceType);
			}

			// Token: 0x06011435 RID: 70709 RVA: 0x0067BFFB File Offset: 0x0067A1FB
			public static void FeastReceiveGift(BuildingBlockKey blockKey, int index)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, int>(-1, 19, 199, blockKey, index);
			}

			// Token: 0x06011436 RID: 70710 RVA: 0x0067C00E File Offset: 0x0067A20E
			public static void AddResourceItemToArtisanOrder(ArtisanOrder order, List<ItemDisplayData> itemList)
			{
				GameDataBridge.AddMethodCall<ArtisanOrder, List<ItemDisplayData>>(-1, 19, 200, order, itemList);
			}

			// Token: 0x06011437 RID: 70711 RVA: 0x0067C021 File Offset: 0x0067A221
			public static void IsFeastException(int listenerId, BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 19, 201, blockKey);
			}

			// Token: 0x06011438 RID: 70712 RVA: 0x0067C033 File Offset: 0x0067A233
			public static void UseFeastThanksLetter(ItemKey itemKey, int amount)
			{
				GameDataBridge.AddMethodCall<ItemKey, int>(-1, 19, 202, itemKey, amount);
			}

			// Token: 0x06011439 RID: 70713 RVA: 0x0067C046 File Offset: 0x0067A246
			public static void FeastQuickRefill(BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(-1, 19, 203, blockKey);
			}

			// Token: 0x0601143A RID: 70714 RVA: 0x0067C058 File Offset: 0x0067A258
			public static void FeastSetTargetType(BuildingBlockKey blockKey, short targetType)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, short>(-1, 19, 204, blockKey, targetType);
			}

			// Token: 0x0601143B RID: 70715 RVA: 0x0067C06B File Offset: 0x0067A26B
			public static void GetSectExtraLegacyBuildingStates(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 205);
			}

			// Token: 0x0601143C RID: 70716 RVA: 0x0067C07C File Offset: 0x0067A27C
			public static void SetJieqingGameData(SectStoryJieqingGame currJieqingGameState, SectStoryJieqingGame chapterJieqingGameState)
			{
				GameDataBridge.AddMethodCall<SectStoryJieqingGame, SectStoryJieqingGame>(-1, 19, 206, currJieqingGameState, chapterJieqingGameState);
			}

			// Token: 0x0601143D RID: 70717 RVA: 0x0067C08F File Offset: 0x0067A28F
			public static void ConsumeExtraLegacyPoint(short legacyTemplateId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 19, 207, legacyTemplateId);
			}

			// Token: 0x0601143E RID: 70718 RVA: 0x0067C0A1 File Offset: 0x0067A2A1
			public static void IsSectBuiltExtraLegacyBuilding(int listenerId, sbyte orgTemplateId)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 19, 208, orgTemplateId);
			}

			// Token: 0x0601143F RID: 70719 RVA: 0x0067C0B3 File Offset: 0x0067A2B3
			public static void InitJieqingGameData()
			{
				GameDataBridge.AddMethodCall(-1, 19, 209);
			}

			// Token: 0x06011440 RID: 70720 RVA: 0x0067C0C4 File Offset: 0x0067A2C4
			public static void RemoveSectExtraLegacyBuilding(sbyte orgTemplateId)
			{
				GameDataBridge.AddMethodCall<sbyte>(-1, 19, 210, orgTemplateId);
			}

			// Token: 0x06011441 RID: 70721 RVA: 0x0067C0D6 File Offset: 0x0067A2D6
			public static void BuildExtraLegacyBuilding(BuildingBlockKey blockKey)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(-1, 19, 211, blockKey);
			}

			// Token: 0x06011442 RID: 70722 RVA: 0x0067C0E8 File Offset: 0x0067A2E8
			public static void GetCharacterExtraLegacyPointWorth(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 212, charId);
			}

			// Token: 0x06011443 RID: 70723 RVA: 0x0067C0FA File Offset: 0x0067A2FA
			public static void GetExtraLegacyPointCharacterCountOnBlock(int listenerId, Location location, List<short> orgTemplateIds)
			{
				GameDataBridge.AddMethodCall<Location, List<short>>(listenerId, 19, 213, location, orgTemplateIds);
			}

			// Token: 0x06011444 RID: 70724 RVA: 0x0067C10D File Offset: 0x0067A30D
			public static void GetSectExtraLegacyBuildingCounts(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 214);
			}

			// Token: 0x06011445 RID: 70725 RVA: 0x0067C11E File Offset: 0x0067A31E
			public static void SaveMainUiCustomButtons(List<sbyte> customButtons)
			{
				GameDataBridge.AddMethodCall<List<sbyte>>(-1, 19, 215, customButtons);
			}

			// Token: 0x06011446 RID: 70726 RVA: 0x0067C130 File Offset: 0x0067A330
			public static void SetMapBlockCharCustomInfoList(List<short> templateIds)
			{
				GameDataBridge.AddMethodCall<List<short>>(-1, 19, 216, templateIds);
			}

			// Token: 0x06011447 RID: 70727 RVA: 0x0067C142 File Offset: 0x0067A342
			public static void SetMapBlockCharCustomButtonList(List<short> templateIds)
			{
				GameDataBridge.AddMethodCall<List<short>>(-1, 19, 217, templateIds);
			}

			// Token: 0x06011448 RID: 70728 RVA: 0x0067C154 File Offset: 0x0067A354
			public static void GetAreaCharacterJieQingSignAmount(int listenerId, int areaId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 218, areaId);
			}

			// Token: 0x06011449 RID: 70729 RVA: 0x0067C166 File Offset: 0x0067A366
			public static void CheckLocationHasBeggerSkill1(int listenerId, Location location)
			{
				GameDataBridge.AddMethodCall<Location>(listenerId, 19, 219, location);
			}

			// Token: 0x0601144A RID: 70730 RVA: 0x0067C178 File Offset: 0x0067A378
			public static void SetJixiDrainNeili(bool drainNeili)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 19, 220, drainNeili);
			}

			// Token: 0x0601144B RID: 70731 RVA: 0x0067C18A File Offset: 0x0067A38A
			public static void SetJixiTarget(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 19, 221, charId);
			}

			// Token: 0x0601144C RID: 70732 RVA: 0x0067C19C File Offset: 0x0067A39C
			public static void TaiwuTransferNeiliAllocToJixi(int amount, byte neiliType)
			{
				GameDataBridge.AddMethodCall<int, byte>(-1, 19, 222, amount, neiliType);
			}

			// Token: 0x0601144D RID: 70733 RVA: 0x0067C1AF File Offset: 0x0067A3AF
			public static void JixiTransferNeiliAllocToTaiwu(int amount, byte neiliType)
			{
				GameDataBridge.AddMethodCall<int, byte>(-1, 19, 223, amount, neiliType);
			}

			// Token: 0x0601144E RID: 70734 RVA: 0x0067C1C2 File Offset: 0x0067A3C2
			public static void SetJixiDrainType(sbyte neiliType)
			{
				GameDataBridge.AddMethodCall<sbyte>(-1, 19, 224, neiliType);
			}

			// Token: 0x0601144F RID: 70735 RVA: 0x0067C1D4 File Offset: 0x0067A3D4
			public static void GetJixiSpecialInteractDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 225);
			}

			// Token: 0x06011450 RID: 70736 RVA: 0x0067C1E5 File Offset: 0x0067A3E5
			public static void InitJixiSpecialInteractData()
			{
				GameDataBridge.AddMethodCall(-1, 19, 226);
			}

			// Token: 0x06011451 RID: 70737 RVA: 0x0067C1F6 File Offset: 0x0067A3F6
			public static void JixiRescueTaiwu()
			{
				GameDataBridge.AddMethodCall(-1, 19, 227);
			}

			// Token: 0x06011452 RID: 70738 RVA: 0x0067C207 File Offset: 0x0067A407
			public static void GetCharacterExtraLegacyPointWorthCalculated(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 228, charId);
			}

			// Token: 0x06011453 RID: 70739 RVA: 0x0067C219 File Offset: 0x0067A419
			public static void GetSectRanshanThreeCorpsesData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 229);
			}

			// Token: 0x06011454 RID: 70740 RVA: 0x0067C22A File Offset: 0x0067A42A
			public static void SetTaiwuTransformFiveElementsTarget(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 19, 230, charId);
			}

			// Token: 0x06011455 RID: 70741 RVA: 0x0067C23C File Offset: 0x0067A43C
			public static void SetTaiwuTargetFiveElementsType(sbyte fiveElementsType)
			{
				GameDataBridge.AddMethodCall<sbyte>(-1, 19, 231, fiveElementsType);
			}

			// Token: 0x06011456 RID: 70742 RVA: 0x0067C24E File Offset: 0x0067A44E
			public static void GetSectYuanshanThreeVitalsData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 19, 232);
			}

			// Token: 0x06011457 RID: 70743 RVA: 0x0067C25F File Offset: 0x0067A45F
			public static void RequestAllRecruitCharacterData(int listenerId, BuildingBlockKey targetBuildingBlock)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey>(listenerId, 19, 233, targetBuildingBlock);
			}

			// Token: 0x06011458 RID: 70744 RVA: 0x0067C271 File Offset: 0x0067A471
			public static void GetTipLegendaryBookDisplayData(int listenerId, sbyte type)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 19, 234, type);
			}

			// Token: 0x06011459 RID: 70745 RVA: 0x0067C283 File Offset: 0x0067A483
			public static void GetSectMembersWorthExtraLegacyPoint(int listenerId, sbyte orgTemplateId)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 19, 235, orgTemplateId);
			}

			// Token: 0x0601145A RID: 70746 RVA: 0x0067C295 File Offset: 0x0067A495
			public static void GetCharacterExtraLegacyPointWorthForMapBlock(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 19, 236, charId);
			}
		}

		// Token: 0x02002610 RID: 9744
		public static class AsyncCall
		{
			// Token: 0x0601145B RID: 70747 RVA: 0x0067C2A7 File Offset: 0x0067A4A7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetCombatSkillOrderPlan instead.", true)]
			public static void SetCombatSkillOrderPlan(IAsyncMethodRequestHandler requestHandler, ShortList plan, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601145C RID: 70748 RVA: 0x0067C2AF File Offset: 0x0067A4AF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.AddLocationMark instead.", true)]
			public static void AddLocationMark(IAsyncMethodRequestHandler requestHandler, Location location, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601145D RID: 70749 RVA: 0x0067C2B7 File Offset: 0x0067A4B7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.RemoveLocationMark instead.", true)]
			public static void RemoveLocationMark(IAsyncMethodRequestHandler requestHandler, Location location, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601145E RID: 70750 RVA: 0x0067C2C0 File Offset: 0x0067A4C0
			public static void AddReadingEventBookId(IAsyncMethodRequestHandler requestHandler, int id, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 3, id, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601145F RID: 70751 RVA: 0x0067C2EB File Offset: 0x0067A4EB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.RemoveReadingEventBookId instead.", true)]
			public static void RemoveReadingEventBookId(IAsyncMethodRequestHandler requestHandler, int id, short templateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011460 RID: 70752 RVA: 0x0067C2F4 File Offset: 0x0067A4F4
			public static void GetAllLifeSkillCombatUsedCard(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 5, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011461 RID: 70753 RVA: 0x0067C320 File Offset: 0x0067A520
			public static void GetAllLifeSkillCombatCard(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 6, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011462 RID: 70754 RVA: 0x0067C34A File Offset: 0x0067A54A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetLifeSkillCombatUsedCard instead.", true)]
			public static void SetLifeSkillCombatUsedCard(IAsyncMethodRequestHandler requestHandler, sbyte skillType, LifeSkillCombatCardCollection collection, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011463 RID: 70755 RVA: 0x0067C354 File Offset: 0x0067A554
			public static void GetCharacterLifeSkillCombatUsedCard(IAsyncMethodRequestHandler requestHandler, sbyte skillType, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte, int>(19, 8, skillType, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011464 RID: 70756 RVA: 0x0067C380 File Offset: 0x0067A580
			public static void GetLifeSkillCombatUsedCard(IAsyncMethodRequestHandler requestHandler, sbyte skillType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(19, 9, skillType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011465 RID: 70757 RVA: 0x0067C3AC File Offset: 0x0067A5AC
			public static void GetAllLifeSkillCombatNewCard(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 10, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011466 RID: 70758 RVA: 0x0067C3D7 File Offset: 0x0067A5D7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetLifeSkillCombatCardNotNew instead.", true)]
			public static void SetLifeSkillCombatCardNotNew(IAsyncMethodRequestHandler requestHandler, sbyte skillType, sbyte cardId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011467 RID: 70759 RVA: 0x0067C3E0 File Offset: 0x0067A5E0
			public static void GetCharTeammateCommands(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 12, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011468 RID: 70760 RVA: 0x0067C40C File Offset: 0x0067A60C
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetLegendaryBookWeaponSlot instead.", true)]
			public static void SetLegendaryBookWeaponSlot(IAsyncMethodRequestHandler requestHandler, sbyte skillType, ItemKey weaponKey, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011469 RID: 70761 RVA: 0x0067C414 File Offset: 0x0067A614
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetLegendaryBookSkillSlot instead.", true)]
			public static void SetLegendaryBookSkillSlot(IAsyncMethodRequestHandler requestHandler, sbyte skillType, int index, short skillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601146A RID: 70762 RVA: 0x0067C41C File Offset: 0x0067A61C
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.UnlockLegendaryBookBreakPlate instead.", true)]
			public static void UnlockLegendaryBookBreakPlate(IAsyncMethodRequestHandler requestHandler, sbyte skillType, bool isYin, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601146B RID: 70763 RVA: 0x0067C424 File Offset: 0x0067A624
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.UnlockLegendaryBookBonus instead.", true)]
			public static void UnlockLegendaryBookBonus(IAsyncMethodRequestHandler requestHandler, sbyte skillType, bool isYin, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601146C RID: 70764 RVA: 0x0067C42C File Offset: 0x0067A62C
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.EnterUnlockBreakPlateCombat instead.", true)]
			public static void EnterUnlockBreakPlateCombat(IAsyncMethodRequestHandler requestHandler, sbyte skillType, bool isYin, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601146D RID: 70765 RVA: 0x0067C434 File Offset: 0x0067A634
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.ExecuteActiveProfessionSkill instead.", true)]
			public static void ExecuteActiveProfessionSkill(IAsyncMethodRequestHandler requestHandler, int professionId, int skillIndex, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601146E RID: 70766 RVA: 0x0067C43C File Offset: 0x0067A63C
			public static void IsProfessionalSkillUnlocked(IAsyncMethodRequestHandler requestHandler, int professionId, int skillIndex, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int>(19, 19, professionId, skillIndex, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601146F RID: 70767 RVA: 0x0067C46C File Offset: 0x0067A66C
			public static void CanExecuteProfessionSkill(IAsyncMethodRequestHandler requestHandler, int professionId, int skillIndex, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int>(19, 20, professionId, skillIndex, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011470 RID: 70768 RVA: 0x0067C499 File Offset: 0x0067A699
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetProfessionTestSetting instead.", true)]
			public static void SetProfessionTestSetting(IAsyncMethodRequestHandler requestHandler, bool noSkillCooldown, bool noSkillCost, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011471 RID: 70769 RVA: 0x0067C4A4 File Offset: 0x0067A6A4
			public static void GetCharacterCustomDisplayName(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 22, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011472 RID: 70770 RVA: 0x0067C4D0 File Offset: 0x0067A6D0
			public static void GetTianJieFuLuCount(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 23, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011473 RID: 70771 RVA: 0x0067C4FB File Offset: 0x0067A6FB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.GmCmd_GenerateTreasure instead.", true)]
			public static void GmCmd_GenerateTreasure(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011474 RID: 70772 RVA: 0x0067C504 File Offset: 0x0067A704
			public static void FindTreasure(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 25, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011475 RID: 70773 RVA: 0x0067C530 File Offset: 0x0067A730
			public static void CheckSpecialCondition(IAsyncMethodRequestHandler requestHandler, int professionId, int skillIndex, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int>(19, 26, professionId, skillIndex, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011476 RID: 70774 RVA: 0x0067C55D File Offset: 0x0067A75D
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.ConfirmExecuteSkill instead.", true)]
			public static void ConfirmExecuteSkill(IAsyncMethodRequestHandler requestHandler, ProfessionSkillArg professionSkillArg, bool isFinished, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011477 RID: 70775 RVA: 0x0067C568 File Offset: 0x0067A768
			public static void FindTreasureExpect(IAsyncMethodRequestHandler requestHandler, Location location, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Location>(19, 28, location, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011478 RID: 70776 RVA: 0x0067C594 File Offset: 0x0067A794
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.UnlockAllProfessionSkills instead.", true)]
			public static void UnlockAllProfessionSkills(IAsyncMethodRequestHandler requestHandler, bool maxSeniority, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011479 RID: 70777 RVA: 0x0067C59C File Offset: 0x0067A79C
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetProfessionSeniorityTarget instead.", true)]
			public static void SetProfessionSeniorityTarget(IAsyncMethodRequestHandler requestHandler, int seniority, int professionId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601147A RID: 70778 RVA: 0x0067C5A4 File Offset: 0x0067A7A4
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.GmCmd_Profession_SetBuddhistMonkSavedSoulCount instead.", true)]
			public static void GmCmd_Profession_SetBuddhistMonkSavedSoulCount(IAsyncMethodRequestHandler requestHandler, int count, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601147B RID: 70779 RVA: 0x0067C5AC File Offset: 0x0067A7AC
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.GmCmd_Profession_SetTempleVisited instead.", true)]
			public static void GmCmd_Profession_SetTempleVisited(IAsyncMethodRequestHandler requestHandler, sbyte stateTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601147C RID: 70780 RVA: 0x0067C5B4 File Offset: 0x0067A7B4
			public static void InitAiLifeSkillCombatUsedCard(IAsyncMethodRequestHandler requestHandler, sbyte skillType, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte, int>(19, 33, skillType, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601147D RID: 70781 RVA: 0x0067C5E1 File Offset: 0x0067A7E1
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.GmCmd_Profession_RecoverHunterCarrierAttackCount instead.", true)]
			public static void GmCmd_Profession_RecoverHunterCarrierAttackCount(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601147E RID: 70782 RVA: 0x0067C5EC File Offset: 0x0067A7EC
			public static void GetBlockMerchantTypes(IAsyncMethodRequestHandler requestHandler, Location location, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Location>(19, 35, location, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601147F RID: 70783 RVA: 0x0067C618 File Offset: 0x0067A818
			public static void GetCharacterMasteredCombatSkills(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 36, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011480 RID: 70784 RVA: 0x0067C644 File Offset: 0x0067A844
			public static void AddCharacterMasteredCombatSkill(IAsyncMethodRequestHandler requestHandler, int charId, short skillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short>(19, 37, charId, skillTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011481 RID: 70785 RVA: 0x0067C674 File Offset: 0x0067A874
			public static void RemoveCharacterMasteredCombatSkill(IAsyncMethodRequestHandler requestHandler, int charId, short skillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short>(19, 38, charId, skillTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011482 RID: 70786 RVA: 0x0067C6A1 File Offset: 0x0067A8A1
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.InvokeFindExtraTreasureEvent instead.", true)]
			public static void InvokeFindExtraTreasureEvent(IAsyncMethodRequestHandler requestHandler, TreasureFindResult result, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011483 RID: 70787 RVA: 0x0067C6AC File Offset: 0x0067A8AC
			public static void SetAdvancedTeammateCommands(IAsyncMethodRequestHandler requestHandler, int charId, sbyte cmdType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, sbyte>(19, 40, charId, cmdType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011484 RID: 70788 RVA: 0x0067C6DC File Offset: 0x0067A8DC
			public static void CancelAdvancedTeammateCommands(IAsyncMethodRequestHandler requestHandler, int charId, sbyte cmdType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, sbyte>(19, 41, charId, cmdType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011485 RID: 70789 RVA: 0x0067C70C File Offset: 0x0067A90C
			public static void GetAllHeavenlyTrees(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 42, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011486 RID: 70790 RVA: 0x0067C738 File Offset: 0x0067A938
			public static void GetHeavenlyTreeNearBlocks(IAsyncMethodRequestHandler requestHandler, int id, int maxSteps, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int>(19, 43, id, maxSteps, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011487 RID: 70791 RVA: 0x0067C768 File Offset: 0x0067A968
			public static void GetInformationSettings(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 44, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011488 RID: 70792 RVA: 0x0067C794 File Offset: 0x0067A994
			public static void GetPoisonImmunities(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 45, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011489 RID: 70793 RVA: 0x0067C7C0 File Offset: 0x0067A9C0
			public static void GetDreamBackTaiwuRelatedCharactersForRelations(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 46, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601148A RID: 70794 RVA: 0x0067C7EC File Offset: 0x0067A9EC
			public static void GetDreamBackTaiwuGenealogy(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 47, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601148B RID: 70795 RVA: 0x0067C818 File Offset: 0x0067AA18
			public static void GetCharacterDisplayDataListForDreamBackRelations(IAsyncMethodRequestHandler requestHandler, List<int> charIds, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(19, 48, charIds, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601148C RID: 70796 RVA: 0x0067C844 File Offset: 0x0067AA44
			public static void GetDreamBackLifeRecordByDate(IAsyncMethodRequestHandler requestHandler, int startDate, int monthCount, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int>(19, 49, startDate, monthCount, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601148D RID: 70797 RVA: 0x0067C874 File Offset: 0x0067AA74
			public static void GetNameAndLifeRelatedDataListForDreamBack(IAsyncMethodRequestHandler requestHandler, List<int> charIds, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(19, 50, charIds, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601148E RID: 70798 RVA: 0x0067C8A0 File Offset: 0x0067AAA0
			public static void IsCharacterHatingItemRevealed(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 51, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601148F RID: 70799 RVA: 0x0067C8CC File Offset: 0x0067AACC
			public static void IsCharacterLovingItemRevealed(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 52, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011490 RID: 70800 RVA: 0x0067C8F8 File Offset: 0x0067AAF8
			public static void IsCharacterHobbyRevealed(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 53, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011491 RID: 70801 RVA: 0x0067C924 File Offset: 0x0067AB24
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetCharacterRevealedHobbies instead.", true)]
			public static void SetCharacterRevealedHobbies(IAsyncMethodRequestHandler requestHandler, int charId, bool isLovingItem, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011492 RID: 70802 RVA: 0x0067C92C File Offset: 0x0067AB2C
			public static void GetConflictCombatSkill(IAsyncMethodRequestHandler requestHandler, short templateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(19, 55, templateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011493 RID: 70803 RVA: 0x0067C958 File Offset: 0x0067AB58
			public static void GetAllDreamBackLifeRecords(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 56, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011494 RID: 70804 RVA: 0x0067C984 File Offset: 0x0067AB84
			public static void GetDreamBackTaiwuBirthAndEndDates(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 57, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011495 RID: 70805 RVA: 0x0067C9B0 File Offset: 0x0067ABB0
			public static void IsCurrentTaiwuOverwrittenByDreamBack(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 58, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011496 RID: 70806 RVA: 0x0067C9DB File Offset: 0x0067ABDB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.ApplyConflictCombatSkillResult instead.", true)]
			public static void ApplyConflictCombatSkillResult(IAsyncMethodRequestHandler requestHandler, short templateId, bool overwrite, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011497 RID: 70807 RVA: 0x0067C9E4 File Offset: 0x0067ABE4
			public static void HaveConflictCombatSkill(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 60, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011498 RID: 70808 RVA: 0x0067CA0F File Offset: 0x0067AC0F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.AddTaiwuOneWayRelationCoolDown instead.", true)]
			public static void AddTaiwuOneWayRelationCoolDown(IAsyncMethodRequestHandler requestHandler, int charId, bool isAdoreRelation, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011499 RID: 70809 RVA: 0x0067CA18 File Offset: 0x0067AC18
			public static void IsTaiwuAbleToAddOneWayRelation(IAsyncMethodRequestHandler requestHandler, int charId, bool isAdoreRelation, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, bool>(19, 62, charId, isAdoreRelation, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601149A RID: 70810 RVA: 0x0067CA48 File Offset: 0x0067AC48
			public static void GetTaiwuAddOneWayRelationCoolDown(IAsyncMethodRequestHandler requestHandler, int charId, bool isAdoreRelation, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, bool>(19, 63, charId, isAdoreRelation, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601149B RID: 70811 RVA: 0x0067CA75 File Offset: 0x0067AC75
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.FeedCarrier instead.", true)]
			public static void FeedCarrier(IAsyncMethodRequestHandler requestHandler, ItemKey carrier, ItemKey food, int count, ItemSourceType sourceType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601149C RID: 70812 RVA: 0x0067CA80 File Offset: 0x0067AC80
			public static void GetCarrierTamePoint(IAsyncMethodRequestHandler requestHandler, int carrierId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 65, carrierId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601149D RID: 70813 RVA: 0x0067CAAC File Offset: 0x0067ACAC
			public static void GetDreamBackCharacterDisplayDataList(IAsyncMethodRequestHandler requestHandler, List<int> charIdList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(19, 66, charIdList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601149E RID: 70814 RVA: 0x0067CAD8 File Offset: 0x0067ACD8
			public static void GetCarrierMaxTamePoint(IAsyncMethodRequestHandler requestHandler, int carrierId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 67, carrierId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601149F RID: 70815 RVA: 0x0067CB04 File Offset: 0x0067AD04
			public static void GetCurrMaxJiaoPoolCount(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 68, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114A0 RID: 70816 RVA: 0x0067CB30 File Offset: 0x0067AD30
			public static void GmCmd_FindFiveLoongLocation(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 69, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114A1 RID: 70817 RVA: 0x0067CB5C File Offset: 0x0067AD5C
			public static void GetJiaoPoolBlockStyle(IAsyncMethodRequestHandler requestHandler, int poolId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 70, poolId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114A2 RID: 70818 RVA: 0x0067CB88 File Offset: 0x0067AD88
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetJiaoPoolBlockStyle instead.", true)]
			public static void SetJiaoPoolBlockStyle(IAsyncMethodRequestHandler requestHandler, int poolId, short style, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114A3 RID: 70819 RVA: 0x0067CB90 File Offset: 0x0067AD90
			public static void GetChildrenOfLoongById(IAsyncMethodRequestHandler requestHandler, int id, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 72, id, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114A4 RID: 70820 RVA: 0x0067CBBC File Offset: 0x0067ADBC
			public static void GetJiaoPoolList(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 73, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114A5 RID: 70821 RVA: 0x0067CBE8 File Offset: 0x0067ADE8
			public static void GetJiaoById(IAsyncMethodRequestHandler requestHandler, int id, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 74, id, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114A6 RID: 70822 RVA: 0x0067CC14 File Offset: 0x0067AE14
			public static void GetJiaoPoolAllJiaoData(IAsyncMethodRequestHandler requestHandler, int poolId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 75, poolId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114A7 RID: 70823 RVA: 0x0067CC40 File Offset: 0x0067AE40
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.PutJiaoInPool instead.", true)]
			public static void PutJiaoInPool(IAsyncMethodRequestHandler requestHandler, int poolId, int id, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114A8 RID: 70824 RVA: 0x0067CC48 File Offset: 0x0067AE48
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.PutAnotherJiaoInPool instead.", true)]
			public static void PutAnotherJiaoInPool(IAsyncMethodRequestHandler requestHandler, int poolId, int id, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114A9 RID: 70825 RVA: 0x0067CC50 File Offset: 0x0067AE50
			public static void PutJiaoOutOfPool(IAsyncMethodRequestHandler requestHandler, int poolId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 78, poolId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114AA RID: 70826 RVA: 0x0067CC7C File Offset: 0x0067AE7C
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.ChangeNurturance instead.", true)]
			public static void ChangeNurturance(IAsyncMethodRequestHandler requestHandler, int poolId, short templateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114AB RID: 70827 RVA: 0x0067CC84 File Offset: 0x0067AE84
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.ChangeJiaoName instead.", true)]
			public static void ChangeJiaoName(IAsyncMethodRequestHandler requestHandler, int poolId, string name, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114AC RID: 70828 RVA: 0x0067CC8C File Offset: 0x0067AE8C
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.DisableJiaoPool instead.", true)]
			public static void DisableJiaoPool(IAsyncMethodRequestHandler requestHandler, int poolId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114AD RID: 70829 RVA: 0x0067CC94 File Offset: 0x0067AE94
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.EnableJiaoPool instead.", true)]
			public static void EnableJiaoPool(IAsyncMethodRequestHandler requestHandler, int poolId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114AE RID: 70830 RVA: 0x0067CC9C File Offset: 0x0067AE9C
			public static void GetJiaoLoongNameRelatedDataList(IAsyncMethodRequestHandler requestHandler, List<int> jiaoLoongIds, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(19, 83, jiaoLoongIds, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114AF RID: 70831 RVA: 0x0067CCC8 File Offset: 0x0067AEC8
			public static void GetAllJiaoForPool(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 84, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114B0 RID: 70832 RVA: 0x0067CCF4 File Offset: 0x0067AEF4
			public static void GetAllJiaoForEvolve(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 85, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114B1 RID: 70833 RVA: 0x0067CD20 File Offset: 0x0067AF20
			public static void GetJiaoByItemKey(IAsyncMethodRequestHandler requestHandler, ItemKey key, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey>(19, 86, key, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114B2 RID: 70834 RVA: 0x0067CD4C File Offset: 0x0067AF4C
			public static void GetJiaosByItemKeys(IAsyncMethodRequestHandler requestHandler, List<ItemKey> keys, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<ItemKey>>(19, 87, keys, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114B3 RID: 70835 RVA: 0x0067CD78 File Offset: 0x0067AF78
			public static void GetChildrenOfLoongByItemKey(IAsyncMethodRequestHandler requestHandler, ItemKey key, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey>(19, 88, key, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114B4 RID: 70836 RVA: 0x0067CDA4 File Offset: 0x0067AFA4
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.PutEggIntoPool instead.", true)]
			public static void PutEggIntoPool(IAsyncMethodRequestHandler requestHandler, int poolId, ItemKey eggItemKey, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114B5 RID: 70837 RVA: 0x0067CDAC File Offset: 0x0067AFAC
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.JiaoPoolInteract instead.", true)]
			public static void JiaoPoolInteract(IAsyncMethodRequestHandler requestHandler, int poolId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114B6 RID: 70838 RVA: 0x0067CDB4 File Offset: 0x0067AFB4
			public static void GmCmd_AddJiao(IAsyncMethodRequestHandler requestHandler, short templateId, short nurturanceTemplateId, sbyte growthStage, bool isMale, bool isMaxTamePoint, bool isMaxProperty, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, short, sbyte, bool, bool, bool>(19, 91, templateId, nurturanceTemplateId, growthStage, isMale, isMaxTamePoint, isMaxProperty, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114B7 RID: 70839 RVA: 0x0067CDE9 File Offset: 0x0067AFE9
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.GmCmd_PutJiaoInFirstPool instead.", true)]
			public static void GmCmd_PutJiaoInFirstPool(IAsyncMethodRequestHandler requestHandler, int jiaoId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114B8 RID: 70840 RVA: 0x0067CDF4 File Offset: 0x0067AFF4
			public static void GmCmd_AddChildOfLoong(IAsyncMethodRequestHandler requestHandler, short templateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(19, 93, templateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114B9 RID: 70841 RVA: 0x0067CE20 File Offset: 0x0067B020
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.JiaoEvolveToChildOfLoong instead.", true)]
			public static void JiaoEvolveToChildOfLoong(IAsyncMethodRequestHandler requestHandler, ItemKey key, short childOfLoongTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114BA RID: 70842 RVA: 0x0067CE28 File Offset: 0x0067B028
			public static void GetJiaoEvolutionChoice(IAsyncMethodRequestHandler requestHandler, ItemKey key, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey>(19, 95, key, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114BB RID: 70843 RVA: 0x0067CE54 File Offset: 0x0067B054
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.ResetJiaoPoolStatus instead.", true)]
			public static void ResetJiaoPoolStatus(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114BC RID: 70844 RVA: 0x0067CE5C File Offset: 0x0067B05C
			public static void GetAllAdultJiao(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 97, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114BD RID: 70845 RVA: 0x0067CE88 File Offset: 0x0067B088
			public static void GetAllEvolvingJiao(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 98, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114BE RID: 70846 RVA: 0x0067CEB4 File Offset: 0x0067B0B4
			public static void GetJiaoTemplateIdByCarrierTemplateId(IAsyncMethodRequestHandler requestHandler, short templateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(19, 99, templateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114BF RID: 70847 RVA: 0x0067CEE0 File Offset: 0x0067B0E0
			public static void CalcResourceChangeByJiaoPool(IAsyncMethodRequestHandler requestHandler, sbyte resourceType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(19, 100, resourceType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114C0 RID: 70848 RVA: 0x0067CF0C File Offset: 0x0067B10C
			public static void IsOwnedChildrenOfLoong(IAsyncMethodRequestHandler requestHandler, short templateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(19, 101, templateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114C1 RID: 70849 RVA: 0x0067CF38 File Offset: 0x0067B138
			public static void GetNextRandomChildrenOfLoong(IAsyncMethodRequestHandler requestHandler, int jiaoId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 102, jiaoId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114C2 RID: 70850 RVA: 0x0067CF64 File Offset: 0x0067B164
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.GmCmd_AddFleeCarrier instead.", true)]
			public static void GmCmd_AddFleeCarrier(IAsyncMethodRequestHandler requestHandler, bool isJiaoLoong, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114C3 RID: 70851 RVA: 0x0067CF6C File Offset: 0x0067B16C
			public static void GetIsJiaoPoolOpen(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 104, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114C4 RID: 70852 RVA: 0x0067CF98 File Offset: 0x0067B198
			public static void FillJiaoRecordArgumentCollection(IAsyncMethodRequestHandler requestHandler, JiaoPoolRecordArgumentCollection collection, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<JiaoPoolRecordArgumentCollection>(19, 105, collection, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114C5 RID: 70853 RVA: 0x0067CFC4 File Offset: 0x0067B1C4
			public static void GetJiaoEvolutionPageStatus(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 106, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114C6 RID: 70854 RVA: 0x0067CFF0 File Offset: 0x0067B1F0
			public static void GetIsBabysittingMode(IAsyncMethodRequestHandler requestHandler, int poolId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 107, poolId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114C7 RID: 70855 RVA: 0x0067D01C File Offset: 0x0067B21C
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetIsBabysittingMode instead.", true)]
			public static void SetIsBabysittingMode(IAsyncMethodRequestHandler requestHandler, int poolId, bool isOn, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114C8 RID: 70856 RVA: 0x0067D024 File Offset: 0x0067B224
			public static void GetFiveLoongDictCount(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 109, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114C9 RID: 70857 RVA: 0x0067D050 File Offset: 0x0067B250
			public static void GetJiaoLoongNameRelatedData(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey>(19, 110, itemKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114CA RID: 70858 RVA: 0x0067D07C File Offset: 0x0067B27C
			public static void IsJiaoAbleToPet(IAsyncMethodRequestHandler requestHandler, int jiaoId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 111, jiaoId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114CB RID: 70859 RVA: 0x0067D0A8 File Offset: 0x0067B2A8
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.PetJiao instead.", true)]
			public static void PetJiao(IAsyncMethodRequestHandler requestHandler, int jiaoId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114CC RID: 70860 RVA: 0x0067D0B0 File Offset: 0x0067B2B0
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.JiaoPoolPetJiao instead.", true)]
			public static void JiaoPoolPetJiao(IAsyncMethodRequestHandler requestHandler, int poolId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114CD RID: 70861 RVA: 0x0067D0B8 File Offset: 0x0067B2B8
			public static void GetTaiwuAddOneWayRelationResultCode(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 114, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114CE RID: 70862 RVA: 0x0067D0E4 File Offset: 0x0067B2E4
			public static void RequestRecruitCharacterData(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey buildingBlockKey, int earningDataIndex, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, int>(19, 115, buildingBlockKey, earningDataIndex, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114CF RID: 70863 RVA: 0x0067D114 File Offset: 0x0067B314
			public static void RequestRecruitCharacterData(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey buildingBlockKey, int earningDataIndex, bool autoRemove, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey, int, bool>(19, 115, buildingBlockKey, earningDataIndex, autoRemove, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114D0 RID: 70864 RVA: 0x0067D143 File Offset: 0x0067B343
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.GmCmd_AddThreeCorpses instead.", true)]
			public static void GmCmd_AddThreeCorpses(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114D1 RID: 70865 RVA: 0x0067D14B File Offset: 0x0067B34B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.ApplyRanshanThreeCorpsesLegendaryBookKeepingResult instead.", true)]
			public static void ApplyRanshanThreeCorpsesLegendaryBookKeepingResult(IAsyncMethodRequestHandler requestHandler, List<sbyte> huajuBooks, List<sbyte> xuanzhiBooks, List<sbyte> yingjiaoBooks, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114D2 RID: 70866 RVA: 0x0067D154 File Offset: 0x0067B354
			public static void GetItemListForRanshanTreeCorpsesLegendaryBookKeeping(IAsyncMethodRequestHandler requestHandler, int corpseId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 118, corpseId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114D3 RID: 70867 RVA: 0x0067D180 File Offset: 0x0067B380
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.GmCmd_AddDisplayEventLegendaryBookKeeping instead.", true)]
			public static void GmCmd_AddDisplayEventLegendaryBookKeeping(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114D4 RID: 70868 RVA: 0x0067D188 File Offset: 0x0067B388
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetRanshanThreeCorpsesCharacterTarget instead.", true)]
			public static void SetRanshanThreeCorpsesCharacterTarget(IAsyncMethodRequestHandler requestHandler, short templateId, sbyte targetId, sbyte notch, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114D5 RID: 70869 RVA: 0x0067D190 File Offset: 0x0067B390
			public static void GetBookStrategiesExpireTime(IAsyncMethodRequestHandler requestHandler, ItemKey book, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey>(19, 121, book, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114D6 RID: 70870 RVA: 0x0067D1BC File Offset: 0x0067B3BC
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetMonthlyNotificationSortingGroup instead.", true)]
			public static void SetMonthlyNotificationSortingGroup(IAsyncMethodRequestHandler requestHandler, NotificationSortingGroup group, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114D7 RID: 70871 RVA: 0x0067D1C4 File Offset: 0x0067B3C4
			public static void SetCharTeammateCommandsManual(IAsyncMethodRequestHandler requestHandler, int charId, List<sbyte> cmdTypes, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, List<sbyte>>(19, 123, charId, cmdTypes, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114D8 RID: 70872 RVA: 0x0067D1F4 File Offset: 0x0067B3F4
			public static void GetCharAdvancedTeammateCommands(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 124, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114D9 RID: 70873 RVA: 0x0067D220 File Offset: 0x0067B420
			public static void IsStoneRoomFull(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 125, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114DA RID: 70874 RVA: 0x0067D24C File Offset: 0x0067B44C
			public static void ExtinguishFulongInFlameArea(IAsyncMethodRequestHandler requestHandler, short blockId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(19, 126, blockId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114DB RID: 70875 RVA: 0x0067D278 File Offset: 0x0067B478
			public static void ExtinguishFulongInFlameArea(IAsyncMethodRequestHandler requestHandler, short blockId, bool isEvent, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, bool>(19, 126, blockId, isEvent, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114DC RID: 70876 RVA: 0x0067D2A5 File Offset: 0x0067B4A5
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.TriggerFulongInFlameAreaMine instead.", true)]
			public static void TriggerFulongInFlameAreaMine(IAsyncMethodRequestHandler requestHandler, short blockId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114DD RID: 70877 RVA: 0x0067D2AD File Offset: 0x0067B4AD
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.ApplyFulongInFlameAreaFullyExtinguished instead.", true)]
			public static void ApplyFulongInFlameAreaFullyExtinguished(IAsyncMethodRequestHandler requestHandler, int index, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114DE RID: 70878 RVA: 0x0067D2B5 File Offset: 0x0067B4B5
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.ApplyFulongInFlameAreaFullyExtinguished instead.", true)]
			public static void ApplyFulongInFlameAreaFullyExtinguished(IAsyncMethodRequestHandler requestHandler, int index, bool triggerEvent, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114DF RID: 70879 RVA: 0x0067D2BD File Offset: 0x0067B4BD
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.GmCmd_GenerateFulongFlameArea instead.", true)]
			public static void GmCmd_GenerateFulongFlameArea(IAsyncMethodRequestHandler requestHandler, int radius, int mineCount, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114E0 RID: 70880 RVA: 0x0067D2C5 File Offset: 0x0067B4C5
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.HunterSkill_AnimalCharacterToItem instead.", true)]
			public static void HunterSkill_AnimalCharacterToItem(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114E1 RID: 70881 RVA: 0x0067D2CD File Offset: 0x0067B4CD
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.ConfirmProfessionSkillsEquipment instead.", true)]
			public static void ConfirmProfessionSkillsEquipment(IAsyncMethodRequestHandler requestHandler, TaiwuProfessionSkillSlots slots, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114E2 RID: 70882 RVA: 0x0067D2D8 File Offset: 0x0067B4D8
			public static void GmCmd_CastTasterUltimateOnCurrentBlock(IAsyncMethodRequestHandler requestHandler, bool isCombatSkill, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool>(19, 132, isCombatSkill, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114E3 RID: 70883 RVA: 0x0067D307 File Offset: 0x0067B507
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.EatTianJieFuLu instead.", true)]
			public static void EatTianJieFuLu(IAsyncMethodRequestHandler requestHandler, int charId, ItemKey itemKey, int amount, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114E4 RID: 70884 RVA: 0x0067D310 File Offset: 0x0067B510
			public static void CheckAristocratUltimateSpecialCondition(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 134, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114E5 RID: 70885 RVA: 0x0067D340 File Offset: 0x0067B540
			public static void CheckBeggarUltimateSpecialCondition(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 135, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114E6 RID: 70886 RVA: 0x0067D370 File Offset: 0x0067B570
			public static void CheckTasterUltimateSpecialCondition(IAsyncMethodRequestHandler requestHandler, bool isCombatSkill, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool>(19, 136, isCombatSkill, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114E7 RID: 70887 RVA: 0x0067D3A0 File Offset: 0x0067B5A0
			public static void GM_GetFriendOrFamilySendGift(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 137, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114E8 RID: 70888 RVA: 0x0067D3CF File Offset: 0x0067B5CF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.GmCmd_CreateGearMate instead.", true)]
			public static void GmCmd_CreateGearMate(IAsyncMethodRequestHandler requestHandler, int templateId, string name, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114E9 RID: 70889 RVA: 0x0067D3D8 File Offset: 0x0067B5D8
			public static void GetGearMateRepairEffect(IAsyncMethodRequestHandler requestHandler, int gearMateId, int artisanId, sbyte type, ItemKey toolKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int, sbyte, ItemKey>(19, 139, gearMateId, artisanId, type, toolKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114EA RID: 70890 RVA: 0x0067D40C File Offset: 0x0067B60C
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.RepairGearMate instead.", true)]
			public static void RepairGearMate(IAsyncMethodRequestHandler requestHandler, int gearMateId, int artisanId, sbyte type, ItemKey toolKey, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114EB RID: 70891 RVA: 0x0067D414 File Offset: 0x0067B614
			public static void GetGearMateRepairRequirement(IAsyncMethodRequestHandler requestHandler, int gearMateId, int artisanId, sbyte type, ItemKey toolKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int, sbyte, ItemKey>(19, 141, gearMateId, artisanId, type, toolKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114EC RID: 70892 RVA: 0x0067D448 File Offset: 0x0067B648
			public static void GetGearMateAvailableRepairCount(IAsyncMethodRequestHandler requestHandler, int artisanId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 142, artisanId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114ED RID: 70893 RVA: 0x0067D478 File Offset: 0x0067B678
			public static void GetGearMateRepairRequirementDisplayDatas(IAsyncMethodRequestHandler requestHandler, int gearMateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 143, gearMateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114EE RID: 70894 RVA: 0x0067D4A7 File Offset: 0x0067B6A7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.UpgradeGearMate instead.", true)]
			public static void UpgradeGearMate(IAsyncMethodRequestHandler requestHandler, int charId, sbyte type, ItemKey itemKey, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114EF RID: 70895 RVA: 0x0067D4AF File Offset: 0x0067B6AF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.UpgradeGearMate instead.", true)]
			public static void UpgradeGearMate(IAsyncMethodRequestHandler requestHandler, int charId, sbyte type, ItemKey itemKey, int count, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114F0 RID: 70896 RVA: 0x0067D4B7 File Offset: 0x0067B6B7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.UpgradeGearMate instead.", true)]
			public static void UpgradeGearMate(IAsyncMethodRequestHandler requestHandler, int charId, sbyte type, ItemKey itemKey, int count, ItemSourceType sourceType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114F1 RID: 70897 RVA: 0x0067D4C0 File Offset: 0x0067B6C0
			public static void GetCharacterConsummateLevelProgress(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 145, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114F2 RID: 70898 RVA: 0x0067D4F0 File Offset: 0x0067B6F0
			public static void GetMartialArtistCreateGoodRandomEnemyAndBadRandomEnemyCount(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 146, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114F3 RID: 70899 RVA: 0x0067D520 File Offset: 0x0067B720
			public static void GetGearMateById(IAsyncMethodRequestHandler requestHandler, int artisanId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 147, artisanId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114F4 RID: 70900 RVA: 0x0067D550 File Offset: 0x0067B750
			public static void CheckSpecialCondition_SavageSkill_1(IAsyncMethodRequestHandler requestHandler, ProfessionData professionData, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ProfessionData>(19, 148, professionData, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114F5 RID: 70901 RVA: 0x0067D580 File Offset: 0x0067B780
			public static void GetMerchantExtraGoods(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 149, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114F6 RID: 70902 RVA: 0x0067D5AF File Offset: 0x0067B7AF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetProfessionExtraSeniority instead.", true)]
			public static void SetProfessionExtraSeniority(IAsyncMethodRequestHandler requestHandler, int professionId, int extraSeniority, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114F7 RID: 70903 RVA: 0x0067D5B8 File Offset: 0x0067B7B8
			public static void CanShowProfessionSkillUnlocked(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 151, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114F8 RID: 70904 RVA: 0x0067D5E8 File Offset: 0x0067B7E8
			public static void GetGearMateBreakoutCombatSkillBanReasonList(IAsyncMethodRequestHandler requestHandler, int gearMateId, List<short> checkCombatSkillList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, List<short>>(19, 152, gearMateId, checkCombatSkillList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114F9 RID: 70905 RVA: 0x0067D618 File Offset: 0x0067B818
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetDukeSkill3Crickets instead.", true)]
			public static void SetDukeSkill3Crickets(IAsyncMethodRequestHandler requestHandler, List<short> effectBlocks, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060114FA RID: 70906 RVA: 0x0067D620 File Offset: 0x0067B820
			public static void GetAllSkillBooksGearMateCanRead(IAsyncMethodRequestHandler requestHandler, bool isCombatSkill, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool>(19, 154, isCombatSkill, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114FB RID: 70907 RVA: 0x0067D650 File Offset: 0x0067B850
			public static void CanIdentifyCricket(IAsyncMethodRequestHandler requestHandler, int cricketId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 155, cricketId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114FC RID: 70908 RVA: 0x0067D680 File Offset: 0x0067B880
			public static void CanUpgradeCricket(IAsyncMethodRequestHandler requestHandler, int cricketId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 156, cricketId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114FD RID: 70909 RVA: 0x0067D6B0 File Offset: 0x0067B8B0
			public static void CanConvertToAnimalCharacter(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey>(19, 157, itemKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114FE RID: 70910 RVA: 0x0067D6E0 File Offset: 0x0067B8E0
			public static void GetJiaoLoongDisplayDataByItemKey(IAsyncMethodRequestHandler requestHandler, ItemKey key, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey>(19, 158, key, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060114FF RID: 70911 RVA: 0x0067D70F File Offset: 0x0067B90F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.GmCmd_SetCharacterProficiencies instead.", true)]
			public static void GmCmd_SetCharacterProficiencies(IAsyncMethodRequestHandler requestHandler, int charId, short skillId, int value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011500 RID: 70912 RVA: 0x0067D717 File Offset: 0x0067B917
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.GmCmd_CreateRandomEnemyAroundHeavenlyTree instead.", true)]
			public static void GmCmd_CreateRandomEnemyAroundHeavenlyTree(IAsyncMethodRequestHandler requestHandler, int count, int range, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011501 RID: 70913 RVA: 0x0067D71F File Offset: 0x0067B91F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.GmCmd_ShowUnlockedProfessionSkill instead.", true)]
			public static void GmCmd_ShowUnlockedProfessionSkill(IAsyncMethodRequestHandler requestHandler, int skillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011502 RID: 70914 RVA: 0x0067D727 File Offset: 0x0067B927
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetVillagerRoleAutoActionState instead.", true)]
			public static void SetVillagerRoleAutoActionState(IAsyncMethodRequestHandler requestHandler, short roleTemplateId, ulong actionStates, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011503 RID: 70915 RVA: 0x0067D72F File Offset: 0x0067B92F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.ChangeBuildingArrangementSettingPresetData instead.", true)]
			public static void ChangeBuildingArrangementSettingPresetData(IAsyncMethodRequestHandler requestHandler, int index, BuildingOptionAutoGiveMemberPreset preset, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011504 RID: 70916 RVA: 0x0067D737 File Offset: 0x0067B937
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.AddMaterialToArtisanOrder instead.", true)]
			public static void AddMaterialToArtisanOrder(IAsyncMethodRequestHandler requestHandler, ArtisanOrder order, List<ItemDisplayData> itemList, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011505 RID: 70917 RVA: 0x0067D740 File Offset: 0x0067B940
			public static void GetArtisanOrderProductionPool(IAsyncMethodRequestHandler requestHandler, ArtisanOrder order, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ArtisanOrder>(19, 165, order, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011506 RID: 70918 RVA: 0x0067D76F File Offset: 0x0067B96F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetArtisanOrderProductionType instead.", true)]
			public static void SetArtisanOrderProductionType(IAsyncMethodRequestHandler requestHandler, ArtisanOrder order, short itemSubType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011507 RID: 70919 RVA: 0x0067D777 File Offset: 0x0067B977
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetArtisanOrderStorageType instead.", true)]
			public static void SetArtisanOrderStorageType(IAsyncMethodRequestHandler requestHandler, ArtisanOrder order, ItemSourceType type, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011508 RID: 70920 RVA: 0x0067D780 File Offset: 0x0067B980
			public static void GetNpcArtisanOrder(IAsyncMethodRequestHandler requestHandler, int artisanId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 168, artisanId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011509 RID: 70921 RVA: 0x0067D7AF File Offset: 0x0067B9AF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.InterceptArtisanOrder instead.", true)]
			public static void InterceptArtisanOrder(IAsyncMethodRequestHandler requestHandler, int artisanId, int subscriberId, bool isDebateWon, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601150A RID: 70922 RVA: 0x0067D7B8 File Offset: 0x0067B9B8
			public static void GetBuildingArtisanOrder(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(19, 170, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601150B RID: 70923 RVA: 0x0067D7E7 File Offset: 0x0067B9E7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.CreateArtisanOrder instead.", true)]
			public static void CreateArtisanOrder(IAsyncMethodRequestHandler requestHandler, int artisanId, int subscriberId, sbyte lifeSkillType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601150C RID: 70924 RVA: 0x0067D7EF File Offset: 0x0067B9EF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.CreateArtisanOrder instead.", true)]
			public static void CreateArtisanOrder(IAsyncMethodRequestHandler requestHandler, int artisanId, int subscriberId, sbyte lifeSkillType, short makeItemSubType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601150D RID: 70925 RVA: 0x0067D7F8 File Offset: 0x0067B9F8
			public static void GetProductionPoolPreview(IAsyncMethodRequestHandler requestHandler, int artisanId, sbyte lifeSkillType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, sbyte>(19, 172, artisanId, lifeSkillType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601150E RID: 70926 RVA: 0x0067D828 File Offset: 0x0067BA28
			public static void GetProductionPoolPreview(IAsyncMethodRequestHandler requestHandler, int artisanId, sbyte lifeSkillType, short itemSubType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, sbyte, short>(19, 172, artisanId, lifeSkillType, itemSubType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601150F RID: 70927 RVA: 0x0067D85A File Offset: 0x0067BA5A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.ArtisanOrderDebate instead.", true)]
			public static void ArtisanOrderDebate(IAsyncMethodRequestHandler requestHandler, int artisanId, bool isDebateWon, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011510 RID: 70928 RVA: 0x0067D864 File Offset: 0x0067BA64
			public static void GetArtisanOrderMaterialPreview(IAsyncMethodRequestHandler requestHandler, ArtisanOrder order, List<ItemDisplayData> itemList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ArtisanOrder, List<ItemDisplayData>>(19, 174, order, itemList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011511 RID: 70929 RVA: 0x0067D894 File Offset: 0x0067BA94
			public static void GetArtisanOrderCanProduceItemSubType(IAsyncMethodRequestHandler requestHandler, ArtisanOrder order, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ArtisanOrder>(19, 175, order, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011512 RID: 70930 RVA: 0x0067D8C3 File Offset: 0x0067BAC3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetFarmerAutoCollectStorageType instead.", true)]
			public static void SetFarmerAutoCollectStorageType(IAsyncMethodRequestHandler requestHandler, sbyte type, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011513 RID: 70931 RVA: 0x0067D8CB File Offset: 0x0067BACB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.UpdateWoodenXiangshuAvatarSelectedFeatures instead.", true)]
			public static void UpdateWoodenXiangshuAvatarSelectedFeatures(IAsyncMethodRequestHandler requestHandler, short featureId, bool isAdd, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011514 RID: 70932 RVA: 0x0067D8D4 File Offset: 0x0067BAD4
			public static void GmCmd_GetBuildingAreaEffectProgresses(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 178, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011515 RID: 70933 RVA: 0x0067D902 File Offset: 0x0067BB02
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.GmCmd_SetBuildingAreaEffectProgresses instead.", true)]
			public static void GmCmd_SetBuildingAreaEffectProgresses(IAsyncMethodRequestHandler requestHandler, int animal, int cricket, int adventure, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011516 RID: 70934 RVA: 0x0067D90A File Offset: 0x0067BB0A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.GmCmd_ReleaseAllKilledByLongYufuCharacters instead.", true)]
			public static void GmCmd_ReleaseAllKilledByLongYufuCharacters(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011517 RID: 70935 RVA: 0x0067D912 File Offset: 0x0067BB12
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.GmCmd_RecordKilledByLongYufuCharacter instead.", true)]
			public static void GmCmd_RecordKilledByLongYufuCharacter(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011518 RID: 70936 RVA: 0x0067D91A File Offset: 0x0067BB1A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.GmCmd_VitalInfectionInOut instead.", true)]
			public static void GmCmd_VitalInfectionInOut(IAsyncMethodRequestHandler requestHandler, int charId, int type, int value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011519 RID: 70937 RVA: 0x0067D924 File Offset: 0x0067BB24
			public static void CheckSpecialCondition_HunterSkill2(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 183, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601151A RID: 70938 RVA: 0x0067D954 File Offset: 0x0067BB54
			public static void GetThreeVitalsCharDataList(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 184, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601151B RID: 70939 RVA: 0x0067D982 File Offset: 0x0067BB82
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.GmCmd_InitThreeVitals instead.", true)]
			public static void GmCmd_InitThreeVitals(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601151C RID: 70940 RVA: 0x0067D98C File Offset: 0x0067BB8C
			public static void GetThreeVitalsTargetCharDataList(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 186, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601151D RID: 70941 RVA: 0x0067D9BA File Offset: 0x0067BBBA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.TransferInfectionBetweenVitalAndCharacter instead.", true)]
			public static void TransferInfectionBetweenVitalAndCharacter(IAsyncMethodRequestHandler requestHandler, int charId, SectStoryThreeVitalsCharacterType type, int value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601151E RID: 70942 RVA: 0x0067D9C2 File Offset: 0x0067BBC2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetVitalInPrison instead.", true)]
			public static void SetVitalInPrison(IAsyncMethodRequestHandler requestHandler, SectStoryThreeVitalsCharacterType type, bool isInPrison, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601151F RID: 70943 RVA: 0x0067D9CC File Offset: 0x0067BBCC
			public static void GetBuildingArtisanOrderAfterUpdate(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(19, 189, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011520 RID: 70944 RVA: 0x0067D9FC File Offset: 0x0067BBFC
			public static void GetCanSelectThreeVitalsDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 190, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011521 RID: 70945 RVA: 0x0067DA2C File Offset: 0x0067BC2C
			public static void AreVitalsDemon(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 191, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011522 RID: 70946 RVA: 0x0067DA5C File Offset: 0x0067BC5C
			public static void GetOppositeThreeVitalsCharDataList(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 192, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011523 RID: 70947 RVA: 0x0067DA8A File Offset: 0x0067BC8A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetVitalHasPlayedComeAnim instead.", true)]
			public static void SetVitalHasPlayedComeAnim(IAsyncMethodRequestHandler requestHandler, SectStoryThreeVitalsCharacterType type, bool hasPlayedComeAnim, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011524 RID: 70948 RVA: 0x0067DA94 File Offset: 0x0067BC94
			public static void GetResourceBlockProducingCoreCooldown(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(19, 194, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011525 RID: 70949 RVA: 0x0067DAC3 File Offset: 0x0067BCC3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.FeastAddDish instead.", true)]
			public static void FeastAddDish(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, int index, ItemKey itemKey, ItemSourceType sourceType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011526 RID: 70950 RVA: 0x0067DACB File Offset: 0x0067BCCB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.FeastSetAutoRefill instead.", true)]
			public static void FeastSetAutoRefill(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, bool value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011527 RID: 70951 RVA: 0x0067DAD4 File Offset: 0x0067BCD4
			public static void GetFeast(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(19, 197, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011528 RID: 70952 RVA: 0x0067DB03 File Offset: 0x0067BD03
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.FeastRemoveDish instead.", true)]
			public static void FeastRemoveDish(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, int index, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011529 RID: 70953 RVA: 0x0067DB0B File Offset: 0x0067BD0B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.FeastRemoveDish instead.", true)]
			public static void FeastRemoveDish(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, int index, ItemSourceType sourceType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601152A RID: 70954 RVA: 0x0067DB13 File Offset: 0x0067BD13
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.FeastReceiveGift instead.", true)]
			public static void FeastReceiveGift(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, int index, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601152B RID: 70955 RVA: 0x0067DB1B File Offset: 0x0067BD1B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.AddResourceItemToArtisanOrder instead.", true)]
			public static void AddResourceItemToArtisanOrder(IAsyncMethodRequestHandler requestHandler, ArtisanOrder order, List<ItemDisplayData> itemList, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601152C RID: 70956 RVA: 0x0067DB24 File Offset: 0x0067BD24
			public static void IsFeastException(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(19, 201, blockKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601152D RID: 70957 RVA: 0x0067DB53 File Offset: 0x0067BD53
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.UseFeastThanksLetter instead.", true)]
			public static void UseFeastThanksLetter(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, int amount, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601152E RID: 70958 RVA: 0x0067DB5B File Offset: 0x0067BD5B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.FeastQuickRefill instead.", true)]
			public static void FeastQuickRefill(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601152F RID: 70959 RVA: 0x0067DB63 File Offset: 0x0067BD63
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.FeastSetTargetType instead.", true)]
			public static void FeastSetTargetType(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, short targetType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011530 RID: 70960 RVA: 0x0067DB6C File Offset: 0x0067BD6C
			public static void GetSectExtraLegacyBuildingStates(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 205, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011531 RID: 70961 RVA: 0x0067DB9A File Offset: 0x0067BD9A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetJieqingGameData instead.", true)]
			public static void SetJieqingGameData(IAsyncMethodRequestHandler requestHandler, SectStoryJieqingGame currJieqingGameState, SectStoryJieqingGame chapterJieqingGameState, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011532 RID: 70962 RVA: 0x0067DBA2 File Offset: 0x0067BDA2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.ConsumeExtraLegacyPoint instead.", true)]
			public static void ConsumeExtraLegacyPoint(IAsyncMethodRequestHandler requestHandler, short legacyTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011533 RID: 70963 RVA: 0x0067DBAC File Offset: 0x0067BDAC
			public static void IsSectBuiltExtraLegacyBuilding(IAsyncMethodRequestHandler requestHandler, sbyte orgTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(19, 208, orgTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011534 RID: 70964 RVA: 0x0067DBDB File Offset: 0x0067BDDB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.InitJieqingGameData instead.", true)]
			public static void InitJieqingGameData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011535 RID: 70965 RVA: 0x0067DBE3 File Offset: 0x0067BDE3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.RemoveSectExtraLegacyBuilding instead.", true)]
			public static void RemoveSectExtraLegacyBuilding(IAsyncMethodRequestHandler requestHandler, sbyte orgTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011536 RID: 70966 RVA: 0x0067DBEB File Offset: 0x0067BDEB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.BuildExtraLegacyBuilding instead.", true)]
			public static void BuildExtraLegacyBuilding(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011537 RID: 70967 RVA: 0x0067DBF4 File Offset: 0x0067BDF4
			public static void GetCharacterExtraLegacyPointWorth(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 212, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011538 RID: 70968 RVA: 0x0067DC24 File Offset: 0x0067BE24
			public static void GetExtraLegacyPointCharacterCountOnBlock(IAsyncMethodRequestHandler requestHandler, Location location, List<short> orgTemplateIds, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Location, List<short>>(19, 213, location, orgTemplateIds, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011539 RID: 70969 RVA: 0x0067DC54 File Offset: 0x0067BE54
			public static void GetSectExtraLegacyBuildingCounts(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 214, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601153A RID: 70970 RVA: 0x0067DC82 File Offset: 0x0067BE82
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SaveMainUiCustomButtons instead.", true)]
			public static void SaveMainUiCustomButtons(IAsyncMethodRequestHandler requestHandler, List<sbyte> customButtons, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601153B RID: 70971 RVA: 0x0067DC8A File Offset: 0x0067BE8A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetMapBlockCharCustomInfoList instead.", true)]
			public static void SetMapBlockCharCustomInfoList(IAsyncMethodRequestHandler requestHandler, List<short> templateIds, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601153C RID: 70972 RVA: 0x0067DC92 File Offset: 0x0067BE92
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetMapBlockCharCustomButtonList instead.", true)]
			public static void SetMapBlockCharCustomButtonList(IAsyncMethodRequestHandler requestHandler, List<short> templateIds, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601153D RID: 70973 RVA: 0x0067DC9C File Offset: 0x0067BE9C
			public static void GetAreaCharacterJieQingSignAmount(IAsyncMethodRequestHandler requestHandler, int areaId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 218, areaId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601153E RID: 70974 RVA: 0x0067DCCC File Offset: 0x0067BECC
			public static void CheckLocationHasBeggerSkill1(IAsyncMethodRequestHandler requestHandler, Location location, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Location>(19, 219, location, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601153F RID: 70975 RVA: 0x0067DCFB File Offset: 0x0067BEFB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetJixiDrainNeili instead.", true)]
			public static void SetJixiDrainNeili(IAsyncMethodRequestHandler requestHandler, bool drainNeili, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011540 RID: 70976 RVA: 0x0067DD03 File Offset: 0x0067BF03
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetJixiTarget instead.", true)]
			public static void SetJixiTarget(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011541 RID: 70977 RVA: 0x0067DD0B File Offset: 0x0067BF0B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.TaiwuTransferNeiliAllocToJixi instead.", true)]
			public static void TaiwuTransferNeiliAllocToJixi(IAsyncMethodRequestHandler requestHandler, int amount, byte neiliType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011542 RID: 70978 RVA: 0x0067DD13 File Offset: 0x0067BF13
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.JixiTransferNeiliAllocToTaiwu instead.", true)]
			public static void JixiTransferNeiliAllocToTaiwu(IAsyncMethodRequestHandler requestHandler, int amount, byte neiliType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011543 RID: 70979 RVA: 0x0067DD1B File Offset: 0x0067BF1B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetJixiDrainType instead.", true)]
			public static void SetJixiDrainType(IAsyncMethodRequestHandler requestHandler, sbyte neiliType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011544 RID: 70980 RVA: 0x0067DD24 File Offset: 0x0067BF24
			public static void GetJixiSpecialInteractDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 225, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011545 RID: 70981 RVA: 0x0067DD52 File Offset: 0x0067BF52
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.InitJixiSpecialInteractData instead.", true)]
			public static void InitJixiSpecialInteractData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011546 RID: 70982 RVA: 0x0067DD5A File Offset: 0x0067BF5A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.JixiRescueTaiwu instead.", true)]
			public static void JixiRescueTaiwu(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011547 RID: 70983 RVA: 0x0067DD64 File Offset: 0x0067BF64
			public static void GetCharacterExtraLegacyPointWorthCalculated(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 228, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011548 RID: 70984 RVA: 0x0067DD94 File Offset: 0x0067BF94
			public static void GetSectRanshanThreeCorpsesData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 229, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011549 RID: 70985 RVA: 0x0067DDC2 File Offset: 0x0067BFC2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetTaiwuTransformFiveElementsTarget instead.", true)]
			public static void SetTaiwuTransformFiveElementsTarget(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601154A RID: 70986 RVA: 0x0067DDCA File Offset: 0x0067BFCA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ExtraDomainMethod.Call.SetTaiwuTargetFiveElementsType instead.", true)]
			public static void SetTaiwuTargetFiveElementsType(IAsyncMethodRequestHandler requestHandler, sbyte fiveElementsType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601154B RID: 70987 RVA: 0x0067DDD4 File Offset: 0x0067BFD4
			public static void GetSectYuanshanThreeVitalsData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(19, 232, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601154C RID: 70988 RVA: 0x0067DE04 File Offset: 0x0067C004
			public static void RequestAllRecruitCharacterData(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey targetBuildingBlock, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<BuildingBlockKey>(19, 233, targetBuildingBlock, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601154D RID: 70989 RVA: 0x0067DE34 File Offset: 0x0067C034
			public static void GetTipLegendaryBookDisplayData(IAsyncMethodRequestHandler requestHandler, sbyte type, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(19, 234, type, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601154E RID: 70990 RVA: 0x0067DE64 File Offset: 0x0067C064
			public static void GetSectMembersWorthExtraLegacyPoint(IAsyncMethodRequestHandler requestHandler, sbyte orgTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(19, 235, orgTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601154F RID: 70991 RVA: 0x0067DE94 File Offset: 0x0067C094
			public static void GetCharacterExtraLegacyPointWorthForMapBlock(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(19, 236, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}
		}
	}
}
