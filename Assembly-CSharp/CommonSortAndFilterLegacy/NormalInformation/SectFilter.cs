using System;
using System.Collections.Generic;
using GameData.Domains.Information;

namespace CommonSortAndFilterLegacy.NormalInformation
{
	// Token: 0x02000483 RID: 1155
	public class SectFilter : DetailedFilterLine<NormalInformationDisplayData>
	{
		// Token: 0x170006EA RID: 1770
		// (get) Token: 0x0600410C RID: 16652 RVA: 0x00200C29 File Offset: 0x001FEE29
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600410D RID: 16653 RVA: 0x00200C2C File Offset: 0x001FEE2C
		protected override IEnumerable<DetailedFilterMenuBase<NormalInformationDisplayData>> GenerateMenus()
		{
			yield return new SectMenu();
			yield break;
		}

		// Token: 0x170006EB RID: 1771
		// (get) Token: 0x0600410E RID: 16654 RVA: 0x00200C3C File Offset: 0x001FEE3C
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x170006EC RID: 1772
		// (get) Token: 0x0600410F RID: 16655 RVA: 0x00200C3F File Offset: 0x001FEE3F
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06004110 RID: 16656 RVA: 0x00200C44 File Offset: 0x001FEE44
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(1))
			};
		}
	}
}
