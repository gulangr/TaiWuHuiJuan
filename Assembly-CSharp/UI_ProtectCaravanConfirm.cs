using System;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x02000304 RID: 772
public class UI_ProtectCaravanConfirm : UIBase
{
	// Token: 0x06002DAA RID: 11690 RVA: 0x0016A048 File Offset: 0x00168248
	public override void OnInit(ArgumentBox argsBox)
	{
		argsBox.Get<Action>("onConfirm", out this._onConfirm);
		string cost;
		argsBox.Get("cost", out cost);
		base.CGet<TextMeshProUGUI>("Cost").text = cost;
	}

	// Token: 0x06002DAB RID: 11691 RVA: 0x0016A088 File Offset: 0x00168288
	private void OnCancelClick()
	{
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x06002DAC RID: 11692 RVA: 0x0016A09C File Offset: 0x0016829C
	private void OnConfirmClick()
	{
		Action onConfirm = this._onConfirm;
		if (onConfirm != null)
		{
			onConfirm();
		}
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x06002DAD RID: 11693 RVA: 0x0016A0C2 File Offset: 0x001682C2
	public override void QuickHide()
	{
		base.QuickHide();
		this.OnCancelClick();
	}

	// Token: 0x06002DAE RID: 11694 RVA: 0x0016A0D4 File Offset: 0x001682D4
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

	// Token: 0x06002DAF RID: 11695 RVA: 0x0016A11C File Offset: 0x0016831C
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

	// Token: 0x04002106 RID: 8454
	private Action _onConfirm;
}
