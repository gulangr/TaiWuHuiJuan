using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DBB RID: 3515
	public class MaterialFabricDetailedFilterLine : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x17001277 RID: 4727
		// (get) Token: 0x0600A991 RID: 43409 RVA: 0x004E5D77 File Offset: 0x004E3F77
		public override int Id
		{
			get
			{
				return 28;
			}
		}

		// Token: 0x17001278 RID: 4728
		// (get) Token: 0x0600A992 RID: 43410 RVA: 0x004E5D7B File Offset: 0x004E3F7B
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A993 RID: 43411 RVA: 0x004E5D7E File Offset: 0x004E3F7E
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new FabricHardnessMenu();
			yield break;
		}

		// Token: 0x0600A994 RID: 43412 RVA: 0x004E5D90 File Offset: 0x004E3F90
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(6, ToggleKey.CreateIndexKey(0))
			};
		}
	}
}
