using System;
using UnityEngine;

// Token: 0x020000F4 RID: 244
public class EncyclopediaKit : CommandKitBase
{
	// Token: 0x06000850 RID: 2128 RVA: 0x00039144 File Offset: 0x00037344
	public EncyclopediaKit()
	{
		base.Id = 6;
		this.GroupDescLanguageId = LanguageKey.LK_Encyclopedia_Title;
		this.GroupCommand = new HotKeyCommand[]
		{
			EncyclopediaKit.PrevSearch,
			EncyclopediaKit.NextSearch
		};
	}

	// Token: 0x04000B53 RID: 2899
	public static HotKeyCommand PrevSearch = new HotKeyCommand(1, LanguageKey.LK_Encyclopedia_HotKey_PrevSearch, KeyCode.R, KeyCode.None, true, true);

	// Token: 0x04000B54 RID: 2900
	public static HotKeyCommand NextSearch = new HotKeyCommand(2, LanguageKey.LK_Encyclopedia_HotKey_NextSearch, KeyCode.F, KeyCode.None, true, true);
}
