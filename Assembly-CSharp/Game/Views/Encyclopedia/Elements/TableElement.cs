using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FrameWork.UISystem.UIElements;
using Game.Views.Encyclopedia.SyntaxTree;
using Game.Views.Encyclopedia.Utilities;
using Game.Views.Encyclopedia.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Encyclopedia.Elements
{
	// Token: 0x02000A8E RID: 2702
	public class TableElement : Element, ISearch
	{
		// Token: 0x17000E7A RID: 3706
		// (get) Token: 0x0600844B RID: 33867 RVA: 0x003D8CDA File Offset: 0x003D6EDA
		public VariableGridLayoutGroupDefaultWidth CellContainer
		{
			get
			{
				return this.cellContainer;
			}
		}

		// Token: 0x17000E7B RID: 3707
		// (get) Token: 0x0600844C RID: 33868 RVA: 0x003D8CE2 File Offset: 0x003D6EE2
		public RectTransform CellContainerRectTrans
		{
			get
			{
				return this.cellContainer.transform as RectTransform;
			}
		}

		// Token: 0x17000E7C RID: 3708
		// (get) Token: 0x0600844D RID: 33869 RVA: 0x003D8CF4 File Offset: 0x003D6EF4
		public List<CellElement> CellElementList
		{
			get
			{
				return this._cellElementList;
			}
		}

		// Token: 0x17000E7D RID: 3709
		// (get) Token: 0x0600844E RID: 33870 RVA: 0x003D8CFC File Offset: 0x003D6EFC
		public int SelfId
		{
			get
			{
				return base.NodeData.Id;
			}
		}

		// Token: 0x17000E7E RID: 3710
		// (get) Token: 0x0600844F RID: 33871 RVA: 0x003D8D09 File Offset: 0x003D6F09
		public EEncyclopediaContentLevel SelfLevel
		{
			get
			{
				TableCollectionElement parent = this.Parent;
				return (parent != null) ? parent.NodeData.Level : base.NodeData.Level;
			}
		}

		// Token: 0x06008450 RID: 33872 RVA: 0x003D8D2C File Offset: 0x003D6F2C
		private void Awake()
		{
			this.expandBtn.ClearAndAddListener(new Action(this.OnClickExpandBtn));
		}

		// Token: 0x06008451 RID: 33873 RVA: 0x003D8D48 File Offset: 0x003D6F48
		protected override void OnInit()
		{
			this.layoutGroup.padding.left = base.GetLayoutPadding();
			int refTemplateId = base.NodeData.ConfigItem.Inserts.FirstOrDefault((int t) => EncyclopediaReference.Instance[t].InsertType == EEncyclopediaReferenceInsertType.ConfigTable);
			this.InitData(EncyclopediaReference.Instance[refTemplateId], -1);
		}

		// Token: 0x06008452 RID: 33874 RVA: 0x003D8DB8 File Offset: 0x003D6FB8
		public static int CalculateTableColCount(EncyclopediaReferenceItem refConfig, string[][] table)
		{
			return (from rows in table
			select rows.Length - rows.Reverse<string>().TakeWhile(new Func<string, bool>(string.IsNullOrEmpty)).Count<string>()).Append(refConfig.Desc.Length - refConfig.Desc.Reverse<string>().TakeWhile(new Func<string, bool>(string.IsNullOrEmpty)).Count<string>()).Max();
		}

		// Token: 0x06008453 RID: 33875 RVA: 0x003D8E20 File Offset: 0x003D7020
		public static bool ParsingCfgHeader(EncyclopediaReferenceItem refConfig, out string[] header)
		{
			header = (from x in refConfig.Desc
			select x.Split(':', StringSplitOptions.None)[0].Trim()).ToArray<string>();
			return header.Any((string x) => x.Length > 0);
		}

		// Token: 0x06008454 RID: 33876 RVA: 0x003D8E8C File Offset: 0x003D708C
		public void InitData(EncyclopediaReferenceItem refConfig, int index = -1)
		{
			this.index = index;
			string tableName = refConfig.Param;
			string[][] table = EncyclopediaDataProcessor.GetTable(tableName);
			this._colCount = TableElement.CalculateTableColCount(refConfig, table);
			this._rowCount = table.Length;
			int headerCount = EncyclopediaDataProcessor.GetTableExtraHeaderCount(refConfig.LinkId) + 1;
			this.cellContainer.constraintCount = this._colCount;
			List<TableCell> tableCells = new List<TableCell>();
			string[] header;
			bool hasHeader = TableElement.ParsingCfgHeader(refConfig, out header);
			bool flag = hasHeader;
			if (flag)
			{
				TableElement.AddTableCellRow(tableCells, 0, this._colCount, this.SelfId, true, true, header);
			}
			for (int row = 1; row <= this._rowCount; row++)
			{
				TableElement.AddTableCellRow(tableCells, hasHeader ? row : (row - 1), this._colCount, this.SelfId, row < headerCount, false, table[row - 1]);
			}
			this.CreateCells(tableCells, this.SelfLevel);
			this._tableText = refConfig.Title.Split('\n', 2, StringSplitOptions.None);
			this.tableFooter.text = (this._tableText.CheckIndex(1) ? this._tableText[1] : "");
			this.tableFooter.gameObject.SetActive(!string.IsNullOrWhiteSpace(this.tableFooter.text));
			bool flag2 = this.Parent == null;
			if (flag2)
			{
				this.tableHeader.text = this._tableText[0];
				this.tableHeader.gameObject.SetActive(true);
			}
			else
			{
				this.tableHeader.gameObject.SetActive(false);
				base.NodeData.IsCollapse = false;
				this.expandBtn.gameObject.SetActive(false);
			}
			this.RefreshCollapseState();
		}

		// Token: 0x06008455 RID: 33877 RVA: 0x003D9040 File Offset: 0x003D7240
		public static void AddTableCell(List<TableCell> tableCells, int index, int row, int col, int parentId, bool isHeader, bool isColHeader, string text)
		{
			int x = 1;
			int y = 1;
			text = TableElement.TdStart.Replace(TableElement.TdEnd.Replace(text, ""), delegate(Match match)
			{
				string value = match.Value;
				foreach (string param in value.Substring(3, value.Length - 1 - 3).Split(' ', StringSplitOptions.None))
				{
					string[] parsed = param.Split('=', 2, StringSplitOptions.None);
					string text2 = parsed[0];
					string a = text2;
					if (!(a == "rowspan"))
					{
						if (a == "colspan")
						{
							bool flag = parsed.CheckIndex(1);
							if (flag)
							{
								int.TryParse(parsed[1].Replace("\"", ""), out x);
							}
						}
					}
					else
					{
						bool flag2 = parsed.CheckIndex(1);
						if (flag2)
						{
							int.TryParse(parsed[1].Replace("\"", ""), out y);
						}
					}
				}
				return "";
			});
			tableCells.Add(new TableCell
			{
				index = index,
				row = row,
				col = col,
				parentId = parentId,
				isHeader = isHeader,
				isColHeader = isColHeader,
				extraX = x - 1,
				extraY = y - 1,
				text = ((isHeader && !text.Contains("<align=")) ? ("<align=\"center\">" + text + "</align>") : text)
			});
		}

		// Token: 0x06008456 RID: 33878 RVA: 0x003D911C File Offset: 0x003D731C
		public static void AddTableCellRow(List<TableCell> tableCells, int row, int colCount, int parentId, bool isHeader, bool isColHeader, string[] texts)
		{
			int col;
			for (col = 0; col < Math.Min(texts.Length, colCount); col++)
			{
				TableElement.AddTableCell(tableCells, row * colCount + col, row, col, parentId, isHeader || col == 0, isHeader, texts[col]);
			}
			while (col < colCount)
			{
				TableElement.AddTableCell(tableCells, row * colCount + col, row, col, parentId, isHeader || col == 0, isHeader, string.Empty);
				col++;
			}
		}

		// Token: 0x06008457 RID: 33879 RVA: 0x003D9194 File Offset: 0x003D7394
		private void CreateCells(List<TableCell> tableCells, EEncyclopediaContentLevel level)
		{
			this._cellElementList.Clear();
			for (int i = 0; i < this.cellContainer.transform.childCount; i++)
			{
				this.cellContainer.transform.GetChild(i).gameObject.SetActive(false);
			}
			this.cellContainer.AddMergeOptions(from x in tableCells
			select new ValueTuple<int, int>(x.extraX, x.extraY));
			for (int j = 0; j < tableCells.Count; j++)
			{
				TableCell tableCell = tableCells[j];
				CellElement cell = (j < this.cellContainer.transform.childCount) ? this.cellContainer.transform.GetChild(j).GetComponent<CellElement>() : Object.Instantiate<CellElement>(this.cellPrefab, this.cellContainer.transform, false);
				cell.Init(tableCell, level);
				cell.gameObject.SetActive(true);
				this._cellElementList.Add(cell);
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(this.cellContainer.RectTransform);
		}

		// Token: 0x06008458 RID: 33880 RVA: 0x003D92BA File Offset: 0x003D74BA
		public void RefreshSearchResultHighlight(OptimizedHtmlPatternMatcher value, bool onlyTitle = false)
		{
			this.RefreshSearchResultHighlightImpl(value, onlyTitle, false);
		}

		// Token: 0x06008459 RID: 33881 RVA: 0x003D92C8 File Offset: 0x003D74C8
		public void RefreshSearchResultHighlightImpl(OptimizedHtmlPatternMatcher value, bool onlyTitle = false, bool forceSelecting = false)
		{
			bool flag = this._cachedSearchingValue != ((value != null) ? value.Pattern : null);
			if (flag)
			{
				this._cachedSearchingValue = ((value != null) ? value.Pattern : null);
				this._headerValue = Utility.GetHighlightText(this._tableText[0], base.NodeData.Level, value, -1, false);
				this._footerValue = (this._tableText.CheckIndex(1) ? Utility.GetHighlightText(this._tableText[1], base.NodeData.Level, value, -1, false) : string.Empty);
			}
			bool isSelecting = forceSelecting || (BasicInfoView.IsShowSearchResult && base.NodeData.Id == BasicInfoView.CurSearchResultId);
			foreach (CellElement cell in this._cellElementList)
			{
				cell.RefreshSearchResultHighlightImpl(value, false, isSelecting);
			}
			this.tableHeader.text = (isSelecting ? Utility.GetHighlightText(this._tableText[0], base.NodeData.Level, value, BasicInfoView.CurSearchResultIndex.TableHeaderIndex(this.index), true) : this._headerValue);
			bool flag2 = this._tableText.CheckIndex(1);
			if (flag2)
			{
				this.tableFooter.text = (isSelecting ? Utility.GetHighlightText(this._tableText[1], base.NodeData.Level, value, BasicInfoView.CurSearchResultIndex.TableFooterIndex(this.index), true) : this._footerValue);
			}
		}

		// Token: 0x0600845A RID: 33882 RVA: 0x003D9460 File Offset: 0x003D7660
		public void SetArrowShowOrHide(bool show)
		{
			this.arrow.localScale = new Vector3(1f, (float)(show ? -1 : 1), 1f);
			bool flag = this.Parent == null;
			if (flag)
			{
				base.NodeData.IsCollapse = !show;
			}
		}

		// Token: 0x0600845B RID: 33883 RVA: 0x003D94B0 File Offset: 0x003D76B0
		public override RectTransform GetSearchingRect(SearchIndex searchIndex)
		{
			bool flag = searchIndex != null;
			if (flag)
			{
				switch (searchIndex.ResultType)
				{
				case SearchResultType.SingleText:
					return this.tableHeader.gameObject.GetComponent<RectTransform>();
				case SearchResultType.TableCell:
				{
					bool flag2 = this.cellContainer.transform.childCount > searchIndex.Index1;
					if (flag2)
					{
						return this.cellContainer.transform.GetChild(searchIndex.Index1) as RectTransform;
					}
					break;
				}
				case SearchResultType.TableFooter:
					return this.tableFooter.gameObject.GetComponent<RectTransform>();
				}
			}
			return base.GetSearchingRect(searchIndex);
		}

		// Token: 0x0600845C RID: 33884 RVA: 0x003D955C File Offset: 0x003D775C
		private void OnClickExpandBtn()
		{
			bool flag = this.Parent == null;
			if (flag)
			{
				base.NodeData.IsCollapse = !base.NodeData.IsCollapse;
			}
			this.RefreshCollapseState();
		}

		// Token: 0x0600845D RID: 33885 RVA: 0x003D9598 File Offset: 0x003D7798
		private void RefreshCollapseState()
		{
			bool show = !base.NodeData.IsCollapse;
			this.arrow.localScale = new Vector3(1f, (float)(show ? -1 : 1), 1f);
			this.expandLayout.gameObject.SetActive(show);
		}

		// Token: 0x0400655F RID: 25951
		[SerializeField]
		private GameObject expandLayout;

		// Token: 0x04006560 RID: 25952
		[SerializeField]
		private VariableGridLayoutGroupDefaultWidth cellContainer;

		// Token: 0x04006561 RID: 25953
		[SerializeField]
		private CellElement cellPrefab;

		// Token: 0x04006562 RID: 25954
		[SerializeField]
		protected LayoutGroup layoutGroup;

		// Token: 0x04006563 RID: 25955
		[SerializeField]
		private TMP_Text tableHeader;

		// Token: 0x04006564 RID: 25956
		[SerializeField]
		private TMP_Text tableFooter;

		// Token: 0x04006565 RID: 25957
		[SerializeField]
		private RectTransform arrow;

		// Token: 0x04006566 RID: 25958
		[SerializeField]
		private CButton expandBtn;

		// Token: 0x04006567 RID: 25959
		private int _rowCount;

		// Token: 0x04006568 RID: 25960
		private int _colCount;

		// Token: 0x04006569 RID: 25961
		private string _headerValue;

		// Token: 0x0400656A RID: 25962
		private string _footerValue;

		// Token: 0x0400656B RID: 25963
		private readonly List<CellElement> _cellElementList = new List<CellElement>();

		// Token: 0x0400656C RID: 25964
		[NonSerialized]
		public TableCollectionElement Parent = null;

		// Token: 0x0400656D RID: 25965
		private string[] _tableText;

		// Token: 0x0400656E RID: 25966
		public int index = -1;

		// Token: 0x0400656F RID: 25967
		private static readonly Regex TdStart = new Regex("<td[^>]*>", RegexOptions.Compiled);

		// Token: 0x04006570 RID: 25968
		private static readonly Regex TdEnd = new Regex("</td>", RegexOptions.Compiled);
	}
}
