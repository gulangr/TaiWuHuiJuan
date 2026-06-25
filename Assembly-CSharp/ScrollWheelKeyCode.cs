using System;
using UnityEngine;

// Token: 0x0200001F RID: 31
public static class ScrollWheelKeyCode
{
	// Token: 0x060000F6 RID: 246 RVA: 0x00007710 File Offset: 0x00005910
	public static bool IsScrollWheel(KeyCode keyCode)
	{
		return keyCode == (KeyCode)1000 || keyCode == (KeyCode)1001;
	}

	// Token: 0x0400009F RID: 159
	public const KeyCode ScrollUp = (KeyCode)1000;

	// Token: 0x040000A0 RID: 160
	public const KeyCode ScrollDown = (KeyCode)1001;

	// Token: 0x040000A1 RID: 161
	public const float ScrollDeltaThreshold = 0.5f;
}
