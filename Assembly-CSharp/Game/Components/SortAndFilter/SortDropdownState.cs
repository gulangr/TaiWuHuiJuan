using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CCF RID: 3279
	internal class SortDropdownState
	{
		// Token: 0x17001149 RID: 4425
		// (get) Token: 0x0600A5C7 RID: 42439 RVA: 0x004D3FFD File Offset: 0x004D21FD
		public List<SortItemState> ValidStates
		{
			get
			{
				return (from t in this.ItemStates
				where (this.DisplayingSortIds.Count == 0 || this.DisplayingSortIds.Contains(t.SortId)) && !this.IsSortIdPendingForRemove(t.SortId)
				select t).ToList<SortItemState>();
			}
		}

		// Token: 0x1700114A RID: 4426
		// (get) Token: 0x0600A5C8 RID: 42440 RVA: 0x004D401B File Offset: 0x004D221B
		public List<SortItemState> AllStates
		{
			get
			{
				return (from t in this.ItemStates
				where !this.IsSortIdPendingForRemove(t.SortId)
				select t).ToList<SortItemState>();
			}
		}

		// Token: 0x0600A5C9 RID: 42441 RVA: 0x004D403C File Offset: 0x004D223C
		public bool IsSortIdPendingForRemove(short sortId)
		{
			return this._pendingRemovalInfo != null && this._pendingRemovalInfo.Value.SortId == sortId;
		}

		// Token: 0x1700114B RID: 4427
		// (get) Token: 0x0600A5CA RID: 42442 RVA: 0x004D4071 File Offset: 0x004D2271
		public bool HasPendingRemoval
		{
			get
			{
				return this._pendingRemovalInfo != null;
			}
		}

		// Token: 0x0600A5CB RID: 42443 RVA: 0x004D407E File Offset: 0x004D227E
		public void ClearPendingRemoval()
		{
			this._pendingRemovalInfo = null;
		}

		// Token: 0x0600A5CC RID: 42444 RVA: 0x004D4090 File Offset: 0x004D2290
		public void SetPendingRemoval(short sortId, int index)
		{
			this._pendingRemovalInfo = new PendingRemovalInfo?(new PendingRemovalInfo
			{
				SortId = sortId,
				IdOrder = index
			});
		}

		// Token: 0x0600A5CD RID: 42445 RVA: 0x004D40C4 File Offset: 0x004D22C4
		public void RestoreStateFromPendingRemoval()
		{
			bool flag = this._pendingRemovalInfo != null;
			if (flag)
			{
				PendingRemovalInfo info = this._pendingRemovalInfo.Value;
				this.ItemStates[info.IdOrder] = new SortItemState
				{
					SortId = info.SortId,
					SortDirection = ESortDirection.Descending
				};
				this._pendingRemovalInfo = null;
			}
		}

		// Token: 0x040082E2 RID: 33506
		internal List<SortItemState> ItemStates;

		// Token: 0x040082E3 RID: 33507
		public HashSet<short> DisplayingSortIds = new HashSet<short>();

		// Token: 0x040082E4 RID: 33508
		private PendingRemovalInfo? _pendingRemovalInfo;
	}
}
