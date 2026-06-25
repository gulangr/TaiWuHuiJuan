using System;

namespace UnityEngine.UI
{
	// Token: 0x02000FAE RID: 4014
	[AddComponentMenu("Layout/Variable Grid Layout Group", 152)]
	public class VariableGridLayoutGroup : LayoutGroup
	{
		// Token: 0x170014E2 RID: 5346
		// (get) Token: 0x0600B897 RID: 47255 RVA: 0x005426FE File Offset: 0x005408FE
		public RectTransform RectTransform
		{
			get
			{
				return base.transform as RectTransform;
			}
		}

		// Token: 0x170014E3 RID: 5347
		// (get) Token: 0x0600B898 RID: 47256 RVA: 0x0054270C File Offset: 0x0054090C
		// (set) Token: 0x0600B899 RID: 47257 RVA: 0x00542724 File Offset: 0x00540924
		public VariableGridLayoutGroup.Corner startCorner
		{
			get
			{
				return this.m_StartCorner;
			}
			set
			{
				base.SetProperty<VariableGridLayoutGroup.Corner>(ref this.m_StartCorner, value);
			}
		}

		// Token: 0x170014E4 RID: 5348
		// (get) Token: 0x0600B89A RID: 47258 RVA: 0x00542738 File Offset: 0x00540938
		// (set) Token: 0x0600B89B RID: 47259 RVA: 0x00542750 File Offset: 0x00540950
		public VariableGridLayoutGroup.Axis startAxis
		{
			get
			{
				return this.m_StartAxis;
			}
			set
			{
				base.SetProperty<VariableGridLayoutGroup.Axis>(ref this.m_StartAxis, value);
			}
		}

		// Token: 0x170014E5 RID: 5349
		// (get) Token: 0x0600B89C RID: 47260 RVA: 0x00542764 File Offset: 0x00540964
		// (set) Token: 0x0600B89D RID: 47261 RVA: 0x0054277C File Offset: 0x0054097C
		public TextAnchor cellAlignment
		{
			get
			{
				return this.m_CellAlignment;
			}
			set
			{
				base.SetProperty<TextAnchor>(ref this.m_CellAlignment, value);
			}
		}

		// Token: 0x170014E6 RID: 5350
		// (get) Token: 0x0600B89E RID: 47262 RVA: 0x00542790 File Offset: 0x00540990
		// (set) Token: 0x0600B89F RID: 47263 RVA: 0x005427A8 File Offset: 0x005409A8
		public Vector2 spacing
		{
			get
			{
				return this.m_Spacing;
			}
			set
			{
				base.SetProperty<Vector2>(ref this.m_Spacing, value);
			}
		}

		// Token: 0x170014E7 RID: 5351
		// (get) Token: 0x0600B8A0 RID: 47264 RVA: 0x005427BC File Offset: 0x005409BC
		// (set) Token: 0x0600B8A1 RID: 47265 RVA: 0x005427D4 File Offset: 0x005409D4
		public VariableGridLayoutGroup.Constraint constraint
		{
			get
			{
				return this.m_Constraint;
			}
			set
			{
				base.SetProperty<VariableGridLayoutGroup.Constraint>(ref this.m_Constraint, value);
			}
		}

		// Token: 0x170014E8 RID: 5352
		// (get) Token: 0x0600B8A2 RID: 47266 RVA: 0x005427E8 File Offset: 0x005409E8
		// (set) Token: 0x0600B8A3 RID: 47267 RVA: 0x00542800 File Offset: 0x00540A00
		public int constraintCount
		{
			get
			{
				return this.m_ConstraintCount;
			}
			set
			{
				base.SetProperty<int>(ref this.m_ConstraintCount, Mathf.Max(1, value));
			}
		}

		// Token: 0x170014E9 RID: 5353
		// (get) Token: 0x0600B8A4 RID: 47268 RVA: 0x00542818 File Offset: 0x00540A18
		// (set) Token: 0x0600B8A5 RID: 47269 RVA: 0x00542830 File Offset: 0x00540A30
		public bool childForceExpandWidth
		{
			get
			{
				return this.m_ChildForceExpandWidth;
			}
			set
			{
				base.SetProperty<bool>(ref this.m_ChildForceExpandWidth, value);
			}
		}

		// Token: 0x170014EA RID: 5354
		// (get) Token: 0x0600B8A6 RID: 47270 RVA: 0x00542844 File Offset: 0x00540A44
		// (set) Token: 0x0600B8A7 RID: 47271 RVA: 0x0054285C File Offset: 0x00540A5C
		public bool childForceExpandHeight
		{
			get
			{
				return this.m_ChildForceExpandHeight;
			}
			set
			{
				base.SetProperty<bool>(ref this.m_ChildForceExpandHeight, value);
			}
		}

		// Token: 0x0600B8A8 RID: 47272 RVA: 0x00542870 File Offset: 0x00540A70
		protected VariableGridLayoutGroup()
		{
		}

		// Token: 0x170014EB RID: 5355
		// (get) Token: 0x0600B8A9 RID: 47273 RVA: 0x005428C1 File Offset: 0x00540AC1
		// (set) Token: 0x0600B8AA RID: 47274 RVA: 0x005428C9 File Offset: 0x00540AC9
		public int columns { get; private set; }

		// Token: 0x170014EC RID: 5356
		// (get) Token: 0x0600B8AB RID: 47275 RVA: 0x005428D2 File Offset: 0x00540AD2
		// (set) Token: 0x0600B8AC RID: 47276 RVA: 0x005428DA File Offset: 0x00540ADA
		public int rows { get; private set; }

		// Token: 0x0600B8AD RID: 47277 RVA: 0x005428E4 File Offset: 0x00540AE4
		public int GetCellIndexAtGridRef(int column, int row)
		{
			bool flag = column >= 0 && column < this.columns && row >= 0 && row < this.rows;
			int result;
			if (flag)
			{
				result = this.cellIndexAtGridRef[column, row];
			}
			else
			{
				result = -1;
			}
			return result;
		}

		// Token: 0x0600B8AE RID: 47278 RVA: 0x00542928 File Offset: 0x00540B28
		public int GetCellColumn(int cellIndex)
		{
			bool flag = cellIndex >= 0 && cellIndex < base.rectChildren.Count;
			int result;
			if (flag)
			{
				result = this.cellColumn[cellIndex];
			}
			else
			{
				result = -1;
			}
			return result;
		}

		// Token: 0x0600B8AF RID: 47279 RVA: 0x00542960 File Offset: 0x00540B60
		public int GetCellRow(int cellIndex)
		{
			bool flag = cellIndex >= 0 && cellIndex < base.rectChildren.Count;
			int result;
			if (flag)
			{
				result = this.cellRow[cellIndex];
			}
			else
			{
				result = -1;
			}
			return result;
		}

		// Token: 0x0600B8B0 RID: 47280 RVA: 0x00542998 File Offset: 0x00540B98
		public float GetColumnPositionWithinGrid(int column)
		{
			bool flag = column <= 0 || column >= this.columns;
			float result;
			if (flag)
			{
				result = 0f;
			}
			else
			{
				float pos = 0f;
				for (int c = 0; c < column; c++)
				{
					pos += this.GetColumnWidth(c) + this.spacing.x;
				}
				result = pos;
			}
			return result;
		}

		// Token: 0x0600B8B1 RID: 47281 RVA: 0x005429FC File Offset: 0x00540BFC
		public float GetRowPositionWithinGrid(int row)
		{
			bool flag = row <= 0 || row >= this.rows;
			float result;
			if (flag)
			{
				result = 0f;
			}
			else
			{
				float pos = 0f;
				for (int r = 0; r < row; r++)
				{
					pos += this.GetRowHeight(r) + this.spacing.y;
				}
				result = pos;
			}
			return result;
		}

		// Token: 0x0600B8B2 RID: 47282 RVA: 0x00542A60 File Offset: 0x00540C60
		public float GetColumnWidth(int column)
		{
			bool flag = column < 0 || column >= this.columns;
			float result;
			if (flag)
			{
				result = 0f;
			}
			else
			{
				result = this.columnWidths[column];
			}
			return result;
		}

		// Token: 0x0600B8B3 RID: 47283 RVA: 0x00542A9C File Offset: 0x00540C9C
		public float GetRowHeight(int row)
		{
			bool flag = row < 0 || row >= this.rows;
			float result;
			if (flag)
			{
				result = 0f;
			}
			else
			{
				result = this.rowHeights[row];
			}
			return result;
		}

		// Token: 0x0600B8B4 RID: 47284 RVA: 0x00542AD5 File Offset: 0x00540CD5
		public virtual float GetCellColumnWidth(int column, int index)
		{
			return this.GetColumnWidth(column);
		}

		// Token: 0x0600B8B5 RID: 47285 RVA: 0x00542ADE File Offset: 0x00540CDE
		public virtual float GetCellRowHeight(int row, int index)
		{
			return this.GetRowHeight(row);
		}

		// Token: 0x0600B8B6 RID: 47286 RVA: 0x00542AE7 File Offset: 0x00540CE7
		protected virtual float GetCellPreferredWidth(int index)
		{
			return LayoutUtility.GetPreferredWidth(base.rectChildren[index]);
		}

		// Token: 0x0600B8B7 RID: 47287 RVA: 0x00542AFA File Offset: 0x00540CFA
		protected virtual float GetCellPreferredHeight(int index)
		{
			return LayoutUtility.GetPreferredHeight(base.rectChildren[index]);
		}

		// Token: 0x0600B8B8 RID: 47288 RVA: 0x00542B10 File Offset: 0x00540D10
		private void InitializeLayout()
		{
			this.columns = ((this.constraint == VariableGridLayoutGroup.Constraint.FixedColumnCount) ? Mathf.Min(this.constraintCount, base.rectChildren.Count) : Mathf.CeilToInt((float)base.rectChildren.Count / (float)this.constraintCount));
			this.rows = ((this.constraint == VariableGridLayoutGroup.Constraint.FixedRowCount) ? Mathf.Min(this.constraintCount, base.rectChildren.Count) : Mathf.CeilToInt((float)base.rectChildren.Count / (float)this.constraintCount));
			this.cellIndexAtGridRef = new int[this.columns, this.rows];
			this.cellColumn = new int[base.rectChildren.Count];
			this.cellRow = new int[base.rectChildren.Count];
			this.cellPreferredSizes = new Vector2[base.rectChildren.Count];
			this.columnWidths = new float[this.columns];
			this.rowHeights = new float[this.rows];
			this.totalColumnWidth = 0f;
			this.totalRowHeight = 0f;
			for (int a = 0; a < this.columns; a++)
			{
				for (int b = 0; b < this.rows; b++)
				{
					this.cellIndexAtGridRef[a, b] = -1;
				}
			}
			int cOrigin = 0;
			int rOrigin = 0;
			int cNext = 1;
			int rNext = 1;
			bool flag = this.startCorner == VariableGridLayoutGroup.Corner.UpperRight || this.startCorner == VariableGridLayoutGroup.Corner.LowerRight;
			if (flag)
			{
				cOrigin = this.columns - 1;
				cNext = -1;
			}
			bool flag2 = this.startCorner == VariableGridLayoutGroup.Corner.LowerLeft || this.startCorner == VariableGridLayoutGroup.Corner.LowerRight;
			if (flag2)
			{
				rOrigin = this.rows - 1;
				rNext = -1;
			}
			int c = cOrigin;
			int r = rOrigin;
			for (int cell = 0; cell < base.rectChildren.Count; cell++)
			{
				this.cellIndexAtGridRef[c, r] = cell;
				this.cellColumn[cell] = c;
				this.cellRow[cell] = r;
				this.cellPreferredSizes[cell] = new Vector2(this.GetCellPreferredWidth(cell), this.GetCellPreferredHeight(cell));
				this.columnWidths[c] = Mathf.Max(this.columnWidths[c], this.cellPreferredSizes[cell].x);
				this.rowHeights[r] = Mathf.Max(this.rowHeights[r], this.cellPreferredSizes[cell].y);
				bool flag3 = this.startAxis == VariableGridLayoutGroup.Axis.Horizontal;
				if (flag3)
				{
					c += cNext;
					bool flag4 = c < 0 || c >= this.columns;
					if (flag4)
					{
						c = cOrigin;
						r += rNext;
					}
				}
				else
				{
					r += rNext;
					bool flag5 = r < 0 || r >= this.rows;
					if (flag5)
					{
						r = rOrigin;
						c += cNext;
					}
				}
			}
			this.PostProcessColumnWidths();
			for (int col = 0; col < this.columns; col++)
			{
				this.totalColumnWidth += this.columnWidths[col];
			}
			for (int row = 0; row < this.rows; row++)
			{
				this.totalRowHeight += this.rowHeights[row];
			}
		}

		// Token: 0x0600B8B9 RID: 47289 RVA: 0x00542E75 File Offset: 0x00541075
		protected virtual void PostProcessColumnWidths()
		{
		}

		// Token: 0x0600B8BA RID: 47290 RVA: 0x00542E78 File Offset: 0x00541078
		public override void CalculateLayoutInputHorizontal()
		{
			base.CalculateLayoutInputHorizontal();
			this.InitializeLayout();
			float totalMinWidth = (float)base.padding.horizontal;
			float totalPreferredWidth = (float)base.padding.horizontal + this.totalColumnWidth + this.spacing.x * (float)(this.columns - 1);
			base.SetLayoutInputForAxis(totalMinWidth, totalPreferredWidth, -1f, 0);
			float extraWidth = LayoutUtility.GetPreferredWidth(base.rectTransform) - totalPreferredWidth;
			bool flag = extraWidth > 0f && this.childForceExpandWidth;
			if (flag)
			{
				bool[] expandColumn = new bool[this.columns];
				int columnsToExpand = 0;
				for (int c = 0; c < this.columns; c++)
				{
					expandColumn[c] = false;
					for (int r = 0; r < this.rows; r++)
					{
						int index = this.GetCellIndexAtGridRef(c, r);
						bool flag2 = index < base.rectChildren.Count;
						if (flag2)
						{
							RectTransform child = base.rectChildren[index];
							VariableGridCell cellOptions = child.GetComponent<VariableGridCell>();
							bool flag3 = cellOptions == null || !cellOptions.overrideForceExpandWidth || cellOptions.forceExpandWidth;
							if (flag3)
							{
								expandColumn[c] = true;
								columnsToExpand++;
								break;
							}
						}
					}
				}
				for (int c2 = 0; c2 < this.columns; c2++)
				{
					bool flag4 = expandColumn[c2];
					if (flag4)
					{
						this.columnWidths[c2] += extraWidth / (float)columnsToExpand;
					}
				}
			}
		}

		// Token: 0x0600B8BB RID: 47291 RVA: 0x00543008 File Offset: 0x00541208
		public override void CalculateLayoutInputVertical()
		{
			float totalMinHeight = (float)base.padding.vertical;
			float totalPreferredHeight = (float)base.padding.vertical + this.totalRowHeight + this.spacing.y * (float)(this.rows - 1);
			base.SetLayoutInputForAxis(totalMinHeight, totalPreferredHeight, -1f, 1);
			float extraHeight = LayoutUtility.GetPreferredHeight(base.rectTransform) - totalPreferredHeight;
			bool flag = extraHeight > 0f && this.childForceExpandHeight;
			if (flag)
			{
				bool[] expandRow = new bool[this.rows];
				int rowsToExpand = 0;
				for (int r = 0; r < this.rows; r++)
				{
					expandRow[r] = false;
					for (int c = 0; c < this.columns; c++)
					{
						int index = this.GetCellIndexAtGridRef(c, r);
						bool flag2 = index >= 0 && index < base.rectChildren.Count;
						if (!flag2)
						{
							expandRow[r] = true;
							rowsToExpand++;
							break;
						}
						RectTransform child = base.rectChildren[index];
						VariableGridCell cellOptions = child.GetComponent<VariableGridCell>();
						bool flag3 = cellOptions == null || !cellOptions.overrideForceExpandHeight || cellOptions.forceExpandHeight;
						if (flag3)
						{
							expandRow[r] = true;
							rowsToExpand++;
							break;
						}
					}
				}
				for (int r2 = 0; r2 < this.rows; r2++)
				{
					bool flag4 = expandRow[r2];
					if (flag4)
					{
						this.rowHeights[r2] += extraHeight / (float)rowsToExpand;
					}
				}
			}
		}

		// Token: 0x0600B8BC RID: 47292 RVA: 0x005431A6 File Offset: 0x005413A6
		public override void SetLayoutHorizontal()
		{
			this.SetCellsAlongAxis(0);
		}

		// Token: 0x0600B8BD RID: 47293 RVA: 0x005431B1 File Offset: 0x005413B1
		public override void SetLayoutVertical()
		{
			this.SetCellsAlongAxis(1);
		}

		// Token: 0x0600B8BE RID: 47294 RVA: 0x005431BC File Offset: 0x005413BC
		private void SetCellsAlongAxis(int axis)
		{
			float space = (axis == 0) ? base.rectTransform.rect.width : base.rectTransform.rect.height;
			float extraSpace = space - LayoutUtility.GetPreferredSize(base.rectTransform, axis);
			float gridOrigin = (float)((axis == 0) ? base.padding.left : base.padding.top);
			bool flag = axis == 0;
			if (flag)
			{
				bool flag2 = base.childAlignment == TextAnchor.UpperCenter || base.childAlignment == TextAnchor.MiddleCenter || base.childAlignment == TextAnchor.LowerCenter;
				if (flag2)
				{
					gridOrigin += extraSpace / 2f;
				}
				else
				{
					bool flag3 = base.childAlignment == TextAnchor.UpperRight || base.childAlignment == TextAnchor.MiddleRight || base.childAlignment == TextAnchor.LowerRight;
					if (flag3)
					{
						gridOrigin += extraSpace;
					}
				}
			}
			else
			{
				bool flag4 = base.childAlignment == TextAnchor.MiddleLeft || base.childAlignment == TextAnchor.MiddleCenter || base.childAlignment == TextAnchor.MiddleRight;
				if (flag4)
				{
					gridOrigin += extraSpace / 2f;
				}
				else
				{
					bool flag5 = base.childAlignment == TextAnchor.LowerLeft || base.childAlignment == TextAnchor.LowerCenter || base.childAlignment == TextAnchor.LowerRight;
					if (flag5)
					{
						gridOrigin += extraSpace;
					}
				}
			}
			bool forceExpand = (axis == 0) ? this.childForceExpandWidth : this.childForceExpandHeight;
			int alignment = 0;
			bool flag6 = axis == 0;
			if (flag6)
			{
				bool flag7 = this.cellAlignment == TextAnchor.UpperLeft || this.cellAlignment == TextAnchor.MiddleLeft || this.cellAlignment == TextAnchor.LowerLeft;
				if (flag7)
				{
					alignment = -1;
				}
				bool flag8 = this.cellAlignment == TextAnchor.UpperRight || this.cellAlignment == TextAnchor.MiddleRight || this.cellAlignment == TextAnchor.LowerRight;
				if (flag8)
				{
					alignment = 1;
				}
			}
			else
			{
				bool flag9 = this.cellAlignment == TextAnchor.UpperLeft || this.cellAlignment == TextAnchor.UpperCenter || this.cellAlignment == TextAnchor.UpperRight;
				if (flag9)
				{
					alignment = -1;
				}
				bool flag10 = this.cellAlignment == TextAnchor.LowerLeft || this.cellAlignment == TextAnchor.LowerCenter || this.cellAlignment == TextAnchor.LowerRight;
				if (flag10)
				{
					alignment = 1;
				}
			}
			for (int i = 0; i < base.rectChildren.Count; i++)
			{
				this.SetCellSize(i, axis, gridOrigin, forceExpand, alignment);
			}
		}

		// Token: 0x0600B8BF RID: 47295 RVA: 0x005433DC File Offset: 0x005415DC
		protected virtual void SetCellSize(int i, int axis, float gridOrigin, bool forceExpand, int alignment)
		{
			int colrow = (axis == 0) ? this.GetCellColumn(i) : this.GetCellRow(i);
			float cellOrigin = gridOrigin + ((axis == 0) ? this.GetColumnPositionWithinGrid(colrow) : this.GetRowPositionWithinGrid(colrow));
			float cellSpace = (axis == 0) ? this.GetCellColumnWidth(colrow, i) : this.GetCellRowHeight(colrow, i);
			RectTransform child = base.rectChildren[i];
			float cellSize = LayoutUtility.GetPreferredSize(child, axis);
			float cellExtraSpace = cellSpace - cellSize;
			bool cellForceExpand = forceExpand;
			int thisCellAlignment = alignment;
			VariableGridCell cellOptions = child.GetComponent<VariableGridCell>();
			bool flag = cellOptions != null;
			if (flag)
			{
				bool flag2 = (axis == 0) ? cellOptions.overrideForceExpandWidth : cellOptions.overrideForceExpandHeight;
				if (flag2)
				{
					cellForceExpand = ((axis == 0) ? cellOptions.forceExpandWidth : cellOptions.forceExpandHeight);
				}
				bool overrideCellAlignment = cellOptions.overrideCellAlignment;
				if (overrideCellAlignment)
				{
					bool flag3 = axis == 0;
					if (flag3)
					{
						bool flag4 = cellOptions.cellAlignment == TextAnchor.UpperLeft || cellOptions.cellAlignment == TextAnchor.MiddleLeft || cellOptions.cellAlignment == TextAnchor.LowerLeft;
						if (flag4)
						{
							thisCellAlignment = -1;
						}
						else
						{
							bool flag5 = cellOptions.cellAlignment == TextAnchor.UpperCenter || cellOptions.cellAlignment == TextAnchor.MiddleCenter || cellOptions.cellAlignment == TextAnchor.LowerCenter;
							if (flag5)
							{
								thisCellAlignment = 0;
							}
							else
							{
								thisCellAlignment = 1;
							}
						}
					}
					else
					{
						bool flag6 = cellOptions.cellAlignment == TextAnchor.UpperLeft || cellOptions.cellAlignment == TextAnchor.UpperCenter || cellOptions.cellAlignment == TextAnchor.UpperRight;
						if (flag6)
						{
							thisCellAlignment = -1;
						}
						else
						{
							bool flag7 = cellOptions.cellAlignment == TextAnchor.MiddleLeft || cellOptions.cellAlignment == TextAnchor.MiddleCenter || cellOptions.cellAlignment == TextAnchor.MiddleRight;
							if (flag7)
							{
								thisCellAlignment = 0;
							}
							else
							{
								thisCellAlignment = 1;
							}
						}
					}
				}
			}
			bool flag8 = cellForceExpand;
			if (flag8)
			{
				cellSize = cellSpace;
			}
			else
			{
				bool flag9 = thisCellAlignment == 0;
				if (flag9)
				{
					cellOrigin += cellExtraSpace / 2f;
				}
				bool flag10 = thisCellAlignment == 1;
				if (flag10)
				{
					cellOrigin += cellExtraSpace;
				}
			}
			base.SetChildAlongAxis(base.rectChildren[i], axis, cellOrigin, cellSize);
		}

		// Token: 0x04008F29 RID: 36649
		[SerializeField]
		protected VariableGridLayoutGroup.Corner m_StartCorner = VariableGridLayoutGroup.Corner.UpperLeft;

		// Token: 0x04008F2A RID: 36650
		[SerializeField]
		protected VariableGridLayoutGroup.Axis m_StartAxis = VariableGridLayoutGroup.Axis.Horizontal;

		// Token: 0x04008F2B RID: 36651
		[SerializeField]
		protected TextAnchor m_CellAlignment = TextAnchor.UpperLeft;

		// Token: 0x04008F2C RID: 36652
		[SerializeField]
		protected Vector2 m_Spacing = Vector2.zero;

		// Token: 0x04008F2D RID: 36653
		[SerializeField]
		protected VariableGridLayoutGroup.Constraint m_Constraint = VariableGridLayoutGroup.Constraint.FixedColumnCount;

		// Token: 0x04008F2E RID: 36654
		[SerializeField]
		protected int m_ConstraintCount = 3;

		// Token: 0x04008F2F RID: 36655
		[SerializeField]
		protected bool m_ChildForceExpandWidth = true;

		// Token: 0x04008F30 RID: 36656
		[SerializeField]
		protected bool m_ChildForceExpandHeight = true;

		// Token: 0x04008F33 RID: 36659
		private int[,] cellIndexAtGridRef;

		// Token: 0x04008F34 RID: 36660
		private int[] cellColumn;

		// Token: 0x04008F35 RID: 36661
		private int[] cellRow;

		// Token: 0x04008F36 RID: 36662
		private Vector2[] cellPreferredSizes;

		// Token: 0x04008F37 RID: 36663
		protected float[] columnWidths;

		// Token: 0x04008F38 RID: 36664
		protected float[] rowHeights;

		// Token: 0x04008F39 RID: 36665
		private float totalColumnWidth;

		// Token: 0x04008F3A RID: 36666
		private float totalRowHeight;

		// Token: 0x020025EA RID: 9706
		public enum Corner
		{
			// Token: 0x0400E992 RID: 59794
			UpperLeft,
			// Token: 0x0400E993 RID: 59795
			UpperRight,
			// Token: 0x0400E994 RID: 59796
			LowerLeft,
			// Token: 0x0400E995 RID: 59797
			LowerRight
		}

		// Token: 0x020025EB RID: 9707
		public enum Axis
		{
			// Token: 0x0400E997 RID: 59799
			Horizontal,
			// Token: 0x0400E998 RID: 59800
			Vertical
		}

		// Token: 0x020025EC RID: 9708
		public enum Constraint
		{
			// Token: 0x0400E99A RID: 59802
			FixedColumnCount,
			// Token: 0x0400E99B RID: 59803
			FixedRowCount,
			// Token: 0x0400E99C RID: 59804
			AutoColumnCount
		}
	}
}
