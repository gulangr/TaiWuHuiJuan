using System;
using System.Collections.Generic;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using UnityEngine;

// Token: 0x02000224 RID: 548
public abstract class BaseItemScrollView : Refers
{
	// Token: 0x060022F6 RID: 8950
	public abstract void Init();

	// Token: 0x060022F7 RID: 8951
	public abstract void ReRender();

	// Token: 0x060022F8 RID: 8952
	public abstract void SetItemList(ref List<ItemDisplayData> itemList);

	// Token: 0x1700036B RID: 875
	// (get) Token: 0x060022F9 RID: 8953
	public abstract ItemSortAndFilter SortAndFilter { get; }

	// Token: 0x1700036C RID: 876
	// (get) Token: 0x060022FA RID: 8954
	public abstract RectTransform ViewportRectTransform { get; }

	// Token: 0x060022FB RID: 8955
	public abstract void SetItemToSelectCountMode(int index, ItemView itemView, Action<int> onConfirmSetCount, Action onCancelSetCount, int initSelectCount = 0, int limitCount = 0, int minCount = 1, string limitTip = null, bool keepSelectedOnHide = false, Action<int> onCountChange = null, Transform itemViewOriginParent = null, int itemViewOriginSibling = -1);

	// Token: 0x060022FC RID: 8956
	public abstract ItemView FindItemViewByItem(ItemKey key);

	// Token: 0x060022FD RID: 8957 RVA: 0x00102690 File Offset: 0x00100890
	public void SaveSortFilterSetting(bool saveGlobalSettings = true)
	{
		bool flag = this._listTag != null;
		if (flag)
		{
			SingletonObject.getInstance<GameSort>().SetItemSortConfig(this._listTag, this.SortAndFilter.SortFilterSetting);
		}
	}

	// Token: 0x060022FE RID: 8958
	public abstract void HandleClickItem(ItemDisplayData itemData, ItemView itemView, Action<ItemView> onClick);

	// Token: 0x04001AD7 RID: 6871
	[Tooltip("简略视图每行道具数")]
	public int SimpleViewLineCount;

	// Token: 0x04001AD8 RID: 6872
	[Tooltip("详细视图每行道具数")]
	public int DetailViewLineCount;

	// Token: 0x04001AD9 RID: 6873
	[Tooltip("简略视图道具排列间距")]
	public Vector2 SimpleViewGap;

	// Token: 0x04001ADA RID: 6874
	[Tooltip("详细视图道具排列间距")]
	public Vector2 DetailViewGap;

	// Token: 0x04001ADB RID: 6875
	[Tooltip("初始的详略")]
	public bool IsDetailView = true;

	// Token: 0x04001ADC RID: 6876
	protected string _listTag;
}
