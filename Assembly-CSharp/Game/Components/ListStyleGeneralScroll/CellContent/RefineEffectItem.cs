using System;
using Config;
using Game.Views.MouseTips.Item.Common;
using GameData.Domains.Item;
using TMPro;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EE0 RID: 3808
	public class RefineEffectItem : MonoBehaviour
	{
		// Token: 0x0600AF3A RID: 44858 RVA: 0x004FD598 File Offset: 0x004FB798
		public void Refresh(ItemKey itemKey, sbyte targetItemType)
		{
			short materialTemplateId = itemKey.TemplateId;
			base.gameObject.SetActive(materialTemplateId >= 0);
			bool flag = materialTemplateId < 0;
			if (!flag)
			{
				MaterialItem materialConfig = Config.Material.Instance[materialTemplateId];
				RefiningEffectItem refineConfig = RefiningEffect.Instance[materialConfig.RefiningEffect];
				if (!true)
				{
				}
				sbyte[] array;
				switch (targetItemType)
				{
				case 0:
					array = refineConfig.WeaponBonusValues;
					break;
				case 1:
					array = refineConfig.ArmorBonusValues;
					break;
				case 2:
					array = refineConfig.AccessoryBonusValues;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				if (!true)
				{
				}
				sbyte[] effectType = array;
				if (!true)
				{
				}
				string text;
				switch (targetItemType)
				{
				case 0:
					text = TipsRefiningEffect.GetRefinePropertyIconName(refineConfig.WeaponType, true);
					break;
				case 1:
					text = TipsRefiningEffect.GetRefinePropertyIconName(refineConfig.ArmorType, true);
					break;
				case 2:
					text = TipsRefiningEffect.GetRefinePropertyIconName(refineConfig.AccessoryType, true);
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				if (!true)
				{
				}
				string iconName = text;
				if (!true)
				{
				}
				switch (targetItemType)
				{
				case 0:
					text = TipsRefiningEffect.GetRefinePropertyName(refineConfig.WeaponType);
					break;
				case 1:
					text = TipsRefiningEffect.GetRefinePropertyName(refineConfig.ArmorType);
					break;
				case 2:
					text = TipsRefiningEffect.GetRefinePropertyName(refineConfig.AccessoryType);
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				if (!true)
				{
				}
				string propertyName = text;
				this.icon.SetSprite(iconName, false, null);
				this.poisonName.text = propertyName;
				if (!true)
				{
				}
				ECharacterPropertyDisplayType echaracterPropertyDisplayTypeByERefiningEffectType;
				switch (targetItemType)
				{
				case 0:
					echaracterPropertyDisplayTypeByERefiningEffectType = TipsRefiningEffect.GetECharacterPropertyDisplayTypeByERefiningEffectType(refineConfig.WeaponType);
					break;
				case 1:
					echaracterPropertyDisplayTypeByERefiningEffectType = TipsRefiningEffect.GetECharacterPropertyDisplayTypeByERefiningEffectType(refineConfig.ArmorType);
					break;
				case 2:
					echaracterPropertyDisplayTypeByERefiningEffectType = TipsRefiningEffect.GetECharacterPropertyDisplayTypeByERefiningEffectType(refineConfig.AccessoryType);
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				if (!true)
				{
				}
				ECharacterPropertyDisplayType displayType = echaracterPropertyDisplayTypeByERefiningEffectType;
				CharacterPropertyDisplayItem displayTypeConfig = CharacterPropertyDisplay.Instance[displayType.ToInt()];
				sbyte value = effectType[(int)materialConfig.Grade];
				int unit = (displayType == ECharacterPropertyDisplayType.EquipmentMinAttackRange || displayType == ECharacterPropertyDisplayType.EquipmentMaxAttackRange) ? 10 : 1;
				string valueStr = TooltipItemRefiningEffect.GetValueStr((int)value, displayTypeConfig.IsPercent, unit);
				this.poisonValue.text = valueStr;
				this.disableStyleRoot.SetStyleEffect(itemKey.IsValid(), false);
			}
		}

		// Token: 0x040087C0 RID: 34752
		[SerializeField]
		private CImage icon;

		// Token: 0x040087C1 RID: 34753
		[SerializeField]
		private TextMeshProUGUI poisonName;

		// Token: 0x040087C2 RID: 34754
		[SerializeField]
		private TextMeshProUGUI poisonValue;

		// Token: 0x040087C3 RID: 34755
		[SerializeField]
		private DisableStyleRoot disableStyleRoot;
	}
}
