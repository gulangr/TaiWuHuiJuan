using System;
using GameData.Domains.LifeRecord.GeneralRecord;
using GameData.GameDataBridge;

namespace GameData.Domains.LifeRecord
{
	// Token: 0x02000FC5 RID: 4037
	public static class LifeRecordDomainMethod
	{
		// Token: 0x02002605 RID: 9733
		public static class Call
		{
			// Token: 0x06011264 RID: 70244 RVA: 0x00679C44 File Offset: 0x00677E44
			public static void Get(int listenerId, int charId, int beginIndex, int count)
			{
				GameDataBridge.AddMethodCall<int, int, int>(listenerId, 13, 0, charId, beginIndex, count);
			}

			// Token: 0x06011265 RID: 70245 RVA: 0x00679C54 File Offset: 0x00677E54
			public static void GetByDate(int listenerId, int charId, int startDate, int monthCount)
			{
				GameDataBridge.AddMethodCall<int, int, int>(listenerId, 13, 1, charId, startDate, monthCount);
			}

			// Token: 0x06011266 RID: 70246 RVA: 0x00679C64 File Offset: 0x00677E64
			public static void GetLast(int listenerId, int charId, int count)
			{
				GameDataBridge.AddMethodCall<int, int>(listenerId, 13, 2, charId, count);
			}

			// Token: 0x06011267 RID: 70247 RVA: 0x00679C73 File Offset: 0x00677E73
			public static void GetRelated(int listenerId, int charId, int date, short recordType, int relatedCharId)
			{
				GameDataBridge.AddMethodCall<int, int, short, int>(listenerId, 13, 3, charId, date, recordType, relatedCharId);
			}

			// Token: 0x06011268 RID: 70248 RVA: 0x00679C85 File Offset: 0x00677E85
			public static void GetDead(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 13, 4, charId);
			}

			// Token: 0x06011269 RID: 70249 RVA: 0x00679C93 File Offset: 0x00677E93
			public static void GetRecordRenderInfoArguments(int listenerId, string key, RecordArgumentsRequest request)
			{
				GameDataBridge.AddMethodCall<string, RecordArgumentsRequest>(listenerId, 13, 5, key, request);
			}

			// Token: 0x0601126A RID: 70250 RVA: 0x00679CA2 File Offset: 0x00677EA2
			public static void GetRecordRenderInfoArguments(int listenerId, string key, RecordArgumentsRequest request, bool isDreamBack)
			{
				GameDataBridge.AddMethodCall<string, RecordArgumentsRequest, bool>(listenerId, 13, 5, key, request, isDreamBack);
			}

			// Token: 0x0601126B RID: 70251 RVA: 0x00679CB2 File Offset: 0x00677EB2
			public static void GetReversedRecord(int listenerId, int charId, int startCount, int readCount)
			{
				GameDataBridge.AddMethodCall<int, int, int>(listenerId, 13, 6, charId, startCount, readCount);
			}

			// Token: 0x0601126C RID: 70252 RVA: 0x00679CC2 File Offset: 0x00677EC2
			public static void GetReversedRecord(int listenerId, int charId, int startCount, int readCount, bool isDreamBack)
			{
				GameDataBridge.AddMethodCall<int, int, int, bool>(listenerId, 13, 6, charId, startCount, readCount, isDreamBack);
			}
		}

		// Token: 0x02002606 RID: 9734
		public static class AsyncCall
		{
			// Token: 0x0601126D RID: 70253 RVA: 0x00679CD4 File Offset: 0x00677ED4
			public static void Get(IAsyncMethodRequestHandler requestHandler, int charId, int beginIndex, int count, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int, int>(13, 0, charId, beginIndex, count, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601126E RID: 70254 RVA: 0x00679D04 File Offset: 0x00677F04
			public static void GetByDate(IAsyncMethodRequestHandler requestHandler, int charId, int startDate, int monthCount, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int, int>(13, 1, charId, startDate, monthCount, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601126F RID: 70255 RVA: 0x00679D34 File Offset: 0x00677F34
			public static void GetLast(IAsyncMethodRequestHandler requestHandler, int charId, int count, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int>(13, 2, charId, count, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011270 RID: 70256 RVA: 0x00679D60 File Offset: 0x00677F60
			public static void GetRelated(IAsyncMethodRequestHandler requestHandler, int charId, int date, short recordType, int relatedCharId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int, short, int>(13, 3, charId, date, recordType, relatedCharId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011271 RID: 70257 RVA: 0x00679D90 File Offset: 0x00677F90
			public static void GetDead(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(13, 4, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011272 RID: 70258 RVA: 0x00679DBC File Offset: 0x00677FBC
			public static void GetRecordRenderInfoArguments(IAsyncMethodRequestHandler requestHandler, string key, RecordArgumentsRequest request, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<string, RecordArgumentsRequest>(13, 5, key, request, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011273 RID: 70259 RVA: 0x00679DE8 File Offset: 0x00677FE8
			public static void GetRecordRenderInfoArguments(IAsyncMethodRequestHandler requestHandler, string key, RecordArgumentsRequest request, bool isDreamBack, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<string, RecordArgumentsRequest, bool>(13, 5, key, request, isDreamBack, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011274 RID: 70260 RVA: 0x00679E18 File Offset: 0x00678018
			public static void GetReversedRecord(IAsyncMethodRequestHandler requestHandler, int charId, int startCount, int readCount, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int, int>(13, 6, charId, startCount, readCount, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011275 RID: 70261 RVA: 0x00679E48 File Offset: 0x00678048
			public static void GetReversedRecord(IAsyncMethodRequestHandler requestHandler, int charId, int startCount, int readCount, bool isDreamBack, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int, int, bool>(13, 6, charId, startCount, readCount, isDreamBack, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}
		}
	}
}
