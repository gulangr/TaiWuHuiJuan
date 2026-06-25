using System;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x0200026C RID: 620
public class MouseTipActiveLoop : MouseTipBase
{
	// Token: 0x1700047A RID: 1146
	// (get) Token: 0x060028EC RID: 10476 RVA: 0x0012F8F4 File Offset: 0x0012DAF4
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060028ED RID: 10477 RVA: 0x0012F8F7 File Offset: 0x0012DAF7
	protected override void Init(ArgumentBox argsBox)
	{
		this.Refresh(argsBox);
	}

	// Token: 0x060028EE RID: 10478 RVA: 0x0012F904 File Offset: 0x0012DB04
	public override void Refresh(ArgumentBox argBox)
	{
		this.InitRefers();
		string dot = LocalStringManager.Get(LanguageKey.LK_Dot_Symbol);
		this._title.text = LocalStringManager.Get(LanguageKey.LK_ActiveLoop_Tip_Title).ColorReplace();
		this._desc1.text = LocalStringManager.Get(LanguageKey.LK_ActiveLoop_Tip_Desc1).ColorReplace();
		this._title1.text = LocalStringManager.Get(LanguageKey.LK_ActiveLoop_Tip_CostTitle).ColorReplace();
		this._desc2.text = dot + LocalStringManager.Get(LanguageKey.LK_ActiveLoop_Tip_Desc2).ColorReplace();
		this._desc9.text = dot + LocalStringManager.Get(LanguageKey.LK_ActiveLoop_Tip_Desc4).ColorReplace();
		this._desc6.text = LocalStringManager.Get(LanguageKey.LK_ActiveLoop_Tip_NotEnough1).ColorReplace();
		this._desc7.text = LocalStringManager.Get(LanguageKey.LK_ActiveLoop_Tip_NotEnough2).ColorReplace();
		this._desc8.text = LocalStringManager.Get(LanguageKey.LK_ActiveLoop_Tip_NotEnough3).ColorReplace();
		bool showCost6;
		argBox.Get("ShowCost6", out showCost6);
		this._descHolder6.SetActive(showCost6);
		bool showCost7;
		argBox.Get("ShowCost7", out showCost7);
		this._descHolder7.SetActive(showCost7);
		bool showCost8;
		argBox.Get("ShowCost8", out showCost8);
		this._descHolder8.SetActive(showCost8);
		bool showCost9;
		argBox.Get("ShowCost10", out showCost9);
		this._descHolder10.SetActive(showCost9);
		string cost;
		argBox.Get("Cost1", out cost);
		this._sub1Text.text = cost.ColorReplace();
		string cost2;
		argBox.Get("Cost2", out cost2);
		this._sub2Text.text = cost2.ColorReplace();
	}

	// Token: 0x060028EF RID: 10479 RVA: 0x0012FAB8 File Offset: 0x0012DCB8
	private void InitRefers()
	{
		this._title = base.CGet<TextMeshProUGUI>("Title");
		this._desc1 = base.CGet<TextMeshProUGUI>("Desc1");
		this._title1 = base.CGet<TextMeshProUGUI>("Title1");
		this._sub1Text = base.CGet<TextMeshProUGUI>("Sub1Text");
		this._sub2Text = base.CGet<TextMeshProUGUI>("Sub2Text");
		this._desc2 = base.CGet<TextMeshProUGUI>("Desc2");
		this._desc6 = base.CGet<TextMeshProUGUI>("Desc6");
		this._desc7 = base.CGet<TextMeshProUGUI>("Desc7");
		this._desc8 = base.CGet<TextMeshProUGUI>("Desc8");
		this._descHolder6 = base.CGet<GameObject>("DescHolder6");
		this._descHolder7 = base.CGet<GameObject>("DescHolder7");
		this._descHolder8 = base.CGet<GameObject>("DescHolder8");
		this._desc9 = base.CGet<TextMeshProUGUI>("Desc9");
		this._descHolder10 = base.CGet<GameObject>("DescHolder10");
	}

	// Token: 0x04001DC9 RID: 7625
	private TextMeshProUGUI _title;

	// Token: 0x04001DCA RID: 7626
	private TextMeshProUGUI _desc1;

	// Token: 0x04001DCB RID: 7627
	private TextMeshProUGUI _title1;

	// Token: 0x04001DCC RID: 7628
	private TextMeshProUGUI _sub1Text;

	// Token: 0x04001DCD RID: 7629
	private TextMeshProUGUI _sub2Text;

	// Token: 0x04001DCE RID: 7630
	private TextMeshProUGUI _desc2;

	// Token: 0x04001DCF RID: 7631
	private TextMeshProUGUI _desc6;

	// Token: 0x04001DD0 RID: 7632
	private TextMeshProUGUI _desc7;

	// Token: 0x04001DD1 RID: 7633
	private TextMeshProUGUI _desc8;

	// Token: 0x04001DD2 RID: 7634
	private GameObject _descHolder6;

	// Token: 0x04001DD3 RID: 7635
	private GameObject _descHolder7;

	// Token: 0x04001DD4 RID: 7636
	private GameObject _descHolder8;

	// Token: 0x04001DD5 RID: 7637
	private TextMeshProUGUI _desc9;

	// Token: 0x04001DD6 RID: 7638
	private GameObject _descHolder10;
}
