using System;
using System.Collections.Generic;
using GameData.Domains.Building;

namespace Game.Components.SortAndFilter.SelectChicken
{
	// Token: 0x02000CDA RID: 3290
	public class SelectChickenMainFilterLine : DetailedFilterLineLogic<Chicken>
	{
		// Token: 0x1700115A RID: 4442
		// (get) Token: 0x0600A62F RID: 42543 RVA: 0x004D619C File Offset: 0x004D439C
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700115B RID: 4443
		// (get) Token: 0x0600A630 RID: 42544 RVA: 0x004D619F File Offset: 0x004D439F
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700115C RID: 4444
		// (get) Token: 0x0600A631 RID: 42545 RVA: 0x004D61A2 File Offset: 0x004D43A2
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600A632 RID: 42546 RVA: 0x004D61A5 File Offset: 0x004D43A5
		protected override IEnumerable<DetailedFilterMenuLogic<Chicken>> GenerateMenus()
		{
			yield break;
		}

		// Token: 0x0600A633 RID: 42547 RVA: 0x004D61B8 File Offset: 0x004D43B8
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
