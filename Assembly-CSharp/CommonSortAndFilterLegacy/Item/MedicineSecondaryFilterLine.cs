using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000543 RID: 1347
	public class MedicineSecondaryFilterLine : SecondaryFilterLineBase<ItemDisplayData>
	{
		// Token: 0x170007F5 RID: 2037
		// (get) Token: 0x060043A8 RID: 17320 RVA: 0x00207D32 File Offset: 0x00205F32
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x170007F6 RID: 2038
		// (get) Token: 0x060043A9 RID: 17321 RVA: 0x00207D35 File Offset: 0x00205F35
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x060043AA RID: 17322 RVA: 0x00207D38 File Offset: 0x00205F38
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new MedicineTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x060043AB RID: 17323 RVA: 0x00207D48 File Offset: 0x00205F48
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(1))
			};
		}
	}
}
