using System;
using SG;

namespace UnityEngine.UI
{
	// Token: 0x02000FA7 RID: 4007
	[Serializable]
	public class LoopScrollPrefabSource
	{
		// Token: 0x0600B7D5 RID: 47061 RVA: 0x0053D020 File Offset: 0x0053B220
		public virtual GameObject GetObject()
		{
			bool flag = !this.inited;
			if (flag)
			{
				ResourceManager.Instance.InitPool(this.prefab, this.poolSize, PoolInflationType.DOUBLE);
				this.inited = true;
			}
			return ResourceManager.Instance.GetObjectFromPool(this.prefab, true, 0);
		}

		// Token: 0x0600B7D6 RID: 47062 RVA: 0x0053D072 File Offset: 0x0053B272
		public virtual void ReturnObject(Transform go)
		{
			ResourceManager.Instance.ReturnObjectToPool(go.gameObject);
			Action<Transform> action = this.callback;
			if (action != null)
			{
				action(go);
			}
		}

		// Token: 0x04008EDC RID: 36572
		public GameObject prefab;

		// Token: 0x04008EDD RID: 36573
		public Action<Transform> callback;

		// Token: 0x04008EDE RID: 36574
		public int poolSize = 5;

		// Token: 0x04008EDF RID: 36575
		private bool inited = false;
	}
}
