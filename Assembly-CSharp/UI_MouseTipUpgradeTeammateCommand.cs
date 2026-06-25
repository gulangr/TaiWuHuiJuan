using System;
using Config;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x020002EC RID: 748
public class UI_MouseTipUpgradeTeammateCommand : MouseTipBase
{
	// Token: 0x06002BFA RID: 11258 RVA: 0x001583DC File Offset: 0x001565DC
	protected override void Init(ArgumentBox argsBox)
	{
		int id;
		argsBox.Get("Id", out id);
		int medal;
		argsBox.Get("Medal", out medal);
		int needMedal;
		argsBox.Get("NeedMedal", out needMedal);
		int authority;
		argsBox.Get("Authority", out authority);
		int needAuthority;
		argsBox.Get("NeedAuthority", out needAuthority);
		bool isContained;
		argsBox.Get("IsContained", out isContained);
		int needTime;
		argsBox.Get("NeedTime", out needTime);
		TeammateCommandItem config = TeammateCommand.Instance[id];
		base.CGet<TextMeshProUGUI>("Title").text = config.Name;
		base.CGet<TextMeshProUGUI>("Content").text = config.Description.ColorReplace();
		Refers medalRefers = base.CGet<Refers>("Medal");
		sbyte medalType = config.MedalType;
		if (!true)
		{
		}
		string text;
		switch (medalType)
		{
		case 0:
			text = ((medal < 0) ? UI_MouseTipUpgradeTeammateCommand.FeatureIconConfig[1][0] : ((medal > 0) ? UI_MouseTipUpgradeTeammateCommand.FeatureIconConfig[0][0] : UI_MouseTipUpgradeTeammateCommand.FeatureIconConfig[3][0]));
			break;
		case 1:
			text = ((medal < 0) ? UI_MouseTipUpgradeTeammateCommand.FeatureIconConfig[1][1] : ((medal > 0) ? UI_MouseTipUpgradeTeammateCommand.FeatureIconConfig[0][1] : UI_MouseTipUpgradeTeammateCommand.FeatureIconConfig[3][1]));
			break;
		case 2:
			text = ((medal < 0) ? UI_MouseTipUpgradeTeammateCommand.FeatureIconConfig[1][2] : ((medal > 0) ? UI_MouseTipUpgradeTeammateCommand.FeatureIconConfig[0][2] : UI_MouseTipUpgradeTeammateCommand.FeatureIconConfig[3][2]));
			break;
		default:
			if (!true)
			{
			}
			<PrivateImplementationDetails>.ThrowSwitchExpressionException(medalType);
			break;
		}
		if (!true)
		{
		}
		string medalIconName = text;
		medalRefers.CGet<CImage>("Icon").SetSprite(medalIconName, false, null);
		sbyte medalType2 = config.MedalType;
		if (!true)
		{
		}
		LanguageKey languageKey;
		switch (medalType2)
		{
		case 0:
			languageKey = LanguageKey.LK_Feature_Attack;
			break;
		case 1:
			languageKey = LanguageKey.LK_Feature_Defence;
			break;
		case 2:
			languageKey = LanguageKey.LK_Feature_Wisdom;
			break;
		default:
			if (!true)
			{
			}
			<PrivateImplementationDetails>.ThrowSwitchExpressionException(medalType2);
			break;
		}
		if (!true)
		{
		}
		LanguageKey medalNameKey = languageKey;
		medalRefers.CGet<TextMeshProUGUI>("Name").SetText(LocalStringManager.Get(medalNameKey), true);
		string medalValueColor = (Mathf.Abs(medal) >= needMedal) ? "brightblue" : "brightred";
		string medalValue = string.Format("{0}/{1}", Mathf.Abs(medal).ToString().SetColor(medalValueColor), needMedal);
		medalRefers.CGet<TextMeshProUGUI>("Value").SetText(medalValue, true);
		Refers authorityRefers = base.CGet<Refers>("Authority");
		authorityRefers.CGet<CImage>("Icon").SetSprite("mousetip_ziyuan_7", false, null);
		authorityRefers.CGet<TextMeshProUGUI>("Name").SetText(ResourceType.Instance[7].Name, true);
		string authorityValueColor = (authority >= needAuthority) ? "brightblue" : "brightred";
		string authorityValue = string.Format("{0}/{1}", CommonUtils.GetDisplayStringForNum(authority, 100000).SetColor(authorityValueColor), needAuthority);
		authorityRefers.CGet<TextMeshProUGUI>("Value").SetText(authorityValue, true);
		Refers timeRefers = base.CGet<Refers>("Time");
		int time = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
		string timeValueColor = (time >= needTime) ? "brightblue" : "brightred";
		timeRefers.CGet<TextMeshProUGUI>("Value").text = LocalStringManager.GetFormat(LanguageKey.LK_Make_Resource_Require_Meet, time.ToString().SetColor(timeValueColor), needTime);
		Refers containRefers = base.CGet<Refers>("Contain");
		containRefers.gameObject.SetActive(isContained);
		bool flag = isContained;
		if (flag)
		{
			containRefers.CGet<TextMeshProUGUI>("Name").text = LocalStringManager.GetFormat(LanguageKey.LK_UpgradeTeammateCommand_Upgrade_Tip_NotContain, config.Name).SetColor("brightred");
		}
	}

	// Token: 0x04001FEB RID: 8171
	private static readonly string[][] FeatureIconConfig = new string[][]
	{
		new string[]
		{
			"mousetip_characteristic_10",
			"mousetip_characteristic_9",
			"mousetip_characteristic_11"
		},
		new string[]
		{
			"mousetip_characteristic_4",
			"mousetip_characteristic_3",
			"mousetip_characteristic_5"
		},
		new string[]
		{
			"mousetip_characteristic_1",
			"mousetip_characteristic_0",
			"mousetip_characteristic_2"
		},
		new string[]
		{
			"mousetip_characteristic_7",
			"mousetip_characteristic_6",
			"mousetip_characteristic_8"
		}
	};
}
