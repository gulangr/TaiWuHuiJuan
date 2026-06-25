using System;
using System.Collections.Generic;

namespace CommonSortAndFilterLegacy.CharacterTable
{
	// Token: 0x0200057A RID: 1402
	public class CharacterTableFallenFilterLine : DetailedFilterLine<CharacterTableSortAndFilterData>
	{
		// Token: 0x17000843 RID: 2115
		// (get) Token: 0x060044AA RID: 17578 RVA: 0x0020A635 File Offset: 0x00208835
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060044AB RID: 17579 RVA: 0x0020A638 File Offset: 0x00208838
		protected override IEnumerable<DetailedFilterMenuBase<CharacterTableSortAndFilterData>> GenerateMenus()
		{
			yield return new FallenMenu();
			yield break;
		}

		// Token: 0x17000844 RID: 2116
		// (get) Token: 0x060044AC RID: 17580 RVA: 0x0020A648 File Offset: 0x00208848
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000845 RID: 2117
		// (get) Token: 0x060044AD RID: 17581 RVA: 0x0020A64B File Offset: 0x0020884B
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060044AE RID: 17582 RVA: 0x0020A650 File Offset: 0x00208850
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
