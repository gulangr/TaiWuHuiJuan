using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using Game.Views.MouseTips.Common;
using Game.Views.MouseTips.Item.Common;
using GameData.Domains.Item.Display;
using UnityEngine;

namespace Game.Views.MouseTips.Item
{
	// Token: 0x020008A6 RID: 2214
	public class TooltipMaterial : TooltipItemBase
	{
		// Token: 0x17000C92 RID: 3218
		// (get) Token: 0x060069E5 RID: 27109 RVA: 0x0030CDDA File Offset: 0x0030AFDA
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060069E6 RID: 27110 RVA: 0x0030CDE0 File Offset: 0x0030AFE0
		protected override void Init(ArgumentBox argsBox)
		{
			argsBox.Get<ItemDisplayData>("ItemData", out this._itemData);
			argsBox.Get("TemplateDataOnly", out this._templateDataOnly);
			this._itemKey = this._itemData.RealKey;
			this.configData = Config.Material.Instance[this._itemKey.TemplateId];
			base.Init(argsBox);
			this.DisableDetail = !this.HasSelfPoison;
			base.PostInit();
			this.Refresh();
		}

		// Token: 0x060069E7 RID: 27111 RVA: 0x0030CE63 File Offset: 0x0030B063
		public override void SetNewData(ArgumentBox argsBox)
		{
			this.Init(argsBox);
			this.Refresh();
		}

		// Token: 0x060069E8 RID: 27112 RVA: 0x0030CE75 File Offset: 0x0030B075
		public override void Refresh()
		{
			base.Refresh();
			this.RefreshEatArea();
			this.RefreshRefine();
			UIElement element = this.Element;
			if (element != null)
			{
				element.ShowAfterRefresh();
			}
		}

		// Token: 0x060069E9 RID: 27113 RVA: 0x0030CEA0 File Offset: 0x0030B0A0
		private void RefreshEatArea()
		{
			bool showProfession = (!this._templateDataOnly && SingletonObject.getInstance<ProfessionModel>().IsSkillEquipped(54)) || this.ForceShowProfession;
			bool flag = !showProfession;
			if (flag)
			{
				this.rootEatArea.SetActive(false);
				this.rootCombat.SetActive(false);
			}
			else
			{
				List<TooltipItemProperty> list = this.layoutAddProperty.GetComponentsInChildren<TooltipItemProperty>(true).ToList<TooltipItemProperty>();
				int index = 0;
				for (ECharacterPropertyReferencedType type = ECharacterPropertyReferencedType.HitRateStrength; type <= ECharacterPropertyReferencedType.ResistOfIllusoryPoison; type++)
				{
					int baseValue = this.GetAddPropertyBaseValue(type);
					TooltipUtil.AppendAddProperty(ref index, list, (short)type, baseValue, baseValue, this.IsDetail, false, false, false, true, false, false, false);
				}
				for (int i = index; i < list.Count; i++)
				{
					list[i].gameObject.SetActive(false);
				}
				this.layoutAddProperty.gameObject.SetActive(index > 0);
				this.propertyLifespan.gameObject.SetActive(this.configData.BaseMaxHealthDelta > 0);
				this.propertyLifespan.SetValue(string.Format("+{0}", this.configData.BaseMaxHealthDelta).SetColor("brightblue"));
				this.propertyTime.gameObject.SetActive(this.configData.Duration > 0);
				this.propertyTime.SetValue(this.configData.Duration.ToString().SetColor("brightyellow"));
				bool isShow = this.layoutAddProperty.gameObject.activeSelf || this.propertyLifespan.gameObject.activeSelf;
				this.rootEatArea.SetActive(isShow);
				this.rootCombat.SetActive(isShow);
				bool flag2 = isShow;
				if (flag2)
				{
					this.propertyCost.SetValue(this.configData.ConsumedFeatureMedals.ToString().SetColor("brightyellow"));
				}
			}
		}

		// Token: 0x060069EA RID: 27114 RVA: 0x0030D094 File Offset: 0x0030B294
		private int GetAddPropertyBaseValue(ECharacterPropertyReferencedType type)
		{
			if (!true)
			{
			}
			short result;
			switch (type)
			{
			case ECharacterPropertyReferencedType.HitRateStrength:
				result = this.configData.HitRateStrength;
				break;
			case ECharacterPropertyReferencedType.HitRateTechnique:
				result = this.configData.HitRateTechnique;
				break;
			case ECharacterPropertyReferencedType.HitRateSpeed:
				result = this.configData.HitRateSpeed;
				break;
			case ECharacterPropertyReferencedType.HitRateMind:
				result = this.configData.HitRateMind;
				break;
			case ECharacterPropertyReferencedType.PenetrateOfOuter:
				result = this.configData.PenetrateOfOuter;
				break;
			case ECharacterPropertyReferencedType.PenetrateOfInner:
				result = this.configData.PenetrateOfInner;
				break;
			case ECharacterPropertyReferencedType.AvoidRateStrength:
				result = this.configData.AvoidRateStrength;
				break;
			case ECharacterPropertyReferencedType.AvoidRateTechnique:
				result = this.configData.AvoidRateTechnique;
				break;
			case ECharacterPropertyReferencedType.AvoidRateSpeed:
				result = this.configData.AvoidRateSpeed;
				break;
			case ECharacterPropertyReferencedType.AvoidRateMind:
				result = this.configData.AvoidRateMind;
				break;
			case ECharacterPropertyReferencedType.PenetrateResistOfOuter:
				result = this.configData.PenetrateResistOfOuter;
				break;
			case ECharacterPropertyReferencedType.PenetrateResistOfInner:
				result = this.configData.PenetrateResistOfInner;
				break;
			case ECharacterPropertyReferencedType.RecoveryOfStance:
				result = this.configData.RecoveryOfStance;
				break;
			case ECharacterPropertyReferencedType.RecoveryOfBreath:
				result = this.configData.RecoveryOfBreath;
				break;
			case ECharacterPropertyReferencedType.MoveSpeed:
				result = this.configData.MoveSpeed;
				break;
			case ECharacterPropertyReferencedType.RecoveryOfFlaw:
				result = this.configData.RecoveryOfFlaw;
				break;
			case ECharacterPropertyReferencedType.CastSpeed:
				result = this.configData.CastSpeed;
				break;
			case ECharacterPropertyReferencedType.RecoveryOfBlockedAcupoint:
				result = this.configData.RecoveryOfBlockedAcupoint;
				break;
			case ECharacterPropertyReferencedType.WeaponSwitchSpeed:
				result = this.configData.WeaponSwitchSpeed;
				break;
			case ECharacterPropertyReferencedType.AttackSpeed:
				result = this.configData.AttackSpeed;
				break;
			case ECharacterPropertyReferencedType.InnerRatio:
				result = this.configData.InnerRatio;
				break;
			case ECharacterPropertyReferencedType.RecoveryOfQiDisorder:
				result = this.configData.RecoveryOfQiDisorder;
				break;
			case ECharacterPropertyReferencedType.ResistOfHotPoison:
				result = this.configData.ResistOfHotPoison;
				break;
			case ECharacterPropertyReferencedType.ResistOfGloomyPoison:
				result = this.configData.ResistOfGloomyPoison;
				break;
			case ECharacterPropertyReferencedType.ResistOfColdPoison:
				result = this.configData.ResistOfColdPoison;
				break;
			case ECharacterPropertyReferencedType.ResistOfRedPoison:
				result = this.configData.ResistOfRedPoison;
				break;
			case ECharacterPropertyReferencedType.ResistOfRottenPoison:
				result = this.configData.ResistOfRottenPoison;
				break;
			case ECharacterPropertyReferencedType.ResistOfIllusoryPoison:
				result = this.configData.ResistOfIllusoryPoison;
				break;
			default:
				throw new ArgumentOutOfRangeException("type", type, null);
			}
			if (!true)
			{
			}
			return (int)result;
		}

		// Token: 0x060069EB RID: 27115 RVA: 0x0030D304 File Offset: 0x0030B504
		private void RefreshRefine()
		{
			bool hasRefiningEffect = this.configData.RefiningEffect > -1;
			this.rootRefine.SetActive(hasRefiningEffect);
			bool flag = !hasRefiningEffect;
			if (!flag)
			{
				RefiningEffectItem refiningEffectConfig = RefiningEffect.Instance[this.configData.RefiningEffect];
				ECharacterPropertyDisplayType displayType = TipsRefiningEffect.GetECharacterPropertyDisplayTypeByERefiningEffectType(refiningEffectConfig.WeaponType);
				CharacterPropertyDisplayItem displayTypeConfig = CharacterPropertyDisplay.Instance[displayType.ToInt()];
				string iconName = TipsRefiningEffect.GetRefinePropertyIconName(refiningEffectConfig.WeaponType, true);
				string propertyName = TipsRefiningEffect.GetRefinePropertyName(refiningEffectConfig.WeaponType);
				sbyte propertyValue = refiningEffectConfig.WeaponBonusValues[(int)this.configData.Grade];
				int unit = (displayType == ECharacterPropertyDisplayType.EquipmentMinAttackRange || displayType == ECharacterPropertyDisplayType.EquipmentMaxAttackRange) ? 10 : 1;
				this.propertyRefineWeapon.Set(iconName, propertyName, this.GetValue((int)propertyValue, displayTypeConfig.IsPercent, unit), true);
				ECharacterPropertyDisplayType displayType2 = TipsRefiningEffect.GetECharacterPropertyDisplayTypeByERefiningEffectType(refiningEffectConfig.ArmorType);
				CharacterPropertyDisplayItem displayTypeConfig2 = CharacterPropertyDisplay.Instance[displayType2.ToInt()];
				string iconName2 = TipsRefiningEffect.GetRefinePropertyIconName(refiningEffectConfig.ArmorType, true);
				string propertyName2 = TipsRefiningEffect.GetRefinePropertyName(refiningEffectConfig.ArmorType);
				sbyte propertyValue2 = refiningEffectConfig.ArmorBonusValues[(int)this.configData.Grade];
				this.propertyRefineArmor.Set(iconName2, propertyName2, this.GetValue((int)propertyValue2, displayTypeConfig2.IsPercent, 1), true);
				ECharacterPropertyDisplayType displayType3 = TipsRefiningEffect.GetECharacterPropertyDisplayTypeByERefiningEffectType(refiningEffectConfig.AccessoryType);
				CharacterPropertyDisplayItem displayTypeConfig3 = CharacterPropertyDisplay.Instance[displayType3.ToInt()];
				string iconName3 = TipsRefiningEffect.GetRefinePropertyIconName(refiningEffectConfig.AccessoryType, true);
				string propertyName3 = TipsRefiningEffect.GetRefinePropertyName(refiningEffectConfig.AccessoryType);
				sbyte propertyValue3 = refiningEffectConfig.AccessoryBonusValues[(int)this.configData.Grade];
				this.propertyRefineAccessory.Set(iconName3, propertyName3, this.GetValue((int)propertyValue3, displayTypeConfig3.IsPercent, 1), true);
			}
		}

		// Token: 0x060069EC RID: 27116 RVA: 0x0030D4CC File Offset: 0x0030B6CC
		private string GetValue(int value, bool isPercent, int unit = 1)
		{
			return TooltipItemRefiningEffect.GetValueStr(value, isPercent, unit);
		}

		// Token: 0x060069ED RID: 27117 RVA: 0x0030D4E8 File Offset: 0x0030B6E8
		protected override void InitItemDisableFunctionList()
		{
			base.InitItemDisableFunctionList();
			bool flag = !this.configData.Repairable;
			if (flag)
			{
				this._disableFunctionList.Add(ItemFunction.Repairable);
			}
			bool flag2 = !this.configData.Transferable;
			if (flag2)
			{
				this._disableFunctionList.Add(ItemFunction.Transferable);
			}
			bool flag3 = !this.configData.Poisonable;
			if (flag3)
			{
				this._disableFunctionList.Add(ItemFunction.Poisonable);
			}
			bool flag4 = !this.configData.Refinable;
			if (flag4)
			{
				this._disableFunctionList.Add(ItemFunction.Refinable);
			}
		}

		// Token: 0x04004C42 RID: 19522
		[Header("服食效果")]
		[SerializeField]
		private GameObject rootEatArea;

		// Token: 0x04004C43 RID: 19523
		[SerializeField]
		private Transform layoutAddProperty;

		// Token: 0x04004C44 RID: 19524
		[SerializeField]
		private TooltipItemProperty propertyTime;

		// Token: 0x04004C45 RID: 19525
		[SerializeField]
		private TooltipItemProperty propertyLifespan;

		// Token: 0x04004C46 RID: 19526
		[Header("战斗使用")]
		[SerializeField]
		private GameObject rootCombat;

		// Token: 0x04004C47 RID: 19527
		[SerializeField]
		private TooltipItemProperty propertyCost;

		// Token: 0x04004C48 RID: 19528
		[Header("精制效果")]
		[SerializeField]
		private GameObject rootRefine;

		// Token: 0x04004C49 RID: 19529
		[SerializeField]
		private TooltipItemProperty propertyRefineWeapon;

		// Token: 0x04004C4A RID: 19530
		[SerializeField]
		private TooltipItemProperty propertyRefineArmor;

		// Token: 0x04004C4B RID: 19531
		[SerializeField]
		private TooltipItemProperty propertyRefineAccessory;

		// Token: 0x04004C4C RID: 19532
		private MaterialItem configData;
	}
}
