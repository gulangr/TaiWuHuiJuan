using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using Game.Views.Legacy.UICommon.SortAndFilter;
using TMPro;
using UnityEngine;

// Token: 0x02000358 RID: 856
public class SimpleAbstractSort : MonoBehaviour
{
	// Token: 0x1700056B RID: 1387
	// (get) Token: 0x060031D9 RID: 12761 RVA: 0x00189EC8 File Offset: 0x001880C8
	public List<SimpleAbstractSort.Sort> Sorts
	{
		get
		{
			return this._sorts;
		}
	}

	// Token: 0x1700056C RID: 1388
	// (get) Token: 0x060031DA RID: 12762 RVA: 0x00189ED0 File Offset: 0x001880D0
	public bool IsAnySortActive
	{
		get
		{
			return this._sorts.Count > 0;
		}
	}

	// Token: 0x060031DB RID: 12763 RVA: 0x00189EE0 File Offset: 0x001880E0
	public void Init(SimpleAbstractSort.Config config)
	{
		this._config = config;
		this.GenerateSortItems();
		this.LoadSorts();
	}

	// Token: 0x060031DC RID: 12764 RVA: 0x00189EF8 File Offset: 0x001880F8
	public void SaveSorts()
	{
		bool flag = this._config.SaveKey.IsNullOrEmpty();
		if (!flag)
		{
			SingletonObject.getInstance<GameSort>().SetSimpleAbstractSortConfig(this._config.SaveKey, this._sorts);
		}
	}

	// Token: 0x060031DD RID: 12765 RVA: 0x00189F38 File Offset: 0x00188138
	private void LoadSorts()
	{
		bool flag = this._config.SaveKey.IsNullOrEmpty();
		if (!flag)
		{
			this._sorts.Clear();
			List<SimpleAbstractSort.Sort> simpleAbstractSortConfig = SingletonObject.getInstance<GameSort>().GetSimpleAbstractSortConfig(this._config.SaveKey);
			bool flag2 = simpleAbstractSortConfig == null;
			if (!flag2)
			{
				this._sorts.AddRange(simpleAbstractSortConfig);
				this.OnSortChange();
			}
		}
	}

	// Token: 0x060031DE RID: 12766 RVA: 0x00189F9C File Offset: 0x0018819C
	public int Compare(SimpleAbstractSort.ComparableData left, SimpleAbstractSort.ComparableData right)
	{
		foreach (SimpleAbstractSort.Sort sort in this._sorts)
		{
			int sortId = sort.Id;
			int compareResult = left.SortIdToValueDict[sortId].CompareTo(right.SortIdToValueDict[sortId]);
			bool flag = compareResult != 0;
			if (flag)
			{
				return sort.IsAscending ? compareResult : (-compareResult);
			}
		}
		return 0;
	}

	// Token: 0x060031DF RID: 12767 RVA: 0x0018A038 File Offset: 0x00188238
	private void GenerateSortItems()
	{
		this._sorts.Clear();
		CommonUtils.PrepareEnoughChildren(base.transform, this.itemTemplate.gameObject, this._config.ItemConfigs.Count, null);
		for (int i = 0; i < this._config.ItemConfigs.Count; i++)
		{
			Transform child = base.transform.GetChild(i);
			child.gameObject.name = i.ToString();
			SimpleAbstractSortItemTemplate refers = child.GetComponent<SimpleAbstractSortItemTemplate>();
			this.RefreshSortItem(refers, this._config.ItemConfigs[i].Id);
		}
	}

	// Token: 0x060031E0 RID: 12768 RVA: 0x0018A0EC File Offset: 0x001882EC
	private void RefreshSortItems()
	{
		for (int i = 0; i < this._config.ItemConfigs.Count; i++)
		{
			Transform child = base.transform.GetChild(i);
			SimpleAbstractSortItemTemplate refers = child.GetComponent<SimpleAbstractSortItemTemplate>();
			this.RefreshSortItem(refers, this._config.ItemConfigs[i].Id);
		}
	}

	// Token: 0x060031E1 RID: 12769 RVA: 0x0018A150 File Offset: 0x00188350
	private void RefreshSortItem(SimpleAbstractSortItemTemplate refers, int sortId)
	{
		GameObject checkMark = refers.checkMark;
		GameObject upArrow = refers.upArrow;
		GameObject downArrow = refers.downArrow;
		TextMeshProUGUI nameTxt = refers.nameTxt;
		TextMeshProUGUI index = refers.indexTxt;
		GameObject indexBg = refers.indexBg;
		nameTxt.text = this._config.ItemConfigs[sortId].Text;
		int sortIndex = this._sorts.FindIndex((SimpleAbstractSort.Sort s) => s.Id == sortId);
		bool flag = sortIndex != -1;
		if (flag)
		{
			SimpleAbstractSort.Sort sort = this._sorts[sortIndex];
			upArrow.SetActive(sort.IsAscending);
			downArrow.SetActive(!sort.IsAscending);
			indexBg.SetActive(true);
			checkMark.SetActive(true);
			index.text = (sortIndex + 1).ToString();
		}
		else
		{
			upArrow.SetActive(false);
			downArrow.SetActive(false);
			indexBg.SetActive(false);
			checkMark.SetActive(false);
		}
		refers.GetComponent<CButton>().ClearAndAddListener(delegate
		{
			this.OnItemButtonClick(sortId);
		});
	}

	// Token: 0x060031E2 RID: 12770 RVA: 0x0018A27C File Offset: 0x0018847C
	private void OnItemButtonClick(int sortId)
	{
		int findIndex = this._sorts.FindIndex((SimpleAbstractSort.Sort s) => s.Id == sortId);
		bool flag = findIndex == -1;
		if (flag)
		{
			this._sorts.Add(new SimpleAbstractSort.Sort
			{
				Id = sortId,
				IsAscending = false
			});
			this.OnSortChange();
		}
		else
		{
			SimpleAbstractSort.Sort sort = this._sorts[findIndex];
			bool isAscending = sort.IsAscending;
			if (isAscending)
			{
				this._sorts.Remove(sort);
			}
			else
			{
				sort.IsAscending = true;
			}
			this.OnSortChange();
		}
	}

	// Token: 0x060031E3 RID: 12771 RVA: 0x0018A31F File Offset: 0x0018851F
	private void OnSortChange()
	{
		this.RefreshSortItems();
		Action onSortChanged = this._config.OnSortChanged;
		if (onSortChanged != null)
		{
			onSortChanged();
		}
		this.SaveSorts();
	}

	// Token: 0x060031E4 RID: 12772 RVA: 0x0018A347 File Offset: 0x00188547
	public void SortList<T>(List<T> targetList)
	{
	}

	// Token: 0x04002491 RID: 9361
	private readonly List<SimpleAbstractSort.Sort> _sorts = new List<SimpleAbstractSort.Sort>();

	// Token: 0x04002492 RID: 9362
	private SimpleAbstractSort.Config _config;

	// Token: 0x04002493 RID: 9363
	public SimpleAbstractSortItemTemplate itemTemplate;

	// Token: 0x02001700 RID: 5888
	public struct ItemConfig
	{
		// Token: 0x0400AA07 RID: 43527
		public int Id;

		// Token: 0x0400AA08 RID: 43528
		public string Text;
	}

	// Token: 0x02001701 RID: 5889
	public struct Config
	{
		// Token: 0x0600D306 RID: 54022 RVA: 0x005B0F40 File Offset: 0x005AF140
		public Config(List<SimpleAbstractSort.ItemConfig> itemConfigs, Action onSortChanged, string saveKey)
		{
			this.ItemConfigs = itemConfigs;
			this.OnSortChanged = onSortChanged;
			this.SaveKey = saveKey;
		}

		// Token: 0x0400AA09 RID: 43529
		public List<SimpleAbstractSort.ItemConfig> ItemConfigs;

		// Token: 0x0400AA0A RID: 43530
		public Action OnSortChanged;

		// Token: 0x0400AA0B RID: 43531
		public string SaveKey;
	}

	// Token: 0x02001702 RID: 5890
	public struct ComparableData
	{
		// Token: 0x0600D307 RID: 54023 RVA: 0x005B0F58 File Offset: 0x005AF158
		public ComparableData(Dictionary<int, int> sortIdToValueDict)
		{
			this.SortIdToValueDict = sortIdToValueDict;
		}

		// Token: 0x0400AA0C RID: 43532
		public Dictionary<int, int> SortIdToValueDict;
	}

	// Token: 0x02001703 RID: 5891
	public class Sort
	{
		// Token: 0x0400AA0D RID: 43533
		public int Id;

		// Token: 0x0400AA0E RID: 43534
		public bool IsAscending;
	}
}
