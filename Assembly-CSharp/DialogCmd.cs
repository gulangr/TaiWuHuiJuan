using System;
using UnityEngine;

// Token: 0x0200040F RID: 1039
public class DialogCmd
{
	// Token: 0x04002CAF RID: 11439
	public string Title;

	// Token: 0x04002CB0 RID: 11440
	public string Content;

	// Token: 0x04002CB1 RID: 11441
	public Action Yes;

	// Token: 0x04002CB2 RID: 11442
	public Action No;

	// Token: 0x04002CB3 RID: 11443
	public sbyte Type = 1;

	// Token: 0x04002CB4 RID: 11444
	public string GroupYesText;

	// Token: 0x04002CB5 RID: 11445
	public string GroupNoText;

	// Token: 0x04002CB6 RID: 11446
	public TMPTextSpriteHelper.SizeFitType SpriteHelperFitType = TMPTextSpriteHelper.SizeFitType.Static;

	// Token: 0x04002CB7 RID: 11447
	public Vector2 SpriteHelperSize = new Vector2(30f, 30f);

	// Token: 0x04002CB8 RID: 11448
	public EDialogType DialogType = EDialogType.None;
}
