using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using TMPro;

// Token: 0x020002B8 RID: 696
public class MouseTipLoopProgress : MouseTipBase
{
	// Token: 0x06002ABE RID: 10942 RVA: 0x001473F8 File Offset: 0x001455F8
	protected override void Init(ArgumentBox argsBox)
	{
		this._title = base.CGet<TextMeshProUGUI>("Title");
		this._neiliText = base.CGet<TextMeshProUGUI>("NeiliText");
		this._fiveElementText = base.CGet<TextMeshProUGUI>("FiveElementText");
		this._extraNeiliAllocationProgressTextList = base.CGetList<TextMeshProUGUI>("ExtraNeiliAllocationProgressText_");
		this.Refresh(argsBox);
	}

	// Token: 0x06002ABF RID: 10943 RVA: 0x00147454 File Offset: 0x00145654
	public override void Refresh(ArgumentBox argBox)
	{
		this._title.text = LocalStringManager.Get(LanguageKey.LK_MouseTipLoopProgress_Title);
		bool hasLoopingNeigong;
		argBox.Get("HasLoopingNeigong", out hasLoopingNeigong);
		short obtainedNeili;
		short maxNeili;
		bool flag = hasLoopingNeigong && argBox.Get("ObtainedNeili", out obtainedNeili) && argBox.Get("MaxNeili", out maxNeili);
		if (flag)
		{
			string color = (obtainedNeili >= maxNeili) ? "brightblue" : "brightred";
			string coloredObtainedNeili = string.Format("<color=#{0}>{1}</color>", color, obtainedNeili);
			string text = string.Format("{0}/<color=#pinkyellow>{1}</color>", coloredObtainedNeili, maxNeili);
			this._neiliText.text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTipLoopProgress_Neili, text).ColorReplace();
		}
		else
		{
			this._neiliText.text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTipLoopProgress_Neili, "-").ColorReplace();
		}
		NeiliTypeItem neiliTypeConfig;
		bool fiveElementOk;
		bool flag2 = hasLoopingNeigong && argBox.Get<NeiliTypeItem>("NeiliTypeConfig", out neiliTypeConfig) && argBox.Get("FiveElementOk", out fiveElementOk);
		if (flag2)
		{
			string color2 = fiveElementOk ? "brightblue" : "brightred";
			string coloredFiveElement = string.Concat(new string[]
			{
				"<color=#",
				color2,
				">",
				neiliTypeConfig.Name,
				"</color>"
			});
			this._fiveElementText.text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTipLoopProgress_FiveElements, coloredFiveElement).ColorReplace();
		}
		else
		{
			this._fiveElementText.text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTipLoopProgress_FiveElements, "-").ColorReplace();
		}
		int[] progress;
		bool flag3 = argBox.Get<int[]>("TaiwuExtraNeiliAllocationProgress", out progress);
		if (flag3)
		{
			int maxProgress = LoopingCommonUtils.GetNeiliAllocationMaxProgress();
			for (int i = 0; i < 4; i++)
			{
				string color3 = (progress[i] >= maxProgress) ? "brightblue" : "brightred";
				int extraNeiliAllocation = LoopingCommonUtils.CalcExtraNeiliAllocationFromProgress(progress[i]);
				string coloredExtraNeiliAllocation = string.Format("<color=#{0}>{1}</color>", color3, extraNeiliAllocation);
				string text2 = string.Format("{0}/<color=#pinkyellow>{1}</color>", coloredExtraNeiliAllocation, GlobalConfig.Instance.MaxExtraNeiliAllocation);
				this._extraNeiliAllocationProgressTextList[i].text = LocalStringManager.GetFormat("LK_MouseTipLoopProgress_NeiliAllocation_" + i.ToString(), text2).ColorReplace();
			}
		}
		else
		{
			for (int j = 0; j < 4; j++)
			{
				this._extraNeiliAllocationProgressTextList[j].text = LocalStringManager.GetFormat("LK_MouseTipLoopProgress_NeiliAllocation_" + j.ToString(), "-");
			}
		}
	}

	// Token: 0x04001EEE RID: 7918
	private TextMeshProUGUI _title;

	// Token: 0x04001EEF RID: 7919
	private TextMeshProUGUI _neiliText;

	// Token: 0x04001EF0 RID: 7920
	private TextMeshProUGUI _fiveElementText;

	// Token: 0x04001EF1 RID: 7921
	private List<TextMeshProUGUI> _extraNeiliAllocationProgressTextList;
}
