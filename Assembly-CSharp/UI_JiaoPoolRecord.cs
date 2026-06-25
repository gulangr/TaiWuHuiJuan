using System;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.Components;
using GameData.Common;
using GameData.DLC.FiveLoong;
using GameData.Domains.Extra;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameDataExtensions;
using TMPro;
using UnityEngine;

// Token: 0x020001B7 RID: 439
public class UI_JiaoPoolRecord : UIBase
{
	// Token: 0x170002B9 RID: 697
	// (get) Token: 0x06001A45 RID: 6725 RVA: 0x000AD7C0 File Offset: 0x000AB9C0
	private InfinityScroll RecordScroll
	{
		get
		{
			return base.CGet<InfinityScroll>("VerticalScrollView");
		}
	}

	// Token: 0x170002BA RID: 698
	// (get) Token: 0x06001A46 RID: 6726 RVA: 0x000AD7CD File Offset: 0x000AB9CD
	private RectTransform Arrow
	{
		get
		{
			return base.CGet<RectTransform>("Arrow");
		}
	}

	// Token: 0x170002BB RID: 699
	// (get) Token: 0x06001A47 RID: 6727 RVA: 0x000AD7DA File Offset: 0x000AB9DA
	private GameObject ButtonFoldoutObj
	{
		get
		{
			return base.CGet<GameObject>("ButtonFoldout");
		}
	}

	// Token: 0x170002BC RID: 700
	// (get) Token: 0x06001A48 RID: 6728 RVA: 0x000AD7E7 File Offset: 0x000AB9E7
	private RectTransform FoldPanel
	{
		get
		{
			return base.CGet<RectTransform>("FoldPanel");
		}
	}

	// Token: 0x170002BD RID: 701
	// (get) Token: 0x06001A49 RID: 6729 RVA: 0x000AD7F4 File Offset: 0x000AB9F4
	private TextMeshProUGUI TitleContent
	{
		get
		{
			return base.CGet<TextMeshProUGUI>("TitleContent");
		}
	}

	// Token: 0x170002BE RID: 702
	// (get) Token: 0x06001A4A RID: 6730 RVA: 0x000AD801 File Offset: 0x000ABA01
	private GameObject EmptyObj
	{
		get
		{
			return base.CGet<GameObject>("Empty");
		}
	}

	// Token: 0x06001A4B RID: 6731 RVA: 0x000AD80E File Offset: 0x000ABA0E
	public override void OnInit(ArgumentBox argsBox)
	{
		this.RecordScroll.OnItemRender += this.OnRecordItemRender;
		this.FoldPanel.gameObject.SetActive(false);
		this._foldState = false;
		this.RefreshBtnFold();
	}

	// Token: 0x06001A4C RID: 6732 RVA: 0x000AD849 File Offset: 0x000ABA49
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(19, 96, ulong.MaxValue, null));
	}

	// Token: 0x06001A4D RID: 6733 RVA: 0x000AD864 File Offset: 0x000ABA64
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b != 0)
			{
				if (b == 1)
				{
					bool flag = notification.DomainId == 19 && notification.MethodId == 105;
					if (flag)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._jiaoPoolRecordArgumentCollection);
						this._jiaoPoolRecordArgumentCollection.InitMap();
						this.RefreshRecord();
						this.RefreshBtnFold();
						this.Element.ShowAfterRefresh();
					}
				}
			}
			else
			{
				DataUid uid = notification.Uid;
				ushort dataId = uid.DataId;
				ushort num = dataId;
				if (num == 96)
				{
					List<JiaoPoolRecordList> list = null;
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref list);
					bool flag2 = this._jiaoPoolRecordList == null;
					if (flag2)
					{
						this._jiaoPoolRecordList = new JiaoPoolRecordList();
					}
					this._jiaoPoolRecordList.Collection = new List<JiaoPoolRecord>();
					foreach (JiaoPoolRecordList recordList in list)
					{
						this._jiaoPoolRecordList.Collection.AddRange(recordList.Collection);
					}
					this._jiaoPoolRecordList.Collection.Sort(delegate(JiaoPoolRecord left, JiaoPoolRecord right)
					{
						bool flag3 = left.Date != right.Date;
						int result;
						if (flag3)
						{
							result = left.Date - right.Date;
						}
						else
						{
							bool flag4 = left.RecordTemplateId != right.RecordTemplateId;
							if (flag4)
							{
								result = (int)(left.RecordTemplateId - right.RecordTemplateId);
							}
							else
							{
								result = left.Jiao1Id - right.Jiao1Id;
							}
						}
						return result;
					});
					this.RequestDecodeData();
				}
			}
		}
	}

	// Token: 0x06001A4E RID: 6734 RVA: 0x000ADA44 File Offset: 0x000ABC44
	protected override void OnClick(Transform btn)
	{
		bool flag = btn.name == "ButtonFoldout";
		if (flag)
		{
			this.SwitchFoldState();
		}
	}

	// Token: 0x06001A4F RID: 6735 RVA: 0x000ADA6F File Offset: 0x000ABC6F
	private void OnEnable()
	{
		GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUIChanged));
	}

	// Token: 0x06001A50 RID: 6736 RVA: 0x000ADA8A File Offset: 0x000ABC8A
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUIChanged));
	}

	// Token: 0x06001A51 RID: 6737 RVA: 0x000ADAA8 File Offset: 0x000ABCA8
	private void RequestDecodeData()
	{
		if (this._jiaoPoolRecordArgumentCollection == null)
		{
			this._jiaoPoolRecordArgumentCollection = new JiaoPoolRecordArgumentCollection();
		}
		this._jiaoPoolRecordArgumentCollection.JiaoIdList = new List<int>();
		foreach (JiaoPoolRecord jiaoPoolRecord in this._jiaoPoolRecordList.Collection)
		{
			bool flag = jiaoPoolRecord.Jiao1Id >= 0 && !this._jiaoPoolRecordArgumentCollection.JiaoIdList.Contains(jiaoPoolRecord.Jiao1Id);
			if (flag)
			{
				this._jiaoPoolRecordArgumentCollection.JiaoIdList.Add(jiaoPoolRecord.Jiao1Id);
			}
			bool flag2 = jiaoPoolRecord.Jiao2Id >= 0 && !this._jiaoPoolRecordArgumentCollection.JiaoIdList.Contains(jiaoPoolRecord.Jiao2Id);
			if (flag2)
			{
				this._jiaoPoolRecordArgumentCollection.JiaoIdList.Add(jiaoPoolRecord.Jiao2Id);
			}
		}
		this._jiaoPoolRecordArgumentCollection.JiaoNameList = null;
		this._jiaoPoolRecordArgumentCollection.JiaoNameMap = null;
		ExtraDomainMethod.Call.FillJiaoRecordArgumentCollection(this.Element.GameDataListenerId, this._jiaoPoolRecordArgumentCollection);
	}

	// Token: 0x06001A52 RID: 6738 RVA: 0x000ADBD8 File Offset: 0x000ABDD8
	private void OnTopUIChanged(ArgumentBox box)
	{
		bool shouldShowLog = UIManager.Instance.IsFocusElement(UIElement.BuildingJiaoPool);
		base.transform.GetChild(0).gameObject.SetActive(shouldShowLog);
	}

	// Token: 0x06001A53 RID: 6739 RVA: 0x000ADC10 File Offset: 0x000ABE10
	private void SwitchFoldState()
	{
		this.EmptyObj.gameObject.SetActive(false);
		this._foldState = !this._foldState;
		CommonUtils.TryKillTween(this._switchFoldTweener, false);
		this._switchFoldTweener = DOVirtual.Float(0f, 1f, 0.3f, delegate(float stepValue)
		{
			this.FoldPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(this._foldState ? 0f : 350f, this._foldState ? 350f : 0f, stepValue));
		}).OnComplete(delegate
		{
			bool flag2 = !this._foldState;
			if (flag2)
			{
				this.FoldPanel.gameObject.SetActive(false);
				this.RefreshRecord();
			}
			else
			{
				this.RecordScroll.InitPageCount();
				this.RefreshRecord();
				this.RecordScroll.ScrollToEnd();
			}
		});
		this.RefreshBtnFold();
		bool flag = this._foldState && !this.FoldPanel.gameObject.activeSelf;
		if (flag)
		{
			this.FoldPanel.gameObject.SetActive(true);
		}
	}

	// Token: 0x06001A54 RID: 6740 RVA: 0x000ADCC0 File Offset: 0x000ABEC0
	private void RefreshBtnFold()
	{
		this.Arrow.DOLocalRotate(new Vector3(0f, 0f, (float)(this._foldState ? 180 : 0)), 0.3f, RotateMode.Fast);
		this.ButtonFoldoutObj.gameObject.SetActive(this._jiaoPoolRecordList != null && this._jiaoPoolRecordList.Collection.Count > 0);
	}

	// Token: 0x06001A55 RID: 6741 RVA: 0x000ADD30 File Offset: 0x000ABF30
	private void RebuildDisplayItems()
	{
		this._displayItems.Clear();
		JiaoPoolRecordList jiaoPoolRecordList = this._jiaoPoolRecordList;
		bool flag = ((jiaoPoolRecordList != null) ? jiaoPoolRecordList.Collection : null) == null;
		if (!flag)
		{
			int lastDate = int.MinValue;
			for (int i = 0; i < this._jiaoPoolRecordList.Collection.Count; i++)
			{
				JiaoPoolRecord record = this._jiaoPoolRecordList.Collection[i];
				bool flag2 = record.Date != lastDate;
				if (flag2)
				{
					this._displayItems.Add(new UI_JiaoPoolRecord.DisplayItem
					{
						IsDateRow = true,
						RecordIndex = i
					});
					lastDate = record.Date;
				}
				this._displayItems.Add(new UI_JiaoPoolRecord.DisplayItem
				{
					IsDateRow = false,
					RecordIndex = i
				});
			}
		}
	}

	// Token: 0x06001A56 RID: 6742 RVA: 0x000ADE10 File Offset: 0x000AC010
	private static string GetRecordDateText(int date)
	{
		TimeManager timeManager = SingletonObject.getInstance<TimeManager>();
		MonthItem monthConfig = Month.Instance.GetItem(timeManager.GetMonthInYear(date));
		return timeManager.GetDateDisplayContent(date) + monthConfig.Name;
	}

	// Token: 0x06001A57 RID: 6743 RVA: 0x000ADE4C File Offset: 0x000AC04C
	private void RefreshRecord()
	{
		this.RebuildDisplayItems();
		bool foldState = this._foldState;
		if (foldState)
		{
			this.TitleContent.text = LocalStringManager.Get(LanguageKey.LK_JiaoPoolRecord_Title);
			this.RecordScroll.SetDataCount(this._displayItems.Count);
			this.EmptyObj.gameObject.SetActive(false);
		}
		else
		{
			string content = this._jiaoPoolRecordList.DecodeRecord(this._jiaoPoolRecordList.Collection.Count - 1, this._jiaoPoolRecordArgumentCollection).Item2.ColorReplace();
			bool flag = string.IsNullOrEmpty(content);
			if (flag)
			{
				content = LocalStringManager.Get(LanguageKey.LK_JiaoPoolRecord_Title_None);
			}
			this.TitleContent.text = content;
			this.EmptyObj.gameObject.SetActive(true);
		}
	}

	// Token: 0x06001A58 RID: 6744 RVA: 0x000ADF14 File Offset: 0x000AC114
	private void OnRecordItemRender(int index, GameObject refers)
	{
		Refers itemRefers = refers.GetComponent<Refers>();
		TextMeshProUGUI content = itemRefers.CGet<TextMeshProUGUI>("Content");
		GameObject titleBg = itemRefers.CGet<GameObject>("TitleBg");
		bool flag = !this._displayItems.CheckIndex(index);
		if (flag)
		{
			content.text = string.Empty;
			titleBg.SetActive(false);
		}
		else
		{
			UI_JiaoPoolRecord.DisplayItem displayItem = this._displayItems[index];
			bool isDateRow = displayItem.IsDateRow;
			if (isDateRow)
			{
				content.text = UI_JiaoPoolRecord.GetRecordDateText(this._jiaoPoolRecordList.Collection[displayItem.RecordIndex].Date);
				titleBg.SetActive(true);
			}
			else
			{
				content.text = this._jiaoPoolRecordList.DecodeRecord(displayItem.RecordIndex, this._jiaoPoolRecordArgumentCollection).Item2.ColorReplace();
				titleBg.SetActive(false);
			}
		}
	}

	// Token: 0x040014B2 RID: 5298
	private JiaoPoolRecordList _jiaoPoolRecordList;

	// Token: 0x040014B3 RID: 5299
	private JiaoPoolRecordArgumentCollection _jiaoPoolRecordArgumentCollection;

	// Token: 0x040014B4 RID: 5300
	private readonly List<UI_JiaoPoolRecord.DisplayItem> _displayItems = new List<UI_JiaoPoolRecord.DisplayItem>();

	// Token: 0x040014B5 RID: 5301
	private bool _foldState = false;

	// Token: 0x040014B6 RID: 5302
	private Tweener _switchFoldTweener;

	// Token: 0x040014B7 RID: 5303
	private const float RecordPanelHeight = 350f;

	// Token: 0x0200135C RID: 4956
	private struct DisplayItem
	{
		// Token: 0x04009D8C RID: 40332
		public bool IsDateRow;

		// Token: 0x04009D8D RID: 40333
		public int RecordIndex;
	}
}
