using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004CE RID: 1230
	public class EquipSecondaryFilterLine : SecondaryFilterLineBase<ItemDisplayData>
	{
		// Token: 0x1700075F RID: 1887
		// (get) Token: 0x0600422C RID: 16940 RVA: 0x002037DB File Offset: 0x002019DB
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x17000760 RID: 1888
		// (get) Token: 0x0600422D RID: 16941 RVA: 0x002037DE File Offset: 0x002019DE
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600422E RID: 16942 RVA: 0x002037E1 File Offset: 0x002019E1
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new EquipTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x0600422F RID: 16943 RVA: 0x002037F4 File Offset: 0x002019F4
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(2))
			};
		}
	}
}
