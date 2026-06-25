using System;
using UnityEngine;

namespace Game.Views.Bottom
{
	// Token: 0x02000C35 RID: 3125
	public interface IMainShortcutButton : IAsyncMethodRequestHandler
	{
		// Token: 0x06009ED0 RID: 40656 RVA: 0x004A4A08 File Offset: 0x004A2C08
		void OnChildClicked(MainShortcutButton child)
		{
		}

		// Token: 0x06009ED1 RID: 40657 RVA: 0x004A4A0B File Offset: 0x004A2C0B
		void OnChildEnter(MainShortcutButton child)
		{
		}

		// Token: 0x06009ED2 RID: 40658 RVA: 0x004A4A0E File Offset: 0x004A2C0E
		void OnChildExit(MainShortcutButton child)
		{
		}

		// Token: 0x06009ED3 RID: 40659 RVA: 0x004A4A11 File Offset: 0x004A2C11
		void SetButtonData(Sprite sprite, int templateId, string name)
		{
		}

		// Token: 0x06009ED4 RID: 40660 RVA: 0x004A4A14 File Offset: 0x004A2C14
		bool CleanData()
		{
			return false;
		}
	}
}
