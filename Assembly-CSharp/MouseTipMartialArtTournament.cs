using System;
using FrameWork;
using TMPro;

// Token: 0x020002BE RID: 702
public class MouseTipMartialArtTournament : MouseTipBase
{
	// Token: 0x170004AD RID: 1197
	// (get) Token: 0x06002AD4 RID: 10964 RVA: 0x00148909 File Offset: 0x00146B09
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002AD5 RID: 10965 RVA: 0x0014890C File Offset: 0x00146B0C
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
