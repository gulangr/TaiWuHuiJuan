using System;
using System.Collections.Generic;
using GameData.Domains.Information;

namespace CommonSortAndFilterLegacy.NormalInformation
{
	// Token: 0x02000480 RID: 1152
	public class ProfessionFilter : DetailedFilterLine<NormalInformationDisplayData>
	{
		// Token: 0x170006E5 RID: 1765
		// (get) Token: 0x06004100 RID: 16640 RVA: 0x00200B0A File Offset: 0x001FED0A
		public override int Id
		{
			get
			{
				return 6;
			}
		}

		// Token: 0x06004101 RID: 16641 RVA: 0x00200B0D File Offset: 0x001FED0D
		protected override IEnumerable<DetailedFilterMenuBase<NormalInformationDisplayData>> GenerateMenus()
		{
			yield return new ProfessionMenu();
			yield break;
		}

		// Token: 0x170006E6 RID: 1766
		// (get) Token: 0x06004102 RID: 16642 RVA: 0x00200B1D File Offset: 0x001FED1D
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x170006E7 RID: 1767
		// (get) Token: 0x06004103 RID: 16643 RVA: 0x00200B20 File Offset: 0x001FED20
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06004104 RID: 16644 RVA: 0x00200B24 File Offset: 0x001FED24
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(6))
			};
		}
	}
}
