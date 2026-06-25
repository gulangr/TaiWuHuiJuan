using System;
using System.Collections.Generic;
using GameData.Utilities;
using UnityEngine;

// Token: 0x02000158 RID: 344
public class CombatPoolManager : MonoBehaviour, ISingletonInit, IDisposable
{
	// Token: 0x060012F0 RID: 4848 RVA: 0x00073AEC File Offset: 0x00071CEC
	public void CreatePool(string key, GameObject prefab, int capacity)
	{
		CombatPool pool = this._pools.GetOrNew(key);
		bool flag = pool.Prefab != null && pool.Prefab != prefab;
		if (flag)
		{
			AdaptableLog.TagWarning("CombatPoolManager", "pool prefab " + key + " be override", false);
		}
		pool.Prefab = prefab;
		pool.EnsureCapacity(capacity);
	}

	// Token: 0x060012F1 RID: 4849 RVA: 0x00073B54 File Offset: 0x00071D54
	public void RemovePool(string key)
	{
		CombatPool pool;
		bool flag = this._pools.TryGetValue(key, out pool);
		if (flag)
		{
			pool.DestroyAll();
			this._pools.Remove(key);
		}
	}

	// Token: 0x060012F2 RID: 4850 RVA: 0x00073B8A File Offset: 0x00071D8A
	public bool HasPool(string key)
	{
		return this._pools.ContainsKey(key);
	}

	// Token: 0x060012F3 RID: 4851 RVA: 0x00073B98 File Offset: 0x00071D98
	public GameObject Get(string key)
	{
		CombatPool pool;
		return this._pools.TryGetValue(key, out pool) ? pool.Get() : null;
	}

	// Token: 0x060012F4 RID: 4852 RVA: 0x00073BBE File Offset: 0x00071DBE
	public void Return(string key, GameObject go)
	{
		this._pools[key].Return(go);
	}

	// Token: 0x060012F5 RID: 4853 RVA: 0x00073BD3 File Offset: 0x00071DD3
	public void Init()
	{
	}

	// Token: 0x060012F6 RID: 4854 RVA: 0x00073BD6 File Offset: 0x00071DD6
	public void Dispose()
	{
	}

	// Token: 0x04001002 RID: 4098
	private readonly Dictionary<string, CombatPool> _pools = new Dictionary<string, CombatPool>();
}
