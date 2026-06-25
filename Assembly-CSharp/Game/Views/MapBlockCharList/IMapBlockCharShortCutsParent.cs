using System;

namespace Game.Views.MapBlockCharList
{
	// Token: 0x0200093A RID: 2362
	public interface IMapBlockCharShortCutsParent
	{
		// Token: 0x06006E18 RID: 28184
		bool CanClick(int id, int charId);

		// Token: 0x06006E19 RID: 28185
		void OnClick(int id, int charId);
	}
}
