using System;
using Config;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using UnityEngine;

namespace Game.Views.MouseTips.Item.Common
{
	// Token: 0x020008AB RID: 2219
	public class TooltipCarrierTameArea : MonoBehaviour
	{
		// Token: 0x06006A3B RID: 27195 RVA: 0x003105F4 File Offset: 0x0030E7F4
		public void Refresh(ItemDisplayData itemData, int slot, bool templateDataOnly, bool isLoong, bool isYouth)
		{
			bool hasTame = isYouth || TooltipCarrierTameArea.HasTame(itemData.RealKey);
			base.gameObject.SetActive(hasTame);
			bool flag = !hasTame;
			if (!flag)
			{
				this.RefreshTame(itemData, templateDataOnly);
				this.RefreshDrive(itemData, isLoong, isYouth);
				this.RefreshBeastEquip(slot);
			}
		}

		// Token: 0x06006A3C RID: 27196 RVA: 0x0031064C File Offset: 0x0030E84C
		private void RefreshTame(ItemDisplayData itemData, bool templateDataOnly)
		{
			bool flag = itemData.JiaoLoongDisplayData != null;
			if (flag)
			{
				int maxTame = itemData.JiaoLoongDisplayData.MaxTamePoint;
				string color = (itemData.CarrierTamePoint >= maxTame) ? "brightblue" : "brightred";
				string tameStr = string.Format("{0}/{1}", itemData.JiaoLoongDisplayData.TamePoint.ToString().SetColor(color), maxTame);
				this.propertyTame.SetValue(tameStr);
			}
			else
			{
				int maxTame2 = 100;
				string color2 = (itemData.CarrierTamePoint >= maxTame2) ? "brightblue" : "brightred";
				string tame = templateDataOnly ? "-" : itemData.CarrierTamePoint.ToString().SetColor(color2);
				string tameStr2 = string.Format("{0}/{1}", tame, maxTame2);
				this.propertyTame.SetValue(tameStr2);
			}
		}

		// Token: 0x06006A3D RID: 27197 RVA: 0x00310724 File Offset: 0x0030E924
		private void RefreshDrive(ItemDisplayData itemData, bool isLoong, bool isYouth)
		{
			bool isShow = !isYouth;
			this.propertyDriveBeast.gameObject.SetActive(isShow);
			bool flag = !isShow;
			if (!flag)
			{
				CarrierItem carrierItem = Carrier.Instance[itemData.RealKey.TemplateId];
				CombatStateItem combatStateConfig = CombatState.Instance[carrierItem.CombatState];
				LanguageKey tipKey = isLoong ? LanguageKey.LK_MouseTips_CarrierDriveBeastTip : LanguageKey.LK_MouseTips_Jiao_CarrierDriveBeastTip;
				this.propertyDriveBeast.SetValue(tipKey.TrFormat(carrierItem.Name, combatStateConfig.Name));
			}
		}

		// Token: 0x06006A3E RID: 27198 RVA: 0x003107AC File Offset: 0x0030E9AC
		private void RefreshBeastEquip(int slot)
		{
			if (!true)
			{
			}
			string text;
			if (slot != 12)
			{
				if (slot != 13)
				{
					text = "";
				}
				else
				{
					text = LanguageKey.LK_MouseTip_Carrier_Equipped_In_Beast_Slot.Tr();
				}
			}
			else
			{
				text = LanguageKey.LK_MouseTip_Carrier_Equipped_In_Livestock_Slot.Tr();
			}
			if (!true)
			{
			}
			string beastEquipStr = text;
			this.propertyBeastEquip.gameObject.SetActive(!beastEquipStr.IsNullOrEmpty());
			this.propertyBeastEquip.SetValue(beastEquipStr);
		}

		// Token: 0x06006A3F RID: 27199 RVA: 0x0031081C File Offset: 0x0030EA1C
		public static bool HasTame(ItemKey itemKey)
		{
			CarrierItem configData = Carrier.Instance[itemKey.TemplateId];
			return ItemTemplateHelper.HasCarrierTame(configData.ItemType, configData.TemplateId);
		}

		// Token: 0x04004CA3 RID: 19619
		[SerializeField]
		private TooltipItemProperty propertyTame;

		// Token: 0x04004CA4 RID: 19620
		[SerializeField]
		private TooltipItemProperty propertyDriveBeast;

		// Token: 0x04004CA5 RID: 19621
		[SerializeField]
		private TooltipItemProperty propertyBeastEquip;
	}
}
