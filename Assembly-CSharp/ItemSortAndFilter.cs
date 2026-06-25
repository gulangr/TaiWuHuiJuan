using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Story.SectMainStory;
using TMPro;
using UnityEngine;

// Token: 0x02000351 RID: 849
public class ItemSortAndFilter : Refers
{
	// Token: 0x17000562 RID: 1378
	// (get) Token: 0x0600316F RID: 12655 RVA: 0x001859C2 File Offset: 0x00183BC2
	public IReadOnlyList<ItemDisplayData> ItemList
	{
		get
		{
			return this._itemList;
		}
	}

	// Token: 0x17000563 RID: 1379
	// (get) Token: 0x06003170 RID: 12656 RVA: 0x001859CA File Offset: 0x00183BCA
	// (set) Token: 0x06003171 RID: 12657 RVA: 0x001859D2 File Offset: 0x00183BD2
	public bool IsDetailView { get; private set; }

	// Token: 0x17000564 RID: 1380
	// (get) Token: 0x06003172 RID: 12658 RVA: 0x001859DC File Offset: 0x00183BDC
	public ItemSortAndFilter.ItemFilterType ActiveFilterType
	{
		get
		{
			CToggleObsolete active = this._filterTogGroup.GetActive();
			return (ItemSortAndFilter.ItemFilterType)((active != null) ? new int?(active.Key) : null).Value;
		}
	}

	// Token: 0x06003173 RID: 12659 RVA: 0x00185A18 File Offset: 0x00183C18
	public void SetTargetMixedPoisonMedicine(short medicineId)
	{
		this._poisonTypeList.Clear();
		bool flag = medicineId < 0;
		if (!flag)
		{
			sbyte mixedPoisonType = MixedPoisonType.FromMedicineTemplateId(medicineId);
			this._poisonTypeList.AddRange(MixedPoisonType.ToPoisonTypes[(int)mixedPoisonType]);
		}
	}

	// Token: 0x06003174 RID: 12660 RVA: 0x00185A56 File Offset: 0x00183C56
	public void SetSpecialBreakSelectedBonusId(short bonusId)
	{
		this._specialBreakSelectedBonusId = bonusId;
		this.IsOnSpecialBreakMultiplySelect = true;
	}

	// Token: 0x06003175 RID: 12661 RVA: 0x00185A68 File Offset: 0x00183C68
	private void Awake()
	{
		bool flag = this.UserFloat <= 0f;
		if (flag)
		{
			this.UserFloat = 0.5f;
		}
		this.Init();
	}

	// Token: 0x06003176 RID: 12662 RVA: 0x00185A9C File Offset: 0x00183C9C
	private void OnDisable()
	{
		this.StaticAheadItemKeysList.Clear();
		this._disableStyleViewSwitch = false;
	}

	// Token: 0x06003177 RID: 12663 RVA: 0x00185AB4 File Offset: 0x00183CB4
	public void Init()
	{
		bool inited = this._inited;
		if (!inited)
		{
			this.SortFilterSetting = new ItemSortFilterSetting();
			this._sortBtnHolder = base.CGet<RectTransform>("SortTypeHolder");
			for (int i = 0; i < this._sortBtnHolder.childCount; i++)
			{
				ItemSortAndFilter.SortType type = (ItemSortAndFilter.SortType)i;
				CButtonObsolete sortBtn = this._sortBtnHolder.GetChild(i).GetComponent<CButtonObsolete>();
				sortBtn.ClearAndAddListener(delegate
				{
					this.OnClickSortType(sortBtn, type);
				});
			}
			this._filterTogGroup = base.CGet<CToggleGroupObsolete>("Filter");
			this._filterTogGroup.InitPreOnToggle(-1);
			this._filterTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnItemFilterTogChange);
			this._equipFilterTogGroup = base.CGet<CToggleGroupObsolete>("EquipTypeFilter");
			this._equipFilterTogGroup.InitPreOnToggle(-1);
			this._equipFilterTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnEquipFilterTogChange);
			this._itemSortTogGroup = base.CGet<CToggleGroupObsolete>("ItemSort");
			this._itemSortTogGroup.InitPreOnToggle(-1);
			this._itemSortTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnItemSortTogChange);
			bool flag = this.CTryGet<CToggleGroupObsolete>("MedicineTypeFilter", out this._medicineFilterTogGroup);
			if (flag)
			{
				this._medicineFilterTogGroup.InitPreOnToggle(-1);
				this._medicineFilterTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnMedicineFilterTogChange);
			}
			bool flag2 = this.CTryGet<CToggleGroupObsolete>("SpecialBreakTypeFilter", out this._specialBreakFilterTogGroup);
			if (flag2)
			{
				this._specialBreakFilterTogGroup.InitPreOnToggle(-1);
				this._specialBreakFilterTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnSpecialBreakFilterTogChange);
			}
			bool flag3 = this.CTryGet<CToggleGroupObsolete>("ClothingWeaveTypeFilter", out this._clothingWeaveFilterTogGroup);
			if (flag3)
			{
				this._clothingWeaveFilterTogGroup.InitPreOnToggle(-1);
				this._clothingWeaveFilterTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnClothingWeaveFilterTogChange);
			}
			bool flag4 = this.CTryGet<CToggleGroupObsolete>("PoisonTypeFilter", out this._poisonFilterTogGroup);
			if (flag4)
			{
				this._poisonFilterTogGroup.InitPreOnToggle(-1);
				this._poisonFilterTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnPoisonFilterTogChange);
			}
			bool flag5 = this.CTryGet<CToggleGroupObsolete>("EntertainTypeFilter", out this._entertainFilterTogGroup);
			if (flag5)
			{
				this._entertainFilterTogGroup.InitPreOnToggle(-1);
				this._entertainFilterTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnEntertainFilterTogChange);
			}
			bool flag6 = this.CTryGet<CToggleGroupObsolete>("BookTypeFilter", out this._bookFilterTogGroup);
			if (flag6)
			{
				this._bookFilterTogGroup.InitPreOnToggle(-1);
				this._bookFilterTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnBookFilterTogChange);
			}
			bool flag7 = this.CTryGet<CToggleGroupObsolete>("MaterialTypeFilter", out this._materialFilterTogGroup);
			if (flag7)
			{
				this._materialFilterTogGroup.InitPreOnToggle(-1);
				this._materialFilterTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnMaterialFilterTogChange);
			}
			this._inited = true;
		}
	}

	// Token: 0x06003178 RID: 12664 RVA: 0x00185D9D File Offset: 0x00183F9D
	public void InitIsDetailView(bool isDetailView)
	{
		this.IsDetailView = isDetailView;
		this._itemSortTogGroup.Set(this.IsDetailView ? 0 : 1, true, false);
	}

	// Token: 0x06003179 RID: 12665 RVA: 0x00185DC4 File Offset: 0x00183FC4
	public void SetItemList(ref List<ItemDisplayData> itemList, bool reset = false, string listTag = null, bool detailView = false, Action onItemListChanged = null, Action onViewTypeChanged = null)
	{
		this._itemList = itemList;
		if (reset)
		{
			this.IsDetailView = detailView;
			this._itemSortTogGroup.Set(this.IsDetailView ? 0 : 1, true, false);
			this._onItemListChanged = onItemListChanged;
			this._onViewTypeChanged = onViewTypeChanged;
			this.SortFilterSetting = SingletonObject.getInstance<GameSort>().GetItemSortConfig(listTag);
			bool flag = this._filterTogGroup && this._filterTogGroup.gameObject.activeSelf;
			if (flag)
			{
				this._filterTogInitializing = true;
				bool flag2 = this.SortFilterSetting.ItemFilterType.Count == 1;
				if (flag2)
				{
					this._filterTogGroup.Set((int)this.SortFilterSetting.ItemFilterType[0], true, true);
				}
				else
				{
					this._filterTogGroup.Set(0, true, true);
				}
				this._filterTogInitializing = false;
			}
			else
			{
				bool flag3 = this._equipFilterTogGroup && this._equipFilterTogGroup.gameObject.activeSelf;
				if (flag3)
				{
					this._equipTogInitializing = true;
					bool flag4 = this.SortFilterSetting.EquipFilterType.Count == 1;
					if (flag4)
					{
						this._equipFilterTogGroup.Set((int)this.SortFilterSetting.EquipFilterType[0], true, true);
					}
					else
					{
						this._equipFilterTogGroup.Set(0, true, true);
					}
					this._equipTogInitializing = false;
				}
				else
				{
					bool flag5 = this._medicineFilterTogGroup && this._medicineFilterTogGroup.gameObject.activeSelf;
					if (flag5)
					{
						this._medicineTogInitializing = true;
						bool flag6 = this.SortFilterSetting.MedicineFilterType.Count == 1;
						if (flag6)
						{
							this._medicineFilterTogGroup.Set((int)this.SortFilterSetting.MedicineFilterType[0], true, true);
						}
						else
						{
							this._medicineFilterTogGroup.Set(0, true, true);
						}
						this._medicineTogInitializing = false;
					}
					else
					{
						bool flag7 = this._specialBreakFilterTogGroup && this._specialBreakFilterTogGroup.gameObject.activeSelf;
						if (flag7)
						{
							this._specialBreakTogInitializing = true;
							bool flag8 = this.SortFilterSetting.SpecialBreakFilterType.Count == 1;
							if (flag8)
							{
								this._specialBreakFilterTogGroup.Set((int)this.SortFilterSetting.SpecialBreakFilterType[0], true, true);
							}
							else
							{
								this._specialBreakFilterTogGroup.Set(0, true, true);
							}
							this._specialBreakTogInitializing = false;
						}
						else
						{
							bool flag9 = this._clothingWeaveFilterTogGroup && this._clothingWeaveFilterTogGroup.gameObject.activeSelf;
							if (flag9)
							{
								this._clothingWeaveTogInitializing = true;
								bool flag10 = this.SortFilterSetting.ClothingWeaverFilterType.Count == 1;
								if (flag10)
								{
									this._clothingWeaveFilterTogGroup.Set((int)this.SortFilterSetting.ClothingWeaverFilterType[0], true, true);
								}
								else
								{
									this._clothingWeaveFilterTogGroup.Set(0, true, true);
								}
								this._clothingWeaveTogInitializing = false;
							}
							else
							{
								bool flag11 = this._poisonFilterTogGroup && this._poisonFilterTogGroup.gameObject.activeSelf;
								if (flag11)
								{
									this._poisonTogInitializing = true;
									bool flag12 = this.SortFilterSetting.PoisonFilterType.Count == 1;
									if (flag12)
									{
										this._poisonFilterTogGroup.Set((int)this.SortFilterSetting.PoisonFilterType[0], true, true);
									}
									else
									{
										this._poisonFilterTogGroup.Set(0, true, true);
									}
									this._poisonTogInitializing = false;
								}
								else
								{
									bool flag13 = this._entertainFilterTogGroup && this._entertainFilterTogGroup.gameObject.activeSelf;
									if (flag13)
									{
										this._entertainTogInitializing = true;
										bool flag14 = this.SortFilterSetting.EntertainFilterType.Count == 1;
										if (flag14)
										{
											this._entertainFilterTogGroup.Set((int)this.SortFilterSetting.EntertainFilterType[0], true, true);
										}
										else
										{
											this._entertainFilterTogGroup.Set(-1, true, true);
										}
										this._entertainTogInitializing = false;
									}
									else
									{
										bool flag15 = this._bookFilterTogGroup && this._bookFilterTogGroup.gameObject.activeSelf;
										if (flag15)
										{
											this._bookTogInitializing = true;
											bool flag16 = this.SortFilterSetting.BookFilterType.Count == 1;
											if (flag16)
											{
												this._bookFilterTogGroup.Set((int)this.SortFilterSetting.BookFilterType[0], true, true);
											}
											else
											{
												this._bookFilterTogGroup.Set(0, true, true);
											}
											this._bookTogInitializing = false;
										}
										else
										{
											bool flag17 = this._materialFilterTogGroup && this._materialFilterTogGroup.gameObject.activeSelf;
											if (flag17)
											{
												this._materialTogInitializing = true;
												bool flag18 = this.SortFilterSetting.MaterialFilterType.Count == 1;
												if (flag18)
												{
													this._materialFilterTogGroup.Set((int)this.SortFilterSetting.MaterialFilterType[0], true, true);
												}
												else
												{
													this._materialFilterTogGroup.Set(0, true, true);
												}
												this._materialTogInitializing = false;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			for (ItemSortAndFilter.SortType type = ItemSortAndFilter.SortType.Name; type < ItemSortAndFilter.SortType.Count; type++)
			{
				int index = this.SortFilterSetting.SortTypes.IndexOf(type);
				Transform sortBtnTrans = this._sortBtnHolder.GetChild((int)type);
				RectTransform arrow = sortBtnTrans.Find("Arrow").GetComponent<RectTransform>();
				arrow.gameObject.SetActive(index >= 0);
				bool flag19 = index >= 0;
				if (flag19)
				{
					bool isDescSort = this.SortFilterSetting.IsDescSort[index];
					arrow.localRotation = SortFilter.GetArrowRotation(isDescSort);
					arrow.anchoredPosition = SortFilter.GetArrowAnchoredPos(isDescSort);
				}
				sortBtnTrans.Find("Index").GetComponent<TextMeshProUGUI>().text = ((index < 0) ? "" : string.Format("{0}", index + 1));
				sortBtnTrans.Find("IndexBg").gameObject.SetActive(index >= 0);
				sortBtnTrans.Find("CheckMark").gameObject.SetActive(index >= 0);
			}
		}
		this.UpdateItemList();
	}

	// Token: 0x0600317A RID: 12666 RVA: 0x001863E4 File Offset: 0x001845E4
	public void LockFilterType(ItemSortAndFilter.ItemFilterType type, ItemSortAndFilter.LockFilterTypeToggleActionMode mode = ItemSortAndFilter.LockFilterTypeToggleActionMode.Default)
	{
		this._filterTogGroup.SetInteractable(type == ItemSortAndFilter.ItemFilterType.Invalid, null);
		bool flag = mode == ItemSortAndFilter.LockFilterTypeToggleActionMode.Default;
		if (flag)
		{
			this._filterTogGroup.Set((int)type, true, true);
		}
		else
		{
			CToggleObsolete oldToggle = this._filterTogGroup.GetActive();
			bool flag2 = type > ItemSortAndFilter.ItemFilterType.Invalid;
			if (flag2)
			{
				bool flag3 = oldToggle.Key != (int)type;
				if (flag3)
				{
					this._filterTogGroup.Set((int)type, true, true);
				}
			}
			else
			{
				this._filterTogGroup.Set((int)type, true, true);
			}
		}
	}

	// Token: 0x0600317B RID: 12667 RVA: 0x00186468 File Offset: 0x00184668
	public void LockFilterType(List<ItemSortAndFilter.ItemFilterType> typeList, ItemSortAndFilter.LockFilterTypeToggleActionMode mode = ItemSortAndFilter.LockFilterTypeToggleActionMode.Default)
	{
		for (int i = 0; i < this._filterTogGroup.Count(); i++)
		{
			bool interactable = typeList.Contains((ItemSortAndFilter.ItemFilterType)i);
			this._filterTogGroup.SetInteractable(interactable, i);
		}
		bool flag = mode == ItemSortAndFilter.LockFilterTypeToggleActionMode.Default;
		if (flag)
		{
			this._filterTogGroup.Set(0, true, true);
		}
		else
		{
			CToggleObsolete oldToggle = this._filterTogGroup.GetActive();
			bool flag2 = !typeList.Contains((ItemSortAndFilter.ItemFilterType)oldToggle.Key);
			if (flag2)
			{
				this._filterTogGroup.Set(0, true, true);
			}
			else
			{
				this._filterTogGroup.Set(oldToggle.Key, true, true);
			}
		}
	}

	// Token: 0x0600317C RID: 12668 RVA: 0x00186510 File Offset: 0x00184710
	public void ShowFilterType(ItemSortAndFilter.ItemFilterType type)
	{
		for (int i = 1; i < this._filterTogGroup.Count(); i++)
		{
			this._filterTogGroup.Get(i).gameObject.SetActive(type == ItemSortAndFilter.ItemFilterType.Invalid || i == (int)type);
		}
	}

	// Token: 0x0600317D RID: 12669 RVA: 0x0018655C File Offset: 0x0018475C
	public void ShowFilterType(List<ItemSortAndFilter.ItemFilterType> typeList)
	{
		for (int i = 1; i < this._filterTogGroup.Count(); i++)
		{
			bool interactable = typeList.Contains((ItemSortAndFilter.ItemFilterType)i);
			this._filterTogGroup.Get(i).gameObject.SetActive(interactable);
		}
	}

	// Token: 0x0600317E RID: 12670 RVA: 0x001865A8 File Offset: 0x001847A8
	public void ShowAllFilterType()
	{
		for (int i = 1; i < this._filterTogGroup.Count(); i++)
		{
			this._filterTogGroup.Get(i).gameObject.SetActive(true);
		}
	}

	// Token: 0x0600317F RID: 12671 RVA: 0x001865EC File Offset: 0x001847EC
	public void ShowEquipFilterType(ItemSortAndFilter.EquipFilterType equipType)
	{
		for (int i = 1; i < this._equipFilterTogGroup.Count(); i++)
		{
			this._equipFilterTogGroup.Get(i).gameObject.SetActive(equipType == ItemSortAndFilter.EquipFilterType.Invalid || i == (int)equipType);
		}
	}

	// Token: 0x06003180 RID: 12672 RVA: 0x00186638 File Offset: 0x00184838
	public void ShowEquipFilterType(List<ItemSortAndFilter.EquipFilterType> equipTypeList)
	{
		for (int i = 1; i < this._equipFilterTogGroup.Count(); i++)
		{
			bool interactable = equipTypeList.Contains((ItemSortAndFilter.EquipFilterType)i);
			this._equipFilterTogGroup.Get(i).gameObject.SetActive(interactable);
		}
	}

	// Token: 0x06003181 RID: 12673 RVA: 0x00186684 File Offset: 0x00184884
	public void ShowAllEquipFilterType()
	{
		for (int i = 1; i < this._equipFilterTogGroup.Count(); i++)
		{
			this._equipFilterTogGroup.Get(i).gameObject.SetActive(true);
		}
	}

	// Token: 0x06003182 RID: 12674 RVA: 0x001866C6 File Offset: 0x001848C6
	public void SetFilterType(ItemSortAndFilter.ItemFilterType type, bool forceRaiseEvent = false)
	{
		this._filterTogGroup.Set((int)type, true, forceRaiseEvent);
	}

	// Token: 0x06003183 RID: 12675 RVA: 0x001866D8 File Offset: 0x001848D8
	public void SetEquipFilterType(ItemSortAndFilter.EquipFilterType type, bool forceRaiseEvent = false)
	{
		this._equipFilterTogGroup.Set((int)type, true, forceRaiseEvent);
	}

	// Token: 0x06003184 RID: 12676 RVA: 0x001866EC File Offset: 0x001848EC
	public void SetEquipFilterTypeDisableStyle(ItemSortAndFilter.EquipFilterType type, bool value)
	{
		CToggleObsolete filter = this._equipFilterTogGroup.Get((int)type);
		DisableStyleRoot disableStyle = filter.GetComponent<DisableStyleRoot>();
		bool flag = disableStyle != null;
		if (flag)
		{
			disableStyle.SetStyleEffect(value, false);
			this._equipFilterTogGroup.SetInteractable(!value, filter);
		}
	}

	// Token: 0x06003185 RID: 12677 RVA: 0x00186738 File Offset: 0x00184938
	public ItemSortAndFilter.EquipFilterType GetCurrentActiveEquipFilterType()
	{
		return (ItemSortAndFilter.EquipFilterType)this._equipFilterTogGroup.GetActive().Key;
	}

	// Token: 0x06003186 RID: 12678 RVA: 0x0018675A File Offset: 0x0018495A
	public void SetMedicineFilterType(ItemSortAndFilter.MedicineFilterType type, bool forceRaiseEvent = false)
	{
		this._medicineFilterTogGroup.Set((int)type, true, forceRaiseEvent);
	}

	// Token: 0x06003187 RID: 12679 RVA: 0x0018676C File Offset: 0x0018496C
	public void SetSpecialBreakFilterType(ItemSortAndFilter.SpecialBreakFilterType type, bool forceRaiseEvent = false)
	{
		this._specialBreakFilterTogGroup.Set((int)type, true, forceRaiseEvent);
	}

	// Token: 0x06003188 RID: 12680 RVA: 0x0018677E File Offset: 0x0018497E
	public void SetPoisonFilterType(ItemSortAndFilter.PoisonFilterType type, bool forceRaiseEvent = false)
	{
		this._poisonFilterTogGroup.Set((int)type, true, forceRaiseEvent);
	}

	// Token: 0x06003189 RID: 12681 RVA: 0x00186790 File Offset: 0x00184990
	public void SetEntertainFilterType(ItemSortAndFilter.EntertainFilterType type, bool forceRaiseEvent = false)
	{
		this._entertainFilterTogGroup.Set((int)type, true, forceRaiseEvent);
	}

	// Token: 0x0600318A RID: 12682 RVA: 0x001867A2 File Offset: 0x001849A2
	public void SetBookFilterType(ItemSortAndFilter.BookFilterType type, bool forceRaiseEvent = false)
	{
		this._bookFilterTogGroup.Set((int)type, true, forceRaiseEvent);
	}

	// Token: 0x0600318B RID: 12683 RVA: 0x001867B4 File Offset: 0x001849B4
	public void SetMaterialFilterType(ItemSortAndFilter.MaterialFilterType type, bool forceRaiseEvent = false)
	{
		this._materialFilterTogGroup.Set((int)type, true, forceRaiseEvent);
	}

	// Token: 0x0600318C RID: 12684 RVA: 0x001867C8 File Offset: 0x001849C8
	public bool IsFilterTypeVisible(ItemSortAndFilter.ItemFilterType type)
	{
		return this._filterTogGroup.Get((int)type).gameObject.activeSelf;
	}

	// Token: 0x0600318D RID: 12685 RVA: 0x001867F0 File Offset: 0x001849F0
	private void EnableSwitchStyle()
	{
		this._disableStyleViewSwitch = false;
		this._itemSortTogGroup.SetInteractable(true, null);
	}

	// Token: 0x0600318E RID: 12686 RVA: 0x00186808 File Offset: 0x00184A08
	private void OnClickSortType(CButtonObsolete btn, ItemSortAndFilter.SortType type)
	{
		int index = this.SortFilterSetting.SortTypes.IndexOf(type);
		RectTransform arrow = btn.transform.Find("Arrow").GetComponent<RectTransform>();
		bool flag = index >= 0;
		if (flag)
		{
			bool flag2 = this.SortFilterSetting.IsDescSort[index];
			if (flag2)
			{
				this.SortFilterSetting.IsDescSort[index] = false;
			}
			else
			{
				this.SortFilterSetting.SortTypes.RemoveAt(index);
				this.SortFilterSetting.IsDescSort.RemoveAt(index);
				for (int i = index; i < this.SortFilterSetting.SortTypes.Count; i++)
				{
					this._sortBtnHolder.GetChild((int)this.SortFilterSetting.SortTypes[i]).Find("Index").GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
				}
				index = -1;
			}
		}
		else
		{
			index = this.SortFilterSetting.SortTypes.Count;
			this.SortFilterSetting.SortTypes.Add(type);
			this.SortFilterSetting.IsDescSort.Add(true);
		}
		arrow.gameObject.SetActive(index >= 0);
		bool flag3 = index >= 0;
		if (flag3)
		{
			bool isDescSort = this.SortFilterSetting.IsDescSort[index];
			arrow.localRotation = SortFilter.GetArrowRotation(isDescSort);
			arrow.anchoredPosition = SortFilter.GetArrowAnchoredPos(isDescSort);
		}
		btn.transform.Find("Index").GetComponent<TextMeshProUGUI>().text = ((index < 0) ? "" : string.Format("{0}", index + 1));
		btn.transform.Find("IndexBg").gameObject.SetActive(index >= 0);
		btn.transform.Find("CheckMark").gameObject.SetActive(index >= 0);
		this.UpdateItemList();
	}

	// Token: 0x0600318F RID: 12687 RVA: 0x00186A18 File Offset: 0x00184C18
	private void OnItemFilterTogChange(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		bool filterTogInitializing = this._filterTogInitializing;
		if (!filterTogInitializing)
		{
			this.SortFilterSetting.ItemFilterType.Clear();
			bool flag = !this._filterTogInitializing;
			if (flag)
			{
				bool flag2 = this._filterTogGroup.GetActive().Key == 0;
				if (flag2)
				{
					foreach (CToggleObsolete tog in this._filterTogGroup.GetAll())
					{
						bool flag3 = tog.Key != 0 && (ItemSortAndFilter.IsToggleActive(tog) || this.AllTypeIncludeInactive);
						if (flag3)
						{
							this.SortFilterSetting.ItemFilterType.Add((ItemSortAndFilter.ItemFilterType)tog.Key);
						}
					}
				}
				else
				{
					this.SortFilterSetting.ItemFilterType.Add((ItemSortAndFilter.ItemFilterType)this._filterTogGroup.GetActive().Key);
				}
			}
			this.UpdateItemList();
		}
	}

	// Token: 0x06003190 RID: 12688 RVA: 0x00186B20 File Offset: 0x00184D20
	private void OnEquipFilterTogChange(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		bool equipTogInitializing = this._equipTogInitializing;
		if (!equipTogInitializing)
		{
			this.SortFilterSetting.EquipFilterType.Clear();
			bool flag = !this._equipTogInitializing;
			if (flag)
			{
				bool flag2 = this._equipFilterTogGroup.GetActive().Key == 0;
				if (flag2)
				{
					foreach (CToggleObsolete tog in this._equipFilterTogGroup.GetAll())
					{
						bool flag3 = tog.Key != 0 && ItemSortAndFilter.IsToggleActive(tog);
						if (flag3)
						{
							this.SortFilterSetting.EquipFilterType.Add((ItemSortAndFilter.EquipFilterType)tog.Key);
						}
					}
				}
				else
				{
					this.SortFilterSetting.EquipFilterType.Add((ItemSortAndFilter.EquipFilterType)this._equipFilterTogGroup.GetActive().Key);
				}
			}
			this.UpdateItemList();
		}
	}

	// Token: 0x06003191 RID: 12689 RVA: 0x00186C1C File Offset: 0x00184E1C
	private void OnMedicineFilterTogChange(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		bool medicineTogInitializing = this._medicineTogInitializing;
		if (!medicineTogInitializing)
		{
			this.SortFilterSetting.MedicineFilterType.Clear();
			bool flag = !this._medicineTogInitializing;
			if (flag)
			{
				bool flag2 = this._medicineFilterTogGroup.GetActive().Key == 0;
				if (flag2)
				{
					foreach (CToggleObsolete tog in this._medicineFilterTogGroup.GetAll())
					{
						bool flag3 = tog.Key != 0 && ItemSortAndFilter.IsToggleActive(tog);
						if (flag3)
						{
							this.SortFilterSetting.MedicineFilterType.Add((ItemSortAndFilter.MedicineFilterType)tog.Key);
						}
					}
				}
				else
				{
					this.SortFilterSetting.MedicineFilterType.Add((ItemSortAndFilter.MedicineFilterType)this._medicineFilterTogGroup.GetActive().Key);
				}
			}
			this.UpdateItemList();
		}
	}

	// Token: 0x06003192 RID: 12690 RVA: 0x00186D18 File Offset: 0x00184F18
	private void OnSpecialBreakFilterTogChange(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		bool specialBreakTogInitializing = this._specialBreakTogInitializing;
		if (!specialBreakTogInitializing)
		{
			this.SortFilterSetting.SpecialBreakFilterType.Clear();
			bool flag = !this._specialBreakTogInitializing;
			if (flag)
			{
				bool flag2 = this._specialBreakFilterTogGroup.GetActive().Key == 0;
				if (flag2)
				{
					foreach (CToggleObsolete tog in this._specialBreakFilterTogGroup.GetAll())
					{
						bool flag3 = tog.Key != 0 && ItemSortAndFilter.IsToggleActive(tog);
						if (flag3)
						{
							this.SortFilterSetting.SpecialBreakFilterType.Add((ItemSortAndFilter.SpecialBreakFilterType)tog.Key);
						}
					}
				}
				else
				{
					this.SortFilterSetting.SpecialBreakFilterType.Add((ItemSortAndFilter.SpecialBreakFilterType)this._specialBreakFilterTogGroup.GetActive().Key);
				}
			}
			this.UpdateItemList();
		}
	}

	// Token: 0x06003193 RID: 12691 RVA: 0x00186E14 File Offset: 0x00185014
	private void OnClothingWeaveFilterTogChange(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		bool clothingWeaveTogInitializing = this._clothingWeaveTogInitializing;
		if (!clothingWeaveTogInitializing)
		{
			this.SortFilterSetting.ClothingWeaverFilterType.Clear();
			bool flag = !this._clothingWeaveTogInitializing;
			if (flag)
			{
				bool flag2 = this._clothingWeaveFilterTogGroup.GetActive().Key == 0;
				if (flag2)
				{
					foreach (CToggleObsolete tog in this._clothingWeaveFilterTogGroup.GetAll())
					{
						bool flag3 = tog.Key != 0 && ItemSortAndFilter.IsToggleActive(tog);
						if (flag3)
						{
							this.SortFilterSetting.ClothingWeaverFilterType.Add((ItemSortAndFilter.ClothingWeaverFilterType)tog.Key);
						}
					}
				}
				else
				{
					this.SortFilterSetting.ClothingWeaverFilterType.Add((ItemSortAndFilter.ClothingWeaverFilterType)this._clothingWeaveFilterTogGroup.GetActive().Key);
				}
			}
			this.UpdateItemList();
		}
	}

	// Token: 0x06003194 RID: 12692 RVA: 0x00186F10 File Offset: 0x00185110
	private void OnPoisonFilterTogChange(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		bool poisonTogInitializing = this._poisonTogInitializing;
		if (!poisonTogInitializing)
		{
			this.SortFilterSetting.PoisonFilterType.Clear();
			bool flag = !this._poisonTogInitializing;
			if (flag)
			{
				bool flag2 = this._poisonFilterTogGroup.GetActive().Key == 0;
				if (flag2)
				{
					foreach (CToggleObsolete tog in this._poisonFilterTogGroup.GetAll())
					{
						bool flag3 = tog.Key != 0 && ItemSortAndFilter.IsToggleActive(tog);
						if (flag3)
						{
							this.SortFilterSetting.PoisonFilterType.Add((ItemSortAndFilter.PoisonFilterType)tog.Key);
						}
					}
				}
				else
				{
					this.SortFilterSetting.PoisonFilterType.Add((ItemSortAndFilter.PoisonFilterType)this._poisonFilterTogGroup.GetActive().Key);
				}
			}
			this.UpdateItemList();
		}
	}

	// Token: 0x06003195 RID: 12693 RVA: 0x0018700C File Offset: 0x0018520C
	private void OnEntertainFilterTogChange(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		bool entertainTogInitializing = this._entertainTogInitializing;
		if (!entertainTogInitializing)
		{
			this.SortFilterSetting.EntertainFilterType.Clear();
			bool flag = !this._entertainTogInitializing;
			if (flag)
			{
				bool flag2 = this._entertainFilterTogGroup.GetActive().Key == -1;
				if (flag2)
				{
					foreach (CToggleObsolete tog in this._entertainFilterTogGroup.GetAll())
					{
						bool flag3 = tog.Key != 0 && ItemSortAndFilter.IsToggleActive(tog);
						if (flag3)
						{
							this.SortFilterSetting.EntertainFilterType.Add((ItemSortAndFilter.EntertainFilterType)tog.Key);
						}
					}
				}
				else
				{
					this.SortFilterSetting.EntertainFilterType.Add((ItemSortAndFilter.EntertainFilterType)this._entertainFilterTogGroup.GetActive().Key);
				}
			}
			this.UpdateItemList();
		}
	}

	// Token: 0x06003196 RID: 12694 RVA: 0x00187108 File Offset: 0x00185308
	private void OnBookFilterTogChange(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		bool bookTogInitializing = this._bookTogInitializing;
		if (!bookTogInitializing)
		{
			this.SortFilterSetting.BookFilterType.Clear();
			bool flag = !this._bookTogInitializing;
			if (flag)
			{
				bool flag2 = this._bookFilterTogGroup.GetActive().Key == 0;
				if (flag2)
				{
					foreach (CToggleObsolete tog in this._bookFilterTogGroup.GetAll())
					{
						bool flag3 = tog.Key != 0 && ItemSortAndFilter.IsToggleActive(tog);
						if (flag3)
						{
							this.SortFilterSetting.BookFilterType.Add((ItemSortAndFilter.BookFilterType)tog.Key);
						}
					}
				}
				else
				{
					this.SortFilterSetting.BookFilterType.Add((ItemSortAndFilter.BookFilterType)this._bookFilterTogGroup.GetActive().Key);
				}
			}
			this.UpdateItemList();
		}
	}

	// Token: 0x06003197 RID: 12695 RVA: 0x00187204 File Offset: 0x00185404
	private void OnMaterialFilterTogChange(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		bool materialTogInitializing = this._materialTogInitializing;
		if (!materialTogInitializing)
		{
			this.SortFilterSetting.MaterialFilterType.Clear();
			bool flag = !this._materialTogInitializing;
			if (flag)
			{
				bool flag2 = this._materialFilterTogGroup.GetActive().Key == 0;
				if (flag2)
				{
					foreach (CToggleObsolete tog in this._materialFilterTogGroup.GetAll())
					{
						bool flag3 = tog.Key != 0 && ItemSortAndFilter.IsToggleActive(tog);
						if (flag3)
						{
							this.SortFilterSetting.MaterialFilterType.Add((ItemSortAndFilter.MaterialFilterType)tog.Key);
						}
					}
				}
				else
				{
					this.SortFilterSetting.MaterialFilterType.Add((ItemSortAndFilter.MaterialFilterType)this._materialFilterTogGroup.GetActive().Key);
				}
			}
			this.UpdateItemList();
		}
	}

	// Token: 0x06003198 RID: 12696 RVA: 0x00187300 File Offset: 0x00185500
	private void OnItemSortTogChange(CToggleObsolete newTog, CToggleObsolete olgTog)
	{
		bool disableStyleViewSwitch = this._disableStyleViewSwitch;
		if (!disableStyleViewSwitch)
		{
			this._disableStyleViewSwitch = true;
			this._itemSortTogGroup.SetInteractable(false, null);
			this.IsDetailView = (newTog.Key == 0);
			Action onViewTypeChanged = this._onViewTypeChanged;
			if (onViewTypeChanged != null)
			{
				onViewTypeChanged();
			}
			base.Invoke("EnableSwitchStyle", this.UserFloat);
		}
	}

	// Token: 0x06003199 RID: 12697 RVA: 0x00187364 File Offset: 0x00185564
	private void UpdateItemList()
	{
		this.OutputItemList.Clear();
		bool activeSelf = this._filterTogGroup.gameObject.activeSelf;
		if (activeSelf)
		{
			List<ItemSortAndFilter.ItemFilterType> typeList = this.SortFilterSetting.ItemFilterType;
			bool flag = typeList.Count == 0 || typeList[0] == ItemSortAndFilter.ItemFilterType.Invalid;
			if (flag)
			{
				this.OutputItemList.AddRange(this._itemList);
			}
			else
			{
				this.OutputItemList.AddRange(this._itemList.FindAll((ItemDisplayData data) => typeList.Contains(ItemSortAndFilter.GetFilterType(data.Key.ItemType))));
			}
		}
		else
		{
			bool activeSelf2 = this._equipFilterTogGroup.gameObject.activeSelf;
			if (activeSelf2)
			{
				List<ItemSortAndFilter.EquipFilterType> typeList = this.SortFilterSetting.EquipFilterType;
				bool flag2 = typeList.Count == 0 || typeList[0] == ItemSortAndFilter.EquipFilterType.Invalid;
				if (flag2)
				{
					this.OutputItemList.AddRange(this._itemList);
				}
				else
				{
					this.OutputItemList.AddRange(this._itemList.FindAll((ItemDisplayData data) => !data.Key.IsValid() || typeList.Contains(ItemSortAndFilter.GetEquipFilterType(data.Key))));
				}
			}
			else
			{
				bool activeSelf3 = this._medicineFilterTogGroup.gameObject.activeSelf;
				if (activeSelf3)
				{
					List<ItemSortAndFilter.MedicineFilterType> typeList = this.SortFilterSetting.MedicineFilterType;
					bool flag3 = typeList.Count == 0 || typeList[0] == ItemSortAndFilter.MedicineFilterType.Invalid;
					if (flag3)
					{
						this.OutputItemList.AddRange(this._itemList);
					}
					else
					{
						this.OutputItemList.AddRange(this._itemList.FindAll((ItemDisplayData data) => !data.Key.IsValid() || typeList.Contains(ItemSortAndFilter.GetMedicineFilterType(data.Key))));
					}
				}
				else
				{
					bool activeSelf4 = this._specialBreakFilterTogGroup.gameObject.activeSelf;
					if (activeSelf4)
					{
						List<ItemSortAndFilter.SpecialBreakFilterType> typeList = this.SortFilterSetting.SpecialBreakFilterType;
						bool flag4 = typeList.Count == 0 || typeList[0] == ItemSortAndFilter.SpecialBreakFilterType.Invalid;
						if (flag4)
						{
							this.OutputItemList.AddRange(this._itemList);
						}
						else
						{
							this.OutputItemList.AddRange(this._itemList.FindAll((ItemDisplayData data) => !data.Key.IsValid() || typeList.Contains(ItemSortAndFilter.GetSpecialBreakFilterType(data.Key))));
						}
					}
					else
					{
						bool activeSelf5 = this._clothingWeaveFilterTogGroup.gameObject.activeSelf;
						if (activeSelf5)
						{
							List<ItemSortAndFilter.ClothingWeaverFilterType> typeList = this.SortFilterSetting.ClothingWeaverFilterType;
							bool flag5 = typeList.Count == 0 || typeList[0] == ItemSortAndFilter.ClothingWeaverFilterType.Invalid;
							if (flag5)
							{
								this.OutputItemList.AddRange(this._itemList);
							}
							else
							{
								this.OutputItemList.AddRange(this._itemList.FindAll((ItemDisplayData data) => typeList.Contains(ItemSortAndFilter.GetClothingWeaverFilterType(data.Key))));
							}
						}
						else
						{
							bool activeSelf6 = this._poisonFilterTogGroup.gameObject.activeSelf;
							if (activeSelf6)
							{
								List<ItemSortAndFilter.PoisonFilterType> typeList = this.SortFilterSetting.PoisonFilterType;
								bool flag6 = typeList.Count == 0 || typeList[0] == ItemSortAndFilter.PoisonFilterType.Invalid;
								if (flag6)
								{
									this.OutputItemList.AddRange(this._itemList);
								}
								else
								{
									this.OutputItemList.AddRange(this._itemList.FindAll(delegate(ItemDisplayData data)
									{
										List<ItemSortAndFilter.PoisonFilterType> list = ItemSortAndFilter.GetPoisonFilterTypePoolList(data.Key);
										List<ItemSortAndFilter.PoisonFilterType> intersect = typeList.Intersect(list).ToPoolList<ItemSortAndFilter.PoisonFilterType>();
										bool meet = intersect.Count > 0;
										EasyPool.Free<List<ItemSortAndFilter.PoisonFilterType>>(list);
										EasyPool.Free<List<ItemSortAndFilter.PoisonFilterType>>(intersect);
										return meet;
									}));
								}
							}
							else
							{
								bool activeSelf7 = this._entertainFilterTogGroup.gameObject.activeSelf;
								if (activeSelf7)
								{
									List<ItemSortAndFilter.EntertainFilterType> typeList = this.SortFilterSetting.EntertainFilterType;
									bool flag7 = typeList.Count == 0 || typeList[0] == ItemSortAndFilter.EntertainFilterType.Invalid;
									if (flag7)
									{
										this.OutputItemList.AddRange(this._itemList);
									}
									else
									{
										this.OutputItemList.AddRange(this._itemList.FindAll(delegate(ItemDisplayData data)
										{
											ItemSortAndFilter.EntertainFilterType filterType = typeList[0];
											short itemSubType = ItemTemplateHelper.GetItemSubType(data.Key.ItemType, data.Key.TemplateId);
											bool flag13 = filterType == ItemSortAndFilter.EntertainFilterType.Tea;
											bool result;
											if (flag13)
											{
												result = (itemSubType == 900);
											}
											else
											{
												bool flag14 = filterType == ItemSortAndFilter.EntertainFilterType.Wine;
												if (flag14)
												{
													result = (itemSubType == 901);
												}
												else
												{
													List<ItemSortAndFilter.EntertainFilterType> list = ItemSortAndFilter.GetEntertainFilterTypePoolList(data.Key);
													List<ItemSortAndFilter.EntertainFilterType> intersect = typeList.Intersect(list).ToPoolList<ItemSortAndFilter.EntertainFilterType>();
													bool meet = intersect.Count > 0;
													EasyPool.Free<List<ItemSortAndFilter.EntertainFilterType>>(list);
													EasyPool.Free<List<ItemSortAndFilter.EntertainFilterType>>(intersect);
													result = meet;
												}
											}
											return result;
										}));
									}
								}
								else
								{
									bool flag8 = this._bookFilterTogGroup && this._bookFilterTogGroup.gameObject.activeSelf;
									if (flag8)
									{
										List<ItemSortAndFilter.BookFilterType> typeList = this.SortFilterSetting.BookFilterType;
										bool flag9 = typeList.Count == 0 || typeList[0] == ItemSortAndFilter.BookFilterType.Invalid;
										if (flag9)
										{
											this.OutputItemList.AddRange(this._itemList);
										}
										else
										{
											this.OutputItemList.AddRange(this._itemList.FindAll((ItemDisplayData data) => typeList.Contains(ItemSortAndFilter.GetBookFilterType(data.Key))));
										}
									}
									else
									{
										bool flag10 = this._materialFilterTogGroup && this._materialFilterTogGroup.gameObject.activeSelf;
										if (flag10)
										{
											List<ItemSortAndFilter.MaterialFilterType> typeList = this.SortFilterSetting.MaterialFilterType;
											bool flag11 = typeList.Count == 0 || typeList[0] == ItemSortAndFilter.MaterialFilterType.Invalid;
											if (flag11)
											{
												this.OutputItemList.AddRange(this._itemList);
											}
											else
											{
												this.OutputItemList.AddRange(this._itemList.FindAll((ItemDisplayData data) => typeList.Contains(ItemSortAndFilter.GetMaterialFilterType(data.Key))));
											}
										}
										else
										{
											this.OutputItemList.AddRange(this._itemList);
										}
									}
								}
							}
						}
					}
				}
			}
		}
		bool flag12 = this.SetItemInteraction != null;
		if (flag12)
		{
			foreach (ItemDisplayData data2 in this.OutputItemList)
			{
				data2.Interactable = this.SetItemInteraction(data2);
			}
		}
		bool sortEnabled = this.SortEnabled;
		if (sortEnabled)
		{
			this.OutputItemList.Sort(new Comparison<ItemDisplayData>(this.ItemCompare));
		}
		Action onItemListChanged = this._onItemListChanged;
		if (onItemListChanged != null)
		{
			onItemListChanged();
		}
	}

	// Token: 0x0600319A RID: 12698 RVA: 0x00187980 File Offset: 0x00185B80
	private int ItemCompare(ItemDisplayData itemL, ItemDisplayData itemR)
	{
		ItemSortAndFilter.<>c__DisplayClass98_0 CS$<>8__locals1;
		CS$<>8__locals1.itemL = itemL;
		CS$<>8__locals1.itemR = itemR;
		bool flag = this.IsOnSpecialBreakMultiplySelect && this._specialBreakSelectedBonusId != -1;
		if (flag)
		{
			bool leftFit = SectMainStorySharedMethods.IsEmeiBonusFit(this._specialBreakSelectedBonusId, CS$<>8__locals1.itemL.Key);
			bool rightFit = SectMainStorySharedMethods.IsEmeiBonusFit(this._specialBreakSelectedBonusId, CS$<>8__locals1.itemR.Key);
			bool flag2 = leftFit != rightFit;
			if (flag2)
			{
				return leftFit ? -1 : 1;
			}
		}
		ItemKey key = CS$<>8__locals1.itemL.Key;
		bool flag3;
		if (key.Equals(ItemKey.Invalid))
		{
			key = CS$<>8__locals1.itemR.Key;
			flag3 = !key.Equals(ItemKey.Invalid);
		}
		else
		{
			flag3 = false;
		}
		bool flag4 = flag3;
		int result3;
		if (flag4)
		{
			result3 = -1;
		}
		else
		{
			key = CS$<>8__locals1.itemR.Key;
			bool flag5;
			if (key.Equals(ItemKey.Invalid))
			{
				key = CS$<>8__locals1.itemL.Key;
				flag5 = !key.Equals(ItemKey.Invalid);
			}
			else
			{
				flag5 = false;
			}
			bool flag6 = flag5;
			if (flag6)
			{
				result3 = 1;
			}
			else
			{
				bool flag7 = this.StaticAheadItemKeysList.Contains(CS$<>8__locals1.itemL.Key) && !this.StaticAheadItemKeysList.Contains(CS$<>8__locals1.itemR.Key);
				if (flag7)
				{
					result3 = -1;
				}
				else
				{
					bool flag8 = this.StaticAheadItemKeysList.Contains(CS$<>8__locals1.itemR.Key) && !this.StaticAheadItemKeysList.Contains(CS$<>8__locals1.itemL.Key);
					if (flag8)
					{
						result3 = 1;
					}
					else
					{
						bool isEmptyToolL = ItemTemplateHelper.IsEmptyTool(CS$<>8__locals1.itemL.Key.ItemType, CS$<>8__locals1.itemL.Key.TemplateId);
						bool isEmptyToolR = ItemTemplateHelper.IsEmptyTool(CS$<>8__locals1.itemR.Key.ItemType, CS$<>8__locals1.itemR.Key.TemplateId);
						bool flag9 = isEmptyToolL != isEmptyToolR;
						if (flag9)
						{
							result3 = isEmptyToolR.CompareTo(isEmptyToolL);
						}
						else
						{
							bool flag10 = CS$<>8__locals1.itemL.Interactable != CS$<>8__locals1.itemR.Interactable;
							if (flag10)
							{
								result3 = (CS$<>8__locals1.itemL.Interactable ? -1 : 1);
							}
							else
							{
								bool flag11 = CS$<>8__locals1.itemL.UnavailableType != CS$<>8__locals1.itemR.UnavailableType;
								if (flag11)
								{
									result3 = ((CS$<>8__locals1.itemL.UnavailableType == ItemDisplayData.ItemUnavailableType.Valid) ? -1 : 1);
								}
								else
								{
									bool leftIsTianSuiBaoLuItem = CommonUtils.IsTianSuiBaoLuItem(CS$<>8__locals1.itemL.Key.ItemType, CS$<>8__locals1.itemL.Key.TemplateId);
									bool rightIsTianSuiBaoLuItem = CommonUtils.IsTianSuiBaoLuItem(CS$<>8__locals1.itemR.Key.ItemType, CS$<>8__locals1.itemR.Key.TemplateId);
									bool flag12 = leftIsTianSuiBaoLuItem && !rightIsTianSuiBaoLuItem;
									if (flag12)
									{
										result3 = -1;
									}
									else
									{
										bool flag13 = !leftIsTianSuiBaoLuItem && rightIsTianSuiBaoLuItem;
										if (flag13)
										{
											result3 = 1;
										}
										else
										{
											bool isMiscResourceL = ItemTemplateHelper.IsMiscResource(CS$<>8__locals1.itemL.Key.ItemType, CS$<>8__locals1.itemL.Key.TemplateId);
											bool isMiscResourceR = ItemTemplateHelper.IsMiscResource(CS$<>8__locals1.itemR.Key.ItemType, CS$<>8__locals1.itemR.Key.TemplateId);
											bool flag14 = isMiscResourceL && !isMiscResourceR;
											if (flag14)
											{
												result3 = -1;
											}
											else
											{
												bool flag15 = !isMiscResourceL && isMiscResourceR;
												if (flag15)
												{
													result3 = 1;
												}
												else
												{
													sbyte poisonTypeL = ItemTemplateHelper.GetMedicineItemPoisonType(CS$<>8__locals1.itemL.Key.ItemType, CS$<>8__locals1.itemL.Key.TemplateId);
													sbyte poisonTypeR = ItemTemplateHelper.GetMedicineItemPoisonType(CS$<>8__locals1.itemR.Key.ItemType, CS$<>8__locals1.itemR.Key.TemplateId);
													bool isOnAddPoison = this.IsOnAddPoison;
													if (isOnAddPoison)
													{
														bool poisonTypeLIsSelected = this._poisonTypeList.Contains(poisonTypeL);
														bool poisonTypeRIsSelected = this._poisonTypeList.Contains(poisonTypeR);
														bool flag16 = poisonTypeLIsSelected != poisonTypeRIsSelected;
														if (flag16)
														{
															return poisonTypeLIsSelected ? -1 : 1;
														}
													}
													else
													{
														bool isOnRemovePoison = this.IsOnRemovePoison;
														if (isOnRemovePoison)
														{
															int result = ItemSortAndFilter.<ItemCompare>g__ComparePoison|98_0(ref CS$<>8__locals1);
															bool flag17 = result != 0;
															if (flag17)
															{
																return result;
															}
														}
													}
													bool flag18 = CS$<>8__locals1.itemL.UsingType != CS$<>8__locals1.itemR.UsingType;
													if (flag18)
													{
														result3 = CS$<>8__locals1.itemR.UsingType.CompareTo(CS$<>8__locals1.itemL.UsingType);
													}
													else
													{
														sbyte itemGradeL = ItemTemplateHelper.GetGrade(CS$<>8__locals1.itemL.Key.ItemType, CS$<>8__locals1.itemL.Key.TemplateId);
														sbyte itemGradeR = ItemTemplateHelper.GetGrade(CS$<>8__locals1.itemR.Key.ItemType, CS$<>8__locals1.itemR.Key.TemplateId);
														for (int i = 0; i < this.SortFilterSetting.SortTypes.Count; i++)
														{
															switch (this.SortFilterSetting.SortTypes[i])
															{
															case ItemSortAndFilter.SortType.Name:
															{
																bool isLCricket = CS$<>8__locals1.itemL.Key.ItemType == 11;
																bool isRCricket = CS$<>8__locals1.itemR.Key.ItemType == 11;
																string itemNameL = isLCricket ? CS$<>8__locals1.itemL.CalcCricketName() : ItemTemplateHelper.GetName(CS$<>8__locals1.itemL.Key.ItemType, CS$<>8__locals1.itemL.Key.TemplateId);
																string itemNameR = isRCricket ? CS$<>8__locals1.itemR.CalcCricketName() : ItemTemplateHelper.GetName(CS$<>8__locals1.itemR.Key.ItemType, CS$<>8__locals1.itemR.Key.TemplateId);
																bool flag19 = itemNameL != itemNameR;
																if (flag19)
																{
																	return (!this.SortFilterSetting.IsDescSort[i]) ? Utils_Sorting.CompareByCurrentLangEncoding(itemNameL, itemNameR) : Utils_Sorting.CompareByCurrentLangEncoding(itemNameR, itemNameL);
																}
																break;
															}
															case ItemSortAndFilter.SortType.Grade:
															{
																bool flag20 = itemGradeL != itemGradeR;
																if (flag20)
																{
																	return this.SortFilterSetting.IsDescSort[i] ? itemGradeR.CompareTo(itemGradeL) : itemGradeL.CompareTo(itemGradeR);
																}
																break;
															}
															case ItemSortAndFilter.SortType.Value:
															{
																bool flag21 = CS$<>8__locals1.itemL.Value != CS$<>8__locals1.itemR.Value;
																if (flag21)
																{
																	return this.SortFilterSetting.IsDescSort[i] ? CS$<>8__locals1.itemR.Value.CompareTo(CS$<>8__locals1.itemL.Value) : CS$<>8__locals1.itemL.Value.CompareTo(CS$<>8__locals1.itemR.Value);
																}
																break;
															}
															case ItemSortAndFilter.SortType.Weight:
															{
																bool flag22 = CS$<>8__locals1.itemL.Weight != CS$<>8__locals1.itemR.Weight;
																if (flag22)
																{
																	return this.SortFilterSetting.IsDescSort[i] ? CS$<>8__locals1.itemR.Weight.CompareTo(CS$<>8__locals1.itemL.Weight) : CS$<>8__locals1.itemL.Weight.CompareTo(CS$<>8__locals1.itemR.Weight);
																}
																break;
															}
															}
														}
														bool flag23 = CS$<>8__locals1.itemL.Key.ItemType != CS$<>8__locals1.itemR.Key.ItemType;
														if (flag23)
														{
															key = CS$<>8__locals1.itemL.Key;
															result3 = key.ItemType.CompareTo(CS$<>8__locals1.itemR.Key.ItemType);
														}
														else
														{
															short subTypeL = ItemTemplateHelper.GetItemSubType(CS$<>8__locals1.itemL.Key.ItemType, CS$<>8__locals1.itemL.Key.TemplateId);
															short subTypeR = ItemTemplateHelper.GetItemSubType(CS$<>8__locals1.itemR.Key.ItemType, CS$<>8__locals1.itemR.Key.TemplateId);
															bool flag24 = subTypeL != subTypeR;
															if (flag24)
															{
																result3 = subTypeL.CompareTo(subTypeR);
															}
															else
															{
																bool flag25 = itemGradeL != itemGradeR;
																if (flag25)
																{
																	result3 = itemGradeR.CompareTo(itemGradeL);
																}
																else
																{
																	bool itemStackableL = ItemTemplateHelper.IsStackable(CS$<>8__locals1.itemL.Key.ItemType, CS$<>8__locals1.itemL.Key.TemplateId);
																	bool itemStackableR = ItemTemplateHelper.IsStackable(CS$<>8__locals1.itemR.Key.ItemType, CS$<>8__locals1.itemR.Key.TemplateId);
																	bool flag26 = itemStackableL != itemStackableR;
																	if (flag26)
																	{
																		result3 = (itemStackableL ? 1 : -1);
																	}
																	else
																	{
																		bool flag27 = CS$<>8__locals1.itemL.Key.ItemType == CS$<>8__locals1.itemR.Key.ItemType;
																		if (flag27)
																		{
																			bool flag28 = CS$<>8__locals1.itemR.Key.ItemType == 11;
																			if (flag28)
																			{
																				bool flag29 = itemGradeL != itemGradeR;
																				if (flag29)
																				{
																					result3 = itemGradeL.CompareTo(itemGradeR);
																				}
																				else
																				{
																					bool flag30 = CS$<>8__locals1.itemL.CricketColorId != CS$<>8__locals1.itemR.CricketColorId;
																					if (flag30)
																					{
																						result3 = CS$<>8__locals1.itemL.CricketColorId.CompareTo(CS$<>8__locals1.itemR.CricketColorId);
																					}
																					else
																					{
																						bool flag31 = CS$<>8__locals1.itemL.CricketPartId != CS$<>8__locals1.itemR.CricketPartId;
																						if (flag31)
																						{
																							result3 = CS$<>8__locals1.itemL.CricketPartId.CompareTo(CS$<>8__locals1.itemR.CricketPartId);
																						}
																						else
																						{
																							key = CS$<>8__locals1.itemL.Key;
																							result3 = key.Id.CompareTo(CS$<>8__locals1.itemR.Key.Id);
																						}
																					}
																				}
																			}
																			else
																			{
																				bool flag32 = poisonTypeL != poisonTypeR;
																				if (flag32)
																				{
																					sbyte poisonOderL = PoisonSortingOrder.GetSortingOrderByType(poisonTypeL);
																					sbyte poisonOderR = PoisonSortingOrder.GetSortingOrderByType(poisonTypeR);
																					bool flag33 = poisonOderL != poisonOderR;
																					if (flag33)
																					{
																						return poisonOderL.CompareTo(poisonOderR);
																					}
																				}
																				key = CS$<>8__locals1.itemL.Key;
																				bool flag34 = key.TemplateEquals(CS$<>8__locals1.itemR.Key);
																				if (flag34)
																				{
																					int result2 = ItemSortAndFilter.<ItemCompare>g__ComparePoison|98_0(ref CS$<>8__locals1);
																					bool flag35 = result2 != 0;
																					if (flag35)
																					{
																						return result2;
																					}
																				}
																				bool flag36 = CS$<>8__locals1.itemL.Key.TemplateId != CS$<>8__locals1.itemR.Key.TemplateId;
																				if (flag36)
																				{
																					key = CS$<>8__locals1.itemL.Key;
																					result3 = key.TemplateId.CompareTo(CS$<>8__locals1.itemR.Key.TemplateId);
																				}
																				else
																				{
																					bool flag37 = CS$<>8__locals1.itemL.ExtraGoodsType != CS$<>8__locals1.itemR.ExtraGoodsType;
																					if (flag37)
																					{
																						result3 = CS$<>8__locals1.itemL.ExtraGoodsType.CompareTo(CS$<>8__locals1.itemR.ExtraGoodsType);
																					}
																					else
																					{
																						result3 = 0;
																					}
																				}
																			}
																		}
																		else
																		{
																			key = CS$<>8__locals1.itemL.Key;
																			result3 = key.ItemType.CompareTo(CS$<>8__locals1.itemR.Key.ItemType);
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		return result3;
	}

	// Token: 0x0600319B RID: 12699 RVA: 0x0018849C File Offset: 0x0018669C
	public static ItemSortAndFilter.ItemFilterType GetFilterType(sbyte itemType)
	{
		ItemSortAndFilter.ItemFilterType result;
		switch (itemType)
		{
		case 0:
		case 1:
		case 2:
		case 3:
		case 4:
			result = ItemSortAndFilter.ItemFilterType.Equip;
			break;
		case 5:
			result = ItemSortAndFilter.ItemFilterType.Material;
			break;
		case 6:
			result = ItemSortAndFilter.ItemFilterType.Make;
			break;
		case 7:
		case 9:
			result = ItemSortAndFilter.ItemFilterType.Food;
			break;
		case 8:
			result = ItemSortAndFilter.ItemFilterType.Medicine;
			break;
		case 10:
			result = ItemSortAndFilter.ItemFilterType.Book;
			break;
		default:
			result = ItemSortAndFilter.ItemFilterType.Other;
			break;
		}
		return result;
	}

	// Token: 0x0600319C RID: 12700 RVA: 0x00188500 File Offset: 0x00186700
	public static ItemSortAndFilter.EquipFilterType GetEquipFilterType(ItemKey itemKey)
	{
		int equipmentType = ItemTemplateHelper.GetEquipmentType(itemKey.ItemType, itemKey.TemplateId);
		if (!true)
		{
		}
		ItemSortAndFilter.EquipFilterType result;
		switch (equipmentType)
		{
		case 0:
			result = ItemSortAndFilter.EquipFilterType.Weapon;
			break;
		case 1:
			result = ItemSortAndFilter.EquipFilterType.Helm;
			break;
		case 2:
			result = ItemSortAndFilter.EquipFilterType.Clothing;
			break;
		case 3:
			result = ItemSortAndFilter.EquipFilterType.Torso;
			break;
		case 4:
			result = ItemSortAndFilter.EquipFilterType.Bracers;
			break;
		case 5:
			result = ItemSortAndFilter.EquipFilterType.Boots;
			break;
		case 6:
			result = ItemSortAndFilter.EquipFilterType.Accessory;
			break;
		case 7:
			result = ItemSortAndFilter.EquipFilterType.Carrier;
			break;
		case 8:
			result = ItemSortAndFilter.EquipFilterType.LivestockCarrier;
			break;
		case 9:
			result = ItemSortAndFilter.EquipFilterType.BeastCarrier;
			break;
		case 10:
			result = ItemSortAndFilter.EquipFilterType.Pocket;
			break;
		default:
			result = ItemSortAndFilter.EquipFilterType.Invalid;
			break;
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x0600319D RID: 12701 RVA: 0x00188590 File Offset: 0x00186790
	public static ItemSortAndFilter.MedicineFilterType GetMedicineFilterType(ItemKey itemKey)
	{
		bool flag = itemKey.ItemType == 8;
		ItemSortAndFilter.MedicineFilterType result;
		if (flag)
		{
			MedicineItem config = Medicine.Instance[itemKey.TemplateId];
			bool flag2 = config.BuffAndOtherMedicine == 1;
			if (flag2)
			{
				result = ItemSortAndFilter.MedicineFilterType.Buff;
			}
			else
			{
				bool flag3 = config.BuffAndOtherMedicine == 2;
				if (flag3)
				{
					result = ItemSortAndFilter.MedicineFilterType.Other;
				}
				else
				{
					switch (config.EffectType)
					{
					case EMedicineEffectType.RecoverOuterInjury:
						return ItemSortAndFilter.MedicineFilterType.Outer;
					case EMedicineEffectType.RecoverInnerInjury:
						return ItemSortAndFilter.MedicineFilterType.Inner;
					case EMedicineEffectType.RecoverHealth:
						return ItemSortAndFilter.MedicineFilterType.Health;
					case EMedicineEffectType.ChangeDisorderOfQi:
						return ItemSortAndFilter.MedicineFilterType.Disorder;
					case EMedicineEffectType.DetoxPoison:
						return ItemSortAndFilter.MedicineFilterType.Detox;
					case EMedicineEffectType.ApplyPoison:
						return ItemSortAndFilter.MedicineFilterType.Poison;
					}
					result = ItemSortAndFilter.MedicineFilterType.Invalid;
				}
			}
		}
		else
		{
			bool flag4 = itemKey.ItemType == 5;
			if (flag4)
			{
				result = ItemSortAndFilter.MedicineFilterType.Other;
			}
			else
			{
				bool flag5 = ItemTemplateHelper.IsTianJieFuLu(itemKey.ItemType, itemKey.TemplateId);
				if (flag5)
				{
					result = ItemSortAndFilter.MedicineFilterType.Other;
				}
				else
				{
					result = ItemSortAndFilter.MedicineFilterType.Invalid;
				}
			}
		}
		return result;
	}

	// Token: 0x0600319E RID: 12702 RVA: 0x00188668 File Offset: 0x00186868
	public static ItemSortAndFilter.SpecialBreakFilterType GetSpecialBreakFilterType(ItemKey itemKey)
	{
		sbyte itemType = itemKey.ItemType;
		sbyte b = itemType;
		ItemSortAndFilter.SpecialBreakFilterType result;
		switch (b)
		{
		case 0:
			result = ItemSortAndFilter.SpecialBreakFilterType.Weapon;
			break;
		case 1:
			result = ItemSortAndFilter.SpecialBreakFilterType.Armor;
			break;
		case 2:
			result = ItemSortAndFilter.SpecialBreakFilterType.Accessory;
			break;
		default:
			if (b == 10)
			{
				short subType = ItemTemplateHelper.GetItemSubType(itemKey.ItemType, itemKey.TemplateId);
				bool flag = subType == 1001;
				if (flag)
				{
					result = ItemSortAndFilter.SpecialBreakFilterType.CombatSkillBook;
					break;
				}
				bool flag2 = subType == 1000;
				if (flag2)
				{
					result = ItemSortAndFilter.SpecialBreakFilterType.LifeSkillBook;
					break;
				}
			}
			result = ItemSortAndFilter.SpecialBreakFilterType.Invalid;
			break;
		}
		return result;
	}

	// Token: 0x0600319F RID: 12703 RVA: 0x001886EC File Offset: 0x001868EC
	public static ItemSortAndFilter.ClothingWeaverFilterType GetClothingWeaverFilterType(ItemKey itemKey)
	{
		bool flag = itemKey.ItemType == 3;
		ItemSortAndFilter.ClothingWeaverFilterType result;
		if (flag)
		{
			ClothingItem config = Clothing.Instance[itemKey.TemplateId];
			result = (ItemSortAndFilter.ClothingWeaverFilterType)config.WeaveType;
		}
		else
		{
			result = ItemSortAndFilter.ClothingWeaverFilterType.Invalid;
		}
		return result;
	}

	// Token: 0x060031A0 RID: 12704 RVA: 0x00188728 File Offset: 0x00186928
	public static List<ItemSortAndFilter.PoisonFilterType> GetPoisonFilterTypePoolList(ItemKey itemKey)
	{
		ItemSortAndFilter.<>c__DisplayClass104_0 CS$<>8__locals1;
		CS$<>8__locals1.list = EasyPool.Get<List<ItemSortAndFilter.PoisonFilterType>>();
		bool flag = itemKey.ItemType == 5;
		if (flag)
		{
			MaterialItem config = Config.Material.Instance[itemKey.TemplateId];
			ItemSortAndFilter.<GetPoisonFilterTypePoolList>g__AddToList|104_0(config.InnatePoisons, ref CS$<>8__locals1);
		}
		else
		{
			bool flag2 = itemKey.ItemType == 4;
			if (flag2)
			{
				CarrierItem config2 = Carrier.Instance[itemKey.TemplateId];
				ItemSortAndFilter.<GetPoisonFilterTypePoolList>g__AddToList|104_0(config2.InnatePoisons, ref CS$<>8__locals1);
			}
		}
		return CS$<>8__locals1.list;
	}

	// Token: 0x060031A1 RID: 12705 RVA: 0x001887B0 File Offset: 0x001869B0
	public static List<ItemSortAndFilter.EntertainFilterType> GetEntertainFilterTypePoolList(ItemKey itemKey)
	{
		List<ItemSortAndFilter.EntertainFilterType> list = EasyPool.Get<List<ItemSortAndFilter.EntertainFilterType>>();
		bool flag = itemKey.ItemType != 7;
		List<ItemSortAndFilter.EntertainFilterType> result;
		if (flag)
		{
			result = list;
		}
		else
		{
			FoodItem config = Food.Instance[itemKey.TemplateId];
			for (int i = 0; i < config.FoodType.Count; i++)
			{
				list.Add((ItemSortAndFilter.EntertainFilterType)config.FoodType[i]);
			}
			result = list;
		}
		return result;
	}

	// Token: 0x060031A2 RID: 12706 RVA: 0x00188824 File Offset: 0x00186A24
	public static ItemSortAndFilter.BookFilterType GetBookFilterType(ItemKey itemKey)
	{
		bool flag = itemKey.ItemType == 10;
		if (flag)
		{
			SkillBookItem config = SkillBook.Instance[itemKey.TemplateId];
			short itemSubType = config.ItemSubType;
			short num = itemSubType;
			if (num == 1000)
			{
				return ItemSortAndFilter.BookFilterType.Life;
			}
			if (num == 1001)
			{
				return ItemSortAndFilter.BookFilterType.Combat;
			}
		}
		return ItemSortAndFilter.BookFilterType.Invalid;
	}

	// Token: 0x060031A3 RID: 12707 RVA: 0x00188884 File Offset: 0x00186A84
	public static ItemSortAndFilter.MaterialFilterType GetMaterialFilterType(ItemKey itemKey)
	{
		bool flag = itemKey.ItemType == 5;
		if (flag)
		{
			switch (Config.Material.Instance[itemKey.TemplateId].ResourceType)
			{
			case 0:
				return ItemSortAndFilter.MaterialFilterType.Food;
			case 1:
				return ItemSortAndFilter.MaterialFilterType.Wood;
			case 2:
				return ItemSortAndFilter.MaterialFilterType.Metal;
			case 3:
				return ItemSortAndFilter.MaterialFilterType.Jade;
			case 4:
				return ItemSortAndFilter.MaterialFilterType.Fabric;
			case 5:
				return ItemSortAndFilter.MaterialFilterType.Herb;
			}
		}
		return ItemSortAndFilter.MaterialFilterType.Invalid;
	}

	// Token: 0x060031A4 RID: 12708 RVA: 0x00188900 File Offset: 0x00186B00
	public void SaveFilter()
	{
		this._lastItemFilterType.Clear();
		this._lastItemFilterType.AddRange(this.SortFilterSetting.ItemFilterType);
		this._lastEquipFilterType.Clear();
		this._lastEquipFilterType.AddRange(this.SortFilterSetting.EquipFilterType);
		this._lastMedicineFilterType.Clear();
		this._lastMedicineFilterType.AddRange(this.SortFilterSetting.MedicineFilterType);
		this._lastSpecialBreakFilterType.Clear();
		this._lastSpecialBreakFilterType.AddRange(this.SortFilterSetting.SpecialBreakFilterType);
	}

	// Token: 0x060031A5 RID: 12709 RVA: 0x0018899C File Offset: 0x00186B9C
	public void LoadFilter()
	{
		bool flag = this._lastItemFilterType.Count == ItemSortAndFilter.ItemFilterType.Count.ToInt() - 1;
		if (flag)
		{
			this.SetFilterType(ItemSortAndFilter.ItemFilterType.Invalid, true);
		}
		else
		{
			foreach (ItemSortAndFilter.ItemFilterType filterType in this._lastItemFilterType)
			{
				this.SetFilterType(filterType, true);
			}
		}
		bool flag2 = this._lastEquipFilterType.Count == ItemSortAndFilter.EquipFilterType.Count.ToInt() - 1;
		if (flag2)
		{
			this.SetEquipFilterType(ItemSortAndFilter.EquipFilterType.Invalid, true);
		}
		else
		{
			foreach (ItemSortAndFilter.EquipFilterType filterType2 in this._lastEquipFilterType)
			{
				this.SetEquipFilterType(filterType2, true);
			}
		}
		bool flag3 = this._lastMedicineFilterType.Count == ItemSortAndFilter.MedicineFilterType.Count.ToInt() - 1;
		if (flag3)
		{
			this.SetMedicineFilterType(ItemSortAndFilter.MedicineFilterType.Invalid, true);
		}
		else
		{
			foreach (ItemSortAndFilter.MedicineFilterType filterType3 in this._lastMedicineFilterType)
			{
				this.SetMedicineFilterType(filterType3, true);
			}
		}
		bool flag4 = this._lastSpecialBreakFilterType.Count == ItemSortAndFilter.SpecialBreakFilterType.Count.ToInt() - 1;
		if (flag4)
		{
			this.SetSpecialBreakFilterType(ItemSortAndFilter.SpecialBreakFilterType.Invalid, true);
		}
		else
		{
			foreach (ItemSortAndFilter.SpecialBreakFilterType filterType4 in this._lastSpecialBreakFilterType)
			{
				this.SetSpecialBreakFilterType(filterType4, true);
			}
		}
		this._lastItemFilterType.Clear();
		this._lastEquipFilterType.Clear();
		this._lastMedicineFilterType.Clear();
		this._lastSpecialBreakFilterType.Clear();
	}

	// Token: 0x060031A6 RID: 12710 RVA: 0x00188BAC File Offset: 0x00186DAC
	public void ShowClothingWaveFilter(bool show)
	{
		this._clothingWeaveFilterTogGroup.gameObject.SetActive(show);
		this._filterTogGroup.gameObject.SetActive(!show);
	}

	// Token: 0x060031A7 RID: 12711 RVA: 0x00188BD8 File Offset: 0x00186DD8
	private static bool IsToggleActive(CToggleObsolete tog)
	{
		return tog.gameObject.activeSelf && tog.interactable && tog.enabled;
	}

	// Token: 0x060031A8 RID: 12712 RVA: 0x00188C08 File Offset: 0x00186E08
	public void SwitchFilterGroupActive(string filterGroupName)
	{
		foreach (string referName in this.Names)
		{
			bool flag = referName.Equals("ItemSort");
			if (!flag)
			{
				CToggleGroupObsolete toggleGroup = base.CGet<CToggleGroupObsolete>(referName);
				if (toggleGroup != null)
				{
					toggleGroup.gameObject.SetActive(false);
				}
			}
		}
		CToggleGroupObsolete filter = base.CGet<CToggleGroupObsolete>(filterGroupName);
		filter.gameObject.SetActive(true);
	}

	// Token: 0x060031AA RID: 12714 RVA: 0x00188D60 File Offset: 0x00186F60
	[CompilerGenerated]
	internal static int <ItemCompare>g__ComparePoison|98_0(ref ItemSortAndFilter.<>c__DisplayClass98_0 A_0)
	{
		bool flag = A_0.itemL.HasAnyPoison != A_0.itemR.HasAnyPoison;
		int result;
		if (flag)
		{
			result = (A_0.itemL.HasAnyPoison ? -1 : 1);
		}
		else
		{
			bool hasAnyPoison = A_0.itemL.HasAnyPoison;
			if (hasAnyPoison)
			{
				bool flag2 = A_0.itemL.PoisonIsIdentified != A_0.itemR.PoisonIsIdentified;
				if (flag2)
				{
					return A_0.itemL.PoisonIsIdentified ? -1 : 1;
				}
				FullPoisonEffects poisonEffects = A_0.itemL.PoisonEffects;
				int poisonCountL = (poisonEffects != null) ? poisonEffects.GetTotalPoisonCount() : 0;
				FullPoisonEffects poisonEffects2 = A_0.itemR.PoisonEffects;
				int poisonCountR = (poisonEffects2 != null) ? poisonEffects2.GetTotalPoisonCount() : 0;
				bool flag3 = poisonCountL != poisonCountR;
				if (flag3)
				{
					return poisonCountR.CompareTo(poisonCountL);
				}
				FullPoisonEffects poisonEffects3 = A_0.itemL.PoisonEffects;
				int poisonGradeL = (poisonEffects3 != null) ? poisonEffects3.GetMaxGrade() : 0;
				FullPoisonEffects poisonEffects4 = A_0.itemR.PoisonEffects;
				int poisonGradeR = (poisonEffects4 != null) ? poisonEffects4.GetMaxGrade() : 0;
				bool flag4 = poisonGradeL != poisonGradeR;
				if (flag4)
				{
					return poisonGradeR.CompareTo(poisonGradeL);
				}
			}
			result = 0;
		}
		return result;
	}

	// Token: 0x060031AB RID: 12715 RVA: 0x00188E8C File Offset: 0x0018708C
	[CompilerGenerated]
	internal unsafe static void <GetPoisonFilterTypePoolList>g__AddToList|104_0(PoisonsAndLevels poisonsAndLevels, ref ItemSortAndFilter.<>c__DisplayClass104_0 A_1)
	{
		for (int i = 0; i < 6; i++)
		{
			short value = *(ref poisonsAndLevels.Values.FixedElementField + (IntPtr)i * 2);
			bool flag = value > 0;
			if (flag)
			{
				ItemSortAndFilter.PoisonFilterType type = i + ItemSortAndFilter.PoisonFilterType.Hot;
				A_1.list.Add(type);
			}
		}
	}

	// Token: 0x04002437 RID: 9271
	private List<ItemDisplayData> _itemList;

	// Token: 0x04002438 RID: 9272
	private Action _onItemListChanged;

	// Token: 0x04002439 RID: 9273
	private Action _onViewTypeChanged;

	// Token: 0x0400243A RID: 9274
	private bool _inited = false;

	// Token: 0x0400243B RID: 9275
	[NonSerialized]
	public ItemSortFilterSetting SortFilterSetting;

	// Token: 0x0400243C RID: 9276
	private readonly List<ItemSortAndFilter.ItemFilterType> _lastItemFilterType = new List<ItemSortAndFilter.ItemFilterType>();

	// Token: 0x0400243D RID: 9277
	private readonly List<ItemSortAndFilter.EquipFilterType> _lastEquipFilterType = new List<ItemSortAndFilter.EquipFilterType>();

	// Token: 0x0400243E RID: 9278
	private readonly List<ItemSortAndFilter.MedicineFilterType> _lastMedicineFilterType = new List<ItemSortAndFilter.MedicineFilterType>();

	// Token: 0x0400243F RID: 9279
	private readonly List<ItemSortAndFilter.SpecialBreakFilterType> _lastSpecialBreakFilterType = new List<ItemSortAndFilter.SpecialBreakFilterType>();

	// Token: 0x04002440 RID: 9280
	public readonly List<ItemDisplayData> OutputItemList = new List<ItemDisplayData>();

	// Token: 0x04002441 RID: 9281
	[NonSerialized]
	public readonly List<ItemKey> StaticAheadItemKeysList = new List<ItemKey>();

	// Token: 0x04002443 RID: 9283
	private RectTransform _sortBtnHolder;

	// Token: 0x04002444 RID: 9284
	private CToggleGroupObsolete _filterTogGroup;

	// Token: 0x04002445 RID: 9285
	private CToggleGroupObsolete _equipFilterTogGroup;

	// Token: 0x04002446 RID: 9286
	private CToggleGroupObsolete _medicineFilterTogGroup;

	// Token: 0x04002447 RID: 9287
	private CToggleGroupObsolete _specialBreakFilterTogGroup;

	// Token: 0x04002448 RID: 9288
	private CToggleGroupObsolete _clothingWeaveFilterTogGroup;

	// Token: 0x04002449 RID: 9289
	private CToggleGroupObsolete _poisonFilterTogGroup;

	// Token: 0x0400244A RID: 9290
	private CToggleGroupObsolete _entertainFilterTogGroup;

	// Token: 0x0400244B RID: 9291
	private CToggleGroupObsolete _bookFilterTogGroup;

	// Token: 0x0400244C RID: 9292
	private CToggleGroupObsolete _materialFilterTogGroup;

	// Token: 0x0400244D RID: 9293
	private CToggleGroupObsolete _itemSortTogGroup;

	// Token: 0x0400244E RID: 9294
	private bool _filterTogInitializing = false;

	// Token: 0x0400244F RID: 9295
	private bool _equipTogInitializing = false;

	// Token: 0x04002450 RID: 9296
	private bool _medicineTogInitializing = false;

	// Token: 0x04002451 RID: 9297
	private bool _specialBreakTogInitializing = false;

	// Token: 0x04002452 RID: 9298
	private bool _clothingWeaveTogInitializing = false;

	// Token: 0x04002453 RID: 9299
	private bool _poisonTogInitializing = false;

	// Token: 0x04002454 RID: 9300
	private bool _entertainTogInitializing = false;

	// Token: 0x04002455 RID: 9301
	private bool _bookTogInitializing = false;

	// Token: 0x04002456 RID: 9302
	private bool _materialTogInitializing = false;

	// Token: 0x04002457 RID: 9303
	private bool _disableStyleViewSwitch = false;

	// Token: 0x04002458 RID: 9304
	[NonSerialized]
	public bool SortEnabled = true;

	// Token: 0x04002459 RID: 9305
	[NonSerialized]
	public bool IsOnRemovePoison;

	// Token: 0x0400245A RID: 9306
	[NonSerialized]
	public bool IsOnAddPoison;

	// Token: 0x0400245B RID: 9307
	[NonSerialized]
	public bool AllTypeIncludeInactive = false;

	// Token: 0x0400245C RID: 9308
	private readonly List<sbyte> _poisonTypeList = new List<sbyte>();

	// Token: 0x0400245D RID: 9309
	[NonSerialized]
	public Func<ItemDisplayData, bool> SetItemInteraction;

	// Token: 0x0400245E RID: 9310
	[NonSerialized]
	public bool IsOnSpecialBreakMultiplySelect;

	// Token: 0x0400245F RID: 9311
	private short _specialBreakSelectedBonusId = -1;

	// Token: 0x020016E5 RID: 5861
	public enum SortType
	{
		// Token: 0x0400A98B RID: 43403
		Invalid = -1,
		// Token: 0x0400A98C RID: 43404
		Name,
		// Token: 0x0400A98D RID: 43405
		Grade,
		// Token: 0x0400A98E RID: 43406
		Value,
		// Token: 0x0400A98F RID: 43407
		Weight,
		// Token: 0x0400A990 RID: 43408
		Count
	}

	// Token: 0x020016E6 RID: 5862
	public enum ItemFilterType
	{
		// Token: 0x0400A992 RID: 43410
		Invalid,
		// Token: 0x0400A993 RID: 43411
		Food,
		// Token: 0x0400A994 RID: 43412
		Medicine,
		// Token: 0x0400A995 RID: 43413
		Equip,
		// Token: 0x0400A996 RID: 43414
		Book,
		// Token: 0x0400A997 RID: 43415
		Make,
		// Token: 0x0400A998 RID: 43416
		Material,
		// Token: 0x0400A999 RID: 43417
		Other,
		// Token: 0x0400A99A RID: 43418
		Count
	}

	// Token: 0x020016E7 RID: 5863
	public enum EntertainFilterType
	{
		// Token: 0x0400A99C RID: 43420
		Invalid = -1,
		// Token: 0x0400A99D RID: 43421
		Tea = 11,
		// Token: 0x0400A99E RID: 43422
		Wine,
		// Token: 0x0400A99F RID: 43423
		Vegetarian = 15,
		// Token: 0x0400A9A0 RID: 43424
		Fruit = 4,
		// Token: 0x0400A9A1 RID: 43425
		Meat = 14,
		// Token: 0x0400A9A2 RID: 43426
		Bird = 0,
		// Token: 0x0400A9A3 RID: 43427
		Beast,
		// Token: 0x0400A9A4 RID: 43428
		Fish,
		// Token: 0x0400A9A5 RID: 43429
		Count = 8
	}

	// Token: 0x020016E8 RID: 5864
	public enum EquipFilterType
	{
		// Token: 0x0400A9A7 RID: 43431
		Invalid,
		// Token: 0x0400A9A8 RID: 43432
		Weapon,
		// Token: 0x0400A9A9 RID: 43433
		Helm,
		// Token: 0x0400A9AA RID: 43434
		Torso,
		// Token: 0x0400A9AB RID: 43435
		Bracers,
		// Token: 0x0400A9AC RID: 43436
		Boots,
		// Token: 0x0400A9AD RID: 43437
		Accessory,
		// Token: 0x0400A9AE RID: 43438
		Clothing,
		// Token: 0x0400A9AF RID: 43439
		Carrier,
		// Token: 0x0400A9B0 RID: 43440
		LivestockCarrier,
		// Token: 0x0400A9B1 RID: 43441
		BeastCarrier,
		// Token: 0x0400A9B2 RID: 43442
		Pocket,
		// Token: 0x0400A9B3 RID: 43443
		Count
	}

	// Token: 0x020016E9 RID: 5865
	public enum MedicineFilterType
	{
		// Token: 0x0400A9B5 RID: 43445
		Invalid,
		// Token: 0x0400A9B6 RID: 43446
		Outer,
		// Token: 0x0400A9B7 RID: 43447
		Inner,
		// Token: 0x0400A9B8 RID: 43448
		Detox,
		// Token: 0x0400A9B9 RID: 43449
		Poison,
		// Token: 0x0400A9BA RID: 43450
		Disorder,
		// Token: 0x0400A9BB RID: 43451
		Health,
		// Token: 0x0400A9BC RID: 43452
		Buff,
		// Token: 0x0400A9BD RID: 43453
		Other,
		// Token: 0x0400A9BE RID: 43454
		Count
	}

	// Token: 0x020016EA RID: 5866
	public enum SpecialBreakFilterType
	{
		// Token: 0x0400A9C0 RID: 43456
		Invalid,
		// Token: 0x0400A9C1 RID: 43457
		CombatSkillBook,
		// Token: 0x0400A9C2 RID: 43458
		LifeSkillBook,
		// Token: 0x0400A9C3 RID: 43459
		Weapon,
		// Token: 0x0400A9C4 RID: 43460
		Armor,
		// Token: 0x0400A9C5 RID: 43461
		Accessory,
		// Token: 0x0400A9C6 RID: 43462
		Count
	}

	// Token: 0x020016EB RID: 5867
	public enum ClothingWeaverFilterType
	{
		// Token: 0x0400A9C8 RID: 43464
		Invalid,
		// Token: 0x0400A9C9 RID: 43465
		Normal,
		// Token: 0x0400A9CA RID: 43466
		Sect,
		// Token: 0x0400A9CB RID: 43467
		Village,
		// Token: 0x0400A9CC RID: 43468
		Other,
		// Token: 0x0400A9CD RID: 43469
		Count
	}

	// Token: 0x020016EC RID: 5868
	public enum PoisonFilterType
	{
		// Token: 0x0400A9CF RID: 43471
		Invalid,
		// Token: 0x0400A9D0 RID: 43472
		Hot,
		// Token: 0x0400A9D1 RID: 43473
		Gloomy,
		// Token: 0x0400A9D2 RID: 43474
		Cold,
		// Token: 0x0400A9D3 RID: 43475
		Red,
		// Token: 0x0400A9D4 RID: 43476
		Rotten,
		// Token: 0x0400A9D5 RID: 43477
		Illusory,
		// Token: 0x0400A9D6 RID: 43478
		Count
	}

	// Token: 0x020016ED RID: 5869
	public enum BookFilterType
	{
		// Token: 0x0400A9D8 RID: 43480
		Invalid,
		// Token: 0x0400A9D9 RID: 43481
		Combat,
		// Token: 0x0400A9DA RID: 43482
		Life,
		// Token: 0x0400A9DB RID: 43483
		Count
	}

	// Token: 0x020016EE RID: 5870
	public enum MaterialFilterType
	{
		// Token: 0x0400A9DD RID: 43485
		Invalid,
		// Token: 0x0400A9DE RID: 43486
		Food,
		// Token: 0x0400A9DF RID: 43487
		Wood,
		// Token: 0x0400A9E0 RID: 43488
		Metal,
		// Token: 0x0400A9E1 RID: 43489
		Jade,
		// Token: 0x0400A9E2 RID: 43490
		Fabric,
		// Token: 0x0400A9E3 RID: 43491
		Herb,
		// Token: 0x0400A9E4 RID: 43492
		Count
	}

	// Token: 0x020016EF RID: 5871
	public enum LockFilterTypeToggleActionMode
	{
		// Token: 0x0400A9E6 RID: 43494
		Default,
		// Token: 0x0400A9E7 RID: 43495
		NoActionIfPossible
	}
}
