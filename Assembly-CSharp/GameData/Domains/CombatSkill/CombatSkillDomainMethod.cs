using System;
using System.Collections.Generic;
using GameData.GameDataBridge;

namespace GameData.Domains.CombatSkill
{
	// Token: 0x02000FCB RID: 4043
	public static class CombatSkillDomainMethod
	{
		// Token: 0x02002611 RID: 9745
		public static class Call
		{
			// Token: 0x06011550 RID: 70992 RVA: 0x0067DEC3 File Offset: 0x0067C0C3
			public static void GetCombatSkillDisplayData(int listenerId, int charId, List<short> skillTemplateIdList)
			{
				GameDataBridge.AddMethodCall<int, List<short>>(listenerId, 7, 0, charId, skillTemplateIdList);
			}

			// Token: 0x06011551 RID: 70993 RVA: 0x0067DED1 File Offset: 0x0067C0D1
			public static void GetCombatSkillBreakStepCount(int listenerId, int charId, short skillTemplateId)
			{
				GameDataBridge.AddMethodCall<int, short>(listenerId, 7, 1, charId, skillTemplateId);
			}

			// Token: 0x06011552 RID: 70994 RVA: 0x0067DEDF File Offset: 0x0067C0DF
			public static void GetCharacterEquipCombatSkillDisplayData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 7, 2, charId);
			}

			// Token: 0x06011553 RID: 70995 RVA: 0x0067DEEC File Offset: 0x0067C0EC
			public static void GetCombatSkillDisplayDataOnce(int listenerId, int charId, short skillTemplateId)
			{
				GameDataBridge.AddMethodCall<int, short>(listenerId, 7, 3, charId, skillTemplateId);
			}

			// Token: 0x06011554 RID: 70996 RVA: 0x0067DEFA File Offset: 0x0067C0FA
			public static void GetEffectDescriptionData(int listenerId, int charId, List<short> skillIds)
			{
				GameDataBridge.AddMethodCall<int, List<short>>(listenerId, 7, 4, charId, skillIds);
			}

			// Token: 0x06011555 RID: 70997 RVA: 0x0067DF08 File Offset: 0x0067C108
			public static void CalcTaiwuExtraDeltaNeiliPerLoop(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 7, 5);
			}

			// Token: 0x06011556 RID: 70998 RVA: 0x0067DF14 File Offset: 0x0067C114
			public static void CalcTaiwuExtraDeltaNeiliAllocationPerLoop(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 7, 6);
			}

			// Token: 0x06011557 RID: 70999 RVA: 0x0067DF20 File Offset: 0x0067C120
			public static void GetCombatSkillPreviewDisplayDataOnce(int listenerId, int charId, short skillTemplateId)
			{
				GameDataBridge.AddMethodCall<int, short>(listenerId, 7, 7, charId, skillTemplateId);
			}

			// Token: 0x06011558 RID: 71000 RVA: 0x0067DF2E File Offset: 0x0067C12E
			public static void GetCombatSkillBreakoutStepsMaxPower(int listenerId, int charId, short skillTemplateId)
			{
				GameDataBridge.AddMethodCall<int, short>(listenerId, 7, 8, charId, skillTemplateId);
			}

			// Token: 0x06011559 RID: 71001 RVA: 0x0067DF3C File Offset: 0x0067C13C
			public static void GetCombatSkillBreakBonuses(int listenerId, int charId, short skillTemplateId)
			{
				GameDataBridge.AddMethodCall<int, short>(listenerId, 7, 9, charId, skillTemplateId);
			}

			// Token: 0x0601155A RID: 71002 RVA: 0x0067DF4B File Offset: 0x0067C14B
			public static void SetActivePage(int listenerId, int charId, short skillId, byte pageId, sbyte direction)
			{
				GameDataBridge.AddMethodCall<int, short, byte, sbyte>(listenerId, 7, 10, charId, skillId, pageId, direction);
			}

			// Token: 0x0601155B RID: 71003 RVA: 0x0067DF5D File Offset: 0x0067C15D
			public static void DeActivePage(int listenerId, int charId, short skillId, byte pageId, sbyte direction)
			{
				GameDataBridge.AddMethodCall<int, short, byte, sbyte>(listenerId, 7, 11, charId, skillId, pageId, direction);
			}

			// Token: 0x0601155C RID: 71004 RVA: 0x0067DF6F File Offset: 0x0067C16F
			public static void CalcTaiwuCombatSkillBreakSuccessRate(int listenerId, short skillTemplateId)
			{
				GameDataBridge.AddMethodCall<short>(listenerId, 7, 12, skillTemplateId);
			}

			// Token: 0x0601155D RID: 71005 RVA: 0x0067DF7D File Offset: 0x0067C17D
			public static void CalcCombatSkillBreakAvailableStepsDisplayData(int listenerId, int charId, short skillTemplateId)
			{
				GameDataBridge.AddMethodCall<int, short>(listenerId, 7, 13, charId, skillTemplateId);
			}

			// Token: 0x0601155E RID: 71006 RVA: 0x0067DF8C File Offset: 0x0067C18C
			public static void GetEquipCombatSkillDisplayData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 7, 14, charId);
			}

			// Token: 0x0601155F RID: 71007 RVA: 0x0067DF9A File Offset: 0x0067C19A
			public static void GetCharacterMenuCombatSkillListItemDisplayData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 7, 15, charId);
			}

			// Token: 0x06011560 RID: 71008 RVA: 0x0067DFA8 File Offset: 0x0067C1A8
			public static void GetLoopingTransferNeiliProportionOfFiveElementsDataForTaiwu(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 7, 16);
			}

			// Token: 0x06011561 RID: 71009 RVA: 0x0067DFB5 File Offset: 0x0067C1B5
			public static void GetCombatSkillDisplayDataForPractice(int listenerId, int charId, short skillTemplateId)
			{
				GameDataBridge.AddMethodCall<int, short>(listenerId, 7, 17, charId, skillTemplateId);
			}

			// Token: 0x06011562 RID: 71010 RVA: 0x0067DFC4 File Offset: 0x0067C1C4
			public static void CalcTaiwuExtraDeltaNeiliAllocationLoops(int listenerId, int combatSkillId, int totalLoopsCount)
			{
				GameDataBridge.AddMethodCall<int, int>(listenerId, 7, 18, combatSkillId, totalLoopsCount);
			}

			// Token: 0x06011563 RID: 71011 RVA: 0x0067DFD3 File Offset: 0x0067C1D3
			public static void GetLearnedCombatSkillByType(int listenerId, int charId, sbyte skillType)
			{
				GameDataBridge.AddMethodCall<int, sbyte>(listenerId, 7, 19, charId, skillType);
			}

			// Token: 0x06011564 RID: 71012 RVA: 0x0067DFE2 File Offset: 0x0067C1E2
			public static void GetCombatSkillDisplayDataForListOnce(int listenerId, int charId, short skillTemplateId)
			{
				GameDataBridge.AddMethodCall<int, short>(listenerId, 7, 20, charId, skillTemplateId);
			}

			// Token: 0x06011565 RID: 71013 RVA: 0x0067DFF1 File Offset: 0x0067C1F1
			public static void GetCombatSkillDisplayDataForList(int listenerId, int charId, List<short> skillTemplateIdList)
			{
				GameDataBridge.AddMethodCall<int, List<short>>(listenerId, 7, 21, charId, skillTemplateIdList);
			}

			// Token: 0x06011566 RID: 71014 RVA: 0x0067E000 File Offset: 0x0067C200
			public static void GetCombatSkillEquipment(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 7, 22, charId);
			}

			// Token: 0x06011567 RID: 71015 RVA: 0x0067E00E File Offset: 0x0067C20E
			public static void GetCharacterEquipNeigongBreakList(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 7, 23, charId);
			}

			// Token: 0x06011568 RID: 71016 RVA: 0x0067E01C File Offset: 0x0067C21C
			public static void GetCharacterEquipAssistanceBreakList(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 7, 24, charId);
			}
		}

		// Token: 0x02002612 RID: 9746
		public static class AsyncCall
		{
			// Token: 0x06011569 RID: 71017 RVA: 0x0067E02C File Offset: 0x0067C22C
			public static void GetCombatSkillDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, List<short> skillTemplateIdList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, List<short>>(7, 0, charId, skillTemplateIdList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601156A RID: 71018 RVA: 0x0067E058 File Offset: 0x0067C258
			public static void GetCombatSkillBreakStepCount(IAsyncMethodRequestHandler requestHandler, int charId, short skillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short>(7, 1, charId, skillTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601156B RID: 71019 RVA: 0x0067E084 File Offset: 0x0067C284
			public static void GetCharacterEquipCombatSkillDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(7, 2, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601156C RID: 71020 RVA: 0x0067E0B0 File Offset: 0x0067C2B0
			public static void GetCombatSkillDisplayDataOnce(IAsyncMethodRequestHandler requestHandler, int charId, short skillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short>(7, 3, charId, skillTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601156D RID: 71021 RVA: 0x0067E0DC File Offset: 0x0067C2DC
			public static void GetEffectDescriptionData(IAsyncMethodRequestHandler requestHandler, int charId, List<short> skillIds, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, List<short>>(7, 4, charId, skillIds, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601156E RID: 71022 RVA: 0x0067E108 File Offset: 0x0067C308
			public static void CalcTaiwuExtraDeltaNeiliPerLoop(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(7, 5, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601156F RID: 71023 RVA: 0x0067E134 File Offset: 0x0067C334
			public static void CalcTaiwuExtraDeltaNeiliAllocationPerLoop(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(7, 6, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011570 RID: 71024 RVA: 0x0067E160 File Offset: 0x0067C360
			public static void GetCombatSkillPreviewDisplayDataOnce(IAsyncMethodRequestHandler requestHandler, int charId, short skillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short>(7, 7, charId, skillTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011571 RID: 71025 RVA: 0x0067E18C File Offset: 0x0067C38C
			public static void GetCombatSkillBreakoutStepsMaxPower(IAsyncMethodRequestHandler requestHandler, int charId, short skillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short>(7, 8, charId, skillTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011572 RID: 71026 RVA: 0x0067E1B8 File Offset: 0x0067C3B8
			public static void GetCombatSkillBreakBonuses(IAsyncMethodRequestHandler requestHandler, int charId, short skillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short>(7, 9, charId, skillTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011573 RID: 71027 RVA: 0x0067E1E4 File Offset: 0x0067C3E4
			public static void SetActivePage(IAsyncMethodRequestHandler requestHandler, int charId, short skillId, byte pageId, sbyte direction, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short, byte, sbyte>(7, 10, charId, skillId, pageId, direction, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011574 RID: 71028 RVA: 0x0067E214 File Offset: 0x0067C414
			public static void DeActivePage(IAsyncMethodRequestHandler requestHandler, int charId, short skillId, byte pageId, sbyte direction, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short, byte, sbyte>(7, 11, charId, skillId, pageId, direction, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011575 RID: 71029 RVA: 0x0067E244 File Offset: 0x0067C444
			public static void CalcTaiwuCombatSkillBreakSuccessRate(IAsyncMethodRequestHandler requestHandler, short skillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short>(7, 12, skillTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011576 RID: 71030 RVA: 0x0067E270 File Offset: 0x0067C470
			public static void CalcCombatSkillBreakAvailableStepsDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, short skillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short>(7, 13, charId, skillTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011577 RID: 71031 RVA: 0x0067E29C File Offset: 0x0067C49C
			public static void GetEquipCombatSkillDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(7, 14, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011578 RID: 71032 RVA: 0x0067E2C8 File Offset: 0x0067C4C8
			public static void GetCharacterMenuCombatSkillListItemDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(7, 15, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011579 RID: 71033 RVA: 0x0067E2F4 File Offset: 0x0067C4F4
			public static void GetLoopingTransferNeiliProportionOfFiveElementsDataForTaiwu(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(7, 16, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601157A RID: 71034 RVA: 0x0067E320 File Offset: 0x0067C520
			public static void GetCombatSkillDisplayDataForPractice(IAsyncMethodRequestHandler requestHandler, int charId, short skillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short>(7, 17, charId, skillTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601157B RID: 71035 RVA: 0x0067E34C File Offset: 0x0067C54C
			public static void CalcTaiwuExtraDeltaNeiliAllocationLoops(IAsyncMethodRequestHandler requestHandler, int combatSkillId, int totalLoopsCount, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int>(7, 18, combatSkillId, totalLoopsCount, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601157C RID: 71036 RVA: 0x0067E378 File Offset: 0x0067C578
			public static void GetLearnedCombatSkillByType(IAsyncMethodRequestHandler requestHandler, int charId, sbyte skillType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, sbyte>(7, 19, charId, skillType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601157D RID: 71037 RVA: 0x0067E3A4 File Offset: 0x0067C5A4
			public static void GetCombatSkillDisplayDataForListOnce(IAsyncMethodRequestHandler requestHandler, int charId, short skillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short>(7, 20, charId, skillTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601157E RID: 71038 RVA: 0x0067E3D0 File Offset: 0x0067C5D0
			public static void GetCombatSkillDisplayDataForList(IAsyncMethodRequestHandler requestHandler, int charId, List<short> skillTemplateIdList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, List<short>>(7, 21, charId, skillTemplateIdList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601157F RID: 71039 RVA: 0x0067E3FC File Offset: 0x0067C5FC
			public static void GetCombatSkillEquipment(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(7, 22, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011580 RID: 71040 RVA: 0x0067E428 File Offset: 0x0067C628
			public static void GetCharacterEquipNeigongBreakList(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(7, 23, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011581 RID: 71041 RVA: 0x0067E454 File Offset: 0x0067C654
			public static void GetCharacterEquipAssistanceBreakList(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(7, 24, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}
		}
	}
}
