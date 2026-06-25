using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Building;
using GameData.Domains.Map;
using TMPro;
using UnityEngine;

// Token: 0x020001A7 RID: 423
public class BuildingExpandResourceTable : MonoBehaviour
{
	// Token: 0x1700029A RID: 666
	// (get) Token: 0x06001805 RID: 6149 RVA: 0x000935C6 File Offset: 0x000917C6
	private BuildingModel BuildingModel
	{
		get
		{
			return SingletonObject.getInstance<BuildingModel>();
		}
	}

	// Token: 0x1700029B RID: 667
	// (get) Token: 0x06001806 RID: 6150 RVA: 0x000935CD File Offset: 0x000917CD
	private IList<CustomizeTableSortData> _sorterInfos
	{
		get
		{
			return this.CustomizeTable.CurrentSorters;
		}
	}

	// Token: 0x06001807 RID: 6151 RVA: 0x000935DC File Offset: 0x000917DC
	private int GetColumnWidth(int scaleCount, int index)
	{
		bool flag = index > scaleCount + 1;
		int result;
		if (flag)
		{
			result = 0;
		}
		else
		{
			if (!true)
			{
			}
			int[] array;
			if (scaleCount != 1)
			{
				if (scaleCount != 2)
				{
					array = this.columnWidth3;
				}
				else
				{
					array = this.columnWidth2;
				}
			}
			else
			{
				array = this.columnWidth1;
			}
			if (!true)
			{
			}
			int[] targetArr = array;
			result = targetArr[index];
		}
		return result;
	}

	// Token: 0x06001808 RID: 6152 RVA: 0x00093630 File Offset: 0x00091830
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x06001809 RID: 6153 RVA: 0x0009363C File Offset: 0x0009183C
	private void Init()
	{
		bool inited = this._inited;
		if (!inited)
		{
			this.SetupTableMainConfig();
			this._inited = true;
		}
	}

	// Token: 0x0600180A RID: 6154 RVA: 0x00093664 File Offset: 0x00091864
	public void Setup(BuildingBlockData blockData)
	{
		this._blockData = blockData;
		this.CustomizeTable.OnInit();
	}

	// Token: 0x0600180B RID: 6155 RVA: 0x0009367C File Offset: 0x0009187C
	private void SetupTableMainConfig()
	{
		CustomizeTableConfig mainConfig = new CustomizeTableConfig();
		mainConfig.TableHeadTemplate = this.TableHeadTemplate;
		mainConfig.TableRowTemplate = this.TableRowTemplate;
		mainConfig.TableHeaderRowTemplate = this.TableHeadRowTemplate;
		mainConfig.CellTemplates = new Dictionary<int, Refers>();
		mainConfig.CellTemplates[0] = this.NumberTemplate;
		mainConfig.CellTemplates[1] = this.ButtonNumberTemplate;
		mainConfig.OnItemRender = new Action<int, CommonCustomizeTableRow>(this.OnRenderEffectInfo);
		this.CustomizeTable.SetMainConfig(mainConfig);
	}

	// Token: 0x0600180C RID: 6156 RVA: 0x00093704 File Offset: 0x00091904
	private void SetupCurrentTablePage(List<short> scaleTemplateIdList)
	{
		List<CustomizeTableColumeConfig> columnConfigs = new List<CustomizeTableColumeConfig>();
		CustomizeTablePageConfig pageConfig = new CustomizeTablePageConfig(columnConfigs, LanguageKey.Invalid, new Action<List<CustomizeTableSortData>>(this.OnSortData));
		columnConfigs.Add(new CustomizeTableColumeConfig(0, 1, LanguageKey.LK_Building_Level, (float)this.GetColumnWidth(scaleTemplateIdList.Count, 0), true));
		columnConfigs.Add(new CustomizeTableColumeConfig(1, 0, LanguageKey.LK_Building_ResourceExpand_Influence, (float)this.GetColumnWidth(scaleTemplateIdList.Count, 1), true));
		for (int i = 0; i < scaleTemplateIdList.Count; i++)
		{
			columnConfigs.Add(new CustomizeTableColumeConfig(2 + i, 0, BuildingScale.Instance[scaleTemplateIdList[i]].Name, (float)this.GetColumnWidth(scaleTemplateIdList.Count, 2 + i), true));
		}
		this.CustomizeTable.SetUpCurrentPage(pageConfig);
	}

	// Token: 0x0600180D RID: 6157 RVA: 0x000937CB File Offset: 0x000919CB
	private void OnSortData(List<CustomizeTableSortData> sorterInfos)
	{
		this._blocksInfo.Sort(new Comparison<BuildingBlockData>(this.Sorter));
	}

	// Token: 0x0600180E RID: 6158 RVA: 0x000937E8 File Offset: 0x000919E8
	private unsafe int Sorter(BuildingBlockData x, BuildingBlockData y)
	{
		foreach (CustomizeTableSortData sortConfig in this._sorterInfos)
		{
			bool desc = sortConfig.IsDescending;
			int columnId = sortConfig.ColumnId;
			int num = columnId;
			int diff;
			if (num != 0)
			{
				if (num != 1)
				{
					Span<int> span = new Span<int>(stackalloc byte[(UIntPtr)4], 1);
					Span<int> sumElements = span;
					int scaleIndex = sortConfig.ColumnId - 2;
					short scaleTemplateId = this._scaleTemplateIdList[scaleIndex];
					diff = this.GetScalePercentage(scaleTemplateId, this.GetLevel(y), this.blockDataEffectPercentageDic[y], sumElements) - this.GetScalePercentage(scaleTemplateId, this.GetLevel(x), this.blockDataEffectPercentageDic[x], sumElements);
				}
				else
				{
					diff = this.blockDataEffectPercentageDic[y] - this.blockDataEffectPercentageDic[x];
				}
			}
			else
			{
				diff = this.GetLevel(y) - this.GetLevel(x);
			}
			int res = (diff > 0) ? 1 : ((diff < 0) ? -1 : 0);
			res = (sortConfig.IsDescending ? res : (-res));
			bool flag = diff != 0;
			if (flag)
			{
				return res;
			}
		}
		return 0;
	}

	// Token: 0x0600180F RID: 6159 RVA: 0x00093938 File Offset: 0x00091B38
	public void SetupPage(List<short> scaleTemplateIdList)
	{
		this._scaleTemplateIdList = scaleTemplateIdList;
		this.SetupCurrentTablePage(scaleTemplateIdList);
	}

	// Token: 0x06001810 RID: 6160 RVA: 0x0009394C File Offset: 0x00091B4C
	public void UpdateData(List<BuildingBlockData> rawBlocksInfo)
	{
		this.blockDataEffectPercentageDic.Clear();
		for (int i = 0; i < rawBlocksInfo.Count; i++)
		{
			this.blockDataEffectPercentageDic[rawBlocksInfo[i]] = GameData.Domains.Building.SharedMethods.GetResourceBlockEffectPercentage(i);
		}
		this._rawBlocksInfo = rawBlocksInfo;
		this._blocksInfo.Clear();
		this._blocksInfo.AddRange(this._rawBlocksInfo);
		this._blocksInfo.Sort(new Comparison<BuildingBlockData>(this.Sorter));
		this.CustomizeTable.UpdateData<BuildingBlockData>(this._blocksInfo);
	}

	// Token: 0x06001811 RID: 6161 RVA: 0x000939E8 File Offset: 0x00091BE8
	private unsafe void OnRenderEffectInfo(int index, CommonCustomizeTableRow refers)
	{
		BuildingBlockData blockData = this._blocksInfo[index];
		int level = this.GetLevel(index);
		int percentage = this.blockDataEffectPercentageDic[blockData];
		for (int i = 0; i < refers.ChildCount; i++)
		{
			string bgImg = (i % 2 == 0) ? "ui_sp_base_1" : "ui_sp_base_2";
			Refers cellRefers = refers.GetChildRefers(i);
			cellRefers.CGet<CImage>("Bg").SetSprite(bgImg, false, null);
			bool flag = i == 0;
			if (flag)
			{
				cellRefers.CGet<TextMeshProUGUI>("Text").SetText(level.ToString(), true);
				CButtonObsolete jump = cellRefers.CGet<CButtonObsolete>("Button");
				GameObject invalidIcon = cellRefers.CGet<GameObject>("Invalid");
				bool jumpable = blockData.BlockIndex != this._blockData.BlockIndex;
				jump.gameObject.SetActive(jumpable);
				invalidIcon.SetActive(!jumpable);
				jump.interactable = true;
				short jumpIndex = blockData.BlockIndex;
				jump.ClearAndAddListener(delegate
				{
					jump.interactable = false;
					GEvent.OnEvent(UiEvents.NotifySwitchBuildingManage, EasyPool.Get<ArgumentBox>().Set("BuildingBlockIndex", jumpIndex));
				});
			}
			else
			{
				bool flag2 = i == 1;
				if (flag2)
				{
					TextMeshProUGUI percentageTxt = cellRefers.CGet<TextMeshProUGUI>("Text");
					string color = "brightblue";
					bool flag3 = percentage <= 0;
					if (flag3)
					{
						color = "darkred";
					}
					else
					{
						bool flag4 = percentage <= 50;
						if (flag4)
						{
							color = "brightred";
						}
					}
					percentageTxt.SetText((percentage.ToString() + "%").SetColor(color), true);
				}
				else
				{
					Span<int> span = new Span<int>(stackalloc byte[(UIntPtr)4], 1);
					Span<int> sumElements = span;
					bool flag5 = i >= this._scaleTemplateIdList.Count + 2;
					if (flag5)
					{
						break;
					}
					short scaleTemplateId = this._scaleTemplateIdList[i - 2];
					TextMeshProUGUI effectText = cellRefers.CGet<TextMeshProUGUI>("Text");
					BuildingScaleItem scaleConfig = BuildingScale.Instance[scaleTemplateId];
					int value = this.GetScalePercentage(scaleTemplateId, level, percentage, sumElements);
					effectText.text = UI_BuildingManage.GetBuildingScaleFormatString(scaleConfig.Type, value);
				}
			}
		}
	}

	// Token: 0x06001812 RID: 6162 RVA: 0x00093C20 File Offset: 0x00091E20
	private unsafe int GetScalePercentage(short scaleTemplateId, int level, int percentage, Span<int> sumElements)
	{
		BuildingScaleItem scaleConfig = BuildingScale.Instance[scaleTemplateId];
		int effectValue = GameData.Domains.Building.SharedMethods.GetResourceBlockEffectPercentageValue(level, percentage);
		*sumElements[0] = effectValue;
		return GameData.Domains.Building.SharedMethods.CalcResourceBlockTotalEffectValue(scaleConfig.Formula, sumElements);
	}

	// Token: 0x06001813 RID: 6163 RVA: 0x00093C60 File Offset: 0x00091E60
	private int GetLevel(int index)
	{
		BuildingBlockData blockData = this._blocksInfo[index];
		return this.GetLevel(blockData);
	}

	// Token: 0x06001814 RID: 6164 RVA: 0x00093C88 File Offset: 0x00091E88
	private int GetLevel(BuildingBlockData blockData)
	{
		Location taiwuVillageLocation = SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageBlock();
		BuildingBlockKey blockKey = new BuildingBlockKey(taiwuVillageLocation.AreaId, taiwuVillageLocation.BlockId, blockData.BlockIndex);
		return (int)this.BuildingModel.GetBuildingLevel(blockKey, blockData);
	}

	// Token: 0x0400134C RID: 4940
	private bool _inited;

	// Token: 0x0400134D RID: 4941
	private const int fixedColumnCount = 2;

	// Token: 0x0400134E RID: 4942
	[SerializeField]
	private CustomizeTable CustomizeTable;

	// Token: 0x0400134F RID: 4943
	[SerializeField]
	private Refers TableRowTemplate;

	// Token: 0x04001350 RID: 4944
	[SerializeField]
	private Refers TableHeadRowTemplate;

	// Token: 0x04001351 RID: 4945
	[SerializeField]
	private Refers TableHeadTemplate;

	// Token: 0x04001352 RID: 4946
	[SerializeField]
	private Refers NumberTemplate;

	// Token: 0x04001353 RID: 4947
	[SerializeField]
	private Refers ButtonNumberTemplate;

	// Token: 0x04001354 RID: 4948
	private BuildingBlockData _blockData;

	// Token: 0x04001355 RID: 4949
	private int[] columnWidth1 = new int[]
	{
		120,
		120,
		578
	};

	// Token: 0x04001356 RID: 4950
	private int[] columnWidth2 = new int[]
	{
		120,
		120,
		289,
		289
	};

	// Token: 0x04001357 RID: 4951
	private int[] columnWidth3 = new int[]
	{
		120,
		120,
		192,
		192,
		194
	};

	// Token: 0x04001358 RID: 4952
	private List<BuildingBlockData> _blocksInfo = new List<BuildingBlockData>();

	// Token: 0x04001359 RID: 4953
	private List<BuildingBlockData> _rawBlocksInfo;

	// Token: 0x0400135A RID: 4954
	private List<short> _scaleTemplateIdList;

	// Token: 0x0400135B RID: 4955
	private Dictionary<BuildingBlockData, int> blockDataEffectPercentageDic = new Dictionary<BuildingBlockData, int>();

	// Token: 0x020012F2 RID: 4850
	private enum EColumnType
	{
		// Token: 0x04009C1A RID: 39962
		Number,
		// Token: 0x04009C1B RID: 39963
		ButtonNumber
	}
}
