using System;
using Config;
using FrameWork;
using Game.Views.MouseTips.Item.Common;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using GameData.Domains.Item.Display;
using UnityEngine;

namespace Game.Views.MouseTips.Item
{
	// Token: 0x0200089C RID: 2204
	public class TooltipCarrier : TooltipItemBase
	{
		// Token: 0x17000C87 RID: 3207
		// (get) Token: 0x06006969 RID: 26985 RVA: 0x00307480 File Offset: 0x00305680
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

		// Token: 0x0600696A RID: 26986 RVA: 0x003074B4 File Offset: 0x003056B4
		protected override void Init(ArgumentBox argsBox)
		{
			argsBox.Get<ItemDisplayData>("ItemData", out this._itemData);
			argsBox.Get("IsInCompareUI", out this._isInCompareUI);
			argsBox.Get("TemplateDataOnly", out this._templateDataOnly);
			argsBox.Get<CarrierMaxProperty>("CarrierMaxProperty", out this._maxProperty);
			bool flag = !argsBox.Get("EquipSlot", out this._slot);
			if (flag)
			{
				this._slot = -1;
			}
			bool flag2 = !argsBox.Get("CharId", out this._charId);
			if (flag2)
			{
				this._charId = -1;
			}
			this._itemKey = this._itemData.RealKey;
			this.configData = Carrier.Instance[this._itemKey.TemplateId];
			this._hasTame = TooltipCarrierTameArea.HasTame(this._itemKey);
			base.Init(argsBox);
			GameObject gameObject = this.carrierNote;
			int slot = this._slot;
			gameObject.SetActive(slot == 11 || slot == 12);
			this.beastNote.SetActive(this._slot == 13);
			this.DisableDetail = !this._hasTame;
			base.PostInit();
			this.Refresh();
			GlobalDomainMethod.Call.InvokeGuidingTrigger(262);
		}

		// Token: 0x0600696B RID: 26987 RVA: 0x003075F1 File Offset: 0x003057F1
		public override void SetNewData(ArgumentBox argsBox)
		{
			this.Init(argsBox);
			this.Refresh();
		}

		// Token: 0x0600696C RID: 26988 RVA: 0x00307604 File Offset: 0x00305804
		public override void Refresh()
		{
			base.Refresh();
			this.RefreshCarrierProperty();
			this.RefreshTameDetail();
			this.tameArea.Refresh(this._itemData, this._slot, this._templateDataOnly, false, false);
			UIElement element = this.Element;
			if (element != null)
			{
				element.ShowAfterRefresh();
			}
		}

		// Token: 0x0600696D RID: 26989 RVA: 0x0030765C File Offset: 0x0030585C
		private void RefreshCarrierProperty()
		{
			this.propertyDropRate.gameObject.SetActive(this.configData.BaseDropRateBonus > 0);
			this.propertyDropRate.Set("", LanguageKey.LK_ItemTips_Carrier_Add_DropRate.Tr(), string.Format("+{0}%", this.configData.BaseDropRateBonus).SetColor("brightblue"), true);
			DisableStyleRoot styleRoot = this.propertyDropRate.StyleRoot;
			if (styleRoot != null)
			{
				styleRoot.SetInteractable(this._slot != 13 && (this._maxProperty == null || this._maxProperty.WorkingCarrierDropBonus <= (int)this.configData.BaseDropRateBonus));
			}
			this.propertyExploreBonusRate.gameObject.SetActive(this.configData.BaseExploreBonusRate > 0);
			this.propertyExploreBonusRate.Set("", LanguageKey.LK_ItemTips_Carrier_Add_ExploreBonusRate.Tr(), string.Format("+{0}%", this.configData.BaseExploreBonusRate).SetColor("brightblue"), true);
			DisableStyleRoot styleRoot2 = this.propertyExploreBonusRate.StyleRoot;
			if (styleRoot2 != null)
			{
				styleRoot2.SetInteractable(this._slot != 13 && (this._maxProperty == null || this._maxProperty.WorkingCarrierExploreBonusRate <= (int)this.configData.BaseExploreBonusRate));
			}
			this.propertyCaptureRate.gameObject.SetActive(this.configData.BaseCaptureRateBonus > 0);
			this.propertyCaptureRate.Set("", LanguageKey.LK_ItemTips_Carrier_Add_CaptureRate.Tr(), string.Format("+{0}%", this.configData.BaseCaptureRateBonus).SetColor("brightblue"), true);
			DisableStyleRoot styleRoot3 = this.propertyCaptureRate.StyleRoot;
			if (styleRoot3 != null)
			{
				styleRoot3.SetInteractable(this._slot != 13 && (this._maxProperty == null || this._maxProperty.WorkingCarrierCaptureRateBonus <= (int)this.configData.BaseCaptureRateBonus));
			}
			this.propertyCaptives.gameObject.SetActive(this.configData.BaseMaxKidnapSlotCountBonus > 0);
			this.propertyCaptives.Set("", LanguageKey.LK_Captives_Limit.Tr(), string.Format("+{0}", this.configData.BaseMaxKidnapSlotCountBonus).SetColor("brightblue"), true);
			DisableStyleRoot styleRoot4 = this.propertyCaptives.StyleRoot;
			if (styleRoot4 != null)
			{
				styleRoot4.SetInteractable(this._slot != 13 && (this._maxProperty == null || this._maxProperty.WorkingCarrierKidnapMaxSlotCount <= (int)this.configData.BaseMaxKidnapSlotCountBonus));
			}
			this.propertyInventory.gameObject.SetActive(this.configData.BaseMaxInventoryLoadBonus > 0);
			this.propertyInventory.Set("", LanguageKey.LK_ItemTips_Carrier_Add_Inventory.Tr(), string.Format("+{0:f1}", (float)this.configData.BaseMaxInventoryLoadBonus / 100f).SetColor("brightblue"), true);
			DisableStyleRoot styleRoot5 = this.propertyInventory.StyleRoot;
			if (styleRoot5 != null)
			{
				styleRoot5.SetInteractable(this._slot != 13 && (this._maxProperty == null || this._maxProperty.WorkingCarrierMaxInventoryLoadBonus <= (int)this.configData.BaseMaxInventoryLoadBonus));
			}
			this.propertyTravelSpeed.gameObject.SetActive(this.configData.BaseTravelTimeReduction > 0);
			this.propertyTravelSpeed.Set("", LanguageKey.LK_ItemTips_Carrier_Add_TravelSpeed.Tr(), string.Format("-{0}%", this.configData.BaseTravelTimeReduction).SetColor("brightblue"), true);
			DisableStyleRoot styleRoot6 = this.propertyTravelSpeed.StyleRoot;
			if (styleRoot6 != null)
			{
				styleRoot6.SetInteractable(this._slot != 13 && (this._maxProperty == null || this._maxProperty.WorkingCarrierTimeBonus <= (int)this.configData.BaseTravelTimeReduction));
			}
			bool isAnyShow = this.propertyDropRate.gameObject.activeSelf || this.propertyExploreBonusRate.gameObject.activeSelf || this.propertyCaptureRate.gameObject.activeSelf || this.propertyCaptives.gameObject.activeSelf || this.propertyInventory.gameObject.activeSelf || this.propertyTravelSpeed.gameObject.activeSelf;
			this.rootBaseProperty.SetActive(isAnyShow);
		}

		// Token: 0x0600696E RID: 26990 RVA: 0x00307AE4 File Offset: 0x00305CE4
		private void RefreshTameDetail()
		{
			this.propertyCombatState.gameObject.SetActive(this._hasTame);
			bool flag = !this._hasTame;
			if (!flag)
			{
				CombatStateItem combatStateConfig = CombatState.Instance[this.configData.CombatState];
				this.propertyCombatState.Set("", combatStateConfig.Name, combatStateConfig.Desc.ColorReplace(), true);
			}
		}

		// Token: 0x0600696F RID: 26991 RVA: 0x00307B54 File Offset: 0x00305D54
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

		// Token: 0x04004BB3 RID: 19379
		[Header("代步属性")]
		[SerializeField]
		private GameObject rootBaseProperty;

		// Token: 0x04004BB4 RID: 19380
		[SerializeField]
		private GameObject carrierNote;

		// Token: 0x04004BB5 RID: 19381
		[SerializeField]
		private GameObject beastNote;

		// Token: 0x04004BB6 RID: 19382
		[SerializeField]
		private TooltipItemProperty propertyDropRate;

		// Token: 0x04004BB7 RID: 19383
		[SerializeField]
		private TooltipItemProperty propertyExploreBonusRate;

		// Token: 0x04004BB8 RID: 19384
		[SerializeField]
		private TooltipItemProperty propertyCaptureRate;

		// Token: 0x04004BB9 RID: 19385
		[SerializeField]
		private TooltipItemProperty propertyCaptives;

		// Token: 0x04004BBA RID: 19386
		[SerializeField]
		private TooltipItemProperty propertyInventory;

		// Token: 0x04004BBB RID: 19387
		[SerializeField]
		private TooltipItemProperty propertyTravelSpeed;

		// Token: 0x04004BBC RID: 19388
		[SerializeField]
		private TooltipCarrierTameArea tameArea;

		// Token: 0x04004BBD RID: 19389
		[Header("详细模式")]
		[SerializeField]
		private TooltipItemProperty propertyCombatState;

		// Token: 0x04004BBE RID: 19390
		private CarrierItem configData;

		// Token: 0x04004BBF RID: 19391
		private int _slot;

		// Token: 0x04004BC0 RID: 19392
		private bool _hasTame;

		// Token: 0x04004BC1 RID: 19393
		private CarrierMaxProperty _maxProperty;
	}
}
