using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004D2 RID: 1234
	public class HelmDetailedFilterLine : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x17000763 RID: 1891
		// (get) Token: 0x06004237 RID: 16951 RVA: 0x00203AFB File Offset: 0x00201CFB
		public override int Id
		{
			get
			{
				return 15;
			}
		}

		// Token: 0x17000764 RID: 1892
		// (get) Token: 0x06004238 RID: 16952 RVA: 0x00203AFF File Offset: 0x00201CFF
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x06004239 RID: 16953 RVA: 0x00203B02 File Offset: 0x00201D02
		public HelmDetailedFilterLine(ESortAndFilterControllerType sortAndFilterControllerType = ESortAndFilterControllerType.Item)
		{
			this._sortAndFilterControllerType = sortAndFilterControllerType;
		}

		// Token: 0x0600423A RID: 16954 RVA: 0x00203B1A File Offset: 0x00201D1A
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new HelmMaterialMenu();
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
			yield return new HelmFabricHardnessMenu();
			yield return new HelmWoodHardnessMenu();
			yield return new HelmJadeHardnessMenu();
			yield return new HelmMetalHardnessMenu();
			yield return new HelmHerbMakeTypeMenu();
			yield break;
		}

		// Token: 0x0600423B RID: 16955 RVA: 0x00203B2C File Offset: 0x00201D2C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(3, ToggleKey.CreateIndexKey(1))
			};
		}

		// Token: 0x04002EDB RID: 11995
		protected ESortAndFilterControllerType _sortAndFilterControllerType = ESortAndFilterControllerType.Item;
	}
}
