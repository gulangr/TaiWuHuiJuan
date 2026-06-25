using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004BF RID: 1215
	public class BracersDetailedFilterLine : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x17000742 RID: 1858
		// (get) Token: 0x060041EA RID: 16874 RVA: 0x00202D6C File Offset: 0x00200F6C
		public override int Id
		{
			get
			{
				return 17;
			}
		}

		// Token: 0x17000743 RID: 1859
		// (get) Token: 0x060041EB RID: 16875 RVA: 0x00202D70 File Offset: 0x00200F70
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x060041EC RID: 16876 RVA: 0x00202D73 File Offset: 0x00200F73
		public BracersDetailedFilterLine(ESortAndFilterControllerType sortAndFilterControllerType = ESortAndFilterControllerType.Item)
		{
			this._sortAndFilterControllerType = sortAndFilterControllerType;
		}

		// Token: 0x060041ED RID: 16877 RVA: 0x00202D8B File Offset: 0x00200F8B
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new BracersMaterialMenu();
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
			yield return new BracersFabricHardnessMenu();
			yield return new BracersWoodHardnessMenu();
			yield return new BracersJadeHardnessMenu();
			yield return new BracersMetalHardnessMenu();
			yield break;
		}

		// Token: 0x060041EE RID: 16878 RVA: 0x00202D9C File Offset: 0x00200F9C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(3, ToggleKey.CreateIndexKey(3))
			};
		}

		// Token: 0x04002EC1 RID: 11969
		protected ESortAndFilterControllerType _sortAndFilterControllerType = ESortAndFilterControllerType.Item;
	}
}
