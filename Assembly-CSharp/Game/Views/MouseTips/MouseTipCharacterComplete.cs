using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using Game.Components.Avatar;
using Game.Components.Character;
using Game.Components.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200083B RID: 2107
	public class MouseTipCharacterComplete : MouseTipBase
	{
		// Token: 0x060066B9 RID: 26297 RVA: 0x002ED908 File Offset: 0x002EBB08
		protected override void Init(ArgumentBox argsBox)
		{
			MouseTipCharacterComplete.<>c__DisplayClass25_0 CS$<>8__locals1 = new MouseTipCharacterComplete.<>c__DisplayClass25_0();
			CS$<>8__locals1.<>4__this = this;
			base.RectTransform.localScale = Vector3.zero;
			bool flag = !argsBox.Get("RemainTime", out CS$<>8__locals1.remainTime);
			if (flag)
			{
				CS$<>8__locals1.remainTime = -1;
			}
			CharacterDisplayDataForTooltip data;
			bool flag2 = argsBox.Get<CharacterDisplayDataForTooltip>("Data", out data);
			if (flag2)
			{
				this.OnDataReady(data, CS$<>8__locals1.remainTime);
			}
			else
			{
				int charId;
				bool flag3 = argsBox.Get("CharId", out charId);
				if (!flag3)
				{
					throw new ArgumentException("No character given.");
				}
				CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForTooltip(this, charId, new AsyncMethodCallbackDelegate(CS$<>8__locals1.<Init>g__CallBack|0));
			}
		}

		// Token: 0x060066BA RID: 26298 RVA: 0x002ED9AC File Offset: 0x002EBBAC
		private void OnDataReady(CharacterDisplayDataForTooltip data, int remainTime)
		{
			bool flag = data == null;
			if (!flag)
			{
				this.SetMainFrame(data, remainTime);
				this.SetFeatureFrame(data);
				this.SetSkillFrame(data);
				this.SetIllustration(data);
				base.RectTransform.localScale = Vector3.one;
			}
		}

		// Token: 0x060066BB RID: 26299 RVA: 0x002ED9F6 File Offset: 0x002EBBF6
		private void SetMainFrame(CharacterDisplayDataForTooltip data, int remainTime)
		{
			this.SetAvatar(data);
			this.SetPersonality(data);
			this.SetMainAttribute(data);
			this.SetRemainTime(remainTime);
		}

		// Token: 0x060066BC RID: 26300 RVA: 0x002EDA1C File Offset: 0x002EBC1C
		private void SetAvatar(CharacterDisplayDataForTooltip data)
		{
			bool flag = data.TemplateId >= 0;
			if (flag)
			{
				this.avatar.Refresh(data.AvatarRelatedData, data.TemplateId);
			}
			else
			{
				this.avatar.Refresh(data.AvatarRelatedData);
			}
			NameRelatedData nameRelatedData = data.GetNameRelatedData();
			this.title.text = (this.charName.text = NameCenter.GetMonasticTitleOrDisplayName(ref nameRelatedData, data.Id == SingletonObject.getInstance<BasicGameData>().TaiwuCharId, false));
		}

		// Token: 0x060066BD RID: 26301 RVA: 0x002EDAA2 File Offset: 0x002EBCA2
		private void SetPersonality(CharacterDisplayDataForTooltip data)
		{
			this.personalityType.Set(data.Personalities);
		}

		// Token: 0x060066BE RID: 26302 RVA: 0x002EDAB8 File Offset: 0x002EBCB8
		private void SetMainAttribute(CharacterDisplayDataForTooltip data)
		{
			Game.Components.Character.Gender gender = this.gender;
			sbyte b = data.Gender;
			if (!true)
			{
			}
			CommonUtils.EDisplayGender displayGender;
			if (b != 0)
			{
				if (b != 1)
				{
					displayGender = CommonUtils.EDisplayGender.Hidden;
				}
				else
				{
					displayGender = CommonUtils.EDisplayGender.Male;
				}
			}
			else
			{
				displayGender = CommonUtils.EDisplayGender.Female;
			}
			if (!true)
			{
			}
			gender.Set(displayGender);
			this.behaviourType.Set(data.BehaviorType);
			Charm charm = this.charm;
			short num = data.AvatarRelatedData.AvatarData.GetCharm(data.Age, data.AvatarRelatedData.ClothingDisplayId);
			sbyte b2 = data.Gender;
			short num2 = data.Age;
			short clothingDisplayId = data.AvatarRelatedData.ClothingDisplayId;
			bool isFixedPresetType = CreatingType.IsFixedPresetType(data.CreatingType);
			AvatarData avatarData = data.AvatarRelatedData.AvatarData;
			charm.Set(num, b2, num2, clothingDisplayId, isFixedPresetType, avatarData == null || avatarData.FaceVisible);
			this.age.SetValue(data.Age.ToString());
			for (int i = 0; i < 6; i++)
			{
				this.attributes[i].SetValue(data.MainAttributes[i].ToString());
			}
		}

		// Token: 0x060066BF RID: 26303 RVA: 0x002EDBBB File Offset: 0x002EBDBB
		private void SetRemainTime(int remainTime)
		{
			this.remainTimeFrame.gameObject.SetActive(remainTime >= 0);
			this.remainTimeText.text = remainTime.ToString();
		}

		// Token: 0x060066C0 RID: 26304 RVA: 0x002EDBE9 File Offset: 0x002EBDE9
		private void SetFeatureFrame(CharacterDisplayDataForTooltip data)
		{
			this.SetFeature(data);
			this.SetTeammateCommand(data);
		}

		// Token: 0x060066C1 RID: 26305 RVA: 0x002EDBFC File Offset: 0x002EBDFC
		private void SetFeature(CharacterDisplayDataForTooltip data)
		{
			List<short> featureIds = (from x in data.FeatureIds
			where !CharacterFeature.Instance[x].Hidden
			select x).ToPoolList<short>();
			bool showMoreFeatureItem = featureIds.Count > 6;
			this.features.Rebuild<Feature>(showMoreFeatureItem ? 5 : featureIds.Count, delegate(Feature container, int i)
			{
				container.Set(featureIds[i], -1, false, -1);
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

		// Token: 0x060066C2 RID: 26306 RVA: 0x002EDD1C File Offset: 0x002EBF1C
		private void SetTeammateCommand(CharacterDisplayDataForTooltip data)
		{
			List<sbyte> list = data.TeammateCommands;
			bool flag;
			if (list == null)
			{
				flag = false;
			}
			else
			{
				flag = list.Any((sbyte x) => x >= 0);
			}
			bool flag2 = flag;
			if (flag2)
			{
				this.teammateCommands.Set(99, 99, 99, data.TeammateCommands, null);
				this.teammateCommands.gameObject.SetActive(true);
			}
			else
			{
				this.teammateCommands.gameObject.SetActive(false);
			}
		}

		// Token: 0x060066C3 RID: 26307 RVA: 0x002EDDA0 File Offset: 0x002EBFA0
		private void SetSkillFrame(CharacterDisplayDataForTooltip data)
		{
			this.SetAnySkillFrame(data, true);
			this.SetAnySkillFrame(data, false);
		}

		// Token: 0x060066C4 RID: 26308 RVA: 0x002EDDB8 File Offset: 0x002EBFB8
		private void SetAnySkillFrame(CharacterDisplayDataForTooltip data, bool isLifeSkill)
		{
			ValueTuple<int, int, sbyte>[] render = this.IterQualifications(data, isLifeSkill).ToArray<ValueTuple<int, int, sbyte>>();
			(isLifeSkill ? this.lifeTitleGrowth : this.combatTitleGrowth).text = (isLifeSkill ? LanguageKey.LK_Building_RejectRecruitPeople_Tips_Detail_1 : LanguageKey.LK_Building_RejectRecruitPeople_Tips_Detail_2).TrFormat(CommonUtils.GetAgeGrowthString((int)(isLifeSkill ? data.LifeSkillQualificationGrowthType : data.CombatSkillQualificationGrowthType), (int)data.Age)).ColorReplace();
			bool flag = render.Length == 0;
			if (flag)
			{
				(isLifeSkill ? this.lifeStupid : this.combatStupid).SetActive(true);
			}
			else
			{
				(isLifeSkill ? this.lifeStupid : this.combatStupid).SetActive(false);
				(isLifeSkill ? this.lifeSkill : this.combatSkill).Rebuild<QualificationItem>(render.Length, delegate(QualificationItem item, int i)
				{
					ValueTuple<int, int, sbyte> valueTuple = render[i];
					int type = valueTuple.Item1;
					int value = valueTuple.Item2;
					sbyte grade = valueTuple.Item3;
					item.Set(string.Format("{0}{1}", isLifeSkill ? this.lifeIconNamePrefix : this.combatIconNamePrefix, type), LanguageKey.LK_Building_RejectRecruitPeople_Tips_Detail_Content.TrFormat(isLifeSkill ? Config.LifeSkillType.Instance[type].Name : CombatSkillType.Instance[type].Name, Colors.Instance.GradeColors[(int)grade].ColorToHexString(""), value));
				});
			}
		}

		// Token: 0x060066C5 RID: 26309 RVA: 0x002EDEC8 File Offset: 0x002EC0C8
		[return: TupleElementNames(new string[]
		{
			"type",
			"value",
			"grade"
		})]
		private unsafe IEnumerable<ValueTuple<int, int, sbyte>> IterQualifications(CharacterDisplayDataForTooltip data, bool isLifeSkill)
		{
			int typeCount = isLifeSkill ? 16 : 14;
			int num;
			for (int i = 0; i < typeCount; i = num + 1)
			{
				short value = isLifeSkill ? (*data.LifeSkillQualifications[i]) : (*data.CombatSkillQualifications[i]);
				sbyte grade = Grade.GetQualificationGrade((int)value);
				bool flag = grade < 3;
				if (!flag)
				{
					yield return new ValueTuple<int, int, sbyte>(i, (int)value, grade);
				}
				num = i;
			}
			yield break;
		}

		// Token: 0x060066C6 RID: 26310 RVA: 0x002EDEE8 File Offset: 0x002EC0E8
		private void SetIllustration(CharacterDisplayDataForTooltip data)
		{
			short maxLifeSkillValue = data.LifeSkillQualifications.GetMaxLifeSkillValue();
			short maxCombatSkillValue = data.CombatSkillQualifications.GetMaxCombatSkillValue();
			string path = (maxLifeSkillValue >= maxCombatSkillValue) ? "RemakeResources/Textures/LifeSkillType_Loading/" : "RemakeResources/Textures/CombatSkillType_Loading/";
			string texName = (maxLifeSkillValue >= maxCombatSkillValue) ? Config.LifeSkillType.Instance[data.LifeSkillQualifications.GetMaxLifeSkillType()].LoadingTexture : CombatSkillType.Instance[data.CombatSkillQualifications.GetMaxCombatSkillType()].LoadingTexture;
			ResLoader.Load<Texture2D>(Path.Combine(path, texName), delegate(Texture2D tex)
			{
				this.illustration.texture = tex;
				this.illustration.SetNativeSize();
				this.illustration.enabled = true;
			}, null, false);
		}

		// Token: 0x04004817 RID: 18455
		[SerializeField]
		private CRawImage illustration;

		// Token: 0x04004818 RID: 18456
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04004819 RID: 18457
		[SerializeField]
		private TMP_Text charName;

		// Token: 0x0400481A RID: 18458
		[SerializeField]
		private TMP_Text title;

		// Token: 0x0400481B RID: 18459
		[SerializeField]
		private TMP_Text remainTimeText;

		// Token: 0x0400481C RID: 18460
		[SerializeField]
		private RectTransform remainTimeFrame;

		// Token: 0x0400481D RID: 18461
		[SerializeField]
		private Game.Components.Character.Personalities personalityType;

		// Token: 0x0400481E RID: 18462
		[SerializeField]
		private Game.Components.Character.Gender gender;

		// Token: 0x0400481F RID: 18463
		[SerializeField]
		private Charm charm;

		// Token: 0x04004820 RID: 18464
		[SerializeField]
		private Game.Components.Character.BehaviorType behaviourType;

		// Token: 0x04004821 RID: 18465
		[SerializeField]
		private PropertyItem age;

		// Token: 0x04004822 RID: 18466
		[SerializeField]
		private PropertyItem[] attributes;

		// Token: 0x04004823 RID: 18467
		[SerializeField]
		private TeammateCommands teammateCommands;

		// Token: 0x04004824 RID: 18468
		[SerializeField]
		private TemplatedContainerAssemblyNew features;

		// Token: 0x04004825 RID: 18469
		[SerializeField]
		private TMP_Text moreFeatureText;

		// Token: 0x04004826 RID: 18470
		[SerializeField]
		private GameObject moreFeatureGo;

		// Token: 0x04004827 RID: 18471
		[SerializeField]
		private GameObject[] lessFeatureGo;

		// Token: 0x04004828 RID: 18472
		[SerializeField]
		private TemplatedContainerAssemblyNew lifeSkill;

		// Token: 0x04004829 RID: 18473
		[SerializeField]
		private TemplatedContainerAssemblyNew combatSkill;

		// Token: 0x0400482A RID: 18474
		[SerializeField]
		private TMP_Text lifeTitleGrowth;

		// Token: 0x0400482B RID: 18475
		[SerializeField]
		private TMP_Text combatTitleGrowth;

		// Token: 0x0400482C RID: 18476
		[SerializeField]
		private GameObject lifeStupid;

		// Token: 0x0400482D RID: 18477
		[SerializeField]
		private GameObject combatStupid;

		// Token: 0x0400482E RID: 18478
		[SerializeField]
		private string lifeIconNamePrefix = "ui9_icon_craftsmanship_small_1_";

		// Token: 0x0400482F RID: 18479
		[SerializeField]
		private string combatIconNamePrefix = "ui9_icon_attainments_small_1_";
	}
}
