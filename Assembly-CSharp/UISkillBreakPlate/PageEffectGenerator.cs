using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Character;

namespace UISkillBreakPlate
{
	// Token: 0x0200041C RID: 1052
	internal class PageEffectGenerator
	{
		// Token: 0x06003E76 RID: 15990 RVA: 0x001F5514 File Offset: 0x001F3714
		public PageEffectGenerator(SkillBreakPageEffectImplementItem pageEffectItem, List<SkillBreakPageEffectDisplay> outEffectDisplayList, CombatSkillItem skillConfig)
		{
			this._pageEffectItem = pageEffectItem;
			this._outEffectDisplayList = outEffectDisplayList;
			this._specialTextIndex = 0;
			this._skillConfig = skillConfig;
		}

		// Token: 0x06003E77 RID: 15991 RVA: 0x001F553C File Offset: 0x001F373C
		public void Generate()
		{
			this._outEffectDisplayList.Clear();
			this.GenerateAddPower();
			this.GenerateAddMaxPower();
			this.GenerateAddRequirements();
			this.GenerateRecoveryOfBreath();
			this.GenerateRecoveryOfStance();
			this.GenerateRecoveryOfAcupoint();
			this.GenerateRecoveryOfFlaw();
			this.GenerateRecoveryOfQiDisorder();
			this.GenerateSwitchSpeed();
			this.GenerateInnerRatio();
			this.GenerateMoveSpeed();
			this.GenerateAttackSpeed();
			this.GenerateCastSpeed();
			this.GenerateHitValues();
			this.GeneratePenetrations();
			this.GenerateAvoidValues();
			this.GeneratePenetrationResists();
			this.GenerateCostBreathAndStance();
			this.GenerateCastFrame();
			this.GenerateAttackRangeForward();
			this.GenerateAttackRangeBackward();
			this.GenerateMakeDamage();
			this.GenerateHitFactor();
			this.GenerateCostMobilityByFrame();
			this.GenerateCostMobilityByMove();
			this.GenerateAcceptDirectDamageNoFatal();
			this.GenerateAcceptDirectDamageOnFatal();
			this.GenerateFlawRecoverSpeed();
			this.GenerateAcupointRecoverSpeed();
			this.GenerateSilenceRate();
			this.GenerateSilenceFrame();
		}

		// Token: 0x06003E78 RID: 15992 RVA: 0x001F5630 File Offset: 0x001F3830
		private void GenerateCombatProperty(int value, sbyte combatPropertyId, bool isPercent = false)
		{
			bool flag = value == 0;
			if (!flag)
			{
				SkillBreakPageEffectDisplay display = PageEffectGenerator.GenerateByCombatProperty(combatPropertyId, value, isPercent);
				this._outEffectDisplayList.Add(display);
			}
		}

		// Token: 0x06003E79 RID: 15993 RVA: 0x001F565E File Offset: 0x001F385E
		private void GenerateAddPower()
		{
			this.GenerateCombatProperty(this._pageEffectItem.AddPower, 0, false);
		}

		// Token: 0x06003E7A RID: 15994 RVA: 0x001F5674 File Offset: 0x001F3874
		private void GenerateAddMaxPower()
		{
			this.GenerateCombatProperty(this._pageEffectItem.AddMaxPower, 1, false);
		}

		// Token: 0x06003E7B RID: 15995 RVA: 0x001F568A File Offset: 0x001F388A
		private void GenerateAddRequirements()
		{
			this.GenerateCombatProperty(this._pageEffectItem.AddRequirement, 48, true);
		}

		// Token: 0x06003E7C RID: 15996 RVA: 0x001F56A1 File Offset: 0x001F38A1
		private void GenerateCastFrame()
		{
			this.GenerateCombatProperty(this._pageEffectItem.CastFrame, 2, true);
		}

		// Token: 0x06003E7D RID: 15997 RVA: 0x001F56B7 File Offset: 0x001F38B7
		private void GenerateCostMobilityByFrame()
		{
			this.GenerateCombatProperty(this._pageEffectItem.CostMobilityByFrame, 16, true);
		}

		// Token: 0x06003E7E RID: 15998 RVA: 0x001F56CE File Offset: 0x001F38CE
		private void GenerateCostMobilityByMove()
		{
			this.GenerateCombatProperty(this._pageEffectItem.CostMobilityByMove, 17, true);
		}

		// Token: 0x06003E7F RID: 15999 RVA: 0x001F56E8 File Offset: 0x001F38E8
		private void GenerateCharacterProperty(int value, ECharacterPropertyReferencedType refType, bool isPercent = false)
		{
			bool flag = value == 0;
			if (!flag)
			{
				SkillBreakPageEffectDisplay display = PageEffectGenerator.GenerateByCharacterProperty(refType, value, isPercent);
				this._outEffectDisplayList.Add(display);
			}
		}

		// Token: 0x06003E80 RID: 16000 RVA: 0x001F5716 File Offset: 0x001F3916
		private void GenerateRecoveryOfBreath()
		{
			this.GenerateCharacterProperty((int)(this._pageEffectItem.RecoveryOfBreath * (short)this._skillConfig.GridCost), ECharacterPropertyReferencedType.RecoveryOfBreath, false);
		}

		// Token: 0x06003E81 RID: 16001 RVA: 0x001F5739 File Offset: 0x001F3939
		private void GenerateRecoveryOfStance()
		{
			this.GenerateCharacterProperty((int)(this._pageEffectItem.RecoveryOfStance * (short)this._skillConfig.GridCost), ECharacterPropertyReferencedType.RecoveryOfStance, false);
		}

		// Token: 0x06003E82 RID: 16002 RVA: 0x001F575C File Offset: 0x001F395C
		private void GenerateRecoveryOfAcupoint()
		{
			this.GenerateCharacterProperty((int)(this._pageEffectItem.RecoveryOfAcupoint * (short)this._skillConfig.GridCost), ECharacterPropertyReferencedType.RecoveryOfBlockedAcupoint, false);
		}

		// Token: 0x06003E83 RID: 16003 RVA: 0x001F577F File Offset: 0x001F397F
		private void GenerateRecoveryOfFlaw()
		{
			this.GenerateCharacterProperty((int)(this._pageEffectItem.RecoveryOfFlaw * (short)this._skillConfig.GridCost), ECharacterPropertyReferencedType.RecoveryOfFlaw, false);
		}

		// Token: 0x06003E84 RID: 16004 RVA: 0x001F57A2 File Offset: 0x001F39A2
		private void GenerateRecoveryOfQiDisorder()
		{
			this.GenerateCharacterProperty((int)(this._pageEffectItem.RecoveryOfQiDisorder * (short)this._skillConfig.GridCost), ECharacterPropertyReferencedType.RecoveryOfQiDisorder, false);
		}

		// Token: 0x06003E85 RID: 16005 RVA: 0x001F57C5 File Offset: 0x001F39C5
		private void GenerateSwitchSpeed()
		{
			this.GenerateCharacterProperty((int)(this._pageEffectItem.SwitchSpeed * (short)this._skillConfig.GridCost), ECharacterPropertyReferencedType.WeaponSwitchSpeed, false);
		}

		// Token: 0x06003E86 RID: 16006 RVA: 0x001F57E8 File Offset: 0x001F39E8
		private void GenerateInnerRatio()
		{
			this.GenerateCharacterProperty((int)(this._pageEffectItem.InnerRatio * (short)this._skillConfig.GridCost), ECharacterPropertyReferencedType.InnerRatio, false);
		}

		// Token: 0x06003E87 RID: 16007 RVA: 0x001F580B File Offset: 0x001F3A0B
		private void GenerateMoveSpeed()
		{
			this.GenerateCharacterProperty((int)(this._pageEffectItem.MoveSpeed * (short)this._skillConfig.GridCost), ECharacterPropertyReferencedType.MoveSpeed, false);
		}

		// Token: 0x06003E88 RID: 16008 RVA: 0x001F582E File Offset: 0x001F3A2E
		private void GenerateAttackSpeed()
		{
			this.GenerateCharacterProperty((int)(this._pageEffectItem.AttackSpeed * (short)this._skillConfig.GridCost), ECharacterPropertyReferencedType.AttackSpeed, false);
		}

		// Token: 0x06003E89 RID: 16009 RVA: 0x001F5851 File Offset: 0x001F3A51
		private void GenerateCastSpeed()
		{
			this.GenerateCharacterProperty((int)(this._pageEffectItem.CastSpeed * (short)this._skillConfig.GridCost), ECharacterPropertyReferencedType.CastSpeed, false);
		}

		// Token: 0x06003E8A RID: 16010 RVA: 0x001F5874 File Offset: 0x001F3A74
		private void GenerateHitValues()
		{
			HitOrAvoidShorts hitValues = this._pageEffectItem.HitValues;
			this.GenerateCharacterProperty((int)(hitValues[0] * (short)this._skillConfig.GridCost), ECharacterPropertyReferencedType.HitRateStrength, false);
			this.GenerateCharacterProperty((int)(hitValues[1] * (short)this._skillConfig.GridCost), ECharacterPropertyReferencedType.HitRateTechnique, false);
			this.GenerateCharacterProperty((int)(hitValues[2] * (short)this._skillConfig.GridCost), ECharacterPropertyReferencedType.HitRateSpeed, false);
			this.GenerateCharacterProperty((int)(hitValues[3] * (short)this._skillConfig.GridCost), ECharacterPropertyReferencedType.HitRateMind, false);
		}

		// Token: 0x06003E8B RID: 16011 RVA: 0x001F5904 File Offset: 0x001F3B04
		private void GeneratePenetrations()
		{
			OuterAndInnerShorts penetrations = this._pageEffectItem.Penetrations;
			this.GenerateCharacterProperty((int)(penetrations.Outer * (short)this._skillConfig.GridCost), ECharacterPropertyReferencedType.PenetrateOfOuter, false);
			this.GenerateCharacterProperty((int)(penetrations.Inner * (short)this._skillConfig.GridCost), ECharacterPropertyReferencedType.PenetrateOfInner, false);
		}

		// Token: 0x06003E8C RID: 16012 RVA: 0x001F5958 File Offset: 0x001F3B58
		private void GenerateAvoidValues()
		{
			HitOrAvoidShorts avoidValues = this._pageEffectItem.AvoidValues;
			this.GenerateCharacterProperty((int)(avoidValues[0] * (short)this._skillConfig.GridCost), ECharacterPropertyReferencedType.AvoidRateStrength, false);
			this.GenerateCharacterProperty((int)(avoidValues[1] * (short)this._skillConfig.GridCost), ECharacterPropertyReferencedType.AvoidRateTechnique, false);
			this.GenerateCharacterProperty((int)(avoidValues[2] * (short)this._skillConfig.GridCost), ECharacterPropertyReferencedType.AvoidRateSpeed, false);
			this.GenerateCharacterProperty((int)(avoidValues[3] * (short)this._skillConfig.GridCost), ECharacterPropertyReferencedType.AvoidRateMind, false);
		}

		// Token: 0x06003E8D RID: 16013 RVA: 0x001F59EC File Offset: 0x001F3BEC
		private void GeneratePenetrationResists()
		{
			OuterAndInnerShorts penetrationResists = this._pageEffectItem.PenetrationResists;
			this.GenerateCharacterProperty((int)(penetrationResists.Outer * (short)this._skillConfig.GridCost), ECharacterPropertyReferencedType.PenetrateResistOfOuter, false);
			this.GenerateCharacterProperty((int)(penetrationResists.Inner * (short)this._skillConfig.GridCost), ECharacterPropertyReferencedType.PenetrateResistOfInner, false);
		}

		// Token: 0x06003E8E RID: 16014 RVA: 0x001F5A40 File Offset: 0x001F3C40
		private void GenerateHitFactor()
		{
			this.GenerateCharacterProperty(this._pageEffectItem.HitFactor, ECharacterPropertyReferencedType.HitRateStrength, true);
			this.GenerateCharacterProperty(this._pageEffectItem.HitFactor, ECharacterPropertyReferencedType.HitRateTechnique, true);
			this.GenerateCharacterProperty(this._pageEffectItem.HitFactor, ECharacterPropertyReferencedType.HitRateSpeed, true);
			this.GenerateCharacterProperty(this._pageEffectItem.HitFactor, ECharacterPropertyReferencedType.HitRateMind, true);
		}

		// Token: 0x06003E8F RID: 16015 RVA: 0x001F5AA0 File Offset: 0x001F3CA0
		private void GenerateSkillBreakDisplay(int value, sbyte id, PageEffectGenerator.ValueDisplayFunc formatFunc = null)
		{
			bool flag = value == 0;
			if (!flag)
			{
				SkillBreakPageEffectDisplay display = PageEffectGenerator.GenerateBySkillBreakDisplay(value, id, formatFunc);
				this._outEffectDisplayList.Add(display);
			}
		}

		// Token: 0x06003E90 RID: 16016 RVA: 0x001F5ACE File Offset: 0x001F3CCE
		private void GenerateCostBreathAndStance()
		{
			this.GenerateSkillBreakDisplay(this._pageEffectItem.CostBreathAndStance, 0, null);
			this.GenerateSkillBreakDisplay(this._pageEffectItem.CostBreathAndStance, 1, null);
		}

		// Token: 0x06003E91 RID: 16017 RVA: 0x001F5AF9 File Offset: 0x001F3CF9
		private void GenerateAttackRangeForward()
		{
			this.GenerateSkillBreakDisplay(this._pageEffectItem.AttackRangeForward, 38, new PageEffectGenerator.ValueDisplayFunc(PageEffectGenerator.FormatDivide10));
		}

		// Token: 0x06003E92 RID: 16018 RVA: 0x001F5B1B File Offset: 0x001F3D1B
		private void GenerateAttackRangeBackward()
		{
			this.GenerateSkillBreakDisplay(this._pageEffectItem.AttackRangeBackward, 39, new PageEffectGenerator.ValueDisplayFunc(PageEffectGenerator.FormatDivide10));
		}

		// Token: 0x06003E93 RID: 16019 RVA: 0x001F5B3D File Offset: 0x001F3D3D
		private void GenerateMakeDamage()
		{
			this.GenerateSkillBreakDisplay(this._pageEffectItem.MakeDamage, 40, null);
		}

		// Token: 0x06003E94 RID: 16020 RVA: 0x001F5B54 File Offset: 0x001F3D54
		private void GenerateFlawRecoverSpeed()
		{
			this.GenerateSkillBreakDisplay(this._pageEffectItem.FlawRecoverSpeed, 42, null);
		}

		// Token: 0x06003E95 RID: 16021 RVA: 0x001F5B6B File Offset: 0x001F3D6B
		private void GenerateAcupointRecoverSpeed()
		{
			this.GenerateSkillBreakDisplay(this._pageEffectItem.AcupointRecoverSpeed, 43, null);
		}

		// Token: 0x06003E96 RID: 16022 RVA: 0x001F5B82 File Offset: 0x001F3D82
		private void GenerateSilenceRate()
		{
			this.GenerateSkillBreakDisplay(this._pageEffectItem.SilenceRate, 44, null);
		}

		// Token: 0x06003E97 RID: 16023 RVA: 0x001F5B99 File Offset: 0x001F3D99
		private void GenerateSilenceFrame()
		{
			this.GenerateSkillBreakDisplay(this._pageEffectItem.SilenceFrame, 45, null);
		}

		// Token: 0x06003E98 RID: 16024 RVA: 0x001F5BB0 File Offset: 0x001F3DB0
		private static string FormatDivide10(int value)
		{
			return string.Format("{0}", (float)value / 10f);
		}

		// Token: 0x06003E99 RID: 16025 RVA: 0x001F5BDC File Offset: 0x001F3DDC
		private void GenerateAddSpecialText(int value)
		{
			bool flag = value <= 0;
			if (!flag)
			{
				string specialText = this._pageEffectItem.EffectDesc[this._specialTextIndex];
				string desc = string.Format(specialText, value).ColorReplace();
				this._outEffectDisplayList.Add(new SkillBreakPageEffectDisplay("", desc, null));
			}
		}

		// Token: 0x06003E9A RID: 16026 RVA: 0x001F5C34 File Offset: 0x001F3E34
		private void GenerateAcceptDirectDamageNoFatal()
		{
			this.GenerateAddSpecialText(Math.Abs(this._pageEffectItem.AcceptDirectDamageNoFatal));
		}

		// Token: 0x06003E9B RID: 16027 RVA: 0x001F5C4D File Offset: 0x001F3E4D
		private void GenerateAcceptDirectDamageOnFatal()
		{
			this.GenerateAddSpecialText(Math.Abs(this._pageEffectItem.AcceptDirectDamageOnFatal));
		}

		// Token: 0x06003E9C RID: 16028 RVA: 0x001F5C68 File Offset: 0x001F3E68
		private static SkillBreakPageEffectDisplay GenerateByCombatProperty(sbyte combatPropertyId, int value, bool isPercent)
		{
			CombatSkillPropertyItem displayConfig = CombatSkillProperty.Instance[combatPropertyId];
			string displayName = displayConfig.Name;
			string color = (value > 0) ? displayConfig.PlusColor : displayConfig.MinusColor;
			string sign = (value > 0) ? "+" : "";
			string percent = isPercent ? "%" : "";
			string str = string.Format("{0}{1}{2}", sign, value, percent).SetColor(color);
			return new SkillBreakPageEffectDisplay(displayName, str, displayConfig.TipsSmallIcon);
		}

		// Token: 0x06003E9D RID: 16029 RVA: 0x001F5CF0 File Offset: 0x001F3EF0
		private static SkillBreakPageEffectDisplay GenerateByCharacterProperty(ECharacterPropertyReferencedType referencedType, int value, bool isPercent)
		{
			CharacterPropertyDisplayItem displayConfig = PageEffectGenerator.GetCharacterPropertyDisplayItem((short)referencedType);
			string displayName = displayConfig.Name;
			string color = (value > 0) ? displayConfig.PositiveColor : displayConfig.NegativeColor;
			string sign = (value > 0) ? "+" : "";
			string percent = isPercent ? "%" : "";
			string str = string.Format("{0}{1}{2}", sign, value, percent).SetColor(color);
			return new SkillBreakPageEffectDisplay(displayName, str, displayConfig.TipsIcon);
		}

		// Token: 0x06003E9E RID: 16030 RVA: 0x001F5D74 File Offset: 0x001F3F74
		private static SkillBreakPageEffectDisplay GenerateBySkillBreakDisplay(int value, sbyte id, PageEffectGenerator.ValueDisplayFunc valueDisplayFunc = null)
		{
			SkillBreakEffectDisplayItem displayConfig = SkillBreakEffectDisplay.Instance[id];
			string displayName = displayConfig.Name;
			bool isPlusColor = value > 0;
			bool isInverse = displayConfig.IsInverse;
			if (isInverse)
			{
				isPlusColor = !isPlusColor;
			}
			string color = isPlusColor ? displayConfig.PlusColor : displayConfig.MinusColor;
			string sign = (value > 0) ? "+" : "";
			string percent = displayConfig.IsPercent ? "%" : "";
			string valueStr = (valueDisplayFunc == null) ? value.ToString() : valueDisplayFunc(value);
			string str = (sign + valueStr + percent).SetColor(color);
			return new SkillBreakPageEffectDisplay(displayName, str, displayConfig.Icon);
		}

		// Token: 0x06003E9F RID: 16031 RVA: 0x001F5E24 File Offset: 0x001F4024
		private static CharacterPropertyDisplayItem GetCharacterPropertyDisplayItem(short propertyRefType)
		{
			CharacterPropertyReferencedItem refConfig = CharacterPropertyReferenced.Instance[propertyRefType];
			return CharacterPropertyDisplay.Instance[refConfig.DisplayType];
		}

		// Token: 0x04002D04 RID: 11524
		private readonly SkillBreakPageEffectImplementItem _pageEffectItem;

		// Token: 0x04002D05 RID: 11525
		private readonly List<SkillBreakPageEffectDisplay> _outEffectDisplayList;

		// Token: 0x04002D06 RID: 11526
		private readonly int _specialTextIndex;

		// Token: 0x04002D07 RID: 11527
		private readonly CombatSkillItem _skillConfig;

		// Token: 0x020018BF RID: 6335
		// (Invoke) Token: 0x0600D7BD RID: 55229
		private delegate string ValueDisplayFunc(int bonusValue);
	}
}
