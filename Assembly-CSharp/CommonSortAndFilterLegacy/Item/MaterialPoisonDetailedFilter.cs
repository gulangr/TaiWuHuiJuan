using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000536 RID: 1334
	internal class MaterialPoisonDetailedFilter : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x170007E5 RID: 2021
		// (get) Token: 0x0600437D RID: 17277 RVA: 0x00207383 File Offset: 0x00205583
		public override int Id
		{
			get
			{
				return 30;
			}
		}

		// Token: 0x170007E6 RID: 2022
		// (get) Token: 0x0600437E RID: 17278 RVA: 0x00207387 File Offset: 0x00205587
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600437F RID: 17279 RVA: 0x0020738A File Offset: 0x0020558A
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new MaterialPoisonTypeMenu();
			yield return new MaterialPoisonPropertyMenu();
			yield return new MaterialPoisonCategoryMenu();
			yield break;
		}

		// Token: 0x06004380 RID: 17280 RVA: 0x0020739C File Offset: 0x0020559C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(6, ToggleKey.CreateIndexKey(5))
			};
		}
	}
}
