using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Config;
using GameData.GameDataBridge;

// Token: 0x02000101 RID: 257
public class AsyncMethodDispatcher
{
	// Token: 0x060008AC RID: 2220 RVA: 0x0003B458 File Offset: 0x00039658
	public void OnWorldDataReady()
	{
		this._listenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
		this._receivedCount = 0;
		this._sentCount = 0;
		this._asyncCallbacks = new ConcurrentQueue<AsyncMethodCallbackDelegate>();
		this._asyncCheckers = new ConcurrentQueue<AsyncMethodDispatcher.AsyncMethodCallChecker>();
		this._skipIds = new HashSet<int>();
	}

	// Token: 0x060008AD RID: 2221 RVA: 0x0003B4AC File Offset: 0x000396AC
	public void OnLeaveWorld()
	{
		bool flag = this._listenerId >= 0;
		if (flag)
		{
			GameDataBridge.UnregisterListener(this._listenerId);
			this._listenerId = -1;
			this._asyncCallbacks = null;
			this._asyncCheckers = null;
			this._skipIds = null;
		}
	}

	// Token: 0x060008AE RID: 2222 RVA: 0x0003B4F4 File Offset: 0x000396F4
	public void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			bool flag = notification.Type != 1;
			if (flag)
			{
				throw new Exception("Error Game Data Notification");
			}
			this._receivedCount++;
			AsyncMethodDispatcher.AsyncMethodCallChecker checker;
			AsyncMethodCallbackDelegate callback;
			bool flag2 = !this._asyncCheckers.TryDequeue(out checker) || !this._asyncCallbacks.TryDequeue(out callback);
			if (flag2)
			{
				throw new Exception(string.Format("AsyncMethodDispatcher handled invalid checker or callback : {0}, {1}", notification.DomainId, notification.MethodId));
			}
			bool flag3 = this._skipIds.Contains(this._receivedCount);
			if (flag3)
			{
				this._skipIds.Remove(this._receivedCount);
			}
			else
			{
				bool flag4 = checker.DomainId != notification.DomainId || checker.MethodId != notification.MethodId;
				if (flag4)
				{
					throw new Exception(string.Format("AsyncMethodDispatcher handled mismatch notification : checker({0}${1}), notification({2}${3})", new object[]
					{
						checker.DomainId,
						checker.MethodId,
						notification.DomainId,
						notification.MethodId
					}));
				}
				try
				{
					if (callback != null)
					{
						callback(notification.ValueOffset, wrapper.DataPool);
					}
				}
				catch (Exception e)
				{
					PredefinedLog.Show(18, string.Format("AsyncMethodDispatcher catch a exception with ({0},{1}), \nexception={2}\ncallback={3}", new object[]
					{
						notification.DomainId,
						notification.MethodId,
						e,
						(callback != null) ? callback.Method : null
					}));
				}
			}
		}
	}

	// Token: 0x060008AF RID: 2223 RVA: 0x0003B700 File Offset: 0x00039900
	public void UnregisterAsyncMethodCall(int id)
	{
		HashSet<int> skipIds = this._skipIds;
		if (skipIds != null)
		{
			skipIds.Add(id);
		}
	}

	// Token: 0x060008B0 RID: 2224 RVA: 0x0003B718 File Offset: 0x00039918
	private void EnqueueMethodCallback(ushort domainId, ushort methodId, AsyncMethodCallbackDelegate callback)
	{
		bool flag = this._asyncCallbacks == null || this._asyncCheckers == null;
		if (flag)
		{
			PredefinedLog.Show(14, string.Format("Send request after leave world ({0},{1})", domainId, methodId));
		}
		else
		{
			this._sentCount++;
			this._asyncCallbacks.Enqueue(callback);
			this._asyncCheckers.Enqueue(new AsyncMethodDispatcher.AsyncMethodCallChecker
			{
				DomainId = domainId,
				MethodId = methodId
			});
		}
	}

	// Token: 0x060008B1 RID: 2225 RVA: 0x0003B7A0 File Offset: 0x000399A0
	public int AsyncMethodCall(ushort domainId, ushort methodId, AsyncMethodCallbackDelegate callback)
	{
		GameDataBridge.AddMethodCall(this._listenerId, domainId, methodId);
		this.EnqueueMethodCallback(domainId, methodId, callback);
		return this._sentCount;
	}

	// Token: 0x060008B2 RID: 2226 RVA: 0x0003B7D0 File Offset: 0x000399D0
	public int AsyncMethodCall<T1>(ushort domainId, ushort methodId, T1 arg1, AsyncMethodCallbackDelegate callback)
	{
		GameDataBridge.AddMethodCall<T1>(this._listenerId, domainId, methodId, arg1);
		this.EnqueueMethodCallback(domainId, methodId, callback);
		return this._sentCount;
	}

	// Token: 0x060008B3 RID: 2227 RVA: 0x0003B804 File Offset: 0x00039A04
	public int AsyncMethodCall<T1, T2>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, AsyncMethodCallbackDelegate callback)
	{
		GameDataBridge.AddMethodCall<T1, T2>(this._listenerId, domainId, methodId, arg1, arg2);
		this.EnqueueMethodCallback(domainId, methodId, callback);
		return this._sentCount;
	}

	// Token: 0x060008B4 RID: 2228 RVA: 0x0003B838 File Offset: 0x00039A38
	public int AsyncMethodCall<T1, T2, T3>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, AsyncMethodCallbackDelegate callback)
	{
		GameDataBridge.AddMethodCall<T1, T2, T3>(this._listenerId, domainId, methodId, arg1, arg2, arg3);
		this.EnqueueMethodCallback(domainId, methodId, callback);
		return this._sentCount;
	}

	// Token: 0x060008B5 RID: 2229 RVA: 0x0003B870 File Offset: 0x00039A70
	public int AsyncMethodCall<T1, T2, T3, T4>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, AsyncMethodCallbackDelegate callback)
	{
		GameDataBridge.AddMethodCall<T1, T2, T3, T4>(this._listenerId, domainId, methodId, arg1, arg2, arg3, arg4);
		this.EnqueueMethodCallback(domainId, methodId, callback);
		return this._sentCount;
	}

	// Token: 0x060008B6 RID: 2230 RVA: 0x0003B8A8 File Offset: 0x00039AA8
	public int AsyncMethodCall<T1, T2, T3, T4, T5>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, AsyncMethodCallbackDelegate callback)
	{
		GameDataBridge.AddMethodCall<T1, T2, T3, T4, T5>(this._listenerId, domainId, methodId, arg1, arg2, arg3, arg4, arg5);
		this.EnqueueMethodCallback(domainId, methodId, callback);
		return this._sentCount;
	}

	// Token: 0x060008B7 RID: 2231 RVA: 0x0003B8E4 File Offset: 0x00039AE4
	public int AsyncMethodCall<T1, T2, T3, T4, T5, T6>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, AsyncMethodCallbackDelegate callback)
	{
		GameDataBridge.AddMethodCall<T1, T2, T3, T4, T5, T6>(this._listenerId, domainId, methodId, arg1, arg2, arg3, arg4, arg5, arg6);
		this.EnqueueMethodCallback(domainId, methodId, callback);
		return this._sentCount;
	}

	// Token: 0x060008B8 RID: 2232 RVA: 0x0003B920 File Offset: 0x00039B20
	public int AsyncMethodCall<T1, T2, T3, T4, T5, T6, T7>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, AsyncMethodCallbackDelegate callback)
	{
		GameDataBridge.AddMethodCall<T1, T2, T3, T4, T5, T6, T7>(this._listenerId, domainId, methodId, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
		this.EnqueueMethodCallback(domainId, methodId, callback);
		return this._sentCount;
	}

	// Token: 0x060008B9 RID: 2233 RVA: 0x0003B960 File Offset: 0x00039B60
	public int AsyncMethodCall<T1, T2, T3, T4, T5, T6, T7, T8>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, AsyncMethodCallbackDelegate callback)
	{
		GameDataBridge.AddMethodCall<T1, T2, T3, T4, T5, T6, T7, T8>(this._listenerId, domainId, methodId, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
		this.EnqueueMethodCallback(domainId, methodId, callback);
		return this._sentCount;
	}

	// Token: 0x04000BC7 RID: 3015
	private int _listenerId = -1;

	// Token: 0x04000BC8 RID: 3016
	private int _receivedCount = 0;

	// Token: 0x04000BC9 RID: 3017
	private int _sentCount = 0;

	// Token: 0x04000BCA RID: 3018
	private ConcurrentQueue<AsyncMethodCallbackDelegate> _asyncCallbacks;

	// Token: 0x04000BCB RID: 3019
	private ConcurrentQueue<AsyncMethodDispatcher.AsyncMethodCallChecker> _asyncCheckers;

	// Token: 0x04000BCC RID: 3020
	private HashSet<int> _skipIds;

	// Token: 0x02001151 RID: 4433
	public struct AsyncMethodCallChecker
	{
		// Token: 0x0400968E RID: 38542
		public ushort DomainId;

		// Token: 0x0400968F RID: 38543
		public ushort MethodId;
	}
}
