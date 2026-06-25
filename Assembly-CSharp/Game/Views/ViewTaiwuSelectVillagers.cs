using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.CellContent;
using Game.Components.SortAndFilter;
using Game.Views.CharacterMenu;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views
{
	// Token: 0x02000714 RID: 1812
	public class ViewTaiwuSelectVillagers : UIBase
	{
		// Token: 0x060055FD RID: 22013 RVA: 0x0027D724 File Offset: 0x0027B924
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
			this.listScroll.RowSelectedProvider = delegate(int i, object o)
			{
				VillagerCharDisplayData data = o as VillagerCharDisplayData;
				return data != null && this._selectedCharId.Contains(data.CharacterId);
			};
			this.listScroll.RowDisabledProvider = delegate(int i, object o)
			{
				VillagerCharDisplayData data = o as VillagerCharDisplayData;
				return data == null || (int)data.PhysiologicalAge < GlobalConfig.Instance.AgeBaby;
			};
			this.confirm.gameObject.SetActive(true);
			this.selectAll.gameObject.SetActive(true);
			this.selectAll.SetIsOnWithoutNotify(false);
			this.expelCounter.gameObject.SetActive(true);
			this.expelCounter.text = LanguageKey.LK_VillagerInfo_Expel_Counter.TrFormat(0, this._dataList.Count((VillagerCharDisplayData x) => (int)x.PhysiologicalAge >= GlobalConfig.Instance.AgeBaby));
			this.listScroll.InfiniteScroll.ReRender();
			this.confirm.onClick.ResetListener(new Action(this.ConfirmExpel));
			this.selectAll.onValueChanged.ResetListener(new Action<bool>(this.SelectAll));
		}

		// Token: 0x060055FE RID: 22014 RVA: 0x0027D8C0 File Offset: 0x0027BAC0
		private void RequestDataIfUiChanged(ArgumentBox argsBox)
		{
			bool flag = !this._shouldRefreshData;
			if (flag)
			{
				bool flag2 = UIManager.Instance.IsFocusElement(UIElement.CharacterMenu);
				if (flag2)
				{
					this._shouldRefreshData = true;
				}
			}
			else
			{
				bool flag3 = UIManager.Instance.IsFocusElement(this.Element);
				if (flag3)
				{
					bool flag4 = this._ids == null;
					if (flag4)
					{
						CharacterDomainMethod.AsyncCall.GetVillagerCharDisplayDataList(this, new AsyncMethodCallbackDelegate(this.<RequestDataIfUiChanged>g__Handle|22_0));
					}
					else
					{
						CharacterDomainMethod.AsyncCall.GetCharDisplayDataListAsVillager(this, this._ids, new AsyncMethodCallbackDelegate(this.<RequestDataIfUiChanged>g__Handle|22_0));
					}
				}
			}
		}

		// Token: 0x060055FF RID: 22015 RVA: 0x0027D94B File Offset: 0x0027BB4B
		private void OnEnable()
		{
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.RequestDataIfUiChanged));
		}

		// Token: 0x06005600 RID: 22016 RVA: 0x0027D968 File Offset: 0x0027BB68
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.RequestDataIfUiChanged));
			this._selectedCharId.Clear();
			this._dataList.Clear();
			TabSortStateManager<ViewTaiwuSelectVillagers.VillagerSubPage, VillagerCharDisplayData> tabSortStateManager = this._tabSortStateManager;
			if (tabSortStateManager != null)
			{
				tabSortStateManager.ClearAll();
			}
			this._filteredDataList.Clear();
		}

		// Token: 0x06005601 RID: 22017 RVA: 0x0027D9C4 File Offset: 0x0027BBC4
		public override void OnInit(ArgumentBox argsBox)
		{
			if (argsBox != null)
			{
				argsBox.Get<Action<int[]>>("onMultiSelect", out this._onSelect);
			}
			bool flag = argsBox != null && argsBox.Get<List<int>>("CharIds", out this._ids);
			if (flag)
			{
				if (this._ids == null)
				{
					this._ids = new List<int>();
				}
			}
			else
			{
				this._ids = null;
			}
			this.expelTips.gameObject.SetActive(true);
			bool hideExpel;
			bool flag2 = argsBox != null && argsBox.Get("hideExpel", out hideExpel);
			if (flag2)
			{
				this.expelTips.gameObject.SetActive(!hideExpel);
			}
			bool isVillager = this._ids == null;
			this.title.text = (isVillager ? LanguageKey.LK_Taiwu_Villagers_Title.Tr() : LanguageKey.LK_VillagerInfo_Title_For_Map_Block_Character.Tr());
			this.subPageToggleGroup.Set(isVillager ? 0 : 1, false);
			this.subPageToggleGroup.Get(0).gameObject.SetActive(isVillager);
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.RequestData));
			this.selectAll.SetIsOnWithoutNotify(false);
		}

		// Token: 0x06005602 RID: 22018 RVA: 0x0027DAF8 File Offset: 0x0027BCF8
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 1)
				{
					ushort domainId = notification.DomainId;
					if (domainId != 4)
					{
						goto IL_60;
					}
					ushort methodId = notification.MethodId;
					if (methodId != 211 && methodId != 220)
					{
						goto IL_60;
					}
					bool flag = true;
					IL_63:
					bool flag2 = flag;
					if (flag2)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._dataList);
						this.RefreshList();
						this.Element.ShowAfterRefresh();
					}
					continue;
					IL_60:
					flag = false;
					goto IL_63;
				}
			}
		}

		// Token: 0x06005603 RID: 22019 RVA: 0x0027DBD0 File Offset: 0x0027BDD0
		private void InitSubPageToggles()
		{
			this.subPageToggleGroup.Init(-1);
			this.subPageToggleGroup.OnActiveIndexChange += delegate(int newIndex, int oldIndex)
			{
				this.OnSubPageChanged((ViewTaiwuSelectVillagers.VillagerSubPage)newIndex);
			};
			List<CToggle> toggles = this.subPageToggleGroup.GetAll();
			for (int i = 0; i < toggles.Count; i++)
			{
				CToggle toggle = toggles[i];
				TextMeshProUGUI label = toggle.transform.Find("Label").GetComponent<TextMeshProUGUI>();
				label.text = ViewTaiwuSelectVillagers.ToggleGroupNameKeys[i].Tr();
			}
		}

		// Token: 0x06005604 RID: 22020 RVA: 0x0027DC60 File Offset: 0x0027BE60
		private void InitSortAndFilter()
		{
			bool flag = this.sortAndFilter == null;
			if (!flag)
			{
				this._sortAndFilterController = new VillagerSortAndFilterController(this.sortAndFilter, new Func<int, bool>(this.IsTaiwu), new Func<int, bool>(this.IsSpecialTeammate));
				this._sortAndFilterController.Init(new Action(this.OnSortOrFilterChanged), "TeamSort");
				bool flag2 = this.listScroll != null;
				if (flag2)
				{
					this.listScroll.SetSortController(this._sortAndFilterController);
				}
				this._tabSortStateManager = new TabSortStateManager<ViewTaiwuSelectVillagers.VillagerSubPage, VillagerCharDisplayData>(this._sortAndFilterController);
			}
		}

		// Token: 0x06005605 RID: 22021 RVA: 0x0027DCFB File Offset: 0x0027BEFB
		private void InitListScroll()
		{
			this.listScroll.OnRowClicked += delegate(int i, RowItem row)
			{
				row.SetSelected(this.SwitchMultiSelect(i));
			};
		}

		// Token: 0x06005606 RID: 22022 RVA: 0x0027DD16 File Offset: 0x0027BF16
		private bool SwitchMultiSelect(int index)
		{
			return this.SwitchMultiSelect(this._filteredDataList[index]);
		}

		// Token: 0x06005607 RID: 22023 RVA: 0x0027DD2A File Offset: 0x0027BF2A
		private bool BanExpel(VillagerCharDisplayData data)
		{
			return (int)data.PhysiologicalAge < GlobalConfig.Instance.AgeBaby;
		}

		// Token: 0x06005608 RID: 22024 RVA: 0x0027DD3E File Offset: 0x0027BF3E
		private bool BanExpelOrSelected(VillagerCharDisplayData data)
		{
			return this.BanExpel(data) || this._selectedCharId.Contains(data.CharacterId);
		}

		// Token: 0x06005609 RID: 22025 RVA: 0x0027DD60 File Offset: 0x0027BF60
		private bool SwitchMultiSelect(VillagerCharDisplayData data)
		{
			int id = data.CharacterId;
			bool hasContent = this._selectedCharId.Add(id);
			bool flag = !hasContent;
			if (flag)
			{
				this._selectedCharId.Remove(id);
			}
			this.confirm.interactable = (this._selectedCharId.Count > 0);
			this.selectAll.SetIsOnWithoutNotify(this._filteredDataList.All(new Func<VillagerCharDisplayData, bool>(this.BanExpelOrSelected)));
			this.listScroll.InfiniteScroll.ReRender();
			this.expelCounter.text = LanguageKey.LK_VillagerInfo_Expel_Counter.TrFormat(this._selectedCharId.Count, this._dataList.Count((VillagerCharDisplayData x) => (int)x.PhysiologicalAge >= GlobalConfig.Instance.AgeBaby));
			return hasContent;
		}

		// Token: 0x0600560A RID: 22026 RVA: 0x0027DE3F File Offset: 0x0027C03F
		private void ConfirmExpel()
		{
			Action<int[]> onSelect = this._onSelect;
			if (onSelect != null)
			{
				onSelect(this._selectedCharId.ToArray<int>());
			}
			this.QuickHide();
		}

		// Token: 0x0600560B RID: 22027 RVA: 0x0027DE68 File Offset: 0x0027C068
		private void SelectAll(bool isSelect)
		{
			if (isSelect)
			{
				foreach (VillagerCharDisplayData item3 in from item in this._filteredDataList
				where (int)item.PhysiologicalAge >= GlobalConfig.Instance.AgeBaby
				select item)
				{
					this._selectedCharId.Add(item3.CharacterId);
				}
			}
			else
			{
				foreach (VillagerCharDisplayData item2 in this._filteredDataList)
				{
					this._selectedCharId.Remove(item2.CharacterId);
				}
			}
			this.confirm.interactable = (this._selectedCharId.Count > 0);
			this.expelCounter.text = LanguageKey.LK_VillagerInfo_Expel_Counter.TrFormat(this._selectedCharId.Count, this._dataList.Count((VillagerCharDisplayData x) => (int)x.PhysiologicalAge >= GlobalConfig.Instance.AgeBaby));
			this.listScroll.InfiniteScroll.ReRender();
		}

		// Token: 0x0600560C RID: 22028 RVA: 0x0027DFC0 File Offset: 0x0027C1C0
		private void ShowDropDownMenu(int index, RowItem row)
		{
			ViewTaiwuSelectVillagers.<>c__DisplayClass39_0 CS$<>8__locals1 = new ViewTaiwuSelectVillagers.<>c__DisplayClass39_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.index = index;
			CS$<>8__locals1.data = this._filteredDataList[CS$<>8__locals1.index];
			WorldMapModel model = SingletonObject.getInstance<WorldMapModel>();
			List<ViewPopupMenu.BtnData> btnList = new List<ViewPopupMenu.BtnData>(4);
			bool flag = (CS$<>8__locals1.data.CharacterTemplateId != -1 && Character.Instance[CS$<>8__locals1.data.CharacterTemplateId].CanOpenCharacterMenu) || CS$<>8__locals1.data.CreatingType == 1;
			if (flag)
			{
				btnList.Add(new ViewPopupMenu.BtnData(LanguageKey.LK_VillagerInfo_Info.Tr(), true, EItemMenuDisplayOrder.Info, delegate()
				{
					CS$<>8__locals1.<>4__this.ShowCharacterMenu(CS$<>8__locals1.index);
					base.<ShowDropDownMenu>g__OnCancel|5();
				}, null, null, false));
			}
			bool flag2 = CS$<>8__locals1.data.CreatingType == 1;
			if (flag2)
			{
				btnList.Add(CS$<>8__locals1.data.Followed ? new ViewPopupMenu.BtnData(LanguageKey.LK_VillagerInfo_Unfollowing.Tr(), true, EItemMenuDisplayOrder.Following, delegate()
				{
					TaiwuDomainMethod.Call.TaiwuUnfollowNpc(CS$<>8__locals1.data.CharacterId);
					CS$<>8__locals1.data.Followed = false;
					CS$<>8__locals1.<>4__this.listScroll.InfiniteScroll.ReRender();
					base.<ShowDropDownMenu>g__OnCancel|5();
				}, null, null, false) : new ViewPopupMenu.BtnData(LanguageKey.LK_VillagerInfo_Following.Tr(), true, EItemMenuDisplayOrder.Following, delegate()
				{
					TaiwuDomainMethod.Call.TaiwuFollowNpc(CS$<>8__locals1.data.CharacterId);
					CS$<>8__locals1.data.Followed = true;
					CS$<>8__locals1.<>4__this.listScroll.InfiniteScroll.ReRender();
					base.<ShowDropDownMenu>g__OnCancel|5();
				}, null, null, false));
			}
			bool flag3 = (int)CS$<>8__locals1.data.PhysiologicalAge >= GlobalConfig.Instance.AgeBaby && this._ids == null;
			if (flag3)
			{
				btnList.Add(new ViewPopupMenu.BtnData(LanguageKey.LK_VillagerInfo_Expel.Tr(), true, EItemMenuDisplayOrder.Expel, delegate()
				{
					base.<ShowDropDownMenu>g__OnCancel|5();
					UIElement dialog = UIElement.Dialog;
					ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
					string key = "Cmd";
					DialogCmd dialogCmd = new DialogCmd();
					dialogCmd.Title = LanguageKey.LK_VillagerInfo_Batch_Expel.Tr();
					dialogCmd.Content = LanguageKey.LK_VillagerInfo_Batch_Expel_Single_Confirm.TrFormat(NameCenter.GetMonasticTitleOrDisplayName(ref CS$<>8__locals1.data.NameData, false, false));
					dialogCmd.Type = 1;
					Action yes;
					if ((yes = CS$<>8__locals1.<>9__6) == null)
					{
						yes = (CS$<>8__locals1.<>9__6 = delegate()
						{
							TaiwuDomainMethod.Call.ExpelVillager(CS$<>8__locals1.data.CharacterId);
							CS$<>8__locals1.<>4__this.RequestData();
						});
					}
					dialogCmd.Yes = yes;
					dialogCmd.No = null;
					dialog.SetOnInitArgs(argumentBox.SetObject(key, dialogCmd));
					UIManager.Instance.MaskUI(UIElement.Dialog);
				}, null, null, false));
			}
			bool flag4 = model.GetStateId(CS$<>8__locals1.data.Location.AreaId) == model.CurrentStateId;
			if (flag4)
			{
				btnList.Add(new ViewPopupMenu.BtnData(LanguageKey.LK_VillagerInfo_GotoLocation.Tr(), true, EItemMenuDisplayOrder.Location, delegate()
				{
					CS$<>8__locals1.<>4__this.JumpToLocation(CS$<>8__locals1.<>4__this._filteredDataList[CS$<>8__locals1.index]);
					base.<ShowDropDownMenu>g__OnCancel|5();
				}, null, null, false));
			}
			bool flag5 = btnList.Count == 0;
			if (!flag5)
			{
				RectTransform itemRectTrans = row.transform as RectTransform;
				Vector3 itemScreenPos = UIManager.Instance.UiCamera.WorldToScreenPoint(itemRectTrans.position);
				Vector3 mouseScreenPos = Input.mousePosition;
				itemScreenPos.x = mouseScreenPos.x;
				UIElement.PopupMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("BtnInfo", btnList).SetObject("ScreenPos", itemScreenPos).SetObject("ItemSize", itemRectTrans.rect.size).SetObject("OnCancel", new Action(CS$<>8__locals1.<ShowDropDownMenu>g__OnCancel|5)));
				UIManager.Instance.ShowUI(UIElement.PopupMenu, true);
			}
		}

		// Token: 0x0600560D RID: 22029 RVA: 0x0027E237 File Offset: 0x0027C437
		private void ShowCharacterMenu(int index)
		{
			this.ShowCharacterMenu(this._filteredDataList[index]);
		}

		// Token: 0x0600560E RID: 22030 RVA: 0x0027E24C File Offset: 0x0027C44C
		private void ShowCharacterMenu(VillagerCharDisplayData data)
		{
			UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", data.CharacterId).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.CharacterBase, ECharacterSubPage.None)));
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x0600560F RID: 22031 RVA: 0x0027E2A4 File Offset: 0x0027C4A4
		private void RequestData()
		{
			this._selectedCharId.Clear();
			this.confirm.interactable = false;
			bool flag = this._ids == null;
			if (flag)
			{
				CharacterDomainMethod.Call.GetVillagerCharDisplayDataList(this.Element.GameDataListenerId);
			}
			else
			{
				CharacterDomainMethod.Call.GetCharDisplayDataListAsVillager(this.Element.GameDataListenerId, this._ids);
			}
		}

		// Token: 0x06005610 RID: 22032 RVA: 0x0027E302 File Offset: 0x0027C502
		public void OnGetVillagerCharDisplayDataList(List<VillagerCharDisplayData> dataList, int taiwuCharId)
		{
			this._taiwuCharId = taiwuCharId;
			this._dataList = (dataList ?? new List<VillagerCharDisplayData>());
			this.RefreshList();
		}

		// Token: 0x06005611 RID: 22033 RVA: 0x0027E323 File Offset: 0x0027C523
		private void RefreshList()
		{
			this.RefreshListStructure();
			this.RefreshListData();
		}

		// Token: 0x06005612 RID: 22034 RVA: 0x0027E334 File Offset: 0x0027C534
		private void RefreshListStructure()
		{
			IEnumerable<ColumnDefinition> columnDefinitions = this.GenerateColumnDefinitions(this._currentSubPage);
			this.PrepareRowTemplateContainers(this._currentSubPage);
			this.listScroll.ClearInfinityScrollCache();
			this.listScroll.Init<VillagerCharDisplayData>(columnDefinitions, true, null, null);
			this.BindCellStyleProvider();
		}

		// Token: 0x06005613 RID: 22035 RVA: 0x0027E380 File Offset: 0x0027C580
		private void RefreshListData()
		{
			VillagerSortAndFilterController sortAndFilterController = this._sortAndFilterController;
			Func<VillagerCharDisplayData, bool> func;
			if ((func = ((sortAndFilterController != null) ? sortAndFilterController.GenerateFilter() : null)) == null && (func = ViewTaiwuSelectVillagers.<>c.<>9__46_0) == null)
			{
				func = (ViewTaiwuSelectVillagers.<>c.<>9__46_0 = ((VillagerCharDisplayData _) => true));
			}
			Func<VillagerCharDisplayData, bool> filter = func;
			VillagerSortAndFilterController sortAndFilterController2 = this._sortAndFilterController;
			Comparison<VillagerCharDisplayData> comparer = (sortAndFilterController2 != null) ? sortAndFilterController2.GenerateComparer(this._dataList) : null;
			bool flag;
			if (comparer != null)
			{
				TabSortStateManager<ViewTaiwuSelectVillagers.VillagerSubPage, VillagerCharDisplayData> tabSortStateManager = this._tabSortStateManager;
				flag = (tabSortStateManager != null && tabSortStateManager.ShouldSort());
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				this._dataList.Sort(comparer);
			}
			else
			{
				this._dataList.Sort((VillagerCharDisplayData x, VillagerCharDisplayData y) => x.CharacterId.CompareTo(y.CharacterId));
			}
			this._filteredDataList = this._dataList.Where(filter).ToList<VillagerCharDisplayData>();
			this.listScroll.SetData<VillagerCharDisplayData>(this._filteredDataList, -1);
		}

		// Token: 0x06005614 RID: 22036 RVA: 0x0027E460 File Offset: 0x0027C660
		private void BindCellStyleProvider()
		{
			bool flag = this.listScroll == null;
			if (!flag)
			{
				this.listScroll.CellStyleProvider = delegate(ListStyleGeneralScroll.CellStyleContext context)
				{
					VillagerCharDisplayData rowData = context.RowData as VillagerCharDisplayData;
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
								bool flag5 = !ViewTaiwuSelectVillagers.TryGetComparableValue(this._currentSubPage, context.ColumnIndex, rowData, out value);
								if (flag5)
								{
									result = ListStyleGeneralScroll.CellStyle.Default;
								}
								else
								{
									int maxValue;
									bool flag6 = !ViewTaiwuSelectVillagers.TryGetMaxComparableValue(this._currentSubPage, context.ColumnIndex, this._filteredDataList, out maxValue);
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
											bool flag8 = ViewTaiwuSelectVillagers.TryGetMinComparableValue(this._currentSubPage, context.ColumnIndex, this._filteredDataList, out minValue) && minValue == maxValue;
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

		// Token: 0x06005615 RID: 22037 RVA: 0x0027E49C File Offset: 0x0027C69C
		private unsafe static bool TryGetComparableValue(ViewTaiwuSelectVillagers.VillagerSubPage subPage, int columnIndex, VillagerCharDisplayData data, out int value)
		{
			value = 0;
			int idx = columnIndex - 1;
			bool result;
			switch (subPage)
			{
			case ViewTaiwuSelectVillagers.VillagerSubPage.Role:
			{
				if (!true)
				{
				}
				int num = 0;
				if (!true)
				{
				}
				value = num;
				result = (idx >= 0 && idx < 10);
				break;
			}
			case ViewTaiwuSelectVillagers.VillagerSubPage.Villager:
			{
				ViewTaiwuSelectVillagers.EVillagerColumn col = (ViewTaiwuSelectVillagers.EVillagerColumn)idx;
				if (!true)
				{
				}
				int num;
				if (col != ViewTaiwuSelectVillagers.EVillagerColumn.Age)
				{
					switch (col)
					{
					case ViewTaiwuSelectVillagers.EVillagerColumn.Potential:
						num = data.Potential;
						break;
					case ViewTaiwuSelectVillagers.EVillagerColumn.Happiness:
						num = (int)data.Happiness;
						break;
					case ViewTaiwuSelectVillagers.EVillagerColumn.DefeatMark:
						num = (int)data.DefeatMarkCount;
						break;
					default:
						num = 0;
						break;
					}
				}
				else
				{
					num = (int)data.PhysiologicalAge;
				}
				if (!true)
				{
				}
				value = num;
				result = (idx >= 0 && idx < 10);
				break;
			}
			case ViewTaiwuSelectVillagers.VillagerSubPage.State:
			{
				ViewTaiwuSelectVillagers.EStateColumn col2 = (ViewTaiwuSelectVillagers.EStateColumn)idx;
				bool flag = col2 == ViewTaiwuSelectVillagers.EStateColumn.Favorability && !data.IsInteractedWithTaiwu;
				if (flag)
				{
					value = int.MinValue;
					result = true;
				}
				else
				{
					bool flag2 = col2 == ViewTaiwuSelectVillagers.EStateColumn.Charm && !ViewTaiwuSelectVillagers.IsCharmComparable(data);
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
						case ViewTaiwuSelectVillagers.EStateColumn.Age:
							num = (int)data.PhysiologicalAge;
							goto IL_1B1;
						case ViewTaiwuSelectVillagers.EStateColumn.DefeatMark:
							num = (int)data.DefeatMarkCount;
							goto IL_1B1;
						case ViewTaiwuSelectVillagers.EStateColumn.Charm:
							num = (int)data.Charm;
							goto IL_1B1;
						case ViewTaiwuSelectVillagers.EStateColumn.Happiness:
							num = (int)data.Happiness;
							goto IL_1B1;
						case ViewTaiwuSelectVillagers.EStateColumn.Favorability:
							num = (int)data.FavorabilityToTaiwu;
							goto IL_1B1;
						case ViewTaiwuSelectVillagers.EStateColumn.Alertness:
							num = data.Alertness;
							goto IL_1B1;
						case ViewTaiwuSelectVillagers.EStateColumn.Samsara:
							num = (int)data.PreexistenceCharCount;
							goto IL_1B1;
						case ViewTaiwuSelectVillagers.EStateColumn.Fame:
							num = (int)data.Fame;
							goto IL_1B1;
						}
						num = 0;
						IL_1B1:
						if (!true)
						{
						}
						value = num;
						result = (idx >= 0 && idx < 10);
					}
				}
				break;
			}
			case ViewTaiwuSelectVillagers.VillagerSubPage.Property:
			{
				ViewTaiwuSelectVillagers.EPropertyColumn col3 = (ViewTaiwuSelectVillagers.EPropertyColumn)idx;
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
					case ViewTaiwuSelectVillagers.EPropertyColumn.PenetrateOuter:
						num = data.Penetrations.Outer;
						break;
					case ViewTaiwuSelectVillagers.EPropertyColumn.PenetrateInner:
						num = data.Penetrations.Inner;
						break;
					case ViewTaiwuSelectVillagers.EPropertyColumn.ResistOuter:
						num = data.PenetrationResists.Outer;
						break;
					case ViewTaiwuSelectVillagers.EPropertyColumn.ResistInner:
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
			case ViewTaiwuSelectVillagers.VillagerSubPage.Property2:
			{
				ViewTaiwuSelectVillagers.EProperty2Column col4 = (ViewTaiwuSelectVillagers.EProperty2Column)idx;
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
						bool flag6 = col4 == ViewTaiwuSelectVillagers.EProperty2Column.QiDisorder;
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
			case ViewTaiwuSelectVillagers.VillagerSubPage.LifeSkill:
			{
				ViewTaiwuSelectVillagers.ELifeSkillColumn col5 = (ViewTaiwuSelectVillagers.ELifeSkillColumn)idx;
				bool flag7 = idx >= 0 && idx <= 15;
				if (flag7)
				{
					value = (int)(*data.LifeSkillQualifications[idx]);
					result = true;
				}
				else
				{
					bool flag8 = col5 == ViewTaiwuSelectVillagers.ELifeSkillColumn.Growth;
					if (flag8)
					{
						value = (int)ViewTaiwuSelectVillagers.GetSkillGrowthAddValue(data.ActualAge, (int)data.LifeSkillGrowthType);
						result = true;
					}
					else
					{
						result = false;
					}
				}
				break;
			}
			case ViewTaiwuSelectVillagers.VillagerSubPage.CombatSkill:
			{
				ViewTaiwuSelectVillagers.ECombatSkillColumn col6 = (ViewTaiwuSelectVillagers.ECombatSkillColumn)idx;
				bool flag9 = idx >= 0 && idx <= 13;
				if (flag9)
				{
					value = (int)(*data.CombatSkillQualifications[idx]);
					result = true;
				}
				else
				{
					bool flag10 = col6 == ViewTaiwuSelectVillagers.ECombatSkillColumn.Growth;
					if (flag10)
					{
						value = (int)ViewTaiwuSelectVillagers.GetSkillGrowthAddValue(data.ActualAge, (int)data.CombatSkillGrowthType);
						result = true;
					}
					else
					{
						result = false;
					}
				}
				break;
			}
			case ViewTaiwuSelectVillagers.VillagerSubPage.Personality:
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
			case ViewTaiwuSelectVillagers.VillagerSubPage.Item:
			{
				ViewTaiwuSelectVillagers.EItemColumn col7 = (ViewTaiwuSelectVillagers.EItemColumn)idx;
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
					if (col7 != ViewTaiwuSelectVillagers.EItemColumn.InventoryLoad)
					{
						if (col7 != ViewTaiwuSelectVillagers.EItemColumn.KidnapCount)
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
			case ViewTaiwuSelectVillagers.VillagerSubPage.Command:
			{
				ViewTaiwuSelectVillagers.ECommandColumn col8 = (ViewTaiwuSelectVillagers.ECommandColumn)idx;
				if (!true)
				{
				}
				int num;
				switch (col8)
				{
				case ViewTaiwuSelectVillagers.ECommandColumn.AttackMedal:
					num = data.AttackMedal;
					break;
				case ViewTaiwuSelectVillagers.ECommandColumn.DefenceMedal:
					num = data.DefenceMedal;
					break;
				case ViewTaiwuSelectVillagers.ECommandColumn.WisdomMedal:
					num = data.WisdomMedal;
					break;
				case ViewTaiwuSelectVillagers.ECommandColumn.Command0:
					num = (int)((data.Command.Items != null && data.Command.Items.CheckIndex(0)) ? data.Command.Items[0] : -1);
					break;
				case ViewTaiwuSelectVillagers.ECommandColumn.Command1:
					num = (int)((data.Command.Items != null && data.Command.Items.CheckIndex(1)) ? data.Command.Items[1] : -1);
					break;
				case ViewTaiwuSelectVillagers.ECommandColumn.Command2:
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

		// Token: 0x06005616 RID: 22038 RVA: 0x0027EA30 File Offset: 0x0027CC30
		private static bool TryGetMaxComparableValue(ViewTaiwuSelectVillagers.VillagerSubPage subPage, int columnIndex, List<VillagerCharDisplayData> list, out int maxValue)
		{
			maxValue = int.MinValue;
			bool found = false;
			foreach (VillagerCharDisplayData data in list)
			{
				int value;
				bool flag = !ViewTaiwuSelectVillagers.TryGetComparableValue(subPage, columnIndex, data, out value);
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

		// Token: 0x06005617 RID: 22039 RVA: 0x0027EAB4 File Offset: 0x0027CCB4
		private static bool TryGetMinComparableValue(ViewTaiwuSelectVillagers.VillagerSubPage subPage, int columnIndex, List<VillagerCharDisplayData> list, out int minValue)
		{
			minValue = int.MaxValue;
			bool found = false;
			foreach (VillagerCharDisplayData data in list)
			{
				int value;
				bool flag = !ViewTaiwuSelectVillagers.TryGetComparableValue(subPage, columnIndex, data, out value);
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

		// Token: 0x06005618 RID: 22040 RVA: 0x0027EB38 File Offset: 0x0027CD38
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitions(ViewTaiwuSelectVillagers.VillagerSubPage subPage)
		{
			yield return this.CreateAvatarWithNameColumn();
			switch (subPage)
			{
			case ViewTaiwuSelectVillagers.VillagerSubPage.Villager:
			{
				foreach (ColumnDefinition col in this.GenerateVillagerColumns())
				{
					yield return col;
					col = null;
				}
				IEnumerator<ColumnDefinition> enumerator = null;
				break;
			}
			case ViewTaiwuSelectVillagers.VillagerSubPage.State:
			{
				foreach (ColumnDefinition col2 in ViewTaiwuSelectVillagers.GenerateStateColumns())
				{
					yield return col2;
					col2 = null;
				}
				IEnumerator<ColumnDefinition> enumerator2 = null;
				break;
			}
			case ViewTaiwuSelectVillagers.VillagerSubPage.Property:
			{
				foreach (ColumnDefinition col3 in ViewTaiwuSelectVillagers.GeneratePropertyColumns())
				{
					yield return col3;
					col3 = null;
				}
				IEnumerator<ColumnDefinition> enumerator3 = null;
				break;
			}
			case ViewTaiwuSelectVillagers.VillagerSubPage.Property2:
			{
				foreach (ColumnDefinition col4 in ViewTaiwuSelectVillagers.GenerateProperty2Columns())
				{
					yield return col4;
					col4 = null;
				}
				IEnumerator<ColumnDefinition> enumerator4 = null;
				break;
			}
			case ViewTaiwuSelectVillagers.VillagerSubPage.LifeSkill:
			{
				foreach (ColumnDefinition col5 in ViewTaiwuSelectVillagers.GenerateLifeSkillColumns())
				{
					yield return col5;
					col5 = null;
				}
				IEnumerator<ColumnDefinition> enumerator5 = null;
				break;
			}
			case ViewTaiwuSelectVillagers.VillagerSubPage.CombatSkill:
			{
				foreach (ColumnDefinition col6 in ViewTaiwuSelectVillagers.GenerateCombatSkillColumns())
				{
					yield return col6;
					col6 = null;
				}
				IEnumerator<ColumnDefinition> enumerator6 = null;
				break;
			}
			case ViewTaiwuSelectVillagers.VillagerSubPage.Personality:
			{
				foreach (ColumnDefinition col7 in ViewTaiwuSelectVillagers.GeneratePersonalityColumns())
				{
					yield return col7;
					col7 = null;
				}
				IEnumerator<ColumnDefinition> enumerator7 = null;
				break;
			}
			case ViewTaiwuSelectVillagers.VillagerSubPage.Item:
			{
				foreach (ColumnDefinition col8 in ViewTaiwuSelectVillagers.GenerateItemColumns())
				{
					yield return col8;
					col8 = null;
				}
				IEnumerator<ColumnDefinition> enumerator8 = null;
				break;
			}
			case ViewTaiwuSelectVillagers.VillagerSubPage.Command:
			{
				foreach (ColumnDefinition col9 in ViewTaiwuSelectVillagers.GenerateCommandColumns())
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

		// Token: 0x06005619 RID: 22041 RVA: 0x0027EB4F File Offset: 0x0027CD4F
		private IEnumerable<RowCellContainer> GetCellContainerTemplates(ViewTaiwuSelectVillagers.VillagerSubPage subPage)
		{
			yield return this.avatarAndNameCellContainer;
			if (subPage != ViewTaiwuSelectVillagers.VillagerSubPage.Villager)
			{
				if (subPage != ViewTaiwuSelectVillagers.VillagerSubPage.Command)
				{
					int columnCount = ViewTaiwuSelectVillagers.GetColumnCount(subPage);
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
				int columnCount = ViewTaiwuSelectVillagers.GetColumnCount(subPage);
				int num;
				for (int k = 0; k < columnCount; k = num + 1)
				{
					yield return (k == 5) ? this.buttonCellContainer : this.singleTextCellContainer;
					num = k;
				}
			}
			yield break;
		}

		// Token: 0x0600561A RID: 22042 RVA: 0x0027EB68 File Offset: 0x0027CD68
		private static int GetColumnCount(ViewTaiwuSelectVillagers.VillagerSubPage subPage)
		{
			if (!true)
			{
			}
			int result;
			switch (subPage)
			{
			case ViewTaiwuSelectVillagers.VillagerSubPage.Role:
				result = 6;
				break;
			case ViewTaiwuSelectVillagers.VillagerSubPage.Villager:
				result = 9;
				break;
			case ViewTaiwuSelectVillagers.VillagerSubPage.State:
				result = 10;
				break;
			case ViewTaiwuSelectVillagers.VillagerSubPage.Property:
				result = 10;
				break;
			case ViewTaiwuSelectVillagers.VillagerSubPage.Property2:
				result = 9;
				break;
			case ViewTaiwuSelectVillagers.VillagerSubPage.LifeSkill:
				result = 17;
				break;
			case ViewTaiwuSelectVillagers.VillagerSubPage.CombatSkill:
				result = 15;
				break;
			case ViewTaiwuSelectVillagers.VillagerSubPage.Personality:
				result = 7;
				break;
			case ViewTaiwuSelectVillagers.VillagerSubPage.Item:
				result = 10;
				break;
			case ViewTaiwuSelectVillagers.VillagerSubPage.Command:
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

		// Token: 0x0600561B RID: 22043 RVA: 0x0027EBE8 File Offset: 0x0027CDE8
		private void PrepareRowTemplateContainers(ViewTaiwuSelectVillagers.VillagerSubPage subPage)
		{
			RowItem cachedTemplate;
			bool flag = this._rowTemplateCache.TryGetValue(subPage, out cachedTemplate);
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

		// Token: 0x0600561C RID: 22044 RVA: 0x0027EC3C File Offset: 0x0027CE3C
		private RowItem CreateRowTemplateForSubPage(ViewTaiwuSelectVillagers.VillagerSubPage subPage)
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

		// Token: 0x0600561D RID: 22045 RVA: 0x0027ECE0 File Offset: 0x0027CEE0
		private void PreCreateAllRowTemplates()
		{
			this.rowTemplate.gameObject.SetActive(false);
			foreach (object obj in Enum.GetValues(typeof(ViewTaiwuSelectVillagers.VillagerSubPage)))
			{
				ViewTaiwuSelectVillagers.VillagerSubPage subPage = (ViewTaiwuSelectVillagers.VillagerSubPage)obj;
				RowItem template = this.CreateRowTemplateForSubPage(subPage);
				this._rowTemplateCache[subPage] = template;
			}
		}

		// Token: 0x0600561E RID: 22046 RVA: 0x0027ED68 File Offset: 0x0027CF68
		private ColumnDefinition CreateAvatarWithNameColumn()
		{
			ColumnDefinition<VillagerCharDisplayData, AvatarWithNameCellData> columnDefinition = new ColumnDefinition<VillagerCharDisplayData, AvatarWithNameCellData>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 330f,
				FlexibleWidth = 0f,
				PreferredWidth = 330f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Char_Name.Tr());
			columnDefinition.CellDataGenerator = ((VillagerCharDisplayData data) => AvatarWithNameCellData.FromVillagerCharDisplayData(data, this.IsTaiwu(data.CharacterId), delegate(int _)
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
				argumentBox.Set("CharId", data.CharacterId);
			}));
			columnDefinition.SortId = 0;
			return columnDefinition;
		}

		// Token: 0x0600561F RID: 22047 RVA: 0x0027EDF8 File Offset: 0x0027CFF8
		private static ColumnDefinition CreateTextColumn(Func<string> headerKey, Func<VillagerCharDisplayData, string> valueGetter, short sortId = -1, float minWidth = 30f, float preferredWidth = 90f)
		{
			return new ColumnDefinition<VillagerCharDisplayData, string>
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

		// Token: 0x06005620 RID: 22048 RVA: 0x0027EE58 File Offset: 0x0027D058
		private static ColumnDefinition CreateIconAndTextColumn(Func<string> headerKey, Func<VillagerCharDisplayData, IconAndTextCellData> valueGetter, short sortId = -1, float minWidth = 80f, float preferredWidth = 120f)
		{
			return new ColumnDefinition<VillagerCharDisplayData, IconAndTextCellData>
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

		// Token: 0x06005621 RID: 22049 RVA: 0x0027EEB8 File Offset: 0x0027D0B8
		private ColumnDefinition CreateLocationColumn(Func<string> headerKey, short sortId = -1, float minWidth = 300f, float preferredWidth = 300f)
		{
			WorldMapModel model = SingletonObject.getInstance<WorldMapModel>();
			return new ColumnDefinition<VillagerCharDisplayData, SingleButtonCellData>
			{
				LayoutOption = new LayoutOption
				{
					MinWidth = minWidth,
					FlexibleWidth = 1f,
					PreferredWidth = preferredWidth,
					Priority = 1
				},
				TableHeadLabel = headerKey,
				CellDataGenerator = ((VillagerCharDisplayData data) => new SingleButtonCellData
				{
					LabelText = CommonUtils.GetRelativeLocationName(data.LocationNameRelatedData),
					OnClick = delegate()
					{
						this.JumpToLocation(data);
					},
					MouseTipText = ((UIElement.AdventureRemake.Exist || UIElement.AdventureMajorEvent.Exist) ? LanguageKey.LK_CharacterLocationFind_Tips_Disabled : LanguageKey.LK_CharacterLocationFind_Tips_NotAvailable).Tr()
				}),
				SortId = sortId
			};
		}

		// Token: 0x06005622 RID: 22050 RVA: 0x0027EF2C File Offset: 0x0027D12C
		private void JumpToLocation(VillagerCharDisplayData data)
		{
			WorldMapModel model = SingletonObject.getInstance<WorldMapModel>();
			bool flag = model.GetStateId(data.Location.AreaId) != model.CurrentStateId;
			if (!flag)
			{
				SingletonObject.getInstance<WorldMapModel>().JumpToTemporaryMark(data.Location, 0);
				this.QuickHide();
			}
		}

		// Token: 0x06005623 RID: 22051 RVA: 0x0027EF7B File Offset: 0x0027D17B
		private IEnumerable<ColumnDefinition> GenerateVillagerColumns()
		{
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Char_Age.Tr(), (VillagerCharDisplayData data) => (data.PhysiologicalAge >= 0) ? data.PhysiologicalAge.ToString() : LanguageKey.LK_Unknow.Tr(), 8, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_VillagerInfo_Role.Tr(), (VillagerCharDisplayData data) => CommonUtils.GetOrganizationGradeString(data.OrgInfo, data.Gender, data.PhysiologicalAge, (int)data.CharacterTemplateId), 1, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_VillagerInfo_Working_Status.Tr(), delegate(VillagerCharDisplayData data)
			{
				int num = VillagerSortController.WorkingStatusOrder(data);
				if (!true)
				{
				}
				string result;
				switch (num)
				{
				case 0:
					result = LanguageKey.LK_VillagerInfo_Idle.Tr();
					break;
				case 1:
					result = LanguageKey.LK_VillagerInfo_KeepGrave.Tr();
					break;
				case 2:
					result = LanguageKey.LK_VillagerInfo_ShopManage.Tr();
					break;
				case 3:
					result = LanguageKey.LK_VillagerInfo_Job.Tr();
					break;
				default:
					result = LanguageKey.LK_VillagerInfo_None.Tr();
					break;
				}
				if (!true)
				{
				}
				return result;
			}, 129, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_VillagerInfo_Working_Location.Tr(), delegate(VillagerCharDisplayData data)
			{
				VillagerWorkData workData = data.VillagerWorkData;
				return (workData != null && workData.BlockTemplateId != -1) ? BuildingBlock.Instance[workData.BuildingBlockIndex].Name : LanguageKey.LK_VillagerInfo_None.Tr();
			}, 126, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_VillagerInfo_Working_Role.Tr(), delegate(VillagerCharDisplayData data)
			{
				int num = VillagerSortController.WorkingRoleOrder(data);
				if (!true)
				{
				}
				string result;
				if (num != 0)
				{
					if (num != 1)
					{
						result = LanguageKey.LK_VillagerInfo_None.Tr();
					}
					else
					{
						result = LanguageKey.LK_VillagerInfo_Mentor.Tr();
					}
				}
				else
				{
					result = LanguageKey.LK_VillagerInfo_Mentee.Tr();
				}
				if (!true)
				{
				}
				return result;
			}, 127, 30f, 90f);
			yield return this.CreateLocationColumn(() => LanguageKey.LK_VillagerInfo_Location.Tr(), 125, 300f, 300f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_VillagerInfo_Potential.Tr(), (VillagerCharDisplayData data) => data.Potential.ToString(), 128, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_VillagerInfo_Happiness.Tr(), (VillagerCharDisplayData data) => CommonUtils.GetHappinessString(HappinessType.GetHappinessType(data.Happiness)), 12, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_VillagerInfo_DefeatMark.Tr(), (VillagerCharDisplayData data) => data.DefeatMarkCount.ToString(), 53, 30f, 90f);
			yield break;
		}

		// Token: 0x06005624 RID: 22052 RVA: 0x0027EF8B File Offset: 0x0027D18B
		private static IEnumerable<ColumnDefinition> GenerateStateColumns()
		{
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Char_Age.Tr(), (VillagerCharDisplayData data) => data.PhysiologicalAge.ToString(), 8, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Health.Tr(), (VillagerCharDisplayData data) => CommonUtils.GetCharacterHealthInfo(data.Health, data.MaxLeftHealth, data.CharacterId).Item1, 10, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Injury.Tr(), (VillagerCharDisplayData data) => data.DefeatMarkCount.ToString(), 53, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Charm.Tr(), (VillagerCharDisplayData data) => ViewTaiwuSelectVillagers.GetCharmDisplayString(data), 9, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Behavior.Tr(), (VillagerCharDisplayData data) => CommonUtils.GetBehaviorString(data.BehaviorType), 57, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Happiness.Tr(), (VillagerCharDisplayData data) => CommonUtils.GetHappinessString(HappinessType.GetHappinessType(data.Happiness)), 12, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Favorability.Tr(), new Func<VillagerCharDisplayData, string>(ViewTaiwuSelectVillagers.GetFavorDisplayString), 11, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Alertness.Tr(), (VillagerCharDisplayData data) => CommonUtils.GetAlertnessNameByValue(data.Alertness), 130, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Samsara.Tr(), (VillagerCharDisplayData data) => data.PreexistenceCharCount.ToString(), 58, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Fame.Tr(), (VillagerCharDisplayData data) => CommonUtils.GetFameString(FameType.GetFameType(data.Fame)), 59, 30f, 90f);
			yield break;
		}

		// Token: 0x06005625 RID: 22053 RVA: 0x0027EF94 File Offset: 0x0027D194
		private static IEnumerable<ColumnDefinition> GeneratePropertyColumns()
		{
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Strength.Tr(), (VillagerCharDisplayData data) => data.MaxMainAttributes[0].ToString(), 60, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Dexterity.Tr(), (VillagerCharDisplayData data) => data.MaxMainAttributes[1].ToString(), 61, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Concentration.Tr(), (VillagerCharDisplayData data) => data.MaxMainAttributes[2].ToString(), 62, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Vitality.Tr(), (VillagerCharDisplayData data) => data.MaxMainAttributes[3].ToString(), 63, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Energy.Tr(), (VillagerCharDisplayData data) => data.MaxMainAttributes[4].ToString(), 64, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Intelligence.Tr(), (VillagerCharDisplayData data) => data.MaxMainAttributes[5].ToString(), 65, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Penetrate_Outer.Tr(), (VillagerCharDisplayData data) => data.Penetrations.Outer.ToString(), 22, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Penetrate_Inner.Tr(), (VillagerCharDisplayData data) => data.Penetrations.Inner.ToString(), 23, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Penetrate_Resist_Outer.Tr(), (VillagerCharDisplayData data) => data.PenetrationResists.Outer.ToString(), 29, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Penetrate_Resist_Inner.Tr(), (VillagerCharDisplayData data) => data.PenetrationResists.Inner.ToString(), 30, 30f, 90f);
			yield break;
		}

		// Token: 0x06005626 RID: 22054 RVA: 0x0027EF9D File Offset: 0x0027D19D
		private static IEnumerable<ColumnDefinition> GenerateProperty2Columns()
		{
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_HitType_0.Tr(), (VillagerCharDisplayData data) => data.HitValues[0].ToString(), 24, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_HitType_1.Tr(), (VillagerCharDisplayData data) => data.HitValues[1].ToString(), 25, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_HitType_2.Tr(), (VillagerCharDisplayData data) => data.HitValues[2].ToString(), 26, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_HitType_3.Tr(), (VillagerCharDisplayData data) => data.HitValues[3].ToString(), 27, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_AvoidType_0.Tr(), (VillagerCharDisplayData data) => data.AvoidValues[0].ToString(), 33, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_AvoidType_1.Tr(), (VillagerCharDisplayData data) => data.AvoidValues[1].ToString(), 34, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_AvoidType_2.Tr(), (VillagerCharDisplayData data) => data.AvoidValues[2].ToString(), 35, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_AvoidType_3.Tr(), (VillagerCharDisplayData data) => data.AvoidValues[3].ToString(), 36, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Qi_Disorder.Tr(), (VillagerCharDisplayData data) => ((int)(data.DisorderOfQi / 10)).ToString(), 55, 30f, 90f);
			yield break;
		}

		// Token: 0x06005627 RID: 22055 RVA: 0x0027EFA6 File Offset: 0x0027D1A6
		private static IEnumerable<ColumnDefinition> GenerateLifeSkillColumns()
		{
			int num;
			for (int i = 0; i < 16; i = num + 1)
			{
				ViewTaiwuSelectVillagers.<>c__DisplayClass66_0 CS$<>8__locals1 = new ViewTaiwuSelectVillagers.<>c__DisplayClass66_0();
				CS$<>8__locals1.index = i;
				yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LocalStringManager.Get(string.Format("LK_LifeSkillType_{0}", CS$<>8__locals1.index)), (VillagerCharDisplayData data) => data.LifeSkillQualifications[CS$<>8__locals1.index].ToString(), (short)(66 + CS$<>8__locals1.index), 40f, 60f);
				CS$<>8__locals1 = null;
				num = i;
			}
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Growth.Tr(), (VillagerCharDisplayData data) => ViewTaiwuSelectVillagers.GetSkillGrowthString(data.ActualAge, data.LifeSkillGrowthType), 118, 30f, 90f);
			yield break;
		}

		// Token: 0x06005628 RID: 22056 RVA: 0x0027EFAF File Offset: 0x0027D1AF
		private static IEnumerable<ColumnDefinition> GenerateCombatSkillColumns()
		{
			int num;
			for (int i = 0; i < 14; i = num + 1)
			{
				ViewTaiwuSelectVillagers.<>c__DisplayClass67_0 CS$<>8__locals1 = new ViewTaiwuSelectVillagers.<>c__DisplayClass67_0();
				CS$<>8__locals1.index = i;
				yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LocalStringManager.Get(string.Format("LK_CombatSkillType_{0}", CS$<>8__locals1.index)), (VillagerCharDisplayData data) => data.CombatSkillQualifications[CS$<>8__locals1.index].ToString(), (short)(82 + CS$<>8__locals1.index), 40f, 60f);
				CS$<>8__locals1 = null;
				num = i;
			}
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Growth.Tr(), (VillagerCharDisplayData data) => ViewTaiwuSelectVillagers.GetSkillGrowthString(data.ActualAge, data.CombatSkillGrowthType), 119, 30f, 90f);
			yield break;
		}

		// Token: 0x06005629 RID: 22057 RVA: 0x0027EFB8 File Offset: 0x0027D1B8
		private static IEnumerable<ColumnDefinition> GeneratePersonalityColumns()
		{
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Personality_Calm_Name.Tr(), (VillagerCharDisplayData data) => data.Personalities[0].ToString(), 96, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Personality_Clever_Name.Tr(), (VillagerCharDisplayData data) => data.Personalities[1].ToString(), 97, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Personality_Enthusiastic_Name.Tr(), (VillagerCharDisplayData data) => data.Personalities[2].ToString(), 98, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Personality_Brave_Name.Tr(), (VillagerCharDisplayData data) => data.Personalities[3].ToString(), 99, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Personality_Firm_Name.Tr(), (VillagerCharDisplayData data) => data.Personalities[4].ToString(), 100, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Personality_Lucky_Name.Tr(), (VillagerCharDisplayData data) => data.Personalities[5].ToString(), 101, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Personality_Perceptive_Name.Tr(), (VillagerCharDisplayData data) => data.Personalities[6].ToString(), 102, 30f, 90f);
			yield break;
		}

		// Token: 0x0600562A RID: 22058 RVA: 0x0027EFC1 File Offset: 0x0027D1C1
		private static IEnumerable<ColumnDefinition> GenerateItemColumns()
		{
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Food.Tr(), (VillagerCharDisplayData data) => data.Resources[0].ToString(), 103, 40f, 60f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Wood.Tr(), (VillagerCharDisplayData data) => data.Resources[1].ToString(), 104, 40f, 60f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Metal.Tr(), (VillagerCharDisplayData data) => data.Resources[2].ToString(), 105, 40f, 60f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Jade.Tr(), (VillagerCharDisplayData data) => data.Resources[3].ToString(), 106, 40f, 60f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Fabric.Tr(), (VillagerCharDisplayData data) => data.Resources[4].ToString(), 107, 40f, 60f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Herb.Tr(), (VillagerCharDisplayData data) => data.Resources[5].ToString(), 108, 40f, 60f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Money.Tr(), (VillagerCharDisplayData data) => data.Resources[6].ToString(), 109, 40f, 60f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Authority.Tr(), (VillagerCharDisplayData data) => data.Resources[7].ToString(), 110, 40f, 60f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Inventory.Tr(), (VillagerCharDisplayData data) => ViewTaiwuSelectVillagers.GetInventoryLoadString(data.CurrInventoryLoad, data.MaxInventoryLoad), 37, 30f, 90f);
			yield return ViewTaiwuSelectVillagers.CreateTextColumn(() => LanguageKey.LK_Kidnap.Tr(), (VillagerCharDisplayData data) => data.KidnapCount.ToString(), 111, 30f, 90f);
			yield break;
		}

		// Token: 0x0600562B RID: 22059 RVA: 0x0027EFCA File Offset: 0x0027D1CA
		private static IEnumerable<ColumnDefinition> GenerateCommandColumns()
		{
			yield return ViewTaiwuSelectVillagers.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Attack.Tr(), (VillagerCharDisplayData data) => ViewTaiwuSelectVillagers.CreateMedalCellData(data.AttackMedal, ViewTaiwuSelectVillagers.EMedalType.Attack), 112, 80f, 120f);
			yield return ViewTaiwuSelectVillagers.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Defence.Tr(), (VillagerCharDisplayData data) => ViewTaiwuSelectVillagers.CreateMedalCellData(data.DefenceMedal, ViewTaiwuSelectVillagers.EMedalType.Defence), 113, 80f, 120f);
			yield return ViewTaiwuSelectVillagers.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Wisdom.Tr(), (VillagerCharDisplayData data) => ViewTaiwuSelectVillagers.CreateMedalCellData(data.WisdomMedal, ViewTaiwuSelectVillagers.EMedalType.Wisdom), 114, 80f, 120f);
			yield return ViewTaiwuSelectVillagers.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_0.Tr(), (VillagerCharDisplayData data) => ViewTaiwuSelectVillagers.CreateCommandCellData(data, 0), 115, 80f, 120f);
			yield return ViewTaiwuSelectVillagers.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_1.Tr(), (VillagerCharDisplayData data) => ViewTaiwuSelectVillagers.CreateCommandCellData(data, 1), 116, 80f, 120f);
			yield return ViewTaiwuSelectVillagers.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_2.Tr(), (VillagerCharDisplayData data) => ViewTaiwuSelectVillagers.CreateCommandCellData(data, 2), 117, 80f, 120f);
			yield break;
		}

		// Token: 0x0600562C RID: 22060 RVA: 0x0027EFD4 File Offset: 0x0027D1D4
		private static IconAndTextCellData CreateMedalCellData(int medalCount, ViewTaiwuSelectVillagers.EMedalType medalType)
		{
			bool flag = medalCount == 0;
			IconAndTextCellData result;
			if (flag)
			{
				result = IconAndTextCellData.TextOnly("-");
			}
			else
			{
				string iconName = ViewTaiwuSelectVillagers.GetMedalIconName(medalCount, medalType);
				string text = string.Format(" x{0}", Mathf.Abs(medalCount));
				result = new IconAndTextCellData(iconName, text, true, false, false, false);
			}
			return result;
		}

		// Token: 0x0600562D RID: 22061 RVA: 0x0027F028 File Offset: 0x0027D228
		private static string GetMedalIconName(int medalCount, ViewTaiwuSelectVillagers.EMedalType medalType)
		{
			int signKey = (medalCount > 0) ? 1 : ((medalCount < 0) ? -1 : 0);
			if (!true)
			{
			}
			string text;
			switch (medalType)
			{
			case ViewTaiwuSelectVillagers.EMedalType.Attack:
				text = MedalSummary.AttackMedalIconConfig[signKey];
				break;
			case ViewTaiwuSelectVillagers.EMedalType.Defence:
				text = MedalSummary.DefenceMedalIconConfig[signKey];
				break;
			case ViewTaiwuSelectVillagers.EMedalType.Wisdom:
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

		// Token: 0x0600562E RID: 22062 RVA: 0x0027F0A8 File Offset: 0x0027D2A8
		private static IconAndTextCellData CreateCommandCellData(VillagerCharDisplayData data, int commandIndex)
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

		// Token: 0x0600562F RID: 22063 RVA: 0x0027F134 File Offset: 0x0027D334
		private bool IsTaiwu(int charId)
		{
			return charId == this._taiwuCharId;
		}

		// Token: 0x06005630 RID: 22064 RVA: 0x0027F150 File Offset: 0x0027D350
		private bool IsSpecialTeammate(int charId)
		{
			VillagerCharDisplayData data = this._dataList.Find((VillagerCharDisplayData d) => d.CharacterId == charId);
			return data != null && data.IsSpecialGroupMember;
		}

		// Token: 0x06005631 RID: 22065 RVA: 0x0027F194 File Offset: 0x0027D394
		private static string GetCharmDisplayString(VillagerCharDisplayData data)
		{
			return CommonUtils.GetCharmLevelText(data.Charm, data.Gender, data.PhysiologicalAge, data.ClothDisplayId, CreatingType.IsFixedPresetType(data.CreatingType), data.FaceVisible);
		}

		// Token: 0x06005632 RID: 22066 RVA: 0x0027F1D4 File Offset: 0x0027D3D4
		public static bool IsCharmComparable(VillagerCharDisplayData data)
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

		// Token: 0x06005633 RID: 22067 RVA: 0x0027F238 File Offset: 0x0027D438
		private static string GetFavorDisplayString(VillagerCharDisplayData data)
		{
			return CommonUtils.GetFavorStringByInteracted(data.FavorabilityToTaiwu, data.IsInteractedWithTaiwu);
		}

		// Token: 0x06005634 RID: 22068 RVA: 0x0027F25C File Offset: 0x0027D45C
		private static string GetSkillGrowthString(short actualAge, sbyte growthType)
		{
			sbyte addValue = ViewTaiwuSelectVillagers.GetSkillGrowthAddValue(actualAge, (int)growthType);
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

		// Token: 0x06005635 RID: 22069 RVA: 0x0027F31C File Offset: 0x0027D51C
		private static sbyte GetSkillGrowthAddValue(short actualAge, int growthType)
		{
			AgeEffectItem ageData = AgeEffect.Instance[Math.Min((int)actualAge, AgeEffect.Instance.Count - 1)];
			return (growthType == 0) ? ageData.SkillQualificationAverage : ((growthType == 1) ? ageData.SkillQualificationPrecocious : ageData.SkillQualificationLateBlooming);
		}

		// Token: 0x06005636 RID: 22070 RVA: 0x0027F368 File Offset: 0x0027D568
		private static string GetInventoryLoadString(int currLoad, int maxLoad)
		{
			string currLoadStr = ((float)currLoad / 100f).ToString("f1").SetColor(CommonUtils.GetLoadWeightValueColor(currLoad, maxLoad));
			return string.Format("{0}/{1:f1}", currLoadStr, (float)maxLoad / 100f);
		}

		// Token: 0x06005637 RID: 22071 RVA: 0x0027F3B4 File Offset: 0x0027D5B4
		private void OnSubPageChanged(ViewTaiwuSelectVillagers.VillagerSubPage subPage)
		{
			bool flag = this._currentSubPage == subPage;
			if (!flag)
			{
				this._currentSubPage = subPage;
				TabSortStateManager<ViewTaiwuSelectVillagers.VillagerSubPage, VillagerCharDisplayData> tabSortStateManager = this._tabSortStateManager;
				if (tabSortStateManager != null)
				{
					tabSortStateManager.OnTabChange(subPage);
				}
				this.RefreshList();
			}
		}

		// Token: 0x06005638 RID: 22072 RVA: 0x0027F3F2 File Offset: 0x0027D5F2
		private void OnSortOrFilterChanged()
		{
			this.RefreshListData();
		}

		// Token: 0x17000A71 RID: 2673
		// (get) Token: 0x06005639 RID: 22073 RVA: 0x0027F3FC File Offset: 0x0027D5FC
		public static string SearchingText
		{
			get
			{
				ViewTaiwuSelectVillagers viewTaiwuSelectVillagers = UIElement.TaiwuSelectVillagers.UiBaseAs<ViewTaiwuSelectVillagers>();
				return (viewTaiwuSelectVillagers != null) ? viewTaiwuSelectVillagers.searchingField.text : null;
			}
		}

		// Token: 0x0600563F RID: 22079 RVA: 0x0027F543 File Offset: 0x0027D743
		[CompilerGenerated]
		private void <RequestDataIfUiChanged>g__Handle|22_0(int offset, RawDataPool pool)
		{
			Serializer.Deserialize(pool, offset, ref this._dataList);
			this.RefreshListData();
			this._shouldRefreshData = false;
		}

		// Token: 0x04003AE6 RID: 15078
		private static readonly List<LanguageKey> ToggleGroupNameKeys = new List<LanguageKey>
		{
			LanguageKey.LK_Main_SummaryInfo_Identity,
			LanguageKey.LK_Villager,
			LanguageKey.LK_Team_Tog_State,
			LanguageKey.LK_Team_Tog_Property,
			LanguageKey.LK_Team_Tog_Property_Hit,
			LanguageKey.LK_Team_Tog_LifeSkill,
			LanguageKey.LK_Team_Tog_CombatSkill,
			LanguageKey.LK_Team_Tog_Personality,
			LanguageKey.LK_Team_Tog_Item,
			LanguageKey.LK_Team_Tog_Command
		};

		// Token: 0x04003AE7 RID: 15079
		private ViewTaiwuSelectVillagers.VillagerSubPage _currentSubPage = ViewTaiwuSelectVillagers.VillagerSubPage.Villager;

		// Token: 0x04003AE8 RID: 15080
		private List<VillagerCharDisplayData> _dataList = new List<VillagerCharDisplayData>();

		// Token: 0x04003AE9 RID: 15081
		private List<VillagerCharDisplayData> _filteredDataList = new List<VillagerCharDisplayData>();

		// Token: 0x04003AEA RID: 15082
		private VillagerSortAndFilterController _sortAndFilterController;

		// Token: 0x04003AEB RID: 15083
		private TabSortStateManager<ViewTaiwuSelectVillagers.VillagerSubPage, VillagerCharDisplayData> _tabSortStateManager;

		// Token: 0x04003AEC RID: 15084
		private int _taiwuCharId = -1;

		// Token: 0x04003AED RID: 15085
		private readonly Dictionary<ViewTaiwuSelectVillagers.VillagerSubPage, RowItem> _rowTemplateCache = new Dictionary<ViewTaiwuSelectVillagers.VillagerSubPage, RowItem>();

		// Token: 0x04003AEE RID: 15086
		private readonly HashSet<int> _selectedCharId = new HashSet<int>();

		// Token: 0x04003AEF RID: 15087
		private bool _shouldRefreshData = false;

		// Token: 0x04003AF0 RID: 15088
		[SerializeField]
		private TMP_Text title;

		// Token: 0x04003AF1 RID: 15089
		private List<int> _ids;

		// Token: 0x04003AF2 RID: 15090
		private Action<int[]> _onSelect;

		// Token: 0x04003AF3 RID: 15091
		[SerializeField]
		private ListStyleGeneralScroll listScroll;

		// Token: 0x04003AF4 RID: 15092
		[SerializeField]
		private SortAndFilterLegacy sortAndFilter;

		// Token: 0x04003AF5 RID: 15093
		[SerializeField]
		private CToggleGroup subPageToggleGroup;

		// Token: 0x04003AF6 RID: 15094
		[SerializeField]
		private TMP_InputField searchingField;

		// Token: 0x04003AF7 RID: 15095
		[SerializeField]
		private CToggle selectAll;

		// Token: 0x04003AF8 RID: 15096
		[SerializeField]
		private CButton confirm;

		// Token: 0x04003AF9 RID: 15097
		[SerializeField]
		private TMP_Text expelCounter;

		// Token: 0x04003AFA RID: 15098
		[SerializeField]
		private TMP_Text expelTips;

		// Token: 0x04003AFB RID: 15099
		[SerializeField]
		private CButton btnClose;

		// Token: 0x04003AFC RID: 15100
		[Header("行模板配置")]
		[SerializeField]
		private RowItem rowTemplate;

		// Token: 0x04003AFD RID: 15101
		[SerializeField]
		private RowCellContainer singleTextCellContainer;

		// Token: 0x04003AFE RID: 15102
		[SerializeField]
		private RowCellContainer buttonCellContainer;

		// Token: 0x04003AFF RID: 15103
		[SerializeField]
		private RowCellContainer avatarAndNameCellContainer;

		// Token: 0x04003B00 RID: 15104
		[SerializeField]
		private RowCellContainer iconAndTextCellContainer;

		// Token: 0x02001B69 RID: 7017
		private enum EVillagerRoleColumn
		{
			// Token: 0x0400BA98 RID: 47768
			BehaviorType,
			// Token: 0x0400BA99 RID: 47769
			Personality,
			// Token: 0x0400BA9A RID: 47770
			SkillType1,
			// Token: 0x0400BA9B RID: 47771
			SkillType2,
			// Token: 0x0400BA9C RID: 47772
			SkillType3,
			// Token: 0x0400BA9D RID: 47773
			SkillType4,
			// Token: 0x0400BA9E RID: 47774
			Count
		}

		// Token: 0x02001B6A RID: 7018
		private enum EVillagerColumn
		{
			// Token: 0x0400BAA0 RID: 47776
			Age,
			// Token: 0x0400BAA1 RID: 47777
			GradeName,
			// Token: 0x0400BAA2 RID: 47778
			WorkStatus,
			// Token: 0x0400BAA3 RID: 47779
			WorkLocation,
			// Token: 0x0400BAA4 RID: 47780
			WorkRole,
			// Token: 0x0400BAA5 RID: 47781
			Location,
			// Token: 0x0400BAA6 RID: 47782
			Potential,
			// Token: 0x0400BAA7 RID: 47783
			Happiness,
			// Token: 0x0400BAA8 RID: 47784
			DefeatMark,
			// Token: 0x0400BAA9 RID: 47785
			Count
		}

		// Token: 0x02001B6B RID: 7019
		private enum EStateColumn
		{
			// Token: 0x0400BAAB RID: 47787
			Age,
			// Token: 0x0400BAAC RID: 47788
			Health,
			// Token: 0x0400BAAD RID: 47789
			DefeatMark,
			// Token: 0x0400BAAE RID: 47790
			Charm,
			// Token: 0x0400BAAF RID: 47791
			Behavior,
			// Token: 0x0400BAB0 RID: 47792
			Happiness,
			// Token: 0x0400BAB1 RID: 47793
			Favorability,
			// Token: 0x0400BAB2 RID: 47794
			Alertness,
			// Token: 0x0400BAB3 RID: 47795
			Samsara,
			// Token: 0x0400BAB4 RID: 47796
			Fame,
			// Token: 0x0400BAB5 RID: 47797
			Count
		}

		// Token: 0x02001B6C RID: 7020
		private enum EPropertyColumn
		{
			// Token: 0x0400BAB7 RID: 47799
			AttrStrength,
			// Token: 0x0400BAB8 RID: 47800
			AttrDexterity,
			// Token: 0x0400BAB9 RID: 47801
			AttrConcentration,
			// Token: 0x0400BABA RID: 47802
			AttrVitality,
			// Token: 0x0400BABB RID: 47803
			AttrEnergy,
			// Token: 0x0400BABC RID: 47804
			AttrIntelligence,
			// Token: 0x0400BABD RID: 47805
			PenetrateOuter,
			// Token: 0x0400BABE RID: 47806
			PenetrateInner,
			// Token: 0x0400BABF RID: 47807
			ResistOuter,
			// Token: 0x0400BAC0 RID: 47808
			ResistInner,
			// Token: 0x0400BAC1 RID: 47809
			Count
		}

		// Token: 0x02001B6D RID: 7021
		private enum EProperty2Column
		{
			// Token: 0x0400BAC3 RID: 47811
			HitStrength,
			// Token: 0x0400BAC4 RID: 47812
			HitTechnique,
			// Token: 0x0400BAC5 RID: 47813
			HitSpeed,
			// Token: 0x0400BAC6 RID: 47814
			HitMind,
			// Token: 0x0400BAC7 RID: 47815
			AvoidStrength,
			// Token: 0x0400BAC8 RID: 47816
			AvoidTechnique,
			// Token: 0x0400BAC9 RID: 47817
			AvoidSpeed,
			// Token: 0x0400BACA RID: 47818
			AvoidMind,
			// Token: 0x0400BACB RID: 47819
			QiDisorder,
			// Token: 0x0400BACC RID: 47820
			Count
		}

		// Token: 0x02001B6E RID: 7022
		private enum ELifeSkillColumn
		{
			// Token: 0x0400BACE RID: 47822
			Skill0,
			// Token: 0x0400BACF RID: 47823
			Skill1,
			// Token: 0x0400BAD0 RID: 47824
			Skill2,
			// Token: 0x0400BAD1 RID: 47825
			Skill3,
			// Token: 0x0400BAD2 RID: 47826
			Skill4,
			// Token: 0x0400BAD3 RID: 47827
			Skill5,
			// Token: 0x0400BAD4 RID: 47828
			Skill6,
			// Token: 0x0400BAD5 RID: 47829
			Skill7,
			// Token: 0x0400BAD6 RID: 47830
			Skill8,
			// Token: 0x0400BAD7 RID: 47831
			Skill9,
			// Token: 0x0400BAD8 RID: 47832
			Skill10,
			// Token: 0x0400BAD9 RID: 47833
			Skill11,
			// Token: 0x0400BADA RID: 47834
			Skill12,
			// Token: 0x0400BADB RID: 47835
			Skill13,
			// Token: 0x0400BADC RID: 47836
			Skill14,
			// Token: 0x0400BADD RID: 47837
			Skill15,
			// Token: 0x0400BADE RID: 47838
			Growth,
			// Token: 0x0400BADF RID: 47839
			Count
		}

		// Token: 0x02001B6F RID: 7023
		private enum ECombatSkillColumn
		{
			// Token: 0x0400BAE1 RID: 47841
			Skill0,
			// Token: 0x0400BAE2 RID: 47842
			Skill1,
			// Token: 0x0400BAE3 RID: 47843
			Skill2,
			// Token: 0x0400BAE4 RID: 47844
			Skill3,
			// Token: 0x0400BAE5 RID: 47845
			Skill4,
			// Token: 0x0400BAE6 RID: 47846
			Skill5,
			// Token: 0x0400BAE7 RID: 47847
			Skill6,
			// Token: 0x0400BAE8 RID: 47848
			Skill7,
			// Token: 0x0400BAE9 RID: 47849
			Skill8,
			// Token: 0x0400BAEA RID: 47850
			Skill9,
			// Token: 0x0400BAEB RID: 47851
			Skill10,
			// Token: 0x0400BAEC RID: 47852
			Skill11,
			// Token: 0x0400BAED RID: 47853
			Skill12,
			// Token: 0x0400BAEE RID: 47854
			Skill13,
			// Token: 0x0400BAEF RID: 47855
			Growth,
			// Token: 0x0400BAF0 RID: 47856
			Count
		}

		// Token: 0x02001B70 RID: 7024
		private enum EPersonalityColumn
		{
			// Token: 0x0400BAF2 RID: 47858
			P0,
			// Token: 0x0400BAF3 RID: 47859
			P1,
			// Token: 0x0400BAF4 RID: 47860
			P2,
			// Token: 0x0400BAF5 RID: 47861
			P3,
			// Token: 0x0400BAF6 RID: 47862
			P4,
			// Token: 0x0400BAF7 RID: 47863
			P5,
			// Token: 0x0400BAF8 RID: 47864
			P6,
			// Token: 0x0400BAF9 RID: 47865
			Count
		}

		// Token: 0x02001B71 RID: 7025
		private enum EItemColumn
		{
			// Token: 0x0400BAFB RID: 47867
			Food,
			// Token: 0x0400BAFC RID: 47868
			Wood,
			// Token: 0x0400BAFD RID: 47869
			Metal,
			// Token: 0x0400BAFE RID: 47870
			Jade,
			// Token: 0x0400BAFF RID: 47871
			Fabric,
			// Token: 0x0400BB00 RID: 47872
			Herb,
			// Token: 0x0400BB01 RID: 47873
			Money,
			// Token: 0x0400BB02 RID: 47874
			Authority,
			// Token: 0x0400BB03 RID: 47875
			InventoryLoad,
			// Token: 0x0400BB04 RID: 47876
			KidnapCount,
			// Token: 0x0400BB05 RID: 47877
			Count
		}

		// Token: 0x02001B72 RID: 7026
		private enum ECommandColumn
		{
			// Token: 0x0400BB07 RID: 47879
			AttackMedal,
			// Token: 0x0400BB08 RID: 47880
			DefenceMedal,
			// Token: 0x0400BB09 RID: 47881
			WisdomMedal,
			// Token: 0x0400BB0A RID: 47882
			Command0,
			// Token: 0x0400BB0B RID: 47883
			Command1,
			// Token: 0x0400BB0C RID: 47884
			Command2,
			// Token: 0x0400BB0D RID: 47885
			Count
		}

		// Token: 0x02001B73 RID: 7027
		private enum EMedalType
		{
			// Token: 0x0400BB0F RID: 47887
			Attack,
			// Token: 0x0400BB10 RID: 47888
			Defence,
			// Token: 0x0400BB11 RID: 47889
			Wisdom
		}

		// Token: 0x02001B74 RID: 7028
		public enum VillagerSubPage
		{
			// Token: 0x0400BB13 RID: 47891
			Role,
			// Token: 0x0400BB14 RID: 47892
			Villager,
			// Token: 0x0400BB15 RID: 47893
			State,
			// Token: 0x0400BB16 RID: 47894
			Property,
			// Token: 0x0400BB17 RID: 47895
			Property2,
			// Token: 0x0400BB18 RID: 47896
			LifeSkill,
			// Token: 0x0400BB19 RID: 47897
			CombatSkill,
			// Token: 0x0400BB1A RID: 47898
			Personality,
			// Token: 0x0400BB1B RID: 47899
			Item,
			// Token: 0x0400BB1C RID: 47900
			Command
		}
	}
}
