using System;
using System.Collections.Generic;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x0200035A RID: 858
public class SimpleItemSort : Refers
{
	// Token: 0x060031F1 RID: 12785 RVA: 0x0018A66C File Offset: 0x0018886C
	public void Init()
	{
		this.InitRefers();
		SimpleItemSort.ItemOrder.Clear();
		SimpleItemSort.ItemOrder.Add(this._name);
		SimpleItemSort.ItemOrder.Add(this._grade);
		SimpleItemSort.ItemOrder.Add(this._value);
		SimpleItemSort.ItemOrder.Add(this._weight);
		for (int index = 0; index < SimpleItemSort.ItemOrder.Count; index++)
		{
			Refers item = SimpleItemSort.ItemOrder[index];
			CButtonObsolete button = item.GetComponent<CButtonObsolete>();
			int ii = index;
			button.ClearAndAddListener(delegate
			{
				this.OnItemButtonClick(ii);
			});
		}
	}

	// Token: 0x060031F2 RID: 12786 RVA: 0x0018A727 File Offset: 0x00188927
	public void SetCallback(Action onSortChanged)
	{
		this._onSortChanged = onSortChanged;
	}

	// Token: 0x060031F3 RID: 12787 RVA: 0x0018A734 File Offset: 0x00188934
	public int CompareItem(SimpleItemSort.ItemCompareItem itemL, SimpleItemSort.ItemCompareItem itemR)
	{
		foreach (SimpleItemSort.Sort sort in this._sorts)
		{
			switch (sort.Type)
			{
			case SimpleItemSort.SortType.Name:
			{
				int nameCompare = Utils_Sorting.CompareByCurrentLangEncoding(itemL.Name, itemR.Name);
				bool flag = nameCompare != 0;
				if (flag)
				{
					return sort.IsAscending ? nameCompare : (-nameCompare);
				}
				break;
			}
			case SimpleItemSort.SortType.Grade:
			{
				bool flag2 = itemL.Grade != itemR.Grade;
				if (flag2)
				{
					return (int)(sort.IsAscending ? (itemL.Grade - itemR.Grade) : (itemR.Grade - itemL.Grade));
				}
				break;
			}
			case SimpleItemSort.SortType.Value:
			{
				bool flag3 = itemL.Value != itemR.Value;
				if (flag3)
				{
					return sort.IsAscending ? (itemL.Value - itemR.Value) : (itemR.Value - itemL.Value);
				}
				break;
			}
			case SimpleItemSort.SortType.Weight:
			{
				bool flag4 = itemL.Weight != itemR.Weight;
				if (flag4)
				{
					return sort.IsAscending ? (itemL.Weight - itemR.Weight) : (itemR.Weight - itemL.Weight);
				}
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		return 0;
	}

	// Token: 0x060031F4 RID: 12788 RVA: 0x0018A8C8 File Offset: 0x00188AC8
	private void OnItemButtonClick(int index)
	{
		int findIndex = this._sorts.FindIndex((SimpleItemSort.Sort s) => s.Type == SimpleItemSort.SortTypeOrder[index]);
		bool flag = findIndex == -1;
		if (flag)
		{
			this._sorts.Add(new SimpleItemSort.Sort
			{
				Type = SimpleItemSort.SortTypeOrder[index],
				IsAscending = false
			});
			this.RefreshSortItems();
			Action onSortChanged = this._onSortChanged;
			if (onSortChanged != null)
			{
				onSortChanged();
			}
		}
		else
		{
			SimpleItemSort.Sort sort = this._sorts[findIndex];
			bool isAscending = sort.IsAscending;
			if (isAscending)
			{
				this._sorts.Remove(sort);
			}
			else
			{
				sort.IsAscending = true;
			}
			this.RefreshSortItems();
			Action onSortChanged2 = this._onSortChanged;
			if (onSortChanged2 != null)
			{
				onSortChanged2();
			}
		}
	}

	// Token: 0x060031F5 RID: 12789 RVA: 0x0018A998 File Offset: 0x00188B98
	private void RefreshSortItems()
	{
		int i;
		int j;
		for (i = 0; i < SimpleItemSort.SortTypeOrder.Length; i = j + 1)
		{
			int sortIndex = this._sorts.FindIndex((SimpleItemSort.Sort s) => s.Type == SimpleItemSort.SortTypeOrder[i]);
			bool flag = sortIndex != -1;
			if (flag)
			{
				this.RefreshSortItem(SimpleItemSort.ItemOrder[i], sortIndex, this._sorts[sortIndex]);
			}
			else
			{
				this.RefreshSortItem(SimpleItemSort.ItemOrder[i], -1, null);
			}
			j = i;
		}
	}

	// Token: 0x060031F6 RID: 12790 RVA: 0x0018AA44 File Offset: 0x00188C44
	private void RefreshSortItem(Refers item, int i, SimpleItemSort.Sort sort)
	{
		GameObject checkMark = item.CGet<GameObject>("CheckMark");
		GameObject upArrow = item.CGet<GameObject>("UpArrow");
		GameObject downArrow = item.CGet<GameObject>("DownArrow");
		TextMeshProUGUI index = item.CGet<TextMeshProUGUI>("Index");
		GameObject indexBg = item.CGet<GameObject>("IndexBg");
		bool flag = sort != null;
		if (flag)
		{
			upArrow.SetActive(sort.IsAscending);
			downArrow.SetActive(!sort.IsAscending);
			indexBg.SetActive(true);
			checkMark.SetActive(true);
			index.text = (i + 1).ToString();
		}
		else
		{
			upArrow.SetActive(false);
			downArrow.SetActive(false);
			indexBg.SetActive(false);
			checkMark.SetActive(false);
		}
	}

	// Token: 0x060031F7 RID: 12791 RVA: 0x0018AB04 File Offset: 0x00188D04
	private void InitRefers()
	{
		this._name = base.CGet<Refers>("Name");
		this._grade = base.CGet<Refers>("Grade");
		this._value = base.CGet<Refers>("Value");
		this._weight = base.CGet<Refers>("Weight");
	}

	// Token: 0x040024A0 RID: 9376
	private Action _onSortChanged;

	// Token: 0x040024A1 RID: 9377
	private List<SimpleItemSort.Sort> _sorts = new List<SimpleItemSort.Sort>();

	// Token: 0x040024A2 RID: 9378
	private static readonly SimpleItemSort.SortType[] SortTypeOrder = new SimpleItemSort.SortType[]
	{
		SimpleItemSort.SortType.Name,
		SimpleItemSort.SortType.Grade,
		SimpleItemSort.SortType.Value,
		SimpleItemSort.SortType.Weight
	};

	// Token: 0x040024A3 RID: 9379
	private static readonly List<Refers> ItemOrder = new List<Refers>();

	// Token: 0x040024A4 RID: 9380
	private Refers _name;

	// Token: 0x040024A5 RID: 9381
	private Refers _grade;

	// Token: 0x040024A6 RID: 9382
	private Refers _value;

	// Token: 0x040024A7 RID: 9383
	private Refers _weight;

	// Token: 0x02001708 RID: 5896
	public enum SortType
	{
		// Token: 0x0400AA1F RID: 43551
		Name,
		// Token: 0x0400AA20 RID: 43552
		Grade,
		// Token: 0x0400AA21 RID: 43553
		Value,
		// Token: 0x0400AA22 RID: 43554
		Weight
	}

	// Token: 0x02001709 RID: 5897
	public class Sort
	{
		// Token: 0x0400AA23 RID: 43555
		public SimpleItemSort.SortType Type;

		// Token: 0x0400AA24 RID: 43556
		public bool IsAscending;
	}

	// Token: 0x0200170A RID: 5898
	public struct ItemCompareItem
	{
		// Token: 0x0400AA25 RID: 43557
		public string Name;

		// Token: 0x0400AA26 RID: 43558
		public sbyte Grade;

		// Token: 0x0400AA27 RID: 43559
		public int Value;

		// Token: 0x0400AA28 RID: 43560
		public int Weight;
	}
}
