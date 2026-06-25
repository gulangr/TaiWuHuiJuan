using System;
using UnityEngine;

// Token: 0x020000F3 RID: 243
public class CommonCommandKit : CommandKitBase
{
	// Token: 0x0600084E RID: 2126 RVA: 0x00038F4C File Offset: 0x0003714C
	public CommonCommandKit()
	{
		base.Id = 1;
		this.GroupDescLanguageId = LanguageKey.LK_HotKeyGroup_Common;
		this.GroupCommand = new HotKeyCommand[]
		{
			global::CommonCommandKit.Esc,
			global::CommonCommandKit.Space,
			global::CommonCommandKit.RightMouse,
			global::CommonCommandKit.OpenGMPanel,
			global::CommonCommandKit.LeftMouse,
			global::CommonCommandKit.PrimaryInteraction,
			global::CommonCommandKit.SecondaryInteraction,
			global::CommonCommandKit.StickTips
		};
		this.TipDisplayCommandArray = new HotKeyCommand[]
		{
			global::CommonCommandKit.Esc,
			global::CommonCommandKit.Space,
			global::CommonCommandKit.RightMouse,
			global::CommonCommandKit.LeftMouse,
			global::CommonCommandKit.StickTips,
			global::CommonCommandKit.ClearStickTips,
			global::CommonCommandKit.Enter,
			global::CommonCommandKit.Shift,
			global::CommonCommandKit.Alt,
			global::CommonCommandKit.PrimaryInteraction,
			global::CommonCommandKit.SecondaryInteraction
		};
	}

	// Token: 0x04000B47 RID: 2887
	public static HotKeyCommand Esc = new HotKeyCommand(1, LanguageKey.LK_SystemSetting_HotKeyGroup_Common_Cancel, KeyCode.Escape, KeyCode.None, false, true);

	// Token: 0x04000B48 RID: 2888
	public static HotKeyCommand Space = new HotKeyCommand(2, LanguageKey.LK_SystemSetting_HotKeyGroup_Common_Confirm, KeyCode.Space, KeyCode.None, false, true);

	// Token: 0x04000B49 RID: 2889
	public static HotKeyCommand RightMouse = new HotKeyCommand(3, LanguageKey.LK_SystemSetting_HotKeyGroup_Common_Close, KeyCode.Mouse1, KeyCode.None, false, true);

	// Token: 0x04000B4A RID: 2890
	public static HotKeyCommand StickTips = new HotKeyCommand(4, LanguageKey.LK_SystemSetting_HotKeyGroup_Common_StickTips, KeyCode.T, KeyCode.None, true, true);

	// Token: 0x04000B4B RID: 2891
	public static HotKeyCommand LeftMouse = new HotKeyCommand(5, LanguageKey.LK_SystemSetting_HotKeyGroup_Interact, KeyCode.Mouse0, KeyCode.None, false, true);

	// Token: 0x04000B4C RID: 2892
	public static HotKeyCommand ClearStickTips = new HotKeyCommand(6, LanguageKey.LK_SystemSetting_HotKeyGroup_Common_ClearStickTips, KeyCode.KeypadPeriod, KeyCode.None, true, true);

	// Token: 0x04000B4D RID: 2893
	public static HotKeyCommand Enter = new HotKeyCommand(7, LanguageKey.LK_SystemSetting_HotKeyGroup_Common_Confirm, KeyCode.Return, KeyCode.None, false, true);

	// Token: 0x04000B4E RID: 2894
	public static HotKeyCommand Shift = new HotKeyCommand(8, LanguageKey.LK_HotKeyGroup_Common_Shift, KeyCode.LeftShift, KeyCode.None, false, true);

	// Token: 0x04000B4F RID: 2895
	public static HotKeyCommand Alt = new HotKeyCommand(9, LanguageKey.LK_HotKeyGroup_Common_Alt, KeyCode.LeftAlt, KeyCode.None, false, true);

	// Token: 0x04000B50 RID: 2896
	public static HotKeyCommand OpenGMPanel = new HotKeyCommand(10, LanguageKey.LK_HotKeyGroup_Common_OpenGMPanel, KeyCode.BackQuote, KeyCode.None, false, false);

	// Token: 0x04000B51 RID: 2897
	public static HotKeyCommand PrimaryInteraction = new HotKeyCommand(11, LanguageKey.LK_SystemSetting_HotKeyGroup_PrimaryInteraction, KeyCode.F, KeyCode.None, true, true);

	// Token: 0x04000B52 RID: 2898
	public static HotKeyCommand SecondaryInteraction = new HotKeyCommand(12, LanguageKey.LK_SystemSetting_HotKeyGroup_SecondaryInteraction, KeyCode.G, KeyCode.None, true, true);
}
