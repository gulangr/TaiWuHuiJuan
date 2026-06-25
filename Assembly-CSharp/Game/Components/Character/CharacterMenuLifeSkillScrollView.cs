using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;
using Game.Views.CharacterMenu;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F1B RID: 3867
	public class CharacterMenuLifeSkillScrollView : MonoBehaviour, ILanguage
	{
		// Token: 0x0600B22A RID: 45610 RVA: 0x00511F8E File Offset: 0x0051018E
		private void Awake()
		{
			this.Init();
		}

		// Token: 0x0600B22B RID: 45611 RVA: 0x00511F98 File Offset: 0x00510198
		public void Init()
		{
			bool inited = this._inited;
			if (!inited)
			{
				this._skillScroll.OnItemRender += this.OnRenderItem;
				this._skillScroll.OnItemHide += this.OnHideItem;
				this._skillScroll.OnLanguageChanged += this.OnLanguageChange;
				this._inited = true;
			}
		}

		// Token: 0x0600B22C RID: 45612 RVA: 0x00512004 File Offset: 0x00510204
		public void SetLifeSkillList(List<CharacterMenuLifeSkillItemData> skillList, bool reset = false, bool interactable = true, string listTag = null, Action<CharacterMenuLifeSkillItemData, CharacterMenuLifeSkillItem> onRenderLifeSkill = null, bool addEmptyItem = false, bool isShowNeiLiFinish = false, GameObject customEmptyObject = null, bool scrollToTopWhenListCountChanged = true)
		{
			this._currentSkillList = skillList;
			if (reset)
			{
				this._onRenderSkill = onRenderLifeSkill;
				this._listTag = listTag;
				this._interactable = interactable;
			}
			this._isShowNeiLiFinish = isShowNeiLiFinish;
			this._customEmptyObject = customEmptyObject;
			this._scrollToTopWhenCountChanged = scrollToTopWhenListCountChanged;
			this.OnSkillListChanged();
		}

		// Token: 0x0600B22D RID: 45613 RVA: 0x00512056 File Offset: 0x00510256
		public void ScrollToTop()
		{
			this._skillScroll.ScrollTo(0, 0.3f);
		}

		// Token: 0x0600B22E RID: 45614 RVA: 0x0051206B File Offset: 0x0051026B
		public void ScrollTo(int index)
		{
			this._skillScroll.ScrollTo(index, 0.3f);
		}

		// Token: 0x0600B22F RID: 45615 RVA: 0x00512080 File Offset: 0x00510280
		public void ReRender()
		{
			this._skillScroll.ReRender();
		}

		// Token: 0x0600B230 RID: 45616 RVA: 0x0051208F File Offset: 0x0051028F
		public void SaveSortFilterSetting(bool saveGlobalSettings = true)
		{
		}

		// Token: 0x0600B231 RID: 45617 RVA: 0x00512092 File Offset: 0x00510292
		private void OnSkillListChanged()
		{
			this._skillScroll.UpdateData(this._currentSkillList.Count);
			Action onSkillListChangeFinal = this.OnSkillListChangeFinal;
			if (onSkillListChangeFinal != null)
			{
				onSkillListChangeFinal();
			}
		}

		// Token: 0x0600B232 RID: 45618 RVA: 0x005120C0 File Offset: 0x005102C0
		private void OnRenderItem(int index, GameObject skillItem)
		{
			CharacterMenuLifeSkillItemData skillData = this._currentSkillList[index];
			CharacterMenuLifeSkillItem skillView = skillItem.GetComponent<CharacterMenuLifeSkillItem>();
			RectTransform itemTransform = skillView.GetComponent<RectTransform>();
			skillView.Set(skillData);
			Action<CharacterMenuLifeSkillItemData, CharacterMenuLifeSkillItem> onRenderSkill = this._onRenderSkill;
			if (onRenderSkill != null)
			{
				onRenderSkill(skillData, skillView);
			}
		}

		// Token: 0x0600B233 RID: 45619 RVA: 0x00512105 File Offset: 0x00510305
		private void OnHideItem(GameObject skillItem)
		{
		}

		// Token: 0x0600B234 RID: 45620 RVA: 0x00512108 File Offset: 0x00510308
		private void OnLanguageChange(GameObject item, LocalStringManager.LanguageType languageType)
		{
		}

		// Token: 0x0600B235 RID: 45621 RVA: 0x0051210C File Offset: 0x0051030C
		public void OnLanguageChange(LocalStringManager.LanguageType languageType)
		{
			ILanguage iLanguage = this._skillScroll;
			bool flag = iLanguage != null;
			if (flag)
			{
				iLanguage.OnLanguageChange(languageType);
			}
		}

		// Token: 0x04008A31 RID: 35377
		[SerializeField]
		private InfinityScroll _skillScroll;

		// Token: 0x04008A32 RID: 35378
		private List<CharacterMenuLifeSkillItemData> _currentSkillList;

		// Token: 0x04008A33 RID: 35379
		private string _listTag;

		// Token: 0x04008A34 RID: 35380
		private Action<CharacterMenuLifeSkillItemData, CharacterMenuLifeSkillItem> _onRenderSkill;

		// Token: 0x04008A35 RID: 35381
		private bool _interactable;

		// Token: 0x04008A36 RID: 35382
		private bool _inited = false;

		// Token: 0x04008A37 RID: 35383
		private bool _isShowNeiLiFinish = false;

		// Token: 0x04008A38 RID: 35384
		private bool _scrollToTopWhenCountChanged = true;

		// Token: 0x04008A39 RID: 35385
		private GameObject _customEmptyObject;

		// Token: 0x04008A3A RID: 35386
		public Action OnSkillListChangeFinal;
	}
}
