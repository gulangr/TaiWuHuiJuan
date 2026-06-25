using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DDA RID: 3546
	public class MedicineSecondaryFilterLine : SecondaryFilterLineLogic<ITradeableContent>
	{
		// Token: 0x170012A1 RID: 4769
		// (get) Token: 0x0600AA30 RID: 43568 RVA: 0x004E77C1 File Offset: 0x004E59C1
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x170012A2 RID: 4770
		// (get) Token: 0x0600AA31 RID: 43569 RVA: 0x004E77C4 File Offset: 0x004E59C4
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600AA32 RID: 43570 RVA: 0x004E77C7 File Offset: 0x004E59C7
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new MedicineTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x0600AA33 RID: 43571 RVA: 0x004E77D8 File Offset: 0x004E59D8
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(1))
			};
		}
	}
}
