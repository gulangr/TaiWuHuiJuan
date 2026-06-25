using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D3C RID: 3388
	public class WeaponDetailedFilterLine : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x170011B5 RID: 4533
		// (get) Token: 0x0600A7AD RID: 42925 RVA: 0x004DF871 File Offset: 0x004DDA71
		public override int Id
		{
			get
			{
				return 14;
			}
		}

		// Token: 0x170011B6 RID: 4534
		// (get) Token: 0x0600A7AE RID: 42926 RVA: 0x004DF875 File Offset: 0x004DDA75
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A7AF RID: 42927 RVA: 0x004DF878 File Offset: 0x004DDA78
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new EquipStatusMenu(7, new List<EquipStatusMenu.EEquipStatusMenuId>
			{
				EquipStatusMenu.EEquipStatusMenuId.Equiped,
				EquipStatusMenu.EEquipStatusMenuId.Preset,
				EquipStatusMenu.EEquipStatusMenuId.NotEquiped
			});
			yield return new WeaponSubTypeMenu();
			yield return new WeaponMaterialMenu();
			yield return new WeaponFabricHardnessMenu();
			yield return new WeaponWoodHardnessMenu();
			yield return new WeaponJadeHardnessMenu();
			yield return new WeaponMetalHardnessMenu();
			yield return new WeaponHerbMakeTypeMenu();
			yield break;
		}

		// Token: 0x0600A7B0 RID: 42928 RVA: 0x004DF888 File Offset: 0x004DDA88
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(3, ToggleKey.CreateIndexKey(0))
			};
		}
	}
}
