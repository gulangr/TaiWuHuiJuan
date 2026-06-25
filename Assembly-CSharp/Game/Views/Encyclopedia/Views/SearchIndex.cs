using System;
using Game.Views.Encyclopedia.SyntaxTree;

namespace Game.Views.Encyclopedia.Views
{
	// Token: 0x02000A6D RID: 2669
	public class SearchIndex
	{
		// Token: 0x06008386 RID: 33670 RVA: 0x003D3A62 File Offset: 0x003D1C62
		public SearchIndex(int index)
		{
			this.Index0 = index;
		}

		// Token: 0x06008387 RID: 33671 RVA: 0x003D3A81 File Offset: 0x003D1C81
		public SearchIndex(int index, int cellIndex)
		{
			this.Index0 = index;
			this.Index1 = cellIndex;
		}

		// Token: 0x06008388 RID: 33672 RVA: 0x003D3AA7 File Offset: 0x003D1CA7
		public SearchIndex(int index, int cellIndex, int tableIndex)
		{
			this.Index0 = index;
			this.Index1 = cellIndex;
			this.Index2 = tableIndex;
		}

		// Token: 0x06008389 RID: 33673 RVA: 0x003D3AD4 File Offset: 0x003D1CD4
		public bool Equals(SearchIndex other)
		{
			return other.Index0 == this.Index0 && other.Index1 == this.Index1 && other.Index2 == this.Index2;
		}

		// Token: 0x17000E6E RID: 3694
		// (get) Token: 0x0600838A RID: 33674 RVA: 0x003D3B03 File Offset: 0x003D1D03
		public int SingleTextIndex
		{
			get
			{
				return (this.Index1 == -1 && this.Index2 == -1) ? this.Index0 : -1;
			}
		}

		// Token: 0x0600838B RID: 33675 RVA: 0x003D3B20 File Offset: 0x003D1D20
		public int CellIndex(TableCell cellData)
		{
			return (this.Index1 == cellData.index) ? this.Index0 : -1;
		}

		// Token: 0x0600838C RID: 33676 RVA: 0x003D3B39 File Offset: 0x003D1D39
		public int TableHeaderIndex(int index = -1)
		{
			return (this.Index2 == index && this.Index1 == -1) ? this.Index0 : -1;
		}

		// Token: 0x0600838D RID: 33677 RVA: 0x003D3B56 File Offset: 0x003D1D56
		public int TableCollectionHeaderIndex(int index = -1)
		{
			return (this.Index1 == index && this.Index2 == -1) ? this.Index0 : -1;
		}

		// Token: 0x0600838E RID: 33678 RVA: 0x003D3B73 File Offset: 0x003D1D73
		public int TableFooterIndex(int index = -1)
		{
			return (this.Index2 == index && this.Index1 == -2) ? this.Index0 : -1;
		}

		// Token: 0x17000E6F RID: 3695
		// (get) Token: 0x0600838F RID: 33679 RVA: 0x003D3B94 File Offset: 0x003D1D94
		public SearchResultType ResultType
		{
			get
			{
				SearchResultType result;
				if (this.Index2 == this.Index1 && this.Index2 == -1)
				{
					result = SearchResultType.SingleText;
				}
				else
				{
					int index = this.Index1;
					if (!true)
					{
					}
					SearchResultType searchResultType;
					if (index != -2)
					{
						if (index != -1)
						{
							searchResultType = SearchResultType.TableCell;
						}
						else
						{
							searchResultType = SearchResultType.TableTitleInCollection;
						}
					}
					else
					{
						searchResultType = SearchResultType.TableFooter;
					}
					if (!true)
					{
					}
					result = searchResultType;
				}
				return result;
			}
		}

		// Token: 0x06008390 RID: 33680 RVA: 0x003D3BE7 File Offset: 0x003D1DE7
		public override string ToString()
		{
			return string.Format("{0}, {1}, {2}", this.Index0, this.Index1, this.Index2);
		}

		// Token: 0x040064B0 RID: 25776
		public readonly int Index0;

		// Token: 0x040064B1 RID: 25777
		public readonly int Index1 = -1;

		// Token: 0x040064B2 RID: 25778
		public readonly int Index2 = -1;

		// Token: 0x040064B3 RID: 25779
		public const int DefaultIndex = -1;

		// Token: 0x040064B4 RID: 25780
		public const int TableTitleIndex = -1;

		// Token: 0x040064B5 RID: 25781
		public const int TableCollectionTitleIndex = -1;

		// Token: 0x040064B6 RID: 25782
		public const int TableNoteIndex = -2;

		// Token: 0x040064B7 RID: 25783
		public const int TableCollectionSelfIndex = -1;
	}
}
