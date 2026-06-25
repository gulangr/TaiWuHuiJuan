using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Item;

namespace Game.Views.Combat
{
	// Token: 0x02000B14 RID: 2836
	public sealed class CombatRawCreateDestinationAccessoryDetailLine : AccessoryDetailedFilterLine
	{
		// Token: 0x17000F62 RID: 3938
		// (get) Token: 0x06008B35 RID: 35637 RVA: 0x00405E0A File Offset: 0x0040400A
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000F63 RID: 3939
		// (get) Token: 0x06008B36 RID: 35638 RVA: 0x00405E0D File Offset: 0x0040400D
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06008B37 RID: 35639 RVA: 0x00405E10 File Offset: 0x00404010
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
