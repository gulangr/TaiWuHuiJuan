using System;
using Config;
using FrameWork;
using Game.Views.MouseTips.Item.Common;
using GameData.Domains.Global;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips.Item
{
	// Token: 0x02000898 RID: 2200
	public class TooltipArmor : TooltipItemBase
	{
		// Token: 0x17000C85 RID: 3205
		// (get) Token: 0x0600694C RID: 26956 RVA: 0x00306643 File Offset: 0x00304843
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

		// Token: 0x0600694D RID: 26957 RVA: 0x00306674 File Offset: 0x00304874
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
			this.configData = Armor.Instance[this._itemKey.TemplateId];
			base.Init(argsBox);
			base.PostInit();
			this.Refresh();
			GlobalDomainMethod.Call.InvokeGuidingTrigger(261);
		}

		// Token: 0x0600694E RID: 26958 RVA: 0x00306724 File Offset: 0x00304924
		public override void SetNewData(ArgumentBox argsBox)
		{
			this.Init(argsBox);
			this.Refresh();
		}

		// Token: 0x0600694F RID: 26959 RVA: 0x00306738 File Offset: 0x00304938
		public override void Refresh()
		{
			base.Refresh();
			this.RefreshArmorProperty();
			this.RefreshPenetrateResistProperty();
			this.RefreshAvoidProperty();
			bool templateDataOnly = this._templateDataOnly;
			if (templateDataOnly)
			{
				this.requirementArea.Refresh(!this._fixedPower, this._itemKey, false);
			}
			else
			{
				this.requirementArea.Refresh(!this._fixedPower, this._itemData);
			}
			this.RefreshDefendPropertyDetail();
			this.tooltipItemRefiningEffect.SetForArmor(this._itemData.RefiningEffects);
			UIElement element = this.Element;
			if (element != null)
			{
				element.ShowAfterRefresh();
			}
		}

		// Token: 0x06006950 RID: 26960 RVA: 0x003067D5 File Offset: 0x003049D5
		private void RefreshArmorProperty()
		{
			this.RefreshRequirementsPower();
			this.RefreshEquipmentAttack();
			this.RefreshEquipmentDefense();
			this.RefreshReduceInjury();
		}

		// Token: 0x06006951 RID: 26961 RVA: 0x003067F4 File Offset: 0x003049F4
		private void RefreshRequirementsPower()
		{
			this.textRequirementsPower.text = TooltipItemRequirementArea.GetPowerStr(this._templateDataOnly ? null : this._itemData);
		}

		// Token: 0x06006952 RID: 26962 RVA: 0x0030681C File Offset: 0x00304A1C
		private void RefreshEquipmentAttack()
		{
			string iconName = TipsRefiningEffect.GetRefinePropertyIconName(ERefiningEffectArmorType.EquipmentAttack, true);
			string propertyName = TipsRefiningEffect.GetRefinePropertyName(ERefiningEffectArmorType.EquipmentAttack);
			string content = base.GetBonusValue((int)this.configData.BaseEquipmentAttack, (int)this._itemData.EquipmentAttack, new Func<int, string>(this.EquipmentPropertyHandler), "", false);
			this.propertyEquipmentAttack.Set(iconName, propertyName, content, true);
		}

		// Token: 0x06006953 RID: 26963 RVA: 0x00306878 File Offset: 0x00304A78
		private void RefreshEquipmentDefense()
		{
			string iconName = TipsRefiningEffect.GetRefinePropertyIconName(ERefiningEffectArmorType.EquipmentDefense, true);
			string propertyName = TipsRefiningEffect.GetRefinePropertyName(ERefiningEffectArmorType.EquipmentDefense);
			string content = base.GetBonusValue((int)this.configData.BaseEquipmentDefense, (int)this._itemData.EquipmentDefense, new Func<int, string>(this.EquipmentPropertyHandler), "", false);
			this.propertyEquipmentDefense.Set(iconName, propertyName, content, true);
		}

		// Token: 0x06006954 RID: 26964 RVA: 0x003068D4 File Offset: 0x00304AD4
		private string EquipmentPropertyHandler(int value)
		{
			return ((float)value / 100f).ToString("F2");
		}

		// Token: 0x06006955 RID: 26965 RVA: 0x003068FC File Offset: 0x00304AFC
		private void RefreshReduceInjury()
		{
			this.propertyReduceOuterInjury.gameObject.SetActive(true);
			string outerContent = base.GetBonusValue((int)this.configData.BaseInjuryFactors.Outer, (int)this._itemData.InjuryFactors.Outer, null, "", true);
			this.propertyReduceOuterInjury.Set("ui9_mousetip_attribute_waishangjiangdi", LanguageKey.LK_ItemTips_Armor_Reduce_Outer_Injury.Tr(), outerContent, true);
			this.propertyReduceInnerInjury.gameObject.SetActive(true);
			string innerContent = base.GetBonusValue((int)this.configData.BaseInjuryFactors.Inner, (int)this._itemData.InjuryFactors.Inner, null, "", true);
			this.propertyReduceInnerInjury.Set("ui9_mousetip_attribute_neishangjiangdi", LanguageKey.LK_ItemTips_Armor_Reduce_Inner_Injury.Tr(), innerContent, true);
		}

		// Token: 0x06006956 RID: 26966 RVA: 0x003069C4 File Offset: 0x00304BC4
		private void RefreshPenetrateResistProperty()
		{
			int outerPenetrate = (int)(this._itemData.PenetrationInfo.Item1 * this._itemData.PowerInfo.Power / 100);
			int innerPenetrate = (int)(this._itemData.PenetrationInfo.Item2 * this._itemData.PowerInfo.Power / 100);
			int baseInnerPenetrate = (int)(this.configData.BasePenetrationResistFactors.Inner * this._itemData.PowerInfo.Power / 100);
			int baseOuterPenetrate = (int)(this.configData.BasePenetrationResistFactors.Outer * this._itemData.PowerInfo.Power / 100);
			this.propertyDefendOuter.Set("ui9_icon_attribute_defence_big_0", LanguageKey.LK_Penetrate_Resist_Outer.Tr(), TooltipItemBase.GetBonusValue(baseOuterPenetrate, outerPenetrate, false, null, "outterinjury", true), true);
			this.propertyDefendInner.Set("ui9_icon_attribute_defence_big_1", LanguageKey.LK_Penetrate_Resist_Inner.Tr(), TooltipItemBase.GetBonusValue(baseInnerPenetrate, innerPenetrate, false, null, "innerinjury", true), true);
		}

		// Token: 0x06006957 RID: 26967 RVA: 0x00306ABC File Offset: 0x00304CBC
		private unsafe void RefreshAvoidProperty()
		{
			int count = 0;
			for (sbyte hitType = 0; hitType < 4; hitType += 1)
			{
				TooltipItemProperty hitRefers = this.propertyAvoidTypes[(int)hitType];
				int hitValue = (int)(*(ref this._itemData.HitAvoidFactor.Items.FixedElementField + (IntPtr)hitType * 2));
				count += hitValue;
				bool isShow = hitValue != 0;
				DisableStyleRoot styleRoot = hitRefers.StyleRoot;
				if (styleRoot != null)
				{
					styleRoot.SetInteractable(isShow);
				}
				string iconName = "ui9_icon_attribute_avoid_big_" + hitType.ToString();
				LanguageKey nameKey = LanguageKey.LK_AvoidType_0 + (int)hitType;
				string color = (hitValue >= 100) ? "brightblue" : "brightred";
				string content = isShow ? string.Format("{0}%", 100 + hitValue).SetColor(color) : 0.ToString();
				hitRefers.Set(iconName, nameKey.Tr(), content, true);
			}
			this.propertyAvoidArea.gameObject.SetActive(count > 0);
		}

		// Token: 0x06006958 RID: 26968 RVA: 0x00306BB0 File Offset: 0x00304DB0
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

		// Token: 0x06006959 RID: 26969 RVA: 0x00306C41 File Offset: 0x00304E41
		private void RefreshDefendPropertyDetail()
		{
			this.propertyDefendDetail.SetValue(LanguageKey.LK_ItemTips_ArmorEffect_Tips.Tr());
		}

		// Token: 0x04004B90 RID: 19344
		[Header("护具属性")]
		[SerializeField]
		private TextMeshProUGUI textRequirementsPower;

		// Token: 0x04004B91 RID: 19345
		[SerializeField]
		private TooltipItemProperty propertyEquipmentAttack;

		// Token: 0x04004B92 RID: 19346
		[SerializeField]
		private TooltipItemProperty propertyEquipmentDefense;

		// Token: 0x04004B93 RID: 19347
		[SerializeField]
		private TooltipItemProperty propertyReduceOuterInjury;

		// Token: 0x04004B94 RID: 19348
		[SerializeField]
		private TooltipItemProperty propertyReduceInnerInjury;

		// Token: 0x04004B95 RID: 19349
		[Header("防御属性")]
		[SerializeField]
		private TooltipItemProperty propertyDefendOuter;

		// Token: 0x04004B96 RID: 19350
		[SerializeField]
		private TooltipItemProperty propertyDefendInner;

		// Token: 0x04004B97 RID: 19351
		[Header("化解属性")]
		[SerializeField]
		private TooltipItemProperty[] propertyAvoidTypes;

		// Token: 0x04004B98 RID: 19352
		[SerializeField]
		private RectTransform propertyAvoidArea;

		// Token: 0x04004B99 RID: 19353
		[Header("详细模式 发挥需求")]
		[SerializeField]
		private TooltipItemRequirementArea requirementArea;

		// Token: 0x04004B9A RID: 19354
		[SerializeField]
		private TooltipItemProperty propertyDefendDetail;

		// Token: 0x04004B9B RID: 19355
		[Header("详细模式 精制效果")]
		[SerializeField]
		private TooltipItemRefiningEffect tooltipItemRefiningEffect;

		// Token: 0x04004B9C RID: 19356
		private ArmorItem configData;
	}
}
