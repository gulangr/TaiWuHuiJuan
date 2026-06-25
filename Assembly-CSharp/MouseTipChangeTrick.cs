using System;
using FrameWork;
using TMPro;

// Token: 0x0200027D RID: 637
public class MouseTipChangeTrick : MouseTipBase
{
	// Token: 0x06002943 RID: 10563 RVA: 0x00134318 File Offset: 0x00132518
	protected override void Init(ArgumentBox argsBox)
	{
		TextMeshProUGUI textTitle = base.CGet<TextMeshProUGUI>("Title");
		TextMeshProUGUI textDesc = base.CGet<TextMeshProUGUI>("DescText1");
		TextMeshProUGUI textDesc2 = base.CGet<TextMeshProUGUI>("DescText2");
		TextMeshProUGUI textDesc3 = base.CGet<TextMeshProUGUI>("DescText3");
		string title;
		argsBox.Get("arg0", out title);
		string desc;
		argsBox.Get("arg1", out desc);
		string desc2;
		argsBox.Get("arg2", out desc2);
		string desc3;
		argsBox.Get("arg3", out desc3);
		textTitle.SetText(title, true);
		textDesc.SetText(desc, true);
		textDesc2.SetText(desc2, true);
		textDesc3.SetText(desc3, true);
	}
}
