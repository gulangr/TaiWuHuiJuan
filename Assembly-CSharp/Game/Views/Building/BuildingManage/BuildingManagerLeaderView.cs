using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.Common;
using GameData.Domains.Building;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000BF2 RID: 3058
	public class BuildingManagerLeaderView : MonoBehaviour
	{
		// Token: 0x06009B6B RID: 39787 RVA: 0x0048CA14 File Offset: 0x0048AC14
		public unsafe void Refresh(int index, int charId, BuildingBlockData blockData, VillagerRoleCharacterDisplayData villagerRoleData, CharacterDisplayData charData, int efficiency, bool hasTaiwuShrine, bool isUnlocked, Action<int> onSelectChar, Action<int> onCancelChar, Action<int, bool> onSetUnlockChar, Action<bool> onAssignRole)
		{
			this._charId = charId;
			bool exist = charId >= 0;
			this.rootExist.SetActive(exist);
			string warningContent = exist ? string.Empty : LanguageKey.LK_Building_TeachBook_FailReason1.Tr().ColorReplace();
			this.SetWarningTip(warningContent);
			this.buttonSelect.ClearAndAddListener(delegate
			{
				onSelectChar(index);
			});
			this.buttonChange.ClearAndAddListener(delegate
			{
				onSelectChar(index);
			});
			this.buttonCancel.ClearAndAddListener(delegate
			{
				onCancelChar(index);
			});
			bool flag = !exist;
			if (!flag)
			{
				this.buttonAvatar.ClearAndAddListener(delegate
				{
					this.ShowCharacterMenu(charId);
				});
				BuildingBlockItem buildingConfig = BuildingBlock.Instance[blockData.TemplateId];
				this.avatar.Refresh(charData, true);
				this.textAge.text = LanguageKey.LK_Age.TrFormat(charData.PhysiologicalAge);
				this.textName.text = NameCenter.GetMonasticTitleOrDisplayName(charData, false);
				string title = LanguageKey.LK_Main_SummaryInfo_Identity.Tr();
				string value = CommonUtils.GetIdentityString(charData.OrgInfo, charData.Gender, charData.PhysiologicalAge, false);
				this.propertyIdentity.Set(title, value, null);
				string skillName = string.Empty;
				int skillAttainment = 0;
				int skillQualification = 0;
				bool flag2 = buildingConfig.RequireLifeSkillType >= 0;
				if (flag2)
				{
					sbyte type = buildingConfig.RequireLifeSkillType;
					LifeSkillTypeItem config = LifeSkillType.Instance[type];
					skillName = config.Name;
					skillAttainment = (int)(*villagerRoleData.LifeSkillAttainments[(int)type]);
					skillQualification = (int)(*villagerRoleData.LifeSkillQualifications[(int)type]);
				}
				else
				{
					bool flag3 = buildingConfig.RequireCombatSkillType >= 0;
					if (flag3)
					{
						sbyte type2 = buildingConfig.RequireCombatSkillType;
						CombatSkillTypeItem config2 = CombatSkillType.Instance[type2];
						skillName = config2.Name;
						skillAttainment = (int)(*villagerRoleData.CombatSkillAttainments[(int)type2]);
						skillQualification = (int)(*villagerRoleData.CombatSkillQualifications[(int)type2]);
					}
				}
				string title2 = skillName + LanguageKey.LK_Attainment.Tr();
				this.propertyAttainment.Set(title2, skillAttainment.ToString(), null);
				string title3 = skillName + LanguageKey.LK_Qualification.Tr();
				this.propertyQualification.Set(title3, skillQualification.ToString(), null);
				string title4 = LanguageKey.LK_Building_Arrangement_Teach.Tr();
				string value2 = LocalStringManager.Get(string.Format("LK_Grade_{0}", villagerRoleData.ReadBookMaxGrade)).SetColor(Colors.Instance.GradeColors[(int)villagerRoleData.ReadBookMaxGrade]);
				this.propertyTeachGrade.Set(title4, value2, null);
				string title5 = LanguageKey.LK_Building_Shop_CharacterInfo_Efficency.Tr();
				string value3 = string.Format("{0}%", efficiency);
				this.propertyEfficiency.Set(title5, value3, null);
				this.textMatch.text = (villagerRoleData.MatchVillagerRole ? LanguageKey.LK_Building_Arrangement_TeachMatch.Tr() : LanguageKey.LK_Building_Arrangement_TeachNotMatch.Tr());
				this.buttonMatch.gameObject.SetActive(villagerRoleData.MatchVillagerRole);
				this.buttonNotMatch.gameObject.SetActive(!villagerRoleData.MatchVillagerRole);
				CButton button = villagerRoleData.MatchVillagerRole ? this.buttonMatch : this.buttonNotMatch;
				button.interactable = hasTaiwuShrine;
				bool interactable = button.interactable;
				if (interactable)
				{
					button.ClearAndAddListener(delegate
					{
						bool flag4 = !villagerRoleData.MatchVillagerRole;
						if (flag4)
						{
							short roleKey = buildingConfig.VillagerRoleTemplateIds[0];
							VillagerRoleUtils.ConfirmAndAssignRole(villagerRoleData.Id, roleKey, null, onAssignRole, null);
						}
						else
						{
							VillagerRoleUtils.ConfirmAndAssignRole(villagerRoleData.Id, -1, null, onAssignRole, null);
						}
					});
				}
				this.buttonLock.gameObject.SetActive(!isUnlocked);
				this.buttonUnlock.gameObject.SetActive(isUnlocked);
				this.buttonLock.ClearAndAddListener(delegate
				{
					onSetUnlockChar(charId, true);
				});
				this.buttonUnlock.ClearAndAddListener(delegate
				{
					onSetUnlockChar(charId, false);
				});
				this.InitMouseTipDisplayer();
				this.InitLeaderTips(buildingConfig, this.buttonMatch);
				this.InitLeaderTips(buildingConfig, this.buttonNotMatch);
			}
		}

		// Token: 0x06009B6C RID: 39788 RVA: 0x0048CED7 File Offset: 0x0048B0D7
		public void SetWarningTip(string content)
		{
			this.rootWarning.SetActive(!content.IsNullOrEmpty());
			this.textWarning.text = content;
		}

		// Token: 0x06009B6D RID: 39789 RVA: 0x0048CEFC File Offset: 0x0048B0FC
		private void ShowCharacterMenu(int charId)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharacterId", charId);
			argBox.Set("PreviousView", 12);
			UIElement.CharacterMenu.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x06009B6E RID: 39790 RVA: 0x0048CF48 File Offset: 0x0048B148
		private void InitMouseTipDisplayer()
		{
			TooltipInvoker mouseTipDisplayer = this.tipChar;
			bool flag = mouseTipDisplayer == null;
			if (!flag)
			{
				mouseTipDisplayer.enabled = (this._charId >= 0);
				bool flag2 = this._charId < 0;
				if (!flag2)
				{
					mouseTipDisplayer.Type = TipType.CharacterOnMapBlock;
					TooltipInvoker tooltipInvoker = mouseTipDisplayer;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					mouseTipDisplayer.RuntimeParam.Set("CharId", this._charId);
				}
			}
		}

		// Token: 0x06009B6F RID: 39791 RVA: 0x0048CFC4 File Offset: 0x0048B1C4
		private void InitLeaderTips(BuildingBlockItem buildingConfig, CButton button)
		{
			short[] roleIds = buildingConfig.VillagerRoleTemplateIds;
			TooltipInvoker tipMatch = button.GetComponent<TooltipInvoker>();
			tipMatch.Type = TipType.GeneralLines;
			TooltipInvoker tooltipInvoker = tipMatch;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			BuildingManagerLeaderView.<>c__DisplayClass26_0 CS$<>8__locals1;
			CS$<>8__locals1.param = tipMatch.RuntimeParam;
			CS$<>8__locals1.param.Set("Title", LocalStringManager.Get(LanguageKey.LK_Building_Leader_Tips_Title));
			CS$<>8__locals1.lineCount = 0;
			BuildingManagerLeaderView.<InitLeaderTips>g__AddNode|26_0(new GeneralLineData
			{
				Type = 3,
				Args = new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Building_Leader_Tips_Content)
				}
			}, ref CS$<>8__locals1);
			BuildingManagerLeaderView.<InitLeaderTips>g__AddNode|26_0(new GeneralLineData
			{
				Type = 1,
				Args = new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Building_Leader_Tips_SubTitle_1)
				}
			}, ref CS$<>8__locals1);
			foreach (short roleId in roleIds)
			{
				string roleName = VillagerRoleUtils.GetRoleOriginalNameWithGrade(roleId);
				BuildingManagerLeaderView.<InitLeaderTips>g__AddNode|26_0(new GeneralLineData
				{
					Type = 5,
					Args = new List<string>
					{
						roleName
					},
					ExtraArgs = new List<object>
					{
						20
					}
				}, ref CS$<>8__locals1);
			}
			CS$<>8__locals1.param.Set("LineCount", CS$<>8__locals1.lineCount);
		}

		// Token: 0x06009B70 RID: 39792 RVA: 0x0048D114 File Offset: 0x0048B314
		public void Refresh(CharacterDisplayData charData, sbyte lifeSkillType, int attainment, int qualification, int progressDelta)
		{
			this._charId = charData.CharacterId;
			this.buttonAvatar.ClearAndAddListener(delegate
			{
				this.ShowCharacterMenu(charData.CharacterId);
			});
			this.avatar.Refresh(charData, true);
			this.textAge.text = LanguageKey.LK_Age.TrFormat(charData.PhysiologicalAge);
			this.textName.text = NameCenter.GetMonasticTitleOrDisplayName(charData, false);
			this.InitMouseTipDisplayer();
			string title = LanguageKey.LK_Main_SummaryInfo_Identity.Tr();
			string value = CommonUtils.GetIdentityString(charData.OrgInfo, charData.Gender, charData.PhysiologicalAge, false);
			this.propertyIdentity.Set(title, value, null);
			LifeSkillTypeItem skillConfig = LifeSkillType.Instance[lifeSkillType];
			string skillName = skillConfig.Name;
			string title2 = skillName + LanguageKey.LK_Attainment.Tr();
			this.propertyAttainment.Set(title2, attainment.ToString(), null);
			string title3 = skillName + LanguageKey.LK_Qualification.Tr();
			this.propertyQualification.Set(title3, qualification.ToString(), null);
			string title4 = LanguageKey.LK_Building_Shop_CharacterInfo_Efficency.Tr();
			string value2 = string.Format("{0}%", progressDelta);
			this.propertyEfficiency.Set(title4, value2, null);
			this.propertyIdentity.gameObject.SetActive(true);
			this.propertyAttainment.gameObject.SetActive(true);
			this.propertyQualification.gameObject.SetActive(true);
			this.propertyEfficiency.gameObject.SetActive(true);
			this.buttonLock.gameObject.SetActive(false);
			this.buttonUnlock.gameObject.SetActive(false);
			this.buttonMatch.gameObject.SetActive(false);
			this.buttonNotMatch.gameObject.SetActive(false);
			this.textMatch.gameObject.SetActive(false);
			this.propertyTeachGrade.gameObject.SetActive(false);
			this.buttonSelect.gameObject.SetActive(false);
			this.buttonChange.gameObject.SetActive(false);
			this.buttonCancel.gameObject.SetActive(false);
		}

		// Token: 0x06009B72 RID: 39794 RVA: 0x0048D3B0 File Offset: 0x0048B5B0
		[CompilerGenerated]
		internal static void <InitLeaderTips>g__AddNode|26_0(GeneralLineData lineData, ref BuildingManagerLeaderView.<>c__DisplayClass26_0 A_1)
		{
			ArgumentBox param = A_1.param;
			string format = "LineData{0}";
			int num = A_1.lineCount + 1;
			A_1.lineCount = num;
			param.SetObject(string.Format(format, num), lineData);
		}

		// Token: 0x04007857 RID: 30807
		[SerializeField]
		private TooltipInvoker tipChar;

		// Token: 0x04007858 RID: 30808
		[SerializeField]
		private CButton buttonUnlock;

		// Token: 0x04007859 RID: 30809
		[SerializeField]
		private CButton buttonLock;

		// Token: 0x0400785A RID: 30810
		[SerializeField]
		private CButton buttonSelect;

		// Token: 0x0400785B RID: 30811
		[SerializeField]
		private CButton buttonCancel;

		// Token: 0x0400785C RID: 30812
		[SerializeField]
		private CButton buttonMatch;

		// Token: 0x0400785D RID: 30813
		[SerializeField]
		private CButton buttonNotMatch;

		// Token: 0x0400785E RID: 30814
		[SerializeField]
		private TextMeshProUGUI textMatch;

		// Token: 0x0400785F RID: 30815
		[SerializeField]
		private CButton buttonChange;

		// Token: 0x04007860 RID: 30816
		[SerializeField]
		private CButton buttonAvatar;

		// Token: 0x04007861 RID: 30817
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04007862 RID: 30818
		[SerializeField]
		private GameObject rootExist;

		// Token: 0x04007863 RID: 30819
		[SerializeField]
		private GameObject rootWarning;

		// Token: 0x04007864 RID: 30820
		[SerializeField]
		private TextMeshProUGUI textWarning;

		// Token: 0x04007865 RID: 30821
		[SerializeField]
		private TextMeshProUGUI textAge;

		// Token: 0x04007866 RID: 30822
		[SerializeField]
		private TextMeshProUGUI textName;

		// Token: 0x04007867 RID: 30823
		[SerializeField]
		private PropertyItem propertyIdentity;

		// Token: 0x04007868 RID: 30824
		[SerializeField]
		private PropertyItem propertyQualification;

		// Token: 0x04007869 RID: 30825
		[SerializeField]
		private PropertyItem propertyAttainment;

		// Token: 0x0400786A RID: 30826
		[SerializeField]
		private PropertyItem propertyTeachGrade;

		// Token: 0x0400786B RID: 30827
		[SerializeField]
		private PropertyItem propertyEfficiency;

		// Token: 0x0400786C RID: 30828
		private int _charId;
	}
}
