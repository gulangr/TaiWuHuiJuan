using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.CellContent;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.SelectCharacter;
using Game.Views.CharacterMenu;
using Game.Views.Select;
using GameData.Common;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Building
{
	// Token: 0x02000BEC RID: 3052
	public class ViewTaiwuVillageStoneRoom : UIBase
	{
		// Token: 0x06009ACC RID: 39628 RVA: 0x00487FF8 File Offset: 0x004861F8
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get("villageLevel", out this._villageLevel);
			SelectCharacterConfig config = new SelectCharacterConfig
			{
				SelectionMode = ESelectCharacterSelectionMode.Multiple,
				TargetCount = int.MaxValue,
				FilterMenuIds = new List<ESelectCharacterFilterMenuId>
				{
					ESelectCharacterFilterMenuId.Gender,
					ESelectCharacterFilterMenuId.BehaviorType,
					ESelectCharacterFilterMenuId.Relation,
					ESelectCharacterFilterMenuId.AdoreRelation,
					ESelectCharacterFilterMenuId.EnemyRelation,
					ESelectCharacterFilterMenuId.Organization,
					ESelectCharacterFilterMenuId.Sect
				}
			};
			this.CacheInitData(config, new List<ISelectCharacterData>());
			this._pendingInitialize = true;
			this._uiInitialized = false;
		}

		// Token: 0x06009ACD RID: 39629 RVA: 0x00488094 File Offset: 0x00486294
		private void CacheInitData(SelectCharacterConfig config, List<ISelectCharacterData> dataList)
		{
			this._config = config;
			this._dataList.Clear();
			bool flag = dataList != null;
			if (flag)
			{
				this._dataList.AddRange(dataList);
			}
			this._bannedCharacterIds.Clear();
			bool flag2 = this._config.BannedCharacterIds != null;
			if (flag2)
			{
				this._bannedCharacterIds.UnionWith(this._config.BannedCharacterIds);
			}
			this._selectedCharId.Clear();
			this._searchText = string.Empty;
			this._rowTemplateCache.Clear();
			this._sortAndFilterController = null;
			this._listStructureReady = false;
			this._currentSubPage = ESelectCharacterBaseSubPage.State;
		}

		// Token: 0x06009ACE RID: 39630 RVA: 0x00488137 File Offset: 0x00486337
		private void Setup()
		{
			if (this._config == null)
			{
				this._config = new SelectCharacterConfig();
			}
			this.UpdateInfoLabel();
			this.InitSubPageToggles();
			this.InitSortAndFilter();
			this.RefreshList();
			this.UpdateSelectedArea();
		}

		// Token: 0x06009ACF RID: 39631 RVA: 0x00488170 File Offset: 0x00486370
		public void SetInfoText(string text)
		{
			this._config.InfoText = text;
			this.UpdateInfoLabel();
		}

		// Token: 0x06009AD0 RID: 39632 RVA: 0x00488188 File Offset: 0x00486388
		private void Awake()
		{
			this.rowTemplate.gameObject.SetActive(false);
			this.avatarAndNameCellContainer.gameObject.SetActive(false);
			this.singleTextCellContainer.gameObject.SetActive(false);
			this.iconAndTextCellContainer.gameObject.SetActive(false);
			this.flatModeScroll.OnItemRender += this.FlatModeScroll_OnItemRender;
			this.flatModeScroll.gameObject.SetActive(true);
			this.scroll.gameObject.SetActive(false);
			bool flag = this.scroll != null && this.rowTemplate != null;
			if (flag)
			{
				this.scroll.SetRowTemplate(this.rowTemplate);
			}
			bool flag2 = this.scroll != null;
			if (flag2)
			{
				this.scroll.RowSelectedProvider = new Func<int, object, bool>(this.IsRowSelected);
			}
			this.scroll.OnRowClicked += this.OnRowClicked;
			this.scroll.RowDisabledProvider = new Func<int, object, bool>(this.IsRowDisabled);
			bool flag3 = this.searchButton != null;
			if (flag3)
			{
				this.searchButton.ClearAndAddListener(new Action(this.OnSearchButtonClicked));
			}
			bool flag4 = this.searchInput != null;
			if (flag4)
			{
				this.searchInput.onEndEdit.AddListener(new UnityAction<string>(this.OnSearchInputEndEdit));
				this.searchInput.onValueChanged.AddListener(new UnityAction<string>(this.OnSearchInputEndEdit));
			}
			this.btnSave.ClearAndAddListener(new Action(this.OnClickSave));
			this.btnRelease.ClearAndAddListener(new Action(this.OnClickRelease));
			this.btnExpel.ClearAndAddListener(new Action(this.OnClickExpel));
			this.btnKill.ClearAndAddListener(new Action(this.OnClickKill));
			this.btnClose.ClearAndAddListener(delegate
			{
				UIManager.Instance.HideUI(this.Element);
			});
			this.btnSwitchMode.Init(1);
			this.btnSwitchMode.SetWithoutNotify(1);
			this.btnSwitchMode.OnActiveIndexChange += this.OnSwitchMode;
			this.btnSelectAll.ClearAndAddListener(new Action(this.OnClickSelectAll));
		}

		// Token: 0x06009AD1 RID: 39633 RVA: 0x004883E0 File Offset: 0x004865E0
		private void OnClickSelectAll()
		{
			bool flag = this._selectedCharId.Count < this._dataList.Count;
			if (flag)
			{
				foreach (ISelectCharacterData item in this._dataList)
				{
					this._selectedCharId.Add(item.CharacterId);
				}
			}
			else
			{
				this._selectedCharId.Clear();
			}
			this.UpdateDisplay();
		}

		// Token: 0x06009AD2 RID: 39634 RVA: 0x00488478 File Offset: 0x00486678
		private void OnSwitchMode(int currIndex, int prevIndex)
		{
			Debug.Log("===========================currIndex: " + currIndex.ToString());
			this._listMode = (currIndex == 0);
			this.flatModeScroll.gameObject.SetActive(!this._listMode);
			this.scroll.gameObject.SetActive(this._listMode);
			this.subPageToggleGroup.gameObject.SetActive(this._listMode);
			this.sortAndFilter.SetSortButtonVisible(!this._listMode);
		}

		// Token: 0x06009AD3 RID: 39635 RVA: 0x00488504 File Offset: 0x00486704
		private void OnClickKill()
		{
			this.ShowDialogView(LocalStringManager.Get(LanguageKey.LK_Building_StoneSelectTitle4), LocalStringManager.Get(LanguageKey.LK_Building_StoneSaveTips4), delegate
			{
				BuildingDomainMethod.Call.DealInfectedPeople(this._selectedCharId.ToList<int>(), 4);
				this._selectedCharId.Clear();
			}, null);
		}

		// Token: 0x06009AD4 RID: 39636 RVA: 0x0048852F File Offset: 0x0048672F
		private void OnClickExpel()
		{
			this.ShowDialogView(LocalStringManager.Get(LanguageKey.LK_Building_StoneSelectTitle3), LocalStringManager.Get(LanguageKey.LK_Building_StoneSaveTips3), delegate
			{
				BuildingDomainMethod.Call.DealInfectedPeople(this._selectedCharId.ToList<int>(), 3);
				this._selectedCharId.Clear();
			}, null);
		}

		// Token: 0x06009AD5 RID: 39637 RVA: 0x0048855A File Offset: 0x0048675A
		private void OnClickRelease()
		{
			this.ShowDialogView(LocalStringManager.Get(LanguageKey.LK_Building_StoneSelectTitle2), LocalStringManager.Get(LanguageKey.LK_Building_StoneSaveTips2), delegate
			{
				BuildingDomainMethod.Call.DealInfectedPeople(this._selectedCharId.ToList<int>(), 2);
				this._selectedCharId.Clear();
			}, null);
		}

		// Token: 0x06009AD6 RID: 39638 RVA: 0x00488585 File Offset: 0x00486785
		private void OnClickSave()
		{
			this.ShowDialogView(LocalStringManager.Get(LanguageKey.LK_Building_StoneSelectTitle1), LocalStringManager.Get(LanguageKey.LK_Building_StoneSaveTips1), delegate
			{
				BuildingDomainMethod.Call.DealInfectedPeople(this._selectedCharId.ToList<int>(), 1);
				this._selectedCharId.Clear();
			}, null);
		}

		// Token: 0x06009AD7 RID: 39639 RVA: 0x004885B0 File Offset: 0x004867B0
		public void ShowDialogView(string title, string content, Action actionYes, Action actionNo = null)
		{
			DialogCmd cmd = new DialogCmd
			{
				Title = title,
				Content = content,
				Yes = delegate()
				{
					Action actionYes2 = actionYes;
					if (actionYes2 != null)
					{
						actionYes2();
					}
				},
				No = delegate()
				{
					Action actionNo2 = actionNo;
					if (actionNo2 != null)
					{
						actionNo2();
					}
				},
				GroupYesText = LocalStringManager.Get(LanguageKey.LK_HotKeyGroup_Common_Confirm),
				GroupNoText = LocalStringManager.Get(LanguageKey.LK_HotKeyGroup_Common_Cancel)
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x06009AD8 RID: 39640 RVA: 0x00488658 File Offset: 0x00486858
		private void FlatModeScroll_OnItemRender(int index, GameObject itemObj)
		{
			StoneRoomCharView charView = itemObj.GetComponent<StoneRoomCharView>();
			bool flag = this._filteredDataList.Count <= index;
			if (flag)
			{
				charView.SetEmpty();
			}
			else
			{
				CharacterDisplayDataForGeneralScrollList data = this._filteredDataList[index].GetGeneralScrollListData();
				charView.Set(data, this._selectedCharId.Contains(data.CharacterId), delegate
				{
					this.OnRowClicked(index, null);
				});
			}
		}

		// Token: 0x06009AD9 RID: 39641 RVA: 0x004886E4 File Offset: 0x004868E4
		private void OnEnable()
		{
			GlobalDomainMethod.Call.InvokeGuidingTrigger(99);
			this.searchInput.text = string.Empty;
			bool flag = this._pendingInitialize || !this._uiInitialized;
			if (flag)
			{
				this._hasCustomSubPage = false;
				this._customSubPageIndex = -1;
				this._listStructureReady = false;
				this.Setup();
				this._pendingInitialize = false;
				this._uiInitialized = true;
			}
		}

		// Token: 0x06009ADA RID: 39642 RVA: 0x00488750 File Offset: 0x00486950
		private void OnDestroy()
		{
			this.scroll.OnRowClicked -= this.OnRowClicked;
			this.btnSwitchMode.OnActiveIndexChange -= this.OnSwitchMode;
			TabSortStateManager<int, ISelectCharacterData> tabSortStateManager = this._tabSortStateManager;
			if (tabSortStateManager != null)
			{
				tabSortStateManager.ClearAll();
			}
			this.flatModeScroll.OnItemRender -= this.FlatModeScroll_OnItemRender;
		}

		// Token: 0x06009ADB RID: 39643 RVA: 0x004887B8 File Offset: 0x004869B8
		private void InitSubPageToggles()
		{
			bool flag = this.subPageToggleGroup == null;
			if (!flag)
			{
				this.subPageToggleGroup.OnActiveIndexChange -= this.OnSubPageChanged;
				List<CToggle> rawToggleList = this.subPageToggleGroup.GetAll();
				bool flag2 = rawToggleList != null;
				if (flag2)
				{
					rawToggleList.RemoveAll((CToggle t) => t == null);
				}
				bool flag3 = this._dynamicToggle != null;
				if (flag3)
				{
					int index = this.subPageToggleGroup.GetAll().IndexOf(this._dynamicToggle);
					bool flag4 = index >= 0;
					if (flag4)
					{
						this.subPageToggleGroup.Remove(index);
					}
					Object.Destroy(this._dynamicToggle.gameObject);
					this._dynamicToggle = null;
				}
				this.subPageToggleGroup.Init(-1);
				this.subPageToggleGroup.OnActiveIndexChange += this.OnSubPageChanged;
				List<CToggle> toggles = this.subPageToggleGroup.GetAll();
				int baseCount = ViewTaiwuVillageStoneRoom.BaseSubPageNameKeys.Count;
				int i = 0;
				while (i < toggles.Count && i < baseCount)
				{
					CToggle toggle = toggles[i];
					bool flag5 = toggle == null;
					if (!flag5)
					{
						Transform transform = toggle.transform.Find("Label");
						TextMeshProUGUI label = (transform != null) ? transform.GetComponent<TextMeshProUGUI>() : null;
						bool flag6 = label != null;
						if (flag6)
						{
							label.text = ViewTaiwuVillageStoneRoom.BaseSubPageNameKeys[i].Tr();
						}
					}
					i++;
				}
				this._hasCustomSubPage = !string.IsNullOrEmpty(this._config.CustomSubPageName);
				this._customSubPageIndex = -1;
				bool hasCustomSubPage = this._hasCustomSubPage;
				if (hasCustomSubPage)
				{
					bool flag7 = toggles.Count <= baseCount && toggles.Count > 0;
					if (flag7)
					{
						CToggle templateToggle = toggles[0];
						CToggle cloned = Object.Instantiate<CToggle>(templateToggle, templateToggle.transform.parent);
						cloned.transform.SetSiblingIndex(0);
						this._dynamicToggle = cloned;
						this.subPageToggleGroup.Clear();
						this.subPageToggleGroup.AddAllChildToggles();
						toggles = this.subPageToggleGroup.GetAll();
					}
					bool flag8 = toggles.Count > baseCount;
					if (flag8)
					{
						this._customSubPageIndex = 0;
						CToggle customToggle = toggles[0];
						Transform transform2 = customToggle.transform.Find("Label");
						TextMeshProUGUI label2 = (transform2 != null) ? transform2.GetComponent<TextMeshProUGUI>() : null;
						bool flag9 = label2 != null;
						if (flag9)
						{
							label2.text = this._config.CustomSubPageName;
						}
						customToggle.gameObject.SetActive(true);
						for (int j = 0; j < baseCount; j++)
						{
							CToggle toggle2 = toggles[j + 1];
							Transform transform3 = toggle2.transform.Find("Label");
							TextMeshProUGUI toggleLabel = (transform3 != null) ? transform3.GetComponent<TextMeshProUGUI>() : null;
							bool flag10 = toggleLabel != null;
							if (flag10)
							{
								toggleLabel.text = ViewTaiwuVillageStoneRoom.BaseSubPageNameKeys[j].Tr();
							}
						}
					}
					else
					{
						bool flag11 = toggles.Count > 0;
						if (flag11)
						{
							this._customSubPageIndex = Math.Min(baseCount - 1, toggles.Count - 1);
							CToggle customToggle2 = toggles[this._customSubPageIndex];
							Transform transform4 = customToggle2.transform.Find("Label");
							TextMeshProUGUI label3 = (transform4 != null) ? transform4.GetComponent<TextMeshProUGUI>() : null;
							bool flag12 = label3 != null;
							if (flag12)
							{
								label3.text = this._config.CustomSubPageName;
							}
							customToggle2.gameObject.SetActive(true);
						}
					}
					bool flag13 = this._customSubPageIndex >= 0;
					if (flag13)
					{
						this.subPageToggleGroup.Set(this._customSubPageIndex, true);
					}
				}
			}
		}

		// Token: 0x06009ADC RID: 39644 RVA: 0x00488B90 File Offset: 0x00486D90
		private void OnSubPageChanged(int newIndex, int oldIndex)
		{
			bool hasCustomSubPage = this._hasCustomSubPage;
			if (hasCustomSubPage)
			{
				bool flag = newIndex == this._customSubPageIndex;
				if (flag)
				{
					this._currentSubPage = (ESelectCharacterBaseSubPage)(-1);
				}
				else
				{
					this._currentSubPage = (ESelectCharacterBaseSubPage)((this._customSubPageIndex == 0 && this.subPageToggleGroup.GetAll().Count > ViewTaiwuVillageStoneRoom.BaseSubPageNameKeys.Count) ? (newIndex - 1) : newIndex);
				}
			}
			else
			{
				this._currentSubPage = (ESelectCharacterBaseSubPage)newIndex;
			}
			TabSortStateManager<int, ISelectCharacterData> tabSortStateManager = this._tabSortStateManager;
			if (tabSortStateManager != null)
			{
				tabSortStateManager.OnTabChange(newIndex);
			}
			this.RefreshList();
		}

		// Token: 0x06009ADD RID: 39645 RVA: 0x00488C20 File Offset: 0x00486E20
		private void InitSortAndFilter()
		{
			bool flag = this.sortAndFilter == null;
			if (!flag)
			{
				this._sortAndFilterController = new SelectCharacterSortAndFilterController(this.sortAndFilter, this._config.FilterMenuIds, null, false);
				this._sortAndFilterController.Init(new Action(this.OnSortAndFilterChanged), "SelectCharacter");
				this.scroll.SetSortController(this._sortAndFilterController);
				this._tabSortStateManager = new TabSortStateManager<int, ISelectCharacterData>(this._sortAndFilterController);
			}
		}

		// Token: 0x06009ADE RID: 39646 RVA: 0x00488C9E File Offset: 0x00486E9E
		private void OnSortAndFilterChanged()
		{
			this.RefreshListData();
		}

		// Token: 0x06009ADF RID: 39647 RVA: 0x00488CA8 File Offset: 0x00486EA8
		private void RefreshList()
		{
			this.RefreshListStructure();
			this.RefreshListData();
		}

		// Token: 0x06009AE0 RID: 39648 RVA: 0x00488CBC File Offset: 0x00486EBC
		private void RefreshListStructure()
		{
			IEnumerable<ColumnDefinition> columnDefinitions = this.GenerateColumnDefinitions();
			this.PrepareRowTemplateContainers();
			this.UpdateSelectedArea();
			this.scroll.ClearInfinityScrollCache();
			this.scroll.Init<ISelectCharacterData>(columnDefinitions, true, null, null);
			this._listStructureReady = true;
		}

		// Token: 0x06009AE1 RID: 39649 RVA: 0x00488D04 File Offset: 0x00486F04
		private void RefreshListData()
		{
			bool flag = !this._listStructureReady;
			if (flag)
			{
				this.RefreshListStructure();
			}
			List<ISelectCharacterData> searchFiltered = this.ApplySearch(this._dataList);
			SelectCharacterSortAndFilterController sortAndFilterController = this._sortAndFilterController;
			Func<ISelectCharacterData, bool> func;
			if ((func = ((sortAndFilterController != null) ? sortAndFilterController.GenerateFilter() : null)) == null && (func = ViewTaiwuVillageStoneRoom.<>c.<>9__60_0) == null)
			{
				func = (ViewTaiwuVillageStoneRoom.<>c.<>9__60_0 = ((ISelectCharacterData _) => true));
			}
			Func<ISelectCharacterData, bool> filter = func;
			IEnumerable<ISelectCharacterData> filtered = searchFiltered.Where(filter);
			this._filteredDataList.Clear();
			this._filteredDataList.AddRange(filtered);
			SelectCharacterSortAndFilterController sortAndFilterController2 = this._sortAndFilterController;
			Comparison<ISelectCharacterData> comparer = (sortAndFilterController2 != null) ? sortAndFilterController2.GenerateComparer(this._filteredDataList) : null;
			bool flag2 = comparer != null;
			if (flag2)
			{
				this._filteredDataList.Sort(comparer);
			}
			SelectCharacterSortAndFilterController sortAndFilterController3 = this._sortAndFilterController;
			if (sortAndFilterController3 != null)
			{
				sortAndFilterController3.AfterFilter(this._dataList);
			}
			this.scroll.SetData<ISelectCharacterData>(this._filteredDataList, this.GetSingleSelectedIndex());
			int displayAmount = Math.Max(8, this._filteredDataList.Count);
			this.flatModeScroll.SetDataCount(displayAmount);
		}

		// Token: 0x06009AE2 RID: 39650 RVA: 0x00488E14 File Offset: 0x00487014
		private List<ISelectCharacterData> ApplySearch(List<ISelectCharacterData> dataList)
		{
			bool flag = string.IsNullOrEmpty(this._searchText);
			List<ISelectCharacterData> result;
			if (flag)
			{
				result = dataList;
			}
			else
			{
				Func<ISelectCharacterData, string> extractor = this._config.SearchTextExtractor ?? new Func<ISelectCharacterData, string>(ViewTaiwuVillageStoneRoom.DefaultSearchTextExtractor);
				result = (from d in dataList
				where extractor(d).Contains(this._searchText)
				select d).ToList<ISelectCharacterData>();
			}
			return result;
		}

		// Token: 0x06009AE3 RID: 39651 RVA: 0x00488E80 File Offset: 0x00487080
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

		// Token: 0x06009AE4 RID: 39652 RVA: 0x00488EB6 File Offset: 0x004870B6
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitions()
		{
			yield return this.CreateAvatarWithNameColumn();
			bool flag = this._hasCustomSubPage && this._currentSubPage < ESelectCharacterBaseSubPage.State;
			if (flag)
			{
				bool flag2 = this._config.CustomColumnGenerator != null;
				if (flag2)
				{
					foreach (ColumnDefinition col in this._config.CustomColumnGenerator())
					{
						yield return col;
						col = null;
					}
					IEnumerator<ColumnDefinition> enumerator = null;
				}
				yield break;
			}
			switch (this._currentSubPage)
			{
			case ESelectCharacterBaseSubPage.State:
			{
				foreach (ColumnDefinition col2 in ViewTaiwuVillageStoneRoom.GenerateStateColumns())
				{
					yield return col2;
					col2 = null;
				}
				IEnumerator<ColumnDefinition> enumerator2 = null;
				break;
			}
			case ESelectCharacterBaseSubPage.Property:
			{
				foreach (ColumnDefinition col3 in ViewTaiwuVillageStoneRoom.GeneratePropertyColumns())
				{
					yield return col3;
					col3 = null;
				}
				IEnumerator<ColumnDefinition> enumerator3 = null;
				break;
			}
			case ESelectCharacterBaseSubPage.Property2:
			{
				foreach (ColumnDefinition col4 in ViewTaiwuVillageStoneRoom.GenerateProperty2Columns())
				{
					yield return col4;
					col4 = null;
				}
				IEnumerator<ColumnDefinition> enumerator4 = null;
				break;
			}
			case ESelectCharacterBaseSubPage.LifeSkill:
			{
				foreach (ColumnDefinition col5 in ViewTaiwuVillageStoneRoom.GenerateLifeSkillColumns())
				{
					yield return col5;
					col5 = null;
				}
				IEnumerator<ColumnDefinition> enumerator5 = null;
				break;
			}
			case ESelectCharacterBaseSubPage.CombatSkill:
			{
				foreach (ColumnDefinition col6 in ViewTaiwuVillageStoneRoom.GenerateCombatSkillColumns())
				{
					yield return col6;
					col6 = null;
				}
				IEnumerator<ColumnDefinition> enumerator6 = null;
				break;
			}
			case ESelectCharacterBaseSubPage.Personality:
			{
				foreach (ColumnDefinition col7 in ViewTaiwuVillageStoneRoom.GeneratePersonalityColumns())
				{
					yield return col7;
					col7 = null;
				}
				IEnumerator<ColumnDefinition> enumerator7 = null;
				break;
			}
			case ESelectCharacterBaseSubPage.Item:
			{
				foreach (ColumnDefinition col8 in ViewTaiwuVillageStoneRoom.GenerateItemColumns())
				{
					yield return col8;
					col8 = null;
				}
				IEnumerator<ColumnDefinition> enumerator8 = null;
				break;
			}
			case ESelectCharacterBaseSubPage.Command:
			{
				foreach (ColumnDefinition col9 in ViewTaiwuVillageStoneRoom.GenerateCommandColumns())
				{
					yield return col9;
					col9 = null;
				}
				IEnumerator<ColumnDefinition> enumerator9 = null;
				break;
			}
			}
			yield break;
			yield break;
		}

		// Token: 0x06009AE5 RID: 39653 RVA: 0x00488EC8 File Offset: 0x004870C8
		private ColumnDefinition CreateAvatarWithNameColumn()
		{
			ColumnDefinition<ISelectCharacterData, AvatarWithNameCellData> columnDefinition = new ColumnDefinition<ISelectCharacterData, AvatarWithNameCellData>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 330f,
				FlexibleWidth = 0f,
				PreferredWidth = 330f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Char_Name.Tr());
			columnDefinition.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				CharacterDisplayDataForGeneralScrollList generalData = data.GetGeneralScrollListData();
				return AvatarWithNameCellData.FromCharacterDisplayDataForGeneralScrollList(generalData, false, new Action<int>(this.OnCharacterAvatarClicked), null);
			};
			columnDefinition.SortId = 0;
			return columnDefinition;
		}

		// Token: 0x06009AE6 RID: 39654 RVA: 0x00488F58 File Offset: 0x00487158
		private void OnCharacterAvatarClicked(int characterId)
		{
			this.OpenCharacterMenu(characterId);
		}

		// Token: 0x06009AE7 RID: 39655 RVA: 0x00488F64 File Offset: 0x00487164
		private void OpenCharacterMenu(int charId)
		{
			UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", charId).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.CharacterBase, ECharacterSubPage.None)));
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x06009AE8 RID: 39656 RVA: 0x00488FB4 File Offset: 0x004871B4
		private static ColumnDefinition CreateTextColumn(Func<string> headerKey, Func<CharacterDisplayDataForGeneralScrollList, string> valueGetter, short sortId = -1, float minWidth = 30f, float preferredWidth = 90f)
		{
			return new ColumnDefinition<ISelectCharacterData, string>
			{
				LayoutOption = new LayoutOption
				{
					MinWidth = minWidth,
					FlexibleWidth = 1f,
					PreferredWidth = preferredWidth,
					Priority = 1
				},
				TableHeadLabel = headerKey,
				CellDataGenerator = ((ISelectCharacterData data) => valueGetter(data.GetGeneralScrollListData())),
				SortId = sortId
			};
		}

		// Token: 0x06009AE9 RID: 39657 RVA: 0x0048902C File Offset: 0x0048722C
		private static ColumnDefinition CreateIconAndTextColumn(Func<string> headerKey, Func<ISelectCharacterData, IconAndTextCellData> valueGetter, short sortId = -1, float minWidth = 30f, float preferredWidth = 90f)
		{
			return new ColumnDefinition<ISelectCharacterData, IconAndTextCellData>
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

		// Token: 0x06009AEA RID: 39658 RVA: 0x00489084 File Offset: 0x00487284
		private static IEnumerable<ColumnDefinition> GenerateStateColumns()
		{
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Char_Age.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.PhysiologicalAge.ToString(), 8, 236f, 236f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Health.Tr(), (CharacterDisplayDataForGeneralScrollList data) => CommonUtils.GetCharacterHealthInfo(data.Health, data.MaxLeftHealth, data.CharacterId).Item1, 10, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Charm.Tr(), (CharacterDisplayDataForGeneralScrollList data) => ViewTaiwuVillageStoneRoom.GetCharmDisplayString(data), 9, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Behavior.Tr(), (CharacterDisplayDataForGeneralScrollList data) => CommonUtils.GetBehaviorString(data.BehaviorType), 57, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Happiness.Tr(), (CharacterDisplayDataForGeneralScrollList data) => CommonUtils.GetHappinessString(HappinessType.GetHappinessType(data.Happiness)), 12, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Favorability.Tr(), (CharacterDisplayDataForGeneralScrollList data) => ViewTaiwuVillageStoneRoom.GetFavorDisplayString(data), 11, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Fame.Tr(), (CharacterDisplayDataForGeneralScrollList data) => CommonUtils.GetFameString(FameType.GetFameType(data.Fame)), 59, 30f, 90f);
			yield break;
		}

		// Token: 0x06009AEB RID: 39659 RVA: 0x00489090 File Offset: 0x00487290
		private static string GetCharmDisplayString(CharacterDisplayDataForGeneralScrollList data)
		{
			return CommonUtils.GetCharmLevelText(data.Charm, data.Gender, data.PhysiologicalAge, data.ClothDisplayId, CreatingType.IsFixedPresetType(data.CreatingType), data.FaceVisible);
		}

		// Token: 0x06009AEC RID: 39660 RVA: 0x004890D0 File Offset: 0x004872D0
		private static string GetFavorDisplayString(CharacterDisplayDataForGeneralScrollList data)
		{
			return CommonUtils.GetFavorStringByInteracted(data.FavorabilityToTaiwu, data.IsInteractedWithTaiwu);
		}

		// Token: 0x06009AED RID: 39661 RVA: 0x004890F3 File Offset: 0x004872F3
		private static IEnumerable<ColumnDefinition> GeneratePropertyColumns()
		{
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Strength.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.MaxMainAttributes[0].ToString(), 60, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Dexterity.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.MaxMainAttributes[1].ToString(), 61, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Concentration.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.MaxMainAttributes[2].ToString(), 62, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Vitality.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.MaxMainAttributes[3].ToString(), 63, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Energy.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.MaxMainAttributes[4].ToString(), 64, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Intelligence.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.MaxMainAttributes[5].ToString(), 65, 30f, 90f);
			yield break;
		}

		// Token: 0x06009AEE RID: 39662 RVA: 0x004890FC File Offset: 0x004872FC
		private static IEnumerable<ColumnDefinition> GenerateProperty2Columns()
		{
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_HitType_0.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.HitValues[0].ToString(), 24, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_HitType_1.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.HitValues[1].ToString(), 25, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_HitType_2.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.HitValues[2].ToString(), 26, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_HitType_3.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.HitValues[3].ToString(), 27, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_AvoidType_0.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.AvoidValues[0].ToString(), 33, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_AvoidType_1.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.AvoidValues[1].ToString(), 34, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_AvoidType_2.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.AvoidValues[2].ToString(), 35, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_AvoidType_3.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.AvoidValues[3].ToString(), 36, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Qi_Disorder.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.DisorderOfQi.ToString(), 55, 30f, 90f);
			yield break;
		}

		// Token: 0x06009AEF RID: 39663 RVA: 0x00489105 File Offset: 0x00487305
		private static IEnumerable<ColumnDefinition> GenerateLifeSkillColumns()
		{
			int num;
			for (int i = 0; i < 16; i = num + 1)
			{
				ViewTaiwuVillageStoneRoom.<>c__DisplayClass74_0 CS$<>8__locals1 = new ViewTaiwuVillageStoneRoom.<>c__DisplayClass74_0();
				CS$<>8__locals1.index = i;
				yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LocalStringManager.Get(string.Format("LK_LifeSkillType_{0}", CS$<>8__locals1.index)), (CharacterDisplayDataForGeneralScrollList data) => data.LifeSkillQualifications[CS$<>8__locals1.index].ToString(), (short)(66 + CS$<>8__locals1.index), 40f, 60f);
				CS$<>8__locals1 = null;
				num = i;
			}
			yield break;
		}

		// Token: 0x06009AF0 RID: 39664 RVA: 0x0048910E File Offset: 0x0048730E
		private static IEnumerable<ColumnDefinition> GenerateCombatSkillColumns()
		{
			int num;
			for (int i = 0; i < 14; i = num + 1)
			{
				ViewTaiwuVillageStoneRoom.<>c__DisplayClass75_0 CS$<>8__locals1 = new ViewTaiwuVillageStoneRoom.<>c__DisplayClass75_0();
				CS$<>8__locals1.index = i;
				yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LocalStringManager.Get(string.Format("LK_CombatSkillType_{0}", CS$<>8__locals1.index)), (CharacterDisplayDataForGeneralScrollList data) => data.CombatSkillQualifications[CS$<>8__locals1.index].ToString(), (short)(82 + CS$<>8__locals1.index), 40f, 60f);
				CS$<>8__locals1 = null;
				num = i;
			}
			yield break;
		}

		// Token: 0x06009AF1 RID: 39665 RVA: 0x00489117 File Offset: 0x00487317
		private static IEnumerable<ColumnDefinition> GeneratePersonalityColumns()
		{
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Personality_Calm_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[0].ToString(), 96, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Personality_Clever_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[1].ToString(), 97, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Personality_Enthusiastic_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[2].ToString(), 98, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Personality_Brave_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[3].ToString(), 99, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Personality_Firm_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[4].ToString(), 100, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Personality_Lucky_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[5].ToString(), 101, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Personality_Perceptive_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[6].ToString(), 102, 30f, 90f);
			yield break;
		}

		// Token: 0x06009AF2 RID: 39666 RVA: 0x00489120 File Offset: 0x00487320
		private static IEnumerable<ColumnDefinition> GenerateItemColumns()
		{
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Food.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Resources[0].ToString(), 103, 40f, 60f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Wood.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Resources[1].ToString(), 104, 40f, 60f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Metal.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Resources[2].ToString(), 105, 40f, 60f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Jade.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Resources[3].ToString(), 106, 40f, 60f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Fabric.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Resources[4].ToString(), 107, 40f, 60f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Herb.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Resources[5].ToString(), 108, 40f, 60f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Money.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Resources[6].ToString(), 109, 40f, 60f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Authority.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Resources[7].ToString(), 110, 40f, 60f);
			yield return ViewTaiwuVillageStoneRoom.CreateTextColumn(() => LanguageKey.LK_Inventory.Tr(), (CharacterDisplayDataForGeneralScrollList data) => string.Format("{0:f1}/{1:f1}", (float)data.CurrInventoryLoad / 100f, (float)data.MaxInventoryLoad / 100f), 37, 30f, 90f);
			yield break;
		}

		// Token: 0x06009AF3 RID: 39667 RVA: 0x00489129 File Offset: 0x00487329
		private static IEnumerable<ColumnDefinition> GenerateCommandColumns()
		{
			yield return ViewTaiwuVillageStoneRoom.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Attack.Tr(), (ISelectCharacterData data) => ViewTaiwuVillageStoneRoom.CreateMedalCellData(data.GetGeneralScrollListData().AttackMedal, 0), 112, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Defence.Tr(), (ISelectCharacterData data) => ViewTaiwuVillageStoneRoom.CreateMedalCellData(data.GetGeneralScrollListData().DefenceMedal, 1), 113, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Wisdom.Tr(), (ISelectCharacterData data) => ViewTaiwuVillageStoneRoom.CreateMedalCellData(data.GetGeneralScrollListData().WisdomMedal, 2), 114, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_0.Tr(), (ISelectCharacterData data) => ViewTaiwuVillageStoneRoom.CreateCommandCellData(data, 0), 115, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_1.Tr(), (ISelectCharacterData data) => ViewTaiwuVillageStoneRoom.CreateCommandCellData(data, 1), 116, 30f, 90f);
			yield return ViewTaiwuVillageStoneRoom.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_2.Tr(), (ISelectCharacterData data) => ViewTaiwuVillageStoneRoom.CreateCommandCellData(data, 2), 117, 30f, 90f);
			yield break;
		}

		// Token: 0x06009AF4 RID: 39668 RVA: 0x00489134 File Offset: 0x00487334
		private static IconAndTextCellData CreateMedalCellData(int medalCount, int medalType)
		{
			bool flag = medalCount == 0;
			IconAndTextCellData result;
			if (flag)
			{
				result = IconAndTextCellData.TextOnly("-");
			}
			else
			{
				string iconName = ViewTaiwuVillageStoneRoom.GetMedalIconName(medalCount, medalType);
				string text = string.Format(" x{0}", Mathf.Abs(medalCount));
				result = new IconAndTextCellData(iconName, text, true, false, false, false);
			}
			return result;
		}

		// Token: 0x06009AF5 RID: 39669 RVA: 0x00489188 File Offset: 0x00487388
		private static string GetMedalIconName(int medalCount, int medalType)
		{
			int signKey = (medalCount > 0) ? 1 : ((medalCount < 0) ? -1 : 0);
			if (!true)
			{
			}
			string text;
			switch (medalType)
			{
			case 0:
				text = MedalSummary.AttackMedalIconConfig[signKey];
				break;
			case 1:
				text = MedalSummary.DefenceMedalIconConfig[signKey];
				break;
			case 2:
				text = MedalSummary.WisdomMedalIconConfig[signKey];
				break;
			default:
				text = string.Empty;
				break;
			}
			if (!true)
			{
			}
			string iconNumber = text;
			return "ui9_icon_strategy_big_" + iconNumber;
		}

		// Token: 0x06009AF6 RID: 39670 RVA: 0x00489208 File Offset: 0x00487408
		private static IconAndTextCellData CreateCommandCellData(ISelectCharacterData data, int commandIndex)
		{
			CharacterDisplayDataForGeneralScrollList generalData = data.GetGeneralScrollListData();
			bool flag = generalData.Command.Items == null || !generalData.Command.Items.CheckIndex(commandIndex);
			IconAndTextCellData result;
			if (flag)
			{
				result = IconAndTextCellData.TextOnly("-");
			}
			else
			{
				sbyte commandId = generalData.Command.Items[commandIndex];
				bool flag2 = commandId < 0;
				if (flag2)
				{
					result = IconAndTextCellData.TextOnly("-");
				}
				else
				{
					TeammateCommandItem cmdConfig = Config.TeammateCommand.Instance[(int)commandId];
					result = IconAndTextCellData.TextOnly(cmdConfig.Name);
				}
			}
			return result;
		}

		// Token: 0x06009AF7 RID: 39671 RVA: 0x0048929C File Offset: 0x0048749C
		private void PrepareRowTemplateContainers()
		{
			int subPageKey = (int)this._currentSubPage;
			RowItem currentTemplate;
			bool flag = this._rowTemplateCache.TryGetValue(subPageKey, out currentTemplate);
			if (flag)
			{
				this.scroll.SetRowTemplate(currentTemplate);
			}
			else
			{
				currentTemplate = this.CreateRowTemplateForSubPage();
				this._rowTemplateCache[subPageKey] = currentTemplate;
				this.scroll.SetRowTemplate(currentTemplate);
			}
		}

		// Token: 0x06009AF8 RID: 39672 RVA: 0x004892F8 File Offset: 0x004874F8
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

		// Token: 0x06009AF9 RID: 39673 RVA: 0x00489404 File Offset: 0x00487604
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitionsForCurrentSubPage()
		{
			bool flag = this._hasCustomSubPage && this._currentSubPage < ESelectCharacterBaseSubPage.State;
			IEnumerable<ColumnDefinition> result;
			if (flag)
			{
				Func<IEnumerable<ColumnDefinition>> customColumnGenerator = this._config.CustomColumnGenerator;
				result = (((customColumnGenerator != null) ? customColumnGenerator() : null) ?? Enumerable.Empty<ColumnDefinition>());
			}
			else
			{
				ESelectCharacterBaseSubPage currentSubPage = this._currentSubPage;
				if (!true)
				{
				}
				IEnumerable<ColumnDefinition> enumerable;
				switch (currentSubPage)
				{
				case ESelectCharacterBaseSubPage.State:
					enumerable = ViewTaiwuVillageStoneRoom.GenerateStateColumns();
					break;
				case ESelectCharacterBaseSubPage.Property:
					enumerable = ViewTaiwuVillageStoneRoom.GeneratePropertyColumns();
					break;
				case ESelectCharacterBaseSubPage.Property2:
					enumerable = ViewTaiwuVillageStoneRoom.GenerateProperty2Columns();
					break;
				case ESelectCharacterBaseSubPage.LifeSkill:
					enumerable = ViewTaiwuVillageStoneRoom.GenerateLifeSkillColumns();
					break;
				case ESelectCharacterBaseSubPage.CombatSkill:
					enumerable = ViewTaiwuVillageStoneRoom.GenerateCombatSkillColumns();
					break;
				case ESelectCharacterBaseSubPage.Personality:
					enumerable = ViewTaiwuVillageStoneRoom.GeneratePersonalityColumns();
					break;
				case ESelectCharacterBaseSubPage.Item:
					enumerable = ViewTaiwuVillageStoneRoom.GenerateItemColumns();
					break;
				case ESelectCharacterBaseSubPage.Command:
					enumerable = ViewTaiwuVillageStoneRoom.GenerateCommandColumns();
					break;
				default:
					enumerable = Enumerable.Empty<ColumnDefinition>();
					break;
				}
				if (!true)
				{
				}
				result = enumerable;
			}
			return result;
		}

		// Token: 0x06009AFA RID: 39674 RVA: 0x004894D8 File Offset: 0x004876D8
		private int GetColumnCount()
		{
			bool flag = this._hasCustomSubPage && this._currentSubPage < ESelectCharacterBaseSubPage.State;
			int result;
			if (flag)
			{
				Func<IEnumerable<ColumnDefinition>> customColumnGenerator = this._config.CustomColumnGenerator;
				result = ((customColumnGenerator != null) ? customColumnGenerator().Count<ColumnDefinition>() : 0);
			}
			else
			{
				ESelectCharacterBaseSubPage currentSubPage = this._currentSubPage;
				if (!true)
				{
				}
				int num;
				switch (currentSubPage)
				{
				case ESelectCharacterBaseSubPage.State:
					num = 7;
					break;
				case ESelectCharacterBaseSubPage.Property:
					num = 6;
					break;
				case ESelectCharacterBaseSubPage.Property2:
					num = 9;
					break;
				case ESelectCharacterBaseSubPage.LifeSkill:
					num = 16;
					break;
				case ESelectCharacterBaseSubPage.CombatSkill:
					num = 14;
					break;
				case ESelectCharacterBaseSubPage.Personality:
					num = 7;
					break;
				case ESelectCharacterBaseSubPage.Item:
					num = 9;
					break;
				case ESelectCharacterBaseSubPage.Command:
					num = 6;
					break;
				default:
					num = 0;
					break;
				}
				if (!true)
				{
				}
				result = num;
			}
			return result;
		}

		// Token: 0x06009AFB RID: 39675 RVA: 0x00489584 File Offset: 0x00487784
		private void OnRowClicked(int index, RowItem rowItem)
		{
			bool flag = index < 0 || index >= this._filteredDataList.Count;
			if (!flag)
			{
				ISelectCharacterData data = this._filteredDataList[index];
				int charId = data.CharacterId;
				bool flag2 = this._bannedCharacterIds.Contains(charId);
				if (!flag2)
				{
					bool flag3 = this._config.SelectionMode == ESelectCharacterSelectionMode.Single;
					if (flag3)
					{
						bool flag4 = this._selectedCharId.Contains(charId);
						if (flag4)
						{
							bool flag5 = this._config.InteractionMode == ESelectCharacterInteractionMode.Slot;
							if (flag5)
							{
								this._selectedCharId.Remove(charId);
							}
						}
						else
						{
							this._selectedCharId.Clear();
							this._selectedCharId.Add(charId);
						}
					}
					else
					{
						bool flag6 = this._selectedCharId.Contains(charId);
						if (flag6)
						{
							this._selectedCharId.Remove(charId);
						}
						else
						{
							bool flag7 = this._selectedCharId.Count >= this._config.TargetCount;
							if (flag7)
							{
								return;
							}
							this._selectedCharId.Add(charId);
						}
					}
					this.UpdateDisplay();
				}
			}
		}

		// Token: 0x06009AFC RID: 39676 RVA: 0x004896A8 File Offset: 0x004878A8
		private bool IsRowSelected(int index, object rowData)
		{
			ISelectCharacterData data = rowData as ISelectCharacterData;
			bool flag = data == null;
			return !flag && this._selectedCharId.Contains(data.CharacterId);
		}

		// Token: 0x06009AFD RID: 39677 RVA: 0x004896E4 File Offset: 0x004878E4
		private int GetSingleSelectedIndex()
		{
			bool flag = this._config.SelectionMode > ESelectCharacterSelectionMode.Single;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				bool flag2 = this._selectedCharId.Count != 1;
				if (flag2)
				{
					result = -1;
				}
				else
				{
					int selectedId = -1;
					foreach (int item in this._selectedCharId)
					{
						selectedId = item;
					}
					for (int i = 0; i < this._filteredDataList.Count; i++)
					{
						bool flag3 = this._filteredDataList[i].CharacterId == selectedId;
						if (flag3)
						{
							return i;
						}
					}
					result = -1;
				}
			}
			return result;
		}

		// Token: 0x06009AFE RID: 39678 RVA: 0x004897B8 File Offset: 0x004879B8
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = btnName == "Close";
			if (flag)
			{
				this.QuickHide();
			}
			else
			{
				bool flag2 = btnName == "Confirm";
				if (flag2)
				{
					this.OnConfirmClicked();
				}
			}
		}

		// Token: 0x06009AFF RID: 39679 RVA: 0x004897FB File Offset: 0x004879FB
		private void UpdateSelectedArea()
		{
			this.UpdateInfoLabel();
		}

		// Token: 0x06009B00 RID: 39680 RVA: 0x00489805 File Offset: 0x00487A05
		private void OnSearchButtonClicked()
		{
			TMP_InputField tmp_InputField = this.searchInput;
			this._searchText = (((tmp_InputField != null) ? tmp_InputField.text : null) ?? string.Empty);
			this.RefreshListData();
		}

		// Token: 0x06009B01 RID: 39681 RVA: 0x00489830 File Offset: 0x00487A30
		private void OnSearchInputEndEdit(string text)
		{
			this._searchText = text;
			this.RefreshListData();
		}

		// Token: 0x06009B02 RID: 39682 RVA: 0x00489841 File Offset: 0x00487A41
		private void OnConfirmClicked()
		{
			this.QuickHide();
		}

		// Token: 0x06009B03 RID: 39683 RVA: 0x0048984C File Offset: 0x00487A4C
		private void UpdateInfoLabel()
		{
			this.txtSelectedAmount.SetText(LocalStringManager.GetFormat(LanguageKey.LK_Building_InfectedPeopleCount, this._selectedCharId.Count, this._config.TargetCount), true);
			this.txtTotalAmount.SetText(LocalStringManager.GetFormat(LanguageKey.LK_Building_InfectedPeopleCount_Total, this._dataList.Count, BuildingScale.DefValue.StoneRoomCapacity.GetLevelEffect((int)this._villageLevel)), true);
		}

		// Token: 0x06009B04 RID: 39684 RVA: 0x004898D0 File Offset: 0x00487AD0
		private bool IsRowDisabled(int index, object rowData)
		{
			ISelectCharacterData data = rowData as ISelectCharacterData;
			bool flag = data == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				int charId = data.CharacterId;
				bool flag2 = this._bannedCharacterIds != null && this._bannedCharacterIds.Contains(charId);
				if (flag2)
				{
					result = true;
				}
				else
				{
					bool flag3 = this._config.SelectionMode == ESelectCharacterSelectionMode.Multiple && this._selectedCharId.Count >= this._config.TargetCount;
					result = (flag3 && !this._selectedCharId.Contains(charId));
				}
			}
			return result;
		}

		// Token: 0x06009B05 RID: 39685 RVA: 0x00489966 File Offset: 0x00487B66
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 4, ulong.MaxValue, null));
		}

		// Token: 0x06009B06 RID: 39686 RVA: 0x00489980 File Offset: 0x00487B80
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			base.OnNotifyGameData(notifications);
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 0)
				{
					DataUid uid = notification.Uid;
					ushort dataId = uid.DataId;
					ushort num = dataId;
					if (num == 4)
					{
						List<int> stoneRoomCharList = new List<int>();
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref stoneRoomCharList);
						this.GetDisplayData(stoneRoomCharList);
						this.Element.ShowAfterRefresh();
					}
				}
			}
		}

		// Token: 0x06009B07 RID: 39687 RVA: 0x00489A3C File Offset: 0x00487C3C
		private void GetDisplayData(List<int> stoneRoomCharList)
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForGeneralScrollListBatch(null, stoneRoomCharList, delegate(int offset, RawDataPool pool)
			{
				List<CharacterDisplayDataForGeneralScrollList> displayData = new List<CharacterDisplayDataForGeneralScrollList>();
				Serializer.Deserialize(pool, offset, ref displayData);
				bool flag = displayData.Count > 0;
				if (flag)
				{
					GlobalDomainMethod.Call.InvokeGuidingTrigger(140);
				}
				List<ISelectCharacterData> selectList = new List<ISelectCharacterData>();
				bool flag2 = selectList != null;
				if (flag2)
				{
					foreach (CharacterDisplayDataForGeneralScrollList item in displayData)
					{
						selectList.Add(new BasicSelectCharacterDataAdapter(item));
					}
				}
				this._dataList.Clear();
				this._dataList.AddRange(selectList);
				this.UpdateDisplay();
			});
		}

		// Token: 0x06009B08 RID: 39688 RVA: 0x00489A53 File Offset: 0x00487C53
		private void UpdateDisplay()
		{
			this.RefreshListData();
			this.UpdateSelectedArea();
			this.UpdateInfoLabel();
			this.RefreshButtons();
		}

		// Token: 0x06009B09 RID: 39689 RVA: 0x00489A74 File Offset: 0x00487C74
		private void RefreshButtons()
		{
			this.btnSave.interactable = (this._selectedCharId.Count > 0);
			this.btnRelease.interactable = (this._selectedCharId.Count > 0);
			this.btnExpel.interactable = (this._selectedCharId.Count > 0);
			this.btnKill.interactable = (this._selectedCharId.Count > 0);
			this.signSelectAll.gameObject.SetActive(this._selectedCharId.Count == this._dataList.Count);
		}

		// Token: 0x040077C4 RID: 30660
		private static readonly List<LanguageKey> BaseSubPageNameKeys = new List<LanguageKey>
		{
			LanguageKey.LK_Team_Tog_State,
			LanguageKey.LK_Team_Tog_Property,
			LanguageKey.LK_Team_Tog_Property_Hit,
			LanguageKey.LK_Team_Tog_LifeSkill,
			LanguageKey.LK_Team_Tog_CombatSkill,
			LanguageKey.LK_Team_Tog_Personality,
			LanguageKey.LK_Team_Tog_Item,
			LanguageKey.LK_Team_Tog_Command
		};

		// Token: 0x040077C5 RID: 30661
		[SerializeField]
		private CToggleGroup subPageToggleGroup;

		// Token: 0x040077C6 RID: 30662
		[SerializeField]
		private ListStyleGeneralScroll scroll;

		// Token: 0x040077C7 RID: 30663
		[SerializeField]
		private SortAndFilter sortAndFilter;

		// Token: 0x040077C8 RID: 30664
		[Header("行模板配置")]
		[SerializeField]
		private RowItem rowTemplate;

		// Token: 0x040077C9 RID: 30665
		[SerializeField]
		private RowCellContainer avatarAndNameCellContainer;

		// Token: 0x040077CA RID: 30666
		[SerializeField]
		private RowCellContainer singleTextCellContainer;

		// Token: 0x040077CB RID: 30667
		[SerializeField]
		private RowCellContainer iconAndTextCellContainer;

		// Token: 0x040077CC RID: 30668
		[SerializeField]
		private TMP_InputField searchInput;

		// Token: 0x040077CD RID: 30669
		[SerializeField]
		private CButton searchButton;

		// Token: 0x040077CE RID: 30670
		[Header("已选列表配置")]
		[SerializeField]
		private TextMeshProUGUI txtSelectedAmount;

		// Token: 0x040077CF RID: 30671
		[SerializeField]
		private TextMeshProUGUI txtTotalAmount;

		// Token: 0x040077D0 RID: 30672
		[Header("平铺界面")]
		[SerializeField]
		private InfinityScroll flatModeScroll;

		// Token: 0x040077D1 RID: 30673
		[Header("交互")]
		[SerializeField]
		private CButton btnSave;

		// Token: 0x040077D2 RID: 30674
		[SerializeField]
		private CButton btnRelease;

		// Token: 0x040077D3 RID: 30675
		[SerializeField]
		private CButton btnExpel;

		// Token: 0x040077D4 RID: 30676
		[SerializeField]
		private CButton btnKill;

		// Token: 0x040077D5 RID: 30677
		[SerializeField]
		private CButton btnClose;

		// Token: 0x040077D6 RID: 30678
		[SerializeField]
		private CToggleGroup btnSwitchMode;

		// Token: 0x040077D7 RID: 30679
		[SerializeField]
		private CButton btnSelectAll;

		// Token: 0x040077D8 RID: 30680
		[SerializeField]
		private GameObject signSelectAll;

		// Token: 0x040077D9 RID: 30681
		private SelectCharacterConfig _config;

		// Token: 0x040077DA RID: 30682
		private readonly List<ISelectCharacterData> _dataList = new List<ISelectCharacterData>();

		// Token: 0x040077DB RID: 30683
		private readonly List<ISelectCharacterData> _filteredDataList = new List<ISelectCharacterData>();

		// Token: 0x040077DC RID: 30684
		private readonly HashSet<int> _bannedCharacterIds = new HashSet<int>();

		// Token: 0x040077DD RID: 30685
		private SelectCharacterSortAndFilterController _sortAndFilterController;

		// Token: 0x040077DE RID: 30686
		private TabSortStateManager<int, ISelectCharacterData> _tabSortStateManager;

		// Token: 0x040077DF RID: 30687
		private ESelectCharacterBaseSubPage _currentSubPage = ESelectCharacterBaseSubPage.State;

		// Token: 0x040077E0 RID: 30688
		private bool _hasCustomSubPage;

		// Token: 0x040077E1 RID: 30689
		private int _customSubPageIndex = -1;

		// Token: 0x040077E2 RID: 30690
		private bool _listStructureReady;

		// Token: 0x040077E3 RID: 30691
		private CToggle _dynamicToggle;

		// Token: 0x040077E4 RID: 30692
		private string _searchText = string.Empty;

		// Token: 0x040077E5 RID: 30693
		private readonly Dictionary<int, RowItem> _rowTemplateCache = new Dictionary<int, RowItem>();

		// Token: 0x040077E6 RID: 30694
		private bool _pendingInitialize;

		// Token: 0x040077E7 RID: 30695
		private bool _uiInitialized;

		// Token: 0x040077E8 RID: 30696
		private sbyte _villageLevel;

		// Token: 0x040077E9 RID: 30697
		private HashSet<int> _selectedCharId = new HashSet<int>();

		// Token: 0x040077EA RID: 30698
		private bool _listMode = true;

		// Token: 0x020022E8 RID: 8936
		public enum SaveInfectedType
		{
			// Token: 0x0400DCB2 RID: 56498
			Save = 1,
			// Token: 0x0400DCB3 RID: 56499
			Release,
			// Token: 0x0400DCB4 RID: 56500
			Expel,
			// Token: 0x0400DCB5 RID: 56501
			Kill
		}
	}
}
