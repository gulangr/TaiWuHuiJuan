using System;

namespace Game.Views.Bottom
{
	// Token: 0x02000C58 RID: 3160
	public interface IMainMenuButtonParent : IAsyncMethodRequestHandler
	{
		// Token: 0x0600A0ED RID: 41197 RVA: 0x004B2738 File Offset: 0x004B0938
		void OnChildClicked(MainMenuButton child)
		{
		}

		// Token: 0x0600A0EE RID: 41198 RVA: 0x004B273B File Offset: 0x004B093B
		void OnChildEnter(MainMenuButton child)
		{
		}

		// Token: 0x0600A0EF RID: 41199 RVA: 0x004B273E File Offset: 0x004B093E
		void OnChildExit(MainMenuButton child)
		{
		}
	}
}
