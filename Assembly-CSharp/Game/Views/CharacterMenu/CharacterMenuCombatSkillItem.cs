using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UI.LanguageRule;
using GameData.Domains.CombatSkill;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B62 RID: 2914
	public class CharacterMenuCombatSkillItem : MonoBehaviour
	{
		// Token: 0x06009050 RID: 36944 RVA: 0x00434070 File Offset: 0x00432270
		public void Set(CombatSkillDisplayData skillData)
		{
			bool flag = skillData == null;
			if (!flag)
			{
				CombatSkillItem config = CombatSkill.Instance[skillData.TemplateId];
				Color gradeColor = Colors.Instance.GradeColors[(int)config.Grade];
				this.SetSkillName(config.Name.SetColor(gradeColor));
				this.power.text = skillData.Power.ToString() + "%";
				this.gradeLine.color = gradeColor;
				this.icon.SetSprite(config.Icon, false, null);
				this.icon.SetColor(CommonUtils.GetFiveElementColor((int)config.FiveElements));
				this.back.SetSprite("ui9_icon_combat_skill_frame_" + config.Grade.ToString(), false, null);
				this.frame.SetSprite(CharacterMenuCombatSkillItem.FramePaths[(int)config.EquipType] + config.Grade.ToString(), false, null);
				bool isBrokenOut = CombatSkillStateHelper.IsBrokenOut(skillData.ActivationState);
				this.effectDirection.gameObject.SetActive(isBrokenOut);
				bool flag2 = isBrokenOut;
				if (flag2)
				{
					bool direction = CombatSkillStateHelper.GetCombatSkillDirection(skillData.ActivationState) == 0;
					this.effectDirection.SetSpriteOnly("ui9_icon_combat_skill_effect_direction_" + (direction ? "0" : "1"), false, null);
				}
				for (byte pageId = 1; pageId < 6; pageId += 1)
				{
					sbyte direction2 = CombatSkillStateHelper.GetPageActiveDirection(skillData.ActivationState, pageId);
					bool flag3 = direction2 == -1;
					if (flag3)
					{
						this.activationStateList[(int)(pageId - 1)].gameObject.SetActive(false);
					}
					else
					{
						this.activationStateList[(int)(pageId - 1)].color = CharacterMenuCombatSkillItem.ActivationStateColors[(int)(direction2 + 1)].HexStringToColor();
						this.activationStateList[(int)(pageId - 1)].gameObject.SetActive(true);
					}
				}
				List<CImage> list = this.activationStateList;
				list[list.Count - 1].gameObject.SetActive(CombatSkillStateHelper.GetActiveOutlinePageType(skillData.ActivationState) >= 0);
				this.UpdateBonusDisplay(skillData.LuohanId >= 0, skillData.BreakBonusGrades, (int)skillData.TemplateId);
				this.RefreshBonusTip(skillData.LuohanId, skillData.BreakBonusGrades, skillData.CharId, (int)skillData.TemplateId);
				this.UpdateMouseTip(skillData.TemplateId, skillData.CharId);
				this.fiveElementsFrame.gameObject.SetActive(false);
				this.revokedMark.SetActive(skillData.Revoked);
			}
		}

		// Token: 0x06009051 RID: 36945 RVA: 0x00434305 File Offset: 0x00432505
		public void SetFavorIcon(bool showIcon)
		{
			GameObject gameObject = this.favorIcon;
			if (gameObject != null)
			{
				gameObject.SetActive(showIcon);
			}
		}

		// Token: 0x06009052 RID: 36946 RVA: 0x0043431C File Offset: 0x0043251C
		public void SetEquipCombatSkillRunningMarkVisible(bool visible)
		{
			bool flag = this.equipCombatSkillRunningMark != null;
			if (flag)
			{
				this.equipCombatSkillRunningMark.SetActive(visible);
			}
		}

		// Token: 0x06009053 RID: 36947 RVA: 0x0043434C File Offset: 0x0043254C
		public void SetHoverIcon(bool showIcon)
		{
			bool flag = this.hoverIcon == null;
			if (!flag)
			{
				this.hoverIcon.SetActive(showIcon);
			}
		}

		// Token: 0x06009054 RID: 36948 RVA: 0x0043437C File Offset: 0x0043257C
		public void SetSelectedIcon(bool showIcon)
		{
			bool flag = this.selectedIcon == null;
			if (!flag)
			{
				this.selectedIcon.SetActive(showIcon);
			}
		}

		// Token: 0x06009055 RID: 36949 RVA: 0x004343AC File Offset: 0x004325AC
		public void SetByCharacterMenuList(CombatSkillDisplayDataCharacterMenuListItem skillData)
		{
			bool flag = skillData == null;
			if (!flag)
			{
				this.Set(skillData.TemplateId, skillData.Power, skillData.BreakSuccess, skillData.ActivationState, skillData.BreakBonusGrades, skillData.CharId, skillData.LuohanId, skillData.Revoked);
			}
		}

		// Token: 0x06009056 RID: 36950 RVA: 0x004343FC File Offset: 0x004325FC
		public void Set(short templateId, short skillPower, bool breakSuccess, ushort activationState, List<sbyte> breakBonusGrades, int charId, sbyte luohanId, bool revoked)
		{
			bool isLuohanBonus = luohanId >= 0;
			CombatSkillItem config = CombatSkill.Instance[templateId];
			Color gradeColor = Colors.Instance.GradeColors[(int)config.Grade];
			this.SetSkillName(config.Name.SetColor(gradeColor));
			this.power.text = skillPower.ToString() + "%";
			this.gradeLine.color = gradeColor;
			this.icon.SetSprite(config.Icon, false, null);
			this.icon.SetColor(CommonUtils.GetFiveElementColor((int)config.FiveElements));
			this.back.SetSprite("ui9_icon_combat_skill_frame_" + config.Grade.ToString(), false, null);
			this.frame.SetSprite(CharacterMenuCombatSkillItem.FramePaths[(int)config.EquipType] + config.Grade.ToString(), false, null);
			bool isBrokenOut = CombatSkillStateHelper.IsBrokenOut(activationState);
			this.effectDirection.gameObject.SetActive(isBrokenOut);
			bool flag = isBrokenOut;
			if (flag)
			{
				bool direction = CombatSkillStateHelper.GetCombatSkillDirection(activationState) == 0;
				this.effectDirection.SetSpriteOnly("ui9_icon_combat_skill_effect_direction_" + (direction ? "0" : "1"), false, null);
			}
			for (byte pageId = 1; pageId < 6; pageId += 1)
			{
				sbyte direction2 = CombatSkillStateHelper.GetPageActiveDirection(activationState, pageId);
				bool flag2 = direction2 == -1;
				if (flag2)
				{
					this.activationStateList[(int)(pageId - 1)].gameObject.SetActive(false);
				}
				else
				{
					this.activationStateList[(int)(pageId - 1)].color = CharacterMenuCombatSkillItem.ActivationStateColors[(int)(direction2 + 1)].HexStringToColor();
					this.activationStateList[(int)(pageId - 1)].gameObject.SetActive(true);
				}
			}
			List<CImage> list = this.activationStateList;
			list[list.Count - 1].gameObject.SetActive(CombatSkillStateHelper.GetActiveOutlinePageType(activationState) >= 0);
			this.UpdateBonusDisplay(isLuohanBonus, breakBonusGrades, (int)templateId);
			this.RefreshBonusTip(luohanId, breakBonusGrades, charId, (int)templateId);
			this.UpdateMouseTip(templateId, charId);
			this.fiveElementsFrame.gameObject.SetActive(false);
			this.revokedMark.SetActive(revoked);
		}

		// Token: 0x06009057 RID: 36951 RVA: 0x00434640 File Offset: 0x00432840
		public void UpdateMouseTip(short templateId, int charId)
		{
			this.mouseTip.enabled = (templateId >= 0 && charId >= 0);
			bool flag = templateId < 0 || charId < 0;
			if (!flag)
			{
				this.mouseTip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("CombatSkillId", templateId).Set("CharId", charId);
				bool showing = this.mouseTip.Showing;
				if (showing)
				{
					this.mouseTip.Refresh(false, -1);
				}
			}
		}

		// Token: 0x06009058 RID: 36952 RVA: 0x004346C0 File Offset: 0x004328C0
		private void RefreshBonusTip(sbyte luohanId, List<sbyte> breakBonusGrades, int charId, int templateId)
		{
			TooltipInvoker tip = (luohanId >= 0) ? this.luohanBonusTip : this.bonusTip;
			Behaviour behaviour = tip;
			bool enabled;
			if (breakBonusGrades == null)
			{
				enabled = false;
			}
			else
			{
				enabled = breakBonusGrades.Any((sbyte grade) => grade >= 0);
			}
			behaviour.enabled = enabled;
			tip.Type = TipType.CombatSkillBonus;
			TooltipInvoker tooltipInvoker = tip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			tip.RuntimeParam.Set("CharId", charId).Set("SkillId", (short)templateId).Set("luohanId", luohanId);
		}

		// Token: 0x06009059 RID: 36953 RVA: 0x00434768 File Offset: 0x00432968
		private void UpdateBonusDisplay(bool isLuohanBonus, List<sbyte> breakBonusGrades, int templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			int bonusCount = config.SkillBreakPlate.BonusCount;
			if (isLuohanBonus)
			{
				this.normalBonusHolder.SetActive(false);
				this.luohanBonusHolder.SetActive(true);
				for (int i = 0; i < bonusCount; i++)
				{
					this.luohanBonusHolder.transform.GetChild(i).gameObject.SetActive(true);
				}
				for (int j = bonusCount; j < this.luohanBonusHolder.transform.childCount; j++)
				{
					this.luohanBonusHolder.transform.GetChild(j).gameObject.SetActive(false);
				}
			}
			else
			{
				if (breakBonusGrades != null)
				{
					breakBonusGrades.Sort((sbyte x, sbyte y) => y.CompareTo(x));
				}
				for (int k = 0; k < this.bonusList.Count; k++)
				{
					this.bonusList[k].gameObject.SetActive(k < bonusCount);
					bool flag = this.bonusList[k].gameObject;
					if (flag)
					{
						this.bonusList[k].SetSprite(breakBonusGrades.CheckIndex(k) ? ("ui9_icon_grade_small_" + breakBonusGrades[k].ToString()) : "ui9_icon_grade_small_disable", false, null);
					}
				}
				this.normalBonusHolder.SetActive(true);
				this.luohanBonusHolder.SetActive(false);
			}
		}

		// Token: 0x0600905A RID: 36954 RVA: 0x00434914 File Offset: 0x00432B14
		private void SetSkillName(string text)
		{
			this.skillName.text = text;
			this.skillName.text.ColorReplace();
			this.skillName.fontSize = (float)((LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN) ? 24 : 20);
			this.skillName.ForceMeshUpdate(false, true);
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
			{
				TMP_Text tmp_Text = this.skillName;
				TMP_TextInfo textInfo = this.skillName.textInfo;
				tmp_Text.overflowMode = ((((textInfo != null) ? textInfo.lineCount : 0) <= 2) ? TextOverflowModes.Overflow : TextOverflowModes.Ellipsis);
			});
			LanguageRuleTips tips = this.skillName.GetComponent<LanguageRuleTips>();
			tips.OnLanguageChange(LocalStringManager.CurLanguageType);
			tips.SetTipActive(LocalStringManager.CurLanguageType > LocalStringManager.LanguageType.CN);
		}

		// Token: 0x04006EF6 RID: 28406
		[SerializeField]
		private CImage back;

		// Token: 0x04006EF7 RID: 28407
		[SerializeField]
		private CImage frame;

		// Token: 0x04006EF8 RID: 28408
		[SerializeField]
		private CImage icon;

		// Token: 0x04006EF9 RID: 28409
		[SerializeField]
		private CImage effectDirection;

		// Token: 0x04006EFA RID: 28410
		[SerializeField]
		private List<CImage> bonusList;

		// Token: 0x04006EFB RID: 28411
		[SerializeField]
		private TextMeshProUGUI power;

		// Token: 0x04006EFC RID: 28412
		[SerializeField]
		private TextMeshProUGUI skillName;

		// Token: 0x04006EFD RID: 28413
		[SerializeField]
		private List<CImage> activationStateList;

		// Token: 0x04006EFE RID: 28414
		[SerializeField]
		private CImage gradeLine;

		// Token: 0x04006EFF RID: 28415
		[SerializeField]
		private TooltipInvoker mouseTip;

		// Token: 0x04006F00 RID: 28416
		[SerializeField]
		private TooltipInvoker bonusTip;

		// Token: 0x04006F01 RID: 28417
		[SerializeField]
		private TooltipInvoker luohanBonusTip;

		// Token: 0x04006F02 RID: 28418
		[SerializeField]
		private GameObject normalBonusHolder;

		// Token: 0x04006F03 RID: 28419
		[SerializeField]
		private GameObject luohanBonusHolder;

		// Token: 0x04006F04 RID: 28420
		[SerializeField]
		private CImage fiveElementsFrame;

		// Token: 0x04006F05 RID: 28421
		[SerializeField]
		private GameObject favorIcon;

		// Token: 0x04006F06 RID: 28422
		[SerializeField]
		private GameObject equipCombatSkillRunningMark;

		// Token: 0x04006F07 RID: 28423
		[SerializeField]
		private GameObject hoverIcon;

		// Token: 0x04006F08 RID: 28424
		[SerializeField]
		private GameObject selectedIcon;

		// Token: 0x04006F09 RID: 28425
		[SerializeField]
		private GameObject revokedMark;

		// Token: 0x04006F0A RID: 28426
		public const int StandardWidth = 184;

		// Token: 0x04006F0B RID: 28427
		public const int StandardHeight = 196;

		// Token: 0x04006F0C RID: 28428
		private static readonly string[] ActivationStateColors = new string[]
		{
			"#3f3f3f",
			"#4f769e",
			"#bd484d"
		};

		// Token: 0x04006F0D RID: 28429
		private const string NoBonusColor = "#3a3a3a";

		// Token: 0x04006F0E RID: 28430
		private static readonly string[] FramePaths = new string[]
		{
			"ui9_icon_combat_skill_type_neigong_",
			"ui9_icon_combat_skill_type_attack_",
			"ui9_icon_combat_skill_type_agile_",
			"ui9_icon_combat_skill_type_defense_",
			"ui9_icon_combat_skill_type_assist_"
		};
	}
}
