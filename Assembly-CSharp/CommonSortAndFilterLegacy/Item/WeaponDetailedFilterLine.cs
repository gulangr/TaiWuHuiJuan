using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004E6 RID: 1254
	public class WeaponDetailedFilterLine : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x1700078A RID: 1930
		// (get) Token: 0x06004280 RID: 17024 RVA: 0x00204394 File Offset: 0x00202594
		public override int Id
		{
			get
			{
				return 14;
			}
		}

		// Token: 0x1700078B RID: 1931
		// (get) Token: 0x06004281 RID: 17025 RVA: 0x00204398 File Offset: 0x00202598
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x06004282 RID: 17026 RVA: 0x0020439B File Offset: 0x0020259B
		public WeaponDetailedFilterLine(ESortAndFilterControllerType sortAndFilterControllerType = ESortAndFilterControllerType.Item)
		{
			this._sortAndFilterControllerType = sortAndFilterControllerType;
		}

		// Token: 0x06004283 RID: 17027 RVA: 0x002043B3 File Offset: 0x002025B3
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new WeaponSubTypeMenu();
			yield return new WeaponMaterialMenu();
			ESortAndFilterControllerType sortAndFilterControllerType = this._sortAndFilterControllerType;
			if (!true)
			{
			}
			EquipStatusMenu equipStatusMenu;
			if (sortAndFilterControllerType != ESortAndFilterControllerType.Equip)
			{
				equipStatusMenu = new EquipStatusMenu(7, new List<EEquipStatusMenuId>
				{
					EEquipStatusMenuId.Equiped,
					EEquipStatusMenuId.Preset,
					EEquipStatusMenuId.NotEquiped,
					EEquipStatusMenuId.Refine,
					EEquipStatusMenuId.Position,
					EEquipStatusMenuId.HasSpecialEffect
				});
			}
			else
			{
				equipStatusMenu = new EquipStatusMenu(7, new List<EEquipStatusMenuId>
				{
					EEquipStatusMenuId.Preset,
					EEquipStatusMenuId.Refine,
					EEquipStatusMenuId.Position,
					EEquipStatusMenuId.HasSpecialEffect
				});
			}
			if (!true)
			{
			}
			yield return equipStatusMenu;
			yield return new WeaponFabricHardnessMenu();
			yield return new WeaponWoodHardnessMenu();
			yield return new WeaponJadeHardnessMenu();
			yield return new WeaponMetalHardnessMenu();
			yield return new WeaponHerbMakeTypeMenu();
			yield break;
		}

		// Token: 0x06004284 RID: 17028 RVA: 0x002043C4 File Offset: 0x002025C4
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(3, ToggleKey.CreateIndexKey(0))
			};
		}

		// Token: 0x04002F03 RID: 12035
		protected ESortAndFilterControllerType _sortAndFilterControllerType = ESortAndFilterControllerType.Item;
	}
}
