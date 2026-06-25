using System;
using UnityEngine;

// Token: 0x020000F8 RID: 248
public class MainInterfaceFunctionCommandKit : CommandKitBase
{
	// Token: 0x06000859 RID: 2137 RVA: 0x00039628 File Offset: 0x00037828
	public MainInterfaceFunctionCommandKit()
	{
		base.Id = 11;
		this.GroupDescLanguageId = LanguageKey.LK_HotKeyGroup_MainInterfaceFunction;
		this.GroupCommand = new HotKeyCommand[]
		{
			global::MainInterfaceFunctionCommandKit.QuickEntry,
			global::MainInterfaceFunctionCommandKit.Heal,
			global::MainInterfaceFunctionCommandKit.LegendaryBook,
			global::MainInterfaceFunctionCommandKit.VillagerList,
			global::MainInterfaceFunctionCommandKit.DispatchList,
			global::MainInterfaceFunctionCommandKit.SettlementInformation,
			global::MainInterfaceFunctionCommandKit.TaiwuNotes,
			global::MainInterfaceFunctionCommandKit.ShowInstantNotificationEvent,
			global::MainInterfaceFunctionCommandKit.FollowCharacter,
			global::MainInterfaceFunctionCommandKit.TaiwuLegacy,
			global::MainInterfaceFunctionCommandKit.ViewScroll,
			global::MainInterfaceFunctionCommandKit.InscriptionCharacter,
			global::MainInterfaceFunctionCommandKit.Tutorial,
			global::MainInterfaceFunctionCommandKit.TaiwuEncyclopedia,
			global::MainInterfaceFunctionCommandKit.Achievement,
			global::MainInterfaceFunctionCommandKit.Loop,
			global::MainInterfaceFunctionCommandKit.Read,
			global::MainInterfaceFunctionCommandKit.PartWorldMapPause
		};
		this.TipDisplayCommandArray = new HotKeyCommand[]
		{
			global::MainInterfaceFunctionCommandKit.QuickEntry
		};
	}

	// Token: 0x04000B74 RID: 2932
	public static HotKeyCommand QuickEntry = new HotKeyCommand(1, LanguageKey.LK_HotKeyGroup_MainInterfaceFunction_QuickEntry, KeyCode.Tab, KeyCode.None, true, true);

	// Token: 0x04000B75 RID: 2933
	public static HotKeyCommand Heal = new HotKeyCommand(2, LanguageKey.LK_HotKeyGroup_MainInterfaceFunction_Heal, KeyCode.H, KeyCode.None, true, true);

	// Token: 0x04000B76 RID: 2934
	public static HotKeyCommand LegendaryBook = new HotKeyCommand(3, LanguageKey.LK_HotKeyGroup_MainInterfaceFunction_QishuBible, KeyCode.Backslash, KeyCode.None, true, true);

	// Token: 0x04000B77 RID: 2935
	public static HotKeyCommand VillagerList = new HotKeyCommand(4, LanguageKey.LK_HotKeyGroup_MainInterfaceFunction_VillagerList, KeyCode.L, KeyCode.None, true, true);

	// Token: 0x04000B78 RID: 2936
	public static HotKeyCommand DispatchList = new HotKeyCommand(5, LanguageKey.LK_HotKeyGroup_MainInterfaceFunction_DispatchList, KeyCode.P, KeyCode.None, true, true);

	// Token: 0x04000B79 RID: 2937
	public static HotKeyCommand SettlementInformation = new HotKeyCommand(7, LanguageKey.LK_HotKeyGroup_MainInterfaceFunction_FactionIntel, KeyCode.Q, KeyCode.None, true, true);

	// Token: 0x04000B7A RID: 2938
	public static HotKeyCommand TaiwuNotes = new HotKeyCommand(8, LanguageKey.LK_HotKeyGroup_MainInterfaceFunction_TaiwuNotes, KeyCode.O, KeyCode.None, true, true);

	// Token: 0x04000B7B RID: 2939
	public static HotKeyCommand ShowInstantNotificationEvent = new HotKeyCommand(9, LanguageKey.LK_HotKeyGroup_Map_ShowInstantNotificationEvent, KeyCode.B, KeyCode.None, true, true);

	// Token: 0x04000B7C RID: 2940
	public static HotKeyCommand FollowCharacter = new HotKeyCommand(10, LanguageKey.LK_HotKeyGroup_MainInterfaceFunction_FollowCharacter, KeyCode.LeftBracket, KeyCode.None, true, true);

	// Token: 0x04000B7D RID: 2941
	public static HotKeyCommand TaiwuLegacy = new HotKeyCommand(11, LanguageKey.LK_HotKeyGroup_MainInterfaceFunction_TaiwuLegacy, KeyCode.RightBracket, KeyCode.None, true, true);

	// Token: 0x04000B7E RID: 2942
	public static HotKeyCommand ViewScroll = new HotKeyCommand(12, LanguageKey.LK_HotKeyGroup_MainInterfaceFunction_ViewScroll, KeyCode.Semicolon, KeyCode.None, true, true);

	// Token: 0x04000B7F RID: 2943
	public static HotKeyCommand InscriptionCharacter = new HotKeyCommand(13, LanguageKey.LK_HotKeyGroup_MainInterfaceFunction_InscriptionCharacter, KeyCode.Quote, KeyCode.None, true, true);

	// Token: 0x04000B80 RID: 2944
	public static HotKeyCommand Tutorial = new HotKeyCommand(14, LanguageKey.LK_HotKeyGroup_MainInterfaceFunction_Tutorial, KeyCode.U, KeyCode.None, true, true);

	// Token: 0x04000B81 RID: 2945
	public static HotKeyCommand TaiwuEncyclopedia = new HotKeyCommand(15, LanguageKey.LK_HotKeyGroup_MainInterfaceFunction_TaiwuEncyclopedia, KeyCode.V, KeyCode.None, true, true);

	// Token: 0x04000B82 RID: 2946
	public static HotKeyCommand Achievement = new HotKeyCommand(16, LanguageKey.LK_HotKeyGroup_MainInterfaceFunction_Achievement, KeyCode.Slash, KeyCode.None, true, true);

	// Token: 0x04000B83 RID: 2947
	public static HotKeyCommand Loop = new HotKeyCommand(18, LanguageKey.LK_SystemSetting_HotKeyGroup_Map_Loop, KeyCode.Comma, KeyCode.None, true, true);

	// Token: 0x04000B84 RID: 2948
	public static HotKeyCommand Read = new HotKeyCommand(19, LanguageKey.LK_SystemSetting_HotKeyGroup_Map_Read, KeyCode.Period, KeyCode.None, true, true);

	// Token: 0x04000B85 RID: 2949
	public static HotKeyCommand PartWorldMapPause = new HotKeyCommand(20, LanguageKey.LK_SystemSetting_HotKeyGroup_PartWorldMapPause, KeyCode.Space, KeyCode.None, true, true);
}
