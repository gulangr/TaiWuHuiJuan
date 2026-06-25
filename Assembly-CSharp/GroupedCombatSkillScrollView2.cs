using System;
using System.Collections.Generic;
using CommonSortAndFilterLegacy;
using CommonSortAndFilterLegacy.CombatSkill;
using GameData.Domains.CombatSkill;
using TMPro;
using UnityEngine;

// Token: 0x020001EF RID: 495
public class GroupedCombatSkillScrollView2 : MonoBehaviour
{
	// Token: 0x17000341 RID: 833
	// (get) Token: 0x06002068 RID: 8296 RVA: 0x000EC041 File Offset: 0x000EA241
	public List<CombatSkillDisplayData> OutputSkillList
	{
		get
		{
			CombatSkillSortAndFilterController sortAndFilterController = this._sortAndFilterController;
			return ((sortAndFilterController != null) ? sortAndFilterController.OutputDataList : null) ?? new List<CombatSkillDisplayData>();
		}
	}

	// Token: 0x06002069 RID: 8297 RVA: 0x000EC060 File Offset: 0x000EA260
	public void Init(string sortSaveKey = "CombatSkill")
	{
		bool inited = this._inited;
		if (!inited)
		{
			this._sortAndFilterController = new CombatSkillSortAndFilterController(this.sortAndFilter);
			this._sortAndFilterController.Init(new Action(this.OnSkillListChanged), sortSaveKey);
			this.scrollView.Init();
			this.scrollView.OnGroupTitleRender = new Action<int, Refers>(this.OnRenderTitleInternal);
			this.scrollView.OnItemRender = new Action<int, int, Refers>(this.OnRenderSkillInternal);
			this._inited = true;
		}
	}

	// Token: 0x0600206A RID: 8298 RVA: 0x000EC0E8 File Offset: 0x000EA2E8
	public void SetSkillList(List<CombatSkillDisplayData> skillList, GroupedCombatSkillScrollView2.CombatSkillGroupGetter groupGetter = null, Action<CombatSkillDisplayData, Refers> onRenderSkill = null, Action<GroupedCombatSkillScrollView2.CombatSkillGroup, Refers> onRenderTitle = null, bool isReset = false, bool forceRefresh = false)
	{
		this._forceRefresh = forceRefresh;
		if (isReset)
		{
			this._onRenderSkill = onRenderSkill;
			this._onRenderTitle = onRenderTitle;
			bool flag = groupGetter != null;
			if (flag)
			{
				this._groupGetter = groupGetter;
			}
			bool flag2 = this._groupGetter == null;
			if (flag2)
			{
				throw new Exception("GroupGetter is null");
			}
			this.ResetScroll();
		}
		this._sortAndFilterController.SetDataList(skillList, true);
		this._forceRefresh = false;
	}

	// Token: 0x0600206B RID: 8299 RVA: 0x000EC158 File Offset: 0x000EA358
	public void ReRender()
	{
		this.scrollView.ReRender();
	}

	// Token: 0x0600206C RID: 8300 RVA: 0x000EC167 File Offset: 0x000EA367
	public void ResetScroll()
	{
		this.scrollView.UpdateData(this._scrollDataList, true);
	}

	// Token: 0x0600206D RID: 8301 RVA: 0x000EC180 File Offset: 0x000EA380
	private void OnSkillListChanged()
	{
		bool flag = this.EnableDataChangeCheck && this._inited;
		if (flag)
		{
			this._oldSkillGroups.Clear();
			this._oldSkillGroups.AddRange(this._skillGroups);
		}
		this._skillGroups.Clear();
		bool flag2 = this._groupGetter != null;
		if (flag2)
		{
			this._skillGroups.AddRange(this._groupGetter(this._sortAndFilterController.OutputDataList));
		}
		bool shouldUpdate = !this._inited || !this.EnableDataChangeCheck || this._forceRefresh;
		bool flag3 = !shouldUpdate && this.EnableDataChangeCheck && this._inited;
		if (flag3)
		{
			shouldUpdate = !this.AreSkillGroupsEqual(this._oldSkillGroups, this._skillGroups);
		}
		bool flag4 = shouldUpdate;
		if (flag4)
		{
			this.UpdateScrollDataList(this._skillGroups);
			this.scrollView.UpdateData(this._scrollDataList, !this._inited);
		}
		Action skillListChangedAction = this.SkillListChangedAction;
		if (skillListChangedAction != null)
		{
			skillListChangedAction();
		}
	}

	// Token: 0x0600206E RID: 8302 RVA: 0x000EC290 File Offset: 0x000EA490
	private void UpdateScrollDataList(List<GroupedCombatSkillScrollView2.CombatSkillGroup> skillGroups)
	{
		this._scrollDataList.Clear();
		foreach (GroupedCombatSkillScrollView2.CombatSkillGroup skillGroup in skillGroups)
		{
			GroupedInfinityScroll.GroupItem groupItem = new GroupedInfinityScroll.GroupItem(skillGroup.GroupId, skillGroup.SkillList.Count);
			this._scrollDataList.Add(groupItem);
		}
	}

	// Token: 0x0600206F RID: 8303 RVA: 0x000EC310 File Offset: 0x000EA510
	private void OnRenderTitleInternal(int groupIndex, Refers refers)
	{
		bool flag = groupIndex >= 0 && groupIndex < this._skillGroups.Count;
		if (flag)
		{
			GroupedCombatSkillScrollView2.CombatSkillGroup skillGroup = this._skillGroups[groupIndex];
			TextMeshProUGUI titleName = refers.CGet<TextMeshProUGUI>("TitleName");
			bool flag2 = titleName != null;
			if (flag2)
			{
				titleName.text = skillGroup.GroupTitleText.ColorReplace();
			}
			Action<GroupedCombatSkillScrollView2.CombatSkillGroup, Refers> onRenderTitle = this._onRenderTitle;
			if (onRenderTitle != null)
			{
				onRenderTitle(skillGroup, refers);
			}
		}
	}

	// Token: 0x06002070 RID: 8304 RVA: 0x000EC384 File Offset: 0x000EA584
	private void OnRenderSkillInternal(int groupIndex, int index, Refers refers)
	{
		bool flag = groupIndex >= 0 && groupIndex < this._skillGroups.Count && index >= 0 && index < this._skillGroups[groupIndex].SkillList.Count;
		if (flag)
		{
			CombatSkillDisplayData skillData = this._skillGroups[groupIndex].SkillList[index];
			CommonCombatSkill commonCombatSkill = refers.CGet<CommonCombatSkill>("CommonCombatSkill");
			bool flag2 = commonCombatSkill != null;
			if (flag2)
			{
				commonCombatSkill.Refresh(skillData);
			}
			Action<CombatSkillDisplayData, Refers> onRenderSkill = this._onRenderSkill;
			if (onRenderSkill != null)
			{
				onRenderSkill(skillData, refers);
			}
		}
	}

	// Token: 0x06002071 RID: 8305 RVA: 0x000EC418 File Offset: 0x000EA618
	private bool AreSkillGroupsEqual(List<GroupedCombatSkillScrollView2.CombatSkillGroup> oldGroups, List<GroupedCombatSkillScrollView2.CombatSkillGroup> newGroups)
	{
		bool flag = oldGroups.Count != newGroups.Count;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			for (int i = 0; i < oldGroups.Count; i++)
			{
				GroupedCombatSkillScrollView2.CombatSkillGroup oldGroup = oldGroups[i];
				GroupedCombatSkillScrollView2.CombatSkillGroup newGroup = newGroups[i];
				bool flag2 = oldGroup.GroupId != newGroup.GroupId || oldGroup.GroupTitleText != newGroup.GroupTitleText;
				if (flag2)
				{
					return false;
				}
				bool flag3 = !this.AreSkillListsEqual(oldGroup.SkillList, newGroup.SkillList);
				if (flag3)
				{
					return false;
				}
			}
			result = true;
		}
		return result;
	}

	// Token: 0x06002072 RID: 8306 RVA: 0x000EC4BC File Offset: 0x000EA6BC
	private bool AreSkillListsEqual(List<CombatSkillDisplayData> oldList, List<CombatSkillDisplayData> newList)
	{
		bool flag = oldList.Count != newList.Count;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			for (int i = 0; i < oldList.Count; i++)
			{
				bool flag2 = !this.AreSkillsEqual(oldList[i], newList[i]);
				if (flag2)
				{
					return false;
				}
			}
			result = true;
		}
		return result;
	}

	// Token: 0x06002073 RID: 8307 RVA: 0x000EC520 File Offset: 0x000EA720
	private bool AreSkillsEqual(CombatSkillDisplayData oldSkill, CombatSkillDisplayData newSkill)
	{
		bool flag = oldSkill == null && newSkill == null;
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			bool flag2 = oldSkill == null || newSkill == null;
			result = (!flag2 && (oldSkill.CharId == newSkill.CharId && oldSkill.TemplateId == newSkill.TemplateId && oldSkill.Mastered == newSkill.Mastered && oldSkill.Revoked == newSkill.Revoked && oldSkill.Power == newSkill.Power && oldSkill.GridCount == newSkill.GridCount && oldSkill.CanAffect == newSkill.CanAffect && oldSkill.Conflicting == newSkill.Conflicting && oldSkill.ActivationState == newSkill.ActivationState) && oldSkill.ReadingState == newSkill.ReadingState);
		}
		return result;
	}

	// Token: 0x04001875 RID: 6261
	[SerializeField]
	private CommonSortAndFilter sortAndFilter;

	// Token: 0x04001876 RID: 6262
	[SerializeField]
	private GroupedInfinityScroll scrollView;

	// Token: 0x04001877 RID: 6263
	private CombatSkillSortAndFilterController _sortAndFilterController;

	// Token: 0x04001878 RID: 6264
	private bool _inited;

	// Token: 0x04001879 RID: 6265
	private readonly List<GroupedCombatSkillScrollView2.CombatSkillGroup> _skillGroups = new List<GroupedCombatSkillScrollView2.CombatSkillGroup>();

	// Token: 0x0400187A RID: 6266
	private readonly List<GroupedInfinityScroll.GroupItem> _scrollDataList = new List<GroupedInfinityScroll.GroupItem>();

	// Token: 0x0400187B RID: 6267
	private readonly List<GroupedCombatSkillScrollView2.CombatSkillGroup> _oldSkillGroups = new List<GroupedCombatSkillScrollView2.CombatSkillGroup>();

	// Token: 0x0400187C RID: 6268
	private GroupedCombatSkillScrollView2.CombatSkillGroupGetter _groupGetter;

	// Token: 0x0400187D RID: 6269
	private Action<CombatSkillDisplayData, Refers> _onRenderSkill;

	// Token: 0x0400187E RID: 6270
	private Action<GroupedCombatSkillScrollView2.CombatSkillGroup, Refers> _onRenderTitle;

	// Token: 0x0400187F RID: 6271
	private bool _forceRefresh;

	// Token: 0x04001880 RID: 6272
	public Action SkillListChangedAction;

	// Token: 0x04001881 RID: 6273
	public bool EnableDataChangeCheck;

	// Token: 0x02001480 RID: 5248
	// (Invoke) Token: 0x0600CC13 RID: 52243
	public delegate List<GroupedCombatSkillScrollView2.CombatSkillGroup> CombatSkillGroupGetter(List<CombatSkillDisplayData> skillList);

	// Token: 0x02001481 RID: 5249
	public struct CombatSkillGroup
	{
		// Token: 0x0600CC16 RID: 52246 RVA: 0x005957F9 File Offset: 0x005939F9
		public CombatSkillGroup(int groupId, string groupTitleText)
		{
			this.GroupId = groupId;
			this.GroupTitleText = groupTitleText;
			this.SkillList = new List<CombatSkillDisplayData>();
		}

		// Token: 0x0400A171 RID: 41329
		public int GroupId;

		// Token: 0x0400A172 RID: 41330
		public string GroupTitleText;

		// Token: 0x0400A173 RID: 41331
		public List<CombatSkillDisplayData> SkillList;
	}
}
