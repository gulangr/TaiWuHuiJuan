using System;
using System.Text;
using Config;
using FrameWork;
using GameData.Domains.Building;
using GameData.Domains.Combat;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

// Token: 0x020002DB RID: 731
public class MouseTipSundries : MouseTipItem
{
	// Token: 0x170004CD RID: 1229
	// (get) Token: 0x06002B7B RID: 11131 RVA: 0x00152C95 File Offset: 0x00150E95
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002B7C RID: 11132 RVA: 0x00152C98 File Offset: 0x00150E98
	protected override void Init(ArgumentBox argsBox)
	{
		base.Init(argsBox);
		this.InitRefers();
		ItemDisplayData itemData;
		argsBox.Get<ItemDisplayData>("ItemData", out itemData);
		this._itemData = itemData;
		bool templateDataOnly;
		argsBox.Get("TemplateDataOnly", out templateDataOnly);
		MiscItem configData = Misc.Instance[itemData.Key.TemplateId];
		this._goldThread.SetActive(itemData.Key.TemplateId == 275);
		this._name.text = configData.Name;
		this._gradeBack.SetSprite(ItemView.GetGradeIcon(configData.Grade), false, null);
		this._gradeName.text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", configData.Grade));
		this._grade.text = (LocalStringManager.Get(string.Format("LK_Num_{0}", (int)(9 - configData.Grade))) + LocalStringManager.Get(LanguageKey.LK_Item_Grade)).SetColor(Colors.Instance.GradeColors[(int)configData.Grade]);
		this._value.text = (templateDataOnly ? configData.BaseValue.ToString() : itemData.Value.ToString());
		this._durability.SetActive(!templateDataOnly);
		this._material.SetActive(!templateDataOnly);
		this._itemIcon.SetSprite(configData.Icon, false, null);
		base.SetItemDesc(configData.Desc, itemData.LoveTokenDataItem);
		this._subType.text = LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", configData.ItemSubType));
		this._weight.text = NumberFormatUtils.FormatItemWeight(itemData.Weight);
		this._durability.SetActive(itemData.MaxDurability > 0);
		bool flag = itemData.MaxDurability > 0;
		if (flag)
		{
			bool hasHalfDurability = itemData.Durability > itemData.MaxDurability / 2;
			this._currDurabilityYellow.gameObject.SetActive(hasHalfDurability);
			this._currDurabilityRed.gameObject.SetActive(!hasHalfDurability);
			(hasHalfDurability ? this._currDurabilityYellow : this._currDurabilityRed).text = itemData.Durability.ToString();
			this._maxDurability.text = string.Format("/{0}", itemData.MaxDurability);
		}
		this.InitItemDisableFunctionList(itemData);
		base.RefreshDisableFunction();
		this._eatEffect.SetActive(configData.Neili > 0);
		bool flag2 = configData.Neili > 0;
		if (flag2)
		{
			this._addNeili.text = configData.Neili.ToString();
		}
		bool canCastBossSkill = !templateDataOnly && GameData.Domains.Combat.SharedConstValue.SwordFragment2BossId.ContainsKey(itemData.Key.TemplateId) && itemData.SpecialArg >= 0;
		bool isFuyuSword = itemData.Key.TemplateId == 239;
		bool isTianSuiBaoLuItem = CommonUtils.IsTianSuiBaoLuItem(itemData.Key.ItemType, itemData.Key.TemplateId);
		this._useInCombat.SetActive(configData.ConsumedFeatureMedals > 0 || canCastBossSkill || isFuyuSword || isTianSuiBaoLuItem);
		bool flag3 = configData.ConsumedFeatureMedals > 0 || canCastBossSkill || isFuyuSword || isTianSuiBaoLuItem;
		if (flag3)
		{
			this._costWisdom.text = string.Format("x{0}", configData.ConsumedFeatureMedals);
			this._castBossSkill.SetActive(canCastBossSkill);
			this._bossSkillTrick.SetActive(canCastBossSkill);
			this._infectionTips.SetActive(canCastBossSkill);
			this._fuyuSwordTips.SetActive(isFuyuSword);
			this._tianSuiBaoLuEffect.SetActive(isTianSuiBaoLuItem);
			bool flag4 = canCastBossSkill;
			if (flag4)
			{
				CombatSkillItem skillConfig = CombatSkill.Instance[itemData.SpecialArg];
				StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
				strBuilder.Clear();
				for (int i = 0; i < skillConfig.TrickCost.Count; i++)
				{
					bool flag5 = i > 0;
					if (flag5)
					{
						strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Split_Symbol));
					}
					strBuilder.Append(Config.TrickType.Instance[skillConfig.TrickCost[i].TrickType].Name);
				}
				this._bossSkillName.text = skillConfig.Name.SetColor(Colors.Instance.GradeColors[(int)skillConfig.Grade]);
				this._requireWeaponTrick.text = strBuilder.ToString();
				EasyPool.Free<StringBuilder>(strBuilder);
			}
			else
			{
				bool flag6 = isTianSuiBaoLuItem;
				if (flag6)
				{
					this.RefreshTianSuiBaoLuEffect(itemData.Key.TemplateId);
				}
			}
		}
		base.RefreshHoldCount();
		base.RefreshHotkeyDisplayLockItem();
		this.RefershCricketJar(configData);
	}

	// Token: 0x06002B7D RID: 11133 RVA: 0x0015315C File Offset: 0x0015135C
	private void RefreshTianSuiBaoLuEffect(short templateId)
	{
		if (!true)
		{
		}
		LanguageKey languageKey;
		switch (templateId)
		{
		case 254:
			languageKey = LanguageKey.LK_CombatUseEffect_ShenJianSpiritWords;
			break;
		case 255:
			languageKey = LanguageKey.LK_CombatUseEffect_HuanSheSpiritWords;
			break;
		case 256:
			languageKey = LanguageKey.LK_CombatUseEffect_FuCangSpiritWords;
			break;
		case 257:
			languageKey = LanguageKey.LK_CombatUseEffect_YinShenSpiritWords;
			break;
		case 258:
			languageKey = LanguageKey.LK_CombatUseEffect_QuLiuWuSpiritWords;
			break;
		case 259:
			languageKey = LanguageKey.LK_CombatUseEffect_ShenJianMysteryWords;
			break;
		case 260:
			languageKey = LanguageKey.LK_CombatUseEffect_HuanSheMysteryWords;
			break;
		case 261:
			languageKey = LanguageKey.LK_CombatUseEffect_FuCangMysteryWords;
			break;
		case 262:
			languageKey = LanguageKey.LK_CombatUseEffect_YinShenMysteryWords;
			break;
		case 263:
			languageKey = LanguageKey.LK_CombatUseEffect_QuLiuWuMysteryWords;
			break;
		default:
			throw new ArgumentOutOfRangeException("templateId");
		}
		if (!true)
		{
		}
		LanguageKey key = languageKey;
		this._tianSuiBaoLuEffectLabel.text = LocalStringManager.Get(key);
	}

	// Token: 0x06002B7E RID: 11134 RVA: 0x00153218 File Offset: 0x00151418
	private void RefershCricketJar(MiscItem configData)
	{
		bool isCricketJar = configData.ItemSubType == 1201;
		this._cricketJarEffect.SetActive(isCricketJar);
		bool flag = !isCricketJar;
		if (!flag)
		{
			this._cricketHealPercent.text = string.Format("{0}%", configData.CricketHealInjuryOdds);
			this._cricketRegenDuration.text = SharedMethods.CalcCricketRegenTime(configData.Grade).ToString();
		}
	}

	// Token: 0x06002B7F RID: 11135 RVA: 0x0015328C File Offset: 0x0015148C
	protected override void InitItemDisableFunctionList(ItemDisplayData itemDisplayData)
	{
		base.InitItemDisableFunctionList(itemDisplayData);
		MiscItem configData = Misc.Instance[itemDisplayData.Key.TemplateId];
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

	// Token: 0x06002B80 RID: 11136 RVA: 0x00153324 File Offset: 0x00151524
	private void InitRefers()
	{
		this._name = base.CGet<TextMeshProUGUI>("Name");
		this._gradeBack = base.CGet<CImage>("GradeBack");
		this._gradeName = base.CGet<TextMeshProUGUI>("GradeName");
		this._grade = base.CGet<TextMeshProUGUI>("Grade");
		this._value = base.CGet<TextMeshProUGUI>("Value");
		this._itemIcon = base.CGet<CImage>("ItemIcon");
		this._desc = base.CGet<TextMeshProUGUI>("Desc");
		this._subType = base.CGet<TextMeshProUGUI>("SubType");
		this._currDurabilityYellow = base.CGet<TextMeshProUGUI>("CurrDurabilityYellow");
		this._currDurabilityRed = base.CGet<TextMeshProUGUI>("CurrDurabilityRed");
		this._maxDurability = base.CGet<TextMeshProUGUI>("MaxDurability");
		this._weight = base.CGet<TextMeshProUGUI>("Weight");
		this._eatEffect = base.CGet<GameObject>("EatEffect");
		this._addNeili = base.CGet<TextMeshProUGUI>("AddNeili");
		this._useInCombat = base.CGet<GameObject>("UseInCombat");
		this._costWisdom = base.CGet<TextMeshProUGUI>("CostWisdom");
		this._castBossSkill = base.CGet<GameObject>("CastBossSkill");
		this._bossSkillName = base.CGet<TextMeshProUGUI>("BossSkillName");
		this._bossSkillTrick = base.CGet<GameObject>("BossSkillTrick");
		this._requireWeaponTrick = base.CGet<TextMeshProUGUI>("RequireWeaponTrick");
		this._infectionTips = base.CGet<GameObject>("InfectionTips");
		this._fuyuSwordTips = base.CGet<GameObject>("FuyuSwordTips");
		this._material = base.CGet<GameObject>("Material");
		this._disableFunctionLayout = base.CGet<GameObject>("DisableFunctionLayout");
		this._durability = base.CGet<GameObject>("Durability");
		this._holdCountText = base.CGet<TextMeshProUGUI>("HoldCountText");
		this._goldThread = base.CGet<GameObject>("GoldThread");
		this._cricketJarEffect = base.CGet<GameObject>("CricketJarEffect");
		this._cricketRegenDuration = base.CGet<TextMeshProUGUI>("CricketRegenDuration");
		this._cricketHealPercent = base.CGet<TextMeshProUGUI>("CricketHealPercent");
		this._tianSuiBaoLuEffect = base.CGet<GameObject>("TianSuiBaoLuEffect");
		this._tianSuiBaoLuEffectLabel = base.CGet<TextMeshProUGUI>("TianSuiBaoLuEffectLabel");
	}

	// Token: 0x04001FA0 RID: 8096
	private TextMeshProUGUI _name;

	// Token: 0x04001FA1 RID: 8097
	private CImage _gradeBack;

	// Token: 0x04001FA2 RID: 8098
	private TextMeshProUGUI _gradeName;

	// Token: 0x04001FA3 RID: 8099
	private TextMeshProUGUI _grade;

	// Token: 0x04001FA4 RID: 8100
	private TextMeshProUGUI _value;

	// Token: 0x04001FA5 RID: 8101
	private CImage _itemIcon;

	// Token: 0x04001FA6 RID: 8102
	private TextMeshProUGUI _desc;

	// Token: 0x04001FA7 RID: 8103
	private TextMeshProUGUI _subType;

	// Token: 0x04001FA8 RID: 8104
	private TextMeshProUGUI _currDurabilityYellow;

	// Token: 0x04001FA9 RID: 8105
	private TextMeshProUGUI _currDurabilityRed;

	// Token: 0x04001FAA RID: 8106
	private TextMeshProUGUI _maxDurability;

	// Token: 0x04001FAB RID: 8107
	private TextMeshProUGUI _weight;

	// Token: 0x04001FAC RID: 8108
	private GameObject _eatEffect;

	// Token: 0x04001FAD RID: 8109
	private TextMeshProUGUI _addNeili;

	// Token: 0x04001FAE RID: 8110
	private GameObject _useInCombat;

	// Token: 0x04001FAF RID: 8111
	private TextMeshProUGUI _costWisdom;

	// Token: 0x04001FB0 RID: 8112
	private GameObject _castBossSkill;

	// Token: 0x04001FB1 RID: 8113
	private TextMeshProUGUI _bossSkillName;

	// Token: 0x04001FB2 RID: 8114
	private GameObject _bossSkillTrick;

	// Token: 0x04001FB3 RID: 8115
	private TextMeshProUGUI _requireWeaponTrick;

	// Token: 0x04001FB4 RID: 8116
	private GameObject _infectionTips;

	// Token: 0x04001FB5 RID: 8117
	private GameObject _fuyuSwordTips;

	// Token: 0x04001FB6 RID: 8118
	private GameObject _material;

	// Token: 0x04001FB7 RID: 8119
	private GameObject _disableFunctionLayout;

	// Token: 0x04001FB8 RID: 8120
	private GameObject _durability;

	// Token: 0x04001FB9 RID: 8121
	private TextMeshProUGUI _holdCountText;

	// Token: 0x04001FBA RID: 8122
	private GameObject _goldThread;

	// Token: 0x04001FBB RID: 8123
	private GameObject _cricketJarEffect;

	// Token: 0x04001FBC RID: 8124
	private TextMeshProUGUI _cricketRegenDuration;

	// Token: 0x04001FBD RID: 8125
	private TextMeshProUGUI _cricketHealPercent;

	// Token: 0x04001FBE RID: 8126
	private GameObject _tianSuiBaoLuEffect;

	// Token: 0x04001FBF RID: 8127
	private TextMeshProUGUI _tianSuiBaoLuEffectLabel;
}
