using System;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000454 RID: 1108
	public struct MenuOptionIndex : IEquatable<MenuOptionIndex>
	{
		// Token: 0x06004057 RID: 16471 RVA: 0x001FF3C8 File Offset: 0x001FD5C8
		public MenuOptionIndex(int menuId, int optionIndex)
		{
			this.MenuId = menuId;
			this.OptionIndex = optionIndex;
		}

		// Token: 0x06004058 RID: 16472 RVA: 0x001FF3DC File Offset: 0x001FD5DC
		public bool Equals(MenuOptionIndex other)
		{
			return this.MenuId == other.MenuId && this.OptionIndex == other.OptionIndex;
		}

		// Token: 0x06004059 RID: 16473 RVA: 0x001FF410 File Offset: 0x001FD610
		public override bool Equals(object obj)
		{
			bool result;
			if (obj is MenuOptionIndex)
			{
				MenuOptionIndex other = (MenuOptionIndex)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600405A RID: 16474 RVA: 0x001FF43C File Offset: 0x001FD63C
		public override int GetHashCode()
		{
			return HashCode.Combine<int, int>(this.MenuId, this.OptionIndex);
		}

		// Token: 0x04002DFC RID: 11772
		public int MenuId;

		// Token: 0x04002DFD RID: 11773
		public int OptionIndex;
	}
}
