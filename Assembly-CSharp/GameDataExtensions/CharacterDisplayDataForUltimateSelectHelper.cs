using System;
using System.Collections.Generic;
using System.Text;
using Config;
using FrameWork;
using GameData.Domains.Character.Display;
using UnityEngine;

namespace GameDataExtensions
{
	// Token: 0x020006D5 RID: 1749
	public static class CharacterDisplayDataForUltimateSelectHelper
	{
		// Token: 0x06005365 RID: 21349 RVA: 0x0026A104 File Offset: 0x00268304
		public static void InitValuesCache(this CharacterDisplayDataForUltimateSelect data)
		{
			data.InitValuesCache(CharacterDisplayDataForUltimateSelectHelper.InitCacheDelegate);
		}

		// Token: 0x06005366 RID: 21350 RVA: 0x0026A114 File Offset: 0x00268314
		private unsafe static void InitCacheDict(CharacterDisplayDataForUltimateSelect data, Dictionary<int, ValueTuple<int, string>> valuesCache)
		{
			bool isTaiwu = data.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			string name = NameCenter.GetCharMonasticTitleAndNameByNameRelatedData(ref data.NameRelatedData, isTaiwu, false);
			valuesCache.Add(0, new ValueTuple<int, string>(0, name));
			valuesCache.Add(2, new ValueTuple<int, string>((int)data.PhysiologicalAge, data.PhysiologicalAge.ToString()));
			valuesCache.Add(3, new ValueTuple<int, string>((int)data.LeftMaxHealth, CommonUtils.GetCharacterHealthInfo(data.Health, data.LeftMaxHealth, -1).Item1));
			valuesCache.Add(19, new ValueTuple<int, string>((int)data.DefeatMarkCount, data.DefeatMarkCount.ToString()));
			valuesCache.Add(4, new ValueTuple<int, string>((int)data.Gender, CommonUtils.GetGenderString(CommonUtils.GetDisplayGender(data.Gender, data.NameRelatedData.CharTemplateId))));
			data.AvatarRelatedData.AvatarData.ClothDisplayId = data.AvatarRelatedData.ClothingDisplayId;
			string charmValue = CommonUtils.GetCharmLevelText(data.Charm, data.Gender, data.AvatarRelatedData.DisplayAge, data.AvatarRelatedData.ClothingDisplayId, false, data.AvatarRelatedData.AvatarData.FaceVisible);
			valuesCache.Add(201, new ValueTuple<int, string>((int)data.Charm, charmValue));
			valuesCache.Add(5, new ValueTuple<int, string>((int)data.BehaviorType, CommonUtils.GetBehaviorString(data.BehaviorType)));
			valuesCache.Add(6, new ValueTuple<int, string>((int)data.HappinessType, CommonUtils.GetHappinessString(data.HappinessType)));
			valuesCache.Add(7, new ValueTuple<int, string>((int)data.FavorabilityToTaiwu, CommonUtils.GetFavorStringByInteracted(data.FavorabilityToTaiwu, data.IsInteractedWithTaiwu)));
			valuesCache.Add(21, new ValueTuple<int, string>((int)data.PreexistenceCharCount, data.PreexistenceCharCount.ToString()));
			valuesCache.Add(8, new ValueTuple<int, string>((int)data.FameType, CommonUtils.GetFameString(data.FameType)));
			WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
			string organization = SingletonObject.getInstance<WorldMapModel>().GetSettlementName(data.OrganizationInfo);
			valuesCache.Add(24, new ValueTuple<int, string>(-1, organization));
			int consummateLevelGrade = Math.Clamp((data.ConsummateLevel - 1) / 2, 0, 8);
			Color consummateLevelColor = Colors.Instance.GradeColors[consummateLevelGrade];
			valuesCache.Add(34, new ValueTuple<int, string>(data.ConsummateLevel, data.ConsummateLevel.ToString().SetColor(consummateLevelColor)));
			string identityString = CommonUtils.GetIdentityString(data.OrganizationInfo, data.Gender, data.PhysiologicalAge, false);
			valuesCache.Add(1, new ValueTuple<int, string>((int)data.OrganizationInfo.Grade, identityString));
			string positionString = SingletonObject.getInstance<WorldMapModel>().GetFullBlockName(data.LocationNameData, false, true, true, false);
			valuesCache.Add(25, new ValueTuple<int, string>(-1, positionString));
			valuesCache.Add(11, new ValueTuple<int, string>(data.AttackMedal, (data.AttackMedal == 0) ? "-" : Mathf.Abs(data.AttackMedal).ToString()));
			valuesCache.Add(12, new ValueTuple<int, string>(data.DefenceMedal, (data.DefenceMedal == 0) ? "-" : Mathf.Abs(data.DefenceMedal).ToString()));
			valuesCache.Add(13, new ValueTuple<int, string>(data.WisdomMedal, (data.WisdomMedal == 0) ? "-" : Mathf.Abs(data.WisdomMedal).ToString()));
			int strength = (int)data.MaxMainAttributes.Items.FixedElementField;
			valuesCache.Add(100, new ValueTuple<int, string>(strength, strength.ToString()));
			int dexterity = (int)(*(ref data.MaxMainAttributes.Items.FixedElementField + 2));
			valuesCache.Add(101, new ValueTuple<int, string>(dexterity, dexterity.ToString()));
			int concentration = (int)(*(ref data.MaxMainAttributes.Items.FixedElementField + (IntPtr)2 * 2));
			valuesCache.Add(102, new ValueTuple<int, string>(concentration, concentration.ToString()));
			int vitality = (int)(*(ref data.MaxMainAttributes.Items.FixedElementField + (IntPtr)3 * 2));
			valuesCache.Add(103, new ValueTuple<int, string>(vitality, vitality.ToString()));
			int energy = (int)(*(ref data.MaxMainAttributes.Items.FixedElementField + (IntPtr)4 * 2));
			valuesCache.Add(104, new ValueTuple<int, string>(energy, energy.ToString()));
			int intelligence = (int)(*(ref data.MaxMainAttributes.Items.FixedElementField + (IntPtr)5 * 2));
			valuesCache.Add(105, new ValueTuple<int, string>(intelligence, intelligence.ToString()));
			int penetrationsOuter = data.Penetrations.Outer;
			valuesCache.Add(110, new ValueTuple<int, string>(penetrationsOuter, penetrationsOuter.ToString()));
			int penetrationsInner = data.Penetrations.Inner;
			valuesCache.Add(111, new ValueTuple<int, string>(penetrationsInner, penetrationsInner.ToString()));
			int penetrationResistsOuter = data.PenetrationResists.Outer;
			valuesCache.Add(116, new ValueTuple<int, string>(penetrationResistsOuter, penetrationResistsOuter.ToString()));
			int penetrationResistsInner = data.PenetrationResists.Inner;
			valuesCache.Add(117, new ValueTuple<int, string>(penetrationResistsInner, penetrationResistsInner.ToString()));
			int hitStrength = data.HitValues.Items.FixedElementField;
			valuesCache.Add(106, new ValueTuple<int, string>(hitStrength, hitStrength.ToString()));
			int hitTechnique = *(ref data.HitValues.Items.FixedElementField + 4);
			valuesCache.Add(107, new ValueTuple<int, string>(hitTechnique, hitTechnique.ToString()));
			int hitSpeed = *(ref data.HitValues.Items.FixedElementField + (IntPtr)2 * 4);
			valuesCache.Add(108, new ValueTuple<int, string>(hitSpeed, hitSpeed.ToString()));
			int hitMind = *(ref data.HitValues.Items.FixedElementField + (IntPtr)3 * 4);
			valuesCache.Add(109, new ValueTuple<int, string>(hitMind, hitMind.ToString()));
			int hitAvoidStrength = data.AvoidValues.Items.FixedElementField;
			valuesCache.Add(112, new ValueTuple<int, string>(hitAvoidStrength, hitAvoidStrength.ToString()));
			int hitAvoidTechnique = *(ref data.AvoidValues.Items.FixedElementField + 4);
			valuesCache.Add(113, new ValueTuple<int, string>(hitAvoidTechnique, hitAvoidTechnique.ToString()));
			int hitAvoidSpeed = *(ref data.AvoidValues.Items.FixedElementField + (IntPtr)2 * 4);
			valuesCache.Add(114, new ValueTuple<int, string>(hitAvoidSpeed, hitAvoidSpeed.ToString()));
			int hitAvoidMind = *(ref data.AvoidValues.Items.FixedElementField + (IntPtr)3 * 4);
			valuesCache.Add(115, new ValueTuple<int, string>(hitAvoidMind, hitAvoidMind.ToString()));
			valuesCache.Add(20, new ValueTuple<int, string>((int)data.DisorderOfQi, data.DisorderOfQi.ToString()));
			int lifeSkillValue = (int)data.LifeSkillQualifications.Items.FixedElementField;
			valuesCache.Add(134, new ValueTuple<int, string>(lifeSkillValue, lifeSkillValue.ToString()));
			lifeSkillValue = (int)(*(ref data.LifeSkillQualifications.Items.FixedElementField + 2));
			valuesCache.Add(135, new ValueTuple<int, string>(lifeSkillValue, lifeSkillValue.ToString()));
			lifeSkillValue = (int)(*(ref data.LifeSkillQualifications.Items.FixedElementField + (IntPtr)2 * 2));
			valuesCache.Add(136, new ValueTuple<int, string>(lifeSkillValue, lifeSkillValue.ToString()));
			lifeSkillValue = (int)(*(ref data.LifeSkillQualifications.Items.FixedElementField + (IntPtr)3 * 2));
			valuesCache.Add(137, new ValueTuple<int, string>(lifeSkillValue, lifeSkillValue.ToString()));
			lifeSkillValue = (int)(*(ref data.LifeSkillQualifications.Items.FixedElementField + (IntPtr)4 * 2));
			valuesCache.Add(138, new ValueTuple<int, string>(lifeSkillValue, lifeSkillValue.ToString()));
			lifeSkillValue = (int)(*(ref data.LifeSkillQualifications.Items.FixedElementField + (IntPtr)5 * 2));
			valuesCache.Add(139, new ValueTuple<int, string>(lifeSkillValue, lifeSkillValue.ToString()));
			lifeSkillValue = (int)(*(ref data.LifeSkillQualifications.Items.FixedElementField + (IntPtr)6 * 2));
			valuesCache.Add(140, new ValueTuple<int, string>(lifeSkillValue, lifeSkillValue.ToString()));
			lifeSkillValue = (int)(*(ref data.LifeSkillQualifications.Items.FixedElementField + (IntPtr)7 * 2));
			valuesCache.Add(141, new ValueTuple<int, string>(lifeSkillValue, lifeSkillValue.ToString()));
			lifeSkillValue = (int)(*(ref data.LifeSkillQualifications.Items.FixedElementField + (IntPtr)8 * 2));
			valuesCache.Add(142, new ValueTuple<int, string>(lifeSkillValue, lifeSkillValue.ToString()));
			lifeSkillValue = (int)(*(ref data.LifeSkillQualifications.Items.FixedElementField + (IntPtr)9 * 2));
			valuesCache.Add(143, new ValueTuple<int, string>(lifeSkillValue, lifeSkillValue.ToString()));
			lifeSkillValue = (int)(*(ref data.LifeSkillQualifications.Items.FixedElementField + (IntPtr)10 * 2));
			valuesCache.Add(144, new ValueTuple<int, string>(lifeSkillValue, lifeSkillValue.ToString()));
			lifeSkillValue = (int)(*(ref data.LifeSkillQualifications.Items.FixedElementField + (IntPtr)11 * 2));
			valuesCache.Add(145, new ValueTuple<int, string>(lifeSkillValue, lifeSkillValue.ToString()));
			lifeSkillValue = (int)(*(ref data.LifeSkillQualifications.Items.FixedElementField + (IntPtr)12 * 2));
			valuesCache.Add(146, new ValueTuple<int, string>(lifeSkillValue, lifeSkillValue.ToString()));
			lifeSkillValue = (int)(*(ref data.LifeSkillQualifications.Items.FixedElementField + (IntPtr)13 * 2));
			valuesCache.Add(147, new ValueTuple<int, string>(lifeSkillValue, lifeSkillValue.ToString()));
			lifeSkillValue = (int)(*(ref data.LifeSkillQualifications.Items.FixedElementField + (IntPtr)14 * 2));
			valuesCache.Add(148, new ValueTuple<int, string>(lifeSkillValue, lifeSkillValue.ToString()));
			lifeSkillValue = (int)(*(ref data.LifeSkillQualifications.Items.FixedElementField + (IntPtr)15 * 2));
			valuesCache.Add(149, new ValueTuple<int, string>(lifeSkillValue, lifeSkillValue.ToString()));
			valuesCache.Add(18, new ValueTuple<int, string>((int)data.GetSkillGrowthAddValue((int)data.LifeSkillGrowthType), data.GetSkillGrowthString((int)data.LifeSkillGrowthType)));
			int combatSkillValue = (int)data.CombatSkillQualifications.Items.FixedElementField;
			valuesCache.Add(166, new ValueTuple<int, string>(combatSkillValue, combatSkillValue.ToString()));
			combatSkillValue = (int)(*(ref data.CombatSkillQualifications.Items.FixedElementField + 2));
			valuesCache.Add(167, new ValueTuple<int, string>(combatSkillValue, combatSkillValue.ToString()));
			combatSkillValue = (int)(*(ref data.CombatSkillQualifications.Items.FixedElementField + (IntPtr)2 * 2));
			valuesCache.Add(168, new ValueTuple<int, string>(combatSkillValue, combatSkillValue.ToString()));
			combatSkillValue = (int)(*(ref data.CombatSkillQualifications.Items.FixedElementField + (IntPtr)3 * 2));
			valuesCache.Add(169, new ValueTuple<int, string>(combatSkillValue, combatSkillValue.ToString()));
			combatSkillValue = (int)(*(ref data.CombatSkillQualifications.Items.FixedElementField + (IntPtr)4 * 2));
			valuesCache.Add(170, new ValueTuple<int, string>(combatSkillValue, combatSkillValue.ToString()));
			combatSkillValue = (int)(*(ref data.CombatSkillQualifications.Items.FixedElementField + (IntPtr)5 * 2));
			valuesCache.Add(171, new ValueTuple<int, string>(combatSkillValue, combatSkillValue.ToString()));
			combatSkillValue = (int)(*(ref data.CombatSkillQualifications.Items.FixedElementField + (IntPtr)6 * 2));
			valuesCache.Add(172, new ValueTuple<int, string>(combatSkillValue, combatSkillValue.ToString()));
			combatSkillValue = (int)(*(ref data.CombatSkillQualifications.Items.FixedElementField + (IntPtr)7 * 2));
			valuesCache.Add(173, new ValueTuple<int, string>(combatSkillValue, combatSkillValue.ToString()));
			combatSkillValue = (int)(*(ref data.CombatSkillQualifications.Items.FixedElementField + (IntPtr)8 * 2));
			valuesCache.Add(174, new ValueTuple<int, string>(combatSkillValue, combatSkillValue.ToString()));
			combatSkillValue = (int)(*(ref data.CombatSkillQualifications.Items.FixedElementField + (IntPtr)9 * 2));
			valuesCache.Add(175, new ValueTuple<int, string>(combatSkillValue, combatSkillValue.ToString()));
			combatSkillValue = (int)(*(ref data.CombatSkillQualifications.Items.FixedElementField + (IntPtr)10 * 2));
			valuesCache.Add(176, new ValueTuple<int, string>(combatSkillValue, combatSkillValue.ToString()));
			combatSkillValue = (int)(*(ref data.CombatSkillQualifications.Items.FixedElementField + (IntPtr)11 * 2));
			valuesCache.Add(177, new ValueTuple<int, string>(combatSkillValue, combatSkillValue.ToString()));
			combatSkillValue = (int)(*(ref data.CombatSkillQualifications.Items.FixedElementField + (IntPtr)12 * 2));
			valuesCache.Add(178, new ValueTuple<int, string>(combatSkillValue, combatSkillValue.ToString()));
			combatSkillValue = (int)(*(ref data.CombatSkillQualifications.Items.FixedElementField + (IntPtr)13 * 2));
			valuesCache.Add(179, new ValueTuple<int, string>(combatSkillValue, combatSkillValue.ToString()));
			valuesCache.Add(17, new ValueTuple<int, string>((int)data.GetSkillGrowthAddValue((int)data.CombatSkillGrowthType), data.GetSkillGrowthString((int)data.CombatSkillGrowthType)));
			int personalityValue = (int)data.Personalities.Items.FixedElementField;
			valuesCache.Add(194, new ValueTuple<int, string>(personalityValue, personalityValue.ToString()));
			personalityValue = (int)(*(ref data.Personalities.Items.FixedElementField + 1));
			valuesCache.Add(195, new ValueTuple<int, string>(personalityValue, personalityValue.ToString()));
			personalityValue = (int)(*(ref data.Personalities.Items.FixedElementField + 2));
			valuesCache.Add(196, new ValueTuple<int, string>(personalityValue, personalityValue.ToString()));
			personalityValue = (int)(*(ref data.Personalities.Items.FixedElementField + 3));
			valuesCache.Add(197, new ValueTuple<int, string>(personalityValue, personalityValue.ToString()));
			personalityValue = (int)(*(ref data.Personalities.Items.FixedElementField + 4));
			valuesCache.Add(198, new ValueTuple<int, string>(personalityValue, personalityValue.ToString()));
			personalityValue = (int)(*(ref data.Personalities.Items.FixedElementField + 5));
			valuesCache.Add(199, new ValueTuple<int, string>(personalityValue, personalityValue.ToString()));
			personalityValue = (int)(*(ref data.Personalities.Items.FixedElementField + 6));
			valuesCache.Add(200, new ValueTuple<int, string>(personalityValue, personalityValue.ToString()));
			int resourceValue = data.Resources.Items.FixedElementField;
			valuesCache.Add(26, new ValueTuple<int, string>(resourceValue, resourceValue.ToString()));
			resourceValue = *(ref data.Resources.Items.FixedElementField + 4);
			valuesCache.Add(27, new ValueTuple<int, string>(resourceValue, resourceValue.ToString()));
			resourceValue = *(ref data.Resources.Items.FixedElementField + (IntPtr)2 * 4);
			valuesCache.Add(28, new ValueTuple<int, string>(resourceValue, resourceValue.ToString()));
			resourceValue = *(ref data.Resources.Items.FixedElementField + (IntPtr)3 * 4);
			valuesCache.Add(29, new ValueTuple<int, string>(resourceValue, resourceValue.ToString()));
			resourceValue = *(ref data.Resources.Items.FixedElementField + (IntPtr)4 * 4);
			valuesCache.Add(30, new ValueTuple<int, string>(resourceValue, resourceValue.ToString()));
			resourceValue = *(ref data.Resources.Items.FixedElementField + (IntPtr)5 * 4);
			valuesCache.Add(31, new ValueTuple<int, string>(resourceValue, resourceValue.ToString()));
			resourceValue = *(ref data.Resources.Items.FixedElementField + (IntPtr)6 * 4);
			valuesCache.Add(32, new ValueTuple<int, string>(resourceValue, resourceValue.ToString()));
			resourceValue = *(ref data.Resources.Items.FixedElementField + (IntPtr)7 * 4);
			valuesCache.Add(33, new ValueTuple<int, string>(resourceValue, resourceValue.ToString()));
			string currLoadStr = ((float)data.CurrInventoryLoad / 100f).ToString("f1").SetColor(CommonUtils.GetLoadWeightValueColor(data.CurrInventoryLoad, data.MaxInventoryLoad));
			valuesCache.Add(22, new ValueTuple<int, string>(data.CurrInventoryLoad, string.Format("{0}/{1:f1}", currLoadStr, (float)data.MaxInventoryLoad / 100f)));
			valuesCache.Add(23, new ValueTuple<int, string>((int)data.KidnapCount, data.KidnapCount.ToString()));
			valuesCache.Add(38, new ValueTuple<int, string>((int)data.VillagerNeedWaitTime, data.VillagerNeedWaitTime.ToString()));
		}

		// Token: 0x06005367 RID: 21351 RVA: 0x0026B10C File Offset: 0x0026930C
		private static string GetSkillGrowthString(this CharacterDisplayDataForUltimateSelect data, int growthType)
		{
			int addValue = (int)data.GetSkillGrowthAddValue(growthType);
			StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
			strBuilder.Clear();
			strBuilder.Append((growthType == 0) ? LocalStringManager.Get("LK_Qualification_Growth_Average") : ((growthType == 1) ? LocalStringManager.Get("LK_Qualification_Growth_Precocious") : LocalStringManager.Get("LK_Qualification_Growth_LateBlooming")));
			bool flag = addValue > 0;
			if (flag)
			{
				strBuilder.Append(string.Format("+{0}", addValue).SetColor("lightblue"));
			}
			else
			{
				bool flag2 = addValue == 0;
				if (flag2)
				{
					strBuilder.Append("+0".SetColor("lightgrey"));
				}
				else
				{
					strBuilder.Append(string.Format("{0}", addValue).SetColor("red"));
				}
			}
			return strBuilder.ToString();
		}

		// Token: 0x06005368 RID: 21352 RVA: 0x0026B1D8 File Offset: 0x002693D8
		public static sbyte GetSkillGrowthAddValue(this CharacterDisplayDataForUltimateSelect data, int growthType)
		{
			AgeEffectItem ageData = AgeEffect.Instance[Mathf.Min((int)data.PhysiologicalAge, AgeEffect.Instance.Count - 1)];
			return (growthType == 0) ? ageData.SkillQualificationAverage : ((growthType == 1) ? ageData.SkillQualificationPrecocious : ageData.SkillQualificationLateBlooming);
		}

		// Token: 0x04003875 RID: 14453
		private static readonly Action<CharacterDisplayDataForUltimateSelect, Dictionary<int, ValueTuple<int, string>>> InitCacheDelegate = new Action<CharacterDisplayDataForUltimateSelect, Dictionary<int, ValueTuple<int, string>>>(CharacterDisplayDataForUltimateSelectHelper.InitCacheDict);
	}
}
