using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004DC RID: 1244
	public class TorsoDetailedFilterLine : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x17000778 RID: 1912
		// (get) Token: 0x0600425E RID: 16990 RVA: 0x00203F6A File Offset: 0x0020216A
		public override int Id
		{
			get
			{
				return 16;
			}
		}

		// Token: 0x17000779 RID: 1913
		// (get) Token: 0x0600425F RID: 16991 RVA: 0x00203F6E File Offset: 0x0020216E
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x06004260 RID: 16992 RVA: 0x00203F71 File Offset: 0x00202171
		public TorsoDetailedFilterLine(ESortAndFilterControllerType sortAndFilterControllerType = ESortAndFilterControllerType.Item)
		{
			this._sortAndFilterControllerType = sortAndFilterControllerType;
		}

		// Token: 0x06004261 RID: 16993 RVA: 0x00203F89 File Offset: 0x00202189
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new TorsoMaterialMenu();
			ESortAndFilterControllerType sortAndFilterControllerType = this._sortAndFilterControllerType;
			if (!true)
			{
			}
			EquipStatusMenu equipStatusMenu;
			if (sortAndFilterControllerType != ESortAndFilterControllerType.Equip)
			{
				equipStatusMenu = new EquipStatusMenu(5, new List<EEquipStatusMenuId>
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
				equipStatusMenu = new EquipStatusMenu(5, new List<EEquipStatusMenuId>
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
			yield return new TorsoFabricHardnessMenu();
			yield return new TorsoWoodHardnessMenu();
			yield return new TorsoJadeHardnessMenu();
			yield return new TorsoMetalHardnessMenu();
			yield break;
		}

		// Token: 0x06004262 RID: 16994 RVA: 0x00203F9C File Offset: 0x0020219C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(3, ToggleKey.CreateIndexKey(2))
			};
		}

		// Token: 0x04002EEA RID: 12010
		protected ESortAndFilterControllerType _sortAndFilterControllerType = ESortAndFilterControllerType.Item;
	}
}
