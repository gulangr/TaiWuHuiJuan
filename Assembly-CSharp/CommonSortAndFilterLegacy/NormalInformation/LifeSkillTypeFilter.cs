using System;
using System.Collections.Generic;
using GameData.Domains.Information;

namespace CommonSortAndFilterLegacy.NormalInformation
{
	// Token: 0x0200047B RID: 1147
	public class LifeSkillTypeFilter : DetailedFilterLine<NormalInformationDisplayData>
	{
		// Token: 0x170006DF RID: 1759
		// (get) Token: 0x060040F1 RID: 16625 RVA: 0x002009BC File Offset: 0x001FEBBC
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x060040F2 RID: 16626 RVA: 0x002009BF File Offset: 0x001FEBBF
		protected override IEnumerable<DetailedFilterMenuBase<NormalInformationDisplayData>> GenerateMenus()
		{
			yield return new LifeSkillTypeMenu();
			yield break;
		}

		// Token: 0x170006E0 RID: 1760
		// (get) Token: 0x060040F3 RID: 16627 RVA: 0x002009CF File Offset: 0x001FEBCF
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x170006E1 RID: 1761
		// (get) Token: 0x060040F4 RID: 16628 RVA: 0x002009D2 File Offset: 0x001FEBD2
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060040F5 RID: 16629 RVA: 0x002009D8 File Offset: 0x001FEBD8
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(2))
			};
		}
	}
}
