using System;
using System.Collections.Generic;
using System.Text;
using Config;
using Config.ConfigCells.Character;
using FrameWork;
using Game.Views.MouseTips;

namespace Game.Views.TipsBuilders
{
	// Token: 0x0200074E RID: 1870
	[CommonTipBuilder(50, TipType.LegendaryBookBonus)]
	public class CommonTipsBuilder_LegendaryBookBonus : ICommonTipBuilder
	{
		// Token: 0x06005AC8 RID: 23240 RVA: 0x002A1C18 File Offset: 0x0029FE18
		public ArgumentBox BuildTip(CommonTipItem commonTipItem, ArgumentBox arg)
		{
			CommonTipSimpleRuntime runtime = CommonTipsHelper.GetOrCreateSimpleRuntimeTipForBuild(commonTipItem, arg);
			runtime.ShowAllParagraphs();
			runtime.ShowAllAtoms();
			sbyte skillType;
			arg.Get("SkillType", out skillType);
			short bonusType;
			arg.Get("BonusType", out bonusType);
			short slotType;
			arg.Get("SlotType", out slotType);
			bool showExpCost;
			arg.Get("ShowExpCost", out showExpCost);
			int needExp;
			arg.Get("NeedExp", out needExp);
			int currExp;
			arg.Get("CurrExp", out currExp);
			runtime.Set("LegendaryBookBonus.BonusTitle", null);
			runtime.Set("LegendaryBookBonus.UnlockEffectDesc", null);
			runtime.Set("LegendaryBookBonus.CurrentExp", null);
			runtime.Set("LegendaryBookBonus.NeedExp", null);
			for (int index = 0; index < 4; index++)
			{
				runtime.Set(CommonTipsBuilder_LegendaryBookBonus.GetPropertyNameKey(index), null);
				runtime.Set(CommonTipsBuilder_LegendaryBookBonus.GetPropertyValueKey(index), null);
				runtime.HideAtom("Effect", CommonTipsBuilder_LegendaryBookBonus.GetPropertyAtomName(index));
			}
			runtime.Set("LegendaryBookBonus.BonusTitle", CombatSkillType.Instance[skillType].Name);
			LegendaryBookPropertyBonusTypeItem bonusConfig = LegendaryBookPropertyBonusType.Instance[bonusType];
			bool hasAnyProperty = false;
			for (int index2 = 0; index2 < 4; index2++)
			{
				bool show = index2 < bonusConfig.PropertyBonusList.Length;
				runtime.SetAtomVisible("Effect", CommonTipsBuilder_LegendaryBookBonus.GetPropertyAtomName(index2), show);
				bool flag = !show;
				if (!flag)
				{
					PropertyAndValue addProperty = bonusConfig.PropertyBonusList[index2];
					runtime.Set(CommonTipsBuilder_LegendaryBookBonus.GetPropertyNameKey(index2), CommonTipsBuilder_LegendaryBookBonus.GetPropertyDisplayName(addProperty.PropertyId));
					runtime.Set(CommonTipsBuilder_LegendaryBookBonus.GetPropertyValueKey(index2), CommonTipsBuilder_LegendaryBookBonus.FormatPropertyValue(addProperty.PropertyId, (int)addProperty.Value));
					hasAnyProperty = true;
				}
			}
			runtime.SetParagraphVisible("Effect", hasAnyProperty);
			bool showUnlock = slotType >= 0;
			runtime.SetParagraphVisible("Unlock", showUnlock);
			bool flag2 = showUnlock;
			if (flag2)
			{
				runtime.Set("LegendaryBookBonus.UnlockEffectDesc", CommonTipsBuilder_LegendaryBookBonus.BuildUnlockEffectDesc(slotType));
			}
			runtime.SetParagraphVisible("Space", showUnlock && hasAnyProperty);
			runtime.SetParagraphVisible("CostExp", showExpCost);
			bool flag3 = showExpCost;
			if (flag3)
			{
				runtime.Set("LegendaryBookBonus.CurrentExp", currExp.ToString().SetColor((needExp <= currExp) ? "brightblue" : "brightred"));
				runtime.Set("LegendaryBookBonus.NeedExp", needExp.ToString());
			}
			return arg;
		}

		// Token: 0x06005AC9 RID: 23241 RVA: 0x002A1E84 File Offset: 0x002A0084
		private static string BuildUnlockEffectDesc(short slotType)
		{
			LegendaryBookSlotItem slotConfig = LegendaryBookSlot.Instance[slotType];
			StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
			strBuilder.Append("<indent=15px>");
			strBuilder.Append(slotConfig.Name);
			strBuilder.Append("</indent>");
			strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Colon_Symbol));
			strBuilder.Append(slotConfig.Desc.ColorReplace());
			string desc = strBuilder.ToString();
			EasyPool.Free<StringBuilder>(strBuilder);
			return desc;
		}

		// Token: 0x06005ACA RID: 23242 RVA: 0x002A1F00 File Offset: 0x002A0100
		private static string GetPropertyDisplayName(short propertyId)
		{
			bool flag = (int)propertyId < CharacterPropertyReferenced.Instance.Count;
			string name;
			if (flag)
			{
				short displayType = CharacterPropertyReferenced.Instance[propertyId].DisplayType;
				name = CharacterPropertyDisplay.Instance[displayType].Name;
			}
			else
			{
				name = CombatSkillProperty.Instance[(int)propertyId - CharacterPropertyReferenced.Instance.Count].Name;
			}
			return name;
		}

		// Token: 0x06005ACB RID: 23243 RVA: 0x002A1F64 File Offset: 0x002A0164
		private static string FormatPropertyValue(short propertyId, int value)
		{
			bool isPercent = CommonTipsBuilder_LegendaryBookBonus.ShowPercentPropertyTypes.Contains(propertyId);
			int absValue = Math.Abs(value);
			string sign = (value >= 0) ? "+" : "-";
			string suffix = isPercent ? "%" : string.Empty;
			string color = (value >= 0) ? "brightblue" : "brightred";
			return string.Format("{0}{1}{2}", sign, absValue, suffix).SetColor(color);
		}

		// Token: 0x06005ACC RID: 23244 RVA: 0x002A1FD8 File Offset: 0x002A01D8
		private static string GetPropertyAtomName(int index)
		{
			return string.Format("property{0:D2}", index + 1);
		}

		// Token: 0x06005ACD RID: 23245 RVA: 0x002A1FFC File Offset: 0x002A01FC
		private static string GetPropertyNameKey(int index)
		{
			return string.Format("LegendaryBookBonus.PropertyName{0:D2}", index + 1);
		}

		// Token: 0x06005ACE RID: 23246 RVA: 0x002A2020 File Offset: 0x002A0220
		private static string GetPropertyValueKey(int index)
		{
			return string.Format("LegendaryBookBonus.PropertyValue{0:D2}", index + 1);
		}

		// Token: 0x04003E88 RID: 16008
		private const string ParagraphUnlock = "Unlock";

		// Token: 0x04003E89 RID: 16009
		private const string ParagraphSpace = "Space";

		// Token: 0x04003E8A RID: 16010
		private const string ParagraphEffect = "Effect";

		// Token: 0x04003E8B RID: 16011
		private const string ParagraphCostExp = "CostExp";

		// Token: 0x04003E8C RID: 16012
		private const string KeyBonusTitle = "LegendaryBookBonus.BonusTitle";

		// Token: 0x04003E8D RID: 16013
		private const string KeyUnlockEffectDesc = "LegendaryBookBonus.UnlockEffectDesc";

		// Token: 0x04003E8E RID: 16014
		private const string KeyCurrentExp = "LegendaryBookBonus.CurrentExp";

		// Token: 0x04003E8F RID: 16015
		private const string KeyNeedExp = "LegendaryBookBonus.NeedExp";

		// Token: 0x04003E90 RID: 16016
		private const string EffectNameLayoutBegin = "<indent=15px>";

		// Token: 0x04003E91 RID: 16017
		private const string EffectNameLayoutEnd = "</indent>";

		// Token: 0x04003E92 RID: 16018
		private const int MaxPropertyAtomCount = 4;

		// Token: 0x04003E93 RID: 16019
		private static readonly List<short> ShowPercentPropertyTypes = new List<short>
		{
			6,
			7,
			8,
			9,
			10,
			11,
			12,
			13,
			14,
			15,
			16,
			17
		};
	}
}
