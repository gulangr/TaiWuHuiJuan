using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using Game.Components.Character;
using Game.Components.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200083F RID: 2111
	public class AttributePanel : MonoBehaviour
	{
		// Token: 0x17000C5D RID: 3165
		// (get) Token: 0x060066D9 RID: 26329 RVA: 0x002EEA07 File Offset: 0x002ECC07
		public RectTransform RectTransform
		{
			get
			{
				return base.transform as RectTransform;
			}
		}

		// Token: 0x060066DA RID: 26330 RVA: 0x002EEA14 File Offset: 0x002ECC14
		public unsafe void Set(CharacterDisplayDataForMapBlock data)
		{
			this.personalityType.Set(data.Personalities);
			this.medalSummary.Set(data.AttackMedal, data.DefenceMedal, data.WisdomMedal);
			this.teammateCommands.Set(data.AttackMedal, data.DefenceMedal, data.WisdomMedal, data.TeammateCommands, null);
			for (int i = 0; i < 6; i++)
			{
				this.mainAttributes[i].SetValue(data.MainAttributes[i].ToString());
			}
			this.lifeTitleGrowth.text = LanguageKey.LK_Building_RejectRecruitPeople_Tips_Detail_1.TrFormat(CommonUtils.GetAgeGrowthString((int)data.LifeSkillGrowthType, (int)data.Age)).ColorReplace();
			this.combatTitleGrowth.text = LanguageKey.LK_Building_RejectRecruitPeople_Tips_Detail_2.TrFormat(CommonUtils.GetAgeGrowthString((int)data.CombatSkillGrowthType, (int)data.Age)).ColorReplace();
			for (int j = 0; j < 16; j++)
			{
				short value = *data.LifeSkillQualifications[j];
				sbyte grade = Grade.GetQualificationGrade((int)value);
				this.life[j].Set(string.Format("{0}{1}", this.lifeIconNamePrefix, j), LanguageKey.LK_Mousetip_CharacterOnMapBlock_Qualifications_And_Attainments_Text.TrFormat(new object[]
				{
					Config.LifeSkillType.Instance[j].Name,
					Colors.Instance.GradeColors[(int)grade].ColorToHexString(""),
					value,
					*data.LifeSkillAttainments[j]
				}).ColorReplace());
			}
			for (int k = 0; k < 14; k++)
			{
				short value2 = *data.CombatSkillQualifications[k];
				sbyte grade2 = Grade.GetQualificationGrade((int)value2);
				this.combat[k].Set(string.Format("{0}{1}", this.combatIconNamePrefix, k), LanguageKey.LK_Mousetip_CharacterOnMapBlock_Qualifications_And_Attainments_Text.TrFormat(new object[]
				{
					CombatSkillType.Instance[k].Name,
					Colors.Instance.GradeColors[(int)grade2].ColorToHexString(""),
					value2,
					*data.CombatSkillAttainments[k]
				}).ColorReplace());
			}
			int value3 = data.DivinePower;
			sbyte grade3 = Grade.GetQualificationGrade(value3);
			this.divine.Set(this.combatIconNamePrefix + "14", LanguageKey.LK_Mousetip_CharacterOnMapBlock_Qualifications_And_Attainments_Text.TrFormat(new object[]
			{
				LanguageKey.LK_Shenli.Tr(),
				Colors.Instance.GradeColors[(int)grade3].ColorToHexString(""),
				value3,
				data.DivinePower
			}).ColorReplace());
			value3 = data.GhostTechnique;
			grade3 = Grade.GetQualificationGrade(value3);
			this.ghost.Set(this.combatIconNamePrefix + "15", LanguageKey.LK_Mousetip_CharacterOnMapBlock_Qualifications_And_Attainments_Text.TrFormat(new object[]
			{
				LanguageKey.LK_Guishu.Tr(),
				Colors.Instance.GradeColors[(int)grade3].ColorToHexString(""),
				value3,
				data.GhostTechnique
			}).ColorReplace());
			this.SetFeature(data);
		}

		// Token: 0x060066DB RID: 26331 RVA: 0x002EED90 File Offset: 0x002ECF90
		private void SetFeature(CharacterDisplayDataForMapBlock data)
		{
			List<short> featureIds = (from x in data.FeatureIds
			where !CharacterFeature.Instance[x].Hidden
			select x).ToPoolList<short>();
			bool showMoreFeatureItem = featureIds.Count > 9;
			this.features.Rebuild<Feature>(showMoreFeatureItem ? 8 : featureIds.Count, delegate(Feature container, int i)
			{
				short featureId = featureIds[i];
				int characterId = data.CharacterId;
				bool isTaiwu = false;
				Dictionary<short, int> temporaryFeatureLeftTimes = data.TemporaryFeatureLeftTimes;
				int duration;
				container.Set(featureId, characterId, isTaiwu, (temporaryFeatureLeftTimes != null && temporaryFeatureLeftTimes.TryGetValue(featureIds[i], out duration)) ? duration : -1);
			});
			int lessCount = showMoreFeatureItem ? 0 : ((featureIds.Count + 3 - 1) / 3 * 3 - featureIds.Count);
			int j = this.lessFeatureGo.Length;
			while (j-- > 0)
			{
				this.lessFeatureGo[j].gameObject.SetActive(j < lessCount);
			}
			this.moreFeatureText.text = LanguageKey.LK_MouseTipCharacterComplete_MoreFeature_Label.TrFormat(featureIds.Count).ColorReplace();
			this.moreFeatureGo.gameObject.SetActive(showMoreFeatureItem);
			EasyPool.Free<List<short>>(featureIds);
		}

		// Token: 0x04004850 RID: 18512
		[SerializeField]
		private Game.Components.Character.Personalities personalityType;

		// Token: 0x04004851 RID: 18513
		[SerializeField]
		private TeammateCommands teammateCommands;

		// Token: 0x04004852 RID: 18514
		[SerializeField]
		private MedalSummary medalSummary;

		// Token: 0x04004853 RID: 18515
		[SerializeField]
		private PropertyItem[] mainAttributes;

		// Token: 0x04004854 RID: 18516
		[SerializeField]
		private TMP_Text lifeTitleGrowth;

		// Token: 0x04004855 RID: 18517
		[SerializeField]
		private TMP_Text combatTitleGrowth;

		// Token: 0x04004856 RID: 18518
		[SerializeField]
		private QualificationItem[] life;

		// Token: 0x04004857 RID: 18519
		[SerializeField]
		private QualificationItem[] combat;

		// Token: 0x04004858 RID: 18520
		[SerializeField]
		private QualificationItem divine;

		// Token: 0x04004859 RID: 18521
		[SerializeField]
		private QualificationItem ghost;

		// Token: 0x0400485A RID: 18522
		[SerializeField]
		private TemplatedContainerAssemblyNew features;

		// Token: 0x0400485B RID: 18523
		[SerializeField]
		private TMP_Text moreFeatureText;

		// Token: 0x0400485C RID: 18524
		[SerializeField]
		private GameObject moreFeatureGo;

		// Token: 0x0400485D RID: 18525
		[SerializeField]
		private GameObject[] lessFeatureGo;

		// Token: 0x0400485E RID: 18526
		[SerializeField]
		private string lifeIconNamePrefix = "ui9_icon_craftsmanship_small_1_";

		// Token: 0x0400485F RID: 18527
		[SerializeField]
		private string combatIconNamePrefix = "ui9_icon_attainments_small_1_";
	}
}
