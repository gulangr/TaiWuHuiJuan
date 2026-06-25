using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.CombatSkill;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001E8 RID: 488
public class CombatSkillSortAndFilter : Refers
{
	// Token: 0x1700033C RID: 828
	// (get) Token: 0x06002009 RID: 8201 RVA: 0x000E9285 File Offset: 0x000E7485
	private CombatSkillTypeTogGroup CombatSkillTypeTogGroup
	{
		get
		{
			return this._typeFilterTogGroup.GetComponent<CombatSkillTypeTogGroup>();
		}
	}

	// Token: 0x0600200A RID: 8202 RVA: 0x000E9292 File Offset: 0x000E7492
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x0600200B RID: 8203 RVA: 0x000E929C File Offset: 0x000E749C
	private void OnDisable()
	{
		this.StaticAheadIdList.Clear();
		this.StaticLastIdList.Clear();
	}

	// Token: 0x0600200C RID: 8204 RVA: 0x000E92B8 File Offset: 0x000E74B8
	public void Init()
	{
		bool inited = this._inited;
		if (!inited)
		{
			this._sortBtnHolder = base.CGet<RectTransform>("SortTypeHolder");
			for (int i = 0; i < this._sortBtnHolder.childCount; i++)
			{
				CombatSkillSortAndFilter.SortType type = (CombatSkillSortAndFilter.SortType)i;
				CButtonObsolete sortBtn = this._sortBtnHolder.GetChild(i).GetComponent<CButtonObsolete>();
				sortBtn.ClearAndAddListener(delegate
				{
					this.OnClickSortType(sortBtn, type);
				});
			}
			this._typeFilterTogGroup = base.CGet<CToggleGroupObsolete>("TypeFilter");
			this._typeFilterTogGroup.InitPreOnToggle(-1);
			this._typeFilterTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnTypeFilterTogChange);
			this._sectFilterTogGroup = base.CGet<CToggleGroupObsolete>("SectFilter");
			this._sectFilterTogGroup.InitPreOnToggle(-1);
			this._sectFilterTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnSectFilterTogChange);
			for (int j = 0; j < 16; j++)
			{
				CToggleObsolete tog = this._sectFilterTogGroup.Get(j + 1);
				tog.GetComponentInChildren<TextMeshProUGUI>().text = LocalStringManager.Get(string.Format("LK_CombatSkill_Filter_Sect{0}", j));
			}
			bool flag = this.CTryGet<MultiStateToggle>("MultiStateSwitch", out this._breakToggle);
			if (flag)
			{
				this._breakToggle.onValueChanged.RemoveAllListeners();
				this._breakToggle.onValueChanged.AddListener(new UnityAction<int>(this.OnBreakFilterChange));
			}
			this._fiveElementFilter = base.CGet<CToggleGroupObsolete>("FiveElementFilter");
			this._fiveElementFilter.InitPreOnToggle(-1);
			this._fiveElementFilter.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnFiveElementToggleChange);
			for (int k = 0; k < 6; k++)
			{
				CToggleObsolete toggle = this._fiveElementFilter.Get(k + 1);
				Refers refers = toggle.GetComponent<Refers>();
				TextMeshProUGUI label = refers.CGet<TextMeshProUGUI>("Label");
				TextMeshProUGUI fiveElementLabel = refers.CGet<TextMeshProUGUI>("FiveElementLabel");
				label.gameObject.SetActive(false);
				fiveElementLabel.gameObject.SetActive(this.showFiveElementLabel);
				CImage icon = refers.CGet<CImage>("Icon");
				fiveElementLabel.text = LocalStringManager.Get(string.Format("LK_FiveElements_Type_{0}", k));
				icon.SetSprite(string.Format("sp_icon_fiveelements_{0}", k), false, null);
			}
			CToggleObsolete toggleAll = this._fiveElementFilter.Get(0);
			Refers refers2 = toggleAll.GetComponent<Refers>();
			TextMeshProUGUI label2 = refers2.CGet<TextMeshProUGUI>("Label");
			TextMeshProUGUI fiveElementLabel2 = refers2.CGet<TextMeshProUGUI>("FiveElementLabel");
			label2.gameObject.SetActive(true);
			fiveElementLabel2.gameObject.SetActive(false);
			label2.text = LocalStringManager.Get(LanguageKey.LK_Filter_Type_All);
			this._inited = true;
		}
	}

	// Token: 0x0600200D RID: 8205 RVA: 0x000E959E File Offset: 0x000E779E
	public void SetOnTypeFilterChange(Action<sbyte> onTypeFilterChange)
	{
		this._onTypeFilterChange = onTypeFilterChange;
	}

	// Token: 0x0600200E RID: 8206 RVA: 0x000E95A8 File Offset: 0x000E77A8
	public void SetOnSortPreChange(Action onSortPreChange)
	{
		this._onSortPreChange = onSortPreChange;
	}

	// Token: 0x0600200F RID: 8207 RVA: 0x000E95B4 File Offset: 0x000E77B4
	public void SetSkillList(List<CombatSkillDisplayData> skillList, bool reset = false, string listTag = null, Action onSkillListChanged = null, bool addEmptyItem = false)
	{
		this._skillList = skillList;
		this._addEmptyItem = addEmptyItem;
		if (reset)
		{
			this._onSkillListChanged = onSkillListChanged;
			this.SortFilterSetting = ((listTag != null) ? SingletonObject.getInstance<GameSort>().GetCombatSkillSortConfig(listTag) : new CombatSkillSortFilterSetting());
			bool activeSelf = this._typeFilterTogGroup.gameObject.activeSelf;
			if (activeSelf)
			{
				bool activeSelf2 = this._typeFilterTogGroup.Get(-1).gameObject.activeSelf;
				if (activeSelf2)
				{
					this._typeFilterTogGroup.Set(-1, true, false);
				}
				else
				{
					this._typeFilterTogGroup.Set((int)this.SortFilterSetting.SkillType, true, false);
				}
			}
			bool activeSelf3 = this._sectFilterTogGroup.gameObject.activeSelf;
			if (activeSelf3)
			{
				this._sectTogInitializing = true;
				bool flag = this.AutoResetSect || this._sectFilterTogGroup.GetActive() == null;
				if (flag)
				{
					this._sectFilterTogGroup.Set(0, true, false);
				}
				this._sectTogInitializing = false;
			}
			for (CombatSkillSortAndFilter.SortType type = CombatSkillSortAndFilter.SortType.Grade; type < CombatSkillSortAndFilter.SortType.Count; type++)
			{
				int index = this.SortFilterSetting.SortTypes.IndexOf(type);
				Transform sortBtnTrans = this._sortBtnHolder.GetChild((int)type);
				RectTransform arrow = sortBtnTrans.Find("Arrow").GetComponent<RectTransform>();
				arrow.gameObject.SetActive(index >= 0);
				bool flag2 = index >= 0;
				if (flag2)
				{
					bool isDescSort = this.SortFilterSetting.IsDescSort[index];
					arrow.localRotation = SortFilter.GetArrowRotation(isDescSort);
					arrow.anchoredPosition = SortFilter.GetArrowAnchoredPos(isDescSort);
				}
				sortBtnTrans.Find("Index").GetComponent<TextMeshProUGUI>().text = ((index < 0) ? "" : string.Format("{0}", index + 1));
				sortBtnTrans.Find("IndexBg").gameObject.SetActive(index >= 0);
				sortBtnTrans.Find("CheckMark").gameObject.SetActive(index >= 0);
			}
		}
		this.UpdateSkillList();
	}

	// Token: 0x06002010 RID: 8208 RVA: 0x000E97D4 File Offset: 0x000E79D4
	public List<CombatSkillDisplayData> GetAllSkillList()
	{
		return this._skillList;
	}

	// Token: 0x06002011 RID: 8209 RVA: 0x000E97EC File Offset: 0x000E79EC
	public void SetLearnedCount(IList<short> learnedCombatSkillIds)
	{
		this.CombatSkillTypeTogGroup.SetLearnedCount(learnedCombatSkillIds);
	}

	// Token: 0x06002012 RID: 8210 RVA: 0x000E97FC File Offset: 0x000E79FC
	public CombatSkillTypeTogGroup.ToggleInfo? GetSelectedSkillTypeToggleInfo()
	{
		return this.CombatSkillTypeTogGroup.GetSelectedSkillTypeToggleInfo();
	}

	// Token: 0x06002013 RID: 8211 RVA: 0x000E981C File Offset: 0x000E7A1C
	public CombatSkillTypeTogGroup.ToggleInfo? GetToggleInfoByKey(int key)
	{
		return this.CombatSkillTypeTogGroup.GetToggleInfoByKey(key);
	}

	// Token: 0x06002014 RID: 8212 RVA: 0x000E983C File Offset: 0x000E7A3C
	private void OnClickSortType(CButtonObsolete btn, CombatSkillSortAndFilter.SortType type)
	{
		Action onSortPreChange = this._onSortPreChange;
		if (onSortPreChange != null)
		{
			onSortPreChange();
		}
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
		this.UpdateSkillList();
	}

	// Token: 0x06002015 RID: 8213 RVA: 0x000E9A5C File Offset: 0x000E7C5C
	private void OnTypeFilterTogChange(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		this.SortFilterSetting.SkillType = (sbyte)togNew.Key;
		Action<sbyte> onTypeFilterChange = this._onTypeFilterChange;
		if (onTypeFilterChange != null)
		{
			onTypeFilterChange(this.SortFilterSetting.SkillType);
		}
		this.UpdateSkillList();
	}

	// Token: 0x06002016 RID: 8214 RVA: 0x000E9A95 File Offset: 0x000E7C95
	private void OnSectFilterTogChange(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		this.UpdateSkillList();
	}

	// Token: 0x06002017 RID: 8215 RVA: 0x000E9A9F File Offset: 0x000E7C9F
	private void OnFiveElementToggleChange(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		this.UpdateSkillList();
	}

	// Token: 0x06002018 RID: 8216 RVA: 0x000E9AA9 File Offset: 0x000E7CA9
	private void OnBreakFilterChange(int newState)
	{
		this.UpdateSkillList();
	}

	// Token: 0x06002019 RID: 8217 RVA: 0x000E9AB4 File Offset: 0x000E7CB4
	private void UpdateSkillList()
	{
		bool sectTogInitializing = this._sectTogInitializing;
		if (!sectTogInitializing)
		{
			this.OutputSkillList.Clear();
			this.OutputSkillList.AddRange(this._skillList);
			bool flag = this._typeFilterTogGroup.gameObject.activeSelf && this._typeFilterTogGroup.GetActive().Key != -1;
			if (flag)
			{
				int toggleKey = this._typeFilterTogGroup.GetActive().Key;
				bool flag2 = toggleKey >= 0 && toggleKey < CombatSkillTypeTogGroup.ToggleConfig.Count;
				if (flag2)
				{
					CombatSkillTypeTogGroup.ToggleInfo toggleInfo = CombatSkillTypeTogGroup.ToggleConfig[toggleKey];
					this.OutputSkillList.RemoveAll(delegate(CombatSkillDisplayData data)
					{
						CombatSkillItem config = CombatSkill.Instance[data.TemplateId];
						bool flag8 = config.Type != toggleInfo.CombatSkillType;
						bool result;
						if (flag8)
						{
							result = true;
						}
						else
						{
							bool flag9 = config.EquipType != toggleInfo.EquipType;
							result = flag9;
						}
						return result;
					});
				}
			}
			int sectTogKey = this._sectFilterTogGroup.GetActive().Key;
			bool flag3 = this._sectFilterTogGroup.gameObject.activeSelf && sectTogKey != 0;
			if (flag3)
			{
				this.OutputSkillList.RemoveAll(delegate(CombatSkillDisplayData data)
				{
					sbyte sectId = CombatSkill.Instance[data.TemplateId].SectId;
					sectId = (sbyte)Mathf.Min((int)sectId, 17);
					return sectTogKey - 1 != (int)sectId;
				});
			}
			int fiveElementFilterKey = this._fiveElementFilter.GetActive().Key;
			bool flag4 = this._fiveElementFilter.gameObject.activeSelf && fiveElementFilterKey != 0;
			if (flag4)
			{
				this.OutputSkillList.RemoveAll((CombatSkillDisplayData data) => (int)CombatSkill.Instance[data.TemplateId].FiveElements != fiveElementFilterKey - 1);
			}
			bool flag5 = this._breakToggle != null && this._breakToggle.gameObject.activeSelf;
			if (flag5)
			{
				int breakState = this._breakToggle.State;
				bool flag6 = breakState == 1;
				if (flag6)
				{
					this.OutputSkillList.RemoveAll((CombatSkillDisplayData data) => data.BreakSuccess);
				}
				else
				{
					bool flag7 = breakState == 2;
					if (flag7)
					{
						this.OutputSkillList.RemoveAll((CombatSkillDisplayData data) => !data.BreakSuccess);
					}
				}
			}
			this.OutputSkillList.Sort(new Comparison<CombatSkillDisplayData>(this.ItemCompare));
			bool addEmptyItem = this._addEmptyItem;
			if (addEmptyItem)
			{
				this.OutputSkillList.Insert(0, null);
			}
			Action onSkillListChanged = this._onSkillListChanged;
			if (onSkillListChanged != null)
			{
				onSkillListChanged();
			}
		}
	}

	// Token: 0x0600201A RID: 8218 RVA: 0x000E9D1C File Offset: 0x000E7F1C
	private int ItemCompare(CombatSkillDisplayData skillL, CombatSkillDisplayData skillR)
	{
		bool flag = this.StaticAheadIdList.Contains(skillL.TemplateId);
		if (flag)
		{
			bool flag2 = !this.StaticAheadIdList.Contains(skillR.TemplateId);
			if (flag2)
			{
				return -1;
			}
		}
		bool flag3 = this.StaticAheadIdList.Contains(skillR.TemplateId);
		if (flag3)
		{
			bool flag4 = !this.StaticAheadIdList.Contains(skillL.TemplateId);
			if (flag4)
			{
				return 1;
			}
		}
		bool flag5 = this.StaticLastIdList.Contains(skillL.TemplateId);
		if (flag5)
		{
			bool flag6 = !this.StaticLastIdList.Contains(skillR.TemplateId);
			if (flag6)
			{
				return 1;
			}
		}
		bool flag7 = this.StaticLastIdList.Contains(skillR.TemplateId);
		if (flag7)
		{
			bool flag8 = !this.StaticLastIdList.Contains(skillL.TemplateId);
			if (flag8)
			{
				return -1;
			}
		}
		CombatSkillItem configL = CombatSkill.Instance[skillL.TemplateId];
		CombatSkillItem configR = CombatSkill.Instance[skillR.TemplateId];
		for (int i = 0; i < this.SortFilterSetting.SortTypes.Count; i++)
		{
			switch (this.SortFilterSetting.SortTypes[i])
			{
			case CombatSkillSortAndFilter.SortType.Grade:
			{
				bool flag9 = configL.Grade != configR.Grade;
				if (flag9)
				{
					return this.SortFilterSetting.IsDescSort[i] ? configR.Grade.CompareTo(configL.Grade) : configL.Grade.CompareTo(configR.Grade);
				}
				break;
			}
			case CombatSkillSortAndFilter.SortType.Name:
			{
				bool flag10 = configL.Name != configR.Name;
				if (flag10)
				{
					return (!this.SortFilterSetting.IsDescSort[i]) ? Utils_Sorting.CompareByCurrentLangEncoding(configL.Name, configR.Name) : Utils_Sorting.CompareByCurrentLangEncoding(configR.Name, configL.Name);
				}
				break;
			}
			case CombatSkillSortAndFilter.SortType.Power:
			{
				bool flag11 = skillL.Power != skillR.Power;
				if (flag11)
				{
					return this.SortFilterSetting.IsDescSort[i] ? skillR.Power.CompareTo(skillL.Power) : skillL.Power.CompareTo(skillR.Power);
				}
				break;
			}
			}
		}
		bool flag12 = configL.Type != configR.Type;
		int result;
		if (flag12)
		{
			result = configL.Type.CompareTo(configR.Type);
		}
		else
		{
			result = skillL.TemplateId.CompareTo(skillR.TemplateId);
		}
		return result;
	}

	// Token: 0x0600201B RID: 8219 RVA: 0x000E9FDC File Offset: 0x000E81DC
	public void SetSectFilterType(int key)
	{
		this._sectFilterTogGroup.Set(key, true, false);
	}

	// Token: 0x0600201C RID: 8220 RVA: 0x000E9FEE File Offset: 0x000E81EE
	public void SetTypeFilterType(int key, bool forceRaiseEvent = false)
	{
		this._typeFilterTogGroup.Set(key, true, forceRaiseEvent);
	}

	// Token: 0x04001821 RID: 6177
	public const sbyte SectFilterTypeCount = 16;

	// Token: 0x04001822 RID: 6178
	public const sbyte OtherSectFilterType = 17;

	// Token: 0x04001823 RID: 6179
	private List<CombatSkillDisplayData> _skillList;

	// Token: 0x04001824 RID: 6180
	private Action _onSkillListChanged;

	// Token: 0x04001825 RID: 6181
	private bool _inited = false;

	// Token: 0x04001826 RID: 6182
	[NonSerialized]
	public CombatSkillSortFilterSetting SortFilterSetting;

	// Token: 0x04001827 RID: 6183
	public readonly List<CombatSkillDisplayData> OutputSkillList = new List<CombatSkillDisplayData>();

	// Token: 0x04001828 RID: 6184
	public List<short> StaticAheadIdList = new List<short>();

	// Token: 0x04001829 RID: 6185
	public List<short> StaticLastIdList = new List<short>();

	// Token: 0x0400182A RID: 6186
	[Tooltip("五行筛选是否显示文本")]
	public bool showFiveElementLabel = true;

	// Token: 0x0400182B RID: 6187
	private RectTransform _sortBtnHolder;

	// Token: 0x0400182C RID: 6188
	private CToggleGroupObsolete _typeFilterTogGroup;

	// Token: 0x0400182D RID: 6189
	private CToggleGroupObsolete _sectFilterTogGroup;

	// Token: 0x0400182E RID: 6190
	private CToggleGroupObsolete _fiveElementFilter;

	// Token: 0x0400182F RID: 6191
	private MultiStateToggle _breakToggle;

	// Token: 0x04001830 RID: 6192
	private bool _sectTogInitializing = false;

	// Token: 0x04001831 RID: 6193
	private bool _addEmptyItem = false;

	// Token: 0x04001832 RID: 6194
	private Action<sbyte> _onTypeFilterChange;

	// Token: 0x04001833 RID: 6195
	private Action _onSortPreChange;

	// Token: 0x04001834 RID: 6196
	public bool AutoResetSect = true;

	// Token: 0x02001478 RID: 5240
	public enum SortType
	{
		// Token: 0x0400A15A RID: 41306
		Invalid = -1,
		// Token: 0x0400A15B RID: 41307
		Grade,
		// Token: 0x0400A15C RID: 41308
		Name,
		// Token: 0x0400A15D RID: 41309
		Power,
		// Token: 0x0400A15E RID: 41310
		Count
	}
}
