using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace GearMate
{
	// Token: 0x02000618 RID: 1560
	public class GearMateItemSelector : Refers
	{
		// Token: 0x06004944 RID: 18756 RVA: 0x00224870 File Offset: 0x00222A70
		public void Init(UI_GearMate uiGearMate)
		{
			this._parent = uiGearMate;
			this.InitRefers();
			this._itemSrouceToggleGroup.InitPreOnToggle(-1);
			this._itemSrouceToggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnItemSourceToggleChange);
			foreach (ItemSourceType sourceType in this._itemSourceTypeArray)
			{
				this._itemDict[sourceType] = new List<ItemDisplayData>();
				foreach (Dictionary<ItemSourceType, Dictionary<ItemKey, int>> dict in this._selectedItemDictArray)
				{
					dict[sourceType] = new Dictionary<ItemKey, int>();
				}
			}
			this._itemScrollView.Init();
			List<ItemDisplayData> currentItems = this.CurrentItems;
			this._itemScrollView.SetItemList(ref currentItems, true, "gearmate_scroll", this._itemScrollView.SortAndFilter.IsDetailView, new Action<ItemDisplayData, ItemView>(this.OnRenderItem));
			UIElement element = this._parent.Element;
			element.OnHide = (Action)Delegate.Combine(element.OnHide, new Action(delegate()
			{
				this.HideGradeLimitTip();
			}));
			this._itemScrollView.InfinityScroll.AddOnScrollEvent(new Action(this.OnScroll));
			this._itemScrollView.ItemListChangedAction = delegate()
			{
				ItemGradeFilterSetting _setting = SingletonObject.getInstance<GameSort>().GetItemGradeFilterSetting();
				sbyte grade = _setting.GetGrade(this.CurItemGradeFilterSourceType);
				this.RefreshMultiplyOptionItemsItems(grade);
			};
			this.SwitchSelectedItemDict(0);
		}

		// Token: 0x06004945 RID: 18757 RVA: 0x002249BA File Offset: 0x00222BBA
		public void SwitchSelectedItemDict(int targetSubPageIndex)
		{
			this._selectedItemDict = this._selectedItemDictArray[targetSubPageIndex];
			this._currentPageIndex = targetSubPageIndex;
		}

		// Token: 0x06004946 RID: 18758 RVA: 0x002249D4 File Offset: 0x00222BD4
		public void ClearSelectedItemDictArray()
		{
			foreach (Dictionary<ItemSourceType, Dictionary<ItemKey, int>> dict in this._selectedItemDictArray)
			{
				foreach (KeyValuePair<ItemSourceType, Dictionary<ItemKey, int>> item in dict)
				{
					dict[item.Key].Clear();
				}
			}
		}

		// Token: 0x06004947 RID: 18759 RVA: 0x00224A4C File Offset: 0x00222C4C
		private void RemoveOtherPageSelectedItem(int targetSubPageIndex, ItemKey itemKey)
		{
			for (int pageIndex = 0; pageIndex < this._selectedItemDictArray.Length; pageIndex++)
			{
				bool flag = targetSubPageIndex == pageIndex;
				if (!flag)
				{
					foreach (KeyValuePair<ItemSourceType, Dictionary<ItemKey, int>> keyValuePair in this._selectedItemDictArray[pageIndex])
					{
						ItemSourceType itemSourceType;
						Dictionary<ItemKey, int> dictionary;
						keyValuePair.Deconstruct(out itemSourceType, out dictionary);
						Dictionary<ItemKey, int> dict = dictionary;
						dict.Remove(itemKey);
					}
				}
			}
		}

		// Token: 0x06004948 RID: 18760 RVA: 0x00224AE0 File Offset: 0x00222CE0
		private void ClearOtherPageSelectedItem(int targetSubPageIndex)
		{
			for (int pageIndex = 0; pageIndex < this._selectedItemDictArray.Length; pageIndex++)
			{
				bool flag = targetSubPageIndex == pageIndex;
				if (!flag)
				{
					foreach (KeyValuePair<ItemSourceType, Dictionary<ItemKey, int>> keyValuePair in this._selectedItemDictArray[pageIndex])
					{
						ItemSourceType itemSourceType;
						Dictionary<ItemKey, int> dictionary;
						keyValuePair.Deconstruct(out itemSourceType, out dictionary);
						Dictionary<ItemKey, int> dict = dictionary;
						dict.Clear();
					}
				}
			}
		}

		// Token: 0x06004949 RID: 18761 RVA: 0x00224B70 File Offset: 0x00222D70
		private void OnRenderItem(ItemDisplayData itemDisplayData, ItemView itemView)
		{
			bool canSelect = true;
			bool flag = this._parent.CurrentLeafSubPage != null;
			if (flag)
			{
				int num;
				canSelect = this._parent.CurrentLeafSubPage.CheckItemInteractable(itemDisplayData, out num);
				bool flag2 = !canSelect;
				if (flag2)
				{
					this._parent.CurrentLeafSubPage.RefreshItemTipNotInteractable(itemView);
				}
			}
			itemView.SetClickEvent(delegate
			{
				this.OnClickItem(itemDisplayData, itemView);
			});
			int selectedCount;
			bool flag3 = this.SelectedItems.TryGetValue(itemDisplayData.Key, out selectedCount);
			if (flag3)
			{
				this.SetItemViewSelected(itemView, selectedCount);
				itemView.SetInteractable(true);
			}
			else
			{
				itemView.SetInteractable(canSelect);
				itemView.SetAmountPos(false);
			}
			this.SetVillagerNeedMark(itemView, itemDisplayData.ItemSourceTypeEnum);
		}

		// Token: 0x0600494A RID: 18762 RVA: 0x00224C78 File Offset: 0x00222E78
		private void OnClickItem(ItemDisplayData itemDisplayData, ItemView itemView)
		{
			GearMateItemSelector.<>c__DisplayClass6_0 CS$<>8__locals1 = new GearMateItemSelector.<>c__DisplayClass6_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.itemDisplayData = itemDisplayData;
			this._itemScrollView.HandleClickItem(CS$<>8__locals1.itemDisplayData, itemView, new Action<ItemView>(CS$<>8__locals1.<OnClickItem>g__Action|0));
		}

		// Token: 0x0600494B RID: 18763 RVA: 0x00224CBC File Offset: 0x00222EBC
		private void SetItemSelectCount(ItemKey itemKey, ItemView itemView, int count)
		{
			this.AddItemToSelectedDict(itemKey, count);
			this.SetItemViewSelected(itemView, count);
			bool flag = count > 0;
			if (flag)
			{
				ItemGradeFilterSetting setting = SingletonObject.getInstance<GameSort>().GetItemGradeFilterSetting();
				sbyte grade = setting.GetGrade(this.CurItemGradeFilterSourceType);
				bool isLimited = grade > 0 && grade <= ItemTemplateHelper.GetGrade(itemKey.ItemType, itemKey.TemplateId);
				bool flag2 = isLimited;
				if (flag2)
				{
					this.ShowGradeLimitTip(itemView);
				}
			}
			this.SetItemList();
		}

		// Token: 0x0600494C RID: 18764 RVA: 0x00224D34 File Offset: 0x00222F34
		private void SetItemViewSelected(ItemView itemView, int count)
		{
			itemView.SetHighLight(count >= 1);
			itemView.SetSelectState(count >= 1);
			bool flag = count >= 1;
			if (flag)
			{
				itemView.SetSelectedCount(count);
			}
			else
			{
				itemView.SetCount(true, false, itemView.Data.Amount);
			}
		}

		// Token: 0x0600494D RID: 18765 RVA: 0x00224D88 File Offset: 0x00222F88
		private void AddItemToSelectedDict(ItemKey itemKey, int count)
		{
			int value = 0;
			bool flag = this.SelectedItems.ContainsKey(itemKey);
			if (flag)
			{
				bool flag2 = count == 0;
				if (flag2)
				{
					value = -this.SelectedItems[itemKey];
					this.SelectedItems.Remove(itemKey);
				}
				else
				{
					value = count - this.SelectedItems[itemKey];
					this.SetSelectCount(itemKey, count);
				}
			}
			else
			{
				bool flag3 = count > 0;
				if (flag3)
				{
					value = count;
					this.SetSelectCount(itemKey, count);
				}
			}
			bool flag4 = value != 0;
			if (flag4)
			{
				this._parent.CurrentLeafSubPage.OnItemChanged(itemKey, value, false, false, true);
			}
			this._parent.CurrentLeafSubPage.SetButtonState(this.SelectedItemsTotalCount != 0);
		}

		// Token: 0x0600494E RID: 18766 RVA: 0x00224E3C File Offset: 0x0022303C
		private void OnItemSourceToggleChange(CToggleObsolete newToggle, CToggleObsolete oldToggle)
		{
			bool flag = !newToggle;
			if (!flag)
			{
				int newTogKey = newToggle.Key;
				this._itemSourceType = this._itemSourceTypeArray[newTogKey];
				this.RequestItems();
				this.RefreshScroll();
				this._parent.CurrentLeafSubPage.StopDropAnimation();
				this.RefreshProcessValue(true);
				MonoJoint componentInChildren = newToggle.GetComponentInChildren<MonoJoint>(true);
				if (componentInChildren != null)
				{
					componentInChildren.JointSync();
				}
			}
		}

		// Token: 0x0600494F RID: 18767 RVA: 0x00224EA8 File Offset: 0x002230A8
		private void RefreshProcessValue(bool isChangeSource = false)
		{
			this._parent.CurrentLeafSubPage.ResetProcessValue(isChangeSource);
			foreach (KeyValuePair<ItemSourceType, Dictionary<ItemKey, int>> keyValuePair in this._selectedItemDict)
			{
				ItemSourceType itemSourceType;
				Dictionary<ItemKey, int> dictionary;
				keyValuePair.Deconstruct(out itemSourceType, out dictionary);
				Dictionary<ItemKey, int> items = dictionary;
				foreach (KeyValuePair<ItemKey, int> item in items)
				{
					this._parent.CurrentLeafSubPage.OnItemChanged(item.Key, item.Value, false, false, false);
				}
			}
		}

		// Token: 0x06004950 RID: 18768 RVA: 0x00224F78 File Offset: 0x00223178
		public void SelectAll()
		{
			this._isAllSelected = this._multiplyOptionItems.All(delegate(ItemDisplayData item)
			{
				int num;
				bool canSelect2 = this._parent.CurrentLeafSubPage.CheckItemInteractable(item, out num);
				bool flag4 = !canSelect2;
				bool result;
				if (flag4)
				{
					result = true;
				}
				else
				{
					int selectedCount2;
					this.SelectedItems.TryGetValue(item.Key, out selectedCount2);
					result = (item.Amount == selectedCount2);
				}
				return result;
			});
			this._parent.CurrentLeafSubPage.StopDropAnimation();
			bool isAllSelected = this._isAllSelected;
			if (isAllSelected)
			{
				this.ClearSelectCount();
				this.RefreshProcessValue(false);
			}
			else
			{
				foreach (ItemDisplayData item2 in this._itemScrollView.MySortAndFilter.OutputItemList)
				{
					bool flag = !this._multiplyOptionItems.Contains(item2);
					if (!flag)
					{
						int canSelectCount;
						bool canSelect = this._parent.CurrentLeafSubPage.CheckItemInteractable(item2, out canSelectCount);
						bool flag2 = !canSelect;
						if (!flag2)
						{
							int selectedCount;
							this.SelectedItems.TryGetValue(item2.Key, out selectedCount);
							canSelectCount = Math.Clamp(canSelectCount, 0, item2.Amount - selectedCount);
							this.SetSelectCount(item2.Key, selectedCount + canSelectCount);
							this._parent.CurrentLeafSubPage.OnItemChanged(item2.Key, canSelectCount, true, true, true);
						}
					}
				}
				this._parent.CurrentLeafSubPage.PlayDropAnimation();
			}
			GearMateAttributeUpdatePage gearMateAttributeUpdatePage = this._parent.CurrentLeafSubPage as GearMateAttributeUpdatePage;
			bool flag3 = gearMateAttributeUpdatePage != null;
			if (flag3)
			{
				gearMateAttributeUpdatePage.SetAttributeProgressBarByAllSelected();
			}
			this.SetItemList();
		}

		// Token: 0x06004951 RID: 18769 RVA: 0x002250F4 File Offset: 0x002232F4
		private void SetSelectCount(ItemKey itemKey, int newCount)
		{
			this.SelectedItems[itemKey] = newCount;
			this.RemoveOtherPageSelectedItem(this._currentPageIndex, itemKey);
		}

		// Token: 0x06004952 RID: 18770 RVA: 0x00225113 File Offset: 0x00223313
		private void ClearSelectCount()
		{
			this.SelectedItems.Clear();
			this.ClearOtherPageSelectedItem(this._currentPageIndex);
		}

		// Token: 0x06004953 RID: 18771 RVA: 0x00225130 File Offset: 0x00223330
		public void Confirm()
		{
			this._parent.SetDisableClickActive(true);
			this._parent.CurrentLeafSubPage.SetButtonState(false);
			foreach (KeyValuePair<ItemSourceType, Dictionary<ItemKey, int>> keyValuePair in this._selectedItemDict)
			{
				ItemSourceType itemSourceType;
				Dictionary<ItemKey, int> dictionary;
				keyValuePair.Deconstruct(out itemSourceType, out dictionary);
				ItemSourceType type = itemSourceType;
				Dictionary<ItemKey, int> items = dictionary;
				foreach (KeyValuePair<ItemKey, int> item in items)
				{
					this._parent.CurrentLeafSubPage.Confirm(new ValueTuple<ItemKey, int>(item.Key, item.Value), type);
				}
				items.Clear();
			}
			this.RequestAllSourceTypeItems();
			this._parent.CurrentLeafSubPage.PlayUpgradeAnim(new Action(this.<Confirm>g__Action|15_0));
		}

		// Token: 0x06004954 RID: 18772 RVA: 0x00225248 File Offset: 0x00223448
		private void RefreshScroll()
		{
			this.HideGradeLimitTip();
			TextMeshProUGUI text = this._text;
			SubPage currentSubPageIndex = (SubPage)this._parent.CurrentSubPageIndex;
			if (!true)
			{
			}
			LanguageKey id;
			if (currentSubPageIndex != SubPage.Feature)
			{
				if (currentSubPageIndex != SubPage.Neili)
				{
					id = LanguageKey.LK_GearMate_ItemTitle;
				}
				else
				{
					id = LanguageKey.LK_ItemType_12;
				}
			}
			else
			{
				id = LanguageKey.LK_CharacterMenu_Tog_Equip;
			}
			if (!true)
			{
			}
			text.text = LocalStringManager.Get(id);
			ItemGradeFilterSetting _setting = SingletonObject.getInstance<GameSort>().GetItemGradeFilterSetting();
			sbyte grade = _setting.GetGrade(this.CurItemGradeFilterSourceType);
			this._filteredItems.Clear();
			this._filteredItems.AddRange(from d in this.CurrentItems
			where this.FilterItem((SubPage)this._parent.CurrentSubPageIndex, d)
			select d);
			this.RefreshMultiplyOptionItemsItems(grade);
			this.SetItemList();
		}

		// Token: 0x06004955 RID: 18773 RVA: 0x00225304 File Offset: 0x00223504
		private void RefreshMultiplyOptionItemsItems(sbyte grade)
		{
			List<ItemDisplayData> items = (this._parent.CurrentSubPageIndex == 0) ? (from d in this._filteredItems
			where this.FilterAttributeItemBySortAndFilter(d)
			select d).ToList<ItemDisplayData>() : this._filteredItems;
			this._multiplyOptionItems.Clear();
			this._multiplyOptionItems.AddRange(from d in items
			where ItemTemplateHelper.GetGrade(d.Key.ItemType, d.Key.TemplateId) < grade || grade < 0
			select d);
			this.RefreshBtnSelectAll();
		}

		// Token: 0x06004956 RID: 18774 RVA: 0x0022538C File Offset: 0x0022358C
		private void SetItemList()
		{
			List<ItemDisplayData> items = this._filteredItems;
			bool flag = this._parent.CurrentLeafSubPage == null;
			if (!flag)
			{
				this._itemScrollView.SetItemList(ref items, false, null, false, null);
				this._parent.CurrentLeafSubPage.SetButtonState(this.SelectedItemsTotalCount != 0);
			}
		}

		// Token: 0x06004957 RID: 18775 RVA: 0x002253E4 File Offset: 0x002235E4
		private void RefreshBtnSelectAll()
		{
			this._btnSelectAll.interactable = (this._multiplyOptionItems.Count != 0);
			TooltipInvoker tip = this._btnSelectAll.GetComponent<TooltipInvoker>();
			tip.enabled = false;
		}

		// Token: 0x06004958 RID: 18776 RVA: 0x00225420 File Offset: 0x00223620
		public void RefreshItemFilter()
		{
			this._filter.SetActive(this._parent.CurrentSubPageIndex != 0);
			List<ItemSortAndFilter.ItemFilterType> filterTypes;
			bool flag = this._parent.CurrentSubPageIndex != 0 && GearMateItemSelector.SubPageToItemFilterTypeDict.TryGetValue((SubPage)this._parent.CurrentSubPageIndex, out filterTypes);
			if (flag)
			{
				this._itemScrollView.SortAndFilter.ShowFilterType(filterTypes);
				filterTypes.Add(ItemSortAndFilter.ItemFilterType.Invalid);
				this._itemScrollView.SortAndFilter.LockFilterType(filterTypes, ItemSortAndFilter.LockFilterTypeToggleActionMode.Default);
				filterTypes.Remove(ItemSortAndFilter.ItemFilterType.Invalid);
			}
			this.RefreshScroll();
		}

		// Token: 0x06004959 RID: 18777 RVA: 0x002254B1 File Offset: 0x002236B1
		public void OnListenerIdReady()
		{
			this.RequestItems();
			TaiwuDomainMethod.Call.GetTreasuryNeededItemList(this._parent.Element.GameDataListenerId);
			TaiwuDomainMethod.Call.CanTransferItemToWarehouse(this._parent.Element.GameDataListenerId);
		}

		// Token: 0x0600495A RID: 18778 RVA: 0x002254E7 File Offset: 0x002236E7
		private void RequestItems()
		{
			TaiwuDomainMethod.Call.GetAllItems(this._parent.Element.GameDataListenerId, this._itemSourceType, true);
		}

		// Token: 0x0600495B RID: 18779 RVA: 0x00225508 File Offset: 0x00223708
		private void RequestAllSourceTypeItems()
		{
			foreach (KeyValuePair<ItemSourceType, List<ItemDisplayData>> item in this._itemDict)
			{
				TaiwuDomainMethod.Call.GetAllItems(this._parent.Element.GameDataListenerId, item.Key, true);
			}
		}

		// Token: 0x0600495C RID: 18780 RVA: 0x00225578 File Offset: 0x00223778
		public void OpenMultiplyOption(CButtonObsolete button)
		{
			bool flag = UIManager.Instance.IsFocusElement(UIElement.ItemMultiplyOptionOld);
			if (flag)
			{
				UIManager.Instance.HideUI(UIElement.ItemMultiplyOptionOld);
			}
			else
			{
				RectTransform rectTrans = button.GetComponent<RectTransform>();
				Vector3 localPos = default(Vector3).SetY(rectTrans.rect.height * 0.5f);
				Vector3 pos = rectTrans.TransformPoint(localPos);
				ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("AnchorItem", rectTrans).SetObject("Pos", pos).Set("Type", this.CurItemGradeFilterSourceType).Set("IsGearMate", true).SetObject("OnSetGrade", new Action(this.RefreshScroll));
				UIElement.ItemMultiplyOptionOld.SetOnInitArgs(args);
				UIManager.Instance.ShowUI(UIElement.ItemMultiplyOptionOld, true);
			}
		}

		// Token: 0x0600495D RID: 18781 RVA: 0x0022565C File Offset: 0x0022385C
		public void HandleMethodReturn(Notification notification, NotificationWrapper wrapper)
		{
			ushort domianId = notification.DomainId;
			ushort methodId = notification.MethodId;
			RawDataPool pool = wrapper.DataPool;
			int offset = notification.ValueOffset;
			bool flag = domianId == 5;
			if (flag)
			{
				bool flag2 = methodId == 118;
				if (flag2)
				{
					ValueTuple<ItemSourceType, List<ItemDisplayData>> tuple = default(ValueTuple<ItemSourceType, List<ItemDisplayData>>);
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref tuple);
					this._itemDict[tuple.Item1].Clear();
					bool flag3 = tuple.Item2 != null;
					if (flag3)
					{
						this._itemDict[tuple.Item1].AddRange(tuple.Item2);
					}
					this.RefreshScroll();
				}
				else
				{
					bool flag4 = notification.MethodId == 139;
					if (flag4)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._villagerNeededItemSet);
					}
					else
					{
						bool flag5 = notification.MethodId == 42;
						if (flag5)
						{
							bool canTransfer = true;
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref canTransfer);
							Transform toggleGroupTransform = this._itemSrouceToggleGroup.transform;
							for (int i = 1; i < 3; i++)
							{
								CToggleObsolete toggle = toggleGroupTransform.GetChild(i).GetComponent<CToggleObsolete>();
								toggle.interactable = canTransfer;
								toggle.GetComponent<TooltipInvoker>().enabled = !toggle.interactable;
							}
							CToggleObsolete tog = this._itemSrouceToggleGroup.GetAll().First((CToggleObsolete t) => canTransfer ? t.isOn : t.interactable);
							this._itemSrouceToggleGroup.Set(tog, true);
						}
					}
				}
			}
		}

		// Token: 0x0600495E RID: 18782 RVA: 0x0022580C File Offset: 0x00223A0C
		private void ShowGradeLimitTip(ItemView itemView)
		{
			RectTransform tip;
			bool flag = this.CTryGet<RectTransform>("GradeLimitTip", out tip);
			if (flag)
			{
				bool flag2 = !tip.gameObject.activeSelf;
				if (flag2)
				{
					tip.gameObject.SetActive(true);
				}
				PositionFollower follower = tip.GetComponent<PositionFollower>();
				RectTransform itemRectTrans = itemView.GetComponent<RectTransform>();
				follower.Target = itemRectTrans;
				follower.Offset = new Vector2(itemRectTrans.sizeDelta.x * 0.5f, -itemRectTrans.sizeDelta.y);
				follower.Excute();
				Rect itemRect = CommonUtils.RectTransToScreenPos(tip, UIManager.Instance.UiCamera);
				Rect scrollRect = CommonUtils.RectTransToScreenPos(this._itemScrollView.ViewportRectTransform, UIManager.Instance.UiCamera);
				bool isOverlap = itemRect.Overlaps(scrollRect);
				bool flag3 = !isOverlap;
				if (flag3)
				{
					follower.Offset = new Vector2(itemRectTrans.sizeDelta.x * 0.5f, tip.sizeDelta.y);
				}
				CanvasGroup canvasGroup = tip.GetComponent<CanvasGroup>();
				canvasGroup.DOKill(false);
				canvasGroup.alpha = 1f;
				canvasGroup.DOFade(0.2f, 1f).SetLoops(5, LoopType.Yoyo);
				canvasGroup.DOFade(0f, 0.2f).SetDelay(5f);
			}
		}

		// Token: 0x0600495F RID: 18783 RVA: 0x00225960 File Offset: 0x00223B60
		public void HideGradeLimitTip()
		{
			RectTransform tip;
			bool flag = this.CTryGet<RectTransform>("GradeLimitTip", out tip);
			if (flag)
			{
				bool activeSelf = tip.gameObject.activeSelf;
				if (activeSelf)
				{
					tip.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06004960 RID: 18784 RVA: 0x0022599D File Offset: 0x00223B9D
		private void OnScroll()
		{
			this.HideGradeLimitTip();
		}

		// Token: 0x06004961 RID: 18785 RVA: 0x002259A8 File Offset: 0x00223BA8
		private void InitRefers()
		{
			this._itemScrollView = base.CGet<ItemScrollView>("ItemScrollView");
			this._itemSrouceToggleGroup = base.CGet<CToggleGroupObsolete>("ItemSrouceToggleGroup");
			this._warehouse = base.CGet<CToggleObsolete>("Warehouse");
			this._treasury = base.CGet<CToggleObsolete>("Treasury");
			this._stockStorage = base.CGet<CToggleObsolete>("StockStorage");
			this._btnSelectAll = base.CGet<CButtonObsolete>("BtnSelectAll");
			this._btnMultiplyOption = base.CGet<CButtonObsolete>("BtnMultiplyOption");
			this._focusItemMask = base.CGet<GameObject>("FocusItemMask");
			this._itemSelector = base.CGet<MultiplyItemScrollView>("ItemSelector");
			this._text = base.CGet<TextMeshProUGUI>("Text");
			this._filter = base.CGet<GameObject>("Filter");
		}

		// Token: 0x1700092E RID: 2350
		// (get) Token: 0x06004962 RID: 18786 RVA: 0x00225A71 File Offset: 0x00223C71
		public int SelectedItemsTotalCount
		{
			get
			{
				return this._selectedItemDict.Sum((KeyValuePair<ItemSourceType, Dictionary<ItemKey, int>> d) => d.Value.Count);
			}
		}

		// Token: 0x1700092F RID: 2351
		// (get) Token: 0x06004963 RID: 18787 RVA: 0x00225A9D File Offset: 0x00223C9D
		private Dictionary<ItemKey, int> SelectedItems
		{
			get
			{
				return this._selectedItemDict[this._itemSourceType];
			}
		}

		// Token: 0x06004964 RID: 18788 RVA: 0x00225AB0 File Offset: 0x00223CB0
		public Dictionary<ItemSourceType, Dictionary<ItemKey, int>> GetTypeToSelectedItemDict(int pageIndex)
		{
			bool flag = pageIndex > this._selectedItemDictArray.Length;
			Dictionary<ItemSourceType, Dictionary<ItemKey, int>> result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = this._selectedItemDictArray[pageIndex];
			}
			return result;
		}

		// Token: 0x17000930 RID: 2352
		// (get) Token: 0x06004965 RID: 18789 RVA: 0x00225ADD File Offset: 0x00223CDD
		private List<ItemDisplayData> CurrentItems
		{
			get
			{
				return this._itemDict[this._itemSourceType];
			}
		}

		// Token: 0x17000931 RID: 2353
		// (get) Token: 0x06004966 RID: 18790 RVA: 0x00225AF0 File Offset: 0x00223CF0
		private ItemGradeFilterSetting.ItemGradeFilterSourceType CurItemGradeFilterSourceType
		{
			get
			{
				return ItemGradeFilterSetting.GetItemGradeFilterSourceType(this._itemSourceType, this._itemSourceType == ItemSourceType.Inventory, true);
			}
		}

		// Token: 0x06004967 RID: 18791 RVA: 0x00225B08 File Offset: 0x00223D08
		public bool FilterItem(SubPage page, ItemDisplayData item)
		{
			bool result;
			switch (page)
			{
			case SubPage.Attribute:
				result = (item.Key.ItemType == 5 && Config.Material.Instance[item.Key.TemplateId].ResourceType <= 5);
				break;
			case SubPage.Feature:
				result = this.FilterFeatureItem(item);
				break;
			case SubPage.Consummate:
				result = this.FilterConsummateItem(item);
				break;
			case SubPage.Neili:
				result = this.FilterNeiliItem(item);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		// Token: 0x06004968 RID: 18792 RVA: 0x00225B8C File Offset: 0x00223D8C
		private bool FilterConsummateItem(ItemDisplayData item)
		{
			return item.Key.ItemType == 5 && Config.Material.Instance[item.Key.TemplateId].RefiningEffect >= 0;
		}

		// Token: 0x06004969 RID: 18793 RVA: 0x00225BD0 File Offset: 0x00223DD0
		private bool FilterFeatureItem(ItemDisplayData item)
		{
			sbyte itemType = item.Key.ItemType;
			return itemType == 0 || itemType == 1 || itemType == 2;
		}

		// Token: 0x0600496A RID: 18794 RVA: 0x00225C04 File Offset: 0x00223E04
		private bool FilterNeiliItem(ItemDisplayData item)
		{
			short templateId = item.Key.TemplateId;
			return item.Key.ItemType == 12 && (templateId == 9 || templateId == 10 || templateId == 11 || templateId == 12 || templateId == 13 || templateId == 14 || templateId == 15 || templateId == 16 || templateId == 17);
		}

		// Token: 0x0600496B RID: 18795 RVA: 0x00225C64 File Offset: 0x00223E64
		private bool FilterAttributeItemBySortAndFilter(ItemDisplayData item)
		{
			ItemSortAndFilter.MaterialFilterType targetMaterialFilterType = (this._itemScrollView.SortAndFilter.SortFilterSetting.MaterialFilterType.Count == ItemSortAndFilter.MaterialFilterType.Count.ToInt() - 1) ? ItemSortAndFilter.MaterialFilterType.Invalid : this._itemScrollView.SortAndFilter.SortFilterSetting.MaterialFilterType.First<ItemSortAndFilter.MaterialFilterType>();
			return targetMaterialFilterType == ItemSortAndFilter.MaterialFilterType.Invalid || Config.Material.Instance[item.Key.TemplateId].ResourceType == (sbyte)targetMaterialFilterType - 1;
		}

		// Token: 0x0600496C RID: 18796 RVA: 0x00225CE4 File Offset: 0x00223EE4
		private void SetVillagerNeedMark(ItemView itemView, ItemSourceType sourceType)
		{
			bool sourceTypeIsMeet = sourceType == ItemSourceType.Treasury;
			bool flag = !sourceTypeIsMeet;
			if (flag)
			{
				itemView.SetVillagerNeedMark(false, true);
			}
			else
			{
				ItemKey tempKey = ItemKey.Invalid;
				tempKey.ItemType = itemView.Data.Key.ItemType;
				tempKey.TemplateId = itemView.Data.Key.TemplateId;
				bool isNeeded = this._villagerNeededItemSet.Contains(tempKey);
				itemView.SetVillagerNeedMark(isNeeded, true);
			}
		}

		// Token: 0x0600496D RID: 18797 RVA: 0x00225D58 File Offset: 0x00223F58
		public GearMateItemSelector()
		{
			ItemSourceType[] array = new ItemSourceType[3];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.4636993D3E1DA4E9D6B8F87B79E8F7C6D018580D52661950EABC3845C5897A4D).FieldHandle);
			this._itemSourceTypeArray = array;
			this._currentPageIndex = -1;
			this._selectedItemDictArray = new Dictionary<ItemSourceType, Dictionary<ItemKey, int>>[]
			{
				new Dictionary<ItemSourceType, Dictionary<ItemKey, int>>(),
				new Dictionary<ItemSourceType, Dictionary<ItemKey, int>>(),
				new Dictionary<ItemSourceType, Dictionary<ItemKey, int>>()
			};
			this._itemDict = new Dictionary<ItemSourceType, List<ItemDisplayData>>();
			this._villagerNeededItemSet = new List<ItemKey>();
			this._filteredItems = new List<ItemDisplayData>();
			this._multiplyOptionItems = new List<ItemDisplayData>();
			base..ctor();
		}

		// Token: 0x06004972 RID: 18802 RVA: 0x00225EC6 File Offset: 0x002240C6
		[CompilerGenerated]
		private void <Confirm>g__Action|15_0()
		{
			this._parent.SetDisableClickActive(false);
			ExtraDomainMethod.Call.GetGearMateById(this._parent.Element.GameDataListenerId, this._parent.GearMateDisplayData.CharacterId);
		}

		// Token: 0x040032FC RID: 13052
		private ItemScrollView _itemScrollView;

		// Token: 0x040032FD RID: 13053
		private CToggleGroupObsolete _itemSrouceToggleGroup;

		// Token: 0x040032FE RID: 13054
		private CToggleObsolete _warehouse;

		// Token: 0x040032FF RID: 13055
		private CToggleObsolete _treasury;

		// Token: 0x04003300 RID: 13056
		private CToggleObsolete _stockStorage;

		// Token: 0x04003301 RID: 13057
		private CButtonObsolete _btnSelectAll;

		// Token: 0x04003302 RID: 13058
		private CButtonObsolete _btnMultiplyOption;

		// Token: 0x04003303 RID: 13059
		private GameObject _focusItemMask;

		// Token: 0x04003304 RID: 13060
		private MultiplyItemScrollView _itemSelector;

		// Token: 0x04003305 RID: 13061
		private TextMeshProUGUI _text;

		// Token: 0x04003306 RID: 13062
		private GameObject _filter;

		// Token: 0x04003307 RID: 13063
		private ItemSourceType _itemSourceType = ItemSourceType.Inventory;

		// Token: 0x04003308 RID: 13064
		private readonly ItemSourceType[] _itemSourceTypeArray;

		// Token: 0x04003309 RID: 13065
		private UI_GearMate _parent;

		// Token: 0x0400330A RID: 13066
		private Dictionary<ItemSourceType, Dictionary<ItemKey, int>> _selectedItemDict;

		// Token: 0x0400330B RID: 13067
		private int _currentPageIndex;

		// Token: 0x0400330C RID: 13068
		private readonly Dictionary<ItemSourceType, Dictionary<ItemKey, int>>[] _selectedItemDictArray;

		// Token: 0x0400330D RID: 13069
		private readonly Dictionary<ItemSourceType, List<ItemDisplayData>> _itemDict;

		// Token: 0x0400330E RID: 13070
		private List<ItemKey> _villagerNeededItemSet;

		// Token: 0x0400330F RID: 13071
		private readonly List<ItemDisplayData> _filteredItems;

		// Token: 0x04003310 RID: 13072
		private readonly List<ItemDisplayData> _multiplyOptionItems;

		// Token: 0x04003311 RID: 13073
		private bool _isAllSelected;

		// Token: 0x04003312 RID: 13074
		public static readonly Dictionary<SubPage, List<ItemSortAndFilter.ItemFilterType>> SubPageToItemFilterTypeDict = new Dictionary<SubPage, List<ItemSortAndFilter.ItemFilterType>>
		{
			{
				SubPage.Feature,
				new List<ItemSortAndFilter.ItemFilterType>
				{
					ItemSortAndFilter.ItemFilterType.Equip
				}
			},
			{
				SubPage.Consummate,
				new List<ItemSortAndFilter.ItemFilterType>
				{
					ItemSortAndFilter.ItemFilterType.Material
				}
			},
			{
				SubPage.Neili,
				new List<ItemSortAndFilter.ItemFilterType>
				{
					ItemSortAndFilter.ItemFilterType.Other
				}
			}
		};
	}
}
