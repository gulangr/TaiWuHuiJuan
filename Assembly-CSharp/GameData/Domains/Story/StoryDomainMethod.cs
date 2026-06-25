using System;
using System.Collections.Generic;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.GameDataBridge;

namespace GameData.Domains.Story
{
	// Token: 0x02000FBF RID: 4031
	public static class StoryDomainMethod
	{
		// Token: 0x020025F9 RID: 9721
		public static class Call
		{
			// Token: 0x06011088 RID: 69768 RVA: 0x00676FE5 File Offset: 0x006751E5
			public static void GetSectMainStoryActiveStatus(int listenerId, sbyte orgTemplateId)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 20, 0, orgTemplateId);
			}

			// Token: 0x06011089 RID: 69769 RVA: 0x00676FF3 File Offset: 0x006751F3
			public static void SetSectMainStoryActiveStatus(sbyte orgTemplateId, bool pause)
			{
				GameDataBridge.AddMethodCall<sbyte, bool>(-1, 20, 1, orgTemplateId, pause);
			}

			// Token: 0x0601108A RID: 69770 RVA: 0x00677002 File Offset: 0x00675202
			public static void NotifySectStoryActivated(sbyte orgTemplateId)
			{
				GameDataBridge.AddMethodCall<sbyte>(-1, 20, 2, orgTemplateId);
			}

			// Token: 0x0601108B RID: 69771 RVA: 0x00677010 File Offset: 0x00675210
			public static void GetBaihuaLifeLinkNeiliType(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 20, 3);
			}

			// Token: 0x0601108C RID: 69772 RVA: 0x0067701D File Offset: 0x0067521D
			public static void GetSectBaihuaLifeLinkDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 20, 4);
			}

			// Token: 0x0601108D RID: 69773 RVA: 0x0067702A File Offset: 0x0067522A
			public static void SetLifeLinkCharacter(int charId, int index, bool isLifeGate)
			{
				GameDataBridge.AddMethodCall<int, int, bool>(-1, 20, 5, charId, index, isLifeGate);
			}

			// Token: 0x0601108E RID: 69774 RVA: 0x0067703A File Offset: 0x0067523A
			public static void ShaolinInterruptDemonSlayerTrial(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 20, 6);
			}

			// Token: 0x0601108F RID: 69775 RVA: 0x00677047 File Offset: 0x00675247
			public static void ShaolinRegenerateRestricts(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 20, 7);
			}

			// Token: 0x06011090 RID: 69776 RVA: 0x00677054 File Offset: 0x00675254
			public static void ShaolinQueryRestrictsAreSatisfied(int listenerId, int index)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 20, 8, index);
			}

			// Token: 0x06011091 RID: 69777 RVA: 0x00677062 File Offset: 0x00675262
			public static void ShaolinStartDemonSlayerTrial(int index)
			{
				GameDataBridge.AddMethodCall<int>(-1, 20, 9, index);
			}

			// Token: 0x06011092 RID: 69778 RVA: 0x00677071 File Offset: 0x00675271
			public static void ShaolinGenerateTemporaryDemon(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 20, 10);
			}

			// Token: 0x06011093 RID: 69779 RVA: 0x0067707F File Offset: 0x0067527F
			public static void ShaolinClearTemporaryDemon(List<int> demonCharIds)
			{
				GameDataBridge.AddMethodCall<List<int>>(-1, 20, 11, demonCharIds);
			}

			// Token: 0x06011094 RID: 69780 RVA: 0x0067708E File Offset: 0x0067528E
			public static void CreateMirrorCharacter(int listenerId, bool isMale, string familyName, string firstName)
			{
				GameDataBridge.AddMethodCall<bool, string, string>(listenerId, 20, 12, isMale, familyName, firstName);
			}

			// Token: 0x06011095 RID: 69781 RVA: 0x0067709F File Offset: 0x0067529F
			public static void GetDefendHeavenlyTreeDisplayData(int listenerId, bool includeGrownTree, Location curTreeLocation, List<Location> viewTreeLocationList)
			{
				GameDataBridge.AddMethodCall<bool, Location, List<Location>>(listenerId, 20, 13, includeGrownTree, curTreeLocation, viewTreeLocationList);
			}

			// Token: 0x06011096 RID: 69782 RVA: 0x006770B0 File Offset: 0x006752B0
			public static void DefendHeavenlyTreeFeed(int listenerId, int treeId, ItemDisplayData itemData)
			{
				GameDataBridge.AddMethodCall<int, ItemDisplayData>(listenerId, 20, 14, treeId, itemData);
			}

			// Token: 0x06011097 RID: 69783 RVA: 0x006770C0 File Offset: 0x006752C0
			public static void TryTriggerThiefCatch(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 20, 15);
			}

			// Token: 0x06011098 RID: 69784 RVA: 0x006770CE File Offset: 0x006752CE
			public static void CatchThief(sbyte thiefLevel, bool timeOut)
			{
				GameDataBridge.AddMethodCall<sbyte, bool>(-1, 20, 16, thiefLevel, timeOut);
			}

			// Token: 0x06011099 RID: 69785 RVA: 0x006770DE File Offset: 0x006752DE
			public static void GetSectZhujianGearMateAttributeDisplayData(int listenerId, int gearMateId, int characterId)
			{
				GameDataBridge.AddMethodCall<int, int>(listenerId, 20, 17, gearMateId, characterId);
			}

			// Token: 0x0601109A RID: 69786 RVA: 0x006770EE File Offset: 0x006752EE
			public static void GetSectZhujianGearMateSkillDisplayData(int listenerId, int gearMateId, bool isCombatSkill)
			{
				GameDataBridge.AddMethodCall<int, bool>(listenerId, 20, 18, gearMateId, isCombatSkill);
			}

			// Token: 0x0601109B RID: 69787 RVA: 0x006770FE File Offset: 0x006752FE
			public static void GetGearMateBreakoutDisplayData(int listenerId, int gearMateId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 20, 19, gearMateId);
			}

			// Token: 0x0601109C RID: 69788 RVA: 0x0067710D File Offset: 0x0067530D
			public static void GetSectZhujianGearMateFeatureDisplayData(int listenerId, int gearMateId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 20, 20, gearMateId);
			}

			// Token: 0x0601109D RID: 69789 RVA: 0x0067711C File Offset: 0x0067531C
			public static void GetSectZhujianGearMateConsummateDisplayData(int listenerId, int gearMateId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 20, 21, gearMateId);
			}

			// Token: 0x0601109E RID: 69790 RVA: 0x0067712B File Offset: 0x0067532B
			public static void JingangMonkSoulBtnShow(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 20, 22);
			}

			// Token: 0x0601109F RID: 69791 RVA: 0x00677139 File Offset: 0x00675339
			public static void GetCurAreaValidCharactersForTripodVessel(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 20, 23);
			}

			// Token: 0x060110A0 RID: 69792 RVA: 0x00677147 File Offset: 0x00675347
			public static void ApplyKongsangSpecialInteract(List<int> characterIds, List<ItemKeyAndCount> selectedWugKingCountList)
			{
				GameDataBridge.AddMethodCall<List<int>, List<ItemKeyAndCount>>(-1, 20, 24, characterIds, selectedWugKingCountList);
			}

			// Token: 0x060110A1 RID: 69793 RVA: 0x00677157 File Offset: 0x00675357
			public static void GetEmeiBreakBonusCollection(int listenerId, short combatSkillId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 20, 25, combatSkillId);
			}

			// Token: 0x060110A2 RID: 69794 RVA: 0x00677166 File Offset: 0x00675366
			public static void AddEmeiSkillBreakBonus(int listenerId, short combatSkillId, short bonusTypeTemplateId)
			{
				GameDataBridge.AddMethodCall<short, short>(listenerId, 20, 26, combatSkillId, bonusTypeTemplateId);
			}

			// Token: 0x060110A3 RID: 69795 RVA: 0x00677176 File Offset: 0x00675376
			public static void GmCmd_SectEmeiAddSkillBreakBonus(int listenerId, short combatSkillId, short bonusTypeTemplateId)
			{
				GameDataBridge.AddMethodCall<short, short>(listenerId, 20, 27, combatSkillId, bonusTypeTemplateId);
			}

			// Token: 0x060110A4 RID: 69796 RVA: 0x00677186 File Offset: 0x00675386
			public static void GetEmeiBreakBonusDisplayData(int listenerId, short combatSkillId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 20, 28, combatSkillId);
			}

			// Token: 0x060110A5 RID: 69797 RVA: 0x00677195 File Offset: 0x00675395
			public static void GetSectEmeiSpecialBreakDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 20, 29);
			}

			// Token: 0x060110A6 RID: 69798 RVA: 0x006771A3 File Offset: 0x006753A3
			public static void EmeiTransferBonusProgress(int listenerId, short bonusTemplateId, List<ItemKey> itemKeys)
			{
				GameDataBridge.AddMethodCall<short, List<ItemKey>>(listenerId, 20, 30, bonusTemplateId, itemKeys);
			}

			// Token: 0x060110A7 RID: 69799 RVA: 0x006771B3 File Offset: 0x006753B3
			public static void RemoveEmeiSkillBreakBonus(int listenerId, short combatSkillId, short bonusTypeTemplateId)
			{
				GameDataBridge.AddMethodCall<short, short>(listenerId, 20, 31, combatSkillId, bonusTypeTemplateId);
			}

			// Token: 0x060110A8 RID: 69800 RVA: 0x006771C3 File Offset: 0x006753C3
			public static void GmCmd_SectEmeiClearSkillBreakBonus(int listenerId, short combatSkillId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 20, 32, combatSkillId);
			}

			// Token: 0x060110A9 RID: 69801 RVA: 0x006771D2 File Offset: 0x006753D2
			public static void GetSectMainStoryTriggerConditions(int listenerId, short templateId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 20, 33, templateId);
			}

			// Token: 0x060110AA RID: 69802 RVA: 0x006771E1 File Offset: 0x006753E1
			public static void DriveWugKing(int listenerId, int charId, sbyte wugType, sbyte driveType)
			{
				GameDataBridge.AddMethodCall<int, sbyte, sbyte>(listenerId, 20, 34, charId, wugType, driveType);
			}

			// Token: 0x060110AB RID: 69803 RVA: 0x006771F2 File Offset: 0x006753F2
			public static void RefiningWugKing(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 20, 35);
			}

			// Token: 0x060110AC RID: 69804 RVA: 0x00677200 File Offset: 0x00675400
			public static void DropPoisonsToWugJug(int listenerId, Inventory poisonMaterials)
			{
				GameDataBridge.AddMethodCall<Inventory>(listenerId, 20, 36, poisonMaterials);
			}

			// Token: 0x060110AD RID: 69805 RVA: 0x0067720F File Offset: 0x0067540F
			public static void GetWugKingDriveStatuses(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 20, 37, charId);
			}

			// Token: 0x060110AE RID: 69806 RVA: 0x0067721E File Offset: 0x0067541E
			public static void GetThreeVitalsReplaceTeammateRecord(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 20, 38);
			}

			// Token: 0x060110AF RID: 69807 RVA: 0x0067722C File Offset: 0x0067542C
			public static void ThreeVitalsReplaceTeammateRecordRemove(int typeInt)
			{
				GameDataBridge.AddMethodCall<int>(-1, 20, 39, typeInt);
			}

			// Token: 0x060110B0 RID: 69808 RVA: 0x0067723B File Offset: 0x0067543B
			public static void ThreeVitalsReplaceTeammateRecordSet(int typeInt, int index)
			{
				GameDataBridge.AddMethodCall<int, int>(-1, 20, 40, typeInt, index);
			}

			// Token: 0x060110B1 RID: 69809 RVA: 0x0067724B File Offset: 0x0067544B
			public static void DefendHeavenlyTreeClearEnemy(int listenerId, int treeId, int charId)
			{
				GameDataBridge.AddMethodCall<int, int>(listenerId, 20, 41, treeId, charId);
			}

			// Token: 0x060110B2 RID: 69810 RVA: 0x0067725B File Offset: 0x0067545B
			public static void GmCmd_ClearIronPlateCooldown()
			{
				GameDataBridge.AddMethodCall(-1, 20, 42);
			}

			// Token: 0x060110B3 RID: 69811 RVA: 0x00677269 File Offset: 0x00675469
			public static void GetIronPlateCombatCharId(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 20, 43);
			}

			// Token: 0x060110B4 RID: 69812 RVA: 0x00677277 File Offset: 0x00675477
			public static void GetIronPlateOptionCharIdList(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 20, 44);
			}

			// Token: 0x060110B5 RID: 69813 RVA: 0x00677285 File Offset: 0x00675485
			public static void SetIconPlateFollowingCharId(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 20, 45, charId);
			}

			// Token: 0x060110B6 RID: 69814 RVA: 0x00677294 File Offset: 0x00675494
			public static void GmCmd_SetIconPlateIsUnlocked(bool isUnlocked)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 20, 46, isUnlocked);
			}

			// Token: 0x060110B7 RID: 69815 RVA: 0x006772A3 File Offset: 0x006754A3
			public static void GmCmd_ClearDivineFlameCooldown()
			{
				GameDataBridge.AddMethodCall(-1, 20, 47);
			}

			// Token: 0x060110B8 RID: 69816 RVA: 0x006772B1 File Offset: 0x006754B1
			public static void GmCmd_SetDivineFlameIsUnlocked(bool isUnlocked)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 20, 48, isUnlocked);
			}

			// Token: 0x060110B9 RID: 69817 RVA: 0x006772C0 File Offset: 0x006754C0
			public static void UseDivineFlame(sbyte xiangshuAvatarId, int targetCharId, Location targetLocation)
			{
				GameDataBridge.AddMethodCall<sbyte, int, Location>(-1, 20, 49, xiangshuAvatarId, targetCharId, targetLocation);
			}

			// Token: 0x060110BA RID: 69818 RVA: 0x006772D1 File Offset: 0x006754D1
			public static void GetDivineFlameSelectTargetCharIdList(int listenerId, sbyte xiangshuAvatarId)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 20, 50, xiangshuAvatarId);
			}

			// Token: 0x060110BB RID: 69819 RVA: 0x006772E0 File Offset: 0x006754E0
			public static void CheckDivineFlameTarget(int listenerId, sbyte xiangshuAvatarId)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 20, 51, xiangshuAvatarId);
			}

			// Token: 0x060110BC RID: 69820 RVA: 0x006772EF File Offset: 0x006754EF
			public static void GetDivineFlameSelectTargetLocationList(int listenerId, sbyte xiangshuAvatarId, Location location)
			{
				GameDataBridge.AddMethodCall<sbyte, Location>(listenerId, 20, 52, xiangshuAvatarId, location);
			}

			// Token: 0x060110BD RID: 69821 RVA: 0x006772FF File Offset: 0x006754FF
			public static void GetDivineFlameDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 20, 53);
			}

			// Token: 0x060110BE RID: 69822 RVA: 0x0067730D File Offset: 0x0067550D
			public static void UpdateSectEmeiGuidanceData(Location location)
			{
				GameDataBridge.AddMethodCall<Location>(-1, 20, 54, location);
			}

			// Token: 0x060110BF RID: 69823 RVA: 0x0067731C File Offset: 0x0067551C
			public static void OnClickEmeiGuidance(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 20, 55, charId);
			}
		}

		// Token: 0x020025FA RID: 9722
		public static class AsyncCall
		{
			// Token: 0x060110C0 RID: 69824 RVA: 0x0067732C File Offset: 0x0067552C
			public static void GetSectMainStoryActiveStatus(IAsyncMethodRequestHandler requestHandler, sbyte orgTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(20, 0, orgTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110C1 RID: 69825 RVA: 0x00677357 File Offset: 0x00675557
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use StoryDomainMethod.Call.SetSectMainStoryActiveStatus instead.", true)]
			public static void SetSectMainStoryActiveStatus(IAsyncMethodRequestHandler requestHandler, sbyte orgTemplateId, bool pause, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060110C2 RID: 69826 RVA: 0x0067735F File Offset: 0x0067555F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use StoryDomainMethod.Call.NotifySectStoryActivated instead.", true)]
			public static void NotifySectStoryActivated(IAsyncMethodRequestHandler requestHandler, sbyte orgTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060110C3 RID: 69827 RVA: 0x00677368 File Offset: 0x00675568
			public static void GetBaihuaLifeLinkNeiliType(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(20, 3, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110C4 RID: 69828 RVA: 0x00677394 File Offset: 0x00675594
			public static void GetSectBaihuaLifeLinkDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(20, 4, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110C5 RID: 69829 RVA: 0x006773BE File Offset: 0x006755BE
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use StoryDomainMethod.Call.SetLifeLinkCharacter instead.", true)]
			public static void SetLifeLinkCharacter(IAsyncMethodRequestHandler requestHandler, int charId, int index, bool isLifeGate, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060110C6 RID: 69830 RVA: 0x006773C8 File Offset: 0x006755C8
			public static void ShaolinInterruptDemonSlayerTrial(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(20, 6, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110C7 RID: 69831 RVA: 0x006773F4 File Offset: 0x006755F4
			public static void ShaolinRegenerateRestricts(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(20, 7, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110C8 RID: 69832 RVA: 0x00677420 File Offset: 0x00675620
			public static void ShaolinQueryRestrictsAreSatisfied(IAsyncMethodRequestHandler requestHandler, int index, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(20, 8, index, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110C9 RID: 69833 RVA: 0x0067744B File Offset: 0x0067564B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use StoryDomainMethod.Call.ShaolinStartDemonSlayerTrial instead.", true)]
			public static void ShaolinStartDemonSlayerTrial(IAsyncMethodRequestHandler requestHandler, int index, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060110CA RID: 69834 RVA: 0x00677454 File Offset: 0x00675654
			public static void ShaolinGenerateTemporaryDemon(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(20, 10, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110CB RID: 69835 RVA: 0x0067747F File Offset: 0x0067567F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use StoryDomainMethod.Call.ShaolinClearTemporaryDemon instead.", true)]
			public static void ShaolinClearTemporaryDemon(IAsyncMethodRequestHandler requestHandler, List<int> demonCharIds, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060110CC RID: 69836 RVA: 0x00677488 File Offset: 0x00675688
			public static void CreateMirrorCharacter(IAsyncMethodRequestHandler requestHandler, bool isMale, string familyName, string firstName, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool, string, string>(20, 12, isMale, familyName, firstName, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110CD RID: 69837 RVA: 0x006774B8 File Offset: 0x006756B8
			public static void GetDefendHeavenlyTreeDisplayData(IAsyncMethodRequestHandler requestHandler, bool includeGrownTree, Location curTreeLocation, List<Location> viewTreeLocationList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool, Location, List<Location>>(20, 13, includeGrownTree, curTreeLocation, viewTreeLocationList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110CE RID: 69838 RVA: 0x006774E8 File Offset: 0x006756E8
			public static void DefendHeavenlyTreeFeed(IAsyncMethodRequestHandler requestHandler, int treeId, ItemDisplayData itemData, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ItemDisplayData>(20, 14, treeId, itemData, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110CF RID: 69839 RVA: 0x00677518 File Offset: 0x00675718
			public static void TryTriggerThiefCatch(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(20, 15, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110D0 RID: 69840 RVA: 0x00677543 File Offset: 0x00675743
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use StoryDomainMethod.Call.CatchThief instead.", true)]
			public static void CatchThief(IAsyncMethodRequestHandler requestHandler, sbyte thiefLevel, bool timeOut, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060110D1 RID: 69841 RVA: 0x0067754C File Offset: 0x0067574C
			public static void GetSectZhujianGearMateAttributeDisplayData(IAsyncMethodRequestHandler requestHandler, int gearMateId, int characterId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int>(20, 17, gearMateId, characterId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110D2 RID: 69842 RVA: 0x0067757C File Offset: 0x0067577C
			public static void GetSectZhujianGearMateSkillDisplayData(IAsyncMethodRequestHandler requestHandler, int gearMateId, bool isCombatSkill, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, bool>(20, 18, gearMateId, isCombatSkill, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110D3 RID: 69843 RVA: 0x006775AC File Offset: 0x006757AC
			public static void GetGearMateBreakoutDisplayData(IAsyncMethodRequestHandler requestHandler, int gearMateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(20, 19, gearMateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110D4 RID: 69844 RVA: 0x006775D8 File Offset: 0x006757D8
			public static void GetSectZhujianGearMateFeatureDisplayData(IAsyncMethodRequestHandler requestHandler, int gearMateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(20, 20, gearMateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110D5 RID: 69845 RVA: 0x00677604 File Offset: 0x00675804
			public static void GetSectZhujianGearMateConsummateDisplayData(IAsyncMethodRequestHandler requestHandler, int gearMateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(20, 21, gearMateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110D6 RID: 69846 RVA: 0x00677630 File Offset: 0x00675830
			public static void JingangMonkSoulBtnShow(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(20, 22, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110D7 RID: 69847 RVA: 0x0067765C File Offset: 0x0067585C
			public static void GetCurAreaValidCharactersForTripodVessel(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(20, 23, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110D8 RID: 69848 RVA: 0x00677687 File Offset: 0x00675887
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use StoryDomainMethod.Call.ApplyKongsangSpecialInteract instead.", true)]
			public static void ApplyKongsangSpecialInteract(IAsyncMethodRequestHandler requestHandler, List<int> characterIds, List<ItemKeyAndCount> selectedWugKingCountList, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060110D9 RID: 69849 RVA: 0x00677690 File Offset: 0x00675890
			public static void GetEmeiBreakBonusCollection(IAsyncMethodRequestHandler requestHandler, short combatSkillId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(20, 25, combatSkillId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110DA RID: 69850 RVA: 0x006776BC File Offset: 0x006758BC
			public static void AddEmeiSkillBreakBonus(IAsyncMethodRequestHandler requestHandler, short combatSkillId, short bonusTypeTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, short>(20, 26, combatSkillId, bonusTypeTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110DB RID: 69851 RVA: 0x006776EC File Offset: 0x006758EC
			public static void GmCmd_SectEmeiAddSkillBreakBonus(IAsyncMethodRequestHandler requestHandler, short combatSkillId, short bonusTypeTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, short>(20, 27, combatSkillId, bonusTypeTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110DC RID: 69852 RVA: 0x0067771C File Offset: 0x0067591C
			public static void GetEmeiBreakBonusDisplayData(IAsyncMethodRequestHandler requestHandler, short combatSkillId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(20, 28, combatSkillId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110DD RID: 69853 RVA: 0x00677748 File Offset: 0x00675948
			public static void GetSectEmeiSpecialBreakDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(20, 29, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110DE RID: 69854 RVA: 0x00677774 File Offset: 0x00675974
			public static void EmeiTransferBonusProgress(IAsyncMethodRequestHandler requestHandler, short bonusTemplateId, List<ItemKey> itemKeys, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, List<ItemKey>>(20, 30, bonusTemplateId, itemKeys, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110DF RID: 69855 RVA: 0x006777A4 File Offset: 0x006759A4
			public static void RemoveEmeiSkillBreakBonus(IAsyncMethodRequestHandler requestHandler, short combatSkillId, short bonusTypeTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, short>(20, 31, combatSkillId, bonusTypeTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110E0 RID: 69856 RVA: 0x006777D4 File Offset: 0x006759D4
			public static void GmCmd_SectEmeiClearSkillBreakBonus(IAsyncMethodRequestHandler requestHandler, short combatSkillId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(20, 32, combatSkillId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110E1 RID: 69857 RVA: 0x00677800 File Offset: 0x00675A00
			public static void GetSectMainStoryTriggerConditions(IAsyncMethodRequestHandler requestHandler, short templateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(20, 33, templateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110E2 RID: 69858 RVA: 0x0067782C File Offset: 0x00675A2C
			public static void DriveWugKing(IAsyncMethodRequestHandler requestHandler, int charId, sbyte wugType, sbyte driveType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, sbyte, sbyte>(20, 34, charId, wugType, driveType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110E3 RID: 69859 RVA: 0x0067785C File Offset: 0x00675A5C
			public static void RefiningWugKing(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(20, 35, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110E4 RID: 69860 RVA: 0x00677888 File Offset: 0x00675A88
			public static void DropPoisonsToWugJug(IAsyncMethodRequestHandler requestHandler, Inventory poisonMaterials, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Inventory>(20, 36, poisonMaterials, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110E5 RID: 69861 RVA: 0x006778B4 File Offset: 0x00675AB4
			public static void GetWugKingDriveStatuses(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(20, 37, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110E6 RID: 69862 RVA: 0x006778E0 File Offset: 0x00675AE0
			public static void GetThreeVitalsReplaceTeammateRecord(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(20, 38, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110E7 RID: 69863 RVA: 0x0067790B File Offset: 0x00675B0B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use StoryDomainMethod.Call.ThreeVitalsReplaceTeammateRecordRemove instead.", true)]
			public static void ThreeVitalsReplaceTeammateRecordRemove(IAsyncMethodRequestHandler requestHandler, int typeInt, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060110E8 RID: 69864 RVA: 0x00677913 File Offset: 0x00675B13
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use StoryDomainMethod.Call.ThreeVitalsReplaceTeammateRecordSet instead.", true)]
			public static void ThreeVitalsReplaceTeammateRecordSet(IAsyncMethodRequestHandler requestHandler, int typeInt, int index, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060110E9 RID: 69865 RVA: 0x0067791C File Offset: 0x00675B1C
			public static void DefendHeavenlyTreeClearEnemy(IAsyncMethodRequestHandler requestHandler, int treeId, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int>(20, 41, treeId, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110EA RID: 69866 RVA: 0x00677949 File Offset: 0x00675B49
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use StoryDomainMethod.Call.GmCmd_ClearIronPlateCooldown instead.", true)]
			public static void GmCmd_ClearIronPlateCooldown(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060110EB RID: 69867 RVA: 0x00677954 File Offset: 0x00675B54
			public static void GetIronPlateCombatCharId(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(20, 43, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110EC RID: 69868 RVA: 0x00677980 File Offset: 0x00675B80
			public static void GetIronPlateOptionCharIdList(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(20, 44, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110ED RID: 69869 RVA: 0x006779AB File Offset: 0x00675BAB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use StoryDomainMethod.Call.SetIconPlateFollowingCharId instead.", true)]
			public static void SetIconPlateFollowingCharId(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060110EE RID: 69870 RVA: 0x006779B3 File Offset: 0x00675BB3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use StoryDomainMethod.Call.GmCmd_SetIconPlateIsUnlocked instead.", true)]
			public static void GmCmd_SetIconPlateIsUnlocked(IAsyncMethodRequestHandler requestHandler, bool isUnlocked, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060110EF RID: 69871 RVA: 0x006779BB File Offset: 0x00675BBB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use StoryDomainMethod.Call.GmCmd_ClearDivineFlameCooldown instead.", true)]
			public static void GmCmd_ClearDivineFlameCooldown(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060110F0 RID: 69872 RVA: 0x006779C3 File Offset: 0x00675BC3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use StoryDomainMethod.Call.GmCmd_SetDivineFlameIsUnlocked instead.", true)]
			public static void GmCmd_SetDivineFlameIsUnlocked(IAsyncMethodRequestHandler requestHandler, bool isUnlocked, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060110F1 RID: 69873 RVA: 0x006779CB File Offset: 0x00675BCB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use StoryDomainMethod.Call.UseDivineFlame instead.", true)]
			public static void UseDivineFlame(IAsyncMethodRequestHandler requestHandler, sbyte xiangshuAvatarId, int targetCharId, Location targetLocation, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060110F2 RID: 69874 RVA: 0x006779D4 File Offset: 0x00675BD4
			public static void GetDivineFlameSelectTargetCharIdList(IAsyncMethodRequestHandler requestHandler, sbyte xiangshuAvatarId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(20, 50, xiangshuAvatarId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110F3 RID: 69875 RVA: 0x00677A00 File Offset: 0x00675C00
			public static void CheckDivineFlameTarget(IAsyncMethodRequestHandler requestHandler, sbyte xiangshuAvatarId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(20, 51, xiangshuAvatarId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110F4 RID: 69876 RVA: 0x00677A2C File Offset: 0x00675C2C
			public static void GetDivineFlameSelectTargetLocationList(IAsyncMethodRequestHandler requestHandler, sbyte xiangshuAvatarId, Location location, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte, Location>(20, 52, xiangshuAvatarId, location, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110F5 RID: 69877 RVA: 0x00677A5C File Offset: 0x00675C5C
			public static void GetDivineFlameDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(20, 53, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110F6 RID: 69878 RVA: 0x00677A87 File Offset: 0x00675C87
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use StoryDomainMethod.Call.UpdateSectEmeiGuidanceData instead.", true)]
			public static void UpdateSectEmeiGuidanceData(IAsyncMethodRequestHandler requestHandler, Location location, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060110F7 RID: 69879 RVA: 0x00677A8F File Offset: 0x00675C8F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use StoryDomainMethod.Call.OnClickEmeiGuidance instead.", true)]
			public static void OnClickEmeiGuidance(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}
		}
	}
}
