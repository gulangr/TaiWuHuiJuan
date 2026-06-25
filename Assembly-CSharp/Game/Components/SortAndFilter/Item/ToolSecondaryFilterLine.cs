using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DE0 RID: 3552
	public class ToolSecondaryFilterLine : SecondaryFilterLineLogic<ITradeableContent>
	{
		// Token: 0x170012AD RID: 4781
		// (get) Token: 0x0600AA51 RID: 43601 RVA: 0x004E7E71 File Offset: 0x004E6071
		public override int Id
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x170012AE RID: 4782
		// (get) Token: 0x0600AA52 RID: 43602 RVA: 0x004E7E74 File Offset: 0x004E6074
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600AA53 RID: 43603 RVA: 0x004E7E77 File Offset: 0x004E6077
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new ToolTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x0600AA54 RID: 43604 RVA: 0x004E7E88 File Offset: 0x004E6088
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(4))
			};
		}
	}
}
