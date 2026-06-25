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
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.LifeRecord;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000BA8 RID: 2984
	public class ViewCharacterMenuLifeRecords : UI_CharacterMenuSubPageBase
	{
		// Token: 0x17001019 RID: 4121
		// (get) Token: 0x0600957C RID: 38268 RVA: 0x0045B4BF File Offset: 0x004596BF
		public override LanguageKey TitleKey
		{
			get
			{
				return LanguageKey.LK_CharacterMenu_Title_LifeRecords;
			}
		}

		// Token: 0x0600957D RID: 38269 RVA: 0x0045B4C8 File Offset: 0x004596C8
		public override bool CheckState(ECharacterSubToggleBase curSubTogglePage, ECharacterSubPage curSubPage)
		{
			return curSubTogglePage == ECharacterSubToggleBase.StoryBase;
		}

		// Token: 0x0600957E RID: 38270 RVA: 0x0045B4E0 File Offset: 0x004596E0
		private void RequestData()
		{
			bool flag = !this._isInitDreamBack;
			if (!flag)
			{
				this.SetLifeRecordLoading(true);
				this.ChangeViewShow(this._isCurrentTaiwuOverwrittenByDreamBack && base.CharacterMenu.CurrentCharacterIsTaiwu);
				this.lifeRecord.Clear();
				this.lifeRecord.Set(this, base.CharacterMenu.CurCharacterId, this.IsCurrentSelectDreamBack());
			}
		}

		// Token: 0x0600957F RID: 38271 RVA: 0x0045B54C File Offset: 0x0045974C
		private void OnSwitchDreamBack(int prevIndex, int curIndex)
		{
			this.RequestData();
		}

		// Token: 0x06009580 RID: 38272 RVA: 0x0045B558 File Offset: 0x00459758
		public override void OnCurrentCharacterChange(int prevCharacterId)
		{
			bool flag = base.CharacterMenu.CurCharacterId < 0;
			if (!flag)
			{
				bool flag2 = base.CharacterMenu.CurCharacterId == prevCharacterId;
				if (!flag2)
				{
					this.RequestData();
				}
			}
		}

		// Token: 0x06009581 RID: 38273 RVA: 0x0045B598 File Offset: 0x00459798
		private bool IsCurrentSelectDreamBack()
		{
			return this.CurrentSelectDreamBack && base.CharacterMenu.CurrentCharacterIsTaiwu;
		}

		// Token: 0x06009582 RID: 38274 RVA: 0x0045B5C0 File Offset: 0x004597C0
		public override void OnSubpageInVisible()
		{
			this.lifeRecordDateSelector.OnDisable();
		}

		// Token: 0x1700101A RID: 4122
		// (get) Token: 0x06009583 RID: 38275 RVA: 0x0045B5CF File Offset: 0x004597CF
		// (set) Token: 0x06009584 RID: 38276 RVA: 0x0045B5DC File Offset: 0x004597DC
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

		// Token: 0x06009585 RID: 38277 RVA: 0x0045B60C File Offset: 0x0045980C
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
			bool flag = !this._firstEnter;
			if (flag)
			{
				this._firstEnter = true;
				GlobalDomainMethod.Call.InvokeGuidingTrigger(118);
			}
			this._isInitDreamBack = false;
			this.switchDreamBack.SetWithoutNotify(0);
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
			{
				this.SetLifeRecordLoading(true);
				ExtraDomainMethod.AsyncCall.IsCurrentTaiwuOverwrittenByDreamBack(this, delegate(int offset, RawDataPool dataPool)
				{
					Serializer.Deserialize(dataPool, offset, ref this._isCurrentTaiwuOverwrittenByDreamBack);
					this._isInitDreamBack = true;
					this.RequestData();
				});
			}));
		}

		// Token: 0x1700101B RID: 4123
		// (get) Token: 0x06009586 RID: 38278 RVA: 0x0045B77C File Offset: 0x0045997C
		private bool CurrentSelectDreamBack
		{
			get
			{
				return this._isCurrentTaiwuOverwrittenByDreamBack && this.switchDreamBack.gameObject.activeSelf && this.switchDreamBack.GetActiveIndex() == 1;
			}
		}

		// Token: 0x06009587 RID: 38279 RVA: 0x0045B7AC File Offset: 0x004599AC
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
			this.switchDreamBack.Init(-1);
			this.switchDreamBack.OnActiveIndexChange += this.OnSwitchDreamBack;
			this.switchDreamBack.gameObject.SetActive(false);
		}

		// Token: 0x06009588 RID: 38280 RVA: 0x0045B8F8 File Offset: 0x00459AF8
		private void SetLifeRecordLoading(bool isLoading)
		{
			this.localLoadingAnim.SetLoadingState(isLoading);
			if (isLoading)
			{
				this.localLoadingAnim.EnsureContentActiveForLayout();
			}
		}

		// Token: 0x06009589 RID: 38281 RVA: 0x0045B924 File Offset: 0x00459B24
		private void Update()
		{
			this.currentlyHeight = this.lifeRecordRect.rect.height;
			bool flag = !this.isForceRebuildLayout || !Mathf.Approximately(this.lifeRecordHeight, this.currentlyHeight);
			if (!flag)
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
					this.lifeRecordScrollRect.verticalNormalizedPosition = num + 0.01f;
				}
				this.lifeRecordHeight = this.currentlyHeight;
				this.isForceRebuildLayout = false;
			}
		}

		// Token: 0x0600958A RID: 38282 RVA: 0x0045B9E9 File Offset: 0x00459BE9
		private void ChangeViewShow(bool isDreamBack)
		{
			this.switchDreamBack.gameObject.SetActive(isDreamBack);
		}

		// Token: 0x0600958B RID: 38283 RVA: 0x0045BA00 File Offset: 0x00459C00
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

		// Token: 0x0600958C RID: 38284 RVA: 0x0045BAF8 File Offset: 0x00459CF8
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

		// Token: 0x0600958D RID: 38285 RVA: 0x0045BB84 File Offset: 0x00459D84
		private void OnAllRecordReceived(RenderedRecordDataList<RenderedRecordData> dataList)
		{
			this.SetLifeRecordLoading(false);
			bool flag = dataList.Data.Record.Count < ViewCharacterMenuLifeRecords.PageSize || dataList.Data.Record[ViewCharacterMenuLifeRecords.PageSize - 1].Date < dataList.Data.EndDate - 108;
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
				ViewCharacterMenuLifeRecords.SummaryData summaryData = new ViewCharacterMenuLifeRecords.SummaryData(dataList.Data.EndDate);
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

		// Token: 0x0600958E RID: 38286 RVA: 0x0045BD2C File Offset: 0x00459F2C
		private IEnumerator ProcessAllRecordReceived(RenderedRecordDataList<RenderedRecordData> dataList)
		{
			ViewCharacterMenuLifeRecords.SummaryData summaryData = new ViewCharacterMenuLifeRecords.SummaryData(dataList.Data.EndDate);
			int index = 0;
			while (index + ViewCharacterMenuLifeRecords.PageSize < dataList.Data.Record.Count)
			{
				bool flag = summaryData.ReadData(dataList.Data.Record.GetRange(index, ViewCharacterMenuLifeRecords.PageSize), dataList.Data.ArgumentCollection);
				if (flag)
				{
					this.lifeRecordSummary.Set(summaryData.Years, summaryData.Scores, summaryData.Type);
					yield break;
				}
				index += ViewCharacterMenuLifeRecords.PageSize;
				yield return new WaitForEndOfFrame();
			}
			summaryData.RecordYearData();
			this.lifeRecordSummary.Set(summaryData.Years, summaryData.Scores, summaryData.Type);
			yield break;
		}

		// Token: 0x0600958F RID: 38287 RVA: 0x0045BD42 File Offset: 0x00459F42
		private new void OnDisable()
		{
			this.lifeRecord.Clear();
		}

		// Token: 0x040072A9 RID: 29353
		[SerializeField]
		private CToggleGroup switchDreamBack;

		// Token: 0x040072AA RID: 29354
		[SerializeField]
		private CImage isImportantcheckMark;

		// Token: 0x040072AB RID: 29355
		[SerializeField]
		private CToggle isImportantCheckBox;

		// Token: 0x040072AC RID: 29356
		[SerializeField]
		private Game.Components.Character.LifeRecord.LifeRecord lifeRecord;

		// Token: 0x040072AD RID: 29357
		[SerializeField]
		private LifeRecordSummary lifeRecordSummary;

		// Token: 0x040072AE RID: 29358
		[SerializeField]
		private LifeRecordDateSelector lifeRecordDateSelector;

		// Token: 0x040072AF RID: 29359
		[SerializeField]
		private FilterDropdown filterDropdown;

		// Token: 0x040072B0 RID: 29360
		[SerializeField]
		private RectTransform lifeRecordRect;

		// Token: 0x040072B1 RID: 29361
		[SerializeField]
		private RectTransform lifeRecordContentRect;

		// Token: 0x040072B2 RID: 29362
		[SerializeField]
		private LoopVerticalScrollRect lifeRecordScrollRect;

		// Token: 0x040072B3 RID: 29363
		[SerializeField]
		private CharacterMenuLocalLoadingAnim localLoadingAnim;

		// Token: 0x040072B4 RID: 29364
		public const short FavorToSeeBornRecord = 10000;

		// Token: 0x040072B5 RID: 29365
		private float lifeRecordHeight;

		// Token: 0x040072B6 RID: 29366
		private float currentlyHeight;

		// Token: 0x040072B7 RID: 29367
		private bool isForceRebuildLayout;

		// Token: 0x040072B8 RID: 29368
		private bool _firstEnter;

		// Token: 0x040072B9 RID: 29369
		private bool _isInitDreamBack;

		// Token: 0x040072BA RID: 29370
		private bool _isCurrentTaiwuOverwrittenByDreamBack;

		// Token: 0x040072BB RID: 29371
		private static int PageSize = 1000;

		// Token: 0x040072BC RID: 29372
		private readonly HashSet<int> _date = new HashSet<int>();

		// Token: 0x02002220 RID: 8736
		public class SummaryData
		{
			// Token: 0x0600FD4B RID: 64843 RVA: 0x0063E018 File Offset: 0x0063C218
			public SummaryData(int date)
			{
				this.NextDate = date - date % 12;
			}

			// Token: 0x0600FD4C RID: 64844 RVA: 0x0063E080 File Offset: 0x0063C280
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

			// Token: 0x0600FD4D RID: 64845 RVA: 0x0063E294 File Offset: 0x0063C494
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

			// Token: 0x0400D836 RID: 55350
			public int NextDate;

			// Token: 0x0400D837 RID: 55351
			public int DataCount;

			// Token: 0x0400D838 RID: 55352
			public int YearScore;

			// Token: 0x0400D839 RID: 55353
			[TupleElementNames(new string[]
			{
				"Name",
				"Score"
			})]
			public ValueTuple<string, int> Positive = new ValueTuple<string, int>("", 49);

			// Token: 0x0400D83A RID: 55354
			[TupleElementNames(new string[]
			{
				"Name",
				"Score"
			})]
			public ValueTuple<string, int> Negative = new ValueTuple<string, int>("", 51);

			// Token: 0x0400D83B RID: 55355
			public List<int> Years = new List<int>();

			// Token: 0x0400D83C RID: 55356
			public List<int> Scores = new List<int>();

			// Token: 0x0400D83D RID: 55357
			public List<string> Type = new List<string>();
		}
	}
}
