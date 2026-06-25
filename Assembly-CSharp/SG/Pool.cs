using System;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
	// Token: 0x02000FB2 RID: 4018
	internal class Pool
	{
		// Token: 0x0600B8CC RID: 47308 RVA: 0x00543C58 File Offset: 0x00541E58
		public Pool(GameObject poolObject, GameObject rootPoolObj, int initialCount, PoolInflationType type)
		{
			bool flag = poolObject == null;
			if (!flag)
			{
				this.poolObject = poolObject;
				this.inflationType = type;
				this.rootObj = new GameObject(poolObject.name + "Pool");
				this.rootObj.transform.SetParent(rootPoolObj.transform, false);
				GameObject go = Object.Instantiate<GameObject>(poolObject);
				PoolObject po = go.GetComponent<PoolObject>();
				bool flag2 = po == null;
				if (flag2)
				{
					po = go.AddComponent<PoolObject>();
				}
				po.poolObject = poolObject;
				this.AddObjectToPool(po);
				this.populatePool(Mathf.Max(initialCount, 1));
			}
		}

		// Token: 0x0600B8CD RID: 47309 RVA: 0x00543D14 File Offset: 0x00541F14
		private void AddObjectToPool(PoolObject po)
		{
			po.gameObject.SetActive(false);
			po.gameObject.name = this.poolObject.name;
			this.availableObjStack.Push(po);
			po.isPooled = true;
			po.gameObject.transform.SetParent(this.rootObj.transform, false);
		}

		// Token: 0x0600B8CE RID: 47310 RVA: 0x00543D78 File Offset: 0x00541F78
		private void populatePool(int initialCount)
		{
			for (int index = 0; index < initialCount; index++)
			{
				PoolObject po = Object.Instantiate<PoolObject>(this.availableObjStack.Peek());
				this.AddObjectToPool(po);
			}
		}

		// Token: 0x0600B8CF RID: 47311 RVA: 0x00543DB4 File Offset: 0x00541FB4
		public GameObject NextAvailableObject(bool autoActive)
		{
			PoolObject po = null;
			bool flag = this.availableObjStack.Count > 1;
			if (flag)
			{
				po = this.availableObjStack.Pop();
			}
			else
			{
				int increaseSize = 0;
				bool flag2 = this.inflationType == PoolInflationType.INCREMENT;
				if (flag2)
				{
					increaseSize = 1;
				}
				else
				{
					bool flag3 = this.inflationType == PoolInflationType.DOUBLE;
					if (flag3)
					{
						increaseSize = this.availableObjStack.Count + Mathf.Max(this.objectsInUse, 0);
					}
				}
				bool flag4 = increaseSize > 0;
				if (flag4)
				{
					this.populatePool(increaseSize);
					po = this.availableObjStack.Pop();
				}
			}
			GameObject result = null;
			bool flag5 = po != null;
			if (flag5)
			{
				this.objectsInUse++;
				po.isPooled = false;
				result = po.gameObject;
				if (autoActive)
				{
					result.SetActive(true);
				}
			}
			return result;
		}

		// Token: 0x0600B8D0 RID: 47312 RVA: 0x00543E90 File Offset: 0x00542090
		public void ReturnObjectToPool(PoolObject po)
		{
			bool flag = this.poolObject.Equals(po.poolObject);
			if (flag)
			{
				this.objectsInUse--;
				bool isPooled = po.isPooled;
				if (!isPooled)
				{
					this.AddObjectToPool(po);
				}
			}
			else
			{
				Debug.LogError(string.Format("Trying to add object to incorrect pool {0} {1}", po.poolObject.name, this.poolObject.name));
			}
		}

		// Token: 0x04008F45 RID: 36677
		private Stack<PoolObject> availableObjStack = new Stack<PoolObject>();

		// Token: 0x04008F46 RID: 36678
		private GameObject rootObj;

		// Token: 0x04008F47 RID: 36679
		private GameObject poolObject;

		// Token: 0x04008F48 RID: 36680
		private PoolInflationType inflationType;

		// Token: 0x04008F49 RID: 36681
		private int objectsInUse = 0;
	}
}
