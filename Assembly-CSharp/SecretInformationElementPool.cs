using System;
using UnityEngine;

// Token: 0x0200021D RID: 541
public class SecretInformationElementPool : MonoBehaviour
{
	// Token: 0x17000364 RID: 868
	// (get) Token: 0x06002292 RID: 8850 RVA: 0x001005D2 File Offset: 0x000FE7D2
	// (set) Token: 0x06002293 RID: 8851 RVA: 0x001005D9 File Offset: 0x000FE7D9
	public static SecretInformationElementPool Instance { get; private set; }

	// Token: 0x06002294 RID: 8852 RVA: 0x001005E1 File Offset: 0x000FE7E1
	private void Awake()
	{
		SecretInformationElementPool.Instance = this;
		this._maskAvatarPool = new PoolItem("SecretInformationElementPoolMaskAvatarPrefab", this.MaskAvatarPrefab);
		this._itemViewPool = new PoolItem("SecretInformationElementPoolItemViewPrefab", this.ItemViewPrefab);
	}

	// Token: 0x06002295 RID: 8853 RVA: 0x00100618 File Offset: 0x000FE818
	public GameObject GetMaskedAvatar()
	{
		return this._maskAvatarPool.GetObject();
	}

	// Token: 0x06002296 RID: 8854 RVA: 0x00100638 File Offset: 0x000FE838
	public void DestroyMaskedAvatar(GameObject obj)
	{
		bool flag = null == obj;
		if (!flag)
		{
			this._maskAvatarPool.DestroyObject(obj);
		}
	}

	// Token: 0x06002297 RID: 8855 RVA: 0x00100660 File Offset: 0x000FE860
	public GameObject GetItemView()
	{
		return this._itemViewPool.GetObject();
	}

	// Token: 0x06002298 RID: 8856 RVA: 0x00100680 File Offset: 0x000FE880
	public void DestroyItemView(GameObject obj)
	{
		bool flag = null == obj;
		if (!flag)
		{
			this._itemViewPool.DestroyObject(obj);
		}
	}

	// Token: 0x04001A96 RID: 6806
	public GameObject MaskAvatarPrefab;

	// Token: 0x04001A97 RID: 6807
	public GameObject ItemViewPrefab;

	// Token: 0x04001A99 RID: 6809
	private PoolItem _maskAvatarPool;

	// Token: 0x04001A9A RID: 6810
	private PoolItem _itemViewPool;
}
