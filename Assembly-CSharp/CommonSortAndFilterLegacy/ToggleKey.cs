using System;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x0200044A RID: 1098
	public struct ToggleKey : IEquatable<ToggleKey>
	{
		// Token: 0x06004047 RID: 16455 RVA: 0x001FF1CC File Offset: 0x001FD3CC
		public bool Equals(ToggleKey other)
		{
			return this.IsAll == other.IsAll && this.Index == other.Index;
		}

		// Token: 0x06004048 RID: 16456 RVA: 0x001FF200 File Offset: 0x001FD400
		public override bool Equals(object obj)
		{
			bool result;
			if (obj is ToggleKey)
			{
				ToggleKey other = (ToggleKey)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06004049 RID: 16457 RVA: 0x001FF22C File Offset: 0x001FD42C
		public override int GetHashCode()
		{
			return HashCode.Combine<bool, int>(this.IsAll, this.Index);
		}

		// Token: 0x0600404A RID: 16458 RVA: 0x001FF250 File Offset: 0x001FD450
		public static ToggleKey CreateIndexKey(int index)
		{
			return new ToggleKey
			{
				IsAll = false,
				Index = index
			};
		}

		// Token: 0x04002DE2 RID: 11746
		public bool IsAll;

		// Token: 0x04002DE3 RID: 11747
		public int Index;

		// Token: 0x04002DE4 RID: 11748
		public static readonly ToggleKey AllKey = new ToggleKey
		{
			IsAll = true,
			Index = -1
		};
	}
}
