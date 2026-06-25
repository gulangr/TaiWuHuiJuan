using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views
{
	// Token: 0x020006E3 RID: 1763
	public class CustomizeTableComponent : Refers
	{
		// Token: 0x17000A57 RID: 2647
		// (get) Token: 0x060053B9 RID: 21433 RVA: 0x0026CEC1 File Offset: 0x0026B0C1
		public IList<CustomizeTableSortData> CurrentSorters
		{
			get
			{
				return this._currentSorters;
			}
		}

		// Token: 0x060053BA RID: 21434 RVA: 0x0026CECC File Offset: 0x0026B0CC
		public void OnInit()
		{
			this.Init();
			this._currentSorters.Clear();
			this.UpdateHead();
			this._infinityScroll.ScrollTo(0, 0.3f);
			RectTransform scrollRectTrans = this._infinityScroll.gameObject.transform as RectTransform;
		}

		// Token: 0x060053BB RID: 21435 RVA: 0x0026CF1C File Offset: 0x0026B11C
		private void Awake()
		{
			this.Init();
		}

		// Token: 0x060053BC RID: 21436 RVA: 0x0026CF28 File Offset: 0x0026B128
		private void Init()
		{
			bool inited = this._inited;
			if (!inited)
			{
				this._inited = true;
				this._infinityScroll.OnItemRender += this.OnRenderInfo;
			}
		}

		// Token: 0x060053BD RID: 21437 RVA: 0x0026CF61 File Offset: 0x0026B161
		private void OnDestroy()
		{
			this._infinityScroll.OnItemRender -= this.OnRenderInfo;
		}

		// Token: 0x060053BE RID: 21438 RVA: 0x0026CF7C File Offset: 0x0026B17C
		public void SetMainConfig(CustomizeTableConfig mainConfig)
		{
			this._mainConfig = mainConfig;
		}

		// Token: 0x060053BF RID: 21439 RVA: 0x0026CF86 File Offset: 0x0026B186
		public void SetUpCurrentPage(CustomizeTablePageConfig pageConfig)
		{
			this._currentPageConfig = pageConfig;
			this.InitPage();
			this.InitTitleSort();
		}

		// Token: 0x060053C0 RID: 21440 RVA: 0x0026CFA0 File Offset: 0x0026B1A0
		private void InitPage()
		{
			foreach (Refers item in this._headCells)
			{
				bool flag = item == null;
				if (!flag)
				{
					Object.DestroyImmediate(item.gameObject);
				}
			}
			this._headCells.Clear();
			bool flag2 = this._headTemplateInstance == null;
			if (flag2)
			{
				this._headTemplateInstance = Object.Instantiate<Refers>(this._mainConfig.TableHeaderRowTemplate, this._titleHolder);
			}
			this._rowTemplateInstance = Object.Instantiate<Refers>(this._mainConfig.TableRowTemplate, this._candidateHolder);
			for (int i = 0; i < this._currentPageConfig.ColumnConfigs.Count; i++)
			{
				CustomizeTableColumeConfig columnConfig = this._currentPageConfig.ColumnConfigs[i];
				Refers rowCellTemplate = this.GetElementPrefab(columnConfig.ElementType);
				Refers rowCell = Object.Instantiate<Refers>(rowCellTemplate, this._rowTemplateInstance.GetComponent<CommonCustomizeTableRowComponent>().content);
				Refers headCell = Object.Instantiate<Refers>((i == 0 && this._mainConfig.TableHeadFirstTemplate != null) ? this._mainConfig.TableHeadFirstTemplate : this._mainConfig.TableHeadTemplate, this._headTemplateInstance.transform);
				Refers headCellRefers = headCell.GetComponent<Refers>();
				CButton headCellButton = headCell.GetComponent<CButton>();
				float headCellWidth = columnConfig.Width - 2f + (float)((i == this._currentPageConfig.ColumnConfigs.Count - 1 || i == 0) ? 1 : 0);
				headCellRefers.UserInt = columnConfig.ColumeId;
				bool canSort = columnConfig.CanSort;
				if (canSort)
				{
					headCellButton.ClearAndAddListener(delegate
					{
						this.OnClickSortButton(headCellButton);
					});
					headCellButton.interactable = true;
				}
				else
				{
					headCellButton.interactable = false;
				}
				headCellRefers.CGet<TextMeshProUGUI>("Label").SetText(columnConfig.ElementLocalName, true);
				headCell.GetComponent<RectTransform>().SetWidth(headCellWidth);
				rowCell.GetComponent<RectTransform>().SetWidth(columnConfig.Width);
				headCell.transform.GetChild(0).gameObject.SetActive(i < this._currentPageConfig.ColumnConfigs.Count - 1);
				headCell.gameObject.SetActive(true);
				rowCell.gameObject.SetActive(true);
				this._headCells.Add(headCellRefers);
			}
		}

		// Token: 0x060053C1 RID: 21441 RVA: 0x0026D24C File Offset: 0x0026B44C
		private Refers GetElementPrefab(int elementType)
		{
			return this._mainConfig.CellTemplates[elementType];
		}

		// Token: 0x060053C2 RID: 21442 RVA: 0x0026D26F File Offset: 0x0026B46F
		private void OnDisable()
		{
		}

		// Token: 0x060053C3 RID: 21443 RVA: 0x0026D272 File Offset: 0x0026B472
		private void InitTitleSort()
		{
		}

		// Token: 0x060053C4 RID: 21444 RVA: 0x0026D278 File Offset: 0x0026B478
		private void OnClickSortButton(CButton btn)
		{
			int infoColumnId = btn.GetComponent<Refers>().UserInt;
			for (int index = 0; index < this._currentSorters.Count; index++)
			{
				CustomizeTableSortData sorter = this._currentSorters[index];
				bool flag = sorter.ColumnId == infoColumnId;
				if (flag)
				{
					bool isDescending = sorter.IsDescending;
					if (isDescending)
					{
						sorter.IsDescending = false;
					}
					else
					{
						this._currentSorters.RemoveAt(index);
					}
					this.UpdateHead();
					this.Sort();
					this.RefreshScroll(this._currentDataAmount);
					return;
				}
			}
			this._currentSorters.Add(new CustomizeTableSortData(infoColumnId));
			this.UpdateHead();
			this.Sort();
			this.RefreshScroll(this._currentDataAmount);
		}

		// Token: 0x060053C5 RID: 21445 RVA: 0x0026D337 File Offset: 0x0026B537
		private void Sort()
		{
			Action<List<CustomizeTableSortData>> onSort = this._currentPageConfig.OnSort;
			if (onSort != null)
			{
				onSort(this._currentSorters);
			}
		}

		// Token: 0x060053C6 RID: 21446 RVA: 0x0026D358 File Offset: 0x0026B558
		private void UpdateSorter(Refers sorterRefers, int index, bool isDesc)
		{
			GameObject arrow = sorterRefers.CGet<GameObject>("Arrow");
			arrow.gameObject.SetActive(index >= 0);
			RectTransform arrowRect = arrow.GetComponent<RectTransform>();
			arrowRect.localRotation = SortFilter.GetArrowRotation(isDesc);
			GameObject numberIcon = sorterRefers.CGet<GameObject>("NumberIcon");
			numberIcon.gameObject.SetActive(true);
			TextMeshProUGUI tmp = sorterRefers.CGet<TextMeshProUGUI>("NumberLabel");
			tmp.text = (index + 1).ToString();
		}

		// Token: 0x060053C7 RID: 21447 RVA: 0x0026D3D4 File Offset: 0x0026B5D4
		private void UpdateHead()
		{
			foreach (Refers headCell in this._headCells)
			{
				headCell.CGet<GameObject>("Arrow").SetActive(false);
				headCell.CGet<GameObject>("NumberIcon").SetActive(false);
			}
			for (int index = 0; index < this._currentSorters.Count; index++)
			{
				CustomizeTableSortData sorter = this._currentSorters[index];
				int elementIndex = this.GetElementIndex(sorter.ColumnId);
				Refers headCell2 = this._headCells[elementIndex];
				GameObject arrow = headCell2.CGet<GameObject>("Arrow");
				arrow.SetActive(index >= 0);
				RectTransform arrowRect = arrow.GetComponent<RectTransform>();
				arrowRect.localRotation = SortFilter.GetArrowRotation(sorter.IsDescending);
				GameObject numberIcon = headCell2.CGet<GameObject>("NumberIcon");
				numberIcon.gameObject.SetActive(true);
				TextMeshProUGUI tmp = headCell2.CGet<TextMeshProUGUI>("NumberLabel");
				tmp.text = (index + 1).ToString();
			}
		}

		// Token: 0x060053C8 RID: 21448 RVA: 0x0026D510 File Offset: 0x0026B710
		private int GetElementIndex(int columnId)
		{
			CustomizeTablePageConfig config = this._currentPageConfig;
			bool flag = config == null;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				for (int i = 0; i < config.ColumnConfigs.Count; i++)
				{
					bool flag2 = config.ColumnConfigs[i].ColumeId == columnId;
					if (flag2)
					{
						return i;
					}
				}
				result = -1;
			}
			return result;
		}

		// Token: 0x060053C9 RID: 21449 RVA: 0x0026D570 File Offset: 0x0026B770
		public void UpdateData<T>(List<T> datas)
		{
			this.RefreshScroll(datas.Count);
		}

		// Token: 0x060053CA RID: 21450 RVA: 0x0026D580 File Offset: 0x0026B780
		private void RefreshScroll(int dataAmount)
		{
			this._currentDataAmount = dataAmount;
			this._infinityScroll.UpdateStyle(InfinityScroll.ScrollDirection.FromTop, 1, this._infinityScroll.gap, this._infinityScroll.padding, this._rowTemplateInstance.gameObject);
			this._infinityScroll.UpdateData(dataAmount);
		}

		// Token: 0x060053CB RID: 21451 RVA: 0x0026D5D1 File Offset: 0x0026B7D1
		private void OnRenderInfo(int index, GameObject refers)
		{
			this._mainConfig.OnItemRender(index, refers.GetComponent<CommonCustomizeTableRowComponent>());
		}

		// Token: 0x0400389B RID: 14491
		[SerializeField]
		private RectTransform _candidateHolder;

		// Token: 0x0400389C RID: 14492
		[SerializeField]
		private RectTransform _titleHolder;

		// Token: 0x0400389D RID: 14493
		[SerializeField]
		private InfinityScroll _infinityScroll;

		// Token: 0x0400389E RID: 14494
		private Refers _rowTemplateInstance;

		// Token: 0x0400389F RID: 14495
		private Refers _headTemplateInstance;

		// Token: 0x040038A0 RID: 14496
		private readonly List<CustomizeTableSortData> _currentSorters = new List<CustomizeTableSortData>();

		// Token: 0x040038A1 RID: 14497
		private bool _inited = false;

		// Token: 0x040038A2 RID: 14498
		private const string EmptyHeadCellImage = "ui_sp_title_5_2";

		// Token: 0x040038A3 RID: 14499
		private const int TableElementWidthOffset = 2;

		// Token: 0x040038A4 RID: 14500
		private readonly List<Refers> _headCells = new List<Refers>();

		// Token: 0x040038A5 RID: 14501
		private CustomizeTableConfig _mainConfig;

		// Token: 0x040038A6 RID: 14502
		private CustomizeTablePageConfig _currentPageConfig;

		// Token: 0x040038A7 RID: 14503
		private int _currentDataAmount;
	}
}
