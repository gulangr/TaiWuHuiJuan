using System;
using FrameWork;
using TMPro;

// Token: 0x020002D4 RID: 724
public class MouseTipSimpleWide : MouseTipBase
{
	// Token: 0x170004BA RID: 1210
	// (get) Token: 0x06002B3F RID: 11071 RVA: 0x00151A6E File Offset: 0x0014FC6E
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002B40 RID: 11072 RVA: 0x00151A74 File Offset: 0x0014FC74
	protected override void Init(ArgumentBox argsBox)
	{
		TextMeshProUGUI textTitle = base.CGet<TextMeshProUGUI>("Title");
		TextMeshProUGUI textDesc = base.CGet<TextMeshProUGUI>("Desc");
		string title;
		argsBox.Get("arg0", out title);
		string content;
		argsBox.Get("arg1", out content);
		textTitle.text = title;
		textDesc.text = content.ColorReplace();
		textDesc.GetComponent<TMPTextSpriteHelper>().Parse();
	}
}
