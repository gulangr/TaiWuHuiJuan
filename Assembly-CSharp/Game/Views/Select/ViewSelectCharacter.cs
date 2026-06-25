using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UI;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.CellContent;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.SelectCharacter;
using Game.Views.CharacterMenu;
using Game.Views.Select.SelectCharacter;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.Relation;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Select
{
	// Token: 0x020007AC RID: 1964
	public class ViewSelectCharacter : UIBase
	{
		// Token: 0x06005EFC RID: 24316 RVA: 0x002B9308 File Offset: 0x002B7508
		public override void OnInit(ArgumentBox argsBox)
		{
			CommonSelectCharacterConfig config;
			argsBox.Get<CommonSelectCharacterConfig>("SelectCharacterConfig", out config);
			IReadOnlyList<ISelectCharacterData> dataList;
			argsBox.Get<IReadOnlyList<ISelectCharacterData>>("SelectCharacterDataList", out dataList);
			SelectCharacterCallback callback;
			argsBox.Get<SelectCharacterCallback>("SelectCharacterCallback", out callback);
			Action cancelCallback;
			argsBox.Get<Action>("SelectCharacterCancelCallback", out cancelCallback);
			argsBox.Get("ShowInFront", out this._displayBg);
			this.SetupLayer();
			this.CacheInitData(config ?? new CommonSelectCharacterConfig(), dataList ?? new List<ISelectCharacterData>(), callback, cancelCallback);
			this._pendingInitialize = true;
			this._uiInitialized = false;
			this.btnClear.gameObject.SetActive(this._config.SelectionMode == ESelectCharacterSelectionMode.Multiple);
			this.btnFill.gameObject.SetActive(this._config.SelectionMode == ESelectCharacterSelectionMode.Multiple);
		}

		// Token: 0x06005EFD RID: 24317 RVA: 0x002B93D0 File Offset: 0x002B75D0
		public void SetupLayer()
		{
			int targetSortLayer = this._displayBg ? this.sortingLayerFront : this.sortingLayerNormal;
			bool flag = this.canvas.sortingOrder != targetSortLayer;
			if (flag)
			{
				this.canvas.sortingOrder = targetSortLayer;
				UIMaskManager instance = SingletonObject.getInstance<UIMaskManager>();
				if (instance != null)
				{
					instance.NotifyOwnerSortingChanged(base.transform);
				}
			}
		}

		// Token: 0x06005EFE RID: 24318 RVA: 0x002B9430 File Offset: 0x002B7630
		private void CacheInitData(CommonSelectCharacterConfig config, IReadOnlyList<ISelectCharacterData> dataList, SelectCharacterCallback callback, Action cancelCallback)
		{
			this._config = config;
			this._dataList.Clear();
			this._subpageIndexDic.Clear();
			for (int i = 0; i < config.Subpages.Count; i++)
			{
				this._subpageIndexDic[i] = config.Subpages[i];
			}
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
			this._callback = callback;
			this._cancelCallback = cancelCallback;
			this._selectedCharacterIds.Clear();
			bool flag3 = this._config.InitialSelectedCharacterIds != null;
			if (flag3)
			{
				this._selectedCharacterIds.AddRange(this._config.InitialSelectedCharacterIds);
			}
			this._searchText = string.Empty;
			this._rowTemplateCache.Clear();
			this._sortAndFilterController = null;
			this._listStructureReady = false;
			this._currentSubPage = config.Subpages[0];
		}

		// Token: 0x06005EFF RID: 24319 RVA: 0x002B955C File Offset: 0x002B775C
		private void Setup()
		{
			if (this._config == null)
			{
				this._config = new CommonSelectCharacterConfig();
			}
			this.UpdateButtonVisibilities();
			this.UpdateTitle();
			this.UpdateCustomTextInfo();
			this.InitSubPageToggles();
			this.InitSortAndFilter();
			this.RefreshList();
			this.UpdateSelectedArea();
		}

		// Token: 0x06005F00 RID: 24320 RVA: 0x002B95AE File Offset: 0x002B77AE
		private void UpdateButtonVisibilities()
		{
			this.close.gameObject.SetActive(this._config.MinSelectionCount == 0);
		}

		// Token: 0x06005F01 RID: 24321 RVA: 0x002B95D0 File Offset: 0x002B77D0
		private void UpdateTitle()
		{
			bool flag = this._config.Title.IsNullOrEmpty();
			if (flag)
			{
				this.titleLabel.text = LanguageKey.LK_SelectCharacter_Default_Title.Tr();
			}
			else
			{
				this.titleLabel.text = this._config.Title;
			}
		}

		// Token: 0x06005F02 RID: 24322 RVA: 0x002B9624 File Offset: 0x002B7824
		private void UpdateCustomTextInfo()
		{
			Func<IReadOnlyList<int>, string> customTextGenerator = this._config.CustomTextGenerator;
			string customText = ((customTextGenerator != null) ? customTextGenerator(this._selectedCharacterIds) : null) ?? string.Empty;
			bool showCustomText = !string.IsNullOrEmpty(customText);
			this.customTextArea.SetActive(showCustomText);
			bool flag = showCustomText;
			if (flag)
			{
				this.customTextLabel.text = customText;
			}
		}

		// Token: 0x06005F03 RID: 24323 RVA: 0x002B9684 File Offset: 0x002B7884
		private void Update()
		{
			bool flag = UIManager.Instance.IsFocusElement(UIElement.CharacterMenu);
			if (flag)
			{
				bool flag2 = this.canvas.sortingOrder != this.sortingLayerBack;
				if (flag2)
				{
					this.canvas.sortingOrder = this.sortingLayerBack;
					UIMaskManager instance = SingletonObject.getInstance<UIMaskManager>();
					if (instance != null)
					{
						instance.NotifyOwnerSortingChanged(base.transform);
					}
				}
			}
			bool flag3 = !UIManager.Instance.IsFocusElement(this.Element);
			if (!flag3)
			{
				this.SetupLayer();
				bool triggerButtonDown = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
				this.inputCounter -= Time.deltaTime;
				bool flag4 = this.IsSearchInputActive();
				if (flag4)
				{
					this.inputCounter = 0.2f;
				}
				bool flag5 = this.confirm.interactable && triggerButtonDown && this.inputCounter < 0f;
				if (flag5)
				{
					this.OnConfirmClicked();
				}
			}
		}

		// Token: 0x06005F04 RID: 24324 RVA: 0x002B977C File Offset: 0x002B797C
		private bool IsSearchInputActive()
		{
			return this.searchInput != null && this.searchInput.isFocused;
		}

		// Token: 0x06005F05 RID: 24325 RVA: 0x002B97AC File Offset: 0x002B79AC
		private void Awake()
		{
			this.selectedAreaSwitchToggle.isOn = false;
			this.selectedScrollArea.SetActive(false);
			this.rowTemplate.gameObject.SetActive(false);
			this.avatarAndNameCellContainer.gameObject.SetActive(false);
			this.singleTextCellContainer.gameObject.SetActive(false);
			this.iconAndTextCellContainer.gameObject.SetActive(false);
			this.readProgressCellContainer.gameObject.SetActive(false);
			this.btnFill.ClearAndAddListener(new Action(this.OnSelectAll));
			this.btnClear.ClearAndAddListener(new Action(this.OnDeselectAll));
			bool flag = this.selectedSlotTemplate != null;
			if (flag)
			{
				this.selectedSlotTemplate.gameObject.SetActive(false);
			}
			bool flag2 = this.scroll != null && this.rowTemplate != null;
			if (flag2)
			{
				this.scroll.SetRowTemplate(this.rowTemplate);
			}
			bool flag3 = this.scroll != null;
			if (flag3)
			{
				this.scroll.RowSelectedProvider = new Func<int, object, bool>(this.IsRowSelected);
			}
			this.scroll.OnRowClicked += this.OnRowClicked;
			this.scroll.RowDisabledProvider = new Func<int, object, bool>(this.IsRowDisabled);
			bool flag4 = this.searchButton != null;
			if (flag4)
			{
				this.searchButton.ClearAndAddListener(new Action(this.OnSearchButtonClicked));
			}
			bool flag5 = this.btnClearSearch != null;
			if (flag5)
			{
				this.btnClearSearch.ClearAndAddListener(new Action(this.OnClickBtnClearSearch));
			}
			bool flag6 = this.searchInput != null;
			if (flag6)
			{
				this.searchInput.onEndEdit.AddListener(new UnityAction<string>(this.OnSearchInputEndEdit));
			}
			bool flag7 = this.selectedAreaSwitchToggle != null;
			if (flag7)
			{
				this.selectedAreaSwitchToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnSelectedAreaSwitchClicked));
			}
			bool flag8 = this.confirm != null;
			if (flag8)
			{
				this.confirm.ClearAndAddListener(new Action(this.OnConfirmClicked));
			}
			bool flag9 = this.selectedScroll != null;
			if (flag9)
			{
				this.selectedScroll.OnItemRender += this.OnSelectedScrollItemRender;
			}
		}

		// Token: 0x06005F06 RID: 24326 RVA: 0x002B9A14 File Offset: 0x002B7C14
		private void OnEnable()
		{
			this.searchInput.text = string.Empty;
			bool flag = this._pendingInitialize || !this._uiInitialized;
			if (flag)
			{
				this._listStructureReady = false;
				this.Setup();
				this._pendingInitialize = false;
				this._uiInitialized = true;
			}
			else
			{
				this.RefreshListData();
				this.UpdateSelectedArea();
				this.UpdateTitle();
				this.UpdateCustomTextInfo();
			}
		}

		// Token: 0x06005F07 RID: 24327 RVA: 0x002B9A88 File Offset: 0x002B7C88
		private void OnDestroy()
		{
			this.scroll.OnRowClicked -= this.OnRowClicked;
			bool flag = this.selectedScroll != null;
			if (flag)
			{
				this.selectedScroll.OnItemRender -= this.OnSelectedScrollItemRender;
			}
			TabSortStateManager<int, ISelectCharacterData> tabSortStateManager = this._tabSortStateManager;
			if (tabSortStateManager != null)
			{
				tabSortStateManager.ClearAll();
			}
		}

		// Token: 0x06005F08 RID: 24328 RVA: 0x002B9AE8 File Offset: 0x002B7CE8
		private void InitSubPageToggles()
		{
			bool flag = this.subPageToggleGroup == null;
			if (!flag)
			{
				this.subPageToggleGroup.OnActiveIndexChange -= this.OnSubPageChanged;
				this.subPageToggleGroup.Clear();
				CommonUtils.PrepareEnoughChildren(this.subPageToggleGroup.transform, this.subPageToggleTemplate.gameObject, this._config.Subpages.Count, null);
				for (int i = 0; i < this._config.Subpages.Count; i++)
				{
					Refers refers = this.subPageToggleGroup.transform.GetChild(i).GetComponent<Refers>();
					this.subPageToggleGroup.Add(refers.CGet<CToggle>("ToggleComp"));
					TextMeshProUGUI label = refers.CGet<TextMeshProUGUI>("Label");
					bool flag2 = label != null;
					if (flag2)
					{
						label.text = this._config.Subpages[i].GetName();
					}
					refers.CGet<GameObject>("RightLine").SetActive(i != this._config.Subpages.Count - 1);
				}
				this.subPageToggleGroup.Init(-1);
				this.subPageToggleGroup.OnActiveIndexChange += this.OnSubPageChanged;
				this.subPageToggleGroup.Set(0, false);
			}
		}

		// Token: 0x06005F09 RID: 24329 RVA: 0x002B9C52 File Offset: 0x002B7E52
		private void OnSubPageChanged(int newIndex, int oldIndex)
		{
			this._currentSubPage = this._subpageIndexDic[newIndex];
			TabSortStateManager<int, ISelectCharacterData> tabSortStateManager = this._tabSortStateManager;
			if (tabSortStateManager != null)
			{
				tabSortStateManager.OnTabChange(newIndex);
			}
			this.RefreshList();
		}

		// Token: 0x06005F0A RID: 24330 RVA: 0x002B9C84 File Offset: 0x002B7E84
		private void InitSortAndFilter()
		{
			bool flag = this._config.FilterMenuIds == null || this._config.FilterMenuIds.Count == 0;
			if (flag)
			{
				this.sortAndFilter.gameObject.SetActive(false);
			}
			else
			{
				this.sortAndFilter.gameObject.SetActive(true);
				this._sortAndFilterController = new SelectCharacterSortAndFilterController(this.sortAndFilter, this._config.FilterMenuIds, null, this._config.SkipFallbackSort);
				this._sortAndFilterController.Init(new Action(this.OnSortAndFilterChanged), "SelectCharacter");
				this.scroll.SetSortController(this._sortAndFilterController);
				this._tabSortStateManager = new TabSortStateManager<int, ISelectCharacterData>(this._sortAndFilterController);
			}
		}

		// Token: 0x06005F0B RID: 24331 RVA: 0x002B9D48 File Offset: 0x002B7F48
		private void OnSortAndFilterChanged()
		{
			this.RefreshListData();
		}

		// Token: 0x06005F0C RID: 24332 RVA: 0x002B9D54 File Offset: 0x002B7F54
		private void OnSelectAll()
		{
			this._cachedSet = this._selectedCharacterIds.ToHashSet<int>();
			foreach (ISelectCharacterData item in this._filteredDataList)
			{
				bool flag = this._cachedSet.Contains(item.CharacterId) || this._bannedCharacterIds.Contains(item.CharacterId);
				if (!flag)
				{
					bool flag2 = this._config.TargetCount > 0 && this._selectedCharacterIds.Count >= this._config.TargetCount;
					if (flag2)
					{
						break;
					}
					this._selectedCharacterIds.Add(item.CharacterId);
				}
			}
			this.RefreshDisplay();
			this.UpdateSelectedArea();
			this.UpdateButtonState();
		}

		// Token: 0x06005F0D RID: 24333 RVA: 0x002B9E44 File Offset: 0x002B8044
		private void OnDeselectAll()
		{
			this._selectedCharacterIds.Clear();
			this.RefreshDisplay();
			this.UpdateSelectedArea();
			this.UpdateButtonState();
		}

		// Token: 0x06005F0E RID: 24334 RVA: 0x002B9E68 File Offset: 0x002B8068
		private void RefreshList()
		{
			this.RefreshListStructure();
			this.RefreshListData();
		}

		// Token: 0x06005F0F RID: 24335 RVA: 0x002B9E7C File Offset: 0x002B807C
		private void RefreshListStructure()
		{
			IEnumerable<ColumnDefinition> columnDefinitions = this.GenerateColumnDefinitions();
			this.PrepareRowTemplateContainers();
			bool flag = this.selectedScroll != null && this.selectedSlotTemplate != null;
			if (flag)
			{
				this.selectedScroll.srcPrefab = this.selectedSlotTemplate.gameObject;
				this.selectedScroll.ClearCache();
			}
			this.UpdateSelectedArea();
			this.scroll.ClearInfinityScrollCache();
			this.scroll.Init<ISelectCharacterData>(columnDefinitions, true, null, null);
			this._listStructureReady = true;
		}

		// Token: 0x06005F10 RID: 24336 RVA: 0x002B9F08 File Offset: 0x002B8108
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
			if ((func = ((sortAndFilterController != null) ? sortAndFilterController.GenerateFilter() : null)) == null && (func = ViewSelectCharacter.<>c.<>9__69_0) == null)
			{
				func = (ViewSelectCharacter.<>c.<>9__69_0 = ((ISelectCharacterData _) => true));
			}
			Func<ISelectCharacterData, bool> filter = func;
			IEnumerable<ISelectCharacterData> filtered = searchFiltered.Where(filter);
			this._filteredDataList.Clear();
			this._filteredDataList.AddRange(filtered);
			bool shouldSort = this._tabSortStateManager == null || this._tabSortStateManager.ShouldSort();
			bool flag2 = shouldSort;
			if (flag2)
			{
				SelectCharacterSortAndFilterController sortAndFilterController2 = this._sortAndFilterController;
				Comparison<ISelectCharacterData> comparer = (sortAndFilterController2 != null) ? sortAndFilterController2.GenerateComparer(this._filteredDataList) : null;
				bool flag3 = comparer != null;
				if (flag3)
				{
					this._filteredDataList.Sort(comparer);
				}
			}
			SelectCharacterSortAndFilterController sortAndFilterController3 = this._sortAndFilterController;
			if (sortAndFilterController3 != null)
			{
				sortAndFilterController3.AfterFilter(this._dataList);
			}
			this.RefreshDisplay();
		}

		// Token: 0x06005F11 RID: 24337 RVA: 0x002BA006 File Offset: 0x002B8206
		private void RefreshDisplay()
		{
			this.scroll.SetData<ISelectCharacterData>(this._filteredDataList, this.GetSingleSelectedIndex());
			this.UpdateButtonState();
			this.UpdateCustomTextInfo();
		}

		// Token: 0x06005F12 RID: 24338 RVA: 0x002BA030 File Offset: 0x002B8230
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
				Func<ISelectCharacterData, string> extractor = this._config.SearchTextExtractor ?? new Func<ISelectCharacterData, string>(ViewSelectCharacter.DefaultSearchTextExtractor);
				result = (from d in dataList
				where extractor(d).Contains(this._searchText)
				select d).ToList<ISelectCharacterData>();
			}
			return result;
		}

		// Token: 0x06005F13 RID: 24339 RVA: 0x002BA09C File Offset: 0x002B829C
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

		// Token: 0x06005F14 RID: 24340 RVA: 0x002BA0D2 File Offset: 0x002B82D2
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitions()
		{
			yield return this._config.CustomAvatar ?? ViewSelectCharacter.CreateAvatarWithNameColumn(this._config.RefreshDeadAsAlive, this._config.MouseTipModifier, new Action<int>(this.OpenCharacterMenu));
			bool flag = this._config.CustomColumnGenerator != null && this._config.CustomColumnGenerator.ContainsKey(this._currentSubPage);
			if (flag)
			{
				foreach (ColumnDefinition col in this._config.CustomColumnGenerator[this._currentSubPage]())
				{
					yield return col;
					col = null;
				}
				IEnumerator<ColumnDefinition> enumerator = null;
				yield break;
			}
			switch (this._currentSubPage)
			{
			case ESelectCharacterSubPage.State:
			{
				foreach (ColumnDefinition col2 in ViewSelectCharacter.GenerateStateColumns())
				{
					yield return col2;
					col2 = null;
				}
				IEnumerator<ColumnDefinition> enumerator2 = null;
				break;
			}
			case ESelectCharacterSubPage.Property:
			{
				foreach (ColumnDefinition col3 in ViewSelectCharacter.GeneratePropertyColumns())
				{
					yield return col3;
					col3 = null;
				}
				IEnumerator<ColumnDefinition> enumerator3 = null;
				break;
			}
			case ESelectCharacterSubPage.Property2:
			{
				foreach (ColumnDefinition col4 in ViewSelectCharacter.GenerateProperty2Columns())
				{
					yield return col4;
					col4 = null;
				}
				IEnumerator<ColumnDefinition> enumerator4 = null;
				break;
			}
			case ESelectCharacterSubPage.LifeSkill:
			{
				foreach (ColumnDefinition col5 in ViewSelectCharacter.GenerateLifeSkillColumns())
				{
					yield return col5;
					col5 = null;
				}
				IEnumerator<ColumnDefinition> enumerator5 = null;
				break;
			}
			case ESelectCharacterSubPage.CombatSkill:
			{
				foreach (ColumnDefinition col6 in ViewSelectCharacter.GenerateCombatSkillColumns())
				{
					yield return col6;
					col6 = null;
				}
				IEnumerator<ColumnDefinition> enumerator6 = null;
				break;
			}
			case ESelectCharacterSubPage.Personality:
			{
				foreach (ColumnDefinition col7 in ViewSelectCharacter.GeneratePersonalityColumns())
				{
					yield return col7;
					col7 = null;
				}
				IEnumerator<ColumnDefinition> enumerator7 = null;
				break;
			}
			case ESelectCharacterSubPage.Item:
			{
				foreach (ColumnDefinition col8 in ViewSelectCharacter.GenerateItemColumns())
				{
					yield return col8;
					col8 = null;
				}
				IEnumerator<ColumnDefinition> enumerator8 = null;
				break;
			}
			case ESelectCharacterSubPage.Command:
			{
				foreach (ColumnDefinition col9 in ViewSelectCharacter.GenerateCommandColumns())
				{
					yield return col9;
					col9 = null;
				}
				IEnumerator<ColumnDefinition> enumerator9 = null;
				break;
			}
			case ESelectCharacterSubPage.Villager:
			{
				foreach (ColumnDefinition col10 in VillagerSelectCharacterSelectionHelper.GenerateVillagerColumns())
				{
					yield return col10;
					col10 = null;
				}
				IEnumerator<ColumnDefinition> enumerator10 = null;
				break;
			}
			case ESelectCharacterSubPage.DarkAah:
				throw new NotImplementedException();
			case ESelectCharacterSubPage.YuanshanInfection:
			{
				foreach (ColumnDefinition col11 in this.GenerateYuanshanInfectionColumns())
				{
					yield return col11;
					col11 = null;
				}
				IEnumerator<ColumnDefinition> enumerator11 = null;
				break;
			}
			case ESelectCharacterSubPage.ApproveRate:
			{
				foreach (ColumnDefinition col12 in ViewSelectCharacter.GenerateApproveRateColumns())
				{
					yield return col12;
					col12 = null;
				}
				IEnumerator<ColumnDefinition> enumerator12 = null;
				break;
			}
			case ESelectCharacterSubPage.Grave:
				throw new NotImplementedException();
			case ESelectCharacterSubPage.AssignRolePeasant:
			{
				foreach (ColumnDefinition col13 in ViewSelectCharacter.GenerateAssignRolePeasantColumns())
				{
					yield return col13;
					col13 = null;
				}
				IEnumerator<ColumnDefinition> enumerator13 = null;
				break;
			}
			case ESelectCharacterSubPage.AssignRoleCraftsman:
			{
				foreach (ColumnDefinition col14 in ViewSelectCharacter.GenerateAssignRoleCraftsmanColumns())
				{
					yield return col14;
					col14 = null;
				}
				IEnumerator<ColumnDefinition> enumerator14 = null;
				break;
			}
			case ESelectCharacterSubPage.AssignRoleDoctor:
			{
				foreach (ColumnDefinition col15 in ViewSelectCharacter.GenerateAssignRoleDoctorColumns())
				{
					yield return col15;
					col15 = null;
				}
				IEnumerator<ColumnDefinition> enumerator15 = null;
				break;
			}
			case ESelectCharacterSubPage.AssignRoleMerchant:
			{
				foreach (ColumnDefinition col16 in ViewSelectCharacter.GenerateAssignRoleMerchantColumns())
				{
					yield return col16;
					col16 = null;
				}
				IEnumerator<ColumnDefinition> enumerator16 = null;
				break;
			}
			case ESelectCharacterSubPage.AssignRoleLiterati:
			{
				foreach (ColumnDefinition col17 in ViewSelectCharacter.GenerateAssignRoleLiteratiColumns())
				{
					yield return col17;
					col17 = null;
				}
				IEnumerator<ColumnDefinition> enumerator17 = null;
				break;
			}
			case ESelectCharacterSubPage.AssignRoleSwordTombKeeper:
			{
				foreach (ColumnDefinition col18 in ViewSelectCharacter.GenerateAssignRoleTombKeeperColumns())
				{
					yield return col18;
					col18 = null;
				}
				IEnumerator<ColumnDefinition> enumerator18 = null;
				break;
			}
			case ESelectCharacterSubPage.AssignRoleEnvoy:
			{
				foreach (ColumnDefinition col19 in ViewSelectCharacter.GenerateAssignRoleEnvoyColumns())
				{
					yield return col19;
					col19 = null;
				}
				IEnumerator<ColumnDefinition> enumerator19 = null;
				break;
			}
			case ESelectCharacterSubPage.Baihua:
			{
				foreach (ColumnDefinition col20 in ViewSelectCharacter.GenerateBaihuaColumns())
				{
					yield return col20;
					col20 = null;
				}
				IEnumerator<ColumnDefinition> enumerator20 = null;
				break;
			}
			}
			yield break;
			yield break;
		}

		// Token: 0x06005F15 RID: 24341 RVA: 0x002BA0E4 File Offset: 0x002B82E4
		public static ColumnDefinition CreateAvatarWithNameColumn(bool refreshDeadAsAlive, Action<TooltipInvoker, int> mouseTipModifier = null, Action<int> onClickCallback = null)
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
				AvatarWithNameCellData ret = AvatarWithNameCellData.FromCharacterDisplayDataForGeneralScrollList(generalData, generalData != null && generalData.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId, onClickCallback, mouseTipModifier);
				bool refreshDeadAsAlive2 = refreshDeadAsAlive;
				if (refreshDeadAsAlive2)
				{
					AvatarWithNameCellData avatarWithNameCellData = ret;
					object obj;
					if (generalData == null)
					{
						obj = null;
					}
					else
					{
						AvatarRelatedData avatarRelatedData = generalData.AvatarRelatedData;
						obj = ((avatarRelatedData != null) ? avatarRelatedData.AvatarData : null);
					}
					avatarWithNameCellData.AsGrave = (obj == null);
				}
				return ret;
			};
			columnDefinition.SortId = 0;
			return columnDefinition;
		}

		// Token: 0x06005F16 RID: 24342 RVA: 0x002BA190 File Offset: 0x002B8390
		private void OpenCharacterMenu(int charId)
		{
			bool flag = this._config == null || !this._config.CanShowCharacterMenu || this._config.RefreshDeadAsAlive;
			if (!flag)
			{
				UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", charId).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.CharacterBase, ECharacterSubPage.None)));
				UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
			}
		}

		// Token: 0x06005F17 RID: 24343 RVA: 0x002BA20C File Offset: 0x002B840C
		private static ColumnDefinition CreateTextColumn(Func<string> headerKey, Func<CharacterDisplayDataForGeneralScrollList, string> valueGetter, short sortId = -1, float minWidth = 30f, float preferredWidth = 90f)
		{
			return ViewSelectCharacter.CreateGenericTextColumn<ISelectCharacterData>(headerKey, (ISelectCharacterData data) => valueGetter(data.GetGeneralScrollListData()), sortId, minWidth, preferredWidth);
		}

		// Token: 0x06005F18 RID: 24344 RVA: 0x002BA23C File Offset: 0x002B843C
		public static ColumnDefinition CreateGenericTextColumn<T>(Func<string> headerKey, Func<T, string> valueGetter, short sortId = -1, float minWidth = 30f, float preferredWidth = 90f)
		{
			return new ColumnDefinition<T, string>
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

		// Token: 0x06005F19 RID: 24345 RVA: 0x002BA29C File Offset: 0x002B849C
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

		// Token: 0x06005F1A RID: 24346 RVA: 0x002BA2F4 File Offset: 0x002B84F4
		private static ColumnDefinition CreateIconAndTextColumn(Func<string> headerKey, Func<CharacterDisplayDataForGeneralScrollList, IconAndTextCellData> valueGetter, short sortId = -1, float minWidth = 30f, float preferredWidth = 90f)
		{
			return new ColumnDefinition<CharacterDisplayDataForGeneralScrollList, IconAndTextCellData>
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

		// Token: 0x06005F1B RID: 24347 RVA: 0x002BA34C File Offset: 0x002B854C
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
			bool flag2 = this.selectedSlotTemplate != null && currentTemplate != null;
			if (flag2)
			{
				this.selectedSlotTemplate.PrepareStructure(currentTemplate);
			}
		}

		// Token: 0x06005F1C RID: 24348 RVA: 0x002BA3D4 File Offset: 0x002B85D4
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
					bool flag2 = genericArgs.Length >= 2 && genericArgs[1] == typeof(ReadProgressData);
					if (flag2)
					{
						container = Object.Instantiate<RowCellContainer>(this.readProgressCellContainer, containerRoot);
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

		// Token: 0x06005F1D RID: 24349 RVA: 0x002BA51C File Offset: 0x002B871C
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitionsForCurrentSubPage()
		{
			bool flag = this._config.CustomColumnGenerator != null && this._config.CustomColumnGenerator.ContainsKey(this._currentSubPage);
			IEnumerable<ColumnDefinition> result;
			if (flag)
			{
				result = this._config.CustomColumnGenerator[this._currentSubPage]();
			}
			else
			{
				ESelectCharacterSubPage currentSubPage = this._currentSubPage;
				if (!true)
				{
				}
				IEnumerable<ColumnDefinition> enumerable;
				switch (currentSubPage)
				{
				case ESelectCharacterSubPage.State:
					enumerable = ViewSelectCharacter.GenerateStateColumns();
					goto IL_16E;
				case ESelectCharacterSubPage.Property:
					enumerable = ViewSelectCharacter.GeneratePropertyColumns();
					goto IL_16E;
				case ESelectCharacterSubPage.Property2:
					enumerable = ViewSelectCharacter.GenerateProperty2Columns();
					goto IL_16E;
				case ESelectCharacterSubPage.LifeSkill:
					enumerable = ViewSelectCharacter.GenerateLifeSkillColumns();
					goto IL_16E;
				case ESelectCharacterSubPage.CombatSkill:
					enumerable = ViewSelectCharacter.GenerateCombatSkillColumns();
					goto IL_16E;
				case ESelectCharacterSubPage.Personality:
					enumerable = ViewSelectCharacter.GeneratePersonalityColumns();
					goto IL_16E;
				case ESelectCharacterSubPage.Item:
					enumerable = ViewSelectCharacter.GenerateItemColumns();
					goto IL_16E;
				case ESelectCharacterSubPage.Command:
					enumerable = ViewSelectCharacter.GenerateCommandColumns();
					goto IL_16E;
				case ESelectCharacterSubPage.Villager:
					enumerable = VillagerSelectCharacterSelectionHelper.GenerateVillagerColumns();
					goto IL_16E;
				case ESelectCharacterSubPage.YuanshanInfection:
					enumerable = this.GenerateYuanshanInfectionColumns();
					goto IL_16E;
				case ESelectCharacterSubPage.ApproveRate:
					enumerable = ViewSelectCharacter.GenerateApproveRateColumns();
					goto IL_16E;
				case ESelectCharacterSubPage.AssignRolePeasant:
					enumerable = ViewSelectCharacter.GenerateAssignRolePeasantColumns();
					goto IL_16E;
				case ESelectCharacterSubPage.AssignRoleCraftsman:
					enumerable = ViewSelectCharacter.GenerateAssignRoleCraftsmanColumns();
					goto IL_16E;
				case ESelectCharacterSubPage.AssignRoleDoctor:
					enumerable = ViewSelectCharacter.GenerateAssignRoleDoctorColumns();
					goto IL_16E;
				case ESelectCharacterSubPage.AssignRoleMerchant:
					enumerable = ViewSelectCharacter.GenerateAssignRoleMerchantColumns();
					goto IL_16E;
				case ESelectCharacterSubPage.AssignRoleLiterati:
					enumerable = ViewSelectCharacter.GenerateAssignRoleLiteratiColumns();
					goto IL_16E;
				case ESelectCharacterSubPage.AssignRoleSwordTombKeeper:
					enumerable = ViewSelectCharacter.GenerateAssignRoleTombKeeperColumns();
					goto IL_16E;
				case ESelectCharacterSubPage.AssignRoleEnvoy:
					enumerable = ViewSelectCharacter.GenerateAssignRoleEnvoyColumns();
					goto IL_16E;
				case ESelectCharacterSubPage.Baihua:
					enumerable = ViewSelectCharacter.GenerateBaihuaColumns();
					goto IL_16E;
				}
				enumerable = Enumerable.Empty<ColumnDefinition>();
				IL_16E:
				if (!true)
				{
				}
				result = enumerable;
			}
			return result;
		}

		// Token: 0x06005F1E RID: 24350 RVA: 0x002BA6A0 File Offset: 0x002B88A0
		private int GetColumnCount()
		{
			ESelectCharacterSubPage currentSubPage = this._currentSubPage;
			if (!true)
			{
			}
			int result;
			switch (currentSubPage)
			{
			case ESelectCharacterSubPage.State:
				result = 9;
				break;
			case ESelectCharacterSubPage.Property:
				result = 6;
				break;
			case ESelectCharacterSubPage.Property2:
				result = 8;
				break;
			case ESelectCharacterSubPage.LifeSkill:
				result = 16;
				break;
			case ESelectCharacterSubPage.CombatSkill:
				result = 14;
				break;
			case ESelectCharacterSubPage.Personality:
				result = 7;
				break;
			case ESelectCharacterSubPage.Item:
				result = 9;
				break;
			case ESelectCharacterSubPage.Command:
				result = 6;
				break;
			case ESelectCharacterSubPage.Villager:
				result = 9;
				break;
			case ESelectCharacterSubPage.DarkAah:
				result = 7;
				break;
			case ESelectCharacterSubPage.YuanshanInfection:
				result = 7;
				break;
			case ESelectCharacterSubPage.ApproveRate:
				result = 7;
				break;
			case ESelectCharacterSubPage.Grave:
				result = 5;
				break;
			default:
				result = 0;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06005F1F RID: 24351 RVA: 0x002BA740 File Offset: 0x002B8940
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
						bool flag4 = this._selectedCharacterIds.Contains(charId);
						if (flag4)
						{
							bool flag5 = this._config.InteractionMode == ESelectCharacterInteractionMode.Slot;
							if (flag5)
							{
								this._selectedCharacterIds.Remove(charId);
							}
						}
						else
						{
							this._selectedCharacterIds.Clear();
							this._selectedCharacterIds.Add(charId);
						}
					}
					else
					{
						bool flag6 = this._selectedCharacterIds.Contains(charId);
						if (flag6)
						{
							this._selectedCharacterIds.Remove(charId);
						}
						else
						{
							bool flag7 = this._selectedCharacterIds.Count >= this._config.TargetCount;
							if (flag7)
							{
								return;
							}
							this._selectedCharacterIds.Add(charId);
						}
					}
					this.UpdateSelectedArea();
					this.UpdateButtonState();
					this.RefreshListData();
				}
			}
		}

		// Token: 0x06005F20 RID: 24352 RVA: 0x002BA870 File Offset: 0x002B8A70
		private bool IsRowSelected(int index, object rowData)
		{
			ISelectCharacterData data = rowData as ISelectCharacterData;
			bool flag = data == null;
			return !flag && this._selectedCharacterIds.Contains(data.CharacterId);
		}

		// Token: 0x06005F21 RID: 24353 RVA: 0x002BA8AC File Offset: 0x002B8AAC
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
				bool flag2 = this._selectedCharacterIds.Count != 1;
				if (flag2)
				{
					result = -1;
				}
				else
				{
					int selectedId = this._selectedCharacterIds[0];
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

		// Token: 0x06005F22 RID: 24354 RVA: 0x002BA940 File Offset: 0x002B8B40
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

		// Token: 0x06005F23 RID: 24355 RVA: 0x002BA984 File Offset: 0x002B8B84
		private void UpdateSelectedArea()
		{
			this.selectedCountLabel.text = LanguageKey.LK_SelectCharacter_Selected.TrFormat(this._selectedCharacterIds.Count, this._config.TargetCount);
			this.selectedScroll.SetDataCount(this._selectedCharacterIds.Count);
		}

		// Token: 0x06005F24 RID: 24356 RVA: 0x002BA9E0 File Offset: 0x002B8BE0
		private void OnSelectedScrollItemRender(int index, GameObject item)
		{
			SelectedSlotItem slotItem = item.GetComponent<SelectedSlotItem>();
			bool flag = slotItem == null;
			if (!flag)
			{
				slotItem.RestoreRowItemReference();
				slotItem.SetSlotState(false);
				this.RenderSelectedCharacter(index, slotItem);
			}
		}

		// Token: 0x06005F25 RID: 24357 RVA: 0x002BAA1C File Offset: 0x002B8C1C
		private void RenderSelectedCharacter(int index, SelectedSlotItem slotItem)
		{
			int charId = this._selectedCharacterIds[index];
			ISelectCharacterData data = this._dataList.Find((ISelectCharacterData d) => d.CharacterId == charId);
			bool flag = data == null;
			if (!flag)
			{
				RowItem rowItem = slotItem.CurrentRowItem;
				bool flag2 = rowItem == null;
				if (!flag2)
				{
					List<ColumnDefinition> columnDefinitions = this.GenerateColumnDefinitions().ToList<ColumnDefinition>();
					rowItem.Init(columnDefinitions, true);
					rowItem.SetData(data, index < this._selectedCharacterIds.Count - 1, null);
					rowItem.SetSelected(true);
					rowItem.SetRowInteraction(true, index, new Action<int, RowItem>(this.OnClickSelectedRow));
				}
			}
		}

		// Token: 0x06005F26 RID: 24358 RVA: 0x002BAAC8 File Offset: 0x002B8CC8
		private void OnClickSelectedRow(int index, RowItem item)
		{
			bool flag = index < 0 || index >= this._selectedCharacterIds.Count;
			if (!flag)
			{
				this._selectedCharacterIds.RemoveAt(index);
				this.UpdateSelectedArea();
				this.UpdateButtonState();
				this.RefreshListData();
			}
		}

		// Token: 0x06005F27 RID: 24359 RVA: 0x002BAB18 File Offset: 0x002B8D18
		private void OnSelectedAreaSwitchClicked(bool activeSelectArea)
		{
			bool flag = this.selectedScrollArea != null;
			if (flag)
			{
				this.selectedScrollArea.SetActive(activeSelectArea);
			}
		}

		// Token: 0x06005F28 RID: 24360 RVA: 0x002BAB45 File Offset: 0x002B8D45
		private void OnSearchButtonClicked()
		{
			TMP_InputField tmp_InputField = this.searchInput;
			this._searchText = (((tmp_InputField != null) ? tmp_InputField.text : null) ?? string.Empty);
			this.RefreshListData();
		}

		// Token: 0x06005F29 RID: 24361 RVA: 0x002BAB70 File Offset: 0x002B8D70
		private void OnClickBtnClearSearch()
		{
			this._searchText = string.Empty;
			this.searchInput.text = this._searchText;
			this.RefreshListData();
			this.selectedScrollArea.SetActive(this._selectedCharacterIds.Count > 0);
			this.selectedAreaSwitchToggle.SetIsOnWithoutNotify(this._selectedCharacterIds.Count > 0);
		}

		// Token: 0x06005F2A RID: 24362 RVA: 0x002BABD6 File Offset: 0x002B8DD6
		private void OnSearchInputEndEdit(string text)
		{
			this._searchText = text;
			this.RefreshListData();
		}

		// Token: 0x06005F2B RID: 24363 RVA: 0x002BABE7 File Offset: 0x002B8DE7
		private void OnConfirmClicked()
		{
			SelectCharacterCallback callback = this._callback;
			if (callback != null)
			{
				callback(new List<int>(this._selectedCharacterIds));
			}
			base.QuickHide();
		}

		// Token: 0x06005F2C RID: 24364 RVA: 0x002BAC10 File Offset: 0x002B8E10
		public override void QuickHide()
		{
			bool flag = this._config.MinSelectionCount > 0;
			if (!flag)
			{
				Action cancelCallback = this._cancelCallback;
				if (cancelCallback != null)
				{
					cancelCallback();
				}
				base.QuickHide();
			}
		}

		// Token: 0x06005F2D RID: 24365 RVA: 0x002BAC4C File Offset: 0x002B8E4C
		private void UpdateButtonState()
		{
			this.btnFill.interactable = (this._selectedCharacterIds.Count + this._bannedCharacterIds.Count < this._filteredDataList.Count);
			this.btnClear.interactable = (this._selectedCharacterIds.Count > 0);
			this.btnFill.GetComponent<DisableStyleRoot>().SetStyleEffect(!this.btnFill.interactable, false);
			this.btnClear.GetComponent<DisableStyleRoot>().SetStyleEffect(!this.btnClear.interactable, false);
			bool flag = this.confirm == null;
			if (!flag)
			{
				bool flag2 = this._config.ConfirmInteractableChecker != null;
				if (flag2)
				{
					this.confirm.interactable = this._config.ConfirmInteractableChecker(this._selectedCharacterIds);
				}
				else
				{
					bool canConfirm = this._config.InteractionMode == ESelectCharacterInteractionMode.Slot || ((this._config.SelectionMode == ESelectCharacterSelectionMode.Single) ? (this._selectedCharacterIds.Count == 1) : (this._selectedCharacterIds.Count > 0));
					this.confirm.interactable = (canConfirm && this._selectedCharacterIds.Count >= this._config.MinSelectionCount);
				}
			}
		}

		// Token: 0x06005F2E RID: 24366 RVA: 0x002BADA0 File Offset: 0x002B8FA0
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
					bool flag3 = this._config.SelectionMode == ESelectCharacterSelectionMode.Multiple && this._selectedCharacterIds.Count >= this._config.TargetCount;
					result = (flag3 && !this._selectedCharacterIds.Contains(charId));
				}
			}
			return result;
		}

		// Token: 0x06005F2F RID: 24367 RVA: 0x002BAE38 File Offset: 0x002B9038
		public static void Show(CommonSelectCharacterConfig config, IReadOnlyList<ISelectCharacterData> dataList, SelectCharacterCallback selectCharacterCallback, Action cancelCallback = null, bool showInFront = false)
		{
			UIElement.SelectChar.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("SelectCharacterConfig", config).SetObject("SelectCharacterDataList", dataList).SetObject("SelectCharacterCallback", selectCharacterCallback).SetObject("SelectCharacterCancelCallback", cancelCallback).Set("ShowInFront", showInFront));
			UIManager.Instance.MaskUI(UIElement.SelectChar);
		}

		// Token: 0x17000BA1 RID: 2977
		// (get) Token: 0x06005F30 RID: 24368 RVA: 0x002BAEA0 File Offset: 0x002B90A0
		public static ColumnDefinition Favor
		{
			get
			{
				return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Favorability.Tr(), (CharacterDisplayDataForGeneralScrollList data) => ViewSelectCharacter.GetFavorDisplayString(data), 11, 30f, 90f);
			}
		}

		// Token: 0x17000BA2 RID: 2978
		// (get) Token: 0x06005F31 RID: 24369 RVA: 0x002BAEFC File Offset: 0x002B90FC
		public static ColumnDefinition Alertness
		{
			get
			{
				return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Alertness.Tr(), (CharacterDisplayDataForGeneralScrollList data) => CommonUtils.GetAlertnessNameByValue(data.Alertness), 130, 30f, 90f);
			}
		}

		// Token: 0x17000BA3 RID: 2979
		// (get) Token: 0x06005F32 RID: 24370 RVA: 0x002BAF5C File Offset: 0x002B915C
		public static ColumnDefinition Behavior
		{
			get
			{
				return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Behavior.Tr(), (CharacterDisplayDataForGeneralScrollList data) => CommonUtils.GetBehaviorString(data.BehaviorType), 57, 30f, 90f);
			}
		}

		// Token: 0x17000BA4 RID: 2980
		// (get) Token: 0x06005F33 RID: 24371 RVA: 0x002BAFB8 File Offset: 0x002B91B8
		public static ColumnDefinition Relationship
		{
			get
			{
				return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_RelationShip.Tr(), (CharacterDisplayDataForGeneralScrollList data) => CommonUtils.GetHighestPriorityRelationText(data.RelationToTaiwu, data.IsSameFactionWithTaiwu), 136, 30f, 90f);
			}
		}

		// Token: 0x17000BA5 RID: 2981
		// (get) Token: 0x06005F34 RID: 24372 RVA: 0x002BB018 File Offset: 0x002B9218
		public static ColumnDefinition Fame
		{
			get
			{
				return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Fame.Tr(), (CharacterDisplayDataForGeneralScrollList data) => CommonUtils.GetFameString(FameType.GetFameType(data.Fame)), 59, 30f, 90f);
			}
		}

		// Token: 0x17000BA6 RID: 2982
		// (get) Token: 0x06005F35 RID: 24373 RVA: 0x002BB074 File Offset: 0x002B9274
		public static ColumnDefinition CharacterIdentity
		{
			get
			{
				return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Identity.Tr(), (CharacterDisplayDataForGeneralScrollList data) => CommonUtils.GetOrganizationGradeString(data.OrgInfo, data.Gender, data.PhysiologicalAge, (int)data.CharacterTemplateId), 1, 30f, 90f);
			}
		}

		// Token: 0x17000BA7 RID: 2983
		// (get) Token: 0x06005F36 RID: 24374 RVA: 0x002BB0D0 File Offset: 0x002B92D0
		public static ColumnDefinition PreexistenceCharCount
		{
			get
			{
				return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Samsara.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.PreexistenceCharCount.ToString(), -1, 30f, 90f);
			}
		}

		// Token: 0x06005F37 RID: 24375 RVA: 0x002BB12B File Offset: 0x002B932B
		public static IEnumerable<ColumnDefinition> GenerateStateColumns()
		{
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Char_Age.Tr(), (CharacterDisplayDataForGeneralScrollList data) => ViewSelectCharacter.GetAgeDisplayString(data), 8, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Health.Tr(), (CharacterDisplayDataForGeneralScrollList data) => ViewSelectCharacter.GetHealthDisplayString(data), 10, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Injury.Tr(), (CharacterDisplayDataForGeneralScrollList data) => ViewSelectCharacter.FormatDisplayValue((int)data.DefeatMarkCount), 53, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Qi_Disorder.Tr(), (CharacterDisplayDataForGeneralScrollList data) => ViewSelectCharacter.FormatDisplayValue((int)data.DisorderOfQi), 55, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Charm.Tr(), (CharacterDisplayDataForGeneralScrollList data) => ViewSelectCharacter.GetCharmDisplayString(data), 9, 30f, 90f);
			yield return ViewSelectCharacter.Behavior;
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Happiness.Tr(), (CharacterDisplayDataForGeneralScrollList data) => CommonUtils.GetHappinessString(HappinessType.GetHappinessType(data.Happiness)), 12, 30f, 90f);
			yield return ViewSelectCharacter.Favor;
			yield return ViewSelectCharacter.Alertness;
			yield return ViewSelectCharacter.Fame;
			yield break;
		}

		// Token: 0x06005F38 RID: 24376 RVA: 0x002BB134 File Offset: 0x002B9334
		private static string GetCharmDisplayString(CharacterDisplayDataForGeneralScrollList data)
		{
			return CommonUtils.GetCharmLevelText(data.Charm, data.Gender, data.PhysiologicalAge, data.ClothDisplayId, CreatingType.IsFixedPresetType(data.CreatingType), data.FaceVisible);
		}

		// Token: 0x06005F39 RID: 24377 RVA: 0x002BB174 File Offset: 0x002B9374
		private static string GetFavorDisplayString(CharacterDisplayDataForGeneralScrollList data)
		{
			return CommonUtils.GetFavorStringByInteracted(data.FavorabilityToTaiwu, data.IsInteractedWithTaiwu);
		}

		// Token: 0x06005F3A RID: 24378 RVA: 0x002BB198 File Offset: 0x002B9398
		private static string GetAgeDisplayString(CharacterDisplayDataForGeneralScrollList data)
		{
			bool isNonEvolutionaryType = CreatingType.IsNonEvolutionaryType(data.CreatingType);
			return (Character.Instance[data.CharacterTemplateId].HideAge && isNonEvolutionaryType) ? "-" : data.PhysiologicalAge.ToString();
		}

		// Token: 0x06005F3B RID: 24379 RVA: 0x002BB1E4 File Offset: 0x002B93E4
		private static string GetHealthDisplayString(CharacterDisplayDataForGeneralScrollList data)
		{
			bool hideHealth = data.HideHealth;
			string result;
			if (hideHealth)
			{
				result = "-";
			}
			else
			{
				result = CommonUtils.GetCharacterHealthInfo(data.Health, data.MaxLeftHealth, data.CharacterId).Item1;
			}
			return result;
		}

		// Token: 0x06005F3C RID: 24380 RVA: 0x002BB224 File Offset: 0x002B9424
		private static string FormatDisplayValue(int value)
		{
			return (value < 0) ? "-" : value.ToString();
		}

		// Token: 0x06005F3D RID: 24381 RVA: 0x002BB248 File Offset: 0x002B9448
		private static IEnumerable<ColumnDefinition> GeneratePropertyColumns()
		{
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Strength.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.MaxMainAttributes[0].ToString(), 60, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Dexterity.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.MaxMainAttributes[1].ToString(), 61, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Concentration.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.MaxMainAttributes[2].ToString(), 62, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Vitality.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.MaxMainAttributes[3].ToString(), 63, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Energy.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.MaxMainAttributes[4].ToString(), 64, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Intelligence.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.MaxMainAttributes[5].ToString(), 65, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Penetrate_Outer.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Penetrations.Outer.ToString(), 22, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Penetrate_Inner.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Penetrations.Inner.ToString(), 23, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Penetrate_Resist_Outer.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.PenetrationResists.Outer.ToString(), 29, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Penetrate_Resist_Inner.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.PenetrationResists.Inner.ToString(), 30, 30f, 90f);
			yield break;
		}

		// Token: 0x06005F3E RID: 24382 RVA: 0x002BB251 File Offset: 0x002B9451
		private static IEnumerable<ColumnDefinition> GenerateProperty2Columns()
		{
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_HitType_0.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.HitValues[0].ToString(), 24, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_HitType_1.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.HitValues[1].ToString(), 25, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_HitType_2.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.HitValues[2].ToString(), 26, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_HitType_3.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.HitValues[3].ToString(), 27, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_AvoidType_0.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.AvoidValues[0].ToString(), 33, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_AvoidType_1.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.AvoidValues[1].ToString(), 34, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_AvoidType_2.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.AvoidValues[2].ToString(), 35, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_AvoidType_3.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.AvoidValues[3].ToString(), 36, 30f, 90f);
			yield break;
		}

		// Token: 0x06005F3F RID: 24383 RVA: 0x002BB25A File Offset: 0x002B945A
		private static IEnumerable<ColumnDefinition> GenerateLifeSkillColumns()
		{
			int num;
			for (int i = 0; i < 16; i = num + 1)
			{
				ViewSelectCharacter.<>c__DisplayClass124_0 CS$<>8__locals1 = new ViewSelectCharacter.<>c__DisplayClass124_0();
				CS$<>8__locals1.index = i;
				yield return ViewSelectCharacter.CreateTextColumn(() => LocalStringManager.Get(string.Format("LK_LifeSkillType_{0}", CS$<>8__locals1.index)), (CharacterDisplayDataForGeneralScrollList data) => data.LifeSkillQualifications[CS$<>8__locals1.index].ToString(), (short)(66 + CS$<>8__locals1.index), 40f, 60f);
				CS$<>8__locals1 = null;
				num = i;
			}
			yield break;
		}

		// Token: 0x06005F40 RID: 24384 RVA: 0x002BB263 File Offset: 0x002B9463
		private static IEnumerable<ColumnDefinition> GenerateCombatSkillColumns()
		{
			int num;
			for (int i = 0; i < 14; i = num + 1)
			{
				ViewSelectCharacter.<>c__DisplayClass125_0 CS$<>8__locals1 = new ViewSelectCharacter.<>c__DisplayClass125_0();
				CS$<>8__locals1.index = i;
				yield return ViewSelectCharacter.CreateTextColumn(() => LocalStringManager.Get(string.Format("LK_CombatSkillType_{0}", CS$<>8__locals1.index)), (CharacterDisplayDataForGeneralScrollList data) => data.CombatSkillQualifications[CS$<>8__locals1.index].ToString(), (short)(82 + CS$<>8__locals1.index), 40f, 60f);
				CS$<>8__locals1 = null;
				num = i;
			}
			yield break;
		}

		// Token: 0x06005F41 RID: 24385 RVA: 0x002BB26C File Offset: 0x002B946C
		private static IEnumerable<ColumnDefinition> GeneratePersonalityColumns()
		{
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Personality_Calm_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[0].ToString(), 96, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Personality_Clever_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[1].ToString(), 97, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Personality_Enthusiastic_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[2].ToString(), 98, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Personality_Brave_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[3].ToString(), 99, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Personality_Firm_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[4].ToString(), 100, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Personality_Lucky_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[5].ToString(), 101, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Personality_Perceptive_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[6].ToString(), 102, 30f, 90f);
			yield break;
		}

		// Token: 0x06005F42 RID: 24386 RVA: 0x002BB275 File Offset: 0x002B9475
		private static IEnumerable<ColumnDefinition> GenerateItemColumns()
		{
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Food.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Resources[0].ToString(), 103, 40f, 60f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Wood.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Resources[1].ToString(), 104, 40f, 60f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Metal.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Resources[2].ToString(), 105, 40f, 60f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Jade.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Resources[3].ToString(), 106, 40f, 60f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Fabric.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Resources[4].ToString(), 107, 40f, 60f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Herb.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Resources[5].ToString(), 108, 40f, 60f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Money.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Resources[6].ToString(), 109, 40f, 60f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Authority.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Resources[7].ToString(), 110, 40f, 60f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Inventory.Tr(), (CharacterDisplayDataForGeneralScrollList data) => string.Format("{0:f1}/{1:f1}", (float)data.CurrInventoryLoad / 100f, (float)data.MaxInventoryLoad / 100f), 37, 30f, 90f);
			yield break;
		}

		// Token: 0x06005F43 RID: 24387 RVA: 0x002BB27E File Offset: 0x002B947E
		private static IEnumerable<ColumnDefinition> GenerateCommandColumns()
		{
			yield return ViewSelectCharacter.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Attack.Tr(), (ISelectCharacterData data) => ViewSelectCharacter.CreateMedalCellData(data.GetGeneralScrollListData().AttackMedal, 0), 112, 30f, 90f);
			yield return ViewSelectCharacter.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Defence.Tr(), (ISelectCharacterData data) => ViewSelectCharacter.CreateMedalCellData(data.GetGeneralScrollListData().DefenceMedal, 1), 113, 30f, 90f);
			yield return ViewSelectCharacter.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Wisdom.Tr(), (ISelectCharacterData data) => ViewSelectCharacter.CreateMedalCellData(data.GetGeneralScrollListData().WisdomMedal, 2), 114, 30f, 90f);
			yield return ViewSelectCharacter.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_0.Tr(), (ISelectCharacterData data) => ViewSelectCharacter.CreateCommandCellData(data, 0), 115, 30f, 90f);
			yield return ViewSelectCharacter.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_1.Tr(), (ISelectCharacterData data) => ViewSelectCharacter.CreateCommandCellData(data, 1), 116, 30f, 90f);
			yield return ViewSelectCharacter.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_2.Tr(), (ISelectCharacterData data) => ViewSelectCharacter.CreateCommandCellData(data, 2), 117, 30f, 90f);
			yield break;
		}

		// Token: 0x06005F44 RID: 24388 RVA: 0x002BB287 File Offset: 0x002B9487
		private IEnumerable<ColumnDefinition> GenerateYuanshanInfectionColumns()
		{
			ColumnDefinition<ISelectCharacterData, string> columnDefinition = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_SelectCharacter_Column_Infect.Tr());
			columnDefinition.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				YuanshanSelectCharacterDataAdapter adapter = data as YuanshanSelectCharacterDataAdapter;
				bool flag = adapter != null;
				string result;
				if (flag)
				{
					result = adapter.Data.Infection.ToString();
				}
				else
				{
					result = "-";
				}
				return result;
			};
			columnDefinition.SortId = 123;
			yield return columnDefinition;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition2 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition2.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition2.TableHeadLabel = (() => LanguageKey.LK_RelationShip.Tr());
			columnDefinition2.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				CharacterDisplayDataForGeneralScrollList generalData = data.GetGeneralScrollListData();
				bool flag = generalData == null;
				string result;
				if (flag)
				{
					result = "-";
				}
				else
				{
					bool flag2 = generalData.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
					if (flag2)
					{
						result = "-";
					}
					else
					{
						result = ViewSelectCharacter.GetHighestPriorityRelationText(generalData.RelationToTaiwu, generalData.IsSameFactionWithTaiwu);
					}
				}
				return result;
			};
			yield return columnDefinition2;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition3 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition3.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition3.TableHeadLabel = (() => LanguageKey.LK_Favorability.Tr());
			columnDefinition3.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				CharacterDisplayDataForGeneralScrollList generalData = data.GetGeneralScrollListData();
				return CommonUtils.GetFavorStringByInteracted(generalData.FavorabilityToTaiwu, generalData.IsInteractedWithTaiwu);
			};
			columnDefinition3.SortId = 11;
			yield return columnDefinition3;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition4 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition4.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition4.TableHeadLabel = (() => LanguageKey.LK_Main_SummaryInfo_Behavior.Tr());
			columnDefinition4.CellDataGenerator = ((ISelectCharacterData data) => CommonUtils.GetBehaviorString(data.GetGeneralScrollListData().BehaviorType));
			columnDefinition4.SortId = 57;
			yield return columnDefinition4;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition5 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition5.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition5.TableHeadLabel = (() => LanguageKey.LK_Main_SummaryInfo_Fame.Tr());
			columnDefinition5.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				CharacterDisplayDataForGeneralScrollList generalData = data.GetGeneralScrollListData();
				return CommonUtils.GetFameString(FameType.GetFameType(generalData.Fame));
			};
			columnDefinition5.SortId = 59;
			yield return columnDefinition5;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition6 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition6.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition6.TableHeadLabel = (() => LanguageKey.LK_Main_SummaryInfo_Identity.Tr());
			columnDefinition6.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				CharacterDisplayDataForGeneralScrollList generalData = data.GetGeneralScrollListData();
				return CommonUtils.GetIdentityStringWithSpecialCharacterConfig((int)generalData.CharacterTemplateId, generalData.OrgInfo, generalData.Gender, generalData.PhysiologicalAge, false);
			};
			columnDefinition6.SortId = 1;
			yield return columnDefinition6;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition7 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition7.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition7.TableHeadLabel = (() => LanguageKey.LK_Main_SummaryInfo_Organization.Tr());
			columnDefinition7.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				CharacterDisplayDataForGeneralScrollList generalData = data.GetGeneralScrollListData();
				string text;
				return CommonUtils.TryGetCharacterSpecialGradeName((int)generalData.CharacterTemplateId, out text) ? "-" : SingletonObject.getInstance<WorldMapModel>().GetSettlementName(generalData.OrgInfo);
			};
			yield return columnDefinition7;
			yield break;
		}

		// Token: 0x06005F45 RID: 24389 RVA: 0x002BB297 File Offset: 0x002B9497
		private static IEnumerable<ColumnDefinition> GenerateApproveRateColumns()
		{
			yield return ViewSelectCharacter.CreateGenericTextColumn<OrganizationMemberDisplayDataForGeneralScrollList>(() => LanguageKey.LK_Organization_Approving.Tr(), (OrganizationMemberDisplayDataForGeneralScrollList data) => string.Format("{0:f01}%", (float)data.ApprovingRate / 10f), 178, 30f, 90f);
			yield return ViewSelectCharacter.Relationship;
			yield return ViewSelectCharacter.Favor;
			yield return ViewSelectCharacter.Alertness;
			yield return ViewSelectCharacter.Behavior;
			yield return ViewSelectCharacter.Fame;
			yield return ViewSelectCharacter.CharacterIdentity;
			yield return ViewSelectCharacter.CreateGenericTextColumn<OrganizationMemberDisplayDataForGeneralScrollList>(() => LanguageKey.LK_VillagerInfo_InfluencePower.Tr(), (OrganizationMemberDisplayDataForGeneralScrollList data) => data.InfluencePower.ToString(), 137, 30f, 90f);
			yield break;
		}

		// Token: 0x06005F46 RID: 24390 RVA: 0x002BB2A0 File Offset: 0x002B94A0
		public static IEnumerable<ColumnDefinition> GenerateAssignRolePeasantColumns()
		{
			yield return ViewSelectCharacter.CharacterIdentity;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_VillagerInfo_WorkingStatus.Tr());
			columnDefinition.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				VillagerSelectCharacterDataAdapter villagerData = data as VillagerSelectCharacterDataAdapter;
				bool flag = villagerData != null;
				string result;
				if (flag)
				{
					result = ViewSelectCharacter.GetWorkStatusText(villagerData.GetRawData());
				}
				else
				{
					result = "";
				}
				return result;
			};
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				PreferredWidth = 100f,
				FlexibleWidth = 1f
			};
			yield return columnDefinition;
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Personality_Perceptive_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[6].ToString(), 102, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LocalStringManager.Get(LanguageKey.LK_LifeSkillType_14.Tr()), (CharacterDisplayDataForGeneralScrollList data) => data.LifeSkillQualifications[14].ToString(), 80, 40f, 60f);
			yield break;
		}

		// Token: 0x06005F47 RID: 24391 RVA: 0x002BB2A9 File Offset: 0x002B94A9
		public static IEnumerable<ColumnDefinition> GenerateAssignRoleCraftsmanColumns()
		{
			yield return ViewSelectCharacter.CharacterIdentity;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_VillagerInfo_WorkingStatus.Tr());
			columnDefinition.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				VillagerSelectCharacterDataAdapter villagerData = data as VillagerSelectCharacterDataAdapter;
				bool flag = villagerData != null;
				string result;
				if (flag)
				{
					result = ViewSelectCharacter.GetWorkStatusText(villagerData.GetRawData());
				}
				else
				{
					result = "";
				}
				return result;
			};
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				PreferredWidth = 100f,
				FlexibleWidth = 1f
			};
			yield return columnDefinition;
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Personality_Firm_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[4].ToString(), 100, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LocalStringManager.Get(LanguageKey.LK_LifeSkillType_6.Tr()), (CharacterDisplayDataForGeneralScrollList data) => data.LifeSkillQualifications[6].ToString(), 72, 40f, 60f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LocalStringManager.Get(LanguageKey.LK_LifeSkillType_7.Tr()), (CharacterDisplayDataForGeneralScrollList data) => data.LifeSkillQualifications[7].ToString(), 73, 40f, 60f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LocalStringManager.Get(LanguageKey.LK_LifeSkillType_10.Tr()), (CharacterDisplayDataForGeneralScrollList data) => data.LifeSkillQualifications[10].ToString(), 76, 40f, 60f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LocalStringManager.Get(LanguageKey.LK_LifeSkillType_11.Tr()), (CharacterDisplayDataForGeneralScrollList data) => data.LifeSkillQualifications[11].ToString(), 77, 40f, 60f);
			yield break;
		}

		// Token: 0x06005F48 RID: 24392 RVA: 0x002BB2B2 File Offset: 0x002B94B2
		public static IEnumerable<ColumnDefinition> GenerateAssignRoleDoctorColumns()
		{
			yield return ViewSelectCharacter.CharacterIdentity;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_VillagerInfo_WorkingStatus.Tr());
			columnDefinition.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				VillagerSelectCharacterDataAdapter villagerData = data as VillagerSelectCharacterDataAdapter;
				bool flag = villagerData != null;
				string result;
				if (flag)
				{
					result = ViewSelectCharacter.GetWorkStatusText(villagerData.GetRawData());
				}
				else
				{
					result = "";
				}
				return result;
			};
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				PreferredWidth = 100f,
				FlexibleWidth = 1f
			};
			yield return columnDefinition;
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Personality_Calm_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[0].ToString(), 96, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LocalStringManager.Get(LanguageKey.LK_LifeSkillType_8.Tr()), (CharacterDisplayDataForGeneralScrollList data) => data.LifeSkillQualifications[8].ToString(), 74, 40f, 60f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LocalStringManager.Get(LanguageKey.LK_LifeSkillType_9.Tr()), (CharacterDisplayDataForGeneralScrollList data) => data.LifeSkillQualifications[9].ToString(), 75, 40f, 60f);
			yield break;
		}

		// Token: 0x06005F49 RID: 24393 RVA: 0x002BB2BB File Offset: 0x002B94BB
		public static IEnumerable<ColumnDefinition> GenerateAssignRoleMerchantColumns()
		{
			yield return ViewSelectCharacter.CharacterIdentity;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_VillagerInfo_WorkingStatus.Tr());
			columnDefinition.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				VillagerSelectCharacterDataAdapter villagerData = data as VillagerSelectCharacterDataAdapter;
				bool flag = villagerData != null;
				string result;
				if (flag)
				{
					result = ViewSelectCharacter.GetWorkStatusText(villagerData.GetRawData());
				}
				else
				{
					result = "";
				}
				return result;
			};
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				PreferredWidth = 100f,
				FlexibleWidth = 1f
			};
			yield return columnDefinition;
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Personality_Enthusiastic_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[2].ToString(), 98, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LocalStringManager.Get(LanguageKey.LK_LifeSkillType_15.Tr()), (CharacterDisplayDataForGeneralScrollList data) => data.LifeSkillQualifications[15].ToString(), 81, 40f, 60f);
			yield break;
		}

		// Token: 0x06005F4A RID: 24394 RVA: 0x002BB2C4 File Offset: 0x002B94C4
		public static IEnumerable<ColumnDefinition> GenerateAssignRoleLiteratiColumns()
		{
			yield return ViewSelectCharacter.CharacterIdentity;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_VillagerInfo_WorkingStatus.Tr());
			columnDefinition.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				VillagerSelectCharacterDataAdapter villagerData = data as VillagerSelectCharacterDataAdapter;
				bool flag = villagerData != null;
				string result;
				if (flag)
				{
					result = ViewSelectCharacter.GetWorkStatusText(villagerData.GetRawData());
				}
				else
				{
					result = "";
				}
				return result;
			};
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				PreferredWidth = 100f,
				FlexibleWidth = 1f
			};
			yield return columnDefinition;
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Personality_Clever_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[1].ToString(), 97, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LocalStringManager.Get(LanguageKey.LK_LifeSkillType_5.Tr()), (CharacterDisplayDataForGeneralScrollList data) => data.LifeSkillQualifications[5].ToString(), 71, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LocalStringManager.Get(LanguageKey.LK_LifeSkillType_0.Tr()), (CharacterDisplayDataForGeneralScrollList data) => data.LifeSkillQualifications[0].ToString(), 66, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LocalStringManager.Get(LanguageKey.LK_LifeSkillType_1.Tr()), (CharacterDisplayDataForGeneralScrollList data) => data.LifeSkillQualifications[1].ToString(), 67, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LocalStringManager.Get(LanguageKey.LK_LifeSkillType_2.Tr()), (CharacterDisplayDataForGeneralScrollList data) => data.LifeSkillQualifications[2].ToString(), 68, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LocalStringManager.Get(LanguageKey.LK_LifeSkillType_3.Tr()), (CharacterDisplayDataForGeneralScrollList data) => data.LifeSkillQualifications[3].ToString(), 69, 30f, 90f);
			yield break;
		}

		// Token: 0x06005F4B RID: 24395 RVA: 0x002BB2CD File Offset: 0x002B94CD
		public static IEnumerable<ColumnDefinition> GenerateAssignRoleTombKeeperColumns()
		{
			yield return ViewSelectCharacter.CharacterIdentity;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_VillagerInfo_WorkingStatus.Tr());
			columnDefinition.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				VillagerSelectCharacterDataAdapter villagerData = data as VillagerSelectCharacterDataAdapter;
				bool flag = villagerData != null;
				string result;
				if (flag)
				{
					result = ViewSelectCharacter.GetWorkStatusText(villagerData.GetRawData());
				}
				else
				{
					result = "";
				}
				return result;
			};
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				PreferredWidth = 100f,
				FlexibleWidth = 1f
			};
			yield return columnDefinition;
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Personality_Brave_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[3].ToString(), 99, 30f, 90f);
			yield return ViewSelectCharacter.CreateIconAndTextColumn(() => LocalStringManager.Get(LanguageKey.LK_CombatSkill_2.Tr()), (ISelectCharacterData data) => ViewSelectCharacter.CreateCombatSkillCompareCellData(data), 208, 40f, 60f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LocalStringManager.Get(LanguageKey.LK_LifeSkillType_13.Tr()), (CharacterDisplayDataForGeneralScrollList data) => data.LifeSkillQualifications[13].ToString(), 79, 40f, 60f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LocalStringManager.Get(LanguageKey.LK_LifeSkillType_12.Tr()), (CharacterDisplayDataForGeneralScrollList data) => data.LifeSkillQualifications[12].ToString(), 78, 40f, 60f);
			yield break;
		}

		// Token: 0x06005F4C RID: 24396 RVA: 0x002BB2D6 File Offset: 0x002B94D6
		public static IEnumerable<ColumnDefinition> GenerateAssignRoleEnvoyColumns()
		{
			yield return ViewSelectCharacter.CharacterIdentity;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_VillagerInfo_WorkingStatus.Tr());
			columnDefinition.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				VillagerSelectCharacterDataAdapter villagerData = data as VillagerSelectCharacterDataAdapter;
				bool flag = villagerData != null;
				string result;
				if (flag)
				{
					result = ViewSelectCharacter.GetWorkStatusText(villagerData.GetRawData());
				}
				else
				{
					result = "";
				}
				return result;
			};
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				PreferredWidth = 100f,
				FlexibleWidth = 1f
			};
			yield return columnDefinition;
			yield return ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Personality_Lucky_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[5].ToString(), 101, 30f, 90f);
			yield return ViewSelectCharacter.CreateTextColumn(() => LocalStringManager.Get(LanguageKey.LK_LifeSkillType_4.Tr()), (CharacterDisplayDataForGeneralScrollList data) => data.LifeSkillQualifications[4].ToString(), 70, 40f, 60f);
			yield break;
		}

		// Token: 0x06005F4D RID: 24397 RVA: 0x002BB2DF File Offset: 0x002B94DF
		public unsafe static IEnumerable<ColumnDefinition> GenerateBaihuaColumns()
		{
			yield return ViewSelectCharacter.CharacterIdentity;
			yield return ViewSelectCharacter.Relationship;
			yield return ViewSelectCharacter.Favor;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_SelectCharacter_NeiliType_Metal.Tr());
			columnDefinition.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				BaihuaLifeLinkSelectCharacterDataAdapter adapter = data as BaihuaLifeLinkSelectCharacterDataAdapter;
				bool flag = adapter != null;
				string result;
				if (flag)
				{
					CharacterDisplayDataForLifeLink baihuaData = adapter.GetRawData();
					result = string.Format("{0}%", *baihuaData.NeiliPercent[0]);
				}
				else
				{
					result = "-";
				}
				return result;
			};
			columnDefinition.SortId = 218;
			yield return columnDefinition;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition2 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition2.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition2.TableHeadLabel = (() => LanguageKey.LK_SelectCharacter_NeiliType_Wood.Tr());
			columnDefinition2.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				BaihuaLifeLinkSelectCharacterDataAdapter adapter = data as BaihuaLifeLinkSelectCharacterDataAdapter;
				bool flag = adapter != null;
				string result;
				if (flag)
				{
					CharacterDisplayDataForLifeLink baihuaData = adapter.GetRawData();
					result = string.Format("{0}%", *baihuaData.NeiliPercent[1]);
				}
				else
				{
					result = "-";
				}
				return result;
			};
			columnDefinition2.SortId = 219;
			yield return columnDefinition2;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition3 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition3.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition3.TableHeadLabel = (() => LanguageKey.LK_SelectCharacter_NeiliType_Water.Tr());
			columnDefinition3.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				BaihuaLifeLinkSelectCharacterDataAdapter adapter = data as BaihuaLifeLinkSelectCharacterDataAdapter;
				bool flag = adapter != null;
				string result;
				if (flag)
				{
					CharacterDisplayDataForLifeLink baihuaData = adapter.GetRawData();
					result = string.Format("{0}%", *baihuaData.NeiliPercent[2]);
				}
				else
				{
					result = "-";
				}
				return result;
			};
			columnDefinition3.SortId = 220;
			yield return columnDefinition3;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition4 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition4.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition4.TableHeadLabel = (() => LanguageKey.LK_SelectCharacter_NeiliType_Fire.Tr());
			columnDefinition4.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				BaihuaLifeLinkSelectCharacterDataAdapter adapter = data as BaihuaLifeLinkSelectCharacterDataAdapter;
				bool flag = adapter != null;
				string result;
				if (flag)
				{
					CharacterDisplayDataForLifeLink baihuaData = adapter.GetRawData();
					result = string.Format("{0}%", *baihuaData.NeiliPercent[3]);
				}
				else
				{
					result = "-";
				}
				return result;
			};
			columnDefinition4.SortId = 221;
			yield return columnDefinition4;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition5 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition5.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition5.TableHeadLabel = (() => LanguageKey.LK_SelectCharacter_NeiliType_Earth.Tr());
			columnDefinition5.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				BaihuaLifeLinkSelectCharacterDataAdapter adapter = data as BaihuaLifeLinkSelectCharacterDataAdapter;
				bool flag = adapter != null;
				string result;
				if (flag)
				{
					CharacterDisplayDataForLifeLink baihuaData = adapter.GetRawData();
					result = string.Format("{0}%", *baihuaData.NeiliPercent[4]);
				}
				else
				{
					result = "-";
				}
				return result;
			};
			columnDefinition5.SortId = 222;
			yield return columnDefinition5;
			ColumnDefinition<ISelectCharacterData, string> columnDefinition6 = new ColumnDefinition<ISelectCharacterData, string>();
			columnDefinition6.TableHeadLabel = (() => LanguageKey.LK_Health.Tr());
			columnDefinition6.CellDataGenerator = delegate(ISelectCharacterData data)
			{
				CharacterDisplayDataForGeneralScrollList genData = data.GetGeneralScrollListData();
				return (genData != null) ? ViewSelectCharacter.GetHealthDisplayString(genData) : "";
			};
			columnDefinition6.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition6.SortId = 10;
			yield return columnDefinition6;
			yield break;
		}

		// Token: 0x06005F4E RID: 24398 RVA: 0x002BB2E8 File Offset: 0x002B94E8
		public static string GetWorkStatusText(VillagerSelectCharacterDisplayData data)
		{
			CharacterTableLocationData locationData = data.LocationData;
			string result;
			switch (data.WorkStatus)
			{
			case 0:
				result = LanguageKey.LK_Villager_WorkStatus_Unemployed.Tr().SetColor("brightblue");
				break;
			case 1:
				result = LanguageKey.LK_Villager_WorkStatus_InTaiwuGroup.Tr().SetColor("brightred");
				break;
			case 2:
			{
				bool flag = data.ArrangementTemplateId >= 0;
				if (flag)
				{
					string workName = VillagerRoleArrangement.Instance[data.ArrangementTemplateId].ShortName;
					string locationName = (data.ArrangementTemplateId == 13 && data.SwordTombId >= 0) ? LocalStringManager.Get(string.Format("LK_SwordTomb_{0}", data.SwordTombId)) : ((locationData.AreaTemplateId >= 0) ? MapArea.Instance[locationData.AreaTemplateId].Name : "");
					result = LanguageKey.LK_Villager_WorkStatus_Working.TrFormat(locationName, workName).SetColor("brightred");
				}
				else
				{
					bool flag2 = data.WorkType == 1;
					if (flag2)
					{
						result = (data.IsBuyOperation ? LanguageKey.LK_VillagerSelection_Crafting.Tr() : LanguageKey.LK_VillagerSelection_Managing.Tr());
					}
					else
					{
						bool flag3 = data.WorkType == 0;
						if (flag3)
						{
							result = (data.IsBuyOperation ? LanguageKey.LK_VillagerSelection_Building.Tr() : LanguageKey.LK_VillagerSelection_Removing.Tr());
						}
						else
						{
							string locationName2 = SingletonObject.getInstance<WorldMapModel>().GetBlockName(locationData.AreaId, locationData.BlockId, locationData.BlockTemplateId, -1);
							string workName2 = LocalStringManager.Get(string.Format("LK_WorkType_{0}", data.WorkType));
							result = LanguageKey.LK_Villager_WorkStatus_Working.TrFormat(locationName2, workName2).SetColor("brightred");
						}
					}
				}
				break;
			}
			case 3:
				result = LanguageKey.LK_Villager_WorkStatus_NotOldEnough.Tr().SetColor("brightred");
				break;
			case 4:
				result = LanguageKey.LK_ResidentState_Infected.Tr().SetColor("brightred");
				break;
			case 5:
				result = LanguageKey.LK_Villager_WorkStatus_ProtectingTaiwuVillage.Tr().SetColor("brightred");
				break;
			default:
				result = "";
				break;
			}
			return result;
		}

		// Token: 0x06005F4F RID: 24399 RVA: 0x002BB510 File Offset: 0x002B9710
		private static string GetAgeDisplayString(ISelectCharacterData data)
		{
			CharacterDisplayDataForGeneralScrollList genData = data.GetGeneralScrollListData();
			bool flag = genData == null;
			string result;
			if (flag)
			{
				result = "";
			}
			else
			{
				bool isNonEvolutionaryType = CreatingType.IsNonEvolutionaryType(genData.CreatingType);
				result = ((Character.Instance[genData.CharacterTemplateId].HideAge && isNonEvolutionaryType) ? "-" : genData.PhysiologicalAge.ToString());
			}
			return result;
		}

		// Token: 0x06005F50 RID: 24400 RVA: 0x002BB578 File Offset: 0x002B9778
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
				string iconName = ViewSelectCharacter.GetMedalIconName(medalCount, medalType);
				string text = string.Format(" x{0}", Mathf.Abs(medalCount));
				result = new IconAndTextCellData(iconName, text, true, false, false, false);
			}
			return result;
		}

		// Token: 0x06005F51 RID: 24401 RVA: 0x002BB5CC File Offset: 0x002B97CC
		private unsafe static IconAndTextCellData CreateLifeSkillCompareCellData(ISelectCharacterData data, params sbyte[] lifeSkills)
		{
			int valueResult = 0;
			sbyte skillResult = lifeSkills[0];
			for (int i = 0; i < lifeSkills.Length; i++)
			{
				short newValue = *data.GetGeneralScrollListData().LifeSkillQualifications[(int)lifeSkills[i]];
				bool flag = (int)newValue >= valueResult;
				if (flag)
				{
					skillResult = lifeSkills[i];
					valueResult = (int)newValue;
				}
			}
			string iconName = Config.LifeSkillType.Instance[skillResult].DisplayIcon;
			string text = valueResult.ToString();
			return new IconAndTextCellData(iconName, text, true, false, false, false);
		}

		// Token: 0x06005F52 RID: 24402 RVA: 0x002BB654 File Offset: 0x002B9854
		private unsafe static IconAndTextCellData CreateCombatSkillCompareCellData(ISelectCharacterData data)
		{
			int valueResult = 0;
			int skillResult = 0;
			for (int i = 0; i < 14; i++)
			{
				short newValue = *data.GetGeneralScrollListData().CombatSkillQualifications[i];
				bool flag = (int)newValue >= valueResult;
				if (flag)
				{
					skillResult = i;
					valueResult = (int)newValue;
				}
			}
			string iconName = CombatSkillType.Instance[skillResult].DisplayIcon;
			string text = valueResult.ToString();
			return new IconAndTextCellData(iconName, text, true, false, false, false);
		}

		// Token: 0x06005F53 RID: 24403 RVA: 0x002BB6D4 File Offset: 0x002B98D4
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

		// Token: 0x06005F54 RID: 24404 RVA: 0x002BB754 File Offset: 0x002B9954
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
					TeammateCommandItem cmdConfig = Config.TeammateCommand.Instance[commandId];
					result = IconAndTextCellData.TextOnly(cmdConfig.Name);
				}
			}
			return result;
		}

		// Token: 0x06005F55 RID: 24405 RVA: 0x002BB7E8 File Offset: 0x002B99E8
		private static string GetHighestPriorityRelationText(ushort relationToTaiwu, bool isSameFaction)
		{
			bool flag = RelationType.ContainParentRelations(relationToTaiwu);
			string result;
			if (flag)
			{
				result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Parent);
			}
			else
			{
				bool flag2 = RelationType.ContainChildRelations(relationToTaiwu);
				if (flag2)
				{
					result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Child);
				}
				else
				{
					bool flag3 = RelationType.ContainBrotherOrSisterRelations(relationToTaiwu);
					if (flag3)
					{
						result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Bro);
					}
					else
					{
						bool flag4 = RelationType.HasRelation(relationToTaiwu, 1024);
						if (flag4)
						{
							result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Wife);
						}
						else
						{
							bool flag5 = RelationType.HasRelation(relationToTaiwu, 32768);
							if (flag5)
							{
								result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Enemy);
							}
							else
							{
								bool flag6 = RelationType.HasRelation(relationToTaiwu, 16384);
								if (flag6)
								{
									result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Adored);
								}
								else
								{
									bool flag7 = RelationType.HasRelation(relationToTaiwu, 2048) || RelationType.HasRelation(relationToTaiwu, 4096);
									if (flag7)
									{
										result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Mentor);
									}
									else
									{
										bool flag8 = RelationType.HasRelation(relationToTaiwu, 512);
										if (flag8)
										{
											result = LocalStringManager.Get(LanguageKey.LK_RelationShip_SwornBro);
										}
										else
										{
											bool flag9 = RelationType.HasRelation(relationToTaiwu, 8192);
											if (flag9)
											{
												result = LocalStringManager.Get(LanguageKey.LK_RelationShip_Friend);
											}
											else if (isSameFaction)
											{
												result = LocalStringManager.Get(LanguageKey.LK_Faction);
											}
											else
											{
												result = "-";
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x040041C6 RID: 16838
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

		// Token: 0x040041C7 RID: 16839
		[SerializeField]
		private TextMeshProUGUI titleLabel;

		// Token: 0x040041C8 RID: 16840
		[SerializeField]
		private CToggleGroup subPageToggleGroup;

		// Token: 0x040041C9 RID: 16841
		[SerializeField]
		private CToggle subPageToggleTemplate;

		// Token: 0x040041CA RID: 16842
		[SerializeField]
		private ListStyleGeneralScroll scroll;

		// Token: 0x040041CB RID: 16843
		[SerializeField]
		private CButton confirm;

		// Token: 0x040041CC RID: 16844
		[SerializeField]
		private CButton close;

		// Token: 0x040041CD RID: 16845
		[SerializeField]
		private GameObject selectedScrollArea;

		// Token: 0x040041CE RID: 16846
		[SerializeField]
		private InfinityScroll selectedScroll;

		// Token: 0x040041CF RID: 16847
		[SerializeField]
		private SortAndFilter sortAndFilter;

		// Token: 0x040041D0 RID: 16848
		[SerializeField]
		private CButton btnFill;

		// Token: 0x040041D1 RID: 16849
		[SerializeField]
		private CButton btnClear;

		// Token: 0x040041D2 RID: 16850
		[Header("底部信息提示文本")]
		[SerializeField]
		private TextMeshProUGUI customTextLabel;

		// Token: 0x040041D3 RID: 16851
		[SerializeField]
		private GameObject customTextArea;

		// Token: 0x040041D4 RID: 16852
		[Header("行模板配置")]
		[SerializeField]
		private RowItem rowTemplate;

		// Token: 0x040041D5 RID: 16853
		[SerializeField]
		private RowCellContainer avatarAndNameCellContainer;

		// Token: 0x040041D6 RID: 16854
		[SerializeField]
		private RowCellContainer singleTextCellContainer;

		// Token: 0x040041D7 RID: 16855
		[SerializeField]
		private RowCellContainer iconAndTextCellContainer;

		// Token: 0x040041D8 RID: 16856
		[SerializeField]
		private RowCellContainer readProgressCellContainer;

		// Token: 0x040041D9 RID: 16857
		[SerializeField]
		private TMP_InputField searchInput;

		// Token: 0x040041DA RID: 16858
		[SerializeField]
		private CButton searchButton;

		// Token: 0x040041DB RID: 16859
		[SerializeField]
		private CButton btnClearSearch;

		// Token: 0x040041DC RID: 16860
		[SerializeField]
		private TextMeshProUGUI selectedCountLabel;

		// Token: 0x040041DD RID: 16861
		[Header("已选列表配置")]
		[SerializeField]
		private CToggle selectedAreaSwitchToggle;

		// Token: 0x040041DE RID: 16862
		[SerializeField]
		private SelectedSlotItem selectedSlotTemplate;

		// Token: 0x040041DF RID: 16863
		[SerializeField]
		private int sortingLayerNormal = 699;

		// Token: 0x040041E0 RID: 16864
		[SerializeField]
		private int sortingLayerFront = 710;

		// Token: 0x040041E1 RID: 16865
		[SerializeField]
		private int sortingLayerBack = 599;

		// Token: 0x040041E2 RID: 16866
		[SerializeField]
		private Canvas canvas;

		// Token: 0x040041E3 RID: 16867
		private Dictionary<int, ESelectCharacterSubPage> _subpageIndexDic = new Dictionary<int, ESelectCharacterSubPage>();

		// Token: 0x040041E4 RID: 16868
		private CommonSelectCharacterConfig _config;

		// Token: 0x040041E5 RID: 16869
		private readonly List<ISelectCharacterData> _dataList = new List<ISelectCharacterData>();

		// Token: 0x040041E6 RID: 16870
		private readonly List<ISelectCharacterData> _filteredDataList = new List<ISelectCharacterData>();

		// Token: 0x040041E7 RID: 16871
		private readonly List<int> _selectedCharacterIds = new List<int>();

		// Token: 0x040041E8 RID: 16872
		private readonly HashSet<int> _bannedCharacterIds = new HashSet<int>();

		// Token: 0x040041E9 RID: 16873
		private SelectCharacterSortAndFilterController _sortAndFilterController;

		// Token: 0x040041EA RID: 16874
		private TabSortStateManager<int, ISelectCharacterData> _tabSortStateManager;

		// Token: 0x040041EB RID: 16875
		private SelectCharacterCallback _callback;

		// Token: 0x040041EC RID: 16876
		private Action _cancelCallback;

		// Token: 0x040041ED RID: 16877
		private ESelectCharacterSubPage _currentSubPage = ESelectCharacterSubPage.State;

		// Token: 0x040041EE RID: 16878
		private bool _listStructureReady;

		// Token: 0x040041EF RID: 16879
		private CToggle _dynamicToggle;

		// Token: 0x040041F0 RID: 16880
		private string _searchText = string.Empty;

		// Token: 0x040041F1 RID: 16881
		private readonly Dictionary<int, RowItem> _rowTemplateCache = new Dictionary<int, RowItem>();

		// Token: 0x040041F2 RID: 16882
		private bool _pendingInitialize;

		// Token: 0x040041F3 RID: 16883
		private bool _uiInitialized;

		// Token: 0x040041F4 RID: 16884
		private bool _displayBg;

		// Token: 0x040041F5 RID: 16885
		private float inputCounter = 0.2f;

		// Token: 0x040041F6 RID: 16886
		private HashSet<int> _cachedSet;

		// Token: 0x040041F7 RID: 16887
		public static ColumnDefinition BirthDate = ViewSelectCharacter.CreateTextColumn(() => LanguageKey.LK_Birthday.Tr(), (CharacterDisplayDataForGeneralScrollList data) => TimeManager.GetYearDisplayString(data.BirthDate), -1, 30f, 90f);
	}
}
