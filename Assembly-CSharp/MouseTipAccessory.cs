using System;
using System.Text;
using Config;
using FrameWork;
using Game.Components.MouseTip;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameDataExtensions;
using TMPro;
using UnityEngine;

// Token: 0x0200026B RID: 619
public class MouseTipAccessory : MouseTipItem
{
	// Token: 0x17000479 RID: 1145
	// (get) Token: 0x060028E4 RID: 10468 RVA: 0x0012EA9A File Offset: 0x0012CC9A
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

	// Token: 0x060028E5 RID: 10469 RVA: 0x0012EACC File Offset: 0x0012CCCC
	protected override void Init(ArgumentBox argsBox)
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
		AccessoryItem configData = Accessory.Instance[itemData.Key.TemplateId];
		TextMeshProUGUI currDurabilityYellow = base.CGet<TextMeshProUGUI>("CurrDurabilityYellow");
		TextMeshProUGUI currDurabilityRed = base.CGet<TextMeshProUGUI>("CurrDurabilityRed");
		StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
		base.CGet<TextMeshProUGUI>("Name").text = configData.Name;
		base.CGet<CImage>("GradeBack").SetSprite((configData.MysteryEffectId >= 0) ? "ui9_icon_grade_mystery" : ItemView.GetGradeIcon(configData.Grade), false, null);
		base.CGet<TextMeshProUGUI>("GradeName").text = ((configData.MysteryEffectId >= 0) ? LocalStringManager.Get(LanguageKey.LK_MouseTip_Mystery_Short_Key) : LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", configData.Grade)));
		base.CGet<TextMeshProUGUI>("Grade").text = ((configData.MysteryEffectId >= 0) ? LocalStringManager.Get(LanguageKey.LK_Item_Grade) : (LocalStringManager.Get(string.Format("LK_Num_{0}", (int)(9 - configData.Grade))) + LocalStringManager.Get(LanguageKey.LK_Item_Grade))).SetColor(Colors.Instance.GradeColors[(int)configData.Grade]);
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
		base.CGet<TextMeshProUGUI>("Weight").text = NumberFormatUtils.FormatItemWeight(itemData.Weight);
		currDurabilityYellow.gameObject.SetActive(hasHalfDurability);
		currDurabilityRed.gameObject.SetActive(!hasHalfDurability);
		(hasHalfDurability ? currDurabilityYellow : currDurabilityRed).text = itemData.Durability.ToString();
		base.CGet<TextMeshProUGUI>("MaxDurability").text = string.Format("/{0}", itemData.MaxDurability);
		base.CGet<GameObject>("DropRate").SetActive(configData.DropRateBonus > 0);
		bool flag3 = configData.DropRateBonus > 0;
		if (flag3)
		{
			base.CGet<TextMeshProUGUI>("AddDropRate").text = string.Format("+{0}%", configData.DropRateBonus);
		}
		base.CGet<GameObject>("ExploreBonusRate").SetActive(configData.BaseExploreBonusRate > 0);
		bool flag4 = configData.BaseExploreBonusRate > 0;
		if (flag4)
		{
			base.CGet<TextMeshProUGUI>("AddExploreBonusRate").text = string.Format("+{0}%", configData.BaseExploreBonusRate);
		}
		base.CGet<GameObject>("CaptureRate").SetActive(configData.BaseCaptureRateBonus > 0);
		bool flag5 = configData.BaseCaptureRateBonus > 0;
		if (flag5)
		{
			base.CGet<TextMeshProUGUI>("AddCaptureRate").text = string.Format("+{0}%", configData.BaseCaptureRateBonus);
		}
		base.CGet<GameObject>("Inventory").SetActive(configData.MaxInventoryLoadBonus > 0);
		bool flag6 = configData.MaxInventoryLoadBonus > 0;
		if (flag6)
		{
			base.CGet<TextMeshProUGUI>("AddInventory").text = string.Format("+{0:f1}", (float)configData.MaxInventoryLoadBonus / 100f);
		}
		base.CGet<GameObject>("CombatSkillPower").SetActive(configData.BonusCombatSkillSect >= 0);
		bool flag7 = configData.BonusCombatSkillSect >= 0;
		if (flag7)
		{
			base.CGet<TextMeshProUGUI>("PowerOrganization").text = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_Organization_Add_Combat_Skill_Power, Organization.Instance[configData.BonusCombatSkillSect].Name);
			base.CGet<TextMeshProUGUI>("AddPower").text = string.Format("+{0}", GlobalConfig.Instance.SectAccessoryBonusCombatSkillPower);
		}
		base.CGet<Refers>("CombatSkillPowerLimit").gameObject.SetActive(configData.CombatSkillAddMaxPower > 0);
		bool flag8 = configData.CombatSkillAddMaxPower > 0;
		if (flag8)
		{
			base.CGet<Refers>("CombatSkillPowerLimit").CGet<TextMeshProUGUI>("AddPower").SetText(string.Format("+{0}", configData.CombatSkillAddMaxPower), true);
		}
		bool anyEffectShowed = configData.DropRateBonus > 0 || configData.BaseCaptureRateBonus > 0 || configData.MaxInventoryLoadBonus > 0;
		this.InitItemDisableFunctionList(itemData);
		base.RefreshDisableFunction();
		RectTransform addPropertyHolder = base.CGet<RectTransform>("AddPropertyHolder");
		int index = 0;
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 0, (int)configData.Strength, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 1, (int)configData.Dexterity, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 2, (int)configData.Concentration, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 3, (int)configData.Vitality, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 4, (int)configData.Energy, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 5, (int)configData.Intelligence, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 6, (int)configData.HitRateStrength, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 7, (int)configData.HitRateTechnique, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 8, (int)configData.HitRateSpeed, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 9, (int)configData.HitRateMind, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 10, (int)configData.PenetrateOfOuter, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 11, (int)configData.PenetrateOfInner, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 12, (int)configData.AvoidRateStrength, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 13, (int)configData.AvoidRateTechnique, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 14, (int)configData.AvoidRateSpeed, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 15, (int)configData.AvoidRateMind, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 16, (int)configData.PenetrateResistOfOuter, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 17, (int)configData.PenetrateResistOfInner, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 18, (int)configData.RecoveryOfStance, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 19, (int)configData.RecoveryOfBreath, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 20, (int)configData.MoveSpeed, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 21, (int)configData.RecoveryOfFlaw, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 22, (int)configData.CastSpeed, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 23, (int)configData.RecoveryOfBlockedAcupoint, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 24, (int)configData.WeaponSwitchSpeed, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 25, (int)configData.AttackSpeed, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 26, (int)configData.InnerRatio, false, false, false, true, false, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 27, (int)configData.RecoveryOfQiDisorder, false, false, false, true, false, false);
		for (int i = index; i < addPropertyHolder.childCount; i++)
		{
			addPropertyHolder.GetChild(i).gameObject.SetActive(false);
		}
		anyEffectShowed = (anyEffectShowed || index > 0);
		RectTransform poisonResistHolder = base.CGet<RectTransform>("PoisonResistHolder");
		bool showPoisonResist = configData.ResistOfHotPoison != 0 || configData.ResistOfGloomyPoison != 0 || configData.ResistOfColdPoison != 0 || configData.ResistOfRedPoison != 0 || configData.ResistOfRottenPoison != 0 || configData.ResistOfIllusoryPoison != 0;
		poisonResistHolder.gameObject.SetActive(showPoisonResist);
		bool flag9 = showPoisonResist;
		if (flag9)
		{
			MouseTip_Util.UpdatePoisonResistRefers(poisonResistHolder.GetChild(0).GetComponent<Refers>(), ECharacterPropertyDisplayType.ResistOfHotPoison, (int)configData.ResistOfHotPoison);
			MouseTip_Util.UpdatePoisonResistRefers(poisonResistHolder.GetChild(1).GetComponent<Refers>(), ECharacterPropertyDisplayType.ResistOfGloomyPoison, (int)configData.ResistOfGloomyPoison);
			MouseTip_Util.UpdatePoisonResistRefers(poisonResistHolder.GetChild(2).GetComponent<Refers>(), ECharacterPropertyDisplayType.ResistOfRedPoison, (int)configData.ResistOfRedPoison);
			MouseTip_Util.UpdatePoisonResistRefers(poisonResistHolder.GetChild(3).GetComponent<Refers>(), ECharacterPropertyDisplayType.ResistOfColdPoison, (int)configData.ResistOfColdPoison);
			MouseTip_Util.UpdatePoisonResistRefers(poisonResistHolder.GetChild(4).GetComponent<Refers>(), ECharacterPropertyDisplayType.ResistOfRottenPoison, (int)configData.ResistOfRottenPoison);
			MouseTip_Util.UpdatePoisonResistRefers(poisonResistHolder.GetChild(5).GetComponent<Refers>(), ECharacterPropertyDisplayType.ResistOfIllusoryPoison, (int)configData.ResistOfIllusoryPoison);
		}
		anyEffectShowed = (anyEffectShowed || showPoisonResist);
		base.RefreshPoisons(default(PoisonsAndLevels), itemData);
		base.RefreshRequirement(this._itemData.Requirements);
		base.RefreshAttachedEffect();
		EasyPool.Free<StringBuilder>(strBuilder);
		base.RefreshHoldCount();
		base.RefreshHotkeyDisplayLockItem();
		base.ForceRebuildLayout(2U, null);
		base.PostInit();
	}

	// Token: 0x060028E6 RID: 10470 RVA: 0x0012F628 File Offset: 0x0012D828
	private void Update()
	{
		AccessoryItem configData = Accessory.Instance[this._itemData.Key.TemplateId];
		this.UpdatePowerInfo(configData);
		bool flag = configData.MysteryEffectId >= 0;
		if (flag)
		{
			base.UpdateCompare();
		}
		else
		{
			base.UpdateCompareCommonPart();
			base.UpdateMoreInfoCtrl();
		}
	}

	// Token: 0x060028E7 RID: 10471 RVA: 0x0012F684 File Offset: 0x0012D884
	public override void SetNewData(ArgumentBox argsBox)
	{
		this.Init(argsBox);
	}

	// Token: 0x060028E8 RID: 10472 RVA: 0x0012F690 File Offset: 0x0012D890
	protected override void InitItemDisableFunctionList(ItemDisplayData itemDisplayData)
	{
		base.InitItemDisableFunctionList(itemDisplayData);
		AccessoryItem configData = Accessory.Instance[itemDisplayData.Key.TemplateId];
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

	// Token: 0x060028E9 RID: 10473 RVA: 0x0012F728 File Offset: 0x0012D928
	private void UpdatePowerInfo(AccessoryItem configData)
	{
		bool flag = configData == null;
		if (!flag)
		{
			TextMeshProUGUI power = base.CGet<TextMeshProUGUI>("Power");
			TextMeshProUGUI maxPower = base.CGet<TextMeshProUGUI>("CurrAndMaxPower");
			GameObject powerSpace = base.CGet<GameObject>("PowerSpace");
			GameObject powerHolder = base.CGet<GameObject>("PowerHolder");
			GameObject compatibility = base.CGet<GameObject>("Compatibility");
			MysteryEffectInfo mysteryEffect = base.CGet<MysteryEffectInfo>("MysteryEffect");
			bool flag2 = configData.MysteryEffectId < 0;
			if (flag2)
			{
				maxPower.gameObject.SetActive(false);
				powerSpace.gameObject.SetActive(false);
				powerHolder.gameObject.SetActive(false);
				compatibility.gameObject.SetActive(false);
				mysteryEffect.gameObject.SetActive(false);
			}
			else
			{
				maxPower.gameObject.SetActive(true);
				powerSpace.gameObject.SetActive(true);
				powerHolder.gameObject.SetActive(true);
				compatibility.gameObject.SetActive(true);
				mysteryEffect.gameObject.SetActive(true);
				string powerStr = this._itemData.ShouldShowPower() ? (this._itemData.PowerInfo.Power.ToString() ?? "") : "-";
				power.text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_WeaponAndArmor_Power, powerStr);
				maxPower.text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_WeaponAndArmor_RequirementsPower, this._itemData.ShouldShowPower() ? this._itemData.PowerInfo.RequirementsPower.ToString() : "-", this._itemData.PowerInfo.MaxPower.ToString());
				this.UpdateMysteryInfo(configData, mysteryEffect, this._itemData.PowerInfo.RequirementsPower);
			}
		}
	}

	// Token: 0x060028EA RID: 10474 RVA: 0x0012F8DA File Offset: 0x0012DADA
	private void UpdateMysteryInfo(AccessoryItem configData, MysteryEffectInfo mysteryEffect, short requirementsPower)
	{
		mysteryEffect.RefreshInfo(configData.MysteryEffectId, requirementsPower);
	}
}
