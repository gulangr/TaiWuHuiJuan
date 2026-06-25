using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D77 RID: 3447
	public class LivestockCarrierDetailedFilterLine : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x17001227 RID: 4647
		// (get) Token: 0x0600A884 RID: 43140 RVA: 0x004E0F7F File Offset: 0x004DF17F
		public override int Id
		{
			get
			{
				return 22;
			}
		}

		// Token: 0x17001228 RID: 4648
		// (get) Token: 0x0600A885 RID: 43141 RVA: 0x004E0F83 File Offset: 0x004DF183
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A886 RID: 43142 RVA: 0x004E0F86 File Offset: 0x004DF186
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new LivestockCarrierSubTypeMenu();
			yield break;
		}

		// Token: 0x0600A887 RID: 43143 RVA: 0x004E0F98 File Offset: 0x004DF198
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(3, ToggleKey.CreateIndexKey(8))
			};
		}
	}
}
