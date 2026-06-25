using System;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;
using UnityEngine;

// Token: 0x020001E5 RID: 485
public class CombatSkillScrollView : Refers
{
	// Token: 0x17000339 RID: 825
	// (get) Token: 0x06001FD8 RID: 8152 RVA: 0x000E8580 File Offset: 0x000E6780
	// (set) Token: 0x06001FD9 RID: 8153 RVA: 0x000E8588 File Offset: 0x000E6788
	public CombatSkillSortAndFilter SortAndFilter { get; private set; }

	// Token: 0x06001FDA RID: 8154 RVA: 0x000E8591 File Offset: 0x000E6791
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x06001FDB RID: 8155 RVA: 0x000E859C File Offset: 0x000E679C
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
			this._inited = true;
		}
	}

	// Token: 0x06001FDC RID: 8156 RVA: 0x000E8618 File Offset: 0x000E6818
	public void SetCombatSkillList(List<CombatSkillDisplayData> skillList, bool reset = false, bool interactable = true, string listTag = null, Action<CombatSkillDisplayData, CombatSkillView> onRenderSkill = null, bool addEmptyItem = false, bool isShowNeiLiFinish = false, GameObject customEmptyObject = null, bool scrollToTopWhenListCountChanged = true)
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
		this.SortAndFilter.SetSkillList(skillList, reset, listTag, new Action(this.OnSkillListChanged), addEmptyItem);
	}

	// Token: 0x06001FDD RID: 8157 RVA: 0x000E867A File Offset: 0x000E687A
	public void ScrollToTop()
	{
		this._skillScroll.ScrollTo(0, 0.3f);
	}

	// Token: 0x06001FDE RID: 8158 RVA: 0x000E868F File Offset: 0x000E688F
	public void ScrollTo(int index)
	{
		this._skillScroll.ScrollTo(index, 0.3f);
	}

	// Token: 0x06001FDF RID: 8159 RVA: 0x000E86A4 File Offset: 0x000E68A4
	public void ReRender()
	{
		this._skillScroll.ReRender();
	}

	// Token: 0x06001FE0 RID: 8160 RVA: 0x000E86B4 File Offset: 0x000E68B4
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

	// Token: 0x06001FE1 RID: 8161 RVA: 0x000E8720 File Offset: 0x000E6920
	public CombatSkillView FindActiveItem(short skillId)
	{
		int index = this.SortAndFilter.OutputSkillList.FindIndex((CombatSkillDisplayData data) => data.TemplateId == skillId);
		return (index >= 0) ? (this._skillScroll.GetActiveCell(index) as CombatSkillView) : null;
	}

	// Token: 0x06001FE2 RID: 8162 RVA: 0x000E8774 File Offset: 0x000E6974
	public void SaveSortFilterSetting(bool saveGlobalSettings = true)
	{
		bool flag = this._listTag != null;
		if (flag)
		{
			SingletonObject.getInstance<GameSort>().SetCombatSkillSortConfig(this._listTag, this.SortAndFilter.SortFilterSetting);
		}
	}

	// Token: 0x06001FE3 RID: 8163 RVA: 0x000E87AC File Offset: 0x000E69AC
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

	// Token: 0x06001FE4 RID: 8164 RVA: 0x000E888C File Offset: 0x000E6A8C
	private void OnRenderItem(int index, Refers skillRefers)
	{
		CombatSkillDisplayData skillData = this.SortAndFilter.OutputSkillList[index];
		CombatSkillView skillView = skillRefers as CombatSkillView;
		RectTransform itemTransform = skillView.GetComponent<RectTransform>();
		skillView.SetData(skillData, this._interactable, false, true, skillData != null && this._isShowNeiLiFinish);
		skillView.RefreshMouseTip();
		skillView.CGet<GameObject>("MouseOver").gameObject.SetActive(this._interactable && itemTransform.rect.Contains(UIManager.Instance.MousePosToLocalPos(itemTransform)));
		skillView.SetChecked(false);
		Action<CombatSkillDisplayData, CombatSkillView> onRenderSkill = this._onRenderSkill;
		if (onRenderSkill != null)
		{
			onRenderSkill(skillData, skillView);
		}
	}

	// Token: 0x06001FE5 RID: 8165 RVA: 0x000E8934 File Offset: 0x000E6B34
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

	// Token: 0x04001801 RID: 6145
	private string _listTag;

	// Token: 0x04001802 RID: 6146
	private InfinityScrollLegacy _skillScroll;

	// Token: 0x04001803 RID: 6147
	private Action<CombatSkillDisplayData, CombatSkillView> _onRenderSkill;

	// Token: 0x04001804 RID: 6148
	private bool _interactable;

	// Token: 0x04001805 RID: 6149
	private bool _inited = false;

	// Token: 0x04001806 RID: 6150
	private bool _isShowNeiLiFinish = false;

	// Token: 0x04001807 RID: 6151
	private bool _scrollToTopWhenCountChanged = true;

	// Token: 0x04001808 RID: 6152
	private GameObject _customEmptyObject;

	// Token: 0x04001809 RID: 6153
	public Action OnSkillListChangeFinal;
}
