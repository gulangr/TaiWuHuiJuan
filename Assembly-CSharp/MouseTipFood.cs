using System;
using Config;
using FrameWork;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

// Token: 0x0200029E RID: 670
public class MouseTipFood : MouseTipItem
{
	// Token: 0x17000497 RID: 1175
	// (get) Token: 0x06002A21 RID: 10785 RVA: 0x001411E7 File Offset: 0x0013F3E7
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002A22 RID: 10786 RVA: 0x001411EC File Offset: 0x0013F3EC
	protected unsafe override void Init(ArgumentBox argsBox)
	{
		base.Init(argsBox);
		ItemDisplayData itemData;
		argsBox.Get<ItemDisplayData>("ItemData", out itemData);
		bool templateDataOnly;
		argsBox.Get("TemplateDataOnly", out templateDataOnly);
		this._itemData = itemData;
		FoodItem configData = Food.Instance[itemData.Key.TemplateId];
		RectTransform addPropertyHolder = base.CGet<RectTransform>("AddPropertyHolder");
		RectTransform poisonResistHolder = base.CGet<RectTransform>("PoisonResistHolder");
		bool showPoisonResist = configData.ResistOfHotPoison != 0 || configData.ResistOfGloomyPoison != 0 || configData.ResistOfColdPoison != 0 || configData.ResistOfRedPoison != 0 || configData.ResistOfRottenPoison != 0 || configData.ResistOfIllusoryPoison != 0;
		base.CGet<TextMeshProUGUI>("Name").text = configData.Name;
		base.CGet<CImage>("GradeBack").SetSprite(ItemView.GetGradeIcon(configData.Grade), false, null);
		base.CGet<TextMeshProUGUI>("GradeName").text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", configData.Grade));
		base.CGet<TextMeshProUGUI>("Grade").text = (LocalStringManager.Get(string.Format("LK_Num_{0}", (int)(9 - configData.Grade))) + LocalStringManager.Get(LanguageKey.LK_Item_Grade)).SetColor(Colors.Instance.GradeColors[(int)configData.Grade]);
		base.CGet<TextMeshProUGUI>("Value").text = (templateDataOnly ? configData.BaseValue.ToString() : itemData.Value.ToString());
		base.CGet<GameObject>("Material").SetActive(!templateDataOnly);
		base.CGet<CImage>("ItemIcon").SetSprite(configData.Icon, false, null);
		base.SetItemDesc(configData.Desc, itemData.LoveTokenDataItem);
		base.CGet<TextMeshProUGUI>("SubType").text = LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", configData.ItemSubType));
		base.CGet<TextMeshProUGUI>("Weight").text = NumberFormatUtils.FormatItemWeight(itemData.Weight);
		short eatingTime;
		bool showEatingTime = argsBox.Get("EatingTime", out eatingTime);
		base.CGet<GameObject>("EatingTimeTips").SetActive(showEatingTime);
		bool flag = showEatingTime;
		if (flag)
		{
			base.CGet<TextMeshProUGUI>("EatingTime").text = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_Eating_Time, eatingTime).ColorReplace();
		}
		this.InitItemDisableFunctionList(itemData);
		base.RefreshDisableFunction();
		int index = 0;
		for (int i = 0; i < 6; i++)
		{
			index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, (short)(0 + i), (int)(*(ref configData.MainAttributesRegen.Items.FixedElementField + (IntPtr)i * 2)), true, false, false, false, true, false);
		}
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 0, (int)configData.Strength, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 1, (int)configData.Dexterity, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 2, (int)configData.Concentration, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 3, (int)configData.Vitality, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 4, (int)configData.Energy, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 5, (int)configData.Intelligence, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 6, (int)configData.HitRateStrength, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 7, (int)configData.HitRateTechnique, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 8, (int)configData.HitRateSpeed, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 9, (int)configData.HitRateMind, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 10, (int)configData.PenetrateOfOuter, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 11, (int)configData.PenetrateOfInner, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 12, (int)configData.AvoidRateStrength, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 13, (int)configData.AvoidRateTechnique, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 14, (int)configData.AvoidRateSpeed, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 15, (int)configData.AvoidRateMind, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 16, (int)configData.PenetrateResistOfOuter, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 17, (int)configData.PenetrateResistOfInner, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 18, (int)configData.RecoveryOfStance, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 19, (int)configData.RecoveryOfBreath, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 20, (int)configData.MoveSpeed, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 21, (int)configData.RecoveryOfFlaw, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 22, (int)configData.CastSpeed, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 23, (int)configData.RecoveryOfBlockedAcupoint, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 24, (int)configData.WeaponSwitchSpeed, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 25, (int)configData.AttackSpeed, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 26, (int)configData.InnerRatio, false, false, false, true, true, false);
		index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 27, (int)configData.RecoveryOfQiDisorder, false, false, false, true, true, false);
		for (int j = index; j < addPropertyHolder.childCount; j++)
		{
			addPropertyHolder.GetChild(j).gameObject.SetActive(false);
		}
		base.CGet<GameObject>("PoisonResistSpace").SetActive(showPoisonResist);
		poisonResistHolder.gameObject.SetActive(showPoisonResist);
		bool flag2 = showPoisonResist;
		if (flag2)
		{
			MouseTip_Util.UpdatePoisonResistRefers(poisonResistHolder.GetChild(0).GetComponent<Refers>(), ECharacterPropertyDisplayType.ResistOfHotPoison, (int)configData.ResistOfHotPoison);
			MouseTip_Util.UpdatePoisonResistRefers(poisonResistHolder.GetChild(1).GetComponent<Refers>(), ECharacterPropertyDisplayType.ResistOfGloomyPoison, (int)configData.ResistOfGloomyPoison);
			MouseTip_Util.UpdatePoisonResistRefers(poisonResistHolder.GetChild(2).GetComponent<Refers>(), ECharacterPropertyDisplayType.ResistOfRedPoison, (int)configData.ResistOfRedPoison);
			MouseTip_Util.UpdatePoisonResistRefers(poisonResistHolder.GetChild(3).GetComponent<Refers>(), ECharacterPropertyDisplayType.ResistOfColdPoison, (int)configData.ResistOfColdPoison);
			MouseTip_Util.UpdatePoisonResistRefers(poisonResistHolder.GetChild(4).GetComponent<Refers>(), ECharacterPropertyDisplayType.ResistOfRottenPoison, (int)configData.ResistOfRottenPoison);
			MouseTip_Util.UpdatePoisonResistRefers(poisonResistHolder.GetChild(5).GetComponent<Refers>(), ECharacterPropertyDisplayType.ResistOfIllusoryPoison, (int)configData.ResistOfIllusoryPoison);
		}
		base.RefreshPoisons(default(PoisonsAndLevels), itemData);
		base.CGet<TextMeshProUGUI>("Duration").text = configData.Duration.ToString();
		base.CGet<GameObject>("UseInCombat").SetActive(configData.ConsumedFeatureMedals >= 0);
		bool flag3 = configData.ConsumedFeatureMedals >= 0;
		if (flag3)
		{
			base.CGet<TextMeshProUGUI>("CostWisdom").text = string.Format("x{0}", configData.ConsumedFeatureMedals);
		}
		base.RefreshHoldCount();
		base.RefreshHotkeyDisplayLockItem();
		base.ForceRebuildLayout(2U, null);
	}

	// Token: 0x06002A23 RID: 10787 RVA: 0x00141A64 File Offset: 0x0013FC64
	protected override void InitItemDisableFunctionList(ItemDisplayData itemDisplayData)
	{
		base.InitItemDisableFunctionList(itemDisplayData);
		FoodItem configData = Food.Instance[itemDisplayData.Key.TemplateId];
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
