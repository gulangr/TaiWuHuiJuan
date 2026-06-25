using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x0200051F RID: 1311
	public class MaterialSecondaryFilterLine : SecondaryFilterLineBase<ItemDisplayData>
	{
		// Token: 0x170007CC RID: 1996
		// (get) Token: 0x0600433C RID: 17212 RVA: 0x0020652B File Offset: 0x0020472B
		public override int Id
		{
			get
			{
				return 6;
			}
		}

		// Token: 0x170007CD RID: 1997
		// (get) Token: 0x0600433D RID: 17213 RVA: 0x0020652E File Offset: 0x0020472E
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600433E RID: 17214 RVA: 0x00206531 File Offset: 0x00204731
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new MaterialTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x0600433F RID: 17215 RVA: 0x00206544 File Offset: 0x00204744
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(5))
			};
		}
	}
}
