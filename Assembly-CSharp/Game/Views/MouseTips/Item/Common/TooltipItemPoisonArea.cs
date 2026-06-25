using System;
using Config;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using UnityEngine;

namespace Game.Views.MouseTips.Item.Common
{
	// Token: 0x020008B0 RID: 2224
	public class TooltipItemPoisonArea : MonoBehaviour
	{
		// Token: 0x06006A69 RID: 27241 RVA: 0x00311AB3 File Offset: 0x0030FCB3
		public static string GetPoisonTypeIcon(sbyte type)
		{
			return string.Format("{0}{1}", "ui9_icon_poison_0_", type);
		}

		// Token: 0x06006A6A RID: 27242 RVA: 0x00311ACA File Offset: 0x0030FCCA
		public static string GetPoisonLevelIcon(int categoryIconIndex)
		{
			return string.Format("{0}{1}", "ui9_icon_venom_", categoryIconIndex);
		}

		// Token: 0x06006A6B RID: 27243 RVA: 0x00311AE4 File Offset: 0x0030FCE4
		private unsafe bool RefreshSelfPoisons(PoisonsAndLevels poisonsAndLevels)
		{
			bool isNonZero = poisonsAndLevels.IsNonZero();
			this.rootSelfPoison.gameObject.SetActive(isNonZero);
			bool flag = isNonZero;
			if (flag)
			{
				for (sbyte order = 0; order < 6; order += 1)
				{
					sbyte type = PoisonType.GetTypeBySortingOrder(order);
					PoisonItem poisonTypeConfig = Poison.Instance[type];
					TooltipItemPropertyPoison poisonItem = this.propertySelfPoisons[(int)type];
					short poisonValue = *(ref poisonsAndLevels.Values.FixedElementField + (IntPtr)type * 2);
					sbyte poisonLevel = *(ref poisonsAndLevels.Levels.FixedElementField + type);
					bool show = poisonValue > 0;
					poisonItem.gameObject.SetActive(show);
					bool flag2 = show;
					if (flag2)
					{
						string typeIcon = TooltipItemPoisonArea.GetPoisonTypeIcon(type);
						int levelIconIndex = TooltipCombatSkill.GetPoisonLevelIconIndex(poisonLevel);
						string levelIcon = TooltipItemPoisonArea.GetPoisonLevelIcon(levelIconIndex);
						string value = poisonValue.ToString().SetColor("brightblue");
						poisonItem.Set(poisonTypeConfig.Name, value, false, typeIcon, levelIcon, (int)poisonLevel);
					}
				}
			}
			return isNonZero;
		}

		// Token: 0x06006A6C RID: 27244 RVA: 0x00311BE0 File Offset: 0x0030FDE0
		private unsafe bool RefreshAttachedPoisons(ItemDisplayData itemData)
		{
			bool hasPoisonEffect = itemData.HasAnyPoison && itemData.PoisonIsIdentified;
			this.rootAttachedPoison.gameObject.SetActive(hasPoisonEffect);
			bool flag = !hasPoisonEffect;
			bool result;
			if (flag)
			{
				this.propertyMixedPoison.gameObject.SetActive(false);
				result = false;
			}
			else
			{
				FullPoisonEffects poisonEffects = itemData.PoisonEffects;
				for (sbyte order = 0; order < 6; order += 1)
				{
					sbyte type = PoisonType.GetTypeBySortingOrder(order);
					PoisonItem poisonTypeConfig = Poison.Instance[type];
					TooltipItemPropertyPoison poisonItem = this.propertyAttachedPoisons[(int)type];
					PoisonsAndLevels poisonsAndLevels = poisonEffects.GetAllPoisonsAndLevels();
					short poisonValue = *(ref poisonsAndLevels.Values.FixedElementField + (IntPtr)type * 2);
					sbyte poisonLevel = *(ref poisonsAndLevels.Levels.FixedElementField + type);
					bool show = poisonValue > 0;
					poisonItem.gameObject.SetActive(show);
					bool flag2 = show;
					if (flag2)
					{
						string typeIcon = TooltipItemPoisonArea.GetPoisonTypeIcon(type);
						int levelIconIndex = TooltipCombatSkill.GetPoisonLevelIconIndex(poisonLevel);
						string levelIcon = TooltipItemPoisonArea.GetPoisonLevelIcon(levelIconIndex);
						string value = poisonValue.ToString().SetColor("brightblue");
						bool isCondensed = poisonEffects.IsCondensed && poisonEffects.PoisonSlotList.Exists((PoisonSlot s) => s.IsCondensed && s.MedicineConfig.PoisonType == type);
						poisonItem.Set(poisonTypeConfig.Name, value, isCondensed, typeIcon, levelIcon, (int)poisonLevel);
					}
				}
				this.propertyMixedPoison.gameObject.SetActive(poisonEffects.IsThreePoisonsMix());
				bool activeSelf = this.propertyMixedPoison.gameObject.activeSelf;
				if (activeSelf)
				{
					MedicineItem medicineConfig = Medicine.Instance[poisonEffects.GetMedicineTemplateId()];
					this.propertyMixedPoison.SetValue(medicineConfig.Name.SetColor("additionalpoison"));
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06006A6D RID: 27245 RVA: 0x00311DC0 File Offset: 0x0030FFC0
		private bool RefreshIdentify(ItemDisplayData itemData)
		{
			bool showPoisonIdentifyTips = itemData.PoisonIsIdentified && !itemData.HasAnyPoison;
			this.propertyIdentify.gameObject.SetActive(showPoisonIdentifyTips);
			this.propertyIdentify.SetValue(LanguageKey.LK_ItemTips_No_Poison.Tr().SetColor("brightblue"));
			return showPoisonIdentifyTips;
		}

		// Token: 0x06006A6E RID: 27246 RVA: 0x00311E1C File Offset: 0x0031001C
		public void Refresh(PoisonsAndLevels poisonsAndLevels, ItemDisplayData itemData)
		{
			bool hasInnatePoison = this.RefreshSelfPoisons(poisonsAndLevels);
			bool hasPoisonEffect = this.RefreshAttachedPoisons(itemData);
			bool showPoisonIdentifyTips = this.RefreshIdentify(itemData);
			bool isShow = hasInnatePoison || hasPoisonEffect || showPoisonIdentifyTips;
			base.gameObject.SetActive(isShow);
		}

		// Token: 0x04004CDD RID: 19677
		[SerializeField]
		private Transform rootSelfPoison;

		// Token: 0x04004CDE RID: 19678
		[SerializeField]
		private TooltipItemPropertyPoison[] propertySelfPoisons;

		// Token: 0x04004CDF RID: 19679
		[SerializeField]
		private Transform rootAttachedPoison;

		// Token: 0x04004CE0 RID: 19680
		[SerializeField]
		private TooltipItemPropertyPoison[] propertyAttachedPoisons;

		// Token: 0x04004CE1 RID: 19681
		[SerializeField]
		private TooltipItemProperty propertyMixedPoison;

		// Token: 0x04004CE2 RID: 19682
		[SerializeField]
		private TooltipItemProperty propertyIdentify;
	}
}
