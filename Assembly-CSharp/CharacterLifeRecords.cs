using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x020001C8 RID: 456
public class CharacterLifeRecords : Refers
{
	// Token: 0x170002E6 RID: 742
	// (get) Token: 0x06001C1C RID: 7196 RVA: 0x000C22FD File Offset: 0x000C04FD
	// (set) Token: 0x06001C1D RID: 7197 RVA: 0x000C2305 File Offset: 0x000C0505
	public int CharacterId { get; set; }

	// Token: 0x170002E7 RID: 743
	// (get) Token: 0x06001C1E RID: 7198 RVA: 0x000C230E File Offset: 0x000C050E
	// (set) Token: 0x06001C1F RID: 7199 RVA: 0x000C2318 File Offset: 0x000C0518
	public int YearStart
	{
		get
		{
			return this._yearStart;
		}
		set
		{
			this._yearStart = value;
			LifeRecordsController controller = SingletonObject.getInstance<LifeRecordsController>();
			for (int year = this._yearStart; year <= this._yearEnd; year++)
			{
				controller.SetShowingYear(year, this.CharacterId, false);
			}
		}
	}

	// Token: 0x170002E8 RID: 744
	// (get) Token: 0x06001C20 RID: 7200 RVA: 0x000C235F File Offset: 0x000C055F
	private Refers DetailPanel
	{
		get
		{
			return base.CGet<Refers>("DetailPanel");
		}
	}

	// Token: 0x170002E9 RID: 745
	// (get) Token: 0x06001C21 RID: 7201 RVA: 0x000C236C File Offset: 0x000C056C
	private GroupedInfinityScroll GroupedScrollView
	{
		get
		{
			return this.DetailPanel.CGet<GroupedInfinityScroll>("GroupedScrollView");
		}
	}

	// Token: 0x06001C22 RID: 7202 RVA: 0x000C237E File Offset: 0x000C057E
	private void OnEnable()
	{
		this.Init();
		this.Refresh();
	}

	// Token: 0x06001C23 RID: 7203 RVA: 0x000C2390 File Offset: 0x000C0590
	public void Refresh()
	{
		this._isTaiwu = (this.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
		this._focusYear = (this._yearEnd = SingletonObject.getInstance<TimeManager>().GetYear());
		this._scrollDataList.Clear();
		this._scrollGroupList.Clear();
		GEvent.Add(UiEvents.OnCharacterLifeRecordYearReady, new GEvent.Callback(this.OnCharacterLifeRecordYearDataReady));
		this.YearStart = this._yearEnd - this.YearDiff;
	}

	// Token: 0x06001C24 RID: 7204 RVA: 0x000C2414 File Offset: 0x000C0614
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.OnCharacterLifeRecordYearReady, new GEvent.Callback(this.OnCharacterLifeRecordYearDataReady));
	}

	// Token: 0x06001C25 RID: 7205 RVA: 0x000C2430 File Offset: 0x000C0630
	private void Init()
	{
		bool flag = this.isInit;
		if (!flag)
		{
			this.isInit = true;
			this.GroupedScrollView.Init();
			this.GroupedScrollView.OnItemRender = new Action<int, int, Refers>(this.OnItemRender);
			this.GroupedScrollView.OnGroupTitleRender = new Action<int, Refers>(this.OnGroupTitleRender);
			this.GroupedScrollView.UpdateData(this._scrollDataList, true);
			this._charNameFullBtnPoolItem = new PoolItem(string.Empty, base.CGet<GameObject>("FullCharNameButton"));
			this._charNameLeftPartBtnPoolItem = new PoolItem(string.Empty, base.CGet<GameObject>("LeftPartCharNameButton"));
			this._charNameRightPartBtnPoolItem = new PoolItem(string.Empty, base.CGet<GameObject>("RightPartCharNameButton"));
		}
	}

	// Token: 0x06001C26 RID: 7206 RVA: 0x000C24F4 File Offset: 0x000C06F4
	private void OnGroupTitleRender(int groupIndex, Refers refers)
	{
		UI_CharacterMenuLifeRecords.RecordGroup group = this._scrollGroupList[groupIndex];
		refers.CGet<TextMeshProUGUI>("TitleName").text = group.GroupTitleText;
	}

	// Token: 0x06001C27 RID: 7207 RVA: 0x000C2528 File Offset: 0x000C0728
	private void OnItemRender(int groupIndex, int dataIndex, Refers refers)
	{
		string invisibleRecordString = new string('·', 50) + LocalStringManager.Get(LanguageKey.LK_LifeRecord_NeedMoreFavor);
		UI_CharacterMenuLifeRecords.RecordGroup group = this._scrollGroupList[groupIndex];
		bool flag = dataIndex < 0 || dataIndex >= group.MonthData.RecordList.Count;
		if (flag)
		{
			Debug.LogWarning(string.Format("DataIndex out of range,dataIndex:{0},RecordList count:{1}", dataIndex, group.MonthData.RecordList.Count));
		}
		else
		{
			ValueTuple<int, string, string, short> valueTuple = group.MonthData.RecordList[dataIndex];
			int score = valueTuple.Item1;
			string content = valueTuple.Item3;
			short templateId = valueTuple.Item4;
			LifeRecordItem config = LifeRecord.Instance.GetItem(templateId);
			bool flag2 = refers.UserObject == null;
			if (flag2)
			{
				RectTransform btnRoot = refers.CGet<RectTransform>("ButtonRoot");
				CharacterNameClickLinkHandler handler = new CharacterNameClickLinkHandler(btnRoot, this._charNameFullBtnPoolItem, this._charNameLeftPartBtnPoolItem, this._charNameRightPartBtnPoolItem, new Action<int>(this.OnCharacterNameClicked));
				refers.UserObject = handler;
			}
			else
			{
				CharacterNameClickLinkHandler handler = refers.UserObject as CharacterNameClickLinkHandler;
			}
			TMPTextSpriteHelper spriteHelper = refers.CGet<TMPTextSpriteHelper>("SpriteHelper");
			TextMeshProUGUI label = refers.CGet<TextMeshProUGUI>("Content");
			bool flag3 = config != null && config.RequiredFavorability > 0 && this._favorToTaiwu < config.RequiredFavorability;
			if (flag3)
			{
				content = invisibleRecordString;
			}
			bool flag4 = templateId < 0;
			if (flag4)
			{
				bool flag5 = !this._isTaiwu && this._favorToTaiwu < 10000;
				if (flag5)
				{
					content = invisibleRecordString;
				}
			}
			label.text = content;
			spriteHelper.Parse();
		}
	}

	// Token: 0x06001C28 RID: 7208 RVA: 0x000C26C3 File Offset: 0x000C08C3
	private void OnCharacterNameClicked(int charId)
	{
	}

	// Token: 0x06001C29 RID: 7209 RVA: 0x000C26C8 File Offset: 0x000C08C8
	private void UpdateScrollDataList(List<UI_CharacterMenuLifeRecords.RecordGroup> itemGroups)
	{
		this._scrollDataList.Clear();
		foreach (UI_CharacterMenuLifeRecords.RecordGroup itemGroup in itemGroups)
		{
			GroupedInfinityScroll.GroupItem groupItem = new GroupedInfinityScroll.GroupItem(itemGroup.GroupId, itemGroup.MonthData.RecordList.Count);
			this._scrollDataList.Add(groupItem);
		}
	}

	// Token: 0x06001C2A RID: 7210 RVA: 0x000C274C File Offset: 0x000C094C
	private void OnCharacterLifeRecordYearDataReady(ArgumentBox box)
	{
		int charId;
		bool flag = !box.Get("CharacterId", out charId) || charId != this.CharacterId;
		if (!flag)
		{
			this.RefreshYearList();
			int year;
			bool flag2 = !box.Get("Year", out year) || !this._yearList.Contains(year);
			if (!flag2)
			{
				SingletonObject.getInstance<LifeRecordsController>().GetCharacterLifeRecordYearDataList(this.CharacterId, this._yearList, this._yearDataList);
				bool flag3 = this._yearDataList.Count != this._yearList.Count;
				if (!flag3)
				{
					this.RefreshDetailView(true);
				}
			}
		}
	}

	// Token: 0x06001C2B RID: 7211 RVA: 0x000C27F4 File Offset: 0x000C09F4
	private void RefreshYearList()
	{
		this._yearList.Clear();
		for (int i = this._yearStart; i <= this._yearEnd; i++)
		{
			this._yearList.Add(i);
		}
	}

	// Token: 0x06001C2C RID: 7212 RVA: 0x000C2838 File Offset: 0x000C0A38
	private void RefreshDetailView(bool init)
	{
		this.UpdateScrollGroupList(this._yearDataList);
		this.UpdateScrollDataList(this._scrollGroupList);
		base.DelayFrameCall(delegate
		{
			this.GroupedScrollView.UpdateData(this._scrollDataList, false);
		}, 1U);
		this._favorToTaiwu = SingletonObject.getInstance<LifeRecordsController>().GetFavorabilityToTaiwuFromCacheData(this.CharacterId, false);
		bool isTaiwu = this._isTaiwu;
		if (isTaiwu)
		{
			this._favorToTaiwu = 30000;
		}
	}

	// Token: 0x06001C2D RID: 7213 RVA: 0x000C28A0 File Offset: 0x000C0AA0
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

	// Token: 0x040015ED RID: 5613
	private int _yearStart;

	// Token: 0x040015EE RID: 5614
	private int _yearEnd;

	// Token: 0x040015EF RID: 5615
	private int _focusYear;

	// Token: 0x040015F0 RID: 5616
	[Tooltip("yearStart = yearEnd - yearDiff")]
	public int YearDiff = 0;

	// Token: 0x040015F1 RID: 5617
	private readonly List<int> _yearList = new List<int>();

	// Token: 0x040015F2 RID: 5618
	private readonly List<LifeRecordsController.LifeRecordYearData> _yearDataList = new List<LifeRecordsController.LifeRecordYearData>();

	// Token: 0x040015F3 RID: 5619
	private bool isInit;

	// Token: 0x040015F4 RID: 5620
	private readonly List<GroupedInfinityScroll.GroupItem> _scrollDataList = new List<GroupedInfinityScroll.GroupItem>();

	// Token: 0x040015F5 RID: 5621
	private readonly List<UI_CharacterMenuLifeRecords.RecordGroup> _scrollGroupList = new List<UI_CharacterMenuLifeRecords.RecordGroup>();

	// Token: 0x040015F6 RID: 5622
	private short _favorToTaiwu;

	// Token: 0x040015F7 RID: 5623
	private bool _isTaiwu;

	// Token: 0x040015F8 RID: 5624
	private PoolItem _charNameFullBtnPoolItem;

	// Token: 0x040015F9 RID: 5625
	private PoolItem _charNameLeftPartBtnPoolItem;

	// Token: 0x040015FA RID: 5626
	private PoolItem _charNameRightPartBtnPoolItem;
}
