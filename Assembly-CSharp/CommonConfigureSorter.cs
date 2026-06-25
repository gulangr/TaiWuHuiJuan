using System;
using TMPro;
using UnityEngine;

// Token: 0x02000329 RID: 809
public class CommonConfigureSorter : CommonConfigureBase
{
	// Token: 0x1700052A RID: 1322
	// (set) Token: 0x06002F0C RID: 12044 RVA: 0x00171E68 File Offset: 0x00170068
	public string SwitchText
	{
		set
		{
			foreach (TMP_Text description in this.descriptions)
			{
				description.text = value;
			}
		}
	}

	// Token: 0x1700052B RID: 1323
	// (set) Token: 0x06002F0D RID: 12045 RVA: 0x00171E98 File Offset: 0x00170098
	private Sprite SortingIndicator
	{
		set
		{
			foreach (CImage image in this.sortingIndicators)
			{
				image.sprite = value;
			}
		}
	}

	// Token: 0x06002F0E RID: 12046 RVA: 0x00171EC8 File Offset: 0x001700C8
	public void SetOrder(int newOrder)
	{
		bool flag = newOrder < 0;
		if (flag)
		{
			this.ResetOrder();
		}
		else
		{
			this.order = newOrder;
			this.numberBase.SetActive(true);
			this.number.text = (newOrder + 1).ToString();
		}
	}

	// Token: 0x06002F0F RID: 12047 RVA: 0x00171F14 File Offset: 0x00170114
	public void ResetOrder()
	{
		this.order = -1;
		this.numberBase.SetActive(false);
		this.ChangeSortingType(0);
	}

	// Token: 0x06002F10 RID: 12048 RVA: 0x00171F34 File Offset: 0x00170134
	public void ChangeSortingType(int type)
	{
		this.sortingType = type;
		if (!true)
		{
		}
		ValueTuple<Sprite, string> valueTuple;
		if (type != 1)
		{
			if (type != 2)
			{
				valueTuple = new ValueTuple<Sprite, string>(this.noSorting, LanguageKey.LK_UI_SortingOrder_0.Tr());
			}
			else
			{
				valueTuple = new ValueTuple<Sprite, string>(this.reversedOrder, LanguageKey.LK_UI_SortingOrder_2.Tr());
			}
		}
		else
		{
			valueTuple = new ValueTuple<Sprite, string>(this.normalOrder, LanguageKey.LK_UI_SortingOrder_1.Tr());
		}
		if (!true)
		{
		}
		ValueTuple<Sprite, string> valueTuple2 = valueTuple;
		this.SortingIndicator = valueTuple2.Item1;
		this.SwitchText = valueTuple2.Item2;
	}

	// Token: 0x06002F11 RID: 12049 RVA: 0x00171FCA File Offset: 0x001701CA
	public void CyclingSortingType()
	{
		this.ChangeSortingType((this.sortingType + 1) % 3);
	}

	// Token: 0x06002F12 RID: 12050 RVA: 0x00171FE0 File Offset: 0x001701E0
	public void OnClickLeft()
	{
		int oldSortingType = this.sortingType;
		this.parent.Clear();
		this.parent.AddSortingOrder(this.index, 0);
		this.ChangeSortingType((oldSortingType + 1) % 3);
		bool flag = this.sortingType == 0;
		if (flag)
		{
			this.parent.Clear();
		}
	}

	// Token: 0x06002F13 RID: 12051 RVA: 0x0017203C File Offset: 0x0017023C
	public void OnClickButton()
	{
		int num = this.sortingType;
		int num2 = num;
		if (num2 != 0)
		{
			if (num2 != 1)
			{
				int oldOrder = this.order;
				this.ResetOrder();
				this.order = oldOrder;
				this.parent.RecordRemoveSortingOrder(this);
			}
			else
			{
				this.CyclingSortingType();
			}
		}
		else
		{
			this.SetOrder(this.order);
			bool flag = this.order == -1;
			if (flag)
			{
				this.parent.AddSortingOrder(this.index, -1);
			}
			else
			{
				this.CyclingSortingType();
			}
		}
	}

	// Token: 0x06002F14 RID: 12052 RVA: 0x001720C4 File Offset: 0x001702C4
	public void ApplyReset()
	{
		bool flag = this.sortingType != 0 || this.order == -1;
		if (!flag)
		{
			this.parent.OnChildMouseLeave();
			this.ResetOrder();
		}
	}

	// Token: 0x0400222A RID: 8746
	[ReadOnly]
	public int index;

	// Token: 0x0400222B RID: 8747
	[ReadOnly]
	public int order = -1;

	// Token: 0x0400222C RID: 8748
	[ReadOnly]
	public int sortingType;

	// Token: 0x0400222D RID: 8749
	[SerializeField]
	internal CommonSorterManager parent;

	// Token: 0x0400222E RID: 8750
	[SerializeField]
	private Sprite noSorting;

	// Token: 0x0400222F RID: 8751
	[SerializeField]
	private Sprite normalOrder;

	// Token: 0x04002230 RID: 8752
	[SerializeField]
	private Sprite reversedOrder;

	// Token: 0x04002231 RID: 8753
	[SerializeField]
	private CImage[] sortingIndicators;

	// Token: 0x04002232 RID: 8754
	[SerializeField]
	private GameObject numberBase;

	// Token: 0x04002233 RID: 8755
	[SerializeField]
	private TMP_Text number;

	// Token: 0x04002234 RID: 8756
	[SerializeField]
	private TMP_Text[] descriptions;
}
