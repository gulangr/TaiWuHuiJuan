using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Config;
using FrameWork;
using Game.Views.Encyclopedia;
using TMPro;
using UnityEngine;

// Token: 0x0200029B RID: 667
public class MouseTipFiveElements : MouseTipBase
{
	// Token: 0x06002A18 RID: 10776 RVA: 0x00140E54 File Offset: 0x0013F054
	protected override void Init(ArgumentBox argsBox)
	{
		this.Element.ForceListenCommand = true;
		int neiliType;
		argsBox.Get("neiliType", out neiliType);
		NeiliTypeItem config = NeiliType.Instance[neiliType];
		base.CGet<TextMeshProUGUI>("Title").text = config.Name.ColorReplace();
		base.CGet<TextMeshProUGUI>("Desc").text = config.SimpleDesc.ColorReplace();
		base.CGet<TextMeshProUGUI>("Effect").text = config.EffectDesc.ColorReplace();
		Refers conditionTemplate = base.CGet<Refers>("Condition");
		RectTransform conditionContent = base.CGet<RectTransform>("ConditionContent");
		conditionContent.gameObject.SetActive(true);
		for (int i = 0; i < conditionContent.childCount; i++)
		{
			conditionContent.GetChild(i).gameObject.SetActive(false);
		}
		List<string> neiliTypeConditionTexts = config.NeiliTypeConditionText;
		bool flag = neiliTypeConditionTexts.Count > 1;
		if (flag)
		{
			for (int j = 0; j < neiliTypeConditionTexts.Count; j++)
			{
				bool flag2 = j >= conditionContent.childCount;
				Refers conditionRefers;
				if (flag2)
				{
					conditionRefers = Object.Instantiate<Refers>(conditionTemplate, conditionContent, false);
				}
				else
				{
					conditionRefers = conditionContent.GetChild(j).GetComponent<Refers>();
				}
				string text = Regex.Unescape(neiliTypeConditionTexts[j]);
				conditionRefers.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.GetFormat(LanguageKey.LK_FIveElements_NeiliCondition_Multiply, LocalStringManager.Get(string.Format("LK_Number{0}", j + 1)));
				conditionRefers.CGet<TextMeshProUGUI>("Tips").text = text.ColorReplace();
				conditionRefers.gameObject.SetActive(true);
			}
		}
		else
		{
			bool flag3 = neiliTypeConditionTexts.Count == 1;
			if (flag3)
			{
				string text2 = Regex.Unescape(neiliTypeConditionTexts[0]);
				conditionTemplate.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.Get(LanguageKey.LK_FIveElements_NeiliCondition);
				conditionTemplate.CGet<TextMeshProUGUI>("Tips").text = text2.ColorReplace();
				conditionTemplate.gameObject.SetActive(true);
			}
			else
			{
				conditionContent.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06002A19 RID: 10777 RVA: 0x00141086 File Offset: 0x0013F286
	public override void Refresh(ArgumentBox argBox)
	{
		this.Init(argBox);
	}

	// Token: 0x06002A1A RID: 10778 RVA: 0x00141094 File Offset: 0x0013F294
	private void Update()
	{
		bool flag = CommonCommandKit.PrimaryInteraction.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			ViewEncyclopediaPanel.OpenLink(EncyclopediaTipLink.DefValue.QiAttributes);
		}
	}
}
