using System;
using FrameWork;
using GameData.Domains.Extra;
using TMPro;
using UnityEngine;

// Token: 0x020002E1 RID: 737
public class MouseTipThreeVitals : MouseTipBase
{
	// Token: 0x06002BA5 RID: 11173 RVA: 0x00154B8C File Offset: 0x00152D8C
	protected override void Init(ArgumentBox argsBox)
	{
		argsBox.Get("isGoodEnd", out this._isGoodEnd);
		argsBox.Get<SectStoryThreeVitalsCharacter>("vitalData", out this._vitalData);
		LanguageKey descKey = this._isGoodEnd ? LanguageKey.LK_ThreeVitals_Vital_Tip_Content_Good : LanguageKey.LK_ThreeVitals_Vital_Tip_Content_Bad;
		base.CGet<TextMeshProUGUI>("Desc").text = LocalStringManager.Get(descKey).ColorReplace();
		LanguageKey descDetail = this._isGoodEnd ? LanguageKey.LK_ThreeVitals_Vital_Tip_Content_GoodDetail : LanguageKey.LK_ThreeVitals_Vital_Tip_Content_BadDetail;
		TextMeshProUGUI descDetailText = base.CGet<TextMeshProUGUI>("DescDetail");
		descDetailText.text = LocalStringManager.Get(descDetail).ColorReplace();
		descDetailText.GetComponent<TMPTextSpriteHelper>().Parse();
		int helpThreshold = this._isGoodEnd ? GlobalConfig.Instance.ThreeVitalsThresholdHigh : GlobalConfig.Instance.ThreeVitalsThresholdLow;
		bool isHelp = UI_ThreeVitals.IsVitalHelping(this._isGoodEnd, this._vitalData.Infection);
		DisableStyleRoot dangerLayout = base.CGet<DisableStyleRoot>("DangerLayout");
		DisableStyleRoot helpLayout = base.CGet<DisableStyleRoot>("HelpLayout");
		dangerLayout.SetStyleEffect(isHelp, false);
		helpLayout.SetStyleEffect(!isHelp, false);
		helpLayout.transform.SetSiblingIndex(this._isGoodEnd ? 1 : 2);
		int dangerStart = this._isGoodEnd ? helpThreshold : 0;
		int dangerEnd = this._isGoodEnd ? GlobalConfig.Instance.ThreeVitalsMaxInfection : helpThreshold;
		base.CGet<TextMeshProUGUI>("DangerRange").text = LocalStringManager.GetFormat(LanguageKey.LK_ThreeVitals_Vital_Tip_Infection_Range, dangerStart, dangerEnd).ColorReplace();
		LanguageKey dangerEffectKey = this._isGoodEnd ? LanguageKey.LK_ThreeVitals_Vital_Tip_Infection_Danger_Effect_Good : LanguageKey.LK_ThreeVitals_Vital_Tip_Infection_Danger_Effect_Bad;
		base.CGet<TextMeshProUGUI>("DangerEffect").text = LocalStringManager.Get(dangerEffectKey).ColorReplace();
		int helpStart = this._isGoodEnd ? 0 : (helpThreshold + 1);
		int helpEnd = this._isGoodEnd ? (helpThreshold - 1) : GlobalConfig.Instance.ThreeVitalsMaxInfection;
		base.CGet<TextMeshProUGUI>("HelpRange").text = LocalStringManager.GetFormat(LanguageKey.LK_ThreeVitals_Vital_Tip_Infection_Range, helpStart, helpEnd).ColorReplace();
		LanguageKey helpEffectKey = this._isGoodEnd ? LanguageKey.LK_ThreeVitals_Vital_Tip_Infection_Help_Effect_Good : LanguageKey.LK_ThreeVitals_Vital_Tip_Infection_Help_Effect_Bad;
		base.CGet<TextMeshProUGUI>("HelpEffect").text = LocalStringManager.Get(helpEffectKey).ColorReplace();
		base.CGet<GameObject>("EffectLayout").SetActive(!isHelp);
		bool flag = !isHelp;
		if (flag)
		{
			int betrayOdds = this._vitalData.CalcBetrayOdds(!this._isGoodEnd);
			base.CGet<TextMeshProUGUI>("Effect").text = LocalStringManager.GetFormat(LanguageKey.LK_ThreeVitals_Vital_Tip_Effect_Danger, betrayOdds).ColorReplace();
		}
	}

	// Token: 0x04001FD7 RID: 8151
	private bool _isGoodEnd;

	// Token: 0x04001FD8 RID: 8152
	private SectStoryThreeVitalsCharacter _vitalData;
}
