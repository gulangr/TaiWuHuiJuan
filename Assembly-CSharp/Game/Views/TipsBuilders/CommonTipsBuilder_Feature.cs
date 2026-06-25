using System;
using System.Collections.Generic;
using System.Text;
using Config;
using FrameWork;
using Game.Views.MouseTips;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.TipsBuilders
{
	// Token: 0x0200074D RID: 1869
	[CommonTipBuilder(28, TipType.Feature)]
	public class CommonTipsBuilder_Feature : ICommonTipBuilder
	{
		// Token: 0x06005ABA RID: 23226 RVA: 0x002A1050 File Offset: 0x0029F250
		public ArgumentBox BuildTip(CommonTipItem commonTipItem, ArgumentBox arg)
		{
			CommonTipSimpleRuntime runtime = CommonTipsHelper.GetOrCreateSimpleRuntimeTipForBuild(commonTipItem, arg);
			runtime.ShowAllAtoms();
			short featureId;
			arg.Get("FeatureId", out featureId);
			int characterId;
			arg.Get("CharacterId", out characterId);
			bool templateDataOnly;
			arg.Get("TemplateDataOnly", out templateDataOnly);
			CharacterFeatureItem configData = CharacterFeature.Instance[featureId];
			bool isInGame = GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame;
			bool showSmallVillageName = !templateDataOnly && isInGame && GameData.Domains.World.SharedMethods.SmallVillageXiangshuProgress() && (configData.TemplateId == 210 || configData.TemplateId == 211);
			runtime.Set("CharacterFeature.Name", showSmallVillageName ? configData.SmallVillageName : configData.Name);
			runtime.Set("CharacterFeature.Desc", showSmallVillageName ? configData.SmallVillageDesc : configData.Desc);
			string effectDesc = string.IsNullOrEmpty(configData.EffectDesc) ? string.Empty : ((configData.TemplateId == 738) ? configData.EffectDesc.GetFormat(configData.Duration.ToString()) : configData.EffectDesc);
			runtime.Set("CharacterFeature.EffectDesc", effectDesc);
			bool hasAnyEffectItem = false;
			Debug.Log(string.Format("[PBTest][CommonTip] Building tip for CharacterFeature {0} (ID: {1}), TemplateDataOnly: {2}", configData.Name, featureId, templateDataOnly));
			hasAnyEffectItem |= CommonTipsBuilder_Feature.SetIndexedArray(runtime, "CombatSkillPowerBonuses", "CharacterFeature.CombatSkillPowerBonuses", configData.CombatSkillPowerBonuses, new Func<int, string>(CommonTipsBuilder_Feature.FormatSignedPercent));
			hasAnyEffectItem |= CommonTipsBuilder_Feature.SetIndexedArray(runtime, "FiveElementPowerBonuses", "CharacterFeature.FiveElementPowerBonuses", configData.FiveElementPowerBonuses, new Func<int, string>(CommonTipsBuilder_Feature.FormatSignedPercent));
			hasAnyEffectItem |= CommonTipsBuilder_Feature.SetIndexedArray(runtime, "CombatSkillSlotBonuses", "CharacterFeature.CombatSkillSlotBonuses", configData.CombatSkillSlotBonuses, new Func<int, string>(CommonTipsBuilder_Feature.FormatSignedValue));
			hasAnyEffectItem |= CommonTipsBuilder_Feature.SetScalarAtom(runtime, "PersonalityCalm", "CharacterFeature.PersonalityCalm", (int)configData.PersonalityCalm, new Func<int, string>(CommonTipsBuilder_Feature.FormatSignedValue));
			hasAnyEffectItem |= CommonTipsBuilder_Feature.SetScalarAtom(runtime, "PersonalityClever", "CharacterFeature.PersonalityClever", (int)configData.PersonalityClever, new Func<int, string>(CommonTipsBuilder_Feature.FormatSignedValue));
			hasAnyEffectItem |= CommonTipsBuilder_Feature.SetScalarAtom(runtime, "PersonalityEnthusiastic", "CharacterFeature.PersonalityEnthusiastic", (int)configData.PersonalityEnthusiastic, new Func<int, string>(CommonTipsBuilder_Feature.FormatSignedValue));
			hasAnyEffectItem |= CommonTipsBuilder_Feature.SetScalarAtom(runtime, "PersonalityBrave", "CharacterFeature.PersonalityBrave", (int)configData.PersonalityBrave, new Func<int, string>(CommonTipsBuilder_Feature.FormatSignedValue));
			hasAnyEffectItem |= CommonTipsBuilder_Feature.SetScalarAtom(runtime, "PersonalityFirm", "CharacterFeature.PersonalityFirm", (int)configData.PersonalityFirm, new Func<int, string>(CommonTipsBuilder_Feature.FormatSignedValue));
			hasAnyEffectItem |= CommonTipsBuilder_Feature.SetScalarAtom(runtime, "PersonalityLucky", "CharacterFeature.PersonalityLucky", (int)configData.PersonalityLucky, new Func<int, string>(CommonTipsBuilder_Feature.FormatSignedValue));
			hasAnyEffectItem |= CommonTipsBuilder_Feature.SetScalarAtom(runtime, "PersonalityPerceptive", "CharacterFeature.PersonalityPerceptive", (int)configData.PersonalityPerceptive, new Func<int, string>(CommonTipsBuilder_Feature.FormatSignedValue));
			hasAnyEffectItem |= CommonTipsBuilder_Feature.SetScalarAtom(runtime, "FavorabilityIncrementFactor01", "CharacterFeature.FavorabilityIncrementFactor", (int)(configData.FavorabilityIncrementFactor - 100), (int value) => CommonTipsBuilder_Feature.FormatSignedPercent(value, true));
			hasAnyEffectItem |= CommonTipsBuilder_Feature.SetScalarAtom(runtime, "FavorabilityIncrementFactor02", "CharacterFeature.FavorabilityDecrementFactor", (int)(configData.FavorabilityDecrementFactor - 100), (int value) => CommonTipsBuilder_Feature.FormatSignedPercent(value, false));
			hasAnyEffectItem |= CommonTipsBuilder_Feature.SetScalarAtom(runtime, "FavorabilityIncrementFactor03", "CharacterFeature.AdoreMultiplePeopleChanceFactor", (int)(configData.AdoreMultiplePeopleChanceFactor - 100), (int _) => CommonTipsBuilder_Feature.FormatAdoreFactor((int)configData.AdoreMultiplePeopleChanceFactor));
			hasAnyEffectItem |= CommonTipsBuilder_Feature.SetScalarAtom(runtime, "FavorabilityIncrementFactor04", "CharacterFeature.QiDisorderDelta", (int)configData.QiDisorderDelta, new Func<int, string>(CommonTipsBuilder_Feature.FormatSignedValue));
			hasAnyEffectItem |= CommonTipsBuilder_Feature.SetScalarAtom(runtime, "FavorabilityIncrementFactor05", "CharacterFeature.HealthDelta", configData.HealthDelta, new Func<int, string>(CommonTipsBuilder_Feature.FormatSignedValue));
			hasAnyEffectItem |= CommonTipsBuilder_Feature.SetScalarAtom(runtime, "FavorabilityIncrementFactor06", "CharacterFeature.MaxNeiliAllocationDebuff", configData.MaxNeiliAllocationDebuff, new Func<int, string>(CommonTipsBuilder_Feature.FormatSignedValue));
			runtime.Set("CharacterFeature.SectFameBonu", null);
			runtime.HideAtom("Default", "FavorabilityIncrementFactor07");
			hasAnyEffectItem |= CommonTipsBuilder_Feature.SetScalarAtom(runtime, "FavorabilityIncrementFactor08", "CharacterFeature.NotSectFameBonu", (int)configData.NotSectFameBonu, new Func<int, string>(CommonTipsBuilder_Feature.FormatSignedValue));
			hasAnyEffectItem |= CommonTipsBuilder_Feature.SetScalarAtom(runtime, "FavorabilityIncrementFactor09", "CharacterFeature.TaiwuFameBonu", (int)configData.TaiwuFameBonu, new Func<int, string>(CommonTipsBuilder_Feature.FormatSignedValue));
			hasAnyEffectItem |= CommonTipsBuilder_Feature.SetScalarAtom(runtime, "FavorabilityIncrementFactor10", "CharacterFeature.AttachPoisonBonus", (int)configData.AttachPoisonBonus, new Func<int, string>(CommonTipsBuilder_Feature.FormatSignedPercent));
			Debug.Log(string.Format("[PBTest][CommonTip] AttachPoisonBonus: {0}, show: {1}", configData.AttachPoisonBonus, configData.AttachPoisonBonus != 0));
			hasAnyEffectItem |= CommonTipsBuilder_Feature.SetScalarAtom(runtime, "FavorabilityIncrementFactor11", "CharacterFeature.DetoxPoisonBonus", (int)configData.DetoxPoisonBonus, new Func<int, string>(CommonTipsBuilder_Feature.FormatSignedPercent));
			hasAnyEffectItem |= CommonTipsBuilder_Feature.SetScalarAtom(runtime, "FavorabilityIncrementFactor12", "CharacterFeature.HealOuterBonus", (int)configData.HealOuterBonus, new Func<int, string>(CommonTipsBuilder_Feature.FormatSignedPercent));
			hasAnyEffectItem |= CommonTipsBuilder_Feature.SetScalarAtom(runtime, "FavorabilityIncrementFactor13", "CharacterFeature.HealInnerBonus", (int)configData.HealInnerBonus, new Func<int, string>(CommonTipsBuilder_Feature.FormatSignedPercent));
			hasAnyEffectItem |= CommonTipsBuilder_Feature.SetScalarAtom(runtime, "FavorabilityIncrementFactor14", "CharacterFeature.QiDisorderDebuffPercent", (int)configData.QiDisorderDebuffPercent, (int value) => CommonTipsBuilder_Feature.FormatSignedPercent(value, false));
			hasAnyEffectItem |= CommonTipsBuilder_Feature.SetScalarAtom(runtime, "FavorabilityIncrementFactor15", "CharacterFeature.QiDisorderBuffPercent", (int)configData.QiDisorderBuffPercent, (int value) => CommonTipsBuilder_Feature.FormatSignedPercent(value, true));
			hasAnyEffectItem |= CommonTipsBuilder_Feature.SetScalarAtom(runtime, "FavorabilityIncrementFactor16", "CharacterFeature.HealthRecovery", configData.HealthRecovery, new Func<int, string>(CommonTipsBuilder_Feature.FormatSignedValue));
			runtime.Set("__Feature.HasAnyEffectItem", hasAnyEffectItem ? bool.TrueString : null);
			runtime.SetAtomVisible("Default", "effects", hasAnyEffectItem);
			bool flag = !templateDataOnly && isInGame && characterId >= 0;
			if (flag)
			{
				CommonTipsBuilder_Feature.StartAsyncDisplayDataFill(runtime, featureId, characterId, configData);
			}
			return arg;
		}

		// Token: 0x06005ABB RID: 23227 RVA: 0x002A172C File Offset: 0x0029F92C
		private static void StartAsyncDisplayDataFill(CommonTipSimpleRuntime runtime, short featureId, int characterId, CharacterFeatureItem configData)
		{
			string asyncToken = Guid.NewGuid().ToString("N");
			runtime.Set("__Feature.AsyncToken", asyncToken);
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, characterId, delegate(int offset, RawDataPool dataPool)
			{
				bool flag = runtime.GetArgument("__Feature.AsyncToken") != asyncToken;
				if (!flag)
				{
					CharacterDisplayData displayData = null;
					Serializer.Deserialize(dataPool, offset, ref displayData);
					bool flag2 = displayData == null;
					if (!flag2)
					{
						bool isTaiwu = characterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
						int fameChange = GameData.Domains.Character.SharedMethods.GetSectFeatureFameBonus(featureId, isTaiwu, displayData.OrgInfo);
						bool flag3 = fameChange != 0;
						if (flag3)
						{
							runtime.Set("CharacterFeature.SectFameBonu", CommonTipsBuilder_Feature.FormatSignedValue(fameChange));
							runtime.ShowAtom("Default", "FavorabilityIncrementFactor07");
						}
						else
						{
							runtime.Set("CharacterFeature.SectFameBonu", null);
							runtime.HideAtom("Default", "FavorabilityIncrementFactor07");
						}
						CommonTipsBuilder_Feature.UpdateEffectsAtomVisibility(runtime, fameChange != 0);
						bool flag4 = featureId != 216;
						if (!flag4)
						{
							runtime.Set("CharacterFeature.EffectDesc", CommonTipsBuilder_Feature.BuildDarkAshEffectDesc(configData, displayData));
						}
					}
				}
			});
		}

		// Token: 0x06005ABC RID: 23228 RVA: 0x002A17A4 File Offset: 0x0029F9A4
		private static string BuildDarkAshEffectDesc(CharacterFeatureItem configData, CharacterDisplayData displayData)
		{
			bool flag = configData == null || displayData == null || string.IsNullOrEmpty(configData.EffectDesc);
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				bool flag2 = displayData.AliveState != 0;
				if (flag2)
				{
					result = configData.EffectDesc.GetFormat(new object[]
					{
						"-",
						"-",
						"-",
						"-\n"
					});
				}
				else
				{
					string initial = configData.EffectDesc.GetFormat(new object[]
					{
						displayData.DarkAshCounter.Total,
						displayData.DarkAshCounter.Tips1,
						displayData.DarkAshCounter.Tips2,
						displayData.DarkAshCounter.Tips3
					});
					bool flag3 = displayData.DarkAshProtector == 512U;
					if (flag3)
					{
						result = initial;
					}
					else
					{
						int starter = 0;
						StringBuilder builder = new StringBuilder(initial);
						builder.AppendLine(string.Empty);
						while (1 << starter < 512)
						{
							bool flag4 = (1U << starter & displayData.DarkAshProtector) > 0U;
							if (flag4)
							{
								builder.Append("\n" + LocalStringManager.Get(string.Format("LK_MouseTip_DarkAsh_Protector{0}", starter)));
							}
							starter++;
						}
						result = builder.ToString();
					}
				}
			}
			return result;
		}

		// Token: 0x06005ABD RID: 23229 RVA: 0x002A190C File Offset: 0x0029FB0C
		private static bool SetIndexedArray(CommonTipSimpleRuntime runtime, string atomNamePrefix, string keyPrefix, IReadOnlyList<sbyte> values, Func<int, string> formatter)
		{
			bool hasVisibleItem = false;
			for (int index = 0; index < values.Count; index++)
			{
				string atomName = CommonTipsBuilder_Feature.GetIndexedName(atomNamePrefix, index);
				string key = CommonTipsBuilder_Feature.GetIndexedName(keyPrefix, index);
				sbyte value = values[index];
				bool show = value != 0;
				runtime.Set(key, show ? formatter((int)value) : null);
				Debug.Log(string.Format("[PBTest][CommonTip] Set indexed array value for {0}: {1} (show: {2})", key, value, show));
				runtime.SetAtomVisible("Default", atomName, show);
				hasVisibleItem = (hasVisibleItem || show);
			}
			return hasVisibleItem;
		}

		// Token: 0x06005ABE RID: 23230 RVA: 0x002A19A8 File Offset: 0x0029FBA8
		private static bool SetIndexedArray(CommonTipSimpleRuntime runtime, string atomNamePrefix, string keyPrefix, IReadOnlyList<short> values, Func<int, string> formatter)
		{
			bool hasVisibleItem = false;
			for (int index = 0; index < values.Count; index++)
			{
				string atomName = CommonTipsBuilder_Feature.GetIndexedName(atomNamePrefix, index);
				string key = CommonTipsBuilder_Feature.GetIndexedName(keyPrefix, index);
				short value = values[index];
				bool show = value != 0;
				runtime.Set(key, show ? formatter((int)value) : null);
				Debug.Log(string.Format("[PBTest][CommonTip] Set indexed array value for {0}: {1} (show: {2})", key, value, show));
				runtime.SetAtomVisible("Default", atomName, show);
				hasVisibleItem = (hasVisibleItem || show);
			}
			return hasVisibleItem;
		}

		// Token: 0x06005ABF RID: 23231 RVA: 0x002A1A44 File Offset: 0x0029FC44
		private static bool SetScalarAtom(CommonTipSimpleRuntime runtime, string atomName, string key, int value, Func<int, string> formatter)
		{
			bool show = value != 0;
			runtime.Set(key, show ? formatter(value) : null);
			Debug.Log(string.Format("[PBTest][CommonTip] Set indexed array value for {0}: {1} (show: {2})", key, value, show));
			runtime.SetAtomVisible("Default", atomName, show);
			return show;
		}

		// Token: 0x06005AC0 RID: 23232 RVA: 0x002A1AA0 File Offset: 0x0029FCA0
		private static void UpdateEffectsAtomVisibility(CommonTipSimpleRuntime runtime, bool hasDynamicEffectItem)
		{
			bool hasStaticEffectItem = string.Equals(runtime.GetArgument("__Feature.HasAnyEffectItem"), bool.TrueString, StringComparison.Ordinal);
			runtime.SetAtomVisible("Default", "effects", hasStaticEffectItem || hasDynamicEffectItem);
		}

		// Token: 0x06005AC1 RID: 23233 RVA: 0x002A1ADC File Offset: 0x0029FCDC
		private static string GetIndexedName(string prefix, int index)
		{
			return string.Format("{0}{1:D2}", prefix, index + 1);
		}

		// Token: 0x06005AC2 RID: 23234 RVA: 0x002A1B04 File Offset: 0x0029FD04
		private static string FormatSignedPercent(int value)
		{
			return CommonTipsBuilder_Feature.FormatSignedPercent(value, true);
		}

		// Token: 0x06005AC3 RID: 23235 RVA: 0x002A1B20 File Offset: 0x0029FD20
		private static string FormatSignedPercent(int value, bool positiveBeneficial)
		{
			return CommonTipsBuilder_Feature.FormatSigned(value, positiveBeneficial, true);
		}

		// Token: 0x06005AC4 RID: 23236 RVA: 0x002A1B3C File Offset: 0x0029FD3C
		private static string FormatSignedValue(int value)
		{
			return CommonTipsBuilder_Feature.FormatSigned(value, true, false);
		}

		// Token: 0x06005AC5 RID: 23237 RVA: 0x002A1B58 File Offset: 0x0029FD58
		private static string FormatAdoreFactor(int rawValue)
		{
			bool positiveBeneficial = rawValue >= 100;
			int displayValue = (rawValue < 100) ? rawValue : (-rawValue);
			return CommonTipsBuilder_Feature.FormatSigned(displayValue, positiveBeneficial, false);
		}

		// Token: 0x06005AC6 RID: 23238 RVA: 0x002A1B88 File Offset: 0x0029FD88
		private static string FormatSigned(int value, bool positiveBeneficial, bool withPercent)
		{
			int absValue = Math.Abs(value);
			string suffix = withPercent ? "%" : string.Empty;
			string sign = (value > 0) ? "+" : ((value < 0) ? "-" : string.Empty);
			string color = (value > 0) ? (positiveBeneficial ? "brightblue" : "brightred") : (positiveBeneficial ? "brightred" : "brightblue");
			return string.Format("{0}{1}{2}", sign, absValue, suffix).SetColor(color);
		}

		// Token: 0x04003E85 RID: 16005
		private const string ParagraphDefault = "Default";

		// Token: 0x04003E86 RID: 16006
		private const string AsyncTokenKey = "__Feature.AsyncToken";

		// Token: 0x04003E87 RID: 16007
		private const string HasAnyEffectItemKey = "__Feature.HasAnyEffectItem";
	}
}
