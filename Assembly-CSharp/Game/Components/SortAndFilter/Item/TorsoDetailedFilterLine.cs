using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D51 RID: 3409
	public class TorsoDetailedFilterLine : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x170011DE RID: 4574
		// (get) Token: 0x0600A7FF RID: 43007 RVA: 0x004E0347 File Offset: 0x004DE547
		public override int Id
		{
			get
			{
				return 16;
			}
		}

		// Token: 0x170011DF RID: 4575
		// (get) Token: 0x0600A800 RID: 43008 RVA: 0x004E034B File Offset: 0x004DE54B
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A801 RID: 43009 RVA: 0x004E034E File Offset: 0x004DE54E
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new EquipStatusMenu(5, new List<EquipStatusMenu.EEquipStatusMenuId>
			{
				EquipStatusMenu.EEquipStatusMenuId.Equiped,
				EquipStatusMenu.EEquipStatusMenuId.Preset,
				EquipStatusMenu.EEquipStatusMenuId.NotEquiped
			});
			yield return new TorsoMaterialMenu();
			yield return new TorsoFabricHardnessMenu();
			yield return new TorsoWoodHardnessMenu();
			yield return new TorsoJadeHardnessMenu();
			yield return new TorsoMetalHardnessMenu();
			yield break;
		}

		// Token: 0x0600A802 RID: 43010 RVA: 0x004E0360 File Offset: 0x004DE560
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(3, ToggleKey.CreateIndexKey(2))
			};
		}
	}
}
