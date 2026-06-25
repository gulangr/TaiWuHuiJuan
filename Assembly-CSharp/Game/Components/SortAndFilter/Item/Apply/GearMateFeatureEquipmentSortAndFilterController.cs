using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item.Apply
{
	// Token: 0x02000DED RID: 3565
	public class GearMateFeatureEquipmentSortAndFilterController : ItemSortAndFilterController
	{
		// Token: 0x0600AA8F RID: 43663 RVA: 0x004E8AD7 File Offset: 0x004E6CD7
		public GearMateFeatureEquipmentSortAndFilterController(ISortAndFilterView sortAndFilter) : base(sortAndFilter, LanguageKey.LK_CharacterMenu_Tog_Equip)
		{
			this.SortController = new ItemSortController();
		}

		// Token: 0x0600AA90 RID: 43664 RVA: 0x004E8AF2 File Offset: 0x004E6CF2
		protected override IEnumerable<FilterLineBase<ITradeableContent>> GenerateFilterLines()
		{
			yield return new GearMateFeatureEquipRootFilterLine();
			yield return new WeaponDetailedFilterLine();
			yield return new HelmDetailedFilterLine();
			yield return new TorsoDetailedFilterLine();
			yield return new BracersDetailedFilterLine();
			yield return new BootsDetailedFilterLine();
			yield return new AccessoryDetailedFilterLine();
			yield break;
		}
	}
}
