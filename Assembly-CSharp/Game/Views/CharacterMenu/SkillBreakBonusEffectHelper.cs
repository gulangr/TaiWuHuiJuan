using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using Config.ConfigCells.Character;
using GameData.Domains.Character;
using GameData.Domains.Taiwu;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B80 RID: 2944
	public static class SkillBreakBonusEffectHelper
	{
		// Token: 0x06009173 RID: 37235 RVA: 0x0043CC10 File Offset: 0x0043AE10
		public static void GenerateBonusEffectDisplays(short combatSkillId, SkillBreakPlateBonus bonus, LifeSkillShorts lifeSkillAttainments, List<SkillBreakBonusEffectDisplay> resultList)
		{
			resultList.Clear();
			CombatSkillItem skillConfig = SkillBreakBonusEffectHelper.GetConfig(combatSkillId);
			sbyte equipType = skillConfig.EquipType;
			SkillBreakBonusEffectHelper.CalcAddLifeSkillRequirement(combatSkillId, bonus, ref lifeSkillAttainments, resultList);
			SkillBreakBonusEffectHelper.CalcReduceCostBreath(combatSkillId, bonus, ref lifeSkillAttainments, resultList);
			SkillBreakBonusEffectHelper.CalcReduceCostStance(combatSkillId, bonus, ref lifeSkillAttainments, resultList);
			SkillBreakBonusEffectHelper.CalcReduceCastFrame(combatSkillId, bonus, ref lifeSkillAttainments, resultList);
			SkillBreakBonusEffectHelper.CalcAddPower(combatSkillId, bonus, resultList);
			SkillBreakBonusEffectHelper.CalcAddMaxPower(combatSkillId, bonus, ref lifeSkillAttainments, resultList);
			SkillBreakBonusEffectHelper.CalcAddInjuryStep(combatSkillId, bonus, resultList);
			SkillBreakBonusEffectHelper.CalcAddFatalStep(combatSkillId, bonus, resultList);
			SkillBreakBonusEffectHelper.CalcAddMindStep(combatSkillId, bonus, resultList);
			SkillBreakBonusEffectHelper.CalcEquipAddProperty(combatSkillId, bonus, resultList);
			SkillBreakBonusEffectHelper.CalcReduceRequirements(combatSkillId, bonus, resultList);
			SkillBreakBonusEffectHelper.CalcInnerRatioChangeRange(combatSkillId, bonus, resultList);
			bool flag = equipType == 0;
			if (flag)
			{
				SkillBreakBonusEffectHelper.CalcSpecificGridCount(bonus, resultList);
				SkillBreakBonusEffectHelper.CalcAddOtherSkillMaxPower(bonus, resultList);
				SkillBreakBonusEffectHelper.CalcTotalObtainableNeili(bonus, resultList);
			}
			bool flag2 = equipType == 1;
			if (flag2)
			{
				SkillBreakBonusEffectHelper.CalcAddAttackRange(bonus, resultList);
				SkillBreakBonusEffectHelper.CalcMakeDirectDamage(bonus, resultList);
				SkillBreakBonusEffectHelper.CalcTotalHit(bonus, resultList);
				SkillBreakBonusEffectHelper.CalcPoison(combatSkillId, bonus, resultList);
			}
			bool flag3 = equipType == 2;
			if (flag3)
			{
				SkillBreakBonusEffectHelper.CalcCostMobilityByFrame(bonus, resultList);
				SkillBreakBonusEffectHelper.CalcCostMobilityByMove(bonus, resultList);
				SkillBreakBonusEffectHelper.CalcCostMobilityByCast(bonus, resultList);
				SkillBreakBonusEffectHelper.CalcAddHitOnCast(bonus, resultList);
			}
			bool flag4 = equipType == 3;
			if (flag4)
			{
				SkillBreakBonusEffectHelper.CalcFightBackPower(bonus, resultList);
				SkillBreakBonusEffectHelper.CalcBouncePower(bonus, resultList);
				SkillBreakBonusEffectHelper.CalcAddPenetrateResist(bonus, resultList);
				SkillBreakBonusEffectHelper.CalcAddAvoidValueOnCast(bonus, resultList);
			}
		}

		// Token: 0x06009174 RID: 37236 RVA: 0x0043CD50 File Offset: 0x0043AF50
		private static void CalcAddLifeSkillRequirement(short combatSkillId, SkillBreakPlateBonus bonus, ref LifeSkillShorts lifeSkillAttainments, List<SkillBreakBonusEffectDisplay> resultList)
		{
			CombatSkillItem config = SkillBreakBonusEffectHelper.GetConfig(combatSkillId);
			List<PropertyAndValue> requirements = config.UsingRequirement;
			int result = bonus.CalcAddLifeSkillRequirement(config.EquipType, ref lifeSkillAttainments);
			bool flag = result == 0;
			if (!flag)
			{
				foreach (PropertyAndValue requirement in requirements)
				{
					ECharacterPropertyReferencedType propertyType = (ECharacterPropertyReferencedType)requirement.PropertyId;
					bool flag2 = !propertyType.IsLifeSkillTypeAttainment();
					if (!flag2)
					{
						int displayTableId = 2 + (propertyType - ECharacterPropertyReferencedType.AttainmentMusic);
						resultList.Add(SkillBreakBonusEffectDisplay.CreateFromDisplayTable((sbyte)displayTableId, result, null));
					}
				}
			}
		}

		// Token: 0x06009175 RID: 37237 RVA: 0x0043CDFC File Offset: 0x0043AFFC
		private static void CalcReduceCostBreath(short combatSkillId, SkillBreakPlateBonus bonus, ref LifeSkillShorts lifeSkillAttainments, List<SkillBreakBonusEffectDisplay> resultList)
		{
			CombatSkillItem config = SkillBreakBonusEffectHelper.GetConfig(combatSkillId);
			int result = bonus.CalcReduceCostBreath(config.EquipType, ref lifeSkillAttainments);
			bool flag = result == 0;
			if (!flag)
			{
				result = -result;
				resultList.Add(SkillBreakBonusEffectDisplay.CreateFromDisplayTable(0, result, null));
			}
		}

		// Token: 0x06009176 RID: 37238 RVA: 0x0043CE3C File Offset: 0x0043B03C
		private static void CalcReduceCostStance(short combatSkillId, SkillBreakPlateBonus bonus, ref LifeSkillShorts lifeSkillAttainments, List<SkillBreakBonusEffectDisplay> resultList)
		{
			CombatSkillItem config = SkillBreakBonusEffectHelper.GetConfig(combatSkillId);
			int result = bonus.CalcReduceCostStance(config.EquipType, ref lifeSkillAttainments);
			bool flag = result == 0;
			if (!flag)
			{
				result = -result;
				resultList.Add(SkillBreakBonusEffectDisplay.CreateFromDisplayTable(1, result, null));
			}
		}

		// Token: 0x06009177 RID: 37239 RVA: 0x0043CE7C File Offset: 0x0043B07C
		private static void CalcReduceCastFrame(short combatSkillId, SkillBreakPlateBonus bonus, ref LifeSkillShorts lifeSkillAttainments, List<SkillBreakBonusEffectDisplay> resultList)
		{
			CombatSkillItem config = SkillBreakBonusEffectHelper.GetConfig(combatSkillId);
			int result = bonus.CalcReduceCastFrame(config.EquipType, ref lifeSkillAttainments);
			bool flag = result == 0;
			if (!flag)
			{
				result = -result;
				resultList.Add(SkillBreakBonusEffectDisplay.CreateFromCombatPropertyTable(2, result, true));
			}
		}

		// Token: 0x06009178 RID: 37240 RVA: 0x0043CEBC File Offset: 0x0043B0BC
		private static void CalcAddPower(short combatSkillId, SkillBreakPlateBonus bonus, List<SkillBreakBonusEffectDisplay> resultList)
		{
			CombatSkillItem config = SkillBreakBonusEffectHelper.GetConfig(combatSkillId);
			int result = bonus.CalcAddPower(config.EquipType);
			bool flag = result == 0;
			if (!flag)
			{
				resultList.Add(SkillBreakBonusEffectDisplay.CreateFromCombatPropertyTable(0, result, true));
			}
		}

		// Token: 0x06009179 RID: 37241 RVA: 0x0043CEF8 File Offset: 0x0043B0F8
		private static void CalcAddMaxPower(short combatSkillId, SkillBreakPlateBonus bonus, ref LifeSkillShorts lifeSkillAttainments, List<SkillBreakBonusEffectDisplay> resultList)
		{
			CombatSkillItem config = SkillBreakBonusEffectHelper.GetConfig(combatSkillId);
			int result = bonus.CalcAddMaxPower(config.EquipType, ref lifeSkillAttainments);
			bool flag = result == 0;
			if (!flag)
			{
				resultList.Add(SkillBreakBonusEffectDisplay.CreateFromCombatPropertyTable(1, result, true));
			}
		}

		// Token: 0x0600917A RID: 37242 RVA: 0x0043CF38 File Offset: 0x0043B138
		private static void CalcAddInjuryStep(short combatSkillId, SkillBreakPlateBonus bonus, List<SkillBreakBonusEffectDisplay> resultList)
		{
			SkillBreakBonusEffectHelper.<>c__DisplayClass7_0 CS$<>8__locals1;
			CS$<>8__locals1.bonus = bonus;
			CS$<>8__locals1.resultList = resultList;
			CS$<>8__locals1.config = SkillBreakBonusEffectHelper.GetConfig(combatSkillId);
			SkillBreakBonusEffectHelper.<CalcAddInjuryStep>g__CalcAddInjuryStepByInner|7_0(true, ref CS$<>8__locals1);
			SkillBreakBonusEffectHelper.<CalcAddInjuryStep>g__CalcAddInjuryStepByInner|7_0(false, ref CS$<>8__locals1);
		}

		// Token: 0x0600917B RID: 37243 RVA: 0x0043CF78 File Offset: 0x0043B178
		private static void CalcAddFatalStep(short combatSkillId, SkillBreakPlateBonus bonus, List<SkillBreakBonusEffectDisplay> resultList)
		{
			CombatSkillItem config = SkillBreakBonusEffectHelper.GetConfig(combatSkillId);
			int result = bonus.CalcAddFatalStep(config.EquipType);
			bool flag = result == 0;
			if (!flag)
			{
				sbyte displayTableId = 32;
				resultList.Add(SkillBreakBonusEffectDisplay.CreateFromDisplayTable(displayTableId, result, null));
			}
		}

		// Token: 0x0600917C RID: 37244 RVA: 0x0043CFB8 File Offset: 0x0043B1B8
		private static void CalcAddMindStep(short combatSkillId, SkillBreakPlateBonus bonus, List<SkillBreakBonusEffectDisplay> resultList)
		{
			CombatSkillItem config = SkillBreakBonusEffectHelper.GetConfig(combatSkillId);
			int result = bonus.CalcAddMindStep(config.EquipType);
			bool flag = result == 0;
			if (!flag)
			{
				sbyte displayTableId = 33;
				resultList.Add(SkillBreakBonusEffectDisplay.CreateFromDisplayTable(displayTableId, result, null));
			}
		}

		// Token: 0x0600917D RID: 37245 RVA: 0x0043CFF8 File Offset: 0x0043B1F8
		private static void CalcEquipAddProperty(short combatSkillId, SkillBreakPlateBonus bonus, List<SkillBreakBonusEffectDisplay> resultList)
		{
			CombatSkillItem config = SkillBreakBonusEffectHelper.GetConfig(combatSkillId);
			for (ECharacterPropertyReferencedType propertyType = ECharacterPropertyReferencedType.Strength; propertyType < ECharacterPropertyReferencedType.Count; propertyType++)
			{
				int result = bonus.CalcEquipAddProperty(config.EquipType, propertyType);
				bool flag = result == 0;
				if (!flag)
				{
					resultList.Add(SkillBreakBonusEffectDisplay.CreateFromCharacterPropertyTable((short)propertyType, result, false));
				}
			}
		}

		// Token: 0x0600917E RID: 37246 RVA: 0x0043D050 File Offset: 0x0043B250
		private static void CalcReduceRequirements(short combatSkillId, SkillBreakPlateBonus bonus, List<SkillBreakBonusEffectDisplay> resultList)
		{
			CombatSkillItem config = SkillBreakBonusEffectHelper.GetConfig(combatSkillId);
			int result = bonus.CalcReduceRequirements(config.EquipType);
			bool flag = result == 0;
			if (!flag)
			{
				resultList.Add(SkillBreakBonusEffectDisplay.CreateFromCombatPropertyTable(48, result, true));
			}
		}

		// Token: 0x0600917F RID: 37247 RVA: 0x0043D090 File Offset: 0x0043B290
		private static void CalcInnerRatioChangeRange(short combatSkillId, SkillBreakPlateBonus bonus, List<SkillBreakBonusEffectDisplay> resultList)
		{
			CombatSkillItem config = SkillBreakBonusEffectHelper.GetConfig(combatSkillId);
			int result = bonus.CalcInnerRatioChangeRange(config.EquipType);
			bool flag = result == 0;
			if (!flag)
			{
				resultList.Add(SkillBreakBonusEffectDisplay.CreateFromCombatPropertyTable(5, result, false));
			}
		}

		// Token: 0x06009180 RID: 37248 RVA: 0x0043D0CC File Offset: 0x0043B2CC
		private static void CalcSpecificGridCount(SkillBreakPlateBonus bonus, List<SkillBreakBonusEffectDisplay> resultList)
		{
			for (sbyte equipType = 1; equipType < 5; equipType += 1)
			{
				sbyte result = bonus.CalcSpecificGridCount(equipType);
				bool flag = result == 0;
				if (!flag)
				{
					int combatPropertyId = (int)(49 + equipType - 1);
					resultList.Add(SkillBreakBonusEffectDisplay.CreateFromCombatPropertyTable((sbyte)combatPropertyId, (int)result, false));
				}
			}
		}

		// Token: 0x06009181 RID: 37249 RVA: 0x0043D11C File Offset: 0x0043B31C
		private static void CalcAddOtherSkillMaxPower(SkillBreakPlateBonus bonus, List<SkillBreakBonusEffectDisplay> resultList)
		{
			for (sbyte equipType = 1; equipType < 5; equipType += 1)
			{
				short result = bonus.CalcAddOtherSkillMaxPower(equipType);
				bool flag = result == 0;
				if (!flag)
				{
					int displayTableId = (int)(34 + equipType - 1);
					resultList.Add(SkillBreakBonusEffectDisplay.CreateFromDisplayTable((sbyte)displayTableId, (int)result, null));
				}
			}
		}

		// Token: 0x06009182 RID: 37250 RVA: 0x0043D16C File Offset: 0x0043B36C
		private static void CalcTotalObtainableNeili(SkillBreakPlateBonus bonus, List<SkillBreakBonusEffectDisplay> resultList)
		{
			int result = bonus.CalcTotalObtainableNeili();
			bool flag = result == 0;
			if (!flag)
			{
				sbyte combatPropertyId = 6;
				resultList.Add(SkillBreakBonusEffectDisplay.CreateFromCombatPropertyTable(combatPropertyId, result, false));
			}
		}

		// Token: 0x06009183 RID: 37251 RVA: 0x0043D1A0 File Offset: 0x0043B3A0
		private static void CalcAddAttackRange(SkillBreakPlateBonus bonus, List<SkillBreakBonusEffectDisplay> resultList)
		{
			SkillBreakBonusEffectHelper.<>c__DisplayClass16_0 CS$<>8__locals1;
			CS$<>8__locals1.bonus = bonus;
			CS$<>8__locals1.resultList = resultList;
			SkillBreakBonusEffectHelper.<CalcAddAttackRange>g__CalcForDirection|16_0(true, ref CS$<>8__locals1);
			SkillBreakBonusEffectHelper.<CalcAddAttackRange>g__CalcForDirection|16_0(false, ref CS$<>8__locals1);
		}

		// Token: 0x06009184 RID: 37252 RVA: 0x0043D1D4 File Offset: 0x0043B3D4
		private static string BonusFormatDivide10(int bonusValue)
		{
			return string.Format("{0}", (float)bonusValue / 10f);
		}

		// Token: 0x06009185 RID: 37253 RVA: 0x0043D200 File Offset: 0x0043B400
		private static void CalcMakeDirectDamage(SkillBreakPlateBonus bonus, List<SkillBreakBonusEffectDisplay> resultList)
		{
			int result = bonus.CalcMakeDamage();
			bool flag = result == 0;
			if (!flag)
			{
				sbyte displayTableId = 40;
				resultList.Add(SkillBreakBonusEffectDisplay.CreateFromDisplayTable(displayTableId, result, null));
			}
		}

		// Token: 0x06009186 RID: 37254 RVA: 0x0043D234 File Offset: 0x0043B434
		private static void CalcTotalHit(SkillBreakPlateBonus bonus, List<SkillBreakBonusEffectDisplay> resultList)
		{
			int result = bonus.CalcTotalHit();
			bool flag = result == 0;
			if (!flag)
			{
				sbyte combatPropertyId = 30;
				resultList.Add(SkillBreakBonusEffectDisplay.CreateFromCombatPropertyTable(combatPropertyId, result, true));
			}
		}

		// Token: 0x06009187 RID: 37255 RVA: 0x0043D268 File Offset: 0x0043B468
		private unsafe static void CalcPoison(short combatSkillId, SkillBreakPlateBonus bonus, List<SkillBreakBonusEffectDisplay> resultList)
		{
			CombatSkillItem skillConfig = SkillBreakBonusEffectHelper.GetConfig(combatSkillId);
			for (sbyte poisonType = 0; poisonType < 6; poisonType += 1)
			{
				int result = bonus.CalcPoison(poisonType);
				bool flag = result == 0;
				if (!flag)
				{
					bool flag2 = *(ref skillConfig.Poisons.Values.FixedElementField + (IntPtr)poisonType * 2) == 0;
					if (!flag2)
					{
						int combatPropertyId = (int)(42 + poisonType);
						resultList.Add(SkillBreakBonusEffectDisplay.CreateFromCombatPropertyTable((sbyte)combatPropertyId, result, true));
					}
				}
			}
		}

		// Token: 0x06009188 RID: 37256 RVA: 0x0043D2E0 File Offset: 0x0043B4E0
		private static void CalcCostMobilityByFrame(SkillBreakPlateBonus bonus, List<SkillBreakBonusEffectDisplay> resultList)
		{
			int result = bonus.CalcCostMobilityByFrame();
			bool flag = result == 0;
			if (!flag)
			{
				sbyte combatPropertyId = 16;
				resultList.Add(SkillBreakBonusEffectDisplay.CreateFromCombatPropertyTable(combatPropertyId, result, true));
			}
		}

		// Token: 0x06009189 RID: 37257 RVA: 0x0043D314 File Offset: 0x0043B514
		private static void CalcCostMobilityByMove(SkillBreakPlateBonus bonus, List<SkillBreakBonusEffectDisplay> resultList)
		{
			int result = bonus.CalcCostMobilityByMove();
			bool flag = result == 0;
			if (!flag)
			{
				sbyte combatPropertyId = 17;
				resultList.Add(SkillBreakBonusEffectDisplay.CreateFromCombatPropertyTable(combatPropertyId, result, true));
			}
		}

		// Token: 0x0600918A RID: 37258 RVA: 0x0043D348 File Offset: 0x0043B548
		private static void CalcCostMobilityByCast(SkillBreakPlateBonus bonus, List<SkillBreakBonusEffectDisplay> resultList)
		{
			int result = bonus.CalcCostMobilityByCast();
			bool flag = result == 0;
			if (!flag)
			{
				sbyte combatPropertyId = 10;
				resultList.Add(SkillBreakBonusEffectDisplay.CreateFromCombatPropertyTable(combatPropertyId, result, true));
			}
		}

		// Token: 0x0600918B RID: 37259 RVA: 0x0043D37C File Offset: 0x0043B57C
		private static void CalcAddHitOnCast(SkillBreakPlateBonus bonus, List<SkillBreakBonusEffectDisplay> resultList)
		{
			int result = bonus.CalcAddHitOnCast();
			bool flag = result == 0;
			if (!flag)
			{
				for (int i = 0; i < 4; i++)
				{
					int combatPropertyId = 12 + i;
					resultList.Add(SkillBreakBonusEffectDisplay.CreateFromCombatPropertyTable((sbyte)combatPropertyId, result, true));
				}
			}
		}

		// Token: 0x0600918C RID: 37260 RVA: 0x0043D3C8 File Offset: 0x0043B5C8
		private static void CalcFightBackPower(SkillBreakPlateBonus bonus, List<SkillBreakBonusEffectDisplay> resultList)
		{
			int result = bonus.CalcFightBackPower();
			bool flag = result == 0;
			if (!flag)
			{
				sbyte combatPropertyId = 24;
				resultList.Add(SkillBreakBonusEffectDisplay.CreateFromCombatPropertyTable(combatPropertyId, result, true));
			}
		}

		// Token: 0x0600918D RID: 37261 RVA: 0x0043D3FC File Offset: 0x0043B5FC
		private static void CalcBouncePower(SkillBreakPlateBonus bonus, List<SkillBreakBonusEffectDisplay> resultList)
		{
			int result = bonus.CalcBouncePower();
			bool flag = result == 0;
			if (!flag)
			{
				sbyte combatPropertyId = 25;
				resultList.Add(SkillBreakBonusEffectDisplay.CreateFromCombatPropertyTable(combatPropertyId, result, true));
				combatPropertyId = 26;
				resultList.Add(SkillBreakBonusEffectDisplay.CreateFromCombatPropertyTable(combatPropertyId, result, true));
			}
		}

		// Token: 0x0600918E RID: 37262 RVA: 0x0043D440 File Offset: 0x0043B640
		private static void CalcAddPenetrateResist(SkillBreakPlateBonus bonus, List<SkillBreakBonusEffectDisplay> resultList)
		{
			int result = bonus.CalcAddPenetrateResist();
			bool flag = result == 0;
			if (!flag)
			{
				sbyte combatPropertyId = 18;
				resultList.Add(SkillBreakBonusEffectDisplay.CreateFromCombatPropertyTable(combatPropertyId, result, true));
				combatPropertyId = 19;
				resultList.Add(SkillBreakBonusEffectDisplay.CreateFromCombatPropertyTable(combatPropertyId, result, true));
			}
		}

		// Token: 0x0600918F RID: 37263 RVA: 0x0043D484 File Offset: 0x0043B684
		private static void CalcAddAvoidValueOnCast(SkillBreakPlateBonus bonus, List<SkillBreakBonusEffectDisplay> resultList)
		{
			int result = bonus.CalcAddAvoidValueOnCast();
			bool flag = result == 0;
			if (!flag)
			{
				for (int i = 0; i < 4; i++)
				{
					int combatPropertyId = 20 + i;
					resultList.Add(SkillBreakBonusEffectDisplay.CreateFromCombatPropertyTable((sbyte)combatPropertyId, result, true));
				}
			}
		}

		// Token: 0x06009190 RID: 37264 RVA: 0x0043D4D0 File Offset: 0x0043B6D0
		private static sbyte GetInjuryStepDisplayTableId(bool isInner, sbyte bodyType)
		{
			sbyte result;
			if (isInner)
			{
				if (!true)
				{
				}
				sbyte b;
				switch (bodyType)
				{
				case 0:
					b = 26;
					break;
				case 1:
					b = 27;
					break;
				case 2:
					b = 25;
					break;
				case 3:
					b = 28;
					break;
				case 4:
					b = 29;
					break;
				case 5:
					b = 30;
					break;
				case 6:
					b = 31;
					break;
				default:
					throw new ArgumentOutOfRangeException("bodyType", bodyType, null);
				}
				if (!true)
				{
				}
				result = b;
			}
			else
			{
				if (!true)
				{
				}
				sbyte b;
				switch (bodyType)
				{
				case 0:
					b = 19;
					break;
				case 1:
					b = 20;
					break;
				case 2:
					b = 18;
					break;
				case 3:
					b = 21;
					break;
				case 4:
					b = 22;
					break;
				case 5:
					b = 23;
					break;
				case 6:
					b = 24;
					break;
				default:
					throw new ArgumentOutOfRangeException("bodyType", bodyType, null);
				}
				if (!true)
				{
				}
				result = b;
			}
			return result;
		}

		// Token: 0x06009191 RID: 37265 RVA: 0x0043D5B0 File Offset: 0x0043B7B0
		private static CombatSkillItem GetConfig(short combatSkillId)
		{
			return CombatSkill.Instance[combatSkillId];
		}

		// Token: 0x06009192 RID: 37266 RVA: 0x0043D5D0 File Offset: 0x0043B7D0
		public static void GenerateExtraBonusEffectDisplays(short bonusType, SkillBreakBonusCollection collection, List<SkillBreakBonusEffectDisplay> resultList)
		{
			resultList.Clear();
			bool flag = collection == null;
			if (!flag)
			{
				int totalCharPropertyCount = CharacterPropertyReferenced.Instance.Count;
				bool isCombatSkillProperty = (int)bonusType >= totalCharPropertyCount;
				bool flag2 = !isCombatSkillProperty;
				if (flag2)
				{
					short bonusVal;
					bool flag3 = !collection.CharacterPropertyBonusDict.TryGetValue(bonusType, out bonusVal);
					if (!flag3)
					{
						short displayId = CharacterPropertyReferenced.Instance[bonusType].DisplayType;
						CharacterPropertyDisplayItem propertyData = CharacterPropertyDisplay.Instance[displayId];
						resultList.Add(SkillBreakBonusEffectDisplay.CreateFromCharacterPropertyTable(bonusType, (int)bonusVal, propertyData.IsPercent));
					}
				}
				else
				{
					short templateId = (short)((int)bonusType - totalCharPropertyCount);
					short bonusVal2;
					bool flag4 = !collection.CombatSkillPropertyBonusDict.TryGetValue(templateId, out bonusVal2);
					if (!flag4)
					{
						sbyte combatPropertyId = (sbyte)templateId;
						CombatSkillPropertyItem propertyData2 = CombatSkillProperty.Instance.GetItem(combatPropertyId);
						resultList.Add(SkillBreakBonusEffectDisplay.CreateFromCombatPropertyTable(combatPropertyId, (int)bonusVal2, propertyData2.IsPercent));
					}
				}
			}
		}

		// Token: 0x06009193 RID: 37267 RVA: 0x0043D6B0 File Offset: 0x0043B8B0
		[CompilerGenerated]
		internal static void <CalcAddInjuryStep>g__CalcAddInjuryStepByInner|7_0(bool isInner, ref SkillBreakBonusEffectHelper.<>c__DisplayClass7_0 A_1)
		{
			int result = A_1.bonus.CalcAddInjuryStep(A_1.config.EquipType, isInner);
			bool flag = result == 0;
			if (!flag)
			{
				int[] configDamageSteps = isInner ? A_1.config.InnerDamageSteps : A_1.config.OuterDamageSteps;
				for (sbyte part = 0; part < 7; part += 1)
				{
					bool flag2 = configDamageSteps[(int)part] == 0;
					if (!flag2)
					{
						sbyte displayTableId = SkillBreakBonusEffectHelper.GetInjuryStepDisplayTableId(isInner, part);
						A_1.resultList.Add(SkillBreakBonusEffectDisplay.CreateFromDisplayTable(displayTableId, result, null));
					}
				}
			}
		}

		// Token: 0x06009194 RID: 37268 RVA: 0x0043D740 File Offset: 0x0043B940
		[CompilerGenerated]
		internal static void <CalcAddAttackRange>g__CalcForDirection|16_0(bool isForward, ref SkillBreakBonusEffectHelper.<>c__DisplayClass16_0 A_1)
		{
			int result = A_1.bonus.CalcAddAttackRange(isForward);
			bool flag = result == 0;
			if (!flag)
			{
				sbyte displayTableId = isForward ? 38 : 39;
				A_1.resultList.Add(SkillBreakBonusEffectDisplay.CreateFromDisplayTable(displayTableId, result, new SkillBreakBonusEffectDisplay.BonusValueDisplayFunc(SkillBreakBonusEffectHelper.BonusFormatDivide10)));
			}
		}
	}
}
