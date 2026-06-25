using System;
using UnityEngine;

// Token: 0x020000F2 RID: 242
public static class CombatCommandHelper
{
	// Token: 0x0600084D RID: 2125 RVA: 0x00038F2C File Offset: 0x0003712C
	public static HotKeyCommand Create(this ECombatCommandType commandType, LanguageKey descId, KeyCode key, KeyCode fnKey = KeyCode.None, bool canReset = true)
	{
		return new HotKeyCommand((byte)commandType, descId, key, fnKey, canReset, true);
	}
}
