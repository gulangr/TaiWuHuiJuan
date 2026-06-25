using System;
using System.Collections.Generic;

// Token: 0x02000354 RID: 852
public class ResidentSortAndFilter : Refers
{
	// Token: 0x060031C1 RID: 12737 RVA: 0x00189440 File Offset: 0x00187640
	public void Init()
	{
		bool inited = this._inited;
		if (!inited)
		{
			this.SortFilterSetting = new ResidentSortFilterSetting();
			this._filterTogGroup = base.CGet<CToggleGroupObsolete>("Filter");
			this._filterTogGroup.InitPreOnToggle(-1);
			this._filterTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnCharIdFilterTogChange);
			this._inited = true;
		}
	}

	// Token: 0x060031C2 RID: 12738 RVA: 0x001894A4 File Offset: 0x001876A4
	public void SetCharIDList(ref List<ValueTuple<int, string>> charIdList, Action onCharIdListChanged = null, string listTag = null)
	{
		this._charIdList = charIdList;
		this._listTag = listTag;
		this._onCharIdListChanged = onCharIdListChanged;
		this.SortFilterSetting = SingletonObject.getInstance<GameSort>().GetResidentSortConfig(listTag);
		bool activeSelf = this._filterTogGroup.gameObject.activeSelf;
		if (activeSelf)
		{
			foreach (ResidentSortAndFilter.ResidentFilterType type in this.SortFilterSetting.ResidentFilterType)
			{
				this._filterTogGroup.Set((int)type, true, false);
			}
		}
		this.UpdateCharIDList();
	}

	// Token: 0x060031C3 RID: 12739 RVA: 0x0018954C File Offset: 0x0018774C
	private void OnCharIdFilterTogChange(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		bool filterTogAutoSelecting = this._filterTogAutoSelecting;
		if (!filterTogAutoSelecting)
		{
			this._filterTogAutoSelecting = true;
			bool flag = togNew != null;
			if (flag)
			{
				bool flag2 = togNew.Key == 0;
				if (flag2)
				{
					for (int i = 1; i < this._filterTogGroup.Count(); i++)
					{
						CToggleObsolete tog = this._filterTogGroup.Get(i);
						bool flag3 = !tog.isOn;
						if (flag3)
						{
							this._filterTogGroup.Set(tog, true);
						}
					}
				}
				else
				{
					bool flag4 = this._filterTogGroup.GetAllActive().Count >= this._filterTogGroup.Count() - 1;
					if (flag4)
					{
						this._filterTogGroup.Set(this._filterTogGroup.Get(0), true);
					}
				}
			}
			bool flag5 = togOld != null;
			if (flag5)
			{
				bool flag6 = togOld.Key == 0;
				if (flag6)
				{
					for (int j = 1; j < this._filterTogGroup.Count(); j++)
					{
						this._filterTogGroup.Set(this._filterTogGroup.Get(j), false);
					}
				}
				else
				{
					CToggleObsolete togAll = this._filterTogGroup.Get(0);
					bool isOn = togAll.isOn;
					if (isOn)
					{
						this._filterTogGroup.Set(togAll, false);
					}
				}
			}
			this._filterTogAutoSelecting = false;
			this.UpdateCharIDList();
		}
	}

	// Token: 0x060031C4 RID: 12740 RVA: 0x001896B8 File Offset: 0x001878B8
	private void UpdateCharIDList()
	{
		this.OutputCharIdList.Clear();
		bool activeSelf = this._filterTogGroup.gameObject.activeSelf;
		if (activeSelf)
		{
			List<ValueTuple<int, string>> tmpCharList = this._charIdList.FindAll((ValueTuple<int, string> data) => this._filterTogGroup.Get((int)ResidentSortAndFilter.GetFilterType(data.Item2)).isOn);
			foreach (ValueTuple<int, string> tmp in tmpCharList)
			{
				this.OutputCharIdList.Add(tmp.Item1);
			}
		}
		else
		{
			foreach (ValueTuple<int, string> tmp2 in this._charIdList)
			{
				this.OutputCharIdList.Add(tmp2.Item1);
			}
		}
		Action onCharIdListChanged = this._onCharIdListChanged;
		if (onCharIdListChanged != null)
		{
			onCharIdListChanged();
		}
	}

	// Token: 0x060031C5 RID: 12741 RVA: 0x001897BC File Offset: 0x001879BC
	public static ResidentSortAndFilter.ResidentFilterType GetFilterType(string charDes)
	{
		bool flag = charDes.Equals(LocalStringManager.Get(LanguageKey.LK_ResidentState_Residence));
		ResidentSortAndFilter.ResidentFilterType result;
		if (flag)
		{
			result = ResidentSortAndFilter.ResidentFilterType.Resident;
		}
		else
		{
			bool flag2 = charDes.Equals(LocalStringManager.Get(LanguageKey.LK_ResidentState_ComfortableHouse));
			if (flag2)
			{
				result = ResidentSortAndFilter.ResidentFilterType.ComfortableHouse;
			}
			else
			{
				bool flag3 = charDes.Equals(LocalStringManager.Get(LanguageKey.LK_ResidentState_Homeless));
				if (flag3)
				{
					result = ResidentSortAndFilter.ResidentFilterType.Homeless;
				}
				else
				{
					result = ResidentSortAndFilter.ResidentFilterType.Invalid;
				}
			}
		}
		return result;
	}

	// Token: 0x060031C6 RID: 12742 RVA: 0x0018981C File Offset: 0x00187A1C
	public void SaveSortFilterSetting()
	{
		bool flag = this._listTag != null;
		if (flag)
		{
			SingletonObject.getInstance<GameSort>().SetResidentSortConfig(this._listTag, this.SortFilterSetting);
		}
	}

	// Token: 0x04002476 RID: 9334
	private Action _onCharIdListChanged;

	// Token: 0x04002477 RID: 9335
	private CToggleGroupObsolete _filterTogGroup;

	// Token: 0x04002478 RID: 9336
	private bool _inited = false;

	// Token: 0x04002479 RID: 9337
	private string _listTag;

	// Token: 0x0400247A RID: 9338
	private bool _filterTogAutoSelecting = false;

	// Token: 0x0400247B RID: 9339
	[NonSerialized]
	public ResidentSortFilterSetting SortFilterSetting;

	// Token: 0x0400247C RID: 9340
	public List<ValueTuple<int, string>> _charIdList = new List<ValueTuple<int, string>>();

	// Token: 0x0400247D RID: 9341
	public readonly List<int> OutputCharIdList = new List<int>();

	// Token: 0x020016FC RID: 5884
	public enum SortType
	{
		// Token: 0x0400A9F8 RID: 43512
		Invalid = -1,
		// Token: 0x0400A9F9 RID: 43513
		Name,
		// Token: 0x0400A9FA RID: 43514
		OrgGrade,
		// Token: 0x0400A9FB RID: 43515
		Age,
		// Token: 0x0400A9FC RID: 43516
		Work
	}

	// Token: 0x020016FD RID: 5885
	public enum ResidentFilterType
	{
		// Token: 0x0400A9FE RID: 43518
		Invalid,
		// Token: 0x0400A9FF RID: 43519
		Resident,
		// Token: 0x0400AA00 RID: 43520
		ComfortableHouse,
		// Token: 0x0400AA01 RID: 43521
		Homeless,
		// Token: 0x0400AA02 RID: 43522
		Count
	}
}
