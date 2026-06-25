using System;
using System.Collections.Generic;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.CombatSkill;
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UISkillBreakPlate;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000BAB RID: 2987
	public class ViewCharacterMenuPractice : UI_CharacterMenuSubPageBase
	{
		// Token: 0x17001022 RID: 4130
		// (get) Token: 0x060095E8 RID: 38376 RVA: 0x0045EA3F File Offset: 0x0045CC3F
		public override LanguageKey TitleKey
		{
			get
			{
				return LanguageKey.LK_CharacterMenu_Title_Practice;
			}
		}

		// Token: 0x17001023 RID: 4131
		// (get) Token: 0x060095E9 RID: 38377 RVA: 0x0045EA46 File Offset: 0x0045CC46
		public override bool ShowCharacterList
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060095EA RID: 38378 RVA: 0x0045EA49 File Offset: 0x0045CC49
		public override bool CheckState(ECharacterSubToggleBase curSubTogglePage, ECharacterSubPage curSubPage)
		{
			return curSubTogglePage == ECharacterSubToggleBase.PracticeBase;
		}

		// Token: 0x17001024 RID: 4132
		// (get) Token: 0x060095EB RID: 38379 RVA: 0x0045EA4F File Offset: 0x0045CC4F
		private bool CurrCharIsTaiwu
		{
			get
			{
				return base.CharacterMenu.CurrentCharacterIsTaiwu;
			}
		}

		// Token: 0x17001025 RID: 4133
		// (get) Token: 0x060095EC RID: 38380 RVA: 0x0045EA5C File Offset: 0x0045CC5C
		private bool IsTaiwuTeamButNotBeast
		{
			get
			{
				return base.CharacterMenu.IsTaiwuTeam && !base.CharacterMenu.IsTaiwuBeastTeammate(base.CharacterMenu.CurCharacterId);
			}
		}

		// Token: 0x17001026 RID: 4134
		// (get) Token: 0x060095ED RID: 38381 RVA: 0x0045EA88 File Offset: 0x0045CC88
		private bool CanInteract
		{
			get
			{
				CharacterMenuFunctionControlItem config;
				return !base.CharacterMenu.StackView.gameObject.activeSelf && (!base.CharacterMenu.TryGetFunctionControlConfig(out config) || !base.CharacterMenu.IsFunctionBanned(config.SkillBreak));
			}
		}

		// Token: 0x17001027 RID: 4135
		// (get) Token: 0x060095EE RID: 38382 RVA: 0x0045EAD5 File Offset: 0x0045CCD5
		private GameData.Domains.Taiwu.SkillBreakPlate Plate
		{
			get
			{
				return this._currentSkillData.SkillBreakPlate;
			}
		}

		// Token: 0x17001028 RID: 4136
		// (get) Token: 0x060095EF RID: 38383 RVA: 0x0045EAE2 File Offset: 0x0045CCE2
		private bool IsBroken
		{
			get
			{
				return CombatSkillStateHelper.IsBrokenOut(this._skillData[this._skillTemplateId].ActivationState);
			}
		}

		// Token: 0x17001029 RID: 4137
		// (get) Token: 0x060095F0 RID: 38384 RVA: 0x0045EAFF File Offset: 0x0045CCFF
		private bool IsLuohanBreak
		{
			get
			{
				return this._currentSkillData.CombatSkillDisplayData.LuohanId >= 0;
			}
		}

		// Token: 0x1700102A RID: 4138
		// (get) Token: 0x060095F1 RID: 38385 RVA: 0x0045EB17 File Offset: 0x0045CD17
		private bool IsRevoked
		{
			get
			{
				return this._skillData[this._skillTemplateId].Revoked;
			}
		}

		// Token: 0x1700102B RID: 4139
		// (get) Token: 0x060095F2 RID: 38386 RVA: 0x0045EB2F File Offset: 0x0045CD2F
		private bool CanBreak
		{
			get
			{
				return !this.IsRevoked && !base.CharacterMenu.OpenFromCombatPrepare && this.combatSkillPanel.NormalPagesCount == 5 && this.combatSkillPanel.OutlinePagesCount == 1;
			}
		}

		// Token: 0x060095F3 RID: 38387 RVA: 0x0045EB68 File Offset: 0x0045CD68
		public void PresetSkill(short templateId)
		{
			bool flag = base.gameObject.activeSelf && this.CurrCharIsTaiwu;
			if (!flag)
			{
				this.skillSelect.skillSortAndFilter.ClearAllFilter();
				this._skillTemplateId = templateId;
				this.skillSelect.SelectedTemplateId = templateId;
			}
		}

		// Token: 0x060095F4 RID: 38388 RVA: 0x0045EBB8 File Offset: 0x0045CDB8
		public void SetSkill(short templateId, bool resetFilter)
		{
			if (resetFilter)
			{
				this.skillSelect.skillSortAndFilter.ClearAllFilter();
			}
			this._skillTemplateId = -1;
			this.skillSelect.Select(templateId);
		}

		// Token: 0x060095F5 RID: 38389 RVA: 0x0045EBF0 File Offset: 0x0045CDF0
		public override void OnInit(ArgumentBox argsBox)
		{
		}

		// Token: 0x060095F6 RID: 38390 RVA: 0x0045EBF3 File Offset: 0x0045CDF3
		private void Awake()
		{
			this.InitCombatSkillScroll();
			this.InitCombatSkillPanel();
			this.InitBreakButtons();
			this.InitDesc();
			this.InitConflictExchangePanel();
		}

		// Token: 0x060095F7 RID: 38391 RVA: 0x0045EC1C File Offset: 0x0045CE1C
		private void OnEnable()
		{
			GEvent.Add(UiEvents.EnterCharacterMenuPractice, new GEvent.Callback(this.OnEnterFromEquipCombatSkill));
			this.InitConflictExchangePanel();
			this.skillPanel.SetActive(false);
			this.skillDesc.SetActive(false);
			this.notSelected.SetActive(true);
			this.notDesc.SetActive(true);
			this._skillData.Clear();
			this.UpdateVisible();
			this.RequestData();
			this.combatSkillPanel.CloseJumpPanel();
			GlobalDomainMethod.Call.InvokeGuidingTrigger(134);
		}

		// Token: 0x060095F8 RID: 38392 RVA: 0x0045ECB4 File Offset: 0x0045CEB4
		private void OnDestroy()
		{
			GEvent.Remove(UiEvents.EnterCharacterMenuPractice, new GEvent.Callback(this.OnEnterFromEquipCombatSkill));
			PoolItem textRowPool = this._textRowPool;
			if (textRowPool != null)
			{
				textRowPool.Destroy();
			}
			this._textRowPool = null;
			PoolItem doublePropertyRowPool = this._doublePropertyRowPool;
			if (doublePropertyRowPool != null)
			{
				doublePropertyRowPool.Destroy();
			}
			this._doublePropertyRowPool = null;
			PoolItem bonusNamePool = this._bonusNamePool;
			if (bonusNamePool != null)
			{
				bonusNamePool.Destroy();
			}
			this._bonusNamePool = null;
			PoolItem bonusEffectPool = this._bonusEffectPool;
			if (bonusEffectPool != null)
			{
				bonusEffectPool.Destroy();
			}
			this._bonusEffectPool = null;
		}

		// Token: 0x060095F9 RID: 38393 RVA: 0x0045ED44 File Offset: 0x0045CF44
		public override void OnCurrentCharacterChange(int prevCharacterId)
		{
			bool flag = base.CharacterMenu.CurCharacterId < 0;
			if (!flag)
			{
				bool isTaiwuTeamButNotBeast = this.IsTaiwuTeamButNotBeast;
				if (isTaiwuTeamButNotBeast)
				{
					this.localLoadingAnim.SetLoadingState(true);
				}
				this._skillData.Clear();
				this.UpdateVisible();
				this.RequestData();
			}
		}

		// Token: 0x060095FA RID: 38394 RVA: 0x0045ED97 File Offset: 0x0045CF97
		private void UpdateVisible()
		{
			this.contentPanel.SetActive(this.IsTaiwuTeamButNotBeast);
			this.invisiblePage.SetActive(!this.IsTaiwuTeamButNotBeast);
		}

		// Token: 0x060095FB RID: 38395 RVA: 0x0045EDC4 File Offset: 0x0045CFC4
		private void UpdateAll()
		{
			bool needCheckFold = this._needCheckFold;
			if (needCheckFold)
			{
				this._needCheckFold = false;
				this.DoFold(this.IsBroken);
			}
			bool needAutoSelect = this._needAutoSelect;
			if (needAutoSelect)
			{
				for (byte i = 0; i < 5; i += 1)
				{
					this._beforeReBreakSelectedPage = CombatSkillStateHelper.SetPageInactive(this._beforeReBreakSelectedPage, i);
				}
				this._currentSkillData.CombatSkillDisplayData.ActivationState = this._beforeReBreakSelectedPage;
				this._beforeReBreakSelectedPage = 0;
				this._needAutoSelect = false;
				this._keepOutline = true;
			}
			this.RefreshConflictExchangeState();
			this.UpdateBreakInfo();
			this.UpdateBreakPanel();
			this.UpdateBreakButtons();
			this.UpdateInnerRatioVisibility();
			this.UpdateDesc();
			this.UpdateFold();
		}

		// Token: 0x060095FC RID: 38396 RVA: 0x0045EE80 File Offset: 0x0045D080
		private void RequestData()
		{
			bool flag = !this.IsTaiwuTeamButNotBeast;
			if (flag)
			{
				this.localLoadingAnim.SetLoadingState(false);
			}
			else
			{
				CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForPractice(this, base.CharacterMenu.CurCharacterId, delegate(int offset1, RawDataPool dataPool1)
				{
					Serializer.Deserialize(dataPool1, offset1, ref this._data);
					CombatSkillDomainMethod.AsyncCall.GetCombatSkillDisplayDataForList(this, this._data.CharId, this._data.LearnedCombatSkills, delegate(int offset2, RawDataPool pool2)
					{
						List<CombatSkillDisplayDataForList> listData = new List<CombatSkillDisplayDataForList>();
						Serializer.Deserialize(pool2, offset2, ref listData);
						this.skillSelect.Set(listData, new Action<short>(this.OnCurrentSkillChange));
						bool flag2 = this.skillSelect.Contains(this._skillTemplateId);
						if (flag2)
						{
							this.SetSkill(this._skillTemplateId, false);
						}
						else
						{
							this.skillSelect.SelectWithoutNotify(-1);
							this.TurnOff();
							this.Element.ShowAfterRefresh();
							this.localLoadingAnim.SetLoadingState(false);
						}
					});
				});
				TaiwuDomainMethod.AsyncCall.GetExpandPracticePanel(this, delegate(int offset, RawDataPool pool)
				{
					Serializer.Deserialize(pool, offset, ref this._shouldExpandPanel);
				});
			}
		}

		// Token: 0x060095FD RID: 38397 RVA: 0x0045EEDC File Offset: 0x0045D0DC
		private void RequestCurrentSkillData()
		{
			this.RequestCurrentSkillData(true);
		}

		// Token: 0x060095FE RID: 38398 RVA: 0x0045EEE8 File Offset: 0x0045D0E8
		private void RequestCurrentSkillData(bool keepOutline)
		{
			bool flag = this._skillTemplateId < 0;
			if (!flag)
			{
				short requestedTemplateId = this._skillTemplateId;
				this._keepOutline = keepOutline;
				CombatSkillDomainMethod.AsyncCall.GetCombatSkillDisplayDataForListOnce(this, this._data.CharId, requestedTemplateId, delegate(int offset2, RawDataPool pool2)
				{
					bool flag2 = this._skillTemplateId != requestedTemplateId;
					if (!flag2)
					{
						CombatSkillDisplayDataForList listData = new CombatSkillDisplayDataForList();
						Serializer.Deserialize(pool2, offset2, ref listData);
						this.skillSelect.SetCell(listData);
					}
				});
				CombatSkillDomainMethod.AsyncCall.GetCombatSkillDisplayDataForPractice(this, this._data.CharId, requestedTemplateId, delegate(int offset, RawDataPool pool)
				{
					bool flag2 = this._skillTemplateId != requestedTemplateId;
					if (!flag2)
					{
						this._currentSkillData = null;
						Serializer.Deserialize(pool, offset, ref this._currentSkillData);
						this._skillData[requestedTemplateId] = this._currentSkillData.CombatSkillDisplayData;
						this.UpdateAll();
						this.Element.ShowAfterRefresh();
						this.localLoadingAnim.SetLoadingState(false);
					}
				});
			}
		}

		// Token: 0x060095FF RID: 38399 RVA: 0x0045EF70 File Offset: 0x0045D170
		private void TurnOff()
		{
			this._skillTemplateId = -1;
			this.notSelected.SetActive(true);
			this.notDesc.SetActive(true);
			this.skillPanel.SetActive(false);
			this.skillDesc.SetActive(false);
			GameObject gameObject = this.conflictExchangePanel;
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
		}

		// Token: 0x06009600 RID: 38400 RVA: 0x0045EFCC File Offset: 0x0045D1CC
		private void TurnOn(short templateId)
		{
			bool flag = templateId < 0;
			if (!flag)
			{
				this._skillTemplateId = templateId;
				this._needCheckFold = true;
				this.notSelected.SetActive(false);
				this.notDesc.SetActive(false);
				this.RequestCurrentSkillData(this._currentSkillData != null && this._currentSkillData.CombatSkillDisplayData.CharId == base.CharacterMenu.CurCharacterId && this._currentSkillData.CombatSkillDisplayData.TemplateId == this._skillTemplateId);
			}
		}

		// Token: 0x06009601 RID: 38401 RVA: 0x0045F054 File Offset: 0x0045D254
		private void OnEnterFromEquipCombatSkill(ArgumentBox argumentBox)
		{
			short templateId;
			bool flag = !argumentBox.Get("TemplateId", out templateId) || templateId < 0;
			if (!flag)
			{
				this._skillTemplateId = templateId;
				this.skillSelect.SelectedTemplateId = templateId;
				bool flag2 = this.skillSelect.Contains(templateId);
				if (flag2)
				{
					this.SetSkill(templateId, true);
				}
			}
		}

		// Token: 0x06009602 RID: 38402 RVA: 0x0045F0AA File Offset: 0x0045D2AA
		private void InitCombatSkillScroll()
		{
			this.skillSelect.Init("ViewCharacterMenuPractice", true);
		}

		// Token: 0x06009603 RID: 38403 RVA: 0x0045F0BF File Offset: 0x0045D2BF
		private void InitCombatSkillPanel()
		{
			this.combatSkillPanel.Init(new Action(this.RequestCurrentSkillData), new Action<CombatSkillDisplayData>(this.OnCombatSkillPanelChange), base.GetComponent<RectTransform>(), () => base.CharacterMenu.CanOperate && this.CanInteract, null);
		}

		// Token: 0x06009604 RID: 38404 RVA: 0x0045F0FC File Offset: 0x0045D2FC
		private void InitBreakButtons()
		{
			this.btnBreak.GetComponent<CButton>().ClearAndAddListener(delegate
			{
				this.OnBreakBtnClick(false);
			});
			this.btnReview.GetComponent<CButton>().ClearAndAddListener(delegate
			{
				this.OnBreakBtnClick(true);
			});
			this.btnContinue.GetComponent<CButton>().ClearAndAddListener(delegate
			{
				this.OnBreakBtnClick(false);
			});
			this.btnReBreak.GetComponent<CButton>().ClearAndAddListener(new Action(this.OnReBreakBtnClick));
		}

		// Token: 0x06009605 RID: 38405 RVA: 0x0045F180 File Offset: 0x0045D380
		private void InitDesc()
		{
			this._textRowPool = new PoolItem("ViewCharacterMenuPracticePool0", this.textRow);
			this._doublePropertyRowPool = new PoolItem("ViewCharacterMenuPracticePool1", this.doublePropertyRow);
			this._bonusNamePool = new PoolItem("ViewCharacterMenuPracticePool2", this.bonusNameTemplate);
			this._bonusEffectPool = new PoolItem("ViewCharacterMenuPracticePool3", this.bonusEffectTemplate);
			this.breakPowerTips.PresetParam = new string[]
			{
				LanguageKey.LK_Skill_Break_InfoTip_Title.Tr(),
				""
			};
			this.breakDifficultyTips.PresetParam = new string[]
			{
				LanguageKey.LK_CombatSkill_Practice_NotBroken_Tiile_Difficulty.Tr(),
				""
			};
		}

		// Token: 0x06009606 RID: 38406 RVA: 0x0045F234 File Offset: 0x0045D434
		private void UpdateBreakInfo()
		{
			bool flag = !this.CurrCharIsTaiwu;
			if (flag)
			{
				this.breakInfo.SetActive(false);
			}
			else
			{
				ValueTuple<string, string, string> qualification = this.GetQualification();
				this._qualificationName = qualification.Item1;
				this._qualificationValue = qualification.Item2;
				this._qualificationIcon = qualification.Item3;
				bool isBroken = this.IsBroken;
				if (isBroken)
				{
					this.breakQualification.gameObject.SetActive(false);
					this.breakPower.gameObject.SetActive(true);
				}
				else
				{
					this.breakQualification.SetValue(this._qualificationValue);
					this.breakQualificationIcon.SetSprite(this._qualificationIcon, false, null);
					this.breakQualification.gameObject.SetActive(true);
					this.breakPower.gameObject.SetActive(false);
				}
				bool isLuohanBreak = this.IsLuohanBreak;
				if (isLuohanBreak)
				{
					int maxPower = GameData.Domains.Taiwu.SharedMethods.GetLuohanBreakMaxPower(this._skillTemplateId, this._data.CombatSkillQualifications);
					string[] value = CommonUtils.GetBreakoutMaxPowerName(maxPower, this._skillTemplateId);
					this.breakPower.SetValue(value[0]);
					this.breakPowerTips.PresetParam[1] = LanguageKey.LK_Skill_Break_InfoTip_Power_Desc.TrFormat(maxPower).ColorReplace() + "\n" + LanguageKey.LK_Skill_Break_Cell_Power.TrFormat(value[1]).ColorReplace();
				}
				else
				{
					bool flag2 = this.Plate != null;
					if (flag2)
					{
						string[] value2 = CommonUtils.GetBreakoutMaxPowerName(this.Plate.AddMaxPower, this._skillTemplateId);
						this.breakPower.SetValue(value2[0]);
						this.breakPowerTips.PresetParam[1] = LanguageKey.LK_Skill_Break_InfoTip_Power_Desc.TrFormat(this.Plate.AddMaxPower).ColorReplace() + "\n" + LanguageKey.LK_Skill_Break_Cell_Power.TrFormat(value2[1]).ColorReplace();
					}
				}
				bool flag3 = this._currentSkillData.CombatSkillBreakSuccessRateDisplayData != null;
				if (flag3)
				{
					CombatSkillBreakSuccessRateDisplayData displayData = this._currentSkillData.CombatSkillBreakSuccessRateDisplayData;
					int level = this.GetDifficultyLevel((int)displayData.BaseSuccessRate);
					string value3 = LocalStringManager.Get(string.Format("LK_CombatSkill_Practice_NotBroken_Difficulty_{0}", level)).SetColor(this._levelColor[level]);
					this.breakDifficulty.SetValue(value3);
					this.breakProb.SetValue(string.Format("{0}%", displayData.BaseSuccessRate));
					this.breakDifficultyTips.PresetParam[1] = string.Concat(new string[]
					{
						LanguageKey.LK_Skill_Break_Tip_Content_Difficult.Tr(),
						"\n\n",
						LanguageKey.LK_CombatSkill_Practice_NotBroken_Tiile_Difficulty.Tr(),
						": ",
						value3,
						"\n",
						LanguageKey.LK_CombatSkill_Practice_NotBroken_Prob_Bonus_Intelligence.Tr(),
						": +",
						displayData.AttributesBonus.ToString(),
						"%\n",
						LanguageKey.LK_CombatSkill_Practice_NotBroken_Prob_Bonus_Organization.Tr(),
						": +",
						displayData.OrganizationBonus.ToString(),
						"%\n",
						LanguageKey.LK_CombatSkill_Practice_NotBroken_Prob_Bonus_Consummate.Tr(),
						": +",
						displayData.ConsummateLevelBonus.ToString(),
						"%\n",
						LanguageKey.LK_CombatSkill_Practice_NotBroken_Prob_Bonus_Building.Tr(),
						": +",
						displayData.BuildingBonus.ToString(),
						"%\n"
					});
					this.breakProbTips.PresetParam = this.breakDifficultyTips.PresetParam;
				}
				bool flag4 = this._currentSkillData.CombatSkillBreakAvailableStepsDisplayData != null;
				if (flag4)
				{
					CombatSkillBreakAvailableStepsDisplayData displayData2 = this._currentSkillData.CombatSkillBreakAvailableStepsDisplayData;
					this.breakStep.SetValue(this._currentSkillData.DisplayAvailableSteps.ToString());
					this.CommonRefreshStepTips(this.breakStepTips, this._qualificationIcon, this._qualificationName, this._qualificationValue, (int)displayData2.OrganizationBonus, (int)displayData2.ConsummateLevelBonus, (int)displayData2.BuildingBonus);
					this.CommonRefreshStepTips(this.breakQualificationTips, this._qualificationIcon, this._qualificationName, this._qualificationValue, (int)displayData2.OrganizationBonus, (int)displayData2.ConsummateLevelBonus, (int)displayData2.BuildingBonus);
				}
				this.breakInfo.SetActive(true);
			}
		}

		// Token: 0x06009607 RID: 38407 RVA: 0x0045F67C File Offset: 0x0045D87C
		private void UpdateBreakPanel()
		{
			CombatSkillDisplayData skillData = this._skillData[this._skillTemplateId];
			bool flag = skillData.EffectType == 0;
			if (flag)
			{
				this.directBack.SetActive(true);
				this.reverseBack.SetActive(false);
			}
			else
			{
				bool flag2 = skillData.EffectType == 1;
				if (flag2)
				{
					this.directBack.SetActive(false);
					this.reverseBack.SetActive(true);
				}
				else
				{
					this.directBack.SetActive(false);
					this.reverseBack.SetActive(false);
				}
			}
			this.combatSkillPanel.Set(this._currentSkillData, this._data, this._keepOutline);
		}

		// Token: 0x06009608 RID: 38408 RVA: 0x0045F72C File Offset: 0x0045D92C
		private void UpdateBreakButtons()
		{
			CombatSkillDisplayData combatSkillDisplayData;
			bool flag = this._skillTemplateId < 0 || !this._skillData.TryGetValue(this._skillTemplateId, out combatSkillDisplayData);
			if (!flag)
			{
				bool flag2 = !this.CurrCharIsTaiwu;
				if (flag2)
				{
					this.breakButtons.SetActive(false);
				}
				else
				{
					bool flag3 = !this.CanInteract;
					if (flag3)
					{
						this.breakButtons.SetActive(false);
					}
					else
					{
						this.UpdateBreakAndContinueButton();
						this.UpdateReBreakButton();
						this.UpdateReviewButton();
						bool isLuohanBreak = this.IsLuohanBreak;
						if (isLuohanBreak)
						{
							this.btnBreak.GetComponent<CButton>().interactable = false;
							this.btnContinue.GetComponent<CButton>().interactable = false;
							this.btnReBreak.GetComponent<CButton>().interactable = false;
							this.btnReview.GetComponent<CButton>().interactable = false;
						}
						this.breakButtons.SetActive(true);
					}
				}
			}
		}

		// Token: 0x06009609 RID: 38409 RVA: 0x0045F81A File Offset: 0x0045DA1A
		private void UpdateInnerRatioVisibility()
		{
			this.combatSkillPanel.SetInnerRatioVisible(this.CanInteract);
		}

		// Token: 0x0600960A RID: 38410 RVA: 0x0045F830 File Offset: 0x0045DA30
		private void UpdateDesc()
		{
			CombatSkillDisplayData data = this._skillData[this._skillTemplateId];
			CombatSkillItem config = CombatSkill.Instance[data.TemplateId];
			ushort state = this.IsBroken ? data.ActivationState : this.GetSelectedPage();
			sbyte direction = CombatSkillStateHelper.GetCombatSkillDirection(state);
			this.skillTitle.text = config.Name;
			string directColor = (direction == 0) ? "8dc3c3" : "818181";
			string reverseColor = (direction == 1) ? "ffc0c0" : "818181";
			this.direct.text = ("     " + CommonUtils.GetSpecialEffectDesc(config.DirectEffectID)).SetColor(directColor);
			this.reverse.text = ("     " + CommonUtils.GetSpecialEffectDesc(config.ReverseEffectID)).SetColor(reverseColor);
			this.directIcon.SetSprite("ui9_icon_combat_skill_effect_direction_" + ((direction == 0) ? "0" : "2"), false, null);
			this.reverseIcon.SetSprite("ui9_icon_combat_skill_effect_direction_" + ((direction == 1) ? "1" : "3"), false, null);
			this.RefreshPageEffectGrid();
			bool isBroken = this.IsBroken;
			if (isBroken)
			{
				this.RefreshBonusEffectGrid();
				int num;
				if (!this.IsLuohanBreak)
				{
					GameData.Domains.Taiwu.SkillBreakPlate plate = this.Plate;
					num = ((plate != null) ? plate.AddMaxPower : 0);
				}
				else
				{
					num = GameData.Domains.Taiwu.SharedMethods.GetLuohanBreakMaxPower(this._skillTemplateId, this._data.CombatSkillQualifications);
				}
				int maxPower = num;
				this.breakMaxPower.text = CommonUtils.GetBreakoutMaxPowerName(maxPower, data.TemplateId)[0];
				this.breakCurrentPower.text = string.Format("{0}%", data.Power);
				this.breakEffect.SetActive(true);
			}
			else
			{
				this.breakEffect.SetActive(false);
				this.bonusEffect.SetActive(false);
			}
		}

		// Token: 0x0600960B RID: 38411 RVA: 0x0045FA10 File Offset: 0x0045DC10
		private void UpdateFold()
		{
			bool flag = this._skillTemplateId >= 0;
			if (flag)
			{
				int index = this.IsBroken ? 1 : 0;
				this.DoFold((this._shouldExpandPanel[index] == 0) ? this.IsBroken : (this._shouldExpandPanel[index] > 0));
			}
		}

		// Token: 0x0600960C RID: 38412 RVA: 0x0045FA64 File Offset: 0x0045DC64
		private void OnCurrentSkillChange(short templateId)
		{
			bool flag = this._skillTemplateId == templateId;
			if (flag)
			{
				this.skillSelect.SelectWithoutNotify(templateId);
			}
			else
			{
				this.TurnOn(templateId);
			}
		}

		// Token: 0x0600960D RID: 38413 RVA: 0x0045FA97 File Offset: 0x0045DC97
		private void OnCombatSkillPanelChange(CombatSkillDisplayData data)
		{
			this._skillData[this._skillTemplateId] = data;
			this.UpdateBreakButtons();
			this.UpdateDesc();
		}

		// Token: 0x0600960E RID: 38414 RVA: 0x0045FABC File Offset: 0x0045DCBC
		private void OnBreakBtnClick(bool isReview)
		{
			ushort selectedPage = isReview ? this.Plate.SelectedPages : this.GetSelectedPage();
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("SkillId", this._skillTemplateId);
			argBox.Set("SelectedPage", selectedPage);
			argBox.Set("IsReview", isReview);
			argBox.Set("QualificationTypeName", this._qualificationName);
			argBox.Set("QualificationTypeIcon", this._qualificationIcon);
			argBox.Set("QualificationRequireValue", this._qualificationValue);
			UIElement.SkillBreakPlate.SetOnInitArgs(argBox);
			UIElement skillBreakPlate = UIElement.SkillBreakPlate;
			skillBreakPlate.OnHide = (Action)Delegate.Combine(skillBreakPlate.OnHide, new Action(this.RequestCurrentSkillData));
			UIManager.Instance.ShowUI(UIElement.SkillBreakPlate, true);
		}

		// Token: 0x0600960F RID: 38415 RVA: 0x0045FB8C File Offset: 0x0045DD8C
		private void OnReBreakBtnClick()
		{
			string content = LocalStringManager.Get((this._data.LoopingNeigong == this._skillTemplateId) ? LanguageKey.LK_RebreakLoopingSkill : LanguageKey.LK_Skill_Break_Rebreak_Confirm);
			DialogCmd cmd = new DialogCmd
			{
				Title = LocalStringManager.Get(LanguageKey.LK_Skill_Break_Rebreak),
				Content = content,
				Type = 1,
				Yes = delegate()
				{
					this._needAutoSelect = true;
					this._beforeReBreakSelectedPage = this._skillData[this._skillTemplateId].ActivationState;
					TaiwuDomainMethod.Call.ClearBreakPlate(this._skillTemplateId);
					this.RequestCurrentSkillData();
				}
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x06009610 RID: 38416 RVA: 0x0045FC20 File Offset: 0x0045DE20
		private void UpdateBreakAndContinueButton()
		{
			bool isBroken = this.IsBroken;
			if (isBroken)
			{
				this.btnBreak.SetActive(false);
				this.btnContinue.SetActive(false);
			}
			else
			{
				bool flag = this.Plate == null;
				if (flag)
				{
					this.btnBreak.SetActive(true);
					this.btnContinue.SetActive(false);
				}
				else
				{
					bool flag2 = !this.Plate.Failed;
					if (!flag2)
					{
						this.btnBreak.SetActive(false);
						this.btnContinue.SetActive(false);
						return;
					}
					this.btnBreak.SetActive(false);
					this.btnContinue.SetActive(true);
				}
				this.btnBreak.GetComponent<CButton>().interactable = this.CanBreak;
				this.btnContinue.GetComponent<CButton>().interactable = this.CanBreak;
				int directPageCount = 0;
				int reversePageCount = 0;
				int count = 10;
				for (int i = 0; i < count; i++)
				{
					bool flag3 = this.combatSkillPanel.IsNormalPageActivated(i);
					if (flag3)
					{
						bool flag4 = this.CheckIsDirectByToggleIndex(i);
						if (flag4)
						{
							directPageCount++;
						}
						else
						{
							reversePageCount++;
						}
					}
				}
				this.UpdateGeneralTips(this.btnBreak.GetComponent<TooltipInvoker>(), LanguageKey.LK_Skill_Break.Tr(), directPageCount, reversePageCount);
				this.UpdateGeneralTips(this.btnContinue.GetComponent<TooltipInvoker>(), LanguageKey.LK_Skill_Break_Continue.Tr(), directPageCount, reversePageCount);
			}
		}

		// Token: 0x06009611 RID: 38417 RVA: 0x0045FD90 File Offset: 0x0045DF90
		private void UpdateReBreakButton()
		{
			bool flag = !this.IsBroken && this.Plate == null;
			if (flag)
			{
				this.btnReBreak.SetActive(false);
			}
			else
			{
				this.btnReBreak.SetActive(true);
				TooltipInvoker tip = this.btnReBreak.GetComponent<TooltipInvoker>();
				TooltipInvoker tooltipInvoker = tip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				tip.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Skill_Break_Rebreak));
				bool isLuohanBreak = this.IsLuohanBreak;
				if (isLuohanBreak)
				{
					tip.RuntimeParam.Set("arg1", LanguageKey.LK_Skill_Break_Luohan_Content.Tr());
					this.btnReBreak.GetComponent<CButton>().interactable = false;
				}
				else
				{
					bool canReBreak = true;
					StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
					strBuilder.Clear();
					strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Skill_Break_Rebreak_Desc));
					bool flag2 = this.Plate == null && !this.IsLuohanBreak;
					if (flag2)
					{
						this.reBreakCd.SetActive(false);
					}
					else
					{
						bool flag3 = !this.IsRevoked;
						if (flag3)
						{
							bool flag4 = this.Plate == null;
							if (flag4)
							{
								canReBreak = false;
							}
							bool isInReBreakCd = this._currentSkillData.ReBreakCd > 0;
							this.reBreakCd.SetActive(isInReBreakCd);
							bool flag5 = isInReBreakCd;
							if (flag5)
							{
								this.reBreakCdText.text = this._currentSkillData.ReBreakCd.ToString();
								strBuilder.Append("\n");
								strBuilder.Append(LocalStringManager.GetFormat(LanguageKey.LK_Skill_Break_Rebreak_Cd_Tips, this._currentSkillData.ReBreakCd).SetColor("brightred"));
								canReBreak = false;
							}
							bool openFromCombatPrepare = base.CharacterMenu.OpenFromCombatPrepare;
							if (openFromCombatPrepare)
							{
								canReBreak = false;
							}
						}
						else
						{
							canReBreak = false;
						}
					}
					tip.RuntimeParam.Set("arg1", strBuilder.ToString());
					EasyPool.Free<StringBuilder>(strBuilder);
					this.btnReBreak.GetComponent<CButton>().interactable = canReBreak;
				}
			}
		}

		// Token: 0x06009612 RID: 38418 RVA: 0x0045FF90 File Offset: 0x0045E190
		private void UpdateReviewButton()
		{
			bool flag;
			if (!this.IsBroken)
			{
				GameData.Domains.Taiwu.SkillBreakPlate plate = this.Plate;
				flag = (plate == null || !plate.Failed);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				this.btnReview.SetActive(false);
			}
			else
			{
				this.btnReview.SetActive(true);
				this.btnReview.GetComponent<CButton>().interactable = (!this.IsRevoked && !base.CharacterMenu.OpenFromCombatPrepare && this.Plate != null);
				TooltipInvoker tips = this.btnReview.GetComponent<TooltipInvoker>();
				bool isLuohanBreak = this.IsLuohanBreak;
				if (isLuohanBreak)
				{
					tips.PresetParam = new string[]
					{
						LanguageKey.LK_Skill_Break_Review.Tr(),
						LanguageKey.LK_Skill_Break_Luohan_Content.Tr()
					};
				}
				else
				{
					tips.PresetParam = new string[]
					{
						LanguageKey.LK_Skill_Break_Review.Tr(),
						LanguageKey.LK_Skill_Break_Review_Tip_Content.Tr()
					};
				}
			}
		}

		// Token: 0x06009613 RID: 38419 RVA: 0x0046007D File Offset: 0x0045E27D
		private void DoFold(bool isFold)
		{
		}

		// Token: 0x06009614 RID: 38420 RVA: 0x00460080 File Offset: 0x0045E280
		private bool CheckIsDirectByToggleIndex(int index)
		{
			return index < 5;
		}

		// Token: 0x06009615 RID: 38421 RVA: 0x00460098 File Offset: 0x0045E298
		private int GetActualIndex(int index)
		{
			return (int)((byte)(this.CheckIsDirectByToggleIndex(index) ? index : (index - 5)));
		}

		// Token: 0x06009616 RID: 38422 RVA: 0x004600BC File Offset: 0x0045E2BC
		private byte GetPageId(int index)
		{
			return (byte)(this.GetActualIndex(index) + 1);
		}

		// Token: 0x06009617 RID: 38423 RVA: 0x004600D8 File Offset: 0x0045E2D8
		private ushort GetSelectedPage()
		{
			List<int> togList = this.combatSkillPanel.ActivatedNormalPages;
			ushort selectedPage = 0;
			int outlineActiveToggle = this.combatSkillPanel.ActivatedOutlinePage;
			bool flag = outlineActiveToggle >= 0;
			if (flag)
			{
				selectedPage = CombatSkillStateHelper.SetPageRead(selectedPage, (byte)outlineActiveToggle);
			}
			foreach (int i in togList)
			{
				selectedPage = CombatSkillStateHelper.SetPageRead(selectedPage, CombatSkillStateHelper.GetPageInternalIndex(-1, CombatSkillPanel.GetDirection(i), this.GetPageId(i)));
			}
			return selectedPage;
		}

		// Token: 0x06009618 RID: 38424 RVA: 0x00460178 File Offset: 0x0045E378
		private int GetDifficultyLevel(int successRate)
		{
			if (!true)
			{
			}
			int result;
			if (successRate >= 70)
			{
				if (successRate < 90)
				{
					result = 1;
				}
				else
				{
					result = 0;
				}
			}
			else if (successRate < 50)
			{
				result = 3;
			}
			else
			{
				result = 2;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06009619 RID: 38425 RVA: 0x004601B8 File Offset: 0x0045E3B8
		private unsafe ValueTuple<string, string, string> GetQualification()
		{
			CombatSkillItem configData = CombatSkill.Instance[this._skillTemplateId];
			SkillGradeDataItem skillGradeCfg = SkillGradeData.Instance[configData.Grade];
			short requireQualification = skillGradeCfg.PracticeQualificationRequirement;
			short currQualification = *this._data.CombatSkillQualifications[(int)configData.Type];
			bool isCombatSkillQualification = true;
			sbyte lifeSkillType;
			short lifeSkillQualification = GameData.Domains.Taiwu.SharedMethods.GetQualificationWithSectApprovalBonus(configData.SectId, currQualification, this._data.LifeSkillQualifications, out lifeSkillType);
			bool flag = lifeSkillQualification > currQualification;
			if (flag)
			{
				currQualification = lifeSkillQualification;
				isCombatSkillQualification = false;
			}
			string qualificationTypeName = isCombatSkillQualification ? Config.CombatSkillType.Instance[configData.Type].Name : Config.LifeSkillType.Instance[lifeSkillType].Name;
			string qualificationTypeIcon = isCombatSkillQualification ? string.Format("{0}{1}", "ui9_back_attainments_combat_3_", configData.Type) : string.Format("{0}{1}", "ui9_back_attainments_life_3_", lifeSkillType);
			string currQualificationText = currQualification.ToString().SetColor((currQualification > requireQualification) ? "brightblue" : "brightred");
			return new ValueTuple<string, string, string>(qualificationTypeName, currQualificationText + "/" + requireQualification.ToString(), qualificationTypeIcon);
		}

		// Token: 0x0600961A RID: 38426 RVA: 0x004602E0 File Offset: 0x0045E4E0
		private void UpdateGeneralTips(TooltipInvoker tips, string title, int directPageCount, int reversePageCount)
		{
			bool isLuohanBreak = this.IsLuohanBreak;
			if (isLuohanBreak)
			{
				tips.Type = TipType.Simple;
				tips.PresetParam = new string[]
				{
					title,
					LanguageKey.LK_Skill_Break_Luohan_Content.Tr()
				};
			}
			tips.Type = TipType.CombatSkillBreakout;
			tips.RuntimeParam = EasyPool.Get<ArgumentBox>();
			tips.RuntimeParam.Set("SkillId", this._skillTemplateId).Set("SelectedFirstPageCount", this.combatSkillPanel.OutlinePagesCount).Set("SelectedOtherPageCount", this.combatSkillPanel.NormalPagesCount).Set("DirectPageCount", directPageCount).Set("ReversePageCount", reversePageCount);
		}

		// Token: 0x0600961B RID: 38427 RVA: 0x0046038C File Offset: 0x0045E58C
		private void RefreshPageEffectGrid()
		{
			foreach (GameObject obj in this._textRows)
			{
				this._textRowPool.DestroyObject(obj);
			}
			foreach (GameObject obj2 in this._doublePropertyRows)
			{
				this._doublePropertyRowPool.DestroyObject(obj2);
			}
			this._textRows.Clear();
			this._doublePropertyRows.Clear();
			List<SkillBreakPageEffectDisplay> allEffectList = EasyPool.Get<List<SkillBreakPageEffectDisplay>>();
			List<SkillBreakPageEffectDisplay> pageEffectList = EasyPool.Get<List<SkillBreakPageEffectDisplay>>();
			allEffectList.Clear();
			for (int i = 0; i < 5; i++)
			{
				bool flag = this.combatSkillPanel.IsNormalPageActivated(i);
				if (flag)
				{
					PageEffectHelper.GenerateNormalPageEffects(this._skillTemplateId, i, pageEffectList);
					allEffectList.AddRange(pageEffectList);
				}
				bool flag2 = this.combatSkillPanel.IsNormalPageActivated(i + 5);
				if (flag2)
				{
					PageEffectHelper.GenerateNormalPageEffects(this._skillTemplateId, i + 5, pageEffectList);
					allEffectList.AddRange(pageEffectList);
				}
			}
			Transform first = null;
			Transform last = null;
			foreach (SkillBreakPageEffectDisplay effect in allEffectList)
			{
				bool flag3 = effect.EffectTitle.IsNullOrEmpty() && effect.Icon.IsNullOrEmpty();
				if (flag3)
				{
					Transform obj3 = this._textRowPool.GetObject().transform;
					obj3.GetChild(0).GetComponent<TextMeshProUGUI>().text = effect.EffectDesc.ColorReplace();
					obj3.SetParent(this.pageEffectParent, false);
					obj3.SetAsFirstSibling();
					this._textRows.Add(obj3.gameObject);
					first = obj3;
				}
				else
				{
					bool hasLast = last != null;
					Transform obj4 = hasLast ? last : this._doublePropertyRowPool.GetObject().transform;
					Transform item = obj4.GetChild(hasLast ? 1 : 0);
					item.GetChild(0).GetComponent<CImage>().SetSprite(effect.Icon, false, null);
					item.GetChild(1).GetComponent<TextMeshProUGUI>().text = effect.EffectTitle.ColorReplace();
					item.GetChild(2).GetComponent<TextMeshProUGUI>().text = effect.EffectDesc.ColorReplace();
					item.gameObject.SetActive(true);
					obj4.SetParent(this.pageEffectParent, false);
					this._doublePropertyRows.Add(obj4.gameObject);
					last = (hasLast ? null : obj4);
				}
			}
			bool flag4 = first != null;
			if (flag4)
			{
				first.SetAsFirstSibling();
			}
			bool flag5 = last != null;
			if (flag5)
			{
				last.GetChild(1).gameObject.SetActive(false);
			}
			EasyPool.Free<List<SkillBreakPageEffectDisplay>>(pageEffectList);
			EasyPool.Free<List<SkillBreakPageEffectDisplay>>(allEffectList);
			int activeDirectChildCount = 0;
			foreach (object obj5 in this.pageEffectParent.transform)
			{
				Transform child = (Transform)obj5;
				bool activeSelf = child.gameObject.activeSelf;
				if (activeSelf)
				{
					activeDirectChildCount++;
				}
			}
			this.lineObj.SetActive(activeDirectChildCount > 0);
		}

		// Token: 0x0600961C RID: 38428 RVA: 0x00460760 File Offset: 0x0045E960
		private void RefreshBonusEffectGrid()
		{
			foreach (GameObject obj in this._bonusNames)
			{
				this._bonusNamePool.DestroyObject(obj);
			}
			foreach (GameObject obj2 in this._bonusEffects)
			{
				this._bonusEffectPool.DestroyObject(obj2);
			}
			this._bonusNames.Clear();
			this._bonusEffects.Clear();
			bool flag = this._currentSkillData.Bonuses == null || this._currentSkillData.Bonuses.Count == 0;
			if (flag)
			{
				this.bonusEffect.gameObject.SetActive(false);
			}
			else
			{
				List<SkillBreakPlateBonus> bonuses = this._currentSkillData.Bonuses;
				List<ValueTuple<SkillBreakPlateBonus, List<SkillBreakBonusEffectDisplay>>> result = new List<ValueTuple<SkillBreakPlateBonus, List<SkillBreakBonusEffectDisplay>>>();
				foreach (SkillBreakPlateBonus bonus2 in bonuses)
				{
					bool flag2 = bonus2.Type == ESkillBreakPlateBonusType.None;
					if (!flag2)
					{
						List<SkillBreakBonusEffectDisplay> bonusEffectList = EasyPool.Get<List<SkillBreakBonusEffectDisplay>>();
						SkillBreakBonusEffectHelper.GenerateBonusEffectDisplays(this._skillTemplateId, bonus2, this._data.LifeSkillAttainments, bonusEffectList);
						result.Add(new ValueTuple<SkillBreakPlateBonus, List<SkillBreakBonusEffectDisplay>>(bonus2, bonusEffectList));
					}
				}
				this.bonusEffect.SetActive(result.Count > 0);
				for (int i = 0; i < result.Count; i++)
				{
					ValueTuple<SkillBreakPlateBonus, List<SkillBreakBonusEffectDisplay>> valueTuple = result[i];
					SkillBreakPlateBonus bonus = valueTuple.Item1;
					List<SkillBreakBonusEffectDisplay> bonusEffectList2 = valueTuple.Item2;
					Transform nameObj = this._bonusNamePool.GetObject().transform;
					nameObj.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = LanguageKey.LK_Skill_Break_BonusInformation_Type.TrFormat(LocalStringManager.Get(string.Format("LK_Number{0}", i + 1)));
					bool isLuohanBreak = this.IsLuohanBreak;
					if (isLuohanBreak)
					{
						nameObj.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = Luohan.Instance[this._currentSkillData.CombatSkillDisplayData.LuohanId].Name.SetGradeColor(8);
					}
					else
					{
						SkillBreakPlateUtils.AsyncGetBonusName(this, bonus, delegate(string bonusName)
						{
							nameObj.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = bonusName.SetGradeColor((int)bonus.Grade);
						});
					}
					nameObj.SetParent(this.bonusEffectParent, false);
					this._bonusNames.Add(nameObj.gameObject);
					Transform last = null;
					foreach (SkillBreakBonusEffectDisplay effect in bonusEffectList2)
					{
						bool hasLast = last != null;
						Transform obj3 = hasLast ? last : this._bonusEffectPool.GetObject().transform;
						Transform item = obj3.GetChild(hasLast ? 1 : 0);
						item.GetComponent<SkillBreakBonusEffect>().Refresh(effect, SkillBreakBonusEffect.EBonusIconSize.Small);
						item.gameObject.SetActive(true);
						obj3.SetParent(this.bonusEffectParent, false);
						this._bonusEffects.Add(obj3.gameObject);
						last = (hasLast ? null : obj3);
					}
					bool flag3 = last != null;
					if (flag3)
					{
						last.GetChild(1).gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x0600961D RID: 38429 RVA: 0x00460B30 File Offset: 0x0045ED30
		private void CommonRefreshStepTips(TooltipInvoker tip, string qualificationTypeIcon, string qualificationTypeName, string qualificationRequireValue, int orgBonusValue, int consummateBonusValue, int buildingBonusValue)
		{
			List<object> extraArgs = new List<object>
			{
				20
			};
			GeneralLineData desc1Line = new GeneralLineData
			{
				Type = 11,
				Args = new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Skill_Break_Step_Tip_Desc_1)
				}
			};
			GeneralLineData subTitleQualificationLine = new GeneralLineData
			{
				Type = 1,
				Args = new List<string>
				{
					LocalStringManager.GetFormat(LanguageKey.LK_Brackets_Symbol, LocalStringManager.Get(LanguageKey.LK_Skill_Break_Step_Tip_Title))
				}
			};
			string iconString = string.IsNullOrEmpty(qualificationTypeIcon) ? LanguageKey.LK_None.Tr() : string.Concat(new string[]
			{
				"<SpName=",
				qualificationTypeIcon,
				">",
				qualificationTypeName,
				"  ",
				qualificationRequireValue
			});
			GeneralLineData desc2Line = new GeneralLineData
			{
				Type = 11,
				Args = new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Break_Require_Qualification) + "：" + iconString,
					"true"
				},
				ExtraArgs = extraArgs
			};
			GeneralLineData desc3Line = new GeneralLineData
			{
				Type = 11,
				Args = new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Skill_Break_Step_Tip_Desc_5)
				},
				ExtraArgs = extraArgs
			};
			GeneralLineData subTitleMadLine = new GeneralLineData
			{
				Type = 1,
				Args = new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Skill_Break_Step_Tip_SubTitle_1)
				}
			};
			GeneralLineData desc4Line = new GeneralLineData
			{
				Type = 11,
				Args = new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Skill_Break_Step_Tip_Desc_2)
				},
				ExtraArgs = extraArgs
			};
			GeneralLineData desc5Line = new GeneralLineData
			{
				Type = 11,
				Args = new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Skill_Break_Step_Tip_Desc_3)
				},
				ExtraArgs = extraArgs
			};
			GeneralLineData desc6Line = new GeneralLineData
			{
				Type = 11,
				Args = new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Skill_Break_Step_Tip_Desc_4)
				},
				ExtraArgs = extraArgs
			};
			GeneralLineData space = new GeneralLineData
			{
				Type = 4,
				PreferredHeight = 15f
			};
			if (tip.RuntimeParam == null)
			{
				tip.RuntimeParam = new ArgumentBox();
			}
			tip.Type = TipType.GeneralLines;
			tip.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_Skill_Break_Step_Tip_Title)).SetObject("LineData1", desc1Line).SetObject("LineData2", space).SetObject("LineData3", subTitleQualificationLine).SetObject("LineData4", desc2Line).SetObject("LineData5", desc3Line).SetObject("LineData6", subTitleMadLine).SetObject("LineData7", desc4Line).SetObject("LineData8", desc5Line).SetObject("LineData9", desc6Line).Set("LineCount", 9);
		}

		// Token: 0x0600961E RID: 38430 RVA: 0x00460E00 File Offset: 0x0045F000
		private void InitConflictExchangePanel()
		{
			bool flag = this._conflictPanelInited || this.conflictExchangePanel == null;
			if (!flag)
			{
				this._conflictPanelInited = true;
				this.conflictExchangePanel.SetActive(false);
				ConflickSkillDisplayItem conflickSkillDisplayItem = this.currentSide;
				if (conflickSkillDisplayItem != null)
				{
					conflickSkillDisplayItem.Init(new Action<int, int>(this.OnCurrentPresetChange), delegate
					{
						this.OnClickSelectPracticeDialogCmd(false);
					});
				}
				ConflickSkillDisplayItem conflickSkillDisplayItem2 = this.pastSide;
				if (conflickSkillDisplayItem2 != null)
				{
					conflickSkillDisplayItem2.Init(new Action<int, int>(this.OnPastPresetChange), delegate
					{
						this.OnClickSelectPracticeDialogCmd(true);
					});
				}
			}
		}

		// Token: 0x0600961F RID: 38431 RVA: 0x00460E93 File Offset: 0x0045F093
		private void OnConflictCombatSkillResolved(short _, bool __)
		{
			this.SetLoadingState(true);
			this.RequestData();
		}

		// Token: 0x06009620 RID: 38432 RVA: 0x00460EA8 File Offset: 0x0045F0A8
		private void RefreshConflictExchangeState()
		{
			this.InitConflictExchangePanel();
			CombatSkillDisplayData skillData;
			bool flag = this._skillTemplateId < 0 || !this._skillData.TryGetValue(this._skillTemplateId, out skillData);
			if (!flag)
			{
				bool isDreamBack = SingletonObject.getInstance<BasicGameData>().IsDreamBack;
				bool showConflict = isDreamBack && this.CurrCharIsTaiwu && this.conflictExchangePanel != null && skillData.Conflicting;
				GameObject gameObject = this.conflictExchangePanel;
				if (gameObject != null)
				{
					gameObject.SetActive(showConflict);
				}
				this.skillPanel.SetActive(!showConflict);
				this.skillDesc.SetActive(!showConflict);
				bool flag2 = showConflict;
				if (flag2)
				{
					this.RefreshConflictExchangePanel(this._skillTemplateId, skillData);
				}
			}
		}

		// Token: 0x06009621 RID: 38433 RVA: 0x00460F5C File Offset: 0x0045F15C
		private void RefreshConflictExchangePanel(short skillTemplateId, CombatSkillDisplayData displayData)
		{
			this._cachedCurrentBreakPreset = null;
			this._cachedConflictSkill = null;
			CombatSkillItem config = CombatSkill.Instance[skillTemplateId];
			bool flag = this.conflictTitleName != null;
			if (flag)
			{
				this.conflictTitleName.text = config.Name.SetColor(Colors.Instance.GradeColors[(int)config.Grade]);
				this.gradeBack.SetSprite(string.Format("{0}{1}", "ui9_back_merhod_title_", config.Grade), false, null);
			}
			this._currentPresetIndex = (int)displayData.BreakPlateIndex;
			this.RefreshCurrentConflictSide(skillTemplateId, displayData);
			TaiwuDomainMethod.AsyncCall.GetCombatSkillBreakPreset(this, skillTemplateId, delegate(int offset, RawDataPool pool)
			{
				bool flag2 = this._skillTemplateId != skillTemplateId;
				if (!flag2)
				{
					this._cachedCurrentBreakPreset = new CombatSkillBreakPreset();
					Serializer.Deserialize(pool, offset, ref this._cachedCurrentBreakPreset);
					this.RefreshCurrentConflictSide(skillTemplateId, displayData);
				}
			});
			ExtraDomainMethod.AsyncCall.GetConflictCombatSkill(this, skillTemplateId, delegate(int offset, RawDataPool pool)
			{
				bool flag2 = this._skillTemplateId != skillTemplateId;
				if (!flag2)
				{
					this._cachedConflictSkill = new ConflictCombatSkill();
					Serializer.Deserialize(pool, offset, ref this._cachedConflictSkill);
					ConflictCombatSkill cachedConflictSkill = this._cachedConflictSkill;
					short? num = (cachedConflictSkill != null) ? new short?(cachedConflictSkill.TemplateId) : null;
					int? num2 = (num != null) ? new int?((int)num.GetValueOrDefault()) : null;
					int num3 = -1;
					bool flag3 = num2.GetValueOrDefault() <= num3 & num2 != null;
					if (!flag3)
					{
						ViewCharacterMenuPractice <>4__this = this;
						CombatSkillBreakPreset breakPreset = this._cachedConflictSkill.BreakPreset;
						<>4__this._pastPresetIndex = ((breakPreset != null) ? breakPreset.CurrentIndex : 0);
						this.RefreshPastConflictSide(skillTemplateId);
					}
				}
			});
		}

		// Token: 0x06009622 RID: 38434 RVA: 0x00461064 File Offset: 0x0045F264
		private void RefreshCurrentConflictSide(short skillTemplateId, CombatSkillDisplayData displayData)
		{
			bool flag = this.currentSide == null;
			if (!flag)
			{
				CombatSkillPracticeDisplayData currentSkillData = this._currentSkillData;
				GameData.Domains.Taiwu.SkillBreakPlate livePlate = (currentSkillData != null) ? currentSkillData.SkillBreakPlate : null;
				CombatSkillBreakPreset preset = this._cachedCurrentBreakPreset;
				this.currentSide.UpdatePresetIndex(this._currentPresetIndex);
				GameData.Domains.Taiwu.SkillBreakPlate plate = ViewCharacterMenuPractice.GetPlateAtPresetIndex(preset, livePlate, this._currentPresetIndex);
				bool showEmpty = ViewCharacterMenuPractice.IsEmptyPresetSlot(preset, livePlate, this._currentPresetIndex, plate);
				this.currentSide.RefreshContent(skillTemplateId, plate, showEmpty, this._data.LifeSkillAttainments, this, () => this._skillTemplateId == skillTemplateId);
			}
		}

		// Token: 0x06009623 RID: 38435 RVA: 0x00461114 File Offset: 0x0045F314
		private void RefreshPastConflictSide(short skillTemplateId)
		{
			bool flag = this.pastSide == null || this._cachedConflictSkill == null;
			if (!flag)
			{
				this.pastSide.UpdatePresetIndex(this._pastPresetIndex);
				GameData.Domains.Taiwu.SkillBreakPlate livePlate = this._cachedConflictSkill.BreakPlate;
				CombatSkillBreakPreset preset = this._cachedConflictSkill.BreakPreset;
				GameData.Domains.Taiwu.SkillBreakPlate plate = ViewCharacterMenuPractice.GetPlateAtPresetIndex(preset, livePlate, this._pastPresetIndex);
				bool showEmpty = ViewCharacterMenuPractice.IsEmptyPresetSlot(preset, livePlate, this._pastPresetIndex, plate);
				this.pastSide.RefreshContent(skillTemplateId, plate, showEmpty, this._data.LifeSkillAttainments, this, () => this._skillTemplateId == skillTemplateId);
			}
		}

		// Token: 0x06009624 RID: 38436 RVA: 0x004611D0 File Offset: 0x0045F3D0
		private static GameData.Domains.Taiwu.SkillBreakPlate GetPlateAtPresetIndex(CombatSkillBreakPreset preset, GameData.Domains.Taiwu.SkillBreakPlate livePlate, int index)
		{
			bool flag = preset == null;
			GameData.Domains.Taiwu.SkillBreakPlate result;
			if (flag)
			{
				result = ((index == 0) ? livePlate : null);
			}
			else
			{
				bool flag2 = index == preset.CurrentIndex;
				if (flag2)
				{
					result = livePlate;
				}
				else
				{
					List<CombatSkillBreakSnapshot> presets = preset.Presets;
					GameData.Domains.Taiwu.SkillBreakPlate skillBreakPlate;
					if (presets == null)
					{
						skillBreakPlate = null;
					}
					else
					{
						CombatSkillBreakSnapshot orDefault = presets.GetOrDefault(index);
						skillBreakPlate = ((orDefault != null) ? orDefault.BreakPlate : null);
					}
					result = skillBreakPlate;
				}
			}
			return result;
		}

		// Token: 0x06009625 RID: 38437 RVA: 0x00461224 File Offset: 0x0045F424
		private static bool IsEmptyPresetSlot(CombatSkillBreakPreset preset, GameData.Domains.Taiwu.SkillBreakPlate livePlate, int index, GameData.Domains.Taiwu.SkillBreakPlate resolvedPlate)
		{
			bool flag = resolvedPlate != null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = preset == null;
				if (flag2)
				{
					result = (index != 0 || livePlate == null);
				}
				else
				{
					bool flag3 = index == preset.CurrentIndex;
					if (flag3)
					{
						result = (livePlate == null);
					}
					else
					{
						List<CombatSkillBreakSnapshot> presets = preset.Presets;
						CombatSkillBreakSnapshot snapshot = (presets != null) ? presets.GetOrDefault(index) : null;
						result = (snapshot == null || snapshot.BreakPlate == null);
					}
				}
			}
			return result;
		}

		// Token: 0x06009626 RID: 38438 RVA: 0x00461290 File Offset: 0x0045F490
		private void OnCurrentPresetChange(int togNew, int togOld)
		{
			this._currentPresetIndex = togNew;
			CombatSkillDisplayData skillData;
			bool flag = this._skillData.TryGetValue(this._skillTemplateId, out skillData);
			if (flag)
			{
				this.RefreshCurrentConflictSide(this._skillTemplateId, skillData);
			}
		}

		// Token: 0x06009627 RID: 38439 RVA: 0x004612CA File Offset: 0x0045F4CA
		private void OnPastPresetChange(int togNew, int togOld)
		{
			this._pastPresetIndex = togNew;
			this.RefreshPastConflictSide(this._skillTemplateId);
		}

		// Token: 0x06009628 RID: 38440 RVA: 0x004612E4 File Offset: 0x0045F4E4
		private void OnClickSelectPracticeDialogCmd(bool keepDreamBackResult)
		{
			DialogCmd cmd = new DialogCmd
			{
				Title = LocalStringManager.Get(LanguageKey.LK_Select_Conflict_Practice_Title),
				Content = LocalStringManager.Get(LanguageKey.LK_Select_Conflict_Practice_Desc),
				Yes = delegate()
				{
					ExtraDomainMethod.Call.ApplyConflictCombatSkillResult(this._skillTemplateId, keepDreamBackResult);
					this.OnConflictCombatSkillResolved(this._skillTemplateId, keepDreamBackResult);
				}
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
			UIManager.Instance.ShowUI(UIElement.Dialog, true);
		}

		// Token: 0x040072EF RID: 29423
		private const string DirectionIndent = "     ";

		// Token: 0x040072F0 RID: 29424
		private const string DirectDescEnabled = "8dc3c3";

		// Token: 0x040072F1 RID: 29425
		private const string ReverseDescEnabled = "ffc0c0";

		// Token: 0x040072F2 RID: 29426
		private const string DescDisabled = "818181";

		// Token: 0x040072F3 RID: 29427
		private string[] _levelColor = new string[]
		{
			"dcdee0",
			"5d92c3",
			"885390",
			"d43f38"
		};

		// Token: 0x040072F4 RID: 29428
		public const int WidthSmall = 864;

		// Token: 0x040072F5 RID: 29429
		private const int WidthBig = 1530;

		// Token: 0x040072F6 RID: 29430
		private const int WidthMid = 1214;

		// Token: 0x040072F7 RID: 29431
		[SerializeField]
		private GameObject contentPanel;

		// Token: 0x040072F8 RID: 29432
		[SerializeField]
		private GameObject skillPanel;

		// Token: 0x040072F9 RID: 29433
		[SerializeField]
		private GameObject skillDesc;

		// Token: 0x040072FA RID: 29434
		[SerializeField]
		private GameObject notDesc;

		// Token: 0x040072FB RID: 29435
		[SerializeField]
		private GameObject notSelected;

		// Token: 0x040072FC RID: 29436
		[SerializeField]
		private GameObject invisiblePage;

		// Token: 0x040072FD RID: 29437
		[SerializeField]
		private CombatSkillSelect skillSelect;

		// Token: 0x040072FE RID: 29438
		[SerializeField]
		private CombatSkillPanel combatSkillPanel;

		// Token: 0x040072FF RID: 29439
		[SerializeField]
		private GameObject directBack;

		// Token: 0x04007300 RID: 29440
		[SerializeField]
		private GameObject reverseBack;

		// Token: 0x04007301 RID: 29441
		[SerializeField]
		private GameObject breakInfo;

		// Token: 0x04007302 RID: 29442
		[SerializeField]
		private PropertyItem breakDifficulty;

		// Token: 0x04007303 RID: 29443
		[SerializeField]
		private PropertyItem breakStep;

		// Token: 0x04007304 RID: 29444
		[SerializeField]
		private PropertyItem breakProb;

		// Token: 0x04007305 RID: 29445
		[SerializeField]
		private PropertyItem breakPower;

		// Token: 0x04007306 RID: 29446
		[SerializeField]
		private PropertyItem breakQualification;

		// Token: 0x04007307 RID: 29447
		[SerializeField]
		private TooltipInvoker breakDifficultyTips;

		// Token: 0x04007308 RID: 29448
		[SerializeField]
		private TooltipInvoker breakStepTips;

		// Token: 0x04007309 RID: 29449
		[SerializeField]
		private TooltipInvoker breakProbTips;

		// Token: 0x0400730A RID: 29450
		[SerializeField]
		private TooltipInvoker breakPowerTips;

		// Token: 0x0400730B RID: 29451
		[SerializeField]
		private TooltipInvoker breakQualificationTips;

		// Token: 0x0400730C RID: 29452
		[SerializeField]
		private CImage breakQualificationIcon;

		// Token: 0x0400730D RID: 29453
		[SerializeField]
		private GameObject breakButtons;

		// Token: 0x0400730E RID: 29454
		[SerializeField]
		private GameObject btnReview;

		// Token: 0x0400730F RID: 29455
		[SerializeField]
		private GameObject btnReBreak;

		// Token: 0x04007310 RID: 29456
		[SerializeField]
		private GameObject btnBreak;

		// Token: 0x04007311 RID: 29457
		[SerializeField]
		private GameObject btnContinue;

		// Token: 0x04007312 RID: 29458
		[SerializeField]
		private GameObject reBreakCd;

		// Token: 0x04007313 RID: 29459
		[SerializeField]
		private TextMeshProUGUI reBreakCdText;

		// Token: 0x04007314 RID: 29460
		[SerializeField]
		private CButton btnFold;

		// Token: 0x04007315 RID: 29461
		[SerializeField]
		private RectTransform foldArrow;

		// Token: 0x04007316 RID: 29462
		[SerializeField]
		private TextMeshProUGUI skillTitle;

		// Token: 0x04007317 RID: 29463
		[SerializeField]
		private TextMeshProUGUI direct;

		// Token: 0x04007318 RID: 29464
		[SerializeField]
		private TextMeshProUGUI reverse;

		// Token: 0x04007319 RID: 29465
		[SerializeField]
		private CImage directIcon;

		// Token: 0x0400731A RID: 29466
		[SerializeField]
		private CImage reverseIcon;

		// Token: 0x0400731B RID: 29467
		[SerializeField]
		private Transform pageEffectParent;

		// Token: 0x0400731C RID: 29468
		[SerializeField]
		private GameObject textRow;

		// Token: 0x0400731D RID: 29469
		[SerializeField]
		private GameObject doublePropertyRow;

		// Token: 0x0400731E RID: 29470
		[SerializeField]
		private GameObject lineObj;

		// Token: 0x0400731F RID: 29471
		[SerializeField]
		private GameObject breakEffect;

		// Token: 0x04007320 RID: 29472
		[SerializeField]
		private TextMeshProUGUI breakCurrentPower;

		// Token: 0x04007321 RID: 29473
		[SerializeField]
		private TextMeshProUGUI breakMaxPower;

		// Token: 0x04007322 RID: 29474
		[SerializeField]
		private GameObject bonusEffect;

		// Token: 0x04007323 RID: 29475
		[SerializeField]
		private Transform bonusEffectParent;

		// Token: 0x04007324 RID: 29476
		[SerializeField]
		private GameObject bonusNameTemplate;

		// Token: 0x04007325 RID: 29477
		[SerializeField]
		private GameObject bonusEffectTemplate;

		// Token: 0x04007326 RID: 29478
		[SerializeField]
		private CharacterMenuLocalLoadingAnim localLoadingAnim;

		// Token: 0x04007327 RID: 29479
		private CharacterDisplayDataForPractice _data;

		// Token: 0x04007328 RID: 29480
		private readonly Dictionary<short, CombatSkillDisplayData> _skillData = new Dictionary<short, CombatSkillDisplayData>();

		// Token: 0x04007329 RID: 29481
		private CombatSkillPracticeDisplayData _currentSkillData;

		// Token: 0x0400732A RID: 29482
		private short _skillTemplateId = -1;

		// Token: 0x0400732B RID: 29483
		private bool _needCheckFold;

		// Token: 0x0400732C RID: 29484
		private string _qualificationName;

		// Token: 0x0400732D RID: 29485
		private string _qualificationValue;

		// Token: 0x0400732E RID: 29486
		private string _qualificationIcon;

		// Token: 0x0400732F RID: 29487
		private sbyte[] _shouldExpandPanel = new sbyte[2];

		// Token: 0x04007330 RID: 29488
		private bool _keepOutline;

		// Token: 0x04007331 RID: 29489
		private ushort _beforeReBreakSelectedPage;

		// Token: 0x04007332 RID: 29490
		private bool _needAutoSelect;

		// Token: 0x04007333 RID: 29491
		private PoolItem _textRowPool;

		// Token: 0x04007334 RID: 29492
		private PoolItem _doublePropertyRowPool;

		// Token: 0x04007335 RID: 29493
		private PoolItem _bonusNamePool;

		// Token: 0x04007336 RID: 29494
		private PoolItem _bonusEffectPool;

		// Token: 0x04007337 RID: 29495
		private List<GameObject> _textRows = new List<GameObject>();

		// Token: 0x04007338 RID: 29496
		private List<GameObject> _doublePropertyRows = new List<GameObject>();

		// Token: 0x04007339 RID: 29497
		private List<GameObject> _bonusNames = new List<GameObject>();

		// Token: 0x0400733A RID: 29498
		private List<GameObject> _bonusEffects = new List<GameObject>();

		// Token: 0x0400733B RID: 29499
		[Header("冲突功法处理")]
		[SerializeField]
		private GameObject conflictExchangePanel;

		// Token: 0x0400733C RID: 29500
		[SerializeField]
		private TextMeshProUGUI conflictTitleName;

		// Token: 0x0400733D RID: 29501
		[SerializeField]
		private CImage gradeBack;

		// Token: 0x0400733E RID: 29502
		[SerializeField]
		private ConflickSkillDisplayItem currentSide;

		// Token: 0x0400733F RID: 29503
		[SerializeField]
		private ConflickSkillDisplayItem pastSide;

		// Token: 0x04007340 RID: 29504
		private bool _conflictPanelInited;

		// Token: 0x04007341 RID: 29505
		private ConflictCombatSkill _cachedConflictSkill;

		// Token: 0x04007342 RID: 29506
		private CombatSkillBreakPreset _cachedCurrentBreakPreset;

		// Token: 0x04007343 RID: 29507
		private int _currentPresetIndex;

		// Token: 0x04007344 RID: 29508
		private int _pastPresetIndex;
	}
}
