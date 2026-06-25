using System;
using System.Collections;
using System.Collections.Generic;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using UnityEngine;

// Token: 0x02000350 RID: 848
public class ItemGradeFilterSetting : GameData.Serializer.ICommonObjectSerializationAware
{
	// Token: 0x06003167 RID: 12647 RVA: 0x00185774 File Offset: 0x00183974
	public void SetGrade(ItemGradeFilterSetting.ItemGradeFilterSourceType type, sbyte grade = -1)
	{
		this._gradeDict[type] = (sbyte)Mathf.Clamp((int)grade, -1, 8);
	}

	// Token: 0x06003168 RID: 12648 RVA: 0x0018578C File Offset: 0x0018398C
	public sbyte GetGrade(ItemGradeFilterSetting.ItemGradeFilterSourceType type)
	{
		return this._gradeDict[type];
	}

	// Token: 0x06003169 RID: 12649 RVA: 0x0018579C File Offset: 0x0018399C
	public int GetIndex(ItemGradeFilterSetting.ItemGradeFilterSourceType type)
	{
		sbyte grade = this.GetGrade(type);
		return (int)((grade < 0) ? 0 : (9 - grade));
	}

	// Token: 0x0600316A RID: 12650 RVA: 0x001857C4 File Offset: 0x001839C4
	public sbyte GetGradeByIndex(int index)
	{
		index = Mathf.Clamp(index, -1, 8);
		int garde = (index == 0) ? -1 : (9 - index);
		return (sbyte)garde;
	}

	// Token: 0x0600316B RID: 12651 RVA: 0x001857F0 File Offset: 0x001839F0
	public static ItemGradeFilterSetting.ItemGradeFilterSourceType GetItemGradeFilterSourceType(ItemSourceType itemSourceType, bool isInventory, bool isGearMate = false)
	{
		if (!true)
		{
		}
		ItemGradeFilterSetting.ItemGradeFilterSourceType result;
		switch (itemSourceType)
		{
		case ItemSourceType.Inventory:
			result = (isGearMate ? ItemGradeFilterSetting.ItemGradeFilterSourceType.GearMateInventory : (isInventory ? ItemGradeFilterSetting.ItemGradeFilterSourceType.Inventory : ItemGradeFilterSetting.ItemGradeFilterSourceType.WarehouseInventory));
			break;
		case ItemSourceType.Warehouse:
			result = (isGearMate ? ItemGradeFilterSetting.ItemGradeFilterSourceType.GearMateWarehouse : ItemGradeFilterSetting.ItemGradeFilterSourceType.Warehouse);
			break;
		case ItemSourceType.Treasury:
			result = (isGearMate ? ItemGradeFilterSetting.ItemGradeFilterSourceType.GearMateTreasury : ItemGradeFilterSetting.ItemGradeFilterSourceType.Treasury);
			break;
		case ItemSourceType.Trough:
			result = ItemGradeFilterSetting.ItemGradeFilterSourceType.Trough;
			break;
		case ItemSourceType.Stock:
			result = ItemGradeFilterSetting.ItemGradeFilterSourceType.StockStorageGoodsShelf;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x0600316C RID: 12652 RVA: 0x0018585E File Offset: 0x00183A5E
	public IEnumerable<GameData.Serializer.CommonObjectSerializationMember> ExtraMembers(bool deserializing)
	{
		using (IEnumerator enumerator = Enum.GetValues(typeof(ItemGradeFilterSetting.ItemGradeFilterSourceType)).GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				ItemGradeFilterSetting.<>c__DisplayClass9_0 CS$<>8__locals1 = new ItemGradeFilterSetting.<>c__DisplayClass9_0();
				CS$<>8__locals1.<>4__this = this;
				CS$<>8__locals1.type = (ItemGradeFilterSetting.ItemGradeFilterSourceType)enumerator.Current;
				int idx = (int)CS$<>8__locals1.type;
				yield return GameData.Serializer.CommonObjectSerializationMember.Make<sbyte>(idx.ToString(), () => CS$<>8__locals1.<>4__this.GetGrade(CS$<>8__locals1.type), delegate(sbyte v)
				{
					CS$<>8__locals1.<>4__this.SetGrade(CS$<>8__locals1.type, v);
				});
				CS$<>8__locals1 = null;
			}
		}
		IEnumerator enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x0600316D RID: 12653 RVA: 0x00185878 File Offset: 0x00183A78
	public bool DeserializingUnknownField(string name, out GameData.Serializer.CommonObjectSerializationMember proc)
	{
		int idx;
		bool flag = int.TryParse(name, out idx);
		bool result;
		if (flag)
		{
			ItemGradeFilterSetting.ItemGradeFilterSourceType type = (ItemGradeFilterSetting.ItemGradeFilterSourceType)idx;
			proc = GameData.Serializer.CommonObjectSerializationMember.Make<sbyte>(idx.ToString(), () => this.GetGrade(type), delegate(sbyte v)
			{
				this.SetGrade(type, v);
			});
			result = true;
		}
		else
		{
			proc = default(GameData.Serializer.CommonObjectSerializationMember);
			result = false;
		}
		return result;
	}

	// Token: 0x04002434 RID: 9268
	private const int DefaultGrade = 1;

	// Token: 0x04002435 RID: 9269
	private readonly Dictionary<ItemGradeFilterSetting.ItemGradeFilterSourceType, sbyte> _gradeDict = new Dictionary<ItemGradeFilterSetting.ItemGradeFilterSourceType, sbyte>
	{
		{
			ItemGradeFilterSetting.ItemGradeFilterSourceType.Inventory,
			1
		},
		{
			ItemGradeFilterSetting.ItemGradeFilterSourceType.WarehouseInventory,
			1
		},
		{
			ItemGradeFilterSetting.ItemGradeFilterSourceType.Warehouse,
			1
		},
		{
			ItemGradeFilterSetting.ItemGradeFilterSourceType.Trough,
			1
		},
		{
			ItemGradeFilterSetting.ItemGradeFilterSourceType.Treasury,
			1
		},
		{
			ItemGradeFilterSetting.ItemGradeFilterSourceType.StockStorageWarehouse,
			1
		},
		{
			ItemGradeFilterSetting.ItemGradeFilterSourceType.StockStorageGoodsShelf,
			1
		},
		{
			ItemGradeFilterSetting.ItemGradeFilterSourceType.CraftStorageWarehouse,
			1
		},
		{
			ItemGradeFilterSetting.ItemGradeFilterSourceType.CraftStorageMaterial,
			1
		},
		{
			ItemGradeFilterSetting.ItemGradeFilterSourceType.CraftStorageToFix,
			1
		},
		{
			ItemGradeFilterSetting.ItemGradeFilterSourceType.CraftStorageToDisassemble,
			1
		},
		{
			ItemGradeFilterSetting.ItemGradeFilterSourceType.MedicineStorageWarehouse,
			1
		},
		{
			ItemGradeFilterSetting.ItemGradeFilterSourceType.MedicineStorageMaterial,
			1
		},
		{
			ItemGradeFilterSetting.ItemGradeFilterSourceType.MedicineStorageToDetox,
			1
		},
		{
			ItemGradeFilterSetting.ItemGradeFilterSourceType.MedicineStorageToAddPoison,
			1
		},
		{
			ItemGradeFilterSetting.ItemGradeFilterSourceType.FoodStorageWarehouse,
			1
		},
		{
			ItemGradeFilterSetting.ItemGradeFilterSourceType.FoodStorageMaterial,
			1
		},
		{
			ItemGradeFilterSetting.ItemGradeFilterSourceType.GearMateInventory,
			1
		},
		{
			ItemGradeFilterSetting.ItemGradeFilterSourceType.GearMateWarehouse,
			1
		},
		{
			ItemGradeFilterSetting.ItemGradeFilterSourceType.GearMateTreasury,
			1
		}
	};

	// Token: 0x04002436 RID: 9270
	public const string SaveTag = "Default";

	// Token: 0x020016E1 RID: 5857
	public enum ItemGradeFilterSourceType
	{
		// Token: 0x0400A969 RID: 43369
		Inventory,
		// Token: 0x0400A96A RID: 43370
		WarehouseInventory,
		// Token: 0x0400A96B RID: 43371
		Warehouse,
		// Token: 0x0400A96C RID: 43372
		Trough,
		// Token: 0x0400A96D RID: 43373
		Treasury,
		// Token: 0x0400A96E RID: 43374
		[Obsolete]
		StockStorageWarehouse,
		// Token: 0x0400A96F RID: 43375
		StockStorageGoodsShelf,
		// Token: 0x0400A970 RID: 43376
		[Obsolete]
		CraftStorageWarehouse,
		// Token: 0x0400A971 RID: 43377
		[Obsolete]
		CraftStorageMaterial,
		// Token: 0x0400A972 RID: 43378
		[Obsolete]
		CraftStorageToFix,
		// Token: 0x0400A973 RID: 43379
		[Obsolete]
		CraftStorageToDisassemble,
		// Token: 0x0400A974 RID: 43380
		[Obsolete]
		MedicineStorageWarehouse,
		// Token: 0x0400A975 RID: 43381
		[Obsolete]
		MedicineStorageMaterial,
		// Token: 0x0400A976 RID: 43382
		[Obsolete]
		MedicineStorageToDetox,
		// Token: 0x0400A977 RID: 43383
		[Obsolete]
		MedicineStorageToAddPoison,
		// Token: 0x0400A978 RID: 43384
		[Obsolete]
		FoodStorageWarehouse,
		// Token: 0x0400A979 RID: 43385
		[Obsolete]
		FoodStorageMaterial,
		// Token: 0x0400A97A RID: 43386
		GearMateInventory,
		// Token: 0x0400A97B RID: 43387
		GearMateWarehouse,
		// Token: 0x0400A97C RID: 43388
		GearMateTreasury
	}
}
