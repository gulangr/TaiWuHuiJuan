using System;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002C0 RID: 704
public class MouseTipMedicine : MouseTipItem
{
	// Token: 0x170004AF RID: 1199
	// (get) Token: 0x06002AE0 RID: 10976 RVA: 0x001496A5 File Offset: 0x001478A5
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002AE1 RID: 10977 RVA: 0x001496A8 File Offset: 0x001478A8
	protected unsafe override void Init(ArgumentBox argsBox)
	{
		base.Init(argsBox);
		ItemDisplayData itemData;
		argsBox.Get<ItemDisplayData>("ItemData", out itemData);
		bool templateDataOnly;
		argsBox.Get("TemplateDataOnly", out templateDataOnly);
		this._itemData = itemData;
		bool flag = !argsBox.Get("CharId", out this._charId);
		if (flag)
		{
			this._charId = -1;
		}
		MedicineItem configData = Medicine.Instance[itemData.Key.TemplateId];
		EMedicineEffectType effectType = configData.EffectType;
		int effectValue = templateDataOnly ? ((int)configData.EffectValue) : itemData.MedicineEffectValue;
		base.CGet<TextMeshProUGUI>("Name").text = configData.Name;
		base.CGet<CImage>("GradeBack").SetSprite(ItemView.GetGradeIcon(configData.Grade), false, null);
		base.CGet<TextMeshProUGUI>("GradeName").text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", configData.Grade));
		base.CGet<TextMeshProUGUI>("Grade").text = (LocalStringManager.Get(string.Format("LK_Num_{0}", (int)(9 - configData.Grade))) + LocalStringManager.Get(LanguageKey.LK_Item_Grade)).SetColor(Colors.Instance.GradeColors[(int)configData.Grade]);
		base.CGet<TextMeshProUGUI>("Value").text = (templateDataOnly ? configData.BaseValue.ToString() : itemData.Value.ToString());
		base.CGet<GameObject>("Material").SetActive(!templateDataOnly);
		base.CGet<CImage>("ItemIcon").SetSprite(configData.Icon, false, null);
		base.SetItemDesc(configData.Desc, itemData.LoveTokenDataItem);
		base.CGet<TextMeshProUGUI>("SubType").text = LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", configData.ItemSubType));
		base.CGet<TextMeshProUGUI>("Weight").text = NumberFormatUtils.FormatItemWeight(itemData.Weight);
		short eatingTime;
		this._showEatingTime = argsBox.Get("EatingTime", out eatingTime);
		base.CGet<GameObject>("EatingTimeTips").SetActive(this._showEatingTime);
		bool showEatingTime = this._showEatingTime;
		if (showEatingTime)
		{
			base.CGet<TextMeshProUGUI>("EatingTime").text = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_Eating_Time, eatingTime).ColorReplace();
		}
		this.InitItemDisableFunctionList(itemData);
		base.RefreshDisableFunction();
		bool isPoisonEffect = effectType == EMedicineEffectType.ApplyPoison;
		GameObject eatEffect = base.CGet<GameObject>("EatEffect");
		bool showEatEffect = Medicine.Instance[itemData.Key.TemplateId].HasNormalEatingEffect || this._showEatingTime;
		eatEffect.SetActive(showEatEffect);
		bool flag2 = !isPoisonEffect;
		if (flag2)
		{
			bool isInstant = configData.InstantAffect;
			bool isMonthly = configData.Duration != 0;
			bool isRecoverInjury = effectType == EMedicineEffectType.RecoverOuterInjury || effectType == EMedicineEffectType.RecoverInnerInjury;
			base.CGet<GameObject>("EatHealInjuryInstant").SetActive(isRecoverInjury && isInstant);
			base.CGet<GameObject>("EatHealInjuryMonthly").SetActive(isRecoverInjury && isMonthly);
			base.CGet<GameObject>("EatHealInjuryMonthlyPreTitle").SetActive(isRecoverInjury && isMonthly);
			base.CGet<GameObject>("EatInjuryMonthlyDamageStep").SetActive(isRecoverInjury && isMonthly);
			bool isRecoverHealth = effectType == EMedicineEffectType.RecoverHealth;
			base.CGet<GameObject>("EatHealHealthInstant").SetActive(isRecoverHealth && isInstant);
			base.CGet<GameObject>("EatHealHealthMonthly").SetActive(isRecoverHealth && isMonthly);
			base.CGet<GameObject>("EatHealthMonthlyDamageStep").SetActive(isRecoverHealth && isMonthly);
			bool isRecoveryQi = effectType == EMedicineEffectType.ChangeDisorderOfQi;
			base.CGet<GameObject>("EatHealQiDisorderInstant").SetActive(isRecoveryQi && isInstant);
			base.CGet<GameObject>("EatHealQiDisorderMonthly").SetActive(isRecoveryQi && isMonthly);
			base.CGet<GameObject>("EatQiDisorderMonthlyDamageStep").SetActive(isRecoveryQi && isMonthly);
			base.CGet<GameObject>("EatMonthlyDamageStepHint").SetActive((isRecoverInjury || isRecoverHealth || isRecoveryQi) && isMonthly);
			bool isHealPoison = effectType == EMedicineEffectType.DetoxPoison;
			base.CGet<GameObject>("EatHealPoison").SetActive(isHealPoison);
			bool flag3 = isRecoverInjury;
			if (flag3)
			{
				bool isOuter = effectType == EMedicineEffectType.RecoverOuterInjury;
				bool flag4 = isInstant;
				if (flag4)
				{
					LanguageKey eatHealInjuryInstantTipKey = isOuter ? LanguageKey.LK_ItemTips_MedicineEffect_Injury_Instant_Outer : LanguageKey.LK_ItemTips_MedicineEffect_Injury_Instant_Inner;
					base.CGet<TextMeshProUGUI>("EatHealInjuryInstantTip").text = LocalStringManager.GetFormat(eatHealInjuryInstantTipKey, configData.InjuryRecoveryTimes, effectValue, configData.EffectThresholdValue).ColorReplace();
				}
				bool flag5 = isMonthly;
				if (flag5)
				{
					LanguageKey eatHealInjuryMonthlyTipKey = isOuter ? LanguageKey.LK_ItemTips_MedicineEffect_Injury_Monthly_Outer : LanguageKey.LK_ItemTips_MedicineEffect_Injury_Monthly_Inner;
					base.CGet<TextMeshProUGUI>("EatHealInjuryMonthlyTip").text = LocalStringManager.GetFormat(eatHealInjuryMonthlyTipKey, configData.InjuryRecoveryTimes, effectValue, configData.EffectThresholdValue).ColorReplace();
					base.CGet<TextMeshProUGUI>("EatHealInjuryMonthlyPreTitleTip").text = (isOuter ? LanguageKey.LK_ItemTips_MedicineEffect_Injury_Monthly_PreTitle_Outer : LanguageKey.LK_ItemTips_MedicineEffect_Injury_Monthly_PreTitle_Inner).Tr().ColorReplace();
					LanguageKey eatInjuryMonthlyDamageStepTipKey = isOuter ? LanguageKey.LK_ItemTips_MedicineEffect_Injury_Monthly_DamageStep_Outer : LanguageKey.LK_ItemTips_MedicineEffect_Injury_Monthly_DamageStep_Inner;
					base.CGet<TextMeshProUGUI>("EatInjuryMonthlyDamageStepTip").text = LocalStringManager.GetFormat(eatInjuryMonthlyDamageStepTipKey, configData.DamageStepBonus).ColorReplace();
				}
			}
			else
			{
				bool flag6 = isRecoverHealth;
				if (flag6)
				{
					int value = effectValue;
					string valueText = configData.EffectIsPercentage ? string.Format("{0}%", value) : string.Format("{0}", value);
					bool flag7 = isInstant;
					if (flag7)
					{
						base.CGet<TextMeshProUGUI>("EatHealHealthInstantTip").text = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_MedicineEffect_Health_Instant, valueText).ColorReplace();
					}
					bool flag8 = isMonthly;
					if (flag8)
					{
						base.CGet<TextMeshProUGUI>("EatHealHealthMonthlyTip").text = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_MedicineEffect_Health_Monthly, valueText).ColorReplace();
						base.CGet<TextMeshProUGUI>("EatHealthMonthlyDamageStepTip").text = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_MedicineEffect_Health_Monthly_DamageStep, configData.DamageStepBonus).ColorReplace();
					}
				}
				else
				{
					bool flag9 = isRecoveryQi;
					if (flag9)
					{
						int value2 = effectValue;
						string valueText2 = configData.EffectIsPercentage ? string.Format("{0}%", value2) : string.Format("{0}", value2 / 10);
						bool flag10 = isInstant;
						if (flag10)
						{
							base.CGet<TextMeshProUGUI>("EatHealQiDisorderInstantTip").text = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_MedicineEffect_QiDisorder_Instant, valueText2).ColorReplace();
						}
						bool flag11 = isMonthly;
						if (flag11)
						{
							base.CGet<TextMeshProUGUI>("EatHealQiDisorderMonthlyTip").text = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_MedicineEffect_QiDisorder_Monthly, valueText2).ColorReplace();
							base.CGet<TextMeshProUGUI>("EatQiDisorderMonthlyDamageStepTip").text = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_MedicineEffect_QiDisorder_Monthly_DamageStep, configData.DamageStepBonus).ColorReplace();
						}
					}
					else
					{
						bool flag12 = isHealPoison;
						if (flag12)
						{
							sbyte poisonType = configData.PoisonType;
							PoisonItem poisonConfig = Poison.Instance[poisonType];
							string maxPoisonLevelTips = LocalStringManager.GetFormat(LanguageKey.LK_Poison_Max_Level_Tips, LocalStringManager.Get(string.Format("LK_Poison_Level_{0}", configData.EffectThresholdValue)));
							int value3 = effectValue;
							string text = configData.EffectIsPercentage ? string.Format("{0}%", value3) : string.Format("{0}", value3);
						}
					}
				}
			}
			bool eatCost = configData.RequiredMainAttributeType >= 0;
			base.CGet<GameObject>("EatCost").SetActive(eatCost);
			bool flag13 = eatCost;
			if (flag13)
			{
				CharacterPropertyDisplayItem propertyConfig = CharacterPropertyDisplay.Instance[(int)(0 + configData.RequiredMainAttributeType)];
				base.CGet<CImage>("CostAttributeIcon").SetSprite(propertyConfig.TipsIcon, false, null);
				base.CGet<TextMeshProUGUI>("CostAttributeName").text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTipMedicine_CostProperty, propertyConfig.Name);
				base.CGet<TextMeshProUGUI>("CostAttributeValue").text = configData.RequiredMainAttributeValue.ToString();
			}
			base.CGet<GameObject>("DurationHolder").SetActive(isMonthly);
			base.CGet<TextMeshProUGUI>("Duration").text = configData.Duration.ToString();
			TipsAddProperty addPropertyPrefab = base.CGet<TipsAddProperty>("AddProperty");
			RectTransform addPropertyHolder = base.CGet<RectTransform>("AddPropertyHolder");
			RectTransform addPropertyHolderSpecial = base.CGet<RectTransform>("AddPropertyHolderSpecial");
			bool flag14 = configData.TemplateId == 432;
			RectTransform curAddPropertyHolder;
			RectTransform otherAddPropertyHolder;
			if (flag14)
			{
				curAddPropertyHolder = addPropertyHolderSpecial;
				otherAddPropertyHolder = addPropertyHolder;
				curAddPropertyHolder.GetComponent<GridLayoutGroup>().spacing = new Vector2(40f, 0f);
			}
			else
			{
				bool flag15 = configData.TemplateId == 80;
				if (flag15)
				{
					curAddPropertyHolder = addPropertyHolderSpecial;
					otherAddPropertyHolder = addPropertyHolder;
					curAddPropertyHolder.GetComponent<GridLayoutGroup>().spacing = Vector2.zero;
				}
				else
				{
					curAddPropertyHolder = addPropertyHolder;
					otherAddPropertyHolder = addPropertyHolderSpecial;
				}
			}
			int index = 0;
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 6, (int)configData.HitRateStrength, false, configData.EffectIsPercentage, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 7, (int)configData.HitRateTechnique, false, configData.EffectIsPercentage, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 8, (int)configData.HitRateSpeed, false, configData.EffectIsPercentage, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 9, (int)configData.HitRateMind, false, configData.EffectIsPercentage, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 10, (int)configData.PenetrateOfOuter, false, configData.EffectIsPercentage, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 11, (int)configData.PenetrateOfInner, false, configData.EffectIsPercentage, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 12, (int)configData.AvoidRateStrength, false, configData.EffectIsPercentage, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 13, (int)configData.AvoidRateTechnique, false, configData.EffectIsPercentage, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 14, (int)configData.AvoidRateSpeed, false, configData.EffectIsPercentage, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 15, (int)configData.AvoidRateMind, false, configData.EffectIsPercentage, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 16, (int)configData.PenetrateResistOfOuter, false, configData.EffectIsPercentage, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 17, (int)configData.PenetrateResistOfInner, false, configData.EffectIsPercentage, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 18, (int)configData.RecoveryOfStance, false, configData.EffectIsPercentage, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 19, (int)configData.RecoveryOfBreath, false, configData.EffectIsPercentage, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 20, (int)configData.MoveSpeed, false, configData.EffectIsPercentage, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 21, (int)configData.RecoveryOfFlaw, false, configData.EffectIsPercentage, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 22, (int)configData.CastSpeed, false, configData.EffectIsPercentage, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 23, (int)configData.RecoveryOfBlockedAcupoint, false, configData.EffectIsPercentage, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 24, (int)configData.WeaponSwitchSpeed, false, configData.EffectIsPercentage, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 25, (int)configData.AttackSpeed, false, configData.EffectIsPercentage, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 26, (int)configData.InnerRatio, false, configData.EffectIsPercentage, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 27, (int)configData.RecoveryOfQiDisorder, false, configData.EffectIsPercentage, false, true, false, false);
			base.CGet<GameObject>("AddPropertySpace").SetActive(index > 0 && effectType >= EMedicineEffectType.RecoverOuterInjury);
			curAddPropertyHolder.gameObject.SetActive(index > 0);
			for (int i = index; i < curAddPropertyHolder.childCount; i++)
			{
				curAddPropertyHolder.GetChild(i).gameObject.SetActive(false);
			}
			otherAddPropertyHolder.gameObject.SetActive(false);
		}
		else
		{
			base.CGet<GameObject>("EatHealInjuryInstant").SetActive(false);
			base.CGet<GameObject>("EatHealInjuryMonthly").SetActive(false);
			base.CGet<GameObject>("EatInjuryMonthlyDamageStep").SetActive(false);
			base.CGet<GameObject>("EatHealHealthInstant").SetActive(false);
			base.CGet<GameObject>("EatHealHealthMonthly").SetActive(false);
			base.CGet<GameObject>("EatHealthMonthlyDamageStep").SetActive(false);
			base.CGet<GameObject>("EatHealQiDisorderInstant").SetActive(false);
			base.CGet<GameObject>("EatHealQiDisorderMonthly").SetActive(false);
			base.CGet<GameObject>("EatQiDisorderMonthlyDamageStep").SetActive(false);
			base.CGet<GameObject>("EatHealPoison").SetActive(false);
			base.CGet<GameObject>("EatResistPoison").SetActive(false);
			base.CGet<GameObject>("EatCost").SetActive(false);
			base.CGet<GameObject>("DurationHolder").SetActive(false);
			base.CGet<GameObject>("AddPropertySpace").SetActive(false);
			base.CGet<RectTransform>("AddPropertyHolder").gameObject.SetActive(false);
			base.CGet<RectTransform>("AddPropertyHolderSpecial").gameObject.SetActive(false);
		}
		PoisonsAndLevels innatePoisons = default(PoisonsAndLevels);
		innatePoisons.Initialize();
		bool flag16 = effectType == EMedicineEffectType.ApplyPoison;
		if (flag16)
		{
			sbyte type = configData.PoisonType;
			*(ref innatePoisons.Values.FixedElementField + (IntPtr)type * 2) = (short)effectValue;
			*(ref innatePoisons.Levels.FixedElementField + type) = (sbyte)configData.EffectThresholdValue;
		}
		base.RefreshPoisons(innatePoisons, itemData);
		sbyte poisonType2 = configData.PoisonType;
		PoisonItem poisonConfig2 = Poison.Instance[poisonType2];
		if (!true)
		{
		}
		int num;
		switch (poisonType2)
		{
		case 0:
			num = (int)configData.ResistOfHotPoison;
			break;
		case 1:
			num = (int)configData.ResistOfGloomyPoison;
			break;
		case 2:
			num = (int)configData.ResistOfColdPoison;
			break;
		case 3:
			num = (int)configData.ResistOfRedPoison;
			break;
		case 4:
			num = (int)configData.ResistOfRottenPoison;
			break;
		case 5:
			num = (int)configData.ResistOfIllusoryPoison;
			break;
		default:
			num = 0;
			break;
		}
		if (!true)
		{
		}
		int resistValue = num;
		bool showPoisonResist = resistValue > 0;
		base.CGet<GameObject>("EatResistPoison").SetActive(showPoisonResist);
		bool flag17 = showPoisonResist;
		if (flag17)
		{
			string content = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_MedicineEffect_ResistPoison_Monthly, poisonConfig2.TipsIcon, poisonConfig2.Name, resistValue).ColorReplace();
			base.CGet<TextMeshProUGUI>("EatResistPoisonTip").text = content;
		}
		bool canUseInCombat = configData.ConsumedFeatureMedals >= 0;
		base.CGet<GameObject>("UseInCombat").SetActive(canUseInCombat);
		bool flag18 = canUseInCombat;
		if (flag18)
		{
			base.CGet<TextMeshProUGUI>("CostWisdom").text = string.Format("x{0}", configData.ConsumedFeatureMedals);
		}
		TipWugKingEffect wugKingEffect = base.CGet<TipWugKingEffect>("WugKingEffect");
		wugKingEffect.Refresh(this._itemData.Key, this._charId, this._showEatingTime);
		this._isWugKing = EatingItems.IsWugKing(this._itemData.Key);
		this._isWug = EatingItems.IsWug(this._itemData.Key);
		TextMeshProUGUI killWugWay = base.CGet<TextMeshProUGUI>("KillWugWay");
		bool isWug = this._isWug;
		if (isWug)
		{
			killWugWay.text = TipWugKingEffect.GetKillWugTip(configData);
			TMPTextSpriteHelper helper = killWugWay.GetComponent<TMPTextSpriteHelper>();
			helper.Parse();
		}
		base.CGet<MoreInfo2>("MoreInfo2").gameObject.SetActive(this._isWug);
		TMPTextSpriteHelper[] helpers = base.GetComponentsInChildren<TMPTextSpriteHelper>();
		bool flag19 = helpers != null;
		if (flag19)
		{
			foreach (TMPTextSpriteHelper helper2 in helpers)
			{
				helper2.Parse();
			}
		}
		base.RefreshHoldCount();
		base.RefreshHotkeyDisplayLockItem();
		base.ForceRebuildLayout(2U, null);
	}

	// Token: 0x06002AE2 RID: 10978 RVA: 0x0014A660 File Offset: 0x00148860
	protected override void InitItemDisableFunctionList(ItemDisplayData itemDisplayData)
	{
		base.InitItemDisableFunctionList(itemDisplayData);
		MedicineItem configData = Medicine.Instance[itemDisplayData.Key.TemplateId];
		bool flag = !configData.Repairable;
		if (flag)
		{
			this._disableFunctionList.Add(MouseTipItem.ItemFunction.Repairable);
		}
		bool flag2 = !configData.Transferable;
		if (flag2)
		{
			this._disableFunctionList.Add(MouseTipItem.ItemFunction.Transferable);
		}
		bool flag3 = !configData.Poisonable;
		if (flag3)
		{
			this._disableFunctionList.Add(MouseTipItem.ItemFunction.Poisonable);
		}
		bool flag4 = !configData.Refinable;
		if (flag4)
		{
			this._disableFunctionList.Add(MouseTipItem.ItemFunction.Refinable);
		}
	}

	// Token: 0x06002AE3 RID: 10979 RVA: 0x0014A6F8 File Offset: 0x001488F8
	private void Update()
	{
		bool flag = this.HasStick && this.CanStick;
		if (!flag)
		{
			bool altDown = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
			base.CGet<TextMeshProUGUI>("KillWugWay").gameObject.SetActive(this._isWug && altDown);
			base.CGet<GameObject>("WugKingFeature").SetActive(this._isWugKing && altDown);
			MoreInfo2 moreInfo = base.CGet<MoreInfo2>("MoreInfo2");
			bool flag2 = altDown;
			if (flag2)
			{
				moreInfo.RefreshCancelDetail();
			}
			else
			{
				moreInfo.RefreshPressToDetail();
			}
		}
	}

	// Token: 0x04001EFF RID: 7935
	private bool _isWug;

	// Token: 0x04001F00 RID: 7936
	private bool _isWugKing;

	// Token: 0x04001F01 RID: 7937
	private bool _showEatingTime;
}
