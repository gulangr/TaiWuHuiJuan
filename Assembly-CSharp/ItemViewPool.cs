using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000230 RID: 560
public class ItemViewPool : MonoBehaviour, ISingletonInit, IDisposable
{
	// Token: 0x060023E3 RID: 9187 RVA: 0x00107888 File Offset: 0x00105A88
	public T Get<T>() where T : MonoBehaviour
	{
		Type type = typeof(T);
		PoolItem poolItem;
		return this._poolItemDict.TryGetValue(type.Name, out poolItem) ? poolItem.GetObject().GetComponent<T>() : default(T);
	}

	// Token: 0x060023E4 RID: 9188 RVA: 0x001078D0 File Offset: 0x00105AD0
	public void Return<T>(T view) where T : MonoBehaviour
	{
		Type type = typeof(T);
		PoolItem poolItem;
		bool flag = this._poolItemDict.TryGetValue(type.Name, out poolItem);
		if (flag)
		{
			poolItem.DestroyObject(view.gameObject);
			view.transform.SetParent(base.transform);
		}
	}

	// Token: 0x060023E5 RID: 9189 RVA: 0x0010792C File Offset: 0x00105B2C
	public void Dispose()
	{
		foreach (KeyValuePair<string, PoolItem> keyValuePair in this._poolItemDict)
		{
			string text;
			PoolItem poolItem2;
			keyValuePair.Deconstruct(out text, out poolItem2);
			PoolItem poolItem = poolItem2;
			poolItem.Destroy();
		}
		this._poolItemDict.Clear();
	}

	// Token: 0x060023E6 RID: 9190 RVA: 0x001079A4 File Offset: 0x00105BA4
	public void Init()
	{
		string cricketViewName = "CricketViewNew";
		bool flag = !this._poolItemDict.ContainsKey(cricketViewName);
		if (flag)
		{
			ResLoader.Load<GameObject>("RemakeResources/Prefab/Legacy/Core/Cricket/CricketViewNew", delegate(GameObject go)
			{
				this._poolItemDict[cricketViewName] = new PoolItem(cricketViewName, go);
			}, null, false);
		}
		bool flag2 = SingletonObject.getInstance<DlcManager>().IsDlcInstalled(DlcManager.DlcIdFiveLoong);
		if (flag2)
		{
			string jiaoEggViewName = "JiaoEggView";
			bool flag3 = !this._poolItemDict.ContainsKey(jiaoEggViewName);
			if (flag3)
			{
				ResLoader.LoadByName<GameObject>("JiaoEggView", delegate(GameObject go)
				{
					this._poolItemDict[jiaoEggViewName] = new PoolItem(jiaoEggViewName, go);
				}, null);
			}
		}
	}

	// Token: 0x04001B38 RID: 6968
	private readonly Dictionary<string, PoolItem> _poolItemDict = new Dictionary<string, PoolItem>();
}
