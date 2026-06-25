using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using Config.ConfigCells.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Utilities;
using GameDataExtensions;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips.Item.Common
{
	// Token: 0x020008B4 RID: 2228
	public class TooltipItemRequirementArea : MonoBehaviour
	{
		// Token: 0x06006A7B RID: 27259 RVA: 0x0031255C File Offset: 0x0031075C
		private void RefreshRequirements([TupleElementNames(new string[]
		{
			"type",
			"required",
			"actual"
		})] List<ValueTuple<int, int, int>> requirements)
		{
			bool flag = requirements == null;
			if (!flag)
			{
				this._requirementRows.Clear();
				this._requirementRows.AddRange(this.requirementContainer.GetComponentsInChildren<TooltipCombatSkillRequirementItem>(true));
				for (int i = 0; i < requirements.Count; i++)
				{
					ValueTuple<int, int, int> valueTuple = requirements[i];
					int type = valueTuple.Item1;
					int required = valueTuple.Item2;
					int actual = valueTuple.Item3;
					int displayType = (int)((type >= 0 && type < CharacterPropertyReferenced.Instance.Count) ? CharacterPropertyReferenced.Instance[type].DisplayType : -1);
					string iconSprite = (displayType >= 0 && displayType < CharacterPropertyDisplay.Instance.Count) ? CharacterPropertyDisplay.Instance[displayType].TipsIcon : "";
					string name = (displayType >= 0 && displayType < CharacterPropertyDisplay.Instance.Count) ? CharacterPropertyDisplay.Instance[displayType].ShortName : "";
					TooltipCombatSkillRequirementItem row = (i < this._requirementRows.Count) ? this._requirementRows[i] : Object.Instantiate<TooltipCombatSkillRequirementItem>(this.requirementTemplateItem, this.requirementContainer);
					row.Set(iconSprite, name, actual, required, false);
					row.gameObject.SetActive(true);
				}
				for (int j = requirements.Count; j < this._requirementRows.Count; j++)
				{
					this._requirementRows[j].gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06006A7C RID: 27260 RVA: 0x003126E4 File Offset: 0x003108E4
		public static string GetPowerStr(ItemDisplayData itemData)
		{
			bool isValid = itemData != null && itemData.ShouldShowPower();
			string requirementsPowerStr = isValid ? (itemData.PowerInfo.RequirementsPower.ToString() + "%") : "-";
			string maxPowerStr = isValid ? (itemData.PowerInfo.MaxPower.ToString() + "%") : "-";
			return LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_WeaponAndArmor_RequirementsPower_Content_0, requirementsPowerStr.SetColor("lowwarning"), maxPowerStr.SetColor("brightyellow"));
		}

		// Token: 0x06006A7D RID: 27261 RVA: 0x00312774 File Offset: 0x00310974
		public void Refresh(bool isShow, ItemDisplayData itemData)
		{
			base.gameObject.SetActive(isShow);
			bool flag = !isShow;
			if (!flag)
			{
				this.requirementPowerText.text = TooltipItemRequirementArea.GetPowerStr(itemData);
				this.RefreshRequirements(itemData.Requirements);
			}
		}

		// Token: 0x06006A7E RID: 27262 RVA: 0x003127B8 File Offset: 0x003109B8
		public void Refresh(bool isShow, ItemKey itemKey, bool isMystery = false)
		{
			base.gameObject.SetActive(isShow);
			bool flag = !isShow;
			if (!flag)
			{
				this.requirementPowerText.text = TooltipItemRequirementArea.GetPowerStr(null);
				sbyte itemType = itemKey.ItemType;
				if (!true)
				{
				}
				List<PropertyAndValue> requiredCharacterProperties2;
				switch (itemType)
				{
				case 0:
					requiredCharacterProperties2 = Weapon.Instance[itemKey.TemplateId].RequiredCharacterProperties;
					break;
				case 1:
					requiredCharacterProperties2 = Armor.Instance[itemKey.TemplateId].RequiredCharacterProperties;
					break;
				case 2:
					requiredCharacterProperties2 = Accessory.Instance[itemKey.TemplateId].RequiredCharacterProperties;
					break;
				default:
					throw new Exception(string.Format("Cannot get use power of item type {0}", itemKey.ItemType));
				}
				if (!true)
				{
				}
				List<PropertyAndValue> requiredCharacterProperties = requiredCharacterProperties2;
				List<ValueTuple<int, int, int>> requirements = new List<ValueTuple<int, int, int>>();
				foreach (PropertyAndValue requirement in requiredCharacterProperties)
				{
					requirements.Add(new ValueTuple<int, int, int>((int)requirement.PropertyId, (int)requirement.Value, 0));
				}
				if (isMystery)
				{
					int compatibility = 0;
					int require = itemKey.GetConfig().MysteryEffect.CompatibilityRequirement;
					requirements.Add(new ValueTuple<int, int, int>(135, require, compatibility));
				}
				this.RefreshRequirements(requirements);
			}
		}

		// Token: 0x04004CEA RID: 19690
		[SerializeField]
		private TextMeshProUGUI requirementPowerText;

		// Token: 0x04004CEB RID: 19691
		[SerializeField]
		private Transform requirementContainer;

		// Token: 0x04004CEC RID: 19692
		[SerializeField]
		private TooltipCombatSkillRequirementItem requirementTemplateItem;

		// Token: 0x04004CED RID: 19693
		private readonly List<TooltipCombatSkillRequirementItem> _requirementRows = new List<TooltipCombatSkillRequirementItem>();
	}
}
