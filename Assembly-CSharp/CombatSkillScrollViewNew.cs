using System;
using System.Collections.Generic;
using CommonSortAndFilterLegacy;
using CommonSortAndFilterLegacy.CombatSkill;
using FrameWork.UI.LanguageRule;
using GameData.Domains.CombatSkill;
using UnityEngine;

// Token: 0x020001E6 RID: 486
public class CombatSkillScrollViewNew : Refers, ILanguage
{
	// Token: 0x1700033A RID: 826
	// (get) Token: 0x06001FE7 RID: 8167 RVA: 0x000E89AD File Offset: 0x000E6BAD
	// (set) Token: 0x06001FE8 RID: 8168 RVA: 0x000E89B5 File Offset: 0x000E6BB5
	public CharacterMenuCombatSkillSortAndFilterController SortAndFilter { get; private set; }

	// Token: 0x06001FE9 RID: 8169 RVA: 0x000E89BE File Offset: 0x000E6BBE
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x06001FEA RID: 8170 RVA: 0x000E89C8 File Offset: 0x000E6BC8
	public void Init()
	{
		bool inited = this._inited;
		if (!inited)
		{
			this._commonSortAndFilter = base.CGet<CommonSortAndFilter>("SortAndFilter");
			this.SortAndFilter = new CharacterMenuCombatSkillSortAndFilterController(this._commonSortAndFilter);
			this.SortAndFilter.Init(new Action(this.OnSkillListChanged), "CharacterMenuCombatSkillSortAndFilter");
			this._skillScroll = base.CGet<InfinityScrollLegacy>("VerticalScrollView");
			this._skillScroll.OnItemRender = new Action<int, Refers>(this.OnRenderItem);
			this._skillScroll.OnItemHide = new Action<Refers>(this.OnHideItem);
			this._skillScroll.OnLanguageChange = new Action<Refers, LocalStringManager.LanguageType>(this.OnLanguageChange);
			this._inited = true;
		}
	}

	// Token: 0x06001FEB RID: 8171 RVA: 0x000E8A84 File Offset: 0x000E6C84
	public void SetCombatSkillList(List<CombatSkillDisplayData> skillList, bool reset = false, bool interactable = true, string listTag = null, Action<CombatSkillDisplayData, CommonCombatSkill> onRenderSkill = null, bool addEmptyItem = false, bool isShowNeiLiFinish = false, GameObject customEmptyObject = null, bool scrollToTopWhenListCountChanged = true)
	{
		if (reset)
		{
			this._onRenderSkill = onRenderSkill;
			this._listTag = listTag;
			this._interactable = interactable;
		}
		this._isShowNeiLiFinish = isShowNeiLiFinish;
		this._customEmptyObject = customEmptyObject;
		this._scrollToTopWhenCountChanged = scrollToTopWhenListCountChanged;
		this.SortAndFilter.SetDataList(skillList, true);
	}

	// Token: 0x06001FEC RID: 8172 RVA: 0x000E8AD6 File Offset: 0x000E6CD6
	public void ScrollToTop()
	{
		this._skillScroll.ScrollTo(0, 0.3f);
	}

	// Token: 0x06001FED RID: 8173 RVA: 0x000E8AEB File Offset: 0x000E6CEB
	public void ScrollTo(int index)
	{
		this._skillScroll.ScrollTo(index, 0.3f);
	}

	// Token: 0x06001FEE RID: 8174 RVA: 0x000E8B00 File Offset: 0x000E6D00
	public void ReRender()
	{
		this._skillScroll.ReRender();
	}

	// Token: 0x06001FEF RID: 8175 RVA: 0x000E8B10 File Offset: 0x000E6D10
	public void RefreshCell(short skillTemplateId)
	{
		int index = 0;
		bool flag = skillTemplateId > -1;
		if (flag)
		{
			index = this.SortAndFilter.OutputDataList.FindIndex((CombatSkillDisplayData data) => data != null && data.TemplateId == skillTemplateId);
		}
		bool flag2 = this.SortAndFilter.OutputDataList.CheckIndex(index);
		if (flag2)
		{
			this._skillScroll.RefreshCell(index);
		}
	}

	// Token: 0x06001FF0 RID: 8176 RVA: 0x000E8B7C File Offset: 0x000E6D7C
	public CombatSkillView FindActiveItem(short skillId)
	{
		int index = this.SortAndFilter.OutputDataList.FindIndex((CombatSkillDisplayData data) => data.TemplateId == skillId);
		return (index >= 0) ? (this._skillScroll.GetActiveCell(index) as CombatSkillView) : null;
	}

	// Token: 0x06001FF1 RID: 8177 RVA: 0x000E8BD0 File Offset: 0x000E6DD0
	public void SaveSortFilterSetting(bool saveGlobalSettings = true)
	{
	}

	// Token: 0x06001FF2 RID: 8178 RVA: 0x000E8BD4 File Offset: 0x000E6DD4
	private void OnSkillListChanged()
	{
		int prevCount = this._skillScroll.CurrentDataCount;
		this._skillScroll.UpdateData(this.SortAndFilter.OutputDataList.Count);
		bool flag = this._customEmptyObject;
		if (flag)
		{
			this._customEmptyObject.SetActive(this.SortAndFilter.OutputDataList.Count == 0);
		}
		int currCount = this._skillScroll.CurrentDataCount;
		bool flag2 = prevCount != currCount && this._scrollToTopWhenCountChanged;
		if (flag2)
		{
			this.ScrollToTop();
		}
		bool isEmpty = this.SortAndFilter.IsEmpty;
		bool flag3 = null != this._skillScroll.EmptyObject;
		if (flag3)
		{
			this._skillScroll.EmptyObject.SetActive(isEmpty);
		}
		Action onSkillListChangeFinal = this.OnSkillListChangeFinal;
		if (onSkillListChangeFinal != null)
		{
			onSkillListChangeFinal();
		}
	}

	// Token: 0x06001FF3 RID: 8179 RVA: 0x000E8CA8 File Offset: 0x000E6EA8
	private void OnRenderItem(int index, Refers skillRefers)
	{
		CombatSkillDisplayData skillData = this.SortAndFilter.OutputDataList[index];
		CommonCombatSkill skillView = skillRefers as CommonCombatSkill;
		RectTransform itemTransform = skillView.GetComponent<RectTransform>();
		skillView.Refresh(skillData);
		Action<CombatSkillDisplayData, CommonCombatSkill> onRenderSkill = this._onRenderSkill;
		if (onRenderSkill != null)
		{
			onRenderSkill(skillData, skillView);
		}
	}

	// Token: 0x06001FF4 RID: 8180 RVA: 0x000E8CF4 File Offset: 0x000E6EF4
	private void OnHideItem(Refers skillRefers)
	{
		CommonCombatSkill skillView = skillRefers as CommonCombatSkill;
		skillView.toggle.isOn = false;
	}

	// Token: 0x06001FF5 RID: 8181 RVA: 0x000E8D16 File Offset: 0x000E6F16
	private void OnLanguageChange(Refers refers, LocalStringManager.LanguageType languageType)
	{
		refers.CGet<LanguageRuleTips>("SkillName").OnLanguageChange(languageType);
	}

	// Token: 0x06001FF6 RID: 8182 RVA: 0x000E8D2C File Offset: 0x000E6F2C
	public void OnLanguageChange(LocalStringManager.LanguageType languageType)
	{
		ILanguage iLanguage = this._skillScroll;
		bool flag = iLanguage != null;
		if (flag)
		{
			iLanguage.OnLanguageChange(languageType);
		}
	}

	// Token: 0x0400180B RID: 6155
	private CommonSortAndFilter _commonSortAndFilter;

	// Token: 0x0400180C RID: 6156
	private string _listTag;

	// Token: 0x0400180D RID: 6157
	private InfinityScrollLegacy _skillScroll;

	// Token: 0x0400180E RID: 6158
	private Action<CombatSkillDisplayData, CommonCombatSkill> _onRenderSkill;

	// Token: 0x0400180F RID: 6159
	private bool _interactable;

	// Token: 0x04001810 RID: 6160
	private bool _inited = false;

	// Token: 0x04001811 RID: 6161
	private bool _isShowNeiLiFinish = false;

	// Token: 0x04001812 RID: 6162
	private bool _scrollToTopWhenCountChanged = true;

	// Token: 0x04001813 RID: 6163
	private GameObject _customEmptyObject;

	// Token: 0x04001814 RID: 6164
	public Action OnSkillListChangeFinal;
}
