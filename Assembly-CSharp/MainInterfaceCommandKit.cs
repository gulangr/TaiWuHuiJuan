using System;
using UnityEngine;

// Token: 0x020000F7 RID: 247
public class MainInterfaceCommandKit : CommandKitBase
{
	// Token: 0x06000857 RID: 2135 RVA: 0x000394F8 File Offset: 0x000376F8
	public MainInterfaceCommandKit()
	{
		base.Id = 10;
		this.GroupDescLanguageId = LanguageKey.LK_HotKeyGroup_MainInterface;
		this.GroupCommand = new HotKeyCommand[]
		{
			global::MainInterfaceCommandKit.ViewCharacterPanel,
			global::MainInterfaceCommandKit.MonthPass,
			global::MainInterfaceCommandKit.TaiwuMonthlyReport,
			global::MainInterfaceCommandKit.IndustryView,
			global::MainInterfaceCommandKit.PartWorldMap,
			global::MainInterfaceCommandKit.WorldState,
			global::MainInterfaceCommandKit.ReadBook,
			global::MainInterfaceCommandKit.Looping
		};
	}

	// Token: 0x04000B6C RID: 2924
	public static HotKeyCommand ViewCharacterPanel = new HotKeyCommand(1, LanguageKey.LK_HotKeyGroup_MainInterface_ViewCharacterPanel, KeyCode.I, KeyCode.None, true, true);

	// Token: 0x04000B6D RID: 2925
	public static HotKeyCommand MonthPass = new HotKeyCommand(2, LanguageKey.LK_HotKeyGroup_MainInterface_MonthPass, KeyCode.Space, KeyCode.None, false, true);

	// Token: 0x04000B6E RID: 2926
	public static HotKeyCommand TaiwuMonthlyReport = new HotKeyCommand(3, LanguageKey.LK_HotKeyGroup_MainInterface_TaiwuMonthlyReport, KeyCode.Y, KeyCode.None, true, true);

	// Token: 0x04000B6F RID: 2927
	public static HotKeyCommand IndustryView = new HotKeyCommand(4, LanguageKey.LK_HotKeyGroup_MainInterface_IndustryView, KeyCode.N, KeyCode.None, true, true);

	// Token: 0x04000B70 RID: 2928
	public static HotKeyCommand PartWorldMap = new HotKeyCommand(5, LanguageKey.LK_HotKeyGroup_MainInterface_TravelMap, KeyCode.M, KeyCode.None, true, true);

	// Token: 0x04000B71 RID: 2929
	public static HotKeyCommand WorldState = new HotKeyCommand(6, LanguageKey.LK_HotKeyGroup_MainInterface_WorldState, KeyCode.R, KeyCode.None, true, true);

	// Token: 0x04000B72 RID: 2930
	public static HotKeyCommand ReadBook = new HotKeyCommand(7, LanguageKey.LK_HotKeyGroup_MainInterface_ReadBook, KeyCode.J, KeyCode.None, true, true);

	// Token: 0x04000B73 RID: 2931
	public static HotKeyCommand Looping = new HotKeyCommand(8, LanguageKey.LK_HotKeyGroup_MainInterface_ZhouTianOperation, KeyCode.K, KeyCode.None, true, true);
}
