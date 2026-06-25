using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200002F RID: 47
public class PoolManager
{
	// Token: 0x060001AD RID: 429 RVA: 0x0000AFC0 File Offset: 0x000091C0
	private static void AddData(string path, GameObject prefab)
	{
		bool flag = PoolManager.itemList == null;
		if (flag)
		{
			PoolManager.itemList = new Dictionary<string, PoolItem>();
			bool isPlaying = Application.isPlaying;
			if (isPlaying)
			{
				SingletonObject.getInstance<YieldHelper>().StartYield(PoolManager.CoExpireObject());
			}
		}
		bool flag2 = !PoolManager.itemList.ContainsKey(path);
		if (flag2)
		{
			PoolManager.itemList.Add(path, new PoolItem(path, prefab));
		}
	}

	// Token: 0x060001AE RID: 430 RVA: 0x0000B028 File Offset: 0x00009228
	public static bool HasData(string path)
	{
		bool flag = PoolManager.itemList == null;
		return !flag && PoolManager.itemList.ContainsKey(path);
	}

	// Token: 0x060001AF RID: 431 RVA: 0x0000B058 File Offset: 0x00009258
	public static void SetSrcObject(string key, GameObject gameObject)
	{
		bool flag = PoolManager.itemList == null || !PoolManager.itemList.ContainsKey(key);
		if (flag)
		{
			PoolManager.AddData(key, gameObject);
		}
	}

	// Token: 0x060001B0 RID: 432 RVA: 0x0000B08A File Offset: 0x0000928A
	public static void SetSrcObjectWithTurnOff(string key, GameObject gameObject)
	{
		PoolManager.SetSrcObject(key, gameObject);
		gameObject.SetActive(false);
	}

	// Token: 0x060001B1 RID: 433 RVA: 0x0000B0A0 File Offset: 0x000092A0
	public static void RemoveData(string key)
	{
		bool flag = PoolManager.itemList == null;
		if (!flag)
		{
			bool flag2 = PoolManager.itemList.ContainsKey(key);
			if (flag2)
			{
				PoolManager.itemList[key].Destroy();
			}
			PoolManager.itemList.Remove(key);
		}
	}

	// Token: 0x060001B2 RID: 434 RVA: 0x0000B0E8 File Offset: 0x000092E8
	public static void RemoveObject(string path, GameObject gameObject)
	{
		bool flag = PoolManager.itemList == null || !PoolManager.itemList.ContainsKey(path);
		if (!flag)
		{
			PoolManager.itemList[path].RemoveObject(gameObject);
		}
	}

	// Token: 0x060001B3 RID: 435 RVA: 0x0000B128 File Offset: 0x00009328
	public static GameObject GetObject(string path)
	{
		bool flag = PoolManager.itemList == null || !PoolManager.itemList.ContainsKey(path);
		GameObject result;
		if (flag)
		{
			result = null;
		}
		else
		{
			GameObject obj = PoolManager.itemList[path].GetObject();
			bool flag2 = null == obj;
			if (flag2)
			{
				PoolManager.RemoveNullElems();
			}
			result = obj;
		}
		return result;
	}

	// Token: 0x060001B4 RID: 436 RVA: 0x0000B180 File Offset: 0x00009380
	public static T GetObject<T>(string path) where T : Component
	{
		GameObject o = PoolManager.GetObject(path);
		bool flag = null != o;
		T result;
		if (flag)
		{
			result = o.GetComponent<T>();
		}
		else
		{
			PoolManager.RemoveNullElems();
			result = default(T);
		}
		return result;
	}

	// Token: 0x060001B5 RID: 437 RVA: 0x0000B1BC File Offset: 0x000093BC
	public static void Destroy(string path, GameObject gameObject)
	{
		bool flag = PoolManager.itemList == null || !PoolManager.itemList.ContainsKey(path);
		if (!flag)
		{
			PoolManager.itemList[path].DestroyObject(gameObject);
		}
	}

	// Token: 0x060001B6 RID: 438 RVA: 0x0000B1FC File Offset: 0x000093FC
	public static void ExpireObject()
	{
		bool flag = PoolManager.itemList == null;
		if (!flag)
		{
			foreach (PoolItem poolItem in PoolManager.itemList.Values)
			{
				poolItem.ExpireObject();
			}
		}
	}

	// Token: 0x060001B7 RID: 439 RVA: 0x0000B268 File Offset: 0x00009468
	public static void DisposePool()
	{
		foreach (PoolItem poolItem in PoolManager.itemList.Values)
		{
			poolItem.Destroy();
		}
		PoolManager.itemList = null;
	}

	// Token: 0x060001B8 RID: 440 RVA: 0x0000B2CC File Offset: 0x000094CC
	public static void LogAllItemPool()
	{
		List<string> keyList = new List<string>();
		keyList.AddRange(PoolManager.itemList.Keys);
		Debug.Log(string.Join("\n", keyList));
	}

	// Token: 0x060001B9 RID: 441 RVA: 0x0000B304 File Offset: 0x00009504
	public static void CleanPool()
	{
		bool flag = PoolManager.itemList == null || GameApp.Quiting;
		if (!flag)
		{
			List<string> toRemoveList = new List<string>();
			foreach (PoolItem poolItem in PoolManager.itemList.Values)
			{
				bool flag2 = null == poolItem.prefab;
				if (flag2)
				{
					toRemoveList.Add(poolItem.path);
				}
			}
			toRemoveList.ForEach(new Action<string>(PoolManager.RemoveData));
		}
	}

	// Token: 0x060001BA RID: 442 RVA: 0x0000B3A8 File Offset: 0x000095A8
	public static IEnumerator CoExpireObject()
	{
		for (;;)
		{
			yield return PoolManager.waitExpire;
			bool flag = PoolManager.itemList == null;
			if (flag)
			{
				break;
			}
			PoolManager.ExpireObject();
		}
		yield break;
	}

	// Token: 0x060001BB RID: 443 RVA: 0x0000B3B0 File Offset: 0x000095B0
	private static void RemoveNullElems()
	{
		bool flag = PoolManager.itemList == null;
		if (!flag)
		{
			List<string> removeList = new List<string>();
			foreach (KeyValuePair<string, PoolItem> pair in PoolManager.itemList)
			{
				bool flag2 = pair.Value.RemoveNullElems();
				if (flag2)
				{
					removeList.Add(pair.Key);
				}
			}
			removeList.ForEach(new Action<string>(PoolManager.RemoveData));
		}
	}

	// Token: 0x040000CB RID: 203
	public const int EXPIRE_TIME = 10;

	// Token: 0x040000CC RID: 204
	private static Dictionary<string, PoolItem> itemList;

	// Token: 0x040000CD RID: 205
	private static WaitForSeconds waitExpire = new WaitForSeconds(5f);
}
