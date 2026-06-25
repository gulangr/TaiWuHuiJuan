using System;
using CommonSortAndFilterLegacy.Item;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000444 RID: 1092
	public static class ESortAndFilterControllerTypeExtensions
	{
		// Token: 0x06004024 RID: 16420 RVA: 0x001FD350 File Offset: 0x001FB550
		public static ItemSortAndFilterController GetSortAndFilterController(this ESortAndFilterControllerType sortAndFilterControllerType, CommonSortAndFilter commonSortAndFilter)
		{
			if (!true)
			{
			}
			ItemSortAndFilterController result;
			switch (sortAndFilterControllerType)
			{
			case ESortAndFilterControllerType.Item:
				result = new ItemSortAndFilterController(commonSortAndFilter);
				break;
			case ESortAndFilterControllerType.UsingMedicine:
				result = new UsingMedicineSortAndFilterController(commonSortAndFilter);
				break;
			case ESortAndFilterControllerType.Equip:
				result = new EquipSortAndFilterController(commonSortAndFilter);
				break;
			case ESortAndFilterControllerType.Tool:
				result = new SelectToolSortAndFilterController(commonSortAndFilter);
				break;
			case ESortAndFilterControllerType.Shop:
				result = new ShopItemSortAndFilterController(commonSortAndFilter);
				break;
			default:
				throw new ArgumentException("未定义的排序筛选类型：" + sortAndFilterControllerType.ToString());
			}
			if (!true)
			{
			}
			return result;
		}
	}
}
