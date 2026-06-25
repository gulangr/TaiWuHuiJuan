using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000513 RID: 1299
	public class ItemSortAndFilterController : CommonSortAndFilterController<ItemDisplayData>
	{
		// Token: 0x170007BE RID: 1982
		// (get) Token: 0x06004309 RID: 17161 RVA: 0x00205CE3 File Offset: 0x00203EE3
		protected override string FilterCustomOrderKey
		{
			get
			{
				return "ItemFilterCustomOrder";
			}
		}

		// Token: 0x0600430A RID: 17162 RVA: 0x00205CEA File Offset: 0x00203EEA
		public ItemSortAndFilterController(CommonSortAndFilter sortAndFilter) : base(sortAndFilter)
		{
			this.SortController = new ItemSortController();
		}

		// Token: 0x0600430B RID: 17163 RVA: 0x00205D00 File Offset: 0x00203F00
		protected override IEnumerable<FilterLineBase<ItemDisplayData>> GenerateFilterLines()
		{
			yield return new MainFilterLine();
			yield return new FoodSecondaryFilterLine();
			yield return new MedicineSecondaryFilterLine();
			yield return new EquipSecondaryFilterLine();
			yield return new BookSecondaryFilterLine();
			yield return new ToolSecondaryFilterLine();
			yield return new MaterialSecondaryFilterLine();
			yield return new MiscSecondaryFilterLine();
			yield return new FoodVegetarianTypeDetailedFilterLine();
			yield return new FoodMeatTypeDetailedFilterLine();
			yield return new FoodTeaTypeDetailedFilterLine();
			yield return new FoodAlcoholTypeDetailedFilterLine();
			yield return new MedicinePoisonTypeDetailedFilterLine();
			yield return new MedicineBuffTypeDetailedFilterLine();
			yield return new WeaponDetailedFilterLine(ESortAndFilterControllerType.Item);
			yield return new HelmDetailedFilterLine(ESortAndFilterControllerType.Item);
			yield return new TorsoDetailedFilterLine(ESortAndFilterControllerType.Item);
			yield return new BracersDetailedFilterLine(ESortAndFilterControllerType.Item);
			yield return new BootsDetailedFilterLine(ESortAndFilterControllerType.Item);
			yield return new AccessoryDetailedFilterLine(ESortAndFilterControllerType.Item);
			yield return new ClothingDetailedFilterLine(ESortAndFilterControllerType.Item);
			yield return new CarrierDetailedFilterLine(ESortAndFilterControllerType.Item);
			yield return new AllBookDetailedFilterLine();
			yield return new CombatSkillBookDetailedFilterLine();
			yield return new LifeSkillBookDetailedFilterLine();
			yield return new MaterialFabricDetailedFilter();
			yield return new MaterialWoodDetailedFilter();
			yield return new MaterialJadeDetailedFilter();
			yield return new MaterialMetalDetailedFilter();
			yield return new MaterialHerbDetailedFilter();
			yield return new MaterialPoisonDetailedFilter();
			yield return new MaterialFoodDetailedFilter();
			yield return new MiscItemDetailedFilter();
			yield return new MiscMiscDetailedFilter();
			yield return new MiscKeyItemDetailedFilter();
			yield return new MiscWesternPresentDetailedFilter();
			yield break;
		}
	}
}
