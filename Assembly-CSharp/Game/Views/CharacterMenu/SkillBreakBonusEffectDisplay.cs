using System;
using Config;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B7F RID: 2943
	public struct SkillBreakBonusEffectDisplay
	{
		// Token: 0x0600916F RID: 37231 RVA: 0x0043CA13 File Offset: 0x0043AC13
		internal SkillBreakBonusEffectDisplay(string name, string smallIcon, string bigIcon, string value)
		{
			this.Name = name;
			this.SmallIcon = smallIcon;
			this.BigIcon = bigIcon;
			this.Value = value;
		}

		// Token: 0x06009170 RID: 37232 RVA: 0x0043CA34 File Offset: 0x0043AC34
		public static SkillBreakBonusEffectDisplay CreateFromDisplayTable(sbyte displayTableId, int bonusValue, SkillBreakBonusEffectDisplay.BonusValueDisplayFunc bonusValueDisplayFunc = null)
		{
			SkillBreakEffectDisplayItem displayConfig = SkillBreakEffectDisplay.Instance[displayTableId];
			string name = displayConfig.Name;
			string icon = displayConfig.Icon;
			string bigIcon = displayConfig.BigIcon;
			string valueStr = (bonusValueDisplayFunc == null) ? bonusValue.ToString() : bonusValueDisplayFunc(bonusValue);
			bool isPlusColor = bonusValue > 0;
			bool isInverse = displayConfig.IsInverse;
			if (isInverse)
			{
				isPlusColor = !isPlusColor;
			}
			string color = isPlusColor ? displayConfig.PlusColor : displayConfig.MinusColor;
			string value = (((bonusValue > 0) ? "+" : "") + valueStr + (displayConfig.IsPercent ? "%" : "")).SetColor(color);
			return new SkillBreakBonusEffectDisplay(name, icon, bigIcon, value);
		}

		// Token: 0x06009171 RID: 37233 RVA: 0x0043CAEC File Offset: 0x0043ACEC
		public static SkillBreakBonusEffectDisplay CreateFromCombatPropertyTable(sbyte combatPropertyId, int bonusValue, bool isPercent)
		{
			CombatSkillPropertyItem config = CombatSkillProperty.Instance[combatPropertyId];
			string name = config.Name;
			string smallIcon = config.TipsSmallIcon;
			string bigIcon = config.TipsIcon;
			string value = string.Format("{0}{1}{2}", (bonusValue > 0) ? "+" : "", bonusValue, isPercent ? "%" : "").SetColor((bonusValue > 0) ? config.PlusColor : config.MinusColor);
			return new SkillBreakBonusEffectDisplay(name, smallIcon, bigIcon, value);
		}

		// Token: 0x06009172 RID: 37234 RVA: 0x0043CB74 File Offset: 0x0043AD74
		public static SkillBreakBonusEffectDisplay CreateFromCharacterPropertyTable(short characterPropertyRefTableId, int bonusValue, bool isPercent)
		{
			CharacterPropertyReferencedItem refConfig = CharacterPropertyReferenced.Instance[characterPropertyRefTableId];
			CharacterPropertyDisplayItem displayConfig = CharacterPropertyDisplay.Instance[refConfig.DisplayType];
			string name = displayConfig.Name;
			string smallIcon = displayConfig.TipsIcon;
			string bigIcon = displayConfig.TipsBigIcon;
			string value = string.Format("{0}{1}{2}", (bonusValue > 0) ? "+" : "", bonusValue, isPercent ? "%" : "").SetColor((bonusValue > 0) ? displayConfig.PositiveColor : displayConfig.NegativeColor);
			return new SkillBreakBonusEffectDisplay(name, smallIcon, bigIcon, value);
		}

		// Token: 0x0400701C RID: 28700
		public readonly string Name;

		// Token: 0x0400701D RID: 28701
		public readonly string SmallIcon;

		// Token: 0x0400701E RID: 28702
		public readonly string BigIcon;

		// Token: 0x0400701F RID: 28703
		public readonly string Value;

		// Token: 0x0200218F RID: 8591
		// (Invoke) Token: 0x0600FB7C RID: 64380
		public delegate string BonusValueDisplayFunc(int bonusValue);
	}
}
