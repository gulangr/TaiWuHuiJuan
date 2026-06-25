using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using CharacterDataMonitor;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Domains.CombatSkill;
using GameData.Domains.Global;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B9F RID: 2975
	public class ViewCharacterMenuAttainment : UI_CharacterMenuSubPageBase
	{
		// Token: 0x0600933A RID: 37690 RVA: 0x004492F0 File Offset: 0x004474F0
		public override bool CheckState(ECharacterSubToggleBase curSubTogglePage, ECharacterSubPage curSubPage)
		{
			return curSubTogglePage == ECharacterSubToggleBase.AttainmentBase;
		}

		// Token: 0x17000FE7 RID: 4071
		// (get) Token: 0x0600933B RID: 37691 RVA: 0x00449306 File Offset: 0x00447506
		public override LanguageKey TitleKey
		{
			get
			{
				return LanguageKey.LK_Attainment;
			}
		}

		// Token: 0x17000FE8 RID: 4072
		// (get) Token: 0x0600933C RID: 37692 RVA: 0x0044930D File Offset: 0x0044750D
		private bool IsTaiwuTeamButNotBeast
		{
			get
			{
				return base.CharacterMenu.IsTaiwuTeam && !base.CharacterMenu.IsTaiwuBeastTeammate(base.CharacterMenu.CurCharacterId);
			}
		}

		// Token: 0x0600933D RID: 37693 RVA: 0x00449338 File Offset: 0x00447538
		public override void OnInit(ArgumentBox argsBox)
		{
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
			{
				this.SetAttainmentLoadingState(true);
				this.GetAllDisplayData();
				bool flag = base.CharacterMenu.CurCharacterId >= 0;
				if (flag)
				{
					this.UpdateAvatarMonitor();
				}
			}));
		}

		// Token: 0x0600933E RID: 37694 RVA: 0x00449364 File Offset: 0x00447564
		private void SetAttainmentLoadingState(bool isLoading)
		{
			bool isNonEvolutionaryType = this.IsNonEvolutionaryType();
			bool canShow = base.CharacterMenu.IsTaiwuGearMate(base.CharacterMenu.CurCharacterId) || !isNonEvolutionaryType;
			this.InvisiblePage.SetActive(!canShow);
			bool flag = !canShow;
			if (flag)
			{
				this.localLoadingAnim.SetLoadingState(false);
			}
			else
			{
				this.localLoadingAnim.SetLoadingState(isLoading);
			}
		}

		// Token: 0x0600933F RID: 37695 RVA: 0x004493D0 File Offset: 0x004475D0
		private void Awake()
		{
			this.Init();
		}

		// Token: 0x06009340 RID: 37696 RVA: 0x004493DC File Offset: 0x004475DC
		public void Init()
		{
			bool flag = this.inited;
			if (!flag)
			{
				this.InitOverallPage();
				this.inited = true;
			}
		}

		// Token: 0x06009341 RID: 37697 RVA: 0x00449404 File Offset: 0x00447604
		private void OnEnable()
		{
			ViewCharacterMenuAttainment.EOverallPageState previouseState = this._overallPageState;
			this._overallPageState = ViewCharacterMenuAttainment.EOverallPageState.None;
			bool flag = previouseState == ViewCharacterMenuAttainment.EOverallPageState.LifeSkillEquip;
			if (flag)
			{
				this.OpenLifeSkillSubPage();
			}
			else
			{
				bool flag2 = previouseState == ViewCharacterMenuAttainment.EOverallPageState.Overall;
				if (flag2)
				{
					this.OpenOverallPage();
				}
				else
				{
					bool flag3 = previouseState == ViewCharacterMenuAttainment.EOverallPageState.CombatSkillEquip;
					if (flag3)
					{
						this.OpenCombatSkillSubPage();
					}
				}
			}
			this.RefreshByCachedData();
			this.RefreshTitle();
			this.lifeSkillAttainmentPanel.RefreshTitles();
			this.combatSkillAttainmentPanel.RefreshTitles();
			GlobalDomainMethod.Call.InvokeGuidingTrigger(87);
		}

		// Token: 0x06009342 RID: 37698 RVA: 0x00449488 File Offset: 0x00447688
		private void RefreshTitle()
		{
			for (int i = 0; i < this.combatSkillBgRefers.Length; i++)
			{
				CharacterAttainmentOverviewItem item = this.combatSkillBgRefers[i];
				int index = i;
				bool flag = index == 14 || index == 15;
				if (flag)
				{
					item.SetTitle((index == 14) ? LocalStringManager.Get(LanguageKey.LK_Shenli) : LocalStringManager.Get(LanguageKey.LK_Guishu));
				}
				else
				{
					item.SetTitle(Config.CombatSkillType.Instance[index].Name);
				}
			}
			for (int j = 0; j < this.lifeSkillBgRefers.Length; j++)
			{
				CharacterAttainmentOverviewItem item2 = this.lifeSkillBgRefers[j];
				int index2 = j;
				item2.SetTitle(Config.LifeSkillType.Instance[index2].Name);
			}
		}

		// Token: 0x06009343 RID: 37699 RVA: 0x0044954F File Offset: 0x0044774F
		private void GetAllDisplayData()
		{
			CharacterDomainMethod.AsyncCall.GetCharacterMenuAttainmentDisplayData(null, base.CharacterMenu.CurCharacterId, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._cachedData);
				this.RefreshByCachedData();
				this.SetAttainmentLoadingState(false);
				this.Element.ShowAfterRefresh();
			});
		}

		// Token: 0x06009344 RID: 37700 RVA: 0x00449570 File Offset: 0x00447770
		private unsafe void RefreshByCachedData()
		{
			bool flag = this._cachedData == null;
			if (!flag)
			{
				this.combatSkillAttainmentPanel.SetExpandable(base.CharacterMenu.CurrentCharacterIsTaiwu);
				this.combatSkillAttainmentPanel.SetIsTaiwuCharacter(base.CharacterMenu.CurrentCharacterIsTaiwu);
				this.lifeSkillAttainmentPanel.SetExpandable(base.CharacterMenu.CurrentCharacterIsTaiwu);
				this.lifeSkillAttainmentPanel.SetIsTaiwuCharacter(base.CharacterMenu.CurrentCharacterIsTaiwu);
				string combatPanelTitle;
				this.UpdateQualificationGrowth(this._cachedData.ActualAge, this._cachedData.CombatSkillGrowthType, true, out combatPanelTitle);
				string LifePanelTitle;
				this.UpdateQualificationGrowth(this._cachedData.ActualAge, this._cachedData.LifeSkillGrowthType, false, out LifePanelTitle);
				this.OnAttainmentPanelsUpdate();
				bool flag2 = this._overallPageState == ViewCharacterMenuAttainment.EOverallPageState.Overall;
				if (flag2)
				{
					this.RefreshOverallPage();
				}
				else
				{
					bool flag3 = this._overallPageState == ViewCharacterMenuAttainment.EOverallPageState.CombatSkillEquip;
					if (flag3)
					{
						this.combatSkillAttainmentPanel.SetSecondaryTitle(combatPanelTitle);
						for (sbyte type = 0; type < 14; type += 1)
						{
							this.combatSkillAttainmentPanel.SetQualificationAttainmentItem(type, (int)(*this._cachedData.CombatSkillQualifications[(int)type]), this.GetCombatSkillAttainmentLevel(type), this.GetConvertValueText(this._cachedData.CombatSkillAttainments[(int)type].ToString()));
						}
						this.UpdateAttainmentCombatSkillList();
						this.UpdateCombatAttainmentPanel(true);
					}
					else
					{
						bool flag4 = this._overallPageState == ViewCharacterMenuAttainment.EOverallPageState.LifeSkillEquip;
						if (flag4)
						{
							this.lifeSkillAttainmentPanel.SetSecondaryTitle(LifePanelTitle);
							for (sbyte type2 = 0; type2 < 16; type2 += 1)
							{
								this.lifeSkillAttainmentPanel.SetQualificationAttainmentItem(type2, (int)(*this._cachedData.LifeSkillQualifications[(int)type2]), this.GetLifeSkillAttainmentLevel(type2), this.GetConvertValueText(this._cachedData.LifeSkillAttainments[(int)type2].ToString()));
							}
							this.UpdateAttainmentLifeSkillList();
							this.UpdateLifeAttainmentPanel(true);
						}
					}
				}
			}
		}

		// Token: 0x06009345 RID: 37701 RVA: 0x00449770 File Offset: 0x00447970
		private unsafe void RefreshOverallPage()
		{
			for (sbyte type = 0; type < 14; type += 1)
			{
				this.combatSkillBgRefers[(int)type].SetQualification((*this._cachedData.CombatSkillQualifications[(int)type]).SetValueColor());
				this.combatSkillBgRefers[(int)type].SetAttainment(this.GetConvertValueText(this._cachedData.CombatSkillAttainments[(int)type].ToString()));
			}
			this.combatSkillBgRefers[14].SetQualification(this._cachedData.DivinePower.SetValueColor());
			this.combatSkillBgRefers[14].SetAttainment(this.GetConvertValueText(this._cachedData.DivinePower.ToString()));
			this.combatSkillBgRefers[15].SetQualification(this._cachedData.GhostTechnique.SetValueColor());
			this.combatSkillBgRefers[15].SetAttainment(this.GetConvertValueText(this._cachedData.GhostTechnique.ToString()));
			for (sbyte type2 = 0; type2 < 16; type2 += 1)
			{
				this.lifeSkillBgRefers[(int)type2].SetQualification((*this._cachedData.LifeSkillQualifications[(int)type2]).SetValueColor());
				this.lifeSkillBgRefers[(int)type2].SetAttainment(this.GetConvertValueText(this._cachedData.LifeSkillAttainments[(int)type2].ToString()));
			}
		}

		// Token: 0x06009346 RID: 37702 RVA: 0x004498CC File Offset: 0x00447ACC
		private void CheckEscClose()
		{
			bool flag = this._overallPageState == ViewCharacterMenuAttainment.EOverallPageState.LifeSkillEquip || this._overallPageState == ViewCharacterMenuAttainment.EOverallPageState.CombatSkillEquip;
			if (flag)
			{
				this.OpenOverallPage();
			}
			else
			{
				base.CharacterMenu.QuickHide();
			}
		}

		// Token: 0x06009347 RID: 37703 RVA: 0x00449908 File Offset: 0x00447B08
		private new void OnDisable()
		{
			this.ClearAvatarMonitor();
		}

		// Token: 0x06009348 RID: 37704 RVA: 0x00449912 File Offset: 0x00447B12
		public override void OnSwitchToSubpage(int subPageIndex)
		{
			this.CurTabIndex = subPageIndex;
			this.SelectPage();
			this._isInCombatSkillPage = true;
			this.SetOverallCombat2LifeSkillPage();
			this.OpenOverallPage();
		}

		// Token: 0x06009349 RID: 37705 RVA: 0x00449938 File Offset: 0x00447B38
		private void SelectPage()
		{
			bool isNonEvolutionaryType = this.IsNonEvolutionaryType();
			bool canShow = base.CharacterMenu.IsTaiwuGearMate(base.CharacterMenu.CurCharacterId) || !isNonEvolutionaryType;
			this.InvisiblePage.SetActive(!canShow);
			this.mainPage.SetActive(canShow);
			bool flag = this.CurTabIndex != 0;
			if (flag)
			{
				this._overallPageState = ViewCharacterMenuAttainment.EOverallPageState.None;
			}
		}

		// Token: 0x0600934A RID: 37706 RVA: 0x004499A0 File Offset: 0x00447BA0
		public override void OnSubpageVisible()
		{
			bool flag = this.CurTabIndex != 1;
			if (flag)
			{
				this.Element.ShowAfterRefresh();
			}
		}

		// Token: 0x0600934B RID: 37707 RVA: 0x004499CA File Offset: 0x00447BCA
		public override void OnSubpageInVisible()
		{
			base.OnSubpageInVisible();
		}

		// Token: 0x0600934C RID: 37708 RVA: 0x004499D4 File Offset: 0x00447BD4
		public override void OnCurrentCharacterChange(int prevCharacterId)
		{
			bool flag = base.CharacterMenu.CurCharacterId < 0;
			if (!flag)
			{
				this.SetAttainmentLoadingState(true);
				this.GetAllDisplayData();
				bool flag2 = base.CharacterMenu.CurCharacterId < 0;
				if (!flag2)
				{
					this.UpdateAvatarMonitor();
				}
			}
		}

		// Token: 0x0600934D RID: 37709 RVA: 0x00449A20 File Offset: 0x00447C20
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = btnName == "BtnInteract";
			if (!flag)
			{
				bool flag2 = btnName == "CenterBtn";
				if (flag2)
				{
					base.CharacterMenu.SetCurPageSubpage(1);
				}
				else
				{
					bool flag3 = btnName == "AutoEquipButton";
					if (flag3)
					{
						bool currentCharacterIsTaiwuTeammate = base.CharacterMenu.CurrentCharacterIsTaiwuTeammate;
						if (currentCharacterIsTaiwuTeammate)
						{
							CharacterDomainMethod.Call.AutoSetCombatSkillAttainmentPanels(base.CharacterMenu.CurCharacterId);
							this.GetAllDisplayData();
						}
					}
					else
					{
						bool flag4 = btnName == "BtnSwitchAttainment";
						if (flag4)
						{
							this._isInCombatSkillPage = !this._isInCombatSkillPage;
							this.SetOverallCombat2LifeSkillPage();
						}
					}
				}
			}
		}

		// Token: 0x0600934E RID: 37710 RVA: 0x00449AD4 File Offset: 0x00447CD4
		private void SetOverallCombat2LifeSkillPage()
		{
			this.overallLifeSkillPage.SetActive(!this._isInCombatSkillPage);
			this.overallCombatSkillPage.SetActive(this._isInCombatSkillPage);
			this.txtSwitchAttainment.SetText(this._isInCombatSkillPage ? LanguageKey.LK_CharacterMenuAttainment_Switch_To_LifeSkill.Tr() : LanguageKey.LK_CharacterMenuAttainment_Switch_To_CombatSkill.Tr(), true);
		}

		// Token: 0x0600934F RID: 37711 RVA: 0x00449B34 File Offset: 0x00447D34
		private void UpdateAvatarMonitor()
		{
			this.RemoveListenerForAvatarMonitor();
			this._avatarInfoMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<AvatarInfoMonitor>(base.CharacterMenu.CurCharacterId, false);
			this._avatarInfoMonitor.AddOnCreatingTypeChangeEventListener(new Action(this.SelectPage));
			bool init = this._avatarInfoMonitor.Init;
			if (init)
			{
				this._avatarInfoMonitor.OnDataInit();
			}
		}

		// Token: 0x06009350 RID: 37712 RVA: 0x00449B98 File Offset: 0x00447D98
		private void ClearAvatarMonitor()
		{
			this.RemoveListenerForAvatarMonitor();
			this._avatarInfoMonitor = null;
		}

		// Token: 0x06009351 RID: 37713 RVA: 0x00449BA9 File Offset: 0x00447DA9
		private void RemoveListenerForAvatarMonitor()
		{
			AvatarInfoMonitor avatarInfoMonitor = this._avatarInfoMonitor;
			if (avatarInfoMonitor != null)
			{
				avatarInfoMonitor.RemoveOnCreatingTypeChangeEventListener(new Action(this.SelectPage));
			}
		}

		// Token: 0x06009352 RID: 37714 RVA: 0x00449BCC File Offset: 0x00447DCC
		private void OnAttainmentPanelsUpdate()
		{
			for (sbyte type = 0; type < 14; type += 1)
			{
				this.UpdateSectInfos(type);
			}
		}

		// Token: 0x06009353 RID: 37715 RVA: 0x00449BF6 File Offset: 0x00447DF6
		private void OnLifeSkillTypeTogChange(int togNew)
		{
			base.CharacterMenu.CurrentSelectedLifeSkillType = (sbyte)togNew;
			this._curLifeSkillType = (sbyte)togNew;
			this.UpdateLifeAttainmentPanel(true);
			this.UpdateAttainmentLifeSkillList();
		}

		// Token: 0x06009354 RID: 37716 RVA: 0x00449C1D File Offset: 0x00447E1D
		private void OnCombatSkillTypeTogChange(int togNew)
		{
			base.CharacterMenu.CurrentSelectedCombatSkillType = (sbyte)togNew;
			this.UpdateCombatAttainmentPanel(true);
			this.UpdateAttainmentCombatSkillList();
		}

		// Token: 0x06009355 RID: 37717 RVA: 0x00449C3C File Offset: 0x00447E3C
		[return: TupleElementNames(new string[]
		{
			"desc",
			"tips"
		})]
		private ValueTuple<string, string> GetCombatSkillAttainmentPanelString(sbyte type)
		{
			string skiiTypeName = Config.CombatSkillType.Instance[type].Name.SetColor("brightblue");
			ValueTuple<int, int> info = this.UpdateSectInfos(type);
			int index = GameData.Domains.Character.CombatSkillHelper.CalcAttainments_GetPrimarySectIndex(this._sectInfos);
			bool hasSectBonus = info.Item1 > 0 && index >= 0 && this._sectInfos[index].OrgTemplateId > 0;
			bool flag = hasSectBonus;
			ValueTuple<string, string> strTuple;
			if (flag)
			{
				string sectName = LocalStringManager.Get(string.Format("LK_Sect_Name_Short_{0}", this._sectInfos[index].OrgTemplateId));
				strTuple.Item1 = LocalStringManager.GetFormat(LanguageKey.LK_Combat_Skill_Attainment_Panel_Sect_Bonus_Title, sectName);
				strTuple.Item2 = LocalStringManager.GetFormat(LanguageKey.LK_Combat_Skill_Attainment_Panel_Sect_Bonus_Tips, new object[]
				{
					info.Item1,
					skiiTypeName,
					100 + info.Item2,
					info.Item2
				}).ColorReplace();
			}
			else
			{
				strTuple.Item1 = LocalStringManager.Get(LanguageKey.LK_Combat_Skill_Attainment_Panel_No_Sect_Bonus_Title).SetColor("grey");
				strTuple.Item2 = LocalStringManager.GetFormat(LanguageKey.LK_Combat_Skill_Attainment_Panel_No_Sect_Bonus_Tips, skiiTypeName);
			}
			return strTuple;
		}

		// Token: 0x06009356 RID: 37718 RVA: 0x00449D6C File Offset: 0x00447F6C
		[return: TupleElementNames(new string[]
		{
			"desc",
			"tips"
		})]
		private ValueTuple<string, string> GetLifeSkillAttainmentLevelString(sbyte type)
		{
			int readedBookCount = 0;
			int totalAddAttainment = 0;
			short[] skillIdList = Config.LifeSkillType.Instance[type].SkillList;
			for (int bookGrade = 0; bookGrade < skillIdList.Length; bookGrade++)
			{
				short skillId = skillIdList[bookGrade];
				bool flag = this._cachedData.LearnedLifeSkills == null;
				if (!flag)
				{
					GameData.Domains.Character.LifeSkillItem skillData = this._cachedData.LearnedLifeSkills.SingleOrDefault((GameData.Domains.Character.LifeSkillItem item) => item.SkillTemplateId == skillId);
					bool flag2 = skillData.IsAnyPagesRead();
					if (flag2)
					{
						readedBookCount++;
					}
					int addAttainment = (int)(GlobalConfig.Instance.AddAttainmentPerGrade[bookGrade] / 5) * skillData.GetReadPagesCount();
					totalAddAttainment += addAttainment;
				}
			}
			int attainmentLevel = readedBookCount / 3;
			string attainmentLevelDesc = LocalStringManager.Get(string.Format("LK_Life_Skill_Attainment_Level_{0}", attainmentLevel));
			bool flag3 = attainmentLevel < 2;
			if (flag3)
			{
				attainmentLevelDesc = attainmentLevelDesc.SetColor("grey");
			}
			string skillName = Config.LifeSkillType.Instance[type].Name.SetColor("brightblue");
			string hasBook = LocalStringManager.GetFormat(LanguageKey.LK_Life_Skill_Attainment_Level_Bonus_Tips, new object[]
			{
				readedBookCount,
				skillName,
				100 + totalAddAttainment,
				totalAddAttainment
			});
			string noBook = LocalStringManager.GetFormat(LanguageKey.LK_Life_Skill_Attainment_Level_0_Tips, skillName);
			string attainmentLevelTips = (readedBookCount > 0) ? hasBook : noBook;
			return new ValueTuple<string, string>(attainmentLevelDesc, attainmentLevelTips);
		}

		// Token: 0x06009357 RID: 37719 RVA: 0x00449ED0 File Offset: 0x004480D0
		[return: TupleElementNames(new string[]
		{
			"skillCount",
			"totalAddAttainment"
		})]
		private ValueTuple<int, int> UpdateSectInfos(sbyte type)
		{
			int skillCount = 0;
			int totalAddAttainment = 0;
			this._sectInfos.Clear();
			for (sbyte grade = 0; grade < 9; grade += 1)
			{
				short skillTemplateId = CombatSkillAttainmentPanelsHelper.Get(this._cachedData.CombatSkillAttainmentPanels, type, grade);
				bool flag = skillTemplateId >= 0;
				if (flag)
				{
					skillCount++;
					totalAddAttainment += (int)GlobalConfig.Instance.AddAttainmentPerGrade[(int)grade];
					GameData.Domains.Character.CombatSkillHelper.CalcAttainments_RecordSectInfo(this._sectInfos, CombatSkill.Instance[skillTemplateId].SectId, grade);
				}
			}
			return new ValueTuple<int, int>(skillCount, totalAddAttainment);
		}

		// Token: 0x06009358 RID: 37720 RVA: 0x00449F64 File Offset: 0x00448164
		private string GetConvertValueText(string text)
		{
			bool flag = this._avatarInfoMonitor == null;
			if (flag)
			{
				this.UpdateAvatarMonitor();
			}
			byte creatingType = this._cachedData.CreationType;
			bool flag2 = base.CharacterMenu.IsTaiwuGearMate(this._avatarInfoMonitor.CharacterId);
			string result;
			if (flag2)
			{
				result = text;
			}
			else
			{
				result = CommonUtils.ConvertValueByCreatingType(creatingType, text);
			}
			return result;
		}

		// Token: 0x06009359 RID: 37721 RVA: 0x00449FBC File Offset: 0x004481BC
		private bool IsNonEvolutionaryType()
		{
			bool flag = this._avatarInfoMonitor == null;
			if (flag)
			{
				this.UpdateAvatarMonitor();
			}
			return CreatingType.IsNonEvolutionaryType(this._avatarInfoMonitor.CreatingType);
		}

		// Token: 0x0600935A RID: 37722 RVA: 0x00449FF4 File Offset: 0x004481F4
		public override void QuickHide()
		{
			bool flag = this._overallPageState == ViewCharacterMenuAttainment.EOverallPageState.LifeSkillEquip || this._overallPageState == ViewCharacterMenuAttainment.EOverallPageState.CombatSkillEquip;
			if (flag)
			{
				this.OpenOverallPage();
			}
			else
			{
				base.QuickHide();
			}
		}

		// Token: 0x0600935B RID: 37723 RVA: 0x0044A02C File Offset: 0x0044822C
		private CharacterAttainmentOverallItem GetBgRefers(bool combatSkill, int index)
		{
			if (combatSkill)
			{
				switch (index)
				{
				case 0:
				case 1:
				case 2:
					return this.overallSkillBgRefers[2];
				case 3:
				case 4:
				case 5:
					return this.overallSkillBgRefers[3];
				case 6:
				case 7:
				case 8:
				case 9:
					return this.overallSkillBgRefers[0];
				case 10:
				case 11:
				case 12:
				case 13:
					return this.overallSkillBgRefers[1];
				case 14:
				case 15:
					return this.overallSkillBgRefers[4];
				}
			}
			bool flag = !combatSkill;
			if (flag)
			{
				switch (index)
				{
				case 0:
				case 1:
				case 2:
				case 3:
					return this.overallSkillBgRefers[6];
				case 4:
				case 8:
				case 9:
				case 15:
					return this.overallSkillBgRefers[7];
				case 5:
				case 12:
				case 13:
				case 14:
					return this.overallSkillBgRefers[5];
				case 6:
				case 7:
				case 10:
				case 11:
					return this.overallSkillBgRefers[8];
				}
			}
			return null;
		}

		// Token: 0x0600935C RID: 37724 RVA: 0x0044A164 File Offset: 0x00448364
		private CharacterAttainmentOverallItem GetBgRefers(int index)
		{
			bool combatSkill = index < 16;
			bool flag = !combatSkill;
			if (flag)
			{
				index -= 16;
			}
			return this.GetBgRefers(combatSkill, index);
		}

		// Token: 0x0600935D RID: 37725 RVA: 0x0044A194 File Offset: 0x00448394
		private List<int> GetOverViewChild(int parentIndex, out bool isCombatSkill)
		{
			List<int> result = new List<int>();
			isCombatSkill = false;
			switch (parentIndex)
			{
			case 0:
				result.Add(7);
				result.Add(8);
				result.Add(9);
				result.Add(6);
				isCombatSkill = true;
				break;
			case 1:
				result.Add(10);
				result.Add(11);
				result.Add(12);
				result.Add(13);
				isCombatSkill = true;
				break;
			case 2:
				result.Add(0);
				result.Add(1);
				result.Add(2);
				isCombatSkill = true;
				break;
			case 3:
				result.Add(3);
				result.Add(4);
				result.Add(5);
				isCombatSkill = true;
				break;
			case 4:
				result.Add(14);
				result.Add(15);
				isCombatSkill = true;
				break;
			case 5:
				result.Add(5);
				result.Add(12);
				result.Add(13);
				result.Add(14);
				break;
			case 6:
				result.Add(1);
				result.Add(0);
				result.Add(2);
				result.Add(3);
				break;
			case 7:
				result.Add(4);
				result.Add(8);
				result.Add(9);
				result.Add(15);
				break;
			case 8:
				result.Add(6);
				result.Add(7);
				result.Add(10);
				result.Add(11);
				break;
			}
			return result;
		}

		// Token: 0x0600935E RID: 37726 RVA: 0x0044A324 File Offset: 0x00448524
		private void InitOverallPage()
		{
			for (int i = 0; i < this.overallSkillBgRefers.Length; i++)
			{
				int index = i;
				CharacterAttainmentOverallItem characterAttainmentOverallItem = this.overallSkillBgRefers[i];
				characterAttainmentOverallItem.OnEnter = (Action)Delegate.Combine(characterAttainmentOverallItem.OnEnter, new Action(delegate()
				{
					this.SetOverviewChildHighlight(index, true);
				}));
				CharacterAttainmentOverallItem characterAttainmentOverallItem2 = this.overallSkillBgRefers[i];
				characterAttainmentOverallItem2.OnExit = (Action)Delegate.Combine(characterAttainmentOverallItem2.OnExit, new Action(delegate()
				{
					this.SetOverviewChildHighlight(index, false);
				}));
			}
			for (int j = 0; j < this.combatSkillBgRefers.Length; j++)
			{
				CharacterAttainmentOverviewItem item = this.combatSkillBgRefers[j];
				int index = j;
				item.OverallSkillItem.ClearAndAddListener(delegate
				{
					this.OnClickOverallSkill(index);
				});
				this.SetupOverallCombatSkillItem(j, item);
			}
			for (int k = 0; k < this.lifeSkillBgRefers.Length; k++)
			{
				CharacterAttainmentOverviewItem item2 = this.lifeSkillBgRefers[k];
				int index = k;
				item2.OverallSkillItem.ClearAndAddListener(delegate
				{
					this.OnClickOverallSkill(index + 16);
				});
				this.SetupOverallLifeSkillItem(k, item2);
			}
			this.InitAttainmentPanelActiveSkillAndCandidates();
			this.lifeSkillAttainmentPanel.InitLifeSkill(new Action<int>(this.OnLifeSkillTypeTogChange), delegate(int newIndex)
			{
				this.OnActiveLifeSkillTogChange();
			}, null);
			this.combatSkillAttainmentPanel.InitCombatSkill(new Action<int>(this.OnCombatSkillTypeTogChange), delegate(int newIndex)
			{
				this.OnActiveCombatSkillTogChange();
			}, delegate(int _attainmentPlanIndex)
			{
				short currentPlanId = this._cachedData.CombatSkillAttainmentPlans[this.combatSkillAttainmentPanel.currSkillType];
				bool flag = base.CharacterMenu.CurrentCharacterIsTaiwu && base.CharacterMenu.CanOperate;
				if (flag)
				{
					bool flag2 = (int)currentPlanId != _attainmentPlanIndex;
					if (flag2)
					{
						TaiwuDomainMethod.Call.SelectCombatSkillAttainmentPanelPlan((sbyte)this.combatSkillAttainmentPanel.currSkillType, (sbyte)_attainmentPlanIndex);
						this._cachedData.CombatSkillAttainmentPlans[this.combatSkillAttainmentPanel.currSkillType] = (short)((sbyte)currentPlanId);
						this.GetAllDisplayData();
					}
				}
			});
			this.overallPointerTrigger.EnterEvent.AddListener(delegate()
			{
				for (int l = 0; l < this.overallSkillBgRefers.Length; l++)
				{
					this.overallSkillBgRefers[l].GetComponent<PolygonChecker>().enabled = true;
				}
			});
			this.overallPointerTrigger.ExitEvent.AddListener(delegate()
			{
				for (int l = 0; l < this.overallSkillBgRefers.Length; l++)
				{
					this.overallSkillBgRefers[l].GetComponent<PolygonChecker>().enabled = false;
				}
			});
		}

		// Token: 0x0600935F RID: 37727 RVA: 0x0044A510 File Offset: 0x00448710
		private void SetOverviewChildHighlight(int index, bool isHighlight)
		{
			bool isCombatSkill;
			List<int> targetIndex = this.GetOverViewChild(index, out isCombatSkill);
			bool flag = isCombatSkill;
			if (flag)
			{
				foreach (int item in targetIndex)
				{
					this.combatSkillBgRefers[item].SetParentHover(isHighlight);
				}
			}
			else
			{
				foreach (int item2 in targetIndex)
				{
					this.lifeSkillBgRefers[item2].SetParentHover(isHighlight);
				}
			}
		}

		// Token: 0x06009360 RID: 37728 RVA: 0x0044A5D0 File Offset: 0x004487D0
		private void SetupOverallCombatSkillItem(int index, CharacterAttainmentOverviewItem item)
		{
			CButton btn = item.OverallSkillItem;
			bool flag = index == 14 || index == 15;
			if (flag)
			{
				item.SetTitle((index == 14) ? LocalStringManager.Get(LanguageKey.LK_Shenli) : LocalStringManager.Get(LanguageKey.LK_Guishu));
				item.SetQualification("0");
				item.SetAttainment("0");
				btn.interactable = false;
			}
			else
			{
				btn.interactable = true;
				item.SetTitle(Config.CombatSkillType.Instance[index].Name);
			}
			string iconName = string.Format("{0}{1}", "ui9_icon_attainments_combatskill_3_", index);
			string iconNameHighlight = string.Format("{0}{1}", "ui9_icon_attainments_combatskill_1_", index);
			string iconNameParentHighlight = string.Format("{0}{1}", "ui9_icon_attainments_combatskill_0_", index);
			item.SetIcon(iconName, iconNameHighlight, iconNameParentHighlight);
			item.SetType(true);
		}

		// Token: 0x06009361 RID: 37729 RVA: 0x0044A6B4 File Offset: 0x004488B4
		private void SetupOverallLifeSkillItem(int index, CharacterAttainmentOverviewItem item)
		{
			item.OverallSkillItem.interactable = true;
			item.SetTitle(Config.LifeSkillType.Instance[index].Name);
			string iconName = string.Format("{0}{1}", "ui9_icon_attainments_lifeskill_3_", index);
			string iconNameHighlight = string.Format("{0}{1}", "ui9_icon_attainments_lifeskill_1_", index);
			string iconNameParentHighlight = string.Format("{0}{1}", "ui9_icon_attainments_lifeskill_0_", index);
			item.SetIcon(iconName, iconNameHighlight, iconNameParentHighlight);
			item.SetType(false);
		}

		// Token: 0x06009362 RID: 37730 RVA: 0x0044A73C File Offset: 0x0044893C
		private void OnClickOverallSkill(int index)
		{
			bool flag = !base.CharacterMenu.CanOperate;
			if (!flag)
			{
				bool combatSkill = index < 16;
				bool flag2 = !combatSkill;
				if (flag2)
				{
					index -= 16;
				}
				bool flag3 = combatSkill;
				if (flag3)
				{
					base.CharacterMenu.CurrentSelectedCombatSkillType = (sbyte)index;
					this.OpenCombatSkillSubPage();
				}
				else
				{
					base.CharacterMenu.CurrentSelectedLifeSkillType = (sbyte)index;
					this.OpenLifeSkillSubPage();
				}
			}
		}

		// Token: 0x06009363 RID: 37731 RVA: 0x0044A7A8 File Offset: 0x004489A8
		private void OpenCombatSkillSubPage()
		{
			this._overallPageState = ViewCharacterMenuAttainment.EOverallPageState.CombatSkillEquip;
			this.SetPageCloseEvent(true);
			this.SetupCombatSkillSubPage();
			this.skillEquipPanel.gameObject.SetActive(true);
			this.overallPanel.SetActive(false);
			this.mainTitleText.text = LocalStringManager.Get(LanguageKey.LK_Character_ActiveCombatSkills);
			this.combatSkillAttainmentPanel.SetAttainment((int)base.CharacterMenu.CurrentSelectedCombatSkillType);
			this.RefreshByCachedData();
		}

		// Token: 0x06009364 RID: 37732 RVA: 0x0044A820 File Offset: 0x00448A20
		private void OpenLifeSkillSubPage()
		{
			this._overallPageState = ViewCharacterMenuAttainment.EOverallPageState.LifeSkillEquip;
			this.SetPageCloseEvent(true);
			this.SetupLifeSkillSubPage();
			this.skillEquipPanel.gameObject.SetActive(true);
			this.overallPanel.SetActive(false);
			this.mainTitleText.text = LocalStringManager.Get(LanguageKey.LK_Character_ActiveLifeSkills);
			this.lifeSkillAttainmentPanel.SetAttainment((int)base.CharacterMenu.CurrentSelectedLifeSkillType);
			this.RefreshByCachedData();
		}

		// Token: 0x06009365 RID: 37733 RVA: 0x0044A898 File Offset: 0x00448A98
		private void SetPageCloseEvent(bool flag)
		{
			if (flag)
			{
				base.CharacterMenu.OnTryClosePage = delegate()
				{
					this.OpenOverallPage();
					base.CharacterMenu.OnTryClosePage = null;
				};
			}
			else
			{
				base.CharacterMenu.OnTryClosePage = null;
			}
		}

		// Token: 0x06009366 RID: 37734 RVA: 0x0044A8D4 File Offset: 0x00448AD4
		private void OpenOverallPage()
		{
			this._overallPageState = ViewCharacterMenuAttainment.EOverallPageState.Overall;
			this.SetPageCloseEvent(false);
			this.overallPanel.SetActive(true);
			this.skillEquipPanel.gameObject.SetActive(false);
			this.mainTitleText.text = LocalStringManager.Get(LanguageKey.LK_Character_OverallAttainment);
		}

		// Token: 0x06009367 RID: 37735 RVA: 0x0044A928 File Offset: 0x00448B28
		private void SetupCombatSkillSubPage()
		{
			this.candidatesViewCombatSkill.gameObject.SetActive(true);
			this.candidatesViewLifeSkill.gameObject.SetActive(false);
			this.combatSkillAttainmentPanel.gameObject.SetActive(true);
			this.lifeSkillAttainmentPanel.gameObject.SetActive(false);
			GlobalDomainMethod.Call.InvokeGuidingTrigger(75);
		}

		// Token: 0x06009368 RID: 37736 RVA: 0x0044A988 File Offset: 0x00448B88
		private void SetupLifeSkillSubPage()
		{
			this.candidatesViewCombatSkill.gameObject.SetActive(false);
			this.candidatesViewLifeSkill.gameObject.SetActive(true);
			this.combatSkillAttainmentPanel.gameObject.SetActive(false);
			this.lifeSkillAttainmentPanel.gameObject.SetActive(true);
			GlobalDomainMethod.Call.InvokeGuidingTrigger(75);
		}

		// Token: 0x06009369 RID: 37737 RVA: 0x0044A9E8 File Offset: 0x00448BE8
		private void InitAttainmentPanelActiveSkillAndCandidates()
		{
			this.candidatesViewCombatSkill.Init();
			this.candidatesViewCombatSkill.SetCombatSkillList(null, true, true, "charmenu_combatskill_attainment", new Action<CombatSkillDisplayDataCharacterMenuListItem, CharacterMenuCombatSkillItem>(this.OnRenderAttainmentCombatSkill), false, false, null, true);
		}

		// Token: 0x0600936A RID: 37738 RVA: 0x0044AA28 File Offset: 0x00448C28
		private void UpdateAttainmentCombatSkillList()
		{
			this._attainmentSkillList.Clear();
			bool flag = this._cachedData.LearnedCombatSkillDatasSimple != null;
			if (flag)
			{
				this._attainmentSkillList.AddRange(this._cachedData.LearnedCombatSkillDatasSimple);
			}
			this._attainmentSkillList.RemoveAll(delegate(CombatSkillDisplayDataCharacterMenuListItem data)
			{
				CombatSkillItem configData = CombatSkill.Instance[data.TemplateId];
				int currGradeTog = (this.combatSkillAttainmentPanel.currSkillGradeIndex >= 0) ? this.combatSkillAttainmentPanel.currSkillGradeIndex : 0;
				int grade = base.CharacterMenu.CanOperate ? currGradeTog : 0;
				return configData.Type != base.CharacterMenu.CurrentSelectedCombatSkillType || (int)configData.Grade != grade || !CombatSkillStateHelper.CanEquipOnAttainmentPanel(data.ReadingState, data.Revoked);
			});
			this.candidatesViewCombatSkill.SetCombatSkillList(this._attainmentSkillList, false, true, null, null, false, false, null, true);
			this.ScrollCandidatesToActiveCombatSkill();
		}

		// Token: 0x0600936B RID: 37739 RVA: 0x0044AAA4 File Offset: 0x00448CA4
		private void ScrollCandidatesToActiveCombatSkill()
		{
			bool flag = this._cachedData == null;
			if (!flag)
			{
				sbyte type = base.CharacterMenu.CurrentSelectedCombatSkillType;
				sbyte grade = (sbyte)((this.combatSkillAttainmentPanel.currSkillGradeIndex >= 0) ? this.combatSkillAttainmentPanel.currSkillGradeIndex : 0);
				short activeSkillId = CombatSkillAttainmentPanelsHelper.Get(this._cachedData.CombatSkillAttainmentPanels, type, grade);
				bool flag2 = activeSkillId < 0;
				if (!flag2)
				{
					List<CombatSkillDisplayDataCharacterMenuListItem> filteredData = this.candidatesViewCombatSkill.filteredData;
					bool flag3 = filteredData == null;
					if (!flag3)
					{
						int index = filteredData.FindIndex((CombatSkillDisplayDataCharacterMenuListItem data) => data.TemplateId == activeSkillId);
						bool flag4 = index >= 0;
						if (flag4)
						{
							this.candidatesViewCombatSkill.ScrollTo(index);
						}
					}
				}
			}
		}

		// Token: 0x0600936C RID: 37740 RVA: 0x0044AB67 File Offset: 0x00448D67
		private void OnActiveCombatSkillTogChange()
		{
			this.combatSkillAttainmentPanel.SetExpand(true, false);
			this.UpdateAttainmentCombatSkillList();
		}

		// Token: 0x0600936D RID: 37741 RVA: 0x0044AB7F File Offset: 0x00448D7F
		private void OnActiveLifeSkillTogChange()
		{
			this.lifeSkillAttainmentPanel.SetExpand(true, false);
			this.UpdateAttainmentLifeSkillList();
		}

		// Token: 0x0600936E RID: 37742 RVA: 0x0044AB98 File Offset: 0x00448D98
		private void UpdateAttainmentLifeSkillList()
		{
			int targetGrade = (this.lifeSkillAttainmentPanel.currSkillGradeIndex >= 0) ? this.lifeSkillAttainmentPanel.currSkillGradeIndex : 0;
			short skillId = Config.LifeSkillType.Instance[base.CharacterMenu.CurrentSelectedLifeSkillType].SkillList[targetGrade];
			List<GameData.Domains.Character.LifeSkillItem> lifeSkillItems = (this._cachedData.LearnedLifeSkills == null) ? null : (from item in this._cachedData.LearnedLifeSkills
			where item.SkillTemplateId == skillId
			select item).ToList<GameData.Domains.Character.LifeSkillItem>();
			List<CharacterMenuLifeSkillItemData> skillData = new List<CharacterMenuLifeSkillItemData>();
			bool flag = lifeSkillItems != null;
			if (flag)
			{
				foreach (GameData.Domains.Character.LifeSkillItem item2 in lifeSkillItems)
				{
					sbyte[] readingProgress = new sbyte[5];
					this.CalcReadingProgress(item2, item2.SkillTemplateId, readingProgress);
					skillData.Add(new CharacterMenuLifeSkillItemData
					{
						LifeSkillItem = item2,
						ReadingProgress = readingProgress
					});
				}
			}
			this.candidatesViewLifeSkill.SetLifeSkillList(skillData, false, true, null, new Action<CharacterMenuLifeSkillItemData, CharacterMenuLifeSkillItem>(this.OnRenderLifeSkill), false, false, null, true);
			this.ScrollCandidatesToActiveLifeSkill();
		}

		// Token: 0x0600936F RID: 37743 RVA: 0x0044ACCC File Offset: 0x00448ECC
		private void ScrollCandidatesToActiveLifeSkill()
		{
			sbyte type = base.CharacterMenu.CurrentSelectedLifeSkillType;
			sbyte grade = (sbyte)((this.lifeSkillAttainmentPanel.currSkillGradeIndex >= 0) ? this.lifeSkillAttainmentPanel.currSkillGradeIndex : 0);
			short activeSkillId = this.GetActiveLifeSkillTemplateId(type, grade);
			bool flag = activeSkillId < 0;
			if (!flag)
			{
				short[] skillList = Config.LifeSkillType.Instance[type].SkillList;
				bool flag2 = (int)grade >= skillList.Length || skillList[(int)grade] != activeSkillId;
				if (!flag2)
				{
					this.candidatesViewLifeSkill.ScrollTo(0);
				}
			}
		}

		// Token: 0x06009370 RID: 37744 RVA: 0x0044AD54 File Offset: 0x00448F54
		private void OnRenderLifeSkill(CharacterMenuLifeSkillItemData skillData, CharacterMenuLifeSkillItem skillItem)
		{
			sbyte type = base.CharacterMenu.CurrentSelectedLifeSkillType;
			sbyte grade = (sbyte)((this.lifeSkillAttainmentPanel.currSkillGradeIndex >= 0) ? this.lifeSkillAttainmentPanel.currSkillGradeIndex : 0);
			short activeSkillId = this.GetActiveLifeSkillTemplateId(type, grade);
			skillItem.GetComponent<Refers>().CGet<GameObject>("selected").SetActive(skillData.LifeSkillItem.SkillTemplateId == activeSkillId);
		}

		// Token: 0x06009371 RID: 37745 RVA: 0x0044ADBC File Offset: 0x00448FBC
		private void OnRenderAttainmentCombatSkill(CombatSkillDisplayDataCharacterMenuListItem skillData, CharacterMenuCombatSkillItem skillView)
		{
			CButton skillBtn = skillView.GetComponent<CButton>();
			CombatSkillItem configData = CombatSkill.Instance[skillData.TemplateId];
			skillBtn.ClearAndAddListener(delegate
			{
				this.OnClickAttainmentSkill(skillData.TemplateId, skillView, true);
			});
			sbyte type = base.CharacterMenu.CurrentSelectedCombatSkillType;
			sbyte grade = (sbyte)((this.combatSkillAttainmentPanel.currSkillGradeIndex >= 0) ? this.combatSkillAttainmentPanel.currSkillGradeIndex : 0);
			short activeSkillId = (this._cachedData != null) ? CombatSkillAttainmentPanelsHelper.Get(this._cachedData.CombatSkillAttainmentPanels, type, grade) : -1;
			skillView.SetSelectedIcon(skillData.TemplateId == activeSkillId);
		}

		// Token: 0x06009372 RID: 37746 RVA: 0x0044AE80 File Offset: 0x00449080
		private short GetActiveLifeSkillTemplateId(sbyte type, sbyte grade)
		{
			short[] skillList = Config.LifeSkillType.Instance[type].SkillList;
			bool flag = (int)grade >= skillList.Length;
			short result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				short skillId = skillList[(int)grade];
				CharacterMenuAttainmentDisplayData cachedData = this._cachedData;
				bool flag2 = ((cachedData != null) ? cachedData.LearnedLifeSkills : null) == null;
				if (flag2)
				{
					result = -1;
				}
				else
				{
					for (int i = 0; i < this._cachedData.LearnedLifeSkills.Count; i++)
					{
						bool flag3 = this._cachedData.LearnedLifeSkills[i].SkillTemplateId == skillId;
						if (flag3)
						{
							return skillId;
						}
					}
					result = -1;
				}
			}
			return result;
		}

		// Token: 0x06009373 RID: 37747 RVA: 0x0044AF24 File Offset: 0x00449124
		private void OnClickAttainmentSkill(short skillId, CharacterMenuCombatSkillItem skillView, bool isOn)
		{
			bool flag = !base.CharacterMenu.CanOperate || !isOn;
			if (!flag)
			{
				sbyte type = base.CharacterMenu.CurrentSelectedCombatSkillType;
				sbyte grade = (sbyte)((this.combatSkillAttainmentPanel.currSkillGradeIndex >= 0) ? this.combatSkillAttainmentPanel.currSkillGradeIndex : 0);
				short origSkillId = CombatSkillAttainmentPanelsHelper.Get(this._cachedData.CombatSkillAttainmentPanels, type, grade);
				bool flag2 = origSkillId == skillId || CombatSkill.Instance[skillId].Type != type;
				if (!flag2)
				{
					List<CombatSkillDisplayDataCharacterMenuListItem> learnedCombatSkillDatasSimple = this._cachedData.LearnedCombatSkillDatasSimple;
					CombatSkillDisplayDataCharacterMenuListItem skillData = (learnedCombatSkillDatasSimple != null) ? learnedCombatSkillDatasSimple.FirstOrDefault((CombatSkillDisplayDataCharacterMenuListItem d) => d.TemplateId == skillId) : null;
					bool flag3 = skillData == null || !CombatSkillStateHelper.CanEquipOnAttainmentPanel(skillData.ReadingState, skillData.Revoked);
					if (!flag3)
					{
						bool flag4 = origSkillId >= 0;
						if (flag4)
						{
						}
						short[] copy = new short[this._cachedData.CombatSkillAttainmentPanels.Length];
						Array.Copy(this._cachedData.CombatSkillAttainmentPanels, copy, this._cachedData.CombatSkillAttainmentPanels.Length);
						CombatSkillAttainmentPanelsHelper.Set(copy, type, grade, skillId);
						GameDataBridge.AddDataModification<short[]>(4, 0, (ulong)((long)base.CharacterMenu.CurCharacterId), 61U, copy);
						this.GetAllDisplayData();
					}
				}
			}
		}

		// Token: 0x06009374 RID: 37748 RVA: 0x0044B08C File Offset: 0x0044928C
		private unsafe void UpdateLifeAttainmentPanel(bool autoSelectBookTog = true)
		{
			sbyte type = base.CharacterMenu.CurrentSelectedLifeSkillType;
			ValueTuple<string, string> attainmentStr = this.GetLifeSkillAttainmentLevelString(type);
			int firstLearnedGrade = -1;
			int attainmentLevel = this.GetLifeSkillAttainmentLevel(type);
			this.lifeSkillAttainmentPanel.SetLifeSkillInfoArea(type, *this._cachedData.LifeSkillQualifications[(int)type], this.GetConvertValueText(this._cachedData.LifeSkillAttainments[(int)type].ToString()));
			List<CharacterMenuAttainmentActiveSkillData> activeSkillDataList = new List<CharacterMenuAttainmentActiveSkillData>();
			for (sbyte grade = 0; grade < 9; grade += 1)
			{
				short skillId = (Config.LifeSkillType.Instance[type].SkillList.Count<short>() > (int)grade) ? Config.LifeSkillType.Instance[type].SkillList[(int)grade] : -1;
				int addAttainmentValue = 0;
				bool flag2 = skillId >= 0;
				if (flag2)
				{
					GameData.Domains.Character.LifeSkillItem skillData = default(GameData.Domains.Character.LifeSkillItem);
					bool flag = false;
					bool flag3 = this._cachedData.LearnedLifeSkills != null;
					if (flag3)
					{
						for (int i = 0; i < this._cachedData.LearnedLifeSkills.Count; i++)
						{
							bool flag4 = this._cachedData.LearnedLifeSkills[i].SkillTemplateId == skillId;
							if (flag4)
							{
								skillData = this._cachedData.LearnedLifeSkills[i];
								flag = true;
								break;
							}
						}
					}
					bool flag5 = !flag;
					if (flag5)
					{
						skillId = -1;
					}
					else
					{
						addAttainmentValue = (int)(GlobalConfig.Instance.AddAttainmentPerGrade[(int)grade] / 5) * skillData.GetReadPagesCount();
						sbyte[] readingProgress = new sbyte[5];
					}
				}
				bool flag6 = skillId >= 0;
				if (flag6)
				{
					Config.LifeSkillItem config = LifeSkill.Instance[skillId];
					SkillBookItem skillBookConfig = SkillBook.Instance[config.SkillBookId];
					sbyte[] readingProgress2 = new sbyte[5];
					this.CalcReadingProgress(skillId, readingProgress2);
					activeSkillDataList.Add(new CharacterMenuAttainmentActiveSkillData
					{
						SkillTemplateId = skillId,
						IsCombatSkill = false,
						AddValue = addAttainmentValue,
						Locked = (base.CharacterMenu.CanOperate && this.IsTaiwuTeamButNotBeast),
						Grade = (int)skillBookConfig.Grade,
						Icon = skillBookConfig.Icon,
						Color = Color.white,
						SkillName = skillBookConfig.Name,
						CharId = base.CharacterMenu.CurCharacterId,
						ReadingProgress = readingProgress2
					});
				}
				else
				{
					activeSkillDataList.Add(null);
				}
				bool flag7 = firstLearnedGrade < 0 && skillId >= 0;
				if (flag7)
				{
					firstLearnedGrade = (int)grade;
				}
			}
			this.lifeSkillAttainmentPanel.SetActiveSkills(activeSkillDataList, true, (int)type, this.BuildLifeSkillSlotUnlockedArray(type));
			this.lifeSkillAttainmentPanel.infoArea.SetAttainmentLevelDesc(attainmentStr.Item1.ColorReplace());
			this.lifeSkillAttainmentPanel.SetHintText(attainmentStr.Item2.ColorReplace());
		}

		// Token: 0x06009375 RID: 37749 RVA: 0x0044B360 File Offset: 0x00449560
		private unsafe void UpdateCombatAttainmentPanel(bool autoSelectBookTog = true)
		{
			sbyte type = base.CharacterMenu.CurrentSelectedCombatSkillType;
			short[] attainmentPanels = this._cachedData.CombatSkillAttainmentPanels;
			ValueTuple<string, string> attainmentStr = this.GetCombatSkillAttainmentPanelString(type);
			int attainmentLevel = this.GetCombatSkillAttainmentLevel(type);
			if (!true)
			{
			}
			int num;
			if (type != 14)
			{
				if (type != 15)
				{
					num = (int)(*this._cachedData.CombatSkillQualifications[(int)type]);
				}
				else
				{
					num = this._cachedData.GhostTechnique;
				}
			}
			else
			{
				num = this._cachedData.DivinePower;
			}
			if (!true)
			{
			}
			int qualification = num;
			if (!true)
			{
			}
			if (type != 14)
			{
				if (type != 15)
				{
					num = (int)(*this._cachedData.CombatSkillAttainments[(int)type]);
				}
				else
				{
					num = this._cachedData.GhostTechnique;
				}
			}
			else
			{
				num = this._cachedData.DivinePower;
			}
			if (!true)
			{
			}
			int attainment = num;
			this.combatSkillAttainmentPanel.SetCombatSkillInfoArea(type, qualification, this.GetConvertValueText(attainment.ToString()));
			List<CharacterMenuAttainmentActiveSkillData> activeSkillDataList = new List<CharacterMenuAttainmentActiveSkillData>();
			for (sbyte grade = 0; grade < 9; grade += 1)
			{
				short skillId = CombatSkillAttainmentPanelsHelper.Get(attainmentPanels, type, grade);
				bool flag = skillId >= 0;
				if (flag)
				{
					int addAttainmentValue = (int)GlobalConfig.Instance.AddAttainmentPerGrade[(int)grade];
					CombatSkillItem config = CombatSkill.Instance[skillId];
					activeSkillDataList.Add(new CharacterMenuAttainmentActiveSkillData
					{
						SkillTemplateId = skillId,
						IsCombatSkill = true,
						AddValue = addAttainmentValue,
						Locked = (base.CharacterMenu.CanOperate && this.IsTaiwuTeamButNotBeast),
						Grade = (int)config.Grade,
						Icon = config.Icon,
						Color = Colors.Instance.FiveElementsColors[(int)config.FiveElements],
						SkillName = config.Name,
						CharId = base.CharacterMenu.CurCharacterId
					});
				}
				else
				{
					activeSkillDataList.Add(null);
				}
			}
			this.combatSkillAttainmentPanel.SetActiveSkills(activeSkillDataList, true, (int)type, this.BuildCombatSkillSlotUnlockedArray(type));
			this.combatSkillAttainmentPanel.SetHintText(attainmentStr.Item2.ColorReplace());
			this.combatSkillAttainmentPanel.infoArea.SetAttainmentLevelDesc(attainmentStr.Item1.ColorReplace());
		}

		// Token: 0x06009376 RID: 37750 RVA: 0x0044B5A4 File Offset: 0x004497A4
		private void CalcReadingProgress(GameData.Domains.Character.LifeSkillItem skillData, short skillTemplateId, sbyte[] result)
		{
			bool currentCharacterIsTaiwu = base.CharacterMenu.CurrentCharacterIsTaiwu;
			if (currentCharacterIsTaiwu)
			{
				for (byte i = 0; i < 5; i += 1)
				{
					bool flag = this._taiwuLifeSkills.ContainsKey(skillTemplateId);
					if (flag)
					{
						result[(int)i] = this._taiwuLifeSkills[skillTemplateId].GetBookPageReadingProgress(i);
					}
					else
					{
						bool flag2 = this._taiwuNotLearnLifeSkills.ContainsKey(skillTemplateId);
						if (flag2)
						{
							result[(int)i] = this._taiwuNotLearnLifeSkills[skillTemplateId].GetBookPageReadingProgress(i);
						}
						else
						{
							result[(int)i] = (skillData.IsPageRead(i) ? 100 : 0);
						}
					}
				}
			}
			else
			{
				for (byte j = 0; j < 5; j += 1)
				{
					result[(int)j] = (skillData.IsPageRead(j) ? 100 : 0);
				}
			}
		}

		// Token: 0x06009377 RID: 37751 RVA: 0x0044B66C File Offset: 0x0044986C
		private void CalcReadingProgress(short skillTemplateId, sbyte[] result)
		{
			int index = -1;
			GameData.Domains.Character.LifeSkillItem skillData = default(GameData.Domains.Character.LifeSkillItem);
			bool flag = this._cachedData.LearnedLifeSkills != null;
			if (flag)
			{
				index = this._cachedData.LearnedLifeSkills.FindIndex((GameData.Domains.Character.LifeSkillItem skillItem) => skillItem.SkillTemplateId == skillTemplateId);
				bool flag2 = index >= 0;
				if (flag2)
				{
					skillData = this._cachedData.LearnedLifeSkills[index];
				}
			}
			bool currentCharacterIsTaiwu = base.CharacterMenu.CurrentCharacterIsTaiwu;
			if (currentCharacterIsTaiwu)
			{
				for (byte i = 0; i < 5; i += 1)
				{
					bool flag3 = this._cachedData.TaiwuLifeSkills.ContainsKey(skillTemplateId);
					if (flag3)
					{
						result[(int)i] = this._cachedData.TaiwuLifeSkills[skillTemplateId].GetBookPageReadingProgress(i);
					}
					else
					{
						bool flag4 = this._cachedData.TaiwuNotLearnLifeSkills.ContainsKey(skillTemplateId);
						if (flag4)
						{
							result[(int)i] = this._cachedData.TaiwuNotLearnLifeSkills[skillTemplateId].GetBookPageReadingProgress(i);
						}
						else
						{
							result[(int)i] = ((index >= 0 && skillData.IsPageRead(i)) ? 100 : 0);
						}
					}
				}
			}
			else
			{
				for (byte j = 0; j < 5; j += 1)
				{
					result[(int)j] = ((index >= 0 && skillData.IsPageRead(j)) ? 100 : 0);
				}
			}
		}

		// Token: 0x06009378 RID: 37752 RVA: 0x0044B7E4 File Offset: 0x004499E4
		private bool[] BuildLifeSkillSlotUnlockedArray(sbyte type)
		{
			bool[] result = new bool[9];
			for (sbyte grade = 0; grade < 9; grade += 1)
			{
				result[(int)grade] = this.IsLifeSkillAttainmentSlotUnlocked(type, (int)grade);
			}
			return result;
		}

		// Token: 0x06009379 RID: 37753 RVA: 0x0044B81C File Offset: 0x00449A1C
		private bool[] BuildCombatSkillSlotUnlockedArray(sbyte type)
		{
			bool[] result = new bool[9];
			for (sbyte grade = 0; grade < 9; grade += 1)
			{
				result[(int)grade] = this.IsCombatSkillAttainmentSlotUnlocked(type, (int)grade);
			}
			return result;
		}

		// Token: 0x0600937A RID: 37754 RVA: 0x0044B854 File Offset: 0x00449A54
		private bool IsLifeSkillAttainmentSlotUnlocked(sbyte type, int slotGrade)
		{
			bool flag = slotGrade < 0 || slotGrade >= 9;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				short[] skillIdList = Config.LifeSkillType.Instance[type].SkillList;
				bool flag2 = slotGrade >= skillIdList.Length;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = slotGrade / 3 + 1 > this.GetLifeSkillAttainmentLevel(type);
					if (flag3)
					{
						result = false;
					}
					else
					{
						int requiredBookIndex = (slotGrade <= 0) ? 0 : (slotGrade - 1);
						bool flag4 = requiredBookIndex < 0 || requiredBookIndex >= skillIdList.Length;
						if (flag4)
						{
							result = false;
						}
						else
						{
							short requiredSkillId = skillIdList[requiredBookIndex];
							result = (this._cachedData.LearnedLifeSkills != null && this._cachedData.LearnedLifeSkills.Any((GameData.Domains.Character.LifeSkillItem item) => item.SkillTemplateId == requiredSkillId && item.IsAnyPagesRead()));
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600937B RID: 37755 RVA: 0x0044B924 File Offset: 0x00449B24
		private bool IsCombatSkillAttainmentSlotUnlocked(sbyte type, int slotGrade)
		{
			bool flag = slotGrade < 0 || slotGrade >= 9;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = slotGrade / 3 + 1 > this.GetCombatSkillAttainmentLevel(type);
				if (flag2)
				{
					result = false;
				}
				else
				{
					int requiredGrade = slotGrade;
					bool flag3 = this._cachedData.LearnedCombatSkillDatasSimple == null;
					result = (!flag3 && this._cachedData.LearnedCombatSkillDatasSimple.Any(delegate(CombatSkillDisplayDataCharacterMenuListItem data)
					{
						CombatSkillItem config = CombatSkill.Instance[data.TemplateId];
						return config.Type == type && (int)config.Grade == requiredGrade && CombatSkillStateHelper.CanEquipOnAttainmentPanel(data.ReadingState, data.Revoked);
					}));
				}
			}
			return result;
		}

		// Token: 0x0600937C RID: 37756 RVA: 0x0044B9B0 File Offset: 0x00449BB0
		private int GetCombatSkillAttainmentLevel(sbyte type)
		{
			ViewCharacterMenuAttainment.<>c__DisplayClass105_0 CS$<>8__locals1 = new ViewCharacterMenuAttainment.<>c__DisplayClass105_0();
			CS$<>8__locals1.type = type;
			int readedBookCount = 0;
			int bookGrade;
			int bookGrade2;
			for (bookGrade = 0; bookGrade < 9; bookGrade = bookGrade2 + 1)
			{
				bool flag = this._cachedData.LearnedCombatSkills != null && this._cachedData.LearnedCombatSkills.Any((short item) => CombatSkill.Instance[item].Type == CS$<>8__locals1.type && (int)CombatSkill.Instance[item].Grade == bookGrade);
				if (flag)
				{
					readedBookCount++;
				}
				bookGrade2 = bookGrade;
			}
			int attainmentLevel = readedBookCount / 3;
			return attainmentLevel + 1;
		}

		// Token: 0x0600937D RID: 37757 RVA: 0x0044BA4C File Offset: 0x00449C4C
		private int GetLifeSkillAttainmentLevel(sbyte type)
		{
			int readedBookCount = 0;
			short[] skillIdList = Config.LifeSkillType.Instance[type].SkillList;
			for (int bookGrade = 0; bookGrade < skillIdList.Length; bookGrade++)
			{
				short skillId = skillIdList[bookGrade];
				bool flag = this._cachedData.LearnedLifeSkills != null && this._cachedData.LearnedLifeSkills.Any((GameData.Domains.Character.LifeSkillItem item) => item.SkillTemplateId == skillId && item.IsAnyPagesRead());
				if (flag)
				{
					readedBookCount++;
				}
			}
			int attainmentLevel = readedBookCount / 3;
			return attainmentLevel + 1;
		}

		// Token: 0x0600937E RID: 37758 RVA: 0x0044BAD8 File Offset: 0x00449CD8
		private void UpdateQualificationGrowth(short actualAge, sbyte growthType, bool combatSkill, out string secondaryTitleStr)
		{
			AgeEffectItem ageData = AgeEffect.Instance[Mathf.Min((int)actualAge, AgeEffect.Instance.Count - 1)];
			int addValue = 0;
			StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
			strBuilder.Clear();
			string growthTypeStr;
			switch (growthType)
			{
			case 0:
				growthTypeStr = LocalStringManager.Get("LK_Qualification_Growth_Average");
				addValue = (int)((ageData == null) ? 0 : ageData.SkillQualificationAverage);
				break;
			case 1:
				growthTypeStr = LocalStringManager.Get("LK_Qualification_Growth_Precocious");
				addValue = (int)((ageData == null) ? 0 : ageData.SkillQualificationPrecocious);
				break;
			case 2:
				growthTypeStr = LocalStringManager.Get("LK_Qualification_Growth_LateBlooming");
				addValue = (int)((ageData == null) ? 0 : ageData.SkillQualificationLateBlooming);
				break;
			default:
				growthTypeStr = "";
				break;
			}
			strBuilder.Append(growthTypeStr);
			bool flag = addValue > 0;
			string growthTypeValueStr;
			if (flag)
			{
				growthTypeValueStr = string.Format("+{0}", addValue).SetColor("brightblue");
			}
			else
			{
				bool flag2 = addValue == 0;
				if (flag2)
				{
					growthTypeValueStr = "+0".SetColor("lightgrey");
				}
				else
				{
					growthTypeValueStr = string.Format("{0}", addValue).SetColor("red");
				}
			}
			strBuilder.Append(growthTypeValueStr);
			secondaryTitleStr = strBuilder.ToString();
			strBuilder.Clear();
			strBuilder.Append(growthTypeStr);
			strBuilder.Append(growthTypeValueStr);
			if (combatSkill)
			{
				this.overallCombatSkillGrowth.text = strBuilder.ToString();
			}
			else
			{
				this.overallLifeSkillGrowth.text = strBuilder.ToString();
			}
			EasyPool.Free<StringBuilder>(strBuilder);
		}

		// Token: 0x0400717A RID: 29050
		[SerializeField]
		private GameObject mainPage;

		// Token: 0x0400717B RID: 29051
		[SerializeField]
		private CharacterMenuAttainmentPanel lifeSkillAttainmentPanel;

		// Token: 0x0400717C RID: 29052
		[SerializeField]
		private CharacterMenuAttainmentPanel combatSkillAttainmentPanel;

		// Token: 0x0400717D RID: 29053
		[SerializeField]
		private TextMeshProUGUI mainTitleText;

		// Token: 0x0400717E RID: 29054
		[SerializeField]
		private GameObject overallPanel;

		// Token: 0x0400717F RID: 29055
		[SerializeField]
		private GameObject skillEquipPanel;

		// Token: 0x04007180 RID: 29056
		[SerializeField]
		private TextMeshProUGUI overallCombatSkillGrowth;

		// Token: 0x04007181 RID: 29057
		[SerializeField]
		private TextMeshProUGUI overallLifeSkillGrowth;

		// Token: 0x04007182 RID: 29058
		[SerializeField]
		private GameObject InvisiblePage;

		// Token: 0x04007183 RID: 29059
		[SerializeField]
		private TextMeshProUGUI txtSwitchAttainment;

		// Token: 0x04007184 RID: 29060
		[SerializeField]
		private GameObject overallLifeSkillPage;

		// Token: 0x04007185 RID: 29061
		[SerializeField]
		private GameObject overallCombatSkillPage;

		// Token: 0x04007186 RID: 29062
		[SerializeField]
		private PointerTrigger overallPointerTrigger;

		// Token: 0x04007187 RID: 29063
		[SerializeField]
		private CharacterMenuLocalLoadingAnim localLoadingAnim;

		// Token: 0x04007188 RID: 29064
		private CharacterMenuAttainmentDisplayData _cachedData;

		// Token: 0x04007189 RID: 29065
		private readonly Dictionary<short, TaiwuLifeSkill> _taiwuLifeSkills = new Dictionary<short, TaiwuLifeSkill>();

		// Token: 0x0400718A RID: 29066
		private readonly Dictionary<short, TaiwuLifeSkill> _taiwuNotLearnLifeSkills = new Dictionary<short, TaiwuLifeSkill>();

		// Token: 0x0400718B RID: 29067
		private bool _taiwuLifeSkillsInited;

		// Token: 0x0400718C RID: 29068
		private bool _taiwuNotLearnLifeSkillsInited;

		// Token: 0x0400718D RID: 29069
		private const int Max_COMBAT_SKILL_AMOUNT = 16;

		// Token: 0x0400718E RID: 29070
		private const int ACTIVE_SKILL_SLOT = 9;

		// Token: 0x0400718F RID: 29071
		private readonly List<CombatSkillDisplayDataCharacterMenuListItem> _attainmentSkillList = new List<CombatSkillDisplayDataCharacterMenuListItem>();

		// Token: 0x04007190 RID: 29072
		private readonly List<GameData.Domains.Character.CombatSkillHelper.AttainmentSectInfo> _sectInfos = new List<GameData.Domains.Character.CombatSkillHelper.AttainmentSectInfo>(9);

		// Token: 0x04007191 RID: 29073
		private AvatarInfoMonitor _avatarInfoMonitor;

		// Token: 0x04007192 RID: 29074
		private bool inited = false;

		// Token: 0x04007193 RID: 29075
		private bool _isInCombatSkillPage;

		// Token: 0x04007194 RID: 29076
		private sbyte _curLifeSkillType;

		// Token: 0x04007195 RID: 29077
		public Transform overallSkillContent;

		// Token: 0x04007196 RID: 29078
		[Header("总览界面元素")]
		[SerializeField]
		private CharacterAttainmentOverallItem[] overallSkillBgRefers = new CharacterAttainmentOverallItem[9];

		// Token: 0x04007197 RID: 29079
		[SerializeField]
		private CharacterAttainmentOverviewItem[] combatSkillBgRefers = new CharacterAttainmentOverviewItem[16];

		// Token: 0x04007198 RID: 29080
		[SerializeField]
		private CharacterAttainmentOverviewItem[] lifeSkillBgRefers = new CharacterAttainmentOverviewItem[16];

		// Token: 0x04007199 RID: 29081
		[SerializeField]
		private CharacterMenuCombatSkillScrollView candidatesViewCombatSkill;

		// Token: 0x0400719A RID: 29082
		[SerializeField]
		private CharacterMenuLifeSkillScrollView candidatesViewLifeSkill;

		// Token: 0x0400719B RID: 29083
		private const string overallSkillBgHoverName = "Hover";

		// Token: 0x0400719C RID: 29084
		private string _secondaryTitleCombatCache;

		// Token: 0x0400719D RID: 29085
		private string _secondaryTitleLifeCache;

		// Token: 0x0400719E RID: 29086
		private ViewCharacterMenuAttainment.EOverallPageState _overallPageState = ViewCharacterMenuAttainment.EOverallPageState.Overall;

		// Token: 0x020021C4 RID: 8644
		private enum EOverallPageState
		{
			// Token: 0x0400D6F6 RID: 55030
			None,
			// Token: 0x0400D6F7 RID: 55031
			Overall,
			// Token: 0x0400D6F8 RID: 55032
			LifeSkillEquip,
			// Token: 0x0400D6F9 RID: 55033
			CombatSkillEquip
		}
	}
}
