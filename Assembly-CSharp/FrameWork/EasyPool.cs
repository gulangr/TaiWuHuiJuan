using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine.Pool;

namespace FrameWork
{
	// Token: 0x02000FE3 RID: 4067
	public static class EasyPool
	{
		// Token: 0x0600B9D9 RID: 47577 RVA: 0x0054A9EC File Offset: 0x00548BEC
		public static T Get<T>() where T : class, new()
		{
			return EasyPool.ObjectPool<T>.GetObject();
		}

		// Token: 0x0600B9DA RID: 47578 RVA: 0x0054AA08 File Offset: 0x00548C08
		public static void Free<T>(T freeObj) where T : class, new()
		{
			bool flag = freeObj == null;
			if (!flag)
			{
				bool shouldReject = EasyPool.ShouldReject<T>(freeObj);
				bool flag2 = shouldReject;
				if (!flag2)
				{
					EasyPool.ObjectPool<T>.FreeObject(freeObj);
				}
			}
		}

		// Token: 0x0600B9DB RID: 47579 RVA: 0x0054AA3C File Offset: 0x00548C3C
		private static bool ShouldReject<T>(T freeObj) where T : class, new()
		{
			bool flag = freeObj == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Type type = freeObj.GetType();
				bool flag2 = !type.IsGenericType;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = type.GetGenericTypeDefinition() == typeof(List<>);
					if (flag3)
					{
						int capacity = ListCapacityHelper.GetCapacity(freeObj);
						bool flag4 = capacity <= 256;
						if (flag4)
						{
							result = false;
						}
						else
						{
							EasyPool.<ShouldReject>g__Log|4_0<T>(capacity);
							result = true;
						}
					}
					else
					{
						bool flag5 = type.GetGenericTypeDefinition() == typeof(Dictionary<, >);
						if (flag5)
						{
							int capacity2 = DictionaryCapacityHelper.GetCapacity(freeObj);
							bool flag6 = capacity2 <= 256;
							if (flag6)
							{
								result = false;
							}
							else
							{
								EasyPool.<ShouldReject>g__Log|4_0<T>(capacity2);
								result = true;
							}
						}
						else
						{
							result = false;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600B9DC RID: 47580 RVA: 0x0054AB1C File Offset: 0x00548D1C
		[CompilerGenerated]
		internal static void <ShouldReject>g__Log|4_0<T>(int capacity) where T : class, new()
		{
		}

		// Token: 0x04008FBC RID: 36796
		private const int MaxCapacity = 256;

		// Token: 0x02002630 RID: 9776
		private static class ObjectPool<T> where T : class, new()
		{
			// Token: 0x06011B3F RID: 72511 RVA: 0x00687014 File Offset: 0x00685214
			static ObjectPool()
			{
				EasyPool.ObjectPool<T>.Pool = new UnityEngine.Pool.ObjectPool<T>(() => Activator.CreateInstance<T>(), null, new Action<T>(EasyPool.ObjectPool<T>.OnRelease), null, true, 0, 16);
				EasyPool.ObjectPool<T>.ClearMethod = typeof(T).GetMethod("Clear");
			}

			// Token: 0x06011B40 RID: 72512 RVA: 0x00687071 File Offset: 0x00685271
			private static void OnRelease(T obj)
			{
				MethodInfo clearMethod = EasyPool.ObjectPool<T>.ClearMethod;
				if (clearMethod != null)
				{
					clearMethod.Invoke(obj, EasyPool.ObjectPool<T>.ReusableClearParameterArray);
				}
			}

			// Token: 0x06011B41 RID: 72513 RVA: 0x00687090 File Offset: 0x00685290
			public static T GetObject()
			{
				return EasyPool.ObjectPool<T>.Pool.Get();
			}

			// Token: 0x06011B42 RID: 72514 RVA: 0x006870AC File Offset: 0x006852AC
			public static void FreeObject(T freeObj)
			{
				bool flag = freeObj == null;
				if (!flag)
				{
					EasyPool.ObjectPool<T>.Pool.Release(freeObj);
				}
			}

			// Token: 0x0400E9E2 RID: 59874
			private static readonly UnityEngine.Pool.ObjectPool<T> Pool;

			// Token: 0x0400E9E3 RID: 59875
			private static readonly MethodInfo ClearMethod;

			// Token: 0x0400E9E4 RID: 59876
			private static readonly object[] ReusableClearParameterArray = Array.Empty<object>();
		}
	}
}
