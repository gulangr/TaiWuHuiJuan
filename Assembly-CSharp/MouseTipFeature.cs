using System;
using System.Collections.Generic;
using System.Text;
using Config;
using FrameWork;
using Game.Views.Encyclopedia;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x02000299 RID: 665
public class MouseTipFeature : MouseTipBase
{
	// Token: 0x17000496 RID: 1174
	// (get) Token: 0x06002A0D RID: 10765 RVA: 0x0013F287 File Offset: 0x0013D487
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002A0E RID: 10766 RVA: 0x0013F28C File Offset: 0x0013D48C
	protected override void Init(ArgumentBox argsBox)
	{
		MouseTipFeature.<>c__DisplayClass8_0 CS$<>8__locals1 = new MouseTipFeature.<>c__DisplayClass8_0();
		CS$<>8__locals1.<>4__this = this;
		this.Element.ForceListenCommand = true;
		argsBox.Get("FeatureId", out CS$<>8__locals1.featureId);
		argsBox.Get("CharacterId", out CS$<>8__locals1.characterId);
		argsBox.Get("TemplateDataOnly", out this._templateDataOnly);
		CS$<>8__locals1.configData = CharacterFeature.Instance[CS$<>8__locals1.featureId];
		bool isWide = MouseTipFeature.WideTipsFeature.Contains(CS$<>8__locals1.featureId);
		base.GetComponent<CImage>().SetSprite(isWide ? "mousetip_di_1" : "mousetip_di_3", false, null);
		bool flag = isWide;
		if (flag)
		{
			base.GetComponent<RectTransform>().SetWidth((float)(this.wideBaseWidth + 2));
		}
		else
		{
			base.GetComponent<RectTransform>().SetWidth((float)(this.normalBaseWidth + 2));
		}
		bool isInGame = GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame;
		bool showSmallVillageName = !this._templateDataOnly && isInGame && GameData.Domains.World.SharedMethods.SmallVillageXiangshuProgress() && (CS$<>8__locals1.configData.TemplateId == 210 || CS$<>8__locals1.configData.TemplateId == 211);
		TextMeshProUGUI nameText = base.CGet<TextMeshProUGUI>("Name");
		nameText.text = (showSmallVillageName ? CS$<>8__locals1.configData.SmallVillageName : CS$<>8__locals1.configData.Name);
		TextMeshProUGUI descText = base.CGet<TextMeshProUGUI>("Desc");
		descText.text = (showSmallVillageName ? CS$<>8__locals1.configData.SmallVillageDesc : CS$<>8__locals1.configData.Desc);
		this.remainMonth.gameObject.SetActive(CS$<>8__locals1.configData.Type == ECharacterFeatureType.Temporary && this._templateDataOnly);
		this.remainMonth.text = LanguageKey.LK_MouseTip_FeatureDuration.TrFormat((CS$<>8__locals1.configData.Duration > 0) ? CS$<>8__locals1.configData.Duration.ToString() : LanguageKey.LK_MouseTip_FeatureDuration_Unknown.Tr());
		CS$<>8__locals1.hasEffectChange = false;
		bool qiDisorderDebuff = CS$<>8__locals1.configData.QiDisorderDebuffPercent != 0;
		base.CGet<GameObject>("QiDisorderDebuff").SetActive(qiDisorderDebuff);
		bool flag2 = qiDisorderDebuff;
		if (flag2)
		{
			string valueStr = CS$<>8__locals1.configData.QiDisorderDebuffPercent.ToString() + "%";
			bool flag3 = CS$<>8__locals1.configData.QiDisorderDebuffPercent > 0;
			if (flag3)
			{
				valueStr = "+" + valueStr;
			}
			valueStr = valueStr.SetColor((CS$<>8__locals1.configData.QiDisorderDebuffPercent > 0) ? "brightred" : "brightblue");
			base.CGet<TextMeshProUGUI>("QiDisorderDebuffValue").text = valueStr;
			CS$<>8__locals1.hasEffectChange = true;
		}
		bool qiDisorderBuff = CS$<>8__locals1.configData.QiDisorderBuffPercent != 0;
		base.CGet<GameObject>("QiDisorderBuff").SetActive(qiDisorderBuff);
		bool flag4 = qiDisorderBuff;
		if (flag4)
		{
			string valueStr2 = CS$<>8__locals1.configData.QiDisorderBuffPercent.ToString() + "%";
			bool flag5 = CS$<>8__locals1.configData.QiDisorderBuffPercent > 0;
			if (flag5)
			{
				valueStr2 = "+" + valueStr2;
			}
			valueStr2 = valueStr2.SetColor((CS$<>8__locals1.configData.QiDisorderBuffPercent > 0) ? "brightblue" : "brightred");
			base.CGet<TextMeshProUGUI>("QiDisorderBuffValue").text = valueStr2;
			CS$<>8__locals1.hasEffectChange = true;
		}
		base.CGet<GameObject>("MaxHealth").SetActive(CS$<>8__locals1.configData.MaxHealthPercentBonus != 0);
		bool flag6 = CS$<>8__locals1.configData.MaxHealthPercentBonus != 0;
		if (flag6)
		{
			base.CGet<TextMeshProUGUI>("AddMaxHealth").text = ((CS$<>8__locals1.configData.MaxHealthPercentBonus > 0) ? string.Format("+{0}%", CS$<>8__locals1.configData.MaxHealthPercentBonus) : "");
			base.CGet<TextMeshProUGUI>("ReduceMaxHealth").text = ((CS$<>8__locals1.configData.MaxHealthPercentBonus > 0) ? "" : string.Format("{0}%", CS$<>8__locals1.configData.MaxHealthPercentBonus));
			CS$<>8__locals1.hasEffectChange = true;
		}
		base.CGet<GameObject>("FavorUp").SetActive(CS$<>8__locals1.configData.FavorabilityIncrementFactor != 100);
		bool flag7 = CS$<>8__locals1.configData.FavorabilityIncrementFactor != 100;
		if (flag7)
		{
			base.CGet<TextMeshProUGUI>("AddFavorUp").text = ((CS$<>8__locals1.configData.FavorabilityIncrementFactor > 100) ? string.Format("+{0}%", (int)(CS$<>8__locals1.configData.FavorabilityIncrementFactor - 100)) : "");
			base.CGet<TextMeshProUGUI>("ReduceFavorUp").text = ((CS$<>8__locals1.configData.FavorabilityIncrementFactor > 100) ? "" : string.Format("{0}%", (int)(CS$<>8__locals1.configData.FavorabilityIncrementFactor - 100)));
			CS$<>8__locals1.hasEffectChange = true;
		}
		base.CGet<GameObject>("FavorDown").SetActive(CS$<>8__locals1.configData.FavorabilityDecrementFactor != 100);
		bool flag8 = CS$<>8__locals1.configData.FavorabilityDecrementFactor != 100;
		if (flag8)
		{
			base.CGet<TextMeshProUGUI>("AddFavorDown").text = ((CS$<>8__locals1.configData.FavorabilityDecrementFactor > 100) ? string.Format("+{0}%", (int)(CS$<>8__locals1.configData.FavorabilityDecrementFactor - 100)) : "");
			base.CGet<TextMeshProUGUI>("ReduceFavorDown").text = ((CS$<>8__locals1.configData.FavorabilityDecrementFactor > 100) ? "" : string.Format("{0}%", (int)(CS$<>8__locals1.configData.FavorabilityDecrementFactor - 100)));
			CS$<>8__locals1.hasEffectChange = true;
		}
		RectTransform skillPowerHolder = base.CGet<RectTransform>("SkillPowerUpHolder");
		bool hasSkillPowerUp = CS$<>8__locals1.configData.CombatSkillPowerBonuses.Exist((short value) => Mathf.Abs((int)value) != 0);
		skillPowerHolder.gameObject.SetActive(hasSkillPowerUp);
		bool flag9 = hasSkillPowerUp;
		if (flag9)
		{
			for (sbyte i = 0; i < 5; i += 1)
			{
				Refers targetRefers = skillPowerHolder.Find(string.Format("SkillPowerUp_{0}", i)).GetComponent<Refers>();
				short value2 = CS$<>8__locals1.configData.CombatSkillPowerBonuses[(int)i];
				bool showFlag = value2 != 0;
				targetRefers.gameObject.SetActive(showFlag);
				bool flag10 = showFlag;
				if (flag10)
				{
					string skillType = LocalStringManager.Get(MouseTipFeature.SkillPowerUpTypeIds[(int)i]);
					string valueString = value2.ToString();
					bool flag11 = value2 > 0;
					if (flag11)
					{
						valueString = string.Format("+{0}", value2);
					}
					string descContent = LocalStringManager.GetFormat(LanguageKey.LK_Common_PowerUpPercent, valueString, (value2 > 0) ? "brightblue" : "brightred");
					targetRefers.CGet<TextMeshProUGUI>("Desc").text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_Feature_Power_Value_Format, skillType, descContent.ColorReplace());
				}
			}
			CS$<>8__locals1.hasEffectChange = true;
		}
		RectTransform skillSlotHolder = base.CGet<RectTransform>("AddSkillSlotHolder");
		bool hasSkillSlotBonus = CS$<>8__locals1.configData.CombatSkillSlotBonuses.Exist((sbyte value) => value != 0);
		skillSlotHolder.gameObject.SetActive(hasSkillSlotBonus);
		bool flag12 = hasSkillSlotBonus;
		if (flag12)
		{
			for (sbyte type = 0; type < 5; type += 1)
			{
				sbyte slotCount = CS$<>8__locals1.configData.CombatSkillSlotBonuses[(int)type];
				Refers slotRefers = skillSlotHolder.GetChild((int)type).GetComponent<Refers>();
				slotRefers.gameObject.SetActive(slotCount != 0);
				bool flag13 = slotCount == 0;
				if (!flag13)
				{
					slotRefers.CGet<TextMeshProUGUI>("AddSkillSlot").text = ((slotCount > 0) ? string.Format("+{0}", slotCount) : "");
					slotRefers.CGet<TextMeshProUGUI>("ReduceSkillSlot").text = ((slotCount > 0) ? "" : slotCount.ToString());
				}
			}
			CS$<>8__locals1.hasEffectChange = true;
		}
		GameObject silenceHolder = base.CGet<GameObject>("SilenceHolder");
		silenceHolder.SetActive(CS$<>8__locals1.configData.SilenceFramePercent != 0);
		bool activeSelf = silenceHolder.activeSelf;
		if (activeSelf)
		{
			base.CGet<TextMeshProUGUI>("SilenceTitle").text = CombatSkillProperty.Instance[71].Name;
			bool flag14 = CS$<>8__locals1.configData.SilenceFramePercent > 0;
			if (flag14)
			{
				base.CGet<TextMeshProUGUI>("DebuffSilence").text = string.Format("+{0}%", CS$<>8__locals1.configData.SilenceFramePercent);
				base.CGet<TextMeshProUGUI>("BuffSilence").text = string.Empty;
			}
			else
			{
				base.CGet<TextMeshProUGUI>("DebuffSilence").text = string.Empty;
				base.CGet<TextMeshProUGUI>("BuffSilence").text = string.Format("{0}%", CS$<>8__locals1.configData.SilenceFramePercent);
			}
			CS$<>8__locals1.hasEffectChange = true;
		}
		base.CGet<GameObject>("AdoreHolder").SetActive(CS$<>8__locals1.configData.AdoreMultiplePeopleChanceFactor != 100);
		bool up = CS$<>8__locals1.configData.AdoreMultiplePeopleChanceFactor < 100;
		string color = up ? "brightred" : "brightblue";
		string content = up ? string.Format("+{0}", CS$<>8__locals1.configData.AdoreMultiplePeopleChanceFactor) : string.Format("-{0}", CS$<>8__locals1.configData.AdoreMultiplePeopleChanceFactor);
		base.CGet<TextMeshProUGUI>("AdoreFactor").SetText(content.SetColor(color), true);
		CS$<>8__locals1.effectDescRoot = base.CGet<Refers>("SpecialEffect");
		bool hasEffect = !CS$<>8__locals1.configData.EffectDesc.IsNullOrEmpty();
		bool flag15 = hasEffect;
		if (flag15)
		{
			CS$<>8__locals1.effectDescRoot.CGet<TextMeshProUGUI>("Desc").text = ((CS$<>8__locals1.configData.TemplateId == 738) ? CS$<>8__locals1.configData.EffectDesc.GetFormat(CS$<>8__locals1.configData.Duration.ToString()).ColorReplace() : CS$<>8__locals1.configData.EffectDesc.ColorReplace());
		}
		bool showEffect = hasEffect || CS$<>8__locals1.configData.IgnoreHealthMark;
		CS$<>8__locals1.effectDescRoot.gameObject.SetActive(showEffect);
		CS$<>8__locals1.effectDescRoot.CGet<GameObject>("IgnoreHealth").SetActive(CS$<>8__locals1.configData.IgnoreHealthMark);
		CS$<>8__locals1.effectDescRoot.CGet<GameObject>("DescHolder").SetActive(hasEffect);
		RectTransform addPropertyHolder = base.CGet<RectTransform>("AddPropertyHolder");
		TipsAddProperty addPropertyPrefab = base.CGet<TipsAddProperty>("AddProperty");
		int index = 0;
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 94, (int)CS$<>8__locals1.configData.PersonalityCalm, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 95, (int)CS$<>8__locals1.configData.PersonalityClever, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 96, (int)CS$<>8__locals1.configData.PersonalityEnthusiastic, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 97, (int)CS$<>8__locals1.configData.PersonalityBrave, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 98, (int)CS$<>8__locals1.configData.PersonalityFirm, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 99, (int)CS$<>8__locals1.configData.PersonalityLucky, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 100, (int)CS$<>8__locals1.configData.PersonalityPerceptive, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 0, (int)CS$<>8__locals1.configData.Strength, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 1, (int)CS$<>8__locals1.configData.Dexterity, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 2, (int)CS$<>8__locals1.configData.Concentration, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 3, (int)CS$<>8__locals1.configData.Vitality, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 4, (int)CS$<>8__locals1.configData.Energy, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 5, (int)CS$<>8__locals1.configData.Intelligence, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 103, (int)CS$<>8__locals1.configData.HobbyChangingPeriod, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 101, (int)CS$<>8__locals1.configData.Attraction, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 6, (int)CS$<>8__locals1.configData.HitRateStrength, false, true, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 7, (int)CS$<>8__locals1.configData.HitRateTechnique, false, true, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 8, (int)CS$<>8__locals1.configData.HitRateSpeed, false, true, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 9, (int)CS$<>8__locals1.configData.HitRateMind, false, true, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 10, (int)CS$<>8__locals1.configData.PenetrateOfOuter, false, true, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 11, (int)CS$<>8__locals1.configData.PenetrateOfInner, false, true, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 12, CS$<>8__locals1.configData.AvoidRateStrength, false, true, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 13, CS$<>8__locals1.configData.AvoidRateTechnique, false, true, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 14, CS$<>8__locals1.configData.AvoidRateSpeed, false, true, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 15, CS$<>8__locals1.configData.AvoidRateMind, false, true, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 16, CS$<>8__locals1.configData.PenetrateResistOfOuter, false, true, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 17, CS$<>8__locals1.configData.PenetrateResistOfInner, false, true, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 18, (int)CS$<>8__locals1.configData.RecoveryOfStance, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 19, (int)CS$<>8__locals1.configData.RecoveryOfBreath, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 20, (int)CS$<>8__locals1.configData.MoveSpeed, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 21, (int)CS$<>8__locals1.configData.RecoveryOfFlaw, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 22, (int)CS$<>8__locals1.configData.CastSpeed, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 23, (int)CS$<>8__locals1.configData.RecoveryOfBlockedAcupoint, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 24, (int)CS$<>8__locals1.configData.WeaponSwitchSpeed, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 25, (int)CS$<>8__locals1.configData.AttackSpeed, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 26, (int)CS$<>8__locals1.configData.InnerRatio, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 27, (int)CS$<>8__locals1.configData.RecoveryOfQiDisorder, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddPropertyWithConfig(addPropertyHolder, addPropertyPrefab, index, 28, CS$<>8__locals1.configData.ResistOfHotPoison, false, false, true, false);
		index += MouseTip_Util.AppendAddPropertyWithConfig(addPropertyHolder, addPropertyPrefab, index, 29, CS$<>8__locals1.configData.ResistOfGloomyPoison, false, false, true, false);
		index += MouseTip_Util.AppendAddPropertyWithConfig(addPropertyHolder, addPropertyPrefab, index, 31, CS$<>8__locals1.configData.ResistOfRedPoison, false, false, true, false);
		index += MouseTip_Util.AppendAddPropertyWithConfig(addPropertyHolder, addPropertyPrefab, index, 30, CS$<>8__locals1.configData.ResistOfColdPoison, false, false, true, false);
		index += MouseTip_Util.AppendAddPropertyWithConfig(addPropertyHolder, addPropertyPrefab, index, 32, CS$<>8__locals1.configData.ResistOfRottenPoison, false, false, true, false);
		index += MouseTip_Util.AppendAddPropertyWithConfig(addPropertyHolder, addPropertyPrefab, index, 33, CS$<>8__locals1.configData.ResistOfIllusoryPoison, false, false, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 34, (int)CS$<>8__locals1.configData.QualificationMusic, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 35, (int)CS$<>8__locals1.configData.QualificationChess, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 36, (int)CS$<>8__locals1.configData.QualificationPoem, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 37, (int)CS$<>8__locals1.configData.QualificationPainting, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 38, (int)CS$<>8__locals1.configData.QualificationMath, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 39, (int)CS$<>8__locals1.configData.QualificationAppraisal, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 40, (int)CS$<>8__locals1.configData.QualificationForging, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 41, (int)CS$<>8__locals1.configData.QualificationWoodworking, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 42, (int)CS$<>8__locals1.configData.QualificationMedicine, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 43, (int)CS$<>8__locals1.configData.QualificationToxicology, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 44, (int)CS$<>8__locals1.configData.QualificationWeaving, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 45, (int)CS$<>8__locals1.configData.QualificationJade, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 46, (int)CS$<>8__locals1.configData.QualificationTaoism, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 47, (int)CS$<>8__locals1.configData.QualificationBuddhism, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 48, (int)CS$<>8__locals1.configData.QualificationCooking, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 49, (int)CS$<>8__locals1.configData.QualificationEclectic, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 66, (int)CS$<>8__locals1.configData.QualificationNeigong, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 67, (int)CS$<>8__locals1.configData.QualificationPosing, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 68, (int)CS$<>8__locals1.configData.QualificationStunt, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 69, (int)CS$<>8__locals1.configData.QualificationFistAndPalm, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 70, (int)CS$<>8__locals1.configData.QualificationFinger, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 71, (int)CS$<>8__locals1.configData.QualificationLeg, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 72, (int)CS$<>8__locals1.configData.QualificationThrow, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 73, (int)CS$<>8__locals1.configData.QualificationSword, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 74, (int)CS$<>8__locals1.configData.QualificationBlade, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 75, (int)CS$<>8__locals1.configData.QualificationPolearm, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 76, (int)CS$<>8__locals1.configData.QualificationSpecial, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 77, (int)CS$<>8__locals1.configData.QualificationWhip, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 78, (int)CS$<>8__locals1.configData.QualificationControllableShot, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 79, (int)CS$<>8__locals1.configData.QualificationCombatMusic, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 102, (int)CS$<>8__locals1.configData.Fertility, false, false, false, true, false, false);
		base.CGet<GameObject>("Fame").SetActive(false);
		bool flag16 = !this._templateDataOnly && isInGame && CS$<>8__locals1.characterId >= 0;
		if (flag16)
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, CS$<>8__locals1.characterId, delegate(int offset, RawDataPool dataPool)
			{
				bool rebuildLayout = false;
				CharacterDisplayData displayData = null;
				Serializer.Deserialize(dataPool, offset, ref displayData);
				bool isTaiwu = CS$<>8__locals1.characterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				int fameChange = GameData.Domains.Character.SharedMethods.GetSectFeatureFameBonus(CS$<>8__locals1.featureId, isTaiwu, displayData.OrgInfo);
				bool flag19 = fameChange != 0;
				if (flag19)
				{
					CS$<>8__locals1.<>4__this.CGet<GameObject>("Fame").SetActive(true);
					CS$<>8__locals1.<>4__this.CGet<TextMeshProUGUI>("AddFame").text = ((fameChange > 0) ? string.Format("+{0}", fameChange) : "");
					CS$<>8__locals1.<>4__this.CGet<TextMeshProUGUI>("ReduceFame").text = ((fameChange > 0) ? "" : string.Format("{0}", fameChange));
					rebuildLayout = (CS$<>8__locals1.hasEffectChange = true);
				}
				bool flag20 = CS$<>8__locals1.featureId == 216;
				if (flag20)
				{
					bool flag21 = displayData.AliveState == 0;
					if (flag21)
					{
						string initial = CS$<>8__locals1.configData.EffectDesc.GetFormat(new object[]
						{
							displayData.DarkAshCounter.Total,
							displayData.DarkAshCounter.Tips1,
							displayData.DarkAshCounter.Tips2,
							displayData.DarkAshCounter.Tips3
						});
						bool flag22 = displayData.DarkAshProtector == 512U;
						if (flag22)
						{
							CS$<>8__locals1.effectDescRoot.CGet<TextMeshProUGUI>("Desc").text = initial.ColorReplace();
						}
						else
						{
							int starter = 0;
							StringBuilder sb = new StringBuilder(initial);
							sb.AppendLine("");
							while (1 << starter < 512)
							{
								bool flag23 = (1U << starter & displayData.DarkAshProtector) > 0U;
								if (flag23)
								{
									sb.Append("\n" + LocalStringManager.Get(string.Format("LK_MouseTip_DarkAsh_Protector{0}", starter)));
								}
								starter++;
							}
							CS$<>8__locals1.effectDescRoot.CGet<TextMeshProUGUI>("Desc").text = sb.ToString().ColorReplace();
						}
					}
					else
					{
						CS$<>8__locals1.effectDescRoot.CGet<TextMeshProUGUI>("Desc").text = CS$<>8__locals1.configData.EffectDesc.GetFormat(new object[]
						{
							"-",
							"-",
							"-",
							"-\n"
						}).ColorReplace();
					}
					rebuildLayout = true;
				}
				bool flag24 = rebuildLayout;
				if (flag24)
				{
					base.<Init>g__RebuildLayout|3();
				}
			});
		}
		else
		{
			bool flag17 = CS$<>8__locals1.featureId == 216;
			if (flag17)
			{
				CS$<>8__locals1.effectDescRoot.CGet<TextMeshProUGUI>("Desc").text = CS$<>8__locals1.configData.EffectDesc.GetFormat(new object[]
				{
					"-",
					"-",
					"-",
					"-\n"
				}).ColorReplace();
				CS$<>8__locals1.<Init>g__RebuildLayout|3();
			}
		}
		for (int j = index; j < addPropertyHolder.childCount; j++)
		{
			addPropertyHolder.GetChild(j).gameObject.SetActive(false);
		}
		CS$<>8__locals1.hasEffectChange = (CS$<>8__locals1.hasEffectChange || index > 0);
		base.CGet<GameObject>("FeatureEffect").SetActive(CS$<>8__locals1.hasEffectChange);
		bool flag18 = showEffect | CS$<>8__locals1.hasEffectChange;
		if (flag18)
		{
			CS$<>8__locals1.<Init>g__RebuildLayout|3();
		}
	}

	// Token: 0x06002A0F RID: 10767 RVA: 0x0014086C File Offset: 0x0013EA6C
	private void Update()
	{
		bool flag = !this.HasStick && this.Element != null && CommonCommandKit.PrimaryInteraction.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			ViewEncyclopediaPanel.OpenLink(EncyclopediaTipLink.DefValue.Trait);
		}
	}

	// Token: 0x04001E87 RID: 7815
	[SerializeField]
	private TMP_Text remainMonth;

	// Token: 0x04001E88 RID: 7816
	[SerializeField]
	private int normalBaseWidth = 500;

	// Token: 0x04001E89 RID: 7817
	[SerializeField]
	private int wideBaseWidth = 1100;

	// Token: 0x04001E8A RID: 7818
	private static readonly List<short> WideTipsFeature = new List<short>
	{
		309,
		310,
		311,
		312,
		313,
		314,
		315,
		316,
		317
	};

	// Token: 0x04001E8B RID: 7819
	private static readonly List<LanguageKey> SkillPowerUpTypeIds = new List<LanguageKey>
	{
		LanguageKey.LK_CombatSkill_EquipType_0,
		LanguageKey.LK_CombatSkill_EquipType_1,
		LanguageKey.LK_CombatSkill_EquipType_2,
		LanguageKey.LK_CombatSkill_EquipType_3,
		LanguageKey.LK_CombatSkill_EquipType_4
	};

	// Token: 0x04001E8C RID: 7820
	private bool _templateDataOnly;
}
