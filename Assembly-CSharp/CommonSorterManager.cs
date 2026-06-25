using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000330 RID: 816
public class CommonSorterManager : MonoBehaviour
{
	// Token: 0x06002F4B RID: 12107 RVA: 0x00173504 File Offset: 0x00171704
	private void Awake()
	{
		foreach (CommonConfigureSorter child in this.childs)
		{
			child.ResetOrder();
		}
	}

	// Token: 0x06002F4C RID: 12108 RVA: 0x00173534 File Offset: 0x00171734
	public void CalcOrder(ref int order)
	{
		order %= this.order2Index.Count + 1;
		bool flag = order < 0;
		if (flag)
		{
			order += this.order2Index.Count + 1;
		}
	}

	// Token: 0x06002F4D RID: 12109 RVA: 0x00173570 File Offset: 0x00171770
	public void RefreshChildOrders(int fromOrder)
	{
		int i = this.order2Index.Count;
		while (i-- > fromOrder)
		{
			this.childs[this.order2Index[i]].SetOrder(i);
		}
	}

	// Token: 0x06002F4E RID: 12110 RVA: 0x001735B2 File Offset: 0x001717B2
	public void AddSortingOrder(int index, int order = -1)
	{
		this.CalcOrder(ref order);
		this.order2Index.Insert(order, index);
		this.childs[index].CyclingSortingType();
		this.RefreshChildOrders(order);
	}

	// Token: 0x06002F4F RID: 12111 RVA: 0x001735E2 File Offset: 0x001717E2
	public void RemoveSortingOrder(int order)
	{
		this.CalcOrder(ref order);
		this.childs[this.order2Index[order]].ResetOrder();
		this.order2Index.RemoveAt(order);
		this.RefreshChildOrders(order);
	}

	// Token: 0x06002F50 RID: 12112 RVA: 0x0017361C File Offset: 0x0017181C
	public void RecordRemoveSortingOrder(CommonConfigureSorter child)
	{
		this.previousSortedIndex = child.index;
	}

	// Token: 0x06002F51 RID: 12113 RVA: 0x0017362C File Offset: 0x0017182C
	public void Clear()
	{
		foreach (int index in this.order2Index)
		{
			this.childs[index].ResetOrder();
		}
		this.order2Index.Clear();
	}

	// Token: 0x06002F52 RID: 12114 RVA: 0x00173698 File Offset: 0x00171898
	public void OnChildMouseLeave()
	{
		bool flag = this.previousSortedIndex != -1;
		if (flag)
		{
			this.RemoveSortingOrder(this.childs[this.previousSortedIndex].order);
		}
	}

	// Token: 0x04002260 RID: 8800
	public CommonConfigureSorter[] childs;

	// Token: 0x04002261 RID: 8801
	public List<int> order2Index;

	// Token: 0x04002262 RID: 8802
	public int previousSortedIndex = -1;
}
