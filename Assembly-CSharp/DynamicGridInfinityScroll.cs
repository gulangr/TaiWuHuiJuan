using System;
using System.Collections.Generic;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x0200005D RID: 93
[RequireComponent(typeof(CScrollRectLegacy))]
public class DynamicGridInfinityScroll : MonoBehaviour
{
	// Token: 0x06000305 RID: 773 RVA: 0x000123B8 File Offset: 0x000105B8
	public void SetHandlers(Func<int, int> getCellSizeHandler, Action<int, Refers> onItemRenderHandler)
	{
		this._getCellSize = getCellSizeHandler;
		this._onItemRender = onItemRenderHandler;
	}

	// Token: 0x06000306 RID: 774 RVA: 0x000123C9 File Offset: 0x000105C9
	public void SetDataCount(int dataCount)
	{
		this.Init();
		this.DataCount = dataCount;
		this.ClearAllEmptyObjects();
		this.ClearAllCell();
		this._showingRange = new Vector2Int(-1, -1);
		this.UpdateCoordinateMap();
		this.UpdateShowingRange();
	}

	// Token: 0x06000307 RID: 775 RVA: 0x00012404 File Offset: 0x00010604
	public void RefreshCell(int cellIndex)
	{
		bool flag = !this._waitRefreshIndexList.Contains(cellIndex);
		if (flag)
		{
			this._waitRefreshIndexList.Add(cellIndex);
		}
	}

	// Token: 0x06000308 RID: 776 RVA: 0x00012434 File Offset: 0x00010634
	private void Init()
	{
		bool initFlag = this._initFlag;
		if (!initFlag)
		{
			bool flag = this.ColumnCount <= 0;
			if (flag)
			{
				AdaptableLog.Warning(base.name + " has specified wrong ColumnCount!", false);
			}
			else
			{
				this._cellCoordinateMap = new Dictionary<int, Vector2Int>();
				this._visibleCellsMap = new Dictionary<int, Refers>();
				this._waitRefreshIndexList = new List<int>();
				this._emptyStyleObjList = new List<GameObject>();
				this._poolItems = new Dictionary<int, PoolItem>();
				int i = 0;
				int max = this.SrcItems.Length;
				while (i < max)
				{
					Refers refers = this.SrcItems[i];
					bool flag2 = null == refers;
					if (flag2)
					{
						AdaptableLog.Warning(base.name + " has specified null source prefab,will course unexpected result!", false);
					}
					else
					{
						bool flag3 = this._poolItems.ContainsKey(refers.UserInt);
						if (flag3)
						{
							AdaptableLog.Warning(string.Format("{0} already has a prefab with size {1},{2} will be ignored!", base.name, refers.UserInt, refers.name), false);
						}
						else
						{
							bool flag4 = refers.UserInt > this.ColumnCount;
							if (flag4)
							{
								AdaptableLog.Warning(string.Format("{0} has a prefab with invalid size {1},{2} will be ignored!", base.name, refers.UserInt, refers.name), false);
							}
							else
							{
								PoolItem poolItem = new PoolItem(string.Format("DynamicGridInfinityScroll_{0}_PrefabPool_{1}", base.name, refers.UserInt), refers.gameObject);
								this._poolItems.Add(refers.UserInt, poolItem);
								this._unitSize = (refers.transform as RectTransform).rect.width / (float)refers.UserInt;
							}
						}
					}
					i++;
				}
				this.ScrollRect.Content.SetPivot(new Vector2(0.5f, 1f));
				this.ScrollRect.Content.anchoredPosition = Vector2.zero;
				this.ScrollRect.Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.Padding.x * 2f + (float)this.ColumnCount * this._unitSize);
				this.ScrollRect.OnScrollEvent += this.UpdateShowingRange;
				Array.ForEach<GameObject>(this.EmptyObjects, delegate(GameObject e)
				{
					e.SetActive(false);
				});
				this._emptyObjSize = new Vector2(this._unitSize, this._unitSize);
				this._initFlag = true;
			}
		}
	}

	// Token: 0x06000309 RID: 777 RVA: 0x000126C8 File Offset: 0x000108C8
	private Refers GetCellProperRefers(int cellSize)
	{
		PoolItem poolItem;
		bool flag = this._poolItems.TryGetValue(cellSize, out poolItem);
		Refers result;
		if (flag)
		{
			result = poolItem.GetObject().GetComponent<Refers>();
		}
		else
		{
			result = null;
		}
		return result;
	}

	// Token: 0x0600030A RID: 778 RVA: 0x000126FC File Offset: 0x000108FC
	private void StoreCellRefers(Refers cellRefers)
	{
		bool flag = null == cellRefers;
		if (!flag)
		{
			PoolItem poolItem;
			bool flag2 = this._poolItems.TryGetValue(cellRefers.UserInt, out poolItem);
			if (flag2)
			{
				poolItem.DestroyObject(cellRefers.gameObject);
			}
		}
	}

	// Token: 0x0600030B RID: 779 RVA: 0x0001273C File Offset: 0x0001093C
	private void SetCellPosition(int cellIndex, Refers cellRefers)
	{
		Vector2Int coordinate;
		bool flag = this._cellCoordinateMap.TryGetValue(cellIndex, out coordinate);
		if (flag)
		{
			RectTransform rectTrans = cellRefers.transform as RectTransform;
			Vector2 size = new Vector2(this._unitSize * (float)cellRefers.UserInt, this._unitSize * (float)cellRefers.UserInt);
			this.SetRectTransformPositionAndSize(rectTrans, coordinate, size);
		}
	}

	// Token: 0x0600030C RID: 780 RVA: 0x00012798 File Offset: 0x00010998
	private void SetRectTransformPositionAndSize(RectTransform rectTrans, Vector2Int coordinate, Vector2 size)
	{
		rectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, this.Padding.x + (float)coordinate.y * this._unitSize, size.x);
		rectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, this.Padding.y + (float)coordinate.x * this._unitSize, size.y);
	}

	// Token: 0x0600030D RID: 781 RVA: 0x000127F8 File Offset: 0x000109F8
	private bool IsCellVisible(RectTransform cellRectTransform)
	{
		ValueTuple<Vector2, Vector2> valueTuple = cellRectTransform.TransformRectInfo(this.ScrollRect.Viewport);
		Vector2 minPos = valueTuple.Item1;
		Vector2 maxPos = valueTuple.Item2;
		return this.ScrollRect.Viewport.rect.Contains(minPos) || this.ScrollRect.Viewport.rect.Contains(maxPos);
	}

	// Token: 0x0600030E RID: 782 RVA: 0x00012860 File Offset: 0x00010A60
	private void ClearAllEmptyObjects()
	{
		foreach (GameObject obj in this._emptyStyleObjList)
		{
			Object.Destroy(obj);
		}
		this._emptyStyleObjList.Clear();
	}

	// Token: 0x0600030F RID: 783 RVA: 0x000128C4 File Offset: 0x00010AC4
	private void ClearAllCell()
	{
		foreach (KeyValuePair<int, Refers> pair in this._visibleCellsMap)
		{
			this.StoreCellRefers(pair.Value);
		}
		this._visibleCellsMap.Clear();
	}

	// Token: 0x06000310 RID: 784 RVA: 0x00012930 File Offset: 0x00010B30
	private void SortColumns(List<DynamicGridInfinityScroll.LayoutColumn> columnData)
	{
		columnData.Sort(delegate(DynamicGridInfinityScroll.LayoutColumn left, DynamicGridInfinityScroll.LayoutColumn right)
		{
			bool flag = left.UsedLine != right.UsedLine;
			int result;
			if (flag)
			{
				result = left.UsedLine - right.UsedLine;
			}
			else
			{
				result = left.ColumnIndex - right.ColumnIndex;
			}
			return result;
		});
	}

	// Token: 0x06000311 RID: 785 RVA: 0x0001295C File Offset: 0x00010B5C
	private void PlaceEmptyObjAtColumn(DynamicGridInfinityScroll.LayoutColumn column)
	{
		GameObject emptyObj = Object.Instantiate<GameObject>(this.EmptyObjects.GetRandom<GameObject>(), this.ScrollRect.Content, false);
		Vector2Int coordinate = new Vector2Int(column.UsedLine, column.ColumnIndex);
		this._emptyStyleObjList.Add(emptyObj);
		column.AddUsedLine(1);
		this.SetRectTransformPositionAndSize(emptyObj.transform as RectTransform, coordinate, this._emptyObjSize);
		emptyObj.GetComponentInChildren<TextMeshProUGUI>().text = string.Format("Empty\n{0}", this._emptyStyleObjList.Count);
		emptyObj.SetActive(true);
	}

	// Token: 0x06000312 RID: 786 RVA: 0x000129F8 File Offset: 0x00010BF8
	private bool TryPlaceCell(int index, List<DynamicGridInfinityScroll.LayoutColumn> columnData)
	{
		int size = this._getCellSize(index);
		DynamicGridInfinityScroll.LayoutColumn minLineColumnData = columnData[0];
		for (int i = 0; i < this.ColumnCount; i++)
		{
			DynamicGridInfinityScroll.LayoutColumn column = columnData[i];
			bool flag = column.CanPlaceCell(size, minLineColumnData);
			if (flag)
			{
				this._cellCoordinateMap.Add(index, new Vector2Int(column.UsedLine, column.ColumnIndex));
				column.PlaceCell(size);
				this.SortColumns(columnData);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000313 RID: 787 RVA: 0x00012A88 File Offset: 0x00010C88
	private void FillToNeighbour(List<DynamicGridInfinityScroll.LayoutColumn> columnData)
	{
		DynamicGridInfinityScroll.LayoutColumn columnTarget = columnData[0];
		bool flag;
		do
		{
			this.PlaceEmptyObjAtColumn(columnTarget);
			flag = ((columnTarget.Left != null && columnTarget.Left.UsedLine <= columnTarget.UsedLine) || (columnTarget.Right != null && columnTarget.Right.UsedLine <= columnTarget.UsedLine));
		}
		while (!flag);
	}

	// Token: 0x06000314 RID: 788 RVA: 0x00012AF4 File Offset: 0x00010CF4
	private void UpdateCoordinateMap()
	{
		List<DynamicGridInfinityScroll.LayoutColumn> columnData = new List<DynamicGridInfinityScroll.LayoutColumn>(this.ColumnCount);
		for (int i = 0; i < this.ColumnCount; i++)
		{
			columnData.Add(new DynamicGridInfinityScroll.LayoutColumn(i));
			bool flag = i > 0;
			if (flag)
			{
				columnData[i].Left = columnData[i - 1];
			}
			bool flag2 = i < this.ColumnCount && i > 0;
			if (flag2)
			{
				columnData[i - 1].Right = columnData[i];
			}
		}
		this._cellCoordinateMap.Clear();
		List<int> tempCacheList = new List<int>();
		List<int> toRemoveFromCacheList = new List<int>();
		Action<int> <>9__2;
		for (int j = 0; j < this.DataCount; j++)
		{
			int k = 0;
			int jMax = tempCacheList.Count;
			while (k < jMax)
			{
				bool flag3 = this.TryPlaceCell(tempCacheList[k], columnData);
				if (flag3)
				{
					toRemoveFromCacheList.Add(tempCacheList[k]);
				}
				k++;
			}
			List<int> list = toRemoveFromCacheList;
			Action<int> action;
			if ((action = <>9__2) == null)
			{
				action = (<>9__2 = delegate(int e)
				{
					tempCacheList.Remove(e);
				});
			}
			list.ForEach(action);
			toRemoveFromCacheList.Clear();
			bool flag4 = !this.TryPlaceCell(j, columnData);
			if (flag4)
			{
				tempCacheList.Add(j);
			}
		}
		this.SortColumns(columnData);
		while (tempCacheList.Count > 0)
		{
			int l = 0;
			int jMax2 = tempCacheList.Count;
			while (l < jMax2)
			{
				bool flag5 = this.TryPlaceCell(tempCacheList[l], columnData);
				if (flag5)
				{
					toRemoveFromCacheList.Add(tempCacheList[l]);
				}
				l++;
			}
			toRemoveFromCacheList.ForEach(delegate(int e)
			{
				tempCacheList.Remove(e);
			});
			toRemoveFromCacheList.Clear();
			bool flag6 = tempCacheList.Count > 0;
			if (flag6)
			{
				this.FillToNeighbour(columnData);
				this.SortColumns(columnData);
			}
		}
		DynamicGridInfinityScroll.LayoutColumn columnTarget = columnData.Find((DynamicGridInfinityScroll.LayoutColumn e) => e.ColumnIndex == 0);
		while (columnTarget != null)
		{
			int offset = 0;
			bool flag7 = columnTarget.Left != null;
			if (flag7)
			{
				offset = Mathf.Max(offset, columnTarget.Left.UsedLine - columnTarget.UsedLine);
			}
			bool flag8 = columnTarget.Right != null;
			if (flag8)
			{
				offset = Mathf.Max(offset, columnTarget.Right.UsedLine - columnTarget.UsedLine);
			}
			bool flag9 = offset > DynamicGridInfinityScroll.MaxLineOffset;
			if (flag9)
			{
				this.PlaceEmptyObjAtColumn(columnTarget);
			}
			else
			{
				columnTarget = columnTarget.Right;
			}
		}
		this._lineMax = columnData[this.ColumnCount - 1].UsedLine;
		this.ScrollRect.Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.Padding.y * 2f + (float)this._lineMax * this._unitSize);
	}

	// Token: 0x06000315 RID: 789 RVA: 0x00012E2C File Offset: 0x0001102C
	private void UpdateShowingRange()
	{
		float contentPos = this.ScrollRect.Content.anchoredPosition.y;
		int lineTop = Mathf.Max(Mathf.FloorToInt((contentPos - this.Padding.y) / this._unitSize) - 1, 0);
		int lineBottom = Mathf.Min(this._lineMax, Mathf.CeilToInt((contentPos - this.Padding.y + this.ScrollRect.Viewport.rect.height) / this._unitSize) + 1);
		this._showingRange.x = lineTop;
		this._showingRange.y = lineBottom;
		foreach (KeyValuePair<int, Vector2Int> pair in this._cellCoordinateMap)
		{
			int size = this._getCellSize(pair.Key);
			Refers refers;
			bool flag = this._visibleCellsMap.TryGetValue(pair.Key, out refers);
			if (flag)
			{
				bool flag2 = pair.Value.x + size < this._showingRange.x || pair.Value.x > this._showingRange.y;
				if (flag2)
				{
					this.StoreCellRefers(refers);
					this._visibleCellsMap.Remove(pair.Key);
				}
			}
			else
			{
				bool flag3 = pair.Value.x + size >= this._showingRange.x && pair.Value.x <= this._showingRange.y;
				if (flag3)
				{
					this._waitRefreshIndexList.Add(pair.Key);
				}
			}
		}
	}

	// Token: 0x06000316 RID: 790 RVA: 0x00013018 File Offset: 0x00011218
	private void HandleRefresh()
	{
		bool flag = !this._initFlag || this._waitRefreshIndexList.Count <= 0;
		if (!flag)
		{
			int i = 0;
			int max = this._waitRefreshIndexList.Count;
			while (i < max)
			{
				int index = this._waitRefreshIndexList[i];
				Refers cellRefers;
				bool flag2 = !this._visibleCellsMap.TryGetValue(index, out cellRefers);
				if (flag2)
				{
					cellRefers = this.GetCellProperRefers(this._getCellSize(index));
					cellRefers.transform.SetParent(this.ScrollRect.Content, false);
					this._visibleCellsMap.Add(index, cellRefers);
					cellRefers.name = index.ToString();
					this.SetCellPosition(index, cellRefers);
				}
				this._onItemRender(index, cellRefers);
				i++;
			}
			this._waitRefreshIndexList.Clear();
		}
	}

	// Token: 0x06000317 RID: 791 RVA: 0x00013107 File Offset: 0x00011307
	private void OnDisable()
	{
		this.ClearAllEmptyObjects();
	}

	// Token: 0x06000318 RID: 792 RVA: 0x00013111 File Offset: 0x00011311
	private void LateUpdate()
	{
		this.HandleRefresh();
	}

	// Token: 0x040001B2 RID: 434
	[ReadOnly]
	public int DataCount;

	// Token: 0x040001B3 RID: 435
	public CScrollRectLegacy ScrollRect;

	// Token: 0x040001B4 RID: 436
	public int ColumnCount;

	// Token: 0x040001B5 RID: 437
	public static int MaxLineOffset = 3;

	// Token: 0x040001B6 RID: 438
	public Refers[] SrcItems;

	// Token: 0x040001B7 RID: 439
	[Tooltip("用于占位的 1*1 空物体，必须具有与 1*1 SrcItem相同尺寸")]
	public GameObject[] EmptyObjects;

	// Token: 0x040001B8 RID: 440
	public Vector2 Padding;

	// Token: 0x040001B9 RID: 441
	private Dictionary<int, Vector2Int> _cellCoordinateMap;

	// Token: 0x040001BA RID: 442
	private int _lineMax;

	// Token: 0x040001BB RID: 443
	private bool _initFlag;

	// Token: 0x040001BC RID: 444
	private bool _dragFlag;

	// Token: 0x040001BD RID: 445
	private Func<int, int> _getCellSize;

	// Token: 0x040001BE RID: 446
	private Action<int, Refers> _onItemRender;

	// Token: 0x040001BF RID: 447
	private Vector2Int _showingRange;

	// Token: 0x040001C0 RID: 448
	private List<int> _waitRefreshIndexList;

	// Token: 0x040001C1 RID: 449
	private Dictionary<int, PoolItem> _poolItems;

	// Token: 0x040001C2 RID: 450
	private Dictionary<int, Refers> _visibleCellsMap;

	// Token: 0x040001C3 RID: 451
	private List<GameObject> _emptyStyleObjList;

	// Token: 0x040001C4 RID: 452
	private Vector2 _emptyObjSize;

	// Token: 0x040001C5 RID: 453
	private float _unitSize;

	// Token: 0x020010CF RID: 4303
	private class LayoutColumn
	{
		// Token: 0x170015B1 RID: 5553
		// (get) Token: 0x0600C0A1 RID: 49313 RVA: 0x0056BAF3 File Offset: 0x00569CF3
		// (set) Token: 0x0600C0A2 RID: 49314 RVA: 0x0056BAFB File Offset: 0x00569CFB
		public int ColumnIndex { get; private set; }

		// Token: 0x170015B2 RID: 5554
		// (get) Token: 0x0600C0A3 RID: 49315 RVA: 0x0056BB04 File Offset: 0x00569D04
		// (set) Token: 0x0600C0A4 RID: 49316 RVA: 0x0056BB0C File Offset: 0x00569D0C
		public int UsedLine { get; private set; }

		// Token: 0x0600C0A5 RID: 49317 RVA: 0x0056BB15 File Offset: 0x00569D15
		public void AddUsedLine(int size)
		{
			this.UsedLine += size;
		}

		// Token: 0x0600C0A6 RID: 49318 RVA: 0x0056BB27 File Offset: 0x00569D27
		public LayoutColumn(int columnIndex)
		{
			this.ColumnIndex = columnIndex;
		}

		// Token: 0x0600C0A7 RID: 49319 RVA: 0x0056BB3C File Offset: 0x00569D3C
		public bool CanPlaceCell(int size, DynamicGridInfinityScroll.LayoutColumn minLineColumnData)
		{
			bool isSizeInRange = this.UsedLine + size - minLineColumnData.UsedLine <= DynamicGridInfinityScroll.MaxLineOffset;
			bool flag = !isSizeInRange;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				DynamicGridInfinityScroll.LayoutColumn columnTarget = this;
				while (columnTarget != null && size > 0)
				{
					bool flag2 = columnTarget.UsedLine != this.UsedLine;
					if (flag2)
					{
						return false;
					}
					size--;
					columnTarget = columnTarget.Right;
				}
				result = (size <= 0);
			}
			return result;
		}

		// Token: 0x0600C0A8 RID: 49320 RVA: 0x0056BBBC File Offset: 0x00569DBC
		public void PlaceCell(int size)
		{
			int lineCount = size;
			DynamicGridInfinityScroll.LayoutColumn columnTarget = this;
			while (columnTarget != null && size > 0)
			{
				columnTarget.AddUsedLine(lineCount);
				size--;
				columnTarget = columnTarget.Right;
			}
		}

		// Token: 0x04009471 RID: 38001
		public DynamicGridInfinityScroll.LayoutColumn Left;

		// Token: 0x04009472 RID: 38002
		public DynamicGridInfinityScroll.LayoutColumn Right;
	}
}
