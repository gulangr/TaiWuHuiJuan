using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.CellContent;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.SelectCharacter;
using GameData.Domains.Building;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Select
{
	// Token: 0x020007AD RID: 1965
	public class ViewSelectChicken : UIBase
	{
		// Token: 0x06005F58 RID: 24408 RVA: 0x002BBA70 File Offset: 0x002B9C70
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get<Action<List<int>>>("Callback", out this._callback);
			argsBox.Get<Action<List<int>>>("PreviewCallback", out this._onPreviewSelected);
			argsBox.Get<Action>("CancelCallback", out this._onCancel);
			argsBox.Get<List<GameData.Domains.Building.Chicken>>("ChickenList", out this._dataList);
			List<int> selectedChickenIdList;
			argsBox.Get<List<int>>("SelectedChickenIdList", out selectedChickenIdList);
			bool flag = selectedChickenIdList != null;
			if (flag)
			{
				this._selectedChickenIds.Clear();
				this._selectedChickenIds.AddRange(selectedChickenIdList);
			}
			else
			{
				this._selectedChickenIds = new List<int>();
			}
			argsBox.Get("MaxSelectCount", out this._maxSelectCount);
			argsBox.Get<List<sbyte>>("PersonalitySortTypes", out this._personalitySortTypes);
			argsBox.Get<Dictionary<int, short>>("ChickenIdToRoleDict", out this._chickenIdToRoleDict);
		}

		// Token: 0x06005F59 RID: 24409 RVA: 0x002BBB3A File Offset: 0x002B9D3A
		private void Setup()
		{
			this.InitSortAndFilter();
			this.RefreshList();
			this.UpdateSelectedArea();
		}

		// Token: 0x06005F5A RID: 24410 RVA: 0x002BBB54 File Offset: 0x002B9D54
		private void Awake()
		{
			this.rowTemplate.gameObject.SetActive(false);
			this.avatarAndNameCellContainer.gameObject.SetActive(false);
			this.singleTextCellContainer.gameObject.SetActive(false);
			this.iconAndTextCellContainer.gameObject.SetActive(false);
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
			bool flag4 = this.selectedAreaSwitchButton != null;
			if (flag4)
			{
				this.selectedAreaSwitchButton.onValueChanged.AddListener(new UnityAction<bool>(this.OnSelectedAreaSwitchClicked));
			}
			bool flag5 = this.confirm != null;
			if (flag5)
			{
				this.confirm.ClearAndAddListener(new Action(this.OnConfirmClicked));
			}
			bool flag6 = this.selectedScroll != null;
			if (flag6)
			{
				this.selectedScroll.OnItemRender += this.OnSelectedScrollItemRender;
			}
		}

		// Token: 0x06005F5B RID: 24411 RVA: 0x002BBCC4 File Offset: 0x002B9EC4
		private void OnEnable()
		{
			this.Setup();
		}

		// Token: 0x06005F5C RID: 24412 RVA: 0x002BBCD0 File Offset: 0x002B9ED0
		private void OnDestroy()
		{
			this.scroll.OnRowClicked -= this.OnRowClicked;
			bool flag = this.selectedScroll != null;
			if (flag)
			{
				this.selectedScroll.OnItemRender -= this.OnSelectedScrollItemRender;
			}
		}

		// Token: 0x06005F5D RID: 24413 RVA: 0x002BBD20 File Offset: 0x002B9F20
		private void InitSortAndFilter()
		{
			bool flag = this.sortAndFilter == null;
			if (!flag)
			{
				this._sortAndFilterController = new SelectChickenSortAndFilterController(this.sortAndFilter);
				this._sortAndFilterController.Init(new Action(this.OnSortAndFilterChanged), "SelectChicken");
				this.scroll.SetSortController(this._sortAndFilterController);
			}
		}

		// Token: 0x06005F5E RID: 24414 RVA: 0x002BBD80 File Offset: 0x002B9F80
		private void OnSortAndFilterChanged()
		{
			this.RefreshListData();
		}

		// Token: 0x06005F5F RID: 24415 RVA: 0x002BBD8A File Offset: 0x002B9F8A
		private void RefreshList()
		{
			this.RefreshListStructure();
			this.RefreshListData();
		}

		// Token: 0x06005F60 RID: 24416 RVA: 0x002BBD9C File Offset: 0x002B9F9C
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

		// Token: 0x06005F61 RID: 24417 RVA: 0x002BBE28 File Offset: 0x002BA028
		private void RefreshListData()
		{
			bool flag = !this._listStructureReady;
			if (flag)
			{
				this.RefreshListStructure();
			}
			SelectChickenSortAndFilterController sortAndFilterController = this._sortAndFilterController;
			Func<GameData.Domains.Building.Chicken, bool> func;
			if ((func = ((sortAndFilterController != null) ? sortAndFilterController.GenerateFilter() : null)) == null && (func = ViewSelectChicken.<>c.<>9__36_0) == null)
			{
				func = (ViewSelectChicken.<>c.<>9__36_0 = ((GameData.Domains.Building.Chicken _) => true));
			}
			Func<GameData.Domains.Building.Chicken, bool> filter = func;
			IEnumerable<GameData.Domains.Building.Chicken> filtered = this._dataList.Where(filter);
			this._filteredDataList.Clear();
			this._filteredDataList.AddRange(filtered);
			SelectChickenSortAndFilterController sortAndFilterController2 = this._sortAndFilterController;
			Comparison<GameData.Domains.Building.Chicken> comparer = (sortAndFilterController2 != null) ? sortAndFilterController2.GenerateComparer(this._filteredDataList) : null;
			bool flag2 = comparer != null;
			if (flag2)
			{
				this._filteredDataList.Sort(comparer);
			}
			this.scroll.SetData<GameData.Domains.Building.Chicken>(this._filteredDataList, -1);
			this.UpdateConfirmButtonState();
			bool hasData = this._filteredDataList != null && this._filteredDataList.Count > 0;
			this.selectedAreaSwitchButton.interactable = hasData;
			bool flag3 = !hasData;
			if (flag3)
			{
				this.selectedScrollArea.SetActive(false);
			}
		}

		// Token: 0x06005F62 RID: 24418 RVA: 0x002BBF36 File Offset: 0x002BA136
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitions()
		{
			yield return this.CreateAvatarWithNameColumn();
			foreach (ColumnDefinition col in ViewSelectChicken.GenerateStateColumns())
			{
				yield return col;
				col = null;
			}
			IEnumerator<ColumnDefinition> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06005F63 RID: 24419 RVA: 0x002BBF48 File Offset: 0x002BA148
		private ColumnDefinition CreateAvatarWithNameColumn()
		{
			ColumnDefinition<GameData.Domains.Building.Chicken, ChickenAvatarWithNameCellData> columnDefinition = new ColumnDefinition<GameData.Domains.Building.Chicken, ChickenAvatarWithNameCellData>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 330f,
				FlexibleWidth = 0f,
				PreferredWidth = 330f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Char_Name.Tr());
			columnDefinition.CellDataGenerator = ((GameData.Domains.Building.Chicken data) => new ChickenAvatarWithNameCellData(data.TemplateId));
			columnDefinition.SortId = -1;
			return columnDefinition;
		}

		// Token: 0x06005F64 RID: 24420 RVA: 0x002BBFEC File Offset: 0x002BA1EC
		private static ColumnDefinition CreateTextColumn(Func<string> headerKey, Func<GameData.Domains.Building.Chicken, string> valueGetter, short sortId = -1, float minWidth = 30f, float preferredWidth = 90f)
		{
			return new ColumnDefinition<GameData.Domains.Building.Chicken, string>
			{
				LayoutOption = new LayoutOption
				{
					MinWidth = minWidth,
					FlexibleWidth = 1f,
					PreferredWidth = preferredWidth,
					Priority = 1
				},
				TableHeadLabel = headerKey,
				CellDataGenerator = ((GameData.Domains.Building.Chicken data) => valueGetter(data)),
				SortId = sortId
			};
		}

		// Token: 0x06005F65 RID: 24421 RVA: 0x002BC064 File Offset: 0x002BA264
		private static ColumnDefinition CreateIconAndTextColumn(Func<string> headerKey, Func<GameData.Domains.Building.Chicken, IconAndTextCellData> valueGetter, short sortId = -1, float minWidth = 30f, float preferredWidth = 90f)
		{
			return new ColumnDefinition<GameData.Domains.Building.Chicken, IconAndTextCellData>
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

		// Token: 0x06005F66 RID: 24422 RVA: 0x002BC0BC File Offset: 0x002BA2BC
		private static IEnumerable<ColumnDefinition> GenerateStateColumns()
		{
			yield return ViewSelectChicken.CreateTextColumn(() => LanguageKey.LK_Grade_Title.Tr(), (GameData.Domains.Building.Chicken data) => CommonUtils.GetPreGradeText(Config.Chicken.Instance[data.TemplateId].Grade), -1, 30f, 90f);
			yield return ViewSelectChicken.CreateIconAndTextColumn(() => LanguageKey.LK_Personality_Short.Tr(), (GameData.Domains.Building.Chicken data) => new IconAndTextCellData
			{
				ShowIcon = true,
				Text = Config.Chicken.Instance[data.TemplateId].PersonalityValue.ToString(),
				IconName = "ui9_icon_building_personality_big_" + Config.Chicken.Instance[data.TemplateId].PersonalityType.ToString()
			}, 150, 30f, 90f);
			yield return ViewSelectChicken.CreateTextColumn(() => LanguageKey.LK_Favorability.Tr(), (GameData.Domains.Building.Chicken data) => data.Happiness.ToString(), -1, 30f, 90f);
			yield break;
		}

		// Token: 0x06005F67 RID: 24423 RVA: 0x002BC0C8 File Offset: 0x002BA2C8
		private static string GetCharmDisplayString(CharacterDisplayDataForGeneralScrollList data)
		{
			return CommonUtils.GetCharmLevelText(data.Charm, data.Gender, data.PhysiologicalAge, data.ClothDisplayId, CreatingType.IsFixedPresetType(data.CreatingType), data.FaceVisible);
		}

		// Token: 0x06005F68 RID: 24424 RVA: 0x002BC108 File Offset: 0x002BA308
		private static string GetFavorDisplayString(CharacterDisplayDataForGeneralScrollList data)
		{
			return CommonUtils.GetFavorStringByInteracted(data.FavorabilityToTaiwu, data.IsInteractedWithTaiwu);
		}

		// Token: 0x06005F69 RID: 24425 RVA: 0x002BC12C File Offset: 0x002BA32C
		private void PrepareRowTemplateContainers()
		{
			RowItem currentTemplate = this.CreateRowTemplateForSubPage();
			this.scroll.SetRowTemplate(currentTemplate);
			bool flag = this.selectedSlotTemplate != null && currentTemplate != null;
			if (flag)
			{
				this.selectedSlotTemplate.PrepareStructure(currentTemplate);
			}
		}

		// Token: 0x06005F6A RID: 24426 RVA: 0x002BC17C File Offset: 0x002BA37C
		private RowItem CreateRowTemplateForSubPage()
		{
			RowItem newTemplate = Object.Instantiate<RowItem>(this.rowTemplate, this.rowTemplate.transform.parent);
			newTemplate.gameObject.SetActive(false);
			Transform containerRoot = newTemplate.ContainerRoot;
			RowCellContainer avatarContainer = Object.Instantiate<RowCellContainer>(this.avatarAndNameCellContainer, containerRoot);
			avatarContainer.gameObject.SetActive(true);
			IEnumerable<ColumnDefinition> columnDefinitions = ViewSelectChicken.GenerateStateColumns();
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

		// Token: 0x06005F6B RID: 24427 RVA: 0x002BC288 File Offset: 0x002BA488
		private int GetColumnCount()
		{
			return 4;
		}

		// Token: 0x06005F6C RID: 24428 RVA: 0x002BC29C File Offset: 0x002BA49C
		private void OnRowClicked(int index, RowItem rowItem)
		{
			bool flag = index < 0 || index >= this._filteredDataList.Count;
			if (!flag)
			{
				GameData.Domains.Building.Chicken data = this._filteredDataList[index];
				int charId = data.Id;
				bool flag2 = this._bannedCharacterIds.Contains(charId);
				if (!flag2)
				{
					bool flag3 = this._selectedChickenIds.Contains(charId);
					if (flag3)
					{
						this._selectedChickenIds.Remove(charId);
					}
					else
					{
						bool flag4 = this._selectedChickenIds.Count >= this._maxSelectCount;
						if (flag4)
						{
							return;
						}
						this._selectedChickenIds.Add(charId);
					}
					this.UpdateSelectedArea();
					this.UpdateConfirmButtonState();
					this.RefreshListData();
				}
			}
		}

		// Token: 0x06005F6D RID: 24429 RVA: 0x002BC358 File Offset: 0x002BA558
		private bool IsRowSelected(int index, object rowData)
		{
			GameData.Domains.Building.Chicken data;
			bool flag;
			if (rowData is GameData.Domains.Building.Chicken)
			{
				data = (GameData.Domains.Building.Chicken)rowData;
				flag = (1 == 0);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			return !flag2 && this._selectedChickenIds.Contains(data.Id);
		}

		// Token: 0x06005F6E RID: 24430 RVA: 0x002BC39C File Offset: 0x002BA59C
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

		// Token: 0x06005F6F RID: 24431 RVA: 0x002BC3E0 File Offset: 0x002BA5E0
		private void UpdateSelectedArea()
		{
			this.selectedCountLabel.text = LanguageKey.LK_SelectChicken_Selected.TrFormat(this._selectedChickenIds.Count, this._maxSelectCount);
			this.selectedScroll.SetDataCount(this._maxSelectCount);
		}

		// Token: 0x06005F70 RID: 24432 RVA: 0x002BC434 File Offset: 0x002BA634
		private void OnSelectedScrollItemRender(int index, GameObject item)
		{
			SelectedSlotItem slotItem = item.GetComponent<SelectedSlotItem>();
			if (slotItem != null)
			{
				slotItem.RestoreRowItemReference();
			}
			this.RenderSlotModeItem(index, slotItem);
		}

		// Token: 0x06005F71 RID: 24433 RVA: 0x002BC460 File Offset: 0x002BA660
		private void RenderSlotModeItem(int index, SelectedSlotItem slotItem)
		{
			bool flag = slotItem == null;
			if (!flag)
			{
				bool isEmpty = index >= this._selectedChickenIds.Count;
				slotItem.SetSlotState(isEmpty);
				bool flag2 = isEmpty;
				if (!flag2)
				{
					this.RenderSelectedCharacter(index, slotItem);
				}
			}
		}

		// Token: 0x06005F72 RID: 24434 RVA: 0x002BC4A8 File Offset: 0x002BA6A8
		private void RenderInstantModeItem(int index, SelectedSlotItem slotItem)
		{
			bool flag = slotItem == null;
			if (!flag)
			{
				bool flag2 = index >= this._selectedChickenIds.Count;
				if (!flag2)
				{
					slotItem.SetSlotState(false);
					this.RenderSelectedCharacter(index, slotItem);
				}
			}
		}

		// Token: 0x06005F73 RID: 24435 RVA: 0x002BC4EC File Offset: 0x002BA6EC
		private void RenderSelectedCharacter(int index, SelectedSlotItem slotItem)
		{
			int charId = this._selectedChickenIds[index];
			GameData.Domains.Building.Chicken data = this._dataList.Find((GameData.Domains.Building.Chicken d) => d.Id == charId);
			RowItem rowItem = slotItem.CurrentRowItem;
			bool flag = rowItem == null;
			if (!flag)
			{
				List<ColumnDefinition> columnDefinitions = this.GenerateColumnDefinitions().ToList<ColumnDefinition>();
				rowItem.Init(columnDefinitions, true);
				rowItem.SetData(data, index < this._selectedChickenIds.Count - 1, null);
				rowItem.SetRowInteraction(true, charId, new Action<int, RowItem>(this.OnClickSelectedChicken));
			}
		}

		// Token: 0x06005F74 RID: 24436 RVA: 0x002BC58D File Offset: 0x002BA78D
		private void OnClickSelectedChicken(int chickenId, RowItem item)
		{
			this._selectedChickenIds.Remove(chickenId);
			this.UpdateSelectedArea();
			this.UpdateConfirmButtonState();
			this.RefreshListData();
		}

		// Token: 0x06005F75 RID: 24437 RVA: 0x002BC5B4 File Offset: 0x002BA7B4
		private void OnSelectedAreaSwitchClicked(bool activeSelectArea)
		{
			bool flag = this.selectedScrollArea != null;
			if (flag)
			{
				this.selectedScrollArea.SetActive(activeSelectArea);
			}
		}

		// Token: 0x06005F76 RID: 24438 RVA: 0x002BC5E1 File Offset: 0x002BA7E1
		private void OnSearchInputEndEdit(string text)
		{
			this._searchText = text;
			this.RefreshListData();
		}

		// Token: 0x06005F77 RID: 24439 RVA: 0x002BC5F2 File Offset: 0x002BA7F2
		private void OnConfirmClicked()
		{
			Action<List<int>> callback = this._callback;
			if (callback != null)
			{
				callback(new List<int>(this._selectedChickenIds));
			}
			base.QuickHide();
		}

		// Token: 0x06005F78 RID: 24440 RVA: 0x002BC61C File Offset: 0x002BA81C
		private void UpdateConfirmButtonState()
		{
			bool flag = this.confirm == null;
			if (!flag)
			{
				bool canConfirm = true;
				this.confirm.interactable = canConfirm;
			}
		}

		// Token: 0x040041F8 RID: 16888
		[SerializeField]
		private ListStyleGeneralScroll scroll;

		// Token: 0x040041F9 RID: 16889
		[SerializeField]
		private CButton confirm;

		// Token: 0x040041FA RID: 16890
		[SerializeField]
		private CButton close;

		// Token: 0x040041FB RID: 16891
		[SerializeField]
		private GameObject selectedScrollArea;

		// Token: 0x040041FC RID: 16892
		[SerializeField]
		private InfinityScroll selectedScroll;

		// Token: 0x040041FD RID: 16893
		[SerializeField]
		private SortAndFilter sortAndFilter;

		// Token: 0x040041FE RID: 16894
		[Header("行模板配置")]
		[SerializeField]
		private RowItem rowTemplate;

		// Token: 0x040041FF RID: 16895
		[SerializeField]
		private RowCellContainer avatarAndNameCellContainer;

		// Token: 0x04004200 RID: 16896
		[SerializeField]
		private RowCellContainer singleTextCellContainer;

		// Token: 0x04004201 RID: 16897
		[SerializeField]
		private RowCellContainer iconAndTextCellContainer;

		// Token: 0x04004202 RID: 16898
		[SerializeField]
		private TextMeshProUGUI selectedCountLabel;

		// Token: 0x04004203 RID: 16899
		[Header("已选列表配置")]
		[SerializeField]
		private SelectedSlotItem selectedSlotTemplate;

		// Token: 0x04004204 RID: 16900
		[Header("操作")]
		[SerializeField]
		private CToggle selectedAreaSwitchButton;

		// Token: 0x04004205 RID: 16901
		private readonly HashSet<int> _bannedCharacterIds = new HashSet<int>();

		// Token: 0x04004206 RID: 16902
		private SelectChickenSortAndFilterController _sortAndFilterController;

		// Token: 0x04004207 RID: 16903
		private bool _listStructureReady = false;

		// Token: 0x04004208 RID: 16904
		private CToggle _dynamicToggle;

		// Token: 0x04004209 RID: 16905
		private string _searchText = string.Empty;

		// Token: 0x0400420A RID: 16906
		private Action<List<int>> _callback;

		// Token: 0x0400420B RID: 16907
		private Action<List<int>> _onPreviewSelected;

		// Token: 0x0400420C RID: 16908
		private Action _onCancel;

		// Token: 0x0400420D RID: 16909
		private int _maxSelectCount;

		// Token: 0x0400420E RID: 16910
		private List<sbyte> _personalitySortTypes = new List<sbyte>();

		// Token: 0x0400420F RID: 16911
		private Dictionary<int, short> _chickenIdToRoleDict;

		// Token: 0x04004210 RID: 16912
		private List<GameData.Domains.Building.Chicken> _dataList = new List<GameData.Domains.Building.Chicken>();

		// Token: 0x04004211 RID: 16913
		private List<GameData.Domains.Building.Chicken> _filteredDataList = new List<GameData.Domains.Building.Chicken>();

		// Token: 0x04004212 RID: 16914
		private List<int> _selectedChickenIds = new List<int>();
	}
}
