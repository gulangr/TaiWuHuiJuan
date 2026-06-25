using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.CommandSystem;
using GameData.Domains.Character.AvatarSystem;
using UnityEngine;

// Token: 0x02000038 RID: 56
public sealed class SingletonObject : MonoBehaviour
{
	// Token: 0x17000044 RID: 68
	// (get) Token: 0x060001EF RID: 495 RVA: 0x0000BED0 File Offset: 0x0000A0D0
	// (set) Token: 0x060001F0 RID: 496 RVA: 0x0000BED7 File Offset: 0x0000A0D7
	public static bool IsDestroying { get; private set; }

	// Token: 0x060001F1 RID: 497 RVA: 0x0000BEE0 File Offset: 0x0000A0E0
	public static bool IsCreatedInstance<T>() where T : class, new()
	{
		bool flag = SingletonObject._container == null;
		return !flag && SingletonObject.SingletonMap != null && SingletonObject.SingletonMap.ContainsKey(typeof(T));
	}

	// Token: 0x060001F2 RID: 498 RVA: 0x0000BF24 File Offset: 0x0000A124
	public static void AddInstance<T>(T obj) where T : IDisposable
	{
		bool flag = obj != null && !SingletonObject.IsDestroying;
		if (flag)
		{
			SingletonObject.SingletonMap.Add(typeof(T), obj);
		}
	}

	// Token: 0x060001F3 RID: 499 RVA: 0x0000BF64 File Offset: 0x0000A164
	public static T getInstance<T>() where T : class, new()
	{
		Type type = typeof(T);
		bool flag = type.IsSubclassOf(typeof(Component));
		if (flag)
		{
			bool flag2 = Application.isPlaying && SingletonObject.IsDestroying;
			if (flag2)
			{
				GLog.Warn("SingletonObject is mark as Destroy! Can not get instance any more!");
				return default(T);
			}
			bool flag3 = SingletonObject._container == null;
			if (flag3)
			{
				SingletonObject._container = new GameObject();
				SingletonObject._container.name = SingletonObject.Name;
				SingletonObject._container.AddComponent(typeof(SingletonObject));
			}
		}
		bool flag4 = !SingletonObject.SingletonMap.ContainsKey(type);
		if (flag4)
		{
			object lockObj = SingletonObject.LockObj;
			lock (lockObj)
			{
				bool flag6 = type.IsSubclassOf(typeof(Component));
				if (flag6)
				{
					bool flag7 = !Application.isPlaying;
					if (flag7)
					{
						string tag = "SingletonObject";
						string str = "Can not getInstance of Component subclass when application is not playing:";
						Type type2 = type;
						GLog.TagWarn(tag, str + ((type2 != null) ? type2.ToString() : null), Array.Empty<object>());
						return default(T);
					}
					SingletonObject.SingletonMap.Add(type, SingletonObject._container.AddComponent(typeof(T)));
				}
				else
				{
					SingletonObject.SingletonMap.Add(type, Activator.CreateInstance<T>());
				}
				ISingletonInit sInit = SingletonObject.SingletonMap[type] as ISingletonInit;
				bool flag8 = sInit != null;
				if (flag8)
				{
					sInit.Init();
				}
			}
		}
		return SingletonObject.SingletonMap[type] as T;
	}

	// Token: 0x060001F4 RID: 500 RVA: 0x0000C130 File Offset: 0x0000A330
	public static void RemoveInstance<T>() where T : class, new()
	{
		bool flag = SingletonObject._container != null && SingletonObject.SingletonMap.ContainsKey(typeof(T));
		if (flag)
		{
			Type type = typeof(T);
			T instance = SingletonObject.SingletonMap[type] as T;
			SingletonObject.SingletonMap.Remove(type);
			IDisposable dispose = instance as IDisposable;
			bool flag2 = dispose != null;
			if (flag2)
			{
				dispose.Dispose();
			}
			bool flag3 = typeof(T).IsSubclassOf(typeof(Component));
			if (flag3)
			{
				Object.Destroy(instance as Component);
			}
			string str = "Singleton REMOVE! (";
			Type type2 = type;
			GLog.Warn(str + ((type2 != null) ? type2.ToString() : null) + ")");
		}
	}

	// Token: 0x060001F5 RID: 501 RVA: 0x0000C20C File Offset: 0x0000A40C
	public static void ClearInstances()
	{
		List<Type> removeList = new List<Type>();
		foreach (KeyValuePair<Type, object> pair in SingletonObject.SingletonMap)
		{
			bool flag = SingletonObject.DoNotClearList.Contains(pair.Value.GetType());
			if (!flag)
			{
				IDisposable dispose = pair.Value as IDisposable;
				bool flag2 = dispose != null;
				if (flag2)
				{
					dispose.Dispose();
				}
				bool flag3 = pair.Value.GetType().IsSubclassOf(typeof(Component));
				if (flag3)
				{
					Object.Destroy(pair.Value as Component);
				}
				removeList.Add(pair.Key);
			}
		}
		removeList.ForEach(delegate(Type key)
		{
			SingletonObject.SingletonMap.Remove(key);
		});
	}

	// Token: 0x060001F6 RID: 502 RVA: 0x0000C30C File Offset: 0x0000A50C
	private void Awake()
	{
		GLog.Log("Awake Singleton.");
		Object.DontDestroyOnLoad(base.gameObject);
		SingletonObject.IsDestroying = false;
	}

	// Token: 0x060001F7 RID: 503 RVA: 0x0000C32D File Offset: 0x0000A52D
	private void OnDestroy()
	{
		GLog.Log("Singleton OnDestroy");
		SingletonObject.IsDestroying = true;
	}

	// Token: 0x060001F8 RID: 504 RVA: 0x0000C344 File Offset: 0x0000A544
	private void OnApplicationQuit()
	{
		bool readyToQuit = GameApp.ReadyToQuit;
		if (readyToQuit)
		{
			GLog.Log("Destroy Singleton");
			bool flag = SingletonObject._container != null;
			if (flag)
			{
				Object.Destroy(SingletonObject._container);
				SingletonObject._container = null;
				SingletonObject.IsDestroying = true;
			}
		}
	}

	// Token: 0x040000F8 RID: 248
	private static GameObject _container;

	// Token: 0x040000F9 RID: 249
	private static readonly string Name = "Singleton";

	// Token: 0x040000FA RID: 250
	private static readonly Dictionary<Type, object> SingletonMap = new Dictionary<Type, object>();

	// Token: 0x040000FB RID: 251
	private static readonly object LockObj = new object();

	// Token: 0x040000FC RID: 252
	private static readonly List<Type> DoNotClearList = new List<Type>
	{
		typeof(YieldHelper),
		typeof(GLog),
		typeof(TooltipManager),
		typeof(AvatarManager),
		typeof(GlobalSettings),
		typeof(ResLoader),
		typeof(GameSort),
		typeof(AudioManager),
		typeof(CommandManager),
		typeof(ItemViewPool),
		typeof(CombatPoolManager),
		typeof(DlcManager),
		typeof(SpritePackerHandler),
		typeof(InternalDlcSystem)
	};
}
