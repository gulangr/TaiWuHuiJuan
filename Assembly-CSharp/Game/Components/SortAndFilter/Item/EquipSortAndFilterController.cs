using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D7F RID: 3455
	public class EquipSortAndFilterController : ItemSortAndFilterController
	{
		// Token: 0x0600A8B5 RID: 43189 RVA: 0x004E17E9 File Offset: 0x004DF9E9
		public EquipSortAndFilterController(ISortAndFilterView sortAndFilter) : base(sortAndFilter, LanguageKey.EventEditor_Error_DuplicateGroupKey)
		{
			this.SortController = new ItemSortController();
		}

		// Token: 0x0600A8B6 RID: 43190 RVA: 0x004E1800 File Offset: 0x004DFA00
		protected override IEnumerable<FilterLineBase<ITradeableContent>> GenerateFilterLines()
		{
			yield return new EquipFilterLine();
			yield return new WeaponDetailedFilterLine();
			yield return new HelmDetailedFilterLine();
			yield return new TorsoDetailedFilterLine();
			yield return new BracersDetailedFilterLine();
			yield return new BootsDetailedFilterLine();
			yield return new AccessoryDetailedFilterLine();
			yield return new ClothingDetailedFilterLine();
			yield return new CarrierDetailedFilterLine();
			yield return new LivestockCarrierDetailedFilterLine();
			yield return new BeastCarrierDetailedFilterLine();
			yield return new PocketDetailedFilterLine();
			yield break;
		}
	}
}
