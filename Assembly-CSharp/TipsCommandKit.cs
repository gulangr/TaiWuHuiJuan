using System;
using UnityEngine;

// Token: 0x020000FB RID: 251
public class TipsCommandKit : CommandKitBase
{
	// Token: 0x0600085F RID: 2143 RVA: 0x00039B94 File Offset: 0x00037D94
	public TipsCommandKit()
	{
		base.Id = 9;
		this.GroupDescLanguageId = LanguageKey.LK_HotKeyGroup_Tips;
		this.GroupCommand = new HotKeyCommand[]
		{
			global::TipsCommandKit.ViewDetailInfo,
			global::TipsCommandKit.GlobalTipsHide
		};
		this.TipDisplayCommandArray = new HotKeyCommand[]
		{
			global::TipsCommandKit.ViewDetailInfo,
			global::TipsCommandKit.GlobalTipsHide
		};
	}

	// Token: 0x04000B99 RID: 2969
	public static HotKeyCommand ViewDetailInfo = new HotKeyCommand(1, LanguageKey.LK_HotKeyGroup_Tips_ViewDetailInfo, KeyCode.LeftAlt, KeyCode.None, false, true);

	// Token: 0x04000B9A RID: 2970
	public static HotKeyCommand GlobalTipsHide = new HotKeyCommand(2, LanguageKey.LK_HotKeyGroup_Tips_GlobalTipsHide, KeyCode.H, KeyCode.LeftControl, true, true);
}
