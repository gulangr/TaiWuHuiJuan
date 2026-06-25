using System;
using System.Runtime.CompilerServices;
using Config;
using TMPro;
using UnityEngine;

// Token: 0x020002E8 RID: 744
public class TipsRefiningEffect : Refers
{
	// Token: 0x06002BE4 RID: 11236 RVA: 0x0015774C File Offset: 0x0015594C
	public void SetData(int charId, sbyte itemType, sbyte propertyType, int value, bool percent = true, bool showEquipmentType = false)
	{
		bool flag = charId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		if (flag)
		{
			bool meet = SingletonObject.getInstance<ProfessionModel>().IsProfessionalSkillUnlockedAndEquipped(11);
			bool flag2 = meet;
			if (flag2)
			{
				value = value * 150 / 100;
			}
		}
		string percentStr = percent ? "%" : "";
		base.CGet<CImage>("EffectIcon").SetSprite(TipsRefiningEffect.RefiningIconName[(int)itemType][(int)propertyType], false, null);
		base.CGet<TextMeshProUGUI>("Name").text = LocalStringManager.Get(TipsRefiningEffect.RefiningPropertyNameKey[(int)itemType][(int)propertyType]);
		base.CGet<TextMeshProUGUI>("Value").text = ((value >= 0) ? string.Format("+{0}{1}", value, percentStr) : string.Format("{0}{1}", value, percentStr));
		GameObject type = base.CGet<GameObject>("Type");
		type.SetActive(showEquipmentType);
		if (showEquipmentType)
		{
			if (!true)
			{
			}
			LanguageKey languageKey;
			switch (itemType)
			{
			case 0:
				languageKey = LanguageKey.LK_ItemTips_RefineEffect_Weapon;
				break;
			case 1:
				languageKey = LanguageKey.LK_ItemTips_RefineEffect_Armor;
				break;
			case 2:
				languageKey = LanguageKey.LK_ItemTips_RefineEffect_Accessory;
				break;
			default:
				languageKey = LanguageKey.Invalid;
				break;
			}
			if (!true)
			{
			}
			LanguageKey key = languageKey;
			type.GetComponentInChildren<TextMeshProUGUI>().text = LocalStringManager.Get(key);
		}
	}

	// Token: 0x06002BE5 RID: 11237 RVA: 0x00157888 File Offset: 0x00155A88
	public static string GetRefinePropertyIconName(ERefiningEffectWeaponType effectType, bool isBigIcon = true)
	{
		ECharacterPropertyDisplayType displayType = TipsRefiningEffect.GetECharacterPropertyDisplayTypeByERefiningEffectType(effectType);
		CharacterPropertyDisplayItem property = CharacterPropertyDisplay.Instance[(int)displayType];
		return property.TipsIcon;
	}

	// Token: 0x06002BE6 RID: 11238 RVA: 0x001578B4 File Offset: 0x00155AB4
	public static string GetRefinePropertyIconName(ERefiningEffectArmorType effectType, bool isBigIcon = true)
	{
		ECharacterPropertyDisplayType displayType = TipsRefiningEffect.GetECharacterPropertyDisplayTypeByERefiningEffectType(effectType);
		CharacterPropertyDisplayItem property = CharacterPropertyDisplay.Instance[(int)displayType];
		return property.TipsIcon;
	}

	// Token: 0x06002BE7 RID: 11239 RVA: 0x001578E0 File Offset: 0x00155AE0
	public static string GetRefinePropertyIconName(ERefiningEffectAccessoryType effectType, bool isBigIcon = true)
	{
		ECharacterPropertyDisplayType displayType = TipsRefiningEffect.GetECharacterPropertyDisplayTypeByERefiningEffectType(effectType);
		CharacterPropertyDisplayItem property = CharacterPropertyDisplay.Instance[(int)displayType];
		return property.TipsIcon;
	}

	// Token: 0x06002BE8 RID: 11240 RVA: 0x0015790C File Offset: 0x00155B0C
	public static string GetRefinePropertyName(ERefiningEffectWeaponType effectType)
	{
		ECharacterPropertyDisplayType displayType = TipsRefiningEffect.GetECharacterPropertyDisplayTypeByERefiningEffectType(effectType);
		return CharacterPropertyDisplay.Instance[(int)displayType].Name;
	}

	// Token: 0x06002BE9 RID: 11241 RVA: 0x00157938 File Offset: 0x00155B38
	public static string GetRefinePropertyName(ERefiningEffectArmorType effectType)
	{
		ECharacterPropertyDisplayType displayType = TipsRefiningEffect.GetECharacterPropertyDisplayTypeByERefiningEffectType(effectType);
		return CharacterPropertyDisplay.Instance[(int)displayType].Name;
	}

	// Token: 0x06002BEA RID: 11242 RVA: 0x00157964 File Offset: 0x00155B64
	public static string GetRefinePropertyName(ERefiningEffectAccessoryType effectType)
	{
		ECharacterPropertyDisplayType displayType = TipsRefiningEffect.GetECharacterPropertyDisplayTypeByERefiningEffectType(effectType);
		return CharacterPropertyDisplay.Instance[(int)displayType].Name;
	}

	// Token: 0x06002BEB RID: 11243 RVA: 0x00157990 File Offset: 0x00155B90
	public static ECharacterPropertyDisplayType GetECharacterPropertyDisplayTypeByERefiningEffectType(ERefiningEffectWeaponType effectType)
	{
		if (!true)
		{
		}
		ECharacterPropertyDisplayType result;
		switch (effectType)
		{
		case ERefiningEffectWeaponType.HitRateStrength:
			result = ECharacterPropertyDisplayType.HitRateStrength;
			break;
		case ERefiningEffectWeaponType.HitRateTechnique:
			result = ECharacterPropertyDisplayType.HitRateTechnique;
			break;
		case ERefiningEffectWeaponType.HitRateSpeed:
			result = ECharacterPropertyDisplayType.HitRateSpeed;
			break;
		case ERefiningEffectWeaponType.HitRateMind:
			result = ECharacterPropertyDisplayType.HitRateMind;
			break;
		case ERefiningEffectWeaponType.EquipmentAttack:
			result = ECharacterPropertyDisplayType.EquipmentWeaponAttack;
			break;
		case ERefiningEffectWeaponType.EquipmentDefense:
			result = ECharacterPropertyDisplayType.EquipmentDefense;
			break;
		case ERefiningEffectWeaponType.Penetration:
			result = ECharacterPropertyDisplayType.EquipmentPenetration;
			break;
		case ERefiningEffectWeaponType.Weight:
			result = ECharacterPropertyDisplayType.EquipmentWeight;
			break;
		case ERefiningEffectWeaponType.MinAttackRange:
			result = ECharacterPropertyDisplayType.EquipmentMinAttackRange;
			break;
		case ERefiningEffectWeaponType.MaxDurability:
			result = ECharacterPropertyDisplayType.EquipmentMaxDurability;
			break;
		case ERefiningEffectWeaponType.MaxAttackRange:
			result = ECharacterPropertyDisplayType.EquipmentMaxAttackRange;
			break;
		case ERefiningEffectWeaponType.PursueAttackFactor:
			result = ECharacterPropertyDisplayType.EquipmentPursueAttackFactor;
			break;
		case ERefiningEffectWeaponType.ChangeTrickPercent:
			result = ECharacterPropertyDisplayType.EquipmentChangeTrickPercent;
			break;
		case ERefiningEffectWeaponType.MaxPower:
			result = ECharacterPropertyDisplayType.EquipmentMaxPower;
			break;
		case ERefiningEffectWeaponType.UseRequirement:
			result = ECharacterPropertyDisplayType.EquipmentUseRequirement;
			break;
		default:
			if (!true)
			{
			}
			<PrivateImplementationDetails>.ThrowSwitchExpressionException(effectType);
			break;
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06002BEC RID: 11244 RVA: 0x00157A68 File Offset: 0x00155C68
	public static ECharacterPropertyDisplayType GetECharacterPropertyDisplayTypeByERefiningEffectType(ERefiningEffectArmorType effectType)
	{
		if (!true)
		{
		}
		ECharacterPropertyDisplayType result;
		switch (effectType)
		{
		case ERefiningEffectArmorType.AvoidRateStrength:
			result = ECharacterPropertyDisplayType.AvoidRateStrength;
			break;
		case ERefiningEffectArmorType.AvoidRateTechnique:
			result = ECharacterPropertyDisplayType.AvoidRateTechnique;
			break;
		case ERefiningEffectArmorType.AvoidRateSpeed:
			result = ECharacterPropertyDisplayType.AvoidRateSpeed;
			break;
		case ERefiningEffectArmorType.AvoidRateMind:
			result = ECharacterPropertyDisplayType.AvoidRateMind;
			break;
		case ERefiningEffectArmorType.EquipmentAttack:
			result = ECharacterPropertyDisplayType.EquipmentArmorAttack;
			break;
		case ERefiningEffectArmorType.EquipmentDefense:
			result = ECharacterPropertyDisplayType.EquipmentDefense;
			break;
		case ERefiningEffectArmorType.PenetrationResist:
			result = ECharacterPropertyDisplayType.EquipmentPenetration;
			break;
		case ERefiningEffectArmorType.Weight:
			result = ECharacterPropertyDisplayType.EquipmentWeight;
			break;
		case ERefiningEffectArmorType.InjuryFactorInner:
			result = ECharacterPropertyDisplayType.EquipmentInjuryFactorInner;
			break;
		case ERefiningEffectArmorType.MaxDurability:
			result = ECharacterPropertyDisplayType.EquipmentMaxDurability;
			break;
		case ERefiningEffectArmorType.InjuryFactorOuter:
			result = ECharacterPropertyDisplayType.EquipmentInjuryFactorOuter;
			break;
		case ERefiningEffectArmorType.CounterAttackPower:
			result = ECharacterPropertyDisplayType.EquipmentCounterAttackPower;
			break;
		case ERefiningEffectArmorType.CounterDamagePower:
			result = ECharacterPropertyDisplayType.EquipmentCounterDamagePower;
			break;
		case ERefiningEffectArmorType.MaxPower:
			result = ECharacterPropertyDisplayType.EquipmentMaxPower;
			break;
		case ERefiningEffectArmorType.UseRequirement:
			result = ECharacterPropertyDisplayType.EquipmentUseRequirement;
			break;
		default:
			if (!true)
			{
			}
			<PrivateImplementationDetails>.ThrowSwitchExpressionException(effectType);
			break;
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06002BED RID: 11245 RVA: 0x00157B44 File Offset: 0x00155D44
	public static ECharacterPropertyDisplayType GetECharacterPropertyDisplayTypeByERefiningEffectType(ERefiningEffectAccessoryType effectType)
	{
		if (!true)
		{
		}
		ECharacterPropertyDisplayType result;
		switch (effectType)
		{
		case ERefiningEffectAccessoryType.HitRateStrength:
			result = ECharacterPropertyDisplayType.HitRateStrength;
			break;
		case ERefiningEffectAccessoryType.HitRateTechnique:
			result = ECharacterPropertyDisplayType.HitRateTechnique;
			break;
		case ERefiningEffectAccessoryType.HitRateSpeed:
			result = ECharacterPropertyDisplayType.HitRateSpeed;
			break;
		case ERefiningEffectAccessoryType.HitRateMind:
			result = ECharacterPropertyDisplayType.HitRateMind;
			break;
		case ERefiningEffectAccessoryType.AvoidRateStrength:
			result = ECharacterPropertyDisplayType.AvoidRateStrength;
			break;
		case ERefiningEffectAccessoryType.AvoidRateTechnique:
			result = ECharacterPropertyDisplayType.AvoidRateTechnique;
			break;
		case ERefiningEffectAccessoryType.AvoidRateSpeed:
			result = ECharacterPropertyDisplayType.AvoidRateSpeed;
			break;
		case ERefiningEffectAccessoryType.AvoidRateMind:
			result = ECharacterPropertyDisplayType.AvoidRateMind;
			break;
		case ERefiningEffectAccessoryType.Calm:
			result = ECharacterPropertyDisplayType.PersonalityCalm;
			break;
		case ERefiningEffectAccessoryType.Firm:
			result = ECharacterPropertyDisplayType.PersonalityFirm;
			break;
		case ERefiningEffectAccessoryType.Brave:
			result = ECharacterPropertyDisplayType.PersonalityBrave;
			break;
		case ERefiningEffectAccessoryType.Enthusiastic:
			result = ECharacterPropertyDisplayType.PersonalityEnthusiastic;
			break;
		case ERefiningEffectAccessoryType.Clever:
			result = ECharacterPropertyDisplayType.PersonalityClever;
			break;
		case ERefiningEffectAccessoryType.Lucky:
			result = ECharacterPropertyDisplayType.PersonalityLucky;
			break;
		case ERefiningEffectAccessoryType.Perceptive:
			result = ECharacterPropertyDisplayType.PersonalityPerceptive;
			break;
		default:
			if (!true)
			{
			}
			<PrivateImplementationDetails>.ThrowSwitchExpressionException(effectType);
			break;
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06002BEF RID: 11247 RVA: 0x00157C04 File Offset: 0x00155E04
	// Note: this type is marked as 'beforefieldinit'.
	static TipsRefiningEffect()
	{
		LanguageKey[][] array = new LanguageKey[3][];
		int num = 0;
		LanguageKey[] array2 = new LanguageKey[15];
		RuntimeHelpers.InitializeArray(array2, fieldof(<PrivateImplementationDetails>.EA84A9AE1B29929E778C37365BE788DB7DE03BC230C9BCF4EDD0618838509A00).FieldHandle);
		array[num] = array2;
		int num2 = 1;
		LanguageKey[] array3 = new LanguageKey[15];
		RuntimeHelpers.InitializeArray(array3, fieldof(<PrivateImplementationDetails>.CABF10DA1DD4A07769AF74CDEFD7E1C163D5EFB65D48579FBF6B82DAC711255D).FieldHandle);
		array[num2] = array3;
		int num3 = 2;
		LanguageKey[] array4 = new LanguageKey[15];
		RuntimeHelpers.InitializeArray(array4, fieldof(<PrivateImplementationDetails>.BE6F2887A5ED96CB18BA4798D1F723C11C0D7873DAB9E7BB4C9172BB96D6CF0D).FieldHandle);
		array[num3] = array4;
		TipsRefiningEffect.RefiningPropertyNameKey = array;
		TipsRefiningEffect.RefiningIconName = new string[][]
		{
			new string[]
			{
				"mousetip_mingzhong_big_0",
				"mousetip_mingzhong_big_1",
				"mousetip_mingzhong_big_2",
				"mousetip_mingzhong_big_3",
				"mousetip_jingzhi_3",
				"mousetip_jingzhi_2",
				"mousetip_jingzhi_0",
				"mousetip_jingzhi_5",
				"mousetip_mingzhong_big_1",
				"mousetip_mingzhong_big_2",
				"mousetip_mingzhong_big_3",
				"mousetip_jingzhi_3",
				"mousetip_jingzhi_2",
				"mousetip_jingzhi_0",
				"mousetip_jingzhi_5"
			},
			new string[]
			{
				"mousetip_huajie_big_0",
				"mousetip_huajie_big_1",
				"mousetip_huajie_big_2",
				"mousetip_huajie_big_3",
				"mousetip_jingzhi_4",
				"mousetip_jingzhi_2",
				"mousetip_jingzhi_1",
				"mousetip_jingzhi_5",
				"mousetip_huajie_big_1",
				"mousetip_huajie_big_2",
				"mousetip_huajie_big_3",
				"mousetip_jingzhi_4",
				"mousetip_jingzhi_2",
				"mousetip_jingzhi_1",
				"mousetip_jingzhi_5"
			},
			new string[]
			{
				"mousetip_mingzhong_big_0",
				"mousetip_mingzhong_big_1",
				"mousetip_mingzhong_big_2",
				"mousetip_mingzhong_big_3",
				"mousetip_huajie_big_0",
				"mousetip_huajie_big_1",
				"mousetip_huajie_big_2",
				"mousetip_huajie_big_3",
				"mousetip_mingzhong_big_1",
				"mousetip_mingzhong_big_2",
				"mousetip_mingzhong_big_3",
				"mousetip_huajie_big_0",
				"mousetip_huajie_big_1",
				"mousetip_huajie_big_2",
				"mousetip_huajie_big_3"
			}
		};
	}

	// Token: 0x04001FE6 RID: 8166
	public static readonly LanguageKey[][] RefiningPropertyNameKey;

	// Token: 0x04001FE7 RID: 8167
	public static readonly string[][] RefiningIconName;
}
