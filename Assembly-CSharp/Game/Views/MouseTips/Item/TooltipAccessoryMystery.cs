using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using Config.ConfigCells;
using Game.Views.MouseTips.Item.Common;
using GameData.Combat.Math;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips.Item
{
	// Token: 0x02000896 RID: 2198
	public class TooltipAccessoryMystery : MonoBehaviour
	{
		// Token: 0x06006948 RID: 26952 RVA: 0x00306308 File Offset: 0x00304508
		public void Refresh(MysteryEffectItem config, int index, short requirementsPower, bool isTemplate)
		{
			LanguageKey indexKey = CommonUtils.TraditionalChineseNumber[index + 1];
			int powerRequirement = config.PowerRequirements[index];
			string indexStr = indexKey.Tr() + "·";
			string powerStr = string.Format("{0}%", powerRequirement);
			this.textIndex.SetText(indexStr, true);
			this.textPower.SetText(powerStr, true);
			this.styleRoot.SetInteractable(powerRequirement <= (int)requirementsPower || isTemplate);
			Transform template = this.layoutProperty.GetChild(0);
			List<PropertyAndValueAndModifyType> propertyList = config.BonusValues[index];
			bool isFullPoison = propertyList.Count(delegate(PropertyAndValueAndModifyType p)
			{
				ECharacterPropertyReferencedType type = p.Type;
				return type >= ECharacterPropertyReferencedType.ResistOfHotPoison && type <= ECharacterPropertyReferencedType.ResistOfIllusoryPoison;
			}) == 6;
			List<TooltipItemProperty> propertyItemList = this.layoutProperty.GetComponentsInChildren<TooltipItemProperty>(true).ToList<TooltipItemProperty>();
			int propertyCount = isFullPoison ? 1 : propertyList.Count;
			bool flag = isFullPoison;
			if (flag)
			{
				TooltipItemProperty propertyItem = propertyItemList.First<TooltipItemProperty>();
				string icon = "mousetip_duxing_big_all";
				string title = LanguageKey.LK_CombatSkill_AllPoison.Tr();
				PropertyAndValueAndModifyType property = propertyList.First<PropertyAndValueAndModifyType>();
				string desc = property.Value.ToString() + ((property.Modify == EDataModifyType.AddPercent) ? "%" : "");
				propertyItem.Set(icon, title, desc, true);
				propertyItem.gameObject.SetActive(true);
			}
			else
			{
				for (int i = 0; i < propertyCount; i++)
				{
					PropertyAndValueAndModifyType property2 = propertyList[i];
					CharacterPropertyReferencedItem propertyReferencedItem = CharacterPropertyReferenced.Instance[(int)property2.Type];
					CharacterPropertyDisplayItem propertyDisplayItem = CharacterPropertyDisplay.Instance[propertyReferencedItem.DisplayType];
					TooltipItemProperty propertyItem2 = (i < propertyItemList.Count) ? propertyItemList[i] : Object.Instantiate<Transform>(template, this.layoutProperty).GetComponent<TooltipItemProperty>();
					string icon2 = propertyDisplayItem.TipsBigIcon;
					string title2 = propertyDisplayItem.Name;
					string desc2 = property2.Value.ToString() + ((property2.Modify == EDataModifyType.AddPercent) ? "%" : "");
					propertyItem2.Set(icon2, title2, desc2, true);
					propertyItem2.gameObject.SetActive(true);
				}
			}
			for (int j = propertyCount; j < propertyItemList.Count; j++)
			{
				propertyItemList[j].gameObject.SetActive(false);
			}
		}

		// Token: 0x04004B87 RID: 19335
		[SerializeField]
		private TextMeshProUGUI textIndex;

		// Token: 0x04004B88 RID: 19336
		[SerializeField]
		private TextMeshProUGUI textPower;

		// Token: 0x04004B89 RID: 19337
		[SerializeField]
		private Transform layoutProperty;

		// Token: 0x04004B8A RID: 19338
		[SerializeField]
		private DisableStyleRoot styleRoot;
	}
}
