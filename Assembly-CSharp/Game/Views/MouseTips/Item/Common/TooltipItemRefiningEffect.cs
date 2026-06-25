using System;
using Config;
using GameData.Domains.Item;
using UnityEngine;

namespace Game.Views.MouseTips.Item.Common
{
	// Token: 0x020008B3 RID: 2227
	public class TooltipItemRefiningEffect : MonoBehaviour
	{
		// Token: 0x06006A76 RID: 27254 RVA: 0x00311FE8 File Offset: 0x003101E8
		public void SetForWeapon(RefiningEffects refiningEffects)
		{
			int templateIndexOffset = this.tmeplateProperty.transform.GetSiblingIndex();
			int count = -1;
			for (int i = 0; i < 5; i++)
			{
				short templateId = refiningEffects.GetMaterialTemplateIdAt(i);
				MaterialItem config = Config.Material.Instance[templateId];
				int value = (int)((templateId >= 0) ? RefiningEffect.Instance[config.RefiningEffect].WeaponBonusValues[(int)config.Grade] : 0);
				bool flag = value != 0;
				if (flag)
				{
					count++;
					Transform child = (count + templateIndexOffset < base.transform.childCount) ? base.transform.GetChild(count + templateIndexOffset) : Object.Instantiate<TooltipItemProperty>(this.tmeplateProperty, base.transform).transform;
					child.gameObject.SetActive(true);
					TooltipItemProperty property = child.GetComponent<TooltipItemProperty>();
					ERefiningEffectWeaponType eRefiningEffectWeaponType = RefiningEffect.Instance[config.RefiningEffect].WeaponType;
					ECharacterPropertyDisplayType propertyDisplayType = TipsRefiningEffect.GetECharacterPropertyDisplayTypeByERefiningEffectType(eRefiningEffectWeaponType);
					CharacterPropertyDisplayItem propertyDisplayItem = CharacterPropertyDisplay.Instance[(int)propertyDisplayType];
					int unit = (propertyDisplayType == ECharacterPropertyDisplayType.EquipmentMinAttackRange || propertyDisplayType == ECharacterPropertyDisplayType.EquipmentMaxAttackRange) ? 10 : 1;
					string result = TooltipItemRefiningEffect.GetValueStr(value, propertyDisplayItem.IsPercent, unit);
					property.Set(propertyDisplayItem.TipsIcon, propertyDisplayItem.Name, result.SetColor("brightblue"), true);
				}
			}
			for (int j = count + templateIndexOffset + 1; j < base.transform.childCount; j++)
			{
				base.transform.GetChild(j).gameObject.SetActive(false);
			}
			base.gameObject.SetActive(count >= 0);
		}

		// Token: 0x06006A77 RID: 27255 RVA: 0x00312194 File Offset: 0x00310394
		public void SetForArmor(RefiningEffects refiningEffects)
		{
			int templateIndexOffset = this.tmeplateProperty.transform.GetSiblingIndex();
			int count = -1;
			for (int i = 0; i < 5; i++)
			{
				short templateId = refiningEffects.GetMaterialTemplateIdAt(i);
				MaterialItem config = Config.Material.Instance[templateId];
				int value = (int)((templateId >= 0) ? RefiningEffect.Instance[config.RefiningEffect].ArmorBonusValues[(int)config.Grade] : 0);
				bool flag = value != 0;
				if (flag)
				{
					count++;
					Transform child = (count + templateIndexOffset < base.transform.childCount) ? base.transform.GetChild(count + templateIndexOffset) : Object.Instantiate<TooltipItemProperty>(this.tmeplateProperty, base.transform).transform;
					child.gameObject.SetActive(true);
					TooltipItemProperty property = child.GetComponent<TooltipItemProperty>();
					ERefiningEffectArmorType eRefiningEffectArmorType = RefiningEffect.Instance[config.RefiningEffect].ArmorType;
					ECharacterPropertyDisplayType propertyDisplayType = TipsRefiningEffect.GetECharacterPropertyDisplayTypeByERefiningEffectType(eRefiningEffectArmorType);
					CharacterPropertyDisplayItem propertyDisplayItem = CharacterPropertyDisplay.Instance[(int)propertyDisplayType];
					string result = TooltipItemRefiningEffect.GetValueStr(value, propertyDisplayItem.IsPercent, 1);
					property.Set(propertyDisplayItem.TipsIcon, propertyDisplayItem.Name, result.SetColor("brightblue"), true);
				}
			}
			for (int j = count + templateIndexOffset + 1; j < base.transform.childCount; j++)
			{
				base.transform.GetChild(j).gameObject.SetActive(false);
			}
			base.gameObject.SetActive(count >= 0);
		}

		// Token: 0x06006A78 RID: 27256 RVA: 0x00312328 File Offset: 0x00310528
		public void SetForAccessory(RefiningEffects refiningEffects)
		{
			int templateIndexOffset = this.tmeplateProperty.transform.GetSiblingIndex();
			int count = -1;
			for (int i = 0; i < 5; i++)
			{
				short templateId = refiningEffects.GetMaterialTemplateIdAt(i);
				MaterialItem config = Config.Material.Instance[templateId];
				int value = (int)((templateId >= 0) ? RefiningEffect.Instance[config.RefiningEffect].AccessoryBonusValues[(int)config.Grade] : 0);
				bool flag = value != 0;
				if (flag)
				{
					count++;
					Transform child = (count + templateIndexOffset < base.transform.childCount) ? base.transform.GetChild(count + templateIndexOffset) : Object.Instantiate<TooltipItemProperty>(this.tmeplateProperty, base.transform).transform;
					child.gameObject.SetActive(true);
					TooltipItemProperty property = child.GetComponent<TooltipItemProperty>();
					ERefiningEffectAccessoryType eRefiningEffectAccessoryType = RefiningEffect.Instance[config.RefiningEffect].AccessoryType;
					ECharacterPropertyDisplayType propertyDisplayType = TipsRefiningEffect.GetECharacterPropertyDisplayTypeByERefiningEffectType(eRefiningEffectAccessoryType);
					CharacterPropertyDisplayItem propertyDisplayItem = CharacterPropertyDisplay.Instance[(int)propertyDisplayType];
					string result = TooltipItemRefiningEffect.GetValueStr(value, propertyDisplayItem.IsPercent, 1);
					property.Set(propertyDisplayItem.TipsIcon, propertyDisplayItem.Name, result.SetColor("brightblue"), true);
				}
			}
			for (int j = count + templateIndexOffset + 1; j < base.transform.childCount; j++)
			{
				base.transform.GetChild(j).gameObject.SetActive(false);
			}
			base.gameObject.SetActive(count >= 0);
		}

		// Token: 0x06006A79 RID: 27257 RVA: 0x003124BC File Offset: 0x003106BC
		public static string GetValueStr(int value, bool isPercent, int unit = 1)
		{
			bool flag = unit > 1;
			string result;
			if (flag)
			{
				result = (isPercent ? string.Format("+{0:f1}%", (float)value / (float)unit).SetColor("brightblue") : string.Format("+{0:f1}", (float)value / (float)unit).SetColor("brightblue"));
			}
			else
			{
				result = (isPercent ? string.Format("{0:+0;-0}%", value).SetColor("brightblue") : string.Format("{0:+0;-0}", value).SetColor("brightblue"));
			}
			return result;
		}

		// Token: 0x04004CE9 RID: 19689
		[SerializeField]
		protected TooltipItemProperty tmeplateProperty;
	}
}
