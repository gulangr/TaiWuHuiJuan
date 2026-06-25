using System;
using System.Collections.Generic;
using DisplayConfig;
using GameData.Domains.Character;
using TMPro;
using UnityEngine;

// Token: 0x0200035C RID: 860
public class SimplePersonalitySort : Refers
{
	// Token: 0x17000571 RID: 1393
	// (get) Token: 0x06003219 RID: 12825 RVA: 0x0018BA95 File Offset: 0x00189C95
	public bool IsAnySortActive
	{
		get
		{
			return this._sorts.Count > 0;
		}
	}

	// Token: 0x0600321A RID: 12826 RVA: 0x0018BAA5 File Offset: 0x00189CA5
	public void Init()
	{
		this.InitRefers();
		this.InitAll(null);
	}

	// Token: 0x0600321B RID: 12827 RVA: 0x0018BAB8 File Offset: 0x00189CB8
	private void InitAll(List<sbyte> personalityTypes = null)
	{
		SimplePersonalitySort.ItemOrder.Clear();
		this._personalityTypes.Clear();
		this._sorts.Clear();
		bool flag = personalityTypes == null;
		if (flag)
		{
			for (int p = 0; p < 7; p++)
			{
				this._personalityTypes.Add((sbyte)p);
			}
		}
		else
		{
			this._personalityTypes.AddRange(personalityTypes);
		}
		CommonUtils.PrepareEnoughChildren(base.transform, this._personalityItem.gameObject, this._personalityTypes.Count, null);
		for (int index = 0; index < this._personalityTypes.Count; index++)
		{
			this.InitOneSortItem(index, this._personalityTypes[index]);
		}
		this.RefreshSortItems();
	}

	// Token: 0x0600321C RID: 12828 RVA: 0x0018BB8C File Offset: 0x00189D8C
	private void InitOneSortItem(int uiIndex, sbyte personalityType)
	{
		Refers refers = base.transform.GetChild(uiIndex).GetComponent<Refers>();
		SimplePersonalitySort.ItemOrder.Add(refers);
		CButtonObsolete button = refers.GetComponent<CButtonObsolete>();
		int pp = (int)personalityType;
		button.ClearAndAddListener(delegate
		{
			this.OnItemButtonClick(pp);
		});
		CImage icon = refers.CGet<CImage>("Icon");
		icon.SetSprite(Personality.Instance[(int)personalityType].Icon, false, null);
	}

	// Token: 0x0600321D RID: 12829 RVA: 0x0018BC0B File Offset: 0x00189E0B
	public void SetCallback(Action onSortChanged)
	{
		this._onSortChanged = onSortChanged;
	}

	// Token: 0x0600321E RID: 12830 RVA: 0x0018BC15 File Offset: 0x00189E15
	public void SetVisiblePersonalityTypes(List<sbyte> personalityTypes)
	{
		this.InitAll(personalityTypes);
	}

	// Token: 0x0600321F RID: 12831 RVA: 0x0018BC20 File Offset: 0x00189E20
	public unsafe int CompareItem(Personalities pL, Personalities pR)
	{
		foreach (SimplePersonalitySort.Sort sort in this._sorts)
		{
			int index = (int)sort.PersonalityType;
			bool flag = *pL[index] != *pR[index];
			if (flag)
			{
				return (int)(sort.IsAccending ? (*pL[index] - *pR[index]) : (*pR[index] - *pL[index]));
			}
		}
		return 0;
	}

	// Token: 0x06003220 RID: 12832 RVA: 0x0018BCD0 File Offset: 0x00189ED0
	private void OnItemButtonClick(int index)
	{
		int findIndex = this._sorts.FindIndex((SimplePersonalitySort.Sort s) => (int)s.PersonalityType == index);
		bool flag = findIndex == -1;
		if (flag)
		{
			this._sorts.Add(new SimplePersonalitySort.Sort
			{
				PersonalityType = (sbyte)index,
				IsAccending = false
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
			SimplePersonalitySort.Sort sort = this._sorts[findIndex];
			bool isAccending = sort.IsAccending;
			if (isAccending)
			{
				this._sorts.Remove(sort);
			}
			else
			{
				sort.IsAccending = true;
			}
			this.RefreshSortItems();
			Action onSortChanged2 = this._onSortChanged;
			if (onSortChanged2 != null)
			{
				onSortChanged2();
			}
		}
	}

	// Token: 0x06003221 RID: 12833 RVA: 0x0018BD98 File Offset: 0x00189F98
	private void RefreshSortItems()
	{
		for (int i = 0; i < this._personalityTypes.Count; i++)
		{
			sbyte p = this._personalityTypes[i];
			int sortIndex = this._sorts.FindIndex((SimplePersonalitySort.Sort s) => s.PersonalityType == p);
			bool flag = sortIndex != -1;
			if (flag)
			{
				this.RefreshSortItem(SimplePersonalitySort.ItemOrder[i], sortIndex, this._sorts[sortIndex]);
			}
			else
			{
				this.RefreshSortItem(SimplePersonalitySort.ItemOrder[i], -1, null);
			}
		}
	}

	// Token: 0x06003222 RID: 12834 RVA: 0x0018BE38 File Offset: 0x0018A038
	private void RefreshSortItem(Refers refers, int sortIndex, SimplePersonalitySort.Sort sort)
	{
		GameObject checkMark = refers.CGet<GameObject>("CheckMark");
		GameObject upArrow = refers.CGet<GameObject>("UpArrow");
		GameObject downArrow = refers.CGet<GameObject>("DownArrow");
		TextMeshProUGUI index = refers.CGet<TextMeshProUGUI>("Index");
		GameObject indexBg = refers.CGet<GameObject>("IndexBg");
		bool flag = sort != null;
		if (flag)
		{
			upArrow.SetActive(sort.IsAccending);
			downArrow.SetActive(!sort.IsAccending);
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
	}

	// Token: 0x06003223 RID: 12835 RVA: 0x0018BEF5 File Offset: 0x0018A0F5
	private void InitRefers()
	{
		this._personalityItem = base.CGet<Refers>("PersonalityItem");
	}

	// Token: 0x040024AD RID: 9389
	private Action _onSortChanged;

	// Token: 0x040024AE RID: 9390
	private List<SimplePersonalitySort.Sort> _sorts = new List<SimplePersonalitySort.Sort>();

	// Token: 0x040024AF RID: 9391
	private readonly List<sbyte> _personalityTypes = new List<sbyte>();

	// Token: 0x040024B0 RID: 9392
	private static readonly List<Refers> ItemOrder = new List<Refers>();

	// Token: 0x040024B1 RID: 9393
	private Refers _personalityItem;

	// Token: 0x02001719 RID: 5913
	public class Sort
	{
		// Token: 0x0400AA73 RID: 43635
		public sbyte PersonalityType;

		// Token: 0x0400AA74 RID: 43636
		public bool IsAccending;
	}
}
