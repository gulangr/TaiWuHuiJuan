using System;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Building
{
	// Token: 0x02000BE7 RID: 3047
	public class RecruitFlatModeCharacterView : MonoBehaviour
	{
		// Token: 0x06009A86 RID: 39558 RVA: 0x00485C25 File Offset: 0x00483E25
		private void Awake()
		{
			this.btnMain.ClearAndAddListener(delegate
			{
				Action onClick = this._onClick;
				if (onClick != null)
				{
					onClick();
				}
			});
		}

		// Token: 0x06009A87 RID: 39559 RVA: 0x00485C40 File Offset: 0x00483E40
		public void Set(BuildingRecruitCharacterData buildingRecruitCharacterData, bool isSelected, sbyte requireLifeSkillType, sbyte requireCombatSkillType, Action onClick, short buildingTemplateId)
		{
			this._onClick = onClick;
			RecruitCharacterData data = buildingRecruitCharacterData.CharacterData;
			AvatarRelatedData avatarRelateData = data.GenerateAvatarRelatedData();
			this.avatar.Refresh(avatarRelateData, data.TemplateId);
			ValueTuple<string, string> name = data.FullName.GetName(data.Gender, SingletonObject.getInstance<BasicGameData>().CustomTexts);
			string surname = name.Item1;
			string givenName = name.Item2;
			this.txtName.text = surname + givenName;
			this.txtAge.text = data.Age.ToString();
			CharacterItem charConfig = Character.Instance.GetItem(data.TemplateId);
			int remainTime = GameData.Domains.Building.SharedMethods.GetBuildingEarnPreserveTime(buildingTemplateId) - buildingRecruitCharacterData.RecruitLevel.Second;
			this.stayComp.SetValue(remainTime.ToString());
			bool needAuthority = buildingTemplateId == 223;
			this.authorityComp.SetValue(needAuthority ? GlobalConfig.Instance.RecruitPeopleCost.ToString() : "-");
			CommonUtils.EDisplayGender displayGender = CommonUtils.GetDisplayGender(data.Gender, -1);
			this.genderComp.Set(CommonUtils.GetGenderIconBig(displayGender), LanguageKey.LK_Main_SummaryInfo_Gender.Tr(), CommonUtils.GetGenderString(displayGender));
			bool isFixedPresetType = CreatingType.IsFixedPresetType(charConfig.CreatingType);
			string charmIcon = CommonUtils.GetCharmLevelBigIcon(data.FinalAttraction, data.Age, avatarRelateData.ClothingDisplayId, avatarRelateData.AvatarData.FaceVisible, isFixedPresetType);
			this.charmComp.Set(charmIcon, LanguageKey.LK_Main_SummaryInfo_Charm.Tr(), CommonUtils.GetCharmLevelText(data.FinalAttraction, data.Gender, data.Age, avatarRelateData.ClothingDisplayId, isFixedPresetType, true));
			short baseMorality = data.GetBaseMorality();
			sbyte behaviorType = GameData.Domains.Character.BehaviorType.GetBehaviorType(baseMorality);
			this.behaviorTypeComp.Set(CommonUtils.GetBehaviorTypeIcon(behaviorType), LanguageKey.LK_Main_SummaryInfo_Behavior.Tr(), CommonUtils.GetBehaviorString(behaviorType));
			bool flag = requireCombatSkillType >= 0;
			if (flag)
			{
				string icon = string.Format("{0}{1}", "ui9_back_attainments_combat_", requireCombatSkillType);
				CombatSkillTypeItem combatSkillConfig = CombatSkillType.Instance[requireCombatSkillType];
				this.qualificationComp.Set(icon, combatSkillConfig.Name, data.CombatSkillQualifications[(int)requireCombatSkillType].ToString());
				this.growthComp.SetValue(LanguageKey.LK_Growth.Tr(), CommonUtils.GetSkillGrowthString((int)data.CombatSkillQualificationGrowthType, (int)data.Age));
			}
			else
			{
				string icon2 = string.Format("{0}{1}", "ui9_back_attainments_life_3_", requireLifeSkillType);
				LifeSkillTypeItem lifeSkillConfig = Config.LifeSkillType.Instance[requireLifeSkillType];
				this.qualificationComp.Set(icon2, lifeSkillConfig.Name, data.LifeSkillQualifications[(int)requireLifeSkillType].ToString());
				this.growthComp.SetValue(LanguageKey.LK_Growth.Tr(), CommonUtils.GetSkillGrowthString((int)data.CombatSkillQualificationGrowthType, (int)data.Age));
			}
			this.mouseTip.enabled = false;
			this.mouseTip.Type = TipType.CharacterComplete;
			ExtraDomainMethod.AsyncCall.RequestRecruitCharacterData(null, buildingRecruitCharacterData.BuildingBlockKey, buildingRecruitCharacterData.RecruitInfoIndex, delegate(int offset2, RawDataPool pool2)
			{
				RecruitCharacterData recruitCharacterData = null;
				Serializer.Deserialize(pool2, offset2, ref recruitCharacterData);
				bool flag2 = recruitCharacterData != null;
				if (flag2)
				{
					this.mouseTip.enabled = true;
					this.mouseTip.RuntimeParam = new ArgumentBox();
					int remainTime2 = GameData.Domains.Building.SharedMethods.GetBuildingEarnPreserveTime(buildingTemplateId) - buildingRecruitCharacterData.RecruitLevel.Second;
					this.mouseTip.RuntimeParam.Set("RemainTime", remainTime2);
					this.mouseTip.RuntimeParam.SetObject("Data", new CharacterDisplayDataForTooltip(recruitCharacterData));
				}
			});
			this.SetSelected(isSelected);
		}

		// Token: 0x06009A88 RID: 39560 RVA: 0x00485F7B File Offset: 0x0048417B
		private void SetSelected(bool isSelected)
		{
			this.selectedObj.SetActive(isSelected);
			this.selectedObj2.SetActive(isSelected);
		}

		// Token: 0x04007779 RID: 30585
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x0400777A RID: 30586
		[SerializeField]
		private TextMeshProUGUI txtName;

		// Token: 0x0400777B RID: 30587
		[SerializeField]
		private TextMeshProUGUI txtAge;

		// Token: 0x0400777C RID: 30588
		[SerializeField]
		private ComponentIconTitleValue stayComp;

		// Token: 0x0400777D RID: 30589
		[SerializeField]
		private ComponentIconTitleValue authorityComp;

		// Token: 0x0400777E RID: 30590
		[SerializeField]
		private ComponentIconTitleValue behaviorTypeComp;

		// Token: 0x0400777F RID: 30591
		[SerializeField]
		private ComponentIconTitleValue charmComp;

		// Token: 0x04007780 RID: 30592
		[SerializeField]
		private ComponentIconTitleValue genderComp;

		// Token: 0x04007781 RID: 30593
		[SerializeField]
		private ComponentIconTitleValue qualificationComp;

		// Token: 0x04007782 RID: 30594
		[SerializeField]
		private ComponentIconTitleValue growthComp;

		// Token: 0x04007783 RID: 30595
		[SerializeField]
		private GameObject selectedObj;

		// Token: 0x04007784 RID: 30596
		[SerializeField]
		private GameObject selectedObj2;

		// Token: 0x04007785 RID: 30597
		[SerializeField]
		private CButton btnMain;

		// Token: 0x04007786 RID: 30598
		[SerializeField]
		private TooltipInvoker mouseTip;

		// Token: 0x04007787 RID: 30599
		private Action _onClick;
	}
}
