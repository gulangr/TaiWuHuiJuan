using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using Game.Views.MouseTips.Common;
using Game.Views.MouseTips.Item.Common;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips.Item
{
	// Token: 0x02000895 RID: 2197
	public class TooltipAccessory : TooltipItemBase
	{
		// Token: 0x17000C84 RID: 3204
		// (get) Token: 0x0600693E RID: 26942 RVA: 0x00305850 File Offset: 0x00303A50
		protected override bool CanStick
		{
			get
			{
				bool result;
				if (UIManager.Instance.CheckPopupElementIsInTop(UIElement.CharacterMenuEquip))
				{
					ItemDisplayData itemData = this._itemData;
					result = (itemData != null && itemData.UsingType == ItemDisplayData.ItemUsingType.Equiped);
				}
				else
				{
					result = true;
				}
				return result;
			}
		}

		// Token: 0x0600693F RID: 26943 RVA: 0x00305884 File Offset: 0x00303A84
		protected override void Init(ArgumentBox argsBox)
		{
			argsBox.Get<ItemDisplayData>("ItemData", out this._itemData);
			argsBox.Get("IsInCompareUI", out this._isInCompareUI);
			argsBox.Get("TemplateDataOnly", out this._templateDataOnly);
			bool flag = !argsBox.Get("CharId", out this._charId);
			if (flag)
			{
				this._charId = -1;
			}
			this._itemKey = this._itemData.RealKey;
			this.configData = Accessory.Instance[this._itemKey.TemplateId];
			this._isMystery = (this.configData.MysteryEffectId >= 0);
			base.Init(argsBox);
			ItemDisplayData itemData = this._itemData;
			bool isRefined = itemData != null && itemData.RefiningEffects.IsRefined;
			ItemDisplayData itemData2 = this._itemData;
			bool hasEffect = itemData2 != null && itemData2.HaveEquipmentEffect();
			this.DisableDetail = (!this.HasSelfPoison && !this.HasAttachedPoison && !isRefined && !hasEffect && !this._isMystery);
			base.PostInit();
			this.Refresh();
		}

		// Token: 0x06006940 RID: 26944 RVA: 0x00305993 File Offset: 0x00303B93
		public override void SetNewData(ArgumentBox argsBox)
		{
			this.Init(argsBox);
			this.Refresh();
		}

		// Token: 0x06006941 RID: 26945 RVA: 0x003059A8 File Offset: 0x00303BA8
		public override void Refresh()
		{
			base.Refresh();
			this.RefreshBaseProperty();
			this.RefreshAddProperty();
			this.UpdateMystery();
			bool templateDataOnly = this._templateDataOnly;
			if (templateDataOnly)
			{
				this.requirementArea.Refresh(this._isMystery, this._itemKey, this._isMystery);
			}
			else
			{
				this.requirementArea.Refresh(this._isMystery, this._itemData);
			}
			GameObject gameObject = this.rootDetailMain;
			bool active;
			if (!this._isMystery)
			{
				ItemDisplayData itemData = this._itemData;
				active = (itemData != null && itemData.RefiningEffects.IsRefined);
			}
			else
			{
				active = true;
			}
			gameObject.SetActive(active);
			this.tooltipItemRefiningEffect.SetForAccessory(this._itemData.RefiningEffects);
			UIElement element = this.Element;
			if (element != null)
			{
				element.ShowAfterRefresh();
			}
		}

		// Token: 0x06006942 RID: 26946 RVA: 0x00305A6C File Offset: 0x00303C6C
		private void RefreshBaseProperty()
		{
			this.propertyDropRate.gameObject.SetActive(this.configData.DropRateBonus > 0);
			this.propertyDropRate.Set("", LanguageKey.LK_ItemTips_Carrier_Add_DropRate.Tr(), string.Format("+{0}%", this.configData.DropRateBonus), true);
			this.propertyExploreBonusRate.gameObject.SetActive(this.configData.BaseExploreBonusRate > 0);
			this.propertyExploreBonusRate.Set("", LanguageKey.LK_ItemTips_Carrier_Add_ExploreBonusRate.Tr(), string.Format("+{0}%", this.configData.BaseExploreBonusRate), true);
			this.propertyCaptureRate.gameObject.SetActive(this.configData.BaseCaptureRateBonus > 0);
			this.propertyCaptureRate.Set("", LanguageKey.LK_ItemTips_Carrier_Add_CaptureRate.Tr(), string.Format("+{0}%", this.configData.BaseCaptureRateBonus), true);
			this.propertyInventory.gameObject.SetActive(this.configData.MaxInventoryLoadBonus > 0);
			short maxInventoryLoadBonus = this._templateDataOnly ? this.configData.MaxInventoryLoadBonus : this._itemData.MaxInventoryLoadBonus;
			string propertyInventoryValue = base.GetBonusValue((int)this.configData.MaxInventoryLoadBonus, (int)maxInventoryLoadBonus, (int v) => string.Format("{0:f1}", (float)v / 100f), "", false);
			this.propertyInventory.Set("", LanguageKey.LK_ItemTips_Carrier_Add_Inventory.Tr(), propertyInventoryValue, true);
			this.propertyCombatSkillPower.gameObject.SetActive(this.configData.BonusCombatSkillSect >= 0);
			bool activeSelf = this.propertyCombatSkillPower.gameObject.activeSelf;
			if (activeSelf)
			{
				string sectName = Organization.Instance[this.configData.BonusCombatSkillSect].Name;
				string propertyCombatSkillPowerTitle = LanguageKey.LK_ItemTips_Organization_Add_Combat_Skill_Power.TrFormat(sectName);
				this.propertyCombatSkillPower.Set("", propertyCombatSkillPowerTitle, string.Format("+{0}", GlobalConfig.Instance.SectAccessoryBonusCombatSkillPower), true);
			}
			this.propertyCombatSkillPowerLimit.gameObject.SetActive(this.configData.CombatSkillAddMaxPower > 0);
			this.propertyCombatSkillPowerLimit.Set("", LanguageKey.LK_ItemTips_Accessory_CombatSkillPowerLimit.Tr(), string.Format("+{0}", this.configData.CombatSkillAddMaxPower), true);
			bool isAnyShow = this.propertyDropRate.gameObject.activeSelf || this.propertyExploreBonusRate.gameObject.activeSelf || this.propertyCaptureRate.gameObject.activeSelf || this.propertyInventory.gameObject.activeSelf || this.propertyCombatSkillPower.gameObject.activeSelf || this.propertyCombatSkillPowerLimit.gameObject.activeSelf;
			this.rootBaseProperty.SetActive(isAnyShow);
		}

		// Token: 0x06006943 RID: 26947 RVA: 0x00305D70 File Offset: 0x00303F70
		private void RefreshAddProperty()
		{
			List<TooltipItemProperty> list = this.layoutAddProperty.GetComponentsInChildren<TooltipItemProperty>(true).ToList<TooltipItemProperty>();
			int index = 0;
			for (ECharacterPropertyReferencedType type = ECharacterPropertyReferencedType.Strength; type <= ECharacterPropertyReferencedType.ResistOfIllusoryPoison; type++)
			{
				int baseValue = this.GetAddPropertyBaseValue(type);
				TooltipUtil.AppendAddProperty(ref index, list, (short)type, baseValue, baseValue, this.IsDetail, false, false, false, true, true, false, false);
			}
			for (int i = index; i < list.Count; i++)
			{
				list[i].gameObject.SetActive(false);
			}
			this.layoutAddProperty.gameObject.SetActive(index > 0);
		}

		// Token: 0x06006944 RID: 26948 RVA: 0x00305E10 File Offset: 0x00304010
		private int GetAddPropertyBaseValue(ECharacterPropertyReferencedType type)
		{
			if (!true)
			{
			}
			short result;
			switch (type)
			{
			case ECharacterPropertyReferencedType.Strength:
				result = this.configData.Strength;
				break;
			case ECharacterPropertyReferencedType.Dexterity:
				result = this.configData.Dexterity;
				break;
			case ECharacterPropertyReferencedType.Concentration:
				result = this.configData.Concentration;
				break;
			case ECharacterPropertyReferencedType.Vitality:
				result = this.configData.Vitality;
				break;
			case ECharacterPropertyReferencedType.Energy:
				result = this.configData.Energy;
				break;
			case ECharacterPropertyReferencedType.Intelligence:
				result = this.configData.Intelligence;
				break;
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

		// Token: 0x06006945 RID: 26949 RVA: 0x003060FC File Offset: 0x003042FC
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

		// Token: 0x06006946 RID: 26950 RVA: 0x00306190 File Offset: 0x00304390
		private void UpdateMystery()
		{
			this.rootMystery.SetActive(this._isMystery);
			this.rootDetailMystery.SetActive(this._isMystery);
			this.rootDetailCompatibility.SetActive(this._isMystery);
			bool flag = !this._isMystery;
			if (!flag)
			{
				short requirementsPower = this._itemData.PowerInfo.RequirementsPower;
				string valueText = TooltipItemRequirementArea.GetPowerStr(this._templateDataOnly ? null : this._itemData);
				this.textRequirementsPower.text = valueText;
				MysteryEffectItem config = MysteryEffect.Instance[this.configData.MysteryEffectId];
				this.containerMystery.Rebuild<TooltipAccessoryMystery>(config.BonusValues.Count, delegate(TooltipAccessoryMystery mysteryProperty, int index)
				{
					mysteryProperty.Refresh(config, index, requirementsPower, this._templateDataOnly);
				});
				this.containerMysterySpecial.Rebuild<TooltipAccessoryMysterySpecial>(config.BonusEffects.Count, delegate(TooltipAccessoryMysterySpecial mysterySpecialEffect, int index)
				{
					mysterySpecialEffect.Refresh(config, index, config.BonusValues.Count, requirementsPower, this._templateDataOnly);
				});
				string nameStr = ItemTemplateHelper.GetName(this._itemKey.ItemType, this._itemKey.TemplateId).SetColor("mystery");
				this.commonArea.SetName(nameStr);
				string gradeStr = (LanguageKey.LK_MouseTip_Mystery_Short_Key.Tr() + LanguageKey.LK_Item_Grade.Tr()).SetColor("mystery");
				this.commonArea.SetGrade(gradeStr);
			}
		}

		// Token: 0x04004B74 RID: 19316
		[Header("基本属性")]
		[SerializeField]
		private GameObject rootBaseProperty;

		// Token: 0x04004B75 RID: 19317
		[SerializeField]
		private TooltipItemProperty propertyDropRate;

		// Token: 0x04004B76 RID: 19318
		[SerializeField]
		private TooltipItemProperty propertyExploreBonusRate;

		// Token: 0x04004B77 RID: 19319
		[SerializeField]
		private TooltipItemProperty propertyCaptureRate;

		// Token: 0x04004B78 RID: 19320
		[SerializeField]
		private TooltipItemProperty propertyInventory;

		// Token: 0x04004B79 RID: 19321
		[SerializeField]
		private TooltipItemProperty propertyCombatSkillPower;

		// Token: 0x04004B7A RID: 19322
		[SerializeField]
		private TooltipItemProperty propertyCombatSkillPowerLimit;

		// Token: 0x04004B7B RID: 19323
		[Header("加成属性")]
		[SerializeField]
		private Transform layoutAddProperty;

		// Token: 0x04004B7C RID: 19324
		[Header("玄字效果")]
		[SerializeField]
		private GameObject rootMystery;

		// Token: 0x04004B7D RID: 19325
		[SerializeField]
		private TextMeshProUGUI textRequirementsPower;

		// Token: 0x04004B7E RID: 19326
		[SerializeField]
		private TemplatedContainerAssemblyNew containerMystery;

		// Token: 0x04004B7F RID: 19327
		[SerializeField]
		private TemplatedContainerAssemblyNew containerMysterySpecial;

		// Token: 0x04004B80 RID: 19328
		[SerializeField]
		private TooltipItemRequirementArea requirementArea;

		// Token: 0x04004B81 RID: 19329
		[SerializeField]
		private GameObject rootDetailMain;

		// Token: 0x04004B82 RID: 19330
		[Header("详细模式 精制效果")]
		[SerializeField]
		private TooltipItemRefiningEffect tooltipItemRefiningEffect;

		// Token: 0x04004B83 RID: 19331
		[Header("详细模式 玄字效果")]
		[SerializeField]
		private GameObject rootDetailMystery;

		// Token: 0x04004B84 RID: 19332
		[SerializeField]
		private GameObject rootDetailCompatibility;

		// Token: 0x04004B85 RID: 19333
		private AccessoryItem configData;

		// Token: 0x04004B86 RID: 19334
		private bool _isMystery;
	}
}
