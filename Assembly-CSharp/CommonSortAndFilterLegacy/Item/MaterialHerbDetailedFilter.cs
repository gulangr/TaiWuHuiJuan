using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000528 RID: 1320
	public class MaterialHerbDetailedFilter : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x170007D3 RID: 2003
		// (get) Token: 0x0600434F RID: 17231 RVA: 0x00206783 File Offset: 0x00204983
		public override int Id
		{
			get
			{
				return 29;
			}
		}

		// Token: 0x06004350 RID: 17232 RVA: 0x00206787 File Offset: 0x00204987
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new MaterialHerbTreatmentMenu();
			yield return new MaterialHerbBuffMenu();
			yield return new MaterialHerbCureMenu();
			yield return new MaterialHerbPropertyMenu();
			yield break;
		}

		// Token: 0x170007D4 RID: 2004
		// (get) Token: 0x06004351 RID: 17233 RVA: 0x00206797 File Offset: 0x00204997
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x06004352 RID: 17234 RVA: 0x0020679C File Offset: 0x0020499C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(6, ToggleKey.CreateIndexKey(4))
			};
		}
	}
}
