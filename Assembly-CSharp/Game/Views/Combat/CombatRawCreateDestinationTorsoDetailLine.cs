using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Item;

namespace Game.Views.Combat
{
	// Token: 0x02000B11 RID: 2833
	public sealed class CombatRawCreateDestinationTorsoDetailLine : TorsoDetailedFilterLine
	{
		// Token: 0x17000F5C RID: 3932
		// (get) Token: 0x06008B29 RID: 35625 RVA: 0x00405DD4 File Offset: 0x00403FD4
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000F5D RID: 3933
		// (get) Token: 0x06008B2A RID: 35626 RVA: 0x00405DD7 File Offset: 0x00403FD7
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06008B2B RID: 35627 RVA: 0x00405DDA File Offset: 0x00403FDA
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
