using System;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x0200026D RID: 621
public class MouseTipActiveRead : MouseTipBase
{
	// Token: 0x1700047B RID: 1147
	// (get) Token: 0x060028F1 RID: 10481 RVA: 0x0012FBBD File Offset: 0x0012DDBD
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060028F2 RID: 10482 RVA: 0x0012FBC0 File Offset: 0x0012DDC0
	protected override void Init(ArgumentBox argsBox)
	{
		this.InitRefers();
		this.Refresh(argsBox);
	}

	// Token: 0x060028F3 RID: 10483 RVA: 0x0012FBD4 File Offset: 0x0012DDD4
	private void InitRefers()
	{
		this._title = base.CGet<TextMeshProUGUI>("Title");
		this._desc1 = base.CGet<TextMeshProUGUI>("Desc1");
		this._title1 = base.CGet<TextMeshProUGUI>("Title1");
		this._sub1Text = base.CGet<TextMeshProUGUI>("Sub1Text");
		this._sub2Text = base.CGet<TextMeshProUGUI>("Sub2Text");
		this._desc2 = base.CGet<TextMeshProUGUI>("Desc2");
		this._desc3 = base.CGet<TextMeshProUGUI>("Desc3");
		this._desc4 = base.CGet<TextMeshProUGUI>("Desc4");
		this._desc6 = base.CGet<TextMeshProUGUI>("Desc6");
		this._desc7 = base.CGet<TextMeshProUGUI>("Desc7");
		this._desc8 = base.CGet<TextMeshProUGUI>("Desc8");
		this._descHolder6 = base.CGet<GameObject>("DescHolder6");
		this._descHolder7 = base.CGet<GameObject>("DescHolder7");
		this._descHolder8 = base.CGet<GameObject>("DescHolder8");
		this._desc9 = base.CGet<TextMeshProUGUI>("Desc9");
		this._descHolder10 = base.CGet<GameObject>("DescHolder10");
	}

	// Token: 0x060028F4 RID: 10484 RVA: 0x0012FCF4 File Offset: 0x0012DEF4
	public override void Refresh(ArgumentBox argBox)
	{
		string dot = LocalStringManager.Get(LanguageKey.LK_Dot_Symbol);
		this._title.text = LocalStringManager.Get(LanguageKey.LK_ActiveRead_Tip_Title).ColorReplace();
		this._desc1.text = LocalStringManager.Get(LanguageKey.LK_ActiveRead_Tip_Desc1).ColorReplace();
		this._title1.text = LocalStringManager.Get(LanguageKey.LK_ActiveRead_Tip_CostTitle).ColorReplace();
		this._desc2.text = dot + LocalStringManager.Get(LanguageKey.LK_ActiveRead_Tip_Desc2).ColorReplace();
		this._desc3.text = dot + LocalStringManager.Get(LanguageKey.LK_ActiveRead_Tip_Desc3).ColorReplace();
		this._desc4.text = dot + LocalStringManager.Get(LanguageKey.LK_ActiveRead_Tip_Desc4).ColorReplace();
		this._desc9.text = dot + LocalStringManager.Get(LanguageKey.LK_ActiveRead_Tip_Desc6).ColorReplace();
		this._desc6.text = LocalStringManager.Get(LanguageKey.LK_ActiveRead_Tip_NotEnough1).ColorReplace();
		this._desc7.text = LocalStringManager.Get(LanguageKey.LK_ActiveRead_Tip_NotEnough2).ColorReplace();
		this._desc8.text = LocalStringManager.Get(LanguageKey.LK_ActiveRead_Tip_NotEnough3).ColorReplace();
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

	// Token: 0x04001DD7 RID: 7639
	private TextMeshProUGUI _title;

	// Token: 0x04001DD8 RID: 7640
	private TextMeshProUGUI _desc1;

	// Token: 0x04001DD9 RID: 7641
	private TextMeshProUGUI _title1;

	// Token: 0x04001DDA RID: 7642
	private TextMeshProUGUI _sub1Text;

	// Token: 0x04001DDB RID: 7643
	private TextMeshProUGUI _sub2Text;

	// Token: 0x04001DDC RID: 7644
	private TextMeshProUGUI _desc2;

	// Token: 0x04001DDD RID: 7645
	private TextMeshProUGUI _desc3;

	// Token: 0x04001DDE RID: 7646
	private TextMeshProUGUI _desc4;

	// Token: 0x04001DDF RID: 7647
	private TextMeshProUGUI _desc6;

	// Token: 0x04001DE0 RID: 7648
	private TextMeshProUGUI _desc7;

	// Token: 0x04001DE1 RID: 7649
	private TextMeshProUGUI _desc8;

	// Token: 0x04001DE2 RID: 7650
	private GameObject _descHolder6;

	// Token: 0x04001DE3 RID: 7651
	private GameObject _descHolder7;

	// Token: 0x04001DE4 RID: 7652
	private GameObject _descHolder8;

	// Token: 0x04001DE5 RID: 7653
	private TextMeshProUGUI _desc9;

	// Token: 0x04001DE6 RID: 7654
	private GameObject _descHolder10;
}
