using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Building;
using GameData.Domains.Map;
using TMPro;
using UnityEngine;

namespace Game.Views.Building
{
	// Token: 0x02000BD4 RID: 3028
	public class BuildingExpandResourceTableComponent : MonoBehaviour
	{
		// Token: 0x17001058 RID: 4184
		// (get) Token: 0x0600987A RID: 39034 RVA: 0x0047070E File Offset: 0x0046E90E
		private BuildingModel BuildingModel
		{
			get
			{
				return SingletonObject.getInstance<BuildingModel>();
			}
		}

		// Token: 0x17001059 RID: 4185
		// (get) Token: 0x0600987B RID: 39035 RVA: 0x00470715 File Offset: 0x0046E915
		private IList<CustomizeTableSortData> _sorterInfos
		{
			get
			{
				return this.CustomizeTable.CurrentSorters;
			}
		}

		// Token: 0x0600987C RID: 39036 RVA: 0x00470724 File Offset: 0x0046E924
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

		// Token: 0x0600987D RID: 39037 RVA: 0x00470778 File Offset: 0x0046E978
		private void Awake()
		{
			this.Init();
		}

		// Token: 0x0600987E RID: 39038 RVA: 0x00470784 File Offset: 0x0046E984
		public void Init()
		{
			bool inited = this._inited;
			if (!inited)
			{
				this.SetupTableMainConfig();
				this._inited = true;
			}
		}

		// Token: 0x0600987F RID: 39039 RVA: 0x004707AC File Offset: 0x0046E9AC
		public void Setup(BuildingBlockData blockData)
		{
			this._blockData = blockData;
			this.CustomizeTable.OnInit();
		}

		// Token: 0x06009880 RID: 39040 RVA: 0x004707C4 File Offset: 0x0046E9C4
		private void SetupTableMainConfig()
		{
			CustomizeTableConfig mainConfig = new CustomizeTableConfig();
			mainConfig.TableHeadTemplate = this.TableHeadTemplate;
			mainConfig.TableRowTemplate = this.TableRowTemplate;
			mainConfig.TableHeaderRowTemplate = this.TableHeadRowTemplate;
			mainConfig.CellTemplates = new Dictionary<int, Refers>();
			mainConfig.CellTemplates[0] = this.NumberTemplate;
			mainConfig.CellTemplates[1] = this.ButtonNumberTemplate;
			mainConfig.OnItemRender = new Action<int, CommonCustomizeTableRowComponent>(this.OnRenderEffectInfo);
			this.CustomizeTable.SetMainConfig(mainConfig);
		}

		// Token: 0x06009881 RID: 39041 RVA: 0x0047084C File Offset: 0x0046EA4C
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

		// Token: 0x06009882 RID: 39042 RVA: 0x00470913 File Offset: 0x0046EB13
		private void OnSortData(List<CustomizeTableSortData> sorterInfos)
		{
			this._blocksInfo.Sort(new Comparison<BuildingBlockData>(this.Sorter));
		}

		// Token: 0x06009883 RID: 39043 RVA: 0x00470930 File Offset: 0x0046EB30
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

		// Token: 0x06009884 RID: 39044 RVA: 0x00470A80 File Offset: 0x0046EC80
		public void SetupPage(List<short> scaleTemplateIdList)
		{
			this._scaleTemplateIdList = scaleTemplateIdList;
			this.SetupCurrentTablePage(scaleTemplateIdList);
		}

		// Token: 0x06009885 RID: 39045 RVA: 0x00470A94 File Offset: 0x0046EC94
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

		// Token: 0x06009886 RID: 39046 RVA: 0x00470B30 File Offset: 0x0046ED30
		private unsafe void OnRenderEffectInfo(int index, CommonCustomizeTableRowComponent refers)
		{
			BuildingBlockData blockData = this._blocksInfo[index];
			int level = this.GetLevel(index);
			int percentage = this.blockDataEffectPercentageDic[blockData];
			for (int i = 0; i < refers.ChildCount; i++)
			{
				Refers cellRefers = refers.GetChildRefers(i);
				bool flag = i == refers.ChildCount - 1;
				if (flag)
				{
					cellRefers.CGet<GameObject>("Spliter").SetActive(false);
				}
				bool flag2 = i == 0;
				if (flag2)
				{
					cellRefers.CGet<TextMeshProUGUI>("Text").SetText(level.ToString(), true);
					CButton jump = cellRefers.CGet<CButton>("Button");
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
					bool flag3 = i == 1;
					if (flag3)
					{
						TextMeshProUGUI percentageTxt = cellRefers.CGet<TextMeshProUGUI>("Text");
						string color = "brightblue";
						bool flag4 = percentage <= 0;
						if (flag4)
						{
							color = "darkred";
						}
						else
						{
							bool flag5 = percentage <= 50;
							if (flag5)
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
						bool flag6 = i >= this._scaleTemplateIdList.Count + 2;
						if (flag6)
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

		// Token: 0x06009887 RID: 39047 RVA: 0x00470D64 File Offset: 0x0046EF64
		private unsafe int GetScalePercentage(short scaleTemplateId, int level, int percentage, Span<int> sumElements)
		{
			BuildingScaleItem scaleConfig = BuildingScale.Instance[scaleTemplateId];
			int effectValue = GameData.Domains.Building.SharedMethods.GetResourceBlockEffectPercentageValue(level, percentage);
			*sumElements[0] = effectValue;
			return GameData.Domains.Building.SharedMethods.CalcResourceBlockTotalEffectValue(scaleConfig.Formula, sumElements);
		}

		// Token: 0x06009888 RID: 39048 RVA: 0x00470DA4 File Offset: 0x0046EFA4
		private int GetLevel(int index)
		{
			BuildingBlockData blockData = this._blocksInfo[index];
			return this.GetLevel(blockData);
		}

		// Token: 0x06009889 RID: 39049 RVA: 0x00470DCC File Offset: 0x0046EFCC
		private int GetLevel(BuildingBlockData blockData)
		{
			Location taiwuVillageLocation = SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageBlock();
			BuildingBlockKey blockKey = new BuildingBlockKey(taiwuVillageLocation.AreaId, taiwuVillageLocation.BlockId, blockData.BlockIndex);
			return (int)this.BuildingModel.GetBuildingLevel(blockKey, blockData);
		}

		// Token: 0x04007548 RID: 30024
		private bool _inited;

		// Token: 0x04007549 RID: 30025
		private const int fixedColumnCount = 2;

		// Token: 0x0400754A RID: 30026
		[SerializeField]
		private CustomizeTableComponent CustomizeTable;

		// Token: 0x0400754B RID: 30027
		[SerializeField]
		private Refers TableRowTemplate;

		// Token: 0x0400754C RID: 30028
		[SerializeField]
		private Refers TableHeadRowTemplate;

		// Token: 0x0400754D RID: 30029
		[SerializeField]
		private Refers TableHeadTemplate;

		// Token: 0x0400754E RID: 30030
		[SerializeField]
		private Refers NumberTemplate;

		// Token: 0x0400754F RID: 30031
		[SerializeField]
		private Refers ButtonNumberTemplate;

		// Token: 0x04007550 RID: 30032
		private BuildingBlockData _blockData;

		// Token: 0x04007551 RID: 30033
		private int[] columnWidth1 = new int[]
		{
			120,
			120,
			578
		};

		// Token: 0x04007552 RID: 30034
		private int[] columnWidth2 = new int[]
		{
			120,
			120,
			289,
			289
		};

		// Token: 0x04007553 RID: 30035
		private int[] columnWidth3 = new int[]
		{
			173,
			160,
			173,
			170,
			140
		};

		// Token: 0x04007554 RID: 30036
		private List<BuildingBlockData> _blocksInfo = new List<BuildingBlockData>();

		// Token: 0x04007555 RID: 30037
		private List<BuildingBlockData> _rawBlocksInfo;

		// Token: 0x04007556 RID: 30038
		private List<short> _scaleTemplateIdList;

		// Token: 0x04007557 RID: 30039
		private Dictionary<BuildingBlockData, int> blockDataEffectPercentageDic = new Dictionary<BuildingBlockData, int>();

		// Token: 0x020022A0 RID: 8864
		private enum EColumnType
		{
			// Token: 0x0400DB9A RID: 56218
			Number,
			// Token: 0x0400DB9B RID: 56219
			ButtonNumber
		}
	}
}
