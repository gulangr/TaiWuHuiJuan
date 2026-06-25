using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using Game.Views.CharacterMenu;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.LifeRecord;
using GameData.Domains.LifeRecord.GeneralRecord;
using GameData.Domains.Organization;
using GameData.Domains.Organization.SettlementPrisonRecord;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020003A9 RID: 937
public class UI_SettlementPrisonRecords : UIBase
{
	// Token: 0x170005C8 RID: 1480
	// (get) Token: 0x0600385C RID: 14428 RVA: 0x001C6E00 File Offset: 0x001C5000
	private GroupedInfinityScroll GroupedScrollView
	{
		get
		{
			return base.CGet<GroupedInfinityScroll>("GroupedScrollView");
		}
	}

	// Token: 0x170005C9 RID: 1481
	// (get) Token: 0x0600385D RID: 14429 RVA: 0x001C6E0D File Offset: 0x001C500D
	private LifeRecordsDateSelector LifeRecordsDateSelector
	{
		get
		{
			return base.CGet<LifeRecordsDateSelector>("LifeRecordsDateSelector");
		}
	}

	// Token: 0x170005CA RID: 1482
	// (get) Token: 0x0600385E RID: 14430 RVA: 0x001C6E1A File Offset: 0x001C501A
	private LifeRecordsDateSelector LifeRecordsDateSelectorEN
	{
		get
		{
			return base.CGet<LifeRecordsDateSelector>("LifeRecordsDateSelectorEN");
		}
	}

	// Token: 0x0600385F RID: 14431 RVA: 0x001C6E28 File Offset: 0x001C5028
	public override void OnInit(ArgumentBox argsBox)
	{
		argsBox.Get("SettlementId", out this._settlementId);
		if (this._charNameFullBtnPoolItem == null)
		{
			this._charNameFullBtnPoolItem = new PoolItem(string.Empty, base.CGet<GameObject>("FullCharNameButton"));
		}
		if (this._charNameLeftPartBtnPoolItem == null)
		{
			this._charNameLeftPartBtnPoolItem = new PoolItem(string.Empty, base.CGet<GameObject>("LeftPartCharNameButton"));
		}
		if (this._charNameRightPartBtnPoolItem == null)
		{
			this._charNameRightPartBtnPoolItem = new PoolItem(string.Empty, base.CGet<GameObject>("RightPartCharNameButton"));
		}
		this._scrollDataList.Clear();
		this._scrollGroupList.Clear();
		this.GroupedScrollView.Init();
		this.GroupedScrollView.OnItemRender = new Action<int, int, Refers>(this.OnItemRender);
		this.GroupedScrollView.OnGroupTitleRender = new Action<int, Refers>(this.OnGroupTitleRender);
		this.GroupedScrollView.UpdateData(this._scrollDataList, true);
		this.LifeRecordsDateSelector.Init(new Action<int>(this.OnSelectDate));
		this.LifeRecordsDateSelectorEN.Init(new Action<int>(this.OnSelectDate));
		this.RefreshSelectorForLanguage(this.CurLanguageType);
		this.InitSearch();
		this.NeedDataListenerId = true;
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
		{
			OrganizationDomainMethod.Call.GetSettlementPrisonRecordCollection(this.Element.GameDataListenerId, this._settlementId);
		}));
	}

	// Token: 0x06003860 RID: 14432 RVA: 0x001C6F88 File Offset: 0x001C5188
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = btnName == "CommonButtonClose";
		if (flag)
		{
			this.QuickHide();
		}
	}

	// Token: 0x06003861 RID: 14433 RVA: 0x001C6FB8 File Offset: 0x001C51B8
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 1)
			{
				bool flag = notification.DomainId == 3;
				if (flag)
				{
					bool flag2 = notification.MethodId == 22;
					if (flag2)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._settlementPrisonRecordCollection);
						this.OnCollectionUpdated();
						this.Element.ShowAfterRefresh();
					}
				}
			}
		}
	}

	// Token: 0x06003862 RID: 14434 RVA: 0x001C706C File Offset: 0x001C526C
	private void OnGroupTitleRender(int groupIndex, Refers refers)
	{
		UI_SettlementPrisonRecords.RecordGroup group = this._scrollGroupList[groupIndex];
		refers.CGet<TextMeshProUGUI>("TitleName").text = group.GroupTitleText;
	}

	// Token: 0x06003863 RID: 14435 RVA: 0x001C70A0 File Offset: 0x001C52A0
	private void OnItemRender(int groupIndex, int dataIndex, Refers refers)
	{
		UI_SettlementPrisonRecords.RecordGroup group = this._scrollGroupList[groupIndex];
		string content = group.LifeRecordMonthData.RecordList[dataIndex].Item3;
		bool flag = refers.UserObject == null;
		CharacterNameClickLinkHandler handler;
		if (flag)
		{
			RectTransform btnRoot = refers.CGet<RectTransform>("ButtonRoot");
			handler = new CharacterNameClickLinkHandler(btnRoot, this._charNameFullBtnPoolItem, this._charNameLeftPartBtnPoolItem, this._charNameRightPartBtnPoolItem, new Action<int>(this.OnCharacterNameClicked));
			refers.UserObject = handler;
		}
		else
		{
			handler = (refers.UserObject as CharacterNameClickLinkHandler);
		}
		TMPTextSpriteHelper spriteHelper = refers.CGet<TMPTextSpriteHelper>("SpriteHelper");
		TextMeshProUGUI label = refers.CGet<TextMeshProUGUI>("Content");
		label.text = content;
		if (handler != null)
		{
			handler.ProcessLinkInfo(label, true);
		}
		spriteHelper.Parse();
	}

	// Token: 0x06003864 RID: 14436 RVA: 0x001C7164 File Offset: 0x001C5364
	private void OnCollectionUpdated()
	{
		this._settlementPrisonRecordRenderInfos.Clear();
		this._argumentCollection.Clear();
		this._renderedArgumentCollection.Clear();
		this._originYearDataList.Clear();
		this._filterYearDataList.Clear();
		bool flag = this._settlementPrisonRecordCollection == null || this._settlementPrisonRecordCollection.Count == 0;
		if (flag)
		{
			this.RefreshDetailView(true);
		}
		else
		{
			this._settlementPrisonRecordCollection.GetRenderInfos(this._settlementPrisonRecordRenderInfos, this._argumentCollection);
			string key = "UI_SettlementPrisonRecords";
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
				TimeManager timeManager = SingletonObject.getInstance<TimeManager>();
				foreach (SettlementPrisonRecordRenderInfo renderInfo in this._settlementPrisonRecordRenderInfos)
				{
					bool flag2 = renderInfo.SettlementId != this._settlementId;
					if (!flag2)
					{
						int year = timeManager.GetYearByDate(renderInfo.Date);
						int yearIndex = this._originYearDataList.FindIndex((LifeRecordsController.LifeRecordYearData y) => y.Year == year);
						bool flag3 = yearIndex >= 0;
						LifeRecordsController.LifeRecordYearData yearRecord;
						if (flag3)
						{
							yearRecord = this._originYearDataList[yearIndex];
						}
						else
						{
							yearRecord = new LifeRecordsController.LifeRecordYearData();
							yearRecord.Year = year;
							yearRecord.MonthRecordDataList = new List<LifeRecordsController.LifeRecordMonthData>();
							this._originYearDataList.Add(yearRecord);
						}
						sbyte month = timeManager.GetMonthInYear(renderInfo.Date);
						int monthIndex = yearRecord.MonthRecordDataList.FindIndex((LifeRecordsController.LifeRecordMonthData y) => y.Month == (int)month);
						bool flag4 = monthIndex >= 0;
						LifeRecordsController.LifeRecordMonthData monthRecord;
						if (flag4)
						{
							monthRecord = yearRecord.MonthRecordDataList[monthIndex];
						}
						else
						{
							monthRecord = new LifeRecordsController.LifeRecordMonthData();
							monthRecord.Month = (int)month;
							monthRecord.Date = renderInfo.Date;
							monthRecord.RecordList = new List<ValueTuple<int, string, string, short>>();
							yearRecord.MonthRecordDataList.Add(monthRecord);
						}
						string desc = renderInfo.GetText(this._renderedArgumentCollection).ColorReplace();
						monthRecord.RecordList.Add(new ValueTuple<int, string, string, short>(0, null, desc, 0));
					}
				}
				this._filterYearDataList.AddRange(this._originYearDataList);
				this.RefreshDetailView(true);
			});
		}
	}

	// Token: 0x06003865 RID: 14437 RVA: 0x001C7264 File Offset: 0x001C5464
	private void OnCharacterNameClicked(int charId)
	{
		ArgumentBox args = new ArgumentBox();
		args.SetObject("TargetPageIndex", ECharacterSubToggleBase.StoryBase);
		GEvent.OnEvent(UiEvents.OnNeedOpenCharacterMenuSubPage, args);
		this.HandleCharacterDisplayData(charId);
	}

	// Token: 0x06003866 RID: 14438 RVA: 0x001C72A3 File Offset: 0x001C54A3
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

	// Token: 0x06003867 RID: 14439 RVA: 0x001C72DC File Offset: 0x001C54DC
	private void InitSearch()
	{
		this._searchName = base.CGet<TMP_InputField>("SearchByNameRemake");
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

	// Token: 0x06003868 RID: 14440 RVA: 0x001C7368 File Offset: 0x001C5568
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
		this._filterYearDataList.Clear();
		bool flag2 = inputValue.IsNullOrEmpty();
		if (flag2)
		{
			this._filterYearDataList.AddRange(this._originYearDataList);
		}
		else
		{
			using (List<LifeRecordsController.LifeRecordYearData>.Enumerator enumerator = this._originYearDataList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					LifeRecordsController.LifeRecordYearData yearRecord = enumerator.Current;
					using (List<LifeRecordsController.LifeRecordMonthData>.Enumerator enumerator2 = yearRecord.MonthRecordDataList.GetEnumerator())
					{
						Predicate<LifeRecordsController.LifeRecordYearData> <>9__0;
						while (enumerator2.MoveNext())
						{
							LifeRecordsController.LifeRecordMonthData monthRecord = enumerator2.Current;
							Predicate<LifeRecordsController.LifeRecordMonthData> <>9__1;
							foreach (ValueTuple<int, string, string, short> record in monthRecord.RecordList)
							{
								bool flag3 = !record.Item3.Contains(inputValue);
								if (!flag3)
								{
									List<LifeRecordsController.LifeRecordYearData> filterYearDataList = this._filterYearDataList;
									Predicate<LifeRecordsController.LifeRecordYearData> match;
									if ((match = <>9__0) == null)
									{
										match = (<>9__0 = ((LifeRecordsController.LifeRecordYearData y) => y.Year == yearRecord.Year));
									}
									int yearIndex = filterYearDataList.FindIndex(match);
									bool flag4 = yearIndex >= 0;
									LifeRecordsController.LifeRecordYearData newYearRecord;
									if (flag4)
									{
										newYearRecord = this._filterYearDataList[yearIndex];
									}
									else
									{
										newYearRecord = new LifeRecordsController.LifeRecordYearData();
										newYearRecord.MonthRecordDataList = new List<LifeRecordsController.LifeRecordMonthData>();
										this._filterYearDataList.Add(newYearRecord);
									}
									newYearRecord.Year = yearRecord.Year;
									List<LifeRecordsController.LifeRecordMonthData> monthRecordDataList = newYearRecord.MonthRecordDataList;
									Predicate<LifeRecordsController.LifeRecordMonthData> match2;
									if ((match2 = <>9__1) == null)
									{
										match2 = (<>9__1 = ((LifeRecordsController.LifeRecordMonthData m) => m.Month == monthRecord.Month));
									}
									int monthIndex = monthRecordDataList.FindIndex(match2);
									bool flag5 = monthIndex >= 0;
									LifeRecordsController.LifeRecordMonthData newMonthRecord;
									if (flag5)
									{
										newMonthRecord = newYearRecord.MonthRecordDataList[monthIndex];
									}
									else
									{
										newMonthRecord = new LifeRecordsController.LifeRecordMonthData();
										newMonthRecord.RecordList = new List<ValueTuple<int, string, string, short>>();
										newYearRecord.MonthRecordDataList.Add(newMonthRecord);
									}
									newMonthRecord.Month = monthRecord.Month;
									newMonthRecord.Date = monthRecord.Date;
									newMonthRecord.RecordList.Add(record);
								}
							}
						}
					}
				}
			}
		}
		this.RefreshDetailView(false);
	}

	// Token: 0x06003869 RID: 14441 RVA: 0x001C7678 File Offset: 0x001C5878
	private void RefreshDetailView(bool init)
	{
		this.UpdateScrollGroupList(this._filterYearDataList);
		this.UpdateScrollDataList(this._scrollGroupList);
		this.GroupedScrollView.UpdateData(this._scrollDataList, false);
		bool hasRecord = this._scrollDataList.Count > 0;
		base.CGet<GameObject>("NoContent").SetActive(!hasRecord);
		bool flag = this.CurLanguageType == LocalStringManager.LanguageType.CN;
		if (flag)
		{
			this.LifeRecordsDateSelector.Refresh(this._filterYearDataList, init);
		}
		else
		{
			this.LifeRecordsDateSelectorEN.Refresh(this._filterYearDataList, init);
		}
	}

	// Token: 0x0600386A RID: 14442 RVA: 0x001C770C File Offset: 0x001C590C
	private void UpdateScrollDataList(List<UI_SettlementPrisonRecords.RecordGroup> itemGroups)
	{
		this._scrollDataList.Clear();
		foreach (UI_SettlementPrisonRecords.RecordGroup itemGroup in itemGroups)
		{
			GroupedInfinityScroll.GroupItem groupItem = new GroupedInfinityScroll.GroupItem(itemGroup.GroupId, itemGroup.LifeRecordMonthData.RecordList.Count);
			this._scrollDataList.Add(groupItem);
		}
	}

	// Token: 0x0600386B RID: 14443 RVA: 0x001C7790 File Offset: 0x001C5990
	private void UpdateScrollGroupList(List<LifeRecordsController.LifeRecordYearData> yearsRecordList)
	{
		this._scrollGroupList.Clear();
		int groupId = 0;
		foreach (LifeRecordsController.LifeRecordYearData year in yearsRecordList)
		{
			List<LifeRecordsController.LifeRecordMonthData> monthRecordDataList = year.MonthRecordDataList;
			bool flag = monthRecordDataList != null && monthRecordDataList.Count > 0;
			if (flag)
			{
				foreach (LifeRecordsController.LifeRecordMonthData month in year.MonthRecordDataList)
				{
					List<ValueTuple<int, string, string, short>> recordList = month.RecordList;
					bool flag2 = recordList != null && recordList.Count > 0;
					if (flag2)
					{
						UI_SettlementPrisonRecords.RecordGroup groupItem = new UI_SettlementPrisonRecords.RecordGroup(groupId++, month);
						this._scrollGroupList.Add(groupItem);
					}
				}
			}
		}
	}

	// Token: 0x0600386C RID: 14444 RVA: 0x001C7890 File Offset: 0x001C5A90
	private void OnSelectDate(int date)
	{
		int groupIndex = this._scrollGroupList.FindIndex((UI_SettlementPrisonRecords.RecordGroup g) => g.LifeRecordMonthData.Date == date);
		bool flag = groupIndex >= 0;
		if (flag)
		{
			this.GroupedScrollView.RefillCellsByGroupIndex(groupIndex);
		}
	}

	// Token: 0x0600386D RID: 14445 RVA: 0x001C78DB File Offset: 0x001C5ADB
	public override void OnLanguageChange(LocalStringManager.LanguageType languageType)
	{
		this.RefreshSelectorForLanguage(languageType);
		base.OnLanguageChange(languageType);
	}

	// Token: 0x0600386E RID: 14446 RVA: 0x001C78F0 File Offset: 0x001C5AF0
	private void RefreshSelectorForLanguage(LocalStringManager.LanguageType languageType)
	{
		bool lastIsCN = this.CurLanguageType == LocalStringManager.LanguageType.CN;
		bool curIsCN = languageType == LocalStringManager.LanguageType.CN;
		this.LifeRecordsDateSelector.gameObject.SetActive(curIsCN);
		this.LifeRecordsDateSelectorEN.gameObject.SetActive(!curIsCN);
		bool flag = lastIsCN && !curIsCN;
		if (flag)
		{
			this.LifeRecordsDateSelectorEN.Refresh(this.LifeRecordsDateSelector);
		}
		bool flag2 = !lastIsCN && curIsCN;
		if (flag2)
		{
			this.LifeRecordsDateSelector.Refresh(this.LifeRecordsDateSelectorEN);
		}
	}

	// Token: 0x040028D4 RID: 10452
	private PoolItem _charNameFullBtnPoolItem;

	// Token: 0x040028D5 RID: 10453
	private PoolItem _charNameLeftPartBtnPoolItem;

	// Token: 0x040028D6 RID: 10454
	private PoolItem _charNameRightPartBtnPoolItem;

	// Token: 0x040028D7 RID: 10455
	private SettlementPrisonRecordCollection _settlementPrisonRecordCollection = new SettlementPrisonRecordCollection();

	// Token: 0x040028D8 RID: 10456
	private ArgumentCollection _argumentCollection = new ArgumentCollection();

	// Token: 0x040028D9 RID: 10457
	private RenderedArgumentCollection _renderedArgumentCollection = new RenderedArgumentCollection();

	// Token: 0x040028DA RID: 10458
	private List<SettlementPrisonRecordRenderInfo> _settlementPrisonRecordRenderInfos = new List<SettlementPrisonRecordRenderInfo>();

	// Token: 0x040028DB RID: 10459
	private readonly List<LifeRecordsController.LifeRecordYearData> _originYearDataList = new List<LifeRecordsController.LifeRecordYearData>(1024);

	// Token: 0x040028DC RID: 10460
	private readonly List<LifeRecordsController.LifeRecordYearData> _filterYearDataList = new List<LifeRecordsController.LifeRecordYearData>(1024);

	// Token: 0x040028DD RID: 10461
	private readonly List<GroupedInfinityScroll.GroupItem> _scrollDataList = new List<GroupedInfinityScroll.GroupItem>();

	// Token: 0x040028DE RID: 10462
	private readonly List<UI_SettlementPrisonRecords.RecordGroup> _scrollGroupList = new List<UI_SettlementPrisonRecords.RecordGroup>();

	// Token: 0x040028DF RID: 10463
	private readonly List<int> _charIdList = new List<int>();

	// Token: 0x040028E0 RID: 10464
	private TMP_InputField _searchName;

	// Token: 0x040028E1 RID: 10465
	private short _settlementId;

	// Token: 0x0200180C RID: 6156
	private struct RecordGroup
	{
		// Token: 0x0600D5CE RID: 54734 RVA: 0x005BA6F0 File Offset: 0x005B88F0
		public RecordGroup(int groupId, LifeRecordsController.LifeRecordMonthData monthRecord)
		{
			this.GroupId = groupId;
			this.LifeRecordMonthData = monthRecord;
			int year = monthRecord.Date / 12;
			int month = monthRecord.Date % 12;
			this.GroupTitleText = LocalStringManager.GetFormat(LanguageKey.LK_Game_Time, new object[]
			{
				year + 1,
				month + 1,
				LocalStringManager.Get(string.Format("LK_Season_{0}", TimeKit.GetSeason(monthRecord.Date))),
				Month.Instance[month].Name
			});
		}

		// Token: 0x0400AD5A RID: 44378
		public readonly int GroupId;

		// Token: 0x0400AD5B RID: 44379
		public readonly string GroupTitleText;

		// Token: 0x0400AD5C RID: 44380
		public readonly LifeRecordsController.LifeRecordMonthData LifeRecordMonthData;
	}
}
