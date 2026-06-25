using System;
using GameData.Domains.Character;
using GameData.Utilities;
using TMPro;

// Token: 0x02000340 RID: 832
public class CommonCharacterHealth : Refers
{
	// Token: 0x060030DC RID: 12508 RVA: 0x0017FAB4 File Offset: 0x0017DCB4
	public void Refresh(EHealthType type)
	{
		CImage cimage = this.stateImage;
		string str = "ui_mousetip_healystate_{0}";
		int num = (int)type;
		cimage.SetSprite(str.GetFormat(num.ToString()), false, null);
		this.stateText.text = CommonUtils.GetHealthString(type);
	}

	// Token: 0x040023B3 RID: 9139
	public CImage stateImage;

	// Token: 0x040023B4 RID: 9140
	public TextMeshProUGUI stateText;

	// Token: 0x040023B5 RID: 9141
	public const string StateTextPrefix = "ui_mousetip_healystate_{0}";
}
