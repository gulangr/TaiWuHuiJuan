using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004A6 RID: 1190
	public class AccessoryDetailedFilterLine : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x17000711 RID: 1809
		// (get) Token: 0x06004177 RID: 16759 RVA: 0x00201963 File Offset: 0x001FFB63
		public override int Id
		{
			get
			{
				return 19;
			}
		}

		// Token: 0x17000712 RID: 1810
		// (get) Token: 0x06004178 RID: 16760 RVA: 0x00201967 File Offset: 0x001FFB67
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x06004179 RID: 16761 RVA: 0x0020196A File Offset: 0x001FFB6A
		public AccessoryDetailedFilterLine(ESortAndFilterControllerType sortAndFilterControllerType = ESortAndFilterControllerType.Item)
		{
			this._sortAndFilterControllerType = sortAndFilterControllerType;
		}

		// Token: 0x0600417A RID: 16762 RVA: 0x00201982 File Offset: 0x001FFB82
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new AccessoryPropMenu();
			yield return new AccessoryMaterialMenu();
			ESortAndFilterControllerType sortAndFilterControllerType = this._sortAndFilterControllerType;
			if (!true)
			{
			}
			EquipStatusMenu equipStatusMenu;
			if (sortAndFilterControllerType != ESortAndFilterControllerType.Equip)
			{
				equipStatusMenu = new EquipStatusMenu(6, new List<EEquipStatusMenuId>
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
				equipStatusMenu = new EquipStatusMenu(6, new List<EEquipStatusMenuId>
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
			yield return new AccessoryMainPropMenu();
			yield return new AccessorySecondaryPropMenu();
			yield return new AccessoryAttackPropMenu();
			yield return new AccessoryDefPropMenu();
			yield return new AccessoryPosionPropMenu();
			yield return new AccessoryFabricHardnessMenu();
			yield return new AccessoryWoodHardnessMenu();
			yield return new AccessoryJadeHardnessMenu();
			yield return new AccessoryMetalHardnessMenu();
			yield break;
		}

		// Token: 0x0600417B RID: 16763 RVA: 0x00201994 File Offset: 0x001FFB94
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(3, ToggleKey.CreateIndexKey(5))
			};
		}

		// Token: 0x04002EA2 RID: 11938
		protected ESortAndFilterControllerType _sortAndFilterControllerType = ESortAndFilterControllerType.Item;
	}
}
