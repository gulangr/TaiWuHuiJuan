using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using Config.Common;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Components.Common;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.CellContent;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.SelectCharacter;
using Game.Views.CharacterMenu;
using Game.Views.Select;
using GameData.Domains.Building;
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
	// Token: 0x02000715 RID: 1813
	public class ViewTaiwuVillagers : UIBase
	{
		// Token: 0x06005645 RID: 22085 RVA: 0x0027F78C File Offset: 0x0027D98C
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
			this.multiSelect.onValueChanged.ResetListener(delegate(bool isOn)
			{
				this._selectedCharId.Clear();
				this.listScroll.RowSelectedProvider = (isOn ? delegate(int i, object o)
				{
					VillagerCharDisplayData data = o as VillagerCharDisplayData;
					return data != null && this._selectedCharId.Contains(data.CharacterId);
				} : null);
				ListStyleGeneralScroll listStyleGeneralScroll = this.listScroll;
				Func<int, object, bool> rowDisabledProvider;
				if (!isOn)
				{
					rowDisabledProvider = null;
				}
				else
				{
					rowDisabledProvider = delegate(int i, object o)
					{
						VillagerCharDisplayData data = o as VillagerCharDisplayData;
						return data == null || (int)data.PhysiologicalAge < GlobalConfig.Instance.AgeBaby;
					};
				}
				listStyleGeneralScroll.RowDisabledProvider = rowDisabledProvider;
				this.confirm.gameObject.SetActive(isOn);
				this.selectAll.gameObject.SetActive(isOn);
				this.selectAll.SetIsOnWithoutNotify(false);
				this._selectedCharId.Clear();
				this.expelTips.gameObject.SetActive(isOn);
				this.expelCounter.gameObject.SetActive(isOn);
				this.expelCounter.text = LanguageKey.LK_VillagerInfo_Expel_Counter.TrFormat(0, this._dataList.Count((VillagerCharDisplayData x) => (int)x.PhysiologicalAge >= GlobalConfig.Instance.AgeBaby));
				this.listScroll.InfiniteScroll.ReRender();
			});
			this.confirm.onClick.ResetListener(new Action(this.ConfirmExpel));
			this.selectAll.onValueChanged.ResetListener(new Action<bool>(this.SelectAll));
			this.multiSelect.SetIsOnWithoutNotify(true);
		}

		// Token: 0x06005646 RID: 22086 RVA: 0x0027F854 File Offset: 0x0027DA54
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
						CharacterDomainMethod.AsyncCall.GetVillagerCharDisplayDataList(this, new AsyncMethodCallbackDelegate(this.<RequestDataIfUiChanged>g__Handle|21_0));
					}
					else
					{
						CharacterDomainMethod.AsyncCall.GetCharDisplayDataListAsVillager(this, this._ids, new AsyncMethodCallbackDelegate(this.<RequestDataIfUiChanged>g__Handle|21_0));
					}
				}
			}
		}

		// Token: 0x06005647 RID: 22087 RVA: 0x0027F8DF File Offset: 0x0027DADF
		private void OnEnable()
		{
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.RequestDataIfUiChanged));
		}

		// Token: 0x06005648 RID: 22088 RVA: 0x0027F8FC File Offset: 0x0027DAFC
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.RequestDataIfUiChanged));
			this._selectedCharId.Clear();
			this._dataList.Clear();
			TabSortStateManager<VillagerSubPage, ISelectCharacterData> tabSortStateManager = this._tabSortStateManager;
			if (tabSortStateManager != null)
			{
				tabSortStateManager.ClearAll();
			}
			this._filteredDataList.Clear();
		}

		// Token: 0x06005649 RID: 22089 RVA: 0x0027F958 File Offset: 0x0027DB58
		public override void OnInit(ArgumentBox argsBox)
		{
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
			this.InitSortAndFilter();
			bool isVillager = this._ids == null;
			this.title.text = (isVillager ? LanguageKey.LK_Taiwu_Villagers_Title.Tr() : LanguageKey.LK_VillagerInfo_Title_For_Map_Block_Character.Tr());
			this.subPageToggleGroup.Set(isVillager ? 0 : 1, false);
			this.subPageToggleGroup.Get(0).gameObject.SetActive(isVillager);
			this.multiSelect.transform.parent.gameObject.SetActive(isVillager);
			this.NeedDataListenerId = true;
			this.multiSelect.isOn = false;
			this.bottomGroup.SetActive(isVillager);
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.RequestData));
		}

		// Token: 0x0600564A RID: 22090 RVA: 0x0027FA64 File Offset: 0x0027DC64
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
						goto IL_63;
					}
					ushort methodId = notification.MethodId;
					if (methodId != 211 && methodId != 220)
					{
						goto IL_63;
					}
					bool flag = true;
					IL_66:
					bool flag2 = flag;
					if (flag2)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._dataList);
						bool flag3 = this.cannotMultiSelectReasonDisplayer.enabled = this._dataList.All(new Func<VillagerCharDisplayData, bool>(this.BanExpel));
						if (flag3)
						{
							this.multiSelect.interactable = (this.multiSelect.isOn = false);
						}
						else
						{
							this.multiSelect.interactable = true;
						}
						this.RefreshList();
						this.Element.ShowAfterRefresh();
					}
					continue;
					IL_63:
					flag = false;
					goto IL_66;
				}
			}
		}

		// Token: 0x0600564B RID: 22091 RVA: 0x0027FBA8 File Offset: 0x0027DDA8
		private void InitSubPageToggles()
		{
			this.subPageToggleGroup.Init(-1);
			ToggleGroupHotkeyController.Set(this.Element, this.subPageToggleGroup, 1, null);
			this.subPageToggleGroup.OnActiveIndexChange += delegate(int newIndex, int oldIndex)
			{
				this.OnSubPageChanged((VillagerSubPage)newIndex);
			};
			List<CToggle> toggles = this.subPageToggleGroup.GetAll();
			for (int i = 0; i < toggles.Count; i++)
			{
				CToggle toggle = toggles[i];
				TextMeshProUGUI label = toggle.transform.Find("Label").GetComponent<TextMeshProUGUI>();
				label.text = ViewTaiwuVillagers.ToggleGroupNameKeys[i].Tr();
			}
		}

		// Token: 0x0600564C RID: 22092 RVA: 0x0027FC4C File Offset: 0x0027DE4C
		private void InitSortAndFilter()
		{
			bool flag = this.sortAndFilter == null;
			if (!flag)
			{
				ISortAndFilterView sortAndFilterView = this.sortAndFilter;
				List<ESelectCharacterFilterMenuId> menuIds;
				if (this._ids != null)
				{
					List<ESelectCharacterFilterMenuId> list = new List<ESelectCharacterFilterMenuId>();
					list.Add(ESelectCharacterFilterMenuId.Gender);
					list.Add(ESelectCharacterFilterMenuId.BehaviorType);
					list.Add(ESelectCharacterFilterMenuId.Relation);
					list.Add(ESelectCharacterFilterMenuId.Organization);
					menuIds = list;
					list.Add(ESelectCharacterFilterMenuId.Sect);
				}
				else
				{
					List<ESelectCharacterFilterMenuId> list2 = new List<ESelectCharacterFilterMenuId>();
					list2.Add(ESelectCharacterFilterMenuId.Gender);
					list2.Add(ESelectCharacterFilterMenuId.BehaviorType);
					list2.Add(ESelectCharacterFilterMenuId.Relation);
					list2.Add(ESelectCharacterFilterMenuId.WorkStatus);
					list2.Add(ESelectCharacterFilterMenuId.RoleArrangementWork);
					menuIds = list2;
					list2.Add(ESelectCharacterFilterMenuId.Identity);
				}
				this._sortAndFilterController = new SelectCharacterSortAndFilterController(sortAndFilterView, menuIds, null, false);
				this._sortAndFilterController.Init(new Action(this.OnSortOrFilterChanged), (this._ids == null) ? "VillagerInfoSort" : "MapBlockCharacterDetailSort");
				bool flag2 = this.listScroll != null;
				if (flag2)
				{
					this.listScroll.SetSortController(this._sortAndFilterController);
				}
				this._tabSortStateManager = new TabSortStateManager<VillagerSubPage, ISelectCharacterData>(this._sortAndFilterController);
			}
		}

		// Token: 0x0600564D RID: 22093 RVA: 0x0027FD51 File Offset: 0x0027DF51
		private void InitListScroll()
		{
			this.listScroll.OnRowClicked += delegate(int i, RowItem row)
			{
				bool isOn = this.multiSelect.isOn;
				if (isOn)
				{
					row.SetSelected(this.SwitchMultiSelect(i));
				}
				else
				{
					this.ShowDropDownMenu(i, row);
				}
			};
		}

		// Token: 0x0600564E RID: 22094 RVA: 0x0027FD6C File Offset: 0x0027DF6C
		private bool SwitchMultiSelect(int index)
		{
			return this.SwitchMultiSelect(this._filteredDataList[index]);
		}

		// Token: 0x0600564F RID: 22095 RVA: 0x0027FD80 File Offset: 0x0027DF80
		private bool BanExpel(VillagerCharDisplayData data)
		{
			return (int)data.PhysiologicalAge < GlobalConfig.Instance.AgeBaby;
		}

		// Token: 0x06005650 RID: 22096 RVA: 0x0027FD94 File Offset: 0x0027DF94
		private bool BanExpelOrSelected(VillagerCharDisplayData data)
		{
			return this.BanExpel(data) || this._selectedCharId.Contains(data.CharacterId);
		}

		// Token: 0x06005651 RID: 22097 RVA: 0x0027FDB4 File Offset: 0x0027DFB4
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

		// Token: 0x06005652 RID: 22098 RVA: 0x0027FE94 File Offset: 0x0027E094
		private void ConfirmExpel()
		{
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
			{
				Title = LanguageKey.LK_VillagerInfo_Batch_Expel.Tr(),
				Content = LanguageKey.LK_VillagerInfo_Batch_Expel_Confirm.TrFormat(this._selectedCharId.Count),
				Type = 1,
				Yes = delegate()
				{
					TaiwuDomainMethod.Call.ExpelVillagers(this._selectedCharId.ToList<int>());
					this.RequestData();
				},
				No = null
			}));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x06005653 RID: 22099 RVA: 0x0027FF24 File Offset: 0x0027E124
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

		// Token: 0x06005654 RID: 22100 RVA: 0x0028007C File Offset: 0x0027E27C
		private void ShowDropDownMenu(int index, RowItem row)
		{
			ViewTaiwuVillagers.<>c__DisplayClass37_0 CS$<>8__locals1 = new ViewTaiwuVillagers.<>c__DisplayClass37_0();
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
					base.<ShowDropDownMenu>g__OnCancel|0();
				}, null, null, false)
				{
					TipTitle = LanguageKey.LK_VillagerInfo_Tips_ViewCharacter.Tr(),
					TipContent = LanguageKey.LK_VillagerInfo_Tips_ViewCharacter_Tips.Tr()
				});
			}
			bool flag2 = CS$<>8__locals1.data.CreatingType == 1;
			if (flag2)
			{
				List<ViewPopupMenu.BtnData> list = btnList;
				ViewPopupMenu.BtnData item;
				if (!CS$<>8__locals1.data.Followed)
				{
					ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(LanguageKey.LK_VillagerInfo_Following.Tr(), true, EItemMenuDisplayOrder.Following, delegate()
					{
						TaiwuDomainMethod.Call.TaiwuFollowNpc(CS$<>8__locals1.data.CharacterId);
						CS$<>8__locals1.data.Followed = true;
						CS$<>8__locals1.<>4__this.listScroll.InfiniteScroll.ReRender();
						base.<ShowDropDownMenu>g__OnCancel|0();
					}, null, null, false);
					btnData.TipTitle = LanguageKey.LK_VillagerInfo_Tips_Following_Title.Tr();
					item = btnData;
					btnData.TipContent = LanguageKey.LK_VillagerInfo_Tips_Following_Content.Tr();
				}
				else
				{
					ViewPopupMenu.BtnData btnData2 = new ViewPopupMenu.BtnData(LanguageKey.LK_VillagerInfo_Unfollowing.Tr(), true, EItemMenuDisplayOrder.Following, delegate()
					{
						TaiwuDomainMethod.Call.TaiwuUnfollowNpc(CS$<>8__locals1.data.CharacterId);
						CS$<>8__locals1.data.Followed = false;
						CS$<>8__locals1.<>4__this.listScroll.InfiniteScroll.ReRender();
						base.<ShowDropDownMenu>g__OnCancel|0();
					}, null, null, false);
					btnData2.TipTitle = LanguageKey.LK_VillagerInfo_Tips_Following_Title.Tr();
					item = btnData2;
					btnData2.TipContent = LanguageKey.LK_VillagerInfo_Tips_Following_Content.Tr();
				}
				list.Add(item);
			}
			else
			{
				btnList.Add(new ViewPopupMenu.BtnData(LanguageKey.LK_VillagerInfo_Following.Tr(), false, EItemMenuDisplayOrder.Following, delegate()
				{
				}, null, null, false)
				{
					TipTitle = LanguageKey.LK_VillagerInfo_Tips_Following_Title.Tr(),
					TipContent = LanguageKey.LK_VillagerInfo_Tips_Following_Content_Disable.Tr()
				});
			}
			bool flag3 = this._ids == null;
			if (flag3)
			{
				btnList.Add(new ViewPopupMenu.BtnData(LanguageKey.LK_VillagerInfo_Expel.Tr(), CS$<>8__locals1.data.PhysiologicalAge >= 16, EItemMenuDisplayOrder.Expel, delegate()
				{
					base.<ShowDropDownMenu>g__OnCancel|0();
					UIElement dialog = UIElement.Dialog;
					ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
					string key = "Cmd";
					DialogCmd dialogCmd = new DialogCmd();
					dialogCmd.Title = LanguageKey.LK_VillagerInfo_Tips_Expel_Title.Tr();
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
				}, null, null, false)
				{
					TipTitle = LanguageKey.LK_VillagerInfo_Tips_Expel_Title.Tr(),
					TipContent = ((CS$<>8__locals1.data.PhysiologicalAge >= 16) ? LanguageKey.LK_VillagerInfo_Tips_Expel_Content.Tr() : LanguageKey.LK_VillagerInfo_Expel_Child_Exception.Tr())
				});
			}
			btnList.Add(new ViewPopupMenu.BtnData(LanguageKey.LK_VillagerInfo_GotoLocation.Tr(), model.GetStateId(CS$<>8__locals1.data.Location.AreaId) == model.CurrentStateId && !ExternalRelationStateHelper.IsActive(CS$<>8__locals1.data.ExternalRelation, 188UL), EItemMenuDisplayOrder.Location, delegate()
			{
				CS$<>8__locals1.<>4__this.JumpToLocation(CS$<>8__locals1.<>4__this._filteredDataList[CS$<>8__locals1.index]);
				base.<ShowDropDownMenu>g__OnCancel|0();
			}, null, null, false)
			{
				TipTitle = LanguageKey.LK_VillagerInfo_Tips_CharacterLocationFind_Title.Tr(),
				TipContent = ((model.GetStateId(CS$<>8__locals1.data.Location.AreaId) != model.CurrentStateId) ? LanguageKey.LK_CharacterLocationFind_Tips_NotAvailable.Tr() : (ExternalRelationStateHelper.IsActive(CS$<>8__locals1.data.ExternalRelation, 188UL) ? LanguageKey.LK_CharacterLocationFind_Tips_Adventure.Tr() : (ExternalRelationStateHelper.IsActive(CS$<>8__locals1.data.ExternalRelation, 188UL) ? LanguageKey.LK_CharacterLocationFind_Tips_Unknown.Tr() : LanguageKey.LK_VillagerInfo_Tips_CharacterLocationFind_Available.Tr())))
			});
			bool flag4 = btnList.Count == 0;
			if (!flag4)
			{
				RectTransform itemRectTrans = row.transform as RectTransform;
				Vector3 itemScreenPos = UIManager.Instance.UiCamera.WorldToScreenPoint(itemRectTrans.position);
				Vector3 mouseScreenPos = Input.mousePosition;
				itemScreenPos.x = mouseScreenPos.x;
				UIElement.PopupMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("BtnInfo", btnList).SetObject("ScreenPos", itemScreenPos).SetObject("ItemSize", itemRectTrans.rect.size).SetObject("OnCancel", new Action(CS$<>8__locals1.<ShowDropDownMenu>g__OnCancel|0)));
				UIManager.Instance.ShowUI(UIElement.PopupMenu, true);
			}
		}

		// Token: 0x06005655 RID: 22101 RVA: 0x0028048C File Offset: 0x0027E68C
		private void ShowCharacterMenu(int index)
		{
			this.ShowCharacterMenu(this._filteredDataList[index]);
		}

		// Token: 0x06005656 RID: 22102 RVA: 0x002804A4 File Offset: 0x0027E6A4
		private void ShowCharacterMenu(VillagerCharDisplayData data)
		{
			UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", data.CharacterId).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.CharacterBase, ECharacterSubPage.None)));
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x06005657 RID: 22103 RVA: 0x002804FC File Offset: 0x0027E6FC
		private unsafe void RequestData()
		{
			this.multiSelect.isOn = false;
			this._selectedCharId.Clear();
			this.confirm.interactable = false;
			bool flag = this._ids == null;
			if (flag)
			{
				bool needCostMoreResource = GameData.Domains.Taiwu.SharedMethods.NeedCostMoreResource;
				if (needCostMoreResource)
				{
					TaiwuDomainMethod.AsyncCall.GetTotalVillagerMaintenance(this, delegate(int offset, RawDataPool pool)
					{
						ResourceInts data = default(ResourceInts);
						Serializer.Deserialize(pool, offset, ref data);
						int i = this.resources.Length;
						while (i-- > 0)
						{
							this.resources[i].text = LanguageKey.LK_VillagerInfo_Resource.TrFormat(Config.ResourceType.Instance[i].Name, -(*data[i]));
						}
						this.challengeRect.gameObject.SetActive(true);
					});
				}
				else
				{
					this.challengeRect.gameObject.SetActive(false);
				}
				TaiwuDomainMethod.AsyncCall.RequestTaiwuResourceDisplayData(this, delegate(int offset, RawDataPool pool)
				{
					TaiwuResourceDisplayData data = new TaiwuResourceDisplayData();
					Serializer.Deserialize(pool, offset, ref data);
					this.villagerCount.text = LanguageKey.LK_VillagerInfo_VillagerCount.TrFormat(data.IdleVillager, data.TaiwuPopulationTipsAdult, data.Villager);
					this.villagerExtraInfo.gameObject.SetActive(true);
				});
				CharacterDomainMethod.Call.GetVillagerCharDisplayDataList(this.Element.GameDataListenerId);
			}
			else
			{
				CharacterDomainMethod.Call.GetCharDisplayDataListAsVillager(this.Element.GameDataListenerId, this._ids);
				this.villagerExtraInfo.gameObject.SetActive(false);
			}
		}

		// Token: 0x06005658 RID: 22104 RVA: 0x002805C0 File Offset: 0x0027E7C0
		public void OnGetVillagerCharDisplayDataList(List<VillagerCharDisplayData> dataList, int taiwuCharId)
		{
			this._taiwuCharId = taiwuCharId;
			this._dataList = (dataList ?? new List<VillagerCharDisplayData>());
			this.RefreshList();
		}

		// Token: 0x06005659 RID: 22105 RVA: 0x002805E1 File Offset: 0x0027E7E1
		private void RefreshList()
		{
			this.RefreshListStructure();
			this.RefreshListData();
		}

		// Token: 0x0600565A RID: 22106 RVA: 0x002805F4 File Offset: 0x0027E7F4
		private void RefreshListStructure()
		{
			IEnumerable<ColumnDefinition> columnDefinitions = this.GenerateColumnDefinitions(this._currentSubPage);
			this.PrepareRowTemplateContainers(this._currentSubPage);
			this.listScroll.ClearInfinityScrollCache();
			this.listScroll.Init<VillagerCharDisplayData>(columnDefinitions, true, null, null);
			this.BindCellStyleProvider();
		}

		// Token: 0x0600565B RID: 22107 RVA: 0x00280640 File Offset: 0x0027E840
		private void RefreshListData()
		{
			ViewTaiwuVillagers.<>c__DisplayClass44_0 CS$<>8__locals1 = new ViewTaiwuVillagers.<>c__DisplayClass44_0();
			CS$<>8__locals1.<>4__this = this;
			ViewTaiwuVillagers.<>c__DisplayClass44_0 CS$<>8__locals2 = CS$<>8__locals1;
			SelectCharacterSortAndFilterController sortAndFilterController = this._sortAndFilterController;
			Func<ISelectCharacterData, bool> filter;
			if ((filter = ((sortAndFilterController != null) ? sortAndFilterController.GenerateFilter() : null)) == null && (filter = ViewTaiwuVillagers.<>c.<>9__44_0) == null)
			{
				filter = (ViewTaiwuVillagers.<>c.<>9__44_0 = ((ISelectCharacterData _) => true));
			}
			CS$<>8__locals2.filter = filter;
			SelectCharacterSortAndFilterController sortAndFilterController2 = this._sortAndFilterController;
			Comparison<ISelectCharacterData> comparison;
			if (sortAndFilterController2 == null)
			{
				comparison = null;
			}
			else
			{
				comparison = sortAndFilterController2.GenerateComparer((from x in this._dataList
				select x).ToList<ISelectCharacterData>());
			}
			Comparison<ISelectCharacterData> comparer = comparison;
			SelectCharacterSortAndFilterController sortAndFilterController3 = this._sortAndFilterController;
			if (sortAndFilterController3 != null)
			{
				sortAndFilterController3.AfterFilter(this._dataList);
			}
			bool flag;
			if (comparer != null)
			{
				TabSortStateManager<VillagerSubPage, ISelectCharacterData> tabSortStateManager = this._tabSortStateManager;
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
			this._filteredDataList = (from x in this._dataList
			where CS$<>8__locals1.filter(x) && (string.IsNullOrWhiteSpace(CS$<>8__locals1.<>4__this.searchingField.text) || NameCenter.GetMonasticTitleOrDisplayName(ref x.NameData, false, false).Contains(CS$<>8__locals1.<>4__this.searchingField.text))
			select x).ToList<VillagerCharDisplayData>();
			this.listScroll.SetData<VillagerCharDisplayData>(this._filteredDataList, -1);
			SelectCharacterSortAndFilterController sortAndFilterController4 = this._sortAndFilterController;
			if (sortAndFilterController4 != null)
			{
				sortAndFilterController4.SetFilteredCount(this._filteredDataList.Count);
			}
			this.noContent.SetActive(this._filteredDataList.Count == 0);
		}

		// Token: 0x0600565C RID: 22108 RVA: 0x002807B4 File Offset: 0x0027E9B4
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
								bool flag5 = !ViewTaiwuVillagers.TryGetComparableValue(this._currentSubPage, context.ColumnIndex, rowData, out value);
								if (flag5)
								{
									result = ListStyleGeneralScroll.CellStyle.Default;
								}
								else
								{
									int maxValue;
									bool flag6 = !ViewTaiwuVillagers.TryGetMaxComparableValue(this._currentSubPage, context.ColumnIndex, this._filteredDataList, out maxValue);
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
											bool flag8 = ViewTaiwuVillagers.TryGetMinComparableValue(this._currentSubPage, context.ColumnIndex, this._filteredDataList, out minValue) && minValue == maxValue;
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

		// Token: 0x0600565D RID: 22109 RVA: 0x002807F0 File Offset: 0x0027E9F0
		private unsafe static bool TryGetComparableValue(VillagerSubPage subPage, int columnIndex, VillagerCharDisplayData data, out int value)
		{
			value = 0;
			int idx = columnIndex - 1;
			bool result;
			switch (subPage)
			{
			case VillagerSubPage.Villager:
			{
				ViewTaiwuVillagers.EVillagerColumn col = (ViewTaiwuVillagers.EVillagerColumn)idx;
				if (!true)
				{
				}
				int num;
				if (col != ViewTaiwuVillagers.EVillagerColumn.Age)
				{
					switch (col)
					{
					case ViewTaiwuVillagers.EVillagerColumn.Potential:
						num = data.Potential;
						break;
					case ViewTaiwuVillagers.EVillagerColumn.Happiness:
						num = (int)data.Happiness;
						break;
					case ViewTaiwuVillagers.EVillagerColumn.DefeatMark:
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
			case VillagerSubPage.State:
			{
				ViewTaiwuVillagers.EStateColumn col2 = (ViewTaiwuVillagers.EStateColumn)idx;
				bool flag = col2 == ViewTaiwuVillagers.EStateColumn.Favorability && !data.IsInteractedWithTaiwu;
				if (flag)
				{
					value = int.MinValue;
					result = true;
				}
				else
				{
					bool flag2 = col2 == ViewTaiwuVillagers.EStateColumn.Charm && !ViewTaiwuVillagers.IsCharmComparable(data);
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
						case ViewTaiwuVillagers.EStateColumn.Age:
							num = (int)data.PhysiologicalAge;
							goto IL_181;
						case ViewTaiwuVillagers.EStateColumn.DefeatMark:
							num = (int)data.DefeatMarkCount;
							goto IL_181;
						case ViewTaiwuVillagers.EStateColumn.Charm:
							num = (int)data.Charm;
							goto IL_181;
						case ViewTaiwuVillagers.EStateColumn.Happiness:
							num = (int)data.Happiness;
							goto IL_181;
						case ViewTaiwuVillagers.EStateColumn.Favorability:
							num = (int)data.FavorabilityToTaiwu;
							goto IL_181;
						case ViewTaiwuVillagers.EStateColumn.Alertness:
							num = data.Alertness;
							goto IL_181;
						case ViewTaiwuVillagers.EStateColumn.Samsara:
							num = (int)data.PreexistenceCharCount;
							goto IL_181;
						case ViewTaiwuVillagers.EStateColumn.Fame:
							num = (int)data.Fame;
							goto IL_181;
						}
						num = 0;
						IL_181:
						if (!true)
						{
						}
						value = num;
						result = (idx >= 0 && idx < 10);
					}
				}
				break;
			}
			case VillagerSubPage.Property:
			{
				ViewTaiwuVillagers.EPropertyColumn col3 = (ViewTaiwuVillagers.EPropertyColumn)idx;
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
					case ViewTaiwuVillagers.EPropertyColumn.PenetrateOuter:
						num = data.Penetrations.Outer;
						break;
					case ViewTaiwuVillagers.EPropertyColumn.PenetrateInner:
						num = data.Penetrations.Inner;
						break;
					case ViewTaiwuVillagers.EPropertyColumn.ResistOuter:
						num = data.PenetrationResists.Outer;
						break;
					case ViewTaiwuVillagers.EPropertyColumn.ResistInner:
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
			case VillagerSubPage.Property2:
			{
				ViewTaiwuVillagers.EProperty2Column col4 = (ViewTaiwuVillagers.EProperty2Column)idx;
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
						bool flag6 = col4 == ViewTaiwuVillagers.EProperty2Column.QiDisorder;
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
			case VillagerSubPage.LifeSkill:
			{
				ViewTaiwuVillagers.ELifeSkillColumn col5 = (ViewTaiwuVillagers.ELifeSkillColumn)idx;
				bool flag7 = idx >= 0 && idx <= 15;
				if (flag7)
				{
					value = (int)(*data.LifeSkillQualifications[idx]);
					result = true;
				}
				else
				{
					bool flag8 = col5 == ViewTaiwuVillagers.ELifeSkillColumn.Growth;
					if (flag8)
					{
						value = (int)ViewTaiwuVillagers.GetSkillGrowthAddValue(data.ActualAge, (int)data.LifeSkillGrowthType);
						result = true;
					}
					else
					{
						result = false;
					}
				}
				break;
			}
			case VillagerSubPage.CombatSkill:
			{
				ViewTaiwuVillagers.ECombatSkillColumn col6 = (ViewTaiwuVillagers.ECombatSkillColumn)idx;
				bool flag9 = idx >= 0 && idx <= 13;
				if (flag9)
				{
					value = (int)(*data.CombatSkillQualifications[idx]);
					result = true;
				}
				else
				{
					bool flag10 = col6 == ViewTaiwuVillagers.ECombatSkillColumn.Growth;
					if (flag10)
					{
						value = (int)ViewTaiwuVillagers.GetSkillGrowthAddValue(data.ActualAge, (int)data.CombatSkillGrowthType);
						result = true;
					}
					else
					{
						result = false;
					}
				}
				break;
			}
			case VillagerSubPage.Personality:
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
			case VillagerSubPage.Item:
			{
				ViewTaiwuVillagers.EItemColumn col7 = (ViewTaiwuVillagers.EItemColumn)idx;
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
					if (col7 != ViewTaiwuVillagers.EItemColumn.InventoryLoad)
					{
						if (col7 != ViewTaiwuVillagers.EItemColumn.KidnapCount)
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
			case VillagerSubPage.Command:
			{
				ViewTaiwuVillagers.ECommandColumn col8 = (ViewTaiwuVillagers.ECommandColumn)idx;
				if (!true)
				{
				}
				int num;
				switch (col8)
				{
				case ViewTaiwuVillagers.ECommandColumn.AttackMedal:
					num = data.AttackMedal;
					break;
				case ViewTaiwuVillagers.ECommandColumn.DefenceMedal:
					num = data.DefenceMedal;
					break;
				case ViewTaiwuVillagers.ECommandColumn.WisdomMedal:
					num = data.WisdomMedal;
					break;
				case ViewTaiwuVillagers.ECommandColumn.Command0:
					num = (int)((data.Command.Items != null && data.Command.Items.CheckIndex(0)) ? data.Command.Items[0] : -1);
					break;
				case ViewTaiwuVillagers.ECommandColumn.Command1:
					num = (int)((data.Command.Items != null && data.Command.Items.CheckIndex(1)) ? data.Command.Items[1] : -1);
					break;
				case ViewTaiwuVillagers.ECommandColumn.Command2:
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

		// Token: 0x0600565E RID: 22110 RVA: 0x00280D54 File Offset: 0x0027EF54
		private static bool TryGetMaxComparableValue(VillagerSubPage subPage, int columnIndex, List<VillagerCharDisplayData> list, out int maxValue)
		{
			maxValue = int.MinValue;
			bool found = false;
			foreach (VillagerCharDisplayData data in list)
			{
				int value;
				bool flag = !ViewTaiwuVillagers.TryGetComparableValue(subPage, columnIndex, data, out value);
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

		// Token: 0x0600565F RID: 22111 RVA: 0x00280DD8 File Offset: 0x0027EFD8
		private static bool TryGetMinComparableValue(VillagerSubPage subPage, int columnIndex, List<VillagerCharDisplayData> list, out int minValue)
		{
			minValue = int.MaxValue;
			bool found = false;
			foreach (VillagerCharDisplayData data in list)
			{
				int value;
				bool flag = !ViewTaiwuVillagers.TryGetComparableValue(subPage, columnIndex, data, out value);
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

		// Token: 0x06005660 RID: 22112 RVA: 0x00280E5C File Offset: 0x0027F05C
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitions(VillagerSubPage subPage)
		{
			yield return this.CreateAvatarWithNameColumn();
			switch (subPage)
			{
			case VillagerSubPage.Villager:
			{
				foreach (ColumnDefinition col in this.GenerateVillagerColumns())
				{
					yield return col;
					col = null;
				}
				IEnumerator<ColumnDefinition> enumerator = null;
				break;
			}
			case VillagerSubPage.State:
			{
				foreach (ColumnDefinition col2 in ViewTaiwuVillagers.GenerateStateColumns())
				{
					yield return col2;
					col2 = null;
				}
				IEnumerator<ColumnDefinition> enumerator2 = null;
				break;
			}
			case VillagerSubPage.Property:
			{
				foreach (ColumnDefinition col3 in ViewTaiwuVillagers.GeneratePropertyColumns())
				{
					yield return col3;
					col3 = null;
				}
				IEnumerator<ColumnDefinition> enumerator3 = null;
				break;
			}
			case VillagerSubPage.Property2:
			{
				foreach (ColumnDefinition col4 in ViewTaiwuVillagers.GenerateProperty2Columns())
				{
					yield return col4;
					col4 = null;
				}
				IEnumerator<ColumnDefinition> enumerator4 = null;
				break;
			}
			case VillagerSubPage.LifeSkill:
			{
				foreach (ColumnDefinition col5 in ViewTaiwuVillagers.GenerateLifeSkillColumns())
				{
					yield return col5;
					col5 = null;
				}
				IEnumerator<ColumnDefinition> enumerator5 = null;
				break;
			}
			case VillagerSubPage.CombatSkill:
			{
				foreach (ColumnDefinition col6 in ViewTaiwuVillagers.GenerateCombatSkillColumns())
				{
					yield return col6;
					col6 = null;
				}
				IEnumerator<ColumnDefinition> enumerator6 = null;
				break;
			}
			case VillagerSubPage.Personality:
			{
				foreach (ColumnDefinition col7 in ViewTaiwuVillagers.GeneratePersonalityColumns())
				{
					yield return col7;
					col7 = null;
				}
				IEnumerator<ColumnDefinition> enumerator7 = null;
				break;
			}
			case VillagerSubPage.Item:
			{
				foreach (ColumnDefinition col8 in ViewTaiwuVillagers.GenerateItemColumns())
				{
					yield return col8;
					col8 = null;
				}
				IEnumerator<ColumnDefinition> enumerator8 = null;
				break;
			}
			case VillagerSubPage.Command:
			{
				foreach (ColumnDefinition col9 in ViewTaiwuVillagers.GenerateCommandColumns())
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

		// Token: 0x06005661 RID: 22113 RVA: 0x00280E73 File Offset: 0x0027F073
		private IEnumerable<RowCellContainer> GetCellContainerTemplates(VillagerSubPage subPage)
		{
			yield return this.avatarAndNameCellContainer;
			if (subPage != VillagerSubPage.Villager)
			{
				if (subPage != VillagerSubPage.Command)
				{
					int columnCount = ViewTaiwuVillagers.GetColumnCount(subPage);
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
				int columnCount = ViewTaiwuVillagers.GetColumnCount(subPage);
				int num;
				for (int k = 0; k < columnCount; k = num + 1)
				{
					yield return (k == 5) ? this.buttonCellContainer : this.singleTextCellContainer;
					num = k;
				}
			}
			yield break;
		}

		// Token: 0x06005662 RID: 22114 RVA: 0x00280E8C File Offset: 0x0027F08C
		private static int GetColumnCount(VillagerSubPage subPage)
		{
			if (!true)
			{
			}
			int result;
			switch (subPage)
			{
			case VillagerSubPage.Villager:
				result = 9;
				break;
			case VillagerSubPage.State:
				result = 10;
				break;
			case VillagerSubPage.Property:
				result = 10;
				break;
			case VillagerSubPage.Property2:
				result = 9;
				break;
			case VillagerSubPage.LifeSkill:
				result = 17;
				break;
			case VillagerSubPage.CombatSkill:
				result = 15;
				break;
			case VillagerSubPage.Personality:
				result = 7;
				break;
			case VillagerSubPage.Item:
				result = 10;
				break;
			case VillagerSubPage.Command:
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

		// Token: 0x06005663 RID: 22115 RVA: 0x00280F04 File Offset: 0x0027F104
		private void PrepareRowTemplateContainers(VillagerSubPage subPage)
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

		// Token: 0x06005664 RID: 22116 RVA: 0x00280F58 File Offset: 0x0027F158
		private RowItem CreateRowTemplateForSubPage(VillagerSubPage subPage)
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

		// Token: 0x06005665 RID: 22117 RVA: 0x00280FFC File Offset: 0x0027F1FC
		private void PreCreateAllRowTemplates()
		{
			this.rowTemplate.gameObject.SetActive(false);
			foreach (object obj in Enum.GetValues(typeof(VillagerSubPage)))
			{
				VillagerSubPage subPage = (VillagerSubPage)obj;
				RowItem template = this.CreateRowTemplateForSubPage(subPage);
				this._rowTemplateCache[subPage] = template;
			}
		}

		// Token: 0x06005666 RID: 22118 RVA: 0x00281084 File Offset: 0x0027F284
		private ColumnDefinition CreateAvatarWithNameColumn()
		{
			ColumnDefinition<VillagerCharDisplayData, AvatarWithNameCellData> columnDefinition = new ColumnDefinition<VillagerCharDisplayData, AvatarWithNameCellData>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 314f,
				FlexibleWidth = 0f,
				PreferredWidth = 314f,
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

		// Token: 0x06005667 RID: 22119 RVA: 0x00281114 File Offset: 0x0027F314
		private static ColumnDefinition CreateTextColumn(Func<string> headerKey, Func<VillagerCharDisplayData, string> valueGetter, short sortId = -1, float minWidth = 30f, float preferredWidth = 224f)
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

		// Token: 0x06005668 RID: 22120 RVA: 0x00281174 File Offset: 0x0027F374
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

		// Token: 0x06005669 RID: 22121 RVA: 0x002811D4 File Offset: 0x0027F3D4
		private ColumnDefinition CreateLocationColumn(Func<string> headerKey, short sortId = -1, float minWidth = 254f, float preferredWidth = 254f)
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
					MouseTipText = ((this.multiSelect.isOn || UIElement.AdventureRemake.Exist || UIElement.AdventureMajorEvent.Exist) ? LanguageKey.LK_CharacterLocationFind_Tips_Disabled : LanguageKey.LK_CharacterLocationFind_Tips_NotAvailable).Tr(),
					SingleButtonCellStatus = ((!this.multiSelect.isOn && !UIElement.AdventureRemake.Exist && !UIElement.AdventureMajorEvent.Exist && model.GetStateId(data.Location.AreaId) == model.CurrentStateId) ? SingleButtonCellStatus.EnableInteractable : SingleButtonCellStatus.DisableInteractable)
				}),
				SortId = sortId
			};
		}

		// Token: 0x0600566A RID: 22122 RVA: 0x00281258 File Offset: 0x0027F458
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

		// Token: 0x0600566B RID: 22123 RVA: 0x002812A7 File Offset: 0x0027F4A7
		private IEnumerable<ColumnDefinition> GenerateVillagerColumns()
		{
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Char_Age.Tr(), (VillagerCharDisplayData data) => (data.PhysiologicalAge >= 0) ? data.PhysiologicalAge.ToString() : LanguageKey.LK_Unknow.Tr(), 8, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_VillagerInfo_Role.Tr(), (VillagerCharDisplayData data) => CommonUtils.GetOrganizationGradeString(data.OrgInfo, data.Gender, data.PhysiologicalAge, (int)data.CharacterTemplateId), 1, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_VillagerInfo_Working_Status.Tr(), delegate(VillagerCharDisplayData data)
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
					result = ((((data != null) ? new int?(data.ArrangementTemplateId) : null) != -1) ? VillagerRoleArrangement.Instance[data.ArrangementTemplateId].DescName : LanguageKey.LK_VillagerInfo_Idle.Tr());
					break;
				case 4:
					result = LanguageKey.LK_VillagerInfo_Build.Tr();
					break;
				default:
					result = ((((data != null) ? new int?(data.ArrangementTemplateId) : null) != -1) ? VillagerRoleArrangement.Instance[data.ArrangementTemplateId].DescName : LanguageKey.LK_VillagerInfo_None.Tr());
					break;
				}
				if (!true)
				{
				}
				return result;
			}, 129, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_VillagerInfo_Working_Location.Tr(), delegate(VillagerCharDisplayData data)
			{
				VillagerWorkData workData = data.VillagerWorkData;
				string result;
				if (workData == null || workData.BuildingBlockIndex == -1 || VillagerSortController.WorkingStatusOrder(data) != 2)
				{
					result = LanguageKey.LK_VillagerInfo_None.Tr();
				}
				else
				{
					ConfigData<BuildingBlockItem, short> instance = BuildingBlock.Instance;
					BuildingBlockData taiwuBuildingData = SingletonObject.getInstance<BuildingModel>().GetTaiwuBuildingData(new BuildingBlockKey(workData.AreaId, workData.BlockId, workData.BuildingBlockIndex));
					result = instance[(taiwuBuildingData != null) ? taiwuBuildingData.TemplateId : 0].Name;
				}
				return result;
			}, -1, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_VillagerInfo_Working_Role.Tr(), delegate(VillagerCharDisplayData data)
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
			}, 127, 30f, 224f);
			yield return this.CreateLocationColumn(() => LanguageKey.LK_VillagerInfo_Location.Tr(), -1, 254f, 254f);
			ColumnDefinition columnDefinition;
			if (this._dataList != null)
			{
				columnDefinition = ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Favorability.Tr(), new Func<VillagerCharDisplayData, string>(ViewTaiwuVillagers.GetFavorDisplayString), 11, 194f, 194f);
			}
			else
			{
				columnDefinition = ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_VillagerInfo_Potential.Tr(), (VillagerCharDisplayData data) => data.Potential.ToString(), 128, 194f, 194f);
			}
			yield return columnDefinition;
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_VillagerInfo_Happiness.Tr(), (VillagerCharDisplayData data) => CommonUtils.GetHappinessString(HappinessType.GetHappinessType(data.Happiness)), 12, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_VillagerInfo_DefeatMark.Tr(), (VillagerCharDisplayData data) => data.DefeatMarkCount.ToString(), 53, 222f, 222f);
			yield break;
		}

		// Token: 0x0600566C RID: 22124 RVA: 0x002812B7 File Offset: 0x0027F4B7
		private static IEnumerable<ColumnDefinition> GenerateStateColumns()
		{
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Char_Age.Tr(), (VillagerCharDisplayData data) => data.PhysiologicalAge.ToString(), 8, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Health.Tr(), (VillagerCharDisplayData data) => CommonUtils.GetCharacterHealthInfo(data.Health, data.MaxLeftHealth, data.CharacterId).Item1, 10, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Injury.Tr(), (VillagerCharDisplayData data) => data.DefeatMarkCount.ToString(), 53, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Charm.Tr(), (VillagerCharDisplayData data) => ViewTaiwuVillagers.GetCharmDisplayString(data), 9, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Behavior.Tr(), (VillagerCharDisplayData data) => CommonUtils.GetBehaviorString(data.BehaviorType), 57, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Happiness.Tr(), (VillagerCharDisplayData data) => CommonUtils.GetHappinessString(HappinessType.GetHappinessType(data.Happiness)), 12, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Favorability.Tr(), new Func<VillagerCharDisplayData, string>(ViewTaiwuVillagers.GetFavorDisplayString), 11, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Alertness.Tr(), (VillagerCharDisplayData data) => CommonUtils.GetAlertnessNameByValue(data.Alertness), 130, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Samsara.Tr(), (VillagerCharDisplayData data) => data.PreexistenceCharCount.ToString(), 58, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Fame.Tr(), (VillagerCharDisplayData data) => CommonUtils.GetFameString(FameType.GetFameType(data.Fame)), 59, 30f, 224f);
			yield break;
		}

		// Token: 0x0600566D RID: 22125 RVA: 0x002812C0 File Offset: 0x0027F4C0
		private static IEnumerable<ColumnDefinition> GeneratePropertyColumns()
		{
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Strength.Tr(), (VillagerCharDisplayData data) => data.MaxMainAttributes[0].ToString(), 60, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Dexterity.Tr(), (VillagerCharDisplayData data) => data.MaxMainAttributes[1].ToString(), 61, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Concentration.Tr(), (VillagerCharDisplayData data) => data.MaxMainAttributes[2].ToString(), 62, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Vitality.Tr(), (VillagerCharDisplayData data) => data.MaxMainAttributes[3].ToString(), 63, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Energy.Tr(), (VillagerCharDisplayData data) => data.MaxMainAttributes[4].ToString(), 64, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Intelligence.Tr(), (VillagerCharDisplayData data) => data.MaxMainAttributes[5].ToString(), 65, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Penetrate_Outer.Tr(), (VillagerCharDisplayData data) => data.Penetrations.Outer.ToString(), 22, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Penetrate_Inner.Tr(), (VillagerCharDisplayData data) => data.Penetrations.Inner.ToString(), 23, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Penetrate_Resist_Outer.Tr(), (VillagerCharDisplayData data) => data.PenetrationResists.Outer.ToString(), 29, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Penetrate_Resist_Inner.Tr(), (VillagerCharDisplayData data) => data.PenetrationResists.Inner.ToString(), 30, 30f, 224f);
			yield break;
		}

		// Token: 0x0600566E RID: 22126 RVA: 0x002812C9 File Offset: 0x0027F4C9
		private static IEnumerable<ColumnDefinition> GenerateProperty2Columns()
		{
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_HitType_0.Tr(), (VillagerCharDisplayData data) => data.HitValues[0].ToString(), 24, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_HitType_1.Tr(), (VillagerCharDisplayData data) => data.HitValues[1].ToString(), 25, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_HitType_2.Tr(), (VillagerCharDisplayData data) => data.HitValues[2].ToString(), 26, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_HitType_3.Tr(), (VillagerCharDisplayData data) => data.HitValues[3].ToString(), 27, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_AvoidType_0.Tr(), (VillagerCharDisplayData data) => data.AvoidValues[0].ToString(), 33, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_AvoidType_1.Tr(), (VillagerCharDisplayData data) => data.AvoidValues[1].ToString(), 34, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_AvoidType_2.Tr(), (VillagerCharDisplayData data) => data.AvoidValues[2].ToString(), 35, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_AvoidType_3.Tr(), (VillagerCharDisplayData data) => data.AvoidValues[3].ToString(), 36, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Qi_Disorder.Tr(), (VillagerCharDisplayData data) => ((int)(data.DisorderOfQi / 10)).ToString(), 55, 30f, 224f);
			yield break;
		}

		// Token: 0x0600566F RID: 22127 RVA: 0x002812D2 File Offset: 0x0027F4D2
		private static IEnumerable<ColumnDefinition> GenerateLifeSkillColumns()
		{
			int num;
			for (int i = 0; i < 16; i = num + 1)
			{
				ViewTaiwuVillagers.<>c__DisplayClass64_0 CS$<>8__locals1 = new ViewTaiwuVillagers.<>c__DisplayClass64_0();
				CS$<>8__locals1.index = i;
				yield return ViewTaiwuVillagers.CreateTextColumn(() => LocalStringManager.Get(string.Format("LK_LifeSkillType_{0}", CS$<>8__locals1.index)), (VillagerCharDisplayData data) => data.LifeSkillQualifications[CS$<>8__locals1.index].ToString(), (short)(66 + CS$<>8__locals1.index), 40f, 60f);
				CS$<>8__locals1 = null;
				num = i;
			}
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Growth.Tr(), (VillagerCharDisplayData data) => ViewTaiwuVillagers.GetSkillGrowthString(data.ActualAge, data.LifeSkillGrowthType), 118, 30f, 224f);
			yield break;
		}

		// Token: 0x06005670 RID: 22128 RVA: 0x002812DB File Offset: 0x0027F4DB
		private static IEnumerable<ColumnDefinition> GenerateCombatSkillColumns()
		{
			int num;
			for (int i = 0; i < 14; i = num + 1)
			{
				ViewTaiwuVillagers.<>c__DisplayClass65_0 CS$<>8__locals1 = new ViewTaiwuVillagers.<>c__DisplayClass65_0();
				CS$<>8__locals1.index = i;
				yield return ViewTaiwuVillagers.CreateTextColumn(() => LocalStringManager.Get(string.Format("LK_CombatSkillType_{0}", CS$<>8__locals1.index)), (VillagerCharDisplayData data) => data.CombatSkillQualifications[CS$<>8__locals1.index].ToString(), (short)(82 + CS$<>8__locals1.index), 40f, 60f);
				CS$<>8__locals1 = null;
				num = i;
			}
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Growth.Tr(), (VillagerCharDisplayData data) => ViewTaiwuVillagers.GetSkillGrowthString(data.ActualAge, data.CombatSkillGrowthType), 119, 30f, 224f);
			yield break;
		}

		// Token: 0x06005671 RID: 22129 RVA: 0x002812E4 File Offset: 0x0027F4E4
		private static IEnumerable<ColumnDefinition> GeneratePersonalityColumns()
		{
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Personality_Calm_Name.Tr(), (VillagerCharDisplayData data) => data.Personalities[0].ToString(), 96, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Personality_Clever_Name.Tr(), (VillagerCharDisplayData data) => data.Personalities[1].ToString(), 97, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Personality_Enthusiastic_Name.Tr(), (VillagerCharDisplayData data) => data.Personalities[2].ToString(), 98, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Personality_Brave_Name.Tr(), (VillagerCharDisplayData data) => data.Personalities[3].ToString(), 99, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Personality_Firm_Name.Tr(), (VillagerCharDisplayData data) => data.Personalities[4].ToString(), 100, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Personality_Lucky_Name.Tr(), (VillagerCharDisplayData data) => data.Personalities[5].ToString(), 101, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Personality_Perceptive_Name.Tr(), (VillagerCharDisplayData data) => data.Personalities[6].ToString(), 102, 30f, 224f);
			yield break;
		}

		// Token: 0x06005672 RID: 22130 RVA: 0x002812ED File Offset: 0x0027F4ED
		private static IEnumerable<ColumnDefinition> GenerateItemColumns()
		{
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Food.Tr(), (VillagerCharDisplayData data) => data.Resources[0].ToString(), 103, 40f, 60f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Wood.Tr(), (VillagerCharDisplayData data) => data.Resources[1].ToString(), 104, 40f, 60f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Metal.Tr(), (VillagerCharDisplayData data) => data.Resources[2].ToString(), 105, 40f, 60f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Jade.Tr(), (VillagerCharDisplayData data) => data.Resources[3].ToString(), 106, 40f, 60f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Fabric.Tr(), (VillagerCharDisplayData data) => data.Resources[4].ToString(), 107, 40f, 60f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Herb.Tr(), (VillagerCharDisplayData data) => data.Resources[5].ToString(), 108, 40f, 60f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Money.Tr(), (VillagerCharDisplayData data) => data.Resources[6].ToString(), 109, 40f, 60f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Authority.Tr(), (VillagerCharDisplayData data) => data.Resources[7].ToString(), 110, 40f, 60f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Inventory.Tr(), (VillagerCharDisplayData data) => ViewTaiwuVillagers.GetInventoryLoadString(data.CurrInventoryLoad, data.MaxInventoryLoad), 37, 30f, 224f);
			yield return ViewTaiwuVillagers.CreateTextColumn(() => LanguageKey.LK_Kidnap.Tr(), (VillagerCharDisplayData data) => data.KidnapCount.ToString(), 111, 30f, 224f);
			yield break;
		}

		// Token: 0x06005673 RID: 22131 RVA: 0x002812F6 File Offset: 0x0027F4F6
		private static IEnumerable<ColumnDefinition> GenerateCommandColumns()
		{
			yield return ViewTaiwuVillagers.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Attack.Tr(), (VillagerCharDisplayData data) => ViewTaiwuVillagers.CreateMedalCellData(data.AttackMedal, ViewTaiwuVillagers.EMedalType.Attack), 112, 80f, 120f);
			yield return ViewTaiwuVillagers.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Defence.Tr(), (VillagerCharDisplayData data) => ViewTaiwuVillagers.CreateMedalCellData(data.DefenceMedal, ViewTaiwuVillagers.EMedalType.Defence), 113, 80f, 120f);
			yield return ViewTaiwuVillagers.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Wisdom.Tr(), (VillagerCharDisplayData data) => ViewTaiwuVillagers.CreateMedalCellData(data.WisdomMedal, ViewTaiwuVillagers.EMedalType.Wisdom), 114, 80f, 120f);
			yield return ViewTaiwuVillagers.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_0.Tr(), (VillagerCharDisplayData data) => ViewTaiwuVillagers.CreateCommandCellData(data, 0), 115, 80f, 120f);
			yield return ViewTaiwuVillagers.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_1.Tr(), (VillagerCharDisplayData data) => ViewTaiwuVillagers.CreateCommandCellData(data, 1), 116, 80f, 120f);
			yield return ViewTaiwuVillagers.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_2.Tr(), (VillagerCharDisplayData data) => ViewTaiwuVillagers.CreateCommandCellData(data, 2), 117, 80f, 120f);
			yield break;
		}

		// Token: 0x06005674 RID: 22132 RVA: 0x00281300 File Offset: 0x0027F500
		private static IconAndTextCellData CreateMedalCellData(int medalCount, ViewTaiwuVillagers.EMedalType medalType)
		{
			bool flag = medalCount == 0;
			IconAndTextCellData result;
			if (flag)
			{
				result = IconAndTextCellData.TextOnly("-");
			}
			else
			{
				string iconName = ViewTaiwuVillagers.GetMedalIconName(medalCount, medalType);
				string text = string.Format(" x{0}", Mathf.Abs(medalCount));
				result = new IconAndTextCellData(iconName, text, true, false, false, false);
			}
			return result;
		}

		// Token: 0x06005675 RID: 22133 RVA: 0x00281354 File Offset: 0x0027F554
		private static string GetMedalIconName(int medalCount, ViewTaiwuVillagers.EMedalType medalType)
		{
			int signKey = (medalCount > 0) ? 1 : ((medalCount < 0) ? -1 : 0);
			if (!true)
			{
			}
			string text;
			switch (medalType)
			{
			case ViewTaiwuVillagers.EMedalType.Attack:
				text = MedalSummary.AttackMedalIconConfig[signKey];
				break;
			case ViewTaiwuVillagers.EMedalType.Defence:
				text = MedalSummary.DefenceMedalIconConfig[signKey];
				break;
			case ViewTaiwuVillagers.EMedalType.Wisdom:
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

		// Token: 0x06005676 RID: 22134 RVA: 0x002813D4 File Offset: 0x0027F5D4
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

		// Token: 0x06005677 RID: 22135 RVA: 0x00281460 File Offset: 0x0027F660
		private bool IsTaiwu(int charId)
		{
			return charId == this._taiwuCharId;
		}

		// Token: 0x06005678 RID: 22136 RVA: 0x0028147C File Offset: 0x0027F67C
		private bool IsSpecialTeammate(int charId)
		{
			VillagerCharDisplayData data = this._dataList.Find((VillagerCharDisplayData d) => d.CharacterId == charId);
			return data != null && data.IsSpecialGroupMember;
		}

		// Token: 0x06005679 RID: 22137 RVA: 0x002814C0 File Offset: 0x0027F6C0
		private static string GetCharmDisplayString(VillagerCharDisplayData data)
		{
			return CommonUtils.GetCharmLevelText(data.Charm, data.Gender, data.PhysiologicalAge, data.ClothDisplayId, CreatingType.IsFixedPresetType(data.CreatingType), data.FaceVisible);
		}

		// Token: 0x0600567A RID: 22138 RVA: 0x00281500 File Offset: 0x0027F700
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

		// Token: 0x0600567B RID: 22139 RVA: 0x00281564 File Offset: 0x0027F764
		private static string GetFavorDisplayString(VillagerCharDisplayData data)
		{
			return CommonUtils.GetFavorStringByInteracted(data.FavorabilityToTaiwu, data.IsInteractedWithTaiwu);
		}

		// Token: 0x0600567C RID: 22140 RVA: 0x00281588 File Offset: 0x0027F788
		private static string GetSkillGrowthString(short actualAge, sbyte growthType)
		{
			sbyte addValue = ViewTaiwuVillagers.GetSkillGrowthAddValue(actualAge, (int)growthType);
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

		// Token: 0x0600567D RID: 22141 RVA: 0x00281648 File Offset: 0x0027F848
		private static sbyte GetSkillGrowthAddValue(short actualAge, int growthType)
		{
			AgeEffectItem ageData = AgeEffect.Instance[Math.Min((int)actualAge, AgeEffect.Instance.Count - 1)];
			return (growthType == 0) ? ageData.SkillQualificationAverage : ((growthType == 1) ? ageData.SkillQualificationPrecocious : ageData.SkillQualificationLateBlooming);
		}

		// Token: 0x0600567E RID: 22142 RVA: 0x00281694 File Offset: 0x0027F894
		private static string GetInventoryLoadString(int currLoad, int maxLoad)
		{
			string currLoadStr = ((float)currLoad / 100f).ToString("f1").SetColor(CommonUtils.GetLoadWeightValueColor(currLoad, maxLoad));
			return string.Format("{0}/{1:f1}", currLoadStr, (float)maxLoad / 100f);
		}

		// Token: 0x0600567F RID: 22143 RVA: 0x002816E0 File Offset: 0x0027F8E0
		private void OnSubPageChanged(VillagerSubPage subPage)
		{
			bool flag = this._currentSubPage == subPage;
			if (!flag)
			{
				this._currentSubPage = subPage;
				TabSortStateManager<VillagerSubPage, ISelectCharacterData> tabSortStateManager = this._tabSortStateManager;
				if (tabSortStateManager != null)
				{
					tabSortStateManager.OnTabChange(subPage);
				}
				this.RefreshList();
			}
		}

		// Token: 0x06005680 RID: 22144 RVA: 0x0028171E File Offset: 0x0027F91E
		private void OnSortOrFilterChanged()
		{
			this.RefreshListData();
		}

		// Token: 0x17000A72 RID: 2674
		// (get) Token: 0x06005681 RID: 22145 RVA: 0x00281728 File Offset: 0x0027F928
		public static string SearchingText
		{
			get
			{
				ViewTaiwuVillagers viewTaiwuVillagers = UIElement.TaiwuVillagers.UiBaseAs<ViewTaiwuVillagers>();
				return (viewTaiwuVillagers != null) ? viewTaiwuVillagers.searchingField.text : null;
			}
		}

		// Token: 0x06005688 RID: 22152 RVA: 0x0028198B File Offset: 0x0027FB8B
		[CompilerGenerated]
		private void <RequestDataIfUiChanged>g__Handle|21_0(int offset, RawDataPool pool)
		{
			Serializer.Deserialize(pool, offset, ref this._dataList);
			this.RefreshListData();
			this._shouldRefreshData = false;
		}

		// Token: 0x04003B01 RID: 15105
		private static readonly List<LanguageKey> ToggleGroupNameKeys = new List<LanguageKey>
		{
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

		// Token: 0x04003B02 RID: 15106
		private VillagerSubPage _currentSubPage = VillagerSubPage.Villager;

		// Token: 0x04003B03 RID: 15107
		private List<VillagerCharDisplayData> _dataList = new List<VillagerCharDisplayData>();

		// Token: 0x04003B04 RID: 15108
		private List<VillagerCharDisplayData> _filteredDataList = new List<VillagerCharDisplayData>();

		// Token: 0x04003B05 RID: 15109
		private SelectCharacterSortAndFilterController _sortAndFilterController;

		// Token: 0x04003B06 RID: 15110
		private TabSortStateManager<VillagerSubPage, ISelectCharacterData> _tabSortStateManager;

		// Token: 0x04003B07 RID: 15111
		private int _taiwuCharId = -1;

		// Token: 0x04003B08 RID: 15112
		private readonly Dictionary<VillagerSubPage, RowItem> _rowTemplateCache = new Dictionary<VillagerSubPage, RowItem>();

		// Token: 0x04003B09 RID: 15113
		private readonly HashSet<int> _selectedCharId = new HashSet<int>();

		// Token: 0x04003B0A RID: 15114
		private bool _shouldRefreshData = false;

		// Token: 0x04003B0B RID: 15115
		[SerializeField]
		private TMP_Text title;

		// Token: 0x04003B0C RID: 15116
		private List<int> _ids;

		// Token: 0x04003B0D RID: 15117
		[SerializeField]
		private GameObject noContent;

		// Token: 0x04003B0E RID: 15118
		[SerializeField]
		private ListStyleGeneralScroll listScroll;

		// Token: 0x04003B0F RID: 15119
		[SerializeField]
		private SortAndFilter sortAndFilter;

		// Token: 0x04003B10 RID: 15120
		[SerializeField]
		private CToggleGroup subPageToggleGroup;

		// Token: 0x04003B11 RID: 15121
		[SerializeField]
		private TMP_InputField searchingField;

		// Token: 0x04003B12 RID: 15122
		[SerializeField]
		private CToggle multiSelect;

		// Token: 0x04003B13 RID: 15123
		[SerializeField]
		private CToggle selectAll;

		// Token: 0x04003B14 RID: 15124
		[SerializeField]
		private CButton confirm;

		// Token: 0x04003B15 RID: 15125
		[SerializeField]
		private TMP_Text expelCounter;

		// Token: 0x04003B16 RID: 15126
		[SerializeField]
		private TMP_Text expelTips;

		// Token: 0x04003B17 RID: 15127
		[SerializeField]
		private RectTransform villagerExtraInfo;

		// Token: 0x04003B18 RID: 15128
		[SerializeField]
		private RectTransform challengeRect;

		// Token: 0x04003B19 RID: 15129
		[SerializeField]
		private TMP_Text villagerCount;

		// Token: 0x04003B1A RID: 15130
		[SerializeField]
		private TMP_Text[] resources;

		// Token: 0x04003B1B RID: 15131
		[SerializeField]
		private GameObject bottomGroup;

		// Token: 0x04003B1C RID: 15132
		[Header("行模板配置")]
		[SerializeField]
		private RowItem rowTemplate;

		// Token: 0x04003B1D RID: 15133
		[SerializeField]
		private RowCellContainer singleTextCellContainer;

		// Token: 0x04003B1E RID: 15134
		[SerializeField]
		private RowCellContainer buttonCellContainer;

		// Token: 0x04003B1F RID: 15135
		[SerializeField]
		private RowCellContainer avatarAndNameCellContainer;

		// Token: 0x04003B20 RID: 15136
		[SerializeField]
		private RowCellContainer iconAndTextCellContainer;

		// Token: 0x04003B21 RID: 15137
		[Header("tips")]
		[SerializeField]
		private TooltipInvoker cannotMultiSelectReasonDisplayer;

		// Token: 0x02001B87 RID: 7047
		private enum EVillagerColumn
		{
			// Token: 0x0400BBF2 RID: 48114
			Age,
			// Token: 0x0400BBF3 RID: 48115
			GradeName,
			// Token: 0x0400BBF4 RID: 48116
			WorkStatus,
			// Token: 0x0400BBF5 RID: 48117
			WorkLocation,
			// Token: 0x0400BBF6 RID: 48118
			WorkRole,
			// Token: 0x0400BBF7 RID: 48119
			Location,
			// Token: 0x0400BBF8 RID: 48120
			Potential,
			// Token: 0x0400BBF9 RID: 48121
			Happiness,
			// Token: 0x0400BBFA RID: 48122
			DefeatMark,
			// Token: 0x0400BBFB RID: 48123
			Count
		}

		// Token: 0x02001B88 RID: 7048
		private enum EStateColumn
		{
			// Token: 0x0400BBFD RID: 48125
			Age,
			// Token: 0x0400BBFE RID: 48126
			Health,
			// Token: 0x0400BBFF RID: 48127
			DefeatMark,
			// Token: 0x0400BC00 RID: 48128
			Charm,
			// Token: 0x0400BC01 RID: 48129
			Behavior,
			// Token: 0x0400BC02 RID: 48130
			Happiness,
			// Token: 0x0400BC03 RID: 48131
			Favorability,
			// Token: 0x0400BC04 RID: 48132
			Alertness,
			// Token: 0x0400BC05 RID: 48133
			Samsara,
			// Token: 0x0400BC06 RID: 48134
			Fame,
			// Token: 0x0400BC07 RID: 48135
			Count
		}

		// Token: 0x02001B89 RID: 7049
		private enum EPropertyColumn
		{
			// Token: 0x0400BC09 RID: 48137
			AttrStrength,
			// Token: 0x0400BC0A RID: 48138
			AttrDexterity,
			// Token: 0x0400BC0B RID: 48139
			AttrConcentration,
			// Token: 0x0400BC0C RID: 48140
			AttrVitality,
			// Token: 0x0400BC0D RID: 48141
			AttrEnergy,
			// Token: 0x0400BC0E RID: 48142
			AttrIntelligence,
			// Token: 0x0400BC0F RID: 48143
			PenetrateOuter,
			// Token: 0x0400BC10 RID: 48144
			PenetrateInner,
			// Token: 0x0400BC11 RID: 48145
			ResistOuter,
			// Token: 0x0400BC12 RID: 48146
			ResistInner,
			// Token: 0x0400BC13 RID: 48147
			Count
		}

		// Token: 0x02001B8A RID: 7050
		private enum EProperty2Column
		{
			// Token: 0x0400BC15 RID: 48149
			HitStrength,
			// Token: 0x0400BC16 RID: 48150
			HitTechnique,
			// Token: 0x0400BC17 RID: 48151
			HitSpeed,
			// Token: 0x0400BC18 RID: 48152
			HitMind,
			// Token: 0x0400BC19 RID: 48153
			AvoidStrength,
			// Token: 0x0400BC1A RID: 48154
			AvoidTechnique,
			// Token: 0x0400BC1B RID: 48155
			AvoidSpeed,
			// Token: 0x0400BC1C RID: 48156
			AvoidMind,
			// Token: 0x0400BC1D RID: 48157
			QiDisorder,
			// Token: 0x0400BC1E RID: 48158
			Count
		}

		// Token: 0x02001B8B RID: 7051
		private enum ELifeSkillColumn
		{
			// Token: 0x0400BC20 RID: 48160
			Skill0,
			// Token: 0x0400BC21 RID: 48161
			Skill1,
			// Token: 0x0400BC22 RID: 48162
			Skill2,
			// Token: 0x0400BC23 RID: 48163
			Skill3,
			// Token: 0x0400BC24 RID: 48164
			Skill4,
			// Token: 0x0400BC25 RID: 48165
			Skill5,
			// Token: 0x0400BC26 RID: 48166
			Skill6,
			// Token: 0x0400BC27 RID: 48167
			Skill7,
			// Token: 0x0400BC28 RID: 48168
			Skill8,
			// Token: 0x0400BC29 RID: 48169
			Skill9,
			// Token: 0x0400BC2A RID: 48170
			Skill10,
			// Token: 0x0400BC2B RID: 48171
			Skill11,
			// Token: 0x0400BC2C RID: 48172
			Skill12,
			// Token: 0x0400BC2D RID: 48173
			Skill13,
			// Token: 0x0400BC2E RID: 48174
			Skill14,
			// Token: 0x0400BC2F RID: 48175
			Skill15,
			// Token: 0x0400BC30 RID: 48176
			Growth,
			// Token: 0x0400BC31 RID: 48177
			Count
		}

		// Token: 0x02001B8C RID: 7052
		private enum ECombatSkillColumn
		{
			// Token: 0x0400BC33 RID: 48179
			Skill0,
			// Token: 0x0400BC34 RID: 48180
			Skill1,
			// Token: 0x0400BC35 RID: 48181
			Skill2,
			// Token: 0x0400BC36 RID: 48182
			Skill3,
			// Token: 0x0400BC37 RID: 48183
			Skill4,
			// Token: 0x0400BC38 RID: 48184
			Skill5,
			// Token: 0x0400BC39 RID: 48185
			Skill6,
			// Token: 0x0400BC3A RID: 48186
			Skill7,
			// Token: 0x0400BC3B RID: 48187
			Skill8,
			// Token: 0x0400BC3C RID: 48188
			Skill9,
			// Token: 0x0400BC3D RID: 48189
			Skill10,
			// Token: 0x0400BC3E RID: 48190
			Skill11,
			// Token: 0x0400BC3F RID: 48191
			Skill12,
			// Token: 0x0400BC40 RID: 48192
			Skill13,
			// Token: 0x0400BC41 RID: 48193
			Growth,
			// Token: 0x0400BC42 RID: 48194
			Count
		}

		// Token: 0x02001B8D RID: 7053
		private enum EPersonalityColumn
		{
			// Token: 0x0400BC44 RID: 48196
			P0,
			// Token: 0x0400BC45 RID: 48197
			P1,
			// Token: 0x0400BC46 RID: 48198
			P2,
			// Token: 0x0400BC47 RID: 48199
			P3,
			// Token: 0x0400BC48 RID: 48200
			P4,
			// Token: 0x0400BC49 RID: 48201
			P5,
			// Token: 0x0400BC4A RID: 48202
			P6,
			// Token: 0x0400BC4B RID: 48203
			Count
		}

		// Token: 0x02001B8E RID: 7054
		private enum EItemColumn
		{
			// Token: 0x0400BC4D RID: 48205
			Food,
			// Token: 0x0400BC4E RID: 48206
			Wood,
			// Token: 0x0400BC4F RID: 48207
			Metal,
			// Token: 0x0400BC50 RID: 48208
			Jade,
			// Token: 0x0400BC51 RID: 48209
			Fabric,
			// Token: 0x0400BC52 RID: 48210
			Herb,
			// Token: 0x0400BC53 RID: 48211
			Money,
			// Token: 0x0400BC54 RID: 48212
			Authority,
			// Token: 0x0400BC55 RID: 48213
			InventoryLoad,
			// Token: 0x0400BC56 RID: 48214
			KidnapCount,
			// Token: 0x0400BC57 RID: 48215
			Count
		}

		// Token: 0x02001B8F RID: 7055
		private enum ECommandColumn
		{
			// Token: 0x0400BC59 RID: 48217
			AttackMedal,
			// Token: 0x0400BC5A RID: 48218
			DefenceMedal,
			// Token: 0x0400BC5B RID: 48219
			WisdomMedal,
			// Token: 0x0400BC5C RID: 48220
			Command0,
			// Token: 0x0400BC5D RID: 48221
			Command1,
			// Token: 0x0400BC5E RID: 48222
			Command2,
			// Token: 0x0400BC5F RID: 48223
			Count
		}

		// Token: 0x02001B90 RID: 7056
		private enum EMedalType
		{
			// Token: 0x0400BC61 RID: 48225
			Attack,
			// Token: 0x0400BC62 RID: 48226
			Defence,
			// Token: 0x0400BC63 RID: 48227
			Wisdom
		}
	}
}
