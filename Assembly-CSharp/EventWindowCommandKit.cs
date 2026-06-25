using System;
using UnityEngine;

// Token: 0x020000F6 RID: 246
public class EventWindowCommandKit : CommandKitBase
{
	// Token: 0x06000855 RID: 2133 RVA: 0x00039280 File Offset: 0x00037480
	public EventWindowCommandKit()
	{
		base.Id = 5;
		this.GroupDescLanguageId = LanguageKey.LK_EventWindowCommand;
		this.GroupCommand = new HotKeyCommand[]
		{
			EventWindowCommandKit.TopicChat,
			EventWindowCommandKit.TopicCompare,
			EventWindowCommandKit.TopicStudy,
			EventWindowCommandKit.TopicIntimacy,
			EventWindowCommandKit.TopicHostile,
			EventWindowCommandKit.TopicInteract,
			EventWindowCommandKit.Option1,
			EventWindowCommandKit.Option2,
			EventWindowCommandKit.Option3,
			EventWindowCommandKit.Option4,
			EventWindowCommandKit.Option5,
			EventWindowCommandKit.Option6,
			EventWindowCommandKit.Option7,
			EventWindowCommandKit.Option8,
			EventWindowCommandKit.Option9,
			EventWindowCommandKit.Option10
		};
	}

	// Token: 0x04000B59 RID: 2905
	public static HotKeyCommand PrevTopic = new HotKeyCommand(1, LanguageKey.LK_EventWindow_PrevTopic, KeyCode.Q, KeyCode.None, true, true);

	// Token: 0x04000B5A RID: 2906
	public static HotKeyCommand NextTopic = new HotKeyCommand(2, LanguageKey.LK_EventWindow_NextTopic, KeyCode.E, KeyCode.None, true, true);

	// Token: 0x04000B5B RID: 2907
	public static HotKeyCommand TopicChat = new HotKeyCommand(3, LanguageKey.LK_EventWindow_TopicChat, KeyCode.F1, KeyCode.None, true, true);

	// Token: 0x04000B5C RID: 2908
	public static HotKeyCommand TopicCompare = new HotKeyCommand(4, LanguageKey.LK_EventWindow_TopicCompare, KeyCode.F2, KeyCode.None, true, true);

	// Token: 0x04000B5D RID: 2909
	public static HotKeyCommand TopicStudy = new HotKeyCommand(5, LanguageKey.LK_EventWindow_TopicStudy, KeyCode.F3, KeyCode.None, true, true);

	// Token: 0x04000B5E RID: 2910
	public static HotKeyCommand TopicIntimacy = new HotKeyCommand(6, LanguageKey.LK_EventWindow_TopicIntimacy, KeyCode.F4, KeyCode.None, true, true);

	// Token: 0x04000B5F RID: 2911
	public static HotKeyCommand TopicHostile = new HotKeyCommand(7, LanguageKey.LK_EventWindow_TopicHostile, KeyCode.F5, KeyCode.None, true, true);

	// Token: 0x04000B60 RID: 2912
	public static HotKeyCommand TopicInteract = new HotKeyCommand(8, LanguageKey.LK_EventWindow_TopicInteract, KeyCode.F6, KeyCode.None, true, true);

	// Token: 0x04000B61 RID: 2913
	public static HotKeyCommand Option1 = new HotKeyCommand(9, LanguageKey.LK_EventWindow_SelectOption_1, KeyCode.Alpha1, KeyCode.None, true, true);

	// Token: 0x04000B62 RID: 2914
	public static HotKeyCommand Option2 = new HotKeyCommand(10, LanguageKey.LK_EventWindow_SelectOption_2, KeyCode.Alpha2, KeyCode.None, true, true);

	// Token: 0x04000B63 RID: 2915
	public static HotKeyCommand Option3 = new HotKeyCommand(11, LanguageKey.LK_EventWindow_SelectOption_3, KeyCode.Alpha3, KeyCode.None, true, true);

	// Token: 0x04000B64 RID: 2916
	public static HotKeyCommand Option4 = new HotKeyCommand(12, LanguageKey.LK_EventWindow_SelectOption_4, KeyCode.Alpha4, KeyCode.None, true, true);

	// Token: 0x04000B65 RID: 2917
	public static HotKeyCommand Option5 = new HotKeyCommand(13, LanguageKey.LK_EventWindow_SelectOption_5, KeyCode.Alpha5, KeyCode.None, true, true);

	// Token: 0x04000B66 RID: 2918
	public static HotKeyCommand Option6 = new HotKeyCommand(14, LanguageKey.LK_EventWindow_SelectOption_6, KeyCode.Alpha6, KeyCode.None, true, true);

	// Token: 0x04000B67 RID: 2919
	public static HotKeyCommand Option7 = new HotKeyCommand(15, LanguageKey.LK_EventWindow_SelectOption_7, KeyCode.Alpha7, KeyCode.None, true, true);

	// Token: 0x04000B68 RID: 2920
	public static HotKeyCommand Option8 = new HotKeyCommand(16, LanguageKey.LK_EventWindow_SelectOption_8, KeyCode.Alpha8, KeyCode.None, true, true);

	// Token: 0x04000B69 RID: 2921
	public static HotKeyCommand Option9 = new HotKeyCommand(17, LanguageKey.LK_EventWindow_SelectOption_9, KeyCode.Alpha9, KeyCode.None, true, true);

	// Token: 0x04000B6A RID: 2922
	public static HotKeyCommand Option10 = new HotKeyCommand(18, LanguageKey.LK_EventWindow_SelectOption_10, KeyCode.Alpha0, KeyCode.None, true, true);

	// Token: 0x04000B6B RID: 2923
	public static HotKeyCommand OptionExit = new HotKeyCommand(19, LanguageKey.LK_EventWindow_SelectOption_Exit, KeyCode.Escape, KeyCode.None, true, true);
}
