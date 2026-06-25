using System;
using System.Collections.Generic;
using FrameWork;
using GameData.Domains.Item;

// Token: 0x02000385 RID: 901
public static class HealAttainmentTipsHelper
{
	// Token: 0x0600358F RID: 13711 RVA: 0x001AEA0C File Offset: 0x001ACC0C
	public static void RefreshTips(TooltipInvoker tipDisplayer, ItemKey toolKey, bool isEmptyTool, List<HealAttainmentTipsHelper.AttainmentItem> attainmentItems, bool isHeal = true)
	{
		if (tipDisplayer.RuntimeParam == null)
		{
			tipDisplayer.RuntimeParam = new ArgumentBox();
		}
		ArgumentBox tipParam = tipDisplayer.RuntimeParam;
		tipDisplayer.Type = TipType.GeneralLines;
		tipParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_Heal_AttainmentTip_Title));
		int lineCount = 0;
		HealAttainmentTipsHelper.AddNode(tipParam, HealAttainmentTipsHelper.DescLine(toolKey, isEmptyTool, attainmentItems), ref lineCount);
		LanguageKey attainmentTitleKey = isHeal ? LanguageKey.LK_Heal_AttainmentTip_Attainment_Title_Heal : LanguageKey.LK_Heal_AttainmentTip_Attainment_Title_Make;
		HealAttainmentTipsHelper.AddNode(tipParam, HealAttainmentTipsHelper.SubTitleLine(attainmentTitleKey), ref lineCount);
		attainmentItems.ForEach(delegate(HealAttainmentTipsHelper.AttainmentItem item)
		{
			HealAttainmentTipsHelper.AddNode(tipParam, HealAttainmentTipsHelper.CharAttainmentLine(item), ref lineCount);
		});
		HealAttainmentTipsHelper.AddNode(tipParam, HealAttainmentTipsHelper.SubTitleLine(LanguageKey.LK_Heal_AttainmentTip_ToolEffect_Title), ref lineCount);
		attainmentItems.ForEach(delegate(HealAttainmentTipsHelper.AttainmentItem item)
		{
			HealAttainmentTipsHelper.AddNode(tipParam, HealAttainmentTipsHelper.ToolEffectLine(item), ref lineCount);
		});
		tipParam.Set("LineCount", lineCount);
	}

	// Token: 0x06003590 RID: 13712 RVA: 0x001AEB0C File Offset: 0x001ACD0C
	private static GeneralLineData ToolEffectLine(HealAttainmentTipsHelper.AttainmentItem item)
	{
		string attainmentIcon = "sp_14_iconjiyizhanshi_" + item.SkillType.ToString();
		string attainmentName = LocalStringManager.Get("LK_LifeSkillType_" + item.SkillType.ToString());
		string deltaStr = (item.DeltaAttainment > 0) ? string.Format("<color=#brightblue>+{0}</color>", item.DeltaAttainment) : string.Format("<color=#brightred>{0}</color>", item.DeltaAttainment);
		return new GeneralLineData
		{
			Type = 2,
			Args = new List<string>
			{
				attainmentIcon,
				LocalStringManager.GetFormat(LanguageKey.LK_Heal_AttainmentTip_Attainment, attainmentName, deltaStr.ColorReplace())
			}
		};
	}

	// Token: 0x06003591 RID: 13713 RVA: 0x001AEBC4 File Offset: 0x001ACDC4
	private static GeneralLineData CharAttainmentLine(HealAttainmentTipsHelper.AttainmentItem item)
	{
		string attainmentIcon = "sp_14_iconjiyizhanshi_" + item.SkillType.ToString();
		string attainmentName = LocalStringManager.Get("LK_LifeSkillType_" + item.SkillType.ToString());
		string deltaStr = item.Attainment.ToString().SetColor("pinkyellow");
		return new GeneralLineData
		{
			Type = 2,
			Args = new List<string>
			{
				attainmentIcon,
				LocalStringManager.GetFormat(LanguageKey.LK_Heal_AttainmentTip_Attainment, attainmentName, deltaStr.ColorReplace())
			}
		};
	}

	// Token: 0x06003592 RID: 13714 RVA: 0x001AEC58 File Offset: 0x001ACE58
	private static GeneralLineData SubTitleLine(LanguageKey key)
	{
		return new GeneralLineData
		{
			Type = 1,
			Args = new List<string>
			{
				LocalStringManager.Get(key)
			}
		};
	}

	// Token: 0x06003593 RID: 13715 RVA: 0x001AEC90 File Offset: 0x001ACE90
	private static GeneralLineData DescLine(ItemKey toolKey, bool isEmptyTool, List<HealAttainmentTipsHelper.AttainmentItem> attainmentItems)
	{
		GeneralLineData result;
		if (isEmptyTool)
		{
			result = new GeneralLineData
			{
				Type = 3,
				Args = new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Heal_AttainmentTip_Desc_EmptyTool)
				}
			};
		}
		else
		{
			string itemName = ItemTemplateHelper.GetName(toolKey.ItemType, toolKey.TemplateId);
			sbyte itemGrade = ItemTemplateHelper.GetGrade(toolKey.ItemType, toolKey.TemplateId);
			int validIndex = attainmentItems.FindIndex((HealAttainmentTipsHelper.AttainmentItem a) => a.DeltaAttainment > 0);
			HealAttainmentTipsHelper.AttainmentItem validAttainmentItem = attainmentItems[validIndex];
			bool isAdd = validAttainmentItem.DeltaAttainment >= 0;
			string attainmentName = LocalStringManager.Get("LK_LifeSkillType_" + validAttainmentItem.SkillType.ToString());
			result = new GeneralLineData
			{
				Type = 3,
				Args = new List<string>
				{
					LocalStringManager.GetFormat(isAdd ? LanguageKey.LK_Heal_AttainmentTip_Desc_Add : LanguageKey.LK_Heal_AttainmentTip_Desc_Reduce, itemName.SetGradeColor((int)itemGrade), attainmentName)
				}
			};
		}
		return result;
	}

	// Token: 0x06003594 RID: 13716 RVA: 0x001AED94 File Offset: 0x001ACF94
	private static void AddNode(ArgumentBox tipParam, GeneralLineData lineData, ref int lineCount)
	{
		string format = "LineData{0}";
		int num = lineCount + 1;
		lineCount = num;
		tipParam.SetObject(string.Format(format, num), lineData);
	}

	// Token: 0x0200179C RID: 6044
	public struct AttainmentItem
	{
		// Token: 0x0400AC1E RID: 44062
		public sbyte SkillType;

		// Token: 0x0400AC1F RID: 44063
		public int DeltaAttainment;

		// Token: 0x0400AC20 RID: 44064
		public int Attainment;
	}
}
