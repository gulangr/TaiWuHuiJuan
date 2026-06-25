using System;
using FrameWork;
using UnityEngine;

// Token: 0x02000410 RID: 1040
public class DialogCmdHuge
{
	// Token: 0x06003E1B RID: 15899 RVA: 0x001F29F8 File Offset: 0x001F0BF8
	public DialogCmdHuge SetDefaultText()
	{
		bool flag = string.IsNullOrWhiteSpace(this.LeftText) && this.Left != null;
		if (flag)
		{
			this.LeftText = LanguageKey.LK_Confirm.Tr();
		}
		bool flag2 = string.IsNullOrWhiteSpace(this.RightText) && this.Right != null;
		if (flag2)
		{
			this.RightText = LanguageKey.LK_Cancel.Tr();
		}
		return this;
	}

	// Token: 0x06003E1C RID: 15900 RVA: 0x001F2A65 File Offset: 0x001F0C65
	public void Show()
	{
		UIElement.DialogHuge.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", this));
		UIManager.Instance.MaskUI(UIElement.DialogHuge);
	}

	// Token: 0x04002CB9 RID: 11449
	public string Title;

	// Token: 0x04002CBA RID: 11450
	public string Content;

	// Token: 0x04002CBB RID: 11451
	public Action Left;

	// Token: 0x04002CBC RID: 11452
	public Action Middle;

	// Token: 0x04002CBD RID: 11453
	public Action Right;

	// Token: 0x04002CBE RID: 11454
	public string LeftText;

	// Token: 0x04002CBF RID: 11455
	public string MiddleText;

	// Token: 0x04002CC0 RID: 11456
	public string RightText;

	// Token: 0x04002CC1 RID: 11457
	public string LeftTips;

	// Token: 0x04002CC2 RID: 11458
	public string MiddleTips;

	// Token: 0x04002CC3 RID: 11459
	public string RightTips;

	// Token: 0x04002CC4 RID: 11460
	public TMPTextSpriteHelper.SizeFitType SpriteHelperFitType = TMPTextSpriteHelper.SizeFitType.Static;

	// Token: 0x04002CC5 RID: 11461
	public Vector2 SpriteHelperSize = new Vector2(30f, 30f);

	// Token: 0x04002CC6 RID: 11462
	public float TextWidth = 800f;
}
