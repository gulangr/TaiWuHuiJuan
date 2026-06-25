using System;
using System.Collections.Generic;
using System.Linq;
using CommonSortAndFilterLegacy;
using CommonSortAndFilterLegacy.CharacterTable;
using Config;
using FrameWork;
using Game.Components.Avatar;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Map;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UICommon.Character.Elements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200033C RID: 828
public class CharacterTable : Refers, ILanguage
{
	// Token: 0x17000553 RID: 1363
	// (get) Token: 0x06003066 RID: 12390 RVA: 0x0017A69A File Offset: 0x0017889A
	// (set) Token: 0x06003067 RID: 12391 RVA: 0x0017A6A4 File Offset: 0x001788A4
	public bool CanRowInteract
	{
		get
		{
			return this._canRowInteract;
		}
		set
		{
			this._canRowInteract = value;
			this.selectAllToggle.interactable = value;
			this.confirmBar.SetActive(value);
			foreach (KeyValuePair<int, CommonTableRow> keyValuePair in this._rows)
			{
				int num;
				CommonTableRow commonTableRow;
				keyValuePair.Deconstruct(out num, out commonTableRow);
				int charId = num;
				CommonTableRow row = commonTableRow;
				bool flag = !value || !this._bannedChars.Contains(charId);
				if (flag)
				{
					row.toggle.interactable = value;
				}
				row.Refresh(this._selectedChars.Contains(charId));
			}
		}
	}

	// Token: 0x17000554 RID: 1364
	// (get) Token: 0x06003068 RID: 12392 RVA: 0x0017A764 File Offset: 0x00178964
	private static short DefaultSortChoice
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x17000555 RID: 1365
	// (get) Token: 0x06003069 RID: 12393 RVA: 0x0017A767 File Offset: 0x00178967
	private static int TaiwuId
	{
		get
		{
			return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		}
	}

	// Token: 0x17000556 RID: 1366
	// (get) Token: 0x0600306A RID: 12394 RVA: 0x0017A773 File Offset: 0x00178973
	public static WorldMapModel WorldMapModel
	{
		get
		{
			return SingletonObject.getInstance<WorldMapModel>();
		}
	}

	// Token: 0x0600306B RID: 12395 RVA: 0x0017A77C File Offset: 0x0017897C
	public void Init(List<int> charList = null, List<ItemKey> itemKeys = null, List<global::CharacterTable.CharacterTableFilterData> filters = null, HashSet<int> bannedChars = null, HashSet<int> selectedChars = null, Action<int> onAvatarBtnClicked = null, List<ECharacterTableType> usingPages = null, List<global::CharacterTable.CharacterTableCommonFilterTypes> commonFilterTypesList = null, short charValueCharacterTableElementTemplateId = -1, Func<HashSet<int>, int> getValueFunc = null, bool canSelectSpecialChar = true, bool? canSwitchSelect = null)
	{
		this._charList = charList;
		this._itemKeyList = itemKeys;
		this._filters = filters;
		this._bannedChars = (bannedChars ?? new HashSet<int>());
		this._initialSelectedChars = (selectedChars ?? new HashSet<int>());
		this._onAvatarBtnClicked = onAvatarBtnClicked;
		this._usingPages = usingPages;
		this._charValueCharacterTableTemplateId = charValueCharacterTableElementTemplateId;
		this._canSelectSpecialChar = canSelectSpecialChar;
		this._getCurrValue = (getValueFunc ?? new Func<HashSet<int>, int>(this.GetCurrValue));
		this._selectedChars = new HashSet<int>();
		foreach (int id in this._initialSelectedChars)
		{
			this._selectedChars.Add(id);
		}
		bool flag = canSwitchSelect != null;
		if (flag)
		{
			this.canSwitchSelection = canSwitchSelect.Value;
		}
		bool flag2 = commonFilterTypesList != null;
		if (flag2)
		{
			this.commonFilterTypes.Clear();
			this.commonFilterTypes.AddRange(commonFilterTypesList);
		}
		this._inited = true;
		bool isActiveAndEnabled = base.isActiveAndEnabled;
		if (isActiveAndEnabled)
		{
			this.RefreshData();
		}
	}

	// Token: 0x0600306C RID: 12396 RVA: 0x0017A8AC File Offset: 0x00178AAC
	private void InitConfigs()
	{
		foreach (ECharacterTableType type in this.tableTypes)
		{
			foreach (CharacterTableItem config in ((IEnumerable<CharacterTableItem>)Config.CharacterTable.Instance))
			{
				bool flag = config.Type == type;
				if (flag)
				{
					this._configs[type] = config;
					break;
				}
			}
		}
	}

	// Token: 0x0600306D RID: 12397 RVA: 0x0017A958 File Offset: 0x00178B58
	private void InitPages()
	{
		this._heads.Clear();
		this._headCells.Clear();
		this._rowPrefabs.Clear();
		for (int index = 0; index < this.tableTypes.Count; index++)
		{
			CharacterTableItem config = this._configs[this.tableTypes[index]];
			GameObject obj = Object.Instantiate<GameObject>(this.tablePageToggleTemplate, this.tablePageToggleGroup.transform);
			Refers toggleRefers = obj.GetComponent<Refers>();
			CToggleObsolete toggle = toggleRefers.CGet<CToggleObsolete>("Toggle");
			GameObject head = Object.Instantiate<GameObject>(this.tableHeadTemplate, this.tableHeadHolder);
			GameObject row = Object.Instantiate<GameObject>(this.rowTemplate, base.transform);
			List<Refers> headCells = new List<Refers>();
			toggle.Key = index;
			toggleRefers.CGet<TextMeshProUGUI>("LabelDisable").SetText(config.Title, true);
			toggleRefers.CGet<TextMeshProUGUI>("LabelOff").SetText(config.Title, true);
			toggleRefers.CGet<TextMeshProUGUI>("LabelOn").SetText(config.Title, true);
			bool flag = this.tablePageToggleGroup != null;
			if (flag)
			{
				this.tablePageToggleGroup.Add(toggle);
			}
			for (int i = 0; i < config.Elements.Count; i++)
			{
				short elementTemplateId = config.Elements[i];
				CharacterTableElementItem elementConfig = CharacterTableElement.Instance[elementTemplateId];
				GameObject rowCellTemplate = this.GetElementPrefab(elementTemplateId);
				GameObject rowCell = Object.Instantiate<GameObject>(rowCellTemplate, row.GetComponent<CommonTableRow>().content);
				GameObject headCell = Object.Instantiate<GameObject>(this.tableHeadCellTemplate, head.transform);
				Refers headCellRefers = headCell.GetComponent<Refers>();
				CButtonObsolete headCellButton = headCell.GetComponent<CButtonObsolete>();
				int headCellWidth = config.Width[i] - 2 + ((i == config.Elements.Count - 1 || i == 0) ? 1 : 0);
				bool flag2 = elementConfig.Type == ECharacterTableElementType.Empty;
				if (flag2)
				{
					headCellRefers.CGet<GameObject>("FakeBg").GetComponent<CImage>().SetSprite("ui_sp_title_5_2", false, null);
				}
				bool canSort = elementConfig.CanSort;
				if (canSort)
				{
					headCellButton.ClearAndAddListener(delegate
					{
						this.OnClickHead(elementTemplateId);
					});
					headCellButton.interactable = true;
				}
				else
				{
					headCellButton.interactable = false;
				}
				headCellRefers.CGet<TextMeshProUGUI>("Label").SetText(elementConfig.Name, true);
				headCell.GetComponent<RectTransform>().SetWidth((float)headCellWidth);
				rowCell.GetComponent<RectTransform>().SetWidth((float)config.Width[i]);
				headCells.Add(headCellRefers);
			}
			head.SetActive(false);
			this._headCells.Add(headCells);
			this._heads.Add(head);
			this._rowPrefabs.Add(row.GetComponent<Refers>());
		}
		this.tablePageToggleGroup.InitPreOnToggle(-1);
		CToggleGroupObsolete ctoggleGroupObsolete = this.tablePageToggleGroup;
		ctoggleGroupObsolete.OnActiveToggleChange = (Action<CToggleObsolete, CToggleObsolete>)Delegate.Combine(ctoggleGroupObsolete.OnActiveToggleChange, new Action<CToggleObsolete, CToggleObsolete>(this.OnTablePageToggleChanged));
		this.infinityScroll.OnItemRender = new Action<int, Refers>(this.OnRenderRow);
	}

	// Token: 0x0600306E RID: 12398 RVA: 0x0017AC9C File Offset: 0x00178E9C
	private void InitCommonFilters()
	{
		this._commonFilterController = new CharacterTableSortAndFilterController(this.commonFilter);
		foreach (global::CharacterTable.CharacterTableCommonFilterTypes type in this.commonFilterTypes)
		{
			global::CharacterTable.CharacterTableCommonFilterTypes characterTableCommonFilterTypes = type;
			global::CharacterTable.CharacterTableCommonFilterTypes characterTableCommonFilterTypes2 = characterTableCommonFilterTypes;
			if (characterTableCommonFilterTypes2 != global::CharacterTable.CharacterTableCommonFilterTypes.Fallen)
			{
				if (characterTableCommonFilterTypes2 - global::CharacterTable.CharacterTableCommonFilterTypes.Villager <= 3)
				{
					this._commonFilterController.PreData.Add(new CharacterTableFilterLine());
				}
			}
			else
			{
				this._commonFilterController.PreData.Add(new CharacterTableFallenFilterLine());
			}
		}
		this._commonFilterController.Init(new Action(this.UpdateFilter), "CharacterTable" + this.title.text + "SortAndFilter");
	}

	// Token: 0x0600306F RID: 12399 RVA: 0x0017AD74 File Offset: 0x00178F74
	private void InitFilters()
	{
		bool flag = this.filterToggleGroup == null || this.filterToggleTemplate == null || this._filters == null;
		if (!flag)
		{
			for (int index = 0; index < this._filters.Count; index++)
			{
				global::CharacterTable.CharacterTableFilterData filterData = this._filters[index];
				GameObject obj = Object.Instantiate<GameObject>(this.filterToggleTemplate, this.filterToggleGroup.transform);
				Refers toggleRefers = obj.GetComponent<Refers>();
				CToggleObsolete toggle = toggleRefers.CGet<CToggleObsolete>("Toggle");
				toggle.name = string.Format("CommonFilterToggle_{0}", index);
				toggle.Key = index;
				bool flag2 = !filterData.FilterName.IsNullOrEmpty();
				if (flag2)
				{
					List<TextMeshProUGUI> prefabs = toggleRefers.CGetList<TextMeshProUGUI>("Label");
					foreach (TextMeshProUGUI text in prefabs)
					{
						text.SetText(filterData.FilterName, true);
					}
				}
				bool flag3 = !filterData.FilterIcon.IsNullOrEmpty();
				if (flag3)
				{
					List<CImage> prefabs2 = toggleRefers.CGetList<CImage>("Icon");
					foreach (CImage icon in prefabs2)
					{
						icon.SetSprite(filterData.FilterIcon, false, null);
					}
				}
				toggle.enabled = (filterData.FilterCharIds.Count != 0);
				this.filterToggleGroup.Add(toggle);
				obj.SetActive(true);
			}
			this.filterToggleGroup.InitPreOnToggle(-1);
			CToggleGroupObsolete ctoggleGroupObsolete = this.filterToggleGroup;
			ctoggleGroupObsolete.OnActiveToggleChange = (Action<CToggleObsolete, CToggleObsolete>)Delegate.Combine(ctoggleGroupObsolete.OnActiveToggleChange, new Action<CToggleObsolete, CToggleObsolete>(this.OnFilterToggleChanged));
		}
	}

	// Token: 0x06003070 RID: 12400 RVA: 0x0017AF74 File Offset: 0x00179174
	private void InitSorters()
	{
		for (int index = 0; index < this.tableTypes.Count; index++)
		{
			this._sorters.Add(new List<global::CharacterTable.CharacterTableSortData>());
		}
	}

	// Token: 0x06003071 RID: 12401 RVA: 0x0017AFB0 File Offset: 0x001791B0
	private void InitSelect()
	{
		this.canRowInteractSwitch.onValueChanged.AddListener(new UnityAction<bool>(this.OnClickCanInteractSwitch));
		this.selectAllToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnClickSelectAll));
		this._onRowClicked = new Action<int>(this.OnClickRow);
	}

	// Token: 0x06003072 RID: 12402 RVA: 0x0017B00A File Offset: 0x0017920A
	private void Awake()
	{
		this.InitConfigs();
		this.InitPages();
		this.InitCommonFilters();
		this.InitFilters();
		this.InitSorters();
		this.InitSelect();
	}

	// Token: 0x06003073 RID: 12403 RVA: 0x0017B038 File Offset: 0x00179238
	private void OnEnable()
	{
		this._features.Clear();
		this._highestCharIds.Clear();
		this.searchByName.onValueChanged.RemoveListener(new UnityAction<string>(this.OnInputFieldChanged));
		this.searchByName.onValueChanged.AddListener(new UnityAction<string>(this.OnInputFieldChanged));
		bool inited = this._inited;
		if (inited)
		{
			this.RefreshData();
		}
	}

	// Token: 0x06003074 RID: 12404 RVA: 0x0017B0A9 File Offset: 0x001792A9
	private void OnDisable()
	{
		this._inited = false;
		this.infinityScroll.SetDataCount(0);
	}

	// Token: 0x06003075 RID: 12405 RVA: 0x0017B0C0 File Offset: 0x001792C0
	private void OnTablePageToggleChanged(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		bool flag = togNew == null;
		if (!flag)
		{
			this._heads[this._currPageIndex].SetActive(false);
			this._currPageIndex = togNew.Key;
			this._heads[this._currPageIndex].SetActive(true);
			this.RefreshPage();
		}
	}

	// Token: 0x06003076 RID: 12406 RVA: 0x0017B120 File Offset: 0x00179320
	public void OnLanguageChange(LocalStringManager.LanguageType languageType)
	{
		foreach (Refers feature in this._features)
		{
			List<CharacterFeatureView> prefabs = feature.CGetList<CharacterFeatureView>("Feature");
			foreach (CharacterFeatureView featureItem in prefabs)
			{
				featureItem.gameObject.SetActive(false);
			}
			CharacterFeatureView obj;
			bool flag = feature.CTryGet<CharacterFeatureView>(global::CharacterTable.FeatureGameObjectNames[languageType], out obj);
			if (flag)
			{
				obj.gameObject.SetActive(true);
			}
			else
			{
				feature.CGet<CharacterFeatureView>(global::CharacterTable.FeatureGameObjectNames[LocalStringManager.LanguageType.CN]).gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06003077 RID: 12407 RVA: 0x0017B210 File Offset: 0x00179410
	private void RefreshPageToggle()
	{
		bool flag = this._usingPages == null;
		if (flag)
		{
			for (int i = 0; i < this.tableTypes.Count; i++)
			{
				CToggleObsolete obj = this.tablePageToggleGroup.Get(i);
				obj.transform.SetAsLastSibling();
				obj.gameObject.SetActive(true);
			}
		}
		else
		{
			for (int j = 0; j < this.tableTypes.Count; j++)
			{
				bool flag2 = !this._usingPages.Contains(this.tableTypes[j]);
				if (flag2)
				{
					this.tablePageToggleGroup.Get(j).gameObject.SetActive(false);
				}
			}
			foreach (ECharacterTableType type in this._usingPages)
			{
				for (int k = 0; k < this.tableTypes.Count; k++)
				{
					bool flag3 = this.tableTypes[k] == type;
					if (flag3)
					{
						CToggleObsolete obj2 = this.tablePageToggleGroup.Get(k);
						obj2.transform.SetAsLastSibling();
						obj2.gameObject.SetActive(true);
						break;
					}
				}
			}
		}
	}

	// Token: 0x06003078 RID: 12408 RVA: 0x0017B37C File Offset: 0x0017957C
	private void RefreshCommonFilter()
	{
		for (int index = 0; index < this.commonFilterTypes.Count; index++)
		{
			switch (this.commonFilterTypes[index])
			{
			case global::CharacterTable.CharacterTableCommonFilterTypes.Villager:
				this._commonFilterController.SetDropdownMenuVisible(index, 8, false);
				this._commonFilterController.SetDropdownMenuVisible(index, 11, false);
				this._commonFilterController.SetDropdownMenuVisible(index, 7, true);
				break;
			case global::CharacterTable.CharacterTableCommonFilterTypes.Character:
				this._commonFilterController.SetDropdownMenuVisible(index, 8, true);
				this._commonFilterController.SetDropdownMenuVisible(index, 11, false);
				this._commonFilterController.SetDropdownMenuVisible(index, 7, false);
				break;
			case global::CharacterTable.CharacterTableCommonFilterTypes.Feast:
				this._commonFilterController.SetDropdownMenuVisible(index, 8, false);
				this._commonFilterController.SetDropdownMenuVisible(index, 11, true);
				this._commonFilterController.SetDropdownMenuVisible(index, 7, false);
				break;
			}
		}
	}

	// Token: 0x06003079 RID: 12409 RVA: 0x0017B470 File Offset: 0x00179670
	private void RefreshData()
	{
		List<short> types = this.GetCharacterTableElements();
		this._characterNameData.Clear();
		this._characterValue.Clear();
		this.RefreshPageToggle();
		this.RefreshCommonFilter();
		this.CanRowInteract = (this.activateSelectionWhenReady || this.forceSelect);
		this.canRowInteractSwitch.interactable = !this.forceSelect;
		this.canRowInteractSwitch.isOn = this.activateSelectionWhenReady;
		bool flag = this.forceSelect;
		if (flag)
		{
			this.closeBtn.gameObject.SetActive(false);
			this.cancelBtn.gameObject.SetActive(false);
		}
		this.selectAllToggle.gameObject.SetActive(this.canSelectAll);
		this.canRowInteractSwitchObject.SetActive(this.canSwitchSelection);
		global::CharacterTable.CharacterTableInitMode characterTableInitMode = this.initMode;
		global::CharacterTable.CharacterTableInitMode characterTableInitMode2 = characterTableInitMode;
		if (characterTableInitMode2 != global::CharacterTable.CharacterTableInitMode.ByCharList)
		{
			if (characterTableInitMode2 == global::CharacterTable.CharacterTableInitMode.ByNeedItem)
			{
				CharacterDomainMethod.AsyncCall.GetCharacterTableDisplayDataListWithNeedItem(null, types, this._itemKeyList[0], delegate(int offset, RawDataPool dataPool)
				{
					List<CharacterTableDisplayData> dataList = new List<CharacterTableDisplayData>();
					Serializer.Deserialize(dataPool, offset, ref dataList);
					this._charList = new List<int>();
					foreach (CharacterTableDisplayData data in dataList)
					{
						bool isDisplayData = data.IsDisplayData;
						if (isDisplayData)
						{
							this._charList.Add(data.CharId);
						}
					}
					this.HandleData(dataList, types);
				});
			}
		}
		else
		{
			List<int> charList = this._charList;
			bool flag2 = charList != null && charList.Count > 0;
			if (flag2)
			{
				CharacterDomainMethod.AsyncCall.GetCharacterTableDisplayDataList(null, this._charList, types, delegate(int offset, RawDataPool dataPool)
				{
					List<CharacterTableDisplayData> dataList = new List<CharacterTableDisplayData>();
					Serializer.Deserialize(dataPool, offset, ref dataList);
					this.HandleData(dataList, types);
				});
			}
			else
			{
				this._heads[this._currPageIndex].SetActive(true);
				this.RefreshAllSelectParts();
			}
		}
	}

	// Token: 0x0600307A RID: 12410 RVA: 0x0017B5FC File Offset: 0x001797FC
	private void HandleData(List<CharacterTableDisplayData> dataList, List<short> types)
	{
		List<CharacterTableSortAndFilterData> commonFilterData = new List<CharacterTableSortAndFilterData>();
		foreach (CharacterTableDisplayData data in dataList)
		{
			this._characterData[data.CharId] = data;
		}
		bool flag = this._charValueCharacterTableTemplateId >= 0;
		if (flag)
		{
			foreach (int charId in this._charList)
			{
				this._characterValue[charId] = this._characterData[charId].GetInt(this._charValueCharacterTableTemplateId);
			}
		}
		bool flag2 = !this._canSelectSpecialChar;
		if (flag2)
		{
			foreach (int charId2 in this._charList)
			{
				bool flag3 = CreatingType.IsNonEvolutionaryType(this._characterData[charId2].CreatingType);
				if (flag3)
				{
					this._bannedChars.Add(charId2);
				}
			}
		}
		foreach (int charId3 in this._charList)
		{
			commonFilterData.Add(new CharacterTableSortAndFilterData
			{
				Data = this._characterData[charId3]
			});
		}
		this._commonFilterController.SetDataList(commonFilterData, true);
		bool flag4 = this._charList.Count == 0;
		if (!flag4)
		{
			foreach (short elementId in types)
			{
				bool canHighlight = CharacterTableElement.Instance[elementId].CanHighlight;
				if (canHighlight)
				{
					this._sortingElementTemplateId = elementId;
					this._charList.Sort(new Comparison<int>(this.CompareRowByColumn));
					Dictionary<short, int> highestCharIds = this._highestCharIds;
					short sortingElementTemplateId = this._sortingElementTemplateId;
					List<int> charList = this._charList;
					highestCharIds[sortingElementTemplateId] = charList[charList.Count - 1];
				}
			}
			bool flag5 = this._usingPages != null && !this._usingPages.Contains(this.tableTypes[this._currPageIndex]);
			if (flag5)
			{
				for (int i = 0; i < this.tableTypes.Count; i++)
				{
					bool flag6 = this.tableTypes[i] == this._usingPages[0];
					if (flag6)
					{
						this.tablePageToggleGroup.Set(i, true, false);
						break;
					}
				}
			}
			else
			{
				this._heads[this._currPageIndex].SetActive(true);
				this.RefreshPage();
			}
			this.RefreshAllSelectParts();
		}
	}

	// Token: 0x0600307B RID: 12411 RVA: 0x0017B920 File Offset: 0x00179B20
	private void OnClickHead(short elementTemplateId)
	{
		for (int index = 0; index < this._sorters[this._currPageIndex].Count; index++)
		{
			global::CharacterTable.CharacterTableSortData sorter = this._sorters[this._currPageIndex][index];
			bool flag = sorter.ElementTemplateId == elementTemplateId;
			if (flag)
			{
				bool isDescending = sorter.IsDescending;
				if (isDescending)
				{
					sorter.IsDescending = false;
				}
				else
				{
					this._sorters[this._currPageIndex].RemoveAt(index);
				}
				this.UpdateHead();
				this.RefreshPage();
				return;
			}
		}
		this._sorters[this._currPageIndex].Add(new global::CharacterTable.CharacterTableSortData(elementTemplateId, true));
		this.UpdateHead();
		this.RefreshPage();
	}

	// Token: 0x0600307C RID: 12412 RVA: 0x0017B9E8 File Offset: 0x00179BE8
	private void UpdateHead()
	{
		foreach (Refers headCell in this._headCells[this._currPageIndex])
		{
			headCell.CGet<GameObject>("Arrow").SetActive(false);
			headCell.CGet<GameObject>("NumberIcon").SetActive(false);
		}
		for (int index = 0; index < this._sorters[this._currPageIndex].Count; index++)
		{
			global::CharacterTable.CharacterTableSortData sorter = this._sorters[this._currPageIndex][index];
			int elementIndex = this.GetElementIndex(this.tableTypes[this._currPageIndex], sorter.ElementTemplateId);
			Refers headCell2 = this._headCells[this._currPageIndex][elementIndex];
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

	// Token: 0x0600307D RID: 12413 RVA: 0x0017BB60 File Offset: 0x00179D60
	private int CompareRow(int a, int b)
	{
		List<global::CharacterTable.CharacterTableSortData> sorters = this._sorters[this._currPageIndex];
		foreach (global::CharacterTable.CharacterTableSortData sorter in sorters)
		{
			int result = this.CompareElementData(a, b, sorter.ElementTemplateId);
			bool flag = result == 0;
			if (!flag)
			{
				return sorter.IsDescending ? (-result) : result;
			}
		}
		return this.CompareElementData(a, b, global::CharacterTable.DefaultSortChoice);
	}

	// Token: 0x0600307E RID: 12414 RVA: 0x0017BBFC File Offset: 0x00179DFC
	private int CompareRowByColumn(int a, int b)
	{
		int result = this.CompareElementData(a, b, this._sortingElementTemplateId);
		return (result == 0) ? this.CompareElementData(a, b, global::CharacterTable.DefaultSortChoice) : result;
	}

	// Token: 0x0600307F RID: 12415 RVA: 0x0017BC30 File Offset: 0x00179E30
	private void OnInputFieldChanged(string text)
	{
		this._searchInputText = text;
		this.UpdateFilter();
	}

	// Token: 0x06003080 RID: 12416 RVA: 0x0017BC44 File Offset: 0x00179E44
	private void OnFilterToggleChanged(CToggleObsolete togNew, CToggleObsolete _)
	{
		bool flag = togNew != null;
		if (flag)
		{
			this._currFilterIndex = togNew.Key;
		}
		this.UpdateFilter();
	}

	// Token: 0x06003081 RID: 12417 RVA: 0x0017BC70 File Offset: 0x00179E70
	private void UpdateFilter()
	{
		this._sortedAndFilteredCharList.Clear();
		bool flag = this._filters == null || !this._filters.CheckIndex(this._currFilterIndex);
		if (flag)
		{
			foreach (int charId in this._charList)
			{
				this._sortedAndFilteredCharList.Add(charId);
			}
		}
		else
		{
			foreach (int charId2 in this._charList)
			{
				bool flag2 = this._filters[this._currFilterIndex].FilterCharIds.Contains(charId2);
				if (flag2)
				{
					this._sortedAndFilteredCharList.Add(charId2);
				}
			}
		}
		bool flag3 = !this._searchInputText.IsNullOrEmpty();
		if (flag3)
		{
			for (int index = this._sortedAndFilteredCharList.Count - 1; index >= 0; index--)
			{
				NameRelatedData nameRelatedData = this._characterData[this._sortedAndFilteredCharList[index]].NameData;
				bool flag4 = !NameCenter.SearchTextInDisplayName(ref nameRelatedData, this._searchInputText, false);
				if (flag4)
				{
					this._sortedAndFilteredCharList.RemoveAt(index);
				}
			}
		}
		this._commonFilteredCharIds.Clear();
		foreach (CharacterTableSortAndFilterData data in this._commonFilterController.OutputDataList)
		{
			this._commonFilteredCharIds.Add(data.Data.CharId);
		}
		for (int i = this._sortedAndFilteredCharList.Count - 1; i >= 0; i--)
		{
			bool flag5 = !this._commonFilteredCharIds.Contains(this._sortedAndFilteredCharList[i]);
			if (flag5)
			{
				this._sortedAndFilteredCharList.RemoveAt(i);
			}
		}
		this.RefreshPage();
	}

	// Token: 0x06003082 RID: 12418 RVA: 0x0017BEB4 File Offset: 0x0017A0B4
	public List<int> GetSelectedCharIdList()
	{
		return this._selectedChars.ToList<int>();
	}

	// Token: 0x06003083 RID: 12419 RVA: 0x0017BED4 File Offset: 0x0017A0D4
	public List<int> GetCharIdList()
	{
		return this._charList;
	}

	// Token: 0x06003084 RID: 12420 RVA: 0x0017BEEC File Offset: 0x0017A0EC
	private void OnClickRow(int charId)
	{
		bool flag = this._selectedChars.Contains(charId);
		if (flag)
		{
			bool flag2 = this.multiSelection;
			if (flag2)
			{
				this._selectedChars.Remove(charId);
			}
		}
		else
		{
			bool flag3 = !this._bannedChars.Contains(charId);
			if (flag3)
			{
				bool flag4 = this.multiSelection;
				if (flag4)
				{
					bool flag5 = this.maxSelectCount <= 0 || this._selectedChars.Count < this.maxSelectCount;
					if (flag5)
					{
						this._selectedChars.Add(charId);
					}
				}
				else
				{
					this._selectedChars.Clear();
					this._selectedChars.Add(charId);
				}
			}
		}
		this.RefreshRows();
		this.RefreshAllSelectParts();
	}

	// Token: 0x06003085 RID: 12421 RVA: 0x0017BFA3 File Offset: 0x0017A1A3
	private void OnClickCanInteractSwitch(bool isOn)
	{
		this.CanRowInteract = isOn;
	}

	// Token: 0x06003086 RID: 12422 RVA: 0x0017BFB0 File Offset: 0x0017A1B0
	private void OnClickSelectAll(bool isOn)
	{
		if (isOn)
		{
			foreach (int charId in this._sortedAndFilteredCharList)
			{
				bool flag = !this._bannedChars.Contains(charId);
				if (flag)
				{
					this._selectedChars.Add(charId);
				}
			}
		}
		else
		{
			foreach (int charId2 in this._sortedAndFilteredCharList)
			{
				this._selectedChars.Remove(charId2);
			}
		}
		this.RefreshRows();
		this.RefreshAllSelectParts();
	}

	// Token: 0x06003087 RID: 12423 RVA: 0x0017C088 File Offset: 0x0017A288
	private void RefreshRows()
	{
		foreach (KeyValuePair<int, CommonTableRow> keyValuePair in this._rows)
		{
			int num;
			CommonTableRow commonTableRow;
			keyValuePair.Deconstruct(out num, out commonTableRow);
			int charId = num;
			CommonTableRow row = commonTableRow;
			row.Refresh(this._selectedChars.Contains(charId));
		}
	}

	// Token: 0x06003088 RID: 12424 RVA: 0x0017C0FC File Offset: 0x0017A2FC
	private void RefreshConfirmBtnCanInteract()
	{
		this.confirmBtn.interactable = this.IsSelectionSatisfied();
	}

	// Token: 0x06003089 RID: 12425 RVA: 0x0017C114 File Offset: 0x0017A314
	private void RefreshSelectAllDisplay()
	{
		bool flag = this.canSelectAll;
		if (flag)
		{
			this.selectAllToggle.SetIsOnWithoutNotify(this.IsFilteredRowAllSelected());
		}
	}

	// Token: 0x0600308A RID: 12426 RVA: 0x0017C140 File Offset: 0x0017A340
	private void RefreshSelectText()
	{
		string currString = this._getCurrValue(this._selectedChars).ToString();
		string maxString = (this.maxSelectCount > 0) ? ("/" + this.maxSelectCount.ToString()) : "";
		this.selectText.SetText(currString + maxString, true);
	}

	// Token: 0x0600308B RID: 12427 RVA: 0x0017C1A2 File Offset: 0x0017A3A2
	private void RefreshAllSelectParts()
	{
		this.RefreshSelectText();
		this.RefreshSelectAllDisplay();
		this.RefreshConfirmBtnCanInteract();
	}

	// Token: 0x0600308C RID: 12428 RVA: 0x0017C1BC File Offset: 0x0017A3BC
	private bool IsFilteredRowAllSelected()
	{
		foreach (int charId in this._sortedAndFilteredCharList)
		{
			bool flag = !this._bannedChars.Contains(charId) && !this._selectedChars.Contains(charId);
			if (flag)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600308D RID: 12429 RVA: 0x0017C238 File Offset: 0x0017A438
	private bool IsSelectionSatisfied()
	{
		int currValue = this._getCurrValue(this._selectedChars);
		bool flag = this.CompareCurrAndPrevSelection();
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = this.minSelectCount > 0 && currValue < this.minSelectCount;
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool flag3 = this.maxSelectCount > 0 && currValue > this.maxSelectCount;
				result = !flag3;
			}
		}
		return result;
	}

	// Token: 0x0600308E RID: 12430 RVA: 0x0017C2A8 File Offset: 0x0017A4A8
	private bool CompareCurrAndPrevSelection()
	{
		bool flag = this._selectedChars.Count != this._initialSelectedChars.Count;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			foreach (int id in this._selectedChars)
			{
				bool flag2 = !this._initialSelectedChars.Contains(id);
				if (flag2)
				{
					return false;
				}
			}
			result = true;
		}
		return result;
	}

	// Token: 0x0600308F RID: 12431 RVA: 0x0017C338 File Offset: 0x0017A538
	private int GetCurrValue(HashSet<int> selectedCharIdList)
	{
		int currValue = 0;
		foreach (int charId in selectedCharIdList)
		{
			currValue += this._characterValue.GetValueOrDefault(charId, 1);
		}
		return currValue;
	}

	// Token: 0x06003090 RID: 12432 RVA: 0x0017C39C File Offset: 0x0017A59C
	private void OnRenderRow(int charIndex, Refers charRefers)
	{
		int charId = this._sortedAndFilteredCharList[charIndex];
		CharacterTableItem config = this._configs[this.tableTypes[this._currPageIndex]];
		GameObject row = charRefers.gameObject;
		CommonTableRow commonTableRow = charRefers.GetComponent<CommonTableRow>();
		RectTransform cells = commonTableRow.content;
		List<GameObject> hovers = commonTableRow.hoverObjects;
		this._rows[charId] = commonTableRow;
		for (int i = 0; i < config.Elements.Count; i++)
		{
			Transform cell = cells.GetChild(i);
			Refers cellRefers = cell.GetComponent<Refers>();
			short elementId = config.Elements[i];
			CharacterTableElementItem elementConfig = CharacterTableElement.Instance[elementId];
			this.SetElementData(cellRefers, elementId, this._characterData[charId]);
			GameObject hover;
			bool flag = cellRefers.CTryGet<GameObject>("Hover", out hover);
			if (flag)
			{
				hovers.Add(hover);
			}
			bool flag2 = elementConfig.Type == ECharacterTableElementType.Feature;
			if (flag2)
			{
				this._features.Add(cellRefers);
			}
			int highestId;
			this.UpdateCellImage(cellRefers, elementConfig.Type == ECharacterTableElementType.Empty, this._highestCharIds.TryGetValue(elementId, out highestId) && highestId == charId, i == config.Elements.Count - 1);
		}
		bool flag3 = this.initSelection;
		if (flag3)
		{
			commonTableRow.toggle.interactable = (this.CanRowInteract && !this._bannedChars.Contains(charId));
		}
		commonTableRow.InitClickEvent(this._selectedChars.Contains(charId), charId, this._onRowClicked);
		row.SetActive(true);
	}

	// Token: 0x06003091 RID: 12433 RVA: 0x0017C540 File Offset: 0x0017A740
	private void UpdateCellImage(Refers cell, bool isEmpty, bool isSpecial, bool isLast)
	{
		GameObject hover;
		bool hasHover = cell.CTryGet<GameObject>("Hover", out hover);
		CImage image;
		bool hasBg = cell.TryGetComponent<CImage>(out image);
		if (isEmpty)
		{
			bool flag = hasBg;
			if (flag)
			{
				image.SetSprite("ui_sp_items_base_0", false, null);
			}
			bool flag2 = hasHover;
			if (flag2)
			{
				hover.GetComponent<CImage>().SetSprite("ui_sp_items_base_3", false, null);
			}
		}
		else if (isSpecial)
		{
			if (isLast)
			{
				bool flag3 = hasBg;
				if (flag3)
				{
					image.SetSprite("ui_sp_character_base_2", false, null);
				}
				bool flag4 = hasHover;
				if (flag4)
				{
					hover.GetComponent<CImage>().SetSprite("ui_sp_character_base_3", false, null);
				}
			}
			else
			{
				bool flag5 = hasBg;
				if (flag5)
				{
					image.SetSprite("ui_sp_character_base_0", false, null);
				}
				bool flag6 = hasHover;
				if (flag6)
				{
					hover.GetComponent<CImage>().SetSprite("ui_sp_character_base_1", false, null);
				}
			}
		}
		else if (isLast)
		{
			bool flag7 = hasBg;
			if (flag7)
			{
				image.SetSprite("ui_sp_items_base_2", false, null);
			}
			bool flag8 = hasHover;
			if (flag8)
			{
				hover.GetComponent<CImage>().SetSprite("ui_sp_items_base_5", false, null);
			}
		}
		else
		{
			bool flag9 = hasBg;
			if (flag9)
			{
				image.SetSprite("ui_sp_items_base_1", false, null);
			}
			bool flag10 = hasHover;
			if (flag10)
			{
				hover.GetComponent<CImage>().SetSprite("ui_sp_items_base_4", false, null);
			}
		}
	}

	// Token: 0x06003092 RID: 12434 RVA: 0x0017C684 File Offset: 0x0017A884
	private void RefreshPage()
	{
		this._rows.Clear();
		this._sortedAndFilteredCharList.Sort(new Comparison<int>(this.CompareRow));
		this.infinityScroll.UpdateStyle(InfinityScrollLegacy.ScrollDirection.FromTop, 1, this.infinityScroll.Gap, this.infinityScroll.Padding, this._rowPrefabs[this._currPageIndex]);
		this.infinityScroll.UpdateData(this._sortedAndFilteredCharList.Count);
		this.infinityScroll.GetComponent<CScrollRectLegacy>().ScrollBar.value = 0f;
	}

	// Token: 0x06003093 RID: 12435 RVA: 0x0017C720 File Offset: 0x0017A920
	private void SetElementData(Refers refers, short elementTemplateId, CharacterTableDisplayData data)
	{
		switch (CharacterTableElement.Instance[elementTemplateId].Type)
		{
		case ECharacterTableElementType.Text:
			this.SetText(refers, data, elementTemplateId);
			break;
		case ECharacterTableElementType.TextWithIcon:
			this.SetTextWithIcon(refers, data, elementTemplateId);
			break;
		case ECharacterTableElementType.TextWithSprite:
			this.SetTextWithSprite(refers, data, elementTemplateId);
			break;
		case ECharacterTableElementType.Avatar:
			this.SetAvatar(refers, data);
			break;
		case ECharacterTableElementType.Feature:
			this.SetFeature(refers, data, elementTemplateId);
			break;
		case ECharacterTableElementType.Command:
			this.SetCommand(refers, data, elementTemplateId);
			break;
		}
	}

	// Token: 0x06003094 RID: 12436 RVA: 0x0017C7AC File Offset: 0x0017A9AC
	private bool GetElementValid(short templateId, CharacterTableDisplayData data)
	{
		if (!true)
		{
		}
		bool result;
		if (templateId <= 2)
		{
			if (templateId > 1)
			{
				if (templateId != 2)
				{
					goto IL_22D;
				}
				result = !this.GetCharacterHideAge(data);
				goto IL_25C;
			}
		}
		else
		{
			if (templateId == 8)
			{
				result = (data.GetInt(templateId) != -32768);
				goto IL_25C;
			}
			switch (templateId)
			{
			case 77:
				result = (data.GetInt(78) >= 0);
				goto IL_25C;
			case 78:
			case 79:
			case 80:
			case 93:
			case 95:
			case 96:
			case 97:
			case 98:
			case 102:
			case 104:
			case 105:
			case 106:
			case 107:
				goto IL_22D;
			case 81:
				result = (data.GetInt(templateId) >= 0);
				goto IL_25C;
			case 82:
				result = (data.GetInt(templateId) >= 0);
				goto IL_25C;
			case 83:
				result = (data.GetInt(templateId) >= 0);
				goto IL_25C;
			case 84:
				result = (data.GetInt(templateId) >= 0);
				goto IL_25C;
			case 85:
				result = (data.GetInt(templateId) >= 0);
				goto IL_25C;
			case 86:
				result = (data.GetInt(templateId) >= 0);
				goto IL_25C;
			case 87:
				result = (data.GetInt(templateId) >= 0);
				goto IL_25C;
			case 88:
				result = (data.GetInt(templateId) >= 0);
				goto IL_25C;
			case 89:
				result = (data.GetInt(templateId) >= 0);
				goto IL_25C;
			case 90:
				result = (data.GetInt(templateId) >= 0);
				goto IL_25C;
			case 91:
			case 94:
			case 103:
			case 108:
			case 109:
				break;
			case 92:
				result = (!Character.Instance[data.NameData.CharTemplateId].SpecialGradeName.IsNullOrEmpty() || data.GetOrg(templateId).OrgTemplateId != 0);
				goto IL_25C;
			case 99:
				result = (data.GetWork(99).WorkStatus != 6);
				goto IL_25C;
			case 100:
				result = (data.GetWork(99).IntValue >= 0);
				goto IL_25C;
			case 101:
				result = (data.GetWork(99).IntValue >= 0);
				goto IL_25C;
			default:
				goto IL_22D;
			}
		}
		result = true;
		goto IL_25C;
		IL_22D:
		result = (data.GetInt(templateId) >= 0 && (!CharacterTableElement.Instance[templateId].HideProperty || !this.GetCharacterHideProperty(data)));
		IL_25C:
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06003095 RID: 12437 RVA: 0x0017CA1C File Offset: 0x0017AC1C
	private int GetElementIndex(ECharacterTableType type, short elementId)
	{
		CharacterTableItem config = this._configs[type];
		bool flag = config == null;
		int result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			for (int i = 0; i < config.Elements.Count; i++)
			{
				bool flag2 = config.Elements[i] == elementId;
				if (flag2)
				{
					return i;
				}
			}
			result = -1;
		}
		return result;
	}

	// Token: 0x06003096 RID: 12438 RVA: 0x0017CA80 File Offset: 0x0017AC80
	private GameObject GetElementPrefab(short templateId)
	{
		ECharacterTableElementType type = CharacterTableElement.Instance[templateId].Type;
		if (!true)
		{
		}
		GameObject result;
		switch (type)
		{
		case ECharacterTableElementType.Text:
			result = this.textPrefab;
			break;
		case ECharacterTableElementType.Empty:
			result = this.emptyPrefab;
			break;
		case ECharacterTableElementType.TextWithIcon:
			result = this.textWithIconPrefab;
			break;
		case ECharacterTableElementType.TextWithSprite:
			result = this.textWithSpritePrefab;
			break;
		case ECharacterTableElementType.Avatar:
			result = this.avatarPrefab;
			break;
		case ECharacterTableElementType.Feature:
			result = this.featurePrefab;
			break;
		case ECharacterTableElementType.Command:
			result = this.commandPrefab;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06003097 RID: 12439 RVA: 0x0017CB10 File Offset: 0x0017AD10
	private int CompareElementData(int a, int b, short elementTemplateId)
	{
		CharacterTableDisplayData dataA = this._characterData[a];
		CharacterTableDisplayData dataB = this._characterData[b];
		int charId;
		bool flag = this._highestCharIds.TryGetValue(elementTemplateId, out charId);
		if (flag)
		{
			bool flag2 = a == charId;
			if (flag2)
			{
				return 1;
			}
			bool flag3 = b == charId;
			if (flag3)
			{
				return -1;
			}
		}
		bool validA = this.GetElementValid(elementTemplateId, dataA);
		bool validB = this.GetElementValid(elementTemplateId, dataB);
		bool flag4 = !validA && !validB;
		int result;
		if (flag4)
		{
			result = 0;
		}
		else
		{
			bool flag5 = !validA;
			if (flag5)
			{
				result = -1;
			}
			else
			{
				bool flag6 = !validB;
				if (flag6)
				{
					result = 1;
				}
				else
				{
					if (!true)
					{
					}
					int num;
					if (elementTemplateId <= 46)
					{
						if (elementTemplateId == 0)
						{
							num = this.SortByName(dataA, dataB);
							goto IL_1A5;
						}
						if (elementTemplateId == 46)
						{
							num = this.SortByLifeSkillGrowth(dataA, dataB);
							goto IL_1A5;
						}
					}
					else
					{
						if (elementTemplateId == 61)
						{
							num = this.SortByCombatSkillGrowth(dataA, dataB);
							goto IL_1A5;
						}
						if (elementTemplateId == 77)
						{
							num = this.SortByWeight(dataA, dataB);
							goto IL_1A5;
						}
						switch (elementTemplateId)
						{
						case 91:
							num = this.SortByOrganizationName(dataA, dataB);
							goto IL_1A5;
						case 92:
							num = this.SortByIdentity(dataA, dataB);
							goto IL_1A5;
						case 94:
						case 103:
							num = this.SortByLocation(dataA, dataB, elementTemplateId);
							goto IL_1A5;
						case 99:
							num = this.SortByWorkStatus(dataA, dataB);
							goto IL_1A5;
						case 100:
							num = this.SortByWorkBuilding(dataA, dataB);
							goto IL_1A5;
						case 101:
							num = this.SortByWorkPost(dataA, dataB);
							goto IL_1A5;
						}
					}
					num = dataA.GetInt(elementTemplateId).CompareTo(dataB.GetInt(elementTemplateId));
					IL_1A5:
					if (!true)
					{
					}
					result = num;
				}
			}
		}
		return result;
	}

	// Token: 0x06003098 RID: 12440 RVA: 0x0017CCD0 File Offset: 0x0017AED0
	private bool TryGetElementText(short templateId, CharacterTableDisplayData data, out string text)
	{
		bool flag = !this.GetElementValid(templateId, data);
		bool result;
		if (flag)
		{
			text = "-";
			result = false;
		}
		else
		{
			if (!true)
			{
			}
			string text2;
			if (templateId <= 46)
			{
				switch (templateId)
				{
				case 3:
					text2 = CommonUtils.GetHealthString((sbyte)data.GetInt(templateId));
					goto IL_257;
				case 4:
				case 9:
					goto IL_245;
				case 5:
					text2 = CommonUtils.GetCharmString((sbyte)data.GetInt(templateId), (sbyte)data.GetInt(96));
					goto IL_257;
				case 6:
					text2 = CommonUtils.GetBehaviorString((sbyte)data.GetInt(templateId));
					goto IL_257;
				case 7:
					text2 = CommonUtils.GetHappinessString((sbyte)data.GetInt(templateId));
					goto IL_257;
				case 8:
					text2 = CommonUtils.GetFavorString((short)data.GetInt(templateId));
					goto IL_257;
				case 10:
					text2 = CommonUtils.GetFameString((sbyte)data.GetInt(templateId));
					goto IL_257;
				default:
					if (templateId != 46)
					{
						goto IL_245;
					}
					break;
				}
			}
			else if (templateId != 61)
			{
				switch (templateId)
				{
				case 77:
					text2 = CommonUtils.GetWeightString(data.GetInt(78), data.GetInt(79));
					goto IL_257;
				case 78:
				case 79:
				case 80:
				case 81:
				case 82:
				case 83:
				case 93:
				case 95:
				case 96:
				case 97:
				case 98:
				case 102:
					goto IL_245;
				case 84:
				case 85:
				case 86:
					text2 = TeammateCommand.Instance[data.GetInt(templateId)].Name;
					goto IL_257;
				case 87:
				case 89:
					text2 = LocalStringManager.Get("LK_LegendaryBook_" + data.GetInt(templateId).ToString());
					goto IL_257;
				case 88:
				case 90:
					text2 = "";
					goto IL_257;
				case 91:
					text2 = SingletonObject.getInstance<WorldMapModel>().GetSettlementName(data.GetOrg(templateId));
					goto IL_257;
				case 92:
					text2 = CommonUtils.GetIdentityStringWithSpecialCharacterConfig((int)data.NameData.CharTemplateId, data.GetOrg(templateId), (sbyte)data.GetInt(96), (short)data.GetInt(2), false);
					goto IL_257;
				case 94:
				case 103:
					text2 = this.GetLocationName(94, data);
					goto IL_257;
				case 99:
					text2 = this.GetWorkStatusText(data);
					goto IL_257;
				case 100:
					text2 = this.GetWorkLocationName(data);
					goto IL_257;
				case 101:
					text2 = this.GetWorkPostName(data);
					goto IL_257;
				default:
					goto IL_245;
				}
			}
			text2 = CommonUtils.GetSkillGrowthString(data.GetInt(templateId), data.GetInt(97));
			goto IL_257;
			IL_245:
			text2 = data.GetInt(templateId).ToString();
			IL_257:
			if (!true)
			{
			}
			text = text2;
			result = true;
		}
		return result;
	}

	// Token: 0x06003099 RID: 12441 RVA: 0x0017CF40 File Offset: 0x0017B140
	private bool TryGetElementIcon(short templateId, CharacterTableDisplayData data, out string icon)
	{
		if (!true)
		{
		}
		string text;
		switch (templateId)
		{
		case 81:
			text = (this.GetElementValid(templateId, data) ? global::CharacterTable.PositiveFeatureIcon : global::CharacterTable.NegativeFeatureIcon)[0];
			goto IL_146;
		case 82:
			text = (this.GetElementValid(templateId, data) ? global::CharacterTable.PositiveFeatureIcon : global::CharacterTable.NegativeFeatureIcon)[1];
			goto IL_146;
		case 83:
			text = (this.GetElementValid(templateId, data) ? global::CharacterTable.PositiveFeatureIcon : global::CharacterTable.NegativeFeatureIcon)[2];
			goto IL_146;
		case 87:
			text = (this.GetElementValid(templateId, data) ? "ui_mousetip_combat_big_1_{0}".GetFormat(data.GetInt(templateId).ToString()) : null);
			goto IL_146;
		case 88:
			text = (this.GetElementValid(templateId, data) ? Misc.Instance[240 + data.GetInt(templateId)].Icon : null);
			goto IL_146;
		case 89:
			text = (this.GetElementValid(templateId, data) ? "ui_mousetip_combat_big_1_{0}".GetFormat(data.GetInt(templateId).ToString()) : null);
			goto IL_146;
		case 90:
			text = (this.GetElementValid(templateId, data) ? Misc.Instance[240 + data.GetInt(templateId)].Icon : null);
			goto IL_146;
		}
		text = null;
		IL_146:
		if (!true)
		{
		}
		icon = text;
		return icon != null;
	}

	// Token: 0x0600309A RID: 12442 RVA: 0x0017D0A4 File Offset: 0x0017B2A4
	private string GetLocationName(short templateId, CharacterTableDisplayData data)
	{
		CharacterTableLocationData locationData = data.GetLocation(templateId);
		string locationName = this.GetLocationNameOnly(templateId, data);
		bool flag = locationData.AdventureCoreId > 0;
		string result;
		if (flag)
		{
			result = LanguageKey.LK_LocationItem_InAdventure.TrFormat(locationName, AdventureRemakeModel.Core.GetAdventureAny(locationData.AdventureCoreId).Name).ColorReplace();
		}
		else
		{
			bool flag2 = locationData.KidnapperId >= 0;
			if (flag2)
			{
				result = LanguageKey.LK_LocationItem_Kidnapped.TrFormat(locationName, this.GetCharacterName(locationData.KidnapperId)).ColorReplace();
			}
			else
			{
				result = LanguageKey.LK_LocationItem_Normal.TrFormat(locationName).ColorReplace();
			}
		}
		return result;
	}

	// Token: 0x0600309B RID: 12443 RVA: 0x0017D140 File Offset: 0x0017B340
	private string GetLocationNameOnly(short templateId, CharacterTableDisplayData data)
	{
		CharacterTableLocationData locationData = data.GetLocation(94);
		Location location = new Location(locationData.AreaId, locationData.BlockId);
		bool isCapturedInStoneRoom = locationData.IsCapturedInStoneRoom;
		string locationName;
		if (isCapturedInStoneRoom)
		{
			locationName = LanguageKey.LK_Character_Location_Format_StoneHouse_2.TrFormat(Organization.DefValue.Taiwu.Name);
		}
		else
		{
			bool flag = !location.IsValid();
			if (flag)
			{
				locationName = LanguageKey.LK_Character_Location_Format_Invalid_2.Tr();
			}
			else
			{
				string stateName = MapState.Instance[locationData.StateTemplateId].Name;
				string areaName = MapArea.Instance[locationData.AreaTemplateId].Name;
				locationName = ((templateId == 94) ? (stateName + "-" + areaName) : string.Concat(new string[]
				{
					stateName,
					"-",
					areaName,
					"-",
					global::CharacterTable.WorldMapModel.GetBlockName(location.AreaId, location.BlockId, locationData.BlockTemplateId, (int)locationData.BlockIndex)
				}));
			}
		}
		return locationName;
	}

	// Token: 0x0600309C RID: 12444 RVA: 0x0017D248 File Offset: 0x0017B448
	private string GetWorkStatusText(CharacterTableDisplayData data)
	{
		CharacterTableLocationData locationData = data.GetLocation(99);
		CharacterTableWorkData workData = data.GetWork(99);
		string result;
		switch (workData.WorkStatus)
		{
		case 0:
			result = LanguageKey.LK_Villager_WorkStatus_Unemployed.Tr().SetColor("brightblue");
			break;
		case 1:
			result = LanguageKey.LK_Villager_WorkStatus_InTaiwuGroup.Tr().SetColor("brightred");
			break;
		case 2:
		{
			bool flag = workData.ArrangementTemplateId >= 0;
			if (flag)
			{
				string workName = VillagerRoleArrangement.Instance[workData.ArrangementTemplateId].ShortName;
				string locationName = (workData.ArrangementTemplateId == 13 && workData.IntValue >= 0) ? LocalStringManager.Get("LK_SwordTomb_{0}".GetFormat(workData.IntValue.ToString())) : MapArea.Instance[locationData.AreaTemplateId].Name;
				result = LanguageKey.LK_Villager_WorkStatus_Working.TrFormat(locationName, workName).SetColor("brightred");
			}
			else
			{
				bool flag2 = workData.WorkType == 1;
				if (flag2)
				{
					result = (workData.BoolValue ? LanguageKey.LK_VillagerSelection_Crafting.Tr() : LanguageKey.LK_VillagerSelection_Managing.Tr());
				}
				else
				{
					bool flag3 = workData.WorkType == 0;
					if (flag3)
					{
						result = (workData.BoolValue ? LanguageKey.LK_VillagerSelection_Building.Tr() : LanguageKey.LK_VillagerSelection_Removing.Tr());
					}
					else
					{
						string locationName2 = global::CharacterTable.WorldMapModel.GetBlockName(locationData.AreaId, locationData.BlockId, locationData.BlockTemplateId, -1);
						string workName2 = LocalStringManager.Get("LK_WorkType_{0}".GetFormat(workData.WorkType));
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

	// Token: 0x0600309D RID: 12445 RVA: 0x0017D474 File Offset: 0x0017B674
	private string GetWorkLocationName(CharacterTableDisplayData data)
	{
		CharacterTableWorkData workData = data.GetWork(99);
		BuildingBlockItem config = BuildingBlock.Instance[workData.IntValue];
		return config.Name;
	}

	// Token: 0x0600309E RID: 12446 RVA: 0x0017D4A8 File Offset: 0x0017B6A8
	private string GetWorkPostName(CharacterTableDisplayData data)
	{
		CharacterTableWorkData workData = data.GetWork(99);
		BuildingBlockItem config = BuildingBlock.Instance[workData.IntValue];
		return workData.IsLeader ? config.LeaderName : config.MemberName;
	}

	// Token: 0x0600309F RID: 12447 RVA: 0x0017D4EC File Offset: 0x0017B6EC
	private string GetCharacterName(int charId)
	{
		string charName;
		bool flag = this._characterNameData.TryGetValue(charId, out charName);
		string result;
		if (flag)
		{
			result = charName;
		}
		else
		{
			CharacterTableDisplayData data = this._characterData[charId];
			this._characterNameData[charId] = (this.useAnonymousName ? Character.Instance.GetItem(data.NameData.CharTemplateId).AnonymousTitle : NameCenter.GetMonasticTitleOrDisplayName(ref data.NameData, global::CharacterTable.TaiwuId == charId, false));
			result = this._characterNameData[charId];
		}
		return result;
	}

	// Token: 0x060030A0 RID: 12448 RVA: 0x0017D574 File Offset: 0x0017B774
	private List<short> GetCharacterTableElements()
	{
		List<short> types = new List<short>();
		foreach (global::CharacterTable.CharacterTableCommonFilterTypes type in this.commonFilterTypes)
		{
			global::CharacterTable.CharacterTableCommonFilterTypes characterTableCommonFilterTypes = type;
			global::CharacterTable.CharacterTableCommonFilterTypes characterTableCommonFilterTypes2 = characterTableCommonFilterTypes;
			if (characterTableCommonFilterTypes2 != global::CharacterTable.CharacterTableCommonFilterTypes.Fallen)
			{
				if (characterTableCommonFilterTypes2 - global::CharacterTable.CharacterTableCommonFilterTypes.Villager <= 1)
				{
					types.Add(105);
					types.Add(104);
					types.Add(106);
					types.Add(92);
				}
			}
			else
			{
				types.Add(98);
			}
		}
		foreach (CharacterTableItem config in ((IEnumerable<CharacterTableItem>)Config.CharacterTable.Instance))
		{
			bool flag = this.tableTypes.Contains(config.Type);
			if (flag)
			{
				foreach (short id in config.Elements)
				{
					types.Add(id);
				}
			}
		}
		bool flag2 = this._charValueCharacterTableTemplateId >= 0;
		if (flag2)
		{
			types.Add(this._charValueCharacterTableTemplateId);
		}
		return types;
	}

	// Token: 0x060030A1 RID: 12449 RVA: 0x0017D6D4 File Offset: 0x0017B8D4
	private bool GetCharacterHideAge(CharacterTableDisplayData data)
	{
		return Character.Instance[data.NameData.CharTemplateId].HideAge && CreatingType.IsNonEvolutionaryType(data.CreatingType);
	}

	// Token: 0x060030A2 RID: 12450 RVA: 0x0017D710 File Offset: 0x0017B910
	private bool GetCharacterHideProperty(CharacterTableDisplayData data)
	{
		return CreatingType.IsNonEvolutionaryType(data.CreatingType);
	}

	// Token: 0x060030A3 RID: 12451 RVA: 0x0017D730 File Offset: 0x0017B930
	private void SetText(Refers refers, CharacterTableDisplayData data, short elementTemplateId)
	{
		string text;
		this.TryGetElementText(elementTemplateId, data, out text);
		refers.CGet<TextMeshProUGUI>("TextLabel").SetText(text, true);
	}

	// Token: 0x060030A4 RID: 12452 RVA: 0x0017D75C File Offset: 0x0017B95C
	private void SetTextWithIcon(Refers refers, CharacterTableDisplayData data, short elementTemplateId)
	{
		CImage icon = refers.CGet<CImage>("Icon");
		HorizontalLayoutGroup layout = refers.CGet<HorizontalLayoutGroup>("Content");
		string sprite;
		bool flag = this.TryGetElementIcon(elementTemplateId, data, out sprite);
		if (flag)
		{
			icon.SetSprite(sprite, true, null);
			layout.spacing = (float)global::CharacterTable.TextWithIconSpacing[elementTemplateId];
			icon.gameObject.SetActive(true);
		}
		else
		{
			icon.gameObject.SetActive(false);
		}
		string text;
		this.TryGetElementText(elementTemplateId, data, out text);
		refers.CGet<TextMeshProUGUI>("TextLabel").SetText(text, true);
	}

	// Token: 0x060030A5 RID: 12453 RVA: 0x0017D7F0 File Offset: 0x0017B9F0
	private void SetTextWithSprite(Refers refers, CharacterTableDisplayData data, short elementTemplateId)
	{
		TextMeshProUGUI label = refers.CGet<TextMeshProUGUI>("TextLabel");
		string text;
		bool flag = this.TryGetElementText(elementTemplateId, data, out text);
		if (flag)
		{
			label.SetText(text, true);
			label.GetComponent<TMPTextSpriteHelper>().Parse();
		}
		else
		{
			label.SetText(text, true);
		}
	}

	// Token: 0x060030A6 RID: 12454 RVA: 0x0017D83C File Offset: 0x0017BA3C
	private void SetAvatar(Refers refers, CharacterTableDisplayData data)
	{
		Game.Components.Avatar.Avatar avatar = refers.CGet<Game.Components.Avatar.Avatar>("Avatar");
		TextMeshProUGUI nameLabel = refers.CGet<TextMeshProUGUI>("TextLabel");
		TooltipInvoker headTips = refers.CGet<TooltipInvoker>("HeadTips");
		CButtonObsolete headButton = headTips.GetComponent<CButtonObsolete>();
		CButtonObsolete nameAreaButton = refers.CGet<CButtonObsolete>("NameAreaButton");
		int charId = data.CharId;
		string nameText = this.GetCharacterName(charId);
		this._rows[data.CharId].avatar = avatar;
		avatar.Refresh(data.AvatarData, data.NameData.CharTemplateId);
		nameLabel.text = nameText;
		headTips.Type = TipType.CharacterOnMapBlock;
		TooltipInvoker tooltipInvoker = headTips;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		headTips.RuntimeParam.Set("charId", charId);
		bool flag = this._onAvatarBtnClicked != null;
		if (flag)
		{
			headButton.ClearAndAddListener(delegate
			{
				this._onAvatarBtnClicked(charId);
			});
			nameAreaButton.ClearAndAddListener(delegate
			{
				this._onAvatarBtnClicked(charId);
			});
		}
	}

	// Token: 0x060030A7 RID: 12455 RVA: 0x0017D958 File Offset: 0x0017BB58
	private void SetFeature(Refers refers, CharacterTableDisplayData data, short elementTemplateId)
	{
		int value = data.GetInt(elementTemplateId);
		List<CharacterFeatureView> prefabs = refers.CGetList<CharacterFeatureView>("Feature");
		foreach (CharacterFeatureView featureItem in prefabs)
		{
			featureItem.Set(CharacterFeature.Instance[value], -1, false);
		}
	}

	// Token: 0x060030A8 RID: 12456 RVA: 0x0017D9CC File Offset: 0x0017BBCC
	private void SetCommand(Refers refers, CharacterTableDisplayData data, short elementTemplateId)
	{
		string text;
		bool flag = !this.TryGetElementText(elementTemplateId, data, out text);
		if (flag)
		{
			refers.CGet<GameObject>("Command").SetActive(false);
		}
		else
		{
			List<TextMeshProUGUI> prefabs = refers.CGetList<TextMeshProUGUI>("NameLabel");
			foreach (TextMeshProUGUI commandNameLabel in prefabs)
			{
				commandNameLabel.text = text;
			}
			refers.CGet<GameObject>("Command").SetActive(true);
		}
	}

	// Token: 0x060030A9 RID: 12457 RVA: 0x0017DA64 File Offset: 0x0017BC64
	private int SortByName(CharacterTableDisplayData a, CharacterTableDisplayData b)
	{
		return Utils_Sorting.CompareByCurrentLangEncoding(this.GetCharacterName(a.CharId), this.GetCharacterName(b.CharId));
	}

	// Token: 0x060030AA RID: 12458 RVA: 0x0017DA94 File Offset: 0x0017BC94
	private int SortByCombatSkillGrowth(CharacterTableDisplayData a, CharacterTableDisplayData b)
	{
		sbyte valA = CommonUtils.GetSkillGrowthAddValue(a.GetInt(61), a.GetInt(97));
		sbyte valB = CommonUtils.GetSkillGrowthAddValue(b.GetInt(61), b.GetInt(97));
		return valA.CompareTo(valB);
	}

	// Token: 0x060030AB RID: 12459 RVA: 0x0017DADC File Offset: 0x0017BCDC
	private int SortByLifeSkillGrowth(CharacterTableDisplayData a, CharacterTableDisplayData b)
	{
		sbyte valueA = CommonUtils.GetSkillGrowthAddValue(a.GetInt(46), a.GetInt(97));
		sbyte valueB = CommonUtils.GetSkillGrowthAddValue(b.GetInt(46), b.GetInt(97));
		return valueA.CompareTo(valueB);
	}

	// Token: 0x060030AC RID: 12460 RVA: 0x0017DB24 File Offset: 0x0017BD24
	private int SortByOrganizationName(CharacterTableDisplayData a, CharacterTableDisplayData b)
	{
		OrganizationInfo valueA = a.GetOrg(91);
		OrganizationInfo valueB = b.GetOrg(91);
		return Utils_Sorting.CompareByCurrentLangEncoding(SingletonObject.getInstance<WorldMapModel>().GetSettlementName(valueA), SingletonObject.getInstance<WorldMapModel>().GetSettlementName(valueB));
	}

	// Token: 0x060030AD RID: 12461 RVA: 0x0017DB64 File Offset: 0x0017BD64
	private int SortByIdentity(CharacterTableDisplayData a, CharacterTableDisplayData b)
	{
		OrganizationInfo valueA = a.GetOrg(92);
		OrganizationInfo valueB = b.GetOrg(92);
		return valueA.Grade.CompareTo(valueB.Grade);
	}

	// Token: 0x060030AE RID: 12462 RVA: 0x0017DB9C File Offset: 0x0017BD9C
	private int SortByWeight(CharacterTableDisplayData a, CharacterTableDisplayData b)
	{
		int valueA = a.GetInt(78) - a.GetInt(79);
		int valueB = b.GetInt(78) - b.GetInt(79);
		return valueA.CompareTo(valueB);
	}

	// Token: 0x060030AF RID: 12463 RVA: 0x0017DBDC File Offset: 0x0017BDDC
	private int SortByLocation(CharacterTableDisplayData a, CharacterTableDisplayData b, short elementTemplateId)
	{
		return Utils_Sorting.CompareByCurrentLangEncoding(this.GetLocationNameOnly(elementTemplateId, a), this.GetLocationNameOnly(elementTemplateId, b));
	}

	// Token: 0x060030B0 RID: 12464 RVA: 0x0017DC04 File Offset: 0x0017BE04
	private int SortByWorkStatus(CharacterTableDisplayData a, CharacterTableDisplayData b)
	{
		CharacterTableWorkData dataA = a.GetWork(99);
		CharacterTableWorkData dataB = b.GetWork(99);
		bool flag = dataA.WorkStatus != dataB.WorkStatus;
		int result;
		if (flag)
		{
			int valueA = (dataA.WorkStatus == 2) ? int.MaxValue : ((int)dataA.WorkStatus);
			int valueB = (dataB.WorkStatus == 2) ? int.MaxValue : ((int)dataB.WorkStatus);
			result = valueA.CompareTo(valueB);
		}
		else
		{
			bool flag2 = dataA.WorkType != dataB.WorkType;
			if (flag2)
			{
				result = dataA.WorkType.CompareTo(dataB.WorkType);
			}
			else
			{
				result = dataA.ArrangementTemplateId.CompareTo(dataB.ArrangementTemplateId);
			}
		}
		return result;
	}

	// Token: 0x060030B1 RID: 12465 RVA: 0x0017DCBC File Offset: 0x0017BEBC
	private int SortByWorkBuilding(CharacterTableDisplayData a, CharacterTableDisplayData b)
	{
		CharacterTableWorkData valueA = a.GetWork(99);
		CharacterTableWorkData valueB = b.GetWork(99);
		return valueA.IntValue.CompareTo(valueB.IntValue);
	}

	// Token: 0x060030B2 RID: 12466 RVA: 0x0017DCF4 File Offset: 0x0017BEF4
	private int SortByWorkPost(CharacterTableDisplayData a, CharacterTableDisplayData b)
	{
		CharacterTableWorkData valueA = a.GetWork(99);
		CharacterTableWorkData valueB = b.GetWork(99);
		return valueA.IsLeader.CompareTo(valueB.IsLeader);
	}

	// Token: 0x04002324 RID: 8996
	public global::CharacterTable.CharacterTableInitMode initMode = global::CharacterTable.CharacterTableInitMode.ByCharList;

	// Token: 0x04002325 RID: 8997
	public bool initSelection;

	// Token: 0x04002326 RID: 8998
	public bool canSwitchSelection;

	// Token: 0x04002327 RID: 8999
	public bool activateSelectionWhenReady;

	// Token: 0x04002328 RID: 9000
	public bool multiSelection;

	// Token: 0x04002329 RID: 9001
	public bool canSelectAll;

	// Token: 0x0400232A RID: 9002
	public bool forceSelect;

	// Token: 0x0400232B RID: 9003
	public bool useAnonymousName;

	// Token: 0x0400232C RID: 9004
	public int minSelectCount;

	// Token: 0x0400232D RID: 9005
	public int maxSelectCount = -1;

	// Token: 0x0400232E RID: 9006
	public TextMeshProUGUI title;

	// Token: 0x0400232F RID: 9007
	public CToggleGroupObsolete tablePageToggleGroup;

	// Token: 0x04002330 RID: 9008
	public GameObject tablePageToggleTemplate;

	// Token: 0x04002331 RID: 9009
	public InfinityScrollLegacy infinityScroll;

	// Token: 0x04002332 RID: 9010
	public RectTransform tableHeadHolder;

	// Token: 0x04002333 RID: 9011
	public GameObject tableHeadTemplate;

	// Token: 0x04002334 RID: 9012
	public GameObject tableHeadCellTemplate;

	// Token: 0x04002335 RID: 9013
	public GameObject rowTemplate;

	// Token: 0x04002336 RID: 9014
	public GameObject textPrefab;

	// Token: 0x04002337 RID: 9015
	public GameObject emptyPrefab;

	// Token: 0x04002338 RID: 9016
	public GameObject textWithIconPrefab;

	// Token: 0x04002339 RID: 9017
	public GameObject textWithSpritePrefab;

	// Token: 0x0400233A RID: 9018
	public GameObject avatarPrefab;

	// Token: 0x0400233B RID: 9019
	public GameObject featurePrefab;

	// Token: 0x0400233C RID: 9020
	public GameObject commandPrefab;

	// Token: 0x0400233D RID: 9021
	public TMP_InputField searchByName;

	// Token: 0x0400233E RID: 9022
	public CToggleGroupObsolete filterToggleGroup;

	// Token: 0x0400233F RID: 9023
	public GameObject filterToggleTemplate;

	// Token: 0x04002340 RID: 9024
	public CommonSortAndFilter commonFilter;

	// Token: 0x04002341 RID: 9025
	private CharacterTableSortAndFilterController _commonFilterController;

	// Token: 0x04002342 RID: 9026
	public GameObject confirmBar;

	// Token: 0x04002343 RID: 9027
	public CButtonObsolete confirmBtn;

	// Token: 0x04002344 RID: 9028
	public CButtonObsolete cancelBtn;

	// Token: 0x04002345 RID: 9029
	public CButtonObsolete closeBtn;

	// Token: 0x04002346 RID: 9030
	public CommonSwitch canRowInteractSwitch;

	// Token: 0x04002347 RID: 9031
	public CToggleObsolete selectAllToggle;

	// Token: 0x04002348 RID: 9032
	public TextMeshProUGUI selectText;

	// Token: 0x04002349 RID: 9033
	public GameObject canRowInteractSwitchObject;

	// Token: 0x0400234A RID: 9034
	public List<ECharacterTableType> tableTypes = new List<ECharacterTableType>();

	// Token: 0x0400234B RID: 9035
	public List<global::CharacterTable.CharacterTableCommonFilterTypes> commonFilterTypes = new List<global::CharacterTable.CharacterTableCommonFilterTypes>();

	// Token: 0x0400234C RID: 9036
	private bool _inited;

	// Token: 0x0400234D RID: 9037
	private List<ECharacterTableType> _usingPages;

	// Token: 0x0400234E RID: 9038
	private readonly List<GameObject> _heads = new List<GameObject>();

	// Token: 0x0400234F RID: 9039
	private readonly List<List<Refers>> _headCells = new List<List<Refers>>();

	// Token: 0x04002350 RID: 9040
	private readonly Dictionary<int, CommonTableRow> _rows = new Dictionary<int, CommonTableRow>();

	// Token: 0x04002351 RID: 9041
	private readonly List<Refers> _rowPrefabs = new List<Refers>();

	// Token: 0x04002352 RID: 9042
	private Action<int> _onAvatarBtnClicked;

	// Token: 0x04002353 RID: 9043
	private readonly List<Refers> _features = new List<Refers>();

	// Token: 0x04002354 RID: 9044
	private List<int> _charList;

	// Token: 0x04002355 RID: 9045
	private readonly List<int> _sortedAndFilteredCharList = new List<int>();

	// Token: 0x04002356 RID: 9046
	private readonly Dictionary<int, CharacterTableDisplayData> _characterData = new Dictionary<int, CharacterTableDisplayData>();

	// Token: 0x04002357 RID: 9047
	private readonly Dictionary<int, string> _characterNameData = new Dictionary<int, string>();

	// Token: 0x04002358 RID: 9048
	private bool _canRowInteract;

	// Token: 0x04002359 RID: 9049
	private List<ItemKey> _itemKeyList;

	// Token: 0x0400235A RID: 9050
	private List<global::CharacterTable.CharacterTableFilterData> _filters;

	// Token: 0x0400235B RID: 9051
	private readonly HashSet<int> _commonFilteredCharIds = new HashSet<int>();

	// Token: 0x0400235C RID: 9052
	private string _searchInputText;

	// Token: 0x0400235D RID: 9053
	private readonly List<List<global::CharacterTable.CharacterTableSortData>> _sorters = new List<List<global::CharacterTable.CharacterTableSortData>>();

	// Token: 0x0400235E RID: 9054
	private readonly Dictionary<short, int> _highestCharIds = new Dictionary<short, int>();

	// Token: 0x0400235F RID: 9055
	private short _sortingElementTemplateId;

	// Token: 0x04002360 RID: 9056
	private HashSet<int> _initialSelectedChars;

	// Token: 0x04002361 RID: 9057
	private HashSet<int> _selectedChars;

	// Token: 0x04002362 RID: 9058
	private HashSet<int> _bannedChars;

	// Token: 0x04002363 RID: 9059
	private Action<int> _onRowClicked;

	// Token: 0x04002364 RID: 9060
	private bool _canSelectSpecialChar;

	// Token: 0x04002365 RID: 9061
	private readonly Dictionary<int, int> _characterValue = new Dictionary<int, int>();

	// Token: 0x04002366 RID: 9062
	private short _charValueCharacterTableTemplateId;

	// Token: 0x04002367 RID: 9063
	private Func<HashSet<int>, int> _getCurrValue;

	// Token: 0x04002368 RID: 9064
	private int _currPageIndex;

	// Token: 0x04002369 RID: 9065
	private int _currFilterIndex;

	// Token: 0x0400236A RID: 9066
	private readonly Dictionary<ECharacterTableType, CharacterTableItem> _configs = new Dictionary<ECharacterTableType, CharacterTableItem>();

	// Token: 0x0400236B RID: 9067
	private const string DisableText = "-";

	// Token: 0x0400236C RID: 9068
	private const string SwordTombNames = "LK_SwordTomb_{0}";

	// Token: 0x0400236D RID: 9069
	private const string WorkNames = "LK_WorkType_{0}";

	// Token: 0x0400236E RID: 9070
	private static readonly string[] PositiveFeatureIcon = new string[]
	{
		"ui_sp_icon_characteristic_10",
		"ui_sp_icon_characteristic_9",
		"ui_sp_icon_characteristic_11"
	};

	// Token: 0x0400236F RID: 9071
	private static readonly string[] NegativeFeatureIcon = new string[]
	{
		"ui_sp_icon_characteristic_4",
		"ui_sp_icon_characteristic_3",
		"ui_sp_icon_characteristic_5"
	};

	// Token: 0x04002370 RID: 9072
	private const string LegendaryBookIconSmall = "ui_mousetip_combat_big_1_{0}";

	// Token: 0x04002371 RID: 9073
	private const string EmptyCellImage = "ui_sp_items_base_0";

	// Token: 0x04002372 RID: 9074
	private const string NormalCellImage = "ui_sp_items_base_1";

	// Token: 0x04002373 RID: 9075
	private const string LastCellImage = "ui_sp_items_base_2";

	// Token: 0x04002374 RID: 9076
	private const string EmptyCellHoverImage = "ui_sp_items_base_3";

	// Token: 0x04002375 RID: 9077
	private const string NormalCellHoverImage = "ui_sp_items_base_4";

	// Token: 0x04002376 RID: 9078
	private const string LastCellHoverImage = "ui_sp_items_base_5";

	// Token: 0x04002377 RID: 9079
	private const string SpecialNormalCellImage = "ui_sp_character_base_0";

	// Token: 0x04002378 RID: 9080
	private const string SpecialLastCellImage = "ui_sp_character_base_2";

	// Token: 0x04002379 RID: 9081
	private const string SpecialNormalCellHoverImage = "ui_sp_character_base_1";

	// Token: 0x0400237A RID: 9082
	private const string SpecialLastCellHoverImage = "ui_sp_character_base_3";

	// Token: 0x0400237B RID: 9083
	private const string EmptyHeadCellImage = "ui_sp_title_5_2";

	// Token: 0x0400237C RID: 9084
	private const int TableElementWidthOffset = 2;

	// Token: 0x0400237D RID: 9085
	private static readonly Dictionary<short, int> TextWithIconSpacing = new Dictionary<short, int>
	{
		{
			81,
			10
		},
		{
			82,
			10
		},
		{
			83,
			10
		},
		{
			87,
			4
		},
		{
			89,
			4
		},
		{
			88,
			0
		},
		{
			90,
			0
		}
	};

	// Token: 0x0400237E RID: 9086
	private static readonly Dictionary<LocalStringManager.LanguageType, string> FeatureGameObjectNames = new Dictionary<LocalStringManager.LanguageType, string>
	{
		{
			LocalStringManager.LanguageType.CN,
			"FeatureCN"
		},
		{
			LocalStringManager.LanguageType.EN,
			"FeatureEN"
		}
	};

	// Token: 0x020016C9 RID: 5833
	public class CharacterTableFilterData
	{
		// Token: 0x0600D2B4 RID: 53940 RVA: 0x005B03B4 File Offset: 0x005AE5B4
		public CharacterTableFilterData(string filterName, string filterIcon, List<int> filterCharIds)
		{
			this.FilterName = filterName;
			this.FilterIcon = filterIcon;
			this.FilterCharIds = filterCharIds;
		}

		// Token: 0x0400A91B RID: 43291
		public string FilterName;

		// Token: 0x0400A91C RID: 43292
		public string FilterIcon;

		// Token: 0x0400A91D RID: 43293
		public List<int> FilterCharIds;
	}

	// Token: 0x020016CA RID: 5834
	public class CharacterTableSortData
	{
		// Token: 0x0600D2B5 RID: 53941 RVA: 0x005B03D3 File Offset: 0x005AE5D3
		public CharacterTableSortData(short elementTemplateId, bool isTableHead)
		{
			this.ElementTemplateId = elementTemplateId;
			this.IsDescending = true;
			this.IsTableHead = isTableHead;
		}

		// Token: 0x0400A91E RID: 43294
		public short ElementTemplateId;

		// Token: 0x0400A91F RID: 43295
		public bool IsDescending;

		// Token: 0x0400A920 RID: 43296
		public bool IsTableHead;
	}

	// Token: 0x020016CB RID: 5835
	public enum CharacterTableInitMode
	{
		// Token: 0x0400A922 RID: 43298
		ByCharList,
		// Token: 0x0400A923 RID: 43299
		ByNeedItem
	}

	// Token: 0x020016CC RID: 5836
	public enum CharacterTableCommonFilterTypes
	{
		// Token: 0x0400A925 RID: 43301
		Fallen,
		// Token: 0x0400A926 RID: 43302
		Villager,
		// Token: 0x0400A927 RID: 43303
		Character,
		// Token: 0x0400A928 RID: 43304
		Feast,
		// Token: 0x0400A929 RID: 43305
		Resident
	}
}
