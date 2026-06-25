using System;
using System.Collections.Generic;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;

namespace Game.Components.SortAndFilter.CharacterDisplayDataForMapBlock
{
	// Token: 0x02000E6C RID: 3692
	public class CharacterDisplayDataSortAndFilterController : SortAndFilterController<CharacterDisplayData>
	{
		// Token: 0x0600AC7E RID: 44158 RVA: 0x004EE955 File Offset: 0x004ECB55
		public CharacterDisplayDataSortAndFilterController(ISortAndFilterView sortAndFilter, LanguageKey panelTitleKey = LanguageKey.LK_CommonSortAndFilter_FilterPanel_Title_Character) : base(sortAndFilter, panelTitleKey)
		{
			this.SortController = new CharacterDisplayDataSortController(this);
		}

		// Token: 0x0600AC7F RID: 44159 RVA: 0x004EE96D File Offset: 0x004ECB6D
		public void SetData(MapBlockCharacterList data)
		{
			this.Data = data;
		}

		// Token: 0x0600AC80 RID: 44160 RVA: 0x004EE976 File Offset: 0x004ECB76
		protected override IEnumerable<FilterLineBase<CharacterDisplayData>> GenerateFilterLines()
		{
			yield return new CharacterFilterMain(this);
			yield break;
		}

		// Token: 0x040085A8 RID: 34216
		public MapBlockCharacterList Data;
	}
}
