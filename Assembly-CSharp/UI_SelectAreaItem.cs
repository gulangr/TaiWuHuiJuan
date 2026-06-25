using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Taiwu.Display;
using GameData.Domains.Taiwu.Display.VillagerRoleArrangement;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

// Token: 0x0200039E RID: 926
public class UI_SelectAreaItem : UIBase
{
	// Token: 0x060037B5 RID: 14261 RVA: 0x001C0A44 File Offset: 0x001BEC44
	public override void OnInit(ArgumentBox argsBox)
	{
		this._selectedItem = new TemplateKey(-1, -1);
		bool flag = !this._initialized;
		if (flag)
		{
			this.InitRefers();
			this._itemTwoLevelFilter.Init();
			this.InitFilter();
			this.InitSort();
			InfinityScrollLegacy itemScrollView = this._itemScrollView;
			itemScrollView.OnItemRender = (Action<int, Refers>)Delegate.Combine(itemScrollView.OnItemRender, new Action<int, Refers>(this.OnItemRender));
			this.InitSearch();
			this.InitSimpleOrDetailToggles();
		}
		this.ReadArgs(argsBox);
		this.InitAreaItems();
		this._initialized = true;
		this.OnFilterOutputChanged();
		this.RefreshButtons();
		this.Element.ShowAfterRefresh();
	}

	// Token: 0x060037B6 RID: 14262 RVA: 0x001C0AF4 File Offset: 0x001BECF4
	private void ReadArgs(ArgumentBox argsBox)
	{
		bool flag = argsBox == null;
		if (!flag)
		{
			argsBox.Get<Action<TemplateKey>>("OnSelectItem", out this._onSelectItem);
			argsBox.Get<Action>("OnCancel", out this._onCancel);
			argsBox.Get("IsMerchantMode", out this._isMerchantMode);
			bool isMerchantMode = this._isMerchantMode;
			if (isMerchantMode)
			{
				argsBox.Get<Dictionary<int, VillagerRoleCharacterDisplayData>>("VillagerCharacterDisplayDataDict", out this._villagerCharacterDisplayDataDict);
				argsBox.Get<List<VillagerRoleManageDisplayData>>("RoleManageDisplayList", out this._roleManageDisplayList);
				argsBox.Get("ArrangementId", out this._arrangementId);
				argsBox.Get("AreaId", out this._areaId);
			}
			argsBox.Get("IsForceSelect", out this._isForceSelect);
		}
	}

	// Token: 0x060037B7 RID: 14263 RVA: 0x001C0BAC File Offset: 0x001BEDAC
	protected override void OnClick(Transform btn)
	{
		bool flag = btn.name == "Confirm";
		if (flag)
		{
			Action<TemplateKey> onSelectItem = this._onSelectItem;
			if (onSelectItem != null)
			{
				onSelectItem(this._selectedItem);
			}
			base.QuickHide();
		}
		else
		{
			bool flag2 = btn.name == "Cancel";
			if (flag2)
			{
				Action onCancel = this._onCancel;
				if (onCancel != null)
				{
					onCancel();
				}
				base.QuickHide();
			}
		}
	}

	// Token: 0x060037B8 RID: 14264 RVA: 0x001C0C20 File Offset: 0x001BEE20
	public override void QuickHide()
	{
		bool isForceSelect = this._isForceSelect;
		if (!isForceSelect)
		{
			Action onCancel = this._onCancel;
			if (onCancel != null)
			{
				onCancel();
			}
			base.QuickHide();
		}
	}

	// Token: 0x060037B9 RID: 14265 RVA: 0x001C0C54 File Offset: 0x001BEE54
	private void InitSearch()
	{
		this._searchByName.onValueChanged.AddListener(delegate(string text)
		{
			this._searchInputText = text;
			this.OnFilterOutputChanged();
		});
	}

	// Token: 0x060037BA RID: 14266 RVA: 0x001C0C74 File Offset: 0x001BEE74
	private void InitSimpleOrDetailToggles()
	{
		this._detailToggle.onValueChanged.AddListener(delegate(bool isOn)
		{
			if (isOn)
			{
				this._itemScrollView.UpdateStyle(InfinityScrollLegacy.ScrollDirection.FromTop, 6, new Vector2(8f, 3f), new Vector2(0f, 0f), this._areaItemDetailView);
			}
		});
		this._simpleToggle.onValueChanged.AddListener(delegate(bool isOn)
		{
			if (isOn)
			{
				this._itemScrollView.UpdateStyle(InfinityScrollLegacy.ScrollDirection.FromTop, 13, new Vector2(8f, 3f), new Vector2(0f, 0f), this._areaItemView);
			}
		});
	}

	// Token: 0x060037BB RID: 14267 RVA: 0x001C0CB1 File Offset: 0x001BEEB1
	private void InitSort()
	{
		this._simpleItemSort.Init();
		this._simpleItemSort.SetCallback(new Action(this.OnSortChanged));
	}

	// Token: 0x060037BC RID: 14268 RVA: 0x001C0CD8 File Offset: 0x001BEED8
	private void InitFilter()
	{
		this._itemTwoLevelFilter.SetCallback(new Action(this.OnFilterOutputChanged));
		this._itemTwoLevelFilter.SetFilterType(SimpleItemMainFilter.ItemFilterType.All);
	}

	// Token: 0x060037BD RID: 14269 RVA: 0x001C0D00 File Offset: 0x001BEF00
	private void OnSortChanged()
	{
		this.FilterAndSort();
	}

	// Token: 0x060037BE RID: 14270 RVA: 0x001C0D0A File Offset: 0x001BEF0A
	private void OnFilterOutputChanged()
	{
		this.FilterAndSort();
		this.RefreshSplitterWidth();
		this.RefreshSubFilterPosition();
	}

	// Token: 0x060037BF RID: 14271 RVA: 0x001C0D24 File Offset: 0x001BEF24
	private void RefreshSplitterWidth()
	{
		int baseWidth = 54;
		int space = 4;
		int extraWidth = 32;
		int activeChildCount = this._itemTwoLevelFilter.GetSubFilterActiveToggleCount();
		bool flag = activeChildCount == 0;
		if (flag)
		{
			this._filterSplitter.gameObject.SetActive(false);
		}
		else
		{
			this._filterSplitter.gameObject.SetActive(true);
			int width = extraWidth + baseWidth * activeChildCount + space * Math.Max(0, activeChildCount - 1);
			this._filterSplitter.sizeDelta = new Vector2((float)width, this._filterSplitter.sizeDelta.y);
		}
	}

	// Token: 0x060037C0 RID: 14272 RVA: 0x001C0DB0 File Offset: 0x001BEFB0
	private void RefreshSubFilterPosition()
	{
		RectTransform selectedToggleTransform = this._itemTwoLevelFilter.GetSelectedMainFilterToggleTransform();
		RectTransform target = this._filterSplitter;
		RectTransform target2 = this._itemTwoLevelFilter.SubFilterTransform;
		float worldX = selectedToggleTransform.position.x;
		Transform subFilterParent = target2.parent;
		Vector3 localPos = subFilterParent.InverseTransformPoint(new Vector3(worldX, 0f, 0f));
		target.localPosition = new Vector3(localPos.x, target.localPosition.y, 0f);
		target2.localPosition = new Vector3(localPos.x, target2.localPosition.y, 0f);
	}

	// Token: 0x060037C1 RID: 14273 RVA: 0x001C0E50 File Offset: 0x001BF050
	private void FilterAndSort()
	{
		this._displayingAreaItems = this._areaItems.Where(new Func<AreaItemKey, bool>(this.FilterItem)).ToList<AreaItemKey>();
		this._displayingAreaItems.Sort(new Comparison<AreaItemKey>(this.Compare));
		this.RefreshItemScroll();
	}

	// Token: 0x060037C2 RID: 14274 RVA: 0x001C0EA0 File Offset: 0x001BF0A0
	private int Compare(AreaItemKey itemKeyL, AreaItemKey itemKeyR)
	{
		string nameL = itemKeyL.GetName();
		string nameR = itemKeyR.GetName();
		sbyte gradeL = itemKeyL.GetGrade();
		sbyte gradeR = itemKeyR.GetGrade();
		int valueL = itemKeyL.GetValue();
		int valueR = itemKeyR.GetValue();
		int weightL = itemKeyL.GetWeight();
		int weightR = itemKeyR.GetWeight();
		SimpleItemSort.ItemCompareItem compareItemL = new SimpleItemSort.ItemCompareItem
		{
			Name = nameL,
			Grade = gradeL,
			Value = valueL,
			Weight = weightL
		};
		SimpleItemSort.ItemCompareItem compareItemR = new SimpleItemSort.ItemCompareItem
		{
			Name = nameR,
			Grade = gradeR,
			Value = valueR,
			Weight = weightR
		};
		int compareBySortItem = this._simpleItemSort.CompareItem(compareItemL, compareItemR);
		bool flag = compareBySortItem != 0;
		int result;
		if (flag)
		{
			result = compareBySortItem;
		}
		else
		{
			result = itemKeyL.TemplateId.CompareTo(itemKeyR.TemplateId);
		}
		return result;
	}

	// Token: 0x060037C3 RID: 14275 RVA: 0x001C0F88 File Offset: 0x001BF188
	private bool FilterItem(AreaItemKey itemKey)
	{
		bool flag = !this._itemTwoLevelFilter.IsItemMatch(itemKey.Type, itemKey.TemplateId);
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = !this._searchInputText.IsNullOrEmpty() && !itemKey.GetName().Contains(this._searchInputText);
			result = !flag2;
		}
		return result;
	}

	// Token: 0x060037C4 RID: 14276 RVA: 0x001C0FEA File Offset: 0x001BF1EA
	private void RefreshItemScroll()
	{
		this._itemScrollView.SetDataCount(this._displayingAreaItems.Count);
	}

	// Token: 0x060037C5 RID: 14277 RVA: 0x001C1004 File Offset: 0x001BF204
	private void OnItemRender(int index, Refers refers)
	{
		AreaItemKey itemKey = this._displayingAreaItems[index];
		AreaItemView itemView = refers as AreaItemView;
		TemplateKey templateKey = itemKey.ToTemplateKey();
		bool isSelect = templateKey.ItemType == this._selectedItem.ItemType && templateKey.TemplateId == this._selectedItem.TemplateId;
		itemView.Refresh(templateKey, itemKey.Amount, new Action<TemplateKey>(this.OnItemClick), isSelect);
		this.RefreshArrangementIcon(itemView, itemKey);
	}

	// Token: 0x060037C6 RID: 14278 RVA: 0x001C107C File Offset: 0x001BF27C
	private void RefreshArrangementIcon(AreaItemView itemView, AreaItemKey itemKey)
	{
		bool flag = !this._isMerchantMode;
		if (flag)
		{
			itemView.RefreshArrangementIcon(false, null);
			itemView.SetButtonInteractable(true);
		}
		else
		{
			VillagerRoleManageDisplayData roleDisplayData = this._roleManageDisplayList[3];
			bool flag2 = roleDisplayData.CharacterIds == null;
			if (flag2)
			{
				itemView.RefreshArrangementIcon(false, null);
				itemView.SetButtonInteractable(true);
			}
			else
			{
				foreach (int charId in roleDisplayData.CharacterIds)
				{
					bool flag3 = this.RefreshArrangementIcon(itemView, itemKey, charId);
					if (flag3)
					{
						return;
					}
				}
				itemView.RefreshArrangementIcon(false, null);
				itemView.SetButtonInteractable(true);
			}
		}
	}

	// Token: 0x060037C7 RID: 14279 RVA: 0x001C1144 File Offset: 0x001BF344
	private bool RefreshArrangementIcon(AreaItemView itemView, AreaItemKey itemKey, int charId)
	{
		return false;
	}

	// Token: 0x060037C8 RID: 14280 RVA: 0x001C1148 File Offset: 0x001BF348
	private bool RfRefreshArrangementIconPriceSuppresion(AreaItemView itemView, AreaItemKey itemKey, VillagerRoleCharacterDisplayData displayData)
	{
		VillagerRoleArrangementDisplayDataWrapper displayDataWrapper = displayData.ArrangementDisplayData;
		PriceSuppressionDisplayData arrangementData = displayDataWrapper.ArrangementData as PriceSuppressionDisplayData;
		bool flag = arrangementData.SuppressionItem.ItemType == itemKey.Type && arrangementData.SuppressionItem.TemplateId == itemKey.TemplateId && this._areaId == displayDataWrapper.AreaId;
		bool result;
		if (flag)
		{
			itemView.RefreshArrangementIcon(true, VillagerRoleArrangement.Instance[displayDataWrapper.ArrangementTemplateId].DisplayIcon2);
			itemView.SetButtonInteractable(true);
			result = true;
		}
		else
		{
			result = false;
		}
		return result;
	}

	// Token: 0x060037C9 RID: 14281 RVA: 0x001C11D4 File Offset: 0x001BF3D4
	private bool RefreshArrangementIconPriceGouging(AreaItemView itemView, AreaItemKey itemKey, VillagerRoleCharacterDisplayData displayData)
	{
		VillagerRoleArrangementDisplayDataWrapper displayDataWrapper = displayData.ArrangementDisplayData;
		PriceGougingDisplayData arrangementData = displayDataWrapper.ArrangementData as PriceGougingDisplayData;
		bool flag = arrangementData.GougingItem.ItemType == itemKey.Type && arrangementData.GougingItem.TemplateId == itemKey.TemplateId && this._areaId == displayDataWrapper.AreaId;
		bool result;
		if (flag)
		{
			itemView.RefreshArrangementIcon(true, VillagerRoleArrangement.Instance[displayDataWrapper.ArrangementTemplateId].DisplayIcon2);
			bool interactable = true;
			itemView.SetButtonInteractable(interactable);
			result = true;
		}
		else
		{
			result = false;
		}
		return result;
	}

	// Token: 0x060037CA RID: 14282 RVA: 0x001C1264 File Offset: 0x001BF464
	private void RefreshButtons()
	{
		bool isSelected = this._selectedItem.TemplateId != -1;
		this._confirm.interactable = isSelected;
		this._cancel.interactable = true;
		this._cancel.gameObject.SetActive(!this._isForceSelect);
	}

	// Token: 0x060037CB RID: 14283 RVA: 0x001C12B8 File Offset: 0x001BF4B8
	private void OnItemClick(TemplateKey key)
	{
		int lastIndex = this._displayingAreaItems.FindIndex((AreaItemKey itemKey) => itemKey.TemplateId == this._selectedItem.TemplateId && itemKey.Type == this._selectedItem.ItemType);
		this._selectedItem = key;
		int newIndex = this._displayingAreaItems.FindIndex((AreaItemKey itemKey) => itemKey.TemplateId == key.TemplateId && itemKey.Type == key.ItemType);
		bool flag = newIndex != -1;
		if (flag)
		{
			this._itemScrollView.RefreshCell(newIndex);
		}
		bool flag2 = lastIndex != -1;
		if (flag2)
		{
			this._itemScrollView.RefreshCell(lastIndex);
		}
		this.RefreshButtons();
	}

	// Token: 0x060037CC RID: 14284 RVA: 0x001C1350 File Offset: 0x001BF550
	private void InitAreaItems()
	{
		this._areaItems.Clear();
		List<TemplateKeyAndCount> templateKeyAndCounts = new List<TemplateKeyAndCount>();
		CharacterDomainMethod.AsyncCall.GetAllItemsByAreaId(this, this._areaId, delegate(int offset, RawDataPool dataPool)
		{
			Serializer.Deserialize(dataPool, offset, ref templateKeyAndCounts);
			foreach (TemplateKeyAndCount itemKeyAndCount in templateKeyAndCounts)
			{
				TemplateKey templateKey = itemKeyAndCount.TemplateKey;
				bool flag = templateKey.ItemType == 4;
				if (flag)
				{
					CarrierItem config = Carrier.Instance[templateKey.TemplateId];
					short subType = config.ItemSubType;
					bool flag2 = subType == 401 || subType == 402 || subType == 403;
					if (flag2)
					{
						continue;
					}
				}
				this._areaItems.Add(new AreaItemKey(templateKey.ItemType, templateKey.TemplateId, itemKeyAndCount.Count));
				this.FilterAndSort();
			}
		});
	}

	// Token: 0x060037CD RID: 14285 RVA: 0x001C139C File Offset: 0x001BF59C
	private void InitRefers()
	{
		this._itemTwoLevelFilter = base.CGet<ItemTwoLevelFilter>("ItemTwoLevelFilter");
		this._itemScrollView = base.CGet<InfinityScrollLegacy>("ItemScrollView");
		this._simpleItemSort = base.CGet<SimpleItemSort>("SimpleItemSort");
		this._areaItemView = base.CGet<AreaItemView>("AreaItemView");
		this._areaItemDetailView = base.CGet<AreaItemView>("AreaItemDetailView");
		this._detailToggle = base.CGet<CToggleObsolete>("DetailToggle");
		this._simpleToggle = base.CGet<CToggleObsolete>("SimpleToggle");
		this._searchByName = base.CGet<DelayedInputFieldEvent>("SearchByName");
		this._filterSplitter = base.CGet<RectTransform>("FilterSplitter");
		this._confirm = base.CGet<CButtonObsolete>("Confirm");
		this._cancel = base.CGet<CButtonObsolete>("Cancel");
	}

	// Token: 0x04002849 RID: 10313
	private bool _initialized;

	// Token: 0x0400284A RID: 10314
	private readonly List<AreaItemKey> _areaItems = new List<AreaItemKey>();

	// Token: 0x0400284B RID: 10315
	private List<AreaItemKey> _displayingAreaItems;

	// Token: 0x0400284C RID: 10316
	private string _searchInputText;

	// Token: 0x0400284D RID: 10317
	private Action<TemplateKey> _onSelectItem;

	// Token: 0x0400284E RID: 10318
	private Action _onCancel;

	// Token: 0x0400284F RID: 10319
	private TemplateKey _selectedItem;

	// Token: 0x04002850 RID: 10320
	private bool _isMerchantMode;

	// Token: 0x04002851 RID: 10321
	private bool _isForceSelect;

	// Token: 0x04002852 RID: 10322
	private Dictionary<int, VillagerRoleCharacterDisplayData> _villagerCharacterDisplayDataDict;

	// Token: 0x04002853 RID: 10323
	private List<VillagerRoleManageDisplayData> _roleManageDisplayList;

	// Token: 0x04002854 RID: 10324
	private short _arrangementId;

	// Token: 0x04002855 RID: 10325
	private short _areaId;

	// Token: 0x04002856 RID: 10326
	private ItemTwoLevelFilter _itemTwoLevelFilter;

	// Token: 0x04002857 RID: 10327
	private InfinityScrollLegacy _itemScrollView;

	// Token: 0x04002858 RID: 10328
	private SimpleItemSort _simpleItemSort;

	// Token: 0x04002859 RID: 10329
	private AreaItemView _areaItemView;

	// Token: 0x0400285A RID: 10330
	private AreaItemView _areaItemDetailView;

	// Token: 0x0400285B RID: 10331
	private CToggleObsolete _detailToggle;

	// Token: 0x0400285C RID: 10332
	private CToggleObsolete _simpleToggle;

	// Token: 0x0400285D RID: 10333
	private DelayedInputFieldEvent _searchByName;

	// Token: 0x0400285E RID: 10334
	private RectTransform _filterSplitter;

	// Token: 0x0400285F RID: 10335
	private CButtonObsolete _confirm;

	// Token: 0x04002860 RID: 10336
	private CButtonObsolete _cancel;
}
