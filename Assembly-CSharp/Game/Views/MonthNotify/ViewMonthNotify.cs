using System;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.ListStyleGeneralScroll.CellContent;
using GameData.Common;
using GameData.Domains.Character.Display;
using GameData.Domains.LifeRecord.GeneralRecord;
using GameData.Domains.Taiwu;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.World;
using GameData.Domains.World.Display;
using GameData.Domains.World.MonthlyEvent;
using GameData.Domains.World.Notification;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UILogic.DisplayDataStructure;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.MonthNotify
{
	// Token: 0x020008C5 RID: 2245
	public class ViewMonthNotify : UIBase
	{
		// Token: 0x17000CA0 RID: 3232
		// (get) Token: 0x06006AF4 RID: 27380 RVA: 0x00316EE5 File Offset: 0x003150E5
		private static MonthlyNotificationSortingGroups MonthlyNotificationSortingGroups
		{
			get
			{
				return SingletonObject.getInstance<MonthlyNotificationSortingGroups>();
			}
		}

		// Token: 0x17000CA1 RID: 3233
		// (get) Token: 0x06006AF5 RID: 27381 RVA: 0x00316EEC File Offset: 0x003150EC
		private MonthNotify CurrentDisplayData
		{
			get
			{
				return this._data.MonthNotifies[this._index];
			}
		}

		// Token: 0x17000CA2 RID: 3234
		// (get) Token: 0x06006AF6 RID: 27382 RVA: 0x00316F04 File Offset: 0x00315104
		private int Date
		{
			get
			{
				return this.CurrentDisplayData.Date;
			}
		}

		// Token: 0x17000CA3 RID: 3235
		// (get) Token: 0x06006AF7 RID: 27383 RVA: 0x00316F11 File Offset: 0x00315111
		public static bool NewMonthEventSend
		{
			get
			{
				return !SingletonObject.getInstance<EventModel>().NeedToNotifyNewMonth;
			}
		}

		// Token: 0x17000CA4 RID: 3236
		// (get) Token: 0x06006AF8 RID: 27384 RVA: 0x00316F20 File Offset: 0x00315120
		public string YearMonthDesc
		{
			get
			{
				return LanguageKey.UI_AdvanceMonth_Note_TimeChangeInfo_Month.TrFormat(CommonUtils.GetYearByDate(this.Date), CommonUtils.GetMonthByDate(this.Date) + 1);
			}
		}

		// Token: 0x06006AF9 RID: 27385 RVA: 0x00316F4E File Offset: 0x0031514E
		public static MonthlyNotificationItem GetConfig(NotificationItem item)
		{
			return MonthlyNotification.Instance[item.RenderInfoList[0].RecordType];
		}

		// Token: 0x06006AFA RID: 27386 RVA: 0x00316F6C File Offset: 0x0031516C
		public void RenderMonthlyNotificationTips(NotificationItem item, ArgumentBox argBox)
		{
			argBox.Clear();
			argBox.Set("TemplateId", item.RenderInfoList[0].RecordType);
			argBox.Set("Content", item.ToString());
			List<int> charIds = item.CharIds;
			bool flag = charIds != null && charIds.Count > 0 && this._index >= 0 && this.CurrentDisplayData != null;
			if (flag)
			{
				MonthNotify data = this.CurrentDisplayData;
				List<AvatarWithNameCellData> list = new List<AvatarWithNameCellData>();
				foreach (int charId in item.CharIds)
				{
					NameAndLifeRelatedData charName;
					bool flag2 = data.CharacterNames.TryGetValue(charId, out charName);
					if (flag2)
					{
						AvatarRelatedData avatar;
						bool isGrave = !data.Avatars.TryGetValue(charId, out avatar);
						string nameStr = NameCenter.GetMonasticTitleOrDisplayName(ref charName.NameRelatedData, charId == data.TaiwuId, false);
						list.Add(new AvatarWithNameCellData(avatar, charName.NameRelatedData.CharTemplateId, nameStr, charId, isGrave, null, null, false, false));
					}
				}
				argBox.SetObject("CharacterList", list);
			}
		}

		// Token: 0x06006AFB RID: 27387 RVA: 0x003170A8 File Offset: 0x003152A8
		private void CloseSubpage()
		{
			this.subpageParent.SetActive(false);
			this.jottings.gameObject.SetActive(false);
			this.village.gameObject.SetActive(false);
			this.information.gameObject.SetActive(false);
		}

		// Token: 0x06006AFC RID: 27388 RVA: 0x003170F9 File Offset: 0x003152F9
		public override void OnInit(ArgumentBox argsBox)
		{
			if (argsBox != null)
			{
				argsBox.Get("NeedSave", out this._needSave);
			}
			this.RefreshHotkeyCommands();
		}

		// Token: 0x06006AFD RID: 27389 RVA: 0x0031711C File Offset: 0x0031531C
		private void Awake()
		{
			this.InitHotKeyCommands();
			this.InitMonthDesc();
			this.InitMonthPanel();
			this.InitDateSelector();
			this.InitSectionTypes();
			this.InitSubpages();
			GEvent.Add(EEvents.OnGameStateChange, new GEvent.Callback(this.OnQuit));
		}

		// Token: 0x06006AFE RID: 27390 RVA: 0x0031716C File Offset: 0x0031536C
		private void OnEnable()
		{
			this.mask.SetActive(false);
			GEvent.Add(EEvents.OnSavingWorldStateChange, new GEvent.Callback(this.OnSavingWorldStateChange));
			GEvent.Add(UiEvents.OnMonthNotifySortingGroupChanged, new GEvent.Callback(this.OnGroupSettingChange));
			bool flag = ViewMonthNotify._lastDate < 0 || ViewMonthNotify._lastDate != SingletonObject.getInstance<BasicGameData>().CurrDate;
			if (flag)
			{
				this.animRoot.GetComponent<CanvasGroup>().alpha = 0f;
				this.leftCanvasGroup.alpha = 0f;
				this.rightCanvasGroup.alpha = 0f;
			}
			this.requestEventFlag = false;
			this.RequestData();
		}

		// Token: 0x06006AFF RID: 27391 RVA: 0x00317224 File Offset: 0x00315424
		private void OnDisable()
		{
			this._playedInitAnim = false;
			this.dateSelectorToggle.isOn = false;
			GEvent.Remove(EEvents.OnSavingWorldStateChange, new GEvent.Callback(this.OnSavingWorldStateChange));
			GEvent.Remove(UiEvents.OnMonthNotifySortingGroupChanged, new GEvent.Callback(this.OnGroupSettingChange));
		}

		// Token: 0x06006B00 RID: 27392 RVA: 0x00317278 File Offset: 0x00315478
		private void Update()
		{
			bool refreshing = this._refreshing;
			if (!refreshing)
			{
				foreach (KeyValuePair<HotKeyCommand, Action> keyValuePair in this._commonHotKeyCommands)
				{
					HotKeyCommand hotKeyCommand;
					Action action2;
					keyValuePair.Deconstruct(out hotKeyCommand, out action2);
					HotKeyCommand command = hotKeyCommand;
					Action action = action2;
					bool flag = command.Check(this.Element, false, false, true, true, false);
					if (flag)
					{
						action();
					}
				}
			}
		}

		// Token: 0x06006B01 RID: 27393 RVA: 0x00317304 File Offset: 0x00315504
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(5, 4, ulong.MaxValue, null));
		}

		// Token: 0x06006B02 RID: 27394 RVA: 0x00317320 File Offset: 0x00315520
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 0)
				{
					DataUid uid = notification.Uid;
					bool flag = uid.DomainId == 5 && uid.DataId == 4;
					if (flag)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._needToEscape);
					}
				}
			}
		}

		// Token: 0x06006B03 RID: 27395 RVA: 0x003173C8 File Offset: 0x003155C8
		public override void QuickHide()
		{
			bool flag = SingletonObject.getInstance<BasicGameData>().SavingWorld || SingletonObject.getInstance<BasicGameData>().AdvancingMonthState != 0;
			if (!flag)
			{
				bool activeSelf = this.subpageParent.activeSelf;
				if (activeSelf)
				{
					this.CloseSubpage();
				}
				else
				{
					List<MonthlyEventRenderInfo> monthlyEventRenderInfoList = this._monthlyEventRenderInfoList;
					bool flag2 = monthlyEventRenderInfoList == null || monthlyEventRenderInfoList.Count <= 0;
					if (flag2)
					{
						GEvent.OnEvent(UiEvents.MonthNotifyProcessComplete, null);
						bool needToEscape = this._needToEscape;
						if (needToEscape)
						{
							SingletonObject.getInstance<WorldMapModel>().ChangeTaiwuMoveState(WorldMapModel.MoveState.EscapeToAdjacentBlock);
							TaiwuDomainMethod.Call.EscapeToAdjacentBlock();
							this._needToEscape = false;
						}
						TaiwuEventDomainMethod.Call.TriggerListener("MonthNotifyShowed", true);
						bool flag3 = !ViewMonthNotify.NewMonthEventSend;
						if (flag3)
						{
							TaiwuEventDomainMethod.Call.OnNewGameMonth();
						}
						TaiwuEventDomainMethod.Call.CloseUI("UI_MonthNotify");
						base.QuickHide();
					}
				}
			}
		}

		// Token: 0x06006B04 RID: 27396 RVA: 0x003174A0 File Offset: 0x003156A0
		private void TryEndAdvancingMonth()
		{
			bool flag = SingletonObject.getInstance<BasicGameData>().AdvancingMonthState != 14;
			if (!flag)
			{
				bool needSave = this._needSave && !GMFunc.DisableAutoSaving;
				WorldDomainMethod.Call.AdvanceMonth_DisplayedMonthlyNotifications(needSave);
			}
		}

		// Token: 0x06006B05 RID: 27397 RVA: 0x003174E0 File Offset: 0x003156E0
		private void RequestData()
		{
			WorldDomainMethod.Call.RemoveAllInvalidMonthlyEvents();
			bool flag = !this.requestEventFlag;
			if (flag)
			{
				this.RequestEventCollection();
			}
			bool flag2;
			if (ViewMonthNotify._lastDate == SingletonObject.getInstance<BasicGameData>().CurrDate)
			{
				MonthNotifyDisplayData data = this._data;
				if (data != null && data.MonthNotifies != null)
				{
					flag2 = (this._data.MonthNotifies.Count != 0);
					goto IL_54;
				}
			}
			flag2 = false;
			IL_54:
			bool flag3 = flag2;
			if (flag3)
			{
				this.review.Set(this._classifiedData);
				this.Element.ShowAfterRefresh();
				this.InitOpenAnim();
			}
			else
			{
				bool flag4 = ViewMonthNotify._lastDate == -1;
				if (flag4)
				{
					bool flag5 = !ViewMonthNotify.MonthlyNotificationSortingGroups.IsReady;
					if (flag5)
					{
						return;
					}
				}
				ViewMonthNotify._lastDate = SingletonObject.getInstance<BasicGameData>().CurrDate;
				this._refreshing = true;
				WorldDomainMethod.AsyncCall.GetNewestMonthNotifyDisplayData(null, delegate(int offset, RawDataPool pool)
				{
					Serializer.Deserialize(pool, offset, ref this._data);
					this.RefreshSimple();
					this.Element.ShowAfterRefresh();
					this.InitOpenAnim();
				});
				WorldDomainMethod.AsyncCall.GetMonthNotifyDisplayData(null, delegate(int offset, RawDataPool pool)
				{
					Serializer.Deserialize(pool, offset, ref this._data);
					this.RefreshMonthSlider();
				});
			}
		}

		// Token: 0x06006B06 RID: 27398 RVA: 0x003175CC File Offset: 0x003157CC
		private void RequestEventCollection()
		{
			this.requestEventFlag = true;
			bool flag = SingletonObject.getInstance<BasicGameData>().AdvancingMonthState != 0;
			if (flag)
			{
				WorldDomainMethod.AsyncCall.GetMonthlyEventCollection(this, delegate(int offset, RawDataPool pool)
				{
					MonthlyEventCollection monthlyEventCollection = null;
					Serializer.Deserialize(pool, offset, ref monthlyEventCollection);
					this._monthlyEventArgumentCollection.Clear();
					this._monthlyEventRenderInfoList.Clear();
					if (monthlyEventCollection != null)
					{
						monthlyEventCollection.GetRenderInfos(this._monthlyEventRenderInfoList, this._monthlyEventArgumentCollection);
					}
					bool flag2 = this._monthlyEventRenderInfoList.Count < 1;
					if (flag2)
					{
						this.TryEndAdvancingMonth();
					}
					else
					{
						UIElement.MonthlyEvent.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("NeedSave", this._needSave).SetObject("RenderInfoList", this._monthlyEventRenderInfoList).SetObject("Arguments", this._monthlyEventArgumentCollection));
						UIManager.Instance.MaskUI(UIElement.MonthlyEvent);
					}
				});
			}
		}

		// Token: 0x06006B07 RID: 27399 RVA: 0x00317607 File Offset: 0x00315807
		private void InitHotKeyCommands()
		{
			this._commonHotKeyCommands.Add(TabSwitchCommandKit.PrevTabLevel1, new Action(this.OnClickBtnLeft));
			this._commonHotKeyCommands.Add(TabSwitchCommandKit.NextTabLevel1, new Action(this.OnClickBtnRight));
		}

		// Token: 0x06006B08 RID: 27400 RVA: 0x00317644 File Offset: 0x00315844
		private void InitMonthDesc()
		{
			this.btnClose.ClearAndAddListener(new Action(this.QuickHide));
			this.btnGroupSetting.ClearAndAddListener(new Action(this.OnClickSettings));
		}

		// Token: 0x06006B09 RID: 27401 RVA: 0x00317678 File Offset: 0x00315878
		private void InitDateSelector()
		{
			this.monthToggleGroup.Init(-1);
			this.monthToggleGroup.OnActiveIndexChange += this.OnMonthToggleGroupChange;
		}

		// Token: 0x06006B0A RID: 27402 RVA: 0x003176A0 File Offset: 0x003158A0
		private void InitSectionTypes()
		{
			for (int i = 0; i <= 3; i++)
			{
				this._classifiedData.Add((EMonthlyNotificationSectionType)i, new List<NotificationItem>());
			}
			this.review.Init(new Action<NotificationItem, ArgumentBox>(this.RenderMonthlyNotificationTips));
		}

		// Token: 0x06006B0B RID: 27403 RVA: 0x003176EC File Offset: 0x003158EC
		private void InitMonthPanel()
		{
			this.btnLeft.ClearAndAddListener(new Action(this.OnClickBtnLeft));
			this.btnRight.ClearAndAddListener(new Action(this.OnClickBtnRight));
			this.btnMin.ClearAndAddListener(new Action(this.OnClickBtnMin));
			this.btnMax.ClearAndAddListener(new Action(this.OnClickBtnMax));
			this.dateSelectorToggle.onValueChanged.ResetListener(new Action<bool>(this.OnClickMonthPanelToggle));
		}

		// Token: 0x06006B0C RID: 27404 RVA: 0x00317778 File Offset: 0x00315978
		private void InitSubpages()
		{
			this.btnJottings.ClearAndAddListener(new Action(this.OnClickJottings));
			this.btnInformation.ClearAndAddListener(new Action(this.OnClickInformation));
			this.btnVillage.ClearAndAddListener(new Action(this.OnClickVillage));
			this.btnJottings.GetComponent<PointerTrigger>().EnterEvent.ResetListener(delegate()
			{
				this.btnJottings.transform.GetChild(0).gameObject.SetActive(true);
			});
			this.btnJottings.GetComponent<PointerTrigger>().ExitEvent.ResetListener(delegate()
			{
				this.btnJottings.transform.GetChild(0).gameObject.SetActive(false);
			});
			this.btnInformation.GetComponent<PointerTrigger>().EnterEvent.ResetListener(delegate()
			{
				this.btnInformation.transform.GetChild(0).gameObject.SetActive(true);
			});
			this.btnInformation.GetComponent<PointerTrigger>().ExitEvent.ResetListener(delegate()
			{
				this.btnInformation.transform.GetChild(0).gameObject.SetActive(false);
			});
			this.btnVillage.GetComponent<PointerTrigger>().EnterEvent.ResetListener(delegate()
			{
				this.btnVillage.transform.GetChild(0).gameObject.SetActive(true);
			});
			this.btnVillage.GetComponent<PointerTrigger>().ExitEvent.ResetListener(delegate()
			{
				this.btnVillage.transform.GetChild(0).gameObject.SetActive(false);
			});
			this.jottings.Init(new Action(this.CloseSubpage));
			this.information.Init(new Action(this.CloseSubpage));
			this.village.Init(new Action<NotificationItem, ArgumentBox>(this.RenderMonthlyNotificationTips), new Action(this.CloseSubpage));
		}

		// Token: 0x06006B0D RID: 27405 RVA: 0x003178F0 File Offset: 0x00315AF0
		private void RefreshSimple()
		{
			this.btnLeft.enabled = false;
			this.btnRight.enabled = false;
			this.dateSelectorToggle.enabled = false;
			this._index = 0;
			this.RefreshNotificationItemData();
			this.RefreshMonthDesc();
			this.RefreshDateSelector();
			this.RefreshSectionItems();
			this.RefreshSubpages();
		}

		// Token: 0x06006B0E RID: 27406 RVA: 0x00317950 File Offset: 0x00315B50
		private void Refresh()
		{
			bool flag = this._data.MonthNotifies == null || this._data.MonthNotifies.Count == 0;
			if (flag)
			{
				GLog.Warn("There is no MonthNotify Data!");
			}
			else
			{
				this.RefreshHotkeyCommands();
				this.RefreshNotificationItemData();
				this.RefreshMonthPanel();
				this.RefreshMonthDesc();
				this.RefreshDateSelector();
				this.RefreshSectionItems();
				this.RefreshSubpages();
				this._refreshing = false;
			}
		}

		// Token: 0x06006B0F RID: 27407 RVA: 0x003179CB File Offset: 0x00315BCB
		private void RefreshHotkeyCommands()
		{
			this.prevMonthLabel.text = TabSwitchCommandKit.PrevTabLevel1.ToString();
			this.nextMonthLabel.text = TabSwitchCommandKit.NextTabLevel1.ToString();
		}

		// Token: 0x06006B10 RID: 27408 RVA: 0x003179FC File Offset: 0x00315BFC
		private void RefreshMonthSlider()
		{
			this.monthSlider.onValueChanged.RemoveAllListeners();
			this.monthSlider.maxValue = (float)(this._data.MonthNotifies.Count - 1);
			this.monthSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnMonthIndexChange));
			this.OnMonthIndexChange(0f);
		}

		// Token: 0x06006B11 RID: 27409 RVA: 0x00317A64 File Offset: 0x00315C64
		private void RefreshNotificationItemData()
		{
			this._showDataList.Clear();
			for (int i = 0; i < this._data.MonthNotifies.Count; i++)
			{
				this.GenerateMonthlyNotificationItem(this._data.MonthNotifies[i], this._data.Arguments[i]);
			}
		}

		// Token: 0x06006B12 RID: 27410 RVA: 0x00317AC8 File Offset: 0x00315CC8
		private void RefreshMonthDesc()
		{
			int month = CommonUtils.GetMonthByDate(this.Date);
			MonthlyNotificationItem config = MonthlyNotification.Instance[month];
			this.monthIcon.SetSprite(config.Icon, false, null);
			this.monthNameLabel.text = config.Name;
			this.monthDescLabel.text = config.Desc.Replace("\n", "");
			this.btnClose.interactable = !this._needSave;
			TooltipInvoker tooltipInvoker = this.monthDescTips;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			this.monthDescTips.RuntimeParam.Clear();
			this.monthDescTips.RuntimeParam.Set("TemplateId", config.TemplateId);
			this.monthDescTips.RuntimeParam.Set("Content", config.Desc);
			this.monthDescTips.Refresh(false, -1);
		}

		// Token: 0x06006B13 RID: 27411 RVA: 0x00317BC0 File Offset: 0x00315DC0
		private void RefreshDateSelector()
		{
			this.dateSelectorVersionLabel.text = LanguageKey.LK_MonthNotify_Version.TrFormat(this.Date - GlobalConfig.Instance.GameStartDate + 1);
			this.dateSelectorDateLabel.text = this.YearMonthDesc;
		}

		// Token: 0x06006B14 RID: 27412 RVA: 0x00317C10 File Offset: 0x00315E10
		private void RefreshSectionItems()
		{
			foreach (List<NotificationItem> list in this._classifiedData.Values)
			{
				list.Clear();
			}
			foreach (NotificationItem item in this._showDataList[this._index])
			{
				this._classifiedData[ViewMonthNotify.GetConfig(item).SectionType].Add(item);
			}
			this.review.Set(this._classifiedData);
		}

		// Token: 0x06006B15 RID: 27413 RVA: 0x00317CE4 File Offset: 0x00315EE4
		private void RefreshSubpages()
		{
			bool isFirstMonth = this.Date == GlobalConfig.Instance.GameStartDate;
			short currentAreaId = SingletonObject.getInstance<WorldMapModel>().CurrentAreaId;
			bool canUseVillage = currentAreaId != 135 && currentAreaId != 137;
			this.btnJottings.gameObject.SetActive(!isFirstMonth);
			this.btnInformation.gameObject.SetActive(!isFirstMonth);
			this.btnVillage.gameObject.SetActive(canUseVillage);
			NameAndLifeRelatedData nameAndLifeRelatedData = this.CurrentDisplayData.CharacterNames[this.CurrentDisplayData.TaiwuId];
			this.taiwuNameLabel.text = NameCenter.GetMonasticTitleOrDisplayName(ref nameAndLifeRelatedData.NameRelatedData, true, false);
			this.taiwuAvatar.Refresh(this.CurrentDisplayData.Avatars[this.CurrentDisplayData.TaiwuId], nameAndLifeRelatedData.NameRelatedData.CharTemplateId);
			this.villageCountLabel.text = this._classifiedData[EMonthlyNotificationSectionType.TaiwuVillage].Count.ToString();
			this.informationCountLabel.text = (this.CurrentDisplayData.Information.Count + this.CurrentDisplayData.SecretInformationSnapshots.Count).ToString();
		}

		// Token: 0x06006B16 RID: 27414 RVA: 0x00317E28 File Offset: 0x00316028
		private void RefreshMonthPanel()
		{
			Transform monthParent = this.monthToggleGroup.transform;
			this.monthSlider.SetValueWithoutNotify((float)this._index);
			this.monthToggleGroup.SetWithoutNotify(this._index);
			for (int i = 0; i < this._data.MonthNotifies.Count; i++)
			{
				int date = this._data.MonthNotifies[i].Date;
				int month = CommonUtils.GetMonthByDate(date);
				Transform obj = monthParent.GetChild(i);
				obj.GetChild(1).GetComponent<TextMeshProUGUI>().text = LanguageKey.UI_AdvanceMonth_TimeChangeInfo_Month.TrFormat(month + 1);
				obj.gameObject.SetActive(true);
			}
			for (int j = this._data.MonthNotifies.Count; j < monthParent.childCount; j++)
			{
				monthParent.GetChild(j).gameObject.SetActive(false);
			}
			this.btnLeft.enabled = true;
			this.btnRight.enabled = true;
			this.dateSelectorToggle.enabled = true;
			bool isMin = this._index == this._data.MonthNotifies.Count - 1;
			bool isMax = this._index == 0;
			this.btnLeft.interactable = !isMin;
			this.btnRight.interactable = !isMax;
			this.btnMin.interactable = !isMin;
			this.btnMax.interactable = !isMax;
		}

		// Token: 0x06006B17 RID: 27415 RVA: 0x00317FB4 File Offset: 0x003161B4
		private void OnQuit(ArgumentBox argBox)
		{
			bool flag = GameApp.Instance.GetCurrentGameStateName() == EGameState.Loading;
			if (flag)
			{
				ViewMonthNotify._lastDate = -1;
			}
		}

		// Token: 0x06006B18 RID: 27416 RVA: 0x00317FDC File Offset: 0x003161DC
		private void OnClickBtnLeft()
		{
			AudioManager.Instance.PlaySound("ui_default_click_left", false, false);
			bool flag = this._index == this._data.MonthNotifies.Count - 1;
			if (!flag)
			{
				this.OnMonthIndexChange((float)(this._index + 1));
			}
		}

		// Token: 0x06006B19 RID: 27417 RVA: 0x0031802C File Offset: 0x0031622C
		private void OnClickBtnRight()
		{
			AudioManager.Instance.PlaySound("ui_default_click_left", false, false);
			bool flag = this._index == 0;
			if (!flag)
			{
				this.OnMonthIndexChange((float)(this._index - 1));
			}
		}

		// Token: 0x06006B1A RID: 27418 RVA: 0x0031806B File Offset: 0x0031626B
		private void OnClickBtnMin()
		{
			AudioManager.Instance.PlaySound("ui_default_click_left", false, false);
			this.OnMonthIndexChange((float)(this._data.MonthNotifies.Count - 1));
		}

		// Token: 0x06006B1B RID: 27419 RVA: 0x0031809A File Offset: 0x0031629A
		private void OnClickBtnMax()
		{
			AudioManager.Instance.PlaySound("ui_default_click_left", false, false);
			this.OnMonthIndexChange(0f);
		}

		// Token: 0x06006B1C RID: 27420 RVA: 0x003180BB File Offset: 0x003162BB
		private void OnClickMonthPanelToggle(bool value)
		{
			this.monthPanel.SetActive(value);
		}

		// Token: 0x06006B1D RID: 27421 RVA: 0x003180CB File Offset: 0x003162CB
		private void OnMonthToggleGroupChange(int _, int __)
		{
			this.OnMonthIndexChange((float)this.monthToggleGroup.GetActiveIndex());
		}

		// Token: 0x06006B1E RID: 27422 RVA: 0x003180E1 File Offset: 0x003162E1
		private void OnMonthIndexChange(float value)
		{
			this._index = (int)value;
			this.Refresh();
		}

		// Token: 0x06006B1F RID: 27423 RVA: 0x003180F4 File Offset: 0x003162F4
		private void OnClickSettings()
		{
			bool flag = !SingletonObject.getInstance<BasicGameData>().SavingWorld;
			if (flag)
			{
				UIManager.Instance.MaskUI(UIElement.MonthlyNotificationSortingGroupSettings);
			}
		}

		// Token: 0x06006B20 RID: 27424 RVA: 0x00318128 File Offset: 0x00316328
		private void OnClickVillage()
		{
			bool flag;
			if (!SingletonObject.getInstance<BasicGameData>().SavingWorld)
			{
				MonthNotifyDisplayData data = this._data;
				flag = (data != null && data.MonthNotifies != null);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				this.subpageParent.SetActive(true);
				this.village.Set(this._data.MonthNotifies[this._index], this._classifiedData[EMonthlyNotificationSectionType.TaiwuVillage]);
				this.village.gameObject.SetActive(true);
			}
		}

		// Token: 0x06006B21 RID: 27425 RVA: 0x003181B0 File Offset: 0x003163B0
		private void OnClickJottings()
		{
			bool flag;
			if (!SingletonObject.getInstance<BasicGameData>().SavingWorld)
			{
				MonthNotifyDisplayData data = this._data;
				flag = (data != null && data.MonthNotifies != null);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				this.subpageParent.SetActive(true);
				this.jottings.Set(this._data.MonthNotifies[this._index], this._data.EatenItem);
				this.jottings.gameObject.SetActive(true);
			}
		}

		// Token: 0x06006B22 RID: 27426 RVA: 0x00318238 File Offset: 0x00316438
		private void OnClickInformation()
		{
			bool flag;
			if (!SingletonObject.getInstance<BasicGameData>().SavingWorld)
			{
				MonthNotifyDisplayData data = this._data;
				flag = (data != null && data.MonthNotifies != null);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				this.subpageParent.SetActive(true);
				this.information.Set(this._data.MonthNotifies[this._index], this._data.SecretInformationLocation);
				this.information.gameObject.SetActive(true);
			}
		}

		// Token: 0x06006B23 RID: 27427 RVA: 0x003182BD File Offset: 0x003164BD
		private void OnGroupSettingChange(ArgumentBox argBox)
		{
			this.RequestData();
		}

		// Token: 0x06006B24 RID: 27428 RVA: 0x003182C8 File Offset: 0x003164C8
		private void OnSavingWorldStateChange(ArgumentBox argBox)
		{
			bool saving = SingletonObject.getInstance<BasicGameData>().SavingWorld;
			this.mask.SetActive(saving);
			this.btnClose.interactable = !saving;
			bool flag = !saving;
			if (flag)
			{
				this._needSave = false;
				bool flag2 = UIManager.Instance.IsFocusElement(UIElement.MonthlyEvent);
				if (flag2)
				{
					UIElement.MonthlyEvent.UiBaseAs<UI_MonthlyEvent>().QuickHide();
				}
			}
		}

		// Token: 0x06006B25 RID: 27429 RVA: 0x00318334 File Offset: 0x00316534
		private void InitOpenAnim()
		{
			bool playedInitAnim = this._playedInitAnim;
			if (!playedInitAnim)
			{
				Sequence sequence = base.InitDefaultSequenceIn(false);
				CanvasGroup rootCanvas = this.animRoot.GetComponent<CanvasGroup>();
				base.AddAnimToDefaultSequenceIn(this.animRoot, sequence, 0.33f, 0.33f, -300f, false);
				sequence.AppendCallback(delegate
				{
					rootCanvas.alpha = 0f;
				});
				sequence.AppendCallback(delegate
				{
					rootCanvas.DOFade(1f, 0.66f);
				});
				sequence.AppendCallback(delegate
				{
					this.leftCanvasGroup.alpha = 0f;
				});
				sequence.AppendCallback(delegate
				{
					this.leftCanvasGroup.DOFade(1f, 0.66f);
				});
				sequence.AppendCallback(delegate
				{
					this.rightCanvasGroup.alpha = 0f;
				});
				sequence.AppendCallback(delegate
				{
					this.rightCanvasGroup.DOFade(1f, 0.66f);
				});
				sequence.Play<Sequence>();
				this._playedInitAnim = true;
			}
		}

		// Token: 0x06006B26 RID: 27430 RVA: 0x00318418 File Offset: 0x00316618
		private List<sbyte> GetNotificationMergeableParameters(short templateId)
		{
			return MonthlyNotification.Instance.GetItem(templateId).MergeableParameters;
		}

		// Token: 0x06006B27 RID: 27431 RVA: 0x0031843C File Offset: 0x0031663C
		private void GenerateMonthlyNotificationItem(MonthNotify monthNotify, ArgumentCollectionRenderArguments arguments)
		{
			RenderedArgumentCollection renderedNotificationArgumentCollection = new RenderedArgumentCollection();
			List<NotificationItem> showDataList = new List<NotificationItem>();
			int date = monthNotify.Date;
			this._renderInfoList.Clear();
			this._notificationArgumentCollection.Clear();
			monthNotify.MonthlyNotificationCollection.GetRenderInfos(this._renderInfoList, this._notificationArgumentCollection);
			GameMessageUtils.RenderDynamicArguments(arguments, this._notificationArgumentCollection, renderedNotificationArgumentCollection, false, false);
			GameMessageUtils.RenderFixedArguments(this._notificationArgumentCollection, renderedNotificationArgumentCollection, false);
			foreach (RenderInfo info in this._renderInfoList)
			{
				bool merged = false;
				MonthlyNotificationItem config = MonthlyNotification.Instance[info.RecordType];
				foreach (NotificationItem item in showDataList)
				{
					bool flag = item.TryMerge(date, info);
					if (flag)
					{
						merged = true;
						bool flag2 = config.MergeLimit >= 0 && item.RenderInfoList.Count - 1 == config.MergeLimit;
						if (flag2)
						{
							item.MergeDesc = config.MergeDesc;
							item.MergeType = config.MergeType;
						}
						break;
					}
				}
				bool flag3 = merged;
				if (!flag3)
				{
					NotificationItem newItem = new NotificationItem(date, info, new Func<short, List<sbyte>>(this.GetNotificationMergeableParameters));
					bool flag4 = config.ValueCheckParameters != null;
					if (flag4)
					{
						newItem.MergeableParameterValues = new Dictionary<int, string>();
						sbyte i = 0;
						while ((int)i < info.Arguments.Count)
						{
							bool flag5 = config.ValueCheckParameters.Contains(i);
							if (flag5)
							{
								newItem.MergeableParameterValues.Add((int)i, renderedNotificationArgumentCollection.Get(info.Arguments[(int)i].Item1, info.Arguments[(int)i].Item2));
							}
							i += 1;
						}
					}
					newItem.RenderedArgumentCollection = renderedNotificationArgumentCollection;
					showDataList.Add(newItem);
				}
			}
			foreach (NotificationItem item2 in showDataList)
			{
				List<int> charIdList = null;
				List<ValueTuple<sbyte, short>> itemList = null;
				foreach (RenderInfo renderInfo in item2.RenderInfoList)
				{
					foreach (ValueTuple<sbyte, int> valueTuple in renderInfo.Arguments)
					{
						sbyte argType = valueTuple.Item1;
						int argIndex = valueTuple.Item2;
						bool flag6 = argType == 0;
						if (flag6)
						{
							if (charIdList == null)
							{
								charIdList = new List<int>();
							}
							bool flag7 = !charIdList.Contains(this._notificationArgumentCollection.Characters[argIndex]);
							if (flag7)
							{
								charIdList.Add(this._notificationArgumentCollection.Characters[argIndex]);
							}
						}
						else
						{
							bool flag8 = argType == 2;
							if (flag8)
							{
								if (itemList == null)
								{
									itemList = new List<ValueTuple<sbyte, short>>();
								}
								bool flag9 = !itemList.Contains(this._notificationArgumentCollection.Items[argIndex]);
								if (flag9)
								{
									itemList.Add(this._notificationArgumentCollection.Items[argIndex]);
								}
							}
						}
					}
				}
				item2.CharIds = charIdList;
			}
			this._showDataList.Add(showDataList);
		}

		// Token: 0x04004DA3 RID: 19875
		[SerializeField]
		private CImage monthIcon;

		// Token: 0x04004DA4 RID: 19876
		[SerializeField]
		private TextMeshProUGUI monthNameLabel;

		// Token: 0x04004DA5 RID: 19877
		[SerializeField]
		private TextMeshProUGUI monthDescLabel;

		// Token: 0x04004DA6 RID: 19878
		[SerializeField]
		private TooltipInvoker monthDescTips;

		// Token: 0x04004DA7 RID: 19879
		[SerializeField]
		private CButton btnClose;

		// Token: 0x04004DA8 RID: 19880
		[SerializeField]
		private CButton btnLeft;

		// Token: 0x04004DA9 RID: 19881
		[SerializeField]
		private CButton btnRight;

		// Token: 0x04004DAA RID: 19882
		[SerializeField]
		private CButton btnMin;

		// Token: 0x04004DAB RID: 19883
		[SerializeField]
		private CButton btnMax;

		// Token: 0x04004DAC RID: 19884
		[SerializeField]
		private CToggle dateSelectorToggle;

		// Token: 0x04004DAD RID: 19885
		[SerializeField]
		private TextMeshProUGUI dateSelectorVersionLabel;

		// Token: 0x04004DAE RID: 19886
		[SerializeField]
		private TextMeshProUGUI dateSelectorDateLabel;

		// Token: 0x04004DAF RID: 19887
		[SerializeField]
		private GameObject monthPanel;

		// Token: 0x04004DB0 RID: 19888
		[SerializeField]
		private CSlider monthSlider;

		// Token: 0x04004DB1 RID: 19889
		[SerializeField]
		private CToggleGroup monthToggleGroup;

		// Token: 0x04004DB2 RID: 19890
		[SerializeField]
		private CButton btnGroupSetting;

		// Token: 0x04004DB3 RID: 19891
		[SerializeField]
		private GameObject mask;

		// Token: 0x04004DB4 RID: 19892
		[SerializeField]
		private TextMeshProUGUI prevMonthLabel;

		// Token: 0x04004DB5 RID: 19893
		[SerializeField]
		private TextMeshProUGUI nextMonthLabel;

		// Token: 0x04004DB6 RID: 19894
		[SerializeField]
		private MonthNotifySubPageReview review;

		// Token: 0x04004DB7 RID: 19895
		[SerializeField]
		private MonthNotifySubPageJottings jottings;

		// Token: 0x04004DB8 RID: 19896
		[SerializeField]
		private MonthNotifySubPageInformation information;

		// Token: 0x04004DB9 RID: 19897
		[SerializeField]
		private MonthNotifySubPageVillageJournal village;

		// Token: 0x04004DBA RID: 19898
		[SerializeField]
		private GameObject subpageParent;

		// Token: 0x04004DBB RID: 19899
		[SerializeField]
		private CButton btnJottings;

		// Token: 0x04004DBC RID: 19900
		[SerializeField]
		private CButton btnInformation;

		// Token: 0x04004DBD RID: 19901
		[SerializeField]
		private CButton btnVillage;

		// Token: 0x04004DBE RID: 19902
		[SerializeField]
		private Game.Components.Avatar.Avatar taiwuAvatar;

		// Token: 0x04004DBF RID: 19903
		[SerializeField]
		private TextMeshProUGUI taiwuNameLabel;

		// Token: 0x04004DC0 RID: 19904
		[SerializeField]
		private TextMeshProUGUI villageCountLabel;

		// Token: 0x04004DC1 RID: 19905
		[SerializeField]
		private TextMeshProUGUI informationCountLabel;

		// Token: 0x04004DC2 RID: 19906
		[SerializeField]
		private CanvasGroup leftCanvasGroup;

		// Token: 0x04004DC3 RID: 19907
		[SerializeField]
		private CanvasGroup rightCanvasGroup;

		// Token: 0x04004DC4 RID: 19908
		[SerializeField]
		private GameObject animRoot;

		// Token: 0x04004DC5 RID: 19909
		private bool _playedInitAnim;

		// Token: 0x04004DC6 RID: 19910
		private readonly Dictionary<HotKeyCommand, Action> _commonHotKeyCommands = new Dictionary<HotKeyCommand, Action>();

		// Token: 0x04004DC7 RID: 19911
		private MonthNotifyDisplayData _data;

		// Token: 0x04004DC8 RID: 19912
		private readonly List<MonthlyEventRenderInfo> _monthlyEventRenderInfoList = new List<MonthlyEventRenderInfo>();

		// Token: 0x04004DC9 RID: 19913
		private readonly ArgumentCollection _monthlyEventArgumentCollection = new ArgumentCollection();

		// Token: 0x04004DCA RID: 19914
		private int _index;

		// Token: 0x04004DCB RID: 19915
		private readonly List<List<NotificationItem>> _showDataList = new List<List<NotificationItem>>();

		// Token: 0x04004DCC RID: 19916
		private readonly List<RenderInfo> _renderInfoList = new List<RenderInfo>();

		// Token: 0x04004DCD RID: 19917
		private readonly ArgumentCollection _notificationArgumentCollection = new ArgumentCollection();

		// Token: 0x04004DCE RID: 19918
		private bool _needSave;

		// Token: 0x04004DCF RID: 19919
		private bool _needToEscape;

		// Token: 0x04004DD0 RID: 19920
		private static int _lastDate = -1;

		// Token: 0x04004DD1 RID: 19921
		private bool _refreshing;

		// Token: 0x04004DD2 RID: 19922
		private readonly Dictionary<EMonthlyNotificationSectionType, List<NotificationItem>> _classifiedData = new Dictionary<EMonthlyNotificationSectionType, List<NotificationItem>>();

		// Token: 0x04004DD3 RID: 19923
		private bool requestEventFlag = false;
	}
}
