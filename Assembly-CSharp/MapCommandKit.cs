using System;
using UnityEngine;

// Token: 0x020000F9 RID: 249
public class MapCommandKit : CommandKitBase
{
	// Token: 0x0600085B RID: 2139 RVA: 0x000398A0 File Offset: 0x00037AA0
	public MapCommandKit()
	{
		base.Id = 4;
		this.GroupDescLanguageId = LanguageKey.LK_HotKeyGroup_Map;
		this.GroupCommand = new HotKeyCommand[]
		{
			global::MapCommandKit.ViewScrollUp,
			global::MapCommandKit.ViewScrollDown,
			global::MapCommandKit.FocusOnTaiwuBlock,
			global::MapCommandKit.SearchResult,
			global::MapCommandKit.Legend,
			global::MapCommandKit.PickupMapItems,
			global::MapCommandKit.MoveUp,
			global::MapCommandKit.MoveDown,
			global::MapCommandKit.MoveLeft,
			global::MapCommandKit.MoveRight,
			global::MapCommandKit.Interact,
			global::MapCommandKit.BlockInteract
		};
	}

	// Token: 0x04000B86 RID: 2950
	public static HotKeyCommand MoveUp = new HotKeyCommand(1, LanguageKey.LK_SystemSetting_HotKeyGroup_Map_Move_Up, KeyCode.W, KeyCode.None, true, true);

	// Token: 0x04000B87 RID: 2951
	public static HotKeyCommand MoveDown = new HotKeyCommand(2, LanguageKey.LK_SystemSetting_HotKeyGroup_Map_Move_Down, KeyCode.S, KeyCode.None, true, true);

	// Token: 0x04000B88 RID: 2952
	public static HotKeyCommand MoveLeft = new HotKeyCommand(3, LanguageKey.LK_SystemSetting_HotKeyGroup_Map_Move_Left, KeyCode.A, KeyCode.None, true, true);

	// Token: 0x04000B89 RID: 2953
	public static HotKeyCommand MoveRight = new HotKeyCommand(4, LanguageKey.LK_SystemSetting_HotKeyGroup_Map_Move_Right, KeyCode.D, KeyCode.None, true, true);

	// Token: 0x04000B8A RID: 2954
	public static HotKeyCommand PickupMapItems = new HotKeyCommand(5, LanguageKey.LK_HotKeyGroup_Map_PickupMapItems, KeyCode.G, KeyCode.None, true, true);

	// Token: 0x04000B8B RID: 2955
	public static HotKeyCommand FocusOnTaiwuBlock = new HotKeyCommand(6, LanguageKey.LK_SystemSetting_HotKeyGroup_Map_FocusOnTaiwuBlock, KeyCode.C, KeyCode.None, true, true);

	// Token: 0x04000B8C RID: 2956
	public static HotKeyCommand SearchResult = new HotKeyCommand(7, LanguageKey.LK_HotKeyGroup_Map_SearchResult, KeyCode.Z, KeyCode.None, true, true);

	// Token: 0x04000B8D RID: 2957
	public static HotKeyCommand Legend = new HotKeyCommand(8, LanguageKey.LK_HotKeyGroup_Map_Legend, KeyCode.X, KeyCode.None, true, true);

	// Token: 0x04000B8E RID: 2958
	public static HotKeyCommand MarkCurBlock = new HotKeyCommand(9, LanguageKey.LK_HotKeyGroup_Map_MarkCurBlock, KeyCode.T, KeyCode.None, true, true);

	// Token: 0x04000B8F RID: 2959
	public static HotKeyCommand Interact = new HotKeyCommand(10, LanguageKey.LK_SystemSetting_HotKeyGroup_Interact, KeyCode.Mouse0, KeyCode.None, false, false);

	// Token: 0x04000B90 RID: 2960
	public static HotKeyCommand BlockInteract = new HotKeyCommand(11, LanguageKey.LK_SystemSetting_HotKeyGroup_BlockInteract, KeyCode.Mouse1, KeyCode.None, false, false);

	// Token: 0x04000B91 RID: 2961
	public static HotKeyCommand ViewScrollUp = new HotKeyCommand(12, LanguageKey.LK_SystemSetting_HotKeyGroup_ViewScrollUp, (KeyCode)1000, KeyCode.None, false, false);

	// Token: 0x04000B92 RID: 2962
	public static HotKeyCommand ViewScrollDown = new HotKeyCommand(13, LanguageKey.LK_SystemSetting_HotKeyGroup_ViewScrollDown, (KeyCode)1001, KeyCode.None, false, false);
}
