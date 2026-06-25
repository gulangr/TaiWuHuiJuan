using System;
using System.Collections.Generic;
using System.Reflection;
using GameData.Common;

// Token: 0x02000111 RID: 273
public static class CombatNotifyHelper
{
	// Token: 0x060009DD RID: 2525 RVA: 0x0004133B File Offset: 0x0003F53B
	public static IEnumerable<CombatNotifyHandler> ParseHandlerData<T>(T instance) where T : class
	{
		Type type = instance.GetType();
		Type handlerType = typeof(CombatNotifyHandlerDelegate);
		ICombatNotifySubProcessor subProcessor = instance as ICombatNotifySubProcessor;
		foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod))
		{
			CombatNotifyMethodAttribute methodAttribute = method.GetCustomAttribute<CombatNotifyMethodAttribute>();
			CombatNotifyDataAttribute dataAttribute = method.GetCustomAttribute<CombatNotifyDataAttribute>();
			bool flag = methodAttribute == null && dataAttribute == null;
			if (!flag)
			{
				Delegate handlerNonGeneric = method.CreateDelegate(handlerType, instance);
				bool flag2 = handlerNonGeneric == null;
				if (!flag2)
				{
					yield return new CombatNotifyHandler
					{
						SubProcessor = subProcessor,
						HandlerDelegate = (CombatNotifyHandlerDelegate)handlerNonGeneric,
						MethodAttribute = methodAttribute,
						DataAttribute = dataAttribute
					};
					methodAttribute = null;
					dataAttribute = null;
					handlerNonGeneric = null;
					method = null;
				}
			}
		}
		MethodInfo[] array = null;
		yield break;
	}

	// Token: 0x060009DE RID: 2526 RVA: 0x0004134B File Offset: 0x0003F54B
	public static IEnumerable<DataUid> ParseAutoMonitors<T>()
	{
		Type type = typeof(T);
		foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod))
		{
			bool flag = method.GetCustomAttribute<CombatAutoMonitorAttribute>() == null;
			if (!flag)
			{
				CombatNotifyDataAttribute dataAttribute = method.GetCustomAttribute<CombatNotifyDataAttribute>();
				bool flag2 = dataAttribute == null;
				if (!flag2)
				{
					yield return dataAttribute.Uid;
					dataAttribute = null;
					method = null;
				}
			}
		}
		MethodInfo[] array = null;
		yield break;
	}

	// Token: 0x04000CE7 RID: 3303
	private const BindingFlags MethodBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod;
}
