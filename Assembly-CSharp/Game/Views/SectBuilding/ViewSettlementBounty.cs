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
using Game.Views.Building;
using Game.Views.CharacterMenu;
using Game.Views.Select;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using GameData.Domains.Organization;
using GameData.Domains.Organization.Display;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.SectBuilding
{
	// Token: 0x020007B6 RID: 1974
	public class ViewSettlementBounty : UIBase
	{
		// Token: 0x06006020 RID: 24608 RVA: 0x002C1700 File Offset: 0x002BF900
		private void OnClickButtonImprison()
		{
			TaiwuEventDomainMethod.Call.OnClickedSendPrisonBtn();
			this._needRefreshDataWhenFocus = true;
		}

		// Token: 0x06006021 RID: 24609 RVA: 0x002C1710 File Offset: 0x002BF910
		private static string GetHunterStateText(sbyte state)
		{
			if (!true)
			{
			}
			LanguageKey languageKey;
			switch (state)
			{
			case 0:
				languageKey = LanguageKey.LK_HunterState_NotInProgress;
				break;
			case 1:
				languageKey = LanguageKey.LK_HunterState_Hunting;
				break;
			case 2:
				languageKey = LanguageKey.LK_HunterState_Failed;
				break;
			case 3:
				languageKey = LanguageKey.LK_HunterState_Escorting;
				break;
			default:
				throw new ArgumentOutOfRangeException("state", state, null);
			}
			if (!true)
			{
			}
			LanguageKey key = languageKey;
			return LocalStringManager.Get(key).SetColor("orange");
		}

		// Token: 0x17000BC0 RID: 3008
		// (get) Token: 0x06006022 RID: 24610 RVA: 0x002C1786 File Offset: 0x002BF986
		private int _currentBountyTypeTog
		{
			get
			{
				return this.subBountyToggleGroup.GetActiveIndex();
			}
		}

		// Token: 0x06006023 RID: 24611 RVA: 0x002C1794 File Offset: 0x002BF994
		public override void OnInit(ArgumentBox argsBox)
		{
			bool flag = !argsBox.Get("SettlementId", out this._settlementId);
			if (flag)
			{
				this._settlementId = -1;
			}
			bool notShow;
			bool flag2 = argsBox.Get("NotDisplayButtonImprison", out notShow) && notShow;
			if (flag2)
			{
				this.buttonImprison.gameObject.SetActive(false);
			}
			else
			{
				this.buttonImprison.gameObject.SetActive(true);
			}
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.RefreshData));
			this._dataInited = false;
			SelectCharacterConfig config = new SelectCharacterConfig
			{
				SelectionMode = ESelectCharacterSelectionMode.Multiple,
				TargetCount = int.MaxValue
			};
			this.CacheInitData(config, new List<ISelectCharacterData>());
			this._pendingInitialize = true;
			this._uiInitialized = false;
			bool flag3 = !this._firstEnter;
			if (flag3)
			{
				this._firstEnter = true;
				GlobalDomainMethod.Call.InvokeGuidingTrigger(91);
			}
		}

		// Token: 0x06006024 RID: 24612 RVA: 0x002C1884 File Offset: 0x002BFA84
		private void RefreshData()
		{
			this.subBountyToggleGroup.SetToFirstInteractable(true);
			OrganizationDomainMethod.Call.GetSettlementBountyDisplayData(this.Element.GameDataListenerId, this._settlementId);
		}

		// Token: 0x06006025 RID: 24613 RVA: 0x002C18AC File Offset: 0x002BFAAC
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
			this._currentSubPage = EBountyCharacterListSubPage.Bounty;
		}

		// Token: 0x06006026 RID: 24614 RVA: 0x002C194F File Offset: 0x002BFB4F
		private void Setup()
		{
			if (this._config == null)
			{
				this._config = new SelectCharacterConfig();
			}
			this.InitSubPageToggles();
			this.InitSortAndFilter();
		}

		// Token: 0x06006027 RID: 24615 RVA: 0x002C1974 File Offset: 0x002BFB74
		private void Awake()
		{
			this.subBountyToggleGroup.Init(-1);
			this.subBountyToggleGroup.OnActiveIndexChange += this.OnActiveToggleChange;
			this.buttonImprison.ClearAndAddListener(delegate
			{
				this.OnClickButtonImprison();
			});
			this.scroll.RowInteractionEnabled = false;
			this.rowTemplate.gameObject.SetActive(false);
			this.avatarAndNameCellContainer.gameObject.SetActive(false);
			this.singleTextCellContainer.gameObject.SetActive(false);
			this.iconAndTextCellContainer.gameObject.SetActive(false);
			this.flatModeScroll.OnItemRender += this.FlatModeScroll_OnItemRender;
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
			}
			this.btnClose.ClearAndAddListener(delegate
			{
				UIManager.Instance.HideUI(this.Element);
			});
			this.switchToggleGroup.Init(0);
			this.switchToggleGroup.SetWithoutNotify(0);
			this.switchToggleGroup.OnActiveIndexChange += this.OnSwitchMode;
		}

		// Token: 0x06006028 RID: 24616 RVA: 0x002C1B5C File Offset: 0x002BFD5C
		private void OnActiveToggleChange(int newIndex, int oldIndex)
		{
			bool flag = newIndex >= 0;
			if (flag)
			{
				this.RefreshDispay();
			}
		}

		// Token: 0x06006029 RID: 24617 RVA: 0x002C1B80 File Offset: 0x002BFD80
		private void OnSwitchMode(int newIndex, int oldIndex)
		{
			this._listMode = (newIndex == 0);
			this.flatModeScroll.gameObject.SetActive(!this._listMode);
			this.scroll.gameObject.SetActive(this._listMode);
			this.subPageToggleGroup.gameObject.SetActive(this._listMode);
		}

		// Token: 0x0600602A RID: 24618 RVA: 0x002C1BE0 File Offset: 0x002BFDE0
		private void FlatModeScroll_OnItemRender(int index, GameObject itemObj)
		{
			SettlementBountyCharView charView = itemObj.GetComponent<SettlementBountyCharView>();
			BountyCharacterDataAdapter data = this._filteredDataList[index] as BountyCharacterDataAdapter;
			charView.Set(data.Data, (this._settlementBountyDisplayData == null) ? 0 : this._settlementBountyDisplayData.OrgTemplateId);
		}

		// Token: 0x0600602B RID: 24619 RVA: 0x002C1C2C File Offset: 0x002BFE2C
		private void OnEnable()
		{
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

		// Token: 0x0600602C RID: 24620 RVA: 0x002C1C90 File Offset: 0x002BFE90
		private void OnDestroy()
		{
			this.subBountyToggleGroup.OnActiveIndexChange -= this.OnActiveToggleChange;
			this.scroll.OnRowClicked -= this.OnRowClicked;
			this.flatModeScroll.OnItemRender -= this.FlatModeScroll_OnItemRender;
		}

		// Token: 0x0600602D RID: 24621 RVA: 0x002C1CE8 File Offset: 0x002BFEE8
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
				int baseCount = ViewSettlementBounty.BaseSubPageNameKeys.Count;
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
							label.text = ViewSettlementBounty.BaseSubPageNameKeys[i].Tr();
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
								toggleLabel.text = ViewSettlementBounty.BaseSubPageNameKeys[j].Tr();
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

		// Token: 0x0600602E RID: 24622 RVA: 0x002C20C0 File Offset: 0x002C02C0
		private void OnSubPageChanged(int newIndex, int oldIndex)
		{
			bool hasCustomSubPage = this._hasCustomSubPage;
			if (hasCustomSubPage)
			{
				bool flag = newIndex == this._customSubPageIndex;
				if (flag)
				{
					this._currentSubPage = (EBountyCharacterListSubPage)(-1);
				}
				else
				{
					this._currentSubPage = (EBountyCharacterListSubPage)((this._customSubPageIndex == 0 && this.subPageToggleGroup.GetAll().Count > ViewSettlementBounty.BaseSubPageNameKeys.Count) ? (newIndex - 1) : newIndex);
				}
			}
			else
			{
				this._currentSubPage = (EBountyCharacterListSubPage)newIndex;
			}
			this.RefreshList();
		}

		// Token: 0x0600602F RID: 24623 RVA: 0x002C2140 File Offset: 0x002C0340
		private void InitSortAndFilter()
		{
			bool flag = this.sortAndFilter == null;
			if (!flag)
			{
				this._sortAndFilterController = new SelectCharacterSortAndFilterController(this.sortAndFilter, this._filterMenuIds, this._defaultSortIds, false);
				this.scroll.SetSortController(this._sortAndFilterController);
				this._sortAndFilterController.Init(new Action(this.OnSortAndFilterChanged), "SelectCharacter");
			}
		}

		// Token: 0x06006030 RID: 24624 RVA: 0x002C21AD File Offset: 0x002C03AD
		private void OnSortAndFilterChanged()
		{
			this.RefreshListData();
		}

		// Token: 0x06006031 RID: 24625 RVA: 0x002C21B7 File Offset: 0x002C03B7
		private void RefreshList()
		{
			this.RefreshListStructure();
			this.RefreshListData();
		}

		// Token: 0x06006032 RID: 24626 RVA: 0x002C21C8 File Offset: 0x002C03C8
		private void RefreshListStructure()
		{
			bool flag = !this._dataInited;
			if (!flag)
			{
				IEnumerable<ColumnDefinition> columnDefinitions = this.GenerateColumnDefinitions();
				this.PrepareRowTemplateContainers();
				this.scroll.ClearInfinityScrollCache();
				this.scroll.Init<ISelectCharacterData>(columnDefinitions, false, null, null);
				this._listStructureReady = true;
			}
		}

		// Token: 0x06006033 RID: 24627 RVA: 0x002C2218 File Offset: 0x002C0418
		private void RefreshListData()
		{
			bool flag = !this._listStructureReady;
			if (flag)
			{
				this.RefreshListStructure();
			}
			List<ISelectCharacterData> searchFiltered = this.ApplySearch(this._dataList);
			Func<ISelectCharacterData, bool> filter = this._sortAndFilterController.GenerateFilter();
			this._filteredDataList.Clear();
			foreach (ISelectCharacterData data in searchFiltered)
			{
				bool flag2 = filter(data);
				if (flag2)
				{
					this._filteredDataList.Add(data);
				}
			}
			SelectCharacterSortAndFilterController sortAndFilterController = this._sortAndFilterController;
			Comparison<ISelectCharacterData> comparer = (sortAndFilterController != null) ? sortAndFilterController.GenerateComparer(this._filteredDataList) : null;
			bool flag3 = comparer != null;
			if (flag3)
			{
				this._filteredDataList.Sort(comparer);
			}
			this.scroll.SetData<ISelectCharacterData>(this._filteredDataList, this.GetSingleSelectedIndex());
			this.flatModeScroll.SetDataCount(this._filteredDataList.Count);
			this._sortAndFilterController.AfterFilter(this._filteredDataList);
		}

		// Token: 0x06006034 RID: 24628 RVA: 0x002C2330 File Offset: 0x002C0530
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
				Func<ISelectCharacterData, string> extractor = this._config.SearchTextExtractor ?? new Func<ISelectCharacterData, string>(ViewSettlementBounty.DefaultSearchTextExtractor);
				result = (from d in dataList
				where extractor(d).Contains(this._searchText)
				select d).ToList<ISelectCharacterData>();
			}
			return result;
		}

		// Token: 0x06006035 RID: 24629 RVA: 0x002C239C File Offset: 0x002C059C
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

		// Token: 0x06006036 RID: 24630 RVA: 0x002C23D2 File Offset: 0x002C05D2
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitions()
		{
			yield return this.CreateAvatarWithNameColumn();
			bool flag = this._hasCustomSubPage && this._currentSubPage < EBountyCharacterListSubPage.Bounty;
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
			case EBountyCharacterListSubPage.Bounty:
			{
				foreach (ColumnDefinition col2 in ViewSettlementBounty.GenerateBountyColumns((this._settlementBountyDisplayData == null) ? 0 : this._settlementBountyDisplayData.OrgTemplateId))
				{
					yield return col2;
					col2 = null;
				}
				IEnumerator<ColumnDefinition> enumerator2 = null;
				break;
			}
			case EBountyCharacterListSubPage.State:
			{
				foreach (ColumnDefinition col3 in ViewSettlementBounty.GenerateStateColumns())
				{
					yield return col3;
					col3 = null;
				}
				IEnumerator<ColumnDefinition> enumerator3 = null;
				break;
			}
			case EBountyCharacterListSubPage.Property:
			{
				foreach (ColumnDefinition col4 in ViewSettlementBounty.GeneratePropertyColumns())
				{
					yield return col4;
					col4 = null;
				}
				IEnumerator<ColumnDefinition> enumerator4 = null;
				break;
			}
			case EBountyCharacterListSubPage.Property2:
			{
				foreach (ColumnDefinition col5 in ViewSettlementBounty.GenerateProperty2Columns())
				{
					yield return col5;
					col5 = null;
				}
				IEnumerator<ColumnDefinition> enumerator5 = null;
				break;
			}
			case EBountyCharacterListSubPage.LifeSkill:
			{
				foreach (ColumnDefinition col6 in ViewSettlementBounty.GenerateLifeSkillColumns())
				{
					yield return col6;
					col6 = null;
				}
				IEnumerator<ColumnDefinition> enumerator6 = null;
				break;
			}
			case EBountyCharacterListSubPage.CombatSkill:
			{
				foreach (ColumnDefinition col7 in ViewSettlementBounty.GenerateCombatSkillColumns())
				{
					yield return col7;
					col7 = null;
				}
				IEnumerator<ColumnDefinition> enumerator7 = null;
				break;
			}
			case EBountyCharacterListSubPage.Personality:
			{
				foreach (ColumnDefinition col8 in ViewSettlementBounty.GeneratePersonalityColumns())
				{
					yield return col8;
					col8 = null;
				}
				IEnumerator<ColumnDefinition> enumerator8 = null;
				break;
			}
			case EBountyCharacterListSubPage.Item:
			{
				foreach (ColumnDefinition col9 in ViewSettlementBounty.GenerateItemColumns())
				{
					yield return col9;
					col9 = null;
				}
				IEnumerator<ColumnDefinition> enumerator9 = null;
				break;
			}
			case EBountyCharacterListSubPage.Command:
			{
				foreach (ColumnDefinition col10 in ViewSettlementBounty.GenerateCommandColumns())
				{
					yield return col10;
					col10 = null;
				}
				IEnumerator<ColumnDefinition> enumerator10 = null;
				break;
			}
			}
			yield break;
			yield break;
		}

		// Token: 0x06006037 RID: 24631 RVA: 0x002C23E4 File Offset: 0x002C05E4
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
				return AvatarWithNameCellData.FromCharacterDisplayDataForGeneralScrollList(generalData, false, new Action<int>(this.OpenCharacterMenu), null);
			};
			columnDefinition.SortId = 0;
			return columnDefinition;
		}

		// Token: 0x06006038 RID: 24632 RVA: 0x002C2474 File Offset: 0x002C0674
		private void OpenCharacterMenu(int charId)
		{
			UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", charId).Set("PreviousView", 6).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.CharacterBase, ECharacterSubPage.None)));
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x06006039 RID: 24633 RVA: 0x002C24D0 File Offset: 0x002C06D0
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

		// Token: 0x0600603A RID: 24634 RVA: 0x002C2548 File Offset: 0x002C0748
		private static ColumnDefinition CreateTextColumnBounty(Func<string> headerKey, Func<CharacterDisplayDataForSettlementBounty, string> valueGetter, short sortId = -1, float minWidth = 30f, float preferredWidth = 90f)
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
				CellDataGenerator = ((ISelectCharacterData data) => valueGetter(((BountyCharacterDataAdapter)data).Data)),
				SortId = sortId
			};
		}

		// Token: 0x0600603B RID: 24635 RVA: 0x002C25C0 File Offset: 0x002C07C0
		private static ColumnDefinition CreateIconAndTextColumnBounty(Func<string> headerKey, Func<CharacterDisplayDataForSettlementBounty, IconAndTextCellData> valueGetter, short sortId = -1, float minWidth = 30f, float preferredWidth = 90f)
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
				CellDataGenerator = ((ISelectCharacterData data) => valueGetter(((BountyCharacterDataAdapter)data).Data)),
				SortId = sortId
			};
		}

		// Token: 0x0600603C RID: 24636 RVA: 0x002C2630 File Offset: 0x002C0830
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

		// Token: 0x0600603D RID: 24637 RVA: 0x002C2688 File Offset: 0x002C0888
		private static IEnumerable<ColumnDefinition> GenerateBountyColumns(int settlementId)
		{
			yield return ViewSettlementBounty.CreateTextColumnBounty(() => LanguageKey.LK_Law_CrimeLevel.Tr(), (CharacterDisplayDataForSettlementBounty data) => CommonUtils.GetCrimeSeverityName(data), 15, 30f, 330f);
			yield return ViewSettlementBounty.CreateTextColumnBounty(() => LanguageKey.LK_Sect_Wanted_Reason.Tr(), (CharacterDisplayDataForSettlementBounty data) => CommonUtils.GetCrimeNameString(data), -1, 30f, 328f);
			yield return ViewSettlementBounty.CreateIconAndTextColumnBounty(() => LanguageKey.LK_BountyAmount.Tr(), (CharacterDisplayDataForSettlementBounty data) => new IconAndTextCellData(CommonUtils.GetSettlementBountyIcon(settlementId), data.SettlementBounty.BountyAmount.ToString(), true, false, false, false), 16, 30f, 328f);
			yield return ViewSettlementBounty.CreateTextColumnBounty(() => LanguageKey.LK_HunterState_Short.Tr(), (CharacterDisplayDataForSettlementBounty data) => ViewSettlementBounty.GetHunterStateText(data.HunterState), -1, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumnBounty(() => LanguageKey.LK_Sect_Wanted_TimeRemain.Tr(), (CharacterDisplayDataForSettlementBounty data) => Math.Max(0, data.SettlementBounty.ExpireDate - SingletonObject.getInstance<BasicGameData>().CurrDate).ToString(), -1, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumnBounty(() => LanguageKey.LK_UI_Following_SubTitle_Location.Tr(), (CharacterDisplayDataForSettlementBounty data) => SingletonObject.getInstance<WorldMapModel>().GetFullBlockName(data.FullBlockName, true, true, false, false), -1, 30f, 90f);
			yield break;
		}

		// Token: 0x0600603E RID: 24638 RVA: 0x002C2698 File Offset: 0x002C0898
		private static string GetNameString(CharacterDisplayDataForSettlementBounty data)
		{
			PunishmentTypeItem config = PunishmentType.Instance[data.SettlementBounty.PunishmentType];
			return config.Name.SetColor(PunishmentSeverity.Instance[config.Severity].NameColor);
		}

		// Token: 0x0600603F RID: 24639 RVA: 0x002C26E0 File Offset: 0x002C08E0
		private static IEnumerable<ColumnDefinition> GenerateStateColumns()
		{
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Char_Age.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.PhysiologicalAge.ToString(), 8, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Health.Tr(), (CharacterDisplayDataForGeneralScrollList data) => CommonUtils.GetCharacterHealthInfo(data.Health, data.MaxLeftHealth, data.CharacterId).Item1, 10, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Charm.Tr(), (CharacterDisplayDataForGeneralScrollList data) => ViewSettlementBounty.GetCharmDisplayString(data), 9, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Behavior.Tr(), (CharacterDisplayDataForGeneralScrollList data) => CommonUtils.GetBehaviorString(data.BehaviorType), 57, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Happiness.Tr(), (CharacterDisplayDataForGeneralScrollList data) => CommonUtils.GetHappinessString(HappinessType.GetHappinessType(data.Happiness)), 12, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Favorability.Tr(), (CharacterDisplayDataForGeneralScrollList data) => ViewSettlementBounty.GetFavorDisplayString(data), 11, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Alertness.Tr(), (CharacterDisplayDataForGeneralScrollList data) => CommonUtils.GetAlertnessNameByValue(data.Alertness), 130, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Fame.Tr(), (CharacterDisplayDataForGeneralScrollList data) => CommonUtils.GetFameString(FameType.GetFameType(data.Fame)), 59, 30f, 90f);
			yield break;
		}

		// Token: 0x06006040 RID: 24640 RVA: 0x002C26EC File Offset: 0x002C08EC
		private static string GetCharmDisplayString(CharacterDisplayDataForGeneralScrollList data)
		{
			return CommonUtils.GetCharmLevelText(data.Charm, data.Gender, data.PhysiologicalAge, data.ClothDisplayId, CreatingType.IsFixedPresetType(data.CreatingType), data.FaceVisible);
		}

		// Token: 0x06006041 RID: 24641 RVA: 0x002C272C File Offset: 0x002C092C
		private static string GetFavorDisplayString(CharacterDisplayDataForGeneralScrollList data)
		{
			return CommonUtils.GetFavorStringByInteracted(data.FavorabilityToTaiwu, data.IsInteractedWithTaiwu);
		}

		// Token: 0x06006042 RID: 24642 RVA: 0x002C274F File Offset: 0x002C094F
		private static IEnumerable<ColumnDefinition> GeneratePropertyColumns()
		{
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Strength.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.MaxMainAttributes[0].ToString(), 60, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Dexterity.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.MaxMainAttributes[1].ToString(), 61, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Concentration.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.MaxMainAttributes[2].ToString(), 62, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Vitality.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.MaxMainAttributes[3].ToString(), 63, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Energy.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.MaxMainAttributes[4].ToString(), 64, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Intelligence.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.MaxMainAttributes[5].ToString(), 65, 30f, 90f);
			yield break;
		}

		// Token: 0x06006043 RID: 24643 RVA: 0x002C2758 File Offset: 0x002C0958
		private static IEnumerable<ColumnDefinition> GenerateProperty2Columns()
		{
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_HitType_0.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.HitValues[0].ToString(), 24, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_HitType_1.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.HitValues[1].ToString(), 25, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_HitType_2.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.HitValues[2].ToString(), 26, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_HitType_3.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.HitValues[3].ToString(), 27, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_AvoidType_0.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.AvoidValues[0].ToString(), 33, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_AvoidType_1.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.AvoidValues[1].ToString(), 34, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_AvoidType_2.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.AvoidValues[2].ToString(), 35, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_AvoidType_3.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.AvoidValues[3].ToString(), 36, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Qi_Disorder.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.DisorderOfQi.ToString(), 55, 30f, 90f);
			yield break;
		}

		// Token: 0x06006044 RID: 24644 RVA: 0x002C2761 File Offset: 0x002C0961
		private static IEnumerable<ColumnDefinition> GenerateLifeSkillColumns()
		{
			int num;
			for (int i = 0; i < 16; i = num + 1)
			{
				ViewSettlementBounty.<>c__DisplayClass75_0 CS$<>8__locals1 = new ViewSettlementBounty.<>c__DisplayClass75_0();
				CS$<>8__locals1.index = i;
				yield return ViewSettlementBounty.CreateTextColumn(() => LocalStringManager.Get(string.Format("LK_LifeSkillType_{0}", CS$<>8__locals1.index)), (CharacterDisplayDataForGeneralScrollList data) => data.LifeSkillQualifications[CS$<>8__locals1.index].ToString(), (short)(66 + CS$<>8__locals1.index), 40f, 60f);
				CS$<>8__locals1 = null;
				num = i;
			}
			yield break;
		}

		// Token: 0x06006045 RID: 24645 RVA: 0x002C276A File Offset: 0x002C096A
		private static IEnumerable<ColumnDefinition> GenerateCombatSkillColumns()
		{
			int num;
			for (int i = 0; i < 14; i = num + 1)
			{
				ViewSettlementBounty.<>c__DisplayClass76_0 CS$<>8__locals1 = new ViewSettlementBounty.<>c__DisplayClass76_0();
				CS$<>8__locals1.index = i;
				yield return ViewSettlementBounty.CreateTextColumn(() => LocalStringManager.Get(string.Format("LK_CombatSkillType_{0}", CS$<>8__locals1.index)), (CharacterDisplayDataForGeneralScrollList data) => data.CombatSkillQualifications[CS$<>8__locals1.index].ToString(), (short)(82 + CS$<>8__locals1.index), 40f, 60f);
				CS$<>8__locals1 = null;
				num = i;
			}
			yield break;
		}

		// Token: 0x06006046 RID: 24646 RVA: 0x002C2773 File Offset: 0x002C0973
		private static IEnumerable<ColumnDefinition> GeneratePersonalityColumns()
		{
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Personality_Calm_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[0].ToString(), 96, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Personality_Clever_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[1].ToString(), 97, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Personality_Enthusiastic_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[2].ToString(), 98, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Personality_Brave_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[3].ToString(), 99, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Personality_Firm_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[4].ToString(), 100, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Personality_Lucky_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[5].ToString(), 101, 30f, 90f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Personality_Perceptive_Name.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[6].ToString(), 102, 30f, 90f);
			yield break;
		}

		// Token: 0x06006047 RID: 24647 RVA: 0x002C277C File Offset: 0x002C097C
		private static IEnumerable<ColumnDefinition> GenerateItemColumns()
		{
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Food.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Resources[0].ToString(), 103, 40f, 60f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Wood.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Resources[1].ToString(), 104, 40f, 60f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Metal.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Resources[2].ToString(), 105, 40f, 60f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Jade.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Resources[3].ToString(), 106, 40f, 60f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Fabric.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Resources[4].ToString(), 107, 40f, 60f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Herb.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Resources[5].ToString(), 108, 40f, 60f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Money.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Resources[6].ToString(), 109, 40f, 60f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Authority.Tr(), (CharacterDisplayDataForGeneralScrollList data) => data.Resources[7].ToString(), 110, 40f, 60f);
			yield return ViewSettlementBounty.CreateTextColumn(() => LanguageKey.LK_Inventory.Tr(), (CharacterDisplayDataForGeneralScrollList data) => string.Format("{0:f1}/{1:f1}", (float)data.CurrInventoryLoad / 100f, (float)data.MaxInventoryLoad / 100f), 37, 30f, 90f);
			yield break;
		}

		// Token: 0x06006048 RID: 24648 RVA: 0x002C2785 File Offset: 0x002C0985
		private static IEnumerable<ColumnDefinition> GenerateCommandColumns()
		{
			yield return ViewSettlementBounty.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Attack.Tr(), (ISelectCharacterData data) => ViewSettlementBounty.CreateMedalCellData(data.GetGeneralScrollListData().AttackMedal, 0), 112, 30f, 90f);
			yield return ViewSettlementBounty.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Defence.Tr(), (ISelectCharacterData data) => ViewSettlementBounty.CreateMedalCellData(data.GetGeneralScrollListData().DefenceMedal, 1), 113, 30f, 90f);
			yield return ViewSettlementBounty.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Wisdom.Tr(), (ISelectCharacterData data) => ViewSettlementBounty.CreateMedalCellData(data.GetGeneralScrollListData().WisdomMedal, 2), 114, 30f, 90f);
			yield return ViewSettlementBounty.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_0.Tr(), (ISelectCharacterData data) => ViewSettlementBounty.CreateCommandCellData(data, 0), 115, 30f, 90f);
			yield return ViewSettlementBounty.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_1.Tr(), (ISelectCharacterData data) => ViewSettlementBounty.CreateCommandCellData(data, 1), 116, 30f, 90f);
			yield return ViewSettlementBounty.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_2.Tr(), (ISelectCharacterData data) => ViewSettlementBounty.CreateCommandCellData(data, 2), 117, 30f, 90f);
			yield break;
		}

		// Token: 0x06006049 RID: 24649 RVA: 0x002C2790 File Offset: 0x002C0990
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
				string iconName = ViewSettlementBounty.GetMedalIconName(medalCount, medalType);
				string text = string.Format(" x{0}", Mathf.Abs(medalCount));
				result = new IconAndTextCellData(iconName, text, true, false, false, false);
			}
			return result;
		}

		// Token: 0x0600604A RID: 24650 RVA: 0x002C27E4 File Offset: 0x002C09E4
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

		// Token: 0x0600604B RID: 24651 RVA: 0x002C2864 File Offset: 0x002C0A64
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

		// Token: 0x0600604C RID: 24652 RVA: 0x002C28F8 File Offset: 0x002C0AF8
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

		// Token: 0x0600604D RID: 24653 RVA: 0x002C2954 File Offset: 0x002C0B54
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

		// Token: 0x0600604E RID: 24654 RVA: 0x002C2A60 File Offset: 0x002C0C60
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitionsForCurrentSubPage()
		{
			bool flag = this._hasCustomSubPage && this._currentSubPage < EBountyCharacterListSubPage.Bounty;
			IEnumerable<ColumnDefinition> result;
			if (flag)
			{
				Func<IEnumerable<ColumnDefinition>> customColumnGenerator = this._config.CustomColumnGenerator;
				result = (((customColumnGenerator != null) ? customColumnGenerator() : null) ?? Enumerable.Empty<ColumnDefinition>());
			}
			else
			{
				EBountyCharacterListSubPage currentSubPage = this._currentSubPage;
				if (!true)
				{
				}
				IEnumerable<ColumnDefinition> enumerable;
				switch (currentSubPage)
				{
				case EBountyCharacterListSubPage.Bounty:
					enumerable = ViewSettlementBounty.GenerateBountyColumns((this._settlementBountyDisplayData == null) ? 0 : this._settlementBountyDisplayData.OrgTemplateId);
					break;
				case EBountyCharacterListSubPage.State:
					enumerable = ViewSettlementBounty.GenerateStateColumns();
					break;
				case EBountyCharacterListSubPage.Property:
					enumerable = ViewSettlementBounty.GeneratePropertyColumns();
					break;
				case EBountyCharacterListSubPage.Property2:
					enumerable = ViewSettlementBounty.GenerateProperty2Columns();
					break;
				case EBountyCharacterListSubPage.LifeSkill:
					enumerable = ViewSettlementBounty.GenerateLifeSkillColumns();
					break;
				case EBountyCharacterListSubPage.CombatSkill:
					enumerable = ViewSettlementBounty.GenerateCombatSkillColumns();
					break;
				case EBountyCharacterListSubPage.Personality:
					enumerable = ViewSettlementBounty.GeneratePersonalityColumns();
					break;
				case EBountyCharacterListSubPage.Item:
					enumerable = ViewSettlementBounty.GenerateItemColumns();
					break;
				case EBountyCharacterListSubPage.Command:
					enumerable = ViewSettlementBounty.GenerateCommandColumns();
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

		// Token: 0x0600604F RID: 24655 RVA: 0x002C2B54 File Offset: 0x002C0D54
		private int GetColumnCount()
		{
			bool flag = this._hasCustomSubPage && this._currentSubPage < EBountyCharacterListSubPage.Bounty;
			int result;
			if (flag)
			{
				Func<IEnumerable<ColumnDefinition>> customColumnGenerator = this._config.CustomColumnGenerator;
				result = ((customColumnGenerator != null) ? customColumnGenerator().Count<ColumnDefinition>() : 0);
			}
			else
			{
				EBountyCharacterListSubPage currentSubPage = this._currentSubPage;
				if (!true)
				{
				}
				int num;
				switch (currentSubPage)
				{
				case EBountyCharacterListSubPage.Bounty:
					num = 6;
					break;
				case EBountyCharacterListSubPage.State:
					num = 7;
					break;
				case EBountyCharacterListSubPage.Property:
					num = 6;
					break;
				case EBountyCharacterListSubPage.Property2:
					num = 9;
					break;
				case EBountyCharacterListSubPage.LifeSkill:
					num = 16;
					break;
				case EBountyCharacterListSubPage.CombatSkill:
					num = 14;
					break;
				case EBountyCharacterListSubPage.Personality:
					num = 7;
					break;
				case EBountyCharacterListSubPage.Item:
					num = 9;
					break;
				case EBountyCharacterListSubPage.Command:
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

		// Token: 0x06006050 RID: 24656 RVA: 0x002C2C08 File Offset: 0x002C0E08
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

		// Token: 0x06006051 RID: 24657 RVA: 0x002C2D2C File Offset: 0x002C0F2C
		private bool IsRowSelected(int index, object rowData)
		{
			ISelectCharacterData data = rowData as ISelectCharacterData;
			bool flag = data == null;
			return !flag && this._selectedCharId.Contains(data.CharacterId);
		}

		// Token: 0x06006052 RID: 24658 RVA: 0x002C2D68 File Offset: 0x002C0F68
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

		// Token: 0x06006053 RID: 24659 RVA: 0x002C2E3C File Offset: 0x002C103C
		private void OnSearchButtonClicked()
		{
			TMP_InputField tmp_InputField = this.searchInput;
			this._searchText = (((tmp_InputField != null) ? tmp_InputField.text : null) ?? string.Empty);
			this.RefreshListData();
		}

		// Token: 0x06006054 RID: 24660 RVA: 0x002C2E67 File Offset: 0x002C1067
		private void OnSearchInputEndEdit(string text)
		{
			this._searchText = text;
			this.RefreshListData();
		}

		// Token: 0x06006055 RID: 24661 RVA: 0x002C2E78 File Offset: 0x002C1078
		private void OnConfirmClicked()
		{
			this.QuickHide();
		}

		// Token: 0x06006056 RID: 24662 RVA: 0x002C2E84 File Offset: 0x002C1084
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

		// Token: 0x06006057 RID: 24663 RVA: 0x002C2F1C File Offset: 0x002C111C
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 1)
				{
					bool flag = notification.DomainId == 3;
					if (flag)
					{
						bool flag2 = notification.MethodId == 21;
						if (flag2)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._settlementBountyDisplayData);
							this._dataInited = true;
							this.Element.ShowAfterRefresh();
							this.HandleData();
							this.RefreshDispay();
						}
					}
				}
			}
		}

		// Token: 0x06006058 RID: 24664 RVA: 0x002C2FE4 File Offset: 0x002C11E4
		private void HandleData()
		{
			this._punishmentType.Clear();
			bool flag = this._settlementBountyDisplayData.BountyCharacterDisplayDataDict != null;
			if (flag)
			{
				HashSet<short> punishementTypeSet = new HashSet<short>();
				HashSet<sbyte> mapStateSet = new HashSet<sbyte>();
				foreach (KeyValuePair<int, CharacterDisplayDataForSettlementBounty> keyValuePair in this._settlementBountyDisplayData.BountyCharacterDisplayDataDict)
				{
					int num;
					CharacterDisplayDataForSettlementBounty characterDisplayDataForSettlementBounty;
					keyValuePair.Deconstruct(out num, out characterDisplayDataForSettlementBounty);
					CharacterDisplayDataForSettlementBounty data = characterDisplayDataForSettlementBounty;
					PunishmentTypeItem punishmentTypeConfig = (data.SettlementBounty.PunishmentType >= 0) ? PunishmentType.Instance[data.SettlementBounty.PunishmentType] : null;
					bool flag2 = punishmentTypeConfig == null;
					if (!flag2)
					{
						PunishmentSeverityItem punishmentSeverityItem = (data.SettlementBounty.PunishmentSeverity >= 0) ? PunishmentSeverity.Instance[data.SettlementBounty.PunishmentSeverity] : null;
						bool flag3 = punishmentSeverityItem == null;
						if (!flag3)
						{
							bool flag4 = !this._punishmentType.ContainsKey(punishmentTypeConfig.DisplayType);
							if (flag4)
							{
								this._punishmentType[punishmentTypeConfig.DisplayType] = new List<CharacterDisplayDataForSettlementBounty>();
							}
							this._punishmentType[punishmentTypeConfig.DisplayType].Add(data);
							bool flag5 = data.FullBlockName.stateTemplateId >= 0 && data.FullBlockName.areaTemplateId >= 0 && data.FullBlockName.BlockData != null;
							if (flag5)
							{
								mapStateSet.Add(data.FullBlockName.stateTemplateId);
							}
							bool flag6 = data.SettlementBounty.PunishmentType >= 0;
							if (flag6)
							{
								punishementTypeSet.Add(data.SettlementBounty.PunishmentType);
							}
						}
					}
				}
			}
		}

		// Token: 0x06006059 RID: 24665 RVA: 0x002C31CC File Offset: 0x002C13CC
		private void Update()
		{
			bool flag = !this._previousFocused && UIManager.Instance.IsFocusElement(this.Element) && this._needRefreshDataWhenFocus;
			if (flag)
			{
				this.RefreshData();
				this._needRefreshDataWhenFocus = false;
			}
			this._previousFocused = UIManager.Instance.IsFocusElement(this.Element);
		}

		// Token: 0x0600605A RID: 24666 RVA: 0x002C3228 File Offset: 0x002C1428
		private void RefreshDispay()
		{
			EPunishmentTypeDisplayType displayType = (EPunishmentTypeDisplayType)this._currentBountyTypeTog;
			List<CharacterDisplayDataForSettlementBounty> cachedData = new List<CharacterDisplayDataForSettlementBounty>();
			bool flag = this._punishmentType.ContainsKey(displayType);
			if (flag)
			{
				cachedData = this._punishmentType[displayType];
			}
			List<ISelectCharacterData> selectList = new List<ISelectCharacterData>();
			bool flag2 = selectList != null;
			if (flag2)
			{
				foreach (CharacterDisplayDataForSettlementBounty item in cachedData)
				{
					selectList.Add(new BountyCharacterDataAdapter(item, SingletonObject.getInstance<WorldMapModel>().GetCurrentStateTemplateId()));
				}
			}
			this._dataList.Clear();
			this._dataList.AddRange(selectList);
			this.UpdateDisplay();
		}

		// Token: 0x0600605B RID: 24667 RVA: 0x002C32EC File Offset: 0x002C14EC
		private void UpdateDisplay()
		{
			this.RefreshListData();
			this.RefreshButtons();
		}

		// Token: 0x0600605C RID: 24668 RVA: 0x002C32FD File Offset: 0x002C14FD
		private void RefreshButtons()
		{
		}

		// Token: 0x04004299 RID: 17049
		private List<short> _defaultSortIds = new List<short>
		{
			0,
			151,
			8,
			10,
			15,
			16
		};

		// Token: 0x0400429A RID: 17050
		private static readonly List<LanguageKey> BaseSubPageNameKeys = new List<LanguageKey>
		{
			LanguageKey.LK_BountyAmount_Short,
			LanguageKey.LK_Team_Tog_State,
			LanguageKey.LK_Team_Tog_Property,
			LanguageKey.LK_Team_Tog_Property_Hit,
			LanguageKey.LK_Team_Tog_LifeSkill,
			LanguageKey.LK_Team_Tog_CombatSkill,
			LanguageKey.LK_Team_Tog_Personality,
			LanguageKey.LK_Team_Tog_Item,
			LanguageKey.LK_Team_Tog_Command
		};

		// Token: 0x0400429B RID: 17051
		[SerializeField]
		private CToggleGroup subPageToggleGroup;

		// Token: 0x0400429C RID: 17052
		[SerializeField]
		private ListStyleGeneralScroll scroll;

		// Token: 0x0400429D RID: 17053
		[SerializeField]
		private SortAndFilter sortAndFilter;

		// Token: 0x0400429E RID: 17054
		[Header("行模板配置")]
		[SerializeField]
		private RowItem rowTemplate;

		// Token: 0x0400429F RID: 17055
		[SerializeField]
		private RowCellContainer avatarAndNameCellContainer;

		// Token: 0x040042A0 RID: 17056
		[SerializeField]
		private RowCellContainer singleTextCellContainer;

		// Token: 0x040042A1 RID: 17057
		[SerializeField]
		private RowCellContainer iconAndTextCellContainer;

		// Token: 0x040042A2 RID: 17058
		[SerializeField]
		private TMP_InputField searchInput;

		// Token: 0x040042A3 RID: 17059
		[SerializeField]
		private CButton searchButton;

		// Token: 0x040042A4 RID: 17060
		[Header("平铺界面")]
		[SerializeField]
		private InfinityScroll flatModeScroll;

		// Token: 0x040042A5 RID: 17061
		[Header("交互")]
		[SerializeField]
		private CButton btnClose;

		// Token: 0x040042A6 RID: 17062
		[SerializeField]
		private CToggleGroup switchToggleGroup;

		// Token: 0x040042A7 RID: 17063
		[SerializeField]
		private CButton buttonImprison;

		// Token: 0x040042A8 RID: 17064
		[SerializeField]
		private CToggleGroup subBountyToggleGroup;

		// Token: 0x040042A9 RID: 17065
		private short _settlementId;

		// Token: 0x040042AA RID: 17066
		private SettlementBountyDisplayData _settlementBountyDisplayData;

		// Token: 0x040042AB RID: 17067
		private Dictionary<EPunishmentTypeDisplayType, List<CharacterDisplayDataForSettlementBounty>> _punishmentType = new Dictionary<EPunishmentTypeDisplayType, List<CharacterDisplayDataForSettlementBounty>>();

		// Token: 0x040042AC RID: 17068
		private SelectCharacterConfig _config;

		// Token: 0x040042AD RID: 17069
		private readonly List<ISelectCharacterData> _dataList = new List<ISelectCharacterData>();

		// Token: 0x040042AE RID: 17070
		private readonly List<ISelectCharacterData> _filteredDataList = new List<ISelectCharacterData>();

		// Token: 0x040042AF RID: 17071
		private readonly HashSet<int> _bannedCharacterIds = new HashSet<int>();

		// Token: 0x040042B0 RID: 17072
		private SelectCharacterSortAndFilterController _sortAndFilterController;

		// Token: 0x040042B1 RID: 17073
		private List<ESelectCharacterFilterMenuId> _filterMenuIds = new List<ESelectCharacterFilterMenuId>
		{
			ESelectCharacterFilterMenuId.PunishmentSeverity,
			ESelectCharacterFilterMenuId.PunishmentType,
			ESelectCharacterFilterMenuId.CaseStatus,
			ESelectCharacterFilterMenuId.Location
		};

		// Token: 0x040042B2 RID: 17074
		private EBountyCharacterListSubPage _currentSubPage = EBountyCharacterListSubPage.Bounty;

		// Token: 0x040042B3 RID: 17075
		private bool _hasCustomSubPage;

		// Token: 0x040042B4 RID: 17076
		private int _customSubPageIndex = -1;

		// Token: 0x040042B5 RID: 17077
		private bool _listStructureReady;

		// Token: 0x040042B6 RID: 17078
		private CToggle _dynamicToggle;

		// Token: 0x040042B7 RID: 17079
		private string _searchText = string.Empty;

		// Token: 0x040042B8 RID: 17080
		private readonly Dictionary<int, RowItem> _rowTemplateCache = new Dictionary<int, RowItem>();

		// Token: 0x040042B9 RID: 17081
		private bool _pendingInitialize;

		// Token: 0x040042BA RID: 17082
		private bool _uiInitialized;

		// Token: 0x040042BB RID: 17083
		private bool _dataInited = false;

		// Token: 0x040042BC RID: 17084
		private HashSet<int> _selectedCharId = new HashSet<int>();

		// Token: 0x040042BD RID: 17085
		private bool _firstEnter;

		// Token: 0x040042BE RID: 17086
		private bool _listMode = true;

		// Token: 0x040042BF RID: 17087
		private bool _needRefreshDataWhenFocus = false;

		// Token: 0x040042C0 RID: 17088
		private bool _previousFocused = true;
	}
}
