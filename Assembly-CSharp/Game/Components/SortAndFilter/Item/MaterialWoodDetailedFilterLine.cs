using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DBE RID: 3518
	public class MaterialWoodDetailedFilterLine : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x1700127B RID: 4731
		// (get) Token: 0x0600A99D RID: 43421 RVA: 0x004E5E25 File Offset: 0x004E4025
		public override int Id
		{
			get
			{
				return 29;
			}
		}

		// Token: 0x1700127C RID: 4732
		// (get) Token: 0x0600A99E RID: 43422 RVA: 0x004E5E29 File Offset: 0x004E4029
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A99F RID: 43423 RVA: 0x004E5E2C File Offset: 0x004E402C
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new WoodHardnessMenu();
			yield break;
		}

		// Token: 0x0600A9A0 RID: 43424 RVA: 0x004E5E3C File Offset: 0x004E403C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(6, ToggleKey.CreateIndexKey(1))
			};
		}
	}
}
