using System;
using System.Collections.Generic;
using System.Linq;
using EasyButtons;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000324 RID: 804
public class CommonConfigurableParameterGrid : MonoBehaviour
{
	// Token: 0x06002EF4 RID: 12020 RVA: 0x0017166A File Offset: 0x0016F86A
	public Refers GetLineItem(int index)
	{
		return this.lineItems[index];
	}

	// Token: 0x06002EF5 RID: 12021 RVA: 0x00171678 File Offset: 0x0016F878
	public Refers GetCellItem(int index)
	{
		return this.cellItems[index];
	}

	// Token: 0x17000526 RID: 1318
	// (get) Token: 0x06002EF6 RID: 12022 RVA: 0x00171686 File Offset: 0x0016F886
	public int CellCount
	{
		get
		{
			return this.cellItems.Count;
		}
	}

	// Token: 0x06002EF7 RID: 12023 RVA: 0x00171693 File Offset: 0x0016F893
	private void Awake()
	{
	}

	// Token: 0x06002EF8 RID: 12024 RVA: 0x00171696 File Offset: 0x0016F896
	[Button("清理并生成默认布局")]
	private void ReCreate()
	{
		this.Clear();
		this.CreateDefaultLayoutData();
		this.GenerateLayoutByData();
	}

	// Token: 0x06002EF9 RID: 12025 RVA: 0x001716AE File Offset: 0x0016F8AE
	[Button("清理并 根据配置生成物体")]
	private void ReCreateByConfig()
	{
		this.Clear();
		this.GenerateLayoutByData();
	}

	// Token: 0x06002EFA RID: 12026 RVA: 0x001716C0 File Offset: 0x0016F8C0
	[Button("记录当前布局配置")]
	private void RecordConfig()
	{
		this._gridLayoutData = new ParameterGridLayoutData();
		this._gridLayoutData.LineItems = new ParameterGridLineItem[this.mainLayoutGroup.childCount];
		for (int i = 0; i < this.mainLayoutGroup.childCount; i++)
		{
			RectTransform lineItem = this.mainLayoutGroup.GetChild(i).GetComponent<RectTransform>();
			this._gridLayoutData.LineItems[i].Height = lineItem.rect.height;
			this._gridLayoutData.LineItems[i].GridItems = new ParameterGridGridItem[lineItem.childCount];
			for (int cellIndex = 0; cellIndex < lineItem.childCount; cellIndex++)
			{
				RectTransform cellItem = lineItem.GetChild(cellIndex).GetComponent<RectTransform>();
				this._gridLayoutData.LineItems[i].GridItems[cellIndex].Weight = cellItem.rect.width / lineItem.rect.width;
			}
		}
	}

	// Token: 0x06002EFB RID: 12027 RVA: 0x001717D8 File Offset: 0x0016F9D8
	[Button("清理物体")]
	private void Clear()
	{
		List<GameObject> childList = new List<GameObject>();
		for (int i = 0; i < this.mainLayoutGroup.childCount; i++)
		{
			childList.Add(this.mainLayoutGroup.GetChild(i).gameObject);
		}
		foreach (GameObject obj in childList)
		{
			Object x = obj;
			Refers refers = this.cellTemplate;
			bool flag;
			if (x != ((refers != null) ? refers.gameObject : null))
			{
				Object x2 = obj;
				Refers refers2 = this.lineItemTemplate;
				flag = (x2 != ((refers2 != null) ? refers2.gameObject : null));
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				Object.DestroyImmediate(obj);
			}
		}
	}

	// Token: 0x06002EFC RID: 12028 RVA: 0x001718A4 File Offset: 0x0016FAA4
	[Button("仅更新背景")]
	private void RefreshBack()
	{
		int actualColumnCount = this.columeCount;
		for (int i = 0; i < this.mainLayoutGroup.childCount; i++)
		{
			RectTransform lineItem = this.mainLayoutGroup.GetChild(i) as RectTransform;
			bool flag = lineItem != null;
			if (flag)
			{
				for (int cellIndex = 0; cellIndex < lineItem.childCount; cellIndex++)
				{
					CImage backImage = lineItem.GetChild(cellIndex).gameObject.GetOrAddComponent<CImage>();
					backImage.raycastTarget = this._raycastTarget;
					backImage.type = Image.Type.Sliced;
					bool isEven = i % 2 == 0;
					bool flag2 = this.backgroundMode == CommonConfigurableParameterGrid.GridBackgroundMode.AlternatingRows;
					if (flag2)
					{
						backImage.sprite = (isEven ? this.back1 : this.back2);
					}
					else
					{
						bool flag3 = this.backgroundMode == CommonConfigurableParameterGrid.GridBackgroundMode.Checkerboard;
						if (flag3)
						{
							backImage.sprite = (((cellIndex + (isEven ? 1 : 0)) % 2 == 0) ? this.back1 : this.back2);
						}
					}
				}
			}
		}
	}

	// Token: 0x06002EFD RID: 12029 RVA: 0x001719B8 File Offset: 0x0016FBB8
	private void CreateDefaultLayoutData()
	{
		this._gridLayoutData = new ParameterGridLayoutData();
		this._gridLayoutData.LineItems = new ParameterGridLineItem[this.rowCount];
		float lineHeight = this.useTemplateSize ? this.lineItemTemplate.GetComponent<RectTransform>().rect.size.y : this.defaultLineHeight;
		for (int i = 0; i < this.rowCount; i++)
		{
			this._gridLayoutData.LineItems[i].Height = lineHeight;
			this._gridLayoutData.LineItems[i].GridItems = new ParameterGridGridItem[this.columeCount];
			for (int col = 0; col < this.columeCount; col++)
			{
				this._gridLayoutData.LineItems[i].GridItems[col].Weight = 10f;
			}
		}
	}

	// Token: 0x06002EFE RID: 12030 RVA: 0x00171AA8 File Offset: 0x0016FCA8
	private void GenerateLayoutByData()
	{
		bool flag = this.cellTemplate == null;
		if (flag)
		{
			throw new Exception("template is null");
		}
		bool flag2 = this.lineItemTemplate == null;
		if (flag2)
		{
			throw new Exception("lineTemplate is null");
		}
		for (int i = 0; i < this._gridLayoutData.LineItems.Length; i++)
		{
			Refers lineRefers = this.GetRefers(this.lineItemTemplate, this.mainLayoutGroup);
			this.GenerateLine(lineRefers, this._gridLayoutData.LineItems[i]);
		}
		float lineHeight = this.defaultLineHeight;
		this.RefreshBack();
	}

	// Token: 0x06002EFF RID: 12031 RVA: 0x00171B44 File Offset: 0x0016FD44
	private void GenerateLine(Refers lineRefers, ParameterGridLineItem parameterGridLineItem)
	{
		lineRefers.GetComponent<RectTransform>().SetHeight(parameterGridLineItem.Height);
		float totalWidth = lineRefers.RectTransform.rect.width;
		float totalWeight = parameterGridLineItem.GridItems.Sum((ParameterGridGridItem t) => t.Weight);
		foreach (ParameterGridGridItem item in parameterGridLineItem.GridItems)
		{
			Refers cellRefers = this.GetRefers(this.cellTemplate, lineRefers.transform);
			cellRefers.RectTransform.SetWidth(item.Weight / totalWeight * totalWidth);
			cellRefers.RectTransform.SetHeight(parameterGridLineItem.Height);
		}
	}

	// Token: 0x06002F00 RID: 12032 RVA: 0x00171C08 File Offset: 0x0016FE08
	private Refers GetRefers(Refers template, Transform parent)
	{
		Refers refer = null;
		bool flag = null == refer;
		if (flag)
		{
			refer = Object.Instantiate<Refers>(template, parent);
			refer.gameObject.SetActive(true);
		}
		return refer;
	}

	// Token: 0x06002F01 RID: 12033 RVA: 0x00171C40 File Offset: 0x0016FE40
	public void Init()
	{
		bool inited = this._inited;
		if (!inited)
		{
			this.CollectRowAndCell(true);
			this._inited = true;
		}
	}

	// Token: 0x06002F02 RID: 12034 RVA: 0x00171C6A File Offset: 0x0016FE6A
	public void Init(ParameterGridLayoutData layoutData)
	{
		this._gridLayoutData = layoutData;
		this.ReCreateByConfig();
		this.Init();
	}

	// Token: 0x06002F03 RID: 12035 RVA: 0x00171C82 File Offset: 0x0016FE82
	public void Setup(ParameterGridLayoutData layoutData)
	{
		this._gridLayoutData = layoutData;
		this.ReCreateByConfig();
		this.CollectRowAndCell(true);
	}

	// Token: 0x06002F04 RID: 12036 RVA: 0x00171C9C File Offset: 0x0016FE9C
	private void CollectRowAndCell(bool raiseRenderEvent)
	{
		this.lineItems.Clear();
		this.cellItems.Clear();
		int currentCellIndex = 0;
		for (int i = 0; i < this.mainLayoutGroup.childCount; i++)
		{
			Refers item = this.mainLayoutGroup.GetChild(i).GetComponent<Refers>();
			RectTransform cellHolder = item.CGet<RectTransform>("Holder");
			item.UserInt = i;
			this.lineItems.Add(item);
			if (raiseRenderEvent)
			{
				Action<Refers> onLineRender = this.OnLineRender;
				if (onLineRender != null)
				{
					onLineRender(item);
				}
			}
			for (int cellIndex = 0; cellIndex < cellHolder.childCount; cellIndex++)
			{
				item = cellHolder.GetChild(cellIndex).GetComponent<Refers>();
				item.UserInt = currentCellIndex++;
				this.cellItems.Add(item);
				if (raiseRenderEvent)
				{
					Action<Refers> onCellRender = this.OnCellRender;
					if (onCellRender != null)
					{
						onCellRender(item);
					}
				}
			}
		}
	}

	// Token: 0x04002210 RID: 8720
	[SerializeField]
	private RectTransform mainLayoutGroup;

	// Token: 0x04002211 RID: 8721
	[SerializeField]
	private Refers lineItemTemplate;

	// Token: 0x04002212 RID: 8722
	[SerializeField]
	private Refers cellTemplate;

	// Token: 0x04002213 RID: 8723
	[SerializeField]
	private bool useTemplateSize;

	// Token: 0x04002214 RID: 8724
	[Tooltip("默认的行高，勾选useTemplateSize时，此配置无效")]
	[SerializeField]
	private float defaultLineHeight;

	// Token: 0x04002215 RID: 8725
	[Header("行列数")]
	[Tooltip("行数")]
	[SerializeField]
	[Range(1f, 100f)]
	private int rowCount = 5;

	// Token: 0x04002216 RID: 8726
	[Tooltip("列数")]
	[SerializeField]
	private int columeCount = 2;

	// Token: 0x04002217 RID: 8727
	[Header("背景")]
	[SerializeField]
	private Sprite back1;

	// Token: 0x04002218 RID: 8728
	[SerializeField]
	private Sprite back2;

	// Token: 0x04002219 RID: 8729
	[Header("背景模式设置")]
	[SerializeField]
	private CommonConfigurableParameterGrid.GridBackgroundMode backgroundMode = CommonConfigurableParameterGrid.GridBackgroundMode.Checkerboard;

	// Token: 0x0400221A RID: 8730
	[Header("排版数据")]
	[SerializeField]
	private ParameterGridLayoutData _gridLayoutData;

	// Token: 0x0400221B RID: 8731
	[Header("是否响应射线检测")]
	[SerializeField]
	private bool _raycastTarget;

	// Token: 0x0400221C RID: 8732
	private bool _inited = false;

	// Token: 0x0400221D RID: 8733
	private List<Refers> lineItems = new List<Refers>();

	// Token: 0x0400221E RID: 8734
	private List<Refers> cellItems = new List<Refers>();

	// Token: 0x0400221F RID: 8735
	public Action<Refers> OnLineRender;

	// Token: 0x04002220 RID: 8736
	public Action<Refers> OnCellRender;

	// Token: 0x020016A8 RID: 5800
	public enum GridBackgroundMode
	{
		// Token: 0x0400A89D RID: 43165
		Checkerboard,
		// Token: 0x0400A89E RID: 43166
		AlternatingRows
	}
}
