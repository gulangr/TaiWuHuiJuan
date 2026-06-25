using System;
using System.Collections.Generic;
using CommonSortAndFilterLegacy;
using CommonSortAndFilterLegacy.Item;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameDataExtensions;
using UnityEngine;

// Token: 0x02000225 RID: 549
public abstract class BaseItemScrollView2 : Refers
{
	// Token: 0x1700036D RID: 877
	// (get) Token: 0x06002300 RID: 8960
	public abstract List<ItemDisplayData> OutputItemList { get; }

	// Token: 0x06002301 RID: 8961
	public abstract void Init(ESortAndFilterControllerType sortAndFilterControllerType = ESortAndFilterControllerType.Item, short[] columnSortIds = null);

	// Token: 0x06002302 RID: 8962
	public abstract void ReRender();

	// Token: 0x06002303 RID: 8963
	public abstract void SetItemList(ref List<ItemDisplayData> itemList);

	// Token: 0x1700036E RID: 878
	// (get) Token: 0x06002304 RID: 8964
	public abstract RectTransform ViewportRectTransform { get; }

	// Token: 0x1700036F RID: 879
	// (get) Token: 0x06002305 RID: 8965
	public abstract ItemSortAndFilterController SortAndFilterController { get; }

	// Token: 0x17000370 RID: 880
	// (get) Token: 0x06002306 RID: 8966
	public abstract CommonTableHead TableHead { get; }

	// Token: 0x06002307 RID: 8967 RVA: 0x001026D9 File Offset: 0x001008D9
	public virtual bool IsItemLockedByTask(ItemDisplayData itemData)
	{
		return this.checkQuestStatus && ItemDisplayDataHelper.IsItemLockedByTask(itemData);
	}

	// Token: 0x06002308 RID: 8968 RVA: 0x001026EC File Offset: 0x001008EC
	public void RefreshTableHeadTitle()
	{
		this.TableHead.RefreshTitle(BaseItemScrollView2.ItemTableHeadTitleKeyList);
	}

	// Token: 0x06002309 RID: 8969
	public abstract void SetItemToSelectCountMode(int index, CommonTableRowForItem itemView, Action<int> onConfirmSetCount, Action onCancelSetCount, int initSelectCount = 0, int limitCount = 0, int minCount = 1, string limitTip = null, bool keepSelectedOnHide = false, Action<int> onCountChange = null, Transform itemViewOriginParent = null, int itemViewOriginSibling = -1);

	// Token: 0x0600230A RID: 8970
	public abstract CommonTableRowForItem FindItemViewByItem(ItemKey key);

	// Token: 0x0600230B RID: 8971 RVA: 0x00102700 File Offset: 0x00100900
	public void SaveSortFilterSetting(bool saveGlobalSettings = true)
	{
	}

	// Token: 0x0600230C RID: 8972
	public abstract void HandleClickItem(ItemDisplayData itemData, CommonTableRowForItem itemView, Action<CommonTableRowForItem> onClick);

	// Token: 0x04001ADD RID: 6877
	[Tooltip("检查任务状态（以锁定部分特殊物品）")]
	public bool checkQuestStatus = true;

	// Token: 0x04001ADE RID: 6878
	protected string _listTag;

	// Token: 0x04001ADF RID: 6879
	protected static readonly List<LanguageKey> ItemTableHeadTitleKeyList = new List<LanguageKey>
	{
		LanguageKey.LK_ItemName,
		LanguageKey.LK_ItemType,
		LanguageKey.LK_Weight,
		LanguageKey.LK_ItemValue,
		LanguageKey.LK_Durability,
		LanguageKey.LK_Count,
		LanguageKey.LK_ReadingProgressStatus_Tips_Title
	};
}
