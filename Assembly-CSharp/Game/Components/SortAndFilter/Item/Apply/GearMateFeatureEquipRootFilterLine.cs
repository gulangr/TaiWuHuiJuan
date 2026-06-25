using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item.Apply
{
	// Token: 0x02000DEE RID: 3566
	public class GearMateFeatureEquipRootFilterLine : EquipSecondaryFilterLine
	{
		// Token: 0x170012BD RID: 4797
		// (get) Token: 0x0600AA91 RID: 43665 RVA: 0x004E8B02 File Offset: 0x004E6D02
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AA92 RID: 43666 RVA: 0x004E8B05 File Offset: 0x004E6D05
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}

		// Token: 0x0600AA93 RID: 43667 RVA: 0x004E8B08 File Offset: 0x004E6D08
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new GearMateFeatureEquipTypeSecondaryMenu();
			yield break;
		}
	}
}
