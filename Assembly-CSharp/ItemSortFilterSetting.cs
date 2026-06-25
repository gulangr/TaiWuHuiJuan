using System;
using System.Collections.Generic;
using GameData.Serializer;

// Token: 0x02000352 RID: 850
public class ItemSortFilterSetting : GameData.Serializer.ICommonObjectSerializationAware
{
	// Token: 0x17000565 RID: 1381
	// (get) Token: 0x060031AC RID: 12716 RVA: 0x00188EDC File Offset: 0x001870DC
	// (set) Token: 0x060031AD RID: 12717 RVA: 0x00188EE4 File Offset: 0x001870E4
	public bool IsClassSort { get; private set; }

	// Token: 0x060031AE RID: 12718 RVA: 0x00188EED File Offset: 0x001870ED
	public ItemSortFilterSetting() : this(true)
	{
	}

	// Token: 0x060031AF RID: 12719 RVA: 0x00188EF8 File Offset: 0x001870F8
	private ItemSortFilterSetting(bool setInitFilterType)
	{
		if (setInitFilterType)
		{
			for (ItemSortAndFilter.ItemFilterType type = ItemSortAndFilter.ItemFilterType.Food; type < ItemSortAndFilter.ItemFilterType.Count; type++)
			{
				this.ItemFilterType.Add(type);
			}
			for (ItemSortAndFilter.EquipFilterType type2 = ItemSortAndFilter.EquipFilterType.Weapon; type2 < ItemSortAndFilter.EquipFilterType.Count; type2++)
			{
				this.EquipFilterType.Add(type2);
			}
			for (ItemSortAndFilter.MedicineFilterType type3 = ItemSortAndFilter.MedicineFilterType.Outer; type3 < ItemSortAndFilter.MedicineFilterType.Count; type3++)
			{
				this.MedicineFilterType.Add(type3);
			}
			for (ItemSortAndFilter.SpecialBreakFilterType type4 = ItemSortAndFilter.SpecialBreakFilterType.CombatSkillBook; type4 < ItemSortAndFilter.SpecialBreakFilterType.Count; type4++)
			{
				this.SpecialBreakFilterType.Add(type4);
			}
			for (ItemSortAndFilter.ClothingWeaverFilterType type5 = ItemSortAndFilter.ClothingWeaverFilterType.Normal; type5 < ItemSortAndFilter.ClothingWeaverFilterType.Count; type5++)
			{
				this.ClothingWeaverFilterType.Add(type5);
			}
			for (ItemSortAndFilter.PoisonFilterType type6 = ItemSortAndFilter.PoisonFilterType.Hot; type6 < ItemSortAndFilter.PoisonFilterType.Count; type6++)
			{
				this.PoisonFilterType.Add(type6);
			}
			for (ItemSortAndFilter.MaterialFilterType type7 = ItemSortAndFilter.MaterialFilterType.Food; type7 < ItemSortAndFilter.MaterialFilterType.Count; type7++)
			{
				this.MaterialFilterType.Add(type7);
			}
		}
	}

	// Token: 0x060031B0 RID: 12720 RVA: 0x00189082 File Offset: 0x00187282
	public void SetClassSort(bool classSort)
	{
		this.IsClassSort = classSort;
	}

	// Token: 0x060031B1 RID: 12721 RVA: 0x00189090 File Offset: 0x00187290
	public bool DeserializingUnknownField(string name, out GameData.Serializer.CommonObjectSerializationMember proc)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
		if (num <= 2191473202U)
		{
			if (num != 1347973507U)
			{
				if (num != 1564104736U)
				{
					if (num == 2191473202U)
					{
						if (name == "MaterialFilter")
						{
							proc = GameData.Serializer.CommonObjectSerializationMember.MakeListRefill<ItemSortAndFilter.MaterialFilterType>(name, this.MaterialFilterType);
							return true;
						}
					}
				}
				else if (name == "ItemFilter")
				{
					proc = GameData.Serializer.CommonObjectSerializationMember.MakeListRefill<ItemSortAndFilter.ItemFilterType>(name, this.ItemFilterType);
					return true;
				}
			}
			else if (name == "SortOrder")
			{
				proc = GameData.Serializer.CommonObjectSerializationMember.MakeListRefill<bool>(name, this.IsDescSort);
				return true;
			}
		}
		else if (num <= 3146174295U)
		{
			if (num != 2367949917U)
			{
				if (num == 3146174295U)
				{
					if (name == "ClassSort")
					{
						proc = GameData.Serializer.CommonObjectSerializationMember.Make<bool>(name, () => this.IsClassSort, new Action<bool>(this.SetClassSort));
						return true;
					}
				}
			}
			else if (name == "EquipFilter")
			{
				proc = GameData.Serializer.CommonObjectSerializationMember.MakeListRefill<ItemSortAndFilter.EquipFilterType>(name, this.EquipFilterType);
				return true;
			}
		}
		else if (num != 3207308291U)
		{
			if (num == 4027035321U)
			{
				if (name == "SortType")
				{
					proc = GameData.Serializer.CommonObjectSerializationMember.MakeListRefill<ItemSortAndFilter.SortType>(name, this.SortTypes);
					return true;
				}
			}
		}
		else if (name == "PoisonFilter")
		{
			proc = GameData.Serializer.CommonObjectSerializationMember.MakeListRefill<ItemSortAndFilter.PoisonFilterType>(name, this.PoisonFilterType);
			return true;
		}
		proc = default(GameData.Serializer.CommonObjectSerializationMember);
		return false;
	}

	// Token: 0x04002460 RID: 9312
	public readonly List<ItemSortAndFilter.ItemFilterType> ItemFilterType = new List<ItemSortAndFilter.ItemFilterType>();

	// Token: 0x04002461 RID: 9313
	public readonly List<ItemSortAndFilter.EquipFilterType> EquipFilterType = new List<ItemSortAndFilter.EquipFilterType>();

	// Token: 0x04002462 RID: 9314
	public readonly List<ItemSortAndFilter.MedicineFilterType> MedicineFilterType = new List<ItemSortAndFilter.MedicineFilterType>();

	// Token: 0x04002463 RID: 9315
	public readonly List<ItemSortAndFilter.SpecialBreakFilterType> SpecialBreakFilterType = new List<ItemSortAndFilter.SpecialBreakFilterType>();

	// Token: 0x04002464 RID: 9316
	public readonly List<ItemSortAndFilter.ClothingWeaverFilterType> ClothingWeaverFilterType = new List<ItemSortAndFilter.ClothingWeaverFilterType>();

	// Token: 0x04002465 RID: 9317
	public readonly List<ItemSortAndFilter.PoisonFilterType> PoisonFilterType = new List<ItemSortAndFilter.PoisonFilterType>();

	// Token: 0x04002466 RID: 9318
	public readonly List<ItemSortAndFilter.EntertainFilterType> EntertainFilterType = new List<ItemSortAndFilter.EntertainFilterType>();

	// Token: 0x04002467 RID: 9319
	public readonly List<ItemSortAndFilter.BookFilterType> BookFilterType = new List<ItemSortAndFilter.BookFilterType>();

	// Token: 0x04002468 RID: 9320
	public readonly List<ItemSortAndFilter.MaterialFilterType> MaterialFilterType = new List<ItemSortAndFilter.MaterialFilterType>();

	// Token: 0x04002469 RID: 9321
	public readonly List<ItemSortAndFilter.SortType> SortTypes = new List<ItemSortAndFilter.SortType>();

	// Token: 0x0400246A RID: 9322
	public readonly List<bool> IsDescSort = new List<bool>();

	// Token: 0x0400246C RID: 9324
	private const string ItemEquipFilter = "EquipFilter";

	// Token: 0x0400246D RID: 9325
	private const string ItemPoisonFilter = "PoisonFilter";

	// Token: 0x0400246E RID: 9326
	private const string ItemMaterialFilter = "MaterialFilter";

	// Token: 0x0400246F RID: 9327
	private const string ItemSortType = "SortType";

	// Token: 0x04002470 RID: 9328
	private const string ItemSortOrder = "SortOrder";

	// Token: 0x04002471 RID: 9329
	private const string ItemFilter = "ItemFilter";

	// Token: 0x04002472 RID: 9330
	private const string ItemSortInClass = "ClassSort";
}
