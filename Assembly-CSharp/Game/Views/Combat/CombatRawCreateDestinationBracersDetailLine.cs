using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Item;

namespace Game.Views.Combat
{
	// Token: 0x02000B12 RID: 2834
	public sealed class CombatRawCreateDestinationBracersDetailLine : BracersDetailedFilterLine
	{
		// Token: 0x17000F5E RID: 3934
		// (get) Token: 0x06008B2D RID: 35629 RVA: 0x00405DE6 File Offset: 0x00403FE6
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000F5F RID: 3935
		// (get) Token: 0x06008B2E RID: 35630 RVA: 0x00405DE9 File Offset: 0x00403FE9
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06008B2F RID: 35631 RVA: 0x00405DEC File Offset: 0x00403FEC
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
