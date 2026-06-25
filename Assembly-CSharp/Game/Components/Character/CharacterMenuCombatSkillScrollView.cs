using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.CombatSkill;
using Game.Views.CharacterMenu;
using GameData.Domains.CombatSkill;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F1A RID: 3866
	public class CharacterMenuCombatSkillScrollView : MonoBehaviour, ILanguage
	{
		// Token: 0x1700142E RID: 5166
		// (get) Token: 0x0600B21B RID: 45595 RVA: 0x00511C8E File Offset: 0x0050FE8E
		public List<CombatSkillDisplayDataCharacterMenuListItem> filteredData
		{
			get
			{
				return this._filteredData;
			}
		}

		// Token: 0x1700142F RID: 5167
		// (get) Token: 0x0600B21C RID: 45596 RVA: 0x00511C96 File Offset: 0x0050FE96
		// (set) Token: 0x0600B21D RID: 45597 RVA: 0x00511C9E File Offset: 0x0050FE9E
		public CharacterMenuCombatSkillListItemSortAndFilterController SortAndFilter { get; private set; }

		// Token: 0x0600B21E RID: 45598 RVA: 0x00511CA7 File Offset: 0x0050FEA7
		private void Awake()
		{
			this.Init();
		}

		// Token: 0x0600B21F RID: 45599 RVA: 0x00511CB4 File Offset: 0x0050FEB4
		public void Init()
		{
			bool inited = this._inited;
			if (!inited)
			{
				this.SortAndFilter = new CharacterMenuCombatSkillListItemSortAndFilterController(this.commonSortAndFilter, this.singleLineFilter, this.characterMenuCombatSkillFilter);
				this.SortAndFilter.Init(new Action(this.OnSkillListChanged), "CharacterMenuCombatSkillSortAndFilter");
				this._skillScroll.OnItemRender += this.OnRenderItem;
				this._skillScroll.OnItemHide += this.OnHideItem;
				this._skillScroll.OnLanguageChanged += this.OnLanguageChange;
				this._inited = true;
			}
		}

		// Token: 0x0600B220 RID: 45600 RVA: 0x00511D5C File Offset: 0x0050FF5C
		public void SetCombatSkillList(List<CombatSkillDisplayDataCharacterMenuListItem> skillList, bool reset = false, bool interactable = true, string listTag = null, Action<CombatSkillDisplayDataCharacterMenuListItem, CharacterMenuCombatSkillItem> onRenderSkill = null, bool addEmptyItem = false, bool isShowNeiLiFinish = false, GameObject customEmptyObject = null, bool scrollToTopWhenListCountChanged = true)
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
			this._skillList = skillList;
			this.OnSkillListChanged();
		}

		// Token: 0x0600B221 RID: 45601 RVA: 0x00511DAE File Offset: 0x0050FFAE
		public void ScrollToTop()
		{
			this._skillScroll.ScrollTo(0, 0.3f);
		}

		// Token: 0x0600B222 RID: 45602 RVA: 0x00511DC3 File Offset: 0x0050FFC3
		public void ScrollTo(int index)
		{
			this._skillScroll.ScrollTo(index, 0.3f);
		}

		// Token: 0x0600B223 RID: 45603 RVA: 0x00511DD8 File Offset: 0x0050FFD8
		public void ReRender()
		{
			this._skillScroll.ReRender();
		}

		// Token: 0x0600B224 RID: 45604 RVA: 0x00511DE8 File Offset: 0x0050FFE8
		private void OnSkillListChanged()
		{
			this._filteredData.Clear();
			Func<CombatSkillDisplayDataCharacterMenuListItem, bool> filter = this.SortAndFilter.GenerateFilter();
			bool flag = this._skillList != null;
			if (flag)
			{
				foreach (CombatSkillDisplayDataCharacterMenuListItem item in this._skillList)
				{
					bool flag2 = filter(item);
					if (flag2)
					{
						this._filteredData.Add(item);
					}
				}
			}
			bool flag3 = this._skillList != null;
			if (flag3)
			{
				this.SortAndFilter.AfterFilter(this._skillList);
			}
			Comparison<CombatSkillDisplayDataCharacterMenuListItem> comparer = this.SortAndFilter.GenerateComparer(this._filteredData);
			this._filteredData.Sort(comparer);
			this._skillScroll.UpdateData(this._filteredData.Count);
			Action onSkillListChangeFinal = this.OnSkillListChangeFinal;
			if (onSkillListChangeFinal != null)
			{
				onSkillListChangeFinal();
			}
		}

		// Token: 0x0600B225 RID: 45605 RVA: 0x00511EE4 File Offset: 0x005100E4
		private void OnRenderItem(int index, GameObject skillItem)
		{
			CombatSkillDisplayDataCharacterMenuListItem skillData = this._filteredData[index];
			CharacterMenuCombatSkillItem skillView = skillItem.GetComponent<CharacterMenuCombatSkillItem>();
			RectTransform itemTransform = skillView.GetComponent<RectTransform>();
			skillView.SetByCharacterMenuList(skillData);
			Action<CombatSkillDisplayDataCharacterMenuListItem, CharacterMenuCombatSkillItem> onRenderSkill = this._onRenderSkill;
			if (onRenderSkill != null)
			{
				onRenderSkill(skillData, skillView);
			}
		}

		// Token: 0x0600B226 RID: 45606 RVA: 0x00511F29 File Offset: 0x00510129
		private void OnHideItem(GameObject skillItem)
		{
		}

		// Token: 0x0600B227 RID: 45607 RVA: 0x00511F2C File Offset: 0x0051012C
		private void OnLanguageChange(GameObject item, LocalStringManager.LanguageType languageType)
		{
		}

		// Token: 0x0600B228 RID: 45608 RVA: 0x00511F30 File Offset: 0x00510130
		public void OnLanguageChange(LocalStringManager.LanguageType languageType)
		{
			ILanguage iLanguage = this._skillScroll;
			bool flag = iLanguage != null;
			if (flag)
			{
				iLanguage.OnLanguageChange(languageType);
			}
		}

		// Token: 0x04008A22 RID: 35362
		[SerializeField]
		private SortAndFilter commonSortAndFilter;

		// Token: 0x04008A23 RID: 35363
		[SerializeField]
		private InfinityScroll _skillScroll;

		// Token: 0x04008A24 RID: 35364
		private List<CombatSkillDisplayDataCharacterMenuListItem> _skillList;

		// Token: 0x04008A25 RID: 35365
		private List<CombatSkillDisplayDataCharacterMenuListItem> _filteredData = new List<CombatSkillDisplayDataCharacterMenuListItem>();

		// Token: 0x04008A27 RID: 35367
		private string _listTag;

		// Token: 0x04008A28 RID: 35368
		private Action<CombatSkillDisplayDataCharacterMenuListItem, CharacterMenuCombatSkillItem> _onRenderSkill;

		// Token: 0x04008A29 RID: 35369
		private bool _interactable;

		// Token: 0x04008A2A RID: 35370
		private bool _inited = false;

		// Token: 0x04008A2B RID: 35371
		private bool _isShowNeiLiFinish = false;

		// Token: 0x04008A2C RID: 35372
		private bool _scrollToTopWhenCountChanged = true;

		// Token: 0x04008A2D RID: 35373
		private GameObject _customEmptyObject;

		// Token: 0x04008A2E RID: 35374
		public Action OnSkillListChangeFinal;

		// Token: 0x04008A2F RID: 35375
		public bool singleLineFilter = false;

		// Token: 0x04008A30 RID: 35376
		public bool characterMenuCombatSkillFilter = false;
	}
}
