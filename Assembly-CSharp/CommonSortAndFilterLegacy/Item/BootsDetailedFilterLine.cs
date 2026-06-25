using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004B6 RID: 1206
	public class BootsDetailedFilterLine : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x17000730 RID: 1840
		// (get) Token: 0x060041C8 RID: 16840 RVA: 0x00202944 File Offset: 0x00200B44
		public override int Id
		{
			get
			{
				return 18;
			}
		}

		// Token: 0x17000731 RID: 1841
		// (get) Token: 0x060041C9 RID: 16841 RVA: 0x00202948 File Offset: 0x00200B48
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x060041CA RID: 16842 RVA: 0x0020294B File Offset: 0x00200B4B
		public BootsDetailedFilterLine(ESortAndFilterControllerType sortAndFilterControllerType = ESortAndFilterControllerType.Item)
		{
			this._sortAndFilterControllerType = sortAndFilterControllerType;
		}

		// Token: 0x060041CB RID: 16843 RVA: 0x00202963 File Offset: 0x00200B63
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new BootsMaterialMenu();
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
			yield return new BootsFabricHardnessMenu();
			yield return new BootsWoodHardnessMenu();
			yield return new BootsJadeHardnessMenu();
			yield return new BootsMetalHardnessMenu();
			yield break;
		}

		// Token: 0x060041CC RID: 16844 RVA: 0x00202974 File Offset: 0x00200B74
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(3, ToggleKey.CreateIndexKey(4))
			};
		}

		// Token: 0x04002EB2 RID: 11954
		protected ESortAndFilterControllerType _sortAndFilterControllerType = ESortAndFilterControllerType.Item;
	}
}
