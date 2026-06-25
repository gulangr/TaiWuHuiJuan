using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004CA RID: 1226
	public class ClothingDetailedFilterLine : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x17000758 RID: 1880
		// (get) Token: 0x06004218 RID: 16920 RVA: 0x00203398 File Offset: 0x00201598
		public override int Id
		{
			get
			{
				return 20;
			}
		}

		// Token: 0x17000759 RID: 1881
		// (get) Token: 0x06004219 RID: 16921 RVA: 0x0020339C File Offset: 0x0020159C
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600421A RID: 16922 RVA: 0x0020339F File Offset: 0x0020159F
		public ClothingDetailedFilterLine(ESortAndFilterControllerType sortAndFilterControllerType = ESortAndFilterControllerType.Item)
		{
			this._sortAndFilterControllerType = sortAndFilterControllerType;
		}

		// Token: 0x0600421B RID: 16923 RVA: 0x002033B7 File Offset: 0x002015B7
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new ClothingWeaveTypeMenu();
			ESortAndFilterControllerType sortAndFilterControllerType = this._sortAndFilterControllerType;
			if (!true)
			{
			}
			EquipStatusMenu equipStatusMenu;
			if (sortAndFilterControllerType != ESortAndFilterControllerType.Equip)
			{
				equipStatusMenu = new EquipStatusMenu(1, new List<EEquipStatusMenuId>
				{
					EEquipStatusMenuId.Equiped,
					EEquipStatusMenuId.Preset,
					EEquipStatusMenuId.NotEquiped
				});
			}
			else
			{
				equipStatusMenu = new EquipStatusMenu(1, new List<EEquipStatusMenuId>
				{
					EEquipStatusMenuId.Preset
				});
			}
			if (!true)
			{
			}
			yield return equipStatusMenu;
			yield break;
		}

		// Token: 0x0600421C RID: 16924 RVA: 0x002033C8 File Offset: 0x002015C8
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(3, ToggleKey.CreateIndexKey(6))
			};
		}

		// Token: 0x04002ECC RID: 11980
		protected ESortAndFilterControllerType _sortAndFilterControllerType = ESortAndFilterControllerType.Item;
	}
}
