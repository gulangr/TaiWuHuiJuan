using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x0200045C RID: 1116
	public class SortUiState
	{
		// Token: 0x1700069C RID: 1692
		// (get) Token: 0x0600405E RID: 16478 RVA: 0x001FF47A File Offset: 0x001FD67A
		public List<SortItemState> ValidStates
		{
			get
			{
				return (from t in this.ItemStates
				where (this.DisplayingSortIds.Count == 0 || this.DisplayingSortIds.Contains(t.SortId)) && !this.IsSortIdPendingForRemove(t.SortId)
				select t).ToList<SortItemState>();
			}
		}

		// Token: 0x1700069D RID: 1693
		// (get) Token: 0x0600405F RID: 16479 RVA: 0x001FF498 File Offset: 0x001FD698
		public List<SortItemState> AllStates
		{
			get
			{
				return (from t in this.ItemStates
				where !this.IsSortIdPendingForRemove(t.SortId)
				select t).ToList<SortItemState>();
			}
		}

		// Token: 0x1700069E RID: 1694
		// (get) Token: 0x06004060 RID: 16480 RVA: 0x001FF4B6 File Offset: 0x001FD6B6
		public List<SortItemState> EffectiveStates
		{
			get
			{
				return (from t in this.ItemStates
				where !this.IsSortIdPendingForRemove(t.SortId) && (this.DisplayingSortIds.Count == 0 || this.DisplayingSortIds.Contains(t.SortId))
				select t).ToList<SortItemState>();
			}
		}

		// Token: 0x06004061 RID: 16481 RVA: 0x001FF4D4 File Offset: 0x001FD6D4
		public bool IsSortIdPendingForRemove(short sortId)
		{
			return this.PendingRemovalInfo != null && this.PendingRemovalInfo.Value.SortId == sortId;
		}

		// Token: 0x1700069F RID: 1695
		// (get) Token: 0x06004062 RID: 16482 RVA: 0x001FF509 File Offset: 0x001FD709
		public bool HasPendingRemoval
		{
			get
			{
				return this.PendingRemovalInfo != null;
			}
		}

		// Token: 0x06004063 RID: 16483 RVA: 0x001FF516 File Offset: 0x001FD716
		public void ClearPendingRemoval()
		{
			this.PendingRemovalInfo = null;
		}

		// Token: 0x06004064 RID: 16484 RVA: 0x001FF528 File Offset: 0x001FD728
		public void SetPendingRemoval(short sortId, int index)
		{
			this.PendingRemovalInfo = new PendingRemovalInfo?(new PendingRemovalInfo
			{
				SortId = sortId,
				IdOrder = index
			});
		}

		// Token: 0x06004065 RID: 16485 RVA: 0x001FF55C File Offset: 0x001FD75C
		public void RestoreStateFromPendingRemoval()
		{
			bool flag = this.PendingRemovalInfo != null;
			if (flag)
			{
				PendingRemovalInfo info = this.PendingRemovalInfo.Value;
				this.ItemStates[info.IdOrder] = new SortItemState
				{
					SortId = info.SortId,
					SortDirection = ESortDirection.Descending
				};
				this.PendingRemovalInfo = null;
			}
		}

		// Token: 0x04002E0D RID: 11789
		internal List<SortItemState> ItemStates;

		// Token: 0x04002E0E RID: 11790
		public HashSet<short> DisplayingSortIds = new HashSet<short>();

		// Token: 0x04002E0F RID: 11791
		private PendingRemovalInfo? PendingRemovalInfo = new PendingRemovalInfo?(default(PendingRemovalInfo));
	}
}
