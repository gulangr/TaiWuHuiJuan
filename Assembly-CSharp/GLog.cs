using System;
using System.Runtime.CompilerServices;
using GameData.Utilities;

// Token: 0x0200002A RID: 42
public class GLog : ISingletonInit, IDisposable
{
	// Token: 0x06000176 RID: 374 RVA: 0x0000A13D File Offset: 0x0000833D
	public void Init()
	{
	}

	// Token: 0x06000177 RID: 375 RVA: 0x0000A140 File Offset: 0x00008340
	public void Dispose()
	{
	}

	// Token: 0x06000178 RID: 376 RVA: 0x0000A143 File Offset: 0x00008343
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void internal_log(string msg, string tag = "", string trace = "")
	{
		AdaptableLog.Info("[" + tag + "]" + msg);
	}

	// Token: 0x06000179 RID: 377 RVA: 0x0000A15D File Offset: 0x0000835D
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void internal_warn(string msg, string tag = "", string trace = "")
	{
		AdaptableLog.Warning("[" + tag + "]" + msg, false);
	}

	// Token: 0x0600017A RID: 378 RVA: 0x0000A178 File Offset: 0x00008378
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void internal_error(string msg, string tag = "", string trace = "")
	{
		AdaptableLog.Error("[" + tag + "]" + msg);
	}

	// Token: 0x0600017B RID: 379 RVA: 0x0000A192 File Offset: 0x00008392
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Log(string msg)
	{
		GLog.internal_log(msg, "", "");
	}

	// Token: 0x0600017C RID: 380 RVA: 0x0000A1A8 File Offset: 0x000083A8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Log(object o, params object[] args)
	{
		string msg = o.ToString();
		bool flag = args.Length != 0 && o is string;
		if (flag)
		{
			msg = string.Format(msg, args);
		}
		GLog.internal_log(msg, "", "");
	}

	// Token: 0x0600017D RID: 381 RVA: 0x0000A1EC File Offset: 0x000083EC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void TagLog(string tag, object o, params object[] args)
	{
		string msg = o.ToString();
		bool flag = args.Length != 0 && o is string;
		if (flag)
		{
			msg = string.Format(msg, args);
		}
		GLog.internal_log(msg, tag, "");
	}

	// Token: 0x0600017E RID: 382 RVA: 0x0000A22A File Offset: 0x0000842A
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Warn(string msg)
	{
		GLog.internal_warn(msg, "", "");
	}

	// Token: 0x0600017F RID: 383 RVA: 0x0000A240 File Offset: 0x00008440
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Warn(object o, params object[] args)
	{
		string msg = o.ToString();
		bool flag = args.Length != 0 && o.GetType() == typeof(string);
		if (flag)
		{
			msg = string.Format(msg, args);
		}
		GLog.internal_warn(msg, "", "");
	}

	// Token: 0x06000180 RID: 384 RVA: 0x0000A290 File Offset: 0x00008490
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void TagWarn(string tag, object o, params object[] args)
	{
		string msg = o.ToString();
		bool flag = args.Length != 0 && o.GetType() == typeof(string);
		if (flag)
		{
			msg = string.Format(msg, args);
		}
		GLog.internal_warn(msg, tag, "");
	}

	// Token: 0x06000181 RID: 385 RVA: 0x0000A2DA File Offset: 0x000084DA
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Error(string msg)
	{
		GLog.internal_error(msg, "", "");
	}

	// Token: 0x06000182 RID: 386 RVA: 0x0000A2F0 File Offset: 0x000084F0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Error(object o, params object[] args)
	{
		string msg = o.ToString();
		bool flag = args.Length != 0 && o.GetType() == typeof(string);
		if (flag)
		{
			msg = string.Format(msg, args);
		}
		GLog.internal_error(msg, "", "");
	}

	// Token: 0x06000183 RID: 387 RVA: 0x0000A340 File Offset: 0x00008540
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void TagError(string tag, object o, params object[] args)
	{
		string msg = o.ToString();
		bool flag = args.Length != 0 && o.GetType() == typeof(string);
		if (flag)
		{
			msg = string.Format(msg, args);
		}
		GLog.internal_error(msg, tag, "");
	}

	// Token: 0x06000184 RID: 388 RVA: 0x0000A38A File Offset: 0x0000858A
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogWithTag(object msg, params object[] args)
	{
	}
}
