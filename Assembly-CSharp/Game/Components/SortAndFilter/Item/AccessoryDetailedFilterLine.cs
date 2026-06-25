using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D69 RID: 3433
	public class AccessoryDetailedFilterLine : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x1700120B RID: 4619
		// (get) Token: 0x0600A844 RID: 43076 RVA: 0x004E05AB File Offset: 0x004DE7AB
		public override int Id
		{
			get
			{
				return 19;
			}
		}

		// Token: 0x1700120C RID: 4620
		// (get) Token: 0x0600A845 RID: 43077 RVA: 0x004E05AF File Offset: 0x004DE7AF
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A846 RID: 43078 RVA: 0x004E05B2 File Offset: 0x004DE7B2
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new EquipStatusMenu(6, new List<EquipStatusMenu.EEquipStatusMenuId>
			{
				EquipStatusMenu.EEquipStatusMenuId.Equiped,
				EquipStatusMenu.EEquipStatusMenuId.Preset,
				EquipStatusMenu.EEquipStatusMenuId.NotEquiped
			});
			yield return new AccessoryMaterialMenu();
			yield return new AccessoryPropMenu();
			yield return new AccessoryFabricHardnessMenu();
			yield return new AccessoryWoodHardnessMenu();
			yield return new AccessoryJadeHardnessMenu();
			yield return new AccessoryMetalHardnessMenu();
			yield break;
		}

		// Token: 0x0600A847 RID: 43079 RVA: 0x004E05C4 File Offset: 0x004DE7C4
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(3, ToggleKey.CreateIndexKey(5))
			};
		}
	}
}
