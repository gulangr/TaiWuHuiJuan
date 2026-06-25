using System;
using UnityEngine;

// Token: 0x0200001C RID: 28
public static class HotKeyCommandSpecial
{
	// Token: 0x060000D0 RID: 208 RVA: 0x00005AE4 File Offset: 0x00003CE4
	public static bool EditorCheckKeyGM()
	{
		return Input.GetKeyDown(KeyCode.BackQuote);
	}
}
