using System;
using System.Collections.Generic;
using GameData.GameDataBridge;
using GameData.Utilities;

namespace GameData.Domains.Information
{
	// Token: 0x02000FC8 RID: 4040
	public static class InformationDomainMethod
	{
		// Token: 0x0200260B RID: 9739
		public static class Call
		{
			// Token: 0x06011300 RID: 70400 RVA: 0x0067AAFC File Offset: 0x00678CFC
			public static void GetCharacterNormalInformation(int listenerId, int characterId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 18, 0, characterId);
			}

			// Token: 0x06011301 RID: 70401 RVA: 0x0067AB0A File Offset: 0x00678D0A
			public static void AddNormalInformationToCharacter(int characterId, NormalInformation information)
			{
				GameDataBridge.AddMethodCall<int, NormalInformation>(-1, 18, 1, characterId, information);
			}

			// Token: 0x06011302 RID: 70402 RVA: 0x0067AB19 File Offset: 0x00678D19
			public static void DeleteTmpInformation()
			{
				GameDataBridge.AddMethodCall(-1, 18, 2);
			}

			// Token: 0x06011303 RID: 70403 RVA: 0x0067AB26 File Offset: 0x00678D26
			public static void GetNormalInformationUsedCount(int listenerId, int characterId, NormalInformation information)
			{
				GameDataBridge.AddMethodCall<int, NormalInformation>(listenerId, 18, 3, characterId, information);
			}

			// Token: 0x06011304 RID: 70404 RVA: 0x0067AB35 File Offset: 0x00678D35
			public static void GetSecretInformationDisplayPackage(int listenerId, List<int> secretIds)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 18, 4, secretIds);
			}

			// Token: 0x06011305 RID: 70405 RVA: 0x0067AB43 File Offset: 0x00678D43
			public static void GetSecretInformationDisplayPackageFromCharacter(int listenerId, int characterId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 18, 5, characterId);
			}

			// Token: 0x06011306 RID: 70406 RVA: 0x0067AB51 File Offset: 0x00678D51
			public static void GetSecretInformationDisplayPackageFromBroadcast(int listenerId, int characterId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 18, 6, characterId);
			}

			// Token: 0x06011307 RID: 70407 RVA: 0x0067AB5F File Offset: 0x00678D5F
			public static void GetSecretInformationDisplayPackageForSelections(int listenerId, int characterId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 18, 7, characterId);
			}

			// Token: 0x06011308 RID: 70408 RVA: 0x0067AB6D File Offset: 0x00678D6D
			public static void DiscardSecretInformation(int charId, SecretInformationId secretId)
			{
				GameDataBridge.AddMethodCall<int, SecretInformationId>(-1, 18, 8, charId, secretId);
			}

			// Token: 0x06011309 RID: 70409 RVA: 0x0067AB7C File Offset: 0x00678D7C
			public static void GmCmd_CreateSecretInformationByCharacterIds(int listenerId, string templateDefKeyName, List<int> charIds)
			{
				GameDataBridge.AddMethodCall<string, List<int>>(listenerId, 18, 9, templateDefKeyName, charIds);
			}

			// Token: 0x0601130A RID: 70410 RVA: 0x0067AB8C File Offset: 0x00678D8C
			public static void GmCmd_MakeCharacterReceiveSecretInformation(int listenerId, int characterId, SecretInformationId secretId)
			{
				GameDataBridge.AddMethodCall<int, SecretInformationId>(listenerId, 18, 10, characterId, secretId);
			}

			// Token: 0x0601130B RID: 70411 RVA: 0x0067AB9C File Offset: 0x00678D9C
			public static void DisseminateSecretInformation(int listenerId, SecretInformationId secretId, int sourceCharId, int targetCharId)
			{
				GameDataBridge.AddMethodCall<SecretInformationId, int, int>(listenerId, 18, 11, secretId, sourceCharId, targetCharId);
			}

			// Token: 0x0601130C RID: 70412 RVA: 0x0067ABAD File Offset: 0x00678DAD
			public static void GetCharacterDisplayDataWithInfoList(int listenerId, List<int> charList)
			{
				GameDataBridge.AddMethodCall<List<int>>(listenerId, 18, 12, charList);
			}

			// Token: 0x0601130D RID: 70413 RVA: 0x0067ABBC File Offset: 0x00678DBC
			public static void GmCmd_MakeSecretInformationBroadcast(SecretInformationId secretId)
			{
				GameDataBridge.AddMethodCall<SecretInformationId>(-1, 18, 13, secretId);
			}

			// Token: 0x0601130E RID: 70414 RVA: 0x0067ABCB File Offset: 0x00678DCB
			public static void GmCmd_MakeSecretInformationBroadcast(SecretInformationId secretId, int sourceCharId)
			{
				GameDataBridge.AddMethodCall<SecretInformationId, int>(-1, 18, 13, secretId, sourceCharId);
			}

			// Token: 0x0601130F RID: 70415 RVA: 0x0067ABDB File Offset: 0x00678DDB
			public static void PerformProfessionLiteratiSkill3(NormalInformation normalInformation)
			{
				GameDataBridge.AddMethodCall<NormalInformation>(-1, 18, 14, normalInformation);
			}

			// Token: 0x06011310 RID: 70416 RVA: 0x0067ABEA File Offset: 0x00678DEA
			public static void PerformProfessionLiteratiSkill2(int secretInformationId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 18, 15, secretInformationId);
			}

			// Token: 0x06011311 RID: 70417 RVA: 0x0067ABF9 File Offset: 0x00678DF9
			public static void GetNormalInformationUsedCountAndMax(int listenerId, int characterId, NormalInformation information)
			{
				GameDataBridge.AddMethodCall<int, NormalInformation>(listenerId, 18, 16, characterId, information);
			}

			// Token: 0x06011312 RID: 70418 RVA: 0x0067AC09 File Offset: 0x00678E09
			public static void SettleSecretInformationShopTrade(List<IntPair> secretList, int shopCharId)
			{
				GameDataBridge.AddMethodCall<List<IntPair>, int>(-1, 18, 17, secretList, shopCharId);
			}

			// Token: 0x06011313 RID: 70419 RVA: 0x0067AC19 File Offset: 0x00678E19
			public static void GmCmd_DisseminationSecretInformationToRandomCharacters(int listenerId, SecretInformationId secretId, int sourceCharId, int amount)
			{
				GameDataBridge.AddMethodCall<SecretInformationId, int, int>(listenerId, 18, 18, secretId, sourceCharId, amount);
			}

			// Token: 0x06011314 RID: 70420 RVA: 0x0067AC2A File Offset: 0x00678E2A
			public static void GetCharacterNormalInformationDisplayData(int listenerId, int characterId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 18, 19, characterId);
			}

			// Token: 0x06011315 RID: 70421 RVA: 0x0067AC39 File Offset: 0x00678E39
			public static void SetSecretInformationLevelFactor(int[] value)
			{
				GameDataBridge.AddMethodCall<int[]>(-1, 18, 20, value);
			}

			// Token: 0x06011316 RID: 70422 RVA: 0x0067AC48 File Offset: 0x00678E48
			public static void GetSecretInformationLevelFactor(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 18, 21);
			}

			// Token: 0x06011317 RID: 70423 RVA: 0x0067AC56 File Offset: 0x00678E56
			public static void GetSecretInformationDetailedData(int listenerId, SecretInformationId secretId, int sourceCharId)
			{
				GameDataBridge.AddMethodCall<SecretInformationId, int>(listenerId, 18, 22, secretId, sourceCharId);
			}

			// Token: 0x06011318 RID: 70424 RVA: 0x0067AC66 File Offset: 0x00678E66
			public static void GetSecretInformationAmountFromCharacter(int listenerId, int characterId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 18, 23, characterId);
			}
		}

		// Token: 0x0200260C RID: 9740
		public static class AsyncCall
		{
			// Token: 0x06011319 RID: 70425 RVA: 0x0067AC78 File Offset: 0x00678E78
			public static void GetCharacterNormalInformation(IAsyncMethodRequestHandler requestHandler, int characterId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(18, 0, characterId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601131A RID: 70426 RVA: 0x0067ACA3 File Offset: 0x00678EA3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use InformationDomainMethod.Call.AddNormalInformationToCharacter instead.", true)]
			public static void AddNormalInformationToCharacter(IAsyncMethodRequestHandler requestHandler, int characterId, NormalInformation information, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601131B RID: 70427 RVA: 0x0067ACAB File Offset: 0x00678EAB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use InformationDomainMethod.Call.DeleteTmpInformation instead.", true)]
			public static void DeleteTmpInformation(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601131C RID: 70428 RVA: 0x0067ACB4 File Offset: 0x00678EB4
			public static void GetNormalInformationUsedCount(IAsyncMethodRequestHandler requestHandler, int characterId, NormalInformation information, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, NormalInformation>(18, 3, characterId, information, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601131D RID: 70429 RVA: 0x0067ACE0 File Offset: 0x00678EE0
			public static void GetSecretInformationDisplayPackage(IAsyncMethodRequestHandler requestHandler, List<int> secretIds, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(18, 4, secretIds, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601131E RID: 70430 RVA: 0x0067AD0C File Offset: 0x00678F0C
			public static void GetSecretInformationDisplayPackageFromCharacter(IAsyncMethodRequestHandler requestHandler, int characterId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(18, 5, characterId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601131F RID: 70431 RVA: 0x0067AD38 File Offset: 0x00678F38
			public static void GetSecretInformationDisplayPackageFromBroadcast(IAsyncMethodRequestHandler requestHandler, int characterId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(18, 6, characterId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011320 RID: 70432 RVA: 0x0067AD64 File Offset: 0x00678F64
			public static void GetSecretInformationDisplayPackageForSelections(IAsyncMethodRequestHandler requestHandler, int characterId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(18, 7, characterId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011321 RID: 70433 RVA: 0x0067AD8F File Offset: 0x00678F8F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use InformationDomainMethod.Call.DiscardSecretInformation instead.", true)]
			public static void DiscardSecretInformation(IAsyncMethodRequestHandler requestHandler, int charId, SecretInformationId secretId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011322 RID: 70434 RVA: 0x0067AD98 File Offset: 0x00678F98
			public static void GmCmd_CreateSecretInformationByCharacterIds(IAsyncMethodRequestHandler requestHandler, string templateDefKeyName, List<int> charIds, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<string, List<int>>(18, 9, templateDefKeyName, charIds, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011323 RID: 70435 RVA: 0x0067ADC8 File Offset: 0x00678FC8
			public static void GmCmd_MakeCharacterReceiveSecretInformation(IAsyncMethodRequestHandler requestHandler, int characterId, SecretInformationId secretId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, SecretInformationId>(18, 10, characterId, secretId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011324 RID: 70436 RVA: 0x0067ADF8 File Offset: 0x00678FF8
			public static void DisseminateSecretInformation(IAsyncMethodRequestHandler requestHandler, SecretInformationId secretId, int sourceCharId, int targetCharId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<SecretInformationId, int, int>(18, 11, secretId, sourceCharId, targetCharId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011325 RID: 70437 RVA: 0x0067AE28 File Offset: 0x00679028
			public static void GetCharacterDisplayDataWithInfoList(IAsyncMethodRequestHandler requestHandler, List<int> charList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>>(18, 12, charList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011326 RID: 70438 RVA: 0x0067AE54 File Offset: 0x00679054
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use InformationDomainMethod.Call.GmCmd_MakeSecretInformationBroadcast instead.", true)]
			public static void GmCmd_MakeSecretInformationBroadcast(IAsyncMethodRequestHandler requestHandler, SecretInformationId secretId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011327 RID: 70439 RVA: 0x0067AE5C File Offset: 0x0067905C
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use InformationDomainMethod.Call.GmCmd_MakeSecretInformationBroadcast instead.", true)]
			public static void GmCmd_MakeSecretInformationBroadcast(IAsyncMethodRequestHandler requestHandler, SecretInformationId secretId, int sourceCharId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011328 RID: 70440 RVA: 0x0067AE64 File Offset: 0x00679064
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use InformationDomainMethod.Call.PerformProfessionLiteratiSkill3 instead.", true)]
			public static void PerformProfessionLiteratiSkill3(IAsyncMethodRequestHandler requestHandler, NormalInformation normalInformation, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011329 RID: 70441 RVA: 0x0067AE6C File Offset: 0x0067906C
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use InformationDomainMethod.Call.PerformProfessionLiteratiSkill2 instead.", true)]
			public static void PerformProfessionLiteratiSkill2(IAsyncMethodRequestHandler requestHandler, int secretInformationId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601132A RID: 70442 RVA: 0x0067AE74 File Offset: 0x00679074
			public static void GetNormalInformationUsedCountAndMax(IAsyncMethodRequestHandler requestHandler, int characterId, NormalInformation information, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, NormalInformation>(18, 16, characterId, information, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601132B RID: 70443 RVA: 0x0067AEA1 File Offset: 0x006790A1
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use InformationDomainMethod.Call.SettleSecretInformationShopTrade instead.", true)]
			public static void SettleSecretInformationShopTrade(IAsyncMethodRequestHandler requestHandler, List<IntPair> secretList, int shopCharId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601132C RID: 70444 RVA: 0x0067AEAC File Offset: 0x006790AC
			public static void GmCmd_DisseminationSecretInformationToRandomCharacters(IAsyncMethodRequestHandler requestHandler, SecretInformationId secretId, int sourceCharId, int amount, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<SecretInformationId, int, int>(18, 18, secretId, sourceCharId, amount, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601132D RID: 70445 RVA: 0x0067AEDC File Offset: 0x006790DC
			public static void GetCharacterNormalInformationDisplayData(IAsyncMethodRequestHandler requestHandler, int characterId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(18, 19, characterId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601132E RID: 70446 RVA: 0x0067AF08 File Offset: 0x00679108
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use InformationDomainMethod.Call.SetSecretInformationLevelFactor instead.", true)]
			public static void SetSecretInformationLevelFactor(IAsyncMethodRequestHandler requestHandler, int[] value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601132F RID: 70447 RVA: 0x0067AF10 File Offset: 0x00679110
			public static void GetSecretInformationLevelFactor(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(18, 21, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011330 RID: 70448 RVA: 0x0067AF3C File Offset: 0x0067913C
			public static void GetSecretInformationDetailedData(IAsyncMethodRequestHandler requestHandler, SecretInformationId secretId, int sourceCharId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<SecretInformationId, int>(18, 22, secretId, sourceCharId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011331 RID: 70449 RVA: 0x0067AF6C File Offset: 0x0067916C
			public static void GetSecretInformationAmountFromCharacter(IAsyncMethodRequestHandler requestHandler, int characterId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(18, 23, characterId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}
		}
	}
}
