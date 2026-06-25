using System;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000448 RID: 1096
	public struct ToggleIndex : IEquatable<ToggleIndex>
	{
		// Token: 0x06004042 RID: 16450 RVA: 0x001FF124 File Offset: 0x001FD324
		public ToggleIndex(int lineIndex, ToggleKey toggleKey)
		{
			this.LineIndex = lineIndex;
			this.ToggleKey = toggleKey;
		}

		// Token: 0x06004043 RID: 16451 RVA: 0x001FF138 File Offset: 0x001FD338
		public bool Equals(ToggleIndex other)
		{
			return this.LineIndex == other.LineIndex && this.ToggleKey.Equals(other.ToggleKey);
		}

		// Token: 0x06004044 RID: 16452 RVA: 0x001FF16C File Offset: 0x001FD36C
		public override bool Equals(object obj)
		{
			bool result;
			if (obj is ToggleIndex)
			{
				ToggleIndex other = (ToggleIndex)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06004045 RID: 16453 RVA: 0x001FF198 File Offset: 0x001FD398
		public override int GetHashCode()
		{
			return HashCode.Combine<int, ToggleKey>(this.LineIndex, this.ToggleKey);
		}

		// Token: 0x04002DDE RID: 11742
		public int LineIndex;

		// Token: 0x04002DDF RID: 11743
		public ToggleKey ToggleKey;
	}
}
