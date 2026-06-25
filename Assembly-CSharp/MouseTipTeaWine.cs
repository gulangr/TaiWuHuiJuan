using System;
using System.Collections.Generic;
using CharacterDataMonitor;
using Config;
using FrameWork;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu.Profession;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020002E0 RID: 736
public class MouseTipTeaWine : MouseTipItem
{
	// Token: 0x170004D3 RID: 1235
	// (get) Token: 0x06002B9E RID: 11166 RVA: 0x00153DE5 File Offset: 0x00151FE5
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002B9F RID: 11167 RVA: 0x00153DE8 File Offset: 0x00151FE8
	protected override void Init(ArgumentBox argsBox)
	{
		MouseTipTeaWine.<>c__DisplayClass5_0 CS$<>8__locals1 = new MouseTipTeaWine.<>c__DisplayClass5_0();
		CS$<>8__locals1.<>4__this = this;
		base.Init(argsBox);
		this.Clear();
		ItemDisplayData itemData;
		argsBox.Get<ItemDisplayData>("ItemData", out itemData);
		bool templateDataOnly;
		argsBox.Get("TemplateDataOnly", out templateDataOnly);
		this._itemData = itemData;
		CS$<>8__locals1.configData = TeaWine.Instance[itemData.Key.TemplateId];
		RectTransform addPropertyHolder = base.CGet<RectTransform>("AddPropertyHolder");
		base.CGet<TextMeshProUGUI>("Name").text = CS$<>8__locals1.configData.Name;
		base.CGet<CImage>("GradeBack").SetSprite(ItemView.GetGradeIcon(CS$<>8__locals1.configData.Grade), false, null);
		base.CGet<TextMeshProUGUI>("GradeName").text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", CS$<>8__locals1.configData.Grade));
		base.CGet<TextMeshProUGUI>("Grade").text = (LocalStringManager.Get(string.Format("LK_Num_{0}", (int)(9 - CS$<>8__locals1.configData.Grade))) + LocalStringManager.Get(LanguageKey.LK_Item_Grade)).SetColor(Colors.Instance.GradeColors[(int)CS$<>8__locals1.configData.Grade]);
		base.CGet<TextMeshProUGUI>("Value").text = (templateDataOnly ? CS$<>8__locals1.configData.BaseValue.ToString() : itemData.Value.ToString());
		base.CGet<GameObject>("Material").SetActive(!templateDataOnly);
		base.CGet<CImage>("ItemIcon").SetSprite(CS$<>8__locals1.configData.Icon, false, null);
		base.SetItemDesc(CS$<>8__locals1.configData.Desc, itemData.LoveTokenDataItem);
		base.CGet<TextMeshProUGUI>("SubType").text = LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", CS$<>8__locals1.configData.ItemSubType));
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
		index += MouseTip_Util.AppendAddTeaWineProperty(CS$<>8__locals1.configData.ItemSubType, addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 6, (int)CS$<>8__locals1.configData.HitRateStrength, templateDataOnly);
		index += MouseTip_Util.AppendAddTeaWineProperty(CS$<>8__locals1.configData.ItemSubType, addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 7, (int)CS$<>8__locals1.configData.HitRateTechnique, templateDataOnly);
		index += MouseTip_Util.AppendAddTeaWineProperty(CS$<>8__locals1.configData.ItemSubType, addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 8, (int)CS$<>8__locals1.configData.HitRateSpeed, templateDataOnly);
		index += MouseTip_Util.AppendAddTeaWineProperty(CS$<>8__locals1.configData.ItemSubType, addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 9, (int)CS$<>8__locals1.configData.HitRateMind, templateDataOnly);
		index += MouseTip_Util.AppendAddTeaWineProperty(CS$<>8__locals1.configData.ItemSubType, addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 10, (int)CS$<>8__locals1.configData.PenetrateOfOuter, templateDataOnly);
		index += MouseTip_Util.AppendAddTeaWineProperty(CS$<>8__locals1.configData.ItemSubType, addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 11, (int)CS$<>8__locals1.configData.PenetrateOfInner, templateDataOnly);
		index += MouseTip_Util.AppendAddTeaWineProperty(CS$<>8__locals1.configData.ItemSubType, addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 12, (int)CS$<>8__locals1.configData.AvoidRateStrength, templateDataOnly);
		index += MouseTip_Util.AppendAddTeaWineProperty(CS$<>8__locals1.configData.ItemSubType, addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 13, (int)CS$<>8__locals1.configData.AvoidRateTechnique, templateDataOnly);
		index += MouseTip_Util.AppendAddTeaWineProperty(CS$<>8__locals1.configData.ItemSubType, addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 14, (int)CS$<>8__locals1.configData.AvoidRateSpeed, templateDataOnly);
		index += MouseTip_Util.AppendAddTeaWineProperty(CS$<>8__locals1.configData.ItemSubType, addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 15, (int)CS$<>8__locals1.configData.AvoidRateMind, templateDataOnly);
		index += MouseTip_Util.AppendAddTeaWineProperty(CS$<>8__locals1.configData.ItemSubType, addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 16, (int)CS$<>8__locals1.configData.PenetrateResistOfOuter, templateDataOnly);
		index += MouseTip_Util.AppendAddTeaWineProperty(CS$<>8__locals1.configData.ItemSubType, addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 17, (int)CS$<>8__locals1.configData.PenetrateResistOfInner, templateDataOnly);
		index += MouseTip_Util.AppendAddTeaWineProperty(CS$<>8__locals1.configData.ItemSubType, addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 26, (int)CS$<>8__locals1.configData.InnerRatio, templateDataOnly);
		index += MouseTip_Util.AppendAddTeaWineProperty(CS$<>8__locals1.configData.ItemSubType, addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 27, (int)CS$<>8__locals1.configData.RecoveryOfQiDisorder, templateDataOnly);
		for (int i = index; i < addPropertyHolder.childCount; i++)
		{
			addPropertyHolder.GetChild(i).gameObject.SetActive(false);
		}
		bool flag2 = !templateDataOnly && CS$<>8__locals1.configData.ItemSubType == 901 && CS$<>8__locals1.configData.SolarTermType >= 0;
		if (flag2)
		{
			this._winePercent = 0;
			bool flag3 = SingletonObject.getInstance<ProfessionModel>().IsSkillEquipped(30);
			if (flag3)
			{
				this._drunkWine.Clear();
				EatingItemMonitor monitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<EatingItemMonitor>(SingletonObject.getInstance<BasicGameData>().TaiwuCharId, false);
				foreach (ValueTuple<ItemKey, short> valueTuple in monitor.EatingItemList)
				{
					ItemKey itemKey = valueTuple.Item1;
					bool flag4 = itemKey.IsValid() && ItemTemplateHelper.GetItemSubType(itemKey.ItemType, itemKey.TemplateId) == 901;
					if (flag4)
					{
						this._drunkWine.Add(itemKey.TemplateId);
					}
				}
				ProfessionData professionData = SingletonObject.getInstance<ProfessionModel>().FindProfessionDataBySkillId(30).Item1;
				bool flag5 = professionData != null;
				if (flag5)
				{
					this._winePercent = (int)professionData.GetSeniorityToWineTasterSolarTermBonus(this._drunkWine.Count);
				}
			}
			Transform parent = base.CGet<GameObject>("PropertyHolder").transform;
			GameObject template = base.CGet<GameObject>("SolarTermTemplate");
			SolarTermItem solarTerm = this.GetSolarTermConfig(CS$<>8__locals1.configData.SolarTermType);
			base.CGet<TextMeshProUGUI>("PoemText").SetText(solarTerm.Poem, true);
			base.CGet<TextMeshProUGUI>("SolarTermTitle").SetText(LocalStringManager.GetFormat(LanguageKey.LK_TeaWine_SolarTerm_Title, solarTerm.Name), true);
			bool flag6 = CS$<>8__locals1.configData.SolarTermType == 0;
			if (flag6)
			{
				base.CGet<CImage>("SolarTermBackGround").SetSprite("mousetip_base_8", false, null);
				base.CGet<CImage>("SolarTermImage").SetSprite("mousetip_winetype_0", false, null);
			}
			else
			{
				base.CGet<CImage>("SolarTermBackGround").SetSprite("mousetip_base_9", false, null);
				base.CGet<CImage>("SolarTermImage").SetSprite("mousetip_winetype_1", false, null);
			}
			foreach (byte fiveElementsType in solarTerm.FiveElementsTypesOfCombatSkillBuff)
			{
				this.AddSolarTermPropertyItem(template, parent, string.Format("mousetip_shuxing_{0}", fiveElementsType), LocalStringManager.GetFormat(LanguageKey.LK_Solar_Term_Buff_Skill_Power, LocalStringManager.Get(string.Format("LK_FiveElements_Type_{0}", fiveElementsType))), (int)GlobalConfig.Instance.SolarTermAddCombatSkillPower, true);
			}
			bool flag7 = solarTerm.PoisonBuffType >= 0;
			if (flag7)
			{
				this.AddSolarTermPropertyItem(template, parent, string.Format("mousetip_duxing_{0}", solarTerm.PoisonBuffType), LocalStringManager.GetFormat(LanguageKey.LK_Solar_Term_Buff_Poison, Poison.Instance[solarTerm.PoisonBuffType].Name), (int)GlobalConfig.Instance.SolarTermAddPoisonEffect, true);
			}
			bool flag8 = solarTerm.DetoxBuffType >= 0;
			if (flag8)
			{
				this.AddSolarTermPropertyItem(template, parent, string.Format("mousetip_duxing_{0}", solarTerm.DetoxBuffType), LocalStringManager.GetFormat(LanguageKey.LK_Solar_Term_Buff_Heal_Poison, Poison.Instance[solarTerm.DetoxBuffType].Name), (int)GlobalConfig.Instance.SolarTermAddHealPoison, true);
			}
			bool outerHealingBuff = solarTerm.OuterHealingBuff;
			if (outerHealingBuff)
			{
				this.AddSolarTermPropertyItem(template, parent, "mousetip_waishang", LocalStringManager.Get(LanguageKey.LK_Solar_Term_Buff_Heal_Outer_Injury), (int)GlobalConfig.Instance.SolarTermAddHealOuterInjury, true);
			}
			bool innerHealingBuff = solarTerm.InnerHealingBuff;
			if (innerHealingBuff)
			{
				this.AddSolarTermPropertyItem(template, parent, "mousetip_neishang", LocalStringManager.Get(LanguageKey.LK_Solar_Term_Buff_Heal_Inner_Injury), (int)GlobalConfig.Instance.SolarTermAddHealInnerInjury, true);
			}
			bool qiDisorderRecoveringBuff = solarTerm.QiDisorderRecoveringBuff;
			if (qiDisorderRecoveringBuff)
			{
				this.AddSolarTermPropertyItem(template, parent, "mousetip_qi_1", LocalStringManager.Get(LanguageKey.LK_Solar_Term_Buff_Recover_QiDisorder), (int)GlobalConfig.Instance.SolarTermAddRecoverQiDisorder, true);
				this.AddSolarTermPropertyItem(template, parent, "mousetip_qi_0", LocalStringManager.Get(LanguageKey.LK_Qi_Disorder), (int)GlobalConfig.Instance.SolarTermAddRecoverQiDisorder, false);
			}
			bool healthBuff = solarTerm.HealthBuff;
			if (healthBuff)
			{
				this.AddSolarTermPropertyItem(template, parent, "mousetip_jiankang", LocalStringManager.Get(LanguageKey.LK_Solar_Term_Buff_Health), (int)GlobalConfig.Instance.SolarTermAddHealth, true);
			}
			base.CGet<GameObject>("SolarTerm").SetActive(true);
		}
		else
		{
			base.CGet<GameObject>("SolarTerm").SetActive(false);
		}
		base.RefreshPoisons(default(PoisonsAndLevels), itemData);
		int qiDisorder = (int)(CS$<>8__locals1.configData.DirectChangeOfQiDisorder / 10);
		int qiMax = qiDisorder * GlobalConfig.Instance.TeaWineEffectDisorderOfQiDelta[1] / 100;
		int qiMin = qiDisorder * GlobalConfig.Instance.TeaWineEffectDisorderOfQiDelta[0] / 100;
		base.CGet<GameObject>("QiDisorder").SetActive(qiDisorder != 0);
		base.CGet<GameObject>("QiDisorder").GetComponent<MonoJoint>().JointSync();
		base.CGet<TextMeshProUGUI>("AddQiDisorder").text = ((qiDisorder >= 0) ? string.Format("+({0}~{1})", qiMin, qiMax) : "");
		base.CGet<TextMeshProUGUI>("ReduceQiDisorder").text = ((qiDisorder >= 0) ? "" : string.Format("-({0}~{1})", Math.Abs(qiMin), Math.Abs(qiMax)));
		bool flag9 = CS$<>8__locals1.configData.ActionPointRecover != 0;
		if (flag9)
		{
			bool flag10 = templateDataOnly;
			if (flag10)
			{
				CS$<>8__locals1.<Init>g__UnlockProcedure|1(true);
			}
			else
			{
				ExtraDomainMethod.AsyncCall.IsProfessionalSkillUnlocked(this, 16, 2, delegate(int offset, RawDataPool dataPool)
				{
					bool unlocked = false;
					Serializer.Deserialize(dataPool, offset, ref unlocked);
					base.<Init>g__UnlockProcedure|1(unlocked);
				});
			}
		}
		else
		{
			base.CGet<GameObject>("ActionPoint").SetActive(false);
		}
		base.CGet<TextMeshProUGUI>("Duration").text = CS$<>8__locals1.configData.Duration.ToString();
		base.CGet<TextMeshProUGUI>("CostWisdom").text = string.Format("x{0}", CS$<>8__locals1.configData.ConsumedFeatureMedals);
		base.RefreshHoldCount();
		base.RefreshHotkeyDisplayLockItem();
		base.ForceRebuildLayout(2U, null);
	}

	// Token: 0x06002BA0 RID: 11168 RVA: 0x00154950 File Offset: 0x00152B50
	protected override void InitItemDisableFunctionList(ItemDisplayData itemDisplayData)
	{
		base.InitItemDisableFunctionList(itemDisplayData);
		TeaWineItem configData = TeaWine.Instance[itemDisplayData.Key.TemplateId];
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

	// Token: 0x06002BA1 RID: 11169 RVA: 0x001549E8 File Offset: 0x00152BE8
	private void AddSolarTermPropertyItem(GameObject template, Transform parent, string icon, string propertyName, int value, bool isPlus = true)
	{
		GameObject obj = Object.Instantiate<GameObject>(template, parent);
		Refers refers = obj.GetComponent<Refers>();
		refers.CGet<CImage>("Icon").SetSprite(icon, false, null);
		refers.CGet<TextMeshProUGUI>("PropertyName").text = propertyName;
		refers.CGet<TextMeshProUGUI>("AddValue").text = (isPlus ? "+" : "-") + string.Format("{0}%", value + value * this._winePercent / 100);
		obj.SetActive(true);
		this._clonedObjects.Add(obj);
	}

	// Token: 0x06002BA2 RID: 11170 RVA: 0x00154A88 File Offset: 0x00152C88
	private SolarTermItem GetSolarTermConfig(sbyte type)
	{
		sbyte month = SingletonObject.getInstance<TimeManager>().GetMonthInCurrYear();
		foreach (SolarTermItem solar in ((IEnumerable<SolarTermItem>)SolarTerm.Instance))
		{
			bool flag = solar.Month == month && solar.Type == type;
			if (flag)
			{
				return solar;
			}
		}
		return null;
	}

	// Token: 0x06002BA3 RID: 11171 RVA: 0x00154B00 File Offset: 0x00152D00
	private void Clear()
	{
		foreach (GameObject obj in this._clonedObjects)
		{
			Object.Destroy(obj);
		}
		this._clonedObjects.Clear();
	}

	// Token: 0x04001FD4 RID: 8148
	private readonly List<GameObject> _clonedObjects = new List<GameObject>();

	// Token: 0x04001FD5 RID: 8149
	private readonly HashSet<short> _drunkWine = new HashSet<short>();

	// Token: 0x04001FD6 RID: 8150
	private int _winePercent = 0;
}
