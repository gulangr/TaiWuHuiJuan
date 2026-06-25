using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000C90 RID: 3216
	public abstract class DetailedFilterMenuLogic<TData>
	{
		// Token: 0x1700111B RID: 4379
		// (get) Token: 0x0600A3E5 RID: 41957
		public abstract int Id { get; }

		// Token: 0x1700111C RID: 4380
		// (get) Token: 0x0600A3E6 RID: 41958 RVA: 0x004CA1D2 File Offset: 0x004C83D2
		public virtual EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x1700111D RID: 4381
		// (get) Token: 0x0600A3E7 RID: 41959 RVA: 0x004CA1D8 File Offset: 0x004C83D8
		public virtual MenuOptionIndex? Dependency
		{
			get
			{
				return null;
			}
		}

		// Token: 0x0600A3E8 RID: 41960 RVA: 0x004CA1EE File Offset: 0x004C83EE
		public virtual void OnInit()
		{
		}

		// Token: 0x0600A3E9 RID: 41961
		public abstract StringKey GetMenuBarLabel();

		// Token: 0x0600A3EA RID: 41962
		public abstract List<FilterDropdownItemConfig> GetMenuItemConfigs();

		// Token: 0x0600A3EB RID: 41963
		public abstract bool IsDataMatch(TData data, IReadOnlyCollection<int> selectedIndices);
	}
}
