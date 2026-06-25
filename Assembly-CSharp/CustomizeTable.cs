using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x020001FC RID: 508
[Obsolete]
public class CustomizeTable : Refers
{
	// Token: 0x17000350 RID: 848
	// (get) Token: 0x060020E5 RID: 8421 RVA: 0x000EFEB9 File Offset: 0x000EE0B9
	public IList<CustomizeTableSortData> CurrentSorters
	{
		get
		{
			return this._currentSorters;
		}
	}

	// Token: 0x060020E6 RID: 8422 RVA: 0x000EFEC4 File Offset: 0x000EE0C4
	public void OnInit()
	{
		this.Init();
		this._currentSorters.Clear();
		this.UpdateHead();
		this._infinityScroll.ScrollTo(0, 0.3f);
		this._infinityScroll.OnItemRender = new Action<int, Refers>(this.OnRenderInfo);
		RectTransform scrollRectTrans = this._infinityScroll.gameObject.transform as RectTransform;
	}

	// Token: 0x060020E7 RID: 8423 RVA: 0x000EFF2B File Offset: 0x000EE12B
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x060020E8 RID: 8424 RVA: 0x000EFF38 File Offset: 0x000EE138
	private void Init()
	{
		bool inited = this._inited;
		if (!inited)
		{
			this._inited = true;
		}
	}

	// Token: 0x060020E9 RID: 8425 RVA: 0x000EFF59 File Offset: 0x000EE159
	public void SetMainConfig(CustomizeTableConfig mainConfig)
	{
		this._mainConfig = mainConfig;
	}

	// Token: 0x060020EA RID: 8426 RVA: 0x000EFF63 File Offset: 0x000EE163
	public void SetUpCurrentPage(CustomizeTablePageConfig pageConfig)
	{
		this._currentPageConfig = pageConfig;
		this.InitPage();
		this.InitTitleSort();
	}

	// Token: 0x060020EB RID: 8427 RVA: 0x000EFF7C File Offset: 0x000EE17C
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
			Refers rowCell = Object.Instantiate<Refers>(rowCellTemplate, this._rowTemplateInstance.GetComponent<CommonCustomizeTableRow>().content);
			Refers headCell = Object.Instantiate<Refers>((i == 0 && this._mainConfig.TableHeadFirstTemplate != null) ? this._mainConfig.TableHeadFirstTemplate : this._mainConfig.TableHeadTemplate, this._headTemplateInstance.transform);
			Refers headCellRefers = headCell.GetComponent<Refers>();
			CButtonObsolete headCellButton = headCell.GetComponent<CButtonObsolete>();
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
			this._headCells.Add(headCellRefers);
		}
	}

	// Token: 0x060020EC RID: 8428 RVA: 0x000F01DC File Offset: 0x000EE3DC
	private Refers GetElementPrefab(int elementType)
	{
		return this._mainConfig.CellTemplates[elementType];
	}

	// Token: 0x060020ED RID: 8429 RVA: 0x000F01FF File Offset: 0x000EE3FF
	private void OnDisable()
	{
	}

	// Token: 0x060020EE RID: 8430 RVA: 0x000F0202 File Offset: 0x000EE402
	private void InitTitleSort()
	{
	}

	// Token: 0x060020EF RID: 8431 RVA: 0x000F0208 File Offset: 0x000EE408
	private void OnClickSortButton(CButtonObsolete btn)
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

	// Token: 0x060020F0 RID: 8432 RVA: 0x000F02C7 File Offset: 0x000EE4C7
	private void Sort()
	{
		Action<List<CustomizeTableSortData>> onSort = this._currentPageConfig.OnSort;
		if (onSort != null)
		{
			onSort(this._currentSorters);
		}
	}

	// Token: 0x060020F1 RID: 8433 RVA: 0x000F02E8 File Offset: 0x000EE4E8
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

	// Token: 0x060020F2 RID: 8434 RVA: 0x000F0364 File Offset: 0x000EE564
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

	// Token: 0x060020F3 RID: 8435 RVA: 0x000F04A0 File Offset: 0x000EE6A0
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

	// Token: 0x060020F4 RID: 8436 RVA: 0x000F0500 File Offset: 0x000EE700
	public void UpdateData<T>(List<T> datas)
	{
		this.RefreshScroll(datas.Count);
	}

	// Token: 0x060020F5 RID: 8437 RVA: 0x000F0510 File Offset: 0x000EE710
	private void RefreshScroll(int dataAmount)
	{
		this._currentDataAmount = dataAmount;
		this._infinityScroll.UpdateStyle(InfinityScrollLegacy.ScrollDirection.FromTop, 1, this._infinityScroll.Gap, this._infinityScroll.Padding, this._rowTemplateInstance);
		this._infinityScroll.UpdateData(dataAmount);
	}

	// Token: 0x060020F6 RID: 8438 RVA: 0x000F055C File Offset: 0x000EE75C
	private void OnRenderInfo(int index, Refers refers)
	{
		this._mainConfig.OnItemRender(index, refers.GetComponent<CommonCustomizeTableRow>());
	}

	// Token: 0x04001940 RID: 6464
	[SerializeField]
	private RectTransform _candidateHolder;

	// Token: 0x04001941 RID: 6465
	[SerializeField]
	private RectTransform _titleHolder;

	// Token: 0x04001942 RID: 6466
	[SerializeField]
	private InfinityScrollLegacy _infinityScroll;

	// Token: 0x04001943 RID: 6467
	private Refers _rowTemplateInstance;

	// Token: 0x04001944 RID: 6468
	private Refers _headTemplateInstance;

	// Token: 0x04001945 RID: 6469
	private readonly List<CustomizeTableSortData> _currentSorters = new List<CustomizeTableSortData>();

	// Token: 0x04001946 RID: 6470
	private bool _inited = false;

	// Token: 0x04001947 RID: 6471
	private const string EmptyHeadCellImage = "ui_sp_title_5_2";

	// Token: 0x04001948 RID: 6472
	private const int TableElementWidthOffset = 2;

	// Token: 0x04001949 RID: 6473
	public GameObject tablePageToggleTemplate;

	// Token: 0x0400194A RID: 6474
	private readonly List<Refers> _headCells = new List<Refers>();

	// Token: 0x0400194B RID: 6475
	private CustomizeTableConfig _mainConfig;

	// Token: 0x0400194C RID: 6476
	private CustomizeTablePageConfig _currentPageConfig;

	// Token: 0x0400194D RID: 6477
	private int _currentDataAmount;
}
