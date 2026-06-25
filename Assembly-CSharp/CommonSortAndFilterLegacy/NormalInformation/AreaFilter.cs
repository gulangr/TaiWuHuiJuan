using System;
using System.Collections.Generic;
using GameData.Domains.Information;

namespace CommonSortAndFilterLegacy.NormalInformation
{
	// Token: 0x02000478 RID: 1144
	public class AreaFilter : DetailedFilterLine<NormalInformationDisplayData>
	{
		// Token: 0x170006DA RID: 1754
		// (get) Token: 0x060040E5 RID: 16613 RVA: 0x00200858 File Offset: 0x001FEA58
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x060040E6 RID: 16614 RVA: 0x0020085B File Offset: 0x001FEA5B
		protected override IEnumerable<DetailedFilterMenuBase<NormalInformationDisplayData>> GenerateMenus()
		{
			yield return new AreaMenu();
			yield break;
		}

		// Token: 0x170006DB RID: 1755
		// (get) Token: 0x060040E7 RID: 16615 RVA: 0x0020086B File Offset: 0x001FEA6B
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x170006DC RID: 1756
		// (get) Token: 0x060040E8 RID: 16616 RVA: 0x0020086E File Offset: 0x001FEA6E
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060040E9 RID: 16617 RVA: 0x00200874 File Offset: 0x001FEA74
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(0))
			};
		}
	}
}
