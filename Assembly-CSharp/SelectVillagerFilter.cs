using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000356 RID: 854
public class SelectVillagerFilter : Refers
{
	// Token: 0x1700056A RID: 1386
	// (get) Token: 0x060031CC RID: 12748 RVA: 0x0018996A File Offset: 0x00187B6A
	// (set) Token: 0x060031CD RID: 12749 RVA: 0x00189972 File Offset: 0x00187B72
	public EVillagerFilterType SelectedType
	{
		get
		{
			return this._selectedType;
		}
		set
		{
			this._selectedType = value;
			this.SetToggleStatusAll(value, false);
		}
	}

	// Token: 0x060031CE RID: 12750 RVA: 0x00189988 File Offset: 0x00187B88
	public void SetToggleStatusAll(EVillagerFilterType selectedType, bool triggerEvent)
	{
		foreach (KeyValuePair<EVillagerFilterType, CToggleObsolete> item in this._toggleDic)
		{
			bool flag = selectedType.HasFlag(item.Key);
			if (flag)
			{
				item.Value.isOn = true;
				EVillagerFilterType filterMask;
				bool flag2 = this._mutexDic.TryGetValue(item.Key, out filterMask);
				if (flag2)
				{
					this.SetToggleActive(filterMask, false);
				}
			}
			else
			{
				item.Value.isOn = false;
			}
		}
	}

	// Token: 0x060031CF RID: 12751 RVA: 0x00189A3C File Offset: 0x00187C3C
	private void SetToggleActive(EVillagerFilterType filterMask, bool isActive)
	{
		foreach (KeyValuePair<EVillagerFilterType, CToggleObsolete> item in this._toggleDic)
		{
			bool flag = filterMask.HasFlag(item.Key);
			if (flag)
			{
				item.Value.isOn = isActive;
			}
		}
	}

	// Token: 0x060031D0 RID: 12752 RVA: 0x00189AB8 File Offset: 0x00187CB8
	public void SetCallback(Action callback)
	{
		this._onSelectedTypeChanged = callback;
	}

	// Token: 0x060031D1 RID: 12753 RVA: 0x00189AC2 File Offset: 0x00187CC2
	public void Init(EVillagerFilterType filterTypes, EVillagerFilterType defaultStatus)
	{
		this._filterTypes = filterTypes;
		this._defaultStatus = defaultStatus;
		this.InitRefers();
		this.InitToggles(filterTypes);
		this.SelectedType = defaultStatus;
	}

	// Token: 0x060031D2 RID: 12754 RVA: 0x00189AEC File Offset: 0x00187CEC
	private void InitToggles(EVillagerFilterType filterTypes)
	{
		this.ClearAllToggles();
		bool flag = filterTypes == EVillagerFilterType.None;
		if (!flag)
		{
			int toggleIndex = 0;
			bool flag2 = filterTypes.HasFlag(EVillagerFilterType.All);
			if (flag2)
			{
				this.SetupToggle(this.CreateOrActiveToggle(toggleIndex++), EVillagerFilterType.All, LanguageKey.LK_Item_Filter_Type_All);
			}
			bool flag3 = filterTypes.HasFlag(EVillagerFilterType.Adult);
			if (flag3)
			{
				this.SetupToggle(this.CreateOrActiveToggle(toggleIndex++), EVillagerFilterType.Adult, LanguageKey.LK_Item_Filter_Type_Adult);
			}
			bool flag4 = filterTypes.HasFlag(EVillagerFilterType.Teenager);
			if (flag4)
			{
				this.SetupToggle(this.CreateOrActiveToggle(toggleIndex++), EVillagerFilterType.Teenager, LanguageKey.LK_Item_Filter_Type_Teen);
			}
			bool flag5 = filterTypes.HasFlag(EVillagerFilterType.Learning);
			if (flag5)
			{
				this.SetupToggle(this.CreateOrActiveToggle(toggleIndex++), EVillagerFilterType.Learning, LanguageKey.LK_Item_Filter_Type_Learn);
			}
			bool flag6 = filterTypes.HasFlag(EVillagerFilterType.FinishLearning);
			if (flag6)
			{
				this.SetupToggle(this.CreateOrActiveToggle(toggleIndex++), EVillagerFilterType.FinishLearning, LanguageKey.LK_Item_Filter_Type_FinishLearn);
			}
			bool flag7 = filterTypes.HasFlag(EVillagerFilterType.Farmer);
			if (flag7)
			{
				this.SetupToggle(this.CreateOrActiveToggle(toggleIndex++), EVillagerFilterType.Farmer, LanguageKey.LK_VillagerRole_Filter_Farmer);
			}
		}
	}

	// Token: 0x060031D3 RID: 12755 RVA: 0x00189C38 File Offset: 0x00187E38
	private CToggleObsolete CreateOrActiveToggle(int index)
	{
		bool flag = this._toggleInstances.Count > index;
		CToggleObsolete result;
		if (flag)
		{
			result = this._toggleInstances[index];
		}
		else
		{
			result = Object.Instantiate<CToggleObsolete>(this._filterTemplate, this._toggleGroup.transform);
		}
		result.isOn = false;
		result.gameObject.SetActive(true);
		result.onValueChanged.RemoveAllListeners();
		result.onValueChanged.AddListener(delegate(bool isActive)
		{
			this.OnToggleValueChange(isActive, result);
		});
		this._toggleInstances.Add(result);
		return result;
	}

	// Token: 0x060031D4 RID: 12756 RVA: 0x00189D04 File Offset: 0x00187F04
	private void OnToggleValueChange(bool isActive, CToggleObsolete toggle)
	{
		EVillagerFilterType filterType = (EVillagerFilterType)toggle.Key;
		if (isActive)
		{
			this._selectedType |= filterType;
			EVillagerFilterType filterMask;
			bool flag = this._mutexDic.TryGetValue(filterType, out filterMask);
			if (flag)
			{
				this.SetToggleActive(filterMask, false);
			}
		}
		else
		{
			this._selectedType &= ~filterType;
			bool flag2 = this._selectedType == EVillagerFilterType.None;
			if (flag2)
			{
				this.SelectedType = this._defaultStatus;
			}
		}
		Action onSelectedTypeChanged = this._onSelectedTypeChanged;
		if (onSelectedTypeChanged != null)
		{
			onSelectedTypeChanged();
		}
	}

	// Token: 0x060031D5 RID: 12757 RVA: 0x00189D8E File Offset: 0x00187F8E
	private void SetupToggle(CToggleObsolete toggle, EVillagerFilterType filterType, LanguageKey languageKey)
	{
		toggle.GetComponent<Refers>().CGet<TextMeshProUGUI>("Label").text = LocalStringManager.Get(languageKey);
		this._toggleDic[filterType] = toggle;
		toggle.Key = (int)filterType;
	}

	// Token: 0x060031D6 RID: 12758 RVA: 0x00189DC4 File Offset: 0x00187FC4
	private void ClearAllToggles()
	{
		foreach (CToggleObsolete item in this._toggleInstances)
		{
			item.gameObject.SetActive(false);
		}
		this._toggleDic.Clear();
	}

	// Token: 0x060031D7 RID: 12759 RVA: 0x00189E30 File Offset: 0x00188030
	private void InitRefers()
	{
		this._filterTemplate = base.CGet<CToggleObsolete>("FilterTemplate");
		this._toggleGroup = base.CGet<GameObject>("VillagerFilter");
	}

	// Token: 0x0400247F RID: 9343
	private EVillagerFilterType _selectedType;

	// Token: 0x04002480 RID: 9344
	private EVillagerFilterType _filterTypes;

	// Token: 0x04002481 RID: 9345
	private EVillagerFilterType _defaultStatus;

	// Token: 0x04002482 RID: 9346
	private Action _onSelectedTypeChanged;

	// Token: 0x04002483 RID: 9347
	private CToggleObsolete _filterTemplate;

	// Token: 0x04002484 RID: 9348
	private GameObject _toggleGroup;

	// Token: 0x04002485 RID: 9349
	private Dictionary<EVillagerFilterType, CToggleObsolete> _toggleDic = new Dictionary<EVillagerFilterType, CToggleObsolete>();

	// Token: 0x04002486 RID: 9350
	private List<CToggleObsolete> _toggleInstances = new List<CToggleObsolete>();

	// Token: 0x04002487 RID: 9351
	private Dictionary<EVillagerFilterType, EVillagerFilterType> _mutexDic = new Dictionary<EVillagerFilterType, EVillagerFilterType>
	{
		{
			EVillagerFilterType.All,
			EVillagerFilterType.Adult | EVillagerFilterType.Teenager | EVillagerFilterType.Learning | EVillagerFilterType.FinishLearning | EVillagerFilterType.Farmer
		},
		{
			EVillagerFilterType.Adult,
			EVillagerFilterType.All | EVillagerFilterType.Teenager
		},
		{
			EVillagerFilterType.Teenager,
			EVillagerFilterType.All | EVillagerFilterType.Adult
		},
		{
			EVillagerFilterType.Learning,
			EVillagerFilterType.All | EVillagerFilterType.FinishLearning
		},
		{
			EVillagerFilterType.FinishLearning,
			EVillagerFilterType.All | EVillagerFilterType.Learning
		},
		{
			EVillagerFilterType.Farmer,
			EVillagerFilterType.All
		}
	};
}
