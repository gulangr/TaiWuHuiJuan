using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;
using GameData.GameDataBridge;

namespace GameData.Domains.Merchant
{
	// Token: 0x02000FC3 RID: 4035
	public static class MerchantDomainMethod
	{
		// Token: 0x02002601 RID: 9729
		public static class Call
		{
			// Token: 0x0601116E RID: 69998 RVA: 0x006785C3 File Offset: 0x006767C3
			public static void GetMerchantData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 14, 0, charId);
			}

			// Token: 0x0601116F RID: 69999 RVA: 0x006785D1 File Offset: 0x006767D1
			public static void SettleTrade(MerchantTradeArguments merchantTradeArguments)
			{
				GameDataBridge.AddMethodCall<MerchantTradeArguments>(-1, 14, 1, merchantTradeArguments);
			}

			// Token: 0x06011170 RID: 70000 RVA: 0x006785DF File Offset: 0x006767DF
			public static void ExchangeBook(int npcId, List<ItemDisplayData> boughtItems, List<ItemDisplayData> soldItems, int selfAuthority, int npcAuthority)
			{
				GameDataBridge.AddMethodCall<int, List<ItemDisplayData>, List<ItemDisplayData>, int, int>(-1, 14, 2, npcId, boughtItems, soldItems, selfAuthority, npcAuthority);
			}

			// Token: 0x06011171 RID: 70001 RVA: 0x006785F2 File Offset: 0x006767F2
			public static void PullTradeCaravanLocations()
			{
				GameDataBridge.AddMethodCall(-1, 14, 3);
			}

			// Token: 0x06011172 RID: 70002 RVA: 0x006785FF File Offset: 0x006767FF
			public static void GetCaravanMerchantData(int listenerId, int caravanId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 14, 4, caravanId);
			}

			// Token: 0x06011173 RID: 70003 RVA: 0x0067860D File Offset: 0x0067680D
			public static void GetTradeBookDisplayData(int listenerId, int npcId, bool isFavor)
			{
				GameDataBridge.AddMethodCall<int, bool>(listenerId, 14, 5, npcId, isFavor);
			}

			// Token: 0x06011174 RID: 70004 RVA: 0x0067861C File Offset: 0x0067681C
			public static void GmCmd_AddItem(int charId, sbyte itemType, short templateId, int count, int level)
			{
				GameDataBridge.AddMethodCall<int, sbyte, short, int, int>(-1, 14, 6, charId, itemType, templateId, count, level);
			}

			// Token: 0x06011175 RID: 70005 RVA: 0x0067862F File Offset: 0x0067682F
			public static void GetTaiwuLocationMaxLevelCaravanIdList(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 14, 7);
			}

			// Token: 0x06011176 RID: 70006 RVA: 0x0067863C File Offset: 0x0067683C
			public static void GetCurFavorability(int listenerId, sbyte merchantType)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 14, 8, merchantType);
			}

			// Token: 0x06011177 RID: 70007 RVA: 0x0067864A File Offset: 0x0067684A
			public static void FinishBookTrade(int charId, bool isFavor)
			{
				GameDataBridge.AddMethodCall<int, bool>(-1, 14, 9, charId, isFavor);
			}

			// Token: 0x06011178 RID: 70008 RVA: 0x0067865A File Offset: 0x0067685A
			public static void GetTradeBackBookDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 14, 10);
			}

			// Token: 0x06011179 RID: 70009 RVA: 0x00678668 File Offset: 0x00676868
			public static void GetMerchantInfoCaravanDataList(int listenerId, sbyte merchantType)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 14, 11, merchantType);
			}

			// Token: 0x0601117A RID: 70010 RVA: 0x00678677 File Offset: 0x00676877
			public static void GetMerchantInfoAreaDataList(int listenerId, sbyte merchantType)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 14, 12, merchantType);
			}

			// Token: 0x0601117B RID: 70011 RVA: 0x00678686 File Offset: 0x00676886
			public static void GetMerchantInfoMerchantDataList(int listenerId, sbyte merchantType)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 14, 13, merchantType);
			}

			// Token: 0x0601117C RID: 70012 RVA: 0x00678695 File Offset: 0x00676895
			public static void GetMerchantOverFavorData(int listenerId, sbyte merchantType)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 14, 14, merchantType);
			}

			// Token: 0x0601117D RID: 70013 RVA: 0x006786A4 File Offset: 0x006768A4
			public static void GetBuildingMerchantData(int listenerId, sbyte merchantType, bool isHead)
			{
				GameDataBridge.AddMethodCall<sbyte, bool>(listenerId, 14, 15, merchantType, isHead);
			}

			// Token: 0x0601117E RID: 70014 RVA: 0x006786B4 File Offset: 0x006768B4
			public static void GetCaravanDisplayData(int listenerId, int caravanId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 14, 16, caravanId);
			}

			// Token: 0x0601117F RID: 70015 RVA: 0x006786C3 File Offset: 0x006768C3
			public static void InvestCaravan(int caravanId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 14, 17, caravanId);
			}

			// Token: 0x06011180 RID: 70016 RVA: 0x006786D2 File Offset: 0x006768D2
			public static void GetAllFavorability(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 14, 18);
			}

			// Token: 0x06011181 RID: 70017 RVA: 0x006786E0 File Offset: 0x006768E0
			public static void ProtectCaravan(int caravanId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 14, 19, caravanId);
			}

			// Token: 0x06011182 RID: 70018 RVA: 0x006786EF File Offset: 0x006768EF
			public static void GmCmd_ProtectCaravan(int caravanId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 14, 20, caravanId);
			}

			// Token: 0x06011183 RID: 70019 RVA: 0x006786FE File Offset: 0x006768FE
			public static void GmCmd_ProtectAllCaravan()
			{
				GameDataBridge.AddMethodCall(-1, 14, 21);
			}

			// Token: 0x06011184 RID: 70020 RVA: 0x0067870C File Offset: 0x0067690C
			public static void GmCmd_SetCaravanRobbedRate(int caravanId, short robbedRate)
			{
				GameDataBridge.AddMethodCall<int, short>(-1, 14, 22, caravanId, robbedRate);
			}

			// Token: 0x06011185 RID: 70021 RVA: 0x0067871C File Offset: 0x0067691C
			public static void GmCmd_SetCaravanInvested(int caravanId, bool isInvested)
			{
				GameDataBridge.AddMethodCall<int, bool>(-1, 14, 23, caravanId, isInvested);
			}

			// Token: 0x06011186 RID: 70022 RVA: 0x0067872C File Offset: 0x0067692C
			public static void GmCmd_SetAllCaravanInvested(bool isInvested)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 14, 24, isInvested);
			}

			// Token: 0x06011187 RID: 70023 RVA: 0x0067873B File Offset: 0x0067693B
			public static void GmCmd_SetCaravanState(int caravanId, sbyte caravanState)
			{
				GameDataBridge.AddMethodCall<int, sbyte>(-1, 14, 25, caravanId, caravanState);
			}

			// Token: 0x06011188 RID: 70024 RVA: 0x0067874B File Offset: 0x0067694B
			public static void GmCmd_SetCaravanIncomeData(int caravanId, short incomeBonus, short incomeCriticalRate, short incomeCriticalResult)
			{
				GameDataBridge.AddMethodCall<int, short, short, short>(-1, 14, 26, caravanId, incomeBonus, incomeCriticalRate, incomeCriticalResult);
			}

			// Token: 0x06011189 RID: 70025 RVA: 0x0067875D File Offset: 0x0067695D
			public static void GetSectStorySpecialMerchantData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 14, 27);
			}

			// Token: 0x0601118A RID: 70026 RVA: 0x0067876B File Offset: 0x0067696B
			public static void GetMerchantTemplateId(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 14, 28, charId);
			}

			// Token: 0x0601118B RID: 70027 RVA: 0x0067877A File Offset: 0x0067697A
			public static void GetMerchantBuyBackData(int listenerId, OpenShopEventArguments openShopEventArguments)
			{
				GameDataBridge.AddMethodCall<OpenShopEventArguments>(listenerId, 14, 29, openShopEventArguments);
			}

			// Token: 0x0601118C RID: 70028 RVA: 0x0067878C File Offset: 0x0067698C
			public static void RefreshMerchantGoods(int listenerId, int charOrCaravanId, bool isChar, sbyte level, bool isFromBuilding, bool isHeadBuildingMerchant, sbyte buildingMerchantType)
			{
				GameDataBridge.AddMethodCall<int, bool, sbyte, bool, bool, sbyte>(listenerId, 14, 30, charOrCaravanId, isChar, level, isFromBuilding, isHeadBuildingMerchant, buildingMerchantType);
			}

			// Token: 0x0601118D RID: 70029 RVA: 0x006787AE File Offset: 0x006769AE
			public static void GetFavorabilityWithDelta(int listenerId, sbyte merchantType)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 14, 31, merchantType);
			}

			// Token: 0x0601118E RID: 70030 RVA: 0x006787BD File Offset: 0x006769BD
			public static void GetFavorabilityWithDelta(int listenerId, sbyte merchantType, int delta)
			{
				GameDataBridge.AddMethodCall<sbyte, int>(listenerId, 14, 31, merchantType, delta);
			}

			// Token: 0x0601118F RID: 70031 RVA: 0x006787CD File Offset: 0x006769CD
			public static void CanRefreshMerchantGoods(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 14, 32);
			}

			// Token: 0x06011190 RID: 70032 RVA: 0x006787DB File Offset: 0x006769DB
			public static void CanRefreshMerchantGoods(int listenerId, bool consume)
			{
				GameDataBridge.AddMethodCall<bool>(listenerId, 14, 32, consume);
			}

			// Token: 0x06011191 RID: 70033 RVA: 0x006787EA File Offset: 0x006769EA
			public static void GetMerchantInfoCaravanDataSingle(int listenerId, int targetCaravanId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 14, 33, targetCaravanId);
			}

			// Token: 0x06011192 RID: 70034 RVA: 0x006787F9 File Offset: 0x006769F9
			public static void GmCmd_SetMerchantExp(int charId, sbyte merchantType, int value)
			{
				GameDataBridge.AddMethodCall<int, sbyte, int>(-1, 14, 34, charId, merchantType, value);
			}

			// Token: 0x06011193 RID: 70035 RVA: 0x0067880A File Offset: 0x00676A0A
			public static void GmCmd_GetMerchantExp(int listenerId, int charId, sbyte merchantType)
			{
				GameDataBridge.AddMethodCall<int, sbyte>(listenerId, 14, 35, charId, merchantType);
			}

			// Token: 0x06011194 RID: 70036 RVA: 0x0067881A File Offset: 0x00676A1A
			public static void GmCmd_GetMerchantLevel(int listenerId, int charId, sbyte merchantType)
			{
				GameDataBridge.AddMethodCall<int, sbyte>(listenerId, 14, 36, charId, merchantType);
			}

			// Token: 0x06011195 RID: 70037 RVA: 0x0067882A File Offset: 0x00676A2A
			public static void GmCmd_RemoveMerchantData(int merchantId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 14, 37, merchantId);
			}

			// Token: 0x06011196 RID: 70038 RVA: 0x00678839 File Offset: 0x00676A39
			public static void GmCmd_SetMerchantCharToType(int charId, sbyte type)
			{
				GameDataBridge.AddMethodCall<int, sbyte>(-1, 14, 38, charId, type);
			}
		}

		// Token: 0x02002602 RID: 9730
		public static class AsyncCall
		{
			// Token: 0x06011197 RID: 70039 RVA: 0x0067884C File Offset: 0x00676A4C
			public static void GetMerchantData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(14, 0, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011198 RID: 70040 RVA: 0x00678877 File Offset: 0x00676A77
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MerchantDomainMethod.Call.SettleTrade instead.", true)]
			public static void SettleTrade(IAsyncMethodRequestHandler requestHandler, MerchantTradeArguments merchantTradeArguments, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011199 RID: 70041 RVA: 0x0067887F File Offset: 0x00676A7F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MerchantDomainMethod.Call.ExchangeBook instead.", true)]
			public static void ExchangeBook(IAsyncMethodRequestHandler requestHandler, int npcId, List<ItemDisplayData> boughtItems, List<ItemDisplayData> soldItems, int selfAuthority, int npcAuthority, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601119A RID: 70042 RVA: 0x00678887 File Offset: 0x00676A87
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MerchantDomainMethod.Call.PullTradeCaravanLocations instead.", true)]
			public static void PullTradeCaravanLocations(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601119B RID: 70043 RVA: 0x00678890 File Offset: 0x00676A90
			public static void GetCaravanMerchantData(IAsyncMethodRequestHandler requestHandler, int caravanId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(14, 4, caravanId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601119C RID: 70044 RVA: 0x006788BC File Offset: 0x00676ABC
			public static void GetTradeBookDisplayData(IAsyncMethodRequestHandler requestHandler, int npcId, bool isFavor, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, bool>(14, 5, npcId, isFavor, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601119D RID: 70045 RVA: 0x006788E8 File Offset: 0x00676AE8
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MerchantDomainMethod.Call.GmCmd_AddItem instead.", true)]
			public static void GmCmd_AddItem(IAsyncMethodRequestHandler requestHandler, int charId, sbyte itemType, short templateId, int count, int level, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601119E RID: 70046 RVA: 0x006788F0 File Offset: 0x00676AF0
			public static void GetTaiwuLocationMaxLevelCaravanIdList(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(14, 7, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601119F RID: 70047 RVA: 0x0067891C File Offset: 0x00676B1C
			public static void GetCurFavorability(IAsyncMethodRequestHandler requestHandler, sbyte merchantType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(14, 8, merchantType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060111A0 RID: 70048 RVA: 0x00678947 File Offset: 0x00676B47
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MerchantDomainMethod.Call.FinishBookTrade instead.", true)]
			public static void FinishBookTrade(IAsyncMethodRequestHandler requestHandler, int charId, bool isFavor, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060111A1 RID: 70049 RVA: 0x00678950 File Offset: 0x00676B50
			public static void GetTradeBackBookDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(14, 10, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060111A2 RID: 70050 RVA: 0x0067897C File Offset: 0x00676B7C
			public static void GetMerchantInfoCaravanDataList(IAsyncMethodRequestHandler requestHandler, sbyte merchantType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(14, 11, merchantType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060111A3 RID: 70051 RVA: 0x006789A8 File Offset: 0x00676BA8
			public static void GetMerchantInfoAreaDataList(IAsyncMethodRequestHandler requestHandler, sbyte merchantType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(14, 12, merchantType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060111A4 RID: 70052 RVA: 0x006789D4 File Offset: 0x00676BD4
			public static void GetMerchantInfoMerchantDataList(IAsyncMethodRequestHandler requestHandler, sbyte merchantType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(14, 13, merchantType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060111A5 RID: 70053 RVA: 0x00678A00 File Offset: 0x00676C00
			public static void GetMerchantOverFavorData(IAsyncMethodRequestHandler requestHandler, sbyte merchantType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(14, 14, merchantType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060111A6 RID: 70054 RVA: 0x00678A2C File Offset: 0x00676C2C
			public static void GetBuildingMerchantData(IAsyncMethodRequestHandler requestHandler, sbyte merchantType, bool isHead, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte, bool>(14, 15, merchantType, isHead, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060111A7 RID: 70055 RVA: 0x00678A5C File Offset: 0x00676C5C
			public static void GetCaravanDisplayData(IAsyncMethodRequestHandler requestHandler, int caravanId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(14, 16, caravanId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060111A8 RID: 70056 RVA: 0x00678A88 File Offset: 0x00676C88
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MerchantDomainMethod.Call.InvestCaravan instead.", true)]
			public static void InvestCaravan(IAsyncMethodRequestHandler requestHandler, int caravanId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060111A9 RID: 70057 RVA: 0x00678A90 File Offset: 0x00676C90
			public static void GetAllFavorability(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(14, 18, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060111AA RID: 70058 RVA: 0x00678ABB File Offset: 0x00676CBB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MerchantDomainMethod.Call.ProtectCaravan instead.", true)]
			public static void ProtectCaravan(IAsyncMethodRequestHandler requestHandler, int caravanId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060111AB RID: 70059 RVA: 0x00678AC3 File Offset: 0x00676CC3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MerchantDomainMethod.Call.GmCmd_ProtectCaravan instead.", true)]
			public static void GmCmd_ProtectCaravan(IAsyncMethodRequestHandler requestHandler, int caravanId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060111AC RID: 70060 RVA: 0x00678ACB File Offset: 0x00676CCB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MerchantDomainMethod.Call.GmCmd_ProtectAllCaravan instead.", true)]
			public static void GmCmd_ProtectAllCaravan(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060111AD RID: 70061 RVA: 0x00678AD3 File Offset: 0x00676CD3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MerchantDomainMethod.Call.GmCmd_SetCaravanRobbedRate instead.", true)]
			public static void GmCmd_SetCaravanRobbedRate(IAsyncMethodRequestHandler requestHandler, int caravanId, short robbedRate, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060111AE RID: 70062 RVA: 0x00678ADB File Offset: 0x00676CDB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MerchantDomainMethod.Call.GmCmd_SetCaravanInvested instead.", true)]
			public static void GmCmd_SetCaravanInvested(IAsyncMethodRequestHandler requestHandler, int caravanId, bool isInvested, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060111AF RID: 70063 RVA: 0x00678AE3 File Offset: 0x00676CE3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MerchantDomainMethod.Call.GmCmd_SetAllCaravanInvested instead.", true)]
			public static void GmCmd_SetAllCaravanInvested(IAsyncMethodRequestHandler requestHandler, bool isInvested, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060111B0 RID: 70064 RVA: 0x00678AEB File Offset: 0x00676CEB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MerchantDomainMethod.Call.GmCmd_SetCaravanState instead.", true)]
			public static void GmCmd_SetCaravanState(IAsyncMethodRequestHandler requestHandler, int caravanId, sbyte caravanState, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060111B1 RID: 70065 RVA: 0x00678AF3 File Offset: 0x00676CF3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MerchantDomainMethod.Call.GmCmd_SetCaravanIncomeData instead.", true)]
			public static void GmCmd_SetCaravanIncomeData(IAsyncMethodRequestHandler requestHandler, int caravanId, short incomeBonus, short incomeCriticalRate, short incomeCriticalResult, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060111B2 RID: 70066 RVA: 0x00678AFC File Offset: 0x00676CFC
			public static void GetSectStorySpecialMerchantData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(14, 27, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060111B3 RID: 70067 RVA: 0x00678B28 File Offset: 0x00676D28
			public static void GetMerchantTemplateId(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(14, 28, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060111B4 RID: 70068 RVA: 0x00678B54 File Offset: 0x00676D54
			public static void GetMerchantBuyBackData(IAsyncMethodRequestHandler requestHandler, OpenShopEventArguments openShopEventArguments, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<OpenShopEventArguments>(14, 29, openShopEventArguments, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060111B5 RID: 70069 RVA: 0x00678B80 File Offset: 0x00676D80
			public static void RefreshMerchantGoods(IAsyncMethodRequestHandler requestHandler, int charOrCaravanId, bool isChar, sbyte level, bool isFromBuilding, bool isHeadBuildingMerchant, sbyte buildingMerchantType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, bool, sbyte, bool, bool, sbyte>(14, 30, charOrCaravanId, isChar, level, isFromBuilding, isHeadBuildingMerchant, buildingMerchantType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060111B6 RID: 70070 RVA: 0x00678BB8 File Offset: 0x00676DB8
			public static void GetFavorabilityWithDelta(IAsyncMethodRequestHandler requestHandler, sbyte merchantType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(14, 31, merchantType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060111B7 RID: 70071 RVA: 0x00678BE4 File Offset: 0x00676DE4
			public static void GetFavorabilityWithDelta(IAsyncMethodRequestHandler requestHandler, sbyte merchantType, int delta, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte, int>(14, 31, merchantType, delta, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060111B8 RID: 70072 RVA: 0x00678C14 File Offset: 0x00676E14
			public static void CanRefreshMerchantGoods(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(14, 32, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060111B9 RID: 70073 RVA: 0x00678C40 File Offset: 0x00676E40
			public static void CanRefreshMerchantGoods(IAsyncMethodRequestHandler requestHandler, bool consume, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool>(14, 32, consume, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060111BA RID: 70074 RVA: 0x00678C6C File Offset: 0x00676E6C
			public static void GetMerchantInfoCaravanDataSingle(IAsyncMethodRequestHandler requestHandler, int targetCaravanId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(14, 33, targetCaravanId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060111BB RID: 70075 RVA: 0x00678C98 File Offset: 0x00676E98
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MerchantDomainMethod.Call.GmCmd_SetMerchantExp instead.", true)]
			public static void GmCmd_SetMerchantExp(IAsyncMethodRequestHandler requestHandler, int charId, sbyte merchantType, int value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060111BC RID: 70076 RVA: 0x00678CA0 File Offset: 0x00676EA0
			public static void GmCmd_GetMerchantExp(IAsyncMethodRequestHandler requestHandler, int charId, sbyte merchantType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, sbyte>(14, 35, charId, merchantType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060111BD RID: 70077 RVA: 0x00678CD0 File Offset: 0x00676ED0
			public static void GmCmd_GetMerchantLevel(IAsyncMethodRequestHandler requestHandler, int charId, sbyte merchantType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, sbyte>(14, 36, charId, merchantType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060111BE RID: 70078 RVA: 0x00678CFD File Offset: 0x00676EFD
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MerchantDomainMethod.Call.GmCmd_RemoveMerchantData instead.", true)]
			public static void GmCmd_RemoveMerchantData(IAsyncMethodRequestHandler requestHandler, int merchantId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060111BF RID: 70079 RVA: 0x00678D05 File Offset: 0x00676F05
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use MerchantDomainMethod.Call.GmCmd_SetMerchantCharToType instead.", true)]
			public static void GmCmd_SetMerchantCharToType(IAsyncMethodRequestHandler requestHandler, int charId, sbyte type, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}
		}
	}
}
