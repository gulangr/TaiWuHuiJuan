using System;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.CharacterLocationDisplayData
{
	// Token: 0x02000E58 RID: 3672
	public class CharacterLocationDisplayDataSortController : SortController<CharacterLocationDisplayData>
	{
		// Token: 0x0600AC3E RID: 44094 RVA: 0x004EDE5D File Offset: 0x004EC05D
		public override Comparison<CharacterLocationDisplayData> GenerateComparer(SortStateData sortData)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600AC3F RID: 44095 RVA: 0x004EDE64 File Offset: 0x004EC064
		public override SortUiConfig GenerateConfig()
		{
			return default(SortUiConfig);
		}
	}
}
