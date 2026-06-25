using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CharacterDataMonitor;
using FrameWork;
using Game.Views;
using Game.Views.CharacterMenu;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace UILogic
{
	// Token: 0x020006AF RID: 1711
	public class UI_BeggarSkill3 : UIBase
	{
		// Token: 0x170009C2 RID: 2498
		// (get) Token: 0x06004FE4 RID: 20452 RVA: 0x00255965 File Offset: 0x00253B65
		private bool HasNormal
		{
			get
			{
				return this._normalCharList.Count > 0;
			}
		}

		// Token: 0x06004FE5 RID: 20453 RVA: 0x00255978 File Offset: 0x00253B78
		private void MapCharListAwake()
		{
			this._mapBlockCharListRefers = base.CGet<Refers>("UI_MapBlockCharList");
			UI_BeggarSkill3._searchInputField = this._mapBlockCharListRefers.CGet<TMP_InputField>("SearchInputField");
			this._charScroll = this._mapBlockCharListRefers.CGet<InfinityScrollLegacy>("ActorView");
			this._charScroll.SrcPrefab.gameObject.SetActive(false);
			this._charScroll.OnItemHide = new Action<Refers>(this.OnItemHide);
			this._mapCharListTogGroup = this._mapBlockCharListRefers.CGet<CToggleGroupObsolete>("ToggleHolder");
			this._mapCharListTogGroup.InitPreOnToggle(-1);
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Remove(element.OnListenerIdReady, new Action(this.InitMapCharListTogGroup));
			UIElement element2 = this.Element;
			element2.OnListenerIdReady = (Action)Delegate.Combine(element2.OnListenerIdReady, new Action(this.InitMapCharListTogGroup));
			UI_BeggarSkill3._searchInputField.onValueChanged.RemoveAllListeners();
			UI_BeggarSkill3._searchInputField.onValueChanged.AddListener(delegate(string value)
			{
				bool flag = CommonUtils.FixToShowAbleString(ref value, UI_BeggarSkill3._searchInputField.textComponent.font);
				if (flag)
				{
					UI_BeggarSkill3._searchInputField.SetTextWithoutNotify(value);
				}
				this.UpdateCharList(true, null);
			});
			UI_BeggarSkill3._searchInputField.SetTextWithoutNotify(string.Empty);
		}

		// Token: 0x06004FE6 RID: 20454 RVA: 0x00255A9D File Offset: 0x00253C9D
		private void InitMapCharListTogGroup()
		{
			this._mapCharListTogGroup.SetInteractable(true, 0);
			this._mapCharListTogGroup.SetInteractable(false, 1);
			this._mapCharListTogGroup.SetInteractable(false, 3);
			this._mapCharListTogGroup.SetInteractable(false, 2);
		}

		// Token: 0x06004FE7 RID: 20455 RVA: 0x00255AD8 File Offset: 0x00253CD8
		private void UpdateCharList(bool scrollToTop, Action callback = null)
		{
			Action<int, Refers> onRenderer = new Action<int, Refers>(this.OnRenderCharNormal);
			this._charScroll.OnItemRender = onRenderer;
			string prefabName = "MapBlockCharNormal";
			Refers prefab = this._mapBlockCharListRefers.CGet<Refers>(prefabName);
			int actorAmount = this.RefreshSearchedNormalCharacterData();
			this._mapBlockCharListRefers.CGet<TextMeshProUGUI>("ActorAmount").text = actorAmount.ToString();
			this._charScroll.UpdateStyle(prefab, actorAmount);
			this._charScroll.SetDataCount(actorAmount);
			if (callback != null)
			{
				callback();
			}
			if (scrollToTop)
			{
				this._charScroll.ScrollTo(0, 0.3f);
			}
		}

		// Token: 0x06004FE8 RID: 20456 RVA: 0x00255B78 File Offset: 0x00253D78
		private int CharCompare(int charId1, int charId2)
		{
			CharacterDisplayData data = this._charDataDict[charId1];
			CharacterDisplayData data2 = this._charDataDict[charId2];
			List<ItemDisplayData> items = this._mapCharItemsDic[charId1];
			List<ItemDisplayData> items2 = this._mapCharItemsDic[charId2];
			bool flag = items2.Count != items.Count;
			int result;
			if (flag)
			{
				result = items2.Count.CompareTo(items.Count);
			}
			else
			{
				bool flag2 = data2.OrgInfo.Grade != data.OrgInfo.Grade;
				if (flag2)
				{
					result = data2.OrgInfo.Grade.CompareTo(data.OrgInfo.Grade);
				}
				else
				{
					bool flag3 = data2.OrgInfo.Principal != data.OrgInfo.Principal;
					if (flag3)
					{
						result = data2.OrgInfo.Principal.CompareTo(data.OrgInfo.Principal);
					}
					else
					{
						bool flag4 = data2.PhysiologicalAge != data.PhysiologicalAge;
						if (flag4)
						{
							result = data2.PhysiologicalAge.CompareTo(data.PhysiologicalAge);
						}
						else
						{
							result = charId1.CompareTo(charId2);
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06004FE9 RID: 20457 RVA: 0x00255CA8 File Offset: 0x00253EA8
		private int RefreshSearchedNormalCharacterData()
		{
			this._searchedNormalCharList.Clear();
			bool flag = UI_BeggarSkill3._searchInputField.text.IsNullOrEmpty();
			if (flag)
			{
				this._searchedNormalCharList.AddRange(this._normalCharList);
			}
			else
			{
				this._searchedNormalCharList.AddRange(this._normalCharList.Where(delegate(int charId)
				{
					CharacterDisplayData characterDisplayData;
					bool flag2 = this._charDataDict.TryGetValue(charId, out characterDisplayData);
					bool result;
					if (flag2)
					{
						string nameContent = NameCenter.GetCharMonasticTitleOrNameByDisplayData(characterDisplayData, false, false);
						string org = CommonUtils.GetOrganizationGradeString(characterDisplayData.OrgInfo, characterDisplayData.Gender, characterDisplayData.PhysiologicalAge, -1);
						result = (nameContent.Contains(UI_BeggarSkill3._searchInputField.text) || org.Contains(UI_BeggarSkill3._searchInputField.text));
					}
					else
					{
						result = false;
					}
					return result;
				}));
			}
			return this._searchedNormalCharList.Count;
		}

		// Token: 0x06004FEA RID: 20458 RVA: 0x00255D1C File Offset: 0x00253F1C
		private void OnRenderCharNormal(int index, Refers charRefers)
		{
			UI_BeggarSkill3.<>c__DisplayClass24_0 CS$<>8__locals1 = new UI_BeggarSkill3.<>c__DisplayClass24_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.charRefers = charRefers;
			CS$<>8__locals1.charId = (this._searchedNormalCharList.CheckIndex(index) ? this._searchedNormalCharList[index] : -1);
			CS$<>8__locals1.prefabName = "MapBlockCharNormal";
			CS$<>8__locals1.block = this._mapModel.GetBlockData(this._mapModel.CurrentLocation);
			bool flag = this._charDataDict.TryGetValue(CS$<>8__locals1.charId, out CS$<>8__locals1.characterDisplayData);
			if (flag)
			{
				bool flag2 = this._curSelectedCharRefers == null;
				if (flag2)
				{
					this._curSelectedCharRefers = CS$<>8__locals1.charRefers;
					this._curSelectedCharRefers.CGet<CToggleObsolete>("CToggle").isOn = true;
				}
				MapBlockCharNormal mapBlockCharNormal;
				bool flag3 = CS$<>8__locals1.charRefers.CTryGet<MapBlockCharNormal>(CS$<>8__locals1.prefabName, out mapBlockCharNormal);
				if (flag3)
				{
					CS$<>8__locals1.<OnRenderCharNormal>g__InitMapBlockCharNormal|2(CS$<>8__locals1.charRefers, mapBlockCharNormal, CS$<>8__locals1.block, CS$<>8__locals1.characterDisplayData, CS$<>8__locals1.charId);
					CS$<>8__locals1.<OnRenderCharNormal>g__CheckItemsCount|0(CS$<>8__locals1.charRefers);
				}
			}
			else
			{
				CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(this, CS$<>8__locals1.charId, delegate(int offset, RawDataPool pool)
				{
					Serializer.Deserialize(pool, offset, ref CS$<>8__locals1.characterDisplayData);
					bool flag4 = CS$<>8__locals1.<>4__this._curSelectedCharRefers == null;
					if (flag4)
					{
						CS$<>8__locals1.<>4__this._curSelectedCharRefers = CS$<>8__locals1.charRefers;
						CS$<>8__locals1.<>4__this._curSelectedCharRefers.CGet<CToggleObsolete>("CToggle").isOn = true;
					}
					MapBlockCharNormal mapBlockCharNormal2;
					bool flag5 = CS$<>8__locals1.charRefers.CTryGet<MapBlockCharNormal>(CS$<>8__locals1.prefabName, out mapBlockCharNormal2);
					if (flag5)
					{
						base.<OnRenderCharNormal>g__InitMapBlockCharNormal|2(CS$<>8__locals1.charRefers, mapBlockCharNormal2, CS$<>8__locals1.block, CS$<>8__locals1.characterDisplayData, CS$<>8__locals1.charId);
						base.<OnRenderCharNormal>g__CheckItemsCount|0(CS$<>8__locals1.charRefers);
					}
				});
			}
		}

		// Token: 0x06004FEB RID: 20459 RVA: 0x00255E44 File Offset: 0x00254044
		private void OnItemHide(Refers charRefers)
		{
			MapBlockCharNormal mapBlockCharNormal;
			bool flag = charRefers.CTryGet<MapBlockCharNormal>("MapBlockCharNormal", out mapBlockCharNormal);
			if (flag)
			{
				mapBlockCharNormal.OnHide();
			}
		}

		// Token: 0x06004FEC RID: 20460 RVA: 0x00255E6A File Offset: 0x0025406A
		private void ItemsAwake()
		{
			this._itemScrollView = base.CGet<ItemScrollView>("ItemScrollView");
			this._itemScrollView.Init();
		}

		// Token: 0x06004FED RID: 20461 RVA: 0x00255E8C File Offset: 0x0025408C
		private void UpdateItems(int charId, bool reset = false)
		{
			bool flag = this._mapCharItemsDic.TryGetValue(charId, out this._items);
			if (flag)
			{
				this._itemScrollView.SetItemList(ref this._items, reset, null, this._itemScrollView.SortAndFilter.IsDetailView, new Action<ItemDisplayData, ItemView>(this.OnRenderItem));
			}
		}

		// Token: 0x06004FEE RID: 20462 RVA: 0x00255EE4 File Offset: 0x002540E4
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 1)
				{
					bool flag = notification.DomainId == 4;
					if (flag)
					{
						bool flag2 = notification.MethodId == 27;
						if (flag2)
						{
							this._items.Clear();
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._items);
							this._items = (from data in this._items
							where CommonUtils.CanItemEat(data.Key.ItemType, data.Key.TemplateId, this._taiwuCharId)
							select data).ToList<ItemDisplayData>();
							this._mapCharItemsDic[this._curCharId].Clear();
							this._mapCharItemsDic[this._curCharId].AddRange(this._items);
							this._itemScrollView.SetItemList(ref this._items, false, null, false, null);
						}
					}
				}
			}
		}

		// Token: 0x06004FEF RID: 20463 RVA: 0x0025600C File Offset: 0x0025420C
		private void OnRenderItem(ItemDisplayData itemData, ItemView itemView)
		{
			DragDrop itemDrag = itemView.GetComponent<DragDrop>();
			itemView.SetClickEvent(delegate
			{
				this.OnClickItem(itemData, itemView);
			});
			itemDrag.DragOn = false;
		}

		// Token: 0x06004FF0 RID: 20464 RVA: 0x00256060 File Offset: 0x00254260
		private void OnClickItem(ItemDisplayData itemData, ItemView itemView)
		{
			this._itemScrollView.HandleClickItem(itemData, itemView, new Action<ItemView>(this.<OnClickItem>g__Action|35_0));
		}

		// Token: 0x06004FF1 RID: 20465 RVA: 0x00256080 File Offset: 0x00254280
		private void ShowItemOperateMenu(ItemDisplayData itemData, ItemView itemView)
		{
			List<ViewPopupMenu.BtnData> btnList = new List<ViewPopupMenu.BtnData>();
			ValueTuple<int, string> valueTuple = CommonUtils.CalculateCountAndTip(this._taiwuCharId, itemData.Key, itemData.Amount);
			int count = valueTuple.Item1;
			string reason = valueTuple.Item2;
			bool canEatMore = count > 0;
			EatingItemMonitor eatingItems = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<EatingItemMonitor>(this._taiwuCharId, false);
			bool hasEatWugKing = (from x in eatingItems.EatingItemList
			select x.Item1).Any(new Func<ItemKey, bool>(EatingItems.IsWugKing));
			bool isWugKing = EatingItems.IsWugKing(itemData.Key);
			bool canReplaceWugKing = isWugKing && hasEatWugKing;
			bool flag = itemData.Key.ItemType == 8;
			if (flag)
			{
				bool hasAttributeToTopical = this._characterAttributeView.HasAttributeToTopical(itemData.Key);
				bool interactable = CommonUtils.GetMedicineItemMenuInteractable(itemData.Key, canEatMore, canReplaceWugKing, hasAttributeToTopical, ref reason);
				string btnName = CommonUtils.GetCanEatItemButtonName(itemData.RealKey);
				ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(btnName, interactable, EItemMenuDisplayOrder.Eat, delegate()
				{
					this.OnClickEatItem(itemData);
				}, delegate()
				{
					this.SetEatItemInfectNotice(true, itemData, 1);
				}, delegate()
				{
					this.SetEatItemInfectNotice(false, null, 1);
				}, false);
				btnList.Add(btnData);
				bool flag2 = !interactable;
				if (flag2)
				{
					btnData.SetTip(string.Empty, reason);
				}
			}
			else
			{
				bool flag3 = CommonUtils.CanItemEat(itemData.Key.ItemType, itemData.Key.TemplateId, this._taiwuCharId);
				if (flag3)
				{
					bool flag4 = ItemTemplateHelper.IsTianJieFuLu(itemData.Key.ItemType, itemData.Key.TemplateId);
					if (flag4)
					{
						bool isEnough = itemData.Amount >= ItemTemplateHelper.GetTianJieFuLuCountUnit();
						ViewPopupMenu.BtnData innerButton = new ViewPopupMenu.BtnData(LocalStringManager.Get(LanguageKey.LK_Eat_Item), canEatMore && isEnough, EItemMenuDisplayOrder.Eat, delegate()
						{
							this.OnClickEatItem(itemData);
						}, delegate()
						{
							this.SetEatItemInfectNotice(true, itemData, 1);
						}, delegate()
						{
							this.SetEatItemInfectNotice(false, null, 1);
						}, false);
						bool flag5 = !isEnough;
						if (flag5)
						{
							innerButton.SetTip(string.Empty, LanguageKey.LK_Mousetip_TianjieFulu_NotEnough.Tr().SetColor("brightred"));
						}
						btnList.Add(innerButton);
					}
					else
					{
						bool changeNeili = CommonUtils.CanItemEatForChangeNeili(itemData.Key.ItemType, itemData.Key.TemplateId);
						bool neiliIsNotMax = this._equipCombatSkillMonitor.CurrNeili < this._equipCombatSkillMonitor.MaxNeili;
						bool interactable2 = changeNeili ? (neiliIsNotMax && canEatMore) : canEatMore;
						string btnName2 = LocalStringManager.Get(LanguageKey.LK_Eat_Item);
						ViewPopupMenu.BtnData btnData2 = new ViewPopupMenu.BtnData(btnName2, interactable2, EItemMenuDisplayOrder.Eat, delegate()
						{
							this.OnClickEatItem(itemData);
						}, delegate()
						{
							this.SetEatItemInfectNotice(true, itemData, 1);
						}, delegate()
						{
							this.SetEatItemInfectNotice(false, null, 1);
						}, false);
						btnList.Add(btnData2);
						bool flag6 = !canEatMore;
						if (flag6)
						{
							btnData2.SetTip("", LocalStringManager.Get(LanguageKey.LK_Use_Medicine_Tip_NoSlot));
						}
						else
						{
							bool flag7 = changeNeili && !neiliIsNotMax;
							if (flag7)
							{
								btnData2.SetTip("", LocalStringManager.Get(LanguageKey.LK_ItemTips_Use_NeiliIsMax));
							}
						}
					}
				}
			}
			bool flag8 = btnList.Count > 0;
			if (flag8)
			{
				this._itemScrollView.SetItemToPopupMenuMode(itemData.Key, btnList, delegate
				{
					this.CancelHighLightItemView();
					itemView.SetHighLight(false);
				});
				this.HighLightItemView(itemView);
			}
		}

		// Token: 0x06004FF2 RID: 20466 RVA: 0x00256424 File Offset: 0x00254624
		private void HighLightItemView(ItemView itemView)
		{
			bool flag = null == itemView;
			if (!flag)
			{
				this._focusingTuple.Item1 = itemView;
				this._focusingTuple.Item2 = itemView.transform.parent;
				this._focusingTuple.Item3 = itemView.transform.GetSiblingIndex();
				RectTransform focusMask = this._itemScrollView.CGet<RectTransform>("FocusItemMask");
				itemView.transform.SetParent(focusMask, true);
				itemView.transform.localScale = Vector3.one;
				focusMask.gameObject.SetActive(true);
			}
		}

		// Token: 0x06004FF3 RID: 20467 RVA: 0x002564B4 File Offset: 0x002546B4
		private void CancelHighLightItemView()
		{
			this._itemScrollView.SortAndFilter.gameObject.SetActive(true);
			this.SetItemScrollViewCanScroll(true);
			bool flag = null != this._focusingTuple.Item1;
			if (flag)
			{
				this._focusingTuple.Item1.transform.SetParent(this._focusingTuple.Item2, true);
				this._focusingTuple.Item1.transform.SetSiblingIndex(this._focusingTuple.Item3);
				this._focusingTuple.Item1 = null;
				this._itemScrollView.CGet<RectTransform>("FocusItemMask").gameObject.SetActive(false);
			}
			this._characterAttributeView.HideInfectNotice(true, true);
		}

		// Token: 0x06004FF4 RID: 20468 RVA: 0x00256571 File Offset: 0x00254771
		private void SetItemScrollViewCanScroll(bool canScroll)
		{
			this._itemScrollView.GetComponent<CScrollRectLegacy>().SetScrollEnable(canScroll);
		}

		// Token: 0x06004FF5 RID: 20469 RVA: 0x00256586 File Offset: 0x00254786
		private void OnClickEatItem(ItemDisplayData itemData)
		{
			this._itemScrollView.SortAndFilter.gameObject.SetActive(false);
			UI_BeggarSkill3.<OnClickEatItem>g__Confirm|40_0();
		}

		// Token: 0x06004FF6 RID: 20470 RVA: 0x002565A8 File Offset: 0x002547A8
		private void CallRefreshItems()
		{
			CharacterDomainMethod.Call.GetAllInventoryItems(this.Element.GameDataListenerId, this._curCharId);
			this._charDataDict.Clear();
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this, this._normalCharList, delegate(int i, RawDataPool dataPool)
			{
				List<CharacterDisplayData> displayDataList = null;
				Serializer.Deserialize(dataPool, i, ref displayDataList);
				foreach (CharacterDisplayData data in displayDataList)
				{
					this._charDataDict[data.CharacterId] = data;
				}
				this._normalCharList.Sort(new Comparison<int>(this.CharCompare));
				this._charScroll.ReRender();
			});
		}

		// Token: 0x06004FF7 RID: 20471 RVA: 0x002565E8 File Offset: 0x002547E8
		private void AttributeInit()
		{
			this._characterAttributeView = base.CGet<CharacterAttributeDataView>("CharacterAttributeView");
			this._characterAttributeView.InitElements();
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Remove(element.OnListenerIdReady, new Action(this.OnAttributeListerReady));
			UIElement element2 = this.Element;
			element2.OnListenerIdReady = (Action)Delegate.Combine(element2.OnListenerIdReady, new Action(this.OnAttributeListerReady));
		}

		// Token: 0x06004FF8 RID: 20472 RVA: 0x00256661 File Offset: 0x00254861
		private void AttributeEnable()
		{
			this._lastAttributeTabIndex = CharacterAttributeDataView.CurTabIndex;
			CharacterAttributeDataView.CurTabIndex = 1;
		}

		// Token: 0x06004FF9 RID: 20473 RVA: 0x00256675 File Offset: 0x00254875
		private void OnAttributeListerReady()
		{
			this._characterAttributeView.SetCurrentCharacterId(this._taiwuCharId);
			this._characterAttributeView.RefreshAllHealBtn(this._taiwuCharId, true);
		}

		// Token: 0x06004FFA RID: 20474 RVA: 0x002566A0 File Offset: 0x002548A0
		public void SetEatItemInfectNotice(bool show, ItemDisplayData itemDisplayData = null, int amount = 1)
		{
			bool flag = show && itemDisplayData != null;
			if (flag)
			{
				this._characterAttributeView.ShowInfectNotice(itemDisplayData, amount);
			}
			else
			{
				this._characterAttributeView.HideInfectNotice(true, true);
			}
		}

		// Token: 0x06004FFB RID: 20475 RVA: 0x002566E0 File Offset: 0x002548E0
		public override void OnInit(ArgumentBox argsBox)
		{
			UI_BeggarSkill3.<>c__DisplayClass48_0 CS$<>8__locals1 = new UI_BeggarSkill3.<>c__DisplayClass48_0();
			CS$<>8__locals1.<>4__this = this;
			this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			this._mapModel = SingletonObject.getInstance<WorldMapModel>();
			this._equipCombatSkillMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<EquipCombatSkillMonitor>(this._taiwuCharId, false);
			this.AttributeInit();
			this.NeedDataListenerId = true;
			this.NeedWaitData = true;
			base.CGet<CButtonObsolete>("ButtonClosePopup").ClearAndAddListener(delegate
			{
				UIManager.Instance.HideUI(UIElement.BeggarSkill3);
			});
			this.ItemsAwake();
			this.MapCharListAwake();
			MapBlockData block = this._mapModel.GetBlockData(this._mapModel.CurrentLocation);
			this._normalCharList.Clear();
			bool flag = ((block != null) ? block.CharacterSet : null) != null;
			if (flag)
			{
				this._normalCharList.AddRange(block.CharacterSet);
			}
			this._items.Clear();
			this._mapCharItemsDic.Clear();
			CS$<>8__locals1.charCount = 0;
			using (List<int>.Enumerator enumerator = this._normalCharList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					int id = enumerator.Current;
					CharacterDomainMethod.AsyncCall.GetAllInventoryItems(this, id, delegate(int offset, RawDataPool pool)
					{
						List<ItemDisplayData> itemList = new List<ItemDisplayData>();
						Serializer.Deserialize(pool, offset, ref itemList);
						Dictionary<int, List<ItemDisplayData>> mapCharItemsDic = CS$<>8__locals1.<>4__this._mapCharItemsDic;
						int id = id;
						IEnumerable<ItemDisplayData> source = itemList;
						Func<ItemDisplayData, bool> predicate;
						if ((predicate = CS$<>8__locals1.<>9__3) == null)
						{
							predicate = (CS$<>8__locals1.<>9__3 = ((ItemDisplayData data) => CommonUtils.CanItemEat(data.Key.ItemType, data.Key.TemplateId, CS$<>8__locals1.<>4__this._taiwuCharId)));
						}
						mapCharItemsDic[id] = source.Where(predicate).ToList<ItemDisplayData>();
						int charCount = CS$<>8__locals1.charCount;
						CS$<>8__locals1.charCount = charCount + 1;
						bool flag2 = CS$<>8__locals1.charCount == CS$<>8__locals1.<>4__this._normalCharList.Count;
						if (flag2)
						{
							CS$<>8__locals1.<OnInit>g__CallBack|1();
						}
					});
				}
			}
			this.AttributeEnable();
		}

		// Token: 0x06004FFC RID: 20476 RVA: 0x00256864 File Offset: 0x00254A64
		private void OnDisable()
		{
			this._characterAttributeView.ReleaseCharacter();
			CharacterAttributeDataView.CurTabIndex = this._lastAttributeTabIndex;
		}

		// Token: 0x06005001 RID: 20481 RVA: 0x002569B6 File Offset: 0x00254BB6
		[CompilerGenerated]
		private void <OnClickItem>g__Action|35_0(ItemView realItemView)
		{
			this.ShowItemOperateMenu(realItemView.Data, realItemView);
		}

		// Token: 0x06005002 RID: 20482 RVA: 0x002569C7 File Offset: 0x00254BC7
		[CompilerGenerated]
		internal static void <OnClickEatItem>g__Confirm|40_0()
		{
		}

		// Token: 0x06005003 RID: 20483 RVA: 0x002569CA File Offset: 0x00254BCA
		[CompilerGenerated]
		private void <OnClickEatItem>g__Cancel|40_1()
		{
			this.SetEatItemInfectNotice(false, null, 1);
			this._itemScrollView.ReRender();
			this.CancelHighLightItemView();
		}

		// Token: 0x0400370B RID: 14091
		private int _taiwuCharId = -1;

		// Token: 0x0400370C RID: 14092
		private EquipCombatSkillMonitor _equipCombatSkillMonitor;

		// Token: 0x0400370D RID: 14093
		private Refers _mapBlockCharListRefers;

		// Token: 0x0400370E RID: 14094
		private const int TogKeyNormal = 0;

		// Token: 0x0400370F RID: 14095
		private const int TogKeyEnemy = 1;

		// Token: 0x04003710 RID: 14096
		private const int TogKeyCaravan = 2;

		// Token: 0x04003711 RID: 14097
		private const int TogKeyGrave = 3;

		// Token: 0x04003712 RID: 14098
		private readonly List<int> _normalCharList = new List<int>();

		// Token: 0x04003713 RID: 14099
		private readonly Dictionary<int, CharacterDisplayData> _charDataDict = new Dictionary<int, CharacterDisplayData>();

		// Token: 0x04003714 RID: 14100
		private InfinityScrollLegacy _charScroll;

		// Token: 0x04003715 RID: 14101
		private CToggleGroupObsolete _mapCharListTogGroup;

		// Token: 0x04003716 RID: 14102
		private readonly List<int> _searchedNormalCharList = new List<int>();

		// Token: 0x04003717 RID: 14103
		private WorldMapModel _mapModel;

		// Token: 0x04003718 RID: 14104
		private int _lastTogKey;

		// Token: 0x04003719 RID: 14105
		private const string MapBlockCharNormalKey = "MapBlockCharNormal";

		// Token: 0x0400371A RID: 14106
		private static TMP_InputField _searchInputField;

		// Token: 0x0400371B RID: 14107
		private Refers _curSelectedCharRefers;

		// Token: 0x0400371C RID: 14108
		private ItemScrollView _itemScrollView;

		// Token: 0x0400371D RID: 14109
		[TupleElementNames(new string[]
		{
			"focusingItemView",
			"parent",
			"sibling"
		})]
		private ValueTuple<ItemView, Transform, int> _focusingTuple;

		// Token: 0x0400371E RID: 14110
		private List<ItemDisplayData> _items = new List<ItemDisplayData>();

		// Token: 0x0400371F RID: 14111
		private Dictionary<int, List<ItemDisplayData>> _mapCharItemsDic = new Dictionary<int, List<ItemDisplayData>>();

		// Token: 0x04003720 RID: 14112
		private int _curCharId = -1;

		// Token: 0x04003721 RID: 14113
		private CharacterAttributeDataView _characterAttributeView;

		// Token: 0x04003722 RID: 14114
		private sbyte _lastAttributeTabIndex = 0;
	}
}
