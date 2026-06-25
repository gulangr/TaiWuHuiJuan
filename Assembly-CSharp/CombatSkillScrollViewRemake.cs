using System;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;
using UnityEngine;

// Token: 0x020001E7 RID: 487
public class CombatSkillScrollViewRemake : Refers
{
	// Token: 0x1700033B RID: 827
	// (get) Token: 0x06001FF8 RID: 8184 RVA: 0x000E8D71 File Offset: 0x000E6F71
	// (set) Token: 0x06001FF9 RID: 8185 RVA: 0x000E8D79 File Offset: 0x000E6F79
	public CombatSkillSortAndFilter SortAndFilter { get; private set; }

	// Token: 0x06001FFA RID: 8186 RVA: 0x000E8D82 File Offset: 0x000E6F82
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x06001FFB RID: 8187 RVA: 0x000E8D8C File Offset: 0x000E6F8C
	public void Init()
	{
		bool inited = this._inited;
		if (!inited)
		{
			this.SortAndFilter = base.CGet<CombatSkillSortAndFilter>("SortAndFilter");
			this.SortAndFilter.Init();
			this._skillScroll = base.GetComponent<InfinityScrollLegacy>();
			this._skillScroll.OnItemRender = new Action<int, Refers>(this.OnRenderItem);
			this._skillScroll.OnItemHide = new Action<Refers>(this.OnHideItem);
			this._skillScroll.SetTogGroup(base.CGet<CToggleGroupObsolete>("ContentToggleGroup"), false, true);
			CToggleGroupObsolete togGroup = this._skillScroll.TogGroup;
			togGroup.OnActiveToggleChange = (Action<CToggleObsolete, CToggleObsolete>)Delegate.Combine(togGroup.OnActiveToggleChange, new Action<CToggleObsolete, CToggleObsolete>(this.ActiveToggleChange));
			this._inited = true;
		}
	}

	// Token: 0x06001FFC RID: 8188 RVA: 0x000E8E4D File Offset: 0x000E704D
	private void ActiveToggleChange(CToggleObsolete newTog, CToggleObsolete oldTog)
	{
		Action<CToggleObsolete, CToggleObsolete> onActiveToggleChange = this.OnActiveToggleChange;
		if (onActiveToggleChange != null)
		{
			onActiveToggleChange(newTog, oldTog);
		}
	}

	// Token: 0x06001FFD RID: 8189 RVA: 0x000E8E64 File Offset: 0x000E7064
	public void SetCombatSkillList(List<CombatSkillDisplayData> skillList, bool reset = false, bool interactable = true, string listTag = null, Action<CombatSkillDisplayData, CommonCombatSkill> onRenderSkill = null, bool addEmptyItem = false, bool isShowNeiLiFinish = false, GameObject customEmptyObject = null, bool scrollToTopWhenListCountChanged = true)
	{
		if (reset)
		{
			this._onRenderSkill = onRenderSkill;
			this._listTag = listTag;
			this._interactable = interactable;
		}
		this._skillScroll.SelectedTogKey = -1;
		this._skillScroll.TogGroup.DeSelectAll(false);
		this._isShowNeiLiFinish = isShowNeiLiFinish;
		this._customEmptyObject = customEmptyObject;
		this._scrollToTopWhenCountChanged = scrollToTopWhenListCountChanged;
		this.SortAndFilter.SetSkillList(skillList, reset, listTag, new Action(this.OnSkillListChanged), addEmptyItem);
	}

	// Token: 0x06001FFE RID: 8190 RVA: 0x000E8EE8 File Offset: 0x000E70E8
	public void SetLifeSkillList(int amount, bool reset = false, bool interactable = true, string listTag = null, Action<int, Refers> onRenderLifeSkill = null, bool addEmptyItem = false, bool isShowNeiLiFinish = false, GameObject customEmptyObject = null, bool scrollToTopWhenListCountChanged = true)
	{
		if (reset)
		{
			this._listTag = listTag;
			this._interactable = interactable;
		}
		this._onRenderLifeSkill = onRenderLifeSkill;
		this._skillScroll.SelectedTogKey = -1;
		this._skillScroll.TogGroup.DeSelectAll(false);
		this._isShowNeiLiFinish = isShowNeiLiFinish;
		this._customEmptyObject = customEmptyObject;
		this._scrollToTopWhenCountChanged = scrollToTopWhenListCountChanged;
		this._skillScroll.UpdateData(amount);
	}

	// Token: 0x06001FFF RID: 8191 RVA: 0x000E8F58 File Offset: 0x000E7158
	public void ScrollToTop()
	{
		this._skillScroll.ScrollTo(0, 0.3f);
	}

	// Token: 0x06002000 RID: 8192 RVA: 0x000E8F6D File Offset: 0x000E716D
	public void ScrollTo(int index)
	{
		this._skillScroll.ScrollTo(index, 0.3f);
	}

	// Token: 0x06002001 RID: 8193 RVA: 0x000E8F82 File Offset: 0x000E7182
	public void ReRender()
	{
		this._skillScroll.ReRender();
	}

	// Token: 0x06002002 RID: 8194 RVA: 0x000E8F94 File Offset: 0x000E7194
	public void RefreshCell(short skillTemplateId)
	{
		int index = 0;
		bool flag = skillTemplateId > -1;
		if (flag)
		{
			index = this.SortAndFilter.OutputSkillList.FindIndex((CombatSkillDisplayData data) => data != null && data.TemplateId == skillTemplateId);
		}
		bool flag2 = this.SortAndFilter.OutputSkillList.CheckIndex(index);
		if (flag2)
		{
			this._skillScroll.RefreshCell(index);
		}
	}

	// Token: 0x06002003 RID: 8195 RVA: 0x000E9000 File Offset: 0x000E7200
	public CommonCombatSkill FindActiveItem(short skillId)
	{
		int index = this.SortAndFilter.OutputSkillList.FindIndex((CombatSkillDisplayData data) => data.TemplateId == skillId);
		return (index >= 0) ? (this._skillScroll.GetActiveCell(index) as CommonCombatSkill) : null;
	}

	// Token: 0x06002004 RID: 8196 RVA: 0x000E9054 File Offset: 0x000E7254
	public void SaveSortFilterSetting(bool saveGlobalSettings = true)
	{
		bool flag = this._listTag != null;
		if (flag)
		{
			SingletonObject.getInstance<GameSort>().SetCombatSkillSortConfig(this._listTag, this.SortAndFilter.SortFilterSetting);
		}
	}

	// Token: 0x06002005 RID: 8197 RVA: 0x000E908C File Offset: 0x000E728C
	private void OnSkillListChanged()
	{
		int prevCount = this._skillScroll.CurrentDataCount;
		this._skillScroll.UpdateData(this.SortAndFilter.OutputSkillList.Count);
		bool flag = this._customEmptyObject;
		if (flag)
		{
			this._customEmptyObject.SetActive(this.SortAndFilter.OutputSkillList.Count == 0);
		}
		int currCount = this._skillScroll.CurrentDataCount;
		bool flag2 = prevCount != currCount && this._scrollToTopWhenCountChanged;
		if (flag2)
		{
			this.ScrollToTop();
		}
		bool isEmpty = this.SortAndFilter.GetAllSkillList().Count <= 0;
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

	// Token: 0x06002006 RID: 8198 RVA: 0x000E916C File Offset: 0x000E736C
	private void OnRenderItem(int index, Refers skillRefers)
	{
		bool flag = this._onRenderLifeSkill != null;
		if (flag)
		{
			Action<int, Refers> onRenderLifeSkill = this._onRenderLifeSkill;
			if (onRenderLifeSkill != null)
			{
				onRenderLifeSkill(index, skillRefers);
			}
		}
		else
		{
			CombatSkillDisplayData skillData = this.SortAndFilter.OutputSkillList[index];
			CommonCombatSkill skillView = skillRefers as CommonCombatSkill;
			RectTransform itemTransform = skillView.GetComponent<RectTransform>();
			skillView.Refresh(skillData);
			skillView.hover.SetActive(this._interactable && itemTransform.rect.Contains(UIManager.Instance.MousePosToLocalPos(itemTransform)));
			Action<CombatSkillDisplayData, CommonCombatSkill> onRenderSkill = this._onRenderSkill;
			if (onRenderSkill != null)
			{
				onRenderSkill(skillData, skillView);
			}
		}
	}

	// Token: 0x06002007 RID: 8199 RVA: 0x000E920C File Offset: 0x000E740C
	private void OnHideItem(Refers skillRefers)
	{
		bool flag = skillRefers.Names.Contains("GrayRoot");
		if (flag)
		{
			skillRefers.CGet<DisableStyleRoot>("GrayRoot").SetStyleEffect(false, false);
		}
		bool flag2 = skillRefers.Names.Contains("FinishIcon");
		if (flag2)
		{
			skillRefers.CGet<GameObject>("FinishIcon").SetActive(false);
		}
	}

	// Token: 0x04001816 RID: 6166
	private string _listTag;

	// Token: 0x04001817 RID: 6167
	private InfinityScrollLegacy _skillScroll;

	// Token: 0x04001818 RID: 6168
	private Action<CombatSkillDisplayData, CommonCombatSkill> _onRenderSkill;

	// Token: 0x04001819 RID: 6169
	private Action<int, Refers> _onRenderLifeSkill;

	// Token: 0x0400181A RID: 6170
	private bool _interactable;

	// Token: 0x0400181B RID: 6171
	private bool _inited = false;

	// Token: 0x0400181C RID: 6172
	private bool _isShowNeiLiFinish = false;

	// Token: 0x0400181D RID: 6173
	private bool _scrollToTopWhenCountChanged = true;

	// Token: 0x0400181E RID: 6174
	private GameObject _customEmptyObject;

	// Token: 0x0400181F RID: 6175
	[HideInInspector]
	public Action OnSkillListChangeFinal;

	// Token: 0x04001820 RID: 6176
	[HideInInspector]
	public Action<CToggleObsolete, CToggleObsolete> OnActiveToggleChange;
}
