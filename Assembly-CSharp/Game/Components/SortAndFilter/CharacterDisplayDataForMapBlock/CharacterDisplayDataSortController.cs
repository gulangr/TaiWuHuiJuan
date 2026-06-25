using System;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.CharacterDisplayDataForMapBlock
{
	// Token: 0x02000E6D RID: 3693
	public class CharacterDisplayDataSortController : SortController<CharacterDisplayData>
	{
		// Token: 0x0600AC81 RID: 44161 RVA: 0x004EE986 File Offset: 0x004ECB86
		public CharacterDisplayDataSortController(CharacterDisplayDataSortAndFilterController controller)
		{
			this.Controller = controller;
		}

		// Token: 0x0600AC82 RID: 44162 RVA: 0x004EE998 File Offset: 0x004ECB98
		public override Comparison<CharacterDisplayData> GenerateComparer(SortStateData sortData)
		{
			return (CharacterDisplayData x, CharacterDisplayData y) => 0;
		}

		// Token: 0x0600AC83 RID: 44163 RVA: 0x004EE9CC File Offset: 0x004ECBCC
		public override SortUiConfig GenerateConfig()
		{
			return default(SortUiConfig);
		}

		// Token: 0x040085A9 RID: 34217
		public CharacterDisplayDataSortAndFilterController Controller;
	}
}
