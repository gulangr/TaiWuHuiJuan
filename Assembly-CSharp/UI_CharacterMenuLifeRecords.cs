using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using Game.Views.CharacterMenu;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001D5 RID: 469
public class UI_CharacterMenuLifeRecords : UI_CharacterMenuSubPageBase
{
	// Token: 0x1700030B RID: 779
	// (get) Token: 0x06001E63 RID: 7779 RVA: 0x000DADFF File Offset: 0x000D8FFF
	public override LanguageKey TitleKey
	{
		get
		{
			return LanguageKey.LK_CharacterMenu_Title_LifeRecords;
		}
	}

	// Token: 0x06001E64 RID: 7780 RVA: 0x000DAE08 File Offset: 0x000D9008
	public override bool CheckState(ECharacterSubToggleBase curSubTogglePage, ECharacterSubPage curSubPage)
	{
		return curSubTogglePage == ECharacterSubToggleBase.StoryBase;
	}

	// Token: 0x1700030C RID: 780
	// (get) Token: 0x06001E65 RID: 7781 RVA: 0x000DAE1E File Offset: 0x000D901E
	private bool _currentSelectDreamBack
	{
		get
		{
			return base.CGet<CToggleGroupObsolete>("CrossArchiveTogGroup").gameObject.activeSelf && base.CGet<CToggleGroupObsolete>("CrossArchiveTogGroup").GetActive().Key == 0;
		}
	}

	// Token: 0x1700030D RID: 781
	// (get) Token: 0x06001E66 RID: 7782 RVA: 0x000DAE52 File Offset: 0x000D9052
	private Refers DetailPanel
	{
		get
		{
			return base.CGet<Refers>("DetailPanel");
		}
	}

	// Token: 0x1700030E RID: 782
	// (get) Token: 0x06001E67 RID: 7783 RVA: 0x000DAE5F File Offset: 0x000D905F
	private Refers SummaryPanel
	{
		get
		{
			return base.CGet<Refers>("SummaryPanel");
		}
	}

	// Token: 0x1700030F RID: 783
	// (get) Token: 0x06001E68 RID: 7784 RVA: 0x000DAE6C File Offset: 0x000D906C
	private GroupedInfinityScroll GroupedScrollView
	{
		get
		{
			return this.DetailPanel.CGet<GroupedInfinityScroll>("GroupedScrollView");
		}
	}

	// Token: 0x17000310 RID: 784
	// (get) Token: 0x06001E69 RID: 7785 RVA: 0x000DAE7E File Offset: 0x000D907E
	private LifeRecordsDateSelector LifeRecordsDateSelector
	{
		get
		{
			return base.CGet<LifeRecordsDateSelector>("LifeRecordsDateSelector");
		}
	}

	// Token: 0x17000311 RID: 785
	// (get) Token: 0x06001E6A RID: 7786 RVA: 0x000DAE8B File Offset: 0x000D908B
	private LifeRecordsDateSelector LifeRecordsDateSelectorEN
	{
		get
		{
			return base.CGet<LifeRecordsDateSelector>("LifeRecordsDateSelectorEN");
		}
	}

	// Token: 0x17000312 RID: 786
	// (get) Token: 0x06001E6B RID: 7787 RVA: 0x000DAE98 File Offset: 0x000D9098
	private CToggleObsolete ToggleGreat
	{
		get
		{
			return base.CGet<CToggleObsolete>("ToggleGreat");
		}
	}

	// Token: 0x17000313 RID: 787
	// (get) Token: 0x06001E6C RID: 7788 RVA: 0x000DAEA5 File Offset: 0x000D90A5
	private CButtonObsolete ButtonDisplaySetting
	{
		get
		{
			return base.CGet<CButtonObsolete>("ButtonDisplaySetting");
		}
	}

	// Token: 0x17000314 RID: 788
	// (get) Token: 0x06001E6D RID: 7789 RVA: 0x000DAEB2 File Offset: 0x000D90B2
	private CButtonObsolete ButtonResetSetting
	{
		get
		{
			return base.CGet<CButtonObsolete>("ButtonResetSetting");
		}
	}

	// Token: 0x17000315 RID: 789
	// (get) Token: 0x06001E6E RID: 7790 RVA: 0x000DAEBF File Offset: 0x000D90BF
	private GameObject DisplaySettings
	{
		get
		{
			return base.CGet<GameObject>("DisplaySettings");
		}
	}

	// Token: 0x17000316 RID: 790
	// (get) Token: 0x06001E6F RID: 7791 RVA: 0x000DAECC File Offset: 0x000D90CC
	private LifeRecordsDateSelector CurLifeRecordsDateSelector
	{
		get
		{
			return (this.CurLanguageType == LocalStringManager.LanguageType.CN) ? this.LifeRecordsDateSelector : this.LifeRecordsDateSelectorEN;
		}
	}

	// Token: 0x06001E70 RID: 7792 RVA: 0x000DAEE4 File Offset: 0x000D90E4
	public override void OnInit(ArgumentBox argsBox)
	{
		this._scrollDataList.Clear();
		this._scrollGroupList.Clear();
		CToggleGroupObsolete togGroup = base.CGet<CToggleGroupObsolete>("CrossArchiveTogGroup");
		togGroup.InitPreOnToggle(1);
		togGroup.OnActiveToggleChange = delegate(CToggleObsolete toggle, CToggleObsolete cToggle)
		{
			this.OnCurrentCharacterChange(base.CharacterMenu.CurCharacterId);
		};
		togGroup.gameObject.SetActive(false);
		ExtraDomainMethod.AsyncCall.IsCurrentTaiwuOverwrittenByDreamBack(this, delegate(int offset, RawDataPool dataPool)
		{
			Serializer.Deserialize(dataPool, offset, ref this._isCurrentTaiwuOverwrittenByDreamBack);
			bool flag = !this._isCurrentTaiwuOverwrittenByDreamBack;
			if (!flag)
			{
				this.ChangeView(this._isCurrentTaiwuOverwrittenByDreamBack && base.CharacterMenu.CurrentCharacterIsTaiwu);
				SingletonObject.getInstance<LifeRecordsController>().DreamBackInit();
			}
		});
	}

	// Token: 0x06001E71 RID: 7793 RVA: 0x000DAF50 File Offset: 0x000D9150
	private void Awake()
	{
		this.AwakeInit();
	}

	// Token: 0x06001E72 RID: 7794 RVA: 0x000DAF5C File Offset: 0x000D915C
	private new void OnDisable()
	{
		bool flag = !SingletonObject.IsDestroying;
		if (flag)
		{
			LifeRecordsController instance = SingletonObject.getInstance<LifeRecordsController>();
			if (instance != null)
			{
				instance.ClearAllCache();
			}
		}
		this.DisplaySettings.SetActive(false);
	}

	// Token: 0x06001E73 RID: 7795 RVA: 0x000DAF94 File Offset: 0x000D9194
	private void ChangeView(bool isDreamBack)
	{
		GameObject togGroupGameObject = base.CGet<CToggleGroupObsolete>("CrossArchiveTogGroup").gameObject;
		togGroupGameObject.SetActive(isDreamBack);
	}

	// Token: 0x06001E74 RID: 7796 RVA: 0x000DAFBC File Offset: 0x000D91BC
	private bool IsCurrentSelectDreamBack()
	{
		return this._currentSelectDreamBack && base.CharacterMenu.CurrentCharacterIsTaiwu;
	}

	// Token: 0x06001E75 RID: 7797 RVA: 0x000DAFE4 File Offset: 0x000D91E4
	public override void OnCurrentCharacterChange(int prevCharacterId)
	{
		this.StopRefreshFocus();
		bool flag = base.CharacterMenu.CurCharacterId < 0;
		if (!flag)
		{
			bool flag2 = !((CharacterMenuSubPageElement)this.Element).Visible;
			if (!flag2)
			{
				bool flag3 = !base.CharacterMenu.CurrentCharacterIsTaiwu;
				if (flag3)
				{
					base.CGet<CToggleGroupObsolete>("CrossArchiveTogGroup").Set(1, true, false);
				}
				this.ChangeView(this._isCurrentTaiwuOverwrittenByDreamBack && base.CharacterMenu.CurrentCharacterIsTaiwu);
				this._yearEnd = (this.IsCurrentSelectDreamBack() ? SingletonObject.getInstance<LifeRecordsController>().GetDreamBackStartYearAndEndYearData().Item2 : SingletonObject.getInstance<TimeManager>().GetYear());
				this._focusYear = this._yearEnd;
				bool flag4 = this.IsCurrentSelectDreamBack();
				if (flag4)
				{
					SingletonObject.getInstance<LifeRecordsController>().UpdateDreamBackYear();
					SingletonObject.getInstance<LifeRecordsController>().SetDreamBackShowingYear(this._focusYear);
				}
				else
				{
					SingletonObject.getInstance<LifeRecordsController>().SetShowingYear(this._focusYear, base.CharacterMenu.CurCharacterId, !this.IsCurrentSelectDreamBack());
				}
			}
		}
	}

	// Token: 0x06001E76 RID: 7798 RVA: 0x000DB0F8 File Offset: 0x000D92F8
	public override void OnSubpageVisible()
	{
		GEvent.Add(UiEvents.OnGetCharBirthDateByLifeRecordModel, new GEvent.Callback(this.RefreshTopYearPageList));
		GEvent.Add(UiEvents.OnCharacterLifeRecordYearReady, new GEvent.Callback(this.OnCharacterLifeRecordYearDataReady));
		this._summaryIsSimple = true;
		this._onlyShowGreat = false;
		this.ToggleGreat.isOn = false;
		this.RefreshPanelSize();
		this._yearEnd = (this.IsCurrentSelectDreamBack() ? SingletonObject.getInstance<LifeRecordsController>().GetDreamBackStartYearAndEndYearData().Item2 : SingletonObject.getInstance<TimeManager>().GetYear());
		this.RefreshYearList();
		this._focusYear = (this._yearStart = this._yearEnd);
		bool flag = this.IsCurrentSelectDreamBack();
		if (flag)
		{
			SingletonObject.getInstance<LifeRecordsController>().UpdateDreamBackYear();
		}
		else
		{
			SingletonObject.getInstance<LifeRecordsController>().SetShowingYear(this._yearEnd, base.CharacterMenu.CurCharacterId, false);
		}
	}

	// Token: 0x06001E77 RID: 7799 RVA: 0x000DB1D8 File Offset: 0x000D93D8
	public override void OnSubpageInVisible()
	{
		bool isDestroying = SingletonObject.IsDestroying;
		if (!isDestroying)
		{
			LifeRecordsController controller = SingletonObject.getInstance<LifeRecordsController>();
			bool flag = null != controller;
			if (flag)
			{
				controller.StopReadRecords();
				controller.RemoveLatestYearRecordCache();
			}
			GEvent.Remove(UiEvents.OnGetCharBirthDateByLifeRecordModel, new GEvent.Callback(this.RefreshTopYearPageList));
			GEvent.Remove(UiEvents.OnCharacterLifeRecordYearReady, new GEvent.Callback(this.OnCharacterLifeRecordYearDataReady));
			this.GroupedScrollView.UpdateData(null, false);
			this.CollectAllItem();
			this.CollectAllLink();
			this.LifeRecordsDateSelector.HidePanel();
			this.LifeRecordsDateSelectorEN.HidePanel();
			this._filterYearDataList.Clear();
			this._yearDataList.Clear();
		}
	}

	// Token: 0x06001E78 RID: 7800 RVA: 0x000DB294 File Offset: 0x000D9494
	private void AwakeInit()
	{
		this._summeryPanelHeightForSimpleMode = this.SummaryPanel.RectTransform.rect.height;
		this._detailPanelHeightForSimpleMode = this.DetailPanel.RectTransform.rect.height;
		this.SummaryPanel.CGet<Refers>("LinkPrefab").gameObject.SetActive(false);
		this.SummaryPanel.CGet<Refers>("SummaryDetailItem").gameObject.SetActive(false);
		this.SummaryPanel.CGet<Refers>("SummaryDetailItemEN").gameObject.SetActive(false);
		this.SummaryPanel.CGet<Refers>("SummarySimpleItem").gameObject.SetActive(false);
		this.SummaryPanel.CGet<Refers>("SummarySimpleItemEN").gameObject.SetActive(false);
		this.SummaryPanel.CGet<CButtonObsolete>("ButtonMode").ClearAndAddListener(new Action(this.SwitchSummaryMode));
		this.ToggleGreat.onValueChanged.RemoveAllListeners();
		this.ToggleGreat.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleGreatChange));
		this.GroupedScrollView.Init();
		this.GroupedScrollView.OnItemRender = new Action<int, int, Refers>(this.OnItemRender);
		this.GroupedScrollView.OnGroupTitleRender = new Action<int, Refers>(this.OnGroupTitleRender);
		this.GroupedScrollView.LoopScroll.verticalScrollbar.onValueChanged.AddListener(delegate(float v)
		{
			bool flag = Mathf.Abs(v - this._lastScrollbarValue) < 0.01f;
			if (!flag)
			{
				bool flag2 = this._lastScrollbarValue >= 1f && v >= 1f;
				if (!flag2)
				{
					bool flag3 = this._lastScrollbarValue <= 0f && v <= 0f;
					if (!flag3)
					{
						this.RefreshFocus();
						this._lastScrollbarValue = v;
					}
				}
			}
		});
		this.GroupedScrollView.AddOnScrollEvent(new Action(this.RefreshFocus));
		this.GroupedScrollView.UpdateData(this._scrollDataList, true);
		this.LifeRecordsDateSelector.Init(new Action<int>(this.OnSelectDate));
		this.LifeRecordsDateSelectorEN.Init(new Action<int>(this.OnSelectDate));
		this.RefreshSelectorForLanguage(this.CurLanguageType);
		this._charNameFullBtnPoolItem = new PoolItem(string.Empty, base.CGet<GameObject>("FullCharNameButton"));
		this._charNameLeftPartBtnPoolItem = new PoolItem(string.Empty, base.CGet<GameObject>("LeftPartCharNameButton"));
		this._charNameRightPartBtnPoolItem = new PoolItem(string.Empty, base.CGet<GameObject>("RightPartCharNameButton"));
		this.ButtonDisplaySetting.ClearAndAddListener(new Action(this.OnClickButtonDisplaySetting));
		this.ButtonResetSetting.ClearAndAddListener(new Action(this.OnClickButtonResetSetting));
		this.InitDisplaySettings(true, false);
	}

	// Token: 0x06001E79 RID: 7801 RVA: 0x000DB510 File Offset: 0x000D9710
	private void OnGroupTitleRender(int groupIndex, Refers refers)
	{
		UI_CharacterMenuLifeRecords.RecordGroup group = this._scrollGroupList[groupIndex];
		refers.CGet<TextMeshProUGUI>("TitleName").text = group.GroupTitleText;
	}

	// Token: 0x06001E7A RID: 7802 RVA: 0x000DB544 File Offset: 0x000D9744
	private void OnItemRender(int groupIndex, int dataIndex, Refers refers)
	{
		string invisibleRecordString = new string('·', 50) + LocalStringManager.Get(LanguageKey.LK_LifeRecord_NeedMoreFavor);
		UI_CharacterMenuLifeRecords.RecordGroup group = this._scrollGroupList[groupIndex];
		ValueTuple<int, string, string, short> valueTuple = group.MonthData.RecordList[dataIndex];
		int score = valueTuple.Item1;
		string content = valueTuple.Item3;
		short templateId = valueTuple.Item4;
		LifeRecordItem config = LifeRecord.Instance.GetItem(templateId);
		string backSp = UI_CharacterMenuLifeRecords.GetRecordLineBackSprite(score, ((config != null) ? new ELifeRecordDisplayType?(config.DisplayType) : null) == ELifeRecordDisplayType.Great);
		refers.CGet<CImage>("Back").SetSprite(backSp, false, null);
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
		bool flag2 = config != null && config.RequiredFavorability > 0 && this._favorToTaiwu < config.RequiredFavorability;
		if (flag2)
		{
			content = invisibleRecordString;
		}
		bool flag3 = templateId < 0;
		if (flag3)
		{
			bool flag4 = !base.CharacterMenu.CurrentCharacterIsTaiwu && this._favorToTaiwu < 10000;
			if (flag4)
			{
				content = invisibleRecordString;
			}
		}
		label.text = content;
		if (handler != null)
		{
			handler.ProcessLinkInfo(label, true);
		}
		spriteHelper.Parse();
	}

	// Token: 0x06001E7B RID: 7803 RVA: 0x000DB6F4 File Offset: 0x000D98F4
	private void RefreshFocus()
	{
		bool isStopRefreshFocus = this._isStopRefreshFocus;
		if (!isStopRefreshFocus)
		{
			this._showingGroupList.Clear();
			Rect scrollRect = CommonUtils.RectTransToScreenPos(this.GroupedScrollView.GetViewport(), UIManager.Instance.UiCamera);
			foreach (KeyValuePair<GameObject, GroupedInfinityScroll.LineData> keyValuePair in this.GroupedScrollView.RenderingItems)
			{
				GameObject gameObject;
				GroupedInfinityScroll.LineData lineData3;
				keyValuePair.Deconstruct(out gameObject, out lineData3);
				GameObject item = gameObject;
				GroupedInfinityScroll.LineData lineData = lineData3;
				RectTransform rectTransform = item.transform as RectTransform;
				Rect itemRect = CommonUtils.RectTransToScreenPos(rectTransform, UIManager.Instance.UiCamera);
				bool containsMin = scrollRect.ContainsWithBorder(itemRect.min);
				bool containsMax = scrollRect.ContainsWithBorder(itemRect.max);
				bool overlaps = containsMin || containsMax || (itemRect.yMin < scrollRect.yMax && itemRect.yMax > scrollRect.yMin);
				bool flag = overlaps;
				if (flag)
				{
					this._showingGroupList.Add(lineData.GroupIndex);
				}
			}
			bool flag2 = this._showingGroupList.Count == 0;
			if (!flag2)
			{
				string dateInfo = string.Join(", ", from idx in this._showingGroupList
				select string.Format("G{0}:D{1}", idx, this._scrollGroupList[idx].MonthData.Date));
				float targetY = scrollRect.yMax - scrollRect.height * 0.25f;
				float minDistance = float.MaxValue;
				int selectedGroupIndex = -1;
				foreach (KeyValuePair<GameObject, GroupedInfinityScroll.LineData> keyValuePair in this.GroupedScrollView.RenderingItems)
				{
					GameObject gameObject;
					GroupedInfinityScroll.LineData lineData3;
					keyValuePair.Deconstruct(out gameObject, out lineData3);
					GameObject item2 = gameObject;
					GroupedInfinityScroll.LineData lineData2 = lineData3;
					bool flag3 = !this._showingGroupList.Contains(lineData2.GroupIndex);
					if (!flag3)
					{
						RectTransform rectTransform2 = item2.transform as RectTransform;
						Rect itemRect2 = CommonUtils.RectTransToScreenPos(rectTransform2, UIManager.Instance.UiCamera);
						float itemCenterY = (itemRect2.yMin + itemRect2.yMax) / 2f;
						float distance = Mathf.Abs(itemCenterY - targetY);
						bool flag4 = distance < minDistance;
						if (flag4)
						{
							minDistance = distance;
							selectedGroupIndex = lineData2.GroupIndex;
						}
					}
				}
				bool flag5 = selectedGroupIndex < 0;
				if (!flag5)
				{
					UI_CharacterMenuLifeRecords.RecordGroup data = this._scrollGroupList[selectedGroupIndex];
					int year = SingletonObject.getInstance<TimeManager>().GetYearByDate(data.MonthData.Date);
					bool flag6 = this._focusYear != year;
					if (flag6)
					{
						this._focusYear = year;
						this.RefreshSummary();
					}
					bool flag7 = this.CurLifeRecordsDateSelector.SelectedMonthData != data.MonthData;
					if (flag7)
					{
						this.CurLifeRecordsDateSelector.Select(data.MonthData, false);
					}
				}
			}
		}
	}

	// Token: 0x06001E7C RID: 7804 RVA: 0x000DB9E4 File Offset: 0x000D9BE4
	public static string GetRecordLineBackSprite(int score, bool isGreat)
	{
		string result;
		if (isGreat)
		{
			result = "ui_charactermenu_30_important_0";
		}
		else
		{
			if (!true)
			{
			}
			string text;
			if (score < 60)
			{
				if (score < 40)
				{
					if (score >= 20)
					{
						text = "ui_charactermenu_30_badmarker_1";
					}
					else
					{
						text = "ui_charactermenu_30_badmarker_2";
					}
				}
				else if (score >= 50)
				{
					if (score != 50)
					{
						text = "ui_charactermenu_30_goodmarker_0";
					}
					else
					{
						text = string.Empty;
					}
				}
				else
				{
					text = "ui_charactermenu_30_badmarker_0";
				}
			}
			else if (score >= 80)
			{
				text = "ui_charactermenu_30_goodmarker_2";
			}
			else
			{
				text = "ui_charactermenu_30_goodmarker_1";
			}
			if (!true)
			{
			}
			string sp = text;
			result = sp;
		}
		return result;
	}

	// Token: 0x06001E7D RID: 7805 RVA: 0x000DBA6C File Offset: 0x000D9C6C
	private void RefreshTopYearPageList(ArgumentBox box)
	{
		int charId;
		bool flag = !box.Get("CharacterId", out charId) || charId != base.CharacterMenu.CurCharacterId;
		if (!flag)
		{
			int birthDate;
			bool flag2 = box.Get("BirthDate", out birthDate);
			if (flag2)
			{
				birthDate = Mathf.Max(birthDate, 5);
				this._yearStart = SingletonObject.getInstance<TimeManager>().GetYearByDate(birthDate);
				this.RefreshYearList();
				this._focusYear = Mathf.Clamp(this._focusYear, this._yearStart, this._yearEnd);
				this.UpdateShowingYear();
				this.Element.ShowAfterRefresh();
			}
		}
	}

	// Token: 0x06001E7E RID: 7806 RVA: 0x000DBB08 File Offset: 0x000D9D08
	private void RefreshYearList()
	{
		this._yearList.Clear();
		for (int i = this._yearStart; i <= this._yearEnd; i++)
		{
			this._yearList.Add(i);
		}
	}

	// Token: 0x06001E7F RID: 7807 RVA: 0x000DBB4C File Offset: 0x000D9D4C
	private void OnSelectDate(int date)
	{
		this.StopRefreshFocus();
		int groupIndex = this._scrollGroupList.FindIndex((UI_CharacterMenuLifeRecords.RecordGroup g) => g.MonthData.Date == date);
		bool flag = groupIndex >= 0;
		if (flag)
		{
			this.GroupedScrollView.RefillCellsByGroupIndex(groupIndex);
		}
		int year = SingletonObject.getInstance<TimeManager>().GetYearByDate(date);
		bool flag2 = this._focusYear != year;
		if (flag2)
		{
			this._focusYear = year;
			this.RefreshSummary();
		}
	}

	// Token: 0x06001E80 RID: 7808 RVA: 0x000DBBD3 File Offset: 0x000D9DD3
	private void StopRefreshFocus()
	{
		this._isStopRefreshFocus = true;
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(4U, delegate
		{
			this._isStopRefreshFocus = false;
		});
	}

	// Token: 0x06001E81 RID: 7809 RVA: 0x000DBBF8 File Offset: 0x000D9DF8
	private void UpdateShowingYear()
	{
		LifeRecordsController recordsController = SingletonObject.getInstance<LifeRecordsController>();
		foreach (int year in this._yearList)
		{
			bool flag = this.IsCurrentSelectDreamBack();
			if (flag)
			{
				recordsController.SetDreamBackShowingYear(year);
			}
			else
			{
				recordsController.SetShowingYear(year, base.CharacterMenu.CurCharacterId, false);
			}
		}
	}

	// Token: 0x06001E82 RID: 7810 RVA: 0x000DBC78 File Offset: 0x000D9E78
	private void OnCharacterLifeRecordYearDataReady(ArgumentBox box)
	{
		int charId;
		bool flag = !box.Get("CharacterId", out charId) || charId != base.CharacterMenu.CurCharacterId;
		if (!flag)
		{
			this.RefreshYearList();
			int year;
			bool flag2 = !box.Get("Year", out year) || !this._yearList.Contains(year);
			if (!flag2)
			{
				bool flag3 = this.IsCurrentSelectDreamBack();
				if (flag3)
				{
					SingletonObject.getInstance<LifeRecordsController>().GetDreamBackCharacterLifeRecordYearDataList(this._yearList, this._tempYearDataList);
				}
				else
				{
					SingletonObject.getInstance<LifeRecordsController>().GetCharacterLifeRecordYearDataList(base.CharacterMenu.CurCharacterId, this._yearList, this._tempYearDataList);
				}
				bool flag4 = this._tempYearDataList.Count != this._yearList.Count;
				if (!flag4)
				{
					bool flag5 = this._yearDataList.ContentIsSame(this._tempYearDataList);
					if (!flag5)
					{
						this._yearDataList.Clear();
						this._yearDataList.AddRange(this._tempYearDataList);
						this.RefreshDetailView(true);
						this.RefreshSummary();
					}
				}
			}
		}
	}

	// Token: 0x06001E83 RID: 7811 RVA: 0x000DBD98 File Offset: 0x000D9F98
	private void CollectAllLink()
	{
		RectTransform cacheRoot = this.SummaryPanel.CGet<RectTransform>("PrefabCacheRoot");
		RectTransform linkRoot = this.SummaryPanel.CGet<RectTransform>("LinkLineRoot");
		Refers[] usingLinkLines = linkRoot.GetComponentsInTopChildren(true);
		foreach (Refers line in usingLinkLines)
		{
			line.name = "LinkLine";
			line.transform.SetParent(cacheRoot, false);
		}
	}

	// Token: 0x06001E84 RID: 7812 RVA: 0x000DBE0C File Offset: 0x000DA00C
	private void CollectAllItem()
	{
		RectTransform cacheRoot = this.SummaryPanel.CGet<RectTransform>("PrefabCacheRoot");
		RectTransform itemRoot = this.SummaryPanel.CGet<RectTransform>("ItemRoot");
		Refers[] usingItemRefers = itemRoot.GetComponentsInTopChildren(true);
		string prefabName = this.GetSummaryPrefabName();
		foreach (Refers item in usingItemRefers)
		{
			item.name = prefabName;
			item.transform.SetParent(cacheRoot, false);
		}
	}

	// Token: 0x06001E85 RID: 7813 RVA: 0x000DBE84 File Offset: 0x000DA084
	private void UpdateScrollDataList(List<UI_CharacterMenuLifeRecords.RecordGroup> itemGroups)
	{
		this._scrollDataList.Clear();
		foreach (UI_CharacterMenuLifeRecords.RecordGroup itemGroup in itemGroups)
		{
			GroupedInfinityScroll.GroupItem groupItem = new GroupedInfinityScroll.GroupItem(itemGroup.GroupId, itemGroup.MonthData.RecordList.Count);
			this._scrollDataList.Add(groupItem);
		}
	}

	// Token: 0x06001E86 RID: 7814 RVA: 0x000DBF08 File Offset: 0x000DA108
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
						UI_CharacterMenuLifeRecords.RecordGroup groupItem = new UI_CharacterMenuLifeRecords.RecordGroup(groupId++, month);
						this._scrollGroupList.Add(groupItem);
					}
				}
			}
		}
	}

	// Token: 0x06001E87 RID: 7815 RVA: 0x000DC008 File Offset: 0x000DA208
	private void RefreshDetailView(bool init)
	{
		this.StopRefreshFocus();
		this.RefreshFilterYearDataList();
		this.UpdateScrollGroupList(this._filterYearDataList);
		this.UpdateScrollDataList(this._scrollGroupList);
		this.GroupedScrollView.UpdateData(this._scrollDataList, false);
		this._favorToTaiwu = SingletonObject.getInstance<LifeRecordsController>().GetFavorabilityToTaiwuFromCacheData(base.CharacterMenu.CurCharacterId, this.IsCurrentSelectDreamBack());
		bool currentCharacterIsTaiwu = base.CharacterMenu.CurrentCharacterIsTaiwu;
		if (currentCharacterIsTaiwu)
		{
			this._favorToTaiwu = 30000;
		}
		bool hasRecord = this._scrollDataList.Count > 0;
		this.DetailPanel.CGet<GameObject>("NoContent").SetActive(!hasRecord);
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

	// Token: 0x06001E88 RID: 7816 RVA: 0x000DC0EC File Offset: 0x000DA2EC
	private void RefreshFilterYearDataList()
	{
		this._filterYearDataList.Clear();
		bool flag = this._yearDataList == null;
		if (!flag)
		{
			bool flag2 = !this._onlyShowGreat && this._enabledDisplayTypeSet.Count == ELifeRecordDisplayType.Count.ToInt();
			if (flag2)
			{
				this._filterYearDataList.AddRange(this._yearDataList);
			}
			else
			{
				foreach (LifeRecordsController.LifeRecordYearData yearData in this._yearDataList)
				{
					List<LifeRecordsController.LifeRecordMonthData> monthRecordDataList = yearData.MonthRecordDataList;
					bool flag3 = monthRecordDataList == null || monthRecordDataList.Count <= 0;
					if (!flag3)
					{
						LifeRecordsController.LifeRecordYearData tempYearData = null;
						foreach (LifeRecordsController.LifeRecordMonthData monthData in yearData.MonthRecordDataList)
						{
							List<ValueTuple<int, string, string, short>> recordList = monthData.RecordList;
							bool flag4 = recordList == null || recordList.Count <= 0;
							if (!flag4)
							{
								LifeRecordsController.LifeRecordMonthData tempMonthData = null;
								foreach (ValueTuple<int, string, string, short> record in monthData.RecordList)
								{
									bool flag5 = record.Item4 < 0;
									if (!flag5)
									{
										LifeRecordItem config = LifeRecord.Instance[record.Item4];
										bool onlyShowGreat = this._onlyShowGreat;
										if (onlyShowGreat)
										{
											bool flag6 = config.DisplayType != ELifeRecordDisplayType.Great;
											if (flag6)
											{
												continue;
											}
										}
										else
										{
											bool flag7 = !this._enabledDisplayTypeSet.Contains(config.DisplayType);
											if (flag7)
											{
												continue;
											}
										}
										bool flag8 = tempYearData == null;
										if (flag8)
										{
											tempYearData = new LifeRecordsController.LifeRecordYearData
											{
												Year = yearData.Year,
												Key = yearData.Key,
												Score = yearData.Score,
												StartDate = yearData.StartDate,
												MonthRecordDataList = new List<LifeRecordsController.LifeRecordMonthData>()
											};
											this._filterYearDataList.Add(tempYearData);
										}
										bool flag9 = tempMonthData == null;
										if (flag9)
										{
											tempMonthData = new LifeRecordsController.LifeRecordMonthData
											{
												Date = monthData.Date,
												Score = monthData.Score,
												RecordList = new List<ValueTuple<int, string, string, short>>()
											};
											tempYearData.MonthRecordDataList.Add(tempMonthData);
										}
										tempMonthData.RecordList.Add(record);
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06001E89 RID: 7817 RVA: 0x000DC3D0 File Offset: 0x000DA5D0
	private void OnCharacterNameClicked(int charId)
	{
		this.HandleCharacterDisplayData(charId);
	}

	// Token: 0x06001E8A RID: 7818 RVA: 0x000DC3DB File Offset: 0x000DA5DB
	private void HandleCharacterDisplayData(int charId)
	{
		CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataListForRelations(this, new List<int>
		{
			charId
		}, delegate(int offset, RawDataPool dataPool)
		{
			List<CharacterDisplayDataForRelations> charData = new List<CharacterDisplayDataForRelations>();
			Serializer.Deserialize(dataPool, offset, ref charData);
			ArgumentBox args = new ArgumentBox();
			args.SetObject("TargetPageIndex", ECharacterSubToggleBase.StoryBase);
			GEvent.OnEvent(UiEvents.OnNeedOpenCharacterMenuSubPage, args);
			base.CharacterMenu.GotoCharacterWithStackView(charData[0], null);
		});
	}

	// Token: 0x06001E8B RID: 7819 RVA: 0x000DC400 File Offset: 0x000DA600
	private void SetLink(int score, Transform fromItem, Transform toItem)
	{
		RectTransform cacheRoot = this.SummaryPanel.CGet<RectTransform>("PrefabCacheRoot");
		RectTransform linkRoot = this.SummaryPanel.CGet<RectTransform>("LinkLineRoot");
		Transform linkLineTrans = cacheRoot.Find("LinkLine");
		bool flag = linkLineTrans != null;
		Refers linkLineRefers;
		if (flag)
		{
			linkLineRefers = linkLineTrans.GetComponent<Refers>();
			linkLineTrans.SetParent(linkRoot, false);
		}
		else
		{
			linkLineRefers = Object.Instantiate<Refers>(this.SummaryPanel.CGet<Refers>("LinkPrefab"), linkRoot, false);
			linkLineTrans = linkLineRefers.transform;
		}
		Vector3 fromItemPosOffset = fromItem.InverseTransformPoint(fromItem.GetComponent<Refers>().CGet<CImage>("Type").transform.position);
		Vector3 fromItemLocalPosition = fromItem.localPosition + fromItemPosOffset;
		linkLineTrans.localPosition = fromItemLocalPosition;
		SkeletonGraphic animGraphic = linkLineRefers.CGet<SkeletonGraphic>("LinkLineAnimation");
		if (!true)
		{
		}
		string text;
		if (score >= 50)
		{
			if (score <= 50)
			{
				text = "LinkSkeleton_Normal";
			}
			else
			{
				text = "LinkSkeleton_Good";
			}
		}
		else
		{
			text = "LinkSkeleton_Bad";
		}
		if (!true)
		{
		}
		string skeletonDataKey = text;
		CommonUtils.SetSkeletonDataAsset(animGraphic, this.SummaryPanel.CGet<SkeletonDataAsset>(skeletonDataKey), "default", "animation", true);
		Vector3 toItemPosOffset = toItem.InverseTransformPoint(toItem.GetComponent<Refers>().CGet<CImage>("Type").transform.position);
		Vector3 toItemLocalPosition = toItem.localPosition + toItemPosOffset;
		float length = Vector2.Distance(fromItemLocalPosition, toItemLocalPosition);
		RectTransform linkLineRectTrans = linkLineTrans as RectTransform;
		linkLineRectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, length);
		float angle = Mathf.Atan2(toItemLocalPosition.y - fromItemLocalPosition.y, toItemLocalPosition.x - fromItemLocalPosition.x) * 57.29578f;
		linkLineTrans.localEulerAngles = linkLineRectTrans.localEulerAngles.SetZ(angle);
		linkLineRefers.gameObject.SetActive(true);
	}

	// Token: 0x06001E8C RID: 7820 RVA: 0x000DC5BC File Offset: 0x000DA7BC
	private void RenderSummarySimpleItem(LifeRecordsController.LifeRecordYearData yearRecordData, Refers refers)
	{
		int score = yearRecordData.Score;
		if (!true)
		{
		}
		LanguageKey languageKey;
		if (score <= 50)
		{
			if (score >= 50)
			{
				languageKey = LanguageKey.UI_LifeRecord_Year_Normal;
			}
			else
			{
				languageKey = LanguageKey.UI_LifeRecord_Year_Bad;
			}
		}
		else
		{
			languageKey = LanguageKey.UI_LifeRecord_Year_Good;
		}
		if (!true)
		{
		}
		LanguageKey yearNameKey = languageKey;
		refers.CGet<TextMeshProUGUI>("Name").text = yearNameKey.Tr();
	}

	// Token: 0x06001E8D RID: 7821 RVA: 0x000DC618 File Offset: 0x000DA818
	private void RenderSummaryDetailItem(LifeRecordsController.LifeRecordYearData yearRecordData, Refers refers)
	{
		this.RenderSummarySimpleItem(yearRecordData, refers);
		refers.CGet<TextMeshProUGUI>("Year").text = LocalStringManager.GetFormat(LanguageKey.UI_AdvanceMonth_TimeChangeInfo_Year, yearRecordData.Year);
		int score = yearRecordData.Score;
		if (!true)
		{
		}
		string text;
		if (score >= 50)
		{
			if (score <= 50)
			{
				text = "ui_charactermenu_30_experience_base_2";
			}
			else
			{
				text = "ui_charactermenu_30_experience_base_1";
			}
		}
		else
		{
			text = "ui_charactermenu_30_experience_base_3";
		}
		if (!true)
		{
		}
		string typeSprite = text;
		refers.CGet<CImage>("Type").SetSprite(typeSprite, false, null);
	}

	// Token: 0x06001E8E RID: 7822 RVA: 0x000DC6A0 File Offset: 0x000DA8A0
	private string GetSummaryPrefabName()
	{
		bool isCn = this.CurLanguageType == LocalStringManager.LanguageType.CN;
		return this._summaryIsSimple ? (isCn ? "SummarySimpleItem" : "SummarySimpleItemEN") : (isCn ? "SummaryDetailItem" : "SummaryDetailItemEN");
	}

	// Token: 0x06001E8F RID: 7823 RVA: 0x000DC6E8 File Offset: 0x000DA8E8
	private void RefreshSummary()
	{
		this.CollectAllLink();
		this.CollectAllItem();
		this._summaryDataList.Clear();
		List<LifeRecordsController.LifeRecordYearData> yearDataList = this._yearDataList;
		bool flag = yearDataList != null && yearDataList.Count > 0;
		if (flag)
		{
			int minYear = this._yearDataList.Min((LifeRecordsController.LifeRecordYearData d) => d.Year);
			int maxYear = this._yearDataList.Max((LifeRecordsController.LifeRecordYearData d) => d.Year);
			int targetMinYear = Math.Max(minYear, this._focusYear - this.summaryStartYearOffset);
			int targetMaxYear = Math.Min(maxYear, targetMinYear + this.summaryStartYearOffset);
			this._summaryDataList.AddRange(from d in this._yearDataList
			where d.Year >= targetMinYear && d.Year <= targetMaxYear
			select d);
		}
		RectTransform cacheRoot = this.SummaryPanel.CGet<RectTransform>("PrefabCacheRoot");
		RectTransform itemRoot = this.SummaryPanel.CGet<RectTransform>("ItemRoot");
		string prefabName = this.GetSummaryPrefabName();
		Refers prefab = this.SummaryPanel.CGet<Refers>(prefabName);
		RectTransform prefabRectTrans = prefab.transform as RectTransform;
		float prefabWidth = prefabRectTrans.rect.width;
		float itemGap = this._summaryIsSimple ? this.summarySimpleItemGap : this.summaryDetailItemGap;
		int num;
		if (this._summaryDataList.Count <= 0)
		{
			num = 0;
		}
		else
		{
			num = this._summaryDataList.Max((LifeRecordsController.LifeRecordYearData d) => d.Score);
		}
		int maxScore = num;
		int num2;
		if (this._summaryDataList.Count <= 0)
		{
			num2 = 0;
		}
		else
		{
			num2 = this._summaryDataList.Min((LifeRecordsController.LifeRecordYearData d) => d.Score);
		}
		int minScore = num2;
		float height = (itemRoot.rect.height - prefabWidth) * 0.5f;
		for (int i = 0; i < this._summaryDataList.Count; i++)
		{
			LifeRecordsController.LifeRecordYearData yearData = this._summaryDataList[i];
			Transform yearItemTrans = cacheRoot.Find(prefabName);
			bool flag2 = yearItemTrans == null;
			Refers yearItemRefers;
			if (flag2)
			{
				yearItemRefers = Object.Instantiate<Refers>(prefab, itemRoot, false);
				yearItemTrans = yearItemRefers.transform;
			}
			else
			{
				yearItemTrans.SetParent(itemRoot, false);
				yearItemRefers = yearItemTrans.GetComponent<Refers>();
			}
			yearItemRefers.gameObject.SetActive(true);
			yearItemRefers.name = yearData.Year.ToString();
			bool summaryIsSimple = this._summaryIsSimple;
			if (summaryIsSimple)
			{
				this.RenderSummarySimpleItem(yearData, yearItemRefers);
			}
			else
			{
				this.RenderSummaryDetailItem(yearData, yearItemRefers);
			}
			Vector3 pos = yearItemTrans.localPosition;
			pos.z = 0f;
			pos.x = ((float)i + 0.5f) * prefabWidth + (float)i * itemGap + this.summeryItemHorizontalPadding;
			float scoreRate = (this._summaryIsSimple || minScore == maxScore) ? 0.5f : Mathf.Clamp((float)(yearData.Score - minScore) * 1f / (float)(maxScore - minScore), 0f, 1f);
			pos.y = Mathf.Lerp(-height, height, scoreRate);
			yearItemTrans.localPosition = pos;
			bool flag3 = i > 0 && !this._summaryIsSimple;
			if (flag3)
			{
				Transform prevYearTrans = itemRoot.Find(string.Format("{0}", yearData.Year - 1));
				bool flag4 = prevYearTrans;
				if (flag4)
				{
					int score = 50;
					bool flag5 = yearData.Score > this._summaryDataList[i - 1].Score;
					if (flag5)
					{
						score = 80;
					}
					bool flag6 = yearData.Score < this._summaryDataList[i - 1].Score;
					if (flag6)
					{
						score = 30;
					}
					this.SetLink(score, prevYearTrans, yearItemTrans);
				}
			}
		}
		float contentWidth = (itemRoot.childCount > 0) ? (itemRoot.GetChild(itemRoot.childCount - 1).localPosition.x + prefabWidth * 0.5f + this.summeryItemHorizontalPadding) : 0f;
		itemRoot.sizeDelta = itemRoot.sizeDelta.SetX(contentWidth);
		RectTransform linkRoot = this.SummaryPanel.CGet<RectTransform>("LinkLineRoot");
		linkRoot.sizeDelta = itemRoot.sizeDelta;
		CScrollRectLegacy scroll = this.SummaryPanel.CGet<CScrollRectLegacy>("HorizontalScrollView");
		scroll.NeedHideScrollBar = true;
		scroll.ScrollBar.value = 1f;
		scroll.SetClick(new Action(this.SwitchSummaryMode));
	}

	// Token: 0x06001E90 RID: 7824 RVA: 0x000DCB95 File Offset: 0x000DAD95
	private void SwitchSummaryMode()
	{
		this.CollectAllLink();
		this.CollectAllItem();
		this._summaryIsSimple = !this._summaryIsSimple;
		this.RefreshPanelSize();
		this.RefreshSummary();
	}

	// Token: 0x06001E91 RID: 7825 RVA: 0x000DCBC4 File Offset: 0x000DADC4
	private void RefreshPanelSize()
	{
		float summeryHeight = this._summaryIsSimple ? this._summeryPanelHeightForSimpleMode : (this._summeryPanelHeightForSimpleMode + this.summeryPanelHeightOffsetForDetailMode);
		float detailHeight = this._summaryIsSimple ? this._detailPanelHeightForSimpleMode : (this._detailPanelHeightForSimpleMode - this.summeryPanelHeightOffsetForDetailMode);
		this.SummaryPanel.RectTransform.sizeDelta = this.SummaryPanel.RectTransform.sizeDelta.SetY(summeryHeight);
		this.DetailPanel.RectTransform.sizeDelta = this.DetailPanel.RectTransform.sizeDelta.SetY(detailHeight);
	}

	// Token: 0x06001E92 RID: 7826 RVA: 0x000DCC5C File Offset: 0x000DAE5C
	private void OnToggleGreatChange(bool isOn)
	{
		this._onlyShowGreat = isOn;
		this.RefreshDetailView(false);
	}

	// Token: 0x06001E93 RID: 7827 RVA: 0x000DCC70 File Offset: 0x000DAE70
	private void InitDisplaySettings(bool isOn, bool show)
	{
		this.DisplaySettings.SetActive(show);
		CommonConfigureBase[] lineList = this.DisplaySettings.GetComponentsInChildren<CommonConfigureBase>();
		for (int index = 0; index < lineList.Length; index++)
		{
			CommonConfigureBase configureBase = lineList[index];
			ELifeRecordDisplayType displayType = (ELifeRecordDisplayType)index;
			if (!true)
			{
			}
			string text;
			switch (displayType)
			{
			case ELifeRecordDisplayType.Great:
				text = LanguageKey.LK_LifeRecord_DisplayType_Great.Tr();
				break;
			case ELifeRecordDisplayType.Normal:
				text = LanguageKey.LK_LifeRecord_DisplayType_Normal.Tr();
				break;
			case ELifeRecordDisplayType.Relation:
				text = LanguageKey.LK_LifeRecord_DisplayType_Relation.Tr();
				break;
			case ELifeRecordDisplayType.Study:
				text = LanguageKey.LK_LifeRecord_DisplayType_Study.Tr();
				break;
			case ELifeRecordDisplayType.Produce:
				text = LanguageKey.LK_LifeRecord_DisplayType_Produce.Tr();
				break;
			case ELifeRecordDisplayType.Combat:
				text = LanguageKey.LK_LifeRecord_DisplayType_Combat.Tr();
				break;
			case ELifeRecordDisplayType.Negative:
				text = LanguageKey.LK_LifeRecord_DisplayType_Negative.Tr();
				break;
			case ELifeRecordDisplayType.Crime:
				text = LanguageKey.LK_LifeRecord_DisplayType_Crime.Tr();
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (!true)
			{
			}
			string typeName = text;
			configureBase.Text = typeName;
			CommonSwitch commonSwitch = configureBase.GetComponentInChildren<CommonSwitch>();
			commonSwitch.onValueChanged.RemoveAllListeners();
			commonSwitch.isOn = isOn2;
			this.<InitDisplaySettings>g__Change|98_0(displayType, isOn2);
			commonSwitch.onValueChanged.AddListener(delegate(bool isOn)
			{
				this.<InitDisplaySettings>g__Change|98_0(displayType, isOn);
				bool flag = !this._onlyShowGreat;
				if (flag)
				{
					this.RefreshDetailView(false);
				}
			});
		}
	}

	// Token: 0x06001E94 RID: 7828 RVA: 0x000DCDCE File Offset: 0x000DAFCE
	private void OnClickButtonDisplaySetting()
	{
		this.DisplaySettings.SetActive(!this.DisplaySettings.activeSelf);
	}

	// Token: 0x06001E95 RID: 7829 RVA: 0x000DCDEB File Offset: 0x000DAFEB
	private void OnClickButtonResetSetting()
	{
		this.InitDisplaySettings(true, true);
		this.RefreshDetailView(false);
	}

	// Token: 0x06001E96 RID: 7830 RVA: 0x000DCDFF File Offset: 0x000DAFFF
	public override void OnLanguageChange(LocalStringManager.LanguageType languageType)
	{
		this.CollectAllLink();
		this.CollectAllItem();
		this.RefreshSelectorForLanguage(languageType);
		base.OnLanguageChange(languageType);
		this.RefreshSummary();
	}

	// Token: 0x06001E97 RID: 7831 RVA: 0x000DCE28 File Offset: 0x000DB028
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

	// Token: 0x06001E9F RID: 7839 RVA: 0x000DD0B0 File Offset: 0x000DB2B0
	[CompilerGenerated]
	private void <InitDisplaySettings>g__Change|98_0(ELifeRecordDisplayType displayType, bool enable)
	{
		if (enable)
		{
			this._enabledDisplayTypeSet.Add(displayType);
		}
		else
		{
			this._enabledDisplayTypeSet.Remove(displayType);
		}
	}

	// Token: 0x04001711 RID: 5905
	private const string LinkPrefab = "LinkLine";

	// Token: 0x04001712 RID: 5906
	private const string SummaryDetailItem = "SummaryDetailItem";

	// Token: 0x04001713 RID: 5907
	private const string SummaryDetailItemEN = "SummaryDetailItemEN";

	// Token: 0x04001714 RID: 5908
	private const string SummarySimpleItem = "SummarySimpleItem";

	// Token: 0x04001715 RID: 5909
	private const string SummarySimpleItemEN = "SummarySimpleItemEN";

	// Token: 0x04001716 RID: 5910
	private const string LinkSkeletonData_Normal = "LinkSkeleton_Normal";

	// Token: 0x04001717 RID: 5911
	private const string LinkSkeletonData_Good = "LinkSkeleton_Good";

	// Token: 0x04001718 RID: 5912
	private const string LinkSkeletonData_Bad = "LinkSkeleton_Bad";

	// Token: 0x04001719 RID: 5913
	public const short FavorToSeeBornRecord = 10000;

	// Token: 0x0400171A RID: 5914
	public float summaryDetailItemGap;

	// Token: 0x0400171B RID: 5915
	public float summarySimpleItemGap;

	// Token: 0x0400171C RID: 5916
	public int summaryStartYearOffset = 12;

	// Token: 0x0400171D RID: 5917
	public float summeryPanelHeightOffsetForDetailMode;

	// Token: 0x0400171E RID: 5918
	public float summeryItemHorizontalPadding;

	// Token: 0x0400171F RID: 5919
	private bool _summaryIsSimple = true;

	// Token: 0x04001720 RID: 5920
	private bool _onlyShowGreat;

	// Token: 0x04001721 RID: 5921
	private readonly List<LifeRecordsController.LifeRecordYearData> _summaryDataList = new List<LifeRecordsController.LifeRecordYearData>();

	// Token: 0x04001722 RID: 5922
	private bool _isCurrentTaiwuOverwrittenByDreamBack;

	// Token: 0x04001723 RID: 5923
	private int _yearStart;

	// Token: 0x04001724 RID: 5924
	private int _yearEnd;

	// Token: 0x04001725 RID: 5925
	private int _focusYear;

	// Token: 0x04001726 RID: 5926
	private readonly List<int> _yearList = new List<int>();

	// Token: 0x04001727 RID: 5927
	private readonly List<LifeRecordsController.LifeRecordYearData> _yearDataList = new List<LifeRecordsController.LifeRecordYearData>();

	// Token: 0x04001728 RID: 5928
	private readonly List<LifeRecordsController.LifeRecordYearData> _tempYearDataList = new List<LifeRecordsController.LifeRecordYearData>();

	// Token: 0x04001729 RID: 5929
	private readonly List<LifeRecordsController.LifeRecordYearData> _filterYearDataList = new List<LifeRecordsController.LifeRecordYearData>();

	// Token: 0x0400172A RID: 5930
	private PoolItem _charNameFullBtnPoolItem;

	// Token: 0x0400172B RID: 5931
	private PoolItem _charNameLeftPartBtnPoolItem;

	// Token: 0x0400172C RID: 5932
	private PoolItem _charNameRightPartBtnPoolItem;

	// Token: 0x0400172D RID: 5933
	private readonly List<GroupedInfinityScroll.GroupItem> _scrollDataList = new List<GroupedInfinityScroll.GroupItem>();

	// Token: 0x0400172E RID: 5934
	private readonly List<UI_CharacterMenuLifeRecords.RecordGroup> _scrollGroupList = new List<UI_CharacterMenuLifeRecords.RecordGroup>();

	// Token: 0x0400172F RID: 5935
	private short _favorToTaiwu;

	// Token: 0x04001730 RID: 5936
	private readonly HashSet<ELifeRecordDisplayType> _enabledDisplayTypeSet = new HashSet<ELifeRecordDisplayType>();

	// Token: 0x04001731 RID: 5937
	private float _detailPanelHeightForSimpleMode;

	// Token: 0x04001732 RID: 5938
	private float _summeryPanelHeightForSimpleMode;

	// Token: 0x04001733 RID: 5939
	private bool _isStopRefreshFocus;

	// Token: 0x04001734 RID: 5940
	private readonly List<int> _showingGroupList = new List<int>();

	// Token: 0x04001735 RID: 5941
	private float _lastScrollbarValue = -1f;

	// Token: 0x02001440 RID: 5184
	public struct RecordGroup
	{
		// Token: 0x0600CB46 RID: 52038 RVA: 0x005936C4 File Offset: 0x005918C4
		public RecordGroup(int groupId, LifeRecordsController.LifeRecordMonthData monthData)
		{
			this.GroupId = groupId;
			this.MonthData = monthData;
			int year = monthData.Date / 12;
			int month = monthData.Date % 12;
			this.GroupTitleText = LocalStringManager.GetFormat(LanguageKey.LK_Game_Time, new object[]
			{
				year + 1,
				month + 1,
				LocalStringManager.Get(string.Format("LK_Season_{0}", TimeKit.GetSeason(monthData.Date))),
				Month.Instance[month].Name
			});
		}

		// Token: 0x0400A049 RID: 41033
		public readonly int GroupId;

		// Token: 0x0400A04A RID: 41034
		public readonly string GroupTitleText;

		// Token: 0x0400A04B RID: 41035
		public readonly LifeRecordsController.LifeRecordMonthData MonthData;
	}
}
