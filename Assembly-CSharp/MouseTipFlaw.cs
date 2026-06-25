using System;
using Config;
using FrameWork;
using Game.Views.Encyclopedia;
using TMPro;

// Token: 0x0200029C RID: 668
public class MouseTipFlaw : MouseTipBase
{
	// Token: 0x06002A1C RID: 10780 RVA: 0x001410D0 File Offset: 0x0013F2D0
	protected override void Init(ArgumentBox argsBox)
	{
		this.Element.ForceListenCommand = true;
		TextMeshProUGUI textTitle = base.CGet<TextMeshProUGUI>("Title");
		TextMeshProUGUI textDesc = base.CGet<TextMeshProUGUI>("Desc");
		CImage partIcon = base.CGet<CImage>("PartIcon");
		TextMeshProUGUI textPartName = base.CGet<TextMeshProUGUI>("PartName");
		TextMeshProUGUI textPartDesc = base.CGet<TextMeshProUGUI>("PartDesc");
		string title;
		argsBox.Get("Title", out title);
		string desc;
		argsBox.Get("Desc", out desc);
		string partIconName;
		argsBox.Get("PartIcon", out partIconName);
		string partName;
		argsBox.Get("PartName", out partName);
		string partDesc;
		argsBox.Get("PartDesc", out partDesc);
		textTitle.text = title;
		textDesc.text = desc;
		partIcon.SetSprite(partIconName, false, null);
		textPartName.text = partName;
		textPartDesc.text = partDesc;
	}

	// Token: 0x06002A1D RID: 10781 RVA: 0x001411A0 File Offset: 0x0013F3A0
	private void Update()
	{
		bool flag = CommonCommandKit.PrimaryInteraction.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			ViewEncyclopediaPanel.OpenLink(EncyclopediaTipLink.DefValue.Openings);
		}
	}
}
