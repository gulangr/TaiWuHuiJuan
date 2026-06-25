using System;

// Token: 0x02000371 RID: 881
public class SheetButtonInfo
{
	// Token: 0x060033A9 RID: 13225 RVA: 0x001989A8 File Offset: 0x00196BA8
	public SheetButtonInfo()
	{
	}

	// Token: 0x060033AA RID: 13226 RVA: 0x001989B2 File Offset: 0x00196BB2
	public SheetButtonInfo(string showText, Action onButtonClick, bool interactable = true, string mouseTip = "")
	{
		this.ButtonShowText = showText;
		this.OnButtonClick = onButtonClick;
		this.Interactable = interactable;
		this.MouseTip = mouseTip;
	}

	// Token: 0x04002593 RID: 9619
	public string ButtonShowText;

	// Token: 0x04002594 RID: 9620
	public bool Interactable;

	// Token: 0x04002595 RID: 9621
	public Action OnButtonClick;

	// Token: 0x04002596 RID: 9622
	public string MouseTip;

	// Token: 0x04002597 RID: 9623
	public Action OnButtonEnter;

	// Token: 0x04002598 RID: 9624
	public Action OnButtonExit;
}
