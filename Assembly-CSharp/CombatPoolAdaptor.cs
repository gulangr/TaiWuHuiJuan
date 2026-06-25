using System;
using UnityEngine;

// Token: 0x02000157 RID: 343
public static class CombatPoolAdaptor
{
	// Token: 0x1700021A RID: 538
	// (get) Token: 0x060012E8 RID: 4840 RVA: 0x000739B6 File Offset: 0x00071BB6
	private static CombatPoolManager Pool
	{
		get
		{
			return SingletonObject.getInstance<CombatPoolManager>();
		}
	}

	// Token: 0x060012E9 RID: 4841 RVA: 0x000739C0 File Offset: 0x00071BC0
	public static void Preload(string key, GameObject prefab, int capacity)
	{
		prefab.SetActive(false);
		GameObject ownedPrefab = Object.Instantiate<GameObject>(prefab, CombatPoolAdaptor.Pool.transform);
		CombatPoolAdaptor.SetSrcObject(key, ownedPrefab, capacity);
	}

	// Token: 0x060012EA RID: 4842 RVA: 0x000739F0 File Offset: 0x00071BF0
	public static void SetSrcObject(string key, GameObject prefab, int capacity = 0)
	{
		prefab.gameObject.SetActive(false);
		CombatPoolAdaptor.Pool.CreatePool(key, prefab, capacity);
	}

	// Token: 0x060012EB RID: 4843 RVA: 0x00073A10 File Offset: 0x00071C10
	public static void RemoveData(string key)
	{
		bool flag = CombatPoolAdaptor.Pool != null;
		if (flag)
		{
			CombatPoolAdaptor.Pool.RemovePool(key);
		}
	}

	// Token: 0x060012EC RID: 4844 RVA: 0x00073A39 File Offset: 0x00071C39
	public static bool HasObject(string key)
	{
		return CombatPoolAdaptor.Pool.HasPool(key);
	}

	// Token: 0x060012ED RID: 4845 RVA: 0x00073A48 File Offset: 0x00071C48
	public static GameObject GetObject(string key, bool autoActive = true)
	{
		GameObject go = CombatPoolAdaptor.Pool.Get(key);
		bool flag = go != null && autoActive;
		if (flag)
		{
			go.SetActive(true);
		}
		return go;
	}

	// Token: 0x060012EE RID: 4846 RVA: 0x00073A7C File Offset: 0x00071C7C
	public static T GetObject<T>(string key, bool autoActive = true) where T : Component
	{
		GameObject go = CombatPoolAdaptor.GetObject(key, autoActive);
		return (go != null) ? go.GetComponent<T>() : default(T);
	}

	// Token: 0x060012EF RID: 4847 RVA: 0x00073AB0 File Offset: 0x00071CB0
	public static void Destroy(string key, GameObject instance, bool autoDeActive = true)
	{
		bool flag = CombatPoolAdaptor.Pool == null;
		if (!flag)
		{
			CombatPoolAdaptor.Pool.Return(key, instance);
			if (autoDeActive)
			{
				instance.SetActive(false);
			}
		}
	}
}
