using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Item;
using Game.Views.Cricket.Combat;
using Game.Views.Select;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.Cricket
{
	// Token: 0x02000AB7 RID: 2743
	public class CricketCombatSelectItemPanel : MonoBehaviour
	{
		// Token: 0x17000ED0 RID: 3792
		// (get) Token: 0x06008683 RID: 34435 RVA: 0x003E9050 File Offset: 0x003E7250
		// (set) Token: 0x06008684 RID: 34436 RVA: 0x003E9058 File Offset: 0x003E7258
		public bool IsShowing { get; private set; }

		// Token: 0x06008685 RID: 34437 RVA: 0x003E9061 File Offset: 0x003E7261
		private void Awake()
		{
			this.switchToggles.Init(1);
			this._isCardMode = true;
			this.switchToggles.OnActiveIndexChange += this.OnActiveIndexChange;
			this.EnsureInitialized();
		}

		// Token: 0x06008686 RID: 34438 RVA: 0x003E9097 File Offset: 0x003E7297
		private void OnEnable()
		{
			this.switchToggles.Set(this._isCardMode ? 1 : 0, true);
		}

		// Token: 0x06008687 RID: 34439 RVA: 0x003E90B4 File Offset: 0x003E72B4
		private void Update()
		{
			bool flag = !this.IsShowing || !this.confirmButton.interactable;
			if (!flag)
			{
				bool flag2 = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return);
				if (flag2)
				{
					this.OnClickConfirm();
				}
			}
		}

		// Token: 0x06008688 RID: 34440 RVA: 0x003E9100 File Offset: 0x003E7300
		private void OnDisable()
		{
			bool lifted = this._lifted;
			if (lifted)
			{
				this.RestoreLiftTargets();
			}
		}

		// Token: 0x06008689 RID: 34441 RVA: 0x003E9120 File Offset: 0x003E7320
		public void Show(IReadOnlyList<ItemDisplayData> data, ItemKey initItemKey, Func<ItemDisplayData, bool> canSelectItem, bool showFairCombatPoint, int usedFairCombatPoint, int maxFairCombatPoint, Func<ItemKey, int> getFairCombatUsedPointByItem, Action<ItemKey> onPreview, Action<ItemKey> onSelect, Action onClose)
		{
			this.EnsureInitialized();
			this.IsShowing = true;
			this._onSelect = onSelect;
			this._onClose = onClose;
			this._onPreview = onPreview;
			this._canSelectItem = canSelectItem;
			this._getFairCombatUsedPointByItem = getFairCombatUsedPointByItem;
			this._currentInitKey = initItemKey;
			this.SetFairCombatPoint(showFairCombatPoint, usedFairCombatPoint, maxFairCombatPoint);
			base.gameObject.SetActive(true);
			this.ApplySourceData(data);
			this.LiftTargets();
			this.Refresh();
		}

		// Token: 0x0600868A RID: 34442 RVA: 0x003E919B File Offset: 0x003E739B
		public void RefreshCandidates(IReadOnlyList<ItemDisplayData> data, ItemKey initItemKey, Func<ItemDisplayData, bool> canSelectItem, bool showFairCombatPoint, int usedFairCombatPoint, int maxFairCombatPoint, Func<ItemKey, int> getFairCombatUsedPointByItem)
		{
			this.EnsureInitialized();
			this._canSelectItem = canSelectItem;
			this._getFairCombatUsedPointByItem = getFairCombatUsedPointByItem;
			this._currentInitKey = initItemKey;
			this.SetFairCombatPoint(showFairCombatPoint, usedFairCombatPoint, maxFairCombatPoint);
			this.ApplySourceData(data);
			this.Refresh();
		}

		// Token: 0x0600868B RID: 34443 RVA: 0x003E91D8 File Offset: 0x003E73D8
		public void Hide(bool notifyClose = true)
		{
			bool flag = !this.IsShowing;
			if (!flag)
			{
				this.IsShowing = false;
				this.RestoreLiftTargets();
				base.gameObject.SetActive(false);
				if (notifyClose)
				{
					Action onClose = this._onClose;
					if (onClose != null)
					{
						onClose();
					}
				}
			}
		}

		// Token: 0x0600868C RID: 34444 RVA: 0x003E9228 File Offset: 0x003E7428
		private void EnsureInitialized()
		{
			bool inited = this._inited;
			if (!inited)
			{
				this.titleLabel.text = LanguageKey.LK_CricketBattle_SelectCricket.Tr();
				this.closeButton.ClearAndAddListener(delegate
				{
					this.Hide(true);
				});
				this.confirmButton.ClearAndAddListener(new Action(this.OnClickConfirm));
				this._sortAndFilterController = new ItemSortAndFilterController(this.sortAndFilter, LanguageKey.EventEditor_Error_DuplicateGroupKey);
				this._sortAndFilterController.Init(new Action(this.Refresh), "CricketCombatSelectItem");
				this._sortAndFilterController.ItemSortController.EnablePriorityCompare = false;
				this._sortAndFilterController.SetToggleVisible(0, new List<int>
				{
					6
				}, false);
				this._sortAndFilterController.SetToggleIsOnWithoutNotify(0, 6);
				this._sortAndFilterController.SetVisibleSortIds(CricketCombatSelectItemPanel.VisibleSortIds);
				this.sortAndFilter.SetEntryButtonForceHidden(true);
				List<ColumnDefinition> columns = new List<ColumnDefinition>
				{
					SelectItemColumnHelper.CreateIconAndNameColumn(),
					CricketCombatSelectItemPanel.CreateCricketAmountColumn(),
					SelectItemColumnHelper.CreateTypeColumn(),
					SelectItemColumnHelper.CreateValueColumn(),
					SelectItemColumnHelper.CreateWeightColumn(),
					SelectItemColumnHelper.CreateCricketDurabilityColumn(),
					SelectItemColumnHelper.CreateCricketHpColumn(),
					SelectItemColumnHelper.CreateCricketSpColumn(),
					SelectItemColumnHelper.CreateCricketVigorColumn(),
					SelectItemColumnHelper.CreateCricketStrengthColumn(),
					SelectItemColumnHelper.CreateCricketBiteColumn(),
					SelectItemColumnHelper.CreateCricketWinsColumn()
				};
				bool isEnabled = CricketFairCombatHelper.IsEnabled;
				if (isEnabled)
				{
					columns.Add(this.CreateFairCombatPointColumn());
				}
				this.rowTemplate.gameObject.SetActive(false);
				this.singleTextCellContainer.gameObject.SetActive(false);
				this.cricketItemIconAndNameCellContainer.gameObject.SetActive(false);
				this.PrepareRowTemplateContainers(columns);
				this.scroll.SetRowTemplate(this.rowTemplate);
				this.scroll.Init<ITradeableContent>(columns, true, null, new Action<int, RowItem>(this.OnRowClicked));
				this.scroll.SetSortController(this._sortAndFilterController);
				this.scroll.RowDisabledProvider = new Func<int, object, bool>(this.IsRowDisabled);
				this.scroll.RowSelectedProvider = delegate(int _, object rowData)
				{
					ItemDisplayData itemData = rowData as ItemDisplayData;
					return itemData != null && CricketCombatSelectItemPanel.IsCricketSelected(itemData);
				};
				this.cardScroll.Init<ITradeableContent>(columns, true, new Action<int, GameObject>(this.OnCardItemRender), new Action<int, RowItem>(this.OnRowClicked));
				this.cardScroll.SetSortController(this._sortAndFilterController);
				this.cardScroll.RowDisabledProvider = new Func<int, object, bool>(this.IsRowDisabled);
				this.cardScroll.RowSelectedProvider = delegate(int _, object rowData)
				{
					ItemDisplayData itemData = rowData as ItemDisplayData;
					return itemData != null && CricketCombatSelectItemPanel.IsCricketSelected(itemData);
				};
				this.RefreshCardMode();
				this._inited = true;
			}
		}

		// Token: 0x0600868D RID: 34445 RVA: 0x003E9507 File Offset: 0x003E7707
		private void SetFairCombatPoint(bool showFairCombatPoint, int usedFairCombatPoint, int maxFairCombatPoint)
		{
			this._showFairCombatPoint = showFairCombatPoint;
			this._usedFairCombatPoint = usedFairCombatPoint;
			this._maxFairCombatPoint = maxFairCombatPoint;
			this.RefreshFairCombatPointUI();
		}

		// Token: 0x0600868E RID: 34446 RVA: 0x003E9528 File Offset: 0x003E7728
		private void RefreshFairCombatPointUI()
		{
			this.fairCombatPointRoot.SetActive(this._showFairCombatPoint);
			bool flag = !this._showFairCombatPoint;
			if (!flag)
			{
				this.fairCombatPointText.text = LanguageKey.LK_CricketCombat_FairCombat_Point.TrFormat(this._usedFairCombatPoint, this._maxFairCombatPoint);
			}
		}

		// Token: 0x0600868F RID: 34447 RVA: 0x003E9584 File Offset: 0x003E7784
		private static bool IsCricketSelected(ItemDisplayData data)
		{
			return CricketCombatKit.Board.SelfCrickets.Any(delegate(ItemDisplayData x)
			{
				ItemKey? itemKey = (x != null) ? new ItemKey?(x.Key) : null;
				ItemKey key = data.Key;
				return itemKey != null && (itemKey == null || itemKey.GetValueOrDefault() == key);
			});
		}

		// Token: 0x06008690 RID: 34448 RVA: 0x003E95C0 File Offset: 0x003E77C0
		private static string GetCricketAmountString(ItemDisplayData data)
		{
			bool isSelected = CricketCombatSelectItemPanel.IsCricketSelected(data);
			bool flag = isSelected;
			string result;
			if (flag)
			{
				result = string.Format("1/{0}", data.Amount);
			}
			else
			{
				result = data.Amount.ToString();
			}
			return result;
		}

		// Token: 0x06008691 RID: 34449 RVA: 0x003E9604 File Offset: 0x003E7804
		private static ColumnDefinition CreateCricketAmountColumn()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 60f,
				FlexibleWidth = 1f,
				PreferredWidth = 80f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Count.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => CricketCombatSelectItemPanel.GetCricketAmountString((ItemDisplayData)data));
			columnDefinition.SortId = 17;
			return columnDefinition;
		}

		// Token: 0x06008692 RID: 34450 RVA: 0x003E96A8 File Offset: 0x003E78A8
		private void PrepareRowTemplateContainers(IEnumerable<ColumnDefinition> columnDefinitions)
		{
			Transform containerRoot = this.rowTemplate.ContainerRoot;
			for (int i = containerRoot.childCount - 1; i >= 0; i--)
			{
				Transform child = containerRoot.GetChild(i);
				bool flag = child.GetComponent<RowCellContainer>() != null;
				if (flag)
				{
					Object.Destroy(child.gameObject);
				}
			}
			foreach (ColumnDefinition columnDef in columnDefinitions)
			{
				RowCellContainer containerTemplate = (columnDef is ColumnDefinition<ITradeableContent, ITradeableContent>) ? this.cricketItemIconAndNameCellContainer : this.singleTextCellContainer;
				RowCellContainer container = Object.Instantiate<RowCellContainer>(containerTemplate, containerRoot);
				container.gameObject.SetActive(true);
			}
		}

		// Token: 0x06008693 RID: 34451 RVA: 0x003E9774 File Offset: 0x003E7974
		private void ApplySourceData(IReadOnlyList<ItemDisplayData> data)
		{
			this._sourceData.Clear();
			bool flag = data == null;
			if (!flag)
			{
				foreach (ItemDisplayData item in data)
				{
					this._sourceData.Add(item);
				}
			}
		}

		// Token: 0x06008694 RID: 34452 RVA: 0x003E97DC File Offset: 0x003E79DC
		private void Refresh()
		{
			this._filteredData.Clear();
			bool flag = this._sourceData.Count == 0;
			if (flag)
			{
				this.scroll.SetData<ITradeableContent>(this._filteredData, -1);
				this.cardScroll.SetData<ITradeableContent>(this._filteredData, -1);
				this._sortAndFilterController.SetFilteredCount(0);
				this.confirmButton.interactable = false;
			}
			else
			{
				this._sortAndFilterController.NotifyDataChanged(this._sourceData);
				Func<ITradeableContent, bool> filter = this._sortAndFilterController.GenerateFilter();
				foreach (ITradeableContent data in this._sourceData)
				{
					bool flag2 = filter == null || filter(data);
					if (flag2)
					{
						this._filteredData.Add(data);
					}
				}
				Comparison<ITradeableContent> comparer = this._sortAndFilterController.GenerateComparer(this._filteredData);
				bool flag3 = comparer != null;
				if (flag3)
				{
					this._filteredData.Sort(comparer);
				}
				this._sortAndFilterController.AfterFilter(this._sourceData);
				int selectedIndex = this._filteredData.FindIndex((ITradeableContent x) => x.RealKey.Equals(this._currentInitKey) && !this.IsRowDisabled(-1, x));
				this.confirmButton.interactable = (this._filteredData.Count > 0);
				bool isCardMode = this._isCardMode;
				if (isCardMode)
				{
					this.cardScroll.SetData<ITradeableContent>(this._filteredData, selectedIndex);
				}
				else
				{
					this.scroll.SetData<ITradeableContent>(this._filteredData, selectedIndex);
				}
			}
		}

		// Token: 0x06008695 RID: 34453 RVA: 0x003E9974 File Offset: 0x003E7B74
		private void OnActiveIndexChange(int newIndex, int oldIndex)
		{
			this._isCardMode = (newIndex == 1);
			this.RefreshCardMode();
			this.Refresh();
		}

		// Token: 0x06008696 RID: 34454 RVA: 0x003E998F File Offset: 0x003E7B8F
		private void RefreshCardMode()
		{
			this.cardScroll.gameObject.SetActive(this._isCardMode);
			this.scroll.gameObject.SetActive(!this._isCardMode);
		}

		// Token: 0x06008697 RID: 34455 RVA: 0x003E99C4 File Offset: 0x003E7BC4
		private void OnCardItemRender(int index, GameObject rowObject)
		{
			bool flag = index < 0 || index >= this._filteredData.Count;
			if (!flag)
			{
				ITradeableContent rowData = this._filteredData[index];
				RowItemMain rowItemMain = rowObject.GetComponentInChildren<RowItemMain>();
				rowItemMain.SetData(rowData);
				CricketCombatSelectableCardItem selectableCardItem = rowObject.GetComponent<CricketCombatSelectableCardItem>();
				selectableCardItem.Set(rowItemMain, true);
			}
		}

		// Token: 0x06008698 RID: 34456 RVA: 0x003E9A1C File Offset: 0x003E7C1C
		private void OnRowClicked(int index, RowItem row)
		{
			bool flag = index < 0 || index >= this._filteredData.Count;
			if (!flag)
			{
				ItemDisplayData data = this._filteredData[index] as ItemDisplayData;
				bool flag2 = data == null || !this.CanSelectItem(data);
				if (!flag2)
				{
					Action<ItemKey> onPreview = this._onPreview;
					if (onPreview != null)
					{
						onPreview(data.Key);
					}
					this.confirmButton.interactable = true;
				}
			}
		}

		// Token: 0x06008699 RID: 34457 RVA: 0x003E9A98 File Offset: 0x003E7C98
		private void UpdateSelectionVisual(int index)
		{
			bool isCardMode = this._isCardMode;
			if (isCardMode)
			{
				this.cardScroll.SetSelectedRow(index);
			}
			else
			{
				this.scroll.SetSelectedRow(index);
			}
		}

		// Token: 0x0600869A RID: 34458 RVA: 0x003E9ACC File Offset: 0x003E7CCC
		private void RefreshFairCombatPointPreview(ItemKey selectedItemKey)
		{
			bool flag = !this._showFairCombatPoint || this._getFairCombatUsedPointByItem == null;
			if (!flag)
			{
				int usedFairCombatPoint = this._getFairCombatUsedPointByItem(selectedItemKey);
				this.SetFairCombatPoint(true, usedFairCombatPoint, this._maxFairCombatPoint);
			}
		}

		// Token: 0x0600869B RID: 34459 RVA: 0x003E9B10 File Offset: 0x003E7D10
		private void OnClickConfirm()
		{
			Action<ItemKey> onSelect = this._onSelect;
			if (onSelect != null)
			{
				onSelect(this._currentInitKey);
			}
		}

		// Token: 0x0600869C RID: 34460 RVA: 0x003E9B2C File Offset: 0x003E7D2C
		private ColumnDefinition CreateFairCombatPointColumn()
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = new LayoutOption(90f, 1f, 120f, 1);
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_CricketCombat_FairCombat_ValuePoint.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => this.GetCricketCost((ItemDisplayData)data).ToString());
			return columnDefinition;
		}

		// Token: 0x0600869D RID: 34461 RVA: 0x003E9B98 File Offset: 0x003E7D98
		private int GetCricketCost(ItemDisplayData itemData)
		{
			return CricketFairCombatHelper.GetCricketCost(itemData);
		}

		// Token: 0x0600869E RID: 34462 RVA: 0x003E9BB0 File Offset: 0x003E7DB0
		private bool IsRowDisabled(int _, object rowData)
		{
			ItemDisplayData itemData = rowData as ItemDisplayData;
			return itemData == null || !this.CanSelectItem(itemData);
		}

		// Token: 0x0600869F RID: 34463 RVA: 0x003E9BDC File Offset: 0x003E7DDC
		private bool CanSelectItem(ItemDisplayData itemData)
		{
			Func<ItemDisplayData, bool> canSelectItem = this._canSelectItem;
			return canSelectItem == null || canSelectItem(itemData);
		}

		// Token: 0x060086A0 RID: 34464 RVA: 0x003E9C04 File Offset: 0x003E7E04
		private void LiftTargets()
		{
			bool flag = this._lifted || this.liftTargets.Length == 0;
			if (!flag)
			{
				this._liftCaches.Clear();
				foreach (GameObject target in this.liftTargets)
				{
					RectTransform rt = (RectTransform)target.transform;
					this._liftCaches.Add(new CricketCombatSelectItemPanel.LiftCache(rt));
					rt.SetParent(this.liftRoot, false);
				}
				this._lifted = true;
			}
		}

		// Token: 0x060086A1 RID: 34465 RVA: 0x003E9C8C File Offset: 0x003E7E8C
		private void RestoreLiftTargets()
		{
			bool flag = !this._lifted;
			if (!flag)
			{
				for (int i = this._liftCaches.Count - 1; i >= 0; i--)
				{
					CricketCombatSelectItemPanel.LiftCache cache = this._liftCaches[i];
					cache.Target.SetParent(cache.Parent, false);
					cache.Target.SetSiblingIndex(cache.SiblingIndex);
					cache.Target.localScale = cache.LocalScale;
					cache.Target.localRotation = cache.LocalRotation;
					cache.Target.anchoredPosition = cache.AnchoredPosition;
				}
				this._liftCaches.Clear();
				this._lifted = false;
			}
		}

		// Token: 0x04006753 RID: 26451
		private static readonly short[] VisibleSortIds = new short[]
		{
			0,
			17,
			5,
			6,
			42,
			46,
			47,
			48,
			49,
			50,
			44
		};

		// Token: 0x04006754 RID: 26452
		[SerializeField]
		private RectTransform liftRoot;

		// Token: 0x04006755 RID: 26453
		[SerializeField]
		private SortAndFilter sortAndFilter;

		// Token: 0x04006756 RID: 26454
		[SerializeField]
		private ListStyleGeneralScroll scroll;

		// Token: 0x04006757 RID: 26455
		[SerializeField]
		private CardStyleGeneralScroll cardScroll;

		// Token: 0x04006758 RID: 26456
		[SerializeField]
		private RowItem rowTemplate;

		// Token: 0x04006759 RID: 26457
		[SerializeField]
		private RowCellContainer singleTextCellContainer;

		// Token: 0x0400675A RID: 26458
		[SerializeField]
		private RowCellContainer cricketItemIconAndNameCellContainer;

		// Token: 0x0400675B RID: 26459
		[SerializeField]
		private TextMeshProUGUI titleLabel;

		// Token: 0x0400675C RID: 26460
		[SerializeField]
		private GameObject fairCombatPointRoot;

		// Token: 0x0400675D RID: 26461
		[SerializeField]
		private TextMeshProUGUI fairCombatPointText;

		// Token: 0x0400675E RID: 26462
		[SerializeField]
		private CButton closeButton;

		// Token: 0x0400675F RID: 26463
		[SerializeField]
		private CButton confirmButton;

		// Token: 0x04006760 RID: 26464
		[SerializeField]
		private CToggleGroup switchToggles;

		// Token: 0x04006761 RID: 26465
		[SerializeField]
		private GameObject[] liftTargets = Array.Empty<GameObject>();

		// Token: 0x04006762 RID: 26466
		private readonly List<ITradeableContent> _sourceData = new List<ITradeableContent>();

		// Token: 0x04006763 RID: 26467
		private readonly List<ITradeableContent> _filteredData = new List<ITradeableContent>();

		// Token: 0x04006764 RID: 26468
		private readonly List<CricketCombatSelectItemPanel.LiftCache> _liftCaches = new List<CricketCombatSelectItemPanel.LiftCache>();

		// Token: 0x04006765 RID: 26469
		private ItemSortAndFilterController _sortAndFilterController;

		// Token: 0x04006766 RID: 26470
		private Action<ItemKey> _onSelect;

		// Token: 0x04006767 RID: 26471
		private Action _onClose;

		// Token: 0x04006768 RID: 26472
		private Func<ItemDisplayData, bool> _canSelectItem;

		// Token: 0x04006769 RID: 26473
		private Func<ItemKey, int> _getFairCombatUsedPointByItem;

		// Token: 0x0400676A RID: 26474
		private Action<ItemKey> _onPreview;

		// Token: 0x0400676B RID: 26475
		private ItemKey _currentInitKey = ItemKey.Invalid;

		// Token: 0x0400676C RID: 26476
		private int _usedFairCombatPoint;

		// Token: 0x0400676D RID: 26477
		private int _maxFairCombatPoint;

		// Token: 0x0400676E RID: 26478
		private bool _inited;

		// Token: 0x0400676F RID: 26479
		private bool _isCardMode;

		// Token: 0x04006770 RID: 26480
		private bool _lifted;

		// Token: 0x04006771 RID: 26481
		private bool _showFairCombatPoint;

		// Token: 0x02002074 RID: 8308
		private readonly struct LiftCache
		{
			// Token: 0x0600F742 RID: 63298 RVA: 0x00628D4C File Offset: 0x00626F4C
			public LiftCache(RectTransform target)
			{
				this.Target = target;
				this.Parent = target.parent;
				this.SiblingIndex = target.GetSiblingIndex();
				this.LocalScale = target.localScale;
				this.LocalRotation = target.localRotation;
				this.AnchoredPosition = target.anchoredPosition;
			}

			// Token: 0x0400D11C RID: 53532
			public readonly RectTransform Target;

			// Token: 0x0400D11D RID: 53533
			public readonly Transform Parent;

			// Token: 0x0400D11E RID: 53534
			public readonly int SiblingIndex;

			// Token: 0x0400D11F RID: 53535
			public readonly Vector3 LocalScale;

			// Token: 0x0400D120 RID: 53536
			public readonly Quaternion LocalRotation;

			// Token: 0x0400D121 RID: 53537
			public readonly Vector2 AnchoredPosition;
		}
	}
}
