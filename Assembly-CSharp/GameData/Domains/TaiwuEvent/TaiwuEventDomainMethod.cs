using System;
using System.Collections.Generic;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Information;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.TaiwuEvent.DisplayEvent;
using GameData.Domains.TaiwuEvent.MonthlyEventActions;
using GameData.GameDataBridge;
using GameData.Utilities;

namespace GameData.Domains.TaiwuEvent
{
	// Token: 0x02000FBD RID: 4029
	public static class TaiwuEventDomainMethod
	{
		// Token: 0x020025F5 RID: 9717
		public static class Call
		{
			// Token: 0x06010D5C RID: 68956 RVA: 0x00672A1A File Offset: 0x00670C1A
			public static void GetMonthlyActionStateAndTime(int listenerId, MonthlyActionKey key)
			{
				GameDataBridge.AddMethodCall<MonthlyActionKey>(listenerId, 12, 0, key);
			}

			// Token: 0x06010D5D RID: 68957 RVA: 0x00672A28 File Offset: 0x00670C28
			public static void InitConchShipEvents()
			{
				GameDataBridge.AddMethodCall(-1, 12, 1);
			}

			// Token: 0x06010D5E RID: 68958 RVA: 0x00672A35 File Offset: 0x00670C35
			public static void TriggerListener(string key, bool value)
			{
				GameDataBridge.AddMethodCall<string, bool>(-1, 12, 2, key, value);
			}

			// Token: 0x06010D5F RID: 68959 RVA: 0x00672A44 File Offset: 0x00670C44
			public static void SetItemSelectResult(string key, ItemKey itemKey, bool callComplete)
			{
				GameDataBridge.AddMethodCall<string, ItemKey, bool>(-1, 12, 3, key, itemKey, callComplete);
			}

			// Token: 0x06010D60 RID: 68960 RVA: 0x00672A54 File Offset: 0x00670C54
			public static void SetCharacterSelectResult(string key, int charId, bool callComplete)
			{
				GameDataBridge.AddMethodCall<string, int, bool>(-1, 12, 4, key, charId, callComplete);
			}

			// Token: 0x06010D61 RID: 68961 RVA: 0x00672A64 File Offset: 0x00670C64
			public static void SetSecretInformationSelectResult(string key, int secretId)
			{
				GameDataBridge.AddMethodCall<string, int>(-1, 12, 5, key, secretId);
			}

			// Token: 0x06010D62 RID: 68962 RVA: 0x00672A73 File Offset: 0x00670C73
			public static void SetNormalInformationSelectResult(string key, NormalInformation normalInformation)
			{
				GameDataBridge.AddMethodCall<string, NormalInformation>(-1, 12, 6, key, normalInformation);
			}

			// Token: 0x06010D63 RID: 68963 RVA: 0x00672A82 File Offset: 0x00670C82
			public static void StartHandleEventDuringAdvance()
			{
				GameDataBridge.AddMethodCall(-1, 12, 7);
			}

			// Token: 0x06010D64 RID: 68964 RVA: 0x00672A8F File Offset: 0x00670C8F
			public static void GetTriggeredEventSummaryDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 12, 8);
			}

			// Token: 0x06010D65 RID: 68965 RVA: 0x00672A9C File Offset: 0x00670C9C
			public static void SetEventInProcessing(string eventGuid)
			{
				GameDataBridge.AddMethodCall<string>(-1, 12, 9, eventGuid);
			}

			// Token: 0x06010D66 RID: 68966 RVA: 0x00672AAB File Offset: 0x00670CAB
			public static void EventSelect(string eventGuid, string optionKey)
			{
				GameDataBridge.AddMethodCall<string, string>(-1, 12, 10, eventGuid, optionKey);
			}

			// Token: 0x06010D67 RID: 68967 RVA: 0x00672ABB File Offset: 0x00670CBB
			public static void EventSelect(string eventGuid, string optionKey, bool isContinue)
			{
				GameDataBridge.AddMethodCall<string, string, bool>(-1, 12, 10, eventGuid, optionKey, isContinue);
			}

			// Token: 0x06010D68 RID: 68968 RVA: 0x00672ACC File Offset: 0x00670CCC
			public static void GetEventDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 12, 11);
			}

			// Token: 0x06010D69 RID: 68969 RVA: 0x00672ADA File Offset: 0x00670CDA
			public static void GmCmd_SaveMonthlyActionManager()
			{
				GameDataBridge.AddMethodCall(-1, 12, 12);
			}

			// Token: 0x06010D6A RID: 68970 RVA: 0x00672AE8 File Offset: 0x00670CE8
			public static void OnCharacterClicked(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 12, 13, charId);
			}

			// Token: 0x06010D6B RID: 68971 RVA: 0x00672AF7 File Offset: 0x00670CF7
			public static void OnLetTeammateLeaveGroup(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 12, 14, charId);
			}

			// Token: 0x06010D6C RID: 68972 RVA: 0x00672B06 File Offset: 0x00670D06
			public static void OnInteractCaravan(int caravanId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 12, 15, caravanId);
			}

			// Token: 0x06010D6D RID: 68973 RVA: 0x00672B15 File Offset: 0x00670D15
			public static void OnInteractKidnappedCharacter(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 12, 16, charId);
			}

			// Token: 0x06010D6E RID: 68974 RVA: 0x00672B24 File Offset: 0x00670D24
			public static void OnSectBuildingClicked(short buildingTemplateId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 12, 17, buildingTemplateId);
			}

			// Token: 0x06010D6F RID: 68975 RVA: 0x00672B33 File Offset: 0x00670D33
			public static void OnRecordEnterGame()
			{
				GameDataBridge.AddMethodCall(-1, 12, 18);
			}

			// Token: 0x06010D70 RID: 68976 RVA: 0x00672B41 File Offset: 0x00670D41
			public static void OnNewGameMonth()
			{
				GameDataBridge.AddMethodCall(-1, 12, 19);
			}

			// Token: 0x06010D71 RID: 68977 RVA: 0x00672B4F File Offset: 0x00670D4F
			public static void OnCombatWithXiangshuMinionComplete(short templateId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 12, 20, templateId);
			}

			// Token: 0x06010D72 RID: 68978 RVA: 0x00672B5E File Offset: 0x00670D5E
			public static void OnBlackMaskAnimationComplete(bool maskVisible)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 12, 21, maskVisible);
			}

			// Token: 0x06010D73 RID: 68979 RVA: 0x00672B6D File Offset: 0x00670D6D
			public static void OnMakingSystemOpened(BuildingBlockKey blockKey, short templateId)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, short>(-1, 12, 22, blockKey, templateId);
			}

			// Token: 0x06010D74 RID: 68980 RVA: 0x00672B7D File Offset: 0x00670D7D
			public static void OnCollectedMakingSystemItem(BuildingBlockKey blockKey, short templateId, bool showingGetItem)
			{
				GameDataBridge.AddMethodCall<BuildingBlockKey, short, bool>(-1, 12, 23, blockKey, templateId, showingGetItem);
			}

			// Token: 0x06010D75 RID: 68981 RVA: 0x00672B8E File Offset: 0x00670D8E
			public static void OnSectSpecialBuildingClicked(short templateId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 12, 24, templateId);
			}

			// Token: 0x06010D76 RID: 68982 RVA: 0x00672B9D File Offset: 0x00670D9D
			public static void AnimalAvatarClicked(int animalId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 12, 25, animalId);
			}

			// Token: 0x06010D77 RID: 68983 RVA: 0x00672BAC File Offset: 0x00670DAC
			public static void MainStoryFinishCatchCricket(bool result)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 12, 26, result);
			}

			// Token: 0x06010D78 RID: 68984 RVA: 0x00672BBB File Offset: 0x00670DBB
			public static void LoadEventsFromPath(string eventDataDirectory)
			{
				GameDataBridge.AddMethodCall<string>(-1, 12, 27, eventDataDirectory);
			}

			// Token: 0x06010D79 RID: 68985 RVA: 0x00672BCA File Offset: 0x00670DCA
			public static void NpcTombClicked(int tombId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 12, 28, tombId);
			}

			// Token: 0x06010D7A RID: 68986 RVA: 0x00672BD9 File Offset: 0x00670DD9
			public static void SetLifeSkillSelectResult(short lifeSkillId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 12, 29, lifeSkillId);
			}

			// Token: 0x06010D7B RID: 68987 RVA: 0x00672BE8 File Offset: 0x00670DE8
			public static void SetCombatSkillSelectResult(short combatSkillId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 12, 30, combatSkillId);
			}

			// Token: 0x06010D7C RID: 68988 RVA: 0x00672BF7 File Offset: 0x00670DF7
			public static void OnLifeSkillCombatForceSilent(int charId, sbyte concessionCount, sbyte inducementCount)
			{
				GameDataBridge.AddMethodCall<int, sbyte, sbyte>(-1, 12, 31, charId, concessionCount, inducementCount);
			}

			// Token: 0x06010D7D RID: 68989 RVA: 0x00672C08 File Offset: 0x00670E08
			public static void TryMoveWhenMoveDisable()
			{
				GameDataBridge.AddMethodCall(-1, 12, 32);
			}

			// Token: 0x06010D7E RID: 68990 RVA: 0x00672C16 File Offset: 0x00670E16
			public static void TryMoveToInvalidLocationInTutorial()
			{
				GameDataBridge.AddMethodCall(-1, 12, 33);
			}

			// Token: 0x06010D7F RID: 68991 RVA: 0x00672C24 File Offset: 0x00670E24
			public static void SetCharacterSetSelectResult(string actionName, string key, CharacterSet characterSet)
			{
				GameDataBridge.AddMethodCall<string, string, CharacterSet>(-1, 12, 34, actionName, key, characterSet);
			}

			// Token: 0x06010D80 RID: 68992 RVA: 0x00672C35 File Offset: 0x00670E35
			public static void OnCharacterTemplateClicked(short characterTemplateId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 12, 35, characterTemplateId);
			}

			// Token: 0x06010D81 RID: 68993 RVA: 0x00672C44 File Offset: 0x00670E44
			public static void CloseUI(string uiName)
			{
				GameDataBridge.AddMethodCall<string>(-1, 12, 36, uiName);
			}

			// Token: 0x06010D82 RID: 68994 RVA: 0x00672C53 File Offset: 0x00670E53
			public static void CloseUI(string uiName, bool presetBool)
			{
				GameDataBridge.AddMethodCall<string, bool>(-1, 12, 36, uiName, presetBool);
			}

			// Token: 0x06010D83 RID: 68995 RVA: 0x00672C63 File Offset: 0x00670E63
			public static void CloseUI(string uiName, bool presetBool, int presetInt)
			{
				GameDataBridge.AddMethodCall<string, bool, int>(-1, 12, 36, uiName, presetBool, presetInt);
			}

			// Token: 0x06010D84 RID: 68996 RVA: 0x00672C74 File Offset: 0x00670E74
			public static void SetIsQuickStartGame(bool flag)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 12, 37, flag);
			}

			// Token: 0x06010D85 RID: 68997 RVA: 0x00672C83 File Offset: 0x00670E83
			public static void TaiwuCollectWudangHeavenlyTreeSeed(sbyte resourceType)
			{
				GameDataBridge.AddMethodCall<sbyte>(-1, 12, 38, resourceType);
			}

			// Token: 0x06010D86 RID: 68998 RVA: 0x00672C92 File Offset: 0x00670E92
			public static void GetEventLogData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 12, 39);
			}

			// Token: 0x06010D87 RID: 68999 RVA: 0x00672CA0 File Offset: 0x00670EA0
			public static void StartNewDialog(IntPair charIds, string dialog, string rawResponseData, EventActorData leftActor, EventActorData rightActor, string leftName, string rightName, short merchantTemplateId)
			{
				GameDataBridge.AddMethodCall<IntPair, string, string, EventActorData, EventActorData, string, string, short>(-1, 12, 40, charIds, dialog, rawResponseData, leftActor, rightActor, leftName, rightName, merchantTemplateId);
			}

			// Token: 0x06010D88 RID: 69000 RVA: 0x00672CC5 File Offset: 0x00670EC5
			public static void TaiwuVillagerExpelled(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 12, 41, charId);
			}

			// Token: 0x06010D89 RID: 69001 RVA: 0x00672CD4 File Offset: 0x00670ED4
			public static void GmCmd_TaiwuCrossArchive()
			{
				GameDataBridge.AddMethodCall(-1, 12, 42);
			}

			// Token: 0x06010D8A RID: 69002 RVA: 0x00672CE2 File Offset: 0x00670EE2
			public static void TaiwuCrossArchiveFindMemory(sbyte type)
			{
				GameDataBridge.AddMethodCall<sbyte>(-1, 12, 43, type);
			}

			// Token: 0x06010D8B RID: 69003 RVA: 0x00672CF1 File Offset: 0x00670EF1
			public static void UserLoadDreamBackArchive()
			{
				GameDataBridge.AddMethodCall(-1, 12, 44);
			}

			// Token: 0x06010D8C RID: 69004 RVA: 0x00672CFF File Offset: 0x00670EFF
			public static void OperateInventoryItem(int charId, sbyte operationType, ItemDisplayData itemData)
			{
				GameDataBridge.AddMethodCall<int, sbyte, ItemDisplayData>(-1, 12, 45, charId, operationType, itemData);
			}

			// Token: 0x06010D8D RID: 69005 RVA: 0x00672D10 File Offset: 0x00670F10
			public static void SetItemSelectCount(string key, int count)
			{
				GameDataBridge.AddMethodCall<string, int>(-1, 12, 46, key, count);
			}

			// Token: 0x06010D8E RID: 69006 RVA: 0x00672D20 File Offset: 0x00670F20
			public static void SettlementTreasuryBuildingClicked(short templateId, byte currStatus, sbyte currPage)
			{
				GameDataBridge.AddMethodCall<short, byte, sbyte>(-1, 12, 47, templateId, currStatus, currPage);
			}

			// Token: 0x06010D8F RID: 69007 RVA: 0x00672D31 File Offset: 0x00670F31
			public static void SetListenerEventActionISerializableArg(string actionName, string key, ItemKey value)
			{
				GameDataBridge.AddMethodCall<string, string, ItemKey>(-1, 12, 48, actionName, key, value);
			}

			// Token: 0x06010D90 RID: 69008 RVA: 0x00672D42 File Offset: 0x00670F42
			public static void SetListenerEventActionIntArg(string actionName, string key, int value)
			{
				GameDataBridge.AddMethodCall<string, string, int>(-1, 12, 49, actionName, key, value);
			}

			// Token: 0x06010D91 RID: 69009 RVA: 0x00672D53 File Offset: 0x00670F53
			public static void SetListenerEventActionBoolArg(string actionName, string key, bool value)
			{
				GameDataBridge.AddMethodCall<string, string, bool>(-1, 12, 50, actionName, key, value);
			}

			// Token: 0x06010D92 RID: 69010 RVA: 0x00672D64 File Offset: 0x00670F64
			public static void SetListenerEventActionStringArg(string actionName, string key, string value)
			{
				GameDataBridge.AddMethodCall<string, string, string>(-1, 12, 51, actionName, key, value);
			}

			// Token: 0x06010D93 RID: 69011 RVA: 0x00672D75 File Offset: 0x00670F75
			public static void GetValidInteractionEventOptions(int listenerId, int targetCharId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 12, 52, targetCharId);
			}

			// Token: 0x06010D94 RID: 69012 RVA: 0x00672D84 File Offset: 0x00670F84
			public static void SetListenerEventActionIntListArg(string actionName, string key, IntList value)
			{
				GameDataBridge.AddMethodCall<string, string, IntList>(-1, 12, 53, actionName, key, value);
			}

			// Token: 0x06010D95 RID: 69013 RVA: 0x00672D95 File Offset: 0x00670F95
			public static void SetListenerEventActionItemKeyArg(string actionName, string key, ItemKey value)
			{
				GameDataBridge.AddMethodCall<string, string, ItemKey>(-1, 12, 54, actionName, key, value);
			}

			// Token: 0x06010D96 RID: 69014 RVA: 0x00672DA6 File Offset: 0x00670FA6
			public static void TriggerShixiangDrumEasterEgg()
			{
				GameDataBridge.AddMethodCall(-1, 12, 55);
			}

			// Token: 0x06010D97 RID: 69015 RVA: 0x00672DB4 File Offset: 0x00670FB4
			public static void InteractPrisoner(int characterId, int interactPrisonerType)
			{
				GameDataBridge.AddMethodCall<int, int>(-1, 12, 56, characterId, interactPrisonerType);
			}

			// Token: 0x06010D98 RID: 69016 RVA: 0x00672DC4 File Offset: 0x00670FC4
			public static void OnClickedSendPrisonBtn()
			{
				GameDataBridge.AddMethodCall(-1, 12, 57);
			}

			// Token: 0x06010D99 RID: 69017 RVA: 0x00672DD2 File Offset: 0x00670FD2
			public static void OnClickedPrisonBtn(short buildingTemplateId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 12, 58, buildingTemplateId);
			}

			// Token: 0x06010D9A RID: 69018 RVA: 0x00672DE1 File Offset: 0x00670FE1
			public static void SetCharacterMultSelectResult(string key, List<int> charIds, bool callComplete)
			{
				GameDataBridge.AddMethodCall<string, List<int>, bool>(-1, 12, 59, key, charIds, callComplete);
			}

			// Token: 0x06010D9B RID: 69019 RVA: 0x00672DF2 File Offset: 0x00670FF2
			public static void SetCricketBettingResult(bool ok, Wager wager, int index)
			{
				GameDataBridge.AddMethodCall<bool, Wager, int>(-1, 12, 60, ok, wager, index);
			}

			// Token: 0x06010D9C RID: 69020 RVA: 0x00672E03 File Offset: 0x00671003
			public static void GetImplementedFunctionIds(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 12, 61);
			}

			// Token: 0x06010D9D RID: 69021 RVA: 0x00672E11 File Offset: 0x00671011
			public static void SetEventScriptExecutionPause(bool isPaused)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 12, 62, isPaused);
			}

			// Token: 0x06010D9E RID: 69022 RVA: 0x00672E20 File Offset: 0x00671020
			public static void EventScriptExecuteNext()
			{
				GameDataBridge.AddMethodCall(-1, 12, 63);
			}

			// Token: 0x06010D9F RID: 69023 RVA: 0x00672E2E File Offset: 0x0067102E
			public static void GmCmd_TaiwuWantedSectPunished(sbyte orgTemplateId, sbyte severity)
			{
				GameDataBridge.AddMethodCall<sbyte, sbyte>(-1, 12, 64, orgTemplateId, severity);
			}

			// Token: 0x06010DA0 RID: 69024 RVA: 0x00672E3E File Offset: 0x0067103E
			public static void EventSelectContinue()
			{
				GameDataBridge.AddMethodCall(-1, 12, 65);
			}

			// Token: 0x06010DA1 RID: 69025 RVA: 0x00672E4C File Offset: 0x0067104C
			public static void SetSelectCount(int count)
			{
				GameDataBridge.AddMethodCall<int>(-1, 12, 66, count);
			}

			// Token: 0x06010DA2 RID: 69026 RVA: 0x00672E5B File Offset: 0x0067105B
			public static void SetListenerEventActionShortListArg(string actionName, string key, ShortList value)
			{
				GameDataBridge.AddMethodCall<string, string, ShortList>(-1, 12, 67, actionName, key, value);
			}

			// Token: 0x06010DA3 RID: 69027 RVA: 0x00672E6C File Offset: 0x0067106C
			public static void SetShowingEventShortListArg(string key, ShortList value)
			{
				GameDataBridge.AddMethodCall<string, ShortList>(-1, 12, 68, key, value);
			}

			// Token: 0x06010DA4 RID: 69028 RVA: 0x00672E7C File Offset: 0x0067107C
			public static void OnClickMapPickupEvent(Location location)
			{
				GameDataBridge.AddMethodCall<Location>(-1, 12, 69, location);
			}

			// Token: 0x06010DA5 RID: 69029 RVA: 0x00672E8B File Offset: 0x0067108B
			public static void OnClickMapPickupNormalEvent(Location location)
			{
				GameDataBridge.AddMethodCall<Location>(-1, 12, 70, location);
			}

			// Token: 0x06010DA6 RID: 69030 RVA: 0x00672E9A File Offset: 0x0067109A
			public static void OnClickDeportButton(int type, bool isGood)
			{
				GameDataBridge.AddMethodCall<int, bool>(-1, 12, 71, type, isGood);
			}

			// Token: 0x06010DA7 RID: 69031 RVA: 0x00672EAA File Offset: 0x006710AA
			public static void OnSwitchToGuardedPage(byte currStatus, sbyte currPage)
			{
				GameDataBridge.AddMethodCall<byte, sbyte>(-1, 12, 72, currStatus, currPage);
			}

			// Token: 0x06010DA8 RID: 69032 RVA: 0x00672EBA File Offset: 0x006710BA
			public static void GmCmd_AddJieqingMaskCharId(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 12, 73, charId);
			}

			// Token: 0x06010DA9 RID: 69033 RVA: 0x00672EC9 File Offset: 0x006710C9
			public static void GmCmd_RemoveJieqingMaskCharId(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 12, 74, charId);
			}

			// Token: 0x06010DAA RID: 69034 RVA: 0x00672ED8 File Offset: 0x006710D8
			public static void EventCommonOptionSelect(short templateId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 12, 75, templateId);
			}

			// Token: 0x06010DAB RID: 69035 RVA: 0x00672EE7 File Offset: 0x006710E7
			public static void JumpToInteractionEventOption(int listenerId, int targetCharId, short customButtonTemplateId)
			{
				GameDataBridge.AddMethodCall<int, short>(listenerId, 12, 76, targetCharId, customButtonTemplateId);
			}

			// Token: 0x06010DAC RID: 69036 RVA: 0x00672EF7 File Offset: 0x006710F7
			public static void JumpToInteractionEventOptionByInteractionId(int listenerId, int targetCharId, short targetTemplateId)
			{
				GameDataBridge.AddMethodCall<int, short>(listenerId, 12, 77, targetCharId, targetTemplateId);
			}

			// Token: 0x06010DAD RID: 69037 RVA: 0x00672F07 File Offset: 0x00671107
			public static void OnTaiwuTryInvite(int charId, Location location)
			{
				GameDataBridge.AddMethodCall<int, Location>(-1, 12, 78, charId, location);
			}

			// Token: 0x06010DAE RID: 69038 RVA: 0x00672F17 File Offset: 0x00671117
			public static void ReloadConchShipEvents(List<string> packageNameList)
			{
				GameDataBridge.AddMethodCall<List<string>>(-1, 12, 79, packageNameList);
			}

			// Token: 0x06010DAF RID: 69039 RVA: 0x00672F26 File Offset: 0x00671126
			public static void OnClickMapPickupBatchEvent(BatchMapPickupInfo pickupInfo)
			{
				GameDataBridge.AddMethodCall<BatchMapPickupInfo>(-1, 12, 80, pickupInfo);
			}

			// Token: 0x06010DB0 RID: 69040 RVA: 0x00672F35 File Offset: 0x00671135
			public static void GmCmd_GetGlobalArgBoxInt(int listenerId, string key)
			{
				GameDataBridge.AddMethodCall<string>(listenerId, 12, 81, key);
			}

			// Token: 0x06010DB1 RID: 69041 RVA: 0x00672F44 File Offset: 0x00671144
			public static void GmCmd_SetGlobalArgBoxInt(string key, int value)
			{
				GameDataBridge.AddMethodCall<string, int>(-1, 12, 82, key, value);
			}

			// Token: 0x06010DB2 RID: 69042 RVA: 0x00672F54 File Offset: 0x00671154
			public static void CheckIsShowingEvent(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 12, 83);
			}

			// Token: 0x06010DB3 RID: 69043 RVA: 0x00672F62 File Offset: 0x00671162
			public static void OnClickChickenCoop()
			{
				GameDataBridge.AddMethodCall(-1, 12, 84);
			}

			// Token: 0x06010DB4 RID: 69044 RVA: 0x00672F70 File Offset: 0x00671170
			public static void SetShowingEventItemKeyArg(string key, ItemKey value)
			{
				GameDataBridge.AddMethodCall<string, ItemKey>(-1, 12, 85, key, value);
			}

			// Token: 0x06010DB5 RID: 69045 RVA: 0x00672F80 File Offset: 0x00671180
			public static void SetShowingEventShortArg(string key, short value)
			{
				GameDataBridge.AddMethodCall<string, short>(-1, 12, 86, key, value);
			}

			// Token: 0x06010DB6 RID: 69046 RVA: 0x00672F90 File Offset: 0x00671190
			public static void OnEnterBuildingArea(Location location)
			{
				GameDataBridge.AddMethodCall<Location>(-1, 12, 87, location);
			}

			// Token: 0x06010DB7 RID: 69047 RVA: 0x00672F9F File Offset: 0x0067119F
			public static void UpdateShowingEventTaiwuCharacterDisplayData()
			{
				GameDataBridge.AddMethodCall(-1, 12, 88);
			}

			// Token: 0x06010DB8 RID: 69048 RVA: 0x00672FAD File Offset: 0x006711AD
			public static void EventCommonOptionPreview(short templateId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 12, 89, templateId);
			}

			// Token: 0x06010DB9 RID: 69049 RVA: 0x00672FBC File Offset: 0x006711BC
			public static void GmCmd_TravelToPastTaiwuVillage()
			{
				GameDataBridge.AddMethodCall(-1, 12, 90);
			}

			// Token: 0x06010DBA RID: 69050 RVA: 0x00672FCA File Offset: 0x006711CA
			public static void GmCmd_BackFromPastTaiwuVillage()
			{
				GameDataBridge.AddMethodCall(-1, 12, 91);
			}

			// Token: 0x06010DBB RID: 69051 RVA: 0x00672FD8 File Offset: 0x006711D8
			public static void EventCommonOptionHaveAvailableOption(int listenerId, short templateId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 12, 92, templateId);
			}

			// Token: 0x06010DBC RID: 69052 RVA: 0x00672FE7 File Offset: 0x006711E7
			public static void GmCmd_TriggerOvercomeCombatOver(int combatResult, int combatType, int mainEnemyId)
			{
				GameDataBridge.AddMethodCall<int, int, int>(-1, 12, 93, combatResult, combatType, mainEnemyId);
			}
		}

		// Token: 0x020025F6 RID: 9718
		public static class AsyncCall
		{
			// Token: 0x06010DBD RID: 69053 RVA: 0x00672FF8 File Offset: 0x006711F8
			public static void GetMonthlyActionStateAndTime(IAsyncMethodRequestHandler requestHandler, MonthlyActionKey key, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<MonthlyActionKey>(12, 0, key, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010DBE RID: 69054 RVA: 0x00673023 File Offset: 0x00671223
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.InitConchShipEvents instead.", true)]
			public static void InitConchShipEvents(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DBF RID: 69055 RVA: 0x0067302B File Offset: 0x0067122B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.TriggerListener instead.", true)]
			public static void TriggerListener(IAsyncMethodRequestHandler requestHandler, string key, bool value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DC0 RID: 69056 RVA: 0x00673033 File Offset: 0x00671233
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.SetItemSelectResult instead.", true)]
			public static void SetItemSelectResult(IAsyncMethodRequestHandler requestHandler, string key, ItemKey itemKey, bool callComplete, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DC1 RID: 69057 RVA: 0x0067303B File Offset: 0x0067123B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.SetCharacterSelectResult instead.", true)]
			public static void SetCharacterSelectResult(IAsyncMethodRequestHandler requestHandler, string key, int charId, bool callComplete, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DC2 RID: 69058 RVA: 0x00673043 File Offset: 0x00671243
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.SetSecretInformationSelectResult instead.", true)]
			public static void SetSecretInformationSelectResult(IAsyncMethodRequestHandler requestHandler, string key, int secretId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DC3 RID: 69059 RVA: 0x0067304B File Offset: 0x0067124B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.SetNormalInformationSelectResult instead.", true)]
			public static void SetNormalInformationSelectResult(IAsyncMethodRequestHandler requestHandler, string key, NormalInformation normalInformation, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DC4 RID: 69060 RVA: 0x00673053 File Offset: 0x00671253
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.StartHandleEventDuringAdvance instead.", true)]
			public static void StartHandleEventDuringAdvance(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DC5 RID: 69061 RVA: 0x0067305C File Offset: 0x0067125C
			public static void GetTriggeredEventSummaryDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(12, 8, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010DC6 RID: 69062 RVA: 0x00673086 File Offset: 0x00671286
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.SetEventInProcessing instead.", true)]
			public static void SetEventInProcessing(IAsyncMethodRequestHandler requestHandler, string eventGuid, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DC7 RID: 69063 RVA: 0x0067308E File Offset: 0x0067128E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.EventSelect instead.", true)]
			public static void EventSelect(IAsyncMethodRequestHandler requestHandler, string eventGuid, string optionKey, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DC8 RID: 69064 RVA: 0x00673096 File Offset: 0x00671296
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.EventSelect instead.", true)]
			public static void EventSelect(IAsyncMethodRequestHandler requestHandler, string eventGuid, string optionKey, bool isContinue, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DC9 RID: 69065 RVA: 0x006730A0 File Offset: 0x006712A0
			public static void GetEventDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(12, 11, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010DCA RID: 69066 RVA: 0x006730CB File Offset: 0x006712CB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.GmCmd_SaveMonthlyActionManager instead.", true)]
			public static void GmCmd_SaveMonthlyActionManager(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DCB RID: 69067 RVA: 0x006730D3 File Offset: 0x006712D3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.OnCharacterClicked instead.", true)]
			public static void OnCharacterClicked(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DCC RID: 69068 RVA: 0x006730DB File Offset: 0x006712DB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.OnLetTeammateLeaveGroup instead.", true)]
			public static void OnLetTeammateLeaveGroup(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DCD RID: 69069 RVA: 0x006730E3 File Offset: 0x006712E3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.OnInteractCaravan instead.", true)]
			public static void OnInteractCaravan(IAsyncMethodRequestHandler requestHandler, int caravanId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DCE RID: 69070 RVA: 0x006730EB File Offset: 0x006712EB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.OnInteractKidnappedCharacter instead.", true)]
			public static void OnInteractKidnappedCharacter(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DCF RID: 69071 RVA: 0x006730F3 File Offset: 0x006712F3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.OnSectBuildingClicked instead.", true)]
			public static void OnSectBuildingClicked(IAsyncMethodRequestHandler requestHandler, short buildingTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DD0 RID: 69072 RVA: 0x006730FB File Offset: 0x006712FB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.OnRecordEnterGame instead.", true)]
			public static void OnRecordEnterGame(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DD1 RID: 69073 RVA: 0x00673103 File Offset: 0x00671303
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.OnNewGameMonth instead.", true)]
			public static void OnNewGameMonth(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DD2 RID: 69074 RVA: 0x0067310B File Offset: 0x0067130B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.OnCombatWithXiangshuMinionComplete instead.", true)]
			public static void OnCombatWithXiangshuMinionComplete(IAsyncMethodRequestHandler requestHandler, short templateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DD3 RID: 69075 RVA: 0x00673113 File Offset: 0x00671313
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.OnBlackMaskAnimationComplete instead.", true)]
			public static void OnBlackMaskAnimationComplete(IAsyncMethodRequestHandler requestHandler, bool maskVisible, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DD4 RID: 69076 RVA: 0x0067311B File Offset: 0x0067131B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.OnMakingSystemOpened instead.", true)]
			public static void OnMakingSystemOpened(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, short templateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DD5 RID: 69077 RVA: 0x00673123 File Offset: 0x00671323
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.OnCollectedMakingSystemItem instead.", true)]
			public static void OnCollectedMakingSystemItem(IAsyncMethodRequestHandler requestHandler, BuildingBlockKey blockKey, short templateId, bool showingGetItem, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DD6 RID: 69078 RVA: 0x0067312B File Offset: 0x0067132B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.OnSectSpecialBuildingClicked instead.", true)]
			public static void OnSectSpecialBuildingClicked(IAsyncMethodRequestHandler requestHandler, short templateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DD7 RID: 69079 RVA: 0x00673133 File Offset: 0x00671333
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.AnimalAvatarClicked instead.", true)]
			public static void AnimalAvatarClicked(IAsyncMethodRequestHandler requestHandler, int animalId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DD8 RID: 69080 RVA: 0x0067313B File Offset: 0x0067133B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.MainStoryFinishCatchCricket instead.", true)]
			public static void MainStoryFinishCatchCricket(IAsyncMethodRequestHandler requestHandler, bool result, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DD9 RID: 69081 RVA: 0x00673143 File Offset: 0x00671343
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.LoadEventsFromPath instead.", true)]
			public static void LoadEventsFromPath(IAsyncMethodRequestHandler requestHandler, string eventDataDirectory, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DDA RID: 69082 RVA: 0x0067314B File Offset: 0x0067134B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.NpcTombClicked instead.", true)]
			public static void NpcTombClicked(IAsyncMethodRequestHandler requestHandler, int tombId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DDB RID: 69083 RVA: 0x00673153 File Offset: 0x00671353
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.SetLifeSkillSelectResult instead.", true)]
			public static void SetLifeSkillSelectResult(IAsyncMethodRequestHandler requestHandler, short lifeSkillId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DDC RID: 69084 RVA: 0x0067315B File Offset: 0x0067135B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.SetCombatSkillSelectResult instead.", true)]
			public static void SetCombatSkillSelectResult(IAsyncMethodRequestHandler requestHandler, short combatSkillId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DDD RID: 69085 RVA: 0x00673163 File Offset: 0x00671363
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.OnLifeSkillCombatForceSilent instead.", true)]
			public static void OnLifeSkillCombatForceSilent(IAsyncMethodRequestHandler requestHandler, int charId, sbyte concessionCount, sbyte inducementCount, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DDE RID: 69086 RVA: 0x0067316B File Offset: 0x0067136B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.TryMoveWhenMoveDisable instead.", true)]
			public static void TryMoveWhenMoveDisable(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DDF RID: 69087 RVA: 0x00673173 File Offset: 0x00671373
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.TryMoveToInvalidLocationInTutorial instead.", true)]
			public static void TryMoveToInvalidLocationInTutorial(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DE0 RID: 69088 RVA: 0x0067317B File Offset: 0x0067137B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.SetCharacterSetSelectResult instead.", true)]
			public static void SetCharacterSetSelectResult(IAsyncMethodRequestHandler requestHandler, string actionName, string key, CharacterSet characterSet, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DE1 RID: 69089 RVA: 0x00673183 File Offset: 0x00671383
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.OnCharacterTemplateClicked instead.", true)]
			public static void OnCharacterTemplateClicked(IAsyncMethodRequestHandler requestHandler, short characterTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DE2 RID: 69090 RVA: 0x0067318B File Offset: 0x0067138B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.CloseUI instead.", true)]
			public static void CloseUI(IAsyncMethodRequestHandler requestHandler, string uiName, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DE3 RID: 69091 RVA: 0x00673193 File Offset: 0x00671393
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.CloseUI instead.", true)]
			public static void CloseUI(IAsyncMethodRequestHandler requestHandler, string uiName, bool presetBool, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DE4 RID: 69092 RVA: 0x0067319B File Offset: 0x0067139B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.CloseUI instead.", true)]
			public static void CloseUI(IAsyncMethodRequestHandler requestHandler, string uiName, bool presetBool, int presetInt, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DE5 RID: 69093 RVA: 0x006731A3 File Offset: 0x006713A3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.SetIsQuickStartGame instead.", true)]
			public static void SetIsQuickStartGame(IAsyncMethodRequestHandler requestHandler, bool flag, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DE6 RID: 69094 RVA: 0x006731AB File Offset: 0x006713AB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.TaiwuCollectWudangHeavenlyTreeSeed instead.", true)]
			public static void TaiwuCollectWudangHeavenlyTreeSeed(IAsyncMethodRequestHandler requestHandler, sbyte resourceType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DE7 RID: 69095 RVA: 0x006731B4 File Offset: 0x006713B4
			public static void GetEventLogData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(12, 39, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010DE8 RID: 69096 RVA: 0x006731DF File Offset: 0x006713DF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.StartNewDialog instead.", true)]
			public static void StartNewDialog(IAsyncMethodRequestHandler requestHandler, IntPair charIds, string dialog, string rawResponseData, EventActorData leftActor, EventActorData rightActor, string leftName, string rightName, short merchantTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DE9 RID: 69097 RVA: 0x006731E7 File Offset: 0x006713E7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.TaiwuVillagerExpelled instead.", true)]
			public static void TaiwuVillagerExpelled(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DEA RID: 69098 RVA: 0x006731EF File Offset: 0x006713EF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.GmCmd_TaiwuCrossArchive instead.", true)]
			public static void GmCmd_TaiwuCrossArchive(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DEB RID: 69099 RVA: 0x006731F7 File Offset: 0x006713F7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.TaiwuCrossArchiveFindMemory instead.", true)]
			public static void TaiwuCrossArchiveFindMemory(IAsyncMethodRequestHandler requestHandler, sbyte type, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DEC RID: 69100 RVA: 0x006731FF File Offset: 0x006713FF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.UserLoadDreamBackArchive instead.", true)]
			public static void UserLoadDreamBackArchive(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DED RID: 69101 RVA: 0x00673207 File Offset: 0x00671407
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.OperateInventoryItem instead.", true)]
			public static void OperateInventoryItem(IAsyncMethodRequestHandler requestHandler, int charId, sbyte operationType, ItemDisplayData itemData, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DEE RID: 69102 RVA: 0x0067320F File Offset: 0x0067140F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.SetItemSelectCount instead.", true)]
			public static void SetItemSelectCount(IAsyncMethodRequestHandler requestHandler, string key, int count, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DEF RID: 69103 RVA: 0x00673217 File Offset: 0x00671417
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.SettlementTreasuryBuildingClicked instead.", true)]
			public static void SettlementTreasuryBuildingClicked(IAsyncMethodRequestHandler requestHandler, short templateId, byte currStatus, sbyte currPage, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DF0 RID: 69104 RVA: 0x0067321F File Offset: 0x0067141F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.SetListenerEventActionISerializableArg instead.", true)]
			public static void SetListenerEventActionISerializableArg(IAsyncMethodRequestHandler requestHandler, string actionName, string key, ItemKey value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DF1 RID: 69105 RVA: 0x00673227 File Offset: 0x00671427
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.SetListenerEventActionIntArg instead.", true)]
			public static void SetListenerEventActionIntArg(IAsyncMethodRequestHandler requestHandler, string actionName, string key, int value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DF2 RID: 69106 RVA: 0x0067322F File Offset: 0x0067142F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.SetListenerEventActionBoolArg instead.", true)]
			public static void SetListenerEventActionBoolArg(IAsyncMethodRequestHandler requestHandler, string actionName, string key, bool value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DF3 RID: 69107 RVA: 0x00673237 File Offset: 0x00671437
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.SetListenerEventActionStringArg instead.", true)]
			public static void SetListenerEventActionStringArg(IAsyncMethodRequestHandler requestHandler, string actionName, string key, string value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DF4 RID: 69108 RVA: 0x00673240 File Offset: 0x00671440
			public static void GetValidInteractionEventOptions(IAsyncMethodRequestHandler requestHandler, int targetCharId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(12, 52, targetCharId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010DF5 RID: 69109 RVA: 0x0067326C File Offset: 0x0067146C
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.SetListenerEventActionIntListArg instead.", true)]
			public static void SetListenerEventActionIntListArg(IAsyncMethodRequestHandler requestHandler, string actionName, string key, IntList value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DF6 RID: 69110 RVA: 0x00673274 File Offset: 0x00671474
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.SetListenerEventActionItemKeyArg instead.", true)]
			public static void SetListenerEventActionItemKeyArg(IAsyncMethodRequestHandler requestHandler, string actionName, string key, ItemKey value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DF7 RID: 69111 RVA: 0x0067327C File Offset: 0x0067147C
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.TriggerShixiangDrumEasterEgg instead.", true)]
			public static void TriggerShixiangDrumEasterEgg(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DF8 RID: 69112 RVA: 0x00673284 File Offset: 0x00671484
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.InteractPrisoner instead.", true)]
			public static void InteractPrisoner(IAsyncMethodRequestHandler requestHandler, int characterId, int interactPrisonerType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DF9 RID: 69113 RVA: 0x0067328C File Offset: 0x0067148C
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.OnClickedSendPrisonBtn instead.", true)]
			public static void OnClickedSendPrisonBtn(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DFA RID: 69114 RVA: 0x00673294 File Offset: 0x00671494
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.OnClickedPrisonBtn instead.", true)]
			public static void OnClickedPrisonBtn(IAsyncMethodRequestHandler requestHandler, short buildingTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DFB RID: 69115 RVA: 0x0067329C File Offset: 0x0067149C
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.SetCharacterMultSelectResult instead.", true)]
			public static void SetCharacterMultSelectResult(IAsyncMethodRequestHandler requestHandler, string key, List<int> charIds, bool callComplete, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DFC RID: 69116 RVA: 0x006732A4 File Offset: 0x006714A4
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.SetCricketBettingResult instead.", true)]
			public static void SetCricketBettingResult(IAsyncMethodRequestHandler requestHandler, bool ok, Wager wager, int index, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DFD RID: 69117 RVA: 0x006732AC File Offset: 0x006714AC
			public static void GetImplementedFunctionIds(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(12, 61, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010DFE RID: 69118 RVA: 0x006732D7 File Offset: 0x006714D7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.SetEventScriptExecutionPause instead.", true)]
			public static void SetEventScriptExecutionPause(IAsyncMethodRequestHandler requestHandler, bool isPaused, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010DFF RID: 69119 RVA: 0x006732DF File Offset: 0x006714DF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.EventScriptExecuteNext instead.", true)]
			public static void EventScriptExecuteNext(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010E00 RID: 69120 RVA: 0x006732E7 File Offset: 0x006714E7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.GmCmd_TaiwuWantedSectPunished instead.", true)]
			public static void GmCmd_TaiwuWantedSectPunished(IAsyncMethodRequestHandler requestHandler, sbyte orgTemplateId, sbyte severity, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010E01 RID: 69121 RVA: 0x006732EF File Offset: 0x006714EF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.EventSelectContinue instead.", true)]
			public static void EventSelectContinue(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010E02 RID: 69122 RVA: 0x006732F7 File Offset: 0x006714F7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.SetSelectCount instead.", true)]
			public static void SetSelectCount(IAsyncMethodRequestHandler requestHandler, int count, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010E03 RID: 69123 RVA: 0x006732FF File Offset: 0x006714FF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.SetListenerEventActionShortListArg instead.", true)]
			public static void SetListenerEventActionShortListArg(IAsyncMethodRequestHandler requestHandler, string actionName, string key, ShortList value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010E04 RID: 69124 RVA: 0x00673307 File Offset: 0x00671507
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.SetShowingEventShortListArg instead.", true)]
			public static void SetShowingEventShortListArg(IAsyncMethodRequestHandler requestHandler, string key, ShortList value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010E05 RID: 69125 RVA: 0x0067330F File Offset: 0x0067150F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.OnClickMapPickupEvent instead.", true)]
			public static void OnClickMapPickupEvent(IAsyncMethodRequestHandler requestHandler, Location location, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010E06 RID: 69126 RVA: 0x00673317 File Offset: 0x00671517
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.OnClickMapPickupNormalEvent instead.", true)]
			public static void OnClickMapPickupNormalEvent(IAsyncMethodRequestHandler requestHandler, Location location, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010E07 RID: 69127 RVA: 0x0067331F File Offset: 0x0067151F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.OnClickDeportButton instead.", true)]
			public static void OnClickDeportButton(IAsyncMethodRequestHandler requestHandler, int type, bool isGood, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010E08 RID: 69128 RVA: 0x00673327 File Offset: 0x00671527
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.OnSwitchToGuardedPage instead.", true)]
			public static void OnSwitchToGuardedPage(IAsyncMethodRequestHandler requestHandler, byte currStatus, sbyte currPage, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010E09 RID: 69129 RVA: 0x0067332F File Offset: 0x0067152F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.GmCmd_AddJieqingMaskCharId instead.", true)]
			public static void GmCmd_AddJieqingMaskCharId(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010E0A RID: 69130 RVA: 0x00673337 File Offset: 0x00671537
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.GmCmd_RemoveJieqingMaskCharId instead.", true)]
			public static void GmCmd_RemoveJieqingMaskCharId(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010E0B RID: 69131 RVA: 0x0067333F File Offset: 0x0067153F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.EventCommonOptionSelect instead.", true)]
			public static void EventCommonOptionSelect(IAsyncMethodRequestHandler requestHandler, short templateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010E0C RID: 69132 RVA: 0x00673348 File Offset: 0x00671548
			public static void JumpToInteractionEventOption(IAsyncMethodRequestHandler requestHandler, int targetCharId, short customButtonTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short>(12, 76, targetCharId, customButtonTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010E0D RID: 69133 RVA: 0x00673378 File Offset: 0x00671578
			public static void JumpToInteractionEventOptionByInteractionId(IAsyncMethodRequestHandler requestHandler, int targetCharId, short targetTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short>(12, 77, targetCharId, targetTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010E0E RID: 69134 RVA: 0x006733A5 File Offset: 0x006715A5
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.OnTaiwuTryInvite instead.", true)]
			public static void OnTaiwuTryInvite(IAsyncMethodRequestHandler requestHandler, int charId, Location location, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010E0F RID: 69135 RVA: 0x006733AD File Offset: 0x006715AD
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.ReloadConchShipEvents instead.", true)]
			public static void ReloadConchShipEvents(IAsyncMethodRequestHandler requestHandler, List<string> packageNameList, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010E10 RID: 69136 RVA: 0x006733B5 File Offset: 0x006715B5
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.OnClickMapPickupBatchEvent instead.", true)]
			public static void OnClickMapPickupBatchEvent(IAsyncMethodRequestHandler requestHandler, BatchMapPickupInfo pickupInfo, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010E11 RID: 69137 RVA: 0x006733C0 File Offset: 0x006715C0
			public static void GmCmd_GetGlobalArgBoxInt(IAsyncMethodRequestHandler requestHandler, string key, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<string>(12, 81, key, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010E12 RID: 69138 RVA: 0x006733EC File Offset: 0x006715EC
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.GmCmd_SetGlobalArgBoxInt instead.", true)]
			public static void GmCmd_SetGlobalArgBoxInt(IAsyncMethodRequestHandler requestHandler, string key, int value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010E13 RID: 69139 RVA: 0x006733F4 File Offset: 0x006715F4
			public static void CheckIsShowingEvent(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(12, 83, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010E14 RID: 69140 RVA: 0x0067341F File Offset: 0x0067161F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.OnClickChickenCoop instead.", true)]
			public static void OnClickChickenCoop(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010E15 RID: 69141 RVA: 0x00673427 File Offset: 0x00671627
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.SetShowingEventItemKeyArg instead.", true)]
			public static void SetShowingEventItemKeyArg(IAsyncMethodRequestHandler requestHandler, string key, ItemKey value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010E16 RID: 69142 RVA: 0x0067342F File Offset: 0x0067162F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.SetShowingEventShortArg instead.", true)]
			public static void SetShowingEventShortArg(IAsyncMethodRequestHandler requestHandler, string key, short value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010E17 RID: 69143 RVA: 0x00673437 File Offset: 0x00671637
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.OnEnterBuildingArea instead.", true)]
			public static void OnEnterBuildingArea(IAsyncMethodRequestHandler requestHandler, Location location, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010E18 RID: 69144 RVA: 0x0067343F File Offset: 0x0067163F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.UpdateShowingEventTaiwuCharacterDisplayData instead.", true)]
			public static void UpdateShowingEventTaiwuCharacterDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010E19 RID: 69145 RVA: 0x00673447 File Offset: 0x00671647
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.EventCommonOptionPreview instead.", true)]
			public static void EventCommonOptionPreview(IAsyncMethodRequestHandler requestHandler, short templateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010E1A RID: 69146 RVA: 0x0067344F File Offset: 0x0067164F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.GmCmd_TravelToPastTaiwuVillage instead.", true)]
			public static void GmCmd_TravelToPastTaiwuVillage(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010E1B RID: 69147 RVA: 0x00673457 File Offset: 0x00671657
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.GmCmd_BackFromPastTaiwuVillage instead.", true)]
			public static void GmCmd_BackFromPastTaiwuVillage(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010E1C RID: 69148 RVA: 0x00673460 File Offset: 0x00671660
			public static void EventCommonOptionHaveAvailableOption(IAsyncMethodRequestHandler requestHandler, short templateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(12, 92, templateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010E1D RID: 69149 RVA: 0x0067348C File Offset: 0x0067168C
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TaiwuEventDomainMethod.Call.GmCmd_TriggerOvercomeCombatOver instead.", true)]
			public static void GmCmd_TriggerOvercomeCombatOver(IAsyncMethodRequestHandler requestHandler, int combatResult, int combatType, int mainEnemyId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}
		}
	}
}
