using System;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CB7 RID: 3255
	public struct MenuOptionIndex : IEquatable<MenuOptionIndex>
	{
		// Token: 0x0600A50E RID: 42254 RVA: 0x004CF344 File Offset: 0x004CD544
		public MenuOptionIndex(int menuId, int optionIndex)
		{
			this.MenuId = menuId;
			this.OptionIndex = optionIndex;
		}

		// Token: 0x0600A50F RID: 42255 RVA: 0x004CF358 File Offset: 0x004CD558
		public bool Equals(MenuOptionIndex other)
		{
			return this.MenuId == other.MenuId && this.OptionIndex == other.OptionIndex;
		}

		// Token: 0x0600A510 RID: 42256 RVA: 0x004CF38C File Offset: 0x004CD58C
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

		// Token: 0x0600A511 RID: 42257 RVA: 0x004CF3B8 File Offset: 0x004CD5B8
		public override int GetHashCode()
		{
			return HashCode.Combine<int, int>(this.MenuId, this.OptionIndex);
		}

		// Token: 0x04008279 RID: 33401
		public int MenuId;

		// Token: 0x0400827A RID: 33402
		public int OptionIndex;
	}
}
