using System;
using Game.Components.SortAndFilter.Book;

namespace Game.Components.SortAndFilter.Item.Apply
{
	// Token: 0x02000DF1 RID: 3569
	public static class ESortAndFilterControllerTypeExtensions
	{
		// Token: 0x0600AA9B RID: 43675 RVA: 0x004E8CF0 File Offset: 0x004E6EF0
		public static ItemSortAndFilterController GetSortAndFilterController(this ESortAndFilterControllerType sortAndFilterControllerType, ISortAndFilterView commonSortAndFilter)
		{
			if (!true)
			{
			}
			ItemSortAndFilterController result;
			switch (sortAndFilterControllerType)
			{
			case ESortAndFilterControllerType.Empty:
				result = new EmptyItemController(commonSortAndFilter, LanguageKey.EventEditor_Error_DuplicateGroupKey);
				goto IL_D5;
			case ESortAndFilterControllerType.Item:
				result = new ItemSortAndFilterController(commonSortAndFilter, LanguageKey.EventEditor_Error_DuplicateGroupKey);
				goto IL_D5;
			case ESortAndFilterControllerType.UsingMedicine:
				result = new UsingMedicineSortAndFilterController(commonSortAndFilter);
				goto IL_D5;
			case ESortAndFilterControllerType.Equip:
				result = new EquipSortAndFilterController(commonSortAndFilter);
				goto IL_D5;
			case ESortAndFilterControllerType.Shop:
				result = new ShopSortAndFilterController(commonSortAndFilter, LanguageKey.LK_Exchange_Filter_Title);
				goto IL_D5;
			case ESortAndFilterControllerType.CombatSkillBook:
				result = new CombatSkillBookSortAndFilterController(commonSortAndFilter);
				goto IL_D5;
			case ESortAndFilterControllerType.LifeSkillBook:
				result = new LifeSkillBookSortAndFilterController(commonSortAndFilter);
				goto IL_D5;
			case ESortAndFilterControllerType.MaterialAsRoot:
				result = new MaterialAsRootSortAndFilterController(commonSortAndFilter);
				goto IL_D5;
			case ESortAndFilterControllerType.GearMateFeatureEquipment:
				result = new GearMateFeatureEquipmentSortAndFilterController(commonSortAndFilter);
				goto IL_D5;
			case ESortAndFilterControllerType.Food:
				result = new FoodSortAndFilterController(commonSortAndFilter);
				goto IL_D5;
			case ESortAndFilterControllerType.CraftsmanProduct:
				result = new CraftsmanProductSortAndFilterController(commonSortAndFilter);
				goto IL_D5;
			case ESortAndFilterControllerType.SortOnly:
				result = new SortOnlyItemController(commonSortAndFilter, LanguageKey.EventEditor_Error_DuplicateGroupKey);
				goto IL_D5;
			}
			throw new ArgumentException("未定义的排序筛选类型：" + sortAndFilterControllerType.ToString());
			IL_D5:
			if (!true)
			{
			}
			return result;
		}
	}
}
