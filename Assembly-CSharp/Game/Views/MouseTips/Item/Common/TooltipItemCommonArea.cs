using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips.Item.Common
{
	// Token: 0x020008AD RID: 2221
	public class TooltipItemCommonArea : MonoBehaviour
	{
		// Token: 0x06006A59 RID: 27225 RVA: 0x00311253 File Offset: 0x0030F453
		public void SetName(string gradeStr)
		{
			this.textName.SetText(gradeStr, true);
		}

		// Token: 0x06006A5A RID: 27226 RVA: 0x00311263 File Offset: 0x0030F463
		public void SetGrade(string gradeStr)
		{
			this.textGrade.SetText(gradeStr, true);
		}

		// Token: 0x06006A5B RID: 27227 RVA: 0x00311273 File Offset: 0x0030F473
		public string GetName()
		{
			return this.textName.text;
		}

		// Token: 0x06006A5C RID: 27228 RVA: 0x00311280 File Offset: 0x0030F480
		public void Refresh(ItemDisplayData itemData, bool isDetail)
		{
			this.Refresh(itemData.RealKey, false);
			this.RefreshDurability(itemData, isDetail);
			this.RefreshValue(itemData, isDetail);
			this.RefreshWeight(itemData, isDetail);
			this.rootOrganization.SetActive(false);
		}

		// Token: 0x06006A5D RID: 27229 RVA: 0x003112BC File Offset: 0x0030F4BC
		private void RefreshValue(ItemDisplayData itemData, bool isDetail)
		{
			ItemKey itemKey = itemData.RealKey;
			int baseValue = ItemTemplateHelper.GetBaseValue(itemKey.ItemType, itemKey.TemplateId);
			bool flag = !itemData.RealKey.IsValid();
			if (flag)
			{
				this.textValue.text = baseValue.ToString();
			}
			else
			{
				int finalValue = (int)itemData.Value;
				this.textValue.text = TooltipItemBase.GetBonusValue(baseValue, finalValue, isDetail, null, "", false);
			}
		}

		// Token: 0x06006A5E RID: 27230 RVA: 0x00311334 File Offset: 0x0030F534
		private void RefreshWeight(ItemDisplayData itemData, bool isDetail)
		{
			ItemKey itemKey = itemData.RealKey;
			int baseValue = ItemTemplateHelper.GetBaseWeight(itemKey.ItemType, itemKey.TemplateId);
			int finalValue = itemData.Weight;
			this.textWeight.text = TooltipItemBase.GetBonusValue(baseValue, finalValue, isDetail, new Func<int, string>(NumberFormatUtils.FormatItemWeight), "", false);
		}

		// Token: 0x06006A5F RID: 27231 RVA: 0x00311388 File Offset: 0x0030F588
		public void Refresh(ItemKey itemKey, bool isTemplate = true)
		{
			string itemIcon = ItemTemplateHelper.GetIcon(itemKey.ItemType, itemKey.TemplateId);
			this.imageIcon.SetSprite(itemIcon, false, null);
			string itemName = ItemTemplateHelper.GetName(itemKey.ItemType, itemKey.TemplateId);
			this.textName.text = itemName;
			sbyte itemGrade = ItemTemplateHelper.GetGrade(itemKey.ItemType, itemKey.TemplateId);
			string itemGradeStr = CommonUtils.GetFullGradeText(itemGrade, true);
			this.textGrade.text = itemGradeStr;
			sbyte backIndex = itemGrade;
			bool flag = itemKey.ItemType == 2;
			if (flag)
			{
				AccessoryItem config = Accessory.Instance[itemKey.TemplateId];
				bool flag2 = config != null && config.MysteryEffectId >= 0;
				if (flag2)
				{
					backIndex = 9;
				}
			}
			this.imageGradeBack.SetSprite("ui9_mousetip_base_level_{0}".GetFormat(backIndex), false, null);
			this.textType.text = CommonUtils.GetItemTypeName(itemKey.ItemType);
			this.RefreshMaterial(itemKey);
			if (isTemplate)
			{
				int itemValue = ItemTemplateHelper.GetBaseValue(itemKey.ItemType, itemKey.TemplateId);
				this.textValue.text = itemValue.ToString();
				int itemWeight = ItemTemplateHelper.GetBaseWeight(itemKey.ItemType, itemKey.TemplateId);
				this.textWeight.text = NumberFormatUtils.FormatItemWeight(itemWeight);
				this.rootDurability.SetActive(false);
			}
			this.textDesc.text = ItemTemplateHelper.GetDesc(itemKey.ItemType, itemKey.TemplateId).ColorReplace();
			this.textDesc.gameObject.SetActive(!this.textDesc.text.IsNullOrEmpty());
			this.textFunctionDesc.text = ItemTemplateHelper.GetFunctionDesc(itemKey.ItemType, itemKey.TemplateId).ColorReplace();
			this.textFunctionDesc.gameObject.SetActive(!this.textFunctionDesc.text.IsNullOrEmpty());
		}

		// Token: 0x06006A60 RID: 27232 RVA: 0x00311570 File Offset: 0x0030F770
		private void RefreshMaterial(ItemKey itemKey)
		{
			short makeItemSubType = ItemTemplateHelper.GetMakeItemSubType(itemKey.ItemType, itemKey.TemplateId);
			MakeItemSubTypeItem makeItemSubTypeConfig = MakeItemSubType.Instance[makeItemSubType];
			short itemSubType = ItemTemplateHelper.GetItemSubType(itemKey.ItemType, itemKey.TemplateId);
			bool flag = itemSubType >= 400 && itemSubType < 500;
			if (flag)
			{
				this.textMaterial.text = (CommonUtils.GetCarrierSubTypeName(itemSubType) ?? "");
			}
			else
			{
				this.textMaterial.text = ((makeItemSubTypeConfig != null) ? makeItemSubTypeConfig.FilterName : null) + CommonUtils.GetItemSubTypeName(itemSubType);
			}
		}

		// Token: 0x06006A61 RID: 27233 RVA: 0x0031160C File Offset: 0x0030F80C
		public void RefreshAmount(int amount)
		{
			bool isShowAmount = amount > 0;
			GameObject gameObject = this.rootAmount;
			if (gameObject != null)
			{
				gameObject.SetActive(isShowAmount);
			}
			bool flag = isShowAmount;
			if (flag)
			{
				this.textAmount.text = LanguageKey.LK_Item_Mousetip_TotalCount.TrFormat(amount);
			}
		}

		// Token: 0x06006A62 RID: 27234 RVA: 0x00311654 File Offset: 0x0030F854
		public void RefreshDurability(ItemDisplayData itemData, bool isDetail)
		{
			bool isShowDurability = itemData.MaxDurability > 0;
			this.rootDurability.SetActive(isShowDurability);
			bool flag = isShowDurability;
			if (flag)
			{
				string color = (itemData.Durability > itemData.MaxDurability / 2) ? string.Empty : "brightred";
				List<short> equipmentEffectIds = itemData.EquipmentEffectIds;
				int num;
				if (equipmentEffectIds == null)
				{
					num = 0;
				}
				else
				{
					num = equipmentEffectIds.Sum((short id) => EquipmentEffect.Instance[id].MaxDurabilityChange);
				}
				int maxDurabilityChange = num;
				int baseMax = (int)(itemData.MaxDurability * 100) / (100 + maxDurabilityChange);
				string maxStr = TooltipItemBase.GetBonusValue(baseMax, (int)itemData.MaxDurability, isDetail, null, "", false);
				this.textDurability.text = itemData.Durability.ToString().SetColor(color) + "/" + maxStr;
			}
		}

		// Token: 0x06006A63 RID: 27235 RVA: 0x00311724 File Offset: 0x0030F924
		public void Refresh(ItemDisplayData itemData, string name, string desc, string funcDesc, sbyte grade, string icon, string itemType, string value)
		{
			this.textName.text = name;
			string itemGradeStr = CommonUtils.GetFullGradeText(grade, true);
			this.textGrade.text = itemGradeStr;
			this.imageGradeBack.SetSprite("ui9_mousetip_base_level_{0}".GetFormat(grade), false, null);
			this.imageIcon.SetSprite(icon, false, null);
			this.RefreshWeight(itemData, false);
			this.textType.text = itemType;
			this.RefreshMaterial(itemData.RealKey);
			this.textValue.text = value;
			this.RefreshDurability(itemData, false);
			this.textDesc.text = desc;
			this.textDesc.gameObject.SetActive(true);
			this.textFunctionDesc.text = funcDesc;
			this.textFunctionDesc.gameObject.SetActive(!funcDesc.IsNullOrEmpty());
			this.rootOrganization.SetActive(false);
		}

		// Token: 0x06006A64 RID: 27236 RVA: 0x00311814 File Offset: 0x0030FA14
		public void RefreshBook(ItemDisplayData itemData)
		{
			this.Refresh(itemData.RealKey, false);
			this.RefreshDurability(itemData, false);
			this.RefreshValue(itemData, false);
			this.RefreshWeight(itemData, false);
			SkillBookItem configData = SkillBook.Instance[itemData.Key.TemplateId];
			bool isCombatSkill = configData.ItemSubType == 1001;
			string skillTypeName = isCombatSkill ? CombatSkillType.Instance[configData.CombatSkillType].Name : LifeSkillType.Instance[configData.LifeSkillType].Name;
			this.textMaterial.text = skillTypeName;
			this.rootOrganization.SetActive(isCombatSkill);
			bool flag = isCombatSkill;
			if (flag)
			{
				CombatSkillItem combatSkillConfig = CombatSkill.Instance[configData.CombatSkillTemplateId];
				OrganizationItem orgConfig = Organization.Instance[combatSkillConfig.SectId];
				this.textOrganization.text = orgConfig.Name;
			}
		}

		// Token: 0x04004CC5 RID: 19653
		[SerializeField]
		private CImage imageGradeBack;

		// Token: 0x04004CC6 RID: 19654
		[SerializeField]
		private CImage imageIcon;

		// Token: 0x04004CC7 RID: 19655
		[SerializeField]
		private TextMeshProUGUI textAmount;

		// Token: 0x04004CC8 RID: 19656
		[SerializeField]
		private GameObject rootAmount;

		// Token: 0x04004CC9 RID: 19657
		[SerializeField]
		private TextMeshProUGUI textName;

		// Token: 0x04004CCA RID: 19658
		[SerializeField]
		private TextMeshProUGUI textGrade;

		// Token: 0x04004CCB RID: 19659
		[SerializeField]
		private TextMeshProUGUI textMaterial;

		// Token: 0x04004CCC RID: 19660
		[SerializeField]
		private TextMeshProUGUI textType;

		// Token: 0x04004CCD RID: 19661
		[SerializeField]
		private TextMeshProUGUI textValue;

		// Token: 0x04004CCE RID: 19662
		[SerializeField]
		private TextMeshProUGUI textWeight;

		// Token: 0x04004CCF RID: 19663
		[SerializeField]
		private TextMeshProUGUI textDurability;

		// Token: 0x04004CD0 RID: 19664
		[SerializeField]
		private GameObject rootDurability;

		// Token: 0x04004CD1 RID: 19665
		[SerializeField]
		private TextMeshProUGUI textOrganization;

		// Token: 0x04004CD2 RID: 19666
		[SerializeField]
		private GameObject rootOrganization;

		// Token: 0x04004CD3 RID: 19667
		[SerializeField]
		private TextMeshProUGUI textDesc;

		// Token: 0x04004CD4 RID: 19668
		[SerializeField]
		private TextMeshProUGUI textFunctionDesc;
	}
}
