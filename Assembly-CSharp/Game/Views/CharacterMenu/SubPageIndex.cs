using System;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B9C RID: 2972
	public struct SubPageIndex
	{
		// Token: 0x06009339 RID: 37689 RVA: 0x004492DC File Offset: 0x004474DC
		public SubPageIndex(ECharacterSubToggleBase subToggleIndex, ECharacterSubPage subSubPageIndex = ECharacterSubPage.None)
		{
			this.SubToggleIndex = subToggleIndex;
			this.SubSubPageIndex = subSubPageIndex;
		}

		// Token: 0x0400715E RID: 29022
		public ECharacterSubToggleBase SubToggleIndex;

		// Token: 0x0400715F RID: 29023
		public ECharacterSubPage SubSubPageIndex;
	}
}
