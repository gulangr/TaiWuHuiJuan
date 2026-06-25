using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200002D RID: 45
public class PoolItem
{
	// Token: 0x0600019E RID: 414 RVA: 0x0000AB08 File Offset: 0x00008D08
	public PoolItem(string path, GameObject srcPrefab)
	{
		this.path = path;
		this.prefab = srcPrefab;
		this.objectList = new Dictionary<int, PoolItemTime>();
		this.prefab.gameObject.SetActive(false);
	}

	// Token: 0x0600019F RID: 415 RVA: 0x0000AB40 File Offset: 0x00008D40
	public void DestroyObject(GameObject gameObject)
	{
		int hashKey = gameObject.GetInstanceID();
		bool flag = this.objectList.ContainsKey(hashKey);
		if (flag)
		{
			this.objectList[hashKey].Destroy();
		}
		else
		{
			PoolItemTime item = new PoolItemTime(gameObject);
			this.objectList.Add(hashKey, item);
			item.Destroy();
		}
	}

	// Token: 0x060001A0 RID: 416 RVA: 0x0000AB9C File Offset: 0x00008D9C
	public GameObject GetObject()
	{
		bool flag = null == this.prefab;
		GameObject result;
		if (flag)
		{
			result = null;
		}
		else
		{
			this.RemoveNullElems();
			foreach (PoolItemTime poolItemTime in this.objectList.Values)
			{
				bool destroyStatus = poolItemTime.DestroyStatus;
				if (destroyStatus)
				{
					return poolItemTime.Active();
				}
			}
			GameObject copy = Object.Instantiate<GameObject>(this.prefab);
			int hashKey = copy.GetInstanceID();
			for (int i = 0; i < 20; i++)
			{
				bool flag2 = !this.objectList.ContainsKey(hashKey);
				if (flag2)
				{
					break;
				}
				Object.Destroy(copy);
				copy = Object.Instantiate<GameObject>(this.prefab);
				hashKey = copy.GetInstanceID();
			}
			PoolItemTime item = new PoolItemTime(copy);
			this.objectList.Add(hashKey, item);
			result = item.Active();
		}
		return result;
	}

	// Token: 0x060001A1 RID: 417 RVA: 0x0000ACB0 File Offset: 0x00008EB0
	public void RemoveObject(GameObject gameObject)
	{
		int hashKey = gameObject.GetInstanceID();
		bool flag = this.objectList.ContainsKey(hashKey);
		if (flag)
		{
			Object.Destroy(gameObject);
			this.objectList.Remove(hashKey);
		}
	}

	// Token: 0x060001A2 RID: 418 RVA: 0x0000ACEC File Offset: 0x00008EEC
	public void Destroy()
	{
		List<PoolItemTime> poolList = new List<PoolItemTime>();
		bool flag = this.objectList != null;
		if (flag)
		{
			foreach (PoolItemTime poolItemTime in this.objectList.Values)
			{
				poolList.Add(poolItemTime);
			}
		}
		poolList.ForEach(delegate(PoolItemTime item)
		{
			bool flag2 = null != item.gameObject;
			if (flag2)
			{
				Object.Destroy(item.gameObject);
			}
		});
		this.objectList = null;
	}

	// Token: 0x060001A3 RID: 419 RVA: 0x0000AD8C File Offset: 0x00008F8C
	public void ExpireObject()
	{
		IList<PoolItemTime> expireList = new List<PoolItemTime>();
		foreach (PoolItemTime poolItemTime in this.objectList.Values)
		{
			bool flag = poolItemTime.IsExpire();
			if (flag)
			{
				expireList.Add(poolItemTime);
			}
		}
		int expireCount = expireList.Count;
		for (int index = 0; index < expireCount; index++)
		{
			this.RemoveObject(expireList[index].gameObject);
		}
	}

	// Token: 0x060001A4 RID: 420 RVA: 0x0000AE30 File Offset: 0x00009030
	public bool RemoveNullElems()
	{
		List<int> removeList = new List<int>();
		foreach (KeyValuePair<int, PoolItemTime> pair in this.objectList)
		{
			bool flag = null == pair.Value.gameObject;
			if (flag)
			{
				removeList.Add(pair.Key);
			}
		}
		removeList.ForEach(delegate(int key)
		{
			this.objectList.Remove(key);
		});
		return null == this.prefab;
	}

	// Token: 0x040000C5 RID: 197
	public string path;

	// Token: 0x040000C6 RID: 198
	public readonly GameObject prefab;

	// Token: 0x040000C7 RID: 199
	public Dictionary<int, PoolItemTime> objectList;
}
