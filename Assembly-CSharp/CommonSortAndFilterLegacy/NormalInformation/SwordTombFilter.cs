using System;
using System.Collections.Generic;
using GameData.Domains.Information;

namespace CommonSortAndFilterLegacy.NormalInformation
{
	// Token: 0x02000486 RID: 1158
	public class SwordTombFilter : DetailedFilterLine<NormalInformationDisplayData>
	{
		// Token: 0x170006EF RID: 1775
		// (get) Token: 0x06004118 RID: 16664 RVA: 0x00200DB8 File Offset: 0x001FEFB8
		public override int Id
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x06004119 RID: 16665 RVA: 0x00200DBB File Offset: 0x001FEFBB
		protected override IEnumerable<DetailedFilterMenuBase<NormalInformationDisplayData>> GenerateMenus()
		{
			yield return new SwordTombMenu();
			yield break;
		}

		// Token: 0x170006F0 RID: 1776
		// (get) Token: 0x0600411A RID: 16666 RVA: 0x00200DCB File Offset: 0x001FEFCB
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x170006F1 RID: 1777
		// (get) Token: 0x0600411B RID: 16667 RVA: 0x00200DCE File Offset: 0x001FEFCE
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600411C RID: 16668 RVA: 0x00200DD4 File Offset: 0x001FEFD4
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(5))
			};
		}
	}
}
