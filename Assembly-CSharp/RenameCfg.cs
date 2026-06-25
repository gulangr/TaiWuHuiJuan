using System;
using FrameWork;

// Token: 0x02000411 RID: 1041
public class RenameCfg
{
	// Token: 0x06003E1E RID: 15902 RVA: 0x001F2AC3 File Offset: 0x001F0CC3
	public void Show()
	{
		UIElement.Rename.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cfg", this));
		UIManager.Instance.MaskUI(UIElement.Rename);
	}

	// Token: 0x04002CC7 RID: 11463
	public string Title;

	// Token: 0x04002CC8 RID: 11464
	public string Description;

	// Token: 0x04002CC9 RID: 11465
	public bool IsHideDescription;

	// Token: 0x04002CCA RID: 11466
	public string EmptyDesc = string.Empty;

	// Token: 0x04002CCB RID: 11467
	public string Default = string.Empty;

	// Token: 0x04002CCC RID: 11468
	public int CharCount = 0;

	// Token: 0x04002CCD RID: 11469
	public Action<string> Submit;

	// Token: 0x04002CCE RID: 11470
	public Action Cancel;
}
