using System;
using System.Collections.Generic;
using GameData.GameDataBridge;

namespace GameData.Domains.World
{
	// Token: 0x02000FBB RID: 4027
	public static class WorldDomainMethod
	{
		// Token: 0x020025F1 RID: 9713
		public static class Call
		{
			// Token: 0x06010D1C RID: 68892 RVA: 0x0067261B File Offset: 0x0067081B
			public static void CreateWorld(WorldCreationInfo info, List<int> challengeModeIds)
			{
				GameDataBridge.AddMethodCall<WorldCreationInfo, List<int>>(-1, 1, 0, info, challengeModeIds);
			}

			// Token: 0x06010D1D RID: 68893 RVA: 0x00672629 File Offset: 0x00670829
			public static void SetWorldCreationInfo(WorldCreationInfo info, bool inherit)
			{
				GameDataBridge.AddMethodCall<WorldCreationInfo, bool>(-1, 1, 1, info, inherit);
			}

			// Token: 0x06010D1E RID: 68894 RVA: 0x00672637 File Offset: 0x00670837
			public static void GetWorldCreationInfo(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 1, 2);
			}

			// Token: 0x06010D1F RID: 68895 RVA: 0x00672643 File Offset: 0x00670843
			public static void GetJuniorXiangshuLocations(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 1, 3);
			}

			// Token: 0x06010D20 RID: 68896 RVA: 0x0067264F File Offset: 0x0067084F
			public static void HandleMonthlyEvent(int offset)
			{
				GameDataBridge.AddMethodCall<int>(-1, 1, 4, offset);
			}

			// Token: 0x06010D21 RID: 68897 RVA: 0x0067265C File Offset: 0x0067085C
			public static void GetMonthlyEventCollection(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 1, 5);
			}

			// Token: 0x06010D22 RID: 68898 RVA: 0x00672668 File Offset: 0x00670868
			public static void RemoveAllInvalidMonthlyEvents()
			{
				GameDataBridge.AddMethodCall(-1, 1, 6);
			}

			// Token: 0x06010D23 RID: 68899 RVA: 0x00672674 File Offset: 0x00670874
			public static void ProcessAllMonthlyEventsWithDefaultOption()
			{
				GameDataBridge.AddMethodCall(-1, 1, 7);
			}

			// Token: 0x06010D24 RID: 68900 RVA: 0x00672680 File Offset: 0x00670880
			public static void SpecifyWorldPopulationType(byte worldPopulationType)
			{
				GameDataBridge.AddMethodCall<byte>(-1, 1, 8, worldPopulationType);
			}

			// Token: 0x06010D25 RID: 68901 RVA: 0x0067268D File Offset: 0x0067088D
			public static void AdvanceDaysInMonth(int days)
			{
				GameDataBridge.AddMethodCall<int>(-1, 1, 9, days);
			}

			// Token: 0x06010D26 RID: 68902 RVA: 0x0067269B File Offset: 0x0067089B
			public static void AdvanceMonth()
			{
				GameDataBridge.AddMethodCall(-1, 1, 10);
			}

			// Token: 0x06010D27 RID: 68903 RVA: 0x006726A8 File Offset: 0x006708A8
			public static void AdvanceMonth_DisplayedMonthlyNotifications(bool saveWorld)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 1, 11, saveWorld);
			}

			// Token: 0x06010D28 RID: 68904 RVA: 0x006726B6 File Offset: 0x006708B6
			public static void GmCmd_AddMonthlyEvent(int listenerId, short startTemplateId, short endTemplateId, int selfCharId, int targetCharId)
			{
				GameDataBridge.AddMethodCall<short, short, int, int>(listenerId, 1, 12, startTemplateId, endTemplateId, selfCharId, targetCharId);
			}

			// Token: 0x06010D29 RID: 68905 RVA: 0x006726C8 File Offset: 0x006708C8
			public static void GmCmd_AddSectJieqingNpcExtraLegacyPoints(int charId, int delta)
			{
				GameDataBridge.AddMethodCall<int, int>(-1, 1, 13, charId, delta);
			}

			// Token: 0x06010D2A RID: 68906 RVA: 0x006726D7 File Offset: 0x006708D7
			public static void SetTopTask(int topTaskInfoId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 1, 14, topTaskInfoId);
			}

			// Token: 0x06010D2B RID: 68907 RVA: 0x006726E5 File Offset: 0x006708E5
			public static void SetTopTask(int topTaskInfoId, int targetIndex)
			{
				GameDataBridge.AddMethodCall<int, int>(-1, 1, 14, topTaskInfoId, targetIndex);
			}

			// Token: 0x06010D2C RID: 68908 RVA: 0x006726F4 File Offset: 0x006708F4
			public static void GmCmd_AddExtraTask(int taskChainId, int taskInfoId)
			{
				GameDataBridge.AddMethodCall<int, int>(-1, 1, 15, taskChainId, taskInfoId);
			}

			// Token: 0x06010D2D RID: 68909 RVA: 0x00672703 File Offset: 0x00670903
			public static void GmCmd_RemoveTriggeredExtraTask(int taskChainId, int taskInfoId)
			{
				GameDataBridge.AddMethodCall<int, int>(-1, 1, 16, taskChainId, taskInfoId);
			}

			// Token: 0x06010D2E RID: 68910 RVA: 0x00672712 File Offset: 0x00670912
			public static void GetAdvanceMonthSoftConditions(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 1, 17);
			}

			// Token: 0x06010D2F RID: 68911 RVA: 0x0067271F File Offset: 0x0067091F
			public static void GmCmd_SetWorldFunctionUnlockHint(int templateId, bool trigger)
			{
				GameDataBridge.AddMethodCall<int, bool>(-1, 1, 18, templateId, trigger);
			}

			// Token: 0x06010D30 RID: 68912 RVA: 0x0067272E File Offset: 0x0067092E
			public static void RequestWorldStateData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 1, 19);
			}

			// Token: 0x06010D31 RID: 68913 RVA: 0x0067273B File Offset: 0x0067093B
			public static void GmCmd_AddResetWorldSettingsChance()
			{
				GameDataBridge.AddMethodCall(-1, 1, 20);
			}

			// Token: 0x06010D32 RID: 68914 RVA: 0x00672748 File Offset: 0x00670948
			public static void GetMonthNotifyDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 1, 21);
			}

			// Token: 0x06010D33 RID: 68915 RVA: 0x00672755 File Offset: 0x00670955
			public static void TriggeredGuidingChapter(short templateId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 1, 22, templateId);
			}

			// Token: 0x06010D34 RID: 68916 RVA: 0x00672763 File Offset: 0x00670963
			public static void TriggeredGuidingChapter(short templateId, EGuidingChapterState state)
			{
				GameDataBridge.AddMethodCall<short, EGuidingChapterState>(-1, 1, 22, templateId, state);
			}

			// Token: 0x06010D35 RID: 68917 RVA: 0x00672772 File Offset: 0x00670972
			public static void RequestSetStat(short statId, int value)
			{
				GameDataBridge.AddMethodCall<short, int>(-1, 1, 23, statId, value);
			}

			// Token: 0x06010D36 RID: 68918 RVA: 0x00672781 File Offset: 0x00670981
			public static void ResetStatsAndAchievements()
			{
				GameDataBridge.AddMethodCall(-1, 1, 24);
			}

			// Token: 0x06010D37 RID: 68919 RVA: 0x0067278E File Offset: 0x0067098E
			public static void GetNewestMonthNotifyDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 1, 25);
			}

			// Token: 0x06010D38 RID: 68920 RVA: 0x0067279B File Offset: 0x0067099B
			public static void GmCmd_SetAllGuidingChapter(bool value)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 1, 26, value);
			}

			// Token: 0x06010D39 RID: 68921 RVA: 0x006727A9 File Offset: 0x006709A9
			public static void OnClickDamageHugeSword()
			{
				GameDataBridge.AddMethodCall(-1, 1, 27);
			}
		}

		// Token: 0x020025F2 RID: 9714
		public static class AsyncCall
		{
			// Token: 0x06010D3A RID: 68922 RVA: 0x006727B6 File Offset: 0x006709B6
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use WorldDomainMethod.Call.CreateWorld instead.", true)]
			public static void CreateWorld(IAsyncMethodRequestHandler requestHandler, WorldCreationInfo info, List<int> challengeModeIds, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010D3B RID: 68923 RVA: 0x006727BE File Offset: 0x006709BE
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use WorldDomainMethod.Call.SetWorldCreationInfo instead.", true)]
			public static void SetWorldCreationInfo(IAsyncMethodRequestHandler requestHandler, WorldCreationInfo info, bool inherit, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010D3C RID: 68924 RVA: 0x006727C8 File Offset: 0x006709C8
			public static void GetWorldCreationInfo(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(1, 2, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010D3D RID: 68925 RVA: 0x006727F4 File Offset: 0x006709F4
			public static void GetJuniorXiangshuLocations(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(1, 3, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010D3E RID: 68926 RVA: 0x0067281D File Offset: 0x00670A1D
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use WorldDomainMethod.Call.HandleMonthlyEvent instead.", true)]
			public static void HandleMonthlyEvent(IAsyncMethodRequestHandler requestHandler, int offset, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010D3F RID: 68927 RVA: 0x00672828 File Offset: 0x00670A28
			public static void GetMonthlyEventCollection(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(1, 5, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010D40 RID: 68928 RVA: 0x00672851 File Offset: 0x00670A51
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use WorldDomainMethod.Call.RemoveAllInvalidMonthlyEvents instead.", true)]
			public static void RemoveAllInvalidMonthlyEvents(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010D41 RID: 68929 RVA: 0x00672859 File Offset: 0x00670A59
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use WorldDomainMethod.Call.ProcessAllMonthlyEventsWithDefaultOption instead.", true)]
			public static void ProcessAllMonthlyEventsWithDefaultOption(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010D42 RID: 68930 RVA: 0x00672861 File Offset: 0x00670A61
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use WorldDomainMethod.Call.SpecifyWorldPopulationType instead.", true)]
			public static void SpecifyWorldPopulationType(IAsyncMethodRequestHandler requestHandler, byte worldPopulationType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010D43 RID: 68931 RVA: 0x00672869 File Offset: 0x00670A69
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use WorldDomainMethod.Call.AdvanceDaysInMonth instead.", true)]
			public static void AdvanceDaysInMonth(IAsyncMethodRequestHandler requestHandler, int days, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010D44 RID: 68932 RVA: 0x00672871 File Offset: 0x00670A71
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use WorldDomainMethod.Call.AdvanceMonth instead.", true)]
			public static void AdvanceMonth(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010D45 RID: 68933 RVA: 0x00672879 File Offset: 0x00670A79
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use WorldDomainMethod.Call.AdvanceMonth_DisplayedMonthlyNotifications instead.", true)]
			public static void AdvanceMonth_DisplayedMonthlyNotifications(IAsyncMethodRequestHandler requestHandler, bool saveWorld, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010D46 RID: 68934 RVA: 0x00672884 File Offset: 0x00670A84
			public static void GmCmd_AddMonthlyEvent(IAsyncMethodRequestHandler requestHandler, short startTemplateId, short endTemplateId, int selfCharId, int targetCharId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, short, int, int>(1, 12, startTemplateId, endTemplateId, selfCharId, targetCharId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010D47 RID: 68935 RVA: 0x006728B4 File Offset: 0x00670AB4
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use WorldDomainMethod.Call.GmCmd_AddSectJieqingNpcExtraLegacyPoints instead.", true)]
			public static void GmCmd_AddSectJieqingNpcExtraLegacyPoints(IAsyncMethodRequestHandler requestHandler, int charId, int delta, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010D48 RID: 68936 RVA: 0x006728BC File Offset: 0x00670ABC
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use WorldDomainMethod.Call.SetTopTask instead.", true)]
			public static void SetTopTask(IAsyncMethodRequestHandler requestHandler, int topTaskInfoId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010D49 RID: 68937 RVA: 0x006728C4 File Offset: 0x00670AC4
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use WorldDomainMethod.Call.SetTopTask instead.", true)]
			public static void SetTopTask(IAsyncMethodRequestHandler requestHandler, int topTaskInfoId, int targetIndex, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010D4A RID: 68938 RVA: 0x006728CC File Offset: 0x00670ACC
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use WorldDomainMethod.Call.GmCmd_AddExtraTask instead.", true)]
			public static void GmCmd_AddExtraTask(IAsyncMethodRequestHandler requestHandler, int taskChainId, int taskInfoId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010D4B RID: 68939 RVA: 0x006728D4 File Offset: 0x00670AD4
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use WorldDomainMethod.Call.GmCmd_RemoveTriggeredExtraTask instead.", true)]
			public static void GmCmd_RemoveTriggeredExtraTask(IAsyncMethodRequestHandler requestHandler, int taskChainId, int taskInfoId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010D4C RID: 68940 RVA: 0x006728DC File Offset: 0x00670ADC
			public static void GetAdvanceMonthSoftConditions(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(1, 17, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010D4D RID: 68941 RVA: 0x00672906 File Offset: 0x00670B06
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use WorldDomainMethod.Call.GmCmd_SetWorldFunctionUnlockHint instead.", true)]
			public static void GmCmd_SetWorldFunctionUnlockHint(IAsyncMethodRequestHandler requestHandler, int templateId, bool trigger, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010D4E RID: 68942 RVA: 0x00672910 File Offset: 0x00670B10
			public static void RequestWorldStateData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(1, 19, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010D4F RID: 68943 RVA: 0x0067293A File Offset: 0x00670B3A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use WorldDomainMethod.Call.GmCmd_AddResetWorldSettingsChance instead.", true)]
			public static void GmCmd_AddResetWorldSettingsChance(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010D50 RID: 68944 RVA: 0x00672944 File Offset: 0x00670B44
			public static void GetMonthNotifyDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(1, 21, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010D51 RID: 68945 RVA: 0x0067296E File Offset: 0x00670B6E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use WorldDomainMethod.Call.TriggeredGuidingChapter instead.", true)]
			public static void TriggeredGuidingChapter(IAsyncMethodRequestHandler requestHandler, short templateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010D52 RID: 68946 RVA: 0x00672976 File Offset: 0x00670B76
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use WorldDomainMethod.Call.TriggeredGuidingChapter instead.", true)]
			public static void TriggeredGuidingChapter(IAsyncMethodRequestHandler requestHandler, short templateId, EGuidingChapterState state, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010D53 RID: 68947 RVA: 0x0067297E File Offset: 0x00670B7E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use WorldDomainMethod.Call.RequestSetStat instead.", true)]
			public static void RequestSetStat(IAsyncMethodRequestHandler requestHandler, short statId, int value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010D54 RID: 68948 RVA: 0x00672986 File Offset: 0x00670B86
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use WorldDomainMethod.Call.ResetStatsAndAchievements instead.", true)]
			public static void ResetStatsAndAchievements(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010D55 RID: 68949 RVA: 0x00672990 File Offset: 0x00670B90
			public static void GetNewestMonthNotifyDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(1, 25, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06010D56 RID: 68950 RVA: 0x006729BA File Offset: 0x00670BBA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use WorldDomainMethod.Call.GmCmd_SetAllGuidingChapter instead.", true)]
			public static void GmCmd_SetAllGuidingChapter(IAsyncMethodRequestHandler requestHandler, bool value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010D57 RID: 68951 RVA: 0x006729C2 File Offset: 0x00670BC2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use WorldDomainMethod.Call.OnClickDamageHugeSword instead.", true)]
			public static void OnClickDamageHugeSword(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}
		}
	}
}
