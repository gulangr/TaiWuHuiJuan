using System;
using UnityEngine;

// Token: 0x020000F5 RID: 245
public class EventEditorCommandKit : CommandKitBase
{
	// Token: 0x170000E1 RID: 225
	// (get) Token: 0x06000852 RID: 2130 RVA: 0x000391A9 File Offset: 0x000373A9
	public override bool ShowInSettings
	{
		get
		{
			return GameApp.Instance.EnableEventEditor;
		}
	}

	// Token: 0x06000853 RID: 2131 RVA: 0x000391B8 File Offset: 0x000373B8
	public EventEditorCommandKit()
	{
		base.Id = 2;
		this.GroupDescLanguageId = LanguageKey.LK_HotKeyGroup_EventEditor;
		this.GroupCommand = new HotKeyCommand[]
		{
			EventEditorCommandKit.SaveCommand,
			EventEditorCommandKit.UndoCommand,
			EventEditorCommandKit.RedoCommand,
			EventEditorCommandKit.AddOption
		};
	}

	// Token: 0x04000B55 RID: 2901
	public static HotKeyCommand SaveCommand = new HotKeyCommand(1, LanguageKey.LK_HotKeyGroup_EventEditor_Save, KeyCode.S, KeyCode.LeftControl, true, true);

	// Token: 0x04000B56 RID: 2902
	public static HotKeyCommand UndoCommand = new HotKeyCommand(2, LanguageKey.LK_HotKeyGroup_EventEditor_Undo, KeyCode.Z, KeyCode.LeftControl, true, true);

	// Token: 0x04000B57 RID: 2903
	public static HotKeyCommand RedoCommand = new HotKeyCommand(3, LanguageKey.LK_HotKeyGroup_EventEditor_Redo, KeyCode.Y, KeyCode.LeftControl, true, true);

	// Token: 0x04000B58 RID: 2904
	public static HotKeyCommand AddOption = new HotKeyCommand(6, LanguageKey.LK_HotKeyGroup_EventEditor_AddOption, KeyCode.A, KeyCode.LeftAlt, true, true);
}
