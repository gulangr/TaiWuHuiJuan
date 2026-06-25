using System;
using System.Collections.Generic;
using System.Text;
using Config;
using Config.ConfigCells.Character;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using GameData.Domains.Character;
using GameData.Domains.CombatSkill;
using GameData.Domains.Story.SectMainStory;
using UnityEngine;

namespace Game.Views.SectInteract.Emei
{
	// Token: 0x020009EB RID: 2539
	public class EmeiAddBonus : MonoBehaviour
	{
		// Token: 0x17000DAC RID: 3500
		// (get) Token: 0x06007CB3 RID: 31923 RVA: 0x0039F69C File Offset: 0x0039D89C
		private EmeiAddBonusItem BonusItem
		{
			get
			{
				return (this._selectedIndex == 0) ? this.leftBonus : this.rightBonus;
			}
		}

		// Token: 0x17000DAD RID: 3501
		// (get) Token: 0x06007CB4 RID: 31924 RVA: 0x0039F6B4 File Offset: 0x0039D8B4
		public short SelectedBonus
		{
			get
			{
				return this.BonusItem.Selected.TemplateId;
			}
		}

		// Token: 0x06007CB5 RID: 31925 RVA: 0x0039F6C8 File Offset: 0x0039D8C8
		public bool CanInteractBonus(short bonusTemplateId, int count)
		{
			bool flag = count <= 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this._skillTemplateId < 0;
				if (flag2)
				{
					result = false;
				}
				else
				{
					short oldBonusId = (this._selectedIndex == 0) ? this._data[this._skillTemplateId].EmeiBonus1 : this._data[this._skillTemplateId].EmeiBonus2;
					short selectedId = (this._selectedIndex == 0) ? this.rightBonus.Selected.TemplateId : this.leftBonus.Selected.TemplateId;
					bool flag3 = selectedId == bonusTemplateId && count <= 1;
					result = (!flag3 && (oldBonusId != bonusTemplateId && BonusAppearFilter.FilterByAppearType(SkillBreakPlateGridBonusType.Instance[bonusTemplateId], this._data[this._skillTemplateId])) && this.CanReduceBonus(bonusTemplateId));
				}
			}
			return result;
		}

		// Token: 0x06007CB6 RID: 31926 RVA: 0x0039F7AD File Offset: 0x0039D9AD
		public bool CanInteractBonusPure(short bonusTemplateId)
		{
			return this._skillTemplateId >= 0 && BonusAppearFilter.FilterByAppearType(SkillBreakPlateGridBonusType.Instance[bonusTemplateId], this._data[this._skillTemplateId]) && this.CanReduceBonus(bonusTemplateId);
		}

		// Token: 0x06007CB7 RID: 31927 RVA: 0x0039F7E8 File Offset: 0x0039D9E8
		private bool CanReduceBonus(short bonusTemplateId)
		{
			CombatSkillDisplayDataForList skillData = this._data[this._skillTemplateId];
			SkillBreakPlateGridBonusTypeItem bonusCfg = SkillBreakPlateGridBonusType.Instance[bonusTemplateId];
			HitOrAvoidInts hitDist = skillData.HitDistribution;
			short oldBonusId = (this._selectedIndex == 0) ? skillData.EmeiBonus1 : skillData.EmeiBonus2;
			foreach (PropertyAndValue propBonus in bonusCfg.CombatSkillPropertyBonusList)
			{
				bool flag = propBonus.Value >= 0;
				if (!flag)
				{
					bool flag2 = propBonus.PropertyId < 31 || propBonus.PropertyId > 33;
					if (!flag2)
					{
						int hitType = (int)(propBonus.PropertyId - 31);
						int currentValue = hitDist[hitType];
						bool flag3 = oldBonusId >= 0;
						if (flag3)
						{
							SkillBreakPlateGridBonusTypeItem oldCfg = SkillBreakPlateGridBonusType.Instance[oldBonusId];
							foreach (PropertyAndValue oldProp in oldCfg.CombatSkillPropertyBonusList)
							{
								bool flag4 = oldProp.PropertyId == propBonus.PropertyId;
								if (flag4)
								{
									currentValue -= (int)oldProp.Value;
									break;
								}
							}
						}
						bool flag5 = currentValue <= (int)(-(int)propBonus.Value);
						if (flag5)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		// Token: 0x06007CB8 RID: 31928 RVA: 0x0039F940 File Offset: 0x0039DB40
		private void OnDisable()
		{
			this.SetSkill(-1);
		}

		// Token: 0x06007CB9 RID: 31929 RVA: 0x0039F94C File Offset: 0x0039DB4C
		public void Init(Action<short, short> onRemoveBonus, Action<short, short, short> onAddBonus, Action<short, short, short, short, short> onAddBonuses, Action updateBonusList)
		{
			this._onRemoveBonus = onRemoveBonus;
			this._onAddBonus = onAddBonus;
			this._onAddBonuses = onAddBonuses;
			this._updateBonusList = updateBonusList;
			this.scroll.Init("EmeiSelectCombatSkill", false);
			this.leftBonus.Init(0, new Action<int>(this.OnSelectBonusItemIndex), new Action<int>(this.OnCancelBonus), new Action<int>(this.OnRemoveBonus));
			this.rightBonus.Init(1, new Action<int>(this.OnSelectBonusItemIndex), new Action<int>(this.OnCancelBonus), new Action<int>(this.OnRemoveBonus));
			this.btnConfirm.ClearAndAddListener(new Action(this.Confirm));
			this.RefreshButtonConfirm();
		}

		// Token: 0x06007CBA RID: 31930 RVA: 0x0039FA0A File Offset: 0x0039DC0A
		public void SetSkill(short templateId)
		{
			this.OnSelectCombatSkill(templateId);
		}

		// Token: 0x06007CBB RID: 31931 RVA: 0x0039FA18 File Offset: 0x0039DC18
		public void Set(List<CombatSkillDisplayDataForList> skillListData, Dictionary<short, SectEmeiBreakBonusData> bonuses, bool isAdvanceUnlocked)
		{
			this._data.Clear();
			foreach (CombatSkillDisplayDataForList data in skillListData)
			{
				this._data[data.TemplateId] = data;
			}
			this._bonuses = bonuses;
			this._skillListData = skillListData;
			this.leftBonus.Locked = false;
			this.rightBonus.Locked = !isAdvanceUnlocked;
			this.arrows.SetActive(!isAdvanceUnlocked);
			this.scroll.Set(this._skillListData, new Action<short>(this.OnSelectCombatSkill));
			this.OnSelectBonusItemIndex(0);
			this.SetSkill(-1);
		}

		// Token: 0x06007CBC RID: 31932 RVA: 0x0039FAEC File Offset: 0x0039DCEC
		public void Set(SectEmeiBreakBonusData bonus)
		{
			this.BonusItem.Selected = ((this.BonusItem.Selected.TemplateId == bonus.TemplateId) ? ViewEmeiCombatSkillSpecialBreak.InvalidBonus : bonus);
			this.RefreshButtonConfirm();
		}

		// Token: 0x06007CBD RID: 31933 RVA: 0x0039FB22 File Offset: 0x0039DD22
		private void OnSelectBonusItemIndex(int index)
		{
			this._selectedIndex = index;
			this.leftBonus.SetSelectedStatus(index == 0);
			this.rightBonus.SetSelectedStatus(index != 0);
			this.RefreshButtonConfirm();
			this._updateBonusList();
		}

		// Token: 0x06007CBE RID: 31934 RVA: 0x0039FB60 File Offset: 0x0039DD60
		private void OnSelectCombatSkill(short templateId)
		{
			bool flag = this._skillTemplateId == templateId || templateId < 0;
			if (flag)
			{
				this.noContent.SetActive(true);
				this.content.SetActive(false);
				this._skillTemplateId = -1;
				this.leftBonus.Origin = ViewEmeiCombatSkillSpecialBreak.InvalidBonus;
				this.rightBonus.Origin = ViewEmeiCombatSkillSpecialBreak.InvalidBonus;
			}
			else
			{
				this._skillTemplateId = templateId;
				CombatSkillDisplayDataForList data = this._data[templateId];
				this.skill.Set(data, null, false);
				this.leftBonus.Origin = ((data.EmeiBonus1 >= 0) ? this._bonuses[data.EmeiBonus1] : ViewEmeiCombatSkillSpecialBreak.InvalidBonus);
				this.rightBonus.Origin = ((data.EmeiBonus2 >= 0) ? this._bonuses[data.EmeiBonus2] : ViewEmeiCombatSkillSpecialBreak.InvalidBonus);
				this.noContent.SetActive(false);
				this.content.SetActive(true);
			}
			this.leftBonus.Selected = ViewEmeiCombatSkillSpecialBreak.InvalidBonus;
			this.rightBonus.Selected = ViewEmeiCombatSkillSpecialBreak.InvalidBonus;
			this._updateBonusList();
			this.RefreshButtonConfirm();
			this.scroll.SelectWithoutNotify(this._skillTemplateId);
		}

		// Token: 0x06007CBF RID: 31935 RVA: 0x0039FCAC File Offset: 0x0039DEAC
		private void OnCancelBonus(int index)
		{
			bool flag = index != this._selectedIndex;
			if (!flag)
			{
				this.BonusItem.Selected = ViewEmeiCombatSkillSpecialBreak.InvalidBonus;
				this.RefreshButtonConfirm();
			}
		}

		// Token: 0x06007CC0 RID: 31936 RVA: 0x0039FCE4 File Offset: 0x0039DEE4
		private void OnRemoveBonus(int index)
		{
			EmeiAddBonusItem bonus = (index == 0) ? this.leftBonus : this.rightBonus;
			this._onRemoveBonus(this._skillTemplateId, bonus.Origin.TemplateId);
			bonus.Origin = ViewEmeiCombatSkillSpecialBreak.InvalidBonus;
		}

		// Token: 0x06007CC1 RID: 31937 RVA: 0x0039FD30 File Offset: 0x0039DF30
		private void Confirm()
		{
			bool flag = this.leftBonus.Selected.TemplateId >= 0;
			if (flag)
			{
				bool flag2 = this.rightBonus.Selected.TemplateId >= 0;
				if (flag2)
				{
					this._onAddBonuses(this._skillTemplateId, this.leftBonus.Selected.TemplateId, this.leftBonus.Origin.TemplateId, this.rightBonus.Selected.TemplateId, this.rightBonus.Origin.TemplateId);
				}
				else
				{
					this._onAddBonus(this._skillTemplateId, this.leftBonus.Selected.TemplateId, this.leftBonus.Origin.TemplateId);
				}
			}
			else
			{
				bool flag3 = this.rightBonus.Selected.TemplateId >= 0;
				if (flag3)
				{
					this._onAddBonus(this._skillTemplateId, this.rightBonus.Selected.TemplateId, this.rightBonus.Origin.TemplateId);
				}
			}
		}

		// Token: 0x06007CC2 RID: 31938 RVA: 0x0039FE54 File Offset: 0x0039E054
		private void RefreshButtonConfirm()
		{
			this.btnConfirm.interactable = (this._skillTemplateId >= 0 && (this.leftBonus.Selected.TemplateId >= 0 || this.rightBonus.Selected.TemplateId >= 0));
			TooltipInvoker tip = this.btnConfirm.GetComponent<TooltipInvoker>();
			tip.RuntimeParam = null;
			bool flag = !this.btnConfirm.interactable;
			if (flag)
			{
				tip.enabled = true;
				ArgumentBox argumentBox = new ArgumentBox();
				argumentBox.Set("arg0", LocalStringManager.Get(LanguageKey.LK_GetItem_CombatSkillComprehend));
				StringBuilder sb = EasyPool.Get<StringBuilder>();
				sb.Clear();
				bool flag2 = this._skillTemplateId < 0;
				if (flag2)
				{
					sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_CombatSkill_SpecialBreak_CantNotAddReason_CombatSkill).SetColor("brightred"));
				}
				else
				{
					bool flag3 = this.BonusItem.Selected.TemplateId < 0;
					if (flag3)
					{
						sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_CombatSkill_SpecialBreak_CantNotAddReason_Entry).SetColor("brightred"));
					}
				}
				argumentBox.Set("arg1", sb.ToString().TrimEnd());
				tip.RuntimeParam = argumentBox;
				EasyPool.Free<StringBuilder>(sb);
			}
			else
			{
				tip.enabled = false;
			}
		}

		// Token: 0x04005EE9 RID: 24297
		public GameObject content;

		// Token: 0x04005EEA RID: 24298
		public GameObject noContent;

		// Token: 0x04005EEB RID: 24299
		public EmeiAddBonusItem leftBonus;

		// Token: 0x04005EEC RID: 24300
		public EmeiAddBonusItem rightBonus;

		// Token: 0x04005EED RID: 24301
		public GameObject arrows;

		// Token: 0x04005EEE RID: 24302
		public EmeiCombatSkillItem skill;

		// Token: 0x04005EEF RID: 24303
		public CButton btnConfirm;

		// Token: 0x04005EF0 RID: 24304
		public CombatSkillSelect scroll;

		// Token: 0x04005EF1 RID: 24305
		private Dictionary<short, SectEmeiBreakBonusData> _bonuses;

		// Token: 0x04005EF2 RID: 24306
		private Dictionary<short, CombatSkillDisplayDataForList> _data = new Dictionary<short, CombatSkillDisplayDataForList>();

		// Token: 0x04005EF3 RID: 24307
		private int _selectedIndex = 0;

		// Token: 0x04005EF4 RID: 24308
		private short _skillTemplateId = -1;

		// Token: 0x04005EF5 RID: 24309
		private Action<short, short> _onRemoveBonus;

		// Token: 0x04005EF6 RID: 24310
		private Action<short, short, short> _onAddBonus;

		// Token: 0x04005EF7 RID: 24311
		private Action<short, short, short, short, short> _onAddBonuses;

		// Token: 0x04005EF8 RID: 24312
		private Action _updateBonusList;

		// Token: 0x04005EF9 RID: 24313
		private List<CombatSkillDisplayDataForList> _skillListData;
	}
}
