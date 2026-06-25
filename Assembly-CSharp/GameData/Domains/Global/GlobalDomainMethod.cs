using System;
using GameData.Domains.Global.Inscription;
using GameData.GameDataBridge;

namespace GameData.Domains.Global
{
	// Token: 0x02000FC9 RID: 4041
	public static class GlobalDomainMethod
	{
		// Token: 0x0200260D RID: 9741
		public static class Call
		{
			// Token: 0x06011332 RID: 70450 RVA: 0x0067AF98 File Offset: 0x00679198
			public static void EnterNewWorld(sbyte archiveId)
			{
				GameDataBridge.AddMethodCall<sbyte>(-1, 0, 0, archiveId);
			}

			// Token: 0x06011333 RID: 70451 RVA: 0x0067AFA5 File Offset: 0x006791A5
			public static void LoadWorld(sbyte archiveId, long backupTimestamp)
			{
				GameDataBridge.AddMethodCall<sbyte, long>(-1, 0, 1, archiveId, backupTimestamp);
			}

			// Token: 0x06011334 RID: 70452 RVA: 0x0067AFB3 File Offset: 0x006791B3
			public static void LoadEnding(sbyte archiveId)
			{
				GameDataBridge.AddMethodCall<sbyte>(-1, 0, 2, archiveId);
			}

			// Token: 0x06011335 RID: 70453 RVA: 0x0067AFC0 File Offset: 0x006791C0
			public static void SaveWorld()
			{
				GameDataBridge.AddMethodCall(-1, 0, 3);
			}

			// Token: 0x06011336 RID: 70454 RVA: 0x0067AFCC File Offset: 0x006791CC
			public static void LeaveWorld()
			{
				GameDataBridge.AddMethodCall(-1, 0, 4);
			}

			// Token: 0x06011337 RID: 70455 RVA: 0x0067AFD8 File Offset: 0x006791D8
			public static void GetArchivesInfo(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 0, 5);
			}

			// Token: 0x06011338 RID: 70456 RVA: 0x0067AFE4 File Offset: 0x006791E4
			public static void DeleteArchive(sbyte archiveId)
			{
				GameDataBridge.AddMethodCall<sbyte>(-1, 0, 6, archiveId);
			}

			// Token: 0x06011339 RID: 70457 RVA: 0x0067AFF1 File Offset: 0x006791F1
			public static void InscribeCharacter(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 0, 7, charId);
			}

			// Token: 0x0601133A RID: 70458 RVA: 0x0067AFFE File Offset: 0x006791FE
			public static void RemoveInscribedCharacter(InscribedCharacterKey key)
			{
				GameDataBridge.AddMethodCall<InscribedCharacterKey>(-1, 0, 8, key);
			}

			// Token: 0x0601133B RID: 70459 RVA: 0x0067B00B File Offset: 0x0067920B
			public static void SetGameBuildInfo(string gameVersion, string gameBuildDate)
			{
				GameDataBridge.AddMethodCall<string, string>(-1, 0, 9, gameVersion, gameBuildDate);
			}

			// Token: 0x0601133C RID: 70460 RVA: 0x0067B01A File Offset: 0x0067921A
			public static void PackAllCrossArchiveGameData()
			{
				GameDataBridge.AddMethodCall(-1, 0, 10);
			}

			// Token: 0x0601133D RID: 70461 RVA: 0x0067B027 File Offset: 0x00679227
			public static void SetGlobalFlag(sbyte flagType, bool value)
			{
				GameDataBridge.AddMethodCall<sbyte, bool>(-1, 0, 11, flagType, value);
			}

			// Token: 0x0601133E RID: 70462 RVA: 0x0067B036 File Offset: 0x00679236
			public static void GetGlobalFlag(int listenerId, sbyte flagType)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 0, 12, flagType);
			}

			// Token: 0x0601133F RID: 70463 RVA: 0x0067B044 File Offset: 0x00679244
			public static void CheckDriveSpace(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 0, 13);
			}

			// Token: 0x06011340 RID: 70464 RVA: 0x0067B051 File Offset: 0x00679251
			public static void UpdateSharedGlobalSettings(int listenerId, SharedGlobalSettings settings)
			{
				GameDataBridge.AddMethodCall<SharedGlobalSettings>(listenerId, 0, 14, settings);
			}

			// Token: 0x06011341 RID: 70465 RVA: 0x0067B05F File Offset: 0x0067925F
			public static void ReloadAllConfigData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 0, 15);
			}

			// Token: 0x06011342 RID: 70466 RVA: 0x0067B06C File Offset: 0x0067926C
			public static void SetCompressionType(byte compressionType)
			{
				GameDataBridge.AddMethodCall<byte>(-1, 0, 16, compressionType);
			}

			// Token: 0x06011343 RID: 70467 RVA: 0x0067B07A File Offset: 0x0067927A
			public static void EnterInGameGuideWorld(short templateId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 0, 17, templateId);
			}

			// Token: 0x06011344 RID: 70468 RVA: 0x0067B088 File Offset: 0x00679288
			public static void ExitInGameGuideWorld()
			{
				GameDataBridge.AddMethodCall(-1, 0, 18);
			}

			// Token: 0x06011345 RID: 70469 RVA: 0x0067B095 File Offset: 0x00679295
			public static void EnterTutorialWorld(short templateId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 0, 19, templateId);
			}

			// Token: 0x06011346 RID: 70470 RVA: 0x0067B0A3 File Offset: 0x006792A3
			public static void SetInscribedCharacterPinOrder(InscribedCharacterKey key, int order)
			{
				GameDataBridge.AddMethodCall<InscribedCharacterKey, int>(-1, 0, 20, key, order);
			}

			// Token: 0x06011347 RID: 70471 RVA: 0x0067B0B2 File Offset: 0x006792B2
			public static void RemoveInscribedCharacterPinOrder(InscribedCharacterKey key)
			{
				GameDataBridge.AddMethodCall<InscribedCharacterKey>(-1, 0, 21, key);
			}

			// Token: 0x06011348 RID: 70472 RVA: 0x0067B0C0 File Offset: 0x006792C0
			public static void SetCustomProtagonistPreset(CustomProtagonistPreset customProtagonistPreset)
			{
				GameDataBridge.AddMethodCall<CustomProtagonistPreset>(-1, 0, 22, customProtagonistPreset);
			}

			// Token: 0x06011349 RID: 70473 RVA: 0x0067B0CE File Offset: 0x006792CE
			public static void GetAchievementDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 0, 23);
			}

			// Token: 0x0601134A RID: 70474 RVA: 0x0067B0DB File Offset: 0x006792DB
			public static void SetLastTimeOpenAchievements()
			{
				GameDataBridge.AddMethodCall(-1, 0, 24);
			}

			// Token: 0x0601134B RID: 70475 RVA: 0x0067B0E8 File Offset: 0x006792E8
			public static void InvokeGuidingTrigger(short templateId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 0, 25, templateId);
			}
		}

		// Token: 0x0200260E RID: 9742
		public static class AsyncCall
		{
			// Token: 0x0601134C RID: 70476 RVA: 0x0067B0F6 File Offset: 0x006792F6
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use GlobalDomainMethod.Call.EnterNewWorld instead.", true)]
			public static void EnterNewWorld(IAsyncMethodRequestHandler requestHandler, sbyte archiveId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601134D RID: 70477 RVA: 0x0067B0FE File Offset: 0x006792FE
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use GlobalDomainMethod.Call.LoadWorld instead.", true)]
			public static void LoadWorld(IAsyncMethodRequestHandler requestHandler, sbyte archiveId, long backupTimestamp, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601134E RID: 70478 RVA: 0x0067B106 File Offset: 0x00679306
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use GlobalDomainMethod.Call.LoadEnding instead.", true)]
			public static void LoadEnding(IAsyncMethodRequestHandler requestHandler, sbyte archiveId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601134F RID: 70479 RVA: 0x0067B10E File Offset: 0x0067930E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use GlobalDomainMethod.Call.SaveWorld instead.", true)]
			public static void SaveWorld(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011350 RID: 70480 RVA: 0x0067B116 File Offset: 0x00679316
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use GlobalDomainMethod.Call.LeaveWorld instead.", true)]
			public static void LeaveWorld(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011351 RID: 70481 RVA: 0x0067B120 File Offset: 0x00679320
			public static void GetArchivesInfo(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(0, 5, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011352 RID: 70482 RVA: 0x0067B149 File Offset: 0x00679349
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use GlobalDomainMethod.Call.DeleteArchive instead.", true)]
			public static void DeleteArchive(IAsyncMethodRequestHandler requestHandler, sbyte archiveId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011353 RID: 70483 RVA: 0x0067B151 File Offset: 0x00679351
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use GlobalDomainMethod.Call.InscribeCharacter instead.", true)]
			public static void InscribeCharacter(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011354 RID: 70484 RVA: 0x0067B159 File Offset: 0x00679359
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use GlobalDomainMethod.Call.RemoveInscribedCharacter instead.", true)]
			public static void RemoveInscribedCharacter(IAsyncMethodRequestHandler requestHandler, InscribedCharacterKey key, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011355 RID: 70485 RVA: 0x0067B161 File Offset: 0x00679361
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use GlobalDomainMethod.Call.SetGameBuildInfo instead.", true)]
			public static void SetGameBuildInfo(IAsyncMethodRequestHandler requestHandler, string gameVersion, string gameBuildDate, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011356 RID: 70486 RVA: 0x0067B169 File Offset: 0x00679369
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use GlobalDomainMethod.Call.PackAllCrossArchiveGameData instead.", true)]
			public static void PackAllCrossArchiveGameData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011357 RID: 70487 RVA: 0x0067B171 File Offset: 0x00679371
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use GlobalDomainMethod.Call.SetGlobalFlag instead.", true)]
			public static void SetGlobalFlag(IAsyncMethodRequestHandler requestHandler, sbyte flagType, bool value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011358 RID: 70488 RVA: 0x0067B17C File Offset: 0x0067937C
			public static void GetGlobalFlag(IAsyncMethodRequestHandler requestHandler, sbyte flagType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(0, 12, flagType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011359 RID: 70489 RVA: 0x0067B1A8 File Offset: 0x006793A8
			public static void CheckDriveSpace(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(0, 13, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601135A RID: 70490 RVA: 0x0067B1D4 File Offset: 0x006793D4
			public static void UpdateSharedGlobalSettings(IAsyncMethodRequestHandler requestHandler, SharedGlobalSettings settings, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<SharedGlobalSettings>(0, 14, settings, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601135B RID: 70491 RVA: 0x0067B200 File Offset: 0x00679400
			public static void ReloadAllConfigData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(0, 15, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601135C RID: 70492 RVA: 0x0067B22A File Offset: 0x0067942A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use GlobalDomainMethod.Call.SetCompressionType instead.", true)]
			public static void SetCompressionType(IAsyncMethodRequestHandler requestHandler, byte compressionType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601135D RID: 70493 RVA: 0x0067B232 File Offset: 0x00679432
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use GlobalDomainMethod.Call.EnterInGameGuideWorld instead.", true)]
			public static void EnterInGameGuideWorld(IAsyncMethodRequestHandler requestHandler, short templateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601135E RID: 70494 RVA: 0x0067B23A File Offset: 0x0067943A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use GlobalDomainMethod.Call.ExitInGameGuideWorld instead.", true)]
			public static void ExitInGameGuideWorld(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601135F RID: 70495 RVA: 0x0067B242 File Offset: 0x00679442
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use GlobalDomainMethod.Call.EnterTutorialWorld instead.", true)]
			public static void EnterTutorialWorld(IAsyncMethodRequestHandler requestHandler, short templateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011360 RID: 70496 RVA: 0x0067B24A File Offset: 0x0067944A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use GlobalDomainMethod.Call.SetInscribedCharacterPinOrder instead.", true)]
			public static void SetInscribedCharacterPinOrder(IAsyncMethodRequestHandler requestHandler, InscribedCharacterKey key, int order, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011361 RID: 70497 RVA: 0x0067B252 File Offset: 0x00679452
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use GlobalDomainMethod.Call.RemoveInscribedCharacterPinOrder instead.", true)]
			public static void RemoveInscribedCharacterPinOrder(IAsyncMethodRequestHandler requestHandler, InscribedCharacterKey key, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011362 RID: 70498 RVA: 0x0067B25A File Offset: 0x0067945A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use GlobalDomainMethod.Call.SetCustomProtagonistPreset instead.", true)]
			public static void SetCustomProtagonistPreset(IAsyncMethodRequestHandler requestHandler, CustomProtagonistPreset customProtagonistPreset, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011363 RID: 70499 RVA: 0x0067B264 File Offset: 0x00679464
			public static void GetAchievementDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(0, 23, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011364 RID: 70500 RVA: 0x0067B28E File Offset: 0x0067948E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use GlobalDomainMethod.Call.SetLastTimeOpenAchievements instead.", true)]
			public static void SetLastTimeOpenAchievements(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011365 RID: 70501 RVA: 0x0067B296 File Offset: 0x00679496
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use GlobalDomainMethod.Call.InvokeGuidingTrigger instead.", true)]
			public static void InvokeGuidingTrigger(IAsyncMethodRequestHandler requestHandler, short templateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}
		}
	}
}
