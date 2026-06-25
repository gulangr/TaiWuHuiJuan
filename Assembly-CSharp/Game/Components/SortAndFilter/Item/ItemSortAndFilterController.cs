using System;
using System.Collections.Generic;
using System.Diagnostics;
using Game.Views.Select;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D9B RID: 3483
	public class ItemSortAndFilterController : SortAndFilterController<ITradeableContent>
	{
		// Token: 0x14000085 RID: 133
		// (add) Token: 0x0600A90D RID: 43277 RVA: 0x004E2688 File Offset: 0x004E0888
		// (remove) Token: 0x0600A90E RID: 43278 RVA: 0x004E26C0 File Offset: 0x004E08C0
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ESelectItemFilterType> OnFilterTypeChanged;

		// Token: 0x0600A90F RID: 43279 RVA: 0x004E26F5 File Offset: 0x004E08F5
		public ItemSortAndFilterController(ISortAndFilterView sortAndFilter, LanguageKey panelTitleKey = LanguageKey.EventEditor_Error_DuplicateGroupKey) : base(sortAndFilter, panelTitleKey)
		{
			this.SortController = new ItemSortController();
		}

		// Token: 0x17001253 RID: 4691
		// (get) Token: 0x0600A910 RID: 43280 RVA: 0x004E2713 File Offset: 0x004E0913
		public ItemSortController ItemSortController
		{
			get
			{
				return (ItemSortController)this.SortController;
			}
		}

		// Token: 0x0600A911 RID: 43281 RVA: 0x004E2720 File Offset: 0x004E0920
		public ESelectItemFilterType GetCurrentFilterType()
		{
			ESelectItemFilterType result = ESelectItemFilterType.All;
			for (int i = 0; i < base.FilterLines.Count; i++)
			{
				FilterLineBase<ITradeableContent> line = base.FilterLines[i];
				bool flag = base.SortAndFilterState.LineStates == null || base.SortAndFilterState.LineStates.Count <= i || !base.SortAndFilterState.LineStates[i].IsActive;
				if (!flag)
				{
					LineState lineState = base.SortAndFilterState.LineStates[i];
					if (!true)
					{
					}
					ESelectItemFilterType eselectItemFilterType;
					if (!(line is MainFilterLine))
					{
						if (!(line is FoodSecondaryFilterLine) && !(line is FoodVegetarianTypeDetailedFilterLine) && !(line is FoodMeatTypeDetailedFilterLine) && !(line is FoodTeaTypeDetailedFilterLine) && !(line is FoodAlcoholTypeDetailedFilterLine))
						{
							if (!(line is MedicineSecondaryFilterLine) && !(line is MedicinePoisonTypeDetailedFilterLine) && !(line is MedicineBuffTypeDetailedFilterLine))
							{
								if (!(line is WeaponDetailedFilterLine))
								{
									if (!(line is HelmDetailedFilterLine) && !(line is TorsoDetailedFilterLine) && !(line is BracersDetailedFilterLine) && !(line is BootsDetailedFilterLine))
									{
										if (!(line is AccessoryDetailedFilterLine))
										{
											if (!(line is ClothingDetailedFilterLine))
											{
												if (!(line is EquipFilterLine) && !(line is EquipSecondaryFilterLine))
												{
													if (!(line is BookSecondaryFilterLine) && !(line is AllBookDetailedFilterLine) && !(line is CombatSkillBookDetailedFilterLine) && !(line is LifeSkillBookDetailedFilterLine))
													{
														if (!(line is ToolSecondaryFilterLine))
														{
															if (!(line is MaterialSecondaryFilterLine) && !(line is MaterialAdditionalSecondaryFilterLine) && !(line is MaterialFabricDetailedFilterLine) && !(line is MaterialWoodDetailedFilterLine) && !(line is MaterialJadeDetailedFilter) && !(line is MaterialMetalDetailedFilter) && !(line is MaterialHerbDetailedFilter) && !(line is MaterialPoisonDetailedFilter) && !(line is MaterialFoodDetailedFilter))
															{
																if (!(line is MiscSecondaryFilterLine))
																{
																	if (!(line is MiscItemDetailedFilter) && !(line is MiscMiscDetailedFilter) && !(line is MiscOtherDetailedFilter) && !(line is MiscKeyItemDetailedFilter))
																	{
																		eselectItemFilterType = ESelectItemFilterType.All;
																	}
																	else
																	{
																		eselectItemFilterType = ESelectItemFilterType.Misc;
																	}
																}
																else
																{
																	eselectItemFilterType = ItemSortAndFilterController.GetMiscFilterType(lineState);
																}
															}
															else
															{
																eselectItemFilterType = ESelectItemFilterType.Material;
															}
														}
														else
														{
															eselectItemFilterType = ESelectItemFilterType.Tool;
														}
													}
													else
													{
														eselectItemFilterType = ESelectItemFilterType.Book;
													}
												}
												else
												{
													eselectItemFilterType = ItemSortAndFilterController.GetEquipFilterType(lineState);
												}
											}
											else
											{
												eselectItemFilterType = ESelectItemFilterType.EquipmentClothing;
											}
										}
										else
										{
											eselectItemFilterType = ESelectItemFilterType.EquipmentAccessory;
										}
									}
									else
									{
										eselectItemFilterType = ESelectItemFilterType.EquipmentArmor;
									}
								}
								else
								{
									eselectItemFilterType = ESelectItemFilterType.EquipmentWeapon;
								}
							}
							else
							{
								eselectItemFilterType = ESelectItemFilterType.Medicine;
							}
						}
						else
						{
							eselectItemFilterType = ESelectItemFilterType.Food;
						}
					}
					else
					{
						eselectItemFilterType = ESelectItemFilterType.All;
					}
					if (!true)
					{
					}
					ESelectItemFilterType filterType = eselectItemFilterType;
					bool flag2 = filterType > ESelectItemFilterType.All;
					if (flag2)
					{
						result = filterType;
					}
				}
			}
			return result;
		}

		// Token: 0x0600A912 RID: 43282 RVA: 0x004E29B0 File Offset: 0x004E0BB0
		private static ESelectItemFilterType GetMiscFilterType(LineState lineState)
		{
			ToggleKey toggleState = lineState.ToggleGroupState;
			bool isAll = toggleState.IsAll;
			ESelectItemFilterType result;
			if (isAll)
			{
				result = ESelectItemFilterType.Misc;
			}
			else
			{
				bool flag = toggleState.Index == MiscTypeSecondaryMenu.GetSecondaryFilterIndex(EMiscFilterKey.Cricket);
				if (flag)
				{
					result = ESelectItemFilterType.MiscCricket;
				}
				else
				{
					result = ESelectItemFilterType.Misc;
				}
			}
			return result;
		}

		// Token: 0x0600A913 RID: 43283 RVA: 0x004E29F4 File Offset: 0x004E0BF4
		private static ESelectItemFilterType GetEquipFilterType(LineState lineState)
		{
			ToggleKey toggleState = lineState.ToggleGroupState;
			bool isAll = toggleState.IsAll;
			ESelectItemFilterType result;
			if (isAll)
			{
				result = ESelectItemFilterType.Equipment;
			}
			else
			{
				EEquipSubFilterKeys eequipSubFilterKeys = (EEquipSubFilterKeys)toggleState.Index;
				if (!true)
				{
				}
				ESelectItemFilterType eselectItemFilterType;
				if (eequipSubFilterKeys - EEquipSubFilterKeys.Carrier > 2)
				{
					eselectItemFilterType = ESelectItemFilterType.Equipment;
				}
				else
				{
					eselectItemFilterType = ESelectItemFilterType.EquipmentCarrier;
				}
				if (!true)
				{
				}
				result = eselectItemFilterType;
			}
			return result;
		}

		// Token: 0x0600A914 RID: 43284 RVA: 0x004E2A40 File Offset: 0x004E0C40
		protected void NotifyFilterTypeChanged()
		{
			ESelectItemFilterType newFilterType = this.GetCurrentFilterType();
			bool flag = newFilterType != this._currentFilterType;
			if (flag)
			{
				this._currentFilterType = newFilterType;
				Action<ESelectItemFilterType> onFilterTypeChanged = this.OnFilterTypeChanged;
				if (onFilterTypeChanged != null)
				{
					onFilterTypeChanged(this._currentFilterType);
				}
			}
		}

		// Token: 0x0600A915 RID: 43285 RVA: 0x004E2A88 File Offset: 0x004E0C88
		protected override void OnFilterChanged(int lineId)
		{
			bool flag = lineId >= 0;
			if (flag)
			{
				base.SortAndFilter.UpdateLineActive(base.GetLineIndexById(lineId));
			}
			base.UpdateStateFromUI();
			this.NotifyFilterTypeChanged();
			base.InvokeStateChanged();
		}

		// Token: 0x0600A916 RID: 43286 RVA: 0x004E2AC9 File Offset: 0x004E0CC9
		protected override IEnumerable<FilterLineBase<ITradeableContent>> GenerateFilterLines()
		{
			yield return new MainFilterLine();
			yield return new FoodSecondaryFilterLine();
			yield return new MedicineSecondaryFilterLine();
			yield return new EquipSecondaryFilterLine();
			yield return new BookSecondaryFilterLine();
			yield return new ToolSecondaryFilterLine();
			yield return new MaterialSecondaryFilterLine();
			yield return new MaterialAdditionalSecondaryFilterLine();
			yield return new MiscSecondaryFilterLine();
			yield return new FoodVegetarianTypeDetailedFilterLine();
			yield return new FoodMeatTypeDetailedFilterLine();
			yield return new FoodTeaTypeDetailedFilterLine();
			yield return new FoodAlcoholTypeDetailedFilterLine();
			yield return new MedicinePoisonTypeDetailedFilterLine();
			yield return new MedicineBuffTypeDetailedFilterLine();
			yield return new WeaponDetailedFilterLine();
			yield return new HelmDetailedFilterLine();
			yield return new TorsoDetailedFilterLine();
			yield return new BracersDetailedFilterLine();
			yield return new BootsDetailedFilterLine();
			yield return new AccessoryDetailedFilterLine();
			yield return new ClothingDetailedFilterLine();
			yield return new AllBookDetailedFilterLine();
			yield return new CombatSkillBookDetailedFilterLine();
			yield return new LifeSkillBookDetailedFilterLine();
			yield return new MaterialFabricDetailedFilterLine();
			yield return new MaterialWoodDetailedFilterLine();
			yield return new MaterialJadeDetailedFilter();
			yield return new MaterialMetalDetailedFilter();
			yield return new MaterialHerbDetailedFilter();
			yield return new MaterialPoisonDetailedFilter();
			yield return new MaterialFoodDetailedFilter();
			yield return new MiscItemDetailedFilter();
			yield return new MiscMiscDetailedFilter();
			yield return new MiscOtherDetailedFilter();
			yield return new MiscKeyItemDetailedFilter();
			yield break;
		}

		// Token: 0x04008481 RID: 33921
		private ESelectItemFilterType _currentFilterType = ESelectItemFilterType.All;
	}
}
