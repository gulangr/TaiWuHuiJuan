using System;
using System.Collections.Generic;
using GameData.Domains.Item;
using GameData.GameDataBridge;

namespace GameData.Domains.Adventure
{
	// Token: 0x02000FD0 RID: 4048
	public static class AdventureDomainMethod
	{
		// Token: 0x0200261B RID: 9755
		public static class Call
		{
			// Token: 0x06011AA9 RID: 72361 RVA: 0x00685D9A File Offset: 0x00683F9A
			public static void TryInvokeConfirmEnterEvent(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 10, 0);
			}

			// Token: 0x06011AAA RID: 72362 RVA: 0x00685DA7 File Offset: 0x00683FA7
			public static void GmCmd_GenerateAdventureRemake(int adventureId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 10, 1, adventureId);
			}

			// Token: 0x06011AAB RID: 72363 RVA: 0x00685DB5 File Offset: 0x00683FB5
			public static void ExitAdventureRemake(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 10, 2);
			}

			// Token: 0x06011AAC RID: 72364 RVA: 0x00685DC2 File Offset: 0x00683FC2
			public static void EnterAdventureRemake(int listenerId, int adventureId, List<ItemKey> costItems)
			{
				GameDataBridge.AddMethodCall<int, List<ItemKey>>(listenerId, 10, 3, adventureId, costItems);
			}

			// Token: 0x06011AAD RID: 72365 RVA: 0x00685DD1 File Offset: 0x00683FD1
			public static void MoveInAdventure(int listenerId, sbyte directionSbyte)
			{
				GameDataBridge.AddMethodCall<sbyte>(listenerId, 10, 4, directionSbyte);
			}

			// Token: 0x06011AAE RID: 72366 RVA: 0x00685DDF File Offset: 0x00683FDF
			public static void GmCmd_ChangeAdventureRemakeParameter(int adventureId, string key, int delta)
			{
				GameDataBridge.AddMethodCall<int, string, int>(-1, 10, 5, adventureId, key, delta);
			}

			// Token: 0x06011AAF RID: 72367 RVA: 0x00685DEF File Offset: 0x00683FEF
			public static void InteractElement(int listenerId, int elementId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 10, 6, elementId);
			}

			// Token: 0x06011AB0 RID: 72368 RVA: 0x00685DFD File Offset: 0x00683FFD
			public static void UseTemporaryItem(int listenerId, ItemKey itemKey)
			{
				GameDataBridge.AddMethodCall<ItemKey>(listenerId, 10, 7, itemKey);
			}

			// Token: 0x06011AB1 RID: 72369 RVA: 0x00685E0B File Offset: 0x0068400B
			public static void GmCmd_AdventureAddTemporaryItem(int count, sbyte itemType, short templateId)
			{
				GameDataBridge.AddMethodCall<int, sbyte, short>(-1, 10, 8, count, itemType, templateId);
			}

			// Token: 0x06011AB2 RID: 72370 RVA: 0x00685E1B File Offset: 0x0068401B
			public static void GmCmd_ReloadAdventureCore()
			{
				GameDataBridge.AddMethodCall(-1, 10, 9);
			}

			// Token: 0x06011AB3 RID: 72371 RVA: 0x00685E29 File Offset: 0x00684029
			public static void AdventureCanEnter(int listenerId, int adventureId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 10, 10, adventureId);
			}

			// Token: 0x06011AB4 RID: 72372 RVA: 0x00685E38 File Offset: 0x00684038
			public static void PostEnterAdventureRemake(int listenerId, int adventureId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 10, 11, adventureId);
			}

			// Token: 0x06011AB5 RID: 72373 RVA: 0x00685E47 File Offset: 0x00684047
			public static void EnterMajorEvent(int listenerId, int id, List<ItemKey> costItems)
			{
				GameDataBridge.AddMethodCall<int, List<ItemKey>>(listenerId, 10, 12, id, costItems);
			}

			// Token: 0x06011AB6 RID: 72374 RVA: 0x00685E57 File Offset: 0x00684057
			public static void SelectMajorEvent(int listenerId, int selectedNode)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 10, 13, selectedNode);
			}

			// Token: 0x06011AB7 RID: 72375 RVA: 0x00685E66 File Offset: 0x00684066
			public static void PostEnterMajorEvent(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 10, 14);
			}

			// Token: 0x06011AB8 RID: 72376 RVA: 0x00685E74 File Offset: 0x00684074
			public static void GmCmd_GenerateAdventureMajorEvent(int coreId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 10, 15, coreId);
			}

			// Token: 0x06011AB9 RID: 72377 RVA: 0x00685E83 File Offset: 0x00684083
			public static void AddProgress(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 10, 16);
			}

			// Token: 0x06011ABA RID: 72378 RVA: 0x00685E91 File Offset: 0x00684091
			public static void AddProgress(int listenerId, int addValue)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 10, 16, addValue);
			}

			// Token: 0x06011ABB RID: 72379 RVA: 0x00685EA0 File Offset: 0x006840A0
			public static void FinishAdventure(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 10, 17);
			}

			// Token: 0x06011ABC RID: 72380 RVA: 0x00685EAE File Offset: 0x006840AE
			public static void GmCmd_ChangeAdventureRemakeElementParameter(int adventureId, int elementId, string key, int delta)
			{
				GameDataBridge.AddMethodCall<int, int, string, int>(-1, 10, 18, adventureId, elementId, key, delta);
			}

			// Token: 0x06011ABD RID: 72381 RVA: 0x00685EC0 File Offset: 0x006840C0
			public static void SelectAndMarkCustomTextInvoked(int listenerId, int adventureId, List<string> keys)
			{
				GameDataBridge.AddMethodCall<int, List<string>>(listenerId, 10, 19, adventureId, keys);
			}

			// Token: 0x06011ABE RID: 72382 RVA: 0x00685ED0 File Offset: 0x006840D0
			public static void GmCmd_RemoveAllAdventures()
			{
				GameDataBridge.AddMethodCall(-1, 10, 20);
			}

			// Token: 0x06011ABF RID: 72383 RVA: 0x00685EDE File Offset: 0x006840DE
			public static void ConsumeActionPointInAdventure(int listenerId, int value)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 10, 21, value);
			}

			// Token: 0x06011AC0 RID: 72384 RVA: 0x00685EED File Offset: 0x006840ED
			public static void GmCmd_ReGenerateAdventure(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 10, 22);
			}
		}

		// Token: 0x0200261C RID: 9756
		public static class AsyncCall
		{
			// Token: 0x06011AC1 RID: 72385 RVA: 0x00685EFC File Offset: 0x006840FC
			public static void TryInvokeConfirmEnterEvent(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(10, 0, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011AC2 RID: 72386 RVA: 0x00685F26 File Offset: 0x00684126
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use AdventureDomainMethod.Call.GmCmd_GenerateAdventureRemake instead.", true)]
			public static void GmCmd_GenerateAdventureRemake(IAsyncMethodRequestHandler requestHandler, int adventureId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011AC3 RID: 72387 RVA: 0x00685F30 File Offset: 0x00684130
			public static void ExitAdventureRemake(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(10, 2, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011AC4 RID: 72388 RVA: 0x00685F5C File Offset: 0x0068415C
			public static void EnterAdventureRemake(IAsyncMethodRequestHandler requestHandler, int adventureId, List<ItemKey> costItems, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, List<ItemKey>>(10, 3, adventureId, costItems, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011AC5 RID: 72389 RVA: 0x00685F88 File Offset: 0x00684188
			public static void MoveInAdventure(IAsyncMethodRequestHandler requestHandler, sbyte directionSbyte, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<sbyte>(10, 4, directionSbyte, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011AC6 RID: 72390 RVA: 0x00685FB3 File Offset: 0x006841B3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use AdventureDomainMethod.Call.GmCmd_ChangeAdventureRemakeParameter instead.", true)]
			public static void GmCmd_ChangeAdventureRemakeParameter(IAsyncMethodRequestHandler requestHandler, int adventureId, string key, int delta, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011AC7 RID: 72391 RVA: 0x00685FBC File Offset: 0x006841BC
			public static void InteractElement(IAsyncMethodRequestHandler requestHandler, int elementId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(10, 6, elementId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011AC8 RID: 72392 RVA: 0x00685FE8 File Offset: 0x006841E8
			public static void UseTemporaryItem(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey>(10, 7, itemKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011AC9 RID: 72393 RVA: 0x00686013 File Offset: 0x00684213
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use AdventureDomainMethod.Call.GmCmd_AdventureAddTemporaryItem instead.", true)]
			public static void GmCmd_AdventureAddTemporaryItem(IAsyncMethodRequestHandler requestHandler, int count, sbyte itemType, short templateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011ACA RID: 72394 RVA: 0x0068601B File Offset: 0x0068421B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use AdventureDomainMethod.Call.GmCmd_ReloadAdventureCore instead.", true)]
			public static void GmCmd_ReloadAdventureCore(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011ACB RID: 72395 RVA: 0x00686024 File Offset: 0x00684224
			public static void AdventureCanEnter(IAsyncMethodRequestHandler requestHandler, int adventureId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(10, 10, adventureId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011ACC RID: 72396 RVA: 0x00686050 File Offset: 0x00684250
			public static void PostEnterAdventureRemake(IAsyncMethodRequestHandler requestHandler, int adventureId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(10, 11, adventureId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011ACD RID: 72397 RVA: 0x0068607C File Offset: 0x0068427C
			public static void EnterMajorEvent(IAsyncMethodRequestHandler requestHandler, int id, List<ItemKey> costItems, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, List<ItemKey>>(10, 12, id, costItems, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011ACE RID: 72398 RVA: 0x006860AC File Offset: 0x006842AC
			public static void SelectMajorEvent(IAsyncMethodRequestHandler requestHandler, int selectedNode, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(10, 13, selectedNode, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011ACF RID: 72399 RVA: 0x006860D8 File Offset: 0x006842D8
			public static void PostEnterMajorEvent(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(10, 14, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011AD0 RID: 72400 RVA: 0x00686103 File Offset: 0x00684303
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use AdventureDomainMethod.Call.GmCmd_GenerateAdventureMajorEvent instead.", true)]
			public static void GmCmd_GenerateAdventureMajorEvent(IAsyncMethodRequestHandler requestHandler, int coreId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011AD1 RID: 72401 RVA: 0x0068610C File Offset: 0x0068430C
			public static void AddProgress(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(10, 16, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011AD2 RID: 72402 RVA: 0x00686138 File Offset: 0x00684338
			public static void AddProgress(IAsyncMethodRequestHandler requestHandler, int addValue, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(10, 16, addValue, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011AD3 RID: 72403 RVA: 0x00686164 File Offset: 0x00684364
			public static void FinishAdventure(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(10, 17, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011AD4 RID: 72404 RVA: 0x0068618F File Offset: 0x0068438F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use AdventureDomainMethod.Call.GmCmd_ChangeAdventureRemakeElementParameter instead.", true)]
			public static void GmCmd_ChangeAdventureRemakeElementParameter(IAsyncMethodRequestHandler requestHandler, int adventureId, int elementId, string key, int delta, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011AD5 RID: 72405 RVA: 0x00686198 File Offset: 0x00684398
			public static void SelectAndMarkCustomTextInvoked(IAsyncMethodRequestHandler requestHandler, int adventureId, List<string> keys, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, List<string>>(10, 19, adventureId, keys, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011AD6 RID: 72406 RVA: 0x006861C5 File Offset: 0x006843C5
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use AdventureDomainMethod.Call.GmCmd_RemoveAllAdventures instead.", true)]
			public static void GmCmd_RemoveAllAdventures(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011AD7 RID: 72407 RVA: 0x006861D0 File Offset: 0x006843D0
			public static void ConsumeActionPointInAdventure(IAsyncMethodRequestHandler requestHandler, int value, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(10, 21, value, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011AD8 RID: 72408 RVA: 0x006861FC File Offset: 0x006843FC
			public static void GmCmd_ReGenerateAdventure(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(10, 22, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}
		}
	}
}
