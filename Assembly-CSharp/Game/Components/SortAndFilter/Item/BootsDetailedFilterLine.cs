using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D61 RID: 3425
	public class BootsDetailedFilterLine : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x170011FC RID: 4604
		// (get) Token: 0x0600A82D RID: 43053 RVA: 0x004E04DF File Offset: 0x004DE6DF
		public override int Id
		{
			get
			{
				return 18;
			}
		}

		// Token: 0x170011FD RID: 4605
		// (get) Token: 0x0600A82E RID: 43054 RVA: 0x004E04E3 File Offset: 0x004DE6E3
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A82F RID: 43055 RVA: 0x004E04E6 File Offset: 0x004DE6E6
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new EquipStatusMenu(5, new List<EquipStatusMenu.EEquipStatusMenuId>
			{
				EquipStatusMenu.EEquipStatusMenuId.Equiped,
				EquipStatusMenu.EEquipStatusMenuId.Preset,
				EquipStatusMenu.EEquipStatusMenuId.NotEquiped
			});
			yield return new BootsMaterialMenu();
			yield return new BootsFabricHardnessMenu();
			yield return new BootsWoodHardnessMenu();
			yield return new BootsJadeHardnessMenu();
			yield return new BootsMetalHardnessMenu();
			yield break;
		}

		// Token: 0x0600A830 RID: 43056 RVA: 0x004E04F8 File Offset: 0x004DE6F8
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(3, ToggleKey.CreateIndexKey(4))
			};
		}
	}
}
