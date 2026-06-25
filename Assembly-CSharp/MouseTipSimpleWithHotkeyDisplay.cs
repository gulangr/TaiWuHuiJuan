using System;
using FrameWork;
using TMPro;

// Token: 0x020002D5 RID: 725
public class MouseTipSimpleWithHotkeyDisplay : MouseTipBase
{
	// Token: 0x170004BB RID: 1211
	// (get) Token: 0x06002B42 RID: 11074 RVA: 0x00151AE0 File Offset: 0x0014FCE0
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002B43 RID: 11075 RVA: 0x00151AE3 File Offset: 0x0014FCE3
	protected override void Init(ArgumentBox argsBox)
	{
		this.Refresh(argsBox);
	}

	// Token: 0x06002B44 RID: 11076 RVA: 0x00151AF0 File Offset: 0x0014FCF0
	public override void Refresh(ArgumentBox argsBox)
	{
		this.InitRefers();
		string title;
		argsBox.Get("arg0", out title);
		string content;
		argsBox.Get("arg1", out content);
		short hotkeyDisplayId;
		bool showHotkeyDisplay = argsBox.Get("HotkeyDisplayId", out hotkeyDisplayId);
		this._title.text = title;
		this._desc.text = content.ColorReplace();
		this._desc.GetComponent<TMPTextSpriteHelper>().Parse();
		this._hotkeyDisplay.gameObject.SetActive(showHotkeyDisplay);
		bool flag = showHotkeyDisplay;
		if (flag)
		{
			this._hotkeyDisplay.Refresh(hotkeyDisplayId);
		}
	}

	// Token: 0x06002B45 RID: 11077 RVA: 0x00151B88 File Offset: 0x0014FD88
	private void InitRefers()
	{
		bool inited = this._inited;
		if (!inited)
		{
			this._title = base.CGet<TextMeshProUGUI>("Title");
			this._desc = base.CGet<TextMeshProUGUI>("Desc");
			this._hotkeyDisplay = base.CGet<HotkeyDisplay>("HotkeyDisplay");
			this._inited = true;
		}
	}

	// Token: 0x04001F66 RID: 8038
	private bool _inited;

	// Token: 0x04001F67 RID: 8039
	private TextMeshProUGUI _title;

	// Token: 0x04001F68 RID: 8040
	private TextMeshProUGUI _desc;

	// Token: 0x04001F69 RID: 8041
	private HotkeyDisplay _hotkeyDisplay;
}
