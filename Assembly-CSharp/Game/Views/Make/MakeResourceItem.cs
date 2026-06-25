using System;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Item;
using GameData.Domains.Map;
using TMPro;
using UnityEngine;

namespace Game.Views.Make
{
	// Token: 0x02000950 RID: 2384
	public class MakeResourceItem : MonoBehaviour
	{
		// Token: 0x06007098 RID: 28824 RVA: 0x0034211C File Offset: 0x0034031C
		public void Init(sbyte resourceType, Action<sbyte, int, bool> onCountChanged)
		{
			this._resourceType = resourceType;
			this._onCountChanged = onCountChanged;
			ResourceTypeItem config = ResourceType.Instance[this._resourceType];
			this.imageIcon.SetSprite(config.Icon, false, null);
			this.textName.SetText(config.Name, true);
			this.buttonAdd.ClearAndAddListener(new Action(this.OnClickButtonAdd));
			this.buttonMax.ClearAndAddListener(new Action(this.OnClickButtonMax));
			this.buttonReduce.ClearAndAddListener(new Action(this.OnClickButtonReduce));
			this.buttonMin.ClearAndAddListener(new Action(this.OnClickButtonMin));
		}

		// Token: 0x06007099 RID: 28825 RVA: 0x003421D0 File Offset: 0x003403D0
		public bool Refresh(short makeItemSubType, bool isMain, int needAmount, int curCount, int maxCount, int remainCount)
		{
			this._isMain = isMain;
			this._curCount = curCount;
			this._maxCount = maxCount;
			this._remainCount = remainCount;
			BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
			int ownedAmount = buildingModel.GetResourceCount(this._resourceType);
			bool isMeet = ownedAmount >= needAmount;
			string amountColor = isMeet ? "brightblue" : "brightred";
			string amountStr = CommonUtils.GetDisplayStringForNum(ownedAmount, 100000).SetColor(amountColor) + "/" + CommonUtils.GetDisplayStringForNum(needAmount, 100000);
			this.textAmount.SetText(amountStr, true);
			this.textCount.SetText(string.Format("{0}/{1}", curCount, maxCount), true);
			bool canAdd = curCount < maxCount && remainCount > 0;
			this.buttonAdd.interactable = (this.buttonMax.interactable = canAdd);
			bool canReduce = curCount > 0;
			this.buttonReduce.interactable = (this.buttonMin.interactable = canReduce);
			MakeItemSubTypeItem makeItemSubTypeConfig = MakeItemSubType.Instance[makeItemSubType];
			sbyte itemType = makeItemSubTypeConfig.Result.ItemType;
			sbyte resourceType = this._resourceType;
			if (!true)
			{
			}
			sbyte b;
			switch (resourceType)
			{
			case 1:
				b = makeItemSubTypeConfig.WoodEffect;
				break;
			case 2:
				b = makeItemSubTypeConfig.MetalEffect;
				break;
			case 3:
				b = makeItemSubTypeConfig.JadeEffect;
				break;
			case 4:
				b = makeItemSubTypeConfig.FabricEffect;
				break;
			default:
				b = -1;
				break;
			}
			if (!true)
			{
			}
			sbyte equipmentBonusType = b;
			bool hasEffect = equipmentBonusType > -1;
			this.rootEffect.SetActive(hasEffect);
			bool flag = hasEffect;
			if (flag)
			{
				ValueTuple<string, Sprite, string> equipmentBonusInfo = this.GetEquipmentBonusInfo(equipmentBonusType, itemType);
				string typeName = equipmentBonusInfo.Item1;
				Sprite typeIcon = equipmentBonusInfo.Item2;
				string tipContent = equipmentBonusInfo.Item3;
				MaterialResources curMaterialResources = default(MaterialResources);
				curMaterialResources.Initialize();
				curMaterialResources.Set((int)this._resourceType, (short)this._curCount);
				int percentage = ItemTemplateHelper.GetMaterialResourceBonusValuePercentage(itemType, makeItemSubTypeConfig.Result.TemplateId, equipmentBonusType, curMaterialResources);
				bool flag2 = equipmentBonusType == 4;
				if (flag2)
				{
					percentage = 170 - percentage;
				}
				string rateStr = string.Format("{0}%", percentage);
				this.textEffectName.text = typeName;
				this.textEffectValue.text = rateStr;
				this.imageEffect.sprite = typeIcon;
				TooltipInvoker tipDisplayer = this.tipEffect;
				tipDisplayer.Type = TipType.Simple;
				bool flag3 = tipDisplayer.PresetParam == null || tipDisplayer.PresetParam.Length == 0;
				if (flag3)
				{
					tipDisplayer.PresetParam = new string[2];
				}
				tipDisplayer.PresetParam[0] = typeName;
				tipDisplayer.PresetParam[1] = tipContent;
			}
			return isMeet;
		}

		// Token: 0x0600709A RID: 28826 RVA: 0x0034247E File Offset: 0x0034067E
		private void OnClickButtonAdd()
		{
			this.ChangeCount(true, false);
		}

		// Token: 0x0600709B RID: 28827 RVA: 0x00342489 File Offset: 0x00340689
		private void OnClickButtonMax()
		{
			this.ChangeCount(true, true);
		}

		// Token: 0x0600709C RID: 28828 RVA: 0x00342494 File Offset: 0x00340694
		private void OnClickButtonReduce()
		{
			this.ChangeCount(false, false);
		}

		// Token: 0x0600709D RID: 28829 RVA: 0x0034249F File Offset: 0x0034069F
		private void OnClickButtonMin()
		{
			this.ChangeCount(false, true);
		}

		// Token: 0x0600709E RID: 28830 RVA: 0x003424AC File Offset: 0x003406AC
		private void ChangeCount(bool isAdd, bool isAll)
		{
			if (isAll)
			{
				this._curCount = (isAdd ? (this._isMain ? (this._curCount + Math.Min(this._remainCount, this._maxCount - this._curCount)) : this._maxCount) : 0);
			}
			else
			{
				this._curCount = (isAdd ? (this._curCount + 1) : (this._curCount - 1));
			}
			this._onCountChanged(this._resourceType, this._curCount, isAdd);
		}

		// Token: 0x0600709F RID: 28831 RVA: 0x00342530 File Offset: 0x00340730
		private ValueTuple<string, Sprite, string> GetEquipmentBonusInfo(sbyte equipmentBonusType, sbyte itemType)
		{
			LanguageKey nameKey = LanguageKey.Invalid;
			string nameContent = string.Empty;
			bool isWeapon = itemType == 0;
			Sprite iconSprite = null;
			LanguageKey tipKey = LanguageKey.Invalid;
			string tipContent = string.Empty;
			switch (equipmentBonusType)
			{
			case 0:
				nameKey = (isWeapon ? LanguageKey.LK_WeaponEquipAttack : LanguageKey.LK_ArmorEquipAttack);
				iconSprite = (isWeapon ? this.spriteEffectAttackWeapon : this.spriteEffectAttackArmor);
				tipKey = (isWeapon ? LanguageKey.LK_WeaponEquipAttack_Tip : LanguageKey.LK_ArmorEquipAttack_Tip);
				break;
			case 1:
				nameKey = LanguageKey.LK_EquipDefense;
				iconSprite = this.spriteEffectDefend;
				tipKey = (isWeapon ? LanguageKey.LK_EquipDefense_Weapon_Tip : LanguageKey.LK_EquipDefense_Armor_Tip);
				break;
			case 2:
				nameKey = LanguageKey.LK_Combat_Attack_Value;
				iconSprite = this.spriteEffectPenetration;
				tipKey = LanguageKey.LK_Combat_Attack_Value_Tip;
				break;
			case 3:
				nameKey = LanguageKey.LK_Combat_Defend_Value;
				iconSprite = this.spriteEffectPenetrationResist;
				tipKey = LanguageKey.LK_Combat_Defend_Value_Tip;
				break;
			case 4:
				nameKey = LanguageKey.LK_Weight;
				iconSprite = this.spriteEffectWeight;
				tipKey = LanguageKey.LK_Weight_Tip;
				break;
			}
			nameContent = LocalStringManager.Get(nameKey);
			tipContent = LocalStringManager.Get(tipKey);
			return new ValueTuple<string, Sprite, string>(nameContent, iconSprite, tipContent);
		}

		// Token: 0x04005390 RID: 21392
		[SerializeField]
		private CImage imageIcon;

		// Token: 0x04005391 RID: 21393
		[SerializeField]
		private TextMeshProUGUI textName;

		// Token: 0x04005392 RID: 21394
		[SerializeField]
		private TextMeshProUGUI textAmount;

		// Token: 0x04005393 RID: 21395
		[SerializeField]
		private TextMeshProUGUI textCount;

		// Token: 0x04005394 RID: 21396
		[SerializeField]
		private CButton buttonAdd;

		// Token: 0x04005395 RID: 21397
		[SerializeField]
		private CButton buttonMax;

		// Token: 0x04005396 RID: 21398
		[SerializeField]
		private CButton buttonReduce;

		// Token: 0x04005397 RID: 21399
		[SerializeField]
		private CButton buttonMin;

		// Token: 0x04005398 RID: 21400
		[SerializeField]
		private GameObject rootEffect;

		// Token: 0x04005399 RID: 21401
		[SerializeField]
		private CImage imageEffect;

		// Token: 0x0400539A RID: 21402
		[SerializeField]
		private TextMeshProUGUI textEffectName;

		// Token: 0x0400539B RID: 21403
		[SerializeField]
		private TextMeshProUGUI textEffectValue;

		// Token: 0x0400539C RID: 21404
		[SerializeField]
		private TooltipInvoker tipEffect;

		// Token: 0x0400539D RID: 21405
		[SerializeField]
		private Sprite spriteEffectAttackArmor;

		// Token: 0x0400539E RID: 21406
		[SerializeField]
		private Sprite spriteEffectAttackWeapon;

		// Token: 0x0400539F RID: 21407
		[SerializeField]
		private Sprite spriteEffectDefend;

		// Token: 0x040053A0 RID: 21408
		[SerializeField]
		private Sprite spriteEffectPenetration;

		// Token: 0x040053A1 RID: 21409
		[SerializeField]
		private Sprite spriteEffectPenetrationResist;

		// Token: 0x040053A2 RID: 21410
		[SerializeField]
		private Sprite spriteEffectWeight;

		// Token: 0x040053A3 RID: 21411
		private sbyte _resourceType;

		// Token: 0x040053A4 RID: 21412
		private Action<sbyte, int, bool> _onCountChanged;

		// Token: 0x040053A5 RID: 21413
		private int _curCount;

		// Token: 0x040053A6 RID: 21414
		private int _maxCount;

		// Token: 0x040053A7 RID: 21415
		private int _remainCount;

		// Token: 0x040053A8 RID: 21416
		private bool _isMain;
	}
}
