using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DA8 RID: 3496
	public class MaterialMetalDetailedFilter : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x17001260 RID: 4704
		// (get) Token: 0x0600A955 RID: 43349 RVA: 0x004E5153 File Offset: 0x004E3353
		public override int Id
		{
			get
			{
				return 31;
			}
		}

		// Token: 0x17001261 RID: 4705
		// (get) Token: 0x0600A956 RID: 43350 RVA: 0x004E5157 File Offset: 0x004E3357
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A957 RID: 43351 RVA: 0x004E515A File Offset: 0x004E335A
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new MetalHardnessMenu();
			yield break;
		}

		// Token: 0x0600A958 RID: 43352 RVA: 0x004E516C File Offset: 0x004E336C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(6, ToggleKey.CreateIndexKey(3))
			};
		}
	}
}
