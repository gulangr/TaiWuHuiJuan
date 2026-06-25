using System;
using System.Collections.Generic;
using Config;
using Config.ConfigCells;
using GameData.Domains.Item;
using GameData.Domains.Merchant;
using UnityEngine;

// Token: 0x02000349 RID: 841
public class ItemUtils
{
	// Token: 0x06003146 RID: 12614 RVA: 0x00184454 File Offset: 0x00182654
	public static string GetItemColorName(sbyte itemType, short templateId)
	{
		string name = ItemTemplateHelper.GetName(itemType, templateId);
		sbyte grade = ItemTemplateHelper.GetGrade(itemType, templateId);
		Color color = Colors.Instance.GradeColors[(int)grade];
		return string.Concat(new string[]
		{
			"<color=",
			color.ColorToHexString("#"),
			">",
			name,
			"</color>"
		});
	}

	// Token: 0x06003147 RID: 12615 RVA: 0x001844BC File Offset: 0x001826BC
	public static string GetItemIconBack(sbyte itemType, short templateId)
	{
		return string.Format("itemiconback_{0}", itemType);
	}

	// Token: 0x06003148 RID: 12616 RVA: 0x001844E0 File Offset: 0x001826E0
	public static int GetItemBuyPrice(int srcPrice, int favorChangeRate, int percentChange)
	{
		return Math.Max(0, srcPrice * (200 - 50 * favorChangeRate / 100 + percentChange) / 100);
	}

	// Token: 0x06003149 RID: 12617 RVA: 0x0018450C File Offset: 0x0018270C
	public static int GetItemSoldPrice(int srcPrice, int favorChangeRate, short itemSubType, short merchantLoveItemType, short merchantHateItemType, sbyte merchantBehaviorType, int durabilityRate)
	{
		return MerchantData.GetItemSoldPrice(srcPrice, favorChangeRate, itemSubType, merchantLoveItemType, merchantHateItemType, merchantBehaviorType, durabilityRate);
	}

	// Token: 0x0600314A RID: 12618 RVA: 0x00184520 File Offset: 0x00182720
	public static bool MatchItemFilterRule(ItemKey itemKey, ItemFilterRulesItem rule)
	{
		bool flag = rule == null;
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			bool flag2 = rule.AppointId.TemplateId != -1;
			if (flag2)
			{
				result = (itemKey.ItemType == rule.AppointId.ItemType && itemKey.TemplateId == rule.AppointId.TemplateId);
			}
			else
			{
				List<PresetItemSubTypeWithGradeRange> appointOrSubTypeCore = rule.AppointOrSubTypeCore;
				bool flag3 = appointOrSubTypeCore != null && appointOrSubTypeCore.Count > 0;
				if (flag3)
				{
					sbyte grade = ItemTemplateHelper.GetGrade(itemKey.ItemType, itemKey.TemplateId);
					short subType = ItemTemplateHelper.GetItemSubType(itemKey.ItemType, itemKey.TemplateId);
					foreach (PresetItemSubTypeWithGradeRange itemSubTypeWithGrade in rule.AppointOrSubTypeCore)
					{
						bool flag4 = subType == itemSubTypeWithGrade.SubType && grade >= itemSubTypeWithGrade.GradeMin && grade <= itemSubTypeWithGrade.GradeMax;
						if (flag4)
						{
							return true;
						}
					}
				}
				bool flag5 = rule.AppointOrIdCore != null && rule.AppointOrIdCore.Count > 0;
				if (flag5)
				{
					foreach (PresetItemTemplateIdGroup coreCell in rule.AppointOrIdCore)
					{
						bool flag6 = itemKey.ItemType == coreCell.ItemType && itemKey.TemplateId >= coreCell.StartId && itemKey.TemplateId < coreCell.StartId + (short)coreCell.GroupLength;
						if (flag6)
						{
							return true;
						}
					}
				}
				result = false;
			}
		}
		return result;
	}
}
