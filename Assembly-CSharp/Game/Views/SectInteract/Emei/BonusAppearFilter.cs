using System;
using Config;
using GameData.Domains.Character;
using GameData.Domains.CombatSkill;

namespace Game.Views.SectInteract.Emei
{
	// Token: 0x020009EC RID: 2540
	internal static class BonusAppearFilter
	{
		// Token: 0x06007CC4 RID: 31940 RVA: 0x0039FFB8 File Offset: 0x0039E1B8
		private static bool IsNeigong(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.Type == 0;
		}

		// Token: 0x06007CC5 RID: 31941 RVA: 0x0039FFE0 File Offset: 0x0039E1E0
		private static bool IsPosingSkill(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.Type == 1;
		}

		// Token: 0x06007CC6 RID: 31942 RVA: 0x003A0008 File Offset: 0x0039E208
		private static bool IsPassiveSkill(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return GameData.Domains.Character.CombatSkillHelper.IsPassiveSkill(config.EquipType);
		}

		// Token: 0x06007CC7 RID: 31943 RVA: 0x003A0034 File Offset: 0x0039E234
		private static bool IsDefendSkill(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.EquipType == 3;
		}

		// Token: 0x06007CC8 RID: 31944 RVA: 0x003A005C File Offset: 0x0039E25C
		private static bool IsAttackSkill(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.EquipType == 1;
		}

		// Token: 0x06007CC9 RID: 31945 RVA: 0x003A0084 File Offset: 0x0039E284
		private static bool IsMixCombatSkill(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.FiveElements == 5;
		}

		// Token: 0x06007CCA RID: 31946 RVA: 0x003A00AC File Offset: 0x0039E2AC
		private static bool HasHitStrength(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.AddHitOnCast[0] > 0;
		}

		// Token: 0x06007CCB RID: 31947 RVA: 0x003A00D8 File Offset: 0x0039E2D8
		private static bool HasHitTechnique(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.AddHitOnCast[1] > 0;
		}

		// Token: 0x06007CCC RID: 31948 RVA: 0x003A0104 File Offset: 0x0039E304
		private static bool HasHitSpeed(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.AddHitOnCast[2] > 0;
		}

		// Token: 0x06007CCD RID: 31949 RVA: 0x003A0130 File Offset: 0x0039E330
		private static bool HasHitMind(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.AddHitOnCast[3] > 0;
		}

		// Token: 0x06007CCE RID: 31950 RVA: 0x003A015C File Offset: 0x0039E35C
		private static bool HasOuterDef(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.AddOuterPenetrateResistOnCast > 0;
		}

		// Token: 0x06007CCF RID: 31951 RVA: 0x003A0184 File Offset: 0x0039E384
		private static bool HasInnerDef(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.AddInnerPenetrateResistOnCast > 0;
		}

		// Token: 0x06007CD0 RID: 31952 RVA: 0x003A01AC File Offset: 0x0039E3AC
		private static bool HasAvoidStrength(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.AddAvoidOnCast[0] > 0;
		}

		// Token: 0x06007CD1 RID: 31953 RVA: 0x003A01D8 File Offset: 0x0039E3D8
		private static bool HasAvoidTechnique(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.AddAvoidOnCast[1] > 0;
		}

		// Token: 0x06007CD2 RID: 31954 RVA: 0x003A0204 File Offset: 0x0039E404
		private static bool HasAvoidSpeed(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.AddAvoidOnCast[2] > 0;
		}

		// Token: 0x06007CD3 RID: 31955 RVA: 0x003A0230 File Offset: 0x0039E430
		private static bool HasAvoidMind(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.AddAvoidOnCast[3] > 0;
		}

		// Token: 0x06007CD4 RID: 31956 RVA: 0x003A025C File Offset: 0x0039E45C
		private static bool HasFightbackPower(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.FightBackDamage > 0;
		}

		// Token: 0x06007CD5 RID: 31957 RVA: 0x003A0284 File Offset: 0x0039E484
		private static bool HasBouncePowerOuter(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.BounceRateOfOuterInjury > 0;
		}

		// Token: 0x06007CD6 RID: 31958 RVA: 0x003A02AC File Offset: 0x0039E4AC
		private static bool HasBouncePowerInner(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.BounceRateOfInnerInjury > 0;
		}

		// Token: 0x06007CD7 RID: 31959 RVA: 0x003A02D4 File Offset: 0x0039E4D4
		private static bool HasBounceDistance(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.BounceDistance > 0;
		}

		// Token: 0x06007CD8 RID: 31960 RVA: 0x003A02FC File Offset: 0x0039E4FC
		private static bool IsSpeedCostPass(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.MobilityReduceSpeed > 0;
		}

		// Token: 0x06007CD9 RID: 31961 RVA: 0x003A0324 File Offset: 0x0039E524
		private static bool IsMoveCostPass(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.MoveCostMobility > 0;
		}

		// Token: 0x06007CDA RID: 31962 RVA: 0x003A034C File Offset: 0x0039E54C
		private static bool IsTechniqueHitDistributionPass(CombatSkillDisplayDataForList data)
		{
			return data.HitDistribution[1] > 10;
		}

		// Token: 0x06007CDB RID: 31963 RVA: 0x003A0370 File Offset: 0x0039E570
		private static bool IsSpeedHitDistributionPass(CombatSkillDisplayDataForList data)
		{
			return data.HitDistribution[2] > 10;
		}

		// Token: 0x06007CDC RID: 31964 RVA: 0x003A0394 File Offset: 0x0039E594
		private static bool IsStrengthHitDistributionPass(CombatSkillDisplayDataForList data)
		{
			return data.HitDistribution[0] > 10;
		}

		// Token: 0x06007CDD RID: 31965 RVA: 0x003A03B8 File Offset: 0x0039E5B8
		private static bool IsHitChestPass(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.InjuryPartAtkRateDistribution[0] > 0;
		}

		// Token: 0x06007CDE RID: 31966 RVA: 0x003A03E4 File Offset: 0x0039E5E4
		private static bool IsHitBellyPass(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.InjuryPartAtkRateDistribution[1] > 0;
		}

		// Token: 0x06007CDF RID: 31967 RVA: 0x003A0410 File Offset: 0x0039E610
		private static bool IsHitHeadPass(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.InjuryPartAtkRateDistribution[2] > 0;
		}

		// Token: 0x06007CE0 RID: 31968 RVA: 0x003A043C File Offset: 0x0039E63C
		private static bool IsHitBothHandsPass(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.InjuryPartAtkRateDistribution[3] > 0 && config.InjuryPartAtkRateDistribution[4] > 0;
		}

		// Token: 0x06007CE1 RID: 31969 RVA: 0x003A0474 File Offset: 0x0039E674
		private static bool IsHitBothLegsPass(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.InjuryPartAtkRateDistribution[5] > 0 && config.InjuryPartAtkRateDistribution[6] > 0;
		}

		// Token: 0x06007CE2 RID: 31970 RVA: 0x003A04AC File Offset: 0x0039E6AC
		private static bool IsAtkAcupointEffectPass(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.HasAtkAcupointEffect;
		}

		// Token: 0x06007CE3 RID: 31971 RVA: 0x003A04D0 File Offset: 0x0039E6D0
		private static bool IsAtkFlawEffectPass(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.HasAtkFlawEffect;
		}

		// Token: 0x06007CE4 RID: 31972 RVA: 0x003A04F4 File Offset: 0x0039E6F4
		private unsafe static bool IsPoisonPass(short templateId, short poisonType)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return *(ref config.Poisons.Values.FixedElementField + (IntPtr)poisonType * 2) > 0;
		}

		// Token: 0x06007CE5 RID: 31973 RVA: 0x003A052C File Offset: 0x0039E72C
		private static bool IsTrickCostPass(CombatSkillDisplayDataForList data, short breakTemplateId)
		{
			sbyte trickType = -1;
			foreach (SpecialBreakProperty specialBreakProperty in CommonUtils.GetBreakEntryPropertiesByTemplateId(breakTemplateId))
			{
				short propertyId = specialBreakProperty.propertyId;
				bool flag = propertyId >= 53 && propertyId <= 68;
				if (flag)
				{
					trickType = (sbyte)(specialBreakProperty.propertyId - 53);
					break;
				}
			}
			int need = 0;
			bool flag2 = trickType != -1;
			if (flag2)
			{
				bool flag3 = data.CostTricks != null;
				if (flag3)
				{
					foreach (NeedTrick costTrick in data.CostTricks)
					{
						bool flag4 = costTrick.TrickType != trickType;
						if (!flag4)
						{
							need += (int)costTrick.NeedCount;
						}
					}
				}
			}
			return need > 0;
		}

		// Token: 0x06007CE6 RID: 31974 RVA: 0x003A063C File Offset: 0x0039E83C
		private static bool IsTrickCostPassConfig(short templateId, short breakTemplateId)
		{
			sbyte trickType = -1;
			foreach (SpecialBreakProperty specialBreakProperty in CommonUtils.GetBreakEntryPropertiesByTemplateId(breakTemplateId))
			{
				short propertyId = specialBreakProperty.propertyId;
				bool flag = propertyId >= 53 && propertyId <= 68;
				if (flag)
				{
					trickType = (sbyte)(specialBreakProperty.propertyId - 53);
					break;
				}
			}
			int need = 0;
			bool flag2 = trickType != -1;
			if (flag2)
			{
				CombatSkillItem config = CombatSkill.Instance[templateId];
				bool flag3 = config.TrickCost != null;
				if (flag3)
				{
					foreach (NeedTrick costTrick in config.TrickCost)
					{
						bool flag4 = costTrick.TrickType != trickType;
						if (!flag4)
						{
							need += (int)costTrick.NeedCount;
						}
					}
				}
			}
			return need > 0;
		}

		// Token: 0x06007CE7 RID: 31975 RVA: 0x003A0758 File Offset: 0x0039E958
		private static bool IsJumpPrepareFramePass(short templateId)
		{
			CombatSkillItem config = CombatSkill.Instance[templateId];
			return config.JumpPrepareFrame > 0;
		}

		// Token: 0x06007CE8 RID: 31976 RVA: 0x003A0780 File Offset: 0x0039E980
		public static bool FilterByAppearType(SkillBreakPlateGridBonusTypeItem bonusConfig, CombatSkillDisplayDataForList combatSkillDisplayData)
		{
			ESkillBreakPlateGridBonusTypeAppearType appearType = bonusConfig.AppearType;
			short templateId = bonusConfig.TemplateId;
			bool result;
			switch (appearType)
			{
			case ESkillBreakPlateGridBonusTypeAppearType.Never:
				result = false;
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.NoLimit:
				result = true;
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.NeigongAndPassiveSkill:
				result = (BonusAppearFilter.IsNeigong(combatSkillDisplayData.TemplateId) || BonusAppearFilter.IsPassiveSkill(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.NeigongAndPassiveSkillExclude:
				result = (!BonusAppearFilter.IsNeigong(combatSkillDisplayData.TemplateId) && !BonusAppearFilter.IsPassiveSkill(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.NeigongOnly:
				result = BonusAppearFilter.IsNeigong(combatSkillDisplayData.TemplateId);
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.NeigongOnlyExcludeMix:
				result = (BonusAppearFilter.IsNeigong(combatSkillDisplayData.TemplateId) && !BonusAppearFilter.IsMixCombatSkill(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.PosingOnly:
				result = BonusAppearFilter.IsPosingSkill(combatSkillDisplayData.TemplateId);
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.PosingAndStrength:
				result = (BonusAppearFilter.IsPosingSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.HasHitStrength(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.PosingAndTechnique:
				result = (BonusAppearFilter.IsPosingSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.HasHitTechnique(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.PosingAndSpeed:
				result = (BonusAppearFilter.IsPosingSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.HasHitSpeed(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.PosingAndMind:
				result = (BonusAppearFilter.IsPosingSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.HasHitMind(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.PosingAndSpeedCost:
				result = (BonusAppearFilter.IsPosingSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.IsSpeedCostPass(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.PosingAndMoveCost:
				result = (BonusAppearFilter.IsPosingSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.IsMoveCostPass(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.DefendSkillAndOuterDef:
				result = (BonusAppearFilter.IsDefendSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.HasOuterDef(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.DefendSkillAndInnerDef:
				result = (BonusAppearFilter.IsDefendSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.HasInnerDef(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.DefendSkillAndAvoidStrength:
				result = (BonusAppearFilter.IsDefendSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.HasAvoidStrength(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.DefendSkillAndAvoidTechnique:
				result = (BonusAppearFilter.IsDefendSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.HasAvoidTechnique(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.DefendSkillAndAvoidSpeed:
				result = (BonusAppearFilter.IsDefendSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.HasAvoidSpeed(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.DefendSkillAndAvoidMind:
				result = (BonusAppearFilter.IsDefendSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.HasAvoidMind(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.DefendSkillAndFightbackPower:
				result = (BonusAppearFilter.IsDefendSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.HasFightbackPower(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.DefendSkillAndBouncePowerOuter:
				result = (BonusAppearFilter.IsDefendSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.HasBouncePowerOuter(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.DefendSkillAndBouncePowerInner:
				result = (BonusAppearFilter.IsDefendSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.HasBouncePowerInner(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.DefendSkillAndBounceDistance:
				result = (BonusAppearFilter.IsDefendSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.HasBounceDistance(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.DefendSkillOnly:
				result = BonusAppearFilter.IsDefendSkill(combatSkillDisplayData.TemplateId);
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.AttackSkillOnly:
				result = BonusAppearFilter.IsAttackSkill(combatSkillDisplayData.TemplateId);
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.AttackSkillAndTechniqueHitDistribution:
				result = (BonusAppearFilter.IsAttackSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.IsTechniqueHitDistributionPass(combatSkillDisplayData));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.AttackSkillAndSpeedHitDistribution:
				result = (BonusAppearFilter.IsAttackSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.IsSpeedHitDistributionPass(combatSkillDisplayData));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.AttackSkillAndStrengthHitDistribution:
				result = (BonusAppearFilter.IsAttackSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.IsStrengthHitDistributionPass(combatSkillDisplayData));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.AttackSkillAndHitChest:
				result = (BonusAppearFilter.IsAttackSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.IsHitChestPass(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.AttackSkillAndHitBelly:
				result = (BonusAppearFilter.IsAttackSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.IsHitBellyPass(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.AttackSkillAndHitHead:
				result = (BonusAppearFilter.IsAttackSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.IsHitHeadPass(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.AttackSkillAndHitBothHands:
				result = (BonusAppearFilter.IsAttackSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.IsHitBothHandsPass(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.AttackSkillAndHitBothLegs:
				result = (BonusAppearFilter.IsAttackSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.IsHitBothLegsPass(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.AttackSkillAndHasAtkAcupointEffect:
				result = (BonusAppearFilter.IsAttackSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.IsAtkAcupointEffectPass(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.AttackSkillAndHasAtkFlawEffect:
				result = (BonusAppearFilter.IsAttackSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.IsAtkFlawEffectPass(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.AttackSkillAndHotPoison:
				result = (BonusAppearFilter.IsAttackSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.IsPoisonPass(combatSkillDisplayData.TemplateId, 0));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.AttackSkillAndGloomyPoison:
				result = (BonusAppearFilter.IsAttackSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.IsPoisonPass(combatSkillDisplayData.TemplateId, 1));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.AttackSkillAndColdPoison:
				result = (BonusAppearFilter.IsAttackSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.IsPoisonPass(combatSkillDisplayData.TemplateId, 2));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.AttackSkillAndRedPoison:
				result = (BonusAppearFilter.IsAttackSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.IsPoisonPass(combatSkillDisplayData.TemplateId, 3));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.AttackSkillAndRottenPoison:
				result = (BonusAppearFilter.IsAttackSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.IsPoisonPass(combatSkillDisplayData.TemplateId, 4));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.AttackSkillAndIllusoryPoison:
				result = (BonusAppearFilter.IsAttackSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.IsPoisonPass(combatSkillDisplayData.TemplateId, 5));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.AttackSkillAndTrickCost:
				result = (BonusAppearFilter.IsAttackSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.IsTrickCostPass(combatSkillDisplayData, templateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.PosingAndJumpPrepareFrame:
				result = (BonusAppearFilter.IsPosingSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.IsJumpPrepareFramePass(combatSkillDisplayData.TemplateId));
				break;
			case ESkillBreakPlateGridBonusTypeAppearType.AttackSkillAndTrickCostExist:
				result = (BonusAppearFilter.IsAttackSkill(combatSkillDisplayData.TemplateId) && BonusAppearFilter.IsTrickCostPassConfig(combatSkillDisplayData.TemplateId, templateId));
				break;
			default:
				result = false;
				break;
			}
			return result;
		}
	}
}
