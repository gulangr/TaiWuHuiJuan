using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.CellContent;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.SelectCharacter;
using Game.Views.Select;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Building
{
	// Token: 0x02000BEA RID: 3050
	public class ViewRecruitPeopleOverview : UIBase, ITableHead
	{
		// Token: 0x17001063 RID: 4195
		// (get) Token: 0x06009A91 RID: 39569 RVA: 0x00486150 File Offset: 0x00484350
		private int _cachedDataCount
		{
			get
			{
				return (from t in this._cachedData
				where t.CharacterDataList != null
				select t).Sum((BuildingRecruitData t) => t.CharacterDataList.Count);
			}
		}

		// Token: 0x06009A92 RID: 39570 RVA: 0x004861AC File Offset: 0x004843AC
		private void Awake()
		{
			this.btnClose.ClearAndAddListener(new Action(this.QuickHide));
			this.btnSelectAll.ClearAndAddListener(new Action(this.OnClickSelectAll));
			this.switchToggle.Init(0);
			this.switchToggle.SetWithoutNotify(0);
			this.switchToggle.OnActiveIndexChange += this.SwitchDisplayMode;
			this.btnRecuit.ClearAndAddListener(delegate
			{
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
				{
					Title = LocalStringManager.Get(LanguageKey.LK_Building_RecruitPeople_Dialog_Title).ColorReplace(),
					Content = LocalStringManager.Get(LanguageKey.LK_Building_RecruitPeople_Dialog_Text) + "\n" + LocalStringManager.GetFormat(LanguageKey.LK_Building_RecruitPeople_Dialog_NeedAuthority, this._selectedCharacterIds.Count((BuildingRecruitCharacterData t) => this._buildingBlockKeyTemplateIdDic[t.BuildingBlockKey] == 223) * (int)GlobalConfig.Instance.RecruitPeopleCost).ColorReplace(),
					Type = 1,
					Yes = delegate()
					{
						this.OnRecurit();
					}
				}));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			});
			this.btnRefuse.ClearAndAddListener(delegate
			{
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
				{
					Title = LocalStringManager.Get(LanguageKey.LK_Building_RejectRecruitPeople_Cmd_Title).ColorReplace(),
					Content = LocalStringManager.Get(LanguageKey.LK_Building_RejectRecruitPeople_Cmd_Text).ColorReplace(),
					Type = 1,
					Yes = delegate()
					{
						this.OnRefuse();
					}
				}));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			});
			this.InitSortAndFilter();
			this.RefreshListStructure();
			this.InitSubPageToggles();
		}

		// Token: 0x06009A93 RID: 39571 RVA: 0x00486264 File Offset: 0x00484464
		private void OnClickSelectAll()
		{
			int maxAmount = this._cachedDataCount;
			bool isCurrentSelectAll = maxAmount <= this._selectedCharacterIds.Count;
			this._selectedCharacterIds.Clear();
			this._buildingTypeSelections.Clear();
			bool flag = isCurrentSelectAll;
			if (flag)
			{
				this.SetTotalAmount(0, maxAmount);
			}
			else
			{
				foreach (BuildingRecruitData item in this._cachedData)
				{
					foreach (BuildingRecruitCharacterData data in item.CharacterDataList)
					{
						this._selectedCharacterIds.Add(data);
					}
					this._buildingTypeSelections[item.BuildingTemplateId] = item.CharacterDataList.Count;
				}
				this.SetTotalAmount(maxAmount, maxAmount);
			}
			this._flatModeDirty = true;
			this._listModeDirty = true;
			this.RefreshDisplay();
			this.UpdateButtonState();
		}

		// Token: 0x06009A94 RID: 39572 RVA: 0x00486394 File Offset: 0x00484594
		private void SwitchDisplayMode(int currIndex, int prevIndex)
		{
			this._currentInListMode = (currIndex == 0);
			this.listModeArea.SetActive(this._currentInListMode);
			this.flatModeArea.SetActive(!this._currentInListMode);
			bool currentInListMode = this._currentInListMode;
			if (currentInListMode)
			{
				this.RefreshListMode();
			}
			else
			{
				this.RefreshFlatMode();
			}
		}

		// Token: 0x06009A95 RID: 39573 RVA: 0x004863F4 File Offset: 0x004845F4
		private void OnListItemRender(int index, GameObject rowObject)
		{
			RecruitRowContentSwitcher comp = rowObject.GetComponent<RecruitRowContentSwitcher>();
			bool flag = this._titleRowBuildingDic.ContainsKey(index);
			if (flag)
			{
				short buildingTempalteId = this._titleRowBuildingDic[index];
				int maxAmount = 0;
				foreach (BuildingRecruitData item in this._cachedData)
				{
					bool flag2 = item.BuildingTemplateId == buildingTempalteId;
					if (flag2)
					{
						maxAmount = item.CharacterDataList.Count;
						break;
					}
				}
				comp.SetRowContentActive(false);
				comp.txtTitle.text = BuildingBlock.Instance[buildingTempalteId].Name;
				int currentSelected = this._buildingTypeSelections.ContainsKey(buildingTempalteId) ? this._buildingTypeSelections[buildingTempalteId] : 0;
				comp.txtAmount.text = string.Format("({0}/{1})", currentSelected, maxAmount);
			}
			else
			{
				comp.SetRowContentActive(true);
				RowItem rowItem = rowObject.GetComponent<RowItem>();
				rowItem.Init(this._columnDefinitions, true);
				BuildingRecruitCharacterData rowData = this._filteredDataList[index];
				comp.MouseTip.Type = TipType.CharacterComplete;
				comp.MouseTip.enabled = false;
				short buildingTemplateId = -1;
				int currentIndex = -1;
				foreach (KeyValuePair<int, short> item2 in this._titleRowBuildingDic)
				{
					bool flag3 = item2.Key < index && item2.Key > currentIndex;
					if (flag3)
					{
						currentIndex = item2.Key;
						buildingTemplateId = item2.Value;
					}
				}
				ExtraDomainMethod.AsyncCall.RequestRecruitCharacterData(this, rowData.BuildingBlockKey, rowData.RecruitInfoIndex, delegate(int offset2, RawDataPool pool2)
				{
					RecruitCharacterData recruitCharacterData = null;
					Serializer.Deserialize(pool2, offset2, ref recruitCharacterData);
					bool flag4 = recruitCharacterData != null && comp != null;
					if (flag4)
					{
						comp.MouseTip.enabled = true;
						comp.MouseTip.RuntimeParam = new ArgumentBox();
						int remainTime = GameData.Domains.Building.SharedMethods.GetBuildingEarnPreserveTime(buildingTemplateId) - rowData.RecruitLevel.Second;
						comp.MouseTip.RuntimeParam.Set("RemainTime", remainTime);
						comp.MouseTip.RuntimeParam.SetObject("Data", new CharacterDisplayDataForTooltip(recruitCharacterData));
					}
				});
				rowItem.SetData(rowData, index < this._filteredDataList.Count - 1, null);
				rowItem.SetRowInteraction(true, index, new Action<int, RowItem>(this.OnRowClicked));
				rowItem.SetInteractable(true, true);
				rowItem.SetDisabled(false);
				bool isSelected = this._selectedCharacterIds.Contains(rowData);
				rowItem.SetSelected(isSelected);
			}
		}

		// Token: 0x06009A96 RID: 39574 RVA: 0x00486680 File Offset: 0x00484880
		private void OnRefuse()
		{
			int countMax = this._selectedCharacterIds.Count;
			List<BuildingRecruitCharacterData> sortedDesc = this._selectedCharacterIds.ToList<BuildingRecruitCharacterData>();
			sortedDesc.Sort((BuildingRecruitCharacterData a, BuildingRecruitCharacterData b) => b.RecruitInfoIndex - a.RecruitInfoIndex);
			foreach (BuildingRecruitCharacterData item in sortedDesc)
			{
				BuildingDomainMethod.Call.RejectBuildingBlockRecruitPeople(item.BuildingBlockKey, item.RecruitInfoIndex);
				UIElement.BuildingArea.UiBaseAs<ViewBuildingArea>().UpdateShopGetItemInfo(item.BuildingBlockKey);
			}
			bool flag = countMax >= this._cachedDataCount;
			if (flag)
			{
				this.QuickHide();
			}
			else
			{
				this.CallRefresh();
			}
			this._selectedCharacterIds.Clear();
			this._buildingTypeSelections.Clear();
		}

		// Token: 0x06009A97 RID: 39575 RVA: 0x00486770 File Offset: 0x00484970
		private void OnRecurit()
		{
			ViewRecruitPeopleOverview.<>c__DisplayClass51_0 CS$<>8__locals1 = new ViewRecruitPeopleOverview.<>c__DisplayClass51_0();
			CS$<>8__locals1.<>4__this = this;
			this._isInConfirm = true;
			CS$<>8__locals1.count = 0;
			CS$<>8__locals1.countMax = this._selectedCharacterIds.Count;
			CS$<>8__locals1.allCharIds = new List<int>();
			List<BuildingRecruitCharacterData> sortedDesc = this._selectedCharacterIds.ToList<BuildingRecruitCharacterData>();
			sortedDesc.Sort((BuildingRecruitCharacterData a, BuildingRecruitCharacterData b) => b.RecruitInfoIndex - a.RecruitInfoIndex);
			using (List<BuildingRecruitCharacterData>.Enumerator enumerator = sortedDesc.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BuildingRecruitCharacterData item = enumerator.Current;
					BuildingDomainMethod.AsyncCall.AcceptBuildingBlockRecruitPeople(this, item.BuildingBlockKey, item.RecruitInfoIndex, delegate(int offset, RawDataPool pool)
					{
						int charId = -1;
						Serializer.Deserialize(pool, offset, ref charId);
						CS$<>8__locals1.count++;
						CS$<>8__locals1.allCharIds.Add(charId);
						bool flag = CS$<>8__locals1.count >= CS$<>8__locals1.countMax;
						if (flag)
						{
							CS$<>8__locals1.<OnRecurit>g__OnFinished|0();
						}
						UIElement.BuildingArea.UiBaseAs<ViewBuildingArea>().UpdateShopGetItemInfo(item.BuildingBlockKey);
					});
				}
			}
			this._selectedCharacterIds.Clear();
			this._buildingTypeSelections.Clear();
		}

		// Token: 0x06009A98 RID: 39576 RVA: 0x0048687C File Offset: 0x00484A7C
		private void InitSubPageToggles()
		{
			bool flag = this.subPageToggleGroup == null;
			if (!flag)
			{
				this.subPageToggleGroup.gameObject.SetActive(false);
			}
		}

		// Token: 0x06009A99 RID: 39577 RVA: 0x004868B0 File Offset: 0x00484AB0
		private void InitSortAndFilter()
		{
			bool flag = this.sortAndFilter == null;
			if (!flag)
			{
				this._sortAndFilterController = new RecruitCharacterSortAndFilterController(this.sortAndFilter, new List<ESelectCharacterFilterMenuId>
				{
					ESelectCharacterFilterMenuId.Gender
				});
				this._tabSortStateManager = new TabSortStateManager<int, BuildingRecruitCharacterData>(this._sortAndFilterController);
				this._sortAndFilterController.Init(new Action(this.OnSortAndFilterChanged), "SelectCharacter");
			}
		}

		// Token: 0x06009A9A RID: 39578 RVA: 0x0048691C File Offset: 0x00484B1C
		private void OnSortAndFilterChanged()
		{
			bool skipSort = this._tabSortStateManager != null && !this._tabSortStateManager.ShouldSort();
			this.HandleData(!skipSort);
			this.RefreshDisplay();
		}

		// Token: 0x06009A9B RID: 39579 RVA: 0x00486956 File Offset: 0x00484B56
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitions()
		{
			yield return this.CreateAvatarWithNameColumn();
			foreach (ColumnDefinition col in this.GenerateOverviewColumns())
			{
				yield return col;
				col = null;
			}
			IEnumerator<ColumnDefinition> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06009A9C RID: 39580 RVA: 0x00486966 File Offset: 0x00484B66
		private IEnumerable<ColumnDefinition> GenerateOverviewColumns()
		{
			yield return ViewRecruitPeopleOverview.CreateIconAndTextColumn(() => LanguageKey.LK_Building_RecruitPeople_StayMonthTitle.Tr(), new Func<BuildingRecruitCharacterData, IconAndTextCellData>(this.GetStayMonthCellData), -1, 30f, 260f);
			yield return ViewRecruitPeopleOverview.CreateIconAndTextColumn(() => LanguageKey.LK_EventWindow_FameActionAuthorityCost.Tr(), new Func<BuildingRecruitCharacterData, IconAndTextCellData>(this.GetAuthorityCellData), -1, 30f, 260f);
			yield return ViewRecruitPeopleOverview.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Gender.Tr(), new Func<BuildingRecruitCharacterData, string>(ViewRecruitPeopleOverview.GetGenderDisplayString), 122, 30f, 255f);
			yield return ViewRecruitPeopleOverview.CreateTextColumn(() => LanguageKey.LK_Char_Age.Tr(), (BuildingRecruitCharacterData data) => data.CharacterData.Age.ToString(), 8, 30f, 255f);
			yield return ViewRecruitPeopleOverview.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Behavior.Tr(), new Func<BuildingRecruitCharacterData, string>(ViewRecruitPeopleOverview.GetBehaviorDisplayString), 57, 30f, 255f);
			yield return ViewRecruitPeopleOverview.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Charm.Tr(), new Func<BuildingRecruitCharacterData, string>(ViewRecruitPeopleOverview.GetCharmDisplayString), 9, 30f, 255f);
			yield return ViewRecruitPeopleOverview.CreateTextColumn(() => LanguageKey.LK_Qualification.Tr(), new Func<BuildingRecruitCharacterData, string>(this.GetQualificationDisplayString), -1, 30f, 255f);
			yield return ViewRecruitPeopleOverview.CreateTextColumn(() => LanguageKey.LK_Growth.Tr(), new Func<BuildingRecruitCharacterData, string>(ViewRecruitPeopleOverview.GetGrowthTypeDisplayString), 119, 30f, 255f);
			yield break;
		}

		// Token: 0x06009A9D RID: 39581 RVA: 0x00486978 File Offset: 0x00484B78
		private IconAndTextCellData GetStayMonthCellData(BuildingRecruitCharacterData data)
		{
			return new IconAndTextCellData("ui9_icon_month", this.GetStayMonthDisplayString(data), true, false, false, true);
		}

		// Token: 0x06009A9E RID: 39582 RVA: 0x004869A0 File Offset: 0x00484BA0
		private IconAndTextCellData GetAuthorityCellData(BuildingRecruitCharacterData data)
		{
			bool flag = this._buildingBlockKeyTemplateIdDic[data.BuildingBlockKey] == 223;
			IconAndTextCellData result;
			if (flag)
			{
				result = new IconAndTextCellData("ui9_icon_resource_big_7", GlobalConfig.Instance.RecruitPeopleCost.ToString(), true, false, false, false);
			}
			else
			{
				result = IconAndTextCellData.TextOnly("-");
			}
			return result;
		}

		// Token: 0x06009A9F RID: 39583 RVA: 0x004869FC File Offset: 0x00484BFC
		private string GetStayMonthDisplayString(BuildingRecruitCharacterData data)
		{
			short buildingTemplateId;
			bool flag = !this._buildingBlockKeyTemplateIdDic.TryGetValue(data.BuildingBlockKey, out buildingTemplateId);
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				result = (GameData.Domains.Building.SharedMethods.GetBuildingEarnPreserveTime(buildingTemplateId) - data.RecruitLevel.Second).ToString();
			}
			return result;
		}

		// Token: 0x06009AA0 RID: 39584 RVA: 0x00486A4C File Offset: 0x00484C4C
		private static string GetGenderDisplayString(BuildingRecruitCharacterData data)
		{
			CommonUtils.EDisplayGender displayGender = CommonUtils.GetDisplayGender(data.CharacterData.Gender, -1);
			return CommonUtils.GetGenderString(displayGender);
		}

		// Token: 0x06009AA1 RID: 39585 RVA: 0x00486A78 File Offset: 0x00484C78
		private static string GetBehaviorDisplayString(BuildingRecruitCharacterData data)
		{
			short baseMorality = data.CharacterData.GetBaseMorality();
			sbyte behaviorType = GameData.Domains.Character.BehaviorType.GetBehaviorType(baseMorality);
			return CommonUtils.GetBehaviorString(behaviorType);
		}

		// Token: 0x06009AA2 RID: 39586 RVA: 0x00486AA4 File Offset: 0x00484CA4
		private static string GetCharmDisplayString(BuildingRecruitCharacterData data)
		{
			CharacterItem charConfig = Character.Instance.GetItem(data.CharacterData.TemplateId);
			bool isFixedPresetType = CreatingType.IsFixedPresetType(charConfig.CreatingType);
			return CommonUtils.GetCharmLevelText(data.CharacterData.FinalAttraction, data.CharacterData.Gender, data.CharacterData.Age, data.CharacterData.AvatarData.ClothDisplayId, isFixedPresetType, true);
		}

		// Token: 0x06009AA3 RID: 39587 RVA: 0x00486B10 File Offset: 0x00484D10
		private string GetQualificationDisplayString(BuildingRecruitCharacterData data)
		{
			short buildingTemplateId;
			bool flag = !this._buildingBlockKeyTemplateIdDic.TryGetValue(data.BuildingBlockKey, out buildingTemplateId);
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				BuildingBlockItem buildingConfig = BuildingBlock.Instance[buildingTemplateId];
				bool flag2 = buildingConfig.RequireCombatSkillType >= 0;
				if (flag2)
				{
					result = data.CharacterData.CombatSkillQualifications[(int)buildingConfig.RequireCombatSkillType].ToString();
				}
				else
				{
					result = data.CharacterData.LifeSkillQualifications[(int)buildingConfig.RequireLifeSkillType].ToString();
				}
			}
			return result;
		}

		// Token: 0x06009AA4 RID: 39588 RVA: 0x00486B9C File Offset: 0x00484D9C
		private static string GetGrowthTypeDisplayString(BuildingRecruitCharacterData data)
		{
			sbyte combatSkillQualificationGrowthType = data.CharacterData.CombatSkillQualificationGrowthType;
			if (!true)
			{
			}
			string result;
			if (combatSkillQualificationGrowthType != 0)
			{
				if (combatSkillQualificationGrowthType != 1)
				{
					result = LanguageKey.LK_Qualification_Growth_LateBlooming.Tr();
				}
				else
				{
					result = LanguageKey.LK_Qualification_Growth_Precocious.Tr();
				}
			}
			else
			{
				result = LanguageKey.LK_Qualification_Growth_Average.Tr();
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06009AA5 RID: 39589 RVA: 0x00486BF8 File Offset: 0x00484DF8
		private ColumnDefinition CreateAvatarWithNameColumn()
		{
			ColumnDefinition<BuildingRecruitCharacterData, AvatarWithNameCellData> columnDefinition = new ColumnDefinition<BuildingRecruitCharacterData, AvatarWithNameCellData>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 330f,
				FlexibleWidth = 0f,
				PreferredWidth = 330f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Char_Name.Tr());
			columnDefinition.CellDataGenerator = ((BuildingRecruitCharacterData data) => AvatarWithNameCellData.FromRecruitData(data, null, null));
			columnDefinition.SortId = 0;
			return columnDefinition;
		}

		// Token: 0x06009AA6 RID: 39590 RVA: 0x00486C9C File Offset: 0x00484E9C
		private static ColumnDefinition CreateTextColumn(Func<string> headerKey, Func<BuildingRecruitCharacterData, string> valueGetter, short sortId = -1, float minWidth = 30f, float preferredWidth = 255f)
		{
			return new ColumnDefinition<BuildingRecruitCharacterData, string>
			{
				LayoutOption = new LayoutOption
				{
					MinWidth = minWidth,
					FlexibleWidth = 1f,
					PreferredWidth = preferredWidth,
					Priority = 1
				},
				TableHeadLabel = headerKey,
				CellDataGenerator = ((BuildingRecruitCharacterData data) => valueGetter(data)),
				SortId = sortId
			};
		}

		// Token: 0x06009AA7 RID: 39591 RVA: 0x00486D14 File Offset: 0x00484F14
		private static ColumnDefinition CreateIconAndTextColumn(Func<string> headerKey, Func<BuildingRecruitCharacterData, IconAndTextCellData> valueGetter, short sortId = -1, float minWidth = 30f, float preferredWidth = 260f)
		{
			return new ColumnDefinition<BuildingRecruitCharacterData, IconAndTextCellData>
			{
				LayoutOption = new LayoutOption
				{
					MinWidth = minWidth,
					PreferredWidth = preferredWidth,
					FlexibleWidth = 1f
				},
				TableHeadLabel = headerKey,
				CellDataGenerator = valueGetter,
				SortId = sortId
			};
		}

		// Token: 0x06009AA8 RID: 39592 RVA: 0x00486D6C File Offset: 0x00484F6C
		private void OnDestroy()
		{
			TabSortStateManager<int, BuildingRecruitCharacterData> tabSortStateManager = this._tabSortStateManager;
			if (tabSortStateManager != null)
			{
				tabSortStateManager.ClearAll();
			}
		}

		// Token: 0x06009AA9 RID: 39593 RVA: 0x00486D84 File Offset: 0x00484F84
		private void RefreshDisplay()
		{
			bool currentInListMode = this._currentInListMode;
			if (currentInListMode)
			{
				this.RefreshListMode();
			}
			else
			{
				this.RefreshFlatMode();
			}
		}

		// Token: 0x06009AAA RID: 39594 RVA: 0x00486DAC File Offset: 0x00484FAC
		private void RefreshListMode()
		{
			bool flag = !this._listModeDirty;
			if (!flag)
			{
				this.SetListScrollData(this._filteredDataList, false);
				this._listModeDirty = false;
			}
		}

		// Token: 0x06009AAB RID: 39595 RVA: 0x00486DE0 File Offset: 0x00484FE0
		private void RefreshFlatMode()
		{
			bool flag = !this._flatModeDirty;
			if (!flag)
			{
				CommonUtils.PrepareEnoughChildren(this.flatModeRect, this.flatModeTemplate.gameObject, this._cachedData.Count, null);
				for (int i = 0; i < this._cachedData.Count; i++)
				{
					RecruitFlatModeScrollView container = this.flatModeRect.GetChild(i).GetComponent<RecruitFlatModeScrollView>();
					container.Set(this._cachedData[i].CharacterDataList, this._selectedCharacterIds, this._cachedData[i].BuildingTemplateId, new Action<BuildingRecruitCharacterData>(this.OnFlatCharClicked));
				}
				this._flatModeDirty = false;
			}
		}

		// Token: 0x06009AAC RID: 39596 RVA: 0x00486EA0 File Offset: 0x004850A0
		private void RefreshListStructure()
		{
			IEnumerable<ColumnDefinition> columnDefinitions = this.GenerateColumnDefinitions();
			this.PrepareRowTemplateContainers();
			this.UpdateSelectedArea();
			this.InitScrollHead(columnDefinitions);
			this._listStructureReady = true;
		}

		// Token: 0x06009AAD RID: 39597 RVA: 0x00486ED4 File Offset: 0x004850D4
		private void InitScrollHead(IEnumerable<ColumnDefinition> columnDefinitions)
		{
			this._columnDefinitions.Clear();
			this._columnDefinitions.AddRange(columnDefinitions);
			this._tableHeadCells.Clear();
			this._columnSortIds = (from c in this._columnDefinitions
			select c.SortId).ToArray<short>();
			CommonUtils.PrepareEnoughChildren(this.tableHeadRoot.transform, this.tableHeadCellTemplate.gameObject, this._columnDefinitions.Count, null);
			for (int i = 0; i < this._columnDefinitions.Count; i++)
			{
				TableHeadCell headCell = this.tableHeadRoot.transform.GetChild(i).GetComponent<TableHeadCell>();
				headCell.Init(this._columnDefinitions[i], i < this._columnDefinitions.Count - 1, i, new Action<int, short>(this.OnTableHeadCellClicked));
				this._tableHeadCells.Add(headCell);
			}
			bool flag = this._sortAndFilterController != null;
			if (flag)
			{
				RecruitCharacterSortAndFilterController sortController = this._sortAndFilterController;
				sortController.UnregisterTableHead(this);
				sortController.RegisterTableHead(this, this._columnSortIds);
				this.SyncSortStateFromController();
			}
		}

		// Token: 0x06009AAE RID: 39598 RVA: 0x00487014 File Offset: 0x00485214
		public void SyncSortStateFromController()
		{
			bool flag = this._sortAndFilterController == null;
			if (flag)
			{
				this.ClearAllSortStates();
			}
			else
			{
				SortStateData sortData = this._sortAndFilterController.SortAndFilterState.SortData;
				bool flag2 = ((sortData != null) ? sortData.ItemStates : null) == null || sortData.ItemStates.Count == 0;
				if (flag2)
				{
					this.ClearAllSortStates();
				}
				else
				{
					foreach (TableHeadCell headCell in this._tableHeadCells)
					{
						short sortId = headCell.SortId;
						bool flag3 = sortId < 0;
						if (flag3)
						{
							headCell.ClearSortState();
						}
						else
						{
							int sortOrder = -1;
							ESortDirection direction = ESortDirection.Descending;
							for (int i = 0; i < sortData.ItemStates.Count; i++)
							{
								bool flag4 = sortData.ItemStates[i].SortId == sortId;
								if (flag4)
								{
									sortOrder = i;
									direction = sortData.ItemStates[i].SortDirection;
									break;
								}
							}
							headCell.SetSortState(sortOrder, direction);
						}
					}
				}
			}
		}

		// Token: 0x06009AAF RID: 39599 RVA: 0x00487154 File Offset: 0x00485354
		private void ClearAllSortStates()
		{
			foreach (TableHeadCell headCell in this._tableHeadCells)
			{
				headCell.ClearSortState();
			}
		}

		// Token: 0x06009AB0 RID: 39600 RVA: 0x004871AC File Offset: 0x004853AC
		private void OnTableHeadCellClicked(int columnIndex, short sortId)
		{
			bool flag = this._sortAndFilterController == null || sortId < 0;
			if (!flag)
			{
				SortStateData currentState = this._sortAndFilterController.SortAndFilterState.SortData;
				List<SortItemState> newItemStates = new List<SortItemState>();
				int existingIndex = -1;
				ESortDirection existingDirection = ESortDirection.Descending;
				bool flag2 = ((currentState != null) ? currentState.ItemStates : null) != null;
				if (flag2)
				{
					for (int i = 0; i < currentState.ItemStates.Count; i++)
					{
						bool flag3 = currentState.ItemStates[i].SortId == sortId;
						if (flag3)
						{
							existingIndex = i;
							existingDirection = currentState.ItemStates[i].SortDirection;
							break;
						}
					}
				}
				bool flag4 = existingIndex >= 0;
				if (flag4)
				{
					bool flag5 = ((currentState != null) ? currentState.ItemStates : null) != null;
					if (flag5)
					{
						foreach (SortItemState item in currentState.ItemStates)
						{
							bool flag6 = item.SortId != sortId;
							if (flag6)
							{
								newItemStates.Add(item);
							}
						}
					}
					bool flag7 = existingDirection == ESortDirection.Descending;
					if (flag7)
					{
						newItemStates.Insert(existingIndex, new SortItemState
						{
							SortId = sortId,
							SortDirection = ESortDirection.Ascending
						});
					}
				}
				else
				{
					bool flag8 = ((currentState != null) ? currentState.ItemStates : null) != null;
					if (flag8)
					{
						newItemStates.AddRange(currentState.ItemStates);
					}
					newItemStates.Add(new SortItemState
					{
						SortId = sortId,
						SortDirection = ESortDirection.Descending
					});
				}
				this._sortAndFilterController.SetSortState(new SortStateData
				{
					ItemStates = newItemStates
				});
			}
		}

		// Token: 0x06009AB1 RID: 39601 RVA: 0x00487378 File Offset: 0x00485578
		private void UpdateSelectedArea()
		{
		}

		// Token: 0x06009AB2 RID: 39602 RVA: 0x0048737C File Offset: 0x0048557C
		private void HandleData(bool needSort = true)
		{
			this._filteredDataList.Clear();
			this._titleRowBuildingDic.Clear();
			this._flatModeDirty = true;
			this._listModeDirty = true;
			Comparison<BuildingRecruitCharacterData> comparison;
			if (!needSort)
			{
				comparison = null;
			}
			else
			{
				RecruitCharacterSortAndFilterController sortAndFilterController = this._sortAndFilterController;
				comparison = ((sortAndFilterController != null) ? sortAndFilterController.GenerateComparer(this._filteredDataList) : null);
			}
			Comparison<BuildingRecruitCharacterData> comparer = comparison;
			List<BuildingRecruitCharacterData> allData = new List<BuildingRecruitCharacterData>();
			List<BuildingRecruitCharacterData> searchFiltered = new List<BuildingRecruitCharacterData>();
			foreach (BuildingRecruitData item in this._cachedData)
			{
				bool flag = item.CharacterDataList != null;
				if (flag)
				{
					allData.AddRange(item.CharacterDataList);
				}
				searchFiltered = item.CharacterDataList;
				RecruitCharacterSortAndFilterController sortAndFilterController2 = this._sortAndFilterController;
				Func<BuildingRecruitCharacterData, bool> func;
				if ((func = ((sortAndFilterController2 != null) ? sortAndFilterController2.GenerateFilter() : null)) == null && (func = ViewRecruitPeopleOverview.<>c.<>9__79_0) == null)
				{
					func = (ViewRecruitPeopleOverview.<>c.<>9__79_0 = ((BuildingRecruitCharacterData _) => true));
				}
				Func<BuildingRecruitCharacterData, bool> filter = func;
				List<BuildingRecruitCharacterData> filtered = searchFiltered.Where(filter).ToList<BuildingRecruitCharacterData>();
				bool flag2 = comparer != null;
				if (flag2)
				{
					filtered.Sort(comparer);
				}
				this._titleRowBuildingDic[this._filteredDataList.Count] = item.BuildingTemplateId;
				this._filteredDataList.Add(null);
				this._filteredDataList.AddRange(filtered);
			}
			RecruitCharacterSortAndFilterController sortAndFilterController3 = this._sortAndFilterController;
			if (sortAndFilterController3 != null)
			{
				sortAndFilterController3.AfterFilter(allData);
			}
		}

		// Token: 0x06009AB3 RID: 39603 RVA: 0x004874F4 File Offset: 0x004856F4
		private void SetListScrollData(List<BuildingRecruitCharacterData> filteredDataList, bool needRegenerate = false)
		{
			bool flag = this._currentTemplate == null;
			if (!flag)
			{
				bool flag2 = needRegenerate || this._rowTemplateChange;
				if (flag2)
				{
					for (int i = this.listCharacterRect.childCount - 1; i >= 0; i--)
					{
						Object.DestroyImmediate(this.listCharacterRect.GetChild(i).gameObject);
					}
					this._rowTemplateChange = false;
				}
				CommonUtils.PrepareEnoughChildren(this.listCharacterRect, this._currentTemplate.gameObject, filteredDataList.Count, null);
				for (int j = 0; j < filteredDataList.Count; j++)
				{
					this.OnListItemRender(j, this.listCharacterRect.GetChild(j).gameObject);
				}
			}
		}

		// Token: 0x06009AB4 RID: 39604 RVA: 0x004875C8 File Offset: 0x004857C8
		private void UpdateButtonState()
		{
			this.btnRecuit.interactable = (this._selectedCharacterIds.Count > 0);
			this.btnRefuse.interactable = (this._selectedCharacterIds.Count > 0);
			int ownedAuthority = SingletonObject.getInstance<BuildingModel>().GetResourceCount(7);
			int needAuthority = this._selectedCharacterIds.Count((BuildingRecruitCharacterData t) => this._buildingBlockKeyTemplateIdDic[t.BuildingBlockKey] == 223) * (int)GlobalConfig.Instance.RecruitPeopleCost;
			bool enough = ownedAuthority >= needAuthority;
			bool flag = needAuthority == 0;
			if (flag)
			{
				this.txtAuthority.text = "-/-";
			}
			else
			{
				this.txtAuthority.text = string.Format("{0}/{1}", ownedAuthority.ToString().SetColor(enough ? "brightblue" : "brightred"), needAuthority);
			}
		}

		// Token: 0x06009AB5 RID: 39605 RVA: 0x00487698 File Offset: 0x00485898
		private static string DefaultSearchTextExtractor(ISelectCharacterData data)
		{
			CharacterDisplayDataForGeneralScrollList generalData = data.GetGeneralScrollListData();
			bool flag = generalData == null;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				result = NameCenter.GetMonasticTitleOrDisplayName(ref generalData.NameData, false, false);
			}
			return result;
		}

		// Token: 0x06009AB6 RID: 39606 RVA: 0x004876D0 File Offset: 0x004858D0
		private void OnFlatCharClicked(BuildingRecruitCharacterData target)
		{
			bool flag = target == null;
			if (!flag)
			{
				this.OnCharacterClicked(target);
			}
		}

		// Token: 0x06009AB7 RID: 39607 RVA: 0x004876F4 File Offset: 0x004858F4
		private void OnRowClicked(int index, RowItem rowItem)
		{
			bool flag = index < 0 || index >= this._filteredDataList.Count;
			if (!flag)
			{
				BuildingRecruitCharacterData data = this._filteredDataList[index];
				this.OnCharacterClicked(data);
			}
		}

		// Token: 0x06009AB8 RID: 39608 RVA: 0x00487738 File Offset: 0x00485938
		private void OnCharacterClicked(BuildingRecruitCharacterData data)
		{
			short buildingTemplateId = this._buildingBlockKeyTemplateIdDic[data.BuildingBlockKey];
			bool flag = !this._buildingTypeSelections.ContainsKey(buildingTemplateId);
			if (flag)
			{
				this._buildingTypeSelections[buildingTemplateId] = 0;
			}
			bool flag2 = this._selectedCharacterIds.Contains(data);
			if (flag2)
			{
				this._selectedCharacterIds.Remove(data);
				Dictionary<short, int> buildingTypeSelections = this._buildingTypeSelections;
				short key = buildingTemplateId;
				int num = buildingTypeSelections[key];
				buildingTypeSelections[key] = num - 1;
			}
			else
			{
				this._selectedCharacterIds.Add(data);
				Dictionary<short, int> buildingTypeSelections2 = this._buildingTypeSelections;
				short key = buildingTemplateId;
				int num = buildingTypeSelections2[key];
				buildingTypeSelections2[key] = num + 1;
			}
			int currentSelectedAmount = 0;
			foreach (KeyValuePair<short, int> item in this._buildingTypeSelections)
			{
				currentSelectedAmount += item.Value;
			}
			this.SetTotalAmount(currentSelectedAmount, this._cachedDataCount);
			this.UpdateSelectedArea();
			this.UpdateButtonState();
			this.HandleData(true);
			this.RefreshDisplay();
		}

		// Token: 0x06009AB9 RID: 39609 RVA: 0x00487868 File Offset: 0x00485A68
		private void SetTotalAmount(int currentSelectedAmount, int cachedDataCount)
		{
			this.txtTotalSelectAmount.text = LocalStringManager.GetFormat(LanguageKey.UI_CheckInspcription_InlcudeCount, currentSelectedAmount, this._cachedDataCount);
			this.selectAllSign.gameObject.SetActive(cachedDataCount > 0 && cachedDataCount <= currentSelectedAmount);
		}

		// Token: 0x06009ABA RID: 39610 RVA: 0x004878BC File Offset: 0x00485ABC
		private void PrepareRowTemplateContainers()
		{
			int subPageKey = 0;
			RowItem currentTemplate;
			bool flag = this._rowTemplateCache.TryGetValue(subPageKey, out currentTemplate);
			if (flag)
			{
				this._currentTemplate = currentTemplate;
			}
			else
			{
				currentTemplate = this.CreateRowTemplateForSubPage();
				this._rowTemplateCache[subPageKey] = currentTemplate;
				this._currentTemplate = currentTemplate;
			}
			this._rowTemplateChange = true;
		}

		// Token: 0x06009ABB RID: 39611 RVA: 0x00487910 File Offset: 0x00485B10
		private RowItem CreateRowTemplateForSubPage()
		{
			RowItem newTemplate = Object.Instantiate<RowItem>(this.rowTemplate, this.rowTemplate.transform.parent);
			newTemplate.gameObject.SetActive(false);
			Transform containerRoot = newTemplate.ContainerRoot;
			RowCellContainer avatarContainer = Object.Instantiate<RowCellContainer>(this.avatarAndNameCellContainer, containerRoot);
			avatarContainer.gameObject.SetActive(true);
			IEnumerable<ColumnDefinition> columnDefinitions = this.GenerateColumnDefinitionsForCurrentSubPage();
			foreach (ColumnDefinition columnDef in columnDefinitions)
			{
				Type[] genericArgs = columnDef.GetType().GetGenericArguments();
				bool flag = genericArgs.Length >= 2 && genericArgs[1] == typeof(IconAndTextCellData);
				RowCellContainer container;
				if (flag)
				{
					container = Object.Instantiate<RowCellContainer>(this.iconAndTextCellContainer, containerRoot);
				}
				else
				{
					container = Object.Instantiate<RowCellContainer>(this.singleTextCellContainer, containerRoot);
				}
				container.gameObject.SetActive(true);
			}
			newTemplate.ResetSibling();
			return newTemplate;
		}

		// Token: 0x06009ABC RID: 39612 RVA: 0x00487A1C File Offset: 0x00485C1C
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitionsForCurrentSubPage()
		{
			return this.GenerateOverviewColumns();
		}

		// Token: 0x06009ABD RID: 39613 RVA: 0x00487A34 File Offset: 0x00485C34
		public override void OnInit(ArgumentBox argsBox)
		{
			bool flag = argsBox == null || !argsBox.Get<BuildingBlockKey>("BuildingBlockKey", out this._specifiedBuildingBlockKey);
			if (flag)
			{
				this._specifiedBuildingBlockKey = BuildingBlockKey.Invalid;
			}
			this._flatModeDirty = true;
			this._listModeDirty = true;
			this.txtTotalSelectAmount.text = string.Empty;
			this._buildingBlockKeys.Clear();
			this._buildingTypeSelections.Clear();
			this._recruitCount.Clear();
			this._isInConfirm = false;
			foreach (GameObject obj in this._instantiatedBlockObjects.Values.ToArray<GameObject>())
			{
				Object.Destroy(obj);
			}
			this._instantiatedBlockObjects.Clear();
			this._instantiatedBlockTemplateObjectsRecord.Clear();
			base.StopAllCoroutines();
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.CallRefresh));
		}

		// Token: 0x06009ABE RID: 39614 RVA: 0x00487B2D File Offset: 0x00485D2D
		private void CallRefresh()
		{
			ExtraDomainMethod.Call.RequestAllRecruitCharacterData(this.Element.GameDataListenerId, this._specifiedBuildingBlockKey);
		}

		// Token: 0x06009ABF RID: 39615 RVA: 0x00487B48 File Offset: 0x00485D48
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 1)
				{
					bool flag = notification.DomainId == 19;
					if (flag)
					{
						bool flag2 = notification.MethodId == 233;
						if (flag2)
						{
							this._cachedData.Clear();
							this._buildingBlockKeyTemplateIdDic.Clear();
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._cachedData);
							foreach (BuildingRecruitData item in this._cachedData)
							{
								bool flag3 = item.CharacterDataList == null;
								if (!flag3)
								{
									foreach (BuildingRecruitCharacterData characterData in item.CharacterDataList)
									{
										this._buildingBlockKeyTemplateIdDic[characterData.BuildingBlockKey] = item.BuildingTemplateId;
									}
								}
							}
							this.SetTotalAmount(0, this._cachedDataCount);
							this.Element.ShowAfterRefresh();
							this.RefreshByCachedData();
						}
					}
				}
			}
		}

		// Token: 0x06009AC0 RID: 39616 RVA: 0x00487D08 File Offset: 0x00485F08
		private void RefreshByCachedData()
		{
			this.HandleData(true);
			this.RefreshDisplay();
			this.UpdateButtonState();
		}

		// Token: 0x06009AC1 RID: 39617 RVA: 0x00487D24 File Offset: 0x00485F24
		private void ShowGetPeopleView(List<int> charIdList)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.SetObject("CharIdList", charIdList);
			argBox.Set("ObtainType", 15);
			UIElement.GetItem.SetOnInitArgs(argBox);
			UIManager.Instance.MaskUI(UIElement.GetItem);
			GEvent.OnEvent(UiEvents.OnUpdateQuickBtnState, null);
		}

		// Token: 0x06009AC2 RID: 39618 RVA: 0x00487D80 File Offset: 0x00485F80
		private void OnDisable()
		{
			GEvent.OnEvent(UiEvents.OnUpdateQuickBtnState, null);
			this._selectedCharacterIds.Clear();
		}

		// Token: 0x06009AC3 RID: 39619 RVA: 0x00487DA0 File Offset: 0x00485FA0
		public void BindSortController(ISortAndFilterController sortController, short[] columnSortIds)
		{
			this.SyncSortStateFromController();
		}

		// Token: 0x06009AC4 RID: 39620 RVA: 0x00487DAA File Offset: 0x00485FAA
		public void UnbindSortController()
		{
			this.ClearAllSortStates();
		}

		// Token: 0x04007794 RID: 30612
		[Header("表头")]
		[SerializeField]
		private HorizontalLayoutGroup tableHeadRoot;

		// Token: 0x04007795 RID: 30613
		[SerializeField]
		private TableHeadCell tableHeadCellTemplate;

		// Token: 0x04007796 RID: 30614
		[SerializeField]
		private CToggleGroup subPageToggleGroup;

		// Token: 0x04007797 RID: 30615
		[SerializeField]
		private SortAndFilter sortAndFilter;

		// Token: 0x04007798 RID: 30616
		[Header("交互")]
		[SerializeField]
		private CButton btnClose;

		// Token: 0x04007799 RID: 30617
		[SerializeField]
		private CButton btnRecuit;

		// Token: 0x0400779A RID: 30618
		[SerializeField]
		private CButton btnRefuse;

		// Token: 0x0400779B RID: 30619
		[SerializeField]
		private TextMeshProUGUI txtTotalSelectAmount;

		// Token: 0x0400779C RID: 30620
		[SerializeField]
		private CButton btnSelectAll;

		// Token: 0x0400779D RID: 30621
		[SerializeField]
		private CToggleGroup switchToggle;

		// Token: 0x0400779E RID: 30622
		[SerializeField]
		private GameObject selectAllSign;

		// Token: 0x0400779F RID: 30623
		[Header("行模板配置")]
		[SerializeField]
		private RowItem rowTemplate;

		// Token: 0x040077A0 RID: 30624
		[SerializeField]
		private RowCellContainer avatarAndNameCellContainer;

		// Token: 0x040077A1 RID: 30625
		[SerializeField]
		private RowCellContainer singleTextCellContainer;

		// Token: 0x040077A2 RID: 30626
		[SerializeField]
		private RowCellContainer iconAndTextCellContainer;

		// Token: 0x040077A3 RID: 30627
		[Header("表结构")]
		[SerializeField]
		private RectTransform listCharacterRect;

		// Token: 0x040077A4 RID: 30628
		[SerializeField]
		private GameObject listModeArea;

		// Token: 0x040077A5 RID: 30629
		[SerializeField]
		private GameObject flatModeArea;

		// Token: 0x040077A6 RID: 30630
		[Header("平铺模式")]
		[SerializeField]
		private RecruitFlatModeScrollView flatModeTemplate;

		// Token: 0x040077A7 RID: 30631
		[SerializeField]
		private RectTransform flatModeRect;

		// Token: 0x040077A8 RID: 30632
		[Header("威望")]
		[SerializeField]
		private TextMeshProUGUI txtAuthority;

		// Token: 0x040077A9 RID: 30633
		private const ERecruitBaseSubPage _currentSubPage = ERecruitBaseSubPage.Overview;

		// Token: 0x040077AA RID: 30634
		private TabSortStateManager<int, BuildingRecruitCharacterData> _tabSortStateManager;

		// Token: 0x040077AB RID: 30635
		private RecruitCharacterSortAndFilterController _sortAndFilterController;

		// Token: 0x040077AC RID: 30636
		private bool _listStructureReady;

		// Token: 0x040077AD RID: 30637
		private readonly List<BuildingRecruitCharacterData> _filteredDataList = new List<BuildingRecruitCharacterData>();

		// Token: 0x040077AE RID: 30638
		private readonly List<ColumnDefinition> _columnDefinitions = new List<ColumnDefinition>();

		// Token: 0x040077AF RID: 30639
		private readonly List<TableHeadCell> _tableHeadCells = new List<TableHeadCell>();

		// Token: 0x040077B0 RID: 30640
		private readonly Dictionary<int, RowItem> _rowTemplateCache = new Dictionary<int, RowItem>();

		// Token: 0x040077B1 RID: 30641
		private readonly Dictionary<int, short> _titleRowBuildingDic = new Dictionary<int, short>();

		// Token: 0x040077B2 RID: 30642
		private List<BuildingRecruitData> _cachedData = new List<BuildingRecruitData>();

		// Token: 0x040077B3 RID: 30643
		private readonly Dictionary<BuildingBlockKey, short> _buildingBlockKeyTemplateIdDic = new Dictionary<BuildingBlockKey, short>();

		// Token: 0x040077B4 RID: 30644
		private bool _flatModeDirty = true;

		// Token: 0x040077B5 RID: 30645
		private bool _listModeDirty = true;

		// Token: 0x040077B6 RID: 30646
		private bool _rowTemplateChange = false;

		// Token: 0x040077B7 RID: 30647
		private BuildingBlockKey _specifiedBuildingBlockKey;

		// Token: 0x040077B8 RID: 30648
		private readonly HashSet<BuildingBlockKey> _buildingBlockKeys = new HashSet<BuildingBlockKey>();

		// Token: 0x040077B9 RID: 30649
		private readonly HashSet<BuildingRecruitCharacterData> _selectedCharacterIds = new HashSet<BuildingRecruitCharacterData>();

		// Token: 0x040077BA RID: 30650
		private readonly Dictionary<short, int> _buildingTypeSelections = new Dictionary<short, int>();

		// Token: 0x040077BB RID: 30651
		private readonly Dictionary<BuildingBlockKey, int> _recruitCount = new Dictionary<BuildingBlockKey, int>();

		// Token: 0x040077BC RID: 30652
		private bool _isInConfirm;

		// Token: 0x040077BD RID: 30653
		private readonly Dictionary<BuildingBlockKey, GameObject> _instantiatedBlockObjects = new Dictionary<BuildingBlockKey, GameObject>();

		// Token: 0x040077BE RID: 30654
		private readonly Dictionary<short, GameObject> _instantiatedBlockTemplateObjectsRecord = new Dictionary<short, GameObject>();

		// Token: 0x040077BF RID: 30655
		private bool _currentInListMode = true;

		// Token: 0x040077C0 RID: 30656
		private short[] _columnSortIds;

		// Token: 0x040077C1 RID: 30657
		private RowItem _currentTemplate;
	}
}
