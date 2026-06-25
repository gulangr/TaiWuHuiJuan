using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using CharacterDataMonitor;
using Config;
using Config.ConfigCells.Character;
using DG.Tweening;
using DisplayConfig;
using FrameWork;
using Game.Components.Character;
using Game.Views.SectInteract;
using Game.Views.Villager;
using GameData.DLC.FiveLoong;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Alertness;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.Relation;
using GameData.Domains.CombatSkill;
using GameData.Domains.Item;
using GameData.Domains.Map;
using GameData.Domains.Merchant;
using GameData.Domains.Organization;
using GameData.Domains.Organization.Display;
using GameData.Domains.Taiwu.Profession;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UnityEngine;

// Token: 0x02000120 RID: 288
public static class CommonUtils
{
	// Token: 0x06000A96 RID: 2710 RVA: 0x000440EC File Offset: 0x000422EC
	public static string GetResOrExpName(int type)
	{
		bool flag = type < 0 || type >= Config.ResourceType.Instance.Count;
		string result;
		if (flag)
		{
			result = LocalStringManager.Get(LanguageKey.LK_Exp);
		}
		else
		{
			result = Config.ResourceType.Instance[type].Name;
		}
		return result;
	}

	// Token: 0x06000A97 RID: 2711 RVA: 0x00044138 File Offset: 0x00042338
	public static string GetResOrExpIconLegacy(int type)
	{
		bool flag = type < 0 || type >= Config.ResourceType.Instance.Count;
		string icon;
		if (flag)
		{
			icon = CharacterPropertyDisplay.Instance[104].Icon;
		}
		else
		{
			icon = Config.ResourceType.Instance[type].Icon;
		}
		return icon;
	}

	// Token: 0x06000A98 RID: 2712 RVA: 0x0004418C File Offset: 0x0004238C
	public static string GetResOrExpIcon(sbyte type, bool isBig = true)
	{
		string prefix = isBig ? "ui9_icon_resource_big_" : "ui9_icon_resource_small_";
		bool flag = type < 0 || type >= 8;
		string result;
		if (flag)
		{
			result = "ui9_icon_item_exp";
		}
		else
		{
			result = prefix + type.ToString();
		}
		return result;
	}

	// Token: 0x06000A99 RID: 2713 RVA: 0x000441D8 File Offset: 0x000423D8
	public static string GetGenderString(CommonUtils.EDisplayGender displayGender)
	{
		string result;
		switch (displayGender)
		{
		case CommonUtils.EDisplayGender.Male:
			result = LocalStringManager.Get(LanguageKey.LK_Gender_Man);
			break;
		case CommonUtils.EDisplayGender.Female:
			result = LocalStringManager.Get(LanguageKey.LK_Gender_Woman);
			break;
		case CommonUtils.EDisplayGender.Hidden:
			result = "-";
			break;
		default:
			result = LocalStringManager.Get(LanguageKey.LK_Unknown);
			break;
		}
		return result;
	}

	// Token: 0x06000A9A RID: 2714 RVA: 0x0004422E File Offset: 0x0004242E
	public static string GetJiaoGenderString(bool gender)
	{
		return gender ? LanguageKey.LK_Animal_Male.Tr().ColorReplace() : LanguageKey.LK_Animal_Female.Tr().ColorReplace();
	}

	// Token: 0x06000A9B RID: 2715 RVA: 0x00044254 File Offset: 0x00042454
	public static string GetGenderIcon(CommonUtils.EDisplayGender displayGender)
	{
		switch (displayGender)
		{
		case CommonUtils.EDisplayGender.Male:
			return "sp_icon_gender_male";
		case CommonUtils.EDisplayGender.Female:
			return "sp_icon_gender_female";
		}
		return string.Empty;
	}

	// Token: 0x06000A9C RID: 2716 RVA: 0x00044294 File Offset: 0x00042494
	public static string GetGenderIconBig(CommonUtils.EDisplayGender displayGender)
	{
		bool flag = displayGender == CommonUtils.EDisplayGender.Female;
		string result;
		if (flag)
		{
			result = "ui9_icon_gender_big_0";
		}
		else
		{
			bool flag2 = displayGender == CommonUtils.EDisplayGender.Male;
			if (flag2)
			{
				result = "ui9_icon_gender_big_1";
			}
			else
			{
				result = "ui9_icon_gender_big_2";
			}
		}
		return result;
	}

	// Token: 0x06000A9D RID: 2717 RVA: 0x000442D0 File Offset: 0x000424D0
	public static string GetCrimeSeverityName(CharacterDisplayDataForSettlementBounty data)
	{
		sbyte severityConfigId = data.SettlementBounty.PunishmentSeverity;
		bool flag = severityConfigId < 0;
		string result;
		if (flag)
		{
			result = "-";
		}
		else
		{
			PunishmentSeverityItem config = PunishmentSeverity.Instance[severityConfigId];
			result = config.Name.SetColor(config.NameColor);
		}
		return result;
	}

	// Token: 0x06000A9E RID: 2718 RVA: 0x0004431C File Offset: 0x0004251C
	public static string GetCrimeNameString(CharacterDisplayDataForSettlementBounty data)
	{
		PunishmentTypeItem punishmentConfig = PunishmentType.Instance[data.SettlementBounty.PunishmentType];
		sbyte severityConfigId = data.SettlementBounty.PunishmentSeverity;
		bool flag = severityConfigId < 0;
		string result;
		if (flag)
		{
			result = "-";
		}
		else
		{
			result = punishmentConfig.Name.SetColor(PunishmentSeverity.Instance[severityConfigId].NameColor);
		}
		return result;
	}

	// Token: 0x06000A9F RID: 2719 RVA: 0x0004437C File Offset: 0x0004257C
	public static string GetHunterStateText(sbyte state)
	{
		if (!true)
		{
		}
		LanguageKey languageKey;
		switch (state)
		{
		case 0:
			languageKey = LanguageKey.LK_HunterState_NotInProgress;
			break;
		case 1:
			languageKey = LanguageKey.LK_HunterState_Hunting;
			break;
		case 2:
			languageKey = LanguageKey.LK_HunterState_Failed;
			break;
		case 3:
			languageKey = LanguageKey.LK_HunterState_Escorting;
			break;
		default:
			throw new ArgumentOutOfRangeException("state", state, null);
		}
		if (!true)
		{
		}
		LanguageKey key = languageKey;
		return LocalStringManager.Get(key).SetColor("orange");
	}

	// Token: 0x06000AA0 RID: 2720 RVA: 0x000443F4 File Offset: 0x000425F4
	public static CommonUtils.EDisplayGender GetDisplayGender(sbyte gender, short templateId)
	{
		CharacterItem characterTemplate = Character.Instance.GetItem(templateId);
		bool flag = characterTemplate != null && characterTemplate.GroupId == 676;
		CommonUtils.EDisplayGender result;
		if (flag)
		{
			result = CommonUtils.EDisplayGender.Hidden;
		}
		else
		{
			result = ((gender == 1) ? CommonUtils.EDisplayGender.Male : CommonUtils.EDisplayGender.Female);
		}
		return result;
	}

	// Token: 0x06000AA1 RID: 2721 RVA: 0x00044438 File Offset: 0x00042638
	public static void AsyncGetDisplayGender(IAsyncMethodRequestHandler requestHandler, int charId, sbyte gender, Action<CommonUtils.EDisplayGender> onGotData)
	{
		bool flag = charId < 0;
		if (flag)
		{
			Action<CommonUtils.EDisplayGender> onGotData2 = onGotData;
			if (onGotData2 != null)
			{
				onGotData2((gender == 1) ? CommonUtils.EDisplayGender.Male : CommonUtils.EDisplayGender.Female);
			}
		}
		else
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(requestHandler, charId, delegate(int offset, RawDataPool dataPool)
			{
				CharacterDisplayData displayData = null;
				Serializer.Deserialize(dataPool, offset, ref displayData);
				bool flag2 = displayData != null;
				if (flag2)
				{
					Action<CommonUtils.EDisplayGender> onGotData3 = onGotData;
					if (onGotData3 != null)
					{
						onGotData3(CommonUtils.GetDisplayGender(displayData.Gender, displayData.TemplateId));
					}
				}
				else
				{
					Action<CommonUtils.EDisplayGender> onGotData4 = onGotData;
					if (onGotData4 != null)
					{
						onGotData4((gender == 1) ? CommonUtils.EDisplayGender.Male : CommonUtils.EDisplayGender.Female);
					}
				}
			});
		}
	}

	// Token: 0x06000AA2 RID: 2722 RVA: 0x00044498 File Offset: 0x00042698
	public static string GetFavorString(short favorability)
	{
		bool flag = favorability == short.MinValue;
		string result;
		if (flag)
		{
			result = "-".SetColor("lightgrey");
		}
		else
		{
			bool flag2 = favorability <= -26000;
			if (flag2)
			{
				result = LocalStringManager.Get(LanguageKey.LK_Favor_Type_0).ColorReplace();
			}
			else
			{
				bool flag3 = favorability <= -22000;
				if (flag3)
				{
					result = LocalStringManager.Get(LanguageKey.LK_Favor_Type_1).ColorReplace();
				}
				else
				{
					bool flag4 = favorability <= -18000;
					if (flag4)
					{
						result = LocalStringManager.Get(LanguageKey.LK_Favor_Type_2).ColorReplace();
					}
					else
					{
						bool flag5 = favorability <= -14000;
						if (flag5)
						{
							result = LocalStringManager.Get(LanguageKey.LK_Favor_Type_3).ColorReplace();
						}
						else
						{
							bool flag6 = favorability <= -10000;
							if (flag6)
							{
								result = LocalStringManager.Get(LanguageKey.LK_Favor_Type_4).ColorReplace();
							}
							else
							{
								bool flag7 = favorability <= -6000;
								if (flag7)
								{
									result = LocalStringManager.Get(LanguageKey.LK_Favor_Type_5).ColorReplace();
								}
								else
								{
									bool flag8 = favorability < 6000;
									if (flag8)
									{
										result = LocalStringManager.Get(LanguageKey.LK_Favor_Type_6).ColorReplace();
									}
									else
									{
										bool flag9 = favorability >= 26000;
										if (flag9)
										{
											result = LocalStringManager.Get(LanguageKey.LK_Favor_Type_12).ColorReplace();
										}
										else
										{
											bool flag10 = favorability >= 22000;
											if (flag10)
											{
												result = LocalStringManager.Get(LanguageKey.LK_Favor_Type_11).ColorReplace();
											}
											else
											{
												bool flag11 = favorability >= 18000;
												if (flag11)
												{
													result = LocalStringManager.Get(LanguageKey.LK_Favor_Type_10).ColorReplace();
												}
												else
												{
													bool flag12 = favorability >= 14000;
													if (flag12)
													{
														result = LocalStringManager.Get(LanguageKey.LK_Favor_Type_9).ColorReplace();
													}
													else
													{
														bool flag13 = favorability >= 10000;
														if (flag13)
														{
															result = LocalStringManager.Get(LanguageKey.LK_Favor_Type_8).ColorReplace();
														}
														else
														{
															bool flag14 = favorability >= 6000;
															if (flag14)
															{
																result = LocalStringManager.Get(LanguageKey.LK_Favor_Type_7).ColorReplace();
															}
															else
															{
																result = "-".SetColor("lightgrey");
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06000AA3 RID: 2723 RVA: 0x000446B8 File Offset: 0x000428B8
	public static string GetFavorStringByInteracted(short favorability, bool isInteracted)
	{
		bool flag = !isInteracted;
		string result;
		if (flag)
		{
			result = "-".SetColor("lightgrey");
		}
		else
		{
			result = CommonUtils.GetFavorString(favorability);
		}
		return result;
	}

	// Token: 0x06000AA4 RID: 2724 RVA: 0x000446EC File Offset: 0x000428EC
	public static string GetFavorStringByLevel(sbyte level)
	{
		level -= -6;
		bool flag = !CommonUtils.FavorabilityLangIdArray.CheckIndex((int)level);
		string result;
		if (flag)
		{
			result = "-".SetColor("lightgrey");
		}
		else
		{
			result = LocalStringManager.Get(CommonUtils.FavorabilityLangIdArray[(int)level]).ColorReplace();
		}
		return result;
	}

	// Token: 0x06000AA5 RID: 2725 RVA: 0x0004473C File Offset: 0x0004293C
	[Obsolete]
	public static string GetFavorIconByInteractedLegacy(short favorability, bool isInteracted)
	{
		bool flag = !isInteracted;
		string result;
		if (flag)
		{
			result = "sp_icon_favorability_unknown";
		}
		else
		{
			result = CommonUtils.GetFavorIconLegacy(favorability);
		}
		return result;
	}

	// Token: 0x06000AA6 RID: 2726 RVA: 0x00044764 File Offset: 0x00042964
	[Obsolete]
	public static string GetFavorIconLegacy(short favorability)
	{
		sbyte favorabilityLevel = FavorabilityType.GetFavorabilityType(favorability);
		return CommonUtils.GetFavorIconByFavorabilityTypeLegacy(favorabilityLevel);
	}

	// Token: 0x06000AA7 RID: 2727 RVA: 0x00044784 File Offset: 0x00042984
	[Obsolete]
	public static string GetFavorIconByFavorabilityTypeLegacy(sbyte favorabilityLevel)
	{
		sbyte index = FavorabilityType.ToIndex(favorabilityLevel);
		bool flag = CommonUtils.FavorabilityIconArray.CheckIndex((int)index);
		string result;
		if (flag)
		{
			result = CommonUtils.FavorabilityIconArray[(int)index];
		}
		else
		{
			result = "sp_icon_favorability_unknown";
		}
		return result;
	}

	// Token: 0x06000AA8 RID: 2728 RVA: 0x000447BC File Offset: 0x000429BC
	public static string GetBehaviorString(sbyte behaviorType)
	{
		bool flag = behaviorType == 0;
		string color;
		if (flag)
		{
			color = "BehaviorType_Just";
		}
		else
		{
			bool flag2 = behaviorType == 1;
			if (flag2)
			{
				color = "BehaviorType_Kind";
			}
			else
			{
				bool flag3 = behaviorType == 2;
				if (flag3)
				{
					color = "BehaviorType_Even";
				}
				else
				{
					bool flag4 = behaviorType == 3;
					if (flag4)
					{
						color = "BehaviorType_Rebel";
					}
					else
					{
						bool flag5 = behaviorType == 4;
						if (!flag5)
						{
							return LocalStringManager.Get(LanguageKey.LK_Goodness_None).SetColor("white");
						}
						color = "BehaviorType_Egoistic";
					}
				}
			}
		}
		return Config.BehaviorType.Instance[(short)behaviorType].Name.SetColor(color);
	}

	// Token: 0x06000AA9 RID: 2729 RVA: 0x00044854 File Offset: 0x00042A54
	public static string GetFameString(sbyte fameType)
	{
		int index = (int)fameType;
		bool flag = fameType == -2;
		if (flag)
		{
			index = CommonUtils.FameStringConfig.Count - 1;
		}
		LanguageKey langId = CommonUtils.FameStringConfig[index];
		return LocalStringManager.Get(langId).ColorReplace();
	}

	// Token: 0x06000AAA RID: 2730 RVA: 0x00044898 File Offset: 0x00042A98
	[Obsolete]
	public static string GetFameIconLegacy(sbyte fameType)
	{
		bool flag = !CommonUtils.FameIconConfig.CheckIndex((int)fameType);
		if (flag)
		{
			fameType = 3;
		}
		return CommonUtils.FameIconConfig[(int)fameType];
	}

	// Token: 0x06000AAB RID: 2731 RVA: 0x000448C8 File Offset: 0x00042AC8
	public static string GetHappinessString(sbyte happinessLevel)
	{
		bool flag = CommonUtils.HappinessLevelConfig.CheckIndex((int)happinessLevel);
		string result;
		if (flag)
		{
			result = LocalStringManager.Get(CommonUtils.HappinessLevelConfig[(int)happinessLevel]).ColorReplace();
		}
		else
		{
			result = LocalStringManager.Get(LanguageKey.LK_Unknown).ColorReplace();
		}
		return result;
	}

	// Token: 0x06000AAC RID: 2732 RVA: 0x0004490C File Offset: 0x00042B0C
	[Obsolete]
	public static string GetHappinessIconLegacy(sbyte happinessLevel)
	{
		bool flag = CommonUtils.HappinessLevelIconConfig.CheckIndex((int)happinessLevel);
		string result;
		if (flag)
		{
			result = LocalStringManager.Get(CommonUtils.HappinessLevelIconConfig[(int)happinessLevel]).ColorReplace();
		}
		else
		{
			result = "sp_icon_happiness_3";
		}
		return result;
	}

	// Token: 0x06000AAD RID: 2733 RVA: 0x00044948 File Offset: 0x00042B48
	public static string GetAgeGrowthString(int growthType, int age)
	{
		AgeEffectItem ageData = AgeEffect.Instance[Mathf.Min(age, AgeEffect.Instance.Count - 1)];
		if (!true)
		{
		}
		sbyte b;
		if (growthType != 0)
		{
			if (growthType != 1)
			{
				b = ageData.SkillQualificationLateBlooming;
			}
			else
			{
				b = ageData.SkillQualificationPrecocious;
			}
		}
		else
		{
			b = ageData.SkillQualificationAverage;
		}
		if (!true)
		{
		}
		int addValue = (int)b;
		StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
		strBuilder.Clear();
		StringBuilder stringBuilder = strBuilder;
		if (!true)
		{
		}
		string value;
		if (growthType != 0)
		{
			if (growthType != 1)
			{
				value = LocalStringManager.Get(LanguageKey.LK_Qualification_Growth_LateBlooming);
			}
			else
			{
				value = LocalStringManager.Get(LanguageKey.LK_Qualification_Growth_Precocious);
			}
		}
		else
		{
			value = LocalStringManager.Get(LanguageKey.LK_Qualification_Growth_Average);
		}
		if (!true)
		{
		}
		stringBuilder.Append(value);
		bool flag = addValue > 0;
		if (flag)
		{
			strBuilder.Append(string.Format(" +{0}", addValue).SetColor("lightblue"));
		}
		else
		{
			bool flag2 = addValue == 0;
			if (flag2)
			{
				strBuilder.Append(" +0".SetColor("lightgrey"));
			}
			else
			{
				strBuilder.Append(string.Format(" {0}", addValue).SetColor("red"));
			}
		}
		string ret = strBuilder.ToString();
		EasyPool.Free<StringBuilder>(strBuilder);
		return ret;
	}

	// Token: 0x06000AAE RID: 2734 RVA: 0x00044A84 File Offset: 0x00042C84
	public static string GetFiveElementsIconByType(sbyte type)
	{
		bool flag = !CommonUtils.CharacterFiveElementsIcons.CheckIndex((int)type);
		if (flag)
		{
			throw new Exception(string.Format("Invalid five elements type : {0}", type));
		}
		return CommonUtils.CharacterFiveElementsIcons[(int)type];
	}

	// Token: 0x06000AAF RID: 2735 RVA: 0x00044AC8 File Offset: 0x00042CC8
	public static string GetFiveElementsNameByType(sbyte type)
	{
		bool flag = !CommonUtils.CharacterFiveElementsNameIds.CheckIndex((int)type);
		if (flag)
		{
			throw new Exception(string.Format("Invalid five elements type to get name : {0}", type));
		}
		return LocalStringManager.Get(CommonUtils.CharacterFiveElementsNameIds[(int)type]);
	}

	// Token: 0x06000AB0 RID: 2736 RVA: 0x00044B10 File Offset: 0x00042D10
	public static int GetCharacterSkillGradeByValue(short value)
	{
		int level = GameData.Domains.Character.SharedMethods.GetCharacterSkillGradeByValue(value);
		bool flag = !Colors.Instance.GradeColors.CheckIndex(level);
		if (flag)
		{
			level = Colors.Instance.GradeColors.Length - 1;
		}
		return level;
	}

	// Token: 0x06000AB1 RID: 2737 RVA: 0x00044B50 File Offset: 0x00042D50
	public static Color GetCharacterSkillColorByValue(short value)
	{
		return Colors.Instance.GradeColors[CommonUtils.GetCharacterSkillGradeByValue(value)];
	}

	// Token: 0x06000AB2 RID: 2738 RVA: 0x00044B68 File Offset: 0x00042D68
	public static Color GetSkillNameColorByIndex(int index)
	{
		bool flag = CommonUtils._skillNameColors == null;
		if (flag)
		{
			CommonUtils._skillNameColors = new Color[]
			{
				Colors.Instance["pinkyellow"],
				Colors.Instance.GradeColors[3],
				Colors.Instance.GradeColors[5],
				Colors.Instance.GradeColors[8]
			};
		}
		return CommonUtils._skillNameColors[index];
	}

	// Token: 0x06000AB3 RID: 2739 RVA: 0x00044BF8 File Offset: 0x00042DF8
	public unsafe static sbyte GetSingleValueCollectionModificationsType(RawDataPool dataPool, int offset)
	{
		int elementsCount;
		return (sbyte)(*(int*)dataPool.GetPointerWithHeader(offset, (uint*)(&elementsCount)));
	}

	// Token: 0x06000AB4 RID: 2740 RVA: 0x00044C18 File Offset: 0x00042E18
	public unsafe static void GetModifiedSingleValueCollectionKeyOfClass<[IsUnmanaged] TKey, TValue>(RawDataPool dataPool, int offset, HashSet<TKey> modifiedKeys) where TKey : struct, ValueType where TValue : class, ISerializableGameData, new()
	{
		int elementsCount;
		byte* pData = dataPool.GetPointerWithHeader(offset, (uint*)(&elementsCount));
		sbyte modificationsType = (sbyte)(*(int*)pData);
		pData += 4;
		modifiedKeys.Clear();
		bool flag = modificationsType == 1 && elementsCount > 0;
		if (flag)
		{
			byte* pCurrData = pData;
			for (int i = 0; i < elementsCount; i++)
			{
				sbyte modificationType = *(sbyte*)pCurrData;
				pCurrData++;
				TKey elementId = *(TKey*)pCurrData;
				pCurrData += sizeof(TKey);
				modifiedKeys.Add(elementId);
				bool flag2 = modificationType == 0 || modificationType == 1;
				if (flag2)
				{
					int subContentSize = *(int*)pCurrData;
					pCurrData += 4;
					bool flag3 = subContentSize > 0;
					if (flag3)
					{
						TValue element = Activator.CreateInstance<TValue>();
						pCurrData += element.Deserialize(pCurrData);
					}
				}
			}
		}
	}

	// Token: 0x06000AB5 RID: 2741 RVA: 0x00044CE0 File Offset: 0x00042EE0
	public unsafe static void GetModifiedSingleValueCollectionKeyOfStruct<[IsUnmanaged] TKey, TValue>(RawDataPool dataPool, int offset, HashSet<TKey> modifiedKeys) where TKey : struct, ValueType where TValue : struct, ISerializableGameData
	{
		int elementsCount;
		byte* pData = dataPool.GetPointerWithHeader(offset, (uint*)(&elementsCount));
		sbyte modificationsType = (sbyte)(*(int*)pData);
		pData += 4;
		modifiedKeys.Clear();
		bool flag = modificationsType == 1 && elementsCount > 0;
		if (flag)
		{
			byte* pCurrData = pData;
			for (int i = 0; i < elementsCount; i++)
			{
				sbyte modificationType = *(sbyte*)pCurrData;
				pCurrData++;
				TKey elementId = *(TKey*)pCurrData;
				pCurrData += sizeof(TKey);
				modifiedKeys.Add(elementId);
				bool flag2 = modificationType == 0 || modificationType == 1;
				if (flag2)
				{
					TValue element = Activator.CreateInstance<TValue>();
					pCurrData += element.Deserialize(pCurrData);
				}
			}
		}
	}

	// Token: 0x06000AB6 RID: 2742 RVA: 0x00044D8C File Offset: 0x00042F8C
	public unsafe static void GetModifiedSingleValueCollectionKeyOfPrimitive<[IsUnmanaged] TKey, [IsUnmanaged] TValue>(RawDataPool dataPool, int offset, HashSet<TKey> modifiedKeys) where TKey : struct, ValueType where TValue : struct, ValueType
	{
		int elementsCount;
		byte* pData = dataPool.GetPointerWithHeader(offset, (uint*)(&elementsCount));
		sbyte modificationsType = (sbyte)(*(int*)pData);
		pData += 4;
		modifiedKeys.Clear();
		bool flag = modificationsType == 1 && elementsCount > 0;
		if (flag)
		{
			byte* pCurrData = pData;
			for (int i = 0; i < elementsCount; i++)
			{
				sbyte modificationType = *(sbyte*)pCurrData;
				pCurrData++;
				TKey elementId = *(TKey*)pCurrData;
				pCurrData += sizeof(TKey);
				modifiedKeys.Add(elementId);
				bool flag2 = modificationType == 0 || modificationType == 1;
				if (flag2)
				{
					pCurrData += sizeof(TValue);
				}
			}
		}
	}

	// Token: 0x06000AB7 RID: 2743 RVA: 0x00044E28 File Offset: 0x00043028
	public static string GetCharmString(sbyte charmType, sbyte gender)
	{
		if (!true)
		{
		}
		string result;
		switch (charmType)
		{
		case -3:
			result = LanguageKey.LK_Charm_Naked.Tr().SetColor("GradeColor_0");
			break;
		case -2:
			result = LanguageKey.LK_Charm_Childish.Tr().SetColor("GradeColor_0");
			break;
		case -1:
			result = LanguageKey.LK_Unknow.Tr().SetColor("GradeColor_0");
			break;
		default:
			result = LocalStringManager.Get((CommonUtils.CharmLevelDisplayLanguageIds[(int)charmType].Length > 1) ? CommonUtils.CharmLevelDisplayLanguageIds[(int)charmType][(int)gender] : CommonUtils.CharmLevelDisplayLanguageIds[(int)charmType][0]).ColorReplace();
			break;
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06000AB8 RID: 2744 RVA: 0x00044ECC File Offset: 0x000430CC
	public static string GetCharmLevelText(short charm, sbyte gender, short age, short clothDisplayId, bool isFixedCharacter = false, bool faceVisible = true)
	{
		bool flag = !faceVisible;
		string result;
		if (flag)
		{
			result = LocalStringManager.Get(LanguageKey.LK_Unknow).SetColor("GradeColor_0");
		}
		else
		{
			bool flag2 = !isFixedCharacter;
			if (flag2)
			{
				bool flag3 = age < 16;
				if (flag3)
				{
					return LocalStringManager.Get(LanguageKey.LK_Charm_Childish).SetColor("GradeColor_0");
				}
				bool flag4 = clothDisplayId == 0;
				if (flag4)
				{
					return LocalStringManager.Get(LanguageKey.LK_Charm_Naked).SetColor("GradeColor_0");
				}
			}
			byte charmLevel = 0;
			for (int i = 0; i < AvatarData.CharmLevel.Length; i++)
			{
				bool flag5 = charm < AvatarData.CharmLevel[i];
				if (flag5)
				{
					break;
				}
				charmLevel += 1;
			}
			bool flag6 = !CommonUtils.CharmLevelDisplayLanguageIds.CheckIndex((int)charmLevel);
			if (flag6)
			{
				charmLevel = 8;
			}
			LanguageKey[] charmLevelKeyArray = CommonUtils.CharmLevelDisplayLanguageIds[(int)charmLevel];
			LanguageKey langId = charmLevelKeyArray[0];
			bool flag7 = charmLevelKeyArray.Length > 1;
			if (flag7)
			{
				langId = charmLevelKeyArray[(int)gender];
			}
			result = LocalStringManager.Get(langId).ColorReplace();
		}
		return result;
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x00044FD0 File Offset: 0x000431D0
	[Obsolete]
	public static string GetCharmLevelIconLegacy(short charm, short age, short clothDisplayId, bool faceVisible)
	{
		bool flag = age < 16 || !faceVisible;
		string result;
		if (flag)
		{
			result = "sp_icon_charm_126";
		}
		else
		{
			bool flag2 = clothDisplayId == 0;
			if (flag2)
			{
				result = "sp_icon_charm_127";
			}
			else
			{
				byte charmLevel = 0;
				for (int i = 0; i < AvatarData.CharmLevel.Length; i++)
				{
					bool flag3 = charm < AvatarData.CharmLevel[i];
					if (flag3)
					{
						break;
					}
					charmLevel += 1;
				}
				bool flag4 = !CommonUtils.CharmLevelDisplayIcons.CheckIndex((int)charmLevel);
				if (flag4)
				{
					charmLevel = 8;
				}
				result = CommonUtils.CharmLevelDisplayIcons[(int)charmLevel];
			}
		}
		return result;
	}

	// Token: 0x06000ABA RID: 2746 RVA: 0x00045060 File Offset: 0x00043260
	public static byte GetCharmLevel(short charm)
	{
		byte charmLevel = 0;
		for (int i = 0; i < AvatarData.CharmLevel.Length; i++)
		{
			bool flag = charm < AvatarData.CharmLevel[i];
			if (flag)
			{
				break;
			}
			charmLevel += 1;
		}
		return charmLevel;
	}

	// Token: 0x06000ABB RID: 2747 RVA: 0x000450A4 File Offset: 0x000432A4
	public static int GetCharmIconIndex(short charm, short age, short clothDisplayId, bool faceVisible, bool isFixedCharacter)
	{
		bool flag = !isFixedCharacter;
		if (flag)
		{
			bool flag2 = age < 16 || !faceVisible;
			if (flag2)
			{
				return 0;
			}
			bool flag3 = clothDisplayId == 0;
			if (flag3)
			{
				return 0;
			}
		}
		byte charmLevel = CommonUtils.GetCharmLevel(charm);
		if (!true)
		{
		}
		int result;
		switch (charmLevel)
		{
		case 0:
		case 1:
		case 2:
			result = 0;
			break;
		case 3:
			result = 1;
			break;
		case 4:
			result = 2;
			break;
		case 5:
			result = 3;
			break;
		case 6:
			result = 4;
			break;
		case 7:
			result = 5;
			break;
		case 8:
			result = 6;
			break;
		default:
			result = 6;
			break;
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06000ABC RID: 2748 RVA: 0x00045148 File Offset: 0x00043348
	public static string GetCharmLevelBigIcon(short charm, short age, short clothDisplayId, bool faceVisible, bool isFixedCharacter)
	{
		string prefix = "ui9_icon_charm_big_";
		return prefix + CommonUtils.GetCharmIconIndex(charm, age, clothDisplayId, faceVisible, isFixedCharacter).ToString();
	}

	// Token: 0x06000ABD RID: 2749 RVA: 0x0004517C File Offset: 0x0004337C
	public static string GetCharmLevelSmallIcon(short charm, short age, short clothDisplayId, bool faceVisible, bool isFixedCharacter)
	{
		string prefix = "ui9_icon_charm_small_";
		return prefix + CommonUtils.GetCharmIconIndex(charm, age, clothDisplayId, faceVisible, isFixedCharacter).ToString();
	}

	// Token: 0x06000ABE RID: 2750 RVA: 0x000451B0 File Offset: 0x000433B0
	public static int GetBehaviorTypeIconIndex(sbyte behaviorType)
	{
		return (int)MathUtils.Clamp(behaviorType, 0, 4);
	}

	// Token: 0x06000ABF RID: 2751 RVA: 0x000451CC File Offset: 0x000433CC
	public static string GetBehaviorTypeIcon(sbyte behaviorType)
	{
		return "ui9_icon_behavior_type_" + CommonUtils.GetBehaviorTypeIconIndex(behaviorType).ToString();
	}

	// Token: 0x06000AC0 RID: 2752 RVA: 0x000451F8 File Offset: 0x000433F8
	public static string GetBehaviorTypeBigIcon(sbyte behaviorType)
	{
		return "ui9_icon_stance_big_" + CommonUtils.GetBehaviorTypeIconIndex(behaviorType).ToString();
	}

	// Token: 0x06000AC1 RID: 2753 RVA: 0x00045224 File Offset: 0x00043424
	public static int GetIdentityIconIndex(sbyte level)
	{
		return (int)MathUtils.Clamp(level, 0, 8);
	}

	// Token: 0x06000AC2 RID: 2754 RVA: 0x00045240 File Offset: 0x00043440
	public static string GetIdentityIconName(sbyte level)
	{
		return "ui9_icon_identity_big_" + CommonUtils.GetIdentityIconIndex(level).ToString();
	}

	// Token: 0x06000AC3 RID: 2755 RVA: 0x0004526C File Offset: 0x0004346C
	public static int GetFameIconIndex(sbyte fameType)
	{
		bool flag = fameType < 0;
		int result;
		if (flag)
		{
			result = 3;
		}
		else
		{
			result = (int)MathUtils.Clamp(fameType, 0, 6);
		}
		return result;
	}

	// Token: 0x06000AC4 RID: 2756 RVA: 0x00045294 File Offset: 0x00043494
	public static string GetFameIconName(sbyte fameType)
	{
		return "ui9_icon_fame_big_" + CommonUtils.GetFameIconIndex(fameType).ToString();
	}

	// Token: 0x06000AC5 RID: 2757 RVA: 0x000452C0 File Offset: 0x000434C0
	public static int GetHappinessIconIndex(sbyte happinessLevel)
	{
		return (int)MathUtils.Clamp(happinessLevel, 0, 6);
	}

	// Token: 0x06000AC6 RID: 2758 RVA: 0x000452DC File Offset: 0x000434DC
	public static string GetHappinessIconName(sbyte happinessLevel)
	{
		return "ui9_icon_happiness_big_" + CommonUtils.GetHappinessIconIndex(happinessLevel).ToString();
	}

	// Token: 0x06000AC7 RID: 2759 RVA: 0x00045308 File Offset: 0x00043508
	public static int GetFavorabilityIconIndex(short favorability, bool isInteracted)
	{
		bool flag = !isInteracted;
		int result;
		if (flag)
		{
			result = 0;
		}
		else
		{
			sbyte level = FavorabilityType.GetFavorabilityType(favorability);
			result = CommonUtils.GetFavorabilityIconIndexByLevel(level, true);
		}
		return result;
	}

	// Token: 0x06000AC8 RID: 2760 RVA: 0x00045334 File Offset: 0x00043534
	public static int GetFavorabilityIconIndexByLevel(sbyte level, bool isInteracted)
	{
		bool flag = !isInteracted;
		int result;
		if (flag)
		{
			result = 0;
		}
		else
		{
			sbyte index = FavorabilityType.ToIndex(level);
			if (!true)
			{
			}
			int num;
			if (index > 5)
			{
				switch (index)
				{
				case 6:
					num = 2;
					break;
				case 7:
					num = 3;
					break;
				case 8:
					num = 4;
					break;
				case 9:
					num = 5;
					break;
				case 10:
					num = 6;
					break;
				case 11:
					num = 7;
					break;
				case 12:
					num = 8;
					break;
				default:
					num = 1;
					break;
				}
			}
			else
			{
				num = 1;
			}
			if (!true)
			{
			}
			result = num;
		}
		return result;
	}

	// Token: 0x06000AC9 RID: 2761 RVA: 0x000453B0 File Offset: 0x000435B0
	public static string GetFavorabilityIconName(short favorability, bool isInteracted)
	{
		return "ui9_icon_favorability_big_" + CommonUtils.GetFavorabilityIconIndex(favorability, isInteracted).ToString();
	}

	// Token: 0x06000ACA RID: 2762 RVA: 0x000453DC File Offset: 0x000435DC
	public static string GetFavorabilityLevelIconName(sbyte favorabilityLevel, bool isInteracted)
	{
		return "ui9_icon_favorability_big_" + CommonUtils.GetFavorabilityIconIndexByLevel(favorabilityLevel, isInteracted).ToString();
	}

	// Token: 0x06000ACB RID: 2763 RVA: 0x00045408 File Offset: 0x00043608
	public static string GetConsummateIcon(sbyte consummateLevel)
	{
		return string.Format("ui9_icon_consummate_level_big_{0}", consummateLevel);
	}

	// Token: 0x06000ACC RID: 2764 RVA: 0x0004542C File Offset: 0x0004362C
	public static string GetOrganizationIcon(short templateId)
	{
		OrganizationItem config = Config.Organization.Instance[(int)templateId];
		bool isSect = config.IsSect;
		string result;
		if (isSect)
		{
			sbyte goodness = config.Goodness;
			if (!true)
			{
			}
			string text;
			switch (goodness)
			{
			case -1:
				text = "ui9_icon_organization_big_bad_sect";
				break;
			case 0:
				text = "ui9_icon_organization_big_normal_sect";
				break;
			case 1:
				text = "ui9_icon_organization_big_good_sect";
				break;
			default:
				if (!true)
				{
				}
				<PrivateImplementationDetails>.ThrowSwitchExpressionException(goodness);
				break;
			}
			if (!true)
			{
			}
			result = text;
		}
		else
		{
			bool flag = templateId == 16;
			if (flag)
			{
				result = "ui9_icon_organization_big_taiwu_village";
			}
			else
			{
				result = "ui9_icon_organization_big_other";
			}
		}
		return result;
	}

	// Token: 0x06000ACD RID: 2765 RVA: 0x000454C4 File Offset: 0x000436C4
	public static string GetAlertnessIcon(int level)
	{
		level = (int)CharacterAlertnessData.ValidateLevel(level);
		return "ui9_icon_alertness_big_" + level.ToString();
	}

	// Token: 0x06000ACE RID: 2766 RVA: 0x000454F0 File Offset: 0x000436F0
	public static string GetAlertnessName(int level)
	{
		bool flag = level < 0;
		string result;
		if (flag)
		{
			result = "-";
		}
		else
		{
			level = (int)CharacterAlertnessData.ValidateLevel(level);
			result = LocalStringManager.Get(LanguageKey.LK_Alertness_Level_0 + level).SetColor(Colors.Instance.AlertnessColors[level]);
		}
		return result;
	}

	// Token: 0x06000ACF RID: 2767 RVA: 0x0004553C File Offset: 0x0004373C
	public static string GetAlertnessNameByValue(int value)
	{
		sbyte level = CharacterAlertnessData.GetLevel(value);
		return CommonUtils.GetAlertnessName((int)level);
	}

	// Token: 0x06000AD0 RID: 2768 RVA: 0x0004555C File Offset: 0x0004375C
	public static string GetSkillGrowthString(int growthType, int actualAge)
	{
		int addValue = (int)CommonUtils.GetSkillGrowthAddValue(growthType, actualAge);
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

	// Token: 0x06000AD1 RID: 2769 RVA: 0x00045628 File Offset: 0x00043828
	public static sbyte GetSkillGrowthAddValue(int growthType, int actualAge)
	{
		AgeEffectItem ageData = AgeEffect.Instance[Mathf.Min(actualAge, AgeEffect.Instance.Count - 1)];
		if (!true)
		{
		}
		sbyte result;
		if (growthType != 0)
		{
			if (growthType != 1)
			{
				result = ageData.SkillQualificationLateBlooming;
			}
			else
			{
				result = ageData.SkillQualificationPrecocious;
			}
		}
		else
		{
			result = ageData.SkillQualificationAverage;
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06000AD2 RID: 2770 RVA: 0x00045688 File Offset: 0x00043888
	public static string GetOrganizationExtraDescString(sbyte orgTemplateId)
	{
		OrganizationItem configData = Config.Organization.Instance[orgTemplateId];
		return LocalStringManager.GetFormat(LanguageKey.LK_Organization_ExtraDescFormat, new object[]
		{
			configData.Name,
			LocalStringManager.Get((configData.Goodness == 1) ? LanguageKey.LK_Organization_ExtraDescGood : ((configData.Goodness == -1) ? LanguageKey.LK_Organization_ExtraDescEvil : LanguageKey.LK_Organization_ExtraDescNormal)),
			LocalStringManager.Get(string.Format("LK_Goodness_{0}", GameData.Domains.Character.BehaviorType.GetBehaviorType(configData.MainMorality))),
			LocalStringManager.Get(string.Format("LK_Goodness_{0}", GameData.Domains.Character.BehaviorType.GetBehaviorType(configData.MainMorality))),
			(configData.Goodness == 0) ? string.Empty : LocalStringManager.Get((configData.Goodness == 1) ? LanguageKey.LK_Organization_ExtraDescFameLimitLow : LanguageKey.LK_Organization_ExtraDescFameLimitHigh)
		});
	}

	// Token: 0x06000AD3 RID: 2771 RVA: 0x0004575C File Offset: 0x0004395C
	public static string GetOrganizationGradeString(OrganizationInfo orgInfo, sbyte gender, short physiologicalAge = -1, int characterTemplateId = -1)
	{
		bool flag = GameData.Domains.World.SharedMethods.SmallVillageXiangshu((short)orgInfo.OrgTemplateId, true);
		string result;
		if (flag)
		{
			result = CommonUtils.GetSmallVillageXiangshuOrgGradeString((short)orgInfo.OrgTemplateId);
		}
		else
		{
			string name;
			bool flag2 = CommonUtils.TryGetCharacterSpecialGradeName(characterTemplateId, out name);
			if (flag2)
			{
				result = name.SetGradeColor((int)orgInfo.Grade);
			}
			else
			{
				WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
				string orgName = mapModel.GetSettlementName(orgInfo);
				bool flag3 = string.IsNullOrEmpty(orgName);
				if (flag3)
				{
					orgName = Config.Organization.Instance[orgInfo.OrgTemplateId].Name.SetColor("lightbrown");
				}
				string gradeName = CommonUtils.GetIdentityString(orgInfo, gender, physiologicalAge, false);
				result = LocalStringManager.GetFormat(LanguageKey.LK_Identity_OrgGrade, orgName, gradeName);
			}
		}
		return result;
	}

	// Token: 0x06000AD4 RID: 2772 RVA: 0x00045800 File Offset: 0x00043A00
	public static bool TryGetCharacterSpecialGradeName(int templateId, out string res)
	{
		res = null;
		bool flag = templateId < 0;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			res = Character.Instance[templateId].SpecialGradeName;
			result = !string.IsNullOrEmpty(res);
		}
		return result;
	}

	// Token: 0x06000AD5 RID: 2773 RVA: 0x00045840 File Offset: 0x00043A40
	public static string GetSmallVillageXiangshuOrgGradeString(short orgTemplateId)
	{
		bool flag = orgTemplateId == 19;
		string result;
		if (flag)
		{
			result = LocalStringManager.Get(LanguageKey.LK_SmallVillage_InfectedOrgAnonymous);
		}
		else
		{
			bool flag2 = orgTemplateId == 20;
			if (flag2)
			{
				result = LocalStringManager.Get(LanguageKey.LK_SmallVillage_InfectedAnonymous);
			}
			else
			{
				result = LocalStringManager.Get(LanguageKey.LK_SmallVillage_InfectedOrgAnonymous);
			}
		}
		return result;
	}

	// Token: 0x06000AD6 RID: 2774 RVA: 0x00045888 File Offset: 0x00043A88
	public static string GetXiangshuMinion0AnonymousTitle()
	{
		return Character.Instance[366].AnonymousTitle;
	}

	// Token: 0x06000AD7 RID: 2775 RVA: 0x000458B0 File Offset: 0x00043AB0
	public static string GetIdentityStringWithSpecialCharacterConfig(int characterTemlateId, OrganizationInfo orgInfo, sbyte gender, short physiologicalAge, bool isReclusive = false)
	{
		CharacterItem characterConfig = Character.Instance[characterTemlateId];
		return (!characterConfig.SpecialGradeName.IsNullOrEmpty()) ? characterConfig.SpecialGradeName.SetGradeColor((int)orgInfo.Grade) : CommonUtils.GetIdentityString(orgInfo, gender, physiologicalAge, isReclusive);
	}

	// Token: 0x06000AD8 RID: 2776 RVA: 0x000458F8 File Offset: 0x00043AF8
	public static string GetIdentityString(OrganizationInfo orgInfo, sbyte gender, short physiologicalAge, bool isReclusive = false)
	{
		OrganizationMemberItem memberConfig = OrganizationMember.Instance[Config.Organization.Instance[orgInfo.OrgTemplateId].Members[(int)orgInfo.Grade]];
		string identityString = "";
		bool flag = physiologicalAge >= 0 && physiologicalAge < memberConfig.IdentityActiveAge;
		if (flag)
		{
			identityString = LocalStringManager.Get((AgeGroup.GetAgeGroup(physiologicalAge) == 0) ? LanguageKey.LK_Baby : ((gender == 0) ? LanguageKey.LK_Girl : LanguageKey.LK_Boy));
		}
		else
		{
			bool flag2 = orgInfo.OrgTemplateId != 0;
			if (flag2)
			{
				identityString = (orgInfo.Principal ? memberConfig.GradeName : memberConfig.SpouseAnonymousTitles[(int)gender]);
				if (isReclusive)
				{
					identityString = LocalStringManager.GetFormat(LanguageKey.LK_ReclusivedCharPrefix, identityString);
				}
			}
		}
		bool flag3 = GameData.Domains.World.SharedMethods.SmallVillageXiangshu((short)orgInfo.OrgTemplateId, true);
		string result;
		if (flag3)
		{
			result = CommonUtils.GetSmallVillageXiangshuOrgGradeString((short)orgInfo.OrgTemplateId);
		}
		else
		{
			result = identityString.SetGradeColor((int)orgInfo.Grade);
		}
		return result;
	}

	// Token: 0x06000AD9 RID: 2777 RVA: 0x000459E0 File Offset: 0x00043BE0
	public static string GetSettlementBountyIcon(int settlementId)
	{
		bool flag = settlementId < 0;
		string result;
		if (flag)
		{
			result = "ui9_icon_resource_big_6";
		}
		else
		{
			sbyte goodness = Config.Organization.Instance[settlementId].Goodness;
			sbyte b = goodness;
			if (b != 0)
			{
				if (b != 1)
				{
					result = "ui9_icon_resource_big_6";
				}
				else
				{
					result = "ui9_icon_resource_big_7";
				}
			}
			else
			{
				result = "ui9_icon_resource_big_8";
			}
		}
		return result;
	}

	// Token: 0x06000ADA RID: 2778 RVA: 0x00045A38 File Offset: 0x00043C38
	public static bool CheckCharIsChild(OrganizationInfo orgInfo, short physiologicalAge)
	{
		OrganizationMemberItem memberConfig = OrganizationMember.Instance[Config.Organization.Instance[orgInfo.OrgTemplateId].Members[(int)orgInfo.Grade]];
		return physiologicalAge >= 0 && physiologicalAge < memberConfig.IdentityActiveAge;
	}

	// Token: 0x06000ADB RID: 2779 RVA: 0x00045A84 File Offset: 0x00043C84
	public static bool IsTaiwuVillageSubordinateOrganization(OrganizationInfo orgInfo)
	{
		bool flag = orgInfo.OrgTemplateId == 16;
		return flag || orgInfo.SettlementId == SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageSettlementId();
	}

	// Token: 0x06000ADC RID: 2780 RVA: 0x00045ABA File Offset: 0x00043CBA
	public static bool ShouldDisplayOrganizationMemberPotentialSuccessor(CharacterDisplayData data)
	{
		return ((data != null) ? data.OrganizationMemberPotentialSuccessor : null) != null && !CommonUtils.IsTaiwuVillageSubordinateOrganization(data.OrgInfo);
	}

	// Token: 0x06000ADD RID: 2781 RVA: 0x00045ADC File Offset: 0x00043CDC
	public static bool TryGetOrganizationMemberPotentialSuccessorDisplay(CharacterDisplayData data, out string successorName, out string successorIdentity)
	{
		successorName = null;
		successorIdentity = null;
		bool flag = data == null || CommonUtils.IsTaiwuVillageSubordinateOrganization(data.OrgInfo);
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			CharacterDisplayData successor = data.OrganizationMemberPotentialSuccessor;
			bool flag2 = successor == null || successor.CharacterId < 0;
			if (flag2)
			{
				result = false;
			}
			else
			{
				successorName = NameCenter.GetMonasticTitleOrDisplayName(successor, false).SetColor("pinkyellow");
				successorIdentity = CommonUtils.GetIdentityStringWithSpecialCharacterConfig((int)successor.TemplateId, successor.OrgInfo, successor.Gender, successor.PhysiologicalAge, false);
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06000ADE RID: 2782 RVA: 0x00045B60 File Offset: 0x00043D60
	public static void AppendOrganizationMemberPotentialSuccessorIdentityTip(StringBuilder sb, CharacterDisplayData data)
	{
		string successorName;
		string successorIdentity;
		bool flag = !CommonUtils.TryGetOrganizationMemberPotentialSuccessorDisplay(data, out successorName, out successorIdentity);
		if (!flag)
		{
			sb.AppendLine();
			sb.Append(LanguageKey.LK_Main_SummaryInfo_Identity_PrefixDot.Tr());
			sb.AppendFormat(LocalStringManager.Get("LK_RelationShip_OrganizationMemberPotentialSuccessor"), successorName, successorIdentity);
		}
	}

	// Token: 0x06000ADF RID: 2783 RVA: 0x00045BAC File Offset: 0x00043DAC
	public static bool TryFormatOrganizationMemberPotentialSuccessorRelationTitleTip(CharacterDisplayData data, bool hasKnownSuccessor, out string tip)
	{
		tip = null;
		bool flag = data == null || CommonUtils.IsTaiwuVillageSubordinateOrganization(data.OrgInfo);
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			OrganizationItem orgCfg = Config.Organization.Instance[data.OrgInfo.OrgTemplateId];
			bool flag2 = orgCfg == null;
			if (flag2)
			{
				result = false;
			}
			else
			{
				string holderName = NameCenter.GetMonasticTitleOrDisplayName(data, false).SetColor("pinkyellow");
				string positionName = CommonUtils.GetOrganizationGradeString(data.OrgInfo, data.Gender, data.PhysiologicalAge, (int)data.TemplateId).SetColor("pinkyellow");
				string key = hasKnownSuccessor ? "LK_RelationShip_OrganizationMemberPotentialSuccessor_tip_A" : "LK_RelationShip_OrganizationMemberPotentialSuccessor_tip_B";
				tip = string.Format(LocalStringManager.Get(key), holderName, positionName);
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06000AE0 RID: 2784 RVA: 0x00045C64 File Offset: 0x00043E64
	public static bool CheckCharIsMerchant(OrganizationInfo orgInfo)
	{
		OrganizationMemberItem memberConfig = OrganizationMember.Instance[Config.Organization.Instance[orgInfo.OrgTemplateId].Members[(int)orgInfo.Grade]];
		sbyte type = 4;
		return memberConfig.IdentityInteractConfig.Contains(type);
	}

	// Token: 0x06000AE1 RID: 2785 RVA: 0x00045CAC File Offset: 0x00043EAC
	public static string GetCharacterGradeString(OrganizationInfo orgInfo, sbyte gender, short physiologicalAge = -1)
	{
		OrganizationMemberItem memberConfig = OrganizationMember.Instance[Config.Organization.Instance[orgInfo.OrgTemplateId].Members[(int)orgInfo.Grade]];
		bool flag = physiologicalAge >= 0 && physiologicalAge < memberConfig.IdentityActiveAge;
		string gradeName;
		if (flag)
		{
			gradeName = LocalStringManager.Get((AgeGroup.GetAgeGroup(physiologicalAge) == 0) ? LanguageKey.LK_Baby : ((gender == 0) ? LanguageKey.LK_Girl : LanguageKey.LK_Boy));
			gradeName = string.Concat(new string[]
			{
				"<color=",
				Colors.Instance.GradeColors[(int)orgInfo.Grade].ColorToHexString("#"),
				">",
				gradeName,
				"</color>"
			});
		}
		else
		{
			gradeName = (orgInfo.Principal ? memberConfig.GradeName : memberConfig.SpouseAnonymousTitles[(int)gender]);
			gradeName = string.Concat(new string[]
			{
				"<color=",
				Colors.Instance.GradeColors[(int)orgInfo.Grade].ColorToHexString("#"),
				">",
				gradeName,
				"</color>"
			});
		}
		return gradeName;
	}

	// Token: 0x06000AE2 RID: 2786 RVA: 0x00045DD8 File Offset: 0x00043FD8
	public static string GetDurabilityString(int currentValue, int maxValue)
	{
		bool flag = maxValue == 0;
		string result2;
		if (flag)
		{
			result2 = "-";
		}
		else
		{
			StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
			bool flag2 = currentValue <= maxValue / 2;
			string color;
			if (flag2)
			{
				color = "brightred";
			}
			else
			{
				color = "pinkyellow";
			}
			strBuilder.Clear();
			strBuilder.Append(currentValue.ToString().SetColor(color));
			strBuilder.Append(string.Format("/{0}", maxValue).ToString().SetColor("pinkyellow"));
			string result = strBuilder.ToString();
			EasyPool.Free<StringBuilder>(strBuilder);
			result2 = result;
		}
		return result2;
	}

	// Token: 0x06000AE3 RID: 2787 RVA: 0x00045E70 File Offset: 0x00044070
	public static string GetRequireValueColor(int needValue, int currentValue, int maxValue = -1)
	{
		bool flag = maxValue < 0;
		if (flag)
		{
			maxValue = needValue * 2;
		}
		bool flag2 = currentValue >= maxValue;
		string result;
		if (flag2)
		{
			result = "lightblue";
		}
		else
		{
			bool flag3 = currentValue >= needValue;
			if (flag3)
			{
				result = "lightgreen";
			}
			else
			{
				bool flag4 = currentValue >= needValue / 2;
				if (flag4)
				{
					result = "yellow";
				}
				else
				{
					result = "red";
				}
			}
		}
		return result;
	}

	// Token: 0x06000AE4 RID: 2788 RVA: 0x00045ED4 File Offset: 0x000440D4
	public static string GetLeftValueColor(int leftValue, int totalValue)
	{
		bool flag = leftValue == 0 && totalValue == 0;
		string result;
		if (flag)
		{
			result = "lightgrey";
		}
		else
		{
			bool flag2 = leftValue <= totalValue / 2;
			if (flag2)
			{
				result = "red";
			}
			else
			{
				bool flag3 = leftValue < totalValue;
				if (flag3)
				{
					result = "lightyellow";
				}
				else
				{
					result = "lightgreen";
				}
			}
		}
		return result;
	}

	// Token: 0x06000AE5 RID: 2789 RVA: 0x00045F28 File Offset: 0x00044128
	public static string GetWeightString(int currLoad, int maxLoad)
	{
		string color = CommonUtils.GetLoadWeightValueColor(currLoad, maxLoad);
		string currLoadStr = ((float)currLoad / 100f).ToString("f1").SetColor(color);
		return string.Format("{0}/{1:f1}", currLoadStr, (float)maxLoad / 100f);
	}

	// Token: 0x06000AE6 RID: 2790 RVA: 0x00045F78 File Offset: 0x00044178
	public static string GetLoadWeightValueColor(int currLoad, int maxLoad)
	{
		bool flag = currLoad == 0 && maxLoad == 0;
		string result;
		if (flag)
		{
			result = "lightgrey";
		}
		else
		{
			bool flag2 = currLoad > maxLoad;
			if (flag2)
			{
				result = "red";
			}
			else
			{
				bool flag3 = currLoad >= maxLoad / 2;
				if (flag3)
				{
					result = "lightyellow";
				}
				else
				{
					result = "white";
				}
			}
		}
		return result;
	}

	// Token: 0x06000AE7 RID: 2791 RVA: 0x00045FCC File Offset: 0x000441CC
	public static string GetMoneyValueColor(bool isAdd)
	{
		return isAdd ? "brightblue" : "brightred";
	}

	// Token: 0x06000AE8 RID: 2792 RVA: 0x00045FF0 File Offset: 0x000441F0
	public static string GetAddReduceString(int delta)
	{
		bool flag = delta == 0;
		string result;
		if (flag)
		{
			result = string.Format("+{0}", delta).SetColor("pinkyellow");
		}
		else
		{
			bool flag2 = delta > 0;
			if (flag2)
			{
				result = string.Format("+{0}", delta).ToString().SetColor("brightblue");
			}
			else
			{
				result = delta.ToString().SetColor("brightred");
			}
		}
		return result;
	}

	// Token: 0x06000AE9 RID: 2793 RVA: 0x00046064 File Offset: 0x00044264
	public static string GetPersonalityColor(int personalityType)
	{
		string result;
		switch (personalityType)
		{
		case 0:
			result = "PersonalityType_Calm";
			break;
		case 1:
			result = "PersonalityType_Clever";
			break;
		case 2:
			result = "PersonalityType_Enthusiastic";
			break;
		case 3:
			result = "PersonalityType_Brave";
			break;
		case 4:
			result = "PersonalityType_Firm";
			break;
		case 5:
			result = "PersonalityType_Lucky";
			break;
		case 6:
			result = "PersonalityType_Perceptive";
			break;
		default:
			result = "grey";
			break;
		}
		return result;
	}

	// Token: 0x06000AEA RID: 2794 RVA: 0x000460DC File Offset: 0x000442DC
	public static string GetColoredStringByCompare(int x, int y, int ord, bool largeNumberWithChinese = false)
	{
		string color = (ord == 0) ? "pinkyellow" : ((ord > 0) ? "brightred" : "brightblue");
		string xString = largeNumberWithChinese ? CommonUtils.GetDisplayStringForNum(x, 100000) : x.ToString();
		string yString = largeNumberWithChinese ? CommonUtils.GetDisplayStringForNum(y, 100000) : y.ToString();
		return xString.SetColor(color) + "/" + yString;
	}

	// Token: 0x06000AEB RID: 2795 RVA: 0x0004614C File Offset: 0x0004434C
	public static string GetDisplayStringForNum(int num, int threshold = 100000)
	{
		string numStr = num.ToString();
		bool flag = Math.Abs(num) >= threshold;
		if (flag)
		{
			numStr = string.Format("{0}{1}", num / 10000, LocalStringManager.Get(LanguageKey.LK_Num_Ten_Thousand));
		}
		bool flag2 = Math.Abs(num) >= 100000000;
		if (flag2)
		{
			numStr = string.Format("{0}{1}", num / 100000000, LocalStringManager.Get(LanguageKey.LK_Num_OneHundredMillion));
		}
		return numStr;
	}

	// Token: 0x06000AEC RID: 2796 RVA: 0x000461D4 File Offset: 0x000443D4
	public static string GetDisplayStringForNum(long num)
	{
		string numStr = num.ToString();
		int threshold = 100000;
		bool flag = Math.Abs(num) >= (long)threshold;
		if (flag)
		{
			numStr = string.Format("{0}{1}", num / 10000L, LocalStringManager.Get(LanguageKey.LK_Num_Ten_Thousand));
		}
		bool flag2 = Math.Abs(num) >= 100000000L;
		if (flag2)
		{
			numStr = string.Format("{0}{1}", num / 100000000L, LocalStringManager.Get(LanguageKey.LK_Num_OneHundredMillion));
		}
		return numStr;
	}

	// Token: 0x06000AED RID: 2797 RVA: 0x00046268 File Offset: 0x00044468
	public static bool CanItemEat(sbyte itemType, short itemTemplateId)
	{
		bool flag = itemType == 7 || itemType == 9 || itemType == 8;
		return flag || ItemTemplateHelper.IsTianJieFuLu(itemType, itemTemplateId) || CommonUtils.CanItemEatForChangeNeili(itemType, itemTemplateId) || CommonUtils.CanItemEatForChangeMaxNeili(itemType, itemTemplateId);
	}

	// Token: 0x06000AEE RID: 2798 RVA: 0x000462B0 File Offset: 0x000444B0
	public static bool CanItemEat(sbyte itemType, short itemTemplateId, int charId)
	{
		return CommonUtils.CanItemEat(itemType, itemTemplateId) || CommonUtils.CanMaterialEat(itemType, itemTemplateId, charId);
	}

	// Token: 0x06000AEF RID: 2799 RVA: 0x000462D8 File Offset: 0x000444D8
	public static bool IsTianSuiBaoLuItem(sbyte itemType, short itemTemplateId)
	{
		return itemType == 12 && (itemTemplateId == 254 || itemTemplateId == 255 || itemTemplateId == 256 || itemTemplateId == 257 || itemTemplateId == 258 || itemTemplateId == 259 || itemTemplateId == 260 || itemTemplateId == 261 || itemTemplateId == 262 || itemTemplateId == 263);
	}

	// Token: 0x06000AF0 RID: 2800 RVA: 0x00046348 File Offset: 0x00044548
	public static bool CanMaterialEat(sbyte itemType, short itemTemplateId, int charId)
	{
		bool flag = charId != SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			ProfessionModel professionModel = SingletonObject.getInstance<ProfessionModel>();
			result = (professionModel.IsSkillEquipped(54) && itemType == 5 && Config.Material.Instance[itemTemplateId].Grade > 6 && Config.Material.Instance[itemTemplateId].ItemSubType == 505);
		}
		return result;
	}

	// Token: 0x06000AF1 RID: 2801 RVA: 0x000463B4 File Offset: 0x000445B4
	[Obsolete]
	public static bool CanItemEatForChangeNeili(sbyte itemType, short itemTemplateId)
	{
		bool flag = itemType == 12;
		return flag && Misc.Instance[itemTemplateId].Neili > 0;
	}

	// Token: 0x06000AF2 RID: 2802 RVA: 0x000463E8 File Offset: 0x000445E8
	public static bool CanItemEatForChangeMaxNeili(sbyte itemType, short itemTemplateId)
	{
		bool flag = itemType == 12;
		bool result;
		if (flag)
		{
			bool flag2;
			if (Misc.Instance[itemTemplateId].MaxNeili <= 0)
			{
				int first = Misc.Instance[itemTemplateId].FiveElementTransfer.First;
				flag2 = (first >= 0 && first < 5 && Misc.Instance[itemTemplateId].FiveElementTransfer.Second > 0);
			}
			else
			{
				flag2 = true;
			}
			result = flag2;
		}
		else
		{
			result = false;
		}
		return result;
	}

	// Token: 0x06000AF3 RID: 2803 RVA: 0x00046458 File Offset: 0x00044658
	[Obsolete]
	[return: TupleElementNames(new string[]
	{
		"Count",
		"Tip"
	})]
	public static ValueTuple<int, string> CalculateCountAndTip(int charId, ItemKey key, int amount = 2147483647)
	{
		int limitCount = amount;
		string limitTip = string.Empty;
		bool flag = key.ItemType == 12 && ItemTemplateHelper.GetGroupId(key.ItemType, key.TemplateId) == 375;
		ValueTuple<int, string> result;
		if (flag)
		{
			result = new ValueTuple<int, string>(int.MaxValue, string.Empty);
		}
		else
		{
			bool flag2 = key.ItemType == 12 && key.TemplateId != 265;
			if (flag2)
			{
				MiscItem miscItem = Misc.Instance[key.TemplateId];
				EquipCombatSkillMonitor equipCombatSkillMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<EquipCombatSkillMonitor>(charId, false);
				limitCount = Mathf.CeilToInt((float)(equipCombatSkillMonitor.MaxNeili - equipCombatSkillMonitor.CurrNeili) / (float)miscItem.Neili);
				limitTip = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_NeiliMax);
				result = new ValueTuple<int, string>(limitCount, limitTip);
			}
			else
			{
				bool flag3 = key.ItemType == 12 && key.TemplateId == 265;
				if (flag3)
				{
					limitCount = ((amount >= ItemTemplateHelper.GetTianJieFuLuCountUnit()) ? ItemTemplateHelper.GetTianJieFuLuCountUnit() : 0);
					limitTip = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_CountLess);
				}
				bool flag4 = key.ItemType == 8;
				if (flag4)
				{
					MedicineItem medicineItem = Medicine.Instance.GetItem(key.TemplateId);
					bool flag5 = medicineItem.RequiredMainAttributeType >= 0;
					if (flag5)
					{
						int attrLimit = Math.Max((int)(SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<AttributeMonitor>(charId, false).CurMainAttribute[(int)medicineItem.RequiredMainAttributeType] / (short)Math.Max(medicineItem.RequiredMainAttributeValue, 1)), 0);
						bool flag6 = attrLimit < limitCount;
						if (flag6)
						{
							ECharacterPropertyDisplayType type = ECharacterPropertyDisplayType.Strength + (int)medicineItem.RequiredMainAttributeType;
							CharacterPropertyDisplayItem characterPropertyDisplayItem = CharacterPropertyDisplay.Instance[type.ToInt()];
							limitCount = attrLimit;
							limitTip = LanguageKey.LK_UsingMedicine_Tip_Attribute_Not_Enough.TrFormat(((characterPropertyDisplayItem != null) ? characterPropertyDisplayItem.Name : null) ?? "{unknown}");
							bool flag7 = characterPropertyDisplayItem == null;
							if (flag7)
							{
								Debug.LogError(string.Format("medicineItem ({0}, {1}) requires property {2}, trying to get type {3}'s name, but failed.", new object[]
								{
									medicineItem.Name,
									key.TemplateId,
									medicineItem.RequiredMainAttributeType,
									type
								}));
							}
							return new ValueTuple<int, string>(limitCount, limitTip);
						}
					}
					bool flag8 = medicineItem.Duration == 0;
					if (flag8)
					{
						return new ValueTuple<int, string>(amount, limitTip);
					}
				}
				bool flag9 = limitCount > 0;
				if (flag9)
				{
					int slotCount = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<EatingItemMonitor>(charId, false).GetAvailableEatingSlotsCount() * ItemTemplateHelper.GetItemCountUnit(key.ItemType, key.TemplateId);
					limitCount = Math.Min(slotCount, limitCount);
					bool flag10 = limitCount == 0;
					if (flag10)
					{
						limitTip = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_SlotMax);
					}
				}
				result = new ValueTuple<int, string>(limitCount, limitTip);
			}
		}
		return result;
	}

	// Token: 0x06000AF4 RID: 2804 RVA: 0x00046708 File Offset: 0x00044908
	[Obsolete]
	public static bool GetMedicineItemMenuInteractable(ItemKey itemKey, bool canEatMore, bool canReplaceWugKing, bool isAttributeMeet, ref string tipContent)
	{
		bool flag = !canEatMore || itemKey.ItemType != 8;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			MedicineItem medicineConfig = Medicine.Instance[itemKey.TemplateId];
			bool isOuterMedicine = medicineConfig.EffectType == EMedicineEffectType.RecoverOuterInjury;
			bool isInnerMedicine = medicineConfig.EffectType == EMedicineEffectType.RecoverInnerInjury;
			bool healOuterLocked = SingletonObject.getInstance<DisplayTriggerModel>().HealingOuterRestriction;
			bool healInnerLocked = SingletonObject.getInstance<DisplayTriggerModel>().HealingInnerRestriction;
			bool medicineLocked = (isOuterMedicine && healOuterLocked) || (isInnerMedicine && healInnerLocked);
			bool isTopical = medicineConfig.Duration == 0;
			bool isEating = !isTopical;
			bool innerInteractable = (!medicineLocked && isAttributeMeet) || canReplaceWugKing;
			bool flag2 = !innerInteractable;
			if (flag2)
			{
				bool flag3 = medicineLocked;
				if (flag3)
				{
					tipContent = LocalStringManager.Get(LanguageKey.LK_Use_Medicine_Tip_NotAllow);
				}
				else
				{
					bool flag4 = isEating;
					if (flag4)
					{
						tipContent = LocalStringManager.Get(LanguageKey.LK_Use_Medicine_Tip_NoSlot);
					}
					else
					{
						ECharacterPropertyDisplayType type = ECharacterPropertyDisplayType.Strength + (int)medicineConfig.RequiredMainAttributeType;
						CharacterPropertyDisplayItem characterPropertyDisplayItem = CharacterPropertyDisplay.Instance[type.ToInt()];
						tipContent = LocalStringManager.GetFormat(LanguageKey.LK_UsingMedicine_Tip_Attribute_Not_Enough, characterPropertyDisplayItem.Name) + LanguageKey.LK_Ignore.Tr().SetColor("brightred");
					}
				}
			}
			result = innerInteractable;
		}
		return result;
	}

	// Token: 0x06000AF5 RID: 2805 RVA: 0x00046834 File Offset: 0x00044A34
	public static bool IsRecoverInjuryMainMedicineItem(ItemKey itemKey)
	{
		bool flag = itemKey.ItemType != 8;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			MedicineItem config = Medicine.Instance[itemKey.TemplateId];
			bool flag2;
			if (config.Duration == 0)
			{
				EMedicineEffectType effectType = config.EffectType;
				flag2 = (effectType == EMedicineEffectType.RecoverInnerInjury || effectType <= EMedicineEffectType.RecoverOuterInjury);
			}
			else
			{
				flag2 = false;
			}
			result = flag2;
		}
		return result;
	}

	// Token: 0x06000AF6 RID: 2806 RVA: 0x0004688C File Offset: 0x00044A8C
	public static bool IsNoDurationMedicineItem(ItemKey itemKey)
	{
		bool flag = itemKey.ItemType != 8;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			MedicineItem config = Medicine.Instance[itemKey.TemplateId];
			result = (config.Duration <= 0 || config.InstantAffect);
		}
		return result;
	}

	// Token: 0x06000AF7 RID: 2807 RVA: 0x000468D8 File Offset: 0x00044AD8
	public static string GetCanEatItemButtonName(ItemKey itemKey)
	{
		sbyte itemType = itemKey.ItemType;
		sbyte b = itemType;
		if (b != 8)
		{
			if (b == 12)
			{
				return LanguageKey.LK_Use.Tr();
			}
		}
		else
		{
			bool flag = CommonUtils.IsNoDurationMedicineItem(itemKey);
			if (flag)
			{
				return LanguageKey.LK_Use.Tr();
			}
		}
		return LanguageKey.LK_Eat_Item.Tr();
	}

	// Token: 0x06000AF8 RID: 2808 RVA: 0x00046934 File Offset: 0x00044B34
	public static string GetSevenElementTypeString(sbyte elementType)
	{
		return DisplayConfig.Personality.Instance[(int)elementType].Name;
	}

	// Token: 0x06000AF9 RID: 2809 RVA: 0x00046958 File Offset: 0x00044B58
	[return: TupleElementNames(new string[]
	{
		"content",
		"progress",
		"type"
	})]
	public static ValueTuple<string, float, int> GetCharacterHealthInfo(short health, short leftHealth, int characterId = -1)
	{
		bool flag = health == -1 && leftHealth == -1;
		ValueTuple<string, float, int> result;
		if (flag)
		{
			result = new ValueTuple<string, float, int>(string.Empty, 0f, 5);
		}
		else
		{
			EHealthType type = CommonUtils.GetHealthType(health, leftHealth, characterId);
			sbyte langIndex = (sbyte)type;
			string content = CommonUtils.GetHealthString(type);
			float progress = 0f;
			bool flag2 = leftHealth > 0;
			if (flag2)
			{
				progress = (float)health / (float)leftHealth;
			}
			bool flag3 = type == EHealthType.Unknown;
			if (flag3)
			{
				progress = 1f;
			}
			result = new ValueTuple<string, float, int>(content, progress, (int)langIndex);
		}
		return result;
	}

	// Token: 0x06000AFA RID: 2810 RVA: 0x000469D4 File Offset: 0x00044BD4
	public static EHealthType GetHealthType(short health, short leftHealth, int characterId = -1)
	{
		List<short> featureIds = (characterId != -1) ? SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<FeatureMonitor>(characterId, false).FeatureIds : null;
		return (characterId != -1) ? HealthTypeHelper.CalcType(featureIds, health, leftHealth) : HealthTypeHelper.CalcType(health, leftHealth);
	}

	// Token: 0x06000AFB RID: 2811 RVA: 0x00046A14 File Offset: 0x00044C14
	public static string GetHealthColorString(EHealthType healthType)
	{
		if (!true)
		{
		}
		string result;
		switch (healthType)
		{
		case EHealthType.Dying:
			result = "#AF3737";
			break;
		case EHealthType.CriticallyIll:
			result = "#c04f2d";
			break;
		case EHealthType.Weak:
			result = "#aaa772";
			break;
		case EHealthType.Sick:
			result = "#5e7b69";
			break;
		case EHealthType.Healthy:
			result = "#aec9e3";
			break;
		default:
			result = Colors.Instance["lightgrey"].ColorToHexString("#");
			break;
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06000AFC RID: 2812 RVA: 0x00046A90 File Offset: 0x00044C90
	public static string GetHealthString(EHealthType healthType)
	{
		return string.Concat(new string[]
		{
			"<color=",
			CommonUtils.GetHealthColorString(healthType),
			">",
			LocalStringManager.Get(CommonUtils.CharacterHealthLevelIds[(int)((sbyte)healthType)]),
			"</color>"
		});
	}

	// Token: 0x06000AFD RID: 2813 RVA: 0x00046AE0 File Offset: 0x00044CE0
	public static string GetHealthString(sbyte healthType)
	{
		return string.Concat(new string[]
		{
			"<color=",
			CommonUtils.GetHealthColorString((EHealthType)healthType),
			">",
			LocalStringManager.Get(CommonUtils.CharacterHealthLevelIds[(int)healthType]),
			"</color>"
		});
	}

	// Token: 0x06000AFE RID: 2814 RVA: 0x00046B30 File Offset: 0x00044D30
	public static ValueTuple<string, string> GetHealthStringDivided(EHealthType healthType)
	{
		string colorString = CommonUtils.GetHealthColorString(healthType);
		sbyte typeIndex = (sbyte)healthType;
		return new ValueTuple<string, string>(string.Format("<color={0}>{1}</color>", colorString, LocalStringManager.Get(CommonUtils.CharacterHealthLevelIds[(int)typeIndex])[0]), string.Format("<color={0}>{1}</color>", colorString, LocalStringManager.Get(CommonUtils.CharacterHealthLevelIds[(int)typeIndex])[1]));
	}

	// Token: 0x06000AFF RID: 2815 RVA: 0x00046B98 File Offset: 0x00044D98
	public static int Health2Age(short health)
	{
		return Mathf.CeilToInt(Mathf.Max(0f, (float)health) / 12f);
	}

	// Token: 0x06000B00 RID: 2816 RVA: 0x00046BC4 File Offset: 0x00044DC4
	public static string GetHealthIcon(EHealthType type)
	{
		return "ui9_icon_health_big_" + type.ToInt().ToString();
	}

	// Token: 0x06000B01 RID: 2817 RVA: 0x00046BF0 File Offset: 0x00044DF0
	public static string GetQiDisorderString(sbyte level)
	{
		return CommonUtils.DisorderOfQiLevelLangKeys[(int)level].Tr().SetColor(CommonUtils.DisorderOfQiLevelColors[(int)level]);
	}

	// Token: 0x06000B02 RID: 2818 RVA: 0x00046C1C File Offset: 0x00044E1C
	public static string GetFiveElementsColor(sbyte type)
	{
		if (!true)
		{
		}
		string result;
		switch (type)
		{
		case 0:
			result = "FiveElementType_Jingang";
			break;
		case 1:
			result = "FiveElementType_Zixia";
			break;
		case 2:
			result = "FiveElementType_Xuanyin";
			break;
		case 3:
			result = "FiveElementType_Chunyang";
			break;
		case 4:
			result = "FiveElementType_Guiyuan";
			break;
		case 5:
			result = "FiveElementType_Hunyuan";
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06000B03 RID: 2819 RVA: 0x00046C90 File Offset: 0x00044E90
	public static string GetHealthIconSmall(EHealthType type)
	{
		return "ui9_icon_health_small_" + type.ToInt().ToString();
	}

	// Token: 0x06000B04 RID: 2820 RVA: 0x00046CBC File Offset: 0x00044EBC
	public static Color GetProgressColor(int value, int max, float lineGreen = 0.5f, float lineYellow = 0.8f)
	{
		bool flag = (float)value < (float)max * lineGreen;
		Color result;
		if (flag)
		{
			result = Colors.Instance["lightgreen"];
		}
		else
		{
			bool flag2 = (float)value < (float)max * lineYellow;
			if (flag2)
			{
				result = Colors.Instance["yellow"];
			}
			else
			{
				result = Colors.Instance["red"];
			}
		}
		return result;
	}

	// Token: 0x06000B05 RID: 2821 RVA: 0x00046D1C File Offset: 0x00044F1C
	public static string GetLocationStateAreaName(LocationNameRelatedData location)
	{
		bool flag = location.AreaTemplateId < 0;
		string result;
		if (flag)
		{
			result = LocalStringManager.Get(LanguageKey.LK_Unknown_Area_Name);
		}
		else
		{
			MapAreaItem areaCfg = MapArea.Instance[location.AreaTemplateId];
			MapStateItem stateCfg = MapState.Instance[areaCfg.StateID];
			result = stateCfg.Name + "-" + areaCfg.Name;
		}
		return result;
	}

	// Token: 0x06000B06 RID: 2822 RVA: 0x00046D80 File Offset: 0x00044F80
	public static string GetRelativeLocationName(LocationNameRelatedData location)
	{
		bool flag = location.AreaTemplateId < 0;
		string result;
		if (flag)
		{
			result = LocalStringManager.Get(LanguageKey.LK_Unknown_Area_Name);
		}
		else
		{
			MapAreaItem mapAreaItem = MapArea.Instance[location.AreaTemplateId];
			string stateName = MapState.Instance[mapAreaItem.StateID].Name;
			string areaName = mapAreaItem.Name;
			string settlementName = "";
			bool flag2 = location.SettlementMapBlockTemplateId >= 0;
			if (flag2)
			{
				settlementName = LocalStringManager.GetFormat(LanguageKey.LK_Quotation_Marks_Fix, CommonUtils.GetSettlementString(location.SettlementRandomNameId, location.SettlementMapBlockTemplateId));
			}
			bool flag3 = location.AreaTemplateId == SingletonObject.getInstance<WorldMapModel>().BrokenPerformAreaSettlementData.Item3 && location.SettlementMapBlockTemplateId == 36;
			if (flag3)
			{
				settlementName = LocalStringManager.Get(LanguageKey.LK_Stockade_InStory);
			}
			string direction = "";
			bool flag4 = location.Direction != -1;
			if (flag4)
			{
				direction = LocalStringManager.Get(string.Format("LK_Direction_{0}", location.Direction));
			}
			result = LocalStringManager.GetFormat(LanguageKey.LK_Map_RelativeLocationName, new object[]
			{
				stateName,
				areaName,
				settlementName,
				direction
			});
		}
		return result;
	}

	// Token: 0x06000B07 RID: 2823 RVA: 0x00046EA4 File Offset: 0x000450A4
	public static string GetLocationName(CharacterLocationDisplayData locationDisplayData)
	{
		FullBlockName fullBlockName = locationDisplayData.FullBlockName;
		MapStateItem stateConfig = MapState.Instance[fullBlockName.stateTemplateId];
		MapAreaItem areaConfig = MapArea.Instance[fullBlockName.areaTemplateId];
		bool flag = stateConfig == null || areaConfig == null;
		string locationName;
		if (flag)
		{
			locationName = LanguageKey.LK_Character_Location_Format_Invalid_2.Tr();
		}
		else
		{
			bool isCapturedInStoneRoom = locationDisplayData.IsCapturedInStoneRoom;
			if (isCapturedInStoneRoom)
			{
				locationName = LanguageKey.LK_Character_Location_Format_StoneHouse_2.TrFormat(Config.Organization.Instance[16].Name);
			}
			else
			{
				MapBlockData blockData = fullBlockName.BlockData;
				WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
				int nameIndex = worldMapModel.GetBlockNameIndex(blockData, worldMapModel.GetAreaSize(blockData.AreaId));
				string blockName = worldMapModel.GetBlockName(blockData.AreaId, blockData.BlockId, blockData.TemplateId, nameIndex);
				locationName = string.Concat(new string[]
				{
					stateConfig.Name,
					"-",
					areaConfig.Name,
					"-",
					blockName
				});
			}
		}
		return locationName;
	}

	// Token: 0x06000B08 RID: 2824 RVA: 0x00046FAC File Offset: 0x000451AC
	public static string GetSpecificLocationName(LocationNameRelatedData location)
	{
		bool flag = location.AreaTemplateId < 0;
		string result;
		if (flag)
		{
			result = LocalStringManager.Get(LanguageKey.LK_Unknown_Area_Name);
		}
		else
		{
			MapAreaItem mapAreaItem = MapArea.Instance[location.AreaTemplateId];
			string stateName = MapState.Instance[mapAreaItem.StateID].Name;
			string areaName = mapAreaItem.Name;
			string settlementName = "";
			bool flag2 = location.SettlementMapBlockTemplateId >= 0;
			if (flag2)
			{
				settlementName = LocalStringManager.GetFormat(LanguageKey.LK_Quotation_Marks_Fix, CommonUtils.GetSettlementString(location.SettlementRandomNameId, location.SettlementMapBlockTemplateId));
			}
			bool flag3 = location.AreaTemplateId == SingletonObject.getInstance<WorldMapModel>().BrokenPerformAreaSettlementData.Item3 && location.SettlementMapBlockTemplateId == 36;
			if (flag3)
			{
				settlementName = LocalStringManager.Get(LanguageKey.LK_Stockade_InStory);
			}
			result = string.Concat(new string[]
			{
				stateName,
				"-",
				areaName,
				"-",
				settlementName,
				" "
			});
		}
		return result;
	}

	// Token: 0x06000B09 RID: 2825 RVA: 0x000470A8 File Offset: 0x000452A8
	public static string GetSettlementString(short randomNameId, short mapBlockTemplateId)
	{
		return new SettlementNameRelatedData(randomNameId, mapBlockTemplateId).GetName();
	}

	// Token: 0x06000B0A RID: 2826 RVA: 0x000470C4 File Offset: 0x000452C4
	public static bool FixToShowAbleString(ref string valueStr, TMP_FontAsset fontAsset)
	{
		bool flag = string.IsNullOrEmpty(valueStr);
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = null == fontAsset;
			if (flag2)
			{
				valueStr = string.Empty;
				result = true;
			}
			else
			{
				bool fixHappenFlag = false;
				List<char> finalCharList = new List<char>();
				for (int i = 0; i < valueStr.Length; i++)
				{
					char character = valueStr[i];
					bool flag3 = fontAsset.HasCharacter(character, true, false);
					if (flag3)
					{
						finalCharList.Add(character);
					}
					else
					{
						fixHappenFlag = true;
					}
				}
				valueStr = new string(finalCharList.ToArray());
				result = fixHappenFlag;
			}
		}
		return result;
	}

	// Token: 0x06000B0B RID: 2827 RVA: 0x0004715C File Offset: 0x0004535C
	public static void FixAndSetInputFieldText(this TMP_InputField inputField, ref string valueStr, TMP_FontAsset fontAsset)
	{
		bool flag = CommonUtils.FixToShowAbleString(ref valueStr, fontAsset);
		if (flag)
		{
			valueStr = valueStr.Replace(" ", string.Empty);
			bool flag2 = !string.IsNullOrEmpty(valueStr);
			if (flag2)
			{
				valueStr = valueStr.Substring(0, Mathf.Min(valueStr.Length, inputField.characterLimit - 1));
			}
			inputField.SetTextWithoutNotify(valueStr);
		}
	}

	// Token: 0x06000B0C RID: 2828 RVA: 0x000471C0 File Offset: 0x000453C0
	public static bool SensitiveWordHandle(this TMP_InputField inputField, ref string valueStr)
	{
		bool flag = string.IsNullOrEmpty(valueStr);
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			List<SensitiveWordsMatchResult> matchList = SensitiveWordsSystem.Instance.TryMatch(valueStr, 10);
			bool flag2 = matchList != null && matchList.Count > 0;
			if (flag2)
			{
				inputField.SetTextWithoutNotify(string.Empty);
				result = true;
			}
			else
			{
				inputField.SetTextWithoutNotify(valueStr);
				result = false;
			}
		}
		return result;
	}

	// Token: 0x06000B0D RID: 2829 RVA: 0x00047220 File Offset: 0x00045420
	public static void InputOnSelectBindMoveTextEnd(this TMP_InputField inputField)
	{
		inputField.onSelect.RemoveAllListeners();
		Action <>9__1;
		inputField.onSelect.AddListener(delegate(string str)
		{
			YieldHelper instance = SingletonObject.getInstance<YieldHelper>();
			uint frame = 10U;
			Action job;
			if ((job = <>9__1) == null)
			{
				job = (<>9__1 = delegate()
				{
					inputField.caretPosition = 0;
					inputField.MoveTextEnd(true);
				});
			}
			instance.DelayFrameDo(frame, job);
		});
	}

	// Token: 0x06000B0E RID: 2830 RVA: 0x0004726C File Offset: 0x0004546C
	public static void SetAsTypedInput<TV>(this TMP_InputField inputField, Action<TV> receiver) where TV : IConvertible
	{
		string[] refer = new string[]
		{
			inputField.text
		};
		inputField.onValueChanged.ResetListener(delegate(string v)
		{
			try
			{
				TV sov = (TV)((object)((IConvertible)v).ToType(typeof(TV), CultureInfo.InvariantCulture));
				receiver(sov);
				refer[0] = v;
			}
			catch (Exception _)
			{
				inputField.SetTextWithoutNotify(refer[0]);
			}
		});
	}

	// Token: 0x06000B0F RID: 2831 RVA: 0x000472C8 File Offset: 0x000454C8
	public static string GetSpecialEffectDesc(int effectTemplateId)
	{
		return CommonUtils.GetSpecialEffectDescInternal(new CombatSkillEffectDescriptionDisplayData
		{
			EffectId = effectTemplateId
		}, false, 0, -1);
	}

	// Token: 0x06000B10 RID: 2832 RVA: 0x000472F4 File Offset: 0x000454F4
	public static string GetSpecialEffectDesc(int combatSkillTemplateId, bool isDirect)
	{
		return CommonUtils.GetSpecialEffectDesc(combatSkillTemplateId, isDirect, false);
	}

	// Token: 0x06000B11 RID: 2833 RVA: 0x00047310 File Offset: 0x00045510
	public static string GetSpecialEffectDesc(int combatSkillTemplateId, bool isDirect, bool autoCheckBoss)
	{
		CombatSkillItem combatSkillConfig = CombatSkill.Instance[combatSkillTemplateId];
		int effectId = isDirect ? combatSkillConfig.DirectEffectID : combatSkillConfig.ReverseEffectID;
		return CommonUtils.GetSpecialEffectDescInternal(new CombatSkillEffectDescriptionDisplayData
		{
			EffectId = effectId
		}, autoCheckBoss, 0, -1);
	}

	// Token: 0x06000B12 RID: 2834 RVA: 0x0004735C File Offset: 0x0004555C
	public static string GetSpecialEffectDesc(CombatSkillDisplayData displayData)
	{
		return CommonUtils.GetSpecialEffectDesc(displayData, false);
	}

	// Token: 0x06000B13 RID: 2835 RVA: 0x00047378 File Offset: 0x00045578
	public static string GetSpecialEffectDesc(CombatSkillDisplayData displayData, bool autoCheckBoss)
	{
		bool flag = displayData == null;
		string result;
		if (flag)
		{
			result = string.Empty;
		}
		else
		{
			result = CommonUtils.GetSpecialEffectDescInternal(displayData.EffectDescription, autoCheckBoss, 0, -1);
		}
		return result;
	}

	// Token: 0x06000B14 RID: 2836 RVA: 0x000473A8 File Offset: 0x000455A8
	public static string GetSpecialEffectDesc(CombatSkillEffectDescriptionDisplayData effectDescriptionData)
	{
		return CommonUtils.GetSpecialEffectDescInternal(effectDescriptionData, false, 0, -1);
	}

	// Token: 0x06000B15 RID: 2837 RVA: 0x000473C4 File Offset: 0x000455C4
	public static string GetSpecialEffectDesc(CombatSkillEffectDescriptionDisplayData effectDescriptionData, bool autoCheckBoss)
	{
		return CommonUtils.GetSpecialEffectDescInternal(effectDescriptionData, autoCheckBoss, 0, -1);
	}

	// Token: 0x06000B16 RID: 2838 RVA: 0x000473E0 File Offset: 0x000455E0
	public static string GetSpecialEffectDescSpecifyIndex(int effectTemplateId, int index = 0, int countByLast = -1)
	{
		return CommonUtils.GetSpecialEffectDescInternal(new CombatSkillEffectDescriptionDisplayData
		{
			EffectId = effectTemplateId
		}, false, index, countByLast);
	}

	// Token: 0x06000B17 RID: 2839 RVA: 0x0004740C File Offset: 0x0004560C
	public static string GetSpecialEffectDescSpecifyIndex(CombatSkillEffectDescriptionDisplayData effectDescriptionData, int index = 0, int countByLast = -1)
	{
		return CommonUtils.GetSpecialEffectDescInternal(effectDescriptionData, false, index, countByLast);
	}

	// Token: 0x06000B18 RID: 2840 RVA: 0x00047428 File Offset: 0x00045628
	private static string GetSpecialEffectDescInternal(CombatSkillEffectDescriptionDisplayData effectDescription, bool autoCheckBoss, int index = 0, int countByLast = -1)
	{
		bool flag = effectDescription.EffectId < 0;
		string result;
		if (flag)
		{
			result = string.Empty;
		}
		else
		{
			SpecialEffectItem config = SpecialEffect.Instance[effectDescription.EffectId];
			index = ((countByLast > 0) ? (config.Desc.Length - countByLast) : index);
			index = Mathf.Clamp(index, 0, config.Desc.Length - 1);
			bool flag2;
			if (autoCheckBoss)
			{
				string[] playerCastBossSkillDesc = config.PlayerCastBossSkillDesc;
				flag2 = (playerCastBossSkillDesc != null && playerCastBossSkillDesc.Length > 0);
			}
			else
			{
				flag2 = false;
			}
			bool hasBossDesc = flag2;
			bool useDetail = false;
			string[] descSource = hasBossDesc ? config.PlayerCastBossSkillDesc : (useDetail ? config.DetailedDesc : config.Desc);
			string sourceDesc = descSource[Mathf.Clamp(index, 0, descSource.Length - 1)];
			bool flag3 = config.AffectRequirePower == null || config.AffectRequirePower.Length == 0;
			if (flag3)
			{
				result = sourceDesc;
			}
			else
			{
				StringBuilder builder = EasyPool.Get<StringBuilder>();
				builder.Clear();
				builder.Append(sourceDesc);
				bool flag4 = effectDescription.AffectRequirePower == null;
				if (flag4)
				{
					CommonUtils.ReplaceAffectRequirePower(builder, config);
				}
				else
				{
					CommonUtils.ReplaceAffectRequirePower(builder, effectDescription);
				}
				string ret = builder.ToString();
				EasyPool.Free<StringBuilder>(builder);
				result = ret;
			}
		}
		return result;
	}

	// Token: 0x06000B19 RID: 2841 RVA: 0x0004754C File Offset: 0x0004574C
	private static void ReplaceAffectRequirePower(StringBuilder builder, SpecialEffectItem specialEffectConfig)
	{
		int[] affectRequirePower = specialEffectConfig.AffectRequirePower;
		for (int i = 0; i < affectRequirePower.Length; i++)
		{
			int power = (affectRequirePower[i] >= 0) ? affectRequirePower[i] : CommonUtils.CalcSumMax2HitDistribution((int)specialEffectConfig.SkillTemplateId);
			power = Mathf.CeilToInt((float)power / 10f);
			builder.Replace(string.Format("${0}$", i), LocalStringManager.Get(string.Format("LK_Num_{0}", power)));
		}
	}

	// Token: 0x06000B1A RID: 2842 RVA: 0x000475C8 File Offset: 0x000457C8
	private static void ReplaceAffectRequirePower(StringBuilder builder, CombatSkillEffectDescriptionDisplayData effectDescriptionData)
	{
		List<int> affectRequirePower = effectDescriptionData.AffectRequirePower;
		for (int i = 0; i < affectRequirePower.Count; i++)
		{
			int power = affectRequirePower[i];
			power = Mathf.CeilToInt((float)power / 10f);
			builder.Replace(string.Format("${0}$", i), LocalStringManager.Get(string.Format("LK_Num_{0}", power)));
		}
	}

	// Token: 0x06000B1B RID: 2843 RVA: 0x00047638 File Offset: 0x00045838
	public static int CalcSumMax2HitDistribution(int skillTemplateId)
	{
		bool flag = skillTemplateId < 0;
		int result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			CombatSkillItem combatSkillConfig = CombatSkill.Instance[skillTemplateId];
			sbyte[] hitDistribution = combatSkillConfig.PerHitDamageRateDistribution;
			bool flag2 = hitDistribution.Length <= 2;
			if (flag2)
			{
				result = hitDistribution.Sum();
			}
			else
			{
				int max0 = (int)hitDistribution[0];
				int max = (int)hitDistribution[1];
				for (int hitIndex = 2; hitIndex < hitDistribution.Length; hitIndex++)
				{
					sbyte hit = hitDistribution[hitIndex];
					bool flag3 = max0 > max;
					if (flag3)
					{
						bool flag4 = (int)hit > max;
						if (flag4)
						{
							max = (int)hit;
						}
					}
					else
					{
						bool flag5 = (int)hit > max0;
						if (flag5)
						{
							max0 = (int)hit;
						}
					}
				}
				result = Math.Max(max0, 0) + Math.Max(max, 0);
			}
		}
		return result;
	}

	// Token: 0x06000B1C RID: 2844 RVA: 0x000476F0 File Offset: 0x000458F0
	public unsafe static int CalcSumMax2HitDistribution(HitOrAvoidInts hitDistribution)
	{
		int max0 = hitDistribution.Items.FixedElementField;
		int max = *(ref hitDistribution.Items.FixedElementField + 4);
		for (int i = 2; i < 4; i++)
		{
			int hit = *(ref hitDistribution.Items.FixedElementField + (IntPtr)i * 4);
			bool flag = max0 > max;
			if (flag)
			{
				bool flag2 = hit > max;
				if (flag2)
				{
					max = hit;
				}
			}
			else
			{
				bool flag3 = hit > max0;
				if (flag3)
				{
					max0 = hit;
				}
			}
		}
		return Math.Max(max0, 0) + Math.Max(max, 0);
	}

	// Token: 0x06000B1D RID: 2845 RVA: 0x00047780 File Offset: 0x00045980
	public static sbyte GetMaxCombatSkillGradeByFavorability(short favorability)
	{
		sbyte type = FavorabilityType.GetFavorabilityType(favorability);
		return (sbyte)((type >= 2) ? Mathf.Min((int)((type - 2 + 1) * 2 - 1), 8) : -1);
	}

	// Token: 0x06000B1E RID: 2846 RVA: 0x000477B0 File Offset: 0x000459B0
	public static sbyte GetMaxCombatSkillGradeByApprovingRate(short approvingRate)
	{
		return (sbyte)Mathf.Max((int)(approvingRate / 100 - 2), 0);
	}

	// Token: 0x06000B1F RID: 2847 RVA: 0x000477D0 File Offset: 0x000459D0
	public static void AlignTransformToTarget(Transform alignTrans, Transform targetTrans, Transform moveTrans = null)
	{
		bool flag = null == moveTrans;
		if (flag)
		{
			moveTrans = alignTrans;
		}
		bool flag2 = null == alignTrans || null == targetTrans;
		if (!flag2)
		{
			Vector3 offset = targetTrans.position - alignTrans.position;
			offset.z = 0f;
			Vector3 pos = moveTrans.position;
			pos += offset;
			moveTrans.position = pos;
		}
	}

	// Token: 0x06000B20 RID: 2848 RVA: 0x0004783A File Offset: 0x00045A3A
	public static void SetSkeletonDataAsset(SkeletonGraphic graphic, SkeletonDataAsset dataAsset, string skinName = "default", string animationName = "animation", bool loop = true)
	{
		graphic.skeletonDataAsset = dataAsset;
		graphic.Initialize(true);
		graphic.initialSkinName = skinName;
		graphic.AnimationState.SetAnimation(0, animationName, loop);
	}

	// Token: 0x06000B21 RID: 2849 RVA: 0x00047864 File Offset: 0x00045A64
	public static int GetResourceCountLevel(sbyte resourceType, int count)
	{
		int[] threshold = (resourceType == 6) ? CommonUtils.MoneyCountThreshold : ((resourceType == 7) ? CommonUtils.AuthorityCountThreshold : CommonUtils.ResourceCountThreshold);
		int level;
		for (level = 0; level < threshold.Length; level++)
		{
			bool flag = count >= threshold[level];
			if (!flag)
			{
				break;
			}
		}
		return level;
	}

	// Token: 0x06000B22 RID: 2850 RVA: 0x000478BC File Offset: 0x00045ABC
	public static void SetResourceImage(sbyte resourceType, int count, CRawImage rawImage, float adjustScale = 1f)
	{
		int level = CommonUtils.GetResourceCountLevel(resourceType, count);
		ResourceTypeItem config = ((int)resourceType < Config.ResourceType.Instance.Count) ? Config.ResourceType.Instance.GetItem(resourceType) : null;
		string resourceFolder = CommonUtils.ResourceFolderNames[(int)resourceType];
		string texturePath = string.Format("RemakeResources/Textures/{0}/{1}{2}", resourceFolder, ((config != null) ? config.ImgPrefix : null) ?? "charactermenu3_10_exp_", level);
		ResLoader.Load<Texture2D>(texturePath, delegate(Texture2D texture)
		{
			rawImage.texture = texture;
			rawImage.SetNativeSize();
			rawImage.transform.localScale = Vector3.one * CommonUtils.ResourceImageScaleConfig[(int)resourceType][level] * adjustScale;
		}, null, false);
	}

	// Token: 0x06000B23 RID: 2851 RVA: 0x0004796C File Offset: 0x00045B6C
	public static void SetRawImage(CRawImage rawImage, string texturePath, bool setNativeSize = false)
	{
		ResLoader.Load<Texture2D>(texturePath, delegate(Texture2D texture)
		{
			rawImage.texture = texture;
			rawImage.enabled = true;
			bool setNativeSize2 = setNativeSize;
			if (setNativeSize2)
			{
				rawImage.SetNativeSize();
			}
		}, delegate(string error)
		{
			Debug.LogWarning(texturePath + "  load fail");
		}, false);
	}

	// Token: 0x06000B24 RID: 2852 RVA: 0x000479BC File Offset: 0x00045BBC
	[Obsolete]
	public static string GetIdentityIcon(sbyte level)
	{
		level = MathUtils.Clamp(level, 0, 8);
		return string.Format("sp_icon_identify_{0}", level);
	}

	// Token: 0x06000B25 RID: 2853 RVA: 0x000479E8 File Offset: 0x00045BE8
	public static string GetIdentityIconByLevel(sbyte level)
	{
		level = MathUtils.Clamp(level, 0, 8);
		return string.Format("{0}{1}", "ui9_icon_identity_big_", level);
	}

	// Token: 0x06000B26 RID: 2854 RVA: 0x00047A1C File Offset: 0x00045C1C
	public static MapBlockItem GetConfigSafe(this MapBlockData blockData)
	{
		bool flag = blockData.RootBlockId >= 0;
		if (flag)
		{
			MapBlockData rootBlock = blockData.GetRootBlock();
			bool flag2 = rootBlock != null;
			if (flag2)
			{
				return rootBlock.GetConfig();
			}
		}
		return MapBlock.Instance.GetItem(blockData.TemplateId);
	}

	// Token: 0x06000B27 RID: 2855 RVA: 0x00047A6C File Offset: 0x00045C6C
	public static string GetFilterLifeSkillTypeIcon(int index)
	{
		index = MathUtils.Clamp(index, 0, 16);
		return string.Format("sp_icon_jiyi_{0}", index);
	}

	// Token: 0x06000B28 RID: 2856 RVA: 0x00047A9C File Offset: 0x00045C9C
	public static string GetBuildingClassTipIcon(EBuildingBlockClass buildingClass)
	{
		switch (buildingClass)
		{
		case EBuildingBlockClass.Static:
		case EBuildingBlockClass.Villiage:
		case EBuildingBlockClass.Eclectic:
			return "mousetip_jiyi_15";
		case EBuildingBlockClass.Resource:
			return "mousetip_resource";
		case EBuildingBlockClass.Kungfu:
			return "mousetip_gongfa_0";
		case EBuildingBlockClass.Music:
			return "mousetip_jiyi_0";
		case EBuildingBlockClass.Chess:
			return "mousetip_jiyi_1";
		case EBuildingBlockClass.Poem:
			return "mousetip_jiyi_2";
		case EBuildingBlockClass.Painting:
			return "mousetip_jiyi_3";
		case EBuildingBlockClass.Math:
			return "mousetip_jiyi_4";
		case EBuildingBlockClass.Appraisal:
			return "mousetip_jiyi_5";
		case EBuildingBlockClass.Forging:
			return "mousetip_jiyi_6";
		case EBuildingBlockClass.Woodworking:
			return "mousetip_jiyi_7";
		case EBuildingBlockClass.Medicine:
			return "mousetip_jiyi_8";
		case EBuildingBlockClass.Toxicology:
			return "mousetip_jiyi_9";
		case EBuildingBlockClass.Weaving:
			return "mousetip_jiyi_10";
		case EBuildingBlockClass.Jade:
			return "mousetip_jiyi_11";
		case EBuildingBlockClass.Taoism:
			return "mousetip_jiyi_12";
		case EBuildingBlockClass.Buddhism:
			return "mousetip_jiyi_13";
		case EBuildingBlockClass.Cooking:
			return "mousetip_jiyi_14";
		}
		return "";
	}

	// Token: 0x06000B29 RID: 2857 RVA: 0x00047BB3 File Offset: 0x00045DB3
	public static string GetBuildingWidthIcon(sbyte width)
	{
		return (width == 2) ? "building_zhange_1" : "building_zhange_0";
	}

	// Token: 0x06000B2A RID: 2858 RVA: 0x00047BC8 File Offset: 0x00045DC8
	public static string GetFilterCombatSkillTypeIcon(int index)
	{
		index = MathUtils.Clamp(index, 0, 14);
		return string.Format("sp_icon_wuxue_{0}", index);
	}

	// Token: 0x06000B2B RID: 2859 RVA: 0x00047BF8 File Offset: 0x00045DF8
	public static void ShowDialog(string title, string text, Action onConfirm = null, EDialogType dialogType = EDialogType.None)
	{
		DialogCmd cmd = new DialogCmd
		{
			Title = title.ColorReplace(),
			Content = text.ColorReplace(),
			Type = 2,
			Yes = onConfirm,
			DialogType = dialogType
		};
		CommonUtils.ShowDialog(cmd);
	}

	// Token: 0x06000B2C RID: 2860 RVA: 0x00047C40 File Offset: 0x00045E40
	public static void ShowConfirmDialog(string title, string text, Action onConfirm, Action onCancel = null, EDialogType dialogType = EDialogType.None)
	{
		DialogCmd cmd = new DialogCmd
		{
			Title = title.ColorReplace(),
			Content = text.ColorReplace(),
			Type = 1,
			Yes = onConfirm,
			No = onCancel,
			DialogType = dialogType
		};
		CommonUtils.ShowDialog(cmd);
	}

	// Token: 0x06000B2D RID: 2861 RVA: 0x00047C90 File Offset: 0x00045E90
	public static void ShowConditionDialog(string title, string text, bool confirmInteractable, Action onConfirm, Action onCancel = null, EDialogType dialogType = EDialogType.None)
	{
		DialogCmd cmd = new DialogCmd
		{
			Title = title.ColorReplace(),
			Content = text.ColorReplace(),
			Type = 1,
			Yes = onConfirm,
			No = onCancel,
			DialogType = dialogType
		};
		CommonUtils.ShowDialog(cmd);
	}

	// Token: 0x06000B2E RID: 2862 RVA: 0x00047CE4 File Offset: 0x00045EE4
	public static void ShowDialog(DialogCmd cmd)
	{
		bool flag = CommonUtils.CheckDonotShow(cmd);
		if (flag)
		{
			Action yes = cmd.Yes;
			if (yes != null)
			{
				yes();
			}
		}
		else
		{
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}
	}

	// Token: 0x06000B2F RID: 2863 RVA: 0x00047D3C File Offset: 0x00045F3C
	public static bool CheckDonotShow(DialogCmd cmd)
	{
		bool flag = cmd.DialogType == EDialogType.None;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			GlobalSettings settingData = SingletonObject.getInstance<GlobalSettings>();
			result = settingData.CheckDialogDoNotShow(cmd.DialogType);
		}
		return result;
	}

	// Token: 0x06000B30 RID: 2864 RVA: 0x00047D74 File Offset: 0x00045F74
	public static void TryKillTween(Tweener tweener, bool callComplete)
	{
		bool flag = tweener == null;
		if (!flag)
		{
			bool flag2 = tweener.IsActive() && tweener.IsPlaying();
			if (flag2)
			{
				tweener.Kill(callComplete);
			}
		}
	}

	// Token: 0x06000B31 RID: 2865 RVA: 0x00047DAC File Offset: 0x00045FAC
	public static void TryKillTween(Tween tween, bool callComplete)
	{
		bool flag = tween == null;
		if (!flag)
		{
			bool flag2 = tween.IsActive() && tween.IsPlaying();
			if (flag2)
			{
				tween.Kill(callComplete);
			}
		}
	}

	// Token: 0x06000B32 RID: 2866 RVA: 0x00047DE4 File Offset: 0x00045FE4
	public static string GetResourceSpriteName(sbyte resourceType, bool random)
	{
		int index = 0;
		if (random)
		{
			index = Random.Range(0, 3);
		}
		string result;
		switch (resourceType)
		{
		case 0:
			result = string.Format("sp_image_resource_food_{0}", index);
			break;
		case 1:
			result = string.Format("sp_image_resource_wood_{0}", index);
			break;
		case 2:
			result = string.Format("sp_image_resource_ston_{0}", index);
			break;
		case 3:
			result = string.Format("sp_image_resource_jade_{0}", index);
			break;
		case 4:
			result = string.Format("sp_image_resource_silk_{0}", index);
			break;
		case 5:
			result = string.Format("sp_image_resource_herbal_{0}", index);
			break;
		default:
			result = "mousetip_actualcombat_big";
			break;
		}
		return result;
	}

	// Token: 0x06000B33 RID: 2867 RVA: 0x00047EA8 File Offset: 0x000460A8
	public static string GetTaiwuSpriteName()
	{
		sbyte gender = SingletonObject.getInstance<WorldMapModel>().TaiwuGender;
		return CommonUtils.GetTaiwuSpriteName(gender);
	}

	// Token: 0x06000B34 RID: 2868 RVA: 0x00047ECC File Offset: 0x000460CC
	public static string GetTaiwuSpriteName(sbyte gender)
	{
		return (gender == 1) ? "map_icon_taiwu_0" : "map_icon_taiwu_1";
	}

	// Token: 0x06000B35 RID: 2869 RVA: 0x00047EF0 File Offset: 0x000460F0
	public static string GetProfessionSkillSprite(ProfessionSkillItem config, string atlas = "bottom", string group = null, bool isLocked = false)
	{
		StringBuilder builder = EasyPool.Get<StringBuilder>();
		bool flag = !atlas.IsNullOrEmpty();
		if (flag)
		{
			builder.Append(atlas);
			builder.Append('_');
		}
		builder.Append("profession_tubiao_");
		bool flag2 = !group.IsNullOrEmpty();
		if (flag2)
		{
			builder.Append(group);
			builder.Append('_');
		}
		builder.Append(config.TriggerType.ToInt());
		builder.Append('_');
		builder.Append((int)(isLocked ? 0 : config.Level));
		string ret = builder.ToString();
		EasyPool.Free<StringBuilder>(builder);
		return ret;
	}

	// Token: 0x06000B36 RID: 2870 RVA: 0x00047F9C File Offset: 0x0004619C
	internal static string GetProfessionAnimalGradeName(ProfessionData professionData)
	{
		byte[] array = GlobalConfig.Instance.HunterSkill2_AnimalCountIndexToAnimalConsummateLevelList[(int)(professionData.GetSeniorityAnimalCount() - 1)];
		byte consummateLevel = array[array.Length - 1];
		int grade = (int)((consummateLevel + 1) / 2);
		return LocalStringManager.Get(string.Format("LK_Animal_Grade_{0}", grade)).SetColor(Colors.Instance.GradeColors[grade]);
	}

	// Token: 0x06000B37 RID: 2871 RVA: 0x00047FFC File Offset: 0x000461FC
	public static int GetProfessionVisionLevel(ProfessionData professionData)
	{
		int level = CommonUtils.GetProfessionVisionLevelAfterSkill(professionData);
		return (level <= 1) ? 0 : ((level <= 2) ? 1 : 2);
	}

	// Token: 0x06000B38 RID: 2872 RVA: 0x00048024 File Offset: 0x00046224
	public static int GetProfessionVisionLevelAfterSkill(ProfessionData professionData)
	{
		int vision = professionData.GetSeniorityVisionRangeBonus();
		WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
		MapBlockData block = mapModel.GetBlockData(new Location(mapModel.CurrentAreaId, mapModel.CurrentBlockId)).GetRootBlock();
		sbyte viewRange = block.GetConfig().ViewRange;
		return vision + (int)viewRange;
	}

	// Token: 0x06000B39 RID: 2873 RVA: 0x00048071 File Offset: 0x00046271
	public static string GetOrgGradeName(int grade)
	{
		return LocalStringManager.Get(string.Format("LK_OrgGrade_{0}", grade)).SetColor(Colors.Instance.GradeColors[grade]);
	}

	// Token: 0x06000B3A RID: 2874 RVA: 0x000480A0 File Offset: 0x000462A0
	public static string GetItemGradeShortNameWithMoreThan(int grade)
	{
		bool flag = grade <= 0 || grade > 8;
		string result;
		if (flag)
		{
			result = LocalStringManager.Get(LanguageKey.LK_Grade_No_Limit);
		}
		else
		{
			string gradeShort = CommonUtils.GetShortGradeText(grade, true);
			bool flag2 = grade == 8;
			if (flag2)
			{
				result = gradeShort;
			}
			else
			{
				result = gradeShort + " " + LocalStringManager.Get(LanguageKey.LK_Grade_MoreThan);
			}
		}
		return result;
	}

	// Token: 0x06000B3B RID: 2875 RVA: 0x000480F8 File Offset: 0x000462F8
	public static string GetShortGradeText(int grade, bool hasColor = true)
	{
		string gradeShort = LocalStringManager.Get(string.Format("LK_Mousetip_Grade_Short_{0}", grade));
		if (hasColor)
		{
			gradeShort = gradeShort.SetColor(Colors.Instance.GradeColors[grade]);
		}
		return gradeShort;
	}

	// Token: 0x06000B3C RID: 2876 RVA: 0x0004813D File Offset: 0x0004633D
	public static string GetPreGradeText(sbyte grade)
	{
		return LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", grade));
	}

	// Token: 0x06000B3D RID: 2877 RVA: 0x00048154 File Offset: 0x00046354
	public static string GetFullGradeText(sbyte grade, bool hasColor = true)
	{
		string result = LocalStringManager.Get(string.Format("LK_Grade_{0}", grade));
		if (hasColor)
		{
			result = result.SetColor(Colors.Instance.GradeColors[(int)grade]);
		}
		return result;
	}

	// Token: 0x06000B3E RID: 2878 RVA: 0x00048199 File Offset: 0x00046399
	public static string GetItemTypeName(sbyte itemType)
	{
		return LocalStringManager.Get(string.Format("LK_ItemType_{0}", itemType));
	}

	// Token: 0x06000B3F RID: 2879 RVA: 0x000481B0 File Offset: 0x000463B0
	public static string GetItemSubTypeName(short itemSubType)
	{
		return LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", itemSubType));
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x000481C8 File Offset: 0x000463C8
	public static string GetCarrierSubTypeName(short itemSubType)
	{
		bool flag = itemSubType == 401;
		string result;
		if (flag)
		{
			result = LanguageKey.LK_Mousetip_Carrier_1.Tr();
		}
		else
		{
			bool flag2 = itemSubType == 400;
			if (flag2)
			{
				result = LanguageKey.LK_Mousetip_Carrier_0.Tr();
			}
			else
			{
				result = LanguageKey.LK_Mousetip_Carrier_2.Tr();
			}
		}
		return result;
	}

	// Token: 0x06000B41 RID: 2881 RVA: 0x00048216 File Offset: 0x00046416
	public static string GetTraditionalNumber(sbyte number)
	{
		return LocalStringManager.Get(string.Format("LK_TraditionalChineseNumber_{0}", number));
	}

	// Token: 0x06000B42 RID: 2882 RVA: 0x00048230 File Offset: 0x00046430
	public static string GetPreexistenceNumberText(sbyte number)
	{
		return "ui9_preexistence_positions_" + MathUtils.Clamp(number, 0, 9).ToString();
	}

	// Token: 0x06000B43 RID: 2883 RVA: 0x00048260 File Offset: 0x00046460
	public static Color GetMerchantLevelColor(sbyte level)
	{
		if (!true)
		{
		}
		Color result;
		switch (level)
		{
		case 0:
			result = Colors.Instance["pinkyellow"];
			break;
		case 1:
			result = Colors.Instance.GradeColors[2];
			break;
		case 2:
			result = Colors.Instance.GradeColors[4];
			break;
		case 3:
			result = Colors.Instance.GradeColors[5];
			break;
		case 4:
			result = Colors.Instance.GradeColors[6];
			break;
		case 5:
			result = Colors.Instance.GradeColors[7];
			break;
		case 6:
			result = Colors.Instance.GradeColors[8];
			break;
		default:
			result = Colors.Instance["pinkyellow"];
			break;
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06000B44 RID: 2884 RVA: 0x00048338 File Offset: 0x00046538
	public static string GetMerchantLevel(sbyte level)
	{
		Color color = CommonUtils.GetMerchantLevelColor(level);
		return CommonUtils.GetTraditionalNumber(level).SetColor(color);
	}

	// Token: 0x06000B45 RID: 2885 RVA: 0x0004835D File Offset: 0x0004655D
	public static string GetDebtIcon(long worth)
	{
		return (worth > 0L) ? "sp_icon_qianenshiyi_1" : "sp_icon_qianenshiyi_2";
	}

	// Token: 0x06000B46 RID: 2886 RVA: 0x00048370 File Offset: 0x00046570
	public static sbyte ConvertShowIndexToConfigIndex(sbyte showIndex)
	{
		sbyte configIndex = showIndex;
		bool flag = showIndex == 0;
		if (flag)
		{
			configIndex = 2;
		}
		else
		{
			bool flag2 = showIndex == 1;
			if (flag2)
			{
				configIndex = 0;
			}
			else
			{
				bool flag3 = showIndex == 2;
				if (flag3)
				{
					configIndex = 1;
				}
			}
		}
		return configIndex;
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x000483B0 File Offset: 0x000465B0
	public static string ConvertValueByCreatingType(byte creatingType, string value)
	{
		bool flag = CreatingType.IsNonEvolutionaryType(creatingType);
		string result;
		if (flag)
		{
			result = "-";
		}
		else
		{
			result = value;
		}
		return result;
	}

	// Token: 0x06000B48 RID: 2888 RVA: 0x000483D8 File Offset: 0x000465D8
	public static void CheckForAvatarExtraInfo(int charId, AvatarData avatarData, ref short clothDisplayId)
	{
		EventModel eventModel = SingletonObject.getInstance<EventModel>();
		bool flag = eventModel.NeedShowAsMarriageLook1(charId);
		if (flag)
		{
			avatarData.ChangeToMarriageStyle1();
			clothDisplayId = avatarData.ClothDisplayId;
		}
		bool flag2 = eventModel.NeedShowAsMarriageLook2(charId);
		if (flag2)
		{
			avatarData.ChangeToMarriageStyle2();
			clothDisplayId = avatarData.ClothDisplayId;
		}
		avatarData.ShowBlush = eventModel.NeedShowBlush(charId);
		avatarData.ShowJieqingMask = eventModel.NeedShowJieqingMask(charId);
		eventModel.CheckAvatarClothDisplayIdForEvent(charId, avatarData, null);
	}

	// Token: 0x06000B49 RID: 2889 RVA: 0x00048448 File Offset: 0x00046648
	public static sbyte GetDarkAshStyle(CharacterDisplayData characterDisplayData)
	{
		return CommonUtils.GetDarkAshStyle(characterDisplayData.CharacterId, characterDisplayData.DarkAshProtector);
	}

	// Token: 0x06000B4A RID: 2890 RVA: 0x0004846C File Offset: 0x0004666C
	public static sbyte GetDarkAshStyle(int characterId, uint darkAshProtector)
	{
		bool flag = (darkAshProtector & 512U) == 0U;
		sbyte result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			result = (sbyte)(characterId & 3);
		}
		return result;
	}

	// Token: 0x06000B4B RID: 2891 RVA: 0x00048494 File Offset: 0x00046694
	public static sbyte GetXiangshuInfectionStyle(CharacterDisplayData displayData)
	{
		List<short> featureIds = displayData.FeatureIds;
		bool flag = featureIds != null;
		if (flag)
		{
			bool flag2 = featureIds.Contains(211);
			if (flag2)
			{
				return 1;
			}
			bool flag3 = featureIds.Contains(210);
			if (flag3)
			{
				return 0;
			}
		}
		return -1;
	}

	// Token: 0x06000B4C RID: 2892 RVA: 0x000484E0 File Offset: 0x000466E0
	public static sbyte GetHuanxinFaceStyle(CharacterDisplayData displayData)
	{
		List<short> featureIds = displayData.FeatureIds;
		bool flag = featureIds != null;
		if (flag)
		{
			bool flag2 = featureIds.Contains(863);
			if (flag2)
			{
				return 0;
			}
			bool flag3 = featureIds.Contains(864);
			if (flag3)
			{
				return 1;
			}
			bool flag4 = featureIds.Contains(865);
			if (flag4)
			{
				return 2;
			}
			bool flag5 = featureIds.Contains(866);
			if (flag5)
			{
				return 3;
			}
			bool flag6 = featureIds.Contains(867);
			if (flag6)
			{
				return 4;
			}
			bool flag7 = featureIds.Contains(868);
			if (flag7)
			{
				return 5;
			}
		}
		return -1;
	}

	// Token: 0x06000B4D RID: 2893 RVA: 0x00048580 File Offset: 0x00046780
	public static List<short> GetUnlockBuildingListFromConfig(Config.LifeSkillItem lifeSkillItem, List<short> idList)
	{
		idList.Clear();
		foreach (Config.ShortList shortList in lifeSkillItem.UnlockBuildingList)
		{
			foreach (short id in shortList.DataList)
			{
				bool flag = id != -1;
				if (flag)
				{
					idList.Add(id);
				}
			}
		}
		return idList;
	}

	// Token: 0x06000B4E RID: 2894 RVA: 0x00048638 File Offset: 0x00046838
	public static List<short> GetUnlockBuildingListFromConfigByPage(Config.LifeSkillItem lifeSkillItem, List<short> idList, int page)
	{
		idList.Clear();
		bool flag = page < lifeSkillItem.UnlockBuildingList.Count;
		if (flag)
		{
			foreach (short id in lifeSkillItem.UnlockBuildingList[page].DataList)
			{
				bool flag2 = id != -1;
				if (flag2)
				{
					idList.Add(id);
				}
			}
		}
		return idList;
	}

	// Token: 0x06000B4F RID: 2895 RVA: 0x000486CC File Offset: 0x000468CC
	public static LifeSkillPageUnlockType GetSkillPageUnlockType(Config.LifeSkillItem lifeSkillItem, int page)
	{
		List<short> unlockBuilding = EasyPool.Get<List<short>>();
		CommonUtils.GetUnlockBuildingListFromConfigByPage(lifeSkillItem, unlockBuilding, page);
		bool flag = unlockBuilding.Count > 0;
		LifeSkillPageUnlockType result;
		if (flag)
		{
			EasyPool.Free<List<short>>(unlockBuilding);
			result = LifeSkillPageUnlockType.Building;
		}
		else
		{
			int tag = (int)lifeSkillItem.UnlockInformationList[page];
			bool flag2 = tag != 0;
			if (flag2)
			{
				result = LifeSkillPageUnlockType.Knowledge;
			}
			else
			{
				result = LifeSkillPageUnlockType.Invalid;
			}
		}
		return result;
	}

	// Token: 0x06000B50 RID: 2896 RVA: 0x00048720 File Offset: 0x00046920
	public static int GetFiveElementByNeiliType(int neiliType)
	{
		bool flag = neiliType > 5;
		int result;
		if (flag)
		{
			result = neiliType / 6 - 1;
		}
		else
		{
			result = neiliType;
		}
		return result;
	}

	// Token: 0x06000B51 RID: 2897 RVA: 0x00048744 File Offset: 0x00046944
	public static ValueTuple<string, string> GetConsummateLevelShowDataLegacy(sbyte level)
	{
		int index = Mathf.Clamp(Mathf.Max((int)(level - 1), 0) / 2, 0, 8);
		string iconName = string.Format("combat_icon_jingyuan_{0}", index);
		string levelName = LocalStringManager.Get(LanguageKey.LK_Consummate_Level_0 + index).SetColor(Colors.Instance.GradeColors[index]);
		return new ValueTuple<string, string>(iconName, levelName);
	}

	// Token: 0x06000B52 RID: 2898 RVA: 0x000487A4 File Offset: 0x000469A4
	public static ValueTuple<string, string> GetConsummateLevelShowData(sbyte level)
	{
		int index = Mathf.Clamp(Mathf.Max((int)(level - 1), 0) / 2, 0, 8);
		string iconName = string.Format("ui9_icon_consummate_level_big_{0}", index);
		string levelName = LocalStringManager.Get(LanguageKey.LK_Consummate_Level_0 + index).SetColor(Colors.Instance.GradeColors[index]);
		return new ValueTuple<string, string>(iconName, levelName);
	}

	// Token: 0x06000B53 RID: 2899 RVA: 0x00048804 File Offset: 0x00046A04
	public static string GetConsummateLevelShowDataFull(sbyte level, out string iconName)
	{
		int index = Mathf.Clamp(Mathf.Max((int)(level - 1), 0) / 2, 0, 8);
		iconName = string.Format("ui9_icon_consummate_level_big_{0}", index);
		string levelName = LocalStringManager.Get(LanguageKey.LK_Consummate_Level_0 + index);
		return LocalStringManager.GetFormat(LanguageKey.LK_Consummate_Level_Desc, level, levelName).SetColor(Colors.Instance.GradeColors[index]);
	}

	// Token: 0x06000B54 RID: 2900 RVA: 0x00048870 File Offset: 0x00046A70
	public static List<SpecialBreakProperty> GetBreakEntryPropertiesByTemplateId(short templateId)
	{
		SkillBreakPlateGridBonusTypeItem bonusTypeData = SkillBreakPlateGridBonusType.Instance[templateId];
		List<SpecialBreakProperty> results = new List<SpecialBreakProperty>();
		for (int i = 0; i < bonusTypeData.CharacterPropertyBonusList.Length; i++)
		{
			PropertyAndValue bonus = bonusTypeData.CharacterPropertyBonusList[i];
			short displayTypeId = CharacterPropertyReferenced.Instance[bonus.PropertyId].DisplayType;
			CharacterPropertyDisplayItem propertyData = CharacterPropertyDisplay.Instance[displayTypeId];
			bool isInverse = propertyData.IsInverse;
			int displayFix = propertyData.DisplayFix;
			bool isDisplaySpecially = propertyData.IsDisplaySpecially;
			bool isPositive = true;
			SpecialBreakProperty property = CommonUtils.GetPropertyBouns(bonus.Value, propertyData.Name, propertyData.IsPercent, isInverse, displayFix, isDisplaySpecially, ref isPositive);
			property.propertyId = bonus.PropertyId;
			property.isEquipEffect = true;
			bool flag = isPositive;
			if (flag)
			{
				results.Insert(0, property);
			}
			else
			{
				results.Add(property);
			}
		}
		for (int j = 0; j < bonusTypeData.CombatSkillPropertyBonusList.Length; j++)
		{
			PropertyAndValue bonus2 = bonusTypeData.CombatSkillPropertyBonusList[j];
			CombatSkillPropertyItem propertyData2 = CombatSkillProperty.Instance[(int)bonus2.PropertyId];
			bool isPositive2 = true;
			SpecialBreakProperty property2 = CommonUtils.GetPropertyBouns(bonus2.Value, propertyData2.Name, propertyData2.IsPercent, propertyData2.IsInverse, propertyData2.DisplayFix, propertyData2.IsDisplaySpecially, ref isPositive2);
			property2.propertyId = bonus2.PropertyId;
			property2.isEquipEffect = false;
			bool flag2 = isPositive2;
			if (flag2)
			{
				results.Insert(0, property2);
			}
			else
			{
				results.Add(property2);
			}
		}
		return results;
	}

	// Token: 0x06000B55 RID: 2901 RVA: 0x00048A18 File Offset: 0x00046C18
	public static SpecialBreakProperty GetPropertyBouns(short value, string name, bool isPercent, bool isInverse, int displayFix, bool isDisplaySpecially, ref bool isPositive)
	{
		float showValue = (float)value;
		bool flag = displayFix != 0;
		if (flag)
		{
			bool flag2 = displayFix > 0;
			if (flag2)
			{
				showValue *= (float)displayFix;
			}
			else
			{
				showValue /= (float)Mathf.Abs(displayFix);
			}
		}
		string valueStr = (showValue >= 0f) ? string.Format("+{0}", showValue) : showValue.ToString();
		if (isPercent)
		{
			valueStr += "%";
		}
		SpecialBreakProperty specialBreakProperty = default(SpecialBreakProperty);
		bool positive;
		if (isInverse)
		{
			positive = (value < 0);
		}
		else
		{
			positive = (value >= 0);
		}
		isPositive = positive;
		bool flag3 = positive;
		if (flag3)
		{
			specialBreakProperty.name = name.SetColor("brightblue");
			specialBreakProperty.value = valueStr.SetColor("brightblue");
		}
		else
		{
			specialBreakProperty.name = name.SetColor("brightred");
			specialBreakProperty.value = valueStr.SetColor("brightred");
		}
		specialBreakProperty.isDisplaySpecially = isDisplaySpecially;
		specialBreakProperty.displayFix = displayFix;
		specialBreakProperty.isInverse = isInverse;
		specialBreakProperty.isPercent = isPercent;
		return specialBreakProperty;
	}

	// Token: 0x06000B56 RID: 2902 RVA: 0x00048B3C File Offset: 0x00046D3C
	public static List<bool> GetBreakValuePercentsByTemplateId(short templateId)
	{
		SkillBreakPlateGridBonusTypeItem bonusTypeData = SkillBreakPlateGridBonusType.Instance[templateId];
		List<bool> results = new List<bool>();
		for (int i = 0; i < bonusTypeData.CharacterPropertyBonusList.Length; i++)
		{
			PropertyAndValue bonus = bonusTypeData.CharacterPropertyBonusList[i];
			short displayTypeId = CharacterPropertyReferenced.Instance[bonus.PropertyId].DisplayType;
			CharacterPropertyDisplayItem propertyData = CharacterPropertyDisplay.Instance[displayTypeId];
			results.Add(propertyData.IsPercent);
		}
		for (int j = 0; j < bonusTypeData.CombatSkillPropertyBonusList.Length; j++)
		{
			PropertyAndValue bonus2 = bonusTypeData.CombatSkillPropertyBonusList[j];
			CombatSkillPropertyItem propertyData2 = CombatSkillProperty.Instance[(int)bonus2.PropertyId];
			results.Add(propertyData2.IsPercent);
		}
		return results;
	}

	// Token: 0x06000B57 RID: 2903 RVA: 0x00048C10 File Offset: 0x00046E10
	public static string[] GetBreakoutMaxPowerName(int addPowerValue, short templateId)
	{
		CombatSkillItem config = CombatSkill.Instance[templateId];
		sbyte gradeGroup = Grade.GetGroup(config.Grade);
		int[] valueArray = GlobalConfig.BreakoutMaxPowerNameValueArray[(int)gradeGroup];
		int powerGrade = 0;
		for (int i = 0; i < valueArray.Length; i++)
		{
			bool flag = addPowerValue > valueArray[i];
			if (!flag)
			{
				break;
			}
			powerGrade = i;
		}
		string levelString = LocalStringManager.Get(string.Format("LK_Skill_Break_MaxPower_Grade{0}", powerGrade)).SetGradeColor(powerGrade);
		return new string[]
		{
			string.Format("<color=#brightblue>+{0}</color>({1})", addPowerValue, levelString).ColorReplace(),
			levelString
		};
	}

	// Token: 0x06000B58 RID: 2904 RVA: 0x00048CBC File Offset: 0x00046EBC
	public static void GetNeighborBlocks(BuildingAreaData buildingAreaData, List<BuildingBlockData> blockList, short blockIndex, ref List<BuildingBlockData> neighborBlockList, sbyte blockWidth = 1, int range = 2, List<int> neighborDistanceList = null)
	{
		List<short> neighborIndexList = EasyPool.Get<List<short>>();
		neighborBlockList.Clear();
		buildingAreaData.GetNeighborBlocks(blockIndex, blockWidth, neighborIndexList, neighborDistanceList, range);
		List<int> neighborDistanceListResult = new List<int>();
		for (int i = 0; i < neighborIndexList.Count; i++)
		{
			BuildingBlockData neighborBlock = blockList[(int)neighborIndexList[i]];
			bool flag = neighborBlock.RootBlockIndex >= 0;
			if (flag)
			{
				neighborBlock = blockList[(int)neighborBlock.RootBlockIndex];
			}
			bool flag2 = !neighborBlockList.Contains(neighborBlock);
			if (flag2)
			{
				neighborBlockList.Add(neighborBlock);
				bool flag3 = neighborDistanceList != null;
				if (flag3)
				{
					int distance = CommonUtils.GetDistanceInTwoBlock(blockIndex, (int)neighborBlock.BlockIndex, (int)blockWidth, (int)buildingAreaData.Width, (int)BuildingBlock.Instance[neighborBlock.TemplateId].Width);
					neighborDistanceListResult.Add(distance);
				}
			}
		}
		EasyPool.Free<List<short>>(neighborIndexList);
		bool flag4 = neighborDistanceList != null;
		if (flag4)
		{
			neighborDistanceList.Clear();
			neighborDistanceList.AddRange(neighborDistanceListResult);
		}
	}

	// Token: 0x06000B59 RID: 2905 RVA: 0x00048DC0 File Offset: 0x00046FC0
	public static int GetDistanceInTwoBlock(short rootIndex, int neighborIndex, int blockWidth, int areaWidth, int neighborWidth)
	{
		int distance = int.MaxValue;
		int blockX = (int)rootIndex % areaWidth;
		int blockY = (int)rootIndex / areaWidth;
		int neighborBlockX = neighborIndex % areaWidth;
		int neighborBlockY = neighborIndex / areaWidth;
		for (int x = neighborBlockX; x < Math.Min(neighborBlockX + neighborWidth, areaWidth); x++)
		{
			for (int y = neighborBlockY; y < Math.Min(neighborBlockY + neighborWidth, areaWidth); y++)
			{
				int manhattanDistance = MathUtils.GetManhattanDistance(blockX, blockY, x, y, blockWidth);
				distance = Mathf.Min(distance, manhattanDistance);
			}
		}
		return distance;
	}

	// Token: 0x06000B5A RID: 2906 RVA: 0x00048E48 File Offset: 0x00047048
	public static int GetDependBuildingsMinDistance(short rootIndex, List<BuildingBlockData> neighborList, List<BuildingBlockData> blockList, BuildingBlockItem configData, int areaWidth)
	{
		int result = int.MaxValue;
		for (int i = 0; i < neighborList.Count; i++)
		{
			BuildingBlockData neighborBlock = neighborList[i];
			bool flag = neighborBlock.RootBlockIndex >= 0;
			if (flag)
			{
				neighborBlock = blockList[(int)neighborBlock.RootBlockIndex];
			}
			bool flag2 = neighborBlock.TemplateId != 0 && neighborBlock.CanUse();
			if (flag2)
			{
				int dependIndex = configData.DependBuildings.IndexOf(neighborBlock.TemplateId);
				bool flag3 = dependIndex >= 0;
				if (flag3)
				{
					BuildingBlockItem neighborConfigData = BuildingBlock.Instance[neighborBlock.TemplateId];
					int distance = CommonUtils.GetDistanceInTwoBlock(rootIndex, (int)neighborBlock.BlockIndex, (int)configData.Width, areaWidth, (int)neighborConfigData.Width);
					int currentDistance = result;
					result = Mathf.Min(distance, currentDistance);
				}
			}
		}
		bool flag4 = result == int.MaxValue;
		if (flag4)
		{
			result = 1;
		}
		return result;
	}

	// Token: 0x06000B5B RID: 2907 RVA: 0x00048F34 File Offset: 0x00047134
	public static bool IsBuildingCostResourcesEnough(BuildingBlockItem config)
	{
		sbyte i = 0;
		while ((int)i < config.BaseBuildCost.Length)
		{
			ushort cost = config.BaseBuildCost[(int)i];
			bool flag = SingletonObject.getInstance<BuildingModel>().GetResourceCount(i) < (int)cost;
			if (flag)
			{
				return false;
			}
			i += 1;
		}
		return true;
	}

	// Token: 0x06000B5C RID: 2908 RVA: 0x00048F84 File Offset: 0x00047184
	public static Color Saturationize(Color inColor, float saturation)
	{
		float average = (inColor.r + inColor.g + inColor.b) / 3f;
		inColor += (inColor - new Color(average, average, average, 0f)) * saturation;
		return inColor;
	}

	// Token: 0x06000B5D RID: 2909 RVA: 0x00048FD4 File Offset: 0x000471D4
	public static Color GetNegativeEffectColor(Color origin)
	{
		Color col = origin;
		Color tone = new Color(-0.05f, -0.05f, 0.05f, 0.86f);
		float avg = (col.r * 0.37f + col.g * 0.75f + col.b * 0.15f) / 1.27f;
		Vector3 tonec = new Vector3(col.r, col.g, col.b);
		tonec.X += (avg - col.r) * tone.a;
		tonec.Y += (avg - col.g) * tone.a;
		tonec.Z += (avg - col.b) * tone.a;
		Vector3 rgb = Vector3.Clamp(tonec + new Vector3(tone.r, tone.g, tone.b), Vector3.Zero, Vector3.One);
		col.r = rgb.X;
		col.g = rgb.Y;
		col.b = rgb.Z;
		col = CommonUtils.Saturationize(col, 0f);
		Vector3 dc = Vector3.One - new Vector3(origin.r, origin.g, origin.b);
		float a = 0.1f;
		Vector3 resultRGB = Vector3.Lerp(new Vector3(col.r, col.g, col.b), dc, a);
		col.r = resultRGB.X;
		col.g = resultRGB.Y;
		col.b = resultRGB.Z;
		return col;
	}

	// Token: 0x06000B5E RID: 2910 RVA: 0x00049174 File Offset: 0x00047374
	public static bool HasRelation(ushort relationTypes, ushort relationType)
	{
		bool flag = relationTypes == ushort.MaxValue;
		return !flag && RelationType.HasRelation(relationTypes, relationType);
	}

	// Token: 0x06000B5F RID: 2911 RVA: 0x000491A0 File Offset: 0x000473A0
	public static string GetHighestPriorityRelationText(ushort relationToTaiwu, bool isSameFaction)
	{
		bool flag = relationToTaiwu == ushort.MaxValue;
		string result;
		if (flag)
		{
			result = "-";
		}
		else
		{
			bool flag2 = RelationType.ContainParentRelations(relationToTaiwu);
			if (flag2)
			{
				result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Parent);
			}
			else
			{
				bool flag3 = RelationType.ContainChildRelations(relationToTaiwu);
				if (flag3)
				{
					result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Child);
				}
				else
				{
					bool flag4 = RelationType.ContainBrotherOrSisterRelations(relationToTaiwu);
					if (flag4)
					{
						result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Bro);
					}
					else
					{
						bool flag5 = RelationType.HasRelation(relationToTaiwu, 1024);
						if (flag5)
						{
							result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Wife);
						}
						else
						{
							bool flag6 = RelationType.HasRelation(relationToTaiwu, 32768);
							if (flag6)
							{
								result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Enemy);
							}
							else
							{
								bool flag7 = RelationType.HasRelation(relationToTaiwu, 16384);
								if (flag7)
								{
									result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Adored);
								}
								else
								{
									bool flag8 = RelationType.HasRelation(relationToTaiwu, 2048) || RelationType.HasRelation(relationToTaiwu, 4096);
									if (flag8)
									{
										result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Mentor);
									}
									else
									{
										bool flag9 = RelationType.HasRelation(relationToTaiwu, 512);
										if (flag9)
										{
											result = LocalStringManager.Get(LanguageKey.LK_RelationShip_SwornBro);
										}
										else
										{
											bool flag10 = RelationType.HasRelation(relationToTaiwu, 8192);
											if (flag10)
											{
												result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Friend);
											}
											else if (isSameFaction)
											{
												result = LocalStringManager.Get(LanguageKey.LK_Faction);
											}
											else
											{
												result = "-";
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06000B60 RID: 2912 RVA: 0x00049300 File Offset: 0x00047500
	public static int GetHighestPriorityRelationIndex(ushort relationToTaiwu, bool isSameFaction)
	{
		bool flag = relationToTaiwu == ushort.MaxValue;
		int result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			bool flag2 = RelationType.ContainParentRelations(relationToTaiwu);
			if (flag2)
			{
				result = int.MaxValue;
			}
			else
			{
				bool flag3 = RelationType.ContainChildRelations(relationToTaiwu);
				if (flag3)
				{
					result = 2147483646;
				}
				else
				{
					bool flag4 = RelationType.ContainBrotherOrSisterRelations(relationToTaiwu);
					if (flag4)
					{
						result = 2147483645;
					}
					else
					{
						bool flag5 = RelationType.HasRelation(relationToTaiwu, 1024);
						if (flag5)
						{
							result = 2147483644;
						}
						else
						{
							bool flag6 = RelationType.HasRelation(relationToTaiwu, 32768);
							if (flag6)
							{
								result = 2147483643;
							}
							else
							{
								bool flag7 = RelationType.HasRelation(relationToTaiwu, 16384);
								if (flag7)
								{
									result = 2147483642;
								}
								else
								{
									bool flag8 = RelationType.HasRelation(relationToTaiwu, 2048) || RelationType.HasRelation(relationToTaiwu, 4096);
									if (flag8)
									{
										result = 2147483641;
									}
									else
									{
										bool flag9 = RelationType.HasRelation(relationToTaiwu, 512);
										if (flag9)
										{
											result = 2147483640;
										}
										else
										{
											bool flag10 = RelationType.HasRelation(relationToTaiwu, 8192);
											if (flag10)
											{
												result = 2147483639;
											}
											else if (isSameFaction)
											{
												result = 2147483638;
											}
											else
											{
												result = -1;
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06000B61 RID: 2913 RVA: 0x00049424 File Offset: 0x00047624
	public static Vector3[] GetBalanceLayoutPositionsFromCircle(int posCount, float radius, float angleGap, Vector3 stdDirection)
	{
		float totalAngle = angleGap * (float)(posCount - 1);
		bool flag = totalAngle <= 0f;
		Vector3[] result;
		if (flag)
		{
			result = new Vector3[]
			{
				stdDirection.normalized * radius
			};
		}
		else
		{
			Vector3[] posArray = new Vector3[posCount];
			Vector3 startDirection = Quaternion.Euler(0f, 0f, totalAngle * 0.5f) * stdDirection;
			for (int i = 0; i < posCount; i++)
			{
				posArray[i] = (Quaternion.Euler(0f, 0f, -angleGap * (float)i) * startDirection).normalized * radius;
			}
			result = posArray;
		}
		return result;
	}

	// Token: 0x06000B62 RID: 2914 RVA: 0x000494DC File Offset: 0x000476DC
	public static bool CanUnlockBuildingByMainProgress(BuildingBlockItem configData)
	{
		bool flag = configData.TemplateId == 51;
		bool result;
		if (flag)
		{
			result = SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(23);
		}
		else
		{
			bool flag2 = configData.TemplateId == 50;
			result = (!flag2 || SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(12));
		}
		return result;
	}

	// Token: 0x06000B63 RID: 2915 RVA: 0x0004952C File Offset: 0x0004772C
	public static void SetLoongDebuff(List<LoongInfo> loongInfos, CharacterDisplayData characterDisplayData, TooltipInvoker mouseTipDisplayer, CircleGenerator circleGenerator)
	{
		bool isDlcInstalled = SingletonObject.getInstance<DlcManager>().IsDlcInstalled(DlcManager.DlcIdFiveLoong);
		bool flag = !isDlcInstalled;
		if (!flag)
		{
			List<LoongInfo> loongExist = new List<LoongInfo>();
			List<short> longTemplatedIds = new List<short>();
			CharacterItem characterConfig = Character.Instance[characterDisplayData.TemplateId];
			bool flag2 = characterConfig.CreatingType != 1;
			if (!flag2)
			{
				for (int i = 0; i < loongInfos.Count; i++)
				{
					bool flag3 = !loongInfos[i].IsDisappear && loongInfos[i].GetCharacterDebuffCount(characterDisplayData.CharacterId) > 0;
					if (flag3)
					{
						loongExist.Add(loongInfos[i]);
						longTemplatedIds.Add(loongInfos[i].CharacterTemplateId);
					}
				}
				bool flag4 = loongExist.Count > 0;
				if (flag4)
				{
					ResLoader.LoadByName<GameObject>("LoongDebuff", delegate(GameObject prefab)
					{
						circleGenerator.Prefab = prefab;
						bool flag5 = mouseTipDisplayer != null;
						if (flag5)
						{
							mouseTipDisplayer.Type = TipType.loongDebuff;
							mouseTipDisplayer.RuntimeParam = new ArgumentBox().SetObject("loongInfos", loongExist);
							mouseTipDisplayer.RuntimeParam.Set("CharId", characterDisplayData.CharacterId);
						}
						circleGenerator.Radius = 0.35f;
						circleGenerator.StartAngle = CommonUtils.AngelMapping2[loongExist.Count].Item1;
						circleGenerator.EndAngle = CommonUtils.AngelMapping2[loongExist.Count].Item2;
						circleGenerator.NumberOfObjects = loongExist.Count;
						circleGenerator.Size = new Vector3(0.3f, 0.3f, 0.3f);
						circleGenerator.GenerateLoongDebuffObjects(longTemplatedIds, false);
					}, null);
				}
			}
		}
	}

	// Token: 0x06000B64 RID: 2916 RVA: 0x00049664 File Offset: 0x00047864
	public static Rect RectTransToScreenPos(RectTransform rt, Camera cam)
	{
		Vector3[] corners = new Vector3[4];
		rt.GetWorldCorners(corners);
		Vector2 v0 = RectTransformUtility.WorldToScreenPoint(cam, corners[0]);
		Vector2 v = RectTransformUtility.WorldToScreenPoint(cam, corners[2]);
		Rect rect = new Rect(v0, v - v0);
		return rect;
	}

	// Token: 0x06000B65 RID: 2917 RVA: 0x000496B4 File Offset: 0x000478B4
	public static Resolution GetBestMaxResolution(Resolution[] resolutions)
	{
		bool flag = resolutions == null || resolutions.Length == 0;
		if (flag)
		{
			throw new Exception("resolutions is null or length is 0");
		}
		Resolution maxResolution = resolutions[0];
		Array.ForEach<Resolution>(resolutions, delegate(Resolution resolution)
		{
			bool flag2 = resolution.width > maxResolution.width;
			if (flag2)
			{
				maxResolution = resolution;
			}
			else
			{
				bool flag3 = resolution.width == maxResolution.width && resolution.height > maxResolution.height;
				if (flag3)
				{
					maxResolution = resolution;
				}
			}
		});
		return maxResolution;
	}

	// Token: 0x06000B66 RID: 2918 RVA: 0x0004970C File Offset: 0x0004790C
	public static string GetNeiliTypeBackSpriteName(sbyte neiliType)
	{
		NeiliTypeItem typeConfig = NeiliType.Instance[neiliType];
		return (typeConfig.ColorType == 1) ? "combat_icon_benyuan_0" : "combat_icon_benyuan_1";
	}

	// Token: 0x06000B67 RID: 2919 RVA: 0x00049743 File Offset: 0x00047943
	public static string GetNeiliTypeSpriteName(sbyte neiliType)
	{
		return string.Format("ui9_icon_fiveelement_conflict_{0}_{1}", NeiliType.Instance[neiliType].FiveElements, (NeiliType.Instance[neiliType].ColorType == 2) ? 1 : 0);
	}

	// Token: 0x06000B68 RID: 2920 RVA: 0x00049780 File Offset: 0x00047980
	public static string GetNeiliTypeName(sbyte neiliType)
	{
		NeiliTypeItem typeConfig = NeiliType.Instance[neiliType];
		bool isBuff = typeConfig.ColorType == 1;
		return typeConfig.Name.Substring(3).SetColor(isBuff ? "lightblue" : "pinkyellow");
	}

	// Token: 0x06000B69 RID: 2921 RVA: 0x000497C8 File Offset: 0x000479C8
	public static string GetPercentString(int value, int max)
	{
		bool flag = max == 0;
		if (flag)
		{
			value = 0;
			max = 1;
		}
		return (value * 100 / max).ToString() + "%";
	}

	// Token: 0x06000B6A RID: 2922 RVA: 0x00049804 File Offset: 0x00047A04
	public static List<sbyte> MergeTeammateCommandList(List<sbyte> normalCommands, List<sbyte> advanceCommands, List<sbyte> result = null)
	{
		bool flag = result == null;
		if (flag)
		{
			result = new List<sbyte>();
		}
		result.Clear();
		bool flag2 = normalCommands == null;
		List<sbyte> result2;
		if (flag2)
		{
			result2 = result;
		}
		else
		{
			foreach (sbyte normalCommandId in normalCommands)
			{
				bool flag3 = normalCommandId == -1 || advanceCommands == null;
				if (flag3)
				{
					result.Add(normalCommandId);
				}
				else
				{
					TeammateCommandItem config = Config.TeammateCommand.Instance[normalCommandId];
					ETeammateCommandImplement normalImplement = config.Implement;
					int sameImplementAdvanceCommandIndex = advanceCommands.FindIndex(delegate(sbyte advanceCommandId)
					{
						bool flag5 = advanceCommandId == -1;
						bool result3;
						if (flag5)
						{
							result3 = false;
						}
						else
						{
							TeammateCommandItem advanceConfig = Config.TeammateCommand.Instance[advanceCommandId];
							result3 = (advanceConfig.Implement == normalImplement);
						}
						return result3;
					});
					bool flag4 = sameImplementAdvanceCommandIndex != -1;
					if (flag4)
					{
						result.Add(advanceCommands[sameImplementAdvanceCommandIndex]);
					}
					else
					{
						result.Add(normalCommandId);
					}
				}
			}
			result2 = result;
		}
		return result2;
	}

	// Token: 0x06000B6B RID: 2923 RVA: 0x00049904 File Offset: 0x00047B04
	public static sbyte GetNormalTeammateCommand(sbyte commandTemplateId)
	{
		TeammateCommandItem config = Config.TeammateCommand.Instance[commandTemplateId];
		bool flag = config.Type == ETeammateCommandType.Normal;
		sbyte result;
		if (flag)
		{
			result = commandTemplateId;
		}
		else
		{
			bool flag2 = config.Type == ETeammateCommandType.Advance;
			if (flag2)
			{
				foreach (TeammateCommandItem c in ((IEnumerable<TeammateCommandItem>)Config.TeammateCommand.Instance))
				{
					bool flag3 = c.Type == ETeammateCommandType.Normal && c.Implement == config.Implement;
					if (flag3)
					{
						return c.TemplateId;
					}
				}
				throw new Exception(string.Format("advance teammate command {0} has no normal command. please check config table!", commandTemplateId));
			}
			result = -1;
		}
		return result;
	}

	// Token: 0x06000B6C RID: 2924 RVA: 0x000499C8 File Offset: 0x00047BC8
	public static sbyte GetAdvanceTeammateCommand(sbyte commandTemplateId)
	{
		TeammateCommandItem config = Config.TeammateCommand.Instance[commandTemplateId];
		bool flag = config.Type == ETeammateCommandType.Advance;
		sbyte result;
		if (flag)
		{
			result = commandTemplateId;
		}
		else
		{
			bool flag2 = config.Type == ETeammateCommandType.Normal;
			if (flag2)
			{
				foreach (TeammateCommandItem c in ((IEnumerable<TeammateCommandItem>)Config.TeammateCommand.Instance))
				{
					bool flag3 = c.Type == ETeammateCommandType.Advance && c.Implement == config.Implement;
					if (flag3)
					{
						return c.TemplateId;
					}
				}
				throw new Exception(string.Format("normal teammate command {0} has no advance command. please check config table!", commandTemplateId));
			}
			result = -1;
		}
		return result;
	}

	// Token: 0x06000B6D RID: 2925 RVA: 0x00049A8C File Offset: 0x00047C8C
	public static void CalculateOwnedCommandMedals(List<sbyte> commands, Dictionary<int, int> medalDict)
	{
		medalDict.Clear();
		for (int i = 0; i < 3; i++)
		{
			medalDict[i] = 0;
		}
		bool flag = commands == null;
		if (!flag)
		{
			foreach (sbyte commandId in commands)
			{
				bool flag2 = commandId == -1;
				if (!flag2)
				{
					TeammateCommandItem config = Config.TeammateCommand.Instance[commandId];
					int medalType = (int)config.MedalType;
					medalDict[medalType] += (int)config.MedalCount;
				}
			}
		}
	}

	// Token: 0x06000B6E RID: 2926 RVA: 0x00049B48 File Offset: 0x00047D48
	public static string GetFeatureMedalIcon(int medalType, int medalCount)
	{
		if (!true)
		{
		}
		Dictionary<int, string> dictionary;
		if (medalType != 0)
		{
			if (medalType != 1)
			{
				dictionary = MedalSummary.WisdomMedalIconConfig;
			}
			else
			{
				dictionary = MedalSummary.DefenceMedalIconConfig;
			}
		}
		else
		{
			dictionary = MedalSummary.AttackMedalIconConfig;
		}
		if (!true)
		{
		}
		Dictionary<int, string> iconConfig = dictionary;
		bool flag = medalCount > 0;
		string iconNumber;
		if (flag)
		{
			iconNumber = iconConfig[1];
		}
		else
		{
			bool flag2 = medalCount < 0;
			if (flag2)
			{
				iconNumber = iconConfig[-1];
			}
			else
			{
				iconNumber = iconConfig[0];
			}
		}
		return "ui9_icon_strategy_big_" + iconNumber;
	}

	// Token: 0x06000B6F RID: 2927 RVA: 0x00049BC8 File Offset: 0x00047DC8
	public static string GetFeatureMedalTypeText(int medalType)
	{
		string result;
		switch (medalType)
		{
		case 0:
			result = LocalStringManager.Get(LanguageKey.LK_Feature_Attack);
			break;
		case 1:
			result = LocalStringManager.Get(LanguageKey.LK_Feature_Defence);
			break;
		case 2:
			result = LocalStringManager.Get(LanguageKey.LK_Feature_Wisdom);
			break;
		default:
			result = null;
			break;
		}
		return result;
	}

	// Token: 0x06000B70 RID: 2928 RVA: 0x00049C1C File Offset: 0x00047E1C
	public static void PrepareEnoughChildren(Transform parent, GameObject template, int targetCount, CommonUtils.PrepareExtraItemInfo? extraItemInfo = null)
	{
		int beforeOffset = (extraItemInfo != null && extraItemInfo.GetValueOrDefault().TemplateOrder == CommonUtils.EPrepareTemplateOrder.AfterExtraItems) ? extraItemInfo.Value.ExtraItemCount : 0;
		bool needSetSiblingToFirst = extraItemInfo != null && extraItemInfo.Value.TemplateOrder == CommonUtils.EPrepareTemplateOrder.BeforeExtraItems;
		int extraItemCount = (extraItemInfo != null) ? extraItemInfo.GetValueOrDefault().ExtraItemCount : 0;
		int needSpawnCount = Math.Max(0, targetCount - (parent.childCount - extraItemCount));
		for (int i = 0; i < needSpawnCount; i++)
		{
			GameObject go = Object.Instantiate<GameObject>(template, parent);
			bool flag = needSetSiblingToFirst;
			if (flag)
			{
				go.transform.SetAsFirstSibling();
			}
		}
		for (int index = beforeOffset; index < beforeOffset + targetCount; index++)
		{
			parent.GetChild(index).gameObject.SetActive(true);
		}
		int hideStartIndex = beforeOffset + targetCount;
		int toHideCount = Math.Max(0, parent.childCount - extraItemCount - targetCount);
		for (int index2 = hideStartIndex; index2 < hideStartIndex + toHideCount; index2++)
		{
			parent.GetChild(index2).gameObject.SetActive(false);
		}
	}

	// Token: 0x06000B71 RID: 2929 RVA: 0x00049D4C File Offset: 0x00047F4C
	[return: TupleElementNames(new string[]
	{
		"type",
		"value"
	})]
	public unsafe static ValueTuple<sbyte, short> GetAttainmentWithSectApprovalBonus(sbyte orgTemplateId, short currAttainment, LifeSkillShorts lifeSkillAttainments)
	{
		bool flag = orgTemplateId == 0;
		ValueTuple<sbyte, short> result;
		if (flag)
		{
			result = new ValueTuple<sbyte, short>(-1, currAttainment);
		}
		else
		{
			SectApprovingEffectItem config = SectApprovingEffect.Instance[(int)(orgTemplateId - 1)];
			short maxAttainment = currAttainment;
			sbyte type = -1;
			foreach (sbyte lifeSkillType in config.RequirementSubstitutions)
			{
				bool flag2 = maxAttainment < *lifeSkillAttainments[(int)lifeSkillType];
				if (flag2)
				{
					maxAttainment = *lifeSkillAttainments[(int)lifeSkillType];
					type = lifeSkillType;
				}
			}
			result = new ValueTuple<sbyte, short>(type, maxAttainment);
		}
		return result;
	}

	// Token: 0x06000B72 RID: 2930 RVA: 0x00049DF4 File Offset: 0x00047FF4
	public static ECraftType GetCraftTypesByBuildingBlockId(short buildingId)
	{
		if (!true)
		{
		}
		ECraftType result;
		if (buildingId <= 149)
		{
			switch (buildingId)
			{
			case 127:
				result = ECraftType.Tea;
				goto IL_A3;
			case 128:
				result = ECraftType.Wine;
				goto IL_A3;
			case 129:
				result = ECraftType.Forging;
				goto IL_A3;
			default:
				if (buildingId == 139)
				{
					result = ECraftType.Woodworking;
					goto IL_A3;
				}
				if (buildingId == 149)
				{
					result = ECraftType.Medicine;
					goto IL_A3;
				}
				break;
			}
		}
		else if (buildingId <= 169)
		{
			if (buildingId == 159)
			{
				result = ECraftType.Toxicology;
				goto IL_A3;
			}
			if (buildingId == 169)
			{
				result = ECraftType.Weaving;
				goto IL_A3;
			}
		}
		else
		{
			if (buildingId == 179)
			{
				result = ECraftType.Jade;
				goto IL_A3;
			}
			if (buildingId == 203)
			{
				result = ECraftType.Cooking;
				goto IL_A3;
			}
		}
		throw new ArgumentException(string.Format("Unknown buildingId: {0}", buildingId));
		IL_A3:
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06000B73 RID: 2931 RVA: 0x00049EB0 File Offset: 0x000480B0
	public static string GetCraftTypesNameByEnum(ECraftType craftType)
	{
		string result;
		switch (craftType)
		{
		case ECraftType.Tea:
			result = LocalStringManager.Get(LanguageKey.LK_Craftsman_Tea);
			break;
		case ECraftType.Wine:
			result = LocalStringManager.Get(LanguageKey.LK_Craftsman_Alcohol);
			break;
		case ECraftType.Forging:
			result = LocalStringManager.Get(LanguageKey.LK_LifeSkillType_6);
			break;
		case ECraftType.Woodworking:
			result = LocalStringManager.Get(LanguageKey.LK_LifeSkillType_7);
			break;
		case ECraftType.Weaving:
			result = LocalStringManager.Get(LanguageKey.LK_LifeSkillType_10);
			break;
		case ECraftType.Medicine:
			result = LocalStringManager.Get(LanguageKey.LK_MedicineStorageMaterial);
			break;
		case ECraftType.Toxicology:
			result = LocalStringManager.Get(LanguageKey.LK_Make_Poison);
			break;
		case ECraftType.Cooking:
			result = LocalStringManager.Get(LanguageKey.LK_Cooking);
			break;
		case ECraftType.Jade:
			result = LocalStringManager.Get(LanguageKey.LK_LifeSkillType_11);
			break;
		default:
			result = "";
			break;
		}
		return result;
	}

	// Token: 0x06000B74 RID: 2932 RVA: 0x00049F70 File Offset: 0x00048170
	public static string GetCraftTypeIconByEnum(ECraftType craftType)
	{
		string iconName;
		switch (craftType)
		{
		case ECraftType.Tea:
			iconName = "building_tuan_caicha";
			break;
		case ECraftType.Wine:
			iconName = "building_tuan_qujiu";
			break;
		case ECraftType.Forging:
			iconName = "building_tuan_duanzao";
			break;
		case ECraftType.Woodworking:
			iconName = "building_tuan_zhimu";
			break;
		case ECraftType.Weaving:
			iconName = "building_tuan_zhijin";
			break;
		case ECraftType.Medicine:
			iconName = "building_tuan_zhiyao";
			break;
		case ECraftType.Toxicology:
			iconName = "building_tuan_liandu";
			break;
		case ECraftType.Cooking:
			iconName = "building_tuan_pengren";
			break;
		case ECraftType.Jade:
			iconName = "building_tuan_qiaojiang";
			break;
		default:
			iconName = "";
			break;
		}
		return iconName;
	}

	// Token: 0x06000B75 RID: 2933 RVA: 0x0004A010 File Offset: 0x00048210
	public static string GetCraftTypeIconDisableByEnum(ECraftType craftType)
	{
		string iconName;
		switch (craftType)
		{
		case ECraftType.Tea:
			iconName = "building_tuan_caicha_1";
			break;
		case ECraftType.Wine:
			iconName = "building_tuan_qujiu_1";
			break;
		case ECraftType.Forging:
			iconName = "building_tuan_duanzao_1";
			break;
		case ECraftType.Woodworking:
			iconName = "building_tuan_zhimu_1";
			break;
		case ECraftType.Weaving:
			iconName = "building_tuan_zhijin_1";
			break;
		case ECraftType.Medicine:
			iconName = "building_tuan_zhiyao_1";
			break;
		case ECraftType.Toxicology:
			iconName = "building_tuan_liandu_1";
			break;
		case ECraftType.Cooking:
			iconName = "building_tuan_pengren_1";
			break;
		case ECraftType.Jade:
			iconName = "building_tuan_qiaojiang_1";
			break;
		default:
			iconName = "";
			break;
		}
		return iconName;
	}

	// Token: 0x06000B76 RID: 2934 RVA: 0x0004A0B0 File Offset: 0x000482B0
	public static void SetActiveByMove(RectTransform rect, bool isActive)
	{
		bool flag = !rect;
		if (!flag)
		{
			rect.anchoredPosition = (isActive ? Vector2.zero : new Vector2(0f, -10000f));
		}
	}

	// Token: 0x06000B77 RID: 2935 RVA: 0x0004A0F0 File Offset: 0x000482F0
	public static bool CheckRoleDispatch(short templateId)
	{
		return ViewVillagerWork.EnabledRoleIdArray.Contains(templateId) || templateId == 0;
	}

	// Token: 0x06000B78 RID: 2936 RVA: 0x0004A118 File Offset: 0x00048318
	public static bool CheckShopLearnEvent(short shopEventId)
	{
		return ShopEvent.Instance[shopEventId].ShopEventType == 1;
	}

	// Token: 0x06000B79 RID: 2937 RVA: 0x0004A140 File Offset: 0x00048340
	public static List<MedicineItem> GetResultMedicine(short materialId)
	{
		List<MedicineItem> result = new List<MedicineItem>();
		MaterialItem matConfig = Config.Material.Instance[materialId];
		List<MakeItemTypeItem> makeItemTypes = (from t in matConfig.CraftableItemTypes
		select MakeItemType.Instance[t]).ToList<MakeItemTypeItem>();
		foreach (MakeItemTypeItem item in makeItemTypes)
		{
			List<MakeItemSubTypeItem> makeItemSubType = (from t in item.MakeItemSubTypes
			select MakeItemSubType.Instance[t]).ToList<MakeItemSubTypeItem>();
			result.AddRange((from t in makeItemSubType
			where t.Result.ItemType == 8
			select Medicine.Instance[t.Result.TemplateId]).ToList<MedicineItem>());
		}
		return result;
	}

	// Token: 0x06000B7A RID: 2938 RVA: 0x0004A264 File Offset: 0x00048464
	public static void GetCharacterPropReferenceDisplayInfo(short type, out string propertyName, out string icon)
	{
		bool flag = (int)type < CharacterPropertyReferenced.Instance.Count;
		if (flag)
		{
			CharacterPropertyDisplayItem configData = CharacterPropertyDisplay.Instance[CharacterPropertyReferenced.Instance[type].DisplayType];
			propertyName = configData.Name;
			icon = configData.TipsIcon;
		}
		else
		{
			CombatSkillPropertyItem configData2 = CombatSkillProperty.Instance[(int)type - CharacterPropertyReferenced.Instance.Count];
			propertyName = configData2.Name;
			icon = configData2.TipsIcon;
		}
	}

	// Token: 0x06000B7B RID: 2939 RVA: 0x0004A2DC File Offset: 0x000484DC
	public static void QueryJieqingSpecialInteractionUnlocked(Action<bool> callback, IAsyncMethodRequestHandler handler = null)
	{
		OrganizationDomainMethod.AsyncCall.GetSectFunctionStatus(handler, 13, SectFunctionStatuses.SectFunctionStatusType.SpecialInteractionUnlocked, delegate(int offset, RawDataPool pool)
		{
			bool unlocked = false;
			Serializer.Deserialize(pool, offset, ref unlocked);
			Action<bool> callback2 = callback;
			if (callback2 != null)
			{
				callback2(unlocked);
			}
		});
	}

	// Token: 0x06000B7C RID: 2940 RVA: 0x0004A310 File Offset: 0x00048510
	public static int GetCharacterMinimumExtraLegacyPoints(sbyte consummateLevel)
	{
		int index = Math.Clamp((int)consummateLevel, 0, GlobalConfig.Instance.CombatGetExpBase.Length - 1);
		return (int)GlobalConfig.Instance.CombatGetExpBase[index] * GlobalConfig.Instance.JieQingPointRequire / 100;
	}

	// Token: 0x06000B7D RID: 2941 RVA: 0x0004A352 File Offset: 0x00048552
	public static bool IsCharacterEligibleForJieqingSeizeFortune(int worth, sbyte consummateLevel)
	{
		return worth >= CommonUtils.GetCharacterMinimumExtraLegacyPoints(consummateLevel);
	}

	// Token: 0x06000B7E RID: 2942 RVA: 0x0004A360 File Offset: 0x00048560
	public static int GetJieqingSignLevelIndex(int point)
	{
		bool flag = point < 100;
		int result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			bool flag2 = point <= 189;
			if (flag2)
			{
				result = 0;
			}
			else
			{
				bool flag3 = point <= 349;
				if (flag3)
				{
					result = 1;
				}
				else
				{
					bool flag4 = point <= 589;
					if (flag4)
					{
						result = 2;
					}
					else
					{
						bool flag5 = point <= 909;
						if (flag5)
						{
							result = 3;
						}
						else
						{
							bool flag6 = point <= 1309;
							if (flag6)
							{
								result = 4;
							}
							else
							{
								bool flag7 = point <= 1789;
								if (flag7)
								{
									result = 5;
								}
								else
								{
									bool flag8 = point <= 2349;
									if (flag8)
									{
										result = 6;
									}
									else
									{
										bool flag9 = point <= 2989;
										if (flag9)
										{
											result = 7;
										}
										else
										{
											bool flag10 = point <= 3340;
											if (flag10)
											{
												result = 8;
											}
											else
											{
												result = 8;
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06000B7F RID: 2943 RVA: 0x0004A450 File Offset: 0x00048650
	public static List<short> GetSectOrgListByFlag(int sign)
	{
		CommonUtils.<>c__DisplayClass264_0 CS$<>8__locals1;
		CS$<>8__locals1.sign = sign;
		CS$<>8__locals1.result = new List<short>();
		bool flag = CS$<>8__locals1.sign == -1;
		List<short> result;
		if (flag)
		{
			CS$<>8__locals1.result.Add(1);
			CS$<>8__locals1.result.Add(2);
			CS$<>8__locals1.result.Add(3);
			CS$<>8__locals1.result.Add(4);
			CS$<>8__locals1.result.Add(5);
			CS$<>8__locals1.result.Add(11);
			CS$<>8__locals1.result.Add(12);
			CS$<>8__locals1.result.Add(13);
			CS$<>8__locals1.result.Add(14);
			CS$<>8__locals1.result.Add(15);
			result = CS$<>8__locals1.result;
		}
		else
		{
			EJieQingSignState signFlag = (EJieQingSignState)CS$<>8__locals1.sign;
			bool flag2 = !signFlag.HasFlag(EJieQingSignState.AllSect);
			if (flag2)
			{
				result = CS$<>8__locals1.result;
			}
			else
			{
				bool flag3 = signFlag.HasFlag(EJieQingSignState.HonourSect);
				if (flag3)
				{
					CommonUtils.<GetSectOrgListByFlag>g__CheckSect|264_0(1, ref CS$<>8__locals1);
					CommonUtils.<GetSectOrgListByFlag>g__CheckSect|264_0(2, ref CS$<>8__locals1);
					CommonUtils.<GetSectOrgListByFlag>g__CheckSect|264_0(3, ref CS$<>8__locals1);
					CommonUtils.<GetSectOrgListByFlag>g__CheckSect|264_0(4, ref CS$<>8__locals1);
					CommonUtils.<GetSectOrgListByFlag>g__CheckSect|264_0(5, ref CS$<>8__locals1);
				}
				CommonUtils.<GetSectOrgListByFlag>g__CheckSect|264_0(6, ref CS$<>8__locals1);
				CommonUtils.<GetSectOrgListByFlag>g__CheckSect|264_0(7, ref CS$<>8__locals1);
				CommonUtils.<GetSectOrgListByFlag>g__CheckSect|264_0(8, ref CS$<>8__locals1);
				CommonUtils.<GetSectOrgListByFlag>g__CheckSect|264_0(9, ref CS$<>8__locals1);
				CommonUtils.<GetSectOrgListByFlag>g__CheckSect|264_0(10, ref CS$<>8__locals1);
				bool flag4 = signFlag.HasFlag(EJieQingSignState.DishonourSect);
				if (flag4)
				{
					CommonUtils.<GetSectOrgListByFlag>g__CheckSect|264_0(11, ref CS$<>8__locals1);
					CommonUtils.<GetSectOrgListByFlag>g__CheckSect|264_0(12, ref CS$<>8__locals1);
					CommonUtils.<GetSectOrgListByFlag>g__CheckSect|264_0(13, ref CS$<>8__locals1);
					CommonUtils.<GetSectOrgListByFlag>g__CheckSect|264_0(14, ref CS$<>8__locals1);
					CommonUtils.<GetSectOrgListByFlag>g__CheckSect|264_0(15, ref CS$<>8__locals1);
				}
				result = CS$<>8__locals1.result;
			}
		}
		return result;
	}

	// Token: 0x06000B80 RID: 2944 RVA: 0x0004A61C File Offset: 0x0004881C
	public static List<short> GetJieqingMurderMapOrgFilter()
	{
		int sign = SingletonObject.getInstance<GlobalSettings>().JieQingMurderSignDisplay;
		List<short> result = CommonUtils.GetSectOrgListByFlag(sign);
		bool flag = result.Count > 0;
		List<short> result2;
		if (flag)
		{
			result2 = result;
		}
		else
		{
			result2 = new List<short>(CommonUtils.JieqingMurderMapOrgIds);
		}
		return result2;
	}

	// Token: 0x06000B81 RID: 2945 RVA: 0x0004A65C File Offset: 0x0004885C
	public static bool CheckSectFlag(int sign, int orgTemplateId)
	{
		int orgSign = 1 << orgTemplateId;
		bool flag = !((EJieQingSignState)sign).HasFlag(EJieQingSignState.AllSect);
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool isHonour = orgTemplateId == 1 || orgTemplateId == 2 || orgTemplateId == 3 || orgTemplateId == 4 || orgTemplateId == 5;
			bool flag2 = isHonour && !((EJieQingSignState)sign).HasFlag(EJieQingSignState.HonourSect);
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool flag3 = !isHonour && !((EJieQingSignState)sign).HasFlag(EJieQingSignState.DishonourSect);
				result = (!flag3 && (sign & orgSign) != 0);
			}
		}
		return result;
	}

	// Token: 0x06000B82 RID: 2946 RVA: 0x0004A70C File Offset: 0x0004890C
	public static bool CheckSectIsHonour(int orgTemplateId)
	{
		return orgTemplateId == 1 || orgTemplateId == 2 || orgTemplateId == 3 || orgTemplateId == 4 || orgTemplateId == 5;
	}

	// Token: 0x06000B83 RID: 2947 RVA: 0x0004A738 File Offset: 0x00048938
	public static bool CheckSectIsDishonour(int orgTemplateId)
	{
		return orgTemplateId == 11 || orgTemplateId == 12 || orgTemplateId == 13 || orgTemplateId == 14 || orgTemplateId == 15;
	}

	// Token: 0x06000B84 RID: 2948 RVA: 0x0004A768 File Offset: 0x00048968
	public static void ShowCombatSkillQuickEdit(int charId, CombatSkillDisplayData combatSkillDisplayData, RectTransform referenceRect)
	{
		UIElement.CombatSkillQuickEdit.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharId", charId).SetObject("ReferenceRect", referenceRect).SetObject("CombatSkillDisplayData", combatSkillDisplayData));
		UIManager.Instance.ShowUI(UIElement.CombatSkillQuickEdit, true);
	}

	// Token: 0x06000B85 RID: 2949 RVA: 0x0004A7B8 File Offset: 0x000489B8
	public static IEnumerable<sbyte> IterCityOrganizationKeys()
	{
		yield return 21;
		yield return 22;
		yield return 23;
		yield return 24;
		yield return 25;
		yield return 26;
		yield return 27;
		yield return 28;
		yield return 29;
		yield return 30;
		yield return 31;
		yield return 32;
		yield return 33;
		yield return 34;
		yield return 35;
		yield break;
	}

	// Token: 0x06000B86 RID: 2950 RVA: 0x0004A7C4 File Offset: 0x000489C4
	public static bool IsMerchantFavorabilityReachProgressLimit(int merchantFavorability, out int worldProgressLimitedLevel, out int worldProgressLimitedFavor)
	{
		sbyte worldProgress = SingletonObject.getInstance<BasicGameData>().XiangshuProgress;
		int length = GlobalConfig.Instance.MerchantFavorabilityXiangshuLevelRequirements.Length;
		int overIndex = length;
		for (int i = 0; i < length; i++)
		{
			bool flag = GlobalConfig.Instance.MerchantFavorabilityXiangshuLevelRequirements[i] > (int)worldProgress;
			if (flag)
			{
				overIndex = i;
				break;
			}
		}
		worldProgressLimitedFavor = overIndex * 10;
		worldProgressLimitedLevel = GameData.Domains.Merchant.SharedMethods.GetFavorLevel(worldProgressLimitedFavor);
		return merchantFavorability >= worldProgressLimitedFavor;
	}

	// Token: 0x06000B87 RID: 2951 RVA: 0x0004A838 File Offset: 0x00048A38
	public static string GetConsummateLevelTips(int i)
	{
		ChallengeModeData challengeModeData = SingletonObject.getInstance<BasicGameData>().ChallengeModeData;
		sbyte actualLevel = (sbyte)i;
		challengeModeData.ApplyChallengeModeLimitedNeiliAllocation(ref actualLevel);
		int value = ConsummateLevel.Instance[actualLevel].MaxNeiliAllocation - ConsummateLevel.Instance[0].MaxNeiliAllocation;
		return LocalStringManager.GetFormat(LanguageKey.LK_Consummate_Level_Tips, i).ColorReplace() + "\n" + LocalStringManager.GetFormat(LanguageKey.LK_Consummate_Neili_Tips, value).ColorReplace();
	}

	// Token: 0x06000B88 RID: 2952 RVA: 0x0004A8B8 File Offset: 0x00048AB8
	public static string GetWisdomIcon(sbyte wisdomType)
	{
		if (!true)
		{
		}
		string text;
		switch (wisdomType)
		{
		case -1:
			text = MedalSummary.WisdomMedalIconConfig[0];
			break;
		case 0:
			text = MedalSummary.WisdomMedalIconConfig[1];
			break;
		case 1:
			text = MedalSummary.WisdomMedalIconConfig[-1];
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (!true)
		{
		}
		string medalIndex = text;
		return "ui9_icon_strategy_big_" + medalIndex;
	}

	// Token: 0x06000B89 RID: 2953 RVA: 0x0004A928 File Offset: 0x00048B28
	public static int GetYearByDate(int date)
	{
		return Mathf.CeilToInt(((float)date + 0.5f) / 12f);
	}

	// Token: 0x06000B8A RID: 2954 RVA: 0x0004A950 File Offset: 0x00048B50
	public static int GetMonthByDate(int date)
	{
		return (int)((sbyte)(date % 12));
	}

	// Token: 0x06000B8B RID: 2955 RVA: 0x0004A968 File Offset: 0x00048B68
	public static Color GetFiveElementColor(int fiveElements)
	{
		return Colors.Instance.PersonalityTypeColors[fiveElements];
	}

	// Token: 0x06000B8C RID: 2956 RVA: 0x0004A98C File Offset: 0x00048B8C
	public static void SetMixPoisonBorder(GameObject border, int charId)
	{
		CharacterDomainMethod.AsyncCall.GetMixedPoisonTypeRelatedMarkCountArray(null, charId, delegate(int offset, RawDataPool dataPool)
		{
			int[] markCount = new int[20];
			Serializer.Deserialize(dataPool, offset, ref markCount);
			bool hasMixPoison = false;
			for (int i = 0; i < markCount.Length; i++)
			{
				bool flag = markCount[i] > 0;
				if (flag)
				{
					hasMixPoison = true;
					break;
				}
			}
			GameObject border2 = border;
			if (border2 != null)
			{
				border2.SetActive(hasMixPoison);
			}
			bool flag2 = border != null && hasMixPoison;
			if (flag2)
			{
				border.GetComponent<TooltipInvoker>().RuntimeParam = EasyPool.Get<ArgumentBox>().Set("CharacterId", charId).SetObject("MarkCount", markCount);
			}
		});
	}

	// Token: 0x06000B8D RID: 2957 RVA: 0x0004A9C8 File Offset: 0x00048BC8
	public static string GetPoisonNameColor(sbyte poisonType)
	{
		return CommonUtils.PoisonNameColor[(int)poisonType];
	}

	// Token: 0x06000B8E RID: 2958 RVA: 0x0004A9E4 File Offset: 0x00048BE4
	public static string GetPoisonName(sbyte poisonType)
	{
		return LocalStringManager.Get(CommonUtils.PoisonNameLStrings[(int)poisonType]);
	}

	// Token: 0x06000B8F RID: 2959 RVA: 0x0004AA04 File Offset: 0x00048C04
	public static string GetPercentageText(LanguageKey key, int value)
	{
		if (!true)
		{
		}
		string arg;
		if (value >= 100)
		{
			if (value != 100)
			{
				arg = "brightblue";
			}
			else
			{
				arg = "brightyellow";
			}
		}
		else
		{
			arg = "brightred";
		}
		if (!true)
		{
		}
		return key.TrFormat(arg, value).ColorReplace();
	}

	// Token: 0x06000B90 RID: 2960 RVA: 0x0004AA54 File Offset: 0x00048C54
	[return: TupleElementNames(new string[]
	{
		"color",
		"content"
	})]
	public static ValueTuple<string, string> GetValueColorAndText(int value, bool isPositive = true, int baseValue = 0, bool showPlusMark = true, bool isPercentage = false)
	{
		int num = isPositive ? (value - baseValue) : (baseValue - value);
		if (!true)
		{
		}
		string text;
		if (num >= 0)
		{
			if (num != 0)
			{
				text = "brightblue";
			}
			else
			{
				text = "brightyellow";
			}
		}
		else
		{
			text = "brightred";
		}
		if (!true)
		{
		}
		string item = text;
		bool flag = showPlusMark && value > 0;
		if (!true)
		{
		}
		string item2;
		if (flag)
		{
			if (!isPercentage)
			{
				item2 = string.Format("+{0}", value);
			}
			else
			{
				item2 = string.Format("+{0}%", value);
			}
		}
		else if (!isPercentage)
		{
			item2 = value.ToString();
		}
		else
		{
			item2 = string.Format("{0}%", value);
		}
		if (!true)
		{
		}
		return new ValueTuple<string, string>(item, item2);
	}

	// Token: 0x06000B91 RID: 2961 RVA: 0x0004AB08 File Offset: 0x00048D08
	public static string GetColoredValue(int value, bool isPositive = true, int baseValue = 0, bool showPlusMark = true, bool isPercentage = false)
	{
		ValueTuple<string, string> valueColorAndText = CommonUtils.GetValueColorAndText(value, isPositive, baseValue, showPlusMark, isPercentage);
		string color = valueColorAndText.Item1;
		string content = valueColorAndText.Item2;
		return content.SetColor(color);
	}

	// Token: 0x06000B92 RID: 2962 RVA: 0x0004AB39 File Offset: 0x00048D39
	public static void SetPercentageText(this TMP_Text text, LanguageKey key, int value)
	{
		text.text = CommonUtils.GetPercentageText(key, value);
	}

	// Token: 0x06000B93 RID: 2963 RVA: 0x0004AB4C File Offset: 0x00048D4C
	// Note: this type is marked as 'beforefieldinit'.
	static CommonUtils()
	{
		LanguageKey[] array = new LanguageKey[10];
		RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.B873D9FC3DC4BACF85ED5D05B6E27E7437F070A0C2409A89AA8F457E98EA6822).FieldHandle);
		CommonUtils.DigitLanguageKeys = array;
		LanguageKey[] array2 = new LanguageKey[11];
		RuntimeHelpers.InitializeArray(array2, fieldof(<PrivateImplementationDetails>.93645C824B8BB2ED6E7156B5A55F58CB6C069A03E0C6E2AE2752D1A9A0DB0549).FieldHandle);
		CommonUtils.TraditionalChineseNumber = array2;
		LanguageKey[] array3 = new LanguageKey[13];
		RuntimeHelpers.InitializeArray(array3, fieldof(<PrivateImplementationDetails>.FF9B192CA48737DB949B5E5E85971C22D402A366DFB92598AE0D2D6D394C03A9).FieldHandle);
		CommonUtils.FavorabilityLangIdArray = array3;
		CommonUtils.FavorabilityIconArray = new string[]
		{
			"sp_icon_favorability_hateful6",
			"sp_icon_favorability_hateful5",
			"sp_icon_favorability_hateful4",
			"sp_icon_favorability_hateful3",
			"sp_icon_favorability_hateful2",
			"sp_icon_favorability_hateful1",
			"sp_icon_favorability_unfamiliar",
			"sp_icon_favorability_favorite1",
			"sp_icon_favorability_favorite2",
			"sp_icon_favorability_favorite3",
			"sp_icon_favorability_favorite4",
			"sp_icon_favorability_favorite5",
			"sp_icon_favorability_favorite6"
		};
		CommonUtils.FameStringConfig = new List<LanguageKey>
		{
			LanguageKey.LK_FameLevel_0,
			LanguageKey.LK_FameLevel_1,
			LanguageKey.LK_FameLevel_2,
			LanguageKey.LK_FameLevel_3,
			LanguageKey.LK_FameLevel_4,
			LanguageKey.LK_FameLevel_5,
			LanguageKey.LK_FameLevel_6,
			LanguageKey.LK_FameLevel_7
		};
		CommonUtils.FameIconConfig = new string[]
		{
			"sp_icon_fame_0",
			"sp_icon_fame_1",
			"sp_icon_fame_2",
			"sp_icon_fame_3",
			"sp_icon_fame_4",
			"sp_icon_fame_5",
			"sp_icon_fame_6"
		};
		LanguageKey[] array4 = new LanguageKey[7];
		RuntimeHelpers.InitializeArray(array4, fieldof(<PrivateImplementationDetails>.6A378A302C8FCE3A8D4356CB4A998A0D58C4750CA41A513790172E633DD47350).FieldHandle);
		CommonUtils.HappinessLevelConfig = array4;
		CommonUtils.HappinessLevelIconConfig = new string[]
		{
			"sp_icon_happiness_0",
			"sp_icon_happiness_1",
			"sp_icon_happiness_2",
			"sp_icon_happiness_3",
			"sp_icon_happiness_4",
			"sp_icon_happiness_5",
			"sp_icon_happiness_6"
		};
		CommonUtils.CharacterFiveElementsIcons = new string[]
		{
			"ui_sp_icon_fiveelements_0",
			"ui_sp_icon_fiveelements_1",
			"ui_sp_icon_fiveelements_2",
			"ui_sp_icon_fiveelements_3",
			"ui_sp_icon_fiveelements_4",
			"ui_sp_icon_fiveelements_5"
		};
		LanguageKey[] array5 = new LanguageKey[6];
		RuntimeHelpers.InitializeArray(array5, fieldof(<PrivateImplementationDetails>.298383F02DDA1EC45BF6949DFF29CB8DE07262412CF7D8C8291F2416230A2383).FieldHandle);
		CommonUtils.CharacterFiveElementsNameIds = array5;
		CommonUtils.CharmLevelDisplayLanguageIds = new LanguageKey[][]
		{
			new LanguageKey[]
			{
				LanguageKey.LK_Charm_Level_1
			},
			new LanguageKey[]
			{
				LanguageKey.LK_Charm_Level_2
			},
			new LanguageKey[]
			{
				LanguageKey.LK_Charm_Level_3
			},
			new LanguageKey[]
			{
				LanguageKey.LK_Charm_Level_4
			},
			new LanguageKey[]
			{
				LanguageKey.LK_Charm_Level_5
			},
			new LanguageKey[]
			{
				LanguageKey.LK_Charm_Level_6_A,
				LanguageKey.LK_Charm_Level_6_B
			},
			new LanguageKey[]
			{
				LanguageKey.LK_Charm_Level_7_A,
				LanguageKey.LK_Charm_Level_7_B
			},
			new LanguageKey[]
			{
				LanguageKey.LK_Charm_Level_8_A,
				LanguageKey.LK_Charm_Level_8_B
			},
			new LanguageKey[]
			{
				LanguageKey.LK_Charm_Level_9
			}
		};
		CommonUtils.CharmLevelDisplayIcons = new string[]
		{
			"sp_icon_charm_0",
			"sp_icon_charm_1",
			"sp_icon_charm_2",
			"sp_icon_charm_3",
			"sp_icon_charm_4",
			"sp_icon_charm_5",
			"sp_icon_charm_6",
			"sp_icon_charm_7",
			"sp_icon_charm_8"
		};
		LanguageKey[] array6 = new LanguageKey[6];
		RuntimeHelpers.InitializeArray(array6, fieldof(<PrivateImplementationDetails>.DA9C6965616067EF47972D97CA3B6FDEDFCA2256729DBCEED2C7BB587F9DB24D).FieldHandle);
		CommonUtils.CharacterHealthLevelIds = array6;
		CommonUtils.DisorderOfQiLevelColors = new string[]
		{
			"BehaviorType_Just",
			"BehaviorType_Even",
			"BehaviorType_Even",
			"BehaviorType_Even",
			"brightred"
		};
		LanguageKey[] array7 = new LanguageKey[5];
		RuntimeHelpers.InitializeArray(array7, fieldof(<PrivateImplementationDetails>.E23492D894508AE955CA047489440C5B44AB511667C0ACFD8B9553E5949F84DA).FieldHandle);
		CommonUtils.DisorderOfQiLevelLangKeys = array7;
		CommonUtils.ResourceCountThreshold = new int[]
		{
			100,
			500,
			1000,
			2500,
			5000,
			10000,
			25000,
			50000,
			100000,
			150000,
			200000
		};
		CommonUtils.MoneyCountThreshold = new int[]
		{
			500,
			2500,
			5000,
			12500,
			25000,
			50000,
			125000,
			250000,
			500000,
			750000,
			1000000
		};
		CommonUtils.AuthorityCountThreshold = new int[]
		{
			50,
			250,
			500,
			1250,
			2500,
			5000,
			12500,
			25000,
			50000,
			75000,
			100000
		};
		CommonUtils.ResourceImageScaleConfig = new float[][]
		{
			new float[]
			{
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f
			},
			new float[]
			{
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f
			},
			new float[]
			{
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f
			},
			new float[]
			{
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f
			},
			new float[]
			{
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f
			},
			new float[]
			{
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f
			},
			new float[]
			{
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				0.9f
			},
			new float[]
			{
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f
			},
			new float[]
			{
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f
			}
		};
		CommonUtils.ResourceFolderNames = new string[]
		{
			"ResourceLevel_Food",
			"ResourceLevel_Wood",
			"ResourceLevel_Metal",
			"ResourceLevel_Jade",
			"ResourceLevel_Fabric",
			"ResourceLevel_Herb",
			"ResourceLevel_Money",
			"ResourceLevel_Authority",
			"ResourceLevel_Exp"
		};
		CommonUtils.AngelMapping = new Dictionary<int, ValueTuple<float, float>>
		{
			{
				1,
				new ValueTuple<float, float>(315f, 315f)
			},
			{
				2,
				new ValueTuple<float, float>(305f, 325f)
			},
			{
				3,
				new ValueTuple<float, float>(297f, 333f)
			},
			{
				4,
				new ValueTuple<float, float>(291f, 339f)
			},
			{
				5,
				new ValueTuple<float, float>(287f, 343f)
			},
			{
				6,
				new ValueTuple<float, float>(285f, 345f)
			}
		};
		CommonUtils.AngelMapping2 = new Dictionary<int, ValueTuple<float, float>>
		{
			{
				1,
				new ValueTuple<float, float>(315f, 315f)
			},
			{
				2,
				new ValueTuple<float, float>(302f, 328f)
			},
			{
				3,
				new ValueTuple<float, float>(290f, 339f)
			},
			{
				4,
				new ValueTuple<float, float>(282f, 348f)
			},
			{
				5,
				new ValueTuple<float, float>(274f, 355f)
			},
			{
				6,
				new ValueTuple<float, float>(275f, 360f)
			}
		};
		CommonUtils.FiveLongEffectMapping = new Dictionary<short, string>
		{
			{
				249,
				"eff_charactermenu_huo"
			},
			{
				246,
				"eff_charactermenu_jin"
			},
			{
				247,
				"eff_charactermenu_shui"
			},
			{
				248,
				"eff_charactermenu_mu"
			},
			{
				250,
				"eff_charactermenu_tu"
			}
		};
		CommonUtils.FiveLongImageMapping = new Dictionary<short, string>
		{
			{
				249,
				"fiveloong_mark_3"
			},
			{
				246,
				"fiveloong_mark_0"
			},
			{
				247,
				"fiveloong_mark_2"
			},
			{
				248,
				"fiveloong_mark_4"
			},
			{
				250,
				"fiveloong_mark_1"
			}
		};
		CommonUtils.JieqingMurderMapOrgIds = new short[]
		{
			1,
			2,
			4,
			3,
			5,
			6,
			7,
			9,
			8,
			10,
			11,
			12,
			13,
			14,
			15
		};
		LanguageKey[] array8 = new LanguageKey[6];
		RuntimeHelpers.InitializeArray(array8, fieldof(<PrivateImplementationDetails>.ABC27D63F181A80374AC8C8CEF5B348F27CC1549748405CCE62EAA3DA431BC08).FieldHandle);
		CommonUtils.PoisonNameLStrings = array8;
		CommonUtils.PoisonNameColor = new string[]
		{
			"<color=#hotpoison>{0}</color>",
			"<color=#gloomypoison>{0}</color>",
			"<color=#coldpoison>{0}</color>",
			"<color=#redpoison>{0}</color>",
			"<color=#rottenpoison>{0}</color>",
			"<color=#illusorypoison>{0}</color>"
		};
	}

	// Token: 0x06000B94 RID: 2964 RVA: 0x0004B290 File Offset: 0x00049490
	[CompilerGenerated]
	internal static void <GetSectOrgListByFlag>g__CheckSect|264_0(short orgId, ref CommonUtils.<>c__DisplayClass264_0 A_1)
	{
		int orgFlag = 1 << (int)orgId;
		bool flag = (A_1.sign & orgFlag) == orgFlag;
		if (flag)
		{
			A_1.result.Add(orgId);
		}
	}

	// Token: 0x04000D81 RID: 3457
	public static readonly LanguageKey[] DigitLanguageKeys;

	// Token: 0x04000D82 RID: 3458
	public static readonly LanguageKey[] TraditionalChineseNumber;

	// Token: 0x04000D83 RID: 3459
	private static readonly LanguageKey[] FavorabilityLangIdArray;

	// Token: 0x04000D84 RID: 3460
	private static readonly string[] FavorabilityIconArray;

	// Token: 0x04000D85 RID: 3461
	private static readonly List<LanguageKey> FameStringConfig;

	// Token: 0x04000D86 RID: 3462
	private static readonly string[] FameIconConfig;

	// Token: 0x04000D87 RID: 3463
	private static readonly LanguageKey[] HappinessLevelConfig;

	// Token: 0x04000D88 RID: 3464
	private static readonly string[] HappinessLevelIconConfig;

	// Token: 0x04000D89 RID: 3465
	public static readonly string[] CharacterFiveElementsIcons;

	// Token: 0x04000D8A RID: 3466
	private static readonly LanguageKey[] CharacterFiveElementsNameIds;

	// Token: 0x04000D8B RID: 3467
	private static Color[] _skillNameColors;

	// Token: 0x04000D8C RID: 3468
	private static readonly LanguageKey[][] CharmLevelDisplayLanguageIds;

	// Token: 0x04000D8D RID: 3469
	private static readonly string[] CharmLevelDisplayIcons;

	// Token: 0x04000D8E RID: 3470
	private const int TenThousand = 10000;

	// Token: 0x04000D8F RID: 3471
	private const int OneHundredMillion = 100000000;

	// Token: 0x04000D90 RID: 3472
	public static readonly LanguageKey[] CharacterHealthLevelIds;

	// Token: 0x04000D91 RID: 3473
	public static readonly string[] DisorderOfQiLevelColors;

	// Token: 0x04000D92 RID: 3474
	public static readonly LanguageKey[] DisorderOfQiLevelLangKeys;

	// Token: 0x04000D93 RID: 3475
	private static readonly int[] ResourceCountThreshold;

	// Token: 0x04000D94 RID: 3476
	private static readonly int[] MoneyCountThreshold;

	// Token: 0x04000D95 RID: 3477
	private static readonly int[] AuthorityCountThreshold;

	// Token: 0x04000D96 RID: 3478
	private static readonly float[][] ResourceImageScaleConfig;

	// Token: 0x04000D97 RID: 3479
	private static readonly string[] ResourceFolderNames;

	// Token: 0x04000D98 RID: 3480
	private const string ProfessionCommonBackgroundMidfix = "profession_tubiao_";

	// Token: 0x04000D99 RID: 3481
	[TupleElementNames(new string[]
	{
		"startAngel",
		"endAngle"
	})]
	public static readonly Dictionary<int, ValueTuple<float, float>> AngelMapping;

	// Token: 0x04000D9A RID: 3482
	[TupleElementNames(new string[]
	{
		"startAngel",
		"endAngle"
	})]
	public static readonly Dictionary<int, ValueTuple<float, float>> AngelMapping2;

	// Token: 0x04000D9B RID: 3483
	public static readonly Dictionary<short, string> FiveLongEffectMapping;

	// Token: 0x04000D9C RID: 3484
	public static readonly Dictionary<short, string> FiveLongImageMapping;

	// Token: 0x04000D9D RID: 3485
	private static readonly short[] JieqingMurderMapOrgIds;

	// Token: 0x04000D9E RID: 3486
	public static readonly LanguageKey[] PoisonNameLStrings;

	// Token: 0x04000D9F RID: 3487
	private static readonly string[] PoisonNameColor;

	// Token: 0x02001167 RID: 4455
	public enum EDisplayGender
	{
		// Token: 0x040096ED RID: 38637
		Male,
		// Token: 0x040096EE RID: 38638
		Female,
		// Token: 0x040096EF RID: 38639
		Hidden
	}

	// Token: 0x02001168 RID: 4456
	public enum EPrepareTemplateOrder
	{
		// Token: 0x040096F1 RID: 38641
		BeforeExtraItems,
		// Token: 0x040096F2 RID: 38642
		AfterExtraItems
	}

	// Token: 0x02001169 RID: 4457
	public struct PrepareExtraItemInfo
	{
		// Token: 0x040096F3 RID: 38643
		public int ExtraItemCount;

		// Token: 0x040096F4 RID: 38644
		public CommonUtils.EPrepareTemplateOrder TemplateOrder;
	}
}
