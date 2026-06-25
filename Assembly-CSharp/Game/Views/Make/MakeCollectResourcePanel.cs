using System;
using System.Collections.Generic;
using GameData.Domains.Map;
using UnityEngine;

namespace Game.Views.Make
{
	// Token: 0x0200094F RID: 2383
	public class MakeCollectResourcePanel : MonoBehaviour
	{
		// Token: 0x06007090 RID: 28816 RVA: 0x00341DFC File Offset: 0x0033FFFC
		private void Awake()
		{
			for (int index = 0; index < this.ShowingResIconCacheList.Length; index++)
			{
				this.ShowingResIconCacheList[index] = new List<GameObject>();
			}
			this._resourceIconPool = new PoolItem("CollectResource_ResourceIcon", this.resourceIcon);
			this._itemIconPool = new PoolItem("CollectResource_ItemIcon", this.itemIcon);
			this.collectResourceItem.Init(this);
		}

		// Token: 0x06007091 RID: 28817 RVA: 0x00341E67 File Offset: 0x00340067
		private void OnDisable()
		{
			this.Clear();
		}

		// Token: 0x06007092 RID: 28818 RVA: 0x00341E74 File Offset: 0x00340074
		public void SetData(sbyte resourceType, int count, bool isAdd, int totalCount)
		{
			bool flag = !isAdd;
			if (flag)
			{
				List<GameObject> list = this.ShowingResIconCacheList[(int)resourceType];
				bool flag2 = list == null || list.Count == 0;
				if (!flag2)
				{
					int index = 0;
					foreach (GameObject item in list)
					{
						bool activeSelf = item.activeSelf;
						if (activeSelf)
						{
							index++;
						}
					}
					index = ((count > 0) ? Mathf.FloorToInt((float)(index / (count + 1))) : index);
					for (int i = list.Count - 1; i > -1; i--)
					{
						GameObject item2 = list[i];
						bool flag3 = item2.activeSelf && index > 0;
						if (flag3)
						{
							item2.gameObject.SetActive(false);
							index--;
						}
					}
				}
			}
			else
			{
				Vector2 localPosSelf = UIManager.Instance.MousePosToLocalPos(this.collectResourceItem.GetComponent<RectTransform>());
				this.collectResourceItem.SetIconHolderPosition(localPosSelf);
				CollectResourceResult collect = default(CollectResourceResult);
				collect.ResourceType = resourceType;
				collect.ResourceCount = (int)Math.Floor((double)count / (double)totalCount * 10.0);
				this.collectResourceItem.Set(collect);
				base.StartCoroutine(this.collectResourceItem.DropItem());
			}
		}

		// Token: 0x06007093 RID: 28819 RVA: 0x00341FE8 File Offset: 0x003401E8
		public GameObject GetDropIcon(bool isItem, sbyte type)
		{
			GameObject res;
			if (isItem)
			{
				res = this._itemIconPool.GetObject();
				this._showingItemIconCacheList.Add(res);
			}
			else
			{
				string resourceSprite = CommonUtils.GetResourceSpriteName(type, true);
				res = this._resourceIconPool.GetObject();
				res.GetComponent<CImage>().SetSprite(resourceSprite, false, null);
				bool flag = this.ShowingResIconCacheList.CheckIndex((int)type);
				if (flag)
				{
					this.ShowingResIconCacheList[(int)type].Add(res);
				}
				else
				{
					this._showingPracticeIconCacheList.Add(res);
				}
			}
			return res;
		}

		// Token: 0x06007094 RID: 28820 RVA: 0x00342074 File Offset: 0x00340274
		public void SetIsCollecting(bool value)
		{
			this._collecting = value;
		}

		// Token: 0x06007095 RID: 28821 RVA: 0x00342080 File Offset: 0x00340280
		public void Clear()
		{
			sbyte index = 0;
			while ((int)index < this.ShowingResIconCacheList.Length)
			{
				this.ClearResourceIcon(index);
				index += 1;
			}
		}

		// Token: 0x06007096 RID: 28822 RVA: 0x003420B0 File Offset: 0x003402B0
		private void ClearResourceIcon(sbyte resourceType)
		{
			List<GameObject> list = this.ShowingResIconCacheList[(int)resourceType];
			bool flag = list == null;
			if (!flag)
			{
				list.ForEach(new Action<GameObject>(this._resourceIconPool.DestroyObject));
				list.Clear();
			}
		}

		// Token: 0x04005387 RID: 21383
		[SerializeField]
		private MakeCollectResourceItem collectResourceItem;

		// Token: 0x04005388 RID: 21384
		[SerializeField]
		private GameObject resourceIcon;

		// Token: 0x04005389 RID: 21385
		[SerializeField]
		private GameObject itemIcon;

		// Token: 0x0400538A RID: 21386
		[NonSerialized]
		public readonly List<GameObject>[] ShowingResIconCacheList = new List<GameObject>[6];

		// Token: 0x0400538B RID: 21387
		private readonly List<GameObject> _showingItemIconCacheList = new List<GameObject>();

		// Token: 0x0400538C RID: 21388
		private readonly List<GameObject> _showingPracticeIconCacheList = new List<GameObject>();

		// Token: 0x0400538D RID: 21389
		private PoolItem _resourceIconPool;

		// Token: 0x0400538E RID: 21390
		private PoolItem _itemIconPool;

		// Token: 0x0400538F RID: 21391
		private bool _collecting;
	}
}
