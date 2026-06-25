using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.CellContent;
using Game.Components.SortAndFilter;
using Game.Components.Switch;
using Game.Views.CharacterMenu;
using Game.Views.Select;
using Game.Views.VillagerRoleView;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views
{
	// Token: 0x0200071F RID: 1823
	public class CharacterScrollMultipleMode : MonoBehaviour
	{
		// Token: 0x060056F2 RID: 22258 RVA: 0x0028673C File Offset: 0x0028493C
		public void SetData(List<VillagerCharDisplayData> dataList, short roleKey, bool interactable, bool clearSelected = false, bool rebuildListStructure = false)
		{
			short prevRoleKey = this._roleKey;
			this._dataInited = true;
			this._dataList = dataList;
			this._currentModeInteractable = interactable;
			this._roleKey = roleKey;
			if (clearSelected)
			{
				this._selectedCharId.Clear();
				this._selectedCharacterDatas.Clear();
				Action onSelectDataChange = this.OnSelectDataChange;
				if (onSelectDataChange != null)
				{
					onSelectDataChange();
				}
			}
			bool flag = this._listStructureReady && (rebuildListStructure || (prevRoleKey != roleKey && this._currentSubPage == EVillagerCharacterListSubPage.Role));
			if (flag)
			{
				this._listStructureReady = false;
			}
			this.UpdateDisplay();
		}

		// Token: 0x17000A81 RID: 2689
		// (get) Token: 0x060056F3 RID: 22259 RVA: 0x002867D5 File Offset: 0x002849D5
		public HashSet<int> SelectedCharId
		{
			get
			{
				return this._selectedCharId;
			}
		}

		// Token: 0x060056F4 RID: 22260 RVA: 0x002867E0 File Offset: 0x002849E0
		public void Init()
		{
			bool inited = this._inited;
			if (!inited)
			{
				this._inited = true;
				this._dataInited = false;
				SelectCharacterConfig selectCharacterConfig = new SelectCharacterConfig();
				selectCharacterConfig.SelectionMode = ESelectCharacterSelectionMode.Multiple;
				selectCharacterConfig.TargetCount = int.MaxValue;
				this._pendingInitialize = true;
				this._uiInitialized = false;
			}
		}

		// Token: 0x060056F5 RID: 22261 RVA: 0x00286830 File Offset: 0x00284A30
		private void Setup()
		{
			if (this._config == null)
			{
				this._config = new SelectCharacterConfig();
			}
			this.InitSubPageToggles();
			this.InitSortAndFilter();
		}

		// Token: 0x060056F6 RID: 22262 RVA: 0x00286854 File Offset: 0x00284A54
		private void Awake()
		{
			this.btnSelectAll.ClearAndAddListener(new Action(this.OnClickSelectAll));
			this.btnClearAll.ClearAndAddListener(new Action(this.OnClickClearAll));
			this.selRoot.SetActive(false);
			this.rowTemplate.gameObject.SetActive(false);
			this.avatarAndNameCellContainer.gameObject.SetActive(false);
			this.singleTextCellContainer.gameObject.SetActive(false);
			this.iconAndTextCellContainer.gameObject.SetActive(false);
			this.dropdownContainer.gameObject.SetActive(false);
			this.flatModeScroll.OnItemRender += this.FlatModeScroll_OnItemRender;
			this.selectedFlatModeScroll.OnItemRender += this.SelectedFlatModeScroll_OnItemRender;
			bool flag = this.scroll != null && this.rowTemplate != null;
			if (flag)
			{
				this.scroll.SetRowTemplate(this.rowTemplate);
				this.selectedScroll.SetRowTemplate(this.rowTemplate);
			}
			bool flag2 = this.scroll != null;
			if (flag2)
			{
				this.scroll.RowSelectedProvider = new Func<int, object, bool>(this.IsRowSelected);
				this.selectedScroll.RowSelectedProvider = new Func<int, object, bool>(this.IsRowSelected);
			}
			this.scroll.OnRowClicked += this.OnRowClicked;
			this.scroll.RowDisabledProvider = new Func<int, object, bool>(this.IsRowDisabled);
			this.selectedScroll.OnRowClicked += this.OnSelectedRowClicked;
			this.selectedScroll.RowDisabledProvider = new Func<int, object, bool>(this.IsRowDisabled);
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
		}

		// Token: 0x060056F7 RID: 22263 RVA: 0x00286A88 File Offset: 0x00284C88
		private void OnClickSelectAll()
		{
			foreach (VillagerCharDisplayData data in this._filteredDataList)
			{
				bool flag = !this._selectedCharacterDatas.Contains(data);
				if (flag)
				{
					this._selectedCharacterDatas.Add(data);
				}
			}
			this._selectedCharId = (from t in this._filteredDataList
			select t.CharacterId).ToHashSet<int>();
			Action onSelectDataChange = this.OnSelectDataChange;
			if (onSelectDataChange != null)
			{
				onSelectDataChange();
			}
			this.UpdateDisplay();
		}

		// Token: 0x060056F8 RID: 22264 RVA: 0x00286B4C File Offset: 0x00284D4C
		private void OnClickClearAll()
		{
			this._selectedCharacterDatas.Clear();
			this._selectedCharId.Clear();
			Action onSelectDataChange = this.OnSelectDataChange;
			if (onSelectDataChange != null)
			{
				onSelectDataChange();
			}
			this.UpdateDisplay();
		}

		// Token: 0x060056F9 RID: 22265 RVA: 0x00286B80 File Offset: 0x00284D80
		private void FlatModeScroll_OnItemRender(int index, GameObject itemObj)
		{
			RectTransform rect = itemObj.GetComponent<RectTransform>();
			bool flag = this._roleKey == 3;
			if (flag)
			{
				rect.sizeDelta = new Vector2(234f, 341f);
			}
			else
			{
				rect.sizeDelta = new Vector2(234f, 300f);
			}
			AssignPageVillagerView charView = itemObj.GetComponent<AssignPageVillagerView>();
			VillagerCharDisplayData data = this._filteredDataList[index];
			charView.Set(data, delegate
			{
				this.OnClickMain(index);
			}, delegate
			{
				this.OnClickRemove(index);
			}, delegate
			{
				this.OnChangeMerchant(index);
			}, this._selectedCharId.Contains(data.CharacterId), this._currentModeInteractable);
		}

		// Token: 0x060056FA RID: 22266 RVA: 0x00286C42 File Offset: 0x00284E42
		private void OnChangeMerchant(int index)
		{
			this.RefreshListData();
		}

		// Token: 0x060056FB RID: 22267 RVA: 0x00286C4C File Offset: 0x00284E4C
		private void SelectedFlatModeScroll_OnItemRender(int index, GameObject itemObj)
		{
			AssignPageVillagerView charView = itemObj.GetComponent<AssignPageVillagerView>();
			VillagerCharDisplayData data = this._selectedCharacterDatas[index];
			charView.Set(data, delegate
			{
				this.OnClickMainSelected(index);
			}, delegate
			{
				this.OnClickRemoveSelected(index);
			}, delegate
			{
				this.OnChangeMerchant(index);
			}, this._selectedCharId.Contains(data.CharacterId), this._currentModeInteractable);
		}

		// Token: 0x060056FC RID: 22268 RVA: 0x00286CCA File Offset: 0x00284ECA
		private void OnClickRemove(int index)
		{
			Action<int> onRemoveVillagerRole = this.OnRemoveVillagerRole;
			if (onRemoveVillagerRole != null)
			{
				onRemoveVillagerRole(this._filteredDataList[index].CharacterId);
			}
		}

		// Token: 0x060056FD RID: 22269 RVA: 0x00286CF0 File Offset: 0x00284EF0
		private void OnClickMainSelected(int index)
		{
			bool flag = index < 0 || index >= this._selectedCharacterDatas.Count;
			if (!flag)
			{
				VillagerCharDisplayData data = this._selectedCharacterDatas[index];
				int charId = data.CharacterId;
				bool flag2 = this._selectedCharId.Contains(charId);
				if (flag2)
				{
					this._selectedCharId.Remove(charId);
					this._selectedCharacterDatas.Remove(data);
				}
				Action onSelectDataChange = this.OnSelectDataChange;
				if (onSelectDataChange != null)
				{
					onSelectDataChange();
				}
				this.UpdateDisplay();
				this.RefreshStyle();
			}
		}

		// Token: 0x060056FE RID: 22270 RVA: 0x00286D7C File Offset: 0x00284F7C
		private void OnClickRemoveSelected(int index)
		{
			Action<int> onRemoveVillagerRole = this.OnRemoveVillagerRole;
			if (onRemoveVillagerRole != null)
			{
				onRemoveVillagerRole(this._selectedCharacterDatas[index].CharacterId);
			}
		}

		// Token: 0x060056FF RID: 22271 RVA: 0x00286DA4 File Offset: 0x00284FA4
		private void OnClickMain(int index)
		{
			bool flag = index < 0 || index >= this._filteredDataList.Count;
			if (!flag)
			{
				VillagerCharDisplayData data = this._filteredDataList[index];
				int charId = data.CharacterId;
				bool flag2 = this._bannedCharacterIds.Contains(charId);
				if (!flag2)
				{
					bool flag3 = this._selectedCharId.Contains(charId);
					if (flag3)
					{
						this._selectedCharId.Remove(charId);
						this._selectedCharacterDatas.Remove(data);
					}
					else
					{
						this._selectedCharId.Add(charId);
						this._selectedCharacterDatas.Add(data);
					}
					Action onSelectDataChange = this.OnSelectDataChange;
					if (onSelectDataChange != null)
					{
						onSelectDataChange();
					}
					this.UpdateDisplay();
					this.RefreshStyle();
				}
			}
		}

		// Token: 0x06005700 RID: 22272 RVA: 0x00286E68 File Offset: 0x00285068
		private void OnEnable()
		{
			this.searchInput.text = string.Empty;
			bool flag = this._pendingInitialize || !this._uiInitialized;
			if (flag)
			{
				this._hasCustomSubPage = false;
				this._customSubPageIndex = -1;
				this._currentShowStyle = -1;
				this._listStructureReady = false;
				this.Setup();
				this._pendingInitialize = false;
				this._uiInitialized = true;
			}
		}

		// Token: 0x06005701 RID: 22273 RVA: 0x00286ED4 File Offset: 0x002850D4
		private void OnDestroy()
		{
			this.scroll.OnRowClicked -= this.OnRowClicked;
			this.selectedScroll.OnRowClicked -= this.OnSelectedRowClicked;
			TabSortStateManager<int, VillagerCharDisplayData> tabSortStateManager = this._tabSortStateManager;
			if (tabSortStateManager != null)
			{
				tabSortStateManager.ClearAll();
			}
			this.flatModeScroll.OnItemRender -= this.FlatModeScroll_OnItemRender;
			this.selectedFlatModeScroll.OnItemRender -= this.SelectedFlatModeScroll_OnItemRender;
			this.switchToggleBig.onValueChanged.RemoveAllListeners();
		}

		// Token: 0x06005702 RID: 22274 RVA: 0x00286F68 File Offset: 0x00285168
		private void InitSubPageToggles()
		{
			bool flag = this.subPageToggleGroup == null;
			if (!flag)
			{
				this.switchToggleGroup.OnActiveIndexChange -= this.OnSwitchChanged;
				this.switchToggleBig.onValueChanged.RemoveAllListeners();
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
				this.switchToggleGroup.Init(-1);
				this.switchToggleGroup.OnActiveIndexChange += this.OnSwitchChanged;
				this.switchToggleBig.isOn = false;
				this.switchToggleBig.onValueChanged.AddListener(new UnityAction<bool>(this.OnChangedSwitchToggleBig));
				this.switchToggleGroup.Set(1, true);
				List<CToggle> toggles = this.subPageToggleGroup.GetAll();
				int baseCount = CharacterScrollMultipleMode.BaseSubPageNameKeys.Count;
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
							label.text = CharacterScrollMultipleMode.BaseSubPageNameKeys[i].Tr();
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
								toggleLabel.text = CharacterScrollMultipleMode.BaseSubPageNameKeys[j].Tr();
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

		// Token: 0x06005703 RID: 22275 RVA: 0x002873B0 File Offset: 0x002855B0
		private void OnSubPageChanged(int newIndex, int oldIndex)
		{
			bool hasCustomSubPage = this._hasCustomSubPage;
			if (hasCustomSubPage)
			{
				bool flag = newIndex == this._customSubPageIndex;
				if (flag)
				{
					this._currentSubPage = (EVillagerCharacterListSubPage)(-1);
				}
				else
				{
					this._currentSubPage = (EVillagerCharacterListSubPage)((this._customSubPageIndex == 0 && this.subPageToggleGroup.GetAll().Count > CharacterScrollMultipleMode.BaseSubPageNameKeys.Count) ? (newIndex - 1) : newIndex);
				}
			}
			else
			{
				this._currentSubPage = (EVillagerCharacterListSubPage)newIndex;
			}
			TabSortStateManager<int, VillagerCharDisplayData> tabSortStateManager = this._tabSortStateManager;
			if (tabSortStateManager != null)
			{
				tabSortStateManager.OnTabChange(newIndex);
			}
			this.RefreshList();
		}

		// Token: 0x06005704 RID: 22276 RVA: 0x00287440 File Offset: 0x00285640
		private void OnSwitchChanged(int newIndex, int oldIndex)
		{
			this._currentShowStyle = newIndex;
			this._listMode = !this._listMode;
			this.flatModeScroll.gameObject.SetActive(!this._listMode);
			this.scroll.gameObject.SetActive(this._listMode);
			this.subPageToggleGroup.gameObject.SetActive(this._listMode);
			this.RefreshStyle();
		}

		// Token: 0x06005705 RID: 22277 RVA: 0x002874B3 File Offset: 0x002856B3
		private void OnChangedSwitchToggleBig(bool isSelectedSwitch)
		{
			this.selRoot.SetActive(isSelectedSwitch);
			this.RefreshStyle();
		}

		// Token: 0x06005706 RID: 22278 RVA: 0x002874CC File Offset: 0x002856CC
		private void RefreshStyle()
		{
			bool flag = this._currentShowStyle == 0;
			if (flag)
			{
				bool flag2 = this.selectedScroll == null;
				if (flag2)
				{
					return;
				}
				this.selectedFlatModeScroll.gameObject.SetActive(false);
				this.selectedScroll.gameObject.SetActive(true);
				this.selectedScroll.SetData<VillagerCharDisplayData>(this._selectedCharacterDatas, this.GetSingleSelectedIndex());
			}
			bool flag3 = this._currentShowStyle == 1;
			if (flag3)
			{
				bool flag4 = this.selectedFlatModeScroll == null;
				if (!flag4)
				{
					this.selectedScroll.gameObject.SetActive(false);
					this.selectedFlatModeScroll.gameObject.SetActive(true);
					this.selectedFlatModeScroll.ClearCache();
					this.selectedFlatModeScroll.SetDataCount(this._selectedCharacterDatas.Count);
				}
			}
		}

		// Token: 0x06005707 RID: 22279 RVA: 0x002875A4 File Offset: 0x002857A4
		private void InitSortAndFilter()
		{
			bool flag = this.sortAndFilter == null;
			if (!flag)
			{
				this.sortAndFilter.ClearAllFilter();
				this._sortAndFilterController = new VillagerCharacterScrollController(this.sortAndFilter, new Func<int, bool>(this.IsTaiwu), new Func<int, bool>(this.IsSpecialTeammate));
				this.scroll.SetSortController(this._sortAndFilterController);
				this._sortAndFilterController.Init(new Action(this.OnSortAndFilterChanged), "SelectCharacter");
				this._tabSortStateManager = new TabSortStateManager<int, VillagerCharDisplayData>(this._sortAndFilterController);
				this._sortAndFilterController.AfterFilter(new List<VillagerCharDisplayData>());
			}
		}

		// Token: 0x06005708 RID: 22280 RVA: 0x00287650 File Offset: 0x00285850
		private bool IsTaiwu(int charId)
		{
			return false;
		}

		// Token: 0x06005709 RID: 22281 RVA: 0x00287664 File Offset: 0x00285864
		private bool IsSpecialTeammate(int charId)
		{
			VillagerCharDisplayData data = this._dataList.Find((VillagerCharDisplayData d) => d.CharacterId == charId);
			return data != null && data.IsSpecialGroupMember;
		}

		// Token: 0x0600570A RID: 22282 RVA: 0x002876A7 File Offset: 0x002858A7
		private void OnSortAndFilterChanged()
		{
			this._selectedCharId.Clear();
			this._selectedCharacterDatas.Clear();
			this.RefreshListData();
		}

		// Token: 0x0600570B RID: 22283 RVA: 0x002876C9 File Offset: 0x002858C9
		private void RefreshList()
		{
			this.RefreshListStructure();
			this.RefreshListData();
		}

		// Token: 0x0600570C RID: 22284 RVA: 0x002876DC File Offset: 0x002858DC
		private void RefreshListStructure()
		{
			bool flag = !this._dataInited;
			if (!flag)
			{
				IEnumerable<ColumnDefinition> columnDefinitions = this.GenerateColumnDefinitions();
				this.PrepareRowTemplateContainers();
				this.scroll.ClearInfinityScrollCache();
				this.scroll.Init<ISelectCharacterData>(columnDefinitions, true, null, null);
				this.selectedScroll.ClearInfinityScrollCache();
				this.selectedScroll.Init<ISelectCharacterData>(columnDefinitions, true, null, null);
				this._listStructureReady = true;
			}
		}

		// Token: 0x0600570D RID: 22285 RVA: 0x00287748 File Offset: 0x00285948
		private void RefreshListData()
		{
			bool flag = !this._listStructureReady;
			if (flag)
			{
				this.RefreshListStructure();
			}
			List<VillagerCharDisplayData> searchFiltered = this.ApplySearch(this._dataList);
			VillagerCharacterScrollController sortAndFilterController = this._sortAndFilterController;
			Func<VillagerCharDisplayData, bool> func;
			if ((func = ((sortAndFilterController != null) ? sortAndFilterController.GenerateFilter() : null)) == null && (func = CharacterScrollMultipleMode.<>c.<>9__75_0) == null)
			{
				func = (CharacterScrollMultipleMode.<>c.<>9__75_0 = ((VillagerCharDisplayData _) => true));
			}
			Func<VillagerCharDisplayData, bool> filter = func;
			IEnumerable<VillagerCharDisplayData> filtered = searchFiltered.Where(filter);
			this._filteredDataList.Clear();
			this._filteredDataList.AddRange(filtered);
			VillagerCharacterScrollController sortAndFilterController2 = this._sortAndFilterController;
			Comparison<VillagerCharDisplayData> comparer = (sortAndFilterController2 != null) ? sortAndFilterController2.GenerateComparer(this._filteredDataList) : null;
			bool flag2 = comparer != null;
			if (flag2)
			{
				this._filteredDataList.Sort(comparer);
			}
			bool flag3 = this._roleKey == 3;
			if (flag3)
			{
				this.flatModeScroll.gap = new Vector2(35f, 20f);
			}
			else
			{
				this.flatModeScroll.gap = new Vector2(35f, -15f);
			}
			this.flatModeScroll.ClearCache();
			this.scroll.SetData<VillagerCharDisplayData>(this._filteredDataList, this.GetSingleSelectedIndex());
			this.flatModeScroll.SetDataCount(this._filteredDataList.Count);
			this.RefreshSelectTxtButton();
			VillagerCharacterScrollController sortAndFilterController3 = this._sortAndFilterController;
			if (sortAndFilterController3 != null)
			{
				sortAndFilterController3.AfterFilter(this._dataList);
			}
		}

		// Token: 0x0600570E RID: 22286 RVA: 0x002878A8 File Offset: 0x00285AA8
		private void RefreshSelectTxtButton()
		{
			this.txtSelectedAmount.text = LocalStringManager.GetFormat(LanguageKey.UI_CheckInspcription_InlcudeCount, this._selectedCharId.Count, this._filteredDataList.Count);
			this.checkmarkSelectAll.gameObject.SetActive(this._selectedCharId.Count != 0 && this._filteredDataList.Count == this._selectedCharId.Count);
		}

		// Token: 0x0600570F RID: 22287 RVA: 0x00287928 File Offset: 0x00285B28
		private List<VillagerCharDisplayData> ApplySearch(List<VillagerCharDisplayData> dataList)
		{
			bool flag = string.IsNullOrEmpty(this._searchText);
			List<VillagerCharDisplayData> result;
			if (flag)
			{
				result = dataList;
			}
			else
			{
				result = (from d in dataList
				where CharacterScrollMultipleMode.DefaultSearchTextExtractor(d).Contains(this._searchText)
				select d).ToList<VillagerCharDisplayData>();
			}
			return result;
		}

		// Token: 0x06005710 RID: 22288 RVA: 0x00287964 File Offset: 0x00285B64
		private static string DefaultSearchTextExtractor(VillagerCharDisplayData data)
		{
			return NameCenter.GetMonasticTitleOrDisplayName(ref data.NameData, false, false);
		}

		// Token: 0x06005711 RID: 22289 RVA: 0x00287983 File Offset: 0x00285B83
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitions()
		{
			yield return this.CreateAvatarWithNameColumn();
			bool flag = this._hasCustomSubPage && this._currentSubPage < EVillagerCharacterListSubPage.Role;
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
			case EVillagerCharacterListSubPage.Role:
			{
				foreach (ColumnDefinition col2 in this.GenerateRoleColumns())
				{
					yield return col2;
					col2 = null;
				}
				IEnumerator<ColumnDefinition> enumerator2 = null;
				break;
			}
			case EVillagerCharacterListSubPage.Villager:
			{
				foreach (ColumnDefinition col3 in this.GenerateVillagerColumns())
				{
					yield return col3;
					col3 = null;
				}
				IEnumerator<ColumnDefinition> enumerator3 = null;
				break;
			}
			case EVillagerCharacterListSubPage.State:
			{
				foreach (ColumnDefinition col4 in CharacterScrollMultipleMode.GenerateStateColumns())
				{
					yield return col4;
					col4 = null;
				}
				IEnumerator<ColumnDefinition> enumerator4 = null;
				break;
			}
			case EVillagerCharacterListSubPage.Property:
			{
				foreach (ColumnDefinition col5 in CharacterScrollMultipleMode.GeneratePropertyColumns())
				{
					yield return col5;
					col5 = null;
				}
				IEnumerator<ColumnDefinition> enumerator5 = null;
				break;
			}
			case EVillagerCharacterListSubPage.Property2:
			{
				foreach (ColumnDefinition col6 in CharacterScrollMultipleMode.GenerateProperty2Columns())
				{
					yield return col6;
					col6 = null;
				}
				IEnumerator<ColumnDefinition> enumerator6 = null;
				break;
			}
			case EVillagerCharacterListSubPage.LifeSkill:
			{
				foreach (ColumnDefinition col7 in CharacterScrollMultipleMode.GenerateLifeSkillColumns())
				{
					yield return col7;
					col7 = null;
				}
				IEnumerator<ColumnDefinition> enumerator7 = null;
				break;
			}
			case EVillagerCharacterListSubPage.CombatSkill:
			{
				foreach (ColumnDefinition col8 in CharacterScrollMultipleMode.GenerateCombatSkillColumns())
				{
					yield return col8;
					col8 = null;
				}
				IEnumerator<ColumnDefinition> enumerator8 = null;
				break;
			}
			case EVillagerCharacterListSubPage.Personality:
			{
				foreach (ColumnDefinition col9 in CharacterScrollMultipleMode.GeneratePersonalityColumns())
				{
					yield return col9;
					col9 = null;
				}
				IEnumerator<ColumnDefinition> enumerator9 = null;
				break;
			}
			case EVillagerCharacterListSubPage.Item:
			{
				foreach (ColumnDefinition col10 in CharacterScrollMultipleMode.GenerateItemColumns())
				{
					yield return col10;
					col10 = null;
				}
				IEnumerator<ColumnDefinition> enumerator10 = null;
				break;
			}
			case EVillagerCharacterListSubPage.Command:
			{
				foreach (ColumnDefinition col11 in CharacterScrollMultipleMode.GenerateCommandColumns())
				{
					yield return col11;
					col11 = null;
				}
				IEnumerator<ColumnDefinition> enumerator11 = null;
				break;
			}
			}
			yield break;
			yield break;
		}

		// Token: 0x06005712 RID: 22290 RVA: 0x00287994 File Offset: 0x00285B94
		private ColumnDefinition CreateAvatarWithNameColumn()
		{
			ColumnDefinition<VillagerCharDisplayData, AvatarWithNameCellData> columnDefinition = new ColumnDefinition<VillagerCharDisplayData, AvatarWithNameCellData>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 330f,
				FlexibleWidth = 0f,
				PreferredWidth = 330f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Char_Name.Tr());
			columnDefinition.CellDataGenerator = ((VillagerCharDisplayData data) => AvatarWithNameCellData.FromVillagerDisplayData(data, false, new Action<int>(this.OpenCharacterMenu), null));
			columnDefinition.SortId = 0;
			return columnDefinition;
		}

		// Token: 0x06005713 RID: 22291 RVA: 0x00287A24 File Offset: 0x00285C24
		private void OpenCharacterMenu(int charId)
		{
			UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", charId).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.CharacterBase, ECharacterSubPage.None)));
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x06005714 RID: 22292 RVA: 0x00287A74 File Offset: 0x00285C74
		private static ColumnDefinition CreateIconAndTextColumn(Func<string> headerKey, Func<VillagerCharDisplayData, IconAndTextCellData> valueGetter, short sortId = -1, float minWidth = 30f, float preferredWidth = 90f)
		{
			return new ColumnDefinition<VillagerCharDisplayData, IconAndTextCellData>
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

		// Token: 0x06005715 RID: 22293 RVA: 0x00287ACC File Offset: 0x00285CCC
		private IEnumerable<ColumnDefinition> GenerateRoleColumns()
		{
			bool flag = this._roleKey == 3;
			if (flag)
			{
				yield return this.CreateMerchantColumn(() => LanguageKey.LK_Merchant.Tr(), -1, 300f, 300f);
			}
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Behavior.Tr(), (VillagerCharDisplayData data) => CommonUtils.GetBehaviorString(data.BehaviorType), 57, 30f, 330f);
			yield return CharacterScrollMultipleMode.CreateIconAndTextColumn(() => LanguageKey.LK_Personality_Short.Tr(), (VillagerCharDisplayData data) => CharacterScrollMultipleMode.CreatePersonalityCellData(this._roleKey, data), -1, 30f, 330f);
			yield return CharacterScrollMultipleMode.CreateIconAndTextColumn(() => LanguageKey.LK_VillagerRole_Lineage_LifeSkill_Require_TableHead.Tr() + "1", (VillagerCharDisplayData data) => CharacterScrollMultipleMode.CreateSkillCellData(this._roleKey, data, 0), -1, 30f, 330f);
			short roleKey = this._roleKey;
			bool flag2 = roleKey == 1 || roleKey == 2 || roleKey == 4 || roleKey == 5;
			if (flag2)
			{
				yield return CharacterScrollMultipleMode.CreateIconAndTextColumn(() => LanguageKey.LK_VillagerRole_Lineage_LifeSkill_Require_TableHead.Tr() + "2", (VillagerCharDisplayData data) => CharacterScrollMultipleMode.CreateSkillCellData(this._roleKey, data, 1), -1, 30f, 330f);
			}
			roleKey = this._roleKey;
			bool flag3 = roleKey == 1 || roleKey == 4 || roleKey == 5;
			if (flag3)
			{
				yield return CharacterScrollMultipleMode.CreateIconAndTextColumn(() => LanguageKey.LK_VillagerRole_Lineage_LifeSkill_Require_TableHead.Tr() + "3", (VillagerCharDisplayData data) => CharacterScrollMultipleMode.CreateSkillCellData(this._roleKey, data, 2), -1, 30f, 330f);
			}
			roleKey = this._roleKey;
			bool flag4 = roleKey == 1 || roleKey == 4;
			if (flag4)
			{
				yield return CharacterScrollMultipleMode.CreateIconAndTextColumn(() => LanguageKey.LK_VillagerRole_Lineage_LifeSkill_Require_TableHead.Tr() + "4", (VillagerCharDisplayData data) => CharacterScrollMultipleMode.CreateSkillCellData(this._roleKey, data, 3), -1, 30f, 330f);
			}
			bool flag5 = this._roleKey == 4;
			if (flag5)
			{
				yield return CharacterScrollMultipleMode.CreateIconAndTextColumn(() => LanguageKey.LK_VillagerRole_Lineage_LifeSkill_Require_TableHead.Tr() + "5", (VillagerCharDisplayData data) => CharacterScrollMultipleMode.CreateSkillCellData(this._roleKey, data, 4), -1, 30f, 330f);
			}
			yield break;
		}

		// Token: 0x06005716 RID: 22294 RVA: 0x00287ADC File Offset: 0x00285CDC
		private static IconAndTextCellData CreateSkillCellData(short roleKey, VillagerCharDisplayData data, int propIndex)
		{
			bool flag = data == null || roleKey < 0;
			IconAndTextCellData result;
			if (flag)
			{
				result = IconAndTextCellData.TextOnly("-");
			}
			else
			{
				ValueTuple<int, short, bool> villagerRoleAttainmentTypeAndValue = CharacterScrollMultipleMode.GetVillagerRoleAttainmentTypeAndValue(roleKey, data, propIndex);
				int skillType = villagerRoleAttainmentTypeAndValue.Item1;
				short skillValue = villagerRoleAttainmentTypeAndValue.Item2;
				bool isLifeSkill = villagerRoleAttainmentTypeAndValue.Item3;
				bool flag2 = skillValue < 0;
				if (flag2)
				{
					result = IconAndTextCellData.TextOnly("-");
				}
				else
				{
					string iconName = (isLifeSkill ? "ui9_back_attainments_life_0_" : "ui9_back_attainments_combat_0_") + skillType.ToString();
					string text = skillValue.ToString();
					result = new IconAndTextCellData(iconName, text, true, false, false, false);
				}
			}
			return result;
		}

		// Token: 0x06005717 RID: 22295 RVA: 0x00287B78 File Offset: 0x00285D78
		[return: TupleElementNames(new string[]
		{
			"skillType",
			"value",
			"isLifeSkill"
		})]
		public unsafe static ValueTuple<int, short, bool> GetVillagerRoleAttainmentTypeAndValue(short roleTemplateId, VillagerCharDisplayData data, int indexProp)
		{
			short value = -1;
			bool flag = roleTemplateId == 0;
			ValueTuple<int, short, bool> result;
			if (flag)
			{
				bool flag2 = indexProp == 0;
				if (flag2)
				{
					value = *data.LifeSkillAttainments[14];
				}
				result = new ValueTuple<int, short, bool>(14, value, true);
			}
			else
			{
				bool flag3 = roleTemplateId == 1;
				if (flag3)
				{
					int lifeSkillType = 6;
					switch (indexProp)
					{
					case 1:
						lifeSkillType = 7;
						break;
					case 2:
						lifeSkillType = 10;
						break;
					case 3:
						lifeSkillType = 11;
						break;
					}
					value = *data.LifeSkillAttainments[lifeSkillType];
					result = new ValueTuple<int, short, bool>(lifeSkillType, value, true);
				}
				else
				{
					bool flag4 = roleTemplateId == 2;
					if (flag4)
					{
						sbyte lifeSkillType2 = 0;
						bool flag5 = indexProp == 0;
						if (flag5)
						{
							lifeSkillType2 = 8;
							value = *data.LifeSkillAttainments[(int)lifeSkillType2];
						}
						else
						{
							bool flag6 = indexProp == 1;
							if (flag6)
							{
								lifeSkillType2 = 9;
								value = *data.LifeSkillAttainments[(int)lifeSkillType2];
							}
							else
							{
								value = -1;
							}
						}
						result = new ValueTuple<int, short, bool>((int)lifeSkillType2, value, true);
					}
					else
					{
						bool flag7 = roleTemplateId == 3;
						if (flag7)
						{
							bool flag8 = indexProp == 0;
							if (flag8)
							{
								value = *data.LifeSkillAttainments[15];
							}
							result = new ValueTuple<int, short, bool>(15, value, true);
						}
						else
						{
							bool flag9 = roleTemplateId == 4;
							if (flag9)
							{
								int lifeSkillType3 = (indexProp == 4) ? 5 : indexProp;
								value = *data.LifeSkillAttainments[lifeSkillType3];
								result = new ValueTuple<int, short, bool>(lifeSkillType3, value, true);
							}
							else
							{
								bool flag10 = roleTemplateId == 5;
								if (flag10)
								{
									bool flag11 = indexProp == 0;
									if (flag11)
									{
										value = *data.LifeSkillAttainments[12];
										result = new ValueTuple<int, short, bool>(12, value, true);
									}
									else
									{
										bool flag12 = indexProp == 1;
										if (flag12)
										{
											value = *data.LifeSkillAttainments[13];
											result = new ValueTuple<int, short, bool>(13, value, true);
										}
										else
										{
											sbyte combatSkillType = 0;
											bool flag13 = indexProp == 2;
											if (flag13)
											{
												for (sbyte i = 0; i < 14; i += 1)
												{
													bool flag14 = *data.CombatSkillAttainments[(int)i] > value;
													if (flag14)
													{
														combatSkillType = i;
														value = *data.CombatSkillAttainments[(int)i];
													}
												}
											}
											result = new ValueTuple<int, short, bool>((int)combatSkillType, value, false);
										}
									}
								}
								else
								{
									bool flag15 = indexProp == 0;
									if (flag15)
									{
										value = *data.LifeSkillAttainments[4];
									}
									result = new ValueTuple<int, short, bool>(4, value, true);
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06005718 RID: 22296 RVA: 0x00287DC0 File Offset: 0x00285FC0
		private static IconAndTextCellData CreatePersonalityCellData(short roleKey, VillagerCharDisplayData data)
		{
			bool flag = data == null || roleKey < 0;
			IconAndTextCellData result;
			if (flag)
			{
				result = IconAndTextCellData.TextOnly("-");
			}
			else
			{
				ValueTuple<sbyte, short> villagerRolePersonalityTypeAndValue = ViewVillagerRole.GetVillagerRolePersonalityTypeAndValue(roleKey, data);
				sbyte personalityType = villagerRolePersonalityTypeAndValue.Item1;
				short value = villagerRolePersonalityTypeAndValue.Item2;
				string iconName = "ui9_icon_personality_big_" + personalityType.ToString();
				string text = value.ToString();
				result = new IconAndTextCellData(iconName, text, true, false, false, false);
			}
			return result;
		}

		// Token: 0x06005719 RID: 22297 RVA: 0x00287E2C File Offset: 0x0028602C
		private ColumnDefinition CreateMerchantColumn(Func<string> headerKey, short sortId = -1, float minWidth = 300f, float preferredWidth = 300f)
		{
			return new ColumnDefinition<VillagerCharDisplayData, MerchantDropdownCellData>
			{
				LayoutOption = new LayoutOption
				{
					MinWidth = minWidth,
					FlexibleWidth = 1f,
					PreferredWidth = preferredWidth,
					Priority = 1
				},
				TableHeadLabel = headerKey,
				CellDataGenerator = ((VillagerCharDisplayData data) => new MerchantDropdownCellData(new Action<int, int>(this.OnChangeMerchantType), data.CharacterId)),
				SortId = sortId
			};
		}

		// Token: 0x0600571A RID: 22298 RVA: 0x00287E98 File Offset: 0x00286098
		private void OnChangeMerchantType(int index, int charId)
		{
			sbyte setValue = (sbyte)((index == 0) ? 7 : (index - 1));
			TaiwuDomainMethod.Call.SetMerchantType(charId, setValue);
			this.RefreshListData();
		}

		// Token: 0x0600571B RID: 22299 RVA: 0x00287EC0 File Offset: 0x002860C0
		private IEnumerable<ColumnDefinition> GenerateVillagerColumns()
		{
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Char_Age.Tr(), (VillagerCharDisplayData data) => (data.PhysiologicalAge >= 0) ? data.PhysiologicalAge.ToString() : LanguageKey.LK_Unknow.Tr(), 8, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_VillagerInfo_Role.Tr(), (VillagerCharDisplayData data) => CommonUtils.GetOrganizationGradeString(data.OrgInfo, data.Gender, data.PhysiologicalAge, (int)data.CharacterTemplateId), 1, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_VillagerInfo_Working_Status.Tr(), delegate(VillagerCharDisplayData data)
			{
				int num = VillagerSortController.WorkingStatusOrder(data);
				if (!true)
				{
				}
				string result;
				switch (num)
				{
				case 0:
					result = LanguageKey.LK_VillagerInfo_Idle.Tr();
					break;
				case 1:
					result = LanguageKey.LK_VillagerInfo_KeepGrave.Tr();
					break;
				case 2:
					result = LanguageKey.LK_VillagerInfo_ShopManage.Tr();
					break;
				case 3:
					result = LanguageKey.LK_VillagerInfo_Job.Tr();
					break;
				default:
					result = LanguageKey.LK_VillagerInfo_None.Tr();
					break;
				}
				if (!true)
				{
				}
				return result;
			}, 129, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_VillagerInfo_Working_Location.Tr(), delegate(VillagerCharDisplayData data)
			{
				VillagerWorkData workData = data.VillagerWorkData;
				return (workData != null && workData.BlockTemplateId != -1) ? BuildingBlock.Instance[workData.BuildingBlockIndex].Name : LanguageKey.LK_VillagerInfo_None.Tr();
			}, 126, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_VillagerInfo_Working_Role.Tr(), delegate(VillagerCharDisplayData data)
			{
				int num = VillagerSortController.WorkingRoleOrder(data);
				if (!true)
				{
				}
				string result;
				if (num != 0)
				{
					if (num != 1)
					{
						result = LanguageKey.LK_VillagerInfo_None.Tr();
					}
					else
					{
						result = LanguageKey.LK_VillagerInfo_Mentor.Tr();
					}
				}
				else
				{
					result = LanguageKey.LK_VillagerInfo_Mentee.Tr();
				}
				if (!true)
				{
				}
				return result;
			}, 127, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_VillagerInfo_Location.Tr(), (VillagerCharDisplayData data) => CommonUtils.GetRelativeLocationName(data.LocationNameRelatedData), -1, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_VillagerInfo_Potential.Tr(), (VillagerCharDisplayData data) => data.Potential.ToString(), 128, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_VillagerInfo_Happiness.Tr(), (VillagerCharDisplayData data) => CommonUtils.GetHappinessString(HappinessType.GetHappinessType(data.Happiness)), 12, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_VillagerInfo_DefeatMark.Tr(), (VillagerCharDisplayData data) => data.DefeatMarkCount.ToString(), 53, 30f, 90f);
			yield break;
		}

		// Token: 0x0600571C RID: 22300 RVA: 0x00287ED0 File Offset: 0x002860D0
		private static ColumnDefinition CreateTextColumn(Func<string> headerKey, Func<VillagerCharDisplayData, string> valueGetter, short sortId = -1, float minWidth = 30f, float preferredWidth = 90f)
		{
			return new ColumnDefinition<VillagerCharDisplayData, string>
			{
				LayoutOption = new LayoutOption
				{
					MinWidth = minWidth,
					FlexibleWidth = 1f,
					PreferredWidth = preferredWidth,
					Priority = 1
				},
				TableHeadLabel = headerKey,
				CellDataGenerator = valueGetter,
				SortId = sortId
			};
		}

		// Token: 0x0600571D RID: 22301 RVA: 0x00287F30 File Offset: 0x00286130
		private ColumnDefinition CreateLocationColumn(Func<string> headerKey, short sortId = -1, float minWidth = 300f, float preferredWidth = 300f)
		{
			WorldMapModel model = SingletonObject.getInstance<WorldMapModel>();
			ColumnDefinition<VillagerCharDisplayData, SingleButtonCellData> columnDefinition = new ColumnDefinition<VillagerCharDisplayData, SingleButtonCellData>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = minWidth,
				FlexibleWidth = 1f,
				PreferredWidth = preferredWidth,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = headerKey;
			columnDefinition.CellDataGenerator = ((VillagerCharDisplayData data) => new SingleButtonCellData
			{
				LabelText = CommonUtils.GetRelativeLocationName(data.LocationNameRelatedData),
				OnClick = null
			});
			columnDefinition.SortId = sortId;
			return columnDefinition;
		}

		// Token: 0x0600571E RID: 22302 RVA: 0x00287FB4 File Offset: 0x002861B4
		private static IEnumerable<ColumnDefinition> GenerateStateColumns()
		{
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Char_Age.Tr(), (VillagerCharDisplayData data) => data.PhysiologicalAge.ToString(), 8, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Health.Tr(), (VillagerCharDisplayData data) => CommonUtils.GetCharacterHealthInfo(data.Health, data.MaxLeftHealth, data.CharacterId).Item1, 10, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Injury.Tr(), (VillagerCharDisplayData data) => data.DefeatMarkCount.ToString(), 53, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Charm.Tr(), (VillagerCharDisplayData data) => CharacterScrollMultipleMode.GetCharmDisplayString(data), 9, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Behavior.Tr(), (VillagerCharDisplayData data) => CommonUtils.GetBehaviorString(data.BehaviorType), 57, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Happiness.Tr(), (VillagerCharDisplayData data) => CommonUtils.GetHappinessString(HappinessType.GetHappinessType(data.Happiness)), 12, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Favorability.Tr(), new Func<VillagerCharDisplayData, string>(CharacterScrollMultipleMode.GetFavorDisplayString), 11, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Alertness.Tr(), (VillagerCharDisplayData data) => CommonUtils.GetAlertnessNameByValue(data.Alertness), 130, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Samsara.Tr(), (VillagerCharDisplayData data) => data.PreexistenceCharCount.ToString(), 58, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Fame.Tr(), (VillagerCharDisplayData data) => CommonUtils.GetFameString(FameType.GetFameType(data.Fame)), 59, 30f, 90f);
			yield break;
		}

		// Token: 0x0600571F RID: 22303 RVA: 0x00287FC0 File Offset: 0x002861C0
		private static string GetCharmDisplayString(VillagerCharDisplayData data)
		{
			return CommonUtils.GetCharmLevelText(data.Charm, data.Gender, data.PhysiologicalAge, data.ClothDisplayId, CreatingType.IsFixedPresetType(data.CreatingType), data.FaceVisible);
		}

		// Token: 0x06005720 RID: 22304 RVA: 0x00288000 File Offset: 0x00286200
		private static string GetFavorDisplayString(CharacterDisplayDataForGeneralScrollList data)
		{
			return CommonUtils.GetFavorStringByInteracted(data.FavorabilityToTaiwu, data.IsInteractedWithTaiwu);
		}

		// Token: 0x06005721 RID: 22305 RVA: 0x00288024 File Offset: 0x00286224
		private static string GetFavorDisplayString(VillagerCharDisplayData data)
		{
			return CommonUtils.GetFavorStringByInteracted(data.FavorabilityToTaiwu, data.IsInteractedWithTaiwu);
		}

		// Token: 0x06005722 RID: 22306 RVA: 0x00288047 File Offset: 0x00286247
		private static IEnumerable<ColumnDefinition> GeneratePropertyColumns()
		{
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Strength.Tr(), (VillagerCharDisplayData data) => data.MaxMainAttributes[0].ToString(), 60, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Dexterity.Tr(), (VillagerCharDisplayData data) => data.MaxMainAttributes[1].ToString(), 61, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Concentration.Tr(), (VillagerCharDisplayData data) => data.MaxMainAttributes[2].ToString(), 62, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Vitality.Tr(), (VillagerCharDisplayData data) => data.MaxMainAttributes[3].ToString(), 63, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Energy.Tr(), (VillagerCharDisplayData data) => data.MaxMainAttributes[4].ToString(), 64, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Intelligence.Tr(), (VillagerCharDisplayData data) => data.MaxMainAttributes[5].ToString(), 65, 30f, 90f);
			yield break;
		}

		// Token: 0x06005723 RID: 22307 RVA: 0x00288050 File Offset: 0x00286250
		private static IEnumerable<ColumnDefinition> GenerateProperty2Columns()
		{
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_HitType_0.Tr(), (VillagerCharDisplayData data) => data.HitValues[0].ToString(), 24, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_HitType_1.Tr(), (VillagerCharDisplayData data) => data.HitValues[1].ToString(), 25, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_HitType_2.Tr(), (VillagerCharDisplayData data) => data.HitValues[2].ToString(), 26, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_HitType_3.Tr(), (VillagerCharDisplayData data) => data.HitValues[3].ToString(), 27, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_AvoidType_0.Tr(), (VillagerCharDisplayData data) => data.AvoidValues[0].ToString(), 33, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_AvoidType_1.Tr(), (VillagerCharDisplayData data) => data.AvoidValues[1].ToString(), 34, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_AvoidType_2.Tr(), (VillagerCharDisplayData data) => data.AvoidValues[2].ToString(), 35, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_AvoidType_3.Tr(), (VillagerCharDisplayData data) => data.AvoidValues[3].ToString(), 36, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Qi_Disorder.Tr(), (VillagerCharDisplayData data) => data.DisorderOfQi.ToString(), 55, 30f, 90f);
			yield break;
		}

		// Token: 0x06005724 RID: 22308 RVA: 0x00288059 File Offset: 0x00286259
		private static IEnumerable<ColumnDefinition> GenerateLifeSkillColumns()
		{
			int num;
			for (int i = 0; i < 16; i = num + 1)
			{
				CharacterScrollMultipleMode.<>c__DisplayClass98_0 CS$<>8__locals1 = new CharacterScrollMultipleMode.<>c__DisplayClass98_0();
				CS$<>8__locals1.index = i;
				yield return CharacterScrollMultipleMode.CreateTextColumn(() => LocalStringManager.Get(string.Format("LK_LifeSkillType_{0}", CS$<>8__locals1.index)), (VillagerCharDisplayData data) => data.LifeSkillQualifications[CS$<>8__locals1.index].ToString(), (short)(66 + CS$<>8__locals1.index), 40f, 60f);
				CS$<>8__locals1 = null;
				num = i;
			}
			yield break;
		}

		// Token: 0x06005725 RID: 22309 RVA: 0x00288062 File Offset: 0x00286262
		private static IEnumerable<ColumnDefinition> GenerateCombatSkillColumns()
		{
			int num;
			for (int i = 0; i < 14; i = num + 1)
			{
				CharacterScrollMultipleMode.<>c__DisplayClass99_0 CS$<>8__locals1 = new CharacterScrollMultipleMode.<>c__DisplayClass99_0();
				CS$<>8__locals1.index = i;
				yield return CharacterScrollMultipleMode.CreateTextColumn(() => LocalStringManager.Get(string.Format("LK_CombatSkillType_{0}", CS$<>8__locals1.index)), (VillagerCharDisplayData data) => data.CombatSkillQualifications[CS$<>8__locals1.index].ToString(), (short)(82 + CS$<>8__locals1.index), 40f, 60f);
				CS$<>8__locals1 = null;
				num = i;
			}
			yield break;
		}

		// Token: 0x06005726 RID: 22310 RVA: 0x0028806B File Offset: 0x0028626B
		private static IEnumerable<ColumnDefinition> GeneratePersonalityColumns()
		{
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Personality_Calm_Name.Tr(), (VillagerCharDisplayData data) => data.Personalities[0].ToString(), 96, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Personality_Clever_Name.Tr(), (VillagerCharDisplayData data) => data.Personalities[1].ToString(), 97, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Personality_Enthusiastic_Name.Tr(), (VillagerCharDisplayData data) => data.Personalities[2].ToString(), 98, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Personality_Brave_Name.Tr(), (VillagerCharDisplayData data) => data.Personalities[3].ToString(), 99, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Personality_Firm_Name.Tr(), (VillagerCharDisplayData data) => data.Personalities[4].ToString(), 100, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Personality_Lucky_Name.Tr(), (VillagerCharDisplayData data) => data.Personalities[5].ToString(), 101, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Personality_Perceptive_Name.Tr(), (VillagerCharDisplayData data) => data.Personalities[6].ToString(), 102, 30f, 90f);
			yield break;
		}

		// Token: 0x06005727 RID: 22311 RVA: 0x00288074 File Offset: 0x00286274
		private static IEnumerable<ColumnDefinition> GenerateItemColumns()
		{
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Food.Tr(), (VillagerCharDisplayData data) => data.Resources[0].ToString(), 103, 40f, 60f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Wood.Tr(), (VillagerCharDisplayData data) => data.Resources[1].ToString(), 104, 40f, 60f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Metal.Tr(), (VillagerCharDisplayData data) => data.Resources[2].ToString(), 105, 40f, 60f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Jade.Tr(), (VillagerCharDisplayData data) => data.Resources[3].ToString(), 106, 40f, 60f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Fabric.Tr(), (VillagerCharDisplayData data) => data.Resources[4].ToString(), 107, 40f, 60f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Herb.Tr(), (VillagerCharDisplayData data) => data.Resources[5].ToString(), 108, 40f, 60f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Money.Tr(), (VillagerCharDisplayData data) => data.Resources[6].ToString(), 109, 40f, 60f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Authority.Tr(), (VillagerCharDisplayData data) => data.Resources[7].ToString(), 110, 40f, 60f);
			yield return CharacterScrollMultipleMode.CreateTextColumn(() => LanguageKey.LK_Inventory.Tr(), (VillagerCharDisplayData data) => string.Format("{0:f1}/{1:f1}", (float)data.CurrInventoryLoad / 100f, (float)data.MaxInventoryLoad / 100f), 37, 30f, 90f);
			yield break;
		}

		// Token: 0x06005728 RID: 22312 RVA: 0x0028807D File Offset: 0x0028627D
		private static IEnumerable<ColumnDefinition> GenerateCommandColumns()
		{
			yield return CharacterScrollMultipleMode.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Attack.Tr(), (VillagerCharDisplayData data) => CharacterScrollMultipleMode.CreateMedalCellData(data.AttackMedal, 0), 112, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Defence.Tr(), (VillagerCharDisplayData data) => CharacterScrollMultipleMode.CreateMedalCellData(data.DefenceMedal, 1), 113, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Wisdom.Tr(), (VillagerCharDisplayData data) => CharacterScrollMultipleMode.CreateMedalCellData(data.WisdomMedal, 2), 114, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_0.Tr(), (VillagerCharDisplayData data) => CharacterScrollMultipleMode.CreateCommandCellData(data, 0), 115, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_1.Tr(), (VillagerCharDisplayData data) => CharacterScrollMultipleMode.CreateCommandCellData(data, 1), 116, 30f, 90f);
			yield return CharacterScrollMultipleMode.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_2.Tr(), (VillagerCharDisplayData data) => CharacterScrollMultipleMode.CreateCommandCellData(data, 2), 117, 30f, 90f);
			yield break;
		}

		// Token: 0x06005729 RID: 22313 RVA: 0x00288088 File Offset: 0x00286288
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
				string iconName = CharacterScrollMultipleMode.GetMedalIconName(medalCount, medalType);
				string text = string.Format(" x{0}", Mathf.Abs(medalCount));
				result = new IconAndTextCellData(iconName, text, true, false, false, false);
			}
			return result;
		}

		// Token: 0x0600572A RID: 22314 RVA: 0x002880DC File Offset: 0x002862DC
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

		// Token: 0x0600572B RID: 22315 RVA: 0x0028815C File Offset: 0x0028635C
		private static IconAndTextCellData CreateCommandCellData(VillagerCharDisplayData data, int commandIndex)
		{
			bool flag = data.Command.Items == null || !data.Command.Items.CheckIndex(commandIndex);
			IconAndTextCellData result;
			if (flag)
			{
				result = IconAndTextCellData.TextOnly("-");
			}
			else
			{
				sbyte commandId = data.Command.Items[commandIndex];
				bool flag2 = commandId < 0;
				if (flag2)
				{
					result = IconAndTextCellData.TextOnly("-");
				}
				else
				{
					TeammateCommandItem cmdConfig = Config.TeammateCommand.Instance[commandId];
					result = IconAndTextCellData.TextOnly(cmdConfig.Name);
				}
			}
			return result;
		}

		// Token: 0x0600572C RID: 22316 RVA: 0x002881EC File Offset: 0x002863EC
		private void PrepareRowTemplateContainers()
		{
			int subPageKey = (int)this._currentSubPage;
			int roleKey = (int)this._roleKey;
			Dictionary<int, RowItem> roleDic;
			bool flag = this._rowTemplateCache.TryGetValue(subPageKey, out roleDic);
			if (flag)
			{
				RowItem currentTemplate;
				bool flag2 = roleDic.TryGetValue(roleKey, out currentTemplate);
				if (flag2)
				{
					this.scroll.SetRowTemplate(currentTemplate);
					this.selectedScroll.SetRowTemplate(currentTemplate);
				}
				else
				{
					currentTemplate = this.CreateRowTemplateForSubPage();
					this._rowTemplateCache[subPageKey].Add(roleKey, currentTemplate);
					this.scroll.SetRowTemplate(currentTemplate);
					this.selectedScroll.SetRowTemplate(currentTemplate);
				}
			}
			else
			{
				RowItem currentTemplate = this.CreateRowTemplateForSubPage();
				Dictionary<int, RowItem> roleKeyDic = new Dictionary<int, RowItem>
				{
					{
						roleKey,
						currentTemplate
					}
				};
				this._rowTemplateCache.Add(subPageKey, roleKeyDic);
				this.scroll.SetRowTemplate(currentTemplate);
				this.selectedScroll.SetRowTemplate(currentTemplate);
			}
		}

		// Token: 0x0600572D RID: 22317 RVA: 0x002882C8 File Offset: 0x002864C8
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
					bool flag2 = genericArgs.Length >= 2 && genericArgs[1] == typeof(MerchantDropdownCellData);
					if (flag2)
					{
						container = Object.Instantiate<RowCellContainer>(this.dropdownContainer, containerRoot);
					}
					else
					{
						container = Object.Instantiate<RowCellContainer>(this.singleTextCellContainer, containerRoot);
					}
				}
				container.gameObject.SetActive(true);
			}
			newTemplate.ResetSibling();
			return newTemplate;
		}

		// Token: 0x0600572E RID: 22318 RVA: 0x00288410 File Offset: 0x00286610
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitionsForCurrentSubPage()
		{
			bool flag = this._hasCustomSubPage && this._currentSubPage < EVillagerCharacterListSubPage.Role;
			IEnumerable<ColumnDefinition> result;
			if (flag)
			{
				Func<IEnumerable<ColumnDefinition>> customColumnGenerator = this._config.CustomColumnGenerator;
				result = (((customColumnGenerator != null) ? customColumnGenerator() : null) ?? Enumerable.Empty<ColumnDefinition>());
			}
			else
			{
				EVillagerCharacterListSubPage currentSubPage = this._currentSubPage;
				if (!true)
				{
				}
				IEnumerable<ColumnDefinition> enumerable;
				switch (currentSubPage)
				{
				case EVillagerCharacterListSubPage.Role:
					enumerable = this.GenerateRoleColumns();
					break;
				case EVillagerCharacterListSubPage.Villager:
					enumerable = this.GenerateVillagerColumns();
					break;
				case EVillagerCharacterListSubPage.State:
					enumerable = CharacterScrollMultipleMode.GenerateStateColumns();
					break;
				case EVillagerCharacterListSubPage.Property:
					enumerable = CharacterScrollMultipleMode.GeneratePropertyColumns();
					break;
				case EVillagerCharacterListSubPage.Property2:
					enumerable = CharacterScrollMultipleMode.GenerateProperty2Columns();
					break;
				case EVillagerCharacterListSubPage.LifeSkill:
					enumerable = CharacterScrollMultipleMode.GenerateLifeSkillColumns();
					break;
				case EVillagerCharacterListSubPage.CombatSkill:
					enumerable = CharacterScrollMultipleMode.GenerateCombatSkillColumns();
					break;
				case EVillagerCharacterListSubPage.Personality:
					enumerable = CharacterScrollMultipleMode.GeneratePersonalityColumns();
					break;
				case EVillagerCharacterListSubPage.Item:
					enumerable = CharacterScrollMultipleMode.GenerateItemColumns();
					break;
				case EVillagerCharacterListSubPage.Command:
					enumerable = CharacterScrollMultipleMode.GenerateCommandColumns();
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

		// Token: 0x0600572F RID: 22319 RVA: 0x002884FC File Offset: 0x002866FC
		private int GetColumnCount()
		{
			bool flag = this._hasCustomSubPage && this._currentSubPage < EVillagerCharacterListSubPage.Role;
			int result;
			if (flag)
			{
				Func<IEnumerable<ColumnDefinition>> customColumnGenerator = this._config.CustomColumnGenerator;
				result = ((customColumnGenerator != null) ? customColumnGenerator().Count<ColumnDefinition>() : 0);
			}
			else
			{
				EVillagerCharacterListSubPage currentSubPage = this._currentSubPage;
				if (!true)
				{
				}
				int num;
				switch (currentSubPage)
				{
				case EVillagerCharacterListSubPage.Role:
					num = 7;
					break;
				case EVillagerCharacterListSubPage.Villager:
					num = 10;
					break;
				case EVillagerCharacterListSubPage.State:
					num = 7;
					break;
				case EVillagerCharacterListSubPage.Property:
					num = 6;
					break;
				case EVillagerCharacterListSubPage.Property2:
					num = 9;
					break;
				case EVillagerCharacterListSubPage.LifeSkill:
					num = 16;
					break;
				case EVillagerCharacterListSubPage.CombatSkill:
					num = 14;
					break;
				case EVillagerCharacterListSubPage.Personality:
					num = 7;
					break;
				case EVillagerCharacterListSubPage.Item:
					num = 9;
					break;
				case EVillagerCharacterListSubPage.Command:
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

		// Token: 0x06005730 RID: 22320 RVA: 0x002885B7 File Offset: 0x002867B7
		private void OnRowClicked(int index, RowItem rowItem)
		{
			this.OnClickMain(index);
			this.RefreshStyle();
		}

		// Token: 0x06005731 RID: 22321 RVA: 0x002885CC File Offset: 0x002867CC
		private bool IsRowSelected(int index, object rowData)
		{
			VillagerCharDisplayData data = rowData as VillagerCharDisplayData;
			bool flag = data == null;
			return !flag && this._selectedCharId.Contains(data.CharacterId);
		}

		// Token: 0x06005732 RID: 22322 RVA: 0x00288608 File Offset: 0x00286808
		private int GetSingleSelectedIndex()
		{
			return -1;
		}

		// Token: 0x06005733 RID: 22323 RVA: 0x0028861C File Offset: 0x0028681C
		private void OnSelectedRowClicked(int index, RowItem item)
		{
			bool flag = index < 0 || index >= this._selectedCharacterDatas.Count;
			if (!flag)
			{
				this.OnClickMainSelected(index);
			}
		}

		// Token: 0x06005734 RID: 22324 RVA: 0x00288650 File Offset: 0x00286850
		private void OnSearchButtonClicked()
		{
			TMP_InputField tmp_InputField = this.searchInput;
			this._searchText = (((tmp_InputField != null) ? tmp_InputField.text : null) ?? string.Empty);
			this.RefreshListData();
		}

		// Token: 0x06005735 RID: 22325 RVA: 0x0028867B File Offset: 0x0028687B
		private void OnSearchInputEndEdit(string text)
		{
			this._searchText = text;
			this.RefreshListData();
		}

		// Token: 0x06005736 RID: 22326 RVA: 0x0028868C File Offset: 0x0028688C
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

		// Token: 0x06005737 RID: 22327 RVA: 0x00288722 File Offset: 0x00286922
		private void UpdateDisplay()
		{
			this.RefreshListData();
			this.RefreshButtons();
			this.RefreshStyle();
		}

		// Token: 0x06005738 RID: 22328 RVA: 0x0028873A File Offset: 0x0028693A
		private void RefreshButtons()
		{
		}

		// Token: 0x04003B6F RID: 15215
		private bool _currentModeInteractable = true;

		// Token: 0x04003B70 RID: 15216
		private static readonly List<LanguageKey> BaseSubPageNameKeys = new List<LanguageKey>
		{
			LanguageKey.LK_Main_SummaryInfo_Identity,
			LanguageKey.LK_Villager,
			LanguageKey.LK_Team_Tog_State,
			LanguageKey.LK_Team_Tog_Property,
			LanguageKey.LK_Team_Tog_Property_Hit,
			LanguageKey.LK_Team_Tog_LifeSkill,
			LanguageKey.LK_Team_Tog_CombatSkill,
			LanguageKey.LK_Team_Tog_Personality,
			LanguageKey.LK_Team_Tog_Item,
			LanguageKey.LK_Team_Tog_Command
		};

		// Token: 0x04003B71 RID: 15217
		[SerializeField]
		private CToggleGroup subPageToggleGroup;

		// Token: 0x04003B72 RID: 15218
		[SerializeField]
		private ListStyleGeneralScroll scroll;

		// Token: 0x04003B73 RID: 15219
		[SerializeField]
		private ListStyleGeneralScroll selectedScroll;

		// Token: 0x04003B74 RID: 15220
		[SerializeField]
		private GameObject selRoot;

		// Token: 0x04003B75 RID: 15221
		[SerializeField]
		private SortAndFilter sortAndFilter;

		// Token: 0x04003B76 RID: 15222
		[SerializeField]
		private TextMeshProUGUI txtSelectedAmount;

		// Token: 0x04003B77 RID: 15223
		[Header("行模板配置")]
		[SerializeField]
		private RowItem rowTemplate;

		// Token: 0x04003B78 RID: 15224
		[SerializeField]
		private RowCellContainer avatarAndNameCellContainer;

		// Token: 0x04003B79 RID: 15225
		[SerializeField]
		private RowCellContainer singleTextCellContainer;

		// Token: 0x04003B7A RID: 15226
		[SerializeField]
		private RowCellContainer iconAndTextCellContainer;

		// Token: 0x04003B7B RID: 15227
		[SerializeField]
		private RowCellContainer dropdownContainer;

		// Token: 0x04003B7C RID: 15228
		[SerializeField]
		private TMP_InputField searchInput;

		// Token: 0x04003B7D RID: 15229
		[SerializeField]
		private CButton searchButton;

		// Token: 0x04003B7E RID: 15230
		[Header("平铺界面")]
		[SerializeField]
		private InfinityScroll flatModeScroll;

		// Token: 0x04003B7F RID: 15231
		[SerializeField]
		private AssignPageVillagerView flatModeCahrTemplate;

		// Token: 0x04003B80 RID: 15232
		[SerializeField]
		private InfinityScroll selectedFlatModeScroll;

		// Token: 0x04003B81 RID: 15233
		[Header("交互")]
		[SerializeField]
		private CButton btnSelectAll;

		// Token: 0x04003B82 RID: 15234
		[SerializeField]
		private CImage checkmarkSelectAll;

		// Token: 0x04003B83 RID: 15235
		[SerializeField]
		private CToggleGroup switchToggleGroup;

		// Token: 0x04003B84 RID: 15236
		[SerializeField]
		private CButton btnClearAll;

		// Token: 0x04003B85 RID: 15237
		[SerializeField]
		private SwitchToggleSmall switchToggleBig;

		// Token: 0x04003B86 RID: 15238
		private List<VillagerCharDisplayData> _dataList = new List<VillagerCharDisplayData>();

		// Token: 0x04003B87 RID: 15239
		private List<VillagerCharDisplayData> _filteredDataList = new List<VillagerCharDisplayData>();

		// Token: 0x04003B88 RID: 15240
		private short _roleKey;

		// Token: 0x04003B89 RID: 15241
		private SelectCharacterConfig _config;

		// Token: 0x04003B8A RID: 15242
		private readonly List<VillagerCharDisplayData> _selectedCharacterDatas = new List<VillagerCharDisplayData>();

		// Token: 0x04003B8B RID: 15243
		private readonly HashSet<int> _bannedCharacterIds = new HashSet<int>();

		// Token: 0x04003B8C RID: 15244
		private VillagerCharacterScrollController _sortAndFilterController;

		// Token: 0x04003B8D RID: 15245
		private TabSortStateManager<int, VillagerCharDisplayData> _tabSortStateManager;

		// Token: 0x04003B8E RID: 15246
		private EVillagerCharacterListSubPage _currentSubPage = EVillagerCharacterListSubPage.Role;

		// Token: 0x04003B8F RID: 15247
		private bool _hasCustomSubPage;

		// Token: 0x04003B90 RID: 15248
		private int _customSubPageIndex = -1;

		// Token: 0x04003B91 RID: 15249
		private bool _listStructureReady;

		// Token: 0x04003B92 RID: 15250
		private int _currentShowStyle = -1;

		// Token: 0x04003B93 RID: 15251
		private CToggle _dynamicToggle;

		// Token: 0x04003B94 RID: 15252
		private string _searchText = string.Empty;

		// Token: 0x04003B95 RID: 15253
		private readonly Dictionary<int, Dictionary<int, RowItem>> _rowTemplateCache = new Dictionary<int, Dictionary<int, RowItem>>();

		// Token: 0x04003B96 RID: 15254
		private bool _pendingInitialize;

		// Token: 0x04003B97 RID: 15255
		private bool _uiInitialized;

		// Token: 0x04003B98 RID: 15256
		private bool _dataInited = false;

		// Token: 0x04003B99 RID: 15257
		private HashSet<int> _selectedCharId = new HashSet<int>();

		// Token: 0x04003B9A RID: 15258
		public Action OnSelectDataChange;

		// Token: 0x04003B9B RID: 15259
		public Action<int> OnRemoveVillagerRole;

		// Token: 0x04003B9C RID: 15260
		private bool _inited = false;

		// Token: 0x04003B9D RID: 15261
		private bool _listMode = true;
	}
}
