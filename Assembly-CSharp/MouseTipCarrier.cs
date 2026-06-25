using System;
using System.Text;
using Config;
using FrameWork;
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x0200027C RID: 636
public class MouseTipCarrier : MouseTipItem
{
	// Token: 0x17000486 RID: 1158
	// (get) Token: 0x0600293C RID: 10556 RVA: 0x00133B7B File Offset: 0x00131D7B
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

	// Token: 0x0600293D RID: 10557 RVA: 0x00133BAC File Offset: 0x00131DAC
	protected override void Init(ArgumentBox argsBox)
	{
		base.Init(argsBox);
		ItemDisplayData itemData;
		argsBox.Get<ItemDisplayData>("ItemData", out itemData);
		bool templateDataOnly;
		argsBox.Get("TemplateDataOnly", out templateDataOnly);
		argsBox.Get("IsInCompareUI", out this._isInCompareUI);
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
		this._itemData = itemData;
		CarrierItem configData = Carrier.Instance[itemData.Key.TemplateId];
		TextMeshProUGUI currDurabilityYellow = base.CGet<TextMeshProUGUI>("CurrDurabilityYellow");
		TextMeshProUGUI currDurabilityRed = base.CGet<TextMeshProUGUI>("CurrDurabilityRed");
		base.CGet<TextMeshProUGUI>("Name").text = configData.Name;
		base.CGet<CImage>("GradeBack").SetSprite(ItemView.GetGradeIcon(configData.Grade), false, null);
		base.CGet<TextMeshProUGUI>("GradeName").text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", configData.Grade));
		base.CGet<TextMeshProUGUI>("Grade").text = (LocalStringManager.Get(string.Format("LK_Num_{0}", (int)(9 - configData.Grade))) + LocalStringManager.Get(LanguageKey.LK_Item_Grade)).SetColor(Colors.Instance.GradeColors[(int)configData.Grade]);
		base.CGet<TextMeshProUGUI>("Value").text = (templateDataOnly ? configData.BaseValue.ToString() : itemData.Value.ToString());
		base.CGet<GameObject>("Durability").SetActive(!templateDataOnly);
		base.CGet<GameObject>("Material").SetActive(!templateDataOnly);
		base.CGet<CImage>("ItemIcon").SetSprite(configData.Icon, false, null);
		base.SetItemDesc(configData.Desc, itemData.LoveTokenDataItem);
		bool hasHalfDurability = itemData.Durability > itemData.MaxDurability / 2;
		base.CGet<TextMeshProUGUI>("SubType").text = LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", configData.ItemSubType));
		base.CGet<TextMeshProUGUI>("Weight").text = NumberFormatUtils.FormatItemWeight(itemData.Weight);
		currDurabilityYellow.gameObject.SetActive(hasHalfDurability);
		currDurabilityRed.gameObject.SetActive(!hasHalfDurability);
		(hasHalfDurability ? currDurabilityYellow : currDurabilityRed).text = itemData.Durability.ToString();
		base.CGet<TextMeshProUGUI>("MaxDurability").text = string.Format("/{0}", itemData.MaxDurability);
		base.CGet<GameObject>("DropRate").SetActive(configData.BaseDropRateBonus > 0);
		bool flag3 = configData.BaseDropRateBonus > 0;
		if (flag3)
		{
			base.CGet<TextMeshProUGUI>("AddDropRate").text = string.Format("+{0}%", configData.BaseDropRateBonus);
		}
		base.CGet<GameObject>("CaptureRate").SetActive(configData.BaseCaptureRateBonus > 0);
		bool flag4 = configData.BaseCaptureRateBonus > 0;
		if (flag4)
		{
			base.CGet<TextMeshProUGUI>("AddCaptureRate").text = string.Format("+{0}%", configData.BaseCaptureRateBonus);
		}
		base.CGet<GameObject>("ExploreBonusRate").SetActive(configData.BaseExploreBonusRate > 0);
		bool flag5 = configData.BaseExploreBonusRate > 0;
		if (flag5)
		{
			base.CGet<TextMeshProUGUI>("AddExploreBonusRate").text = string.Format("+{0}%", configData.BaseExploreBonusRate);
		}
		base.CGet<GameObject>("TravelSpeed").SetActive(configData.BaseTravelTimeReduction > 0);
		bool flag6 = configData.BaseTravelTimeReduction > 0;
		if (flag6)
		{
			base.CGet<TextMeshProUGUI>("AddTravelSpeed").text = string.Format("-{0}%", configData.BaseTravelTimeReduction);
		}
		base.CGet<GameObject>("Inventory").SetActive(configData.BaseMaxInventoryLoadBonus > 0);
		bool flag7 = configData.BaseMaxInventoryLoadBonus > 0;
		if (flag7)
		{
			base.CGet<TextMeshProUGUI>("AddInventory").text = string.Format("+{0:f1}", (float)configData.BaseMaxInventoryLoadBonus / 100f);
		}
		base.CGet<GameObject>("CarrierEffect").SetActive(configData.BaseDropRateBonus > 0 || configData.BaseCaptureRateBonus > 0 || configData.BaseTravelTimeReduction > 0 || configData.BaseMaxInventoryLoadBonus > 0);
		bool hasTame = ItemTemplateHelper.HasCarrierTame(configData.ItemType, configData.TemplateId);
		base.CGet<GameObject>("DriveBeast").SetActive(hasTame);
		base.CGet<GameObject>("CombatState").SetActive(hasTame);
		base.CGet<GameObject>("Tame").SetActive(hasTame);
		bool flag8 = hasTame;
		if (flag8)
		{
			base.CGet<TextMeshProUGUI>("TameTips").text = LocalStringManager.Get(LanguageKey.LK_ItemTips_Carrier_Tame);
			base.CGet<TextMeshProUGUI>("TameTips").GetComponent<TMPTextSpriteHelper>().Parse();
			base.CGet<TextMeshProUGUI>("DriveTips").text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTips_CarrierDriveBeastTip, configData.Name, CombatState.Instance[configData.CombatState].Name);
			base.CGet<TextMeshProUGUI>("CombatStateText").text = CombatState.Instance[configData.CombatState].Desc.ColorReplace();
			this.RefreshTame(itemData.Key.Id, configData);
		}
		this.InitItemDisableFunctionList(itemData);
		base.RefreshDisableFunction();
		base.RefreshHoldCount();
		base.RefreshHotkeyDisplayLockItem();
		bool flag9 = configData.EquipmentType == 9;
		if (flag9)
		{
			this.beastEquipInfo.gameObject.SetActive(this._slot != -1);
			TMP_Text tmp_Text = this.beastEquipInfo;
			int slot = this._slot;
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
			tmp_Text.text = text;
		}
		else
		{
			this.beastEquipInfo.gameObject.SetActive(false);
		}
		base.PostInit();
		GlobalDomainMethod.Call.InvokeGuidingTrigger(262);
	}

	// Token: 0x0600293E RID: 10558 RVA: 0x001341EB File Offset: 0x001323EB
	private void Update()
	{
		base.UpdateCompareCommonPart();
		base.UpdateMoreInfoCtrl();
	}

	// Token: 0x0600293F RID: 10559 RVA: 0x001341FC File Offset: 0x001323FC
	public override void SetNewData(ArgumentBox argsBox)
	{
		this.Init(argsBox);
	}

	// Token: 0x06002940 RID: 10560 RVA: 0x00134208 File Offset: 0x00132408
	private void RefreshTame(int carrierId, CarrierItem carrierItem)
	{
		base.CGet<GameObject>("Tame").SetActive(carrierId >= 0);
		bool flag = carrierId < 0;
		if (!flag)
		{
			ExtraDomainMethod.AsyncCall.GetCarrierTamePoint(this, carrierId, delegate(int offset, RawDataPool dataPool)
			{
				int value = -1;
				Serializer.Deserialize(dataPool, offset, ref value);
				sbyte maxValue = carrierItem.TamePoint;
				ExtraDomainMethod.AsyncCall.GetCarrierMaxTamePoint(this, carrierId, delegate(int offsetInternal, RawDataPool dataPoolInternal)
				{
					bool flag2 = null == this;
					if (!flag2)
					{
						Serializer.Deserialize(dataPoolInternal, offsetInternal, ref maxValue);
						StringBuilder sb = new StringBuilder();
						sb.Append("<color=#" + ((value == 100) ? "brightblue" : "brightred") + ">");
						sb.Append((value == -1) ? 0 : value);
						sb.Append("</color>");
						sb.Append(string.Format("<color=#pinkyellow>/{0}</color>", maxValue));
						this.CGet<TextMeshProUGUI>("TameValue").text = sb.ToString().ColorReplace();
					}
				});
			});
		}
	}

	// Token: 0x06002941 RID: 10561 RVA: 0x00134278 File Offset: 0x00132478
	protected override void InitItemDisableFunctionList(ItemDisplayData itemDisplayData)
	{
		base.InitItemDisableFunctionList(itemDisplayData);
		CarrierItem configData = Carrier.Instance[itemDisplayData.Key.TemplateId];
		bool flag = !configData.Repairable;
		if (flag)
		{
			this._disableFunctionList.Add(MouseTipItem.ItemFunction.Repairable);
		}
		bool flag2 = !configData.Transferable;
		if (flag2)
		{
			this._disableFunctionList.Add(MouseTipItem.ItemFunction.Transferable);
		}
		bool flag3 = !configData.Poisonable;
		if (flag3)
		{
			this._disableFunctionList.Add(MouseTipItem.ItemFunction.Poisonable);
		}
		bool flag4 = !configData.Refinable;
		if (flag4)
		{
			this._disableFunctionList.Add(MouseTipItem.ItemFunction.Refinable);
		}
	}

	// Token: 0x04001E03 RID: 7683
	[SerializeField]
	private TMP_Text beastEquipInfo;

	// Token: 0x04001E04 RID: 7684
	private int _slot;
}
