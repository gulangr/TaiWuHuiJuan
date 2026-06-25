using System;
using Config;
using FrameWork;
using Game.Views.Encyclopedia;
using TMPro;
using UnityEngine;

// Token: 0x020002D2 RID: 722
public class MouseTipSimple : MouseTipBase
{
	// Token: 0x170004B9 RID: 1209
	// (get) Token: 0x06002B34 RID: 11060 RVA: 0x00151833 File Offset: 0x0014FA33
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002B35 RID: 11061 RVA: 0x00151838 File Offset: 0x0014FA38
	protected override void Init(ArgumentBox argsBox)
	{
		TextMeshProUGUI textTitle = base.CGet<TextMeshProUGUI>("Title");
		TextMeshProUGUI textDesc = base.CGet<TextMeshProUGUI>("Desc");
		GameObject hotKey = base.CGet<GameObject>("HotKey");
		string title;
		argsBox.Get("arg0", out title);
		string content;
		argsBox.Get("arg1", out content);
		textTitle.text = title;
		textDesc.text = content.ColorReplace();
		textDesc.GetComponent<TMPTextSpriteHelper>().Parse();
		this._linkType = TooltipManager.GetEncyclopediaLinkId(argsBox);
		hotKey.SetActive(this._linkType > 0);
		this.Element.ForceListenCommand = (this._linkType > 0);
	}

	// Token: 0x06002B36 RID: 11062 RVA: 0x001518D8 File Offset: 0x0014FAD8
	private void Update()
	{
		bool flag = this._linkType > 0 && CommonCommandKit.PrimaryInteraction.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			ViewEncyclopediaPanel.OpenLink(EncyclopediaTipLink.Instance[this._linkType]);
		}
	}

	// Token: 0x06002B37 RID: 11063 RVA: 0x00151921 File Offset: 0x0014FB21
	public override void Refresh(ArgumentBox argBox)
	{
		this.Init(argBox);
	}

	// Token: 0x04001F61 RID: 8033
	private int _linkType = -1;
}
