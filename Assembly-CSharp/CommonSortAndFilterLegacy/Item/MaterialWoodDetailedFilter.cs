using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x0200053B RID: 1339
	public class MaterialWoodDetailedFilter : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x170007EB RID: 2027
		// (get) Token: 0x0600438D RID: 17293 RVA: 0x00207511 File Offset: 0x00205711
		public override int Id
		{
			get
			{
				return 26;
			}
		}

		// Token: 0x0600438E RID: 17294 RVA: 0x00207515 File Offset: 0x00205715
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new WoodMakeTypeMenu();
			yield return new WoodHardnessMenu();
			yield break;
		}

		// Token: 0x170007EC RID: 2028
		// (get) Token: 0x0600438F RID: 17295 RVA: 0x00207525 File Offset: 0x00205725
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x06004390 RID: 17296 RVA: 0x00207528 File Offset: 0x00205728
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(6, ToggleKey.CreateIndexKey(1))
			};
		}
	}
}
