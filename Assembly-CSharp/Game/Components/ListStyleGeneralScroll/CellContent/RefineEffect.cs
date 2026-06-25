using System;
using Config;
using Game.Views.MouseTips.Item.Common;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EDE RID: 3806
	public class RefineEffect : MonoBehaviour, ICellContent<ItemDisplayData>, ICellContent
	{
		// Token: 0x0600AF34 RID: 44852 RVA: 0x004FD324 File Offset: 0x004FB524
		public void SetData(ItemDisplayData material)
		{
			bool flag = RefineEffect._itemType < 0;
			if (!flag)
			{
				sbyte targetItemType = RefineEffect._itemType;
				MaterialItem materialConfig = Config.Material.Instance[material.RealKey.TemplateId];
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
			}
		}

		// Token: 0x0600AF35 RID: 44853 RVA: 0x004FD52A File Offset: 0x004FB72A
		public static void SetTarget(sbyte itemType)
		{
			RefineEffect._itemType = itemType;
		}

		// Token: 0x040087BB RID: 34747
		[SerializeField]
		private CImage icon;

		// Token: 0x040087BC RID: 34748
		[SerializeField]
		private TextMeshProUGUI poisonName;

		// Token: 0x040087BD RID: 34749
		[SerializeField]
		private TextMeshProUGUI poisonValue;

		// Token: 0x040087BE RID: 34750
		private static sbyte _itemType = -1;
	}
}
