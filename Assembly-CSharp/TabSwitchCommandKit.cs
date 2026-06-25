using System;
using UnityEngine;

// Token: 0x020000FA RID: 250
public class TabSwitchCommandKit : CommandKitBase
{
	// Token: 0x0600085D RID: 2141 RVA: 0x00039A68 File Offset: 0x00037C68
	public TabSwitchCommandKit()
	{
		base.Id = 8;
		this.GroupDescLanguageId = LanguageKey.LK_HotKeyGroup_TabSwitch;
		this.GroupCommand = new HotKeyCommand[]
		{
			global::TabSwitchCommandKit.PrevTabLevel1,
			global::TabSwitchCommandKit.NextTabLevel1,
			global::TabSwitchCommandKit.PrevTabLevel2,
			global::TabSwitchCommandKit.NextTabLevel2,
			global::TabSwitchCommandKit.PrevTabLevel3,
			global::TabSwitchCommandKit.NextTabLevel3
		};
		this.TipDisplayCommandArray = new HotKeyCommand[]
		{
			global::TabSwitchCommandKit.PrevTabLevel1,
			global::TabSwitchCommandKit.NextTabLevel1,
			global::TabSwitchCommandKit.PrevTabLevel2,
			global::TabSwitchCommandKit.NextTabLevel2,
			global::TabSwitchCommandKit.PrevTabLevel3,
			global::TabSwitchCommandKit.NextTabLevel3
		};
	}

	// Token: 0x04000B93 RID: 2963
	public static HotKeyCommand PrevTabLevel1 = new HotKeyCommand(1, LanguageKey.LK_HotKeyGroup_TabSwitch_PrevTabLevel1, KeyCode.Q, KeyCode.None, true, true);

	// Token: 0x04000B94 RID: 2964
	public static HotKeyCommand NextTabLevel1 = new HotKeyCommand(2, LanguageKey.LK_HotKeyGroup_TabSwitch_NextTabLevel1, KeyCode.E, KeyCode.None, true, true);

	// Token: 0x04000B95 RID: 2965
	public static HotKeyCommand PrevTabLevel2 = new HotKeyCommand(3, LanguageKey.LK_HotKeyGroup_TabSwitch_PrevTabLevel2, KeyCode.A, KeyCode.None, true, true);

	// Token: 0x04000B96 RID: 2966
	public static HotKeyCommand NextTabLevel2 = new HotKeyCommand(4, LanguageKey.LK_HotKeyGroup_TabSwitch_NextTabLevel2, KeyCode.D, KeyCode.None, true, true);

	// Token: 0x04000B97 RID: 2967
	public static HotKeyCommand PrevTabLevel3 = new HotKeyCommand(5, LanguageKey.LK_HotKeyGroup_TabSwitch_PrevTabLevel3, KeyCode.Z, KeyCode.None, true, true);

	// Token: 0x04000B98 RID: 2968
	public static HotKeyCommand NextTabLevel3 = new HotKeyCommand(6, LanguageKey.LK_HotKeyGroup_TabSwitch_NextTabLevel3, KeyCode.C, KeyCode.None, true, true);
}
