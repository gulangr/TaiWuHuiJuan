using System;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
	// Token: 0x02000FB3 RID: 4019
	[DisallowMultipleComponent]
	[AddComponentMenu("")]
	public class ResourceManager : MonoBehaviour
	{
		// Token: 0x170014EE RID: 5358
		// (get) Token: 0x0600B8D1 RID: 47313 RVA: 0x00543F08 File Offset: 0x00542108
		public static ResourceManager Instance
		{
			get
			{
				bool flag = ResourceManager.mInstance == null;
				if (flag)
				{
					ResourceManager.mInstance = new GameObject("ResourceManager", new Type[]
					{
						typeof(ResourceManager)
					})
					{
						transform = 
						{
							localPosition = new Vector3(9999999f, 9999999f, 9999999f)
						}
					}.GetComponent<ResourceManager>();
					bool isPlaying = Application.isPlaying;
					if (isPlaying)
					{
						Object.DontDestroyOnLoad(ResourceManager.mInstance.gameObject);
					}
					else
					{
						Debug.LogWarning("[ResourceManager] You'd better ignore ResourceManager in Editor mode");
					}
				}
				return ResourceManager.mInstance;
			}
		}

		// Token: 0x0600B8D2 RID: 47314 RVA: 0x00543FA4 File Offset: 0x005421A4
		public void InitPool(GameObject poolObject, int size, PoolInflationType type = PoolInflationType.DOUBLE)
		{
			bool flag = poolObject == null;
			if (flag)
			{
				Debug.LogError("[ResourceManager] Invalide prefab name for pooling");
			}
			else
			{
				bool flag2 = this.poolDict.ContainsKey(poolObject);
				if (!flag2)
				{
					this.poolDict[poolObject] = new Pool(poolObject, base.gameObject, size, type);
				}
			}
		}

		// Token: 0x0600B8D3 RID: 47315 RVA: 0x00543FFC File Offset: 0x005421FC
		public GameObject GetObjectFromPool(GameObject poolObject, bool autoActive = true, int autoCreate = 0)
		{
			GameObject result = null;
			bool flag = !this.poolDict.ContainsKey(poolObject) && autoCreate > 0;
			if (flag)
			{
				this.InitPool(poolObject, autoCreate, PoolInflationType.INCREMENT);
			}
			bool flag2 = this.poolDict.ContainsKey(poolObject);
			if (flag2)
			{
				Pool pool = this.poolDict[poolObject];
				result = pool.NextAvailableObject(autoActive);
			}
			return result;
		}

		// Token: 0x0600B8D4 RID: 47316 RVA: 0x00544060 File Offset: 0x00542260
		public void ReturnObjectToPool(GameObject go)
		{
			PoolObject po = go.GetComponent<PoolObject>();
			bool flag = po == null;
			if (!flag)
			{
				Pool pool = null;
				bool flag2 = this.poolDict.TryGetValue(po.poolObject, out pool);
				if (flag2)
				{
					pool.ReturnObjectToPool(po);
				}
			}
		}

		// Token: 0x0600B8D5 RID: 47317 RVA: 0x005440AC File Offset: 0x005422AC
		public void ReturnTransformToPool(Transform t)
		{
			bool flag = t == null;
			if (!flag)
			{
				this.ReturnObjectToPool(t.gameObject);
			}
		}

		// Token: 0x04008F4A RID: 36682
		private Dictionary<GameObject, Pool> poolDict = new Dictionary<GameObject, Pool>();

		// Token: 0x04008F4B RID: 36683
		private static ResourceManager mInstance;
	}
}
