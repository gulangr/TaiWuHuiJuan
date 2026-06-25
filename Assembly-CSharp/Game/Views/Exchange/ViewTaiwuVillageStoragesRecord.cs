using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
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

namespace Game.Views.Exchange
{
	// Token: 0x02000A35 RID: 2613
	public class ViewTaiwuVillageStoragesRecord : UIBase
	{
		// Token: 0x060080D4 RID: 32980 RVA: 0x003BED08 File Offset: 0x003BCF08
		public override void OnInit(ArgumentBox argsBox)
		{
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			box.Set("ShowBlackMask", true);
			box.Set("ShowWaitAnimation", true);
			box.Set("Message", LocalStringManager.Get(LanguageKey.LK_Waiting));
			UIElement.FullScreenMask.SetOnInitArgs(box);
			UIElement.FullScreenMask.Show();
			TaiwuVillageStorageType storageType;
			bool flag = argsBox.Get<TaiwuVillageStorageType>("StorageType", out storageType);
			if (flag)
			{
				this._storageType = storageType;
			}
			ViewTaiwuVillageStoragesRecord.TogKey togKey = this.GetTogKey(this._storageType);
			this.toggleGroup = base.CGet<CToggleGroup>("ToggleGroup");
			this.toggleGroup.Init(-1);
			this.toggleGroup.OnActiveIndexChange += this.OnActiveToggleChange;
			this.toggleGroup.SetWithoutNotify(togKey.ToInt());
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

		// Token: 0x060080D5 RID: 32981 RVA: 0x003BEFAC File Offset: 0x003BD1AC
		private void OnActiveToggleChange(int newTog, int oldTog)
		{
			this._storageType = this.GetStorageType((ViewTaiwuVillageStoragesRecord.TogKey)newTog);
			this.Refresh();
		}

		// Token: 0x060080D6 RID: 32982 RVA: 0x003BEFC3 File Offset: 0x003BD1C3
		private void LateUpdate()
		{
			base.CGet<LayoutElement>("ControlPart").preferredWidth = this._pointerRectTrans.anchoredPosition.x - 27f;
		}

		// Token: 0x060080D7 RID: 32983 RVA: 0x003BEFF0 File Offset: 0x003BD1F0
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = btnName == "Close";
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x060080D8 RID: 32984 RVA: 0x003BF020 File Offset: 0x003BD220
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

		// Token: 0x060080D9 RID: 32985 RVA: 0x003BF0D8 File Offset: 0x003BD2D8
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
				num = this._yearRecords.Min((ViewTaiwuVillageStoragesRecord.YearRecord y) => y.Year);
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

		// Token: 0x060080DA RID: 32986 RVA: 0x003BF25C File Offset: 0x003BD45C
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

		// Token: 0x060080DB RID: 32987 RVA: 0x003BF308 File Offset: 0x003BD508
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

		// Token: 0x060080DC RID: 32988 RVA: 0x003BF3F2 File Offset: 0x003BD5F2
		private void SetTopYearItemSelectState(Refers refers, bool selectState)
		{
			refers.CGet<CToggleObsolete>("Toggle").isOn = selectState;
			refers.CGet<CToggleObsolete>("Toggle").interactable = !selectState;
		}

		// Token: 0x060080DD RID: 32989 RVA: 0x003BF41C File Offset: 0x003BD61C
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

		// Token: 0x060080DE RID: 32990 RVA: 0x003BF4E4 File Offset: 0x003BD6E4
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

		// Token: 0x060080DF RID: 32991 RVA: 0x003BF574 File Offset: 0x003BD774
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

		// Token: 0x060080E0 RID: 32992 RVA: 0x003BF668 File Offset: 0x003BD868
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
					int yearIndex = this._yearRecords.FindIndex((ViewTaiwuVillageStoragesRecord.YearRecord y) => y.Year == year);
					bool flag2 = yearIndex >= 0;
					ViewTaiwuVillageStoragesRecord.YearRecord yearRecord;
					if (flag2)
					{
						yearRecord = this._yearRecords[yearIndex];
					}
					else
					{
						yearRecord = new ViewTaiwuVillageStoragesRecord.YearRecord();
						yearRecord.Year = year;
						this._yearRecords.Add(yearRecord);
					}
					sbyte month = timeManager.GetMonthInYear(renderInfo.Date);
					int monthIndex = yearRecord.MonthRecords.FindIndex((ViewTaiwuVillageStoragesRecord.MonthRecord y) => y.Month == (int)month);
					bool flag3 = monthIndex >= 0;
					ViewTaiwuVillageStoragesRecord.MonthRecord monthRecord;
					if (flag3)
					{
						monthRecord = yearRecord.MonthRecords[monthIndex];
					}
					else
					{
						monthRecord = new ViewTaiwuVillageStoragesRecord.MonthRecord();
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

		// Token: 0x060080E1 RID: 32993 RVA: 0x003BF828 File Offset: 0x003BDA28
		private void RefreshAsDetailView(List<ViewTaiwuVillageStoragesRecord.YearRecord> yearRecords)
		{
			ViewTaiwuVillageStoragesRecord.<>c__DisplayClass34_0 CS$<>8__locals1 = new ViewTaiwuVillageStoragesRecord.<>c__DisplayClass34_0();
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
				List<ViewTaiwuVillageStoragesRecord.MonthRecord> monthDataList = yearRecords[i].MonthRecords;
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

		// Token: 0x060080E2 RID: 32994 RVA: 0x003BFCF0 File Offset: 0x003BDEF0
		private void CollectPage(CanvasGroup page, List<Refers> dateLineList, List<Refers> contentLineList, List<CImage> splitLineList)
		{
			CImage[] splitLines = page.transform.GetComponentsInTopChildren(true);
			Refers[] refers = page.transform.GetComponentsInTopChildren(true);
			splitLineList.AddRange(splitLines);
			dateLineList.AddRange(refers.FindAll((Refers e) => e.UserInt == 0));
			contentLineList.AddRange(refers.FindAll((Refers e) => e.UserInt == 1));
		}

		// Token: 0x060080E3 RID: 32995 RVA: 0x003BFD7C File Offset: 0x003BDF7C
		private void OnCharacterNameClicked(int charId)
		{
			ArgumentBox args = new ArgumentBox();
			args.SetObject("TargetPageIndex", ECharacterSubToggleBase.StoryBase);
			GEvent.OnEvent(UiEvents.OnNeedOpenCharacterMenuSubPage, args);
			this.HandleCharacterDisplayData(charId);
		}

		// Token: 0x060080E4 RID: 32996 RVA: 0x003BFDBB File Offset: 0x003BDFBB
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

		// Token: 0x060080E5 RID: 32997 RVA: 0x003BFDF4 File Offset: 0x003BDFF4
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

		// Token: 0x060080E6 RID: 32998 RVA: 0x003BFE80 File Offset: 0x003BE080
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
				using (List<ViewTaiwuVillageStoragesRecord.YearRecord>.Enumerator enumerator = this._yearRecords.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ViewTaiwuVillageStoragesRecord.YearRecord yearRecord = enumerator.Current;
						using (List<ViewTaiwuVillageStoragesRecord.MonthRecord>.Enumerator enumerator2 = yearRecord.MonthRecords.GetEnumerator())
						{
							Predicate<ViewTaiwuVillageStoragesRecord.YearRecord> <>9__0;
							while (enumerator2.MoveNext())
							{
								ViewTaiwuVillageStoragesRecord.MonthRecord monthRecord = enumerator2.Current;
								Predicate<ViewTaiwuVillageStoragesRecord.MonthRecord> <>9__1;
								foreach (string record in monthRecord.Records)
								{
									bool flag3 = !record.Contains(inputValue);
									if (!flag3)
									{
										List<ViewTaiwuVillageStoragesRecord.YearRecord> selectedYearRecords = this._selectedYearRecords;
										Predicate<ViewTaiwuVillageStoragesRecord.YearRecord> match;
										if ((match = <>9__0) == null)
										{
											match = (<>9__0 = ((ViewTaiwuVillageStoragesRecord.YearRecord y) => y.Year == yearRecord.Year));
										}
										int yearIndex = selectedYearRecords.FindIndex(match);
										bool flag4 = yearIndex >= 0;
										ViewTaiwuVillageStoragesRecord.YearRecord newYearRecord;
										if (flag4)
										{
											newYearRecord = this._selectedYearRecords[yearIndex];
										}
										else
										{
											newYearRecord = new ViewTaiwuVillageStoragesRecord.YearRecord();
											this._selectedYearRecords.Add(newYearRecord);
										}
										newYearRecord.Year = yearRecord.Year;
										List<ViewTaiwuVillageStoragesRecord.MonthRecord> monthRecords = newYearRecord.MonthRecords;
										Predicate<ViewTaiwuVillageStoragesRecord.MonthRecord> match2;
										if ((match2 = <>9__1) == null)
										{
											match2 = (<>9__1 = ((ViewTaiwuVillageStoragesRecord.MonthRecord m) => m.Month == monthRecord.Month));
										}
										int monthIndex = monthRecords.FindIndex(match2);
										bool flag5 = monthIndex >= 0;
										ViewTaiwuVillageStoragesRecord.MonthRecord newMonthRecord;
										if (flag5)
										{
											newMonthRecord = newYearRecord.MonthRecords[monthIndex];
										}
										else
										{
											newMonthRecord = new ViewTaiwuVillageStoragesRecord.MonthRecord();
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

		// Token: 0x060080E7 RID: 32999 RVA: 0x003C0180 File Offset: 0x003BE380
		private TaiwuVillageStorageType GetStorageType(ViewTaiwuVillageStoragesRecord.TogKey togKey)
		{
			if (!true)
			{
			}
			TaiwuVillageStorageType result;
			switch (togKey)
			{
			case ViewTaiwuVillageStoragesRecord.TogKey.Warehouse:
				result = TaiwuVillageStorageType.Warehouse;
				break;
			case ViewTaiwuVillageStoragesRecord.TogKey.Treasury:
				result = TaiwuVillageStorageType.Treasury;
				break;
			case ViewTaiwuVillageStoragesRecord.TogKey.Stock:
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

		// Token: 0x060080E8 RID: 33000 RVA: 0x003C01C8 File Offset: 0x003BE3C8
		private ViewTaiwuVillageStoragesRecord.TogKey GetTogKey(TaiwuVillageStorageType storageType)
		{
			if (!true)
			{
			}
			ViewTaiwuVillageStoragesRecord.TogKey result;
			switch (storageType)
			{
			case TaiwuVillageStorageType.Warehouse:
				result = ViewTaiwuVillageStoragesRecord.TogKey.Warehouse;
				break;
			case TaiwuVillageStorageType.Treasury:
				result = ViewTaiwuVillageStoragesRecord.TogKey.Treasury;
				break;
			case TaiwuVillageStorageType.Stock:
				result = ViewTaiwuVillageStoragesRecord.TogKey.Stock;
				break;
			default:
				throw new ArgumentOutOfRangeException("storageType", storageType, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x060080E9 RID: 33001 RVA: 0x003C0214 File Offset: 0x003BE414
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

		// Token: 0x04006269 RID: 25193
		[SerializeField]
		private CToggleGroup toggleGroup;

		// Token: 0x0400626A RID: 25194
		private List<Vector2Int> _topYearDataList = new List<Vector2Int>();

		// Token: 0x0400626B RID: 25195
		private int _focusYear;

		// Token: 0x0400626C RID: 25196
		private List<CanvasGroup> _detailViewPageList;

		// Token: 0x0400626D RID: 25197
		private float _detailPageHeight;

		// Token: 0x0400626E RID: 25198
		private RectTransform _pointerRectTrans;

		// Token: 0x0400626F RID: 25199
		private PoolItem _charNameFullBtnPoolItem;

		// Token: 0x04006270 RID: 25200
		private PoolItem _charNameLeftPartBtnPoolItem;

		// Token: 0x04006271 RID: 25201
		private PoolItem _charNameRightPartBtnPoolItem;

		// Token: 0x04006272 RID: 25202
		private TaiwuVillageStoragesRecordCollection _taiwuVillageStoragesRecordCollection = new TaiwuVillageStoragesRecordCollection();

		// Token: 0x04006273 RID: 25203
		private ArgumentCollection _argumentCollection = new ArgumentCollection();

		// Token: 0x04006274 RID: 25204
		private RenderedArgumentCollection _renderedArgumentCollection = new RenderedArgumentCollection();

		// Token: 0x04006275 RID: 25205
		private List<TaiwuVillageStoragesRecordRenderInfo> _taiwuVillageStoragesRecordRenderInfos = new List<TaiwuVillageStoragesRecordRenderInfo>();

		// Token: 0x04006276 RID: 25206
		private readonly List<ViewTaiwuVillageStoragesRecord.YearRecord> _yearRecords = new List<ViewTaiwuVillageStoragesRecord.YearRecord>(1024);

		// Token: 0x04006277 RID: 25207
		private readonly List<ViewTaiwuVillageStoragesRecord.YearRecord> _selectedYearRecords = new List<ViewTaiwuVillageStoragesRecord.YearRecord>(1024);

		// Token: 0x04006278 RID: 25208
		private readonly List<int> _charIdList = new List<int>();

		// Token: 0x04006279 RID: 25209
		private TMP_InputField _searchName;

		// Token: 0x0400627A RID: 25210
		private TaiwuVillageStorageType _storageType = TaiwuVillageStorageType.Warehouse;

		// Token: 0x02001FD5 RID: 8149
		private class YearRecord
		{
			// Token: 0x0400CEE3 RID: 52963
			public int Year;

			// Token: 0x0400CEE4 RID: 52964
			public readonly List<ViewTaiwuVillageStoragesRecord.MonthRecord> MonthRecords = new List<ViewTaiwuVillageStoragesRecord.MonthRecord>();
		}

		// Token: 0x02001FD6 RID: 8150
		private class MonthRecord
		{
			// Token: 0x0400CEE5 RID: 52965
			public int Date;

			// Token: 0x0400CEE6 RID: 52966
			public int Month;

			// Token: 0x0400CEE7 RID: 52967
			public readonly List<string> Records = new List<string>();
		}

		// Token: 0x02001FD7 RID: 8151
		private enum TogKey
		{
			// Token: 0x0400CEE9 RID: 52969
			Warehouse,
			// Token: 0x0400CEEA RID: 52970
			Treasury,
			// Token: 0x0400CEEB RID: 52971
			Stock
		}
	}
}
