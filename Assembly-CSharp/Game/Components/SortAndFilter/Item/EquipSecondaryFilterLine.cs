using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DDC RID: 3548
	public class EquipSecondaryFilterLine : SecondaryFilterLineLogic<ITradeableContent>
	{
		// Token: 0x170012A5 RID: 4773
		// (get) Token: 0x0600AA3B RID: 43579 RVA: 0x004E7A01 File Offset: 0x004E5C01
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x170012A6 RID: 4774
		// (get) Token: 0x0600AA3C RID: 43580 RVA: 0x004E7A04 File Offset: 0x004E5C04
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600AA3D RID: 43581 RVA: 0x004E7A07 File Offset: 0x004E5C07
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new EquipTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x0600AA3E RID: 43582 RVA: 0x004E7A18 File Offset: 0x004E5C18
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(2))
			};
		}
	}
}
