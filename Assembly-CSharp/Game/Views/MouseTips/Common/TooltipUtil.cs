using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using Game.Views.MouseTips.Item.Common;
using UnityEngine;

namespace Game.Views.MouseTips.Common
{
	// Token: 0x020008B7 RID: 2231
	public static class TooltipUtil
	{
		// Token: 0x06006A90 RID: 27280 RVA: 0x00312F74 File Offset: 0x00311174
		public static void AppendAddProperty(ref int index, List<TooltipItemProperty> propertyList, short type, int baseValue, int finalValue, bool isDetail, bool isRecover = false, bool percent = false, bool showZero = false, bool showAddMark = true, bool isBigIcon = false, bool basedOnPercent = false, bool isRecoveryProperty = false)
		{
			bool flag = finalValue == 0 && !showZero;
			if (!flag)
			{
				bool flag2 = index < propertyList.Count;
				TooltipItemProperty addPropertyUi;
				if (flag2)
				{
					addPropertyUi = propertyList[index];
				}
				else
				{
					TooltipItemProperty template = propertyList.First<TooltipItemProperty>();
					addPropertyUi = Object.Instantiate<TooltipItemProperty>(template, template.transform.parent);
					addPropertyUi.transform.localScale = Vector3.one;
				}
				bool isInverse = false;
				bool flag3 = (int)type < CharacterPropertyReferenced.Instance.Count;
				string propertyName;
				string iconName;
				if (flag3)
				{
					CharacterPropertyDisplayItem configData = CharacterPropertyDisplay.Instance[CharacterPropertyReferenced.Instance[type].DisplayType];
					propertyName = configData.Name;
					if (isBigIcon)
					{
						iconName = configData.TipsBigIcon;
					}
					else
					{
						iconName = configData.TipsIcon;
					}
				}
				else
				{
					CombatSkillPropertyItem configData2 = CombatSkillProperty.Instance[(int)type - CharacterPropertyReferenced.Instance.Count];
					propertyName = configData2.Name;
					iconName = configData2.TipsIcon;
				}
				string addMarkStr = showAddMark ? "+" : "";
				string percentStr = percent ? "%" : "";
				bool isBuff = (!isInverse && finalValue >= 0) || (isInverse && finalValue < 0);
				int valueStr = basedOnPercent ? (100 + finalValue) : finalValue;
				string showValueStr = (finalValue >= 0) ? string.Format("{0}{1}", addMarkStr, valueStr) : string.Format("{0}", valueStr);
				string title = isRecover ? (propertyName + LocalStringManager.Get(LanguageKey.LK_ItemTips_Property_Recover_Postfix)) : propertyName;
				string value = (showValueStr + percentStr).SetColor(isBuff ? "brightblue" : "brightred");
				string offsetValue = isDetail ? TooltipItemBase.GetOffsetValue(baseValue, finalValue, null, percent) : string.Empty;
				addPropertyUi.Set(iconName, title, value + offsetValue, true);
				addPropertyUi.gameObject.SetActive(true);
				index++;
			}
		}
	}
}
