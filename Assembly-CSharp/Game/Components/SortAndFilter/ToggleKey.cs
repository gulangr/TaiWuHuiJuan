using System;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CAD RID: 3245
	public struct ToggleKey : IEquatable<ToggleKey>
	{
		// Token: 0x0600A4FE RID: 42238 RVA: 0x004CF15C File Offset: 0x004CD35C
		public bool Equals(ToggleKey other)
		{
			return this.IsAll == other.IsAll && this.Index == other.Index;
		}

		// Token: 0x0600A4FF RID: 42239 RVA: 0x004CF190 File Offset: 0x004CD390
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

		// Token: 0x0600A500 RID: 42240 RVA: 0x004CF1BC File Offset: 0x004CD3BC
		public override int GetHashCode()
		{
			return HashCode.Combine<bool, int>(this.IsAll, this.Index);
		}

		// Token: 0x0600A501 RID: 42241 RVA: 0x004CF1E0 File Offset: 0x004CD3E0
		public static ToggleKey CreateIndexKey(int index)
		{
			return new ToggleKey
			{
				IsAll = false,
				Index = index
			};
		}

		// Token: 0x0400825F RID: 33375
		public bool IsAll;

		// Token: 0x04008260 RID: 33376
		public int Index;

		// Token: 0x04008261 RID: 33377
		public static readonly ToggleKey AllKey = new ToggleKey
		{
			IsAll = true,
			Index = -1
		};
	}
}
