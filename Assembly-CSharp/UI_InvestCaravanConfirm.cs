using System;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x02000303 RID: 771
public class UI_InvestCaravanConfirm : UIBase
{
	// Token: 0x06002DA3 RID: 11683 RVA: 0x00169F14 File Offset: 0x00168114
	public override void OnInit(ArgumentBox argsBox)
	{
		argsBox.Get<Action>("onConfirm", out this._onConfirm);
		string cost;
		argsBox.Get("cost", out cost);
		base.CGet<TextMeshProUGUI>("Cost").text = cost;
	}

	// Token: 0x06002DA4 RID: 11684 RVA: 0x00169F54 File Offset: 0x00168154
	private void OnCancelClick()
	{
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x06002DA5 RID: 11685 RVA: 0x00169F68 File Offset: 0x00168168
	private void OnConfirmClick()
	{
		Action onConfirm = this._onConfirm;
		if (onConfirm != null)
		{
			onConfirm();
		}
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x06002DA6 RID: 11686 RVA: 0x00169F8E File Offset: 0x0016818E
	public override void QuickHide()
	{
		base.QuickHide();
		this.OnCancelClick();
	}

	// Token: 0x06002DA7 RID: 11687 RVA: 0x00169FA0 File Offset: 0x001681A0
	protected override void OnClick(Transform btn)
	{
		string name = btn.name;
		string a = name;
		if (!(a == "Confirm"))
		{
			if (a == "Cancel")
			{
				this.OnCancelClick();
			}
		}
		else
		{
			this.OnConfirmClick();
		}
	}

	// Token: 0x06002DA8 RID: 11688 RVA: 0x00169FE8 File Offset: 0x001681E8
	private void Update()
	{
		bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			this.OnConfirmClick();
		}
		else
		{
			bool flag2 = CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false);
			if (flag2)
			{
				this.QuickHide();
			}
		}
	}

	// Token: 0x04002105 RID: 8453
	private Action _onConfirm;
}
