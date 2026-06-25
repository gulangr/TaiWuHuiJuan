using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using GameData.Domains.Combat;
using GameData.Domains.Item.Display;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.MouseTips.Item.Common
{
	// Token: 0x020008B5 RID: 2229
	public class TooltipItemSpecialArea : MonoBehaviour
	{
		// Token: 0x06006A80 RID: 27264 RVA: 0x0031292C File Offset: 0x00310B2C
		public void Refresh(ItemDisplayData itemData, int eatingTime)
		{
			bool isAnyShow = this.RefreshEquipmentEffect(itemData);
			isAnyShow = (this.RefreshEatingRemainDuration(eatingTime) || isAnyShow);
			isAnyShow = (this.RefreshWeave(itemData) || isAnyShow);
			base.gameObject.SetActive(isAnyShow);
			this.propertyKillWug.gameObject.SetActive(false);
		}

		// Token: 0x06006A81 RID: 27265 RVA: 0x00312978 File Offset: 0x00310B78
		public void RefreshWug(int eatingTime, string killWugTip)
		{
			this.RefreshEquipmentEffect(null);
			this.RefreshEatingRemainDuration(eatingTime);
			this.RefreshWeave(null);
			base.gameObject.SetActive(true);
			this.propertyKillWug.gameObject.SetActive(true);
			this.propertyKillWug.SetValue(killWugTip);
		}

		// Token: 0x06006A82 RID: 27266 RVA: 0x003129CC File Offset: 0x00310BCC
		private bool RefreshEquipmentEffect(ItemDisplayData itemData)
		{
			bool flag = itemData == null;
			bool result;
			if (flag)
			{
				this.layoutSpecial.gameObject.SetActive(false);
				result = false;
			}
			else
			{
				List<ValueTuple<string, string>> effects = TooltipItemSpecialArea.GetEquipmentEffectInfoList(itemData);
				Transform layout = this.layoutSpecial;
				Transform template = layout.GetChild(1);
				for (int i = 0; i < effects.Count; i++)
				{
					Transform child = (i + 1 < layout.childCount) ? layout.GetChild(i + 1) : Object.Instantiate<Transform>(template, layout);
					child.gameObject.SetActive(true);
					ValueTuple<string, string> valueTuple = effects[i];
					string effectName = valueTuple.Item1;
					string effectDesc = valueTuple.Item2;
					TooltipItemProperty property = child.GetComponent<TooltipItemProperty>();
					property.Set("", (i == 0) ? LanguageKey.LK_MouseTip_Equipment_SpecialEffect_Title.Tr() : "", effectName, true);
				}
				for (int j = effects.Count + 1; j < layout.childCount; j++)
				{
					layout.GetChild(j).gameObject.SetActive(false);
				}
				layout.gameObject.SetActive(effects.Count > 0);
				result = layout.gameObject.activeSelf;
			}
			return result;
		}

		// Token: 0x06006A83 RID: 27267 RVA: 0x00312B04 File Offset: 0x00310D04
		[return: TupleElementNames(new string[]
		{
			"name",
			"desc"
		})]
		public static List<ValueTuple<string, string>> GetEquipmentEffectInfoList(ItemDisplayData itemData)
		{
			List<ValueTuple<string, string>> effects = new List<ValueTuple<string, string>>();
			IItemConfig config = itemData.Key.GetConfig();
			bool flag = config.EquipmentMasteryId >= 0;
			if (flag)
			{
				SpecialEffectItem effectConfig = SpecialEffect.Instance[config.EquipmentMasteryId];
				effects.Add(new ValueTuple<string, string>(effectConfig.Name, effectConfig.Desc[0]));
			}
			List<short> equipmentEffectIds = itemData.EquipmentEffectIds;
			bool flag2 = equipmentEffectIds != null && equipmentEffectIds.Count > 0;
			if (flag2)
			{
				foreach (short effectId in itemData.EquipmentEffectIds)
				{
					EquipmentEffectItem effectConfig2 = EquipmentEffect.Instance[effectId];
					effects.Add(new ValueTuple<string, string>(effectConfig2.Name, effectConfig2.Desc));
				}
			}
			List<WeaponEffectDisplayData> weaponEffectDisplayDataList = itemData.WeaponEffectDisplayDataList;
			bool flag3 = weaponEffectDisplayDataList != null && weaponEffectDisplayDataList.Count > 0;
			if (flag3)
			{
				foreach (WeaponEffectDisplayData effectKey in itemData.WeaponEffectDisplayDataList)
				{
					string title = SpecialEffect.Instance[effectKey.EffectDescription.EffectId].Name;
					string desc = CommonUtils.GetSpecialEffectDescSpecifyIndex(effectKey.EffectDescription, 0, 1);
					effects.Add(new ValueTuple<string, string>(title, desc));
				}
			}
			return effects;
		}

		// Token: 0x06006A84 RID: 27268 RVA: 0x00312C98 File Offset: 0x00310E98
		private bool RefreshEatingRemainDuration(int eatingTime)
		{
			this.propertyEatingRemainDuration.gameObject.SetActive(eatingTime > 0);
			this.propertyEatingRemainDuration.SetValue(LanguageKey.LK_ItemTips_Eating_Time.TrFormat(eatingTime).ColorReplace());
			return this.propertyEatingRemainDuration.gameObject.activeSelf;
		}

		// Token: 0x06006A85 RID: 27269 RVA: 0x00312CF0 File Offset: 0x00310EF0
		private bool RefreshWeave(ItemDisplayData itemData)
		{
			bool isWeaved = itemData != null && itemData.IsWeaved;
			this.propertyWeave.gameObject.SetActive(isWeaved);
			bool flag = isWeaved;
			if (flag)
			{
				ClothingItem weavedClothingConfig = Clothing.Instance[itemData.WeavedClothingTemplateId];
				Color color = Colors.Instance.GradeColors[(int)weavedClothingConfig.Grade];
				string weavedName = weavedClothingConfig.Name.SetColor(color);
				this.propertyWeave.SetValue(weavedName);
			}
			return this.propertyWeave.gameObject.activeSelf;
		}

		// Token: 0x04004CEE RID: 19694
		[SerializeField]
		private Transform layoutSpecial;

		// Token: 0x04004CEF RID: 19695
		[SerializeField]
		private TooltipItemProperty propertyEatingRemainDuration;

		// Token: 0x04004CF0 RID: 19696
		[SerializeField]
		private TooltipItemProperty propertyWeave;

		// Token: 0x04004CF1 RID: 19697
		[SerializeField]
		private TooltipItemProperty propertyKillWug;
	}
}
