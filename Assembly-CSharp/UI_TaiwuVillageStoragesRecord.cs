using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using Game.Views.CharacterMenu;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.LifeRecord;
using GameData.Domains.LifeRecord.GeneralRecord;
using GameData.Domains.Organization.TaiwuVillageStoragesRecord;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001BF RID: 447
public class UI_TaiwuVillageStoragesRecord : UIBase
{
	// Token: 0x06001BBB RID: 7099 RVA: 0x000BF258 File Offset: 0x000BD458
	public override void OnInit(ArgumentBox argsBox)
	{
		ArgumentBox box = EasyPool.Get<ArgumentBox>();
		box.Set("ShowBlackMask", true);
		box.Set("ShowWaitAnimation", true);
		box.Set("Message", LocalStringManager.Get(LanguageKey.LK_Waiting));
		UIElement.FullScreenMask.SetOnInitArgs(box);
		UIElement.FullScreenMask.Show();
		TaiwuVillageStorageType storageType;
		bool flag = argsBox != null && argsBox.Get<TaiwuVillageStorageType>("StorageType", out storageType);
		if (flag)
		{
			this._storageType = storageType;
		}
		else
		{
			this._storageType = TaiwuVillageStorageType.Warehouse;
		}
		UI_TaiwuVillageStoragesRecord.TogKey togKey = this.GetTogKey(this._storageType);
		this._toggleGroup = base.CGet<CToggleGroupObsolete>("ToggleGroup");
		this._toggleGroup.InitPreOnToggle(-1);
		this._toggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnActiveToggleChange);
		this._toggleGroup.SetWithoutNotify(togKey.ToInt(), true);
		base.CGet<Refers>("DetailPanel").gameObject.SetActive(true);
		base.CGet<IdSwitch>("IDSwitchController").gameObject.SetActive(false);
		HorizontalPageSwitchController controller = base.CGet<HorizontalPageSwitchController>("PageSwitchController");
		controller.PageItemRefreshHandler = new Action<int, Refers>(this.RefreshTopYearItemOne);
		controller.SetItemSelectStateHandler = new Action<Refers, bool>(this.SetTopYearItemSelectState);
		controller.InitPageCount(0, 0, false);
		controller.RegisterOnSelectIndexChangeHandler(new Action<int>(this.OnSelectYearIndexChange));
		base.CGet<IdSwitch>("IDSwitchController").OnValueChanged = new Action<int>(this.OnDetailViewPageSwitch);
		Refers detailRefers = base.CGet<Refers>("DetailPanel");
		detailRefers.CGet<CanvasGroup>("PagePrefab").gameObject.SetActive(false);
		detailRefers.CGet<Refers>("DateLinePrefab").gameObject.SetActive(false);
		detailRefers.CGet<Refers>("ContentLine").gameObject.SetActive(false);
		detailRefers.CGet<RectTransform>("SplitLinePrefab").gameObject.SetActive(false);
		this._detailPageHeight = detailRefers.CGet<CanvasGroup>("PagePrefab").GetComponent<RectTransform>().rect.height;
		this._pointerRectTrans = base.CGet<PositionFollower>("Follower").GetComponent<RectTransform>();
		this._charNameFullBtnPoolItem = new PoolItem(string.Empty, base.CGet<GameObject>("FullCharNameButton"));
		this._charNameLeftPartBtnPoolItem = new PoolItem(string.Empty, base.CGet<GameObject>("LeftPartCharNameButton"));
		this._charNameRightPartBtnPoolItem = new PoolItem(string.Empty, base.CGet<GameObject>("RightPartCharNameButton"));
		this._focusYear = SingletonObject.getInstance<TimeManager>().GetYear();
		this.InitSearch();
		this.NeedDataListenerId = true;
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
		{
			TaiwuDomainMethod.Call.GetTaiwuVillageStoragesRecordCollection(this.Element.GameDataListenerId);
		}));
	}

	// Token: 0x06001BBC RID: 7100 RVA: 0x000BF50B File Offset: 0x000BD70B
	private void OnActiveToggleChange(CToggleObsolete newTog, CToggleObsolete oldTog)
	{
		this._storageType = this.GetStorageType((UI_TaiwuVillageStoragesRecord.TogKey)newTog.Key);
		this.Refresh();
	}

	// Token: 0x06001BBD RID: 7101 RVA: 0x000BF527 File Offset: 0x000BD727
	private void LateUpdate()
	{
		base.CGet<LayoutElement>("ControlPart").preferredWidth = this._pointerRectTrans.anchoredPosition.x - 27f;
	}

	// Token: 0x06001BBE RID: 7102 RVA: 0x000BF554 File Offset: 0x000BD754
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = btnName == "Close";
		if (flag)
		{
			this.QuickHide();
		}
	}

	// Token: 0x06001BBF RID: 7103 RVA: 0x000BF584 File Offset: 0x000BD784
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 1)
			{
				bool flag = notification.DomainId == 5;
				if (flag)
				{
					bool flag2 = notification.MethodId == 214;
					if (flag2)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._taiwuVillageStoragesRecordCollection);
						this.OnCollectionUpdated();
						this.Element.ShowAfterRefresh();
					}
				}
			}
		}
	}

	// Token: 0x06001BC0 RID: 7104 RVA: 0x000BF63C File Offset: 0x000BD83C
	private void ReInitTopPageList()
	{
		int yearCurrent = SingletonObject.getInstance<TimeManager>().GetYear();
		this._topYearDataList.Clear();
		bool isFiveYearMode = true;
		int unit = isFiveYearMode ? 5 : 1;
		int num;
		if (this._yearRecords.Count <= 0)
		{
			num = yearCurrent;
		}
		else
		{
			num = this._yearRecords.Min((UI_TaiwuVillageStoragesRecord.YearRecord y) => y.Year);
		}
		int minYear = num;
		int yearStart = Math.Max(1, minYear / unit * unit);
		int yearRangeA = yearStart;
		int initIndex = -1;
		for (int i = yearStart; i <= yearCurrent; i++)
		{
			bool flag = i == this._focusYear;
			if (flag)
			{
				initIndex = this._topYearDataList.Count;
			}
			bool flag2 = isFiveYearMode;
			if (flag2)
			{
				bool flag3 = (i % unit == 0 && i != yearStart) || i == yearCurrent;
				if (flag3)
				{
					Vector2Int vec = new Vector2Int(yearRangeA, i);
					this._topYearDataList.Add(vec);
					yearRangeA = i + 1;
				}
			}
			else
			{
				Vector2Int vec2 = new Vector2Int(i, i);
				this._topYearDataList.Add(vec2);
			}
		}
		Action<int, Refers> refreshHandler = new Action<int, Refers>(this.RefreshTopYearItemOne);
		bool flag4 = isFiveYearMode;
		if (flag4)
		{
			refreshHandler = new Action<int, Refers>(this.RefreshTopYearItemFive);
		}
		HorizontalPageSwitchController controller = base.CGet<HorizontalPageSwitchController>("PageSwitchController");
		controller.PageItemRefreshHandler = refreshHandler;
		controller.SetItemSelectStateHandler = new Action<Refers, bool>(this.SetTopYearItemSelectState);
		controller.InitPageCount(this._topYearDataList.Count, initIndex, false);
	}

	// Token: 0x06001BC1 RID: 7105 RVA: 0x000BF7C0 File Offset: 0x000BD9C0
	private void RefreshTopYearItemOne(int index, Refers refers)
	{
		TextMeshProUGUI textComponent = refers.CGet<TextMeshProUGUI>("LabelOff");
		textComponent.text = LocalStringManager.GetFormat(LanguageKey.UI_AdvanceMonth_TimeChangeInfo_Year, this._topYearDataList[index].x);
		refers.CGet<TextMeshProUGUI>("LabelOn").text = textComponent.text;
		refers.CGet<CToggleObsolete>("Toggle").onValueChanged.RemoveAllListeners();
		refers.CGet<CToggleObsolete>("Toggle").onValueChanged.AddListener(delegate(bool isOn)
		{
			if (isOn)
			{
				this._focusYear = this._topYearDataList[index].x;
				this.CGet<HorizontalPageSwitchController>("PageSwitchController").SetSelect(index, true);
			}
		});
	}

	// Token: 0x06001BC2 RID: 7106 RVA: 0x000BF86C File Offset: 0x000BDA6C
	private void RefreshTopYearItemFive(int index, Refers refers)
	{
		Vector2Int vec = this._topYearDataList[index];
		int yearBegin = vec.x;
		int yearEnd = vec.y;
		string yearRange = (yearBegin == yearEnd) ? yearBegin.ToString() : string.Format("{0}-{1}", yearBegin, yearEnd);
		string labelString = LocalStringManager.GetFormat(LanguageKey.UI_AdvanceMonth_TimeChangeInfo_Year, yearRange);
		TextMeshProUGUI textComponent = refers.CGet<TextMeshProUGUI>("LabelOff");
		textComponent.text = labelString;
		refers.CGet<TextMeshProUGUI>("LabelOn").text = labelString;
		refers.CGet<CToggleObsolete>("Toggle").onValueChanged.RemoveAllListeners();
		refers.CGet<CToggleObsolete>("Toggle").onValueChanged.AddListener(delegate(bool isOn)
		{
			if (isOn)
			{
				this._focusYear = yearBegin;
				this.CGet<HorizontalPageSwitchController>("PageSwitchController").SetSelect(index, true);
			}
		});
	}

	// Token: 0x06001BC3 RID: 7107 RVA: 0x000BF956 File Offset: 0x000BDB56
	private void SetTopYearItemSelectState(Refers refers, bool selectState)
	{
		refers.CGet<CToggleObsolete>("Toggle").isOn = selectState;
		refers.CGet<CToggleObsolete>("Toggle").interactable = !selectState;
	}

	// Token: 0x06001BC4 RID: 7108 RVA: 0x000BF980 File Offset: 0x000BDB80
	private void OnSelectYearIndexChange(int index)
	{
		this._searchName.SetTextWithoutNotify(string.Empty);
		Refers itemRefers = base.CGet<HorizontalPageSwitchController>("PageSwitchController").GetPageItem(index);
		bool flag = null != itemRefers;
		if (flag)
		{
			base.CGet<PositionFollower>("Follower").Target = itemRefers.transform;
		}
		int startYear = this._topYearDataList[index].x;
		int endYear = this._topYearDataList[index].y;
		this._selectedYearRecords.Clear();
		this._selectedYearRecords.AddRange(from y in this._yearRecords
		where y.Year >= startYear && y.Year <= endYear
		select y);
		this.RefreshAsDetailView(this._selectedYearRecords);
	}

	// Token: 0x06001BC5 RID: 7109 RVA: 0x000BFA48 File Offset: 0x000BDC48
	private void OnDetailViewPageSwitch(int value)
	{
		for (int i = 0; i < this._detailViewPageList.Count; i++)
		{
			CanvasGroup canvasGroup = this._detailViewPageList[i];
			canvasGroup.DOKill(false);
			bool flag = i + 1 == value;
			if (flag)
			{
				canvasGroup.DOFade(1f, 0.3f).SetAutoKill(true);
				canvasGroup.blocksRaycasts = true;
			}
			else
			{
				canvasGroup.DOFade(0f, 0.3f).SetAutoKill(true);
				canvasGroup.blocksRaycasts = false;
			}
		}
	}

	// Token: 0x06001BC6 RID: 7110 RVA: 0x000BFAD8 File Offset: 0x000BDCD8
	private void OnCollectionUpdated()
	{
		this._taiwuVillageStoragesRecordRenderInfos.Clear();
		this._argumentCollection.Clear();
		this._renderedArgumentCollection.Clear();
		bool flag = this._taiwuVillageStoragesRecordCollection == null || this._taiwuVillageStoragesRecordCollection.Count == 0;
		if (flag)
		{
			this._yearRecords.Clear();
			this.ReInitTopPageList();
		}
		else
		{
			this._taiwuVillageStoragesRecordCollection.GetRenderInfos(this._taiwuVillageStoragesRecordRenderInfos, this._argumentCollection);
			string key = "UI_TaiwuVillageStoragesRecord";
			this._charIdList.Clear();
			this._charIdList.AddRange(this._argumentCollection.Characters);
			this._charIdList.AddRange(this._argumentCollection.CharacterRealNames);
			RecordArgumentsRequest argRequest = new RecordArgumentsRequest(this._argumentCollection)
			{
				Characters = this._charIdList
			};
			LifeRecordDomainMethod.AsyncCall.GetRecordRenderInfoArguments(this, key, argRequest, delegate(int offset, RawDataPool dataPool)
			{
				ArgumentCollectionRenderArguments dynamicArguments = null;
				Serializer.Deserialize(dataPool, offset, ref dynamicArguments);
				GameMessageUtils.RenderDynamicArguments(dynamicArguments, this._argumentCollection, this._renderedArgumentCollection, true, true);
				GameMessageUtils.RenderFixedArguments(this._argumentCollection, this._renderedArgumentCollection, true);
				this.Refresh();
			});
		}
	}

	// Token: 0x06001BC7 RID: 7111 RVA: 0x000BFBCC File Offset: 0x000BDDCC
	private void Refresh()
	{
		TimeManager timeManager = SingletonObject.getInstance<TimeManager>();
		this._yearRecords.Clear();
		foreach (TaiwuVillageStoragesRecordRenderInfo renderInfo in this._taiwuVillageStoragesRecordRenderInfos)
		{
			bool flag = renderInfo.StorageType != this._storageType.ToSbyte();
			if (!flag)
			{
				int year = timeManager.GetYearByDate(renderInfo.Date);
				int yearIndex = this._yearRecords.FindIndex((UI_TaiwuVillageStoragesRecord.YearRecord y) => y.Year == year);
				bool flag2 = yearIndex >= 0;
				UI_TaiwuVillageStoragesRecord.YearRecord yearRecord;
				if (flag2)
				{
					yearRecord = this._yearRecords[yearIndex];
				}
				else
				{
					yearRecord = new UI_TaiwuVillageStoragesRecord.YearRecord();
					yearRecord.Year = year;
					this._yearRecords.Add(yearRecord);
				}
				sbyte month = timeManager.GetMonthInYear(renderInfo.Date);
				int monthIndex = yearRecord.MonthRecords.FindIndex((UI_TaiwuVillageStoragesRecord.MonthRecord y) => y.Month == (int)month);
				bool flag3 = monthIndex >= 0;
				UI_TaiwuVillageStoragesRecord.MonthRecord monthRecord;
				if (flag3)
				{
					monthRecord = yearRecord.MonthRecords[monthIndex];
				}
				else
				{
					monthRecord = new UI_TaiwuVillageStoragesRecord.MonthRecord();
					monthRecord.Month = (int)month;
					monthRecord.Date = renderInfo.Date;
					yearRecord.MonthRecords.Add(monthRecord);
				}
				string desc = renderInfo.GetText(this._renderedArgumentCollection).ColorReplace();
				monthRecord.Records.Add(desc);
			}
		}
		this.ReInitTopPageList();
	}

	// Token: 0x06001BC8 RID: 7112 RVA: 0x000BFD8C File Offset: 0x000BDF8C
	private void RefreshAsDetailView(List<UI_TaiwuVillageStoragesRecord.YearRecord> yearRecords)
	{
		UI_TaiwuVillageStoragesRecord.<>c__DisplayClass34_0 CS$<>8__locals1 = new UI_TaiwuVillageStoragesRecord.<>c__DisplayClass34_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.detailRefers = base.CGet<Refers>("DetailPanel");
		CanvasGroup canvasGroup = CS$<>8__locals1.detailRefers.CGet<CanvasGroup>("DetailPanel");
		canvasGroup.alpha = 0f;
		CS$<>8__locals1.detailRefers.gameObject.SetActive(true);
		CS$<>8__locals1.pageRoot = CS$<>8__locals1.detailRefers.CGet<RectTransform>("ContentRoot");
		bool flag = this._detailViewPageList == null;
		if (flag)
		{
			this._detailViewPageList = new List<CanvasGroup>();
		}
		CS$<>8__locals1.dateLineCache = new List<Refers>();
		CS$<>8__locals1.contentLineCache = new List<Refers>();
		CS$<>8__locals1.splitLineCache = new List<CImage>();
		CS$<>8__locals1.pageCache = new List<CanvasGroup>(CS$<>8__locals1.pageRoot.GetComponentsInTopChildren(true));
		CS$<>8__locals1.pageCache.ForEach(delegate(CanvasGroup e)
		{
			CS$<>8__locals1.<>4__this.CollectPage(e, CS$<>8__locals1.dateLineCache, CS$<>8__locals1.contentLineCache, CS$<>8__locals1.splitLineCache);
		});
		this._detailViewPageList.Clear();
		CS$<>8__locals1.curPage = CS$<>8__locals1.<RefreshAsDetailView>g__GetPage|1();
		this._detailViewPageList.Add(CS$<>8__locals1.curPage);
		CS$<>8__locals1.pageContentHeight = 0f;
		List<RectTransform> lineCache = new List<RectTransform>();
		bool hasRecord = false;
		for (int i = 0; i < yearRecords.Count; i++)
		{
			List<UI_TaiwuVillageStoragesRecord.MonthRecord> monthDataList = yearRecords[i].MonthRecords;
			bool flag2 = monthDataList == null || monthDataList.Count <= 0;
			if (!flag2)
			{
				for (int j = 0; j < monthDataList.Count; j++)
				{
					List<string> recordList = monthDataList[j].Records;
					bool flag3 = recordList == null || recordList.Count <= 0;
					if (!flag3)
					{
						bool flag4 = i + j > 0;
						if (flag4)
						{
							RectTransform splitLine = CS$<>8__locals1.<RefreshAsDetailView>g__GetSplitLine|4(CS$<>8__locals1.curPage.transform);
							lineCache.Add(splitLine);
						}
						int date = monthDataList[j].Date;
						Refers dateLine = CS$<>8__locals1.<RefreshAsDetailView>g__GetDateLine|2(CS$<>8__locals1.curPage.transform);
						MonthItem monthConfig = Month.Instance.GetItem(SingletonObject.getInstance<TimeManager>().GetMonthInYear(date));
						string dateDesc = SingletonObject.getInstance<TimeManager>().GetDateDisplayContent(date);
						dateLine.CGet<TextMeshProUGUI>("Date").text = dateDesc + " " + monthConfig.Name;
						lineCache.Add(dateLine.GetComponent<RectTransform>());
						for (int k = 0; k < recordList.Count; k++)
						{
							string content = recordList[k];
							Refers contentLine = CS$<>8__locals1.<RefreshAsDetailView>g__GetContentLine|3(CS$<>8__locals1.curPage.transform);
							bool flag5 = contentLine.UserObject == null;
							CharacterNameClickLinkHandler handler;
							if (flag5)
							{
								RectTransform btnRoot = contentLine.CGet<RectTransform>("ButtonRoot");
								handler = new CharacterNameClickLinkHandler(btnRoot, this._charNameFullBtnPoolItem, this._charNameLeftPartBtnPoolItem, this._charNameRightPartBtnPoolItem, new Action<int>(this.OnCharacterNameClicked));
								contentLine.UserObject = handler;
							}
							else
							{
								handler = (contentLine.UserObject as CharacterNameClickLinkHandler);
							}
							TMPTextSpriteHelper spriteHelper = contentLine.CGet<TMPTextSpriteHelper>("SpriteHelper");
							TextMeshProUGUI label = contentLine.CGet<TextMeshProUGUI>("Content");
							label.text = content;
							if (handler != null)
							{
								handler.ProcessLinkInfo(label, true);
							}
							spriteHelper.Parse();
							RectTransform lineRect = contentLine.GetComponent<RectTransform>();
							lineRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, label.GetPreferredValues().y + 7f);
							lineCache.Add(lineRect);
							CS$<>8__locals1.<RefreshAsDetailView>g__AddLines|5(lineCache);
							hasRecord = true;
						}
					}
				}
			}
		}
		CS$<>8__locals1.dateLineCache.ForEach(delegate(Refers e)
		{
			e.gameObject.SetActive(false);
		});
		CS$<>8__locals1.contentLineCache.ForEach(delegate(Refers e)
		{
			e.gameObject.SetActive(false);
		});
		CS$<>8__locals1.splitLineCache.ForEach(delegate(CImage e)
		{
			e.gameObject.SetActive(false);
		});
		CS$<>8__locals1.pageCache.ForEach(delegate(CanvasGroup e)
		{
			e.gameObject.SetActive(false);
		});
		base.CGet<IdSwitch>("IDSwitchController").Init(this._detailViewPageList.Count, this._detailViewPageList.Count, 1);
		this.OnDetailViewPageSwitch(this._detailViewPageList.Count);
		base.CGet<IdSwitch>("IDSwitchController").gameObject.SetActive(this._detailViewPageList.Count > 1);
		CS$<>8__locals1.detailRefers.CGet<GameObject>("NoContent").SetActive(!hasRecord);
		canvasGroup.DOFade(1f, 0.5f);
		UIElement.FullScreenMask.Hide(false);
	}

	// Token: 0x06001BC9 RID: 7113 RVA: 0x000C0254 File Offset: 0x000BE454
	private void CollectPage(CanvasGroup page, List<Refers> dateLineList, List<Refers> contentLineList, List<CImage> splitLineList)
	{
		CImage[] splitLines = page.transform.GetComponentsInTopChildren(true);
		Refers[] refers = page.transform.GetComponentsInTopChildren(true);
		splitLineList.AddRange(splitLines);
		dateLineList.AddRange(refers.FindAll((Refers e) => e.UserInt == 0));
		contentLineList.AddRange(refers.FindAll((Refers e) => e.UserInt == 1));
	}

	// Token: 0x06001BCA RID: 7114 RVA: 0x000C02E0 File Offset: 0x000BE4E0
	private void OnCharacterNameClicked(int charId)
	{
		ArgumentBox args = new ArgumentBox();
		args.SetObject("TargetPageIndex", ECharacterSubToggleBase.StoryBase);
		GEvent.OnEvent(UiEvents.OnNeedOpenCharacterMenuSubPage, args);
		this.HandleCharacterDisplayData(charId);
	}

	// Token: 0x06001BCB RID: 7115 RVA: 0x000C031F File Offset: 0x000BE51F
	private void HandleCharacterDisplayData(int charId)
	{
		List<int> list = new List<int>();
		list.Add(charId);
		CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataListForRelations(this, list, delegate(int offset, RawDataPool dataPool)
		{
			List<CharacterDisplayDataForRelations> charData = new List<CharacterDisplayDataForRelations>();
			Serializer.Deserialize(dataPool, offset, ref charData);
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharacterId", charData[0].CharacterId);
			argBox.SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.RelationshipBase, ECharacterSubPage.None));
			UIElement.CharacterMenu.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
			ViewCharacterMenu characterMenu = UIElement.CharacterMenu.UiBaseAs<ViewCharacterMenu>();
			characterMenu.GotoCharacterWithoutStackView(charData[0], null);
		});
	}

	// Token: 0x06001BCC RID: 7116 RVA: 0x000C0358 File Offset: 0x000BE558
	private void InitSearch()
	{
		this._searchName = base.CGet<TMP_InputField>("SearchByName");
		this._searchName.onValueChanged.RemoveAllListeners();
		this._searchName.onEndEdit.RemoveAllListeners();
		this._searchName.text = string.Empty;
		this._searchName.onValueChanged.AddListener(delegate(string inputValue)
		{
			this.UpdateDisplayDataBySearch(this._searchName);
		});
		this._searchName.onEndEdit.AddListener(delegate(string inputValue)
		{
			this.UpdateDisplayDataBySearch(this._searchName);
		});
	}

	// Token: 0x06001BCD RID: 7117 RVA: 0x000C03E4 File Offset: 0x000BE5E4
	private void UpdateDisplayDataBySearch(TMP_InputField searchName)
	{
		string inputValue = searchName.text;
		CommonUtils.FixToShowAbleString(ref inputValue, searchName.textComponent.font);
		inputValue = inputValue.Replace(" ", string.Empty);
		bool flag = !string.IsNullOrEmpty(inputValue);
		if (flag)
		{
			inputValue = inputValue.Substring(0, Mathf.Min(inputValue.Length, searchName.characterLimit - 1));
		}
		searchName.SetTextWithoutNotify(inputValue);
		bool flag2 = inputValue.IsNullOrEmpty();
		if (flag2)
		{
			HorizontalPageSwitchController controller = base.CGet<HorizontalPageSwitchController>("PageSwitchController");
			controller.SetSelect(controller.CurPageIndex, true);
		}
		else
		{
			this._selectedYearRecords.Clear();
			using (List<UI_TaiwuVillageStoragesRecord.YearRecord>.Enumerator enumerator = this._yearRecords.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					UI_TaiwuVillageStoragesRecord.YearRecord yearRecord = enumerator.Current;
					using (List<UI_TaiwuVillageStoragesRecord.MonthRecord>.Enumerator enumerator2 = yearRecord.MonthRecords.GetEnumerator())
					{
						Predicate<UI_TaiwuVillageStoragesRecord.YearRecord> <>9__0;
						while (enumerator2.MoveNext())
						{
							UI_TaiwuVillageStoragesRecord.MonthRecord monthRecord = enumerator2.Current;
							Predicate<UI_TaiwuVillageStoragesRecord.MonthRecord> <>9__1;
							foreach (string record in monthRecord.Records)
							{
								bool flag3 = !record.Contains(inputValue);
								if (!flag3)
								{
									List<UI_TaiwuVillageStoragesRecord.YearRecord> selectedYearRecords = this._selectedYearRecords;
									Predicate<UI_TaiwuVillageStoragesRecord.YearRecord> match;
									if ((match = <>9__0) == null)
									{
										match = (<>9__0 = ((UI_TaiwuVillageStoragesRecord.YearRecord y) => y.Year == yearRecord.Year));
									}
									int yearIndex = selectedYearRecords.FindIndex(match);
									bool flag4 = yearIndex >= 0;
									UI_TaiwuVillageStoragesRecord.YearRecord newYearRecord;
									if (flag4)
									{
										newYearRecord = this._selectedYearRecords[yearIndex];
									}
									else
									{
										newYearRecord = new UI_TaiwuVillageStoragesRecord.YearRecord();
										this._selectedYearRecords.Add(newYearRecord);
									}
									newYearRecord.Year = yearRecord.Year;
									List<UI_TaiwuVillageStoragesRecord.MonthRecord> monthRecords = newYearRecord.MonthRecords;
									Predicate<UI_TaiwuVillageStoragesRecord.MonthRecord> match2;
									if ((match2 = <>9__1) == null)
									{
										match2 = (<>9__1 = ((UI_TaiwuVillageStoragesRecord.MonthRecord m) => m.Month == monthRecord.Month));
									}
									int monthIndex = monthRecords.FindIndex(match2);
									bool flag5 = monthIndex >= 0;
									UI_TaiwuVillageStoragesRecord.MonthRecord newMonthRecord;
									if (flag5)
									{
										newMonthRecord = newYearRecord.MonthRecords[monthIndex];
									}
									else
									{
										newMonthRecord = new UI_TaiwuVillageStoragesRecord.MonthRecord();
										newYearRecord.MonthRecords.Add(newMonthRecord);
									}
									newMonthRecord.Month = monthRecord.Month;
									newMonthRecord.Date = monthRecord.Date;
									newMonthRecord.Records.Add(record);
								}
							}
						}
					}
				}
			}
			this.RefreshAsDetailView(this._selectedYearRecords);
		}
	}

	// Token: 0x06001BCE RID: 7118 RVA: 0x000C06E4 File Offset: 0x000BE8E4
	private TaiwuVillageStorageType GetStorageType(UI_TaiwuVillageStoragesRecord.TogKey togKey)
	{
		if (!true)
		{
		}
		TaiwuVillageStorageType result;
		switch (togKey)
		{
		case UI_TaiwuVillageStoragesRecord.TogKey.Warehouse:
			result = TaiwuVillageStorageType.Warehouse;
			break;
		case UI_TaiwuVillageStoragesRecord.TogKey.Treasury:
			result = TaiwuVillageStorageType.Treasury;
			break;
		case UI_TaiwuVillageStoragesRecord.TogKey.Stock:
			result = TaiwuVillageStorageType.Stock;
			break;
		default:
			throw new ArgumentOutOfRangeException("togKey", togKey, null);
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06001BCF RID: 7119 RVA: 0x000C072C File Offset: 0x000BE92C
	private UI_TaiwuVillageStoragesRecord.TogKey GetTogKey(TaiwuVillageStorageType storageType)
	{
		if (!true)
		{
		}
		UI_TaiwuVillageStoragesRecord.TogKey result;
		switch (storageType)
		{
		case TaiwuVillageStorageType.Warehouse:
			result = UI_TaiwuVillageStoragesRecord.TogKey.Warehouse;
			break;
		case TaiwuVillageStorageType.Treasury:
			result = UI_TaiwuVillageStoragesRecord.TogKey.Treasury;
			break;
		case TaiwuVillageStorageType.Stock:
			result = UI_TaiwuVillageStoragesRecord.TogKey.Stock;
			break;
		default:
			throw new ArgumentOutOfRangeException("storageType", storageType, null);
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06001BD0 RID: 7120 RVA: 0x000C0778 File Offset: 0x000BE978
	public static TaiwuVillageStorageType GetStorageType(ItemSourceType sourceType)
	{
		if (!true)
		{
		}
		TaiwuVillageStorageType result;
		switch (sourceType)
		{
		case ItemSourceType.Warehouse:
			result = TaiwuVillageStorageType.Warehouse;
			goto IL_3C;
		case ItemSourceType.Treasury:
			result = TaiwuVillageStorageType.Treasury;
			goto IL_3C;
		case ItemSourceType.Stock:
			result = TaiwuVillageStorageType.Stock;
			goto IL_3C;
		}
		throw new ArgumentOutOfRangeException("sourceType", sourceType, null);
		IL_3C:
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x040015A3 RID: 5539
	private List<Vector2Int> _topYearDataList = new List<Vector2Int>();

	// Token: 0x040015A4 RID: 5540
	private int _focusYear;

	// Token: 0x040015A5 RID: 5541
	private List<CanvasGroup> _detailViewPageList;

	// Token: 0x040015A6 RID: 5542
	private float _detailPageHeight;

	// Token: 0x040015A7 RID: 5543
	private RectTransform _pointerRectTrans;

	// Token: 0x040015A8 RID: 5544
	private PoolItem _charNameFullBtnPoolItem;

	// Token: 0x040015A9 RID: 5545
	private PoolItem _charNameLeftPartBtnPoolItem;

	// Token: 0x040015AA RID: 5546
	private PoolItem _charNameRightPartBtnPoolItem;

	// Token: 0x040015AB RID: 5547
	private TaiwuVillageStoragesRecordCollection _taiwuVillageStoragesRecordCollection = new TaiwuVillageStoragesRecordCollection();

	// Token: 0x040015AC RID: 5548
	private ArgumentCollection _argumentCollection = new ArgumentCollection();

	// Token: 0x040015AD RID: 5549
	private RenderedArgumentCollection _renderedArgumentCollection = new RenderedArgumentCollection();

	// Token: 0x040015AE RID: 5550
	private List<TaiwuVillageStoragesRecordRenderInfo> _taiwuVillageStoragesRecordRenderInfos = new List<TaiwuVillageStoragesRecordRenderInfo>();

	// Token: 0x040015AF RID: 5551
	private readonly List<UI_TaiwuVillageStoragesRecord.YearRecord> _yearRecords = new List<UI_TaiwuVillageStoragesRecord.YearRecord>(1024);

	// Token: 0x040015B0 RID: 5552
	private readonly List<UI_TaiwuVillageStoragesRecord.YearRecord> _selectedYearRecords = new List<UI_TaiwuVillageStoragesRecord.YearRecord>(1024);

	// Token: 0x040015B1 RID: 5553
	private readonly List<int> _charIdList = new List<int>();

	// Token: 0x040015B2 RID: 5554
	private TMP_InputField _searchName;

	// Token: 0x040015B3 RID: 5555
	private CToggleGroupObsolete _toggleGroup;

	// Token: 0x040015B4 RID: 5556
	private TaiwuVillageStorageType _storageType = TaiwuVillageStorageType.Warehouse;

	// Token: 0x020013B7 RID: 5047
	private class YearRecord
	{
		// Token: 0x04009EB7 RID: 40631
		public int Year;

		// Token: 0x04009EB8 RID: 40632
		public readonly List<UI_TaiwuVillageStoragesRecord.MonthRecord> MonthRecords = new List<UI_TaiwuVillageStoragesRecord.MonthRecord>();
	}

	// Token: 0x020013B8 RID: 5048
	private class MonthRecord
	{
		// Token: 0x04009EB9 RID: 40633
		public int Date;

		// Token: 0x04009EBA RID: 40634
		public int Month;

		// Token: 0x04009EBB RID: 40635
		public readonly List<string> Records = new List<string>();
	}

	// Token: 0x020013B9 RID: 5049
	private enum TogKey
	{
		// Token: 0x04009EBD RID: 40637
		Warehouse,
		// Token: 0x04009EBE RID: 40638
		Treasury,
		// Token: 0x04009EBF RID: 40639
		Stock
	}
}
