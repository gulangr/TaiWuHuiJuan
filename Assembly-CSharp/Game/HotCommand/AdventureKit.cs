using System;
using UnityEngine;

namespace Game.HotCommand
{
	// Token: 0x02000C86 RID: 3206
	public class AdventureKit : CommandKitBase
	{
		// Token: 0x0600A39F RID: 41887 RVA: 0x004C8E8E File Offset: 0x004C708E
		public AdventureKit()
		{
			base.Id = 7;
			this.GroupDescLanguageId = LanguageKey.LK_Adventure;
			this.GroupCommand = new HotKeyCommand[]
			{
				AdventureKit.SimpleViewMode
			};
		}

		// Token: 0x04007F50 RID: 32592
		public static HotKeyCommand SimpleViewMode = new HotKeyCommand(1, LanguageKey.Lk_Adventure_SimpleView_Switch, KeyCode.CapsLock, KeyCode.None, true, true);
	}
}
