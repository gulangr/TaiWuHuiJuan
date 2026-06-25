using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DE3 RID: 3555
	public class MaterialAdditionalSecondaryFilterLine : SecondaryFilterLineLogic<ITradeableContent>
	{
		// Token: 0x170012B3 RID: 4787
		// (get) Token: 0x0600AA61 RID: 43617 RVA: 0x004E8093 File Offset: 0x004E6293
		public override int Id
		{
			get
			{
				return 45;
			}
		}

		// Token: 0x170012B4 RID: 4788
		// (get) Token: 0x0600AA62 RID: 43618 RVA: 0x004E8097 File Offset: 0x004E6297
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600AA63 RID: 43619 RVA: 0x004E809A File Offset: 0x004E629A
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new MaterialAdditionalSecondaryMenu();
			yield break;
		}

		// Token: 0x0600AA64 RID: 43620 RVA: 0x004E80AC File Offset: 0x004E62AC
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(5))
			};
		}
	}
}
