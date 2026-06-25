using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004F0 RID: 1264
	public class EquipSortAndFilterController : ItemSortAndFilterController
	{
		// Token: 0x170007A3 RID: 1955
		// (get) Token: 0x060042B3 RID: 17075 RVA: 0x00204B4A File Offset: 0x00202D4A
		protected override string FilterCustomOrderKey
		{
			get
			{
				return "EquipSortAndFilterController";
			}
		}

		// Token: 0x060042B4 RID: 17076 RVA: 0x00204B51 File Offset: 0x00202D51
		public EquipSortAndFilterController(CommonSortAndFilter sortAndFilter) : base(sortAndFilter)
		{
			this.SortController = new ItemSortController();
		}

		// Token: 0x060042B5 RID: 17077 RVA: 0x00204B67 File Offset: 0x00202D67
		protected override IEnumerable<FilterLineBase<ItemDisplayData>> GenerateFilterLines()
		{
			yield return new CharacterMenuEquipFilterLine();
			yield return new WeaponDetailedFilterLine(ESortAndFilterControllerType.Equip);
			yield return new HelmDetailedFilterLine(ESortAndFilterControllerType.Equip);
			yield return new TorsoDetailedFilterLine(ESortAndFilterControllerType.Equip);
			yield return new BracersDetailedFilterLine(ESortAndFilterControllerType.Equip);
			yield return new BootsDetailedFilterLine(ESortAndFilterControllerType.Equip);
			yield return new AccessoryDetailedFilterLine(ESortAndFilterControllerType.Equip);
			yield return new ClothingDetailedFilterLine(ESortAndFilterControllerType.Equip);
			yield return new CarrierDetailedFilterLine(ESortAndFilterControllerType.Equip);
			yield break;
		}
	}
}
