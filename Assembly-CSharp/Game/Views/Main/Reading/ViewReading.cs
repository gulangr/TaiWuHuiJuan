using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.CellContent;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Book;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Main.Reading
{
	// Token: 0x02000973 RID: 2419
	public class ViewReading : UIBase
	{
		// Token: 0x17000D24 RID: 3364
		// (get) Token: 0x060073A2 RID: 29602 RVA: 0x0035BB5C File Offset: 0x00359D5C
		private bool TypeIsCombatSkill
		{
			get
			{
				return this.skillTypeTogGroup.GetActiveIndex() == 0;
			}
		}

		// Token: 0x17000D25 RID: 3365
		// (get) Token: 0x060073A3 RID: 29603 RVA: 0x0035BB6C File Offset: 0x00359D6C
		private bool RefTypeIsCombatSkill
		{
			get
			{
				return this.refSkillTypeTogGroup.GetActiveIndex() == 0;
			}
		}

		// Token: 0x060073A4 RID: 29604 RVA: 0x0035BB7C File Offset: 0x00359D7C
		public override void OnInit(ArgumentBox argsBox)
		{
			GlobalDomainMethod.Call.InvokeGuidingTrigger(78);
			this._curReadingBook = ItemKey.Invalid;
			this.empty.gameObject.SetActive(this._curReadingBook == ItemKey.Invalid);
			this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			this._inited = false;
			this.NeedDataListenerId = true;
			this._bookDisplayData.Clear();
			this._pageDisplayData.Clear();
			this._availableBookList.Clear();
			this._availableReferenceBookList.Clear();
			this._availableReferenceBookBonusSpeedDict.Clear();
			this.selectBookScroll.SetData<ItemKey>(Array.Empty<ItemKey>(), -1);
			this.selectReferenceBookScroll.SetData<ItemKey>(Array.Empty<ItemKey>(), -1);
			this.readingPages.fivePages.gameObject.SetActive(false);
			this.readingPages.sixPages.gameObject.SetActive(false);
			this.intelligenceRoot.gameObject.SetActive(this._curReadingBook.IsValid());
			this._receivedBookTypes = 0;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
			{
				TaiwuDomainMethod.Call.CheckNotInInventoryBooks();
				CharacterDomainMethod.Call.GetAllLifeSkillAttainment(this.Element.GameDataListenerId, this._taiwuCharId);
				CharacterDomainMethod.Call.GetAllCombatSkillAttainment(this.Element.GameDataListenerId, this._taiwuCharId);
				CharacterDomainMethod.Call.GetInventoryItems(this.Element.GameDataListenerId, this._taiwuCharId, 1001);
				CharacterDomainMethod.Call.GetInventoryItems(this.Element.GameDataListenerId, this._taiwuCharId, 1000);
			}));
		}

		// Token: 0x060073A5 RID: 29605 RVA: 0x0035BCB4 File Offset: 0x00359EB4
		private void Awake()
		{
			this.InitSortAndFilter();
			this.InitBookTypeToggleGroups();
			this.InitBookScroll();
			this.InitReferenceBookScroll();
			this.UpdateCurReadingBookPages(true);
			this.bookIntro.itemEmptyBtn.ClearAndAddListener(new Action(this.ChangeChooseBook));
			PointerTrigger bookIntroPointerTrigger = this.bookIntro.itemCard.GetComponent<PointerTrigger>();
			PointerTrigger pointerTrigger = bookIntroPointerTrigger;
			if (pointerTrigger.EnterEvent == null)
			{
				pointerTrigger.EnterEvent = new UnityEvent();
			}
			pointerTrigger = bookIntroPointerTrigger;
			if (pointerTrigger.ExitEvent == null)
			{
				pointerTrigger.ExitEvent = new UnityEvent();
			}
			bookIntroPointerTrigger.EnterEvent.AddListener(delegate()
			{
				this._InbookIntroPointerTrigger = true;
				bool flag = this.chooseBook.GetComponent<CanvasGroup>().alpha > 0f || this._InremoveCurBookBtnPointerTrigger;
				if (flag)
				{
					this.bookIntro.itemCard.ChangeHoverActive(this.chooseBook.GetComponent<CanvasGroup>().alpha > 0f && !this._InremoveCurBookBtnPointerTrigger);
				}
				else
				{
					this.bookIntro.itemCard.OnMouseEnterEvent();
				}
			});
			bookIntroPointerTrigger.ExitEvent.AddListener(delegate()
			{
				this._InbookIntroPointerTrigger = false;
				this.bookIntro.itemCard.OnMouseExitEvent();
			});
			PointerTrigger removeCurBookBtnPointerTrigger = this.bookIntro.removeCurBookBtn.GetComponent<PointerTrigger>();
			pointerTrigger = removeCurBookBtnPointerTrigger;
			if (pointerTrigger.EnterEvent == null)
			{
				pointerTrigger.EnterEvent = new UnityEvent();
			}
			pointerTrigger = removeCurBookBtnPointerTrigger;
			if (pointerTrigger.ExitEvent == null)
			{
				pointerTrigger.ExitEvent = new UnityEvent();
			}
			removeCurBookBtnPointerTrigger.EnterEvent.AddListener(delegate()
			{
				this._InremoveCurBookBtnPointerTrigger = true;
				this.bookIntro.itemCard.OnMouseExitEvent();
			});
			removeCurBookBtnPointerTrigger.ExitEvent.AddListener(delegate()
			{
				this._InremoveCurBookBtnPointerTrigger = false;
				this.bookIntro.itemCard.ChangeHoverActive(this._InbookIntroPointerTrigger);
				bool flag = !this._InbookIntroPointerTrigger || this.chooseBook.GetComponent<CanvasGroup>().alpha > 0f;
				if (!flag)
				{
					this.bookIntro.itemCard.OnMouseEnterEvent();
				}
			});
			for (int i = 0; i < this.referenceBookList.Count; i++)
			{
				ReadingReferenceBookInfoPrefab referenceBook = this.referenceBookList[i];
				int slotIndex = i;
				referenceBook.emptyBtn.ClearAndAddListener(delegate
				{
					this.SetReferenceSlotSelected(slotIndex);
					this.MoveToChooseReferenceForward();
					UIManager.Instance.SetEscHandler(new Action(this.MoveToChooseReferenceBackForward));
				});
				referenceBook.selectedBtn.ClearAndAddListener(delegate
				{
					this.SetReferenceSlotSelected(slotIndex);
					this.MoveToChooseReferenceForward();
					UIManager.Instance.SetEscHandler(new Action(this.MoveToChooseReferenceBackForward));
				});
				pointerTrigger = referenceBook.selectedBtnPointerTrigger;
				if (pointerTrigger.EnterEvent == null)
				{
					pointerTrigger.EnterEvent = new UnityEvent();
				}
				pointerTrigger = referenceBook.selectedBtnPointerTrigger;
				if (pointerTrigger.ExitEvent == null)
				{
					pointerTrigger.ExitEvent = new UnityEvent();
				}
				referenceBook.selectedBtnPointerTrigger.EnterEvent.AddListener(delegate()
				{
					referenceBook.ChangeStateInSelectedBtn(true);
					bool flag = this.chooseReference.GetComponent<CanvasGroup>().alpha > 0f || referenceBook.inRemoveBtnPointerTrigger;
					if (flag)
					{
						referenceBook.hoverGo.SetActive(this.chooseReference.GetComponent<CanvasGroup>().alpha > 0f && !referenceBook.inRemoveBtnPointerTrigger);
					}
					else
					{
						referenceBook.selectRoot.SetActive(true);
					}
				});
				referenceBook.selectedBtnPointerTrigger.ExitEvent.AddListener(delegate()
				{
					referenceBook.hoverGo.SetActive(false);
					referenceBook.ChangeStateInSelectedBtn(false);
					referenceBook.selectRoot.SetActive(false);
				});
				pointerTrigger = referenceBook.removeBtnPointerTrigger;
				if (pointerTrigger.EnterEvent == null)
				{
					pointerTrigger.EnterEvent = new UnityEvent();
				}
				pointerTrigger = referenceBook.removeBtnPointerTrigger;
				if (pointerTrigger.ExitEvent == null)
				{
					pointerTrigger.ExitEvent = new UnityEvent();
				}
				referenceBook.removeBtnPointerTrigger.EnterEvent.AddListener(delegate()
				{
					referenceBook.hoverGo.SetActive(false);
					referenceBook.ChangeStateInRemoveBtn(true);
					referenceBook.selectRoot.SetActive(false);
				});
				referenceBook.removeBtnPointerTrigger.ExitEvent.AddListener(delegate()
				{
					referenceBook.ChangeStateInRemoveBtn(false);
					referenceBook.hoverGo.SetActive(referenceBook.inSelectedBtnPointerTrigger);
					bool flag = !referenceBook.inSelectedBtnPointerTrigger || this.chooseReference.GetComponent<CanvasGroup>().alpha > 0f;
					if (!flag)
					{
						referenceBook.selectRoot.SetActive(true);
					}
				});
			}
			this.chooseBookGroup.alpha = 0f;
			this.chooseReferenceGroup.alpha = 0f;
			this.exitChooseBookBtn.ClearAndAddListener(new Action(this.MoveToChooseBookBackForward));
			this.exitChooseReferenceBtn.ClearAndAddListener(new Action(this.MoveToChooseReferenceBackForward));
		}

		// Token: 0x060073A6 RID: 29606 RVA: 0x0035BFC8 File Offset: 0x0035A1C8
		private void InitSortAndFilter()
		{
			this._selectCombatBookSortAndFilterController = new ReadingCombatSkillBookSortAndFilterController(this.selectCombatBookSortAndFilter, new Comparison<ITradeableContent>(this.CompareBookContent));
			this._selectCombatBookSortAndFilterController.Init(new Action(this.UpdateBookList), "ReadingSelectCombatBook");
			this._selectLifeBookSortAndFilterController = new ReadingLifeSkillBookSortAndFilterController(this.selectLifeBookSortAndFilter, new Comparison<ITradeableContent>(this.CompareBookContent));
			this._selectLifeBookSortAndFilterController.Init(new Action(this.UpdateBookList), "ReadingSelectLifeBook");
			this._selectCombatReferenceBookSortAndFilterController = new ReadingCombatSkillBookSortAndFilterController(this.selectCombatReferenceBookSortAndFilter, new Comparison<ITradeableContent>(this.CompareReferenceBookContent));
			this._selectCombatReferenceBookSortAndFilterController.Init(new Action(this.OnReferenceBookSortAndFilterChanged), "ReadingSelectCombatReferenceBook");
			this._selectLifeReferenceBookSortAndFilterController = new ReadingLifeSkillBookSortAndFilterController(this.selectLifeReferenceBookSortAndFilter, new Comparison<ITradeableContent>(this.CompareReferenceBookContent));
			this._selectLifeReferenceBookSortAndFilterController.Init(new Action(this.OnReferenceBookSortAndFilterChanged), "ReadingSelectLifeReferenceBook");
		}

		// Token: 0x060073A7 RID: 29607 RVA: 0x0035C0BE File Offset: 0x0035A2BE
		private void OnReferenceBookSortAndFilterChanged()
		{
			this._shouldExecuteUpdateCurPage = false;
			this.UpdateReferenceBookList();
		}

		// Token: 0x060073A8 RID: 29608 RVA: 0x0035C0D0 File Offset: 0x0035A2D0
		private void InitBookTypeToggleGroups()
		{
			this.skillTypeTogGroup.Init(-1);
			ToggleGroupHotkeyController.Set(this.Element, this.skillTypeTogGroup, 0, () => this.chooseBook.GetComponent<CanvasGroup>().alpha == 1f);
			this.skillTypeTogGroup.OnActiveIndexChange += delegate(int _, int _)
			{
				this.RefreshBookSortAndFilterVisible();
				this.UpdateBookList();
			};
			this.skillTypeTogGroup.Set(0, true);
			this.refSkillTypeTogGroup.Init(-1);
			ToggleGroupHotkeyController.Set(this.Element, this.skillTypeTogGroup, 0, () => this.chooseReference.GetComponent<CanvasGroup>().alpha == 1f);
			this.refSkillTypeTogGroup.OnActiveIndexChange += delegate(int _, int _)
			{
				this.RefreshReferenceBookSortAndFilterVisible();
				this.SetupReferenceBookScroll();
				this.OnReferenceBookSortAndFilterChanged();
			};
			this.refSkillTypeTogGroup.Set(0, true);
		}

		// Token: 0x060073A9 RID: 29609 RVA: 0x0035C182 File Offset: 0x0035A382
		private void RefreshBookSortAndFilterVisible()
		{
			this.selectCombatBookSortAndFilter.gameObject.SetActive(this.TypeIsCombatSkill);
			this.selectLifeBookSortAndFilter.gameObject.SetActive(!this.TypeIsCombatSkill);
			this.BindBookScrollSortController();
		}

		// Token: 0x060073AA RID: 29610 RVA: 0x0035C1BD File Offset: 0x0035A3BD
		private void RefreshReferenceBookSortAndFilterVisible()
		{
			this.selectCombatReferenceBookSortAndFilter.gameObject.SetActive(this.RefTypeIsCombatSkill);
			this.selectLifeReferenceBookSortAndFilter.gameObject.SetActive(!this.RefTypeIsCombatSkill);
			this.BindReferenceBookScrollSortController();
		}

		// Token: 0x060073AB RID: 29611 RVA: 0x0035C1F8 File Offset: 0x0035A3F8
		private void BindBookScrollSortController()
		{
			this.selectBookScroll.SetSortController(this.GetActiveSortAndFilterController(false, this.TypeIsCombatSkill));
		}

		// Token: 0x060073AC RID: 29612 RVA: 0x0035C214 File Offset: 0x0035A414
		private void BindReferenceBookScrollSortController()
		{
			this.selectReferenceBookScroll.SetSortController(this.GetActiveSortAndFilterController(true, this.RefTypeIsCombatSkill));
		}

		// Token: 0x060073AD RID: 29613 RVA: 0x0035C230 File Offset: 0x0035A430
		private void InitBookScroll()
		{
			this.selectBookRowTemplate.gameObject.SetActive(false);
			this.selectBookItemIconAndNameCellContainer.gameObject.SetActive(false);
			this.selectBookSingleTextCellContainer.gameObject.SetActive(false);
			this.selectBookInfoCellContainer.gameObject.SetActive(false);
			this.PrepareBookRowTemplateContainers();
			this.selectBookScroll.SetRowTemplate(this.selectBookRowTemplate);
			this.selectBookScroll.Init<ItemKey>(this.GenerateBookColumnDefinitions(), true, new Action<int, GameObject>(this.OnRenderReadingBookRow), null);
			this.selectBookScroll.RowSelectedProvider = new Func<int, object, bool>(this.IsReadingBookRowSelected);
			this.selectBookScroll.RowDisabledProvider = new Func<int, object, bool>(this.IsReadingBookRowDisabled);
			this.selectBookScroll.OnRowClicked += this.OnClickReadingBookRow;
			this.BindBookScrollSortController();
		}

		// Token: 0x060073AE RID: 29614 RVA: 0x0035C310 File Offset: 0x0035A510
		private void InitReferenceBookScroll()
		{
			this.SetupReferenceBookScroll();
			this.selectReferenceBookScroll.RowSelectedProvider = new Func<int, object, bool>(this.IsReferenceBookRowSelected);
			this.selectReferenceBookScroll.RowDisabledProvider = new Func<int, object, bool>(this.IsReferenceBookRowDisabled);
			this.selectReferenceBookScroll.OnRowClicked += this.OnClickReferenceBookRow;
		}

		// Token: 0x060073AF RID: 29615 RVA: 0x0035C370 File Offset: 0x0035A570
		private void SetupReferenceBookScroll()
		{
			this.selectReferenceBookRowTemplate.gameObject.SetActive(false);
			this.selectReferenceBookItemIconAndNameCellContainer.gameObject.SetActive(false);
			this.selectReferenceBookSingleTextCellContainer.gameObject.SetActive(false);
			this.selectReferenceBookInfoCellContainer.gameObject.SetActive(false);
			this.selectReferenceBookStrategyCellContainer.gameObject.SetActive(false);
			this.PrepareReferenceBookRowTemplateContainers();
			this.selectReferenceBookScroll.SetRowTemplate(this.selectReferenceBookRowTemplate);
			this.selectReferenceBookScroll.Init<ItemKey>(this.GenerateReferenceBookColumnDefinitions(), true, new Action<int, GameObject>(this.OnRenderReferenceBookRow), null);
			this.BindReferenceBookScrollSortController();
		}

		// Token: 0x060073B0 RID: 29616 RVA: 0x0035C418 File Offset: 0x0035A618
		private void PrepareBookRowTemplateContainers()
		{
			RowCellContainer[] templates = new RowCellContainer[]
			{
				this.selectBookItemIconAndNameCellContainer,
				this.selectBookSingleTextCellContainer,
				this.selectBookInfoCellContainer
			};
			ViewReading.PrepareRowTemplateContainers(this.selectBookRowTemplate, templates);
		}

		// Token: 0x060073B1 RID: 29617 RVA: 0x0035C458 File Offset: 0x0035A658
		private void PrepareReferenceBookRowTemplateContainers()
		{
			bool showInspiration = !this.RefTypeIsCombatSkill;
			List<RowCellContainer> templates = new List<RowCellContainer>
			{
				this.selectReferenceBookItemIconAndNameCellContainer,
				this.selectReferenceBookSingleTextCellContainer,
				this.selectReferenceBookInfoCellContainer,
				this.selectReferenceBookSingleTextCellContainer,
				this.selectReferenceBookStrategyCellContainer
			};
			bool flag = showInspiration;
			if (flag)
			{
				templates.Insert(4, this.selectReferenceBookSingleTextCellContainer);
			}
			ViewReading.PrepareRowTemplateContainers(this.selectReferenceBookRowTemplate, templates);
		}

		// Token: 0x060073B2 RID: 29618 RVA: 0x0035C4DC File Offset: 0x0035A6DC
		private static void PrepareRowTemplateContainers(RowItem rowTemplate, IReadOnlyList<RowCellContainer> templates)
		{
			Transform containerRoot = rowTemplate.ContainerRoot;
			for (int i = containerRoot.childCount - 1; i >= 0; i--)
			{
				Transform child = containerRoot.GetChild(i);
				bool flag = child.GetComponent<RowCellContainer>() != null;
				if (flag)
				{
					Object.Destroy(child.gameObject);
				}
			}
			foreach (RowCellContainer template in templates)
			{
				RowCellContainer container = Object.Instantiate<RowCellContainer>(template, containerRoot);
				container.gameObject.SetActive(true);
			}
		}

		// Token: 0x060073B3 RID: 29619 RVA: 0x0035C58C File Offset: 0x0035A78C
		private IEnumerable<ColumnDefinition> GenerateBookColumnDefinitions()
		{
			ColumnDefinition<ItemKey, ReadingBookIconAndNameCellData> columnDefinition = new ColumnDefinition<ItemKey, ReadingBookIconAndNameCellData>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 300f,
				FlexibleWidth = 1f,
				PreferredWidth = 340f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Reading_TableHead_Item.Tr());
			columnDefinition.CellDataGenerator = new Func<ItemKey, ReadingBookIconAndNameCellData>(this.GetReadingBookIconAndNameCellData);
			columnDefinition.SortId = -1;
			yield return columnDefinition;
			ColumnDefinition<ItemKey, string> columnDefinition2 = new ColumnDefinition<ItemKey, string>();
			columnDefinition2.LayoutOption = new LayoutOption
			{
				MinWidth = 80f,
				FlexibleWidth = 1f,
				PreferredWidth = 110f,
				Priority = 1
			};
			columnDefinition2.TableHeadLabel = (() => LanguageKey.LK_Reading_TableHead_Durability.Tr());
			columnDefinition2.CellDataGenerator = delegate(ItemKey itemKey)
			{
				ITradeableContent itemDisplayData = this.GetReadingBookItemDisplayData(itemKey);
				return CommonUtils.GetDurabilityString(itemDisplayData.Durability, itemDisplayData.MaxDurability);
			};
			columnDefinition2.SortId = 18;
			yield return columnDefinition2;
			ColumnDefinition<ItemKey, ReadingBookPageInfoCellData> columnDefinition3 = new ColumnDefinition<ItemKey, ReadingBookPageInfoCellData>();
			columnDefinition3.LayoutOption = new LayoutOption
			{
				MinWidth = 355f,
				FlexibleWidth = 1f,
				PreferredWidth = 355f,
				Priority = 1
			};
			columnDefinition3.TableHeadLabel = (() => LanguageKey.LK_Reading_TableHead_BookInfo.Tr());
			columnDefinition3.CellDataGenerator = ((ItemKey itemKey) => this.GetReadingBookPageInfoCellData(itemKey, false));
			columnDefinition3.SortId = -1;
			yield return columnDefinition3;
			yield break;
		}

		// Token: 0x060073B4 RID: 29620 RVA: 0x0035C59C File Offset: 0x0035A79C
		private IEnumerable<ColumnDefinition> GenerateReferenceBookColumnDefinitions()
		{
			bool showInspiration = !this.RefTypeIsCombatSkill;
			ColumnDefinition<ItemKey, ReadingBookIconAndNameCellData> columnDefinition = new ColumnDefinition<ItemKey, ReadingBookIconAndNameCellData>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 300f,
				FlexibleWidth = 1f,
				PreferredWidth = 340f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Reading_TableHead_Item.Tr());
			columnDefinition.CellDataGenerator = new Func<ItemKey, ReadingBookIconAndNameCellData>(this.GetReadingBookIconAndNameCellData);
			columnDefinition.SortId = -1;
			yield return columnDefinition;
			ColumnDefinition<ItemKey, string> columnDefinition2 = new ColumnDefinition<ItemKey, string>();
			columnDefinition2.LayoutOption = new LayoutOption
			{
				MinWidth = 80f,
				FlexibleWidth = 1f,
				PreferredWidth = 110f,
				Priority = 1
			};
			columnDefinition2.TableHeadLabel = (() => LanguageKey.LK_Reading_TableHead_Durability.Tr());
			columnDefinition2.CellDataGenerator = delegate(ItemKey itemKey)
			{
				ITradeableContent itemDisplayData = this.GetReadingBookItemDisplayData(itemKey);
				return CommonUtils.GetDurabilityString(itemDisplayData.Durability, itemDisplayData.MaxDurability);
			};
			columnDefinition2.SortId = 18;
			yield return columnDefinition2;
			ColumnDefinition<ItemKey, ReadingBookPageInfoCellData> columnDefinition3 = new ColumnDefinition<ItemKey, ReadingBookPageInfoCellData>();
			columnDefinition3.LayoutOption = new LayoutOption
			{
				MinWidth = 355f,
				FlexibleWidth = 1f,
				PreferredWidth = 355f,
				Priority = 1
			};
			columnDefinition3.TableHeadLabel = (() => LanguageKey.LK_Reading_TableHead_BookInfo.Tr());
			columnDefinition3.CellDataGenerator = ((ItemKey itemKey) => this.GetReadingBookPageInfoCellData(itemKey, true));
			columnDefinition3.SortId = -1;
			yield return columnDefinition3;
			ColumnDefinition<ItemKey, string> columnDefinition4 = new ColumnDefinition<ItemKey, string>();
			columnDefinition4.LayoutOption = new LayoutOption
			{
				MinWidth = 80f,
				FlexibleWidth = 1f,
				PreferredWidth = 100f,
				Priority = 1
			};
			columnDefinition4.TableHeadLabel = (() => LanguageKey.LK_Reading_TableHead_Efficiency.Tr());
			columnDefinition4.CellDataGenerator = new Func<ItemKey, string>(this.GetReferenceEfficiency);
			columnDefinition4.SortId = 214;
			yield return columnDefinition4;
			bool flag = showInspiration;
			if (flag)
			{
				ColumnDefinition<ItemKey, string> columnDefinition5 = new ColumnDefinition<ItemKey, string>();
				columnDefinition5.LayoutOption = new LayoutOption
				{
					MinWidth = 80f,
					FlexibleWidth = 1f,
					PreferredWidth = 100f,
					Priority = 1
				};
				columnDefinition5.TableHeadLabel = (() => LanguageKey.LK_Reading_TableHead_Inspiration.Tr());
				columnDefinition5.CellDataGenerator = new Func<ItemKey, string>(this.GetReferenceInspiration);
				columnDefinition5.SortId = 215;
				yield return columnDefinition5;
			}
			ColumnDefinition<ItemKey, ReadingStrategiesCellData> columnDefinition6 = new ColumnDefinition<ItemKey, ReadingStrategiesCellData>();
			columnDefinition6.LayoutOption = new LayoutOption
			{
				MinWidth = 100f,
				FlexibleWidth = 1f,
				PreferredWidth = 110f,
				Priority = 1
			};
			columnDefinition6.TableHeadLabel = (() => LanguageKey.LK_Reading_TableHead_Strategy.Tr());
			columnDefinition6.CellDataGenerator = new Func<ItemKey, ReadingStrategiesCellData>(this.GetReferenceStrategies);
			columnDefinition6.SortId = -1;
			yield return columnDefinition6;
			yield break;
		}

		// Token: 0x060073B5 RID: 29621 RVA: 0x0035C5AC File Offset: 0x0035A7AC
		private ITradeableContent GetReadingBookItemDisplayData(ItemKey itemKey)
		{
			return this._bookDisplayData[itemKey.Id];
		}

		// Token: 0x060073B6 RID: 29622 RVA: 0x0035C5D0 File Offset: 0x0035A7D0
		private ReadingBookIconAndNameCellData GetReadingBookIconAndNameCellData(ItemKey itemKey)
		{
			ITradeableContent itemData = this.GetReadingBookItemDisplayData(itemKey);
			bool isFinished = this._allBookReadingProgressList.GetOrDefault(itemKey.Id) >= 100;
			return new ReadingBookIconAndNameCellData(itemData, isFinished);
		}

		// Token: 0x060073B7 RID: 29623 RVA: 0x0035C60C File Offset: 0x0035A80C
		private ReadingBookPageInfoCellData GetReadingBookPageInfoCellData(ItemKey itemKey, bool enableSupplyState)
		{
			SkillBookPageDisplayData pageDisplayData = this._pageDisplayData[itemKey.Id];
			int stateLength = pageDisplayData.State.Length;
			bool[] states = enableSupplyState ? this.GetReferenceBookSupplyStates(itemKey) : this.GetEmptyPageStates(stateLength);
			return new ReadingBookPageInfoCellData(pageDisplayData.State, pageDisplayData.ReadingProgress, pageDisplayData.Type, pageDisplayData.IsCombatBook, states);
		}

		// Token: 0x060073B8 RID: 29624 RVA: 0x0035C66C File Offset: 0x0035A86C
		private bool[] GetReferenceBookSupplyStates(ItemKey itemKey)
		{
			SkillBookPageDisplayData pageDisplayData = this._pageDisplayData[itemKey.Id];
			bool[] states = new bool[pageDisplayData.State.Length];
			SkillBookPageDisplayData readingPageDisplayData;
			bool flag = !this._curReadingBook.IsValid() || !this._curReadingBook.TemplateEquals(itemKey) || !this._pageDisplayData.TryGetValue(this._curReadingBook.Id, out readingPageDisplayData);
			bool[] result;
			if (flag)
			{
				result = states;
			}
			else
			{
				for (int i = 0; i < pageDisplayData.State.Length; i++)
				{
					bool hasSupply = pageDisplayData.State[i] < readingPageDisplayData.State[i];
					bool flag2 = pageDisplayData.IsCombatBook && pageDisplayData.Type[i] != readingPageDisplayData.Type[i];
					if (flag2)
					{
						hasSupply = false;
					}
					states[i] = hasSupply;
				}
				result = states;
			}
			return result;
		}

		// Token: 0x060073B9 RID: 29625 RVA: 0x0035C74C File Offset: 0x0035A94C
		private bool[] GetEmptyPageStates(int length)
		{
			bool[] states;
			bool flag = this._emptyBookPageStates.TryGetValue(length, out states);
			bool[] result;
			if (flag)
			{
				result = states;
			}
			else
			{
				states = new bool[length];
				this._emptyBookPageStates[length] = states;
				result = states;
			}
			return result;
		}

		// Token: 0x060073BA RID: 29626 RVA: 0x0035C78C File Offset: 0x0035A98C
		private string GetReferenceEfficiency(ItemKey itemKey)
		{
			int refBonusSpeed = this._availableReferenceBookBonusSpeedDict.GetOrDefault(itemKey);
			string efficiencyText = string.Format("{0}%", refBonusSpeed);
			return this.HasReferenceBonus(itemKey) ? efficiencyText.SetColor("goldyellow") : efficiencyText;
		}

		// Token: 0x060073BB RID: 29627 RVA: 0x0035C7D4 File Offset: 0x0035A9D4
		private string GetReferenceInspiration(ItemKey itemKey)
		{
			SkillBookItem configData = SkillBook.Instance[itemKey.TemplateId];
			int inspirationRate = (configData.ItemSubType == 1000) ? LifeSkill.Instance[configData.LifeSkillTemplateId].ReadingEventBonusRate : ((int)CombatSkill.Instance[configData.CombatSkillTemplateId].QiArtStrategyGenerateProbability);
			return string.Format("{0}%", inspirationRate);
		}

		// Token: 0x060073BC RID: 29628 RVA: 0x0035C844 File Offset: 0x0035AA44
		private ReadingStrategiesCellData GetReferenceStrategies(ItemKey itemKey)
		{
			SkillBookItem configData = SkillBook.Instance[itemKey.TemplateId];
			bool flag = configData.ItemSubType == 1001;
			ReadingStrategiesCellData result;
			if (flag)
			{
				result = ReadingStrategiesCellData.Empty;
			}
			else
			{
				this._strategyNameCache.Clear();
				this._strategyTipCache.Clear();
				List<byte> strategyList = LifeSkill.Instance[configData.LifeSkillTemplateId].ProvidedReadingStrategies;
				bool flag2 = strategyList == null || strategyList.Count == 0;
				if (flag2)
				{
					result = ReadingStrategiesCellData.Empty;
				}
				else
				{
					for (int i = 0; i < strategyList.Count; i++)
					{
						ReadingStrategyItem strategyConfig = ReadingStrategy.Instance[(int)strategyList[i]];
						string strategyName = strategyConfig.Name;
						this._strategyNameCache.Add(string.IsNullOrEmpty(strategyName) ? string.Empty : strategyName.First<char>().ToString());
						this._strategyTipCache.Add(new ReadingStrategyTipData(strategyConfig.Name, strategyConfig.Desc));
					}
					result = new ReadingStrategiesCellData(this._strategyNameCache.ToArray(), this._strategyTipCache.ToArray());
				}
			}
			return result;
		}

		// Token: 0x060073BD RID: 29629 RVA: 0x0035C974 File Offset: 0x0035AB74
		private bool IsReadingBookRowSelected(int index, object rowData)
		{
			bool result;
			if (rowData is ItemKey)
			{
				ItemKey itemKey = (ItemKey)rowData;
				result = this._curReadingBook.Equals(itemKey);
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060073BE RID: 29630 RVA: 0x0035C9A4 File Offset: 0x0035ABA4
		private bool IsReadingBookRowDisabled(int index, object rowData)
		{
			bool result;
			if (rowData is ItemKey)
			{
				ItemKey itemKey = (ItemKey)rowData;
				result = this.IsAlreadyPlaced(itemKey);
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060073BF RID: 29631 RVA: 0x0035C9D0 File Offset: 0x0035ABD0
		private bool IsReferenceBookRowSelected(int index, object rowData)
		{
			bool result;
			if (rowData is ItemKey)
			{
				ItemKey itemKey = (ItemKey)rowData;
				result = this._referenceBooks.Contains(itemKey);
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060073C0 RID: 29632 RVA: 0x0035CA00 File Offset: 0x0035AC00
		private bool IsReferenceBookRowDisabled(int index, object rowData)
		{
			ItemKey itemKey;
			bool flag;
			if (rowData is ItemKey)
			{
				itemKey = (ItemKey)rowData;
				flag = (1 == 0);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			bool result;
			if (flag2)
			{
				result = true;
			}
			else
			{
				bool flag3 = this._referenceBooks.Contains(itemKey);
				result = (!flag3 && (this.IsAlreadyPlaced(itemKey) || !this._hasAvailableSlot));
			}
			return result;
		}

		// Token: 0x060073C1 RID: 29633 RVA: 0x0035CA5C File Offset: 0x0035AC5C
		private void OnClickReadingBookRow(int index, RowItem rowItem)
		{
			bool flag = index < 0 || index >= this._availableBookList.Count;
			if (!flag)
			{
				bool interactable = this.readingEventBtn.interactable;
				if (interactable)
				{
					this.OnClickCheckDialogCmd(delegate
					{
						this.ClearCurrentReadingEvent();
						this.ChangeReadingBook(index);
					});
				}
				else
				{
					this.ChangeReadingBook(index);
				}
			}
		}

		// Token: 0x060073C2 RID: 29634 RVA: 0x0035CADC File Offset: 0x0035ACDC
		private void OnClickReferenceBookRow(int index, RowItem rowItem)
		{
			bool flag = index < 0 || index >= this._availableReferenceBookList.Count;
			if (!flag)
			{
				ItemKey itemKey = this._availableReferenceBookList[index];
				bool wasSelected = this._referenceBooks.Contains(itemKey);
				int newIndex = wasSelected ? -1 : index;
				int oldIndex = wasSelected ? index : -1;
				bool interactable = this.readingEventBtn.interactable;
				if (interactable)
				{
					this.OnClickCheckDialogCmd(delegate
					{
						this.ClearCurrentReadingEvent();
						this.ChangeReferenceBook(newIndex, oldIndex);
					});
				}
				else
				{
					this.ChangeReferenceBook(newIndex, oldIndex);
				}
			}
		}

		// Token: 0x060073C3 RID: 29635 RVA: 0x0035CB88 File Offset: 0x0035AD88
		private void OnRenderReadingBookRow(int index, GameObject obj)
		{
			RowItem rowItem = obj.GetComponent<RowItem>();
			ItemKey itemKey = this._availableBookList[index];
			ViewReading.SetupRowItemTip(rowItem, this._bookDisplayData[itemKey.Id]);
		}

		// Token: 0x060073C4 RID: 29636 RVA: 0x0035CBC4 File Offset: 0x0035ADC4
		private void OnRenderReferenceBookRow(int index, GameObject obj)
		{
			RowItem rowItem = obj.GetComponent<RowItem>();
			ItemKey itemKey = this._availableReferenceBookList[index];
			ViewReading.SetupRowItemTip(rowItem, this._bookDisplayData[itemKey.Id]);
		}

		// Token: 0x060073C5 RID: 29637 RVA: 0x0035CC00 File Offset: 0x0035AE00
		private static void SetupRowItemTip(RowItem rowItem, ItemDisplayData itemDisplayData)
		{
			TooltipInvoker tipDisplayer = rowItem.TipDisplayer;
			tipDisplayer.Type = TooltipManager.ItemTypeToTipType[itemDisplayData.Key.ItemType];
			tipDisplayer.RuntimeParam = new ArgumentBox().SetObject("ItemData", itemDisplayData);
			tipDisplayer.RuntimeParam.Set("ShowPageInfo", true);
			tipDisplayer.Refresh(false, -1);
		}

		// Token: 0x060073C6 RID: 29638 RVA: 0x0035CC64 File Offset: 0x0035AE64
		private void ChangeChooseBook()
		{
			this.bookIntro.itemCard.OnMouseExitEvent();
			this.bookIntro.itemCard.ChangeHoverActive(this._InbookIntroPointerTrigger);
			this.MoveToChooseBookForward();
			UIManager.Instance.SetEscHandler(new Action(this.MoveToChooseBookBackForward));
		}

		// Token: 0x060073C7 RID: 29639 RVA: 0x0035CCB8 File Offset: 0x0035AEB8
		private void OpenReadingEvent()
		{
			UIManager.Instance.ShowUI(UIElement.ReadingEvent, true);
		}

		// Token: 0x060073C8 RID: 29640 RVA: 0x0035CCCC File Offset: 0x0035AECC
		private void SetReadingEventBtnState(bool interactable)
		{
			this.readingEventBtn.interactable = interactable;
			this.readingEventBtn.GetComponent<DisableStyleRoot>().SetStyleEffect(!this.readingEventBtn.interactable, false);
			this.readingEventParticleHolder.SetActive(interactable);
			this.intelligenceCostHolder.SetActive(interactable);
		}

		// Token: 0x060073C9 RID: 29641 RVA: 0x0035CD24 File Offset: 0x0035AF24
		private void SetReadingEventBtnMouseTip(int readInCombatCount, int readInLifeSkillCombatCount)
		{
			TooltipInvoker readingEventMouseTipDisplayer = this.readingEventBtn.GetComponent<TooltipInvoker>();
			TooltipInvoker tooltipInvoker = readingEventMouseTipDisplayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			readingEventMouseTipDisplayer.RuntimeParam.Clear();
			readingEventMouseTipDisplayer.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_Reading_ReadInCombatTitleTip));
			readingEventMouseTipDisplayer.RuntimeParam.Set("Content", LocalStringManager.Get(LanguageKey.LK_Reading_ReadInCombatContentTip));
			readingEventMouseTipDisplayer.RuntimeParam.Set("SubHeadingText", LocalStringManager.Get(LanguageKey.LK_Reading_Monthly_ReadInCombat));
			string readInCombatCountStr = ViewReading.FormatCountString(readInCombatCount, "lightblue", "brightred", "pinkyellow");
			string readInLifeSkillCombatCountStr = ViewReading.FormatCountString(readInLifeSkillCombatCount, "lightblue", "brightred", "pinkyellow");
			readingEventMouseTipDisplayer.RuntimeParam.Set("CombatCountText", LocalStringManager.GetFormat(LanguageKey.LK_Reading_ReadInCombat_Count, readInCombatCountStr));
			readingEventMouseTipDisplayer.RuntimeParam.Set("LifeSkillCountText", LocalStringManager.GetFormat(LanguageKey.LK_Reading_ReadInLifeSkillCombat_Count, readInLifeSkillCombatCountStr));
		}

		// Token: 0x060073CA RID: 29642 RVA: 0x0035CE18 File Offset: 0x0035B018
		private static string FormatCountString(int count, string trueColor, string falseColor, string suffixColor)
		{
			string countColor = (count > 0) ? trueColor : falseColor;
			return count.ToString().SetColor(countColor) + "/1".SetColor(suffixColor);
		}

		// Token: 0x060073CB RID: 29643 RVA: 0x0035CE50 File Offset: 0x0035B050
		private void OnEnable()
		{
			GEvent.Add(EEvents.OnMonthChange, new GEvent.Callback(this.OnMonthChange));
			GEvent.Add(UiEvents.CloseReadingEvent, new GEvent.Callback(this.OnCloseReadingEvent));
			GEvent.Add(UiEvents.RefreshBookList, new GEvent.Callback(this.CurReadingBookDelete));
			bool canRemoveBook = SingletonObject.getInstance<TutorialChapterModel>().GetFunctionStatus(14);
			this.removeCurBookBtn.interactable = canRemoveBook;
		}

		// Token: 0x060073CC RID: 29644 RVA: 0x0035CEC4 File Offset: 0x0035B0C4
		private void CurReadingBookDelete(ArgumentBox argbox)
		{
			int curReadingBookId;
			argbox.Get("CurReadingBookId", out curReadingBookId);
			bool isFind = false;
			foreach (ItemDisplayData book in this._lifeSkillBooks)
			{
				bool flag = book.Key.Id == curReadingBookId;
				if (flag)
				{
					this._lifeSkillBooks.Remove(book);
					isFind = true;
					break;
				}
			}
			bool flag2 = !isFind;
			if (flag2)
			{
				foreach (ItemDisplayData book2 in this._combatSkillBooks)
				{
					bool flag3 = book2.Key.Id == curReadingBookId;
					if (flag3)
					{
						this._combatSkillBooks.Remove(book2);
						break;
					}
				}
			}
			this.UpdateBookList();
			this.UpdateReferenceBookList();
		}

		// Token: 0x060073CD RID: 29645 RVA: 0x0035CFCC File Offset: 0x0035B1CC
		private void OnCloseReadingEvent(ArgumentBox argbox)
		{
			this.UpdateBookDisplayData();
		}

		// Token: 0x060073CE RID: 29646 RVA: 0x0035CFD8 File Offset: 0x0035B1D8
		private void OnDisable()
		{
			bool flag = this._curReadingBook.IsValid();
			if (flag)
			{
				SkillBookItem skillBookItem = SkillBook.Instance[this._curReadingBook.TemplateId];
				bool flag2 = skillBookItem != null;
				if (flag2)
				{
					bool flag3 = skillBookItem.ItemSubType == 1001;
					if (flag3)
					{
						GlobalDomainMethod.Call.InvokeGuidingTrigger(303);
					}
					else
					{
						bool flag4 = skillBookItem.ItemSubType == 1000;
						if (flag4)
						{
							GlobalDomainMethod.Call.InvokeGuidingTrigger(327);
						}
					}
				}
			}
			this._allBookIdList.Clear();
			GEvent.Remove(EEvents.OnMonthChange, new GEvent.Callback(this.OnMonthChange));
			GEvent.Remove(UiEvents.RefreshBookList, new GEvent.Callback(this.CurReadingBookDelete));
			GEvent.Remove(UiEvents.CloseReadingEvent, new GEvent.Callback(this.OnCloseReadingEvent));
			bool inGuiding = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			if (inGuiding)
			{
				TaiwuEventDomainMethod.Call.TriggerListener(EventActionKey.DefValue.TutorialExitViewReading, false);
			}
			this.bookIntroduction.DOKill(false);
			this.bookParticulars.DOKill(false);
			this.referenceBooks.DOKill(false);
			this.chooseReference.DOKill(false);
			this.chooseBookGroup.DOKill(false);
			this.chooseReferenceGroup.DOKill(false);
			this.bookIntroduction.anchoredPosition = new Vector2(398f, -31f);
			this.bookParticulars.anchoredPosition = new Vector2(198f, -25f);
			this.chooseReference.anchoredPosition = new Vector2(1200f, 3f);
			this.chooseBook.anchoredPosition = new Vector2(980f, 3f);
			this.chooseBookGroup.alpha = 0f;
			this.chooseReferenceGroup.alpha = 0f;
			UIManager.Instance.SetEscHandler(null);
		}

		// Token: 0x060073CF RID: 29647 RVA: 0x0035D1B4 File Offset: 0x0035B3B4
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(5, 47, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 60, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)this._taiwuCharId, new uint[]
			{
				43U,
				79U
			}));
		}

		// Token: 0x060073D0 RID: 29648 RVA: 0x0035D21C File Offset: 0x0035B41C
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b != 0)
				{
					if (b == 1)
					{
						this.HandleMethodReturn(notification.DomainId, notification.MethodId, notification.ValueOffset, wrapper.DataPool);
					}
				}
				else
				{
					this.HandleDataModification(notification.Uid, notification.ValueOffset, wrapper.DataPool);
				}
			}
		}

		// Token: 0x060073D1 RID: 29649 RVA: 0x0035D2C4 File Offset: 0x0035B4C4
		private unsafe void HandleDataModification(DataUid uid, int offset, RawDataPool dataPool)
		{
			ushort domainId = uid.DomainId;
			ushort num = domainId;
			if (num != 4)
			{
				if (num != 5)
				{
					if (num == 19)
					{
						ushort dataId = uid.DataId;
						ushort num2 = dataId;
						if (num2 != 10)
						{
							if (num2 == 60)
							{
								Serializer.Deserialize(dataPool, offset, ref this._readInLifeSkillCombatCount);
								this.SetReadingEventBtnMouseTip((int)this._readInCombatCount, (int)this._readInLifeSkillCombatCount);
							}
						}
						else
						{
							Serializer.Deserialize(dataPool, offset, ref this._readingEventBookIdList);
							this.SetReadingEventBtnState(this._readingEventBookIdList.Contains(this._curReadingBook.Id));
							this.UpdateUsedReferenceBooks();
						}
					}
				}
				else
				{
					switch (uid.DataId)
					{
					case 43:
					{
						Serializer.Deserialize(dataPool, offset, ref this._curReadingBook);
						this.empty.gameObject.SetActive(this._curReadingBook == ItemKey.Invalid);
						this.SyncBookUsingTypes();
						ItemDisplayData displayData = this._bookDisplayData.GetOrDefault(this._curReadingBook.Id);
						ReadingDisplayHelper.UpdateReadingBookInfo(this.bookIntro, this.readingInspireHolder, this._curReadingBook, displayData, this._referenceBooks, false, new Action(this.ChangeChooseBook), true);
						this.RefreshBookDesc(this._curReadingBook);
						bool flag = this._curReadingBook.IsValid();
						if (flag)
						{
							TaiwuDomainMethod.Call.GetCurReadingStrategies(this.Element.GameDataListenerId);
							bool flag2 = displayData != null && displayData.Durability == 1;
							if (flag2)
							{
								ViewReading.ShowDurabilityTip(this.durabilityTip);
							}
							else
							{
								ViewReading.HideDurabilityTip(this.durabilityTip);
							}
						}
						else
						{
							this.UpdateCurReadingBookPages(true);
							this._inited = true;
							this.SetReadingEventBtnState(false);
							ViewReading.HideDurabilityTip(this.durabilityTip);
						}
						this.intelligenceRoot.SetActive(this._curReadingBook.IsValid());
						this.UpdateUsedReferenceBooks();
						this.UpdateBookList();
						this.UpdateReferenceBookList();
						this.readingEventBtn.ClearAndAddListener(new Action(this.OpenReadingEvent));
						break;
					}
					case 44:
					{
						Serializer.Deserialize(dataPool, offset, ref this._referenceBooks);
						this.ClearReferenceSlotSelected();
						this._isChangingReferenceBook = false;
						this.SyncBookUsingTypes();
						this.UpdateUsedReferenceBooks();
						this._hasAvailableSlot = this.HasAvailableReferenceBookSlot();
						this.UpdateBookList();
						this.UpdateReferenceBookList();
						ItemDisplayData displayData2 = this._bookDisplayData.GetOrDefault(this._curReadingBook.Id);
						ReadingDisplayHelper.UpdateReadingBookInfo(this.bookIntro, this.readingInspireHolder, this._curReadingBook, displayData2, this._referenceBooks, false, new Action(this.ChangeChooseBook), true);
						break;
					}
					case 45:
						Serializer.Deserialize(dataPool, offset, ref this._unlockStates);
						this.UpdateUsedReferenceBooks();
						this._hasAvailableSlot = this.HasAvailableReferenceBookSlot();
						break;
					case 47:
						Serializer.Deserialize(dataPool, offset, ref this._readInCombatCount);
						break;
					}
				}
			}
			else
			{
				bool flag3 = uid.DataId == 0;
				if (flag3)
				{
					uint subId = uid.SubId1;
					uint num3 = subId;
					if (num3 != 43U)
					{
						if (num3 == 79U)
						{
							MainAttributes mainAttributes;
							Serializer.Deserialize(dataPool, offset, ref mainAttributes);
							this._maxIntelligence = (int)(*(ref mainAttributes.Items.FixedElementField + (IntPtr)5 * 2));
							this.intelligenceCost.SetText(this._curIntelligence.ToString() + "/" + this._maxIntelligence.ToString(), true);
						}
					}
					else
					{
						MainAttributes mainAttributes2;
						Serializer.Deserialize(dataPool, offset, ref mainAttributes2);
						this._curIntelligence = (int)(*(ref mainAttributes2.Items.FixedElementField + (IntPtr)5 * 2));
						this.intelligenceCost.SetText(this._curIntelligence.ToString() + "/" + this._maxIntelligence.ToString(), true);
					}
				}
			}
		}

		// Token: 0x060073D2 RID: 29650 RVA: 0x0035D684 File Offset: 0x0035B884
		private void HandleMethodReturn(ushort domainId, ushort methodId, int offset, RawDataPool dataPool)
		{
			switch (domainId)
			{
			case 4:
				if (methodId != 26)
				{
					if (methodId != 88)
					{
						if (methodId == 90)
						{
							int[] lifeSkillAttainments = null;
							Serializer.Deserialize(dataPool, offset, ref lifeSkillAttainments);
						}
					}
					else
					{
						int[] combatSkillAttainments = null;
						Serializer.Deserialize(dataPool, offset, ref combatSkillAttainments);
					}
				}
				else
				{
					List<ItemDisplayData> displayDataList = null;
					Serializer.Deserialize(dataPool, offset, ref displayDataList);
					this._receivedBookTypes += 1;
					List<int> ids = new List<int>();
					bool flag = displayDataList != null && displayDataList.Count > 0;
					if (flag)
					{
						foreach (ItemDisplayData data in displayDataList)
						{
							bool flag2 = !ids.Contains(data.Key.Id);
							if (flag2)
							{
								ids.Add(data.Key.Id);
							}
							this._bookDisplayData.Add(data.Key.Id, data);
							ItemDomainMethod.Call.GetSkillBookPagesInfo(this.Element.GameDataListenerId, data.Key);
						}
						bool flag3 = this._receivedBookTypes == 1;
						if (flag3)
						{
							this._combatSkillBooks.Clear();
							this._combatSkillBooks.AddRange(displayDataList);
						}
						else
						{
							this._lifeSkillBooks.Clear();
							this._lifeSkillBooks.AddRange(displayDataList);
						}
						this._allBookIdList.AddRange(ids);
					}
					else
					{
						bool flag4 = this._receivedBookTypes == 1;
						if (flag4)
						{
							this._combatSkillBooks.Clear();
						}
						else
						{
							this._lifeSkillBooks.Clear();
						}
					}
					this.SyncBookUsingTypes();
					bool flag5 = !this._inited && this._receivedBookTypes > 1;
					if (flag5)
					{
						TaiwuDomainMethod.Call.GetTotalReadingProgressList(this.Element.GameDataListenerId, this._allBookIdList);
						base.AppendMonitorFieldId(new UIBase.MonitorDataField(5, 43, ulong.MaxValue, null));
						base.AppendMonitorFieldId(new UIBase.MonitorDataField(5, 44, ulong.MaxValue, null));
						base.AppendMonitorFieldId(new UIBase.MonitorDataField(5, 45, ulong.MaxValue, null));
						base.AppendMonitorFieldId(new UIBase.MonitorDataField(19, 10, ulong.MaxValue, null));
						this.Element.ShowAfterRefresh();
					}
					ItemDisplayData displayData;
					bool flag6 = this._bookDisplayData.TryGetValue(this._curReadingBook.Id, out displayData);
					if (flag6)
					{
						ReadingDisplayHelper.UpdateReadingBookInfo(this.bookIntro, this.readingInspireHolder, this._curReadingBook, displayData, this._referenceBooks, false, new Action(this.ChangeChooseBook), true);
						this.RefreshBookDesc(this._curReadingBook);
					}
				}
				break;
			case 5:
			{
				bool flag7 = methodId == 30;
				if (flag7)
				{
					Serializer.Deserialize(dataPool, offset, ref this._curStrategies);
					ExtraDomainMethod.Call.GetBookStrategiesExpireTime(this.Element.GameDataListenerId, this._curReadingBook);
				}
				else
				{
					bool flag8 = methodId == 65;
					if (flag8)
					{
						List<sbyte> readingProgressList = new List<sbyte>();
						Serializer.Deserialize(dataPool, offset, ref readingProgressList);
						this._allBookReadingProgressList.Clear();
						bool flag9 = readingProgressList != null && readingProgressList.Count > 0;
						if (flag9)
						{
							for (int i = 0; i < readingProgressList.Count; i++)
							{
								this._allBookReadingProgressList.Add(this._allBookIdList[i], readingProgressList[i]);
							}
						}
					}
				}
				break;
			}
			case 6:
			{
				bool flag10 = methodId == 9;
				if (flag10)
				{
					SkillBookPageDisplayData displayData2 = null;
					Serializer.Deserialize(dataPool, offset, ref displayData2);
					this._pageDisplayData.Add(displayData2.ItemKey.Id, displayData2);
					bool flag11 = this._pageDisplayData.Count == this._bookDisplayData.Count && this._receivedBookTypes > 1;
					if (flag11)
					{
						this._receivedBookTypes = 0;
						this.UpdateBookList();
						this.UpdateReferenceBookList();
					}
				}
				break;
			}
			default:
				if (domainId == 19)
				{
					bool flag12 = methodId == 121;
					if (flag12)
					{
						Serializer.Deserialize(dataPool, offset, ref this._curReadingBookExpireTime);
						this.UpdateCurReadingBookPages(true);
						this._inited = true;
					}
				}
				break;
			}
		}

		// Token: 0x060073D3 RID: 29651 RVA: 0x0035DAB8 File Offset: 0x0035BCB8
		private void SyncBookUsingTypes()
		{
			foreach (ItemDisplayData displayData in this._bookDisplayData.Values)
			{
				displayData.UsingType = ItemDisplayData.ItemUsingType.Invalid;
			}
			ItemDisplayData readingBookDisplayData;
			bool flag = this._curReadingBook.IsValid() && this._bookDisplayData.TryGetValue(this._curReadingBook.Id, out readingBookDisplayData);
			if (flag)
			{
				readingBookDisplayData.UsingType = ItemDisplayData.ItemUsingType.Reading;
			}
			for (int i = 0; i < this._referenceBooks.Length; i++)
			{
				ItemKey referenceBook = this._referenceBooks[i];
				bool flag2 = !referenceBook.IsValid();
				if (!flag2)
				{
					ItemDisplayData referenceBookDisplayData;
					bool flag3 = !this._bookDisplayData.TryGetValue(referenceBook.Id, out referenceBookDisplayData);
					if (!flag3)
					{
						bool flag4 = this._curReadingBook.Equals(referenceBook);
						if (!flag4)
						{
							referenceBookDisplayData.UsingType = ItemDisplayData.ItemUsingType.Referring;
						}
					}
				}
			}
		}

		// Token: 0x060073D4 RID: 29652 RVA: 0x0035DBC8 File Offset: 0x0035BDC8
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = btnName == "RemoveCurBookBtn";
			if (flag)
			{
				bool interactable = this.readingEventBtn.interactable;
				if (interactable)
				{
					this.OnClickCheckDialogCmd(delegate
					{
						this.ClearCurrentReadingEvent();
						this.SetReadingBookInvalid();
					});
				}
				else
				{
					this.SetReadingBookInvalid();
				}
			}
			else
			{
				bool flag2 = btnName == "CloseBtn";
				if (flag2)
				{
					UIManager.Instance.HideUI(this.Element);
				}
			}
		}

		// Token: 0x060073D5 RID: 29653 RVA: 0x0035DC44 File Offset: 0x0035BE44
		private void UpdateCurReadingBookPages(bool clearPage = true)
		{
			TaiwuDomainMethod.AsyncCall.GetReadingResult(this, delegate(int offset, RawDataPool dataPool)
			{
				int[] progress = null;
				Serializer.Deserialize(dataPool, offset, ref progress);
				List<SkillBookPageDisplayData> refDataList = new List<SkillBookPageDisplayData>(this._referenceBooks.Length);
				foreach (ItemKey refBook in this._referenceBooks)
				{
					SkillBookPageDisplayData pageData;
					bool flag = refBook.IsValid() && this._pageDisplayData.TryGetValue(refBook.Id, out pageData);
					if (flag)
					{
						refDataList.Add(pageData);
					}
				}
				SkillBookPageDisplayData pageDisplayData = null;
				bool isBookRead = false;
				bool flag2 = this._curReadingBook.IsValid();
				if (flag2)
				{
					pageDisplayData = this._pageDisplayData[this._curReadingBook.Id];
					isBookRead = this._bookDisplayData[this._curReadingBook.Id].IsReadingFinished;
				}
				ReadingDisplayHelper.UpdatePages(this.readingPages, this._curReadingBook, pageDisplayData, this._curStrategies, this._curReadingBookExpireTime, progress, isBookRead, !this._inited, clearPage, refDataList, null, false);
			});
		}

		// Token: 0x060073D6 RID: 29654 RVA: 0x0035DC7C File Offset: 0x0035BE7C
		private void UpdateUsedReferenceBooks()
		{
			int unlockedCount = 3;
			for (int i = 0; i < this._referenceBooks.Length; i++)
			{
				ItemKey referenceBook = this._referenceBooks[i];
				ReadingReferenceBookInfoPrefab refers = this.referenceBookList[i];
				bool flag = !this.IsReferenceBookSlotUnlocked(i);
				if (flag)
				{
					unlockedCount--;
					refers.emptyBack.SetActive(false);
					refers.selectedBack.SetActive(false);
					refers.lockBack.SetActive(true);
					refers.lockTip.SetText(LocalStringManager.GetFormat(LanguageKey.LK_Reading_LockTip, GlobalConfig.Instance.ReferenceBookSlotUnlockParams[i].ToString().SetColor("orange")), true);
					refers.specialEffect.SetActive(false);
					ViewReading.HideDurabilityTip(refers.durabilityTip);
				}
				else
				{
					ItemDisplayData displayData;
					bool flag2 = referenceBook.IsValid() && this._bookDisplayData.TryGetValue(referenceBook.Id, out displayData);
					if (flag2)
					{
						refers.emptyBack.SetActive(false);
						refers.selectedBack.SetActive(true);
						refers.lockBack.SetActive(false);
						SkillBookItem refBookCfg = SkillBook.Instance[referenceBook.TemplateId];
						refers.txtName.text = refBookCfg.Name;
						refers.durability.text = CommonUtils.GetDurabilityString((int)displayData.Durability, (int)displayData.MaxDurability);
						refers.item.Set(displayData, false);
						refers.selectedBtnTooltipInvoker.Type = TipType.SkillBook;
						refers.selectedBtnTooltipInvoker.RuntimeParam = new ArgumentBox().SetObject("ItemData", displayData).Set("ShowPageInfo", true).Set("TemplateDataOnly", false);
						bool flag3 = displayData.Durability == 1;
						if (flag3)
						{
							ViewReading.ShowDurabilityTip(refers.durabilityTip);
						}
						else
						{
							ViewReading.HideDurabilityTip(refers.durabilityTip);
						}
						int index = i;
						bool interactable = this.readingEventBtn.interactable;
						if (interactable)
						{
							Action <>9__3;
							refers.removeBtn.ClearAndAddListener(delegate
							{
								ViewReading <>4__this = this;
								Action yesAction;
								if ((yesAction = <>9__3) == null)
								{
									yesAction = (<>9__3 = delegate()
									{
										this.ClearCurrentReadingEvent();
										this.RemoveReferenceBookBySlotIndex(index);
									});
								}
								<>4__this.OnClickCheckDialogCmd(yesAction);
							});
						}
						else
						{
							refers.removeBtn.ClearAndAddListener(delegate
							{
								this.RemoveReferenceBookBySlotIndex(index);
							});
						}
						this.RefreshReferenceBookPages(refers, referenceBook);
						bool isSpecial = this.HasReferenceBonus(referenceBook);
						refers.specialEffect.SetActive(isSpecial);
						refers.efficiencyHolder.SetActive(this._curReadingBook.IsValid() && referenceBook.IsValid());
						bool flag4 = this._curReadingBook.IsValid() && referenceBook.IsValid();
						if (flag4)
						{
							TaiwuDomainMethod.AsyncCall.GetRefBonusSpeed(this, referenceBook, delegate(int offset, RawDataPool dataPool)
							{
								int refBonusSpeed = 0;
								Serializer.Deserialize(dataPool, offset, ref refBonusSpeed);
								string efficiencyText = string.Format("{0}%", refBonusSpeed);
								refers.efficiency.text = (isSpecial ? efficiencyText.SetColor("goldyellow") : efficiencyText);
							});
						}
						Transform strategyRoot = refers.strategy.transform;
						TextMeshProUGUI readingEventBonusRateText = refers.rate;
						bool flag5 = refBookCfg.LifeSkillTemplateId > -1;
						if (flag5)
						{
							int readingEventBonusRate = LifeSkill.Instance[refBookCfg.LifeSkillTemplateId].ReadingEventBonusRate;
							readingEventBonusRateText.text = string.Format("{0}%", Mathf.Max(0, readingEventBonusRate));
							readingEventBonusRateText.transform.parent.gameObject.SetActive(true);
							short lifeSkillTemplateId = ItemTemplateHelper.GetLifeSkillTemplateIdFromSkillBook((int)referenceBook.TemplateId);
							List<byte> providedStrategies = LifeSkill.Instance[lifeSkillTemplateId].ProvidedReadingStrategies;
							GameObject template = strategyRoot.GetChild(0).gameObject;
							bool flag6 = providedStrategies != null && providedStrategies.Count > 0;
							if (flag6)
							{
								strategyRoot.parent.gameObject.SetActive(true);
								for (int j = 0; j < providedStrategies.Count; j++)
								{
									byte strategyId = providedStrategies[j];
									ReadingStrategyItem strategyConfig = ReadingStrategy.Instance[(int)strategyId];
									GameObject strategyObj = (strategyRoot.childCount <= j) ? Object.Instantiate<GameObject>(template, strategyRoot) : strategyRoot.GetChild(j).gameObject;
									strategyObj.SetActive(true);
									strategyObj.GetComponentInChildren<TextMeshProUGUI>().text = strategyConfig.Name.First<char>().ToString();
									TooltipInvoker tipDisplayer = strategyObj.GetComponent<TooltipInvoker>();
									TooltipInvoker tooltipInvoker = tipDisplayer;
									if (tooltipInvoker.RuntimeParam == null)
									{
										tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
									}
									tipDisplayer.RuntimeParam.Clear();
									tipDisplayer.RuntimeParam.Set("arg0", strategyConfig.Name).Set("arg1", strategyConfig.Desc);
								}
								for (int k = providedStrategies.Count; k < strategyRoot.childCount; k++)
								{
									strategyRoot.GetChild(k).gameObject.SetActive(false);
								}
							}
							else
							{
								strategyRoot.parent.gameObject.SetActive(false);
							}
						}
						else
						{
							readingEventBonusRateText.transform.parent.gameObject.SetActive(false);
							strategyRoot.parent.gameObject.SetActive(false);
						}
					}
					else
					{
						refers.emptyBack.SetActive(true);
						refers.selectedBack.SetActive(false);
						refers.lockBack.SetActive(false);
						refers.specialEffect.SetActive(false);
						ViewReading.HideDurabilityTip(refers.durabilityTip);
					}
				}
			}
		}

		// Token: 0x060073D7 RID: 29655 RVA: 0x0035E2CC File Offset: 0x0035C4CC
		private void RefreshReferenceBookPages(ReadingReferenceBookInfoPrefab refers, ItemKey itemKey)
		{
			SkillBookPageDisplayData pageDisplayData;
			bool flag = !this._pageDisplayData.TryGetValue(itemKey.Id, out pageDisplayData);
			if (!flag)
			{
				SkillBookPageDisplayData readingPageDisplayData = this._curReadingBook.IsValid() ? this._pageDisplayData[this._curReadingBook.Id] : null;
				SkillBookItem configData = SkillBook.Instance[itemKey.TemplateId];
				int pageCount = (configData.ItemSubType == 1001) ? 6 : 5;
				for (int i = 0; i < 6; i++)
				{
					ReadingBookPages page = refers.pages[i];
					bool flag2 = i >= pageCount;
					if (flag2)
					{
						page.gameObject.SetActive(false);
					}
					else
					{
						page.gameObject.SetActive(true);
						ReadingDisplayHelper.SetPageCompleteState(pageDisplayData.State[i], refers.pages[i].pageIcon);
					}
				}
				for (int j = 0; j < refers.pages.Count; j++)
				{
					bool flag3 = readingPageDisplayData != null && readingPageDisplayData.State.CheckIndex(j) && this._curReadingBook.IsValid() && this._curReadingBook.TemplateEquals(itemKey);
					if (flag3)
					{
						sbyte readingState = readingPageDisplayData.State[j];
						sbyte state = pageDisplayData.State[j];
						bool hasSupply = state < readingState;
						bool flag4 = pageDisplayData.IsCombatBook && pageDisplayData.Type[j] != readingPageDisplayData.Type[j];
						if (flag4)
						{
							hasSupply = false;
						}
						refers.pages[j].supply.SetActive(hasSupply);
					}
					else
					{
						refers.pages[j].supply.SetActive(false);
					}
				}
			}
		}

		// Token: 0x060073D8 RID: 29656 RVA: 0x0035E498 File Offset: 0x0035C698
		private static void ShowDurabilityTip(GameObject tip)
		{
			tip.SetActive(true);
			CanvasGroup canvasGroup = tip.GetComponent<CanvasGroup>();
			canvasGroup.DOKill(false);
			canvasGroup.alpha = 1f;
			canvasGroup.DOFade(0.2f, 1f).SetLoops(5, LoopType.Yoyo).OnComplete(delegate
			{
				canvasGroup.alpha = 1f;
			});
		}

		// Token: 0x060073D9 RID: 29657 RVA: 0x0035E50C File Offset: 0x0035C70C
		private static void HideDurabilityTip(GameObject tip)
		{
			tip.SetActive(false);
		}

		// Token: 0x060073DA RID: 29658 RVA: 0x0035E518 File Offset: 0x0035C718
		private void RefreshBookList(bool isRef)
		{
			List<ItemKey> bookList = isRef ? this._availableReferenceBookList : this._availableBookList;
			List<ITradeableContent> filterSource = isRef ? this._referenceBookFilterSource : this._bookFilterSource;
			bool isCombatSkill = isRef ? this.RefTypeIsCombatSkill : this.TypeIsCombatSkill;
			List<ItemDisplayData> source = isCombatSkill ? this._combatSkillBooks : this._lifeSkillBooks;
			SortAndFilterController<ITradeableContent> controller = this.GetActiveSortAndFilterController(isRef, isCombatSkill);
			bookList.Clear();
			filterSource.Clear();
			this.CollectBookFilterSource(filterSource, source, isRef);
			controller.NotifyDataChanged(filterSource);
			Func<ITradeableContent, bool> filter = controller.GenerateFilter();
			List<ITradeableContent> filteredData = new List<ITradeableContent>();
			foreach (ITradeableContent data in filterSource)
			{
				bool flag = !filter(data);
				if (!flag)
				{
					filteredData.Add(data);
					bookList.Add(data.Key);
				}
			}
			controller.AfterFilter(filterSource);
		}

		// Token: 0x060073DB RID: 29659 RVA: 0x0035E620 File Offset: 0x0035C820
		private void CollectBookFilterSource(List<ITradeableContent> filterSource, List<ItemDisplayData> source, bool isRef)
		{
			foreach (ItemDisplayData data in source)
			{
				bool flag = !this._allBookReadingProgressList.ContainsKey(data.Key.Id);
				if (!flag)
				{
					bool flag2 = isRef && this._curReadingBook.Equals(data.Key);
					if (!flag2)
					{
						bool flag3 = !isRef && this._referenceBooks.Contains(data.Key);
						if (!flag3)
						{
							filterSource.Add(data);
						}
					}
				}
			}
		}

		// Token: 0x060073DC RID: 29660 RVA: 0x0035E6D0 File Offset: 0x0035C8D0
		private SortAndFilterController<ITradeableContent> GetActiveSortAndFilterController(bool isRef, bool isCombatSkill)
		{
			SortAndFilterController<ITradeableContent> result;
			if (isRef)
			{
				result = (isCombatSkill ? this._selectCombatReferenceBookSortAndFilterController : this._selectLifeReferenceBookSortAndFilterController);
			}
			else
			{
				result = (isCombatSkill ? this._selectCombatBookSortAndFilterController : this._selectLifeBookSortAndFilterController);
			}
			return result;
		}

		// Token: 0x060073DD RID: 29661 RVA: 0x0035E70C File Offset: 0x0035C90C
		private void UpdateBookList()
		{
			this.RefreshBookList(false);
			SortAndFilterController<ITradeableContent> controller = this.GetActiveSortAndFilterController(false, this.TypeIsCombatSkill);
			this.SortBookListByController(this._availableBookList, controller, new Comparison<ItemKey>(this.CompareBook));
			this.selectBookScroll.SetData<ItemKey>(this._availableBookList, -1);
		}

		// Token: 0x060073DE RID: 29662 RVA: 0x0035E760 File Offset: 0x0035C960
		private void UpdateReferenceBookList()
		{
			ViewReading.<>c__DisplayClass151_0 CS$<>8__locals1 = new ViewReading.<>c__DisplayClass151_0();
			CS$<>8__locals1.<>4__this = this;
			this.RefreshBookList(true);
			this._availableReferenceBookBonusSpeedDict.Clear();
			CS$<>8__locals1.count = 0;
			bool flag = this._availableReferenceBookList.Count != 0;
			if (flag)
			{
				using (List<ItemKey>.Enumerator enumerator = this._availableReferenceBookList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ItemKey bookKey = enumerator.Current;
						TaiwuDomainMethod.AsyncCall.GetRefBonusSpeed(this, bookKey, delegate(int offset, RawDataPool dataPool)
						{
							int refBonusSpeed = 0;
							Serializer.Deserialize(dataPool, offset, ref refBonusSpeed);
							CS$<>8__locals1.<>4__this._availableReferenceBookBonusSpeedDict[bookKey] = refBonusSpeed;
							int count = CS$<>8__locals1.count;
							CS$<>8__locals1.count = count + 1;
							bool flag2 = CS$<>8__locals1.count == CS$<>8__locals1.<>4__this._availableReferenceBookList.Count;
							if (flag2)
							{
								CS$<>8__locals1.<UpdateReferenceBookList>g__OnGotBonusSpeed|0();
							}
						});
					}
				}
			}
			else
			{
				this.selectReferenceBookScroll.SetData<ItemKey>(this._availableReferenceBookList, -1);
			}
		}

		// Token: 0x060073DF RID: 29663 RVA: 0x0035E82C File Offset: 0x0035CA2C
		private void SortBookListByController(List<ItemKey> bookList, SortAndFilterController<ITradeableContent> controller, Comparison<ItemKey> fallbackComparer)
		{
			bool flag = bookList.Count <= 1;
			if (!flag)
			{
				List<ITradeableContent> sourceData = new List<ITradeableContent>(bookList.Count);
				foreach (ItemKey key in bookList)
				{
					ItemDisplayData displayData;
					bool flag2 = this._bookDisplayData.TryGetValue(key.Id, out displayData);
					if (flag2)
					{
						sourceData.Add(displayData);
					}
				}
				Comparison<ITradeableContent> comparer = controller.GenerateComparer(sourceData);
				bool flag3 = comparer == null;
				if (flag3)
				{
					bookList.Sort(fallbackComparer);
				}
				else
				{
					bookList.Sort(delegate(ItemKey a, ItemKey b)
					{
						ItemDisplayData aDisplayData;
						ItemDisplayData bDisplayData;
						bool flag4 = !this._bookDisplayData.TryGetValue(a.Id, out aDisplayData) || !this._bookDisplayData.TryGetValue(b.Id, out bDisplayData);
						int result;
						if (flag4)
						{
							result = fallbackComparer(a, b);
						}
						else
						{
							int comparerResult = comparer(aDisplayData, bDisplayData);
							bool flag5 = comparerResult != 0;
							if (flag5)
							{
								result = comparerResult;
							}
							else
							{
								result = fallbackComparer(a, b);
							}
						}
						return result;
					});
				}
			}
		}

		// Token: 0x060073E0 RID: 29664 RVA: 0x0035E910 File Offset: 0x0035CB10
		private int CompareBookContent(ITradeableContent a, ITradeableContent b)
		{
			return this.CompareBook(a.Key, b.Key);
		}

		// Token: 0x060073E1 RID: 29665 RVA: 0x0035E934 File Offset: 0x0035CB34
		private int CompareReferenceBookContent(ITradeableContent a, ITradeableContent b)
		{
			return this.CompareReferenceBook(a.Key, b.Key);
		}

		// Token: 0x060073E2 RID: 29666 RVA: 0x0035E958 File Offset: 0x0035CB58
		private int CompareBook(ItemKey a, ItemKey b)
		{
			bool aUsing = this._curReadingBook.Equals(a);
			bool bUsing = this._curReadingBook.Equals(b);
			bool flag = aUsing != bUsing;
			int result;
			if (flag)
			{
				result = (aUsing ? -1 : 1);
			}
			else
			{
				bool aFinished = this._allBookReadingProgressList.GetOrDefault(a.Id) >= 100;
				bool bFinished = this._allBookReadingProgressList.GetOrDefault(b.Id) >= 100;
				bool flag2 = aFinished != bFinished;
				if (flag2)
				{
					result = (aFinished ? 1 : -1);
				}
				else
				{
					sbyte aGrade = this._bookDisplayData[a.Id].Grade;
					sbyte bGrade = this._bookDisplayData[b.Id].Grade;
					bool flag3 = aGrade != bGrade;
					if (flag3)
					{
						result = ((aGrade > bGrade) ? -1 : 1);
					}
					else
					{
						int aPageScore = this.GetUnreadPageScore(a.Id);
						int bPageScore = this.GetUnreadPageScore(b.Id);
						bool flag4 = aPageScore != bPageScore;
						if (flag4)
						{
							result = ((aPageScore > bPageScore) ? -1 : 1);
						}
						else
						{
							result = a.TemplateId.CompareTo(b.TemplateId);
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060073E3 RID: 29667 RVA: 0x0035EA88 File Offset: 0x0035CC88
		private int GetUnreadPageScore(int bookId)
		{
			SkillBookPageDisplayData pageData;
			bool flag = !this._pageDisplayData.TryGetValue(bookId, out pageData);
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				int score = 0;
				for (int i = 0; i < pageData.State.Length; i++)
				{
					bool flag2 = pageData.ReadingProgress[i] < 100;
					if (flag2)
					{
						int num = score;
						sbyte b = pageData.State[i];
						if (!true)
						{
						}
						int num2;
						switch (b)
						{
						case 0:
							num2 = 3;
							break;
						case 1:
							num2 = 2;
							break;
						case 2:
							num2 = 1;
							break;
						default:
							num2 = 0;
							break;
						}
						if (!true)
						{
						}
						score = num + num2;
					}
				}
				result = score;
			}
			return result;
		}

		// Token: 0x060073E4 RID: 29668 RVA: 0x0035EB30 File Offset: 0x0035CD30
		private int CompareReferenceBook(ItemKey a, ItemKey b)
		{
			bool aUsing = this._referenceBooks.Contains(a);
			bool bUsing = this._referenceBooks.Contains(b);
			bool flag = aUsing != bUsing;
			int result;
			if (flag)
			{
				result = (aUsing ? -1 : 1);
			}
			else
			{
				bool flag2 = aUsing;
				if (flag2)
				{
					result = ((this._referenceBooks.IndexOf(a) < this._referenceBooks.IndexOf(b)) ? -1 : 1);
				}
				else
				{
					bool aCanSupply = this.CheckRefCanSupply(a);
					bool bCanSupply = this.CheckRefCanSupply(b);
					bool flag3 = aCanSupply != bCanSupply;
					if (flag3)
					{
						result = (aCanSupply ? -1 : 1);
					}
					else
					{
						int aBonusSpeed = this._availableReferenceBookBonusSpeedDict.GetOrDefault(a);
						int bBonusSpeed = this._availableReferenceBookBonusSpeedDict.GetOrDefault(b);
						bool flag4 = aBonusSpeed != bBonusSpeed;
						if (flag4)
						{
							result = ((aBonusSpeed > bBonusSpeed) ? -1 : 1);
						}
						else
						{
							bool aIsSpecial = this.HasReferenceBonus(a);
							bool bIsSpecial = this.HasReferenceBonus(b);
							bool flag5 = aIsSpecial != bIsSpecial;
							if (flag5)
							{
								result = (aIsSpecial ? -1 : 1);
							}
							else
							{
								sbyte aGrade = this._bookDisplayData[a.Id].Grade;
								sbyte bGrade = this._bookDisplayData[b.Id].Grade;
								bool flag6 = aGrade != bGrade;
								if (flag6)
								{
									result = ((aGrade < bGrade) ? -1 : 1);
								}
								else
								{
									int aPageScore = this.GetUnreadPageScore(a.Id);
									int bPageScore = this.GetUnreadPageScore(b.Id);
									bool flag7 = aPageScore != bPageScore;
									if (flag7)
									{
										result = ((aPageScore < bPageScore) ? -1 : 1);
									}
									else
									{
										result = a.TemplateId.CompareTo(b.TemplateId);
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060073E5 RID: 29669 RVA: 0x0035ECD4 File Offset: 0x0035CED4
		private bool CheckRefCanSupply(ItemKey refBookKey)
		{
			bool flag = !this._curReadingBook.IsValid() || !this._curReadingBook.TemplateEquals(refBookKey);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				SkillBookPageDisplayData pageDisplayData = this._pageDisplayData[refBookKey.Id];
				SkillBookPageDisplayData readingPageDisplayData = this._pageDisplayData[this._curReadingBook.Id];
				bool hasSupply = false;
				for (int i = 0; i < readingPageDisplayData.State.Length; i++)
				{
					sbyte readingState = readingPageDisplayData.State[i];
					sbyte state = pageDisplayData.State[i];
					hasSupply = (state < readingState);
					bool flag2 = pageDisplayData.IsCombatBook && pageDisplayData.Type[i] != readingPageDisplayData.Type[i];
					if (flag2)
					{
						hasSupply = false;
					}
					bool flag3 = hasSupply;
					if (flag3)
					{
						break;
					}
				}
				result = hasSupply;
			}
			return result;
		}

		// Token: 0x060073E6 RID: 29670 RVA: 0x0035EDB0 File Offset: 0x0035CFB0
		private bool HasReferenceBonus(ItemKey refBookKey)
		{
			return this.CheckRefBonusSpeedIsSpecial(refBookKey) || this.IsRefBookSameType(refBookKey);
		}

		// Token: 0x060073E7 RID: 29671 RVA: 0x0035EDD8 File Offset: 0x0035CFD8
		private bool IsRefBookSameType(ItemKey refBookKey)
		{
			bool flag = !this._curReadingBook.IsValid();
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				SkillBookItem bookConfig = SkillBook.Instance[this._curReadingBook.TemplateId];
				SkillBookItem refBookConfig = SkillBook.Instance[refBookKey.TemplateId];
				bool flag2 = bookConfig.ItemSubType != refBookConfig.ItemSubType;
				if (flag2)
				{
					result = false;
				}
				else
				{
					sbyte refBookSkillType = (refBookConfig.ItemSubType == 1000) ? refBookConfig.LifeSkillType : refBookConfig.CombatSkillType;
					sbyte bookSkillType = (bookConfig.ItemSubType == 1000) ? bookConfig.LifeSkillType : bookConfig.CombatSkillType;
					result = (refBookSkillType == bookSkillType);
				}
			}
			return result;
		}

		// Token: 0x060073E8 RID: 29672 RVA: 0x0035EE88 File Offset: 0x0035D088
		private bool CheckRefBonusSpeedIsSpecial(ItemKey refBookKey)
		{
			bool flag = !this._curReadingBook.IsValid();
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				SkillBookItem bookConfig = SkillBook.Instance[this._curReadingBook.TemplateId];
				List<short> bonusRefBookIds = bookConfig.ReferenceBooksWithBonus;
				result = (bonusRefBookIds != null && bonusRefBookIds.Contains(refBookKey.TemplateId));
			}
			return result;
		}

		// Token: 0x060073E9 RID: 29673 RVA: 0x0035EEDF File Offset: 0x0035D0DF
		private void OnMonthChange(ArgumentBox argBox)
		{
			this.UpdateBookDisplayData();
		}

		// Token: 0x060073EA RID: 29674 RVA: 0x0035EEEC File Offset: 0x0035D0EC
		private void UpdateBookDisplayData()
		{
			this._bookDisplayData.Clear();
			this._pageDisplayData.Clear();
			this._allBookIdList.Clear();
			this._receivedBookTypes = 0;
			this._inited = false;
			CharacterDomainMethod.Call.GetInventoryItems(this.Element.GameDataListenerId, this._taiwuCharId, 1001);
			CharacterDomainMethod.Call.GetInventoryItems(this.Element.GameDataListenerId, this._taiwuCharId, 1000);
		}

		// Token: 0x060073EB RID: 29675 RVA: 0x0035EF64 File Offset: 0x0035D164
		private void OnClickCheckDialogCmd(Action yesAction)
		{
			this._bookChangeDialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_ReadingEvent);
			this._bookChangeDialogCmd.Content = LocalStringManager.Get(LanguageKey.LK_ReadingEvent_ChangeBook_Tip).ColorReplace();
			this._bookChangeDialogCmd.Yes = yesAction;
			this._bookChangeDialogCmd.No = delegate()
			{
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", this._bookChangeDialogCmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x060073EC RID: 29676 RVA: 0x0035F007 File Offset: 0x0035D207
		private void ClearCurrentReadingEvent()
		{
			this.SetReadingEventBtnState(false);
			ExtraDomainMethod.Call.RemoveReadingEventBookId(this._curReadingBook.Id, this._curReadingBook.TemplateId);
		}

		// Token: 0x060073ED RID: 29677 RVA: 0x0035F030 File Offset: 0x0035D230
		private void SetReferenceSlotSelected(int slotIndex)
		{
			this.ClearReferenceSlotSelected();
			this._selectedReferenceSlotIndex = slotIndex;
			ReadingReferenceBookInfoPrefab refBook = this.referenceBookList[slotIndex];
			bool flag = refBook.emptyBack.activeSelf && refBook.emptySelectedGo != null;
			if (flag)
			{
				refBook.emptySelectedGo.SetActive(true);
			}
			bool flag2 = refBook.selectedBack.activeSelf && refBook.selectedBackSelectedGo != null;
			if (flag2)
			{
				refBook.selectedBackSelectedGo.SetActive(true);
			}
		}

		// Token: 0x060073EE RID: 29678 RVA: 0x0035F0B4 File Offset: 0x0035D2B4
		private void ClearReferenceSlotSelected()
		{
			bool flag = this._selectedReferenceSlotIndex < 0;
			if (!flag)
			{
				ReadingReferenceBookInfoPrefab refBook = this.referenceBookList[this._selectedReferenceSlotIndex];
				bool flag2 = refBook.emptySelectedGo != null;
				if (flag2)
				{
					refBook.emptySelectedGo.SetActive(false);
				}
				bool flag3 = refBook.selectedBackSelectedGo != null;
				if (flag3)
				{
					refBook.selectedBackSelectedGo.SetActive(false);
				}
				this._selectedReferenceSlotIndex = -1;
			}
		}

		// Token: 0x060073EF RID: 29679 RVA: 0x0035F124 File Offset: 0x0035D324
		private void RemoveReferenceBookBySlotIndex(int index)
		{
			bool flag = index < this._referenceBooks.Length - 1;
			if (flag)
			{
				Array.Copy(this._referenceBooks, index + 1, this._referenceBooks, index, this._referenceBooks.Length - index - 1);
			}
			ItemKey[] array = this._referenceBooks;
			array[array.Length - 1] = ItemKey.Invalid;
			for (int i = this._referenceBooks.Length - 1; i >= index; i--)
			{
				bool flag2 = this.IsReferenceBookSlotUnlocked(i);
				if (flag2)
				{
					TaiwuDomainMethod.Call.SetReferenceBook((sbyte)i, this._referenceBooks[i]);
				}
			}
		}

		// Token: 0x060073F0 RID: 29680 RVA: 0x0035F1B8 File Offset: 0x0035D3B8
		private void ChangeReadingBook(int togNewIndex)
		{
			bool flag = togNewIndex < 0;
			if (!flag)
			{
				ItemKey curSelectedBook = this._availableBookList[togNewIndex];
				TaiwuDomainMethod.Call.SetReadingBook(curSelectedBook);
				this.SetReadingEventBtnState(this._readingEventBookIdList.Contains(curSelectedBook.Id));
			}
		}

		// Token: 0x060073F1 RID: 29681 RVA: 0x0035F1FC File Offset: 0x0035D3FC
		private void SetReadingBookInvalid()
		{
			this._curReadingBook = ItemKey.Invalid;
			this.empty.gameObject.SetActive(this._curReadingBook == ItemKey.Invalid);
			TaiwuDomainMethod.Call.SetReadingBook(ItemKey.Invalid);
		}

		// Token: 0x060073F2 RID: 29682 RVA: 0x0035F238 File Offset: 0x0035D438
		private void ChangeReferenceBook(int togNewIndex, int togOldIndex)
		{
			bool isChangingReferenceBook = this._isChangingReferenceBook;
			if (!isChangingReferenceBook)
			{
				this._isChangingReferenceBook = true;
				bool flag = togNewIndex < 0 && togOldIndex >= 0;
				if (flag)
				{
					ItemKey refBook = this._availableReferenceBookList[togOldIndex];
					int index = this._referenceBooks.IndexOf(refBook);
					bool flag2 = index < 0;
					if (flag2)
					{
						this._isChangingReferenceBook = false;
					}
					else
					{
						bool flag3 = index < this._referenceBooks.Length - 1;
						if (flag3)
						{
							Array.Copy(this._referenceBooks, index + 1, this._referenceBooks, index, this._referenceBooks.Length - index - 1);
						}
						ItemKey[] array = this._referenceBooks;
						array[array.Length - 1] = ItemKey.Invalid;
						for (int i = this._referenceBooks.Length - 1; i >= index; i--)
						{
							bool flag4 = this.IsReferenceBookSlotUnlocked(i);
							if (flag4)
							{
								TaiwuDomainMethod.Call.SetReferenceBook((sbyte)i, this._referenceBooks[i]);
							}
						}
					}
				}
				else
				{
					ItemKey bookToAdd = this._availableReferenceBookList[togNewIndex];
					bool flag5 = this._referenceBooks.Contains(bookToAdd);
					if (flag5)
					{
						this._isChangingReferenceBook = false;
					}
					else
					{
						sbyte j = 0;
						while ((int)j < this._referenceBooks.Length)
						{
							bool flag6 = !this._referenceBooks[(int)j].IsValid() && (this._unlockStates >> (int)j & 1) == 1;
							if (flag6)
							{
								this._referenceBooks[(int)j] = bookToAdd;
								TaiwuDomainMethod.Call.SetReferenceBook(j, bookToAdd);
								return;
							}
							j += 1;
						}
						this._isChangingReferenceBook = false;
					}
				}
			}
		}

		// Token: 0x060073F3 RID: 29683 RVA: 0x0035F3D8 File Offset: 0x0035D5D8
		private void RefreshBookDesc(ItemKey bookKey)
		{
			bool flag = !bookKey.IsValid();
			if (flag)
			{
				this.description.text = "";
			}
			else
			{
				SkillBookItem configData = SkillBook.Instance[bookKey.TemplateId];
				this.description.text = configData.Desc;
			}
		}

		// Token: 0x060073F4 RID: 29684 RVA: 0x0035F42C File Offset: 0x0035D62C
		private void MoveToChooseBookForward()
		{
			this.bookIntroduction.DOKill(false);
			this.bookParticulars.DOKill(false);
			this.referenceBooks.DOKill(false);
			this.chooseBook.DOKill(false);
			this.chooseReference.DOKill(false);
			this.chooseBookGroup.DOKill(false);
			this.chooseReferenceGroup.DOKill(false);
			this.bookIntroduction.DOAnchorPosX(-275f, 0.35f, false).SetEase(Ease.OutSine);
			this.bookParticulars.DOAnchorPosX(-410f, 0.35f, false).SetEase(Ease.OutSine);
			this.referenceBooks.DOAnchorPosX(-237f, 0.35f, false).SetEase(Ease.OutSine);
			this.chooseBook.DOAnchorPosX(-20f, 0.35f, false).SetEase(Ease.OutSine);
			this.chooseReference.DOAnchorPosX(1200f, 0.35f, false).SetEase(Ease.OutSine);
			this.chooseBookGroup.DOFade(1f, 0.5f);
			this.chooseReferenceGroup.DOFade(0f, 0.5f);
		}

		// Token: 0x060073F5 RID: 29685 RVA: 0x0035F554 File Offset: 0x0035D754
		private void MoveToChooseBookBackForward()
		{
			this.bookIntroduction.DOKill(false);
			this.bookParticulars.DOKill(false);
			this.referenceBooks.DOKill(false);
			this.chooseBook.DOKill(false);
			this.chooseBookGroup.DOKill(false);
			this.chooseReferenceGroup.DOKill(false);
			this.bookIntroduction.DOAnchorPosX(398f, 0.35f, false).SetEase(Ease.OutSine);
			this.bookParticulars.DOAnchorPosX(198f, 0.35f, false).SetEase(Ease.OutSine);
			this.referenceBooks.DOAnchorPosX(-180f, 0.35f, false).SetEase(Ease.OutSine);
			this.chooseBook.DOAnchorPosX(980f, 0.35f, false).SetEase(Ease.OutSine);
			this.chooseBookGroup.DOFade(0f, 0.5f);
			this.chooseReferenceGroup.DOFade(0f, 0.5f);
		}

		// Token: 0x060073F6 RID: 29686 RVA: 0x0035F650 File Offset: 0x0035D850
		private void MoveToChooseReferenceForward()
		{
			this.bookIntroduction.DOKill(false);
			this.bookParticulars.DOKill(false);
			this.referenceBooks.DOKill(false);
			this.chooseReference.DOKill(false);
			this.chooseBook.DOKill(false);
			this.chooseBookGroup.DOKill(false);
			this.chooseReferenceGroup.DOKill(false);
			this.bookIntroduction.DOAnchorPosX(-275f, 0.35f, false).SetEase(Ease.OutSine);
			this.bookParticulars.DOAnchorPosX(-525f, 0.35f, false).SetEase(Ease.OutSine);
			this.referenceBooks.DOAnchorPosX(-326f, 0.35f, false).SetEase(Ease.OutSine);
			this.chooseBook.DOAnchorPosX(980f, 0.35f, false).SetEase(Ease.OutSine);
			this.chooseReference.DOAnchorPosX(-20f, 0.35f, false).SetEase(Ease.OutSine);
			this.chooseBookGroup.DOFade(0f, 0.5f);
			this.chooseReferenceGroup.DOFade(1f, 0.5f);
			UIManager.Instance.SetEscHandler(null);
		}

		// Token: 0x060073F7 RID: 29687 RVA: 0x0035F784 File Offset: 0x0035D984
		private void MoveToChooseReferenceBackForward()
		{
			this.ClearReferenceSlotSelected();
			this.bookIntroduction.DOKill(false);
			this.bookParticulars.DOKill(false);
			this.referenceBooks.DOKill(false);
			this.chooseReference.DOKill(false);
			this.chooseBookGroup.DOKill(false);
			this.chooseReferenceGroup.DOKill(false);
			this.bookIntroduction.DOAnchorPosX(398f, 0.35f, false).SetEase(Ease.OutSine);
			this.bookParticulars.DOAnchorPosX(198f, 0.35f, false).SetEase(Ease.OutSine);
			this.referenceBooks.DOAnchorPosX(-180f, 0.35f, false).SetEase(Ease.OutSine);
			this.chooseReference.DOAnchorPosX(1200f, 0.35f, false).SetEase(Ease.OutSine);
			this.chooseBookGroup.DOFade(0f, 0.5f);
			this.chooseReferenceGroup.DOFade(0f, 0.5f);
			UIManager.Instance.SetEscHandler(null);
		}

		// Token: 0x060073F8 RID: 29688 RVA: 0x0035F894 File Offset: 0x0035DA94
		private bool HasAvailableReferenceBookSlot()
		{
			bool flag = this._referenceBooks == null;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				for (int i = 0; i < this._referenceBooks.Length; i++)
				{
					bool flag2 = this._referenceBooks[i].IsValid();
					if (!flag2)
					{
						bool flag3 = this.IsReferenceBookSlotUnlocked(i);
						if (flag3)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060073F9 RID: 29689 RVA: 0x0035F8FC File Offset: 0x0035DAFC
		private bool IsReferenceBookSlotUnlocked(int index)
		{
			return (this._unlockStates >> index & 1) == 1;
		}

		// Token: 0x060073FA RID: 29690 RVA: 0x0035F920 File Offset: 0x0035DB20
		private bool IsAlreadyPlaced(ItemKey bookKey)
		{
			bool flag = this._referenceBooks != null;
			if (flag)
			{
				foreach (ItemKey refBook in this._referenceBooks)
				{
					bool flag2 = refBook.IsValid() && refBook.Id == bookKey.Id;
					if (flag2)
					{
						return true;
					}
				}
			}
			return this._curReadingBook.IsValid() && this._curReadingBook.Id == bookKey.Id;
		}

		// Token: 0x060073FB RID: 29691 RVA: 0x0035F9A9 File Offset: 0x0035DBA9
		public void ShowSelectReferenceBookPanel()
		{
			this.MoveToChooseReferenceForward();
			UIManager.Instance.SetEscHandler(new Action(this.MoveToChooseReferenceBackForward));
		}

		// Token: 0x060073FC RID: 29692 RVA: 0x0035F9CC File Offset: 0x0035DBCC
		public void ShowSelectBookPanel(int lineId = 26, int menuId = 0, int optionIndex = 2)
		{
			this.MoveToChooseBookForward();
			UIManager.Instance.SetEscHandler(new Action(this.MoveToChooseBookBackForward));
			this.selectCombatBookSortAndFilter.OnSectionSelectionChanged(lineId, menuId, optionIndex);
			this.selectLifeBookSortAndFilter.OnSectionSelectionChanged(lineId + 1, menuId, optionIndex);
		}

		// Token: 0x0400562A RID: 22058
		[SerializeField]
		private CToggleGroup skillTypeTogGroup;

		// Token: 0x0400562B RID: 22059
		[SerializeField]
		private SortAndFilter selectCombatBookSortAndFilter;

		// Token: 0x0400562C RID: 22060
		[SerializeField]
		private SortAndFilter selectLifeBookSortAndFilter;

		// Token: 0x0400562D RID: 22061
		[SerializeField]
		private ListStyleGeneralScroll selectBookScroll;

		// Token: 0x0400562E RID: 22062
		[SerializeField]
		private RowItem selectBookRowTemplate;

		// Token: 0x0400562F RID: 22063
		[SerializeField]
		private RowCellContainer selectBookItemIconAndNameCellContainer;

		// Token: 0x04005630 RID: 22064
		[SerializeField]
		private RowCellContainer selectBookSingleTextCellContainer;

		// Token: 0x04005631 RID: 22065
		[SerializeField]
		private RowCellContainer selectBookInfoCellContainer;

		// Token: 0x04005632 RID: 22066
		[SerializeField]
		private ReadingBookIntro bookIntro;

		// Token: 0x04005633 RID: 22067
		[SerializeField]
		private TextMeshProUGUI description;

		// Token: 0x04005634 RID: 22068
		[SerializeField]
		private List<ReadingReferenceBookInfoPrefab> referenceBookList;

		// Token: 0x04005635 RID: 22069
		[SerializeField]
		private ReadingPages readingPages;

		// Token: 0x04005636 RID: 22070
		[SerializeField]
		private ListStyleGeneralScroll selectReferenceBookScroll;

		// Token: 0x04005637 RID: 22071
		[SerializeField]
		private RowItem selectReferenceBookRowTemplate;

		// Token: 0x04005638 RID: 22072
		[SerializeField]
		private RowCellContainer selectReferenceBookItemIconAndNameCellContainer;

		// Token: 0x04005639 RID: 22073
		[SerializeField]
		private RowCellContainer selectReferenceBookSingleTextCellContainer;

		// Token: 0x0400563A RID: 22074
		[SerializeField]
		private RowCellContainer selectReferenceBookInfoCellContainer;

		// Token: 0x0400563B RID: 22075
		[SerializeField]
		private RowCellContainer selectReferenceBookStrategyCellContainer;

		// Token: 0x0400563C RID: 22076
		[SerializeField]
		private CToggleGroup refSkillTypeTogGroup;

		// Token: 0x0400563D RID: 22077
		[SerializeField]
		private SortAndFilter selectCombatReferenceBookSortAndFilter;

		// Token: 0x0400563E RID: 22078
		[SerializeField]
		private SortAndFilter selectLifeReferenceBookSortAndFilter;

		// Token: 0x0400563F RID: 22079
		[SerializeField]
		private CButton readingEventBtn;

		// Token: 0x04005640 RID: 22080
		[SerializeField]
		private GameObject intelligenceRoot;

		// Token: 0x04005641 RID: 22081
		[SerializeField]
		private GameObject durabilityTip;

		// Token: 0x04005642 RID: 22082
		[SerializeField]
		private GameObject readingEventParticleHolder;

		// Token: 0x04005643 RID: 22083
		[SerializeField]
		private TextMeshProUGUI intelligenceCost;

		// Token: 0x04005644 RID: 22084
		[SerializeField]
		private GameObject intelligenceCostHolder;

		// Token: 0x04005645 RID: 22085
		[SerializeField]
		private ReadingInspireHolder readingInspireHolder;

		// Token: 0x04005646 RID: 22086
		[SerializeField]
		private CButton removeCurBookBtn;

		// Token: 0x04005647 RID: 22087
		[SerializeField]
		private RectTransform bookIntroduction;

		// Token: 0x04005648 RID: 22088
		[SerializeField]
		private RectTransform bookParticulars;

		// Token: 0x04005649 RID: 22089
		[SerializeField]
		private RectTransform chooseBook;

		// Token: 0x0400564A RID: 22090
		[SerializeField]
		private RectTransform chooseReference;

		// Token: 0x0400564B RID: 22091
		[SerializeField]
		private RectTransform referenceBooks;

		// Token: 0x0400564C RID: 22092
		[SerializeField]
		private CButton exitChooseBookBtn;

		// Token: 0x0400564D RID: 22093
		[SerializeField]
		private CButton exitChooseReferenceBtn;

		// Token: 0x0400564E RID: 22094
		[SerializeField]
		private CanvasGroup chooseBookGroup;

		// Token: 0x0400564F RID: 22095
		[SerializeField]
		private CanvasGroup chooseReferenceGroup;

		// Token: 0x04005650 RID: 22096
		[SerializeField]
		private GameObject empty;

		// Token: 0x04005651 RID: 22097
		private int _taiwuCharId;

		// Token: 0x04005652 RID: 22098
		private readonly Dictionary<int, ItemDisplayData> _bookDisplayData = new Dictionary<int, ItemDisplayData>();

		// Token: 0x04005653 RID: 22099
		private readonly Dictionary<int, SkillBookPageDisplayData> _pageDisplayData = new Dictionary<int, SkillBookPageDisplayData>();

		// Token: 0x04005654 RID: 22100
		private readonly List<ItemDisplayData> _lifeSkillBooks = new List<ItemDisplayData>();

		// Token: 0x04005655 RID: 22101
		private readonly List<ItemDisplayData> _combatSkillBooks = new List<ItemDisplayData>();

		// Token: 0x04005656 RID: 22102
		private readonly List<int> _allBookIdList = new List<int>();

		// Token: 0x04005657 RID: 22103
		private readonly Dictionary<int, sbyte> _allBookReadingProgressList = new Dictionary<int, sbyte>();

		// Token: 0x04005658 RID: 22104
		private readonly List<ItemKey> _availableBookList = new List<ItemKey>();

		// Token: 0x04005659 RID: 22105
		private readonly List<ItemKey> _availableReferenceBookList = new List<ItemKey>();

		// Token: 0x0400565A RID: 22106
		private readonly Dictionary<ItemKey, int> _availableReferenceBookBonusSpeedDict = new Dictionary<ItemKey, int>();

		// Token: 0x0400565B RID: 22107
		private readonly List<ITradeableContent> _bookFilterSource = new List<ITradeableContent>();

		// Token: 0x0400565C RID: 22108
		private readonly List<ITradeableContent> _referenceBookFilterSource = new List<ITradeableContent>();

		// Token: 0x0400565D RID: 22109
		private byte _receivedBookTypes;

		// Token: 0x0400565E RID: 22110
		private bool _inited;

		// Token: 0x0400565F RID: 22111
		private List<int> _readingEventBookIdList = new List<int>();

		// Token: 0x04005660 RID: 22112
		private ItemKey _curReadingBook;

		// Token: 0x04005661 RID: 22113
		private ItemKey[] _referenceBooks = new ItemKey[]
		{
			ItemKey.Invalid,
			ItemKey.Invalid,
			ItemKey.Invalid
		};

		// Token: 0x04005662 RID: 22114
		private bool _isChangingReferenceBook;

		// Token: 0x04005663 RID: 22115
		private int _selectedReferenceSlotIndex = -1;

		// Token: 0x04005664 RID: 22116
		private ReadingBookStrategies _curStrategies;

		// Token: 0x04005665 RID: 22117
		private IntList _curReadingBookExpireTime;

		// Token: 0x04005666 RID: 22118
		private byte _unlockStates;

		// Token: 0x04005667 RID: 22119
		private bool _hasAvailableSlot;

		// Token: 0x04005668 RID: 22120
		private bool _shouldExecuteUpdateCurPage;

		// Token: 0x04005669 RID: 22121
		private int _curIntelligence;

		// Token: 0x0400566A RID: 22122
		private int _maxIntelligence;

		// Token: 0x0400566B RID: 22123
		private sbyte _readInCombatCount;

		// Token: 0x0400566C RID: 22124
		private sbyte _readInLifeSkillCombatCount;

		// Token: 0x0400566D RID: 22125
		private readonly DialogCmd _bookChangeDialogCmd = new DialogCmd();

		// Token: 0x0400566E RID: 22126
		private const float BookItemColumnMinWidth = 300f;

		// Token: 0x0400566F RID: 22127
		private const float BookItemColumnPreferredWidth = 340f;

		// Token: 0x04005670 RID: 22128
		private const float DurabilityColumnMinWidth = 80f;

		// Token: 0x04005671 RID: 22129
		private const float DurabilityColumnPreferredWidth = 110f;

		// Token: 0x04005672 RID: 22130
		private const float BookInfoColumnMinWidth = 355f;

		// Token: 0x04005673 RID: 22131
		private const float BookInfoColumnPreferredWidth = 355f;

		// Token: 0x04005674 RID: 22132
		private const float EfficiencyColumnMinWidth = 80f;

		// Token: 0x04005675 RID: 22133
		private const float EfficiencyColumnPreferredWidth = 100f;

		// Token: 0x04005676 RID: 22134
		private const float InspirationColumnMinWidth = 80f;

		// Token: 0x04005677 RID: 22135
		private const float InspirationColumnPreferredWidth = 100f;

		// Token: 0x04005678 RID: 22136
		private const float StrategyColumnMinWidth = 100f;

		// Token: 0x04005679 RID: 22137
		private const float StrategyColumnPreferredWidth = 110f;

		// Token: 0x0400567A RID: 22138
		private readonly Dictionary<int, bool[]> _emptyBookPageStates = new Dictionary<int, bool[]>();

		// Token: 0x0400567B RID: 22139
		private readonly List<string> _strategyNameCache = new List<string>();

		// Token: 0x0400567C RID: 22140
		private readonly List<ReadingStrategyTipData> _strategyTipCache = new List<ReadingStrategyTipData>();

		// Token: 0x0400567D RID: 22141
		private ReadingCombatSkillBookSortAndFilterController _selectCombatBookSortAndFilterController;

		// Token: 0x0400567E RID: 22142
		private ReadingLifeSkillBookSortAndFilterController _selectLifeBookSortAndFilterController;

		// Token: 0x0400567F RID: 22143
		private ReadingCombatSkillBookSortAndFilterController _selectCombatReferenceBookSortAndFilterController;

		// Token: 0x04005680 RID: 22144
		private ReadingLifeSkillBookSortAndFilterController _selectLifeReferenceBookSortAndFilterController;

		// Token: 0x04005681 RID: 22145
		private bool _InbookIntroPointerTrigger;

		// Token: 0x04005682 RID: 22146
		private bool _InremoveCurBookBtnPointerTrigger;
	}
}
