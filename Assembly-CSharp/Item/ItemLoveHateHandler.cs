using System;
using UnityEngine;

namespace Item
{
	// Token: 0x02000604 RID: 1540
	[RequireComponent(typeof(ItemView))]
	public class ItemLoveHateHandler : MonoBehaviour
	{
		// Token: 0x06004890 RID: 18576 RVA: 0x0021F468 File Offset: 0x0021D668
		public void Refresh(bool love, bool hate)
		{
			if (love && this._loveMarkInstance == null)
			{
				this._loveMarkInstance = this.CreateMarkInstance(this.LovePrefab, this.LovePrefabOffset);
			}
			bool flag = null != this._loveMarkInstance;
			if (flag)
			{
				this._loveMarkInstance.SetActive(love);
			}
			if (hate && this._hateMarkInstance == null)
			{
				this._hateMarkInstance = this.CreateMarkInstance(this.HatePrefab, this.HatePrefabOffset);
			}
			bool flag2 = null != this._hateMarkInstance;
			if (flag2)
			{
				this._hateMarkInstance.SetActive(hate);
			}
		}

		// Token: 0x06004891 RID: 18577 RVA: 0x0021F504 File Offset: 0x0021D704
		private GameObject CreateMarkInstance(GameObject srcPrefab, Vector3 localPos)
		{
			GameObject instance = Object.Instantiate<GameObject>(srcPrefab, base.transform, false);
			instance.transform.localPosition = localPos;
			return instance;
		}

		// Token: 0x04003213 RID: 12819
		public Vector2 LovePrefabOffset;

		// Token: 0x04003214 RID: 12820
		public Vector2 HatePrefabOffset;

		// Token: 0x04003215 RID: 12821
		public GameObject LovePrefab;

		// Token: 0x04003216 RID: 12822
		public GameObject HatePrefab;

		// Token: 0x04003217 RID: 12823
		private GameObject _loveMarkInstance;

		// Token: 0x04003218 RID: 12824
		private GameObject _hateMarkInstance;
	}
}
