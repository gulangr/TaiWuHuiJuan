using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x0200052E RID: 1326
	public class MaterialJadeDetailedFilter : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x170007DD RID: 2013
		// (get) Token: 0x06004369 RID: 17257 RVA: 0x0020729B File Offset: 0x0020549B
		public override int Id
		{
			get
			{
				return 27;
			}
		}

		// Token: 0x0600436A RID: 17258 RVA: 0x0020729F File Offset: 0x0020549F
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new JadeMakeTypeMenu();
			yield return new JadeHardnessMenu();
			yield break;
		}

		// Token: 0x170007DE RID: 2014
		// (get) Token: 0x0600436B RID: 17259 RVA: 0x002072AF File Offset: 0x002054AF
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600436C RID: 17260 RVA: 0x002072B4 File Offset: 0x002054B4
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(6, ToggleKey.CreateIndexKey(2))
			};
		}
	}
}
