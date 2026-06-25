using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000437 RID: 1079
	public abstract class DetailedFilterMenuBase<TData>
	{
		// Token: 0x06003FED RID: 16365 RVA: 0x001FCF3A File Offset: 0x001FB13A
		public virtual void OnInit()
		{
		}

		// Token: 0x06003FEE RID: 16366
		public abstract DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig();

		// Token: 0x06003FEF RID: 16367
		public abstract List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs();

		// Token: 0x17000686 RID: 1670
		// (get) Token: 0x06003FF0 RID: 16368
		public abstract bool EnableDragHold { get; }

		// Token: 0x06003FF1 RID: 16369
		public abstract List<DetailFilterMultiSelectDropdownItemConfig> GetDynamicMenuConfigs(List<TData> dataList);

		// Token: 0x06003FF2 RID: 16370
		public abstract bool IsDataMatch(TData data, IReadOnlyCollection<int> selectedIndices);

		// Token: 0x17000687 RID: 1671
		// (get) Token: 0x06003FF3 RID: 16371
		public abstract int Id { get; }

		// Token: 0x17000688 RID: 1672
		// (get) Token: 0x06003FF4 RID: 16372 RVA: 0x001FCF40 File Offset: 0x001FB140
		public virtual MenuOptionIndex? Dependency
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000689 RID: 1673
		// (get) Token: 0x06003FF5 RID: 16373 RVA: 0x001FCF56 File Offset: 0x001FB156
		public virtual int Version
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x1700068A RID: 1674
		// (get) Token: 0x06003FF6 RID: 16374
		public abstract FilterLogic LogicType { get; }
	}
}
