using System;
using System.Collections.Generic;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

// Token: 0x0200034E RID: 846
public class InfectSortAndFilter : Refers
{
	// Token: 0x0600315B RID: 12635 RVA: 0x00184F00 File Offset: 0x00183100
	public void Init()
	{
		bool inited = this._inited;
		if (!inited)
		{
			this.SortFilterSetting = new InfectSortFilterSetting();
			this._sortBtnHolder = base.CGet<RectTransform>("SortTypeHolder");
			for (int i = 0; i < this._sortBtnHolder.childCount; i++)
			{
				InfectSortAndFilter.SortType type = (InfectSortAndFilter.SortType)i;
				CButtonObsolete sortBtn = this._sortBtnHolder.GetChild(i).GetComponent<CButtonObsolete>();
				sortBtn.ClearAndAddListener(delegate
				{
					this.OnClickSortType(sortBtn, type);
				});
			}
			this._filterTogGroup = base.CGet<CToggleGroupObsolete>("Filter");
			this._filterTogGroup.InitPreOnToggle(-1);
			this._filterTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnFilterTogChange);
			this._inited = true;
		}
	}

	// Token: 0x0600315C RID: 12636 RVA: 0x00184FD8 File Offset: 0x001831D8
	public void SetList(ref List<CharacterDisplayDataForInfect> list, bool isGoodEnd, bool reset = false, string listTag = null, Action onListChanged = null)
	{
		this._charList = list;
		this._isGoodEnd = isGoodEnd;
		bool flag = !reset;
		if (flag)
		{
			this.UpdateList();
		}
		else
		{
			this._onListChanged = onListChanged;
			this._listTag = listTag;
			this.SortFilterSetting = SingletonObject.getInstance<GameSort>().GetInfectSortConfig(listTag);
			bool flag2 = this._filterTogGroup && this._filterTogGroup.gameObject.activeSelf;
			if (flag2)
			{
				this._filterTogInitializing = true;
				bool flag3 = this.SortFilterSetting.FilterTypes.Count == 1;
				if (flag3)
				{
					this._filterTogGroup.Set((int)this.SortFilterSetting.FilterTypes[0], true, true);
				}
				else
				{
					this._filterTogGroup.Set(0, true, true);
				}
				this._filterTogInitializing = false;
			}
			for (InfectSortAndFilter.SortType type = InfectSortAndFilter.SortType.Infect; type < InfectSortAndFilter.SortType.Count; type++)
			{
				int index = this.SortFilterSetting.SortTypes.IndexOf(type);
				Transform sortBtnTrans = this._sortBtnHolder.GetChild((int)type);
				RectTransform arrow = sortBtnTrans.Find("Arrow").GetComponent<RectTransform>();
				arrow.gameObject.SetActive(index >= 0);
				bool flag4 = index >= 0;
				if (flag4)
				{
					bool isDescSort = this.SortFilterSetting.IsDescSort[index];
					arrow.localRotation = SortFilter.GetArrowRotation(isDescSort);
					arrow.anchoredPosition = SortFilter.GetArrowAnchoredPos(isDescSort);
				}
				sortBtnTrans.Find("Index").GetComponent<TextMeshProUGUI>().text = ((index < 0) ? "" : string.Format("{0}", index + 1));
				sortBtnTrans.Find("IndexBg").gameObject.SetActive(index >= 0);
				sortBtnTrans.Find("CheckMark").gameObject.SetActive(index >= 0);
			}
			this.UpdateList();
		}
	}

	// Token: 0x0600315D RID: 12637 RVA: 0x001851C8 File Offset: 0x001833C8
	private void OnClickSortType(CButtonObsolete btn, InfectSortAndFilter.SortType type)
	{
		int index = this.SortFilterSetting.SortTypes.IndexOf(type);
		RectTransform arrow = btn.transform.Find("Arrow").GetComponent<RectTransform>();
		bool flag = index >= 0;
		if (flag)
		{
			bool flag2 = this.SortFilterSetting.IsDescSort[index];
			if (flag2)
			{
				this.SortFilterSetting.IsDescSort[index] = false;
			}
			else
			{
				this.SortFilterSetting.SortTypes.RemoveAt(index);
				this.SortFilterSetting.IsDescSort.RemoveAt(index);
				for (int i = index; i < this.SortFilterSetting.SortTypes.Count; i++)
				{
					this._sortBtnHolder.GetChild((int)this.SortFilterSetting.SortTypes[i]).Find("Index").GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
				}
				index = -1;
			}
		}
		else
		{
			index = this.SortFilterSetting.SortTypes.Count;
			this.SortFilterSetting.SortTypes.Add(type);
			this.SortFilterSetting.IsDescSort.Add(true);
		}
		arrow.gameObject.SetActive(index >= 0);
		bool flag3 = index >= 0;
		if (flag3)
		{
			bool isDescSort = this.SortFilterSetting.IsDescSort[index];
			arrow.localRotation = SortFilter.GetArrowRotation(isDescSort);
			arrow.anchoredPosition = SortFilter.GetArrowAnchoredPos(isDescSort);
		}
		btn.transform.Find("Index").GetComponent<TextMeshProUGUI>().text = ((index < 0) ? "" : string.Format("{0}", index + 1));
		btn.transform.Find("IndexBg").gameObject.SetActive(index >= 0);
		btn.transform.Find("CheckMark").gameObject.SetActive(index >= 0);
		this.UpdateList();
	}

	// Token: 0x0600315E RID: 12638 RVA: 0x001853D8 File Offset: 0x001835D8
	private int ItemCompare(CharacterDisplayDataForInfect itemL, CharacterDisplayDataForInfect itemR)
	{
		bool interactableL = UI_ThreeVitals.IsTargetInteractable(this._isGoodEnd, (int)itemL.Infection);
		bool interactableR = UI_ThreeVitals.IsTargetInteractable(this._isGoodEnd, (int)itemR.Infection);
		bool flag = interactableL != interactableR;
		int result;
		if (flag)
		{
			result = interactableR.CompareTo(interactableL);
		}
		else
		{
			for (int i = 0; i < this.SortFilterSetting.SortTypes.Count; i++)
			{
				InfectSortAndFilter.SortType sortType = this.SortFilterSetting.SortTypes[i];
				InfectSortAndFilter.SortType sortType2 = sortType;
				if (sortType2 == InfectSortAndFilter.SortType.Infect)
				{
					bool flag2 = itemL.TempInfection != itemR.TempInfection;
					if (flag2)
					{
						return this.SortFilterSetting.IsDescSort[i] ? itemR.TempInfection.CompareTo(itemL.TempInfection) : itemL.TempInfection.CompareTo(itemR.TempInfection);
					}
				}
			}
			result = 0;
		}
		return result;
	}

	// Token: 0x0600315F RID: 12639 RVA: 0x001854C8 File Offset: 0x001836C8
	private void UpdateList()
	{
		this.OutputList.Clear();
		bool activeSelf = this._filterTogGroup.gameObject.activeSelf;
		if (activeSelf)
		{
			List<InfectSortAndFilter.FilterType> typeList = this.SortFilterSetting.FilterTypes;
			bool flag = typeList.Count == 0 || typeList[0] == InfectSortAndFilter.FilterType.All;
			if (flag)
			{
				this.OutputList.AddRange(this._charList);
			}
			else
			{
				this.OutputList.AddRange(this._charList.FindAll((CharacterDisplayDataForInfect data) => typeList.Contains(this.GetFilterType(data))));
			}
		}
		this.OutputList.Sort(new Comparison<CharacterDisplayDataForInfect>(this.ItemCompare));
		Action onListChanged = this._onListChanged;
		if (onListChanged != null)
		{
			onListChanged();
		}
	}

	// Token: 0x06003160 RID: 12640 RVA: 0x0018559C File Offset: 0x0018379C
	private InfectSortAndFilter.FilterType GetFilterType(CharacterDisplayDataForInfect data)
	{
		bool isTeammate = data.IsTeammate;
		InfectSortAndFilter.FilterType result;
		if (isTeammate)
		{
			result = InfectSortAndFilter.FilterType.Teammate;
		}
		else
		{
			bool isKidnapped = data.IsKidnapped;
			if (isKidnapped)
			{
				result = InfectSortAndFilter.FilterType.Kidnap;
			}
			else
			{
				result = InfectSortAndFilter.FilterType.All;
			}
		}
		return result;
	}

	// Token: 0x06003161 RID: 12641 RVA: 0x001855CC File Offset: 0x001837CC
	private void OnFilterTogChange(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		bool filterTogInitializing = this._filterTogInitializing;
		if (!filterTogInitializing)
		{
			this.SortFilterSetting.FilterTypes.Clear();
			bool flag = !this._filterTogInitializing;
			if (flag)
			{
				InfectSortAndFilter.FilterType type = (InfectSortAndFilter.FilterType)this._filterTogGroup.GetActive().Key;
				this.SortFilterSetting.FilterTypes.Add((type == InfectSortAndFilter.FilterType.All) ? InfectSortAndFilter.FilterType.All : type);
			}
			this.UpdateList();
		}
	}

	// Token: 0x06003162 RID: 12642 RVA: 0x00185638 File Offset: 0x00183838
	private static bool IsToggleActive(CToggleObsolete tog)
	{
		return tog.gameObject.activeSelf && tog.interactable && tog.enabled;
	}

	// Token: 0x06003163 RID: 12643 RVA: 0x00185668 File Offset: 0x00183868
	public void SaveSortFilterSetting()
	{
		bool flag = this._listTag != null;
		if (flag)
		{
			SingletonObject.getInstance<GameSort>().SetInfectSortConfig(this._listTag, this.SortFilterSetting);
		}
	}

	// Token: 0x04002427 RID: 9255
	private List<CharacterDisplayDataForInfect> _charList;

	// Token: 0x04002428 RID: 9256
	private bool _inited = false;

	// Token: 0x04002429 RID: 9257
	private string _listTag;

	// Token: 0x0400242A RID: 9258
	[NonSerialized]
	public InfectSortFilterSetting SortFilterSetting;

	// Token: 0x0400242B RID: 9259
	public readonly List<CharacterDisplayDataForInfect> OutputList = new List<CharacterDisplayDataForInfect>();

	// Token: 0x0400242C RID: 9260
	private Action _onListChanged;

	// Token: 0x0400242D RID: 9261
	private RectTransform _sortBtnHolder;

	// Token: 0x0400242E RID: 9262
	private CToggleGroupObsolete _filterTogGroup;

	// Token: 0x0400242F RID: 9263
	private bool _filterTogInitializing = false;

	// Token: 0x04002430 RID: 9264
	private bool _isGoodEnd;

	// Token: 0x020016DC RID: 5852
	public enum FilterType
	{
		// Token: 0x0400A959 RID: 43353
		All,
		// Token: 0x0400A95A RID: 43354
		Teammate,
		// Token: 0x0400A95B RID: 43355
		Kidnap,
		// Token: 0x0400A95C RID: 43356
		Count
	}

	// Token: 0x020016DD RID: 5853
	public enum SortType
	{
		// Token: 0x0400A95E RID: 43358
		Invalid = -1,
		// Token: 0x0400A95F RID: 43359
		Infect,
		// Token: 0x0400A960 RID: 43360
		Count
	}
}
