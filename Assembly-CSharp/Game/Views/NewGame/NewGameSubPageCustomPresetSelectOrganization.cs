using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Global;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Views.NewGame
{
	// Token: 0x02000807 RID: 2055
	public class NewGameSubPageCustomPresetSelectOrganization : NewGameSubPageCustomPresetItemBase
	{
		// Token: 0x17000C1F RID: 3103
		// (get) Token: 0x060064C8 RID: 25800 RVA: 0x002E1478 File Offset: 0x002DF678
		public override DialogCmd StartGameCheck
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000C20 RID: 3104
		// (get) Token: 0x060064C9 RID: 25801 RVA: 0x002E147B File Offset: 0x002DF67B
		// (set) Token: 0x060064CA RID: 25802 RVA: 0x002E1483 File Offset: 0x002DF683
		public override bool StartGameChecked { get; set; }

		// Token: 0x17000C21 RID: 3105
		// (get) Token: 0x060064CB RID: 25803 RVA: 0x002E148C File Offset: 0x002DF68C
		public override int SpentPoints
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000C22 RID: 3106
		// (get) Token: 0x060064CC RID: 25804 RVA: 0x002E148F File Offset: 0x002DF68F
		public override int RemainingPoints
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060064CD RID: 25805 RVA: 0x002E1492 File Offset: 0x002DF692
		private void Awake()
		{
			this.changeBirthAreaToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnChangeBirthAreaToggleChanged));
		}

		// Token: 0x060064CE RID: 25806 RVA: 0x002E14B2 File Offset: 0x002DF6B2
		public override void RefreshUI()
		{
			this.InitToggleGroup();
		}

		// Token: 0x060064CF RID: 25807 RVA: 0x002E14BC File Offset: 0x002DF6BC
		public override void ApplyToPreset(CustomProtagonistPresetItem presetItem)
		{
		}

		// Token: 0x060064D0 RID: 25808 RVA: 0x002E14BF File Offset: 0x002DF6BF
		public override void ApplyFromPreset(CustomProtagonistPresetItem presetItem)
		{
		}

		// Token: 0x060064D1 RID: 25809 RVA: 0x002E14C4 File Offset: 0x002DF6C4
		public void SetToCustom()
		{
			bool flag = this.sectPlanToggleGroup == null;
			if (!flag)
			{
				int oldIndex = this.sectPlanToggleGroup.GetActiveIndex();
				this.sectPlanToggleGroup.SetWithoutNotify(0);
				this.RefreshCheckmark2(0, oldIndex);
				this._isCustom = true;
				this._currentOrgIndex = -1;
				this.RefreshChangeBirthAreaToggleState();
			}
		}

		// Token: 0x060064D2 RID: 25810 RVA: 0x002E151C File Offset: 0x002DF71C
		private void InitToggleGroup()
		{
			bool initDone = this._initDone;
			if (!initDone)
			{
				this.sectPlanToggleGroup.allowSwitchOff = true;
				this.sectPlanToggleGroup.allowUncheck = true;
				this.sectPlanToggleGroup.Init(-1);
				this.sectPlanToggleGroup.OnActiveIndexChange += this.OnSectPlanToggleIndexChange;
				this.RefreshToggles();
				bool flag = this.sectPlanToggleGroup.GetActiveIndex() < 0;
				if (flag)
				{
					this.sectPlanToggleGroup.Set(0, false);
				}
				this._isCustom = (this.sectPlanToggleGroup.GetActiveIndex() == 0);
				this._initDone = true;
				this.RefreshChangeBirthAreaToggleState();
			}
		}

		// Token: 0x060064D3 RID: 25811 RVA: 0x002E15C0 File Offset: 0x002DF7C0
		private void RefreshToggles()
		{
			List<CToggle> toggleList = this.sectPlanToggleGroup.GetAll();
			for (int i = 0; i < toggleList.Count; i++)
			{
				bool isOrg = i > 0 && i <= 15;
				toggleList[i].gameObject.SetActive(isOrg || i == 0);
				bool flag = isOrg;
				if (flag)
				{
					this.RefreshToggleTooltip(toggleList[i], i);
				}
				else
				{
					bool flag2 = i == 0 && this.toggleLabels != null && i < this.toggleLabels.Length && this.toggleLabels[i] != null;
					if (flag2)
					{
						this.toggleLabels[i].SetText(LanguageKey.LK_NewGame_CustomPreset_DefaultPreset_Toggle.Tr(), true);
					}
				}
			}
		}

		// Token: 0x060064D4 RID: 25812 RVA: 0x002E1684 File Offset: 0x002DF884
		private void RefreshToggleTooltip(CToggle toggle, int index)
		{
			OrganizationItem orgConfig = Organization.Instance[index];
			bool flag = orgConfig == null;
			if (!flag)
			{
				Graphic targetGraphic = toggle.targetGraphic;
				bool flag2 = targetGraphic == null;
				if (!flag2)
				{
					TooltipInvoker mouseTip = targetGraphic.gameObject.GetOrAddComponent<TooltipInvoker>();
					mouseTip.Type = TipType.SingleDesc;
					TooltipInvoker tooltipInvoker = mouseTip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					mouseTip.RuntimeParam.Clear();
					mouseTip.RuntimeParam.Set("arg0", orgConfig.Name);
					bool flag3 = this.toggleLabels != null && index < this.toggleLabels.Length && this.toggleLabels[index] != null;
					if (flag3)
					{
						this.toggleLabels[index].SetText(orgConfig.Name, true);
					}
				}
			}
		}

		// Token: 0x060064D5 RID: 25813 RVA: 0x002E1758 File Offset: 0x002DF958
		private void OnSectPlanToggleIndexChange(int newIndex, int oldIndex)
		{
			bool flag = !this._initDone || this._processingToggleChange;
			if (!flag)
			{
				this._processingToggleChange = true;
				bool flag2 = newIndex == -1 && oldIndex >= 0;
				if (flag2)
				{
					this.PerformAction(oldIndex);
					this.sectPlanToggleGroup.Set(oldIndex, true);
				}
				else
				{
					bool flag3 = newIndex >= 0;
					if (flag3)
					{
						this.PerformAction(newIndex);
						this.RefreshCheckmark2(newIndex, oldIndex);
					}
				}
				this.RefreshChangeBirthAreaToggleState();
				this._processingToggleChange = false;
			}
		}

		// Token: 0x060064D6 RID: 25814 RVA: 0x002E17DC File Offset: 0x002DF9DC
		private void PerformAction(int index)
		{
			bool flag = index == 0;
			if (flag)
			{
				this._isCustom = true;
				this._currentOrgIndex = -1;
				this.ApplyCustomDistribution();
			}
			else
			{
				bool flag2 = index > 0 && index <= 15;
				if (flag2)
				{
					this._isCustom = false;
					this._currentOrgIndex = index;
					this.ApplyOrgPreset(index);
				}
			}
		}

		// Token: 0x060064D7 RID: 25815 RVA: 0x002E1838 File Offset: 0x002DFA38
		private void RefreshCheckmark2(int newIndex, int oldIndex)
		{
			bool flag = oldIndex >= 0;
			if (flag)
			{
				CToggle oldToggle = this.sectPlanToggleGroup.Get(oldIndex);
				if (oldToggle != null)
				{
					Transform transform = oldToggle.transform.Find("Checkmark2");
					if (transform != null)
					{
						transform.gameObject.SetActive(false);
					}
				}
			}
			bool flag2 = newIndex >= 0;
			if (flag2)
			{
				CToggle newToggle = this.sectPlanToggleGroup.Get(newIndex);
				if (newToggle != null)
				{
					Transform transform2 = newToggle.transform.Find("Checkmark2");
					if (transform2 != null)
					{
						transform2.gameObject.SetActive(true);
					}
				}
			}
		}

		// Token: 0x060064D8 RID: 25816 RVA: 0x002E18C8 File Offset: 0x002DFAC8
		private void OnChangeBirthAreaToggleChanged(bool isOn)
		{
			bool flag = !isOn || this._isCustom || this._currentOrgIndex <= 0;
			if (!flag)
			{
				this.AutoSelectBirthArea(this._currentOrgIndex);
			}
		}

		// Token: 0x060064D9 RID: 25817 RVA: 0x002E1904 File Offset: 0x002DFB04
		private void RefreshChangeBirthAreaToggleState()
		{
			bool isCustom = this._isCustom;
			if (isCustom)
			{
				this.changeBirthAreaToggle.isOn = false;
				this.changeBirthAreaToggle.interactable = false;
				bool flag = this.changeBirthAreaTip != null;
				if (flag)
				{
					this.changeBirthAreaTip.Type = TipType.SingleDesc;
					TooltipInvoker tooltipInvoker = this.changeBirthAreaTip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					this.changeBirthAreaTip.RuntimeParam.Set("arg0", LanguageKey.LK_NewGame_SelectOrganization_AutoSelectBirthAreaToggle_Disable_Tip.Tr());
					this.changeBirthAreaTip.enabled = true;
				}
			}
			else
			{
				this.changeBirthAreaToggle.interactable = true;
				bool flag2 = this.changeBirthAreaTip != null;
				if (flag2)
				{
					TooltipInvoker tooltipInvoker = this.changeBirthAreaTip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					this.changeBirthAreaTip.enabled = false;
				}
			}
		}

		// Token: 0x060064DA RID: 25818 RVA: 0x002E19F4 File Offset: 0x002DFBF4
		private unsafe void ApplyCustomDistribution()
		{
			CustomProtagonistPresetItem data = new CustomProtagonistPresetItem();
			GlobalConfig global = GlobalConfig.Instance;
			data.Clear();
			int attrHalf = (int)(global.CustomProtagonistMainAttributeTotalPoint / 2);
			int perAttr = attrHalf / 6;
			int attrRem = attrHalf - perAttr * 6;
			for (sbyte i = 0; i < 6; i += 1)
			{
				*data.MainAttributes[(int)i] = (short)perAttr;
			}
			bool flag = attrRem > 0;
			if (flag)
			{
				ref short ptr = ref data.MainAttributes[0];
				ptr += (short)attrRem;
			}
			int lifeHalf = (int)(global.CustomProtagonistLifeSkillQualificationTotalPoint / 2);
			int perLife = lifeHalf / 16;
			int lifeRem = lifeHalf - perLife * 16;
			for (sbyte j = 0; j < 16; j += 1)
			{
				*data.LifeSkillQualifications[(int)j] = (short)perLife;
			}
			bool flag2 = lifeRem > 0;
			if (flag2)
			{
				ref short ptr2 = ref data.LifeSkillQualifications[0];
				ptr2 += (short)lifeRem;
			}
			int combatHalf = (int)(global.CustomProtagonistCombatSkillQualificationTotalPoint / 2);
			int perCombat = combatHalf / 14;
			int combatRem = combatHalf - perCombat * 14;
			for (sbyte k = 0; k < 14; k += 1)
			{
				*data.CombatSkillQualifications[(int)k] = (short)perCombat;
			}
			bool flag3 = combatRem > 0;
			if (flag3)
			{
				ref short ptr3 = ref data.CombatSkillQualifications[0];
				ptr3 += (short)combatRem;
			}
			bool flag4 = this.ParentCustomPresetPage != null;
			if (flag4)
			{
				this.ParentCustomPresetPage.ApplyDataToSubPages(data);
				this.ParentCustomPresetPage.OnSubPageDataModified();
			}
		}

		// Token: 0x060064DB RID: 25819 RVA: 0x002E1B64 File Offset: 0x002DFD64
		private void ApplyOrgPreset(int toggleIndex)
		{
			int orgIndex = toggleIndex - 1;
			int characterId = 1060 + orgIndex;
			CharacterItem characterItem = Character.Instance[characterId];
			bool flag = characterItem == null;
			if (!flag)
			{
				CustomProtagonistPresetItem orgData = characterItem;
				bool flag2 = this.ParentCustomPresetPage != null;
				if (flag2)
				{
					this.ParentCustomPresetPage.ApplyOrgPresetData(orgData);
				}
				bool isOn = this.changeBirthAreaToggle.isOn;
				if (isOn)
				{
					this.AutoSelectBirthArea(toggleIndex);
				}
			}
		}

		// Token: 0x060064DC RID: 25820 RVA: 0x002E1BD8 File Offset: 0x002DFDD8
		private void AutoSelectBirthArea(int toggleIndex)
		{
			OrganizationItem orgConfig = Organization.Instance[toggleIndex];
			bool flag = orgConfig == null;
			if (!flag)
			{
				sbyte mapStateId = NewGameSubPageCustomPresetSelectOrganization.FindMapStateIdForOrg(toggleIndex);
				bool flag2 = mapStateId >= 0;
				if (flag2)
				{
					MapStateItem mapStateItem = MapState.Instance[mapStateId];
					bool flag3 = mapStateItem != null;
					if (flag3)
					{
						LandFormTypeItem landFormTypeItem = LandFormType.Instance[0];
						this.ParentCustomPresetPage.CreationInfoMap["TaiwuVillageStateTemplateId"] = mapStateItem.TemplateId.ToString();
						this.ParentCustomPresetPage.CreationInfoMap["TaiwuVillageLandFormType"] = ((int)((landFormTypeItem != null) ? landFormTypeItem.TemplateId : 0)).ToString();
						NewGameSubPageBornArea bornSubPage = this.ParentCustomPresetPage.GetTargetSubPage<NewGameSubPageBornArea>(ViewNewGame.ENewGameSubType.BornArea);
						if (bornSubPage != null)
						{
							bornSubPage.Setup();
						}
					}
				}
				bool flag4 = orgConfig.MainMorality > short.MinValue;
				if (flag4)
				{
					this.ParentCustomPresetPage.CreationInfoMap["Morality"] = orgConfig.MainMorality.ToString();
					NewGameSubPageName nameSubPage = this.ParentCustomPresetPage.GetTargetSubPage<NewGameSubPageName>(ViewNewGame.ENewGameSubType.Name);
					if (nameSubPage != null)
					{
						nameSubPage.Init();
					}
				}
			}
		}

		// Token: 0x060064DD RID: 25821 RVA: 0x002E1D00 File Offset: 0x002DFF00
		private static sbyte FindMapStateIdForOrg(int orgId)
		{
			for (sbyte i = 0; i <= 30; i += 1)
			{
				MapStateItem ms = MapState.Instance[i];
				bool flag = ms != null && ms.SectID == (sbyte)orgId;
				if (flag)
				{
					return ms.TemplateId;
				}
			}
			return -1;
		}

		// Token: 0x04004635 RID: 17973
		[Header("内部引用")]
		[SerializeField]
		private CToggle changeBirthAreaToggle;

		// Token: 0x04004636 RID: 17974
		[SerializeField]
		private CToggleGroup sectPlanToggleGroup;

		// Token: 0x04004637 RID: 17975
		[SerializeField]
		private TooltipInvoker changeBirthAreaTip;

		// Token: 0x04004638 RID: 17976
		[SerializeField]
		private TextMeshProUGUI[] toggleLabels;

		// Token: 0x04004639 RID: 17977
		private bool _initDone;

		// Token: 0x0400463A RID: 17978
		private bool _processingToggleChange;

		// Token: 0x0400463B RID: 17979
		private bool _isCustom;

		// Token: 0x0400463C RID: 17980
		private int _currentOrgIndex = -1;

		// Token: 0x0400463D RID: 17981
		private const int OrgPresetCount = 15;

		// Token: 0x0400463E RID: 17982
		private const int OrgPresetStartId = 1060;
	}
}
