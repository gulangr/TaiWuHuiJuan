using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using GameData.Domains.Taiwu;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B94 RID: 2964
	public static class SkillBreakStartSwapButtonTipsHelper
	{
		// Token: 0x0600924F RID: 37455 RVA: 0x00442BA0 File Offset: 0x00440DA0
		public static void RefreshStartSwapButtonTips(TooltipInvoker tip, GameData.Domains.Taiwu.SkillBreakPlate displayPlate, int currentExp, short skillId)
		{
			tip.Type = TipType.GeneralLines;
			if (tip.RuntimeParam == null)
			{
				tip.RuntimeParam = new ArgumentBox();
			}
			SkillBreakStartSwapButtonTipsHelper.<>c__DisplayClass0_0 CS$<>8__locals1;
			CS$<>8__locals1.tipParam = tip.RuntimeParam;
			CS$<>8__locals1.tipParam.Set("Title", LanguageKey.LK_CombatSkill_SwapSkill.Tr());
			bool flag = displayPlate == null;
			if (flag)
			{
				CS$<>8__locals1.tipParam.Set("LineCount", 0);
				tip.Refresh(false, -1);
			}
			else
			{
				int requiredBaseExp = SkillBreakStartSwapButtonTipsHelper.CalculateRequiredBaseExp(skillId);
				int requiredExtraExp = SkillBreakStartSwapButtonTipsHelper.CalculateRequiredExtraExp(displayPlate, skillId);
				int totalRequiredExp = requiredBaseExp + requiredExtraExp;
				string currentExpText = CommonUtils.GetDisplayStringForNum(currentExp, 100000);
				currentExpText = ((currentExp >= totalRequiredExp) ? currentExpText.SetColor("brightblue") : currentExpText.SetColor("brightred"));
				string baseExpText = CommonUtils.GetDisplayStringForNum(requiredBaseExp, 100000).SetColor("pinkyellow");
				string extraExpText = CommonUtils.GetDisplayStringForNum(requiredExtraExp, 100000);
				string usedCountText = CommonUtils.GetDisplayStringForNum(displayPlate.SwapCount, 100000);
				CS$<>8__locals1.lineCount = 0;
				SkillBreakStartSwapButtonTipsHelper.<RefreshStartSwapButtonTips>g__AddLine|0_0(new GeneralLineData
				{
					Type = 1,
					Args = new List<string>
					{
						LanguageKey.LK_CombatSkill_SwapSkill_Tip_SubTitle_1.Tr()
					}
				}, ref CS$<>8__locals1);
				SkillBreakStartSwapButtonTipsHelper.<RefreshStartSwapButtonTips>g__AddLine|0_0(new GeneralLineData
				{
					Type = 2,
					Args = new List<string>
					{
						"ui9_icon_resource_small_8",
						LanguageKey.LK_CombatSkill_SwapSkill_Tip_SubContent_1.TrFormat(currentExpText, baseExpText, extraExpText)
					}
				}, ref CS$<>8__locals1);
				SkillBreakStartSwapButtonTipsHelper.<RefreshStartSwapButtonTips>g__AddLine|0_0(new GeneralLineData
				{
					Type = 1,
					Args = new List<string>
					{
						LanguageKey.LK_CombatSkill_SwapSkill_Tip_SubTitle_2.Tr()
					}
				}, ref CS$<>8__locals1);
				SkillBreakStartSwapButtonTipsHelper.<RefreshStartSwapButtonTips>g__AddLine|0_0(new GeneralLineData(5, new List<string>
				{
					LanguageKey.LK_CombatSkill_SwapSkill_Tip_SubContent_2_1.Tr()
				}, new List<object>
				{
					20f
				}), ref CS$<>8__locals1);
				SkillBreakStartSwapButtonTipsHelper.<RefreshStartSwapButtonTips>g__AddLine|0_0(new GeneralLineData(5, new List<string>
				{
					LanguageKey.LK_CombatSkill_SwapSkill_Tip_SubContent_2_2.Tr()
				}, new List<object>
				{
					20f
				}), ref CS$<>8__locals1);
				SkillBreakStartSwapButtonTipsHelper.<RefreshStartSwapButtonTips>g__AddLine|0_0(new GeneralLineData
				{
					Type = 1,
					Args = new List<string>
					{
						LanguageKey.LK_CombatSkill_SwapSkill_Tip_SubTitle_3.Tr()
					}
				}, ref CS$<>8__locals1);
				SkillBreakStartSwapButtonTipsHelper.<RefreshStartSwapButtonTips>g__AddLine|0_0(new GeneralLineData(5, new List<string>
				{
					LanguageKey.LK_CombatSkill_SwapSkill_Tip_SubContent_3_1.TrFormat(usedCountText)
				}, new List<object>
				{
					20f
				}), ref CS$<>8__locals1);
				SkillBreakStartSwapButtonTipsHelper.<RefreshStartSwapButtonTips>g__AddLine|0_0(new GeneralLineData
				{
					Type = 1,
					Args = new List<string>
					{
						LanguageKey.LK_CombatSkill_SwapSkill_Tip_SubContent_3_2.Tr()
					}
				}, ref CS$<>8__locals1);
				CS$<>8__locals1.tipParam.Set("LineCount", CS$<>8__locals1.lineCount);
				tip.Refresh(false, -1);
			}
		}

		// Token: 0x06009250 RID: 37456 RVA: 0x00442E78 File Offset: 0x00441078
		private static int CalculateRequiredBaseExp(short skillId)
		{
			return (int)(GlobalConfig.Instance.SwapSkillBreakCostExp * Config.SkillBreakPlate.Instance[CombatSkill.Instance[skillId].Grade].CostExp);
		}

		// Token: 0x06009251 RID: 37457 RVA: 0x00442EB4 File Offset: 0x004410B4
		private static int CalculateRequiredExtraExp(GameData.Domains.Taiwu.SkillBreakPlate displayPlate, short skillId)
		{
			int usedSwapCount = displayPlate.SwapCount;
			return usedSwapCount * (int)Config.SkillBreakPlate.Instance[CombatSkill.Instance[skillId].Grade].CostExp;
		}

		// Token: 0x06009252 RID: 37458 RVA: 0x00442EF0 File Offset: 0x004410F0
		[CompilerGenerated]
		internal static void <RefreshStartSwapButtonTips>g__AddLine|0_0(GeneralLineData lineData, ref SkillBreakStartSwapButtonTipsHelper.<>c__DisplayClass0_0 A_1)
		{
			ArgumentBox tipParam = A_1.tipParam;
			string format = "LineData{0}";
			int num = A_1.lineCount + 1;
			A_1.lineCount = num;
			tipParam.SetObject(string.Format(format, num), lineData);
		}
	}
}
