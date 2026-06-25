using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D59 RID: 3417
	public class BracersDetailedFilterLine : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x170011ED RID: 4589
		// (get) Token: 0x0600A816 RID: 43030 RVA: 0x004E0413 File Offset: 0x004DE613
		public override int Id
		{
			get
			{
				return 17;
			}
		}

		// Token: 0x170011EE RID: 4590
		// (get) Token: 0x0600A817 RID: 43031 RVA: 0x004E0417 File Offset: 0x004DE617
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A818 RID: 43032 RVA: 0x004E041A File Offset: 0x004DE61A
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new EquipStatusMenu(5, new List<EquipStatusMenu.EEquipStatusMenuId>
			{
				EquipStatusMenu.EEquipStatusMenuId.Equiped,
				EquipStatusMenu.EEquipStatusMenuId.Preset,
				EquipStatusMenu.EEquipStatusMenuId.NotEquiped
			});
			yield return new BracersMaterialMenu();
			yield return new BracersFabricHardnessMenu();
			yield return new BracersWoodHardnessMenu();
			yield return new BracersJadeHardnessMenu();
			yield return new BracersMetalHardnessMenu();
			yield break;
		}

		// Token: 0x0600A819 RID: 43033 RVA: 0x004E042C File Offset: 0x004DE62C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(3, ToggleKey.CreateIndexKey(3))
			};
		}
	}
}
