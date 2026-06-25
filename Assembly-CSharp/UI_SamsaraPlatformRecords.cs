using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using Game.Views.CharacterMenu;
using GameData.Domains.Building;
using GameData.Domains.Building.SamsaraPlatformRecord;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.LifeRecord;
using GameData.Domains.LifeRecord.GeneralRecord;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001BD RID: 445
public class UI_SamsaraPlatformRecords : UIBase
{
	// Token: 0x06001B9B RID: 7067 RVA: 0x000BDB58 File Offset: 0x000BBD58
	public override void OnInit(ArgumentBox argsBox)
	{
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
			BuildingDomainMethod.Call.GetSamsaraPlatformRecord(this.Element.GameDataListenerId);
		}));
	}

	// Token: 0x06001B9C RID: 7068 RVA: 0x000BDD34 File Offset: 0x000BBF34
	private void LateUpdate()
	{
		base.CGet<LayoutElement>("ControlPart").preferredWidth = this._pointerRectTrans.anchoredPosition.x - 27f;
	}

	// Token: 0x06001B9D RID: 7069 RVA: 0x000BDD60 File Offset: 0x000BBF60
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = btnName == "Close";
		if (flag)
		{
			this.QuickHide();
		}
	}

	// Token: 0x06001B9E RID: 7070 RVA: 0x000BDD90 File Offset: 0x000BBF90
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 1)
			{
				bool flag = notification.DomainId == 9;
				if (flag)
				{
					bool flag2 = notification.MethodId == 125;
					if (flag2)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._samsaraPlatformRecordCollection);
						this.OnCollectionUpdated();
						this.Element.ShowAfterRefresh();
					}
				}
			}
		}
	}

	// Token: 0x06001B9F RID: 7071 RVA: 0x000BDE48 File Offset: 0x000BC048
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
			num = this._yearRecords.Min((UI_SamsaraPlatformRecords.YearRecord y) => y.Year);
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

	// Token: 0x06001BA0 RID: 7072 RVA: 0x000BDFCC File Offset: 0x000BC1CC
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

	// Token: 0x06001BA1 RID: 7073 RVA: 0x000BE078 File Offset: 0x000BC278
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

	// Token: 0x06001BA2 RID: 7074 RVA: 0x000BE162 File Offset: 0x000BC362
	private void SetTopYearItemSelectState(Refers refers, bool selectState)
	{
		refers.CGet<CToggleObsolete>("Toggle").isOn = selectState;
		refers.CGet<CToggleObsolete>("Toggle").interactable = !selectState;
	}

	// Token: 0x06001BA3 RID: 7075 RVA: 0x000BE18C File Offset: 0x000BC38C
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

	// Token: 0x06001BA4 RID: 7076 RVA: 0x000BE254 File Offset: 0x000BC454
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

	// Token: 0x06001BA5 RID: 7077 RVA: 0x000BE2E4 File Offset: 0x000BC4E4
	private void OnCollectionUpdated()
	{
		this._samsaraPlatformRecordRenderInfos.Clear();
		this._argumentCollection.Clear();
		this._renderedArgumentCollection.Clear();
		bool flag = this._samsaraPlatformRecordCollection == null || this._samsaraPlatformRecordCollection.Count == 0;
		if (flag)
		{
			this._yearRecords.Clear();
			this.ReInitTopPageList();
		}
		else
		{
			this._samsaraPlatformRecordCollection.GetRenderInfos(this._samsaraPlatformRecordRenderInfos, this._argumentCollection);
			string key = "UI_SamsaraPlatformRecords";
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
				GameMessageUtils.RenderFixedArguments(this._argumentCollection, this._renderedArgumentCollection, false);
				TimeManager timeManager = SingletonObject.getInstance<TimeManager>();
				this._yearRecords.Clear();
				foreach (SamsaraPlatformRecordRenderInfo renderInfo in this._samsaraPlatformRecordRenderInfos)
				{
					int year = timeManager.GetYearByDate(renderInfo.Date);
					int yearIndex = this._yearRecords.FindIndex((UI_SamsaraPlatformRecords.YearRecord y) => y.Year == year);
					bool flag2 = yearIndex >= 0;
					UI_SamsaraPlatformRecords.YearRecord yearRecord;
					if (flag2)
					{
						yearRecord = this._yearRecords[yearIndex];
					}
					else
					{
						yearRecord = new UI_SamsaraPlatformRecords.YearRecord();
						yearRecord.Year = year;
						this._yearRecords.Add(yearRecord);
					}
					sbyte month = timeManager.GetMonthInYear(renderInfo.Date);
					int monthIndex = yearRecord.MonthRecords.FindIndex((UI_SamsaraPlatformRecords.MonthRecord y) => y.Month == (int)month);
					bool flag3 = monthIndex >= 0;
					UI_SamsaraPlatformRecords.MonthRecord monthRecord;
					if (flag3)
					{
						monthRecord = yearRecord.MonthRecords[monthIndex];
					}
					else
					{
						monthRecord = new UI_SamsaraPlatformRecords.MonthRecord();
						monthRecord.Month = (int)month;
						monthRecord.Date = renderInfo.Date;
						yearRecord.MonthRecords.Add(monthRecord);
					}
					string desc = renderInfo.GetText(this._renderedArgumentCollection).ColorReplace();
					monthRecord.Records.Add(desc);
				}
				this.ReInitTopPageList();
			});
		}
	}

	// Token: 0x06001BA6 RID: 7078 RVA: 0x000BE3D8 File Offset: 0x000BC5D8
	private void RefreshAsDetailView(List<UI_SamsaraPlatformRecords.YearRecord> yearRecords)
	{
		UI_SamsaraPlatformRecords.<>c__DisplayClass29_0 CS$<>8__locals1 = new UI_SamsaraPlatformRecords.<>c__DisplayClass29_0();
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
			List<UI_SamsaraPlatformRecords.MonthRecord> monthDataList = yearRecords[i].MonthRecords;
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
	}

	// Token: 0x06001BA7 RID: 7079 RVA: 0x000BE894 File Offset: 0x000BCA94
	private void CollectPage(CanvasGroup page, List<Refers> dateLineList, List<Refers> contentLineList, List<CImage> splitLineList)
	{
		CImage[] splitLines = page.transform.GetComponentsInTopChildren(true);
		Refers[] refers = page.transform.GetComponentsInTopChildren(true);
		splitLineList.AddRange(splitLines);
		dateLineList.AddRange(refers.FindAll((Refers e) => e.UserInt == 0));
		contentLineList.AddRange(refers.FindAll((Refers e) => e.UserInt == 1));
	}

	// Token: 0x06001BA8 RID: 7080 RVA: 0x000BE920 File Offset: 0x000BCB20
	private void OnCharacterNameClicked(int charId)
	{
		ArgumentBox args = new ArgumentBox();
		args.SetObject("TargetPageIndex", ECharacterSubToggleBase.StoryBase);
		GEvent.OnEvent(UiEvents.OnNeedOpenCharacterMenuSubPage, args);
		this.HandleCharacterDisplayData(charId);
	}

	// Token: 0x06001BA9 RID: 7081 RVA: 0x000BE95F File Offset: 0x000BCB5F
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

	// Token: 0x06001BAA RID: 7082 RVA: 0x000BE998 File Offset: 0x000BCB98
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

	// Token: 0x06001BAB RID: 7083 RVA: 0x000BEA24 File Offset: 0x000BCC24
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
			using (List<UI_SamsaraPlatformRecords.YearRecord>.Enumerator enumerator = this._yearRecords.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					UI_SamsaraPlatformRecords.YearRecord yearRecord = enumerator.Current;
					using (List<UI_SamsaraPlatformRecords.MonthRecord>.Enumerator enumerator2 = yearRecord.MonthRecords.GetEnumerator())
					{
						Predicate<UI_SamsaraPlatformRecords.YearRecord> <>9__0;
						while (enumerator2.MoveNext())
						{
							UI_SamsaraPlatformRecords.MonthRecord monthRecord = enumerator2.Current;
							Predicate<UI_SamsaraPlatformRecords.MonthRecord> <>9__1;
							foreach (string record in monthRecord.Records)
							{
								bool flag3 = !record.Contains(inputValue);
								if (!flag3)
								{
									List<UI_SamsaraPlatformRecords.YearRecord> selectedYearRecords = this._selectedYearRecords;
									Predicate<UI_SamsaraPlatformRecords.YearRecord> match;
									if ((match = <>9__0) == null)
									{
										match = (<>9__0 = ((UI_SamsaraPlatformRecords.YearRecord y) => y.Year == yearRecord.Year));
									}
									int yearIndex = selectedYearRecords.FindIndex(match);
									bool flag4 = yearIndex >= 0;
									UI_SamsaraPlatformRecords.YearRecord newYearRecord;
									if (flag4)
									{
										newYearRecord = this._selectedYearRecords[yearIndex];
									}
									else
									{
										newYearRecord = new UI_SamsaraPlatformRecords.YearRecord();
										this._selectedYearRecords.Add(newYearRecord);
									}
									newYearRecord.Year = yearRecord.Year;
									List<UI_SamsaraPlatformRecords.MonthRecord> monthRecords = newYearRecord.MonthRecords;
									Predicate<UI_SamsaraPlatformRecords.MonthRecord> match2;
									if ((match2 = <>9__1) == null)
									{
										match2 = (<>9__1 = ((UI_SamsaraPlatformRecords.MonthRecord m) => m.Month == monthRecord.Month));
									}
									int monthIndex = monthRecords.FindIndex(match2);
									bool flag5 = monthIndex >= 0;
									UI_SamsaraPlatformRecords.MonthRecord newMonthRecord;
									if (flag5)
									{
										newMonthRecord = newYearRecord.MonthRecords[monthIndex];
									}
									else
									{
										newMonthRecord = new UI_SamsaraPlatformRecords.MonthRecord();
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

	// Token: 0x0400158F RID: 5519
	private List<Vector2Int> _topYearDataList = new List<Vector2Int>();

	// Token: 0x04001590 RID: 5520
	private int _focusYear;

	// Token: 0x04001591 RID: 5521
	private List<CanvasGroup> _detailViewPageList;

	// Token: 0x04001592 RID: 5522
	private float _detailPageHeight;

	// Token: 0x04001593 RID: 5523
	private RectTransform _pointerRectTrans;

	// Token: 0x04001594 RID: 5524
	private PoolItem _charNameFullBtnPoolItem;

	// Token: 0x04001595 RID: 5525
	private PoolItem _charNameLeftPartBtnPoolItem;

	// Token: 0x04001596 RID: 5526
	private PoolItem _charNameRightPartBtnPoolItem;

	// Token: 0x04001597 RID: 5527
	private SamsaraPlatformRecordCollection _samsaraPlatformRecordCollection = new SamsaraPlatformRecordCollection();

	// Token: 0x04001598 RID: 5528
	private ArgumentCollection _argumentCollection = new ArgumentCollection();

	// Token: 0x04001599 RID: 5529
	private RenderedArgumentCollection _renderedArgumentCollection = new RenderedArgumentCollection();

	// Token: 0x0400159A RID: 5530
	private List<SamsaraPlatformRecordRenderInfo> _samsaraPlatformRecordRenderInfos = new List<SamsaraPlatformRecordRenderInfo>();

	// Token: 0x0400159B RID: 5531
	private readonly List<UI_SamsaraPlatformRecords.YearRecord> _yearRecords = new List<UI_SamsaraPlatformRecords.YearRecord>(1024);

	// Token: 0x0400159C RID: 5532
	private readonly List<UI_SamsaraPlatformRecords.YearRecord> _selectedYearRecords = new List<UI_SamsaraPlatformRecords.YearRecord>(1024);

	// Token: 0x0400159D RID: 5533
	private readonly List<int> _charIdList = new List<int>();

	// Token: 0x0400159E RID: 5534
	private TMP_InputField _searchName;

	// Token: 0x020013A8 RID: 5032
	private class YearRecord
	{
		// Token: 0x04009E8A RID: 40586
		public int Year;

		// Token: 0x04009E8B RID: 40587
		public readonly List<UI_SamsaraPlatformRecords.MonthRecord> MonthRecords = new List<UI_SamsaraPlatformRecords.MonthRecord>();
	}

	// Token: 0x020013A9 RID: 5033
	private class MonthRecord
	{
		// Token: 0x04009E8C RID: 40588
		public int Date;

		// Token: 0x04009E8D RID: 40589
		public int Month;

		// Token: 0x04009E8E RID: 40590
		public readonly List<string> Records = new List<string>();
	}
}
