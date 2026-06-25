using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CD3 RID: 3283
	public class TabSortStateManager<TTabKey, TData>
	{
		// Token: 0x0600A5F1 RID: 42481 RVA: 0x004D44AC File Offset: 0x004D26AC
		public TabSortStateManager(SortAndFilterController<TData> controller)
		{
			this._controller = controller;
		}

		// Token: 0x0600A5F2 RID: 42482 RVA: 0x004D44C8 File Offset: 0x004D26C8
		public void OnTabChange(TTabKey newTabKey)
		{
			bool flag = this._controller == null;
			if (!flag)
			{
				bool hasCurrentTab = this._hasCurrentTab;
				if (hasCurrentTab)
				{
					SortStateData currentSortData = this._controller.SortAndFilterState.SortData;
					this._tabSortStates[this._currentTabKey] = TabSortStateManager<TTabKey, TData>.CloneSortData(currentSortData);
				}
				this._currentTabKey = newTabKey;
				this._hasCurrentTab = true;
				SortStateData savedState;
				bool flag2;
				if (this._tabSortStates.TryGetValue(newTabKey, out savedState))
				{
					List<SortItemState> itemStates = savedState.ItemStates;
					flag2 = (itemStates != null && itemStates.Count > 0);
				}
				else
				{
					flag2 = false;
				}
				bool flag3 = flag2;
				if (flag3)
				{
					this._controller.SetSortState(TabSortStateManager<TTabKey, TData>.CloneSortData(savedState));
				}
				else
				{
					this._controller.SetSortState(new SortStateData
					{
						ItemStates = new List<SortItemState>()
					});
				}
				this._controller.AddSortIdByHead();
			}
		}

		// Token: 0x0600A5F3 RID: 42483 RVA: 0x004D459C File Offset: 0x004D279C
		public bool ShouldSort()
		{
			SortStateData savedState;
			bool flag;
			if (this._tabSortStates.TryGetValue(this._currentTabKey, out savedState))
			{
				List<SortItemState> itemStates = savedState.ItemStates;
				flag = (itemStates != null && itemStates.Count > 0);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			bool result;
			if (flag2)
			{
				result = true;
			}
			else
			{
				SortAndFilterController<TData> controller = this._controller;
				SortStateData currentState = (controller != null) ? controller.SortAndFilterState.SortData : null;
				bool flag3;
				if (currentState == null)
				{
					flag3 = false;
				}
				else
				{
					List<SortItemState> itemStates2 = currentState.ItemStates;
					int? num = (itemStates2 != null) ? new int?(itemStates2.Count) : null;
					int num2 = 0;
					flag3 = (num.GetValueOrDefault() > num2 & num != null);
				}
				result = flag3;
			}
			return result;
		}

		// Token: 0x17001151 RID: 4433
		// (get) Token: 0x0600A5F4 RID: 42484 RVA: 0x004D463A File Offset: 0x004D283A
		public TTabKey CurrentTabKey
		{
			get
			{
				return this._currentTabKey;
			}
		}

		// Token: 0x0600A5F5 RID: 42485 RVA: 0x004D4642 File Offset: 0x004D2842
		public void ClearAll()
		{
			this._tabSortStates.Clear();
			this._hasCurrentTab = false;
		}

		// Token: 0x0600A5F6 RID: 42486 RVA: 0x004D4658 File Offset: 0x004D2858
		private static SortStateData CloneSortData(SortStateData source)
		{
			bool flag = ((source != null) ? source.ItemStates : null) == null;
			SortStateData result;
			if (flag)
			{
				result = new SortStateData
				{
					ItemStates = new List<SortItemState>()
				};
			}
			else
			{
				result = new SortStateData
				{
					ItemStates = new List<SortItemState>(source.ItemStates)
				};
			}
			return result;
		}

		// Token: 0x040082F9 RID: 33529
		private readonly Dictionary<TTabKey, SortStateData> _tabSortStates = new Dictionary<TTabKey, SortStateData>();

		// Token: 0x040082FA RID: 33530
		private readonly SortAndFilterController<TData> _controller;

		// Token: 0x040082FB RID: 33531
		private TTabKey _currentTabKey;

		// Token: 0x040082FC RID: 33532
		private bool _hasCurrentTab;
	}
}
