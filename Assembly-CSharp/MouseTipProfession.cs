using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002C7 RID: 711
public class MouseTipProfession : MouseTipBase
{
	// Token: 0x170004B3 RID: 1203
	// (get) Token: 0x06002AF8 RID: 11000 RVA: 0x0014B87F File Offset: 0x00149A7F
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002AF9 RID: 11001 RVA: 0x0014B882 File Offset: 0x00149A82
	protected override void Init(ArgumentBox argsBox)
	{
		this.Refresh(argsBox);
	}

	// Token: 0x06002AFA RID: 11002 RVA: 0x0014B88D File Offset: 0x00149A8D
	private void ReadArgs(ArgumentBox argsBox)
	{
		argsBox.Get("ProfessionId", out this._professionId);
	}

	// Token: 0x06002AFB RID: 11003 RVA: 0x0014B8A2 File Offset: 0x00149AA2
	public override void Refresh(ArgumentBox argsBox)
	{
		this.ReadArgs(argsBox);
		this.InitRefers();
		this.RefreshInner();
	}

	// Token: 0x06002AFC RID: 11004 RVA: 0x0014B8BC File Offset: 0x00149ABC
	private void RefreshInner()
	{
		this._config = Profession.Instance[this._professionId];
		this._titleLabel.text = this._config.Name;
		this._descLabel.text = this._config.Desc;
		this.RefreshSeniorityGains();
		TaiwuDomainMethod.AsyncCall.GetProfessionTipDisplayData(this, this._professionId, new AsyncMethodCallbackDelegate(this.OnGetProfessionTipDisplayData));
	}

	// Token: 0x06002AFD RID: 11005 RVA: 0x0014B930 File Offset: 0x00149B30
	private void OnGetProfessionTipDisplayData(int offset, RawDataPool pool)
	{
		ProfessionTipDisplayData displayData = new ProfessionTipDisplayData();
		Serializer.Deserialize(pool, offset, ref displayData);
		this.RefreshClothingArea(displayData);
		this.RefreshBonusArea(displayData);
	}

	// Token: 0x06002AFE RID: 11006 RVA: 0x0014B960 File Offset: 0x00149B60
	private void RefreshBonusArea(ProfessionTipDisplayData displayData)
	{
		int totalBonus = displayData.AttainmentBonus * displayData.ProfessionUpgradeBonus / 100;
		this._totalBonusLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_ProfessionTip_FinalBonus, MouseTipProfession.FormatPercent(totalBonus)).ColorReplace();
		this._attainmentBonusValue.text = LocalStringManager.GetFormat(LanguageKey.LK_ProfessionTip_AttainmentBonus, MouseTipProfession.FormatPercent(displayData.AttainmentBonus)).ColorReplace();
		string worldDifficultyName = WorldCreation.Instance[13].Options[displayData.ProfessionUpgrade];
		this._upgradeBonusLabel.text = (worldDifficultyName + MouseTipProfession.FormatPercent(displayData.ProfessionUpgradeBonus)).ColorReplace();
		this._upgradeIcon.SetSprite("mousetip_shijiexijie_13_" + displayData.ProfessionUpgrade.ToString(), false, null);
		this.RefreshAttainmentItems(displayData.WorkingSkillType);
	}

	// Token: 0x06002AFF RID: 11007 RVA: 0x0014BA34 File Offset: 0x00149C34
	private void RefreshAttainmentItems(sbyte workingSkillType)
	{
		string sepChar = LocalStringManager.Get(LanguageKey.LK_Separator);
		List<sbyte> types = this._config.BonusLifeSkills;
		bool isCombatSkill = false;
		bool flag = types.Count == 0;
		if (flag)
		{
			types = this._config.BonusCombatSkills;
			isCombatSkill = true;
		}
		for (int i = 0; i < this._attainmentLayoutList.Count; i++)
		{
			this._attainmentAreaList[i].SetActive(false);
		}
		for (int j = 0; j < types.Count; j++)
		{
			int groupIndex = j / 5;
			RectTransform layout = this._attainmentLayoutList[groupIndex];
			bool flag2 = j % 5 == 0;
			if (flag2)
			{
				this._attainmentAreaList[groupIndex].SetActive(true);
				int itemCountInThisLine = Math.Clamp(types.Count - groupIndex * 5, 0, 5);
				CommonUtils.PrepareEnoughChildren(layout, this._attainmentTemplate.gameObject, itemCountInThisLine, null);
			}
			Transform item = layout.GetChild(j % 5);
			Refers refers = item.GetComponent<Refers>();
			CImage icon = refers.CGet<CImage>("Icon");
			TextMeshProUGUI label = refers.CGet<TextMeshProUGUI>("Label");
			sbyte type = types[j];
			icon.SetSprite((isCombatSkill ? "mousetip_gongfa_" : "mousetip_jiyi_") + type.ToString(), false, null);
			string skillName = isCombatSkill ? CombatSkillType.Instance[type].Name : LifeSkillType.Instance[type].Name;
			label.text = skillName + ((j == types.Count - 1) ? string.Empty : sepChar);
			item.GetComponent<DisableStyleRoot>().SetStyleEffect(type != workingSkillType, false);
		}
	}

	// Token: 0x06002B00 RID: 11008 RVA: 0x0014BC00 File Offset: 0x00149E00
	private void RefreshClothingArea(ProfessionTipDisplayData displayData)
	{
		short clothing = this._config.BonusClothing;
		ClothingItem clothingConfig = Clothing.Instance[clothing];
		string clothName = clothingConfig.Name.SetGradeColor((int)clothingConfig.Grade);
		this._clothBonusLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_ProfessionTip_ClothBonus, clothName).ColorReplace();
		this._clothBonusLabel.GetComponent<DisableStyleRoot>().SetStyleEffect(!displayData.IsWearingBonusClothing, false);
	}

	// Token: 0x06002B01 RID: 11009 RVA: 0x0014BC70 File Offset: 0x00149E70
	private void RefreshSeniorityGains()
	{
		CommonUtils.PrepareEnoughChildren(this._seniorityGainTips.transform, this._seniorityGainTemplate.gameObject, this._config.SeniorityGainTips.Length, null);
		int i = 0;
		while (i < this._config.SeniorityGainTips.Length)
		{
			uint? dlcId = (i < this._config.SeniorityGainTipsDlcId.Length) ? new uint?(this._config.SeniorityGainTipsDlcId[i]) : null;
			if (dlcId == null)
			{
				goto IL_A7;
			}
			uint? num = dlcId;
			uint num2 = 0U;
			if (num.GetValueOrDefault() == num2 & num != null)
			{
				goto IL_A7;
			}
			bool flag = SingletonObject.getInstance<DlcManager>().IsDlcInstalled(dlcId.Value);
			IL_A8:
			bool isDlcInstalled = flag;
			Transform tip = this._seniorityGainTips.GetChild(i);
			bool flag2 = !isDlcInstalled;
			if (flag2)
			{
				tip.gameObject.SetActive(false);
			}
			else
			{
				Refers refers = tip.GetComponent<Refers>();
				TextMeshProUGUI label = refers.CGet<TextMeshProUGUI>("Label");
				label.text = this._config.SeniorityGainTips[i].ColorReplace();
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
				{
					float labelHeight = label.GetComponent<RectTransform>().rect.height;
					LayoutElement layoutElement = refers.GetComponent<LayoutElement>();
					layoutElement.preferredHeight = labelHeight;
				});
			}
			i++;
			continue;
			IL_A7:
			flag = true;
			goto IL_A8;
		}
	}

	// Token: 0x06002B02 RID: 11010 RVA: 0x0014BDC8 File Offset: 0x00149FC8
	private static string FormatPercent(int percent)
	{
		string color = (percent > 100) ? "brightblue" : ((percent < 100) ? "brightred" : "pinkyellow");
		return string.Format("{0}%", percent).SetColor(color);
	}

	// Token: 0x06002B03 RID: 11011 RVA: 0x0014BE10 File Offset: 0x0014A010
	private void InitRefers()
	{
		this._attainmentAreaList = base.CGetList<GameObject>("AttainmentArea_");
		this._attainmentLayoutList = base.CGetList<RectTransform>("AttainmentLayout_");
		this._upgradeIcon = base.CGet<CImage>("UpgradeIcon");
		this._clothingArea = base.CGet<GameObject>("ClothingArea");
		this._seniorityGainTips = base.CGet<RectTransform>("SeniorityGainTips");
		this._attainmentTemplate = base.CGet<Refers>("AttainmentTemplate");
		this._seniorityGainTemplate = base.CGet<Refers>("SeniorityGainTemplate");
		this._attainmentBonusValue = base.CGet<TextMeshProUGUI>("AttainmentBonusValue");
		this._clothBonusLabel = base.CGet<TextMeshProUGUI>("ClothBonusLabel");
		this._descLabel = base.CGet<TextMeshProUGUI>("DescLabel");
		this._titleLabel = base.CGet<TextMeshProUGUI>("TitleLabel");
		this._totalBonusLabel = base.CGet<TextMeshProUGUI>("TotalBonusLabel");
		this._upgradeBonusLabel = base.CGet<TextMeshProUGUI>("UpgradeBonusLabel");
	}

	// Token: 0x04001F09 RID: 7945
	private int _professionId;

	// Token: 0x04001F0A RID: 7946
	private ProfessionItem _config;

	// Token: 0x04001F0B RID: 7947
	private List<GameObject> _attainmentAreaList;

	// Token: 0x04001F0C RID: 7948
	private List<RectTransform> _attainmentLayoutList;

	// Token: 0x04001F0D RID: 7949
	private CImage _upgradeIcon;

	// Token: 0x04001F0E RID: 7950
	private GameObject _clothingArea;

	// Token: 0x04001F0F RID: 7951
	private RectTransform _seniorityGainTips;

	// Token: 0x04001F10 RID: 7952
	private Refers _attainmentTemplate;

	// Token: 0x04001F11 RID: 7953
	private Refers _seniorityGainTemplate;

	// Token: 0x04001F12 RID: 7954
	private TextMeshProUGUI _attainmentBonusValue;

	// Token: 0x04001F13 RID: 7955
	private TextMeshProUGUI _clothBonusLabel;

	// Token: 0x04001F14 RID: 7956
	private TextMeshProUGUI _descLabel;

	// Token: 0x04001F15 RID: 7957
	private TextMeshProUGUI _titleLabel;

	// Token: 0x04001F16 RID: 7958
	private TextMeshProUGUI _totalBonusLabel;

	// Token: 0x04001F17 RID: 7959
	private TextMeshProUGUI _upgradeBonusLabel;
}
