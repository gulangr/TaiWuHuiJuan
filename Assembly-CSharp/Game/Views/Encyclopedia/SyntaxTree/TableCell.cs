using System;

namespace Game.Views.Encyclopedia.SyntaxTree
{
	// Token: 0x02000A79 RID: 2681
	public struct TableCell : IEquatable<TableCell>
	{
		// Token: 0x17000E72 RID: 3698
		// (get) Token: 0x060083D1 RID: 33745 RVA: 0x003D50D6 File Offset: 0x003D32D6
		public bool Invisible
		{
			get
			{
				return this.extraX < 0 || this.extraY < 0;
			}
		}

		// Token: 0x060083D2 RID: 33746 RVA: 0x003D50F0 File Offset: 0x003D32F0
		public bool Equals(TableCell other)
		{
			return this.index == other.index && this.row == other.row && this.col == other.col && this.parentId == other.parentId && this.isHeader == other.isHeader && this.isColHeader == other.isColHeader && this.text.Equals(other.text);
		}

		// Token: 0x040064F7 RID: 25847
		public int index;

		// Token: 0x040064F8 RID: 25848
		public int row;

		// Token: 0x040064F9 RID: 25849
		public int col;

		// Token: 0x040064FA RID: 25850
		public int parentId;

		// Token: 0x040064FB RID: 25851
		public bool isHeader;

		// Token: 0x040064FC RID: 25852
		public bool isColHeader;

		// Token: 0x040064FD RID: 25853
		public string text;

		// Token: 0x040064FE RID: 25854
		public int extraX;

		// Token: 0x040064FF RID: 25855
		public int extraY;
	}
}
