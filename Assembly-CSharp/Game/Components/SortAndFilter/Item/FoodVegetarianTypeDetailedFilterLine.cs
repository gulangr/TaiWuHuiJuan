using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D80 RID: 3456
	public class FoodVegetarianTypeDetailedFilterLine : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x17001238 RID: 4664
		// (get) Token: 0x0600A8B7 RID: 43191 RVA: 0x004E1810 File Offset: 0x004DFA10
		public override int Id
		{
			get
			{
				return 8;
			}
		}

		// Token: 0x17001239 RID: 4665
		// (get) Token: 0x0600A8B8 RID: 43192 RVA: 0x004E1813 File Offset: 0x004DFA13
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A8B9 RID: 43193 RVA: 0x004E1816 File Offset: 0x004DFA16
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new FoodRecoverTypeMenu();
			yield return new FoodBuffTypeMenu();
			yield break;
		}

		// Token: 0x0600A8BA RID: 43194 RVA: 0x004E1828 File Offset: 0x004DFA28
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(1, ToggleKey.CreateIndexKey(0))
			};
		}
	}
}
