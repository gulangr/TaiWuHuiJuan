using System;
using System.Collections.Generic;
using GameData.Domains.Information;

namespace CommonSortAndFilterLegacy.NormalInformation
{
	// Token: 0x0200048B RID: 1163
	public class WesternFilter : DetailedFilterLine<NormalInformationDisplayData>
	{
		// Token: 0x170006F6 RID: 1782
		// (get) Token: 0x0600412B RID: 16683 RVA: 0x00200F82 File Offset: 0x001FF182
		public override int Id
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x0600412C RID: 16684 RVA: 0x00200F85 File Offset: 0x001FF185
		protected override IEnumerable<DetailedFilterMenuBase<NormalInformationDisplayData>> GenerateMenus()
		{
			yield return new WesternMenu();
			yield break;
		}

		// Token: 0x170006F7 RID: 1783
		// (get) Token: 0x0600412D RID: 16685 RVA: 0x00200F95 File Offset: 0x001FF195
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x170006F8 RID: 1784
		// (get) Token: 0x0600412E RID: 16686 RVA: 0x00200F98 File Offset: 0x001FF198
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600412F RID: 16687 RVA: 0x00200F9C File Offset: 0x001FF19C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(3))
			};
		}
	}
}
