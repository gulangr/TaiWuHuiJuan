using System;
using System.Text;
using Config;
using FrameWork;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameDataExtensions;
using TMPro;
using UnityEngine;

// Token: 0x02000271 RID: 625
public class MouseTipArmor : MouseTipItem
{
	// Token: 0x1700047E RID: 1150
	// (get) Token: 0x06002906 RID: 10502 RVA: 0x00130BEC File Offset: 0x0012EDEC
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

	// Token: 0x06002907 RID: 10503 RVA: 0x00130C20 File Offset: 0x0012EE20
	protected unsafe override void Init(ArgumentBox argsBox)
	{
		base.Init(argsBox);
		ItemDisplayData itemData;
		argsBox.Get<ItemDisplayData>("ItemData", out itemData);
		bool templateDataOnly;
		argsBox.Get("TemplateDataOnly", out templateDataOnly);
		argsBox.Get("IsInCompareUI", out this._isInCompareUI);
		bool flag = !argsBox.Get("CharId", out this._charId);
		if (flag)
		{
			this._charId = -1;
		}
		this._itemData = itemData;
		ArmorItem configData = Armor.Instance[itemData.Key.TemplateId];
		TextMeshProUGUI currDurabilityYellow = base.CGet<TextMeshProUGUI>("CurrDurabilityYellow");
		TextMeshProUGUI currDurabilityRed = base.CGet<TextMeshProUGUI>("CurrDurabilityRed");
		StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
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
		strBuilder.Clear();
		bool flag2 = configData.ResourceType >= 0;
		if (flag2)
		{
			strBuilder.Append(ResourceType.Instance[configData.ResourceType].Name);
		}
		strBuilder.Append(LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", configData.ItemSubType)));
		base.CGet<TextMeshProUGUI>("SubType").text = strBuilder.ToString();
		currDurabilityYellow.gameObject.SetActive(hasHalfDurability);
		currDurabilityRed.gameObject.SetActive(!hasHalfDurability);
		(hasHalfDurability ? currDurabilityYellow : currDurabilityRed).text = itemData.Durability.ToString();
		base.CGet<TextMeshProUGUI>("MaxDurability").text = string.Format("/{0}", itemData.MaxDurability);
		base.CGet<TextMeshProUGUI>("Weight").text = NumberFormatUtils.FormatItemWeight(itemData.Weight);
		base.CGet<TextMeshProUGUI>("EquipAttack").text = string.Format("{0:f2}", (float)itemData.EquipmentAttack / 100f);
		base.CGet<TextMeshProUGUI>("EquipDefend").text = string.Format("{0:f2}", (float)itemData.EquipmentDefense / 100f);
		string powerStr = itemData.ShouldShowPower() ? (itemData.PowerInfo.Power.ToString() ?? "") : "-";
		base.CGet<TextMeshProUGUI>("Power").text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_WeaponAndArmor_Power, powerStr);
		this.SetDurabilityPosition(itemData.ShouldShowPower());
		this.InitItemDisableFunctionList(itemData);
		base.RefreshDisableFunction();
		RectTransform addPropertyHolder = base.CGet<RectTransform>("AddPropertyHolder");
		TipsAddProperty addPropertyPrefab = base.CGet<TipsAddProperty>("AddProperty");
		int index = 0;
		for (sbyte hitType = 0; hitType < 4; hitType += 1)
		{
			index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, (short)(12 + hitType), (int)(*(ref itemData.HitAvoidFactor.Items.FixedElementField + (IntPtr)hitType * 2)), false, true, false, false, true, true);
		}
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 16, (int)(itemData.PenetrationInfo.Item1 * itemData.PowerInfo.Power / 100), false, true, false, false, true, true);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 17, (int)(itemData.PenetrationInfo.Item2 * itemData.PowerInfo.Power / 100), false, true, false, false, true, true);
		for (int i = index; i < addPropertyHolder.childCount; i++)
		{
			addPropertyHolder.GetChild(i).gameObject.SetActive(false);
		}
		base.CGet<GameObject>("OuterInjury").SetActive(itemData.InjuryFactors.Outer > 0);
		base.CGet<TextMeshProUGUI>("ReduceOuterInjury").text = string.Format("{0}%", itemData.InjuryFactors.Outer);
		base.CGet<GameObject>("InnerInjury").SetActive(itemData.InjuryFactors.Inner > 0);
		base.CGet<TextMeshProUGUI>("ReduceInnerInjury").text = string.Format("{0}%", itemData.InjuryFactors.Inner);
		base.CGet<GameObject>("EffectSpace1").SetActive(itemData.InjuryFactors.Outer > 0 || itemData.InjuryFactors.Inner > 0);
		base.RefreshPoisons(default(PoisonsAndLevels), itemData);
		base.RefreshAttachedEffect();
		base.RefreshRequirement(this._itemData.Requirements);
		base.CGet<TextMeshProUGUI>("CurrAndMaxPower").text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_WeaponAndArmor_RequirementsPower, this._itemData.ShouldShowPower() ? this._itemData.PowerInfo.RequirementsPower.ToString() : "-", this._itemData.PowerInfo.MaxPower.ToString());
		EasyPool.Free<StringBuilder>(strBuilder);
		base.RefreshHoldCount();
		base.RefreshHotkeyDisplayLockItem();
		base.ForceRebuildLayout(2U, null);
		base.PostInit();
		GlobalDomainMethod.Call.InvokeGuidingTrigger(261);
	}

	// Token: 0x06002908 RID: 10504 RVA: 0x00131230 File Offset: 0x0012F430
	private void SetDurabilityPosition(bool shouldShowPower)
	{
		base.CGet<RectTransform>("PowerSpace").gameObject.SetActive(shouldShowPower);
		base.CGet<RectTransform>("PowerAndRange").gameObject.SetActive(shouldShowPower);
		Transform durability = base.CGet<GameObject>("Durability").transform;
		durability.SetParent(base.CGet<RectTransform>(shouldShowPower ? "DurabilityHolderWithPower" : "DurabilityHolderNoPower"));
		(durability as RectTransform).anchoredPosition = Vector2.zero;
	}

	// Token: 0x06002909 RID: 10505 RVA: 0x001312AA File Offset: 0x0012F4AA
	private void Update()
	{
		base.UpdateCompare();
	}

	// Token: 0x0600290A RID: 10506 RVA: 0x001312B4 File Offset: 0x0012F4B4
	public override void SetNewData(ArgumentBox argsBox)
	{
		this.Init(argsBox);
	}

	// Token: 0x0600290B RID: 10507 RVA: 0x001312C0 File Offset: 0x0012F4C0
	protected override void InitItemDisableFunctionList(ItemDisplayData itemDisplayData)
	{
		base.InitItemDisableFunctionList(itemDisplayData);
		ArmorItem configData = Armor.Instance[itemDisplayData.Key.TemplateId];
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
}
