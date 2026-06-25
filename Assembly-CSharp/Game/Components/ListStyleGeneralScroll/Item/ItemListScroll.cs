using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll.CellContent;
using Game.Components.ListStyleGeneralScroll.Grouped;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Item.Apply;
using Game.Views;
using Game.Views.Select;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu.ExchangeSystem;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.Item
{
	// Token: 0x02000EAA RID: 3754
	public class ItemListScroll : MonoBehaviour
	{
		// Token: 0x170013B7 RID: 5047
		// (get) Token: 0x0600AE0C RID: 44556 RVA: 0x004F4710 File Offset: 0x004F2910
		public CButton BtnMultiplySelect
		{
			get
			{
				return this.btnMultiplySelect;
			}
		}

		// Token: 0x170013B8 RID: 5048
		// (get) Token: 0x0600AE0D RID: 44557 RVA: 0x004F4718 File Offset: 0x004F2918
		public GameObject ObjMultiplyLockTip
		{
			get
			{
				return this.objMultiplyLockTip;
			}
		}

		// Token: 0x170013B9 RID: 5049
		// (get) Token: 0x0600AE0E RID: 44558 RVA: 0x004F4720 File Offset: 0x004F2920
		public CToggle SwitchSelection
		{
			get
			{
				return this.switchSelection;
			}
		}

		// Token: 0x170013BA RID: 5050
		// (get) Token: 0x0600AE0F RID: 44559 RVA: 0x004F4728 File Offset: 0x004F2928
		public CToggle ToggleMultiplyLock
		{
			get
			{
				return this.toggleMultiplyLock;
			}
		}

		// Token: 0x170013BB RID: 5051
		// (get) Token: 0x0600AE10 RID: 44560 RVA: 0x004F4730 File Offset: 0x004F2930
		public CToggle ToggleSelectAll
		{
			get
			{
				return this.toggleSelectAll;
			}
		}

		// Token: 0x170013BC RID: 5052
		// (get) Token: 0x0600AE11 RID: 44561 RVA: 0x004F4738 File Offset: 0x004F2938
		public RectTransform RectTransform
		{
			get
			{
				return base.transform as RectTransform;
			}
		}

		// Token: 0x170013BD RID: 5053
		// (get) Token: 0x0600AE12 RID: 44562 RVA: 0x004F4745 File Offset: 0x004F2945
		public InfinityScroll InfiniteScroll
		{
			get
			{
				return this._isCardMode ? this.cardScroll.InfiniteScroll : (this.useGroupedScroll ? this.groupedScroll.InfiniteScroll : this.scroll.InfiniteScroll);
			}
		}

		// Token: 0x170013BE RID: 5054
		// (get) Token: 0x0600AE13 RID: 44563 RVA: 0x004F477C File Offset: 0x004F297C
		private IReadOnlyList<object> DataList
		{
			get
			{
				return this._isCardMode ? this.cardScroll.DataList : (this.useGroupedScroll ? this.groupedScroll.DataList : this.scroll.DataList);
			}
		}

		// Token: 0x170013BF RID: 5055
		// (get) Token: 0x0600AE14 RID: 44564 RVA: 0x004F47B3 File Offset: 0x004F29B3
		public RectTransform ViewportRectTransform
		{
			get
			{
				return this.InfiniteScroll.Scroll.Viewport.transform as RectTransform;
			}
		}

		// Token: 0x170013C0 RID: 5056
		// (get) Token: 0x0600AE15 RID: 44565 RVA: 0x004F47CF File Offset: 0x004F29CF
		public ItemListScroll.EColumnType ColumnTypeFlags
		{
			get
			{
				return this._columnTypeFlags;
			}
		}

		// Token: 0x170013C1 RID: 5057
		// (get) Token: 0x0600AE16 RID: 44566 RVA: 0x004F47D7 File Offset: 0x004F29D7
		public SortAndFilterController<ITradeableContent> SortAndFilterController
		{
			get
			{
				return this._sortAndFilterController;
			}
		}

		// Token: 0x170013C2 RID: 5058
		// (get) Token: 0x0600AE17 RID: 44567 RVA: 0x004F47DF File Offset: 0x004F29DF
		public IReadOnlyList<ITradeableContent> FilteredData
		{
			get
			{
				return this._filteredData;
			}
		}

		// Token: 0x0600AE18 RID: 44568 RVA: 0x004F47E8 File Offset: 0x004F29E8
		internal void Init(string sortSaveKey, ESortAndFilterControllerType controllerType = ESortAndFilterControllerType.Item, bool enableRowInteraction = true, Action<ITradeableContent, RowItemLine> onItemRender = null, Action<ITradeableContent, RowItemLine> onItemClick = null, ItemListScroll.EColumnType columnTypeFlags = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability, Dictionary<ItemListScroll.EColumnType, LayoutOption> columnLayoutOptionDict = null, IEnumerable<ColumnDefinition> definitions = null, Action<RowItem> customRowTemplateContainers = null)
		{
			bool inited = this._inited;
			if (!inited)
			{
				this._inited = true;
				this.rowTemplate.gameObject.SetActive(false);
				this.singleTextCellContainer.gameObject.SetActive(false);
				this.itemIconAndNameCellContainer.gameObject.SetActive(false);
				RowCellContainer rowCellContainer = this.bookPageInfoCellContainer;
				if (rowCellContainer != null)
				{
					rowCellContainer.gameObject.SetActive(false);
				}
				RowCellContainer rowCellContainer2 = this.iconAndTextCellContainer;
				if (rowCellContainer2 != null)
				{
					rowCellContainer2.gameObject.SetActive(false);
				}
				RowCellContainer rowCellContainer3 = this.weaponTrickCellContainer;
				if (rowCellContainer3 != null)
				{
					rowCellContainer3.gameObject.SetActive(false);
				}
				RowCellContainer rowCellContainer4 = this.makeCellContainer;
				if (rowCellContainer4 != null)
				{
					rowCellContainer4.gameObject.SetActive(false);
				}
				RowCellContainer rowCellContainer5 = this.poisonInfoCellContainer;
				if (rowCellContainer5 != null)
				{
					rowCellContainer5.gameObject.SetActive(false);
				}
				RowCellContainer rowCellContainer6 = this.refineEffectCellContainer;
				if (rowCellContainer6 != null)
				{
					rowCellContainer6.gameObject.SetActive(false);
				}
				RowCellContainer rowCellContainer7 = this.refineAttributeCellContainer;
				if (rowCellContainer7 != null)
				{
					rowCellContainer7.gameObject.SetActive(false);
				}
				RowCellContainer rowCellContainer8 = this.blankContainer;
				if (rowCellContainer8 != null)
				{
					rowCellContainer8.gameObject.SetActive(false);
				}
				this._onItemRender = onItemRender;
				this._onItemClick = onItemClick;
				this._columnTypeFlags = columnTypeFlags;
				this._columnLayoutOptionDict = columnLayoutOptionDict;
				this._sortAndFilterController = controllerType.GetSortAndFilterController(this.sortAndFilter);
				this._sortAndFilterController.Init(new Action(this.OnSortAndFilterChanged), sortSaveKey);
				this.SetColumnDefinitions(definitions, customRowTemplateContainers);
				bool flag = this.btnSwitchCardMode != null;
				if (flag)
				{
					this.btnSwitchCardMode.Init(this.defaultSwithIndex);
					this.btnSwitchCardMode.OnActiveIndexChange += this.SwitchCardModeToggle;
					this.SwitchCardModeToggle(this.defaultSwithIndex, 0);
				}
				else
				{
					this.RefreshCardMode();
				}
			}
		}

		// Token: 0x0600AE19 RID: 44569 RVA: 0x004F49AA File Offset: 0x004F2BAA
		private void OnSortAndFilterChanged()
		{
			this._selectedIndex = -1;
			Action onSortAndFilterChangedCallback = this.OnSortAndFilterChangedCallback;
			if (onSortAndFilterChangedCallback != null)
			{
				onSortAndFilterChangedCallback();
			}
			this.RefreshList();
		}

		// Token: 0x0600AE1A RID: 44570 RVA: 0x004F49D0 File Offset: 0x004F2BD0
		private void OnItemRender(int index, GameObject item)
		{
			bool flag = this._onItemRender == null;
			if (!flag)
			{
				bool flag2 = this.useGroupedScroll && !this._isCardMode;
				ITradeableContent data;
				if (flag2)
				{
					data = (this.groupedScroll.DataList.GetOrDefault(index) as ITradeableContent);
				}
				else
				{
					data = this._filteredData[index];
				}
				RowItemLine itemView = item.GetComponent<RowItemLine>();
				itemView.SetLocked(data.IsLocked);
				this._onItemRender(data, itemView);
				bool flag3 = itemView is CardItem;
				if (flag3)
				{
					bool flag4 = data.Amount == 0;
					if (flag4)
					{
						RowItemMain rowItemMain = itemView.RowItemMain;
						if (rowItemMain != null)
						{
							rowItemMain.ItemBack.SetCountVisible(false);
						}
					}
					else
					{
						Func<ITradeableContent, string> customAmountDataGenerator = this.CustomAmountDataGenerator;
						string countStr = ((customAmountDataGenerator != null) ? customAmountDataGenerator(data) : null) ?? this.DefaultAmountDataGenerator(data);
						RowItemMain rowItemMain2 = itemView.RowItemMain;
						if (rowItemMain2 != null)
						{
							rowItemMain2.ItemBack.SetCountInfo(countStr, string.Empty);
						}
					}
				}
			}
		}

		// Token: 0x0600AE1B RID: 44571 RVA: 0x004F4AD0 File Offset: 0x004F2CD0
		private void OnItemClick(int index, RowItem item)
		{
			bool flag = this._onItemClick == null;
			if (!flag)
			{
				bool flag2 = this.useGroupedScroll && !this._isCardMode;
				ITradeableContent data;
				if (flag2)
				{
					data = (this.groupedScroll.DataList.GetOrDefault(index) as ITradeableContent);
					this._selectedIndex = ((data != null) ? this._filteredData.IndexOf(data) : -1);
				}
				else
				{
					this._selectedIndex = index;
					data = this._filteredData[index];
				}
				RowItemLine itemView = item as RowItemLine;
				this._onItemClick(data, itemView);
			}
		}

		// Token: 0x0600AE1C RID: 44572 RVA: 0x004F4B64 File Offset: 0x004F2D64
		public void SetColumnDefinitions(IEnumerable<ColumnDefinition> definitions, Action<RowItem> customRowTemplateContainers)
		{
			bool flag = definitions == null || customRowTemplateContainers == null;
			if (flag)
			{
				bool flag2 = definitions != null || customRowTemplateContainers != null;
				if (flag2)
				{
					Debug.LogError("definitions and customRowTemplateContainers should be both null or both non-null");
				}
				this.GenerateColumns(this.GenerateColumnDefinitions());
			}
			else
			{
				this._prepareCustomRowTemplateContainers = customRowTemplateContainers;
				this.GenerateColumns(definitions);
				bool flag3 = this.useGroupedScroll;
				if (flag3)
				{
					this.groupedScroll.ClearInfinityScrollCache();
				}
				else
				{
					this.scroll.ClearInfinityScrollCache();
				}
				this.RefreshList();
			}
		}

		// Token: 0x0600AE1D RID: 44573 RVA: 0x004F4BE4 File Offset: 0x004F2DE4
		internal void SetColumnTypeFlags(ItemListScroll.EColumnType columnTypeFlags)
		{
			this._prepareCustomRowTemplateContainers = null;
			this._columnTypeFlags = columnTypeFlags;
			bool flag = this.useGroupedScroll;
			if (flag)
			{
				this.groupedScroll.ClearInfinityScrollCache();
			}
			else
			{
				this.scroll.ClearInfinityScrollCache();
			}
			this.cardScroll.ClearInfinityScrollCache();
			this.GenerateColumns(this.GenerateColumnDefinitions());
			this.RefreshList();
		}

		// Token: 0x0600AE1E RID: 44574 RVA: 0x004F4C44 File Offset: 0x004F2E44
		private void GenerateColumns(IEnumerable<ColumnDefinition> definitions)
		{
			this.PrepareRowTemplateContainers();
			ColumnDefinition[] defs = definitions.ToArray<ColumnDefinition>();
			bool flag = this.useGroupedScroll;
			if (flag)
			{
				this.groupedScroll.Init<ITradeableContent>(defs, true, new Action<int, GameObject>(this.OnItemRender), new Action<int, RowItem>(this.OnItemClick));
				this.groupedScroll.SetSortController(this._sortAndFilterController);
			}
			else
			{
				this.scroll.Init<ITradeableContent>(defs, true, new Action<int, GameObject>(this.OnItemRender), new Action<int, RowItem>(this.OnItemClick));
				this.scroll.SetSortController(this._sortAndFilterController);
			}
			this.cardScroll.Init<ITradeableContent>(defs, true, new Action<int, GameObject>(this.OnItemRender), new Action<int, RowItem>(this.OnItemClick));
			this.cardScroll.SetSortController(this._sortAndFilterController);
		}

		// Token: 0x0600AE1F RID: 44575 RVA: 0x004F4D18 File Offset: 0x004F2F18
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitions()
		{
			foreach (object obj in Enum.GetValues(typeof(ItemListScroll.EColumnType)))
			{
				ItemListScroll.EColumnType flag = (ItemListScroll.EColumnType)obj;
				bool flag2 = (flag & this._columnTypeFlags) == ~(ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.IconAndNameWithDurability | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability | ItemListScroll.EColumnType.Tame | ItemListScroll.EColumnType.Power | ItemListScroll.EColumnType.SpecialBreakProgress | ItemListScroll.EColumnType.CustomButton | ItemListScroll.EColumnType.SupplyRate | ItemListScroll.EColumnType.Book | ItemListScroll.EColumnType.Wisdom | ItemListScroll.EColumnType.WeaponTrick | ItemListScroll.EColumnType.MakeAttainment | ItemListScroll.EColumnType.MakeTool | ItemListScroll.EColumnType.MakeMaterial | ItemListScroll.EColumnType.PoisonInfo | ItemListScroll.EColumnType.WeaveTemplate | ItemListScroll.EColumnType.WeaveCount | ItemListScroll.EColumnType.ProductRate | ItemListScroll.EColumnType.RefineEffect | ItemListScroll.EColumnType.RefineAttribute | ItemListScroll.EColumnType.RequirVillagerAmount | ItemListScroll.EColumnType.ToolAttainment | ItemListScroll.EColumnType.CricketAge | ItemListScroll.EColumnType.CricketDurability | ItemListScroll.EColumnType.CricketWin | ItemListScroll.EColumnType.CricketLose | ItemListScroll.EColumnType.ShopValue | ItemListScroll.EColumnType.ShopPrice);
				if (!flag2)
				{
					LayoutOption layoutOption = default(LayoutOption);
					Dictionary<ItemListScroll.EColumnType, LayoutOption> columnLayoutOptionDict = this._columnLayoutOptionDict;
					bool hasCustomLayoutOption = columnLayoutOptionDict != null && columnLayoutOptionDict.TryGetValue(flag, out layoutOption);
					ItemListScroll.EColumnType ecolumnType = flag;
					ItemListScroll.EColumnType ecolumnType2 = ecolumnType;
					ItemListScroll.EColumnType ecolumnType3 = ecolumnType2;
					if (ecolumnType3 <= ItemListScroll.EColumnType.MakeAttainment)
					{
						if (ecolumnType3 <= ItemListScroll.EColumnType.Tame)
						{
							if (ecolumnType3 <= ItemListScroll.EColumnType.Type)
							{
								if (ecolumnType3 != ItemListScroll.EColumnType.ShopPrice)
								{
									long num = ecolumnType3 - ItemListScroll.EColumnType.IconAndName;
									if (num <= 3L)
									{
										switch ((uint)num)
										{
										case 0U:
											yield return this.ColumnIconAndName(layoutOption, hasCustomLayoutOption);
											continue;
										case 1U:
											yield return this.ColumnIconAndNameAndDurability(layoutOption, hasCustomLayoutOption);
											continue;
										case 2U:
											continue;
										case 3U:
											yield return this.ColumnAmount(layoutOption, hasCustomLayoutOption);
											continue;
										}
									}
									if (ecolumnType3 == ItemListScroll.EColumnType.Type)
									{
										yield return this.ColumnSubType(layoutOption, hasCustomLayoutOption);
									}
								}
								else
								{
									yield return this.ColumnShopItemPrice(layoutOption, hasCustomLayoutOption);
								}
							}
							else if (ecolumnType3 <= ItemListScroll.EColumnType.Value)
							{
								if (ecolumnType3 != ItemListScroll.EColumnType.Weight)
								{
									if (ecolumnType3 == ItemListScroll.EColumnType.Value)
									{
										yield return this.ColumnValue(layoutOption, hasCustomLayoutOption, LanguageKey.LK_ItemValue);
									}
								}
								else
								{
									yield return this.ColumnWeight(layoutOption, hasCustomLayoutOption);
								}
							}
							else if (ecolumnType3 != ItemListScroll.EColumnType.Durability)
							{
								if (ecolumnType3 == ItemListScroll.EColumnType.Tame)
								{
									yield return this.ColumnTame(layoutOption, hasCustomLayoutOption);
								}
							}
							else
							{
								yield return this.ColumnDurability(layoutOption, hasCustomLayoutOption);
							}
						}
						else if (ecolumnType3 <= ItemListScroll.EColumnType.CustomButton)
						{
							if (ecolumnType3 != ItemListScroll.EColumnType.Power)
							{
								if (ecolumnType3 != ItemListScroll.EColumnType.SpecialBreakProgress)
								{
									if (ecolumnType3 == ItemListScroll.EColumnType.CustomButton)
									{
										yield return this.ColumnCustomButton(layoutOption, hasCustomLayoutOption);
									}
								}
								else
								{
									yield return this.ColumnSpecialBreakProgress(layoutOption, hasCustomLayoutOption);
								}
							}
							else
							{
								yield return this.ColumnPower(layoutOption, hasCustomLayoutOption);
							}
						}
						else if (ecolumnType3 <= ItemListScroll.EColumnType.Wisdom)
						{
							if (ecolumnType3 != ItemListScroll.EColumnType.Book)
							{
								if (ecolumnType3 == ItemListScroll.EColumnType.Wisdom)
								{
									yield return this.ColumnWisdom(layoutOption, hasCustomLayoutOption);
								}
							}
							else
							{
								yield return this.ColumnBook(layoutOption, hasCustomLayoutOption);
							}
						}
						else if (ecolumnType3 != ItemListScroll.EColumnType.WeaponTrick)
						{
							if (ecolumnType3 == ItemListScroll.EColumnType.MakeAttainment)
							{
								yield return this.ColumnMakeAttainment(layoutOption, hasCustomLayoutOption);
							}
						}
						else
						{
							yield return this.ColumnWeaponTrick(layoutOption, hasCustomLayoutOption);
						}
					}
					else if (ecolumnType3 <= ItemListScroll.EColumnType.RefineEffect)
					{
						if (ecolumnType3 <= ItemListScroll.EColumnType.PoisonInfo)
						{
							if (ecolumnType3 != ItemListScroll.EColumnType.MakeTool)
							{
								if (ecolumnType3 != ItemListScroll.EColumnType.MakeMaterial)
								{
									if (ecolumnType3 == ItemListScroll.EColumnType.PoisonInfo)
									{
										yield return this.ColumnPoisonInfo(layoutOption, hasCustomLayoutOption);
									}
								}
								else
								{
									yield return this.ColumnMakeMaterialCount(layoutOption, hasCustomLayoutOption);
								}
							}
							else
							{
								yield return this.ColumnMakeToolCount(layoutOption, hasCustomLayoutOption);
							}
						}
						else if (ecolumnType3 <= ItemListScroll.EColumnType.WeaveCount)
						{
							if (ecolumnType3 != ItemListScroll.EColumnType.WeaveTemplate)
							{
								if (ecolumnType3 == ItemListScroll.EColumnType.WeaveCount)
								{
									yield return this.ColumnWeaveCount(layoutOption, hasCustomLayoutOption);
								}
							}
							else
							{
								yield return this.ColumnWeaveTemplate(layoutOption, hasCustomLayoutOption);
							}
						}
						else if (ecolumnType3 != ItemListScroll.EColumnType.ProductRate)
						{
							if (ecolumnType3 == ItemListScroll.EColumnType.RefineEffect)
							{
								yield return this.ColumnRefineEffect(layoutOption, hasCustomLayoutOption);
							}
						}
						else
						{
							yield return this.ColumnProductRate(layoutOption, hasCustomLayoutOption);
						}
					}
					else if (ecolumnType3 <= ItemListScroll.EColumnType.CricketAge)
					{
						if (ecolumnType3 <= ItemListScroll.EColumnType.RequirVillagerAmount)
						{
							if (ecolumnType3 != ItemListScroll.EColumnType.RefineAttribute)
							{
								if (ecolumnType3 == ItemListScroll.EColumnType.RequirVillagerAmount)
								{
									yield return this.ColumnRequirVillagerAmount(layoutOption, hasCustomLayoutOption);
								}
							}
							else
							{
								yield return this.ColumnRefineAttribute(layoutOption, hasCustomLayoutOption);
							}
						}
						else if (ecolumnType3 != ItemListScroll.EColumnType.ToolAttainment)
						{
							if (ecolumnType3 == ItemListScroll.EColumnType.CricketAge)
							{
								yield return this.ColumnCricketAge(layoutOption, hasCustomLayoutOption);
							}
						}
						else
						{
							yield return this.ColumnToolAttainment(layoutOption, hasCustomLayoutOption);
						}
					}
					else if (ecolumnType3 <= ItemListScroll.EColumnType.CricketWin)
					{
						if (ecolumnType3 != ItemListScroll.EColumnType.CricketDurability)
						{
							if (ecolumnType3 == ItemListScroll.EColumnType.CricketWin)
							{
								yield return this.ColumnCricketWin(layoutOption, hasCustomLayoutOption);
							}
						}
						else
						{
							yield return this.ColumnCricketDurability(layoutOption, hasCustomLayoutOption);
						}
					}
					else if (ecolumnType3 != ItemListScroll.EColumnType.CricketLose)
					{
						if (ecolumnType3 == ItemListScroll.EColumnType.ShopValue)
						{
							yield return this.ColumnShopItemValue(layoutOption, hasCustomLayoutOption);
						}
					}
					else
					{
						yield return this.ColumnCricketLose(layoutOption, hasCustomLayoutOption);
					}
				}
			}
			IEnumerator enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x0600AE20 RID: 44576 RVA: 0x004F4D28 File Offset: 0x004F2F28
		private IEnumerable<ItemListScroll.EColumnType> GenerateColumnTypeFlags()
		{
			foreach (object obj in Enum.GetValues(typeof(ItemListScroll.EColumnType)))
			{
				ItemListScroll.EColumnType flag = (ItemListScroll.EColumnType)obj;
				bool flag2 = (flag & this._columnTypeFlags) == ~(ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.IconAndNameWithDurability | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability | ItemListScroll.EColumnType.Tame | ItemListScroll.EColumnType.Power | ItemListScroll.EColumnType.SpecialBreakProgress | ItemListScroll.EColumnType.CustomButton | ItemListScroll.EColumnType.SupplyRate | ItemListScroll.EColumnType.Book | ItemListScroll.EColumnType.Wisdom | ItemListScroll.EColumnType.WeaponTrick | ItemListScroll.EColumnType.MakeAttainment | ItemListScroll.EColumnType.MakeTool | ItemListScroll.EColumnType.MakeMaterial | ItemListScroll.EColumnType.PoisonInfo | ItemListScroll.EColumnType.WeaveTemplate | ItemListScroll.EColumnType.WeaveCount | ItemListScroll.EColumnType.ProductRate | ItemListScroll.EColumnType.RefineEffect | ItemListScroll.EColumnType.RefineAttribute | ItemListScroll.EColumnType.RequirVillagerAmount | ItemListScroll.EColumnType.ToolAttainment | ItemListScroll.EColumnType.CricketAge | ItemListScroll.EColumnType.CricketDurability | ItemListScroll.EColumnType.CricketWin | ItemListScroll.EColumnType.CricketLose | ItemListScroll.EColumnType.ShopValue | ItemListScroll.EColumnType.ShopPrice);
				if (!flag2)
				{
					yield return flag;
				}
			}
			IEnumerator enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x0600AE21 RID: 44577 RVA: 0x004F4D38 File Offset: 0x004F2F38
		private RowCellContainer GetCellContainerTemplate(ItemListScroll.EColumnType columnType)
		{
			if (!true)
			{
			}
			RowCellContainer result;
			if (columnType <= ItemListScroll.EColumnType.MakeAttainment)
			{
				if (columnType <= ItemListScroll.EColumnType.Book)
				{
					if (columnType - ItemListScroll.EColumnType.IconAndName <= 1L)
					{
						result = this.itemIconAndNameCellContainer;
						goto IL_1C5;
					}
					if (columnType == ItemListScroll.EColumnType.CustomButton)
					{
						result = (this.blankContainer ?? this.singleTextCellContainer);
						goto IL_1C5;
					}
					if (columnType == ItemListScroll.EColumnType.Book)
					{
						result = (this.bookPageInfoCellContainer ?? this.singleTextCellContainer);
						goto IL_1C5;
					}
				}
				else
				{
					if (columnType == ItemListScroll.EColumnType.Wisdom)
					{
						result = (this.iconAndTextCellContainer ?? this.singleTextCellContainer);
						goto IL_1C5;
					}
					if (columnType == ItemListScroll.EColumnType.WeaponTrick)
					{
						result = (this.weaponTrickCellContainer ?? this.singleTextCellContainer);
						goto IL_1C5;
					}
					if (columnType == ItemListScroll.EColumnType.MakeAttainment)
					{
						result = (this.makeCellContainer ?? this.singleTextCellContainer);
						goto IL_1C5;
					}
				}
			}
			else if (columnType <= ItemListScroll.EColumnType.PoisonInfo)
			{
				if (columnType == ItemListScroll.EColumnType.MakeTool)
				{
					result = (this.makeCellContainer ?? this.singleTextCellContainer);
					goto IL_1C5;
				}
				if (columnType == ItemListScroll.EColumnType.MakeMaterial)
				{
					result = (this.makeCellContainer ?? this.singleTextCellContainer);
					goto IL_1C5;
				}
				if (columnType == ItemListScroll.EColumnType.PoisonInfo)
				{
					result = (this.poisonInfoCellContainer ?? this.singleTextCellContainer);
					goto IL_1C5;
				}
			}
			else
			{
				if (columnType == ItemListScroll.EColumnType.RefineEffect)
				{
					result = (this.refineEffectCellContainer ?? this.singleTextCellContainer);
					goto IL_1C5;
				}
				if (columnType == ItemListScroll.EColumnType.RefineAttribute)
				{
					result = (this.refineAttributeCellContainer ?? this.singleTextCellContainer);
					goto IL_1C5;
				}
				if (columnType == ItemListScroll.EColumnType.ToolAttainment)
				{
					result = (this.iconAndTextCellContainer ?? this.singleTextCellContainer);
					goto IL_1C5;
				}
			}
			result = this.singleTextCellContainer;
			IL_1C5:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600AE22 RID: 44578 RVA: 0x004F4F14 File Offset: 0x004F3114
		private void PrepareRowTemplateContainers()
		{
			bool flag = this.rowTemplate == null;
			if (!flag)
			{
				Transform containerRoot = this.rowTemplate.ContainerRoot;
				for (int i = containerRoot.childCount - 1; i >= 0; i--)
				{
					Transform child = containerRoot.GetChild(i);
					bool flag2 = child.GetComponent<RowCellContainer>() != null;
					if (flag2)
					{
						Object.Destroy(child.gameObject);
					}
				}
				bool flag3 = this._prepareCustomRowTemplateContainers != null;
				if (flag3)
				{
					this._prepareCustomRowTemplateContainers(this.rowTemplate);
				}
				else
				{
					foreach (ItemListScroll.EColumnType columnType in this.GenerateColumnTypeFlags())
					{
						this.PrepareRowTemplateContainers(columnType);
					}
				}
			}
		}

		// Token: 0x0600AE23 RID: 44579 RVA: 0x004F4FF8 File Offset: 0x004F31F8
		public void PrepareRowTemplateContainers(ItemListScroll.EColumnType columnType)
		{
			Transform containerRoot = this.rowTemplate.ContainerRoot;
			RowCellContainer containerTemplate = this.GetCellContainerTemplate(columnType);
			RowCellContainer container = Object.Instantiate<RowCellContainer>(containerTemplate, containerRoot);
			container.gameObject.SetActive(true);
		}

		// Token: 0x0600AE24 RID: 44580 RVA: 0x004F5030 File Offset: 0x004F3230
		private void ApplySortAndFilter()
		{
			this._filteredData.Clear();
			bool flag = this._sortAndFilterController == null;
			if (flag)
			{
				bool flag2 = this._originData != null;
				if (flag2)
				{
					this._filteredData.AddRange(this._originData);
				}
				this.RefreshEmpty();
			}
			else
			{
				Func<ITradeableContent, bool> filter = this._sortAndFilterController.GenerateFilter();
				bool flag3 = this._originData == null;
				if (flag3)
				{
					this._sortAndFilterController.SetFilteredCount(0);
					this.RefreshEmpty();
				}
				else
				{
					foreach (ITradeableContent item in this._originData)
					{
						bool flag4 = filter(item);
						if (flag4)
						{
							this._filteredData.Add(item);
						}
					}
					Comparison<ITradeableContent> comparer = this._sortAndFilterController.GenerateComparer(this._filteredData);
					bool flag5 = comparer != null;
					if (flag5)
					{
						this._filteredData.Sort(comparer);
					}
					this._sortAndFilterController.AfterFilter(this._originData);
					this.RefreshEmpty();
				}
			}
		}

		// Token: 0x0600AE25 RID: 44581 RVA: 0x004F515C File Offset: 0x004F335C
		private void RefreshList()
		{
			bool flag = !this._inited;
			if (!flag)
			{
				bool columnTypeFlagsChanged = this._columnTypeFlagsChanged;
				if (columnTypeFlagsChanged)
				{
					this._columnTypeFlagsChanged = false;
					this.PrepareRowTemplateContainers();
					bool flag2 = this.useGroupedScroll;
					if (flag2)
					{
						this.groupedScroll.ClearInfinityScrollCache();
						this.groupedScroll.Init<ITradeableContent>(this.GenerateColumnDefinitions(), true, null, null);
						this.groupedScroll.SetSortController(this._sortAndFilterController);
					}
					else
					{
						this.scroll.ClearInfinityScrollCache();
						this.scroll.Init<ITradeableContent>(this.GenerateColumnDefinitions(), true, null, null);
						this.scroll.SetSortController(this._sortAndFilterController);
					}
					this.cardScroll.ClearInfinityScrollCache();
					this.cardScroll.Init<ITradeableContent>(this.GenerateColumnDefinitions(), true, null, null);
					this.cardScroll.SetSortController(this._sortAndFilterController);
				}
				this.ApplySortAndFilter();
				bool isCardMode = this._isCardMode;
				if (isCardMode)
				{
					this.cardScroll.SetData<ITradeableContent>(this._filteredData, this._selectedIndex);
				}
				else
				{
					bool flag3 = this.useGroupedScroll;
					if (flag3)
					{
						ValueTuple<List<ListStyleGeneralGroupedScroll.Group<ITradeableContent>>, int> valueTuple = this.BuildGroups();
						List<ListStyleGeneralGroupedScroll.Group<ITradeableContent>> groups = valueTuple.Item1;
						int selectedContentIndex = valueTuple.Item2;
						this.groupedScroll.ShowGroupTitles = !this.IsGroupDisabled();
						this.groupedScroll.SetData<ITradeableContent>(groups, selectedContentIndex);
					}
					else
					{
						this.scroll.SetData<ITradeableContent>(this._filteredData, this._selectedIndex);
					}
				}
			}
		}

		// Token: 0x0600AE26 RID: 44582 RVA: 0x004F52D4 File Offset: 0x004F34D4
		[return: TupleElementNames(new string[]
		{
			"groups",
			"selectedContentIndex"
		})]
		private ValueTuple<List<ListStyleGeneralGroupedScroll.Group<ITradeableContent>>, int> BuildGroups()
		{
			this._groups.Clear();
			this._groupMap.Clear();
			this._groupOrder.Clear();
			ITradeableContent selectedData = this._filteredData.CheckIndexReadOnly(this._selectedIndex) ? this._filteredData[this._selectedIndex] : null;
			bool flag = this.IsGroupDisabled();
			ValueTuple<List<ListStyleGeneralGroupedScroll.Group<ITradeableContent>>, int> result;
			if (flag)
			{
				List<ListStyleGeneralGroupedScroll.Group<ITradeableContent>> single = new List<ListStyleGeneralGroupedScroll.Group<ITradeableContent>>
				{
					new ListStyleGeneralGroupedScroll.Group<ITradeableContent>(string.Empty, this._filteredData, true)
				};
				int selectedIndex = (selectedData != null) ? this._filteredData.IndexOf(selectedData) : -1;
				result = new ValueTuple<List<ListStyleGeneralGroupedScroll.Group<ITradeableContent>>, int>(single, selectedIndex);
			}
			else
			{
				bool flag2 = this._customBuildGroup != null;
				if (flag2)
				{
					this._customBuildGroup();
				}
				else
				{
					this.BuildDefaultGroups();
				}
				int selectedContentIndex = this.GetGroupSelectedContentIndex();
				result = new ValueTuple<List<ListStyleGeneralGroupedScroll.Group<ITradeableContent>>, int>(this._groups, selectedContentIndex);
			}
			return result;
		}

		// Token: 0x0600AE27 RID: 44583 RVA: 0x004F53B8 File Offset: 0x004F35B8
		private void BuildDefaultGroups()
		{
			List<LineState> lineStates = this._sortAndFilterController.SortAndFilterState.LineStates;
			int num;
			if (lineStates == null)
			{
				num = -1;
			}
			else
			{
				num = lineStates.FindLastIndex(delegate(LineState s)
				{
					bool result;
					if (s.IsActive)
					{
						ESortAndFilterOneLineType type = s.Type;
						result = (type == ESortAndFilterOneLineType.ToggleGroup || type == ESortAndFilterOneLineType.SingleSelectFilter);
					}
					else
					{
						result = false;
					}
					return result;
				});
			}
			int stateIndex = num;
			bool filterLevelOneAndTwo = stateIndex >= 0;
			bool flag = filterLevelOneAndTwo;
			if (flag)
			{
				List<FilterLineBase<ITradeableContent>> lineList = this._sortAndFilterController.FilterLines;
				FilterLineBase<ITradeableContent> line = lineList[stateIndex];
				bool isLevelOne = line.Id == 0;
				List<ITradeableContent> sourceList = new List<ITradeableContent>(this._filteredData);
				bool flag2 = isLevelOne;
				if (flag2)
				{
					List<ITradeableContent> resourceItemsToRemove = (from d in sourceList
					where d.IsResource
					select d).ToList<ITradeableContent>();
					this._groupMap[0] = resourceItemsToRemove;
					this.AddGroup(0, LanguageKey.LK_CommonSortAndFilter_Filter_Misc_0.Tr(), resourceItemsToRemove, sourceList, true);
					List<FilterToggleConfig> toggleConfigs = line.GenerateConfig().ToggleGroupLineConfig.Config.FilterToggleConfigs;
					for (int index = 0; index < toggleConfigs.Count; index++)
					{
						FilterToggleConfig config = toggleConfigs[index];
						LineState childLineState = new LineState
						{
							IsActive = true,
							Type = ESortAndFilterOneLineType.ToggleGroup,
							ToggleGroupState = new ToggleKey
							{
								Index = index,
								IsAll = false
							}
						};
						List<ITradeableContent> itemsToRemove = (from d in sourceList
						where line.IsDataMatch(d, childLineState)
						select d).ToList<ITradeableContent>();
						this.AddGroup(index, config.TipContent.GetString(), itemsToRemove, sourceList, true);
					}
				}
				else
				{
					DetailedFilterLineLogic<ITradeableContent> secondaryFilterLine = line as DetailedFilterLineLogic<ITradeableContent>;
					DetailedFilterMenuLogic<ITradeableContent> menuLine = (secondaryFilterLine != null) ? secondaryFilterLine.GetMenus().First<DetailedFilterMenuLogic<ITradeableContent>>() : null;
					List<FilterDropdownItemConfig> menuConfigs = menuLine.GetMenuItemConfigs();
					List<int> selections = new List<int>();
					Func<ITradeableContent, bool> <>9__3;
					for (int index2 = 0; index2 < menuConfigs.Count; index2++)
					{
						FilterDropdownItemConfig config2 = menuConfigs[index2];
						selections.Clear();
						selections.Add(index2);
						IEnumerable<ITradeableContent> source = sourceList;
						Func<ITradeableContent, bool> predicate;
						if ((predicate = <>9__3) == null)
						{
							predicate = (<>9__3 = ((ITradeableContent d) => menuLine.IsDataMatch(d, selections)));
						}
						List<ITradeableContent> itemsToRemove2 = source.Where(predicate).ToList<ITradeableContent>();
						this.AddGroup(index2, config2.Text.GetString(), itemsToRemove2, sourceList, true);
					}
				}
			}
		}

		// Token: 0x0600AE28 RID: 44584 RVA: 0x004F5668 File Offset: 0x004F3868
		private int GetGroupSelectedContentIndex()
		{
			ITradeableContent selectedData = this._filteredData.CheckIndexReadOnly(this._selectedIndex) ? this._filteredData[this._selectedIndex] : null;
			int selectedContentIndex = -1;
			bool flag = selectedData != null;
			if (flag)
			{
				int contentIndex = 0;
				foreach (int key in this._groupOrder)
				{
					List<ITradeableContent> list = this._groupMap[key];
					for (int i = 0; i < list.Count; i++)
					{
						bool flag2 = list[i] == selectedData;
						if (flag2)
						{
							selectedContentIndex = contentIndex;
							break;
						}
						contentIndex++;
					}
					bool flag3 = selectedContentIndex >= 0;
					if (flag3)
					{
						break;
					}
				}
			}
			return selectedContentIndex;
		}

		// Token: 0x0600AE29 RID: 44585 RVA: 0x004F5754 File Offset: 0x004F3954
		public void AddGroup(int index, string groupName, List<ITradeableContent> itemsToRemove, List<ITradeableContent> sourceList, bool available = true)
		{
			bool flag = itemsToRemove.Count <= 0;
			if (!flag)
			{
				this._groupMap[index] = itemsToRemove;
				this._groupOrder.Add(index);
				this._groups.Add(new ListStyleGeneralGroupedScroll.Group<ITradeableContent>(groupName, itemsToRemove, available));
				bool flag2 = sourceList != null;
				if (flag2)
				{
					foreach (ITradeableContent item in itemsToRemove)
					{
						sourceList.Remove(item);
					}
				}
			}
		}

		// Token: 0x0600AE2A RID: 44586 RVA: 0x004F57F4 File Offset: 0x004F39F4
		private bool IsGroupDisabled()
		{
			CToggle ctoggle = this.switchSelection;
			bool flag = ctoggle != null && ctoggle.isOn;
			return flag || (!this._keepGroupedOnSort && this.IsSort());
		}

		// Token: 0x0600AE2B RID: 44587 RVA: 0x004F5834 File Offset: 0x004F3A34
		private bool IsSort()
		{
			bool flag = this._sortAndFilterController == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				SortAndFilterState state = this._sortAndFilterController.SortAndFilterState;
				SortStateData sortData = state.SortData;
				bool flag2 = ((sortData != null) ? sortData.ItemStates : null) != null && state.SortData.ItemStates.Count > 0;
				result = flag2;
			}
			return result;
		}

		// Token: 0x0600AE2C RID: 44588 RVA: 0x004F5898 File Offset: 0x004F3A98
		private RowItemLine GetShowingItem(int index, bool realShowing = false)
		{
			GameObject cell = this.InfiniteScroll.GetActiveCell(index);
			RowItemLine rowItemLine;
			if (!this.useGroupedScroll || this._isCardMode)
			{
				rowItemLine = ((cell != null) ? cell.GetComponent<RowItemLine>() : null);
			}
			else
			{
				object obj;
				if (cell == null)
				{
					obj = null;
				}
				else
				{
					GroupedRowWrapper component = cell.GetComponent<GroupedRowWrapper>();
					obj = ((component != null) ? component.ContentRowItem : null);
				}
				rowItemLine = (obj as RowItemLine);
			}
			RowItemLine itemView = rowItemLine;
			bool flag = itemView && realShowing;
			if (flag)
			{
				Rect itemRect = CommonUtils.RectTransToScreenPos(itemView.RectTransform, UIManager.Instance.UiCamera);
				Rect scrollRect = CommonUtils.RectTransToScreenPos(this.InfiniteScroll.Scroll.Viewport, UIManager.Instance.UiCamera);
				bool isOverlap = itemRect.Overlaps(scrollRect);
				bool flag2 = !isOverlap;
				if (flag2)
				{
					return null;
				}
			}
			return itemView;
		}

		// Token: 0x0600AE2D RID: 44589 RVA: 0x004F5958 File Offset: 0x004F3B58
		private void RefreshEmpty()
		{
			bool isEmpty = this._filteredData.Count == 0;
			bool flag = this.emptyObjectArray != null;
			if (flag)
			{
				foreach (GameObject obj in this.emptyObjectArray)
				{
					if (obj != null)
					{
						obj.SetActive(isEmpty);
					}
				}
			}
		}

		// Token: 0x170013C3 RID: 5059
		// (get) Token: 0x0600AE2E RID: 44590 RVA: 0x004F59AF File Offset: 0x004F3BAF
		public bool IsCardMode
		{
			get
			{
				return this._isCardMode;
			}
		}

		// Token: 0x0600AE2F RID: 44591 RVA: 0x004F59B7 File Offset: 0x004F3BB7
		public void SetItemList(IReadOnlyList<ITradeableContent> list)
		{
			this._originData = list;
			this.RefreshList();
		}

		// Token: 0x0600AE30 RID: 44592 RVA: 0x004F59C8 File Offset: 0x004F3BC8
		public void SetItemList(IReadOnlyList<ITradeableContent> list, int selectedIndex)
		{
			this._selectedIndex = selectedIndex;
			this._originData = list;
			this.RefreshList();
		}

		// Token: 0x0600AE31 RID: 44593 RVA: 0x004F59E0 File Offset: 0x004F3BE0
		public void ReRender()
		{
			this.InfiniteScroll.ReRender();
		}

		// Token: 0x0600AE32 RID: 44594 RVA: 0x004F59F0 File Offset: 0x004F3BF0
		public void SetItemToSelectCountMode(RowItemLine itemView, Action<int> onConfirmSetCount, Action onCancelSetCount, int initSelectCount = 0, int limitCount = 0, int minCount = 1, string limitTip = null, bool keepSelectedOnHide = false, Action<int> onCountChange = null, bool isTakeFromTeammate = false)
		{
			this.HighLightItemView(itemView);
			CScrollRect scrollRect = this.InfiniteScroll.Scroll;
			ITradeableContent itemData = itemView.RowItemMain.Data;
			int index = this.DataList.IndexOf(itemData);
			RectTransform itemRectTrans = itemView.transform as RectTransform;
			int maxCount = (limitCount > 0) ? Mathf.Min(limitCount, itemData.Amount) : itemData.Amount;
			bool flag = initSelectCount == 0;
			if (flag)
			{
				initSelectCount = maxCount;
			}
			minCount = Mathf.Clamp(minCount, 1, maxCount);
			initSelectCount = Mathf.Clamp(initSelectCount, minCount, maxCount);
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("MinCount", minCount);
			argBox.Set("MaxCount", maxCount);
			argBox.Set("InitCount", initSelectCount);
			argBox.Set("LimitCount", limitCount);
			bool flag2 = limitCount >= itemData.Amount;
			if (flag2)
			{
				limitTip = string.Empty;
			}
			argBox.Set("LimitTip", limitTip);
			int changeValue = ItemTemplateHelper.GetItemCountUnit(itemData.Key.ItemType, itemData.Key.TemplateId);
			argBox.Set("ChangeValue", changeValue);
			Vector2 followOffset = Vector2.zero;
			argBox.SetObject("FollowOffset", followOffset);
			argBox.SetObject("OnValueChanged", onCountChange);
			argBox.SetObject("OnConfirmSetCount", onConfirmSetCount);
			argBox.SetObject("OnCancelSetCount", onCancelSetCount);
			argBox.SetObject("ItemRectTrans", itemRectTrans);
			argBox.Set("ZeroValid", false);
			if (isTakeFromTeammate)
			{
				argBox.SetObject("TargetItem", itemData.Clone(-1));
			}
			Transform originalParent = itemView.transform.parent;
			UIElement.SetSelectCount.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.SetSelectCount, true);
			UIElement setSelectCount = UIElement.SetSelectCount;
			setSelectCount.OnShowed = (Action)Delegate.Combine(setSelectCount.OnShowed, new Action(delegate()
			{
				scrollRect.SetScrollEnable(false);
				ViewSetSelectCount viewSetSelectCount = UIElement.SetSelectCount.UiBase as ViewSetSelectCount;
				bool flag3 = viewSetSelectCount != null;
				if (flag3)
				{
					viewSetSelectCount.ShowTip();
					viewSetSelectCount.SetTipPosition(itemView.transform as RectTransform);
				}
				itemView.transform.SetParent(UIElement.SetSelectCount.UiBase.transform);
				itemView.SetClickEvent(delegate
				{
					GEvent.OnEvent(UiEvents.OnConfirmSetSelectCount, null);
				});
			}));
			UIElement setSelectCount2 = UIElement.SetSelectCount;
			setSelectCount2.OnHide = (Action)Delegate.Combine(setSelectCount2.OnHide, new Action(delegate()
			{
				scrollRect.SetScrollEnable(true);
				ViewSetSelectCount viewSetSelectCount = UIElement.SetSelectCount.UiBase as ViewSetSelectCount;
				bool flag3 = viewSetSelectCount != null;
				if (flag3)
				{
					viewSetSelectCount.HideTip();
					viewSetSelectCount.ResetTipPosition();
				}
				itemView.transform.SetParent(originalParent);
				this.CancelHighLightItemView();
				this.InfiniteScroll.ReRender();
			}));
		}

		// Token: 0x0600AE33 RID: 44595 RVA: 0x004F5C3C File Offset: 0x004F3E3C
		public void SetItemToPopupMenuMode(RowItemLine itemView, List<ViewPopupMenu.BtnData> btnList, Action onCancel = null, bool isTakeFromTeammate = false)
		{
			CScrollRect scrollRect = this.InfiniteScroll.Scroll;
			this.HighLightItemView(itemView);
			RectTransform itemRectTrans = itemView.transform as RectTransform;
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			Vector3 itemScreenPos = UIManager.Instance.UiCamera.WorldToScreenPoint(itemRectTrans.position);
			Vector3 mouseScreenPos = Input.mousePosition;
			itemScreenPos.x = mouseScreenPos.x;
			argBox.SetObject("BtnInfo", btnList);
			argBox.SetObject("ScreenPos", itemScreenPos);
			argBox.SetObject("ItemSize", itemRectTrans.rect.size);
			argBox.SetObject("OnCancel", onCancel);
			argBox.SetObject("TargetItem", itemView.Data.Clone(-1));
			UIElement popupMenu = UIElement.PopupMenu;
			popupMenu.OnShowed = (Action)Delegate.Combine(popupMenu.OnShowed, new Action(delegate()
			{
				scrollRect.SetScrollEnable(false);
			}));
			UIElement popupMenu2 = UIElement.PopupMenu;
			popupMenu2.OnHide = (Action)Delegate.Combine(popupMenu2.OnHide, new Action(delegate()
			{
				this.CancelHighLightItemView();
				scrollRect.SetScrollEnable(true);
			}));
			UIElement.PopupMenu.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.PopupMenu, true);
		}

		// Token: 0x0600AE34 RID: 44596 RVA: 0x004F5D7C File Offset: 0x004F3F7C
		public void SetItemToSetWeaponInnerRatioMode(RowItemLine itemView)
		{
			CScrollRect scrollRect = this.InfiniteScroll.Scroll;
			this.HighLightItemView(itemView);
			RectTransform itemRectTrans = itemView.GetComponent<RectTransform>();
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.SetObject("ItemRectTrans", itemRectTrans);
			argBox.SetObject("TargetItem", itemView.Data.Clone(-1));
			UIElement setInnerRatio = UIElement.SetInnerRatio;
			setInnerRatio.OnShowed = (Action)Delegate.Combine(setInnerRatio.OnShowed, new Action(delegate()
			{
				scrollRect.SetScrollEnable(false);
			}));
			UIElement setInnerRatio2 = UIElement.SetInnerRatio;
			setInnerRatio2.OnHide = (Action)Delegate.Combine(setInnerRatio2.OnHide, new Action(delegate()
			{
				this.CancelHighLightItemView();
				scrollRect.SetScrollEnable(true);
			}));
			UIElement.SetInnerRatio.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.SetInnerRatio, true);
		}

		// Token: 0x0600AE35 RID: 44597 RVA: 0x004F5E4B File Offset: 0x004F404B
		public void SetScrollEnable(bool canScroll)
		{
			this.InfiniteScroll.Scroll.SetScrollEnable(canScroll);
		}

		// Token: 0x0600AE36 RID: 44598 RVA: 0x004F5E60 File Offset: 0x004F4060
		public void HighLightItemView(RowItem itemView)
		{
			bool flag = null == itemView;
			if (!flag)
			{
				this._focusingTuple.Item1 = itemView;
				this._focusingTuple.Item2 = itemView.transform.parent;
				this._focusingTuple.Item3 = itemView.transform.GetSiblingIndex();
				itemView.transform.SetParent(this.focusItemMask, true);
				itemView.transform.localScale = Vector3.one;
				itemView.SetSelected(true);
				TooltipInvoker itemTip = itemView.TipDisplayer;
				itemTip.enabled = false;
				itemTip.HideTips();
				this.focusItemMask.gameObject.SetActive(true);
			}
		}

		// Token: 0x0600AE37 RID: 44599 RVA: 0x004F5F0C File Offset: 0x004F410C
		public void CancelHighLightItemView()
		{
			bool flag = null == this._focusingTuple.Item1;
			if (!flag)
			{
				this._focusingTuple.Item1.transform.SetParent(this._focusingTuple.Item2, true);
				this._focusingTuple.Item1.transform.SetSiblingIndex(this._focusingTuple.Item3);
				this._focusingTuple.Item1.SetSelected(false);
				this._focusingTuple.Item1.TipDisplayer.enabled = true;
				this._focusingTuple.Item1 = null;
				this.focusItemMask.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600AE38 RID: 44600 RVA: 0x004F5FBE File Offset: 0x004F41BE
		public int FindItemIndex(ITradeableContent itemDisplayData)
		{
			return this.FindItemIndex(itemDisplayData.RealKey);
		}

		// Token: 0x0600AE39 RID: 44601 RVA: 0x004F5FCC File Offset: 0x004F41CC
		public int FindItemIndex(ItemKey key)
		{
			bool flag = this.useGroupedScroll && !this._isCardMode;
			int result;
			if (flag)
			{
				for (int i = 0; i < this.groupedScroll.DataList.Count; i++)
				{
					ITradeableContent itemData = this.groupedScroll.DataList[i] as ITradeableContent;
					bool flag2 = itemData != null && itemData.ContainsItemKey(key);
					if (flag2)
					{
						return this.groupedScroll.FindFlatIndexByContentIndex(i);
					}
				}
				result = -1;
			}
			else
			{
				result = this._filteredData.FindIndex((ITradeableContent data) => data.ContainsItemKey(key));
			}
			return result;
		}

		// Token: 0x0600AE3A RID: 44602 RVA: 0x004F608C File Offset: 0x004F428C
		public RowItemLine FindActiveItem(ItemKey key, bool realShowing = false)
		{
			int index = this.FindItemIndex(key);
			return this.GetShowingItem(index, realShowing);
		}

		// Token: 0x0600AE3B RID: 44603 RVA: 0x004F60B0 File Offset: 0x004F42B0
		public RowItemLine FindActiveItem(ITradeableContent itemDisplayData, bool realShowing = false)
		{
			return this.FindActiveItem(itemDisplayData.RealKey, realShowing);
		}

		// Token: 0x0600AE3C RID: 44604 RVA: 0x004F60D0 File Offset: 0x004F42D0
		public void HandleClickItem(ITradeableContent itemData, RowItemLine itemView, Action<RowItemLine> onClick)
		{
			ItemListScroll.<>c__DisplayClass152_0 CS$<>8__locals1 = new ItemListScroll.<>c__DisplayClass152_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.itemData = itemData;
			CS$<>8__locals1.onClick = onClick;
			int duration = 0;
			Rect itemRect = CommonUtils.RectTransToScreenPos(itemView.RectTransform, UIManager.Instance.UiCamera);
			Rect scrollRect = CommonUtils.RectTransToScreenPos(this.ViewportRectTransform, UIManager.Instance.UiCamera);
			bool flag = !scrollRect.ContainsWithBorder(itemRect.min);
			if (flag)
			{
				this.InfiniteScroll.Scroll.OnScrollEnd = new Action(CS$<>8__locals1.<HandleClickItem>g__FindItemAfterScroll|0);
				Vector2 targetPos = this.InfiniteScroll.Scroll.Content.anchoredPosition + new Vector2(0f, scrollRect.yMin - itemRect.yMin);
				this.InfiniteScroll.Scroll.ScrollTo(targetPos, (float)duration);
			}
			else
			{
				bool flag2 = !scrollRect.ContainsWithBorder(itemRect.max);
				if (flag2)
				{
					this.InfiniteScroll.Scroll.OnScrollEnd = new Action(CS$<>8__locals1.<HandleClickItem>g__FindItemAfterScroll|0);
					Vector2 targetPos2 = this.InfiniteScroll.Scroll.Content.anchoredPosition + new Vector2(0f, scrollRect.yMax - itemRect.yMax);
					this.InfiniteScroll.Scroll.ScrollTo(targetPos2, (float)duration);
				}
				else
				{
					CS$<>8__locals1.onClick(itemView);
				}
			}
		}

		// Token: 0x0600AE3D RID: 44605 RVA: 0x004F623C File Offset: 0x004F443C
		public void Click(ITradeableContent itemData)
		{
			int index = this.FindItemIndex(itemData);
			RowItemLine rowItemLine = this.FindActiveItem(itemData, false);
			bool flag = rowItemLine.OnButtonClicked != null;
			if (flag)
			{
				rowItemLine.OnButtonClicked(index, rowItemLine);
			}
			else
			{
				this.OnItemClick(index, rowItemLine);
			}
		}

		// Token: 0x0600AE3E RID: 44606 RVA: 0x004F6284 File Offset: 0x004F4484
		public void SwitchCardMode()
		{
			this._isCardMode = !this._isCardMode;
			this.RefreshCardMode();
			this.RefreshList();
		}

		// Token: 0x0600AE3F RID: 44607 RVA: 0x004F62A4 File Offset: 0x004F44A4
		public void SwitchCardModeToggle(int newIndex, int oldIndex)
		{
			this._isCardMode = (newIndex == 1);
			this.RefreshCardMode();
			this.RefreshList();
		}

		// Token: 0x0600AE40 RID: 44608 RVA: 0x004F62C0 File Offset: 0x004F44C0
		private void RefreshCardMode()
		{
			this.cardScroll.gameObject.SetActive(this._isCardMode);
			bool flag = this.useGroupedScroll;
			if (flag)
			{
				this.groupedScroll.gameObject.SetActive(!this._isCardMode);
			}
			else
			{
				this.scroll.gameObject.SetActive(!this._isCardMode);
			}
		}

		// Token: 0x0600AE41 RID: 44609 RVA: 0x004F6328 File Offset: 0x004F4528
		public void SetCustomBuildGroup(Action action, bool keepGroupedOnSort = false)
		{
			bool flag = !this.useGroupedScroll;
			if (!flag)
			{
				this._customBuildGroup = action;
				this._keepGroupedOnSort = keepGroupedOnSort;
			}
		}

		// Token: 0x0600AE42 RID: 44610 RVA: 0x004F6354 File Offset: 0x004F4554
		public void SetEmptyContent(string content)
		{
			this._emptyContent = content;
			bool flag = this.emptyObjectArray != null;
			if (flag)
			{
				foreach (GameObject obj in this.emptyObjectArray)
				{
					TextMeshProUGUI text = (obj != null) ? obj.GetComponentInChildren<TextMeshProUGUI>() : null;
					if (text != null)
					{
						text.SetText(content, true);
					}
				}
			}
		}

		// Token: 0x0600AE43 RID: 44611 RVA: 0x004F63B4 File Offset: 0x004F45B4
		public void SetTableHeadSortEnabled(bool enabled)
		{
			this.scroll.SetTableHeadSortEnabled(enabled);
			this.cardScroll.SetTableHeadSortEnabled(enabled);
			bool flag = this.useGroupedScroll;
			if (flag)
			{
				this.groupedScroll.SetTableHeadSortEnabled(enabled);
			}
		}

		// Token: 0x0600AE44 RID: 44612 RVA: 0x004F63F4 File Offset: 0x004F45F4
		public ColumnDefinition<ITradeableContent, ITradeableContent> ColumnIconAndName(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, ITradeableContent> columnDefinition = new ColumnDefinition<ITradeableContent, ITradeableContent>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(200f, 1f, 622f, 1));
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Item.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => data);
			columnDefinition.SortId = 0;
			return columnDefinition;
		}

		// Token: 0x0600AE45 RID: 44613 RVA: 0x004F6478 File Offset: 0x004F4678
		public ColumnDefinition<ITradeableContent, ITradeableContent> ColumnIconAndNameAndDurability(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, ITradeableContent> columnDefinition = new ColumnDefinition<ITradeableContent, ITradeableContent>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(200f, 1f, 622f, 1));
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Name_With_Durability.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => data);
			columnDefinition.SortId = 0;
			return columnDefinition;
		}

		// Token: 0x0600AE46 RID: 44614 RVA: 0x004F64FC File Offset: 0x004F46FC
		public ColumnDefinition<ITradeableContent, string> ColumnAmount(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(200f, 1f, 200f, 1));
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Count.Tr());
			columnDefinition.CellDataGenerator = (this.CustomAmountDataGenerator ?? new Func<ITradeableContent, string>(this.DefaultAmountDataGenerator));
			columnDefinition.SortId = 17;
			return columnDefinition;
		}

		// Token: 0x0600AE47 RID: 44615 RVA: 0x004F6578 File Offset: 0x004F4778
		private string DefaultAmountDataGenerator(ITradeableContent data)
		{
			bool flag = data.Key.IsPrisoner();
			string result;
			if (flag)
			{
				result = "-";
			}
			else
			{
				string maxAmountStr = CommonUtils.GetDisplayStringForNum(data.Amount, 100000);
				bool flag2 = this.SelectedItem == null || this.SelectedItem.Amount <= 0 || !this.SelectedItem.RealKey.Equals(data.RealKey);
				if (flag2)
				{
					result = maxAmountStr;
				}
				else
				{
					string selectedAmountStr = CommonUtils.GetDisplayStringForNum(this.SelectedItem.Amount, 100000);
					result = selectedAmountStr + "/" + maxAmountStr;
				}
			}
			return result;
		}

		// Token: 0x0600AE48 RID: 44616 RVA: 0x004F661A File Offset: 0x004F481A
		[Obsolete("Use ColumnSubType instead")]
		private ColumnDefinition<ITradeableContent, string> ColumnType(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			return this.ColumnSubType(layoutOption, hasCustomLayoutOption);
		}

		// Token: 0x0600AE49 RID: 44617 RVA: 0x004F6624 File Offset: 0x004F4824
		public ColumnDefinition<ITradeableContent, string> ColumnSubType(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(90f, 1f, 122f, 1));
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Type.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => (data.Key.ItemType == -1) ? "-" : LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", ItemTemplateHelper.GetItemSubType(data.Key.ItemType, data.Key.TemplateId))));
			columnDefinition.SortId = 56;
			return columnDefinition;
		}

		// Token: 0x0600AE4A RID: 44618 RVA: 0x004F66AC File Offset: 0x004F48AC
		public ColumnDefinition<ITradeableContent, string> ColumnWeight(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(60f, 1f, 122f, 1));
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Weight.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => data.Key.IsPrisoner() ? "-" : NumberFormatUtils.FormatItemWeight(data.Weight));
			columnDefinition.SortId = 6;
			return columnDefinition;
		}

		// Token: 0x0600AE4B RID: 44619 RVA: 0x004F6730 File Offset: 0x004F4930
		public ColumnDefinition<ITradeableContent, string> ColumnValue(LayoutOption layoutOption, bool hasCustomLayoutOption, LanguageKey titleKey = LanguageKey.LK_ItemValue)
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(60f, 1f, 122f, 1));
			columnDefinition.TableHeadLabel = (() => titleKey.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => (data.CharacterId == -1 && !data.Key.HasTemplate) ? "-" : CommonUtils.GetDisplayStringForNum(data.Value));
			columnDefinition.SortId = 5;
			return columnDefinition;
		}

		// Token: 0x0600AE4C RID: 44620 RVA: 0x004F67B0 File Offset: 0x004F49B0
		public ColumnDefinition<ITradeableContent, string> ColumnDurability(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(120f, 1f, 120f, 1));
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Durability.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => data.DurabilityChange.IsNullOrEmpty() ? CommonUtils.GetDurabilityString(data.Durability, data.MaxDurability) : data.DurabilityChange);
			columnDefinition.SortId = 18;
			return columnDefinition;
		}

		// Token: 0x0600AE4D RID: 44621 RVA: 0x004F6838 File Offset: 0x004F4A38
		public ColumnDefinition<ITradeableContent, string> ColumnTame(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(60f, 1f, 122f, 1));
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Feeding_TamePoint.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => ItemTemplateHelper.HasCarrierTame(data.Key.ItemType, data.Key.TemplateId) ? data.CarrierTamePoint.ToString() : "-");
			columnDefinition.SortId = 41;
			return columnDefinition;
		}

		// Token: 0x0600AE4E RID: 44622 RVA: 0x004F68C0 File Offset: 0x004F4AC0
		public ColumnDefinition<ITradeableContent, string> ColumnPower(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(70f, 1f, 122f, 1));
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_SelectItem_Column_Power.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => (data.PowerInfo.MaxPower > 0) ? string.Format("{0}/{1}", data.PowerInfo.Power, data.PowerInfo.MaxPower) : "-");
			columnDefinition.SortId = 19;
			return columnDefinition;
		}

		// Token: 0x0600AE4F RID: 44623 RVA: 0x004F6948 File Offset: 0x004F4B48
		public ColumnDefinition<ITradeableContent, string> ColumnRequirVillagerAmount(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(60f, 1f, 122f, 1));
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_VillagerNeed_RequiringVillagerAmount.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => (data.PowerInfo.MaxPower > 0) ? string.Format("{0}/{1}", data.PowerInfo.Power, data.PowerInfo.MaxPower) : "-");
			columnDefinition.SortId = 133;
			return columnDefinition;
		}

		// Token: 0x0600AE50 RID: 44624 RVA: 0x004F69D0 File Offset: 0x004F4BD0
		public ColumnDefinition<ITradeableContent, string> ColumnSpecialBreakProgress(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(150f, 1f, 300f, 1));
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_CombatSkill_SpecialBreak_ConvertToExp_Content_ProgressValue.Tr());
			columnDefinition.CellDataGenerator = delegate(ITradeableContent data)
			{
				string format = "{0}%";
				ItemDisplayData itemDisplayData = data as ItemDisplayData;
				return string.Format(format, (((itemDisplayData != null) ? new int?(itemDisplayData.SpecialBreakProgress * 100) : null) / GlobalConfig.Instance.SectStoryEmeiBonusProgressPerCount).GetValueOrDefault());
			};
			columnDefinition.SortId = 131;
			return columnDefinition;
		}

		// Token: 0x0600AE51 RID: 44625 RVA: 0x004F6A58 File Offset: 0x004F4C58
		public ColumnDefinition<ITradeableContent, string> ColumnCustomButton(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(60f, 1f, 140f, 1));
			columnDefinition.TableHeadLabel = (() => string.Empty);
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => string.Empty);
			columnDefinition.SortId = -1;
			return columnDefinition;
		}

		// Token: 0x0600AE52 RID: 44626 RVA: 0x004F6ADC File Offset: 0x004F4CDC
		public ColumnDefinition<ITradeableContent, string> ColumnReplenishRate(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(60f, 1f, 200f, 1));
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Building_Treasury_Replenish_ItemRate.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => data.SpecialArg.ToString() + "%");
			columnDefinition.SortId = 132;
			return columnDefinition;
		}

		// Token: 0x0600AE53 RID: 44627 RVA: 0x004F6B64 File Offset: 0x004F4D64
		public ColumnDefinition<ITradeableContent, BookPageInfoData> ColumnBook(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition progress = SelectItemColumnHelper.CreateBookReadingInfoColumn();
			progress.LayoutOption.MinWidth = 400f;
			return progress as ColumnDefinition<ITradeableContent, BookPageInfoData>;
		}

		// Token: 0x0600AE54 RID: 44628 RVA: 0x004F6B94 File Offset: 0x004F4D94
		public ColumnDefinition<ITradeableContent, IconAndTextCellData> ColumnWisdom(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, IconAndTextCellData> columnDefinition = new ColumnDefinition<ITradeableContent, IconAndTextCellData>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(100f, 1f, 100f, 1));
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Feature_Wisdom.Tr());
			columnDefinition.CellDataGenerator = this.CustomWisdomDataGenerator;
			columnDefinition.SortId = 114;
			return columnDefinition;
		}

		// Token: 0x0600AE55 RID: 44629 RVA: 0x004F6C00 File Offset: 0x004F4E00
		public ColumnDefinition<ITradeableContent, WeaponTrickData> ColumnWeaponTrick(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, WeaponTrickData> columnDefinition = new ColumnDefinition<ITradeableContent, WeaponTrickData>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(400f, 1f, 400f, 1));
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Combat_Trick.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => new WeaponTrickData
			{
				TrickTemplateIdList = data.WeaponTrickList
			});
			columnDefinition.SortId = -1;
			return columnDefinition;
		}

		// Token: 0x0600AE56 RID: 44630 RVA: 0x004F6C84 File Offset: 0x004F4E84
		public ColumnDefinition<ITradeableContent, MakeCellData> ColumnMakeAttainment(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, MakeCellData> columnDefinition = new ColumnDefinition<ITradeableContent, MakeCellData>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(100f, 1f, 100f, 1));
			columnDefinition.TableHeadLabel = (() => SortItem.Instance[156].Names[0]);
			columnDefinition.CellDataGenerator = this.CustomMakeAttainmentDataGenerator;
			columnDefinition.SortId = 156;
			return columnDefinition;
		}

		// Token: 0x0600AE57 RID: 44631 RVA: 0x004F6CF4 File Offset: 0x004F4EF4
		public ColumnDefinition<ITradeableContent, MakeCellData> ColumnMakeToolCount(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, MakeCellData> columnDefinition = new ColumnDefinition<ITradeableContent, MakeCellData>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(100f, 1f, 100f, 1));
			columnDefinition.TableHeadLabel = (() => SortItem.Instance[157].Names[0]);
			columnDefinition.CellDataGenerator = this.CustomMakeToolDataGenerator;
			columnDefinition.SortId = 157;
			return columnDefinition;
		}

		// Token: 0x0600AE58 RID: 44632 RVA: 0x004F6D64 File Offset: 0x004F4F64
		public ColumnDefinition<ITradeableContent, MakeCellData> ColumnMakeMaterialCount(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, MakeCellData> columnDefinition = new ColumnDefinition<ITradeableContent, MakeCellData>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(100f, 1f, 100f, 1));
			columnDefinition.TableHeadLabel = (() => SortItem.Instance[158].Names[0]);
			columnDefinition.CellDataGenerator = this.CustomMakeMaterialDataGenerator;
			columnDefinition.SortId = 158;
			return columnDefinition;
		}

		// Token: 0x0600AE59 RID: 44633 RVA: 0x004F6DD4 File Offset: 0x004F4FD4
		public ColumnDefinition<ITradeableContent, string> ColumnWeaveTemplate(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(150f, 1f, 150f, 1));
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Making_Weave_WeavedClothing_Name.Tr());
			columnDefinition.CellDataGenerator = delegate(ITradeableContent data)
			{
				ItemDisplayData displayData = data as ItemDisplayData;
				return Clothing.Instance[displayData.WeavedClothingTemplateId].Name;
			};
			columnDefinition.SortId = 209;
			return columnDefinition;
		}

		// Token: 0x0600AE5A RID: 44634 RVA: 0x004F6E5C File Offset: 0x004F505C
		public ColumnDefinition<ITradeableContent, string> ColumnWeaveCount(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(100f, 1f, 100f, 1));
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Making_Weave_WeavedClothing_Count.Tr());
			columnDefinition.CellDataGenerator = delegate(ITradeableContent data)
			{
				ItemDisplayData displayData = data as ItemDisplayData;
				return displayData.Amount.ToString();
			};
			columnDefinition.SortId = 210;
			return columnDefinition;
		}

		// Token: 0x0600AE5B RID: 44635 RVA: 0x004F6EE4 File Offset: 0x004F50E4
		public ColumnDefinition<ITradeableContent, ItemDisplayData> ColumnPoisonInfo(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, ItemDisplayData> columnDefinition = new ColumnDefinition<ITradeableContent, ItemDisplayData>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(150f, 1f, 200f, 1));
			columnDefinition.TableHeadLabel = (() => SortItem.Instance[165].Names[0]);
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => (ItemDisplayData)data);
			columnDefinition.SortId = 165;
			return columnDefinition;
		}

		// Token: 0x0600AE5C RID: 44636 RVA: 0x004F6F6C File Offset: 0x004F516C
		public ColumnDefinition<ITradeableContent, ItemDisplayData> ColumnRefineEffect(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, ItemDisplayData> columnDefinition = new ColumnDefinition<ITradeableContent, ItemDisplayData>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(250f, 1f, 280f, 1));
			columnDefinition.TableHeadLabel = (() => SortItem.Instance[167].Names[0]);
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => (ItemDisplayData)data);
			columnDefinition.SortId = 167;
			return columnDefinition;
		}

		// Token: 0x0600AE5D RID: 44637 RVA: 0x004F6FF4 File Offset: 0x004F51F4
		public ColumnDefinition<ITradeableContent, ItemDisplayData> ColumnRefineAttribute(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, ItemDisplayData> columnDefinition = new ColumnDefinition<ITradeableContent, ItemDisplayData>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(250f, 1f, 280f, 1));
			columnDefinition.TableHeadLabel = (() => SortItem.Instance[168].Names[0]);
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => (ItemDisplayData)data);
			columnDefinition.SortId = 168;
			return columnDefinition;
		}

		// Token: 0x0600AE5E RID: 44638 RVA: 0x004F707C File Offset: 0x004F527C
		public ColumnDefinition<ITradeableContent, string> ColumnProductRate(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(200f, 1f, 200f, 1));
			columnDefinition.TableHeadLabel = (() => SortItem.Instance[169].Names[0]);
			columnDefinition.CellDataGenerator = this.CustomProductRateDataGenerator;
			columnDefinition.SortId = 169;
			return columnDefinition;
		}

		// Token: 0x0600AE5F RID: 44639 RVA: 0x004F70EC File Offset: 0x004F52EC
		public ColumnDefinition<ITradeableContent, IconAndTextCellData> ColumnToolAttainment(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, IconAndTextCellData> columnDefinition = new ColumnDefinition<ITradeableContent, IconAndTextCellData>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(100f, 1f, 120f, 1));
			columnDefinition.TableHeadLabel = (() => SortItem.Instance[211].Names[0]);
			columnDefinition.CellDataGenerator = delegate(ITradeableContent d)
			{
				CraftToolItem toolConfig = CraftTool.Instance[d.RealKey.TemplateId];
				LifeSkillTypeItem skillConfig = LifeSkillType.Instance[toolConfig.RequiredLifeSkillTypes.First<sbyte>()];
				return new IconAndTextCellData(skillConfig.Icon, string.Format("+{0}", toolConfig.AttainmentBonus), true, false, false, false);
			};
			columnDefinition.SortId = 211;
			return columnDefinition;
		}

		// Token: 0x0600AE60 RID: 44640 RVA: 0x004F7174 File Offset: 0x004F5374
		public ColumnDefinition<ITradeableContent, string> ColumnCricketAge(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(100f, 1f, 100f, 1));
			columnDefinition.TableHeadLabel = (() => SortItem.Instance[43].Names[0]);
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => string.Format("{0}/{1}", data.CricketData.AgeStr, data.CricketData.MaxAge));
			columnDefinition.SortId = 43;
			return columnDefinition;
		}

		// Token: 0x0600AE61 RID: 44641 RVA: 0x004F71FC File Offset: 0x004F53FC
		public ColumnDefinition<ITradeableContent, string> ColumnCricketDurability(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(100f, 1f, 100f, 1));
			columnDefinition.TableHeadLabel = (() => SortItem.Instance[42].Names[0]);
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => CommonUtils.GetDurabilityString(data.Durability, data.MaxDurability));
			columnDefinition.SortId = 42;
			return columnDefinition;
		}

		// Token: 0x0600AE62 RID: 44642 RVA: 0x004F7284 File Offset: 0x004F5484
		public ColumnDefinition<ITradeableContent, string> ColumnCricketWin(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(100f, 1f, 100f, 1));
			columnDefinition.TableHeadLabel = (() => SortItem.Instance[44].Names[0]);
			columnDefinition.CellDataGenerator = ((ITradeableContent d) => d.CricketData.WinsCount.ToString());
			columnDefinition.SortId = 44;
			return columnDefinition;
		}

		// Token: 0x0600AE63 RID: 44643 RVA: 0x004F730C File Offset: 0x004F550C
		public ColumnDefinition<ITradeableContent, string> ColumnCricketLose(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(100f, 1f, 100f, 1));
			columnDefinition.TableHeadLabel = (() => SortItem.Instance[45].Names[0]);
			columnDefinition.CellDataGenerator = ((ITradeableContent d) => d.CricketData.LossesCount.ToString());
			columnDefinition.SortId = 45;
			return columnDefinition;
		}

		// Token: 0x0600AE64 RID: 44644 RVA: 0x004F7394 File Offset: 0x004F5594
		public ColumnDefinition<ITradeableContent, string> ColumnShopItemValue(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(60f, 1f, 122f, 1));
			columnDefinition.TableHeadLabel = (() => SortItem.Instance[212].Names[0]);
			columnDefinition.CellDataGenerator = delegate(ITradeableContent data)
			{
				bool flag = data.CharacterId == -1 && !data.Key.HasTemplate;
				string result;
				if (flag)
				{
					result = "-";
				}
				else
				{
					CricketData cricketData = data.CricketData;
					int value = (cricketData != null) ? cricketData.CricketValue : ItemTemplateHelper.GetBaseValue(data.RealKey.ItemType, data.RealKey.TemplateId);
					result = CommonUtils.GetDisplayStringForNum(value, 100000);
				}
				return result;
			};
			columnDefinition.SortId = 212;
			return columnDefinition;
		}

		// Token: 0x0600AE65 RID: 44645 RVA: 0x004F741C File Offset: 0x004F561C
		public ColumnDefinition<ITradeableContent, string> ColumnShopItemPrice(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(60f, 1f, 122f, 1));
			columnDefinition.TableHeadLabel = (() => SortItem.Instance[213].Names[0]);
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => (data.CharacterId == -1 && !data.Key.HasTemplate) ? "-" : CommonUtils.GetDisplayStringForNum(data.Value));
			columnDefinition.SortId = 213;
			return columnDefinition;
		}

		// Token: 0x0600AE66 RID: 44646 RVA: 0x004F74A4 File Offset: 0x004F56A4
		public ColumnDefinition<ITradeableContent, string> ColumnGradeName(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(120f, 2f, 242f, 1));
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Main_SummaryInfo_Identity.Tr());
			columnDefinition.CellDataGenerator = delegate(ITradeableContent data)
			{
				bool flag = data.CharacterId == -1;
				string result;
				if (flag)
				{
					result = "-";
				}
				else
				{
					result = CommonUtils.GetOrganizationGradeString(data.OrganizationInfo, data.Gender, data.AvatarRelatedData.DisplayAge, -1);
				}
				return result;
			};
			columnDefinition.SortId = 1;
			return columnDefinition;
		}

		// Token: 0x0600AE67 RID: 44647 RVA: 0x004F7528 File Offset: 0x004F5728
		public ColumnDefinition<ITradeableContent, string> ColumnOrganizationName(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(120f, 2f, 242f, 1));
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Main_SummaryInfo_Organization.Tr());
			columnDefinition.CellDataGenerator = delegate(ITradeableContent data)
			{
				bool flag = data.CharacterId == -1;
				string result;
				if (flag)
				{
					result = "-";
				}
				else
				{
					result = SingletonObject.getInstance<WorldMapModel>().GetSettlementName(data.OrganizationInfo);
				}
				return result;
			};
			columnDefinition.SortId = -1;
			return columnDefinition;
		}

		// Token: 0x0600AE68 RID: 44648 RVA: 0x004F75AC File Offset: 0x004F57AC
		public ColumnDefinition<ITradeableContent, string> ColumnCharacterGrade(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(120f, 2f, 242f, 1));
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Main_SummaryInfo_Identity.Tr());
			columnDefinition.CellDataGenerator = delegate(ITradeableContent data)
			{
				bool flag = data.CharacterId == -1;
				string result;
				if (flag)
				{
					result = "-";
				}
				else
				{
					result = CommonUtils.GetIdentityString(data.OrganizationInfo, data.Gender, data.AvatarRelatedData.DisplayAge, false);
				}
				return result;
			};
			columnDefinition.SortId = 1;
			return columnDefinition;
		}

		// Token: 0x0600AE69 RID: 44649 RVA: 0x004F7630 File Offset: 0x004F5830
		public ColumnDefinition<ITradeableContent, string> ColumnAdvantage(LayoutOption layoutOption, bool hasCustomLayoutOption)
		{
			ColumnDefinition<ITradeableContent, string> columnDefinition = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition.LayoutOption = (hasCustomLayoutOption ? layoutOption : new LayoutOption(60f, 1f, 122f, 1));
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Exchange_Advantage.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => string.Format("{0}%", this.Exchange.CalcValueAdvantage(data.Grade)));
			columnDefinition.SortId = 6;
			return columnDefinition;
		}

		// Token: 0x04008663 RID: 34403
		[SerializeField]
		private ListStyleGeneralScroll scroll;

		// Token: 0x04008664 RID: 34404
		[Header("分组列表")]
		[SerializeField]
		private bool useGroupedScroll;

		// Token: 0x04008665 RID: 34405
		[SerializeField]
		private ListStyleGeneralGroupedScroll groupedScroll;

		// Token: 0x04008666 RID: 34406
		[Header("行模板配置")]
		[SerializeField]
		private RowItem rowTemplate;

		// Token: 0x04008667 RID: 34407
		[SerializeField]
		private RowCellContainer singleTextCellContainer;

		// Token: 0x04008668 RID: 34408
		[SerializeField]
		private RowCellContainer itemIconAndNameCellContainer;

		// Token: 0x04008669 RID: 34409
		[SerializeField]
		private RowCellContainer bookPageInfoCellContainer;

		// Token: 0x0400866A RID: 34410
		[SerializeField]
		private RowCellContainer iconAndTextCellContainer;

		// Token: 0x0400866B RID: 34411
		[SerializeField]
		private RowCellContainer weaponTrickCellContainer;

		// Token: 0x0400866C RID: 34412
		[SerializeField]
		private RowCellContainer makeCellContainer;

		// Token: 0x0400866D RID: 34413
		[SerializeField]
		private RowCellContainer poisonInfoCellContainer;

		// Token: 0x0400866E RID: 34414
		[SerializeField]
		private RowCellContainer refineEffectCellContainer;

		// Token: 0x0400866F RID: 34415
		[SerializeField]
		private RowCellContainer refineAttributeCellContainer;

		// Token: 0x04008670 RID: 34416
		[SerializeField]
		private RowCellContainer blankContainer;

		// Token: 0x04008671 RID: 34417
		[Header("排序筛选")]
		[SerializeField]
		private SortAndFilter sortAndFilter;

		// Token: 0x04008672 RID: 34418
		[SerializeField]
		private RectTransform focusItemMask;

		// Token: 0x04008673 RID: 34419
		[Header("多选相关组件")]
		[SerializeField]
		private CButton btnMultiplySelect;

		// Token: 0x04008674 RID: 34420
		[SerializeField]
		private CToggle switchSelection;

		// Token: 0x04008675 RID: 34421
		[Header("批量锁定相关组件")]
		[SerializeField]
		private CToggle toggleMultiplyLock;

		// Token: 0x04008676 RID: 34422
		[SerializeField]
		private GameObject objMultiplyLockTip;

		// Token: 0x04008677 RID: 34423
		[SerializeField]
		private CToggle toggleSelectAll;

		// Token: 0x04008678 RID: 34424
		[Header("图标卡牌模式相关组件")]
		[SerializeField]
		private CardStyleGeneralScroll cardScroll;

		// Token: 0x04008679 RID: 34425
		[SerializeField]
		private CToggleGroup btnSwitchCardMode;

		// Token: 0x0400867A RID: 34426
		[SerializeField]
		private int defaultSwithIndex = 0;

		// Token: 0x0400867B RID: 34427
		[Header("空状态")]
		[SerializeField]
		private GameObject[] emptyObjectArray;

		// Token: 0x0400867C RID: 34428
		[NonSerialized]
		private ItemListScroll.EColumnType _columnTypeFlags = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability;

		// Token: 0x0400867D RID: 34429
		private bool _columnTypeFlagsChanged;

		// Token: 0x0400867E RID: 34430
		private int _selectedIndex = -1;

		// Token: 0x0400867F RID: 34431
		private SortAndFilterController<ITradeableContent> _sortAndFilterController;

		// Token: 0x04008680 RID: 34432
		private IReadOnlyList<ITradeableContent> _originData;

		// Token: 0x04008681 RID: 34433
		private readonly List<ITradeableContent> _filteredData = new List<ITradeableContent>();

		// Token: 0x04008682 RID: 34434
		private Action<ITradeableContent, RowItemLine> _onItemRender;

		// Token: 0x04008683 RID: 34435
		private Action<ITradeableContent, RowItemLine> _onItemClick;

		// Token: 0x04008684 RID: 34436
		public const ItemListScroll.EColumnType ColumnTypeNormal = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability;

		// Token: 0x04008685 RID: 34437
		public const ItemListScroll.EColumnType ColumnTypeEquipment = ItemListScroll.EColumnType.IconAndNameWithDurability | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Power;

		// Token: 0x04008686 RID: 34438
		public const ItemListScroll.EColumnType ColumnTypeBookNormal = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Book;

		// Token: 0x04008687 RID: 34439
		public const ItemListScroll.EColumnType ColumnTypeCombat = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Wisdom;

		// Token: 0x04008688 RID: 34440
		public const ItemListScroll.EColumnType ColumnTypeCombatRepair = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Durability;

		// Token: 0x04008689 RID: 34441
		public const ItemListScroll.EColumnType ColumnTypeWeaponTrick = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.WeaponTrick;

		// Token: 0x0400868A RID: 34442
		public const ItemListScroll.EColumnType ColumnTypeTool = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Durability | ItemListScroll.EColumnType.ToolAttainment;

		// Token: 0x0400868B RID: 34443
		public const ItemListScroll.EColumnType ColumnTypeRepair = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Durability;

		// Token: 0x0400868C RID: 34444
		public const ItemListScroll.EColumnType ColumnTypeMakeTarget = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.MakeAttainment | ItemListScroll.EColumnType.MakeTool | ItemListScroll.EColumnType.MakeMaterial;

		// Token: 0x0400868D RID: 34445
		public const ItemListScroll.EColumnType ColumnTypePoison = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.PoisonInfo;

		// Token: 0x0400868E RID: 34446
		public const ItemListScroll.EColumnType ColumnTypeRefineEffect = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.RefineEffect;

		// Token: 0x0400868F RID: 34447
		public const ItemListScroll.EColumnType ColumnTypeRefineAttribute = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.RefineAttribute;

		// Token: 0x04008690 RID: 34448
		public const ItemListScroll.EColumnType ColumnTypeNameAmount = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount;

		// Token: 0x04008691 RID: 34449
		public const ItemListScroll.EColumnType ColumnTypeNamePower = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Power;

		// Token: 0x04008692 RID: 34450
		public const ItemListScroll.EColumnType ColumnTypeWeaveTarget = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.WeaveTemplate;

		// Token: 0x04008693 RID: 34451
		public const ItemListScroll.EColumnType ColumnTypeWeaveMaterial = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.WeaveCount;

		// Token: 0x04008694 RID: 34452
		public const ItemListScroll.EColumnType ColumnTypeCraftManProduct = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.ProductRate;

		// Token: 0x04008695 RID: 34453
		public const ItemListScroll.EColumnType ColumnTypeFilterFood = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value;

		// Token: 0x04008696 RID: 34454
		public const ItemListScroll.EColumnType ColumnTypeFilterMedicine = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value;

		// Token: 0x04008697 RID: 34455
		public const ItemListScroll.EColumnType ColumnTypeFilterEquipment = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability | ItemListScroll.EColumnType.Power;

		// Token: 0x04008698 RID: 34456
		public const ItemListScroll.EColumnType ColumnTypeFilterBook = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability | ItemListScroll.EColumnType.Book;

		// Token: 0x04008699 RID: 34457
		public const ItemListScroll.EColumnType ColumnTypeFilterTool = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability | ItemListScroll.EColumnType.ToolAttainment;

		// Token: 0x0400869A RID: 34458
		public const ItemListScroll.EColumnType ColumnTypeFilterMaterial = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value;

		// Token: 0x0400869B RID: 34459
		public const ItemListScroll.EColumnType ColumnTypeFilterMisc = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value;

		// Token: 0x0400869C RID: 34460
		public const ItemListScroll.EColumnType ColumnTypeFilterMiscCricket = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.CricketAge | ItemListScroll.EColumnType.CricketDurability | ItemListScroll.EColumnType.CricketWin | ItemListScroll.EColumnType.CricketLose;

		// Token: 0x0400869D RID: 34461
		public const ItemListScroll.EColumnType ColumnTypeWarehouseNormal = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value;

		// Token: 0x0400869E RID: 34462
		public const ItemListScroll.EColumnType ColumnTypeWarehouseFilterFood = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value;

		// Token: 0x0400869F RID: 34463
		public const ItemListScroll.EColumnType ColumnTypeWarehouseFilterMedicine = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value;

		// Token: 0x040086A0 RID: 34464
		public const ItemListScroll.EColumnType ColumnTypeWarehouseFilterEquipment = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability;

		// Token: 0x040086A1 RID: 34465
		public const ItemListScroll.EColumnType ColumnTypeWarehouseFilterBook = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability;

		// Token: 0x040086A2 RID: 34466
		public const ItemListScroll.EColumnType ColumnTypeWarehouseFilterTool = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability;

		// Token: 0x040086A3 RID: 34467
		public const ItemListScroll.EColumnType ColumnTypeWarehouseFilterMaterial = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value;

		// Token: 0x040086A4 RID: 34468
		public const ItemListScroll.EColumnType ColumnTypeWarehouseFilterMisc = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value;

		// Token: 0x040086A5 RID: 34469
		public const ItemListScroll.EColumnType ColumnTypeWarehouseFilterMiscCricket = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.CricketAge | ItemListScroll.EColumnType.CricketDurability | ItemListScroll.EColumnType.CricketWin | ItemListScroll.EColumnType.CricketLose;

		// Token: 0x040086A6 RID: 34470
		public const ItemListScroll.EColumnType ColumnTypeExchangePerson = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value;

		// Token: 0x040086A7 RID: 34471
		public const ItemListScroll.EColumnType ColumnTypeExchangeSettlement = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value;

		// Token: 0x040086A8 RID: 34472
		public const ItemListScroll.EColumnType ColumnTypeExchangeBook = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability | ItemListScroll.EColumnType.Book;

		// Token: 0x040086A9 RID: 34473
		public const ItemListScroll.EColumnType ColumnTypeExchangeShop = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Durability | ItemListScroll.EColumnType.ShopPrice;

		// Token: 0x040086AA RID: 34474
		private bool _inited = false;

		// Token: 0x040086AB RID: 34475
		[TupleElementNames(new string[]
		{
			"focusingItemView",
			"parent",
			"sibling"
		})]
		private ValueTuple<RowItem, Transform, int> _focusingTuple;

		// Token: 0x040086AC RID: 34476
		private Dictionary<ItemListScroll.EColumnType, LayoutOption> _columnLayoutOptionDict;

		// Token: 0x040086AD RID: 34477
		private readonly List<ListStyleGeneralGroupedScroll.Group<ITradeableContent>> _groups = new List<ListStyleGeneralGroupedScroll.Group<ITradeableContent>>();

		// Token: 0x040086AE RID: 34478
		private readonly Dictionary<int, List<ITradeableContent>> _groupMap = new Dictionary<int, List<ITradeableContent>>();

		// Token: 0x040086AF RID: 34479
		private readonly List<int> _groupOrder = new List<int>();

		// Token: 0x040086B0 RID: 34480
		private bool _isCardMode;

		// Token: 0x040086B1 RID: 34481
		private Action<RowItem> _prepareCustomRowTemplateContainers;

		// Token: 0x040086B2 RID: 34482
		private Action _customBuildGroup;

		// Token: 0x040086B3 RID: 34483
		private bool _keepGroupedOnSort;

		// Token: 0x040086B4 RID: 34484
		private string _emptyContent;

		// Token: 0x040086B5 RID: 34485
		public ITradeableContent SelectedItem;

		// Token: 0x040086B6 RID: 34486
		public Action OnSortAndFilterChangedCallback;

		// Token: 0x040086B7 RID: 34487
		public Func<ITradeableContent, string> CustomAmountDataGenerator;

		// Token: 0x040086B8 RID: 34488
		public Func<ITradeableContent, IconAndTextCellData> CustomWisdomDataGenerator;

		// Token: 0x040086B9 RID: 34489
		public Func<ITradeableContent, MakeCellData> CustomMakeAttainmentDataGenerator;

		// Token: 0x040086BA RID: 34490
		public Func<ITradeableContent, MakeCellData> CustomMakeToolDataGenerator;

		// Token: 0x040086BB RID: 34491
		public Func<ITradeableContent, MakeCellData> CustomMakeMaterialDataGenerator;

		// Token: 0x040086BC RID: 34492
		public Func<ITradeableContent, string> CustomProductRateDataGenerator;

		// Token: 0x040086BD RID: 34493
		public Exchange Exchange;

		// Token: 0x02002519 RID: 9497
		[Flags]
		public enum EColumnType : long
		{
			// Token: 0x0400E6A3 RID: 59043
			IconAndName = 1L,
			// Token: 0x0400E6A4 RID: 59044
			IconAndNameWithDurability = 2L,
			// Token: 0x0400E6A5 RID: 59045
			Amount = 4L,
			// Token: 0x0400E6A6 RID: 59046
			Type = 8L,
			// Token: 0x0400E6A7 RID: 59047
			Weight = 16L,
			// Token: 0x0400E6A8 RID: 59048
			Value = 32L,
			// Token: 0x0400E6A9 RID: 59049
			Durability = 64L,
			// Token: 0x0400E6AA RID: 59050
			Tame = 128L,
			// Token: 0x0400E6AB RID: 59051
			Power = 256L,
			// Token: 0x0400E6AC RID: 59052
			SpecialBreakProgress = 512L,
			// Token: 0x0400E6AD RID: 59053
			CustomButton = 1024L,
			// Token: 0x0400E6AE RID: 59054
			SupplyRate = 2048L,
			// Token: 0x0400E6AF RID: 59055
			Book = 4096L,
			// Token: 0x0400E6B0 RID: 59056
			Wisdom = 8192L,
			// Token: 0x0400E6B1 RID: 59057
			WeaponTrick = 16384L,
			// Token: 0x0400E6B2 RID: 59058
			MakeAttainment = 32768L,
			// Token: 0x0400E6B3 RID: 59059
			MakeTool = 65536L,
			// Token: 0x0400E6B4 RID: 59060
			MakeMaterial = 131072L,
			// Token: 0x0400E6B5 RID: 59061
			PoisonInfo = 262144L,
			// Token: 0x0400E6B6 RID: 59062
			WeaveTemplate = 524288L,
			// Token: 0x0400E6B7 RID: 59063
			WeaveCount = 1048576L,
			// Token: 0x0400E6B8 RID: 59064
			ProductRate = 2097152L,
			// Token: 0x0400E6B9 RID: 59065
			RefineEffect = 4194304L,
			// Token: 0x0400E6BA RID: 59066
			RefineAttribute = 8388608L,
			// Token: 0x0400E6BB RID: 59067
			RequirVillagerAmount = 16777216L,
			// Token: 0x0400E6BC RID: 59068
			ToolAttainment = 33554432L,
			// Token: 0x0400E6BD RID: 59069
			CricketAge = 67108864L,
			// Token: 0x0400E6BE RID: 59070
			CricketDurability = 134217728L,
			// Token: 0x0400E6BF RID: 59071
			CricketWin = 268435456L,
			// Token: 0x0400E6C0 RID: 59072
			CricketLose = 536870912L,
			// Token: 0x0400E6C1 RID: 59073
			ShopValue = 1073741824L,
			// Token: 0x0400E6C2 RID: 59074
			ShopPrice = -2147483648L
		}
	}
}
