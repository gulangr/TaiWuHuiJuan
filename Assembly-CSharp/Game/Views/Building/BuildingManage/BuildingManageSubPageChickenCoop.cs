using System;
using System.Collections.Generic;
using System.Text;
using Config;
using Config.ConfigCells.Character;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Chicken;
using GameData.Domains.Building;
using TMPro;
using UnityEngine;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000BF7 RID: 3063
	public class BuildingManageSubPageChickenCoop : BuildingManageSubPage
	{
		// Token: 0x17001071 RID: 4209
		// (get) Token: 0x06009BB1 RID: 39857 RVA: 0x0048F582 File Offset: 0x0048D782
		// (set) Token: 0x06009BB2 RID: 39858 RVA: 0x0048F58A File Offset: 0x0048D78A
		private bool IsCardMode
		{
			get
			{
				return this._isCardMode;
			}
			set
			{
				this._isCardMode = value;
				this.OnSwitchCardMode();
			}
		}

		// Token: 0x06009BB3 RID: 39859 RVA: 0x0048F59B File Offset: 0x0048D79B
		private void Awake()
		{
			this.InitSortAndFilter();
			this.InitScroll();
		}

		// Token: 0x06009BB4 RID: 39860 RVA: 0x0048F5AC File Offset: 0x0048D7AC
		public override void Init(ViewBuildingManage parentView)
		{
			base.Init(parentView);
			this.IsCardMode = true;
		}

		// Token: 0x06009BB5 RID: 39861 RVA: 0x0048F5C0 File Offset: 0x0048D7C0
		public override void Refresh(BuildingManageDisplayData displayData)
		{
			base.Refresh(displayData);
			this._chickenDatas.Clear();
			int i = 0;
			for (;;)
			{
				int num = i;
				List<GameData.Domains.Building.Chicken> chickens = displayData.Chickens;
				int? num2 = (chickens != null) ? new int?(chickens.Count) : null;
				if (!(num < num2.GetValueOrDefault() & num2 != null))
				{
					break;
				}
				this._chickenDatas.Add(new ChickenData
				{
					Id = displayData.Chickens[i].Id,
					TemplateId = displayData.Chickens[i].TemplateId,
					NickName = displayData.ChickenNickNames[i],
					Happiness = displayData.Chickens[i].Happiness
				});
				i++;
			}
			int index = this._chickenDatas.FindIndex((ChickenData v) => v.Id == this._selectedId);
			this.chickenInfoItem.gameObject.SetActive(index >= 0);
			bool flag = index >= 0;
			if (flag)
			{
				this.chickenInfoItem.Set(this._chickenDatas[index]);
			}
			this.scrollTitle.text = string.Format("{0}/{1}", this._chickenDatas.Count, Config.Chicken.Instance.Count);
			this.RefreshList();
		}

		// Token: 0x06009BB6 RID: 39862 RVA: 0x0048F72A File Offset: 0x0048D92A
		private void InitSortAndFilter()
		{
			this._sortAndFilterController = new ChickenSortAndFilterController(this.sortAndFilter);
			this._sortAndFilterController.Init(new Action(this.RefreshList), "Chicken");
		}

		// Token: 0x06009BB7 RID: 39863 RVA: 0x0048F75C File Offset: 0x0048D95C
		private void InitScroll()
		{
			this.listScroll.OnRowClicked += this.OnClickItem;
			this.listScroll.SetRowTemplate(this.rowTemplate);
			this.listScroll.Init<ChickenData>(this.GenerateColumnDefinitions(), true, null, null);
			this.listScroll.SetSortController(this._sortAndFilterController);
			this.cardScroll.OnItemRender += this.OnCardItemRender;
			this.switchToggleGroup.Init(1);
			this.switchToggleGroup.SetWithoutNotify(1);
			this.switchToggleGroup.OnActiveIndexChange += this.OnClickSwitch;
			this.OnSwitchCardMode();
		}

		// Token: 0x06009BB8 RID: 39864 RVA: 0x0048F80C File Offset: 0x0048DA0C
		private void OnClickSwitch(int currentIndex, int previousIndex)
		{
			this.IsCardMode = (currentIndex == 1);
		}

		// Token: 0x06009BB9 RID: 39865 RVA: 0x0048F81C File Offset: 0x0048DA1C
		private void OnCardItemRender(int index, GameObject obj)
		{
			ChickenItem chickenItem = obj.GetComponent<ChickenItem>();
			chickenItem.Set(this._filteredData[index], this._selectedId == this._filteredData[index].Id);
			chickenItem.ChickenButton.ClearAndAddListener(delegate
			{
				this._selectedId = this._filteredData[index].Id;
				this.chickenInfoItem.gameObject.SetActive(true);
				this.chickenInfoItem.Set(this._filteredData[index]);
				this.cardScroll.SetDataCount(this._filteredData.Count);
			});
		}

		// Token: 0x06009BBA RID: 39866 RVA: 0x0048F893 File Offset: 0x0048DA93
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitions()
		{
			ColumnDefinition<ChickenData, ChickenData> columnDefinition = new ColumnDefinition<ChickenData, ChickenData>();
			columnDefinition.LayoutOption = new LayoutOption(400f, 100f, 100f, 1);
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Name.Tr());
			columnDefinition.CellDataGenerator = ((ChickenData data) => data);
			columnDefinition.SortId = 0;
			yield return columnDefinition;
			ColumnDefinition<ChickenData, string> columnDefinition2 = new ColumnDefinition<ChickenData, string>();
			columnDefinition2.LayoutOption = new LayoutOption(215f, 100f, 100f, 1);
			columnDefinition2.TableHeadLabel = (() => LanguageKey.LK_Favorability.Tr());
			columnDefinition2.CellDataGenerator = ((ChickenData data) => data.Happiness.ToString());
			columnDefinition2.SortId = 12;
			yield return columnDefinition2;
			ColumnDefinition<ChickenData, string> columnDefinition3 = new ColumnDefinition<ChickenData, string>();
			columnDefinition3.LayoutOption = new LayoutOption(215f, 100f, 100f, 1);
			columnDefinition3.TableHeadLabel = (() => LanguageKey.LK_Personality_Short.Tr());
			columnDefinition3.CellDataGenerator = delegate(ChickenData data)
			{
				ChickenItem chickenItem = Config.Chicken.Instance.GetItem(data.TemplateId);
				return string.Format("<SpName={0}{1}>{2}", "ui9_icon_building_personality_big_", chickenItem.PersonalityType, chickenItem.PersonalityValue);
			};
			columnDefinition3.SortId = 150;
			yield return columnDefinition3;
			ColumnDefinition<ChickenData, string> columnDefinition4 = new ColumnDefinition<ChickenData, string>();
			columnDefinition4.LayoutOption = new LayoutOption(215f, 100f, 100f, 1);
			columnDefinition4.TableHeadLabel = (() => LanguageKey.LK_Feature.Tr());
			columnDefinition4.CellDataGenerator = delegate(ChickenData data)
			{
				ChickenItem chickenItem = Config.Chicken.Instance.GetItem(data.TemplateId);
				CharacterFeatureItem featureConfig = CharacterFeature.Instance.GetItem(chickenItem.FeatureId);
				int indexMedal = 0;
				StringBuilder sb = EasyPool.Get<StringBuilder>();
				for (int i = 0; i < 3; i++)
				{
					FeatureMedals medals = featureConfig.FeatureMedals[i];
					foreach (sbyte medalType in medals.Values)
					{
						bool flag = indexMedal >= 3;
						if (flag)
						{
							break;
						}
						sb.Append("<SpName=ui9_icon_strategy_big_" + BuildingManageSubPageChickenCoop.FeatureIconConfig[(int)medalType][i] + ">");
					}
				}
				string res = sb.ToString();
				EasyPool.Free<StringBuilder>(sb);
				return res;
			};
			columnDefinition4.SortId = -1;
			yield return columnDefinition4;
			yield break;
		}

		// Token: 0x06009BBB RID: 39867 RVA: 0x0048F8A4 File Offset: 0x0048DAA4
		private void OnClickItem(int index, RowItem item)
		{
			this._selectedId = this._filteredData[index].Id;
			this.chickenInfoItem.gameObject.SetActive(true);
			this.chickenInfoItem.Set(this._filteredData[index]);
			this.listScroll.SetData<ChickenData>(this._filteredData, index);
		}

		// Token: 0x06009BBC RID: 39868 RVA: 0x0048F906 File Offset: 0x0048DB06
		private void OnSwitchCardMode()
		{
			this.RefreshCardMode();
			this.RefreshList();
		}

		// Token: 0x06009BBD RID: 39869 RVA: 0x0048F917 File Offset: 0x0048DB17
		private void RefreshCardMode()
		{
			this.cardScroll.gameObject.SetActive(this._isCardMode);
			this.listScroll.gameObject.SetActive(!this._isCardMode);
		}

		// Token: 0x06009BBE RID: 39870 RVA: 0x0048F94C File Offset: 0x0048DB4C
		private void RefreshList()
		{
			this.ApplySortAndFilter();
			bool isCardMode = this._isCardMode;
			if (isCardMode)
			{
				this.cardScroll.SetDataCount(this._filteredData.Count);
			}
			else
			{
				this.listScroll.SetData<ChickenData>(this._filteredData, this._filteredData.FindIndex((ChickenData v) => v.Id == this._selectedId));
			}
			ChickenSortAndFilterController sortAndFilterController = this._sortAndFilterController;
			if (sortAndFilterController != null)
			{
				sortAndFilterController.AfterFilter(this._chickenDatas);
			}
		}

		// Token: 0x06009BBF RID: 39871 RVA: 0x0048F9CC File Offset: 0x0048DBCC
		private void ApplySortAndFilter()
		{
			this._filteredData.Clear();
			bool flag = this._sortAndFilterController == null || this._chickenDatas == null;
			if (!flag)
			{
				Func<ChickenData, bool> filter = this._sortAndFilterController.GenerateFilter();
				foreach (ChickenData item in this._chickenDatas)
				{
					bool flag2 = filter(item);
					if (flag2)
					{
						this._filteredData.Add(item);
					}
				}
				Comparison<ChickenData> comparer = this._sortAndFilterController.GenerateComparer(this._filteredData);
				this._filteredData.Sort(comparer);
			}
		}

		// Token: 0x040078A5 RID: 30885
		[SerializeField]
		private SortAndFilter sortAndFilter;

		// Token: 0x040078A6 RID: 30886
		[SerializeField]
		private ListStyleGeneralScroll listScroll;

		// Token: 0x040078A7 RID: 30887
		[Header("行模板配置")]
		[SerializeField]
		private RowItem rowTemplate;

		// Token: 0x040078A8 RID: 30888
		[SerializeField]
		private InfinityScroll cardScroll;

		// Token: 0x040078A9 RID: 30889
		[SerializeField]
		private CToggleGroup switchToggleGroup;

		// Token: 0x040078AA RID: 30890
		[SerializeField]
		private ChickenInfoItem chickenInfoItem;

		// Token: 0x040078AB RID: 30891
		[SerializeField]
		private TextMeshProUGUI scrollTitle;

		// Token: 0x040078AC RID: 30892
		private static readonly string[][] FeatureIconConfig = new string[][]
		{
			new string[]
			{
				"0_2",
				"1_2",
				"3_2"
			},
			new string[]
			{
				"0_1",
				"1_1",
				"3_1"
			},
			new string[]
			{
				"0_0",
				"1_0",
				"3_0"
			},
			new string[]
			{
				"0_3",
				"1_3",
				"3_3"
			}
		};

		// Token: 0x040078AD RID: 30893
		private ChickenSortAndFilterController _sortAndFilterController;

		// Token: 0x040078AE RID: 30894
		private int _selectedId;

		// Token: 0x040078AF RID: 30895
		private readonly List<ChickenData> _chickenDatas = new List<ChickenData>();

		// Token: 0x040078B0 RID: 30896
		private readonly List<ChickenData> _filteredData = new List<ChickenData>();

		// Token: 0x040078B1 RID: 30897
		private bool _isCardMode;
	}
}
