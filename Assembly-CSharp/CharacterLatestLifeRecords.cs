using System;
using System.Collections.Generic;
using Config;
using GameData.DLC.FiveLoong;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.LifeRecord;
using GameData.Domains.LifeRecord.GeneralRecord;
using GameData.Domains.Map;
using GameData.Domains.Organization;
using GameData.Domains.Organization.Display;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200031C RID: 796
public class CharacterLatestLifeRecords : Refers, IAsyncMethodRequestHandler
{
	// Token: 0x1700051B RID: 1307
	// (get) Token: 0x06002E9E RID: 11934 RVA: 0x0017018C File Offset: 0x0016E38C
	// (set) Token: 0x06002E9F RID: 11935 RVA: 0x001701A4 File Offset: 0x0016E3A4
	public CharacterLatestLifeRecords.ERenderMode RenderMode
	{
		get
		{
			return this._renderMode;
		}
		set
		{
			this._renderMode = value;
			this.ClearLines();
		}
	}

	// Token: 0x06002EA0 RID: 11936 RVA: 0x001701B5 File Offset: 0x0016E3B5
	public void RegisterAsyncMethodCall(int requestId)
	{
		this._requestedAsyncMethods.Add(requestId);
	}

	// Token: 0x06002EA1 RID: 11937 RVA: 0x001701C8 File Offset: 0x0016E3C8
	public void ClearAsyncMethodCalls()
	{
		AsyncMethodDispatcher dispatcher = SingletonObject.getInstance<AsyncMethodDispatcher>();
		foreach (int one in this._requestedAsyncMethods)
		{
			dispatcher.UnregisterAsyncMethodCall(one);
		}
		this._requestedAsyncMethods.Clear();
	}

	// Token: 0x1700051C RID: 1308
	// (get) Token: 0x06002EA2 RID: 11938 RVA: 0x00170234 File Offset: 0x0016E434
	public bool IsRendering
	{
		get
		{
			return this._isRendering;
		}
	}

	// Token: 0x06002EA3 RID: 11939 RVA: 0x0017023C File Offset: 0x0016E43C
	private void Awake()
	{
		this.InitRefers();
	}

	// Token: 0x06002EA4 RID: 11940 RVA: 0x00170248 File Offset: 0x0016E448
	public void Setup()
	{
		this.InitRefers();
		bool flag = this._gameDataListenerId != -1;
		if (!flag)
		{
			this._gameDataListenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
		}
	}

	// Token: 0x06002EA5 RID: 11941 RVA: 0x00170286 File Offset: 0x0016E486
	public void Clear()
	{
		GameDataBridge.UnregisterListener(this._gameDataListenerId);
		this._gameDataListenerId = -1;
	}

	// Token: 0x06002EA6 RID: 11942 RVA: 0x0017029C File Offset: 0x0016E49C
	private void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 1)
			{
				bool flag = notification.DomainId == 13;
				if (flag)
				{
					bool flag2 = notification.MethodId == 2;
					if (flag2)
					{
						ReadonlyLifeRecords lifeRecords = new ReadonlyLifeRecords();
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref lifeRecords);
						this._context.FinishSelf(this, lifeRecords, delegate
						{
							bool flag3 = this._cache != null;
							if (flag3)
							{
								this._cache[this._renderingCharacterId] = this._context;
							}
							this.Render();
							this._isRendering = false;
						});
					}
				}
			}
		}
	}

	// Token: 0x06002EA7 RID: 11943 RVA: 0x00170360 File Offset: 0x0016E560
	private void InitRefers()
	{
		this._recordsLayout = base.CGet<VerticalLayoutGroup>("RecordsLayout");
		this._recordLineText = base.CGet<TextMeshProUGUI>("RecordLineText");
		this._noContent = base.CGet<GameObject>("NoContent");
		this._recordLineDateAndMessage1 = base.CGet<Refers>("RecordLineDateAndMessage1");
	}

	// Token: 0x06002EA8 RID: 11944 RVA: 0x001703B4 File Offset: 0x0016E5B4
	public bool Refresh(int characterId, int recordCount, Dictionary<int, CharacterLatestLifeRecords.CharacterRecordContext> cache = null)
	{
		bool flag = this._gameDataListenerId == -1;
		bool result;
		if (flag)
		{
			Debug.LogError("to early");
			result = false;
		}
		else
		{
			bool isRendering = this._isRendering;
			if (isRendering)
			{
				result = false;
			}
			else
			{
				this._isRendering = true;
				this._renderingCharacterId = characterId;
				bool flag2 = cache != null && cache.TryGetValue(characterId, out this._context);
				if (flag2)
				{
					this.Render();
					this._isRendering = false;
					result = true;
				}
				else
				{
					this._context = new CharacterLatestLifeRecords.CharacterRecordContext();
					this._cache = cache;
					this._context.StartRequest(this._gameDataListenerId, characterId, recordCount);
					result = true;
				}
			}
		}
		return result;
	}

	// Token: 0x06002EA9 RID: 11945 RVA: 0x00170450 File Offset: 0x0016E650
	private void Render()
	{
		List<CharacterLatestLifeRecords.RecordMessage> records = this._context.RenderAllRecords();
		this._noContent.SetActive(records.Count == 0);
		this._recordsLayout.gameObject.SetActive(records.Count > 0);
		for (int i = 0; i < records.Count; i++)
		{
			bool flag = i > this._recordsLayout.transform.childCount - 1;
			if (flag)
			{
				GameObject line = this.CreaetLine();
				line.SetActive(true);
				this.RefreshLine(line, records[i]);
			}
			else
			{
				Transform line2 = this._recordsLayout.transform.GetChild(i);
				line2.gameObject.SetActive(true);
				this.RefreshLine(line2.gameObject, records[i]);
			}
		}
		for (int j = records.Count; j < this._recordsLayout.transform.childCount; j++)
		{
			this._recordsLayout.transform.GetChild(j).gameObject.SetActive(false);
		}
	}

	// Token: 0x06002EAA RID: 11946 RVA: 0x00170574 File Offset: 0x0016E774
	private GameObject CreaetLine()
	{
		CharacterLatestLifeRecords.ERenderMode renderMode = this._renderMode;
		CharacterLatestLifeRecords.ERenderMode erenderMode = renderMode;
		GameObject result;
		if (erenderMode != CharacterLatestLifeRecords.ERenderMode.SimpleText)
		{
			if (erenderMode != CharacterLatestLifeRecords.ERenderMode.DateAndMessage1)
			{
				throw new ArgumentOutOfRangeException();
			}
			result = Object.Instantiate<GameObject>(this._recordLineDateAndMessage1.gameObject, this._recordsLayout.transform);
		}
		else
		{
			result = Object.Instantiate<GameObject>(this._recordLineText.gameObject, this._recordsLayout.transform);
		}
		return result;
	}

	// Token: 0x06002EAB RID: 11947 RVA: 0x001705DC File Offset: 0x0016E7DC
	private void ClearLines()
	{
		for (int i = 0; i < this._recordsLayout.transform.childCount; i++)
		{
			Object.Destroy(this._recordsLayout.transform.GetChild(i).gameObject);
		}
	}

	// Token: 0x06002EAC RID: 11948 RVA: 0x00170628 File Offset: 0x0016E828
	private void RefreshLine(GameObject line, CharacterLatestLifeRecords.RecordMessage record)
	{
		CharacterLatestLifeRecords.ERenderMode renderMode = this._renderMode;
		CharacterLatestLifeRecords.ERenderMode erenderMode = renderMode;
		if (erenderMode != CharacterLatestLifeRecords.ERenderMode.SimpleText)
		{
			if (erenderMode != CharacterLatestLifeRecords.ERenderMode.DateAndMessage1)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.RefreshLineDateAndMessage1(line, record);
		}
		else
		{
			this.RefreshLineSimpleText(line, record);
		}
	}

	// Token: 0x06002EAD RID: 11949 RVA: 0x00170668 File Offset: 0x0016E868
	private void RefreshLineDateAndMessage1(GameObject line, CharacterLatestLifeRecords.RecordMessage record)
	{
		Refers refers = line.GetComponent<Refers>();
		RectTransform rectTransform = line.GetComponent<RectTransform>();
		rectTransform.sizeDelta = new Vector2(rectTransform.parent.GetComponent<RectTransform>().rect.width, rectTransform.rect.height);
		RectTransform dot = refers.CGet<RectTransform>("Dot");
		TextMeshProUGUI date = refers.CGet<TextMeshProUGUI>("Date");
		TextMeshProUGUI message = refers.CGet<TextMeshProUGUI>("Message");
		date.text = record.Date + ": ";
		LayoutRebuilder.ForceRebuildLayoutImmediate(date.GetComponent<RectTransform>());
		message.text = record.Message;
		float totalWidth = refers.transform.GetComponent<RectTransform>().rect.width;
		float dotWidth = dot.rect.width;
		float dateWidth = date.rectTransform.rect.width;
		RectTransform messageRect = message.rectTransform;
		messageRect.sizeDelta = new Vector2(totalWidth - dotWidth - dateWidth, messageRect.sizeDelta.y);
		LayoutRebuilder.ForceRebuildLayoutImmediate(messageRect);
		message.GetComponent<TMPTextSpriteHelper>().Parse();
	}

	// Token: 0x06002EAE RID: 11950 RVA: 0x00170790 File Offset: 0x0016E990
	private void RefreshLineSimpleText(GameObject line, CharacterLatestLifeRecords.RecordMessage record)
	{
		TextMeshProUGUI label = line.GetComponent<TextMeshProUGUI>();
		label.text = string.Format(this.SimpleTextPattern, record.Date, record.Message);
		line.GetComponent<TMPTextSpriteHelper>().Parse();
	}

	// Token: 0x040021D6 RID: 8662
	private CharacterLatestLifeRecords.ERenderMode _renderMode;

	// Token: 0x040021D7 RID: 8663
	public string SimpleTextPattern = "· {0}: {1}";

	// Token: 0x040021D8 RID: 8664
	private readonly List<int> _requestedAsyncMethods = new List<int>();

	// Token: 0x040021D9 RID: 8665
	private int _gameDataListenerId = -1;

	// Token: 0x040021DA RID: 8666
	private Dictionary<int, CharacterLatestLifeRecords.CharacterRecordContext> _cache;

	// Token: 0x040021DB RID: 8667
	private bool _isRendering;

	// Token: 0x040021DC RID: 8668
	private int _renderingCharacterId;

	// Token: 0x040021DD RID: 8669
	private CharacterLatestLifeRecords.CharacterRecordContext _context;

	// Token: 0x040021DE RID: 8670
	private VerticalLayoutGroup _recordsLayout;

	// Token: 0x040021DF RID: 8671
	private TextMeshProUGUI _recordLineText;

	// Token: 0x040021E0 RID: 8672
	private GameObject _noContent;

	// Token: 0x040021E1 RID: 8673
	private Refers _recordLineDateAndMessage1;

	// Token: 0x020016A1 RID: 5793
	public enum ERenderMode
	{
		// Token: 0x0400A884 RID: 43140
		SimpleText,
		// Token: 0x0400A885 RID: 43141
		DateAndMessage1
	}

	// Token: 0x020016A2 RID: 5794
	public struct RecordMessage
	{
		// Token: 0x0400A886 RID: 43142
		public string Date;

		// Token: 0x0400A887 RID: 43143
		public string Message;
	}

	// Token: 0x020016A3 RID: 5795
	public class CharacterRecordContext
	{
		// Token: 0x0600D262 RID: 53858 RVA: 0x005AE9BC File Offset: 0x005ACBBC
		public CharacterRecordContext()
		{
			this._renderInfos = new List<LifeRecordRenderInfo>();
			this._argumentCollection = new ArgumentCollection();
			this._renderedArgCollection = new RenderedArgumentCollection();
			this._charIdList = new List<int>();
			this._charNameAndLifeDataList = new List<NameAndLifeRelatedData>();
			this._locationNames = new List<LocationNameRelatedData>();
			this._settlementNames = new List<SettlementNameRelatedData>();
			this._jiaoLoongNames = new List<JiaoLoongNameRelatedData>();
		}

		// Token: 0x0600D263 RID: 53859 RVA: 0x005AEA2C File Offset: 0x005ACC2C
		public List<CharacterLatestLifeRecords.RecordMessage> RenderAllRecords()
		{
			List<CharacterLatestLifeRecords.RecordMessage> result = new List<CharacterLatestLifeRecords.RecordMessage>();
			TimeManager timeManager = SingletonObject.getInstance<TimeManager>();
			foreach (LifeRecordRenderInfo renderInfo in this._renderInfos)
			{
				int year = renderInfo.Date / 12;
				int month = renderInfo.Date % 12;
				string date = LocalStringManager.GetFormat(LanguageKey.LK_Game_Time, new object[]
				{
					year + 1,
					month + 1,
					LocalStringManager.Get(string.Format("LK_Season_{0}", TimeKit.GetSeason(renderInfo.Date))),
					Month.Instance[month].Name
				});
				string message = GameMessageUtils.ParseRenderInfoToText(LifeRecord.Instance[renderInfo.RecordType].Desc, renderInfo, this._renderedArgCollection);
				result.Add(new CharacterLatestLifeRecords.RecordMessage
				{
					Date = date,
					Message = message
				});
			}
			return result;
		}

		// Token: 0x0600D264 RID: 53860 RVA: 0x005AEB54 File Offset: 0x005ACD54
		public void StartRequest(int listenerId, int characterId, int recordCount)
		{
			LifeRecordDomainMethod.Call.GetLast(listenerId, characterId, recordCount);
		}

		// Token: 0x0600D265 RID: 53861 RVA: 0x005AEB60 File Offset: 0x005ACD60
		public void FinishSelf(IAsyncMethodRequestHandler requestHandler, ReadonlyLifeRecords lifeRecords, Action onFinished)
		{
			CharacterLatestLifeRecords.CharacterRecordContext.<>c__DisplayClass11_0 CS$<>8__locals1 = new CharacterLatestLifeRecords.CharacterRecordContext.<>c__DisplayClass11_0();
			CS$<>8__locals1.onFinished = onFinished;
			CS$<>8__locals1.<>4__this = this;
			bool flag = lifeRecords.Count == 0;
			if (flag)
			{
				CS$<>8__locals1.onFinished();
			}
			lifeRecords.GetRenderInfos(this._renderInfos, this._argumentCollection);
			GameMessageUtils.RenderFixedArguments(this._argumentCollection, this._renderedArgCollection, false);
			this._charIdList.AddRange(this._argumentCollection.Characters);
			this._charIdList.AddRange(this._argumentCollection.CharacterRealNames);
			CS$<>8__locals1.renderPorgress = 0;
			bool flag2 = this._charIdList.Count > 0;
			if (flag2)
			{
				CharacterDomainMethod.AsyncCall.GetNameAndLifeRelatedDataList(requestHandler, this._charIdList, delegate(int offset, RawDataPool dataPool)
				{
					Serializer.Deserialize(dataPool, offset, ref CS$<>8__locals1.<>4__this._charNameAndLifeDataList);
					int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
					GameMessageUtils.RenderCharacterNames(taiwuCharId, CS$<>8__locals1.<>4__this._charNameAndLifeDataList, CS$<>8__locals1.<>4__this._argumentCollection, CS$<>8__locals1.<>4__this._renderedArgCollection, false);
					base.<FinishSelf>g__TryEnd|0();
				});
			}
			else
			{
				CS$<>8__locals1.<FinishSelf>g__TryEnd|0();
			}
			bool flag3 = this._argumentCollection.Locations.Count > 0;
			if (flag3)
			{
				MapDomainMethod.AsyncCall.GetLocationNameRelatedDataList(requestHandler, this._argumentCollection.Locations, delegate(int offset, RawDataPool dataPool)
				{
					Serializer.Deserialize(dataPool, offset, ref CS$<>8__locals1.<>4__this._locationNames);
					GameMessageUtils.RenderLocationNames(CS$<>8__locals1.<>4__this._locationNames, CS$<>8__locals1.<>4__this._renderedArgCollection);
					base.<FinishSelf>g__TryEnd|0();
				});
			}
			else
			{
				CS$<>8__locals1.<FinishSelf>g__TryEnd|0();
			}
			bool flag4 = this._argumentCollection.Settlements.Count > 0;
			if (flag4)
			{
				OrganizationDomainMethod.AsyncCall.GetSettlementNameRelatedData(requestHandler, this._argumentCollection.Settlements, delegate(int offset, RawDataPool dataPool)
				{
					Serializer.Deserialize(dataPool, offset, ref CS$<>8__locals1.<>4__this._settlementNames);
					GameMessageUtils.RenderSettlementNames(CS$<>8__locals1.<>4__this._settlementNames, CS$<>8__locals1.<>4__this._renderedArgCollection);
					base.<FinishSelf>g__TryEnd|0();
				});
			}
			else
			{
				CS$<>8__locals1.<FinishSelf>g__TryEnd|0();
			}
			bool flag5 = this._argumentCollection.JiaoLoongs.Count > 0;
			if (flag5)
			{
				ExtraDomainMethod.AsyncCall.GetJiaoLoongNameRelatedDataList(requestHandler, this._argumentCollection.JiaoLoongs, delegate(int offset, RawDataPool dataPool)
				{
					Serializer.Deserialize(dataPool, offset, ref CS$<>8__locals1.<>4__this._jiaoLoongNames);
					GameMessageUtils.RenderJiaoLoongNames(CS$<>8__locals1.<>4__this._jiaoLoongNames, CS$<>8__locals1.<>4__this._renderedArgCollection);
					base.<FinishSelf>g__TryEnd|0();
				});
			}
			else
			{
				CS$<>8__locals1.<FinishSelf>g__TryEnd|0();
			}
		}

		// Token: 0x0400A888 RID: 43144
		private List<LifeRecordRenderInfo> _renderInfos;

		// Token: 0x0400A889 RID: 43145
		private ArgumentCollection _argumentCollection;

		// Token: 0x0400A88A RID: 43146
		private RenderedArgumentCollection _renderedArgCollection;

		// Token: 0x0400A88B RID: 43147
		private List<int> _charIdList;

		// Token: 0x0400A88C RID: 43148
		private List<NameAndLifeRelatedData> _charNameAndLifeDataList;

		// Token: 0x0400A88D RID: 43149
		private List<LocationNameRelatedData> _locationNames;

		// Token: 0x0400A88E RID: 43150
		private List<SettlementNameRelatedData> _settlementNames;

		// Token: 0x0400A88F RID: 43151
		private List<JiaoLoongNameRelatedData> _jiaoLoongNames;
	}
}
