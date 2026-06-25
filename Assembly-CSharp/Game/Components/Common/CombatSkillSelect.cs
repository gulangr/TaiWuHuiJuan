using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.CombatSkill.Simplified;
using GameData.Domains.CombatSkill;
using UnityEngine;

namespace Game.Components.Common
{
	// Token: 0x02000F89 RID: 3977
	public class CombatSkillSelect : MonoBehaviour
	{
		// Token: 0x0600B707 RID: 46855 RVA: 0x00536C28 File Offset: 0x00534E28
		public bool Contains(short templateId)
		{
			foreach (CombatSkillDisplayDataForList data in this._filteredData)
			{
				bool flag = data.TemplateId == templateId;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600B708 RID: 46856 RVA: 0x00536C90 File Offset: 0x00534E90
		public void Init(string saveKey, bool autoSelectFirst = true)
		{
			this._sortAndFilterController = new SimplifiedCombatSkillSortAndFilterController(this.skillSortAndFilter);
			this._sortAndFilterController.Init(new Action(this.OnSortAndFilterChange), saveKey);
			this.skillScroll.OnItemRender += this.OnSkillRender;
			this._autoSelectFirst = autoSelectFirst;
			CImage cimage = this.emptyIcon;
			if (cimage != null)
			{
				cimage.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600B709 RID: 46857 RVA: 0x00536CFF File Offset: 0x00534EFF
		public void Set(List<CombatSkillDisplayDataForList> skillListData, Action<short> onSelect)
		{
			this._skillListData = skillListData;
			this._onSelect = onSelect;
			this.UpdateSortAndFilter();
		}

		// Token: 0x0600B70A RID: 46858 RVA: 0x00536D18 File Offset: 0x00534F18
		public void SetCell(CombatSkillDisplayDataForList data)
		{
			for (int index = 0; index < this._skillListData.Count; index++)
			{
				bool flag = this._skillListData[index].TemplateId == data.TemplateId;
				if (flag)
				{
					this._skillListData[index] = data;
					break;
				}
			}
			for (int index2 = 0; index2 < this._filteredData.Count; index2++)
			{
				bool flag2 = this._filteredData[index2].TemplateId == data.TemplateId;
				if (flag2)
				{
					this._filteredData[index2] = data;
					this.skillScroll.RefreshCell(index2);
					break;
				}
			}
		}

		// Token: 0x0600B70B RID: 46859 RVA: 0x00536DCB File Offset: 0x00534FCB
		public void SelectByClick(short templateId)
		{
			this.OnSelect(templateId, true, true);
		}

		// Token: 0x0600B70C RID: 46860 RVA: 0x00536DD8 File Offset: 0x00534FD8
		public void Select(short templateId)
		{
			this.OnSelect(templateId, true, false);
		}

		// Token: 0x0600B70D RID: 46861 RVA: 0x00536DE5 File Offset: 0x00534FE5
		public void SelectWithoutNotify(short templateId)
		{
			this.OnSelect(templateId, false, false);
		}

		// Token: 0x0600B70E RID: 46862 RVA: 0x00536DF4 File Offset: 0x00534FF4
		private void UpdateSortAndFilter()
		{
			this._filteredData.Clear();
			Func<CombatSkillDisplayDataForList, bool> filter = this._sortAndFilterController.GenerateFilter();
			foreach (CombatSkillDisplayDataForList item in this._skillListData)
			{
				bool flag = filter(item);
				if (flag)
				{
					this._filteredData.Add(item);
				}
			}
			Comparison<CombatSkillDisplayDataForList> comparer = this._sortAndFilterController.GenerateComparer(this._filteredData);
			this._filteredData.Sort(comparer);
			this._sortAndFilterController.AfterFilter(this._skillListData);
			this.skillScroll.UpdateData(this._filteredData.Count);
			CImage cimage = this.emptyIcon;
			if (cimage != null)
			{
				cimage.gameObject.SetActive(this._filteredData.Count <= 0);
			}
			for (int i = 0; i < this._filteredData.Count; i++)
			{
				bool flag2 = this._filteredData[i].TemplateId == this.SelectedTemplateId;
				if (flag2)
				{
					this.skillScroll.ScrollTo(i, 0.3f);
					break;
				}
			}
			bool flag3 = this._autoSelectFirst && this.SelectedTemplateId < 0 && this._filteredData.Count > 0;
			if (flag3)
			{
				this.Select(this._filteredData[0].TemplateId);
			}
		}

		// Token: 0x0600B70F RID: 46863 RVA: 0x00536F80 File Offset: 0x00535180
		private void OnSortAndFilterChange()
		{
			this.UpdateSortAndFilter();
		}

		// Token: 0x0600B710 RID: 46864 RVA: 0x00536F8C File Offset: 0x0053518C
		private void OnSkillRender(int index, GameObject obj)
		{
			CombatSkillSelectable item = obj.GetComponent<CombatSkillSelectable>();
			item.Set(this._filteredData[index], new Action<short>(this.SelectByClick), true);
			item.SetSelected(this.SelectedTemplateId == this._filteredData[index].TemplateId);
		}

		// Token: 0x0600B711 RID: 46865 RVA: 0x00536FE4 File Offset: 0x005351E4
		private void OnSelect(short templateId, bool call = true, bool isClick = false)
		{
			bool flag = isClick && this.SelectedTemplateId == templateId;
			if (flag)
			{
				this.SelectedTemplateId = -1;
			}
			else
			{
				this.SelectedTemplateId = templateId;
			}
			for (int i = 0; i < this._filteredData.Count; i++)
			{
				GameObject obj = this.skillScroll.GetActiveCell(i);
				bool flag2 = obj != null;
				if (flag2)
				{
					obj.GetComponent<CombatSkillSelectable>().SetSelected(this.SelectedTemplateId == this._filteredData[i].TemplateId);
				}
			}
			if (call)
			{
				Action<short> onSelect = this._onSelect;
				if (onSelect != null)
				{
					onSelect(templateId);
				}
			}
		}

		// Token: 0x04008E22 RID: 36386
		public SortAndFilter skillSortAndFilter;

		// Token: 0x04008E23 RID: 36387
		public InfinityScroll skillScroll;

		// Token: 0x04008E24 RID: 36388
		public CImage emptyIcon;

		// Token: 0x04008E25 RID: 36389
		private SimplifiedCombatSkillSortAndFilterController _sortAndFilterController;

		// Token: 0x04008E26 RID: 36390
		private List<CombatSkillDisplayDataForList> _skillListData = new List<CombatSkillDisplayDataForList>();

		// Token: 0x04008E27 RID: 36391
		private List<CombatSkillDisplayDataForList> _filteredData = new List<CombatSkillDisplayDataForList>();

		// Token: 0x04008E28 RID: 36392
		private Action<short> _onSelect;

		// Token: 0x04008E29 RID: 36393
		private bool _autoSelectFirst = true;

		// Token: 0x04008E2A RID: 36394
		[NonSerialized]
		public short SelectedTemplateId = -1;
	}
}
