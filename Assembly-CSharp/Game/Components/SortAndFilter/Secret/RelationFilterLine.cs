using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.Secret
{
	// Token: 0x02000CDC RID: 3292
	public class RelationFilterLine : DetailedFilterLineLogic<SecretSortAndFilterData>
	{
		// Token: 0x1700115D RID: 4445
		// (get) Token: 0x0600A639 RID: 42553 RVA: 0x004D63F4 File Offset: 0x004D45F4
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700115E RID: 4446
		// (get) Token: 0x0600A63A RID: 42554 RVA: 0x004D63F7 File Offset: 0x004D45F7
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700115F RID: 4447
		// (get) Token: 0x0600A63B RID: 42555 RVA: 0x004D63FA File Offset: 0x004D45FA
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600A63C RID: 42556 RVA: 0x004D6400 File Offset: 0x004D4600
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}

		// Token: 0x0600A63D RID: 42557 RVA: 0x004D6413 File Offset: 0x004D4613
		protected override IEnumerable<DetailedFilterMenuLogic<SecretSortAndFilterData>> GenerateMenus()
		{
			yield return new RelationFilterMenu();
			yield break;
		}
	}
}
