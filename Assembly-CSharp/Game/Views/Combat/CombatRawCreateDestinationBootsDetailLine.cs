using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Item;

namespace Game.Views.Combat
{
	// Token: 0x02000B13 RID: 2835
	public sealed class CombatRawCreateDestinationBootsDetailLine : BootsDetailedFilterLine
	{
		// Token: 0x17000F60 RID: 3936
		// (get) Token: 0x06008B31 RID: 35633 RVA: 0x00405DF8 File Offset: 0x00403FF8
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000F61 RID: 3937
		// (get) Token: 0x06008B32 RID: 35634 RVA: 0x00405DFB File Offset: 0x00403FFB
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06008B33 RID: 35635 RVA: 0x00405DFE File Offset: 0x00403FFE
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
