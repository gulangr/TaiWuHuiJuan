using System;
using System.Collections.Generic;

// Token: 0x02000124 RID: 292
public class DispatcherInstance : IAsyncMethodRequestHandler
{
	// Token: 0x06000B9D RID: 2973 RVA: 0x0004B302 File Offset: 0x00049502
	public void RegisterAsyncMethodCall(int requestId)
	{
		this._requestedAsyncMethod.Add(requestId);
	}

	// Token: 0x06000B9E RID: 2974 RVA: 0x0004B314 File Offset: 0x00049514
	public void ClearAsyncMethodCalls()
	{
		AsyncMethodDispatcher dispatcher = SingletonObject.getInstance<AsyncMethodDispatcher>();
		foreach (int one in this._requestedAsyncMethod)
		{
			dispatcher.UnregisterAsyncMethodCall(one);
		}
		this._requestedAsyncMethod.Clear();
	}

	// Token: 0x06000B9F RID: 2975 RVA: 0x0004B380 File Offset: 0x00049580
	public void AsyncMethodCall(ushort domainId, ushort methodId, AsyncMethodCallbackDelegate callback)
	{
		this._requestedAsyncMethod.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(domainId, methodId, callback));
	}

	// Token: 0x06000BA0 RID: 2976 RVA: 0x0004B39C File Offset: 0x0004959C
	public void AsyncMethodCall<T1>(ushort domainId, ushort methodId, T1 arg1, AsyncMethodCallbackDelegate callback)
	{
		this._requestedAsyncMethod.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<T1>(domainId, methodId, arg1, callback));
	}

	// Token: 0x06000BA1 RID: 2977 RVA: 0x0004B3BA File Offset: 0x000495BA
	public void AsyncMethodCall<T1, T2>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, AsyncMethodCallbackDelegate callback)
	{
		this._requestedAsyncMethod.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<T1, T2>(domainId, methodId, arg1, arg2, callback));
	}

	// Token: 0x06000BA2 RID: 2978 RVA: 0x0004B3DA File Offset: 0x000495DA
	public void AsyncMethodCall<T1, T2, T3>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, AsyncMethodCallbackDelegate callback)
	{
		this._requestedAsyncMethod.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<T1, T2, T3>(domainId, methodId, arg1, arg2, arg3, callback));
	}

	// Token: 0x06000BA3 RID: 2979 RVA: 0x0004B3FC File Offset: 0x000495FC
	public void AsyncMethodCall<T1, T2, T3, T4>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, AsyncMethodCallbackDelegate callback)
	{
		this._requestedAsyncMethod.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<T1, T2, T3, T4>(domainId, methodId, arg1, arg2, arg3, arg4, callback));
	}

	// Token: 0x06000BA4 RID: 2980 RVA: 0x0004B42C File Offset: 0x0004962C
	public void AsyncMethodCall<T1, T2, T3, T4, T5>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, AsyncMethodCallbackDelegate callback)
	{
		this._requestedAsyncMethod.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<T1, T2, T3, T4, T5>(domainId, methodId, arg1, arg2, arg3, arg4, arg5, callback));
	}

	// Token: 0x06000BA5 RID: 2981 RVA: 0x0004B460 File Offset: 0x00049660
	public void AsyncMethodCall<T1, T2, T3, T4, T5, T6>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, AsyncMethodCallbackDelegate callback)
	{
		this._requestedAsyncMethod.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<T1, T2, T3, T4, T5, T6>(domainId, methodId, arg1, arg2, arg3, arg4, arg5, arg6, callback));
	}

	// Token: 0x06000BA6 RID: 2982 RVA: 0x0004B494 File Offset: 0x00049694
	public void AsyncMethodCall<T1, T2, T3, T4, T5, T6, T7>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, AsyncMethodCallbackDelegate callback)
	{
		this._requestedAsyncMethod.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<T1, T2, T3, T4, T5, T6, T7>(domainId, methodId, arg1, arg2, arg3, arg4, arg5, arg6, arg7, callback));
	}

	// Token: 0x06000BA7 RID: 2983 RVA: 0x0004B4CC File Offset: 0x000496CC
	public void AsyncMethodCall<T1, T2, T3, T4, T5, T6, T7, T8>(ushort domainId, ushort methodId, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, AsyncMethodCallbackDelegate callback)
	{
		this._requestedAsyncMethod.Add(SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<T1, T2, T3, T4, T5, T6, T7, T8>(domainId, methodId, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, callback));
	}

	// Token: 0x04000DA0 RID: 3488
	private readonly List<int> _requestedAsyncMethod = new List<int>();
}
