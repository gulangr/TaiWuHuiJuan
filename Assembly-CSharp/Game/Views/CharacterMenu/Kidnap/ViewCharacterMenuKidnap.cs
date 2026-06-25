using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Components.Common;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.CellContent;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Kidnap;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.CharacterMenu.Kidnap
{
	// Token: 0x02000BBA RID: 3002
	public class ViewCharacterMenuKidnap : UI_CharacterMenuSubPageBase
	{
		// Token: 0x17001031 RID: 4145
		// (get) Token: 0x06009700 RID: 38656 RVA: 0x00466A08 File Offset: 0x00464C08
		public override LanguageKey TitleKey
		{
			get
			{
				return LanguageKey.LK_CharacterMenu_Title_Kidnap;
			}
		}

		// Token: 0x06009701 RID: 38657 RVA: 0x00466A10 File Offset: 0x00464C10
		public override bool CheckState(ECharacterSubToggleBase curSubTogglePage, ECharacterSubPage curSubPage)
		{
			return curSubPage == ECharacterSubPage.Prison;
		}

		// Token: 0x17001032 RID: 4146
		// (get) Token: 0x06009702 RID: 38658 RVA: 0x00466A26 File Offset: 0x00464C26
		protected static int TaiwuId
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x06009703 RID: 38659 RVA: 0x00466A32 File Offset: 0x00464C32
		private void Awake()
		{
			this.InitSubPageToggles();
			this.InitSortAndFilter();
			this.InitCharacterDropdown();
			this.InitModeDropdown();
			this.InitGridScroll();
		}

		// Token: 0x06009704 RID: 38660 RVA: 0x00466A58 File Offset: 0x00464C58
		public override void OnInit(ArgumentBox argsBox)
		{
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
		}

		// Token: 0x06009705 RID: 38661 RVA: 0x00466A89 File Offset: 0x00464C89
		private void OnListenerIdReady()
		{
			this.localLoadingAnim.SetLoadingState(true);
			this.RequestData();
		}

		// Token: 0x06009706 RID: 38662 RVA: 0x00466AA0 File Offset: 0x00464CA0
		public override void OnSubpageVisible()
		{
			base.OnSubpageVisible();
			this.RequestData();
			this.UpdateModeDisplay();
		}

		// Token: 0x06009707 RID: 38663 RVA: 0x00466AB8 File Offset: 0x00464CB8
		public override void OnSubpageInVisible()
		{
			base.OnSubpageInVisible();
			this._dataList.Clear();
			this._filteredDataList.Clear();
			TabSortStateManager<ViewCharacterMenuKidnap.KidnapSubPage, KidnapCharDisplayData> tabSortStateManager = this._tabSortStateManager;
			if (tabSortStateManager != null)
			{
				tabSortStateManager.ClearAll();
			}
			base.CharacterMenu.MoveCharacterScrollBack(0f, Ease.OutExpo);
		}

		// Token: 0x06009708 RID: 38664 RVA: 0x00466B0C File Offset: 0x00464D0C
		public override void OnCurrentCharacterChange(int prevCharacterId)
		{
			bool flag = base.CharacterMenu.CurCharacterId < 0;
			if (!flag)
			{
				this.localLoadingAnim.SetLoadingState(true);
				bool ready = this.Element.Ready;
				if (ready)
				{
					this.RequestData();
				}
			}
		}

		// Token: 0x06009709 RID: 38665 RVA: 0x00466B54 File Offset: 0x00464D54
		protected override void OnNotifyGameDataFiltered(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 1)
				{
					bool flag = notification.DomainId == 4 && notification.MethodId == 203;
					if (flag)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._kidnapMenuData);
						KidnapMenuDisplayData kidnapMenuData = this._kidnapMenuData;
						this._dataList = (((kidnapMenuData != null) ? kidnapMenuData.KidnapCharDisplayDataList : null) ?? new List<KidnapCharDisplayData>());
						this.RefreshAll();
						this.UpdateModeDisplay();
						this.localLoadingAnim.SetLoadingState(false);
						this.Element.ShowAfterRefresh();
					}
					else
					{
						bool flag2 = notification.DomainId == 4 && notification.MethodId == 48;
						if (flag2)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._characterDisplayDataList);
							this.RefreshCharacterDropdown();
						}
					}
				}
			}
		}

		// Token: 0x0600970A RID: 38666 RVA: 0x00466C88 File Offset: 0x00464E88
		private void InitSubPageToggles()
		{
			this.subPageToggleGroup.Init(-1);
			ToggleGroupHotkeyController.Set(this.Element, this.subPageToggleGroup, 1, null);
			this.subPageToggleGroup.OnActiveIndexChange += delegate(int newIndex, int _)
			{
				this.OnSubPageChanged((ViewCharacterMenuKidnap.KidnapSubPage)newIndex);
			};
			List<CToggle> toggles = this.subPageToggleGroup.GetAll();
			for (int i = 0; i < toggles.Count; i++)
			{
				CToggle toggle = toggles[i];
				TextMeshProUGUI label = toggle.transform.Find("Label").GetComponent<TextMeshProUGUI>();
				label.text = ViewCharacterMenuKidnap.ToggleGroupNameKeys[i].Tr();
			}
		}

		// Token: 0x0600970B RID: 38667 RVA: 0x00466D2C File Offset: 0x00464F2C
		private void InitSortAndFilter()
		{
			bool flag = this.sortAndFilter == null;
			if (!flag)
			{
				this._sortAndFilterController = new KidnapSortAndFilterController(this.sortAndFilter, new Func<int, bool>(ViewCharacterMenuKidnap.IsTaiwu));
				this._sortAndFilterController.Init(new Action(this.OnSortOrFilterChanged), "KidnapSort");
				bool flag2 = this.listScroll != null;
				if (flag2)
				{
					this.listScroll.SetSortController(this._sortAndFilterController);
				}
				this._tabSortStateManager = new TabSortStateManager<ViewCharacterMenuKidnap.KidnapSubPage, KidnapCharDisplayData>(this._sortAndFilterController);
			}
		}

		// Token: 0x0600970C RID: 38668 RVA: 0x00466DBC File Offset: 0x00464FBC
		private void InitCharacterDropdown()
		{
			bool flag = this.switchCharacterDropdown == null;
			if (!flag)
			{
				this.switchCharacterDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnCharacterDropdownValueChanged));
			}
		}

		// Token: 0x0600970D RID: 38669 RVA: 0x00466DFC File Offset: 0x00464FFC
		private void InitModeDropdown()
		{
			bool flag = this.switchModeDropdown == null;
			if (!flag)
			{
				this.switchModeDropdown.ClearOptions();
				this.switchModeDropdown.AddOptions(new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_CharacterMenuKidnap_ViewMod_0),
					LocalStringManager.Get(LanguageKey.LK_CharacterMenuKidnap_ViewMod_1)
				});
				this.switchModeDropdown.SetValueWithoutNotify(0);
				this.switchModeDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnModeDropdownValueChanged));
				this._isGridMode = true;
				this.UpdateModeDisplay();
			}
		}

		// Token: 0x0600970E RID: 38670 RVA: 0x00466E94 File Offset: 0x00465094
		private void InitGridScroll()
		{
			bool flag = this.gridScroll == null;
			if (!flag)
			{
				this.gridScroll.OnItemRender += this.OnRenderGridItem;
			}
		}

		// Token: 0x0600970F RID: 38671 RVA: 0x00466ECC File Offset: 0x004650CC
		private void RequestData()
		{
			List<int> charIds = new List<int>();
			IReadOnlyList<CharacterDisplayData> characters = base.CharacterMenu.DisplayCharacters;
			bool flag = characters != null;
			if (flag)
			{
				for (int i = 0; i < characters.Count; i++)
				{
					CharacterDisplayData item = characters[i];
					bool flag2 = item != null;
					if (flag2)
					{
						charIds.Add(item.CharacterId);
					}
				}
			}
			CharacterDomainMethod.Call.GetCharacterDisplayDataList(this.Element.GameDataListenerId, charIds);
			CharacterDomainMethod.Call.GetKidnapMenuDisplayData(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
		}

		// Token: 0x06009710 RID: 38672 RVA: 0x00466F60 File Offset: 0x00465160
		private void RefreshAll()
		{
			KidnapSortAndFilterController sortAndFilterController = this._sortAndFilterController;
			Func<KidnapCharDisplayData, bool> func;
			if ((func = ((sortAndFilterController != null) ? sortAndFilterController.GenerateFilter() : null)) == null && (func = ViewCharacterMenuKidnap.<>c.<>9__50_0) == null)
			{
				func = (ViewCharacterMenuKidnap.<>c.<>9__50_0 = ((KidnapCharDisplayData _) => true));
			}
			Func<KidnapCharDisplayData, bool> filter = func;
			KidnapSortAndFilterController sortAndFilterController2 = this._sortAndFilterController;
			Comparison<KidnapCharDisplayData> comparer = (sortAndFilterController2 != null) ? sortAndFilterController2.GenerateComparer(this._dataList) : null;
			bool flag;
			if (comparer != null)
			{
				TabSortStateManager<ViewCharacterMenuKidnap.KidnapSubPage, KidnapCharDisplayData> tabSortStateManager = this._tabSortStateManager;
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
			this._filteredDataList = this._dataList.Where(filter).ToList<KidnapCharDisplayData>();
			bool isGridMode = this._isGridMode;
			if (isGridMode)
			{
				this.RefreshGridMode();
			}
			else
			{
				this.RefreshListMode();
			}
			bool flag3 = this._kidnapMenuData != null;
			if (flag3)
			{
				this.countLabel.text = LanguageKey.LK_CharacterMenuKidnap_Count.TrFormat(this._kidnapMenuData.CurrentKidnapCount, this._kidnapMenuData.MaxKidnapSlotCount);
			}
		}

		// Token: 0x06009711 RID: 38673 RVA: 0x00467064 File Offset: 0x00465264
		private void RefreshListMode()
		{
			this.RefreshListStructure();
			this.RefreshListData();
		}

		// Token: 0x06009712 RID: 38674 RVA: 0x00467078 File Offset: 0x00465278
		private void RefreshListStructure()
		{
			IEnumerable<ColumnDefinition> columnDefinitions = this.GenerateColumnDefinitions(this._currentSubPage);
			this.PrepareRowTemplateContainers(this._currentSubPage);
			this.listScroll.ClearInfinityScrollCache();
			this.listScroll.Init<KidnapCharDisplayData>(columnDefinitions, true, null, null);
		}

		// Token: 0x06009713 RID: 38675 RVA: 0x004670BC File Offset: 0x004652BC
		private void RefreshListData()
		{
			this.listScroll.SetData<KidnapCharDisplayData>(this._filteredDataList, -1);
		}

		// Token: 0x06009714 RID: 38676 RVA: 0x004670D4 File Offset: 0x004652D4
		private void RefreshGridMode()
		{
			bool flag = this.gridScroll == null;
			if (!flag)
			{
				bool flag2 = this.gridScrollEmpty != null;
				if (flag2)
				{
					this.gridScrollEmpty.SetActive(this._filteredDataList.Count == 0);
				}
				this.gridScroll.UpdateData(this._filteredDataList.Count);
			}
		}

		// Token: 0x06009715 RID: 38677 RVA: 0x00467138 File Offset: 0x00465338
		private void OnRenderGridItem(int index, GameObject obj)
		{
			bool flag = index < 0 || index >= this._filteredDataList.Count;
			if (!flag)
			{
				CharacterMenuKidnapGridItem kidnapItem = obj.GetComponent<CharacterMenuKidnapGridItem>();
				bool flag2 = kidnapItem == null;
				if (!flag2)
				{
					bool canOperateNow = base.CharacterMenu.CanOperate && !base.CharacterMenu.OpenFromCombatPrepare;
					bool canSelectRope = canOperateNow && base.CharacterMenu.CurrentCharacterIsTaiwu;
					kidnapItem.Set(base.CharacterMenu.CurCharacterId, base.CharacterMenu.CurCharacterId, this._filteredDataList[index], canOperateNow, canSelectRope, new Action(this.OnRopeChanged), new Action<int>(this.OnViewPrisoner), delegate
					{
						this.RequestData();
					});
				}
			}
		}

		// Token: 0x06009716 RID: 38678 RVA: 0x00467200 File Offset: 0x00465400
		private void OnRopeChanged()
		{
			this.RequestData();
		}

		// Token: 0x06009717 RID: 38679 RVA: 0x0046720C File Offset: 0x0046540C
		private void OnViewPrisoner(int charId)
		{
			bool activeSelf = base.CharacterMenu.StackView.gameObject.activeSelf;
			if (!activeSelf)
			{
				base.CharacterMenu.StackToNewCharacter(charId);
				base.CharacterMenu.SwitchToSubToggle(ECharacterSubToggleBase.CharacterBase);
			}
		}

		// Token: 0x06009718 RID: 38680 RVA: 0x00467250 File Offset: 0x00465450
		private void OnInteractClicked(int charId)
		{
			bool flag = !this.CanInteractWithKidnappedCharacter();
			if (!flag)
			{
				TaiwuEventDomainMethod.Call.OnInteractKidnappedCharacter(charId);
			}
		}

		// Token: 0x06009719 RID: 38681 RVA: 0x00467274 File Offset: 0x00465474
		private bool CanInteractWithKidnappedCharacter()
		{
			bool isOnNormalInteractEvent = SingletonObject.getInstance<EventModel>().IsOnNormalInteractEvent;
			CharacterMenuFunctionControlItem config;
			return base.CharacterMenu.CanOperate && !base.CharacterMenu.OpenFromCombatPrepare && !isOnNormalInteractEvent && base.CharacterMenu.IsTaiwuTeam && base.CharacterMenu.CurrentCharacterIsTaiwu && (!base.CharacterMenu.TryGetFunctionControlConfig(out config) || !base.CharacterMenu.IsFunctionBanned(config.Kidnapped));
		}

		// Token: 0x0600971A RID: 38682 RVA: 0x004672F4 File Offset: 0x004654F4
		private void UpdateModeDisplay()
		{
			this.listModeRoot.SetActive(!this._isGridMode);
			this.gridModeRoot.SetActive(this._isGridMode);
			this.subPageToggleGroup.gameObject.SetActive(!this._isGridMode);
			bool isGridMode = this._isGridMode;
			if (isGridMode)
			{
				base.CharacterMenu.MoveCharacterScrollBack(0f, Ease.OutExpo);
			}
			else
			{
				base.CharacterMenu.MoveCharacterScrollLeft(0f, Ease.OutExpo);
			}
			this.countLabelBg.SetWidth(this._isGridMode ? 2074f : 2520f);
		}

		// Token: 0x0600971B RID: 38683 RVA: 0x00467398 File Offset: 0x00465598
		private void OnModeDropdownValueChanged(int index)
		{
			bool newIsGridMode = index == 0;
			bool flag = this._isGridMode == newIsGridMode;
			if (!flag)
			{
				this._isGridMode = newIsGridMode;
				this.UpdateModeDisplay();
				this.RefreshAll();
			}
		}

		// Token: 0x0600971C RID: 38684 RVA: 0x004673CF File Offset: 0x004655CF
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitions(ViewCharacterMenuKidnap.KidnapSubPage subPage)
		{
			yield return this.CreateAvatarWithNameColumn();
			switch (subPage)
			{
			case ViewCharacterMenuKidnap.KidnapSubPage.Kidnap:
			{
				foreach (ColumnDefinition col in this.GenerateKidnapColumns())
				{
					yield return col;
					col = null;
				}
				IEnumerator<ColumnDefinition> enumerator = null;
				break;
			}
			case ViewCharacterMenuKidnap.KidnapSubPage.State:
			{
				foreach (ColumnDefinition col2 in this.GenerateStateColumns())
				{
					yield return col2;
					col2 = null;
				}
				IEnumerator<ColumnDefinition> enumerator2 = null;
				break;
			}
			case ViewCharacterMenuKidnap.KidnapSubPage.Property:
			{
				foreach (ColumnDefinition col3 in this.GeneratePropertyColumns())
				{
					yield return col3;
					col3 = null;
				}
				IEnumerator<ColumnDefinition> enumerator3 = null;
				break;
			}
			case ViewCharacterMenuKidnap.KidnapSubPage.Property2:
			{
				foreach (ColumnDefinition col4 in this.GenerateProperty2Columns())
				{
					yield return col4;
					col4 = null;
				}
				IEnumerator<ColumnDefinition> enumerator4 = null;
				break;
			}
			case ViewCharacterMenuKidnap.KidnapSubPage.LifeSkill:
			{
				foreach (ColumnDefinition col5 in this.GenerateLifeSkillColumns())
				{
					yield return col5;
					col5 = null;
				}
				IEnumerator<ColumnDefinition> enumerator5 = null;
				break;
			}
			case ViewCharacterMenuKidnap.KidnapSubPage.CombatSkill:
			{
				foreach (ColumnDefinition col6 in this.GenerateCombatSkillColumns())
				{
					yield return col6;
					col6 = null;
				}
				IEnumerator<ColumnDefinition> enumerator6 = null;
				break;
			}
			case ViewCharacterMenuKidnap.KidnapSubPage.Personality:
			{
				foreach (ColumnDefinition col7 in this.GeneratePersonalityColumns())
				{
					yield return col7;
					col7 = null;
				}
				IEnumerator<ColumnDefinition> enumerator7 = null;
				break;
			}
			case ViewCharacterMenuKidnap.KidnapSubPage.Item:
			{
				foreach (ColumnDefinition col8 in this.GenerateItemColumns())
				{
					yield return col8;
					col8 = null;
				}
				IEnumerator<ColumnDefinition> enumerator8 = null;
				break;
			}
			case ViewCharacterMenuKidnap.KidnapSubPage.Command:
			{
				foreach (ColumnDefinition col9 in this.GenerateCommandColumns())
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

		// Token: 0x0600971D RID: 38685 RVA: 0x004673E6 File Offset: 0x004655E6
		private IEnumerable<RowCellContainer> GetCellContainerTemplates(ViewCharacterMenuKidnap.KidnapSubPage subPage)
		{
			yield return this.avatarAndNameCellContainer;
			if (subPage != ViewCharacterMenuKidnap.KidnapSubPage.Kidnap)
			{
				if (subPage != ViewCharacterMenuKidnap.KidnapSubPage.Command)
				{
					int columnCount = this.GetColumnCount(subPage);
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
				int num;
				for (int k = 0; k < 4; k = num + 1)
				{
					yield return this.singleTextCellContainer;
					num = k;
				}
				yield return this.textWithTipCellContainer;
				yield return this.ropeCellContainer;
				yield return this.singleButtonCellContainer;
			}
			yield break;
		}

		// Token: 0x0600971E RID: 38686 RVA: 0x00467400 File Offset: 0x00465600
		private int GetColumnCount(ViewCharacterMenuKidnap.KidnapSubPage subPage)
		{
			if (!true)
			{
			}
			int result;
			switch (subPage)
			{
			case ViewCharacterMenuKidnap.KidnapSubPage.Kidnap:
				result = 7;
				break;
			case ViewCharacterMenuKidnap.KidnapSubPage.State:
				result = 9;
				break;
			case ViewCharacterMenuKidnap.KidnapSubPage.Property:
				result = 10;
				break;
			case ViewCharacterMenuKidnap.KidnapSubPage.Property2:
				result = 9;
				break;
			case ViewCharacterMenuKidnap.KidnapSubPage.LifeSkill:
				result = 17;
				break;
			case ViewCharacterMenuKidnap.KidnapSubPage.CombatSkill:
				result = 15;
				break;
			case ViewCharacterMenuKidnap.KidnapSubPage.Personality:
				result = 7;
				break;
			case ViewCharacterMenuKidnap.KidnapSubPage.Item:
				result = 10;
				break;
			case ViewCharacterMenuKidnap.KidnapSubPage.Command:
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

		// Token: 0x0600971F RID: 38687 RVA: 0x00467478 File Offset: 0x00465678
		private void PrepareRowTemplateContainers(ViewCharacterMenuKidnap.KidnapSubPage subPage)
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
			foreach (RowCellContainer containerTemplate in this.GetCellContainerTemplates(subPage))
			{
				RowCellContainer container = Object.Instantiate<RowCellContainer>(containerTemplate, containerRoot);
				container.gameObject.SetActive(true);
			}
			this.rowTemplate.ResetSibling();
		}

		// Token: 0x06009720 RID: 38688 RVA: 0x00467540 File Offset: 0x00465740
		private ColumnDefinition CreateAvatarWithNameColumn()
		{
			ColumnDefinition<KidnapCharDisplayData, AvatarWithNameCellData> columnDefinition = new ColumnDefinition<KidnapCharDisplayData, AvatarWithNameCellData>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 330f,
				FlexibleWidth = 0f,
				PreferredWidth = 330f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Char_Name.Tr());
			columnDefinition.CellDataGenerator = ((KidnapCharDisplayData data) => AvatarWithNameCellData.FromKidnapCharDisplayData(data, ViewCharacterMenuKidnap.IsTaiwu(data.CharacterId), null, null));
			columnDefinition.SortId = 0;
			return columnDefinition;
		}

		// Token: 0x06009721 RID: 38689 RVA: 0x004675E4 File Offset: 0x004657E4
		private ColumnDefinition CreateTextColumn(Func<string> headerKey, Func<KidnapCharDisplayData, string> valueGetter, short sortId = -1, float minWidth = 30f, float preferredWidth = 90f)
		{
			return new ColumnDefinition<KidnapCharDisplayData, string>
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

		// Token: 0x06009722 RID: 38690 RVA: 0x00467648 File Offset: 0x00465848
		private ColumnDefinition CreateTextWithTipColumn(Func<string> headerKey, Func<KidnapCharDisplayData, string> valueGetter, Func<KidnapCharDisplayData, Action<TooltipInvoker>> tipRefresherGetter, short sortId = -1, float minWidth = 30f, float preferredWidth = 90f)
		{
			return new ColumnDefinition<KidnapCharDisplayData, TextWithTipCellData>
			{
				LayoutOption = new LayoutOption
				{
					MinWidth = minWidth,
					FlexibleWidth = 1f,
					PreferredWidth = preferredWidth,
					Priority = 1
				},
				TableHeadLabel = headerKey,
				CellDataGenerator = ((KidnapCharDisplayData data) => new TextWithTipCellData
				{
					Text = valueGetter(data),
					TipRefresher = tipRefresherGetter(data)
				}),
				SortId = sortId
			};
		}

		// Token: 0x06009723 RID: 38691 RVA: 0x004676CC File Offset: 0x004658CC
		private ColumnDefinition CreateIconAndTextColumn(Func<string> headerKey, Func<KidnapCharDisplayData, IconAndTextCellData> valueGetter, short sortId = -1, float minWidth = 80f, float preferredWidth = 120f)
		{
			return new ColumnDefinition<KidnapCharDisplayData, IconAndTextCellData>
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

		// Token: 0x06009724 RID: 38692 RVA: 0x00467730 File Offset: 0x00465930
		private ColumnDefinition CreateRopeColumn()
		{
			ColumnDefinition<KidnapCharDisplayData, RopeCellData> columnDefinition = new ColumnDefinition<KidnapCharDisplayData, RopeCellData>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 80f,
				FlexibleWidth = 1f,
				PreferredWidth = 120f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_ItemSubType_1206.Tr());
			columnDefinition.CellDataGenerator = ((KidnapCharDisplayData data) => new RopeCellData
			{
				KidnapCharDisplayData = data,
				KidnapperCharId = base.CharacterMenu.CurCharacterId,
				CanOperate = (base.CharacterMenu.CanOperate && !base.CharacterMenu.OpenFromCombatPrepare),
				CurrentCharacterIsTaiwu = base.CharacterMenu.CurrentCharacterIsTaiwu,
				OnRopeChanged = new Action(this.OnRopeChanged)
			});
			columnDefinition.SortId = 1;
			return columnDefinition;
		}

		// Token: 0x06009725 RID: 38693 RVA: 0x004677C0 File Offset: 0x004659C0
		private ColumnDefinition CreateInteractButtonColumn()
		{
			bool canOperateNow = this.CanInteractWithKidnappedCharacter();
			ColumnDefinition<KidnapCharDisplayData, SingleButtonCellData> columnDefinition = new ColumnDefinition<KidnapCharDisplayData, SingleButtonCellData>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 80f,
				FlexibleWidth = 1f,
				PreferredWidth = 120f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_CharacterMenu_Btn_Talk.Tr());
			columnDefinition.CellDataGenerator = delegate(KidnapCharDisplayData data)
			{
				SingleButtonCellData singleButtonCellData = new SingleButtonCellData();
				singleButtonCellData.LabelText = LanguageKey.LK_CharacterMenu_Btn_Talk.Tr();
				Action onClick;
				if (!canOperateNow)
				{
					onClick = delegate()
					{
					};
				}
				else
				{
					onClick = delegate()
					{
						this.OnInteractClicked(data.CharacterId);
					};
				}
				singleButtonCellData.OnClick = onClick;
				singleButtonCellData.SingleButtonCellStatus = (canOperateNow ? SingleButtonCellStatus.EnableInteractable : SingleButtonCellStatus.DisableInteractable);
				return singleButtonCellData;
			};
			return columnDefinition;
		}

		// Token: 0x06009726 RID: 38694 RVA: 0x00467862 File Offset: 0x00465A62
		private IEnumerable<ColumnDefinition> GenerateKidnapColumns()
		{
			yield return this.CreateTextColumn(() => LanguageKey.LK_Health.Tr(), (KidnapCharDisplayData data) => CommonUtils.GetCharacterHealthInfo(data.Health, data.MaxLeftHealth, data.CharacterId).Item1, 10, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Happiness.Tr(), (KidnapCharDisplayData data) => CommonUtils.GetHappinessString(HappinessType.GetHappinessType(data.Happiness)), 12, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Favorability.Tr(), new Func<KidnapCharDisplayData, string>(ViewCharacterMenuKidnap.GetFavorDisplayString), 11, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Alertness.Tr(), (KidnapCharDisplayData data) => CommonUtils.GetAlertnessNameByValue(data.Alertness), 130, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_PrisonTime.Tr(), (KidnapCharDisplayData data) => string.Format("{0}", data.KidnapDuration), 14, 30f, 90f);
			yield return this.CreateTextWithTipColumn(() => LanguageKey.LK_Kidnap_Resistance_Value.Tr(), (KidnapCharDisplayData data) => data.TotalResistance.ToString(), (KidnapCharDisplayData data) => ViewCharacterMenuKidnap.CreateResistanceTipRefresher(data), 120, 30f, 90f);
			yield return this.CreateRopeColumn();
			yield return this.CreateInteractButtonColumn();
			yield break;
		}

		// Token: 0x06009727 RID: 38695 RVA: 0x00467872 File Offset: 0x00465A72
		private IEnumerable<ColumnDefinition> GenerateStateColumns()
		{
			yield return this.CreateTextColumn(() => LanguageKey.LK_Char_Age.Tr(), (KidnapCharDisplayData data) => data.PhysiologicalAge.ToString(), 8, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Health.Tr(), (KidnapCharDisplayData data) => CommonUtils.GetCharacterHealthInfo(data.Health, data.MaxLeftHealth, data.CharacterId).Item1, 10, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Injury.Tr(), (KidnapCharDisplayData data) => data.DefeatMarkCount.ToString(), 53, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Charm.Tr(), new Func<KidnapCharDisplayData, string>(ViewCharacterMenuKidnap.GetCharmDisplayString), 9, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Behavior.Tr(), (KidnapCharDisplayData data) => CommonUtils.GetBehaviorString(data.BehaviorType), 57, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Happiness.Tr(), (KidnapCharDisplayData data) => CommonUtils.GetHappinessString(HappinessType.GetHappinessType(data.Happiness)), 12, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Favorability.Tr(), new Func<KidnapCharDisplayData, string>(ViewCharacterMenuKidnap.GetFavorDisplayString), 11, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Samsara.Tr(), (KidnapCharDisplayData data) => data.PreexistenceCharCount.ToString(), 58, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Main_SummaryInfo_Fame.Tr(), (KidnapCharDisplayData data) => CommonUtils.GetFameString(FameType.GetFameType(data.Fame)), 59, 30f, 90f);
			yield break;
		}

		// Token: 0x06009728 RID: 38696 RVA: 0x00467882 File Offset: 0x00465A82
		private IEnumerable<ColumnDefinition> GeneratePropertyColumns()
		{
			yield return this.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Strength.Tr(), (KidnapCharDisplayData data) => data.MaxMainAttributes[0].ToString(), 60, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Dexterity.Tr(), (KidnapCharDisplayData data) => data.MaxMainAttributes[1].ToString(), 61, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Concentration.Tr(), (KidnapCharDisplayData data) => data.MaxMainAttributes[2].ToString(), 62, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Vitality.Tr(), (KidnapCharDisplayData data) => data.MaxMainAttributes[3].ToString(), 63, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Energy.Tr(), (KidnapCharDisplayData data) => data.MaxMainAttributes[4].ToString(), 64, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Main_Attribute_Intelligence.Tr(), (KidnapCharDisplayData data) => data.MaxMainAttributes[5].ToString(), 65, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Penetrate_Outer.Tr(), (KidnapCharDisplayData data) => data.Penetrations.Outer.ToString(), 22, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Penetrate_Inner.Tr(), (KidnapCharDisplayData data) => data.Penetrations.Inner.ToString(), 23, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Penetrate_Resist_Outer.Tr(), (KidnapCharDisplayData data) => data.PenetrationResists.Outer.ToString(), 29, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Penetrate_Resist_Inner.Tr(), (KidnapCharDisplayData data) => data.PenetrationResists.Inner.ToString(), 30, 30f, 90f);
			yield break;
		}

		// Token: 0x06009729 RID: 38697 RVA: 0x00467892 File Offset: 0x00465A92
		private IEnumerable<ColumnDefinition> GenerateProperty2Columns()
		{
			yield return this.CreateTextColumn(() => LanguageKey.LK_HitType_0.Tr(), (KidnapCharDisplayData data) => data.HitValues[0].ToString(), 24, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_HitType_1.Tr(), (KidnapCharDisplayData data) => data.HitValues[1].ToString(), 25, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_HitType_2.Tr(), (KidnapCharDisplayData data) => data.HitValues[2].ToString(), 26, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_HitType_3.Tr(), (KidnapCharDisplayData data) => data.HitValues[3].ToString(), 27, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_AvoidType_0.Tr(), (KidnapCharDisplayData data) => data.AvoidValues[0].ToString(), 33, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_AvoidType_1.Tr(), (KidnapCharDisplayData data) => data.AvoidValues[1].ToString(), 34, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_AvoidType_2.Tr(), (KidnapCharDisplayData data) => data.AvoidValues[2].ToString(), 35, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_AvoidType_3.Tr(), (KidnapCharDisplayData data) => data.AvoidValues[3].ToString(), 36, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Qi_Disorder.Tr(), (KidnapCharDisplayData data) => ((int)(data.DisorderOfQi / 10)).ToString(), 55, 30f, 90f);
			yield break;
		}

		// Token: 0x0600972A RID: 38698 RVA: 0x004678A2 File Offset: 0x00465AA2
		private IEnumerable<ColumnDefinition> GenerateLifeSkillColumns()
		{
			int num;
			for (int i = 0; i < 16; i = num + 1)
			{
				ViewCharacterMenuKidnap.<>c__DisplayClass76_0 CS$<>8__locals1 = new ViewCharacterMenuKidnap.<>c__DisplayClass76_0();
				CS$<>8__locals1.index = i;
				yield return this.CreateTextColumn(() => LocalStringManager.Get(string.Format("LK_LifeSkillType_{0}", CS$<>8__locals1.index)), (KidnapCharDisplayData data) => data.LifeSkillQualifications[CS$<>8__locals1.index].ToString(), (short)(66 + CS$<>8__locals1.index), 40f, 60f);
				CS$<>8__locals1 = null;
				num = i;
			}
			yield return this.CreateTextColumn(() => LanguageKey.LK_Growth.Tr(), (KidnapCharDisplayData data) => ViewCharacterMenuKidnap.GetSkillGrowthString(data.ActualAge, data.LifeSkillGrowthType), 118, 30f, 90f);
			yield break;
		}

		// Token: 0x0600972B RID: 38699 RVA: 0x004678B2 File Offset: 0x00465AB2
		private IEnumerable<ColumnDefinition> GenerateCombatSkillColumns()
		{
			int num;
			for (int i = 0; i < 14; i = num + 1)
			{
				ViewCharacterMenuKidnap.<>c__DisplayClass77_0 CS$<>8__locals1 = new ViewCharacterMenuKidnap.<>c__DisplayClass77_0();
				CS$<>8__locals1.index = i;
				yield return this.CreateTextColumn(() => LocalStringManager.Get(string.Format("LK_CombatSkillType_{0}", CS$<>8__locals1.index)), (KidnapCharDisplayData data) => data.CombatSkillQualifications[CS$<>8__locals1.index].ToString(), (short)(82 + CS$<>8__locals1.index), 40f, 60f);
				CS$<>8__locals1 = null;
				num = i;
			}
			yield return this.CreateTextColumn(() => LanguageKey.LK_Growth.Tr(), (KidnapCharDisplayData data) => ViewCharacterMenuKidnap.GetSkillGrowthString(data.ActualAge, data.CombatSkillGrowthType), 119, 30f, 90f);
			yield break;
		}

		// Token: 0x0600972C RID: 38700 RVA: 0x004678C2 File Offset: 0x00465AC2
		private IEnumerable<ColumnDefinition> GeneratePersonalityColumns()
		{
			yield return this.CreateTextColumn(() => LanguageKey.LK_Personality_Calm_Name.Tr(), (KidnapCharDisplayData data) => data.Personalities[0].ToString(), 96, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Personality_Clever_Name.Tr(), (KidnapCharDisplayData data) => data.Personalities[1].ToString(), 97, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Personality_Enthusiastic_Name.Tr(), (KidnapCharDisplayData data) => data.Personalities[2].ToString(), 98, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Personality_Brave_Name.Tr(), (KidnapCharDisplayData data) => data.Personalities[3].ToString(), 99, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Personality_Firm_Name.Tr(), (KidnapCharDisplayData data) => data.Personalities[4].ToString(), 100, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Personality_Lucky_Name.Tr(), (KidnapCharDisplayData data) => data.Personalities[5].ToString(), 101, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Personality_Perceptive_Name.Tr(), (KidnapCharDisplayData data) => data.Personalities[6].ToString(), 102, 30f, 90f);
			yield break;
		}

		// Token: 0x0600972D RID: 38701 RVA: 0x004678D2 File Offset: 0x00465AD2
		private IEnumerable<ColumnDefinition> GenerateItemColumns()
		{
			yield return this.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Food.Tr(), (KidnapCharDisplayData data) => data.Resources[0].ToString(), 103, 40f, 60f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Wood.Tr(), (KidnapCharDisplayData data) => data.Resources[1].ToString(), 104, 40f, 60f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Metal.Tr(), (KidnapCharDisplayData data) => data.Resources[2].ToString(), 105, 40f, 60f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Jade.Tr(), (KidnapCharDisplayData data) => data.Resources[3].ToString(), 106, 40f, 60f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Fabric.Tr(), (KidnapCharDisplayData data) => data.Resources[4].ToString(), 107, 40f, 60f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Herb.Tr(), (KidnapCharDisplayData data) => data.Resources[5].ToString(), 108, 40f, 60f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Money.Tr(), (KidnapCharDisplayData data) => data.Resources[6].ToString(), 109, 40f, 60f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Resource_Name_Authority.Tr(), (KidnapCharDisplayData data) => data.Resources[7].ToString(), 110, 40f, 60f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Inventory.Tr(), (KidnapCharDisplayData data) => ViewCharacterMenuKidnap.GetInventoryLoadString(data.CurrInventoryLoad, data.MaxInventoryLoad), 37, 30f, 90f);
			yield return this.CreateTextColumn(() => LanguageKey.LK_Kidnap.Tr(), (KidnapCharDisplayData data) => data.KidnapCount.ToString(), 111, 30f, 90f);
			yield break;
		}

		// Token: 0x0600972E RID: 38702 RVA: 0x004678E2 File Offset: 0x00465AE2
		private IEnumerable<ColumnDefinition> GenerateCommandColumns()
		{
			yield return this.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Attack.Tr(), (KidnapCharDisplayData data) => ViewCharacterMenuKidnap.CreateMedalCellData(data.AttackMedal, 0), 112, 80f, 120f);
			yield return this.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Defence.Tr(), (KidnapCharDisplayData data) => ViewCharacterMenuKidnap.CreateMedalCellData(data.DefenceMedal, 1), 113, 80f, 120f);
			yield return this.CreateIconAndTextColumn(() => LanguageKey.LK_Feature_Wisdom.Tr(), (KidnapCharDisplayData data) => ViewCharacterMenuKidnap.CreateMedalCellData(data.WisdomMedal, 2), 114, 80f, 120f);
			yield return this.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_0.Tr(), (KidnapCharDisplayData data) => ViewCharacterMenuKidnap.CreateCommandCellData(data, 0), 115, 80f, 120f);
			yield return this.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_1.Tr(), (KidnapCharDisplayData data) => ViewCharacterMenuKidnap.CreateCommandCellData(data, 1), 116, 80f, 120f);
			yield return this.CreateIconAndTextColumn(() => LanguageKey.LK_Team_Property_Title_Command_2.Tr(), (KidnapCharDisplayData data) => ViewCharacterMenuKidnap.CreateCommandCellData(data, 2), 117, 80f, 120f);
			yield break;
		}

		// Token: 0x0600972F RID: 38703 RVA: 0x004678F4 File Offset: 0x00465AF4
		private static IconAndTextCellData CreateMedalCellData(int medalCount, int medalType)
		{
			bool flag = medalCount == 0;
			IconAndTextCellData result;
			if (flag)
			{
				result = IconAndTextCellData.TextOnly("-");
			}
			else
			{
				string iconName = ViewCharacterMenuKidnap.GetMedalIconName(medalCount, medalType);
				string text = string.Format(" x{0}", Mathf.Abs(medalCount));
				result = new IconAndTextCellData(iconName, text, true, false, false, false);
			}
			return result;
		}

		// Token: 0x06009730 RID: 38704 RVA: 0x00467944 File Offset: 0x00465B44
		private static string GetMedalIconName(int medalCount, int medalType)
		{
			int signKey = (medalCount > 0) ? 1 : ((medalCount < 0) ? -1 : 0);
			if (!true)
			{
			}
			string text;
			switch (medalType)
			{
			case 0:
				text = MedalSummary.AttackMedalIconConfig[signKey];
				break;
			case 1:
				text = MedalSummary.DefenceMedalIconConfig[signKey];
				break;
			case 2:
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

		// Token: 0x06009731 RID: 38705 RVA: 0x004679C4 File Offset: 0x00465BC4
		private static IconAndTextCellData CreateCommandCellData(KidnapCharDisplayData data, int commandIndex)
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

		// Token: 0x06009732 RID: 38706 RVA: 0x00467A4C File Offset: 0x00465C4C
		private static string GetCharmDisplayString(KidnapCharDisplayData data)
		{
			return CommonUtils.GetCharmLevelText(data.Charm, data.Gender, data.PhysiologicalAge, data.ClothDisplayId, CreatingType.IsFixedPresetType(data.CreatingType), data.FaceVisible);
		}

		// Token: 0x06009733 RID: 38707 RVA: 0x00467A8C File Offset: 0x00465C8C
		private static string GetFavorDisplayString(KidnapCharDisplayData data)
		{
			return CommonUtils.GetFavorStringByInteracted(data.FavorabilityToTaiwu, data.IsInteractedWithTaiwu);
		}

		// Token: 0x06009734 RID: 38708 RVA: 0x00467AB0 File Offset: 0x00465CB0
		private static string GetSkillGrowthString(short actualAge, sbyte growthType)
		{
			sbyte addValue = ViewCharacterMenuKidnap.GetSkillGrowthAddValue(actualAge, (int)growthType);
			string growthName = (growthType == 0) ? LocalStringManager.Get("LK_Qualification_Growth_Average") : ((growthType == 1) ? LocalStringManager.Get("LK_Qualification_Growth_Precocious") : LocalStringManager.Get("LK_Qualification_Growth_LateBlooming"));
			bool flag = addValue > 0;
			string addValueStr;
			if (flag)
			{
				addValueStr = string.Format("+{0}", addValue).SetColor("lightblue");
			}
			else
			{
				bool flag2 = addValue == 0;
				if (flag2)
				{
					addValueStr = "+0".SetColor("lightgrey");
				}
				else
				{
					addValueStr = string.Format("{0}", addValue).SetColor("red");
				}
			}
			return growthName + addValueStr;
		}

		// Token: 0x06009735 RID: 38709 RVA: 0x00467B5C File Offset: 0x00465D5C
		private static sbyte GetSkillGrowthAddValue(short actualAge, int growthType)
		{
			AgeEffectItem ageData = AgeEffect.Instance[Math.Min((int)actualAge, AgeEffect.Instance.Count - 1)];
			return (growthType == 0) ? ageData.SkillQualificationAverage : ((growthType == 1) ? ageData.SkillQualificationPrecocious : ageData.SkillQualificationLateBlooming);
		}

		// Token: 0x06009736 RID: 38710 RVA: 0x00467BA8 File Offset: 0x00465DA8
		private static string GetInventoryLoadString(int currLoad, int maxLoad)
		{
			string currLoadStr = ((float)currLoad / 100f).ToString("f1").SetColor(CommonUtils.GetLoadWeightValueColor(currLoad, maxLoad));
			return string.Format("{0}/{1:f1}", currLoadStr, (float)maxLoad / 100f);
		}

		// Token: 0x06009737 RID: 38711 RVA: 0x00467BF4 File Offset: 0x00465DF4
		private static Action<TooltipInvoker> CreateResistanceTipRefresher(KidnapCharDisplayData data)
		{
			return delegate(TooltipInvoker tip)
			{
				bool flag = tip == null;
				if (!flag)
				{
					tip.Type = TipType.PrisonerResistance;
					if (tip.RuntimeParam == null)
					{
						tip.RuntimeParam = EasyPool.Get<ArgumentBox>();
					}
					tip.RuntimeParam.Set("IsPrivate", true).Set("Resistance", data.TotalResistance).Set("EscapeRate", data.EscapeRate).Set("RopeEffect", data.RopeEffect).Set("CompletelyInfected", data.CompletelyInfected).Set("OwningBook", data.OwningBook);
				}
			};
		}

		// Token: 0x06009738 RID: 38712 RVA: 0x00467C20 File Offset: 0x00465E20
		private void OnSubPageChanged(ViewCharacterMenuKidnap.KidnapSubPage subPage)
		{
			bool flag = this._currentSubPage == subPage;
			if (!flag)
			{
				this._currentSubPage = subPage;
				TabSortStateManager<ViewCharacterMenuKidnap.KidnapSubPage, KidnapCharDisplayData> tabSortStateManager = this._tabSortStateManager;
				if (tabSortStateManager != null)
				{
					tabSortStateManager.OnTabChange(subPage);
				}
				this.RefreshAll();
			}
		}

		// Token: 0x06009739 RID: 38713 RVA: 0x00467C5E File Offset: 0x00465E5E
		private void OnSortOrFilterChanged()
		{
			this.RefreshAll();
		}

		// Token: 0x0600973A RID: 38714 RVA: 0x00467C68 File Offset: 0x00465E68
		private void RefreshCharacterDropdown()
		{
			bool flag = this.switchCharacterDropdown == null;
			if (!flag)
			{
				this.switchCharacterDropdown.ClearOptions();
				List<string> options = new List<string>();
				foreach (CharacterDisplayData displayData in this._characterDisplayDataList)
				{
					string displayName = NameCenter.GetMonasticTitleOrDisplayName(displayData, ViewCharacterMenuKidnap.IsTaiwu(displayData.CharacterId));
					options.Add(displayName);
				}
				this.switchCharacterDropdown.AddOptions(options);
				this.switchCharacterDropdown.template.SetHeight((float)Math.Min(10, options.Count) * 56f);
				int currentIndex = -1;
				IReadOnlyList<CharacterDisplayData> characters = base.CharacterMenu.DisplayCharacters;
				bool flag2 = characters != null;
				if (flag2)
				{
					for (int i = 0; i < characters.Count; i++)
					{
						bool flag3 = characters[i] != null && characters[i].CharacterId == base.CharacterMenu.CurCharacterId;
						if (flag3)
						{
							currentIndex = i;
							break;
						}
					}
				}
				bool flag4 = currentIndex >= 0;
				if (flag4)
				{
					this.switchCharacterDropdown.SetValueWithoutNotify(currentIndex);
				}
			}
		}

		// Token: 0x0600973B RID: 38715 RVA: 0x00467DB8 File Offset: 0x00465FB8
		private void OnCharacterDropdownValueChanged(int index)
		{
			IReadOnlyList<CharacterDisplayData> characters = base.CharacterMenu.DisplayCharacters;
			bool flag = characters == null || index < 0 || index >= characters.Count || characters[index] == null;
			if (!flag)
			{
				int newCharId = characters[index].CharacterId;
				bool flag2 = newCharId != base.CharacterMenu.CurCharacterId;
				if (flag2)
				{
					base.CharacterMenu.SelectCharacter(newCharId);
					CharacterDomainMethod.Call.GetKidnapMenuDisplayData(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
				}
			}
		}

		// Token: 0x0600973C RID: 38716 RVA: 0x00467E44 File Offset: 0x00466044
		private static bool IsTaiwu(int charId)
		{
			return charId == ViewCharacterMenuKidnap.TaiwuId;
		}

		// Token: 0x040073E8 RID: 29672
		[SerializeField]
		private CToggleGroup subPageToggleGroup;

		// Token: 0x040073E9 RID: 29673
		[SerializeField]
		private SortAndFilterLegacy sortAndFilter;

		// Token: 0x040073EA RID: 29674
		[SerializeField]
		private CDropdown switchModeDropdown;

		// Token: 0x040073EB RID: 29675
		[SerializeField]
		private GameObject listModeRoot;

		// Token: 0x040073EC RID: 29676
		[SerializeField]
		private GameObject gridModeRoot;

		// Token: 0x040073ED RID: 29677
		[SerializeField]
		private RectTransform countLabelBg;

		// Token: 0x040073EE RID: 29678
		[SerializeField]
		private TextMeshProUGUI countLabel;

		// Token: 0x040073EF RID: 29679
		[SerializeField]
		private CharacterMenuLocalLoadingAnim localLoadingAnim;

		// Token: 0x040073F0 RID: 29680
		[SerializeField]
		private ListStyleGeneralScroll listScroll;

		// Token: 0x040073F1 RID: 29681
		[Header("行模板配置")]
		[SerializeField]
		private RowItem rowTemplate;

		// Token: 0x040073F2 RID: 29682
		[SerializeField]
		private RowCellContainer singleTextCellContainer;

		// Token: 0x040073F3 RID: 29683
		[SerializeField]
		private RowCellContainer textWithTipCellContainer;

		// Token: 0x040073F4 RID: 29684
		[SerializeField]
		private RowCellContainer avatarAndNameCellContainer;

		// Token: 0x040073F5 RID: 29685
		[SerializeField]
		private RowCellContainer iconAndTextCellContainer;

		// Token: 0x040073F6 RID: 29686
		[SerializeField]
		private RowCellContainer ropeCellContainer;

		// Token: 0x040073F7 RID: 29687
		[SerializeField]
		private RowCellContainer singleButtonCellContainer;

		// Token: 0x040073F8 RID: 29688
		[SerializeField]
		private CDropdown switchCharacterDropdown;

		// Token: 0x040073F9 RID: 29689
		[SerializeField]
		private RectTransform switchCharacterDropdownContentRect;

		// Token: 0x040073FA RID: 29690
		[SerializeField]
		private InfinityScroll gridScroll;

		// Token: 0x040073FB RID: 29691
		[SerializeField]
		private GameObject gridScrollEmpty;

		// Token: 0x040073FC RID: 29692
		private const float DropdownOptionHeight = 56f;

		// Token: 0x040073FD RID: 29693
		private static readonly List<LanguageKey> ToggleGroupNameKeys = new List<LanguageKey>
		{
			LanguageKey.LK_Kidnap,
			LanguageKey.LK_Team_Tog_State,
			LanguageKey.LK_Team_Tog_Property,
			LanguageKey.LK_Team_Tog_Property_Hit,
			LanguageKey.LK_Team_Tog_LifeSkill,
			LanguageKey.LK_Team_Tog_CombatSkill,
			LanguageKey.LK_Team_Tog_Personality,
			LanguageKey.LK_Team_Tog_Item,
			LanguageKey.LK_Team_Tog_Command
		};

		// Token: 0x040073FE RID: 29694
		private const float CountLabelBgWidthInGridMode = 2074f;

		// Token: 0x040073FF RID: 29695
		private const float CountLabelBgWidthInListMode = 2520f;

		// Token: 0x04007400 RID: 29696
		private ViewCharacterMenuKidnap.KidnapSubPage _currentSubPage = ViewCharacterMenuKidnap.KidnapSubPage.Kidnap;

		// Token: 0x04007401 RID: 29697
		private KidnapMenuDisplayData _kidnapMenuData;

		// Token: 0x04007402 RID: 29698
		private List<KidnapCharDisplayData> _dataList = new List<KidnapCharDisplayData>();

		// Token: 0x04007403 RID: 29699
		private List<KidnapCharDisplayData> _filteredDataList = new List<KidnapCharDisplayData>();

		// Token: 0x04007404 RID: 29700
		private KidnapSortAndFilterController _sortAndFilterController;

		// Token: 0x04007405 RID: 29701
		private TabSortStateManager<ViewCharacterMenuKidnap.KidnapSubPage, KidnapCharDisplayData> _tabSortStateManager;

		// Token: 0x04007406 RID: 29702
		private List<CharacterDisplayData> _characterDisplayDataList = new List<CharacterDisplayData>();

		// Token: 0x04007407 RID: 29703
		private bool _isGridMode = true;

		// Token: 0x0200226C RID: 8812
		private enum KidnapSubPage
		{
			// Token: 0x0400DA57 RID: 55895
			Kidnap,
			// Token: 0x0400DA58 RID: 55896
			State,
			// Token: 0x0400DA59 RID: 55897
			Property,
			// Token: 0x0400DA5A RID: 55898
			Property2,
			// Token: 0x0400DA5B RID: 55899
			LifeSkill,
			// Token: 0x0400DA5C RID: 55900
			CombatSkill,
			// Token: 0x0400DA5D RID: 55901
			Personality,
			// Token: 0x0400DA5E RID: 55902
			Item,
			// Token: 0x0400DA5F RID: 55903
			Command
		}
	}
}
