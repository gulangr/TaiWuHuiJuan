using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Item;

namespace Game.Views.Combat
{
	// Token: 0x02000B10 RID: 2832
	public sealed class CombatRawCreateDestinationHelmDetailLine : HelmDetailedFilterLine
	{
		// Token: 0x17000F5A RID: 3930
		// (get) Token: 0x06008B25 RID: 35621 RVA: 0x00405DC2 File Offset: 0x00403FC2
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000F5B RID: 3931
		// (get) Token: 0x06008B26 RID: 35622 RVA: 0x00405DC5 File Offset: 0x00403FC5
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06008B27 RID: 35623 RVA: 0x00405DC8 File Offset: 0x00403FC8
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
