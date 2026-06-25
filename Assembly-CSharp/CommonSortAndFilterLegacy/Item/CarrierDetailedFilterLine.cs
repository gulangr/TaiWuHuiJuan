using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004C7 RID: 1223
	public class CarrierDetailedFilterLine : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x17000754 RID: 1876
		// (get) Token: 0x0600420C RID: 16908 RVA: 0x00203194 File Offset: 0x00201394
		public override int Id
		{
			get
			{
				return 21;
			}
		}

		// Token: 0x17000755 RID: 1877
		// (get) Token: 0x0600420D RID: 16909 RVA: 0x00203198 File Offset: 0x00201398
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600420E RID: 16910 RVA: 0x0020319B File Offset: 0x0020139B
		public CarrierDetailedFilterLine(ESortAndFilterControllerType sortAndFilterControllerType = ESortAndFilterControllerType.Item)
		{
			this._sortAndFilterControllerType = sortAndFilterControllerType;
		}

		// Token: 0x0600420F RID: 16911 RVA: 0x002031B3 File Offset: 0x002013B3
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new CarrierSubTypeMenu();
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

		// Token: 0x06004210 RID: 16912 RVA: 0x002031C4 File Offset: 0x002013C4
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(3, ToggleKey.CreateIndexKey(7))
			};
		}

		// Token: 0x04002EC7 RID: 11975
		protected ESortAndFilterControllerType _sortAndFilterControllerType = ESortAndFilterControllerType.Item;
	}
}
