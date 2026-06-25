using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Character.LifeRecord;
using Game.Components.SortAndFilter;
using GameData.Domains.LifeRecord;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000BB0 RID: 2992
	public class ViewLifeRecords : UIBase
	{
		// Token: 0x06009685 RID: 38533 RVA: 0x004638DC File Offset: 0x00461ADC
		public static void Show(int charId)
		{
			bool flag = SingletonObject.getInstance<WorldMapModel>().IsAtSecretVillage();
			if (!flag)
			{
				UIElement.LifeRecords.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharId", charId));
				UIManager.Instance.MaskUI(UIElement.LifeRecords);
			}
		}

		// Token: 0x1700102F RID: 4143
		// (get) Token: 0x06009686 RID: 38534 RVA: 0x00463925 File Offset: 0x00461B25
		// (set) Token: 0x06009687 RID: 38535 RVA: 0x00463934 File Offset: 0x00461B34
		public bool IsSimple
		{
			get
			{
				return this.lifeRecordSummary.IsSimple;
			}
			set
			{
				bool flag = this.IsSimple == value;
				if (!flag)
				{
					this.lifeRecordSummary.IsSimple = !value;
				}
			}
		}

		// Token: 0x06009688 RID: 38536 RVA: 0x00463964 File Offset: 0x00461B64
		public override void OnInit(ArgumentBox argsBox)
		{
			this.lifeRecord.Clear();
			this.filterDropdown.Setup(new FilterDropdownConfig
			{
				MenuBarLabel = LanguageKey.LK_Resource_Choosy_ItemMenu,
				ItemConfigs = new List<FilterDropdownItemConfig>
				{
					new FilterDropdownItemConfig(LanguageKey.LK_LifeRecord_DisplayType_Great),
					new FilterDropdownItemConfig(LanguageKey.LK_LifeRecord_DisplayType_Normal),
					new FilterDropdownItemConfig(LanguageKey.LK_LifeRecord_DisplayType_Relation),
					new FilterDropdownItemConfig(LanguageKey.LK_LifeRecord_DisplayType_Study),
					new FilterDropdownItemConfig(LanguageKey.LK_LifeRecord_DisplayType_Produce),
					new FilterDropdownItemConfig(LanguageKey.LK_LifeRecord_DisplayType_Combat),
					new FilterDropdownItemConfig(LanguageKey.LK_LifeRecord_DisplayType_Negative),
					new FilterDropdownItemConfig(LanguageKey.LK_LifeRecord_DisplayType_Crime)
				},
				DefaultSelectedIndices = new HashSet<int>(),
				IsMultiSelect = true
			});
			argsBox.Get("CharId", out this._charId);
			this.lifeRecord.Set(this, this._charId, false);
		}

		// Token: 0x06009689 RID: 38537 RVA: 0x00463A9C File Offset: 0x00461C9C
		private void Awake()
		{
			bool isImportantOnly = this.lifeRecord.LifeRecordDisplayType == (Game.Components.Character.LifeRecord.LifeRecord.DisplayType.None | Game.Components.Character.LifeRecord.LifeRecord.DisplayType.Great);
			this.isImportantCheckBox.SetIsOnWithoutNotify(isImportantOnly);
			this.isImportantcheckMark.gameObject.SetActive(isImportantOnly);
			this.isImportantCheckBox.onValueChanged.RemoveAllListeners();
			this.isImportantCheckBox.onValueChanged.AddListener(delegate(bool x)
			{
				this.SetFilter(x, this.filterDropdown.GetSelectedIndices());
			});
			this.lifeRecord.OnAllRecordReceived = new Action<RenderedRecordDataList<RenderedRecordData>>(this.OnAllRecordReceived);
			this.lifeRecord.AddScrollEvent(new Action<int>(this.lifeRecordDateSelector.Set));
			this.lifeRecord.AddScrollEvent(new Action<int>(this.lifeRecordSummary.SetDate));
			this.lifeRecord.AddScrollEvent(new Action<int>(this.RefreshLifeRecordHeight));
			this.lifeRecordDateSelector.DateHasData = new Func<int, bool>(this._date.Contains);
			this.filterDropdown.OnSelectionChanged = delegate()
			{
				this.SetFilter(this.isImportantCheckBox.isOn, this.filterDropdown.GetSelectedIndices());
			};
			this.lifeRecordHeight = -1f;
		}

		// Token: 0x0600968A RID: 38538 RVA: 0x00463BAE File Offset: 0x00461DAE
		private void OnEnable()
		{
			GEvent.Add(UiEvents.OnLifeRecordNameButtonClicked, new GEvent.Callback(this.OnLifeRecordNameButtonClicked));
		}

		// Token: 0x0600968B RID: 38539 RVA: 0x00463BCA File Offset: 0x00461DCA
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.OnLifeRecordNameButtonClicked, new GEvent.Callback(this.OnLifeRecordNameButtonClicked));
			this.lifeRecord.Clear();
		}

		// Token: 0x0600968C RID: 38540 RVA: 0x00463BF2 File Offset: 0x00461DF2
		private void OnLifeRecordNameButtonClicked(ArgumentBox _)
		{
			this.QuickHide();
		}

		// Token: 0x0600968D RID: 38541 RVA: 0x00463BFC File Offset: 0x00461DFC
		private void Update()
		{
			this.currentlyHeight = this.lifeRecordRect.rect.height;
			bool flag = this.isForceRebuildLayout && Mathf.Approximately(this.lifeRecordHeight, this.currentlyHeight);
			if (flag)
			{
				float num = this.lifeRecordScrollRect.verticalNormalizedPosition;
				bool flag2 = num >= 1f;
				if (flag2)
				{
					bool flag3 = this.lifeRecordContentRect.rect.height < this.currentlyHeight;
					if (flag3)
					{
						this.lifeRecordScrollRect.verticalNormalizedPosition = 0.98f;
					}
				}
				else
				{
					this.isForceRebuildLayout = false;
					this.lifeRecordScrollRect.verticalNormalizedPosition = num + 0.01f;
				}
			}
		}

		// Token: 0x0600968E RID: 38542 RVA: 0x00463CB4 File Offset: 0x00461EB4
		private void SetFilter(bool isImportant, IReadOnlyCollection<int> selectedIndices)
		{
			this.isImportantcheckMark.gameObject.SetActive(isImportant);
			if (isImportant)
			{
				this.lifeRecord.LifeRecordDisplayType = (Game.Components.Character.LifeRecord.LifeRecord.DisplayType.None | Game.Components.Character.LifeRecord.LifeRecord.DisplayType.Great);
			}
			else
			{
				bool flag = ((selectedIndices != null) ? selectedIndices.Count : 0) == 0;
				if (flag)
				{
					this.lifeRecord.LifeRecordDisplayType = Game.Components.Character.LifeRecord.LifeRecord.DisplayType.All;
				}
				else
				{
					this.lifeRecord.LifeRecordDisplayType = selectedIndices.Aggregate(Game.Components.Character.LifeRecord.LifeRecord.DisplayType.None, (Game.Components.Character.LifeRecord.LifeRecord.DisplayType current, int item) => current | (Game.Components.Character.LifeRecord.LifeRecord.DisplayType)(1 << item + 1));
				}
			}
			this._date.Clear();
			foreach (TransferableRecord item2 in this.lifeRecord.DataList.Filtered)
			{
				this._date.Add(item2.Date);
			}
		}

		// Token: 0x0600968F RID: 38543 RVA: 0x00463DAC File Offset: 0x00461FAC
		private void RefreshLifeRecordHeight(int obj)
		{
			this.currentlyHeight = this.lifeRecordRect.rect.height;
			bool flag = Mathf.Approximately(this.lifeRecordHeight, -1f);
			if (flag)
			{
				this.lifeRecordHeight = this.lifeRecordRect.rect.height;
				this.isForceRebuildLayout = false;
			}
			else
			{
				bool flag2 = !this.isForceRebuildLayout && !Mathf.Approximately(this.lifeRecordHeight, this.currentlyHeight);
				if (flag2)
				{
					this.isForceRebuildLayout = true;
				}
			}
		}

		// Token: 0x06009690 RID: 38544 RVA: 0x00463E38 File Offset: 0x00462038
		private void OnAllRecordReceived(RenderedRecordDataList<RenderedRecordData> dataList)
		{
			bool flag = dataList.Data.Record.Count < ViewLifeRecords.PageSize || dataList.Data.Record[ViewLifeRecords.PageSize - 1].Date < dataList.Data.EndDate - 108;
			if (flag)
			{
				bool flag2 = dataList.Data.Record.Count == 0;
				if (flag2)
				{
					this.lifeRecordSummary.Set(new int[]
					{
						1
					}, new int[]
					{
						-1
					}, new string[]
					{
						LanguageKey.LK_LifeRecord_None.Tr()
					});
					return;
				}
				ViewLifeRecords.SummaryData summaryData = new ViewLifeRecords.SummaryData(dataList.Data.EndDate);
				bool flag3 = !summaryData.ReadData(dataList.Data.Record, dataList.Data.ArgumentCollection);
				if (flag3)
				{
					summaryData.RecordYearData();
				}
				this.lifeRecordSummary.Set(summaryData.Years, summaryData.Scores, summaryData.Type);
			}
			else
			{
				base.StartCoroutine(this.ProcessAllRecordReceived(dataList));
			}
			this._date.Clear();
			foreach (TransferableRecord item in dataList.Filtered)
			{
				this._date.Add(item.Date);
			}
			this.lifeRecordDateSelector.Set(dataList.Data.StartDate, dataList.Data.EndDate, this.lifeRecord);
		}

		// Token: 0x06009691 RID: 38545 RVA: 0x00463FD8 File Offset: 0x004621D8
		private IEnumerator ProcessAllRecordReceived(RenderedRecordDataList<RenderedRecordData> dataList)
		{
			ViewLifeRecords.SummaryData summaryData = new ViewLifeRecords.SummaryData(dataList.Data.EndDate);
			int index = 0;
			while (index + ViewLifeRecords.PageSize < dataList.Data.Record.Count)
			{
				bool flag = summaryData.ReadData(dataList.Data.Record.GetRange(index, ViewLifeRecords.PageSize), dataList.Data.ArgumentCollection);
				if (flag)
				{
					this.lifeRecordSummary.Set(summaryData.Years, summaryData.Scores, summaryData.Type);
					yield break;
				}
				index += ViewLifeRecords.PageSize;
				yield return new WaitForEndOfFrame();
			}
			summaryData.RecordYearData();
			this.lifeRecordSummary.Set(summaryData.Years, summaryData.Scores, summaryData.Type);
			this.Element.ShowAfterRefresh();
			yield break;
		}

		// Token: 0x04007379 RID: 29561
		[SerializeField]
		private CImage isImportantcheckMark;

		// Token: 0x0400737A RID: 29562
		[SerializeField]
		private CToggle isImportantCheckBox;

		// Token: 0x0400737B RID: 29563
		[SerializeField]
		private Game.Components.Character.LifeRecord.LifeRecord lifeRecord;

		// Token: 0x0400737C RID: 29564
		[SerializeField]
		private LifeRecordSummary lifeRecordSummary;

		// Token: 0x0400737D RID: 29565
		[SerializeField]
		private LifeRecordDateSelector lifeRecordDateSelector;

		// Token: 0x0400737E RID: 29566
		[SerializeField]
		private FilterDropdown filterDropdown;

		// Token: 0x0400737F RID: 29567
		[SerializeField]
		private RectTransform lifeRecordRect;

		// Token: 0x04007380 RID: 29568
		[SerializeField]
		private RectTransform lifeRecordContentRect;

		// Token: 0x04007381 RID: 29569
		[SerializeField]
		private LoopVerticalScrollRect lifeRecordScrollRect;

		// Token: 0x04007382 RID: 29570
		public const short FavorToSeeBornRecord = 10000;

		// Token: 0x04007383 RID: 29571
		private float lifeRecordHeight;

		// Token: 0x04007384 RID: 29572
		private float currentlyHeight;

		// Token: 0x04007385 RID: 29573
		private bool isForceRebuildLayout;

		// Token: 0x04007386 RID: 29574
		private int _charId;

		// Token: 0x04007387 RID: 29575
		private static int PageSize = 1000;

		// Token: 0x04007388 RID: 29576
		private readonly HashSet<int> _date = new HashSet<int>();

		// Token: 0x02002251 RID: 8785
		public class SummaryData
		{
			// Token: 0x0600FDD3 RID: 64979 RVA: 0x00641578 File Offset: 0x0063F778
			public SummaryData(int date)
			{
				this.NextDate = date - date % 12;
			}

			// Token: 0x0600FDD4 RID: 64980 RVA: 0x006415E0 File Offset: 0x0063F7E0
			public bool ReadData(IList<TransferableRecord> list, TransferableArgumentCollection argumentCollection)
			{
				foreach (TransferableRecord record in list)
				{
					while (record.Date <= this.NextDate)
					{
						this.RecordYearData();
					}
					bool flag = record.RecordType < 0;
					if (!flag)
					{
						LifeRecordItem cfgItem = Config.LifeRecord.Instance[record.RecordType];
						bool flag2 = cfgItem != null && cfgItem.ScoreType == ELifeRecordScoreType.Absolute;
						if (flag2)
						{
							bool flag3 = this.DataCount < 0;
							if (flag3)
							{
								this.YearScore = Math.Min(this.YearScore, (int)cfgItem.Score);
								bool flag4 = this.YearScore == (int)cfgItem.Score;
								if (flag4)
								{
									List<string> type = this.Type;
									type[type.Count - 1] = cfgItem.Name;
								}
							}
							else
							{
								this.DataCount = int.MinValue;
								this.YearScore = (int)cfgItem.Score;
								this.Type.Add(cfgItem.Name);
							}
						}
						bool flag5 = this.DataCount < 0;
						if (!flag5)
						{
							this.DataCount++;
							int score = record.GetCalculatedLifeRecordScore(argumentCollection);
							this.YearScore += score;
							bool flag6 = score > this.Positive.Item2;
							if (flag6)
							{
								LifeRecordItem lifeRecordItem = Config.LifeRecord.Instance[record.RecordType];
								this.Positive = new ValueTuple<string, int>(((lifeRecordItem != null) ? lifeRecordItem.Name : null) ?? this.Positive.Item1, score);
							}
							else
							{
								bool flag7 = score < this.Negative.Item2;
								if (flag7)
								{
									LifeRecordItem lifeRecordItem2 = Config.LifeRecord.Instance[record.RecordType];
									this.Negative = new ValueTuple<string, int>(((lifeRecordItem2 != null) ? lifeRecordItem2.Name : null) ?? this.Negative.Item1, score);
								}
							}
						}
					}
				}
				return false;
			}

			// Token: 0x0600FDD5 RID: 64981 RVA: 0x006417F4 File Offset: 0x0063F9F4
			public void RecordYearData()
			{
				bool flag = this.DataCount < 0;
				if (flag)
				{
					this.Scores.Add(this.YearScore);
					this.Years.Add(this.NextDate / 12 + 1);
				}
				else
				{
					bool flag2 = this.DataCount == 0;
					if (flag2)
					{
						this.Scores.Add(-1);
						this.Type.Add(LanguageKey.LK_LifeRecord_None.Tr());
						this.Years.Add(this.NextDate / 12 + 1);
					}
					else
					{
						int score = this.YearScore / this.DataCount;
						this.Scores.Add(score);
						this.Type.Add((score > 50) ? this.Positive.Item1 : ((score < 50) ? this.Negative.Item1 : ((this.Positive.Item2 > 100 - this.Negative.Item2) ? this.Positive.Item1 : this.Negative.Item1)));
						this.Years.Add(this.NextDate / 12 + 1);
					}
				}
				this.NextDate -= 12;
				this.Positive = new ValueTuple<string, int>("", 49);
				this.Negative = new ValueTuple<string, int>("", 51);
				this.DataCount = (this.YearScore = 0);
			}

			// Token: 0x0400D928 RID: 55592
			public int NextDate;

			// Token: 0x0400D929 RID: 55593
			public int DataCount;

			// Token: 0x0400D92A RID: 55594
			public int YearScore;

			// Token: 0x0400D92B RID: 55595
			[TupleElementNames(new string[]
			{
				"Name",
				"Score"
			})]
			public ValueTuple<string, int> Positive = new ValueTuple<string, int>("", 49);

			// Token: 0x0400D92C RID: 55596
			[TupleElementNames(new string[]
			{
				"Name",
				"Score"
			})]
			public ValueTuple<string, int> Negative = new ValueTuple<string, int>("", 51);

			// Token: 0x0400D92D RID: 55597
			public List<int> Years = new List<int>();

			// Token: 0x0400D92E RID: 55598
			public List<int> Scores = new List<int>();

			// Token: 0x0400D92F RID: 55599
			public List<string> Type = new List<string>();
		}
	}
}
