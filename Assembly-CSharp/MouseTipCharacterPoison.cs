using System;
using Config;
using FrameWork;
using Game.Views.Encyclopedia;
using GameData.Combat.Math;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200027E RID: 638
public class MouseTipCharacterPoison : MouseTipBase
{
	// Token: 0x06002945 RID: 10565 RVA: 0x001343C0 File Offset: 0x001325C0
	protected override void Init(ArgumentBox argsBox)
	{
		this.Element.ForceListenCommand = true;
		sbyte poisonType;
		argsBox.Get("PoisonType", out poisonType);
		int poisonResist;
		argsBox.Get("PoisonResist", out poisonResist);
		int poisonValue;
		argsBox.Get("PoisonValue", out poisonValue);
		sbyte poisonLevel;
		argsBox.Get("PoisonLevel", out poisonLevel);
		bool isBornImmune;
		argsBox.Get("IsBornImmune", out isBornImmune);
		PoisonItem poisonConfig = Poison.Instance[poisonType];
		string poisonName = poisonConfig.Name;
		bool isImmune = poisonResist >= 1000;
		base.CGet<TextMeshProUGUI>("Title").text = poisonName;
		string resistType = (poisonResist < 0) ? LocalStringManager.Get(LanguageKey.LK_Value_Percent_Up) : LocalStringManager.Get(LanguageKey.LK_Value_Percent_Down);
		string resistColor = (poisonResist < 0) ? "brightred" : "brightblue";
		int resistPercent = Mathf.Min(100, CValuePercent.ParseInt(poisonResist, 1000));
		string content = isBornImmune ? LocalStringManager.GetFormat(LanguageKey.LK_MouseTipCharacterPoison_Resistance_ImmunitySinceBorn, poisonName, poisonResist) : (isImmune ? LocalStringManager.GetFormat(LanguageKey.LK_MouseTipCharacterPoison_Resistance_Immunity, poisonName, poisonResist) : LocalStringManager.GetFormat(LanguageKey.LK_MouseTipCharacterPoison_Resistance, poisonName, poisonResist, resistColor));
		base.CGet<TextMeshProUGUI>("Resistance").text = content.ColorReplace();
		base.CGet<GameObject>("ResistanceEffectLayout").SetActive(!isBornImmune && !isImmune);
		base.CGet<TextMeshProUGUI>("ResistanceEffect").text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTipCharacterPoison_Resistance_Effect, new object[]
		{
			poisonName,
			Mathf.Abs(resistPercent).ToString(),
			resistColor,
			resistType
		}).ColorReplace();
		base.CGet<TextMeshProUGUI>("Current").text = ((poisonValue > 0) ? LocalStringManager.GetFormat(LanguageKey.LK_MouseTipCharacterPoison_Current, poisonName, poisonValue, LocalStringManager.Get(string.Format("LK_Poison_Level_{0}", Mathf.Max((int)poisonLevel, 1)))).ColorReplace() : LocalStringManager.GetFormat(LanguageKey.LK_MouseTipCharacterPoison_Current_None, poisonName));
		int[] rates = this.GetPoisonRates((int)poisonLevel);
		base.CGet<TextMeshProUGUI>("Change").text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTipCharacterPoison_Change, rates[0], rates[1], rates[2]).ColorReplace();
		TextMeshProUGUI state = base.CGet<TextMeshProUGUI>("State");
		state.text = (isImmune ? LocalStringManager.GetFormat(LanguageKey.LK_MouseTipCharacterPoison_Immunity, poisonName.SetColor(poisonConfig.FontColor)).ColorReplace() : poisonConfig.Desc.ColorReplace());
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)state.transform.parent);
		});
	}

	// Token: 0x06002946 RID: 10566 RVA: 0x00134660 File Offset: 0x00132860
	private void Update()
	{
		bool flag = CommonCommandKit.PrimaryInteraction.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			ViewEncyclopediaPanel.OpenLink(EncyclopediaTipLink.DefValue.Toxin);
		}
	}

	// Token: 0x06002947 RID: 10567 RVA: 0x00134694 File Offset: 0x00132894
	private int[] GetPoisonRates(int level)
	{
		if (!true)
		{
		}
		int[] result;
		switch (level)
		{
		case 1:
			result = new int[]
			{
				33,
				100,
				100
			};
			break;
		case 2:
			result = new int[]
			{
				11,
				33,
				100
			};
			break;
		case 3:
			result = new int[]
			{
				0,
				11,
				33
			};
			break;
		default:
			result = new int[]
			{
				100,
				100,
				100
			};
			break;
		}
		if (!true)
		{
		}
		return result;
	}
}
