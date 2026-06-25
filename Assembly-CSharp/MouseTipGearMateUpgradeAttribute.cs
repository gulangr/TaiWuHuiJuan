using System;
using FrameWork;
using TMPro;

// Token: 0x020002A0 RID: 672
public class MouseTipGearMateUpgradeAttribute : MouseTipBase
{
	// Token: 0x06002A29 RID: 10793 RVA: 0x00141C84 File Offset: 0x0013FE84
	protected override void Init(ArgumentBox argsBox)
	{
		this.InitRefers();
		string Desc;
		argsBox.Get("Desc", out Desc);
		this._desc.text = Desc;
	}

	// Token: 0x06002A2A RID: 10794 RVA: 0x00141CB4 File Offset: 0x0013FEB4
	private void InitRefers()
	{
		this._desc = base.CGet<TextMeshProUGUI>("Desc");
	}

	// Token: 0x04001E98 RID: 7832
	private TextMeshProUGUI _desc;
}
