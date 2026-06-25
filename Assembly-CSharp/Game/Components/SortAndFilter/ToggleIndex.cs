using System;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CAB RID: 3243
	public struct ToggleIndex : IEquatable<ToggleIndex>
	{
		// Token: 0x0600A4F9 RID: 42233 RVA: 0x004CF0B4 File Offset: 0x004CD2B4
		public ToggleIndex(int lineIndex, ToggleKey toggleKey)
		{
			this.LineIndex = lineIndex;
			this.ToggleKey = toggleKey;
		}

		// Token: 0x0600A4FA RID: 42234 RVA: 0x004CF0C8 File Offset: 0x004CD2C8
		public bool Equals(ToggleIndex other)
		{
			return this.LineIndex == other.LineIndex && this.ToggleKey.Equals(other.ToggleKey);
		}

		// Token: 0x0600A4FB RID: 42235 RVA: 0x004CF0FC File Offset: 0x004CD2FC
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

		// Token: 0x0600A4FC RID: 42236 RVA: 0x004CF128 File Offset: 0x004CD328
		public override int GetHashCode()
		{
			return HashCode.Combine<int, ToggleKey>(this.LineIndex, this.ToggleKey);
		}

		// Token: 0x0400825B RID: 33371
		public int LineIndex;

		// Token: 0x0400825C RID: 33372
		public ToggleKey ToggleKey;
	}
}
