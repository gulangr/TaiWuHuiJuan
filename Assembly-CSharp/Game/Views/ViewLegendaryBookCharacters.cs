using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UI.LanguageRule;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Components.Common;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.CellContent;
using Game.Components.SortAndFilter;
using Game.Views.CharacterMenu;
using GameData.Domains.Character.Creation;
using GameData.Domains.LegendaryBook;
using TMPro;
using UnityEngine;

namespace Game.Views
{
	// Token: 0x020006EB RID: 1771
	public class ViewLegendaryBookCharacters : UIBase
	{
		// Token: 0x17000A5D RID: 2653
		// (get) Token: 0x060053F9 RID: 21497 RVA: 0x0026E747 File Offset: 0x0026C947
		private int _taiwuCharId
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x060053FA RID: 21498 RVA: 0x0026E754 File Offset: 0x0026C954
		private void Awake()
		{
			this.InitSubPageToggles();
			this.InitSortAndFilter();
			this.PreCreateAllRowTemplates();
			this.InitListScroll();
			this.searchingField.onEndEdit.ResetListener(delegate(string _)
			{
				this.OnSortOrFilterChanged();
			});
			this.searchingField.onValueChanged.ResetListener(delegate(string _)
			{
				this.OnSortOrFilterChanged();
			});
			this.btnClose.ClearAndAddListener(new Action(this.QuickHide));
			this.togGroupLegendaryBook.Init(-1);
			ToggleGroupHotkeyController.Set(this.Element, this.togGroupLegendaryBook, 1, null);
			this.togGroupLegendaryBook.SetWithoutNotify(0);
			this.togGroupLegendaryBook.OnActiveIndexChange += this.OnActiveLegendaryTogChange;
			this.CacheLegendaryBookToggleMaxFontSize();
			this.RefreshLegendaryBookToggleLabels();
			this.togGroupFallenType.Init(-1);
			ToggleGroupHotkeyController.Set(this.Element, this.togGroupFallenType, 2, null);
			this.togGroupFallenType.SetWithoutNotify(0);
			this.togGroupFallenType.OnActiveIndexChange += this.OnActiveFallenTypeChange;
		}

		// Token: 0x060053FB RID: 21499 RVA: 0x0026E86B File Offset: 0x0026CA6B
		private void OnActiveLegendaryTogChange(int newTog, int oldTog)
		{
			this.RefreshDataLegendaryType();
			this.RefreshList();
		}

		// Token: 0x060053FC RID: 21500 RVA: 0x0026E87C File Offset: 0x0026CA7C
		private void OnActiveFallenTypeChange(int newTog, int oldTog)
		{
			this.RefreshDataFallenType();
			this.RefreshList();
		}

		// Token: 0x060053FD RID: 21501 RVA: 0x0026E890 File Offset: 0x0026CA90
		private void RefreshDataFallenType()
		{
			int activeIndex = this.togGroupFallenType.GetActiveIndex();
			if (!true)
			{
			}
			int newFallenType2;
			switch (activeIndex)
			{
			case 0:
				newFallenType2 = 1;
				break;
			case 1:
				newFallenType2 = 2;
				break;
			case 2:
				newFallenType2 = 3;
				break;
			default:
				newFallenType2 = -1;
				break;
			}
			if (!true)
			{
			}
			int newFallenType = newFallenType2;
			this._currentDataList.Clear();
			this._currentDataList.AddRange(from t in this._dataList
			where newFallenType < 0 || (int)t.BookOwnerState == newFallenType
			select t);
		}

		// Token: 0x060053FE RID: 21502 RVA: 0x0026E914 File Offset: 0x0026CB14
		private void RefreshDataLegendaryType()
		{
			int newBookType = this.togGroupLegendaryBook.GetActiveIndex() - 1;
			this._currentDataList.Clear();
			this._currentDataList.AddRange(from t in this._dataList
			where this.togGroupLegendaryBook.GetActiveIndex() == 0 || (int)t.BookType == newBookType
			select t);
			int tempIndex = 0;
			Func<LegendaryBookCharacterRelatedData, bool> <>9__1;
			foreach (CToggle item in this.togGroupLegendaryBook.GetAll())
			{
				newBookType = tempIndex - 1;
				bool flag2 = newBookType != -1;
				if (flag2)
				{
					IEnumerable<LegendaryBookCharacterRelatedData> dataList = this._dataList;
					Func<LegendaryBookCharacterRelatedData, bool> predicate;
					if ((predicate = <>9__1) == null)
					{
						predicate = (<>9__1 = ((LegendaryBookCharacterRelatedData t) => (int)t.BookType == newBookType));
					}
					bool flag = dataList.Any(predicate);
					Refers refers = item.GetComponent<Refers>();
					Color textColor = flag ? this.colorActiveToggleText : this.colorInactiveToggleText;
					refers.CGet<TextMeshProUGUI>("Title").color = textColor;
					refers.CGet<DisableStyleRoot>("DisableStyleRoot").SetStyleEffect(!flag, false);
					item.interactable = flag;
				}
				tempIndex++;
			}
		}

		// Token: 0x060053FF RID: 21503 RVA: 0x0026EA64 File Offset: 0x0026CC64
		private void OnDisable()
		{
			this._currentDataList.Clear();
			TabSortStateManager<LegendaryBookCharacterSubPage, LegendaryBookCharacterRelatedData> tabSortStateManager = this._tabSortStateManager;
			if (tabSortStateManager != null)
			{
				tabSortStateManager.ClearAll();
			}
			this._filteredDataList.Clear();
			Action onCharactersPageClose = this._onCharactersPageClose;
			if (onCharactersPageClose != null)
			{
				onCharactersPageClose();
			}
		}

		// Token: 0x06005400 RID: 21504 RVA: 0x0026EAA4 File Offset: 0x0026CCA4
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get("IsCompetitorMode", out this._isCompetitorMode);
			argsBox.Get<List<LegendaryBookCharacterRelatedData>>("CharDataList", out this._dataList);
			argsBox.Get<Action>("OnClose", out this._onCharactersPageClose);
			this.SetupPage(this._isCompetitorMode);
			this.title.text = (this._isCompetitorMode ? LanguageKey.LK_LegendaryBook_Button_Competitors.Tr() : LanguageKey.LK_LegendaryBook_Button_Fallen.Tr());
		}

		// Token: 0x06005401 RID: 21505 RVA: 0x0026EB20 File Offset: 0x0026CD20
		private void OnEnable()
		{
			bool isCompetitorMode = this._isCompetitorMode;
			if (isCompetitorMode)
			{
				this.RefreshDataLegendaryType();
			}
			else
			{
				this.RefreshDataFallenType();
				this.RefreshFallenTypeAmount();
			}
			this.RefreshList();
		}

		// Token: 0x06005402 RID: 21506 RVA: 0x0026EB5C File Offset: 0x0026CD5C
		private void RefreshFallenTypeAmount()
		{
			int index = 0;
			foreach (CToggle item in this.togGroupFallenType.GetAll())
			{
				if (!true)
				{
				}
				int fallenType2;
				switch (index)
				{
				case 0:
					fallenType2 = 1;
					break;
				case 1:
					fallenType2 = 2;
					break;
				case 2:
					fallenType2 = 3;
					break;
				default:
					fallenType2 = -1;
					break;
				}
				if (!true)
				{
				}
				int fallenType = fallenType2;
				int amount = this._dataList.Count((LegendaryBookCharacterRelatedData t) => (int)t.BookOwnerState == fallenType);
				Refers refers = item.GetComponent<Refers>();
				bool interactable = amount > 0;
				Color textColor = interactable ? this.colorActiveToggleText : this.colorInactiveToggleText;
				TextMeshProUGUI amountText = refers.CGet<TextMeshProUGUI>("Amount");
				amountText.text = LocalStringManager.GetFormat(LanguageKey.LK_Brackets_Fix, amount);
				amountText.color = textColor;
				refers.CGet<TextMeshProUGUI>("Title").color = textColor;
				int iconIndex = index + (interactable ? 0 : 3);
				refers.CGet<CImage>("Icon").SetSprite(string.Format("ui9_btn_legendbook_icon_obsessed_{0}", iconIndex), false, null);
				this.togGroupFallenType.Get(index).interactable = interactable;
				index++;
			}
		}

		// Token: 0x06005403 RID: 21507 RVA: 0x0026ECD0 File Offset: 0x0026CED0
		private void SetupPage(bool isCompetitorMode)
		{
			this.togGroupLegendaryBook.gameObject.SetActive(isCompetitorMode);
			this.togGroupFallenType.gameObject.SetActive(!isCompetitorMode);
		}

		// Token: 0x06005404 RID: 21508 RVA: 0x0026ECFC File Offset: 0x0026CEFC
		private void InitSubPageToggles()
		{
			this.subPageToggleGroup.Init(-1);
			ToggleGroupHotkeyController.Set(this.Element, this.subPageToggleGroup, 0, null);
			this.subPageToggleGroup.OnActiveIndexChange += delegate(int newIndex, int oldIndex)
			{
				this.OnSubPageChanged((LegendaryBookCharacterSubPage)newIndex);
			};
			this.CacheSubPageToggleMaxFontSize();
			this.RefreshSubPageToggleLabels();
		}

		// Token: 0x06005405 RID: 21509 RVA: 0x0026ED54 File Offset: 0x0026CF54
		private void CacheSubPageToggleMaxFontSize()
		{
			List<CToggle> toggles = this.subPageToggleGroup.GetAll();
			bool flag = toggles.Count == 0;
			if (!flag)
			{
				Transform transform = toggles[0].transform.Find("Label");
				TextMeshProUGUI label = (transform != null) ? transform.GetComponent<TextMeshProUGUI>() : null;
				bool flag2 = label != null;
				if (flag2)
				{
					this._subPageToggleMaxFontSize = label.fontSize;
				}
			}
		}

		// Token: 0x06005406 RID: 21510 RVA: 0x0026EDB8 File Offset: 0x0026CFB8
		private void RefreshSubPageToggleLabels()
		{
			List<CToggle> toggles = this.subPageToggleGroup.GetAll();
			for (int i = 0; i < toggles.Count; i++)
			{
				Transform transform = toggles[i].transform.Find("Label");
				TextMeshProUGUI label = (transform != null) ? transform.GetComponent<TextMeshProUGUI>() : null;
				bool flag = label == null;
				if (!flag)
				{
					label.text = ViewLegendaryBookCharacters.ToggleGroupNameKeys[i].Tr();
					TMPTextEnLayoutHelper.ApplySubPageToggleLabel(label, this._subPageToggleMaxFontSize);
					label.ForceMeshUpdate(false, false);
				}
			}
		}

		// Token: 0x06005407 RID: 21511 RVA: 0x0026EE48 File Offset: 0x0026D048
		private void CacheLegendaryBookToggleMaxFontSize()
		{
			bool flag = this.togGroupLegendaryBook.GetAll().Count <= 1;
			if (!flag)
			{
				Refers component = this.togGroupLegendaryBook.Get(1).GetComponent<Refers>();
				TextMeshProUGUI title = (component != null) ? component.CGet<TextMeshProUGUI>("Title") : null;
				bool flag2 = title != null;
				if (flag2)
				{
					this._legendaryBookToggleMaxFontSize = title.fontSize;
				}
			}
		}

		// Token: 0x06005408 RID: 21512 RVA: 0x0026EEAC File Offset: 0x0026D0AC
		private void RefreshLegendaryBookToggleLabels()
		{
			for (int type = 0; type < 14; type++)
			{
				Refers togCompBookPage = this.togGroupLegendaryBook.Get(type + 1).GetComponent<Refers>();
				MiscItem miscConfig = Misc.Instance[240 + type];
				togCompBookPage.CGet<CImage>("Icon").SetSprite(miscConfig.Icon, false, null);
				togCompBookPage.CGet<DisableStyleRoot>("DisableStyleRoot").SetStyleEffect(false, false);
				TextMeshProUGUI title = togCompBookPage.CGet<TextMeshProUGUI>("Title");
				title.text = LocalStringManager.Get(string.Format("LK_LegendaryBook_{0}", type));
				TMPTextEnLayoutHelper.ApplySubPageToggleLabel(title, this._legendaryBookToggleMaxFontSize);
			}
		}

		// Token: 0x06005409 RID: 21513 RVA: 0x0026EF5C File Offset: 0x0026D15C
		public override void OnLanguageChange(LocalStringManager.LanguageType languageType)
		{
			base.OnLanguageChange(languageType);
			this.RefreshSubPageToggleLabels();
			bool isCompetitorMode = this._isCompetitorMode;
			if (isCompetitorMode)
			{
				this.RefreshLegendaryBookToggleLabels();
			}
			bool isActiveAndEnabled = base.isActiveAndEnabled;
			if (isActiveAndEnabled)
			{
				this.RefreshList();
			}
		}

		// Token: 0x0600540A RID: 21514 RVA: 0x0026EF9C File Offset: 0x0026D19C
		private void InitSortAndFilter()
		{
			bool flag = this.sortAndFilter == null;
			if (!flag)
			{
				this._sortAndFilterController = new LegendaryBookCharSortAndFilterController(this.sortAndFilter, new Func<int, bool>(this.IsTaiwu), new Func<int, bool>(this.IsSpecialTeammate));
				this._sortAndFilterController.Init(new Action(this.OnSortOrFilterChanged), "LegendaryBookCharacter");
				bool flag2 = this.listScroll != null;
				if (flag2)
				{
					this.listScroll.SetSortController(this._sortAndFilterController);
				}
				this._tabSortStateManager = new TabSortStateManager<LegendaryBookCharacterSubPage, LegendaryBookCharacterRelatedData>(this._sortAndFilterController);
			}
		}

		// Token: 0x0600540B RID: 21515 RVA: 0x0026F037 File Offset: 0x0026D237
		private void InitListScroll()
		{
		}

		// Token: 0x0600540C RID: 21516 RVA: 0x0026F03A File Offset: 0x0026D23A
		private void ShowCharacterMenu(int index)
		{
			this.ShowCharacterMenu(this._filteredDataList[index]);
		}

		// Token: 0x0600540D RID: 21517 RVA: 0x0026F050 File Offset: 0x0026D250
		private void ShowCharacterMenu(LegendaryBookCharacterRelatedData data)
		{
			UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", data.Id).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.CharacterBase, ECharacterSubPage.None)));
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x0600540E RID: 21518 RVA: 0x0026F0A5 File Offset: 0x0026D2A5
		private void RefreshList()
		{
			this.RefreshListStructure();
			this.RefreshListData();
		}

		// Token: 0x0600540F RID: 21519 RVA: 0x0026F0B8 File Offset: 0x0026D2B8
		private void RefreshListStructure()
		{
			IEnumerable<ColumnDefinition> columnDefinitions = this.GenerateColumnDefinitions(this._currentSubPage);
			this.PrepareRowTemplateContainers(this._currentSubPage);
			this.listScroll.ClearInfinityScrollCache();
			this.listScroll.Init<LegendaryBookCharacterRelatedData>(columnDefinitions, true, null, null);
			this.BindCellStyleProvider();
		}

		// Token: 0x06005410 RID: 21520 RVA: 0x0026F104 File Offset: 0x0026D304
		private void RefreshListData()
		{
			LegendaryBookCharSortAndFilterController sortAndFilterController = this._sortAndFilterController;
			Func<LegendaryBookCharacterRelatedData, bool> func;
			if ((func = ((sortAndFilterController != null) ? sortAndFilterController.GenerateFilter() : null)) == null && (func = ViewLegendaryBookCharacters.<>c.<>9__49_0) == null)
			{
				func = (ViewLegendaryBookCharacters.<>c.<>9__49_0 = ((LegendaryBookCharacterRelatedData _) => true));
			}
			Func<LegendaryBookCharacterRelatedData, bool> filter = func;
			LegendaryBookCharSortAndFilterController sortAndFilterController2 = this._sortAndFilterController;
			Comparison<LegendaryBookCharacterRelatedData> comparer = (sortAndFilterController2 != null) ? sortAndFilterController2.GenerateComparer(this._currentDataList) : null;
			bool flag;
			if (comparer != null)
			{
				TabSortStateManager<LegendaryBookCharacterSubPage, LegendaryBookCharacterRelatedData> tabSortStateManager = this._tabSortStateManager;
				flag = (tabSortStateManager != null && tabSortStateManager.ShouldSort());
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				this._currentDataList.Sort(comparer);
			}
			else
			{
				this._currentDataList.Sort((LegendaryBookCharacterRelatedData x, LegendaryBookCharacterRelatedData y) => x.Id.CompareTo(y.Id));
			}
			this._filteredDataList = this._currentDataList.Where(filter).ToList<LegendaryBookCharacterRelatedData>();
			this.listScroll.SetData<LegendaryBookCharacterRelatedData>(this._filteredDataList, -1);
		}

		// Token: 0x06005411 RID: 21521 RVA: 0x0026F1E4 File Offset: 0x0026D3E4
		private void BindCellStyleProvider()
		{
			bool flag = this.listScroll == null;
			if (!flag)
			{
				this.listScroll.CellStyleProvider = delegate(ListStyleGeneralScroll.CellStyleContext context)
				{
					LegendaryBookCharacterRelatedData rowData = context.RowData as LegendaryBookCharacterRelatedData;
					bool flag2 = rowData == null;
					ListStyleGeneralScroll.CellStyle result;
					if (flag2)
					{
						result = ListStyleGeneralScroll.CellStyle.Default;
					}
					else
					{
						bool flag3 = context.ColumnIndex <= 0;
						if (flag3)
						{
							result = ListStyleGeneralScroll.CellStyle.Default;
						}
						else
						{
							bool flag4 = this._filteredDataList == null || this._filteredDataList.Count <= 1;
							if (flag4)
							{
								result = ListStyleGeneralScroll.CellStyle.Default;
							}
							else
							{
								int value;
								bool flag5 = !ViewLegendaryBookCharacters.TryGetComparableValue(this._currentSubPage, context.ColumnIndex, rowData, out value);
								if (flag5)
								{
									result = ListStyleGeneralScroll.CellStyle.Default;
								}
								else
								{
									int maxValue;
									bool flag6 = !ViewLegendaryBookCharacters.TryGetMaxComparableValue(this._currentSubPage, context.ColumnIndex, this._filteredDataList, out maxValue);
									if (flag6)
									{
										result = ListStyleGeneralScroll.CellStyle.Default;
									}
									else
									{
										bool flag7 = maxValue == int.MinValue;
										if (flag7)
										{
											result = ListStyleGeneralScroll.CellStyle.Default;
										}
										else
										{
											int minValue;
											bool flag8 = ViewLegendaryBookCharacters.TryGetMinComparableValue(this._currentSubPage, context.ColumnIndex, this._filteredDataList, out minValue) && minValue == maxValue;
											if (flag8)
											{
												result = ListStyleGeneralScroll.CellStyle.Default;
											}
											else
											{
												result = new ListStyleGeneralScroll.CellStyle(value >= maxValue);
											}
										}
									}
								}
							}
						}
					}
					return result;
				};
			}
		}

		// Token: 0x06005412 RID: 21522 RVA: 0x0026F220 File Offset: 0x0026D420
		private unsafe static bool TryGetComparableValue(LegendaryBookCharacterSubPage subPage, int columnIndex, LegendaryBookCharacterRelatedData data, out int value)
		{
			value = 0;
			int idx = columnIndex - 1;
			bool result;
			switch (subPage)
			{
			case LegendaryBookCharacterSubPage.LegendaryBook:
			{
				ViewLegendaryBookCharacters.ELegendaryBookColumn col = (ViewLegendaryBookCharacters.ELegendaryBookColumn)idx;
				if (!true)
				{
				}
				int num;
				switch (col)
				{
				case ViewLegendaryBookCharacters.ELegendaryBookColumn.LegendaryBook:
					num = (int)data.BookType;
					break;
				case ViewLegendaryBookCharacters.ELegendaryBookColumn.Sect:
					num = (int)data.OrganizationInfo.OrgTemplateId;
					break;
				case ViewLegendaryBookCharacters.ELegendaryBookColumn.Role:
					num = (int)data.OrganizationInfo.Grade;
					break;
				case ViewLegendaryBookCharacters.ELegendaryBookColumn.ConsummateLevel:
					num = (int)data.ConsummateLevel;
					break;
				case ViewLegendaryBookCharacters.ELegendaryBookColumn.Location:
					num = (int)data.Location.AreaId;
					break;
				case ViewLegendaryBookCharacters.ELegendaryBookColumn.Feature:
					num = (int)data.FeatureId;
					break;
				default:
					num = 0;
					break;
				}
				if (!true)
				{
				}
				value = num;
				result = (idx >= 0 && idx < 10);
				break;
			}
			case LegendaryBookCharacterSubPage.State:
			{
				ViewLegendaryBookCharacters.EStateColumn col2 = (ViewLegendaryBookCharacters.EStateColumn)idx;
				bool flag = col2 == ViewLegendaryBookCharacters.EStateColumn.Favorability && !data.IsInteractedWithTaiwu;
				if (flag)
				{
					value = int.MinValue;
					result = true;
				}
				else
				{
					bool flag2 = col2 == ViewLegendaryBookCharacters.EStateColumn.Charm && !ViewLegendaryBookCharacters.IsCharmComparable(data);
					if (flag2)
					{
						value = int.MinValue;
						result = true;
					}
					else
					{
						if (!true)
						{
						}
						int num;
						switch (col2)
						{
						case ViewLegendaryBookCharacters.EStateColumn.Age:
							num = (int)data.PhysiologicalAge;
							goto IL_1A9;
						case ViewLegendaryBookCharacters.EStateColumn.DefeatMark:
							num = (int)data.DefeatMarkCount;
							goto IL_1A9;
						case ViewLegendaryBookCharacters.EStateColumn.Charm:
							num = (int)data.Charm;
							goto IL_1A9;
						case ViewLegendaryBookCharacters.EStateColumn.Happiness:
							num = (int)data.HappinessType;
							goto IL_1A9;
						case ViewLegendaryBookCharacters.EStateColumn.Favorability:
							num = (int)data.Favorability;
							goto IL_1A9;
						case ViewLegendaryBookCharacters.EStateColumn.Alertness:
							num = data.Alertness;
							goto IL_1A9;
						case ViewLegendaryBookCharacters.EStateColumn.Samsara:
							num = (int)data.PreexistenceCharCount;
							goto IL_1A9;
						case ViewLegendaryBookCharacters.EStateColumn.Fame:
							num = (int)data.FameType;
							goto IL_1A9;
						}
						num = 0;
						IL_1A9:
						if (!true)
						{
						}
						value = num;
						result = (idx >= 0 && idx < 10);
					}
				}
				break;
			}
			case LegendaryBookCharacterSubPage.Property:
			{
				ViewLegendaryBookCharacters.EPropertyColumn col3 = (ViewLegendaryBookCharacters.EPropertyColumn)idx;
				bool flag3 = idx >= 0 && idx <= 5;
				if (flag3)
				{
					value = (int)(*data.MaxMainAttributes[idx]);
					result = true;
				}
				else
				{
					if (!true)
					{
					}
					int num;
					switch (col3)
					{
					case ViewLegendaryBookCharacters.EPropertyColumn.PenetrateOuter:
						num = data.Penetrations.Outer;
						break;
					case ViewLegendaryBookCharacters.EPropertyColumn.PenetrateInner:
						num = data.Penetrations.Inner;
						break;
					case ViewLegendaryBookCharacters.EPropertyColumn.ResistOuter:
						num = data.PenetrationResists.Outer;
						break;
					case ViewLegendaryBookCharacters.EPropertyColumn.ResistInner:
						num = data.PenetrationResists.Inner;
						break;
					default:
						num = 0;
						break;
					}
					if (!true)
					{
					}
					value = num;
					result = (idx >= 0 && idx < 10);
				}
				break;
			}
			case LegendaryBookCharacterSubPage.Property2:
			{
				ViewLegendaryBookCharacters.EProperty2Column col4 = (ViewLegendaryBookCharacters.EProperty2Column)idx;
				bool flag4 = idx >= 0 && idx <= 3;
				if (flag4)
				{
					value = data.HitValues[idx];
					result = true;
				}
				else
				{
					bool flag5 = idx >= 4 && idx <= 7;
					if (flag5)
					{
						value = data.AvoidValues[idx - 4];
						result = true;
					}
					else
					{
						bool flag6 = col4 == ViewLegendaryBookCharacters.EProperty2Column.QiDisorder;
						if (flag6)
						{
							value = (int)(data.DisorderOfQi / 10);
							result = true;
						}
						else
						{
							result = false;
						}
					}
				}
				break;
			}
			case LegendaryBookCharacterSubPage.LifeSkill:
			{
				ViewLegendaryBookCharacters.ELifeSkillColumn col5 = (ViewLegendaryBookCharacters.ELifeSkillColumn)idx;
				bool flag7 = idx >= 0 && idx <= 15;
				if (flag7)
				{
					value = (int)(*data.LifeSkillQualifications[idx]);
					result = true;
				}
				else
				{
					bool flag8 = col5 == ViewLegendaryBookCharacters.ELifeSkillColumn.Growth;
					if (flag8)
					{
						value = (int)ViewLegendaryBookCharacters.GetSkillGrowthAddValue(data.ActualAge, (int)data.LifeSkillGrowthType);
						result = true;
					}
					else
					{
						result = false;
					}
				}
				break;
			}
			case LegendaryBookCharacterSubPage.CombatSkill:
			{
				ViewLegendaryBookCharacters.ECombatSkillColumn col6 = (ViewLegendaryBookCharacters.ECombatSkillColumn)idx;
				bool flag9 = idx >= 0 && idx <= 13;
				if (flag9)
				{
					value = (int)(*data.CombatSkillQualifications[idx]);
					result = true;
				}
				else
				{
					bool flag10 = col6 == ViewLegendaryBookCharacters.ECombatSkillColumn.Growth;
					if (flag10)
					{
						value = (int)ViewLegendaryBookCharacters.GetSkillGrowthAddValue(data.ActualAge, (int)data.CombatSkillGrowthType);
						result = true;
					}
					else
					{
						result = false;
					}
				}
				break;
			}
			case LegendaryBookCharacterSubPage.Personality:
			{
				bool flag11 = idx >= 0 && idx <= 6;
				if (flag11)
				{
					value = (int)(*data.Personalities[idx]);
					result = true;
				}
				else
				{
					result = false;
				}
				break;
			}
			case LegendaryBookCharacterSubPage.Item:
			{
				ViewLegendaryBookCharacters.EItemColumn col7 = (ViewLegendaryBookCharacters.EItemColumn)idx;
				bool flag12 = idx >= 0 && idx <= 7;
				if (flag12)
				{
					value = *data.Resources[idx];
					result = true;
				}
				else
				{
					if (!true)
					{
					}
					int num;
					if (col7 != ViewLegendaryBookCharacters.EItemColumn.InventoryLoad)
					{
						if (col7 != ViewLegendaryBookCharacters.EItemColumn.KidnapCount)
						{
							num = 0;
						}
						else
						{
							num = (int)data.KidnapCount;
						}
					}
					else
					{
						num = data.CurrInventoryLoad;
					}
					if (!true)
					{
					}
					value = num;
					result = (idx >= 0 && idx < 10);
				}
				break;
			}
			case LegendaryBookCharacterSubPage.Command:
			{
				ViewLegendaryBookCharacters.ECommandColumn col8 = (ViewLegendaryBookCharacters.ECommandColumn)idx;
				if (!true)
				{
				}
				int num;
				switch (col8)
				{
				case ViewLegendaryBookCharacters.ECommandColumn.AttackMedal:
					num = data.AttackMedal;
					break;
				case ViewLegendaryBookCharacters.ECommandColumn.DefenceMedal:
					num = data.DefenceMedal;
					break;
				case ViewLegendaryBookCharacters.ECommandColumn.WisdomMedal:
					num = data.WisdomMedal;
					break;
				case ViewLegendaryBookCharacters.ECommandColumn.Command0:
					num = (int)((data.Command.Items != null && data.Command.Items.CheckIndex(0)) ? data.Command.Items[0] : -1);
					break;
				case ViewLegendaryBookCharacters.ECommandColumn.Command1:
					num = (int)((data.Command.Items != null && data.Command.Items.CheckIndex(1)) ? data.Command.Items[1] : -1);
					break;
				case ViewLegendaryBookCharacters.ECommandColumn.Command2:
					num = (int)((data.Command.Items != null && data.Command.Items.CheckIndex(2)) ? data.Command.Items[2] : -1);
					break;
				default:
					num = 0;
					break;
				}
				if (!true)
				{
				}
				value = num;
				result = (idx >= 0 && idx < 6);
				break;
			}
			default:
				result = false;
				break;
			}
			return result;
		}

		// Token: 0x06005413 RID: 21523 RVA: 0x0026F7AC File Offset: 0x0026D9AC
		private static bool TryGetMaxComparableValue(LegendaryBookCharacterSubPage subPage, int columnIndex, List<LegendaryBookCharacterRelatedData> list, out int maxValue)
		{
			maxValue = int.MinValue;
			bool found = false;
			foreach (LegendaryBookCharacterRelatedData data in list)
			{
				int value;
				bool flag = !ViewLegendaryBookCharacters.TryGetComparableValue(subPage, columnIndex, data, out value);
				if (!flag)
				{
					found = true;
					bool flag2 = value > maxValue;
					if (flag2)
					{
						maxValue = value;
					}
				}
			}
			return found;
		}

		// Token: 0x06005414 RID: 21524 RVA: 0x0026F830 File Offset: 0x0026DA30
		private static bool TryGetMinComparableValue(LegendaryBookCharacterSubPage subPage, int columnIndex, List<LegendaryBookCharacterRelatedData> list, out int minValue)
		{
			minValue = int.MaxValue;
			bool found = false;
			foreach (LegendaryBookCharacterRelatedData data in list)
			{
				int value;
				bool flag = !ViewLegendaryBookCharacters.TryGetComparableValue(subPage, columnIndex, data, out value);
				if (!flag)
				{
					found = true;
					bool flag2 = value < minValue;
					if (flag2)
					{
						minValue = value;
					}
				}
			}
			return found;
		}

		// Token: 0x06005415 RID: 21525 RVA: 0x0026F8B4 File Offset: 0x0026DAB4
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitions(LegendaryBookCharacterSubPage subPage)
		{
			yield return this.CreateAvatarWithNameColumn();
			switch (subPage)
			{
			case LegendaryBookCharacterSubPage.LegendaryBook:
			{
				foreach (ColumnDefinition col in this.GenerateLegendaryBookColumns())
				{
					yield return col;
					col = null;
				}
				IEnumerator<ColumnDefinition> enumerator = null;
				break;
			}
			case LegendaryBookCharacterSubPage.State:
			{
				foreach (ColumnDefinition col2 in ViewLegendaryBookCharacters.GenerateStateColumns())
				{
					yield return col2;
					col2 = null;
				}
				IEnumerator<ColumnDefinition> enumerator2 = null;
				break;
			}
			case LegendaryBookCharacterSubPage.Property:
			{
				foreach (ColumnDefinition col3 in ViewLegendaryBookCharacters.GeneratePropertyColumns())
				{
					yield return col3;
					col3 = null;
				}
				IEnumerator<ColumnDefinition> enumerator3 = null;
				break;
			}
			case LegendaryBookCharacterSubPage.Property2:
			{
				foreach (ColumnDefinition col4 in ViewLegendaryBookCharacters.GenerateProperty2Columns())
				{
					yield return col4;
					col4 = null;
				}
				IEnumerator<ColumnDefinition> enumerator4 = null;
				break;
			}
			case LegendaryBookCharacterSubPage.LifeSkill:
			{
				foreach (ColumnDefinition col5 in ViewLegendaryBookCharacters.GenerateLifeSkillColumns())
				{
					yield return col5;
					col5 = null;
				}
				IEnumerator<ColumnDefinition> enumerator5 = null;
				break;
			}
			case LegendaryBookCharacterSubPage.CombatSkill:
			{
				foreach (ColumnDefinition col6 in ViewLegendaryBookCharacters.GenerateCombatSkillColumns())
				{
					yield return col6;
					col6 = null;
				}
				IEnumerator<ColumnDefinition> enumerator6 = null;
				break;
			}
			case LegendaryBookCharacterSubPage.Personality:
			{
				foreach (ColumnDefinition col7 in ViewLegendaryBookCharacters.GeneratePersonalityColumns())
				{
					yield return col7;
					col7 = null;
				}
				IEnumerator<ColumnDefinition> enumerator7 = null;
				break;
			}
			case LegendaryBookCharacterSubPage.Item:
			{
				foreach (ColumnDefinition col8 in ViewLegendaryBookCharacters.GenerateItemColumns())
				{
					yield return col8;
					col8 = null;
				}
				IEnumerator<ColumnDefinition> enumerator8 = null;
				break;
			}
			case LegendaryBookCharacterSubPage.Command:
			{
				foreach (ColumnDefinition col9 in ViewLegendaryBookCharacters.GenerateCommandColumns())
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

		// Token: 0x06005416 RID: 21526 RVA: 0x0026F8CB File Offset: 0x0026DACB
		private IEnumerable<RowCellContainer> GetCellContainerTemplates(LegendaryBookCharacterSubPage subPage)
		{
			yield return this.avatarAndNameCellContainer;
			if (subPage != LegendaryBookCharacterSubPage.LegendaryBook)
			{
				if (subPage != LegendaryBookCharacterSubPage.Command)
				{
					int columnCount = this.GetColumnCount(subPage);
					int num;
					for (int i = 0; i < columnCount; i = num + 1)
					{
						yield return this.singleTextCellContainer;
						num = i;
					}
				}
				else
				{
					int num;
					for (int j = 0; j < 6; j = num + 1)
					{
						yield return this.iconAndTextCellContainer;
						num = j;
					}
				}
			}
			else
			{
				bool flag = this._isCompetitorMode || this.togGroupFallenType.GetActiveIndex() != 2;
				if (flag)
				{
					yield return this.legendaryBookCellContainer;
				}
				yield return this.iconAndTextCellContainer;
				bool isCompetitorMode = this._isCompetitorMode;
				if (isCompetitorMode)
				{
					yield return this.singleTextCellContainer;
				}
				yield return this.iconAndTextCellContainer;
				yield return this.singleTextCellContainer;
				bool flag2 = !this._isCompetitorMode;
				if (flag2)
				{
					yield return this.featureCellContainer;
				}
			}
			yield break;
		}

		// Token: 0x06005417 RID: 21527 RVA: 0x0026F8E4 File Offset: 0x0026DAE4
		private int GetColumnCount(LegendaryBookCharacterSubPage subPage)
		{
			if (!true)
			{
			}
			int result;
			switch (subPage)
			{
			case LegendaryBookCharacterSubPage.LegendaryBook:
				result = this.GetLegendaryBookColumeAmount();
				break;
			case LegendaryBookCharacterSubPage.State:
				result = 10;
				break;
			case LegendaryBookCharacterSubPage.Property:
				result = 10;
				break;
			case LegendaryBookCharacterSubPage.Property2:
				result = 9;
				break;
			case LegendaryBookCharacterSubPage.LifeSkill:
				result = 17;
				break;
			case LegendaryBookCharacterSubPage.CombatSkill:
				result = 15;
				break;
			case LegendaryBookCharacterSubPage.Personality:
				result = 7;
				break;
			case LegendaryBookCharacterSubPage.Item:
				result = 10;
				break;
			case LegendaryBookCharacterSubPage.Command:
				result = 6;
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

		// Token: 0x06005418 RID: 21528 RVA: 0x0026F960 File Offset: 0x0026DB60
		private int GetLegendaryBookColumeAmount()
		{
			int amount = 6;
			bool isCompetitorMode = this._isCompetitorMode;
			int result;
			if (isCompetitorMode)
			{
				result = amount - 1;
			}
			else
			{
				amount--;
				bool flag = this.togGroupFallenType.GetActiveIndex() == 2;
				if (flag)
				{
					amount--;
				}
				result = amount;
			}
			return result;
		}

		// Token: 0x06005419 RID: 21529 RVA: 0x0026F9A4 File Offset: 0x0026DBA4
		private void PrepareRowTemplateContainers(LegendaryBookCharacterSubPage subPage)
		{
			RowItem cachedTemplate;
			bool flag = subPage != LegendaryBookCharacterSubPage.LegendaryBook && this._rowTemplateCache.TryGetValue(subPage, out cachedTemplate);
			if (flag)
			{
				this.listScroll.SetRowTemplate(cachedTemplate);
			}
			else
			{
				RowItem newTemplate = this.CreateRowTemplateForSubPage(subPage);
				this._rowTemplateCache[subPage] = newTemplate;
				this.listScroll.SetRowTemplate(newTemplate);
			}
		}

		// Token: 0x0600541A RID: 21530 RVA: 0x0026FA00 File Offset: 0x0026DC00
		private RowItem CreateRowTemplateForSubPage(LegendaryBookCharacterSubPage subPage)
		{
			RowItem newTemplate = Object.Instantiate<RowItem>(this.rowTemplate, this.rowTemplate.transform.parent);
			newTemplate.gameObject.SetActive(false);
			Transform containerRoot = newTemplate.ContainerRoot;
			foreach (RowCellContainer containerTemplate in this.GetCellContainerTemplates(subPage))
			{
				RowCellContainer container = Object.Instantiate<RowCellContainer>(containerTemplate, containerRoot);
				container.gameObject.SetActive(true);
			}
			newTemplate.ResetSibling();
			return newTemplate;
		}

		// Token: 0x0600541B RID: 21531 RVA: 0x0026FAA4 File Offset: 0x0026DCA4
		private void PreCreateAllRowTemplates()
		{
			this.rowTemplate.gameObject.SetActive(false);
			foreach (object obj in Enum.GetValues(typeof(LegendaryBookCharacterSubPage)))
			{
				LegendaryBookCharacterSubPage subPage = (LegendaryBookCharacterSubPage)obj;
				RowItem template = this.CreateRowTemplateForSubPage(subPage);
				this._rowTemplateCache[subPage] = template;
			}
		}

		// Token: 0x0600541C RID: 21532 RVA: 0x0026FB2C File Offset: 0x0026DD2C
		private ColumnDefinition CreateAvatarWithNameColumn()
		{
			ColumnDefinition<LegendaryBookCharacterRelatedData, AvatarWithNameCellData> columnDefinition = new ColumnDefinition<LegendaryBookCharacterRelatedData, AvatarWithNameCellData>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 330f,
				FlexibleWidth = 0f,
				PreferredWidth = 330f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Char_Name.Tr());
			columnDefinition.CellDataGenerator = ((LegendaryBookCharacterRelatedData data) => AvatarWithNameCellData.FromLegendaryBookCharacterRelatedData(data, this.IsTaiwu(data.Id), delegate(int _)
			{
				this.ShowCharacterMenu(data);
			}, delegate(TooltipInvoker displayer, int _)
			{
				displayer.Type = TipType.CharacterOnMapBlock;
				ArgumentBox argumentBox;
				if ((argumentBox = displayer.RuntimeParam) == null)
				{
					argumentBox = (displayer.RuntimeParam = EasyPool.Get<ArgumentBox>());
				}
				argumentBox.Set("CharId", data.Id);
			}));
			columnDefinition.SortId = 0;
			return columnDefinition;
		}

		// Token: 0x0600541D RID: 21533 RVA: 0x0026FBBC File Offset: 0x0026DDBC
		private static ColumnDefinition CreateTextColumn(Func<string> headerKey, Func<LegendaryBookCharacterRelatedData, string> valueGetter, short sortId = -1, float minWidth = 30f, float preferredWidth = 90f)
		{
			return new ColumnDefinition<LegendaryBookCharacterRelatedData, string>
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

		// Token: 0x0600541E RID: 21534 RVA: 0x0026FC1C File Offset: 0x0026DE1C
		private static ColumnDefinition CreateIconAndTextColumn(Func<string> headerKey, Func<LegendaryBookCharacterRelatedData, IconAndTextCellData> valueGetter, short sortId = -1, float minWidth = 80f, float preferredWidth = 120f)
		{
			return new ColumnDefinition<LegendaryBookCharacterRelatedData, IconAndTextCellData>
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

		// Token: 0x0600541F RID: 21535 RVA: 0x0026FC7C File Offset: 0x0026DE7C
		private static ColumnDefinition CreateLegendaryBookColumn(Func<string> headerKey, Func<LegendaryBookCharacterRelatedData, LegendaryBookCellData> valueGetter, short sortId = -1, float minWidth = 80f, float preferredWidth = 120f)
		{
			return new ColumnDefinition<LegendaryBookCharacterRelatedData, LegendaryBookCellData>
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

		// Token: 0x06005420 RID: 21536 RVA: 0x0026FCDC File Offset: 0x0026DEDC
		private static ColumnDefinition CreateFeatureColumn(Func<string> headerKey, Func<LegendaryBookCharacterRelatedData, LegendaryBookFeatureCellData> valueGetter, short sortId = -1, float minWidth = 80f, float preferredWidth = 120f)
		{
			return new ColumnDefinition<LegendaryBookCharacterRelatedData, LegendaryBookFeatureCellData>
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

		// Token: 0x06005421 RID: 21537 RVA: 0x0026FD3C File Offset: 0x0026DF3C
		private ColumnDefinition CreateLocationColumn(Func<string> headerKey, short sortId = -1, float minWidth = 300f, float preferredWidth = 300f)
		{
			WorldMapModel model = SingletonObject.getInstance<WorldMapModel>();
			return new ColumnDefinition<LegendaryBookCharacterRelatedData, SingleButtonCellData>
			{
				LayoutOption = new LayoutOption
				{
					MinWidth = minWidth,
					FlexibleWidth = 1f,
					PreferredWidth = preferredWidth,
					Priority = 1
				},
				TableHeadLabel = headerKey,
				CellDataGenerator = ((LegendaryBookCharacterRelatedData data) => new SingleButtonCellData
				{
					LabelText = CommonUtils.GetRelativeLocationName(data.LocationNameRelatedData),
					OnClick = delegate()
					{
						this.JumpToLocation(data);
					},
					SingleButtonCellStatus = SingleButtonCellStatus.DisableInteractable
				}),
				SortId = sortId
			};
		}

		// Token: 0x06005422 RID: 21538 RVA: 0x0026FDB0 File Offset: 0x0026DFB0
		private ColumnDefinition CreateLocationTextColumn(Func<string> headerKey, short sortId = -1, float minWidth = 300f, float preferredWidth = 300f)
		{
			WorldMapModel model = SingletonObject.getInstance<WorldMapModel>();
			ColumnDefinition<LegendaryBookCharacterRelatedData, string> columnDefinition = new ColumnDefinition<LegendaryBookCharacterRelatedData, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = minWidth,
				FlexibleWidth = 1f,
				PreferredWidth = preferredWidth,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = headerKey;
			columnDefinition.CellDataGenerator = ((LegendaryBookCharacterRelatedData data) => CommonUtils.GetRelativeLocationName(data.LocationNameRelatedData).SetColor("brightyellow"));
			columnDefinition.SortId = sortId;
			return columnDefinition;
		}

		// Token: 0x06005423 RID: 21539 RVA: 0x0026FE34 File Offset: 0x0026E034
		private void JumpToLocation(LegendaryBookCharacterRelatedData data)
		{
			WorldMapModel model = SingletonObject.getInstance<WorldMapModel>();
			bool flag = model.GetStateId(data.Location.AreaId) != model.CurrentStateId;
			if (!flag)
			{
				SingletonObject.getInstance<WorldMapModel>().JumpToTemporaryMark(data.Location, 0);
				this.QuickHide();
			}
		}

		// Token: 0x06005424 RID: 21540 RVA: 0x0026FE83 File Offset: 0x0026E083
		private IEnumerable<ColumnDefinition> GenerateLegendaryBookColumns()
		{
			bool flag = this._isCompetitorMode || this.togGroupFallenType.GetActiveIndex() != 2;
			if (flag)
			{
				yield return ViewLegendaryBookCharacters.CreateLegendaryBookColumn(() => LanguageKey.LK_MouseTipCharacter_InteractionType_8.Tr(), (LegendaryBookCharacterRelatedData data) => new LegendaryBookCellData(data.BookType), 129, 80f, 120f);
			}
			yield return ViewLegendaryBookCharacters.CreateIconAndTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Organization.Tr(), (LegendaryBookCharacterRelatedData data) => new IconAndTextCellData
			{
				IconName = (Config.Organization.Instance[data.OrganizationInfo.OrgTemplateId].IsSect ? string.Format("{0}{1}", "ui9_icon_largemap_secticon_", data.OrganizationInfo.OrgTemplateId) : string.Empty),
				ShowIcon = true,
				Text = Config.Organization.Instance[data.OrganizationInfo.OrgTemplateId].Name.SetColor("brightyellow"),
				HideIconIfEmpty = true,
				UseNativeSize = true
			}, 126, 80f, 120f);
			bool isCompetitorMode = this._isCompetitorMode;
			if (isCompetitorMode)
			{
				yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Identity.Tr(), (LegendaryBookCharacterRelatedData data) => CommonUtils.GetIdentityStringWithSpecialCharacterConfig((int)data.CharacterTemplateId, data.OrganizationInfo, data.Gender, data.PhysiologicalAge, false), 1, 30f, 90f);
			}
			yield return ViewLegendaryBookCharacters.CreateIconAndTextColumn(() => LanguageKey.LK_Mousetip_Sort_Desc_ConsummateLevel.Tr(), new Func<LegendaryBookCharacterRelatedData, IconAndTextCellData>(this.GetConsummateLevelCellData), 143, 80f, 120f);
			yield return this.CreateLocationTextColumn(() => LanguageKey.LK_VillagerInfo_Location.Tr(), 125, 300f, 300f);
			bool flag2 = !this._isCompetitorMode;
			if (flag2)
			{
				yield return ViewLegendaryBookCharacters.CreateFeatureColumn(() => LanguageKey.LK_Feature.Tr(), (LegendaryBookCharacterRelatedData data) => new LegendaryBookFeatureCellData(data.FeatureId, data.Id), 129, 80f, 120f);
			}
			yield break;
		}

		// Token: 0x06005425 RID: 21541 RVA: 0x0026FE94 File Offset: 0x0026E094
		private IconAndTextCellData GetConsummateLevelCellData(LegendaryBookCharacterRelatedData data)
		{
			string iconName;
			string consummateText = CommonUtils.GetConsummateLevelShowDataFull(data.ConsummateLevel, out iconName);
			return new IconAndTextCellData
			{
				IconName = iconName,
				ShowIcon = true,
				Text = consummateText
			};
		}

		// Token: 0x06005426 RID: 21542 RVA: 0x0026FED5 File Offset: 0x0026E0D5
		private static IEnumerable<ColumnDefinition> GenerateStateColumns()
		{
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Char_Age.Tr(), (LegendaryBookCharacterRelatedData data) => data.PhysiologicalAge.ToString(), 8, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Health.Tr(), (LegendaryBookCharacterRelatedData data) => CommonUtils.GetHealthString(data.HealthType), 10, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Injury.Tr(), (LegendaryBookCharacterRelatedData data) => data.DefeatMarkCount.ToString(), 53, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Charm.Tr(), (LegendaryBookCharacterRelatedData data) => ViewLegendaryBookCharacters.GetCharmDisplayString(data), 9, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Behavior.Tr(), (LegendaryBookCharacterRelatedData data) => CommonUtils.GetBehaviorString(data.BehaviorType), 57, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Happiness.Tr(), (LegendaryBookCharacterRelatedData data) => CommonUtils.GetHappinessString(data.HappinessType), 12, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Favorability.Tr(), new Func<LegendaryBookCharacterRelatedData, string>(ViewLegendaryBookCharacters.GetFavorDisplayString), 11, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Alertness.Tr(), (LegendaryBookCharacterRelatedData data) => CommonUtils.GetAlertnessNameByValue(data.Alertness), 130, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Samsara.Tr(), (LegendaryBookCharacterRelatedData data) => data.PreexistenceCharCount.ToString(), 58, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Fame.Tr(), (LegendaryBookCharacterRelatedData data) => CommonUtils.GetFameString(data.FameType), 59, 30f, 90f);
			yield break;
		}

		// Token: 0x06005427 RID: 21543 RVA: 0x0026FEDE File Offset: 0x0026E0DE
		private static IEnumerable<ColumnDefinition> GeneratePropertyColumns()
		{
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Strength.Tr(), (LegendaryBookCharacterRelatedData data) => data.MaxMainAttributes[0].ToString(), 60, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Dexterity.Tr(), (LegendaryBookCharacterRelatedData data) => data.MaxMainAttributes[1].ToString(), 61, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Concentration.Tr(), (LegendaryBookCharacterRelatedData data) => data.MaxMainAttributes[2].ToString(), 62, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Vitality.Tr(), (LegendaryBookCharacterRelatedData data) => data.MaxMainAttributes[3].ToString(), 63, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Energy.Tr(), (LegendaryBookCharacterRelatedData data) => data.MaxMainAttributes[4].ToString(), 64, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Intelligence.Tr(), (LegendaryBookCharacterRelatedData data) => data.MaxMainAttributes[5].ToString(), 65, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Penetrate_Outer.Tr(), (LegendaryBookCharacterRelatedData data) => data.Penetrations.Outer.ToString(), 22, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Penetrate_Inner.Tr(), (LegendaryBookCharacterRelatedData data) => data.Penetrations.Inner.ToString(), 23, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Penetrate_Resist_Outer.Tr(), (LegendaryBookCharacterRelatedData data) => data.PenetrationResists.Outer.ToString(), 29, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Penetrate_Resist_Inner.Tr(), (LegendaryBookCharacterRelatedData data) => data.PenetrationResists.Inner.ToString(), 30, 30f, 90f);
			yield break;
		}

		// Token: 0x06005428 RID: 21544 RVA: 0x0026FEE7 File Offset: 0x0026E0E7
		private static IEnumerable<ColumnDefinition> GenerateProperty2Columns()
		{
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_HitType_0.Tr(), (LegendaryBookCharacterRelatedData data) => data.HitValues[0].ToString(), 24, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_HitType_1.Tr(), (LegendaryBookCharacterRelatedData data) => data.HitValues[1].ToString(), 25, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_HitType_2.Tr(), (LegendaryBookCharacterRelatedData data) => data.HitValues[2].ToString(), 26, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_HitType_3.Tr(), (LegendaryBookCharacterRelatedData data) => data.HitValues[3].ToString(), 27, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_AvoidType_0.Tr(), (LegendaryBookCharacterRelatedData data) => data.AvoidValues[0].ToString(), 33, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_AvoidType_1.Tr(), (LegendaryBookCharacterRelatedData data) => data.AvoidValues[1].ToString(), 34, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_AvoidType_2.Tr(), (LegendaryBookCharacterRelatedData data) => data.AvoidValues[2].ToString(), 35, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_AvoidType_3.Tr(), (LegendaryBookCharacterRelatedData data) => data.AvoidValues[3].ToString(), 36, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Qi_Disorder.Tr(), (LegendaryBookCharacterRelatedData data) => ((int)(data.DisorderOfQi / 10)).ToString(), 55, 30f, 90f);
			yield break;
		}

		// Token: 0x06005429 RID: 21545 RVA: 0x0026FEF0 File Offset: 0x0026E0F0
		private static IEnumerable<ColumnDefinition> GenerateLifeSkillColumns()
		{
			int num;
			for (int i = 0; i < 16; i = num + 1)
			{
				ViewLegendaryBookCharacters.<>c__DisplayClass74_0 CS$<>8__locals1 = new ViewLegendaryBookCharacters.<>c__DisplayClass74_0();
				CS$<>8__locals1.index = i;
				yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LocalStringManager.Get(string.Format("LK_LifeSkillType_{0}", CS$<>8__locals1.index)), (LegendaryBookCharacterRelatedData data) => data.LifeSkillQualifications[CS$<>8__locals1.index].ToString(), (short)(66 + CS$<>8__locals1.index), 40f, 60f);
				CS$<>8__locals1 = null;
				num = i;
			}
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Growth.Tr(), (LegendaryBookCharacterRelatedData data) => ViewLegendaryBookCharacters.GetSkillGrowthString(data.ActualAge, data.LifeSkillGrowthType), 118, 30f, 90f);
			yield break;
		}

		// Token: 0x0600542A RID: 21546 RVA: 0x0026FEF9 File Offset: 0x0026E0F9
		private static IEnumerable<ColumnDefinition> GenerateCombatSkillColumns()
		{
			int num;
			for (int i = 0; i < 14; i = num + 1)
			{
				ViewLegendaryBookCharacters.<>c__DisplayClass75_0 CS$<>8__locals1 = new ViewLegendaryBookCharacters.<>c__DisplayClass75_0();
				CS$<>8__locals1.index = i;
				yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LocalStringManager.Get(string.Format("LK_CombatSkillType_{0}", CS$<>8__locals1.index)), (LegendaryBookCharacterRelatedData data) => data.CombatSkillQualifications[CS$<>8__locals1.index].ToString(), (short)(82 + CS$<>8__locals1.index), 40f, 60f);
				CS$<>8__locals1 = null;
				num = i;
			}
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Growth.Tr(), (LegendaryBookCharacterRelatedData data) => ViewLegendaryBookCharacters.GetSkillGrowthString(data.ActualAge, data.CombatSkillGrowthType), 119, 30f, 90f);
			yield break;
		}

		// Token: 0x0600542B RID: 21547 RVA: 0x0026FF02 File Offset: 0x0026E102
		private static IEnumerable<ColumnDefinition> GeneratePersonalityColumns()
		{
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Personality_Calm_Name.Tr(), (LegendaryBookCharacterRelatedData data) => data.Personalities[0].ToString(), 96, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Personality_Clever_Name.Tr(), (LegendaryBookCharacterRelatedData data) => data.Personalities[1].ToString(), 97, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Personality_Enthusiastic_Name.Tr(), (LegendaryBookCharacterRelatedData data) => data.Personalities[2].ToString(), 98, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Personality_Brave_Name.Tr(), (LegendaryBookCharacterRelatedData data) => data.Personalities[3].ToString(), 99, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Personality_Firm_Name.Tr(), (LegendaryBookCharacterRelatedData data) => data.Personalities[4].ToString(), 100, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Personality_Lucky_Name.Tr(), (LegendaryBookCharacterRelatedData data) => data.Personalities[5].ToString(), 101, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Personality_Perceptive_Name.Tr(), (LegendaryBookCharacterRelatedData data) => data.Personalities[6].ToString(), 102, 30f, 90f);
			yield break;
		}

		// Token: 0x0600542C RID: 21548 RVA: 0x0026FF0B File Offset: 0x0026E10B
		private static IEnumerable<ColumnDefinition> GenerateItemColumns()
		{
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Food.Tr(), (LegendaryBookCharacterRelatedData data) => data.Resources[0].ToString(), 103, 40f, 60f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Wood.Tr(), (LegendaryBookCharacterRelatedData data) => data.Resources[1].ToString(), 104, 40f, 60f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Metal.Tr(), (LegendaryBookCharacterRelatedData data) => data.Resources[2].ToString(), 105, 40f, 60f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Jade.Tr(), (LegendaryBookCharacterRelatedData data) => data.Resources[3].ToString(), 106, 40f, 60f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Fabric.Tr(), (LegendaryBookCharacterRelatedData data) => data.Resources[4].ToString(), 107, 40f, 60f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Herb.Tr(), (LegendaryBookCharacterRelatedData data) => data.Resources[5].ToString(), 108, 40f, 60f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Money.Tr(), (LegendaryBookCharacterRelatedData data) => data.Resources[6].ToString(), 109, 40f, 60f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Authority.Tr(), (LegendaryBookCharacterRelatedData data) => data.Resources[7].ToString(), 110, 40f, 60f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Inventory.Tr(), (LegendaryBookCharacterRelatedData data) => ViewLegendaryBookCharacters.GetInventoryLoadString(data.CurrInventoryLoad, data.MaxInventoryLoad), 37, 30f, 90f);
			yield return ViewLegendaryBookCharacters.CreateTextColumn(() => LanguageKey.LK_Kidnap.Tr(), (LegendaryBookCharacterRelatedData data) => data.KidnapCount.ToString(), 111, 30f, 90f);
			yield break;
		}

		// Token: 0x0600542D RID: 21549 RVA: 0x0026FF14 File Offset: 0x0026E114
		private static IEnumerable<ColumnDefinition> GenerateCommandColumns()
		{
			yield return ViewLegendaryBookCharacters.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Attack.Tr(), (LegendaryBookCharacterRelatedData data) => ViewLegendaryBookCharacters.CreateMedalCellData(data.AttackMedal, ViewLegendaryBookCharacters.EMedalType.Attack), 112, 80f, 120f);
			yield return ViewLegendaryBookCharacters.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Defence.Tr(), (LegendaryBookCharacterRelatedData data) => ViewLegendaryBookCharacters.CreateMedalCellData(data.DefenceMedal, ViewLegendaryBookCharacters.EMedalType.Defence), 113, 80f, 120f);
			yield return ViewLegendaryBookCharacters.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Wisdom.Tr(), (LegendaryBookCharacterRelatedData data) => ViewLegendaryBookCharacters.CreateMedalCellData(data.WisdomMedal, ViewLegendaryBookCharacters.EMedalType.Wisdom), 114, 80f, 120f);
			yield return ViewLegendaryBookCharacters.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_0.Tr(), (LegendaryBookCharacterRelatedData data) => ViewLegendaryBookCharacters.CreateCommandCellData(data, 0), 115, 80f, 120f);
			yield return ViewLegendaryBookCharacters.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_1.Tr(), (LegendaryBookCharacterRelatedData data) => ViewLegendaryBookCharacters.CreateCommandCellData(data, 1), 116, 80f, 120f);
			yield return ViewLegendaryBookCharacters.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_2.Tr(), (LegendaryBookCharacterRelatedData data) => ViewLegendaryBookCharacters.CreateCommandCellData(data, 2), 117, 80f, 120f);
			yield break;
		}

		// Token: 0x0600542E RID: 21550 RVA: 0x0026FF20 File Offset: 0x0026E120
		private static IconAndTextCellData CreateMedalCellData(int medalCount, ViewLegendaryBookCharacters.EMedalType medalType)
		{
			bool flag = medalCount == 0;
			IconAndTextCellData result;
			if (flag)
			{
				result = IconAndTextCellData.TextOnly("-");
			}
			else
			{
				string iconName = ViewLegendaryBookCharacters.GetMedalIconName(medalCount, medalType);
				string text = string.Format(" x{0}", Mathf.Abs(medalCount));
				result = new IconAndTextCellData(iconName, text, true, false, false, false);
			}
			return result;
		}

		// Token: 0x0600542F RID: 21551 RVA: 0x0026FF74 File Offset: 0x0026E174
		private static string GetMedalIconName(int medalCount, ViewLegendaryBookCharacters.EMedalType medalType)
		{
			int signKey = (medalCount > 0) ? 1 : ((medalCount < 0) ? -1 : 0);
			if (!true)
			{
			}
			string text;
			switch (medalType)
			{
			case ViewLegendaryBookCharacters.EMedalType.Attack:
				text = MedalSummary.AttackMedalIconConfig[signKey];
				break;
			case ViewLegendaryBookCharacters.EMedalType.Defence:
				text = MedalSummary.DefenceMedalIconConfig[signKey];
				break;
			case ViewLegendaryBookCharacters.EMedalType.Wisdom:
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

		// Token: 0x06005430 RID: 21552 RVA: 0x0026FFF4 File Offset: 0x0026E1F4
		private static IconAndTextCellData CreateCommandCellData(LegendaryBookCharacterRelatedData data, int commandIndex)
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

		// Token: 0x06005431 RID: 21553 RVA: 0x00270080 File Offset: 0x0026E280
		private bool IsTaiwu(int charId)
		{
			return charId == this._taiwuCharId;
		}

		// Token: 0x06005432 RID: 21554 RVA: 0x0027009C File Offset: 0x0026E29C
		private bool IsSpecialTeammate(int charId)
		{
			LegendaryBookCharacterRelatedData data = this._currentDataList.Find((LegendaryBookCharacterRelatedData d) => d.Id == charId);
			return data != null && data.IsSpecialGroupMember;
		}

		// Token: 0x06005433 RID: 21555 RVA: 0x002700E0 File Offset: 0x0026E2E0
		private static string GetCharmDisplayString(LegendaryBookCharacterRelatedData data)
		{
			return CommonUtils.GetCharmLevelText(data.Charm, data.Gender, data.PhysiologicalAge, data.ClothDisplayId, CreatingType.IsFixedPresetType(data.CreatingType), data.FaceVisible);
		}

		// Token: 0x06005434 RID: 21556 RVA: 0x00270120 File Offset: 0x0026E320
		public static bool IsCharmComparable(LegendaryBookCharacterRelatedData data)
		{
			bool flag = !data.FaceVisible;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool isFixedCharacter = CreatingType.IsFixedPresetType(data.CreatingType);
				bool flag2 = !isFixedCharacter && data.PhysiologicalAge < 16;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = !isFixedCharacter && data.ClothDisplayId == 0;
					result = !flag3;
				}
			}
			return result;
		}

		// Token: 0x06005435 RID: 21557 RVA: 0x00270184 File Offset: 0x0026E384
		private static string GetFavorDisplayString(LegendaryBookCharacterRelatedData data)
		{
			return CommonUtils.GetFavorStringByInteracted(data.Favorability, data.IsInteractedWithTaiwu);
		}

		// Token: 0x06005436 RID: 21558 RVA: 0x002701A8 File Offset: 0x0026E3A8
		private static string GetSkillGrowthString(short actualAge, sbyte growthType)
		{
			sbyte addValue = ViewLegendaryBookCharacters.GetSkillGrowthAddValue(actualAge, (int)growthType);
			if (!true)
			{
			}
			string text;
			if (growthType != 0)
			{
				if (growthType != 1)
				{
					text = LanguageKey.LK_Qualification_Growth_LateBlooming.Tr();
				}
				else
				{
					text = LanguageKey.LK_Qualification_Growth_Precocious.Tr();
				}
			}
			else
			{
				text = LanguageKey.LK_Qualification_Growth_Average.Tr();
			}
			if (!true)
			{
			}
			string growthName = text;
			if (!true)
			{
			}
			if (addValue <= 0)
			{
				if (addValue >= 0)
				{
					text = "+0".SetColor("lightgrey");
				}
				else
				{
					text = string.Format("{0}", addValue).SetColor("red");
				}
			}
			else
			{
				text = string.Format("+{0}", addValue).SetColor("lightblue");
			}
			if (!true)
			{
			}
			string addValueStr = text;
			return growthName + addValueStr;
		}

		// Token: 0x06005437 RID: 21559 RVA: 0x00270268 File Offset: 0x0026E468
		private static sbyte GetSkillGrowthAddValue(short actualAge, int growthType)
		{
			AgeEffectItem ageData = AgeEffect.Instance[Math.Min((int)actualAge, AgeEffect.Instance.Count - 1)];
			return (growthType == 0) ? ageData.SkillQualificationAverage : ((growthType == 1) ? ageData.SkillQualificationPrecocious : ageData.SkillQualificationLateBlooming);
		}

		// Token: 0x06005438 RID: 21560 RVA: 0x002702B4 File Offset: 0x0026E4B4
		private static string GetInventoryLoadString(int currLoad, int maxLoad)
		{
			string currLoadStr = ((float)currLoad / 100f).ToString("f1").SetColor(CommonUtils.GetLoadWeightValueColor(currLoad, maxLoad));
			return string.Format("{0}/{1:f1}", currLoadStr, (float)maxLoad / 100f);
		}

		// Token: 0x06005439 RID: 21561 RVA: 0x00270300 File Offset: 0x0026E500
		private void OnSubPageChanged(LegendaryBookCharacterSubPage subPage)
		{
			bool flag = this._currentSubPage == subPage;
			if (!flag)
			{
				this._currentSubPage = subPage;
				LegendaryBookCharSortAndFilterController sortAndFilterController = this._sortAndFilterController;
				if (sortAndFilterController != null)
				{
					sortAndFilterController.SetSubPage(subPage);
				}
				TabSortStateManager<LegendaryBookCharacterSubPage, LegendaryBookCharacterRelatedData> tabSortStateManager = this._tabSortStateManager;
				if (tabSortStateManager != null)
				{
					tabSortStateManager.OnTabChange(subPage);
				}
				this.RefreshList();
			}
		}

		// Token: 0x0600543A RID: 21562 RVA: 0x00270351 File Offset: 0x0026E551
		private void OnSortOrFilterChanged()
		{
			this.RefreshListData();
		}

		// Token: 0x17000A5E RID: 2654
		// (get) Token: 0x0600543B RID: 21563 RVA: 0x0027035B File Offset: 0x0026E55B
		public static string SearchingText
		{
			get
			{
				ViewLegendaryBookCharacters viewLegendaryBookCharacters = UIElement.LegendaryBookCharacters.UiBaseAs<ViewLegendaryBookCharacters>();
				return (viewLegendaryBookCharacters != null) ? viewLegendaryBookCharacters.searchingField.text : null;
			}
		}

		// Token: 0x040038EC RID: 14572
		public const string ArgKey_IsCompetitorMode = "IsCompetitorMode";

		// Token: 0x040038ED RID: 14573
		public const string ArgKey_CharDataList = "CharDataList";

		// Token: 0x040038EE RID: 14574
		public const string ArgKey_OnClose = "OnClose";

		// Token: 0x040038EF RID: 14575
		private static readonly List<LanguageKey> ToggleGroupNameKeys = new List<LanguageKey>
		{
			LanguageKey.LK_MouseTipCharacter_InteractionType_8,
			LanguageKey.LK_Team_Tog_State,
			LanguageKey.LK_Team_Tog_Property,
			LanguageKey.LK_Team_Tog_Property_Hit,
			LanguageKey.LK_Team_Tog_LifeSkill,
			LanguageKey.LK_Team_Tog_CombatSkill,
			LanguageKey.LK_Team_Tog_Personality,
			LanguageKey.LK_Team_Tog_Item,
			LanguageKey.LK_Team_Tog_Command
		};

		// Token: 0x040038F0 RID: 14576
		private LegendaryBookCharacterSubPage _currentSubPage = LegendaryBookCharacterSubPage.LegendaryBook;

		// Token: 0x040038F1 RID: 14577
		private List<LegendaryBookCharacterRelatedData> _currentDataList = new List<LegendaryBookCharacterRelatedData>();

		// Token: 0x040038F2 RID: 14578
		private List<LegendaryBookCharacterRelatedData> _dataList = new List<LegendaryBookCharacterRelatedData>();

		// Token: 0x040038F3 RID: 14579
		private List<LegendaryBookCharacterRelatedData> _filteredDataList = new List<LegendaryBookCharacterRelatedData>();

		// Token: 0x040038F4 RID: 14580
		private LegendaryBookCharSortAndFilterController _sortAndFilterController;

		// Token: 0x040038F5 RID: 14581
		private TabSortStateManager<LegendaryBookCharacterSubPage, LegendaryBookCharacterRelatedData> _tabSortStateManager;

		// Token: 0x040038F6 RID: 14582
		private readonly Dictionary<LegendaryBookCharacterSubPage, RowItem> _rowTemplateCache = new Dictionary<LegendaryBookCharacterSubPage, RowItem>();

		// Token: 0x040038F7 RID: 14583
		private bool _isCompetitorMode;

		// Token: 0x040038F8 RID: 14584
		private Action _onCharactersPageClose;

		// Token: 0x040038F9 RID: 14585
		private float _subPageToggleMaxFontSize = 24f;

		// Token: 0x040038FA RID: 14586
		private float _legendaryBookToggleMaxFontSize = 24f;

		// Token: 0x040038FB RID: 14587
		[SerializeField]
		private ListStyleGeneralScroll listScroll;

		// Token: 0x040038FC RID: 14588
		[SerializeField]
		private SortAndFilterLegacy sortAndFilter;

		// Token: 0x040038FD RID: 14589
		[SerializeField]
		private CToggleGroup subPageToggleGroup;

		// Token: 0x040038FE RID: 14590
		[SerializeField]
		private TMP_InputField searchingField;

		// Token: 0x040038FF RID: 14591
		[SerializeField]
		private TextMeshProUGUI title;

		// Token: 0x04003900 RID: 14592
		[SerializeField]
		private CButton btnClose;

		// Token: 0x04003901 RID: 14593
		[Header("行模板配置")]
		[SerializeField]
		private RowItem rowTemplate;

		// Token: 0x04003902 RID: 14594
		[SerializeField]
		private RowCellContainer singleTextCellContainer;

		// Token: 0x04003903 RID: 14595
		[SerializeField]
		private RowCellContainer buttonCellContainer;

		// Token: 0x04003904 RID: 14596
		[SerializeField]
		private RowCellContainer avatarAndNameCellContainer;

		// Token: 0x04003905 RID: 14597
		[SerializeField]
		private RowCellContainer iconAndTextCellContainer;

		// Token: 0x04003906 RID: 14598
		[SerializeField]
		private RowCellContainer legendaryBookCellContainer;

		// Token: 0x04003907 RID: 14599
		[SerializeField]
		private RowCellContainer featureCellContainer;

		// Token: 0x04003908 RID: 14600
		[Header("奇书Toggles(竞争者)")]
		[SerializeField]
		private CToggleGroup togGroupLegendaryBook;

		// Token: 0x04003909 RID: 14601
		[Header("奇书Toggles(堕魔者)")]
		[SerializeField]
		private CToggleGroup togGroupFallenType;

		// Token: 0x0400390A RID: 14602
		[Header("Color")]
		[SerializeField]
		private Color colorActiveToggleText;

		// Token: 0x0400390B RID: 14603
		[SerializeField]
		private Color colorInactiveToggleText;

		// Token: 0x02001B23 RID: 6947
		private enum ELegendaryBookColumn
		{
			// Token: 0x0400B859 RID: 47193
			LegendaryBook,
			// Token: 0x0400B85A RID: 47194
			Sect,
			// Token: 0x0400B85B RID: 47195
			Role,
			// Token: 0x0400B85C RID: 47196
			ConsummateLevel,
			// Token: 0x0400B85D RID: 47197
			Location,
			// Token: 0x0400B85E RID: 47198
			Feature,
			// Token: 0x0400B85F RID: 47199
			Count
		}

		// Token: 0x02001B24 RID: 6948
		private enum EStateColumn
		{
			// Token: 0x0400B861 RID: 47201
			Age,
			// Token: 0x0400B862 RID: 47202
			Health,
			// Token: 0x0400B863 RID: 47203
			DefeatMark,
			// Token: 0x0400B864 RID: 47204
			Charm,
			// Token: 0x0400B865 RID: 47205
			Behavior,
			// Token: 0x0400B866 RID: 47206
			Happiness,
			// Token: 0x0400B867 RID: 47207
			Favorability,
			// Token: 0x0400B868 RID: 47208
			Alertness,
			// Token: 0x0400B869 RID: 47209
			Samsara,
			// Token: 0x0400B86A RID: 47210
			Fame,
			// Token: 0x0400B86B RID: 47211
			Count
		}

		// Token: 0x02001B25 RID: 6949
		private enum EPropertyColumn
		{
			// Token: 0x0400B86D RID: 47213
			AttrStrength,
			// Token: 0x0400B86E RID: 47214
			AttrDexterity,
			// Token: 0x0400B86F RID: 47215
			AttrConcentration,
			// Token: 0x0400B870 RID: 47216
			AttrVitality,
			// Token: 0x0400B871 RID: 47217
			AttrEnergy,
			// Token: 0x0400B872 RID: 47218
			AttrIntelligence,
			// Token: 0x0400B873 RID: 47219
			PenetrateOuter,
			// Token: 0x0400B874 RID: 47220
			PenetrateInner,
			// Token: 0x0400B875 RID: 47221
			ResistOuter,
			// Token: 0x0400B876 RID: 47222
			ResistInner,
			// Token: 0x0400B877 RID: 47223
			Count
		}

		// Token: 0x02001B26 RID: 6950
		private enum EProperty2Column
		{
			// Token: 0x0400B879 RID: 47225
			HitStrength,
			// Token: 0x0400B87A RID: 47226
			HitTechnique,
			// Token: 0x0400B87B RID: 47227
			HitSpeed,
			// Token: 0x0400B87C RID: 47228
			HitMind,
			// Token: 0x0400B87D RID: 47229
			AvoidStrength,
			// Token: 0x0400B87E RID: 47230
			AvoidTechnique,
			// Token: 0x0400B87F RID: 47231
			AvoidSpeed,
			// Token: 0x0400B880 RID: 47232
			AvoidMind,
			// Token: 0x0400B881 RID: 47233
			QiDisorder,
			// Token: 0x0400B882 RID: 47234
			Count
		}

		// Token: 0x02001B27 RID: 6951
		private enum ELifeSkillColumn
		{
			// Token: 0x0400B884 RID: 47236
			Skill0,
			// Token: 0x0400B885 RID: 47237
			Skill1,
			// Token: 0x0400B886 RID: 47238
			Skill2,
			// Token: 0x0400B887 RID: 47239
			Skill3,
			// Token: 0x0400B888 RID: 47240
			Skill4,
			// Token: 0x0400B889 RID: 47241
			Skill5,
			// Token: 0x0400B88A RID: 47242
			Skill6,
			// Token: 0x0400B88B RID: 47243
			Skill7,
			// Token: 0x0400B88C RID: 47244
			Skill8,
			// Token: 0x0400B88D RID: 47245
			Skill9,
			// Token: 0x0400B88E RID: 47246
			Skill10,
			// Token: 0x0400B88F RID: 47247
			Skill11,
			// Token: 0x0400B890 RID: 47248
			Skill12,
			// Token: 0x0400B891 RID: 47249
			Skill13,
			// Token: 0x0400B892 RID: 47250
			Skill14,
			// Token: 0x0400B893 RID: 47251
			Skill15,
			// Token: 0x0400B894 RID: 47252
			Growth,
			// Token: 0x0400B895 RID: 47253
			Count
		}

		// Token: 0x02001B28 RID: 6952
		private enum ECombatSkillColumn
		{
			// Token: 0x0400B897 RID: 47255
			Skill0,
			// Token: 0x0400B898 RID: 47256
			Skill1,
			// Token: 0x0400B899 RID: 47257
			Skill2,
			// Token: 0x0400B89A RID: 47258
			Skill3,
			// Token: 0x0400B89B RID: 47259
			Skill4,
			// Token: 0x0400B89C RID: 47260
			Skill5,
			// Token: 0x0400B89D RID: 47261
			Skill6,
			// Token: 0x0400B89E RID: 47262
			Skill7,
			// Token: 0x0400B89F RID: 47263
			Skill8,
			// Token: 0x0400B8A0 RID: 47264
			Skill9,
			// Token: 0x0400B8A1 RID: 47265
			Skill10,
			// Token: 0x0400B8A2 RID: 47266
			Skill11,
			// Token: 0x0400B8A3 RID: 47267
			Skill12,
			// Token: 0x0400B8A4 RID: 47268
			Skill13,
			// Token: 0x0400B8A5 RID: 47269
			Growth,
			// Token: 0x0400B8A6 RID: 47270
			Count
		}

		// Token: 0x02001B29 RID: 6953
		private enum EPersonalityColumn
		{
			// Token: 0x0400B8A8 RID: 47272
			P0,
			// Token: 0x0400B8A9 RID: 47273
			P1,
			// Token: 0x0400B8AA RID: 47274
			P2,
			// Token: 0x0400B8AB RID: 47275
			P3,
			// Token: 0x0400B8AC RID: 47276
			P4,
			// Token: 0x0400B8AD RID: 47277
			P5,
			// Token: 0x0400B8AE RID: 47278
			P6,
			// Token: 0x0400B8AF RID: 47279
			Count
		}

		// Token: 0x02001B2A RID: 6954
		private enum EItemColumn
		{
			// Token: 0x0400B8B1 RID: 47281
			Food,
			// Token: 0x0400B8B2 RID: 47282
			Wood,
			// Token: 0x0400B8B3 RID: 47283
			Metal,
			// Token: 0x0400B8B4 RID: 47284
			Jade,
			// Token: 0x0400B8B5 RID: 47285
			Fabric,
			// Token: 0x0400B8B6 RID: 47286
			Herb,
			// Token: 0x0400B8B7 RID: 47287
			Money,
			// Token: 0x0400B8B8 RID: 47288
			Authority,
			// Token: 0x0400B8B9 RID: 47289
			InventoryLoad,
			// Token: 0x0400B8BA RID: 47290
			KidnapCount,
			// Token: 0x0400B8BB RID: 47291
			Count
		}

		// Token: 0x02001B2B RID: 6955
		private enum ECommandColumn
		{
			// Token: 0x0400B8BD RID: 47293
			AttackMedal,
			// Token: 0x0400B8BE RID: 47294
			DefenceMedal,
			// Token: 0x0400B8BF RID: 47295
			WisdomMedal,
			// Token: 0x0400B8C0 RID: 47296
			Command0,
			// Token: 0x0400B8C1 RID: 47297
			Command1,
			// Token: 0x0400B8C2 RID: 47298
			Command2,
			// Token: 0x0400B8C3 RID: 47299
			Count
		}

		// Token: 0x02001B2C RID: 6956
		private enum EMedalType
		{
			// Token: 0x0400B8C5 RID: 47301
			Attack,
			// Token: 0x0400B8C6 RID: 47302
			Defence,
			// Token: 0x0400B8C7 RID: 47303
			Wisdom
		}
	}
}
