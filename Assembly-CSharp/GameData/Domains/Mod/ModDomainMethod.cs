using System;
using GameData.GameDataBridge;

namespace GameData.Domains.Mod
{
	// Token: 0x02000FC2 RID: 4034
	public static class ModDomainMethod
	{
		// Token: 0x020025FF RID: 9727
		public static class Call
		{
			// Token: 0x06011154 RID: 69972 RVA: 0x006782F3 File Offset: 0x006764F3
			public static void SetInt(int listenerId, string modIdStr, string dataName, bool isArchive, int val)
			{
				GameDataBridge.AddMethodCall<string, string, bool, int>(listenerId, 16, 0, modIdStr, dataName, isArchive, val);
			}

			// Token: 0x06011155 RID: 69973 RVA: 0x00678305 File Offset: 0x00676505
			public static void SetBool(int listenerId, string modIdStr, string dataName, bool isArchive, bool val)
			{
				GameDataBridge.AddMethodCall<string, string, bool, bool>(listenerId, 16, 1, modIdStr, dataName, isArchive, val);
			}

			// Token: 0x06011156 RID: 69974 RVA: 0x00678317 File Offset: 0x00676517
			public static void SetString(int listenerId, string modIdStr, string dataName, bool isArchive, string val)
			{
				GameDataBridge.AddMethodCall<string, string, bool, string>(listenerId, 16, 2, modIdStr, dataName, isArchive, val);
			}

			// Token: 0x06011157 RID: 69975 RVA: 0x00678329 File Offset: 0x00676529
			public static void SetSerializableModData(int listenerId, string modIdStr, string dataName, bool isArchive, SerializableModData val)
			{
				GameDataBridge.AddMethodCall<string, string, bool, SerializableModData>(listenerId, 16, 3, modIdStr, dataName, isArchive, val);
			}

			// Token: 0x06011158 RID: 69976 RVA: 0x0067833B File Offset: 0x0067653B
			public static void GetInt(int listenerId, string modIdStr, string dataName, bool isArchive)
			{
				GameDataBridge.AddMethodCall<string, string, bool>(listenerId, 16, 4, modIdStr, dataName, isArchive);
			}

			// Token: 0x06011159 RID: 69977 RVA: 0x0067834B File Offset: 0x0067654B
			public static void GetBool(int listenerId, string modIdStr, string dataName, bool isArchive)
			{
				GameDataBridge.AddMethodCall<string, string, bool>(listenerId, 16, 5, modIdStr, dataName, isArchive);
			}

			// Token: 0x0601115A RID: 69978 RVA: 0x0067835B File Offset: 0x0067655B
			public static void GetString(int listenerId, string modIdStr, string dataName, bool isArchive)
			{
				GameDataBridge.AddMethodCall<string, string, bool>(listenerId, 16, 6, modIdStr, dataName, isArchive);
			}

			// Token: 0x0601115B RID: 69979 RVA: 0x0067836B File Offset: 0x0067656B
			public static void GetSerializableModData(int listenerId, string modIdStr, string dataName, bool isArchive)
			{
				GameDataBridge.AddMethodCall<string, string, bool>(listenerId, 16, 7, modIdStr, dataName, isArchive);
			}

			// Token: 0x0601115C RID: 69980 RVA: 0x0067837B File Offset: 0x0067657B
			public static void UpdateModSettings(ModId modId, SerializableModData modData)
			{
				GameDataBridge.AddMethodCall<ModId, SerializableModData>(-1, 16, 8, modId, modData);
			}

			// Token: 0x0601115D RID: 69981 RVA: 0x0067838A File Offset: 0x0067658A
			public static void CallModMethod(string modIdStr, string methodName)
			{
				GameDataBridge.AddMethodCall<string, string>(-1, 16, 9, modIdStr, methodName);
			}

			// Token: 0x0601115E RID: 69982 RVA: 0x0067839A File Offset: 0x0067659A
			public static void CallModMethodWithParam(string modIdStr, string methodName, SerializableModData parameter)
			{
				GameDataBridge.AddMethodCall<string, string, SerializableModData>(-1, 16, 10, modIdStr, methodName, parameter);
			}

			// Token: 0x0601115F RID: 69983 RVA: 0x006783AB File Offset: 0x006765AB
			public static void CallModMethodWithRet(int listenerId, string modIdStr, string methodName)
			{
				GameDataBridge.AddMethodCall<string, string>(listenerId, 16, 11, modIdStr, methodName);
			}

			// Token: 0x06011160 RID: 69984 RVA: 0x006783BB File Offset: 0x006765BB
			public static void CallModMethodWithParamAndRet(int listenerId, string modIdStr, string methodName, SerializableModData parameter)
			{
				GameDataBridge.AddMethodCall<string, string, SerializableModData>(listenerId, 16, 12, modIdStr, methodName, parameter);
			}
		}

		// Token: 0x02002600 RID: 9728
		public static class AsyncCall
		{
			// Token: 0x06011161 RID: 69985 RVA: 0x006783CC File Offset: 0x006765CC
			public static void SetInt(IAsyncMethodRequestHandler requestHandler, string modIdStr, string dataName, bool isArchive, int val, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<string, string, bool, int>(16, 0, modIdStr, dataName, isArchive, val, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011162 RID: 69986 RVA: 0x006783FC File Offset: 0x006765FC
			public static void SetBool(IAsyncMethodRequestHandler requestHandler, string modIdStr, string dataName, bool isArchive, bool val, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<string, string, bool, bool>(16, 1, modIdStr, dataName, isArchive, val, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011163 RID: 69987 RVA: 0x0067842C File Offset: 0x0067662C
			public static void SetString(IAsyncMethodRequestHandler requestHandler, string modIdStr, string dataName, bool isArchive, string val, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<string, string, bool, string>(16, 2, modIdStr, dataName, isArchive, val, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011164 RID: 69988 RVA: 0x0067845C File Offset: 0x0067665C
			public static void SetSerializableModData(IAsyncMethodRequestHandler requestHandler, string modIdStr, string dataName, bool isArchive, SerializableModData val, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<string, string, bool, SerializableModData>(16, 3, modIdStr, dataName, isArchive, val, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011165 RID: 69989 RVA: 0x0067848C File Offset: 0x0067668C
			public static void GetInt(IAsyncMethodRequestHandler requestHandler, string modIdStr, string dataName, bool isArchive, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<string, string, bool>(16, 4, modIdStr, dataName, isArchive, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011166 RID: 69990 RVA: 0x006784BC File Offset: 0x006766BC
			public static void GetBool(IAsyncMethodRequestHandler requestHandler, string modIdStr, string dataName, bool isArchive, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<string, string, bool>(16, 5, modIdStr, dataName, isArchive, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011167 RID: 69991 RVA: 0x006784EC File Offset: 0x006766EC
			public static void GetString(IAsyncMethodRequestHandler requestHandler, string modIdStr, string dataName, bool isArchive, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<string, string, bool>(16, 6, modIdStr, dataName, isArchive, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011168 RID: 69992 RVA: 0x0067851C File Offset: 0x0067671C
			public static void GetSerializableModData(IAsyncMethodRequestHandler requestHandler, string modIdStr, string dataName, bool isArchive, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<string, string, bool>(16, 7, modIdStr, dataName, isArchive, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011169 RID: 69993 RVA: 0x0067854A File Offset: 0x0067674A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ModDomainMethod.Call.UpdateModSettings instead.", true)]
			public static void UpdateModSettings(IAsyncMethodRequestHandler requestHandler, ModId modId, SerializableModData modData, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601116A RID: 69994 RVA: 0x00678552 File Offset: 0x00676752
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ModDomainMethod.Call.CallModMethod instead.", true)]
			public static void CallModMethod(IAsyncMethodRequestHandler requestHandler, string modIdStr, string methodName, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601116B RID: 69995 RVA: 0x0067855A File Offset: 0x0067675A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ModDomainMethod.Call.CallModMethodWithParam instead.", true)]
			public static void CallModMethodWithParam(IAsyncMethodRequestHandler requestHandler, string modIdStr, string methodName, SerializableModData parameter, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601116C RID: 69996 RVA: 0x00678564 File Offset: 0x00676764
			public static void CallModMethodWithRet(IAsyncMethodRequestHandler requestHandler, string modIdStr, string methodName, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<string, string>(16, 11, modIdStr, methodName, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601116D RID: 69997 RVA: 0x00678594 File Offset: 0x00676794
			public static void CallModMethodWithParamAndRet(IAsyncMethodRequestHandler requestHandler, string modIdStr, string methodName, SerializableModData parameter, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<string, string, SerializableModData>(16, 12, modIdStr, methodName, parameter, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}
		}
	}
}
