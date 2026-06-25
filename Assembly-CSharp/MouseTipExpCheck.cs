using System;
using FrameWork;
using TMPro;

// Token: 0x02000297 RID: 663
public class MouseTipExpCheck : MouseTipBase
{
	// Token: 0x06002A05 RID: 10757 RVA: 0x0013F0F5 File Offset: 0x0013D2F5
	protected override void Init(ArgumentBox argsBox)
	{
		this.Refresh(argsBox);
	}

	// Token: 0x06002A06 RID: 10758 RVA: 0x0013F100 File Offset: 0x0013D300
	public override void Refresh(ArgumentBox argBox)
	{
		this.ReadArgs(argBox);
		this.InitRefers();
		bool enough = this._hasExp >= this._needExp;
		this._desc.text = this._descString.ColorReplace();
		this._desc.gameObject.SetActive(!enough);
		string colorName = enough ? "brightblue" : "brightred";
		string hasExpString = this._hasExp.ToString().SetColor(colorName);
		this._expLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_ExpCost, hasExpString, this._needExp).ColorReplace();
	}

	// Token: 0x06002A07 RID: 10759 RVA: 0x0013F1A4 File Offset: 0x0013D3A4
	private void ReadArgs(ArgumentBox argBox)
	{
		argBox.Get("NeedExp", out this._needExp);
		argBox.Get("HasExp", out this._hasExp);
		argBox.Get("Desc", out this._descString);
	}

	// Token: 0x06002A08 RID: 10760 RVA: 0x0013F1DD File Offset: 0x0013D3DD
	private void InitRefers()
	{
		this._desc = base.CGet<TextMeshProUGUI>("Desc");
		this._expLabel = base.CGet<TextMeshProUGUI>("ExpLabel");
	}

	// Token: 0x04001E82 RID: 7810
	private int _hasExp;

	// Token: 0x04001E83 RID: 7811
	private int _needExp;

	// Token: 0x04001E84 RID: 7812
	private string _descString;

	// Token: 0x04001E85 RID: 7813
	private TextMeshProUGUI _desc;

	// Token: 0x04001E86 RID: 7814
	private TextMeshProUGUI _expLabel;
}
