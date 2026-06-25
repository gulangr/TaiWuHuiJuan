using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D49 RID: 3401
	public class HelmDetailedFilterLine : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x170011CF RID: 4559
		// (get) Token: 0x0600A7E8 RID: 42984 RVA: 0x004E027C File Offset: 0x004DE47C
		public override int Id
		{
			get
			{
				return 15;
			}
		}

		// Token: 0x170011D0 RID: 4560
		// (get) Token: 0x0600A7E9 RID: 42985 RVA: 0x004E0280 File Offset: 0x004DE480
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A7EA RID: 42986 RVA: 0x004E0283 File Offset: 0x004DE483
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new EquipStatusMenu(5, new List<EquipStatusMenu.EEquipStatusMenuId>
			{
				EquipStatusMenu.EEquipStatusMenuId.Equiped,
				EquipStatusMenu.EEquipStatusMenuId.Preset,
				EquipStatusMenu.EEquipStatusMenuId.NotEquiped
			});
			yield return new HelmMaterialMenu();
			yield return new HelmFabricHardnessMenu();
			yield return new HelmWoodHardnessMenu();
			yield return new HelmJadeHardnessMenu();
			yield return new HelmMetalHardnessMenu();
			yield break;
		}

		// Token: 0x0600A7EB RID: 42987 RVA: 0x004E0294 File Offset: 0x004DE494
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(3, ToggleKey.CreateIndexKey(1))
			};
		}
	}
}
